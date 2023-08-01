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
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class SwitchConnectionActionView : Form
    {
        SwitchConnectionController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public Control.ControlCollection PanelControls
        {
            get
            {
                return panel1.Controls;
            }
        }
        
        public SwitchConnectionActionView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("SWITCHCONNACTION_groupBox1_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //groupBox1.Text = resourceManager.GetResource("SWITCHCONNACTION_groupBox1_Text");
            lblActionName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblSwitchTo.Text = resourceManager.GetResource("SWITCHCONNACTION_lblSwitchTo_Text");
        }
        public void SetController(SwitchConnectionController controller)
        {
            this.controller = controller;
        }

        internal void UpdateControls(SwitchConnectionActionModel Model)
        {
            txtActionName.Text = Model.Name;
            txtSwitchConnectionName.Text = Model.SwitchToConnectionName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            controller.Save(txtActionName.Text, Convert.ToString(txtSwitchConnectionName.Text));
        }

        internal void SetError(Control control, string message)
        {
            errorProvider1.SetError(control, message);
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("CONST_ACTIONNAMEREQUIRED"));
        }

        private void SwitchConnectionActionView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        private void txtSwitchConnectionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtSwitchConnectionName, e, resourceManager.GetResource("SWITCHCONNACTIONVIEW_txtSwitchConnectionName_Validating_ErrorMsg"));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}
