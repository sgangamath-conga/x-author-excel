/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;


namespace Apttus.XAuthor.AppDesigner
{
    public class ImportAppController
    {
        private ObjectManager objectManager = ObjectManager.GetInstance;
        private ImportAppView View;
        private BaseApplicationController AppController;
        string ImportDirectory = Path.GetTempPath() + "Import_Directory";
        Application application = null;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

        public ImportAppController(ImportAppView view)
        {
            AppController = Globals.ThisAddIn.GetApplicationController();
            this.View = view;
            view.SetController(this);
            this.View.ShowDialog();
        }

        public void ExtractApp(string AppFileName)
        {
            if (!string.IsNullOrEmpty(AppFileName))
            {
                // 1. Reset import directory and local variables.
                PrepareForExtract();

                // 2. Extract App to import directory
                if (AppController.ExtractApp(AppFileName, ImportDirectory))
                {
                    // 3. Load Application object
                    DirectoryInfo di = new DirectoryInfo(ImportDirectory);
                    FileInfo[] ConfigFiles = di.GetFiles("*.xml");
                    FileInfo[] Externalschema = di.GetFiles("*.json");
                    string schemaFile = null;
                    if (Externalschema != null && Externalschema.Length == 1)
                    {
                        schemaFile = File.ReadAllText(ConfigFiles[0].FullName);
                    }
                    if (ConfigFiles != null && ConfigFiles.Length == 1)
                    {
                        // 4. If there is exactly one .xml file then it is assumed to be the config file.
                        string configXml = File.ReadAllText(ConfigFiles[0].FullName);
                        application = ApttusXmlSerializerUtil.Deserialize<Application>(EncryptionHelper.Decrypt(configXml));
                        
                        // 5. Deserialize the schema File
                        if (schemaFile != null && schemaFile.Length > 0)
                        {
                            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                            application.Definition.Schema = encoding.GetBytes(schemaFile);
                        }
                        // 5. Populate View with App Name and Version
                        View.PopulateAppDetails(application.Definition.Name, application.Definition.Version, AppFileName);
                    }
                }
            }
        }

        private void PrepareForExtract()
        {
            // 1. Create Import Directory for every imported app.
            if (Directory.Exists(ImportDirectory))
                Utils.CreateOrClearDirectory(ImportDirectory);
            else
                Directory.CreateDirectory(ImportDirectory);

            // 2. Clear any Application object from any previous file
            application = null;
        }

        public void ImportApp(string AppName, string AppVersion)
        {
            if (application == null)
                return;
            ImportApp(application, AppName, AppVersion, ImportDirectory);
        }

        public void ClearImportDirectory()
        {
            // Clear Import Directory
            if (Directory.Exists(ImportDirectory))
            {
                Utils.CreateOrClearDirectory(ImportDirectory);
                Directory.Delete(ImportDirectory);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application">Application Object</param>
        /// <param name="appImportName">New Proposed App Name</param>
        /// <param name="appVersion">App Version</param>
        /// <param name="importDirectory">Directory in which the App is stored locally</param>
        /// <returns></returns>
        private bool ImportApp(Application application, string appImportName, string appVersion, string importDirectory)
        {
            try
            {

                //set the edition .
                if (string.IsNullOrEmpty(application.Definition.EditionType))
                {
                    if (LicenseActivationManager.GetInstance.DoesOrgHasAdminPackageInstalled) // admin package installed default to Power Admin
                    {
                        application.Definition.EditionType = LicenseActivationManager.GetInstance.SetEditionForApps(null);
                    }
                }

                bool bResult = false;
                String originalAppName = application.Definition.Name;
                string prestoSchemaFilePath = Directory.GetFiles(importDirectory).Where(s => Path.GetExtension(s).Contains("json")).FirstOrDefault();
                String xmlFile = new StringBuilder(importDirectory).Append(Path.DirectorySeparatorChar).Append(originalAppName).Append(".xml").ToString();
                String templateFile = Directory.GetFiles(importDirectory).Where(s => Path.GetExtension(s).Contains("XLS")).FirstOrDefault();

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                string json = "";
                try
                {
                    json = File.ReadAllText(prestoSchemaFilePath);
                }                
                catch (Exception e) //json file does not exist or cannot access
                {                    
                }
                Byte[] schema = encoding.GetBytes(json); //get the presto schema file as byte

                if (templateFile == null)
                    templateFile = Directory.GetFiles(importDirectory).Where(s => Path.GetExtension(s).Contains("xls")).FirstOrDefault();

                if (String.IsNullOrEmpty(xmlFile))
                    throw new Exception(resourceManager.GetResource("IMPORTAPPCTL_ImportApp_ErrMsg"));

                if (String.IsNullOrEmpty(templateFile))
                    throw new Exception(resourceManager.GetResource("IMPORTAPPCTL_ImportAppTemplate_ErrMsg"));
                /*************************************************/
                // 1. Check if Name and Unique Id exists
                //string query = AppController.APP_UNIQUENAMEANDID_CHECK_FIRSTPART + Constants.QUOTE + appImportName + Constants.QUOTE
                //+ AppController.APP_UNIQUENAMEANDID_CHECK_SECONDPART + Constants.QUOTE + application.Definition.UniqueId + Constants.QUOTE;
                //var App = objectManager.QueryDataSet(query);

                var App = AppController.GetAppbyNameOrUniqueId(appImportName, application.Definition.UniqueId);

                if (App != null && App.DataTable.Rows.Count > 0)
                {
                    string AppObjectUniqueIdAttr = AppController.GetApplicationObjectUniqueIdAttribute();
                    string AppObjectIdAttribute = AppController.GetApplicationObjectIdAttribute();
                    string AppObjectNameAttribute = AppController.GetApplicationObjectNameAttribute();

                    foreach (DataRow dr in App.DataTable.Rows)
                    {
                        string OverwriteMessage = string.Format(resourceManager.GetResource("IMPORTAPPCTL_ImportApp_WarnMsg"), appImportName);
                        // Scenario 1: Name matches and Unique Id matches
                        if (dr[AppObjectNameAttribute].ToString() == appImportName && dr[AppObjectUniqueIdAttr].ToString() == application.Definition.UniqueId)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, resourceManager.GetResource("IMPORTAPPCTL_ImportAppCap_WarnMsg"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = AppController.OverrideApplication(application, templateFile, appImportName, dr[AppObjectIdAttribute].ToString(), dr[AppObjectUniqueIdAttr].ToString(), schema);
                            }
                        }
                        // Scenario 2: Name matches and but Unique Id does not match
                        else if (dr[AppObjectNameAttribute].ToString() == appImportName)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, resourceManager.GetResource("IMPORTAPPCTL_ImportAppCap_WarnMsg"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = AppController.OverrideApplication(application, templateFile, appImportName, dr[AppObjectIdAttribute].ToString(), application.Definition.UniqueId, schema);
                            }
                        }
                        // Scenario 3: Name does not match but Unique Id matches
                        else if (dr[AppObjectNameAttribute].ToString() != appImportName && dr[AppObjectUniqueIdAttr].ToString() == application.Definition.UniqueId)
                        {
                            // Generate new Unique Id and Create the app
                            application.Definition.Name = appImportName;
                            string NewUniqueId = Guid.NewGuid().ToString();
                            application.Definition.UniqueId = NewUniqueId;

                            Excel.Application ExcelApp = new Excel.Application();
                            Excel.Workbook ExcelWorkbook = ExcelApp.Workbooks.Open(templateFile, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                            foreach (Excel.Worksheet oSheet in ExcelWorkbook.Worksheets)
                                if (oSheet.Name.Equals(Constants.METADATA_SHEETNAME))
                                {
                                    Excel.Range oRange = oSheet.Cells[1, 1];
                                    oRange.Value2 = NewUniqueId;
                                    ExcelWorkbook.Save();
                                    break;
                                }
                                                        
                            ExcelWorkbook.Close();
                            bResult = AppController.CreateAppAndSaveAttachments(application, templateFile, appImportName, schema);
                        }
                        break;
                    }
                }
                // Scenario 4: Name and Unique Id do not match
                else if (App != null && App.DataTable.Rows.Count == 0)
                {
                    application.Definition.Name = appImportName;
                    bResult = AppController.CreateAppAndSaveAttachments(application, templateFile, appImportName, schema);
                }

                if (bResult)
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("IMPORTAPPCTL_ImportApp_InfoMsg"), resourceManager.GetResource("IMPORTAPPCTL_ImportAppCAP_InfoMsg"));
                return bResult;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, "Import App Error");
                return false;
            }
        }
    }
}
