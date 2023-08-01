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

namespace Apttus.XAuthor.AppRuntime
{
    public partial class FileInputDialog : Form
    {
        public string FilePath { get; private set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public FileInputDialog()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("FILEINPUTDIALOG_label1_Text");
            btnCancel.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnCancel_Text");
            lblTitle.Text = resourceManager.GetResource("FILEINPUT_lblTitle_Text");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Microsoft Excel(xlsx)|*.xlsx";
            dialog.Title = resourceManager.GetResource("DATATRANSVW_Transfer_Msg");

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dialog.FileName;
                txtFilePath.Text = dialog.FileName;
                Close();
            }
        }
    }
}
