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

namespace Apttus.XAuthor.AppRuntime
{
    public partial class InputDialog : Form
    {
        public string FilterSetName { get; private set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public InputDialog()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            //    label1.Text = resourceManager.GetResource("FILEINPUTDIALOG_label1_Text");
            //    btnOk.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnCancel_Text");
            //    lblTitle.Text = resourceManager.GetResource("FILEINPUT_lblTitle_Text");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;

            if (ApttusFormValidator.hasValidationErrors(this.Controls))
                return;

            FilterSetName = txtFiltersetName.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        private void txtFiltersetName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFiltersetName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFiltersetName, resourceManager.GetResource("InputDlg_txtFiltersetName_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtFiltersetName, string.Empty);
            }
        }
    }
}
