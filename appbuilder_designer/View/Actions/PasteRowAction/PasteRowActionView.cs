using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class PasteRowActionView : Form
    {
        ConfigurationManager configManager;
        PasteRowActionController controller;
        PasteRowActionModel Model;
        string saveGroupTargetNamedRange;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public PasteRowActionView()
        {
            InitializeComponent();
            SetCultureData();
            configManager = ConfigurationManager.GetInstance;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label5.Text = resourceManager.GetResource("PASTEACTION_groupBox1_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnSave.Text = resourceManager.GetResource("COMMON_Save_AMPText");
            //groupBox1.Text = resourceManager.GetResource("PASTEACTION_groupBox1_Text");
            label1.Text = resourceManager.GetResource("COMMON_Name_Text");
            label2.Text = resourceManager.GetResource("COMMON_SaveMap_Text");
            label3.Text = resourceManager.GetResource("COMMON_ListObject_Text");
            label4.Text = resourceManager.GetResource("PASTEACTION_label4_Text");
            rdBottom.Text = resourceManager.GetResource("COMMON_Bottom_Text");
            rdPasteAll.Text = resourceManager.GetResource("PASTEACTION_rdPasteAll_Text");
            rdPasteValues.Text = resourceManager.GetResource("PASTEACTION_rdPasteValues_Text");
            rdTop.Text = resourceManager.GetResource("COMMON_Top_Text");
        }

        internal void SetController(PasteRowActionController pasteRowActionController)
        {
            controller = pasteRowActionController;
        }

        internal void FillObjects()
        {
            List<SaveMap> saveMaps = (from sm in configManager.SaveMaps
                                      from sg in sm.SaveGroups
                                      where sg.GroupId != Guid.Empty
                                      select sm).Distinct().ToList<SaveMap>();

            cboSaveMap.DisplayMember = "Name";
            cboSaveMap.ValueMember = "Id";
            cboSaveMap.DataSource = saveMaps;
            cboSaveMap.SelectedIndex = -1;
            cboListObject.SelectedIndex = -1;
            cboListObject.Enabled = false;
        }


        internal void UpdateControls(PasteRowActionModel model)
        {
            Model = model;
            txtName.Text = Model.Name;
            cboSaveMap.SelectedValue = Model.SaveMapId;
            cboListObject.SelectedValue = Model.ListObjectId;

            if (Model.RowLocation == AddRowLocation.Top) 
                rdTop.Checked = true;
            else 
                rdBottom.Checked = true;

            if (model.PasteType == PasteType.PasteValues) 
                rdPasteValues.Checked = true;
            else 
                rdPasteAll.Checked = true;
            cboListObject.Enabled = true;
        }

        private void PasteActionView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        private void cboSaveMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSaveMap.SelectedIndex != -1)
            {
                cboListObject.Enabled = true;
                ApplicationDefinitionManager applicationDefinationManager = ApplicationDefinitionManager.GetInstance;
                SaveMap selection = cboSaveMap.SelectedItem as SaveMap;

                var saveGrps = from sg in selection.SaveGroups
                               where sg.GroupId != Guid.Empty
                               select sg;

                List<ApttusObject> apttusObjects = (from sg in saveGrps
                                                    from ao in applicationDefinationManager.GetAllObjects()
                                                    where sg.AppObject == ao.UniqueId && ao.ObjectType == ObjectType.Repeating
                                                    select ao).ToList<ApttusObject>();
                cboListObject.DisplayMember = "Name";
                cboListObject.ValueMember = "UniqueId";
                cboListObject.DataSource = apttusObjects;
            }
        }


        private void cboListObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSaveMap.SelectedIndex != -1 && cboListObject.SelectedIndex != -1)
            {
                ApttusObject apttusObject = cboListObject.SelectedItem as ApttusObject;
                SaveMap selection = cboSaveMap.SelectedItem as SaveMap;
                string targetNamedRange = (from sg in selection.SaveGroups
                                           where sg.GroupId != Guid.Empty && sg.AppObject == apttusObject.UniqueId
                                           select sg.TargetNamedRange).FirstOrDefault();
                saveGroupTargetNamedRange = targetNamedRange;
            }
        }

        private void cboSaveMap_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboSaveMap, e, resourceManager.GetResource("PASTEROWACTVIEW_cboSaveMap_Validating_InfoMsg"));
        }


        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        private void cboListObject_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboListObject, e, resourceManager.GetResource("PASTEROWACTVIEW_cboListObject_Validating_InfoMsg"));
        }

        internal void SetError(Control control, string errorMsg)
        {
            errorProvider1.SetError(control, errorMsg);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.Controls))
                return;

            if (Model == null)
                Model = new PasteRowActionModel();

            Model.Name = txtName.Text;
            Model.SaveMapId = (cboSaveMap.SelectedItem as SaveMap).Id;
            Model.ListObjectId = (cboListObject.SelectedItem as ApttusObject).UniqueId;
            Model.SaveGroupTargetNamedRange = saveGroupTargetNamedRange;
            Model.RowLocation = (rdTop.Checked ? AddRowLocation.Top : AddRowLocation.Bottom);
            Model.PasteType = (rdPasteValues.Checked ? PasteType.PasteValues : PasteType.PasteAll);
            controller.Save(Model);

            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
            this.Dispose();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
