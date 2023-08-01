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
    public partial class CheckInView : Form
    {
        ICheckInController controller;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public Control.ControlCollection PanelControls
        {
            get
            {
                return controlPanel.Controls;
            }
        }

        public CheckInView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            txtFileName.Text = configurationManager.Definition.Name;
        }


        private void SetCultureData()
        {            
            label4.Text = resourceManager.GetResource("CHECKINACTION_gbCheckIn_Text");
            cellReferenceBtn.Text = resourceManager.GetResource("CHECKINACTION_CellRef_Text");
            manualNameBtn.Text = resourceManager.GetResource("CHECKINACTION_ManualName_Text");
            runtimeFileNameBtn.Text = resourceManager.GetResource("CHECKINACTION_RuntimeFileName_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnOK.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //gbCheckIn.Text = resourceManager.GetResource("CHECKINACTION_gbCheckIn_Text");
            label1.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            label2.Text = resourceManager.GetResource("COMMON_FileName_Text");
            label3.Text = resourceManager.GetResource("COMMON_Object_Text");
        }

        public void SetController(ICheckInController controller)
        {
            this.controller = controller;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string cellRef = "";
            string tag = GetTaggedName();
            string fileName = "";
            if(tag.Equals("Manual"))//use provided file name
            {
                fileName = txtFileName.Text;
            }
            else if (tag.Equals("CellRef"))//save cell reference to be used in runtime.
            {
                cellRef = txtFileName.Text;
            }
            //else use filename, do nothing
            
            controller.Save(txtActionName.Text, fileName, cellRef, cboObject.SelectedItem as ApttusObject);
        }

        internal void FillObjects()
        {
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
            List<ApttusObject> appObjects = appDefManager.GetAllObjects().Where(s => s.ObjectType == ObjectType.Independent).ToList();
            cboObject.DataSource = appObjects;
            cboObject.DisplayMember = "Name";
            cboObject.ValueMember = "UniqueId";            
        }

        internal void UpdateControls(CheckInModel Model)
        {
            txtActionName.Text = Model.Name;
            if(!string.IsNullOrEmpty(Model.FileName)) //if filename is set
            {
                manualNameBtn.Checked = true;
                txtFileName.Enabled = true;
                txtFileName.Text = Model.FileName;
            }            
            else if(!string.IsNullOrEmpty(Model.namedRange)) //if using cell reference
            {
                txtFileName.Enabled = true;
                var range = ExcelHelper.GetRange(Model.namedRange);
                string cellRef = range.Worksheet.Name + "!" + range.get_Address();
                txtFileName.Text = cellRef;
                cellReferenceBtn.Checked = true;
            }            
            cboObject.SelectedValue = Model.AppObjectId;        
        }

        private void CheckInView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        internal void SetError(Control control, string message)
        {
            errorProvider1.SetError(control, message);
        }

        private string GetTaggedName()
        {
            var checkedButton = this.controlPanel.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);//locate the checked radio button
            var tag = checkedButton.Tag;
            return tag.ToString();
        }

        private bool IsSelected(string tag)
        {
            string selected = GetTaggedName();
            if(selected.Equals(tag))
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {   
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));                            
        }

        private void txtFileName_Validating(object sender, CancelEventArgs e)
        {
            if (IsSelected("Manual"))
            {
                controller.ValidateAction(txtFileName, e, resourceManager.GetResource("SAVEATTACHVIEW_txtFileName_Validating_ErrorMsg"));
            }
            else if(IsSelected("CellRef")) //if user chose cell reference
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

        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboObject, e, resourceManager.GetResource("SAVEATTACHVIEW_cboObject_Validating_ErrorMsg"));
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gbCheckIn_Enter(object sender, EventArgs e)
        {

        }

        private void txtActionName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void cellReferenceBtn_CheckedChanged(object sender, EventArgs e)
        {            
            this.getCellRefBtn.Enabled = true;
            this.txtFileName.Enabled = true;
        }

        private void runtimeFileNameBtn_CheckedChanged(object sender, EventArgs e)
        {            
            this.txtFileName.Enabled = false;
            this.getCellRefBtn.Enabled = false;
        }

        private void manualNameBtn_CheckedChanged(object sender, EventArgs e)
        {
            this.txtFileName.Enabled = true;
            this.getCellRefBtn.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            this.txtFileName.Text = ((CheckInController)controller).GetActiveColReference();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
