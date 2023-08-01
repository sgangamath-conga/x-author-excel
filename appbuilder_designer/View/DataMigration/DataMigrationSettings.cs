using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataMigrationSettings : Form
    {
        DataMigrationModel Model;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public DataMigrationSettings(DataMigrationModel model)
        {
            Model = model;
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            lblSettings.Text = resourceManager.GetResource("COMMON_Settings_Text");
            lblSettingsDescription.Text = resourceManager.GetResource("DMSettings_lblSettingsDescription_Text");
            lblExportallSuffix.Text = resourceManager.GetResource("DMSettings_lblExportallSuffix_Text");
            lblExportSelectiveSuffix.Text = resourceManager.GetResource("DMSettings_lblExportSelectiveSuffix_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnOK.Text = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Text");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;

            if (ApttusFormValidator.hasValidationErrors(this.Controls))
                return;

            Model.ExportAllSuffix = txtExportAll.Text;
            Model.ExportSelectiveSuffix = txtExportSelective.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void DataMigrationSettings_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Model.ExportAllSuffix))
                txtExportAll.Text = Model.ExportAllSuffix;
            if (!string.IsNullOrEmpty(Model.ExportSelectiveSuffix))
                txtExportSelective.Text = Model.ExportSelectiveSuffix;
        }

        private void txtExportAll_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtExportAll.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtExportAll, resourceManager.GetResource("DMSettings_txtExportAll_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtExportAll, string.Empty);
            }
        }

        private void txtExportSelective_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtExportSelective.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtExportSelective, resourceManager.GetResource("DMSettings_txtExportSelective_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtExportSelective, string.Empty);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
