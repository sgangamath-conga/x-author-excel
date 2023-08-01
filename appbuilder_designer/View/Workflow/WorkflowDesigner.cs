/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.CPQ;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class WorkflowDesigner : Form
    {
        Dictionary<string, string> lstSteps = null;

        static WorkflowController controller;
        FormOpenMode wfDesignerMode = FormOpenMode.Create;
        ErrorProvider errorProvider = new ErrorProvider();
        public string workflowId = string.Empty;
        bool bPersistedDataNameChanged = false;
        TreeNode selectedNode = null;

        //TableLayoutPanel tlpInputObjects = null;
        Dictionary<string, string> lstPersistedData = null;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public void SetController(WorkflowController wfController)
        {
            controller = wfController;
        }

        #region "Constructor"
      
        public WorkflowDesigner(string wfId)
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            this.workflowId = wfId;

            //lstPersistedData = new Dictionary<string, string>();
            
            lstPersistedData = configManager.GetWorkflowOutputData();
        }

        #endregion

        /// <summary>
        /// Set Culture Data
        /// </summary>
        private void SetCultureData()
        {
            addActionToolStripMenuItem.Text = resourceManager.GetResource("COMMON_AddStep_Text");
            addConditionToolStripMenuItem.Text = resourceManager.GetResource("WFDESIGNER_addConditionToolStripMenuItem_Text");
            addStepToolStripMenuItem.Text = resourceManager.GetResource("COMMON_AddStep_Text");
            btnAddStep.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnApplyAction.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnApplyCondition.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnApplyMoveStep.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnSaveWFName.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnSaveWorkflow.Text = resourceManager.GetResource("COMMON_Save_AMPText");
            chkDisplayCPQActions.Text = resourceManager.GetResource("WFDESIGNER_chkDisplayCPQActions_Text");
            chkExternalInput.Text = resourceManager.GetResource("WFDESIGNER_chkExternalInput_Text");
            chkPersistData.Text = resourceManager.GetResource("WFDESIGNER_chkPersistData_Text");
            chkProvideInput.Text = resourceManager.GetResource("WFDESIGNER_chkProvideInput_Text");
            chkToggleAction.Text = resourceManager.GetResource("WFDESIGNER_chkToggleAction_Text");
            chkSuppressSaveMessage.Text = resourceManager.GetResource("WFDESIGNER_chkSuppressSaveMessage_Text");
            condActiontoolStripMenuItem.Text = resourceManager.GetResource("COMMON_AddAction_Text");
            createWorkflowToolStripMenuItem.Text = resourceManager.GetResource("COMMON_CreateActionflow_Text");
            groupActionInput.Text = resourceManager.GetResource("WFDESIGNER_groupActionInput_Text");
            groupActionOutput.Text = resourceManager.GetResource("WFDESIGNER_groupActionOutput_Text");
            groupActions.Text = resourceManager.GetResource("WFDESIGNER_groupActions_Text");
            groupCondition.Text = resourceManager.GetResource("WFDESIGNER_groupCondition_Text");
            groupMoveStep.Text = resourceManager.GetResource("WFDESIGNER_groupMoveStep_Text");
            groupStep.Text = resourceManager.GetResource("WFDESIGNER_groupStep_Text");
            groupWorkflow.Text = resourceManager.GetResource("COMMON_ActionFlow_Text");
            label1.Text = resourceManager.GetResource("WFDESIGNER_label1_Text");
            label2.Text = resourceManager.GetResource("WFDESIGNER_label2_Text");
            label3.Text = resourceManager.GetResource("COMMON_Condition_Text");
            label4.Text = resourceManager.GetResource("WFDESIGNER_label4_Text");
            label6.Text = resourceManager.GetResource("COMMON_Name_Text");
            lblWFName.Text = resourceManager.GetResource("WFDESIGNER_lblWFName_Text");
            remActiontoolStripMenuItem.Text = resourceManager.GetResource("WFDESIGNER_remActiontoolStripMenuItem_Text");
            removeConditionToolStripMenuItem.Text = resourceManager.GetResource("WFDESIGNER_removeConditionToolStripMenuItem_Text");
            removeStepToolStripMenuItem.Text = resourceManager.GetResource("COMMON_StepUp_Text");
            remSteptoolStripMenuItem.Text = resourceManager.GetResource("COMMON_StepUp_Text");
            selActiontoolStripMenuItem.Text = resourceManager.GetResource("COMMON_AddAction_Text");
            stepDownToolStripMenuItem.Text = resourceManager.GetResource("WFDESIGNER_stepDownToolStripMenuItem_Text");
            stepUpToolStripMenuItem.Text = resourceManager.GetResource("WFDESIGNER_stepUpToolStripMenuItem_Text");
        }
        private void WorkflowDesigner_Load(object sender, EventArgs e)
        {
            if (wfDesignerMode == FormOpenMode.Create)
            {
                ShowWorkflowPanel();
                AddWorkflowToTree(resourceManager.GetResource("COMMON_CreateActionflow_Text"));
            }
            PopulateActions();

            chkDisplayCPQActions.Enabled = LicenseActivationManager.GetInstance.IsFeatureAvailable(Constants.CPQ_API_ACTIONFLOW_TAG);
        }

        #region "Form Control events"

        private void btnClose_Click(object sender, EventArgs e)
        {
            controller.RevertWorkflow();
            this.Close();
        }

        private void btnSaveWFName_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.pnlCreateWorkflow.Controls))
                return;

            TreeNode tnWorkflow = tvWorkflow.Nodes["Actionflow"];

            tnWorkflow.Text = txtWFName.Text;
            tvWorkflow.EndUpdate();
        }

        private void btnAddStep_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.pnlCreateStep.Controls))
                return;

            TreeNode tnStep = tvWorkflow.SelectedNode;

            if (tnStep.Tag.GetType() == typeof(Step))
            {
                Step objStep = (Step)tnStep.Tag;

                objStep.Id = objStep.Id != Guid.Empty ? objStep.Id : Guid.NewGuid();
                tnStep.Text = objStep.Name = txtStep.Text;
                tvWorkflow.EndUpdate();
            }
        }

        private void btnApplyCondition_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.pnlMakeCondition.Controls))
                return;

            TreeNode tnCondition = tvWorkflow.SelectedNode;

            if (tnCondition.Tag.GetType() == typeof(Condition))
            {
                Condition objCondition = (Condition)tnCondition.Tag;

                objCondition.Id = objCondition.Id != Guid.Empty ? objCondition.Id : Guid.NewGuid();
                objCondition.Name = txtCondition.Text;
                tnCondition.Text = objCondition.Criteria = txtCondition.Text;
                tvWorkflow.EndUpdate();
                tvWorkflow.SelectedNode = tnCondition;
            }
        }

        private void btnApplyAction_Click(object sender, EventArgs e)
        {
            TreeNode tnAction = tvWorkflow.SelectedNode;

            if (tnAction.Tag.GetType() == typeof(Step))
            {
                Step objStep = (Step)tnAction.Tag;
                btnAddStep_Click(null, null);

                if ((objStep.Conditions.Count > 0 && objStep.Conditions[0].WorkflowActions.Count > 0) && tnAction.Nodes.Count > 0)   // existing step
                {
                    string actionName = objStep.Conditions[0].WorkflowActions[0].ActionId;
                    tnAction = tvWorkflow.Nodes.Find(actionName, true).First();
                }
                else // new Step
                {
                    if (cbActions.SelectedIndex == 0)
                        AddActionToStep(string.Empty, resourceManager.GetResource("COMMON_ApplyAction_text"), string.Empty, new WorkflowAction());
                    else if (tnAction.Nodes.Count > 0)
                        tnAction = tnAction.Nodes[0];
                    else if (tnAction.Nodes.Count == 0) // Get selected action and add action to step
                        tnAction = AddActionToStep(string.Empty, ((KeyValuePair<string, string>)cbActions.SelectedItem).Value, ((KeyValuePair<string, string>)cbActions.SelectedItem).Key, new WorkflowAction());
                }
            }
            else if (tnAction.Tag.GetType() == typeof(WorkflowAction))
            {
                TreeNode tnStep = tnAction.Parent;
                tvWorkflow.SelectedNode = tnStep;
                btnAddStep_Click(null, null);
                tvWorkflow.SelectedNode = tnAction;
            }

            if (ApttusFormValidator.hasValidationErrors(this.pnlSelectAction.Controls))
                return;

            if (chkPersistData.Checked && ApttusFormValidator.hasValidationErrors(this.pnlOutput.Controls))
                return;

            //if (chkProvideInput.Checked && ApttusFormValidator.hasValidationErrors(this.pnlInput.Controls))
            //    return;
            //string actionId = tnAction.Name;

            if (tnAction.Tag.GetType() == typeof(WorkflowAction) && !((KeyValuePair<string, string>)cbActions.SelectedItem).Value.Equals("-- Select --"))
            {
                WorkflowAction objWFAction = (WorkflowAction)tnAction.Tag;
                tnAction.Text = objWFAction.Name = ((KeyValuePair<string, string>)cbActions.SelectedItem).Value;
                tnAction.Name = objWFAction.ActionId = ((KeyValuePair<string, string>)cbActions.SelectedItem).Key;
                objWFAction.Type = "Action";

                tvWorkflow.EndUpdate();
                tvWorkflow.SelectedNode = tnAction;

                if (chkPersistData.Checked || chkProvideInput.Checked)
                {
                    if (objWFAction.WorkflowActionData == null && chkPersistData.Checked)
                    {
                        if (controller.IsOutputDataNameExists(txtPersistDataName.Text))
                        {
                            ApttusMessageUtil.ShowError(resourceManager.GetResource("WORKFLOW_DataName_ErrorMsg"), resourceManager.GetResource("COMMON_Workflow_Text"));
                            return;
                        }
                    }
                    else if (objWFAction.WorkflowActionData != null && chkPersistData.Checked)
                    {
                        if (bPersistedDataNameChanged && (!string.Equals(txtPersistDataName.Text, objWFAction.WorkflowActionData.OutputDataName)))
                        {
                            if (controller.IsOutputDataNameExists(txtPersistDataName.Text))
                            {
                                ApttusMessageUtil.ShowError(resourceManager.GetResource("WORKFLOW_DataName_ErrorMsg"), resourceManager.GetResource("COMMON_Workflow_Text"));
                                return;
                            }
                        }
                    }
                }

                if (objWFAction.WorkflowActionData == null)
                    objWFAction.WorkflowActionData = new WorkflowActionData();

                objWFAction.WorkflowActionData.Id = objWFAction.WorkflowActionData.Id != Guid.Empty ? objWFAction.WorkflowActionData.Id : Guid.NewGuid();

                objWFAction.WorkflowActionData.OutputPersistData = chkPersistData.Checked;
                if (chkPersistData.Checked)
                {
                    objWFAction.WorkflowActionData.OutputDataName = txtPersistDataName.Text;
                    objWFAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(tnAction.Name);
                    objWFAction.WorkflowActionData.OutputIsGlobal = true;

                    if (lstPersistedData.ContainsKey(objWFAction.WorkflowActionData.Id.ToString()))
                        lstPersistedData.Remove(objWFAction.WorkflowActionData.Id.ToString());

                    lstPersistedData.Add(objWFAction.WorkflowActionData.Id.ToString(), txtPersistDataName.Text);
                }
                else
                {
                    objWFAction.WorkflowActionData.OutputDataName = string.Empty;
                    objWFAction.WorkflowActionData.AppObjectId = Guid.Empty;
                    objWFAction.WorkflowActionData.OutputIsGlobal = false;
                }

                objWFAction.WorkflowActionData.InputData = chkProvideInput.Checked;
                objWFAction.WorkflowActionData.InputIsExternal = chkExternalInput.Checked;
                if (chkProvideInput.Checked)
                    objWFAction.WorkflowActionData.InputDataName = GetInputDataList();
                else
                    objWFAction.WorkflowActionData.InputDataName = null;
                objWFAction.WorkflowActionData.SupressSaveMessage = chkSuppressSaveMessage.Checked;
                tnAction.Tag = objWFAction;
            }
        }

        private void chkToggleAction_CheckedChanged(object sender, EventArgs e)
        {
            if (chkToggleAction.Checked)
            {
                EnableMoveToStep();   
            }
            else
            {
                EnableAddAction();
            }
        }

        private void btnApplyMoveStep_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.pnlMoveAction.Controls))
                return;

            TreeNode tnAction = tvWorkflow.SelectedNode;

            if ((tnAction.Tag.GetType() == typeof(WorkflowAction) || tnAction.Tag.Equals("MoveStep")) && !((KeyValuePair<string, string>)cbMoveAction.SelectedItem).Value.Equals("-- Select --"))
            {
                tnAction.Text = "Move To:" + ((KeyValuePair<string, string>)cbMoveAction.SelectedItem).Value;
                tnAction.Tag = "MoveStep";
                tnAction.Name = ((KeyValuePair<string, string>)cbMoveAction.SelectedItem).Key;
                tvWorkflow.EndUpdate();
                tvWorkflow.SelectedNode = tnAction;
            }
        }

        private void tvWorkflow_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            selectedNode = e.Node;

            if (e.Node.Tag.Equals("Actionflow"))
            {
                ShowWorkflowPanel();
                txtWFName.Text = e.Node.Text.Equals(resourceManager.GetResource("COMMON_CreateActionflow_Text")) ? string.Empty : e.Node.Text;
            }
            if (e.Node.Tag.GetType() == typeof(Step))
            {
                ShowStepPanel();
                txtStep.Text = e.Node.Text.Equals(resourceManager.GetResource("WORKFLOWDESFORM_lbl_definestep")) ? string.Empty : e.Node.Text;

                Step objStep = (Step)e.Node.Tag;
                if ((objStep.Conditions.Count > 0 && objStep.Conditions[0].WorkflowActions.Count > 0) && e.Node.GetNodeCount(true) > 0)
                {
                    string actionName = objStep.Conditions[0].WorkflowActions[0].ActionId;
                    TreeNode tnAction = tvWorkflow.Nodes.Find(actionName, true).First();

                    ShowActionWithStep(tnAction, true);
                }
                else if (e.Node.Nodes.Count > 0)
                {
                    TreeNode tnAction = e.Node.Nodes[0];
                    ShowActionWithStep(tnAction, true);
                }
                else if (e.Node.Nodes.Count == 0)
                {
                    cbActions.SelectedIndex = 0;
                    chkPersistData.Checked = false;
                    chkProvideInput.Checked = false;
                }
            }
            if (e.Node.Tag.GetType() == typeof(Condition))
            {
                ShowConditionPanel();
                txtCondition.Text = e.Node.Text.Equals("Define Condition") ? string.Empty : e.Node.Text;
            }
            if (e.Node.Tag.GetType() == typeof(WorkflowAction))
            {
                ShowActionWithStep(e.Node, false);
            }
            if (e.Node.Tag.Equals("MoveStep"))
            {
                ShowActionPanel();

                EnableMoveToStep();
                cbMoveAction.SelectedIndex = cbMoveAction.FindStringExact(e.Node.Text.Replace("Move To:", ""));
            }
        }

        private void ShowActionWithStep(TreeNode tnAction, bool bIsStepVisible)
        {
            ShowActionPanel(bIsStepVisible);

            EnableAddAction();

            if (tnAction.Parent != null && tnAction.Parent.Tag.GetType() == typeof(Step))
                txtStep.Text = ((Step)tnAction.Parent.Tag).Name;

            cbActions.SelectedValue = tnAction.Name;

            if (cbActions.SelectedValue != null)
            {
                WorkflowAction objWfAction = (WorkflowAction)tnAction.Tag;
                if (objWfAction.WorkflowActionData != null)
                {
                    chkPersistData.Checked = objWfAction.WorkflowActionData.OutputPersistData;
                    txtPersistDataName.Text = objWfAction.WorkflowActionData.OutputDataName;
                    tvWorkflow.SelectedNode = tnAction;
                    
                    chkExternalInput.CheckedChanged -= chkExternalInput_CheckedChanged;
                    chkExternalInput.Checked = objWfAction.WorkflowActionData.InputIsExternal;
                    chkExternalInput.CheckedChanged += chkExternalInput_CheckedChanged;
                    chkProvideInput.Checked = objWfAction.WorkflowActionData.InputData;
                    chkSuppressSaveMessage.Checked = objWfAction.WorkflowActionData.SupressSaveMessage;
                    // Here chkProvideInput_CheckedChanged event shall fire and load Input data
                }
            }
        }

        private void chkProvideInput_CheckedChanged(object sender, EventArgs e)
        {
            if (chkProvideInput.Checked)
            {
                if (cbActions.SelectedValue.ToString() == "0")
                {
                    ApttusMessageUtil.ShowWarning(resourceManager.GetResource("WOKRFLOWACT_chkProvideInput_CheckedChanged_WarnMsg"), resourceManager.GetResource("COMMON_Workflow_Text"), ApttusMessageUtil.Ok);
                    chkProvideInput.Checked = false;
                    return;
                }

                pnlInput.Visible = true;
                RenderInputData(chkExternalInput.Checked);
            }
            else
                pnlInput.Visible = false;

            TreeNode currentNode = selectedNode == null ? tvWorkflow.SelectedNode : selectedNode;
            TreeNode wfActionNode = GetActionNode(currentNode);
            if (wfActionNode != null)
            {
                WorkflowAction wfAction = (WorkflowAction)wfActionNode.Tag;
                if (wfAction.WorkflowActionData != null)
                {
                    wfAction.WorkflowActionData.InputData = chkProvideInput.Checked;
                    wfActionNode.Tag = wfAction;
                }
            }
        }

        private void cbActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if Action is deleted from Actions UI, WF throws exception. Handle exception here.
            if (cbActions.SelectedItem == null)
                return;

            string sAction = ((KeyValuePair<string, string>)cbActions.SelectedItem).Value;

            if (string.IsNullOrEmpty(sAction) || sAction.Equals("-- Select --"))
                return;

            int inputObjects = controller.GetListOfActionObject(((KeyValuePair<string, string>)cbActions.SelectedItem).Key).Count;

            int startIndex = ((KeyValuePair<string, string>)cbActions.SelectedItem).Value.IndexOf("(") + 1;
            int endIndex = ((KeyValuePair<string, string>)cbActions.SelectedItem).Value.IndexOf(")");
            string actionType = sAction.Substring(startIndex, endIndex - startIndex);

            ResetInputOutputData();

            switch (actionType)
            {
                case "SearchAndSelect":
                    chkPersistData.Enabled = true;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case "Display":
                case "Retrieve":
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case "ExecuteQuery":
                case "Query":
                    chkPersistData.Enabled = true;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case "SalesforceMethod":
                case "CallProcedure":
                    chkPersistData.Enabled = true;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case "CheckIn":
                case "CheckOut":
                case "SaveAttachment":
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case Constants.ADDROW_ACTION:
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                case "CPQ":
                    // chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = true;
                    chkSuppressSaveMessage.Enabled = false;
                    break; 
                case "Save":
                    chkSuppressSaveMessage.Enabled = true;
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = false;
                    break;
                case Constants.DataSetAction:
                    chkPersistData.Enabled = true;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    break;
                case Constants.DELETE_ACTION:
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = inputObjects > 0 ? true : false;
                    chkSuppressSaveMessage.Enabled = false;
                    break;
                default:
                    chkSuppressSaveMessage.Enabled = false;
                    chkPersistData.Enabled = false;
                    chkProvideInput.Enabled = false;
                    break;
            }

        }

        private void chkPersistData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPersistData.Checked)
            {
                if (cbActions.SelectedValue.ToString() == "0")
                {
                    ApttusMessageUtil.ShowWarning(resourceManager.GetResource("WOKRFLOWACT_chkProvideInput_CheckedChanged_WarnMsg"), "Workflow", ApttusMessageUtil.Ok);
                    chkPersistData.Checked = false;
                    return;
                }

                pnlOutput.Visible = true;
                txtPersistDataName.Text = string.Empty;
            }
            else
                pnlOutput.Visible = false;

            TreeNode currentNode = selectedNode == null ? tvWorkflow.SelectedNode : selectedNode;
            TreeNode wfActionNode = GetActionNode(currentNode);
            if (wfActionNode != null)
            {
                WorkflowAction wfAction = (WorkflowAction)wfActionNode.Tag;
                if (wfAction.WorkflowActionData != null)
                {
                    wfAction.WorkflowActionData.OutputPersistData = chkPersistData.Checked;
                    wfActionNode.Tag = wfAction;
                }
            }
        }

        internal TreeNode GetActionNode(TreeNode selectedNode)
        {
            TreeNode actionNode = null;
            if (selectedNode.Tag.GetType() == typeof(WorkflowAction))
                actionNode = selectedNode;
            else if (selectedNode.Tag.GetType() == typeof(Step))
            {
                if (selectedNode.Nodes.Count > 0 && selectedNode.Nodes[0].Tag.GetType() == typeof(WorkflowAction))
                    actionNode = selectedNode.Nodes[0];
            }
            return actionNode;
        }
        #endregion

        #region "Context menu events"

        private void removeStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        private void removeConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        private void remActiontoolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        private void selActiontoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowActionPanel();
            AddActionToStep(string.Empty, resourceManager.GetResource("COMMON_ApplyAction_text"), string.Empty, new WorkflowAction());
        }

        private void condActiontoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowActionPanel();
            AddActionToStep(string.Empty, resourceManager.GetResource("COMMON_ApplyAction_text"), string.Empty, new WorkflowAction());
        }

        private void addConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConditionPanel();
            AddConditionToStep(string.Empty, "Define Condition", new Condition());
        }

        private void addActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStepPanel();
            AddStepToWorkflow(resourceManager.GetResource("WORKFLOWDESFORM_lbl_definestep"), new Step());
        }

        #endregion

        #region "Menustrip events"

        private void createWorkflowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvWorkflow.Nodes.Count == 0)
            {
                AddWorkflowToTree("Create Workflow");
            }
        }

        private void addStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStepPanel();
            AddStepToWorkflow(resourceManager.GetResource("WORKFLOWDESFORM_lbl_definestep"), new Step());
        }

        #endregion

        #region "UI validate methods"

        private void AddWorkflowToTree(string wfName)
        {
            TreeNode tnWorkflow = new TreeNode(wfName);
            tnWorkflow.Name = "Actionflow";
            tnWorkflow.Tag = "Actionflow";
            tnWorkflow.ContextMenuStrip = contextMenuForWorkflow;

            tvWorkflow.Nodes.Add(tnWorkflow);
            txtWFName.Text = wfName.Equals(resourceManager.GetResource("COMMON_CreateActionflow_Text")) ? string.Empty : wfName;
        }

        private void AddStepToWorkflow(string stepName, Step objStep, string stepId = "")
        {
            TreeNode addStep = new TreeNode(stepName);
            addStep.Name = string.IsNullOrEmpty(stepId) ? Guid.NewGuid().ToString() : stepId;
            addStep.Tag = objStep;
            addStep.ContextMenuStrip = contextMenuForStep;

            TreeNode tnWorkflow = tvWorkflow.Nodes["Actionflow"];
            tnWorkflow.Nodes.Add(addStep);

            tvWorkflow.ExpandAll();
            tvWorkflow.EndUpdate();
            if (stepName.Equals(resourceManager.GetResource("WORKFLOWDESFORM_lbl_definestep")))
            {
                tvWorkflow.SelectedNode = selectedNode = addStep;
                txtStep.Text = string.Empty;
                if (cbActions.Items.Count > 0)
                    cbActions.SelectedIndex = 0;

                UnSubscribeWFDataEvents();

                chkPersistData.Checked = false;
                chkProvideInput.Checked = false;
                chkPersistData.Enabled = true;
                chkProvideInput.Enabled = true;

                SubscribeWFDataEvents();
            }
            else
                txtStep.Text = stepName;
        }

        private void AddConditionToStep(string stepName, string condName, Condition objCondition)
        {
            TreeNode addCondition = new TreeNode(condName);
            addCondition.Name = condName;
            addCondition.Tag = objCondition;
            addCondition.ContextMenuStrip = contextMenuForCondition;

            TreeNode tnStep = null;

            if (string.IsNullOrEmpty(stepName))
                tnStep = tvWorkflow.SelectedNode;
            else
                tnStep = tvWorkflow.Nodes.Find(stepName, true).First();

            if (tnStep.Tag.GetType() == typeof(Step))
            {
                tnStep.Nodes.Add(addCondition);

                tvWorkflow.ExpandAll();
                tvWorkflow.EndUpdate();

                if (condName.Equals("Define Condition"))
                {
                    tvWorkflow.SelectedNode = addCondition;
                    txtCondition.Text = string.Empty;
                }
                else
                    txtCondition.Text = condName;
            }
        }

        private TreeNode AddActionToStep(string stepName, string actionName, string actionId, WorkflowAction objAction)
        {
            TreeNode addAction = new TreeNode(actionName);
            addAction.Name = string.IsNullOrEmpty(actionId) ? actionName : actionId;
            addAction.Tag = objAction;
            addAction.ContextMenuStrip = contextMenuForAction;

            TreeNode tnStep = null;
            if (string.IsNullOrEmpty(stepName))
                tnStep = tvWorkflow.SelectedNode;
            else
                tnStep = tvWorkflow.Nodes.Find(stepName, true).First();

            if (tnStep.Tag.GetType() == typeof(Step) || tnStep.Tag.GetType() == typeof(Condition))
            {
                tnStep.Nodes.Add(addAction);

                tvWorkflow.ExpandAll();
                tvWorkflow.EndUpdate();

                if (actionName.Equals(resourceManager.GetResource("COMMON_ApplyAction_text")))
                {
                    cbActions.SelectedValue = "0";
                    cbMoveAction.SelectedValue = "0";
                    tvWorkflow.SelectedNode = addAction;
                }
            }
            return addAction;
        }

        private void AddMoveStepToStep(string stepName, string actionName, string actionId)
        {
            TreeNode addAction = new TreeNode(actionName);
            addAction.Name = string.IsNullOrEmpty(actionId) ? actionName : actionId;
            addAction.Tag = "MoveStep";
            addAction.ContextMenuStrip = contextMenuForAction;

            TreeNode tnStep = null;
            if (string.IsNullOrEmpty(stepName))
                tnStep = tvWorkflow.SelectedNode;
            else
                tnStep = tvWorkflow.Nodes.Find(stepName, true).First();

            if (tnStep.Tag.GetType() == typeof(Step) || tnStep.Tag.GetType() == typeof(Condition))
            {
                tnStep.Nodes.Add(addAction);

                tvWorkflow.ExpandAll();
                tvWorkflow.EndUpdate();
            }
        }

        private void ShowWorkflowPanel()
        {
            pnlCreateWorkflow.Visible = true;
            pnlCreateStep.Visible = false;
            pnlMakeCondition.Visible = false;
            pnlSelectAction.Visible = false;
            pnlToggleStep.Visible = false;
            pnlMoveAction.Visible = false;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;
            pnlApplyStepAction.Visible = false;
        }

        private void ShowStepPanel()
        {
            pnlCreateWorkflow.Visible = false;
            pnlCreateStep.Visible = true;
            pnlMakeCondition.Visible = false;
            // Show Step & Action combined
            pnlSelectAction.Visible = true;
            pnlApplyStepAction.Visible = true;
            pnlToggleStep.Visible = false;
            pnlMoveAction.Visible = false;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;
        }

        private void ShowConditionPanel()
        {
            pnlCreateWorkflow.Visible = false;
            pnlCreateStep.Visible = false;
            pnlMakeCondition.Visible = true;
            pnlSelectAction.Visible = false;
            pnlToggleStep.Visible = false;
            pnlMoveAction.Visible = false;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;
        }

        private void ShowActionPanel(bool bIsStepVisible = false)
        {
            pnlCreateWorkflow.Visible = false;
            pnlCreateStep.Visible = true;
            pnlMakeCondition.Visible = false;
            pnlSelectAction.Visible = true;
            pnlApplyStepAction.Visible = true;
            pnlToggleStep.Visible = true;
            pnlMoveAction.Visible = false;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;

            UnSubscribeWFDataEvents();

            chkPersistData.Checked = false;
            chkProvideInput.Checked = false;

            SubscribeWFDataEvents();
        }

        private void EnableMoveToStep()
        {
            PopulateSteps();

            chkToggleAction.Text = "OR Select Action";
            pnlSelectAction.Visible = false;
            pnlMoveAction.Visible = true;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;
            pnlApplyStepAction.Visible = false;
        }

        private void EnableAddAction()
        {
            chkToggleAction.Text = "OR Move to Step";
            pnlSelectAction.Visible = true;
            pnlMoveAction.Visible = false;
            pnlOutput.Visible = chkPersistData.Checked;
            pnlInput.Visible = chkProvideInput.Checked;
        }

        private void ResetInputOutputData()
        {
            UnSubscribeWFDataEvents();
            chkProvideInput.Checked = false;
            chkPersistData.Checked = false;
            pnlOutput.Visible = false;
            pnlInput.Visible = false;
            SubscribeWFDataEvents();
        }

        private void RemoveSelectedNode()
        {
            TreeNode tnSelectedNode = tvWorkflow.SelectedNode;
            WorkflowAction objAction = null;

            if (tnSelectedNode.Tag.GetType() != typeof(Step))
                return;

            if (tnSelectedNode.Tag.GetType() == typeof(WorkflowAction))
                objAction = (WorkflowAction)tnSelectedNode.Tag;
            else
            {
                // Find Action node in treeview
                TreeNode tnAction = tnSelectedNode.Nodes.Cast<TreeNode>()
                                    .Where(r => r.Tag.GetType() == typeof(WorkflowAction))
                                    .FirstOrDefault();

                if (tnAction != null)
                    objAction = (WorkflowAction)tnAction.Tag;
            }

            if (objAction != null && objAction.WorkflowActionData != null && objAction.WorkflowActionData.Id != Guid.Empty) 
            {
                // Remove list of persisted data
                if (lstPersistedData.ContainsKey(objAction.WorkflowActionData.Id.ToString()))
                {
                    lstPersistedData.Remove(objAction.WorkflowActionData.Id.ToString());
                    controller.RemoveWorkflowActionData(objAction.WorkflowActionData);
                }
            }

            // Remove tree node
            tvWorkflow.Nodes.Remove(tnSelectedNode);

            // After step is removed refresh right hand side pane, with currently selected node.
            selectedNode = tvWorkflow.SelectedNode;
            TreeNodeMouseClickEventArgs e = new TreeNodeMouseClickEventArgs(selectedNode, System.Windows.Forms.MouseButtons.Left,0, 0, 0);
            tvWorkflow_NodeMouseClick(null, e);
            // Move selection back to Step node, to allow for continue step removal
            tvWorkflow.SelectedNode = selectedNode;
        }

        internal void UnSubscribeWFDataEvents()
        {
            chkPersistData.CheckedChanged -= chkPersistData_CheckedChanged;
            chkProvideInput.CheckedChanged -= chkProvideInput_CheckedChanged;
        }

        internal void SubscribeWFDataEvents()
        {
            chkPersistData.CheckedChanged += chkPersistData_CheckedChanged;
            chkProvideInput.CheckedChanged += chkProvideInput_CheckedChanged;
        }

        #endregion

        #region "Populate data"

        private void PopulateActions()
        {
            
            Dictionary<string, string> lstActions = controller.GetListOfActions();

            cbActions.SelectedIndexChanged -= cbActions_SelectedIndexChanged; 

            cbActions.DataSource = new BindingSource(lstActions, null);
            cbActions.DisplayMember = "Value";
            cbActions.ValueMember = "Key";

            cbActions.SelectedIndexChanged += cbActions_SelectedIndexChanged;
            cbActions.DropDown += new EventHandler(AdjustWidth_DropDown);
        }

        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is KeyValuePair<string, string>)
                    newWidth = (int)g.MeasureString(((KeyValuePair<string, string>)(currentItem)).Value, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth + 16;
            }

            senderComboBox.DropDownWidth = width;
        }

        private void PopulateSteps()
        {
            lstSteps = controller.GetListOfSteps(tvWorkflow);

            cbMoveAction.DataSource = new BindingSource(lstSteps, null);
            cbMoveAction.DisplayMember = "Value";
            cbMoveAction.ValueMember = "Key";
        }

        #endregion

        #region "Workflow Operations"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveWorkflow_Click(object sender, EventArgs e)
        {
            if (controller.GetListOfSteps(tvWorkflow).Count == 1)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("WORKFLOWDESFORM_SaveEmpty_ErrorMsg"), resourceManager.GetResource("COMMON_ActionFlow_Text"));
                return;
            }

            if (controller.SaveWorkflowToConfig(tvWorkflow))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("WORKFLOWDESFORM_btnSaveWorkflow_ClickEMPTY_InfoMsg"), resourceManager.GetResource("COMMON_ActionFlow_Text"));
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formOpenMode"></param>
        /// <param name="objWorkflow"></param>
        public void LoadWorkflow(FormOpenMode formOpenMode, WorkflowStructure objWorkflow)
        {
            wfDesignerMode = formOpenMode;
            bool CPQActionsIncluded = false;
            if (objWorkflow.Id != Guid.Empty)
            {
                AddWorkflowToTree(objWorkflow.Name);

                for (int stCnt = 0; stCnt < objWorkflow.Steps.Count; stCnt++)
                {
                    AddStepToWorkflow(objWorkflow.Steps[stCnt].Name, objWorkflow.Steps[stCnt], objWorkflow.Steps[stCnt].Id.ToString());

                    for (int condCnt = 0; condCnt < objWorkflow.Steps[stCnt].Conditions.Count; condCnt++)
                    {
                        // If condition criteria is blank, mean this unconditional Action
                        if (!string.IsNullOrEmpty(objWorkflow.Steps[stCnt].Conditions[condCnt].Criteria))
                            AddConditionToStep(objWorkflow.Steps[stCnt].Name, objWorkflow.Steps[stCnt].Conditions[condCnt].Criteria, objWorkflow.Steps[stCnt].Conditions[condCnt]);

                        for (int actionCnt = 0; actionCnt < objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions.Count; actionCnt++)
                        {
                            string predeName = string.IsNullOrEmpty(objWorkflow.Steps[stCnt].Conditions[condCnt].Criteria) ? objWorkflow.Steps[stCnt].Id.ToString() : objWorkflow.Steps[stCnt].Conditions[condCnt].Criteria;

                            // Handle Workflow Action 
                            if (objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Type.Equals("Action"))
                            {
                                if (!CPQActionsIncluded && objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId.StartsWith(CPQBase.CPQID))
                                {
                                    CPQActionsIncluded = true;
                                    controller.DisplayCPQAction = true;
                                    this.chkDisplayCPQActions.Checked = true;
                                }
                                AddActionToStep(predeName, objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Name, objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId, objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt]);

                            }
                            // Handle Move to Step scenario
                            if (objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Type.Equals("MoveStep"))
                                AddMoveStepToStep(predeName, objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Name, objWorkflow.Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId);

                        }
                    }
                }
            }

            ShowWorkflowPanel();
        }

        #endregion

        #region "Validate Workflow Elements"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="message"></param>
        public void SetError(Control control, string message)
        {
            errorProvider.SetError(control, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        private void DisplayError(Control control, CancelEventArgs e, string message)
        {
            if (controller.ValidateWorkflowElement(control))
            {
                SetError(control, string.Empty);
            }
            else
            {
                SetError(control, message);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidateControls()
        {
            return ApttusFormValidator.hasValidationErrors(this.Controls);
        }

        private void txtWFName_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(txtWFName, e, resourceManager.GetResource("WORKFLOWDESFORM_txtWFName_Validating_ErrorMsg"));
        }

        private void txtStep_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(txtStep, e, resourceManager.GetResource("WORKFLOWDESFORM_txtStep_Validating_ErrorMsg"));
        }

        private void txtCondition_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(txtCondition, e, resourceManager.GetResource("WORKFLOWDESFORM_txtCondition_Validating_ErrorMsg"));
        }

        private void cbActions_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(cbActions, e, resourceManager.GetResource("WORKFLOWDESFORM_cbActions_Validating_ErrorMsg"));
        }

        private void cbMoveAction_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(cbMoveAction, e, resourceManager.GetResource("WORKFLOWDESFORM_cbMoveAction_Validating_ErrorMsg"));
        }

        private void txtPersistDataName_Validating(object sender, CancelEventArgs e)
        {
            DisplayError(txtPersistDataName, e, resourceManager.GetResource("WORKFLOWDESFORM_txtPersistDataName_Validating_ErrorMsg"));
        }

        #endregion

        #region "Workflow IO helper methods"

        /// <summary>
        /// 
        /// </summary>
        internal void RenderInputData(bool isInputExternal = false)
        {
            ResetTableLayoutForInput();
            pnlInput.Height = 65;
            groupActionInput.Height = 64;
            pnlInputObjects.Height = 27;

            List<string> lstActionObjects = controller.GetListOfActionObject(cbActions.SelectedValue.ToString());

            // get treenode of action and then it's step, then get wfIO , get inputdata array and assign
            // TODO::  check error in edit mode to resolve why correct data is not loaded
            TreeNode tnAction = tvWorkflow.SelectedNode;
            WorkflowAction objAction = null;

            if (tnAction.Tag.GetType() == typeof(Step))
            {
                Step objStep = (Step)tnAction.Tag;
                if ((objStep.Conditions.Count > 0 && objStep.Conditions[0].WorkflowActions.Count > 0) & tnAction.Nodes.Count > 0)
                {
                    string actionName = objStep.Conditions[0].WorkflowActions[0].ActionId;
                    tnAction = tvWorkflow.Nodes.Find(actionName, true).First();

                    objAction = (WorkflowAction)tnAction.Tag;
                }
            }
            else if (tnAction.Tag.GetType() == typeof(WorkflowAction))
                objAction = (WorkflowAction)tnAction.Tag;

            for (int i = 0; i < lstActionObjects.Count; i++)
            {
                Label lblData = new Label();
                lblData.Dock = DockStyle.Fill;
                lblData.Margin = new Padding(3, 3, 3, 3);
                lblData.Anchor = AnchorStyles.Left;
                lblData.TextAlign = ContentAlignment.MiddleRight;
                lblData.AutoSize = true;
                lblData.Name = lstActionObjects[i] + i.ToString();
                lblData.Text = appDefManager.GetObjectNameById(Guid.Parse(lstActionObjects[i])) + ":";
                lblData.Font = new System.Drawing.Font("Segoe UI", 9F);

                ComboBox cmbPersistedData = new ComboBox();
                TextBox txtExternalData = new TextBox();

                if (isInputExternal)
                {
                    txtExternalData.Name = "txtExternalData" + i.ToString();
                    txtExternalData.Dock = DockStyle.Fill;
                    txtExternalData.Font = new System.Drawing.Font("Segoe UI", 9F);
                    txtExternalData.MaximumSize = new Size(200, 25);
                }
                else
                {
                    cmbPersistedData.Name = "cbIOData" + i.ToString();
                    cmbPersistedData.Dock = DockStyle.Fill;
                    cmbPersistedData.FlatStyle = FlatStyle.Standard;
                    cmbPersistedData.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbPersistedData.Font = new System.Drawing.Font("Segoe UI", 9F);
                    cmbPersistedData.MaximumSize = new Size(200, 25);
                }

                //cmbPersistedData.CreateControl();
                if (lstPersistedData!= null && lstPersistedData.Count > 0)
                {
                    Dictionary<string, string> objectData = controller.GetWorkflowOutputDataById(tvWorkflow, lstActionObjects[i]);

                    if (!isInputExternal && objectData.Count > 0)
                    {
                        cmbPersistedData.DataSource = new BindingSource(objectData, null);
                        cmbPersistedData.DisplayMember = "Value";
                        cmbPersistedData.ValueMember = "Key";
                    }
                }

                tlpInputObjects.Controls.Add(lblData, 0, i);  // add label in column0
                if (isInputExternal)
                    tlpInputObjects.Controls.Add(txtExternalData, 1, i);
                else
                    tlpInputObjects.Controls.Add(cmbPersistedData, 1, i);  // add textbox in column1

                tlpInputObjects.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                if (objAction != null && objAction.WorkflowActionData != null && objAction.WorkflowActionData.InputDataName != null && (i < objAction.WorkflowActionData.InputDataName.Length))
                {
                    if (objAction.WorkflowActionData.InputDataName.Length > 0 && !string.IsNullOrEmpty(objAction.WorkflowActionData.InputDataName[i]))
                    {
                        if (isInputExternal)
                        {
                            Guid newGuid;
                            txtExternalData.Text = Guid.TryParse(objAction.WorkflowActionData.InputDataName[i], out newGuid) ? string.Empty : objAction.WorkflowActionData.InputDataName[i];
                        }
                        else
                            cmbPersistedData.SelectedValue = objAction.WorkflowActionData.InputDataName[i];
                    }
                }
            }

            if (isInputExternal || lstActionObjects.Count > 1)
            {
                if (lstActionObjects.Count >= 8)
                {
                    pnlInput.Height = 365;
                    groupActionInput.Height = 360;
                    pnlInputObjects.Height = 350;
                    tlpInputObjects.AutoScroll = true;
                }
                else
                {
                    pnlInput.Height = pnlInput.Height + (27 * lstActionObjects.Count);
                    groupActionInput.Height = groupActionInput.Height + (26 * lstActionObjects.Count);
                    pnlInputObjects.Height = pnlInputObjects.Height + (26 * lstActionObjects.Count);
                    tlpInputObjects.AutoScroll = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetTableLayoutForInput()
        {
            tlpInputObjects.RowStyles.Clear();
            tlpInputObjects.Controls.Clear();
            tlpInputObjects.RowCount = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string[] GetInputDataList()
        {
            List<string> sIOData = new List<string>();

            // TODO:: check why correct selected value of comboxbox is not returned by ctl.Text
            foreach (Control ctl in tlpInputObjects.Controls)
            {
                if (ctl.GetType() == typeof(ComboBox))
                    sIOData.Add(Convert.ToString(((ComboBox)ctl).SelectedValue));
                if (ctl.GetType() == typeof(TextBox))
                    sIOData.Add(((TextBox)ctl).Text.ToString());
            }

            return sIOData.ToArray();
        }

        private void MoveUp(TreeNode node)
        {
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);

                    // bw : add this line to restore the originally selected node as selected
                    node.TreeView.SelectedNode = node;
                }
            }
        }

        private void MoveDown(TreeNode node)
        {
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);

                    // bw : add this line to restore the originally selected node as selected
                    node.TreeView.SelectedNode = node;
                }
            }
        }

        #endregion

        private void txtPersistDataName_TextChanged(object sender, EventArgs e)
        {
            bPersistedDataNameChanged = true;
        }

        private void stepUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnStep = tvWorkflow.SelectedNode;

            if (tnStep.Tag.GetType() == typeof(Step))
                MoveUp(tnStep);
        }

        private void stepDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tnStep = tvWorkflow.SelectedNode;

            if (tnStep.Tag.GetType() == typeof(Step))
                MoveDown(tnStep);
        }

        private void remSteptoolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedNode();
        }

        private void chkExternalInput_CheckedChanged(object sender, EventArgs e)
        {
            RenderInputData(chkExternalInput.Checked);
        }

        private void chkDisplayCPQActions_CheckedChanged(object sender, EventArgs e)
        {
            controller.DisplayCPQAction = chkDisplayCPQActions.Checked;
            PopulateActions();
        }

        private void WorkflowDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.RevertWorkflow();
        }

        private void chkSuppressSaveMessage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSuppressSaveMessage.Checked)
            {
                if (cbActions.SelectedValue.ToString() == "0")
                {
                    ApttusMessageUtil.ShowWarning(resourceManager.GetResource("WOKRFLOWACT_chkProvideInput_CheckedChanged_WarnMsg"), resourceManager.GetResource("COMMON_Workflow_Text"), ApttusMessageUtil.Ok);
                    chkSuppressSaveMessage.Checked = false;
                    return;
                }
            }
        }


    }
}
