/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Data;
using System.Windows.Forms;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class CheckOutView : Form
    {
        CheckOutViewController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ICheckOutProvider checkOutProvider;

        public CheckOutView(ICheckOutProvider checkOutProvider)
        {
            InitializeComponent();
            SetCultureData();
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.RUNTIME_PRODUCT_NAME;
            this.checkOutProvider = checkOutProvider;
        }
        private void SetCultureData()
        {
            AppName.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_AppName_HeaderText");
            btnCancel.Text = resourceManager.GetResource("CORECOMMON_btnCancel_Text");
            btnSelect.Text = resourceManager.GetResource("CHECKOUTVIEW_btnSelect_Text");
            DocId.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_DocId_HeaderText");
            LastModifiedBy.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_LastModifiedBy_HeaderText");
            LastModifiedById.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_LastModifiedById_HeaderText");
            lblSearch.Text = resourceManager.GetResource("CHECKOUTVIEW_lblSearch_Text");
            Locked.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_Locked_HeaderText");
            ModifiedOn.HeaderText = resourceManager.GetResource("CHECKOUTVIEW_ModifiedOn_HeaderText");
        }
        public void SetController(CheckOutViewController controller)
        {
            this.controller = controller;
            dgvAppFiles.AutoGenerateColumns = false;
            controller.Search();
            SetColumnAlignment();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvAppFiles.SelectedRows.Count == 1)
                controller.ApplicationSelected(dgvAppFiles.SelectedRows[0].Cells[DocId.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[Id.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[LastModifiedById.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[LastModifiedBy.Name].Value.ToString(),
                    Convert.ToBoolean(dgvAppFiles.SelectedRows[0].Cells[Locked.Name].Value));
        }

        public void BindAppsGrid(DataTable dataTable)
        {
            checkOutProvider.SetDataPropertyOfGrid(ref dgvAppFiles);
            dgvAppFiles.DataSource = dataTable;
        }

        private void dgvAppFiles_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1)
                controller.ApplicationSelected(dgvAppFiles.Rows[e.RowIndex].Cells[DocId.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[Id.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[LastModifiedById.Name].Value.ToString(),
                    dgvAppFiles.SelectedRows[0].Cells[LastModifiedBy.Name].Value.ToString(),
                    Convert.ToBoolean(dgvAppFiles.SelectedRows[0].Cells[Locked.Name].Value));
        }

        private void dgvAppFiles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvAppFiles.Columns[e.ColumnIndex].Name == "ModifiedOn")
            {
                ShortFormDateFormat(e);
            }
            else if (dgvAppFiles.Columns[e.ColumnIndex].Name == "Locked")
            {
                if (e.Value.ToString().ToUpper().Equals("TRUE"))
                    e.Value = "Yes";
                else if (e.Value.ToString().ToUpper().Equals("FALSE"))
                    e.Value = "No";
            }
        }

        private static void ShortFormDateFormat(DataGridViewCellFormattingEventArgs formatting)
        {
            if (formatting.Value != null)
            {
                try
                {
                    DateTime theDate = DateTime.Parse(formatting.Value.ToString());
                    String dateString = theDate.ToString("MM/dd/yyyy HH:mm:ss");
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

        internal void SetColumnAlignment()
        {
            foreach (DataGridViewColumn col in dgvAppFiles.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }
    }
}
