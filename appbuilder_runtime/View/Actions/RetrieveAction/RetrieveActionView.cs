/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using VSTO = Microsoft.Office.Tools.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    struct AppObjectMap
    {
        public Guid AppObject { get; set; }
        public ObjectType Type { get; set; }
    }

    public struct ExcelGroupData
    {
        public int StartRow;
        public int RowCount;
        public int ParentGroupId;
    }

    struct TotalCellData
    {
        public Excel.Range TotalRow;
        public Excel.Range TotalsRange;
    };

    class ThreadData
    {
        public ApttusDataSet apttusDataSet { get; set; }
        public DataSet dataSet { get; set; }
        public ObjectType type { get; set; }
    };

    public class RetrieveActionView
    {
        // Get DataManager and ConfigurationManager instance
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        AttachmentsDataManager attachmentManager = AttachmentsDataManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        BackgroundWorker Bgworker = new BackgroundWorker();
        WaitWindowView waitWindow;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// Fill Data
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public void FillData(RetrieveActionModel retrieveAction, RetrieveMap Model, bool InputData, string[] InputDataNames)
        {
            try
            {
                ExcelHelper.ExcelApp.EnableEvents = false;
                bool bShowDisplayFilters = retrieveAction.ShowQueryFilterObjectConfigurations != null && retrieveAction.ShowQueryFilterObjectConfigurations.Count > 0;

                InitializeBackgroundWorker();
                IEnumerable<AppObjectMap> uniqueAppObjectsInMap = Model.RetrieveFields.Select(s => new AppObjectMap { AppObject = s.AppObject, Type = s.Type }).Distinct();
                uniqueAppObjectsInMap = uniqueAppObjectsInMap.Union(Model.RepeatingGroups.Select(s => new AppObjectMap { AppObject = s.AppObject, Type = ObjectType.Repeating }).Distinct());

                // Remove Data Protection before clear data
                foreach (var WorksheetName in dataManager.DataProtection.Select(dp => dp.WorksheetName).Distinct())
                {
                    Excel.Worksheet sheet = ExcelHelper.GetWorkSheet(WorksheetName);
                    ExcelHelper.UpdateSheetProtection(sheet, false);
                }

                foreach (var uniqueAppObj in uniqueAppObjectsInMap)
                {
                    ApttusObject appObject = applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject);
                    ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(InputDataNames, appObject);

                    switch (uniqueAppObj.Type)
                    {
                        case ObjectType.Independent:
                            {
                                if (dataSet != null && dataSet.DataTable != null && dataSet.DataTable.Rows.Count > 0)
                                {
                                    MapIndependentCells(Model, uniqueAppObj.AppObject, dataSet);

                                    // If attachment type is include retrieve fields then execute get attachments method
                                    if (Model.RetrieveFields.Where(t => t.DataType == Datatype.Attachment).ToList().Count > 0)
                                        ProcessAttachments(dataSet);

                                }
                                else
                                {
                                    // Suppress Message as per user's app settings
                                    if (configurationManager.Definition.AppSettings != null)
                                    {
                                        if (!configurationManager.Definition.AppSettings.SuppressNoOfRecords)
                                            ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("RETRIEVEACTVIEW_FillData_InfoMsg"), appObject.Name), resourceManager.GetResource("RETRIEVEACTVIEW_FillDataCAP_InfoMsg"));
                                    }
                                    else
                                        ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("RETRIEVEACTVIEW_FillData_InfoMsg"), appObject.Name), resourceManager.GetResource("RETRIEVEACTVIEW_FillDataCAP_InfoMsg"));
                                }
                            }
                            break;
                        case ObjectType.Repeating:
                            {
                                if (dataSet != null && dataSet.DataTable != null && dataSet.DataTable.Rows.Count > 0)
                                {
                                    RepeatingGroup repGroup = (from rg in Model.RepeatingGroups where rg.AppObject == uniqueAppObj.AppObject select rg).FirstOrDefault();
                                    if (repGroup != null && IsGroupByFieldVisible(repGroup) && IsSortByFieldForVisible(repGroup))
                                    {
                                        MapRepeatingCells(repGroup, dataSet, repGroup.TargetNamedRange);

                                        if (dataSet != null)
                                            dataSet.Parent = dataManager.GetParentDataSetId(repGroup.AppObject);

                                        // If attachment type is include retrieve fields then execute get attachments method
                                        if (repGroup.RetrieveFields.Where(t => t.DataType == Datatype.Attachment).ToList().Count > 0)
                                            ProcessAttachments(dataSet);
                                    }
                                }
                                else if (dataSet != null && dataSet.DataTable != null && dataSet.DataTable.Rows.Count == 0)
                                {
                                    // Suppress Message as per user's app settings
                                    if (configurationManager.Definition.AppSettings != null)
                                    {
                                        if (!configurationManager.Definition.AppSettings.SuppressNoOfRecords)
                                            ApttusMessageUtil.ShowInfo(String.Format(resourceManager.GetResource("RETRIEVEACTVIEW_FillData_InfoMsg"), appObject.Name), resourceManager.GetResource("RETRIEVEACTVIEW_FillDataCAP_InfoMsg"));
                                    }
                                    else
                                        ApttusMessageUtil.ShowInfo(String.Format(resourceManager.GetResource("RETRIEVEACTVIEW_FillData_InfoMsg"), appObject.Name), resourceManager.GetResource("RETRIEVEACTVIEW_FillDataCAP_InfoMsg"));

                                }
                            }
                            break;
                    }
                    if (bShowDisplayFilters)
                        DisplayFilters(retrieveAction, dataSet, appObject.UniqueId);
                }

                // Apply Data Protection after clear data
                foreach (var WorksheetName in dataManager.DataProtection.Select(dp => dp.WorksheetName).Distinct())
                {
                    Excel.Worksheet sheet = ExcelHelper.GetWorkSheet(WorksheetName);
                    ExcelHelper.UpdateSheetProtection(sheet, true);
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Display Action");
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        private void DisplayFilters(RetrieveActionModel retrieveAction, ApttusDataSet dataSet, Guid UniqueId)
        {
            ShowQueryFilterObjectConfiguration obj = retrieveAction.ShowQueryFilterObjectConfigurations.Find(o => o.AppObjectUniqueId == UniqueId);
            if (obj != null)
            {
                Excel.Range cellLocation = null;
                switch (obj.CellReferenceType)
                {
                    case CellReferenceType.CellLocation:
                        cellLocation = ExcelHelper.GetRangeByLocation(obj.CellReferenceValue);
                        break;
                    case CellReferenceType.NamedRange:
                        cellLocation = ExcelHelper.GetRange(obj.CellReferenceValue);
                        break;
                }

                if (cellLocation != null)
                {
                    if (string.IsNullOrEmpty(dataSet.DisplayableWhereClause))
                        cellLocation.Value = string.Empty;
                    else if (dataSet.DisplayableWhereClause != null && dataSet.DisplayableWhereClause.Length < Constants.EXCEL_CELL_LENGTH)
                        cellLocation.Value = dataSet.DisplayableWhereClause;
                    else
                    {
                        cellLocation.Value = resourceManager.GetResource("DISPLAYACTION_ShowFilterLength_Exceeded_Msg");
                        ExceptionLogHelper.DebugLog("Query Filter exceeded excel limit for a cell. Filter : " + dataSet.DisplayableWhereClause);
                    }
                }
                else
                    ExceptionLogHelper.DebugLog("Query Filter exceeded excel limit for a cell. Filter : " + dataSet.DisplayableWhereClause);
            }
        }

        private bool IsGroupByFieldVisible(RepeatingGroup repGroup)
        {
            string groupByField = string.Empty;
            bool bOK = true;
            if (!string.IsNullOrEmpty(repGroup.GroupByField))
            {
                RetrieveField groupByRetField = repGroup.RetrieveFields.Where(f => f.FieldId.Equals(repGroup.GroupByField)).FirstOrDefault();
                if (groupByRetField != null && !fieldLevelSecurity.IsFieldVisible(groupByRetField.AppObject, repGroup.GroupByField, repGroup.AppObject.ToString()))
                {
                    groupByField = repGroup.GroupByField;
                    bOK = false;
                }
            }
            else if (!string.IsNullOrEmpty(repGroup.GroupByField2))
            {
                RetrieveField groupBy2RetField = repGroup.RetrieveFields.Where(f => f.FieldId.Equals(repGroup.GroupByField2)).FirstOrDefault();
                if (groupBy2RetField != null && !fieldLevelSecurity.IsFieldVisible(groupBy2RetField.AppObject, repGroup.GroupByField2, repGroup.AppObject.ToString()))
                {
                    groupByField = repGroup.GroupByField2;
                    bOK = false;
                }
            }
            if (!bOK)
            {
                string fieldName = repGroup.RetrieveFields.Find(rf => rf.FieldId.Equals(groupByField)).FieldName;
                //ApttusMessageUtil.ShowInfo("Group by field : " + fieldName + " is in-visible to the logged-in user. Hence cannot render the records.", "Field Level Security");
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("RETRIEVEACTVIEW_IsGroupByFieldVisible_InfoMsg"), fieldName, Globals.ThisAddIn.userInfo.UserFullName, resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
            }
            return bOK;
        }

        private bool IsSortByFieldForVisible(RepeatingGroup repGroup)
        {
            string sortByField = string.Empty;
            bool bOK = true;
            if (!string.IsNullOrEmpty(repGroup.SortByField))
            {
                RetrieveField sortByRetField = repGroup.RetrieveFields.Where(f => f.FieldId.Equals(repGroup.SortByField)).FirstOrDefault();
                if (sortByRetField != null && !fieldLevelSecurity.IsFieldVisible(sortByRetField.AppObject, repGroup.SortByField, repGroup.AppObject.ToString()))
                {
                    sortByField = repGroup.SortByField;
                    bOK = false;
                }
            }
            else if (!string.IsNullOrEmpty(repGroup.SortByField2))
            {
                RetrieveField sortBy2RetField = repGroup.RetrieveFields.Where(f => f.FieldId.Equals(repGroup.SortByField2)).FirstOrDefault();
                if (sortBy2RetField != null && !fieldLevelSecurity.IsFieldVisible(sortBy2RetField.AppObject, repGroup.SortByField2, repGroup.AppObject.ToString()))
                {
                    sortByField = repGroup.SortByField2;
                    bOK = false;
                }
            }
            if (!bOK)
            {
                string fieldName = repGroup.RetrieveFields.Find(rf => rf.FieldId.Equals(sortByField)).FieldName;
                //ApttusMessageUtil.ShowInfo("Sort by field : " + fieldName + " is in-visible to the logged-in user. Hence cannot render the records.", "Field Level Security");
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("RETRIEVEACTVIEW_IsSortByFieldForVisible_InfoMsg"), fieldName, Globals.ThisAddIn.userInfo.UserFullName, resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
            }
            return bOK;
        }

        private void InitializeBackgroundWorker()
        {
            // Attach event handlers to the BackgroundWorker object.
            Bgworker.DoWork += Bgworker_DoWork;
            Bgworker.RunWorkerCompleted += Bgworker_RunWorkerCompleted;
        }

        void Bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (waitWindow != null)
                waitWindow.CloseWaitWindow();
        }

        void Bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ThreadData data = e.Argument as ThreadData;
                attachmentManager.GetAttachments(data.apttusDataSet);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// ProcessAttachments
        /// </summary>
        public void ProcessAttachments(ApttusDataSet ds)
        {
            // Display Attachment Processing 
            ThreadData threadData = new ThreadData();
            threadData.apttusDataSet = ds;
            waitWindow = new WaitWindowView(resourceManager.GetResource("RETACTVIEW_ProcessAttachments_Msg"), false)
            {
                Parent = ObjectManager.RuntimeMode == RuntimeMode.AddIn ? Control.FromHandle(new IntPtr(Globals.ThisAddIn.Application.Hwnd)) : null,
                StartPosition = FormStartPosition.CenterScreen
            };
            Bgworker.RunWorkerAsync(threadData);

            waitWindow.ShowDialog();
        }




        /// <summary>
        /// Map Independent Cells in Runtime
        /// </summary>
        /// <param name="dt"></param>
        private void MapIndependentCells(RetrieveMap Model, Guid AppObjectUniqueId, ApttusDataSet dataSet)
        {

            DataRow dr = dataSet.DataTable.Rows[0];

            ApttusObject independentApttusObject = applicationDefinitionManager.GetAppObject(AppObjectUniqueId);

            // Filter Independent or repeating
            List<RetrieveField> retrieveFields = (Model.RetrieveFields.Where(t => t.Type == ObjectType.Independent & t.AppObject == AppObjectUniqueId)).ToList();

            // Get all SaveField before rendering
            List<SaveField> saveFields = GetAllSaveFields(AppObjectUniqueId, ObjectType.Independent);
            // Get all SaveFields which don't exist in Repeating
            List<RetrieveField> SaveFieldsToAddToIndependent = (from f in saveFields
                                                                where !(from r in retrieveFields
                                                                        select r.FieldId).Contains(f.FieldId)
                                                                select new RetrieveField
                                                                {
                                                                    AppObject = f.AppObject,
                                                                    FieldId = f.FieldId,
                                                                    Type = f.Type,
                                                                    TargetLocation = f.DesignerLocation,
                                                                    TargetNamedRange = f.TargetNamedRange,
                                                                    TargetColumnIndex = f.TargetColumnIndex,
                                                                    DataType = applicationDefinitionManager.GetField(AppObjectUniqueId, f.FieldId).Datatype
                                                                }).ToList();
            // Add SaveFields which don't exist in Independent (if any found)
            if (SaveFieldsToAddToIndependent.Count > 0)
            {
                retrieveFields.AddRange(SaveFieldsToAddToIndependent);
                // Add datatable columns
                foreach (RetrieveField rf in SaveFieldsToAddToIndependent)
                    if (dataSet.DataTable.Columns[rf.FieldId] == null)
                        dataSet.DataTable.Columns.Add(rf.FieldId);
            }

            foreach (RetrieveField retrieveField in retrieveFields)
            {
                // Set the flag isSaveOtherField if current repeating field is ONLY Save Other field
                bool isSaveOtherField = saveFields.Any(sf => sf.FieldId.Equals(retrieveField.FieldId) && sf.SaveFieldType.Equals(SaveType.SaveOnlyField)
                                            && sf.TargetNamedRange.Equals(retrieveField.TargetNamedRange));

                if (dataSet.DataTable.Columns.Contains(retrieveField.FieldId))
                {

                    bool bIsRichTextEditingDisabled = configurationManager.IsRichTextEditingDisabled;
                    if (retrieveField.DataType == Datatype.Rich_Textarea && !bIsRichTextEditingDisabled)
                    {
                        string id = dr[Constants.ID_ATTRIBUTE] as string;

                        if (String.IsNullOrEmpty(id))
                        {
                            id = Guid.NewGuid().ToString();
                        }
                        bool bVisible, bReadOnly;
                        fieldLevelSecurity.GetFieldVisibleAndReadOnlyStatus(independentApttusObject.UniqueId, retrieveField.FieldId, out bVisible, out bReadOnly);
                        if (bVisible)
                        {
                            string richTextToPopulate = string.Empty;
                            if (!bReadOnly)
                            {
                                if (!String.IsNullOrEmpty(dr[retrieveField.FieldId] as string))
                                    richTextToPopulate = String.Format(Constants.RICHTEXT_FORMULA_EDIT, id);
                                else
                                    richTextToPopulate = String.Format(Constants.RICHTEXT_FORMULA_ADD, id);
                            }
                            else
                                richTextToPopulate = String.Format(Constants.RICHTEXT_FORMULA_VIEW, id);

                            RetrieveHelper.PopulateCellIndependent(richTextToPopulate, independentApttusObject, retrieveFields, retrieveField, true);
                        }

                    }
                    else
                    {
                        RetrieveHelper.PopulateCellIndependent(dr[retrieveField.FieldId], independentApttusObject, retrieveFields, retrieveField, !isSaveOtherField);
                    }
                    // Add Mapped Location to Data Tracker
                    dataManager.AddDataTracker(new ApttusDataTracker
                    {
                        Location = retrieveField.TargetNamedRange,
                        DataSetId = dataSet.Id,
                        Type = retrieveField.Type
                    });
                }
            }
            dataSet.DataTable.AcceptChanges();

        }

        /// <summary>
        /// Render the repeating cells in the Excel Sheet.
        /// </summary>
        /// <param name="repeatingCellDataTable">Table which contains the data to be rendered.</param>
        /// <param name="Model">Model which will be used to retrieve the design-time logic by admin.</param>
        public void MapRepeatingCells(RepeatingGroup ModelGroup, ApttusDataSet dataSet, string sTargetNamedRange)
        {
            try
            {
                DataTable repeatingCellDataTable = dataSet.DataTable;

                ApttusObject repeatingApttusObject = applicationDefinitionManager.GetAppObject(ModelGroup.AppObject);

                // Get all the repeating cell fields from the map.
                List<RetrieveField> repeatingCellFields = ModelGroup.RetrieveFields.ConvertAll(rf => rf.Clone());

                // Get all SaveField from Save Group before rendering
                List<SaveField> saveFields = GetAllSaveGroupFields(ModelGroup.AppObject, ObjectType.Repeating, sTargetNamedRange);

                // Get all SaveFields which don't exist in Repeating
                List<RetrieveField> SaveFieldsToAddToRepeating = (from f in saveFields
                                                                  where !(from r in repeatingCellFields
                                                                          select r.FieldId).Contains(f.FieldId)
                                                                  select new RetrieveField
                                                                  {
                                                                      AppObject = f.AppObject,
                                                                      FieldId = f.FieldId,
                                                                      Type = f.Type,
                                                                      TargetLocation = f.DesignerLocation,
                                                                      TargetNamedRange = f.TargetNamedRange,
                                                                      TargetColumnIndex = f.TargetColumnIndex,
                                                                      DataType = applicationDefinitionManager.GetField(repeatingApttusObject.UniqueId, f.FieldId).Datatype
                                                                  }).ToList();

                // Add SaveFields which don't exist in Repeating (if any found)
                if (SaveFieldsToAddToRepeating.Count > 0)
                    repeatingCellFields.AddRange(SaveFieldsToAddToRepeating);

                // Add all missing datatable columns. set typeof(double) for Double and Decimanl datatypes
                foreach (RetrieveField rf in repeatingCellFields)
                    if (!repeatingCellDataTable.Columns.Contains(rf.FieldId))
                    {
                        DataColumn dc = new DataColumn(rf.FieldId);
                        dc.DataType = rf.DataType == Datatype.Decimal || rf.DataType == Datatype.Double ? typeof(double) : typeof(string);
                        repeatingCellDataTable.Columns.Add(dc);
                    }

                if (repeatingCellFields.Count == 0)
                    return;
                else
                {
                    ////// Quick fix to remove duplicates
                    ////// TODO: fix properly
                    HashSet<string> locationSet = new HashSet<string>();
                    List<RetrieveField> dup = new List<RetrieveField>();

                    foreach (RetrieveField f in repeatingCellFields)
                    {
                        if (locationSet.Contains(f.TargetLocation))
                            dup.Add(f);
                        else
                            locationSet.Add(f.TargetLocation);
                    }

                    foreach (RetrieveField f in dup)
                        repeatingCellFields.Remove(f);

                    // Sort fields based on Target Column Index
                    repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                    {
                        return (rField1.TargetColumnIndex.CompareTo(rField2.TargetColumnIndex));
                    });
                }

                if (ModelGroup.Layout.Equals("Horizontal"))
                {
                    RenderHorizontal(repeatingCellFields, ModelGroup, ModelGroup.AppObject, dataSet, repeatingCellDataTable);
                    return;
                }

                ExcelHelper.ExcelApp.ScreenUpdating = false;

                //Excel.Range repeatingRange = ExcelHelper.GetRange(repeatingCellFields[0].TargetNamedRange);
                // AB-296 use Repeatinggroup targetnamedrange
                Excel.Range repeatingRange = ExcelHelper.GetRange(ModelGroup.TargetNamedRange);
                Excel.Worksheet sheet = repeatingRange.Worksheet;
                // Remove Data Protection before rendering                
                ExcelHelper.UpdateSheetProtection(sheet, false);

                //UpdateNamedRangeVisibility(sheet, false);

                int startingRow = repeatingRange.Row;
                int startingColumn = repeatingRange.Column;

                // ColumnNameAndType collection is created - Column Name and Type are set.
                // There are 2 types of fields: True or False
                // True : SaveOnly fields
                // False : Retrieve fields
                // In case of Add Row (if Id is blank in all records, set all Type = True to apply formula)

                var ColumnNameAndType = new List<KeyValuePair<string, bool>>();
                bool bAllBlankRowsToRender = false;
                if (repeatingCellDataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
                {
                    var NonBlankRows = (from r in repeatingCellDataTable.AsEnumerable()
                                        where !string.IsNullOrEmpty(r.Field<string>(Constants.ID_ATTRIBUTE))
                                        select r);
                    if (NonBlankRows.Count() == 0)
                        bAllBlankRowsToRender = true;
                }

                int NextColumn = 1;
                for (int columnIndex = 1; columnIndex <= repeatingCellFields.Count;)
                {
                    RetrieveField repeatingCellField = repeatingCellFields[columnIndex - 1];

                    // Blank Column insert
                    if (NextColumn != repeatingCellField.TargetColumnIndex)
                    {
                        if (columnIndex > repeatingCellFields.Count)
                            break;
                        NextColumn++;
                        ColumnNameAndType.Add(new KeyValuePair<string, bool>(null, false));
                        continue;
                    }

                    // Non-Blank Column insert
                    if (bAllBlankRowsToRender || (!ModelGroup.RetrieveFields.Exists(s => s.FieldId == repeatingCellField.FieldId)))
                        ColumnNameAndType.Add(new KeyValuePair<string, bool>(repeatingCellField.FieldId, true));
                    else
                        ColumnNameAndType.Add(new KeyValuePair<string, bool>(repeatingCellField.FieldId, false));

                    // Display Map enhancement
                    //ApttusField apttusField = applicationDefinitionManager.GetField(ModelGroup.AppObject, repeatingCellField.FieldId);
                    string repFieldId = (repeatingCellField.AppObject != ModelGroup.AppObject && repeatingCellField.FieldId.IndexOf(Constants.DOT) > 0) ?
                        repeatingCellField.FieldId.Substring(repeatingCellField.FieldId.LastIndexOf(Constants.DOT) + 1) : repeatingCellField.FieldId;

                    ApttusField apttusField = applicationDefinitionManager.GetField(repeatingCellField.AppObject, repFieldId);

                    /* In Case of SalesForce each Object have Name field as Primary name field however in case of MS Dynamics CRM Name field is different for many object
                    * Hence ApttusField should be return as Primary Name field of Lookup Object from Fields                     * 
                    * */
                    if (CRMContextManager.Instance.ActiveCRM == CRM.DynamicsCRM)
                    {
                        if (repeatingCellField.FieldId.EndsWith(Constants.APPENDLOOKUPID) && repeatingCellField.FieldId.IndexOf(Constants.DOT) > 0)
                            apttusField = applicationDefinitionManager.GetField(repeatingCellField.AppObject, repeatingCellField.FieldId);
                    }
                    // Don't apply cell validation for:
                    // 1. Group By field
                    // 2. Lookup Name field
                    if (!(apttusField.Id.Equals(ModelGroup.GroupByField) || apttusField.Id.Equals(ModelGroup.GroupByField2)) && !apttusField.Id.EndsWith(Constants.APPENDLOOKUPID))
                    {
                        // Apply Cell Range Validation based on Datatype
                        ApttusObject currentFieldObject = applicationDefinitionManager.GetAppObject(repeatingCellField.AppObject);
                        RetrieveHelper.PopulateCellRepeating(currentFieldObject, apttusField, repeatingCellFields, repeatingCellField, repeatingRange);
                    }

                    columnIndex++;
                    NextColumn++;
                    if (columnIndex > repeatingCellFields.Count)
                        break;
                }

                //Prepare the data table because grouping might add new rows and thus we need to insert more rows in excel.
                String sortField = ModelGroup.SortByField;
                String groupField = ModelGroup.GroupByField;

                Dictionary<int, List<ExcelGroupData>> outlines = new Dictionary<int, List<ExcelGroupData>>();
                if (!String.IsNullOrEmpty(groupField))
                {
                    List<string> groupByFieldList = new List<string>();
                    groupByFieldList.Add(ModelGroup.GroupByField);
                    if (!String.IsNullOrEmpty(ModelGroup.GroupByField2))
                        groupByFieldList.Add(ModelGroup.GroupByField2);

                    DataTable groupedTable = DoGrouping(dataSet.DataTable, ModelGroup, groupByFieldList, (from c in ColumnNameAndType select c.Key).ToArray(), ref outlines);

                    //Dispose the unsorted DataTable
                    dataSet.DataTable.Dispose();
                    dataSet.DataTable = null;

                    //Update the newly sorted datatable and provide to the DataManager
                    dataSet.DataTable = groupedTable;
                    repeatingCellDataTable = dataSet.DataTable;
                }
                else if (!String.IsNullOrWhiteSpace(sortField))
                {
                    DataView dv = repeatingCellDataTable.DefaultView;
                    if (!String.IsNullOrEmpty(sortField))
                    {
                        string sortDirectionField1 = ModelGroup.SortDirectionField1 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                        string sort = sortField + sortDirectionField1;
                        if (!string.IsNullOrEmpty(ModelGroup.SortByField2))
                        {
                            string sortDirectionField2 = ModelGroup.SortDirectionField2 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                            sort = sort + ", " + ModelGroup.SortByField2 + sortDirectionField2;
                        }
                        dv.Sort = sort;

                        //Dispose the unsorted DataTable
                        dataSet.DataTable.Dispose();
                        dataSet.DataTable = null;

                        //Update the newly sorted datatable and provide to the DataManager
                        dataSet.DataTable = dv.ToTable();
                        repeatingCellDataTable = dataSet.DataTable;
                    }
                }

                //Insert the number of rows that are to rendered as part of ListObject below the startingRow.
                String rows = String.Format("{0}:{1}", startingRow + 2, startingRow + 1 + repeatingCellDataTable.Rows.Count);
                Excel.Range rangeToInsert = sheet.Range[rows];
                rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);

                //After inserting new row, start address changes, so re-assign the starting address of rendering data.
                Excel.Range topLeftHeader = (Excel.Range)sheet.Cells[startingRow, startingColumn];
                Excel.Range topLeftData = (Excel.Range)sheet.Cells[startingRow + 2, startingColumn];
                Excel.Range bottomRightData = (Excel.Range)sheet.Cells[startingRow + 1 + repeatingCellDataTable.Rows.Count, startingColumn + ColumnNameAndType.Count() - 1];
                Excel.Range DataBodyRange = sheet.get_Range(topLeftData, bottomRightData);

                //Apply Header
                if (!String.IsNullOrWhiteSpace(ModelGroup.GridHeader))
                {
                    //Excel.Range startingAddress = sheet.Range[repeatingCellFields.ElementAt(0).TargetNamedRange];
                    Excel.Range header = ExcelHelper.NextVerticalCell(topLeftHeader, -1);
                    if (header != null)
                        header.Value = ModelGroup.GridHeader;
                }

                int rowIndex, colIndex;
                object[,] cellData = null;

                try
                {

                    bool bIsRichTextEditingDisabled = configurationManager.IsRichTextEditingDisabled;

                    cellData = new object[repeatingCellDataTable.Rows.Count, ColumnNameAndType.Count];
                    for (rowIndex = 0; rowIndex < repeatingCellDataTable.Rows.Count; rowIndex++)
                    {
                        // Store ID column in first Cell of repeating group
                        //Excel.Range rngID = DataBodyRange.Cells[rowIndex + 1, 1];
                        //rngID.ID = repeatingCellDataTable.Rows[rowIndex][Constants.ID_ATTRIBUTE].ToString();

                        string id = repeatingCellDataTable.Rows[rowIndex][Constants.ID_ATTRIBUTE] as string;
                        bool bAddRow = false;
                        if (!bIsRichTextEditingDisabled)
                        {
                            if (String.IsNullOrEmpty(id))
                            {
                                bAddRow = true;
                                id = Guid.NewGuid().ToString();
                            }
                        }
                        colIndex = 0;
                        foreach (KeyValuePair<string, bool> columnNamePair in ColumnNameAndType)
                        {
                            if (columnNamePair.Key == null) //there can be empty column names.
                                continue;
                            int tableColumnIndex = repeatingCellDataTable.Columns.IndexOf(columnNamePair.Key);

                            if (tableColumnIndex != -1)
                            {
                                var item = repeatingCellFields.Where(s => s.Type == ObjectType.Repeating && s.FieldId.Equals(columnNamePair.Key)).Select(s => new { s.TargetColumnIndex, s.DataType }).FirstOrDefault();
                                colIndex = item.TargetColumnIndex;
                                colIndex--;

                                //If richtextediting is disabled, then render the richtext in excel cell.
                                if ((item.DataType != Datatype.Rich_Textarea || bIsRichTextEditingDisabled) && item.DataType != Datatype.Attachment && item.DataType != Datatype.Base64)
                                    cellData[rowIndex, colIndex] = repeatingCellDataTable.Rows[rowIndex][tableColumnIndex];
                                else
                                {
                                    if (item.DataType == Datatype.Rich_Textarea)
                                    {
                                        bool bVisible, bReadOnly;
                                        var richTextField = repeatingCellFields.Where(s => s.FieldId.Equals(columnNamePair.Key)).FirstOrDefault();
                                        fieldLevelSecurity.GetFieldVisibleAndReadOnlyStatus(richTextField.AppObject, columnNamePair.Key, out bVisible, out bReadOnly);
                                        if (bVisible)
                                        {
                                            if (!bReadOnly)
                                            {
                                                if (!String.IsNullOrEmpty(repeatingCellDataTable.Rows[rowIndex][tableColumnIndex] as string))
                                                    cellData[rowIndex, colIndex] = String.Format(Constants.RICHTEXT_FORMULA_EDIT, id);
                                                else
                                                    cellData[rowIndex, colIndex] = String.Format(Constants.RICHTEXT_FORMULA_ADD, id);
                                                if (bAddRow)
                                                {
                                                    RichTextDataManager.Instance.AddRecord(id, columnNamePair.Key);
                                                    repeatingCellDataTable.Rows[rowIndex][columnNamePair.Key] = id;
                                                }
                                            }
                                            else
                                                cellData[rowIndex, colIndex] = String.Format(Constants.RICHTEXT_FORMULA_VIEW, id);
                                        }
                                    }
                                    else if (item.DataType == Datatype.Attachment)
                                    {
                                        Guid attachmentGuid;
                                        bool isGuidValue = Guid.TryParse(Convert.ToString(repeatingCellDataTable.Rows[rowIndex][tableColumnIndex]), out attachmentGuid);
                                        cellData[rowIndex, colIndex] = isGuidValue ? string.Empty : repeatingCellDataTable.Rows[rowIndex][tableColumnIndex];

                                        if (!isGuidValue && string.IsNullOrEmpty(Convert.ToString(repeatingCellDataTable.Rows[rowIndex][tableColumnIndex])))
                                        {
                                            attachmentGuid = Guid.NewGuid();
                                            AttachmentsDataManager.GetInstance.CreateRetrieveAttachmentGuid(attachmentGuid, repeatingCellDataTable.Rows[rowIndex], ObjectType.Repeating);
                                        }
                                    }
                                    else if (item.DataType == Datatype.Base64)
                                    {
                                        var DocObj = DocumentObject.GetDocumentObject(repeatingApttusObject.Id);

                                        string documentId = Convert.ToString(repeatingCellDataTable.Rows[rowIndex][Constants.ID_ATTRIBUTE]);
                                        string documentName = Utils.ReplaceInvalidChrs(Convert.ToString(repeatingCellDataTable.Rows[rowIndex][DocObj.Field_FileName_ID]));
                                        string documentType = repeatingCellDataTable.Columns.Contains(DocObj.Field_FileType_ID) ? Convert.ToString(repeatingCellDataTable.Rows[rowIndex][DocObj.Field_FileType_ID]) : "";
                                        string documentBody = Convert.ToString(repeatingCellDataTable.Rows[rowIndex][tableColumnIndex]);
                                        if (!string.IsNullOrEmpty(documentBody))
                                        {
                                            string documentNameWithExt = !string.IsNullOrEmpty(Path.GetExtension(documentName)) ? documentName : documentName + Constants.DOT + documentType;
                                            // Create path in temp folder
                                            string filePathWithExtension = string.IsNullOrEmpty(documentId) ? string.Empty : Utils.GetTempFileName(id, documentNameWithExt, true, true);

                                            // Write file on pre-defined path
                                            if (!string.IsNullOrEmpty(documentId))
                                                Utils.StreamToFile(Convert.FromBase64String(documentBody), filePathWithExtension, true);

                                            // Update In memory data table and celldata
                                            repeatingCellDataTable.Rows[rowIndex][tableColumnIndex] = filePathWithExtension;
                                            cellData[rowIndex, colIndex] = filePathWithExtension;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                    ApttusMessageUtil.ShowError(ex.Message, resourceManager.GetResource("RetrieveActionView_MapRepeatingCells_ErrorMsg"));
                    return;
                }

                //Apply the Data in excel.
                DataBodyRange.Value = cellData;
                cellData = null; // Important

                dataSet.DataTable.AcceptChanges();

                // Add the named range data tracker entry for repeating cells
                dataManager.AddDataTracker(new ApttusDataTracker
                {
                    Location = ModelGroup.TargetNamedRange,
                    DataSetId = dataSet.Id,
                    Type = repeatingCellFields[0].Type
                });

                // ******* IMP : This code should always be after AcceptChanges as it touches *******
                // ******* the VSTO ListObject which can potentially disturb the named range ******* 

                // Recalculating the bottomRightData after VSTO rendering and apply the grid named range.
                bottomRightData = (Excel.Range)sheet.Cells[startingRow + 1 + repeatingCellDataTable.Rows.Count, startingColumn + ColumnNameAndType.Count() - 1];
                Excel.Range FullNamedRange = sheet.get_Range(topLeftHeader, bottomRightData);
                ExcelHelper.AssignNameToRange(FullNamedRange, ModelGroup.TargetNamedRange);

                if (configurationManager.Definition.AppSettings != null && configurationManager.Definition.AppSettings.ShowFilters)
                    ApplyFilter(FullNamedRange);

                if (configurationManager.Definition.AppSettings != null && configurationManager.Definition.AppSettings.AutoSizeColumns)
                    AutoSizeColumns(FullNamedRange);

                //UpdateNamedRangeVisibility(sheet, true);

                if (outlines.Count() > 0)
                {
                    DoOutline(ModelGroup, outlines, DataBodyRange, ColumnNameAndType);
                    outlines.Clear();
                }
                else
                    ApplyFormula(FullNamedRange, ColumnNameAndType, repeatingCellDataTable.Rows.Count);

                dataSet.ColumnNames = (from c in ColumnNameAndType select c.Key).ToList();

                // Apply Data Protection after rendering
                if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(sheet.Name)))
                    ExcelHelper.UpdateSheetProtection(sheet, true);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Display Action");
            }
            finally
            {
                ExcelHelper.ExcelApp.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// Apply Auto Filter on the worksheet
        /// </summary>
        /// <param name="FullNamedRange"></param>
        private void ApplyFilter(Excel.Range FullNamedRange)
        {
            Excel.Worksheet sheet = FullNamedRange.Worksheet;
            bool autoFilter = sheet.AutoFilterMode;
            // If autofilter is already existing on sheet, remove it
            // This is required for auto adjusting Column width after re-rendering 
            if (autoFilter)
                sheet.AutoFilterMode = false;

            Excel.Range currentSelection = Globals.ThisAddIn.Application.Selection;
            Excel.Worksheet currentWorksheet = currentSelection.Worksheet;

            Excel.Worksheet namedRangeWorksheet = FullNamedRange.Worksheet;
            if (namedRangeWorksheet.Visible == Excel.XlSheetVisibility.xlSheetVisible)
            {
                namedRangeWorksheet.Activate();

                FullNamedRange.Select();
                ExcelHelper.ExcelApp.Selection.AutoFilter();

                currentWorksheet.Activate();
                currentSelection.Select();
            }
        }

        private void AutoSizeColumns(Excel.Range FullNamedRange)
        {
            foreach (Excel.Range col in FullNamedRange.Columns)
            {
                var colWidth = col.ColumnWidth;
                if (colWidth > 0)
                {
                    col.AutoFit();
                    colWidth = col.ColumnWidth;
                    //int colWidthForApp = configurationManager.Application.QuickAppModel != null ? configurationManager.Application.QuickAppModel.MaxColumnWidth : 50;
                    int maxColWidth = Convert.ToInt32(configurationManager.Definition.AppSettings.MaxColumnWidth);
                    int colWidthForApp = maxColWidth > 0 ? maxColWidth : 50;
                    if (colWidth > colWidthForApp)
                        col.ColumnWidth = colWidthForApp;
                }
            }
        }

        private List<SaveField> GetAllSaveFields(Guid AppObjectUniqueId, ObjectType ObjectType)
        {
            List<SaveField> saveFields = new List<SaveField>();
            foreach (SaveMap sMap in configurationManager.SaveMaps)
            {
                saveFields.AddRange(from f in sMap.SaveFields
                                    where f.AppObject == AppObjectUniqueId & f.Type == ObjectType
                                    select f);
            }
            return saveFields.Distinct().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppObjectUniqueId"></param>
        /// <param name="ObjectType"></param>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        private List<SaveField> GetAllSaveGroupFields(Guid AppObjectUniqueId, ObjectType ObjectType, string TargetNamedRange)
        {
            List<SaveField> saveFields = new List<SaveField>();
            foreach (SaveMap sMap in configurationManager.SaveMaps)
            {
                foreach (SaveGroup sGroup in sMap.SaveGroups)
                {
                    if (!string.IsNullOrEmpty(TargetNamedRange))
                    {
                        if (sGroup.TargetNamedRange.Equals(TargetNamedRange))
                        {
                            saveFields.AddRange(from f in sMap.SaveFields
                                                where f.AppObject == AppObjectUniqueId && f.Type == ObjectType && f.GroupId == sGroup.GroupId
                                                select f);
                        }
                    }
                }
            }
            return saveFields.Distinct().ToList();
        }

        private string ReplaceEscapeSequence(string field)
        {
            StringBuilder fieldWithEscSeq = new StringBuilder(field.Length);
            for (int i = 0; i < field.Length; ++i)
            {
                char ch = field[i];
                switch (ch)
                {
                    case ']':
                    case '[':
                    case '%':
                    case '*':
                        fieldWithEscSeq.Append("[").Append(ch).Append("]");
                        break;
                    case '\'':
                        fieldWithEscSeq.Append("''");
                        break;
                    default:
                        fieldWithEscSeq.Append(ch);
                        break;
                }
            }
            return fieldWithEscSeq.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repGroup"></param>
        /// <returns></returns>
        private string GetSortFieldsForRepeatingGroup(RepeatingGroup repGroup)
        {
            string sortByField = string.IsNullOrEmpty(repGroup.SortByField) ? string.Empty : repGroup.SortByField;
            if (!String.IsNullOrEmpty(sortByField))
            {
                string sortFieldDirection1 = repGroup.SortDirectionField1 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                sortByField = sortByField + sortFieldDirection1;
            }

            if (!String.IsNullOrEmpty(sortByField) && !String.IsNullOrEmpty(repGroup.SortByField2))
            {
                string sortFieldDirection2 = repGroup.SortDirectionField2 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                sortByField = sortByField + ", " + repGroup.SortByField2 + sortFieldDirection2;
            }
            return sortByField;
        }

        private List<string> GetGroupByValues(DataTable table, RepeatingGroup repGroup, string groupByField)
        {
            List<string> groupByValues = table.AsEnumerable().GroupBy(s => s.Field<string>(groupByField)).Select(s => s.Key).ToList();
            bool descendingSort = false;
            if (groupByField == repGroup.SortByField)
            {
                if (repGroup.SortDirectionField1 == RepeatingGroupSortDirection.Descending)
                    descendingSort = true;
            }
            else if (groupByField == repGroup.SortByField2)
            {
                if (repGroup.SortDirectionField2 == RepeatingGroupSortDirection.Descending)
                    descendingSort = true;
            }

            if (descendingSort)
                groupByValues.Sort((x, y) => -1 * string.Compare(x, y));
            else
                groupByValues.Sort((x, y) => string.Compare(x, y));
            return groupByValues;
        }

        private DataTable DoGrouping(DataTable apttusDataSetInputTable, RepeatingGroup repGroup, List<string> groupByFieldsList, string[] columns, ref Dictionary<int, List<ExcelGroupData>> outlines)
        {
            //Create a new DataTable, which contains grouped data.

            DataTable groupedTable = new DataTable();
            string queryString = string.Empty;
            int expandedRows = 0;
            int groupingLevel = 1;

            //Add columns to the newly created grouped table.
            foreach (string column in columns)
            {
                groupedTable.Columns.Add(column);
                if (!String.IsNullOrEmpty(column))
                    groupedTable.Columns[column].DataType = apttusDataSetInputTable.Columns[column].DataType;
            }


            //Create and add grouped rows to data table.
            CreateGroupedDataTable(groupedTable, repGroup.GroupByField, groupByFieldsList, apttusDataSetInputTable, repGroup, ref outlines, ref queryString, ref expandedRows, groupingLevel, string.Empty);

            return groupedTable;
        }

        private string GetNextGroupByField(string currentGroupByField, List<string> groupByFieldsList)
        {
            int item = groupByFieldsList.IndexOf(currentGroupByField);
            if (item == -1)
                return string.Empty;
            if (groupByFieldsList.Count == (item + 1))
                return string.Empty;
            return groupByFieldsList.ElementAt(item + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupedOutputTable">Table containing Output data in grouped form</param>
        /// <param name="groupByField">GroupByfield by which the data will be grouped</param>
        /// <param name="groupByFieldsList">A list which contains all groupby fields</param>
        /// <param name="inputTable"></param>
        /// <param name="repGroup">Model</param>
        /// <param name="outlines"></param>
        /// <param name="parentChildGroupByFilter">Filter clause to filter the data</param>
        /// <param name="parentGroupRowCount"></param>
        /// <param name="parentGroupID">total number of parent groups</param>
        private void CreateGroupedDataTable(DataTable groupedOutputTable, string groupByField, List<string> groupByFieldsList, DataTable inputTable,
                                    RepeatingGroup repGroup, ref Dictionary<int, List<ExcelGroupData>> outlines, ref string parentChildGroupByFilter,
                                    ref int parentGroupRowCount, int parentGroupID, string parentGroupByValue)
        {
            string sortByField = GetSortFieldsForRepeatingGroup(repGroup);
            bool bShowTotals = repGroup.TotalFields.Count != 0;

            List<string> groupByValues = GetGroupByValues(inputTable, repGroup, groupByField);

            foreach (string groupByValue in groupByValues)
            {
                string individualGroupByFilter = string.Empty;
                DataRow[] rows = null;
                if (String.IsNullOrEmpty(groupByValue))
                    individualGroupByFilter = groupByField + " is null ";
                else
                    individualGroupByFilter = groupByField + " = '" + ReplaceEscapeSequence(groupByValue) + "'";


                inputTable.CaseSensitive = true;

                rows = inputTable.Select(individualGroupByFilter, sortByField);

                string allParentGroupByValue = string.Empty;
                if (!string.IsNullOrEmpty(parentGroupByValue))
                    allParentGroupByValue = parentGroupByValue + Constants.GROUPING_UNIQUE_SEPARATOR + groupByValue;
                if (rows != null && rows.Count() != 0)
                {
                    //Location of calling below 2 lines of code is important
                    if (repGroup.GroupingLayout == GroupingLayout.Top)
                        AddGroupedRows(repGroup.GroupSpacing, parentChildGroupByFilter, groupByValue, groupByField, bShowTotals, ref parentGroupRowCount, groupedOutputTable, allParentGroupByValue);

                    int groupingStartRow = groupedOutputTable.Rows.Count;

                    string nextGroupbyField = GetNextGroupByField(groupByField, groupByFieldsList);

                    if (!String.IsNullOrEmpty(nextGroupbyField))
                    {
                        parentChildGroupByFilter += individualGroupByFilter + " and ";
                        parentGroupByValue += groupByValue;
                        CreateGroupedDataTable(groupedOutputTable, nextGroupbyField, groupByFieldsList, rows.CopyToDataTable(), repGroup, ref outlines, ref parentChildGroupByFilter, ref parentGroupRowCount, parentGroupID, parentGroupByValue);
                        parentGroupByValue = string.Empty;
                        parentChildGroupByFilter = string.Empty;
                    }
                    else
                    {
                        string fullGroupByFilter = parentChildGroupByFilter + individualGroupByFilter;
                        rows = inputTable.Select(fullGroupByFilter, sortByField);
                        using (DataTable filteredDataTable = rows.CopyToDataTable())
                        {
                            bool bIsParent = parentGroupRowCount != 0;
                            bool bAddTotalRow = bShowTotals && bIsParent;

                            if (bAddTotalRow)
                            {
                                //Insert a new row at the bottom for displaying the totals
                                DataRow drNew = filteredDataTable.NewRow();
                                drNew.RowError = Constants.GROUPING_ROW_ERRORMSG_ONLY;
                                filteredDataTable.Rows.Add(drNew);
                            }
                            parentGroupRowCount += filteredDataTable.Rows.Count;

                            foreach (DataRow row in filteredDataTable.Rows)
                            {
                                foreach (string groupField in groupByFieldsList)
                                    row[groupField] = string.Empty;
                            }

                            if (bAddTotalRow)
                                filteredDataTable.Rows[filteredDataTable.Rows.Count - 1][groupByField] = "Total " + groupByValue;

                            if (bIsParent)
                            {
                                ExcelGroupData groupData = new ExcelGroupData
                                {
                                    StartRow = groupedOutputTable.Rows.Count + 1,
                                    RowCount = filteredDataTable.Rows.Count,
                                    ParentGroupId = parentGroupID
                                };
                                AddExcelGroupDataInDictionary(groupData, parentGroupID, outlines);
                            }

                            groupedOutputTable.Merge(filteredDataTable);
                        }
                    }

                    //Location of calling below 2 lines of code is important
                    //if (repGroup.GroupingLayout == GroupingLayout.Bottom)
                    //    AddGroupedRows(repGroup.GroupSpacing, parentChildGroupByFilter, groupByValue, groupByField, bShowTotals, ref parentGroupRowCount, groupedOutputTable);

                    if (String.IsNullOrEmpty(parentChildGroupByFilter))
                    {
                        if (bShowTotals)
                        {
                            parentGroupRowCount++;

                            DataRow dr = groupedOutputTable.NewRow();
                            dr.RowError = Constants.GROUPING_ROW_ERRORMSG_ONLY;
                            dr[groupByField] = "Total " + groupByValue;

                            groupedOutputTable.Rows.Add(dr);
                        }

                        ExcelGroupData groupData = new ExcelGroupData
                        {
                            StartRow = groupingStartRow + 1,
                            RowCount = parentGroupRowCount,
                            ParentGroupId = parentGroupID
                        };
                        AddExcelGroupDataInDictionary(groupData, -1, outlines); //For Parent group which is the first level group, the key is -1.             
                        parentGroupRowCount = 0;
                        ++parentGroupID;
                    }
                }
            }
        }

        private void AddGroupedRows(int spacing, string parentChildGroupByFilter, string groupByValue,
                                    string groupByField, bool bShowTotals, ref int parentGroupRowCount, DataTable groupedOutputTable, string parentGroupByValue)
        {
            for (int i = 0; i < 1; ++i)
            {
                DataRow dr = groupedOutputTable.NewRow();
                if (i == 0)
                    dr[groupByField] = String.IsNullOrEmpty(groupByValue) ? string.Empty : groupByValue;
                dr.RowError = string.Format(Constants.GROUPING_ROW_ERRORMSG_WITHFIELDID, parentGroupByValue);

                if (!String.IsNullOrEmpty(parentChildGroupByFilter)) //If this is true, it means we are not at the first level of grouping.
                    parentGroupRowCount++;

                groupedOutputTable.Rows.Add(dr);
            }
        }

        private void AddExcelGroupDataInDictionary(ExcelGroupData groupData, int parentKey, Dictionary<int, List<ExcelGroupData>> outlines)
        {
            if (outlines.ContainsKey(parentKey))
            {
                List<ExcelGroupData> lst = outlines[parentKey];
                lst.Add(groupData);
            }
            else
            {
                List<ExcelGroupData> lst = new List<ExcelGroupData>();
                lst.Add(groupData);
                outlines.Add(parentKey, lst);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="groupByField"></param>
        /// <param name="sortField"></param>
        /// <param name="groupSpacing"></param>
        /// <param name="outlines"></param>
        /// <returns></returns>
        //private DataTable ApplyGroupingOnTable(DataTable table, string[] columns, RepeatingGroup repGroup, ref Dictionary<int, int> outlines)
        //{
        //    //{
        //    //    //Crap Code. should be removed. just for testing purpose.
        //    //    string groupbyClause = repGroup.GroupByField + "= 'A 1 Insurance Agency' and " + repGroup.GroupByField2 + "= 'Proposal/Price Quote'";
        //    //    DataRow[] rows = table.Select(groupbyClause, string.Empty);
        //    //    int c = rows.Count();
        //    //}
        //    string groupByField = repGroup.GroupByField;

        //    DataTable groupedTable = new DataTable();
        //    //Add columns to the newly created table.
        //    foreach (string column in columns)
        //    {
        //        groupedTable.Columns.Add(column);
        //        if (!String.IsNullOrEmpty(column))
        //            groupedTable.Columns[column].DataType = table.Columns[column].DataType;
        //    }

        //    List<string> groupByValues = GetGroupByValues(table, repGroup, groupByField);

        //    string sortByField = GetSortFieldsForRepeatingGroup(repGroup);

        //    bool bSecondLevelGroupingOn = !String.IsNullOrEmpty(repGroup.GroupByField2);

        //    foreach (string groupByValue in groupByValues)
        //    {
        //        //Retrieve rows which based on the specified filter in the specified sort order.
        //        DataRow[] rows = null;
        //        if (String.IsNullOrEmpty(groupByValue))
        //            rows = table.Select(groupByField + " is null ", sortByField);
        //        else
        //            rows = table.Select(groupByField + " = '" + ReplaceEscapeSequence(groupByValue) + "'", sortByField);

        //        if (rows != null && rows.Count() > 0)
        //        {
        //            foreach (DataRow dr in rows)
        //                dr[groupByField] = String.Empty;

        //            //Apply Spacing between groups
        //            for (int i = 0; i < repGroup.GroupSpacing; ++i)
        //            {
        //                DataRow dr = groupedTable.NewRow();
        //                if (i == 0)
        //                    dr[groupByField] = String.IsNullOrEmpty(groupByValue) ? string.Empty : groupByValue;
        //                dr.RowError = "Do not touch";
        //                groupedTable.Rows.Add(dr);
        //            }

        //            int groupingStartRow = groupedTable.Rows.Count;
        //            //Outline start row and the number of rows in the group, part of excel group.                    
        //            int expandedRows = 0;
        //            using (DataTable dt = rows.CopyToDataTable())
        //            {
        //                if (bSecondLevelGroupingOn)
        //                {
        //                    List<string> groupByValues2 = GetGroupByValues(dt, repGroup, repGroup.GroupByField2);
        //                    foreach (string groupByValue2 in groupByValues2)
        //                    {
        //                        DataRow[] secondLevelRows = null;
        //                        if (String.IsNullOrEmpty(groupByValue2))
        //                            secondLevelRows = dt.Select(repGroup.GroupByField2 + " is null ", sortByField);
        //                        else
        //                            secondLevelRows = dt.Select(repGroup.GroupByField2 + " = '" + ReplaceEscapeSequence(groupByValue2) + "'", sortByField);

        //                        if (secondLevelRows != null && secondLevelRows.Count() > 0)
        //                        {
        //                            //Apply Spacing between groups
        //                            for (int i = 0; i < repGroup.GroupSpacing; ++i)
        //                            {
        //                                DataRow dr = groupedTable.NewRow();
        //                                if (i == 0)
        //                                    dr[repGroup.GroupByField2] = String.IsNullOrEmpty(groupByValue2) ? string.Empty : groupByValue2;
        //                                dr.RowError = "Do not touch";
        //                                groupedTable.Rows.Add(dr);
        //                                expandedRows++;
        //                            }
        //                            foreach (DataRow dr in secondLevelRows)
        //                                dr[repGroup.GroupByField2] = String.Empty;

        //                            outlines[groupedTable.Rows.Count + 1] = secondLevelRows.Count();

        //                            using (DataTable secondLevelTable = secondLevelRows.CopyToDataTable())
        //                            {
        //                                groupedTable.Merge(secondLevelTable);
        //                                expandedRows += secondLevelTable.Rows.Count;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    groupedTable.Merge(dt);
        //                    expandedRows = dt.Rows.Count;
        //                }

        //                outlines[groupingStartRow + 1] = expandedRows;
        //            }
        //        }
        //    }

        //    return groupedTable;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="listObj"></param>
        private void DoOutline(RepeatingGroup repGroup, Dictionary<int, List<ExcelGroupData>> outlines, Excel.Range dataRange, List<KeyValuePair<string, bool>> ColumnNameAndType)
        {
            Excel.Worksheet sheet = dataRange.Worksheet;
            sheet.Outline.SummaryRow = Excel.XlSummaryRow.xlSummaryAbove;

            Dictionary<int, List<TotalCellData>> rangeInParentGroup = new Dictionary<int, List<TotalCellData>>();
            bool bPerformTotalsOperation = repGroup.TotalFields.Count != 0;

            List<ExcelGroupData> parentGroups = outlines.Where(o => o.Key == -1).Select(o => o.Value).FirstOrDefault().ToList();
            bool bHasSubGroups = outlines.Where(o => o.Key != -1).Count() > 0;

            foreach (ExcelGroupData gd in parentGroups)
            {
                Excel.Range startRow = dataRange.Rows[gd.StartRow];
                Excel.Range endRow = dataRange.Rows[gd.StartRow + gd.RowCount - 1];
                Excel.Range outlineRange = sheet.get_Range(startRow, endRow);

                outlineRange.Group();
                if (!bHasSubGroups)
                    ApplyFormula(dataRange, ColumnNameAndType, outlineRange.Rows.Count, outlineRange);
            }

            if (bHasSubGroups)
            {
                //Apply grouping and store the totals data if subtotals needed to be calculated.
                foreach (KeyValuePair<int, List<ExcelGroupData>> o in outlines.OrderBy(o => o.Key))
                {
                    foreach (ExcelGroupData gd in o.Value)
                    {
                        //if (repGroup.GroupingLayout == GroupingLayout.Top)
                        //    startRowIndex = gd.StartRow - repGroup.GroupSpacing;
                        //else
                        //    startRowIndex = gd.StartRow + gd.RowCount;

                        Excel.Range startRow = dataRange.Rows[gd.StartRow];
                        Excel.Range endRow = dataRange.Rows[gd.StartRow + gd.RowCount - 1];
                        Excel.Range outlineRange = sheet.get_Range(startRow, endRow);

                        bool isParentGroup = parentGroups.Any(pg => pg.RowCount == gd.RowCount && pg.StartRow == gd.StartRow);
                        if (!isParentGroup) //grouping is already applied on parent groups, so no need to apply it again. Applying it again will increase the group level, which in turn is displayed incorrectly in excel.
                            outlineRange.Group();

                        if (o.Key != -1) //Only add subgroups. -1 indicates this is a parent group, which is nothing but level of outlining.
                        {
                            if (bPerformTotalsOperation)
                            {
                                int startRowIndex = gd.StartRow + gd.RowCount - 1;
                                if (rangeInParentGroup.ContainsKey(o.Key))
                                {
                                    List<TotalCellData> lstRange = rangeInParentGroup[o.Key];
                                    TotalCellData rg = new TotalCellData
                                    {
                                        TotalRow = dataRange.Rows[startRowIndex],
                                        TotalsRange = sheet.Range[startRow, ExcelHelper.NextVerticalCell(endRow, -1)]
                                    };
                                    lstRange.Add(rg);
                                }
                                else
                                {
                                    List<TotalCellData> lstRange = new List<TotalCellData>();
                                    TotalCellData rg = new TotalCellData
                                    {
                                        TotalRow = dataRange.Rows[startRowIndex],
                                        TotalsRange = sheet.Range[startRow, ExcelHelper.NextVerticalCell(endRow, -1)]
                                    };
                                    lstRange.Add(rg);
                                    rangeInParentGroup.Add(o.Key, lstRange);
                                }
                            }
                            ApplyFormula(dataRange, ColumnNameAndType, outlineRange.Rows.Count, outlineRange);
                        }
                    }
                }
            }
            //Apply sum and subtotals.
            foreach (string totalField in repGroup.TotalFields)
            {
                int totalFieldExcelColumnIndex = 0;
                foreach (KeyValuePair<string, bool> pair in ColumnNameAndType)
                {
                    ++totalFieldExcelColumnIndex;
                    if (!String.IsNullOrEmpty(pair.Key) && pair.Key.Equals(totalField))
                        break;
                }

                foreach (ExcelGroupData parentGroup in parentGroups)
                {
                    List<TotalCellData> lstSubGroupsInParent = rangeInParentGroup.Where(rg => rg.Key == parentGroup.ParentGroupId).Select(o => o.Value).FirstOrDefault();
                    string[] totalSubGroupCellAddress = null;
                    if (lstSubGroupsInParent != null)
                    {
                        totalSubGroupCellAddress = new string[lstSubGroupsInParent.Count];
                        int nCount = 0;
                        foreach (TotalCellData totalRange in lstSubGroupsInParent)
                        {
                            //Scenario For example, I have a formula in b10 to sum($b$2:b9). When i select row 10 and insert, I want the formula to automatically update and then be sum($b$2:b10).                             
                            //Problem : While adding support for add-row in case of grouping, After Inserting Rows, Sum Function Is Not Automatically Extending the formula.
                            //Solution: Got the solution from http://www.mrexcel.com/forum/excel-questions/528429-after-inserting-row-sum-function-not-automatically-extending.html
                            string address = ((totalRange.TotalsRange.Columns[totalFieldExcelColumnIndex] as Excel.Range).Cells[1, 1] as Excel.Range).Address;
                            address = address + ":INDIRECT(\"R[-1]C\",0)";

                            Excel.Range totalCell = (totalRange.TotalRow.Cells[totalFieldExcelColumnIndex] as Excel.Range);
                            totalCell.Formula = "=sum(" + address + ")";
                            ExcelHelper.ApplyGroupingBorder(totalCell, Excel.XlBordersIndex.xlEdgeTop);
                            totalSubGroupCellAddress[nCount] = totalCell.Address;
                            ++nCount;
                        }
                    }
                    else
                    {
                        Excel.Range startRow = dataRange.Rows[parentGroup.StartRow];
                        Excel.Range endRow = dataRange.Rows[parentGroup.StartRow + parentGroup.RowCount - 1];
                        Excel.Range totalsRange = sheet.get_Range(startRow, ExcelHelper.NextVerticalCell(endRow, -1));
                        //Scenario For example, I have a formula in b10 to sum($b$2:b9). When i select row 10 and insert, I want the formula to automatically update and then be sum($b$2:b10).                             
                        //Problem : While adding support for add-row in case of grouping, After Inserting Rows, Sum Function Is Not Automatically Extending the formula.
                        //Solution: Got the solution from http://www.mrexcel.com/forum/excel-questions/528429-after-inserting-row-sum-function-not-automatically-extending.html                        
                        string address = ((totalsRange.Columns[totalFieldExcelColumnIndex] as Excel.Range).Cells[1, 1] as Excel.Range).Address;
                        address = address + ":INDIRECT(\"R[-1]C\",0)";

                        Excel.Range totalCell = (endRow.Cells[totalFieldExcelColumnIndex] as Excel.Range);
                        ExcelHelper.ApplyGroupingBorder(totalCell, Excel.XlBordersIndex.xlEdgeTop);
                        totalCell.Formula = "=sum(" + address + ")";
                    }
                    //sub totals will only be done if the grouping level is more than or equal to 2.
                    //If second level grouping is there, then only apply subtotals.
                    if (!String.IsNullOrEmpty(repGroup.GroupByField2) && totalSubGroupCellAddress != null)
                    {
                        int startRowIndex = parentGroup.StartRow + parentGroup.RowCount - 1;
                        //if (repGroup.GroupingLayout == GroupingLayout.Top)
                        //    startRowIndex = parentGroup.StartRow - repGroup.GroupSpacing;
                        //else
                        //    startRowIndex = parentGroup.StartRow + parentGroup.RowCount;

                        string subtotalformula = "=subtotal(9," + String.Join(",", totalSubGroupCellAddress) + ")";
                        Excel.Range groupingRow = dataRange.Rows[startRowIndex];
                        Excel.Range parentRowCell = groupingRow.Cells[totalFieldExcelColumnIndex];
                        ExcelHelper.ApplyGroupingBorder(parentRowCell, Excel.XlBordersIndex.xlEdgeTop);
                        parentRowCell.Formula = subtotalformula;
                    }
                }
            }
        }

        /// <summary>
        /// Apply formula 
        /// </summary>
        /// <param name="listObj"></param>
        /// <param name="ColumnNameAndType"></param>
        /// <param name="outlineGroupRange"></param>
        private void ApplyFormula(Excel.Range range, List<KeyValuePair<string, bool>> ColumnNameAndType, int rowCount, Excel.Range outlineGroupRange = null)
        {
            int colIndex = 1;
            // Assign range to sheet
            Excel.Worksheet oSheet = range.Worksheet;
            foreach (KeyValuePair<string, bool> col in ColumnNameAndType)
            {
                if (String.IsNullOrEmpty(col.Key) || col.Value)
                {
                    // Check if formula exist then Autofill formula
                    if (outlineGroupRange == null)
                    {
                        Excel.Range formulaCell = range.Cells[2, colIndex];

                        if (formulaCell.HasFormula)
                        {
                            Excel.Range startCell = range.Cells[3, colIndex];
                            Excel.Range endCell = range.Cells[rowCount + 2, colIndex];
                            Excel.Range formulaRange = oSheet.Range[startCell, endCell];

                            formulaCell.Copy(formulaRange);
                            //formulaRange.PasteSpecial(Excel.XlPasteType.xlPasteFormulasAndNumberFormats, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone);
                        }
                    }
                    else if (outlineGroupRange != null)
                    {
                        Excel.Range formulaCell = ExcelHelper.NextVerticalCell(range.Cells[1, colIndex], -1);
                        if (formulaCell.HasFormula)
                        {
                            // Get first cell from group data range
                            Excel.Range startRange = outlineGroupRange.Cells[1, colIndex];
                            // Get last cell from group data range
                            Excel.Range endRange = ExcelHelper.NextVerticalCell(startRange, outlineGroupRange.Rows.Count - 1);
                            Excel.Range destinationRange = oSheet.get_Range(startRange, endRange);

                            // Copy formula from hidden row
                            formulaCell.Copy(destinationRange);
                            //destinationRange.PasteSpecial(Excel.XlPasteType.xlPasteFormulasAndNumberFormats, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone);

                        }
                    }
                }

                //// Set the cursor back to first cell in the range and remove the dotted copy outline
                //range.Cells[1, 1].Select();
                //ExcelHelper.ExcelApp.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                colIndex++;
            }
        }


        private void ApplyFormulaForHorizontal(Excel.Range range, List<KeyValuePair<string, bool>> columnNames, int rowCount, Excel.Range outlineGroupRange = null)
        {

            int colIndex = 1;
            // Assign range to sheet
            Excel.Worksheet oSheet = range.Worksheet;
            foreach (KeyValuePair<string, bool> col in columnNames)
            {
                if (String.IsNullOrEmpty(col.Key) || col.Value)
                {
                    // Define Hidden Row Ranhge
                    int row = range.Row;
                    int col2 = range.Column;
                    Excel.Range hiddenRowRange = range.Cells[0, colIndex];
                    bool IshiddenFormula = hiddenRowRange.HasFormula;

                    /*
                    // Check if formla exist then Autofill formula
                    if (IshiddenFormula && outlineGroupRange != null)
                    {
                        Excel.Range startRange = null;
                        // Get first cell from group data range
                        startRange = outlineGroupRange.Cells[1, colIndex];
                        // Get last cell from group data range
                        Excel.Range endRange = ExcelHelper.NextVerticalCell(startRange, outlineGroupRange.Rows.Count - 1);

                        // Get range 
                        Excel.Range destRange = oSheet.get_Range(startRange, endRange);
                        // Copy formula from hidden row
                        hiddenRowRange.Copy();
                        // Paste numbers with formula
                        oSheet.get_Range(startRange, endRange).PasteSpecial(Excel.XlPasteType.xlPasteFormulasAndNumberFormats |
                                              Excel.XlPasteType.xlPasteColumnWidths, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone);
                    }*/
                    if (outlineGroupRange == null)
                    {
                        Excel.Range formulaCell = range.Cells[colIndex, 2];

                        Excel.Range startCell = range.Cells[colIndex, 3];
                        Excel.Range endCell = range.Cells[colIndex, rowCount + 2];

                        Excel.Range formulaRange = oSheet.Range[startCell, endCell];
                        formulaCell.Copy(formulaRange);
                    }
                }
                colIndex++;
            }


        }
        /// <summary>  
        /// Updates the sheet's row hidden status.
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="bHidden"></param>
        private void UpdateNamedRangeVisibility(Excel.Worksheet sheet, bool bHidden, LayoutType Layout = LayoutType.Vertical)
        {
            try
            {
                // TODO:: Remove use of TargetLocation from below and use targetnamedrange to get Sheet object.
                List<String> namedRanges = new List<string>();

                // Get all retrieve map named ranges
                foreach (RetrieveMap rMap in configurationManager.RetrieveMaps)
                {
                    List<String> Ranges = (from rg in rMap.RepeatingGroups
                                           from rf in rg.RetrieveFields
                                           where rf.TargetLocation.Contains(sheet.Name)
                                           select rg.TargetNamedRange).Distinct().ToList();

                    if (Ranges != null)
                        namedRanges.AddRange(Ranges);

                    //namedRanges = rMap.RetrieveFields.Where(s => s.Type == ObjectType.Repeating && s.TargetLocation.Contains(sheet.Name)).Select(s => s.TargetNamedRange).Distinct();
                }

                // Get all save map named ranges
                List<String> saveOnlyNamedRanges = new List<string>();
                foreach (SaveMap sMap in configurationManager.SaveMaps)
                {
                    List<String> Ranges = (from sg in sMap.SaveGroups
                                           from sf in sMap.SaveFields
                                           where sg.GroupId == sf.GroupId & sf.DesignerLocation.Contains(sheet.Name) &
                                           (!namedRanges.Contains(sg.TargetNamedRange))
                                           select sg.TargetNamedRange).Distinct().ToList();
                    if (Ranges != null)
                        saveOnlyNamedRanges.AddRange(Ranges);
                }

                namedRanges.AddRange(saveOnlyNamedRanges);

                if (Layout == LayoutType.Horizontal)
                {
                    foreach (string namedRange in namedRanges.Distinct())
                    {
                        Excel.Range r = sheet.Range[namedRange].Columns[2];
                        if (!r.Hidden)
                            r.Hidden = bHidden;
                        else if (!bHidden)
                            r.Hidden = bHidden;
                    }
                }
                else
                {

                    foreach (string namedRange in namedRanges.Distinct())
                    {
                        Excel.Range r = sheet.Range[namedRange].Rows[2];
                        if (!r.Hidden)
                            r.Hidden = bHidden;
                        else if (!bHidden)
                            r.Hidden = bHidden;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="InputData"></param>
        /// <param name="InputDataNames"></param>
        public void FillCrossTabData(List<CrossTabRetrieveMap> CrossTabMaps, bool InputData, string[] InputDataNames)
        {
            try
            {
                foreach (CrossTabRetrieveMap CrossTabMap in CrossTabMaps)
                {
                    // Get dataset for Row object and Column object
                    ApttusDataSet rowDataSet = DataManager.GetInstance.ResolveInput(InputDataNames,
                            ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabMap.RowField.AppObject));

                    ApttusDataSet colDataSet = DataManager.GetInstance.ResolveInput(InputDataNames,
                            ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabMap.ColField.AppObject));

                    ApttusDataSet transactionalDataSet = DataManager.GetInstance.ResolveInput(InputDataNames,
                            ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabMap.DataField.AppObject));

                    // Map Cross Tab data
                    MapCrossTabCells(CrossTabMap, rowDataSet, colDataSet, transactionalDataSet);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }


        private void RenderHorizontal(List<RetrieveField> repeatingCellFields, RepeatingGroup repGroup, Guid AppObjectUniqueId, ApttusDataSet dataSet, DataTable repeatingCellDataTable, string SaveOnlyTargetNamedRange = "")
        {

            ApttusObject repeatingApttusObject = applicationDefinitionManager.GetAppObject(AppObjectUniqueId);

            Excel.Range repeatingRange = ExcelHelper.GetRange(repGroup.TargetNamedRange);
            Excel.Worksheet sheet = repeatingRange.Worksheet;

            //UpdateNamedRangeVisibility(sheet, false, LayoutType.Horizontal);

            //Remove Inserted rows while re-rendering
            int nRepeatingCellRows = 2; //In designer there are 2 cols , one which represents labels and another one is a formula row. 
            //While rendering we extend the named range which contains all the cols  as well as the above 2 cols. Thus while re-rendering compare the named range col count
            //if it is greater than 2 that means we will have to remove all the cols  which contains data, except the first 2 cols.
            if (repeatingRange.Columns.Count > nRepeatingCellRows)
            {
                String start = ExcelHelper.GetExcelColumnName(repeatingRange.Column + nRepeatingCellRows);
                String end = ExcelHelper.GetExcelColumnName(repeatingRange.Column + repeatingRange.Columns.Count - 1);
                String colsRange = String.Format("{0}:{1}", start, end);
                Excel.Range rangeToRemove = sheet.Range[colsRange];
                rangeToRemove.Columns.EntireColumn.Delete();
            }

            int startingRow = repeatingRange.Row;
            int startingColumn = repeatingRange.Column;

            //Assign Column names to Excel ListObject
            //List<string> columnNames = new List<string>();
            // Column Name and whether it is a SaveField
            // True : SaveOnly Fields
            // False : Retrieve and Save Field

            var columnNames = new List<KeyValuePair<string, bool>>();

            int NextColumn = 1;
            for (int columnIndex = 1; columnIndex <= repeatingCellFields.Count;)
            {
                RetrieveField repeatingCellField = repeatingCellFields[columnIndex - 1];

                // Blank Column insert
                if (NextColumn != repeatingCellField.TargetColumnIndex)
                {
                    NextColumn++;
                    columnNames.Add(new KeyValuePair<string, bool>(null, false)); // Insert blank columns as specified by the user.
                    continue;
                }

                // Valid data column insert
                if (repGroup.RetrieveFields.Exists(s => s.AppObject.Equals(repeatingCellField.AppObject) && s.FieldId.Equals(repeatingCellField.FieldId)))
                    columnNames.Add(new KeyValuePair<string, bool>(repeatingCellField.FieldId, false)); // Insert blank columns as specified by the user.
                else
                    columnNames.Add(new KeyValuePair<string, bool>(repeatingCellField.FieldId, true)); // Insert blank columns as specified by the user.
                //columnNames.Add(repeatingCellField.FieldId);

                string repFieldId = (repeatingCellField.AppObject != AppObjectUniqueId && repeatingCellField.FieldId.IndexOf(Constants.DOT) > 0) ?
                        repeatingCellField.FieldId.Substring(repeatingCellField.FieldId.LastIndexOf(Constants.DOT) + 1) : repeatingCellField.FieldId;

                ApttusField apttusField = applicationDefinitionManager.GetField(repeatingCellField.AppObject, repFieldId);
                if (!apttusField.Id.Equals(repGroup.GroupByField))
                {
                    Excel.Range rng = repeatingRange.Cells[1, 2];
                    Excel.Range TargetColumnRange = ExcelHelper.NextVerticalCell(rng, repeatingCellField.TargetColumnIndex - 1);
                    if (!apttusField.Id.EndsWith(Constants.APPENDLOOKUPID))
                    {
                        // Dependent Picklist Scenario - Calculate the location of Controlling Picklist which is referred as ReferenceNamedRange
                        Excel.Range ReferenceNamedRange = null;
                        string ReferenceRepeatingCellFieldId = string.Empty;
                        if (apttusField.Datatype == Datatype.Picklist && apttusField.PicklistType == PicklistType.Dependent)
                        {
                            // Find Reference Name Range in case of Dependent Picklist.
                            RetrieveField ReferenceRepeatingCellField = repeatingCellFields.FirstOrDefault(f => f.FieldId == apttusField.ControllingPicklistFieldId);
                            // If field is dependent but controller field is not present on sheet 
                            if (ReferenceRepeatingCellField != null)
                            {
                                ReferenceRepeatingCellFieldId = ReferenceRepeatingCellField.FieldId;
                                ReferenceNamedRange = ExcelHelper.NextVerticalCell(TargetColumnRange, ReferenceRepeatingCellField.TargetColumnIndex - repeatingCellField.TargetColumnIndex);
                            }
                        }

                        // Record Type Scenario - Calculate the location of Record Type (Name) column which is referred as RecordTypeNamedRange
                        Excel.Range RecordTypeNamedRange = null;
                        if (apttusField.Datatype == Datatype.Picklist && repeatingApttusObject.RecordTypes.Count > 0)
                        {
                            ApttusField RecordTypeField = repeatingApttusObject.Fields.FirstOrDefault(f => f.RecordType);
                            if (RecordTypeField != null)
                            {
                                string RecordTypeLookupName = applicationDefinitionManager.GetLookupNameFromLookupId(RecordTypeField.Id);
                                // Check if the Lookup (Name) of Record Type is rendered.
                                if (repeatingCellFields.Exists(f => f.FieldId == RecordTypeLookupName))
                                {
                                    RetrieveField RecordTypeRetrieveField = repeatingCellFields.FirstOrDefault(f => f.FieldId == RecordTypeLookupName);
                                    RecordTypeNamedRange = ExcelHelper.NextVerticalCell(TargetColumnRange, RecordTypeRetrieveField.TargetColumnIndex - repeatingCellField.TargetColumnIndex);
                                }
                            }
                        }

                        ExcelHelper.AddCellRangeValidation(TargetColumnRange, apttusField, new CellValidationInput
                        {
                            ObjectId = applicationDefinitionManager.GetAppObject(repeatingCellField.AppObject).Id,
                            ObjectType = ObjectType.Repeating,
                            LayoutType = LayoutType.Horizontal,
                            ReferenceNamedRange = ReferenceNamedRange,
                            ControllingPicklistFieldId = ReferenceRepeatingCellFieldId,
                            RecordTypeNamedRange = RecordTypeNamedRange
                        });
                    }
                }
                columnIndex++;
                NextColumn++;
            }

            //Prepare the data table because grouping might add new rows and thus we need to insert more rows in excel.
            String sortField = repeatingCellFields.Where(s => s.FieldId == repGroup.SortByField).Select(s => s.FieldId).FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(sortField))
            {
                DataView dv = repeatingCellDataTable.DefaultView;
                if (!String.IsNullOrEmpty(sortField))
                {
                    string sortDirectionField1 = repGroup.SortDirectionField1 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                    string sort = sortField + sortDirectionField1;
                    if (!string.IsNullOrEmpty(repGroup.SortByField2))
                    {
                        string sortDirectionField2 = repGroup.SortDirectionField2 == RepeatingGroupSortDirection.Ascending ? " ASC" : " DESC";
                        sort = sort + ", " + repGroup.SortByField2 + sortDirectionField2;
                    }

                    dv.Sort = sort;

                    //Dispose the unsorted DataTable
                    dataSet.DataTable.Dispose();
                    dataSet.DataTable = null;

                    //Update the newly sorted datatable and provide to the DataManager
                    dataSet.DataTable = dv.ToTable();
                    repeatingCellDataTable = dataSet.DataTable;
                }
            }

            //String rows = String.Format("{0}:{1}", startingRow + 2, startingRow + 1 + repeatingCellDataTable.Rows.Count);
            String ColLetterStart = ExcelHelper.GetExcelColumnName(startingColumn + 2);
            String ColLetterEnd = ExcelHelper.GetExcelColumnName(startingColumn + 1 + repeatingCellDataTable.Rows.Count);
            String cols = String.Format("{0}:{1}", ColLetterStart, ColLetterEnd);
            Excel.Range rangeToInsert = sheet.Columns[cols];
            rangeToInsert.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);

            //After inserting new row, start address changes, so re-assign the starting address of rendering data.
            Excel.Range topLeftHeader = (Excel.Range)sheet.Cells[startingRow, startingColumn];
            Excel.Range topLeftData = (Excel.Range)sheet.Cells[startingRow, startingColumn + 2];
            Excel.Range bottomRightData = (Excel.Range)sheet.Cells[startingRow + columnNames.Count() - 1, repeatingCellDataTable.Rows.Count + startingColumn + 1];
            Excel.Range DataBodyRange = sheet.get_Range(topLeftData, bottomRightData);

            ExcelHelper.ExcelApp.ScreenUpdating = true;

            //Apply Header
            if (!String.IsNullOrWhiteSpace(repGroup.GridHeader))
            {
                //Excel.Range startingAddress = sheet.Range[repeatingCellFields.ElementAt(0).TargetNamedRange];
                Excel.Range header = ExcelHelper.NextVerticalCell(topLeftHeader, -1);
                if (header != null)
                    header.Value = repGroup.GridHeader;
            }

            //Data
            int rowIndex, colIndex;
            object[,] cellData = null;
            try
            {
                cellData = new object[columnNames.Count, repeatingCellDataTable.Rows.Count];
                for (rowIndex = 0; rowIndex < repeatingCellDataTable.Rows.Count; rowIndex++)
                {
                    colIndex = 0;
                    foreach (KeyValuePair<string, bool> columnNamePair in columnNames)
                    {
                        if (columnNamePair.Key == null) //there can be empty column names.
                            continue;
                        int tableColumnIndex = repeatingCellDataTable.Columns.IndexOf(columnNamePair.Key);
                        if (tableColumnIndex != -1)
                        {
                            colIndex = repeatingCellFields.Where(s => s.Type == ObjectType.Repeating && s.FieldId.Equals(columnNamePair.Key)).Select(s => s.TargetColumnIndex).FirstOrDefault();
                            colIndex--;

                            cellData[colIndex, rowIndex] = repeatingCellDataTable.Rows[rowIndex][tableColumnIndex];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Display Action");
                return;
            }

            //Apply the Data in excel.
            DataBodyRange.Value = cellData;
            cellData = null; // Important

            dataSet.DataTable.AcceptChanges();

            // Add the named range data tracker entry for repeating cells
            dataManager.AddDataTracker(new ApttusDataTracker
            {
                Location = repGroup.TargetNamedRange,
                DataSetId = dataSet.Id,
                Type = repeatingCellFields[0].Type
            });

            // ******* IMP : This code should always be after AcceptChanges as it touches *******
            // ******* the VSTO ListObject which can potentially disturb the named range ******* 

            // Recalculating the bottomRightData after VSTO rendering and apply the grid named range.
            bottomRightData = (Excel.Range)sheet.Cells[startingRow + columnNames.Count() - 1, repeatingCellDataTable.Rows.Count + startingColumn + 1];
            int row = bottomRightData.Row;
            int col = bottomRightData.Column;

            Excel.Range FullNamedRange = sheet.get_Range(topLeftHeader, bottomRightData);
            ExcelHelper.AssignNameToRange(FullNamedRange, repGroup.TargetNamedRange);

            //UpdateNamedRangeVisibility(sheet, true, LayoutType.Horizontal);

            ApplyFormulaForHorizontal(FullNamedRange, columnNames, repeatingCellDataTable.Rows.Count);

            dataSet.ColumnNames = (from c in columnNames select c.Key).ToList();
        }

        /// <summary>
        /// Fill data to the cell
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="dataSet"></param>
        public void MapCrossTabCells(CrossTabRetrieveMap Model, ApttusDataSet rowDataSet, ApttusDataSet colDataSet, ApttusDataSet transactionalDataSet)
        {
            if (transactionalDataSet == null)
                return;
            ExcelHelper.ExcelApp.ScreenUpdating = false;
            try
            {
                string CrossTabNamedRange = Model.ColField.TargetNamedRange;
                Excel.Range StartRange = ExcelHelper.GetRange(CrossTabNamedRange);
                StartRange.Value = String.Empty;

                //First Prepare the New Relational(Cross Tab) Data Table.
                DataTable crossTabTable = new DataTable();
                DataTable crossTabIDTable = new DataTable();

                //Add the ColField as a Column for the timebeing as finally this will be nullified.
                crossTabTable.Columns.Add("");
                crossTabIDTable.Columns.Add("");

                // Do row sorting
                if (!String.IsNullOrWhiteSpace(Model.RowSortByFieldId))
                {
                    DataView dv = rowDataSet.DataTable.DefaultView;
                    if (!String.IsNullOrEmpty(Model.RowSortByFieldId))
                    {
                        dv.Sort = Model.RowSortByFieldId + " ASC";

                        //Dispose the unsorted DataTable
                        rowDataSet.DataTable.Dispose();
                        rowDataSet.DataTable = null;

                        //Update the newly sorted datatable and provide to the DataManager
                        rowDataSet.DataTable = dv.ToTable();
                    }
                }
                // Do Col sorting
                if (!String.IsNullOrWhiteSpace(Model.ColSortByFieldId))
                {
                    DataView dv = colDataSet.DataTable.DefaultView;
                    if (!String.IsNullOrEmpty(Model.ColSortByFieldId))
                    {
                        dv.Sort = Model.ColSortByFieldId + " ASC";

                        //Dispose the unsorted DataTable
                        colDataSet.DataTable.Dispose();
                        colDataSet.DataTable = null;

                        //Update the newly sorted datatable and provide to the DataManager
                        colDataSet.DataTable = dv.ToTable();
                    }
                }
                //Now Whatever Rows were retrieved as a part of CrossTab RowField, will be made Columns as part of cross tab table.
                foreach (DataRow dr in colDataSet.DataTable.Rows)
                {
                    crossTabTable.Columns.Add(dr[Model.ColField.FieldId].ToString());
                    crossTabIDTable.Columns.Add(dr[Constants.ID_ATTRIBUTE].ToString());
                }

                //Now add rows in the cross tab table per column field.
                foreach (DataRow dr in rowDataSet.DataTable.Rows)
                {
                    DataRow newTransactionalRow = crossTabTable.NewRow();

                    newTransactionalRow[crossTabTable.Columns[0]] = dr[Model.RowField.FieldId];
                    crossTabTable.Rows.Add(newTransactionalRow);

                    //Create ID Table rows
                    DataRow newIDTableRow = crossTabIDTable.NewRow();
                    newIDTableRow[crossTabIDTable.Columns[0]] = dr[Constants.ID_ATTRIBUTE];
                    crossTabIDTable.Rows.Add(newIDTableRow);
                }

                //set the primary key to the first column name which will always be ColField Name, as this will be needed and handy later on.
                crossTabTable.PrimaryKey = new DataColumn[] { crossTabTable.Columns[0] };
                crossTabIDTable.PrimaryKey = new DataColumn[] { crossTabIDTable.Columns[0] };

                String rowFieldId = Model.RowLookupFieldId;
                String colFieldId = Model.ColLookupFieldId;

                foreach (DataRow dr in transactionalDataSet.DataTable.Rows)
                {
                    string colid = dr[colFieldId] as string;
                    DataColumn dctemp = crossTabIDTable.Columns[colid];
                    DataRow IDTableRow = crossTabIDTable.Rows.Find(dr[rowFieldId]);

                    int Rowno = crossTabIDTable.Rows.IndexOf(IDTableRow);
                    int col = crossTabIDTable.Columns.IndexOf(dctemp);
                    crossTabTable.Rows[Rowno][col] = dr[Model.DataField.FieldId];

                    IDTableRow[dctemp] = dr[Constants.ID_ATTRIBUTE];
                }
                crossTabTable.Columns[0].ColumnName = " "; //Replace the ColField Column Name earlier set, to Empty.                

                int dataStartRow = StartRange.Row;
                int dataStartColumn = StartRange.Column - 1;

                Excel.Worksheet sheet = StartRange.Worksheet;

                //This will be used while re-rendering.
                ResetCrossTabRange(Model);

                //Insert the number of rows that are to rendered as part of ListObject below the startingRow .
                //If the number of rows to be rendered is greater than 1, then only insert new rows, else the existing one row in excel can handle the data
                if (crossTabTable.Rows.Count > 1)
                {
                    string rows = String.Format("{0}:{1}", dataStartRow + 2, dataStartRow + crossTabTable.Rows.Count);
                    Excel.Range rangeToInsert = sheet.Range[rows];
                    rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                }

                //Insert Cols
                //If the number of columns to be rendered is greater than 2, then only insert new columns, else the existing column in excel can handle the data.
                //Why 2 ? : the first column in crossTabTable represents rowfield in excel. second column is from where the columnfield starts.
                if (crossTabTable.Columns.Count > 2)
                {
                    Excel.Range colRangeToInsert = sheet.Range[ExcelHelper.NextHorizontalCell(StartRange, 1), ExcelHelper.NextVerticalCell(ExcelHelper.NextHorizontalCell(StartRange, crossTabTable.Columns.Count - 2), crossTabTable.Rows.Count)];
                    colRangeToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
                }
                Excel.Range topLeftHeader = sheet.Cells[dataStartRow, dataStartColumn];
                //Excel.Range topLeftData = sheet.Cells[dataStartRow + 1, dataStartColumn];

                Excel.Range bottomRightData = sheet.Cells[dataStartRow + crossTabTable.Rows.Count, dataStartColumn + crossTabTable.Columns.Count - 1];
                //Excel.Range topEndHeader = ExcelHelper.NextHorizontalCell(topstartHeader, crossTabIDTable.Columns.Count - 1);
                Excel.Range dataRange = sheet.get_Range(topLeftHeader, bottomRightData);

                // Render Logic
                int rowIndex, colIndex;
                object[,] cellData = new object[crossTabTable.Rows.Count + 1, crossTabTable.Columns.Count];

                for (colIndex = 0; colIndex < crossTabTable.Columns.Count; colIndex++)
                {
                    cellData[0, colIndex] = crossTabTable.Columns[colIndex].ColumnName;
                }

                for (rowIndex = 0; rowIndex < crossTabTable.Rows.Count; rowIndex++)
                {
                    for (colIndex = 0; colIndex < crossTabTable.Columns.Count; colIndex++)
                    {
                        cellData[rowIndex + 1, colIndex] = crossTabTable.Rows[rowIndex][colIndex];
                    }
                }

                // Assign data to excel cell
                dataRange.Value = cellData;
                cellData = null;

                // Apply formulas
                ApplyCrossTabFormulas(StartRange, crossTabTable);

                //Build a cross Tab Data set
                ApttusCrossTabDataSet crossTabDataSet = new ApttusCrossTabDataSet();
                crossTabDataSet.DataTable = crossTabTable;
                crossTabDataSet.IDTable = crossTabIDTable;
                crossTabDataSet.CrossTabID = (from def in applicationDefinitionManager.CrossTabDefinitions
                                              where def.RowHeaderObject.UniqueId.Equals(rowDataSet.AppObjectUniqueID) &
                                              def.ColHeaderObject.UniqueId.Equals(colDataSet.AppObjectUniqueID) &
                                              def.DataObject.UniqueId.Equals(transactionalDataSet.AppObjectUniqueID)
                                              select def.UniqueId).FirstOrDefault();

                crossTabDataSet.Name = transactionalDataSet.Name;

                crossTabDataSet.DataTable.AcceptChanges();

                //Add the cross tab data set to the data manager
                crossTabDataSet = dataManager.AddData(crossTabDataSet);

                // Add the named range data tracker entry for repeating cells
                dataManager.AddDataTracker(new ApttusDataTracker
                {
                    Location = Model.DataField.TargetNamedRange,
                    DataSetId = crossTabDataSet.Id,
                    Type = ObjectType.CrossTab
                });

                ExtendCrossTabRange(Model, crossTabTable.Rows.Count, crossTabTable.Columns.Count);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Display Action");
            }
            ExcelHelper.ExcelApp.ScreenUpdating = true;
        }

        /// <summary>
        /// Extends all the 3 named ranges such as RowRange, ColRange and DataRange.
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        void ExtendCrossTabRange(CrossTabRetrieveMap Model, int rowCount, int colCount)
        {
            Excel.Range colRange = ExcelHelper.GetRange(Model.ColField.TargetNamedRange);
            Excel.Range rowRange = ExcelHelper.GetRange(Model.RowField.TargetNamedRange);
            Excel.Range dataRange = ExcelHelper.GetRange(Model.DataField.TargetNamedRange);

            Excel.Worksheet sheet = colRange.Worksheet;

            Excel.Range fullColRange = sheet.Range[colRange, ExcelHelper.NextHorizontalCell(colRange, colCount - 2)];
            ExcelHelper.AssignNameToRange(fullColRange, Model.ColField.TargetNamedRange);

            Excel.Range fullRowRange = sheet.Range[rowRange, ExcelHelper.NextVerticalCell(rowRange, rowCount - 1)];
            ExcelHelper.AssignNameToRange(fullRowRange, Model.RowField.TargetNamedRange);

            Excel.Range bottomRightDataCell = sheet.Cells[fullRowRange.Row + rowCount - 1, fullColRange.Column + colCount - 2];
            Excel.Range fullDataRange = sheet.Range[dataRange, bottomRightDataCell];
            ExcelHelper.AssignNameToRange(fullDataRange, Model.DataField.TargetNamedRange);
        }

        /// <summary>
        /// Will be used to reset the rendered range to its original form, so that it could be used to re-render the data, by preserving the formatting.
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        void ResetCrossTabRange(CrossTabRetrieveMap Model)
        {
            Excel.Range colRange = ExcelHelper.GetRange(Model.ColField.TargetNamedRange);
            Excel.Range rowRange = ExcelHelper.GetRange(Model.RowField.TargetNamedRange);
            Excel.Range dataRange = ExcelHelper.GetRange(Model.DataField.TargetNamedRange);

            Excel.Worksheet sheet = colRange.Worksheet;

            if (rowRange.Rows.Count > 1)
            {
                Excel.Range rowRangeToDelete = sheet.Range[ExcelHelper.NextVerticalCell(rowRange, 1), ExcelHelper.NextVerticalCell(rowRange, rowRange.Rows.Count - 1)];
                rowRangeToDelete.EntireRow.Rows.Delete();
            }

            int colCount = colRange.Columns.Count;
            if (colCount > 1)
            {
                Excel.Range colRangeToDelete = sheet.Range[ExcelHelper.NextHorizontalCell(colRange, 1), ExcelHelper.NextHorizontalCell(colRange, colCount - 1)];
                colRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);

                if (dataRange.Rows.Count == 1)
                {
                    Excel.Range dataRangeToDelete = sheet.Range[ExcelHelper.NextHorizontalCell(dataRange, 1), ExcelHelper.NextHorizontalCell(dataRange, colCount - 1)];
                    dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                }
            }

            colRange.Value = String.Empty;
            rowRange.Value = String.Empty;
            dataRange.Value = String.Empty;
        }

        /// <summary>
        /// Apply formula to cross tab
        /// </summary>
        /// <param name="StartRange"></param>
        /// <param name="table"></param>
        private void ApplyCrossTabFormulas(Excel.Range StartRange, DataTable table)
        {
            Excel.Worksheet sheet = StartRange.Worksheet;

            //Apply Formula Column wise
            Excel.Range ColumnFormulaRange = ExcelHelper.NextVerticalCell(StartRange, table.Rows.Count + 2);
            Excel.Range totalRowRange = sheet.Range[ColumnFormulaRange, ExcelHelper.NextHorizontalCell(ColumnFormulaRange, table.Columns.Count - 2)];
            ColumnFormulaRange.Copy(totalRowRange);

            //Apply Formula Row Wise
            Excel.Range RowFormulaRange = ExcelHelper.NextVerticalCell(ExcelHelper.NextHorizontalCell(StartRange, table.Columns.Count), 1);
            Excel.Range totalColRange = sheet.Range[RowFormulaRange, ExcelHelper.NextVerticalCell(RowFormulaRange, table.Rows.Count - 1)];
            RowFormulaRange.Copy(totalColRange);
        }

        /// <summary>
        /// Clear Data from sheet
        /// </summary>
        /// <param name="Model"></param>
        internal void ClearData(RetrieveActionModel retrieveAction, RetrieveMap Model, bool InputData, string[] InputDataName)
        {
            try
            {
                if (retrieveAction.DisableExcelEvents)
                    ExcelHelper.ExcelApp.EnableEvents = false;

                WorkbookEventManager.GetInstance.DisableExcelEventsRenderingCalculation();

                ConfigurationManager configManager = ConfigurationManager.GetInstance;
                RetrieveMap retrieveMap = configManager.RetrieveMaps.Where(sm => sm.Id.Equals(Model.Id)).FirstOrDefault();

                // Remove Data Protection before clear data
                foreach (var WorksheetName in dataManager.DataProtection.Select(dp => dp.WorksheetName).Distinct())
                {
                    Excel.Worksheet sheet = ExcelHelper.GetWorkSheet(WorksheetName);
                    ExcelHelper.UpdateSheetProtection(sheet, false);
                }

                //Clear Independent Fields
                List<RetrieveField> independentRetrieveFields = retrieveMap.RetrieveFields.Where(rf => rf.Type == ObjectType.Independent).ToList();
                if (independentRetrieveFields.Count > 0)
                {
                    List<Guid> independentAppObjects = independentRetrieveFields.Select(rf => rf.AppObject).Distinct().ToList();
                    foreach (Guid independentAppObject in independentAppObjects)
                    {
                        // Do Clear Excel Contents, For all Independent fields
                        ClearIndependentCells(independentRetrieveFields.Where(rf => rf.AppObject.Equals(independentAppObject)).ToList());

                        List<RetrieveField> fields = independentRetrieveFields.Where(rf => rf.AppObject.Equals(independentAppObject)).ToList();
                        Guid dataSetId = Guid.Empty;
                        if (InputData)
                        {
                            ApttusDataSet dataSet = dataManager.ResolveInput(InputDataName, applicationDefinitionManager.GetAppObject(independentAppObject));
                            if (dataSet != null)
                                dataSetId = dataSet.Id;

                            ClearAttachmentFolders(dataSet);
                        }

                        foreach (RetrieveField field in fields)
                            dataManager.RemoveDataTracker(field.TargetNamedRange, dataSetId);
                    }
                }

                //Clear Repeating Group
                foreach (RepeatingGroup repeatingGroup in retrieveMap.RepeatingGroups)
                {
                    if (!repeatingGroup.Layout.Equals("Horizontal"))
                    {
                        // Do Clear Excel Contents
                        ExcelHelper.ClearRepeatingCells(repeatingGroup.TargetNamedRange, repeatingGroup.AppObject);
                        // Clear the DataTracker Entry for the repeating cells

                        Guid dataSetId = Guid.Empty;
                        if (InputData)
                        {
                            ApttusDataSet dataSet = dataManager.ResolveInput(InputDataName, applicationDefinitionManager.GetAppObject(repeatingGroup.AppObject));
                            if (dataSet != null)
                                dataSetId = dataSet.Id;

                            ClearAttachmentFolders(dataSet);
                        }
                        dataManager.RemoveDataTracker(repeatingGroup.TargetNamedRange, dataSetId);
                    }
                }

                // Apply Data Protection after clear data
                foreach (var WorksheetName in dataManager.DataProtection.Select(dp => dp.WorksheetName).Distinct())
                {
                    Excel.Worksheet sheet = ExcelHelper.GetWorkSheet(WorksheetName);
                    ExcelHelper.UpdateSheetProtection(sheet, true);
                }

                // Clear attachments datatable
                //attachmentManager.ClearAttachmentData();
            }
            finally
            {
                if (retrieveAction.DisableExcelEvents)
                    ExcelHelper.ExcelApp.EnableEvents = true;

                WorkbookEventManager.GetInstance.EnableExcelEventsRenderingCalculation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        private void ClearAttachmentFolders(ApttusDataSet dataSet)
        {
            if (dataSet != null && dataSet.DataTable != null)
            {
                if (dataSet.DataTable.Columns.Contains(Constants.ATTACHMENT))
                {
                    foreach (DataRow row in dataSet.DataTable.Rows)
                    {
                        attachmentManager.RemoveRecord(dataSet, row[Constants.ID_ATTRIBUTE] as string);
                    }
                }
            }
        }

        /// <summary>
        /// Clear Independent Cells
        /// </summary>
        /// <param name="field"></param>
        private void ClearIndependentCells(List<RetrieveField> fields)
        {
            string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
            ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);
            RowHighLighting rowHighLighting = null;

            if (appInstance != null)
            {
                rowHighLighting = appInstance.RowHighLightingData;
            }
            foreach (RetrieveField field in fields)
            {
                Excel.Range independentDataRange = ExcelHelper.GetRange(field.TargetNamedRange);

                // Clear Independent Data Range
                independentDataRange.ClearContents();

                if (rowHighLighting != null && !rowHighLighting.isCleared)
                {
                    List<string> highlightedRanges = rowHighLighting.GetRanges();
                    if (highlightedRanges.Exists(f => f.Equals(field.TargetLocation)))
                    {
                        independentDataRange.ClearComments();
                        independentDataRange.Interior.Color = rowHighLighting.GetColor(field.TargetLocation);
                    }
                }
                //ExcelHelper.SetCellBorderLineStyle(independentDataRange, Excel.XlLineStyle.xlLineStyleNone);
            }
        }
    }
}
