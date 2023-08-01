using System;

namespace Apttus.XAuthor.Core
{
    public interface IXAuthorDesignerMenuUpdate
    {
        /// <summary>
        ///  
        /// </summary>
        void ChangeMenuLabels();

        /// <summary>
        /// Enable/Disable the option 
        /// </summary>
        void UpdateActions();

        /// <summary>
        /// 
        /// </summary>
        void UpdateAboutText();
    }

    public interface IXAuthorCRMLogin
    {
        //If needed implement them as they are common accross all CRM
        //string UserName { get; }
        //string Password { get; }

        System.Net.IWebProxy Proxy { get; }
    }

    public interface IXAuthorSalesforceLogin : IXAuthorCRMLogin
    {
        string AccessToken { get; }
        string InstanceURL { get; }
    }

    public interface IXAuthorDynamicsLogin : IXAuthorCRMLogin
    {
        Microsoft.Xrm.Sdk.IOrganizationService OrganizationService { get; }
        Tuple<string, string> ConnectionEndPoint { get; }
        DateTimeOffset? TokenExpiresOn { get; }
        string ClientId { get; }
        string RedirectUrl { get; }
        string AuthorityURL { get; }
    }

    public interface IXAuthorAICLogin : IXAuthorCRMLogin
    {
        string TenantURL { get; }
        Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult AuthenticationResult { get; }
        string AuthorityURI { get; }
        string ClientAppKey { get; }
        string ResourceAppKey { get; }
    }
    public interface IXAuthorAICV2Login : IXAuthorCRMLogin
    {
        string TenantURL { get; }
        string TokenResponse { get; }
    }
    public interface ILookupIdAndLookupNameProvider
    {
        string GetLookupIdFromLookupName(string LookupName);
        string GetLookupNameFromLookupId(string LookupId);
        string GetLookupReference__R(string LookupId__C);
        string GetLookupReference__C(string LookupId__R);
        bool IsLookupField(ApttusField apttusField);
        bool IsLookupIdField(ApttusObject obj, ApttusField apttusField);
    }
}
