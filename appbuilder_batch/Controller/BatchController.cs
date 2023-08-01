/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime;
using System.Xml.Linq;
using Apttus.XAuthor.AppRuntime.Modules;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;

namespace Apttus.XAuthor.Batch
{
    public class BatchController : IXAuthorSalesforceLogin
    {
        private static BatchController instance;
        private static object syncRoot = new Object();

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
        FileInfo templateFileInfo;

        ObjectManager objectManager;
        Excel.Application ExcelApp = null;
        Excel.Workbook ExcelWorkbook = null;
        ApplicationObject currentApp = null;
        Logger Extloger = null; // Logger passed from MS
        int WaitTime = 0;
        private string SFAccessToken;
        private string SFInstanceURL;

        private BatchController()
        {
            InitObjectManager();
        }
        public bool Connect(string token, string instanceURL, object logger)
        {
            SFAccessToken = token;

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(instanceURL))
            {
                if (logger != null)
                {
                    Extloger = new Logger(logger);
#if DEBUG
                    Extloger.Info("In the Connect");
#endif
                }
#if DEBUG
                Extloger.Info("instanceURL from APEX : " + instanceURL);
#endif

                string parterAPIVersion = objectManager.GetPartnerAPIVersion();

                //Regex instanceURLRegex = new Regex("https://[^/?]+\\.(sales|visual\\.)force\\.com/services/(S|s)(O|o)(A|a)(P|p)/(u|c)/.*");
                Regex instanceURLRegex = new Regex("https://[^/?]+\\.(sales|visual\\.)force\\.com/services/(S|s)(O|o)(A|a)(P|p)/(u|c)/(\\d+(\\.\\d{1,2})?)/.*");
                Match URLmatch = instanceURLRegex.Match(instanceURL);
                if (URLmatch.Success)
                {
                    string apexAPIVersion = string.Empty;
                    if (URLmatch.Groups.Count > 7)
                        apexAPIVersion = URLmatch.Groups[7].Value;

                    if (apexAPIVersion != parterAPIVersion)
                        instanceURL = instanceURL.Replace(apexAPIVersion, parterAPIVersion);
                }

                if (instanceURL.IndexOf("/services/") > 0)
                {
                    instanceURL = instanceURL.Substring(0, instanceURL.IndexOf("/services/"));
                }

#if DEBUG
                Extloger.Info("instanceURL After Connect : " + instanceURL);
#endif
                SFInstanceURL = instanceURL;

                //return objectManager.Connect(token, instanceURL, null);
                return objectManager.Connect(this);
            }

            return false;
        }

        public static BatchController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new BatchController();
                    }
                }
                return instance;
            }
        }

        public string AccessToken { get { return SFAccessToken; } }

        public string InstanceURL { get { return SFInstanceURL; } }

        public IWebProxy Proxy { get { return null; } }

        public BatchResponse Execute(BatchRequest request)
        {
            BatchResponse response = new BatchResponse { Success = false, ErrorMessage = string.Empty };
            FileInfo OutputFileInfo = null;
            string output = null;
            bool bExcelReleased = false;

            try
            {
                // Initialize X-Author Runtime for Batch Mode
                InitializeXAuthorBatch(request);
#if DEBUG
                Extloger.Info("Ouput Path" + request.OutputPath + Environment.NewLine + "Template Name" + request.TemplateName);
#endif

                currentApp = new ApplicationObject
                {
                    TemplateName = request.TemplateName,
                    Config = request.Config,
                    Template = request.Template
                };

                // Assign Application and Config objects.
                configurationManager.AssignConfig(currentApp.Config);
                // Save the Template and Config to local path
                OutputFileInfo = LoadApplication(currentApp, request.OutputPath);

                if (OutputFileInfo != null)
                {
                    DataManager dataManager = DataManager.GetInstance;
                    dataManager.AddJsonData(request.InputData);
                    // Add App instance , for highlighting purpose
                    Utils.AddApplicationInstance(ApplicationMode.Runtime, request.TemplateName, true);
                    // Execute App Load Workflow
                    commandBarManager.DoPostLoad(null, false);
#if DEBUG
                    Extloger.Info("Action flow completed. Now entering clean-up routines...");
#endif
                    // Save and Close Output Workbook
                    output = SaveAndCloseWorkbook(request.OutputType, request.OutputPath);
#if DEBUG
                    Extloger.Info("After Excel/PDF/None " + output);
#endif
                    ReleaseExcel();
                    bExcelReleased = true;
#if DEBUG
                    Extloger.Info("Released Excel...");
#endif
                    System.Threading.Thread.Sleep(WaitTime);
                    // Create Success Response
                    // 1. Add Output Excel byte array to Response
                    // 2. Add Output Excel Name to Response
                    response.Success = true;
#if DEBUG
                    Extloger.Info("Wait time over before stream to file");
#endif
                    response.OutputExcelFile = Utils.FileToBytes(output); //OutputFileInfo.FullName);
#if DEBUG
                    Extloger.Info("After stream to file." + Environment.NewLine + "************ Presto Execution Complete ************");
#endif
                    response.OutputExcelFileName = output;
                }
                else
                {
                    response.ErrorMessage = "Application template load failed.";
                }
            }
            catch (Exception ex)
            {
                // Log Exception as a part of Batch Response
                string exceptionMessage = "---- Exception handled gracefully within Catch block of Execute method ----"
                    + "Error Message: " + ex.Message + Environment.NewLine + ex.StackTrace;
                response.ErrorMessage = exceptionMessage;
                Extloger.Error(exceptionMessage);
            }
            finally
            {
                try
                {
                    // Cleanup Excel Application and Excel File
                    objectManager.ExcelApp = null;
                    if (OutputFileInfo != null)
                    {
                        Cleanup(OutputFileInfo.FullName);
                        Cleanup(output);
                        if (request.OutputType.StartsWith("PDF"))
                        {
                            // Another temp xlsx file is created for PDF  and need to delete it
                            string FileWithoutExt = Path.GetDirectoryName(output) + "\\" + Path.GetFileNameWithoutExtension(output);

                            string Ext = Path.GetExtension(templateFileInfo.FullName);

                            Cleanup(FileWithoutExt + Ext);
                        }
                    }
                    else
                        Cleanup(string.Empty);

                    // Release Excel.exe always in finally
                    if (response.ErrorMessage.Contains("Exception Occured"))
                        SaveAndCloseWorkbook(request.OutputType, request.OutputPath);

                    if (!bExcelReleased)
                        ReleaseExcel();
                }
                catch (Exception finallyEx)
                {
                    Extloger.Error("---- Exception handled gracefully within Finally block of Execute method ----"
                        + "Error Message: " + finallyEx.Message + Environment.NewLine + finallyEx.StackTrace);
                }
            }
            return response;
        }

        private void InitObjectManager()
        {
            objectManager = ObjectManager.GetInstance;
            ObjectManager.SetCRM(CRM.Salesforce);
            ObjectManager.RuntimeMode = RuntimeMode.Batch;

        }
        private void InitializeXAuthorBatch(BatchRequest request)
        {

            ApttusMessageUtil.SuppressMessages = true;
            ExcelHelper.ExcelApp = ExcelApp = new Excel.Application();
            objectManager.ExcelApp = ExcelApp;
            ExceptionLogHelper.GetInstance(request.OutputPath, Core.Constants.XAUTHORBATCH_LOG_NAME);
            if (Extloger != null)
            {
                Extloger.Info("Started export");
                ExceptionLogHelper.AssignExternalLog(Extloger);

            }

            ExceptionLogHelper.TraceLevel = "Debug";            
        }

        private FileInfo LoadApplication(ApplicationObject app, string OutputPath)
        {
            string filePath = string.Empty;
            string extension = string.Empty;
            templateFileInfo = null;

            if (app != null)
            {
                filePath = OutputPath + Path.DirectorySeparatorChar + configurationManager.Definition.Name + "-Runtime" + Apttus.XAuthor.Core.Constants.HYPHEN + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                extension = Path.GetExtension(app.TemplateName);
                templateFileInfo = new FileInfo(filePath + extension);
                // Write Excel to local path.
                Utils.StreamToXlsx(app.Template, templateFileInfo.FullName);
                // Open Excel file
                ExcelWorkbook = ExcelApp.Workbooks.Open(
                    templateFileInfo.FullName, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing);
            }
            return templateFileInfo;
        }

        private string SaveAndCloseWorkbook(string outputType, string OutputPath)
        {
            // Save and Close Excel Workbook
            bool savechanges = false;
            string outputPath = string.Empty;
            Excel.Application oApp = null;
            try
            {
                ExcelWorkbook.Save();
                savechanges = false;
                string s = templateFileInfo.DirectoryName;
                oApp = ExcelWorkbook.Application;
                ExcelWorkbook.Application.DisplayAlerts = false;
                string FileWithoutExt;
                string FullPath;
                if (outputType != null && outputType.StartsWith("PDF"))
                {

                    string FilePath = OutputPath + Path.DirectorySeparatorChar + configurationManager.Definition.Name + "-Runtime" + Apttus.XAuthor.Core.Constants.HYPHEN + DateTime.Now.ToString("yyyyMMddHHmmss");

#if DEBUG
                    Extloger.Info("Inside SaveAndCloseWorkbook, File path is " + FilePath);
#endif
                    string Ext = Path.GetExtension(templateFileInfo.FullName);
                    FilePath += Ext;

#if DEBUG
                    Extloger.Info("Inside SaveAndCloseWorkbook, before save as " + FilePath);
#endif
                    // For some reason Merger server require this step...
                    ExcelHelper.SaveAs(ExcelWorkbook, FilePath);

#if DEBUG
                    Extloger.Info("Inside SaveAndCloseWorkbook, after save as " + FilePath);
#endif

                    FileWithoutExt = Path.GetFileNameWithoutExtension(FilePath);
                    FullPath = templateFileInfo.DirectoryName + "\\" + FileWithoutExt + "." + outputType;

#if DEBUG
                    Extloger.Info("Inside SaveAndCloseWorkbook, before pdf " + FullPath);
#endif

                    ExcelWorkbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, FullPath,
                        Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    outputPath = FullPath;
#if DEBUG
                    Extloger.Info("Inside SaveAndCloseWorkbook, after pdf " + FullPath);
#endif
                    WaitTime = 1000;

                }
                else
                    outputPath = templateFileInfo.FullName;
            }
            catch (Exception ex)
            {
                Extloger.Error("---- Exception handled gracefully within Catch block of SaveAndCloseWorkbook method ----"
                    + "Error Message: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
#if DEBUG
                Extloger.Info("Inside SaveAndCloseWorkbook (Finally block), Before Close Workbook");
#endif
                bool bWorkbookCloseErrored = false; string WorkbookCloseError = string.Empty;
                bool bExcelAppErrored = false; string ExcelAppError = string.Empty;

                try
                {
                    ExcelWorkbook.Close(savechanges, Type.Missing, Type.Missing);
                }
                catch (Exception finallyEx)
                {
                    bWorkbookCloseErrored = true;
                    WorkbookCloseError = finallyEx.Message;
                }

#if DEBUG
                Extloger.Info("Inside SaveAndCloseWorkbook (Finally block), After Close Workbook");
#endif
                try
                {
                    oApp.DisplayAlerts = true;
                    oApp.Quit();
                }
                catch (Exception finallyEx)
                {
                    bExcelAppErrored = true;
                    ExcelAppError = finallyEx.Message;
                }
#if DEBUG
                Extloger.Info("Inside SaveAndCloseWorkbook (Finally block), After Quitting Excel Application ");
#endif
                if (Extloger != null)
                {
                    StringBuilder sbMessage = new StringBuilder("-------------- SaveAndCloseWorkbook Complete --------------");

                    if (bWorkbookCloseErrored)
                        sbMessage.AppendLine("---- Exception handled gracefully within Finally block of SaveAndCloseWorkbook method while trying to close Excel Workbook ----"
                            + Environment.NewLine + "Error Message: " + WorkbookCloseError);

                    if (bExcelAppErrored)
                        sbMessage.AppendLine("---- Exception handled gracefully within Finally block of SaveAndCloseWorkbook method while trying to quit Excel application ----"
                            + Environment.NewLine + "Error Message: " + ExcelAppError);

                    if (bWorkbookCloseErrored || bExcelAppErrored)
                        Extloger.Error(sbMessage.ToString());
                    else
                        Extloger.Info(sbMessage.ToString());
                }
            }

            return outputPath;
        }

        private void Cleanup(string excelFilePath)
        {
            try
            {

                // Clean up WIP Excel File
                if (File.Exists(excelFilePath))
                    File.Delete(excelFilePath);
            }
            catch (Exception ex)
            {
                // File may be locked so manual clean up will be required. Log error.
                Extloger.Error("---- Exception handled gracefully within Cleanup method ----"
                    + "Error Message: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private void ReleaseExcel()
        {
            try
            {
                // Clean up Excel application
                ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);

            }
            catch (Exception ex)
            {
                // File may be locked so manual clean up will be required. Log error.
                Extloger.Error("---- Exception handled gracefully within ReleaseExcel method ----"
                    + "Error Message: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
