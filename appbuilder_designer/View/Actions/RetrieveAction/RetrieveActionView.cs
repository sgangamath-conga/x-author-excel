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
    public partial class RetrieveActionView : Form
    {
        IRetrieveActionController controller;
        ErrorProvider errorProvider = new ErrorProvider();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ShowFilterObjectView objectfilterView;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private List<ShowQueryFilterObjectConfiguration> queryFilterObjectConfigurations = null;

        public RetrieveActionView()
        {
            InitializeComponent();
            SetCultureData();
            errorProvider.ContainerControl = this;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            objectfilterView = null;
        }

        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("RETRIEVEACTION_groupBox1_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //groupBox1.Text = resourceManager.GetResource("RETRIEVEACTION_groupBox1_Text");
            lblActionName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblRetrieveMap.Text = resourceManager.GetResource("COMMON_DisplayMap_Text") + " :";
            chkDisableExcelEvents.Text = resourceManager.GetResource("Designer_chkDisableExcelEvents_Text");
        }

        public void SetController(IRetrieveActionController controller)
        {
            this.controller = controller;
            this.controller.SetView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(groupBox1.Controls))
                return;

            Guid retrieveMapId = cmbRetrieveMap.SelectedItem != null ? Guid.Parse(cmbRetrieveMap.SelectedValue.ToString()) : Guid.Empty;
            List<ShowQueryFilterObjectConfiguration> showQueryFilterObjectConfigurations = null;

            if (chkShowQueryFilters.Checked && objectfilterView != null)
            {
                showQueryFilterObjectConfigurations = objectfilterView.GetObjectConfigurations();
                //there is a possibility that the no objects are selected to show filters, even though the showQueryFilters checkbox is checked.
                //In that case un-check the checkbox.
                if (showQueryFilterObjectConfigurations == null || (showQueryFilterObjectConfigurations != null && showQueryFilterObjectConfigurations.Count == 0))
                {
                    chkShowQueryFilters.Checked = false;
                    showQueryFilterObjectConfigurations = null;
                }
            }

            if (controller.Save(txtActionName.Text, retrieveMapId, showQueryFilterObjectConfigurations, chkDisableExcelEvents.Checked))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
                this.Close();
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            controller.Cancel();
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("COMMON_NameCannotBeEmpty_ErrorMsg"));
        }

        public void SetError(Control control, string message)
        {
            errorProvider.SetError(control, message);
        }

        public void SetRetrieveMapsList(List<RetrieveMap> retrievelist, List<MatrixMap> matrixList)
        {
            Dictionary<Guid, string> list = new Dictionary<Guid, string>();
            foreach (RetrieveMap rMap in retrievelist)
                list.Add(rMap.Id, rMap.Name);

            foreach (MatrixMap mMap in matrixList)
                list.Add(mMap.Id, mMap.Name);

            cmbRetrieveMap.ValueMember = "Key";
            cmbRetrieveMap.DisplayMember = "Value";
            cmbRetrieveMap.DataSource = new BindingSource(list, null);
        }
        
        internal void FillForm(RetrieveActionModel model)
        {
            txtActionName.Text = model.Name;
            chkDisableExcelEvents.Checked = model.DisableExcelEvents;
            cmbRetrieveMap.SelectedValue = model.RetrieveMapId;
            queryFilterObjectConfigurations = model.ShowQueryFilterObjectConfigurations;
        }

        private void cmbRetrieveMap_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cmbRetrieveMap, e, resourceManager.GetResource("RETRIEVEACTIONVIEW_cmbRetrieveMap_Validating_Text"));
        }

        private void RetrieveActionView_Load(object sender, EventArgs e)
        {
            chkShowQueryFilters.Checked = queryFilterObjectConfigurations != null && queryFilterObjectConfigurations.Count > 0;
        }

        private void chkShowQueryFilters_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowQueryFilters.Checked)
            {
                if (objectfilterView == null)
                {
                    RetrieveMap rMap = configManager.RetrieveMaps.Find(rm => rm.Id == (Guid)cmbRetrieveMap.SelectedValue);
                    objectfilterView = new ShowFilterObjectView(rMap, queryFilterObjectConfigurations);
                }
                pnlObjectFilterview.Controls.Add(objectfilterView);
                objectfilterView.Dock = DockStyle.Fill;
            }
            else
            {
                pnlObjectFilterview.Controls.Remove(objectfilterView);
            }
        }

        private void cmbRetrieveMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ManageFilterCheckbox()) return;

            if (objectfilterView != null && cmbRetrieveMap.SelectedIndex != -1)
            {
                pnlObjectFilterview.Controls.Remove(objectfilterView);
                objectfilterView.Dispose();
                RetrieveMap rMap = configManager.RetrieveMaps.Find(rm => rm.Id == (Guid)cmbRetrieveMap.SelectedValue);
                objectfilterView = new ShowFilterObjectView(rMap, queryFilterObjectConfigurations);
                pnlObjectFilterview.Controls.Add(objectfilterView);
                objectfilterView.Dock = DockStyle.Fill;
                this.Size = new Size(this.Width, 400);
            }

        }

        bool ManageFilterCheckbox()
        {
            bool bEnable = false;
            if (cmbRetrieveMap.SelectedIndex != -1)
            {
                RetrieveMap rMap = configManager.RetrieveMaps.Find(rm => rm.Id == (Guid)cmbRetrieveMap.SelectedValue);
                bEnable = rMap == null ? false : true;
                chkShowQueryFilters.Enabled = bEnable;
            }
            return bEnable;
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
            foreach (object currentItem in senderComboBox.Items)
            {
                if (currentItem is KeyValuePair<Guid, string>)
                {
                    KeyValuePair<Guid, string> item = ((KeyValuePair<Guid, string>)currentItem);
                    newWidth = (int)g.MeasureString(item.Value, font).Width + vertScrollBarWidth;
                }                

                if (width < newWidth)
                    width = newWidth;
            }
            senderComboBox.DropDownWidth = width;
        }
    }
}
