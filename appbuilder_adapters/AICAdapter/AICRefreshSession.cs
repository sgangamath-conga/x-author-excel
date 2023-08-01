using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace Apttus.XAuthor.AICAdapter
{
    public class AICV2RefreshSession : AICRefreshSession
    {
        private static AICV2RefreshSession instance;
        private static object syncRoot = new Object();
        private AICV2ServiceController aicSevice = AICV2ServiceController.GetInstance;
        public new static AICV2RefreshSession GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new AICV2RefreshSession();
                    }
                }
                return instance;
            }
        }
        public override void RefreshConnection()
        {
            if (string.IsNullOrEmpty(aicSevice.tenantURL) || string.IsNullOrEmpty(aicSevice.TokenResponse.expires_on))
                return;
        }
    }
    public class AICRefreshSession
    {
        private static AICRefreshSession instance;
        private static object syncRoot = new Object();
        private AICServiceController aicSevice = AICServiceController.GetInstance;
        const string REDIRECT_URL = "http://xauthor.excel.redirect";
        private string ClientAppKey = string.Empty;
        private string AuthorityURI = string.Empty;
        private string ResourceAppKey = string.Empty;
        public static AICRefreshSession GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new AICRefreshSession();
                    }
                }
                return instance;
            }
        }
        public void SetRefreshSessionInfo(string ClientAppKey, string AuthorityURI, string ResourceAppKey)
        {
            this.ClientAppKey = ClientAppKey;
            this.ResourceAppKey = ResourceAppKey;
            this.AuthorityURI = AuthorityURI;
        }
        public virtual void RefreshConnection()
        {
            if (string.IsNullOrEmpty(aicSevice.tenantURL) || aicSevice.authResult.ExpiresOn == null)
                return;

            IPlatformParameters platformParameters = null;
            if (aicSevice.authResult != null)
            {
                if (aicSevice.authResult.ExpiresOn < DateTimeOffset.UtcNow)
                    platformParameters = new PlatformParameters(PromptBehavior.Auto);
                else
                    return;
            }

            AuthenticationContext authContext = new AuthenticationContext(AuthorityURI);
            AuthenticationResult authResult = authContext.AcquireTokenAsync(ResourceAppKey, ClientAppKey, new Uri(REDIRECT_URL), platformParameters).Result;
            aicSevice.authResult = authResult;
        }
    }
}
