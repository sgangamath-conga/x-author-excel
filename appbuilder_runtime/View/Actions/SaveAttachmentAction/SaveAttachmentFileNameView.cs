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
    public partial class SaveAttachmentFileNameView : Form
    {
        public string FileName { get; set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveAttachmentFileNameView(string fileName, bool fromCheckinAction = false)
        {
            InitializeComponent();
            SetCultureData();
            if(fromCheckinAction) //if called from checkin action, set title to check in
            {
                this.lblTitle.Text = resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg");
            }
            else
            {
                lblTitle.Text = resourceManager.GetResource("SAVEATTCHFILENAMEVIEW_btnOK_ClickCAP_InfoMsg");
            }
            FileName = string.Empty;
            txtFilename.Text = fileName;
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COREMULTILINEVIEW_btnCancel_Text");
            btnOK.Text = resourceManager.GetResource("SAVEATTFILENAMEVIEW_btnOK_Text");
            label1.Text = resourceManager.GetResource("SAVEATTFILENAMEVIEW_label1_Text");            
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilename.Text))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("SAVEATTCHFILENAMEVIEW_btnOK_Click_InfoMsg"), resourceManager.GetResource("SAVEATTCHFILENAMEVIEW_btnOK_ClickCAP_InfoMsg"));
                return;
            }
            FileName = txtFilename.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
