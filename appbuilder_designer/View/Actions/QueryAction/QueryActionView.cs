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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class QueryActionView : Form
    {
        QueryActionController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public QueryActionView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("QUERYACTION_groupActionDetail_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //groupActionDetail.Text = resourceManager.GetResource("QUERYACTION_groupActionDetail_Text");
            label5.Text = resourceManager.GetResource("QUERYACTION_label5_Text");
            lblName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblObject.Text = resourceManager.GetResource("COMMON_Object_Text");
        }

        public void SetController(QueryActionController controller)
        {
            this.controller = controller;
            InitObjectCombo(controller.GetObjects());
        }

        private void expressionBuilder_Load(object sender, EventArgs e)
        {
            this.controller.SetView();
        }

        private ApttusObject SelectedObject
        {
            get
            {
                return ApplicationDefinitionManager.GetInstance.GetAppObject(SelectedObjectUniqueId);
            }
        }

        private Guid SelectedObjectUniqueId
        {
            get
            {
                return ((KeyValuePair<Guid, string>)cboObject.SelectedItem).Key;
            }
        }

        public void LoadBlankForm()
        {
            expressionBuilder.SetExpressionBuilder(null, SelectedObject);
        }

        public void FillForm(QueryActionModel model)
        {
            txtName.Text = model.Name;
            cboObject.SelectedValue = model.TargetObject;

            if (model.MaxRecords != QueryActionModel.MAX_RECORDS_NOLIMIT)
                txtMaxRecords.Text = model.MaxRecords.ToString();
            chkAllowSavingFilters.Checked = model.AllowSavingFilters;
            expressionBuilder.SetExpressionBuilder(controller.GetFilterGroup(), SelectedObject);
        }

        private void InitObjectCombo(List<ApttusObject> objects)
        {
            cboObject.SelectedIndexChanged -= cboObject_SelectedIndexChanged;

            cboObject.DisplayMember = Constants.VALUE_COLUMN;
            cboObject.ValueMember = Constants.KEY_COLUMN;
            cboObject.DataSource = new BindingSource(ConfigurationManager.GetInstance.GetApttusObjectsWithObjectType(), null);

            cboObject.DropDown += new EventHandler(AdjustWidth_DropDown);

            cboObject.SelectedIndexChanged += cboObject_SelectedIndexChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (currentItem is KeyValuePair<Guid, string>)
                {
                    KeyValuePair<Guid, string> item = ((KeyValuePair<Guid, string>)currentItem);
                    newWidth = (int)g.MeasureString(item.Value, font).Width + vertScrollBarWidth;
                }
                else if (currentItem is ApttusField)

                    newWidth = (int)g.MeasureString(((ApttusField)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (!ApttusFormValidator.hasValidationErrors(groupActionDetail.Controls))
            {
                List<SearchFilterGroup> lstFilters = expressionBuilder.SaveExpression(out errorMessage);

                if (String.IsNullOrEmpty(errorMessage))
                {
                    controller.Save(txtName.Text, SelectedObject, lstFilters, txtMaxRecords.Text, chkAllowSavingFilters.Checked);
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
                    this.Close();
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            controller.Cancel();
        }
        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboObject.SelectedIndex == -1)
                return;
            expressionBuilder.SetExpressionBuilder(null, SelectedObject);
        }
        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            String errorMsg = "Name cannot be empty";
            if (string.IsNullOrEmpty(txtName.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtName, errorMsg);
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(txtName, string.Empty);
            }
        }
        
        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(cboObject.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(cboObject, "Object field is mandatory");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cboObject, String.Empty);
            }
        }

        private void QueryActionView_Load(object sender, EventArgs e)
        {
        }

        private void QueryActionView_Shown(object sender, EventArgs e)
        {
            expressionBuilder.MaximumSize = new Size(expressionBuilder.PreferredSize.Width, expressionBuilder.PreferredSize.Height);
        }
    }
}
