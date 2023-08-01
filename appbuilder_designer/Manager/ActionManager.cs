/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apttus.XAuthor.AppDesigner;
using Apttus.XAuthor.AppDesigner.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Every row in the grdAddModifyAction Grid will be represented by property called ActionProperties.
    /// </summary>
    public class ActionProperties
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }
        public string TargetObject { get; set; }
    }    

    public sealed class ActionManager
    {
        private static ActionManager actionManager;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;

        private ActionManager()
        {
        }

        public static ActionManager GetInstance()
        {
            if (actionManager == null)
                actionManager = new ActionManager();
            return actionManager;
        }

        public List<ActionProperties> GetActionsList(String actionType)
        {
            List<Action> actionsbase = configurationManager.Actions.Where(act => !act.IsActionExternal).ToList();
            if (actionsbase == null)
                return null;

            // If actionType is empty load all the actions else if actionType is specified, load the specific type of an action.
            IEnumerable<Action> actions = String.IsNullOrEmpty(actionType) ? actionsbase
                                                                            : (from element in actionsbase
                                                                               where element.Type.Equals(actionType)
                                                                               select element);


            List<ActionProperties> rows = new List<ActionProperties>();
            foreach (Action action in actions)
            {
                ActionProperties row = new ActionProperties();
                row.ActionId = action.Id;
                row.ActionName = action.Name;

                if (action.Type == "Retrieve")
                    row.ActionType = Constants.RETRIEVE_ACTION;
                else
                    row.ActionType = action.Type;

                rows.Add(row);
            }
            return rows;
        }

        public bool AddAction(String type)
        {
            bool bSuccess = false;
            ActionType actionType = GetActionType(type);
            switch (actionType)
            {
                case ActionType.SearchAndSelect:
                    bSuccess = LoadSearchAndSelectForm(string.Empty);
                    break;
                case ActionType.Display:
                    bSuccess = LoadRetrieveActionView(string.Empty);
                    break;
                case ActionType.Query:
                    bSuccess = LoadQueryActionView(string.Empty);
                    break;
                case ActionType.Save:
                    bSuccess = LoadSaveActionView(string.Empty);
                    break;
                case ActionType.SalesforceMethod:
                    bSuccess = LoadCallProcedureActionView(string.Empty);
                    break;
                case ActionType.Macro:
                    bSuccess = LoadMacroActionView(string.Empty);
                    break;
                case ActionType.SaveAttachment:
                    bSuccess = LoadSaveAttachmentActionView(string.Empty);
                    break;
                case ActionType.Clear:
                    bSuccess = LoadClearActionView(string.Empty);
                    break;
                case ActionType.CheckIn:
                    bSuccess = LoadCheckInActionView(string.Empty);
                    break;
                case ActionType.CheckOut:
                    bSuccess = LoadCheckOutActionView(string.Empty);
                    break;
                case ActionType.InputAction:
                    bSuccess = LoadInputAction(string.Empty);
                    break;
                case ActionType.PasteRowAction:
                    bSuccess = LoadPasteRowAction(string.Empty);
                    break;
                case ActionType.PasteSourceDataAction:
                    {
                        //there cannot be more than 1 paste source data action per app.
                        PasteSourceDataActionModel model = configurationManager.Actions.OfType<PasteSourceDataActionModel>().FirstOrDefault();
                        string actionId = model == null ? string.Empty : model.Id;
                        bSuccess = LoadPasteSourceDataAction(actionId);
                    }
                    break;
                case ActionType.SwitchConnection:
                    bSuccess = LoadSwitchConnectionAction(string.Empty);
                    break;
                case ActionType.AddRow:
                    bSuccess = LoadAddRowAction(string.Empty);
                    break;
                case ActionType.DataSetAction:
                    bSuccess = LoadDataSetAction(string.Empty);
                    break;
                case ActionType.Delete:
                    bSuccess = LoadDeleteAction(string.Empty);
                    break;
                default:
                    break;
            }
            return bSuccess;
        }


        public bool EditAction(ActionProperties actionProperties)
        {
            ActionType actionType = GetActionType(actionProperties.ActionType);
            bool bSuccess = false;
            switch (actionType)
            {
                case ActionType.SearchAndSelect:
                    bSuccess = LoadSearchAndSelectForm(actionProperties.ActionId);
                    break;
                case ActionType.Display:
                    bSuccess = LoadRetrieveActionView(actionProperties.ActionId);
                    break;
                case ActionType.Query:
                    bSuccess = LoadQueryActionView(actionProperties.ActionId);
                    break;
                case ActionType.Save:
                    bSuccess = LoadSaveActionView(actionProperties.ActionId);
                    break;
                case ActionType.SalesforceMethod:
                    bSuccess = LoadCallProcedureActionView(actionProperties.ActionId);
                    break;
                case ActionType.Macro:
                    bSuccess = LoadMacroActionView(actionProperties.ActionId);
                    break;
                case ActionType.SaveAttachment:
                    bSuccess = LoadSaveAttachmentActionView(actionProperties.ActionId);
                    break;
                case ActionType.Clear:
                    bSuccess = LoadClearActionView(actionProperties.ActionId);
                    break;
                case ActionType.CheckIn:
                    bSuccess = LoadCheckInActionView(actionProperties.ActionId);
                    break;
                case ActionType.CheckOut:
                    bSuccess = LoadCheckOutActionView(actionProperties.ActionId);
                    break;
                case ActionType.InputAction:
                    bSuccess = LoadInputAction(actionProperties.ActionId);
                    break;
                case ActionType.PasteSourceDataAction:
                    bSuccess = LoadPasteSourceDataAction(actionProperties.ActionId);
                    break;
                case ActionType.PasteRowAction:
                    bSuccess = LoadPasteRowAction(actionProperties.ActionId);
                    break;
                case ActionType.SwitchConnection:
                    bSuccess = LoadSwitchConnectionAction(actionProperties.ActionId);
                    break;
                case ActionType.AddRow:
                    bSuccess = LoadAddRowAction(actionProperties.ActionId);
                    break;
                case ActionType.DataSetAction:
                    bSuccess = LoadDataSetAction(actionProperties.ActionId);
                    break;
                case ActionType.Delete:
                    bSuccess = LoadDeleteAction(actionProperties.ActionId);
                    break;
                default:
                    break;
            }
            return bSuccess;
        }

        private bool LoadDeleteAction(string actionId)
        {
            DeleteActionModel model;
            DeleteActionView view = new DeleteActionView();
            FormOpenMode formOpenMode;
            if (String.IsNullOrEmpty(actionId))
            {
                model = new DeleteActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as DeleteActionModel;
                if(model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }
            DeleteActionController controller = new DeleteActionController(model, view, formOpenMode);
            return true;
        }

        private bool LoadDataSetAction(string actionId)
        {
            DataSetActionModel model;
            DataSetActionView view = new DataSetActionView();
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new DataSetActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as DataSetActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }
            DataSetActionController controller = new DataSetActionController(view, model, formOpenMode);
            return true;
        }


        private bool LoadPasteRowAction(string actionId)
        {
            PasteRowActionModel model;
            PasteRowActionView view = new PasteRowActionView();
            PasteRowActionController controller;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new PasteRowActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as PasteRowActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }
            controller = new PasteRowActionController(view, model, formOpenMode);
            return true;
        }   

        public bool DeleteAction(ActionProperties actionProperties)
        {
            return configurationManager.DeleteAction(actionProperties.ActionId);
        }

        private bool LoadSearchAndSelectForm(string actionId)
        {
            if (String.IsNullOrEmpty(actionId)) //There is no actionId, which means load the form to add a specific action.
            {
                SearchAndSelectActionView searchAndSelectConfigurationForm = new SearchAndSelectActionView();
                searchAndSelectConfigurationForm.ShowDialog();
            }
            else
            {
                SearchAndSelect searchAndSelect = configurationManager.GetActionById(actionId) as SearchAndSelect;
                if (searchAndSelect == null)
                    return false;

                //Load SearchAndSelect Form
                SearchAndSelectActionView searchAndSelectConfigurationForm = new SearchAndSelectActionView(searchAndSelect);
                searchAndSelectConfigurationForm.ShowDialog();
            }
            return true;
        }

        private bool LoadRetrieveActionView(string actionId)
        {

            RetrieveActionView view = new RetrieveActionView();
            RetrieveActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new RetrieveActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as RetrieveActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            RetrieveActionController controller = new RetrieveActionController(model, view, formOpenMode);

            return true;
        }

        private bool LoadSaveActionView(string actionId)
        {

            SaveActionView view = new SaveActionView();
            SaveActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new SaveActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as SaveActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            SaveActionController controller = new SaveActionController(model, view, formOpenMode);

            return true;
        }

        private bool LoadQueryActionView(string actionId)
        {
            QueryActionView view = new QueryActionView();
            QueryActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new QueryActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as QueryActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            QueryActionController controller = new QueryActionController(model, view, formOpenMode);

            return true;
        }

        private bool LoadCallProcedureActionView(string actionId)
        {
            CallProcedure view = new CallProcedure();
            CallProcedureModel model;
            FormOpenMode formMode;

            if (string.IsNullOrEmpty(actionId))
            {
                model = new CallProcedureModel();
                formMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as CallProcedureModel;
                if (model == null)
                    return false;
                formMode = FormOpenMode.Edit;
            }

            CallProcedureController controller = new CallProcedureController(model, view, formMode);

            return true;
        }

        /// <summary>
        /// LoadMacroView for Add and Edit
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        private bool LoadMacroActionView(string actionId)
        {

            MacroActionView view = new MacroActionView();
            MacroActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new MacroActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as MacroActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            //if (FormDataLoadingCompleted != null)
            //    FormDataLoadingCompleted();
            MacroActionController controller = new MacroActionController(model, view, formOpenMode);

            return true;
        }

        private bool LoadSaveAttachmentActionView(string actionId)
        {
            SaveAttachmentView view = new SaveAttachmentView();
            SaveAttachmentModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new SaveAttachmentModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as SaveAttachmentModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            SaveAttachmentController controller = new SaveAttachmentController(view, model, formOpenMode);

            return true;
        }

        private bool LoadClearActionView(string actionId)
        {
            ClearActionView view = new ClearActionView();
            ClearActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new ClearActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as ClearActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            ClearActionController controller = new ClearActionController(view, model, formOpenMode);

            return true;
        }

        private bool LoadCheckInActionView(string actionId)
        {
            CheckInView view = new CheckInView();
            CheckInModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new CheckInModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as CheckInModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            CheckInController controller = new CheckInController(view, model, formOpenMode);
            return true;
        }

        private bool LoadCheckOutActionView(string actionId)
        {
            CheckOutView view = new CheckOutView();
            CheckOutModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new CheckOutModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as CheckOutModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            CheckOutController controller = new CheckOutController(view, model, formOpenMode);
            return true;
        }

        private bool LoadInputAction(string actionId)
        {
            InputActionView view = new InputActionView();
            InputActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new InputActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as InputActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            InputActionController controller = new InputActionController(view, model, formOpenMode);
            return true;
        }

        private bool LoadPasteSourceDataAction(string actionId)
        {
            PasteSourceDataActionView view = new PasteSourceDataActionView();
            PasteSourceDataActionModel model;
            FormOpenMode mode;
            if (string.IsNullOrEmpty(actionId))
            {
                model = new PasteSourceDataActionModel();
                mode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as PasteSourceDataActionModel;
                if (model == null)
                    return false;
                mode = FormOpenMode.Edit;
            }

            PasteSourceDataActionController controller = new PasteSourceDataActionController(model, view, mode);
            return true;
        } 

        private bool LoadSwitchConnectionAction(string actionId)
        {
            SwitchConnectionActionView view = new SwitchConnectionActionView();
            SwitchConnectionActionModel model;
            FormOpenMode formOpenMode;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new SwitchConnectionActionModel();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                model = configurationManager.GetActionById(actionId) as SwitchConnectionActionModel;
                if (model == null)
                    return false;
                formOpenMode = FormOpenMode.Edit;
            }

            SwitchConnectionController controller = new SwitchConnectionController(model, view, formOpenMode);
            return true;
        }

        private bool LoadAddRowAction(string actionId)
        {
            FormOpenMode mode;
            AddRowActionView view = new AddRowActionView();
            AddRowActionModel model;

            if (String.IsNullOrEmpty(actionId))
            {
                model = new AddRowActionModel();
                mode = FormOpenMode.Create;
            }
            else
            {
                mode = FormOpenMode.Edit;
                model = configurationManager.GetActionById(actionId) as AddRowActionModel;
                if (model == null)
                    return false;
            }

            AddRowActionController controller = new AddRowActionController(view, model, mode);
            return true;
        }  

        private ActionType GetActionType(string actionType)
        {
            ActionType type = ActionType.None;
            switch (actionType)
            {
                case "SearchAndSelect":
                    type = ActionType.SearchAndSelect;
                    break;
                case "Form":
                    type = ActionType.Form;
                    break;
                case "SalesforceMethod":
                    type = ActionType.SalesforceMethod;
                    break;
                case "Save":
                    type = ActionType.Save;
                    break;
                case "Display":
                    type = ActionType.Display;
                    break;
                case "Query":
                    type = ActionType.Query;
                    break;
                case "Macro":
                    type = ActionType.Macro;
                    break;
                case Constants.SAVE_ATTACHMENT_ACTION:
                    type = ActionType.SaveAttachment;
                    break;
                case Constants.PASTEROW_ACTION:
                    type = ActionType.PasteRowAction;
                    break;
                case Constants.CLEAR_ACTION:
                    type = ActionType.Clear;
                    break;
                case Constants.CHECK_IN_ACTION:
                    type = ActionType.CheckIn;
                    break;
                case Constants.CHECK_OUT_ACTION:
                    type = ActionType.CheckOut;
                    break;
                case "Retrieve":
                    type = ActionType.Display;
                    break;

                // For Existing Data
                case "ExecuteQuery":
                    type = ActionType.Query;
                    break;

                case "CallProcedure":
                    type = ActionType.SalesforceMethod;
                    break;

                case Constants.INPUT_ACTION:
                    type = ActionType.InputAction;
                    break;
                case Constants.PASTESOURCEDATA_ACTION:
                    type = ActionType.PasteSourceDataAction;
                    break;
                case Constants.SWITCH_CONNECTION_ACTION:
                    type = ActionType.SwitchConnection;
                    break;
                case Constants.ADDROW_ACTION:
                    type = ActionType.AddRow;
                    break;
                case Constants.DataSetAction:
                    type = ActionType.DataSetAction;
                    break;
                case Constants.DELETE_ACTION:
                    type = ActionType.Delete;
                    break;

            }
            return type;
        }

        public bool DeleteAction(string ActionId)
        {
            return configurationManager.DeleteAction(ActionId);
        }
    }
}
