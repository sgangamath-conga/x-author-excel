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
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class ApttusAboutForm : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public ApttusAboutForm()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Apttus.XAuthor.Core.Constants.DESIGNER_PRODUCT_NAME;
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

        private void ApttusAboutForm_Load(object sender, EventArgs e)
        {
            //version.Text = Constants.PRODUCT_NAME + Constants.SPACE + Constants.PRODUCT_EDITION +
            //    Environment.NewLine + resourceManager.GetResource("ABOUT_version_Text")+" "+ ApttusCommandBarManager.GetInstance().GetVersion();

            version.Text = Constants.PRODUCT_NAME + Constants.SPACE + Globals.ThisAddIn.AppDesignerVersion +
                Environment.NewLine + resourceManager.GetResource("ABOUT_version_Text") + " " + ApttusCommandBarManager.GetInstance().GetVersion();



            StringBuilder sb = new StringBuilder();
            sb.Append(resourceManager.GetResource("ABOUT_lblAboutDescription_Text",true));

            lblAboutDescription.Text = sb.ToString();

            lblCopyrightInfo.Text = resourceManager.GetResource("ABOUT_lblCopyrightInfo_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
        }        
    }
}
