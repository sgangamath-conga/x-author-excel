using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Apttus.IntelligentCloud.LoginControl;
using Apttus.XAuthor.AppDesigner.Modules;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public class XAuthorAICLogin : XAuthorBaseCRMLogin , IXAuthorAICLogin
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

                if (aicLogin.Login()  && aicLogin.AICAuthenticationResult != null)
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
        public override string GetApplicationURI(string appUniqueId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("/AppId=").Append(appUniqueId).Append("|")
              .Append("/ExportId=").Append(string.Empty).Append("|")
              .Append("/InstanceURL=").Append(TenantURL).Append("|")
              .Append("/SessionId=").Append(aicLogin.AICAuthenticationResult.AccessToken);

            return sb.ToString();
        }
        protected override void NotifyLogin()
        {
            ConnectionURL = TenantURL;
            applicationConfigManager.ApplicationSettings.Save();

            base.NotifyLogin();
        }

        public Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult AuthenticationResult
        {
            get
            {
                return aicLogin.AICAuthenticationResult;
            }
        }

        public string TenantURL
        {
            get
            {
                return aicLogin.ConnectionEndPoint.Item2;
            }
        }

        public string AuthorityURI {
            get
            {
                return aicLogin.GetAuthorityURI;
            }
        }

        public string ClientAppKey {
            get
            {
                return aicLogin.ClientAppKey;
            }
        }

        public string ResourceAppKey {
            get
            {
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
