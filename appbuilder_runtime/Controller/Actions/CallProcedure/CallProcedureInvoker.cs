/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Description;
using System.Windows.Forms;
using System.Xml;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using Apttus.XAuthor.AppRuntime.Modules;
namespace Apttus.XAuthor.AppRuntime
{
    public class CallProcedureInvoker
    {
        private Apttus.WebBrowserControl.ExtendedWebBrowser browser;
        Dictionary<string, Type> availableTypes;
        private Assembly webServiceAssembly;
        private List<string> services;
        public ActionResult Result { get; private set; }
        public bool OutputPersistData { get; set; }
        public string OutputDataName { get; set; }
        DataManager dataManager = DataManager.GetInstance;
        private CallProcedureModel model;
        public string[] InputDataName { get; set; }
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        bool isServiceCached = false;
        bool IsReady;
        WaitWindowView waitForm;
        public delegate void AsyncWaitProcessCaller();
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        XAuthorSalesforceLogin oAuthWrapper;
        /// <summary>
        /// Text description of the available services within this web service.
        /// </summary>
        public List<string> AvailableServices
        {
            get { return this.services; }
        }

        /// <summary>
        /// Creates the service invoker using the specified web service.
        /// </summary>
        /// <param name="webServiceUri"></param>
        public CallProcedureInvoker(CallProcedureModel model)
        {
            Result = new ActionResult();
            this.model = model;
            if (model.EnableCache)
                isServiceCached = IsAssemblyCached(model.ClassName, model.EnableCache);
            else
                IsAssemblyCached(model.ClassName, model.EnableCache);
            oAuthWrapper = Globals.ThisAddIn.GetLoginObject() as XAuthorSalesforceLogin;
        }

        /// <summary>
        /// Gets a list of all methods available for the specified service.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public List<string> EnumerateServiceMethods(string serviceName)
        {
            List<string> methods = new List<string>();

            if (!this.availableTypes.ContainsKey(serviceName))
                throw new Exception(resourceManager.GetResource("CALLPROINVOKER_EnumerateServiceMethods_ErrMsg"));
            else
            {
                Type type = this.availableTypes[serviceName];

                // only find methods of this object type (the one we generated)
                // we don't want inherited members (this type inherited from SoapHttpClientProtocol)
                foreach (MethodInfo minfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    methods.Add(minfo.Name);

                return methods;
            }
        }

        /// <summary>
        /// Invokes the specified method of the named service.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="serviceName">The name of the service to use.</param>
        /// <param name="methodName">The name of the method to call.</param>
        /// <param name="args">The arguments to the method.</param>
        /// <returns>The return value from the web service method.</returns>
        public T InvokeMethod<T>(string serviceName, string methodName, string sessionId, params object[] args)
        {
            object returnValue = null;

            try
            {
                // Create an instance of the specified service
                object webService = this.webServiceAssembly.CreateInstance(serviceName);
                Type webServiceType = webService.GetType();

                // Create SessionHeader
                object sessionHeader = this.webServiceAssembly.CreateInstance("SessionHeader");
                Type sessionHeaderType = sessionHeader.GetType();
                sessionHeaderType.GetProperty("sessionId").SetValue(sessionHeader, sessionId, null);

                // Set webservice session
                webServiceType.GetProperty("SessionHeaderValue").SetValue(webService, sessionHeader, null);


                MethodInfo method = webServiceType.GetMethod(methodName);
                ParameterInfo[] param = method.GetParameters();

                // Invoke the method
                returnValue = (T)webServiceType.InvokeMember(methodName, BindingFlags.InvokeMethod, null, webService, args);
            }
            catch (Exception e)
            {
                ExceptionLogHelper.ErrorLog(e);
                // Wait for thread complete and update status
                IsReady = true;
                MessageBox.Show((e.InnerException).Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return (T)returnValue;
        }

        /// <summary>
        /// Builds the web service description importer, which allows us to generate a proxy class based on the 
        /// content of the WSDL described by the XmlTextReader.
        /// </summary>
        /// <param name="xmlreader">The WSDL content, described by XML.</param>
        /// <returns>A ServiceDescriptionImporter that can be used to create a proxy class.</returns>
        public static ServiceDescriptionImporter BuildServiceDescriptionImporter(XmlTextReader xmlreader)
        {
            // make sure xml describes a valid wsdl
            if (!ServiceDescription.CanRead(xmlreader))
                throw new Exception(resourceManager.GetResource("CALLPROINVOKER_BuildServiceDescriptionImporter_ErrMsg"));

            // parse wsdl
            ServiceDescription serviceDescription = ServiceDescription.Read(xmlreader);

            // build an importer, that assumes the SOAP protocol, client binding, and generates properties
            ServiceDescriptionImporter descriptionImporter = new ServiceDescriptionImporter();
            descriptionImporter.ProtocolName = "Soap";
            descriptionImporter.AddServiceDescription(serviceDescription, null, null);
            descriptionImporter.Style = ServiceDescriptionImportStyle.Client;
            descriptionImporter.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            return descriptionImporter;
        }

        /// <summary>
        /// Compiles an assembly from the proxy class provided by the ServiceDescriptionImporter.
        /// </summary>
        /// <param name="descriptionImporter"></param>
        /// <returns>An assembly that can be used to execute the web service methods.</returns>
        private Assembly CompileAssembly(ServiceDescriptionImporter descriptionImporter)
        {
            // a namespace and compile unit are needed by importer
            CodeNamespace codeNamespace = new CodeNamespace();
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            codeUnit.Namespaces.Add(codeNamespace);

            ServiceDescriptionImportWarnings importWarnings = descriptionImporter.Import(codeNamespace, codeUnit);

            if (importWarnings == 0) // no warnings
            {
                // create a c# compiler
                CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");

                // include the assembly references needed to compile
                string[] references = new string[2] { "System.Web.Services.dll", "System.Xml.dll" };

                CompilerParameters parameters = new CompilerParameters(references);

                // TODO:: DreamForce fix To reduce time taken by call procedure
                // Cache Assembly to App temp folder to be used
                string filePath = Utils.GetTempFileName(configurationManager.Definition.UniqueId, model.ClassName + ".dll");
                parameters.OutputAssembly = filePath;

                // compile into assembly
                CompilerResults results = compiler.CompileAssemblyFromDom(parameters, codeUnit);

                foreach (CompilerError oops in results.Errors)
                {
                    // trap these errors and make them available to exception object
                    throw new Exception(resourceManager.GetResource("CALLPRO_CompileAssembly_ErrMsg"));
                }

                // all done....
                return results.CompiledAssembly;
            }
            else
            {
                // warnings issued from importers, something wrong with WSDL
                throw new Exception(resourceManager.GetResource("CALLPROINVOKER_CompileAssemblyWSDL_ErrMsg"));
            }
        }

        public void InvokeWSDL(CallProcedureModel model, string[] inputDataName, bool OutputPersistData, string OutputDataName)
        {
            try
            {
                IsReady = false;
                this.model = model;
                this.InputDataName = inputDataName;
                this.OutputPersistData = OutputPersistData;
                this.OutputDataName = OutputDataName;

                // TODO:: for DreamForce scenario
                // If Assembly is cached invoke method from cached assembly
                WaitWindowView waitForm = new WaitWindowView(resourceManager.GetResource("COREWAITWINDOWVIEW_MessageLabel_Text"), true);
                waitForm.StartPosition = FormStartPosition.CenterScreen;
                waitForm.Show();

                if (isServiceCached)
                {
                    AsyncWaitProcessCaller caller = null;
                    caller = new AsyncWaitProcessCaller(InvokeCachedAssembly);

                    IAsyncResult asyncResult = caller.BeginInvoke(null, null);

                    while (!IsReady)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        // wait here if the objects are still loading...
                        System.Windows.Forms.Application.DoEvents();
                    }

                    ApttusObjectManager.ShowNormalCursor();
                    waitForm.Close();
                    waitForm = null;
                    return;
                }

                //
                string SessionId = oAuthWrapper.AccessToken;
                string InstanceURL = oAuthWrapper.InstanceURL;
                string FrontDoorURLPrefix = ApttusGlobals.FRONTDOORURLPREFIX;
                string frontDoorUrl = InstanceURL + FrontDoorURLPrefix + System.Web.HttpUtility.UrlEncode(SessionId);
                // TODO :: get class name from Callprocedure xml from designer
                string returnUrl = InstanceURL + ApttusGlobals.WSDLPATH + model.ClassName + "?wsdl";

                frontDoorUrl += "&retURL=" + System.Web.HttpUtility.UrlEncode(returnUrl);

                // HTTP request
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(frontDoorUrl);
                wr.Timeout = 600000; // 10 minutes
                wr.Method = "GET";
                wr.KeepAlive = true;
                wr.ServicePoint.Expect100Continue = false;

                WebResponse webResponse = wr.GetResponse();
                Stream requestStream = webResponse.GetResponseStream();

                // Check response
                HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                using (Stream data = webResponse.GetResponseStream())
                {
                    string text = new StreamReader(data).ReadToEnd();

                }

                // Use web browser instead
                browser = new Apttus.WebBrowserControl.ExtendedWebBrowser();
                browser.DocumentCompleted += browser_DocumentCompleted;
                browser.Navigate(frontDoorUrl);
                do
                {
                    Thread.Sleep(10);
                    System.Windows.Forms.Application.DoEvents();

                } while (!IsReady);
                //
                waitForm.Close();
                waitForm = null;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Error");
            }
            finally
            {
                if (waitForm != null)
                {
                    waitForm.Close();
                    waitForm = null;
                }
                // unsubscribe the event
                if (!isServiceCached)
                {
                    browser.DocumentCompleted -= browser_DocumentCompleted;
                    browser.Dispose();
                }
            }
        }
        /// <summary>
        /// Browser Document Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if ((sender as System.Windows.Forms.WebBrowser).ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                List<object> lst = new List<object>();
                ObjectManager manager = ObjectManager.GetInstance;
                string strClassName = string.Empty;
                object objResultData;

                this.services = new List<string>(); // available services
                this.availableTypes = new Dictionary<string, Type>(); // available types

                // And here we go: browser.Document.Body.InnerText contains our XML
                string outputResult = browser.DocumentText.Trim();

                // For Remove junk character in XML like "- " 
                outputResult = outputResult.Replace("- ", string.Empty);
                // For Ignore comments in XML like <!- Defination -->
                outputResult = Regex.Replace(outputResult, "(?s)<!-.*?-->", string.Empty);

                // OutPut result convert in MemoryStream
                System.Text.UTF8Encoding myEncoder = new System.Text.UTF8Encoding();
                byte[] bytes = myEncoder.GetBytes(outputResult);
                MemoryStream ms = new MemoryStream(bytes);

                // MemoryStream output convert in XMLTextReader
                XmlTextReader xmlreader = new XmlTextReader(ms);

                ServiceDescriptionImporter descriptionImporter = BuildServiceDescriptionImporter(xmlreader);

                // create an assembly from the web service description
                this.webServiceAssembly = CompileAssembly(descriptionImporter);

                // see what service types are available
                Type[] types = this.webServiceAssembly.GetExportedTypes();

                // and save them
                ////foreach (Type type in types)
                ////{
                ////    services.Add(type.FullName);
                ////    availableTypes.Add(type.FullName, type);
                ////}
                if (strClassName == string.Empty && !strClassName.Contains("Service"))
                {
                    strClassName = model.ClassName + "Service";
                }


                List<object> methodParams = new List<object>();

                foreach (MethodParam param in this.model.MethodParams)
                {

                    if (param.Type == MethodParam.ParamType.Static || param.Type == MethodParam.ParamType.UserInput)
                    {
                        methodParams.Add(param.ParamValue);
                    }
                    else
                    {
                        ApttusObject inputObject = ApplicationDefinitionManager.GetInstance.GetAppObject(Guid.Parse(param.ParamObject));
                        ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(this.InputDataName, inputObject);

                        if (param.Type == MethodParam.ParamType.Object)
                        {
                            Array objects = DataSetToObject(dataSet);

                            if (objects.Length == 1)
                                methodParams.Add(objects.GetValue(0));
                            else
                                methodParams.Add(objects);
                        }
                        else
                        {
                            methodParams.Add(dataSet.DataTable.Rows[0][param.ParamField]);
                        }
                    }
                }
                objResultData = this.InvokeMethod<object>(strClassName, model.MethodName, oAuthWrapper.AccessToken, methodParams.ToArray());
                //objArrayResult = ((object[])objResultData).Cast<object>().ToArray();
                if (OutputPersistData)
                {
                    if (objResultData != null)
                    {
                        PersistOutput(objResultData);

                        Result.Status = ActionResultStatus.Success;
                        // Wait for thread complete and update status
                        IsReady = true;
                    }
                    else
                    {
                        Result.Status = ActionResultStatus.Failure;
                        // Wait for thread complete and update status
                        IsReady = true;
                    }
                }
                else
                {
                    Result.Status = ActionResultStatus.Success;
                    // Wait for thread complete and update status
                    IsReady = true;
                }
            }
        }
        public static DataTable GetDataTableFromObjects(object obj, ApttusObject appObject)
        {
            if (obj != null)
            {
                // For list or single object
                if (obj.GetType().IsArray)
                {
                    Object[] objArray = ((object[])obj).Cast<Object>().ToArray();
                    //object[] objArray =  Array.ConvertAll((object[])obj, s => (object)s);
                    List<string> fieldNames = ConfigurationManager.GetInstance.GetAllAppFields(appObject, true).Select(f => f.FieldId).ToList();

                    DataTable dt = new DataTable(appObject.Id);
                    foreach (string field in fieldNames)
                        dt.Columns.Add(new DataColumn(field));

                    Type objType = obj.GetType();
                    //DataRow dr = dt.NewRow();
                    foreach (object objElement in objArray)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (!dc.ColumnName.Contains("_r."))
                            {
                                string Name;
                                if (objElement.GetType().GetProperty(dc.ColumnName) != null)
                                    dr[dc.ColumnName] = objElement.GetType().GetProperty(dc.ColumnName).GetValue(objElement, null);
                                else if (dc.ColumnName.EndsWith(Constants.APPENDLOOKUPID))
                                {
                                    Name = dc.ColumnName.Substring(dc.ColumnName.LastIndexOf(".") + 1);
                                    dr[dc.ColumnName] = objElement.GetType().GetProperty(Name).GetValue(objElement, null);
                                }

                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                else
                {
                    List<string> fieldNames = ConfigurationManager.GetInstance.GetAllAppFields(appObject, true).Select(f => f.FieldId).ToList();
                    DataTable dt = new DataTable(appObject.Id);
                    foreach (string field in fieldNames)
                        dt.Columns.Add(new DataColumn(field));

                    Type objType = obj.GetType();
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                        if (!dc.ColumnName.Contains("_r."))
                        {
                            string Name;
                            if (obj.GetType().GetProperty(dc.ColumnName) != null)
                                dr[dc.ColumnName] = obj.GetType().GetProperty(dc.ColumnName).GetValue(obj, null);
                            else if (dc.ColumnName.EndsWith(Constants.APPENDLOOKUPID))
                            {
                                Name = dc.ColumnName.Substring(dc.ColumnName.LastIndexOf(".") + 1);
                                dr[dc.ColumnName] = obj.GetType().GetProperty(Name).GetValue(obj, null);
                            }
                        }
                    dt.Rows.Add(dr);
                    return dt;
                }
            }
            return null;
        }

        public Array DataSetToObject(ApttusDataSet dataSet)
        {
            ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(dataSet.AppObjectUniqueID);

            Array objects = Array.CreateInstance(this.webServiceAssembly.CreateInstance(appObject.Id).GetType(), dataSet.DataTable.Rows.Count);

            int i = 0;
            foreach (DataRow row in dataSet.DataTable.Rows)
            {
                object obj = this.webServiceAssembly.CreateInstance(appObject.Id);
                Type objectType = obj.GetType();

                foreach (DataColumn col in dataSet.DataTable.Columns)
                {
                    // TODO: properly handle lookups
                    if (col.ColumnName.Contains(".") || col.ColumnName.Contains("__r"))
                        continue;


                    object val = row[col];

                    if (val is DBNull)
                        val = null;

                    try
                    {
                        // Workaround in case data type is not set properly for column
                        Type propType = objectType.GetProperty(col.ColumnName).PropertyType;
                        if (propType == typeof(double) || propType == typeof(double?))
                            val = Convert.ToDouble(val);
                        else if (propType == typeof(bool) || propType == typeof(bool?))
                            val = Convert.ToBoolean(val);

                        objectType.GetProperty(col.ColumnName).SetValue(obj, val, null);

                        // Try to set specified attribute
                        PropertyInfo specifiedProp = objectType.GetProperty(col.ColumnName + "Specified");

                        if (specifiedProp != null)
                            specifiedProp.SetValue(obj, true, null);
                    }
                    catch (Exception e)
                    {
                    }
                }

                objects.SetValue(obj, i);

                i++;
            }

            return objects;
        }

        internal bool IsAssemblyCached(string className, bool blnEnableCache)
        {
            string filePath = Utils.GetTempFileName(configurationManager.Definition.UniqueId, model.ClassName + ".dll");
            if (File.Exists(filePath))
            {
                if (blnEnableCache)
                {
                    this.webServiceAssembly = Assembly.LoadFrom(filePath);
                    return true;
                }
                else
                    File.Delete(filePath);
            }
            return false;
        }

        /// <summary>
        /// Invoke Method from Cached Assembly
        /// </summary>
        internal void InvokeCachedAssembly()
        {
            object objResultData;

            List<object> methodParams = new List<object>();

            foreach (MethodParam param in this.model.MethodParams)
            {

                if (param.Type == MethodParam.ParamType.Static || param.Type == MethodParam.ParamType.UserInput)
                {
                    methodParams.Add(param.ParamValue);
                }
                else
                {
                    ApttusObject inputObject = ApplicationDefinitionManager.GetInstance.GetAppObject(Guid.Parse(param.ParamObject));
                    ApttusDataSet dataSet = DataManager.GetInstance.ResolveInput(this.InputDataName, inputObject);

                    if (param.Type == MethodParam.ParamType.Object)
                    {
                        Array objects = DataSetToObject(dataSet);

                        if (objects.Length == 1)
                            methodParams.Add(objects.GetValue(0));
                        else
                            methodParams.Add(objects);
                    }
                    else
                    {
                        methodParams.Add(dataSet.DataTable.Rows[0][param.ParamField]);
                    }
                }
            }


            objResultData = this.InvokeMethod<object>(model.ClassName + "Service", model.MethodName, oAuthWrapper.AccessToken, methodParams.ToArray());
            //objArrayResult = ((object[])objResultData).Cast<object>().ToArray();

            Result.Status = ActionResultStatus.Pending_Execution;

            if (OutputPersistData)
            {
                if (objResultData != null)
                {
                    PersistOutput(objResultData);

                    Result.Status = ActionResultStatus.Success;
                    // Wait for thread complete and update status
                    IsReady = true;
                }
                else
                {
                    Result.Status = ActionResultStatus.Failure;
                    // Wait for thread complete and update status
                    IsReady = true;
                }

            }
            else
            {
                Result.Status = ActionResultStatus.Success;
                // Wait for thread complete and update status
                IsReady = true;
            }
        }

        private ApttusDataSet PersistOutput(object objResultData)
        {
            ApttusDataSet resultData = new ApttusDataSet();

            Result.Status = ActionResultStatus.Pending_Execution;

            ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(this.model.ReturnObject);
            try
            {
                if (objResultData != null)
                    resultData.DataTable = GetDataTableFromObjects(objResultData, appObject);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(resourceManager.GetResource("CALLPRO_PersistOutput_Text") + ex.Message, resourceManager.GetResource("CALLPROC_ActionCap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
                return null;
            }

            resultData.Name = OutputDataName;
            resultData.AppObjectUniqueID = appObject.UniqueId;
            dataManager.AddData(resultData);

            return resultData;
        }

    }

}
