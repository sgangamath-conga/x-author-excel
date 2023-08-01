using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Apttus.IntelligentCloud.LoginControl
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpClientManager
    {
        public System.Net.IWebProxy Proxy { get; set; }

        const string REDIRECT_URL = "http://xauthor.excel.redirect";
        const string API_ENDPOINT_PREFIX = "/api/clm/v1/";
        const string APP_URL_KEY = "App-Url";

        private Tuple<string, string> ConnectionEndPoint;


        /// <summary>
        /// 
        /// </summary>
        public AuthenticationResult authResult { get; set; }

        public HttpClientManager(Tuple<string, string> connectionEndPoint)
        {
            ConnectionEndPoint = connectionEndPoint;
        }

        public HttpClientManager(Tuple<string, string> connectionEndPoint, System.Net.IWebProxy proxyObject)
        {
            ConnectionEndPoint = connectionEndPoint;
            Proxy = proxyObject;
        }

        private HttpClientHandler GetHttpClientHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (Proxy != null)
                handler.Proxy = Proxy;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string PreConnectAsync(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());
            string appUrl = string.Format("{0}{1}", ConnectionEndPoint.Item2, API_ENDPOINT_PREFIX);
            client.DefaultRequestHeaders.Add(APP_URL_KEY, appUrl);

            var response = client.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //return response.Headers.Location.AbsolutePath;
                // After upgrading ADAL from 3.19 to 4.4 above URL doesn't work, per MS there were no breaking changes, but this has been acknowledged here.
                // https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/issues/1346
                // Authority URI here needs to be changed from https://login.microsoftonline.com/Guid/oauth2/authorize to https://login.microsoftonline.com/Guid
                return response.Headers.Location.Segments[1];
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xacMetadata"></param>
        /// <returns></returns>
        public void ConnectAsync(Apttus.XAuthor.Context.XAEMetadataContext xaeMetadata, string authorityUri)
        {
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.RefreshSession);
            Uri redirectURI = new Uri(REDIRECT_URL);
            authResult = authContext.AcquireTokenAsync(xaeMetadata.ResourceAppKey, xaeMetadata.ClientAppKey, redirectURI, platformParameters).Result;
        }
        public void AutoConnectAsync(Apttus.XAuthor.Context.XAEMetadataContext xaeMetadata, string authorityUri)
        {
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.Auto);
            Uri redirectURI = new Uri(REDIRECT_URL);
            authResult = authContext.AcquireTokenAsync(xaeMetadata.ResourceAppKey, xaeMetadata.ClientAppKey, redirectURI, platformParameters).Result;
        }
        public void ConnectWithUserCredential(Apttus.XAuthor.Context.XAEMetadataContext xaeMetadata, string authorityUri, UserCredential credential)
        {
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.RefreshSession);
            Uri redirectURI = new Uri(REDIRECT_URL);
            //authResult = authContext.AcquireTokenAsync(xaeMetadata.ResourceAppKey, xaeMetadata.ClientAppKey, redirectURI, platformParameters).Result;
            authResult = authContext.AcquireTokenAsync(xaeMetadata.ResourceAppKey, xaeMetadata.ClientAppKey, credential).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpResponseMessage GetAsync(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());

            if (authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
            }

            string appUrl = string.Format("{0}{1}", ConnectionEndPoint.Item2, API_ENDPOINT_PREFIX);
            client.DefaultRequestHeaders.Add(APP_URL_KEY, appUrl);

            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetAsStringAsync(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());

            if (authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
            }

            client.DefaultRequestHeaders.Add("X_ConnectTo", "X-Author-CRM");

            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                string res = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(res) && res.ToLower().Contains("xae config is undefined"))
                {
                    throw new AICLoginException("XAE_Config_Missing");
                }
            }
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string PostAsStringAsync(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());

            if (authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authResult.AccessTokenType, authResult.AccessToken);
            }

            client.DefaultRequestHeaders.Add("X_ConnectTo", "X-Author-CRM");

            var response = client.PostAsync(url, null).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
