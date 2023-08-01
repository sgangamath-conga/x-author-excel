using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DeleteActionView : Form
    {
        DeleteActionController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;
        List<DeleteObjectData> objects = new List<DeleteObjectData>();
        
        public DeleteActionView()
        {
            InitializeComponent();
            setCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        public void setCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("COMMON_DeleteAction_Text");
            lblActionName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblObject.Text = resourceManager.GetResource("COMMON_Object_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            lblMap.Text = resourceManager.GetResource("COMMON_SaveMap_Text");
            chkPromptMessage.Text = resourceManager.GetResource("DELETEACTION_Prompt_Message");
        }
        
        private ApttusObject SelectedObject
        {
            get
            {
                return appDefManager.GetAppObject(SelectedObjectUniqueId);
            }
        }

        private Guid SelectedObjectUniqueId
        {
            get
            {
                return (cboObject.SelectedItem as DeleteObjectData).AppObject;
            }
        }
        
        private void DeleteActionView_Load(object sender, EventArgs e)
        {
            //dataTableExpressionBuilder.SetCultureData();
        }

        public void FillForm(DeleteActionModel model)
        {
            txtName.Text = model.Name;
            cboMap.SelectedValue = model.MapId;
            cboObject.SelectedValue = model.AppObjectUniqueId;
            chkPromptMessage.Checked = model.PromptConfirmationDialog;
        }

        private void InitMapCombo()
        {
            try
            {
                cboMap.SelectedIndexChanged -= cboMap_SelectedIndexChanged;
                cboMap.DisplayMember = Constants.VALUE_COLUMN;
                cboMap.ValueMember = Constants.KEY_COLUMN;
                Dictionary<Guid,string> saveMaps = controller.getMaps();
                if (saveMaps.Count > 0)
                    cboMap.DataSource = new BindingSource(saveMaps, null);
            }
            finally
            {
                cboMap.SelectedIndex = -1;
                cboMap.SelectedIndexChanged += cboMap_SelectedIndexChanged;
            }
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
            foreach (object currentItem in senderComboBox.Items)
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

        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboObject.SelectedIndex == -1)
                return;
            DeleteObjectData selectedMap = cboObject.SelectedItem as DeleteObjectData;
            if (selectedMap != null)
            {
                lblWarning.Text = string.Format(resourceManager.GetResource("DELETEACTION_Warning_TEXT"), SelectedObject.Name);

                List<SearchFilterGroup> model = controller.FormOpenMode == FormOpenMode.Create ? null : controller.GetFilterGroup();
                dataTableExpressionBuilder.SetExpressionBuilder(model, SelectedObject, selectedMap.TargetNamedRange);
            }
        }

        public void SetController(DeleteActionController deleteActionController)
        {
            this.controller = deleteActionController;
        }

        private void expressionBuilder_Load(object sender, EventArgs e)
        {
            InitMapCombo();
            dataTableExpressionBuilder.SetCultureData();
            this.controller.SetView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (!ApttusFormValidator.hasValidationErrors(gboActionDetails.Controls))
            {
                List<SearchFilterGroup> lstFilters = dataTableExpressionBuilder.SaveExpression(out errorMessage);
                if (String.IsNullOrEmpty(errorMessage))
                {
                    DeleteObjectData selectedSaveMap = (cboObject.SelectedItem as DeleteObjectData);
                    controller.Save(txtName.Text,((KeyValuePair<Guid, string>)cboMap.SelectedItem).Key, selectedSaveMap.SaveGroupId, selectedSaveMap.AppObject,  lstFilters, chkPromptMessage.Checked);
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
                    this.Close();
                }
            }
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            String errorMsg = resourceManager.GetResource("ADDROWACTVIEW_txtActionName_Validating_ErrorMsg");
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

        private void cboMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMap.SelectedIndex == -1)
                return;

            objects.Clear();

            SaveMap saveMap = configManager.SaveMaps.Find(sm => sm.Id == ((KeyValuePair<Guid, string>)cboMap.SelectedItem).Key);
            if (saveMap != null)
            {
                List<DeleteObjectData> appObjects = (from sg in saveMap.SaveGroups
                                                    from rm in configManager.RetrieveMaps
                                                    from rg in rm.RepeatingGroups
                                                    where rg.TargetNamedRange == sg.TargetNamedRange && rg.AppObject == sg.AppObject
                                                    && saveMap.SaveFields.Where(f => f.GroupId == sg.GroupId).Any(f => f.SaveFieldType == SaveType.RetrievedField)
                                                    select new DeleteObjectData { AppObject = sg.AppObject, ObjectName = appDefManager.GetAppObject(sg.AppObject).Name, 
                                                               SaveGroupId = sg.GroupId, TargetNamedRange = sg.TargetNamedRange,
                                                                RetreiveMapId = rm.Id}).ToList();

                cboObject.SelectedIndexChanged -= cboObject_SelectedIndexChanged;

                cboObject.DataSource = appObjects;
                cboObject.ValueMember = "AppObject";
                cboObject.DisplayMember = "ObjectName";
                cboObject.SelectedIndex = -1;

                dataTableExpressionBuilder.ResetExpressionBuilder();

                cboObject.SelectedIndexChanged += cboObject_SelectedIndexChanged;
            }
        }

        private void cboMap_Validating(object sender, CancelEventArgs e)
        {
            String errorMsg = resourceManager.GetResource("DELETEACTVIEW_cboMap_Validating_ErrorMsg");
            if (cboMap.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider.SetError(cboMap, errorMsg);
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cboMap, string.Empty);
            }
        }

        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            String errorMsg = resourceManager.GetResource("DELETEACTVIEW_cboObject_Validating_ErrorMsg");
            if (cboObject.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider.SetError(cboObject, errorMsg);
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cboObject, string.Empty);
            }
        }           
    }

    internal class DeleteObjectData
    {
        public Guid AppObject { get; set; }
        public Guid SaveGroupId { get; set; }
        public string ObjectName { get; set; }
        public string TargetNamedRange { get; set; }
        public Guid RetreiveMapId { get; set; }
    }
}
