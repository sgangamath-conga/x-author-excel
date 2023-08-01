/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public enum DataFieldRendering
    {
        None,
        RenderDataHorizontally,
        RenderDataVertically,
        RenderDataOnCell,
        RenderDataHorizontallyAndVertically
    };

    public class GroupedFieldRecord
    {
        public int Records { get; set; }
        public string Filter { get; set; }
    }

    public class MatrixActionView
    {
        // Get DataManager and ConfigurationManager instance
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        static List<CellLocation> lstCellLocation = new List<CellLocation>(); // this has to be initialized according to your formulas
        // Matrix data tables for in memory use
        DataTable matrixRowData = null;
        DataTable matrixColumnData = null;
        DataTable matrixData = null;

        List<RetrieveField> individualFields = new List<RetrieveField>();

        MatrixMap Model;
        string[] InputDataName;
        List<InputData> inputData;

        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;

        private string newRecordGuid = "NewRecordGuid";
        List<MatrixField> sortedMatrixGroupedFields;
        Dictionary<MatrixField, List<string>> filterPerGroupedfield = new Dictionary<MatrixField, List<string>>();
        //Dictionary<MatrixGroupedField, List<GroupedFieldRecord>> groupedRecordPerField = new Dictionary<MatrixGroupedField, List<GroupedFieldRecord>>();
        Dictionary<string, List<GroupedFieldRecord>> groupedRecordsPerParentFilter = new Dictionary<string, List<GroupedFieldRecord>>();
        Dictionary<string, GroupedFieldRecord> groupedRecordPerFilter = new Dictionary<string,GroupedFieldRecord>();

        private bool bIsColumnRenderedOnce;
        private Dictionary<MatrixField, bool> renderingPerGroupedField = new Dictionary<MatrixField, bool>();
        /// <summary>
        /// 
        /// </summary>
        public MatrixActionView()
        {            
            // Initialize Row & Column Datatables
            matrixRowData = new DataTable();
            matrixRowData.Columns.Add(new DataColumn(Constants.ID_ATTRIBUTE));
            matrixRowData.Columns.Add(new DataColumn(Constants.VALUE_COLUMN));
            matrixRowData.Columns.Add(new DataColumn(Constants.MATRIX_ROWID));
            matrixRowData.Columns.Add(new DataColumn(newRecordGuid));

            matrixColumnData = new DataTable();
            matrixColumnData.Columns.Add(new DataColumn(Constants.ID_ATTRIBUTE));
            matrixColumnData.Columns.Add(new DataColumn(Constants.VALUE_COLUMN));
            matrixColumnData.Columns.Add(new DataColumn(Constants.MATRIX_COLUMNID));
            matrixColumnData.Columns.Add(new DataColumn(Constants.GROUPEDCOLUMNFILTER));
            matrixColumnData.Columns.Add(new DataColumn(newRecordGuid));

            matrixData = new DataTable();
            matrixData.Columns.Add(new DataColumn(Constants.ID_ATTRIBUTE));
            matrixData.Columns.Add(new DataColumn(Constants.VALUE_COLUMN));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_DATAFIELDID));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_DATAROWINDEX, typeof(int)));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_DATACOLINDEX, typeof(int)));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_DATAROWVALUE));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_DATACOLVALUE));

            // For Add Matrix Row and Column case
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_ISROWVALUE, typeof(bool)));
            matrixData.Columns.Add(new DataColumn(Constants.MATRIX_ISCOLUMNVALUE, typeof(bool))); 

        }

        private void CreateDataFieldEntry(object id, object value, Guid dataFieldId, int dataRowIndex, int dataColIndex, string dataRowValue, string dataColValue)
        {
            DataRow matrixDataRow = matrixData.NewRow();
            matrixDataRow[Constants.ID_ATTRIBUTE] = id;
            matrixDataRow[Constants.VALUE_COLUMN] = value;
            matrixDataRow[Constants.MATRIX_DATAFIELDID] = dataFieldId;
            matrixDataRow[Constants.MATRIX_DATAROWINDEX] = dataRowIndex;
            matrixDataRow[Constants.MATRIX_DATACOLINDEX] = dataColIndex;
            matrixDataRow[Constants.MATRIX_DATAROWVALUE] = dataRowValue;
            matrixDataRow[Constants.MATRIX_DATACOLVALUE] = dataColValue;

            matrixData.Rows.Add(matrixDataRow);
        }

        public static DataFieldRendering QueryDataFieldRendering(MatrixField rowField, MatrixField colField)
        {
            if (rowField.RenderingType == MatrixRenderingType.Static && colField.RenderingType == MatrixRenderingType.Static)
                return DataFieldRendering.RenderDataOnCell;
            else if (rowField.RenderingType == MatrixRenderingType.Static && colField.RenderingType == MatrixRenderingType.Dynamic)
                return DataFieldRendering.RenderDataHorizontally;
            else if (rowField.RenderingType == MatrixRenderingType.Dynamic && colField.RenderingType == MatrixRenderingType.Static)
                return DataFieldRendering.RenderDataVertically;
            else if (rowField.RenderingType == MatrixRenderingType.Dynamic && colField.RenderingType == MatrixRenderingType.Dynamic)
                return DataFieldRendering.RenderDataHorizontallyAndVertically;

            return DataFieldRendering.None;
        }

        internal void ConvertIndividualFields()
        {
            individualFields = (from f in this.Model.IndependentFields
                                    select new RetrieveField
                                    {
                                        AppObject = f.AppObjectUniqueID,
                                        FieldId = f.FieldId,
                                        FieldName = f.FieldName,
                                        Type = ObjectType.Independent,
                                        TargetLocation = f.TargetLocation,
                                        TargetNamedRange = f.TargetNamedRange,
                                        DataType = f.DataType,
                                        TargetColumnIndex = 1
                                    }).ToList();
        }

        private bool AreMatrixFieldsVisible(out StringBuilder invisibleFields, out StringBuilder invisibleFilters)
        {
            invisibleFields = new StringBuilder();
            invisibleFilters = new StringBuilder();

            bool bVisible = true;
            HashSet<string> uniquefields = new HashSet<string>();

            foreach (MatrixField rowField in Model.MatrixRow.MatrixFields.GroupBy(mf => mf.FieldId).Select(mf => mf.First()))
            {
                bool bTempVisible = fieldLevelSecurity.IsFieldVisible(rowField.AppObjectUniqueID, rowField.FieldId);
                bVisible = bVisible && bTempVisible;
                if (!bTempVisible)
                    uniquefields.Add(rowField.FieldName);
            }

            foreach (MatrixField colField in Model.MatrixColumn.MatrixFields.GroupBy(mf => mf.FieldId).Select(mf => mf.First()))
            {
                bool bTempVisible = fieldLevelSecurity.IsFieldVisible(colField.AppObjectUniqueID, colField.FieldId);
                bVisible = bVisible && bTempVisible;
                if (!bTempVisible)
                    uniquefields.Add(colField.FieldName);
            }

            foreach (MatrixField dataField in Model.MatrixData.MatrixDataFields.GroupBy(mf => mf.FieldId).Select(mf => mf.First()))
            {
                bool bTempVisible = fieldLevelSecurity.IsFieldVisible(dataField.AppObjectUniqueID, dataField.FieldId);
                bVisible = bVisible && bTempVisible;
                if (!bTempVisible)
                    uniquefields.Add(dataField.FieldName);
            }

            if (Model.MatrixComponents != null)
            {
                foreach (MatrixComponent comp in Model.MatrixComponents)
                {
                    List<SearchFilterGroup> searchFilterGroups = comp.WhereFilterGroups;
                    if (searchFilterGroups != null && searchFilterGroups.Count > 0)
                    {
                        foreach (SearchFilterGroup filterGroup in searchFilterGroups)
                        {
                            List<SearchFilter> searchFilters = filterGroup.Filters;
                            if (searchFilters != null && searchFilters.Count > 0)
                            {
                                foreach (SearchFilter filter in searchFilters)
                                {
                                    if (!fieldLevelSecurity.IsFieldVisible(filter.AppObjectUniqueId, filter.FieldId))
                                    {
                                        ApttusField f = applicationDefinitionManager.GetField(filter.AppObjectUniqueId,filter.FieldId);
                                        invisibleFilters.Append(f.Name).Append(",");
                                        bVisible = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (string str in uniquefields)
                invisibleFields.Append(str).Append(",");

            return bVisible;
        }
        /// <summary>
        /// TODO:: add required parameters and logic in method
        /// </summary>
        public void FillData(MatrixMap Model, bool InputData, string[] InputDataNames)
        {
            try
            {
                // no need to call this in batch mode
                if (ObjectManager.RuntimeMode != RuntimeMode.Batch)
                    Globals.ThisAddIn.Application.ScreenUpdating = false;

                // Assign Model and Inputdata
                this.Model = Model;
                this.InputDataName = InputDataNames;
                this.inputData = ExpressionBuilderHelper.GetInputDataName(InputDataNames);

                StringBuilder invisibleFields, filterFields;
                if (!AreMatrixFieldsVisible(out invisibleFields, out filterFields))
                {
                    string sInvisibleFields = invisibleFields.ToString();
                    string sFilterFields = filterFields.ToString();

                    if (!string.IsNullOrEmpty(sInvisibleFields))
                        ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("MATRIXACTVIEW_FillData_InfoMsg"), sInvisibleFields, Globals.ThisAddIn.userInfo.UserFullName,resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
                    else if (!string.IsNullOrEmpty(sFilterFields))
                        ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("MATRIXACTVIEW_FillData_InfoMsg"), sInvisibleFields, Globals.ThisAddIn.userInfo.UserFullName,resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
                    return;
                }

                foreach (MatrixField colfield in Model.MatrixColumn.MatrixFields)
                    foreach(MatrixField groupedField in colfield.MatrixGroupedFields)
                        renderingPerGroupedField[groupedField] = false;

                // Reset matrix data ranges
                ResetMatrixRange(Model);

                // Set Formulas for MatrixMap
                SetMatrixFormulas(Model);

                //Process Individual Fields
                ProcessMatrixIndividualFields(Model, inputData);

                // Process Matrix Row 
                ProcessMatrixRows(Model, inputData);
                if (matrixRowData.Rows.Count == 0)
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXACTVIEW_FillDataNOROW_InfoMsg"), resourceManager.GetResource("MATRIXACTVIEW_FillDataNOROWCAP_InfoMsg"));
                    return;
                }

                // Process Matrix Column 
                ProcessMatrixColumns();
                if (matrixColumnData.Rows.Count == 0)
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXACTVIEW_FillDataNOCOL_InfoMsg"), resourceManager.GetResource("MATRIXACTVIEW_FillDataNOCOLCAP_InfoMsg"));
                    return;
                }

                // Process Matrix Data
                ProcessMatrixData(Model, inputData);                

            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Matrix Action");
            }
            finally
            {
                if (ObjectManager.RuntimeMode != RuntimeMode.Batch)
                    Globals.ThisAddIn.Application.ScreenUpdating = true;
            }

        }

        private void ProcessMatrixIndividualFields(MatrixMap Model, List<InputData> inputData)
        {
            ConvertIndividualFields();

            foreach (MatrixField independentField in Model.IndependentFields)
            {
                ApttusObject appObject = applicationDefinitionManager.GetAppObject(independentField.AppObjectUniqueID);
                ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(inputData, appObject);

                if (dataSet != null && dataSet.DataTable != null && dataSet.DataTable.Rows != null && dataSet.DataTable.Rows.Count == 1)
                {
                    RetrieveField currentField = individualFields.Where(f => f.FieldId.Equals(independentField.FieldId) && f.AppObject.Equals(independentField.AppObjectUniqueID)).FirstOrDefault();
                    RetrieveHelper.PopulateCellIndependent(dataSet.DataTable.Rows[0][independentField.FieldId], appObject, individualFields, currentField, true);
                }
            }
        }

        internal bool ProcessMatrixRows(MatrixMap Model, List<InputData> inputData)
        {
            // Loop through each matrix field and their respective Object data to prepare Matrix Data
            foreach (MatrixField rowField in Model.MatrixRow.MatrixFields)
            {
                ApttusObject appObject = applicationDefinitionManager.GetAppObject(rowField.AppObjectUniqueID);
                ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(inputData, appObject);

                string rowFieldLookUpId = rowField.FieldId.EndsWith(Constants.APPENDLOOKUPID) ? applicationDefinitionManager.GetLookupIdFromLookupName(rowField.FieldId) : 
                                            Constants.ID_ATTRIBUTE;

                // Render row data for this matrix field
                Excel.Range targetRowRange = ExcelHelper.GetRange(rowField.TargetNamedRange);

                if (rowField.RenderingType == MatrixRenderingType.Dynamic)
                {
                    if (dataSet != null && dataSet.DataTable.Rows.Count > 0)
                    {
                        bool isLookupField = false;
                        string rowFieldForUniqueValues = GetMatrixFieldForUniqueDataFiltering(rowField.FieldId, out isLookupField);
                        ////DataTable dt = dataSet.DataTable;
                        // Get all Unique Column values
                        DataTable dt = dataSet.DataTable.AsEnumerable()
                               .GroupBy(r => new { Col1 = r[rowFieldForUniqueValues] })
                               .Select(g => g.First())
                               .CopyToDataTable();

                        // If sort field is configured sort Column data
                        if (!string.IsNullOrEmpty(rowField.SortFieldId))
                        {
                            DataView dv = dt.DefaultView;
                            dv.Sort = rowField.SortFieldId + " ASC";
                            dt = dv.ToTable();
                        }

                        // Traverse through all data rows and add it matrix row data
                        foreach(DataRow rowData in dt.Rows)
                        {
                            DataRow dr = matrixRowData.NewRow();
                            dr[Constants.ID_ATTRIBUTE] = rowData[rowFieldLookUpId];
                            dr[Constants.VALUE_COLUMN] = rowData[rowField.FieldId];
                            dr[Constants.MATRIX_ROWID] = rowField.Id;

                            matrixRowData.Rows.Add(dr);
                        }

                        // Expand the section or rows to be rendered, Get cell just current row field and than expand for number rows to be added
                        Excel.Range cellBelow = ExcelHelper.NextVerticalCell(targetRowRange, 1);
                        
                        // Get column data for current matrixColumn field being rendered.
                        DataRow[] currentRowData = matrixRowData.Select(Constants.MATRIX_ROWID + " = '" + rowField.Id.ToString() + "'");

                        if (currentRowData.Length > 1)   // If there is only one record retrieved no need to expand range
                        {
                            Excel.Range rangeToInsert = cellBelow.Resize[currentRowData.Length - 1];
                            rangeToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                        }
                        // Create range on which data needs to be rendered
                        Excel.Range expandedRowRange = currentRowData.Length > 1 ? targetRowRange.Resize[currentRowData.Length] : targetRowRange;

                        object[,] cellData = new object[currentRowData.Length, 1];
                        for (int i = 0; i < currentRowData.Length; i++)
                            cellData[i, 0] = currentRowData[i][Constants.VALUE_COLUMN];

                        expandedRowRange.Value = cellData;
                        cellData = null;

                        // Expand named range to expanded section
                        ExcelHelper.AssignNameToRange(expandedRowRange, rowField.TargetNamedRange);
                    }
                }
                else if (rowField.RenderingType == MatrixRenderingType.Static)
                {
                    // For Static no rendering should be done, so read value from Location and add it row data table
                    if (rowField.ValueType == MatrixValueType.FieldValue)
                    {
                        // Find ID column in dataset if it exists
                        DataRow[] staticRow = dataSet.DataTable.Select(rowField.FieldId + "='" + Convert.ToString(targetRowRange.Value) + "'");

                        // Add row in matrix row Data Table
                        DataRow dr = matrixRowData.NewRow();
                        dr[Constants.ID_ATTRIBUTE] = staticRow.Count() > 0 ? staticRow[0][rowFieldLookUpId] : string.Empty;
                        dr[Constants.VALUE_COLUMN] = Convert.ToString(targetRowRange.Value);
                        dr[Constants.MATRIX_ROWID] = rowField.Id;
                        dr[newRecordGuid] = Guid.NewGuid().ToString();

                        matrixRowData.Rows.Add(dr);
                    }
                    else if (rowField.ValueType == MatrixValueType.FieldLabel)
                    {
                        targetRowRange.Value = rowField.FieldName;

                        // Add row in matrix row Data Table+
                        DataRow dr = matrixRowData.NewRow();
                        dr[Constants.ID_ATTRIBUTE] = string.Empty;
                        dr[Constants.VALUE_COLUMN] = rowField.FieldId;
                        dr[Constants.MATRIX_ROWID] = rowField.Id;

                        matrixRowData.Rows.Add(dr);
                    }
                }

            }
            return true;
        }        
        
        internal int ProcessMatrixColumns(MatrixField colField, MatrixMap Model, List<InputData> inputData, string filter, MatrixField groupedColumn)
        {
            int nColumns = 1;
            if (groupedColumn == null || (groupedColumn != null && colField.MatrixGroupedParentId == groupedColumn.Id))
            {
                ApttusObject appObject = applicationDefinitionManager.GetAppObject(colField.AppObjectUniqueID);
                ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(inputData, appObject);

                string colFieldLookUpId = colField.FieldId.EndsWith(Constants.APPENDLOOKUPID) ? applicationDefinitionManager.GetLookupIdFromLookupName(colField.FieldId) :
                            Constants.ID_ATTRIBUTE;

                // Render column data for this matrix field
                Excel.Range targetColumnRange = ExcelHelper.GetRange(colField.TargetNamedRange);

                if (colField.RenderingType == MatrixRenderingType.Dynamic)
                {
                    if (dataSet != null && dataSet.DataTable.Rows.Count > 0)
                    {
                        bool isLookupField = false;
                        string colFieldForUniqueValues = GetMatrixFieldForUniqueDataFiltering(colField.FieldId, out isLookupField);
                        // Get all Unique Column values
                        DataRow[] rows = dataSet.DataTable.Select(filter);
                        if (rows.Count() == 0)
                            return 0;

                        DataTable dt = rows.AsEnumerable()
                               .GroupBy(r => new { Col1 = r[colFieldForUniqueValues] })
                               .Select(g => g.First())
                               .CopyToDataTable();

                        // If sort field is configured sort Column data
                        if (!string.IsNullOrEmpty(colField.SortFieldId))
                        {
                            DataView dv = dt.DefaultView;
                            dv.Sort = colField.SortFieldId + " ASC";
                            dt = dv.ToTable();
                        }

                        // Traverse through all data rows and add it matrix row data
                        foreach (DataRow columnData in dt.Rows)
                        {
                            DataRow dr = matrixColumnData.NewRow();
                            dr[Constants.ID_ATTRIBUTE] = columnData[colFieldLookUpId];
                            dr[Constants.VALUE_COLUMN] = columnData[colField.FieldId];
                            dr[Constants.MATRIX_COLUMNID] = colField.Id;
                            dr[Constants.GROUPEDCOLUMNFILTER] = filter;
                            matrixColumnData.Rows.Add(dr);
                        }

                        // Expand the section or rows to be rendered, Get cell just current row field and than expand for number rows to be added
                        Excel.Range cellBesides = ExcelHelper.NextHorizontalCell(targetColumnRange, targetColumnRange.Columns.Count);


                        // Get column data for current matrixColumn field being rendered.
                        DataRow[] currentColData = matrixColumnData.GetChanges().Select(Constants.MATRIX_COLUMNID + " = '" + colField.Id.ToString() + "'");
                        nColumns = currentColData.Length;

                        int availableCell = 0;
                        if (groupedColumn != null)
                        {
                            if (!bIsColumnRenderedOnce)
                                availableCell = 1;
                            else
                                availableCell = 0;
                        }
                        else
                            availableCell = currentColData.Length > 1 ? 1 : 0;

                        bool bResizeRange = matrixColumnData.Rows.Count > 1;
                        if (bResizeRange)
                        {
                            Excel.Range rangeToInsert = cellBesides.Resize[Type.Missing, currentColData.Length - availableCell];
                            rangeToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
                        }

                        // Create range on which data needs to be rendered
                        Excel.Range renderingDataColumnRange = bResizeRange ?
                                                            targetColumnRange.Worksheet.Range[ExcelHelper.NextHorizontalCell(cellBesides, -currentColData.Length), ExcelHelper.NextHorizontalCell(cellBesides, -1)]
                                                            : targetColumnRange;

                        object[,] cellData = new object[1, currentColData.Length];
                        for (int i = 0; i < currentColData.Length; i++)
                            cellData[0, i] = currentColData[i][Constants.VALUE_COLUMN];

                        renderingDataColumnRange.Value = cellData;
                        cellData = null;

                        // Expand named range to expanded section
                        Excel.Range fullExpandedRange = targetColumnRange.Worksheet.Range[targetColumnRange.Cells[1, 1], ExcelHelper.NextHorizontalCell(cellBesides, -1)];
                        ExcelHelper.AssignNameToRange(fullExpandedRange, colField.TargetNamedRange);

                        matrixColumnData.AcceptChanges();

                        bIsColumnRenderedOnce = true;
                    }
                }
                else if (colField.RenderingType == MatrixRenderingType.Static)
                {
                    // For Static no rendering should be done, so read value from Location and add it row data table
                    if (colField.ValueType == MatrixValueType.FieldValue)
                    {
                        // Find ID column in dataset if it exists
                        DataRow[] staticColumn = dataSet.DataTable.Select(colField.FieldId + "='" + Convert.ToString(targetColumnRange.Value) + "'");

                        // Add row in matrix row Data Table
                        DataRow dr = matrixColumnData.NewRow();
                        dr[Constants.ID_ATTRIBUTE] = staticColumn.Count() > 0 ? staticColumn[0][colFieldLookUpId] : string.Empty;
                        dr[Constants.VALUE_COLUMN] = Convert.ToString(targetColumnRange.Value);
                        dr[Constants.MATRIX_COLUMNID] = colField.Id;
                        dr[newRecordGuid] = Guid.NewGuid().ToString();

                        matrixColumnData.Rows.Add(dr);
                    }
                    else if (colField.ValueType == MatrixValueType.FieldLabel)
                    {
                        targetColumnRange.Value = colField.FieldName;

                        // Add row in matrix row Data Table
                        DataRow dr = matrixColumnData.NewRow();
                        dr[Constants.ID_ATTRIBUTE] = string.Empty;
                        dr[Constants.VALUE_COLUMN] = colField.FieldId;
                        dr[Constants.MATRIX_COLUMNID] = colField.Id;

                        matrixColumnData.Rows.Add(dr);
                    }
                }
            }

            return nColumns;
        }       

        internal bool ProcessMatrixData(MatrixMap Model, List<InputData> inputData)
        {
            bool blnExpandFormula = false;            
            foreach (MatrixDataField dataField in Model.MatrixData.MatrixDataFields)
            {
                ApttusObject dataFieldAppObject = applicationDefinitionManager.GetAppObject(dataField.AppObjectUniqueID);
                ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(inputData, dataFieldAppObject);

                if (dataField.RenderingType == MatrixRenderingType.Dynamic)
                {
                    if (dataSet != null)    // && dataSet.DataTable.Rows.Count > 0
                    {
                        // Apply formula on last data row
                        if (Model.MatrixData.MatrixDataFields.IndexOf(dataField) == Model.MatrixData.MatrixDataFields.Count - 1)
                            blnExpandFormula = true; 
    
                        // Get row and column fields of current data field
                        MatrixField matrixColumn = Model.MatrixColumn.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();
                        MatrixField matrixRow = Model.MatrixRow.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();                        

                        string columnLookUpFieldId = string.Empty, rowLookUpFieldId = string.Empty;
                        bool rowFieldIsLookUp = false, columnFieldIsLookUp = false;

                        string evalAndOr = matrixRow.ValueType.Equals(MatrixValueType.FieldLabel) || matrixColumn.ValueType.Equals(MatrixValueType.FieldLabel) ?
                            " OR " : " AND ";

                        if ((dataField.AppObjectUniqueID == matrixColumn.AppObjectUniqueID) && (dataField.AppObjectUniqueID == matrixRow.AppObjectUniqueID))
                        {
                            columnLookUpFieldId = matrixColumn.FieldId;
                            if (matrixColumn.RenderingType == MatrixRenderingType.Static && matrixColumn.ValueType == MatrixValueType.FieldLabel)
                            {
                                columnLookUpFieldId = Constants.ID_ATTRIBUTE;
                                columnFieldIsLookUp = true;
                            }
                            else if (matrixColumn.RenderingType == MatrixRenderingType.Dynamic && matrixColumn.ValueType == MatrixValueType.FieldValue)
                            {
                                bool isLookupField = false;
                                columnLookUpFieldId = GetMatrixFieldForUniqueDataFiltering(matrixColumn.FieldId, out isLookupField);
                                columnFieldIsLookUp = isLookupField;
                            }
                        }
                        else if (matrixColumn.AppObjectUniqueID != dataFieldAppObject.UniqueId)
                        {
                            columnLookUpFieldId = dataField.ColumnLookupId;
                            if (string.IsNullOrEmpty(columnLookUpFieldId)) //For Backward Compatability. For those apps whose config doesn't have value of MatrixDataField.ColumnLookupId
                                columnLookUpFieldId = getLookFieldId(dataFieldAppObject, applicationDefinitionManager.GetAppObject(matrixColumn.AppObjectUniqueID).Id);

                            columnFieldIsLookUp = true;
                        }
                        else if (matrixColumn.AppObjectUniqueID == dataFieldAppObject.UniqueId)
                        {
                            columnLookUpFieldId = matrixColumn.FieldId;
                            if (matrixColumn.RenderingType == MatrixRenderingType.Static && matrixColumn.ValueType == MatrixValueType.FieldLabel)
                            {
                                columnLookUpFieldId = matrixColumn.FieldId.EndsWith(Constants.APPENDLOOKUPID) ? applicationDefinitionManager.GetLookupIdFromLookupName(matrixColumn.FieldId) : Constants.ID_ATTRIBUTE;
                                columnFieldIsLookUp = true;
                            }
                            else if(matrixColumn.RenderingType == MatrixRenderingType.Dynamic && matrixColumn.ValueType == MatrixValueType.FieldValue)
                            {
                                columnFieldIsLookUp = matrixColumn.FieldId.EndsWith(Constants.APPENDLOOKUPID);
                                columnLookUpFieldId = columnFieldIsLookUp ? applicationDefinitionManager.GetLookupIdFromLookupName(matrixColumn.FieldId) : matrixColumn.FieldId;
                            }
                        }

                        if ((dataField.AppObjectUniqueID == matrixColumn.AppObjectUniqueID) && (dataField.AppObjectUniqueID == matrixRow.AppObjectUniqueID))
                        {
                            rowLookUpFieldId = matrixRow.FieldId;
                            if (matrixRow.RenderingType == MatrixRenderingType.Static && matrixRow.ValueType == MatrixValueType.FieldLabel)
                            {
                                rowLookUpFieldId = Constants.ID_ATTRIBUTE;
                                rowFieldIsLookUp = true;
                            }
                            else if (matrixRow.RenderingType == MatrixRenderingType.Dynamic && matrixRow.ValueType == MatrixValueType.FieldValue)
                            {
                                bool isLookupField = false;
                                rowLookUpFieldId = GetMatrixFieldForUniqueDataFiltering(matrixRow.FieldId, out isLookupField);
                                rowFieldIsLookUp = isLookupField;
                            }
                        }
                        else if (matrixRow.AppObjectUniqueID != dataFieldAppObject.UniqueId)
                        {                            
                            rowLookUpFieldId = dataField.RowLookupId;
                            if (string.IsNullOrEmpty(rowLookUpFieldId)) //For Backward Compatability. For those apps whose config doesn't have value of MatrixDataField.RowLookupId
                                rowLookUpFieldId = getLookFieldId(dataFieldAppObject, applicationDefinitionManager.GetAppObject(matrixRow.AppObjectUniqueID).Id);

                            rowFieldIsLookUp = true;
                        }
                        else if (matrixRow.AppObjectUniqueID == dataFieldAppObject.UniqueId)
                        {
                            rowLookUpFieldId = matrixRow.FieldId;
                            if (matrixRow.RenderingType == MatrixRenderingType.Static && matrixRow.ValueType == MatrixValueType.FieldLabel)
                            {
                                rowLookUpFieldId = matrixRow.FieldId.EndsWith(Constants.APPENDLOOKUPID) ? applicationDefinitionManager.GetLookupIdFromLookupName(matrixRow.FieldId) : Constants.ID_ATTRIBUTE;
                                rowFieldIsLookUp = true;
                            }
                            else if (matrixRow.RenderingType == MatrixRenderingType.Dynamic && matrixRow.ValueType == MatrixValueType.FieldValue)
                            {
                                rowFieldIsLookUp = matrixRow.FieldId.EndsWith(Constants.APPENDLOOKUPID);
                                rowLookUpFieldId = rowFieldIsLookUp ? applicationDefinitionManager.GetLookupIdFromLookupName(matrixRow.FieldId) : matrixRow.FieldId;
                            }
                        }

                        Excel.Range dataRange = null;
                        DataFieldRendering rendering = QueryDataFieldRendering(matrixRow, matrixColumn);
                        string dataFieldWhereClause = GetDataFieldComponentFilters(dataField);
                        if (!string.IsNullOrEmpty(dataFieldWhereClause))
                            dataFieldWhereClause = ReplaceExpressionBuilderOperatorsWithDataTableOperators(dataFieldWhereClause);

                        switch(rendering)
                        {
                            case DataFieldRendering.RenderDataHorizontally:
                                dataRange = RenderDataHorizontally(dataField, rowFieldIsLookUp, columnFieldIsLookUp, evalAndOr, rowLookUpFieldId, columnLookUpFieldId, dataSet, dataFieldWhereClause, blnExpandFormula);
                                break;

                            case DataFieldRendering.RenderDataHorizontallyAndVertically:
                                dataRange = RenderDataHorizontallyAndVertically(dataField, rowFieldIsLookUp, columnFieldIsLookUp, evalAndOr, rowLookUpFieldId, columnLookUpFieldId, dataSet, dataFieldWhereClause, blnExpandFormula);
                                break;

                            case DataFieldRendering.RenderDataOnCell:
                                dataRange = RenderDataOnCell(matrixRow, matrixColumn, dataField, rowFieldIsLookUp, columnFieldIsLookUp, evalAndOr, rowLookUpFieldId, columnLookUpFieldId, dataSet, dataFieldWhereClause);
                                break;

                            case DataFieldRendering.RenderDataVertically:
                                dataRange = RenderDataVertically(dataField, rowFieldIsLookUp, columnFieldIsLookUp, evalAndOr, rowLookUpFieldId, columnLookUpFieldId, dataSet, dataFieldWhereClause , blnExpandFormula);
                                break;

                        }         

                        if (dataRange != null)
                        {
                            ExcelHelper.AssignNameToRange(dataRange, dataField.TargetNamedRange);

                            // Apply Data validations
                            ApttusField apttusDataField = applicationDefinitionManager.GetField(dataField.AppObjectUniqueID, dataField.FieldId);
                            ExcelHelper.AddCellRangeValidation(dataRange, apttusDataField,
                                new CellValidationInput
                                {
                                    ObjectId = dataFieldAppObject.Id,
                                    ObjectType = ObjectType.Repeating,
                                    ReferenceNamedRange = null,
                                    ControllingPicklistFieldId = string.Empty,
                                    RecordTypeNamedRange = null
                                });
                        }
                    }
                }
                else if (dataField.RenderingType == MatrixRenderingType.Static)
                {
                    // TODO:: for static read data from Excel cell and add it matrixcolumndata
                }                
            }

            matrixData.AcceptChanges();
            
            //Create a MatrixDataSet and give it to DataManager. Every MatrixDataSet is associated with MatrixMap.
            ApttusMatrixDataSet matrixDataSet = new ApttusMatrixDataSet(Model.Id);
            matrixDataSet.MatrixDataTable = matrixData;
            dataManager.AddData(matrixDataSet);

            // Add the named range data tracker entry for repeating cells
            //dataManager.AddDataTracker(new ApttusDataTracker
            //{
            //    Location = Model.DataField.TargetNamedRange,
            //    DataSetId = matrixDataSet.Id,
            //    Type = ObjectType.CrossTab
            //});

            return true;
        }

        /// <summary>
        /// DataTable's select clause doesn't understand operators defined in expression builder, hence replace those operators with datatable understandable operators.
        /// </summary>
        /// <param name="dataFieldWhereClause"></param>
        private static string ReplaceExpressionBuilderOperatorsWithDataTableOperators(string dataFieldWhereClause)
        {
            string whereClause = dataFieldWhereClause;
            int notEqualIndex = dataFieldWhereClause.IndexOf("!=");
            if (notEqualIndex != -1)
                whereClause = new StringBuilder(whereClause).Replace("!=", "<>").ToString();

            return whereClause;
        }
        
        private Excel.Range RenderDataOnCell(MatrixField rowField, MatrixField colField, MatrixDataField dataField, bool rowFieldIsLookUp, bool columnFieldIsLookUp, string evalAndOr, string rowLookUpFieldId, string columnLookUpFieldId, ApttusDataSet dataSet, string dataFieldWhereClause)
        {
            Excel.Range dataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);

            DataRow row = matrixRowData.Select(Constants.MATRIX_ROWID + "= '" + rowField.Id + "'").FirstOrDefault();
            DataRow col = matrixColumnData.Select(Constants.MATRIX_COLUMNID + "= '" + colField.Id + "'").FirstOrDefault();

            string rowValue = rowFieldIsLookUp ? Convert.ToString(row[Constants.ID_ATTRIBUTE]) : Convert.ToString(row[Constants.VALUE_COLUMN]);
            string columnValue = columnFieldIsLookUp ? Convert.ToString(col[Constants.ID_ATTRIBUTE]) : Convert.ToString(col[Constants.VALUE_COLUMN]);
            string whereClause = rowLookUpFieldId + "='" + rowValue + "'" + evalAndOr +
                                    columnLookUpFieldId + "='" + columnValue + "'";

            string combinedWhereClause = string.IsNullOrEmpty(dataFieldWhereClause) ? whereClause : 
                Constants.OPEN_BRACKET + whereClause + Constants.CLOSE_BRACKET + Constants.SPACE + Constants.AND + Constants.SPACE + dataFieldWhereClause;

            object value;
            DataRow data = dataSet.DataTable.Select(combinedWhereClause).FirstOrDefault();

            if (data != null)
            {    
                value = dataRange.Value = data[dataField.FieldId];
                // Create blank data field entry for update record since data value is retrieved
                CreateDataFieldEntry(data[Constants.ID_ATTRIBUTE], value, dataField.Id, 1, 1, rowValue, columnValue);
            }
            else
            {
                // Create blank data field entry for creating new record or Matrix Entry
                string uniqueId = string.Empty;
                if (rowField.ValueType == MatrixValueType.FieldValue && colField.ValueType == MatrixValueType.FieldValue)
                    uniqueId = Guid.NewGuid().ToString();
                else if (rowField.ValueType == MatrixValueType.FieldValue && colField.ValueType == MatrixValueType.FieldLabel)
                    uniqueId = row[newRecordGuid] as string;
                else if (colField.ValueType == MatrixValueType.FieldValue && rowField.ValueType == MatrixValueType.FieldLabel)
                    uniqueId = col[newRecordGuid] as string;

                CreateDataFieldEntry(uniqueId, string.Empty, dataField.Id, 1, 1, rowValue, columnValue);
            }

            return dataRange;
        }

        private Excel.Range RenderDataHorizontally(MatrixDataField dataField, bool rowFieldIsLookUp, bool columnFieldIsLookUp, string evalAndOr, string rowLookUpFieldId,
            string columnLookUpFieldId, ApttusDataSet dataSet, string dataFieldWhereClause, bool blnExpandFormula)
        {
            // Get rows for current Data Field
            DataRow[] currentColData = matrixColumnData.Select(Constants.MATRIX_COLUMNID + " = '" + dataField.MatrixColumnId.ToString() + "'");
            Excel.Range targetDataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);

            object[,] objData = new object[1, currentColData.Length];

            foreach (DataRow row in matrixRowData.Rows)
            {
                if (Convert.ToString(row[Constants.MATRIX_ROWID]) == Convert.ToString(dataField.MatrixRowId))
                {
                    int colIndex = 0, rowIndex = 0;
                    foreach (DataRow col in currentColData)
                    {                        
                        // get Row & Column value
                        string rowValue = rowFieldIsLookUp ? Convert.ToString(row[Constants.ID_ATTRIBUTE]) : Convert.ToString(row[Constants.VALUE_COLUMN]);
                        string columnValue = columnFieldIsLookUp ? Convert.ToString(col[Constants.ID_ATTRIBUTE]) : Convert.ToString(col[Constants.VALUE_COLUMN]);
                        string whereClause = rowLookUpFieldId + "='" + rowValue + "'" + evalAndOr +
                                                columnLookUpFieldId + "='" + columnValue + "'" + getMatrixColumnFilter(col);

                        string combinedWhereClause = string.IsNullOrEmpty(dataFieldWhereClause) ? whereClause :
                            Constants.OPEN_BRACKET + whereClause + Constants.CLOSE_BRACKET + Constants.SPACE + Constants.AND + Constants.SPACE + dataFieldWhereClause;
                        
                        object value;
                        DataRow data = dataSet.DataTable.Select(combinedWhereClause).FirstOrDefault();

                        if (data != null)
                        {
                            value = objData[0, colIndex] = data[dataField.FieldId];
                            CreateDataFieldEntry(data[Constants.ID_ATTRIBUTE], value, dataField.Id, rowIndex, colIndex, rowValue, columnValue);
                        }
                        ++colIndex;
                        
                    }
                }
            }

            // Plot data on Excel and Expand Named range
            Excel.Worksheet sheet = targetDataRange.Worksheet;
           
            if (currentColData.Length > 1)
            {
                // Expand range as data field size
                Excel.Range cellBesides = ExcelHelper.NextHorizontalCell(targetDataRange, 1);
                Excel.Range colsToInsert = cellBesides.Resize[Type.Missing, currentColData.Length - 1];
                colsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
            }

            Excel.Range dataRange = sheet.Range[targetDataRange, ExcelHelper.NextHorizontalCell(targetDataRange, currentColData.Length - 1)];
            dataRange.Value = objData;
            if (blnExpandFormula)
                ExpandMatrixFormulas(dataRange, Convert.ToInt32(currentColData.Length), DataFieldRendering.RenderDataHorizontally);
            return dataRange;
        }

        private Excel.Range RenderDataVertically(MatrixDataField dataField, bool rowFieldIsLookUp, bool columnFieldIsLookUp, string evalAndOr, string rowLookUpFieldId,
            string columnLookUpFieldId, ApttusDataSet dataSet, string dataFieldWhereClause, bool blnExpandFormula)
        {
            // Get rows for current Data Field
            DataRow[] currentRowData = matrixRowData.Select(Constants.MATRIX_ROWID + " = '" + dataField.MatrixRowId.ToString() + "'");
            Excel.Range targetDataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);

            object[,] objData = new object[currentRowData.Length, 1];
            int rowIndex = 0, colIndex = 0;

            foreach (DataRow row in currentRowData)
            {
                foreach (DataRow col in matrixColumnData.Rows)
                {
                    if (Convert.ToString(col[Constants.MATRIX_COLUMNID]) == Convert.ToString(dataField.MatrixColumnId))
                    {
                        // get Row & Column value
                        string rowValue = rowFieldIsLookUp ? Convert.ToString(row[Constants.ID_ATTRIBUTE]) : Convert.ToString(row[Constants.VALUE_COLUMN]);
                        string columnValue = columnFieldIsLookUp ? Convert.ToString(col[Constants.ID_ATTRIBUTE]) : Convert.ToString(col[Constants.VALUE_COLUMN]);
                        string whereClause = rowLookUpFieldId + "='" + rowValue + "'" + evalAndOr +
                                                columnLookUpFieldId + "='" + columnValue + "'";

                        string combinedWhereClause = string.IsNullOrEmpty(dataFieldWhereClause) ? whereClause :
                            Constants.OPEN_BRACKET + whereClause + Constants.CLOSE_BRACKET + Constants.SPACE + Constants.AND + Constants.SPACE + dataFieldWhereClause;

                        object value;
                        DataRow data = dataSet.DataTable.Select(combinedWhereClause).FirstOrDefault();

                        if (data != null)
                        {
                            value = objData[rowIndex, 0] = data[dataField.FieldId];
                            CreateDataFieldEntry(data[Constants.ID_ATTRIBUTE], value, dataField.Id, rowIndex, colIndex, rowValue, columnValue);
                        }
                        ++rowIndex;                        
                    }
                }
            }

            // Plot data on Excel and Expand Named range
            Excel.Worksheet sheet = targetDataRange.Worksheet;

            if (currentRowData.Length > 1)
            {
                // Expand range as data field size
                Excel.Range cellBelow = ExcelHelper.NextVerticalCell(targetDataRange, 1);
                Excel.Range rowsToInsert = cellBelow.Resize[currentRowData.Length - 1, Type.Missing];
                rowsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            }

            Excel.Range dataRange = sheet.Range[targetDataRange, ExcelHelper.NextVerticalCell(targetDataRange, currentRowData.Length - 1)];
            dataRange.Value = objData;

            if (blnExpandFormula)
                ExpandMatrixFormulas(dataRange, Convert.ToInt32(currentRowData.Length), DataFieldRendering.RenderDataVertically);

            return dataRange;
        }
        
        private Excel.Range RenderDataHorizontallyAndVertically(MatrixDataField dataField, bool rowFieldIsLookUp, bool columnFieldIsLookUp, string evalAndOr,
            string rowLookUpFieldId, string columnLookUpFieldId, ApttusDataSet dataSet, string dataFieldWhereClause, bool blnExpandFormula)
        {
            MatrixField colField = Model.MatrixColumn.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();
            MatrixField rowField = Model.MatrixRow.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();

            DataRow[] currentRowData = matrixRowData.Select(Constants.MATRIX_ROWID + " = '" + dataField.MatrixRowId.ToString() + "'");
            DataRow[] currentColData = matrixColumnData.Select(Constants.MATRIX_COLUMNID + " = '" + dataField.MatrixColumnId.ToString() + "'");

            int rowCount = currentRowData.Count();
            int colCount = currentColData.Count();

            object[,] objCellData = new object[rowCount, colCount];

            // Get data cell named range
            Excel.Range targetColumnRange = ExcelHelper.GetRange(dataField.TargetNamedRange);
            int rowIndex = 0, colIndex = 0;

            // Traverse through Row and Columns data to create data fields value set
            foreach (DataRow row in currentRowData)
            {
                if (Convert.ToString(row[Constants.MATRIX_ROWID]) == Convert.ToString(dataField.MatrixRowId))
                {
                    foreach (DataRow col in currentColData)
                    {
                        if (Convert.ToString(col[Constants.MATRIX_COLUMNID]) == Convert.ToString(dataField.MatrixColumnId))
                        {
                            // get Row & Column value
                            string rowValue = rowFieldIsLookUp ? Convert.ToString(row[Constants.ID_ATTRIBUTE]) : Convert.ToString(row[Constants.VALUE_COLUMN]);
                            string columnValue = columnFieldIsLookUp ? Convert.ToString(col[Constants.ID_ATTRIBUTE]) : Convert.ToString(col[Constants.VALUE_COLUMN]);

                            string groupedColumnFilter = getMatrixColumnFilter(col);

                            string whereClause = rowLookUpFieldId + "='" + rowValue + "'" + evalAndOr +
                                                    columnLookUpFieldId + "='" + columnValue + "'" + groupedColumnFilter;

                            string combinedWhereClause = string.IsNullOrEmpty(dataFieldWhereClause) ? whereClause :
                                Constants.OPEN_BRACKET + whereClause + Constants.CLOSE_BRACKET + Constants.SPACE + Constants.AND + Constants.SPACE + dataFieldWhereClause;

                            DataRow data = dataSet.DataTable.Select(combinedWhereClause).FirstOrDefault();

                            if (data != null)
                            {
                                object value;
                                value = objCellData[rowIndex,colIndex] = data[dataField.FieldId];                                
                                CreateDataFieldEntry(data[Constants.ID_ATTRIBUTE], value, dataField.Id, rowIndex, colIndex, rowValue, columnValue);
                            }
                            else
                            {
                                // Create blank data field entry for creating new record or Matrix Entry
                                // In Dynamic by Dynamic matrix, if Row and Column both are lookup then only each data cell could represent a Individual Cell
                                // so create empty data field entry in case of Row and Column both are lookup
                                ////if (rowFieldIsLookUp && columnFieldIsLookUp)
                                ////{

                                //if the column field is not grouped, then only create a blank entry for the datafied.
                                bool bCreateEmptyDataField = colField.MatrixGroupedParentId == Guid.Empty;

                                if (bCreateEmptyDataField)
                                {
                                    string uniqueId = Guid.NewGuid().ToString();
                                    CreateDataFieldEntry(uniqueId, string.Empty, dataField.Id, rowIndex, colIndex, rowValue, columnValue);
                                }
                                ////}
                            }
                        }
                        ++colIndex;
                    }
                    ++rowIndex;
                    colIndex = 0;
                }
            }

            // Plot data on Excel and Expand Named range
            Excel.Worksheet sheet = targetColumnRange.Worksheet;

            if (rowCount > 1)
            {
                // Expand Row range
                Excel.Range cellBelow = ExcelHelper.NextVerticalCell(targetColumnRange, 1);
                Excel.Range rowsToInsert = cellBelow.Resize[rowCount - 1, Type.Missing];
                rowsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            }

            if (colCount > 1)
            {
                // Expand Column range
                Excel.Range cellBesides = ExcelHelper.NextHorizontalCell(targetColumnRange, 1);
                Excel.Range lastColumnCell = ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(targetColumnRange, rowCount - 1), colCount - 1);
                Excel.Range columnsToInsert = sheet.Range[cellBesides, lastColumnCell];
                columnsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
            }

            //// Expanded range on which data needs to be plotted
            Excel.Range rangeToInsert = targetColumnRange.Resize[rowCount, colCount];
            rangeToInsert.Value = objCellData;

            if (blnExpandFormula)
                ExpandMatrixFormulas(targetColumnRange, rowCount, DataFieldRendering.RenderDataHorizontallyAndVertically, colCount);                

            return rangeToInsert;
        }
        
        private string GetDataFieldComponentFilters(MatrixDataField matrixDataField)
        {
            string whereClause = string.Empty;
            List<KeyValuePair<string, Guid>> UsedDataSets;

            MatrixComponent matrixComp = this.Model.MatrixComponents.Where(c => c.Id == matrixDataField.MatrixComponentId).FirstOrDefault();
            
            if (matrixComp != null && (matrixComp.WhereFilterGroups != null && matrixComp.WhereFilterGroups.Count > 0) &&
                        (matrixComp.WhereFilterGroups[0].Filters != null && matrixComp.WhereFilterGroups[0].Filters.Count > 0))
            {
                ApttusObject matrixCompObject = applicationDefinitionManager.GetAppObject(matrixComp.AppObjectUniqueID);
                bool chunkValues;
                List<string> fullWhereClauseChunks;
                bool validExpression = ExpressionBuilderHelper.GetExpression(Guid.Empty.ToString(), matrixComp.WhereFilterGroups, matrixCompObject, this.InputDataName, out UsedDataSets, out whereClause, out chunkValues, out fullWhereClauseChunks, false);
            }

            return whereClause;
        }

        private static string getLookFieldId(ApttusObject targetObject, string lookUpObjectId)
        {
            return targetObject.Fields.Where(f => f.Datatype == Datatype.Lookup && f.LookupObject.Id.Equals(lookUpObjectId)).FirstOrDefault().Id;
        }

        private string GetMatrixFieldForUniqueDataFiltering(string matrixFieldId, out bool isLookUpField)
        {
            string returnVal = string.Empty;
            if (matrixFieldId.Equals(Constants.NAME_ATTRIBUTE))  // If row / col field is Name field, use Id Column to get Unique values
            {
                returnVal = Constants.ID_ATTRIBUTE;
                isLookUpField = true;
            }
            else if (matrixFieldId.EndsWith(Constants.APPENDLOOKUPID))   // If row / col field is Lookup.Name field, use LookupId Column to get Unique values
            {
                returnVal = applicationDefinitionManager.GetLookupIdFromLookupName(matrixFieldId);
                isLookUpField = true;
            }
            else
            {
                returnVal = matrixFieldId;
                isLookUpField = false;
            }

            return returnVal;
        }

        /// <summary>
        /// On Rendering reset all range back to original state
        /// </summary>
        /// <param name="model"></param>
        private static void ResetMatrixRange(MatrixMap model)
        {

            //Reset Individual Fields
            foreach (MatrixField field in model.IndependentFields)
                ExcelHelper.GetRange(field.TargetNamedRange).Value = string.Empty;
            
            bool blnExpandFormula = false;

            //Reset Column 
            foreach (MatrixField colField in model.MatrixColumn.MatrixFields)
            {
                if (colField.RenderingType == MatrixRenderingType.Dynamic)
                {
                    Excel.Range colFieldRange = ExcelHelper.GetRange(colField.TargetNamedRange);
                    Excel.Worksheet sheet = colFieldRange.Worksheet;
                    int colCount = colFieldRange.Columns.Count;
                    if (colCount > 1)
                    {
                        colFieldRange.Value = string.Empty;
                        Excel.Range colRangeToDelete = sheet.Range[ExcelHelper.NextHorizontalCell(colFieldRange, 1), ExcelHelper.NextHorizontalCell(colFieldRange, colCount - 1)];
                        colRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                    }
                    
                    //Reset GroupedField per column
                    foreach(MatrixField groupedField in colField.MatrixGroupedFields)
                    {
                        Excel.Range groupedFieldRange = ExcelHelper.GetRange(groupedField.TargetNamedRange);
                        int groupedColCount = groupedFieldRange.Columns.Count;
                        if (groupedColCount > 1)
                        {
                            groupedFieldRange.Value = string.Empty;
                            Excel.Range colRangeToDelete = sheet.Range[ExcelHelper.NextHorizontalCell(groupedFieldRange, 1), ExcelHelper.NextHorizontalCell(groupedFieldRange, groupedColCount - 1)];
                            colRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                        }
                    }
                }
            }            

            //Reset Row
            foreach (MatrixField field in model.MatrixRow.MatrixFields)
            {
                if (field.RenderingType == MatrixRenderingType.Dynamic)
                {
                    Excel.Range rowFieldRange = ExcelHelper.GetRange(field.TargetNamedRange);
                    Excel.Worksheet sheet = rowFieldRange.Worksheet;
                    int rowCount = rowFieldRange.Rows.Count;
                    if (rowCount > 1)
                    {
                        rowFieldRange.Value = string.Empty;
                        Excel.Range rowRangeToDelete = sheet.Range[ExcelHelper.NextVerticalCell(rowFieldRange, 1), ExcelHelper.NextVerticalCell(rowFieldRange, rowCount - 1)];
                        rowRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    }
                }
            }

            //Reset Data
            foreach (MatrixDataField dataField in model.MatrixData.MatrixDataFields)
            {
                MatrixField rowField = model.MatrixRow.MatrixFields.Where(rField => rField.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();
                MatrixField colField = model.MatrixColumn.MatrixFields.Where(cField => cField.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();

                DataFieldRendering dataRendering = QueryDataFieldRendering(rowField, colField);

                Excel.Range dataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);
                Excel.Worksheet sheet = dataRange.Worksheet;
               
                switch (dataRendering)
                {
                    case DataFieldRendering.RenderDataHorizontally:
                        {
                            int nColumnCount = dataRange.Columns.Count;
                            if (nColumnCount > 1)
                            {
                                if (!blnExpandFormula)
                                    ResetMatrixFormulas(dataRange, nColumnCount, DataFieldRendering.RenderDataHorizontally);
                                Excel.Range dataRangeToDelete = sheet.Range[ExcelHelper.NextHorizontalCell(dataRange, 1), ExcelHelper.NextHorizontalCell(dataRange, nColumnCount - 1)];
                                dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            }
                        }
                        break;

                    case DataFieldRendering.RenderDataOnCell:
                        dataRange.Value = string.Empty;
                        break;

                    case DataFieldRendering.RenderDataHorizontallyAndVertically:
                        {
                            int nColumnCount = dataRange.Columns.Count;
                            int nRowCount = dataRange.Rows.Count;
                            if (nColumnCount > 1)
                            {
                                                            
                                // Delete top row left to bottom right section
                                Excel.Range topLeftRange = ExcelHelper.NextHorizontalCell(dataRange, 1);
                                Excel.Range bottomRightRange = ExcelHelper.NextVerticalCell(ExcelHelper.NextHorizontalCell(dataRange, nColumnCount - 1), nRowCount - 1);
                                Excel.Range dataRangeToDelete = sheet.Range[topLeftRange, bottomRightRange];
                                dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                            }
                            if (nRowCount > 1)
                            {
                                // Delete remaining rows in top row section
                                Excel.Range topLeftRange = ExcelHelper.NextVerticalCell(dataRange, 1);
                                Excel.Range bottomRightRange = ExcelHelper.NextVerticalCell(dataRange, nRowCount - 1);
                                Excel.Range dataRangeToDelete = sheet.Range[topLeftRange, bottomRightRange];
                                dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            }
                            if (!blnExpandFormula)
                                ResetMatrixFormulas(dataRange, nRowCount, DataFieldRendering.RenderDataHorizontallyAndVertically, nColumnCount);  
                        }
                        break;

                    case DataFieldRendering.RenderDataVertically:
                        {
                            int nRowCount = dataRange.Rows.Count;
                            if (nRowCount > 1)
                            {
                                if (!blnExpandFormula)
                                    ResetMatrixFormulas(dataRange, nRowCount, DataFieldRendering.RenderDataVertically);

                                Excel.Range dataRangeToDelete = sheet.Range[ExcelHelper.NextVerticalCell(dataRange, 1), ExcelHelper.NextVerticalCell(dataRange, nRowCount - 1)];
                                dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            }
                        }
                        break;
                }
                blnExpandFormula = true;  
                // Remove any on cell value
                dataRange.Value = string.Empty;
            }
        }

        /// <summary>
        /// Reset Formulas 
        /// </summary>
        /// <param name="oRange"></param>
        public static void ResetMatrixFormulas(Excel.Range dataRange, int cntData, DataFieldRendering rendering, int cntColData = 0)
        {
            Excel.Worksheet oSheet = dataRange.Worksheet;
            if (lstCellLocation != null)
            {
                foreach (CellLocation celllocation in lstCellLocation)
                {
                    switch (rendering)
                    {
                        case DataFieldRendering.RenderDataHorizontally:
                            {
                                if (celllocation.ColumnIndex == dataRange.Column)
                                {
                                    Excel.Range formulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                    Excel.Range destinationRange = oSheet.Range[ExcelHelper.NextHorizontalCell(formulaRange, 1), ExcelHelper.NextHorizontalCell(formulaRange, cntData - 1)];
                                    destinationRange.FormulaR1C1 = "";

                                    Excel.Range dataStartRange = oSheet.Range[formulaRange, ExcelHelper.NextHorizontalCell(formulaRange, cntData - 1)];
                                    Excel.Range dataRangeToDelete = oSheet.Range[ExcelHelper.NextHorizontalCell(dataStartRange, 1), ExcelHelper.NextHorizontalCell(dataStartRange, cntData - 1)];
                                    dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftToLeft);
                                }
                            }
                            break;
                        case DataFieldRendering.RenderDataVertically:
                            {
                                if (celllocation.RowIndex == dataRange.Row)
                                {
                                    Excel.Range formulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                    Excel.Range destinationRange = oSheet.Range[ExcelHelper.NextVerticalCell(formulaRange, 1), ExcelHelper.NextVerticalCell(formulaRange, cntData - 1)];
                                    destinationRange.FormulaR1C1 = "";

                                    Excel.Range dataStartRange = oSheet.Range[formulaRange, ExcelHelper.NextVerticalCell(formulaRange, cntData - 1)];
                                    Excel.Range dataRangeToDelete = oSheet.Range[ExcelHelper.NextVerticalCell(dataStartRange, 1), ExcelHelper.NextVerticalCell(dataStartRange, cntData - 1)];
                                    dataRangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                                }
                            }
                            break;
                        case DataFieldRendering.RenderDataHorizontallyAndVertically:
                            {
                                if (celllocation.ColumnIndex == dataRange.Column)
                                {
                                    if (cntColData > 1)
                                    {
                                        Excel.Range colFormulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                        Excel.Range colFormulaVerticalRangeNext = ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1);
                                        Excel.Range rowStartRange = ExcelHelper.NextHorizontalCell(colFormulaVerticalRangeNext, 1);
                                        Excel.Range colDestinationRange = oSheet.Range[rowStartRange, ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1), cntColData - 1)];

                                        colDestinationRange.FormulaR1C1 = "";
                                        colDestinationRange.ClearContents();                                       
                              
                                    }                                    

                                }
                                if (celllocation.RowIndex == dataRange.Row)
                                {
                                    ////Excel.Range rowFormulaRange = null;
                                    ////Excel.Range rowFormulaRangeNext = null;
                                    ////Excel.Range rowDestinationRange = null;
                                    ////rowFormulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                    if (cntData > 1)
                                    {
                                        Excel.Range rowFormulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                        Excel.Range rowFormulaRangeVerticalNext = ExcelHelper.NextVerticalCell(rowFormulaRange, 1);
                                        Excel.Range rowStartRange = ExcelHelper.NextVerticalCell(rowFormulaRange, cntData - 1);

                                        Excel.Range rowEndRange = ExcelHelper.NextHorizontalCell(rowStartRange, cntColData - 1);
                                        Excel.Range rowDestinationRange = oSheet.Range[rowFormulaRangeVerticalNext, rowEndRange];

                                        rowDestinationRange.FormulaR1C1 = "";
                                        rowDestinationRange.ClearContents();

                                        ////rowFormulaRangeNext = ExcelHelper.NextHorizontalCell(rowFormulaRange, cntColData - 1);
                                        ////Excel.Range oRange = ExcelHelper.NextVerticalCell(rowFormulaRangeNext, 1);
                                        ////rowDestinationRange = oSheet.Range[oRange, ExcelHelper.NextVerticalCell(rowFormulaRangeNext, cntData - 1)];
                                        
                                    }
                                    ////else
                                    ////{
                                    ////    rowFormulaRangeNext = ExcelHelper.NextHorizontalCell(rowFormulaRange, cntData - 1);
                                    ////    rowDestinationRange = oSheet.Range[rowFormulaRange, ExcelHelper.NextVerticalCell(rowFormulaRange, cntData - 1)];
                                    ////}

                                    ////rowDestinationRange.FormulaR1C1 = "";
                                    ////rowDestinationRange.ClearContents();

                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Set formulas for Save MatrixMap
        /// </summary>
        private void SetMatrixFormulas(MatrixMap model)
        {
            // Get Matrix Map WorkSheet
            MatrixDataField dataField = model.MatrixData.MatrixDataFields.FirstOrDefault();
            Excel.Range dataFieldRange = ExcelHelper.GetRange(dataField.TargetNamedRange);

            Excel.Worksheet worksheet = dataFieldRange.Worksheet;
            Excel.Range range = worksheet.UsedRange;
            lstCellLocation.Clear();
            foreach (Excel.Range oRange in range)
            {
                if (oRange.HasFormula)
                {
                    CellLocation cellLocation = new CellLocation();
                    cellLocation.RowIndex = oRange.Row;
                    cellLocation.ColumnIndex = oRange.Column;
                    cellLocation.Formula = oRange.Formula;                    
                    lstCellLocation.Add(cellLocation);
                }
            }
        }
      
        /// <summary>
        /// Expand Formulas 
        /// </summary>
        /// <param name="oRange"></param>
        public void ExpandMatrixFormulas(Excel.Range dataRange, int cntData, DataFieldRendering rendering, int cntColData = 0)
        {
            Excel.Worksheet oSheet = dataRange.Worksheet;
            Excel.Range destinationRange = null;
            Excel.Range formulaRange = null;

            if (lstCellLocation != null)
            {
                foreach (CellLocation celllocation in lstCellLocation)
                {
                    switch (rendering)
                    {
                        case DataFieldRendering.RenderDataHorizontally:
                            {
                                Excel.Range rowFormulaRangeNext = null;
                                if (celllocation.ColumnIndex == dataRange.Column)
                                {
                                    formulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];   
                                    if (cntData > 1)
                                    {
                                        Excel.Range cellBesides = ExcelHelper.NextHorizontalCell(formulaRange, 1);
                                        Excel.Range colsToInsert = cellBesides.Resize[Type.Missing, cntData - 1];
                                        colsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
                                        rowFormulaRangeNext = ExcelHelper.NextHorizontalCell(formulaRange, 1);
                                        destinationRange = oSheet.Range[rowFormulaRangeNext, ExcelHelper.NextHorizontalCell(formulaRange, cntData - 1)];
                                    }                                    
                                    else
                                        destinationRange = oSheet.Range[formulaRange, ExcelHelper.NextHorizontalCell(formulaRange, cntData - 1)];                                   
                                    
                                    formulaRange.Copy(destinationRange);
                                }
                            }
                            break;
                        case DataFieldRendering.RenderDataVertically:
                            {
                                Excel.Range colFormulaRangeNext = null;
                                if (celllocation.RowIndex == dataRange.Row)
                                {
                                    formulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                    if (cntData > 1)
                                    {
                                        Excel.Range cellBelow = ExcelHelper.NextVerticalCell(formulaRange, 1);
                                        Excel.Range rowsToInsert = cellBelow.Resize[cntData - 1, Type.Missing];
                                        rowsToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                                        colFormulaRangeNext = ExcelHelper.NextVerticalCell(formulaRange, 1);
                                        destinationRange = oSheet.Range[colFormulaRangeNext, ExcelHelper.NextVerticalCell(formulaRange, cntData - 1)];
                                    }
                                    else                                   
                                        destinationRange = oSheet.Range[formulaRange, ExcelHelper.NextVerticalCell(formulaRange, cntData - 1)];

                                    formulaRange.Copy(destinationRange);
                                }
                            }
                            break;
                        case DataFieldRendering.RenderDataHorizontallyAndVertically:
                            {
                                if (celllocation.ColumnIndex == dataRange.Column)
                                {
                                    Excel.Range colDestinationRange = null;
                                    Excel.Range colFormulaRangeNext = null;
                                    Excel.Range colFormulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];
                                    // C10

                                    if (cntData > 1)
                                    {
                                        colFormulaRangeNext = ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1);
                                        Excel.Range oRange = ExcelHelper.NextHorizontalCell(colFormulaRangeNext, 1);
                                        if (cntColData > 1)
                                            colDestinationRange = oSheet.Range[oRange, ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1), cntColData - 1)];
                                        else
                                            colDestinationRange = oSheet.Range[colFormulaRangeNext, ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1), cntColData - 1)];
                                    }
                                    else
                                    {
                                        colFormulaRangeNext = ExcelHelper.NextHorizontalCell(colFormulaRange, cntData - 1);
                                        Excel.Range oRange = ExcelHelper.NextHorizontalCell(colFormulaRange, 1);
                                        // C10:C11  
                                        if (cntColData > 1)
                                            colDestinationRange = oSheet.Range[oRange, ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(colFormulaRange, cntData - 1), cntColData - 1)];
                                        else
                                            colDestinationRange = oSheet.Range[colFormulaRange, ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(colFormulaRangeNext, cntData - 1), cntColData - 1)];

                                    }
                                    colFormulaRangeNext.Copy(colDestinationRange);
                                }
                                if (celllocation.RowIndex == dataRange.Row)
                                {
                                    Excel.Range rowFormulaRangeNext = null;
                                    Excel.Range rowDestinationRange = null;
                                    Excel.Range rowFormulaRange = oSheet.Cells[celllocation.RowIndex, celllocation.ColumnIndex];

                                    if (cntData > 1)
                                    {
                                        rowFormulaRangeNext = ExcelHelper.NextHorizontalCell(rowFormulaRange, cntColData - 1);
                                        Excel.Range oRange = ExcelHelper.NextVerticalCell(rowFormulaRangeNext, 1);
                                        rowDestinationRange = oSheet.Range[oRange, ExcelHelper.NextVerticalCell(rowFormulaRangeNext, cntData - 1)];
                                    }
                                    else
                                    {
                                        rowFormulaRangeNext = ExcelHelper.NextHorizontalCell(rowFormulaRange, cntData - 1);
                                        rowDestinationRange = oSheet.Range[rowFormulaRange, ExcelHelper.NextVerticalCell(rowFormulaRange, cntData - 1)];
                                    }
                                    rowFormulaRangeNext.Copy(rowDestinationRange);
                                }

                            }
                            break;
                    }
                }
            }
        }

        private string getMatrixColumnFilter(DataRow col)
        {
            string groupedColumnFilter = Convert.ToString(col[Constants.GROUPEDCOLUMNFILTER]);

            if (!string.IsNullOrEmpty(groupedColumnFilter))
                groupedColumnFilter = Constants.SPACE + Constants.AND + Constants.SPACE + groupedColumnFilter;

            return groupedColumnFilter;
        }

        private void ProcessMatrixColumns()
        {
            foreach (MatrixField colField in Model.MatrixColumn.MatrixFields)
            {
                if (colField.MatrixGroupedParentId == Guid.Empty)
                    ProcessMatrixColumns(colField, Model, inputData, string.Empty, null);
                else                
                    ProcessMatrixGroupedColumns(colField);                
            }
            sortedMatrixGroupedFields = null;
        }

        private MatrixField GetNextGroupedField(MatrixField currentField)
        {
            MatrixField nextField = null;
            int index = sortedMatrixGroupedFields.IndexOf(currentField);
            if (index == -1)
                return null;
            else if (sortedMatrixGroupedFields.Count == index + 1)
                return null;
            else
                nextField = sortedMatrixGroupedFields[index + 1];
            return nextField;
        }

        private string createFilterString(string fieldId, string value)
        {
            StringBuilder filter = new StringBuilder(100);
            filter.Append(fieldId).Append(Constants.SPACE).Append(Constants.EQUALS).Append(Constants.SPACE).
                       Append(Constants.QUOTE).Append(value).Append(Constants.QUOTE);
            return filter.ToString();
        }

        private void CreateAndApplyFilterPerGroupedField(MatrixField currentGroupedField, MatrixField parentGroupedField, string parentGroupFilter)
        {
            ApttusObject appObject = applicationDefinitionManager.GetAppObject(currentGroupedField.AppObjectUniqueID);
            ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(inputData, appObject);

            DataRow[] rows = dataSet.DataTable.Select(parentGroupFilter);
            if (rows.Count() == 0)
                return;

            DataTable table = dataSet.DataTable.Select(parentGroupFilter).AsEnumerable()
                           .GroupBy(r => new { Col1 = r[currentGroupedField.FieldId] })
                           .Select(g => g.First())
                           .CopyToDataTable();

            foreach (DataRow row in table.Rows)
            {
                bool isLookup;
                string fieldId = GetMatrixFieldForUniqueDataFiltering(currentGroupedField.FieldId, out isLookup);
                string value = row[fieldId] as string;

                string currentFilter = createFilterString(fieldId, value);

                MatrixField nextGroupedField = GetNextGroupedField(currentGroupedField);

                if (string.IsNullOrEmpty(parentGroupFilter))
                    parentGroupFilter = currentFilter;
                else
                    currentFilter = parentGroupFilter + Constants.SPACE + Constants.AND + Constants.SPACE + currentFilter;

                if (nextGroupedField != null)
                {
                    if (nextGroupedField.AppObjectUniqueID != currentGroupedField.AppObjectUniqueID)
                    {
                        ApttusObject nextGroupedFieldObj = applicationDefinitionManager.GetAppObject(nextGroupedField.AppObjectUniqueID);
                        ApttusField lookupField = nextGroupedFieldObj.Fields.Find(fld => fld.LookupObject != null && fld.LookupObject.Id == appObject.Id);
                        currentFilter = createFilterString(lookupField.Id, row[Constants.ID_ATTRIBUTE] as string);
                    }

                    AddFilterInDictionary(currentGroupedField, parentGroupFilter);
                    CreateAndApplyFilterPerGroupedField(nextGroupedField, currentGroupedField, currentFilter);

                    if (groupedRecordsPerParentFilter.ContainsKey(currentFilter))
                    {
                        List<GroupedFieldRecord> lstGroupedRecords = groupedRecordsPerParentFilter[currentFilter];
                        int totalRec = 0;
                        lstGroupedRecords.ForEach(rec => totalRec += rec.Records);

                        ExpandMatrixGroupedColumns(currentGroupedField, totalRec, row[currentGroupedField.FieldId] as string, currentFilter);
                        if (parentGroupFilter != currentFilter)
                        {
                            GroupedFieldRecord record = groupedRecordPerFilter[currentFilter];
                            AddGroupedFieldRecordPerParentFilter(parentGroupFilter, record);
                        }
                    }
                    if (currentGroupedField.MatrixGroupedParentId == Guid.Empty)
                        parentGroupFilter = string.Empty;
                }
                else
                {
                    MatrixField colfield = Model.MatrixColumn.MatrixFields.Find(fld => fld.MatrixGroupedParentId == currentGroupedField.Id);
                    if (colfield.AppObjectUniqueID != currentGroupedField.AppObjectUniqueID)
                    {
                        ApttusObject columnObj = applicationDefinitionManager.GetAppObject(colfield.AppObjectUniqueID);
                        ApttusField lookupField = columnObj.Fields.Find(fld => fld.LookupObject != null && fld.LookupObject.Id == appObject.Id);
                        currentFilter = createFilterString(lookupField.Id, row[Constants.ID_ATTRIBUTE] as string);
                    }
                    AddFilterInDictionary(currentGroupedField, currentFilter);
                    int nCols = ProcessMatrixColumns(colfield, Model, inputData, currentFilter, currentGroupedField);
                    ExpandMatrixGroupedColumns(currentGroupedField, nCols, row[currentGroupedField.FieldId] as string, currentFilter);
                    GroupedFieldRecord rec = groupedRecordPerFilter[currentFilter];
                    AddGroupedFieldRecordPerParentFilter(parentGroupFilter, rec);

                    if (currentGroupedField.MatrixGroupedParentId == Guid.Empty)
                        parentGroupFilter = string.Empty;
                }
            }
        }

        private void AddGroupedFieldRecordPerParentFilter(string parentFilter, GroupedFieldRecord record)
        {
            List<GroupedFieldRecord> lstGroupedRecords = null;
            if (groupedRecordsPerParentFilter.ContainsKey(parentFilter))
                lstGroupedRecords = groupedRecordsPerParentFilter[parentFilter];
            else
                lstGroupedRecords = new List<GroupedFieldRecord>();

            lstGroupedRecords.Add(record);
            groupedRecordsPerParentFilter[parentFilter] = lstGroupedRecords;
        }

        private void AddFilterInDictionary(MatrixField groupedfield, string filter)
        {
            List<string> filters = null;
            if (filterPerGroupedfield.ContainsKey(groupedfield))
                filters = filterPerGroupedfield[groupedfield];
            else
                filters = new List<string>();

            filters.Add(filter);
            filterPerGroupedfield[groupedfield] = filters;
        }

        private void ResetGroupedColumnRendering(MatrixField colField)
        {
            bIsColumnRenderedOnce = false;
            foreach (MatrixField groupedField in colField.MatrixGroupedFields)
                renderingPerGroupedField[groupedField] = false;
        }

        internal void ProcessMatrixGroupedColumns(MatrixField colField)
        {
            ResetGroupedColumnRendering(colField);
            sortedMatrixGroupedFields = colField.MatrixGroupedFields.OrderBy(s => ExcelHelper.GetRange(s.TargetNamedRange).Row).ToList();
            if (sortedMatrixGroupedFields.Count > 0)
                CreateAndApplyFilterPerGroupedField(sortedMatrixGroupedFields.First(), null, string.Empty);
        }

        private void ExpandMatrixGroupedColumns(MatrixField groupedColumn, int nCols, string value, string filter)
        {
            if (nCols != 0)
            {
                Excel.Range targetColumnRange = ExcelHelper.GetRange(groupedColumn.TargetNamedRange);

                Excel.Range cellBesides = ExcelHelper.NextHorizontalCell(targetColumnRange, targetColumnRange.Columns.Count);

                int availableCells = renderingPerGroupedField[groupedColumn] ? 0 : 1;

                bool bResizeRange = nCols > 1 || availableCells == 0;
                if (bResizeRange)
                {
                    Excel.Range rangeToInsert = cellBesides.Resize[Type.Missing, nCols - availableCells];
                    rangeToInsert.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);
                }

                // Create range on which data needs to be rendered
                Excel.Range renderingDataColumnRange = bResizeRange ?
                                                    targetColumnRange.Worksheet.Range[ExcelHelper.NextHorizontalCell(cellBesides, -nCols), ExcelHelper.NextHorizontalCell(cellBesides, -1)]
                                                    : targetColumnRange;

                renderingDataColumnRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                renderingDataColumnRange.Merge();
                renderingDataColumnRange.Value = value;

                // Expand named range to expanded section
                Excel.Range fullExpandedRange = targetColumnRange.Worksheet.Range[targetColumnRange.Cells[1, 1], ExcelHelper.NextHorizontalCell(cellBesides, -1)];
                ExcelHelper.AssignNameToRange(fullExpandedRange, groupedColumn.TargetNamedRange);

                renderingPerGroupedField[groupedColumn] = true;
            }

            GroupedFieldRecord record = new GroupedFieldRecord();
            record.Records = nCols;
            record.Filter = filter;
            groupedRecordPerFilter[filter] = record;
        }

    }

    // This class is use for preserve formula's location
    public class CellLocation
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Formula { get; set; }        
    }
}
