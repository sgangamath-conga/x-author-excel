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
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class SearchAndSelectActionView : Form
    {
        const int defaultValueColumnIndex = 4;
        SearchAndSelect searchAndSelect;
        BindingSource searchGridBindingSource = new BindingSource();
        BindingSource resultGridBindingSource = new BindingSource();
        ExpressionBuilderView ExpressionBuilder = null;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        // Constructor
        public SearchAndSelectActionView()
        {
            searchAndSelect = new SearchAndSelect();
            InitializeComponent();
            SetCultureData();
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.DESIGNER_PRODUCT_NAME;
            FillObjects();
        }
        public SearchAndSelectActionView(Guid AppObjectUniqueId)
        {
            searchAndSelect = new SearchAndSelect();
            InitializeComponent();
            SetCultureData();
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.DESIGNER_PRODUCT_NAME;
            FillObjects();
            cboSearchObject.SelectedValue = AppObjectUniqueId;
            cboSearchObject.Enabled = false;
        }

        private void SetCultureData()
        {
            label1.Text = resourceManager.GetResource("COMMON_SearchandSelect_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnSave.Text = resourceManager.GetResource("COMMON_Save_AMPText");
            DataType.HeaderText = resourceManager.GetResource("SSACTION_DataType_HeaderText");
            DefaultValue.HeaderText = resourceManager.GetResource("COMMON_DefaultValue_Text");
            groupActionHeader.Text = resourceManager.GetResource("COMMON_SearchandSelect_Text");
            Label.HeaderText = resourceManager.GetResource("COMMON_FieldLabel_Text");
            label2.Text = resourceManager.GetResource("SSACTION_label2_Text");
            label3.Text = resourceManager.GetResource("SSACTION_label3_Text");
            label4.Text = resourceManager.GetResource("SSACTION_label4_Text");
            lblName.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            lblSearchObject.Text = resourceManager.GetResource("COMMON_Object_Text");
            rbMultipleRecords.Text = resourceManager.GetResource("SSACTION_rbMultipleRecords_Text");
            rbSingleRecord.Text = resourceManager.GetResource("SSACTION_rbSingleRecord_Text");
            ResultHeader.HeaderText = resourceManager.GetResource("SSACTION_ResultHeader_HeaderText");
            ResultName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
            ResultSelect.HeaderText = resourceManager.GetResource("COMMON_Display_Text");
            ResultSort.HeaderText = resourceManager.GetResource("SSACTION_ResultSort_HeaderText");
            SearchCheckBox.HeaderText = resourceManager.GetResource("SSACTION_SearchCheckBox_HeaderText");
            SearchDataType.HeaderText = resourceManager.GetResource("COMMON_DataType_Text");
            SearchDefaultValue.HeaderText = resourceManager.GetResource("COMMON_DefaultValue_Text");
            SearchFieldLabel.HeaderText = resourceManager.GetResource("COMMON_FieldLabel_Text");
            SearchFieldName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
            SearchUp1.HeaderText = resourceManager.GetResource("COMMON_Up_Text");
            SearchUp1.ToolTipText = resourceManager.GetResource("COMMON_Up_Text");
            tabFilters.Text = resourceManager.GetResource("COMMON_Filters_Text");
            tabSearch.Text = resourceManager.GetResource("COMMON_Options_Text");
        }
        /// <summary>
        /// Used for editing the selected search and select object.
        /// </summary>
        /// <param name="objSearchAndSelect">Object which will be updated to reflect the editing changes being made.</param>
        public SearchAndSelectActionView(SearchAndSelect objSearchAndSelect)
        {
            searchAndSelect = objSearchAndSelect;
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            // FillObjects
            FillObjects();
            //FillPageSize();

            LoadAction();


        }
        #region Methods

        public SearchAndSelect Object
        {
            get
            {
                return searchAndSelect;
            }
        }

        /// <summary>
        /// Fill Objects
        /// </summary>
        private void FillObjects()
        {
            cboSearchObject.DataSource = new BindingSource(ConfigurationManager.GetInstance.GetApttusObjectsWithObjectType(), null);
            cboSearchObject.DisplayMember = "Value";
            cboSearchObject.ValueMember = "Key";

            cboSearchObject.DropDown += new EventHandler(AdjustWidth_DropDown);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (currentItem is KeyValuePair<Guid, string>) //ApttusObject
                {
                    KeyValuePair<Guid, string> item = ((KeyValuePair<Guid, string>)currentItem);
                    newWidth = (int)g.MeasureString(item.Value, font).Width + vertScrollBarWidth;
                }
                else if (currentItem is ApttusField)
                    newWidth = (int)g.MeasureString(((ApttusField)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        /// <summary>
        /// Fill PageSize
        /// </summary>
        //private void FillPageSize()
        //{
        //    cboPageSize.Items.Add("10");
        //    cboPageSize.Items.Add("20");
        //    cboPageSize.Items.Add("30");
        //    cboPageSize.Items.Add("40");
        //    cboPageSize.Items.Add("50");
        //    cboPageSize.SelectedIndex = 0;
        //}

        /// <summary>
        /// Fill Search List View
        /// </summary>
        /// <param name="objectUniqueId"></param>
        private void FillSearchGridView(Guid objectUniqueId)
        {
            // Search grid
            dgSearch.AutoGenerateColumns = false;
            searchGridBindingSource.DataSource = searchAndSelect.GetSearchFieldsList(objectUniqueId);
            dgSearch.DataSource = searchGridBindingSource;
            dgSearch.ClearSelection();

            // Result grid
            dgResult.AutoGenerateColumns = false;
            resultGridBindingSource.DataSource = searchAndSelect.GetResultFieldsList(objectUniqueId);
            dgResult.DataSource = resultGridBindingSource;
            dgResult.ClearSelection();
        }

        private void InitializeExpressionBuilder()
        {
            ExpressionBuilder = new ExpressionBuilderView();
            ExpressionBuilder.Name = "ExpressionBuilder";
            this.pnlExpressionBuilder.Controls.Add(ExpressionBuilder);
            ExpressionBuilder.Parent = this.pnlExpressionBuilder;
            ExpressionBuilder.Dock = DockStyle.Fill;
            ExpressionBuilder.SetExpressionBuilder(searchAndSelect.SearchFilterGroups, SelectedApttusObject);
        }

        #endregion

        private void cboSearchObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchObject.SelectedIndex == -1)
                return;
            Guid objectUniqueId = SelectedObjectUniqueId;
            // Check if object type is repeating than enable true multiplerecords radio button
            ApttusObject apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(objectUniqueId);
            if (apttusObject == null)
                return;

            if (apttusObject.ObjectType == ObjectType.Repeating || apttusObject.ObjectType == ObjectType.CrossTab)
            {
                rbMultipleRecords.Enabled = true;
            }
            else
            {
                rbMultipleRecords.Checked = false;
                rbMultipleRecords.Enabled = false;
            }
            // Bind search Lietview
            FillSearchGridView(objectUniqueId);

            if (ExpressionBuilder != null && searchAndSelect != null)
                 ExpressionBuilder.SetExpressionBuilder(searchAndSelect.SearchFilterGroups, SelectedApttusObject);
        }

        private ApttusObject SelectedApttusObject
        {
            get
            {
                return ApplicationDefinitionManager.GetInstance.GetAppObject(SelectedObjectUniqueId);
            }
        }

        private Guid SelectedObjectUniqueId
        {
            get
            {
                return ((KeyValuePair<Guid, string>)cboSearchObject.SelectedItem).Key;
            }
        }

        /// <summary>
        /// Load the state of search and select object into the form, which will be used for editing.
        /// </summary>
        /// <param name="objSearchAndSelect"></param>
        private void LoadAction()
        {
            List<SearchField> selectedSearchFields = new List<SearchField>(searchAndSelect.SearchFields);
            List<ResultField> selectedResultFields = new List<ResultField>(searchAndSelect.ResultFields);

            //Load Action Name
            txtActionName.Text = searchAndSelect.Name;
            //Load Target Object
            ApttusObject apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(searchAndSelect.TargetObject);
            cboSearchObject.SelectedValue = apttusObject.UniqueId;
            //Load Page Size
            // cboPageSize.SelectedItem = searchAndSelect.PageSize;

            // Load the selected search fields
            // We merge the selected fields into all fields as this will load the state of defaultvalue, selected value of the checkbox.
            var mergedSearchFields = selectedSearchFields.Union(searchAndSelect.AllSearchFields).Distinct(new SearchFieldComparer()).ToList();
            searchAndSelect.AllSearchFields = mergedSearchFields;
            //set the search grid datasource.
            searchGridBindingSource.DataSource = mergedSearchFields;
            dgSearch.DataSource = searchGridBindingSource;


            // Load the selected search fields
            var mergedResultFields = selectedResultFields.Union(searchAndSelect.AllResultFields).Distinct(new ResultFieldComparer()).ToList();
            //We merge the selected result fields into all fields as this load the state of sortfield and selected value of the checkbox.
            searchAndSelect.AllResultFields = mergedResultFields;
            //set the search grid datasource.
            resultGridBindingSource.DataSource = mergedResultFields;
            dgResult.DataSource = resultGridBindingSource;

            chkAllowSavingFilters.Checked = searchAndSelect.AllowSavingFilters;

        }


        /// <summary>
        /// Saves action and its properties to the config file.
        /// </summary>
        /// <returns></returns>
        private void SaveAction()
        {
            try
            {
                bool blnValid = false;
                string errorMessage = string.Empty;
                List<SearchFilterGroup> lstFilters = ExpressionBuilder.SaveExpression(out errorMessage);
                if (String.IsNullOrEmpty(errorMessage))
                {
                    searchAndSelect.SearchFilterGroups = lstFilters;

                    searchAndSelect.Name = txtActionName.Text;

                    searchAndSelect.TargetObject = SelectedObjectUniqueId;

                    //AB-1827, switched to refer to tag instead of radio button text.
                    searchAndSelect.RecordType = rbSingleRecord.Checked ? rbSingleRecord.Tag.ToString() : rbMultipleRecords.Tag.ToString();

                    searchAndSelect.AllowSavingFilters = chkAllowSavingFilters.Checked;

                    //searchAndSelect.PageSize = Convert.ToString(cboPageSize.SelectedItem);
                    blnValid = searchAndSelect.SaveAction();
                    if (!blnValid)
                    {
                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), this.Handle.ToInt32());
                        this.Close();
                    }
                    //else part added to resolve AB-1030
                    else
                    {
                        if (lstFilters[0].Filters.Count == 0)
                        {
                            ExpressionBuilder.ResetExpressionBuilder();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!HasValidateErrors())
                SaveAction();
        }

        private void SearchConfiguration_Load(object sender, EventArgs e)
        {
            ActiveControl = txtActionName;
            InitializeExpressionBuilder();

            //This is done so that expression builder gets the available size in case of 125-150% resolution.
            if ((ExpressionBuilder.PreferredSize.Width + SystemInformation.VerticalScrollBarWidth) < this.Size.Width)
                this.AutoSize = true;
            else
                this.Size = new Size(ExpressionBuilder.PreferredSize.Width + SystemInformation.VerticalScrollBarWidth, this.Height);
            // Please don't set the below properties in the designer as there is a reason why both properties are not set
            // We cannot resize the tab control, hence all the controls including panels except pnlResult will be drawn perfectly.
            // Hence whatever size is now left will be given to pnlResult, and finally make them dockable within the available size.
            // Hence dgResult Grid won't get cut and it will fit in the available size.
            pnlResult.Dock = DockStyle.Fill;
            dgResult.Dock = DockStyle.Fill;
        }

        private void SearchConfiguration_Shown(object sender, EventArgs e)
        {
            rbSingleRecord.Checked = false;
            rbMultipleRecords.Checked = false;

            RadioButton rb = searchAndSelect.RecordType != null && searchAndSelect.RecordType.Equals("Multiple") ? rbMultipleRecords : rbSingleRecord;
            rb.Checked = true;

            btnCancel.CausesValidation = false;

        }

        private void RemoveSearchFilterForSearchableField(int rowIndex)
        {
            searchGridBindingSource.Position = rowIndex;
            string FieldId = (searchGridBindingSource.Current as SearchField).Id;

            SearchFilter filter = searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters.Where(sf => sf.FieldId.Equals(FieldId) && sf.ValueType == ExpressionValueTypes.UserInput).FirstOrDefault();
            searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters.Remove(filter);

            int index = 0;
            foreach (SearchFilter sf in searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters)
                sf.SequenceNo = ++index;

            ExpressionBuilder.SetExpressionBuilder(searchAndSelect.SearchFilterGroups, SelectedApttusObject);
        }

        private bool AddSearchFilterForSearchableField(int rowIndex)
        {
            int nSequenceNo = 1;
            if (searchAndSelect.SearchFilterGroups == null ||
                (searchAndSelect.SearchFilterGroups != null && searchAndSelect.SearchFilterGroups.Count == 0))
            {
                searchAndSelect.SearchFilterGroups = new List<SearchFilterGroup>();
                SearchFilterGroup sfg = new SearchFilterGroup();
                sfg.Filters = new List<SearchFilter>();
                searchAndSelect.SearchFilterGroups.Add(sfg);
                ExpressionBuilder.ResetExpressionBuilder();
            }
            else
                nSequenceNo = searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters.Count + 1;

            // There are already 7 Filters defined, which is max limit so return from here
            if (nSequenceNo > Constants.EXPRESSION_BUILDER_MAX_ROWS)
            {
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("COMMON_SearchableFieldLimit_InfoMsg"), Convert.ToString(Constants.EXPRESSION_BUILDER_MAX_ROWS)), Constants.DESIGNER_PRODUCT_NAME, this.Handle.ToInt32());
                return false;
            }

            // Do not create User Input filter for "Not Supported" datatypes, as this is not supported from Action UI as well.
            if ((searchGridBindingSource.Current as SearchField).Datatype == Datatype.NotSupported)
                return true;

            SearchFilter sf = new SearchFilter();
            ApttusObject obj = SelectedApttusObject;
            searchGridBindingSource.Position = rowIndex;
            sf.FieldId = (searchGridBindingSource.Current as SearchField).Id;
            sf.AppObjectUniqueId = obj.UniqueId;
            sf.Operator = (searchGridBindingSource.Current as SearchField).Datatype == Datatype.String ? "like %#FILTERVALUE%" : Constants.EQUALS;
            sf.ValueType = ExpressionValueTypes.UserInput;
            sf.Value = string.Empty;
            sf.SequenceNo = nSequenceNo;
            sf.QueryObjects = new List<QueryObject>();
            sf.SearchFilterLabel = obj.Fields.FirstOrDefault(f => f.Id == sf.FieldId).Name; // Set the label as the Field Name
            searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters.Add(sf);
            ExpressionBuilder.CreateExpressionRow(sf);

            return true;
        }

        private void dgSearch_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == defaultValueColumnIndex)
                return;
            DataGridViewCheckBoxCell cell = dgSearch.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
            if (cell == null)
                return;
            object objValue = cell.Value;
            if (objValue != null && cell.Value.Equals(true))
                dgSearch.Columns[defaultValueColumnIndex].ReadOnly = false;
            else
                dgSearch.Columns[defaultValueColumnIndex].ReadOnly = true;

            if (cell.Value.Equals(true))
            {
                bool bResult = AddSearchFilterForSearchableField(e.RowIndex);
                if (!bResult)
                    cell.Value = bResult;
            }
            else
                RemoveSearchFilterForSearchableField(e.RowIndex);
        }

        private void dgSearch_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == defaultValueColumnIndex)
                dgSearch.Columns[defaultValueColumnIndex].ReadOnly = !dgSearch.Columns[defaultValueColumnIndex].ReadOnly;
        }

        private void dgSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == defaultValueColumnIndex)
            {
                DataGridViewCheckBoxCell cell = dgSearch.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
                if (cell == null)
                    return;
                object objValue = cell.Value;
                if (objValue != null && cell.Value.Equals(true))
                    dgSearch.Columns[defaultValueColumnIndex].ReadOnly = false;
                else
                    dgSearch.Columns[defaultValueColumnIndex].ReadOnly = true;
            }
        }

        /// <summary>
        /// Push the row up in the grid
        /// </summary>
        /// <param name="bindingSource"></param>
        private void PushRowUp(BindingSource bindingSource)
        {
            try
            {
                int position = bindingSource.Position;
                if (position == 0) return;

                bindingSource.RaiseListChangedEvents = false;

                object current = bindingSource.Current;
                bindingSource.Remove(current);

                position--;

                bindingSource.Insert(position, current);
                bindingSource.Position = position;

                bindingSource.RaiseListChangedEvents = true;
                bindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                bindingSource.RaiseListChangedEvents = true;
            }
        }

        /// <summary>
        /// Push the row down in the grid.
        /// </summary>
        /// <param name="bindingSource"></param>
        private static void PushRowDown(BindingSource bindingSource)
        {
            try
            {
                int position = bindingSource.Position;
                if (position == bindingSource.Count - 1) return;  // already at bottom

                bindingSource.RaiseListChangedEvents = false;

                object current = bindingSource.Current;
                bindingSource.Remove(current);

                position++;

                bindingSource.Insert(position, current);
                bindingSource.Position = position;

                bindingSource.RaiseListChangedEvents = true;
                bindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                bindingSource.RaiseListChangedEvents = true;
            }
        }

        private void btnSearchUp_Click(object sender, EventArgs e)
        {
            PushRowUp(searchGridBindingSource);
        }

        private void btnSearchDown_Click(object sender, EventArgs e)
        {
            PushRowDown(searchGridBindingSource);
        }

        private void btnResultUp_Click(object sender, EventArgs e)
        {
            PushRowUp(resultGridBindingSource);
        }

        private void btnResultDown_Click(object sender, EventArgs e)
        {
            PushRowDown(resultGridBindingSource);
        }

        private void dgResult_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            updateUpDownButtons(e, btnResultUp, btnResultDown);
        }

       

        /// <summary>
        /// Update the enabled property of up / down buttons based on row selection in the grid
        /// </summary>
        /// <param name="e"></param>
        /// <param name="btnUp"></param>
        /// <param name="btnDown"></param>
        private void updateUpDownButtons(DataGridViewCellEventArgs e, Button btnUp, Button btnDown)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.RowIndex == 0)
                btnUp.Enabled = false;
            else
                btnUp.Enabled = true;

            if (e.RowIndex == (searchGridBindingSource.Count - 1))
                btnDown.Enabled = false;
            else
                btnDown.Enabled = true;
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtActionName.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtActionName, resourceManager.GetResource("SSACTVIEWVALID_txtActionName_Validating_ErrMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(txtActionName, String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool HasValidateErrors()
        {
            return ApttusFormValidator.hasValidationErrors(pnlBasicControls.Controls);
        }

        private void cboSearchObject_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(cboSearchObject.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(cboSearchObject, resourceManager.GetResource("CONST_OBJFIELDNAMEREQUIRED"));
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cboSearchObject, String.Empty);
            }
        }

        private void dgSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgSearch.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }  
    }

    public class SearchFieldComparer : IEqualityComparer<SearchField>
    {
        public SearchFieldComparer()
        {

        }
        public bool Equals(SearchField sf1, SearchField sf2)
        {
            if (sf1.Id.Equals(sf2.Id))
            {
                sf1.IsSelected = true;
                sf2.IsSelected = true;
                return true;
            }
            return false;
        }

        public int GetHashCode(SearchField obj)
        {
            return obj.ToString().GetHashCode();
        }

    }

    public class ResultFieldComparer : IEqualityComparer<ResultField>
    {
        public ResultFieldComparer()
        {

        }
        public bool Equals(ResultField sf1, ResultField sf2)
        {
            if (sf1.Id.Equals(sf2.Id))
            {
                sf1.IsSelected = true;
                sf2.IsSelected = true;
                return true;
            }
            return false;
        }

        public int GetHashCode(ResultField obj)
        {
            return obj.ToString().GetHashCode();
        }

    }
}
