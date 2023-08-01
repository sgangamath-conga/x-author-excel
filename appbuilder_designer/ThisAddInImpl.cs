/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Threading;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Win32;

using Apttus.OAuthLoginControl;
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using Autofac;
using System.Runtime.InteropServices;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ThisAddIn
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        CRMFactory crmFactory = CRMFactory.Instance;

        // Application manager to store app level settings in config file (move from registry)
        public ApplicationConfigManager applicationConfigManager = null;

        // Adapter info
        public ApttusUserInfo userInfo = null;

        public string targetLocation = string.Empty;

        // Command Bar managers
        private ApttusCommandBarManager commandBar = null;        

        /// <summary>
        /// 
        /// </summary>
        public void InitializeApplicationInfo()
        {
            RegisterToExceptionEvents();

            string KeyVal = string.Empty;
            ExceptionLogHelper.GetInstance(Utils.GetApplicationTempDirectory(), Constants.DESIGNER_LOG_NAME);
            ExceptionLogHelper.TraceLevel = "Debug"; // Logs always turned On            
            resourceManager.SetCultureInfo("en-US");
        }

        internal IXAuthorRibbonLogin GetLoginObject()
        {
            return crmFactory.GetLoginObject();
        }

        internal BaseApplicationController GetApplicationController()
        {
            return crmFactory.GetApplicationController();
        }
        /// <summary>
        /// 
        /// </summary>
        internal void SubscribeEvents()
        {
            // In case of multiple excel.exe, UnsubscribeEvents() events from previous events
            UnsubscribeEvents();

            Application.WorkbookBeforeClose += Application_WorkbookBeforeClose;
            Application.WorkbookActivate += Application_WorkbookActivate;
            Application.WorkbookOpen += Application_WorkbookOpen;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UnsubscribeEvents()
        {
            Application.WorkbookBeforeClose -= Application_WorkbookBeforeClose;
            Application.WorkbookActivate -= Application_WorkbookActivate;
            Application.WorkbookOpen -= Application_WorkbookOpen;
        }

        void Application_WorkbookActivate(Excel.Workbook Wb)
        {


            string Editiontype = null;

            Globals.ThisAddIn.AppDesignerVersion = "";
            Globals.ThisAddIn.Application.StatusBar = "";
 
            // Designer
            // Check if metadata sheet exists
            Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
            if (oSheet == null)
            {
                commandBar.ValidateDesignerMenu();
               
                return;
            }

            // Get application Unique Id
            Excel.Range rng = oSheet.Cells[1, 1];
            string appUniqueId = oSheet.Cells[1, 1].Value2 as string;

            // get designer & runtime flag
            string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
            string isDesignerFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_DESIGNER_FILE);

            // Get Application instance
            //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Designer);
            ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, Globals.ThisAddIn.Application.ActiveWorkbook.Name, ApplicationMode.Designer);
           
            //open app situation

            if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null && appInstance.App.Definition.EditionType != null)
            {
                // apps created using the apptype ui will get the correct edition
                Editiontype = appInstance.App.Definition.EditionType;
            }
            else if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null && appInstance.App.QuickAppModel != null)
                //ConfigurationManager.GetInstance.Application.QuickAppModel !=null) 
            {
                // apps created in previous build
                Editiontype = Constants.BASIC_EDITION; 
            }
            else if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null)
            {
                Editiontype = Constants.ENTERPRISE_EDITION;
            }
            else if  (ApplicationHelper.GetInstance.EditionType != null) // new app 
            {
                Editiontype = ApplicationHelper.GetInstance.EditionType;
            }

            if (!string.IsNullOrEmpty(Editiontype))
            {
                Constants.PRODUCT_EDITION = Editiontype;
            }
            
            else
            {
                Constants.PRODUCT_EDITION = "";
            }

            Globals.ThisAddIn.Application.StatusBar = Constants.PRODUCT_EDITION;
            Globals.ThisAddIn.AppDesignerVersion = Constants.PRODUCT_EDITION; 
            

            if (!string.IsNullOrEmpty(isDesignerFile))
            {
                ApttusCommandBarManager.g_IsAppOpen = true;

                commandBar.EnableMenus();
                ApttusRibbon.ActivateTab();
            }
            else
                commandBar.ValidateDesignerMenu();

            if (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true"))
            {
                ApttusCommandBarManager.g_IsAppOpen = false;
                commandBar.EnableMenus();
            }

         

            // Set Application instance
            Utils.SetApplicationInstance(appInstance);
        }

        void Application_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            // Designer
            // get designer flag
            string isDesignerFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_DESIGNER_FILE);

            // Remove Application instance on workbook close
            if (ConfigurationManager.GetInstance.Application != null && !string.IsNullOrEmpty(isDesignerFile))
            {
                if (!Globals.ThisAddIn.Application.ActiveWorkbook.Saved)
                {
                    Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = ApttusMessageUtil.ShowWarning(resourceManager.GetResource("COMMONS_AreYouSure_ShowMsg"), Constants.PRODUCT_NAME, ApttusMessageUtil.YesNo);

                    if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
                    {
                        Globals.ThisAddIn.Application.ActiveWorkbook.Saved = true;
                        //ApplicationManager.GetInstance.Remove(ConfigurationManager.GetInstance.Application.Definition.UniqueId, ApplicationMode.Designer);
                        Utils.RemoveApplicationInstance(ConfigurationManager.GetInstance.Application.Definition.UniqueId, Globals.ThisAddIn.Application.ActiveWorkbook.Name, ApplicationMode.Designer);

                        ApttusCommandBarManager.g_IsAppOpen = false;
                        commandBar.ValidateDesignerMenu();
                        Globals.ThisAddIn.Application.StatusBar = "";
                        Globals.ThisAddIn.AppDesignerVersion = "";
                    }
                    else if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No)
                    {
                        Cancel = true;
                    }
                }
            }
        }

        void Application_WorkbookOpen(Excel.Workbook Wb) {
            try {
                commandBar.ValidateDesignerMenu();
            } catch(Exception ex) {
                ExceptionLogHelper.ErrorLog(ex);
            } finally {
                Marshal.ReleaseComObject(Wb);
            }
        }

        private bool _hasActiveWorkbook;

        public bool HasActiveWorkbook
        {
            get
            {
                var count = this.Application.Workbooks.Count;
                if (count >= 1)
                {
                    try
                    {
                        Excel.Workbook activeWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                        return (activeWorkbook != null) ? true : false;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return false;
            }

            private set
            {
                _hasActiveWorkbook = value;
            }
        }

        public Office.COMAddIn GetRuntimeAddin()
        {
            Office.COMAddIns comAddIns = Globals.ThisAddIn.Application.COMAddIns;
            Office.COMAddIn runtimeAddIn = null;

            try
            {
                foreach (Microsoft.Office.Core.COMAddIn myAddin in comAddIns)
                {
                    if (myAddin.ProgId.Equals("Apttus.X-Author.Apps") || myAddin.ProgId.Equals("Apttus.XAuthor.AppRuntime"))
                    {
                        runtimeAddIn = myAddin;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Runtime Addin");
                return null;
            }
            return runtimeAddIn;
        }


        private static void RegisterToExceptionEvents()
        {
            System.Windows.Forms.Application.ThreadException -= ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledException;

            System.Windows.Forms.Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        private static bool _handlingUnhandledException;
        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException((Exception)e.ExceptionObject);//there is small possibility that this wont be exception but only when interacting with code that can throw object that does not inherit from Exception
        }

        static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private static void HandleUnhandledException(Exception exception)
        {
            if (_handlingUnhandledException)
                return;
            try
            {
                _handlingUnhandledException = true;
                ExceptionLogHelper.DebugLog("UnHandledException");
                ExceptionLogHelper.ErrorLog(exception);
            }
            finally
            {
                _handlingUnhandledException = false;
            }
        }        
    }
}
