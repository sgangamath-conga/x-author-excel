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
using Apttus.XAuthor.Core;


namespace Apttus.XAuthor.AppDesigner
{
    public partial class MacroActionView : Form
    {
        IMacroActionController controller;
        ErrorProvider errorProvider = new ErrorProvider();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public MacroActionView()
        {
            InitializeComponent();
            SetCultureData();

            errorProvider.ContainerControl = this;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("MACROACTION_groupBox1_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            lblActionName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblMacroName.Text = resourceManager.GetResource("MACROACTION_lblMacroName_Text");
            chkMacroOutput.Text = resourceManager.GetResource("MACROACTION_chkMacroOutput_Text");
            chkDisableExcelEvents.Text = resourceManager.GetResource("MACROACTION_chkDisableExcelEvents_Text");
            lblTip.Text = resourceManager.GetResource("MACROACTION_lblTip_Text");
        }

        public void SetController(IMacroActionController controller)
        {
            this.controller = controller;
            this.controller.SetView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(groupBox1.Controls))
                return;

            controller.Save(txtActionName.Text, Convert.ToString(cmbMacro.SelectedValue), chkMacroOutput.Checked, chkDisableExcelEvents.Checked);
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

        /// <summary>
        /// Set Macro list
        /// </summary>
        public void SetMacroList()
        {
            try
            {
                cmbMacro.ValueMember = Constants.ID_ATTRIBUTE;
                cmbMacro.DisplayMember = Constants.NAME_ATTRIBUTE;
                cmbMacro.DataSource = ExcelHelper.GetMacroList();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Error", this.Handle.ToInt32());
            }
        }
        
        internal void FillForm(MacroActionModel model)
        {
            txtActionName.Text = model.Name;            
            cmbMacro.SelectedIndex = cmbMacro.FindStringExact(model.MacroName);
            chkMacroOutput.Checked = model.MacroOuput;
            chkDisableExcelEvents.Checked = model.ExcelEventsDisabled;
        }

        private void cmbMacro_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cmbMacro, e, resourceManager.GetResource("CONST_MACRONAMEREQUIRED"));
        }
    }
}
