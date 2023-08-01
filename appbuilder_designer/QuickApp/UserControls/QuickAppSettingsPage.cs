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
    public partial class QuickAppSettingsPage : ApttusWizardPageView
    {
        public bool IsWizardInEditMode { get; private set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public QuickAppSettingsPage(Panel view, WizardModel model, PageIndex index, bool editMode)
            : base(view, model, index)
        {
            IsWizardInEditMode = editMode;
            InitializeComponent();
            SetCultureData();
        }


        private void SetCultureData()
        {
            chkAddRow.Text = resourceManager.GetResource("QAAPPSETT_chkAddRow_Text");
            chkAutoFilter.Text = resourceManager.GetResource("COMMON_DisplayFilters_Text");
            chkDeleteRow.Text = resourceManager.GetResource("QAAPPSETT_chkDeleteRow_Text");
            chkEditInExcel.Text = string.Format(resourceManager.GetResource("QAAPPSETT_chkEditInExcel_Text"),resourceManager.CRMName);
            chkSaveSheets.Text = resourceManager.GetResource("QAAPPSETT_chkSaveSheets_Text");
            chkViewGridLines.Text = resourceManager.GetResource("QAAPPSETT_chkViewGridLines_Text");
            groupBox1.Text = resourceManager.GetResource("QAAPPSETT_groupBox1_Text");
            groupBox2.Text = resourceManager.GetResource("QAAPPSETT_groupBox2_Text");
            groupBoxWorksheet.Text = resourceManager.GetResource("QAAPPSETT_groupBoxWorksheet_Text");
            label1.Text = resourceManager.GetResource("QAAPPSETT_label1_Text");
            label2.Text = resourceManager.GetResource("QAAPPSETT_label2_Text");
            label3.Text = resourceManager.GetResource("QAAPPSETT_label3_Text");
            lblMaxColWidth.Text = resourceManager.GetResource("COMMON_MaxColumnWidth_Text");
            lblTitle.Text = resourceManager.GetResource("COMMON_WorkSheetTitle_Text") + ": ";
            txtMaxColumnWidth.Text = resourceManager.GetResource("QAAPPSETT_txtMaxColumnWidth_Text");
            txtRetrieveMenuButton.Text = resourceManager.GetResource("QAAPPSETT_txtRetrieveMenuButton_Text");
            txtSaveMenuButton.Text = resourceManager.GetResource("COMMON_btnSave_Text");
        }

        private void SpreadSheetFormattingPage_Load(object sender, EventArgs e)
        {
            if (IsWizardInEditMode)
            {
                chkAddRow.Checked = Model.AllowAddRow;
                chkDeleteRow.Checked = Model.AllowDeleteRow;
                chkEditInExcel.Checked = Model.AllowEditInExcel;
                chkSaveSheets.Checked = Model.AllowSaveForSheets;
                chkViewGridLines.Checked = Model.ShowGridLines;
                chkAutoFilter.Checked = Model.ShowAutoFilter;
                txtWorksheetTitle.Text = Model.WorksheetTitle;
                txtMenuGroupName.Text = Model.MenuGroupName;
                txtRetrieveMenuButton.Text = Model.DisplayMenuButtonName;
                txtSaveMenuButton.Text = Model.SaveMenuButtonName;
                txtMaxColumnWidth.Text = Convert.ToString(Model.MaxColumnWidth);
            }
            else
            {
                string groupName = Model.AppType == QuickAppType.ListApp ? Model.Object.Name : Model.Object.Children[0].Name;
                txtMenuGroupName.Text = groupName;
            }
            // Uncomment below line, once LMA is updated with GoogleSheets flag
            //chkSaveSheets.Visible = LicenseActivationManager.GetInstance.IsFeatureAvailable("GoogleSheets");
            chkSaveSheets.Visible = true;
        }

        public override void ProcessPage()
        {            
            Model.AllowAddRow = chkAddRow.Checked;
            Model.AllowDeleteRow = chkDeleteRow.Checked;
            Model.AllowEditInExcel = chkEditInExcel.Checked;
            Model.AllowSaveForSheets = chkSaveSheets.Checked;
            Model.ShowGridLines = chkViewGridLines.Checked;
            Model.ShowAutoFilter = chkAutoFilter.Checked;
            Model.WorksheetTitle = txtWorksheetTitle.Text;
            Model.MenuGroupName = txtMenuGroupName.Text;
            Model.DisplayMenuButtonName = txtRetrieveMenuButton.Text;
            Model.SaveMenuButtonName = txtSaveMenuButton.Text;
            Model.MaxColumnWidth = Convert.ToInt32(txtMaxColumnWidth.Text);
        }

        public override bool ValidatePage()
        {
            int nMaxColWidth = string.IsNullOrEmpty(txtMaxColumnWidth.Text) ? 0 : Int32.Parse(txtMaxColumnWidth.Text);

            if (string.IsNullOrEmpty(txtMenuGroupName.Text))
            {
                DisplayError(txtMenuGroupName, resourceManager.GetResource("QAAPPSETT_ValidatePageGroupLabel_ErrorMsg"));
                ActiveControl = txtMenuGroupName;
                return false;
            }
            else if (string.IsNullOrEmpty(txtRetrieveMenuButton.Text))
            {
                DisplayError(txtRetrieveMenuButton, resourceManager.GetResource("QAAPPSETT_ValidatePageRetrieveButton_ErrorMsg"));
                ActiveControl = txtRetrieveMenuButton;
                return false;
            }
            else if (string.IsNullOrEmpty(txtSaveMenuButton.Text))
            {
                DisplayError(txtSaveMenuButton, resourceManager.GetResource("QAAPPSETT_ValidatePageSaveButton_ErrorMsg"));
                ActiveControl = txtSaveMenuButton;
                return false;
            }
            else if (nMaxColWidth <= 0)
            {
                DisplayError(txtMaxColumnWidth, resourceManager.GetResource("QAAPPSETT_ValidatePageInvalidColumn_ErrorMsg"));
                ActiveControl = txtMaxColumnWidth;
                return false;
            }
            return true;
        }
    }
}
