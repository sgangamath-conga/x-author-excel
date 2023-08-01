/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class WorkflowController
    {
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

        Dictionary<string, string> lstSteps = null;
        Dictionary<string, string> lstActions = null;
        List<WorkflowActionData> lstWFActionData = null;

        int iStepCounter = 0;

        WorkflowStructure objWorkflow = null;
        WorkflowAction wfAction = null;
        Condition wfCondition = null;
        Step wfStep = null;

        int actionCounter = 0;
        int conditionCounter = 0;
        int stepCounter = 0;
        private FormOpenMode formOpenMode;
        private Guid tmpWfId = Guid.Empty;
        WorkflowStructure cloneWorkflow = null;
        private string stepID = string.Empty;
        private bool bIsValidWfElement = false;
        private string wfValidationMessage = string.Empty;

        public WorkflowController()
        {
            lstActions = new Dictionary<string, string>();
            lstSteps = new Dictionary<string, string>();
            lstWFActionData = new List<WorkflowActionData>();
            lstWFActionData = configurationManager.GetWorkflowActionData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tv"></param>
        public bool SaveWorkflowToConfig(TreeView tv)
        {
            try
            {
                bool bIsValidWF = false;

                // Save Auto-Execute flags to restore after reset
                //HashSet<WorkflowStructure.Trigger> triggers = objWorkflow.Triggers;
                bool autoExecuteRegularMode = objWorkflow.AutoExecuteRegularMode;
                bool autoExecuteEditInExcelMode = objWorkflow.AutoExecuteEditInExcelMode;

                ResetWorkflow();

                // Write our root node as workflow name
                objWorkflow.Name = tv.Nodes[0].Text;

                foreach (TreeNode node in tv.Nodes)
                {
                   bIsValidWF = SaveNodeElement(node.Nodes);
                   if (!bIsValidWF)
                   {
                       ApttusMessageUtil.ShowError(wfValidationMessage, Utils.GetEnumDescription(ListScreenType.Actionflow, string.Empty));
                       return false;
                   }
                }

                //objWorkflow.Triggers = triggers;
                objWorkflow.AutoExecuteRegularMode = autoExecuteRegularMode;
                objWorkflow.AutoExecuteEditInExcelMode = autoExecuteEditInExcelMode;

                // Save it config
                objWorkflow.SaveWorkflow();
                return true;
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), Utils.GetEnumDescription(ListScreenType.Actionflow, string.Empty));
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetWorkflow()
        {
            if (objWorkflow != null && objWorkflow.Id != Guid.Empty)
            {
                tmpWfId = objWorkflow.Id;
                configurationManager.Workflows.Remove(objWorkflow);

                objWorkflow = new WorkflowStructure();
                objWorkflow.Id = tmpWfId;
            }
            else
                objWorkflow = new WorkflowStructure();

            wfAction = null;
            wfCondition = null;
            wfStep = null;

            stepCounter = 0;
            conditionCounter = 0;
            actionCounter = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RevertWorkflow()
        {
            // Add workflow back in previous state
            if (cloneWorkflow != null)
            {
                WorkflowStructure tmpWorkflow = (WorkflowStructure)configurationManager.GetWorkflowById(Convert.ToString(cloneWorkflow.Id));
                if (tmpWorkflow == null)
                    configurationManager.Workflows.Add(cloneWorkflow);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tnc"></param>
        private bool SaveNodeElement(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write a parent node, then iterrate through the children
                if (node.Nodes.Count > 0)
                {
                    bIsValidWfElement = AddWorkflowElement(node, out wfValidationMessage);
                    if (!bIsValidWfElement)
                        break;
                    SaveNodeElement(node.Nodes);
                    // Since this is recursive function if workflow element has failed validation it will come back here, so return from here as well.
                    if (!bIsValidWfElement)
                        break;
                }
                else //No child nodes, so we just add the workflow element
                {
                    bIsValidWfElement = AddWorkflowElement(node, out wfValidationMessage);
                    if (!bIsValidWfElement)
                        return false;
                }
            }

            return bIsValidWfElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private bool AddWorkflowElement(TreeNode node, out string errorMessage)
        {
            bool isValidWorkflowElement = true;
            errorMessage = string.Empty;

            if (node.Tag.GetType() == typeof(Step))
            {
                wfStep = new Step();
                //wfStep = (Step)node.Tag;
                if (string.IsNullOrEmpty(node.Name))
                    wfStep.Id = Guid.NewGuid();
                else
                    wfStep.Id = Guid.Parse(node.Name);

                stepID = wfStep.Id.ToString();
                wfStep.Name = node.Text;
                wfStep.SequenceNo = ++stepCounter;

                // Validate Step element in workflow
                isValidWorkflowElement = ValidateStep(wfStep, out errorMessage);

                objWorkflow.Steps.Add(wfStep);

                // do reset
                conditionCounter = 0;
                wfCondition = null;
            }
            
            if (node.Tag.GetType() == typeof(Condition))
            {
                wfCondition = new Condition();
                wfCondition.Criteria = node.Text;
                wfCondition.SequenceNo = ++conditionCounter;

                wfStep.Conditions.Add(wfCondition);

                // do reset
                actionCounter = 0;
                wfAction = null;
            }

            if (node.Tag.GetType() == typeof(WorkflowAction) || node.Tag.Equals("MoveStep"))
            {
                // In case of unconditional Actions, do a blank condition
                if (wfCondition == null)
                {
                    wfCondition = new Condition();
                    wfCondition.Criteria = string.Empty;
                    wfCondition.SequenceNo = ++conditionCounter;

                    wfStep.Conditions.Add(wfCondition);

                    // do reset
                    actionCounter = 0;
                    wfAction = null;
                }

                wfAction = new WorkflowAction();
                if (node.Tag.GetType() == typeof(WorkflowAction))
                {
                    WorkflowAction nodeAction = (WorkflowAction)node.Tag;
                    wfAction.WorkflowActionData = nodeAction.WorkflowActionData;
                }
                wfAction.Name = node.Text;
                wfAction.ActionId = node.Name.Replace("Move To:", "");
                wfAction.Type = node.Tag.Equals("MoveStep") ? "MoveStep" : "Action";
                wfAction.SequenceNo = ++actionCounter;

                // Validate Step -> Action in workflow
                isValidWorkflowElement = ValidateAction(wfAction, out errorMessage);

                wfCondition.WorkflowActions.Add(wfAction);
            }

            return isValidWorkflowElement;
        }

        internal bool ValidateStep(Step wfStep, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (wfStep.Name.Equals("Define Step"))
            {
                errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("WORKFLOWCONTROLLER_DEFINESTEP_ErrorMsg"), wfStep.Name, Environment.NewLine, wfStep.Name);
                //errorMessage = "'" + wfStep.Name + "' Step is not configured correctly" + Environment.NewLine + "Please select '" + wfStep.Name + "' from Left Navigation pane and click Apply button.";
                return false;
            }
            return true;
        }

        internal bool ValidateAction(WorkflowAction wfAction, out string errorMessage)
        {
            Guid newGuid;
            errorMessage = string.Empty;

            bool isCPQAction = Enum.IsDefined(typeof(Core.CPQ.CPQBase.CPQAction), wfAction.ActionId);
            // Valid action is not assigned
            if (!isCPQAction && !Guid.TryParse(wfAction.ActionId, out newGuid))
            {
                errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("WORKFLOWCONTROLLER_InvalidAction_ErrorMsg"), wfStep.Name, Environment.NewLine, wfStep.Name);
                //errorMessage = "Action for Step '" + wfStep.Name + "' is not configured correctly" + Environment.NewLine + "Please select Step '" + wfStep.Name + "' from Left Navigation pane and click Apply button.";
                return false;
            }

            // Persist Output data is checked but Data Name is empty
            if (wfAction.WorkflowActionData.OutputPersistData && string.IsNullOrEmpty(wfAction.WorkflowActionData.OutputDataName))
            {
                errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("WORKFLOWCONTROLLER_DataName_ErrorMsg"), wfAction.Name, wfStep.Name, Environment.NewLine, wfStep.Name);
                //errorMessage = "Action Output for '" + wfAction.Name + "' under Step '" + wfStep.Name + "' is not configured correctly" + Environment.NewLine + "Please select Step '" + wfStep.Name + "' from Left Navigation pane and click Apply button.";
                return false;
            }

            int expectedInput = GetListOfActionObject(wfAction.ActionId).Count;
            // Input data is checked but input data name is empty
            if (wfAction.WorkflowActionData.InputData)
            {
                int actualInput = wfAction.WorkflowActionData.InputDataName == null ? 0 : wfAction.WorkflowActionData.InputDataName.Count();
                if (actualInput != expectedInput)
                {
                    errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("WORKFLOWCONTROLLER_EmptyInput_ErrorMsg"), wfAction.Name, wfStep.Name, Environment.NewLine, wfStep.Name);
                    //errorMessage = "Action Input for '" + wfAction.Name + "' under Step '" + wfStep.Name + "' is not configured correctly" + Environment.NewLine + "Please select Step '" + wfStep.Name + "' from Left Navigation pane and click Apply button.";
                    return false;
                }
            }
            else if (expectedInput > 0 && !wfAction.WorkflowActionData.InputData)   // Input is exepcted by Action but provide input is not checked.
            {
                errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("WORKFLOWCONTROLLER_PROVIDEINPUT_ErrorMsg"), wfAction.Name, wfStep.Name, Environment.NewLine, wfStep.Name);
                //errorMessage = "Action '" + wfAction.Name + "' under Step '" + wfStep.Name + "' expects Input Data." + Environment.NewLine + "Please select Step '" + wfStep.Name + "' from Left Navigation pane click Provide Input and Apply button.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetListOfActions()
        {
            lstActions.Clear();
            // Read available Actions from ConfigMgr for current App and populate list.
            lstActions.Add("0", "-- Select --");

            foreach (Core.Action act in configurationManager.Actions)
            {
                lstActions.Add(act.Id, act.Name + " (" + act.Type + ")");
            }
            if (DisplayCPQAction)
                Apttus.XAuthor.Core.CPQ.CPQBase.AddCPQActions(lstActions);

            return lstActions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetListOfSteps(TreeView tv)
        {
            // Clear Steps list
            lstSteps.Clear();
            lstSteps.Add("0", "-- Select --");

            foreach (TreeNode node in tv.Nodes)
            {
                AddStepFromChildNodes(node.Nodes);
            }

            return lstSteps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tnc"></param>
        private void AddStepFromChildNodes(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Tag.GetType() == typeof(Step))
                    {
                        iStepCounter++;
                        lstSteps.Add(iStepCounter.ToString(), node.Text);
                    }
                    AddStepFromChildNodes(node.Nodes);
                }
                else 
                {
                    if (node.Tag.GetType() == typeof(Step))
                    {
                        iStepCounter++;
                        lstSteps.Add(iStepCounter.ToString(), node.Text);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public List<string> GetListOfActionObject(string actionId)
        {
            return configurationManager.GetActionObjectsById(actionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstOutputData"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetWorkflowOutputDataById(TreeView tv, string objectId)
        {
            Dictionary<string, string> objectData = new Dictionary<string, string>();

            // Get workflow Action data of existing workflows
            //lstWFActionData = new List<WorkflowActionData>();
            //lstWFActionData = configurationManager.GetWorkflowActionData();

            // Get workflow Action data of current workflow
            foreach (TreeNode node in tv.Nodes)
            {
                AddActionFromChildNodes(node.Nodes);
            }

            // Get input object API name or ID from 
            string inputObjectId = appDefManager.GetAppObject(Guid.Parse(objectId)).Id;

            // Traverse through WorkflowActionData class and filter out data based on AppObject UniqueID.
            for (int i = 0; i < lstWFActionData.Count; i++)
            {
                if (lstWFActionData[i].AppObjectId != Guid.Empty)
                {
                    ApttusObject appObj = appDefManager.GetAppObject(lstWFActionData[i].AppObjectId);
                    string matchObjectId = appObj != null ? appObj.Id : string.Empty;
                    
                    //if (lstWFActionData[i].OutputPersistData && (lstWFActionData[i].AppObjectId == Guid.Parse(objectId)))
                    if (lstWFActionData[i].OutputPersistData && (string.Equals(inputObjectId, matchObjectId)))
                    {
                        if (!objectData.ContainsKey(lstWFActionData[i].Id.ToString()))
                            objectData.Add(lstWFActionData[i].Id.ToString(), lstWFActionData[i].OutputDataName);
                    }
                }
            }

            return objectData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tnc"></param>
        private void AddActionFromChildNodes(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Tag.GetType() == typeof(WorkflowAction))
                    {
                        WorkflowAction objAction = (WorkflowAction)node.Tag;
                        if (objAction.WorkflowActionData != null && !lstWFActionData.Contains(objAction.WorkflowActionData))
                            lstWFActionData.Add(objAction.WorkflowActionData);
                    }
                    AddActionFromChildNodes(node.Nodes);
                }
                else
                {
                    if (node.Tag.GetType() == typeof(WorkflowAction))
                    {
                        WorkflowAction objAction = (WorkflowAction)node.Tag;
                        if (objAction.WorkflowActionData != null && !lstWFActionData.Contains(objAction.WorkflowActionData))
                            lstWFActionData.Add(objAction.WorkflowActionData);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wfActionDataId"></param>
        public void RemoveWorkflowActionData(WorkflowActionData wfActionData)
        {
            if (lstWFActionData.Contains(wfActionData))
                lstWFActionData.Remove(wfActionData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <param name="wfView"></param>
        internal void Initialize(WorkflowStructure workflow, WorkflowDesigner wfView)
        {
            if (workflow == null && string.IsNullOrEmpty(wfView.workflowId))
            {
                objWorkflow = new WorkflowStructure();
                formOpenMode = FormOpenMode.Create;
                //View.LoadControls(formOpenMode, null);

                wfView.ShowDialog();
            }
            else
            {
                if (string.IsNullOrEmpty(wfView.workflowId))
                    objWorkflow = workflow;
                else
                    objWorkflow = (WorkflowStructure)configurationManager.GetWorkflowById(wfView.workflowId);

                formOpenMode = FormOpenMode.Edit;
                cloneWorkflow = objWorkflow.Clone();
                //WorkflowStructure loadWorkflow = objWorkflow.Clone();

                wfView.LoadWorkflow(formOpenMode, objWorkflow);
                wfView.ShowDialog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wfElement"></param>
        /// <param name="e"></param>
        public bool ValidateWorkflowElement(Control wfElement)
        {
            if (string.IsNullOrEmpty(wfElement.Text) || wfElement.Text.Equals("-- Select --"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputDataName"></param>
        /// <returns></returns>
        public bool IsOutputDataNameExists(string outputDataName)
        {
            if (string.IsNullOrEmpty(outputDataName))
                return false;

            return configurationManager.IsWorkflowOutputDataExists(outputDataName);
        }
        public bool DisplayCPQAction
        {
            get;
            set;
        }
    }

}
