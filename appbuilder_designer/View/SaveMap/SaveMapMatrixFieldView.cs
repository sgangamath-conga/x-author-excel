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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class SaveMapMatrixFieldView : Form
    {
        SaveMapMatrixFieldController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveMapMatrixFieldView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("MATRIXFIELDVIEW_Title");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnInclude.Text = resourceManager.GetResource("COMMON_Apply_Text");
            chkSelectAll.Text = resourceManager.GetResource("COMMON_SelectAll_Text");
            lblFilter.Text = resourceManager.GetResource("COMMON_FilterBy_Text");
            MatrixComponentName.HeaderText = resourceManager.GetResource("COMMON_SectionName_Text");
            MatrixMapName.HeaderText = resourceManager.GetResource("COMMON_MapName_HeaderText");
            MatrixObjectName.HeaderText = resourceManager.GetResource("COMMON_ObjectName_Text");
            Type.HeaderText = resourceManager.GetResource("COMMON_Type_Text").Replace(':',' ');
        }
        public void SetController(SaveMapMatrixFieldController controller)
        {
            this.controller = controller;
        }

        public void BindFilterBy(List<SaveMapRetrieveMap> maps)
        {
            cmbFilterBy.ValueMember = "RetrieveMapId";
            cmbFilterBy.DisplayMember = "RetrieveMapName";
            cmbFilterBy.DataSource = maps;
        }

        public void BindGrid(List<SaveMapMatrixField> model)
        {          
            dgvSaveMapRetrieveField.AutoGenerateColumns = false;
            dgvSaveMapRetrieveField.DataSource = model;
            dgvSaveMapRetrieveField.Refresh();
        }

        internal void SetInitialMatrixMap()
        {
            cmbFilterBy.SelectedItem = cmbFilterBy.Items[0];
        }

        private void cmbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.MatrixMapSelectionChange((SaveMapRetrieveMap)cmbFilterBy.SelectedItem);
            chkSelectAll.Checked = false;
        }

        private void btnInclude_Click(object sender, EventArgs e)
        {
            controller.AddMatrixFieldsToSaveMap();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            controller.ToggleSelectAll(chkSelectAll.Checked);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            controller.Close();
        }

        private void dgvSaveMapRetrieveField_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvSaveMapRetrieveField.Columns[e.ColumnIndex].Name == "Type")
            {
                ObjectTypeLabel(e);
           }
        }

        internal void ObjectTypeLabel(DataGridViewCellFormattingEventArgs objType)
        {
            if (objType.Value != null)
            {
                try
                {
                    string fieldObjectType = Utils.GetEnumDescription(objType.Value, string.Empty);
                    objType.Value = fieldObjectType;
                    objType.FormattingApplied = true;
                }
                catch (FormatException)
                {
                    // Set to false in case there are other handlers interested trying to 
                    // format this DataGridViewCellFormattingEventArgs instance.
                    objType.FormattingApplied = false;
                }
            }
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

    }
}
