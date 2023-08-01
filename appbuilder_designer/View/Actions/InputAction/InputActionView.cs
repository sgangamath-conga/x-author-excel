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
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class InputActionView : Form
    {
        InputActionController controller;
        InputActionModel Model;
        BindingSource gridSource;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private Predicate<ApttusField> NotSupportedDataTypes = field => field.Datatype != Datatype.Textarea && field.Datatype != Datatype.Rich_Textarea && field.Datatype != Datatype.NotSupported;

        public InputActionView()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            Model = new InputActionModel();
            gridSource = new BindingSource();
        }

        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtVariableName.Text)) //Variable name cannot be empty
                {
                    MessageBox.Show(new Form() { TopMost = true }, resourceManager.GetResource("INPUTACTION_VariableEmpty_ShowMsg"), resourceManager.GetResource("INPUTACTION_VariableEmptyCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActiveControl = txtVariableName;
                    return;
                }
                if (Model.VariableExists(txtVariableName.Text.Trim(), cboInputObject.SelectedItem as ApttusObject, cboInputField.SelectedItem as ApttusField)) //Variable name should be unique;
                {
                    MessageBox.Show(new Form() { TopMost = true }, resourceManager.GetResource("INPUTACTION_VariableExists_ShowMsg"), resourceManager.GetResource("COMMON_Name_Text"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtVariableName.Text = string.Empty;
                    ActiveControl = txtVariableName;
                    return;
                }
                if (Model.InputActionVariables.Count >= 7)
                {
                    MessageBox.Show(new Form() { TopMost = true }, resourceManager.GetResource("INPUTACTION_VariableValid_ShowMsg"), resourceManager.GetResource("INPUTACTION_VariableValidCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }                

                InputActionVariable variable = new InputActionVariable();

                variable.VariableName = txtVariableName.Text.Trim();
                variable.VariableType = rdoUserInput.Checked ? InputVariableType.UserInput : InputVariableType.ExcelCellReference;

                if (cboInputObject.SelectedIndex != -1 && cboInputField.SelectedIndex != -1)
                {
                    ApttusObject obj = (cboInputObject.SelectedItem as ApttusObject);
                    variable.ObjectUniqueId = obj.UniqueId;
                    variable.ObjectName = obj.Name;

                    ApttusField field = (cboInputField.SelectedItem as ApttusField);
                    variable.FieldId = field.Id;
                    variable.FieldName = field.Name;
                }

                if (variable.VariableType == InputVariableType.ExcelCellReference)
                    variable.ExcelCellReference = txtCellRef.Text;

                Model.AddVariable(variable);
                ActiveControl = txtVariableName;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, "Error",this.Handle.ToInt32());
            }
            finally
            {
                ResetControls();
            }
        }

        private void ResetControls()
        {
            txtVariableName.Text = string.Empty;
            rdoExcelReference.Checked = false;
            rdoUserInput.Checked = true;
            cboInputField.SelectedIndex = cboInputObject.SelectedIndex = -1;
        }

        private void LoadObjects()
        {
            cboInputObject.DisplayMember = Constants.NAME_ATTRIBUTE;
            cboInputObject.ValueMember = Constants.UNIQUEID_ATTRIBUTE;

            cboInputObject.DataSource = ApplicationDefinitionManager.GetInstance.GetAllObjects();
        }

        private void InputActionView_Shown(object sender, EventArgs e)
        {
            controller.SetView();
            gridSource.DataSource = Model.InputActionVariables;
            rdoUserInput.Checked = true;
            btnCancel.CausesValidation = false;
            ActiveControl = txtName;
            this.TopMost = true;
        }

        private void cboInputObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboInputObject.SelectedIndex == -1)
                return;

            cboInputField.DisplayMember = Constants.NAME_ATTRIBUTE;
            cboInputField.ValueMember = Constants.ID_ATTRIBUTE;

            Guid appObjId = (cboInputObject.SelectedItem as ApttusObject).UniqueId;
            List<ApttusField> fields = ApplicationDefinitionManager.GetInstance.GetAllObjects().Where(appObj => appObj.UniqueId.Equals(appObjId)).Select(appObj => appObj.Fields).FirstOrDefault();
            cboInputField.DataSource = fields.Where(field => NotSupportedDataTypes(field)).Select(field => field).ToList();
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = !(MessageBox.Show(resourceManager.GetResource("INPUTACTION_DelRec_ShowMsg"), resourceManager.GetResource("INPUTACTION_DelRecCap_ShowMsg"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes);
        }

        internal void SetController(InputActionController inputActionController)
        {
            controller = inputActionController;
        }

        internal void FillObjects()
        {
            LoadObjects();
            ResetControls();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = gridSource;
        }

        internal void UpdateControls(InputActionModel model)
        {
            Model = null;
            Model = model.Clone();
            txtName.Text = Model.Name;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.Controls))
                return;

            Model.Name = txtName.Text;

            controller.Save(Model);

            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
        }

        internal void SetError(Control control, string errorMsg)
        {
            errorProvider1.SetError(control, errorMsg);
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

#region For Modeless Dialog, below 2 events are needed to be handled manually
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //We explicitly dispose the form on Cancel button, because this form is not a modal dialog, it is a modeless dialog to allow selection of 
            //excel cell while the form is open. Hence we need it to dispose the form on cancel button explicitly while a modal dialog will close the form on hitting the cancel button.
            Dispose();
        }

        private void InputActionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            //we need to explicitly set the cancel flag to false because if error provider has set the error, it wont allow the form to close, because it is a modeless dialog. 
            e.Cancel = false;
        }
#endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Excel.Range rg = Globals.ThisAddIn.Application.Selection as Excel.Range;
            if (rg == null)
                return;
            if (rg.Cells.Count > 1)
                return;
            txtCellRef.Text = ExcelHelper.GetAddress(rg);
            btnAddEntry.Enabled = true;
        }

        private void rdoExcelReference_CheckedChanged(object sender, EventArgs e)
        {
            txtCellRef.Text = string.Empty;
            lnkCellSelection.Enabled = rdoExcelReference.Checked;
            btnAddEntry.Enabled = !rdoExcelReference.Checked;
        }

        private void btnDeleteVariable_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(resourceManager.GetResource("INPUTACTION_DelRec_ShowMsg"), resourceManager.GetResource("INPUTACTION_DelRecCap_ShowMsg"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
                }
            }
        }
    }    
}
