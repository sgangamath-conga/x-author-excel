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
using Office = Microsoft.Office.Core;

using Apttus.XAuthor.Core;
using System.ComponentModel;

namespace Apttus.XAuthor.AppRuntime
{
    public class RuntimeEditActionController
    {
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        WorkbookProtectionManager workbookProtectionManager = WorkbookProtectionManager.GetInstance;
        RichTextDataManager richTextManager = RichTextDataManager.Instance;
        AttachmentsDataManager attachmentManager = AttachmentsDataManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        WaitWindowView waitWindow;

        Excel.Range Selection = null;
        Excel.Range ModelNamedRange = null;
        ObjectManager objectManager = ObjectManager.GetInstance;
        // Define Constructor , this constructor call from workflow
        public RuntimeEditActionController(Excel.Range Selection, Excel.Range NamedRange)
        {
            ModelNamedRange = NamedRange;
            this.Selection = Selection;
            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
            {
                waitWindow = new WaitWindowView(resourceManager.GetResource("DELETERECORDS_WaitWindow_InfoMsg"), false)
                {
                    StartPosition = FormStartPosition.CenterParent
                };
            }
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
            bool IsUserSheetProtection = ExcelHelper.IsSheetProtectedByUser(ModelNamedRange.Worksheet);
            try
            {

                if (IsUserSheetProtection)
                {
                    ExcelHelper.UpdateSheetProtection(ModelNamedRange.Worksheet, false);
                }
                ExcelHelper.ExcelApp.EnableEvents = false;
                Guid datasetId = (from dt in dataManager.AppDataTracker
                                  where dt.Location == ModelNamedRange.Name.Name && dt.Type == ObjectType.Repeating
                                  select dt.DataSetId).FirstOrDefault();

                Excel.Range IntersectRange = ExcelHelper.ExcelApp.Intersect(ModelNamedRange, Selection);
                // Check #1 - Add row is allowed if the selection and the repeating model named range intersect.
                if (IntersectRange != null)
                {
                    Excel.Range firstselectedCell = null;
                    foreach (Excel.Range Cell in IntersectRange.Cells)
                    {
                        firstselectedCell = Cell;
                        break;
                    }

                    int firstGridRow = ((Excel.Range)ModelNamedRange.Cells[1, 1]).Row;

                    SaveMap currentSaveMap = configurationManager.GetSaveMapbyTargetNamedRange(ModelNamedRange.Name.Name);
                    // Check #2 - Add row is allowed if there is a save map for the model name range.
                    if (currentSaveMap != null)
                    {
                        SaveGroup currentSaveGroup = configurationManager.GetSaveGroupbyTargetNamedRange(ModelNamedRange.Name.Name);
                        // Check #3 - Add row is allowed for vertical layout only.
                        if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Vertical"))
                        {
                            RepeatingGroup currentRepeatingGroup = configurationManager.GetRepeatingGroupbyTargetNamedRange(ModelNamedRange.Name.Name);

                            SaveField attachmentSaveField = attachmentManager.IsAttachmentFieldPartOfSaveMap(currentSaveMap, currentSaveGroup);

                            bool bRowsAdded = false;
                            Excel.Range firstInsertedCell = null;
                            Excel.Worksheet InsertWorksheet = firstselectedCell.Worksheet;
                            ExcelHelper.ExcelApp.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                            /************************************************************************/
                            /**** A D D   R O W   F R O M   H E A D E R   &  N O   D A T A S E T ****/
                            /************************************************************************/
                            // 2. User selected header row and datatable/dataset is not created
                            if (firstselectedCell.Row == firstGridRow && datasetId == Guid.Empty)
                            {
                                RetrieveActionView View = new RetrieveActionView();
                                // 2.1. Create a Repeating Group Model if its null. Default Layout is Vertical.
                                if (currentRepeatingGroup == null)
                                {
                                    currentRepeatingGroup = new RepeatingGroup() { Layout = "Vertical" };
                                    currentRepeatingGroup.RetrieveFields = new List<RetrieveField>();
                                    currentRepeatingGroup.AppObject = currentSaveGroup.AppObject;
                                    currentRepeatingGroup.TargetNamedRange = currentSaveGroup.TargetNamedRange;
                                }

                                // 2.2. Create an ApttusDataSet with blank rows
                                ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(currentRepeatingGroup.AppObject);
                                List<string> appFields = new List<string>();
                                DataTable dataTable = configurationManager.GetDataTableFromAllAppFields(apttusObject, false, true, out appFields);

                                string DataSetName = apttusObject.Name + "_" + DateTime.Now.ToString("hhmmssff") + "_" + currentSaveMap.Name;
                                ApttusDataSet newDataSet = new ApttusDataSet
                                {
                                    Name = DataSetName,
                                    AppObjectUniqueID = currentRepeatingGroup.AppObject,
                                    DataTable = dataTable
                                };

                                for (int i = 0; i < RowsToAdd; i++)
                                {
                                    DataRow dr = newDataSet.DataTable.NewRow();
                                    // Attachment
                                    attachmentManager.CreateAttachmentRecordID(attachmentSaveField, dr, ObjectType.Repeating);
                                    newDataSet.DataTable.Rows.Add(dr);
                                }

                                // Add the new Dataset to data manager. Fetch the added dataset which should contain the Id.
                                dataManager.AddData(newDataSet);
                                newDataSet = dataManager.AppData.Where(d => d.Name.Equals(newDataSet.Name)).FirstOrDefault();

                                // Assign Parent Dataset.
                                if (newDataSet.Parent == Guid.Empty)
                                    newDataSet.Parent = dataManager.GetParentDataSetId(currentRepeatingGroup.AppObject);

                                // 2.3. Use MapRepeatingCells to render initial data.
                                View.MapRepeatingCells(currentRepeatingGroup, newDataSet, currentSaveGroup.TargetNamedRange);

                                // 2.4 Calculate first inserted cell for data protection
                                bRowsAdded = true;
                                firstInsertedCell = ExcelHelper.NextVerticalCell(ModelNamedRange.Cells[1, 1], 2);

                                // 2.5 Recalculate the ModelNamedRange
                                ModelNamedRange = ExcelHelper.GetRange(currentSaveGroup.TargetNamedRange);

                                // 2.6 Accept Datatable changes
                                newDataSet.DataTable.AcceptChanges();
                            }
                            /*******************************************************************************/
                            /*** A D D   R O W   F R O M   H E A D E R   &  D A T A S E T    E X I S T S ***/
                            /************************************ O R **************************************/
                            /** A D D   R O W   F R O M   D A T A R O W   &  D A T A S E T    E X I S T S **/
                            /*******************************************************************************/
                            // 3. User selected 
                            //      EITHER  1) header row and datatable/dataset is created 
                            //      OR      2) data row excluding the formula row (datatable should exists)
                            else if ((firstselectedCell.Row == firstGridRow && datasetId != Guid.Empty) || (firstselectedCell.Row > firstGridRow + 1))
                            {
                                ApttusDataSet dataset = dataManager.GetDataByDataSetId(datasetId);

                                bool bIsGroupingOn = currentRepeatingGroup != null && !string.IsNullOrEmpty(currentRepeatingGroup.GroupByField);
                                List<string> groupFieldIds = null;
                                Dictionary<string, string> groupedValueById = null;
                                bool bAllowAddRow = true;

                                if (bIsGroupingOn)
                                {
                                    //1. First Check whether the selected row is not a header row or a formula row. 
                                    int nExcludedRepeatingRowCount = 2; //Exclude 1. Header Row. 2. FormulaRow
                                    int rowIndexInDataTable = (firstselectedCell.Row - firstGridRow - nExcludedRepeatingRowCount);
                                    if (rowIndexInDataTable <= 0)
                                        bAllowAddRow = false;
                                    else
                                    {
                                        //2. Check whether the selected row is not a grouped row. If a user selected a groupedRow, and then did addrow, it will fail.
                                        //A grouped row is a row, which contains GROUPING_ROW_ERRORMSG in RowError property of DataRow.

                                        DataRow dr = dataset.DataTable.Rows[rowIndexInDataTable];
                                        if (dr.RowError.Contains(Constants.GROUPING_ROW_ERRORMSG_ONLY))
                                            bAllowAddRow = false;
                                        else
                                        {
                                            groupFieldIds = new List<string>();
                                            //The order in which the groupbyField items are added is very important, because in FindGroupedValues, we just assign the groupbyvalues
                                            //the order in which the groupByFieldIds are stored in the list.
                                            groupFieldIds.Add(currentRepeatingGroup.GroupByField);
                                            if (!string.IsNullOrEmpty(currentRepeatingGroup.GroupByField2))
                                                groupFieldIds.Add(currentRepeatingGroup.GroupByField2);

                                            groupedValueById = FindGroupedValues(rowIndexInDataTable, dataset, ref groupFieldIds);
                                            bAllowAddRow = groupedValueById != null;
                                        }
                                    }
                                }

                                if (bAllowAddRow)
                                {
                                    bool bLastRowInsert = false;
                                    bool bHeader = false;
                                    bool bZeroRowsDatatable = dataset.DataTable != null && dataset.DataTable.Rows.Count == 0;

                                    if (firstselectedCell.Row - firstGridRow - 1 == dataset.DataTable.Rows.Count)
                                        bLastRowInsert = true;

                                    if (firstselectedCell.Row == firstGridRow)
                                        bHeader = true;

                                    Excel.Range topLeft = ModelNamedRange.Cells[1];
                                    Excel.Range bottomRight = ModelNamedRange.Cells[ModelNamedRange.Cells.Count];

                                    // 3.1 Loop through all the rows to be added to excel, named range and datatable
                                    Excel.Range rangeToInsert = null;
                                    string startRows = string.Empty;
                                    List<DataRow> dataRows = new List<DataRow>();
                                    if (bHeader)
                                    {
                                        // 3.2 Insert into excel
                                        Excel.Range headerStartAddRowCell = ExcelHelper.NextVerticalCell(firstselectedCell, 2);
                                        string rows = String.Format("{0}:{1}", headerStartAddRowCell.Row, headerStartAddRowCell.Row + RowsToAdd - 1);
                                        startRows = String.Format("{0}:{1}", headerStartAddRowCell.Row - firstGridRow + 1, headerStartAddRowCell.Row - firstGridRow + RowsToAdd);
                                        rangeToInsert = InsertWorksheet.Range[rows];
                                        rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);

                                        for (int i = 0; i < RowsToAdd; i++)
                                        {
                                            // 3.3 Insert into datatable
                                            DataRow dr = dataset.DataTable.NewRow();
                                            // Attachment
                                            attachmentManager.CreateAttachmentRecordID(attachmentSaveField, dr, ObjectType.Repeating);
                                            dataset.DataTable.Rows.InsertAt(dr, 0);
                                            dataRows.Add(dr);
                                        }
                                    }
                                    else
                                    {
                                        // 3.2 Insert into excel
                                        Excel.Range nonHeaderStartAddRowCell = ExcelHelper.NextVerticalCell(firstselectedCell, 1);
                                        string rows = String.Format("{0}:{1}", nonHeaderStartAddRowCell.Row, nonHeaderStartAddRowCell.Row + RowsToAdd - 1);
                                        startRows = String.Format("{0}:{1}", nonHeaderStartAddRowCell.Row - firstGridRow + 1, nonHeaderStartAddRowCell.Row - firstGridRow + RowsToAdd);
                                        rangeToInsert = InsertWorksheet.Range[rows];
                                        rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);

                                        for (int i = 0; i < RowsToAdd; i++)
                                        {
                                            // 3.3 Insert into datatable
                                            DataRow dr = dataset.DataTable.NewRow();
                                            if (bIsGroupingOn)
                                            {
                                                //The values assigned for a new row for grouped fields will be used for save purpose, in save action.                                                
                                                foreach (string fieldId in groupFieldIds)
                                                    dr[fieldId] = groupedValueById[fieldId];
                                            }
                                            // Attachment
                                            attachmentManager.CreateAttachmentRecordID(attachmentSaveField, dr, ObjectType.Repeating);
                                            dataset.DataTable.Rows.InsertAt(dr, (firstselectedCell.Row - firstGridRow - 1) + i);
                                            dataRows.Add(dr);
                                        }
                                    }

                                    bool bIsRichTextEditingEnabled = !configurationManager.IsRichTextEditingDisabled;

                                    if (bIsRichTextEditingEnabled)
                                    {
                                        List<SaveField> richTextSaveFields = (from sf in currentSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(currentSaveGroup.GroupId))
                                                                              where applicationDefinitionManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Rich_Textarea
                                                                              select sf).ToList();
                                        if (richTextSaveFields.Count > 0)
                                        {
                                            object[,] data = new object[RowsToAdd, 1];
                                            bool bUniqueDataGenerated = false;

                                            foreach (SaveField sf in richTextSaveFields)
                                            {
                                                Excel.Range col = ModelNamedRange.Rows[startRows].Columns[sf.TargetColumnIndex];
                                                if (!bUniqueDataGenerated)
                                                {
                                                    bUniqueDataGenerated = true;
                                                    for (int i = 0; i < RowsToAdd; ++i)
                                                        data[i, 0] = String.Format(Constants.RICHTEXT_FORMULA_ADD, Guid.NewGuid().ToString());
                                                }

                                                int rowIndex = 0;
                                                foreach (DataRow dr in dataRows)
                                                {
                                                    string id = (data[rowIndex++, 0] as string).Split(new char[] { '\"' })[1];
                                                    dr[sf.FieldId] = id;
                                                    richTextManager.AddRecord(id, sf.FieldId);
                                                }
                                                col.Value = data;
                                            }
                                            data = null;//Important
                                        }
                                    }

                                    // 3.4 If the insert was on the last row, update bottomRight range
                                    if (bLastRowInsert | (bHeader & bZeroRowsDatatable))
                                        bottomRight = ExcelHelper.NextVerticalCell(bottomRight, RowsToAdd);

                                    // 3.5 Re-assign the TargetNamedRange
                                    Excel.Range NamedRange = InsertWorksheet.get_Range(topLeft, bottomRight);
                                    ExcelHelper.AssignNameToRange(NamedRange, currentSaveGroup.TargetNamedRange);
                                    ModelNamedRange = NamedRange;

                                    // 3.6 Apply formula
                                    if (bHeader)
                                        ApplyFormula(NamedRange, firstselectedCell.Row + 2, RowsToAdd);
                                    else
                                        ApplyFormula(NamedRange, firstselectedCell.Row + 1, RowsToAdd);

                                    // 3.7 Calculate first inserted cell for data protection
                                    bRowsAdded = true;
                                    Excel.Range someInsertCell = bHeader ? ExcelHelper.NextVerticalCell(firstselectedCell, 2) : ExcelHelper.NextVerticalCell(firstselectedCell, 1);
                                    firstInsertedCell = InsertWorksheet.Cells[someInsertCell.Row, ((Excel.Range)ModelNamedRange.Cells[1, 1]).Column];

                                    // 3.8 Accept Datatable changes
                                    dataset.DataTable.AcceptChanges();
                                }
                                // 4. User clicked on formula row
                                else if (firstselectedCell.Row == firstGridRow + 1)
                                {
                                    // Do nothing - No rows added.
                                    return false;
                                }
                            }
                            // 5. Update Data Protection
                            if (bRowsAdded)
                            {
                                //add to memory if a row has been added. Therefore protection needs to be disabled when adding comments. 
                                string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
                                //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
                                ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);
                                if (appInstance != null)
                                    appInstance.RowAdded = bRowsAdded;
                                workbookProtectionManager.ProtectRows(currentSaveGroup.TargetNamedRange, firstInsertedCell, RowsToAdd);
                            }
                        }
                        else
                            bAddCheckFailed = true;
                    }
                    else
                        bAddCheckFailed = true;
                }
                else
                    bAddCheckFailed = true;

                if (bAddCheckFailed)
                {
                    ShowOrLogError(resourceManager.GetResource("EDITACTIONCTL_Activating_ErrorMsg"), resourceManager.GetResource("EDITACTIONCTL_ActivatingCap_ErrorMsg"));
                }
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
                if (IsUserSheetProtection)
                {
                    ExcelHelper.UpdateSheetProtection(ModelNamedRange.Worksheet, true);
                }
            }
        }

        private void ShowOrLogError(string Message, string Caption)
        {
            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                ApttusMessageUtil.ShowError(Message, Caption);
            else
                ExceptionLogHelper.DebugLog("{0}:- {1}", Caption, Message);
        }
        /// <summary>
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

        public void Paste(IDataObject iData, Excel.XlPasteType pasteType)
        {
            int rowCount = 0, colCount = 0;
            GetClipBoardTableDimension(iData, ref rowCount, ref colCount);
            if (rowCount > 0 && colCount > 0)
                Paste(rowCount, colCount, pasteType);
            // When a single cell is copied, both dimensions are 0. So this handling is added
            else if (rowCount == 0 && colCount == 0)
                Paste(1, 1, pasteType);
        }

        public void Paste(int rowCount, int colCount, Excel.XlPasteType pasteType)
        {
            int RowsToPaste = rowCount;
            if (RowsToPaste > 0)
            {
                MetadataManager metadataManager = MetadataManager.GetInstance;
                Excel.Range copiedData = null;
                copiedData = metadataManager.PasteCopiedData(colCount, rowCount, ModelNamedRange.Worksheet, pasteType);

                // 2. Insert blank rows before pasting data.
                if (copiedData != null && AddRow(RowsToPaste))
                {
                    bool IsUserSheetProtected = ExcelHelper.IsSheetProtectedByUser(ModelNamedRange.Worksheet);
                    if (IsUserSheetProtected)
                    {
                        ExcelHelper.UserSheetProtection(ModelNamedRange.Worksheet, false);
                    }
                    // 3. Calculate the Range to Paste Data
                    Excel.Range firstGridCell = ModelNamedRange.Cells[1, 1];
                    Excel.Range IntersectRange = ExcelHelper.ExcelApp.Intersect(ModelNamedRange, Selection);
                    Excel.Range firstselectedCell = IntersectRange.Cells[1, 1];

                    int IdColumnIndex = configurationManager.GetIdColumnIndex(ModelNamedRange.Name.Name);
                    int IdColumnIndexInExcel = firstGridCell.Column + IdColumnIndex - 1;

                    if (firstselectedCell.Column > IdColumnIndexInExcel || (firstselectedCell.Column + colCount - 1) < IdColumnIndexInExcel)
                    {
                        // 4.1 Execute Paste without Split
                        Excel.Range TopLeft = ExcelHelper.NextVerticalCell(firstselectedCell, firstselectedCell.Row == firstGridCell.Row ? 2 : 1);
                        Excel.Range BottomRight = ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(TopLeft, rowCount - 1), colCount - 1);
                        Excel.Range PasteRange = ModelNamedRange.Worksheet.get_Range(TopLeft, BottomRight);

                        // 4.1.1 Copy entire copiedData range to Clipboard using Excel out-of-the-box Copy (Ctrl+C)
                        copiedData.Copy();

                        // 4.1.2 Paste using Excel out-of-the-box Paste function (Ctrl+V)
                        PasteRange.PasteSpecial(pasteType);
                    }
                    else
                    {
                        // 4.2 Split the Paste into 2 Parts on either side of Id Column
                        // Left Half
                        if (firstselectedCell.Column < IdColumnIndexInExcel)
                        {
                            int LHSColumnCount = IdColumnIndexInExcel - firstselectedCell.Column;

                            // 4.2.1 Determine the PasteRange (where data will be pasted)
                            Excel.Range TopLeft = ExcelHelper.NextVerticalCell(firstselectedCell, firstselectedCell.Row == firstGridCell.Row ? 2 : 1);
                            Excel.Range BottomRight = ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(TopLeft, rowCount - 1), LHSColumnCount - 1);
                            Excel.Range PasteRange = ModelNamedRange.Worksheet.get_Range(TopLeft, BottomRight);

                            //PasteRange.Value = Utils.DataTableToArray(LHSPasteTable);

                            // 4.2.2 Copy LHS side of copiedData range to Clipboard using Excel out-of-the-box Copy (Ctrl+C)
                            Excel.Range copiedTopLeft = copiedData.Cells[1, 1];
                            Excel.Range copiedBottomRight = copiedData.Cells[copiedData.Rows.Count, LHSColumnCount];
                            Excel.Range copiedLHSData = copiedData.Worksheet.get_Range(copiedTopLeft, copiedBottomRight);
                            copiedLHSData.Copy();

                            // 4.2.3 Paste using Excel out-of-the-box Paste function (Ctrl+V)
                            PasteRange.PasteSpecial(pasteType);

                        }
                        // 4.3 Right Half
                        if (firstselectedCell.Column + colCount - 1 > IdColumnIndexInExcel)
                        {
                            int RHSColumnCount = firstselectedCell.Column + colCount - 1 - IdColumnIndexInExcel;

                            // 4.3.1 Determine the PasteRange (where data will be pasted)
                            Excel.Range TopLeft = ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(firstselectedCell, firstselectedCell.Row == firstGridCell.Row ? 2 : 1), IdColumnIndexInExcel - firstselectedCell.Column + 1);
                            Excel.Range BottomRight = ExcelHelper.NextHorizontalCell(ExcelHelper.NextVerticalCell(TopLeft, rowCount - 1), RHSColumnCount - 1);
                            Excel.Range PasteRange = ModelNamedRange.Worksheet.get_Range(TopLeft, BottomRight);

                            // 4.3.2 Copy RHS side of copiedData range to Clipboard using Excel out-of-the-box Copy (Ctrl+C)
                            Excel.Range copiedTopLeft = copiedData.Cells[1, copiedData.Columns.Count - RHSColumnCount + 1];
                            Excel.Range copiedBottomRight = copiedData.Cells[copiedData.Rows.Count, copiedData.Columns.Count];
                            Excel.Range copiedRHSData = copiedData.Worksheet.get_Range(copiedTopLeft, copiedBottomRight);
                            copiedRHSData.Copy();

                            // 4.3.3 Paste using Excel out-of-the-box Paste function (Ctrl+V)
                            PasteRange.PasteSpecial(pasteType);
                        }
                    }

                    metadataManager.ClearPastedData(copiedData);
                    if (IsUserSheetProtected)
                    {
                        ExcelHelper.UserSheetProtection(ModelNamedRange.Worksheet, true);
                    }
                }
            }
        }


        /// <summary>
        /// Remove Rows
        /// </summary>
        /// <param name="RowsToRemove"></param>
        public void RemoveRow(int RowsToRemove)
        {
            ApttusDataSet dataset = null;
            bool bDeleteCheckFailed = false;
            bool IsUserSheetProtection = ExcelHelper.IsSheetProtectedByUser(ModelNamedRange.Worksheet);
            try
            {

                if (IsUserSheetProtection)
                {
                    ExcelHelper.UpdateSheetProtection(ModelNamedRange.Worksheet, false);
                }
                ExcelHelper.ExcelApp.EnableEvents = false;
                Guid datasetId = (from dt in dataManager.AppDataTracker
                                  where dt.Location == ModelNamedRange.Name.Name && dt.Type == ObjectType.Repeating
                                  select dt.DataSetId).FirstOrDefault();

                int firstGridRow = ((Excel.Range)ModelNamedRange.Cells[1, 1]).Row;

                // Check #1 - Delete row is allowed if the selection and the repeating model named range intersect.
                Excel.Range IntersectRange = ExcelHelper.ExcelApp.Intersect(ModelNamedRange, Selection);
                if (IntersectRange != null)
                {
                    SaveMap currentSaveMap = configurationManager.GetSaveMapbyTargetNamedRange(ModelNamedRange.Name.Name);
                    // Check #2 - Delete row is allowed if there is a save map for the model name range.
                    if (currentSaveMap != null)
                    {
                        SaveGroup currentSaveGroup = configurationManager.GetSaveGroupbyTargetNamedRange(ModelNamedRange.Name.Name);
                        // Check #3 - Delete row is allowed for vertical layout only.
                        if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Vertical"))
                        {
                            int IdColumnIndex = 0;
                            RepeatingGroup currentRepeatingGroup = configurationManager.GetRepeatingGroupbyTargetNamedRange(ModelNamedRange.Name.Name);

                            // Check #4 - Delete row is allowed for non-grouped data and if the Id column exists.
                            if (currentRepeatingGroup != null)
                            {
                                ApttusObject currentAppObject = applicationDefinitionManager.GetAppObject(currentRepeatingGroup.AppObject);
                                if (currentAppObject != null && currentRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == currentAppObject.IdAttribute))
                                {
                                    // In case of Save of Displayed and Id Column is in Repeating Group
                                    IdColumnIndex = (from rf in currentRepeatingGroup.RetrieveFields where rf.FieldId == currentAppObject.IdAttribute select rf.TargetColumnIndex).FirstOrDefault();
                                }
                            }
                            else if (currentSaveMap != null && currentSaveGroup != null)
                            {
                                ApttusObject currentAppObject = applicationDefinitionManager.GetAppObject(currentSaveGroup.AppObject);
                                if (currentAppObject != null && currentSaveMap.SaveFields.Exists(sf => sf.FieldId == currentAppObject.IdAttribute))
                                {
                                    // In case of Save Other field and Id Column is in Save Field
                                    IdColumnIndex = (from sf in currentSaveMap.SaveFields where sf.FieldId == currentAppObject.IdAttribute & sf.GroupId == currentSaveGroup.GroupId select sf.TargetColumnIndex).FirstOrDefault();
                                }
                            }

                            if (IdColumnIndex > 0)
                            {
                                dataset = dataManager.GetDataByDataSetId(datasetId);

                                Dictionary<int, string> VisibleRowNumbers = new Dictionary<int, string>();
                                bool bStopDelete = false;
                                bool bIsGroupingOn = false;
                                if (currentRepeatingGroup != null)
                                    bIsGroupingOn = !String.IsNullOrEmpty(currentRepeatingGroup.GroupByField);

                                foreach (Excel.Range oRow in IntersectRange.Rows)
                                    if (!oRow.Hidden)
                                    {
                                        if (bIsGroupingOn)
                                        {
                                            int nExcludedRepeatingRowCount = 2; //Exclude 1. Header Row. 2. FormulaRow
                                            int rowIndexInDataTable = (oRow.Row - firstGridRow - nExcludedRepeatingRowCount);
                                            if (rowIndexInDataTable > 0)
                                            {
                                                DataRow dr = dataset.DataTable.Rows[rowIndexInDataTable]; //Grouped Rows cannot be deleted.
                                                if (!dr.RowError.Contains(Constants.GROUPING_ROW_ERRORMSG_ONLY))
                                                {
                                                    if (!VisibleRowNumbers.ContainsKey(oRow.Row))
                                                        VisibleRowNumbers.Add(oRow.Row, ((Excel.Range)ModelNamedRange.Cells[oRow.Row - firstGridRow + 1, IdColumnIndex]).Value2);
                                                }
                                                else
                                                    bStopDelete = true;
                                            }
                                            else
                                                bStopDelete = true;

                                            if (bStopDelete)
                                                break;
                                        }
                                        else
                                            VisibleRowNumbers.Add(oRow.Row, ((Excel.Range)ModelNamedRange.Cells[oRow.Row - firstGridRow + 1, IdColumnIndex]).Value2);
                                    }

                                if (bStopDelete)
                                {
                                    VisibleRowNumbers.Clear();
                                    ShowOrLogError(resourceManager.GetResource("EDITACTIONCTL_GrpHeaderRow_Msg"), resourceManager.GetResource("EDITACTIONCTL_GrpHeaderRowCap_Msg"));
                                    return;
                                }
                                // 1.1 Confirm record deletion
                                if (VisibleRowNumbers.Count > 0)
                                {
                                    string DeleteMessage = String.Format(resourceManager.GetResource("RUNTIMEEDITACT_RemoveRow_WarnMsg"), VisibleRowNumbers.Count.ToString(), resourceManager.CRMName);
                                    if (VisibleRowNumbers.Any(r => string.IsNullOrEmpty(r.Value)))
                                        DeleteMessage += resourceManager.GetResource("RUNTIMEEDITACT_RemoveRowNote_WarnMsg");
                                    if (IntersectRange.Rows.Count > VisibleRowNumbers.Count)
                                        DeleteMessage += String.Format(resourceManager.GetResource("RUNTIMEEDITACT_RemoveRowIntersect_WarnMsg"), (IntersectRange.Rows.Count - VisibleRowNumbers.Count).ToString());

                                    RemoveRecords(DeleteMessage, IntersectRange.Worksheet, VisibleRowNumbers, dataset, firstGridRow, datasetId, currentSaveMap, currentSaveGroup);
                                }
                            }
                            else
                                bDeleteCheckFailed = true;
                        }
                        else
                            bDeleteCheckFailed = true;
                    }
                    else
                        bDeleteCheckFailed = true;
                }
                else
                    bDeleteCheckFailed = true;

                if (bDeleteCheckFailed)
                    ShowOrLogError(resourceManager.GetResource("EDITACTIONCTL_Activating_ErrorMsg"), resourceManager.GetResource("EDITACTIONCTL_DelActionCap_Msg"));

            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ShowOrLogError(resourceManager.GetResource("EDITACTIONCTL_DelAction_Msg") + ex.Message.ToString(), resourceManager.GetResource("EDITACTIONCTL_DelActionCap_Msg"));
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
                if (IsUserSheetProtection)
                {
                    ExcelHelper.UpdateSheetProtection(ModelNamedRange.Worksheet, true);
                }
            }
        }
        private void PerformDeleteInBackground(List<ApttusSaveRecord> SaveRecords)
        {
            BackgroundWorker DeleteWorker = new BackgroundWorker();
            DeleteWorker.DoWork += new DoWorkEventHandler(DeleteWorker_DoWork);
            DeleteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DeleteWorker_WorkCompleted);
            DeleteWorker.RunWorkerAsync(SaveRecords);
        }

        private void DeleteWorker_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (waitWindow != null)
            {
                waitWindow.CloseWaitWindow();
                waitWindow = null;
            }
        }

        private void DeleteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                objectManager.Delete((List<ApttusSaveRecord>)e.Argument, false, waitWindow);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                e.Result = true;
            }
        }

        /// <summary>
        /// Removes records from DataTable, Excel and connected CRM.
        /// </summary>
        /// <param name="RemoveWorksheet"></param>
        /// <param name="VisibleRowNumbers"></param>
        /// <param name="dataset"></param>
        /// <param name="firstGridRow"></param>
        /// <param name="datasetId"></param>
        /// <param name="currentSaveMap"></param>
        /// <param name="currentSaveGroup"></param>
        internal void RemoveRecords(string DeleteMessage, Excel.Worksheet RemoveWorksheet, Dictionary<int, string> VisibleRowNumbers, ApttusDataSet dataset, int firstGridRow, Guid datasetId, SaveMap currentSaveMap, SaveGroup currentSaveGroup)
        {
            if (!string.IsNullOrEmpty(DeleteMessage) && ObjectManager.RuntimeMode == RuntimeMode.AddIn)
            {
                TaskDialogResult dr = ApttusMessageUtil.ShowWarning(DeleteMessage, resourceManager.GetResource("RUNTIMEEDITACT_RemoveRowCAP_WarnMsg"), ApttusMessageUtil.YesNo);
                if (dr == TaskDialogResult.No)
                    return;
            }

            Dictionary<int, string> RowNumbersWithId = new Dictionary<int, string>();
            Dictionary<int, string> RowNumbersWithoutId = new Dictionary<int, string>();
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();

            foreach (var VisibleRowNumber in VisibleRowNumbers)
            {
                // 2. Exclude Header and Formula row from IntersectRange (if they exist)
                if ((VisibleRowNumber.Key > firstGridRow + 1) && (datasetId != Guid.Empty))
                {
                    // 3.1 Delete from Salesforce (CRM)
                    //string RecordId = Convert.ToString(dataset.DataTable.Rows[VisibleRowNumber.Key - firstGridRow - 2][Constants.ID_ATTRIBUTE]);
                    if (string.IsNullOrEmpty(VisibleRowNumber.Value))
                        RowNumbersWithoutId.Add(VisibleRowNumber.Key, VisibleRowNumber.Value);
                    else
                    {
                        RowNumbersWithId.Add(VisibleRowNumber.Key, VisibleRowNumber.Value);
                        ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                        {
                            SaveFields = new List<ApttusSaveField>(),
                            ApttusDataSetId = dataset.Id,
                            OperationType = QueryTypes.DELETE,
                            ObjectName = applicationDefinitionManager.GetAppObject(dataset.AppObjectUniqueID).Id,
                            RecordId = VisibleRowNumber.Value,
                            RecordRowNo = VisibleRowNumber.Key - firstGridRow - 2,
                            RecordColumnNo = -1
                        };
                        SaveRecords.Add(SaveRecord);
                    }
                }
            }

            if (RowNumbersWithId.Count > 0)
            {
                PerformDeleteInBackground(SaveRecords);
                if (waitWindow != null)
                    waitWindow.ShowDialog();

                ApttusSaveSummary SaveSummary = SaveHelper.ProcessResults(SaveRecords, true);
                if (SaveSummary.DeleteCount > 0)
                {
                    var SuccessfulDeletionRowNumbers = (from r in RowNumbersWithId
                                                        join sr in SaveRecords on r.Value equals sr.RecordId
                                                        where sr.Success == true
                                                        select r);

                    List<SaveField> richTextSaveFields = (from sf in currentSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(currentSaveGroup.GroupId))
                                                          where applicationDefinitionManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Rich_Textarea
                                                          select sf).ToList();

                    bool bHasRichTextFields = richTextSaveFields.Count > 0;

                    List<SaveField> attachmentsFields = (from sf in currentSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(currentSaveGroup.GroupId))
                                                         where applicationDefinitionManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Attachment
                                                         select sf).ToList();

                    bool bHasAttachmentsFields = attachmentsFields.Count > 0;

                    // 3.2 Delete from Datatable, if any delete records are present
                    foreach (var SuccessfulDeletionRowNumber in SuccessfulDeletionRowNumbers)
                    {
                        DataRow RowToDelete = dataset.DataTable.AsEnumerable().Where(r => r.Field<string>(Constants.ID_ATTRIBUTE) == SuccessfulDeletionRowNumber.Value).FirstOrDefault();

                        // 3.3 Delete from RichTextDatatable, if any delete records are present.
                        if (bHasRichTextFields)
                        {
                            foreach (SaveField sf in richTextSaveFields)
                            {
                                string id = RowToDelete[Constants.ID_ATTRIBUTE] as string;
                                if (!String.IsNullOrEmpty(id))
                                    richTextManager.RemoveRecord(id, sf.FieldId);
                            }
                        }

                        // Delete from attachment data, if any delete records are present
                        if (bHasAttachmentsFields)
                        {
                            string id = RowToDelete[Constants.ID_ATTRIBUTE] as string;
                            if (!String.IsNullOrEmpty(id))
                                attachmentManager.RemoveRecord(dataset, id);
                        }
                        RowToDelete.Delete();
                        RowToDelete.AcceptChanges();
                    }

                    // 3.3 Delete from Excel, if any delete records are present
                    // Order By Descending so rows are deleted from below and sheet row numbers remain valid even after first chunk is deleted.
                    List<int> ChunkSuccessfulDeletionRowNumbers = new List<int>();
                    foreach (var SuccessfulDeletionRowNumber in SuccessfulDeletionRowNumbers.OrderByDescending(s => s.Key))
                    {
                        if (ChunkSuccessfulDeletionRowNumbers.Count == 0)
                            ChunkSuccessfulDeletionRowNumbers.Add(SuccessfulDeletionRowNumber.Key);
                        else
                        {
                            if (ChunkSuccessfulDeletionRowNumbers[ChunkSuccessfulDeletionRowNumbers.Count - 1] - SuccessfulDeletionRowNumber.Key == 1)
                            {
                                // Continous Range - Add to collection chunk
                                ChunkSuccessfulDeletionRowNumbers.Add(SuccessfulDeletionRowNumber.Key);
                            }
                            else
                            {
                                // Non-Continous Range - Delete entire chunk of rows
                                String rows = String.Format("{0}:{1}", ChunkSuccessfulDeletionRowNumbers.Min(), ChunkSuccessfulDeletionRowNumbers.Max());
                                Excel.Range rangeToDelete = RemoveWorksheet.Range[rows];
                                rangeToDelete.EntireRow.Delete(Excel.XlInsertShiftDirection.xlShiftDown);

                                ChunkSuccessfulDeletionRowNumbers.Clear();
                                ChunkSuccessfulDeletionRowNumbers.Add(SuccessfulDeletionRowNumber.Key);
                            }
                        }
                    }

                    if (ChunkSuccessfulDeletionRowNumbers.Count > 0)
                    {
                        // Delete entire chunk of rows
                        String rows = String.Format("{0}:{1}", ChunkSuccessfulDeletionRowNumbers.Min(), ChunkSuccessfulDeletionRowNumbers.Max());
                        Excel.Range rangeToDelete = RemoveWorksheet.Range[rows];
                        rangeToDelete.EntireRow.Delete(Excel.XlInsertShiftDirection.xlShiftDown);
                    }

                }
            }

            if (RowNumbersWithoutId.Count > 0)
            {
                ExcelHelper.UpdateSheetProtection(RemoveWorksheet, false);
                List<SaveField> richTextSaveFields = (from sf in currentSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(currentSaveGroup.GroupId))
                                                      where applicationDefinitionManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Rich_Textarea
                                                      select sf).ToList();

                bool bHasRichTextFields = richTextSaveFields.Count > 0;

                foreach (var RowNumberWithoutId in RowNumbersWithoutId.OrderByDescending(s => s.Key))
                {
                    // 3.2 Delete from Datatable, if any delete records are present
                    dataset.DataTable.Rows.RemoveAt(RowNumberWithoutId.Key - firstGridRow - 2);

                    // 3.3 Remove RichText Fields from RichTextDataManager if any are present.
                    if (bHasRichTextFields)
                    {
                        foreach (SaveField sf in richTextSaveFields)
                        {
                            string formula = ModelNamedRange.Rows[RowNumberWithoutId.Key - firstGridRow + 1].Cells[sf.TargetColumnIndex].Formula as string;
                            if (String.IsNullOrEmpty(formula))
                                continue;
                            string[] values = formula.Split(new char[] { '\"' });
                            if (values.Count() >= 2 && !String.IsNullOrEmpty(values[1]))
                            {
                                richTextManager.RemoveRecord(values[1], sf.FieldId);
                            }
                        }
                    }

                    // 3.4 Delete from Excel, if any delete records are present
                    ((Excel.Range)RemoveWorksheet.Rows[RowNumberWithoutId.Key]).EntireRow.Delete(Excel.XlInsertShiftDirection.xlShiftDown);
                }
                dataset.DataTable.AcceptChanges();

                // 3.4 Update Data Protection for deleted rows without Id
                DataProtectionModel dpm = dataManager.DataProtection.FirstOrDefault(dp => dp.WorksheetName.Equals(RemoveWorksheet.Name)
                    & dp.SaveMapId.Equals(currentSaveMap.Id) & dp.SaveGroupAppObject.Equals(currentSaveGroup.AppObject));
                dpm.LockCount -= RowNumbersWithoutId.Count;

                // 3.5 Remove from DataProtection collection if LockCount is less than 1.
                if (dpm.LockCount < 1)
                    dataManager.DataProtection.Remove(dpm);

                // 3.6 Add sheet protection if DataProtection still exists
                if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(RemoveWorksheet.Name)))
                    ExcelHelper.UpdateSheetProtection(RemoveWorksheet, true);
            }
        }

        private DataTable GetClipBoardDataUsingCSVFormat(IDataObject iData)
        {
            DataTable dt = new DataTable("ExcelData");
            StreamReader ClipboardStream = new StreamReader((Stream)iData.GetData(DataFormats.CommaSeparatedValue));

            while (ClipboardStream.Peek() > 0)
            {
                string FormattedData = ClipboardStream.ReadLine();
                string[] RowData = FormattedData.Split(",".ToCharArray());

                if (dt.Columns.Count <= 0)
                    foreach (var ColumnData in RowData)
                        dt.Columns.Add();

                int ColumnCounter = 0;
                DataRow dr = dt.NewRow();

                foreach (var ColumnData in RowData)
                {
                    dr[ColumnCounter] = ColumnData;
                    ColumnCounter++;
                }

                dt.Rows.Add(dr);
            }

            ClipboardStream.Close();
            return dt;
        }

        private void GetClipBoardTableDimension(IDataObject iData, ref int rowCount, ref int colCount)
        {
            rowCount = colCount = 0;
            string StrRtfData = iData.GetData(DataFormats.Rtf) as string;

            // 1. Fetch the Row Count from Clipboard
            string word = "trowd";
            rowCount = StrRtfData.Split(new string[] { word }, StringSplitOptions.RemoveEmptyEntries).Count() - 1;

            int idxRowEnd = 0;
            int idxRowStart = 0;
            int row = 0;
            List<string> rowData = new List<string>();

            idxRowEnd = StrRtfData.IndexOf(@"\row", idxRowEnd, StringComparison.OrdinalIgnoreCase);

            if (idxRowEnd < 0)
                return;
            else if (StrRtfData[idxRowEnd - 1] == '\\')
            {
                idxRowEnd++;
            }

            idxRowStart = StrRtfData.LastIndexOf(@"\trowd", idxRowEnd, StringComparison.OrdinalIgnoreCase);

            if (idxRowStart < 0)
                return;
            else if (StrRtfData[idxRowStart - 1] == '\\')
            {
                idxRowEnd++;
            }

            string RowStr = StrRtfData.Substring(idxRowStart, idxRowEnd - idxRowStart);
            idxRowEnd++;

            int idxCell = 0;
            int idxCellMem = 0;

            do
            {
                idxCell = RowStr.IndexOf(@"\Cell ", idxCell, StringComparison.OrdinalIgnoreCase);
                if (idxCell < 0)
                    break;
                else if (RowStr[idxCell - 1] == '\\')
                {
                    idxCell++;
                    continue;
                }

                rowData.Add(PurgeRtfCmds(RowStr.Substring(idxCellMem, idxCell - idxCellMem)));
                idxCellMem = idxCell;
                idxCell++;
            }
            while (idxCellMem > 0);

            // 2. Fetch the Column Count from Clipboard
            colCount = rowData.Count;

            // Clean up
            StrRtfData = null;
        }

        string PurgeRtfCmds(string StrRtf)
        {
            int idxRtfStart = 0;
            int idxRtfEnd = 0;

            while (idxRtfStart < StrRtf.Length)
            {
                idxRtfStart = StrRtf.IndexOf('\\', idxRtfStart);
                if (idxRtfStart < 0) break;
                if (StrRtf[idxRtfStart + 1] == '\\')
                {
                    StrRtf = StrRtf.Remove(idxRtfStart, 1);   //1 offset to erase space
                    idxRtfStart++; //sckip "\"
                }
                else
                {
                    idxRtfEnd = StrRtf.IndexOf(' ', idxRtfStart);
                    if (idxRtfEnd < 0)
                        if (StrRtf.Length > 0)
                            idxRtfEnd = StrRtf.Length - 1;
                        else
                            break;
                    StrRtf = StrRtf.Remove(idxRtfStart, idxRtfEnd - idxRtfStart + 1);   //1 offset to erase space
                }

            }

            //Erase spaces at the end of the cell info.
            if (StrRtf.Length > 0)
                while (StrRtf.Length > 0 && StrRtf[StrRtf.Length - 1] == ' ')
                    StrRtf = StrRtf.Remove(StrRtf.Length - 1);

            //Erase spaces at the beginning of the cell info.
            if (StrRtf.Length > 0)
                while (StrRtf[0] == ' ')
                    StrRtf = StrRtf.Substring(1, StrRtf.Length - 1);

            return StrRtf;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddMultiSelectPickListValues()
        {
            Excel.Range namedRange = this.ModelNamedRange;
            Excel.Range Target = this.Selection;

            bool isMultiSelectField = IsSelectionMultiSelectField(namedRange, Target);

            if (isMultiSelectField)
            {
                ExcelHelper.ExcelApp.EnableEvents = false;
                string newValue = Convert.ToString(Target.Value);

                // Get Undo count using commandbars, if there is nothing to undo return.
                Office.CommandBarControl comctl = ExcelHelper.ExcelApp.CommandBars.FindControl(Type.Missing, ((object)128), Type.Missing, Type.Missing);
                if (((dynamic)comctl).accChildCount > 0)
                {
                    ExcelHelper.ExcelApp.Undo();
                    string oldValue = Convert.ToString(Target.Value);

                    Target.Value = newValue;

                    if (string.IsNullOrEmpty(oldValue))
                    {
                        // do nothing
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(newValue))
                        {
                            // do nothing
                        }
                        else
                        {
                            bool valueUsed = oldValue.Contains(newValue);
                            if (valueUsed)
                            {
                                if (oldValue == newValue)
                                    Target.Value = string.Empty;
                                else if (oldValue.Substring(oldValue.Length - newValue.Length) == newValue)
                                    Target.Value = oldValue.Substring(0, oldValue.Length - newValue.Length - 1);
                                else
                                    Target.Value = oldValue.Replace(newValue + ";", "");
                            }
                            else
                                Target.Value = oldValue + ";" + newValue;
                        }
                    }
                }
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedRange"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        internal bool IsSelectionMultiSelectField(Excel.Range namedRange, Excel.Range Target)
        {
            bool isMultiSelectField = false;
            string nameRange = namedRange.Name.Name;

            if (namedRange != null)
            {
                // Look through all repeating groups
                RepeatingGroup currentRepeatingGroup = ConfigurationManager.GetInstance.GetRepeatingGroupbyTargetNamedRange(nameRange);
                if (currentRepeatingGroup != null)
                {
                    List<RetrieveField> multiSelectFields = currentRepeatingGroup.RetrieveFields.Where(f => f.DataType == Datatype.Picklist_MultiSelect && fieldLevelSecurity.IsFieldVisible(f.AppObject, f.FieldId)).ToList();
                    if (currentRepeatingGroup.Layout.Equals("Vertical"))
                    {
                        foreach (RetrieveField rf in multiSelectFields)
                        {
                            if (ExcelHelper.GetRange(rf.TargetNamedRange).Column == Target.Column)
                            {
                                isMultiSelectField = true;
                                break;
                            }
                        }
                    }
                    else if (currentRepeatingGroup.Layout.Equals("Horizontal"))
                    {
                        foreach (RetrieveField rf in multiSelectFields)
                        {
                            if (ExcelHelper.GetRange(rf.TargetNamedRange).Row == Target.Row)
                            {
                                isMultiSelectField = true;
                                break;
                            }
                        }
                    }
                }

                // Look through all save groups
                SaveMap currentSaveMap = ConfigurationManager.GetInstance.GetSaveMapbyTargetNamedRange(nameRange);
                if (currentSaveMap != null)
                {
                    SaveGroup currentSaveGroup = ConfigurationManager.GetInstance.GetSaveGroupbyTargetNamedRange(nameRange);
                    List<SaveField> multiSelectFields = currentSaveMap.SaveFields.Where(sf => sf.SaveFieldType == SaveType.SaveOnlyField).ToList();

                    if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Vertical"))
                    {
                        foreach (SaveField sf in multiSelectFields)
                        {
                            ApttusField apttusField = ApplicationDefinitionManager.GetInstance.GetField(currentSaveGroup.AppObject, sf.FieldId);
                            if (apttusField.Datatype == Datatype.Picklist_MultiSelect)
                            {
                                if (ExcelHelper.GetRange(sf.TargetNamedRange).Column == Target.Column)
                                {
                                    isMultiSelectField = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Horizontal"))
                    {
                        foreach (SaveField sf in multiSelectFields)
                        {
                            ApttusField apttusField = ApplicationDefinitionManager.GetInstance.GetField(currentSaveGroup.AppObject, sf.FieldId);
                            if (apttusField.Datatype == Datatype.Picklist_MultiSelect)
                            {
                                if (ExcelHelper.GetRange(sf.TargetNamedRange).Row == Target.Row)
                                {
                                    isMultiSelectField = true;
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            // Look through all Individual Display fields
            RetrieveField retField = ConfigurationManager.GetInstance.GetRetrieveFieldbyTargetNamedRange(nameRange);
            if (retField != null && retField.DataType == Datatype.Picklist_MultiSelect)
                isMultiSelectField = true;

            // Look through all Individual Save Other fields
            SaveField saveField = configurationManager.GetSaveFieldbyTargetNamedRange(nameRange);
            if (saveField != null)
            {
                ApttusField aptsSaveField = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId);
                if (aptsSaveField.Datatype == Datatype.Picklist_MultiSelect)
                    isMultiSelectField = true;
            }

            return isMultiSelectField;
        }

    }
}

