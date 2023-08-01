/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using LookAhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;
using System.Data;
using Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class LookAheadLauncher
    {
        Excel.Range SourceRange;
        Excel.Range Target;
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configMgr = ConfigurationManager.GetInstance;
        static Dictionary<string, List<string>> CachedData = new Dictionary<string, List<string>>();
        string LHFieldId = null; // retrieve the field from SS UI
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ApttusCommandBarManager commandbar = ApttusCommandBarManager.GetInstance();
        string LHSFieldNameAttribute = null;
        public LookAheadLauncher(Excel.Range SourceRng, Excel.Range Tgt)
        {
            SourceRange = SourceRng;
            Target = Tgt;
        }
        public LookAheadLauncher(Excel.Range Tgt)
        {
            Target = Tgt;
        }
        private LookAheadProperty FindActiveLHProp(LookAheadProperty prop, List<LookAheadProperty> props)
        {
            // find out active LH prop
            // 1. if the LH propoerty collection is empty but there is a LH prop set. this situation come from old versions
            // 2. if the LH Prop collection is not empty , find out the active LH prop
            // 3. if LH is null but collection is not empty, do step 2
            // check if the active prop is SS but user is offline
            // if offline and ss is set, check if Excel DS is set, if so lauch Excel Data source


            //check Online / offline

            //bool Online = Globals.ThisAddIn.oAuthWrapper.token != null && Globals.ThisAddIn.oAuthWrapper.token.access_token != null ? true : false;
            bool Online = commandbar.IsLoggedIn();

            if (prop == null && props == null) return prop;

            if (props == null || props.Count == 0 && prop != null)
            {
                return prop; // prior versions
            }

            if (props != null)
            {
                LookAheadProperty ActiveProp = props.Find(item => item.IsActive);
                if (ActiveProp != null)
                {
                    if (ActiveProp.DataSource.Equals(LookAheadDataSource.SearchAndSelect))
                    {
                        if (Online) return ActiveProp;
                        if (!Online)
                        {
                            // offline and  SS action prop - not a good choice, 
                            // check if there is a Excel DS set up
                            ActiveProp = props.Find(item => item.DataSource.Equals(LookAheadDataSource.Excel));
                            return ActiveProp;
                        }
                    }
                    else // Excel DS
                    {
                        return ActiveProp;

                    }
                }
                else // no active properties
                {

                }

            }
            return null;

        }
        public void LauchUI()
        {
            // find out the field named range in the repeating group range
            //Globals.ThisAddIn.Application.SendKeys("{ESC}", 0);

            int StartRow = SourceRange.Row;
            int SourceCol = Target.Column;
            // read values for repeating section only... 
            Excel.Range FieldRng = Target.Worksheet.Cells[StartRow + 1, SourceCol];
            // check for independant cells 
            Excel.Range IndFldRange = Target.Worksheet.Cells[StartRow, SourceCol];
            string NameRang = null;
            string NameRangeIndependent = null;
            try
            {
                NameRang = FieldRng.Name.Name;
            }
            catch (Exception ex)
            {
                // not a candidate for look ahead. 
                NameRang = null;
            }

            try
            {
                NameRangeIndependent = IndFldRange.Name.Name;

            }
            catch (Exception ex)
            {
                // not a candidate for look ahead. 
                NameRangeIndependent = null;
            }



            if (NameRang != null)
            {// goto repeating group and find out the field and see if there is a hook
                // for LH control
                //RetrieveField rFiled = configMgr.GetRetrieveFieldbyTargetNamedRange(FieldRng.Name.Name);
                RepeatingGroup Rgrp = configMgr.GetRepeatingGroupbyTargetNamedRange(SourceRange.Name.Name);

                if (Rgrp != null)
                {
                    RetrieveField rField = Rgrp.RetrieveFields.Find(item => item.TargetNamedRange.Equals(FieldRng.Name.Name));
                    LookAheadProperty ActiveProp = null;
                    if (rField != null)
                    {
                        LHFieldId = rField.FieldId;
                        // Get Exact Name field from App Object
                        ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
                        if (appObject != null)
                        {
                            ApttusField field = appObject.Fields.FirstOrDefault(f => f.Id == rField.FieldId);
                            if (field != null)
                            {
                                if (field.LookupObject != null)
                                    LHSFieldNameAttribute = field.LookupObject.NameAttribute;
                                else
                                    LHSFieldNameAttribute = field.Id;
                            }
                        }
                        if (rField.LookAheadProps == null && rField.LookAheadProp != null) // 3.x LH doesn't have props collection
                        {
                            ActiveProp = rField.LookAheadProp;

                        }
                        else if (rField.LookAheadProps.Count > 0 || rField.LookAheadProp != null)
                        {
                            // need to find the correct prop : see more comments in the FindActiveLHProp()
                            ActiveProp = FindActiveLHProp(rField.LookAheadProp, rField.LookAheadProps);

                        }
                        if (ActiveProp != null)
                            LauchUI(ActiveProp);

                    }
                }

                ShowSaveLHUI(SourceRange.Name.Name, FieldRng.Name.Name);

            }
            if (NameRangeIndependent != null)
            {
                RetrieveField fld = configMgr.GetRetrieveFieldbyTargetNamedRange(SourceRange.Name.Name);
                if (fld != null)
                {
                    LHFieldId = fld.FieldId;
                    if (fld.LookAheadProps != null && fld.LookAheadProps.Count > 0 || fld.LookAheadProp != null)
                    {
                        // Get Exact Name field from App Object
                        ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(fld.AppObject);
                        if (appObject != null)
                        {
                            ApttusField field = appObject.Fields.FirstOrDefault(f => f.Id == fld.FieldId);
                            if (field.LookupObject != null)
                                LHSFieldNameAttribute = field.LookupObject.NameAttribute;
                            else
                                LHSFieldNameAttribute = field.Id;
                        }
                        // need to find the correct prop : see more comments in the FindActiveLHProp()
                        LookAheadProperty ActiveProp = FindActiveLHProp(fld.LookAheadProp, fld.LookAheadProps);
                        if (ActiveProp != null)
                            LauchUI(ActiveProp);
                    }
                }

            }
            ShowSaveLHUI(SourceRange.Name.Name, NameRangeIndependent);
        }

        private void ShowSaveLHUI(string srcRng, string FldRng)
        {
            SaveMap SMap = configMgr.GetSaveMapbyTargetNamedRange(srcRng);
            if (SMap != null)
            {
                SaveField sField = SMap.SaveFields.Find(item => item.TargetNamedRange.Equals(FldRng));
                if (sField != null)
                {
                    LHFieldId = sField.FieldId;
                    if (sField.LookAheadProp != null ||
                        sField.LookAheadProps != null && sField.LookAheadProps.Count > 0)
                    {

                        LookAheadProperty ActiveProp = FindActiveLHProp(sField.LookAheadProp, sField.LookAheadProps);
                        if (ActiveProp != null)
                            LauchUI(ActiveProp);
                    }
                }
            }
        }

        public void LauchUI(LookAheadProperty prop)
        {

            List<string> SourceList = null;
            if (prop.DataSource.Equals(LookAheadDataSource.Excel))
            {
                //  string location = ((ExcelDataSource)rField.LookAheadProp).SheetReference;
                string Tglocation = ((ExcelDataSource)prop).TargetRange;
                bool RefreshData = ((ExcelDataSource)prop).RefreshData;

                if (Tglocation != null)
                {
                    Excel.Range rng = ExcelHelper.GetRange(Tglocation);
                    if (prop.GetType() == typeof(ExcelMultiColumProp))
                    {
                        // ((ExcelMultiColumProp)prop.DSource).SheetReference
                        LookAheadMultiColHelper Helper = new LookAheadMultiColHelper(rng, Target, prop as ExcelMultiColumProp, RefreshData, Tglocation);
                        Helper.DisplayUI();
                        return;
                    }

                    string id = MetadataManager.GetInstance.GetAppUniqueId();
                    string key = id + "|" + Tglocation;
                    if (CachedData.ContainsKey(key) && !RefreshData)
                    {
                        SourceList = CachedData[key];
                    }
                    else
                    {
                        Array SourceData = (Array)rng.Cells.Value2;
                        // exlude null values
                        string[] strArray = SourceData.OfType<string>().Select(o => o.ToString()).ToArray();
                        if (strArray.Length > 0)
                        {
                            SourceList = strArray.ToList();
                            if (CachedData.ContainsKey(key))
                            {
                                CachedData.Remove(key);
                            }
                            CachedData.Add(key, SourceList);
                        }
                    }

                    if (SourceList != null)
                    {
                        LookAheadSupport LH = new LookAheadSupport();
                        LH.Display(Target, SourceList);
                    }

                }
            }
            else if (prop.DataSource.Equals(LookAheadDataSource.SearchAndSelect))
            {
                // find the ss action and then execute the action
                try
                {
                    SSActionDataSource ssDataSource = prop as SSActionDataSource;
                    string DataSourceName = ssDataSource.ActionID + "_LOOKAHEAD";
                    string sActionId = ssDataSource.ActionID;
                    Core.Action objAction = configMgr.GetActionById(sActionId);
                    if (objAction != null)
                    {
                        if (objAction.GetType() == typeof(Core.SearchAndSelect))
                        {
                            SearchAndSelect objSSAction = (Core.SearchAndSelect)objAction;

                            // Create object S&S runtime class and call execute method.
                            SearchAndSelectActionController ssAction = new SearchAndSelectActionController(objSSAction, null);
                            //if the output persist is not set, dataset wont be added in the data manager

                            ssAction.OutputPersistData = true;
                            ssAction.OutputDataName = DataSourceName;
                            ActionResult res = ssAction.Execute();
                            if (res.Status == ActionResultStatus.Success)
                            {
                                string[] DsNames = { DataSourceName };
                                List<ApttusDataSet> DS = DataManager.GetInstance.GetDatasetsByNames(DsNames);
                                ApttusDataSet Dataset = DS.Count > 0 ? DS[0] : null;

                                if (Dataset != null && LHFieldId != null && ssDataSource.ObjectMapping == null)
                                {// get the field name. 
                                    
                                    // the data table is expected to have a single row hence hardcoded with 0 index
                                    if (LHFieldId.Contains("."))
                                    {
                                        // if the field has parentname.fieldId then the data set will not have parent name along with the field name
                                        // for example, if the field id is Name that come from Account, Data table for Account object will have name
                                        // and if the account name is from the opp retr
                                        string[] FieldName = LHFieldId.Split('.');
                                        LHFieldId = FieldName[FieldName.Length - 1];
                                    }
                                    string sValue = null;
                                    if (Dataset.DataTable.Columns.Contains(LHFieldId))
                                        sValue = Dataset.DataTable.Rows[0][LHFieldId].ToString();
                                    else if (!string.IsNullOrEmpty(LHSFieldNameAttribute))
                                    {
                                        if (Dataset.DataTable.Columns.Contains(LHSFieldNameAttribute)) // For Dynamics CRM Name
                                            sValue = Dataset.DataTable.Rows[0][LHSFieldNameAttribute].ToString();
                                    }

                                    string IdField = Dataset.DataTable.Rows[0][Core.Constants.ID_ATTRIBUTE].ToString();
                                    if (Target != null && sValue != null) Target.Value2 = sValue;
                                    // if the ID field map to another column.
                                    // read the column info from the prop
                                    // and set the value 
                                    /******************/
                                    ReturnDataCollection RetData = ((SSActionDataSource)prop).ReturnColumnData;
                                    if (RetData != null && RetData.DataCollection != null && RetData.DataCollection.Count > 0)
                                    {
                                        // update data into other cells from the return data collection
                                        // the col index will be available for each data and row can be determind
                                        // from the TargetValue

                                        foreach (ReturnColumnData retResult in RetData.DataCollection)
                                        {
                                            int TargetCol; // = retResult.TargetColumn;
                                            int TargetRow;
                                            Range TargetRange;
                                            if (!string.IsNullOrEmpty(retResult.TargetNR))
                                            {
                                                TargetRange = ExcelHelper.GetRange(retResult.TargetNR);
                                                // get the same row and col for independent
                                                if (retResult.FldType == ObjectType.Independent)
                                                {
                                                    TargetCol = TargetRange.Column;
                                                    TargetRow = TargetRange.Row;
                                                }
                                                else // repeating needs to take the Target Range row and Target Value col because the row will remain the same 
                                                {
                                                    TargetCol = TargetRange.Column;
                                                    TargetRow = Target.Row;
                                                }

                                                Range DestRange = TargetRange.Application.Cells[TargetRow, TargetCol];
                                                DestRange.Value2 = IdField;
                                            }
                                        }
                                    }
                                }
                                else if (Dataset != null && Dataset.DataTable != null && ssDataSource.ObjectMapping != null)
                                {
                                    //1. Check whether all the mappedfields exists in the datatable as columns
                                    bool bAllColumnsExists = true;
                                    foreach (MappedField field in ssDataSource.ObjectMapping.MappedFields)
                                    {
                                        if (!Dataset.DataTable.Columns.Contains(field.SourceFieldId))
                                        {
                                            ExceptionLogHelper.DebugLog("Mapping Failed. " + field.SourceFieldId + " doesn't exist in DataTable");
                                            bAllColumnsExists = false;
                                        }
                                    }

                                    if (!bAllColumnsExists)
                                    {
                                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKAHDLAUNCH_LauchUI_InfoMsg"), resourceManager.GetResource("LOOKAHDLAUNCH_LauchUICAP_InfoMsg"), Globals.ThisAddIn.Application.Hwnd);
                                        return;
                                    }

                                    string targetNamedRange = ssDataSource.ObjectMapping.RepeatingGroupTargetNamedRange;
                                    Excel.Range namedRange = ExcelHelper.GetRange(targetNamedRange);

                                    Excel.Range selection = ExcelHelper.ExcelApp.Application.Selection;



                                    RepeatingGroup repeatingGroup = (from rm in configMgr.RetrieveMaps
                                                                     from rg in rm.RepeatingGroups.Where(r => r.TargetNamedRange.Equals(targetNamedRange))
                                                                     select rg).FirstOrDefault();


                                    int columnCount = namedRange.Columns.Count;
                                    int rowCount = Dataset.DataTable.Rows.Count;
                                    int startRow;


                                    //if Search & Select action returns multiple records then add records with new rows else just replace current record.
                                    if (objSSAction.RecordType.Equals(Apttus.XAuthor.Core.Constants.QUERYRESULT_MULTIPLE))
                                    {
                                        //startRow indicates the row position within the RepeatingGroup TargetRange
                                        startRow = selection.Row - (namedRange.Cells[1, 1] as Excel.Range).Row + 2; //Why add 2. Add 2 to skip the header row and formula row.

                                        //2. Add Number of rows present in the dataset.
                                        RuntimeEditActionController controller = new RuntimeEditActionController(selection, namedRange);
                                        if (!controller.AddRow(rowCount))
                                            return;
                                    }
                                    else
                                    {
                                        //startRow indicates the row position within the RepeatingGroup TargetRange
                                        startRow = selection.Row - (namedRange.Cells[1, 1] as Excel.Range).Row + 1; //Why add 3. Add 3 to skip the header row and formula row and replace data in current row.
                                    }

                                    object[,] data;
                                    Excel.Worksheet sheet = namedRange.Worksheet;

                                    int rowIndex = 0;

                                    // Initialise data object with existing values in excel, so only mapping fields will get chaged and remaining will stay untouched.
                                    Excel.Range dataRange = sheet.Range[namedRange.Cells[startRow, 1], namedRange.Cells[startRow + rowCount - 1, columnCount]];
                                    data = dataRange.Value;

                                    if (data == null)
                                        return;

                                    foreach (DataRow row in Dataset.DataTable.Rows)
                                    {
                                        ++rowIndex;
                                        foreach (MappedField field in ssDataSource.ObjectMapping.MappedFields)
                                        {
                                            RetrieveField rf = repeatingGroup.RetrieveFields.Where(r => r.FieldId.Equals(field.TargetFieldId)).FirstOrDefault();
                                            int columnIndex = rf.TargetColumnIndex;
                                            data[rowIndex, columnIndex] = row[field.SourceFieldId];
                                        }
                                    }

                                    dataRange.Value = data;
                                    data = null; //Important
                                }
                            } //  no need to handle failure case. appropriate mssage will be displayed by the action
                        }
                    }

                }
                catch (Exception ex)
                {
                    ApttusMessageUtil.ShowError(ex.Message.ToString(), resourceManager.GetResource("LookAheadLauncher_LauchUI_Cap_ErrorMsg"));
                }
            }
        }
    }

    public class LookAheadMultiColHelper
    {
        Excel.Range SourceRng;
        Excel.Range TargetRng;
        ExcelMultiColumProp mProp;
        private static Dictionary<string, System.Data.DataTable> CachedData = new Dictionary<string, System.Data.DataTable>();
        private string mTargeLocation;
        bool bRefreshData;
        public LookAheadMultiColHelper(Excel.Range source, Excel.Range Target, ExcelMultiColumProp prop, bool RefreshData, string TargeLocation)
        { // read the source data and load it in the dataTable
            SourceRng = source;
            TargetRng = Target;
            mProp = prop;
            mTargeLocation = TargeLocation;
            // CachedData = new Dictionary<string, DataTable>();
            bRefreshData = RefreshData;
        }
        public System.Data.DataTable PopulateDT()
        {
            System.Data.DataTable changedDataTable = null;
            if (SourceRng == null) return changedDataTable;
            ExcelHelper.RangeToDataTable(SourceRng, ref changedDataTable, LayoutType.Vertical);
            return changedDataTable;
        }
        public void DisplayUI()
        {
            LooAheadMultiColSupportHelper LH = new LooAheadMultiColSupportHelper();
            System.Data.DataTable DT = PopulateDataFromRange();
            LH.DisplayMultiCol(TargetRng, DT, mProp);
        }

        public System.Data.DataTable PopulateDataFromRange()
        {

            string id = MetadataManager.GetInstance.GetAppUniqueId();
            string key = id + "|" + mTargeLocation;
            if (CachedData.ContainsKey(key) && !bRefreshData)
            {
                return CachedData[key];
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            int StartRow = SourceRng.Row;
            int StartCol = SourceRng.Column;
            string ColValue = null;

            foreach (Microsoft.Office.Interop.Excel.Range rng in SourceRng.Columns)
            {

                ColValue = rng.Value2[1, 1] as string;
                if (ColValue == null)
                    dt.Columns.Add(ExcelHelper.NumberToColum(rng.Column));
                else
                    dt.Columns.Add((ColValue));
                //Cols.Add(NumberToColum(rng.Column));
            }


            object[,] objArray = (object[,])SourceRng.Value2;

            int columnCount = SourceRng.Columns.Count;

            // Add columns to datatable
            for (int col = 1; col <= columnCount; col++)
            {
                var value = objArray[1, col];
                string colName = value != null ? value.ToString() : string.Empty;
                colName += col.ToString();

                // dt.Columns.Add(colName);
            }

            try
            {
                // Add rows to datatable
                dt.BeginLoadData();
                int row, col;
                object[] data = new object[columnCount];
                for (row = 2; row <= SourceRng.Rows.Count; ++row)
                {
                    for (col = 0; col < columnCount; ++col)
                        data[col] = objArray[row, col + 1];
                    dt.LoadDataRow(data, false);
                }
            }
            finally
            {
                dt.EndLoadData();
            }
            if (CachedData.ContainsKey(key))
            {
                CachedData.Remove(key);
            }
            CachedData.Add(key, dt);
            return dt;

        }
    }
}
