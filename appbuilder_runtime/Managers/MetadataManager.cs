/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class MetadataManager
    {
        private static MetadataManager instance;
        private static object syncRoot = new Object();
        private const int UniqueIdColumn = 1;
        private const int DummyPicklistValueColumn = 2;
        private const int FlagsColumn = 3;
        private const int FormulaLocalizationColumn = 4;

        private enum Flag
        {
            FirstLaunch = 1
        }

        private MetadataManager()
        {
        }

        public static MetadataManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new MetadataManager();
                    }
                }

                return instance;
            }
        }

        public Excel.Worksheet MetadataSheet
        {
            get
            {
                return ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
            }
        }

        public string GetAppUniqueId()
        {
            if (MetadataSheet != null)
            {
                Excel.Range rng = MetadataSheet.Cells[1, UniqueIdColumn];
                return MetadataSheet.Cells[1, 1].Value2 as string;
            }
            return null;
        }

        public bool FirstLaunch
        {
            get
            {
                try
                {
                    bool? val = MetadataSheet.Cells[Flag.FirstLaunch, FlagsColumn].Value as bool?;
                    return (val.HasValue) ? (bool)val : true;
                }
                catch (Exception)
                {
                    return true;
                }
            }

            set
            {
                MetadataSheet.Cells[Flag.FirstLaunch, FlagsColumn].Value = value.ToString();
            }
        }

        public void WritePicklistData(List<string> picklistValues, PicklistTrackerEntry Tracker)
        {
            Excel.Worksheet sheet = MetadataSheet;
            if (sheet != null)
            {
                Excel.Range column = sheet.Columns[Tracker.AbsoluteColumnNo] as Excel.Range;
                if (column != null)
                {
                    //Tracker.NamedRange = Regex.Replace(Tracker.NamedRange, @"[^a-zA-Z0-9_.]", Constants.UNDERSCORE);
                    Excel.Range cellHeader = column.Cells[1, 1] as Excel.Range;
                    if (cellHeader != null)
                        cellHeader.Value = Tracker.NamedRange;

                    Excel.Range cell1 = ExcelHelper.NextVerticalCell(cellHeader, 1);
                    Excel.Range cell2 = ExcelHelper.NextVerticalCell(cellHeader, picklistValues != null ? picklistValues.Count : 1);
                    Excel.Range columnRange = sheet.Range[cell1, cell2];
                    if (columnRange != null)
                    {
                        object[,] data;
                        if (picklistValues != null)
                        {
                            data = new object[picklistValues.Count, 1];
                            int rowCount = 0;
                            foreach (string picklistValue in picklistValues)
                            {
                                data[rowCount, 0] = picklistValue;
                                ++rowCount;
                            }
                        }
                        else
                        {
                            data = new object[1, 1];
                            data[0, 0] = string.Empty;
                        }
                        columnRange.NumberFormat = "@";
                        columnRange.Value = data;
                        data = null;
                        ExcelHelper.AssignNameToRange(columnRange, Tracker.NamedRange, false);
                    }
                }
            }
        }

        public void CreateNamedRangeForInvalidPicklist()
        {
            Excel.Range oRange = MetadataSheet.Cells[1, DummyPicklistValueColumn];
            oRange.Value2 = Constants.INVALIDPICKLISTDATA_TEXT;
            ExcelHelper.AssignNameToRange(oRange, Constants.INVALIDPICKLISTDATA_RANGENAME, false);
        }

        public void WritePicklistNamedRangePair(object[,] cellData, int rowCount)
        {
            Excel.Worksheet sheet = MetadataSheet;
            if (sheet != null)
            {
                Excel.Range column = sheet.Columns[Constants.STARTCOLUMN_PICKLISTKEYVALUEPAIR] as Excel.Range;
                if (column != null)
                {
                    Excel.Range cellValue = column.Cells[1, 1] as Excel.Range;
                    if (cellValue != null)
                        cellValue.Value = "PickListValue";

                    Excel.Range cellKey = column.Cells[1, 2] as Excel.Range;
                    if (cellKey != null)
                        cellKey.Value = "PickListKey";

                    Excel.Range cell1 = ExcelHelper.NextVerticalCell(cellValue, 1);
                    Excel.Range cell2 = ExcelHelper.NextVerticalCell(cellKey, rowCount);

                    Excel.Range columnRange = sheet.Range[cell1, cell2];
                    columnRange.NumberFormat = "@";
                    if (columnRange != null)
                    {
                        columnRange.Value = cellData;
                        cellData = null;
                        ExcelHelper.AssignNameToRange(columnRange, Constants.NAMEDRANGE_PICKLISTKEYVALUEPAIR, false);
                    }
                }
            }
        }

        public Excel.Range PasteCopiedData(int columns, int rows, Excel.Worksheet currentWorksheet, Excel.XlPasteType pasteType)
        {
            DataManager dataManager = DataManager.GetInstance;
            int pasteStartRow = 10000;

            int startColumnForPaste = dataManager.PicklistTracker.Count > 0
                ? (from pt in dataManager.PicklistTracker
                   select pt.AbsoluteColumnNo).Max() + 1
                : Constants.STARTCOLUMN_PICKLISTDATA;

            // Start at 10,001 - In case a Paste All if a formula references a cell above, 
            // starting the paste so far down ensures most references evaluate without error.
            Excel.Range firstPasteCell = MetadataSheet.Cells[pasteStartRow + 1, startColumnForPaste];
            Excel.Range lastPasteCell = MetadataSheet.Cells[pasteStartRow + rows, startColumnForPaste + columns - 1];
            Excel.Range pasteRange = MetadataSheet.get_Range(firstPasteCell, lastPasteCell);

            switch (pasteType)
            {
                case Microsoft.Office.Interop.Excel.XlPasteType.xlPasteAll:
                    // Range.Select() will work only in the main thread, so Paste with Mapping wont be able to use this.
                    // Activate the Metadata sheet before Select() method.
                    ActivateMetadataSheet();
                    firstPasteCell.Select();
                    MetadataSheet.Paste();
                    currentWorksheet.Activate();
                    HideMetadataSheet();
                    break;
                case Microsoft.Office.Interop.Excel.XlPasteType.xlPasteValues:
                    firstPasteCell.PasteSpecial(pasteType);
                    break;
                default:
                    break;
            }

            return pasteRange;
        }

        private void ActivateMetadataSheet()
        {
            MetadataSheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;
            MetadataSheet.Activate();
        }

        private void HideMetadataSheet()
        {
            MetadataSheet.Visible = Excel.XlSheetVisibility.xlSheetVeryHidden;
        }

        public void ClearPastedData(Excel.Range oRange)
        {
            oRange.Clear();
        }

        public string GetLocalizedFormula(string validationFormula)
        {
            string result = string.Empty;
            ((Excel.Range)MetadataSheet.Cells[1, FormulaLocalizationColumn]).Formula = validationFormula;
            result = ((Excel.Range)MetadataSheet.Cells[1, FormulaLocalizationColumn]).FormulaLocal;
            ((Excel.Range)MetadataSheet.Cells[1, FormulaLocalizationColumn]).Clear();
            return result;
        }
    }
}
