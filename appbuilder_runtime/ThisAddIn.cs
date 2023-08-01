/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class ThisAddIn
    {

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            
            try
            {
                if (this.applicationConfigManager == null)
                {
                    Globals.ThisAddIn.applicationConfigManager = ApplicationConfigManager.GetInstance();
                    Globals.ThisAddIn.applicationConfigManager.LoadApplicationSettings();
                }
                //this.oAuthWrapper = new OAuthLoginControl.ApttusOAuth(this.applicationConfigManager.ApplicationSettings);

                commandBar = Modules.ApttusCommandBarManager.GetInstance();
                commandBar.InitializeOptionsForm();

                InitializeXAuthorRuntime();
                // CheckStartupCommand();
                int IEMajorVersion = (new System.Windows.Forms.WebBrowser()).Version.Major;
                if (IEMajorVersion > 7)
                {
                    ApttusRegistryManager.CreateOrUpdateKeyForIECompatibility(true);
                }

                WorkbookEventManager.GetInstance.SubscribeEvents();
                WorkbookEventManager.GetInstance.SubScribeDoubleClick();
                WorkbookEventManager.GetInstance.SubScribeBeforeSave();

                if (Globals.ThisAddIn != null && Globals.ThisAddIn.Application != null && Globals.ThisAddIn.Application.Workbooks != null && Globals.ThisAddIn.Application.Workbooks.Count == 1)
                {
                    WorkbookEventManager.GetInstance.bProtectedToRegularMode = Globals.ThisAddIn.Application.ActiveProtectedViewWindow != null;
                    WorkbookEventManager.GetInstance.Application_WorkbookOpen(Globals.ThisAddIn.Application.ActiveWorkbook);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }


        }
        // store the version num in the global var so that 
        // this info can be displayed in the about ui. 
        public string AppVersion
        {
            get;
            set;
        }

        public void CallEditinExcel(string[] msg)
        {
            commandBar.CallEditinExcel(msg);
        }

        public void LoadAppWithParam(string[] msg)
        {
            commandBar.LoadAppWithParam(msg);
        }

        private AddinUtilities addinUtilities;

        protected override object RequestComAddInAutomationService()
        {
            if (addinUtilities == null)
            {
                addinUtilities = new AddinUtilities();
            }
            return addinUtilities;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try
            {
                // Unscubscribe Excel events
                WorkbookEventManager.GetInstance.UnsubscribeEvents();
                WorkbookEventManager.GetInstance.UnSubScribeDoubleClick();
                WorkbookEventManager.GetInstance.UnSubScribeBeforeSave();

                // Clear Excel temp files
                if (Globals.ThisAddIn.HasActiveWorkbook)
                {
                    Globals.ThisAddIn.Application.ActiveWorkbook.Saved = true;
                    Globals.ThisAddIn.Application.ActiveWorkbook.Close();
                }
                // If app is not loading
                if (!commandBarManager.IsAppLoading())
                    Utils.CleanupXAuthorFiles();

                // Close Notification Icon from Notification Area Icons
                if (Globals.ThisAddIn.apttusNotification != null)
                {
                    Globals.ThisAddIn.apttusNotification.Visible = false;
                    Globals.ThisAddIn.apttusNotification.Icon = null;
                    Globals.ThisAddIn.apttusNotification.Dispose();
                    Globals.ThisAddIn.apttusNotification = null;
                }

                ApttusRegistryManager.CreateOrUpdateKeyForIECompatibility(false);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
