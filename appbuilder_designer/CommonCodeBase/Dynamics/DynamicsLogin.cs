using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.DynamicsCRM.LoginControl;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppDesigner.Modules;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public class XAuthorDynamicsLogin : XAuthorBaseCRMLogin, IXAuthorDynamicsLogin
    {
        private DynamicsCRMWebLogin dynamicsLogin;
        public Tuple<string, string> ConnectionEndPoint => dynamicsLogin.ConnectionEndPoint;
        public DateTimeOffset? TokenExpiresOn => dynamicsLogin.TokenExpiresOn;
        public string ClientId => dynamicsLogin.ClientID;
        public string RedirectUrl => dynamicsLogin.RedirectURL;
        public string AuthorityURL => dynamicsLogin.AuthorityURL;
        public XAuthorDynamicsLogin()
        {
            dynamicsLogin = new DynamicsCRMWebLogin(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings);
        }

        public override void Login()
        {
            try
            {
                string lastLoginUser = applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;
                if (!string.IsNullOrEmpty(lastLoginUser))
                    dynamicsLogin.ConnectionEndPoint = new Tuple<string, string>(applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ConnectionName, applicationConfigManager.ApplicationSettings.AppLogin.Connections[lastLoginUser].ServerHost);

                dynamicsLogin.Proxy = Proxy;

                string displayableId = dynamicsLogin.Login();
                if (!string.IsNullOrEmpty(displayableId) && dynamicsLogin.OrganizationService != null)
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
            dynamicsLogin.ConnectionEndPoint = new Tuple<string, string>(userName, Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections[userName].ServerHost);
            if (string.IsNullOrEmpty(dynamicsLogin.ConnectionEndPoint.Item2))
            {
                MessageBox.Show("Invalid connection URL, please provide valid URL and try again.", Constants.PRODUCT_NAME + " - Switch Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dynamicsLogin.Login();
            if (dynamicsLogin.OrganizationService != null)
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

            ManageConnectionResult connectionResult = dynamicsLogin.ShowManageConnections();
            if (connectionResult == ManageConnectionResult.SwitchLogin)
            {
                dynamicsLogin.Login();
                if (dynamicsLogin.OrganizationService != null)
                {
                    if (DoPostLogin())
                    {
                        applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection = dynamicsLogin.ConnectionEndPoint.Item1;
                        NotifyLogin();
                    }
                }
            }
        }

        public override string GetApplicationURI(string appUniqueId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("/AppId=").Append(appUniqueId).Append("|")
              .Append("/ExportId=").Append(string.Empty).Append("|")
              .Append("/InstanceURL=").Append(dynamicsLogin.ConnectionEndPoint.Item2).Append("|")
              .Append("/SessionId=").Append(((Microsoft.Xrm.Sdk.WebServiceClient.WebProxyClient<Microsoft.Xrm.Sdk.IOrganizationService>)dynamicsLogin.OrganizationService).HeaderToken);

            return sb.ToString();
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

        protected override void NotifyLogin()
        {
            applicationConfigManager.ApplicationSettings.Save();
            base.ConnectionURL = dynamicsLogin.ConnectionEndPoint.Item2;
            base.NotifyLogin();
        }

        public override bool LoggedIn {
            get {
                return dynamicsLogin.OrganizationService != null;
            }
        }

        public Microsoft.Xrm.Sdk.IOrganizationService OrganizationService {
            get {
                return dynamicsLogin.OrganizationService;
            }
        }

        public System.Net.IWebProxy Proxy
        {
            get
            {
                return Helpers.ProxyHelper.GetProxyObject(dynamicsLogin.ConnectionEndPoint.Item2);
            }
        }
    }
}
