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
using EBHelper = Apttus.XAuthor.Core.ExpressionBuilderHelper;

namespace Apttus.XAuthor.AppDesigner
{
    public enum ExpressionItemType : int
    {
        SrNoLabel = 0,
        ObjectFieldText = 1,
        ObjectFieldBrowse = 2,
        ValueTypePicklist = 3,
        OperatorPicklist = 4,
        Value = 5,
        Delete = 6,
        SearchFilter = 100 // This won't be a control
    }

    public enum ResetAfter
    {
        AllBlank = 0,
        ObjectFieldSelection = 1,
        InputSelection = 2,
        OperatorSelection = 3
    }

    public partial class ExpressionBuilderView : UserControl
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        private Dictionary<string, string> logicalOperators = new Dictionary<string, string>();
        private Control CurrentLoadedUserControl { get; set; }
        public List<SearchFilterGroup> Model { get; set; }
        public ApttusObject TargetObject { get; set; }
        private List<Filters> filterOperators = new List<Filters>();

        // private const string AddFilterLogicText = "Add Filter Logic";
        // private const string ClearFilterLogicText = "Clear Filter Logic";

        private QueryLabelView queryLabelView = new QueryLabelView();

        public ExpressionBuilderView()
        {
            IsExpressionBuilderLaunchedFromQuickApp = false;
            InitializeComponent();
            this.CurrentLoadedUserControl = this;
            InitializeValues();
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
            filterOperators = Filters.InitializeFilters();
        }

        public void SetExpressionBuilder(List<SearchFilterGroup> model, ApttusObject TargetObject)
        {
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
            try
            {
                tblExpressions.SuspendLayout();
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
            catch (Exception ex) { }
            finally
            {
                tblExpressions.ResumeLayout();
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
            tblExpressions.SuspendLayout();

            if (bDummyRowAdded)
                RemoveDummyRow();

            if (searchFilter == null)
                searchFilter = new SearchFilter { SequenceNo = tblExpressions.RowCount };
            else
                UpdateSearchFilterForBackwardCompatibility(searchFilter);

            int rowNo = tblExpressions.RowCount;
            ApttusObject apttusObject = EBHelper.GetSearchFilterObject(searchFilter);
            ApttusField apttusField = EBHelper.GetSearchFilterField(searchFilter);

            // Column 1 - Serial Number
            Label lblSerialNumber = new Label();

            //lblSerialNumber.Font = new System.Drawing.Font(lblSerialNumber.Font, FontStyle.Bold);
            lblSerialNumber.Text = searchFilter.SequenceNo.ToString();
            lblSerialNumber.AutoSize = true;
            lblSerialNumber.Margin = new Padding(3, 3, 3, 3);

            // Column 2 - Object and Field Text
            TextBox txtObjectAndField = new TextBox();
            txtObjectAndField.Dock = DockStyle.Fill;
            txtObjectAndField.BackColor = Color.White;
            txtObjectAndField.ReadOnly = true;
            txtObjectAndField.Tag = searchFilter;
            txtObjectAndField.CreateControl();

            // 2.1 Bind the Search Filter to the Object and Field Text
            txtObjectAndField.Tag = searchFilter;

            // 2.2 Set Value for Column 2 - Object and Field Text
            txtObjectAndField.Text = ExpressionBuilderHelper.GetDisplayTextFromQueryObjects(searchFilter);
            txtObjectAndField.MouseHover += new System.EventHandler(txtObjectAndField_MouseHover);
            txtObjectAndField.MouseLeave += new System.EventHandler(txtObjectAndField_MouseLeave);

            // Column 3 - Object and Field Browse
            Button btnObjectAndFieldBrowse = new Button();
            //btnObjectAndFieldBrowse.Dock = DockStyle.Fill;
            btnObjectAndFieldBrowse.FlatStyle = FlatStyle.Flat;
            btnObjectAndFieldBrowse.FlatAppearance.BorderColor = Color.Silver;
            btnObjectAndFieldBrowse.BackColor = Color.Transparent;
            btnObjectAndFieldBrowse.Image = Properties.Resources.levels;
            btnObjectAndFieldBrowse.Click += new EventHandler(btnObjectAndFieldBrowse_Click);
            btnObjectAndFieldBrowse.Size = new System.Drawing.Size(24, 24);

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

            tblExpressions.Controls.Add(lblSerialNumber, ExpressionItemType.SrNoLabel, rowNo);
            tblExpressions.Controls.Add(txtObjectAndField, ExpressionItemType.ObjectFieldText, rowNo);
            tblExpressions.Controls.Add(btnObjectAndFieldBrowse, ExpressionItemType.ObjectFieldBrowse, rowNo);
            tblExpressions.Controls.Add(cboValueType, ExpressionItemType.ValueTypePicklist, rowNo);
            tblExpressions.Controls.Add(cboOperator, ExpressionItemType.OperatorPicklist, rowNo);
            tblExpressions.Controls.Add(valueControl, ExpressionItemType.Value, rowNo);
            tblExpressions.Controls.Add(btnDeleteRow, ExpressionItemType.Delete, rowNo);

            // Important Developer Note: The combobox controls can only be bounded to data, once they are added to the parent control (tblExpressions).
            // Once we bind the filter value types, the cboValueType combobox, SelectedIndexChange event is fired, which in turn fires the cboOperator's SelectedIndexChange event.
            // All these events will be bypassed when we bind the data from the model or in empty row addition.

            if (apttusField != null)
            {
                // Column 4 - Bind and Set value for Value Type Combobox
                cboValueType.SelectedIndexChanged -= cboValueType_SelectedIndexChanged;
                cboOperator.SelectedIndexChanged -= cboOperators_SelectedIndexChanged;

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

                // Column 5 - Bind and Set value for Operators Combobox
                // 5.1 Bind if its existing record
                cboOperator.SelectedIndexChanged -= cboOperators_SelectedIndexChanged;
                BindOperators(cboOperator, apttusObject, apttusField, (ExpressionValueTypes)cboValueType.SelectedValue);
                cboOperator.SelectedIndex = -1; //this is needed. Don't Modify.
                cboOperator.SelectedIndexChanged += cboOperators_SelectedIndexChanged;

                // 5.2 Set Value for Column 5 - Operators set value only if Apttus Field exits
                cboOperator.SelectedItem = cboOperator.Items.OfType<Filters>().SingleOrDefault(f => f.Key.Equals(searchFilter.Operator));
            }

            // Column 6 - Bind and Set Value Control
            if (apttusObject != null && apttusField != null)
            {
                // 6.1 Bind Value Control
                //Once we set the cboOperator SelectedItem property, cboOperator_SelectedIndexChanged Event is fired and hence the correct value control is loaded and ready for data population.
                //So just get the control and set the data appropriately.
                valueControl = tblExpressions.GetControlFromPosition(ExpressionItemType.Value, searchFilter.SequenceNo);

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

        private void UpdateSearchFilterForBackwardCompatibility(SearchFilter searchFilter)
        {
            if (searchFilter != null && searchFilter.QueryObjects != null && searchFilter.QueryObjects.Count == 0
                && !searchFilter.AppObjectUniqueId.Equals(Guid.Empty) && !string.IsNullOrEmpty(searchFilter.FieldId))
            {
                ApttusObject appObject = applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId);

                searchFilter.QueryObjects.Add(new QueryObject
                {
                    SequenceNo = searchFilter.QueryObjects.Count + 1,
                    AppObjectUniqueId = searchFilter.AppObjectUniqueId,
                    LookupFieldId = string.Empty,
                    RelationshipType = QueryRelationshipType.None,
                    BreadCrumbLabel = appObject.Name,
                    BreadCrumbTooltip = appObject.Name,
                    DistanceFromChild = 0,
                    LeafAppObjectUniqueId = searchFilter.AppObjectUniqueId
                });
            }
        }

        private void txtObjectAndField_MouseHover(object sender, EventArgs e)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);
            queryLabelView.LeftParent = false;

            if (searchFilter.ValueType == ExpressionValueTypes.UserInput)
            {
                TextBox txtObjectAndField = GetExpressionItem<TextBox>(sender, ExpressionItemType.ObjectFieldText);
                Point locationOnForm = txtObjectAndField.Parent.PointToScreen(txtObjectAndField.Location);
                locationOnForm.X -= 3;
                locationOnForm.Y -= 48;
                queryLabelView.Location = locationOnForm;
                queryLabelView.ShowForm(searchFilter, this, tblExpressions.GetRow(sender as Control));
            }
        }

        private void txtObjectAndField_MouseLeave(object sender, EventArgs e)
        {
            queryLabelView.LeftParent = true;
        }

        private void btnObjectAndFieldBrowse_Click(object sender, EventArgs e)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);
            ApttusObject filterObject = EBHelper.GetSearchFilterObject(searchFilter);
            if (filterObject == null)
                filterObject = TargetObject;
            // Launch Object and Field picker
            ObjectAndFieldBrowserView view = new ObjectAndFieldBrowserView();
            ObjectAndFieldBrowserController controller = new ObjectAndFieldBrowserController(view, applicationDefinitionManager.GetAppObject(TargetObject.UniqueId), searchFilter);

            // Update the Object and Field text and Search Filter, if the user hit save on the dialog. Search Filter will already have new values.
            if (controller.Browse())
            {
                // In this If block searchFilter already has the values captured from Object and Field Browser
                var valueTypes = GetAvailableValueTypes(filterObject, EBHelper.GetSearchFilterField(searchFilter));
                if (valueTypes != null)
                    searchFilter.ValueType = valueTypes.First();
                TextBox txtObjectAndField = GetExpressionItem<TextBox>(sender, ExpressionItemType.ObjectFieldText);
                txtObjectAndField.Text = ExpressionBuilderHelper.GetDisplayTextFromQueryObjects(searchFilter);
                txtObjectAndField.Tag = searchFilter;

                ResetCurrentFilter(sender, ResetAfter.ObjectFieldSelection);
            }
        }

        void cboValueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Search Filter
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);
            ExpressionValueTypes valueType;
            if (Enum.TryParse((sender as ComboBox).SelectedValue.ToString(), out valueType))
                searchFilter.ValueType = valueType; // Enum.Parse(typeof(ExpressionValueTypes), (sender as ComboBox).SelectedValue.ToString(), true); // ((KeyValuePair<ExpressionValueTypes, string>)((sender as ComboBox).SelectedValue)).Key;
            UpdateCurrentExpression(sender, searchFilter);
            if (searchFilter.ValueType == ExpressionValueTypes.UserInput)
            {
                //Reset Values after selection                
                int row = tblExpressions.GetRow(sender as Control);
                Control LabelValueControl = tblExpressions.GetControlFromPosition(ExpressionItemType.ObjectFieldText, row);
                searchFilter.SearchFilterLabel = GetValue(LabelValueControl);

                Control valueControl = tblExpressions.GetControlFromPosition(ExpressionItemType.Value, row);
                searchFilter.Value = GetValue(valueControl);
            }
            // Reset the Layout
            ResetCurrentFilter(sender, ResetAfter.InputSelection);
        }

        void cboOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update Search Filter
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);
            searchFilter.Operator = ((Filters)(sender as ComboBox).SelectedItem).Key;
            UpdateCurrentExpression(sender, searchFilter);

            // Reset the Layout
            ResetCurrentFilter(sender, ResetAfter.OperatorSelection);
        }

        private void llInValues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);

            // Launch Object and Field picker
            MultiLineCaptureView view = new MultiLineCaptureView();
            MultiLineCaptureController controller = new MultiLineCaptureController(view, EBHelper.GetSearchFilterObject(searchFilter), EBHelper.GetSearchFilterField(searchFilter), searchFilter.Value);

            // Update the Object and Field text and Search Filter, if the user hit save on the dialog. Search Filter will already have new values.
            if (controller.Browse())
            {
                LinkLabel llInValues = GetExpressionItem<LinkLabel>(sender, ExpressionItemType.Value);
                searchFilter.Value = controller.MultiLineValue;

                PopulateInValuesLinkLabel(llInValues, searchFilter.Value);
                UpdateCurrentExpression(sender, searchFilter);
            }
        }

        private void ResetCurrentFilter(object sender, ResetAfter resetAfter)
        {
            SearchFilter searchFilter = GetExpressionItem<SearchFilter>(sender, ExpressionItemType.SearchFilter);
            ApttusObject apttusObject = EBHelper.GetSearchFilterObject(searchFilter);
            ApttusField apttusField = EBHelper.GetSearchFilterField(searchFilter);
            Control valueControl = GetExpressionItem<Control>(sender, ExpressionItemType.Value);
            ComboBox cboType = GetExpressionItem<ComboBox>(sender, ExpressionItemType.ValueTypePicklist);
            ComboBox cboOperator = GetExpressionItem<ComboBox>(sender, ExpressionItemType.OperatorPicklist);
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
                    AddOrReplaceExpressionItem<ComboBox>(sender, newValueControl, ExpressionItemType.Value);

                    break;
                    #endregion

                case ResetAfter.InputSelection:

                    #region "ResetAfter.InputSelection"

                    // 2. Reset the Operator Picklist

                    // Add filter operator for type as static and field is picklist
                    bool isAllInList = filterOperators.Any(a => a.Datatype == Datatype.Picklist && (a.Key == "like #FILTERVALUE%" || a.Key == "like %#FILTERVALUE%" || a.Key == "like %#NOTFILTERVALUE%" || a.Key == "in" || a.Key == "not in"));
                    if (((ValueType)cboType.SelectedItem).Key == ExpressionValueTypes.Input && apttusField.Datatype == Datatype.Picklist && isAllInList)
                    {
                        //filterOperators.RemoveAll(a => a.Key.Any(item => a.Datatype == Datatype.Picklist && (a.Key == "like #FILTERVALUE%" || a.Key == "like %#FILTERVALUE%" || a.Key == "in")));
                        // TODO:: need to move filteroperators.add to Filters.cs class, also extend filters to consider Value Type (i.e. Input, Static, SysVar, UserInput)
                        //filterOperators.RemoveAll(a => a.Key.Any(item => a.Datatype == Datatype.Picklist && (a.Key == "like #FILTERVALUE%" || a.Key == "in")));
                        filterOperators.RemoveAll(a => a.Key.Any(item => a.Datatype == Datatype.Picklist && (a.Key == "like #FILTERVALUE%")));
                    }
                    else if (((ValueType)cboType.SelectedItem).Key == ExpressionValueTypes.Static && apttusField.Datatype == Datatype.Picklist && !isAllInList)
                    {
                        // Added check of if exists to prevent duplicate entries being added to filter operators.
                        // TODO:: need to move filteroperators.add to Filters.cs class, also extend filters to consider Value Type (i.e. Input, Static, SysVar, UserInput)
                        //        and load filters based on datatype plus value type. 
                        if (!filterOperators.Exists(f => f.Key.Equals("like #FILTERVALUE%") && f.Value.Equals("starts with") && f.Datatype.Equals(Core.Datatype.Picklist)))
                            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Picklist));
                        if (!filterOperators.Exists(f => f.Key.Equals("like %#FILTERVALUE%") && f.Value.Equals("contains") && f.Datatype.Equals(Core.Datatype.Picklist)))
                            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Picklist));
                        if (!filterOperators.Exists(f => f.Key.Equals("like %#NOTFILTERVALUE%") && f.Value.Equals("not contains") && f.Datatype.Equals(Core.Datatype.Picklist)))
                            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Picklist));
                        if (!filterOperators.Exists(f => f.Key.Equals("in") && f.Value.Equals("in") && f.Datatype.Equals(Core.Datatype.Picklist)))
                            filterOperators.Add(new Filters("in", "in", Core.Datatype.Picklist));
                        if (!filterOperators.Exists(f => f.Key.Equals("not in") && f.Value.Equals("not in") && f.Datatype.Equals(Core.Datatype.Picklist)))
                            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Picklist));
                    }
                    BindOperators(cboOperator, apttusObject, apttusField, ((ValueType)cboType.SelectedItem).Key);

                    break;
                    #endregion

                case ResetAfter.OperatorSelection:

                    #region "ResetAfter.OperatorSelection"

                    // 3. Reset the Value Control
                    newValueControl = GetValueControl(tblExpressions.GetRow(sender as Control), searchFilter, apttusObject, apttusField);
                    AddOrReplaceExpressionItem<ComboBox>(sender, newValueControl, ExpressionItemType.Value);

                    break;
                    #endregion

                default:
                    break;
            }
        }

        private Control GetValueControl(int row, SearchFilter searchFilter, ApttusObject apttusObject, ApttusField apttusField)
        {
            Control result = null;

            if (searchFilter.ValueType == ExpressionValueTypes.Input || searchFilter.ValueType == ExpressionValueTypes.UserInput)
            {
                // Column 5 - Create ComboBox
                ComboBox cboInputValueControl = new ComboBox();
                cboInputValueControl.Dock = DockStyle.Fill;
                cboInputValueControl.FlatStyle = FlatStyle.Standard;
                cboInputValueControl.DropDownStyle = ComboBoxStyle.DropDownList;
                cboInputValueControl.DropDown += new EventHandler(AdjustWidth_DropDown);
                cboInputValueControl.CreateControl();

                // Picklist
                if (apttusField.Datatype == Datatype.Picklist)
                {
                    PopulatePicklist(cboInputValueControl, apttusField);
                }
                else if (apttusField.Datatype == Datatype.Lookup || apttusField.Datatype == Datatype.Composite)
                {
                    PopulateLookupFields(cboInputValueControl, apttusField, searchFilter);
                }
                //else if (apttusObject.Children.Count > 0 && apttusField.Id == Constants.ID_ATTRIBUTE)
                else if (apttusField.Id == apttusObject.IdAttribute)
                {
                    // Same logic for "=", "!=" and "in"
                    PopulateChildParentLookup(cboInputValueControl, apttusObject);
                }
                else
                {
                    // User selected "Input" value type. It should be a blank ComboBox for any other datatypes.
                }
                result = cboInputValueControl;

            }
            else if (searchFilter.ValueType == ExpressionValueTypes.Static)
            {
                // "Static" with all operators except "In"
                if (searchFilter.Operator.ToUpper().Equals("IN") || searchFilter.Operator.ToUpper().Equals("NOT IN"))
                {
                    LinkLabel llInValues = new LinkLabel();
                    llInValues.AutoSize = true;
                    llInValues.TextAlign = ContentAlignment.BottomCenter;
                    llInValues.LinkClicked += new LinkLabelLinkClickedEventHandler(llInValues_LinkClicked);
                    llInValues.CreateControl();

                    PopulateInValuesLinkLabel(llInValues, searchFilter.Value);
                    result = llInValues;
                }
                else
                {
                    if (apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Boolean)
                    {
                        // "Picklist" and "Boolean" with all operators except "In" and "Contains"
                        if (!searchFilter.Operator.Equals("like %#FILTERVALUE%") && !searchFilter.Operator.Equals("like #FILTERVALUE%") && !searchFilter.Operator.Equals("like %#NOTFILTERVALUE%"))
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
                        else
                        {
                            // "Picklist" with "Contains" - Column 5 - Create TextBox
                            TextBox txtValue = new TextBox();
                            txtValue.BackColor = Color.White;
                            txtValue.Dock = DockStyle.Fill;

                            result = txtValue;
                        }
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
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.SystemVariables)
            {
                // Column 5 - Create ComboBox
                ComboBox cboVariables = new ComboBox();
                cboVariables.Dock = DockStyle.Fill;
                cboVariables.FlatStyle = FlatStyle.Standard;
                cboVariables.DropDownStyle = ComboBoxStyle.DropDownList;
                cboVariables.Name = "SystemVariables";
                cboVariables.DropDown += new EventHandler(AdjustWidth_DropDown);
                cboVariables.CreateControl();

                if (apttusField != null)
                    cboVariables.DataSource = Enum.GetValues(typeof(ExpressionSystemVariables));

                result = cboVariables;
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.CellReference)
            {
                TextBox txtValue = new TextBox();
                txtValue.Dock = DockStyle.Fill;
                txtValue.BackColor = Color.White;
                ////CueProvider.SetCue(txtValue, "e.g. Sheet1!A1");

                result = txtValue;
            }

            if (searchFilter.ValueType == ExpressionValueTypes.UserInput)
                result.Enabled = false;
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
            else if (control.GetType() == typeof(DateTimePicker))
            {

            }
            else if (control.GetType() == typeof(LinkLabel))
            {
                sValue = Convert.ToString(((LinkLabel)control).Tag);
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
                if (searchFilter.ValueType == ExpressionValueTypes.SystemVariables)
                {
                    if (searchFilter.Value.Equals(ExpressionSystemVariables.CurrentDate.ToString()))
                        ((ComboBox)control).SelectedIndex = Convert.ToInt32(ExpressionSystemVariables.CurrentDate);
                    else if (searchFilter.Value.Equals(ExpressionSystemVariables.CurrentUser.ToString()))
                        ((ComboBox)control).SelectedIndex = Convert.ToInt32(ExpressionSystemVariables.CurrentUser);
                    else if (searchFilter.Value.Equals(ExpressionSystemVariables.ExportRecordId.ToString()))
                        ((ComboBox)control).SelectedIndex = Convert.ToInt32(ExpressionSystemVariables.ExportRecordId);

                }
                else if (apttusField.Datatype == Datatype.Lookup)
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
            else if (control.GetType() == typeof(LinkLabel))
            {
                if (searchFilter.ValueType == ExpressionValueTypes.Static)
                {
                    PopulateInValuesLinkLabel((LinkLabel)control, searchFilter.Value);
                }
            }
            else if (control.GetType() == typeof(DateTimePicker))
            {

            }

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
                //if (tblExpressions.RowCount == 1 && (tblExpressions.GetControlFromPosition(2, 1) as ComboBox).SelectedItem == null)
                //    return null;
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
                        SearchFilter searchFilter = GetExpressionItem<SearchFilter>(i, ExpressionItemType.SearchFilter);
                        if (IsValidSearchFilter(searchFilter))
                        {
                            Control valueControl = tblExpressions.GetControlFromPosition(ExpressionItemType.Value, i);
                            searchFilter.Value = GetValue(valueControl);

                            // Repurpose search filter label in case of Cell reference type to hold Cell Location, 
                            // Search Filter label is UI Element and Cell Reference would never appear on UI
                            if (searchFilter.ValueType == ExpressionValueTypes.CellReference)
                                searchFilter.SearchFilterLabel = searchFilter.Value;

                            bValidSearchFilter = ValidateFilterValues(searchFilter, out filterErrorMessage);
                            if (!bValidSearchFilter)
                            {
                                errorMessages.AppendLine("Row #" + (i).ToString() + " - " + filterErrorMessage);
                            }
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
                    Control LogicalOperation = tblExpressions.GetControlFromPosition(ExpressionItemType.SrNoLabel, controlRow);
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

                        tblExpressions.Controls.Add(cboLogicalOperation, ExpressionItemType.SrNoLabel, i);
                    }
                }
                else
                {
                    Label lblLogicalOperation = new Label();
                    lblLogicalOperation.Font = new System.Drawing.Font(lblLogicalOperation.Font, FontStyle.Bold);
                    lblLogicalOperation.Text = "";
                    lblLogicalOperation.Dock = DockStyle.Fill;
                    tblExpressions.Controls.Add(lblLogicalOperation, ExpressionItemType.SrNoLabel, tblExpressions.RowCount);
                }

            }
            // Filter logic added
            else
            {
                foreach (Control ctl in tblExpressions.Controls)
                {
                    int controlRow = tblExpressions.GetRow(ctl);
                    Control LogicalOperation = tblExpressions.GetControlFromPosition(ExpressionItemType.SrNoLabel, controlRow);
                    tblExpressions.Controls.Remove(LogicalOperation);
                }
                for (int i = 1; i <= tblExpressions.RowCount; i++)
                {
                    Label lblSerialNumber = new Label();

                    lblSerialNumber.Font = new System.Drawing.Font(lblSerialNumber.Font, FontStyle.Bold);
                    lblSerialNumber.Text = Convert.ToString(i);
                    lblSerialNumber.Height = 23;
                    lblSerialNumber.TextAlign = ContentAlignment.MiddleCenter;
                    tblExpressions.Controls.Add(lblSerialNumber, ExpressionItemType.SrNoLabel, i);
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
            else if (sf.ValueType == ExpressionValueTypes.Input)
            {
                if (string.IsNullOrEmpty(sf.Value))
                {
                    errorMessage = String.Format(resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesValidValue_ErrMsg"), appObject.Name, apttusField.Name);
                    return false;
                }
            }
            else if (sf.ValueType == ExpressionValueTypes.CellReference)
            {
                // Validate the 2nd part of the expression after ! symbol
                bool isCellReferenceValid = false;
                bool isRangeValidforOperators = false;
                string cellReferenceValue = sf.Value;

                if ((!string.IsNullOrEmpty(cellReferenceValue)) && cellReferenceValue.LastIndexOf(@"!") > 0
                    && cellReferenceValue.LastIndexOf(@"!") < cellReferenceValue.Length)
                {
                    int sheetNameEnd = cellReferenceValue.LastIndexOf(@"!");
                    string sheetName = cellReferenceValue.Substring(0, sheetNameEnd);
                    var workSheet = ExcelHelper.GetWorkSheet(sheetName);
                    if (workSheet != null)
                    {
                        // Worksheet exists, Match the cell reference pattern
                        string cellLocation = cellReferenceValue.Substring(sheetNameEnd + 1);
                        Regex regex = new Regex(@"(\$?[A-Za-z]{1,3})(\$?[0-9]{1,6})");
                        isCellReferenceValid = regex.IsMatch(cellLocation);
                        // For Decimal case
                        if (cellLocation.IndexOf(@":") > 0 && !(sf.Operator == "in" || sf.Operator == "not in"))
                            isRangeValidforOperators = true;
                    }
                }

                else if (!isCellReferenceValid && !isRangeValidforOperators)
                {
                    Excel.Range cellReferenceRange = null;
                    Excel.Range oNameRange = ExcelHelper.GetRange(cellReferenceValue);
                    if (oNameRange == null)
                    {
                        Regex regex = new Regex(@"^='?[^/\[]*]*'?!{1}[A-Z]+[0-9]+$");
                        isCellReferenceValid = regex.IsMatch(cellReferenceValue);

                        if (isCellReferenceValid)
                        {
                            cellReferenceRange = ExcelHelper.GetRangeByLocation(cellReferenceValue);
                            string namedRange = ExcelHelper.GetExcelCellName(cellReferenceRange);
                            if (string.IsNullOrEmpty(namedRange))
                            {
                                errorMessage = resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesExists_ErrMsg");
                                return false;
                            }
                        }
                        else
                        {
                            errorMessage = String.Format(resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesCellRef_ErrMsg"), apttusField.Name);
                            return false;
                        }
                    }
                    else if (sf.Operator != "in" && sf.Operator != "not in")
                    {
                        errorMessage = resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesNameRange_ErrMsg");
                        return false;
                    }
                    return true;
                }

                if (!isCellReferenceValid)
                {
                    errorMessage = String.Format(resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValuesCellRef_ErrMsg"), apttusField.Name);
                    return false;
                }
                else if (isRangeValidforOperators)
                {
                    errorMessage = resourceManager.GetResource("EXPRESSIONBUILDVIEW_ValidateFilterValues_ErrMsg");
                    return false;
                }

            }
            return true;
        }

        private T GetExpressionItem<T>(object sender, ExpressionItemType expressionItemType)
        {
            int row = tblExpressions.GetRow(sender as Control);
            return GetExpressionItem<T>(row, expressionItemType);
        }

        private T GetExpressionItem<T>(int row, ExpressionItemType expressionItemType)
        {
            T result = default(T);
            try
            {
                switch (expressionItemType)
                {
                    case ExpressionItemType.SrNoLabel:
                    case ExpressionItemType.ObjectFieldText:
                    case ExpressionItemType.OperatorPicklist:
                    case ExpressionItemType.ValueTypePicklist:
                        result = (T)Convert.ChangeType(tblExpressions.GetControlFromPosition(expressionItemType, row), typeof(T)); // Return the Control
                        break;
                    case ExpressionItemType.Value:
                        // Control does not implement IConvertible and so can't use (T)Convert.ChangeType. We need to do a double cast for Value Control.
                        result = (T)(object)tblExpressions.GetControlFromPosition(expressionItemType, row);
                        break;
                    case ExpressionItemType.SearchFilter:
                        TextBox txtObjectAndField = tblExpressions.GetControlFromPosition(ExpressionItemType.ObjectFieldText, row) as TextBox;
                        result = (T)Convert.ChangeType(txtObjectAndField.Tag, typeof(T)); // Return the Search Filter
                        break;
                }
            }
            catch (InvalidCastException ex)
            {
                //result = default(T);
                throw ex;
            }

            return result;
        }

        private void AddOrReplaceExpressionItem<T>(object sender, Control newControl, ExpressionItemType expressionItemType)
        {
            int row = tblExpressions.GetRow(sender as Control);
            AddOrReplaceExpressionItem<T>(row, newControl, expressionItemType);
        }

        private void AddOrReplaceExpressionItem<T>(int row, Control newControl, ExpressionItemType expressionItemType)
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
            TextBox txtObjectAndField = GetExpressionItem<TextBox>(row, ExpressionItemType.ObjectFieldText);
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    PopulateFields(sender as ComboBox);
        //}

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

                if (selectedFieldDataType == Datatype.Formula)
                {
                    if (!string.IsNullOrEmpty(apttusField.FormulaType))
                    {
                        selectedFieldDataType = ExpressionBuilderHelper.GetDataTypeFromFormulaType(apttusField.FormulaType);
                    }
                }

                valueTypes = (from f in filterOperators
                              from op in f.SupportedValueTypes
                              where f.Datatype == selectedFieldDataType
                              select op).Distinct().ToList();
            }

            if (IsExpressionBuilderLaunchedFromQuickApp)
                valueTypes.RemoveAll(vt => vt.Equals(ExpressionValueTypes.CellReference));
            return valueTypes;
        }

        private void BindOperators(ComboBox cboOperators, ApttusObject obj, ApttusField selectedField, ExpressionValueTypes valueType)
        {
            Datatype selectedFieldDataType = selectedField.Id.Equals(obj.IdAttribute) ? Datatype.Lookup : selectedField.Datatype;

            if (selectedFieldDataType == Datatype.Formula)
            {
                if (!string.IsNullOrEmpty(selectedField.FormulaType))
                {
                    selectedFieldDataType = ExpressionBuilderHelper.GetDataTypeFromFormulaType(selectedField.FormulaType);
                }
            }

            List<Filters> dataTypeFilters = filterOperators.Where(op => op.Datatype == selectedFieldDataType).ToList();

            // Special handling for User Input and Cell Reference filters
            if (valueType == ExpressionValueTypes.UserInput || valueType == ExpressionValueTypes.CellReference)
            {
                // PICKLIST supports EQUAL and IN operator 
                // LOOKUP, MULTI_PICKLIST, BOOLEAN, NOTSUPPORTED support EQUALS operator
                // TEXTAREA, RICHTEXTAREA are not supposed in query builder.

                if (selectedFieldDataType == Datatype.Picklist)
                    dataTypeFilters.RemoveAll(filter => filter.Key != Constants.EQUALS && filter.Key != "in" && filter.Key != "not in");
                else if (selectedFieldDataType == Datatype.Lookup && valueType == ExpressionValueTypes.CellReference)
                { }
                else if (selectedFieldDataType == Datatype.Lookup || selectedFieldDataType == Datatype.Picklist_MultiSelect ||
                            selectedFieldDataType == Datatype.Boolean || selectedFieldDataType == Datatype.NotSupported)
                    dataTypeFilters.RemoveAll(filter => filter.Key != Constants.EQUALS);
            }
            else if (valueType == ExpressionValueTypes.SystemVariables)
                dataTypeFilters.RemoveAll(filter => filter.Key == "in" || filter.Key == "not in");

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
                List<ApttusObject> lookupObjects = new List<ApttusObject>();
                if (selectedField.MultiLookupObjects != null && selectedField.MultiLookupObjects.Count > 0)
                {
                    List<ApttusObject> objectList = selectedField.MultiLookupObjects;
                    foreach (ApttusObject refernceObject in objectList)
                    {
                        List<ApttusObject> matchAppObjects = applicationDefinitionManager.GetAppObjectById(refernceObject.Id);
                        if (matchAppObjects.Count > 0)
                        {
                            foreach (ApttusObject item in matchAppObjects)
                                lookupObjects.Add(item);
                        }

                    }

                }
                else
                    lookupObjects = applicationDefinitionManager.GetAppObjectById(selectedField.LookupObject.Id);

                List<EBLookupObject> LookupObjects = new List<EBLookupObject>();

                foreach (ApttusObject lookupObject in lookupObjects)
                {
                    if (lookupObject != null)
                    {
                        if ((selectedOperator.Equals("=") && (lookupObject.ObjectType == ObjectType.Independent )) ||
                            (selectedOperator.Equals("!=") && (lookupObject.ObjectType == ObjectType.Independent )) ||
                            (selectedOperator.Equals("in") && (lookupObject.ObjectType == ObjectType.Repeating )) ||
                            (selectedOperator.Equals("not in") && (lookupObject.ObjectType == ObjectType.Repeating )))
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

        //private void PopulateInputAction(ComboBox cboPicklist, ApttusObject apttusObject, ApttusField selectedField)
        //{
        //    List<InputActionModel> inputActions = ConfigurationManager.GetInstance.Actions.OfType<InputActionModel>().ToList();

        //    var variables = (from inputAction in inputActions
        //                     from variable in inputAction.InputActionVariables
        //                     where variable.FieldId.Equals(selectedField.Id) && variable.ObjectUniqueId.Equals(apttusObject.UniqueId)
        //                     select new
        //                     {
        //                         Key = new StringBuilder(inputAction.Name).Append(Constants.DOT).Append(variable.VariableName).ToString(),
        //                         Value = new StringBuilder(inputAction.Id).Append(Constants.DOT).Append(variable.VariableName).ToString()
        //                     }).ToList();

        //    if (variables.Count == 0)
        //        return;

        //    cboPicklist.DataSource = variables;
        //    cboPicklist.DisplayMember = Constants.KEY_COLUMN;
        //    cboPicklist.ValueMember = Constants.VALUE_COLUMN;
        //}

        //void btnDeleteRow_Clicked(object sender, EventArgs e)
        //{
        //    //List<HoldingCell> tempHolding = new List<HoldingCell>();
        //    //HoldingCell cell;

        //    tblExpressions.SuspendLayout();
        //    int row = tblExpressions.GetRow(sender as Button);

        //    for (int i = 1; i < tblExpressions.ColumnCount; i++)
        //    {
        //        Control c = tblExpressions.GetControlFromPosition(i, row);
        //        tblExpressions.Controls.Remove(c);
        //        c.Dispose();
        //    }

        //    tblExpressions.RowStyles.RemoveAt(row);
        //    tblExpressions.RowCount--;

        //    tblExpressions.ResumeLayout(false);
        //    tblExpressions.PerformLayout();

        //    //// Delete all controls on selected row 
        //    //for (int columnIndex = 0; columnIndex < tblExpressions.ColumnCount; columnIndex++)
        //    //{
        //    //    var control = tblExpressions.GetControlFromPosition(columnIndex, row);
        //    //    tblExpressions.Controls.Remove(control);
        //    //}

        //    //// Temporarly Store the Positions
        //    //for (Int32 rowholding = row + 1; rowholding <= tblExpressions.RowCount; rowholding++)
        //    //{
        //    //    for (Int32 col = 0; col <= tblExpressions.ColumnCount - 1; col++)
        //    //    {
        //    //        cell = new HoldingCell();
        //    //        cell.cntrl = tblExpressions.GetControlFromPosition(col, rowholding);
        //    //        //setup position for restore = current row -1 
        //    //        cell.tableLayoutPanelCellPosition = new TableLayoutPanelCellPosition(col, rowholding - 1);
        //    //        tempHolding.Add(cell);
        //    //    }
        //    //}

        //    //// adjust control positions 
        //    //foreach (HoldingCell holdingCell in tempHolding)
        //    //{
        //    //    cell = holdingCell;
        //    //    if (cell.cntrl != null)
        //    //    {
        //    //        tblExpressions.SetCellPosition(cell.cntrl, cell.tableLayoutPanelCellPosition);
        //    //    }
        //    //}
        //    //tempHolding = null;

        //    // delete the row      
        //    //tblExpressions.RowCount--;

        //    // Reset Serial number
        //    if (pnlFilterLogic.Visible)
        //    {
        //        // Remove Serial number lable
        //        for (int i = row; i <= tblExpressions.RowCount; i++)
        //        {
        //            var control = tblExpressions.GetControlFromPosition(ExpressionItemType.SrNoLabel, i);
        //            tblExpressions.Controls.Remove(control);
        //        }
        //        // Add Serial number lable
        //        for (int i = row; i <= tblExpressions.RowCount; i++)
        //        {
        //            Label lblSerialNumber = new Label();

        //            lblSerialNumber.Font = new System.Drawing.Font(lblSerialNumber.Font, FontStyle.Bold);
        //            lblSerialNumber.Text = Convert.ToString(i);
        //            lblSerialNumber.Height = 23;
        //            lblSerialNumber.TextAlign = ContentAlignment.MiddleCenter;
        //            tblExpressions.Controls.Add(lblSerialNumber, ExpressionItemType.SrNoLabel, i);
        //        }
        //    }
        //    lnkAddRow.Visible = tblExpressions.RowCount < Constants.EXPRESSION_BUILDER_MAX_ROWS;
        //}

        void btnDeleteRow_Clicked(object sender, EventArgs e)
        {
            try
            {
                tblExpressions.SuspendLayout();
                List<HoldingCell> tempHolding = new List<HoldingCell>();
                HoldingCell cell;

                int row = tblExpressions.GetRow(sender as Button);

                SearchFilter searchFilterToDelete = GetExpressionItem<SearchFilter>(row, ExpressionItemType.SearchFilter);

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
                        Label lblSerialNumber = GetExpressionItem<Label>(i, ExpressionItemType.SrNoLabel);
                        lblSerialNumber.Text = i.ToString();
                        // Update Search Filter
                        SearchFilter searchFilter = GetExpressionItem<SearchFilter>(i, ExpressionItemType.SearchFilter);
                        searchFilter.SequenceNo = i;
                        UpdateCurrentExpression(i, searchFilter);
                    }
                }

                // Call Remove name range only if filter is of type Cell Reference.
                if (searchFilterToDelete.ValueType == ExpressionValueTypes.CellReference)
                    ExcelHelper.RemoveNamedRange(searchFilterToDelete.SearchFilterLabel);

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
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is ApttusObject)
                    newWidth = (int)g.MeasureString(((ApttusObject)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is ApttusField)
                    newWidth = (int)g.MeasureString(((ApttusField)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is EBLookupObject)
                    newWidth = (int)g.MeasureString(((EBLookupObject)currentItem).DisplayMember, font).Width + vertScrollBarWidth;
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth + 16;
            }

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

        private void ExpressionBuilderView_Load(object sender, EventArgs e)
        {
            SetCultureData();
        }


    }

    class EBLookupObject
    {
        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }
    }

    public static class TableLayoutPanelMethodExtensions
    {
        public static Control GetControlFromPosition(this TableLayoutPanel tableLayoutPanel, ExpressionItemType columnIndex, int row)
        {
            int nColumn = (int)columnIndex;
            return tableLayoutPanel.GetControlFromPosition(nColumn, row);
        }

        public static void Add(this TableLayoutControlCollection tableLayoutControlCollection, Control control, ExpressionItemType columnIndex, int row)
        {
            int nColumn = (int)columnIndex;
            tableLayoutControlCollection.Add(control, nColumn, row);
        }

        public static Control GetControlFromPosition(this TableLayoutPanel tableLayoutPanel, DataTableExpressionItemType columnIndex, int row)
        {
            int nColumn = (int)columnIndex;
            return tableLayoutPanel.GetControlFromPosition(nColumn, row);
        }

        public static void Add(this TableLayoutControlCollection tableLayoutControlCollection, Control control, DataTableExpressionItemType columnIndex, int row)
        {
            int nColumn = (int)columnIndex;
            tableLayoutControlCollection.Add(control, nColumn, row);
        }
    }

    // Hold Control cell position 
    public class HoldingCell
    {
        public Control cntrl;
        public TableLayoutPanelCellPosition tableLayoutPanelCellPosition;
    }
}
