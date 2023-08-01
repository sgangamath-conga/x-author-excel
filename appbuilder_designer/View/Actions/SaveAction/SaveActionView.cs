/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class SaveActionView : Form
    {
        ISaveActionController controller;
        ErrorProvider errorProvider = new ErrorProvider();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveActionView()
        {
            InitializeComponent();
            SetCultureData();
            errorProvider.ContainerControl = this;
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("SAVEACTION_gbSaveAction_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            chkEnableRowHighlight.Text = resourceManager.GetResource("APPSETTVIEW_chkEnableRowHL_Text");
            chkEnableCollisionDetection.Text = resourceManager.GetResource("SAVEACTION_chkEnableCollisionDetection_Text");
            chkEnablePartialSave.Text = resourceManager.GetResource("SAVEACTION_chkEnablePartialSave_Text");
            //gbSaveAction.Text = resourceManager.GetResource("SAVEACTION_gbSaveAction_Text");
            lblActionName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblSaveMap.Text = resourceManager.GetResource("COMMON_SaveMap_Text");
        }

        public void SetController(ISaveActionController controller)
        {
            this.controller = controller;
            this.controller.SetView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(gbSaveAction.Controls))
                return;

            if (controller.Save(txtActionName.Text, cmbSaveMap.SelectedItem != null ? ((SaveMap)cmbSaveMap.SelectedItem).Id : Guid.Empty, chkEnableCollisionDetection.Checked, chkEnablePartialSave.Checked, chkEnableRowHighlight.Checked, int.Parse(txtBatchSize.Text)))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            controller.Cancel();
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        public void SetError(Control control, string message)
        {
            //errorProvider1.SetError(control, message);
            errorProvider.SetError(control, message);
        }

        public void SetSaveMapsList(List<SaveMap> list)
        {
            cmbSaveMap.ValueMember = Constants.ID_ATTRIBUTE;
            cmbSaveMap.DisplayMember = Constants.NAME_ATTRIBUTE;
            cmbSaveMap.DataSource = list;
        }

        internal void FillForm(SaveActionModel model)
        {
            txtActionName.Text = model.Name;
            cmbSaveMap.SelectedValue = model.SaveMapId;
            chkEnableCollisionDetection.Checked = model.EnableCollisionDetection;
            chkEnablePartialSave.Checked = model.EnablePartialSave;
            chkEnableRowHighlight.Checked = model.EnableRowHighlightErrors;
            txtBatchSize.Text = model.BatchSize == 0 ? Constants.SAVE_ACTION_BATCH_SIZE.ToString() : model.BatchSize.ToString();
        }

        internal void SetDefaultValues()
        {
            txtBatchSize.Text = Constants.SAVE_ACTION_BATCH_SIZE.ToString();
        }

        private void cmbSaveMap_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cmbSaveMap, e, resourceManager.GetResource("SAVEACTVIEW_cmbSaveMap_Validating_InfoMsg"));
        }

        private void txtBatchSize_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtBatchSize, e, resourceManager.GetResource("SAVEACTVIEW_txtBatchSize_Validating_InfoMsg"));
        }

        private void SaveActionView_Load(object sender, EventArgs e)
        {
            chkEnableCollisionDetection.Enabled = ObjectManager.GetInstance.GetAdapter().HasCollisionDetection();
        }
    }
}
