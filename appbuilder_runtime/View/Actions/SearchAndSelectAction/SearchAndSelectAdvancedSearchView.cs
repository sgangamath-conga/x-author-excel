/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class SearchAndSelectAdvancedSearchView : ShadowWindowForm
    {
        SearchAndSelectActionController controller;
        public string advancedSearchWhereClause = string.Empty;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public SearchAndSelectAdvancedSearchView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            //this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }

        public void SetController(SearchAndSelectActionController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Set Culture data
        /// </summary>
        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnCancel_Text");
            btnDone.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_lblSearch_Text");
        }

        public void PopulateControls()
        {
            try
            {
                this.SuspendLayout();
                controlTableLayoutPanel.RowStyles.Clear();

                foreach (SearchField searchField in controller.model.SearchFields)
                {
                    ApttusField aptField = controller.appObject.Fields.Where(f => f.Id.Equals(searchField.Id)).FirstOrDefault();
                    AddControlForField(aptField);
                }
                ResizeControlLayoutPanel();

                ResizeForm();

                Control ctl = controlTableLayoutPanel.GetControlFromPosition(1, 0);
                this.ActiveControl = ctl;
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
            controlTableLayoutPanel.RowCount = controlTableLayoutPanel.RowCount - 1;
            controlTableLayoutPanel.ColumnStyles[0].SizeType = SizeType.AutoSize;
        }

        internal void ResizeForm()
        {
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            int titleHeight = screenRectangle.Top - this.Top;

            Height = controlTableLayoutPanel.PreferredSize.Height + titleHeight + (int)mainTableLayoutPanel.RowStyles[1].Height + SystemInformation.FrameBorderSize.Height;
        }

        internal void AddControlForField(ApttusField aptfield)
        {
            string persistedValue = controller.GetPersistedSearchValue(aptfield);
            switch (aptfield.Datatype)
            {
                case Datatype.Lookup:
                    CreateTextBox(aptfield, persistedValue);
                    break;
                case Datatype.Decimal:
                case Datatype.Double:
                case Datatype.NotSupported:
                case Datatype.String:
                    CreateTextBox(aptfield, persistedValue);
                    break;
                case Datatype.Date:
                case Datatype.DateTime:
                    CreateDateTime(aptfield, persistedValue);
                    break;
                case Datatype.Boolean:
                    CreateCheckbox(aptfield, persistedValue);
                    break;
                case Datatype.Picklist:
                    CreateComboBox(aptfield, persistedValue);
                    break;
                case Datatype.Picklist_MultiSelect:
                    CreateListBox(aptfield, persistedValue);
                    break;
            }
            controlTableLayoutPanel.RowCount++;
        }

        internal void AddRowStyle(ControlType type)
        {
            RowStyle style = new RowStyle();
            style.SizeType = SizeType.Absolute;
            style.Height = GetHeight(type);
            controlTableLayoutPanel.RowStyles.Add(style);
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
                    controlHeight = 80;
                    break;
            }
            return controlHeight;
        }

        internal void CreateLabel(string varName)
        {
            Label lblVariable = new Label();
            lblVariable.Anchor = AnchorStyles.Right;
            lblVariable.AutoSize = true;
            lblVariable.TextAlign = ContentAlignment.MiddleCenter;
            lblVariable.Text = varName + " :";

            controlTableLayoutPanel.Controls.Add(lblVariable, (int)ControlColumnIndex.Label1, controlTableLayoutPanel.RowCount - 1);
        }

        internal void CreateTextBox(ApttusField field, string persistedValue)
        {
            TextBox textbox = new TextBox();
            textbox.Anchor = AnchorStyles.Left;
            textbox.Dock = DockStyle.Fill;
            textbox.Tag = field;

            if (!string.IsNullOrEmpty(persistedValue))
                textbox.Text = persistedValue;

            CreateLabel(field.Name);
            controlTableLayoutPanel.Controls.Add(textbox, (int)ControlColumnIndex.Control1, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.TextBox);
        }

        internal void CreateDateTime(ApttusField field, string persistedValue)
        {
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Dock = DockStyle.Fill;
            dateTimePicker.Tag = field;

            CreateLabel(field.Name);
            controlTableLayoutPanel.Controls.Add(dateTimePicker, (int)ControlColumnIndex.Control1, controlTableLayoutPanel.RowCount - 1);

            if (!string.IsNullOrEmpty(persistedValue))
            {
                dateTimePicker.Format = DateTimePickerFormat.Custom;
                dateTimePicker.CustomFormat = "MM/dd/yyyy";
                dateTimePicker.Value = DateTime.Parse(persistedValue);
            }
            else // Set picker value to blank
            {
                dateTimePicker.Format = DateTimePickerFormat.Custom;
                dateTimePicker.CustomFormat = " ";
                dateTimePicker.ValueChanged += (s, e) => { dateTimePicker.CustomFormat = (dateTimePicker.Checked && dateTimePicker.Value != dateTimePicker.MinDate) ? "MM/dd/yyyy" : " "; };
            }
            AddRowStyle(ControlType.DateTimePicker);
        }

        internal void CreateComboBox(ApttusField field, string persistedValue)
        {
            ComboBox cbo = new ComboBox();
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.Dock = DockStyle.Fill;
            cbo.Tag = field;

            CreateLabel(field.Name);
            controlTableLayoutPanel.Controls.Add(cbo, (int)ControlColumnIndex.Control1, controlTableLayoutPanel.RowCount - 1);

            // Bind dropdown
            if (field.PicklistValues != null && field.PicklistValues.Count > 0)
            {
                cbo.DataSource = new BindingSource(field.PicklistValues, null);

                if (!string.IsNullOrEmpty(persistedValue))
                    cbo.SelectedItem = persistedValue;
                else    // Remove or unselect default first selected item
                    cbo.SelectedIndex = -1;
            }
            AddRowStyle(ControlType.ComboBox);
        }

        internal void CreateCheckbox(ApttusField field, string persistedValue)
        {
            CheckBox checkbox = new CheckBox();
            checkbox.Text = string.Empty;
            checkbox.Dock = DockStyle.Fill;
            checkbox.Tag = field;

            if (!string.IsNullOrEmpty(persistedValue))
                checkbox.Checked = Boolean.Parse(persistedValue);

            CreateLabel(field.Name);
            controlTableLayoutPanel.Controls.Add(checkbox, (int)ControlColumnIndex.Control1, controlTableLayoutPanel.RowCount - 1);

            AddRowStyle(ControlType.CheckBox);
        }

        internal void CreateListBox(ApttusField field, string persistedValue)
        {
            ListBox listbox = new ListBox();
            listbox.Dock = DockStyle.Fill;
            listbox.SelectionMode = SelectionMode.MultiSimple;
            listbox.Tag = field;

            CreateLabel(field.Name);
            controlTableLayoutPanel.Controls.Add(listbox, (int)ControlColumnIndex.Control1, controlTableLayoutPanel.RowCount - 1);

            // Bind listbox
            if (field.PicklistValues != null && field.PicklistValues.Count > 0)
            {
                listbox.DataSource = new BindingSource(field.PicklistValues, null);

                // Remove or unselect default first selected item
                listbox.SelectedIndex = -1;

                if (!string.IsNullOrEmpty(persistedValue))
                {
                    string[] selectedItems = persistedValue.Split(';');
                    foreach (string sel in selectedItems)
                    {
                        int index = listbox.FindString(sel);
                        if (index != -1)
                            listbox.SetSelected(index, true);
                    }
                }
            }

            AddRowStyle(ControlType.ListBox);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> whereClauses = new List<string>();

                for (int i = 0; i < controlTableLayoutPanel.RowCount; ++i)
                {
                    Control ctrl = controlTableLayoutPanel.GetControlFromPosition((int)ControlColumnIndex.Control1, i);

                    ApttusField field = ctrl.Tag as ApttusField;
                    string searchCriteria = string.Empty;

                    if (ctrl.GetType() == typeof(TextBox))
                    {
                        TextBox txt = ctrl as TextBox;
                        if (!string.IsNullOrEmpty(txt.Text))
                            searchCriteria = txt.Text;
                    }
                    else if (ctrl.GetType() == typeof(ComboBox))
                    {
                        ComboBox cbo = ctrl as ComboBox;
                        if (cbo.SelectedIndex != -1)
                            searchCriteria = Convert.ToString(cbo.Items[cbo.SelectedIndex]);
                    }
                    else if (ctrl.GetType() == typeof(ListBox))
                    {
                        ListBox listBox = ctrl as ListBox;
                        if (listBox.SelectedItems.Count > 0)
                        {
                            foreach (var item in listBox.SelectedItems)
                                searchCriteria = searchCriteria + Convert.ToString(item) + ";";

                            searchCriteria = searchCriteria.Remove(searchCriteria.Length - 1, 1);
                        }
                    }
                    else if (ctrl.GetType() == typeof(DateTimePicker))
                    {
                        DateTimePicker picker = ctrl as DateTimePicker;
                        if (picker.CustomFormat != " ")
                        {
                            //Utils.IsValidDate(picker.Value.ToString(), sField.Datatype);
                            searchCriteria = picker.Value.ToShortDateString();
                        }
                    }
                    else if (ctrl.GetType() == typeof(CheckBox))
                    {
                        CheckBox chk = ctrl as CheckBox;
                        searchCriteria = chk.Checked.ToString();
                    }

                    // Create all Where clauses
                    if (!string.IsNullOrEmpty(searchCriteria))
                    {
                        //whereClauses.Add(controller.GetWhereClauseBasedOnDataType(searchCriteria, field));
                        string fieldWhereClause = ExpressionBuilderHelper.GetLikeWhereClauseBasedOnDataType(searchCriteria, field);
                        // Add Where clause only if it was constructed
                        if (!string.IsNullOrEmpty(fieldWhereClause))
                            whereClauses.Add(fieldWhereClause);
                    }
                    controller.AddPersistedSearchValue(field, searchCriteria);
                }

                if (whereClauses.Count > 0)
                {
                    advancedSearchWhereClause = Constants.OPEN_BRACKET;
                    advancedSearchWhereClause += string.Join<string>(" AND ", whereClauses);
                    advancedSearchWhereClause += Constants.CLOSE_BRACKET;
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

    }

}
