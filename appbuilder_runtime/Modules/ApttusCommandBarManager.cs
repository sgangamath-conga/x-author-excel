/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.View;
using Excel = Microsoft.Office.Interop.Excel;
using System.ComponentModel;

namespace Apttus.XAuthor.AppRuntime.Modules
{
    public class ApttusCommandBarManager
    {
        private IXAuthorRibbonLogin xauthorLogin;

        //static singleton instance
        private static ApttusCommandBarManager instance = null;

        public static OptionsForm OptionsForm = null;

        public static string g_UserName = string.Empty;
        public static bool g_IsLoggedIn = false;
        public static bool g_IsSharingActiveSheet = false;
        public static bool bCreatePDF = false;
        public static bool g_IsBasicEdition = false;
        public static bool g_IsUserClosingRuntimeApp = false;
        // Since the file name is going to be the same for all edit in excel for the same app
        // users needs to close the app before calling edit in excel again for the app.
        // set this flag before calling edit in exel routine and then reset it back so that
        // regular apps will not have the datetime on the template.
        public static bool EditInExcelMode = false;

        public StartupParameters StartupParameters = new StartupParameters();

        private ApplicationSyncController appSyncController;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private BackgroundWorker AppSyncBackgroundWorker = null;
        internal object g_PickListSynchLock = new object();

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        /// <returns></returns>
        public static ApttusCommandBarManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ApttusCommandBarManager();
            }
            return instance;
        }

        public ApttusUserInfo GetUserInfo()
        {
            return xauthorLogin.UserInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLoggedIn()
        {
            return g_IsLoggedIn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsBasicEdition()
        {
            return g_IsBasicEdition;
        }

        public bool IsUserClosingRuntimeApp()
        {
            return g_IsUserClosingRuntimeApp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAppLoading()
        {
            //return Convert.ToBoolean(ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.IsAppLoading));
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.IsAppLoading))
            {
                string isAppLoading = Convert.ToString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.IsAppLoading]);
                if (!string.IsNullOrEmpty(isAppLoading))
                    return Convert.ToBoolean(isAppLoading);
            }
            return false;
        }

        /// <summary>
        /// This method will be called for the edit to excel directly from SFDC UI
        /// </summary>


        internal void InitializeCRM()
        {
            xauthorLogin = Globals.ThisAddIn.GetLoginObject();

            xauthorLogin.OnConnected += xauthorLogin_OnConnected;
            xauthorLogin.OnLogout += xauthorLogin_OnLogout;
        }

        void xauthorLogin_OnLogout(object sender, EventArgs e)
        {
            //EnableMenus();
        }

        /// <summary>
        /// Once the user is connected to crm, we can perform any post login actions, such as enabling Ribbon menus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xauthorLogin_OnConnected(object sender, EventArgs e)
        {
            Constants.CURRENT_USER_ID = xauthorLogin.UserInfo.UserId;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowLoginForm(Ribbon ribbon)
        {
            xauthorLogin.Login();            
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowManageConnection(Ribbon ribbon)
        {
            xauthorLogin.ShowManageConnection();           
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowOptions()
        {
            OptionsForm = new OptionsForm();
            OptionsForm.ShowDialog();
            OptionsForm.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoAboutBox()
        {
            AboutForm AboutBox = new AboutForm();
            AboutBox.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoLogout(bool showBalloonMessage)
        {
            try
            {
                xauthorLogin.DoLogout(showBalloonMessage);
            }
            catch (Exception ex)
            {
                ex.Source = "LOGOFF";
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Logout");
            }
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        /// <returns></returns>
        public static ApttusCommandBarManager GetInstanceManager()
        {
            if (instance == null)
            {
                instance = new ApttusCommandBarManager();
            }
            return instance;
        }

        internal object GetActiveWorkbookFullName()
        {

            try
            {
                return Globals.ThisAddIn.Application.ActiveWorkbook.FullNameURLEncoded;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetActivWorkbookName()
        {
            try
            {
                return ExcelHelper.ExcelApp.ActiveWorkbook.FullName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save and open app template
        /// </summary>
        /// <param name="appObject">Application for loaded app</param>
        public bool OpenTemplate(ApplicationObject appObject)
        {
            bool templateOpened = false;
            try
            {
                ConfigurationManager configManager = ConfigurationManager.GetInstance;
                //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.IsAppLoading, "true");
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.IsAppLoading] = "true";
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();

                string AppName = configManager.Application.Definition.Name;
                string filePath;
                string fileName;
                if (ApttusCommandBarManager.EditInExcelMode)
                {
                    fileName = AppName + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-Runtime";
                    filePath = Utils.GetTempFileName(configManager.Definition.UniqueId, fileName);
                    configManager.Application.EditInExcelAppUniquFileId = filePath;
                }
                else
                {
                    configManager.Application.EditInExcelAppUniquFileId = null; // re-set the Edit in Excel file. 
                    fileName = AppName + "-Runtime";
                    filePath = Utils.GetTempFileName(configManager.Definition.UniqueId, fileName);
                }

                Utils.StreamToFile(appObject.Config, filePath + Constants.XML);

                // Set extension in write file
                string extension = Path.GetExtension(appObject.TemplateName);
                Microsoft.Office.Interop.Excel.Workbook workbook = ExcelHelper.CheckIfFileOpened(AppName + "-Runtime" + extension);
                double currentMajorVersion = Convert.ToDouble(configManager.Application.Definition.DesignerVersion.Substring(0, 3), System.Globalization.CultureInfo.InvariantCulture);

                if (workbook == null)
                {
                    // Save Excel Template to local path
                    Utils.StreamToXlsx(appObject.Template, filePath + extension);

                    // From v3.7 Template.xlsx file in SFDC will be saved without any flag, so on Runtime add Runtime flag on Designer add Designer flag
                    // For Backward compatibility, i.e. for version <= 3.5, do remove Designer flag
                    // Once App is saved with v3.7, removeDesignerflag won't execute, this will give us desired performance for Edit in Excel use case.
                    if (currentMajorVersion <= Constants.VERSION_TILL_DESIGNER_FLAG_IN_TEMPLATE)
                        ApttusObjectManager.RemoveDesignerFlag(filePath + extension);

                    // Add Current application instance
                    Utils.AddApplicationInstance(ApplicationMode.Runtime, fileName + extension);

                    // Open template file with set extension
                    ApttusObjectManager.OpenFile(filePath + extension);

                    templateOpened = true;
                }
                else
                {
                    //ApttusMessageUtil.ShowError("Application " + workbook.Name + " is already open." + Environment.NewLine + "Please close the file and reopen the application", "Application Open");
                    Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("APTTUSCMDBARMAN_OpenTemplate_WarnMsg"), workbook.Name), resourceManager.GetResource("APTTUSCMDBARMAN_OpenTemplateCAP_WarnMsg"), ApttusMessageUtil.YesNo);
                    if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
                    {
                        g_IsUserClosingRuntimeApp = true;
                        workbook.Close(false);

                        // Save Excel Template to local path
                        Utils.StreamToXlsx(appObject.Template, filePath + extension);

                        // Now Template.xlsx file in SFDC is always saved without any flag, so on Runtime add Runtime flag on Designer add Designer flag
                        if (currentMajorVersion <= Constants.VERSION_TILL_DESIGNER_FLAG_IN_TEMPLATE)
                            ApttusObjectManager.RemoveDesignerFlag(filePath + extension);

                        // Assign Config again
                        configManager.AssignConfig(appObject.Config);

                        // Add Current application instance
                        Utils.AddApplicationInstance(ApplicationMode.Runtime, fileName + extension);

                        // Open template file with set extension
                        ApttusObjectManager.OpenFile(filePath + extension);

                        g_IsUserClosingRuntimeApp = false;
                        templateOpened = true;
                    }
                    else if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No)
                    {
                        // Get application Unique Id
                        MetadataManager metadataManager = MetadataManager.GetInstance;
                        string appUniqueId = metadataManager.GetAppUniqueId();

                        // Get Application instance
                        //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
                        ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);

                        // Set Application instance
                        Utils.SetApplicationInstance(appInstance);

                        templateOpened = false;
                    }
                }

                return templateOpened;
            }
            finally
            {
                //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.IsAppLoading, "false");
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.IsAppLoading] = "false";
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Common entry point for Add-in and Batch modes
        /// </summary>
        /// <param name="ribbon">Ribbon reference</param>
        public void DoPostLoad(Ribbon ribbon, bool bOfflineToOnline)
        {
            // Perform AppSync in 2 parts 
            // 1. Sync Config to Metadata
            // 2. Prepare Excel file to apply data validation on Independent cells
            // 3. Sync Salesforce asynchronously to Metadata
            appSyncController = new ApplicationSyncController();
            appSyncController.WritePicklistData(false, null);
            appSyncController.WritePicklistNamedRangePair();

            appSyncController.CreateObjectsWithoutLookupName();

            if (!bOfflineToOnline)
            {
                //foreach (Excel.Worksheet oSheet in ExcelHelper.ExcelApp.Worksheets)
                //{
                //    if (ExcelHelper.IsUserSheetProtection(oSheet))
                //        ExcelHelper.UserSheetProtection(oSheet, false);
                //}

                ExcelHelper.UpdateApplicableSheetsForUserProtection(false);

                // there are cases where the following code throw exception
                // and menu's wont appear. adding a try catch so that users could atleaset
                // open the app and work . The worst scenario is, picklists may not have the correct values
                // and there will be log if that happens
                try
                {
                    // Executed in both Batch and Addin mode
                    appSyncController.PrepRuntimeExcel();

                    // Set RichText Edit available or not from License Manager
                    //ConfigurationManager.GetInstance.Application.Definition.AppSettings.DisableRichTextEditing = !LicenseActivationManager.GetInstance.IsFeatureAvailable(Constants.RICH_TEXT_EDIT_TAG);
                }
                catch (Exception ex)
                {
                    RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, "PrepRuntimeExcel");
                }

                //foreach (Excel.Worksheet oSheet in ExcelHelper.ExcelApp.Worksheets)
                //{
                //    ExcelHelper.UserSheetProtection(oSheet, true);
                //}

                ExcelHelper.UpdateApplicableSheetsForUserProtection(true);
            }

            // Initialize Background Worker for App Sync
            InitializeBackgroundWorker();

            if (ObjectManager.RuntimeMode != RuntimeMode.Batch && ribbon != null)
            {
                ribbon.ActivateTab("tabAppBuilderRuntime");

                // Build menus from here when App is being launched from Apps Menu or Open App UI
                MenuBuilder MenuBuilder = new MenuBuilder();
                MenuBuilder.Config = ConfigurationManager.GetInstance;
                MenuBuilder.InvalidateMenus(ribbon, true);
                MenuBuilder.BuildMenusFromXml(ribbon);

                //Load All External Libraries
                CopyExternalLibraries();
            }

            // Triggers
            //MetadataManager metadataManager = MetadataManager.GetInstance;
            //if (metadataManager.FirstLaunch == true)
            //{
            //    metadataManager.FirstLaunch = false;
            //    executeFirstAppLoadTrigger();
            //}
            executeAppLoadTrigger();

            // To save Ribbon state X-Author now overrides Excel Save, Don't save, cancel dialog with it's custom dialog.
            // But after picklist sync file has become dirty so, mark workbook as saved to prevent that
            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                Globals.ThisAddIn.Application.ActiveWorkbook.Saved = true;
        }
        /// <summary>
        /// Initializes the background worker for App Sync
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            AppSyncBackgroundWorker = new BackgroundWorker();
            // Attach event handlers to the BackgroundWorker object.
            AppSyncBackgroundWorker.DoWork +=
                new DoWorkEventHandler(AppSyncBackgroundWorker_DoWork);
            AppSyncBackgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(AppSyncBackgroundWorker_RunWorkerCompleted);

            AppSyncBackgroundWorker.RunWorkerAsync();

            Thread.Sleep(1); //Explictly make the context switch.
        }

        /// <summary>
        /// When  background worker is done with app sync, It porceedes to notify user whether any change is found or not in any field's configuration.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppSyncBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bool blnConfigChanged = (bool)e.Result;
                // If Configuration change (Datatype change or field delete) is found, it will show ballon tip with message in context menu strip
                if (blnConfigChanged)
                {
                    if (Globals.ThisAddIn != null)
                        Globals.ThisAddIn.NotifyDataTypeChange();
                }
                AppSyncBackgroundWorker.Dispose();
                AppSyncBackgroundWorker = null;

                // To save Ribbon state X-Author now overrides Excel Save, Don't save, cancel dialog with it's custom dialog.
                // But after picklist sync file has become dirty so, mark workbook as saved to prevent that
                if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                    Globals.ThisAddIn.Application.ActiveWorkbook.Saved = true;
            }
            catch (Exception ex)
            {

                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void AppSyncBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Syncs the picklist and finds any datatype change in it
            // If found, Store bool value in Result variable of BackgroundWroker Event Args.
            lock (g_PickListSynchLock)
            {
                e.Result = UpdateAppAsync();
            }
        }
        private bool UpdateAppAsync()
        {
            try
            {
                List<AppChange> syncChanges = appSyncController.SyncAppConfig(false, true);

                if (syncChanges != null && syncChanges.Count > 0)
                {
                    appSyncController.WritePicklistData(true, syncChanges);
                    appSyncController.WritePicklistNamedRangePair();
                }
                return appSyncController.blnConfigChanged;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, "Update App Sync");
                return false;
            }
        }

        private void executeAppLoadTrigger()
        {
            try
            {
                Guid autoExecuteWorkflow = Guid.Empty;

                if (ApttusCommandBarManager.EditInExcelMode)
                {
                    autoExecuteWorkflow = (from wf in ConfigurationManager.GetInstance.Workflows.OfType<WorkflowStructure>()
                                           where wf.AutoExecuteEditInExcelMode || (wf.Triggers != null && wf.Triggers.Count > 0)
                                           select wf.Id).FirstOrDefault();
                    // actionflow name can passed from the button
                    string ActionFlowName = ApttusCommandBarManager.GetInstance().StartupParameters.ActionFlowName;
                    if (!string.IsNullOrEmpty(ActionFlowName))
                    {
                        Workflow wf = ConfigurationManager.GetInstance.GetWorkflowByName(ActionFlowName);
                        if (wf != null)
                        {
                            autoExecuteWorkflow = wf.Id;
                        }
                        else
                        {
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("WORKFLOW_Invalid_Msg"), ActionFlowName), Constants.RUNTIME_PRODUCT_NAME);
                        }
                    }


                }
                else
                {
                    autoExecuteWorkflow = (from wf in ConfigurationManager.GetInstance.Workflows.OfType<WorkflowStructure>()
                                           where wf.AutoExecuteRegularMode || (wf.Triggers != null && wf.Triggers.Count > 0)
                                           select wf.Id).FirstOrDefault();
                }

                if (!autoExecuteWorkflow.Equals(Guid.Empty))
                    new WorkflowEngine().Execute(autoExecuteWorkflow.ToString());
            }
            catch (Exception ex)
            {

                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

        }

        private void executeFirstAppLoadTrigger()
        {
            // First App Load is not supported and replaced with Regular and Edit in Excel modes.

            //foreach (Workflow wf in ConfigurationManager.GetInstance.Workflows)
            //{
            //    WorkflowStructure wfStruct = wf as WorkflowStructure;

            //    if (wfStruct.Triggers.Contains(WorkflowStructure.Trigger.FirstAppLoad))
            //    {
            //        new WorkflowEngine().Execute(wfStruct.Id.ToString());
            //        break;
            //    }
            //}
        }

        internal void InitializeOptionsForm()
        {
            OptionsForm = new OptionsForm();
        }

        private void CopyExternalLibraries()
        {
            try
            {
                string externalLibraryPath = Utils.GetDirectoryForExternalLib();
                DirectoryInfo di = new DirectoryInfo(externalLibraryPath);
                ConfigurationManager configManager = ConfigurationManager.GetInstance;
                Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);

                foreach (ExternalLibrary externalLib in configManager.Application.ExternalLibraries)
                {
                    string shapeName = externalLib.Id;
                    if (!ExcelHelper.FindShape(oSheet, shapeName))
                        continue;
                    string filePath = Path.Combine(externalLibraryPath, externalLib.ExternalLibraryName);
                    ExcelHelper.CopyEmbeddedFileToDisk(oSheet, filePath, shapeName);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.DebugLog("Error while copying external libraries " + Environment.NewLine);
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        internal string GetApplicationURI(string appUniqueId)
        {
            return xauthorLogin.GetApplicationURI(appUniqueId);
        }

        internal void ResetProxy()
        {
            xauthorLogin.ResetProxy();
        }

        internal void SwitchConnection(string userName)
        {
            xauthorLogin.SwitchConnection(userName);
        }

        internal void ResetToken()
        {
            xauthorLogin.ResetToken();
        }

        public void CallEditinExcel(string[] msg)
        {
            ApttusCommandBarManager.EditInExcelMode = true;
            xauthorLogin.StartXAuthorAppFromStartupParameters(msg);
            ApttusCommandBarManager.EditInExcelMode = false;
        }

        public void LoadAppWithParam(string[] msg)
        {
            xauthorLogin.StartXAuthorAppFromStartupParameters(msg);
        }
        public string GetFrontDoorURL()
        {
            return xauthorLogin.GetFrontDoorURL();
        }

    }
}
