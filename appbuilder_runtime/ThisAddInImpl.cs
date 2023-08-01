/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

using Apttus.OAuthLoginControl;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.WebBrowserControl;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Threading;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class ThisAddIn
    {
        // Command Bar managers
        private ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();

        // OAuth wrapper
        //public ApttusOAuth oAuthWrapper = null;
        // Application manager to store app level settings in config file (move from registry)
        public ApplicationConfigManager applicationConfigManager = null;

        // Apttus Object Manager
        public ObjectManager objectManager;

        // Adapter info
        public ApttusUserInfo userInfo = null;
        //public WebBrowserWrapper UIWrapper = null;

        // Notify Icon
        public NotifyIcon apttusNotification = new NotifyIcon();
        private ContextMenuStrip notifyContextMenuStrip = new ContextMenuStrip();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ApttusCommandBarManager commandBar = null;
        public string ConnectionURL = string.Empty;

        private Ribbon ribbon;
        public Ribbon RuntimeRibbon
        {
            get { return ribbon; }
            set { ribbon = value; }
        }

        private CRMFactory crmFactory = CRMFactory.Instance;

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            RegisterToExceptionEvents();

            ribbon = new Ribbon();
            return ribbon;
        }

        /// <summary>
        /// this method will be called for calling x-author runtime directly from SFDC UI
        /// </summary>
        //public void CheckStartupCommand(string[] msg)
        //{
        //    ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
        //    // System.Windows.Forms.MessageBox.Show("debug ");
        //    commandBarManager.StartupParameters.Parse(msg);
        //    //ApttusCommandBarManager.GetInstance().StartupParameters.Parse(Environment.GetCommandLineArgs());
        //    if (commandBarManager.StartupParameters.SessionId != null)
        //    {
        //        TokenResponse Tok = new TokenResponse();
        //        Tok.access_token = commandBarManager.StartupParameters.SessionId;        //        
        //        //ApttusCommandBarManager.GetInstance().StartupParameters.SessionId;
        //        //TODO : get the correct session id from SFDC UI instead of hardcoding 
        //        Tok.instance_url = commandBarManager.StartupParameters.URLInstance; //"https://na15.salesforce.com";
        //        // objectManager.Connect(Tok.access_token, Tok.instance_url);
        //        Globals.ThisAddIn.oAuthWrapper.token = Tok;
        //        Globals.ThisAddIn.Application.WindowState = Excel.XlWindowState.xlMaximized;
        //        commandBarManager.LoginwithToken(Globals.ThisAddIn.ribbon);
        //        DataManager.GetInstance.StartParameters = commandBarManager.StartupParameters;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public void InitializeXAuthorRuntime()
        {
            string KeyVal = string.Empty;
            ExceptionLogHelper.GetInstance(Utils.GetApplicationTempDirectory(), Constants.RUNTIME_LOG_NAME);
            ExceptionLogHelper.TraceLevel = "Debug"; // Logs always turned On

            apttusNotification.Text = Constants.PRODUCT_NAME;
            apttusNotification.Icon = Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            apttusNotification.Visible = true;

            notifyContextMenuStrip.ShowImageMargin = true;
            notifyContextMenuStrip.ShowItemToolTips = false;

            UpdateResourceManager();

            objectManager = ObjectManager.GetInstance;

            ToolStripMenuItem productName = new ToolStripMenuItem(Constants.EXCEL_CONTEXT_ABOUTRUNTIME);
            productName.Font = new Font(productName.Font, productName.Font.Style | FontStyle.Bold);

            notifyContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                productName,
                new ToolStripSeparator(),
                new ToolStripMenuItem(Constants.EXCEL_CONTEXT_VIEWRUNTIMELOG),
                new ToolStripMenuItem(Constants.EXCEL_CONTEXT_VIEWRUNTIMESAVEMESSAGES),
                new ToolStripMenuItem(Constants.EXCEL_CONTEXT_CLEAR)
            });

            (notifyContextMenuStrip.Items[4] as ToolStripMenuItem).DropDownItems.Add(
                Constants.EXCEL_CONTEXT_CLEARLOGS, null, new EventHandler(Clear_ItemClicked));
            (notifyContextMenuStrip.Items[4] as ToolStripMenuItem).DropDownItems.Add(
                Constants.EXCEL_CONTEXT_CLEARSAVEMESSAGES, null, new EventHandler(Clear_ItemClicked));

            notifyContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(MainMenu_ItemClicked);

            apttusNotification.ContextMenuStrip = notifyContextMenuStrip;

            // Set the Excel Application context whether it is an Add-in Application or New Excel.Application
            ExcelHelper.ExcelApp = Globals.ThisAddIn.Application;

            commandBarManager.InitializeCRM();

        }

        private void UpdateResourceManager()
        {
            try
            {
                if (!Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference))
                    return;

                string Language = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference];
                resourceManager.UpdateResourceManager(Language);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (Globals.ThisAddIn.HasActiveWorkbook)
            //{
            //    if (Globals.ThisAddIn.Application.ActiveWindow.WindowState == Excel.XlWindowState.xlMinimized)
            //    {
            //        Globals.ThisAddIn.Application.ActiveWindow.WindowState = Excel.XlWindowState.xlMaximized;
            //    }
            //}

            //if (Globals.ThisAddIn.Application.Windows.Count > 0)
            //    Globals.ThisAddIn.Application.ActiveWindow.Activate();

            try
            {
                if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_ABOUTRUNTIME)
                {
                    commandBarManager.DoAboutBox();
                }
                else if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_OPTIONS)
                {
                    commandBarManager.ShowOptions();
                }
                else if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_VIEWRUNTIMELOG)
                {
                    Process pi = new Process();
                    pi.StartInfo.FileName = ExceptionLogHelper.LogFileFullPath;
                    pi.Start();
                }
                else if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_VIEWRUNTIMESAVEMESSAGES)
                {
                    Process pi = new Process();
                    pi.StartInfo.FileName = ExceptionLogHelper.SaveMessageLogFullPath;
                    pi.Start();
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void Clear_ItemClicked(object sender, EventArgs e)
        {
            try
            {
                if ((sender as ToolStripItem).Text == Constants.EXCEL_CONTEXT_CLEARLOGS)
                {
                    File.WriteAllText(ExceptionLogHelper.LogFileFullPath, "");
                }
                else if ((sender as ToolStripItem).Text == Constants.EXCEL_CONTEXT_CLEARSAVEMESSAGES)
                {
                    File.WriteAllText(ExceptionLogHelper.SaveMessageLogFullPath, "");
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DoPostLogin(Ribbon ribbon)
        {
            return true;
            //try
            //{
            //    System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(Globals.ThisAddIn.oAuthWrapper.ConnectionEndPoint);
            //    //objectManager.Connect(oAuthWrapper.token.access_token, oAuthWrapper.token.instance_url, proxyObject);
            //    // TODO:: for Edit in Excel perf optimize, if not needed remove GetClientVersion call
            //    try
            //    {
            //        Version packagedVersion = new Version(new ABSalesforceAdapter().GetClientVersion());
            //        Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            //        if (packagedVersion > currentVersion)
            //            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("ADDINIMPL_Download_ErrorMsg"), Constants.PRODUCT_NAME), resourceManager.GetResource("ADDINIMPL_DownloadCap_ErrorMsg"));
            //    }
            //    catch (ArgumentNullException e) { }

            //    userInfo = objectManager.getUserInfo();

            //    // Make logged in flag true
            //    ApttusCommandBarManager.g_IsLoggedIn = true;
            //    Globals.ThisAddIn.oAuthWrapper.IsLoggedIn = true;
            //    string sServerURL = ApttusObjectManager.GetSessionServerUrl(true);
            //    // Assign username to command bar manager
            //    ApttusCommandBarManager.g_UserName = userInfo.UserName;
            //    Constants.CURRENT_USER_ID = userInfo.UserId;

            //    // TODO: LMA once we receive web service method, integrate the code change, right now read XML file.
            //    LicenseActivationManager licenceManager = LicenseActivationManager.GetInstance;

            //    bool bGetFeatureDetails = false;
            //    licenceManager.CheckEdition(out bGetFeatureDetails);
            //    licenceManager.GetFeatureDetails(bGetFeatureDetails);

            //    ApttusCommandBarManager.g_IsBasicEdition = Constants.PRODUCT_EDITION.Equals(Constants.BASIC_EDITION);

            //    this.UIWrapper = new WebBrowserWrapper(ApttusGlobals.APPLICATION_NAME, sServerURL, Globals.ThisAddIn.oAuthWrapper.token.access_token, null);

            //    // Automatically load app from command line parameter (Edit in Excel)
            //    ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
            //    if (commandBarManager.StartupParameters.AppId != null)
            //    {
            //        ABSalesforceAdapter adapter = new ABSalesforceAdapter();
            //        ApplicationController appController = new ApplicationController();

            //        // No need to check this, the check have been added in the server 

            //        //if (!adapter.IsDesigner()) // for designer user, no need to check the permission
            //        //{
            //        //    if (!appController.IsCurrentUserHasAccessToApp(commandBarManager.StartupParameters.AppId))
            //        //    { // user is not assigned for the app
            //        //        ApttusMessageUtil.ShowError("You do not have permission to access this Excel application template.  Please contact your Salesforce Administrator.", "Assignment Validation");
            //        //        return false;
            //        //    }
            //        //}

            //        Globals.ThisAddIn.Application.StatusBar = "Loading template....";

            //        ApplicationObject appObject = appController.LoadApplication(string.Empty, commandBarManager.StartupParameters.AppId);

            //        if (appObject == null)
            //        {
            //            ApttusMessageUtil.ShowError(resourceManager.GetResource("ADDINIMPL_UnableLoad_ErrorMsg"), resourceManager.GetResource("ADDINIMPL_UnableLoadCap_ErrorMsg"));
            //            return true;
            //        }

            //        commandBarManager.OpenTemplate(appObject);

            //        //This flag is necessary otherwise the edit group menu wont be activated
            //        ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_RUNTIME_FILE, "true");
            //        ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_DESIGNER_FILE);
            //        Globals.ThisAddIn.Application.StatusBar = "Executing Action Flow ....";
            //        commandBarManager.DoPostLoad(ribbon, false);
            //    }

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    ExceptionLogHelper.ErrorLog(ex);
            //    //TODO :: Time being handle .NetFramework issue
            //    if (ex.Message.ToString().Contains("The operation 'searchAsync' could not be loaded because it has a parameter or return type of type System.ServiceModel.Channels.Message or a type that has MessageContractAttribute and other parameters of different types. When using System.ServiceModel.Channels.Message or types with MessageContractAttribute, the method must not use any other types of parameters."))
            //        MessageBox.Show(resourceManager.GetResource("ADDINIMP_SetupMissing_ShowMsg"), Constants.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    else if (ex.Message.ToString().Contains("The communication object, System.ServiceModel.ChannelFactory`1[Apttus.XAuthor.SalesforceAdapter.SForce.Soap], cannot be used for communication because it is in the Faulted state."))
            //        MessageBox.Show(resourceManager.GetResource("ADDINIMP_SetupMissing_ShowMsg"), Constants.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    else
            //        ApttusMessageUtil.ShowError(resourceManager.GetResource("ADDIMIMPL_LoginFailed_ErrorMsg"), resourceManager.GetResource("ADDIMIMPL_LoginFailedCap_ErrorMsg"), resourceManager.GetResource("ADDIMIMPL_LoginFailedInstr_ErrorMsg"), ex.Message);

            //    commandBarManager.DoLogout(true);
            //    return false;
            //}
            //finally
            //{
            //    // Reset Startup params to blank
            //    // Reason for resetting start up params, customer can have Macro in Auto Execute action which renames AppName - Runtime.xlsx file
            //    // Now if you do Switch Connection, there will be 2 instances of Runtime file opened, 1. Renamed , 2 AppName - Runtime.xlsx
            //    commandBarManager.StartupParameters = new StartupParameters();
            //}
        }

        /// <summary>
        /// Notifies User with any dataype change or field deletion related information
        /// </summary>
        public void NotifyDataTypeChange()
        {
            if (ApttusCommandBarManager.g_IsLoggedIn)
            {
                apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("ThisAddInImpl_NotifyDataTypeChangeCAP_WarnMsg"), resourceManager.GetResource("ThisAddInImpl_NotifyDataTypeChange_WarnMsg"), ToolTipIcon.Warning);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void NotifyLogin(string userName)
        {
            if (ApttusCommandBarManager.g_IsLoggedIn)
            {
                apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("ADDINIMP_NotifyLoginConnected_ShowMsg"), string.Format(resourceManager.GetResource("ADDINIMP_NotifyLoginWelcome_ShowMsg"), this.userInfo.UserFullName, ConnectionURL, this.userInfo.UserName), ToolTipIcon.Info);
                string sNofifyText = Constants.PRODUCT_NAME + " (" + userName + ")";
                if (sNofifyText.Length >= 64)
                    sNofifyText = sNofifyText.Substring(0, 59) + "...)";

                apttusNotification.Text = sNofifyText;
            }
            else
            {
                ApttusObjectManager.ShowNormalCursor();
            }
        }
        //public void NotifyLogin(bool saveLoginOptions = true)
        //{
        //    if (ApttusCommandBarManager.g_IsLoggedIn)
        //    {
        //        apttusNotification.ShowBalloonTip(1000, "Connected to Apttus", "Welcome Back " + this.userInfo.UserFullName + "!", ToolTipIcon.Info);
        //        string sNofifyText = Constants.PRODUCT_NAME + " (" + ApttusCommandBarManager.g_UserName + ")";
        //        if (sNofifyText.Length >= 64)
        //            sNofifyText = sNofifyText.Substring(0, 59) + "...)";

        //        apttusNotification.Text = sNofifyText;

        //        // Save login info to registry
        //        if (saveLoginOptions)
        //        {
        //            // For Launch from Salesforce oAuthWrapper.CurrentUser, current is blank so create valid entry in OAuthConnections
        //            if (string.IsNullOrEmpty(oAuthWrapper.CurrentUser))
        //            {
        //                if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.ContainsKey(userInfo.UserFullName))
        //                    oAuthWrapper.CurrentUser = userInfo.UserFullName;
        //                else
        //                {
        //                    // Prepare OAuthconnection to be added in config XML file
        //                    SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
        //                    oAuthConnection.ConnectionName = userInfo.UserFullName;
        //                    oAuthConnection.ServerHost = oAuthWrapper.token.instance_url;
        //                    oAuthConnection.LastLoginHint = userInfo.UserName;
        //                    oAuthConnection.OAuthToken = oAuthWrapper.token.access_token;

        //                    // Update this.OAuthoWrapper properties based on Edit in Excel Login
        //                    this.oAuthWrapper.LastLoginHint = oAuthConnection.LastLoginHint;
        //                    this.oAuthWrapper.ConnectionEndPoint = oAuthConnection.ServerHost;
        //                    this.oAuthWrapper.TokenValue = Core.Helpers.JsonHelper.JsonObjectToString(oAuthWrapper.token);

        //                    Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Add(userInfo.UserFullName, oAuthConnection);
        //                }
        //            }
        //            oAuthWrapper.SaveOptions(string.IsNullOrEmpty(oAuthWrapper.CurrentUser) ? userInfo.UserFullName : oAuthWrapper.CurrentUser);
        //        }
        //    }
        //    else
        //    {
        //        ApttusObjectManager.ShowNormalCursor();
        //    }
        //}

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
                RuntimeExceptionLogHelper.DebugLog("UnHandledException");
                RuntimeExceptionLogHelper.ErrorLog(exception,false);
            }
            finally
            {
                _handlingUnhandledException = false;
            }
        }

        public BaseApplicationController GetApplicationController()
        {
            return crmFactory.GetApplicationController();
        }
        public IXAuthorRibbonLogin GetLoginObject()
        {
            return crmFactory.GetLoginObject();
        }
        public ICheckInProvider GetCheckInProvider()
        {
            return crmFactory.GetCheckInProvider();
        }
        public ICheckOutProvider GetCheckOutProvider()
        {
            return crmFactory.GetCheckOutProvider();
        }
        public ISearchAndSelectAction GetSSActionController()
        {
            return crmFactory.GetSearchAndSelectActionController();
        }
        public IQueryAction GetQueryActionProvider(QueryActionModel model, string[] inputDataName)
        {
            return crmFactory.GetQueryActionProvider(model, inputDataName);
        }
        public ISaveHelper GetSaveHelper()
        {
            return crmFactory.GetSaveHelper();
        }
    }
}
