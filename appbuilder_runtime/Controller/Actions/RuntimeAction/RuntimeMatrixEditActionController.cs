/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

using Excel = Microsoft.Office.Interop.Excel;
using VSTO = Microsoft.Office.Tools.Excel;
using Office = Microsoft.Office.Core;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class RuntimeMatrixEditActionController
    {
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        WorkbookProtectionManager workbookProtectionManager = WorkbookProtectionManager.GetInstance;
        RichTextDataManager richTextManager = RichTextDataManager.Instance;
        AttachmentsDataManager attachmentManager = AttachmentsDataManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        Excel.Range Selection = null;
        Excel.Range ModelNamedRange = null;
        ObjectManager objectManager = ObjectManager.GetInstance;
        // Define Constructor , this constructor call from workflow
        public RuntimeMatrixEditActionController(Excel.Range Selection, Excel.Range NamedRange)
        {
            this.ModelNamedRange = NamedRange;
            this.Selection = Selection;
        }

        /// <summary>
        /// This function finds grouped values under which the new records were added.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dataSet"></param>
        /// <param name="groupedFieldIds"></param>
        /// <returns></returns>
        private Dictionary<string, string> FindGroupedValues(int pos, ApttusDataSet dataSet, ref List<string> groupedFieldIds)
        {
            Dictionary<string, string> groupedValueByfieldId = new Dictionary<string, string>();
            int nIndex = pos;
            while (--nIndex >= 0)
            {
                DataRow dr = dataSet.DataTable.Rows[nIndex];
                //1. If it is a grouped-row, it will be identified as when the row was grouped, a msg was set in DataRow.RowError property.
                if (!string.IsNullOrEmpty(dr.RowError))  //Check whether there is a msg set or not. Msg format is Constants.GROUPING_ROW_ERRORMSG_WITHFIELDID.
                {
                    string[] splitValues = dr.RowError.Split(new string[] { Constants.GROUPING_UNIQUE_SEPARATOR }, StringSplitOptions.None);  //Split the msg 
                    if (groupedFieldIds.Count == 1) //for first level grouping.
                    {
                        if (splitValues != null && splitValues.Count() == 2 && Constants.GROUPING_ROW_ERRORMSG_ONLY.Equals(splitValues[0]))
                        {
                            //splitValues[1] will be empty in case of first level grouping.
                            //If the grouping level is 1, we dont have the groupbyvalue in splitValues as this value is not set while we render the grouping in RetrieveAction.
                            //Hence retrieve the value from the datatable. This same is not applicable for n level rows. This method is just applicable for
                            //first level grouping.
                            string fieldId = groupedFieldIds[0];
                            groupedValueByfieldId[fieldId] = dr[fieldId] as string;
                            break;
                        }
                    }
                    else //for n-level grouping.
                    {
                        if (splitValues != null && splitValues.Count() >= 2 && Constants.GROUPING_ROW_ERRORMSG_ONLY.Equals(splitValues[0]))
                        {
                            int nFieldIndex = 1;//splitValues[0] will have the msg "Donottouch", which is of no use,hence index is placed at 1.
                            //In case of n-level grouping we will have all groupByValues of groupByFields
                            foreach (string fieldId in groupedFieldIds)
                                groupedValueByfieldId[fieldId] = splitValues[nFieldIndex++];
                            break;
                        }
                    }
                }
            }

            return groupedValueByfieldId.Count == groupedFieldIds.Count ? groupedValueByfieldId : null;
        }

       
        public bool AddRow(int RowsToAdd)
        {
            bool bAddCheckFailed = false;
            bool bLastRowInsert = false;
            try
            {
                // If sheet is protect by user then it will fire validation
                if (!ExcelHelper.IsSheetProtectedByUser(ModelNamedRange.Worksheet))
                {
                    ExcelHelper.ExcelApp.EnableEvents = false;  
                    Excel.Range IntersectRange = Globals.ThisAddIn.Application.Intersect(ModelNamedRange, Selection);
                    // Check #1 - Add row is allowed if the selection and the matrix model named range intersect.
                    if (IntersectRange != null)
                    {
                        Excel.Range firstselectedCell = null;
                        foreach (Excel.Range Cell in IntersectRange.Cells)
                        {
                            firstselectedCell = Cell;
                            break;
                        }

                        int firstGridRow = ((Excel.Range)ModelNamedRange.Cells[1, 1]).Row;                        
                      
                        SaveMap currentSaveMap = configurationManager.GetMatrixSaveMapbyTargetNamedRange(ModelNamedRange.Name.Name);
                        // Check #2 - Add row is allowed if there is a save map for the model name range.
                        if (currentSaveMap != null)
                        {                                                   
                            Excel.Worksheet InsertWorksheet = firstselectedCell.Worksheet;
                            Globals.ThisAddIn.Application.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                            //ApttusDataSet dataset = dataManager.GetDataByDataSetId(datasetId);                            
                            bool bHeader = false;

                            // If the insert was on the last row, update bottomRight range
                            if (firstselectedCell.Row == ModelNamedRange.Cells[ModelNamedRange.Cells.Count].Row)
                                bLastRowInsert = true;


                            // 3.1 Loop through all the rows to be added to excel, named range and datatable
                            Excel.Range rangeToInsert = null;
                            Excel.Range bottomRow = null;
                            Excel.Range bottomDataRow = null;


                            string rowNameRange = string.Empty;

                            MatrixMap matrixMap = configurationManager.GetMatrixMapIDTargetNamedRange(ModelNamedRange.Name.Name);
                                                        
                            // Start Validation 
                            //MatrixField rowField = matrixMap.MatrixRow.MatrixFields.Where(row => row.TargetNamedRange.Equals(ModelNamedRange.Name.Name)).FirstOrDefault();

                            MatrixField rowField = matrixMap.MatrixRow.MatrixFields.Find(mf => mf.TargetNamedRange.Equals(ModelNamedRange.Name.Name));

                            // Check row field rendering type is static then user can't add row
                            if (rowField != null)
                            {
                                rowNameRange = rowField.TargetNamedRange;
                                if (rowField.RenderingType != MatrixRenderingType.Static)
                                {
                                    ApttusMatrixDataSet matrixDataSet = dataManager.GetMatrixDataSetByMatrixMap(matrixMap.Id);
                                    if (matrixDataSet != null)
                                    {
                                        bool bOneRowsDatatable = matrixDataSet.MatrixDataTable != null && matrixDataSet.MatrixDataTable.Rows.Count == 1;

                                        // If the insert was on the header row, update bottom right range
                                        if (firstselectedCell.Row == firstGridRow)
                                            bHeader = true;

                                        /// 3.2 Insert into excel
                                        Excel.Range insertedRowCell = ExcelHelper.NextVerticalCell(firstselectedCell, 1);

                                        string rows = String.Format("{0}:{1}", insertedRowCell.Row, insertedRowCell.Row + RowsToAdd - 1);
                                        //startRows = String.Format("{0}:{1}", nonHeaderStartAddRowCell.Row - firstGridRow + 1, nonHeaderStartAddRowCell.Row - firstGridRow + RowsToAdd);
                                        rangeToInsert = InsertWorksheet.Range[rows];
                                        rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);                                       

                                        /******************* Incement RowIndex for Newly added rows ***********************/
                                        // Check if excel cell exists in datatable
                                        string existsRowIndex = string.Empty;

                                        int cnt = 0;
                                        foreach (Excel.Range Cell in ModelNamedRange.Cells)
                                        {
                                            if (Cell.Address.Equals(Globals.ThisAddIn.Application.Selection.Address))
                                            {
                                                cnt++;
                                                break;
                                            }
                                            cnt++;
                                        }

                                        existsRowIndex = cnt.ToString();
                                        //bool blnExistsRowIndex = matrixDataSet.MatrixDataTable.Select().ToList().Exists(row => row[Constants.MATRIX_DATAROWINDEX].ToString() == existsRowIndex);
                                        DataRow[] foundRows = matrixDataSet.MatrixDataTable.Select(Constants.MATRIX_DATAROWINDEX + ">= '" + existsRowIndex + "'");
                                        bool blnExistsRowIndex = foundRows != null && foundRows.Count() > 0;

                                        if (blnExistsRowIndex)
                                        {
                                            // Increment all those row index
                                            foreach (DataRow dr in foundRows)
                                                dr[Constants.MATRIX_DATAROWINDEX] = Convert.ToInt32(dr[Constants.MATRIX_DATAROWINDEX]) + 1;
                                        }
                                        /******************* End Comments ***********************/

                                        // TODO in future if add matrix row support custom add row
                                        for (int i = 0; i < RowsToAdd; i++)
                                        {
                                            Guid id = Guid.Empty;
                                            foreach (MatrixDataField dataField in matrixMap.MatrixData.MatrixDataFields)
                                            {
                                                if (dataField.MatrixRowId.Equals(rowField.Id))
                                                {
                                                    string dataNameRange = string.Empty;
                                                    MatrixField colField = matrixMap.MatrixColumn.MatrixFields.Find(mf => mf.Id.Equals(dataField.MatrixColumnId));
                                                    Excel.Range colNameRange = ExcelHelper.GetRange(colField.TargetNamedRange);

                                                    Excel.Range RowNameRange = ExcelHelper.GetRange(rowNameRange);
                                                    int rowscnt = RowNameRange.Cells.Count;
                                                    dataNameRange = dataField.TargetNamedRange;

                                                    int columns = colNameRange.Cells.Count;

                                                    int rowIndex = cnt;
                                                    // Column Count for increment columnindex in datatable
                                                    for (int columncnt = 0; columncnt < columns; columncnt++)
                                                    {
                                                        // Count is used for get value of datarow.
                                                        string filter = Constants.MATRIX_DATAFIELDID + " = '" + dataField.Id + "' and " +
                                                                            Constants.MATRIX_DATACOLINDEX + " = '" + columncnt + "' " + " and " +
                                                                            Constants.MATRIX_DATAROWINDEX + " = '" + existsRowIndex + "'";

                                                        DataRow[] dataRowIndex = matrixDataSet.MatrixDataTable.Select(filter);

                                                        string uniqueId = Guid.NewGuid().ToString();

                                                        if (dataRowIndex != null && dataRowIndex.Count() == 1)
                                                        {

                                                            DataRow matrixDataRow = matrixDataSet.MatrixDataTable.NewRow();
                                                            matrixDataRow[Constants.ID_ATTRIBUTE] = uniqueId;
                                                            matrixDataRow[Constants.VALUE_COLUMN] = string.Empty;
                                                            matrixDataRow[Constants.MATRIX_DATAFIELDID] = dataField.Id;
                                                            matrixDataRow[Constants.MATRIX_DATAROWINDEX] = rowIndex;
                                                            matrixDataRow[Constants.MATRIX_DATACOLINDEX] = columncnt;
                                                            matrixDataRow[Constants.MATRIX_DATAROWVALUE] = string.Empty;
                                                            matrixDataRow[Constants.MATRIX_DATACOLVALUE] = dataRowIndex[0].ItemArray[6];
                                                            matrixDataRow[Constants.MATRIX_ISROWVALUE] = true;
                                                            matrixDataSet.MatrixDataTable.Rows.Add(matrixDataRow);
                                                        }
                                                        else
                                                        {
                                                            int excelColIndex = columncnt + 1;
                                                            string value = string.Empty;
                                                            if (colField.ValueType == MatrixValueType.FieldValue)
                                                            {
                                                                value = Convert.ToString((ExcelHelper.GetRange(colField.TargetNamedRange).Cells[1, excelColIndex] as Excel.Range).Value);
                                                                id = Guid.NewGuid();
                                                            }
                                                            else if (colField.ValueType == MatrixValueType.FieldLabel)
                                                            {
                                                                if (id == Guid.Empty)
                                                                    id = Guid.NewGuid();
                                                            }

                                                            DataRow matrixDataRow = matrixDataSet.MatrixDataTable.NewRow();
                                                            matrixDataRow[Constants.ID_ATTRIBUTE] = id;
                                                            matrixDataRow[Constants.VALUE_COLUMN] = string.Empty;
                                                            matrixDataRow[Constants.MATRIX_DATAFIELDID] = dataField.Id;
                                                            matrixDataRow[Constants.MATRIX_DATAROWINDEX] = rowIndex;
                                                            matrixDataRow[Constants.MATRIX_DATACOLINDEX] = columncnt;
                                                            matrixDataRow[Constants.MATRIX_DATAROWVALUE] = string.Empty;
                                                            matrixDataRow[Constants.MATRIX_DATACOLVALUE] = value;
                                                            matrixDataRow[Constants.MATRIX_ISROWVALUE] = true;

                                                            matrixDataSet.MatrixDataTable.Rows.Add(matrixDataRow);
                                                        }
                                                    }

                                                    Excel.Range DataNameRange = ExcelHelper.GetRange(dataNameRange);

                                                    Excel.Range topRow = ModelNamedRange.Cells[1];
                                                    Excel.Range topDataRow = DataNameRange.Cells[1];

                                                    bottomRow = ModelNamedRange.Cells[ModelNamedRange.Cells.Count];
                                                    bottomDataRow = DataNameRange.Cells[DataNameRange.Cells.Count];

                                                    // If the insert was on the last row, update bottomRight range
                                                    if (bLastRowInsert | (bHeader & bOneRowsDatatable))
                                                    {
                                                        bottomRow = ExcelHelper.NextVerticalCell(bottomRow, RowsToAdd);
                                                        bottomDataRow = ExcelHelper.NextVerticalCell(bottomDataRow, RowsToAdd);
                                                    }

                                                    Excel.Range rowUpdatedRange = InsertWorksheet.get_Range(topRow, bottomRow);
                                                    Excel.Range dataUpdatedRange = InsertWorksheet.get_Range(topDataRow, bottomDataRow);


                                                    // Re-assign Row the TargetNamedRange
                                                    ExcelHelper.AssignNameToRange(rowUpdatedRange, RowNameRange.Name.Name);

                                                    // Re-assign the Data row TargetNamedRange
                                                    ExcelHelper.AssignNameToRange(dataUpdatedRange, DataNameRange.Name.Name);

                                                    // Expand formula base on add rows
                                                    MatrixActionView View = new MatrixActionView();
                                                    DataFieldRendering rendering = MatrixActionView.QueryDataFieldRendering(rowField, colField);

                                                    if (rendering == DataFieldRendering.RenderDataVertically)
                                                    {
                                                        if (!bLastRowInsert)
                                                        {

                                                            MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count, DataFieldRendering.RenderDataVertically);
                                                            View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count, DataFieldRendering.RenderDataVertically);
                                                        }
                                                        else
                                                        {
                                                            MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count + RowsToAdd, DataFieldRendering.RenderDataVertically);
                                                            View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count + RowsToAdd, DataFieldRendering.RenderDataVertically);
                                                        }
                                                    }
                                                    else if (rendering == DataFieldRendering.RenderDataHorizontallyAndVertically)
                                                    {
                                                        if (!bLastRowInsert)
                                                            View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count, DataFieldRendering.RenderDataHorizontallyAndVertically, columns);
                                                        else
                                                            View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count + RowsToAdd, DataFieldRendering.RenderDataHorizontallyAndVertically, columns);
                                                    }
                                                    // End Formula
                                                }
                                            }
                                        }
                                        //Accept Datatable changes
                                        matrixDataSet.MatrixDataTable.AcceptChanges();
                                    }
                                }
                                else
                                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXEDITACTCTL_AddRow_InfoMsg"), Constants.PRODUCT_NAME);
                            }
                            // 4. User clicked on formula row
                            else if (firstselectedCell.Row == firstGridRow + 1)
                            {
                                // Do nothing - No rows added.
                                return false;
                            }
                            
                        }
                        else
                            bAddCheckFailed = true;
                    }
                    else
                        bAddCheckFailed = true;

                    if (bAddCheckFailed)
                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXEDITACTCTL_AddColumnSAVE_InfoMsg"), Constants.PRODUCT_NAME);
                    
                }
                else
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMONRUNTIME_Protected_ErrorMsg"), Constants.PRODUCT_NAME);
                return !bAddCheckFailed;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        public bool AddColumn(int ColumnsToAdd)
        {
            bool bAddCheckFailed = false;
            bool bLastColumnInsert = false;
            try
            {
                // If sheet is protect by user then it will fire validation
                if (!ExcelHelper.IsSheetProtectedByUser(ModelNamedRange.Worksheet))
                {
                    ExcelHelper.ExcelApp.EnableEvents = false;
                    Excel.Range IntersectRange = Globals.ThisAddIn.Application.Intersect(ModelNamedRange, Selection);
                    // Check #1 - Add row is allowed if the selection and the matrix model named range intersect.
                    if (IntersectRange != null)
                    {
                        Excel.Range firstselectedCell = null;
                        foreach (Excel.Range Cell in IntersectRange.Cells)
                        {
                            firstselectedCell = Cell;
                            break;
                        }

                        int firstGridColumn = ((Excel.Range)ModelNamedRange.Cells[1, 1]).Column;

                        string result = configurationManager.GetMapbyNamedRange(ModelNamedRange.Name.Name);
                        if (result == Constants.MATRIXMAP_NAME)
                        {
                            SaveMap currentSaveMap = configurationManager.GetMatrixSaveMapbyTargetNamedRange(ModelNamedRange.Name.Name);
                            // Check #2 - Add row is allowed if there is a save map for the model name range.
                            if (currentSaveMap != null)
                            {
                                Excel.Worksheet InsertWorksheet = firstselectedCell.Worksheet;
                                Globals.ThisAddIn.Application.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                                bool bHeader = false;

                                // If the insert was on the last column, update bottomRight range
                                if (firstselectedCell.Column == ModelNamedRange.Cells[ModelNamedRange.Cells.Count].Column)
                                    bLastColumnInsert = true;


                                // 3.1 Loop through all the rows to be added to excel, named range and datatable                            
                                Excel.Range lastColumn = null;
                                Excel.Range lastDataColumn = null;


                                string outDataNameRange, columnNameRange = string.Empty;
                                MatrixMap matrixMap = configurationManager.GetMatrixMapIDTargetNamedRange(ModelNamedRange.Name.Name);
                                ApttusMatrixDataSet matrixDataSet = dataManager.GetMatrixDataSetByMatrixMap(matrixMap.Id);

                                // Start Validation 
                                ////MatrixField colField = matrixMap.MatrixColumn.MatrixFields.Where(col => col.TargetNamedRange.Equals(ModelNamedRange.Name.Name)).FirstOrDefault();

                                MatrixField colField = matrixMap.MatrixColumn.MatrixFields.Find(mf => mf.TargetNamedRange.Equals(ModelNamedRange.Name.Name));
                                // Check column field rendering type is static then user can't add column
                                if (colField != null)
                                {
                                    columnNameRange = colField.TargetNamedRange;
                                    if (colField.RenderingType != MatrixRenderingType.Static)
                                    {
                                        if (matrixDataSet != null)
                                        {
                                            bool bOneColumnsDatatable = matrixDataSet.MatrixDataTable != null && matrixDataSet.MatrixDataTable.Rows.Count == 1;

                                            // If the insert was on the header column, update bottom right range
                                            if (firstselectedCell.Column == firstGridColumn)
                                                bHeader = true;

                                            /// 3.2 Insert into excel
                                            Excel.Range insertedColumnCell = ExcelHelper.NextHorizontalCell(firstselectedCell, 1);

                                            insertedColumnCell.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight);


                                            /******************* Incement ColumnIndex for Newly added columns ***********************/
                                            // Check if excel cell exists in datatable
                                            string existsColumnIndex = string.Empty;

                                            int cnt = 0;
                                            foreach (Excel.Range Cell in ModelNamedRange.Cells)
                                            {
                                                if (Cell.Address.Equals(Globals.ThisAddIn.Application.Selection.Address))
                                                {
                                                    cnt++;
                                                    break;
                                                }
                                                cnt++;
                                            }

                                            existsColumnIndex = cnt.ToString();
                                            DataRow[] foundColumns = matrixDataSet.MatrixDataTable.Select(Constants.MATRIX_DATACOLINDEX + ">= '" + cnt + "'");
                                            bool blnExistsColumnIndex = foundColumns != null && foundColumns.Count() > 0;

                                            if (blnExistsColumnIndex)
                                            {
                                                // Get all rows grater than equal to excel selected cell. Increment all those column index.
                                                foreach (DataRow dr in foundColumns)
                                                    dr[Constants.MATRIX_DATACOLINDEX] = Convert.ToInt32(dr[Constants.MATRIX_DATACOLINDEX]) + 1;
                                            }
                                            /******************* End Comments ***********************/

                                            string dataFieldId = string.Empty;
                                            // TODO in future if add matrix row support custom add row
                                            for (int i = 0; i < ColumnsToAdd; i++)
                                            {
                                                Guid id = Guid.Empty;
                                                foreach (MatrixDataField dataField in matrixMap.MatrixData.MatrixDataFields)
                                                {
                                                    if (dataField.MatrixColumnId.Equals(colField.Id))
                                                    {
                                                        MatrixField rowField = matrixMap.MatrixRow.MatrixFields.Find(mf => mf.Id.Equals(dataField.MatrixRowId));
                                                        Excel.Range rowNameRange = ExcelHelper.GetRange(rowField.TargetNamedRange);

                                                        Excel.Range ColumnNameRange = ExcelHelper.GetRange(columnNameRange);
                                                        int columns = ColumnNameRange.Cells.Count;

                                                        outDataNameRange = dataField.TargetNamedRange;

                                                        int rows = rowNameRange.Cells.Count;

                                                        int colIndex = cnt;
                                                        for (int rowcnt = 0; rowcnt < rows; rowcnt++)
                                                        {
                                                            // Count is used for get value of datarow.
                                                            string filter = Constants.MATRIX_DATAFIELDID + " = '" + dataField.Id + "' and " +
                                                                            Constants.MATRIX_DATACOLINDEX + " = '" + existsColumnIndex + "' " + " and " +
                                                                            Constants.MATRIX_DATAROWINDEX + " = '" + rowcnt + "'";

                                                            DataRow[] dataRowIndex = matrixDataSet.MatrixDataTable.Select(filter);

                                                            if (dataRowIndex != null && dataRowIndex.Count() == 1)
                                                            {
                                                                DataRow matrixDataRow = matrixDataSet.MatrixDataTable.NewRow();
                                                                matrixDataRow[Constants.ID_ATTRIBUTE] = Guid.NewGuid();
                                                                matrixDataRow[Constants.VALUE_COLUMN] = string.Empty;
                                                                matrixDataRow[Constants.MATRIX_DATAFIELDID] = dataField.Id;
                                                                matrixDataRow[Constants.MATRIX_DATAROWINDEX] = rowcnt;
                                                                matrixDataRow[Constants.MATRIX_DATACOLINDEX] = colIndex;
                                                                matrixDataRow[Constants.MATRIX_DATAROWVALUE] = dataRowIndex[0].ItemArray[5];
                                                                matrixDataRow[Constants.MATRIX_DATACOLVALUE] = string.Empty;
                                                                matrixDataRow[Constants.MATRIX_ISCOLUMNVALUE] = true;

                                                                matrixDataSet.MatrixDataTable.Rows.Add(matrixDataRow);
                                                            }
                                                            else
                                                            {
                                                                int excelRowIndex = rowcnt + 1;
                                                                string value = string.Empty;
                                                                if (rowField.ValueType == MatrixValueType.FieldValue)
                                                                {
                                                                    value = Convert.ToString((ExcelHelper.GetRange(rowField.TargetNamedRange).Cells[excelRowIndex, 1] as Excel.Range).Value);
                                                                    id = Guid.NewGuid();
                                                                }
                                                                else if (rowField.ValueType == MatrixValueType.FieldLabel)
                                                                {
                                                                    if (id == Guid.Empty)
                                                                        id = Guid.NewGuid();
                                                                }

                                                                DataRow matrixDataRow = matrixDataSet.MatrixDataTable.NewRow();
                                                                matrixDataRow[Constants.ID_ATTRIBUTE] = id;
                                                                matrixDataRow[Constants.VALUE_COLUMN] = string.Empty;
                                                                matrixDataRow[Constants.MATRIX_DATAFIELDID] = dataField.Id;
                                                                matrixDataRow[Constants.MATRIX_DATAROWINDEX] = rowcnt;
                                                                matrixDataRow[Constants.MATRIX_DATACOLINDEX] = colIndex;
                                                                matrixDataRow[Constants.MATRIX_DATAROWVALUE] = value;
                                                                matrixDataRow[Constants.MATRIX_DATACOLVALUE] = string.Empty;
                                                                matrixDataRow[Constants.MATRIX_ISCOLUMNVALUE] = true;

                                                                matrixDataSet.MatrixDataTable.Rows.Add(matrixDataRow);
                                                            }
                                                        }

                                                        Excel.Range dataNameRange = ExcelHelper.GetRange(outDataNameRange);
                                                        Excel.Range topColumn = ModelNamedRange.Cells[1];
                                                        Excel.Range topDataColumn = dataNameRange.Cells[1];
                                                        //bottomRow = ExcelHelper.NextVerticalCell(firstselectedCell, RowsToAdd);

                                                        lastColumn = ModelNamedRange.Cells[ModelNamedRange.Cells.Count];
                                                        lastDataColumn = dataNameRange.Cells[dataNameRange.Cells.Count];

                                                        // If the insert was on the last column, update bottomRight range
                                                        if (bLastColumnInsert | (bHeader & bOneColumnsDatatable))
                                                        {
                                                            lastColumn = ExcelHelper.NextHorizontalCell(lastColumn, ColumnsToAdd);
                                                            lastDataColumn = ExcelHelper.NextHorizontalCell(lastDataColumn, ColumnsToAdd);
                                                        }

                                                        Excel.Range columnUpdatedRange = InsertWorksheet.get_Range(topColumn, lastColumn);
                                                        Excel.Range dataUpdatedRange = InsertWorksheet.get_Range(topDataColumn, lastDataColumn);


                                                        // Re-assign column the TargetNamedRange
                                                        ExcelHelper.AssignNameToRange(columnUpdatedRange, columnNameRange);

                                                        // Re-assign the Data column TargetNamedRange
                                                        ExcelHelper.AssignNameToRange(dataUpdatedRange, dataNameRange.Name.Name);


                                                        // Expand formula base on add rows
                                                        MatrixActionView View = new MatrixActionView();
                                                        DataFieldRendering rendering = MatrixActionView.QueryDataFieldRendering(rowField, colField);

                                                        if (rendering == DataFieldRendering.RenderDataHorizontally)
                                                        {
                                                            if (!bLastColumnInsert)
                                                            {

                                                                MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count, DataFieldRendering.RenderDataHorizontally);
                                                                View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count, DataFieldRendering.RenderDataHorizontally);
                                                            }
                                                            else
                                                            {
                                                                MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count + ColumnsToAdd, DataFieldRendering.RenderDataHorizontally);
                                                                View.ExpandMatrixFormulas(dataUpdatedRange, ModelNamedRange.Cells.Count + ColumnsToAdd, DataFieldRendering.RenderDataHorizontally);
                                                            }
                                                        }
                                                        else if (rendering == DataFieldRendering.RenderDataHorizontallyAndVertically)
                                                        {
                                                            if (!bLastColumnInsert)
                                                            {
                                                                //MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, rows, DataFieldRendering.RenderDataHorizontallyAndVertically, columns);
                                                                View.ExpandMatrixFormulas(dataUpdatedRange, rows, DataFieldRendering.RenderDataHorizontallyAndVertically, columns);
                                                            }
                                                            else
                                                            {
                                                                //MatrixActionView.ResetMatrixFormulas(dataUpdatedRange, rows, DataFieldRendering.RenderDataHorizontallyAndVertically, columns + ColumnsToAdd);
                                                                View.ExpandMatrixFormulas(dataUpdatedRange, rows, DataFieldRendering.RenderDataHorizontallyAndVertically, columns + ColumnsToAdd);
                                                            }
                                                        }
                                                        // End Formula
                                                    }
                                                }
                                            }

                                            //Accept Datatable changes
                                            matrixDataSet.MatrixDataTable.AcceptChanges();
                                        }
                                    }
                                    else
                                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXEDITACTCTL_AddColumn_InfoMsg"), Constants.PRODUCT_NAME);
                                }
                                // 4. User clicked on formula column
                                else if (firstselectedCell.Column == firstGridColumn + 1)
                                {
                                    // Do nothing - No column added.
                                    return false;
                                }
                            }
                            else
                                bAddCheckFailed = true;
                        }
                        else
                            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXEDITACTCTL_AddColumn_InfoMsg"), Constants.PRODUCT_NAME);                        
                    }
                    else
                        bAddCheckFailed = true;

                    if (bAddCheckFailed)
                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXEDITACTCTL_AddColumnSAVE_InfoMsg"), Constants.PRODUCT_NAME);

                    ////else if (!bLastRowInsert)
                    ////    ApttusMessageUtil.ShowError("Matrix Add Rows can be added in bottom only.", Constants.PRODUCT_NAME);
                }
                else
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("MATRIXEDITACTIONCTL_ProtectedCol_ErrorMsg"), Constants.PRODUCT_NAME);
                return !bAddCheckFailed;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        /// ApplyFormula on Column
        /// </summary>
        /// <param name="range"></param>
        //private void ApplyFormula(Excel.Range range, Excel.Range firstSelectedCell, int RowsToAdd)
        private void ApplyFormula(Excel.Range range, int formulaStartRow, int RowsToAdd)
        {
            Excel.Worksheet oSheet = range.Worksheet;
            int colIndex = 1;
            foreach (Excel.Range oRange in range.Columns)
            {
                Excel.Range formulaCell = range.Cells[2, colIndex];

                if (formulaCell.HasFormula)
                {
                    Excel.Range startCell = oSheet.Cells[formulaStartRow, oRange.Column];
                    Excel.Range endCell = ExcelHelper.NextVerticalCell(startCell, RowsToAdd - 1);
                    Excel.Range formulaRange = oSheet.Range[startCell, endCell];

                    formulaCell.Copy(formulaRange);
                }
                colIndex++;
            }
        }


    }
}

