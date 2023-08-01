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
    public partial class WorkflowTriggerView : Form
    {
        private WorkflowTriggerController controller;

        public WorkflowTriggerView()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        public void SetController(WorkflowTriggerController controller)
        {
            this.controller = controller;

            if (controller.Triggers != null && controller.Triggers.Contains(WorkflowStructure.Trigger.AppLoad))
            {
                chkAppLaunch.Checked = true;
                radioAppLaunch.Checked = true;

                radioFirstAppLaunch.Enabled = true;
                radioAppLaunch.Enabled = true;
            }

            if ( controller.Triggers != null  && controller.Triggers.Contains(WorkflowStructure.Trigger.FirstAppLoad))
            {
                chkAppLaunch.Checked = true;
                radioFirstAppLaunch.Checked = true;

                radioFirstAppLaunch.Enabled = true;
                radioAppLaunch.Enabled = true;
            }
        }

        private void chkAppLaunch_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;

            if (sender == null)
                return;

            if (checkbox.Checked)
            {
                radioFirstAppLaunch.Enabled = true;
                radioAppLaunch.Enabled = true;
            }
            else
            {
                radioFirstAppLaunch.Enabled = false;
                radioAppLaunch.Enabled = false;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (chkAppLaunch.Checked)
            {
                if (radioFirstAppLaunch.Checked)
                {
                    controller.EnableTrigger(WorkflowStructure.Trigger.FirstAppLoad);
                    controller.DisableTrigger(WorkflowStructure.Trigger.AppLoad);
                } else if (radioAppLaunch.Checked) {
                    controller.DisableTrigger(WorkflowStructure.Trigger.FirstAppLoad);
                    controller.EnableTrigger(WorkflowStructure.Trigger.AppLoad);
                }
            }
            else
            {
                controller.DisableTrigger(WorkflowStructure.Trigger.FirstAppLoad);
                controller.DisableTrigger(WorkflowStructure.Trigger.AppLoad);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioFirstAppLaunch_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
