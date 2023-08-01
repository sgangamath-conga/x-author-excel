using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Win32;
using Apttus.OAuthLoginControl.Modules;
using Apttus.OAuthLoginControl.Helpers;
using Apttus.OAuthLoginControl.Forms;
using System.Net;

namespace Apttus.OAuthLoginControl {

    /// <summary>
    /// 
    /// </summary>
    public class ApttusOAuth {

        #region Attributes

        public TokenResponse token;
        public ObjectsResponse sObject;
        public TokenIDResponse sTokenIDResponseObject;
        public ChatterResponse sChatterObject;
        public ChatterFeedResponse sChatterFeedResponseObject;
        public ChatterGroupResponse sChatterGroupResponseObject;

        public SettingsManager.ApplicationSettings ApplicationSettings = null;
        public ApplicationInformation ApplicationInfo = null;
        private ApttusOAuthLoginForm MyLoginOAuth = null;

        public string clientID = Properties.Settings.Default.clientId;// Chatter Test
        public string clientSecret = Properties.Settings.Default.clientSecret; // Chatter Test

        // Redirect URL configured in SFDC account
        public string redirectURL = "xauthor:callback";
        public string displayParam = "touch"; //"mobile";
        public string version = "50.0";
        private string connectionEndPoint = "https://login.salesforce.com";
        private string lastLoginHint = string.Empty;

        #endregion

        #region Properties

        System.Net.IWebProxy proxy = null;
        string hostURI = string.Empty;
        string tokenValue = string.Empty;
        string currentUser = string.Empty;
        string switchLoginId = string.Empty;
        bool userClosedForm = false;
        bool isLoggedIn = false;

        public string ConnectionEndPoint {
            get { return connectionEndPoint; }
            set { connectionEndPoint = value; }
        }

        public string LastLoginHint {
            get { return lastLoginHint; }
            set { lastLoginHint = value; }
        }

        public System.Net.IWebProxy Proxy {
            get { return proxy; }
            set { proxy = value; }
        }

        public string HostURI {
            get { return hostURI; }
            set { hostURI = value; }
        }

        public string TokenValue {
            get { return tokenValue; }
            set { tokenValue = value; }
        }

        public string CurrentUser {
            get { return currentUser; }
            set { currentUser = value; }
        }

        public string SwitchLoginId {
            get { return switchLoginId; }
            set { switchLoginId = value; }
        }

        public bool UserClosedForm {
            get { return userClosedForm; }
            set { userClosedForm = value; }
        }

        public bool IsLoggedIn {
            get { return isLoggedIn; }
            set { isLoggedIn = value; }
        }

        public bool IsLoginFormVisible {
            get { return (MyLoginOAuth != null && !MyLoginOAuth.IsDisposed && MyLoginOAuth.Visible ); }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationSettings"></param>
        public ApttusOAuth(SettingsManager.ApplicationSettings applicationSettings) {

            this.ApplicationSettings = applicationSettings;
            this.ApplicationInfo = new ApplicationInformation();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Login method to complete the Login flow from subscribing application.
        /// </summary>
        /// <param name="sMode">"Login" value for param indicates to complete the Login flow. 
        ///                     "Authorize" value for param should authorize given login ID and Server host.</param>
        /// <param name="sOfficeApp">Indicate subscribing Office application, i.e. Word, Excel, Powerpoint or Outlook.</param>
        public void Login() {

            // Assign Office application type Word, Excel, Powerpoint or Outlook
            //m_App = OfficeApp;

            MyLoginOAuth = new ApttusOAuthLoginForm(this);
            MyLoginOAuth.Show();
            MyLoginOAuth.InitializeLogin(ApttusGlobals.LoginMode.Login);

            // Wait for token before giving control back to calling application
            while (token == null) {

                System.Windows.Forms.Application.DoEvents();

                if (userClosedForm)
                    break;
            }
        }

        /// <summary>
        /// Switch Login from Manage Connections form.
        /// </summary>
        public void SwitchLogin() {

            // Assign Office application type Word, Excel, Powerpoint or Outlook
            //m_App = OfficeApp;

            MyLoginOAuth = new ApttusOAuthLoginForm(this);
            MyLoginOAuth.Show();
            MyLoginOAuth.InitializeLogin(ApttusGlobals.LoginMode.SwitchLogin);

            // Wait for token before giving control back to calling application
            while (token == null) {

                System.Windows.Forms.Application.DoEvents();

                if (userClosedForm)
                    break;
            }
        }

        /// <summary>
        /// Display Manage Connections form
        /// </summary>
        /// <returns></returns>
        public ApttusGlobals.ManageConnectionResult ShowManageConnections() {

            ApttusGlobals.ManageConnectionResult manageConnectionResult = ApttusGlobals.ManageConnectionResult.DoNothing;
            ApttusManageConnections ManageConnections = new ApttusManageConnections(this);
            System.Windows.Forms.DialogResult dialogResult = ManageConnections.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.Yes) {

                manageConnectionResult = ApttusGlobals.ManageConnectionResult.SwitchLogin;
            }

            return manageConnectionResult;
        }

        /// <summary>
        /// While logging out user set OAuth token value to blank
        /// </summary>
        /// <param name="userName"></param>
        public void LogOut(string userName) {

            this.ApplicationSettings.AppLogin.Connections[userName].OAuthToken = string.Empty;
            token = null; 
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseLoginForm() {

            MyLoginOAuth.Close();
        }

        /// <summary>
        /// Get objects response
        /// </summary>
        /// <returns></returns>
        public bool GetSObjectInfo() {

            // Read the REST resources
            string s = HttpGet(token.instance_url + @"/services/data/v" + version + "/", "");

            if (s == "401")
                return false;

            // Convert the JSON response into a SObject object
            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser = new JavaScriptSerializer();
            sObject = ser.Deserialize<ObjectsResponse>(s);

            return true;
        }

        /// <summary>
        /// Build authorize URI
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizeURI() {

            // Build authorization URI
            var authURI = new StringBuilder();
            authURI.Append(connectionEndPoint + "/services/oauth2/authorize?");
            authURI.Append("response_type=code"); // token code
            authURI.Append("&client_id=" + clientID);
            authURI.Append("&redirect_uri=" + redirectURL);
            authURI.Append("&display=" + displayParam);
            authURI.Append("&immediate=false");
            authURI.Append("&scope=id%20api%20refresh_token%20full%20web");

            if (!String.IsNullOrEmpty(lastLoginHint))
                authURI.Append("&login_hint=" + System.Web.HttpUtility.UrlEncode(lastLoginHint));

            authURI.Append("&refreshTime=" + DateTime.Now.Ticks.ToString());

            return authURI.ToString();
        }

        /// <summary>
        /// Build refresh token URI, get token value and complete flow for Login process.
        /// </summary>
        public bool GetTokenRefresh() {

            string URI = connectionEndPoint + "/services/oauth2/token?";

            StringBuilder body = new StringBuilder();
            body.Append("grant_type=refresh_token");
            body.Append("&client_id=" + clientID);
            body.Append("&client_secret=" + clientSecret);
            body.Append("&refresh_token=" + token.refresh_token);
            body.Append("&refreshTime=" + DateTime.Now.Ticks.ToString());

            string result = HttpPost(URI, body.ToString());

            if (result.Equals("400")) {

                return false;
            }

            // Convert the JSON response into a token object
            DeSerializeToken(result);

            // Convert the JSON response into a SChatterObject object
            DeserializeTokenIdResponse();

            GetSObjectInfo();

            return true;
        }

        /// <summary>
        /// Revoke User Access
        /// </summary>
        /// <param name="code"></param>
        public bool RevokeAccess(string sourceURL, string access_token) {

            // Create the message used to request a token
            string URI = sourceURL + "/services/oauth2/revoke?";

            StringBuilder body = new StringBuilder();
            body.Append("token=" + access_token);
            body.Append("&callback=" + redirectURL);
            body.Append("&refreshTime=" + DateTime.Now.Ticks.ToString());
            
            string result = HttpPost(URI, body.ToString());

            if (result.Equals("400")) {

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get OAuth token value for the first login
        /// </summary>
        /// <param name="code"></param>
        public bool GetToken(string code) {

            // Create the message used to request a token
            string URI = connectionEndPoint + "/services/oauth2/token?";

            StringBuilder body = new StringBuilder();
            body.Append("code=" + code);
            body.Append("&grant_type=authorization_code"); // refresh_token; password
            body.Append("&client_id=" + clientID);
            body.Append("&client_secret=" + clientSecret);
            body.Append("&redirect_uri=" + redirectURL);
            body.Append("&refreshTime=" + DateTime.Now.Ticks.ToString());

            string result = HttpPost(URI, body.ToString());

            if (result.Equals("400")) {

                return false;
            }

            // Convert the JSON response into a token object
            DeSerializeToken(result);
            
            // Convert the JSON response into a SChatterObject object
            DeserializeTokenIdResponse();

            GetSObjectInfo();

            return true;
        }

        /// <summary>
        /// Deserialize Token ID response for chatter object
        /// </summary>
        public void DeserializeTokenIdResponse() {

            string s = HttpGet(token.id + "?version=" + version, "");

            // Convert the JSON response into a SChatterObject object
            JavaScriptSerializer ser = new JavaScriptSerializer();
            sTokenIDResponseObject = ser.Deserialize<TokenIDResponse>(s);

            if (sTokenIDResponseObject != null)
                lastLoginHint = sTokenIDResponseObject.username;
        }
        
        /// <summary>
        /// Get last login information for given office App, if Login info and token exists initiate login flow.
        /// </summary>
        /// <returns></returns>
        public bool GetLastLogin(ApttusGlobals.LoginMode loginMode) {

            if (loginMode == ApttusGlobals.LoginMode.SwitchLogin) {

                return GetSwitchLogin();
            }

            // Verify if last login value exists, if yes get OAuth token sub key value
            currentUser = this.ApplicationSettings.AppLogin.LastUsedConnection;
            this.connectionEndPoint = this.ApplicationSettings.AppLogin.Connections[currentUser].ServerHost;
            this.lastLoginHint = this.ApplicationSettings.AppLogin.Connections[currentUser].LastLoginHint;

            // Verify if OAuth token value for last logged in user exists, if yes deserialize token value
            string OAuthToken = this.ApplicationSettings.AppLogin.Connections[currentUser].OAuthToken;

            if (!String.IsNullOrEmpty(OAuthToken)) {

                // Decrypt OAuth Token value
                OAuthToken = Helpers.StringCipher.decryptString(OAuthToken);

                DeSerializeToken(OAuthToken);
                return true;
            }
            else {

                return false;
            }
        }
        
        /// <summary>
        /// Create entry in registry for logged in user and update last logged-in user info
        /// </summary>
        public void SaveOptions(string userName) {

            string sTokenValue = string.Empty;

            if (ApplicationInfo.SaveInRegistry) {

                sTokenValue = Helpers.StringCipher.encryptString(tokenValue);
            }

            this.ApplicationSettings.AppLogin.Connections[userName].OAuthToken = sTokenValue;
            this.ApplicationSettings.AppLogin.Connections[userName].ServerHost = connectionEndPoint;
            this.ApplicationSettings.AppLogin.Connections[userName].LastLoginHint = lastLoginHint;
            this.ApplicationSettings.AppLogin.LastUsedConnection = userName;
            this.ApplicationSettings.Save();
        }

        /// <summary>
        /// Returns application information such as Application Name, Logo and Description.
        /// </summary>
        /// <returns></returns>
        public ApplicationInformation GetApplicationInfo() {

            return ApplicationInfo;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deserialize access token object
        /// </summary>
        /// <param name="result"></param>
        private void DeSerializeToken(string result) {

            if (result != null && result.Length > 0) {

                JavaScriptSerializer ser = new JavaScriptSerializer();
                token = ser.Deserialize<TokenResponse>(result);

                // Store token value to be stored in Registry
                tokenValue = result;
            }
        }
       
        /// <summary>
        /// Do HTTP Post method
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        private string HttpPost(string URI, string Parameters) {

            try {
            
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);

                if (proxy != null)
                    req.Proxy = proxy;

                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                // Add parameters to post
                byte[] data = System.Text.Encoding.ASCII.GetBytes(Parameters);
                req.ContentLength = data.Length;

                using (System.IO.Stream os = req.GetRequestStream()) {

                    os.Write(data, 0, data.Length);
                }

                // Do the post and get the response.
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;

                // Store hostURI for cookie
                hostURI = resp.ResponseUri.Host;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream())) {

                    return sr.ReadToEnd().Trim();
                }
            }
            catch (WebException ex) {

                // If error code 400 (HttpStatusCode.BadRequest) then might be that the refersh token is not valid or session revoked in salesforce.
                if (ex.Response != null && (ex.Response.GetType() == typeof(HttpWebResponse)) && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.BadRequest)
                    return "400";
                else
                    return ex.Message.ToString();
            }
            catch (Exception ex) {

                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// Do HTTP Get method
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        private string HttpGet(string URI, string Parameters) {

            try {

                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);

                if (proxy != null)
                    req.Proxy = proxy;

                req.Method = "GET";
                req.Headers.Add("Authorization: OAuth " + token.access_token);

                // Capture error if 401 unauthorized , initiate refresh token flow to get new access token
                System.Net.WebResponse resp = req.GetResponse();

                if (resp == null) return null;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream())) {

                    return sr.ReadToEnd().Trim();
                }
            }
            catch (WebException ex) {

                // If error code is 401 (HttpStatusCode.Unauthorized) then get new access token via refresh token.
                if (ex.Response != null && (ex.Response.GetType() == typeof(HttpWebResponse)) && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                    return "401";
                else
                    return ex.Message.ToString();
            }
            catch (Exception ex) {

                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetSwitchLogin() {

            // Verify if OAuth token value for the Switch login user exists, if yes deserialize token value
            string OAuthToken = this.ApplicationSettings.AppLogin.Connections[switchLoginId].OAuthToken; 
            currentUser = switchLoginId;

            if (!String.IsNullOrEmpty(OAuthToken)) {

                // Decrypt OAuth Token value
                OAuthToken = Helpers.StringCipher.decryptString(OAuthToken);

                DeSerializeToken(OAuthToken);
                return true;
            }
            else {

                return false;
            }
        }

        #endregion

    }

    #region "JSON Class"
    public class TokenIDResponse
    {
        public string id { get; set; }
        public string asserted_user { get; set; }
        public string user_id { get; set; }
        public string organization_id { get; set; }
        public string username { get; set; }
        public string nick_name { get; set; }
        public string display_name { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public TokenIDStatus status { get; set; }
        public TokenIDPhotos photos { get; set; }
        public TokenIDURLs urls { get; set; }
        public string active { get; set; }
        public string user_type { get; set; }
        public string language { get; set; }
        public string locale { get; set; }
        public string utcOffset { get; set; }
        public DateTime last_modified_date { get; set; }
    }

    public class TokenIDStatus
    {
        public DateTime? created_date { get; set; }
        public string body { get; set; }
    }

    public class TokenIDPhotos
    {
        public string picture { get; set; }
        public string thumbnail { get; set; }
    }

    public class TokenIDURLs
    {
        public string enterprise { get; set; }
        public string metadata { get; set; }
        public string partner { get; set; }
        public string rest { get; set; }
        public string sobjects { get; set; }
        public string search { get; set; }
        public string query { get; set; }
        public string recent { get; set; }
        public string profile { get; set; }
        public string feeds { get; set; }
        public string groups { get; set; }
        public string users { get; set; }
        public string feed_items { get; set; }
    }

    public class TokenResponse
    {
        public string id { get; set; }
        public string issued_at { get; set; }
        public string refresh_token { get; set; }
        public string instance_url { get; set; }
        public string signature { get; set; }
        public string access_token { get; set; }
        public string scope { get; set; }
    }

    public class ObjectsResponse
    {
        public string sobjects { get; set; }
        public string identity { get; set; }
        public string connect { get; set; }
        public string search { get; set; }
        public string query { get; set; }
        public string chatter { get; set; }
        public string recent { get; set; }
    }

    public class ChatterResponse
    {
        public string groups { get; set; }
        public string users { get; set; }
        public string organization { get; set; }
        public string feeds { get; set; }
    }

    public class ChatterFeedResponse
    {
        public ChatterAvailableFeeds[] feeds { get; set; }
    }

    public class ChatterAvailableFeeds
    {
        public string label { get; set; }
        public string feedItemsUrl { get; set; }
        public string feedUrl { get; set; }
    }

    public class ChatterGroupResponse
    {
        public ChatterAvailableGroups[] groups { get; set; }
    }

    public class ChatterAvailableGroups
    {
        public string name { get; set; }
        public GroupOwner owner { get; set; }
        public string description { get; set; }
        public string visibility { get; set; }
        public int memberCount { get; set; }
        public int fileCount { get; set; }
        public bool canHaveChatterGuests { get; set; }
        public MySubscription mySubscription { get; set; }
        public string myRole { get; set; }
        public photo photo { get; set; }
        public DateTime lastFeedItemPostDate { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string type { get; set; }
    }

    public class GroupOwner
    {
        public string name { get; set; }
        public string title { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public string companyName { get; set; }
        public MySubscription mySubscription { get; set; }
        public photo photo { get; set; }
        public bool isChatterGuest { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string type { get; set; }
    }

    public class MySubscription
    {
        public string id { get; set; }
        public string url { get; set; }
    }

    public class photo
    {
        public string largePhotoUrl { get; set; }
        public string photoVersionId { get; set; }
        public string smallPhotoUrl { get; set; }
    }
    #endregion
}
