/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.SalesforceAdapter.AppBuilderWS;
using System.ServiceModel;
using System.Net;

namespace Apttus.XAuthor.SalesforceAdapter
{
    public sealed class AppBuilderWsController
    {
        // Static variables
        private static AppBuilderWsController instance;
        private static object syncRoot = new Object();
        private static AppBuilderWSPortTypeClient service;
        public static SessionHeader sessionHeader = null;
        public static System.Net.IWebProxy proxy = null;

        // ToDo: Get this endpoint URL from the TokenResponse class (fetched based on login).
        // const string EndPointURL = "https://na15.salesforce.com/services/Soap/class/AppBuilderWS";
        private string EndPointURL { get; set; }
        private string InstanceURL { get; set; }
        // Singleton
        public static AppBuilderWsController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        // To Support TLS 1.2 for spring 16 Org instances
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        instance = new AppBuilderWsController();
                    }
                }
                return instance;
            }
        }

        //// Private constructor
        //private AppBuilderWsController()
        //{
        //    //SetupService();
        //}

        // Create new application
        public string createApplication(Guid uniqueId, string name)
        {
            string appId = string.Empty;

            CreateAppRequest req = new CreateAppRequest();
            req.name = name;
            req.uniqueId = uniqueId.ToString();

            service.createApplication(sessionHeader, null, null, null, req, out appId);
            
            return appId;
        }
        public bool deactivateApplication(string recordId)
        {
            bool? result;
            SaveAppRequest req = new SaveAppRequest();
            req.id = recordId;
            //service.deactivateApp(sessionHeader, null, null, null, req, out result);
            //return result.HasValue ? (bool)result : false;
            return true;
        }
        // Save/Update an existing application
        public bool saveApplication(string recordId, Guid uniqueId, byte[] config, byte[] template, string templateName, byte[] schema, string edition, byte[] googleSheetSchema = null)
        {
            bool? result;

            SaveAppRequest req = new SaveAppRequest();
            req.id = recordId;

            if (uniqueId != Guid.Empty)
                req.uniqueId = uniqueId.ToString();
            if(googleSheetSchema != null)
            {
                req.googleSheetSchema = googleSheetSchema;
            }            
            req.config = config;
            req.template = template;
            req.templateName = templateName;
            req.schema = schema;
            
            //req.edition = edition;

            service.saveApplication(sessionHeader, null, null, null, req, out result);
            return result.HasValue ? (bool)result : false;
        }

        // Upsert app
        public string upsertApplication(string recordId, Guid uniqueId, string name, byte[] config, byte[] template, string templateName)
        {
            string appId = null;

            UpsertAppRequest req = new UpsertAppRequest();
            req.id = recordId;

            if (uniqueId != Guid.Empty)
                req.uniqueId = uniqueId.ToString();

            req.name = name;
            req.config = config;
            req.template = template;
            req.templateName = templateName;

            UpsertAppResult result;
            service.upsertApplication(sessionHeader, null, null, null, req, out result);

            if (result != null)
                appId = result.id;

            return appId;
        }

        // Load an application based on AppId
        public LoadAppResult loadApplication(string recordId, Guid uniqueId)
        {
            LoadAppRequest req = new LoadAppRequest();
            req.id = recordId;

            if (uniqueId != Guid.Empty)
                req.uniqueId = uniqueId.ToString();

            LoadAppResult result = null;
            service.loadApplication(sessionHeader, null, null, null, req, out result);

            // Application__c app = [SELECT Id, Edition__c FROM Application__c WHERE UniqueId__c = :req.uniqueId LIMIT 1];
            
            return result;
        }

        // Return if current user has designer license 
        public bool IsDesigner()
        {
            bool? res = null;
            service.isDesigner(sessionHeader, null, null, null, out res);

            return !res.HasValue ? false : (bool)res;
        }

        // Return client version in package
        public string GetClientVersion()
        {
            string res = null;
            service.getClientVersion(sessionHeader, null, null, null, out res);
            
            return res;
        }

        public bool? checkRuntimeLicense()
        {
            bool? res;
            service.checkRuntimeLicense(sessionHeader, null, null, null, out res);
            return res;
        }

        /// <summary>
        /// Call one method GetLicenseAndFeatureDetail(), which returns both License and feature details and return license info
        /// </summary>
        /// <returns></returns>
        public string GetLicenseDetail()
        {
            licenseResponse lmaResponse = GetLicenseAndFeatureDetail();

            return lmaResponse.licenseDetailXML;
        }

        /// <summary>
        /// Call one method GetLicenseAndFeatureDetail(), which returns both License and feature details and return feature info
        /// </summary>
        /// <returns></returns>
        public string GetFeatureDetail()
        {
            licenseResponse lmaResponse = GetLicenseAndFeatureDetail();

            return lmaResponse.licenseFeatureDetailXML;
        }

        /* not required this logic will be handled in the server
        /// <summary>
        /// 
        /// </summary>
        public void SubmitSyncLicenseAndFeatureDetail()
        {
            ActionParams actionParams = new ActionParams();
            actionParams.SessionId = sessionHeader.sessionId;
            actionParams.SessionUrl = this.InstanceURL;

            service.submitSyncLicenseAndFeatureDetail(sessionHeader, null, null, null, actionParams);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SyncLicenseAndFeatureDetail()
        {
            ActionParams actionParams = new ActionParams();
            actionParams.SessionId = sessionHeader.sessionId;
            actionParams.SessionUrl = this.InstanceURL;

            service.syncLicenseAndFeatureDetail(sessionHeader, null, null, null, actionParams);
        }


        */


        public bool ConnectWithSalesforce(string OAuthToken, string InstanceURL, string NamespacePrefix, System.Net.IWebProxy proxyObject)
        {
            if (OAuthToken != null)
            {
                // Set instanceURL from OAuthLogin
                if (string.IsNullOrEmpty(NamespacePrefix))
                    EndPointURL = InstanceURL + "/services/Soap/class/AppBuilderWS";
                else
                    EndPointURL = InstanceURL + "/services/Soap/class/" + NamespacePrefix + "/AppBuilderWS";

                this.InstanceURL = EndPointURL;
                proxy = proxyObject;
                // Pass EndpointURL in SetupService
                SetupService();
                // Set SessionHeader to call method
                sessionHeader = new SessionHeader { sessionId = OAuthToken };
                return true;
            }
            else
            {
                return false;
            }
        }

        private AppBuilderWSPortTypeClient SetupService()
        {
            try
            {
                // Create the binding
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "basicHttpBinding";
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.OpenTimeout = new TimeSpan(0, 5, 0);
                binding.CloseTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                // Max Received Message Size has been increased from 20000000 (default) to 2147483647 (max)
                binding.MaxReceivedMessageSize = 2147483647;
                // Max Buffer Size has been increased from 20000000 (default) to 2147483647 (max)
                binding.MaxBufferSize = 2147483647;

                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxArrayLength = 20000000;
                binding.ReaderQuotas.MaxStringContentLength = 20000000;

                // Set the transport security to UsernameOverTransport for Plaintext usernames
                EndpointAddress endpoint = new EndpointAddress(EndPointURL);

                // Use the DefaultWebProxy on the binding and set a new web proxy object to the WebRequest context with the address and credentials for the proxy. 
                if (proxy != null)
                {
                    binding.UseDefaultWebProxy = true;
                    System.Net.WebRequest.DefaultWebProxy = proxy;
                }
                
                // use default binding and address from app.config
                service = new AppBuilderWSPortTypeClient(binding, endpoint);

                return service;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// returns license and feature detail xml
        /// </summary>
        /// <returns></returns>
        public licenseResponse GetLicenseAndFeatureDetail()
        {
            licenseResponse res = new licenseResponse();

            ActionParams featureActionParams = new ActionParams();
            featureActionParams.SessionId = sessionHeader.sessionId;
            featureActionParams.SessionUrl = this.InstanceURL;
           
            { service.getLicenseAndFeatureDetail(sessionHeader, null, null, null, featureActionParams, out res); }
           

            return res;
        }

        /// <summary>
        /// Return if current user has Admin Pacakge permission  
        /// true - User has access to Admin Package 
        /// </summary>
        /// <returns></returns>
        public bool IsAdminPackageEditionUser()
        {
            bool? res = null;
            service.isAdmin(sessionHeader, null, null, null, out res);

            return !res.HasValue ? false : (bool)res;
        }

        /// <summary>
        /// Return if Org is a sandbox or not  
        /// true -> Sandbox
        /// </summary>
        /// <returns></returns>
        public bool IsSandBox()
        {
            bool? res = null;
            service.runningInASandbox(sessionHeader, null, null, null, out res);

            return !res.HasValue ? false : (bool)res;
        }


    }
}
