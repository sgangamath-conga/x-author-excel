using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;

namespace Apttus.XAuthor.AICAdapter
{
    public class AICServiceController
    {
        private static AICServiceController instance;
        private static object syncRoot = new Object();
        public AuthenticationResult authResult;
        public string tenantURL = string.Empty;
        public static string APP_URL_KEY = "App-Url";
        public AICRefreshSession session;
        public static System.Net.IWebProxy proxy = null;
        public static AICServiceController GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        instance = new AICServiceController();
                    }
                }
                return instance;
            }
        }

        public AICServiceController()
        {
            // TODO:: connection info is initialized with hard-coded token, replace it once integrated with Authentication
            //this.connectionInfo = new AICConnectionInfo
            //{
            //    AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlZXVkljMVdEMVRrc2JiMzAxc2FzTTVrT3E1USIsImtpZCI6IlZXVkljMVdEMVRrc2JiMzAxc2FzTTVrT3E1USJ9.eyJhdWQiOiIyYTBiN2VmYi1hMTM0LTQ1YWYtOWQyMS01MTc4OTM5MTAwZGYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83OTU1MWQ1Yy00M2MwLTQ3MDctOTNjZS02NDkzYWNiMDJjMGEvIiwiaWF0IjoxNTAxNDg0MjY1LCJuYmYiOjE1MDE0ODQyNjUsImV4cCI6MTUwMTQ4ODE2NSwiYWNyIjoiMSIsImFpbyI6IkFTUUEyLzhEQUFBQXNadG9xTEdYQ2s5M3E1UzV6NjBuMjJXTU9vTzFraFlYVnd1WjcyTElGd0U9IiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6IjJhMGI3ZWZiLWExMzQtNDVhZi05ZDIxLTUxNzg5MzkxMDBkZiIsImFwcGlkYWNyIjoiMSIsImlwYWRkciI6IjEyMi4xNS4yNDkuNjciLCJuYW1lIjoiVGVzdDEgVXNlciIsIm9pZCI6IjI2NjAwNmJmLWYyZWUtNDQ4ZS1hZGY1LWM4NWRmOTdiN2Y2OSIsInBsYXRmIjoiMyIsInNjcCI6IkRpcmVjdG9yeS5BY2Nlc3NBc1VzZXIuQWxsIFVzZXIuUmVhZCIsInN1YiI6InNBaFRhcGpDTXk5bWdKWThhT3ZJczNvRFl1bTZWUmxYYmtaUUFpTnFKQ1EiLCJ0aWQiOiI3OTU1MWQ1Yy00M2MwLTQ3MDctOTNjZS02NDkzYWNiMDJjMGEiLCJ1bmlxdWVfbmFtZSI6InRlc3QxdXNlckBjbG1kZXYtZW5nLmFwdHR1c2Nsb3VkLmlvIiwidXBuIjoidGVzdDF1c2VyQGNsbWRldi1lbmcuYXB0dHVzY2xvdWQuaW8iLCJ2ZXIiOiIxLjAifQ.tgz6rSXfjL3i15W5DudjRTFA2qF18vOUWuAsHT6AOaXMWSlAGQBU16KHrxtNWrpz9CKJ7J6Xa7Sd9LGcBf4LG5TbhfapiTAzgVHjHYdqrWALZ1HVUK47d4aZq4rbSORjao6NPrmF22oAuL2E9DGOWceQR7Z0mvLm3ysOzMdd7lUb43Y0yVCDGiqlvGVZqkJuGzS1AYZDWkG9dlN9m6D37c7frWk_4ukVN_B7w_cLYWMHXhL5Z6tQs1RqtakwZLcdWOn7fMNwMF5V8ZTP2bypzW8V31v1XMy6cj4mh7_zUFFTFV8lFqPUL9i3kh3gx61D3LuTsF3vAaRWv4rmD638-Q",
            //    AppURL = "https://clmdev.apttuscloud.io/ui/clm/clm/home",
            //    AccessTokenType = "Bearer"
            //};
        }

        public bool ConnectWithAIC(AuthenticationResult authenticationResult, string tenantURL, IWebProxy proxyObject)
        {
            //if (authResult == null) Commented this block to avoid swtich connection bug AB-3076
            authResult = authenticationResult;

            proxy = proxyObject;

            if (!string.IsNullOrEmpty(tenantURL))
                this.tenantURL = tenantURL;

            session = AICRefreshSession.GetInstance;

            return true;
        }
        public virtual bool ConnectWithAICV2(AICV2TokenResponse tokenResponse,string TenantURL, IWebProxy proxyObject)
        { return true; }
            /// <summary>
            /// Returns all Objects
            /// </summary>
            /// <returns></returns>
            public string GetAllStandardObjects()
        {

            string metdataAdminUrl = "{0}/api/metadata/v1/admin/objects";

            string url = string.Format(metdataAdminUrl, tenantURL);

            var response = GetAsString(url);

            return response.ToString();
        }

        public string GetUserInfo()
        {
            string getUserInfoUrl = "{0}/api/commonservices/accountservice/user/info";

            string url = string.Format(getUserInfoUrl, tenantURL);

            var response = GetAsString(url);

            return response.ToString();
        }

        /// <summary>
        /// Returns Object by name
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public string GetObjectAndFields(string objectName)
        {
            string objectMetadataUrl = "{0}/api/metadata/v1/objects/{1}";

            string url = string.Format(objectMetadataUrl, tenantURL, objectName);

            var response = GetAsString(url);

            return response.ToString();
        }

        public List<SaveResult> Insert(string objectType, List<Dictionary<string, object>> objectDetails, bool enablePartialSave)
        {
            List<SaveResult> saveResults = new List<SaveResult>();
            string objectCreateUrl = "{0}/api/generic/v1/objects/{1}";

            string url = string.Format(objectCreateUrl, tenantURL, objectType);

            // Serialize our concrete class into a JSON String
            string payload = Task.Run(() => JsonConvert.SerializeObject(objectDetails)).Result;

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = PostAsString(url, httpContent);

            // Create Save Results
            if (objectDetails.Count == 1)
            {
                SaveResult currentSave = new SaveResult();
                string insertedId = JsonConvert.DeserializeObject<string>(response);
                currentSave.id = insertedId;
                currentSave.success = true;
                currentSave.error = string.Empty;

                saveResults.Add(currentSave);
            }
            else if (objectDetails.Count > 1)
            {
                List<string> insertResults = JsonConvert.DeserializeObject<List<string>>(response);
                foreach (var res in insertResults)
                {
                    SaveResult currentSave = new SaveResult();
                    currentSave.id = res;
                    currentSave.success = true;
                    currentSave.error = string.Empty;
                    saveResults.Add(currentSave);
                }
            }

            return saveResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectDetails"></param>
        /// <returns></returns>
        public List<SaveResult> Update(string objectType, List<Dictionary<string, object>> objectDetails, bool enablePartialSave)
        {
            List<SaveResult> saveResults = new List<SaveResult>();
            string objectCreateUrl = "{0}/api/generic/v1/objects/{1}";

            string url = string.Format(objectCreateUrl, tenantURL, objectType);

            // Serialize our concrete class into a JSON String
            string payload = Task.Run(() => JsonConvert.SerializeObject(objectDetails)).Result;

            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = PutAsString(url, httpContent);

            List<Dictionary<string, string>> updateResults = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response);

            int cnt = 0;
            foreach (var result in updateResults)
            {
                SaveResult currentSave = new SaveResult();
                string currentRecordId = string.Empty;
                result.TryGetValue("Id", out currentRecordId);

                if (objectDetails[cnt]["Id"].ToString() == currentRecordId)
                {
                    currentSave.id = currentRecordId;
                    currentSave.success = true;
                    currentSave.error = string.Empty;
                }
                else
                {
                    currentSave.id = objectDetails[cnt]["Id"].ToString();
                    currentSave.success = false;
                    currentSave.error = "Update failed for Record Id: " + objectDetails[cnt]["Id"].ToString();
                }

                saveResults.Add(currentSave);
                cnt++;
            }

            return saveResults;
        }

        public string Upsert(string objectType, List<Dictionary<string, object>> objectDetails, string ExternalId, bool enablePartialSave)
        {
            List<SaveResult> saveResults = new List<SaveResult>();
            //https://clmdev.apttuscloud.io/api/generic/v1/objects/clm_Agreement/upsert?UpsertKey=AgreementNumber
            string objectCreateUrl = "{0}/api/generic/v1/objects/{1}/upsert?UpsertKey={2}";

            string url = string.Format(objectCreateUrl, tenantURL, objectType, ExternalId);

            // Serialize our concrete class into a JSON String
            string payload = Task.Run(() => JsonConvert.SerializeObject(objectDetails)).Result;

            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = PostAsString(url, httpContent);

            return response.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectDetails"></param>
        /// <returns></returns>
        public List<SaveResult> Delete(string objecttype, string[] ids, bool enablePartialSave)
        {
            List<SaveResult> saveResults = new List<SaveResult>();
            //string objectDeleteUrl = "{0}/api/generic/v1/objects/{1}?ids={2}";
            string objectDeleteUrl = "{0}/api/generic/v1/objects/{1}/deletemultiple";

            //string url = string.Format(objectDeleteUrl, tenantURL, objecttype, string.Join(",", ids));
            string url = string.Format(objectDeleteUrl, tenantURL, objecttype, string.Join(",", ids));

            // Serialize our concrete class into a JSON String
            string payload = Task.Run(() => JsonConvert.SerializeObject(ids)).Result;

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            //var response = DeleteAsString(url);
            var response = PostAsString(url, httpContent);

            // If response contains success, delete went fine
            if (response.Contains("Success"))
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    SaveResult currentSave = new SaveResult();
                    currentSave.id = ids[i];
                    currentSave.success = true;
                    currentSave.error = string.Empty;

                    saveResults.Add(currentSave);
                }
            }
            else
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    SaveResult currentSave = new SaveResult();
                    currentSave.id = ids[i];
                    currentSave.success = false;
                    currentSave.error = response.ToString();

                    saveResults.Add(currentSave);
                }
            }

            return saveResults;
        }

        public string Query(string objecttype, string query)
        {
            //string objectQueryUrl = "{0}/api/generic/v1/aql/search/{1}?query={2}";
            string objectQueryUrl = "{0}/api/generic/v1/aql/search/{1}";

            string url = string.Format(objectQueryUrl, tenantURL, objecttype);

            // Serialize our concrete class into a JSON String
            string payload = Task.Run(() => JsonConvert.SerializeObject(query)).Result;

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            //var response = GetAsString(url);
            var response = PostAsString(url, httpContent);

            return response.ToString();
        }

        public List<SaveResult> InsertAttachment(string objecttype, string objectid, List<AICRecord> attachments)
        {
            List<SaveResult> saveResults = new List<SaveResult>();

            string objectQueryUrl = "{0}/api/generic/v1/attachments/{1}/{2}";

            string url = string.Format(objectQueryUrl, tenantURL, objecttype, objectid);

            var httpcontent = new MultipartFormDataContent();

            foreach (AICRecord attach in attachments)
            {
                //var fileContent = JsonConvert.SerializeObject(attach.FileStream);
                //string fileContent = Convert.ToBase64String(attach.FileStream);
                StreamContent streamContent = new StreamContent(new MemoryStream(attach.FileStream));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                httpcontent.Add(streamContent, attach.FileName, attach.FileName);
                httpcontent.Add(new StringContent("{\"Description\": \"" + attach.FileName + "\", \"IsPrivate\": false}", Encoding.UTF8, "application/json"), attach.FileName);
            }

            var response = PostAsString(url, httpcontent);

            Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            foreach (var res in result)
            {
                SaveResult saveResult = new SaveResult();
                saveResult.id = res.Value;
                saveResult.error = string.Empty;
                saveResult.success = true;

                saveResults.Add(saveResult);
            }

            return saveResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual string GetAsString(string url)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (this.authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.GetAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (proxy != null)
                handler.Proxy = proxy;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual string PostAsString(string url, HttpContent content)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (this.authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
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
        public virtual string PutAsString(string url, HttpContent content)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (this.authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
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
        public virtual string DeleteAsString(string url)
        {
            var client = new HttpClient(GetHttpClientHandler());

            if (this.authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.DeleteAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetAppAttachmentIds(string recordId, out string templateId, out string configId)
        {
            templateId = configId = string.Empty;
            string url = string.Format("{0}/api/generic/v1/attachments/xae_App/{1}", tenantURL, recordId);
            string response = GetAsString(url);
            Newtonsoft.Json.Linq.JArray attachments = JArray.Parse(response);
            string templateName = string.Empty;

            foreach (JObject attachment in attachments)
            {
                string name = attachment.Value<string>("Name");
                if (name.Contains("Template.xls"))
                {
                    templateId = attachment.Value<string>("Id");
                    templateName = name;
                }
                else if (name.Equals("AppDefinition.xml"))
                    configId = attachment.Value<string>("Id");
            }
            return templateName;
        }

        public byte[] GetAttachment(string recordId)
        {
            session.RefreshConnection();

            string url = string.Format("{0}/api/generic/v1/attachments/{1}", tenantURL, recordId);
            byte[] fileContent = GetAsByteArray(url);

            return fileContent;
            //return JsonConvert.DeserializeObject<byte[]>(fileContent);
        }

        public void DeleteAttachment(string objectid, string recordId)
        {
            session.RefreshConnection();

            string url = string.Format("{0}/api/generic/v1/attachments/{1}/{2}", tenantURL, objectid, recordId);
            DeleteAsString(url);
        }

        public virtual byte[] GetAsByteArray(string url)
        {
            session.RefreshConnection();

            var client = new HttpClient(GetHttpClientHandler());

            if (this.authResult != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.authResult.AccessTokenType, this.authResult.AccessToken);
            }

            client.DefaultRequestHeaders.Add(APP_URL_KEY, tenantURL);

            var response = client.GetAsync(url).Result;
            VerifyResponse(response);
            return response.Content.ReadAsByteArrayAsync().Result;
        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            string entityName = string.Empty;
            if (objectNames.Count > 1)
            {
                foreach (string objectName in objectNames)
                {
                    try
                    {
                        string objectQueryUrl = "{0}/api/generic/v1/objects/{1}/{2}";

                        string url = string.Format(objectQueryUrl, tenantURL, objectName, primaryID.ToString());

                        var response = GetAsString(url);
                        if (!string.IsNullOrEmpty(response))
                        {
                            entityName = objectName;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return entityName;
        }

        protected void VerifyResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = string.Concat("StatusCode : (", (int)response.StatusCode, ")", response.StatusCode, ", ErrorMessage : ", response.Content.ReadAsStringAsync().Result);
                throw new HttpException((int)response.StatusCode, responseBody);
            }
        }
    }
}
