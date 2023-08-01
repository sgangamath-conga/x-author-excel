using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class MatrixFieldView : Form
    {
        internal MatrixRowObject rowObj;
        internal MatrixColumnObject colObj;
        MatrixDataObject dataObj;
        MatrixViewController controller;
        FormOpenMode Mode;
        int Row, Column;

        private string componentName;
        private string sortFieldId, colLookupId, rowLookupId;

        internal MatrixFieldGridModel GridModel { get; set; }

        internal List<MatrixComponent> Components { get; set; }

        internal Guid AppObjectUniqueID { get; set; }
        internal string ObjectName { get; set; }
        internal string FieldName { get; set; }
        internal string FieldID { get; set; }
        internal MatrixField EditingMatrixField { get; set; }

        private List<string> rowLookupValues, colLookupValues;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public MatrixFieldView()
        {
            InitializeComponent();
            SetCultureData();

            rowLookupValues = new List<string>();
            colLookupValues = new List<string>();

            Mode = FormOpenMode.Create;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnOK.Text = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Text");
            label1.Text = resourceManager.GetResource("MATRIXFIELDVIEW_label1_Text");
            label2.Text = resourceManager.GetResource("COMMON_ObjectName_Text") + " :";
            label3.Text = resourceManager.GetResource("COMMON_FieldName_Text") + " : ";
            rboCol.Text = resourceManager.GetResource("MATRIXFIELDVIEW_rboCol_Text");
            rboData.Text = resourceManager.GetResource("MATRIXFIELDVIEW_rboData_Text");
            rboRow.Text = resourceManager.GetResource("MATRIXFIELDVIEW_rboRow_Text");
        }

        public MatrixFieldView(FormOpenMode mode, MatrixViewController ctr, int row, int column)
            : this()
        {
            Mode = mode;
            controller = ctr;
            Row = row;
            Column = column;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblObjectName.Text = ObjectName;
            lblFieldName.Text = FieldName;

            if (Mode == FormOpenMode.Create)
                ValidateMatrixEntity();

            if (Mode == FormOpenMode.Edit)
                LoadModel();
        }

        private void LoadModel()
        {
            switch (GridModel.EntityType)
            {
                case MatrixEntity.Row:
                    rboRow.Checked = true;
                    rowObj.RenderingType = GridModel.RenderType;
                    rowObj.SortFieldId = sortFieldId;
                    rowObj.ValueType = GridModel.FieldValueType;
                    rboData.Enabled = false;
                    break;
                case MatrixEntity.Column:
                    rboCol.Checked = true;
                    colObj.RenderingType = GridModel.RenderType;
                    colObj.ValueType = GridModel.FieldValueType;
                    colObj.SortFieldId = sortFieldId;
                    rboData.Enabled = false;
                    break;
                case MatrixEntity.GroupedColumn:
                    rboCol.Checked = true;
                    colObj.RenderingType = GridModel.RenderType;
                    colObj.ValueType = GridModel.FieldValueType;
                    colObj.SortFieldId = sortFieldId;
                    rboCol.Enabled = rboRow.Enabled = rboData.Enabled = false;
                    colObj.GroupedField = true;
                    break;
                case MatrixEntity.Data:
                    rboData.Checked = true;
                    dataObj.ComponentName = componentName;
                    rboCol.Enabled = rboRow.Enabled = false;
                    dataObj.ColumnLookupId = colLookupId;
                    dataObj.RowLookupId = rowLookupId;
                    break;
            }
            rboRow.Enabled = rboCol.Enabled = rboData.Enabled = false;
        }
        
        private GroupedColumnValidation ValidateGroupedField()
        {
            GroupedColumnValidation res = controller.ValidateGroupedField(Row, Column, AppObjectUniqueID, FieldID);
            if (res == GroupedColumnValidation.GroupedFieldDuplication)
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("MATRIXFLDVW_GroupedField_Duplication_ErrorMsg"), FieldName), resourceManager.GetResource("MATRIXFLDVW_GroupedFieldCap_ErrorMsg"));
            else if (res == GroupedColumnValidation.NoRelationExistsWithSiblingField)
                ApttusMessageUtil.ShowError(resourceManager.GetResource("MATRIXFLDVW_GroupedField_ErrorMsg"), resourceManager.GetResource("MATRIXFLDVW_GroupedFieldCap_ErrorMsg"));
            else if(res == GroupedColumnValidation.ColumnTypeIsStatic)
                ApttusMessageUtil.ShowError(resourceManager.GetResource("MATRIXFLDVW_GroupedField_ErrorMsg"), resourceManager.GetResource("MATRIXFLDVW_GroupedFieldCap_ErrorMsg"));
            return res;
        }

        private void ValidateMatrixEntity()
        {
            Guid rowId, colId;

            if (controller != null && Row != -1 && Column != -1)
            {
                controller.GetIntersectingRowAndColumnId(Row, Column, out rowId, out colId);
                bool bIsRowEmpty = Guid.Empty.Equals(rowId);
                bool bIsColEmpty = Guid.Empty.Equals(colId);

                if ((bIsRowEmpty || (!bIsRowEmpty && controller.GetRowMatrixField(Row, Column) == null)) 
                    && (bIsColEmpty || (!bIsColEmpty && controller.GetColumnMatrixField(Column, Row) == null)))
                {
                    if (bIsRowEmpty || bIsColEmpty) 
                    {
                        rboData.Enabled = false; //If there is no intersecting row or column, dragged field cannot be data field. Hence disable the Data RadioButton.
                        if (!bIsColEmpty)
                        {
                            //If the draggedField is having a matrixcolumn just beneath it, then the dragged field is a grouped field.
                            if (ValidateGroupedField() != GroupedColumnValidation.Success)
                            {
                                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                                return;
                            }
                            rboCol.Checked = true;
                            rboCol.Enabled = rboRow.Enabled = rboData.Enabled = false;
                            colObj.GroupedField = true;
                            colObj.RenderingType = MatrixRenderingType.Dynamic;
                        }
                    }
                    else //If there is an intersecting row and column, dragged field will be a data field, Hence disable the Row & Column RadioButton.
                    {
                        bool bOK = controller.ValidateDataField(Row, Column, AppObjectUniqueID);
                        if (!bOK)
                        {
                            ApttusMessageUtil.ShowError(resourceManager.GetResource("MATRIXFLDVW_DataField_ErrorMsg"), resourceManager.GetResource("MATRIXFLDVW_DataFieldCap_ErrorMsg"));
                            DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        }

                        rowLookupValues = controller.GetLookupValues(AppObjectUniqueID, MatrixEntity.Row, rowId);
                        colLookupValues = controller.GetLookupValues(AppObjectUniqueID, MatrixEntity.Column, colId);
                        //Get Lookup values for row & column if any based on rowId & colId;
                        rboData.Checked = true;
                        rboRow.Enabled = false;
                        rboCol.Enabled = false;
                    }
                }
                else
                {
                    //ApttusMessageUtil.ShowError(resourceManager.GetResource("MATRIXFLDVW_DataField_ErrorMsg"), resourceManager.GetResource("MATRIXFLDVW_DataFieldCap_ErrorMsg"));
                    ApttusMessageUtil.ShowError("Field Replacing is not allowed, First Remove inplace field then place new one.", "Matrix Section");
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
            }
        }

        private void rboRow_CheckedChanged(object sender, EventArgs e)
        {
            if (rowObj == null)
                rowObj = new MatrixRowObject(AppObjectUniqueID);
            matrixPropertyGrid.SelectedObject = rowObj;
            matrixPropertyGrid.ResetSelectedProperty();
            btnOK.Enabled = true;
        }

        private void rboCol_CheckedChanged(object sender, EventArgs e)
        {
            if (colObj == null)
            {
                colObj = new MatrixColumnObject(AppObjectUniqueID);
                colObj.GroupedField = false;
            }
            matrixPropertyGrid.SelectedObject = colObj;
            matrixPropertyGrid.ResetSelectedProperty();
            btnOK.Enabled = true;
        }

        private void rboData_CheckedChanged(object sender, EventArgs e)
        {
            if (dataObj == null)
                dataObj = new MatrixDataObject(Components, rowLookupValues, colLookupValues, AppObjectUniqueID);
            matrixPropertyGrid.SelectedObject = dataObj;
            matrixPropertyGrid.ResetSelectedProperty();
            btnOK.Enabled = true;
        }

        /// <summary>
        /// Rendering type
        /// </summary>
        public MatrixRenderingType RenderingType
        {
            get
            {
                if (rboCol.Checked)
                    return colObj.RenderingType;
                else if (rboRow.Checked)
                    return rowObj.RenderingType;
                else
                    return MatrixRenderingType.Dynamic; //For Data 
            }
        }

        /// <summary>
        /// Matrix Value type
        /// </summary>
        public MatrixValueType ValueType
        {
            get
            {
                if (rboCol.Checked)
                    return colObj.ValueType;
                else if (rboRow.Checked)
                    return rowObj.ValueType;
                else
                    return Core.MatrixValueType.FieldValue; //For Data
            }
        }

        /// <summary>
        /// Matrix entity type
        /// </summary>
        public MatrixEntity MatrixEntityType
        {
            get
            {
                MatrixEntity matrixEntity = (MatrixEntity)Int32.Parse(entityLayoutPanel.Controls.OfType<RadioButton>().Where(rbo => rbo.Checked == true).FirstOrDefault().Tag.ToString());
                if (matrixEntity == MatrixEntity.Column && colObj != null && colObj.GroupedField)
                    matrixEntity = MatrixEntity.GroupedColumn;
                return matrixEntity;
            }
        }

        public string SortFieldId
        {
            get
            {
                if (rboRow.Checked)
                    return rowObj.SortFieldId;
                else
                    return colObj.SortFieldId;
            }
            set
            {
                sortFieldId = value;
            }
        }

        public string ComponentName
        {
            get
            {
                return dataObj != null ? dataObj.ComponentName : string.Empty;
            }
            set
            {
                componentName = value;
            }
        }

        public string RowLookupId
        {
            get
            {
                return dataObj != null ? dataObj.RowLookupId : string.Empty;
            }
            set
            {
                rowLookupId = value;
                MatrixDataField dataField = EditingMatrixField as MatrixDataField;
                rowLookupValues = controller.GetLookupValues(AppObjectUniqueID, MatrixEntity.Row, dataField.MatrixRowId);
            }
        }

        public string ColumnLookupId
        {
            get
            {
                return dataObj != null ? dataObj.ColumnLookupId : string.Empty;
            }
            set
            {
                colLookupId = value;
                MatrixDataField dataField = EditingMatrixField as MatrixDataField;
                colLookupValues = controller.GetLookupValues(AppObjectUniqueID, MatrixEntity.Column, dataField.MatrixColumnId);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //If datafield is dropped, ComponentName is mandatory.
            if (rboData.Checked && string.IsNullOrEmpty(ComponentName))
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Click_InfoMsg"), resourceManager.GetResource("COMMON_MatrixSection_Text"));
                return;
            }
            else if (rboData.Checked)
            {
                DialogResult = System.Windows.Forms.DialogResult.None;

                bool bRowLookupExists = rowLookupValues.Count > 0;
                bool bColLookupExists = colLookupValues.Count > 0;

                //1. Check if the component name exists or not.
                MatrixComponent component = controller.Model.MatrixComponents.Where(cm => cm.Name.Equals(ComponentName)).FirstOrDefault();
                if (component != null)
                {
                    //2. Check if the existing component's AppObject is different for the current datafield object.
                    if (AppObjectUniqueID != component.AppObjectUniqueID)
                    {
                        ApttusObject componentObj = ApplicationDefinitionManager.GetInstance.GetAppObject(component.AppObjectUniqueID);
                        ApttusObject selectedObj = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectUniqueID);
                        string error = string.Format(resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Click_WarnMsg"), ComponentName, componentObj.Name, selectedObj.Name);
                        ApttusMessageUtil.ShowWarning(error, resourceManager.GetResource("COMMON_MatrixSection_Text"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                        return;
                    }
                }

                //3. Validate whether the user provided the RowLookup
                if (bRowLookupExists && string.IsNullOrEmpty(dataObj.RowLookup))
                {
                    string error = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickRowLookUp_WarnMsg");
                    ApttusMessageUtil.ShowWarning(error, resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickLookUpCap_WarnMsg"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                    return;
                }
                //4. Validate whether the user provided the ColumnLookup
                else if (bColLookupExists && string.IsNullOrEmpty(dataObj.ColumnLookup))
                {
                    string error = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickColLookUp_WarnMsg");
                    ApttusMessageUtil.ShowWarning(error, resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickLookUpCap_WarnMsg"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                    return;
                }
                //5. Validate whether the user provided the same Row & Column Lookup Value
                else if (bRowLookupExists && bColLookupExists && string.Equals(dataObj.RowLookup, dataObj.ColumnLookup))
                {
                    string error = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickRowColLookUp_WarnMsg");
                    ApttusMessageUtil.ShowWarning(error, resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_ClickLookUpCap_WarnMsg"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                    return;
                }
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            if (Mode == FormOpenMode.Edit && GridModel != null)
            {
                GridModel.EntityType = MatrixEntityType;
                GridModel.FieldValueType = ValueType;
                GridModel.RenderType = RenderingType;
                if (GridModel.EntityType != MatrixEntity.Data)
                {
                    if (rboRow.Checked)
                    {
                        EditingMatrixField.SortFieldId = rowObj.SortFieldId;
                        EditingMatrixField.RenderingType = rowObj.RenderingType;
                        EditingMatrixField.ValueType = rowObj.ValueType;
                    }
                    else
                    {
                        EditingMatrixField.SortFieldId = colObj.SortFieldId;
                        EditingMatrixField.RenderingType = colObj.RenderingType;
                        EditingMatrixField.ValueType = colObj.ValueType;
                    }
                }
                else
                {
                    MatrixDataField dataField = EditingMatrixField as MatrixDataField;
                    if (dataField != null)
                    {
                        dataField.RowLookupId = dataObj.RowLookupId;
                        dataField.ColumnLookupId = dataObj.ColumnLookupId;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Serves as a base class for Row and Column
    /// </summary>
    class MatrixObjectBase
    {
        protected MatrixRenderingType renderingType;
        protected bool isGroupedColumn;
        protected string sortField;
        protected string sortFieldId;

        protected Guid AppObjUniqueId;

        protected MatrixObjectBase(Guid uniqueId)
        {
            AppObjUniqueId = uniqueId;
        }

        protected void ValidateRenderingType(MatrixRenderingType value)
        {
            //Enable Disable ValueType combo
            bool bEnable = renderingType == MatrixRenderingType.Dynamic;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())["ValueType"];
            ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];

            FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, bEnable);

            //Enable Disable SortField combo
            descriptor = TypeDescriptor.GetProperties(this.GetType())["SortField"];
            //If the renderingtype is static, disable the sortfield property
            bEnable = renderingType == MatrixRenderingType.Static;
            if (bEnable)
                descriptor.SetValue(this, string.Empty);

            attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];

            fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, bEnable);
        }

        protected void ValidateGroupedColumnField(bool isGroupedColumn)
        {
            //Enable Disable Rendering Type combo
            //PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())["ValueType"];
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())["RenderingType"];
            ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
            FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, isGroupedColumn);
        }

        internal List<string> GetFields()
        {
            ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjUniqueId);
            List<string> fieldIds = (from fld in obj.Fields
                                     where (fld.Datatype != Datatype.Rich_Textarea && fld.Datatype != Datatype.Textarea && fld.Datatype != Datatype.Attachment && !fld.Id.Equals(obj.IdAttribute))
                                     select fld.Name).ToList();

            fieldIds.Insert(0, string.Empty);
            return fieldIds;
        }

        protected void updateSortFieldId(string fieldName)
        {
            sortFieldId = (from fld in ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjUniqueId).Fields
                           where fld.Name.Equals(fieldName)
                           select fld.Id).FirstOrDefault();
        }

        protected void updateSortFieldName(string fieldId)
        {
            sortField = (from fld in ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjUniqueId).Fields
                         where fld.Id.Equals(fieldId)
                         select fld.Name).FirstOrDefault();
        }
    }

    internal class MatrixRowObject : MatrixObjectBase
    {
        public MatrixRowObject(Guid uniqueId)
            : base(uniqueId)
        {
            ValidateRenderingType(RenderingType);
        }

        [Category("Row Properties")]
        [RefreshProperties(RefreshProperties.All)]
        [ReadOnly(false)]
        [LocalDisplayName("MATRIXMAPPING_tblRenderingType")]
        public MatrixRenderingType RenderingType
        {
            get
            {
                return renderingType;
            }
            set
            {
                renderingType = value;
                if (value == MatrixRenderingType.Dynamic)
                    ValueType = MatrixValueType.FieldValue;
                ValidateRenderingType(value);
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [Category("Row Properties")]
        [LocalDisplayName("UCEXPRVIEW_lblValueType_Text")]
        [ReadOnly(false)]
        [TypeConverter(typeof(MatrixValueTypeConverter))]
        public MatrixValueType ValueType { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [Category("Row Properties")]
        [LocalDisplayName("MATRIXMAPPING_tblSortField")]
        [ReadOnly(false)]
        [TypeConverter(typeof(SortFieldConverter))]
        public string SortField
        {
            get
            {
                return sortField;
            }
            set
            {
                sortField = value;
                updateSortFieldId(value);
            }
        }

        internal string SortFieldId
        {
            get
            {
                return sortFieldId;
            }
            set
            {
                sortFieldId = value;
                updateSortFieldName(sortFieldId);
            }
        }
    }

    internal class MatrixColumnObject : MatrixObjectBase
    {
        public MatrixColumnObject(Guid uniqueId)
            : base(uniqueId)
        {
            ValidateRenderingType(RenderingType);
        }

        [ReadOnly(false)]
        [RefreshProperties(RefreshProperties.All)]
        [LocalDisplayName("MATRIXMAPPING_tblRenderingType")]
        [Category("Column Properties")]
        public MatrixRenderingType RenderingType
        {
            get
            {
                return renderingType;
            }
            set
            {
                renderingType = value;
                if (value == MatrixRenderingType.Dynamic)
                    ValueType = MatrixValueType.FieldValue;
                ValidateRenderingType(value);
            }
        }

        [ReadOnly(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Column Properties")]
        [LocalDisplayName("UCEXPRVIEW_lblValueType_Text")]
        [TypeConverter(typeof(MatrixValueTypeConverter))]
        public MatrixValueType ValueType { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [Category("Column Properties")]
        [LocalDisplayName("MATRIXMAPPING_tblSortField")]
        [ReadOnly(false)]
        [TypeConverter(typeof(SortFieldConverter))]
        public string SortField
        {
            get
            {
                return sortField;
            }
            set
            {
                sortField = value;
                updateSortFieldId(value);
            }
        }

        internal string SortFieldId
        {
            get
            {
                return sortFieldId;
            }
            set
            {
                sortFieldId = value;
                updateSortFieldName(value);
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [Category("Column Properties")]
        [DisplayName("Grouped Field")]
        [ReadOnly(true)]
        public bool GroupedField
        {
            get
            {
                return isGroupedColumn;
            }
            set
            {
                isGroupedColumn = value;
                ValidateGroupedColumnField(value);
            }
        }
    }

    /// <summary>
    /// Acts as a base class for MatrixDataObject. Holds variables used internally to convert / populate data.
    /// </summary>
    class MatrixDataObjectBase
    {
        private Guid AppObjectUniqueId;

        private string rowLookupId;
        private string colLookupId;

        /// <summary>
        ///  Wrapped inside MatrixDataObject.RowLookup, a public property displayed inside PropertyGrid. Stores the selected value by the user. 
        /// </summary>
        protected string rowLookupName;

        /// <summary>
        /// Wrapped inside MatrixDataObject.ColumnLookup, a public property displayed inside PropertyGrid. Stores the selected value by the user. 
        /// </summary>
        protected string colLookupName;

        /// <summary>
        /// Provided to the PropertyGrid.ComponentName which displays the list of all components being used. ComponentNamesConverter will provide the list of all components to PropertyGrid via Reflection
        /// </summary>
        internal List<string> ComponentNames { get; set; }

        /// <summary>
        /// List of all lookup names to displayed to select a RowLookup field. RowLookupValueConverter will provide the list of all row lookup values to PropertyGrid.
        /// </summary>
        internal List<string> RowLookUpValues { get; set; }

        /// <summary>
        /// List of all lookup names to displayed to select a ColumnLookup field. ColumnLookupValueConverter will provide the list of all row lookup values to PropertyGrid.
        /// </summary>
        internal List<string> ColumnLookUpValues { get; set; }

        /// <summary>
        /// RowLookupId is the value that will be saved in the config. 
        /// When the PropertyGrid is opened in EditMode, this value will be provided by config and it populates the Name Field of that Object.
        /// </summary>
        internal string RowLookupId
        {
            get
            {
                return rowLookupId;
            }
            set
            {
                rowLookupId = value;
                updateName(MatrixEntity.Row, value);
            }
        }

        /// <summary>
        /// ColumnLookupId is the value that will be saved in the config. 
        /// When the PropertyGrid is opened in EditMode, this value will be provided by config and it populates the Name Field of that Object.
        /// </summary>
        internal string ColumnLookupId
        {
            get
            {
                return colLookupId;
            }
            set
            {
                colLookupId = value;
                updateName(MatrixEntity.Column, value);
            }
        }

        protected MatrixDataObjectBase(Guid id)
        {
            AppObjectUniqueId = id;
            rowLookupId = colLookupId = string.Empty;
            rowLookupName = colLookupName = string.Empty;
        }

        /// <summary>
        /// Converts the input fieldname and sets the rowLookupId or colLookupId property.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        protected void updateId(MatrixEntity entity, string fieldName)
        {
            string lookupId = (from fld in ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectUniqueId).Fields
                               where fld.Name.Equals(fieldName)
                               select fld.Id).FirstOrDefault();
            switch (entity)
            {
                case MatrixEntity.Row:
                    rowLookupId = lookupId;
                    break;
                case MatrixEntity.Column:
                    colLookupId = lookupId;
                    break;
            }
        }

        /// <summary>
        /// Converts the input fieldid and sets the rowLookupName & colLookupName property.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldId"></param>
        protected void updateName(MatrixEntity entity, string fieldId)
        {
            string lookupname = (from fld in ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectUniqueId).Fields
                                 where fld.Id.Equals(fieldId)
                                 select fld.Name).FirstOrDefault();
            switch (entity)
            {
                case MatrixEntity.Row:
                    rowLookupName = lookupname;
                    break;
                case MatrixEntity.Column:
                    colLookupName = lookupname;
                    break;
            }
        }
    }

    internal class MatrixDataObject : MatrixDataObjectBase
    {
        public MatrixDataObject(List<MatrixComponent> components, List<string> rowlookups, List<string> collookups, Guid appObjectId)
            : base(appObjectId)
        {
            ComponentNames = components.Select(cmp => cmp.Name).ToList();
            RowLookUpValues = rowlookups;
            ColumnLookUpValues = collookups;

            //If there is only item in the list, then auto-select the first item, when the dialog box opens. This is applicable for both row & column values.
            if (RowLookUpValues.Count == 1)
                rowLookupName = RowLookUpValues[0];
            if (ColumnLookUpValues.Count == 1)
                colLookupName = ColumnLookUpValues[0];
        }

        [Category("Data Properties")]
        [LocalDisplayName("COMMON_SectionName_Text")]
        [ReadOnly(false)]
        [TypeConverter(typeof(ComponentNamesConverter))]
        public string ComponentName { get; set; }

        [Category("Data Properties")]
        [ReadOnly(true)]
        [LocalDisplayName("MATRIXMAPPING_tblRenderingType")]
        public MatrixRenderingType RenderingType { get { return MatrixRenderingType.Dynamic; } }

        [Category("Data Properties")]
        [ReadOnly(true)]
        [LocalDisplayName("UCEXPRVIEW_lblValueType_Text")]
        [TypeConverter(typeof(MatrixValueTypeConverter))]
        public MatrixValueType ValueType { get { return MatrixValueType.FieldValue; } }

        [Category("Data Properties")]
        [LocalDisplayName("MATRIXMAPPING_tblRowLookup")]
        [TypeConverter(typeof(RowLookupValueConverter))]
        public string RowLookup
        {
            get
            {
                return rowLookupName;
            }
            set
            {
                rowLookupName = value;
                updateId(MatrixEntity.Row, value);
            }
        }

        [Category("Data Properties")]
        [LocalDisplayName("MATRIXMAPPING_tblColumnLookup")]
        [TypeConverter(typeof(ColumnLookupValueConverter))]
        public string ColumnLookup
        {
            get
            {
                return colLookupName;
            }
            set
            {
                colLookupName = value;
                updateId(MatrixEntity.Column, value);
            }
        }
    }

    public class RowLookupValueConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            MatrixDataObject obj = context.Instance as MatrixDataObject;
            return new StandardValuesCollection(obj.RowLookUpValues);
        }
    }

    public class ColumnLookupValueConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            MatrixDataObject obj = context.Instance as MatrixDataObject;
            return new StandardValuesCollection(obj.ColumnLookUpValues);
        }
    }

    public class ComponentNamesConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            MatrixDataObject obj = context.Instance as MatrixDataObject;
            return new StandardValuesCollection(obj.ComponentNames);
        }
    }

    public class SortFieldConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            MatrixObjectBase objBase = context.Instance as MatrixObjectBase;
            return new StandardValuesCollection(objBase.GetFields());
        }
    }

    public class MatrixValueTypeConverter : EnumConverter
    {
        Type enumType;

        public MatrixValueTypeConverter(Type type)
            : base(type)
        {
            enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            FieldInfo fieldInfo = enumType.GetField(Enum.GetName(enumType, value));
            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return descAttr != null ? descAttr.Description : value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            foreach (FieldInfo fieldInfo in enumType.GetFields())
            {
                DescriptionAttribute descAttr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if ((descAttr != null) && ((string)value == descAttr.Description))
                    return Enum.Parse(enumType, fieldInfo.Name);
            }
            return Enum.Parse(enumType, (string)value);
        }
    }
}
