/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Controller.Actions.CPQAction;
using Apttus.XAuthor.Core.CPQ;
using System.IO;

namespace Apttus.XAuthor.AppRuntime
{
    public interface IInputActionData
    {
        bool InputData { get; set; }

        string[] InputDataName { get; set; }
    }

    public interface IOutputActionData
    {
        bool OutputPersistData { get; set; }

        string OutputDataName { get; set; }
    }

    public class WorkflowEngine
    {
        // Get configManager instance
        ConfigurationManager configManager = ConfigurationManager.GetInstance;

        // Get DataManager instance
        DataManager dataManager = DataManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

        StringBuilder saveSummaryAcrossWorkFlow;

        public void Execute(string workflowId)
        {
            try
            {
                if (string.IsNullOrEmpty(workflowId))
                    return;

                saveSummaryAcrossWorkFlow = new StringBuilder();

                WorkflowStructure workflow = (WorkflowStructure)configManager.GetWorkflowById(workflowId);

                // if Action flow is using action not allowed in a given Edition stop execution
                //if (!LicenseCheckWorkflow(workflow))
                //    return;

                // Stop execution if multi connect returns false, i.e. all connections are not done by runtime user
                if (!DoMultiConnect(workflow))
                    return;

                lock (Modules.ApttusCommandBarManager.GetInstance().g_PickListSynchLock)

                    for (int stCnt = 0; stCnt < workflow.Steps.Count; stCnt++)
                    {
                        //moveToStep:
                        string sStep = workflow.Steps[stCnt].Name;

                        for (int condCnt = 0; condCnt < workflow.Steps[stCnt].Conditions.Count; condCnt++)
                        {
                            // If condition criteria is not blank, evaluate condition
                            if (!string.IsNullOrEmpty(workflow.Steps[stCnt].Conditions[condCnt].Criteria))
                            {
                                string sCondition = workflow.Steps[stCnt].Conditions[condCnt].Criteria;
                                // TODO:: evaluate condition using Condition parser to be developed.
                            }

                            // TODO:: Add If condition, check if condition evaluation result if condition evaluated to true then proceed to Execute Action

                            for (int actionCnt = 0; actionCnt < workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions.Count; actionCnt++)
                            {
                                if (workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Type.Equals("Action"))
                                {
                                    string sActionId = workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId;

                                    // Get Action and execute
                                    Core.Action objAction = configManager.GetActionById(sActionId);

                                    ActionResult res = DoAction(objAction, workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt]);

                                    if (res == null || res.Status != ActionResultStatus.Success)
                                        return;
                                }

                                // Handle Move to Step scenario, loop back to previous or forward step
                                if (workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Type.Equals("MoveStep"))
                                {
                                    string sMoveStep = workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Name;

                                    int stepNo = int.Parse(workflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId);

                                    // TODO:: Move to Step, think out scenarios for forward & backward steps
                                    // Do -1 to accomodate for initialization
                                    //stCnt = stepNo - 1;
                                    //goto moveToStep;
                                }
                            }
                        }
                    }

                // Remove non persistent data 
                RemoveNonPersistedData();
                //reset row highlighting status
                resetHighlighting();
            }

            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            GC.Collect(); // ABB Memory Issue
        }

        private bool IsActionSupportedInBatchMode(Core.Action objAction)
        {
            Type ActionType = objAction.GetType();
            if (ActionType == typeof(SearchAndSelect) || 
                ActionType == typeof(CallProcedureModel) ||
                ActionType == typeof(CheckInModel) ||
                ActionType == typeof(CheckOutModel) ||
                ActionType == typeof(DataSetActionModel) ||
                ActionType == typeof(SwitchConnectionActionModel) ||
                ActionType == typeof(PasteRowActionModel) ||
                ActionType == typeof(PasteSourceDataActionModel))
            {
                return false;
            }
            return true;

        }

        internal ActionResult RunExternalAction(Core.Action objAction, string[] inputDataName, bool outputPersistData, string outputDataName)
        {
            ActionResult result = new ActionResult();
            result.Status = ActionResultStatus.Pending_Execution;

            ActionResponse response = null;
            try
            {
                result.Status = ActionResultStatus.Pending_Execution;
                ExternalAction externalAction = objAction as ExternalAction;
                ExternalLibrary model = configManager.Application.ExternalLibraries.Where(lib => lib.Id == externalAction.ExternalLibraryId).FirstOrDefault();
                if (model != null)
                {
                    string externalLibDirectory = Utils.GetDirectoryForExternalLib();

                    DirectoryInfo di = new DirectoryInfo(externalLibDirectory);
                    FileInfo fileInfo = di.GetFiles().Where(file => file.Name.Equals(model.ExternalLibraryName)).FirstOrDefault();

                    if (fileInfo != null)
                    {
                        string externalLibraryPath = Path.Combine(externalLibDirectory, model.ExternalLibraryName);
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(externalLibraryPath);

                        Type externalActionType = assembly.GetExportedTypes().Where(type => type.IsClass
                                        && type.Name.Equals(externalAction.TypeName)
                                        && type.FullName.Equals(externalAction.TypeFullName)).FirstOrDefault();

                        IActionRuntime instance = Activator.CreateInstance(externalActionType) as IActionRuntime;
                        if (instance != null)
                        {
                            // Pass the Action Request to the External Action
                            response = instance.Execute(GetActionRequest());

                            // Let External Action set the Action Result Status
                            result.Status = response.Status;
                        }
                    }
                }
                else
                {
                    ExceptionLogHelper.DebugLog("Unable to locate external library for external action :" + objAction.Name);
                }
            }
            catch (Exception ex)
            {
                result.Status = ActionResultStatus.Failure;
            }
            return result;
        }

        private ActionRequest GetActionRequest()
        {
            ActionRequest request = new ActionRequest();

            //TODO - need to think how action request can be applicable for other CRM
            // Request Header
            //request.RequestHeader.AccessToken = Globals.ThisAddIn.oAuthWrapper.token.access_token;
            //request.RequestHeader.InstanceUrl = Globals.ThisAddIn.oAuthWrapper.token.instance_url;
            //request.RequestHeader.ApiVersion = "35.0";

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objAction"></param>
        internal ActionResult DoAction(Core.Action objAction, Core.WorkflowAction wfAction)
        {
            string[] inputDataNames = null;

            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                inputDataNames = wfAction.WorkflowActionData != null ? ResetInputDataNames(wfAction.WorkflowActionData.InputDataName) : null;
            else if (ObjectManager.RuntimeMode == RuntimeMode.Batch)
            {
                if (wfAction.WorkflowActionData != null)
                    inputDataNames = wfAction.WorkflowActionData.InputIsExternal ? wfAction.WorkflowActionData.InputDataName :
                                                                                   ResetInputDataNames(wfAction.WorkflowActionData.InputDataName);

                if (!IsActionSupportedInBatchMode(objAction))
                {
                    ActionResult res = new ActionResult
                    {
                        Status = ActionResultStatus.Success
                    };
                    return res;
                }
            }

            ActionResult actionRes = null;
            try
            {
                if (objAction.IsActionExternal)
                    actionRes = RunExternalAction(objAction, inputDataNames, wfAction.WorkflowActionData.OutputPersistData, wfAction.WorkflowActionData.OutputDataName);
                if (objAction.GetType() == typeof(Core.SearchAndSelect))
                {
                    // Create object S&S runtime class and call execute method.
                    SearchAndSelectActionController ssAction = new SearchAndSelectActionController((Core.SearchAndSelect)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.OutputPersistData)
                    {
                        ssAction.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                        ssAction.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                    }

                    ssAction.InputData = wfAction.WorkflowActionData.InputData;

                    actionRes = ssAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.RetrieveActionModel))
                {
                    RetrieveMap rMap = configManager.RetrieveMaps.Where(retrieveMap => retrieveMap.Id.Equals(((Core.RetrieveActionModel)objAction).RetrieveMapId)).FirstOrDefault();

                    MatrixMap mMap = configManager.MatrixMaps.Where(matrixMap => matrixMap.Id.Equals(((Core.RetrieveActionModel)objAction).RetrieveMapId)).FirstOrDefault();

                    if (rMap != null)
                    {
                        // Create object of retrieve runtime class and do execute.
                        RetrieveAction retrieveAction = new RetrieveAction((Core.RetrieveActionModel)objAction);

                        if (wfAction.WorkflowActionData.InputData)
                        {
                            retrieveAction.InputData = wfAction.WorkflowActionData.InputData;
                            retrieveAction.InputDataName = inputDataNames;
                        }
                        actionRes = retrieveAction.Execute();
                    }

                    if (mMap != null)
                    {
                        MatrixActionController matrixAction = new MatrixActionController((Core.RetrieveActionModel)objAction);
                        if (wfAction.WorkflowActionData.InputData)
                        {
                            matrixAction.InputData = wfAction.WorkflowActionData.InputData;
                            matrixAction.InputDataName = inputDataNames;
                        }
                        actionRes = matrixAction.Execute();
                    }
                }
                else if (objAction.GetType() == typeof(Core.SaveActionModel))
                {
                    bool SurpressSaveMessage = wfAction.WorkflowActionData.SupressSaveMessage;

                    // Create object of retrieve runtime class and do execute.
                    SaveActionController saveAction = new SaveActionController((Core.SaveActionModel)objAction, SurpressSaveMessage, ref saveSummaryAcrossWorkFlow);
                    actionRes = saveAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.QueryActionModel))
                {
                    // Create object Query action runtime class and call execute method.
                    QueryActionController queryAction = new QueryActionController((Core.QueryActionModel)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.OutputPersistData)
                    {
                        queryAction.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                        queryAction.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                    }

                    queryAction.InputData = wfAction.WorkflowActionData.InputData;

                    actionRes = queryAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.CallProcedureModel))
                {
                    CallProcedureController callProc = new CallProcedureController((Core.CallProcedureModel)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.OutputPersistData)
                    {
                        callProc.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                        callProc.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                    }

                    callProc.InputData = wfAction.WorkflowActionData.InputData;

                    actionRes = callProc.Execute();
                }
                else if (objAction.GetType() == typeof(Core.MacroActionModel))
                {
                    MacroController macroController = new MacroController((Core.MacroActionModel)objAction);

                    actionRes = macroController.Execute();
                }
                else if (objAction.GetType() == typeof(Core.SaveAttachmentModel))
                {
                    SaveAttachmentController saveAttachmentAction = new SaveAttachmentController((Core.SaveAttachmentModel)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.InputData)
                    {
                        saveAttachmentAction.InputData = wfAction.WorkflowActionData.InputData;
                        saveAttachmentAction.InputDataName = inputDataNames;
                    }
                    actionRes = saveAttachmentAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.ClearActionModel))
                {
                    ClearActionController clearAction = new ClearActionController((Core.ClearActionModel)objAction);

                    actionRes = clearAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.CheckInModel))
                {
                    CheckInController checkInAction = new CheckInController((Core.CheckInModel)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.InputData)
                    {
                        checkInAction.InputData = wfAction.WorkflowActionData.InputData;
                        checkInAction.InputDataName = inputDataNames;
                    }
                    actionRes = checkInAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.CheckOutModel))
                {
                    CheckOutController checkOutAction = new CheckOutController((Core.CheckOutModel)objAction, inputDataNames);

                    if (wfAction.WorkflowActionData.InputData)
                    {
                        checkOutAction.InputData = wfAction.WorkflowActionData.InputData;
                        checkOutAction.InputDataName = inputDataNames;
                    }
                    actionRes = checkOutAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.PasteSourceDataActionModel))
                {
                    PasteSourceDataActionController controller = new PasteSourceDataActionController(objAction as PasteSourceDataActionModel);
                    actionRes = controller.Execute();
                }
                else if (objAction.GetType() == typeof(Core.SwitchConnectionActionModel))
                {
                    SwitchConnectionActionController switchConnectionAction = new SwitchConnectionActionController((Core.SwitchConnectionActionModel)objAction);
                    actionRes = switchConnectionAction.Execute();
                }
                else if (objAction.GetType() == typeof(Core.AddRowActionModel))
                {
                    AddRowActionController addRowActionController = new AddRowActionController(objAction as Core.AddRowActionModel);
                    if (wfAction.WorkflowActionData.InputData)
                        addRowActionController.InputDataName = inputDataNames;
                    actionRes = addRowActionController.Execute();
                }
                else if (objAction.GetType() == typeof(Core.PasteRowActionModel))
                {
                    PasteRowActionController pasteRowController = new PasteRowActionController(objAction as Core.PasteRowActionModel);
                    actionRes = pasteRowController.Execute();
                }
                else if (objAction.GetType() == typeof(Core.DataSetActionModel))
                {
                    DataSetActionController controller = new DataSetActionController(objAction as Core.DataSetActionModel);
                    if (wfAction.WorkflowActionData.InputData && wfAction.WorkflowActionData.OutputPersistData)
                    {
                        controller.InputDataNames = inputDataNames;
                        controller.OutputDataName = wfAction.WorkflowActionData.OutputDataName;

                        actionRes = controller.Execute();
                    }
                }
                else if (objAction.GetType() == typeof(Core.DeleteActionModel))
                {
                    DeleteActionController controller = new DeleteActionController(objAction as DeleteActionModel, inputDataNames);
                    actionRes = controller.Execute();
                }
                else if (objAction.Id.StartsWith(CPQBase.CPQID))
                {
                    CartController ctroller = CPQCartControllerBase.GetCartController(objAction, wfAction, inputDataNames);
                    if (ctroller != null)
                    {
                        actionRes = ctroller.Execute();
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow");
            }
            return actionRes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDataNames"></param>
        /// <returns></returns>
        internal string[] ResetInputDataNames(string[] inputDataNames)
        {
            if (inputDataNames == null)
                return null;

            List<string> lstInputData = new List<string>();

            for (int i = 0; i < inputDataNames.Length; i++)
                lstInputData.Add(configManager.GetOutputDataName(inputDataNames[i]));


            if (lstInputData.Count == 0 && inputDataNames.Length > 0)
                return inputDataNames;
            else
                return lstInputData.ToArray();
        }

        internal bool DoMultiConnect(WorkflowStructure workflow)
        {
            List<SwitchConnectionActionModel> connections = configManager.GetWorkflowSwitchConnectionActions(workflow);

            if (connections.Count > 0)
            {
                SwitchConnectionsLoginView view = new SwitchConnectionsLoginView();
                SwitchConnectionsLoginViewController controller = new SwitchConnectionsLoginViewController(connections, view);
                return controller.allConnectionsLoggedIn;
            }
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        internal bool LicenseCheckWorkflow(WorkflowStructure workflow)
        {
            LicenseActivationManager licenseManager = LicenseActivationManager.GetInstance;
            bool returnVal = false;
            foreach (Step step in workflow.Steps)
            {
                foreach (Condition cond in step.Conditions)
                {
                    foreach (WorkflowAction wfAction in cond.WorkflowActions)
                    {
                        // Get Action By Id
                        Core.Action objAction = configManager.GetActionById(wfAction.ActionId);
                        returnVal = licenseManager.IsFeatureAvailable(objAction.Type);
                        if (!returnVal)
                        {
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("WORKFLOW_LicenseCheck_ErrorMsg"), objAction.Type), Constants.RUNTIME_PRODUCT_NAME);
                            break;
                        }
                    }
                }
            }
            return returnVal;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RemoveNonPersistedData()
        {
            for (int i = 0; i < dataManager.AppData.Count; i++)
            {
                if (string.IsNullOrEmpty(dataManager.AppData[i].Name))
                    dataManager.AppData.Remove(dataManager.AppData[i]);
            }
            saveSummaryAcrossWorkFlow = null;
        }
        /// <summary>
        /// resets highlighting status for the current workflow
        /// </summary>
        private void resetHighlighting()
        {
            string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
            if (!string.IsNullOrEmpty(appUniqueId))
            {
                //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
                ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);

                if (appInstance != null && appInstance.RowHighLightingData != null)
                {
                    appInstance.RowHighLightingData.isCleared = false;
                }
            }
        }
    }
}
