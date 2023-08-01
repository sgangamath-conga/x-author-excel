using Apttus.IntelligentCloudV2.Login;
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public class XAuthorAICV2Login : XAuthorBaseCRMLogin, IXAuthorAICV2Login
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
                        NotifyLogin();
                    }
                }
            }
        }
        public override string GetApplicationURI(string appUniqueId)
        {
            StringBuilder sb = new StringBuilder();
            var plainTextBytes = Encoding.UTF8.GetBytes(aicV2Login.TokenResponseJson);
            var token = Convert.ToBase64String(plainTextBytes);

            sb.Append("/AppId=").Append(appUniqueId).Append("|")
              .Append("/ExportId=").Append(string.Empty).Append("|")
              .Append("/InstanceURL=").Append(TenantURL).Append("|")
              .Append("/SessionId=").Append(token);

            return sb.ToString();
        }
        protected override void NotifyLogin()
        {
            ConnectionURL = TenantURL;
            applicationConfigManager.ApplicationSettings.Save();

            base.NotifyLogin();
        }
        private bool DoPostLogin()
        {
            try
            {
                objectManager.Connect(this);

                LoggedInUserInfo = objectManager.getUserInfo();

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
