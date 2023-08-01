/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using System.Xml.Linq;
using Apttus.XAuthor.Batch;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.BatchExe
{
    public class BatchExeController
    {
        private static BatchExeController instance;
        private static object syncRoot = new Object();

        BatchController batchController = BatchController.GetInstance;

        private BatchExeController()
        {
        }

        public static BatchExeController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new BatchExeController();
                    }
                }
                return instance;
            }
        }

        public bool ValidateInput(string[] args, ref Guid ApplicationGuid, ref XDocument InputDataXml, ref string WorkflowName, ref string OutputPath)
        {
            bool a = false, i = false, o = false;

            // 1. -a Application Unique Id - Argument 1
            string stringGuid = args.FirstOrDefault(arg => arg.StartsWith("-a")).Substring(2);
            if (!string.IsNullOrEmpty(stringGuid))
            {
                Guid guid;
                if (Guid.TryParse(stringGuid, out guid))
                {
                    ApplicationGuid = guid;
                    a = true;
                }
                else
                {
                    Console.WriteLine("Invalid format for Application Unique Id");
                }
            }
            else
            {
                Console.WriteLine("Missing Application Unique Id");
            }

            // 2. -i Input Data in XML format - Argument 2
            string xml = args.FirstOrDefault(arg => arg.StartsWith("-i")).Substring(2);
            try
            {
                InputDataXml = XDocument.Parse(xml);
                //InputDataXml.LoadXml(xml);
                i = true;
            }
            catch (Exception)
            {
                Console.WriteLine("There is a load or parse error in processing the Input Data XML. Invalid argument -i");
            }

            // 3. -f Action Flow Name - Argument 3
            string flowName = args.FirstOrDefault(arg => arg.StartsWith("-f")).Substring(2);
            if (!string.IsNullOrEmpty(flowName))
                WorkflowName = flowName;

            // 4. -o Output Path - Argument 4
            string outputPath = args.FirstOrDefault(arg => arg.StartsWith("-o")).Substring(2);
            if (!string.IsNullOrEmpty(outputPath))
            {
                if (Directory.Exists(outputPath))
                {
                    OutputPath = outputPath;
                    o = true;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(outputPath);
                        OutputPath = outputPath;
                        o = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("X-Author tried to create the output directory " + outputPath + ". However the directory creation failed." + Environment.NewLine + Environment.NewLine + "Error Details: " + ex.Message);
                    }
                }
            }

            return (a & i & o);
        }

        public bool ValidateInput(string[] args, ref Guid ApplicationGuid, ref string InputDataJson, ref string WorkflowName, ref string OutputPath)
        {
            bool a = false, i = false, o = false;

            // 1. -a Application Unique Id - Argument 1
            string stringGuid = args.FirstOrDefault(arg => arg.StartsWith("-a")).Substring(2);
            if (!string.IsNullOrEmpty(stringGuid))
            {
                Guid guid;
                if (Guid.TryParse(stringGuid, out guid))
                {
                    ApplicationGuid = guid;
                    a = true;
                }
                else
                {
                    Console.WriteLine("Invalid format for Application Unique Id");
                }
            }
            else
            {
                Console.WriteLine("Missing Application Unique Id");
            }

            // 2. -i Input Data in XML format - Argument 2
            string jsonFileName = args.FirstOrDefault(arg => arg.StartsWith("-i")).Substring(2);
            if (!string.IsNullOrEmpty(jsonFileName))
            {
                InputDataJson = LoadInputJson(jsonFileName);
                i = true;
            }
            else
            {
                Console.WriteLine("Missing Input Data in JSon format. Invalid argument -i");
            }

            //// 3. -f Action Flow Name - Argument 3
            //string flowName = args.FirstOrDefault(arg => arg.StartsWith("-f")).Substring(2);
            //if (!string.IsNullOrEmpty(flowName))
            //    WorkflowName = flowName;

            // 3. -o Output Path - Argument 4
            string outputPath = args.FirstOrDefault(arg => arg.StartsWith("-o")).Substring(2);
            if (!string.IsNullOrEmpty(outputPath))
            {
                if (Directory.Exists(outputPath))
                {
                    OutputPath = outputPath;
                    o = true;
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(outputPath);
                        OutputPath = outputPath;
                        o = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("X-Author tried to create the output directory " + outputPath + ". However the directory creation failed." + Environment.NewLine + Environment.NewLine + "Error Details: " + ex.Message);
                    }
                }
            }

            return (a & i & o);
        }

        public BatchResponse Execute(Guid ApplicationGuid, string InputData, string OutputPath)
        {
            ApplicationObject app = DownloadApplication(ApplicationGuid);
            BatchResponse response = null;

            if (app != null)
            {
                response = batchController.Execute(
                    new BatchRequest
                    {
                        TemplateName = app.TemplateName,
                        Config = app.Config,
                        Template = app.Template,
                        InputData = InputData,
                        OutputPath = OutputPath
                    });
            }
            return response;
        }

        private ApplicationObject DownloadApplication(Guid appUniqueId)
        {
            ApplicationObject app = null;
            ObjectManager objectManager = ObjectManager.GetInstance;
            ObjectManager.SetCRM(CRM.Salesforce);
            //objectManager.Connect("pshah@xauthordev.com", "internal0000", "Ijltmttbl5hnSn7uql3EvvAT", SalesforceEndpoint.Login);
            //objectManager.Connect("rpeermohammed@apttus.com", "internal000", "usDqxmnmP4b668OxqbsyyoTqC", SalesforceEndpoint.Test);
            //if (objectManager.Connect("jules2@apttusdemo.com", "internal00000", "MlfD0bJ4Pr1HC8xDfxFtpIQE", SalesforceEndpoint.Login))
            //if (objectManager.Connect("ksadhwani@arubanetworks.com.na2mimic", "apttuscrm!@#", "", SalesforceEndpoint.Test))
            if (new ABSalesforceAdapter().Connect("ksadhwani@arubanetworks.com.na2mimic", "apttuscrm!@#", "", SalesforceEndpoint.Test))
            {
                SalesforceApplicationController AppController = new SalesforceApplicationController();
                app = AppController.LoadApplication(null, appUniqueId.ToString());
            }
            return app;
        }

        private string LoadInputJson(string jsonFileName)
        {
            string result = string.Empty;
            var JsonFilePath = Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, jsonFileName);
            var sr = new StreamReader(JsonFilePath);
            result = sr.ReadToEnd();

            sr.Close();
            sr.Dispose();
            return result;
        }
    }
}
