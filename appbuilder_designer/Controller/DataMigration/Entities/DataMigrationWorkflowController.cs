using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationWorkflowController
    {
        private DataMigrationModel Model;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        protected List<WorkflowStructure> ActionFlows; //there could be multiple workflows, hence a list of WorkflowStructure
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private DataMigrationMigrateSheetController NameManager = DataMigrationMigrateSheetController.GetInstance;

        internal DataMigrationWorkflowController(DataMigrationModel model)
        {
            ActionFlows = new List<WorkflowStructure>();
            Model = model;
        }

        internal void BuildWorkFlow(bool recreateIfExists = false)
        {
            if (!recreateIfExists)
                BuildAppInfoWorkflow();

            BuildRetrieveWorkflow(recreateIfExists);
            BuildRetrieveALLWorkflow(recreateIfExists);
            BuildSaveWorkflow(recreateIfExists);
        }

        //public void UpdateWorkFlow(List<MigrationObject> Objects, EditModeFlow flow)
        //{
        //    if (Objects == null) return;

        //    switch (flow)
        //    {
        //        case EditModeFlow.Add:
        //            {
        //                foreach (MigrationObject migrationObject in Objects)
        //                {
        //                    AddRetrieveALLWorkflow(migrationObject);
        //                    AddRetrieveWorkflow(migrationObject);
        //                    AddSaveWorkflow(migrationObject);
        //                }
        //                break;
        //            }

        //        case EditModeFlow.Remove:
        //            {
        //                foreach (MigrationObject migrationObject in Objects)
        //                {
        //                    RemoveRetrieveALLWorkflow(migrationObject);
        //                    RemoveRetrieveWorkflow(migrationObject);
        //                    RemoveSaveWorkflow(migrationObject);
        //                }
        //                break;
        //            }
        //    }
        //}

        #region AppInfoWorkFlow
        protected void BuildAppInfoWorkflow()
        {
            if (configManager.Actions.Where(action => action.Type.Equals(Constants.MACRO_ACTION) && action.Name == DataMigrationConstants.AppInfoActionFlowName).Count() == 0)
                return;

            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep = Guid.Empty;

            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow.Name = DataMigrationConstants.AppInfoActionFlowName;

            //First Step
            Step step = new Step();
            step.Name = "Step 1";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = configManager.Actions.Where(action => action.Type.Equals(Constants.MACRO_ACTION) && action.Name == DataMigrationConstants.AppInfoActionFlowName).Select(action => action.Id).FirstOrDefault().ToString();
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            ActionFlows.Add(actionFlow);

            actionFlow.SaveWorkflow();

            Model.MigrationActionFlows.AppInfoActionFlowId = actionFlow.Id;
        }
        #endregion

        #region RetrieveWorkFlow
        private void BuildRetrieveWorkflow(bool recreateIfExists = false)
        {
            WorkflowStructure actionFlow = new WorkflowStructure();

            if (recreateIfExists)
            {
                Guid tmpWfId = Guid.Empty;
                actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.SelectiveActionFlowId) as WorkflowStructure;
                tmpWfId = actionFlow.Id;
                configManager.Workflows.Remove(actionFlow);
                actionFlow = new WorkflowStructure();
                actionFlow.Id = tmpWfId;
            }

            int stepCounter = 0;
            Guid guidStep = Guid.Empty;
            actionFlow.Name = DataMigrationConstants.RetrieveActionFlowName;

            foreach (MigrationObject migrationObject in Model.MigrationObjects.OrderBy(x => x.Sequence))
            {
                AddRetrieveWorkflow(migrationObject, actionFlow, ++stepCounter);
            }

            actionFlow.SaveWorkflow();
            ActionFlows.Add(actionFlow);

            Model.MigrationActionFlows.SelectiveActionFlowId = actionFlow.Id;
        }

        //private void AddRetrieveWorkflow(MigrationObject migrationObject)
        //{
        //    WorkflowStructure actionFlow = new WorkflowStructure();
        //    actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.SelectiveActionFlowId) as WorkflowStructure;
        //    AddRetrieveWorkflow(migrationObject, actionFlow, migrationObject.Sequence);
        //}

        private void AddRetrieveWorkflow(MigrationObject migrationObject, WorkflowStructure actionFlow, int SequenceNo)
        {
            //Check if any steps exist with same sequence then move all them with 2 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= SequenceNo).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo + 2));

            Guid guidStep = Guid.Empty;

            Apttus.XAuthor.Core.Action queryaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                   && action.Name == string.Format(DataMigrationConstants.QueryNameFormat, migrationObject.SheetName))
                   || (action.Type.Equals(Constants.SEARCH_AND_SELECT_ACTION)
                   && action.Name == string.Format(DataMigrationConstants.SearchAndSelectQueryNameFormat, migrationObject.SheetName))).FirstOrDefault();

            //First Step 1. Execute Query
            Step step = new Step();
            step.Name = queryaction.Name;
            step.SequenceNo = SequenceNo;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = queryaction.Name;
            condition.SequenceNo = SequenceNo;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = queryaction.Id;

            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = SequenceNo;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = queryaction.Name;


            // Get Input from Lookup Object's Output
            ApttusObject appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            List<string> lookupInputIds = new List<string>();
            List<string> lookupObjectIds = new List<string>();

            lookupObjectIds = migrationObject.Lookup.Select(x => x.WorkflowInputMigrationObjectId.ToString()).ToList();

            foreach (string objectId in lookupObjectIds)
            {
                try
                {
                    MigrationObject lookupmigrationObject = Model.MigrationObjects.Where(x => x.Id.ToString() == objectId).FirstOrDefault();

                    if (lookupmigrationObject != migrationObject)
                    {
                        Apttus.XAuthor.Core.Action lookupqueryaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                                    && action.Name == string.Format(DataMigrationConstants.QueryNameFormat, lookupmigrationObject.SheetName)) ||
                                    (action.Type.Equals(Constants.SEARCH_AND_SELECT_ACTION) && action.Name == string.Format(DataMigrationConstants.SearchAndSelectQueryNameFormat, lookupmigrationObject.SheetName))).FirstOrDefault();

                        Guid lookupinputid = actionFlow.Steps.Where(x => x.Name == lookupqueryaction.Name).FirstOrDefault().Conditions.Where(x => x.Name == lookupqueryaction.Name).FirstOrDefault().WorkflowActions.FirstOrDefault().WorkflowActionData.Id;
                        lookupInputIds.Add(lookupinputid.ToString());
                    }

                }
                catch (Exception)
                {

                }

            }

            if (lookupInputIds.Count > 0)
            {
                wfAction.WorkflowActionData.InputData = true;
                wfAction.WorkflowActionData.InputDataName = lookupInputIds.ToArray();
            }

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            // if CreateWorksheet is false then no need to create display action - so reutrn else create display action
            if (!migrationObject.CreateWorksheet)
                return;

            //Second Step 2. Display Action

            Apttus.XAuthor.Core.Action displayaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.RETRIEVE_ACTION)
                && action.Name == string.Format(DataMigrationConstants.DisplayActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            step = new Step();
            step.Name = displayaction.Name;
            step.SequenceNo = ++SequenceNo;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = displayaction.Name;
            condition.SequenceNo = SequenceNo;

            wfAction = new WorkflowAction();
            wfAction.ActionId = displayaction.Id;
            WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = SequenceNo;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = Guid.NewGuid();
            wfAction.WorkflowActionData.InputData = true;
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep.ToString() };
            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

        }

        private void RemoveRetrieveWorkflow(MigrationObject migrationObject)
        {
            WorkflowStructure actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.SelectiveActionFlowId) as WorkflowStructure;

            Apttus.XAuthor.Core.Action queryaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                 && action.Name == string.Format(DataMigrationConstants.QueryNameFormat, migrationObject.SheetName))
                 || (action.Type.Equals(Constants.SEARCH_AND_SELECT_ACTION)
                 && action.Name == string.Format(DataMigrationConstants.SearchAndSelectQueryNameFormat, migrationObject.SheetName))).FirstOrDefault();


            actionFlow.Steps.RemoveAll(x => x.Name == queryaction.Name);

            Apttus.XAuthor.Core.Action displayaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.RETRIEVE_ACTION)
                && action.Name == string.Format(DataMigrationConstants.DisplayActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            actionFlow.Steps.RemoveAll(x => x.Name == displayaction.Name);

            //Check if any steps exist with same sequence then move all them with 2 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= migrationObject.Sequence).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo - 2));
        }
        #endregion

        #region RetrieveALLWorkflow
        private void BuildRetrieveALLWorkflow(bool recreateIfExists = false)
        {
            WorkflowStructure actionFlow = new WorkflowStructure();

            if (recreateIfExists)
            {
                Guid tmpWfId = Guid.Empty;
                actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.AllActionFlowId) as WorkflowStructure;
                tmpWfId = actionFlow.Id;
                configManager.Workflows.Remove(actionFlow);
                actionFlow = new WorkflowStructure();
                actionFlow.Id = tmpWfId;
            }

            int stepCounter = 0;
            actionFlow.Name = DataMigrationConstants.RetrieveActionFlowName_ALL;

            foreach (MigrationObject migrationObject in Model.MigrationObjects.OrderBy(x => x.Sequence))
            {
                AddRetrieveALLWorkflow(migrationObject, actionFlow, ++stepCounter);
            }

            actionFlow.SaveWorkflow();
            ActionFlows.Add(actionFlow);

            Model.MigrationActionFlows.AllActionFlowId = actionFlow.Id;
        }

        //private void AddRetrieveALLWorkflow(MigrationObject migrationObject)
        //{
        //    WorkflowStructure actionFlow = new WorkflowStructure();
        //    actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.AllActionFlowId) as WorkflowStructure;
        //    AddRetrieveALLWorkflow(migrationObject, actionFlow, migrationObject.Sequence);
        //}

        private void RemoveRetrieveALLWorkflow(MigrationObject migrationObject)
        {
            WorkflowStructure actionFlow = configManager.GetWorkflowById(Model.MigrationActionFlows.AllActionFlowId) as WorkflowStructure;

            Apttus.XAuthor.Core.Action queryaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                   && action.Name == string.Format(DataMigrationConstants.QueryNameFormat_All, migrationObject.SheetName))).FirstOrDefault();

            actionFlow.Steps.RemoveAll(x => x.Name == queryaction.Name);

            Apttus.XAuthor.Core.Action displayaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.RETRIEVE_ACTION)
               && action.Name == string.Format(DataMigrationConstants.DisplayActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            actionFlow.Steps.RemoveAll(x => x.Name == displayaction.Name);

            //Check if any steps exist with same sequence then move all them with 2 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= migrationObject.Sequence).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo - 2));
        }

        private void AddRetrieveALLWorkflow(MigrationObject migrationObject, WorkflowStructure actionFlow, int SequenceNo)
        {
            //Check if any steps exist with same sequence then move all them with 2 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= SequenceNo).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo + 2));

            Guid guidStep = Guid.Empty;

            Apttus.XAuthor.Core.Action queryaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.EXECUTE_QUERY_ACTION)
                    && action.Name == string.Format(DataMigrationConstants.QueryNameFormat_All, migrationObject.SheetName))).FirstOrDefault();

            //First Step 1. Execute Query
            Step step = new Step();
            step.Name = queryaction.Name;
            step.SequenceNo = SequenceNo;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = queryaction.Name;
            condition.SequenceNo = SequenceNo;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = queryaction.Id;

            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = SequenceNo;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = queryaction.Name;

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            // if CreateWorksheet is false then no need to create display action - so reutrn else create display action
            if (!migrationObject.CreateWorksheet)
                return;

            //Second Step 2. Display Action

            Apttus.XAuthor.Core.Action displayaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.RETRIEVE_ACTION)
                && action.Name == string.Format(DataMigrationConstants.DisplayActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            step = new Step();
            step.Name = displayaction.Name;
            step.SequenceNo = ++SequenceNo;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = displayaction.Name;
            condition.SequenceNo = SequenceNo;

            wfAction = new WorkflowAction();
            wfAction.ActionId = displayaction.Id;
            WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = SequenceNo;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = Guid.NewGuid();
            wfAction.WorkflowActionData.InputData = true;
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep.ToString() };
            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);
        }
        #endregion

        #region SaveWorkFlow
        protected void BuildSaveWorkflow(bool recreateIfExists = false)
        {
            if (configManager.Actions.Where(action => action.Type.Equals(Constants.SAVE_ACTION)).Count() == 0)
                return;

            WorkflowStructure actionFlow = new WorkflowStructure();

            if (recreateIfExists)
            {
                Guid tmpWfId = Guid.Empty;
                actionFlow = (WorkflowStructure)configManager.GetWorkflowByName(DataMigrationConstants.SaveActionFlowName);
                tmpWfId = actionFlow.Id;
                configManager.Workflows.Remove(actionFlow);
                actionFlow = new WorkflowStructure();
                actionFlow.Id = tmpWfId;
            }

            int stepCounter = 0;

            actionFlow.Name = DataMigrationConstants.SaveActionFlowName;

            foreach (MigrationObject migrationObject in Model.MigrationObjects.OrderBy(x => x.Sequence))
            {
                AddSaveWorkflow(migrationObject, actionFlow, ++stepCounter);
            }

            actionFlow.SaveWorkflow();
            ActionFlows.Add(actionFlow);

            Model.MigrationActionFlows.SaveActionFlowId = actionFlow.Id;
        }

        protected void AddSaveWorkflow(MigrationObject migrationObject)
        {
            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow = (WorkflowStructure)configManager.GetWorkflowByName(DataMigrationConstants.SaveActionFlowName);
            AddSaveWorkflow(migrationObject, actionFlow, migrationObject.Sequence);
        }

        private void RemoveSaveWorkflow(MigrationObject migrationObject)
        {
            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow = (WorkflowStructure)configManager.GetWorkflowByName(DataMigrationConstants.SaveActionFlowName);

            Apttus.XAuthor.Core.Action saveaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.SAVE_ACTION)
               && action.Name == string.Format(DataMigrationConstants.SaveActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            actionFlow.Steps.RemoveAll(x => x.Name == saveaction.Name);

            //Check if any steps exist with same sequence then move all them with 1 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= migrationObject.Sequence).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo - 1));
        }

        protected void AddSaveWorkflow(MigrationObject migrationObject, WorkflowStructure actionFlow, int SequenceNo)
        {

            // if CreateWorksheet is false then no need to create display action - so reutrn 
            if (!migrationObject.CreateWorksheet)
                return;

            //Check if any steps exist with same sequence then move all them with 2 steps ahead.
            actionFlow.Steps.Where(x => x.SequenceNo >= SequenceNo).ToList().ForEach(c => c.SequenceNo = (c.SequenceNo + 1));

            Guid guidStep = Guid.Empty;

            Apttus.XAuthor.Core.Action saveaction = configManager.Actions.Where(action => (action.Type.Equals(Constants.SAVE_ACTION)
                && action.Name == string.Format(DataMigrationConstants.SaveActionNameFormat, migrationObject.SheetName))).FirstOrDefault();

            //First Step
            Step step = new Step();
            step.Name = saveaction.Name;
            step.SequenceNo = SequenceNo;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = saveaction.Name;
            condition.SequenceNo = SequenceNo;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = saveaction.Id;
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = SequenceNo;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);
        }
        #endregion

        #region SaveSourceDataWorkFlow
        public void BuildSaveSourceDataWorkFlow(bool recreateIfExists = false)
        {
            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep = Guid.Empty;

            List<Guid> ClonedObjectExternalIdSaveMaps = (from sm in configManager.SaveMaps
                                                         from mo in Model.MigrationObjects
                                                         where (mo.ExternalIdSaveMap == sm.Id && mo.IsCloned == true && mo.CreateWorksheet == true)
                                                         select sm.Id).ToList();

            WorkflowStructure actionFlow = new WorkflowStructure();

            if (recreateIfExists)
            {
                Guid tmpWfId = Guid.Empty;
                actionFlow = (WorkflowStructure)configManager.GetWorkflowByName(DataMigrationConstants.SaveSourceDataActionFlowName);
                tmpWfId = actionFlow.Id;
                configManager.Workflows.Remove(actionFlow);
                actionFlow = new WorkflowStructure();
                actionFlow.Id = tmpWfId;
            }


            actionFlow.Name = DataMigrationConstants.SaveSourceDataActionFlowName;

            Step step = new Step();

            step = GetWorkFlowStep(++stepCounter, ++conditionCounter, DataMigrationConstants.SaveSourceData_UpdateExternalID_ActionName,
                DataMigrationConstants.SaveSourceData_UpdateExternalID_ActionName,
                DataMigrationConstants.SaveSourceData_UpdateExternalID_ActionName, Constants.MACRO_ACTION, ++actionCounter);

            actionFlow.Steps.Add(step);

            // Second Step - Save External IDs 
            step = GetWorkFlowStep(++stepCounter, ++conditionCounter, DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName,
                DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName,
                DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName, Constants.SAVE_ACTION, ++actionCounter);

            actionFlow.Steps.Add(step);

            //Third step - Add cloned object external id save actions.
            if (ClonedObjectExternalIdSaveMaps != null)
            {
                int nIndex = 1;
                foreach (Guid saveActionId in ClonedObjectExternalIdSaveMaps)
                {
                    string saveMapName = DataMigrationConstants.SaveSourceData_SaveExternalID_ActionName + " " + nIndex;
                    step = GetWorkFlowStep(++stepCounter, ++conditionCounter, saveMapName,
                    saveMapName,
                    saveMapName, Constants.SAVE_ACTION, ++actionCounter);
                    ++nIndex;

                    actionFlow.Steps.Add(step);
                }
            }

            //Fourth Step - Copy and Delete Lookup Columns
            step = GetWorkFlowStep(++stepCounter, ++conditionCounter, DataMigrationConstants.SaveSourceData_CopyAndDeleteLookUPColumns_ActionName,
               DataMigrationConstants.SaveSourceData_CopyAndDeleteLookUPColumns_ActionName,
               DataMigrationConstants.SaveSourceData_CopyAndDeleteLookUPColumns_ActionName, Constants.MACRO_ACTION, ++actionCounter);

            actionFlow.Steps.Add(step);

            //Fifth Step - Save as XLSX
            step = GetWorkFlowStep(++stepCounter, ++conditionCounter, DataMigrationConstants.SaveSourceData_SaveAsXLSX_ActionName,
              DataMigrationConstants.SaveSourceData_SaveAsXLSX_ActionName,
              DataMigrationConstants.SaveSourceData_SaveAsXLSX_ActionName, Constants.MACRO_ACTION, ++actionCounter);

            actionFlow.Steps.Add(step);


            ActionFlows.Add(actionFlow);
            actionFlow.SaveWorkflow();

            Model.MigrationActionFlows.SaveSourceDataActionFlowId = actionFlow.Id;
        }

        internal void CreateSourceData()
        {
            System.Collections.Concurrent.ConcurrentBag<DataTransferRange> mappingRanges = new System.Collections.Concurrent.ConcurrentBag<DataTransferRange>();

            foreach (string name in NameManager.AllNames)
            {
                if (Model.MigrationObjects.Exists(x => x.SheetName == name && x.CreateWorksheet == true))
                    mappingRanges.Add(new DataTransferRange() { SourceRange = "$A$3", SourceSheet = name, TargetRange = "$A$3", TargetSheet = name });
            }
            configManager.Application.Definition.Mapping = (new DataTransferMapping() { SourceFile = "", DataTransferRanges = mappingRanges.ToList() });
        }

        internal void BuildSaveSourceData(List<Guid> ClonedObjectExternalIdSaveMaps)
        {
            CreateSourceData();

            // 1. Create Macro Action for Update Exnternal IDs
            MacroActionController MAC = new MacroActionController(new MacroActionModel(), null, FormOpenMode.Create);
            MAC.Save(DataMigrationConstants.SaveSourceData_UpdateExternalID_ActionName, "Module1.UpdateExternalIds", false, false);

            // 2. Create Macro Action for CopyAndDeleteLookupColumns
            MAC = new MacroActionController(new MacroActionModel(), null, FormOpenMode.Create);
            MAC.Save(DataMigrationConstants.SaveSourceData_CopyAndDeleteLookUPColumns_ActionName, "Module1.CopyAndDeleteLookupColumns", false, false);

            // 3. Create Macro Action for SaveAsXLSX
            MAC = new MacroActionController(new MacroActionModel(), null, FormOpenMode.Create);
            MAC.Save(DataMigrationConstants.SaveSourceData_SaveAsXLSX_ActionName, "Module1.SaveAsXLSX", false, false);

            BuildSaveSourceDataWorkFlow();
        }
        #endregion

        public List<MigrationLookup> GetLookUpDetails(ApttusObject appObj)
        {
            List<MigrationLookup> lstLookup = new List<MigrationLookup>();
            List<ApttusField> lstFilterlookup = appObj.Fields.Where(x => x.Datatype == Datatype.Lookup && Model.MigrationObjects.Exists(x1 => x1.ObjectId == x.LookupObject.Id)).ToList();

            if (lstFilterlookup != null && lstFilterlookup.Count > 0)
                foreach (var lookup in lstFilterlookup)
                    lstLookup.Add(new MigrationLookup() { LookupObjectId = lookup.Id, LookupName = lookup.Name, LookupId = lookup.LookupObject.Id });

            return lstLookup;
        }

        Step GetWorkFlowStep(int stepCounter, int conditionCounter, string StepName, string ConditionName, string WorkFlowActionName, string WorkFlowActionType, int actionCounter)
        {
            //First Step - Update External IDs
            Step step = new Step();
            step.Name = StepName;
            step.SequenceNo = stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = ConditionName;
            condition.SequenceNo = conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = configManager.Actions.Where(action => action.Type.Equals(WorkFlowActionType) && action.Name == WorkFlowActionName).Select(action => action.Id).FirstOrDefault().ToString();
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            return step;
        }
    }
}
