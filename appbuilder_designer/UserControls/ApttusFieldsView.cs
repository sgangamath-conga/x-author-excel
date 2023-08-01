/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ApttusFieldsView : UserControl
    {
        ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
        List<ApttusFieldDS> lsfields = new List<ApttusFieldDS>();
        List<string> readOnlyFields = new List<string>();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public ApttusObject AppObject { get; set; }

        public ApttusFieldsView()
        {
            InitializeComponent();
            addImageColumnforDynamicsCRM();
        }

        public void SetCultureData()
        {
            dcDatatype.HeaderText = resourceManager.GetResource("COMMON_DataType_Text");
            dcId.HeaderText = resourceManager.GetResource("COMMON_FieldId_Text");
            dcName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
            lblSearchFields.Text = resourceManager.GetResource("COMMON_SearchField_Text") + " :";
            lnkClear.Text = resourceManager.GetResource("COMMON_Clear_Text");
        }

        private void addImageColumnforDynamicsCRM()
        {
            DataGridViewImageColumn configColumn = new DataGridViewImageColumn();
            configColumn.Name = "Configure";
            configColumn.HeaderText = "";
            configColumn.FillWeight = 20;
            configColumn.Width = 10;
            configColumn.MinimumWidth = 5;
            configColumn.ReadOnly = true;
            this.dgvFields.Columns.Add(configColumn);
            dgvFields.Columns["Configure"].DefaultCellStyle.NullValue = null;
        }

        private void dgvFields_DataSourceChanged(object sender, EventArgs e)
        {
            List<ApttusFieldDS> lsFilterfields = new List<ApttusFieldDS>();
            lsFilterfields = dgvFields.DataSource as List<ApttusFieldDS>;            

            // Disable "Not supported type"            
            if (lsFilterfields != null)
            {
                List<DataGridViewRow> UniqueRowsNotSupportedTypes = (from f in lsFilterfields.Where(fld => fld.Datatype == Datatype.NotSupported)
                                                                     from r in dgvFields.Rows.Cast<DataGridViewRow>().Where(row => row.DataBoundItem == f)
                                                                     select r).ToList();

                // check each cell for Not supported type and make it readonly
                foreach (DataGridViewRow r in UniqueRowsNotSupportedTypes)
                {
                    r.ReadOnly = true;
                    ApttusField field = r.DataBoundItem as ApttusField;
                    if (field.Datatype == Datatype.NotSupported)
                        r.Cells["dcCheck"].ToolTipText = resourceManager.GetResource("APTTUSFIELDVIEW_dgvFields_DataSourceChangedNotSupported_Msg");
                }
            }
            // ID and Name
            var UniqueRowsIdName = (from DataGridViewRow r in dgvFields.Rows
                                    where readOnlyFields.Contains(r.Cells[1].Value)
                                    select r).ToList();

            foreach (DataGridViewRow r in UniqueRowsIdName)
            {
                r.ReadOnly = true;
                ApttusField field = r.DataBoundItem as ApttusField;
                if (field.Id.Equals(AppObject.IdAttribute) || field.Id.Equals(AppObject.NameAttribute))
                    r.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                else //All fields other than name and id field will display a tooltip message.
                    r.Cells[dcCheck.Name].ToolTipText = resourceManager.GetResource("APTTUSFIELDVIEW_dgvFields_DataSourceChangedUsed_Msg");
            }

            // This flag determine whether this is basic edition or not
            if (commandBarManager.IsBasicEdition())
            {
                if (lsFilterfields != null)
                {
                    List<DataGridViewRow> UniqueRows = (from f in lsFilterfields.Where(fld => fld.Datatype == Datatype.Attachment || fld.Datatype == Datatype.Rich_Textarea)
                                                        from r in dgvFields.Rows.Cast<DataGridViewRow>().Where(row => row.DataBoundItem == f)
                                                        select r).ToList();


                    // check each cell for rich text and attachment type and make it readonly
                    foreach (DataGridViewRow r in UniqueRows)
                    {
                        r.ReadOnly = true;
                        ApttusField field = r.DataBoundItem as ApttusField;
                        if (field.Datatype == Datatype.Attachment)
                            r.Cells["dcCheck"].ToolTipText = resourceManager.GetResource("APTTUSFIELDVIEW_dgvFields_DataSourceChangedEdition_Msg");
                        else if(field.Datatype == Datatype.Rich_Textarea)
                            r.Cells["dcCheck"].ToolTipText = resourceManager.GetResource("APTTUSFIELDVIEW_dgvFields_DataSourceChangedRichText_Msg");
                    }
                }
            }

            foreach (DataGridViewRow row in dgvFields.Rows)
            {
                if ((bool)row.Cells[dcCheck.Name].Value)
                {
                    ApttusField field = row.DataBoundItem as ApttusField;
                    if (field.MultiLookupObjects != null && field.MultiLookupObjects.Count > 1)
                        row.Cells["Configure"].Value = Properties.Resources.config_icon;
                }
            }
        }

        public void UpdateDataSource(object dataSource)
        {
            if (dataSource != null)
            {
                dgvFields.DataSource = dataSource;
                lsfields = dgvFields.DataSource as List<ApttusFieldDS>;
            }
            else
            {
                lsfields = null;
                readOnlyFields.Clear();
                dgvFields.DataSource = null;
            }
        }

        public void RenderFieldsGrid(ref List<ApttusFieldDS> dataSource, List<string> readOnlyFieldList)
        {
            readOnlyFields = readOnlyFieldList;
            txtSearchFields.Text = string.Empty;
            dgvFields.AutoGenerateColumns = false;
            dgvFields.DataSource = dataSource;
            lsfields = dgvFields.DataSource as List<ApttusFieldDS>;
            dgvFields.ClearSelection();
        }

        private void txtSearchFields_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchString = txtSearchFields.Text.Trim();
                var filterResults = (from x in lsfields
                                     where x.Id.ToLower().Contains(searchString.ToLower()) || x.Name.ToLower().Contains(searchString.ToLower())
                                     select x).ToList();
                dgvFields.DataSource = filterResults;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void lnkClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtSearchFields.Text = string.Empty;
            dgvFields.DataSource = lsfields;
            dgvFields.ClearSelection();

            txtSearchFields.Focus();
        }

        private void dgvFields_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0)
                return;

            DataGridViewColumn newColumn = dgvFields.Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = dgvFields.SortedColumn;
            ListSortDirection direction = ListSortDirection.Ascending;

            // Decide sort direction
            if (newColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                direction = ListSortDirection.Descending;
            else if (newColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                direction = ListSortDirection.Ascending;
            else if (newColumn.HeaderCell.SortGlyphDirection == SortOrder.None)
                direction = ListSortDirection.Ascending;

            // Sort the selected column.
            //dgvFields.Sort(newColumn, direction);
            List<ApttusFieldDS> lsfields = dgvFields.DataSource as List<ApttusFieldDS>;
            List<ApttusFieldDS> lstFieldsOrderByColumn = new List<ApttusFieldDS>();
            if (e.ColumnIndex == 1)
            {
                if (direction == ListSortDirection.Ascending)
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenBy(f => f.Id).ToList();
                else
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenByDescending(f => f.Id).ToList();
            }
            else if (e.ColumnIndex == 2)
            {
                if (direction == ListSortDirection.Ascending)
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenBy(f => f.Name).ToList();
                else
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenByDescending(f => f.Name).ToList();
            }
            else if (e.ColumnIndex == 3)
            {
                if (direction == ListSortDirection.Ascending)
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenBy(f => f.Datatype.ToString()).ToList();
                else
                    lstFieldsOrderByColumn = lsfields.OrderByDescending(f => f.Included).ThenByDescending(f => f.Datatype.ToString()).ToList();
            }

            dgvFields.DataSource = lstFieldsOrderByColumn;

            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
        }

        private void dgvFields_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != 4)
                return;

            string objectId = AppObject.Id;

            ApttusField applicationApptusField = null;
            ApttusFieldDS appField = lsfields.FirstOrDefault(f => f.Id == dgvFields[1, e.RowIndex].Value.ToString());
            //ApttusField field;

            //if (appObjectwithAllfields != null)
            //    field = appObjectwithAllfields.Fields.FirstOrDefault(f => f.Id == appField.Id);

            var tempObject = ApplicationDefinitionManager.GetInstance.GetAppObjectById(objectId);
            ApttusObject appObject = null;
            if (tempObject != null && tempObject.Count > 0)
                appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(tempObject[0].UniqueId);

            if (appObject != null)
            {
                if (appObject.Fields.Any(f => f.Id == appField.Id))
                    applicationApptusField = appObject.Fields.FirstOrDefault(f => f.Id == appField.Id);
            }
            bool isChecked = (Boolean)dgvFields[0, e.RowIndex].FormattedValue;
            if (isMultiLookupFieldwithObjects(applicationApptusField, appField) && isChecked)
            {
                LookupObjectSelectionUI UI = LookupObjectSelectionUI.GetLookupObjectSelectionUI;
                UI.appApttusFieldDS = appField;
                UI.standardApttusField = applicationApptusField;
                UI.appObjectId = objectId;
                UI.Show();
            }
        }

        private void dgvFields_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvFields.IsCurrentCellDirty)
            {
                dgvFields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                ApttusField applicationApptusField = null;

                var tempObject = ApplicationDefinitionManager.GetInstance.GetAppObjectById(AppObject.Id);

                ApttusObject appObject = null;
                if (tempObject != null && tempObject.Count > 0)
                    appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(tempObject[0].UniqueId);

                ApttusFieldDS appField = lsfields.FirstOrDefault(f => f.Id == dgvFields[1, e.RowIndex].Value.ToString());
                if (appObject != null)
                {
                    if (appObject.Fields.Any(f => f.Id == appField.Id))
                        applicationApptusField = appObject.Fields.FirstOrDefault(f => f.Id == appField.Id);
                }
                bool isChecked = (Boolean)dgvFields[0, e.RowIndex].FormattedValue;

                if (isMultiLookupFieldwithObjects(applicationApptusField, appField) && isChecked)
                    dgvFields["Configure", e.RowIndex].Value = Properties.Resources.config_icon;
                else
                    dgvFields["Configure", e.RowIndex].Value = null;
            }
        }

        private bool isMultiLookupFieldwithObjects(ApttusField standardAppField, ApttusFieldDS appField)
        {
            bool blnMultiLookupField = false;

            if (appField.MultiLookupObjects != null && appField.MultiLookupObjects.Count > 0)
                blnMultiLookupField = true;
            if (standardAppField != null && standardAppField.MultiLookupObjects != null && standardAppField.MultiLookupObjects.Count > 0)
                blnMultiLookupField = true;

            return blnMultiLookupField;

        }

    }
}
