/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class ClearActionController
    {
        // Model
        private ClearActionModel Model;
        public ActionResult Result { get; private set; }
        private DataManager dataManager = DataManager.GetInstance;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ClearActionController(ClearActionModel model)
        {
            this.Model = model;
            Result = new ActionResult();
        }

        //private void ClearSaveGroup(SaveGroup saveGroup)
        //{
        //    int nSkipFormulaRowIndex = 2;
        //    Excel.Range saveGroupDataRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);
        //    Excel.Worksheet sheet = saveGroupDataRange.Worksheet;

        //    if (saveGroupDataRange.Rows.Count > nSkipFormulaRowIndex)
        //    {
        //        //Skip the Formula Row
        //        Excel.Range firstCell = ExcelHelper.NextVerticalCell(saveGroupDataRange, nSkipFormulaRowIndex);
        //        Excel.Range lastCell = ExcelHelper.NextVerticalCell(ExcelHelper.NextHorizontalCell(saveGroupDataRange, saveGroupDataRange.Columns.Count - 1), saveGroupDataRange.Rows.Count - 1);

        //        Excel.Range rangeToClear = sheet.Range[firstCell, lastCell];
                
        //        //Clear All Data
        //        rangeToClear.ClearContents();
                
        
        //        //Delete All Rows
        //        rangeToClear.EntireRow.Rows.Delete();
        //    }
        //}

        private void ClearIndependentSaveField(SaveField saveField)
        {
            Excel.Range saveFieldRange = ExcelHelper.GetRange(saveField.TargetNamedRange);
            try
            {
                if(!saveFieldRange.HasFormula)
                    saveFieldRange.ClearContents();
            }
            catch (Exception ex)
            {

            }            
        }

        private void ClearSaveMap()
        {
            SaveMap saveMap = configManager.SaveMaps.Where(sm => sm.Id.Equals(Model.SaveMapId)).FirstOrDefault();

            //Clear Independent Fields
            List<SaveField> independentSaveFields = saveMap.SaveFields.Where(sf => sf.Type == ObjectType.Independent).ToList();

            foreach (SaveField independentSaveField in independentSaveFields)
            {
                // Do Clear Excel Contents, For all Independent fields
                ClearIndependentSaveField(independentSaveField);

                ApttusDataSet dataSet = dataManager.GetDataSetFromLocation(independentSaveField.TargetNamedRange);
                if (dataSet != null)
                {
                    // Do remove ApttusDataSet
                    dataManager.RemoveDataSet(dataSet);
                }
            }

            //Clear Save Group
            foreach (SaveGroup saveGroup in saveMap.SaveGroups)
            {
                ApttusDataSet dataSet = dataManager.GetDataSetFromLocation(saveGroup.TargetNamedRange);
                if (dataSet != null)
                {
                    // Do Clear Excel Contents
                    //ClearSaveGroup(saveGroup);
                    if (saveGroup.LoadedRows == 0)
                    {
                        ExcelHelper.ClearRepeatingCells(saveGroup.TargetNamedRange, saveGroup.AppObject);

                        // Do remove ApttusDataSet
                        dataManager.RemoveDataSet(dataSet);
                    }
                    else if (saveGroup.LoadedRows > 0) //For Pre-loaded Grid
                    {
                        int skip2Rows = 2; //1. First row is label row. 2. Second row is formula row. to get the actual records skip those 2 rows and we get the number of records
                        Excel.Range preloadedRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);
                        if ((preloadedRange.Rows.Count - skip2Rows) >= saveGroup.LoadedRows)
                        {
                            try
                            {

                                int nRowsToClear = (preloadedRange.Rows.Count - skip2Rows) - saveGroup.LoadedRows;
                                if (nRowsToClear > 0)
                                {
                                    for (int iRowIndex = (saveGroup.LoadedRows + nRowsToClear - 1); iRowIndex >= saveGroup.LoadedRows; --iRowIndex)
                                        dataSet.DataTable.Rows.RemoveAt(iRowIndex);

                                    Excel.Range firstCell = preloadedRange.Cells[saveGroup.LoadedRows + skip2Rows + 1, 1] as Excel.Range;
                                    Excel.Range lastCell = preloadedRange.Cells[preloadedRange.Rows.Count, preloadedRange.Columns.Count];
                                    Excel.Worksheet sheet = firstCell.Worksheet;
                                    Excel.Range range = sheet.Range[firstCell, lastCell];
                                    range.Delete();
                                }

                                //Now Clear the ID Field within the preloadedRange.
                                List<SaveField> fields = saveMap.SaveFields.Where(sf => sf.GroupId.Equals(saveGroup.GroupId)).ToList();
                                SaveField IDField = fields.Find(sf => sf.FieldId.Equals(Constants.ID_ATTRIBUTE));
                                if (IDField != null)
                                {
                                    preloadedRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);

                                    //First Clear all the rows in the datatable.
                                    int nRows = dataSet.DataTable.Rows.Count;
                                    dataSet.DataTable.Rows.Clear();

                                    //Make the parent dataset empty.
                                    dataSet.Parent = Guid.Empty;

                                    //New insert exact same number of empty rows which were there previously, so that when the user saves again the number of rows 
                                    //matches the number of rows in excel.
                                    for(int i = 0; i < nRows;  ++i)
                                    {
                                        DataRow newRow = dataSet.DataTable.NewRow();
                                        dataSet.DataTable.Rows.Add(newRow);
                                    }

                                    //Clear the FieldIndex column in Excel.
                                    int IDFieldIndex = IDField.TargetColumnIndex;
                                    Excel.Range idFieldRange = preloadedRange.Columns[IDFieldIndex];
                                    Excel.Range firstCellOfColumn = idFieldRange.Cells[skip2Rows+1, 1];
                                    Excel.Range lastCellOfColumn = idFieldRange.Cells[idFieldRange.Rows.Count, 1];
                                    Excel.Worksheet sheet = firstCellOfColumn.Worksheet;
                                    Excel.Range idRangeToClear = sheet.Range[firstCellOfColumn, lastCellOfColumn];
                                    idRangeToClear.ClearContents();
                                }
                            }
                            catch (Exception ex)
                            {
                                dataSet.DataTable.RejectChanges();
                                ExceptionLogHelper.ErrorLog(ex);
                            }
                            finally
                            {
                                dataSet.DataTable.AcceptChanges();
                            }
                        }
                        else if ((preloadedRange.Rows.Count - skip2Rows) != saveGroup.LoadedRows)
                        {
                            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CLEARACTCTL_ClearSaveMap_InfoMsg"), resourceManager.GetResource("CLEARACTCTL_ClearSaveMapCAP_InfoMsg"));
                        }
                    }
                }
            }
        }

        private void ClearRetrieveMap()
        {
            RetrieveMap rMap = configManager.RetrieveMaps.Where(rm => rm.Id.Equals(Model.SaveMapId)).FirstOrDefault();
            foreach (Guid AppObjectId in Model.AppObjects)
            {
                List<RetrieveField> independentFields = rMap.RetrieveFields.Where(rf => rf.AppObject.Equals(AppObjectId)).ToList();
                if (independentFields.Count > 0)
                {
                    //Clear Independent Fields
                    foreach (RetrieveField independentField in independentFields)
                    {
                        Excel.Range independentDataRange = ExcelHelper.GetRange(independentField.TargetNamedRange);
                        independentDataRange.ClearContents();
                    }

                    ApttusDataSet dataSet = dataManager.GetDataSetFromLocation(independentFields[0].TargetNamedRange);
                    if (dataSet != null)
                    {
                        dataSet.DataTable.Rows.Clear();
                        dataSet.DataTable.AcceptChanges();
                    }
                }
                else
                {
                    //Clear Repeating Fields of RepeatingGroup 
                    RepeatingGroup repeatingGroup = rMap.RepeatingGroups.Where(rg => rg.AppObject.Equals(AppObjectId)).FirstOrDefault();
                    if (repeatingGroup != null)
                    {
                        ExcelHelper.ClearRepeatingCells(repeatingGroup.TargetNamedRange, repeatingGroup.AppObject);

                        ApttusDataSet dataSet = dataManager.GetDataSetFromLocation(repeatingGroup.TargetNamedRange);
                        if (dataSet != null)
                        {
                            dataSet.DataTable.Rows.Clear();
                            dataSet.DataTable.AcceptChanges();
                        }
                    }
                }
            }
        }

        public ActionResult Execute()
        {

            HashSet<Excel.Worksheet> sheets = new HashSet<Excel.Worksheet>();
            try
            {
                sheets = GetWorksheets();
                foreach (Excel.Worksheet sheet in sheets)
                {
                    if (ExcelHelper.IsSheetProtectedByUser(sheet))
                    {
                        ExcelHelper.UserSheetProtection(sheet, false);
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Clear Action");
                Result.Status = ActionResultStatus.Failure;
            }
            try
            {
                Result.Status = ActionResultStatus.Pending_Execution;
                if (Model.DisableExcelEvents)
                    ExcelHelper.ExcelApp.EnableEvents = false;

                switch (Model.MapType)
                {
                    case ClearActionMapType.SaveMap:
                        ClearSaveMap();
                        break;
                    case ClearActionMapType.DisplayMap:
                        ClearRetrieveMap();
                        break;
                }

                Result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Clear Action");
                Result.Status = ActionResultStatus.Failure;
            }
            finally
            {
                if (Model.DisableExcelEvents)
                    ExcelHelper.ExcelApp.EnableEvents = true;

                // Apply Data Protection after clear data
                foreach (Excel.Worksheet sheet in sheets)
                {
                    if (ExcelHelper.IsSheetProtectedByUser(sheet))
                    {
                        ExcelHelper.UserSheetProtection(sheet, true);
                    }
                }
            }
            return Result;
        }

        private HashSet<Excel.Worksheet> GetWorksheets()
        {
            HashSet<string> sheetNames = new HashSet<string>();

            // Remove Data Protection before clear data
            if (Model.MapType.Equals(ClearActionMapType.SaveMap))
            {
                SaveMap saveMap = configManager.SaveMaps.Where(sm => sm.Id.Equals(Model.SaveMapId)).FirstOrDefault();
                saveMap.SaveFields.ForEach(t => sheetNames.Add(t.DesignerLocation.Split('!')[0]));
            }
            else if (Model.MapType.Equals(ClearActionMapType.DisplayMap))
            {
                RetrieveMap rMap = configManager.RetrieveMaps.Where(rm => rm.Id.Equals(Model.SaveMapId)).FirstOrDefault();
                rMap.RetrieveFields.ForEach(t => sheetNames.Add(t.TargetLocation.Split('!')[0]));
                foreach (RepeatingGroup rg in rMap.RepeatingGroups)
                {
                    string sheetName = ExcelHelper.GetRange(rg.TargetNamedRange).Worksheet.Name;
                    sheetNames.Add(sheetName);
                }
            }
            HashSet<Excel.Worksheet> _sheets = new HashSet<Excel.Worksheet>();
            foreach (string sheetName in sheetNames)
            {
                _sheets.Add(ExcelHelper.GetWorkSheet(sheetName));
            }
            return _sheets;
        }
    }
}
