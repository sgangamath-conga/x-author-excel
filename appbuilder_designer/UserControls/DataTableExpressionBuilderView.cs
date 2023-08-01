/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using Apttus.XAuthor.AppDesigner.Modules;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace Apttus.XAuthor.AppDesigner
{

    public enum DataTableExpressionItemType : int
    {
        SrNoLabel = 0,
        ObjectFieldText = 1,
        ValueTypePicklist = 2,
        OperatorPicklist = 3,
        Value = 4,
        Delete = 5,
        SearchFilter = 100 // This won't be a control
    }

    public partial class DataTableExpressionBuilderView : UserControl
    {
        private Core.DataTableExpressionBuilder dataTableExpressionBuilder;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private Dictionary<string, string> logicalOperators = new Dictionary<string, string>();
        //Added to remember relational field's target objects
        private Dictionary<string, Guid> TargetObjectsOfFields;
        public List<SearchFilterGroup> Model { get; set; }
        public ApttusObject TargetObject { get; set; }
        private List<DataTableFilter> filterOperators = new List<DataTableFilter>();
        private string TargetNamedRange;

        //Not supported datatypes in delete action
        internal Predicate<ApttusField> IsSupportedDataTypes = rField => rField.Datatype != Datatype.Base64 && rField.Datatype != Datatype.Attachment && rField.Datatype != Datatype.Rich_Textarea;

        public DataTableExpressionBuilderView()
        {
            IsExpressionBuilderLaunchedFromQuickApp = false;
            InitializeComponent();
            InitializeValues();
            TargetNamedRange = string.Empty;
            TargetObjectsOfFields = new Dictionary<string, Guid>();
        }

        public void SetCultureData()
        {
            lblFilterBy.Text = resourceManager.GetResource("COMMON_Filters_Text");
            lblFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_lblFilterLogic_Text");
            lblObjectAndFields.Text = resourceManager.GetResource("UCEXPRVIEW_lblObjectAndFields_Text");
            lblOperator.Text = resourceManager.GetResource("UCEXPRVIEW_lblOperator_Text");
            lblValue.Text = resourceManager.GetResource("UCEXPRVIEW_lblValue_Text");
            lblValueType.Text = resourceManager.GetResource("UCEXPRVIEW_lblValueType_Text");
            lnkAddRemoveFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_lnkAddRemoveFilterLogic_Text");
            lnkAddRow.Text = resourceManager.GetResource("COMMON_AddRow_Text");
            lnkClearAll.Text = resourceManager.GetResource("COMMON_ClearAll_Text");
        }

        public bool IsExpressionBuilderLaunchedFromQuickApp { get; set; }

        private void InitializeValues()
        {
            // 1. Populate Logical operators
            logicalOperators.Add("&&", "AND");

            // 2. Populate Standard operators
            filterOperators = DataTableFilter.InitializeFilters();
        }

        public void SetExpressionBuilder(List<SearchFilterGroup> model, ApttusObject TargetObject, string targetNamedRange)
        {
            TargetNamedRange = targetNamedRange;
            dataTableExpressionBuilder = new DataTableExpressionBuilder(TargetObject);

            // 1. Set Model
            if (model == null)
                this.Model = new List<SearchFilterGroup>();
            else
                this.Model = model.Select(sfg => sfg.Clone()).ToList();

            // 2. Set Target Object on which the expression will be built.
            this.TargetObject = TargetObject;

            // 3. Initialize Expresison Builder with values
            ResetExpressionBuilder();

            // 4. Load Existing or Load a new Expression Row
            if (this.Model.Count == 0)
                CreateExpressionRow(null);
            else
                LoadModel();
        }

        public void ResetExpressionBuilder()
        {
            tblExpressions.SuspendLayout();

            if (bDummyRowAdded)
                RemoveDummyRow();

            while (tblExpressions.RowCount > 1)
            {
                int row = tblExpressions.RowCount - 1;
                for (int i = 0; i < tblExpressions.ColumnCount; i++)
                {
                    Control c = tblExpressions.GetControlFromPosition(i, row);
                    if (c != null)
                    {
                        tblExpressions.Controls.Remove(c);
                        c.Dispose();
                    }
                }

                tblExpressions.RowCount--;
            }

            AddDummyRow();
            lnkAddRow.Visible = tblExpressions.RowCount < Constants.EXPRESSION_BUILDER_MAX_ROWS;

            tblExpressions.ResumeLayout();
            tblExpressions.PerformLayout();
        }

        bool bDummyRowAdded = false;
        private void AddDummyRow()
        {
            if (!bDummyRowAdded)
            {
                tblExpressions.RowStyles.Add(new RowStyle
                {
                    SizeType = SizeType.AutoSize,
                    Height = 0
                });
                bDummyRowAdded = true;
                tblExpressions.RowCount++;
            }
        }
        private void RemoveDummyRow()
        {
            if (bDummyRowAdded)
            {
                RowStyle dummyRowStyle = null;
                foreach (RowStyle rowStyle in tblExpressions.RowStyles)
                {
                    if (rowStyle.Height == 0 && rowStyle.SizeType == SizeType.AutoSize)
                    {
                        dummyRowStyle = rowStyle;
                        break;
                    }
                }
                if (dummyRowStyle != null)
                {
                    tblExpressions.RowStyles.Remove(dummyRowStyle);
                    bDummyRowAdded = false;
                    tblExpressions.RowCount--;
                }
            }
        }

        private void LoadModel()
        {
            // Assumption: Current implementation at max will always have 1 SearchFilterGroup
            SearchFilterGroup searchFilterGroup = Model.FirstOrDefault();
            if (searchFilterGroup != null)
            {
                foreach (SearchFilter searchFilter in searchFilterGroup.Filters)
                    CreateExpressionRow(searchFilter);

                if (searchFilterGroup.IsAddFilter)
                {
                    lnkAddRemoveFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_ClearFilterLogic_Text");
                    pnlFilterLogic.Visible = true;
                }
                else
                {
                    lnkAddRemoveFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_lnkAddRemoveFilterLogic_Text");
                    pnlFilterLogic.Visible = false;
                }
                txtFilterLogic.Text = searchFilterGroup.FilterLogicText;
            }
        }

        public void CreateExpressionRow(SearchFilter searchFilter)
        {

            if (dataTableExpressionBuilder == null)
                return;

            tblExpressions.SuspendLayout();

            if (bDummyRowAdded)
                RemoveDummyRow();

            if (searchFilter == null)
                searchFilter = new SearchFilter { SequenceNo = tblExpressions.RowCount };

            int rowNo = tblExpressions.RowCount;

            // Column 1 - Serial Number
            Label lblSerialNumber = new Label();

            //lblSerialNumber.Font = new System.Drawing.Font(lblSerialNumber.Font, FontStyle.Bold);
            lblSerialNumber.Text = searchFilter.SequenceNo.ToString();
            lblSerialNumber.AutoSize = true;
            lblSerialNumber.Margin = new Padding(3, 3, 3, 3);

            // Column 2 - Object and Field Text
            ComboBox cboField = new ComboBox();
            cboField.Dock = DockStyle.Fill;
            cboField.BackColor = Color.White;
            cboField.Tag = searchFilter;
            cboField.DropDownStyle = ComboBoxStyle.DropDownList;
            cboField.CreateControl();
            cboField.DisplayMember = Constants.VALUE_COLUMN;
            cboField.ValueMember = Constants.KEY_COLUMN;

            cboField.DataSource = new BindingSource(GetTargetObjectFieldsAsDictionary(), null);
            cboField.DropDown += new EventHandler(AdjustWidth_DropDown);
            // Column 4 - Value to be compared
            ComboBox cboValueType = new ComboBox();
            cboValueType.Dock = DockStyle.Fill;
            cboValueType.FlatStyle = FlatStyle.Standard;
            cboValueType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboValueType.SelectedIndexChanged += cboValueType_SelectedIndexChanged;
            cboValueType.DropDown += new EventHandler(AdjustWidth_DropDown);
            cboValueType.CreateControl();

            // Column 5 - Operators
            ComboBox cboOperator = new ComboBox();
            cboOperator.Dock = DockStyle.Fill;
            cboOperator.FlatStyle = FlatStyle.Standard;
            cboOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOperator.SelectedIndexChanged += cboOperators_SelectedIndexChanged;
            cboOperator.DropDown += new EventHandler(AdjustWidth_DropDown);
            cboOperator.CreateControl();

            // Column 6  - Value
            TextBox txtValue = new TextBox();
            txtValue.Dock = DockStyle.Fill;
            txtValue.BackColor = Color.White;
            Control valueControl = txtValue;

            // Column 7 - Delete the current row
            Button btnDeleteRow = new Button();
            btnDeleteRow.BackColor = Color.Transparent;
            btnDeleteRow.FlatStyle = FlatStyle.Flat;
            btnDeleteRow.FlatAppearance.BorderColor = Color.Silver;
            btnDeleteRow.Cursor = Cursors.Hand;
            btnDeleteRow.Image = Properties.Resources.cancel_icon;
            btnDeleteRow.Click += new EventHandler(btnDeleteRow_Clicked);
            btnDeleteRow.Size = new System.Drawing.Size(24, 24);

            tblExpressions.Controls.Add(lblSerialNumber, DataTableExpressionItemType.SrNoLabel, rowNo);
            tblExpressions.Controls.Add(cboField, DataTableExpressionItemType.ObjectFieldText, rowNo);
            tblExpressions.Controls.Add(cboValueType, DataTableExpressionItemType.ValueTypePicklist, rowNo);
            tblExpressions.Controls.Add(cboOperator, DataTableExpressionItemType.OperatorPicklist, rowNo);
            tblExpressions.Controls.Add(valueControl, DataTableExpressionItemType.Value, rowNo);
            tblExpressions.Controls.Add(btnDeleteRow, DataTableExpressionItemType.Delete, rowNo);

            cboField.SelectedIndex = -1;
            cboField.SelectedIndexChanged += cboField_SelectedIndexChanged;
            // Important Developer Note: The combobox controls can only be bounded to data, once they are added to the parent control (tblExpressions).
            // Once we bind the filter value types, the cboValueType combobox, SelectedIndexChange event is fired, which in turn fires the cboOperator's SelectedIndexChange event.
            // All these events will be bypassed when we bind the data from the model or in empty row addition.

            ApttusObject apttusObject = dataTableExpressionBuilder.GetSearchFilterObject(searchFilter);
            ApttusField apttusField = dataTableExpressionBuilder.GetSearchFilterField(searchFilter);

            if (apttusField != null)
            {
                // Column 4 - Bind and Set value for Value Type Combobox
                cboValueType.SelectedIndexChanged -= cboValueType_SelectedIndexChanged;
                cboOperator.SelectedIndexChanged -= cboOperators_SelectedIndexChanged;
                cboField.SelectedIndexChanged -= cboField_SelectedIndexChanged;

                //To avoid conflict between normal and relational fields
                cboField.SelectedValue = searchFilter.FieldId;
                // 4.1 Always Bind if its new record or existing
                BindValueTypes(cboValueType, apttusObject, apttusField);
                // 4.2 Set Value for Column 4 - Value Type
                if (apttusField.Datatype == Datatype.Picklist && searchFilter.ValueType == ExpressionValueTypes.Input)
                    // Backward Compatibility - ValueType is Input for Picklist will be treated as Static.
                    cboValueType.SelectedValue = searchFilter.ValueType = ExpressionValueTypes.Static;
                else
                    cboValueType.SelectedValue = searchFilter.ValueType;

                cboValueType.SelectedIndexChanged += cboValueType_SelectedIndexChanged;
                cboOperator.SelectedIndexChanged += cboOperators_SelectedIndexChanged;
                cboField.SelectedIndexChanged += cboField_SelectedIndexChanged;

                // Column 5 - Bind and Set value for Operators Combobox
                // 5.1 Bind if its existing record
                cboOperator.SelectedIndexChanged -= cboOperators_SelectedIndexChanged;
                BindOperators(cboOperator, apttusObject, apttusField, (ExpressionValueTypes)cboValueType.SelectedValue);
                cboOperator.SelectedIndex = -1; //this is needed. Don't Modify.
                cboOperator.SelectedIndexChanged += cboOperators_SelectedIndexChanged;

                // 5.2 Set Value for Column 5 - Operators set value only if Apttus Field exits
                cboOperator.SelectedItem = cboOperator.Items.OfType<DataTableFilter>().SingleOrDefault(f => f.Key.Equals(searchFilter.Operator));
            }

            // Column 6 - Bind and Set Value Control
            if (apttusObject != null && apttusField != null)
            {
                // 6.1 Bind Value Control
                //Once we set the cboOperator SelectedItem property, cboOperator_SelectedIndexChanged Event is fired and hence the correct value control is loaded and ready for data population.
                //So just get the control and set the data appropriately.
                valueControl = tblExpressions.GetControlFromPosition(DataTableExpressionItemType.Value, searchFilter.SequenceNo);

                // 6.2 - Set Value for Value Control
                SetValue(valueControl, searchFilter, searchFilter.SequenceNo, apttusObject, apttusField);
                //(valueControl as ComboBox).SelectedItem = searchFilter.Value; // Temp Code
            }

            // Footer Controls and update tblExpressions
            lnkAddRow.Visible = rowNo < Constants.EXPRESSION_BUILDER_MAX_ROWS;

            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tblExpressions.Padding = new Padding(0, 0, vertScrollWidth, 0);

            tblExpressions.RowCount++;
            tblExpressions.ResumeLayout();
            tblExpressions.PerformLayout();
        }

        private Dictionary<string, string> GetTargetObjectFieldsAsDictionary()
        {
            Dictionary<string, string> fields = TargetObject.Fields.Where(fld => fld.Datatype != Datatype.DateTime).ToDictionary(key => key.Id, value => value.Name);

            if (string.IsNullOrEmpty(TargetNamedRange))
                return fields;

            //Add all empty columns in the repeating group.
            RepeatingGroup repeatingGroup = (from rm in ConfigurationManager.GetInstance.RetrieveMaps
                                             from rg in rm.RepeatingGroups
                                             where rg.TargetNamedRange == TargetNamedRange
                                             select rg).FirstOrDefault();

            List<SaveMap> saveMaps = (from sm in ConfigurationManager.GetInstance.SaveMaps
                                      from sg in sm.SaveGroups
                                      where sg.TargetNamedRange == TargetNamedRange && sg.AppObject == TargetObject.UniqueId
                                      select sm).ToList();

            //Clear all the fields, so that they can appear in the order of TargetColumnIndex. This needs to be done, because empty column also should appear in order of TargetColumnIndex.
            fields.Clear();

            if (repeatingGroup != null)
            {
                Excel.Range targetRange = ExcelHelper.GetRange(TargetNamedRange);
                int nColumns = targetRange.Columns.Count;
                for (int columnIndex = 1; columnIndex <= nColumns; ++columnIndex)
                {
                    RetrieveField field = repeatingGroup.RetrieveFields.Find(rf => rf.TargetColumnIndex == columnIndex);
                    if (field == null)
                    {
                        SaveField saveOtherField = (from sm in saveMaps
                                                    from sg in sm.SaveGroups
                                                    from sf in sm.SaveFields.Where(f => sg.GroupId == f.GroupId && f.TargetColumnIndex == columnIndex
                                                        && f.SaveFieldType == SaveType.SaveOnlyField && f.AppObject == TargetObject.UniqueId)
                                                    select sf).FirstOrDefault();
                        if (saveOtherField != null)
                        {
                            if (!TargetObjectsOfFields.ContainsKey(saveOtherField.FieldId)) TargetObjectsOfFields.Add(saveOtherField.FieldId, TargetObject.UniqueId);
                            fields.Add(saveOtherField.FieldId, TargetObject.GetField(saveOtherField.FieldId).Name);
                        }
                        else
                        {
                            //It is an empty column
                            Excel.Range cell = targetRange.Cells[1, columnIndex] as Excel.Range;
                            string columnName = cell.EntireColumn.Address.Split(':')[0].Replace("$", string.Empty);
                            string Value = "Column " + columnName;
                            string Key = columnIndex.ToString();
                            fields.Add(Key, Value);
                        }
                    }
                    else
                    {
                        //App objects of current object's fields and relational fields would be different.
                        //AppDefManager retrieves correct object for respective field and executes the logic
                        ApttusObject fieldObject = applicationDefinitionManager.GetAppObject(field.AppObject);
                        ApttusField fieldToAdd = fieldObject.GetField(field.FieldId);
                        if (IsSupportedDataTypes(fieldToAdd))
                        {
                            if (!TargetObjectsOfFields.ContainsKey(field.FieldId)) TargetObjectsOfFields.Add(field.FieldId, fieldObject.UniqueId);
                            //Current Object fields
                            if (fieldObject.UniqueId == TargetObject.UniqueId)
                            {
                                fields.Add(field.FieldId, fieldToAdd.Name);
                            }
                            //Incase of relational fields, Field's combo box will display field's name with '.' notation. Ex: Opportunity.Account.Type
                            else
                            {
                                fields.Add(field.FieldId, field.FieldName);
                            }
                        }
                    }
                }
            }
            return fields;
        }

        private void cboField_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, DataTableExpressionItemType.SearchFilter);
            Guid ObjectUniqueId;
            searchFilter.AppObjectUniqueId = TargetObject.UniqueId;
            searchFilter.FieldId = (sender as ComboBox).SelectedValue.ToString();
            searchFilter.SearchFilterLabel = string.Empty;
            //By default target object unique id will be assigned to serach filter but incase of relational field it will be change
            if (TargetObjectsOfFields.TryGetValue(searchFilter.FieldId, out ObjectUniqueId))
            {
                searchFilter.AppObjectUniqueId = ObjectUniqueId;
            }

            ApttusObject obj = applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId);

            var valueTypes = GetAvailableValueTypes(obj, dataTableExpressionBuilder.GetSearchFilterField(searchFilter));
            if (valueTypes != null)
                searchFilter.ValueType = valueTypes.First();
            (sender as ComboBox).Tag = searchFilter;

            ResetCurrentFilter(sender, ResetAfter.ObjectFieldSelection);
        }

        void cboValueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Search Filter
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, DataTableExpressionItemType.SearchFilter);
            ExpressionValueTypes valueType;
            if (Enum.TryParse((sender as ComboBox).SelectedValue.ToString(), out valueType))
                searchFilter.ValueType = valueType;
            UpdateCurrentExpression(sender, searchFilter);
            // Reset the Layout
            ResetCurrentFilter(sender, ResetAfter.InputSelection);
        }

        void cboOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Search Filter
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, DataTableExpressionItemType.SearchFilter);
            searchFilter.Operator = ((DataTableFilter)(sender as ComboBox).SelectedItem).Key;
            UpdateCurrentExpression(sender, searchFilter);

            // Reset the Layout
            ResetCurrentFilter(sender, ResetAfter.OperatorSelection);
        }

        private void llInValues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, DataTableExpressionItemType.SearchFilter);

            // Launch Object and Field picker
            MultiLineCaptureView view = new MultiLineCaptureView();
            MultiLineCaptureController controller = new MultiLineCaptureController(view, dataTableExpressionBuilder.GetSearchFilterObject(searchFilter), dataTableExpressionBuilder.GetSearchFilterField(searchFilter), searchFilter.Value);

            // Update the Object and Field text and Search Filter, if the user hit save on the dialog. Search Filter will already have new values.
            if (controller.Browse())
            {
                LinkLabel llInValues = GetExpressionItem<LinkLabel>(sender, DataTableExpressionItemType.Value);
                searchFilter.Value = controller.MultiLineValue;

                PopulateInValuesLinkLabel(llInValues, searchFilter.Value);
                UpdateCurrentExpression(sender, searchFilter);
            }
        }

        private void ResetCurrentFilter(object sender, ResetAfter resetAfter)
        {
            try
            {
                tblExpressions.SuspendLayout();
                SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, DataTableExpressionItemType.SearchFilter);
                ApttusObject apttusObject = dataTableExpressionBuilder.GetSearchFilterObject(searchFilter);
                ApttusField apttusField = dataTableExpressionBuilder.GetSearchFilterField(searchFilter);
                Control valueControl = GetExpressionItem<Control>(sender, DataTableExpressionItemType.Value);
                ComboBox cboType = GetExpressionItem<ComboBox>(sender, DataTableExpressionItemType.ValueTypePicklist);
                ComboBox cboOperator = GetExpressionItem<ComboBox>(sender, DataTableExpressionItemType.OperatorPicklist);
                Control newValueControl = null;

                switch (resetAfter)
                {
                    case ResetAfter.AllBlank:
                        break;
                    case ResetAfter.ObjectFieldSelection:

                        #region "ResetAfter.ObjectFieldSelection"

                        // 1. Reset the Input Type Picklist
                        BindValueTypes(cboType, apttusObject, apttusField);

                        // 2. Reset the Value Control
                        newValueControl = GetValueControl(tblExpressions.GetRow(sender as Control), searchFilter, apttusObject, apttusField);
                        AddOrReplaceExpressionItem<ComboBox>(sender, newValueControl, DataTableExpressionItemType.Value);

                        break;
                    #endregion

                    case ResetAfter.InputSelection:

                        #region "ResetAfter.InputSelection"

                        // 2. Reset the Operator Picklist                    
                        BindOperators(cboOperator, apttusObject, apttusField, ((ValueType)cboType.SelectedItem).Key);

                        break;
                    #endregion

                    case ResetAfter.OperatorSelection:

                        #region "ResetAfter.OperatorSelection"

                        // 3. Reset the Value Control
                        newValueControl = GetValueControl(tblExpressions.GetRow(sender as Control), searchFilter, apttusObject, apttusField);
                        AddOrReplaceExpressionItem<ComboBox>(sender, newValueControl, DataTableExpressionItemType.Value);

                        break;
                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex) { }
            finally
            {
                tblExpressions.ResumeLayout();
            }
        }

        private Control GetValueControl(int row, SearchFilter searchFilter, ApttusObject apttusObject, ApttusField apttusField)
        {
            Control result = null;
            if (searchFilter.ValueType == ExpressionValueTypes.Static)
            {
                if (apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Boolean)
                {
                    // Column 5 - Create ComboBox
                    ComboBox cboStaticValueControl = new ComboBox();
                    cboStaticValueControl.Dock = DockStyle.Fill;
                    cboStaticValueControl.FlatStyle = FlatStyle.Standard;
                    cboStaticValueControl.DropDownStyle = ComboBoxStyle.DropDownList;
                    cboStaticValueControl.DropDown += new EventHandler(AdjustWidth_DropDown);
                    cboStaticValueControl.CreateControl();

                    switch (apttusField.Datatype)
                    {
                        case Datatype.Picklist:
                        case Datatype.Picklist_MultiSelect:
                            PopulatePicklist(cboStaticValueControl, apttusField);
                            break;
                        case Datatype.Boolean:
                            PopulateBooleanCombo(cboStaticValueControl);
                            break;
                        default:
                            break;
                    }

                    result = cboStaticValueControl;
                }
                else if (apttusField.Datatype == Datatype.Date)
                {
                    DateTimePicker picker = new DateTimePicker();
                    picker.Format = DateTimePickerFormat.Short;
                    picker.Dock = DockStyle.Fill;
                    result = picker;
                }
                else
                {
                    // Column 5 - Create TextBox
                    TextBox txtValue = new TextBox();
                    txtValue.BackColor = Color.White;
                    txtValue.Dock = DockStyle.Fill;

                    result = txtValue;
                }

            }
            return result;
        }

        private string GetValue(Control control)
        {
            string sValue = string.Empty;

            if (control.GetType() == typeof(TextBox))
            {
                sValue = ((TextBox)control).Text;
            }
            else if (control.GetType() == typeof(ComboBox))
            {
                sValue = Convert.ToString(((ComboBox)control).SelectedValue);
            }
            else if (control.GetType() == typeof(LinkLabel))
            {
                sValue = Convert.ToString(((LinkLabel)control).Tag);
            }
            else if (control.GetType() == typeof(DateTimePicker))
            {
                sValue = (control as DateTimePicker).Value.Date.ToShortDateString();
            }

            return sValue;
        }

        private void SetValue(Control control, SearchFilter searchFilter, int controlRow, ApttusObject apttusObject, ApttusField apttusField)
        {
            if (control.GetType() == typeof(TextBox))
            {
                ((TextBox)control).Text = searchFilter.Value;
            }
            else if (control.GetType() == typeof(ComboBox))
            {
                if (apttusField.Datatype == Datatype.Lookup)
                {
                    PopulateLookupFields((ComboBox)control, apttusField, searchFilter);
                    ((ComboBox)control).SelectedValue = searchFilter.Value;
                }
                else if (apttusField.Id == apttusObject.IdAttribute)
                {
                    // Dropdown should already be populated by Operator Selection change event - Code is kept here if needed in future
                    //PopulateChildParentLookup((ComboBox)control, apttusObject); 
                    ((ComboBox)control).SelectedValue = searchFilter.Value;
                }
                else if (apttusField.Datatype == Datatype.Picklist)
                {
                    ((ComboBox)control).SelectedIndex = ((ComboBox)control).FindStringExact(searchFilter.Value);
                }
                else if (apttusField.Datatype == Datatype.Boolean)
                {
                    ((ComboBox)control).SelectedIndex = ((ComboBox)control).FindStringExact(searchFilter.Value);
                }
            }
            else if (control.GetType() == typeof(DateTimePicker))
            {
                if (searchFilter.ValueType == ExpressionValueTypes.Static)
                {
                    DateTime date;
                    if (DateTime.TryParseExact(searchFilter.Value, searchFilter.SearchFilterLabel, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        (control as DateTimePicker).Value = date;
                }
            }
        }

        private bool IsFieldEmptyDateField(SearchFilter filter)
        {
            ApttusObject obj = applicationDefinitionManager.GetAppObject(filter.AppObjectUniqueId);
            ApttusField field = obj.Fields.Find(fld => fld.Id == filter.FieldId);
            if (field == null)
            {
                //Check whether the value in the filter is of type date with the system date format.
                DateTime result;
                if (DateTime.TryParseExact(filter.Value, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                    return true;
            }
            return false;
        }

        public List<SearchFilterGroup> SaveExpression(out string errorMessage)
        {
            StringBuilder errorMessages = new StringBuilder();
            string filterErrorMessage = string.Empty;
            string errorMessageHeader = resourceManager.GetResource("EXPRESSIONBUILDVIEW_SaveExpression_ErrMsg") + Environment.NewLine + Environment.NewLine;
            errorMessage = string.Empty;
            string message = string.Empty;
            bool bValidSearchFilter = false;
            if (tblExpressions.RowCount > 1)
            {
                if (bDummyRowAdded)
                    RemoveDummyRow();

                if (Model != null)
                {
                    StringBuilder sbFilterLogicText = new StringBuilder();
                    if (Model.Count > 0 && !String.IsNullOrEmpty(txtFilterLogic.Text))
                        sbFilterLogicText.Append(Model[0].FilterLogicText);
                    else
                        sbFilterLogicText.Append(txtFilterLogic.Text);

                    Model.Clear();
                    List<SearchFilterGroup> searchFilterGroups = new List<SearchFilterGroup>();

                    // LogicalOperator is now deprecated (not used) but it added for backward compatibility.
                    SearchFilterGroup searchFilterGroup = new SearchFilterGroup() { LogicalOperator = LogicalOperator.AND };
                    searchFilterGroup.Filters = new List<SearchFilter>();

                    for (int i = 1; i < tblExpressions.RowCount; i++)
                    {
                        SearchFilter searchFilter = GetExpressionItem<SearchFilter>(i, DataTableExpressionItemType.SearchFilter);
                        if (IsValidSearchFilter(searchFilter))
                        {
                            Control valueControl = tblExpressions.GetControlFromPosition(DataTableExpressionItemType.Value, i);
                            searchFilter.Value = GetValue(valueControl);

                            //Re-purpose SearchFilterLabel to store the current user' operating system's date format 
                            if (valueControl is DateTimePicker || IsFieldEmptyDateField(searchFilter))
                                searchFilter.SearchFilterLabel = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

                            bValidSearchFilter = ValidateFilterValues(searchFilter, out filterErrorMessage);
                            if (!bValidSearchFilter)
                                errorMessages.AppendLine("Row #" + (i).ToString() + " - " + filterErrorMessage);
                            else
                            {
                                searchFilterGroup.Filters.Add(searchFilter);
                                bValidSearchFilter = true;
                            }
                        }
                        else
                        {
                            // In case Row Count is 2, then the first blank row should be ignored as user may not want to enter any search filter.
                            if (tblExpressions.RowCount > 2)
                                errorMessages.AppendLine("Row #" + (i).ToString());
                        }
                    }

                    // Filter text
                    if (!string.IsNullOrEmpty(txtFilterLogic.Text))
                    {
                        searchFilterGroup.IsAddFilter = true;
                        searchFilterGroup.FilterLogicText = txtFilterLogic.Text;
                        message = ValidateFilters(searchFilterGroup);
                        if (!string.IsNullOrEmpty(message))
                            errorMessages.AppendLine(message);
                        searchFilterGroups.Add(searchFilterGroup.Clone());
                    }

                    if (errorMessages.Length > 0)
                    {
                        ApttusMessageUtil.ShowError(errorMessageHeader + errorMessages.ToString(), resourceManager.GetResource("MULCAPTUREVW_Val_ErrorMsg"));
                        if (sbFilterLogicText.Length > 0)
                            searchFilterGroups[0].FilterLogicText = sbFilterLogicText.ToString();
                        Model = searchFilterGroups;
                    }
                    else
                    {
                        Model.Add(searchFilterGroup);
                    }
                }
            }
            else
                Model = null;

            // Return null if no valid query is found.
            //if (!bValidSearchFilter)
            //    Model = null;

            // ToDo: return boolean instead of errorMessage
            errorMessage = errorMessages.ToString();

            // If expression is valid, create named range for Cell reference filters
            if (string.IsNullOrEmpty(errorMessage) && (Model != null && Model.Count > 0 && Model[0].Filters != null))
                ExcelHelper.CreateCellReferenceNameRanges(Model);

            return Model;
        }

        private void CreateLogicalControl(bool addFilterLogic)
        {
            // Add filter logic not added
            if (!addFilterLogic)
            {
                foreach (Control ctl in tblExpressions.Controls)
                {
                    int controlRow = tblExpressions.GetRow(ctl);
                    Control LogicalOperation = tblExpressions.GetControlFromPosition(DataTableExpressionItemType.SrNoLabel, controlRow);
                    tblExpressions.Controls.Remove(LogicalOperation);
                }

                if (tblExpressions.RowCount > 1)
                {
                    // Start with second row
                    for (int i = 2; i <= tblExpressions.RowCount; i++)
                    {
                        ComboBox cboLogicalOperation = new ComboBox();
                        cboLogicalOperation.Dock = DockStyle.Fill;
                        cboLogicalOperation.FlatStyle = FlatStyle.Flat;
                        cboLogicalOperation.DropDownStyle = ComboBoxStyle.DropDownList;
                        cboLogicalOperation.Visible = tblExpressions.RowCount > 1;
                        cboLogicalOperation.DropDown += new EventHandler(AdjustWidth_DropDown);
                        cboLogicalOperation.CreateControl();

                        cboLogicalOperation.DataSource = new BindingSource(logicalOperators, null);
                        cboLogicalOperation.DisplayMember = "value";
                        cboLogicalOperation.ValueMember = "key";

                        tblExpressions.Controls.Add(cboLogicalOperation, DataTableExpressionItemType.SrNoLabel, i);
                    }
                }
                else
                {
                    Label lblLogicalOperation = new Label();
                    lblLogicalOperation.Font = new System.Drawing.Font(lblLogicalOperation.Font, FontStyle.Bold);
                    lblLogicalOperation.Text = "";
                    lblLogicalOperation.Dock = DockStyle.Fill;
                    tblExpressions.Controls.Add(lblLogicalOperation, DataTableExpressionItemType.SrNoLabel, tblExpressions.RowCount);
                }

            }
            // Filter logic added
            else
            {
                foreach (Control ctl in tblExpressions.Controls)
                {
                    int controlRow = tblExpressions.GetRow(ctl);
                    Control LogicalOperation = tblExpressions.GetControlFromPosition(DataTableExpressionItemType.SrNoLabel, controlRow);
                    tblExpressions.Controls.Remove(LogicalOperation);
                }
                for (int i = 1; i <= tblExpressions.RowCount; i++)
                {
                    Label lblSerialNumber = new Label();

                    lblSerialNumber.Font = new System.Drawing.Font(lblSerialNumber.Font, FontStyle.Bold);
                    lblSerialNumber.Text = Convert.ToString(i);
                    lblSerialNumber.Height = 23;
                    lblSerialNumber.TextAlign = ContentAlignment.MiddleCenter;
                    tblExpressions.Controls.Add(lblSerialNumber, DataTableExpressionItemType.SrNoLabel, i);
                }
            }
        }

        private bool IsValidSearchFilter(SearchFilter sf)
        {
            bool result = false;
            if (sf != null && sf.FieldId != null)
                result = true;

            return result;
        }

        private bool ValidateFilterValues(SearchFilter sf, out string errorMessage)
        {
            errorMessage = string.Empty;

            ApttusObject appObject = applicationDefinitionManager.GetAppObject(sf.AppObjectUniqueId);
            ApttusField apttusField = appObject.Fields.Where(f => f.Id.Equals(sf.FieldId)).FirstOrDefault();

            if (apttusField == null)
                return true;

            // Enforce date validation only if there is some value in Value param, if blank evaluate condition for blank value
            // Evaluating date as blank values is allowed in Salesforce for Equals and Not Equals and hence allowed in Product.
            bool isValidBlankDateExpr = string.IsNullOrEmpty(sf.Value) && (sf.Operator.Equals("=") || sf.Operator.Equals("!="));
            if ((apttusField.Datatype == Datatype.Date || apttusField.Datatype == Datatype.DateTime) &&
                 sf.ValueType == ExpressionValueTypes.Static && !isValidBlankDateExpr)
            {
                bool isDateValid = true;

                if (sf.Operator.ToUpper().Equals("IN") && sf.Value.Trim().IndexOf("\r\n") > 0)
                {
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = sf.Value.Split(stringSeparators, StringSplitOptions.None);
                    foreach (var item in lines)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (string.IsNullOrEmpty(Utils.IsValidDate(item.Trim(), apttusField.Datatype)))
                            {
                                isDateValid = false;
                                break;
                            }
                        }
                    }

                }
                else if (sf.Operator.ToUpper().Equals("IN") && string.IsNullOrEmpty(Utils.IsValidDate(sf.Value.Trim(), apttusField.Datatype)))
                    isDateValid = false;
                else if (sf.Operator.ToUpper().Equals("NOT IN") && sf.Value.Trim().IndexOf("\r\n") > 0)
                {
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = sf.Value.Split(stringSeparators, StringSplitOptions.None);
                    foreach (var item in lines)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (string.IsNullOrEmpty(Utils.IsValidDate(item.Trim(), apttusField.Datatype)))
                            {
                                isDateValid = false;
                                break;
                            }
                        }
                    }
                }
                else if (sf.Operator.ToUpper().Equals("NOT IN") && string.IsNullOrEmpty(Utils.IsValidDate(sf.Value.Trim(), apttusField.Datatype)))
                    isDateValid = false;

                if (!isDateValid)
                {
                    errorMessage = String.Format(resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesValidFormat_ErrMsg"), apttusField.Name);
                    return false;
                }
            }
            return true;
        }

        private T GetExpressionItem<T>(object sender, DataTableExpressionItemType expressionItemType)
        {
            int row = tblExpressions.GetRow(sender as Control);
            return GetExpressionItem<T>(row, expressionItemType);
        }

        private T GetExpressionItem<T>(int row, DataTableExpressionItemType expressionItemType)
        {
            T result = default(T);
            try
            {
                switch (expressionItemType)
                {
                    case DataTableExpressionItemType.SrNoLabel:
                    case DataTableExpressionItemType.ObjectFieldText:
                    case DataTableExpressionItemType.OperatorPicklist:
                    case DataTableExpressionItemType.ValueTypePicklist:
                        result = (T)Convert.ChangeType(tblExpressions.GetControlFromPosition(expressionItemType, row), typeof(T)); // Return the Control
                        break;
                    case DataTableExpressionItemType.Value:
                        // Control does not implement IConvertible and so can't use (T)Convert.ChangeType. We need to do a double cast for Value Control.
                        result = (T)(object)tblExpressions.GetControlFromPosition(expressionItemType, row);
                        break;
                    case DataTableExpressionItemType.SearchFilter:
                        ComboBox txtObjectAndField = tblExpressions.GetControlFromPosition(DataTableExpressionItemType.ObjectFieldText, row) as ComboBox;
                        result = (T)Convert.ChangeType(txtObjectAndField.Tag, typeof(T)); // Return the Search Filter
                        break;
                }
            }
            catch (InvalidCastException ex)
            {
                throw ex;
            }

            return result;
        }

        private void AddOrReplaceExpressionItem<T>(object sender, Control newControl, DataTableExpressionItemType expressionItemType)
        {
            int row = tblExpressions.GetRow(sender as Control);
            AddOrReplaceExpressionItem<T>(row, newControl, expressionItemType);
        }

        private void AddOrReplaceExpressionItem<T>(int row, Control newControl, DataTableExpressionItemType expressionItemType)
        {
            try
            {
                tblExpressions.SuspendLayout();
                Control existingControl = GetExpressionItem<Control>(row, expressionItemType);
                tblExpressions.Controls.Remove(existingControl);
                tblExpressions.Controls.Add(newControl, expressionItemType, row);
                tblExpressions.Controls.SetChildIndex(newControl, (int)expressionItemType);

                if (newControl.GetType() == typeof(ComboBox) && !newControl.Enabled)
                    (newControl as ComboBox).SelectedIndex = -1;
            }
            catch (Exception ex) { }
            finally
            {
                tblExpressions.ResumeLayout();
            }
        }

        private void UpdateCurrentExpression(object sender, SearchFilter searchFilter)
        {
            int row = tblExpressions.GetRow(sender as Control);
            UpdateCurrentExpression(row, searchFilter);
        }

        public void UpdateCurrentExpression(int row, SearchFilter searchFilter)
        {
            ComboBox txtObjectAndField = GetExpressionItem<ComboBox>(row, DataTableExpressionItemType.ObjectFieldText);
            txtObjectAndField.Tag = searchFilter;
        }

        /// <summary>
        /// Validate Filters
        /// </summary>
        /// <returns></returns>
        public string ValidateFilters(SearchFilterGroup searchFilterGroup)
        {
            string errorMessage = string.Empty;
            bool IsValidExpression = false;
            StringBuilder sbErrorMessages = new StringBuilder();
            //System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(@"^[^()]*(?>(?>(?'open'\()[^()]*)+(?>(?'-open'\))[^()]*)+)+(?(open)(?!))$");
            Regex rgx = new Regex(@"^(?=^[^()]*(?>[^()]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))[^()]*$)[(]*[0-9]+[)]*(\s+((?i)and|or)\s+[(]*[0-9]+[)]*)*$");
            IsValidExpression = ValidateExpression(searchFilterGroup.FilterLogicText);
            if (!rgx.IsMatch(searchFilterGroup.FilterLogicText))
            {
                sbErrorMessages.AppendLine(resourceManager.GetResource("EXPREBUILDVIEW_ValidateFiltersLogicIncorrect_Msg"));
            }
            else if (IsValidExpression)
                sbErrorMessages.AppendLine(resourceManager.GetResource("EXPREBUILDVIEW_ValidateFiltersParentheses_Msg"));
            else
            {
                // Get Sequence number from the filtergrouptext  
                string[] arrfilterlogic = new string[Constants.EXPRESSION_BUILDER_MAX_ROWS];
                List<int> userProvidedSequenceNos = new List<int>();
                string RegExPattern = @"(and)+|(or)+|\(|\)|[0-9]+";
                foreach (Match matchFilterLogicChar in Regex.Matches(txtFilterLogic.Text, RegExPattern, RegexOptions.IgnoreCase))
                {
                    //foreach (char filterLogic in txtFilterLogic.Text)
                    //{
                    int sequenceNo;
                    if (int.TryParse(matchFilterLogicChar.Value, out sequenceNo))
                    {
                        if (searchFilterGroup.Filters.Any(sf => sf.SequenceNo == sequenceNo))
                            userProvidedSequenceNos.Add(sequenceNo);
                        else
                            sbErrorMessages.AppendLine(String.Format(resourceManager.GetResource("EXPREBUILDVIEW_ValidateFiltersNotValid_Msg"), sequenceNo.ToString()));
                    }
                }

                // Find if there are any Sequence No's from the Filter which are missing in the Filter Logic Text
                var missingSequenceNos = (from sf in searchFilterGroup.Filters
                                          select sf.SequenceNo)
                                          .Except
                                          (from sr in userProvidedSequenceNos
                                           select sr);
                foreach (var missingSequenceNo in missingSequenceNos)
                    sbErrorMessages.AppendLine(String.Format(resourceManager.GetResource("EXPREBUILDVIEW_ValidateFiltersLogic_Msg"), missingSequenceNo.ToString()));
            }

            if (sbErrorMessages.Length > 0)
                errorMessage = sbErrorMessages.ToString();

            return errorMessage;
        }

        /// <summary>
        /// Validate Expression for conjunction And OR operator
        /// </summary>
        /// <returns></returns>
        private bool ValidateExpression(string strExpression)
        {
            bool blnValidate = false;

            // For extract value from opening and closing brackets
            Regex rgxBracket = new Regex(@"(([^(|)]*))");
            foreach (Match match in rgxBracket.Matches(strExpression))
            {
                if ((match.Value.ToUpper().Contains("AND")) && (match.Value.ToUpper().Contains("OR")))
                {
                    blnValidate = true;
                    break;
                }
            }
            return blnValidate;
        }

        private void BindValueTypes(ComboBox cboValueType, ApttusObject apttusObject, ApttusField selectedField)
        {
            List<ExpressionValueTypes> valueTypes = GetAvailableValueTypes(apttusObject, selectedField);

            //Dictionary<int, string> listValueTypes = new Dictionary<int, string>();
            List<ValueType> listValueTypes = new List<ValueType>();
            foreach (ExpressionValueTypes valueType in valueTypes)
            {
                listValueTypes.Add(new ValueType { Key = valueType, Value = Utils.GetEnumDescription(valueType, string.Empty) });
            }
            cboValueType.DataSource = new BindingSource(listValueTypes, null);
            cboValueType.DisplayMember = "Value";
            cboValueType.ValueMember = "Key";
        }

        private List<ExpressionValueTypes> GetAvailableValueTypes(ApttusObject apttusObject, ApttusField apttusField)
        {
            List<ExpressionValueTypes> valueTypes = null;

            if (apttusField == null)
            {
                valueTypes = Enum.GetValues(typeof(ExpressionValueTypes)).OfType<ExpressionValueTypes>().ToList();
            }
            else
            {
                Datatype selectedFieldDataType = apttusField.Id.Equals(apttusObject.IdAttribute) ? Datatype.Lookup : apttusField.Datatype;

                if (!string.IsNullOrEmpty(apttusField.FormulaType))
                {
                    selectedFieldDataType = ExpressionBuilderHelper.GetDataTypeFromFormulaType(apttusField.FormulaType);
                }

                valueTypes = (from f in filterOperators
                              from op in f.SupportedValueTypes
                              where f.Datatype == selectedFieldDataType
                              select op).Distinct().ToList();
            }
            return valueTypes;
        }

        private void BindOperators(ComboBox cboOperators, ApttusObject obj, ApttusField selectedField, ExpressionValueTypes valueType)
        {
            Datatype selectedFieldDataType = selectedField.Id.Equals(obj.IdAttribute) ? Datatype.Lookup : selectedField.Datatype;

            if (!string.IsNullOrEmpty(selectedField.FormulaType))
            {
                selectedFieldDataType = ExpressionBuilderHelper.GetDataTypeFromFormulaType(selectedField.FormulaType);
            }

            List<DataTableFilter> dataTypeFilters = filterOperators.Where(op => op.Datatype == selectedFieldDataType).ToList();
            cboOperators.DataSource = new BindingSource(dataTypeFilters, null);
            cboOperators.DisplayMember = "Value";
            cboOperators.ValueMember = "Key";
        }

        private void PopulatePicklist(ComboBox cboPicklist, ApttusField selectedField)
        {
            if (selectedField.PicklistValues != null && selectedField.PicklistValues.Count > 0)
            {
                List<string> freshList = new List<string>();
                freshList.AddRange(selectedField.PicklistValues);
                cboPicklist.DataSource = freshList;
            }
        }

        private void PopulateBooleanCombo(ComboBox cboBoolean)
        {
            var items = new BindingList<KeyValuePair<bool, string>>();

            items.Add(new KeyValuePair<bool, string>(true, "True"));
            items.Add(new KeyValuePair<bool, string>(false, "False"));

            cboBoolean.DataSource = items;
            cboBoolean.ValueMember = "Key";
            cboBoolean.DisplayMember = "Value";
        }

        private void PopulateLookupFields(ComboBox cboLookupFields, ApttusField selectedField, SearchFilter searchFilter)
        {
            string selectedOperator = searchFilter.Operator;

            if (selectedField.LookupObject != null)
            {
                List<ApttusObject> lookupObjects = applicationDefinitionManager.GetAppObjectById(selectedField.LookupObject.Id);
                List<EBLookupObject> LookupObjects = new List<EBLookupObject>();

                foreach (ApttusObject lookupObject in lookupObjects)
                {
                    if (lookupObject != null)
                    {
                        if ((selectedOperator.Equals("=") && (lookupObject.ObjectType == ObjectType.Independent)) ||
                            (selectedOperator.Equals("!=") && (lookupObject.ObjectType == ObjectType.Independent)) ||
                            (selectedOperator.Equals("in") && (lookupObject.ObjectType == ObjectType.Repeating)) ||
                            (selectedOperator.Equals("not in") && (lookupObject.ObjectType == ObjectType.Repeating)))
                        {
                            LookupObjects.Add(
                                new EBLookupObject()
                                {
                                    DisplayMember = lookupObject.Name + Constants.DOT + lookupObject.Fields.FirstOrDefault(f => f.Id == lookupObject.IdAttribute).Name,
                                    ValueMember = lookupObject.UniqueId + Constants.DOT + lookupObject.Fields.FirstOrDefault(f => f.Id == lookupObject.IdAttribute).Id
                                }
                            );
                        }
                    }
                }

                if (LookupObjects.Count == 0)
                    return;

                cboLookupFields.DataSource = LookupObjects;
                cboLookupFields.DisplayMember = "DisplayMember";
                cboLookupFields.ValueMember = "ValueMember";
            }
        }

        private void PopulateChildParentLookup(ComboBox cboLookupFields, ApttusObject appObject)
        {
            List<EBLookupObject> LookupObjects = new List<EBLookupObject>();

            List<ApttusObject> allObjects = applicationDefinitionManager.GetParentAndChildObjects(applicationDefinitionManager.AppObjects);

            foreach (ApttusObject obj in allObjects)
            {
                foreach (ApttusField ofield in obj.Fields)
                {
                    if ((ofield.Datatype == Datatype.Lookup) && (ofield.LookupObject != null && ofield.LookupObject.Id == appObject.Id))
                    {
                        LookupObjects.Add(new EBLookupObject()
                        {
                            DisplayMember = applicationDefinitionManager.GetObjectNameById(obj.UniqueId) + "." + ofield.Name,
                            ValueMember = obj.UniqueId + "." + ofield.Id
                        }
                        );
                    }
                    // Self Join support
                    if ((appObject.UniqueId != obj.UniqueId) && (appObject.Id == obj.Id) && ofield.Id == obj.IdAttribute)
                    {
                        LookupObjects.Add(new EBLookupObject()
                        {
                            DisplayMember = applicationDefinitionManager.GetObjectNameById(obj.UniqueId) + "." + ofield.Name,
                            ValueMember = obj.UniqueId + "." + ofield.Id
                        }
                        );
                    }


                }
            }

            if (LookupObjects.Count == 0)
                return;

            cboLookupFields.DataSource = LookupObjects;
            cboLookupFields.DisplayMember = "DisplayMember";
            cboLookupFields.ValueMember = "ValueMember";
        }

        private void PopulateInValuesLinkLabel(LinkLabel llInValues, string value)
        {
            llInValues.Tag = value;
            bool isValueEmpty = string.IsNullOrEmpty(value);
            llInValues.Text = isValueEmpty ? resourceManager.GetResource("COMMON_AddValues_Text") : resourceManager.GetResource("COMMON_UpdateValues_Text");
            llInValues.LinkColor = isValueEmpty ? Color.Orange : Color.DarkOliveGreen;
        }

        void btnDeleteRow_Clicked(object sender, EventArgs e)
        {
            try
            {
                tblExpressions.SuspendLayout();

                List<HoldingCell> tempHolding = new List<HoldingCell>();
                HoldingCell cell;

                int row = tblExpressions.GetRow(sender as Button);

                SearchFilter searchFilterToDelete = GetExpressionItem<SearchFilter>(row, DataTableExpressionItemType.SearchFilter);

                // If the last row is deleted, then reset the expression builder
                if (row == 1 && tblExpressions.RowCount == 2)
                {
                    ResetExpressionBuilder();
                }
                else
                {
                    // Delete all controls on selected row 
                    for (int columnIndex = 0; columnIndex < tblExpressions.ColumnCount; columnIndex++)
                    {
                        var control = tblExpressions.GetControlFromPosition(columnIndex, row);
                        tblExpressions.Controls.Remove(control);
                    }

                    // Temporarily store the positions of all controls below the selected row
                    for (Int32 rowholding = row + 1; rowholding <= tblExpressions.RowCount; rowholding++)
                    {
                        for (Int32 col = 0; col <= tblExpressions.ColumnCount - 1; col++)
                        {
                            cell = new HoldingCell();
                            cell.cntrl = tblExpressions.GetControlFromPosition(col, rowholding);
                            //setup position for restore = current row -1
                            cell.tableLayoutPanelCellPosition = new TableLayoutPanelCellPosition(col, rowholding - 1);
                            tempHolding.Add(cell);
                        }
                    }

                    // Adjust control positions of all controls below the selected row
                    foreach (HoldingCell holdingCell in tempHolding)
                    {
                        cell = holdingCell;
                        if (cell.cntrl != null)
                        {
                            tblExpressions.SetCellPosition(cell.cntrl, cell.tableLayoutPanelCellPosition);
                        }
                    }
                    tempHolding = null;

                    // Delete the row and reduce the RowCount
                    tblExpressions.RowCount--;

                    // Reset Sequence number on 1) Label and 2) Search Filter
                    for (int i = row; i < tblExpressions.RowCount; i++)
                    {
                        // Update Label
                        Label lblSerialNumber = GetExpressionItem<Label>(i, DataTableExpressionItemType.SrNoLabel);
                        lblSerialNumber.Text = i.ToString();
                        // Update Search Filter
                        SearchFilter searchFilter = GetExpressionItem<SearchFilter>(i, DataTableExpressionItemType.SearchFilter);
                        searchFilter.SequenceNo = i;
                        UpdateCurrentExpression(i, searchFilter);
                    }
                }

                lnkAddRow.Visible = tblExpressions.RowCount <= Constants.EXPRESSION_BUILDER_MAX_ROWS;
            }
            finally
            {
                tblExpressions.ResumeLayout();
            }
        }

        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Label label1 = new Label();
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is ApttusObject)
                {
                    label1.Text = ((ApttusObject)currentItem).Name;
                    newWidth = label1.PreferredWidth;
                }
                else if (currentItem is ApttusField)
                {
                    label1.Text = ((ApttusField)currentItem).Name;
                    newWidth = label1.PreferredWidth;
                }
                else if (currentItem is EBLookupObject)
                {
                    label1.Text = ((EBLookupObject)currentItem).DisplayMember;
                    newWidth = label1.PreferredWidth;
                }
                else if (currentItem is string)
                {
                    label1.Text = ((String)currentItem);
                    newWidth = label1.PreferredWidth;
                }
                else if (currentItem is DataTableFilter)
                {
                    label1.Text = (currentItem as DataTableFilter).Value;
                    newWidth = label1.PreferredWidth;
                }// KeyValue pairs were not supported before. It's added to fit relational field's name in combo box
                else if (currentItem is KeyValuePair<string, string>)
                {
                    string[] splits = currentItem.ToString().Split(new char[] { ',' });
                    label1.Text = splits.Length > 1 ? splits[1] : splits[0];
                    newWidth = label1.PreferredWidth;
                }
                if (newWidth > width)
                    width = newWidth + 5;
            }
            label1.Dispose();
            senderComboBox.DropDownWidth = width;
        }

        private void lnkAddRow_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateExpressionRow(null);
        }

        private void lnkClearAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetExpressionBuilder();
        }

        private void lnkAddRemoveFilterLogic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lnkAddRemoveFilterLogic.Text == resourceManager.GetResource("UCEXPRVIEW_lnkAddRemoveFilterLogic_Text"))
            {
                pnlFilterLogic.Visible = true;
                lnkAddRemoveFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_ClearFilterLogic_Text");
            }
            else if (lnkAddRemoveFilterLogic.Text == resourceManager.GetResource("UCEXPRVIEW_ClearFilterLogic_Text"))
            {
                txtFilterLogic.Clear();
                pnlFilterLogic.Visible = false;
                lnkAddRemoveFilterLogic.Text = resourceManager.GetResource("UCEXPRVIEW_lnkAddRemoveFilterLogic_Text");
            }
            //CreateLogicalControl(true);
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            //this.lnkHelp.LinkVisited = true;
            // Navigate to a URL.
            //System.Diagnostics.Process.Start("https://help.salesforce.com/htviewhelpdoc?id=working_with_advanced_filter_conditions_in_reports_and_list_views.htm&siteLang=en_US");
        }
    }
}
