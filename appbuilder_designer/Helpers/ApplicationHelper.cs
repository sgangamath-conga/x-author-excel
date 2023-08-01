/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppDesigner.Modules;
using System.IO;

namespace Apttus.XAuthor.AppDesigner
{
    class ApplicationHelper
    {
        private static ApplicationHelper _instance;
        private ConfigurationManager configurationManager;
        private ApttusCommandBarManager commandBar;
        private IXAuthorApplicationController AppController;
        private ApttusResourceManager resourceManager;
        private ApplicationHelper()
        {
            configurationManager = ConfigurationManager.GetInstance;
            commandBar = ApttusCommandBarManager.GetInstance();
            resourceManager = ApttusResourceManager.GetInstance;
            AppController = Globals.ThisAddIn.GetApplicationController();
        }

        public static ApplicationHelper GetInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApplicationHelper();
                return _instance;
            }
        }
        public string EditionType
        {
            get;
            set;
        }

        public void CreateDataMigrationApplication(string AppName, string FilePath, bool bCreateNew)
        {
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;

            // 1. Define and Initialize variable
            Core.Application NewApp = new Core.Application();
            NewApp.Definition.UniqueId = Guid.NewGuid().ToString();
            NewApp.Definition.Name = AppName;
            NewApp.Definition.Version = "1.0";
            NewApp.Definition.Type = ApplicationType.RepeatingCells;

            //Data Migration App will always have the app edition as Power Admin.
            Globals.ThisAddIn.AppDesignerVersion = NewApp.Definition.EditionType = Constants.ADMIN_EDITION;

            if (AppController.CreateApplication(NewApp))
            {
                string sXAFileName = string.Empty;

                if (bCreateNew)
                {
                    sXAFileName = Utils.GetTempFileName(NewApp.Definition.UniqueId, NewApp.Definition.Name + Constants.XLSX);

                    if (System.IO.File.Exists(sXAFileName))
                        System.IO.File.Delete(sXAFileName);

                    // Check if Workbook doesn't exists while creating apps 
                    //if (wb == null)
                    //    wb = Globals.ThisAddIn.Application.Workbooks.Add();

                    if (!ApttusObjectManager.InsertMetaSheet(NewApp, ref wb))
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("APPHELPER_Metadata_ErrorMsg"), resourceManager.GetResource("APPHELPER_MetadataCap_ErrorMsg"));
                        return;
                    }
                    ApttusObjectManager.SaveAs(wb, sXAFileName);
                    ApttusObjectManager.OpenFile(sXAFileName);
                }
                else if (!bCreateNew)
                {
                    // Passs optional parameter for check existing apps
                    bool isExistingApp = true;
                    sXAFileName = Utils.GetTempFileName(NewApp.Definition.UniqueId, NewApp.Definition.Name + Path.GetExtension(FilePath));

                    ApttusObjectManager.OpenFile(FilePath);
                    wb = Globals.ThisAddIn.Application.ActiveWorkbook;

                    ApttusObjectManager.SaveAs(wb, sXAFileName);
                    if (!ApttusObjectManager.InsertMetaSheet(NewApp, ref wb, isExistingApp))
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("APPHELPER_Metadata_ErrorMsg"), resourceManager.GetResource("APPHELPER_MetadataCap_ErrorMsg"));
                        return;
                    }
                }

                ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_DESIGNER_FILE, "true");
                ApttusRibbon.ActivateTab();
                configurationManager.SetApplication(NewApp);
                ApttusCommandBarManager.g_IsAppOpen = true;
                commandBar.EnableMenus();

                // Add newly created application
                Utils.AddApplicationInstance(ApplicationMode.Designer, Globals.ThisAddIn.Application.ActiveWorkbook.Name, false, true);
            }
        }

        public void CreateApplication(string AppName, string FilePath, bool bCreateNew, bool isQuickQpp = false)
        {
            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;

            // 1. Define and Initialize variable
            Core.Application NewApp = new Core.Application();
            NewApp.Definition.UniqueId = Guid.NewGuid().ToString();
            NewApp.Definition.Name = AppName;
            NewApp.Definition.Version = "1.0";
            NewApp.Definition.Type = ApplicationType.RepeatingCells;


            #region setEdition

            if (isQuickQpp)
            {
                NewApp.Definition.EditionType = Constants.BASIC_EDITION;
            }
            else if (!string.IsNullOrEmpty(EditionType))
            {
                NewApp.Definition.EditionType = EditionType;
            }
            else // should never get here. default to basic and may wanto to log an exception. 
            {
                ExceptionLogHelper.DebugLog("Application Edition type is unknown for app " + AppName);
                NewApp.Definition.EditionType = Constants.BASIC_EDITION;
            }
            Globals.ThisAddIn.AppDesignerVersion = NewApp.Definition.EditionType;

            #endregion


            if (AppController.CreateApplication(NewApp))
            {
                string sXAFileName = string.Empty;

                if (bCreateNew)
                {
                    sXAFileName = Utils.GetTempFileName(NewApp.Definition.UniqueId, NewApp.Definition.Name + Constants.XLSX);

                    if (System.IO.File.Exists(sXAFileName))
                        System.IO.File.Delete(sXAFileName);

                    // Check if Workbook doesn't exists while creating apps 
                    //if (wb == null)
                    //    wb = Globals.ThisAddIn.Application.Workbooks.Add();

                    if (!ApttusObjectManager.InsertMetaSheet(NewApp, ref wb))
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("APPHELPER_Metadata_ErrorMsg"), resourceManager.GetResource("APPHELPER_MetadataCap_ErrorMsg"));
                        return;
                    }
                    ApttusObjectManager.SaveAs(wb, sXAFileName);
                    ApttusObjectManager.OpenFile(sXAFileName);
                }
                else if (!bCreateNew)
                {
                    // Passs optional parameter for check existing apps
                    bool isExistingApp = true;
                    sXAFileName = Utils.GetTempFileName(NewApp.Definition.UniqueId, NewApp.Definition.Name + Path.GetExtension(FilePath));

                    ApttusObjectManager.OpenFile(FilePath);
                    wb = Globals.ThisAddIn.Application.ActiveWorkbook;

                    ApttusObjectManager.SaveAs(wb, sXAFileName);
                    if (!ApttusObjectManager.InsertMetaSheet(NewApp, ref wb, isExistingApp))
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("APPHELPER_Metadata_ErrorMsg"), resourceManager.GetResource("APPHELPER_MetadataCap_ErrorMsg"));
                        return;
                    }
                }

                ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_DESIGNER_FILE, "true");
                ApttusRibbon.ActivateTab(isQuickQpp);
                configurationManager.SetApplication(NewApp);
                ApttusCommandBarManager.g_IsAppOpen = true;
                commandBar.EnableMenus();

                // Add newly created application
                Utils.AddApplicationInstance(ApplicationMode.Designer, Globals.ThisAddIn.Application.ActiveWorkbook.Name, false, isQuickQpp);
            }
        }

        /// <summary>
        /// Get Template Name
        /// </summary>
        /// <returns></returns>
        private string GetTemplateName()
        {
            string extension = string.Empty;
            try
            {
                //get the name of the active document
                ApttusCommandBarManager commandBarTemplate = ApttusCommandBarManager.GetInstance();
                string activeWbPath = commandBarTemplate.GetActiveWorkbookName();
                // Get extension
                extension = Path.GetExtension(activeWbPath);
                //if (configurationManager.Definition.UniqueId != null)
                //{
                //    extension = Utils.GetExtensionsfromDirectory(Utils.GetApplicationPath(configurationManager.Definition.UniqueId));
                //}
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Error");
            }
            return Constants.TEMPLATENAME + extension;
        }

        /// <summary>
        /// Saves the config xml and excel template in salesforce. Google flag is only set to true when using a quick app.
        /// </summary>
        /// <param name="IsMacroExists"></param>
        /// <param name="Extension"></param>
        public void SaveApplication(bool IsMacroExists, string Extension, bool saveForGoogle = false)
        {
            try
            {
                ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_DESIGNER_FILE);
                byte[] xlsxTemplate = ApttusObjectManager.EncodeByteArray(IsMacroExists);

                byte[] googleSheetSchema = null;
                try
                {
                    if (saveForGoogle)
                    {
                        googleSheetSchema = new byte[0];
                        googleSheetSchema = new GoogleSchemaUtil().GetGoogleSchema();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }

                byte[] prestoJSONSchema = GetPrestoSchemaJSON();
                EditionType = ConfigurationManager.GetInstance.Application.Definition.EditionType;
                AppController.Save(xlsxTemplate, GetTemplateName(), prestoJSONSchema, googleSheetSchema, EditionType, IsMacroExists, Extension);
            }
            finally
            {
                ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_DESIGNER_FILE, "true");
            }
        }

        internal byte[] GetPrestoSchemaJSON()
        {
            PrestoSchema prestoSchema = new PrestoSchema();
            return prestoSchema.GeneratePrestoSchema();
        }
    }                
}
