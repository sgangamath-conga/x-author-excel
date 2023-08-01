/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace Apttus.XAuthor.AppDesigner
{
    public class CloneAppController
    {
        private CloneAppView View;
        IXAuthorApplicationController AppController = Globals.ThisAddIn.GetApplicationController();
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        string CloneDirectory = Path.GetTempPath() + "Clone_Directory";

        public CloneAppController(CloneAppView view)
        {
            this.View = view;
            view.SetController(this);
            this.View.PopulateAppDetails(configurationManager.Application.Definition.Name + "-Clone", "1.0");
            this.View.ShowDialog();
        }

        public void CloneApp(string AppName, string AppVersion)
        {
            // 0. Prepare Clone Directory
            Utils.CreateOrClearDirectory(CloneDirectory);

            // 1. Prepare config file for Clone
            // 1.1 Write Config to Temp Location
            string ConfigClonePath = CloneDirectory + Path.DirectorySeparatorChar + configurationManager.Application.Definition.Name + Constants.XML;
            configurationManager.SaveLocal(ConfigClonePath, false);

            // 1.2 Read Temp Config
            string configXml = File.ReadAllText(ConfigClonePath);
            Application CloneApplication = ApttusXmlSerializerUtil.Deserialize<Application>(configXml);

            // 1.3 Set new value for Config.
            string NewUniqueId = Guid.NewGuid().ToString();
            CloneApplication.Definition.Name = AppName;
            CloneApplication.Definition.Version = AppVersion;
            CloneApplication.Definition.UniqueId = NewUniqueId;
            CloneApplication.Definition.Schema = configurationManager.Application.Definition.Schema;

            // 2. Prepare template file for Clone
            // 2.1 FullPath for Active and Clone Workbooks
            string ActiveWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
            string CloneWorkbookName = CloneDirectory + Path.DirectorySeparatorChar + Path.GetFileName(ActiveWorkbookName);

            // before saveas save active workbook to preserve designer flag
            Globals.ThisAddIn.Application.ActiveWorkbook.Save();

            // 2.2 Create Clone Workbook and Set Unique Id
            ApttusObjectManager.SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, CloneWorkbookName);
            Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
            Excel.Range oRange = oSheet.Cells[1, 1];
            oRange.Value2 = NewUniqueId;
            Globals.ThisAddIn.Application.ActiveWorkbook.Save();
            Globals.ThisAddIn.Application.ActiveWorkbook.Close();

            // 2.3 Set the focus back to Active Workbook
            ApttusObjectManager.OpenFile(ActiveWorkbookName);
            
            ////2.4 build schema
            //AppSchema.SchemaUtil SU = new AppSchema.SchemaUtil();
            //byte[] schema = SU.GetQueryActions();
            //CloneApplication.Schema = schema;

            // 3. Clone App
            AppController.CloneApp(CloneApplication, CloneWorkbookName);
        }


        internal void ClearCloneDirectory()
        {
            // Clear Clone Directory
            if (Directory.Exists(CloneDirectory))
            {
                Utils.CreateOrClearDirectory(CloneDirectory);
                Directory.Delete(CloneDirectory);
            }
        }
    }
}
