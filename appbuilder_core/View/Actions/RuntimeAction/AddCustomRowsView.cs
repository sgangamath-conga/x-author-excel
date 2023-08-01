/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.Core
{
    public partial class AddCustomRowsView : Form
    {
        AddCustomRowsController controller;
        private ApttusResourceManager resourceManager;
        public AddCustomRowsView()
        {
            resourceManager = ApttusResourceManager.GetInstance;
            InitializeComponent();
            SetCultureData();

            //this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
            txtRows.Select();
        }


        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("COREADDROW_CustomRowCap_ErrorMsg");
            btnAdd.Text = resourceManager.GetResource("COREADDCUSTROWVIEW_btnAdd_Text");
            btnCancel.Text = resourceManager.GetResource("CORECOMMON_btnCancel_Text");
            lblMessage.Text = resourceManager.GetResource("COREADDCUSTROWVIEW_lblMessage_Text");
            lblRows.Text = resourceManager.GetResource("COREADDCUSTROWVIEW_lblRows_Text");
        }

        public void SetController(AddCustomRowsController controller)
        {
            this.controller = controller;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int result;
            if (int.TryParse(txtRows.Text, out result) && (result > 0 & result < 10001))
            {
                this.controller.result = result;
                this.Dispose();
            }
            else
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("COREADDROW_CustomRow_ErrorMsg"), resourceManager.GetResource("COREADDROW_CustomRowCap_ErrorMsg"), this.Handle.ToInt32());
                txtRows.Clear();
                txtRows.Select();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
