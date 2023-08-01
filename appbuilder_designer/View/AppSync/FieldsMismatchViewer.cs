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
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class FieldsMismatchViewer : UserControl
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DesignerAppSyncController appValidator;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public FieldsMismatchViewer(DesignerAppSyncController appValidator)
        {
            InitializeComponent();
            SetCultureData();
            this.appValidator = appValidator;
        }

        private void SetCultureData()
        {
            lblFieldTypeMismatch.Text = resourceManager.GetResource("FieldsMismatchViewer_lblFieldTypeMismatch_Text");
            lblFieldTypeMismatchDescription.Text = string.Format(resourceManager.GetResource("FieldsMismatchViewer_lblFieldTypeMismatchDescription_Text"),resourceManager.CRMName);
            btnApply.Text = resourceManager.GetResource("FieldsMismatchViewer_SyncFieldButton_Text");
            Object.HeaderText = resourceManager.GetResource("CLEARACTION_AppObject_HeaderText");
            Field.HeaderText = resourceManager.GetResource("SMRETFIELDVIEW_RetrieveFieldName_HeaderText");
            InApp.HeaderText = resourceManager.GetResource("FieldsMismatchViewer_InApp_Text");
            InSalesforce.HeaderText = string.Format(resourceManager.GetResource("FieldsMismatchViewer_lnSalesforce_Text"),resourceManager.CRMName);
            lblDoubleClicktoViewDetails.Text = resourceManager.GetResource("AppSync_DoubleClickToViewDetails_Text");
        }
        internal void RefreshGrid()
        {
            dgvFieldTypeMismatch.AutoGenerateColumns = false;
            dgvFieldTypeMismatch.DataSource = GetSource(appValidator.GetDataTypeMismatchFieldInfo());
            SetDescriptionText();
        }
        void SetDescriptionText()
        {
            lblFieldTypeMismatchDescription.Text = dgvFieldTypeMismatch.RowCount > 0 ?
                string.Format(resourceManager.GetResource("FieldsMismatchViewer_lblFieldTypeMismatchDescription_Text"),resourceManager.CRMName) : string.Format(resourceManager.GetResource("AppSync_FieldsMismatchViewer_NoData_Text"),resourceManager.CRMName);
        }
        List<FieldTypeMismatchGridItem> GetSource(IEnumerable<DataTypeChangeInfo> dataTypeMismatchFields)
        {
            List<FieldTypeMismatchGridItem> source = new List<AppDesigner.FieldTypeMismatchGridItem>();

            foreach (var field in dataTypeMismatchFields)
            {
                ApttusObject appObj = applicationDefinitionManager.GetAppObjectById(field.AppObjectId).FirstOrDefault();
                ApttusField appField = appObj.GetField(field.FieldId);
                source.Add(new FieldTypeMismatchGridItem()
                {
                    FieldID = appField.Id,
                    AppObjectID = appObj.UniqueId.ToString(),
                    Object = string.Format("{0} ({1})", appObj.Name, appObj.Id),
                    Field = string.Format("{0} ({1})", appField.Name, appField.Id),
                    AppDataType = field.From.ToString(),
                    SFDataType = field.To.ToString(),
                    MismatchFieldObject = field
                });
            }
            return source;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            List<DataTypeChangeInfo> mismatchFieldObjects = new List<DataTypeChangeInfo>();
            foreach (DataGridViewRow dr in dgvFieldTypeMismatch.Rows)
            {
                DataGridViewCheckBoxCell chkSelected = dr.Cells[0] as DataGridViewCheckBoxCell;
                if (chkSelected.Value != null && chkSelected.Value.ToString() == "1")
                {
                    FieldTypeMismatchGridItem mismatchField = dr.DataBoundItem as FieldTypeMismatchGridItem;
                    mismatchFieldObjects.Add(mismatchField.MismatchFieldObject);
                }
            }
            appValidator.ChangeDatatypeofFields(mismatchFieldObjects);
            RefreshGrid();
        }

        private void DgvFieldTypeMismatch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                FieldTypeMismatchGridItem missingFieldGridItem = dgvFieldTypeMismatch.Rows[e.RowIndex].DataBoundItem as FieldTypeMismatchGridItem;
                if (missingFieldGridItem != null)
                {
                    DataTypeChangeInfo mismatchField = missingFieldGridItem.MismatchFieldObject;
                    ApttusMessageUtil.ShowInfo(getDetails(mismatchField), Constants.PRODUCT_NAME);
                }
            }
        }

        string getDetails(DataTypeChangeInfo fieldInfo)
        {
            StringBuilder details = new StringBuilder();
            details.Append(string.Format(resourceManager.GetResource("AppSync_FieldMismatch_FieldInfo_Text"), fieldInfo.SynchedField.Name) + "\n");
            if (fieldInfo.FieldUsedInDisplayMaps.Count > 0)
            {
                details.Append(string.Format("\n\n{0}: ", resourceManager.GetResource("COMMON_DisplayMap_Text")));
                foreach (var displayMap in fieldInfo.FieldUsedInDisplayMaps)
                {
                    details.Append(displayMap.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            if (fieldInfo.FieldsUsedInSaveMaps.Count > 0)
            {
                details.Append(string.Format("\n\n{0} ", resourceManager.GetResource("COMMON_SaveMap_Text")));
                foreach (var saveMap in fieldInfo.FieldsUsedInSaveMaps)
                {
                    details.Append(saveMap.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            if (fieldInfo.FieldUsedInActions.Count > 0)
            {
                details.Append(string.Format("\n\n{0}: ", resourceManager.GetResource("COMMON_Actions_Text")));
                foreach (var action in fieldInfo.FieldUsedInActions)
                {
                    details.Append(action.Name + ", ");
                }
                details.Remove(details.Length - 2, 1);
            }
            return details.ToString();
        }
    }

    class FieldTypeMismatchGridItem
    {
        public string FieldID { get; set; }
        public string AppObjectID { get; set; }
        public string Object { get; set; }
        public string Field { get; set; }
        public string AppDataType { get; set; }
        public string SFDataType { get; set; }
        public DataTypeChangeInfo MismatchFieldObject { get; set; }
    }
}
