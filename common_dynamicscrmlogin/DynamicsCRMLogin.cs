using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Apttus.SettingsManager;

namespace Apttus.DynamicsCRM.LoginControl
{
    public class DynamicsCRMWebLogin
    {
        internal const string CLIENT_ID = "6d8e68a0-52fb-4957-8518-7aef14ad297b";
        internal const string REDIRECT_URL = "http://xauthor.excel.redirect";
        //internal const string AUTHORITYURI = "https://login.windows.net/common/oauth2/authorize";
        // After upgrading ADAL from 3.19 to 4.4 above URL doesn't work, per MS there were no breaking changes, but this has been acknowledged here.
        // https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/issues/1346
        //internal const string AUTHORITYURI = "https://login.microsoftonline.com/common/oauth2/authorize";
        internal const string AUTHORITYURI = "https://login.microsoftonline.com/common";

        internal Apttus.SettingsManager.ApplicationSettings ApplicationSettings;

        public Tuple<string, string> ConnectionEndPoint { get; set; }
        public System.Net.IWebProxy Proxy { get; set; }
        public IOrganizationService OrganizationService { get; private set; }

        public DateTimeOffset? TokenExpiresOn { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public string ClientID { get { return CLIENT_ID; } }
        public string RedirectURL { get { return REDIRECT_URL; } }
        public string AuthorityURL { get { return AUTHORITYURI; } }

        public DynamicsCRMWebLogin(Apttus.SettingsManager.ApplicationSettings applicationSettings)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            ApplicationSettings = applicationSettings;
        }

        public ManageConnectionResult ShowManageConnections()
        {
            ManageConnectionResult manageConnectionResult = ManageConnectionResult.DoNothing;
            ApttusManageConnections ManageConnections = new ApttusManageConnections(this);
            System.Windows.Forms.DialogResult dialogResult = ManageConnections.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                manageConnectionResult = ManageConnectionResult.SwitchLogin;

            return manageConnectionResult;
        }

        public string Login()
        {
            try
            {
                Uri oUri = new Uri(ConnectionEndPoint.Item2 + "/XRMServices/2011/Organization.svc/web");
                AuthenticationContext authContext = new AuthenticationContext(AUTHORITYURI);
                IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.RefreshSession);


                AuthenticationResult authResult = authContext.AcquireTokenAsync(ConnectionEndPoint.Item2, CLIENT_ID, new Uri(REDIRECT_URL), platformParameters).Result;
                TokenExpiresOn = authResult.ExpiresOn;
                OrganizationWebProxyClient webServiceProxy = new OrganizationWebProxyClient(oUri, new TimeSpan(0, 15, 0), false);
                webServiceProxy.HeaderToken = authResult.AccessToken;
                webServiceProxy.SdkClientVersion = webServiceProxy.GetType().Assembly.CustomAttributes.ToList()
                                                                            .Where(attr => attr.AttributeType.ToString().Equals("System.Reflection.AssemblyFileVersionAttribute"))
                                                                            .FirstOrDefault().ConstructorArguments[0].Value.ToString();

                if (Proxy != null)
                {
                    if (webServiceProxy.Endpoint.Binding is System.ServiceModel.BasicHttpBinding)
                    {
                        (webServiceProxy.Endpoint.Binding as System.ServiceModel.BasicHttpBinding).UseDefaultWebProxy = true;
                    }

                    System.Net.WebRequest.DefaultWebProxy = Proxy;
                }

                System.Net.WebRequest.DefaultWebProxy = Proxy;

                OrganizationService = webServiceProxy;

                return authResult.UserInfo.DisplayableId;
            }
            catch (Exception ex)
            {
                if (CanRethrowException(ex))
                    throw ex;
            }
            return string.Empty;
        }

        public string AutoLogin()
        {
            try
            {
                Uri oUri = new Uri(ConnectionEndPoint.Item2 + "/XRMServices/2011/Organization.svc/web");
                AuthenticationContext authContext = new AuthenticationContext(AUTHORITYURI);
                IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.Auto);

                //if (IsLoggedIn)
                //{
                //    if (TokenExpiresOn.HasValue && TokenExpiresOn.Value < DateTimeOffset.UtcNow)
                //        platformParameters = new PlatformParameters(PromptBehavior.Auto);
                //    else
                //        return string.Empty;
                //}

                AuthenticationResult authResult = authContext.AcquireTokenAsync(ConnectionEndPoint.Item2, CLIENT_ID, new Uri(REDIRECT_URL), platformParameters).Result;
                TokenExpiresOn = authResult.ExpiresOn;
                OrganizationWebProxyClient webServiceProxy = new OrganizationWebProxyClient(oUri, new TimeSpan(0, 15, 0), false);
                webServiceProxy.HeaderToken = authResult.AccessToken;
                webServiceProxy.SdkClientVersion = webServiceProxy.GetType().Assembly.CustomAttributes.ToList()
                                                                            .Where(attr => attr.AttributeType.ToString().Equals("System.Reflection.AssemblyFileVersionAttribute"))
                                                                            .FirstOrDefault().ConstructorArguments[0].Value.ToString();

                if (Proxy != null)
                {
                    if (webServiceProxy.Endpoint.Binding is System.ServiceModel.BasicHttpBinding)
                    {
                        (webServiceProxy.Endpoint.Binding as System.ServiceModel.BasicHttpBinding).UseDefaultWebProxy = true;
                    }

                    System.Net.WebRequest.DefaultWebProxy = Proxy;
                }

                OrganizationService = webServiceProxy;

                return authResult.UserInfo.DisplayableId;
            }
            catch (Exception ex)
            {
                if (CanRethrowException(ex))
                    throw ex;
            }
            return string.Empty;
        }

        private bool CanRethrowException(Exception ex)
        {
            //this is needed because if the user closes the Dynamics CRM Login UI Dialog, it will still throw an exception and if we re-throw that exception, it will be shown in the Taskbar Notification area via a ballon message.
            if (ex.InnerException != null && ex.InnerException.Message.ToLower() == "user canceled authentication")
                return false;
            return true; //We need to throw and notify the user of all other exceptions other than the above and inform the user what went wrong.
        }
    }

    //public class ConnectionHelper
    //{
    //    //const string CLIENT_ID = "83810e3e-ba76-4590-abc9-6207133c9ccc";
    //    const string CLIENT_ID = "6d8e68a0-52fb-4957-8518-7aef14ad297b";
    //    const string REDIRECT_URL = "http://xauthor.excel.redirect";
    //    const string AUTHORITYURI = "https://login.windows.net/common/oauth2/authorize";

    //    ///// <summary>
    //    ///// Display Manage Connections form
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //public static ManageConnectionResult ShowManageConnections()
    //    //{
    //    //    ManageConnectionResult manageConnectionResult = ManageConnectionResult.DoNothing;
    //    //    ApttusManageConnections ManageConnections = new ApttusManageConnections();
    //    //    DialogResult dialogResult = ManageConnections.ShowDialog();
    //    //    if (dialogResult == DialogResult.Yes)
    //    //    {
    //    //        manageConnectionResult = ManageConnectionResult.SwitchLogin;
    //    //    }

    //    //    return manageConnectionResult;
    //    //}

    //    ///// <summary>
    //    ///// 
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //public static string InitializeConnection()
    //    //{
    //    //    try
    //    //    {
    //    //        Uri oUri = new Uri(Globals.ThisAddIn.applicationConfigManager.ConnectionEndPoint.Item2 + "/XRMServices/2011/Organization.svc/web");
    //    //        AuthenticationContext authContext = new AuthenticationContext(AUTHORITYURI);
    //    //        IPlatformParameters platformParameters = new PlatformParameters(PromptBehavior.RefreshSession);

    //    //        if (Globals.ThisAddIn.applicationConfigManager.IsLoggedIn)
    //    //        {

    //    //            if (Globals.ThisAddIn.applicationConfigManager.TokenExpiresOn.HasValue &&
    //    //                Globals.ThisAddIn.applicationConfigManager.TokenExpiresOn.Value < DateTimeOffset.UtcNow)
    //    //            {
    //    //                platformParameters = new PlatformParameters(PromptBehavior.Auto);
    //    //            }
    //    //            else
    //    //            {
    //    //                return string.Empty;
    //    //            }
    //    //        }

    //    //        AuthenticationResult authResult = authContext.AcquireTokenAsync(Globals.ThisAddIn.applicationConfigManager.ConnectionEndPoint.Item2, CLIENT_ID, new Uri(REDIRECT_URL), platformParameters).Result;
    //    //        Globals.ThisAddIn.applicationConfigManager.TokenExpiresOn = authResult.ExpiresOn;
    //    //        OrganizationWebProxyClient webServiceProxy = new OrganizationWebProxyClient(oUri, false);
    //    //        webServiceProxy.HeaderToken = authResult.AccessToken;
    //    //        webServiceProxy.SdkClientVersion = webServiceProxy.GetType().Assembly.CustomAttributes.ToList()
    //    //                                                                    .Where(attr => attr.AttributeType.ToString().Equals("System.Reflection.AssemblyFileVersionAttribute"))
    //    //                                                                    .FirstOrDefault().ConstructorArguments[0].Value.ToString();

    //    //        Globals.ThisAddIn.applicationConfigManager.OrganizationService = webServiceProxy;

    //    //        return authResult.UserInfo.DisplayableId;
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        ExceptionLogHelper.ErrorLog(ex);
    //    //    }

    //    //    return string.Empty;
    //    //}

    //    /// <summary>
    //    /// Gets the exception message through all the inner exceptions
    //    /// </summary>
    //    /// <param name="exception"></param>
    //    /// <param name="message"></param>
    //    /// <returns></returns>
    //    private static string ExtractErrorMessage(Exception exception, string message)
    //    {

    //        List<string> errorMessages = new List<string>();
    //        errorMessages.Add(message);

    //        string tabs = "\n";

    //        while (exception != null)
    //        {
    //            tabs += "    ";
    //            errorMessages.Add(tabs + exception.Message);
    //            exception = exception.InnerException;
    //        }

    //        return string.Join("-\n", errorMessages);
    //    }
    //}
}
