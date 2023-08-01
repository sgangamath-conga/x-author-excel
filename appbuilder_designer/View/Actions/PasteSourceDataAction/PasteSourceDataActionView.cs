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
    public partial class PasteSourceDataActionView : Form
    {
        IPasteSourceDataActionController controller;
        PasteSourceDataActionModel Model;
        private ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public PasteSourceDataActionView()
        {
            InitializeComponent();
            SetCultureData();

            configManager = ConfigurationManager.GetInstance;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label3.Text = resourceManager.GetResource("COMMON_PasteSourceData_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            chkInputFileName.Text = resourceManager.GetResource("PASTEDATAACTION_chkInputFileName_Text");
            //groupBox1.Text = resourceManager.GetResource("COMMON_PasteSourceData_Text");
            label1.Text = resourceManager.GetResource("COMMON_Name_Text");
            label2.Text = resourceManager.GetResource("PASTEDATAACTION_label2_Text");
        }

        internal void SetController(IPasteSourceDataActionController pasteActionController)
        {
            controller = pasteActionController;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(groupBox1.Controls))
                return;

            Model.Name = txtName.Text;
            Model.InputUserForFileName = chkInputFileName.Checked;

            controller.Save(Model);
        }

        private void PasteActionView_Load(object sender, EventArgs e)
        {
            controller.SetView();
            if (configManager.Application.Definition.Mapping != null)
            {
                ActiveControl = txtName;
                btnSave.Enabled = true;
                txtFileName.Text = configManager.Application.Definition.Mapping.SourceFile;
            }
            else
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("PASTESRCDATAVIEW_PasteActionView_Load_InfoMsg"), resourceManager.GetResource("COMMON_PasteSourceData_Text"), this.Handle.ToInt32());
                Dispose();
            }
        }

        internal void UpdateControls(PasteSourceDataActionModel model)
        {
            Model = model;
            txtName.Text = Model.Name;
            chkInputFileName.Checked = Model.InputUserForFileName;
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        internal void SetError(Control control, string errorMsg)
        {
            errorProvider1.SetError(control, errorMsg);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
