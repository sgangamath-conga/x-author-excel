/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class ActionsValidator : IValidator
    {
        private static ActionsValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public static ActionsValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ActionsValidator();
                        configManager = ConfigurationManager.GetInstance;
                    }
                }
                return instance;
            }
        }

        private bool CanDeleteAction(Core.ActionProperties actionProperties, ValidationResult result)
        {
            bool bCanDelete = true;
            switch (actionProperties.ActionType)
            {
                case Constants.INPUT_ACTION:
                    bCanDelete = CanDeleteInputAction(actionProperties, result);
                    break;
            }
            return bCanDelete;
        }

        private bool CanDeleteInputAction(ActionProperties actionProperties, ValidationResult result)
        {
            bool bCanDelete = true;
            Core.InputActionModel inputAction = configManager.Actions.Where(action => action.Id.Equals(actionProperties.ActionId)).FirstOrDefault() as Core.InputActionModel;

            //Check if any of the action variables are consumed by QueryAction or SearchAndSelect Action
            // 1. Query Action. 
            string searchFilterValue = inputAction.Id + Constants.DOT + "{0}";
            List<Core.QueryActionModel> queryActions = configManager.Actions.OfType<Core.QueryActionModel>().ToList();
            if (queryActions.Count != 0)
            {
                var items = from queryAction in queryActions
                        from searchFilterGroup in queryAction.WhereFilterGroups
                        from searchFilter in searchFilterGroup.Filters.Where(filter => filter.ValueType == ExpressionValueTypes.UserInput)
                        from variable in inputAction.InputActionVariables
                        where searchFilter.Value.Equals(String.Format(searchFilterValue,variable.VariableName))
                        select new { queryAction, variable };

                bCanDelete = items.Count() == 0;
                if (!bCanDelete)
                {
                    result.Success = false;
                    if (result.Messages == null)
                        result.Messages = new List<string>();
                    foreach(var item in items)
                    {
                        StringBuilder sb = new StringBuilder(String.Format(resourceManager.GetResource("ACTVALIDATOR_CanDeleteInputAction_InfoMsg"), item.variable.VariableName, inputAction.Name, item.queryAction.Name));
                        result.Messages.Add(sb.ToString());
                    }
                }
            }

            // 2. Search And Select Action
            List<Core.SearchAndSelect> searchActions = configManager.Actions.OfType<Core.SearchAndSelect>().ToList();
            if (searchActions.Count != 0)
            {
                var items = from searchAction in searchActions
                        from searchFilterGroup in searchAction.SearchFilterGroups
                        from searchFilter in searchFilterGroup.Filters.Where(filter => filter.ValueType == ExpressionValueTypes.UserInput)
                        from variable in inputAction.InputActionVariables
                        where searchFilter.Value.Equals(String.Format(searchFilterValue,variable.VariableName))
                        select new { searchAction, variable };

                bCanDelete = bCanDelete && items.Count() == 0;

                if (items.Count() != 0)
                {
                    result.Success = false;
                    if (result.Messages == null)
                        result.Messages = new List<string>();
                    foreach (var item in items)
                    {
                        StringBuilder sb = new StringBuilder(String.Format(resourceManager.GetResource("ACTVALIDATOR_CanDeleteInputActionSS_InfoMsg"), item.variable.VariableName, inputAction.Name, item.searchAction.Name));
                        result.Messages.Add(sb.ToString());
                    }
                }
            }
            return bCanDelete;
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            Core.ActionProperties action = t as Core.ActionProperties;

            ValidationResult result = new ValidationResult();
            result.Success = true;
            result.EntityType = EntityType.Actions;
            result.EntityName = action.ActionName;                       

            // Get all workflow
            List<Core.Workflow> alltWorkflows = ConfigurationManager.GetInstance.Workflows;
            if (alltWorkflows.Count == 0)
                return results;

            // Check if action exists in workflow or not.
            List<Workflow> lstWorkflow = new List<Workflow>();
            foreach (Workflow wfconfig in alltWorkflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfconfig;

                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.ActionId == action.ActionId)
                                lstWorkflow.Add(wfconfig);
                        }
                    }
                }
            }

            bool bCanDelete = CanDeleteAction(action, result);

            // Return result
            if (lstWorkflow.Count != 0)
            {
                //Set the success to false
                result.Success = false;

                //Build & Set the Error Message
                StringBuilder errorMsgBuilder = new StringBuilder(action.ActionName);
                errorMsgBuilder.Append(" is used by following ActionFlow(s) : ");

                bool bAddComma = false;

                foreach (Workflow wfaction in lstWorkflow)
                {
                    if (bAddComma)
                        errorMsgBuilder.Append(",");
                    errorMsgBuilder.Append(wfaction.Name);
                    bAddComma = true;
                }
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_HenceCanNot_Msg"));

                if (result.Messages == null)
                    result.Messages = new List<string>();
                result.Messages.Add(errorMsgBuilder.ToString());
            }

            if (result.Success == false)
                results.Add(result);

            return results;
        }

        public List<ValidationResult> Validate<T>(T action)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<Core.Action> actions = new List<Core.Action>();

            if (action == null)
                actions = ConfigurationManager.GetInstance.Actions;
            else
                actions = new List<Core.Action> { (action as Core.Action) };

            List<ValidationResult> resultSaveAttcahment = ValidateSaveAttachment(actions);

            if(resultSaveAttcahment.Count != 0)
                results.AddRange(resultSaveAttcahment);

            return results;
        }

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }

        internal List<ValidationResult> ValidateSaveAttachment(List<Core.Action> Actions)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<Core.SaveAttachmentModel> saveAttachmentsActions = Actions.OfType<SaveAttachmentModel>().ToList();
            List<String> currentSheetNames = ExcelHelper.GetActiveBookSheetNames();

            foreach (Core.SaveAttachmentModel model in saveAttachmentsActions) {
                foreach (string sheetName in model.IncludedSheets) {
                    if (!currentSheetNames.Contains(sheetName)) {
                        ValidationResult result = new ValidationResult();
                        result.EntityName = model.Name;
                        result.EntityType = EntityType.Actions;
                        String msg = String.Format(resourceManager.GetResource("ACTIONVALID_ValidateSaveAttachment_ErrMsg"), sheetName, model.Name);
                        if (result.Messages == null)
                            result.Messages = new List<string>();
                        result.Messages.Add(msg);
                        result.Success = false;
                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}
