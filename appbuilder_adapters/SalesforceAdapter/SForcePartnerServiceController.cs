/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.SalesforceAdapter.SForce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.SalesforceAdapter
{

    public sealed class SForcePartnerServiceController
    {

        // Static variables
        private static SForcePartnerServiceController instance;
        private static object syncRoot = new Object();
        private static SoapClient soapClient;

        // Decalre session header to pass in query operation method
        public static SessionHeader sessionHeader = null;

        public static System.Net.IWebProxy proxy = null;
        // ToDo: Get this endpoint URL from the TokenResponse class (fetched based on login).
        public string EndPointURL { get; set; }
        // Partner API version
        public string PARTNER_API_VERSION = "46.0";

        // Private constructor
        //private SForcePartnerServiceController()
        //{
        //    //soapClient = SetupService();
        //}

        // Static method to get the class instance (Singleton Pattern)
        public static SForcePartnerServiceController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        // To Support TLS 1.2 for spring 16 Org instances
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        instance = new SForcePartnerServiceController();
                    }
                }
                return instance;
            }
        }

        #region Service Operation

        public List<T> Query<T>(string soql) where T : sObject, new()
        {
            List<T> returnList = new List<T>();

            QueryResult results = null;
            LimitInfo[] returnLimitInfo = soapClient.query(sessionHeader, null, null, null, null, soql, out results);
            Boolean done = false;

            if (results.size > 0 && results.records != null)
            {
                while (!done)
                {
                    for (int i = 0; i < results.records.Length; i++)
                    {
                        T item = results.records[i] as T;

                        if (item != null)
                            returnList.Add(item);
                    }
                    if (results.done)
                    {
                        done = true;
                    }
                    else
                    {
                        string queryLocator = results.queryLocator;
                        results.records = null;
                        results = null;
                        returnLimitInfo = soapClient.queryMore(sessionHeader, null, null, queryLocator, out results);
                    }

                }
            }
            return returnList;
        }

        public T QuerySingle<T>(string soql) where T : sObject, new()
        {
            T returnValue = new T();

            QueryResult results = null;
            LimitInfo[] returnLimitInfo = soapClient.query(sessionHeader, null, null, null, null, soql, out results);

            if (results.size == 1)
                returnValue = results.records[0] as T;

            return returnValue;
        }

        public QueryResult Query(string soql)
        {
            QueryResult results = null;
            LimitInfo[] returnLimitInfo = soapClient.query(sessionHeader, null, null, null, null, soql, out results);

            return results;
        }

        public SaveResult[] Insert(sObject[] items, bool enablePartialSave)
        {
            SaveResult[] saveResult = null;
            LimitInfo[] returnLimitInfo = null;
            try
            {
                DebuggingInfo dInfo = soapClient.create(sessionHeader, null, null, null, null, null, null, new AllOrNoneHeader() { allOrNone = !enablePartialSave },null,null, null, null, null, items, out returnLimitInfo, out saveResult);
            }
            catch (Exception ex)
            {
                saveResult = new SaveResult[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    saveResult[i] = new SaveResult
                    {
                        success = false,
                        errors = new Error[] { new Error { statusCode = StatusCode.UNKNOWN_EXCEPTION, message = ex.Message } }
                    };
                }
            }
            return saveResult;
        }

        public SaveResult[] Update(sObject[] items, bool enablePartialSave)
        {
            SaveResult[] saveResult = null;
            LimitInfo[] returnLimitInfo = null;
            try
            {
                DebuggingInfo dInfo = soapClient.update(sessionHeader, null, null, null, null, null, null, new AllOrNoneHeader() { allOrNone = !enablePartialSave },null,null, null, null, null, null, items, out returnLimitInfo, out saveResult);
            }
            catch (Exception ex)
            {
                saveResult = new SaveResult[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    saveResult[i] = new SaveResult
                    {
                        success = false,
                        errors = new Error[] { new Error { statusCode = StatusCode.UNKNOWN_EXCEPTION, message = ex.Message } }
                    };
                }
            }
            return saveResult;
        }

        public UpsertResult[] Upsert(sObject[] items, string externalID, bool enablePartialSave)
        {
            UpsertResult[] upsertResult = null;
            LimitInfo[] returnLimitInfo = null;
            try
            {
                DebuggingInfo dInfo = soapClient.upsert(sessionHeader, null, null, null, null, null, null, new AllOrNoneHeader() { allOrNone = !enablePartialSave },null,null,null, null, null, null, externalID, items, out returnLimitInfo, out upsertResult);
            }
            catch (Exception ex)
            {
                upsertResult = new UpsertResult[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    upsertResult[i] = new UpsertResult
                    {
                        success = false,
                        errors = new Error[] { new Error { statusCode = StatusCode.UNKNOWN_EXCEPTION, message = ex.Message } }
                    };
                }
            }
            return upsertResult;
        }

        public DeleteResult[] Delete(string[] ids, bool enablePartialSave)
        {
            DeleteResult[] deleteResult = null;
            LimitInfo[] returnLimitInfo = null;
            try
            {
                DebuggingInfo dInfo = soapClient.delete(sessionHeader, null, null, null, null, null, null,null,new AllOrNoneHeader() { allOrNone = !enablePartialSave },null ,null, null, ids, out returnLimitInfo, out deleteResult);
            }
            catch (Exception ex)
            {
                deleteResult = new DeleteResult[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    deleteResult[i] = new DeleteResult
                    {
                        success = false,
                        errors = new Error[] { new Error { statusCode = StatusCode.UNKNOWN_EXCEPTION, message = ex.Message } }
                    };
                }
            }
            return deleteResult;
        }

        public DebuggingInfo Undelete(string[] ids)
        {
            UndeleteResult[] undeleteResult = null;
            LimitInfo[] returnLimitInfo = null;
            return soapClient.undelete(sessionHeader, null, null, null, null, new AllOrNoneHeader() { allOrNone = true }, null, null,null,null, ids, out returnLimitInfo, out undeleteResult);
        }

        public SearchResult Search(string soql)
        {
            SearchResult searchResult = null;
            LimitInfo[] returnLimitInfo = soapClient.search(sessionHeader, null, null, soql, out searchResult);
            return searchResult;
        }

        public GetUserInfoResult getUserInfo()
        {
            GetUserInfoResult userInfoResult = null;
            LimitInfo[] returnLimitInfo = soapClient.getUserInfo(sessionHeader, null, out userInfoResult);
            return userInfoResult;
        }

        public DescribeGlobalResult GetAllStandardObjects()
        {
            DescribeGlobalResult describeGlobalResult = null;
            LimitInfo[] returnLimitInfo = soapClient.describeGlobal(sessionHeader, null, null, out describeGlobalResult);
            return describeGlobalResult;
        }

        public DescribeSObjectResult GetObjectAndFields(string objectName)
        {
            DescribeSObjectResult describeSObjectResult = null;
            LimitInfo[] returnLimitInfo = soapClient.describeSObject(sessionHeader, null, null, null, objectName, out describeSObjectResult);
            return describeSObjectResult;
        }

        public List<DescribeSObjectResult> GetObjectsAndFields(List<string> objectNames)
        {
            DescribeSObjectResult[] describeSObjectResults = null;
            LimitInfo[] returnLimitInfo = soapClient.describeSObjects(sessionHeader, null, null, null, objectNames.ToArray(), out describeSObjectResults);
            return describeSObjectResults.ToList();
        }

        public DescribeLayoutResult GetObjectLayout(string objectName, string[] recordTypeIds)
        {
            DescribeLayoutResult describeLayboutResult = null;
            //TODO:: see affect of layoutName param, if it breaks Record Type implementation
            LimitInfo[] returnLimitInfo = soapClient.describeLayout(sessionHeader, null, null, objectName, null, recordTypeIds, out describeLayboutResult);
            return describeLayboutResult;
        }

        public void executeAnonymous()
        {

        }

        #endregion

        #region Setup Service Operation

        public bool ConnectWithSalesoforce(string OAuthToken, string InstanceURL,System.Net.IWebProxy proxyObject)
        {
            if (OAuthToken != null)
            {
                // Set instanceURL from OAuthLogi
                EndPointURL = InstanceURL + "/services/Soap/u/" + PARTNER_API_VERSION;

                proxy = proxyObject;
                // Initialize the soapClient. Use default binding and address from app.config
                soapClient = SetupService();
                // Set SessionHeader
                sessionHeader = new SessionHeader() { sessionId = OAuthToken };
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ConnectWithSalesoforce(string username, string password, string token, string endpointURL)
        {

            EndPointURL = endpointURL;
            LoginResult result = this.Login(username, password, token);
            EndPointURL = result.serverUrl;

            if (result.sessionId != null)
            {
                sessionHeader = new SessionHeader() { sessionId = result.sessionId };
                EndPointURL = result.serverUrl;
                soapClient = SetupService();
                return true;
            }
            else
            {
                return false;
            }
        }

        public LoginResult Login(string username, string password, string securityToken)
        {
            try
            {
                // Initialize the soapClient. Use default binding and address from app.config
                SoapClient service = SetupService();

                service.ClientCredentials.UserName.UserName = username;
                service.ClientCredentials.UserName.Password = password + securityToken;                
                LoginResult loginResult = service.login(null, null,
                    service.ClientCredentials.UserName.UserName, service.ClientCredentials.UserName.Password);

                return loginResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SoapClient SetupService()
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
                SoapClient service = new SoapClient(binding, endpoint);

                return service;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
