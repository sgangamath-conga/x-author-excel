/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Apttus.XAuthor.SalesforceAdapter.MetadataSForce;
using System.Net;

namespace Apttus.XAuthor.SalesforceAdapter
{
    public class SForceMetadataServiceController
    {
        // Static variables
        private static SForceMetadataServiceController instance;
        private static object syncRoot = new Object();

        private static MetadataPortTypeClient soapClient;

        // Decalre session header to pass in query operation method
        public static SessionHeader sessionHeader = null;

        // ToDo: Get this endpoint URL from the TokenResponse class (fetched based on login).
        public string EndPointURL { get; set; }

        public static SForceMetadataServiceController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        // To Support TLS 1.2 for spring 16 Org instances
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        instance = new SForceMetadataServiceController();
                    }
                }
                return instance;
            }
        }

        #region Setup Service Operation

        public bool ConnectWithSalesforce(string oAuthToken, string instanceUrl)
        {
            if (oAuthToken != null)
            {
                // Set instanceURL from OAuthLogi
                EndPointURL = instanceUrl + "/services/Soap/m/31.0";
                // Initialize the soapClient. Use default binding and address from app.config
                soapClient = SetupService();
                // Set SessionHeader
                sessionHeader = new SessionHeader() { sessionId = oAuthToken };
                return true;
            }
            else
            {
                return false;
            }
        }

        private MetadataPortTypeClient SetupService()
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

                // use default binding and address from app.config
                MetadataPortTypeClient service = new MetadataPortTypeClient(binding, endpoint);

                return service;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Service Operation

        public void listMetadata(string type)
        {
            ListMetadataQuery query = new ListMetadataQuery();
            query.type = type;  //"CustomObject"
            double asOfVersion = 50.0;

            FileProperties[] fileProps = soapClient.listMetadata(sessionHeader, null, new ListMetadataQuery[] { query }, asOfVersion);

            //if (fProps != null)
            //{
            //    foreach (FileProperties fp in fProps)
            //    {
            //        if (fp.fullName.Contains("Account"))
            //        {
            //            string temp = fp.fullName;
            //        }
            //    }
            //}
        }

        public Metadata[] readMetadata(string type, string[] fullNames)
        {
            Metadata[] objectMetadata = soapClient.readMetadata(sessionHeader, null, type, fullNames);
            return objectMetadata;
        }

        #endregion

    }
}
