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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.Core
{
    public partial class ApplicationBrowserView : Form
    {
        ApplicationBrowserController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;

        public ApplicationBrowserView()
        {
            InitializeComponent();
            SetCultureData();
            //this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            //this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_Title");
            AppName.HeaderText = resourceManager.GetResource("COREAPPBROWSEVIEW_AppName_HeaderText");
            btnCancel.Text = resourceManager.GetResource("CORECOMMON_btnCancel_Text");
            btnSearch.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_btnSearch_Text");
            btnSelect.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_btnSelect_Text");
            chkMyApps.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_chkMyApps_Text");
            Id.HeaderText = resourceManager.GetResource("COREAPPBROWSEVIEW_Id_HeaderText");
            lblSearch.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_lblSearch_Text");
            ModifiedOn.HeaderText = resourceManager.GetResource("COREAPPBROWSEVIEW_ModifiedOn_HeaderText");
            ResultHeader.HeaderText = resourceManager.GetResource("COREAPPBROWSEVIEW_ResultHeader_HeaderText");
            lblTitle.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_lblTitle_Text");
        }
        public void SetController(ApplicationBrowserController controller)
        {
            if (!objectManager.IsDesigner())
                chkMyApps.Visible = false;
            this.controller = controller;
            dgvAppList.AutoGenerateColumns = false;
            controller.Search(string.Empty, false);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            controller.Search(txtSearch.Text, chkMyApps.Checked);
        }

        public void BindAppsGrid(DataTable dataTable)
        {
            dgvAppList.DataSource = dataTable;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvAppList.SelectedRows.Count == 1)
                controller.ApplicationSelected(dgvAppList.SelectedRows[0].Cells[Constants.ID_ATTRIBUTE].Value.ToString(),
                    dgvAppList.SelectedRows[0].Cells["AppName"].Value.ToString());
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dgvAppList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1)
                controller.ApplicationSelected(dgvAppList.Rows[e.RowIndex].Cells[Constants.ID_ATTRIBUTE].Value.ToString(),
                    dgvAppList.Rows[e.RowIndex].Cells["AppName"].Value.ToString());
        }

        //Even though the date internaly stores the year as YYYY, using formatting, the 
        //UI can have the format in YY.   
        private static void ShortFormDateFormat(DataGridViewCellFormattingEventArgs formatting)
        {
            if (formatting.Value != null)
            {
                try
                { 
                    DateTime theDate = DateTime.Parse(formatting.Value.ToString());
                    String dateString = theDate.ToString("MM/dd/yyyy HH:mm");
                    formatting.Value = dateString;
                    formatting.FormattingApplied = true;
                }
                catch (FormatException)
                {
                    // Set to false in case there are other handlers interested trying to 
                    // format this DataGridViewCellFormattingEventArgs instance.
                    formatting.FormattingApplied = false;
                }
            }
        }

        private void dgvAppList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvAppList.Columns[e.ColumnIndex].Name == "ModifiedOn")
            {
                ShortFormDateFormat(e);
            }
        }

        private void chkMyApps_CheckedChanged(object sender, EventArgs e)
        {
            controller.Search(txtSearch.Text, chkMyApps.Checked);
        }
    }
}
