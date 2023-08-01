using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using Apttus.XAuthor.AppDesigner;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class FieldsMissingViewer : UserControl
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DesignerAppSyncController controller;
        ConcurrentBag<MissingFieldItem> MissingFieldSource;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public FieldsMissingViewer(DesignerAppSyncController appSyncController)
        {
            InitializeComponent();
            SetCultureData();
            this.controller = appSyncController;
        }

        private void SetCultureData()
        {
            lblMissingFields.Text = resourceManager.GetResource("FieldsMissingViewer_lblMissingFields_Text");
            //lblMissingFieldsDescription.Text = resourceManager.GetResource("FieldsMissingViewer_lblMissingFieldsDescription_Text");
            btnRemove.Text = resourceManager.GetResource("COMMON_Remove_Text");
            Object.HeaderText = resourceManager.GetResource("CLEARACTION_AppObject_HeaderText");
            Field.HeaderText = resourceManager.GetResource("SMRETFIELDVIEW_RetrieveFieldName_HeaderText");
            DisplayMap.HeaderText = resourceManager.GetResource("COMMON_DisplayMap_Text");
            SaveMap.HeaderText = resourceManager.GetResource("RIBBON_btnSaveMap_Label");
            SearchFilters.HeaderText = resourceManager.GetResource("FieldsMissingViewer_SearchFilters_HeaderText");
            SearchFilters.HeaderText = resourceManager.GetResource("FieldsMissingViewer_SearchFilters_HeaderText");
            lblDoubleClicktoViewDetails.Text = resourceManager.GetResource("AppSync_DoubleClickToViewDetails_Text");
        }

        internal void RefreshGrid()
        {
            DgvMissingFields.AutoGenerateColumns = false;
            MissingFieldSource = controller.GetMissingFieldInfo();
            DgvMissingFields.DataSource = GetSource(MissingFieldSource);
            SetDescriptionText();
        }

        private void AddControl(Control ctrl)
        {
            try
            {
                tableLayoutPanel2.SuspendLayout();
                if (tableLayoutPanel2.Controls.Count > 1)
                    tableLayoutPanel2.Controls.RemoveAt(1);
                tableLayoutPanel2.Controls.Add(ctrl, 0, 0);
                ctrl.Dock = DockStyle.Fill;
                ctrl.Show();
            }
            finally
            {
                tableLayoutPanel2.ResumeLayout();
            }
        }

        void SetDescriptionText()
        {
            Label lblMissingFieldsDescription = new Label();
            if (MissingFieldSource.Count > 0)
            {
                lblMissingFieldsDescription.Text = string.Format(resourceManager.GetResource("FieldsMissingViewer_lblMissingFieldsDescription_Text"),resourceManager.CRMName);
                lblMissingFieldsDescription.AutoSize = true;
            }
            else
            {
                lblMissingFieldsDescription.Text = resourceManager.GetResource("AppSync_FieldsMissingViewer_NoData_Text");
                lblMissingFieldsDescription.AutoSize = false;
            }

            lblMissingFieldsDescription.Dock = DockStyle.Fill;
            AddControl(lblMissingFieldsDescription);
        }

        List<MissingFieldGridItem> GetSource(ConcurrentBag<MissingFieldItem> missingFields)
        {
            List<MissingFieldGridItem> source = new List<AppDesigner.MissingFieldGridItem>();

            foreach (MissingFieldItem missingField in missingFields)
            {
                source.Add(new MissingFieldGridItem()
                {
                    Field = string.Format("{0} ({1})", missingField.FieldName, missingField.FieldId),
                    Object = string.Format("{0} ({1})", missingField.ObjectName, missingField.ObjectID),
                    DisplayMap = missingField.FieldMissingInDisplayMaps.Count > 0 ? "Yes" : "No",
                    SaveMap = missingField.FieldMissingInSaveMaps.Count > 0 ? "Yes" : "No",
                    SearchFilters = missingField.FieldMissingInActions.Count > 0 ? "Yes" : "No",
                    MissingFieldObject = missingField
                });
            }

            return source;
        }

        private void DgvFieldTypeMismatch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                MissingFieldGridItem missingFieldGridItem = DgvMissingFields.Rows[e.RowIndex].DataBoundItem as MissingFieldGridItem;
                if (missingFieldGridItem != null)
                {
                    MissingFieldItem missingField = missingFieldGridItem.MissingFieldObject;
                    ApttusMessageUtil.ShowInfo(getDetails(missingField), Constants.PRODUCT_NAME);
                }
            }
        }

        string getDetails(MissingFieldItem fieldInfo)
        {
            StringBuilder details = new StringBuilder();

            details.Append(string.Format(resourceManager.GetResource("AppSync_FieldMissingViewer_FieldInfo_Text"), fieldInfo.FieldName) + "\n");
            if (fieldInfo.FieldMissingInDisplayMaps.Count > 0)
            {
                details.Append(string.Format("\n\n{0}: ", resourceManager.GetResource("COMMON_DisplayMap_Text")));
                foreach (var displayMap in fieldInfo.FieldMissingInDisplayMaps)
                {
                    details.Append(displayMap.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            if (fieldInfo.FieldMissingInSaveMaps.Count > 0)
            {
                details.Append(string.Format("\n\n{0} ", resourceManager.GetResource("COMMON_SaveMap_Text")));
                foreach (var saveMap in fieldInfo.FieldMissingInSaveMaps)
                {
                    details.Append(saveMap.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            if (fieldInfo.FieldMissingInActions.Count > 0)
            {
                details.Append(string.Format("\n\n{0}: ", resourceManager.GetResource("COMMON_Actions_Text")));
                foreach (var action in fieldInfo.FieldMissingInActions)
                {
                    details.Append(action.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            return details.ToString();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<MissingFieldItem> removedFields = new List<MissingFieldItem>();
            foreach (DataGridViewRow dr in DgvMissingFields.Rows)
            {
                DataGridViewCheckBoxCell chkSelected = dr.Cells[0] as DataGridViewCheckBoxCell;
                if (chkSelected.Value != null && chkSelected.Value.ToString() == "1")
                {
                    MissingFieldGridItem missingField = dr.DataBoundItem as MissingFieldGridItem;
                    removedFields.Add(missingField.MissingFieldObject);
                }
            }
            controller.RemoveFields(removedFields);
            RefreshGrid();
        }
    }

    class MissingFieldGridItem
    {
        public string Object { get; set; }
        public string Field { get; set; }
        public string DisplayMap { get; set; }
        public string SaveMap { get; set; }
        public string SearchFilters { get; set; }
        public MissingFieldItem MissingFieldObject { get; set; }
    }

}
