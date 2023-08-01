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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class RecordSelectionPage : ApttusWizardPageView
    {
        private ApttusObject ModelObject;
        private ExpressionBuilderHost host;
        public ExpressionBuilderModel expressionModel = new ExpressionBuilderModel();
        public bool IsWizardInEditMode { get; private set; }
        private ActionType selectedActionType = ActionType.None;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public RecordSelectionPage(Panel view, WizardModel model, PageIndex index, bool editMode)
            : base(view, model, index)
        {
            IsWizardInEditMode = editMode;
            InitializeComponent();
            SetCultureData();
        }

        private void SetCultureData()
        {
            chkDisplayFields.Text = resourceManager.GetResource("COMMON_DisplayFields_Text");
            chkSaveFields.Text = resourceManager.GetResource("COMMON_SaveFields_Text");
            Display.HeaderText = resourceManager.GetResource("COMMON_Display_Text");
            DisplayOrder.HeaderText = resourceManager.GetResource("QARECSEL_DisplayOrder_HeaderText");
            FieldName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
            groupBox1.Text = resourceManager.GetResource("COMMON_SelectAll_Text");
            label3.Text = resourceManager.GetResource("QARECSEL_label3_Text");
            lblObject.Text = resourceManager.GetResource("COMMON_Object_Text");
            ResultFields.HeaderText = resourceManager.GetResource("QARECSEL_ResultFields_HeaderText");
            Save.HeaderText = resourceManager.GetResource("COMMON_btnSave_Text");
            SearchFields.HeaderText = resourceManager.GetResource("COMMON_SearchField_Text");
            btnFilter.Text = resourceManager.GetResource("COMMON_Filters_Text");
        }

        private void RecordSelectionPage_Load(object sender, EventArgs e)
        {
            this.ActiveControl = dgvFieldSelectionGrid;
            this.ApttusWizardPageActivate += RecordSelectionPage_ApttusWizardPageActivate;

            //If no filter exist create a filter for the parent lookup for the child object.
            if (Model.AppType == QuickAppType.ParentChildApp && PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                IsChildLookupFilterCreated();
        }

        private void EnableDisableDisplayCheckBoxEvent()
        {
            chkDisplayFields.CheckedChanged -= chkDisplayFields_CheckedChanged;
            chkDisplayFields.Checked = false;
            chkSaveFields.Checked = false;
            chkSaveFields.Enabled = false;
            chkDisplayFields.CheckedChanged += chkDisplayFields_CheckedChanged;
        }

        void RecordSelectionPage_ApttusWizardPageActivate()
        {
            // For list by default label is "Object :", for parent child change accordingly.
            lblObject.Text = resourceManager.GetResource("COMMON_Object_Text"); // "Object :";
            if (Model.AppType == QuickAppType.ParentChildApp)
            {
                if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                    lblObject.Text = resourceManager.GetResource("COMMON_ChildObject_Text"); // "Child Object :";
                else if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                    lblObject.Text = resourceManager.GetResource("QAOBJSEL_lblObject_Text");//  "Parent Object :";
            }

            List<ActionSelectionFilter> recordSelectionFilters = ActionSelectionFilter.GetSupportedRecordSelection();
            bool bResetViewAndModel = false;

            RelationalObject relationObjectType = RelationalObject.ParentObject;
            string objectId = string.Empty;
            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
            {
                relationObjectType = RelationalObject.ParentObject;
                objectId = Model.Object.Id;
                if (ModelObject != Model.Object)
                {
                    bResetViewAndModel = true;
                }
                //else if (dgvFieldSelectionGrid.Rows.Count != Model.Object.Fields.Count)
                //    bResetViewAndModel = true;

                if (Model.IsParentDirty || bResetViewAndModel)
                {
                    Model.IsParentDirty = false;

                    EnableDisableDisplayCheckBoxEvent();

                    ModelObject = Model.Object;

                    UpdateView(Model.Object, relationObjectType);

                    if (!IsWizardInEditMode)
                        UpdateExpressionFilterFields();
                    else
                    {
                        //Edit Mode.
                        selectedActionType = Model.Actions.Find(act => act.ObjectType == RelationalObject.ParentObject).ActionType;
                    }
                    //ResetFilters(); //Since the parent object is changed, reset the filters applied if any.
                    if (bResetViewAndModel)
                        UpdateActionSelection(recordSelectionFilters.Where(rs => rs.AppType == Model.AppType && rs.ObjectType == RelationalObject.ParentObject).ToList(), relationObjectType);
                }

                if (Model.AppType == QuickAppType.ParentChildApp)
                    DisableIDCellForSearchAndSelectAction();
            }
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
            {
                relationObjectType = RelationalObject.ChildObject;
                objectId = Model.Object.Children[0].Id;

                if (ModelObject != Model.Object.Children[0])
                {
                    bResetViewAndModel = true;
                    //Don't reset the fields in edit mode.
                    if (!IsWizardInEditMode)
                        Model.DisplayFields.RemoveAll(fld => fld.FieldRelation == RelationalObject.ChildObject);
                }

                if (Model.IsChildDirty || bResetViewAndModel)
                {
                    Model.IsChildDirty = false;

                    EnableDisableDisplayCheckBoxEvent();

                    ModelObject = Model.Object.Children[0];

                    UpdateView(Model.Object.Children[0], relationObjectType);

                    if (!IsWizardInEditMode)
                        UpdateExpressionFilterFields();
                    else
                    {
                        //Edit Mode.
                        selectedActionType = Model.Actions.Find(act => act.ObjectType == RelationalObject.ChildObject).ActionType;
                    }
                    //    ResetFilters(); //Since the child object is changed, reset the filters applied if any.
                    if (bResetViewAndModel)
                        UpdateActionSelection(recordSelectionFilters.Where(rs => rs.AppType == Model.AppType && rs.ObjectType == RelationalObject.ChildObject).ToList(), relationObjectType);
                }
            }

            // Reset filters if field is removed
            UpdateExpressionFilterFields();

            if (IsWizardInEditMode)
            {
                List<ActionSelectionFilter> filters = cboActionSelection.DataSource as List<ActionSelectionFilter>;
                if (relationObjectType == RelationalObject.ParentObject)
                {
                    ActionSelectionFilter actionFilter = Model.Actions.Find(act => act.ObjectType == RelationalObject.ParentObject);
                    cboActionSelection.SelectedItem = filters.Find(action => action.ActionType == actionFilter.ActionType && action.AppType == Model.AppType);
                }
                else
                {
                    ActionSelectionFilter actionFilter = Model.Actions.Find(act => act.ObjectType == RelationalObject.ChildObject);
                    cboActionSelection.SelectedItem = filters.Find(action => action.ActionType == actionFilter.ActionType && action.AppType == Model.AppType);
                }

                ExpressionBuilderModel expressionBuilderModel = Model.Filters.Find(filter => filter.RelationObject == relationObjectType && filter.Object.Id.Equals(objectId));
                if (expressionBuilderModel != null)
                    expressionModel = expressionBuilderModel;
            }

            ResizeFieldsGridColumnWidth();
        }

        private void UpdateExpressionFilterFields()
        {
            ExpressionBuilderModel currentExpressionModel = null;
            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ParentObject).FirstOrDefault();
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ChildObject).FirstOrDefault();

            if (currentExpressionModel == null)
                currentExpressionModel = expressionModel;

            if (currentExpressionModel.Object != null && (currentExpressionModel.Object.UniqueId != ModelObject.UniqueId))
            {
                ResetFilters(true);
                IsChildLookupFilterCreated();
            }

            if (currentExpressionModel.Filters != null && currentExpressionModel.Filters.Count > 0)
            {
                if (currentExpressionModel.Filters.FirstOrDefault().Filters.Count > 0)
                {
                    List<SearchFilter> currentFilter = currentExpressionModel.Filters.FirstOrDefault().Filters;
                    for (int i = 0; i < currentFilter.Count; i++)
                    {
                        ApttusField field = ModelObject.Fields.Find(f => f.Id.Equals(currentFilter[i].FieldId));
                        if (field == null)
                            currentFilter.RemoveAt(i);
                    }

                    int index = 0;
                    foreach (SearchFilter sf in currentExpressionModel.Filters.FirstOrDefault().Filters)
                        sf.SequenceNo = ++index;
                }
            }
        }

        private void UpdateActionSelection(List<ActionSelectionFilter> actionFilters, RelationalObject currentRelationalObject)
        {
            cboActionSelection.DataSource = actionFilters;
            cboActionSelection.DisplayMember = QuickAppConstants.ActionDisplayMember;

            // On Next Back Navigation, assign previously selected Action again
            ActionSelectionFilter selectedAction = Model.Actions.Where(act => act.ObjectType == currentRelationalObject).FirstOrDefault();
            if (selectedAction != null)
            {
                ActionSelectionFilter currentAction = actionFilters.Where(f => f.ObjectType == selectedAction.ObjectType && f.ActionType == selectedAction.ActionType).FirstOrDefault();
                cboActionSelection.SelectedItem = currentAction;
            }
        }

        private void UpdateView(ApttusObject apttusObj, RelationalObject relationalObj)
        {
            txtObjectName.Text = apttusObj.Name;

            List<QuickAppFieldAttribute> list = new List<QuickAppFieldAttribute>();
            foreach (ApttusField field in apttusObj.Fields)
            {
                bool display = false, save = false, searchable = false, resultable = false;
                string objectId = string.Empty;
                string displayOrder = string.Empty;

                QuickAppFieldAttribute fieldAttr = Model.DisplayFields.Find(fld => fld.ObjectId.Equals(apttusObj.Id) && fld.FieldId.Equals(field.Id));
                if (fieldAttr != null)
                {
                    display = fieldAttr.Display;
                    save = fieldAttr.Save;
                    searchable = fieldAttr.SearchFields;

                    resultable = fieldAttr.ResultFields;
                    objectId = apttusObj.Id;
                    displayOrder = fieldAttr.DisplayOrder;
                }
                QuickAppFieldAttribute fs = new QuickAppFieldAttribute
                {
                    FieldName = field.Name,
                    FieldId = field.Id,
                    Display = display,
                    Save = save,
                    ResultFields = resultable,
                    SearchFields = searchable,
                    ObjectId = objectId,
                    DisplayOrder = displayOrder,
                    FieldRelation = relationalObj
                };
                list.Add(fs);
            }

            //Remove those fields which are removed from ApttusObjec.Fields but they exist in Model.DisplayFields. The below if condition does the same.
            List<string> removeitems = (from i in Model.DisplayFields.Where(fld => fld.ObjectId.Equals(apttusObj.Id))
                                        where !(from s in apttusObj.Fields
                                                select s.Id).Contains(i.FieldId)
                                        select i.FieldId).ToList();

            // Remove fields
            if (removeitems != null && removeitems.Count > 0)
                Model.DisplayFields.RemoveAll(f => removeitems.Contains(f.FieldId));

            //if there are no search fields, reset the expression builder filters.
            if (Model.DisplayFields.Where(df => df.SearchFields == true).Count() == 0)
                ResetFilters(false);

            updateDisplayOrder(list);

            dgvFieldSelectionGrid.AutoGenerateColumns = false;
            dgvFieldSelectionGrid.DataSource = list;

            //dgvFieldSelectionGrid.AutoResizeColumns();
            ResizeFieldsGridColumnWidth();

            if (IsWizardInEditMode)
                updateSaveCellAndOrderCellInEditMode();

            updateSaveCellForNonEditableFields();
        }

        /// <summary>
        /// This function updates the displayorder if the user add/removes a field after selection.
        /// </summary>
        public void updateDisplayOrder(List<QuickAppFieldAttribute> listofSelectedFields)
        {
            List<QuickAppFieldAttribute> fieldsTobeDisplayed = listofSelectedFields.Where(fld => fld.Display == true).OrderBy(fld => Convert.ToInt32(fld.DisplayOrder)).ToList();
            if (fieldsTobeDisplayed.Count == 0)
                return;

            int displayOrderIndex = 1;
            foreach (QuickAppFieldAttribute field in fieldsTobeDisplayed)
            {
                field.DisplayOrder = displayOrderIndex.ToString();
                ++displayOrderIndex;
            }
        }

        /// <summary>
        /// Updates the readonly status of ordercell and savecell based on the value of displaycell in edit mode
        /// </summary>
        private void updateSaveCellAndOrderCellInEditMode()
        {
            foreach (DataGridViewRow dataGridRow in dgvFieldSelectionGrid.Rows)
            {
                DataGridViewCell displayCell = dataGridRow.Cells[1];
                DataGridViewCell saveCell = dataGridRow.Cells[3];

                DataGridViewCell orderCell = dataGridRow.Cells[2];

                orderCell.ReadOnly = saveCell.ReadOnly = !(bool)displayCell.EditedFormattedValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateSaveCellForNonEditableFields()
        {
            ApttusObject obj = PageNumber == PageIndex.ParentObjectRecordSelectionPage ? Model.Object : Model.Object.Children[0];

            foreach (DataGridViewRow dataGridRow in dgvFieldSelectionGrid.Rows)
            {
                DataGridViewCell fieldCell = dataGridRow.Cells[0];
                DataGridViewCell saveCell = dataGridRow.Cells[3];

                ApttusField currentField = obj.Fields.Where(f => f.Name.Equals(fieldCell.Value)).FirstOrDefault();
                if (currentField != null && (currentField.Datatype == Datatype.Formula || currentField.Id.Equals(obj.IdAttribute)))
                    saveCell.ReadOnly = true;
            }
        }

        /// <summary>
        /// Disables the search fields and result fields checkbox for ID row.
        /// </summary>
        private void DisableIDCellForSearchAndSelectAction()
        {
            ApttusObject obj = PageNumber == PageIndex.ParentObjectRecordSelectionPage ? Model.Object : Model.Object.Children[0];

            List<QuickAppFieldAttribute> list = dgvFieldSelectionGrid.DataSource as List<QuickAppFieldAttribute>;
            int rowIndex = list.FindIndex(field => field.FieldId.Equals(obj.IdAttribute));

            if (rowIndex >= 0 && dgvFieldSelectionGrid.Columns.Contains(SearchFields))
            {
                dgvFieldSelectionGrid.Rows[rowIndex].Cells[SearchFields.Name].ReadOnly = true;
                dgvFieldSelectionGrid.Rows[rowIndex].Cells[ResultFields.Name].ReadOnly = true;
            }
        }

        private void ResetFilters(bool bClearObject = true)
        {
            if (bClearObject)
                expressionModel.Object = null;
            Model.Filters.Remove(expressionModel);
            expressionModel.Filters.Clear();
        }

        private void SaveFilters()
        {
            string errorMessage = string.Empty;
            List<SearchFilterGroup> filterList = host.ExpressionControl.SaveExpression(out errorMessage);
            if (String.IsNullOrEmpty(errorMessage))
            {
                //if SaveExpression succeeded, then only reset the filters.
                ResetFilters(false);
                if (filterList != null)
                {
                    expressionModel.Filters.AddRange(filterList);
                    // Model.Filters.Add(expressionModel);
                }
                //Filter Saved Successfully hence close the ExpressionBuilder Dialog.
                host.DialogResult = DialogResult.OK;
            }
        }

        private void updateSaveCellAndOrderCell(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            if (dgvFieldSelectionGrid.Columns[e.ColumnIndex].Name.Equals("Display"))
            {
                DataGridViewRow row = dgvFieldSelectionGrid.Rows[e.RowIndex];
                DataGridViewCell fieldCell = row.Cells[0];
                DataGridViewCell displayCell = row.Cells[e.ColumnIndex];
                DataGridViewCell saveCell = row.Cells[e.ColumnIndex + 2];
                saveCell.Value = false;

                DataGridViewCell orderCell = row.Cells[e.ColumnIndex + 1];
                orderCell.Value = string.Empty;
                CalculateDisplayOrder(e.ColumnIndex);

                ApttusObject obj = PageNumber == PageIndex.ParentObjectRecordSelectionPage ? Model.Object : Model.Object.Children[0];

                ApttusField currentField = obj.Fields.Where(f => f.Name.Equals(fieldCell.Value)).FirstOrDefault();

                if (currentField != null && currentField.Datatype == Datatype.Formula || currentField.Id.Equals(obj.IdAttribute))
                    saveCell.ReadOnly = true;
                else
                    orderCell.ReadOnly = saveCell.ReadOnly = !(bool)displayCell.EditedFormattedValue;
            }
        }

        internal void CalculateDisplayOrder(int ColumnIndex)
        {
            List<DataGridViewCell> orderCells = new List<DataGridViewCell>();

            // Create list of order cells which are checked as display field
            foreach (DataGridViewRow row in dgvFieldSelectionGrid.Rows)
            {
                DataGridViewCell displayCell = row.Cells[ColumnIndex];
                DataGridViewCell orderCell = row.Cells[ColumnIndex + 1];

                if ((bool)displayCell.EditedFormattedValue)
                {
                    if (orderCell.Value.Equals(string.Empty))
                        orderCell.Value = 0;
                    orderCells.Add(orderCell);
                }
            }

            // Sort list by display order
            orderCells = orderCells.OrderBy(o => Convert.ToInt32(o.Value)).ToList();

            // Readjust the order 
            for (int i = 0; i < orderCells.Count; i++)
            {
                if (Convert.ToInt32(orderCells[i].Value) > 0 && Convert.ToInt32(orderCells[i].Value) > (i + 1))
                    orderCells[i].Value = (i + 1);

                if (Convert.ToInt32(orderCells[i].Value) == 0)
                    orderCells[i].Value = orderCells.Count;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            updateSaveCellAndOrderCell(e);
        }

        public override void ProcessPage()
        {
            RelationalObject currentRelationalObject = RelationalObject.ParentObject;
            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
            {
                currentRelationalObject = RelationalObject.ParentObject;
                UpdateFields(Model.Object);
                Model.Actions.RemoveAll(act => act.ObjectType == RelationalObject.ParentObject);
                ActionSelectionFilter action = cboActionSelection.SelectedItem as ActionSelectionFilter;
                Model.Actions.Add(new ActionSelectionFilter(action.AppType, RelationalObject.ParentObject, action.ActionType, action.ActionTypeValue));
            }
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
            {
                currentRelationalObject = RelationalObject.ChildObject;
                UpdateFields(Model.Object.Children[0]);
                Model.Actions.RemoveAll(act => act.ObjectType == RelationalObject.ChildObject);

                ActionSelectionFilter action = cboActionSelection.SelectedItem as ActionSelectionFilter;
                Model.Actions.Add(new ActionSelectionFilter(action.AppType, RelationalObject.ChildObject, action.ActionType, action.ActionTypeValue));

                ////If no filter exist create a filter for the parent lookup for the child object.
                //if (expressionModel.Filters.Count == 0)
                //{
                //    List<SearchFilterGroup> filters = CreateChildLookupFilter();
                //    if (filters != null)
                //    {
                //        expressionModel.Object = Model.Object.Children[0];
                //        expressionModel.RelationObject = RelationalObject.ChildObject;
                //        expressionModel.Filters.AddRange(filters);
                //        Model.Filters.Add(expressionModel);
                //    }
                //}
            }

            Model.Filters.RemoveAll(filter => filter.RelationObject == currentRelationalObject);
            if (expressionModel != null && expressionModel.Filters.Count > 0)
                Model.Filters.Add(expressionModel);
        }

        internal ApttusObject TargetObject
        {
            get
            {
                ApttusObject obj = null;
                if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                    obj = Model.Object;
                if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                    obj = Model.Object.Children[0];
                return obj;
            }
        }

        internal void IsChildLookupFilterCreated()
        {
            if (expressionModel.Filters.Count == 0)
            {
                List<SearchFilterGroup> filters = CreateChildLookupFilter();
                if (filters != null)
                {
                    expressionModel.Object = Model.Object.Children[0];
                    expressionModel.RelationObject = RelationalObject.ChildObject;
                    expressionModel.Filters.AddRange(filters);
                    Model.Filters.Add(expressionModel);
                }
            }
        }

        internal List<SearchFilterGroup> CreateChildLookupFilter()
        {
            try
            {
                List<SearchFilterGroup> filterGroupList = new List<SearchFilterGroup>();
                SearchFilterGroup filterGroup = new SearchFilterGroup();
                filterGroup.Filters = new List<SearchFilter>();

                filterGroup.LogicalOperator = LogicalOperator.AND;

                SearchFilter filter = new SearchFilter();

                filter.QueryObjects = new List<QueryObject>();

                filter.AppObjectUniqueId = Model.Object.Children[0].UniqueId;

                ApttusField parentLookupField = (from child in Model.Object.Children
                                                 from field in child.Fields.Where(field => field.Datatype == Datatype.Lookup && field.LookupObject != null).Where(field => field.LookupObject.Id.Equals(Model.Object.Id))
                                                 select field).FirstOrDefault();



                if (parentLookupField == null)
                    return null;

                filter.FieldId = parentLookupField.Id;
                //filter.FieldName = parentLookupField.Name;
                filter.SequenceNo = 1;
                filter.ValueType = 0;
                filter.Operator = "=";
                filter.Value = Model.Object.UniqueId + "." + Model.Object.IdAttribute;

                filterGroup.Filters.Add(filter);
                filterGroupList.Add(filterGroup);
                return filterGroupList;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return null;
            }
        }

        internal bool AddSearchFilterForSearchableField(int rowIndex)
        {
            ExpressionBuilderModel currentExpressionModel = null;
            ApttusObject obj = null;
            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ParentObject).FirstOrDefault();
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ChildObject).FirstOrDefault();

            if (currentExpressionModel == null)
                currentExpressionModel = expressionModel;

            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
            {
                currentExpressionModel.RelationObject = RelationalObject.ParentObject;
                obj = Model.Object;
            }
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
            {
                currentExpressionModel.RelationObject = RelationalObject.ChildObject;
                obj = Model.Object.Children[0];
            }

            if (currentExpressionModel.Filters == null || (currentExpressionModel.Filters != null && currentExpressionModel.Filters.Count == 0))
            {
                currentExpressionModel.Filters = new List<SearchFilterGroup>();
                SearchFilterGroup sfg = new SearchFilterGroup();
                sfg.Filters = new List<SearchFilter>();
                sfg.LogicalOperator = LogicalOperator.AND;
                currentExpressionModel.Filters.Add(sfg);
            }

            int nSequenceNo = currentExpressionModel.Filters.FirstOrDefault().Filters.Count + 1;
            if (nSequenceNo > Constants.EXPRESSION_BUILDER_MAX_ROWS)
            {
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("COMMON_SearchableFieldLimit_InfoMsg"), Convert.ToString(Constants.EXPRESSION_BUILDER_MAX_ROWS)), Constants.DESIGNER_PRODUCT_NAME);
                return false;
            }

            QuickAppFieldAttribute selectedField = (dgvFieldSelectionGrid.DataSource as List<QuickAppFieldAttribute>)[rowIndex];
            ApttusField selectedApttusField = obj.Fields.Where(f => f.Id.Equals(selectedField.FieldId)).FirstOrDefault();

            if (selectedApttusField.Id == obj.IdAttribute || selectedApttusField.Datatype == Datatype.Textarea || selectedApttusField.Datatype == Datatype.Rich_Textarea ||
                selectedApttusField.Datatype == Datatype.Attachment || selectedApttusField.Datatype == Datatype.Email)
                return false;
                
            // Allow formula data type for string ,numeric and boolean for where clause.
            if (selectedApttusField.Datatype == Datatype.Formula)
            {
                if (!string.IsNullOrEmpty(selectedApttusField.FormulaType))
                {
                    if (ExpressionBuilderHelper.GetDataTypeFromFormulaType(selectedApttusField.FormulaType) == Datatype.AnyType)
                        return false;
                }
                else
                    return false; 
            }

            currentExpressionModel.Object = obj;
            SearchFilter sf = new SearchFilter();
            sf.FieldId = selectedApttusField.Id;
            sf.AppObjectUniqueId = obj.UniqueId;
            sf.Operator = selectedApttusField.Datatype == Datatype.String ? "like %#FILTERVALUE%" : Constants.EQUALS;
            sf.ValueType = ExpressionValueTypes.UserInput;
            sf.Value = string.Empty;
            sf.SequenceNo = nSequenceNo;
            sf.QueryObjects = new List<QueryObject>();
            // For User inputs create query object 
            sf.QueryObjects.Add(new QueryObject
            {
                SequenceNo = sf.QueryObjects.Count + 1,
                AppObjectUniqueId = sf.AppObjectUniqueId,
                LookupFieldId = string.Empty,
                RelationshipType = QueryRelationshipType.None,
                BreadCrumbLabel = obj.Name,
                BreadCrumbTooltip = obj.Name,
                DistanceFromChild = 0,
                LeafAppObjectUniqueId = sf.AppObjectUniqueId
            });

            sf.SearchFilterLabel = obj.Fields.FirstOrDefault(f => f.Id == sf.FieldId).Name; // Set the label as the Field Name
            currentExpressionModel.Filters.FirstOrDefault().Filters.Add(sf);

            // Assign it back to expression builder model
            if (IsWizardInEditMode)
                expressionModel = currentExpressionModel;
            return true;
        }

        internal void RemoveSearchFilterForSearchableField(int rowIndex)
        {
            ExpressionBuilderModel currentExpressionModel = null;

            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ParentObject).FirstOrDefault();
            else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                currentExpressionModel = Model.Filters.Where(fil => fil.RelationObject == RelationalObject.ChildObject).FirstOrDefault();

            if (currentExpressionModel == null)
                currentExpressionModel = expressionModel;

            if (currentExpressionModel.Filters == null || currentExpressionModel.Filters.Count == 0)
                return;

            QuickAppFieldAttribute selectedField = (dgvFieldSelectionGrid.DataSource as List<QuickAppFieldAttribute>)[rowIndex];
            string FieldId = selectedField.FieldId;

            SearchFilter filter = currentExpressionModel.Filters.FirstOrDefault().Filters.Where(sf => sf.FieldId.Equals(FieldId) && sf.ValueType == ExpressionValueTypes.UserInput).FirstOrDefault();
            currentExpressionModel.Filters.FirstOrDefault().Filters.Remove(filter);

            int index = 0;
            foreach (SearchFilter sf in currentExpressionModel.Filters.FirstOrDefault().Filters)
                sf.SequenceNo = ++index;
        }

        private void UpdateFields(ApttusObject apttusObj)
        {
            List<QuickAppFieldAttribute> list = (dgvFieldSelectionGrid.DataSource as List<QuickAppFieldAttribute>).
                Where(field => field.Display == true || field.SearchFields == true || field.ResultFields == true).ToList();

            Model.DisplayFields.RemoveAll(displayField => displayField.ObjectId.Equals(apttusObj.Id));

            Model.DisplayFields.AddRange(list);
            foreach (QuickAppFieldAttribute field in list)
            {
                field.ObjectId = apttusObj.Id;
                if (field.FieldId.Equals(apttusObj.IdAttribute))
                    field.Save = false;
            }
        }

        private QuickAppErrorCode ValidateFields()
        {
            List<QuickAppFieldAttribute> fieldsList = (dgvFieldSelectionGrid.DataSource as List<QuickAppFieldAttribute>);
            var fields = fieldsList.Where(field => field.Display == true).OrderBy(field => Convert.ToInt32(field.DisplayOrder));
            var searchFields = fieldsList.Where(field => field.SearchFields == true);
            var resultFileds = fieldsList.Where(field => field.ResultFields == true);

            if (fields.Count() == 0)
                return QuickAppErrorCode.NoDisplayFieldSelected;

            int order = 1;
            foreach (QuickAppFieldAttribute field in fields)
            {
                try
                {
                    if (Convert.ToInt32(field.DisplayOrder) != order)
                        return QuickAppErrorCode.IncorrectOrdering;
                    ++order;
                }
                catch (Exception)
                {
                    return QuickAppErrorCode.IncorrectOrdering;
                }
            }

            ActionSelectionFilter action = cboActionSelection.SelectedItem as ActionSelectionFilter;
            if (action.ActionType == ActionType.SearchAndSelect)
            {
                if (searchFields.Where(field => field.SearchFields == true).Count() == 0)
                    return QuickAppErrorCode.NoSearchFieldSelected;
                else if (resultFileds.Where(field => field.ResultFields == true).Count() == 0)
                    return QuickAppErrorCode.NoResultFieldSelected;

                var result = ValidateQuickAppFields(searchFields);

                if (result != QuickAppErrorCode.Success)
                    return result;

                result = ValidateQuickAppFields(resultFileds);

                if (result != QuickAppErrorCode.Success)
                    return result;
            }

            //Check if both lookup name and lookup id are save field or not.
            var saveFields = fields.Where(field => field.Save == true);
            foreach (QuickAppFieldAttribute field in saveFields)
            {
                if (field.FieldId.Contains(Constants.APPENDLOOKUPID))
                {
                    string lookupId = ApplicationDefinitionManager.GetInstance.GetLookupIdFromLookupName(field.FieldId);
                    if (String.IsNullOrEmpty(lookupId))
                        continue;
                    QuickAppFieldAttribute f = saveFields.Where(g => g.FieldId.Equals(lookupId)).Select(g => g).FirstOrDefault();
                    //if both id and name field are present, uncheck the look up ID field.
                    if (f != null)
                        f.Save = false;
                }
            }

            if (Model.AppType == QuickAppType.ListApp || (Model.AppType == QuickAppType.ParentChildApp && PageNumber == PageIndex.ChildObjectRecordSelectionPage))
            {
                //Check if there was a save field and if at all there was a save field, check whether the user selected the ID attribute or not. If not select it by default.
                if (fieldsList.Where(field => field.Save == true).Count() > 0 && !(fieldsList.Where(field => field.FieldId.Equals(ModelObject.IdAttribute) && field.Display == true).Count() == 1))
                {
                    QuickAppFieldAttribute field = fieldsList.Where(f => f.FieldId.Equals(ModelObject.IdAttribute)).FirstOrDefault();
                    field.Display = true;
                    field.DisplayOrder = order.ToString();
                }
            }
            return QuickAppErrorCode.Success;
        }

        private QuickAppErrorCode ValidateQuickAppFields(IEnumerable<QuickAppFieldAttribute> quickAppFields)
        {
            bool invalidDataTypeFound = false;
            foreach (QuickAppFieldAttribute field in quickAppFields)
            {
                ApttusObject apttusObject = null;
                if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
                {
                    apttusObject = Model.Object;
                }
                else if (PageNumber == PageIndex.ChildObjectRecordSelectionPage)
                {
                    apttusObject = Model.Object.Children[0];
                }

                var apttusField = apttusObject.Fields.Where(f => f.Id == field.FieldId).FirstOrDefault();

                if (apttusField.Datatype == Datatype.Textarea || apttusField.Datatype == Datatype.Rich_Textarea ||
                    apttusField.Datatype == Datatype.Attachment)
                {
                    field.SearchFields = false;
                    field.ResultFields = false;
                    invalidDataTypeFound = true;
                }
                if (CRMContextManager.Instance.ActiveCRM == CRM.AIC && apttusField.Datatype == Datatype.Picklist_MultiSelect)
                {
                    field.SearchFields = false;
                    field.ResultFields = false;
                    invalidDataTypeFound = true;
                }

                if (invalidDataTypeFound)
                    return QuickAppErrorCode.UnsupportedFieldTypeSelected;
            }

            return QuickAppErrorCode.Success;
        }

        public override bool ValidatePage()
        {
            QuickAppErrorCode error = ValidateFields();

            switch (error)
            {
                case QuickAppErrorCode.IncorrectOrdering:
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RECSELPAGE_OrderSeq_ErrorMsg"), resourceManager.GetResource("RECSELPAGE_OrderSeqCap_ErrorMsg"));
                    break;
                case QuickAppErrorCode.NoDisplayFieldSelected:
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RECSELPAGE_DisplayField_ErrorMsg"), resourceManager.GetResource("RECSELPAGE_DisplayFieldCap_ErrorMsg"));
                    break;
                case QuickAppErrorCode.NoSearchFieldSelected:
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RECSELPAGE_SearchField_ErrorMsg"), resourceManager.GetResource("RECSELPAGE_SearchFieldCap_ErrorMsg"));
                    break;
                case QuickAppErrorCode.NoResultFieldSelected:
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RECSELPAGE_ResultField_ErrorMsg"), resourceManager.GetResource("RECSELPAGE_ResultFieldCap_ErrorMsg"));
                    break;
                case QuickAppErrorCode.UnsupportedFieldTypeSelected:
                    //Update FieldSelectionGrid to uncheck all unsupported type fields from save and result list
                    object ds = dgvFieldSelectionGrid.DataSource;
                    dgvFieldSelectionGrid.DataSource = null;
                    dgvFieldSelectionGrid.DataSource = ds;
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RECSELPAGE_UnsupportedFieldType_ErrorMsg"), resourceManager.GetResource("RECSELPAGE_UnsupportedFieldTypeCap_ErrorMsg"));
                    break;
            }
            return error == QuickAppErrorCode.Success;
        }

        private void cboRecordSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboActionSelection.SelectedIndex == -1)
                return;
            ActionSelectionFilter action = cboActionSelection.SelectedItem as ActionSelectionFilter;
            dgvFieldSelectionGrid.Columns["SearchFields"].Visible = action.ActionType == ActionType.SearchAndSelect;
            dgvFieldSelectionGrid.Columns["ResultFields"].Visible = action.ActionType == ActionType.SearchAndSelect;

            ResizeFieldsGridColumnWidth();
        }

        private void ResetFiltersOnActionChange(ActionSelectionFilter action)
        {
            bool bResetSearchFieldsUI = false;
            if (PageNumber == PageIndex.ParentObjectRecordSelectionPage)
            {
                ActionSelectionFilter act = Model.Actions.Find(actModel => actModel.ObjectType == RelationalObject.ParentObject);
                if (action.ActionType != selectedActionType || (act != null && action.ActionType != act.ActionType))
                {
                    selectedActionType = action.ActionType;
                    ResetFilters(false);
                    bResetSearchFieldsUI = true;
                }
            }
            else
            {
                ActionSelectionFilter act = Model.Actions.Find(actModel => actModel.ObjectType == RelationalObject.ChildObject);
                if (action.ActionType != selectedActionType || (act != null && action.ActionType != act.ActionType))
                {
                    selectedActionType = action.ActionType;
                    if (expressionModel.Filters.Count > 0)
                    {
                        expressionModel.Filters[0].Filters.RemoveAll(sf => sf.ValueType != ExpressionValueTypes.Input);
                        bResetSearchFieldsUI = true;
                    }
                }
            }
            if (bResetSearchFieldsUI)
            {
                foreach (DataGridViewRow row in dgvFieldSelectionGrid.Rows)
                    row.Cells[ResultFields.Name].Value = row.Cells[SearchFields.Name].Value = false;
            }
        }

        private void ResizeFieldsGridColumnWidth()
        {
            dgvFieldSelectionGrid.Columns[Display.Name].Width = 45;
            dgvFieldSelectionGrid.Columns[DisplayOrder.Name].Width = 45;
            dgvFieldSelectionGrid.Columns[Save.Name].Width = 45;
            if (Model.AppType == QuickAppType.ParentChildApp)
            {
                dgvFieldSelectionGrid.Columns[SearchFields.Name].Width = 77;
                dgvFieldSelectionGrid.Columns[ResultFields.Name].Width = 77;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            using (host = new ExpressionBuilderHost(this))
                host.ShowDialog();
        }

        public void UpdateExpressionModel()
        {
            switch (PageNumber)
            {
                case PageIndex.ParentObjectRecordSelectionPage:
                    {
                        //ResetFilters();

                        expressionModel.RelationObject = RelationalObject.ParentObject;
                        expressionModel.Object = Model.Object;

                        SaveFilters();
                    }
                    break;

                case PageIndex.ChildObjectRecordSelectionPage:
                    {
                        //ResetFilters();

                        expressionModel.RelationObject = RelationalObject.ChildObject;
                        expressionModel.Object = Model.Object.Children[0];

                        SaveFilters();
                    }
                    break;
            }
        }

        private void chkDisplayFields_CheckedChanged(object sender, EventArgs e)
        {
            bool bChecked = chkDisplayFields.Checked;
            foreach (DataGridViewRow displayRow in dgvFieldSelectionGrid.Rows)
            {
                displayRow.Cells["Display"].Value = bChecked;
            }

            if (!bChecked) //If display fields are unchecked, uncheck all the save fields 
                chkSaveFields.Checked = false;

            chkSaveFields.Enabled = chkDisplayFields.Checked;
        }

        private void chkSaveFields_CheckedChanged(object sender, EventArgs e)
        {
            bool bChecked = chkSaveFields.Checked;
            System.Collections.IEnumerable rows;
            if (bChecked)
                rows = dgvFieldSelectionGrid.Rows.Cast<DataGridViewRow>().Where(row => row.Cells["Display"].Value.Equals(true));
            else
                rows = dgvFieldSelectionGrid.Rows;
            foreach (DataGridViewRow row in rows)
            {
                if (!row.Cells["Save"].ReadOnly)
                    row.Cells["Save"].Value = bChecked;
            }
        }

        private void dgvFieldSelectionGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            updateSaveCellAndOrderCell(e);

            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            if (dgvFieldSelectionGrid.Columns[e.ColumnIndex].Name.Equals("SearchFields"))
            {
                bool bAddOrRemoveFilter = (bool)dgvFieldSelectionGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
                if (bAddOrRemoveFilter)
                    AddSearchFilterForSearchableField(e.RowIndex);
                else
                    RemoveSearchFilterForSearchableField(e.RowIndex);
            }
        }

        private void cboActionSelection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ActionSelectionFilter action = cboActionSelection.SelectedItem as ActionSelectionFilter;
            if (selectedActionType != action.ActionType)
                ResetFiltersOnActionChange(action);
        }


    }
}
