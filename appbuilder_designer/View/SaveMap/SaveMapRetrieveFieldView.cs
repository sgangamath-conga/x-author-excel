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
    public partial class SaveMapRetrieveFieldView : Form
    {
        SaveMapRetrieveFieldController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveMapRetrieveFieldView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("RETRIEVEMAP_Title");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnInclude.Text = resourceManager.GetResource("COMMON_Apply_Text");
            chkSelectAll.Text = resourceManager.GetResource("COMMON_SelectAll_Text");
            DesignerLocation.HeaderText = resourceManager.GetResource("COMMON_Location_Text");
            lblFilter.Text = resourceManager.GetResource("COMMON_FilterBy_Text");
            RetrieveFieldName.HeaderText = resourceManager.GetResource("SMRETFIELDVIEW_RetrieveFieldName_HeaderText");
            RetrieveMapName.HeaderText = resourceManager.GetResource("COMMON_DisplayMap_Text");
            Type.HeaderText = resourceManager.GetResource("COMMON_Type_Text").Replace(':',' ');
        }
        public void SetController(SaveMapRetrieveFieldController controller)
        {
            this.controller = controller;
        }

        public void BindFilterBy(List<SaveMapRetrieveMap> maps)
        {
            cmbFilterBy.ValueMember = "RetrieveMapId";
            cmbFilterBy.DisplayMember = "RetrieveMapName";
            cmbFilterBy.DataSource = maps;
        }

        public void BindGrid(List<SaveMapRetrieveField> model)
        {
            //Filter ID field in Add retrieve field and Add other field popup
            //model = model.Where(id => id.RetrieveFieldId != Constants.ID_ATTRIBUTE).ToList();
    
            dgvSaveMapRetrieveField.AutoGenerateColumns = false;
            dgvSaveMapRetrieveField.DataSource = model;
            dgvSaveMapRetrieveField.Refresh();
        }

        internal void SetInitialRetrieveMap()
        {
            cmbFilterBy.SelectedItem = cmbFilterBy.Items[0];
        }

        private void cmbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller.RetrieveMapSelectionChange((SaveMapRetrieveMap)cmbFilterBy.SelectedItem);
            chkSelectAll.Checked = false;
        }

        private void btnInclude_Click(object sender, EventArgs e)
        {
            controller.AddRetrieveFieldsToSaveMap();
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
    }
}
