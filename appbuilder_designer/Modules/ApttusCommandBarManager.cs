/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Apttus.XAuthor.AppDesigner.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner.Modules
{
    public class ApttusCommandBarManager
    {
        private IXAuthorRibbonLogin xauthorLogin;
        private static ApttusCommandBarManager instance = null;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static ApttusOptionsForm OptionsForm = null;

        //public static string g_UserName = string.Empty;
        public static bool g_IsLoggedIn = false;
        public static bool g_IsAppOpen = false;
        public static bool g_IsBasicEdition = false;

        public ApttusCommandBarManager()
        {
        }
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
        public bool IsAppOpen()
        {
            return g_IsAppOpen;
        }

        public bool IsBasicEdition()
        {
            return g_IsBasicEdition;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeOptionsForm()
        {
            OptionsForm = new ApttusOptionsForm();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowLoginForm()
        {
            xauthorLogin.Login();            
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowManageConnection()
        {
            xauthorLogin.ShowManageConnection();           
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowOptions(bool bHideGeneralTab = false, bool bHideQuickAppTab = false)
        {
            OptionsForm = new ApttusOptionsForm();

            if (bHideGeneralTab)
                OptionsForm.HideGeneral();
            else if (bHideQuickAppTab)
                OptionsForm.HideQuickAppSettings();

            OptionsForm.ShowDialog();
            OptionsForm.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoAboutBox()
        {
            ApttusAboutForm AboutBox = new ApttusAboutForm();
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
        public void EnableMenus()
        {
            LicenseActivationManager licenceManager = LicenseActivationManager.GetInstance;

            //return;
            bool LoggedIn = IsLoggedIn();
            bool AppOpen = IsAppOpen();

            // Access Group Menus
            Globals.Ribbons.ApttusRibbon.btnConnect.Enabled = !LoggedIn;
            Globals.Ribbons.ApttusRibbon.btnSetCRM.Enabled = !LoggedIn;

            // Application Group Menus
            Globals.Ribbons.ApttusRibbon.mnuApplication.Enabled = LoggedIn;
            if (LoggedIn )
            {

                //Change App type menu needs to be enabled only when an app is opened. 
                int ItemCount = Globals.Ribbons.ApttusRibbon.mnuApplication.Items.Count;
                if (ItemCount > 0)
                {
                    
                   // Globals.Ribbons.ApttusRibbon.mnuApplication.Items[]
                    foreach (Microsoft.Office.Tools.Ribbon.RibbonControl  btn in Globals.Ribbons.ApttusRibbon.mnuApplication.Items)
                    {
                       
                        if (btn.Name.Equals("EDIT_APPTYPE"))
                        {
                            btn.Enabled = AppOpen;
                            break;
                        }
                    }
                }
            }

            // TODO:: Include commented code to enforce menu validations on create application
            // Build Group Menus
            // code for LMA
            Globals.Ribbons.ApttusRibbon.btnAppDef.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnAppDef.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnMenuDesigner.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnMenuDesigner.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.splitBtnActions.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.splitBtnActions.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnWorkflow.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnWorkflow.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnRetrieveMap.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnRetrieveMap.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnMatrixMap.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnMatrixMap.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnSaveMap.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSaveMap.Tag.ToString()) : false;

            // If parent action button is enabled, then validate individual Actions
            if (Globals.Ribbons.ApttusRibbon.splitBtnActions.Enabled)
            {
                Globals.Ribbons.ApttusRibbon.btnAddRowAction.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnAddRowAction.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnCheckIn.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnCheckIn.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnCheckOut.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnCheckOut.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnClearAction.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnClearAction.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnRetrieve.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnRetrieve.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnMacroAction.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnMacroAction.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnPasteRow.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnPasteRow.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnpasteSourceData.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnpasteSourceData.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnExecuteQuery.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnExecuteQuery.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnCallProcedure.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnCallProcedure.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnSaveAction.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSaveAction.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnSaveAttachmentAction.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSaveAttachmentAction.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnSearchSelect.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSearchSelect.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnSwitchConnection.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSwitchConnection.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnDataSet.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnDataSet.Tag.ToString());
                Globals.Ribbons.ApttusRibbon.btnDelete.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnDelete.Tag.ToString());
            }

            // Save Application group menus
            Globals.Ribbons.ApttusRibbon.splitbtnSave.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.splitbtnSave.Tag.ToString()) : false;
            if (Globals.Ribbons.ApttusRibbon.splitbtnSave.Enabled)
                Globals.Ribbons.ApttusRibbon.btnSaveAs.Enabled = licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnSaveAs.Tag.ToString());

            Globals.Ribbons.ApttusRibbon.btnValidate.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnValidate.Tag.ToString()) : false;

            // Migrate group
            Globals.Ribbons.ApttusRibbon.btnImport.Enabled = LoggedIn ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnImport.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnExport.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnExport.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnDataLoader.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnDataLoader.Tag.ToString()) : false;
            Globals.Ribbons.ApttusRibbon.btnExternalLibrary.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnExternalLibrary.Tag.ToString()) : false;
            // App Settings
            Globals.Ribbons.ApttusRibbon.btnAppSetting.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnAppSetting.Tag.ToString()) : false;

            // Preview group menus
            Globals.Ribbons.ApttusRibbon.btnPreview.Enabled = (LoggedIn & AppOpen) ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.btnPreview.Tag.ToString()) : false;

            Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Enabled = LoggedIn ? licenceManager.IsFeatureAvailable(Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Tag.ToString()) : false;

            bool isAdminUser = LoggedIn && ObjectManager.GetInstance.IsAdminUser();

            if (ApttusObjectManager.GetDocProperty(Apttus.XAuthor.Core.Constants.APPBUILDER_RUNTIME_FILE) == "true")
            {
                Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Enabled = false;
                Globals.Ribbons.ApttusRibbon.btnDataMigration.Enabled = false;
            }
            else if (LoggedIn && ApttusObjectManager.GetDocProperty(Apttus.XAuthor.Core.Constants.APPBUILDER_DESIGNER_FILE) == "true")
            {
                Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Enabled = true;
                Globals.Ribbons.ApttusRibbon.btnDataMigration.Enabled = isAdminUser;
            }
            else
            {
                bool bIsAppLoad;
                //Boolean.TryParse(ApttusRegistryManager.GetKeyValue(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, "IsAppLoading"), out bIsAppLoad);
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();
                Boolean.TryParse(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.IsAppLoading], out bIsAppLoad);
                if (bIsAppLoad)
                {
                    Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Enabled = false;
                    Globals.Ribbons.ApttusRibbon.btnDataMigration.Enabled = false;
                }
                else
                {
                    Globals.Ribbons.ApttusRibbon.splitBtnQuickApp.Enabled = LoggedIn;
                    Globals.Ribbons.ApttusRibbon.btnDataMigration.Enabled = isAdminUser;
                }
            }
            Globals.Ribbons.ApttusRibbon.btnQuickAppEdit.Enabled = (LoggedIn && AppOpen);
            Globals.Ribbons.ApttusRibbon.btnQuickAppNew.Enabled = (LoggedIn && !AppOpen);
            //Data Migration
            Globals.Ribbons.ApttusRibbon.btnNewDataMigrationApp.Enabled = isAdminUser && !AppOpen;
            Globals.Ribbons.ApttusRibbon.btnEditDataMirationApp.Enabled = isAdminUser && AppOpen;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ValidateDesignerMenu()
        {
            if (string.IsNullOrEmpty(ApttusObjectManager.GetDocProperty(Apttus.XAuthor.Core.Constants.APPBUILDER_DESIGNER_FILE)) || !g_IsAppOpen)
            {
                g_IsAppOpen = false;
                EnableMenus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoLogout(bool showBalloonMessage = false)
        {
            try
            {
                xauthorLogin.DoLogout(showBalloonMessage);
                EnableMenus();
            }
            catch (Exception ex)
            {
                ex.Source = "LOGOFF";
                ApttusObjectManager.HandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetActiveWorkbookName()
        {
            try
            {
                return Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// This will be invoked once in the lifetime of the Addin.
        /// </summary>
        internal void InitializeCRM()
        {
            xauthorLogin = Globals.ThisAddIn.GetLoginObject();

            xauthorLogin.OnConnected += xauthorLogin_OnConnected;
            xauthorLogin.OnLogout += xauthorLogin_OnLogout;
        }

        void xauthorLogin_OnLogout(object sender, EventArgs e)
        {
            EnableMenus();
        }

        /// <summary>
        /// Once the user is connected to crm, we can perform any post login actions, such as enabling Ribbon menus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xauthorLogin_OnConnected(object sender, EventArgs e)
        {
            if (xauthorLogin != null && g_IsLoggedIn)
                EnableMenus();
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
    }
}

