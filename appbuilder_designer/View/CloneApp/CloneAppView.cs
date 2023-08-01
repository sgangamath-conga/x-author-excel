/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class CloneAppView : Form
    {
        CloneAppController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public CloneAppView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }


        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("CLONEAPPVIEW_lblTitle");
            btnClone.Text = resourceManager.GetResource("CLONEAPPVIEW_btnClone_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            lblAppName.Text = resourceManager.GetResource("CLONEAPPVIEW_lblAppName_Text");
        }

        public void SetController(CloneAppController controller)
        {
            this.controller = controller;
        }

        public void PopulateAppDetails(string AppName, string AppVersion)
        {
            txtAppName.Text = AppName;
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(gbAppDetails.Controls) || ApttusFormValidator.hasValidationErrors(gbAppDetails.Controls))
                return;

            controller.CloneApp(txtAppName.Text, "");
            this.Dispose();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void CloneAppView_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.ClearCloneDirectory();
        }

        private void txtAppName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAppName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtAppName, resourceManager.GetResource("ADDROWACTVIEW_txtActionName_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtAppName, string.Empty);
            }
        }
    }
}
