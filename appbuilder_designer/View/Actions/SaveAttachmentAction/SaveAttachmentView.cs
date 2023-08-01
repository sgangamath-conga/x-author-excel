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
    public partial class SaveAttachmentView : Form
    {
        ISaveAttachmentController controller;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveAttachmentView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            txtFileName.Text = configurationManager.Definition.Name;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnOK.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            cbCustomSheets.Text = resourceManager.GetResource("SAVEATTACTION_cbCustomSheets_Text");
            
            lblTitle.Text = resourceManager.GetResource("SAVEATTACTION_gbSaveAttachment_Text");
            label1.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            label2.Text = resourceManager.GetResource("COMMON_FileName_Text");
            label3.Text = resourceManager.GetResource("COMMON_Object_Text");
            label4.Text = resourceManager.GetResource("COMMON_Format_Text") + ":";
            label5.Text = resourceManager.GetResource("SAVEATTACTION_label5_Text");
            rbExcel.Text = resourceManager.GetResource("SAVEATTACTION_rbExcel_Text");
            rbPDF.Text = resourceManager.GetResource("SAVEATTACTION_rbPDF_Text");
            rdoAddNew.Text = resourceManager.GetResource("SAVEATTACTION_rdoAddNew_Text");
            rdoOverrite.Text = resourceManager.GetResource("SAVEATTACTION_rdoOverrite_Text");

            rbCellReferenceFileName.Text = resourceManager.GetResource("SAVEATTACTION_rbCellReferenceFileName_Text");
            rbManualFileName.Text = resourceManager.GetResource("SAVEATTACTION_rbManualFileName_Text");
            rbRunTimeFileName.Text = resourceManager.GetResource("SAVEATTACTION_rbRunTimeFileName_Text");
        }

        public void SetController(ISaveAttachmentController controller)
        {
            this.controller = controller;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FileSaveType SaveType = rbRunTimeFileName.Checked ? FileSaveType.Overwrite : FileSaveType.AddNew;
            AttachmentFormat format = rbPDF.Checked ? AttachmentFormat.PDF : AttachmentFormat.Excel;
            List<string> selectedSheets = lbSheets.SelectedItems.OfType<string>().ToList();
            controller.Save(txtActionName.Text, txtFileName.Text, SaveType, cboObject.SelectedItem as ApttusObject, format, cbCustomSheets.Checked, selectedSheets,rbCellReferenceFileName.Checked);
        }

        internal void FillData()
        {
            // Fill Objects combobox
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
            List<ApttusObject> appObjects = appDefManager.GetAllObjects().Where(s => s.ObjectType == ObjectType.Independent).ToList();
            cboObject.DataSource = appObjects;
            cboObject.DisplayMember = "Name";
            cboObject.ValueMember = "UniqueId";

            // Fill Sheets list
            lbSheets.DataSource = ExcelHelper.GetActiveBookSheetNames();

            // Default Overwrite radio button
            rdoOverrite.Checked = true;

            // Default behaviour is not to show custom sheets
            ToggleCustomSheetsView(false);
        }

        internal void UpdateControls(SaveAttachmentModel Model)
        {
            txtActionName.Text = Model.Name;
            cboObject.SelectedValue = Model.AppObjectId;
            rbRunTimeFileName.Checked = Model.SaveType == FileSaveType.Overwrite;
            rbCellReferenceFileName.Checked = Model.IsFileNameUsingCellRef;
            rbManualFileName.Checked = !(rbRunTimeFileName.Checked || rbCellReferenceFileName.Checked);
            rdoOverrite.Checked = Model.SaveType == FileSaveType.Overwrite;
            rdoAddNew.Checked = Model.SaveType == FileSaveType.AddNew;
            rbExcel.Checked = Model.AttachmentFormat == AttachmentFormat.Excel;
            rbPDF.Checked = Model.AttachmentFormat == AttachmentFormat.PDF;
            cbCustomSheets.Checked = Model.HasCustomSheets;

            if (rbCellReferenceFileName.Checked)
            {
                txtFileName.Text = ExcelHelper.GetAddress(ExcelHelper.GetRange(Model.FileName));
            }
            else if (rbManualFileName.Checked)
            {
                txtFileName.Text = Model.FileName;
            }
            
            
            if (Model.HasCustomSheets)
            {
                for (int x = 0; x < lbSheets.Items.Count; x++)
                    lbSheets.SetSelected(x, Model.IncludedSheets.Contains(lbSheets.Items[x].ToString()));
                lbSheets.TopIndex = 0;
                ToggleCustomSheetsView(true);
            }

        }

        private void SaveAttachmentView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        internal void SetError(Control control, string message)
        {
            errorProvider1.SetError(control, message);
        }

        private string GetTaggedName()
        {
            var checkedButton = this.pnlFileName.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);//locate the checked radio button
            if (checkedButton != null)
            {
                var tag = checkedButton.Tag;
                return tag.ToString();
            }
            return string.Empty;
        }

        private bool IsSelected(string tag)
        {
            string selected = GetTaggedName();
            if (selected.Equals(tag))
            {
                return true;
            }
            else
            {
                return false;
            }
        } 

        private void txtFileName_Validating(object sender, CancelEventArgs e)
        {
            if (IsSelected("Manual"))
            {
                controller.ValidateAction(txtFileName, e, resourceManager.GetResource("SAVEATTACHVIEW_txtFileName_Validating_ErrorMsg"));
            }
            else if (IsSelected("CellRef")) //if user chose cell reference
            {
                bool isvalidReference = ExcelHelper.IsValidCellReference(txtFileName.Text);//check if well formed cell ref
                if (!isvalidReference)
                {
                    e.Cancel = true;
                    SetError((Control)sender, resourceManager.GetResource("ADDROWACTVIEW_txtValue_ValidatingInvalidCell_ErrorMsg"));
                }
                else if (txtFileName.Text.IndexOf(@":") > 0) //check if range selected, only want a single cell
                {
                    e.Cancel = true;
                    SetError((Control)sender, resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValues_ErrMsg"));
                }
                else //check if empty
                {
                    controller.ValidateAction(txtActionName, e, resourceManager.GetResource("ADDROWACTVIEW_txtValue_ValidatingInvalidCell_ErrorMsg"));
                }
            }
        }

        public Control.ControlCollection PanelControls
        {
            get
            {
                return pnlMain.Controls;
            }
        }

        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboObject, e, resourceManager.GetResource("SAVEATTACHVIEW_cboObject_Validating_ErrorMsg"));
        }

        private void cbCustomSheets_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCustomSheetsView(cbCustomSheets.Checked);
        }

        private void ToggleCustomSheetsView(bool bShowCustomSheets)
        {
            panel1.Visible = bShowCustomSheets;
            //if (bShowCustomSheets)
            //{
            //    tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Absolute, 160);
            //}
            //else
            //{
            //    tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Absolute, 0);
            //}
        }

        private void rbCellReferenceFileName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCellReferenceFileName.Checked)
            {
                errorProvider1.Clear();
                txtFileName.Enabled = true;
                txtFileName.Clear();
                getCellRefBtn.Enabled = true;
            }
            
            
        }

        private void rbManualFileName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbManualFileName.Checked)
            {
                errorProvider1.Clear();
                txtFileName.Enabled = true;
                txtFileName.Clear();
                getCellRefBtn.Enabled = false;
            }
            
        }

        private void rbRunTimeFileName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRunTimeFileName.Checked)
            {
                errorProvider1.Clear();
                txtFileName.Enabled = false;
                txtFileName.Clear();
                getCellRefBtn.Enabled = false;
            }
        }

        private void getCellRefBtn_Click(object sender, EventArgs e)
        {
            this.txtFileName.Text = ((SaveAttachmentController)controller).GetActiveColReference();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
