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
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class NewApplication : Form
    {
        private string FilePath;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public NewApplication()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;

            rbNew.Checked = true;
        }

        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("NEWAPP_Title");
            btnBrowse.Text = resourceManager.GetResource("NEWAPP_btnBrowse_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnCreate.Text = resourceManager.GetResource("QAWIZARD_btnNext_Text"); // btnCreate.Text = resourceManager.GetResource("COMMON_Create_AMPText");
            groupBox1.Text = resourceManager.GetResource("NEWAPP_groupBox1_Text");
            label1.Text = resourceManager.GetResource("NEWAPP_label1_Text");
            rbExisting.Text = resourceManager.GetResource("NEWAPP_rbExisting_Text");
            rbNew.Text = resourceManager.GetResource("COMMON_New_Text");
        }
        private void txtAppName_TextChanged(object sender, EventArgs e)
        {
            if (txtAppName.Text.Length > 0)
                btnCreate.Enabled = true;
            else
                btnCreate.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // 1. In case of existing template, file is specified
            if (rbExisting.Checked && String.IsNullOrEmpty(FilePath))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("NEWAPPFORM_btnCreate_Click_InfoMsg"), resourceManager.GetResource("NEWAPPFORM_btnCreate_ClickCAP_InfoMsg"));
                return;
            }
            string AppName = null;
            bool NeworExisting = false;

            try
            {

                AppName = txtAppName.Text;
                NeworExisting = rbNew.Checked;

                ApplicationHelper.GetInstance.EditionType = Constants.ADMIN_EDITION;
                ApplicationHelper.GetInstance.CreateApplication(AppName, FilePath, NeworExisting);
                Dispose();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, "Application Create : Error");
                return;
            }


        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                FilePath = openFileDialog1.FileName;
            else
                FilePath = null;
        }

        private void rbExisting_CheckedChanged(object sender, EventArgs e)
        {
            btnBrowse.Enabled = rbExisting.Checked;
        }

        private void rbNew_CheckedChanged(object sender, EventArgs e)
        {
            btnBrowse.Enabled = false;
            FilePath = null;
        }

        private void txtAppName_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetError(txtAppName, string.Empty);
            if (NameValidationUtil.ValidateFileName(txtAppName.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtAppName, "App name contains invalid characters");
            }
        }

        private void NewApplication_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtAppName;
        }
    }
}
