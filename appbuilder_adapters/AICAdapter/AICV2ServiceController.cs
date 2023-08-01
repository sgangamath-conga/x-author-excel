using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Apttus.XAuthor.AICAdapter
{
    public class AICV2TokenResponse
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
    public class AICV2ServiceController : AICServiceController
    {
        private static AICV2ServiceController instance;
        private static object syncRoot = new Object();
        AICV2TokenResponse tokenResponse;
        public AICV2TokenResponse TokenResponse { get => tokenResponse; private set => tokenResponse = value; }
        public new static AICV2ServiceController GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        instance = new AICV2ServiceController();
                    }
                }
                return instance;
            }
        }

        public override bool ConnectWithAICV2(AICV2TokenResponse tokenResponse, string TenantURL, IWebProxy proxyObject)
        {
            this.tokenResponse = tokenResponse;
            
            if (!string.IsNullOrEmpty(TenantURL))
                tenantURL = TenantURL;

            proxy = proxyObject;
            return true;
        }
        private static HttpClientHandler GetHttpClientHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (proxy != null)
                handler.Proxy = proxy;

            return handler;
        }
        public override string GetAsString(string url)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (!string.IsNullOrEmpty(tokenResponse.token_type) && !string.IsNullOrEmpty(tokenResponse.access_token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.GetAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }
        public override string PostAsString(string url, HttpContent content)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (!string.IsNullOrEmpty(tokenResponse.token_type) && !string.IsNullOrEmpty(tokenResponse.access_token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.PostAsync(url, content).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override string PutAsString(string url, HttpContent content)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (!string.IsNullOrEmpty(tokenResponse.token_type) && !string.IsNullOrEmpty(tokenResponse.access_token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.PutAsync(url, content).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public override string DeleteAsString(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());

            if (!string.IsNullOrEmpty(tokenResponse.token_type) && !string.IsNullOrEmpty(tokenResponse.access_token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.DeleteAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }
        public override byte[] GetAsByteArray(string url)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (!string.IsNullOrEmpty(tokenResponse.token_type) && !string.IsNullOrEmpty(tokenResponse.access_token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.GetAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsByteArrayAsync().Result;
        }
    }
}
