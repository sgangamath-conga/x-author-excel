using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationExcelController
    {
        private DataMigrationModel Model;
        public string MigrationTemplatePath { get; private set; }
        private Excel.Worksheet MigrateSheet;
        private DataMigrationMigrateSheetController NameManager = DataMigrationMigrateSheetController.GetInstance;

        internal DataMigrationExcelController(DataMigrationModel model)
        {
            Model = model;
        }

        internal void ImportMigrationTemplate()
        {
            string tempDirPath = Path.GetTempPath();
            MigrationTemplatePath = Path.Combine(tempDirPath, Model.AppName + Constants.XLSM);
            File.WriteAllBytes(MigrationTemplatePath, Properties.Resources.MigrationTemplate);
        }

        internal void DeleteMigrationTemplate()
        {
            try
            {
                if (File.Exists(MigrationTemplatePath))
                    File.Delete(MigrationTemplatePath);
            }
            catch (Exception)
            {

            }
        }

        internal void CreateUniqueObjectNames()
        {
            NameManager.ResetNames();

            MigrateSheet = Globals.ThisAddIn.Application.Worksheets["Migrate"];
            Excel.Range migrateRange = ExcelHelper.GetRange("Migrate");

            object[,] names = new object[migrateRange.Rows.Count, 1];
            int row = 0, col = 0;

            foreach (MigrationObject obj in Model.MigrationObjects.OrderBy(obj => obj.Sequence))
            {
                obj.SheetName = NameManager.CreateName(obj.ObjectName, obj.Id);

                if (obj.CreateWorksheet)
                    names[row++, col] = obj.SheetName;
            }

            migrateRange.Value = names;
        }

        internal void HideMigrateSheet()
        {
            MigrateSheet.Visible = Excel.XlSheetVisibility.xlSheetHidden;
        }

        internal void ShowMigrateSheet()
        {
            MigrateSheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;
        }

        internal static Excel.Worksheet CreateWorksheet(string ObjectName)
        {
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
            object lastSheet = wb.Worksheets[wb.Worksheets.Count] as Excel.Worksheet;
            Excel.Worksheet sheet = wb.Worksheets.Add(Type.Missing, lastSheet, 1, Excel.XlSheetType.xlWorksheet) as Excel.Worksheet;
            sheet.Name = ObjectName;
            return sheet;
        }

        internal void SelectFirstWorkSheet()
        {
            try
            {
                if (Globals.ThisAddIn.Application.Worksheets.Count > 1)
                    Globals.ThisAddIn.Application.Worksheets[1].Activate();
            }
            catch (Exception)
            { }
        }
    }
}
