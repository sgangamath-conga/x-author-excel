/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class MenuHome : Form, IMenuView
    {
        private MenuMediator TvwAdapter;
        MenuController controller;

        public MenuHome()
        {
            InitializeComponent();

            Icon = Properties.Resources.XA_AppBuilder_Icon;

            TvwAdapter = new MenuMediator(MnuTree, propertyGrid1, cmbMenuSelector, btnCreate,
                btnDelete, btnSave, new Button(), lblWF, new Button(), new Button(), cmboWf, btnAddItem,
                chkShowAddRow, ChkShowDeleteRow, ChkShowChatter, chkMatrixAddColumn);
            XlationUtil xl = new XlationUtil();
            CenterToScreen();
            //xl.XlateAll(this);
            SetCultureData();
        }

        private void SetCultureData()
        {
            ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

            //resourceManager.GetResource("MNUHOME_userProfile");
            lblMenuDesigner.Text = resourceManager.GetResource("COMMON_UserMenu_Text");
            //resourceManager.GetResource("MNUHOME_selectUserProfile");
            label1.Text = resourceManager.GetResource("MNUHOME_mnuDesignerAddLabel");
            lblWF.Text = resourceManager.GetResource("COMMON_ActionFlow_Text");
            //resourceManager.GetResource("MNUHOME_lblWF");
            lblProperties.Text = resourceManager.GetResource("MNUHOME_lblProperties");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            btnDelete.Text = resourceManager.GetResource("COMMON_Remove_Text");
            //resourceManager.GetResource("COMMON_Delete_Text");
            //resourceManager.GetResource("MNUHOME_btnCreateActionflow");
            btnCreate.Text = resourceManager.GetResource("COMMON _btnCreate_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnAddGroup.Text = resourceManager.GetResource("MNUHOME_btnAddGroup");
            btnAddItem.Text = resourceManager.GetResource("MNUHOME_btnAddButton");
            //resourceManager.GetResource("MNUHOME_AppTitle");
            ChkShowChatter.Text = resourceManager.GetResource("MNUHOME_ChkShowChatter");
            ChkShowDeleteRow.Text = string.Format(resourceManager.GetResource("MNUHOME_ChkShowDeleteRow"), resourceManager.CRMName);
            chkMatrixAddColumn.Text = resourceManager.GetResource("MNUHOME_chkMatrixAddColumn");
            chkShowAddRow.Text = resourceManager.GetResource("MNUHOME_chkShowAddRow");

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            controller.AddMenu((int)cmbMenuSelector.SelectedIndex);
        }

        public MenuMediator GetMediator()
        {
            return TvwAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menucontroller"></param>
        public void SetController(MenuController menucontroller)
        {
            controller = menucontroller;
            controller.MenuControlsMediator = TvwAdapter;

            cmbMenuSelector.DataSource = controller.MenuComboDataset();
            cmboWf.ValueMember = "id";
            cmboWf.DisplayMember = "Name";
            cmboWf.DataSource = controller.GetWF();
        }

        public void AttachPropertyGrid(AbstractMenu menu)
        {
            this.propertyGrid1.SelectedObject = menu;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            controller.DeleteItem();
        }

        private void MnuTree_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            controller.OnSelectNode();
        }

        private void MnuTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            controller.OnSelectNode();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            controller.PropertyGridValChanged();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            controller.Save(chkShowAddRow.Checked, ChkShowDeleteRow.Checked, ChkShowChatter.Checked, chkMatrixAddColumn.Checked);
            Close();
        }

        private void cmbMenuSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.MenuTypeChanged();
        }

        private void cmdPermission_Click(object sender, EventArgs e)
        {
            controller.ShowPermission();
        }

        private void btnAttachWF_Click(object sender, EventArgs e)
        {
            // TODO:: 1. Fill in workflwo drop-down with available workflows , workflow name = displaymember, workflow Id = valuemember
            //        2. If workflow is selected from drop-down pass workflow ID in ShowWorkflow() method.
            Workflow wf = cmboWf.SelectedItem as Workflow;
            string id = string.Empty;

            if (wf != null)
                id = wf.Id.ToString();
            controller.ShowWorkflow(id);
        }

        private void cmboWf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboWf.SelectedItem != null && cmboWf.Enabled)
                controller.setWorkFlow(cmboWf.SelectedItem as Workflow);
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            controller.AddMenu(1);
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            controller.AddMenu(2);
        }

        private void MenuHome_Load(object sender, EventArgs e)
        {
            ChkShowChatter.Enabled = LicenseActivationManager.GetInstance.IsFeatureAvailable(Constants.CHATTER_IN_EXCEL_TAG);
        }
    }
}
