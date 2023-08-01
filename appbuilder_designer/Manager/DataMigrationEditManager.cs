using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    internal enum EditModeOperation
    {
        FieldAdded,
        FieldRemoved,
        ObjectAdded,
        ObjectRemoved,
        FilterChanged,
        SpecialLookupFieldToNormalLookupField,
        NormalLookupFieldToSpecialLookupField,
        SequenceChanged
    };
    internal enum EditModeFlow
    {
        Add,
        Remove
    };

    internal enum WorkFlows
    {
        ExportALL,
        ExportSelective,
        Save
    }
    internal class DataMigrationEditManager
    {
        private DataMigrationModelChangeTracker modelChangeTracker;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationHelper applicationHelper = ApplicationHelper.GetInstance;
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

        private DataMigrationMapEditController mapController;
        private DataMigrationActionController actionController;
        private DataMigrationWorkflowController workflowController;
        private DataMigrationExcelController excelController;
        private DataMigrationMenuController menuController;
        private ExternalIDSaveMapController externalIdSaveMapController;

        private object ExcelUILock = new object();
        private DataMigrationModel Model;

        List<MigrationObject> addedObjects;
        List<MigrationObject> removedObjects;
        List<MigrationField> addedFields;
        List<MigrationField> removedFields;
        List<MigrationField> specialLookupFields;
        List<MigrationField> normalLookupFields;

        public DataMigrationEditManager(DataMigrationModel model)
        {
            Model = model;

            mapController = new DataMigrationMapEditController(model, ExcelUILock);
            modelChangeTracker = new DataMigrationModelChangeTracker(model);
            actionController = new DataMigrationActionController(model);
            workflowController = new DataMigrationWorkflowController(model);
            excelController = new DataMigrationExcelController(model);
            menuController = new DataMigrationMenuController(model);
            externalIdSaveMapController = new ExternalIDSaveMapController(model);

            addedObjects = modelChangeTracker.GetAddedObjects();
            removedObjects = modelChangeTracker.GetRemovedObjects();
            addedFields = modelChangeTracker.GetAddedFields();
            removedFields = modelChangeTracker.GetRemovedFields();
            normalLookupFields = modelChangeTracker.GetNormalLookupFields();
            specialLookupFields = modelChangeTracker.GetSpecialLookupFields();
        }

        internal void EditMigrationApp()
        {

            //Common
            UpdateApplication();
            EditSalesforceObjects();
            EditDisplayMaps();
            EditSaveMaps();

            //Delete 
            EditActions(EditModeFlow.Remove);
            //EditWorkFlow(EditModeFlow.Remove);

            //Add
            EditActions(EditModeFlow.Add);

            //Common
            RemoveunwantedSaveMaps();
            UpdateActionsFilters();
            EditWorkFlow();
            EditSaveSourceData();
            EditMenus();

            SelectFirstWorkSheet();

            SaveApplication();

            if (modelChangeTracker.DoesCustomActionsExists())
                DisplayAlert();
        }

        private void RemoveunwantedSaveMaps()
        {
            //Remove unwanted save maps, its actions and from workdlow.
            //1. find save Maps with no fields
            List<SaveMap> sm = externalIdSaveMapController.GetSaveMapsWithoutFields();

            //2. remove those save maps
            configManager.SaveMaps.RemoveAll(i => sm.Contains(i));
        }

        private void SelectFirstWorkSheet()
        {
            excelController.SelectFirstWorkSheet();
        }

        private void HideMigrateSheet()
        {
            excelController.HideMigrateSheet();
        }

        private void ShowMigrateSheet()
        {
            excelController.ShowMigrateSheet();
        }

        private void UpdateApplication()
        {
            excelController.CreateUniqueObjectNames();
            ShowMigrateSheet();
            applicationDefinitionManager.Save();
        }

        private void DisplayAlert()
        {
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No;
            result = ApttusMessageUtil.Show(resourceManager.GetResource("DM_WorkflowChange_Text"), resourceManager.GetResource("DM_WorkflowChangeCaption_Text"), string.Empty, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
                        Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok,
                         modelChangeTracker.GetMessageFromActionFlow(), Globals.ThisAddIn.Application.Hwnd);
        }

        private void SaveApplication()
        {
            HideMigrateSheet();
            configManager.Application.MigrationModel = Model;
            applicationHelper.SaveApplication(true, Constants.XLSM);
        }

        private void EditSaveSourceData()
        {
            workflowController.CreateSourceData();
            workflowController.BuildSaveSourceDataWorkFlow(true);
        }

        private void EditWorkFlow()
        {            
            workflowController.BuildWorkFlow(true);
        }

        private void EditActions(EditModeFlow flow)
        {
            switch (flow)
            {
                case EditModeFlow.Add:
                    {
                        actionController.EditActions(addedObjects, flow, externalIdSaveMapController.ClonedObjectExternalIdSaveMaps);
                        break;
                    }
                case EditModeFlow.Remove:
                    {
                        actionController.EditActions(removedObjects, flow, externalIdSaveMapController.ClonedObjectExternalIdSaveMaps);
                        break;
                    }
            }
        }

        private void EditSaveMaps()
        {
            mapController.UpdateSaveMaps(addedObjects, removedObjects);
            externalIdSaveMapController.UpdateExternalIdSaveMaps(addedObjects, removedObjects, mapController.ModifiedObjects);
        }

        private void EditDisplayMaps()
        {
            mapController.UpdateDisplayMaps(addedObjects, removedObjects);
        }

        private void EditSalesforceObjects()
        {
            mapController.UpdateNormalFields(addedFields, removedFields);
            mapController.UpdateSpecialLookupFields(specialLookupFields, normalLookupFields);
        }

        private void EditMenus()
        {
            menuController.UpdateMenu();
        }

        private void UpdateActionsFilters()
        {
            actionController.UpdateActionFilters();
        }
    }
}
