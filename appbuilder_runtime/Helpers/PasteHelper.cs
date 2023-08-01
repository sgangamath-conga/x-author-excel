/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apttus.XAuthor.Core;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    class PasteHelper
    {
        private WaitWindowView waitForm;
        private ConfigurationManager configManager;
        private Excel.Application ExcelApp = null;
        private string sourceFilePath = string.Empty;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public PasteHelper()
        {
            configManager = ConfigurationManager.GetInstance;
        }

        public string SourceFile
        {
            get
            {
                return sourceFilePath;
            }
            set
            {
                sourceFilePath = value;
            }
        }

        public void StartPasteMapping(bool bInputFileName = true)
        {
            //If there is no mapping for this app, then just return.
            DataTransferMapping model = configManager.Application.Definition.Mapping;
            if (model == null)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("PASTEHELPER_StartPasteMapping_InfoMsg"), resourceManager.GetResource("PASTEHELPER_StartPasteMappingCAP_InfoMsg"));
                return;
            }

            if (bInputFileName)
            {
                FileInputDialog dialog = new FileInputDialog();
                dialog.ShowDialog();

                sourceFilePath = dialog.FilePath;
                if (String.IsNullOrEmpty(sourceFilePath))
                    return;
            }

            WorkbookEventManager.GetInstance.UnsubscribeEvents();
            try
            {
                Thread thread = new Thread(new ThreadStart(DoWork));

                //If a worker thread invokes COM Call, the worker thread's ApartmentState should be STA, otherwise all doom, everything fails.
                thread.SetApartmentState(ApartmentState.STA);

                //Create the Wait Form
                waitForm = new WaitWindowView();
                waitForm.StartPosition = FormStartPosition.CenterParent;

                //Start the Thread
                thread.Start();

                //Show the Wait(Processing...) Dialog
                waitForm.ShowDialog();
            }
            finally
            {
                WorkbookEventManager.GetInstance.SubscribeEvents();
                Globals.ThisAddIn.Application.ActiveWorkbook.ActiveSheet.Cells[1, 1].Select();
            }
        }

        [STAThread]
        private void DoWork()
        {
            try
            {
                PasteFromMapping();
            }
            catch (Exception ex)
            {
            }
            //Globals.ThisAddIn.Application.ScreenUpdating = true;
        }

        private void updateProgessMessage(string message)
        {
            waitForm.Message = message;
        }

        private void PasteFromMapping()
        {
            ExcelHelper.ExcelApp.ScreenUpdating = false;
            Excel.Workbook sourceBook = null;
            List<string> failureMessages = new List<string>();
            Dictionary<string, List<Excel.Name>> namedRangesPerSheet = null;
            try
            {

                DataTransferMapping model = configManager.Application.Definition.Mapping;

                updateProgessMessage(resourceManager.GetResource("PASTEHELPER_ProgressOpen_Msg"));
                sourceBook = LoadSourceWorkbook(model, sourceFilePath);
                if (sourceBook == null)
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("PASTEHELPER_SourceFile_ErrorMsg"), resourceManager.GetResource("PASTEHELPER_SourceFileCap_ErrorMsg"));
                    return;
                }

                Dictionary<string, Excel.Worksheet> sourceSheets = ValidateSourceFile(model, sourceBook);
                if (sourceSheets == null)
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("PASTEHELPER_NotMatchDesFL_ErrorMsg"), resourceManager.GetResource("PASTEHELPER_NotMatchDesFLCap_ErrorMsg"));
                    return;
                }

                updateProgessMessage(resourceManager.GetResource("PASTEHELPER_ProgressRead_Msg"));
                namedRangesPerSheet = FilterSourceBookNamedRangesPerSheet(sourceBook);

                foreach (DataTransferRange dataTransferRange in model.DataTransferRanges)
                {
                    Excel.Worksheet sourceSheet = sourceSheets[dataTransferRange.SourceSheet];
                    Excel.Range sourceCell = sourceSheet.Range[dataTransferRange.SourceRange];

                    Excel.Worksheet destSheet = Globals.ThisAddIn.Application.ActiveWorkbook.Sheets[dataTransferRange.TargetSheet];
                    Excel.Range destCell = destSheet.Range[dataTransferRange.TargetRange];

                    try
                    {
                        //Source sheet - from where the data is to be copied.
                        updateProgessMessage(String.Format(resourceManager.GetResource("PASTEHELPER_PasteFromMappingProcess_InfoMsg"), destSheet.Name));
                        Excel.Range sourceNameRange = GetSourceIntersectingNamedRange(sourceSheet, namedRangesPerSheet, sourceCell);

                        if (sourceNameRange == null)
                        {
                            string msg = string.Format(resourceManager.GetResource("PASTEHELPER_PasteFromMappingSource_InfoMsg"), sourceSheet.Name, dataTransferRange.SourceRange);
                            failureMessages.Add(msg);
                            //updateProgessMessage("Data Loading Failed For " + sourceSheet.Name + " Sheet");
                            continue;
                        }

                        //Destination Sheet - which is the running app in runtime.
                        destCell = ExcelHelper.NextVerticalCell(destCell, -2);
                        if (destCell == null)
                            continue;

                        Excel.Range destNameRange = ExcelHelper.GetNamedRangeFromCell(destCell);
                        if (destNameRange == null)
                        {
                            string msg = string.Format(resourceManager.GetResource("PASTEHELPER_PasteFromMappingDest_InfoMsg"), destSheet.Name);
                            failureMessages.Add(msg);
                            //updateProgessMessage("Data Loading Failed For " + destSheet.Name + " Sheet");
                            continue;
                        }

                        //First 2 rows in sourceNameRange are our designer Label and Formula rows, hence we will skip them, so that we have the actual data only.
                        int nRowsToSkip = 2;

                        if (destNameRange.Rows.Count > nRowsToSkip)
                            ClearPreviousMappings(destSheet, destNameRange);

                        updateProgessMessage(String.Format(resourceManager.GetResource("PASTEHELPER_PasteFromMappingTransfer_InfoMsg"), destSheet.Name));

                        Excel.Range startCell = ExcelHelper.NextVerticalCell(sourceNameRange.Cells[1, 1], 2);
                        Excel.Range endCell = sourceNameRange.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
                        Excel.Range sourceDataRange = sourceNameRange.Worksheet.Range[startCell, endCell];

                        sourceDataRange.Copy();

                        RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(destCell, destNameRange);
                        runtimeEditActionController.Paste(sourceDataRange.Rows.Count, sourceDataRange.Columns.Count, Excel.XlPasteType.xlPasteValues);

                        Marshal.ReleaseComObject(sourceDataRange);
                        Marshal.ReleaseComObject(endCell);
                        Marshal.ReleaseComObject(startCell);
                        Marshal.ReleaseComObject(sourceNameRange);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(sourceCell);
                        Marshal.ReleaseComObject(sourceSheet);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                ExcelHelper.ExcelApp.ScreenUpdating = true;
                waitForm.CloseWaitWindow();
                ClearAllNamedRanges(namedRangesPerSheet);
                namedRangesPerSheet = null;
                CloseSourceBook(sourceBook);
                ReleaseExcel();
                if (failureMessages.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string msg in failureMessages)
                        sb.Append(msg).Append(Environment.NewLine);
                    ApttusMessageUtil.Show(resourceManager.GetResource("PASTEHELPER_PasteMap_ShowMsg"), resourceManager.GetResource("PASTEHELPER_PasteMapCap_ShowMsg"), resourceManager.GetResource("PASTEHELPER_PasteMapInstr_ShowMsg"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok, sb.ToString());
                }

            }
        }

        private void ClearAllNamedRanges(Dictionary<string, List<Excel.Name>> namedRangesPerSheet)
        {
            if (namedRangesPerSheet == null)
                return;
            foreach (KeyValuePair<string, List<Excel.Name>> pairs in namedRangesPerSheet)
            {
                List<Excel.Name> names = namedRangesPerSheet[pairs.Key];
                foreach (Excel.Name name in names)
                {
                    Marshal.ReleaseComObject(name);
                }
            }
            namedRangesPerSheet.Clear();
        }

        private void ClearPreviousMappings(Excel.Worksheet destSheet, Excel.Range destNameRange)
        {
            updateProgessMessage(resourceManager.GetResource("PASTEHELP_ClearPreviousMappings_InfoMsg"));

            string dNamedRange = (destNameRange.Name as Excel.Name).Name;
            ExcelHelper.UpdateSheetProtection(destSheet, false);
            RepeatingGroup repGroup = configManager.GetRepeatingGroupbyTargetNamedRange(dNamedRange);

            ExcelHelper.ClearRepeatingCells(repGroup.TargetNamedRange, repGroup.AppObject, false);
            ExcelHelper.UpdateSheetProtection(destSheet, true);

            Guid datasetId = (from dt in DataManager.GetInstance.AppDataTracker
                              where dt.Location == dNamedRange && dt.Type == ObjectType.Repeating
                              select dt.DataSetId).FirstOrDefault();

            ApttusDataSet dataSet = DataManager.GetInstance.GetDataByDataSetId(datasetId);
            DataManager.GetInstance.RemoveDataSet(dataSet);
        }

        private void CloseSourceBook(Excel.Workbook sourceBook)
        {
            if (sourceBook == null)
                return;
            sourceBook.Application.CutCopyMode = Excel.XlCutCopyMode.xlCut;
            sourceBook.Close(false, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sourceBook);
            sourceBook = null;
        }

        private void ReleaseExcel()
        {
            try
            {
                // Clean up Excel application
                if (ExcelApp != null)
                {
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
                    ExcelApp = null;
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                // File may be locked so manual clean up will be required. Log error.
                ExceptionLogHelper.ErrorLog(ex, false);
            }

        }
        private Excel.Workbook LoadSourceWorkbook(DataTransferMapping model, string sourceFilePath)
        {
            ExcelApp = new Excel.Application();
            Excel.Workbook workbook = null;
            try
            {
                ExcelApp.Visible = false;
                ExcelApp.EnableEvents = false;
                Excel.Workbooks books = ExcelApp.Workbooks;
                workbook = books.Open(sourceFilePath, Type.Missing, true);
                Marshal.ReleaseComObject(books);
            }
            catch (Exception)
            {
                CloseSourceBook(workbook);
                ReleaseExcel();
            }
            return workbook;
        }

        private Dictionary<string, Excel.Worksheet> ValidateSourceFile(DataTransferMapping model, Excel.Workbook sourceWorkbook)
        {
            Dictionary<string, Excel.Worksheet> sheets = new Dictionary<string, Excel.Worksheet>();
            foreach (DataTransferRange rg in model.DataTransferRanges)
            {
                try
                {
                    //Validate whether the input workbook provided by the user contains all the sheets specified at design time or not. If even a single sheet
                    //doesn't exist it's a failure. it's all or none.
                    Excel.Worksheet sheet = sourceWorkbook.Sheets[rg.SourceSheet];
                    sheets[sheet.Name] = sheet;
                    //bFileOK = true;
                    //Marshal.ReleaseComObject(sheet);
                }
                catch (Exception ex)
                {
                    sheets.Clear();
                    sheets = null;
                    break;
                }
            }
            return sheets;
        }

        private Dictionary<string, List<Excel.Name>> FilterSourceBookNamedRangesPerSheet(Excel.Workbook sourceBook)
        {
            Dictionary<string, List<Excel.Name>> nameRangesPerSheet = new Dictionary<string, List<Excel.Name>>();
            Excel.Application app = sourceBook.Application;
            try
            {
                foreach (Excel.Name name in sourceBook.Names)
                {
                    if (!ExcelHelper.IsValidName(name))
                        continue;

                    Excel.Range rg;
                    try
                    {
                        rg = name.RefersToRange;
                        if (rg == null)
                            continue;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    Excel.Worksheet sheet = rg.Worksheet;

                    try
                    {
                        if (sheet.Visible != Excel.XlSheetVisibility.xlSheetVisible)
                            continue;

                        if (nameRangesPerSheet.ContainsKey(sheet.Name))
                        {
                            List<Excel.Name> names = nameRangesPerSheet[sheet.Name];
                            names.Add(name);
                        }
                        else
                        {
                            List<Excel.Name> names = new List<Excel.Name>();
                            names.Add(name);
                            nameRangesPerSheet.Add(sheet.Name, names);
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(sheet);
                        Marshal.ReleaseComObject(rg);
                    }
                }
            }

            finally
            {
                Marshal.ReleaseComObject(app);
            }
            return nameRangesPerSheet;
        }

        private Excel.Range GetSourceIntersectingNamedRange(Excel.Worksheet sheet, Dictionary<string, List<Excel.Name>> namedRangesPerSheet, Excel.Range cell)
        {
            List<Excel.Name> names = namedRangesPerSheet[sheet.Name];
            Excel.Application app = sheet.Application;
            try
            {
                foreach (Excel.Name name in names)
                {
                    if (app.Intersect(name.RefersToRange, cell) != null)
                        return name.RefersToRange;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(app);
            }
            return null;
        }
    }
}
