using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Linq;

namespace Apttus.XAuthor.DynamicsAdapter
{
    public sealed class DynamicsRefreshConnection
    {
        private static DynamicsRefreshConnection instance;
        private static object syncRoot = new Object();
        private DynamicsCRMServiceController serviceController = DynamicsCRMServiceController.GetInstance;

        private string RedirectURL = string.Empty;
        private string ClientId = string.Empty;
        private string AuthorityURI = string.Empty;
        private DateTimeOffset? TokenExpiresOn;
        private Tuple<string, string> ConnectionEndPoint;

        public static DynamicsRefreshConnection GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new DynamicsRefreshConnection();
                    }
                }
                return instance;
            }
        }
        public void SetRefreshSessionInfo(string RedirectURL, string ClientId, string AuthorityURI, DateTimeOffset? TokenExpiresOn, Tuple<string, string> ConnectionEndPoint)
        {
            this.RedirectURL = RedirectURL;
            this.ClientId = ClientId;
            this.AuthorityURI = AuthorityURI;
            this.TokenExpiresOn = TokenExpiresOn;
            this.ConnectionEndPoint = ConnectionEndPoint;
        }

        public void RefreshConnection()
        {

            if (string.IsNullOrEmpty(ConnectionEndPoint.Item2) || TokenExpiresOn == null)
                return;

            Uri oUri = new Uri(ConnectionEndPoint.Item2 + "/XRMServices/2011/Organization.svc/web");
            AuthenticationContext authContext = new AuthenticationContext(AuthorityURI);
            IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.RefreshSession);

            // if orgserviceproxy not null , then consider logged in
            if (serviceController.orgServiceProxy != null)
            {

                if (TokenExpiresOn.HasValue && TokenExpiresOn.Value < DateTimeOffset.UtcNow)
                {
                    platformParameters = new PlatformParameters(PromptBehavior.Auto);
                }
                else
                {
                    return;
                }
            }

            AuthenticationResult authResult = authContext.AcquireTokenAsync(ConnectionEndPoint.Item2, ClientId, new Uri(RedirectURL), platformParameters).Result;
            TokenExpiresOn = authResult.ExpiresOn;
            OrganizationWebProxyClient webServiceProxy = new OrganizationWebProxyClient(oUri, new TimeSpan(0, 15, 0), false);
            webServiceProxy.HeaderToken = authResult.AccessToken;
            webServiceProxy.SdkClientVersion = webServiceProxy.GetType().Assembly.CustomAttributes.ToList()
                                                                        .Where(attr => attr.AttributeType.ToString().Equals("System.Reflection.AssemblyFileVersionAttribute"))
                                                                        .FirstOrDefault().ConstructorArguments[0].Value.ToString();

            serviceController.orgServiceProxy = webServiceProxy;
        }
    }
}
