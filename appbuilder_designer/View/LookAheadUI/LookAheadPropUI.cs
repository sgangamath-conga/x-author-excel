/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Apttus.XAuthor.Core;
using System.Drawing;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class LookAheadPropUI : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public LookAheadPropUI()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            //  HideTabPage(this.tabControl1.TabPages[2]);
        }

        private void SetCultureData()
        {
            button1.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnApply.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            btnDetachUI.Text = resourceManager.GetResource("COMMON_Remove_Text");
            chkActive.Text = resourceManager.GetResource("COMMON_IsActive_Text");
            chkFieldMapping.Text = resourceManager.GetResource("LOOKAHEADPROPUI_chkFieldMapping_Text");
            chkRefreshData.Text = resourceManager.GetResource("LOOKAHEADPROPUI_chkRefreshData_Text");
            chkSSActive.Text = resourceManager.GetResource("COMMON_IsActive_Text");
            cmdCurrentSelection.Text = resourceManager.GetResource("LOOKAHEADPROPUI_cmdCurrentSelection_Text");
            groupBox4.Text = resourceManager.GetResource("LOOKAHEADPROPUI_groupBox4_Text");
            label1.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label1_Text");
            label10.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label10_Text");
            label11.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label11_Text");
            label12.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label12_Text");
            label13.Text = resourceManager.GetResource("LOOKAHEADPROPUI_Title");
            label2.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label2_Text");
            label3.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label3_Text");
            label4.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label4_Text");
            label5.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label5_Text");
            label6.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label6_Text");
            label7.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label7_Text");
            label8.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label8_Text");
            label9.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label9_Text");
            lblReference.Text = resourceManager.GetResource("LOOKAHEADPROPUI_lblReference_Text");
            radAdvanced.Text = resourceManager.GetResource("LOOKAHEADPROPUI_radAdvanced_Text");
            radBasic.Text = resourceManager.GetResource("LOOKAHEADPROPUI_radBasic_Text");
            tbExcel.Text = resourceManager.GetResource("LOOKAHEADPROPUI_tbExcel_Text");
            tbSearchandSelect.Text = resourceManager.GetResource("COMMON_SearchandSelect_Text");
        }

        public FieldMappingView MappingView { get { return fieldMappingView1; } }

        private void HideTabPage(TabPage tp)
        {
            if (tabControl1.TabPages.Contains(tp))
                tabControl1.TabPages.Remove(tp);
        }

        private LookAheadPropController ctrl;
        public LookAheadPropController Controller {
            get { return ctrl; }
            set {
                ctrl = value;
                if (ctrl.GetType().Equals(typeof(LookAheadExcelSourceController)))
                {
                    ((LookAheadExcelSourceController)ctrl).RegisterControls(this.txtReference,
                        this.txtCurrentSelection, this.txtField, lstDisplay, cmboReturn, this.chkRefreshData, this.chkActive,
                        this.radBasic, this.radAdvanced, groupBox4, this.cmboTgtField, this.cmboRetCol2);

                }
            }
        }

        public FieldMapper FldMapper {
            get;
            set;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedIndex == 0)
            {
                Controller = new LookAheadExcelSourceController(this);

                ((LookAheadExcelSourceController)Controller).RegisterControls(this.txtReference,
                         this.txtCurrentSelection, this.txtField, lstDisplay, cmboReturn, this.chkRefreshData, this.chkActive,
                         this.radBasic, this.radAdvanced, groupBox4, this.cmboTgtField, this.cmboRetCol2);

                ExcelDataSource ds = Controller.LookAheadProp as ExcelDataSource;

                if (ds == null || ds.MultiCol)
                {
                    chkActive.Checked = false;
                }
                else if (ds != null)
                {
                    chkActive.Checked = ds.IsActive;
                }

            }
            else if (tc.SelectedIndex == 1) // search and select action 
            {
                // populate ui method is called in ctor and it needs the isactive info so 
                // pass it in the ctor.
                Controller = new LookAheadSearchSelectController(this, this.chkSSActive, this.chkActive, this.chkRefreshData, this.cmboReturnIDFld, this.chkFieldMapping, cmboActionName, fieldMappingView1);
            }
        }

        private void cmdCurrentSelection_Click(object sender, EventArgs e)
        {
            txtReference.Text = ((LookAheadExcelSourceController)Controller).GetActiveColReference();

            if (!radBasic.Checked)
            {
                ((LookAheadExcelSourceController)Controller).GetMultiColums();
                if (FldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
                {
                    cmboTgtField.DataSource = ((LookAheadExcelSourceController)Controller).FillRetrieveFields();
                    cmboTgtField.DisplayMember = "FieldName";
                    cmboTgtField.ValueMember = "TargetColumnIndex";
                }
                else
                {
                    cmboTgtField.DataSource = ((LookAheadExcelSourceController)Controller).FillSaveFields();
                    cmboTgtField.DisplayMember = "FieldName";
                    cmboTgtField.ValueMember = "TargetColumnIndex";
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            LookAheadProperty prop = Controller.Apply();


            if (prop != null)
            {
                if (FldMapper.AddProperty(prop))
                {

                    this.Hide();
                    if (FldMapper.WarnInActiveProps())
                    {

                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKAHEADPROPUIFORM_btnApply_ClickWARN_InfoMsg"), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                    }
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKAHEADPROPUIFORM_btnApply_Click_InfoMsg"), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                    this.Close();

                }

            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetachUI_Click(object sender, EventArgs e)
        {
            Controller.Detach();
        }

        private void cmdGetMultiCol_Click(object sender, EventArgs e)
        {
            ((LookAheadExcelSourceController)Controller).GetMultiColums();
        }

        private void radAdvanced_CheckedChanged_1(object sender, EventArgs e)
        {
            ((LookAheadExcelSourceController)Controller).RadioAdvanceSelect();
        }

        private void radBasic_CheckedChanged_1(object sender, EventArgs e)
        {
            ((LookAheadExcelSourceController)Controller).RadioBasicSelect();
        }

        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            using (Graphics g = senderComboBox.CreateGraphics())
            {
                Font font = senderComboBox.Font;
                int vertScrollBarWidth = (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth = 0;
                foreach (object currentItem in ((ComboBox)sender).Items)
                {
                    if (currentItem is RetrieveField)
                        newWidth = (int)g.MeasureString(((RetrieveField)currentItem).FieldName, font).Width + vertScrollBarWidth;
                    else if (currentItem is SaveFieldBound)
                        newWidth = (int)g.MeasureString(((SaveFieldBound)currentItem).FieldName, font).Width + vertScrollBarWidth;

                    if (width < newWidth)
                        width = newWidth + 16;
                }
            }
            senderComboBox.DropDownWidth = width;
        }

        private void cmboRetCol2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboRetCol2.SelectedIndex == 0)
            {
                cmboTgtField.SelectedIndex = -1;
                cmboTgtField.Enabled = false;
            }
            else
                cmboTgtField.Enabled = true;
        }

        private void cmboActionName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchAndSelect searchAndSelect = cmboActionName.SelectedItem as SearchAndSelect;
            if (searchAndSelect != null && chkFieldMapping.Checked)
            {
                if (FldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
                {
                    RetrieveFieldMapper mapper = FldMapper as RetrieveFieldMapper;
                    fieldMappingView1.RetrieveMap = mapper.MappedRetrieveMap;
                    fieldMappingView1.FieldAppObject = mapper.MappedRetrieveField.AppObject;
                    fieldMappingView1.SearchAndSelectAction = searchAndSelect;
                }
            }
        }

        private void chkFieldMapping_CheckedChanged(object sender, EventArgs e)
        {
            if (Controller != null)
                Controller.PopulateActions(chkFieldMapping.Checked);

            fieldMappingView1.Visible = chkFieldMapping.Checked;
        }

        private void LookAheadPropUI_Load(object sender, EventArgs e)
        {
            chkFieldMapping.Enabled = false;
            fieldMappingView1.SetCultureData();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
