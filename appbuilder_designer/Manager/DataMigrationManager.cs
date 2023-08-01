using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    enum LookupFieldType
    {
        FormulaField,
        RetrieveField
    }

    enum IDType
    {
        ID,
        XID
    }

    internal sealed class DataMigrationManager
    {
        private DataMigrationModel Model;
        private DataMigrationMigrateSheetController NameManager = DataMigrationMigrateSheetController.GetInstance;
        private DataMigrationExcelController excelController;
        private DataMigrationMapController mapController;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ApplicationHelper applicationHelper = ApplicationHelper.GetInstance;
        private DataMigrationActionController actionController;
        private DataMigrationWorkflowController workflowController;
        private DataMigrationMenuController menuController;
        private ExternalIDSaveMapController externalIDSaveMapController;

        public DataMigrationManager(DataMigrationModel model)
        {
            Model = model;
            mapController = new DataMigrationMapController(Model);
            excelController = new DataMigrationExcelController(Model);
            actionController = new DataMigrationActionController(Model);
            workflowController = new DataMigrationWorkflowController(Model);
            menuController = new DataMigrationMenuController(Model);
            externalIDSaveMapController = new ExternalIDSaveMapController(Model);
        }

        private void CreateAppFromMigrationTemplate()
        {
            applicationHelper.CreateDataMigrationApplication(Model.AppName, excelController.MigrationTemplatePath, false);
            excelController.CreateUniqueObjectNames();
        }

        internal void BuildSalesforceObjects()
        {
            applicationDefinitionManager.Save();
        }

        internal void CreateMigrationApp()
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                ImportMigrationTemplate();

                CreateAppFromMigrationTemplate();

                BuildSalesforceObjects();

                BuildDisplayMaps();

                HideMigrateSheet();

                BuildSaveMaps();

                BuildActions();

                BuildWorkFlow();

                BuildSaveSourceData();

                BuildMenu();

                BuildAppSettings();

                SelectFirstWorkSheet();

                SaveApplication();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                DeleteMigrationTemplate();
            }
        }
        private void SelectFirstWorkSheet()
        {
            excelController.SelectFirstWorkSheet();
        }


        private void HideMigrateSheet()
        {
            excelController.HideMigrateSheet();
        }

        private void DeleteMigrationTemplate()
        {
            excelController.DeleteMigrationTemplate();
        }

        private void ImportMigrationTemplate()
        {
            excelController.ImportMigrationTemplate();
        }

        private void SaveApplication()
        {
            configManager.Application.MigrationModel = Model;
            applicationHelper.SaveApplication(true, Constants.XLSM);
        }

        private void BuildDisplayMaps()
        {
            foreach (MigrationObject obj in Model.MigrationObjects.Where(obj => obj.CreateWorksheet == true).OrderBy(obj => obj.Sequence))
                mapController.CreateDisplayMap(obj);
        }

        void BuildSaveMaps()
        {
            foreach (MigrationObject migrationObject in Model.MigrationObjects.Where(obj => obj.CreateWorksheet == true))
                mapController.CreateSaveMap(migrationObject);
            externalIDSaveMapController.CreateSaveMapsForExternalID();
        }

        void BuildActions()
        {
            actionController.BuildActions(externalIDSaveMapController.masterSaveMapId, externalIDSaveMapController.ClonedObjectExternalIdSaveMaps);
        }

        void BuildSaveSourceData()
        {
            workflowController.BuildSaveSourceData(externalIDSaveMapController.ClonedObjectExternalIdSaveMaps);
        }

        private void BuildWorkFlow()
        {
            workflowController.BuildWorkFlow();
        }

        public void BuildMenu()
        {
            menuController.BuildMenu();
        }

        private void BuildAppSettings()
        {
            //In DataMigration App all the save related messages are suppressed.
            AppSettings settings = new AppSettings();
            settings.DisableRichTextEditing = true;
            settings.SuppressAllRecordsSaveSuccess = settings.SuppressDependent = settings.SuppressNoOfRecords = settings.SuppressSave = true;
            configManager.Application.Definition.AppSettings = settings;
        }

    }
}
