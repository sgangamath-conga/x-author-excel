using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Apttus.OAuthLoginControl;
using System.Windows.Forms;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class XAuthorSalesforceLogin : XAuthorBaseCRMLogin, IXAuthorSalesforceLogin
    {
        private ApttusOAuth OAuthLogin;
        private ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();

        public XAuthorSalesforceLogin()
        {
            OAuthLogin = new ApttusOAuth(applicationConfigManager.ApplicationSettings);

            OAuthLogin.clientID = "3MVG9rFJvQRVOvk4Cdn_X9wzEjWMW71zXT.aFEXdKcyZ3LZgAfq6_BagQqoiZ4PpZK9xqWZb9pNP1F634cj8U";
            OAuthLogin.clientSecret = "2031902323682913540";
            OAuthLogin.redirectURL = "xauthorexcel:callback";

            // Initialize OAuthLogin
            OAuthLogin.ApplicationInfo.SaveInRegistry = applicationConfigManager.ApplicationSettings.SaveTokenLocally;
            OAuthLogin.ApplicationInfo.ApplicationRegistryBase = ApttusGlobals.ApttusRegistryBase;
            OAuthLogin.ApplicationInfo.ApplicationName = Constants.RUNTIME_PRODUCT_NAME;
            OAuthLogin.ApplicationInfo.ApplicationType = Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForWord;
            OAuthLogin.ApplicationInfo.ApplicationLogo = global::Apttus.XAuthor.AppRuntime.Properties.Resources.Xauthor_ExcelLogo;
            OAuthLogin.ApplicationInfo.ApplicationIcon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;

            OAuthLogin.ApplicationInfo.ApplicationDescription = Constants.AppDefDescription;
            // Set Internalization support
            OAuthLogin.ApplicationInfo.UseResources = true;
            OAuthLogin.ApplicationInfo.ResourceManager = ApttusResourceManager.resourceManager;
        }

        public override void Login()
        {
            try
            {
                OAuthLogin.token = null;
                System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(OAuthLogin.ConnectionEndPoint);
                if (proxyObject != null)
                    OAuthLogin.Proxy = proxyObject;

                OAuthLogin.Login();
                if (OAuthLogin.token != null)
                {
                    if (DoPostLogin())
                        NotifyLogin();
                }
                else if (OAuthLogin.token == null)
                    OAuthLogin.SwitchLoginId = string.Empty;

                OAuthLogin.CloseLoginForm();
                OAuthLogin.UserClosedForm = false;
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        private bool DoPostLogin()
        {
            try
            {
                //System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(OAuthLogin.ConnectionEndPoint);
                //objectManager.Connect(OAuthLogin.token.access_token, OAuthLogin.token.instance_url, proxyObject);
                objectManager.Connect(this);

                try
                {
                    Version packagedVersion = new Version(new ABSalesforceAdapter().GetClientVersion());
                    Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                    if (packagedVersion > currentVersion)
                        ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("ADDIN_Compatibility_ErrorMsg"), Constants.DESIGNER_PRODUCT_NAME), resourceManager.GetResource("ADDIN_CompatibilityCap_ErrorMsg"));
                }
                catch (ArgumentNullException e) { }

                LoggedInUserInfo = objectManager.getUserInfo();
                Constants.CURRENT_USER_ID = LoggedInUserInfo.UserId;
                OAuthLogin.IsLoggedIn = true;

                ApttusCommandBarManager.g_IsLoggedIn = true;

                LicenseActivationManager licenceManager = LicenseActivationManager.GetInstance;

                bool bGetFeatureDetails = false;
                licenceManager.CheckEdition(out bGetFeatureDetails);
                licenceManager.GetFeatureDetails(bGetFeatureDetails);

                ApttusCommandBarManager.g_IsBasicEdition = Constants.PRODUCT_EDITION.Equals(Constants.BASIC_EDITION);

                return true;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("Application", "error message:" + ex.Message.ToString());
                ExceptionLogHelper.ErrorLog(ex);
                //TODO :: Time being handle .NetFramework issue
                if (ex.Message.ToString().Contains("The operation 'searchAsync' could not be loaded because it has a parameter or return type of type System.ServiceModel.Channels.Message or a type that has MessageContractAttribute and other parameters of different types. When using System.ServiceModel.Channels.Message or types with MessageContractAttribute, the method must not use any other types of parameters."))
                    MessageBox.Show(resourceManager.GetResource("ADDINIMP_SetupMissing_ShowMsg"), resourceManager.GetResource("ADDINIMP_SetupMissingCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (ex.Message.ToString().Contains("The communication object, System.ServiceModel.ChannelFactory`1[Apttus.XAuthor.SalesforceAdapter.SForce.Soap], cannot be used for communication because it is in the Faulted state."))
                    MessageBox.Show(resourceManager.GetResource("ADDINIMP_SetupMissing_ShowMsg"), resourceManager.GetResource("ADDINIMP_SetupMissingCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("ADDIN_Login_ErrorMsg"), resourceManager.GetResource("ADDIN_PermissionCap_ErrorMsg"), resourceManager.GetResource("COMMON_LoginFailed_ErrorMsg"), ex.Message);

                DoLogout(false);
                return false;
            }
            return true;
        }

        public override void DoLogout(bool showBalloonMessage)
        {
            OAuthLogin.SwitchLoginId = string.Empty;
            OAuthLogin.UserClosedForm = false;
            OAuthLogin.token = null;

            base.DoLogout(showBalloonMessage);
        }

        public override void ShowManageConnection()
        {
            applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

            OAuthLogin.UserClosedForm = false;
            Apttus.OAuthLoginControl.Modules.ApttusGlobals.ManageConnectionResult manageConnectionResult = OAuthLogin.ShowManageConnections();

            if (manageConnectionResult == Apttus.OAuthLoginControl.Modules.ApttusGlobals.ManageConnectionResult.SwitchLogin)
            {
                OAuthLogin.UserClosedForm = false;
                OAuthLogin.token = null;

                System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(OAuthLogin.ConnectionEndPoint);

                if (proxyObject != null)
                    OAuthLogin.Proxy = proxyObject;

                OAuthLogin.SwitchLogin();

                if (OAuthLogin.token != null)
                {
                    // Complete post login tasks
                    if (DoPostLogin())
                        NotifyLogin();
                }
                else if (OAuthLogin.token == null)
                {
                    OAuthLogin.SwitchLoginId = string.Empty;
                }

                // Close login form after post login.
                OAuthLogin.CloseLoginForm();
                OAuthLogin.UserClosedForm = false;
            }
        }

        public override void SwitchConnection(string userName)
        {
            string sHost = applicationConfigManager.ApplicationSettings.AppLogin.Connections[userName].ServerHost;
            string sLastLoginHint = applicationConfigManager.ApplicationSettings.AppLogin.Connections[userName].LastLoginHint;

            if (sHost.Length == 0 || string.IsNullOrEmpty(sHost))
            {
                MessageBox.Show(resourceManager.GetResource("RIBBON_InvalidConn_ShowMsg"), resourceManager.GetResource("COMMON_SwitchConnection_Text"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OAuthLogin.UserClosedForm = false;
            OAuthLogin.token = null;
            OAuthLogin.SwitchLoginId = userName;
            OAuthLogin.ConnectionEndPoint = sHost;
            OAuthLogin.LastLoginHint = sLastLoginHint;

            OAuthLogin.SwitchLogin();

            if (OAuthLogin.token != null)
            {
                // Complete post login tasks
                if (DoPostLogin())
                    NotifyLogin();
            }
            else if (OAuthLogin.token == null)
            {
                OAuthLogin.SwitchLoginId = string.Empty;
            }

            // Close login form after post login.
            OAuthLogin.CloseLoginForm();
            OAuthLogin.UserClosedForm = false;
        }

        public override string GetApplicationURI(string appUniqueId)
        {
            StringBuilder applicationURI = new StringBuilder();

            applicationURI.Append("/AppId=").Append(appUniqueId)
                          .Append("|")
                          .Append("/ExportId=").Append(string.Empty)
                          .Append("|")
                          .Append("/InstanceURL=").Append(OAuthLogin.token.instance_url)
                          .Append("|")
                          .Append("/SessionId=").Append(OAuthLogin.token.access_token);

            return applicationURI.ToString();
        }

        public override void ResetProxy()
        {
            OAuthLogin.Proxy = null;
        }

        public override void ResetToken()
        {
            OAuthLogin.TokenValue = string.Empty;
        }

        protected override void NotifyLogin(bool saveLoginOptions = true)
        {
            // Save login info to registry
            if (saveLoginOptions)
            {
                // For Launch from Salesforce oAuthWrapper.CurrentUser, current is blank so create valid entry in OAuthConnections
                if (string.IsNullOrEmpty(OAuthLogin.CurrentUser))
                {
                    if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.ContainsKey(LoggedInUserInfo.UserFullName))
                        OAuthLogin.CurrentUser = LoggedInUserInfo.UserFullName;
                    else
                    {
                        // Prepare OAuthconnection to be added in config XML file
                        SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
                        oAuthConnection.ConnectionName = LoggedInUserInfo.UserFullName;
                        oAuthConnection.ServerHost = OAuthLogin.token.instance_url;
                        oAuthConnection.LastLoginHint = LoggedInUserInfo.UserName;
                        oAuthConnection.OAuthToken = OAuthLogin.token.access_token;

                        // Update this.OAuthoWrapper properties based on Edit in Excel Login
                        this.OAuthLogin.LastLoginHint = oAuthConnection.LastLoginHint;
                        this.OAuthLogin.ConnectionEndPoint = oAuthConnection.ServerHost;
                        this.OAuthLogin.TokenValue = Core.Helpers.JsonHelper.JsonObjectToString(OAuthLogin.token);

                        Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Add(LoggedInUserInfo.UserFullName, oAuthConnection);
                    }
                }

                // Save login info to registry
                OAuthLogin.SaveOptions(string.IsNullOrEmpty(OAuthLogin.CurrentUser) ? UserInfo.UserFullName : OAuthLogin.CurrentUser);
                Globals.ThisAddIn.ConnectionURL = OAuthLogin.token.instance_url;
                base.NotifyLogin();
            }
        }

        public string AccessToken
        {
            get
            {
                return OAuthLogin.token.access_token;
            }
        }

        public string InstanceURL
        {
            get
            {
                return OAuthLogin.token.instance_url;
            }
        }

        public override string GetFrontDoorURL()
        {
            return XmlAuthorUtil.GetFrontDoorURL(OAuthLogin.token.instance_url, OAuthLogin.token.access_token, "");
        }

        public override TokenResponse StartXAuthorAppFromStartupParameters(string[] msg)
        {
            TokenResponse Tok = base.StartXAuthorAppFromStartupParameters(msg);
            OAuthLogin.token = Tok;
            LoginAndOpenApp();
            return Tok;
        }

        public void LoginAndOpenApp()
        {
            try
            {
                System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(this.OAuthLogin.ConnectionEndPoint);

                if (proxyObject != null)
                    OAuthLogin.Proxy = proxyObject;

                if (OAuthLogin.token != null)
                {
                    // Complete post login tasks
                    if (DoPostLogin())
                    {
                        if (base.OpenApp())
                            NotifyLogin();
                    }
                }

            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Login");
            }
            finally
            {
                // Reset Startup params to blank
                // Reason for resetting start up params, customer can have Macro in Auto Execute action which renames AppName - Runtime.xlsx file
                // Now if you do Switch Connection, there will be 2 instances of Runtime file opened, 1. Renamed , 2 AppName - Runtime.xlsx
                commandBarManager.StartupParameters = new StartupParameters();
            }
        }

        public System.Net.IWebProxy Proxy
        {
            get
            {
                return Helpers.ProxyHelper.GetProxyObject(OAuthLogin.ConnectionEndPoint);
            }
        }
    }
}
