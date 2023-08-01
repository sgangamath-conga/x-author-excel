/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{
    class ParentChildApp : QuickApplicationBase
    {
        public ParentChildApp()
        {

        }

        private void GetStartRowCol(ref int startRow, ref int startCol)
        {
            //string RegistryBase = ApttusGlobals.ApttusRegistryBase;

            startRow = QuickAppConstants.IndependentFieldStartRow;
            //string KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ParentChildStartRow);
            //string KeyVal = base.quickAppSettings.ParentChildAppPosition.StartRow;
            //if (!String.IsNullOrEmpty(KeyVal))
            //    startRow = Convert.ToInt32(KeyVal);

            startCol = QuickAppConstants.IndependentFieldStartColumn;
            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ParentChildStartCol);
            //KeyVal = base.quickAppSettings.ParentChildAppPosition.StartCol;
            //if (!String.IsNullOrEmpty(KeyVal))
            //{
            //    startCol = Convert.ToInt32(KeyVal);
            //    if (startCol == 1)
            //        startCol = QuickAppConstants.IndependentFieldStartColumn;
            //}
        }

        public override void BuildDisplayMap(WizardModel Model)
        {
            //Independent fields
            int startRow = 0;
            int row = 0, col = 0;
            GetStartRowCol(ref row, ref col);
            var independentFields = Model.DisplayFields.Where(field => field.Display == true && field.ObjectId.Equals(Model.Object.Id)).OrderBy(field => Convert.ToInt32(field.DisplayOrder));
            if (independentFields.Count() > 0)
            {
                Excel.Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;               
                Excel.Range Target = sheet.Cells[row, col];
                startRow = Target.Row;
                foreach (QuickAppFieldAttribute field in independentFields)
                {
                    ApttusField apttusField = Model.Object.Fields.Where(af => field.FieldName.Equals(af.Name)).FirstOrDefault();
                    string targetLocation = ExcelHelper.GetAddress(Target);
                    string namedRange = ExcelHelper.CreateUniqueNameRange();
                    ExcelHelper.AssignNameToRange(Target, namedRange);
                    retrieveMapcontroller.AddField(Model.Object, apttusField, targetLocation, ObjectType.Independent, namedRange);
                    //Target.Value = "[" + field.FieldName + "]";

                    Excel.Range FieldLabel = ExcelHelper.NextHorizontalCell(Target, -1);
                    string fieldName = field.FieldName;
                    if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                        fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();
                    FieldLabel.Value = fieldName;

                    FieldLabel.EntireColumn.AutoFit();
                    //Next Target Location
                    Target = ExcelHelper.NextVerticalCell(Target, 1);

                    if (Target.Row > startRow)
                        startRow = Target.Row;

                    //if (Target.Row % (row + QuickAppConstants.MaximumSequentialIndependentFields) == 0)
                    //    Target = sheet.Cells[row, QuickAppConstants.NextSuccessiveIndependentFieldStartColumnDiff + Target.Column];                   
                }
            }

            //int spacingBetweenIndependentAndRepeating = 3;
            //startRow = startRow == 0 ? row : (startRow + spacingBetweenIndependentAndRepeating); 
            // Do StartRow ++ to leave one blank row between Independent fields and Repeating fields Header
            startRow = startRow + 2;
            // Do col -- to start repeating fields from column A
            col--;

            //Repeating group fields
            var displayFields = Model.DisplayFields.Where(field => field.Display == true && field.ObjectId.Equals(Model.Object.Children[0].Id)).OrderBy(field => Convert.ToInt32(field.DisplayOrder));
            
            if (displayFields.Count() > 0)
            {
                Excel.Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;
                Excel.Range Target = sheet.Cells[startRow, col];

                foreach (QuickAppFieldAttribute field in displayFields)
                {
                    ApttusField apttusField = Model.Object.Children[0].Fields.Where(af => field.FieldName.Equals(af.Name)).FirstOrDefault();
                    string targetLocation = ExcelHelper.GetAddress(Target);
                    string namedRange = ExcelHelper.CreateUniqueNameRange();
                    ExcelHelper.AssignNameToRange(Target, namedRange);
                    retrieveMapcontroller.AddField(Model.Object.Children[0], apttusField, targetLocation, ObjectType.Repeating, namedRange);

                    Excel.Range FieldLabel = ExcelHelper.NextVerticalCell(Target, -1);
                    string fieldName = field.FieldName;
                    if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                        fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();
                    FieldLabel.Value = fieldName;

                    FieldLabel.EntireColumn.AutoFit();
                    if (salesforceIDColumn == null && field.FieldId.Equals(Model.Object.Children[0].IdAttribute))
                        salesforceIDColumn = FieldLabel;

                    //Next Target Location
                    Target = ExcelHelper.NextHorizontalCell(Target, 1);

                }
            }

            //Set the repeating group header to be displayed at runtime
            // Do add Header for List
            //retrieveMapcontroller.RetrieveMap.RepeatingGroups[0].GridHeader = Model.Object.Name + " " + Model.Object.Children[0].Name + " List";
            
            base.BuildDisplayMap(Model);
        }

        public override void BuildSearchAndSelectAction(WizardModel Model, ActionSelectionFilter action)
        {
            //Build ParentObject SearchAndSelect. Currently Only Master Object is supported as SearchAndSelect Action.
            string actionName = string.Empty;
            string recordType = string.Empty;
            SearchAndSelect searchAndSelect = new SearchAndSelect();
            if (action.ObjectType == RelationalObject.ParentObject)
            {
                actionName = "Search " + Model.Object.Name;
                recordType = "Single";                
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
            }
            else if (action.ObjectType == RelationalObject.ChildObject)
            {
                ApttusObject childObject = Model.Object.Children[0];
                actionName = "Search " + childObject.Name;
                recordType = "Multiple";
                searchAndSelect.TargetObject = childObject.UniqueId;
                searchAndSelect.GetSearchFieldsList(childObject.UniqueId);
                searchAndSelect.GetResultFieldsList(childObject.UniqueId);

                foreach (QuickAppFieldAttribute quickfield in Model.DisplayFields.Where(field => field.SearchFields == true && field.ObjectId.Equals(childObject.Id)))
                {
                    ApttusField apttusField = applicationDefinitionManager.GetField(childObject.UniqueId, quickfield.FieldId);
                    SearchField searchableField = searchAndSelect.AllSearchFields.Where(sf => sf.Id.Equals(apttusField.Id)).FirstOrDefault();
                    searchableField.IsSelected = true;
                }

                foreach (QuickAppFieldAttribute field in Model.DisplayFields.Where(field => field.ResultFields == true && field.ObjectId.Equals(childObject.Id)))
                {
                    ApttusField apttusField = applicationDefinitionManager.GetField(childObject.UniqueId, field.FieldId);
                    ResultField resultField = searchAndSelect.AllResultFields.Where(rf => rf.Id.Equals(apttusField.Id)).FirstOrDefault();
                    resultField.IsSelected = true;
                }

                var filters = Model.Filters.Where(filter => filter.Object.UniqueId.Equals(Model.Object.Children[0].UniqueId) && filter.RelationObject == RelationalObject.ChildObject).Select(filter => filter.Filters);
                searchAndSelect.SearchFilterGroups = new List<SearchFilterGroup>();
                if (filters != null && filters.Count() != 0)
                {
                    foreach(SearchFilterGroup filter in filters.FirstOrDefault())
                        searchAndSelect.SearchFilterGroups.Add(filter);
                }
            }

            searchAndSelect.Name = actionName;
            searchAndSelect.RecordType = recordType;
            searchAndSelect.PageSize = "10";
            searchAndSelectActions.Add(searchAndSelect);

            base.BuildSearchAndSelectAction(Model, action);
        }

        public override void BuildQueryAction(WizardModel Model, ActionSelectionFilter action)
        {
            if (action.ObjectType == RelationalObject.ChildObject)
            {
                string actionName = Model.Object.Children[0].Name + " Query";
                List<SearchFilterGroup> list = Model.Filters.Where(filter => filter.RelationObject == RelationalObject.ChildObject && filter.Object.Id.Equals(Model.Object.Children[0].Id)).Select(filter => filter.Filters).FirstOrDefault();
                queryAction.Save(actionName, Model.Object.Children[0], list, "1000");
            }
            base.BuildQueryAction(Model, action);
        }

        public override void BuildSaveMap(WizardModel Model, int saveMapIndex)
        {
            List<QuickAppFieldAttribute> saveFields = Model.DisplayFields.Where(field => field.Save == true).ToList();
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

        private void BuildRetrieveWorkflow(WizardModel Model)
        {
            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep1, guidStep2;

            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow.Name = QuickAppConstants.RetrieveWorkflowName;
            //actionFlow.AutoExecuteEditInExcelMode = Model.AllowEditInExcel;
            
            //Step 1. Search & Select
            Step step = new Step();
            step.Name = "Step 1";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = configManager.Actions.OfType<SearchAndSelect>().Where(action => action.TargetObject.Equals(Model.Object.UniqueId)).FirstOrDefault().Id;
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep1 = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = "ParentChildAppData1";

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            //Step 2. Execute Query
            step = new Step();
            step.Name = "Step 2";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 2";
            condition.SequenceNo = ++conditionCounter;

            //Find which action to use
            Core.ActionType typeOfAction = Model.Actions.Where(action => action.ObjectType == RelationalObject.ChildObject).FirstOrDefault().ActionType;
            wfAction = new WorkflowAction();

            if (typeOfAction == Core.ActionType.Query)
                wfAction.ActionId = configManager.Actions.OfType<QueryActionModel>().Where(action => action.TargetObject.Equals(Model.Object.Children[0].UniqueId)).FirstOrDefault().Id;
            else if (typeOfAction == Core.ActionType.SearchAndSelect)
                wfAction.ActionId = configManager.Actions.OfType<SearchAndSelect>().Where(action => action.TargetObject.Equals(Model.Object.Children[0].UniqueId)).FirstOrDefault().Id;

            WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep2 = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.InputData = true;
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep1.ToString() };
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = "ParentChildAppData2";

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            //Step 3. Display Action
            step = new Step();
            step.Name = "Step 3";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 3";
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
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep1.ToString(), guidStep2.ToString() };
            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            ActionFlows.Add(actionFlow);

            if (Model.AllowEditInExcel)
                BuildRetrieveWorkflowEditInExcel(Model);
        }

        private void BuildRetrieveWorkflowEditInExcel(WizardModel Model)
        {
            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep1, guidStep2;

            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow.Name = QuickAppConstants.RetrieveWorkflowEditInExcelName;
            actionFlow.AutoExecuteEditInExcelMode = Model.AllowEditInExcel;

            //Step 1. Create Query
            Step step = new Step();
            step.Name = "Step 1";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = BuildQueryActionEditInExcel(Model);
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep1 = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = "ParentChildAppData1_LaunchFromSF";

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            //Step 2. Execute Query
            step = new Step();
            step.Name = "Step 2";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 2";
            condition.SequenceNo = ++conditionCounter;

            //Find which action to use
            Core.ActionType typeOfAction = Model.Actions.Where(action => action.ObjectType == RelationalObject.ChildObject).FirstOrDefault().ActionType;
            wfAction = new WorkflowAction();

            if (typeOfAction == Core.ActionType.Query)
                wfAction.ActionId = configManager.Actions.OfType<QueryActionModel>().Where(action => action.TargetObject.Equals(Model.Object.Children[0].UniqueId)).FirstOrDefault().Id;
            else if (typeOfAction == Core.ActionType.SearchAndSelect)
                wfAction.ActionId = configManager.Actions.OfType<SearchAndSelect>().Where(action => action.TargetObject.Equals(Model.Object.Children[0].UniqueId)).FirstOrDefault().Id;

            WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep2 = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);
            wfAction.WorkflowActionData.InputData = true;
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep1.ToString() };
            wfAction.WorkflowActionData.OutputPersistData = true;
            wfAction.WorkflowActionData.OutputDataName = "ParentChildAppData2_LaunchFromSF";

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            //Step 3. Display Action
            step = new Step();
            step.Name = "Step 3";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 3";
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
            wfAction.WorkflowActionData.InputDataName = new string[] { guidStep1.ToString(), guidStep2.ToString() };
            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            ActionFlows.Add(actionFlow);
        }

        private string BuildQueryActionEditInExcel(WizardModel Model)
        {
            QueryActionModel queryActionModel = new QueryActionModel();
            queryAction = new QueryActionController(queryActionModel, null, FormOpenMode.Create);

            string actionName = Model.Object.Name + " Query_LaunchFromSF";
            List<SearchFilterGroup> list = new List<SearchFilterGroup>();
            SearchFilterGroup searchFilterGroup = new SearchFilterGroup();
            searchFilterGroup.Filters = new List<SearchFilter>();
            searchFilterGroup.LogicalOperator = LogicalOperator.AND;
            list.Add(searchFilterGroup);
            
            SearchFilter exportRecordIdSearchFilter = new SearchFilter();
            exportRecordIdSearchFilter.AppObjectUniqueId = Model.Object.UniqueId;
            exportRecordIdSearchFilter.FieldId = Model.Object.IdAttribute;
            exportRecordIdSearchFilter.ValueType = ExpressionValueTypes.SystemVariables;
            exportRecordIdSearchFilter.Value = ExpressionSystemVariables.ExportRecordId.ToString();
            exportRecordIdSearchFilter.Operator = Constants.EQUALS;
            exportRecordIdSearchFilter.SequenceNo = 1;
            exportRecordIdSearchFilter.QueryObjects = new List<QueryObject>();

            list[0].Filters.Add(exportRecordIdSearchFilter);

            queryAction.Save(actionName, Model.Object, list, "1000");

            return queryActionModel.Id;
        }

        public override void ConfigureWorkflow(WizardModel Model)
        {
            BuildRetrieveWorkflow(Model);
            BuildSaveWorkflow(Model);
            base.ConfigureWorkflow(Model);
        }
    }
}
