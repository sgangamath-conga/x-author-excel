using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;

namespace Apttus.XAuthor.AppDesigner
{
    class DataSourceValidator : IValidator
    {
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private List<string> sourceSheets = new List<string>();
        private List<string> destSheets = new List<string>();
        public DataTransferMapping source = new DataTransferMapping();

        public void assginSheets(List<string> sSheet, List<string> dSheet)
        {
            destSheets = dSheet;
            sourceSheets = sSheet;
        }

        private bool isSheetsPresent()
        {
            return (destSheets.Count > 0 && sourceSheets.Count > 0);
        }

        private void clearSheets()
        {
            destSheets.Clear();
            sourceSheets.Clear();
        }
        

        public List<ValidationResult> Validate<T>(T dataSource)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            DataTransferMapping mapping;
            if (dataSource == null)
                mapping = configManager.Definition.Mapping;
            else
                mapping = dataSource as DataTransferMapping;

            results = checkSheetChange(mapping);

            return results;
        }

        private List<ValidationResult> checkSheetChange(DataTransferMapping mapping)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            bool bFileExists = true;
            if (mapping == null) return results;

            else
            {
                if (!isSheetsPresent())
                {
                    Microsoft.Office.Interop.Excel.Application inMemoryExcelApp = null;
                    Microsoft.Office.Interop.Excel.Workbooks books = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;

                    try
                    {

                        inMemoryExcelApp = new Microsoft.Office.Interop.Excel.Application();
                        inMemoryExcelApp.Visible = false;
                        books = inMemoryExcelApp.Workbooks;
                        bFileExists = File.Exists(mapping.SourceFile);
                        if (bFileExists)
                            workbook = books.Open(mapping.SourceFile);
                        //Populate Source Worksheet
                        if (workbook != null)
                        {
                            foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in workbook.Worksheets)
                            {
                                if (!sheet.Name.Equals(Apttus.XAuthor.Core.Constants.METADATA_SHEETNAME))
                                    sourceSheets.Add(sheet.Name);
                            }
                        }

                        //Populate Destination Worksheet
                        if (Globals.ThisAddIn.Application.ActiveWorkbook != null)
                        {
                            foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets)
                            {
                                if (!sheet.Name.Equals(Apttus.XAuthor.Core.Constants.METADATA_SHEETNAME))
                                    destSheets.Add(sheet.Name);
                            }
                        }
                    }
                    finally
                    {
                        if (workbook != null)
                        {
                            workbook.Close(false);
                            Marshal.ReleaseComObject(workbook);
                            workbook = null;
                        }
                        if (books != null)
                        {
                            Marshal.ReleaseComObject(books);
                            books = null;
                        }
                        if (inMemoryExcelApp != null)
                        {
                            inMemoryExcelApp.Quit();
                            Marshal.ReleaseComObject(inMemoryExcelApp);
                            inMemoryExcelApp = null;
                        }
                    }
                }

                DataTransferRange[] newModel = new DataTransferRange[mapping.DataTransferRanges.Count];
                mapping.DataTransferRanges.CopyTo(newModel);
                List<string> sourceSheetList = new List<string>();
                List<string> destSheetList = new List<string>();
                foreach (DataTransferRange range in newModel)
                {
                    bool check = false;

                    if (!sourceSheets.Contains(range.SourceSheet))
                    {
                        sourceSheetList.Add(range.SourceSheet);
                        check = true;
                    }
                    if (!destSheets.Contains(range.TargetSheet))
                    {
                        if (mapping.DataTransferRanges.Contains(range))
                        {
                            destSheetList.Add(range.TargetSheet);
                            check = true;
                        }
                    }
                    if (!check && !source.DataTransferRanges.Contains(range)) {
                        if(source.DataTransferRanges == null)
                            source.DataTransferRanges = new List<DataTransferRange>();

                        source.DataTransferRanges.Add(range);
                    };
                }

                if ((sourceSheetList.Count > 0 && bFileExists ) || destSheetList.Count > 0)
                {
                    ValidationResult result = new ValidationResult();
                    result.EntityName = resourceManager.GetResource("RIBBON_btnDataLoader_Label");
                    result.EntityType = EntityType.DataSource;
                    result.Success = false;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(resourceManager.GetResource("DataSourceValidator_checkSheetChange_Msg"));
                    if (sourceSheetList.Count > 0 && bFileExists) stringBuilder.Append(string.Format(resourceManager.GetResource("DataSourceValidator_checkSheetChange__Source_Msg"), string.Join(",", sourceSheetList.ToArray())));
                    if (destSheetList.Count > 0) stringBuilder.Append(string.Format(resourceManager.GetResource("DataSourceValidator_checkSheetChange__Target_Msg"), string.Join(",", destSheetList.ToArray())));
                    if (result.Messages == null) result.Messages = new List<string>();
                    result.Messages.Add(stringBuilder.ToString());
                    results.Add(result);
                }
                source.SourceFile = mapping.SourceFile;
                return results;
            }
        }

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
