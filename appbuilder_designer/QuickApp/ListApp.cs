/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppDesigner.Modules;
using Microsoft.Win32;

namespace Apttus.XAuthor.AppDesigner
{
    class ListApp : QuickApplicationBase
    {
        public ListApp()
        {

        }

        private void GetStartRowCol(ref int startRow, ref int startCol)
        {
            //string RegistryBase = ApttusGlobals.ApttusRegistryBase;

            startRow = QuickAppConstants.RepeatingGroupStartRow;
            //string KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListAppStartRow);
            //string KeyVal = base.quickAppSettings.ListAppPosition.StartRow;
            //if (!String.IsNullOrEmpty(KeyVal))
            //    startRow = Convert.ToInt32(KeyVal);

            startCol = QuickAppConstants.RepeatingGroupStartCol;
            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListAppStartCol);
            //KeyVal = base.quickAppSettings.ListAppPosition.StartCol;
            //if (!String.IsNullOrEmpty(KeyVal))
            //    startCol = Convert.ToInt32(KeyVal);
        }

        public override void BuildDisplayMap(WizardModel Model)
        {
            var displayFields = Model.DisplayFields.Where(field => field.Display == true).OrderBy(field => Convert.ToInt32(field.DisplayOrder));

            int repeatingGroupStartRow = 0, repeatingGroupStartCol = 0;
            GetStartRowCol(ref repeatingGroupStartRow, ref repeatingGroupStartCol);

            if (displayFields.Count() > 0)
            {
                Excel.Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;
                Excel.Range Target = sheet.Cells[repeatingGroupStartRow, repeatingGroupStartCol];

                foreach (QuickAppFieldAttribute field in displayFields)
                {
                    ApttusField apttusField = Model.Object.Fields.Where(af => field.FieldId.Equals(af.Id)).FirstOrDefault();
                    string targetLocation = ExcelHelper.GetAddress(Target);
                    string namedRange = ExcelHelper.CreateUniqueNameRange();
                    ExcelHelper.AssignNameToRange(Target, namedRange);
                    retrieveMapcontroller.AddField(Model.Object, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                    Excel.Range FieldLabel = ExcelHelper.NextVerticalCell(Target, -1);
                    string fieldName = field.FieldName;
                    if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                        fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();
                    FieldLabel.Value = fieldName;

                    FieldLabel.EntireColumn.AutoFit();
                    if (salesforceIDColumn == null && field.FieldId.Equals(Model.Object.IdAttribute))
                        salesforceIDColumn = FieldLabel;

                    //Next Target Location
                    Target = ExcelHelper.NextHorizontalCell(Target, 1);
                }
            }

            //Set the repeating group header to be displayed at runtime
            // Do add Header for List
            //retrieveMapcontroller.RetrieveMap.RepeatingGroups[0].GridHeader = Model.Object.Name + " List";

            base.BuildDisplayMap(Model);
        }

        public override void BuildSaveMap(WizardModel Model, int saveMapIndex)
        {
            List<QuickAppFieldAttribute> saveFields = Model.DisplayFields.Where(field => field.Save == true && field.ObjectId.Equals(Model.Object.Id)).ToList();
            if (saveFields.Count == 0)
                return;

            SaveMapRetrieveFieldController controller = new SaveMapRetrieveFieldController(saveMapController.Model, null);
            SaveMapRetrieveMap sm = new SaveMapRetrieveMap();

            sm.RetrieveMapId = configManager.RetrieveMaps[0].Id;
            sm.RetrieveMapName = configManager.RetrieveMaps[0].Name;

            controller.RetrieveMapSelectionChange(sm);

            var r1 = (from f in saveFields
                      from r in controller.Model
                      where r.RetrieveFieldId.Equals(f.FieldId) && applicationDefinitionManager.GetAppObject(r.AppObjectUniqueId).Id.Equals(f.ObjectId)
                      select r);

            foreach (SaveMapRetrieveField r in r1)
                r.Included = true;

            controller.AddRetrieveFieldsToSaveMap();
            saveMapController.UpdateModel(saveMapController.Model.SaveFields);

            saveMapController.Save(QuickAppConstants.SaveMapName, true);
            base.BuildSaveMap(Model, 0);
        }

        public override void BuildSearchAndSelectAction(WizardModel Model, ActionSelectionFilter action)
        {
            string actionName = "Search " + Model.Object.Name;
            string recordType = "Multiple";

            SearchAndSelect searchAndSelect = new SearchAndSelect();

            searchAndSelect.TargetObject = Model.Object.UniqueId;
            searchAndSelect.GetSearchFieldsList(Model.Object.UniqueId);
            searchAndSelect.GetResultFieldsList(Model.Object.UniqueId);

            foreach (QuickAppFieldAttribute quickfield in Model.DisplayFields.Where(field => field.SearchFields == true && field.ObjectId.Equals(Model.Object.Id)))
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(Model.Object.UniqueId, quickfield.FieldId);
                SearchField searchableField = searchAndSelect.AllSearchFields.Where(sf => sf.Id.Equals(apttusField.Id)).FirstOrDefault();
                if (searchableField != null)
                    searchableField.IsSelected = true;
            }

            foreach (QuickAppFieldAttribute field in Model.DisplayFields.Where(field => field.ResultFields == true && field.ObjectId.Equals(Model.Object.Id)))
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(Model.Object.UniqueId, field.FieldId);
                ResultField resultField = searchAndSelect.AllResultFields.Where(rf => rf.Id.Equals(apttusField.Id)).FirstOrDefault();
                if (resultField != null)
                    resultField.IsSelected = true;
            }

            var filters = Model.Filters.Where(filter => filter.Object.UniqueId.Equals(Model.Object.UniqueId) && filter.RelationObject == RelationalObject.ParentObject).Select(filter => filter.Filters);
            searchAndSelect.SearchFilterGroups = new List<SearchFilterGroup>();
            if (filters != null && filters.Count() > 0)
            {
                foreach (SearchFilterGroup filter in filters.FirstOrDefault())
                    searchAndSelect.SearchFilterGroups.Add(filter);
            }

            searchAndSelect.Name = actionName;
            searchAndSelect.RecordType = recordType;
            searchAndSelect.PageSize = "10";
            searchAndSelectActions.Add(searchAndSelect);

            base.BuildSearchAndSelectAction(Model, action);
        }

        public override void BuildQueryAction(WizardModel Model, ActionSelectionFilter action)
        {
            string actionName = Model.Object.Name + " Query";
            List<SearchFilterGroup> list = Model.Filters.Where(filter => filter.RelationObject == RelationalObject.ParentObject && filter.Object.Id.Equals(Model.Object.Id)).Select(filter => filter.Filters).FirstOrDefault();
            queryAction.Save(actionName, Model.Object, list, string.Empty);

            base.BuildQueryAction(Model, action);
        }

        private void BuildRetrieveWorkflow(WizardModel Model)
        {
            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep = Guid.Empty;
            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow.AutoExecuteEditInExcelMode = Model.AllowEditInExcel;
            actionFlow.Name = QuickAppConstants.RetrieveWorkflowName;

            //First Step 1. Execute Query
            Step step = new Step();
            step.Name = "Step 1";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();

            Core.ActionType typeOfAction = Model.Actions.Where(action => action.ObjectType == RelationalObject.ParentObject).FirstOrDefault().ActionType;

            if (typeOfAction == Core.ActionType.Query)
                wfAction.ActionId = configManager.Actions.OfType<QueryActionModel>().Where(action => action.TargetObject.Equals(Model.Object.UniqueId)).FirstOrDefault().Id;
            else if (typeOfAction == Core.ActionType.SearchAndSelect)
                wfAction.ActionId = configManager.Actions.OfType<SearchAndSelect>().Where(action => action.TargetObject.Equals(Model.Object.UniqueId)).FirstOrDefault().Id;

            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = "ListAppData1";

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            //Second Step 2. Display Action
            step = new Step();
            step.Name = "Step 2";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            wfAction = new WorkflowAction();
            wfAction.ActionId = configManager.Actions.Where(action => action.Type.Equals(Constants.RETRIEVE_ACTION)).Select(action => action.Id).FirstOrDefault().ToString();
            WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = Guid.NewGuid();
            wfAction.WorkflowActionData.InputData = true;
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep.ToString() };
            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            ActionFlows.Add(actionFlow);
        }

        public override void ConfigureWorkflow(WizardModel Model)
        {
            BuildRetrieveWorkflow(Model);
            BuildSaveWorkflow(Model);
            base.ConfigureWorkflow(Model);
        }
    }
}
