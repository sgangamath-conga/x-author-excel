using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    internal class CustomAction
    {
        public string actionName { get; set; }
        public string actionType { get; set; }
    }
    /// <summary>
    /// Just to compare 2 models and giv 
    /// </summary>
    internal class DataMigrationModelChangeTracker
    {
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;

        private ConcurrentDictionary<EditModeOperation, List<MigrationObject>> editedObjects = new ConcurrentDictionary<EditModeOperation, List<MigrationObject>>();
        private ConcurrentDictionary<EditModeOperation, List<MigrationField>> editedFields = new ConcurrentDictionary<EditModeOperation, List<MigrationField>>();
        private ConcurrentDictionary<WorkFlows, List<CustomAction>> customActions = new ConcurrentDictionary<WorkFlows, List<CustomAction>>();

        private DataMigrationModel originalModel;
        private DataMigrationModel editModeModel;
        // Objects
        public DataMigrationModelChangeTracker(DataMigrationModel model)
        {
            editModeModel = model;
            originalModel = configManager.Application.MigrationModel;

            List<MigrationObject> addedObjects = null;
            List<MigrationObject> removedObjects = null;
           // List<MigrationObject> sequencealteredObjects = null;
            Parallel.Invoke(() =>
            {
                //Object Addition Check
                addedObjects = (from obj in model.MigrationObjects
                                where !originalModel.MigrationObjects.Exists(originalMigrationObj => originalMigrationObj.Id == obj.Id)
                                select obj).ToList();
                if (addedObjects.Count > 0)
                    editedObjects[EditModeOperation.ObjectAdded] = addedObjects;
            },

            () =>
            {
                //Object Removal Check
                removedObjects = (from obj in originalModel.MigrationObjects
                                  where !model.MigrationObjects.Exists(o => o.Id == obj.Id)
                                  select obj).ToList();

                if (removedObjects.Count > 0)
                    editedObjects[EditModeOperation.ObjectRemoved] = removedObjects;
            });

            Parallel.Invoke(() =>
            {
                //field addition check
                List<MigrationField> addedFields = (from obj in editModeModel.MigrationObjects
                                                    from fld in obj.Fields
                                                    where IsFieldAdded(obj, fld)
                                                    select fld).ToList();
                if (addedFields.Count > 0)
                    editedFields[EditModeOperation.FieldAdded] = addedFields;
            },

            () =>
            {
                //field removal check
                List<MigrationField> removedFields = (from obj in originalModel.MigrationObjects
                                                      from fld in obj.Fields
                                                      where IsFieldRemoved(obj, fld)
                                                      select fld).ToList();

                if (removedFields.Count > 0)
                    editedFields[EditModeOperation.FieldRemoved] = removedFields;
            });

            if (addedObjects != null && addedObjects.Count > 0)
            {
                List<MigrationField> specialLookupFields = (from obj in originalModel.MigrationObjects
                                                            from fld in obj.Fields.Where(f => f.DataType == Datatype.Lookup)
                                                            from objAdded in addedObjects
                                                            where objAdded.IsCloned == false && GetLookupObjectId(obj.AppObjectUniqueId, fld.FieldId) == objAdded.ObjectId
                                                            select fld).ToList();
                if (specialLookupFields.Count > 0)
                    editedFields[EditModeOperation.NormalLookupFieldToSpecialLookupField] = specialLookupFields;
            }

            if (removedObjects != null && removedObjects.Count > 0)
            {
                List<MigrationField> normalLookupFields = (from obj in originalModel.MigrationObjects
                                                           from fld in obj.Fields.Where(f => f.DataType == Datatype.Lookup)
                                                           from objRemoved in removedObjects
                                                           where GetLookupObjectId(obj.AppObjectUniqueId, fld.FieldId) == objRemoved.ObjectId
                                                           select fld).ToList();
                if (normalLookupFields.Count > 0)
                    editedFields[EditModeOperation.SpecialLookupFieldToNormalLookupField] = normalLookupFields;
            }

            IEnumerable<string> queryallactionIds = originalModel.MigrationObjects.Select(x => x.DataRetrievalActionAllId);
            IEnumerable<string> querySelectiveactionIds = originalModel.MigrationObjects.Select(x => x.DataRetrievalActionSelectiveId);
            IEnumerable<string> displayactionIds = originalModel.MigrationObjects.Select(x => x.DisplayActionId);
            IEnumerable<string> saveactionIds = originalModel.MigrationObjects.Select(x => x.SaveActionId);

            List<WorkflowAction> otheractions = null;
            List<WorkflowAction> workflowActions = null;

            //Check Export Workflow
            workflowActions = GetWorkFlowActions(editModeModel.MigrationActionFlows.AllActionFlowId);
            otheractions = workflowActions.Where(x => !queryallactionIds.Contains(x.ActionId) && !displayactionIds.Contains(x.ActionId)).ToList();
            customActions[WorkFlows.ExportALL] = otheractions.Select(x => new CustomAction() { actionName = x.Name, actionType = x.Type }).ToList();

            //Check Export Selective Workflow 
            workflowActions = GetWorkFlowActions(editModeModel.MigrationActionFlows.SelectiveActionFlowId);
            otheractions = workflowActions.Where(x => !querySelectiveactionIds.Contains(x.ActionId) && !displayactionIds.Contains(x.ActionId)).ToList();
            customActions[WorkFlows.ExportSelective] = otheractions.Select(x => new CustomAction() { actionName = x.Name, actionType = x.Type }).ToList();

            //Check Save Workflow 
            workflowActions = GetWorkFlowActions(editModeModel.MigrationActionFlows.SaveActionFlowId);
            otheractions = workflowActions.Where(x => !saveactionIds.Contains(x.ActionId)).ToList();
            customActions[WorkFlows.Save] = otheractions.Select(x => new CustomAction() { actionName = x.Name, actionType = x.Type }).ToList();

        }

        public string GetMessageFromActionFlow()
        {
            string message = DataMigrationConstants.RetrieveActionFlowName_ALL + " :";
            customActions[WorkFlows.ExportALL].ForEach(x => message += Environment.NewLine + "     " + x.actionName);
            message += Environment.NewLine + "-----------------------------------------------";
            message += Environment.NewLine + DataMigrationConstants.RetrieveActionFlowName + " :";
            customActions[WorkFlows.ExportSelective].ForEach(x => message += Environment.NewLine + "     " + x.actionName);
            message += Environment.NewLine + "-----------------------------------------------";
            message += Environment.NewLine + DataMigrationConstants.SaveActionFlowName + " :";
            customActions[WorkFlows.Save].ForEach(x => message += Environment.NewLine + "     " + x.actionName);
            return message;
        }

        public bool DoesCustomActionsExists()
        {
            return (customActions[WorkFlows.ExportALL].Count > 0 || customActions[WorkFlows.ExportSelective].Count > 0 || customActions[WorkFlows.Save].Count > 0);
        }

        List<WorkflowAction> GetWorkFlowActions(Guid workFlowId)
        {
            WorkflowStructure actionFlow;
            List<WorkflowAction> workflowActions = null;

            actionFlow = configManager.GetWorkflowById(workFlowId) as WorkflowStructure;

            workflowActions = (
                   from workflowSteps in actionFlow.Steps
                   from workflowConditions in workflowSteps.Conditions
                   from workflowAction in workflowConditions.WorkflowActions
                   select workflowAction).ToList();

            return workflowActions;
        }

        private string GetLookupObjectId(Guid UniqueId, string fieldId)
        {
            string lookupObjectId = string.Empty;
            ApttusField lookupField = appDefManager.GetField(UniqueId, fieldId);
            if (lookupField != null)
            {
                ApttusObject lookupObj = lookupField.LookupObject;
                if (lookupObjectId != null)
                    lookupObjectId = lookupObj.Id;
            }
            return lookupObjectId;
        }

        private bool IsFieldRemoved(MigrationObject obj, MigrationField fld)
        {
            MigrationObject objFound = editModeModel.MigrationObjects.FirstOrDefault(o => o.Id == obj.Id);
            if (objFound == null)
                return false;
            return !objFound.Fields.Exists(f => f.FieldId == fld.FieldId && f.DataType == fld.DataType);
        }

        private bool IsFieldAdded(MigrationObject obj, MigrationField fld)
        {
            MigrationObject objFound = originalModel.MigrationObjects.FirstOrDefault(o => o.Id == obj.Id);
            if (objFound == null)
                return false;
            return !objFound.Fields.Exists(f => f.FieldId == fld.FieldId && f.DataType == fld.DataType);
        }

        internal List<MigrationObject> GetAddedObjects()
        {
            if (editedObjects.ContainsKey(EditModeOperation.ObjectAdded))
                return editedObjects[EditModeOperation.ObjectAdded];
            return null;
        }        

        internal List<MigrationObject> GetRemovedObjects()
        {
            if (editedObjects.ContainsKey(EditModeOperation.ObjectRemoved))
                return editedObjects[EditModeOperation.ObjectRemoved];
            return null;
        }

        internal List<MigrationField> GetAddedFields()
        {
            if (editedFields.ContainsKey(EditModeOperation.FieldAdded))
                return editedFields[EditModeOperation.FieldAdded];
            return null;
        }

        internal List<MigrationField> GetRemovedFields()
        {
            if (editedFields.ContainsKey(EditModeOperation.FieldRemoved))
                return editedFields[EditModeOperation.FieldRemoved];
            return null;
        }

        internal List<MigrationField> GetSpecialLookupFields()
        {
            if (editedFields.ContainsKey(EditModeOperation.NormalLookupFieldToSpecialLookupField))
                return editedFields[EditModeOperation.NormalLookupFieldToSpecialLookupField];
            return null;
        }

        internal List<MigrationField> GetNormalLookupFields()
        {
            if (editedFields.ContainsKey(EditModeOperation.SpecialLookupFieldToNormalLookupField))
                return editedFields[EditModeOperation.SpecialLookupFieldToNormalLookupField];
            return null;
        }
    }
}
