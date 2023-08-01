/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using EBHelper = Apttus.XAuthor.Core.ExpressionBuilderHelper;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class UserInputUserControl : UserControl
    {
        private UserInputController userInputController;

        private const int Number_Of_Variables_Per_Row = 2;

        private string ActionID;
        private List<SearchFilter> SearchFilters;
        private bool adjustColWidth = true;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        SavedFilters sfSource;
        Button btnAddFilters;
        String selectedFilter;

        public UserInputUserControl()
        {
            InitializeComponent();
        }

        public UserInputUserControl(UserInputController controller)
        {
            InitializeComponent();
            userInputController = controller;
        }

        public void setController(UserInputController controller)
        {
            userInputController = controller;
        }

        public void PopulateControls(string actionId, List<SearchFilter> searchFilters, bool adjustColumnWidth = false)
        {
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            ActionID = actionId;
            adjustColWidth = adjustColumnWidth;
            SearchFilters = searchFilters;
            try
            {
                int filterIndex = 0;
                ApttusDataSet dataSet = userInputController.InputActionDataSet;

                this.SuspendLayout();

                controlTableLayoutPanel.RowStyles.Clear();

                foreach (SearchFilter searchFilter in searchFilters.Where(sf => sf.ValueType == ExpressionValueTypes.UserInput))
                {
                    if (searchFilter.AppObjectUniqueId != null && !searchFilter.AppObjectUniqueId.Equals(Guid.Empty))
                    {
                        ApttusObject obj = applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId);
                        ApttusField field = obj.Fields.Where(apttusfield => apttusfield.Id.Equals(searchFilter.FieldId)).FirstOrDefault();
                        AddControlForField(field, searchFilter, dataSet, filterIndex, searchFilters);
                    }
                    filterIndex++;
                }

                ResizeControlLayoutPanel();

                if (userInputController.FilterSetSupported)
                    AddSavedFilterUI();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private void ResizeControlLayoutPanel()
        {
            if (adjustColWidth)
            {
                //controlTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
                //controlTableLayoutPanel.ColumnStyles[0].Width = 123;
                //controlTableLayoutPanel.ColumnStyles[1].SizeType = SizeType.Absolute;
                //controlTableLayoutPanel.ColumnStyles[1].Width = 225;
                //controlTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.Absolute;
                //controlTableLayoutPanel.ColumnStyles[2].Width = 123;
                //controlTableLayoutPanel.ColumnStyles[3].SizeType = SizeType.Absolute;
                //controlTableLayoutPanel.ColumnStyles[3].Width = 225;
            }
            else
            {
                //controlTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.AutoSize;
                //controlTableLayoutPanel.ColumnStyles[2].SizeType = SizeType.AutoSize;
            }
        }

        private int GetHeight(ControlType type)
        {
            int controlHeight = 0;
            switch (type)
            {
                case ControlType.CheckBox:
                case ControlType.ComboBox:
                case ControlType.DateTimePicker:
                case ControlType.TextBox:
                case ControlType.LinkLabel:
                    controlHeight = 30;
                    break;
                case ControlType.ListBox:
                    controlHeight = 60;
                    break;
            }
            return controlHeight;
        }

        /// <summary>
        ///  
        /// </summary>
        internal int ControlPanelPrefferedHeight {
            get {
                return controlTableLayoutPanel.PreferredSize.Height;
            }
        }

        internal int ControlPanelPrefferedWidth {
            get {
                if (SearchFilters.Count == 1) //If there is only one filter, why to show 2 extra columns, just narrow them
                {
                    controlTableLayoutPanel.ColumnStyles[(int)ControlColumnIndex.Label2].SizeType = SizeType.Absolute;
                    controlTableLayoutPanel.ColumnStyles[(int)ControlColumnIndex.Label2].Width = 0;
                    controlTableLayoutPanel.ColumnStyles[(int)ControlColumnIndex.Control2].Width = 0;

                    return controlTableLayoutPanel.PreferredSize.Width;
                }
                else
                    return controlTableLayoutPanel.PreferredSize.Width;
            }
        }

        internal void AddControlForField(ApttusField field, SearchFilter filter, ApttusDataSet dataSet, int filterIndex, IEnumerable<SearchFilter> filters)
        {
            int columnControlIndex = filterIndex % Number_Of_Variables_Per_Row == 0 ? (int)ControlColumnIndex.Control1 : (int)ControlColumnIndex.Control2;
            // Do not use Field.ID , use Field Label as that makes more sense to End user, if Label is not configured.
            string labelText = string.IsNullOrEmpty(filter.SearchFilterLabel) ? field.Name : filter.SearchFilterLabel;

            // Allow formula data type for string , numeric,boolean , Curency,for where clause.
            if (field.Datatype == Datatype.Formula)
            {
                if (!string.IsNullOrEmpty(field.FormulaType))
                {
                    field.Datatype = ExpressionBuilderHelper.GetDataTypeFromFormulaType(field.FormulaType);
                }
            }

            switch (field.Datatype)
            {
                case Datatype.Lookup:
                    CreateLookup(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

                case Datatype.Decimal:
                case Datatype.Double:
                case Datatype.NotSupported:
                case Datatype.String:
                case Datatype.Email:
                    if (filter.Operator.ToUpper().Equals("IN") || filter.Operator.ToUpper().Equals("NOT IN"))
                        CreateInValuesLinkLabel(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    else
                        CreateTextBox(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

                case Datatype.Date:
                case Datatype.DateTime:
                    if (filter.Operator.ToUpper().Equals("IN") || filter.Operator.ToUpper().Equals("NOT IN"))
                        CreateInValuesLinkLabel(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    else
                        CreateDateTime(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

                case Datatype.Boolean:
                    CreateCheckbox(filter, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

                case Datatype.Picklist:
                case Datatype.Editable_Picklist:
                    if (filter.Operator.Equals(Constants.EQUALS))
                        CreateComboBox(filter, field, labelText, dataSet, filterIndex, columnControlIndex, filters);
                    else if (filter.Operator.ToUpper().Equals("IN") || filter.Operator.ToUpper().Equals("NOT IN"))
                        CreateListBox(filter, field, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

                case Datatype.Picklist_MultiSelect:
                    CreateListBox(filter, field, labelText, dataSet, filterIndex, columnControlIndex);
                    break;

            }
            if (filterIndex % Number_Of_Variables_Per_Row == 1)
                controlTableLayoutPanel.RowCount++;
        }

        internal void AddRowStyle(ControlType type, int filterIndex)
        {
            if (filterIndex % Number_Of_Variables_Per_Row == 0)
            {
                RowStyle style = new RowStyle();
                style.SizeType = SizeType.Absolute;
                style.Height = GetHeight(type);

                controlTableLayoutPanel.RowStyles.Add(style);
            }
            else
            {
                int height = GetHeight(type);
                RowStyle style = controlTableLayoutPanel.RowStyles[controlTableLayoutPanel.RowCount - 1];
                int maxHeight = style.Height > height ? (int)style.Height : (int)height;
                style.Height = maxHeight;
            }
        }

        internal void CreateLookup(SearchFilter filter, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {
            Panel lookupPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                MaximumSize = new Size(200, 25),
                MinimumSize = new Size(200, 25),
                Tag = filter
            };
            TextBox textbox = new TextBox()
            {
                ReadOnly = true,
                BackColor = Color.White,
                Anchor = AnchorStyles.Left,
                Dock = DockStyle.Fill,
                MaximumSize = new Size(175, 25),
                MinimumSize = new Size(175, 25)
            };
            textbox.KeyDown += textbox_KeyDown;

            Button btn = new Button()
            {
                BackColor = SystemColors.Control,
                Width = 25,
                Height = 22,
                Text = "...",
                Dock = DockStyle.Right,
                Tag = textbox
            };
            btn.Click += btn_Click;

            lookupPanel.Controls.Add(textbox);
            lookupPanel.Controls.Add(btn);

            CreateLabel(fieldName, filterIndex);

            controlTableLayoutPanel.Controls.Add(lookupPanel, columnControlIndex, controlTableLayoutPanel.RowCount - 1);
            AddRowStyle(ControlType.TextBox, filterIndex);

            //PopulateData(ControlType.LookUpControl, dataSet, variable, textbox);
        }

        void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                TextBox textbox = sender as TextBox;
                //If Delete key is pressed, reset the textbox control.
                textbox.Text = string.Empty;
                textbox.Tag = string.Empty;
                e.Handled = true;
            }
        }

        private void CreateInValuesLinkLabel(SearchFilter filter, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {
            LinkLabel llInValues = new LinkLabel();
            llInValues.AutoSize = true;
            llInValues.TextAlign = ContentAlignment.BottomCenter;
            llInValues.LinkClicked += new LinkLabelLinkClickedEventHandler(llInValues_LinkClicked);
            llInValues.Tag = new LinkLabelData { searchFilter = filter, value = string.Empty };

            CreateLabel(fieldName, filterIndex);
            controlTableLayoutPanel.Controls.Add(llInValues, columnControlIndex, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.LinkLabel, filterIndex);

            PopulateInValuesLinkLabel(llInValues, string.Empty);
        }

        private void llInValues_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel llInValues = (sender as LinkLabel);
            LinkLabelData llData = (llInValues.Tag as LinkLabelData);

            // Launch Object and Field picker
            MultiLineCaptureView view = new MultiLineCaptureView();
            MultiLineCaptureController controller = new MultiLineCaptureController(view, EBHelper.GetSearchFilterObject(llData.searchFilter), EBHelper.GetSearchFilterField(llData.searchFilter), llData.value);

            // Update the Object and Field text and Search Filter, if the user hit save on the dialog. Search Filter will already have new values.
            if (controller.Browse())
            {
                llData.value = controller.MultiLineValue;
                PopulateInValuesLinkLabel(llInValues, llData.value);
                llInValues.Tag = llData;
            }
        }

        private void PopulateInValuesLinkLabel(LinkLabel llInValues, string value)
        {
            bool isValueEmpty = string.IsNullOrEmpty(value);
            llInValues.Text = isValueEmpty ? resourceManager.GetResource("COMMON_AddValues_Text") : resourceManager.GetResource("COMMON_UpdateValues_Text");
            llInValues.LinkColor = isValueEmpty ? Color.Orange : Color.DarkOliveGreen;
        }

        void btn_Click(object sender, EventArgs e)
        {
            SearchFilter filter = ((sender as Button).Parent as Panel).Tag as SearchFilter;
            using (UserInputLookupForm lookupForm = new UserInputLookupForm(filter))
            {
                if (lookupForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TextBox textbox = (sender as Button).Tag as TextBox;
                    textbox.Tag = lookupForm.LookupId;
                    textbox.Text = lookupForm.LookupName;
                }
            }
        }

        internal void CreateTextBox(SearchFilter filter, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {
            TextBox textbox = new TextBox();
            textbox.Anchor = AnchorStyles.Left;
            textbox.Dock = DockStyle.Fill;
            textbox.Tag = filter;
            textbox.MaximumSize = new System.Drawing.Size(200, 25);
            textbox.MinimumSize = new System.Drawing.Size(200, 25);
            textbox.Name = "txt_" + fieldName;
            CreateLabel(fieldName, filterIndex);

            controlTableLayoutPanel.Controls.Add(textbox, columnControlIndex, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.TextBox, filterIndex);

            //PopulateData(ControlType.TextBox, dataSet, variable, textbox);
        }

        internal void CreateLabel(string varName, int filterIndex)
        {
            Label lblVariable = new Label();
            lblVariable.Anchor = AnchorStyles.None | AnchorStyles.Right;
            lblVariable.AutoSize = true;
            lblVariable.TextAlign = ContentAlignment.MiddleCenter;
            lblVariable.Margin = new Padding(0);
            lblVariable.Text = varName + " :";
            lblVariable.Name = "lbl_" + varName;
            int columnIndex = filterIndex % Number_Of_Variables_Per_Row == 0 ? (int)ControlColumnIndex.Label1 : (int)ControlColumnIndex.Label2;

            controlTableLayoutPanel.Controls.Add(lblVariable, columnIndex, controlTableLayoutPanel.RowCount - 1);
        }

        internal void CreateComboBox(SearchFilter filter, ApttusField field, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex, IEnumerable<SearchFilter> filters)
        {
            ComboBox cbo = new ComboBox();
            cbo.DropDownStyle = field.Datatype == Datatype.Picklist ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
            cbo.Dock = DockStyle.Fill;
            cbo.Tag = filter;
            cbo.MaximumSize = new System.Drawing.Size(200, 25);
            cbo.MinimumSize = new System.Drawing.Size(200, 25);
            cbo.DropDown += new EventHandler(AdjustWidth_DropDown);

            CreateLabel(fieldName, filterIndex);
            controlTableLayoutPanel.Controls.Add(cbo, columnControlIndex, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.ComboBox, filterIndex);

            // If Depedent Picklist exists as User Input, attach SelectedIndexChange event
            bool bControllingPicklist = false;
            bool bDependentPicklist = false;

            var sameObjectUserInputFilters = (from f in filters
                                              where f.AppObjectUniqueId == filter.AppObjectUniqueId && f.FieldId != filter.FieldId &&
                                              f.ValueType == ExpressionValueTypes.UserInput
                                              select f);

            foreach (var sameObjectUserInputFilter in sameObjectUserInputFilters)
            {
                ApttusField apttusField = EBHelper.GetSearchFilterField(sameObjectUserInputFilter);
                if (apttusField.Datatype == Datatype.Picklist)
                {
                    if (apttusField.ControllingPicklistFieldId == field.Id)
                        bControllingPicklist = true;

                    if (field.ControllingPicklistFieldId == apttusField.Id)
                        bDependentPicklist = true;
                }
            }

            if (bControllingPicklist)
                cbo.SelectedIndexChanged += new System.EventHandler(this.cbo_SelectedIndexChanged);

            // First value is blank
            List<string> comboboxValues = new List<string>() { Constants.NOSPACE };
            if (!bDependentPicklist && field.PicklistValues != null)
                comboboxValues.AddRange(field.PicklistValues);
            cbo.DataSource = comboboxValues;
            cbo.SelectedIndex = -1;

            //PopulateData(ControlType.ComboBox, dataSet, variable, cbo);
        }

        private void cbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox control = sender as ComboBox;
            SearchFilter currentFilter = ((sender as ComboBox).Tag as SearchFilter);
            var dependentUserInputFilters = (from f in userInputController.SearchFilterGroups[0].Filters
                                             where f.AppObjectUniqueId == currentFilter.AppObjectUniqueId && f.FieldId != currentFilter.FieldId &&
                                             f.ValueType == ExpressionValueTypes.UserInput
                                             select f);

            foreach (var dependentUserInputFilter in dependentUserInputFilters)
            {
                ApttusField dependentField = EBHelper.GetSearchFilterField(dependentUserInputFilter);
                if (dependentField.Datatype == Datatype.Picklist && dependentField.ControllingPicklistFieldId == currentFilter.FieldId)
                {
                    foreach (ComboBox dependentComboBox in controlTableLayoutPanel.Controls.OfType<ComboBox>())
                    {
                        SearchFilter sf = (dependentComboBox.Tag as SearchFilter);
                        if (sf.AppObjectUniqueId == currentFilter.AppObjectUniqueId && sf.FieldId == dependentField.Id)
                        {
                            // First value is blank
                            List<string> comboboxValues = new List<string>() { Constants.NOSPACE };

                            if (control.SelectedValue != null && !string.IsNullOrEmpty(control.SelectedValue.ToString()))
                            {

                                var values = (from dpi in dependentField.DependentPicklistValues
                                              where dpi.ControllingValue == control.SelectedValue.ToString()
                                              select dpi.PicklistValues).FirstOrDefault();
                                if (values != null)
                                    comboboxValues.AddRange(values);
                            }

                            dependentComboBox.DataSource = comboboxValues;
                            dependentComboBox.SelectedIndex = -1;
                            break;
                        }
                    }
                    break;
                }
            }

        }

        internal void CreateListBox(SearchFilter filter, ApttusField field, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {

            Panel lookupPanel = new Panel();
            lookupPanel.BorderStyle = BorderStyle.FixedSingle;
            lookupPanel.Dock = DockStyle.Fill;
            lookupPanel.Tag = filter;

            TextBox textbox = new TextBox();
            textbox.KeyDown += textbox_KeyDown;
            textbox.ReadOnly = true;
            textbox.BackColor = Color.White;
            textbox.Anchor = AnchorStyles.Left;
            textbox.Dock = DockStyle.Fill;

            Button btn = new Button();
            btn.Click += delegate (object sender, EventArgs e) { OpenMultiSelectPickListView(sender, e, field, filter); };
            btn.BackColor = SystemColors.Control;
            btn.Height = 22;
            btn.Width = 25;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Text = "...";
            btn.Dock = DockStyle.Right;
            btn.Tag = textbox;

            lookupPanel.Controls.Add(textbox);
            lookupPanel.Controls.Add(btn);

            CreateLabel(fieldName, filterIndex);

            controlTableLayoutPanel.Controls.Add(lookupPanel, columnControlIndex, controlTableLayoutPanel.RowCount - 1);
            AddRowStyle(ControlType.TextBox, filterIndex);

            //PopulateData(ControlType.ListBox, dataSet, variable, listbox);
        }

        void OpenMultiSelectPickListView(object sender, EventArgs e, ApttusField field, SearchFilter filter)
        {
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            //get sourcevalues & Selected Values from controls
            TextBox txtselectedvalues = (TextBox)(((Button)sender).Tag);
            List<string> selectedvalues = (!string.IsNullOrEmpty(txtselectedvalues.Text)) ? txtselectedvalues.Text.Split(',').ToList() : new List<string>();
            List<string> Sourcevalues = field.PicklistValues.Except(selectedvalues).ToList();

            // Create Screen Title
            string objectName = string.Empty, fieldName = string.Empty;
            ApttusObject objectdetails = applicationDefinitionManager.GetAppObject(filter.AppObjectUniqueId);

            if (objectdetails != null)
                objectName = objectdetails.Name;

            fieldName = field.Name;

            string screentitle = string.Format("{0}.{1}", objectName, fieldName);


            // Open Form
            using (MultiSelectPickListForm view = new MultiSelectPickListForm(Sourcevalues, selectedvalues, screentitle))
            {
                if (view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtselectedvalues.Tag = String.Join(Environment.NewLine, view.SelectedList);
                    txtselectedvalues.Text = String.Join(",", view.SelectedList);
                }
            }
        }

        internal void CreateCheckbox(SearchFilter filter, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {
            CheckBox checkbox = new CheckBox();
            //checkbox.Text = FieldName;
            checkbox.Dock = DockStyle.Fill;
            checkbox.Tag = filter;
            checkbox.Anchor = AnchorStyles.Top;

            CreateLabel(fieldName, filterIndex);
            controlTableLayoutPanel.Controls.Add(checkbox, columnControlIndex, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.CheckBox, filterIndex);

            //PopulateData(ControlType.CheckBox, dataSet, variable, checkbox);
        }

        internal void CreateDateTime(SearchFilter filter, string fieldName, ApttusDataSet dataSet, int filterIndex, int columnControlIndex)
        {
            DateTimePicker dateTimePicker = new DateTimePicker();
            //dateTimePicker.Dock = DockStyle.Fill;
            dateTimePicker.Tag = filter;
            CreateLabel(fieldName, filterIndex);
            controlTableLayoutPanel.Controls.Add(dateTimePicker, columnControlIndex, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.DateTimePicker, filterIndex);

            bool includeTime = (EBHelper.GetSearchFilterField(filter).Datatype == Datatype.DateTime);
            ClearDateTimePicker(dateTimePicker, includeTime);

            //PopulateData(ControlType.DateTimePicker, dataSet, variable, dateTimePicker);
        }

        public void UpdateValuesInDataSet()
        {
            ApttusDataSet dataSet = userInputController.GetInputVariablesDataSet();

            for (int i = 0; i < controlTableLayoutPanel.RowCount; ++i)
            {
                ExtractDataFromControls(dataSet, (int)ControlColumnIndex.Control1, i);
                ExtractDataFromControls(dataSet, (int)ControlColumnIndex.Control2, i);
            }
        }

        private void ExtractDataFromControls(ApttusDataSet dataSet, int columnIndex, int rowIndex)
        {
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            Control ctrl = controlTableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
            if (ctrl == null)
                return;

            SearchFilter filter = null;
            string data = string.Empty;

            if (ctrl.GetType() == typeof(TextBox))
            {
                TextBox txt = ctrl as TextBox;
                filter = txt.Tag as SearchFilter;
                data = txt.Text;
            }
            else if (ctrl.GetType() == typeof(ComboBox))
            {
                ComboBox cbo = ctrl as ComboBox;
                filter = cbo.Tag as SearchFilter;
                if (cbo.SelectedIndex > 0 && cbo.DropDownStyle == ComboBoxStyle.DropDownList) // 0th index will be blank value
                    data = cbo.Items[cbo.SelectedIndex] as string;
                else if (cbo.DropDownStyle == ComboBoxStyle.DropDown)
                    data = cbo.Text as string;
            }
            else if (ctrl.GetType() == typeof(Panel))  //Lookup
            {
                Panel lookPanel = ctrl as Panel;
                TextBox txt = lookPanel.Controls[0] as TextBox;
                //For Lookup values, we need Id at runtime for querying the data and we need lookup name to restore the value in the form.
                //Hence we store both the values in the dataset which is ';' separated.
                //data = string.Join(";", new string[] { txt.Tag as string, txt.Text }); //lookup id + lookup name
                // With New User Input implementation combination of Id and Name is not required, hence store ID only.
                filter = lookPanel.Tag as SearchFilter;
                //data = string.Join(";", new string[] { txt.Tag as string, txt.Text }); //lookup id + lookup name

                ApttusField af = applicationDefinitionManager.GetField(filter.AppObjectUniqueId, filter.FieldId);

                if (af.Datatype == Datatype.Lookup)
                    data = string.Join(";", new string[] { txt.Tag as string, txt.Text });
                else
                    data = txt.Tag as string;

            }
            else if (ctrl.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker picker = ctrl as DateTimePicker;
                filter = picker.Tag as SearchFilter;
                if (picker.CustomFormat != " ")
                    data = picker.Value.ToString();
            }
            else if (ctrl.GetType() == typeof(ListBox))
            {
                ListBox listbox = ctrl as ListBox;
                filter = listbox.Tag as SearchFilter;
                StringBuilder sb = new StringBuilder();
                if (listbox.SelectedItems.Count > 0)
                    data = string.Join(Environment.NewLine, listbox.SelectedItems.Cast<string>().ToList());
            }
            else if (ctrl.GetType() == typeof(CheckBox))
            {
                CheckBox check = ctrl as CheckBox;
                filter = check.Tag as SearchFilter;
                data = check.Checked ? "true" : "false";
            }
            else if (ctrl.GetType() == typeof(LinkLabel))
            {
                LinkLabel llInValues = ctrl as LinkLabel;
                filter = (llInValues.Tag as LinkLabelData).searchFilter;
                data = (llInValues.Tag as LinkLabelData).value;
                //data = EBHelper.EscapeAndGetFilterValue((llInValues.Tag as LinkLabelData).value, filter.Operator, EBHelper.GetSearchFilterField(filter).Datatype, ValueSeparator.MultiLine);
            }
            userInputController.AddDataInDataSet(dataSet, filter, data);
        }

        private static void ClearDateTimePicker(DateTimePicker dateTimePicker, bool includeTime)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = " ";
            string customFormat = includeTime ? "MM/dd/yyyy h:mm:ss tt" : "MM/dd/yyyy";
            dateTimePicker.ValueChanged += (s, ev) => { dateTimePicker.CustomFormat = (dateTimePicker.Checked && dateTimePicker.Value != dateTimePicker.MinDate) ? customFormat : " "; };
        }

        private void PopulateData(ControlType type, ApttusDataSet dataSet, SearchFilter variable, Control ctrl)
        {
            if (dataSet == null)
                return;

            //Find the Key
            string key = userInputController.CreateKey(variable.SequenceNo, variable.FieldId);

            DataRow row = dataSet.DataTable.Rows.Find(key);
            if (row == null)
                return;

            //Check if the value exists
            string value = row[Constants.VALUE_COLUMN].ToString();
            //if (String.IsNullOrEmpty(value))
            //    return;

            //If the value exists, then populate the control.
            switch (type)
            {
                case ControlType.TextBox:
                    (ctrl as TextBox).Text = value;
                    break;
                case ControlType.CheckBox:
                    (ctrl as CheckBox).Checked = Boolean.Parse(value);
                    break;
                case ControlType.ComboBox:
                    (ctrl as ComboBox).SelectedItem = value;
                    break;
                case ControlType.ListBox:
                    {
                        ListBox listbox = ctrl as ListBox;
                        string[] values = value.Split(new char[] { ';' });
                        foreach (string item in values)
                        {
                            int index = listbox.Items.IndexOf(item);
                            if (index != -1)
                                listbox.SetSelected(index, true);
                        }
                    }
                    break;
                case ControlType.LookUpControl:
                    {
                        string Dvalue = string.IsNullOrEmpty(row[Constants.Display_COLUMN].ToString()) ? row[Constants.VALUE_COLUMN].ToString() : row[Constants.Display_COLUMN].ToString();
                        Dvalue = Dvalue.Replace("\n", ",");
                        value = value.Replace("\n", ",");
                        Panel lookPanel = ctrl as Panel;
                        TextBox txt = (ctrl.Controls[0] as TextBox);
                        txt.Text = Dvalue;
                        txt.Tag = value;

                    }
                    break;
                case ControlType.DateTimePicker:
                    {
                        DateTimePicker dateTimePicker = ctrl as DateTimePicker;
                        if (!string.IsNullOrEmpty(value))
                        {
                            dateTimePicker.Format = DateTimePickerFormat.Custom;
                            dateTimePicker.CustomFormat = "MM/dd/yyyy";
                            dateTimePicker.Value = DateTime.Parse(value);
                        }
                        else
                        {
                            dateTimePicker.Format = DateTimePickerFormat.Custom;
                            dateTimePicker.CustomFormat = " ";
                        }
                    }
                    break;
                case ControlType.LinkLabel:
                    {
                        LinkLabel llInValues = ctrl as LinkLabel;
                        value = value.Replace("\n", "\r\n");
                        llInValues.Tag = new LinkLabelData() { searchFilter = variable, value = value };
                        PopulateInValuesLinkLabel(llInValues, value);
                        //data = EBHelper.EscapeAndGetFilterValue((llInValues.Tag as LinkLabelData).value, filter.Operator, EBHelper.GetSearchFilterField(filter).Datatype, ValueSeparator.MultiLine);
                    }
                    break;
            }
        }

        private void ClearControlAt(int columnIndex, int rowIndex)
        {
            Control ctrl = controlTableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
            if (ctrl == null)
                return;
            if (ctrl.GetType() == typeof(TextBox))
                (ctrl as TextBox).Text = string.Empty;
            else if (ctrl.GetType() == typeof(CheckBox))
                (ctrl as CheckBox).Checked = false;
            else if (ctrl.GetType() == typeof(ComboBox))
            {
                ComboBox cbo = (ctrl as ComboBox);
                cbo.SelectedIndex = -1;
                cbo.SelectedItem = null;
            }
            else if (ctrl.GetType() == typeof(Panel)) //Lookup
            {
                TextBox txt = (ctrl as Panel).Controls[0] as TextBox;
                txt.Text = string.Empty;
                txt.Tag = null;
            }
            else if (ctrl.GetType() == typeof(ListBox))
            {
                ListBox listbox = ctrl as ListBox;
                listbox.SelectedIndex = -1;
                for (int item = 0; item < listbox.Items.Count; ++item)
                    listbox.SetSelected(item, false);
            }
            else if (ctrl.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker dateTimePicker = ctrl as DateTimePicker;
                // Use Date & DateTime differentiator while clearing controls
                bool includeTime = (EBHelper.GetSearchFilterField((SearchFilter)ctrl.Tag).Datatype == Datatype.DateTime);
                ClearDateTimePicker(dateTimePicker, includeTime);
            }
            else if (ctrl.GetType() == typeof(LinkLabel))
            {
                LinkLabel llInValues = ctrl as LinkLabel;
                (llInValues.Tag as LinkLabelData).value = string.Empty;
                PopulateInValuesLinkLabel(llInValues, (llInValues.Tag as LinkLabelData).value);
            }
        }

        public void ClearControls()
        {
            SuspendLayout();
            try
            {
                //reset All controls from the dataset
                for (int i = 0; i < controlTableLayoutPanel.RowCount; ++i)
                {
                    ClearControlAt((int)ControlColumnIndex.Control1, i);
                    ClearControlAt((int)ControlColumnIndex.Control2, i);
                }
                if (userInputController.FilterSetSupported)
                    btnAddFilters.Text = string.Empty;

            }
            finally
            {
                ResumeLayout();
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
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth + 16;
            }

            senderComboBox.DropDownWidth = width;
        }

        #region Saved Filters

        /// <summary>
        /// Used to add Saved Filter UI panel, if user has configured provision for same in designer.
        /// </summary>
        void AddSavedFilterUI()
        {

            Label lblVariable = new Label();
            lblVariable.Anchor = AnchorStyles.None | AnchorStyles.Right;
            lblVariable.AutoSize = true;
            lblVariable.TextAlign = ContentAlignment.MiddleCenter;
            lblVariable.Margin = new Padding(0);
            lblVariable.Text = "Favorite Filter" + " :";
            lblVariable.Name = "lbl_" + "SelectTemplate";
            lblVariable.TextAlign = ContentAlignment.MiddleRight;
            tblFilterTemplatePanel.Controls.Add(lblVariable, 0, 0);


            btnAddFilters = new Button();
            btnAddFilters.Dock = DockStyle.Fill;
            btnAddFilters.MaximumSize = new System.Drawing.Size(200, 25);
            btnAddFilters.MinimumSize = new System.Drawing.Size(200, 25);
            btnAddFilters.FlatStyle = FlatStyle.Flat;
            btnAddFilters.Name = "btnAddFilters";
            btnAddFilters.Click += BtnAddFilters_Click;
            tblFilterTemplatePanel.Controls.Add(btnAddFilters, 1, 0);
            btnAddFilters.Image = Properties.Resources.DownArrowActions;
            btnAddFilters.ImageAlign = ContentAlignment.MiddleRight;
            btnAddFilters.BackColor = Color.White;
            btnAddFilters.TextAlign = ContentAlignment.MiddleLeft;
            btnAddFilters.FlatAppearance.BorderColor = Color.Silver;

            RowStyle style = new RowStyle();
            style.SizeType = SizeType.Absolute;
            style.Height = GetHeight(ControlType.ComboBox);

            tblFilterTemplatePanel.Margin = new Padding(0);
            controlTableLayoutPanel.Margin = new Padding(0);

            tblFilterTemplatePanel.Padding = new Padding(0);
            controlTableLayoutPanel.Padding = new Padding(0);

            tblFilterTemplatePanel.RowStyles.Add(style);

            SetControlAlignmentInFilterGroup();

            ShowAddFilterContextMenu();
        }

        /// <summary>
        /// This method will align search label and lables of filter group - if length of label search text is more than length of largest filter group label text then 
        /// we will add space in all odd placed filter labels to make it aligned with search label and vice versa.
        /// </summary>
        public void SetControlAlignmentInFilterGroup()
        {
            Label lblSearch = (Label)this.Controls.Find("lbl_SelectTemplate", true).FirstOrDefault();

            if (lblSearch == null)
                return;

            try
            {
                int maxlen = userInputController.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0).Max(x => x.SearchFilterLabel.Length);
                string searchfilterLabel = userInputController.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0 && item.SearchFilterLabel.Length == maxlen).FirstOrDefault().SearchFilterLabel;
                Label lbl = (Label)this.Controls.Find("lbl_" + searchfilterLabel, true).FirstOrDefault();
                if (lbl != null && lbl.Right > lblSearch.Right)
                {
                    while (lbl.Right > lblSearch.Right)
                    {
                        lblSearch.Text = " " + lblSearch.Text;
                    }
                }
                else
                {
                    foreach (var searchfilter in userInputController.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0))
                    {
                        lbl = (Label)this.Controls.Find("lbl_" + searchfilter.SearchFilterLabel, true).FirstOrDefault();
                        if (lbl != null)
                        {
                            while (lbl.Right < lblSearch.Right)
                            {
                                lbl.Text = " " + lbl.Text;
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Used to save filters.
        /// </summary>
        void SaveFilters()
        {

            string filtersetName = string.Empty;
            InputDialog inputFilterSetDlg = new InputDialog();
            if (inputFilterSetDlg.ShowDialog() == DialogResult.OK)
            {
                filtersetName = inputFilterSetDlg.FilterSetName;

                UpdateValuesInDataSet();

                SavedFilterHelper.SaveFilters(sfSource, filtersetName, ActionID, userInputController.InputActionDataSet);

                btnAddFilters.Text = filtersetName;
            }
        }

        void UpdateFilters()
        {
            string filtersetName = string.Empty;

            filtersetName = btnAddFilters.Text;

            UpdateValuesInDataSet();

            SavedFilterHelper.SaveFilters(sfSource, filtersetName, ActionID, userInputController.InputActionDataSet);
        }

        /// <summary>
        /// Used to populate filters from selected filter set.
        /// </summary>
        /// <param name="selectedFilterset"></param>
        void PopulateData(string selectedFilterset)
        {
            ApttusDataSet ds = sfSource.UserDefinedFilters.Where(x => x.Name == selectedFilterset).FirstOrDefault().Data;
            for (int i = 0; i < controlTableLayoutPanel.RowCount; ++i)
            {
                PopulateData(ds, controlTableLayoutPanel.GetControlFromPosition((int)ControlColumnIndex.Control1, i));
                PopulateData(ds, controlTableLayoutPanel.GetControlFromPosition((int)ControlColumnIndex.Control2, i));
            }
        }

        void ExecuteSearch()
        {
            if (userInputController.AutoExecutionForSearchFilters)
            {
                mainTableLayoutPanel.Focus();
                SendKeys.Send("{ENTER}");
            }
        }
        /// <summary>
        /// used to populate data from dataset in specific contol.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="ctrl"></param>
        void PopulateData(ApttusDataSet ds, Control ctrl)
        {
            if (ctrl == null)
                return;
            ControlType controlType = GetControlType(ctrl);
            PopulateData(controlType, ds, GetSeachFilter(ctrl, controlType), ctrl);
        }

        SearchFilter GetSeachFilter(Control ctrl, ControlType controlType)
        {
            if (controlType == ControlType.LinkLabel)
                return (ctrl.Tag as LinkLabelData).searchFilter;
            else
                return ctrl.Tag as SearchFilter;
        }

        /// <summary>
        /// Used to get control type.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        ControlType GetControlType(Control ctrl)
        {
            if (ctrl.GetType() == typeof(TextBox))
            {
                return ControlType.TextBox;
            }
            else if (ctrl.GetType() == typeof(ComboBox))
            {
                return ControlType.ComboBox;
            }
            else if (ctrl.GetType() == typeof(Panel))  //Lookup
            {
                return ControlType.LookUpControl;

            }
            else if (ctrl.GetType() == typeof(DateTimePicker))
            {
                return ControlType.DateTimePicker;

            }
            else if (ctrl.GetType() == typeof(ListBox))
            {
                return ControlType.ListBox;

            }
            else if (ctrl.GetType() == typeof(CheckBox))
            {
                return ControlType.CheckBox;
            }
            else if (ctrl.GetType() == typeof(LinkLabel))
            {
                return ControlType.LinkLabel;
            }

            return ControlType.TextBox;
        }

        private void BtnAddFilters_Click(object sender, EventArgs e)
        {
            ShowAddFilterContextMenu();
        }

        private void ShowAddFilterContextMenu()
        {
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Opening += strip_Opening;
            strip.Closing += strip_Closing;
            strip.ItemClicked += strip_ItemClicked;

            sfSource = SavedFilterHelper.GetSavedFilters();
            ToolStripItem item;

            item = strip.Items.Add(" ");
            item.Tag = string.Empty;

            foreach (SavedFilter sf in sfSource.UserDefinedFilters.Where(x => x.ActionID == ActionID))
            {
                item = strip.Items.Add(sf.Name);
                item.Tag = sf.Name;
            }


            strip.Items.Add(new ToolStripSeparator());

            item = strip.Items.Add("Add...");
            item.Tag = "ADD";

            item = strip.Items.Add("Update");
            item.Tag = "UPDATE";
            ManageApplicableMenusForFilters(item);

            item = strip.Items.Add("Remove");
            item.Tag = "REMOVE";
            ManageApplicableMenusForFilters(item);

            strip.Show(btnAddFilters, new Point(0, btnAddFilters.Height));
        }

        void ManageApplicableMenusForFilters(ToolStripItem item)
        {
            if (string.IsNullOrEmpty(btnAddFilters.Text))
            {
                item.Enabled = false;
            }
        }

        private void strip_Opening(object sender, CancelEventArgs e)
        {
            btnAddFilters.Image = Properties.Resources.UpArrowActions;
        }

        private void strip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            btnAddFilters.Image = Properties.Resources.DownArrowActions;
        }

        private void strip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            selectedFilter = e.ClickedItem.Tag.ToString();

            switch (selectedFilter)
            {
                case "":
                    {
                        ClearControls();
                        break;
                    }
                case "ADD":
                    {
                        SaveFilters();
                        break;
                    }
                case "UPDATE":
                    {
                        UpdateFilters();
                        break;
                    }
                case "REMOVE":
                    {
                        SavedFilterHelper.RemoveFilter(sfSource, btnAddFilters.Text, ActionID);
                        ClearControls();
                        break;
                    }
                default:
                    btnAddFilters.Text = selectedFilter;
                    PopulateData(selectedFilter);
                    break;
            }

            ExecuteSearch();
        }
        #endregion
    }

    enum ControlColumnIndex
    {
        Label1,
        Control1,
        Label2,
        Control2
    }

    enum ControlType
    {
        TextBox,
        ComboBox,
        ListBox,
        DateTimePicker,
        CheckBox,
        LookUpControl,
        LinkLabel
    }

    internal class LinkLabelData
    {
        public SearchFilter searchFilter { get; set; }
        public string value { get; set; }
    }
}
