using System;
using Apttus.XAuthor.Core;
using Apttus.IntelligentCloud.LoginControl;
using Apttus.XAuthor.AppRuntime.Modules;
using System.Windows.Forms;
using Apttus.OAuthLoginControl;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Apttus.XAuthor.AppRuntime
{
    class XAuthorAICLogin : XAuthorBaseCRMLogin, IXAuthorAICLogin
    {
        private AICWebLogin aicLogin;
        public XAuthorAICLogin()
        {
            aicLogin = new AICWebLogin(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings);
        }
        public override void Login()
        {
            try
            {
                string lastLoginUser = applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;
                if (!string.IsNullOrEmpty(lastLoginUser))
                    aicLogin.ConnectionEndPoint = new Tuple<string, string>(applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ConnectionName, applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ServerHost);

                aicLogin.Proxy = Proxy;

                if (aicLogin.Login() && aicLogin.AICAuthenticationResult != null)
                {
                    
                    if (DoPostLogin())
                        NotifyLogin();
                }
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        public override void SwitchConnection(string userName)
        {
            aicLogin.ConnectionEndPoint = new Tuple<string, string>(userName, Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections[userName].ServerHost);
            if (string.IsNullOrEmpty(aicLogin.ConnectionEndPoint.Item2))
            {
                MessageBox.Show("Invalid connection URL, please provide valid URL and try again.", Constants.PRODUCT_NAME + " - Switch Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            aicLogin.Proxy = Proxy;
            aicLogin.Login();
            if (aicLogin.AICAuthenticationResult != null)
            {
                if (DoPostLogin())
                {
                    applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection = userName;
                    NotifyLogin();
                }
            }
        }

        public override void ShowManageConnection()
        {
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

            ManageConnectionResult connectionResult = aicLogin.ShowManageConnections();
            if (connectionResult == ManageConnectionResult.SwitchLogin)
            {
                aicLogin.Proxy = Proxy;
                aicLogin.Login();
                if (aicLogin.AICAuthenticationResult != null)
                {
                    if (DoPostLogin())
                    {
                        applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection = aicLogin.ConnectionEndPoint.Item1;
                        NotifyLogin();
                    }
                }
            }
        }
        private bool DoPostLogin()
        {
            try
            {
                objectManager.Connect(this);

                LoggedInUserInfo = objectManager.getUserInfo();
                Constants.CURRENT_USER_ID = LoggedInUserInfo.UserId;
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
                //commandBar.DoLogout();
                return false;
            }
        }
        protected override void NotifyLogin(bool saveLoginOptions = true)
        {
            Globals.ThisAddIn.ConnectionURL = TenantURL;

            applicationConfigManager.ApplicationSettings.Save();

            base.NotifyLogin();
        }
        public override TokenResponse StartXAuthorAppFromStartupParameters(string[] msg)
        {
            TokenResponse Tok = base.StartXAuthorAppFromStartupParameters(msg);
            if (ApttusCommandBarManager.EditInExcelMode)
                LoginEditInExcel(Tok.instance_url);
            else
                LoginAndOpenApp(Tok.instance_url, Tok.access_token);//Preview Button click
            return Tok;
        }

        private void LoginAndOpenApp(string instance_url, string access_token)
        {
            aicLogin.ConnectionEndPoint = new Tuple<string, string>("", instance_url);
            aicLogin.Proxy = Proxy;
            aicLogin.AutoLogin();
            if (aicLogin.AICAuthenticationResult != null)
            {
                if (DoPostLogin())
                {
                    if (base.OpenApp())
                        NotifyLogin();
                }
            }
        }

        private void LoginEditInExcel(string instanceURL)
        {
            string lastLoginUser = applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;
            aicLogin.ConnectionEndPoint = new Tuple<string, string>(applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ConnectionName, instanceURL);
            aicLogin.Proxy = Proxy;
            aicLogin.Login();
            if (aicLogin.AICAuthenticationResult != null)
            {
                if (DoPostLogin())
                {
                    if (base.OpenApp())
                        NotifyLogin();
                }
            }
        }

        public AuthenticationResult AuthenticationResult {
            get { return aicLogin.AICAuthenticationResult; }
        }

        public string TenantURL {
            get {
                return aicLogin.ConnectionEndPoint.Item2;
            }
        }
        public string AuthorityURI {
            get {
                return aicLogin.GetAuthorityURI;
            }
        }
        public string ClientAppKey {
            get {
                return aicLogin.ClientAppKey;
            }
        }
        public string ResourceAppKey {
            get {
                return aicLogin.ResourceAppKey;
            }
        }

        public System.Net.IWebProxy Proxy
        {
            get
            {
                return Helpers.ProxyHelper.GetProxyObject(aicLogin.ConnectionEndPoint.Item2);
            }
        }
    }
}
