using System;
using System.Net;
using System.Windows.Forms;
using Apttus.IntelligentCloudV2.Login;
using Apttus.OAuthLoginControl;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    class XAuthorAICV2Login : XAuthorBaseCRMLogin, IXAuthorAICV2Login
    {
        private AICWebLoginV2 aicV2Login;
        public XAuthorAICV2Login()
        {
            aicV2Login = new AICWebLoginV2(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings);
        }
        public override void Login()
        {
            try
            {
                string lastLoginUser = applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;
                if (!string.IsNullOrEmpty(lastLoginUser))
                    aicV2Login.ConnectionEndPoint = new Tuple<string, string>(applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ConnectionName, applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ServerHost);

                aicV2Login.Proxy = Proxy;
                aicV2Login.Login();
                if (!string.IsNullOrEmpty(aicV2Login.TokenResponseJson))
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
            aicV2Login.ConnectionEndPoint = new Tuple<string, string>(userName, Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections[userName].ServerHost);
            if (string.IsNullOrEmpty(aicV2Login.ConnectionEndPoint.Item2))
            {
                MessageBox.Show("Invalid connection URL, please provide valid URL and try again.", Constants.PRODUCT_NAME + " - Switch Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            aicV2Login.Login();
            if (!string.IsNullOrEmpty(aicV2Login.TokenResponseJson))
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

            ManageConnectionResult connectionResult = aicV2Login.ShowManageConnections();
            if (connectionResult == ManageConnectionResult.SwitchLogin)
            {
                aicV2Login.Login();
                if (!string.IsNullOrEmpty(aicV2Login.TokenResponseJson))
                {
                    if (DoPostLogin())
                    {
                        Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection = aicV2Login.ConnectionEndPoint.Item1;
                        Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();
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
            aicV2Login.ConnectionEndPoint = new Tuple<string, string>("", instance_url);
            var base64EncodedBytes = Convert.FromBase64String(access_token);
            aicV2Login.TokenResponseJson = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            //aicLogin.AutoLogin();
            if (!string.IsNullOrEmpty(aicV2Login.TokenResponseJson))
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
            aicV2Login.ConnectionEndPoint = new Tuple<string, string>(applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ConnectionName, instanceURL);
            aicV2Login.Login();
            if (!string.IsNullOrEmpty(aicV2Login.TokenResponseJson))
            {
                if (DoPostLogin())
                {
                    if (base.OpenApp())
                        NotifyLogin();
                }
            }
        }
        public System.Net.IWebProxy Proxy {
            get {
                return Helpers.ProxyHelper.GetProxyObject(aicV2Login.ConnectionEndPoint.Item2);
            }
        }

        public string TenantURL {
            get {
                return aicV2Login.ConnectionEndPoint.Item2;
            }
        }

        public string TokenResponse {
            get {
                return aicV2Login.TokenResponseJson;
            }
        }
    }
}
