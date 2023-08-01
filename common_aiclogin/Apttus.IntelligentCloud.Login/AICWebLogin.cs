using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Context;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Apttus.IntelligentCloud.LoginControl
{
    public class AICWebLogin
    {
        internal Apttus.SettingsManager.ApplicationSettings ApplicationSettings;
        public Tuple<string, string> ConnectionEndPoint { get; set; }
        public System.Net.IWebProxy Proxy { get; set; }

        public bool IsLoggedIn { get; private set; }

        const string USERINFO_ENDPOINT = "/api/xajs/v1/xAuthor/User/UserInfo";
        const string XAE_CLIENTID_ENDPOINT = "/common/XAEMetadata";
        const string API_ENDPOINT_PREFIX = "/api/clm/v1/";
        const string APP_URL_KEY = "App-Url";

        private string UNFORMATTED_AUTHORITYURI = "https://login.microsoftonline.com/{0}";

        private string AUTHORITYURI = "https://login.microsoftonline.com/{0}";

        private HttpClientManager httpClient;
        public AuthenticationResult AICAuthenticationResult { get { return httpClient.authResult; } }

        private XAEMetadataContext xaeMetadata;
        public AICWebLogin(Apttus.SettingsManager.ApplicationSettings applicationSettings)
        {
            ApplicationSettings = applicationSettings;
        }
        public string GetAuthorityURI {
            get {
                return AUTHORITYURI;
            }
        }
        public string ClientAppKey {
            get {
                return xaeMetadata.ClientAppKey;
            }
        }
        public string ResourceAppKey {
            get {
                return xaeMetadata.ResourceAppKey;
            }
        }
        public ManageConnectionResult ShowManageConnections()
        {
            ManageConnectionResult result = ManageConnectionResult.DoNothing;
            ApttusManageConnections manageConnectionsForm = new ApttusManageConnections(this);
            if (manageConnectionsForm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                result = ManageConnectionResult.SwitchLogin;
            return result;
        }

        public bool Login()
        {
            try
            {
                httpClient = new HttpClientManager(ConnectionEndPoint, Proxy);
                return InitializeConnectionAsync();
            }
            catch (Exception ex)
            {
                if (!ex.InnerException.Message.Equals("User canceled authentication"))
                    throw ex;
                return false;
            }
        }
        public bool AutoLogin()
        {
            try
            {
                httpClient = new HttpClientManager(ConnectionEndPoint, Proxy);
                return AutoInitializeConnectionAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool LoginWithUserCredential(UserCredential credential)
        {
            try
            {
                httpClient = new HttpClientManager(ConnectionEndPoint, Proxy);
                return InitializeConnectionAsync(credential);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool InitializeConnectionAsync(UserCredential credential)
        {
            PreInitializeConnectionAsync();
            xaeMetadata = GetXAEMetadataAsync();
            httpClient.ConnectWithUserCredential(xaeMetadata, AUTHORITYURI, credential);
            //ApttusUserInfo info = GetUserInfo(); //only needed to test user info.
            return true;
        }
        private bool InitializeConnectionAsync()
        {
            PreInitializeConnectionAsync();
            xaeMetadata = GetXAEMetadataAsync();
            httpClient.ConnectAsync(xaeMetadata, AUTHORITYURI);
            //ApttusUserInfo info = GetUserInfo(); //only needed to test user info.
            return true;
        }
        private bool AutoInitializeConnectionAsync()
        {
            PreInitializeConnectionAsync();
            xaeMetadata = GetXAEMetadataAsync();
            httpClient.AutoConnectAsync(xaeMetadata, AUTHORITYURI);
            //ApttusUserInfo info = GetUserInfo(); //only needed to test user info.
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void PreInitializeConnectionAsync()
        {
            string url = string.Format("{0}{1}", ConnectionEndPoint.Item2, API_ENDPOINT_PREFIX);
            string response = httpClient.PreConnectAsync(url);

            if (!string.IsNullOrEmpty(response))
            {
                AUTHORITYURI = string.Format(UNFORMATTED_AUTHORITYURI, response);
            }
            else
            {
                throw new ApplicationException("Failed to connect...");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private XAEMetadataContext GetXAEMetadataAsync()
        {
            string url = string.Format("{0}{1}", ConnectionEndPoint.Item2, XAE_CLIENTID_ENDPOINT);
            var response = httpClient.GetAsStringAsync(url);

            if (!string.IsNullOrEmpty(response))
            {
                return JsonConvert.DeserializeObject<XAEMetadataContext>(response);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ApttusUserInfo GetUserInfo()
        {
            string url = string.Format("{0}{1}", ConnectionEndPoint.Item2, USERINFO_ENDPOINT);
            string userInfo = httpClient.PostAsStringAsync(url);
            ApttusUserInfo userInfoObj = JsonConvert.DeserializeObject<ApttusUserInfo>(userInfo);

            return userInfoObj;
        }

    }
}
