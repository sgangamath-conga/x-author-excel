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
    public partial class PermissionUI : ShadowWindowForm
    {
        private UserProfileController  Controller;
        public PermissionUI(UserProfileController ctrller, List<ApttusUserProfile> Profiles )
        {
            InitializeComponent();
            Controller = ctrller;
            CenterToScreen();
            Controller.UI = this;
            Controller.LoadData(Profiles);
            CenterToScreen();
            XlationUtil xl = new XlationUtil();
            //xl.XlateAll(this);

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        public CheckedListBox lstPermissionUI
        {
            get { return this.lstUserProfile; }
        }

        
        public List<ApttusUserProfile> Profiles
        {
            get;
            set;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Controller.SaveCheckedtems();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstUserProfile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


      
    }
}
