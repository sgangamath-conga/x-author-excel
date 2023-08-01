/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Vbe.Interop;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;
using Microsoft.Office.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class ExcelHelper
    {
        static ConfigurationManager configManager = ConfigurationManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// This method returns the defined Excel name for a single target cell
        /// </summary>
        /// <param name="TargetCell"></param>
        /// <returns></returns>
        public static string GetExcelCellName(Excel.Range TargetCell)
        {
            try
            {
                if (TargetCell.Name != null)
                {
                    Excel.Name name = TargetCell.Name as Excel.Name;
                    if (name.Name != null || name.Name.Length != 0)
                    {
                        return name.Name;
                    }
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the horizontal cell relative to the input cell
        /// </summary>
        /// <param name="Cell"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static Excel.Range NextHorizontalCell(Excel.Range Cell, int cnt)
        {
            Excel.Worksheet osheet = Cell.Worksheet;
            if (Cell.Column + cnt < 1)
                return osheet.Cells[Cell.Row, 1];
            else
                return osheet.Cells[Cell.Row, Cell.Column + cnt];
        }

        public static void RemoveRange(string rngName)
        {
            if (rngName == null) return;
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (wb != null)
            {

                Excel.Worksheet oSheet = wb.ActiveSheet;
                Excel.Range r = oSheet.get_Range(rngName);
                r.Delete();
            }
        }

        public static Excel.Range GetRangeByName(string rngName)
        {
            if (rngName == null) return null;
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (wb != null)
            {

                Excel.Worksheet oSheet = wb.ActiveSheet;
                Excel.Range r = oSheet.get_Range(rngName);
                return r;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetNameAndRange"></param>
        /// <returns></returns>
        public static Excel.Range GetRangeByLocation(string sheetNameAndRange)
        {
            if (string.IsNullOrEmpty(sheetNameAndRange))
                return null;

            string[] splitValues = sheetNameAndRange.Split(Core.Constants.SHEET_DELIMITER.ToCharArray());
            Excel.Worksheet osheet = GetWorkSheet(splitValues[0]);
            return osheet.Range[splitValues[1]];
        }

        /// <summary>
        /// Returns the vertical cell relative to the input cell
        /// </summary>
        /// <param name="Cell"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static Excel.Range NextVerticalCell(Excel.Range Cell, int cnt)
        {
            Excel.Worksheet osheet = Cell.Worksheet;
            if (Cell.Row + cnt < 1)
                return osheet.Cells[1, Cell.Column];
            else
                return osheet.Cells[Cell.Row + cnt, Cell.Column];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Excel.Worksheet GetWorkSheet(string sheetName)
        {
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (wb != null)
            {
                Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                foreach (Excel.Worksheet oSheet in sheets)
                {
                    if (oSheet.Name.Equals(sheetName))
                        return oSheet;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Excel.Worksheet AddWorkSheet(string sheetName)
        {
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Sheets sheets = wb.Sheets as Excel.Sheets;

            Excel.Worksheet newSheet = (Excel.Worksheet)sheets.Add(sheets[3], Type.Missing, Type.Missing, Type.Missing);
            newSheet.Name = sheetName;
            //wb.Save();

            return newSheet;
        }

        /// <summary>
        /// Takes in a cell reference in A1Notation
        /// </summary>
        /// <param name="cellRef"></param>
        /// <returns>bool, true if valid cell reference, false otherwise</returns>
        public static bool IsValidCellReference(string cellRef)
        {
            try
            {
                if (ExcelHelper.GetRangeByLocation(cellRef) == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AssignNameToRange(Excel.Range oRange, string sNamedRange, string comment, bool bApplyBorderFormatting = false)
        {
            AssignNameToRange(oRange, sNamedRange, bApplyBorderFormatting, comment);
        }
        /// <summary>
        /// Creates named range for given range in Excel.
        /// </summary>
        /// <param name="oRange"></param>
        /// <param name="sNamedRange"></param>
        public static void AssignNameToRange(Excel.Range oRange, string sNamedRange, bool bApplyBorderFormatting = false, string comment = "")
        {
            Excel.Workbook m_workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Excel.Name nameRange = m_workbook.Names.Add(sNamedRange, oRange);
            if (!string.IsNullOrEmpty(comment))
                nameRange.Comment = comment;
            // Apply App Builder border style
            if (bApplyBorderFormatting)
            {
                oRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                oRange.Borders.ThemeColor = Excel.XlThemeColor.xlThemeColorAccent5;
                oRange.Borders.TintAndShade = 0;
                oRange.Borders.Weight = Excel.XlBorderWeight.xlHairline;
            }
            //TODO :: For production deliverables, uncomment the below line.
            //nameRange.Visible = bNameRangeVisible;
        }

        /// <summary>
        /// Removes Borders from entire NamedRange.
        /// </summary>
        /// <param name="oRange"></param>
        public static void RemoveBorderFromRange(Excel.Range oRange)
        {
            oRange.Borders[Excel.XlBordersIndex.xlDiagonalDown].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlDiagonalUp].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                oRange.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlLineStyleNone;
        }

        public static Excel.Range GetNamedRangeFromCell(Excel.Range oRange)
        {
            Excel.Range result = null;
            Excel.Range firstCell = null;
            foreach (Excel.Range area in oRange.Areas)
            {
                foreach (Excel.Range cell in area.Cells)
                {
                    firstCell = cell;
                    break;
                }
                break;
            }

            if (firstCell != null)
            {
                List<string> namedRanges = (from r in ConfigurationManager.GetInstance.GetRangeMaps()
                                            //where r.Type == ObjectType.Repeating
                                            select r.RangeName).ToList();

                foreach (string name in namedRanges)
                {
                    //Intersect works within the same worksheet
                    Excel.Range namedRange = GetRange(name);
                    if (namedRange.Worksheet.Name == firstCell.Worksheet.Name)
                    {
                        if (Globals.ThisAddIn.Application.Intersect(namedRange, firstCell) != null) //Target is the single parameter of our handler delegate type.
                        {
                            result = namedRange;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static Excel.Workbook CheckIfFileOpened(string wbook)
        {
            Excel.Workbook workbook = null;
            if (wbook == null) return workbook;
            try
            {
                foreach (Excel.Workbook wb in Globals.ThisAddIn.Application.Workbooks)
                {
                    if (wb.Name.Equals(wbook))
                    {
                        workbook = wb;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

            return workbook;
        }

        public static void CloseWorkbook(Excel.Workbook wb)
        {
            try
            {
                if (wb != null)
                {
                    wb.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

        }
        /// <summary>
        /// Generates a unique Name for Excel Range .
        /// </summary>
        /// <returns></returns>
        public static string CreateUniqueNameRange()
        {
            return new StringBuilder("_").Append(Guid.NewGuid().ToString().Replace('-', '_')).ToString();
        }

        /// <summary>
        /// Removes the specified Named Range from the workbook
        /// </summary>
        /// <param name="sNamedRange"></param>
        public static void RemoveNamedRange(string sNamedRange, bool bClearContents = false)
        {
            if (String.IsNullOrEmpty(sNamedRange))
                return;
            Excel.Workbook m_workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            if (bClearContents)
                ExcelHelper.GetRange(sNamedRange).ClearContents();
            m_workbook.Names.Item(sNamedRange).Delete();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cellBegin"></param>
        ///// <param name="cellEnd"></param>
        //public static void AssignNameToRange(string cellBegin, string cellEnd, string sNamedRange,bool bUpdateExistingNameRange = false, bool bNameRangeVisible = false)
        //{
        //    Excel.Worksheet sheet = GetWorkSheet(cellBegin.Split('!').First());
        //    if (sheet != null)
        //    {
        //        Excel.Workbook m_workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
        //        if (bUpdateExistingNameRange)
        //        {
        //            //Since we are updating the existing named range, delete the Named range first and then assign it.
        //            foreach (Excel.Name name in m_workbook.Names)
        //            {
        //                if (name.Name.Equals(sNamedRange))
        //                {
        //                    name.Delete();
        //                    break;
        //                }
        //            }
        //        }
        //        Excel.Range repeatingCellRange = sheet.Range[cellBegin.Split('!').Last(), cellEnd.Split('!').Last()];
        //        Excel.Name nameRange = m_workbook.Names.Add(sNamedRange, repeatingCellRange);
        //        //TODO :: For production deliverables, uncomment the below line.
        //        //nameRange.Visible = false;   
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="current"></param>
        /// <returns></returns>        
        public static int GetColumnIndex(string sStartRange, string sCurrentRange)
        {
            Excel.Range startRange = ExcelHelper.GetRange(sStartRange);
            Excel.Range currentRange = ExcelHelper.GetRange(sCurrentRange);

            if (startRange != null && currentRange != null)
            {
                if (!startRange.Worksheet.Name.Equals(currentRange.Worksheet.Name))
                    return -2;
                Excel.Worksheet sheet = startRange.Worksheet;
                if (sheet != null)
                    return (currentRange.Column - startRange.Column) + 1;
            }
            return -1;
        }

        //public static int GetColumnIndex(string start, string current)
        //{
        //    Excel.Worksheet sheet = GetWorkSheet(start.Split('!').First());
        //    if (sheet != null)
        //    {
        //        Excel.Range startRange = sheet.Range[start.Split('!').Last()];
        //        Excel.Range currentRange = sheet.Range[current.Split('!').Last()];
        //        return (currentRange.Column - startRange.Column) + 1;
        //    }
        //    return -1;
        //}        

        //public static int GetRowIndex(string start, string current)
        //{
        //    Excel.Worksheet sheet = GetWorkSheet(start.Split('!').First());
        //    if (sheet != null)
        //    {
        //        Excel.Range startRange = sheet.Range[start.Split('!').Last()];
        //        Excel.Range currentRange = sheet.Range[current.Split('!').Last()];
        //        return (currentRange.Row - startRange.Row) + 1;
        //    }
        //    return -1;
        //}

        public static int GetRowIndex(string sStartRange, string sCurrentRange)
        {
            Excel.Range startRange = ExcelHelper.GetRange(sStartRange);
            Excel.Range currentRange = ExcelHelper.GetRange(sCurrentRange);

            if (startRange != null && currentRange != null)
            {

                if (!startRange.Worksheet.Name.Equals(currentRange.Worksheet.Name))
                    return -2;
                Excel.Worksheet sheet = startRange.Worksheet;
                if (sheet != null)
                    return (currentRange.Row - startRange.Row) + 1;
            }
            return -1;
        }

        public static string GetAddress(Excel.Range Target)
        {
            return string.Format("{0}!{1}", Target.Worksheet.Name, Target.get_Address(true, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing));
        }

        public static int GetColumnIndexByTargetNamedRange(string sTargetNamedRange)
        {
            int value = -1;
            if (!string.IsNullOrEmpty(sTargetNamedRange))
                value = GetRange(sTargetNamedRange).Column;
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        public static Excel.Range GetRange(string TargetNamedRange)
        {
            try
            {
                Excel.Workbook m_workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                Excel.Range oRange = m_workbook.Names.Item(TargetNamedRange, System.Type.Missing, System.Type.Missing).RefersToRange;
                return oRange;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int GetRowIndexByTargetNamedRange(string sTargetNamedRange)
        {
            int value = -1;
            if (!string.IsNullOrEmpty(sTargetNamedRange))
                value = ExcelHelper.GetRange(sTargetNamedRange).Row;
            return value;
        }

        public static String GetSheetNameByNamedRange(string sTargetNamedRange)
        {
            if (String.IsNullOrEmpty(sTargetNamedRange))
                return String.Empty;

            return GetRange(sTargetNamedRange).Worksheet.Name;
        }

        /// <summary>
        /// Get Macro list 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMacroList(bool IsVBACodeAllowed = false)
        {
            List<string> macroList = new List<string>();
            vbext_ProcKind prockind = vbext_ProcKind.vbext_pk_Proc;
            string curMacro = string.Empty;
            string newMacro = string.Empty;
            Microsoft.Office.Interop.Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            try
            {
                VBProject pj = wb.Application.ActiveWorkbook.VBProject;
                if (pj.Protection == vbext_ProjectProtection.vbext_pp_none)
                {
                    foreach (VBComponent vbcomp in pj.VBComponents)
                    {
                        if (vbcomp.CodeModule.CountOfLines > 0)
                        {
                            for (int i = 1; i < vbcomp.CodeModule.CountOfLines - 1; i++)
                            {
                                newMacro = vbcomp.CodeModule.get_ProcOfLine(i, out prockind);
                                string str = vbcomp.Name;

                                if (curMacro != newMacro)
                                {
                                    curMacro = newMacro;
                                    if (curMacro != null)
                                        macroList.Add(str + "." + curMacro);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                if (ex.Message.ToString() != "Programmatic access to Visual Basic Project is not trusted" && !IsVBACodeAllowed)
                    MessageBox.Show(resourceManager.GetResource("EXCELHLP_MacroError_ShowMsg"), resourceManager.GetResource("EXCELHLP_MacroErrorCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return macroList;
        }

        public static List<string> GetActiveBookSheetNames()
        {
            List<string> WorkSheets = new List<string>();
            try
            {
                Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
                if (wb != null)
                {
                    Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                    foreach (Excel.Worksheet oSheet in sheets)
                    {
                        if ((!oSheet.Name.Equals("apttusmetadata")) & (oSheet.Visible == Excel.XlSheetVisibility.xlSheetVisible))
                            WorkSheets.Add(oSheet.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return WorkSheets;
        }

        public static Excel.Range GetCurrentSelectedCol()
        {
            Excel.Range r = Globals.ThisAddIn.Application.Selection;

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchFilterGroups"></param>
        public static void CreateCellReferenceNameRanges(List<SearchFilterGroup> searchFilterGroups)
        {
            if (searchFilterGroups != null && searchFilterGroups.Count > 0 && searchFilterGroups[0].Filters != null)
            {
                List<SearchFilter> cellFilters = searchFilterGroups[0].Filters.Where(f => f.ValueType == ExpressionValueTypes.CellReference).ToList();
                if (cellFilters != null && cellFilters.Count > 0)
                {
                    for (int i = 0; i < cellFilters.Count; i++)
                    {
                        Excel.Range oNameRange = ExcelHelper.GetRange(cellFilters[i].SearchFilterLabel);
                        if (oNameRange == null)
                        {
                            string namedRange = GetExcelCellName(oNameRange);
                            Excel.Range cellReferenceRange = GetRangeByLocation(cellFilters[i].SearchFilterLabel);
                            namedRange = GetExcelCellName(cellReferenceRange);
                            if (string.IsNullOrEmpty(namedRange))
                            {
                                namedRange = CreateUniqueNameRange();
                                AssignNameToRange(cellReferenceRange, namedRange);
                            }
                            cellFilters[i].SearchFilterLabel = namedRange;
                        }
                        else
                            cellFilters[i].SearchFilterLabel = cellFilters[i].SearchFilterLabel;

                    }
                }
            }
        }

        public static void ShowGridLines(bool bShowLines = true)
        {
            Globals.ThisAddIn.Application.ActiveWindow.DisplayGridlines = bShowLines;
        }

        public static bool DetectAndExitEditMode()
        {
            bool result = true;
            try
            {
                if (GetEditMode())
                    Globals.ThisAddIn.Application.SendKeys("{ENTER}");
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, false);
                result = false;
            }
            return result;
        }

        public static bool GetEditMode()
        {
            bool result = false;
            CommandBarControl oNewMenu = Globals.ThisAddIn.Application.CommandBars["Worksheet Menu Bar"].FindControl(1, 18, Type.Missing, Type.Missing, true);
            if (oNewMenu != null && !oNewMenu.Enabled)
                result = true;

            return result;
        }

        public static void ApplyBorderToRange(Excel.Range oRange)
        {
            oRange.Borders.LineStyle = Excel.XlLineStyle.xlDash;
            oRange.Borders.ThemeColor = Excel.XlThemeColor.xlThemeColorHyperlink;
            oRange.Borders.TintAndShade = 0;
            oRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
        }

        public static Excel.Shape GetShape(Excel.Worksheet oSheet, string shapeName)
        {
            Excel.Shape shp = null;
            foreach (Excel.Shape shape in oSheet.Shapes)
            {
                if (shape.Name == shapeName)
                {
                    shp = shape;
                    break;
                }
            }
            return shp;
        }

        public static bool FindShape(Excel.Worksheet oSheet, string name)
        {
            bool result = false;
            foreach (Excel.Shape shape in oSheet.Shapes)
            {
                if (shape.Name == name)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="objName"></param>
        public static void DeleteObjectsByNameFromSheet(Excel.Worksheet oSheet, string objName)
        {
            dynamic OLE = oSheet.OLEObjects(Type.Missing);

            //foreach (dynamic obj in oSheet.OLEObjects)
            for (int i = 0; i < OLE.Count; i++)
            {
                dynamic obj = OLE[i + 1];
                string sName = obj.Name;

                if (sName.Equals(objName))
                    oSheet.OLEObjects(obj.Index).Delete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="filePath"></param>
        public static void AddObjectToSheet(Excel.Worksheet oSheet, string filePath, string uniqueId)
        {
            string shapeName = uniqueId;
            try
            {
                Excel.Shape shp = oSheet.Shapes.AddOLEObject(Filename: filePath, DisplayAsIcon: true, IconFileName: uniqueId, IconLabel: uniqueId);
                try
                {
                    shp.Name = shapeName; //ShapeName length cannot be more than 32 characters.
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToString().Contains("Access is denied."))
                    {
                        DeleteObjectsByNameFromSheet(oSheet, shapeName);
                        shp.Name = shapeName;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// User Protection sheet
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="bProtect"></param>
        public static void UserSheetProtection(Excel.Worksheet oSheet, bool bProtect)
        {
            if (configManager.Definition.AppSettings != null)
            {
                // Case of user sheet 
                SheetProtect sheetProtect = configManager.Definition.AppSettings.SheetProtectSettings.Where(s => s.SheetName == oSheet.Name).SingleOrDefault();
                if (sheetProtect != null)
                {
                    if (bProtect)
                    {
                        //CellUnlock(oSheet);
                        oSheet.Protect(sheetProtect.Password,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        oSheet.Protection.AllowFormattingCells, oSheet.Protection.AllowFormattingColumns, oSheet.Protection.AllowFormattingRows,
                        oSheet.Protection.AllowInsertingColumns, oSheet.Protection.AllowInsertingRows, oSheet.Protection.AllowInsertingHyperlinks,
                        oSheet.Protection.AllowDeletingColumns, oSheet.Protection.AllowDeletingRows, oSheet.Protection.AllowSorting, oSheet.Protection.AllowFiltering,
                        oSheet.Protection.AllowUsingPivotTables);
                    }
                    else
                    {
                        oSheet.Unprotect(sheetProtect.Password);
                    }
                }
            }
        }
        public static bool IsUserSheetProtection(Excel.Worksheet oSheet)
        {
            bool blnResult = false;
            if (configManager.Definition.AppSettings != null)
            {
                // Case of user sheet 
                SheetProtect sheetProtect = configManager.Definition.AppSettings.SheetProtectSettings.Where(s => s.SheetName == oSheet.Name).SingleOrDefault();
                if (sheetProtect != null)
                {
                    blnResult = true;
                }
            }
            return blnResult;
        }

        public static bool ValidateSheetForPasswordMismatch(out List<string> lstSheetFailed)
        {
            List<string> lstSheetChecked = new List<string>();
            return ValidateSheetForPasswordMismatch(out lstSheetFailed, out lstSheetChecked);
        }

        public static bool ValidateSheetForPasswordMismatch(out List<string> lstSheetFailed, out List<string> lstSheetChecked)
        {
            //Password mismatch Check
            lstSheetFailed = new List<string>();
            lstSheetChecked = new List<string>();
            bool blnProtectSheetValidation = false;
            bool protectSheetSuccess = true;
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            List<string> resultSheet = configManager.GetSheetNames();

            if (wb != null)
            {
                Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                foreach (Excel.Worksheet oSheet in sheets)
                {
                    if (!oSheet.Name.Equals(Constants.METADATA_SHEETNAME) && oSheet.ProtectContents && resultSheet.Contains(oSheet.Name))
                    {
                        lstSheetChecked.Add(oSheet.Name);

                        string password = (from config in configManager.Definition.AppSettings.SheetProtectSettings
                                           where config.SheetName == oSheet.Name
                                           select config.Password).FirstOrDefault();
                        try
                        {
                            ExcelHelper.UserSheetProtection(oSheet, false);
                        }
                        catch (Exception ex)
                        {
                            lstSheetFailed.Add(oSheet.Name);
                            blnProtectSheetValidation = true;
                            protectSheetSuccess = false;
                        }
                        if (protectSheetSuccess)
                            ExcelHelper.UserSheetProtection(oSheet, true);
                    }
                    protectSheetSuccess = true;
                }
            }

            return blnProtectSheetValidation;
        }

        public static void DeleteWorksheet(string ObjectName)
        {
            try
            {
                Globals.ThisAddIn.Application.DisplayAlerts = false;
                Excel.Worksheet worksheet = ExcelHelper.GetWorkSheet(ObjectName);
                if (worksheet != null)
                    worksheet.Delete();
            }
            finally
            {
                Globals.ThisAddIn.Application.DisplayAlerts = true;

            }
        }
        public static void CheckAppSettings(object sender, AppSettingsEventArgs appSettingsUI)
        {
            AppSettings AppSettingsModel = configManager.Definition.AppSettings;
            if (AppSettingsModel == null && configManager.Application != null)
            {
                Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
                List<string> resultSheet = configManager.GetSheetNames();
                if (wb != null)
                {
                    Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                    foreach (Excel.Worksheet oSheet in sheets)
                    {
                        if (!oSheet.Name.Equals(Constants.METADATA_SHEETNAME) && oSheet.ProtectContents && resultSheet.Contains(oSheet.Name))
                        {
                            SheetProtect sheetProtect = new SheetProtect
                            {
                                SheetName = oSheet.Name,
                                Password = string.Empty
                            };
                            AppSettingsModel.SheetProtectSettings.Add(sheetProtect);
                        }
                    }
                    appSettingsUI.xaeAppSettings.SetDataSource(AppSettingsModel.SheetProtectSettings);

                }
            }
            else
            {

                List<SheetProtect> lstClone = new List<SheetProtect>();
                List<string> lstSheet = new List<string>();
                configManager.Definition.AppSettings.SheetProtectSettings.ForEach((item) => { lstClone.Add(item.Clone()); });
                AppSettingsModel.SheetProtectSettings.Clear();
                Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
                List<string> resultSheet = configManager.GetSheetNames();

                if (wb != null)
                {
                    Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                    foreach (Excel.Worksheet oSheet in sheets)
                    {
                        if (!oSheet.Name.Equals(Constants.METADATA_SHEETNAME) && oSheet.ProtectContents && resultSheet.Contains(oSheet.Name))
                        {
                            if (!lstClone.Exists(e => e.SheetName.Equals(oSheet.Name)))
                            {
                                SheetProtect sheetProtect = new SheetProtect
                                {
                                    SheetName = oSheet.Name,
                                    Password = string.Empty
                                };
                                AppSettingsModel.SheetProtectSettings.Add(sheetProtect);
                            }
                        }
                        else
                        {
                            if (lstClone.Exists(e => e.SheetName.Equals(oSheet.Name)))
                                lstClone.RemoveAll(x => x.SheetName == oSheet.Name);
                        }
                        lstSheet.Add(oSheet.Name);
                    }
                }

                for (int i = 0; i < lstClone.Count; i++)
                {
                    if (!lstSheet.Exists(e => e.Equals(lstClone[i].SheetName)))
                        lstClone.RemoveAll(x => x.SheetName == lstClone[i].SheetName);
                }
                var dictProtectSettingModel = AppSettingsModel.SheetProtectSettings.ToDictionary(p => p.SheetName);
                foreach (var sheetProtect in lstClone)
                {
                    dictProtectSettingModel[sheetProtect.SheetName] = sheetProtect;
                }
                AppSettingsModel.SheetProtectSettings = dictProtectSettingModel.Values.ToList();
                appSettingsUI.xaeAppSettings.SetDataSource(AppSettingsModel.SheetProtectSettings);
            }
        }
        public static void ValidateSheetProtection(object sender, AppSettingsEventArgs appSettingsUI)
        {
            List<string> lstSheet = new List<string>();
            bool blnProtectSheetValidation = ValidateSheetForPasswordMismatch(out lstSheet);
            appSettingsUI.xaeAppSettings.ValidateSheetProtection(blnProtectSheetValidation, lstSheet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetNameAndRange"></param>
        /// <returns></returns>
        public static bool TryGetRangeByLocation(string sheetNameAndRange, out Excel.Range range)
        {
            bool result = true;
            range = null;
            try
            {
                range = GetRangeByLocation(sheetNameAndRange);
                result = range != null;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public static string GetAppUniqueId()
        {
            string appUniqueId = string.Empty;
            Excel.Worksheet oSheet = GetWorkSheet(Constants.METADATA_SHEETNAME);
            if (oSheet != null)
            {
                Excel.Range rng = oSheet.Cells[1, 1];
                appUniqueId = oSheet.Cells[1, 1].Value2 as string;
            }
            return appUniqueId;
        }
    }
}
