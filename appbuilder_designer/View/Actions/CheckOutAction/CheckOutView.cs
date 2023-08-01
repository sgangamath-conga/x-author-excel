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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class CheckOutView : Form
    {
        ICheckOutController controller;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public Control.ControlCollection PanelControls
        {
            get
            {
                return panel1.Controls;
            }
        }

        public CheckOutView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            label2.Text = resourceManager.GetResource("CHECKOUTACTION_gbCheckIn_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnOK.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //gbCheckIn.Text = resourceManager.GetResource("CHECKOUTACTION_gbCheckIn_Text");
            label1.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            label3.Text = resourceManager.GetResource("COMMON_Object_Text");
        }

        public void SetController(ICheckOutController controller)
        {
            this.controller = controller;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            controller.Save(txtActionName.Text, cboObject.SelectedItem as ApttusObject);
        }

        internal void FillObjects()
        {
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
            List<ApttusObject> appObjects = appDefManager.GetAllObjects().Where(s => s.ObjectType == ObjectType.Independent).ToList();
            cboObject.DataSource = appObjects;
            cboObject.DisplayMember = "Name";
            cboObject.ValueMember = "UniqueId";
        }

        internal void UpdateControls(CheckOutModel Model)
        {
            txtActionName.Text = Model.Name;
            cboObject.SelectedValue = Model.AppObjectId;
        }

        internal void SetError(Control control, string message)
        {
            errorProvider1.SetError(control, message);
        }

        private void CheckOutView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboObject, e,  resourceManager.GetResource("SAVEATTACHVIEW_cboObject_Validating_ErrorMsg"));
        }        
    }
}
