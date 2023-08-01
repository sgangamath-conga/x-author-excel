using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    abstract class DataMigrationActionBase
    {
        protected ConfigurationManager configManager = ConfigurationManager.GetInstance;
        protected ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        protected ActionManager actionManager = ActionManager.GetInstance();

        protected void CreateDisplayAction(MigrationObject migrationObject)
        {
            var retrieveMapId = configManager.RetrieveMaps.Where(x => x.Name == string.Format(DataMigrationConstants.DisplayMapNameFormat, migrationObject.SheetName)).FirstOrDefault().Id;
            RetrieveActionModel model = new RetrieveActionModel();
            RetrieveActionController retrieveAction = new RetrieveActionController(model, null, FormOpenMode.Create);
            retrieveAction.Save(string.Format(DataMigrationConstants.DisplayActionNameFormat, migrationObject.SheetName), retrieveMapId, null);
            migrationObject.DisplayActionId = model.Id;
        }

        protected void CreateSaveAction(MigrationObject migrationObject)
        {
            var SaveMapId = configManager.SaveMaps.Where(x => x.Name == string.Format(DataMigrationConstants.SaveMapNameFormat, migrationObject.SheetName)).FirstOrDefault().Id;
            SaveActionModel model = new SaveActionModel();
            model.EnablePartialSave = true;
            SaveActionController saveActionController = new SaveActionController(model, null, FormOpenMode.Create);
            saveActionController.Save(string.Format(DataMigrationConstants.SaveActionNameFormat, migrationObject.SheetName), SaveMapId, false, true, false, Constants.SAVE_ACTION_BATCH_SIZE);
            migrationObject.SaveActionId = model.Id;
        }

        protected void BuildSearchAndSelectAction(MigrationObject migrationObject)
        {
            string actionName = string.Format(DataMigrationConstants.SearchAndSelectQueryNameFormat, migrationObject.SheetName);
            string recordType = "Multiple";

            SearchAndSelect searchAndSelect = new SearchAndSelect();

            searchAndSelect.TargetObject = migrationObject.AppObjectUniqueId;
            searchAndSelect.GetSearchFieldsList(migrationObject.AppObjectUniqueId);
            searchAndSelect.GetResultFieldsList(migrationObject.AppObjectUniqueId);

            ApttusObject obj = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            foreach (MigrationField field in migrationObject.Fields.Where(field => field.FieldId.Equals(obj.NameAttribute)))
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObject.AppObjectUniqueId, field.FieldId);
                SearchField searchableField = searchAndSelect.AllSearchFields.Where(sf => sf.Id.Equals(apttusField.Id)).FirstOrDefault();
                if (searchableField != null)
                    searchableField.IsSelected = true;
            }

            foreach (MigrationField field in migrationObject.Fields.Where(field => field.FieldId.Equals(obj.NameAttribute)))
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObject.AppObjectUniqueId, field.FieldId);
                ResultField resultField = searchAndSelect.AllResultFields.Where(rf => rf.Id.Equals(apttusField.Id)).FirstOrDefault();
                if (resultField != null)
                    resultField.IsSelected = true;
            }

            SearchFilterGroup filters = migrationObject.DataRetrievalAction.SearchFilter.FirstOrDefault();

            searchAndSelect.SearchFilterGroups = new List<SearchFilterGroup>();

            if (filters != null)
                searchAndSelect.SearchFilterGroups.Add(filters);

            searchAndSelect.Name = actionName;
            searchAndSelect.RecordType = recordType;
            searchAndSelect.PageSize = "10";

            searchAndSelect.SaveAction();

            migrationObject.DataRetrievalActionSelectiveId = searchAndSelect.Id;
        }

        protected void BuildQueryAction(MigrationObject migrationObject)
        {
            string actionName = string.Format(DataMigrationConstants.QueryNameFormat, migrationObject.SheetName);
            List<SearchFilterGroup> list = migrationObject.DataRetrievalAction.SearchFilter;
            ApttusObject appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            QueryActionModel queryActionModel = new QueryActionModel();
            QueryActionController queryAction = new QueryActionController(queryActionModel, null, FormOpenMode.Create);
            queryAction.Save(actionName, appObject, list, string.Empty);
            migrationObject.DataRetrievalActionSelectiveId = queryActionModel.Id;
        }

        protected void BuildAllQueryAction(MigrationObject migrationObject)
        {
            string actionName = string.Format(DataMigrationConstants.QueryNameFormat_All, migrationObject.SheetName);
            ApttusObject appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            QueryActionModel queryActionModel = new QueryActionModel();
            QueryActionController queryAction = new QueryActionController(queryActionModel, null, FormOpenMode.Create);
            queryAction.Save(actionName, appObject, new List<SearchFilterGroup>(), string.Empty);
            migrationObject.DataRetrievalActionAllId = queryActionModel.Id;
        }
    }

    internal class DataMigrationActionController : DataMigrationActionBase
    {
        private DataMigrationModel Model;

        public DataMigrationActionController(DataMigrationModel Model)
        {
            this.Model = Model;
        }

        void BuildAppInfoMacroAction()
        {
            MacroActionController MAC = new MacroActionController(new MacroActionModel(), null, FormOpenMode.Create);
            MAC.Save(DataMigrationConstants.AppInfoActionFlowName, "Module1.AppInfo", false, false);
        }

        internal void BuildActions(Guid masterSaveMapId, List<Guid> ClonedObjectExternalIdSaveMaps)
        {
            BuildAppInfoMacroAction();

            foreach (MigrationObject migrationObject in Model.MigrationObjects)
            {
                BuildActions(migrationObject);
            }

            CreateSaveActionForExternalIDSaveMaps(masterSaveMapId, ClonedObjectExternalIdSaveMaps);
        }

        internal void BuildActions(MigrationObject migrationObject)
        {
            switch (migrationObject.DataRetrievalAction.Action)
            {
                case ActionType.SearchAndSelect:
                    BuildSearchAndSelectAction(migrationObject);
                    break;
                case ActionType.Query:
                    BuildQueryAction(migrationObject);
                    break;
            }

            BuildAllQueryAction(migrationObject);

            if (migrationObject.CreateWorksheet)
            {
                CreateSaveAction(migrationObject);

                CreateDisplayAction(migrationObject);
            }
        }

        internal void EditActions(List<MigrationObject> Objects, EditModeFlow flow, List<Guid> ClonedObjectExternalIdSaveMaps)
        {
            if (Objects == null) return;

            switch (flow)
            {
                case EditModeFlow.Add:
                    {
                        foreach (MigrationObject migrationObject in Objects)
                        {
                            BuildActions(migrationObject);
                        }
                        break;
                    }

                case EditModeFlow.Remove:
                    {
                        foreach (MigrationObject migrationObject in Objects)
                        {
                            RemoveActions(migrationObject);
                        }
                        break;
                    }
            }

            CreateSaveActionForClonnedExternalIDSaveMaps(ClonedObjectExternalIdSaveMaps);

        }

        private void RemoveActions(MigrationObject obj)
        {
            RemoveQueryAction(obj);

            if (obj.CreateWorksheet)
            {
                RemoveSaveAction(obj);
                RemoveDisplayAction(obj);
            }
        }

        protected void RemoveDisplayAction(MigrationObject migrationObject)
        {
            RemoveAction(migrationObject.DisplayActionId);
        }

        protected void RemoveSaveAction(MigrationObject migrationObject)
        {
            RemoveAction(migrationObject.SaveActionId);

            if (migrationObject.IsCloned)
            {
                //Remove External ID Actions 
                SaveMap sMap = configManager.SaveMaps.Where(x => x.Id == migrationObject.ExternalIdSaveMap).FirstOrDefault();
                SaveActionModel externalIdSaveAction = configManager.Actions.OfType<Core.SaveActionModel>().Where(x => x.SaveMapId == sMap.Id).FirstOrDefault();
                RemoveAction(externalIdSaveAction.Id);
            }

        }

        protected void RemoveQueryAction(MigrationObject migrationObject)
        {
            RemoveAction(migrationObject.DataRetrievalActionAllId);
            RemoveAction(migrationObject.DataRetrievalActionSelectiveId);
        }

        void RemoveAction(string actionId)
        {
            actionManager.DeleteAction(actionId);
        }

        internal void UpdateActionFilters()
        {
            foreach (MigrationObject migrationObject in Model.MigrationObjects)
            {
                UpdateActionFilter(migrationObject);
            }
        }

        void UpdateActionFilter(MigrationObject migrationObject)
        {
            switch (migrationObject.DataRetrievalAction.Action)
            {
                case ActionType.SearchAndSelect:
                    UpdateSSQueryActionWithFilters(migrationObject);
                    break;
                case ActionType.Query:
                    UpdateQueryActionWithFilters(migrationObject);
                    break;
            }
        }

        protected void UpdateSSQueryActionWithFilters(MigrationObject migrationObject)
        {
            Apttus.XAuthor.Core.SearchAndSelect ssqueryaction = (SearchAndSelect)configManager.Actions.Where(action => (action.Type.Equals(Constants.SEARCH_AND_SELECT_ACTION)
                                                                   && action.Name == string.Format(DataMigrationConstants.SearchAndSelectQueryNameFormat, migrationObject.SheetName)))
                                                                   .FirstOrDefault();
            ssqueryaction.SearchFilterGroups = migrationObject.DataRetrievalAction.SearchFilter;
        }

        protected void UpdateQueryActionWithFilters(MigrationObject migrationObject)
        {
            string actionName = string.Format(DataMigrationConstants.QueryNameFormat, migrationObject.SheetName);
            List<SearchFilterGroup> list = migrationObject.DataRetrievalAction.SearchFilter;
            ApttusObject appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            Apttus.XAuthor.Core.QueryActionModel queryaction = (QueryActionModel)configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                                                             && action.Name == string.Format(DataMigrationConstants.QueryNameFormat, migrationObject.SheetName))).FirstOrDefault();

            QueryActionController queryAction = new QueryActionController(queryaction, null, FormOpenMode.Edit);
            queryAction.Save(actionName, appObject, list, string.Empty);
        }

        private void CreateSaveActionForExternalIDSaveMaps(Guid masterSaveMapId, List<Guid> ClonedObjectExternalIdSaveMaps)
        {
            CreateSaveActionForMasterExternalIDSaveMaps(masterSaveMapId);
            CreateSaveActionForClonnedExternalIDSaveMaps(ClonedObjectExternalIdSaveMaps);
        }
        private void CreateSaveActionForMasterExternalIDSaveMaps(Guid masterSaveMapId)
        {
            //Create Save Action for Master ExternalID Save Map.
            {
                SaveActionModel model = new SaveActionModel();
                SaveActionController saveActionController = new SaveActionController(model, null, FormOpenMode.Create);
                saveActionController.Save(DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName, masterSaveMapId, false, false, false, Constants.SAVE_ACTION_BATCH_SIZE);
            }
        }

        private void CreateSaveActionForClonnedExternalIDSaveMaps(List<Guid> ClonedObjectExternalIdSaveMaps)
        {
            int clonnedObjectCount;

            if (configManager.Application.MigrationModel != null)
                clonnedObjectCount = configManager.Application.MigrationModel.MigrationObjects.Where(x => x.IsCloned == true && x.CreateWorksheet).Count() + 1;
            else
                clonnedObjectCount = 1;

            int nIndex = clonnedObjectCount;

            foreach (Guid clonedObjectExternalIdSaveMap in ClonedObjectExternalIdSaveMaps)
            {
                SaveActionModel model = new SaveActionModel();
                SaveActionController saveActionController = new SaveActionController(model, null, FormOpenMode.Create);
                string saveMapName = DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName + " " + nIndex;
                saveActionController.Save(saveMapName, clonedObjectExternalIdSaveMap, false, false, false, Constants.SAVE_ACTION_BATCH_SIZE);
                ++nIndex;
            }
        }
    }
}
