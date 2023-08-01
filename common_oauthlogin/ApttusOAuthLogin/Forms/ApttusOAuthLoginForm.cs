using Apttus.OAuthLoginControl.Modules;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Apttus.OAuthLoginControl.Forms {

    public partial class ApttusOAuthLoginForm : Form {

        // Code an token generated during authentication
        private string code;

        // instance of the oAuthModule
        private ApttusOAuth OAuthModule = null;
        
        //[DllImport("wininet.dll", SetLastError = true)]
        //private static extern long DeleteUrlCacheEntry(string lpszUrlName);
        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
        private const int INTERNET_HANDLE_TYPE_INTERNET = 1;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        public delegate void SetProgressTextCallBack(string text);
        public delegate void SetProgressStatusCallBack(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOAuth"></param>
        public ApttusOAuthLoginForm(ApttusOAuth objOAuth) {

            this.OAuthModule = objOAuth;
            this.Text = OAuthModule.ApplicationInfo.ApplicationName;

            InitializeComponent();
            RegisterEOBrowserLicenseKey();
            

            Helpers.ApplicationInformation appInfo = objOAuth.ApplicationInfo;
            
            lblAboutDescription.Text = appInfo.ApplicationDescription;
            this.Icon = appInfo.ApplicationIcon == null ? Icon.FromHandle(((Bitmap)appInfo.ApplicationLogo).GetHicon()) :
                            appInfo.ApplicationIcon;
            pbApttusLogo.Image = appInfo.ApplicationLogo;
            this.Text = appInfo.ApplicationName;
        }

        /// <summary>
        /// Add Registry EO Browser licensekey
        /// </summary>
        private void RegisterEOBrowserLicenseKey() {
            EO.WebBrowser.Runtime.AddLicense(
                "gs2xaaW0s8uud4SOscufWbOz8hfrqO7CnrWfWZekzRrxndz22hnlqJfo8h/k" +
                "dpm1wtqzaKm0wuShWer58/D3qeD29h7ArbSmwtyubaa2wd2vW5f69h3youby" +
                "zs2wcpmkwOmMQ5ekscu7rODr/wzzrunpz/DAi9rq5PzmZK61Aw3lksHxCADR" +
                "nbjBzueurODr/wzzrunpz7iJdabw+g7kp+rpz7iJdePt9BDtrNzCnrWfWZek" +
                "zRfonNzyBBDInbXH4e+vjqu6yuCza6rAwBfonNzyBBDInbWRm8ufWZfA/RTi" +
                "nuX39hC9dabw+g7kp+rp9umMQ5ekscu7muPwACK9RoGkscufWZeksefgnduk" +
                "BSTvnrSm1vqtfN/2ABjkW5f69h3youbyzg==");
            
            EO.Base.Runtime.EnableEOWP = true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginMode"></param>
        public void InitializeLogin(ApttusGlobals.LoginMode loginMode) {

            if (!OAuthModule.GetLastLogin(loginMode)) {

                if (!String.IsNullOrEmpty(OAuthModule.CurrentUser)) {
                    this.Text = OAuthModule.ApplicationInfo.ApplicationName + " (" + OAuthModule.CurrentUser + ")";
                }
                ProgressIndicatorStop();
                // Build authorization URI
                string authURI = OAuthModule.GetAuthorizeURI();
                Uri uri = null;
                Uri.TryCreate(authURI, UriKind.RelativeOrAbsolute, out uri);                
                // Redirect the browser control the the SFDC login page                
                WebViewContent.LoadUrl(uri.ToString());                

            }
            else {
                ProgressIndicatorStart();
                string lastLogin = OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection;

                if(OAuthModule.ApplicationInfo.UseResources)
                    lblProgressStatus.Text = string.Format(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_ProgressBar_Label"),(String.IsNullOrEmpty(OAuthModule.SwitchLoginId) ? lastLogin : OAuthModule.SwitchLoginId));
                else
                    lblProgressStatus.Text = "Connecting as " + (String.IsNullOrEmpty(OAuthModule.SwitchLoginId) ? lastLogin : OAuthModule.SwitchLoginId) + ", Please wait...";


                this.Text = OAuthModule.ApplicationInfo.ApplicationName + " (" + (String.IsNullOrEmpty(OAuthModule.SwitchLoginId) ? lastLogin : OAuthModule.SwitchLoginId) + ")";

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWorker_DoWork);
                bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

                bgWorker.RunWorkerAsync();

                while(bgWorker.IsBusy) {

                    Cursor.Current = Cursors.WaitCursor;
                    Application.DoEvents();
                }
            }
        }               
        protected override void OnLoad(EventArgs e) {              
                
            base.OnLoad(e);
        }
        private void WebViewContent_NewWindow(object sender, EO.WebBrowser.NewWindowEventArgs e) {
            e.Accepted = false;
            WebViewContent.LoadUrl(e.TargetUrl);
        }
        private void WebViewContent_IsLoadingChanged(object sender, System.EventArgs e) {
            if(WebViewContent.IsLoading) {
                ProgressIndicatorStart();
            } else {
                ProgressIndicatorStop();
            }
        }

        private void webViewContent_BeforeNavigate(object sender, EO.WebBrowser.BeforeNavigateEventArgs e) {
            // Wait for a redirect to your callback URL
            if(!string.IsNullOrWhiteSpace(e.NewUrl)) {
                if(e.NewUrl.ToString().StartsWith(OAuthModule.redirectURL)) {
                    
                    string lastLogin = OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection;
                    if(OAuthModule.ApplicationInfo.UseResources)
                        this.SetProgressStatus(string.Format(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_ProgressBar_Label"), (String.IsNullOrEmpty(OAuthModule.SwitchLoginId) ? lastLogin : OAuthModule.SwitchLoginId)));
                    else
                        this.SetProgressStatus("Connecting as " + (String.IsNullOrEmpty(OAuthModule.SwitchLoginId) ? lastLogin : OAuthModule.SwitchLoginId) + ", Please wait...");
                                       
                    Application.DoEvents();

                    BackgroundWorker bgWorker = new BackgroundWorker();
                    bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(bgWorker_DoWork);
                    bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
                    if(e.NewUrl.ToString().Contains("code=")) {                       
                        code = e.NewUrl.ToString().Substring(e.NewUrl.ToString().IndexOf("code="));
                        code = code.Replace("code=", string.Empty);

                        if(code.IndexOf('&') > 0) {
                            code = code.Substring(0, code.IndexOf('&'));
                        }

                    }
                    // Check for any errors
                    if(CheckForErrorCode(code))
                        return;

                    // Get the access token
                    if(!OAuthModule.GetToken(code))
                        return;

                    bgWorker.RunWorkerAsync();

                    while(bgWorker.IsBusy) {

                        Cursor.Current = Cursors.WaitCursor;
                        Application.DoEvents();
                    }
                }
            }

        }

        private void webViewContent_LoadFailed(object sender, EO.WebBrowser.LoadFailedEventArgs e) {
            e.UseDefaultMessage();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sCode"></param>
        /// <returns></returns>
        private bool CheckForErrorCode(string sCode) {

            bool bRetValue = false;

            /*
             * unsupported_response_type — response type not supported 
             * invalid_client_id — client identifier invalid 
             * invalid_request — HTTPS required 
             * invalid_request — must use HTTP GET 
             * invalid_request — out-of-band not supported 
             * access_denied — end-user denied authorization
             * redirect_uri_missing — redirect_uri not provided 
             * redirect_uri_mismatch — redirect_uri mismatch with remote access object 
             * immediate_unsuccessful — immediate unsuccessful 
             * invalid_grant — invalid user credentials 
             * invalid_grant — IP restricted or invalid login hours 
             * inactive_user — user is inactive 
             * inactive_org — organization is locked, closed, or suspended 
             * rate_limit_exceeded — number of logins exceeded 
             * invalid_scope — requested scope is invalid, unknown, or malformed
             */

            if (code.Contains("error=access_denied")) {
                if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_Denied_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("You have been denied authorization to the application. You will be prompted again for authorization next time you try to login.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=invalid_client_id")) {
                if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_InvalidClient_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("You have an invalid client Id to access the application. Please contact Conga support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=redirect_uri_missing") ||
                     code.Contains("error=redirect_uri_mismatch")) {

               if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ApplicationName + OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_AuthorizationUri_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               else
                    MessageBox.Show(OAuthModule.ApplicationInfo.ApplicationName + " was not able to get authorization URI from the server. This may be due to a missing or mismatch with the authorization URI. Please contact Conga support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=invalid_grant")) {

                if(OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ApplicationName + OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_InvalidGrant_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show(OAuthModule.ApplicationInfo.ApplicationName + " was not able to get you authenticated due to invalid grant access. This may be due to either IP restriction or inavalid login hours in your Org. Please contact your SFDC System Administrator or Conga support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=inactive_user")) {

                if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_InactiveUser_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("You have been denied authorization to the application. This may be due to the User is inactive. Please contact your SFDC System Administrator or Conga support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=inactive_org")) {

                if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_InactiveOrg_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("You have been denied authorization to the application. This may be due to the Org to which you're trying to connect is locked, closed, or suspended. Please contact your SFDC System Administrator or Conga support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            else if (code.Contains("error=rate_limit_exceeded")) {

                if (OAuthModule.ApplicationInfo.UseResources)
                    MessageBox.Show(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_LimitExceed_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    MessageBox.Show("You have been denied authorization to the application. This may be due to number of logins exceeded. Please contact your SFDC System Administrator or Apttus support for further updates and specify this error message.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                bRetValue = true;
            }
            
            return bRetValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            try
            {
                if (e.Cancelled)
                {
                    OAuthModule.token = null;
                    ProgressIndicatorStop();
                    // Build authorization URI
                    string authURI = OAuthModule.GetAuthorizeURI();

                    Uri uri = null;
                    Uri.TryCreate(authURI, UriKind.RelativeOrAbsolute, out uri);

                    // Redirect the browser control the the SFDC login page                                  
                    WebViewContent.LoadUrl(uri.ToString());                   

                } else
                {
                    ProgressIndicatorStart();
                    if (e.Error == null)
                    {
                        if (OAuthModule.ApplicationInfo.UseResources)
                            this.SetProgressText(OAuthModule.ApplicationInfo.ResourceManager.GetString("OAUTHLOGINFORM_ProgressStatus_Text"));
                        else
                            this.SetProgressText("Connected to Conga, Please wait...");
                    }
                }
            }
            catch (Exception ex)
            {
                // In case of thread not completed yet and LoginForm is Closed this will throw error with Cannot access disposed Object MyLoginOAuth
                // Currently exception is handled just as temporary fix, need to figure out proper sequencing of threads to resolve this issue.
            }
        }

        private void ProgressIndicatorStart() {
            UIWebBrowser.Visible = false;
            progressIndicator.Start();
            pnlAbout.Visible = true;
            pnlProgress.Visible = true;
        }
        private void ProgressIndicatorStop() {
            UIWebBrowser.Visible = true;
            progressIndicator.Stop();
            pnlAbout.Visible = false;
            pnlProgress.Visible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private void SetProgressText(string text) {

            if (this.lblProgressStatus.InvokeRequired) {

                SetProgressTextCallBack callBack = new SetProgressTextCallBack(SetProgressText);
                this.Invoke(callBack, new object[] { text });
            }
            else {

                this.lblProgressStatus.Text = text;
            }
        }

        private void SetProgressStatus(string text) {

            if(this.lblProgressStatus.InvokeRequired) {

                SetProgressStatusCallBack callBack = new SetProgressStatusCallBack(SetProgressText);
                this.Invoke(callBack, new object[] { text });
            } else {

                this.lblProgressStatus.Text = text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            try {
                if (!OAuthModule.GetSObjectInfo())  {
                    // If current token has expired use refresh token to complete login flow
                    if (!OAuthModule.GetTokenRefresh())
                        e.Cancel = true;
                }
                else
                    OAuthModule.DeserializeTokenIdResponse();
            }
            catch (Exception ex)
            {
                //If any thing goes wrong, just cancel the thread execution and bgWorker_RunWorkerCompleted will accordingly handle this.
                e.Cancel = true; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusOAuthLoginForm_Load(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusOAuthLoginForm_FormClosing(object sender, FormClosingEventArgs e) {
                if (e.CloseReason == CloseReason.UserClosing) {
                    OAuthModule.UserClosedForm = true;
                }
        }
    }
}
