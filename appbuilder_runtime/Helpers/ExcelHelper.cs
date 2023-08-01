/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Core = Apttus.XAuthor.Core;
using System.Globalization;
using System.Threading;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using System.Text.RegularExpressions;

namespace Apttus.XAuthor.AppRuntime
{
    public class ExcelHelper
    {
        static ConfigurationManager configManager = ConfigurationManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private static Excel.Application excelApp;
        private static Version excelVersion;
        private static Regex pickListValidation = new Regex(@"[^a-zA-Z0-9_.]");
        public static Excel.Application ExcelApp
        {
            get
            {
                if (excelApp == null && Globals.ThisAddIn != null)
                    excelApp = Globals.ThisAddIn.Application;

                return excelApp;
            }
            set
            {
                excelApp = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Excel.Worksheet GetWorkSheet(string sheetName)
        {
            Excel.Workbook wb = excelApp.ActiveWorkbook;

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
        /// <param name="wb"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Excel.Worksheet GetWorkSheet(Excel.Workbook wb, string sheetName)
        {
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

        public static string GetActiveWorksheetName()
        {
            return excelApp.ActiveWorkbook.Path + Path.DirectorySeparatorChar + excelApp.ActiveWorkbook.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static Excel.Worksheet AddWorkSheet(string sheetName)
        {

            Excel.Workbook wb = excelApp.ActiveWorkbook;

            Excel.Sheets sheets = wb.Sheets as Excel.Sheets;

            Excel.Worksheet newSheet = (Excel.Worksheet)sheets.Add(sheets[3], Type.Missing, Type.Missing, Type.Missing);
            newSheet.Name = sheetName;
            //wb.Save();

            return newSheet;
        }
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
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
        /// Creates named range for given range in Excel.
        /// </summary>
        /// <param name="oRange"></param>
        /// <param name="sNamedRange"></param>
        public static void AssignNameToRange(Excel.Range oRange, string sNamedRange, bool bApplyBorderFormatting = true)
        {
            Excel.Workbook m_workbook = excelApp.ActiveWorkbook;
            Excel.Name nameRange = m_workbook.Names.Add(sNamedRange, oRange);

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
        /// 
        /// </summary>
        /// <param name="SheetNameAndRange"></param>
        /// <returns></returns>
        public static string GetSingleCellValue(string SheetNameAndRange)
        {
            string value = "";
            if (SheetNameAndRange != null)
            {
                string[] splitValues = SheetNameAndRange.Split(Core.Constants.SHEET_DELIMITER.ToCharArray());
                Excel.Worksheet osheet = GetWorkSheet(splitValues[0]);
                return Convert.ToString(osheet.Range[splitValues[1]].Value2);
            }
            return value;
        }

        public static Excel.Range GetRange(string TargetNamedRange)
        {
            try
            {
                Excel.Workbook m_workbook = excelApp.ActiveWorkbook;
                Excel.Range oRange = m_workbook.Names.Item(TargetNamedRange, System.Type.Missing, System.Type.Missing).RefersToRange;
                return oRange;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// This function checks whehter selection has intersecting X-Author range or not based on firstcell.
        /// Function is extended by adding Additional parameter of getOverlappingRange, which intersects entire range (rather than firstcell)
        /// if intersecting X-Author range is found return that range
        /// </summary>
        /// <param name="oRange"></param>
        /// <param name="getOverlappingRange"></param>
        /// <returns></returns>
        public static Excel.Range GetNamedRangeFromCell(Excel.Range oRange, bool getOverlappingRange = false)
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

            if (firstCell != null && ConfigurationManager.GetInstance.Application != null)
            {

                //TODO : Retrieving namedranges can be done using method GetNamedRanges, it will be efficient as it will categorize namedranges per worksheet.
                List<string> namedRanges = (from r in ConfigurationManager.GetInstance.GetRangeMaps()
                                                //where r.Type == ObjectType.Repeating
                                            select r.RangeName).ToList();

                foreach (string name in namedRanges)
                {
                    //Intersect works within the same worksheet
                    Excel.Range namedRange = GetRange(name);
                    if (namedRange != null && (namedRange.Worksheet.Name == firstCell.Worksheet.Name))
                    {
                        if (excelApp.Intersect(namedRange, firstCell) != null) //Target is the single parameter of our handler delegate type.
                        {
                            result = namedRange;
                            break;
                        }
                    }
                }
            }

            if (result == null && getOverlappingRange)
            {
                string overlappingNamedRange = GetOverlapingNamedRange(oRange, getOverlappingRange).FirstOrDefault();
                result = GetRange(overlappingNamedRange);
            }
            return result;
        }

        /// <summary>
        /// This method will check if any named range exists in bottom or Right
        /// </summary>
        /// <param name="oRange">Selected Cell</param>
        /// <returns></returns>
        public static bool DoesNamedRangeExistsInBottomOrRight(Excel.Range oRange)
        {
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

            if (firstCell != null && ConfigurationManager.GetInstance.Application != null)
            {
                List<string> namedRanges = GetRepeatingGroupRangesPerSheet(oRange.Worksheet);

                if (namedRanges == null)
                    return false;

                foreach (string name in namedRanges)
                {
                    //Intersect works within the same worksheet
                    Excel.Range namedRange = GetRange(name);
                    if (namedRange != null && (namedRange.Worksheet.Name == firstCell.Worksheet.Name))
                    {
                        RepeatingGroup repeatingGroup = ConfigurationManager.GetInstance.GetRepeatingGroupbyTargetNamedRange(name);
                        if ((repeatingGroup != null && repeatingGroup.Layout == "Vertical")
                            && (firstCell.Row < namedRange.Row) && ((firstCell.Column < (namedRange.Column + namedRange.Columns.Count)) && (firstCell.Column >= namedRange.Column)))
                            return true;

                        if ((repeatingGroup != null && repeatingGroup.Layout == "Horizontal")
                            && (firstCell.Column < namedRange.Column) && ((firstCell.Row < (namedRange.Row + namedRange.Rows.Count)) && (firstCell.Row >= namedRange.Row)))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// get NamedRanges of actie worksheet & Populate NamedRangePerWorkSheet if its not populated.
        /// </summary>
        /// <param name="oRange"></param>
        /// <returns></returns>
        internal static List<string> GetRepeatingGroupRangesPerSheet(Excel.Worksheet oSheet)
        {
            List<string> rangeMaps = null;
            List<string> repeatingGroupRangesPerSheet = null;

            // Get application Unique Id
            MetadataManager metadataManager = MetadataManager.GetInstance;
            string appUniqueId = metadataManager.GetAppUniqueId();

            // Get Application instance
            //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
            ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);

            if (appInstance != null && appInstance.RepeatingGroupRangesPerWorkSheet != null)
            {
                if (appInstance.RepeatingGroupRangesPerWorkSheet.Count == 0)
                {
                    //rangeMaps = (from r in ConfigurationManager.GetInstance.GetRangeMaps()
                    //               select r.RangeName).ToList();

                    rangeMaps = ConfigurationManager.GetInstance.GetAppNamedRanges();

                    List<string> repeatingGroupRanges;

                    foreach (string targetNameRange in rangeMaps)
                    {
                        Excel.Range range = GetRange(targetNameRange);
                        Excel.Worksheet workSheet = range.Worksheet;

                        if (appInstance.RepeatingGroupRangesPerWorkSheet.ContainsKey(workSheet))
                        {
                            repeatingGroupRanges = appInstance.RepeatingGroupRangesPerWorkSheet[workSheet];
                            repeatingGroupRanges.Add(targetNameRange);
                            appInstance.RepeatingGroupRangesPerWorkSheet[workSheet] = repeatingGroupRanges;
                        }
                        else
                        {
                            repeatingGroupRanges = new List<string>();
                            repeatingGroupRanges.Add(targetNameRange);
                            appInstance.RepeatingGroupRangesPerWorkSheet.Add(workSheet, repeatingGroupRanges);
                        }
                    }
                }

                if (appInstance.RepeatingGroupRangesPerWorkSheet.ContainsKey(oSheet))
                    repeatingGroupRangesPerSheet = appInstance.RepeatingGroupRangesPerWorkSheet[oSheet];
            }

            return repeatingGroupRangesPerSheet;
        }

        public static List<string> GetOverlapingNamedRange(Excel.Range selectedRange, bool getOverlappingRange = false)
        {
            List<string> result = new List<string>();
            HashSet<string> namedRanges = new HashSet<string>(from r in ConfigurationManager.GetInstance.GetRangeMaps()
                                                              where r.Type == ObjectType.Repeating
                                                              select r.RangeName);

            foreach (string name in namedRanges)
            {
                //Intersect works within the same worksheet
                Excel.Range namedRange = GetRange(name);
                if (namedRange.Worksheet.Name == selectedRange.Worksheet.Name)
                {
                    if (excelApp.Intersect(namedRange, selectedRange) != null)
                    {
                        result.Add(name);
                    }
                    else if (getOverlappingRange)
                    {
                        // Get number of rows in datatable for this Named range and compare it against Excel
                        int rowsInDataTable = DataManager.GetInstance.GetDataSetFromLocation(name) != null ? DataManager.GetInstance.GetDataSetFromLocation(name).DataTable.Rows.Count : 0;
                        // Do +2 in datatable rows to account for Header and Hidden formula row
                        if (namedRange.Rows.Count != (rowsInDataTable + 2))
                            result.Add(name);
                    }
                }
            }
            return result;
        }

        public static void PopulateCell(Excel.Range TargetNamedRange, ApttusField apttusField, object Value, bool AssignValue, CellValidationInput cellValidationInput)
        {

            // Apply Data Validation
            AddCellRangeValidation(TargetNamedRange, apttusField, cellValidationInput);

            // Assign Value to the Range
            if (AssignValue)
            {
                switch (apttusField.Datatype)
                {
                    case Datatype.Boolean:
                    case Datatype.Date:
                    case Datatype.DateTime:
                    case Datatype.Double:
                    case Datatype.Decimal:
                    case Datatype.Picklist_MultiSelect:
                    case Datatype.Picklist:
                    case Datatype.String:
                    case Datatype.Textarea:
                    case Datatype.Attachment:
                    case Datatype.Formula:
                    case Datatype.Email:
                    case Datatype.NotSupported:
                    case Datatype.Rich_Textarea:
                        TargetNamedRange.Value2 = Value;
                        break;
                    case Datatype.Lookup:
                    case Datatype.Composite:
                        TargetNamedRange.FormulaR1C1 = Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public static string GetAddress(Excel.Range Target)
        {
            return string.Format("{0}!{1}", Target.Worksheet.Name, Target.get_Address(true, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing));
        }

        private static Datatype GetFieldDataType(ApttusField apttusField)
        {
            Datatype dataType = apttusField.Datatype;

            // DataType - Use Lookup validation for Id field as well
            if (apttusField.Id.Equals(Constants.ID_ATTRIBUTE))
                dataType = Datatype.Lookup;
            else if (dataType == Datatype.Formula)
            {
                switch (apttusField.FormulaType)
                {
                    case Constants.FORMULATYPECURRENCY:
                        dataType = Datatype.Decimal;
                        break;
                    case Constants.FORMULATYPEDOUBLE:
                        dataType = Datatype.Double;
                        break;
                }
            }
            return dataType;
        }

        /// <summary>
        /// Excel helper function to assign data validation to an Excel cell for a valid date
        /// </summary>
        /// <param name="oSheet"></param>
        public static void AddCellRangeValidation(Excel.Range TargetNamedRange, ApttusField apttusField, CellValidationInput cellValidationInput)
        {
            bool bShowError = true;
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            MetadataManager metadataManager = MetadataManager.GetInstance;
            bool bApplyNumberFormat = false;
            bool bApplyDataValidation = false;

            Datatype dataType = GetFieldDataType(apttusField);

            cellValidationInput.culterInfo = Utils.GetLatestCulture;

            // Number Format - Apply Number Format if existing format doesn't exist
            if (Convert.ToString(TargetNamedRange.NumberFormat) == Constants.EXCEL_DEFAULTNUMBERFORMAT)
                bApplyNumberFormat = true;

            // Data Validation - Since there is not straight forward way to identify, whether cell has data validation or not, a try catch block is added.
            // Here we are accessing validation.formula1 property, if data validation exists it will return string value, if not throws an COM exception.
            try
            {
                if (!string.IsNullOrEmpty(TargetNamedRange.Validation.Formula1))
                    bApplyDataValidation = false;
            }
            catch (Exception)
            {
                // Cell Validation doesn't exist, Apply Data Validation
                bApplyDataValidation = true;
            }

            try
            {

                switch (dataType)
                {
                    case Datatype.Boolean:
                        // Number Format
                        //if (bApplyNumberFormat)
                        //{
                        //    TargetNamedRange.NumberFormat = "General";
                        //}

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            // List seperator in Culture info can never be empty of null
                            string listSeperator = Utils.GetLatestCulture.TextInfo.ListSeparator;
                            string TrueFalseString = "TRUE" + listSeperator + "FALSE";
                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, TrueFalseString);
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationBool_InfoMsg");
                        }
                        break;
                    case Datatype.Date:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            TargetNamedRange.NumberFormat = cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern;
                        }

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            DateTime dtStartDate = new DateTime(1900, 1, 1);
                            DateTime dtEndDate = new DateTime(4000, 12, 31);
                            string StartDate = String.Format("{0:" + cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern + "}", dtStartDate);
                            string EndDate = String.Format("{0:" + cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern + "}", dtEndDate);

                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDate, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, StartDate, EndDate);
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationDate_InfoMsg");
                        }
                        break;
                    case Datatype.DateTime:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            //TargetNamedRange.NumberFormat = cellValidationInput.culterInfo.DateTimeFormat.FullDateTimePattern;
                            string dateTimeFormat = cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern + Constants.SPACE +
                                "h" + cellValidationInput.culterInfo.DateTimeFormat.TimeSeparator + "mm" +
                                Constants.SPACE + "AM/PM";
                            TargetNamedRange.NumberFormat = dateTimeFormat;
                        }

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            DateTime dtStartDate = new DateTime(1900, 1, 1);
                            DateTime dtEndDate = new DateTime(4000, 12, 31);
                            string StartDate = String.Format("{0:" + cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern + "}", dtStartDate);
                            string EndDate = String.Format("{0:" + cellValidationInput.culterInfo.DateTimeFormat.ShortDatePattern + "}", dtEndDate);

                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDate, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, StartDate, EndDate);
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationDateTime_InfoMsg");
                        }
                        break;
                    case Datatype.Double:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            //TargetNamedRange.NumberFormat = "_(* #,##0.00_);_(* (#,##0.00);_(@_)";
                            TargetNamedRange.NumberFormat = GetExcelNumberFormat(apttusField);
                        }

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            double MaxDoubleValue = applicationDefinitionManager.GetFieldMaxValue(apttusField);
                            //TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDecimal, Excel.XlDVAlertStyle.xlValidAlertStop,
                            //    Excel.XlFormatConditionOperator.xlBetween, (-1 * MaxCurrencyValue).ToString(), MaxCurrencyValue.ToString());

                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDecimal, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, (-1 * MaxDoubleValue), MaxDoubleValue);

                            TargetNamedRange.Validation.ErrorMessage = String.Format(resourceManager.GetResource("EXCELHELP_AddCellRangeValidationDouble_InfoMsg"), apttusField.Name, (-1 * MaxDoubleValue).ToString(), MaxDoubleValue.ToString());
                        }
                        break;
                    case Datatype.Decimal:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            //TargetNamedRange.NumberFormat = "_($* #,##0.00_);_($* (#,##0.00);_(@_)";
                            TargetNamedRange.NumberFormat = GetExcelNumberFormat(apttusField);
                        }

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            double MaxCurrencyValue = applicationDefinitionManager.GetFieldMaxValue(apttusField);
                            //TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDecimal, Excel.XlDVAlertStyle.xlValidAlertStop,
                            //    Excel.XlFormatConditionOperator.xlBetween, (-1 * MaxCurrencyValue).ToString(), MaxCurrencyValue.ToString());

                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateDecimal, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, (-1 * MaxCurrencyValue), MaxCurrencyValue);

                            TargetNamedRange.Validation.ErrorMessage = String.Format(resourceManager.GetResource("EXCELHELP_AddCellRangeValidationDecimal_InfoMsg"), apttusField.Name, (-1 * MaxCurrencyValue).ToString(), MaxCurrencyValue.ToString());
                        }
                        break;
                    case Datatype.Lookup:
                        // Number Format
                        //if (bApplyNumberFormat)
                        //{
                        //    TargetNamedRange.NumberFormat = "General";
                        //}

                        // Data Validation
                        if (bApplyDataValidation)
                        {
                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateCustom, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, "=\"\"");
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationLookUp_InfoMsg");
                        }
                        break;
                    case Datatype.Picklist_MultiSelect:
                    case Datatype.Picklist:
                    case Datatype.Editable_Picklist:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            TargetNamedRange.NumberFormat = "@";
                        }

                        // Data Validation - Always overwrite/apply data validation for picklist and dependent picklist.
                        TargetNamedRange.Validation.Delete();

                        string validationFormula = GetPicklistValidationFormula(apttusField, cellValidationInput);
                        // Handle Empty Validation Formula - In the scenario where PickList field is created, but no options are defined 
                        // e.g. Product.ProductFamily. Handle empty string or else Excel throws a COM Exception
                        if (!string.IsNullOrEmpty(validationFormula))
                        {
                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop,
                                Excel.XlFormatConditionOperator.xlBetween, metadataManager.GetLocalizedFormula(validationFormula));
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationPickList_InfoMsg");
                            bApplyDataValidation = true;
                        }
                        //Validation error is not applicable for combobox datatype.
                        bShowError = dataType != Datatype.Editable_Picklist;

                        break;
                    case Datatype.String:
                    case Datatype.Textarea:
                    case Datatype.Formula:
                        // Number Format
                        if (bApplyNumberFormat)
                        {
                            //TargetNamedRange.NumberFormat = "General";
                            // Changing String / Textarea datatype format to Text, since some fields like in Agreement.Agreement Number field 
                            // which has value like 00001458.0, using General formatting it gets converted to 1458. 
                            // This causes save module to mark this fields as Dirty and considered for Save operation.
                            TargetNamedRange.NumberFormat = "@";
                        }

                        // Data Validation - not supported for this data types.
                        bApplyDataValidation = false;
                        break;
                    case Datatype.Email:
                        // Email format
                        if (bApplyDataValidation)
                        {

                            string referenceCell = TargetNamedRange.get_Address(false, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                            string validationFormulaEmail = "=AND(NOT(ISERROR(FIND(\"@\"," + referenceCell + "))),NOT(ISERROR(FIND(\".\"," + referenceCell + "))),ISERROR(FIND(\" \"," + referenceCell + ")))";

                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateCustom, Excel.XlDVAlertStyle.xlValidAlertStop,
                               Excel.XlFormatConditionOperator.xlBetween, metadataManager.GetLocalizedFormula(validationFormulaEmail));
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationEmail_InfoMsg");
                        }
                        break;
                    case Datatype.Rich_Textarea:
                        bApplyDataValidation = false;
                        if (!configManager.IsRichTextEditingDisabled)
                        {
                            bApplyDataValidation = true;
                            // since Richtext data is not edited from Excel cells, it has to be treated seperately and blank validation has to be applied all the time.
                            TargetNamedRange.Validation.Delete();
                            TargetNamedRange.Validation.Add(Excel.XlDVType.xlValidateCustom, Excel.XlDVAlertStyle.xlValidAlertStop,
                                    Excel.XlFormatConditionOperator.xlBetween, "=\"\"");
                            TargetNamedRange.Validation.ErrorMessage = resourceManager.GetResource("EXCELHELP_AddCellRangeValidationRichText_InfoMsg");
                        }
                        break;
                    case Datatype.NotSupported:
                        bApplyDataValidation = false;
                        break;
                    default:
                        bApplyDataValidation = false;
                        break;
                }

                if (bApplyDataValidation)
                {
                    TargetNamedRange.Validation.IgnoreBlank = true;
                    TargetNamedRange.Validation.InCellDropdown = true;
                    TargetNamedRange.Validation.InputTitle = Core.Constants.PRODUCT_NAME;
                    TargetNamedRange.Validation.ErrorTitle = Core.Constants.PRODUCT_NAME;
                    TargetNamedRange.Validation.ShowInput = true;
                    TargetNamedRange.Validation.ShowError = bShowError;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Get Excel format for double datatype
        /// For Double format ::  "_(* #,##0.00_);_(* (#,##0.00);_(@_)";
        /// For Decimal format :: "_($* #,##0.00_);_($* (#,##0.00);_(@_)";
        /// </summary>
        /// <param name="apttusField"></param>
        /// <returns></returns>
        public static string GetExcelNumberFormat(ApttusField apttusField)
        {
            string result = string.Empty;
            string strZeros = new String('0', apttusField.Scale);

            //CultureInfo latestCultureInfo = GetLatestCulture;
            //string NegativeSign = latestCultureInfo.NumberFormat.NegativeSign;
            //string NumberDecimalSeparator = latestCultureInfo.NumberFormat.NumberDecimalSeparator;
            //string NumberGroupSeparator = latestCultureInfo.NumberFormat.NumberGroupSeparator;
            //string CurrencyDecimalSeparator = latestCultureInfo.NumberFormat.CurrencyDecimalSeparator;
            //string CurrencyGroupSeparator = latestCultureInfo.NumberFormat.CurrencyGroupSeparator;

            // For Double format
            if (apttusField.Datatype == Datatype.Double)
            {
                // If scale is not in value
                if (!string.IsNullOrEmpty(strZeros))
                    result = "_(* #,##0." + strZeros + "_);_(* (#,##0." + strZeros + ");_(@_)";
                //result = "_(* #" + NumberGroupSeparator + "##0" + NumberDecimalSeparator + strZeros + "_);_(* (#" + NumberGroupSeparator + "##0" + NumberDecimalSeparator + strZeros + ");_(@_)";
                else
                    result = "_(* #,##0_);_(* (#,##0);_(@_)";
                //result = "_(* #" + NumberGroupSeparator + "##0_);_(* (#" + NumberGroupSeparator + "##0);_(@_)";
            }
            // For Decimal format
            else if (apttusField.Datatype == Datatype.Decimal)
            {
                // If scale is not in value
                if (!string.IsNullOrEmpty(strZeros))
                    result = "_(* #,##0." + strZeros + "_);_(* (#,##0." + strZeros + ");_(@_)";
                //result = "_(* #" + CurrencyGroupSeparator + "##0" + CurrencyDecimalSeparator + strZeros + "_);_(* (#" + CurrencyGroupSeparator + "##0" + CurrencyDecimalSeparator + strZeros + ");_(@_)";
                else
                    result = "_(* #,##0_);_(* (#,##0);_(@_)";
                //result = "_(* #" + CurrencyGroupSeparator + "##0_);_(* (#" + CurrencyGroupSeparator + "##0);_(@_)";
            }
            return result;
        }

        public static Excel.Range GetColumnRange(string TargetNamedRange, int columnIndex)
        {
            Excel.Workbook m_workbook = excelApp.ActiveWorkbook;
            Excel.Range oGridRange = m_workbook.Names.Item(TargetNamedRange, System.Type.Missing, System.Type.Missing).RefersToRange;
            return GetColumnRange(oGridRange, columnIndex);
        }

        public static Excel.Range GetColumnRange(Excel.Range oGridRange, int columnIndex)
        {
            Excel.Worksheet oSheet = oGridRange.Worksheet;
            Excel.Range oRange = (Excel.Range)oGridRange.Cells[1, columnIndex];

            return oRange;
        }

        private static string GetPicklistValidationFormula(ApttusField apttusField, CellValidationInput cellValidationInput)
        {
            string FormulaString = string.Empty; // Use GetLocalizedFormula() function convert it.
            string ValidationFormula = string.Empty;

            switch (apttusField.PicklistType)
            {
                #region "Regular Picklist"
                case PicklistType.None:
                case PicklistType.Regular:
                case PicklistType.TwoOption:
                    FormulaString = GetPicklistIndirectFormula(cellValidationInput, apttusField.Id);
                    break;
                #endregion

                case PicklistType.Dependent:
                    bool controllingValuehasSpecialCharacter = false;
                    try
                    {
                        controllingValuehasSpecialCharacter = apttusField.DependentPicklistValues.Any(a => pickListValidation.IsMatch(a.ControllingValue));
                    }
                    catch
                    {
                        controllingValuehasSpecialCharacter = false;
                    }
                    if (cellValidationInput.ReferenceNamedRange != null)
                        FormulaString = GetDependentIndirectFormula(cellValidationInput, apttusField.Id, controllingValuehasSpecialCharacter);
                    else
                        FormulaString = GetPicklistIndirectFormula(cellValidationInput, apttusField.Id); // Dependent is same as Picklist when controlling picklist reference range is null (not present)
                    break;

                default:
                    FormulaString = string.Empty;
                    break;
            }

            #region "Error Handling"
            if (!string.IsNullOrEmpty(FormulaString))
                ValidationFormula = "=IF(IFERROR(ROWS(" + FormulaString + "),-1) < 0, " + Constants.INVALIDPICKLISTDATA_RANGENAME + Constants.COMMA + FormulaString + ")";

            //Removing length as issue was found in Office update 1809 only
            // IFERROR formula is not possible in this scenario.
            //if (ValidationFormula.Length > Constants.EXCEL_DATA_VALIDATION_ISSUE_LIMIT)
            //    ValidationFormula = "=" + FormulaString;

            #endregion
            return ValidationFormula;
        }

        private static string GetPicklistIndirectFormula(CellValidationInput cellValidationInput, string id)
        {
            string FormulaString = string.Empty;

            FormulaString = "INDIRECT(SUBSTITUTE(\"" + cellValidationInput.ObjectId + Constants.DOT + GetRecordTypeFormula(cellValidationInput, CompatibleExcelFormula("VLOOKUP(SUBSTITUTE(", true, false), CompatibleExcelFormula(",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE)", true), Constants.COMMA) + id + "\",\" \",\"_\"))";

            return FormulaString;
        }

        private static string GetDependentIndirectFormula(CellValidationInput cellValidationInput, string id, bool controllingValuehasSpecialCharacter)
        {
            string FormulaString = string.Empty;
            string rc = GetReferenceCell(cellValidationInput);

            FormulaString = "INDIRECT(SUBSTITUTE(\"" + cellValidationInput.ObjectId + Constants.DOT + GetRecordTypeFormula(cellValidationInput, CompatibleExcelFormula("VLOOKUP(SUBSTITUTE(", true, false), CompatibleExcelFormula(",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE)", true), Constants.COMMA) + id + GetFieldLengthFormula(CompatibleExcelFormula(".\"&VLOOKUP(SUBSTITUTE(", false), rc, controllingValuehasSpecialCharacter) + CompatibleExcelFormula(",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE),\" \",\"_\"))", false, true);
            return FormulaString;
        }

        /// AB-3344 Compatible Excel formula will be created for data validation due to restrictions introduced by Microsoft Office365 version 1809
        private static string CompatibleExcelFormula(string formula, bool includesRecordType, bool includesUnderscore = false)
        {
            string FormulaString = formula;
            string replaceString = ",\" \",\"_\"))";            
            FormulaString = (!includesRecordType ? (includesUnderscore ? FormulaString.Replace("VLOOKUP(SUBSTITUTE(", string.Empty).Replace(",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE),\" \",\"_\"))", replaceString) : FormulaString.Replace("VLOOKUP(SUBSTITUTE(", string.Empty)) : (includesUnderscore ? FormulaString.Replace("VLOOKUP(SUBTITUTE(", string.Empty).Replace(",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE),\" \",\"_\"))", replaceString) : FormulaString));

            return FormulaString;
        }

        private static string GetReferenceCell(CellValidationInput cellValidationInput)
        {
            string ReferenceCell = string.Empty;
            if (cellValidationInput.ObjectType == Core.ObjectType.Independent)
            {
                ReferenceCell = cellValidationInput.ReferenceNamedRange.get_Address(true, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
            }
            else if (cellValidationInput.ObjectType == Core.ObjectType.Repeating)
            {
                if (cellValidationInput.LayoutType == Core.LayoutType.Vertical)
                    ReferenceCell = cellValidationInput.ReferenceNamedRange.get_Address(false, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                else if (cellValidationInput.LayoutType == Core.LayoutType.Horizontal)
                    ReferenceCell = cellValidationInput.ReferenceNamedRange.get_Address(true, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
            }
            return ReferenceCell;
        }

        private static string GetRecordTypeFormula(CellValidationInput cellValidationInput, string vLookupPart1, string vLookupPart2, string listSeperator)
        {
            //vLookupPart1 is set as "VLOOKUP(SUBSTITUTE("; // ."&VLOOKUP(SUBSTITUTE(
            //vLookupPart2 is set as "," + ExcelDoubleQuote + "~" + ExcelDoubleQuote + "," + ExcelDoubleQuote + "~~" + ExcelDoubleQuote + ")," + Constants.NAMEDRANGE_PICKLISTKEYVALUEPAIR + ",2,FALSE)"; // ,"~","~~"),PICKLISTKEYVALUEPAIR,2,FALSE)

            string recordTypeFormula = string.Empty;
            if (cellValidationInput.RecordTypeNamedRange != null)
            {
                string ReferenceCell = string.Empty;
                if (cellValidationInput.ObjectType == Core.ObjectType.Independent)
                {
                    ReferenceCell = cellValidationInput.RecordTypeNamedRange.get_Address(true, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                }
                else if (cellValidationInput.ObjectType == Core.ObjectType.Repeating)
                {
                    if (cellValidationInput.LayoutType == Core.LayoutType.Vertical)
                        ReferenceCell = cellValidationInput.RecordTypeNamedRange.get_Address(false, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                    else if (cellValidationInput.LayoutType == Core.LayoutType.Horizontal)
                        ReferenceCell = cellValidationInput.RecordTypeNamedRange.get_Address(true, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                }

                // "&IF(LEN(TRIM(C2))>0,C2&".","")&"
                //recordTypeFormula = "\"&(IF(LEN(TRIM(" + ReferenceCell + ")) > 0" + listSeperator + vLookupPart1 + ReferenceCell + vLookupPart2 + Constants.AMPERSAND + "\".\"" + listSeperator + "\"\"))&\"";
                recordTypeFormula = "\"&IF(LEN(" + ReferenceCell + ")>0," + ReferenceCell + "&\".\",\"\")&\"";

            }
            return recordTypeFormula;
        }

        private static string GetFieldLengthFormula(string formula, string rc, bool controllingValuehasSpecialCharacter)
        {
            string FormulaString = string.Empty; string compareString = Constants.DOT + "\"" + Constants.AMPERSAND;

            if (controllingValuehasSpecialCharacter)
            {
                FormulaString = formula == compareString ? "\"&IF(LEN(" + rc + ")=0,\"\",\".\"&VLOOKUP(SUBSTITUTE(" + rc + ",\"~\",\"~~\"),PICKLISTKEYVALUEPAIR,2,FALSE))" : string.Concat(formula, rc);
            }
            else
            {
                FormulaString = formula == compareString ? "\"&IF(LEN(" + rc + ")=0,\"\",\".\"&" + rc + ")" : string.Concat(formula, rc);
            }
            return FormulaString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="filePath"></param>
        public static void AddObjectToSheet(Excel.Worksheet oSheet, string filePath, string fileName)
        {
            string shapeName = "Apttus_" + fileName;
            try
            {

                Excel.Shape shp = oSheet.Shapes.AddOLEObject(Filename: filePath, DisplayAsIcon: true, IconFileName: fileName, IconLabel: fileName);
                try
                {
                    shp.Name = shapeName;
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
        public static List<string> GetObjectsFromSheet(Excel.Worksheet oSheet)
        {
            List<string> lstObjectName = new List<string>();
            dynamic OLE = oSheet.OLEObjects(Type.Missing);

            //foreach (dynamic obj in oSheet.OLEObjects)
            for (int i = 0; i < OLE.Count; i++)
            {
                dynamic obj = OLE[i + 1];
                string sName = obj.Name;

                if (sName.Contains("Apttus_"))
                    lstObjectName.Add(obj.Name);
            }

            return lstObjectName;
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
        /// <param name="objName"></param>
        /// <returns></returns>
        public static dynamic GetObjectsByNameFromSheet(Excel.Worksheet oSheet, string objName)
        {
            dynamic OLE = oSheet.OLEObjects(Type.Missing);

            //foreach (dynamic obj in oSheet.OLEObjects)
            for (int i = 0; i < OLE.Count; i++)
            {
                dynamic obj = OLE[i + 1];
                string sName = obj.Name;

                if (sName.Equals(objName))
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static MemoryStream OleObjectToByte(Excel.Worksheet oSheet, string objectName)
        {
            try
            {
                if (GetObjectsByNameFromSheet(oSheet, objectName) != null)
                {
                    System.Windows.Forms.Clipboard.Clear();

                    oSheet.OLEObjects(objectName).Copy();
                    System.Windows.Forms.IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();

                    string objType = "Native";
                    Object objToSave = iData.GetData(objType, true);

                    System.Windows.Forms.Clipboard.SetDataObject("");
                    MemoryStream objStream = (MemoryStream)objToSave;

                    //string sStream = Helpers.OLEContentHelper.GetOLEContent(objStream);
                    return objStream;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.DebugLog("Error while deserializing object : " + objectName + " Error Message: " + ex.Message.ToString());
                return null;
            }
        }

        static object[][] convertToJaggedArrayHorizontal(object[,] multiArray, int rowElement, int columnElement)
        {
            object[][] jaggedArray = new object[columnElement][];

            for (int c = 0; c < columnElement; c++)
            {
                jaggedArray[c] = new object[rowElement];
                for (int r = 0; r < rowElement; r++)
                {
                    jaggedArray[c][r] = multiArray[r + 1, c + 1];
                }
            }
            return jaggedArray;
        }

        static object[][] convertToJaggedArrayVertical(object[,] multiArray, int rowElement, int columnElement)
        {
            object[][] jaggedArray = new object[rowElement][];

            for (int c = 0; c < rowElement; c++)
            {
                jaggedArray[c] = new object[columnElement];
                for (int r = 0; r < columnElement; r++)
                {
                    jaggedArray[c][r] = multiArray[c + 1, r + 1];
                }
            }
            return jaggedArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oRange"></param>
        /// <returns></returns>
        public static void RangeToDataTable(Excel.Range oRange, ref DataTable dt, LayoutType type)
        {
            WorkbookEventManager workbookEventManager = WorkbookEventManager.GetInstance;
            workbookEventManager.UnsubscribeEvents();

            try
            {
                dt = new DataTable();
                int rowCount = oRange.Rows.Count;
                int columnCount = oRange.Columns.Count;

                if (type == LayoutType.Horizontal)
                {
                    object[][] objArray = convertToJaggedArrayHorizontal((object[,])oRange.Value2, rowCount, columnCount);

                    // Add columns to datatable
                    for (int row = 0; row < rowCount; row++)
                    {
                        var value = objArray[0][row];
                        string colName = value != null ? value.ToString() : string.Empty;
                        colName += (row + 1).ToString(); // row index starts with 0 so adding 1

                        dt.Columns.Add(colName);
                    }

                    try
                    {
                        // Add rows to datatable
                        dt.BeginLoadData();
                        for (int col = 2; col < columnCount; col++)
                        {
                            dt.LoadDataRow(objArray[col], false);
                        }
                        dt.AcceptChanges();
                    }
                    finally
                    {
                        dt.EndLoadData();
                        objArray = null;
                    }
                }
                else
                {
                    object[][] objArray = convertToJaggedArrayVertical((object[,])oRange.Value2, rowCount, columnCount);

                    // Add columns to datatable
                    for (int col = 0; col < columnCount; col++)
                    {
                        var value = objArray[0][col];
                        string colName = value != null ? value.ToString() : string.Empty;
                        colName += (col + 1).ToString(); // col index starts with 0 so adding 1

                        dt.Columns.Add(colName);
                    }

                    try
                    {
                        // Add rows to datatable
                        dt.BeginLoadData();
                        for (int row = 2; row < rowCount; ++row)
                        {
                            dt.LoadDataRow(objArray[row], false);
                        }
                        dt.AcceptChanges();
                    }
                    finally
                    {
                        dt.EndLoadData();
                        objArray = null;
                    }
                }
            }
            finally
            {
                workbookEventManager.SubscribeEvents();
            }
        }

        public static DataTable RangeToIdDataTable(Excel.Range oRange, LayoutType type = LayoutType.Vertical)
        {
            DataTable dt = new DataTable();

            if (type == LayoutType.Horizontal)
            {
                // Add ID column to be read from Range.ID property.
                dt.Columns.Add(Constants.ID_ATTRIBUTE);
                dt.Columns.Add(Constants.CELL_LOCATION);

                // TODO:: add logic for horizontal rendering
                // Add rows to datatable
                for (int col = 2; col <= oRange.Columns.Count; col++)
                {
                    DataRow dr = dt.NewRow();

                    // Read ID column values
                    Excel.Range rngID = oRange.Cells[col, 1];
                    string RecordID = rngID.ID;
                    if (!string.IsNullOrEmpty(RecordID))
                    {
                        dr[0] = RecordID;
                        dr[1] = Convert.ToString("'" + rngID.Worksheet.Name + "'!" + rngID.Address);

                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                // Add ID column to be read from Range.ID property.
                dt.Columns.Add(Constants.ID_ATTRIBUTE);
                dt.Columns.Add(Constants.CELL_LOCATION);

                // Add rows to datatable
                for (int row = 2; row <= oRange.Rows.Count; row++)
                {
                    DataRow dr = dt.NewRow();

                    // Read ID column values
                    Excel.Range rngID = oRange.Cells[row, 1];
                    string RecordID = rngID.ID;
                    if (!string.IsNullOrEmpty(RecordID))
                    {
                        dr[0] = RecordID;
                        dr[1] = Convert.ToString("'" + rngID.Worksheet.Name + "'!" + rngID.Address);

                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idData"></param>
        public static void IdDataTableToRange(DataTable idData)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < idData.Rows.Count; i++)
            {
                string cellLocation = idData.Rows[i][Constants.CELL_LOCATION].ToString();
                Excel.Range oRange = ExcelHelper.GetSingleCellRange(cellLocation);

                oRange.ID = idData.Rows[i][Constants.ID_ATTRIBUTE].ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetNameAndRange"></param>
        /// <returns></returns>
        public static Excel.Range GetSingleCellRange(string SheetNameAndRange)
        {
            if (SheetNameAndRange != null)
            {
                string[] splitValues = SheetNameAndRange.Split(Core.Constants.SHEET_DELIMITER.ToCharArray());
                Excel.Worksheet osheet = GetWorkSheet(splitValues[0].Replace("'", ""));
                return osheet.Range[splitValues[1]];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oRange"></param>
        /// <returns></returns>
        public static DataTable RangeToDataTableForCrossTab(Excel.Range oRange)
        {
            DataTable dt = new DataTable();

            // Add columns to datatable
            for (int col = 0; col <= oRange.Columns.Count; col++)
            {
                // For empty columns add Column as string.empty.
                // TODO:: check if it affect Save Other fields or not.
                string colName = oRange.Cells[0, col].Value2 != null ? oRange.Cells[0, col].Value2 : string.Empty;
                dt.Columns.Add(colName);
            }

            // Add rows to datatable
            // We get second row from the datatable because in cross tab first row is always blank
            for (int row = 1; row <= oRange.Rows.Count; row++)
            {
                DataRow dr = dt.NewRow();

                // Construct data row
                // we get second column from the sheet because in cross tab first column is always blank
                for (int col = 0; col < oRange.Columns.Count + 1; col++)
                {
                    dr[col] = oRange.Cells[row, col].Value2;
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Sheet to Data
        /// </summary>
        /// <param name="oSheet"></param>
        /// <returns></returns>
        public static DataTable SheetToDataTableColumn(Excel.Worksheet oSheet)
        {

            Excel.Range oRange = oSheet.UsedRange;
            DataTable dt = new DataTable();

            // Add columns to datatable
            for (int col = 1; col <= oRange.Columns.Count; col++)
            {
                dt.Columns.Add(oRange.Cells[1, col].Value2);
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SheetNameAndRange"></param>
        /// <returns></returns>
        public static int GetRowIndex(string DesignerLocation)
        {
            int value = -1;
            if (!string.IsNullOrEmpty(DesignerLocation))
            {
                string[] splitValues = DesignerLocation.Split(Core.Constants.SHEET_DELIMITER.ToCharArray());
                Excel.Worksheet osheet = GetWorkSheet(splitValues[0]);
                value = osheet.Range[splitValues[1]].Row;
            }
            return value;
        }

        public static string SaveACopyAndEncodeWorkbook(Excel.Workbook wb, string SaveAsFileName, bool setDocProperty = true)
        {
            try
            {
                string OriginalFileName = wb.FullName;

                // Make runtime flag false to return from SerializeData as it done above.
                if (setDocProperty)
                    ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_RUNTIME_ATTACHMENTFILE, "true");

                byte[] binaryData = ApttusObjectManager.EncodeByteArray();

                if (setDocProperty)
                    ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_RUNTIME_ATTACHMENTFILE);

                string sReturn = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                return sReturn;
                //return EncodeFileToString(SaveAsFileName);
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public static void SaveAs(Excel.Workbook wb, string SaveAsFilename)
        {
            excelApp.DisplayAlerts = false;

            Excel.XlFileFormat oSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;
            if (SaveAsFilename.ToLower().EndsWith(Constants.XLSMWITHOUTDOT))
                oSaveFormat = Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled;

            wb.SaveAs(
                SaveAsFilename,						        //filename to save as
                oSaveFormat,		                    //file format
                ApttusGlobals.oMissing,			        //password
                ApttusGlobals.oMissing,			        //WriteResPassword
                ApttusGlobals.oMissing,			        //ReadOnlyRecommended
                ApttusGlobals.oMissing,			        //CreateBackup
                Excel.XlSaveAsAccessMode.xlNoChange,	//AccessMode
                ApttusGlobals.oMissing,			        //ConflictResolution
                ApttusGlobals.oFalse,			        //AddToMru
                ApttusGlobals.oMissing,			        //TextCodepage
                ApttusGlobals.oMissing,		            //TextVisualLayout
                ApttusGlobals.oMissing			        //Local
                );

            excelApp.DisplayAlerts = true;
        }


        public static Excel.Workbook CheckIfFileOpened(string wbook)
        {
            Excel.Workbook workbook = null;
            if (wbook == null) return workbook;
            try
            {
                foreach (Excel.Workbook wb in excelApp.Workbooks)
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

        public static string EncodeFileToString(string FileName)
        {
            string result = string.Empty;
            System.IO.FileStream inFile;
            byte[] binaryData;

            try
            {
                inFile = new System.IO.FileStream(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                binaryData = new Byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                inFile.Close();
            }
            catch (System.Exception exp)
            {
                // Error creating stream or reading from it.
                System.Console.WriteLine("{0}", exp.Message);
                return result;
            }

            // Convert the binary input into Base64 UUEncoded output. 
            try
            {
                result = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Binary data array is null.");
                return result;
            }

            binaryData = null;
            return result;

            //// Write the UUEncoded version to the output file.
            //System.IO.StreamWriter outFile;
            //try
            //{
            //    outFile = new System.IO.StreamWriter(outputFileName,
            //                         false,
            //                         System.Text.Encoding.ASCII);
            //    outFile.Write(base64String);
            //    outFile.Close();
            //}
            //catch (System.Exception exp)
            //{
            //    // Error creating stream or writing to it.
            //    System.Console.WriteLine("{0}", exp.Message);
            //}
        }

        public static void UpdateSheetProtection(Excel.Worksheet oSheet, bool bProtect)
        {
            //which means that only the user is blocked from editing the worksheet, while code such as VBA, VB.NET, or C# would not be prevented from manipulating the worksheet
            //when user is on addrow mode, sheet will be procted and lookahead couldn't update the id / lookup fields
            // hence added userInterfaceOnly 
            // JIRA AB-1384
            bool userInterfaceOnly = true;
            if (!ExcelHelper.IsSheetProtectedByUser(oSheet))
            {
                if (bProtect)
                {
                    //1. Drawing Objects
                    //2. Contents
                    //3. Scenarios
                    //4. UserInterface Only
                    //5. Allow Formatting Cells
                    //6. Allow Formatting Columns
                    //7. Allow Formatting Rows
                    //8. Allow Inserting Columns
                    //9. Allow Inserting Rows
                    //10. Allow Inserting Hyperlinks
                    //11. Allow Deleting Columns
                    //12. Allow Deleting Rows
                    //13. Allow Sort
                    //14. Allow Filtering
                    //15. Allow Pivot Tables
                    oSheet.Protect(Constants.XLPROTECTSHEET_PASSWORD,
                    Type.Missing, Type.Missing, Type.Missing, userInterfaceOnly,
                    oSheet.Protection.AllowFormattingCells, oSheet.Protection.AllowFormattingColumns, oSheet.Protection.AllowFormattingRows,
                    oSheet.Protection.AllowInsertingColumns, oSheet.Protection.AllowInsertingRows, oSheet.Protection.AllowInsertingHyperlinks,
                    oSheet.Protection.AllowDeletingColumns, oSheet.Protection.AllowDeletingRows, oSheet.Protection.AllowSorting, oSheet.Protection.AllowFiltering,
                    oSheet.Protection.AllowUsingPivotTables);
                }
                else
                {
                    oSheet.Unprotect(Constants.XLPROTECTSHEET_PASSWORD);
                }
            }
            else
            {
                if (ExcelHelper.IsSheetProtectedByUser(oSheet))
                {
                    UserSheetProtection(oSheet, bProtect);
                }
            }
        }
        /// <summary>
        /// User Protection sheet
        /// </summary>
        /// <param name="oSheet"></param>
        /// <param name="bProtect"></param>
        public static void UserSheetProtection(Excel.Worksheet oSheet, bool bProtect)
        {
            excelApp.ScreenUpdating = false;

            try
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
            catch (Exception)
            {
                throw;
            }
            finally
            {
                excelApp.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// Unloack cell withing X-Author Range
        /// </summary>
        /// <param name="oSheet"></param>
        private static void CellUnlock(Excel.Worksheet oSheet)
        {
            if (ConfigurationManager.GetInstance.Application != null)
            {
                List<string> namedRanges = (from r in ConfigurationManager.GetInstance.GetRangeMaps()
                                            select r.RangeName).ToList();

                foreach (string name in namedRanges)
                {
                    //Intersect works within the same worksheet
                    Excel.Range namedRange = GetRange(name);
                    if (namedRange.Worksheet.Name == oSheet.Name)
                    {
                        // Unloack cell withing X-Author Range
                        namedRange.Locked = false;
                    }
                }
            }
        }

        public static bool IsSheetProtectedByUser(Excel.Worksheet oSheet)
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
        public static void UpdateCellLock(Excel.Range oRange, bool bProtect)
        {
            oRange.Locked = bProtect;
        }

        public static void SetCellBorderLineStyle(Excel.Range cell, Excel.XlLineStyle lineStyle = Excel.XlLineStyle.xlLineStyleNone)
        {
            cell.Borders.LineStyle = lineStyle;
        }

        public static void SetBorderThemeColor(Excel.Range cell, Excel.XlThemeColor themeColor = Excel.XlThemeColor.xlThemeColorAccent6)
        {
            cell.Borders.ThemeColor = themeColor;
        }

        public static void SetBorderWeight(Excel.Range cell, Excel.XlBorderWeight borderWeight)
        {
            cell.Borders.Weight = borderWeight;
        }

        public static void ClearRepeatingCells(string TargetNamedRange, Guid AppObject, bool bRemoveDataProtection = true)
        {
            try
            {
                ExcelApp.EnableEvents = false;

                int nSkipFormulaRowIndex = 2;
                Excel.Range RepeatingCellsDataRange = ExcelHelper.GetRange(TargetNamedRange);
                Excel.Worksheet sheet = RepeatingCellsDataRange.Worksheet;

                //// Remove Data Protection before rendering
                ExcelHelper.UpdateSheetProtection(sheet, false);

                if (RepeatingCellsDataRange.Rows.Count > nSkipFormulaRowIndex)
                {
                    //Skip the Formula Row
                    Excel.Range firstCell = ExcelHelper.NextVerticalCell(RepeatingCellsDataRange, nSkipFormulaRowIndex);
                    Excel.Range lastCell = ExcelHelper.NextVerticalCell(ExcelHelper.NextHorizontalCell(RepeatingCellsDataRange, RepeatingCellsDataRange.Columns.Count - 1), RepeatingCellsDataRange.Rows.Count - 1);

                    Excel.Range rangeToClear = sheet.Range[firstCell, lastCell];

                    // Remove Data Protection for the Repeating Group.
                    //bRemoveDataProtection will be used by DataTransfer when we need to clear the range but don't want the protection to be removed.
                    if (bRemoveDataProtection)
                    {
                        SaveMap currentSaveMap = ConfigurationManager.GetInstance.GetSaveMapbyTargetNamedRange(TargetNamedRange);
                        if (currentSaveMap != null)
                        {
                            DataProtectionModel dpm = DataManager.GetInstance.DataProtection.FirstOrDefault(dp => dp.WorksheetName.Equals(sheet.Name)
                                                        & dp.SaveMapId.Equals(currentSaveMap.Id) & dp.SaveGroupAppObject.Equals(AppObject));
                            if (dpm != null)
                                DataManager.GetInstance.DataProtection.Remove(dpm);
                        }
                    }
                    //Remove the filter if any.
                    Excel.AutoFilter Filter = sheet.AutoFilter;
                    if (Filter != null)
                    {
                        Excel.Range filterRange = Filter.Range;
                        //Check if this repeating group has the filter on top of it.
                        if (excelApp.Intersect(RepeatingCellsDataRange, filterRange) != null)
                            Filter.ShowAllData();
                    }

                    //Clear All Data
                    rangeToClear.ClearContents();

                    //Delete All Rows
                    rangeToClear.EntireRow.Rows.Delete();
                }

                //// Apply Data Protection after rendering
                //if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(sheet.Name)))
                //    ExcelHelper.UpdateSheetProtection(sheet, true);
            }
            finally
            {
                ExcelApp.EnableEvents = true;
            }
        }

        public static bool IsValidName(Excel.Name name)
        {
            try
            {
                string Name = (name.RefersToLocal) as string;
                return Constants.INVALIDEXCELFORMULATEXT.Where(ExcelName => Name.Contains(ExcelName)).Count() == 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void ApplyGroupingBorder(Excel.Range row, Excel.XlBordersIndex borderIndex)
        {
            row.Font.Bold = true;
            row.Borders[borderIndex].LineStyle = Excel.XlLineStyle.xlContinuous;
            row.Borders[borderIndex].ColorIndex = 0;
            row.Borders[borderIndex].TintAndShade = 0;
            row.Borders[borderIndex].Weight = Excel.XlBorderWeight.xlThin;
        }

        public static bool DetectAndExitEditMode()
        {
            bool result = true;
            try
            {
                if (GetEditMode())
                    excelApp.SendKeys("{ENTER}");
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
            CommandBarControl oNewMenu = excelApp.CommandBars["Worksheet Menu Bar"].FindControl(1, 18, Type.Missing, Type.Missing, true);
            if (oNewMenu != null && !oNewMenu.Enabled)
                result = true;

            return result;
        }

        /// <summary>
        /// This method will update Expression Builder Model, for all filter whose value type is Cell Reference.
        /// For Cell Reference Type filters values needs to be updated before hand since it uses Excel Object Model and 
        /// Whole Expression building logic is now in Core library which cannot use Excel Object Model.
        /// </summary>
        /// <param name="searchFilterGroups"></param>
        public static void SetCellReferenceFilterValue(List<SearchFilterGroup> searchFilterGroups)
        {
            if (searchFilterGroups != null && searchFilterGroups.Count > 0 && searchFilterGroups[0].Filters != null)
            {
                List<SearchFilter> cellFilters = searchFilterGroups[0].Filters.Where(f => f.ValueType == ExpressionValueTypes.CellReference).ToList();
                if (cellFilters != null && cellFilters.Count > 0)
                {
                    for (int i = 0; i < cellFilters.Count; i++)
                    {
                        Excel.Range cellReferenceRange = GetRange(cellFilters[i].SearchFilterLabel);
                        if (cellReferenceRange != null)
                        {
                            int nRows = cellReferenceRange.Rows.Count;
                            int nCols = cellReferenceRange.Columns.Count;

                            if (nRows == 1 && nCols == 1)
                                cellFilters[i].Value = Convert.ToString(cellReferenceRange.Value);
                            else
                            {
                                object[,] objArray = (object[,])cellReferenceRange.Value;
                                if (objArray == null)
                                {
                                    cellFilters[i].Value = string.Empty;
                                    continue;
                                }
                                StringBuilder value = new StringBuilder(1000); //Magical Number for buffer allocation.

                                for (int row = 1; row <= nRows; ++row)
                                {
                                    //SELECT Id,Name,City FROM Account WHERE (City__c in ('Ahmedabad','\'\'','Beijing'))
                                    //SELECT Id,Name,Type,Phone,Fax,Website,Industry,AnnualRevenue,Description,AccountSource FROM Account WHERE (Name in ('Acme','''','ABC Consulate'))
                                    for (int col = 1; col <= nCols; ++col)
                                    {
                                        object data = objArray[row, col];
                                        if (data == null)
                                            value.Append(Constants.QUOTE).Append(Convert.ToString(data)).Append(Constants.QUOTE).Append(Environment.NewLine);
                                        else
                                            value.Append(Convert.ToString(data)).Append(Environment.NewLine);
                                    }
                                }
                                objArray = null; //Important
                                cellFilters[i].Value = value.ToString();
                                value.Clear();
                                value.Capacity = 0;
                                value = null;
                            }
                        }
                        else
                            cellFilters[i].Value = string.Empty;
                    }
                }
            }
        }

        public static void UndoLastAction()
        {
            try
            {
                excelApp.EnableEvents = false;
                excelApp.Undo();
            }
            finally
            {
                excelApp.EnableEvents = true;
            }
        }
        public static string NumberToColum(int col)
        {
            if (col < 1 || col > 16384) //Excel columns are one-based (one = 'A')
                throw new ArgumentException("col must be >= 1 and <= 16384");

            if (col <= 26) //one character
                return ((char)(col + 'A' - 1)).ToString();

            else if (col <= 702) //two characters
            {
                char firstChar = (char)((int)((col - 1) / 26) + 'A' - 1);
                char secondChar = (char)(col % 26 + 'A' - 1);

                if (secondChar == '@') //Excel is one-based, but modulo operations are zero-based
                    secondChar = 'Z'; //convert one-based to zero-based

                return string.Format("{0}{1}", firstChar, secondChar);
            }

            else //three characters
            {
                char firstChar = (char)((int)((col - 1) / 702) + 'A' - 1);
                char secondChar = (char)((col - 1) / 26 % 26 + 'A' - 1);
                char thirdChar = (char)(col % 26 + 'A' - 1);

                if (thirdChar == '@') //Excel is one-based, but modulo operations are zero-based
                    thirdChar = 'Z'; //convert one-based to zero-based

                return string.Format("{0}{1}{2}", firstChar, secondChar, thirdChar);
            }
        }
        public static bool IsCellRefValid(string cellRef)
        {

            // Validate the 2nd part of the expression after ! symbol
            bool isCellReferenceValid = false;
            string cellReferenceValue = cellRef;
            if ((!string.IsNullOrEmpty(cellReferenceValue)) && cellReferenceValue.LastIndexOf(@"!") > 0
                && cellReferenceValue.LastIndexOf(@"!") < cellReferenceValue.Length)
            {
                int sheetNameEnd = cellReferenceValue.LastIndexOf(@"!");
                string sheetName = cellReferenceValue.Substring(0, sheetNameEnd);
                var workSheet = GetWorkSheet(sheetName);
                if (workSheet != null)
                {
                    // Worksheet exists, Match the cell reference pattern
                    string cellLocation = cellReferenceValue.Substring(sheetNameEnd + 1);
                    Regex regex = new Regex(@"(\$?[A-Za-z]{1,3})(\$?[0-9]{1,6})");
                    isCellReferenceValid = regex.IsMatch(cellLocation);
                }
            }
            if (!isCellReferenceValid)
            {

                return false;
            }
            return true;

        }

        public static Excel.Range GetRangeByLocation(string sheetNameAndRange)
        {
            if (string.IsNullOrEmpty(sheetNameAndRange))
                return null;

            string[] splitValues = sheetNameAndRange.Split(Core.Constants.SHEET_DELIMITER.ToCharArray());
            Excel.Range oRange = null;
            if (splitValues.Count() == 2)
            {
                Excel.Worksheet osheet = GetWorkSheet(splitValues[0]);
                if (osheet != null)
                    oRange = osheet.Range[splitValues[1]];
            }
            return oRange;
        }

        //public static bool IsValidCellReference(string cellRef)
        //{
        //    try
        //    {
        //        ExcelHelper.GetRangeByLocation(cellRef);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        internal static void CopyEmbeddedFileToDisk(Excel.Worksheet oSheet, string fileLocation, string shapeName)
        {
            try
            {
                System.Windows.Forms.Clipboard.Clear();

                Excel.OLEObject embeddedOLEObject = oSheet.OLEObjects(shapeName) as Excel.OLEObject;
                embeddedOLEObject.Copy();

                System.Windows.Forms.IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();
                MemoryStream objStream = iData.GetData("Native") as MemoryStream;
                System.Windows.Forms.Clipboard.SetDataObject("");
                long sizeOfMyObj = objStream.Length;

                using (objStream)
                {
                    string fileExtension = ".DLL";
                    /*
                     * Finds 3 occurrences of fileExtension. The "Native" format has a content header, which
                     * contains the filename 3 times: one pure filename and then 2 filenames with full path.
                     * After last filename there are 5 extra bytes and then the actual file content starts. 
                     * The actual file content ends 2 bytes before the "Native" format data ends 
                     */
                    long startOffset = 5L;
                    long foundPosition = 0;
                    for (int occurrence = 1; occurrence <= 3; occurrence++)
                    {
                        foundPosition = FindInStream(fileExtension, false, foundPosition, objStream) + fileExtension.Length;
                        if (foundPosition == -1)
                            break;
                    }
                    if (foundPosition != -1)
                    {
                        long fileContentStartPosition = foundPosition + startOffset;
                        int bufCount = 8192;
                        byte[] buffer = new byte[bufCount];
                        int readBytes = 0;
                        using (FileStream fileStream = new FileStream(fileLocation, FileMode.Create))
                        {
                            objStream.Seek(fileContentStartPosition, SeekOrigin.Begin);
                            while (true)
                            {
                                readBytes = objStream.Read(buffer, 0, bufCount);
                                if (readBytes == 0)
                                    break;
                                fileStream.Write(buffer, 0, readBytes);
                            }
                        }
                        File.SetAttributes(fileLocation, FileAttributes.Normal);
                        buffer = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.DebugLog("Error while copying oleobject to disk. \n");
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private static long FindInStream(string textToFind, bool caseSensitive, long startPos, MemoryStream sourceStream)
        {
            /*
             * Converts sought string to an array of chars and gets the array length
             */
            char[] charsToFind = textToFind.ToCharArray();
            long textToFindLength = textToFind.Length;

            /*
             * Converts source stream to an array of bytes and gets the array length
             */
            byte[] bufArray = sourceStream.ToArray();
            long bufLength = bufArray.Length;

            /*
             * Performs string search in the source stream
             */
            for (long x = startPos; x < bufLength; x++)
            {
                bool firstByteFound;
                /*
                 * Finds out whether the first byte of the string is found in the stream
                 */
                if (caseSensitive)
                {
                    firstByteFound = (bufArray[x] == (byte)charsToFind[0]);
                }
                else
                {
                    firstByteFound = (bufArray[x] == (byte)(charsToFind[0].ToString().ToLower())[0]) ||
                     (bufArray[x] == (byte)(charsToFind[0].ToString().ToUpper())[0]);
                }

                if (firstByteFound)
                {
                    long y;

                    /*
                     * Finds out whether all next bytes of the string follow the first found byte in the stream
                     */
                    for (y = 1; y < textToFindLength; y++)
                    {
                        if ((x + y) >= bufLength)
                            break;
                        bool nextByteFound;
                        if (caseSensitive)
                        {
                            nextByteFound = (bufArray[x + y] != (byte)charsToFind[y]);
                        }
                        else
                        {
                            nextByteFound = (bufArray[x + y] != (byte)(charsToFind[y].ToString().ToLower())[0] &&
                             bufArray[x + y] != (byte)(charsToFind[y].ToString().ToUpper())[0]);
                        }
                        if (nextByteFound)
                            break;
                    }
                    if (y == textToFindLength)
                    {
                        sourceStream.Flush();
                        return (x);
                    }
                }
            }
            sourceStream.Flush();
            return (-1);
        }

        /// <summary>
        /// Sets the SearchFilter.FieldId property to ColumnName, if the column is empty. A column is said to be empty, if there is no retrievefield mapped in that column.
        /// </summary>
        /// <param name="group"></param>p
        /// <param name="repeatingGroup"></param>
        /// <param name="targetRange"></param>
        internal static void ApplyEmptyColumnNamesToSearchFilter(SearchFilterGroup searchFilterGroup, RepeatingGroup repeatingGroup, Excel.Range targetRange)
        {
            //Add all empty columns in the repeating group.
            int nColumns = targetRange.Columns.Count;
            for (int columnIndex = 1; columnIndex <= nColumns; ++columnIndex)
            {
                if (repeatingGroup.RetrieveFields.Find(rf => rf.TargetColumnIndex == columnIndex) == null)
                {
                    //It is an empty column
                    //Excel.Range cell = targetRange.Cells[1, columnIndex] as Excel.Range;
                    string columnName = string.Format(Constants.EMPTYCOLUMNFIELDIDFORMAT, columnIndex);
                    IEnumerable<SearchFilter> filters = searchFilterGroup.Filters.Where(sf => sf.FieldId == columnIndex.ToString());
                    if (filters != null && filters.Count() > 0)
                    {
                        foreach (SearchFilter filter in filters)
                            filter.FieldId = columnName;
                    }
                }
            }
        }

        public static void RangeToDataTableWithPreparedColumns(Excel.Range oRange, ref DataTable dt, LayoutType type)
        {
            WorkbookEventManager workbookEventManager = WorkbookEventManager.GetInstance;
            workbookEventManager.UnsubscribeEvents();

            try
            {
                int rowCount = oRange.Rows.Count;
                int columnCount = oRange.Columns.Count;
                if (type == LayoutType.Vertical)
                {
                    object[][] objArray = convertToJaggedArrayVertical((object[,])oRange.Value2, rowCount, columnCount);

                    try
                    {
                        // Add rows to datatable
                        dt.BeginLoadData();
                        for (int row = 2; row < rowCount; ++row)
                        {
                            dt.LoadDataRow(objArray[row], false);
                        }
                        dt.AcceptChanges();
                    }
                    finally
                    {
                        dt.EndLoadData();
                        objArray = null;
                    }
                }
            }
            finally
            {
                workbookEventManager.SubscribeEvents();
            }
        }

        /// <summary>
        /// Iterate User Protected sheets and perform lock/unlock based on parameter bProtect.
        /// </summary>
        /// <param name="bProtect"></param>
        public static void UpdateApplicableSheetsForUserProtection(bool bProtect)
        {
            if (configManager.Definition != null && configManager.Definition.AppSettings != null && configManager.Definition.AppSettings.SheetProtectSettings != null)
            {
                foreach (SheetProtect worksheetdetails in configManager.Definition.AppSettings.SheetProtectSettings)
                {
                    ExcelHelper.UserSheetProtection(ExcelHelper.GetWorkSheet(worksheetdetails.SheetName), bProtect);
                }
            }
        }

        public static void UpdateApplicableSheetsForUserProtection(bool bProtect, RetrieveMap retrieveMap)
        {
            foreach (string targetNamedRange in retrieveMap.UniqueTargetNamedRanges)
            {
                Excel.Worksheet worksheet = GetRange(targetNamedRange).Worksheet;
                if (worksheet != null)
                    ExcelHelper.UserSheetProtection(worksheet, bProtect);
            }
        }
    }

    public class CellValidationInput
    {
        public string ObjectId { get; set; }

        public ObjectType ObjectType { get; set; }

        public LayoutType LayoutType { get; set; }

        public Excel.Range ReferenceNamedRange { get; set; }

        public string ControllingPicklistFieldId { get; set; }

        public Excel.Range RecordTypeNamedRange { get; set; }

        public CultureInfo culterInfo { get; set; }
    }
}
