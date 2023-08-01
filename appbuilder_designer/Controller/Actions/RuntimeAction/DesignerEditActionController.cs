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

namespace Apttus.XAuthor.AppDesigner
{
    public class DesignerEditActionController
    {
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        //Excel.Range Selection = null;
        Excel.Range ModelNamedRange = null;

        public DesignerEditActionController(Excel.Range NamedRange)
        {
            this.ModelNamedRange = NamedRange;
        }

        public bool UpdateRows(SaveGroup saveGroup, int rowCount)
        {
            bool bAddCheckFailed = false;

            try
            {
                // Check #1 - Preloaded rows is allowed for a valid save group and vertical layout only.

                if (saveGroup != null && saveGroup.Layout.Equals("Vertical"))
                {
                    RepeatingGroup currentRepeatingGroup = configurationManager.GetRepeatingGroupbyTargetNamedRange(saveGroup.TargetNamedRange);
                    // Check #2 - Add row is allowed for either "Save Only Group" or for "Repeating Group with Non-Grouped Data".
                    if (currentRepeatingGroup == null || (currentRepeatingGroup != null && string.IsNullOrEmpty(currentRepeatingGroup.GroupByField)))
                    {
                        // Scenario 1: No Rows exists and first time rows are preloaded
                        bool bFirstTimeAdd = (saveGroup.LoadedRows == 0 && rowCount > 0) ? true : false;
                        // Scenario 2: Rows exists and more rows are preloaded
                        bool bMoreRowsAdd = (saveGroup.LoadedRows > 0 && rowCount > saveGroup.LoadedRows) ? true : false;
                        // Scenario 3: Rows exists and rows are reduced (including reduced to 0)
                        bool bRowsReduced = (saveGroup.LoadedRows > 0 && rowCount < saveGroup.LoadedRows) ? true : false;

                        Globals.ThisAddIn.Application.CutCopyMode = Excel.XlCutCopyMode.xlCopy;

                        Excel.Worksheet InsertWorksheet = ModelNamedRange.Worksheet;
                        Excel.Range topLeft = null, startAddRowCell = null, bottomRight = null;

                        if (bFirstTimeAdd || bMoreRowsAdd)
                        {
                            if (bFirstTimeAdd)
                                startAddRowCell = ExcelHelper.NextVerticalCell(ModelNamedRange.Cells[1, 1], 2);
                            else if (bMoreRowsAdd)
                                startAddRowCell = ExcelHelper.NextVerticalCell(ModelNamedRange.Cells[1, 1], saveGroup.LoadedRows + 2);

                            String rows = String.Format("{0}:{1}", startAddRowCell.Row, startAddRowCell.Row + rowCount - 1);
                            Excel.Range rangeToInsert = InsertWorksheet.Range[rows];
                            rangeToInsert.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                        }
                        else if (bRowsReduced)
                        {
                            int rowsToReduce = saveGroup.LoadedRows - rowCount;
                            bottomRight = ModelNamedRange.Cells[ModelNamedRange.Cells.Count];
                            String rows = String.Format("{0}:{1}", bottomRight.Row - (saveGroup.LoadedRows - rowCount) + 1, bottomRight.Row);
                            Excel.Range rangeToDelete = InsertWorksheet.Range[rows];
                            rangeToDelete.EntireRow.Delete(Excel.XlInsertShiftDirection.xlShiftDown);
                        }

                        // 1.4 Re-assign the TargetNamedRange
                        topLeft = ModelNamedRange.Cells[1, 1];
                        bottomRight = ExcelHelper.NextVerticalCell(ModelNamedRange.Cells[1, ModelNamedRange.Columns.Count], rowCount + 1);
                        Excel.Range NamedRange = InsertWorksheet.get_Range(topLeft, bottomRight);
                        ExcelHelper.AssignNameToRange(NamedRange, saveGroup.TargetNamedRange, true);
                        ModelNamedRange = NamedRange;

                        // 1.5 Apply formula
                        ApplyFormula(NamedRange, topLeft.Row + 2, rowCount);

                    }
                    else
                        bAddCheckFailed = true;
                }
                else
                    bAddCheckFailed = true;
                //}
                //else
                //    bAddCheckFailed = true;
                //}
                //else
                //    bAddCheckFailed = true;

                if (bAddCheckFailed)
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("EDITACTIONCTL_Activating_ErrorMsg"), resourceManager.GetResource("COMMON_AddRowAction_Text"));

                return !bAddCheckFailed;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
        }

        /// <summary>
        /// ApplyFormula on Column
        /// </summary>
        /// <param name="range"></param>
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

