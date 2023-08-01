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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ImportAppView : Form
    {
        ImportAppController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ImportAppView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("IMPORTAPPCTL_Title");
            btnBrowseApp.Text = resourceManager.GetResource("IMPORTAPPVIEW_btnBrowseApp_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnImport.Text = resourceManager.GetResource("COMMON_Import_Text");
            gbAppDetails.Text = resourceManager.GetResource("COMMON_AppDetails_Text");
            lblAppName.Text = resourceManager.GetResource("IMPORTAPPVIEW_lblAppName_Text");
        }
        public void SetController(ImportAppController controller)
        {
            this.controller = controller;
        }

        public void PopulateAppDetails(string AppName, string AppVersion, string AppFileName)
        {
            txtAppFileName.Text = AppFileName;
            txtAppName.Text = AppName;
        }

        private void btnBrowseApp_Click(object sender, EventArgs e)
        {
            string ImportDirectory = string.Empty;

            // 1. Find the .app file
            OpenFileDialog ImportDialog = new OpenFileDialog();
            ImportDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ImportDialog.Filter = "X-Author Application|*.app";
            ImportDialog.Title = resourceManager.GetResource("Application_Import_Msg");

            if (ImportDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 2. Extract App
                controller.ExtractApp(ImportDialog.FileName);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            controller.ImportApp(txtAppName.Text, "");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ImportAppView_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.ClearImportDirectory();
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
        
    }
}
