/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{    
    public partial class QuickAppSelectionPage : ApttusWizardPageView
    {
        private IXAuthorApplicationController appController;
        private ImageList imagelist;
        private static int ImageSizeWidth = 40;
        private static int ImageSizeHeight = 40;
        private bool IsQuickAppInEditMode;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public QuickAppSelectionPage(Panel view, WizardModel model, PageIndex index, bool editMode)
            : base(view, model, index)
        {
            IsQuickAppInEditMode = editMode;
            InitializeComponent();
            SetCultureData();

            appController = Globals.ThisAddIn.GetApplicationController();
        }
        
        private void SetCultureData()
        {
            lblListAppDescription.Text = resourceManager.GetResource("QAAPPSEL_lblListAppDescription_Text");
            lblParentChild.Text = resourceManager.GetResource("QAAPPSEL_lblParentChild_Text");
            lblPCDescription.Text = resourceManager.GetResource("QAAPPSEL_lblPCDescription_Text");
            lblSelectAppType.Text = resourceManager.GetResource("QAAPPSEL_lblSelectAppType_Text");
            lblSingleObject.Text = resourceManager.GetResource("QAAPPSEL_lblSingleObject_Text");
            label1.Text = resourceManager.GetResource("NEWAPP_label1_Text");
        }
        public override void ProcessPage()
        {
            if (!IsQuickAppInEditMode)
            {
                Model.AppName = txtAppName.Text;
                //if (listViewAppType.SelectedItems.Count > 0)
                //    Model.AppType = (QuickAppType)listViewAppType.SelectedItems[0].Tag;
            }
        }

        public override bool ValidatePage()
        {
            if (IsQuickAppInEditMode)
                return true; //In edit mode, there is nothing to validate.

            if (txtAppName.TextLength == 0 || String.IsNullOrWhiteSpace(txtAppName.Text))
            {
                DisplayError(txtAppName, resourceManager.GetResource("QAAPPSEL_ValidatePageAppName_ErrorMsg"));
                txtAppName.Text = string.Empty;
                ActiveControl = txtAppName;
                return false;
            }

            if (NameValidationUtil.ValidateFileName(txtAppName.Text))
            {
                DisplayError(txtAppName, resourceManager.GetResource("QAAPPSEL_ValidatePageAppNameInvalid_ErrorMsg"));
                ActiveControl = txtAppName;
                return false;
            }

            if (!rboListApp.Checked && !rboMasterDetailApp.Checked)
            {
                DisplayError(lblSelectAppType, resourceManager.GetResource("QAAPPSEL_ValidatePageAppType_ErrorMsg"));
                return false;
            }

            if (appController.AppNameExists(txtAppName.Text.Trim()))
            {                
                ApttusMessageUtil.ShowError(resourceManager.GetResource("QUICKAPP_AppNameExists_ErrorMsg"), resourceManager.GetResource("QUICKAPP_AppNameExistsCap_ErrorMsg"));
                txtAppName.Text = string.Empty;
                ActiveControl = txtAppName;
                return false;
            }

            ClearErrorProvider();
            return true;
        }

        private void QuickAppSelectionPage_Load(object sender, EventArgs e)
        {
            PageName = "App Selection Page";

            rboListApp.Tag = QuickAppType.ListApp;
            rboMasterDetailApp.Tag = QuickAppType.ParentChildApp;

            if (!IsQuickAppInEditMode)
                this.ActiveControl = txtAppName;
            else
            {
                txtAppName.Text = Model.AppName;
                if (Model.AppType == QuickAppType.ListApp)
                    rboListApp.Checked = true;
                else
                    rboMasterDetailApp.Checked = true;

                txtAppName.Enabled = rboListApp.Checked = rboMasterDetailApp.Checked = false;
            }
        }

        private void rboMasterDetailApp_CheckedChanged(object sender, EventArgs e)
        {
            if (rboMasterDetailApp.Checked == true)
            {
                Model.AppType = QuickAppType.ParentChildApp;
                rboListApp.Checked = false;
            }
        }

        private void rboListApp_CheckedChanged(object sender, EventArgs e)
        {
            if (rboListApp.Checked == true)
            {
                Model.AppType = QuickAppType.ListApp;
                rboMasterDetailApp.Checked = false;
            }
        }

        private void pnlListApp_Click(object sender, EventArgs e)
        {
            rboListApp.Checked = true;
        }

        private void pnlMasterChild_Click(object sender, EventArgs e)
        {
            rboMasterDetailApp.Checked = true;
        }

        private void lblSingleObject_Click(object sender, EventArgs e)
        {
            rboListApp.Checked = true;
        }

        private void lblParentChild_Click(object sender, EventArgs e)
        {
            rboMasterDetailApp.Checked = true;
        }

        private void lblListAppDescription_Click(object sender, EventArgs e)
        {
            rboListApp.Checked = true;
        }

        private void lblPCDescription_Click(object sender, EventArgs e)
        {
            rboMasterDetailApp.Checked = true;
        }

        private void pbParentChild_Click(object sender, EventArgs e)
        {
            rboMasterDetailApp.Checked = true;
        }

        private void pbSingleObject_Click(object sender, EventArgs e)
        {
            rboListApp.Checked = true;
        }
    }    
}
