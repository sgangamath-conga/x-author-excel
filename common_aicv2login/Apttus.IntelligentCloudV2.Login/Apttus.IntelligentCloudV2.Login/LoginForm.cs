using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Windows.Forms;
using EO.WebBrowser;
using System.Collections.Generic;

namespace Apttus.IntelligentCloudV2.Login
{
    public partial class LoginForm : Form
    {
        public string tenantURL;
        public static string userInfoPath = "/api/commonservices/accountservice/user/info";
        public delegate void CloseLoginWindow();

        public LoginForm()
        {
            InitializeComponent();
            RegisterEOBrowserLicenseKey();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        private void RegisterEOBrowserLicenseKey()
        {
            EO.WebBrowser.Runtime.AddLicense(
                "gs2xaaW0s8uud4SOscufWbOz8hfrqO7CnrWfWZekzRrxndz22hnlqJfo8h/k" +
                "dpm1wtqzaKm0wuShWer58/D3qeD29h7ArbSmwtyubaa2wd2vW5f69h3youby" +
                "zs2wcpmkwOmMQ5ekscu7rODr/wzzrunpz/DAi9rq5PzmZK61Aw3lksHxCADR" +
                "nbjBzueurODr/wzzrunpz7iJdabw+g7kp+rpz7iJdePt9BDtrNzCnrWfWZek" +
                "zRfonNzyBBDInbXH4e+vjqu6yuCza6rAwBfonNzyBBDInbWRm8ufWZfA/RTi" +
                "nuX39hC9dabw+g7kp+rp9umMQ5ekscu7muPwACK9RoGkscufWZeksefgnduk" +
                "BSTvnrSm1vqtfN/2ABjkW5f69h3youbyzg==");

            //EO.Base.Runtime.EnableEOWP = true;
        }

        private void webView1_BeforeNavigate(object sender, BeforeNavigateEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewUrl) && e.NewUrl.ToString().StartsWith(tenantURL))
            {
                if (e.NewUrl.ToString().Contains("code="))
                {
                    string code = e.NewUrl.ToString().Substring(e.NewUrl.ToString().IndexOf("code="));
                    code = code.Replace("code=", string.Empty);

                    if (code.IndexOf('&') > 0)
                    {
                        code = code.Substring(0, code.IndexOf('&'));
                    }
                    string responseJSON = GetAsString(tenantURL + userInfoPath, code);

                    DeSerializeTokenResponse(responseJSON);

                    TokenAcquiredEvent3.FireTokenAcquiredHandler(this);
                }
            }
        }

        public bool userClosedForm;
        public string tokenResponseJson;
        private void DeSerializeTokenResponse(string tokenResponse)
        {
            //AICV2TokenResponse tokenRes = JsonConvert.DeserializeObject<AICV2TokenResponse>(tokenResponse);
            //MessageBox.Show("Token acquired: " + tokenRes.access_token);
            tokenResponseJson = tokenResponse;
        }

        public string GetAsString(string url, string authcode)
        {
            var client = new HttpClient(new HttpClientHandler());

            //if (this.authResult != null)
            //{
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
            //}

            if (!string.IsNullOrEmpty(authcode))
                client.DefaultRequestHeaders.Add("Authorization-Code", authcode);
            client.DefaultRequestHeaders.Add("App-Url", tenantURL + "/ui/clm");

            var response = client.GetAsync(url).Result;

            // for unauthorized call, get login url and return
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                IEnumerable<string> values = null;
                response.Headers.TryGetValues("AuthProviderType", out values);
                string authType = values?.First();

                string redirectURL = response.Headers.Location.AbsoluteUri;
                return redirectURL;
            }
            // else ensure success and return result
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTenant.Text))
                {
                    MessageBox.Show("Please enter valid Tenant Endpoint URL");
                    return;
                }

                tenantURL = txtTenant.Text;

                string userInfoAPI = string.Empty;
                if (tenantURL.EndsWith("/"))
                    tenantURL = tenantURL.Remove(tenantURL.Length - 1);

                userInfoAPI = tenantURL + userInfoPath;
                string redirectURL = GetAsString(userInfoAPI, string.Empty);

                InitBrowser(redirectURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured: " + ex.Message.ToString());
            }
        }

        public void InitBrowser(string urlToNavigate)
        {
            webView1.Url = urlToNavigate;
        }

        public void Login(string loginUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(loginUrl))
                {
                    MessageBox.Show("Please enter valid Tenant Endpoint URL");
                    return;
                }

                tenantURL = loginUrl;

                string userInfoAPI = string.Empty;
                if (tenantURL.EndsWith("/"))
                    tenantURL = tenantURL.Remove(tenantURL.Length - 1);

                userInfoAPI = tenantURL + userInfoPath;
                string redirectURL = GetAsString(userInfoAPI, string.Empty);

                InitBrowser(redirectURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured: " + ex.Message.ToString());
            }
        }
    }

    public static class TokenAcquiredEvent3
    {
        public delegate void TokenAcquiredHandler(Form loginForm);
        //public static event TokenAcquiredHandler OnCRMChanged;

        public static void FireTokenAcquiredHandler(Form loginForm)
        {
            //loginForm.Close();
            if (loginForm.InvokeRequired)
            {
                loginForm.Invoke(new MethodInvoker(delegate { loginForm.Close(); }));
            }
        }

        internal static void FireTokenAcquiredHandler(System.Windows.Window window)
        {
            window.Close();
        }
    }
}
