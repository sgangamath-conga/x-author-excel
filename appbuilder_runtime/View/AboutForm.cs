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
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime.View
{
    public partial class AboutForm : Form
    {

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public AboutForm()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Apttus.XAuthor.Core.Constants.RUNTIME_PRODUCT_NAME;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            //version.Text = Constants.PRODUCT_NAME + Constants.SPACE + Constants.PRODUCT_EDITION +
            //    Environment.NewLine + resourceManager.GetResource("ABOUT_version_Text") + " " + ApttusCommandBarManager.GetInstance().GetVersion();

            version.Text = Constants.PRODUCT_NAME + Constants.SPACE + Globals.ThisAddIn.AppVersion +
                Environment.NewLine + resourceManager.GetResource("ABOUT_version_Text") + " " + ApttusCommandBarManager.GetInstance().GetVersion();

            StringBuilder sb = new StringBuilder();
            sb.Append(resourceManager.GetResource("ABOUT_lblAboutDescription_Text",true));

            lblAboutDescription.Text = sb.ToString();

            lblCopyrightInfo.Text = resourceManager.GetResource("ABOUT_lblCopyrightInfo_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
        }
    }
}
