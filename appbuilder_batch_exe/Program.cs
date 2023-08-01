/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Batch;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Apttus.XAuthor.BatchExe
{
    public class Program
    {
        static BatchExeController batchExeController = BatchExeController.GetInstance;

        static Guid ApplicationGuid = Guid.Empty;
        //static XDocument InputDataXml = new XDocument();
        static string InputDataJson = string.Empty;
        static string WorkflowName = string.Empty;
        static string OutputPath = string.Empty;

        static void Main(string[] args)
        {
            /*************************************************************** ********/
            /******** C O N N E C T   U S I N G    A C C E S S    T O K E N ********/
            /******************************************/
            //byte[] template = File.ReadAllBytes(@"C:\Users\Pranav\Documents\Apttus Projects\App Builder\X-Author Batch Output\template.xlsx");
            byte[] template = File.ReadAllBytes(@"C:\PrestoApp\Template.xlsx");
            byte[] config = File.ReadAllBytes(@"C:\PrestoApp\AppDefinition.xml");
            string json = File.ReadAllText(@"C:\PrestoApp\data.txt");

            BatchController controller = BatchController.GetInstance;
            controller.Connect("00DM0000001YHTB!AQQAQNwFxsI6Vh.pOquJ2hq.j.OE9V__1QS3HP2hqM4rl68jm6osgHL4ydEe8ntpF_D54uetF5bh8_Y80bsnDiC..W.kwPdx",
                "https://unmanageclmqa--wauthor--apttus-docgen.visualforce.com", new object());
            BatchRequest request = new BatchRequest();
            request.Config = config;
            request.Template = template;
            request.InputData = json;
            //request.TemplateName = "Template.xlsx";
            request.TemplateName = "Template.xlsx";
            request.OutputPath = @"C:\PrestoApp\output";
            request.OutputType = "XLSX";
            BatchResponse response = controller.Execute(request);



            /*
            if (args.Count() != 3)
            {
                Console.WriteLine(Constants.BATCH_PRODUCT_NAME + Environment.NewLine + Environment .NewLine +
                    "Usage: " + Constants.BATCH_EXE + " -a -i [-f]" + Environment.NewLine +
                    " -a Application Unique Id" + Environment.NewLine +
                    " -i Input Data in JSon format" + Environment.NewLine +
                    //" -f Action Flow Name" + Environment.NewLine + "    if unspecifed first action flow will be executed" + Environment.NewLine +
                    " -o Output Path" + Environment.NewLine
                    );
                Console.ReadLine();
            }
            else
            {
                if (batchExeController.ValidateInput(args, ref ApplicationGuid, ref InputDataJson, ref WorkflowName, ref OutputPath))
                {
                    BatchResponse response = batchExeController.Execute(ApplicationGuid, InputDataJson, OutputPath);
                    if (response != null)
                    {
                        Console.WriteLine(Constants.BATCH_PRODUCT_NAME + Environment.NewLine +
                            "Success: " + response.Success.ToString() + Environment.NewLine +
                            (response.Success
                                ? "Excel Output File Name: " + response.OutputExcelFileName
                                : response.ErrorMessage) + Environment.NewLine
                            );
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Null response recieved from " + Constants.BATCH_PRODUCT_NAME);
                        Console.ReadLine();
                    }
                }
                else
                    Console.ReadLine();

            } */
        }
           
    }
}
