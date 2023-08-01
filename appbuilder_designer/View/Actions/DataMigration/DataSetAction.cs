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
    public partial class DataSetActionView : Form
    {
        private ConfigurationManager configManager;
        private DataSetActionModel Model;
        private ApplicationDefinitionManager appDefManager;
        private DataSetActionController Controller;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public DataSetActionView()
        {
            InitializeComponent();
            SetCultureData();
            configManager = ConfigurationManager.GetInstance;
            appDefManager = ApplicationDefinitionManager.GetInstance;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        public void SetCultureData()
        {
            lblName.Text = resourceManager.GetResource("COMMON_Name_Text");
            lblTargetObject.Text = resourceManager.GetResource("DataSetActionView_lblTargetObject_Text");
            lblOperation.Text = resourceManager.GetResource("DataSetActionView_lblOperation_Text");
            rboOROperation.Text = resourceManager.GetResource("DataSetActionView_rboOROperation_Text");
            rboANDOperation.Text = resourceManager.GetResource("DataSetActionView_rboANDOperation_Text");
            btnOK.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            lbDataSet.Text = resourceManager.GetResource("DataSetActionView_lbDataSet_Text");
        }
        private void DataSetAction_Load(object sender, EventArgs e)
        {
            cboTargetObject.DataSource = new BindingSource(configManager.GetApttusObjectsWithObjectType(), null);
            cboTargetObject.DisplayMember = "Value";
            cboTargetObject.ValueMember = "Key";

            if (Controller.FormOpenMode == FormOpenMode.Edit)
            {
                txtName.Text = Model.Name;
                cboTargetObject.SelectedValue = Model.TargetObject;
                if (Model.Operation == DataSetOperation.OR)
                    rboOROperation.Checked = true;
                else if (Model.Operation == DataSetOperation.AND)
                    rboANDOperation.Checked = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;

            if (ApttusFormValidator.hasValidationErrors(tableLayoutPanel1.Controls))
                return;
            Model.Name = txtName.Text;
            Model.Operation = rboOROperation.Checked ? DataSetOperation.OR : DataSetOperation.AND;
            Model.TargetObject = (Guid)cboTargetObject.SelectedValue;

            Controller.Save();
            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("DATASETACTION_btnOK_Click_Cap_Text"));
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        internal void SetController(DataSetActionController dataSetActionController, DataSetActionModel model)
        {
            Controller = dataSetActionController;
            Model = model;
        }

        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is ApttusObject)
                    newWidth = (int)g.MeasureString((currentItem as ApttusObject).Name, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtName, resourceManager.GetResource("ADDROWACTVIEW_txtActionName_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtName, string.Empty);
            }
        }

       
    }
}
