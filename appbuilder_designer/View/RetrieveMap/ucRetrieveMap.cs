/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;
using System.Drawing;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ucRetrieveMap : UserControl
    {
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private RetrieveMapController controller;
        private BindingList<RetrieveField> listOfRetrieveFields;
        private HashSet<RetrieveField> checkedFields;
        private TreeNode selectedNode;
        private bool itemDragged;
        private BindingSource sortbyBindingSource;
        private BindingSource sortbyBindingSource2;
        private BindingSource groupbyBindingSource;
        private BindingSource groupbyBindingSource2;
        private BindingSource objectBindingSource;
        private BindingSource totalsBindingSource;
        private static string RETRIEVE_MAP_DISPLAY_MEMBER = "FieldName";
        private static string RETRIEVE_MAP_VALUE_MEMBER = "FieldId";

        List<CellLocation> lstCellLocation = new List<CellLocation>(); // this has to be initialized according to your formulas

        public ucRetrieveMap()
        {
            InitializeComponent();
            SetCultureData();
            listOfRetrieveFields = new BindingList<RetrieveField>();
            checkedFields = new HashSet<RetrieveField>();
            listOfRetrieveFields.AllowRemove = true;
        }

        private void SetCultureData()
        {
            btnAddSaveField.Text = resourceManager.GetResource("UCRETRIEVEMAP_btnAddSaveField_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnRemoveFields.Text = resourceManager.GetResource("COMMON_RemoveField_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            cboSortDirectionField1.Text = resourceManager.GetResource("COMMON_Ascending_Text");
            //version.Text = resourceManager.GetResource("UCRETRIEVEMAP_cboSortDirectionField1_Items1");
            cboSortDirectionField2.Text = resourceManager.GetResource("COMMON_Descending_Text");
            // version.Text = resourceManager.GetResource("UCRETRIEVEMAP_cboSortDirectionField2_Items1");
            dataGridViewImageColumn1.HeaderText = resourceManager.GetResource("UCRETRIEVEMAP_dataGridViewImageColumn1_HeaderText");
            FieldName.HeaderText = resourceManager.GetResource("COMMON_Name_Text").Replace(':', ' ');
            lblMappedDetails.Text = resourceManager.GetResource("UCRETRIEVEMAP_groupBox2_Text");
            groupBox4.Text = resourceManager.GetResource("UCRETRIEVEMAP_groupBox4_Text");
            label1.Text = resourceManager.GetResource("UCRETRIEVEMAP_label1_Text");
            label2.Text = resourceManager.GetResource("UCRETRIEVEMAP_label2_Text");
            label4.Text = resourceManager.GetResource("UCRETRIEVEMAP_label4_Text");
            lbLayout.Text = resourceManager.GetResource("COMMON_Layout_Text");
            lblGridName.Text = resourceManager.GetResource("UCRETRIEVEMAP_lblGridName_Text");
            lblGroupBy.Text = resourceManager.GetResource("UCRETRIEVEMAP_lblGroupBy_Text");
            lblHeading.Text = resourceManager.GetResource("COMMON_Header_Text") + " : ";
            lblLayout.Text = resourceManager.GetResource("COMMON_Layout_Text");
            lblName.Text = resourceManager.GetResource("COMMON_Name_Text");
            lblSortBy.Text = resourceManager.GetResource("UCRETRIEVEMAP_lblSortBy_Text");
            lblSpacing.Text = resourceManager.GetResource("UCRETRIEVEMAP_lblSpacing_Text");
            lblType.Text = resourceManager.GetResource("COMMON_Type_Text");
            rboHorizontal.Text = resourceManager.GetResource("UCRETRIEVEMAP_rboHorizontal_Text");
            rboVertical.Text = resourceManager.GetResource("UCRETRIEVEMAP_rboVertical_Text");
            rdbBottom.Text = resourceManager.GetResource("COMMON_Bottom_Text");
            rdbTop.Text = resourceManager.GetResource("COMMON_Top_Text");
            RetrievalType.HeaderText = resourceManager.GetResource("COMMON_Type_Text").Replace(':', ' ');
            tabPageMapSettings.Text = resourceManager.GetResource("COMMON_Fields_Text");
            tabPageProperties.Text = resourceManager.GetResource("COMMON_Options_Text");
            TargetLocation.HeaderText = resourceManager.GetResource("COMMON_Location_Text");
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            Globals.ThisAddIn.Application.SheetChange += Application_SheetChange;
            Globals.ThisAddIn.Application.SheetSelectionChange += Application_SheetSelectionChange;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Globals.ThisAddIn.Application.SheetChange -= Application_SheetChange;
            Globals.ThisAddIn.Application.SheetSelectionChange -= Application_SheetSelectionChange;
            DisposeBindingSource();
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Disposes the repeating cell binding source used by sort and group combobox.
        /// </summary>
        private void DisposeBindingSource()
        {
            RemoveComboboxChangeEvents();

            if (sortbyBindingSource != null)
                sortbyBindingSource.Dispose();
            if (sortbyBindingSource2 != null)
                sortbyBindingSource2.Dispose();
            if (groupbyBindingSource != null)
                groupbyBindingSource.Dispose();
            if (groupbyBindingSource2 != null)
                groupbyBindingSource2.Dispose();
        }

        /// <summary>
        /// Whenever we dispose the bindingsource in DisposeBindingSource() method, it triggers combobox selection change event, this will be suppressed, so that
        /// selection change events are not fired, as this method will be called when we close the taskpane, which is why calling those events don't make any sense.
        /// </summary>
        private void RemoveComboboxChangeEvents()
        {
            cboGroupBy.SelectedIndexChanged -= cboGroupBy_SelectedIndexChanged;
            cboSortby.SelectedIndexChanged -= cboSortby_SelectedIndexChanged;
        }


        private void AddComboboxChangeEvents()
        {
            cboSortby.SelectedIndexChanged += cboSortby_SelectedIndexChanged;
            cboGroupBy.SelectedIndexChanged += cboGroupBy_SelectedIndexChanged;
        }

        /// <summary>
        /// Fill Grid
        /// </summary>
        /// <param name="list"></param>
        internal void AddRow(RetrieveField rField)
        {
            if (rField != null)
            {
                listOfRetrieveFields.Add(rField);
                string temp = tbSearchBox.Text;
                tbSearchBox.Clear();
                dgvMappedFields.Rows[dgvMappedFields.RowCount - 1].Tag = rField;
                dgvMappedFields.Refresh();
                dgvMappedFields.ClearSelection();
                tbSearchBox.Text = temp;
            }
        }

        public void RefreshGrid()
        {
            dgvMappedFields.Refresh();
        }

        /// <summary>
        /// Mouse Move event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvObjectField_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedNode != null)
                {
                    // Prevent Parent Node of treeview
                    if (selectedNode.Level != 0 && selectedNode.FirstNode == null)
                    {
                        // For Handle Save Other field, Keep formulas in memory and reapply on cell
                        if (controller.mapMode == MapMode.SaveMap)
                            SetFormulasForSaveOther();

                        if (tvObjectField.DoDragDrop("[" + ((ApttusField)selectedNode.Tag).Name + "]", DragDropEffects.Copy) == DragDropEffects.Copy)
                            itemDragged = true;
                    }
                    else
                    {
                        this.tvObjectField.DoDragDrop(selectedNode.Text, DragDropEffects.None);
                    }
                }
            }
        }

        /// <summary>
        /// Set formulas for Save Others
        /// </summary>
        private void SetFormulasForSaveOther()
        {
            Excel.Worksheet worksheet = Globals.ThisAddIn.Application.ActiveSheet;
            Excel.Range range = worksheet.UsedRange;
            lstCellLocation.Clear();
            foreach (Excel.Range oRange in range)
            {
                if (oRange.HasFormula)
                {
                    CellLocation cellLocation = new CellLocation();
                    cellLocation.RowIndex = oRange.Row;
                    cellLocation.ColumnIndex = oRange.Column;
                    cellLocation.Formula = oRange.Formula;
                    lstCellLocation.Add(cellLocation);
                }
            }
        }

        /// <summary>
        /// Recalculate formulas 
        /// </summary>
        /// <param name="oRange"></param>
        private void ReCalculateFormulas(Excel.Range oRange)
        {
            if (lstCellLocation != null)
            {
                foreach (CellLocation celllocation in lstCellLocation)
                {
                    //if (Convert.ToString(oRange.Formula) == "[" + ((ApttusField)selectedNode.Tag).Name + "]" && celllocation.ColumnIndex == oRange.Column && celllocation.RowIndex == oRange.Row)
                    if (celllocation.ColumnIndex == oRange.Column && celllocation.RowIndex == oRange.Row)
                    {
                        oRange.Formula = celllocation.Formula;
                    }
                }
            }
        }

        /// <summary>
        /// Item Drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvObjectField_ItemDrag(object sender, ItemDragEventArgs e)
        {
            selectedNode = null;
            // Move the dragged node when the left mouse button is used. 
            if (e.Button == MouseButtons.Left)
                selectedNode = (TreeNode)(e.Item);
        }

        private void Application_SheetChange(object Sh, Excel.Range Target)
        {
            //Worksheet sheet =  Globals.ThisAddIn.Application.ActiveSheet;
            // Get Sheet Name
            Excel.Worksheet sheet = ExcelHelper.GetWorkSheet(Target.Worksheet.Name);
            bool blnResult;
            Excel.Range oRange = null;
            string targetAddress = string.Empty;
            if (itemDragged)
            {
                itemDragged = false;
                ObjectType cellType;
                Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out cellType);

                ApttusObject obj = GetRootObject(selectedNode.Parent); // selectedNode.Parent.Tag as ApttusObject;
                RepeatingGroup repGroup = controller.GetRepeatingGroupFromAppObjectId(obj.UniqueId);

                //Validate the cell whether we can add it or not.
                String secondLevelApttusObject = obj.UniqueId.ToString();

                if (repGroup != null)
                {
                    if (repGroup.Layout.Equals("Horizontal"))
                        rboHorizontal.Checked = true;
                    else
                        rboVertical.Checked = true;

                    controller.SetCellValidatorLayout(rboHorizontal.Checked ? RepeatingCellLayout.Horizontal : RepeatingCellLayout.Vertical);
                }

                blnResult = controller.ValidateCell(Target, cellType, secondLevelApttusObject, selectedNode);

                // If validation execute than no need to call following code
                if (!blnResult)
                {
                    // This code is for create label while drag the field
                    if (Target.Value2 != null)
                    {
                        targetAddress = Target.get_Address(true, true, Excel.XlReferenceStyle.xlA1, false, Type.Missing);
                        oRange = sheet.Range[targetAddress];

                        // If Dropdown is vertical and cell type is repeating
                        if (rboVertical.Checked && cellType == ObjectType.Repeating)
                        {
                            if (controller.mapMode == MapMode.SaveMap)
                            {
                                ReCalculateFormulas(oRange);
                                oRange = ExcelHelper.NextVerticalCell(oRange, -1);
                                string strTemp = Convert.ToString(Target.Value2);
                                string strFinal = ((ApttusField)selectedNode.Tag).Name;
                                oRange.Value2 = strFinal;
                            }
                            else
                            {
                                oRange = ExcelHelper.NextVerticalCell(oRange, -1);
                                string strTemp = Target.Value2;
                                string strFinal = strTemp.Replace("[", "").Replace("]", "");
                                oRange.Value2 = strFinal;
                            }

                        }
                        // If Dropdown is Horizontal and cell type is repeating
                        else if (rboHorizontal.Checked && cellType == ObjectType.Repeating && Target.Column != 1)
                        {
                            if (controller.mapMode == MapMode.SaveMap)
                            {
                                ReCalculateFormulas(oRange);
                                oRange = ExcelHelper.NextHorizontalCell(oRange, -1);
                                string strTemp = Convert.ToString(Target.Value2);
                                string strFinal = ((ApttusField)selectedNode.Tag).Name;
                                oRange.Value2 = strFinal;

                            }
                            else
                            {
                                oRange = ExcelHelper.NextHorizontalCell(oRange, -1);
                                string strTemp = Target.Value2;
                                string strFinal = strTemp.Replace("[", "").Replace("]", "");
                                oRange.Value2 = strFinal;
                            }
                        }
                        // For independent cell    
                        else if (Target.Column != 1)
                        {
                            // ReApply formula for save other map
                            if (controller.mapMode == MapMode.SaveMap)
                            {
                                ReCalculateFormulas(oRange);
                                oRange = ExcelHelper.NextHorizontalCell(oRange, -1);
                                string strTemp = Convert.ToString(Target.Value2);
                                string strFinal = ((ApttusField)selectedNode.Tag).Name;
                                oRange.Value2 = strFinal;
                            }
                            else
                            {
                                oRange = ExcelHelper.NextHorizontalCell(oRange, -1);
                                string strTemp = Target.Value2;
                                string strFinal = strTemp.Replace("[", "").Replace("]", "");
                                oRange.Value2 = strFinal;
                            }
                        }
                    }
                }

                repGroup = controller.GetRepeatingGroupFromAppObjectId(obj.UniqueId);
                if (repGroup != null)
                    cboObject.SelectedItem = repGroup;
            }
            else
            {
                //If a new column is added or deleted, rearrange fields location.                
                if (Target.Cells.Count % Constants.EXCEL_NUMBEROFROWS == 0)
                    ReArrangeFieldsLocation(Sh);
            }
        }

        private void ReArrangeFieldsLocation(object workSheet)
        {
            if (controller.RetrieveMap == null)
                return;
            if (controller.RetrieveMap.RepeatingGroups == null)
                return;

            Excel.Worksheet sheet = workSheet as Excel.Worksheet; //Worksheet in which the sheet change event took place.

            foreach (RepeatingGroup rg in controller.RetrieveMap.RepeatingGroups)
            {
                //Check whether the repeating group belongs to the worksheet in which new column is inserted.
                Excel.Worksheet repeatingGroupSheet = ExcelHelper.GetRange(rg.TargetNamedRange).Worksheet as Excel.Worksheet;
                if (repeatingGroupSheet != sheet)
                    continue;

                foreach (RetrieveField rf in rg.RetrieveFields)
                {
                    int currentColumnIndex = ExcelHelper.GetColumnIndex(rg.TargetNamedRange, rf.TargetNamedRange);
                    if (currentColumnIndex >= 0 && currentColumnIndex != rf.TargetColumnIndex)
                    {
                        rf.TargetColumnIndex = currentColumnIndex;
                        rf.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(rf.TargetNamedRange));
                    }
                }
            }
            dgvMappedFields.Refresh(); //Refresh the grid, so that the location column shows updated values
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public ApttusObject GetRootObject(TreeNode treeNode)
        {
            ApttusObject retVal = null;

            if (treeNode.Parent == null)
                return treeNode.Tag as ApttusObject;
            else
                retVal = GetRootObject(treeNode.Parent);

            return retVal;
        }

        /// <summary>
        /// Fill Type DropDown
        /// </summary>
        private void FillTypeDropDown()
        {
            try
            {
                Dictionary<string, string> listDesc = new Dictionary<string, string>();
                listDesc = Utils.GetObjectTypeDescription();
                cmbType.DisplayMember = "Value";
                cmbType.ValueMember = "Key";

                // IF save other and retrieve map than remove cross tab key from combo box
                listDesc.Remove("CrossTab");
                cmbType.DataSource = new BindingSource(listDesc, null);
                // IF save other and retrieve map than remove cross tab key from combo box
                ////if (controller.mapMode == MapMode.SaveMap && controller.mapMode == MapMode.RetrieveMap && listDesc.Remove("CrossTab"))
                ////    cmbType.DataSource = new BindingSource(listDesc, null);
                ////else
                ////    cmbType.DataSource = new BindingSource(listDesc, null);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_DisplayMapError_ErrMsg") + ex.ToString(), resourceManager.GetResource("COMMON_DisplayMap_Text"));
            }
        }

        /// <summary>
        /// Bind TreeView
        /// Added optional filter parameter for searching
        /// </summary>
        private void BindTreeView(ObjectType cellTypeForRepeating, string filter = "")
        {
            int recursionCounter = 0;

            tvObjectField.Nodes.Clear();
            try
            {
                List<ApttusObject> FilteredObjects = controller.GetAppObjects().Where(obj => obj.ObjectType == cellTypeForRepeating).ToList();

                foreach (ApttusObject FilteredObject in FilteredObjects)
                {
                    TreeNode rootNode = new TreeNode(FilteredObject.Name);
                    rootNode.Tag = FilteredObject;
                    List<ApttusField> sortedFields = FilteredObject.Fields.OrderBy(field => Convert.ToString(field.Name)).ToList();

                    // "filter" is the text searched in Search box
                    // Below condition will filter out fields according to filter if present
                    if (!string.IsNullOrEmpty(filter))
                    {
                        //Look up fields may contain field which is matching filter
                        sortedFields = FilteredObject.Fields.OrderBy(field => Convert.ToString(field.Name)).Where(t => t.Name.ToLower().Contains(filter.ToLower()) || t.Datatype == Datatype.Lookup).ToList();

                        if (sortedFields.Count == 0 && !FilteredObject.Name.ToLower().Contains(filter)) continue;
                        else if (sortedFields.Count == 0 && FilteredObject.Name.ToLower().Contains(filter))
                        {
                            sortedFields = FilteredObject.Fields.OrderBy(field => Convert.ToString(field.Name)).ToList();
                        }
                    }

                    foreach (ApttusField field in sortedFields)
                    {
                        //// Remove ID field when map is opened in Save Map Mode.
                        //if (controller.mapMode == MapMode.SaveMap & field.Id == Apttus.XAuthor.Core.Constants.ID_ATTRIBUTE)
                        //    continue;

                        TreeNode fieldNode = null;
                        bool expandedLookUp = false;
                        if (FilteredObject.Parent != null)
                        {
                            // For Repeating
                            if (FilteredObject.ObjectType == ObjectType.Repeating)
                            {
                                fieldNode = new TreeNode(field.Name);
                                fieldNode.Tag = field;

                                if (controller.mapMode.Equals(MapMode.RetrieveMap) && field.Datatype.Equals(Datatype.Lookup))
                                {
                                    ApttusObject lookUpObject = applicationDefinitionManager.GetFullHierarchyObjects(null).Where(o => o.Id.Equals(field.LookupObject.Id)).FirstOrDefault();
                                    if (lookUpObject != null)
                                    {
                                        // If filter is not empty, look up is not null,
                                        // and lookup name it self does not match the filter (Search string) then it will continue to next field
                                        if (lookUpObject.Fields.Count(t => t.Name.ToLower().Contains(filter.ToLower()) || t.Datatype.Equals(Datatype.Lookup)) == 0 && !string.IsNullOrEmpty(filter) && !field.Name.ToLower().Contains(filter.ToLower()))
                                        {
                                            continue;
                                        }

                                        bool bChildObject = IsLookUpObjectChildOfCurrentObject(lookUpObject, FilteredObject);
                                        if (!bChildObject)
                                        {
                                            fieldNode.Tag = lookUpObject;
                                            PopulateLookUpFields(fieldNode, lookUpObject, field.Id, recursionCounter, filter);
                                            expandedLookUp = true;
                                            if (!string.IsNullOrEmpty(filter))
                                                fieldNode.Expand();
                                            else
                                                fieldNode.Collapse();
                                        }
                                    }
                                }
                            }
                            // For Independent
                            else if (FilteredObject.ObjectType == ObjectType.Independent)
                            {
                                //fieldNode = new TreeNode(FilteredObject.Parent.Name + Core.Constants.DOT + field.Name);
                                fieldNode = new TreeNode(field.Name);
                                fieldNode.Tag = field;
                            }
                            // For Cross tab
                            else if (FilteredObject.ObjectType == ObjectType.CrossTab)
                            {
                                // Put Code here for cross tab
                            }
                        }
                        else
                        {
                            fieldNode = new TreeNode(field.Name);
                            fieldNode.Tag = field;
                            // Display map enhancement 2 , even if object are not hierachy expand tree.
                            if ((FilteredObject.ObjectType == ObjectType.Repeating) &&
                                (controller.mapMode.Equals(MapMode.RetrieveMap) && field.Datatype.Equals(Datatype.Lookup)))
                            {
                                ApttusObject lookUpObject = applicationDefinitionManager.GetFullHierarchyObjects(null).Where(o => o.Id.Equals(field.LookupObject.Id)).FirstOrDefault();
                                if (lookUpObject != null)
                                {
                                    if (lookUpObject.Fields.Count(t => t.Name.ToLower().Contains(filter.ToLower()) || t.Datatype.Equals(Datatype.Lookup)) == 0 && !string.IsNullOrEmpty(filter) && !field.Name.ToLower().Contains(filter.ToLower()))
                                    {
                                        continue;
                                    }
                                    fieldNode.Tag = lookUpObject;
                                    PopulateLookUpFields(fieldNode, lookUpObject, field.Id, recursionCounter, filter);
                                    expandedLookUp = true;
                                    if (!string.IsNullOrEmpty(filter))
                                        fieldNode.Expand();
                                    else
                                        fieldNode.Collapse();
                                }
                            }
                        }
                        //fieldNode.Tag = field;
                        if (field.Datatype.Equals(Datatype.Lookup) && fieldNode.Nodes.Count == 0 && expandedLookUp) continue;
                        rootNode.Nodes.Add(fieldNode);
                        if (!string.IsNullOrEmpty(filter))
                            rootNode.Expand();
                        else
                            rootNode.Collapse();
                    }
                    if (rootNode.Nodes.Count != 0)
                        tvObjectField.Nodes.Add(rootNode);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_DisplayMapError_ErrMsg") + ex.ToString(), resourceManager.GetResource("COMMON_DisplayMap_Text"));
            }
        }

        private bool IsLookUpObjectChildOfCurrentObject(ApttusObject lookUpObject, ApttusObject currentObject)
        {
            bool bReturn = false;
            if (currentObject.Children != null && currentObject.Children.Count > 0)
            {
                foreach (ApttusObject childObject in currentObject.Children)
                {
                    if (childObject.Id.Equals(lookUpObject.Id))
                    {
                        bReturn = true;
                        break;
                    }
                }
            }
            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="lookUpObject"></param>
        private void PopulateLookUpFields(TreeNode rootNode, ApttusObject lookUpObject, string lookUpFieldId, int recursionCounter, string filter = "")
        {
            TreeNode fieldNode = null;
            // format root node, add Actual Object Name and Forecolor
            if (lookUpObject.Fields.Count > 0)
            {
                rootNode.Name = rootNode.Text;
                rootNode.Text = rootNode.Text + Constants.OPEN_BRACKET + lookUpObject.Name + Constants.CLOSE_BRACKET;
                rootNode.ForeColor = Color.Orange;
            }

            List<ApttusField> fields = lookUpObject.Fields;

            // filter is passed from BindTreeView() and again check will be made to look up fields
            if (!string.IsNullOrEmpty(filter))
            {
                fields = lookUpObject.Fields.Where(t => t.Name.ToLower().Contains(filter.ToLower()) || t.Datatype.Equals(Datatype.Lookup)).ToList();
                //This condition will make sure that if filter does not match with any of the fields in look up,
                // but the rootNode.Text (which is name of look up) it self has the match with the filter then it will show all of the fields
                if ((fields.Count(t => t.Datatype.Equals(Datatype.Lookup)) == fields.Count || fields.Count == 0) && rootNode.Text.ToLower().Contains(filter.ToLower()))
                    fields = lookUpObject.Fields;
            }

            foreach (ApttusField field in fields)
            {
                //In case of multi-level hierarchy, lookup name field is already available as part of parent object's tree hierarchy.
                if (field.Id.Equals(lookUpObject.NameAttribute))
                    continue;

                fieldNode = new TreeNode(field.Name);
                fieldNode.Tag = field;
                fieldNode.Name = field.Id.Equals(lookUpObject.IdAttribute) ? lookUpFieldId : string.Empty;

                rootNode.Nodes.Add(fieldNode);

                if (field.Datatype.Equals(Datatype.Lookup))
                {
                    ApttusObject childLookUpObject = applicationDefinitionManager.GetFullHierarchyObjects(null).Where(o => o.Id.Equals(field.LookupObject.Id)).FirstOrDefault();

                    // In case self join, i.e. Lookup field on Self loop may run into infinite loop
                    // for self join or lookup field on self, restrict Tree to 1 level down only
                    if (childLookUpObject != null && (lookUpObject.Id != childLookUpObject.Id))
                    {
                        bool bChildObject = IsLookUpObjectChildOfCurrentObject(childLookUpObject, lookUpObject);
                        if (!bChildObject && recursionCounter < Constants.HIERARCHY_RECURSION_COUNTER)
                        {
                            if (childLookUpObject.Fields.Count(t => t.Name.ToLower().Contains(filter.ToLower()) || t.Datatype.Equals(Datatype.Lookup)) == 0 && !string.IsNullOrEmpty(filter) && !field.Name.ToLower().Contains(filter.ToLower()) && !childLookUpObject.Name.ToLower().Contains(filter.ToLower()))
                            {
                                //In case of cout 0 of fields of childLookUps and filter does not match then empty field node will not server the purpose
                                rootNode.Nodes.Remove(fieldNode);
                                continue;
                            }
                            fieldNode.Tag = childLookUpObject;
                            PopulateLookUpFields(fieldNode, childLookUpObject, field.Id, ++recursionCounter, filter);

                            //Again empty field node is removed
                            if (field.Datatype.Equals(Datatype.Lookup) && fieldNode.Nodes.Count == 0)
                                rootNode.Nodes.Remove(fieldNode);

                            if (!string.IsNullOrEmpty(filter))
                                fieldNode.Expand();
                            else
                                fieldNode.Collapse();
                        }
                    }
                    else if (!field.Name.ToLower().Contains(filter.ToLower()))
                        rootNode.Nodes.Remove(fieldNode);
                }
            }
        }

        public object MapObject {
            get;
            set;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObjectType value;
            Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out value);
            //if (value == ObjectType.CrossTab)
            //{
            //    TaskPaneHelper.RemoveCustomPane("Display Map");
            //    CrossTabRetrieveUI CrossTabRetUI = new CrossTabRetrieveUI();
            //    CrossTabRetrievalController controller = new CrossTabRetrievalController(CrossTabRetUI);
            //    // switch between inde and xtab need this info so that from xtab to independent, 
            //    // initialize method could use the RM 
            //    controller.MapObject = this.controller.RetrieveMap;
            //    RetrieveMap map = this.controller.RetrieveMap;
            //    if (map != null)
            //    {
            //        if (map.CrossTabMaps != null) //if (MapObject.GetType().Equals(typeof(CrossTabRetrieveMap)))
            //            controller.initialize(map);
            //        else
            //            controller.initialize(null);

            //    }

            //    return;
            //}


            UpdateTabPage();
            //  ObjectType value;
            Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out value);
            BindTreeView(value);
            //tvObjectField.ExpandAll();
        }

        /// <summary>
        /// Remove Fields 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Excel.Range oFieldRange = null;
            Excel.Range oLabelRange = null;

            String msg = CanRemoveFields();

            if (!String.IsNullOrEmpty(msg))
            {
                msg = new StringBuilder(msg).Append(" Are you sure you want to remove the fields ?").ToString();
                if (MessageBox.Show(msg, "Remove Fields", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            for (int i = dgvMappedFields.Rows.Count - 1; i >= 0; i--)
            {
                if ((bool)dgvMappedFields.Rows[i].Cells["chkMapped"].FormattedValue)
                {
                    RetrieveField rField = dgvMappedFields.Rows[i].DataBoundItem as RetrieveField;
                    if (!string.IsNullOrEmpty(rField.TargetNamedRange))
                    {
                        RemoveFieldFromSaveMap(rField);

                        oFieldRange = ExcelHelper.GetRange(rField.TargetNamedRange);

                        RepeatingGroup repGroup = null;
                        if (rField != null && rField.Type == ObjectType.Repeating)
                            repGroup = controller.GetRepeatingGroupByRetrieveField(rField);

                        // if field range is null, field has been deleted from Excel, remove from config only.
                        if (oFieldRange == null)
                        {
                            controller.RemoveField(repGroup, rField);
                            dgvMappedFields.Rows.RemoveAt(i);
                        }
                        // For Vertical
                        else if (rField.Type == ObjectType.Repeating && repGroup != null && repGroup.Layout.Equals("Vertical") && oFieldRange != null)
                        {
                            oLabelRange = ExcelHelper.NextVerticalCell(oFieldRange, -1);
                            oFieldRange.ClearContents();
                            controller.RemoveField(repGroup, rField);
                            oLabelRange.ClearContents();
                            dgvMappedFields.Rows.RemoveAt(i);
                        }
                        // For Horizontal 
                        else if (rField.Type == ObjectType.Repeating && repGroup != null && repGroup.Layout.Equals("Horizontal") && oFieldRange != null)
                        {
                            oLabelRange = ExcelHelper.NextHorizontalCell(oFieldRange, -1);
                            oFieldRange.ClearContents();
                            controller.RemoveField(repGroup, rField);
                            oLabelRange.ClearContents();
                            dgvMappedFields.Rows.RemoveAt(i);
                        }
                        // For Independent
                        else if (rField.Type == ObjectType.Independent && oFieldRange != null)
                        {
                            oLabelRange = ExcelHelper.NextHorizontalCell(oFieldRange, -1);
                            oFieldRange.ClearContents();
                            controller.RemoveField(null, rField);
                            oLabelRange.ClearContents();
                            dgvMappedFields.Rows.RemoveAt(i);
                        }
                        if (rField.Type == ObjectType.Repeating)
                            RemoveFieldFromBindingList(rField);
                        if (controller.mapMode == MapMode.SaveMap)
                            ReCalculateFormulas(oFieldRange);

                        listOfRetrieveFields.Remove(rField);
                    }
                    else
                        dgvMappedFields.Rows[i].Cells["chkMapped"].Value = false;
                }
            }
            UpdateAllSaveMaps();
        }

        /// <summary>
        /// Updates the Properties Tabpage once the cell type gets changed.
        /// </summary>
        private void UpdateTabPage()
        {
            int nTabPages = tabControl.TabPages.Count;
            if (cmbType.SelectedIndex == 0)
            {
                if (nTabPages == 2)
                    tabControl.TabPages.Remove(tabPageProperties);
            }
            else
            {
                // Show Properties tab only if in RetrieveMap mode
                if (nTabPages == 1 && controller.mapMode.Equals(MapMode.RetrieveMap))
                    tabControl.TabPages.Add(tabPageProperties);
            }
        }
        /// <summary>
        /// Pass method to Controller
        /// </summary>
        /// <param name="targetlocation"></param>
        public void AddField(Excel.Range Target, TreeNode selectedNode)
        {
            string targetLocation = ExcelHelper.GetAddress(Target);
            ObjectType value;
            Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out value);

            //Create a name range for this cell.
            string namedRange = String.Empty;
            ApttusField apttusField = (ApttusField)selectedNode.Tag;
            ApttusObject apttusObject = GetRootObject(selectedNode.Parent);  //((ApttusObject)selectedNode.Parent.Tag);

            if (value == ObjectType.Independent)
            {
                namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, namedRange);
                controller.AddField(apttusObject, apttusField, targetLocation, value, namedRange, selectedNode);
            }
            else
            {
                // AB-296
                namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, namedRange);
                ApttusObject obj = GetRootObject(selectedNode.Parent);  //((ApttusObject)(selectedNode.Parent.Tag));
                controller.AddField(apttusObject, apttusField, targetLocation, value, namedRange, selectedNode);
                UpdateBindings(obj.UniqueId);
            }
        }

        public void UpdateField(Excel.Range Target)
        {
            string targetLocation = ExcelHelper.GetAddress(Target);
            // AB-296
            string targetNamedRange = ExcelHelper.GetExcelCellName(Target);

            ObjectType value;
            Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out value);

            bool bRetrieveFieldUpdated = controller.UpdateField(selectedNode, targetLocation, value, targetNamedRange);

            if (!bRetrieveFieldUpdated)
                Globals.ThisAddIn.Application.Undo();

            if (value == ObjectType.Repeating && bRetrieveFieldUpdated)
            {
                ApttusObject obj = GetRootObject(selectedNode.Parent); //((ApttusObject)(selectedNode.Parent.Tag));
                UpdateBindings(obj.UniqueId);
                dgvMappedFields.Refresh();
            }
        }
        /// <summary>
        /// Save Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!HasValidateErrors())
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                tabControl.SelectedTab = tabPageProperties;
                tabControl.SelectedTab = tabPageMapSettings;
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                bool bSuccess = true;
                //Save Properties of each repeating group and create named range for each of them.
                foreach (RepeatingGroup repeatingCellGroup in cboObject.Items)
                {
                    bSuccess = SaveRepeatingGroupProperties(repeatingCellGroup, repeatingCellGroup == cboObject.SelectedItem as RepeatingGroup);
                    if (!bSuccess)
                        break;
                }
                if (bSuccess)
                {
                    RefreshLookAheadIcons();
                    //Save the Retrieve Map. This will also save the 
                    controller.Save(txtName.Text);
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RETRIEVEMAP_Success_ShowMsg"), resourceManager.GetResource("RETRIEVEMAP_SuccessCap_ShowMsg"));
                }
                dgvMappedFields.Refresh();
            }
        }

        private void btnAddSaveField_Click(object sender, EventArgs e)
        {
            // Supress Name mandatory field validator in case of Add Save fields.
            //if (!HasValidateErrors())
            //{
            foreach (RepeatingGroup repGroup in controller.RetrieveMap.RepeatingGroups)
                controller.ApplyNameRangeToRepeatingCells(MapMode.SaveMap, repGroup);
            controller.updateSaveMap();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool HasValidateErrors()
        {
            return ApttusFormValidator.hasValidationErrors(this.Controls);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool SaveRepeatingGroupProperties(RepeatingGroup repeatingCellGroup, bool bIsSelectedGroup)
        {
            bool bSuccess = true;
            if (bIsSelectedGroup && repeatingCellGroup != null)
            {
                string groupByFieldID;
                bSuccess = IsGroupByFieldPartOfSaveMap(repeatingCellGroup, cboGroupBy, out groupByFieldID);

                string groupByFieldID2 = string.Empty;
                if (!String.IsNullOrEmpty(Convert.ToString(cboGroupByField2.SelectedValue)))
                {
                    bool bOK = IsGroupByFieldPartOfSaveMap(repeatingCellGroup, cboGroupByField2, out groupByFieldID2);
                    bSuccess = bSuccess && bOK;
                }
                if (bSuccess)
                {
                    repeatingCellGroup.GridHeader = txtGridHeader.Text;
                    repeatingCellGroup.Layout = txtLayout.Text;
                    repeatingCellGroup.SortByField = Convert.ToString(cboSortby.SelectedValue);
                    repeatingCellGroup.SortByField2 = Convert.ToString(cboSortByField2.SelectedValue);
                    repeatingCellGroup.GroupByField = groupByFieldID;
                    repeatingCellGroup.GroupByField2 = groupByFieldID2;

                    if (cboSortDirectionField1.SelectedIndex != -1)
                        repeatingCellGroup.SortDirectionField1 = cboSortDirectionField1.SelectedIndex == 0 ? RepeatingGroupSortDirection.Ascending : RepeatingGroupSortDirection.Descending;
                    if (cboSortDirectionField2.SelectedIndex != -1)
                        repeatingCellGroup.SortDirectionField2 = cboSortDirectionField2.SelectedIndex == 0 ? RepeatingGroupSortDirection.Ascending : RepeatingGroupSortDirection.Descending;

                    repeatingCellGroup.GroupSpacing = 1;
                    repeatingCellGroup.GroupingLayout = Core.GroupingLayout.Top;

                    repeatingCellGroup.TotalFields.Clear();

                    if (lstTotalTields.Enabled)
                    {
                        foreach (object item in lstTotalTields.CheckedItems)
                        {
                            RetrieveField field = item as RetrieveField;
                            repeatingCellGroup.TotalFields.Add(field.FieldId);
                        }
                    }
                }
            }

            // Apply the Named Range for the repeating group.
            if (bSuccess)
                controller.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, repeatingCellGroup);

            return bSuccess;
        }

        /// <summary>
        /// Tab Control for selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
        }

        /// <summary>
        /// Load User control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucRetrieveMap_Load(object sender, EventArgs e)
        {
            dgvMappedFields.AutoGenerateColumns = false;
            dgvMappedFields.DataSource = listOfRetrieveFields;
            btnRemoveFields.Enabled = false;

            Rectangle rect = dgvMappedFields.GetCellDisplayRectangle(0, -1, true);
            // set checkbox header to center of header cell. +1 pixel to position correctly.
            rect.X = rect.Location.X + (rect.Width / 4) - 1;
            rect.Y = 3;
            CheckBox cboHeader = new CheckBox();
            cboHeader.Name = "cboHeader";
            cboHeader.BackColor = Color.White;
            cboHeader.Size = new Size(15, 15);
            cboHeader.Location = rect.Location;
            cboHeader.FlatStyle = FlatStyle.Flat;
            cboHeader.CheckedChanged += new EventHandler(cboHeader_CheckedChanged);

            dgvMappedFields.Controls.Add(cboHeader);

            PopulateSpacingValues();

            rboVertical.Checked = true;

            //Set the focus on Name textbox.
            System.Action action = () => txtName.Focus();
            this.BeginInvoke(action);

            //Disable the SortField2 and GroupbyField2 combobox
            cboGroupByField2.Enabled = cboSortByField2.Enabled = false;
        }

        private void cboHeader_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvMappedFields.RowCount; i++)
            {
                dgvMappedFields[0, i].Value = ((CheckBox)dgvMappedFields.Controls.Find("cboHeader", true)[0]).Checked;
            }
            dgvMappedFields.EndEdit();
        }

        /// <summary>
        /// Assign controller to the View
        /// </summary>
        /// <param name="RetrieveMapController"></param>
        public void SetController(RetrieveMapController retrieveMapController)
        {
            controller = retrieveMapController;
        }

        /// <summary>
        /// Load Controls
        /// </summary>
        /// <param name="formOpenMode"></param>
        /// <param name="map"></param>
        public void LoadControls(FormOpenMode formOpenMode, RetrieveMap map, MapMode mapMode, string saveMapName = "", ObjectType? objectTypeVal = null)
        {
            FillTypeDropDown();

            switch (formOpenMode)
            {
                case FormOpenMode.Create:
                    break;
                case FormOpenMode.Edit:
                    LoadRetrieveMap(map);
                    break;
                case FormOpenMode.ReadOnly:
                    break;
            }

            // Action button visibility
            switch (mapMode)
            {
                case MapMode.RetrieveMap:
                    btnSave.Visible = true;
                    btnAddSaveField.Visible = false;
                    btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");//"Close";
                    break;
                case MapMode.SaveMap:
                    btnSave.Visible = false;
                    btnAddSaveField.Visible = true;
                    btnClose.Text = resourceManager.GetResource("QAWIZARD_btnBack_Text");// "Back";
                    txtName.Text = saveMapName;
                    break;
            }

            if (objectTypeVal == ObjectType.Independent)
                cmbType.SelectedIndex = 0;
            //else if (map.RepeatingGroups != null && map.RepeatingGroups.Count() > 0)
            else if (objectTypeVal == ObjectType.Repeating)
                cmbType.SelectedIndex = 1;
            //else if (map.CrossTabMaps != null && map.CrossTabMaps.Count() > 0)
            else if (objectTypeVal == ObjectType.CrossTab)
                cmbType.SelectedIndex = 2;
            //else if (map.RetrieveFields != null && map.RetrieveFields.Count() > 0)
            //cmbType.SelectedIndex = 0;
            else if (objectTypeVal == null && map.RepeatingGroups != null && map.RepeatingGroups.Count() > 0)
                cmbType.SelectedIndex = 1;
            //else if (objectTypeVal == null && map.CrossTabMaps != null && map.CrossTabMaps.Count() > 0)
            //    cmbType.SelectedIndex = 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        private void LoadRetrieveMap(RetrieveMap map)
        {
            txtName.Text = map.Name;
            int i = 0;
            //Independent Fields
            foreach (RetrieveField field in map.RetrieveFields)
            {
                listOfRetrieveFields.Add(field);
                dgvMappedFields.Rows[i].Tag = field;
                ++i;
            }

            //Repeating Fields
            foreach (RepeatingGroup repGroup in map.RepeatingGroups)
            {
                foreach (RetrieveField field in repGroup.RetrieveFields)
                {
                    listOfRetrieveFields.Add(field);
                    dgvMappedFields.Rows[i].Tag = field;
                    ++i;
                }
            }
            RefreshLookAheadIcons();
        }

        private void RefreshLookAheadIcons()
        {
            foreach (DataGridViewRow row in dgvMappedFields.Rows)
            {
                if (row.DataBoundItem is RetrieveField rField && rField.FieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                {
                    if (rField.LookAheadProps != null && rField.LookAheadProps.Count > 0 || rField.LookAheadProp != null)
                        row.Cells[4].Value = Properties.Resources.LA_Filled;
                    else
                        row.Cells[4].Value = Properties.Resources.LA_Empty;
                }
            }
            dgvMappedFields.Refresh();
        }

        /// <summary>
        /// Sheet Selection Change Event
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        void Application_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            string targetAddress = ExcelHelper.GetAddress(Target);
            foreach (DataGridViewRow dr in dgvMappedFields.Rows)
            {
                if (Convert.ToString(dr.Cells["TargetLocation"].Value) == targetAddress)
                {
                    dr.Selected = true;
                    dgvMappedFields.FirstDisplayedScrollingRowIndex = dr.Index;
                }
                else
                    dr.Selected = false;
            }
        }

        private void dgvMappedFields_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateRemoveFieldsButton();
        }

        private void dgvMappedFields_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateRemoveFieldsButton();
        }

        private void UpdateRemoveFieldsButton()
        {
            btnRemoveFields.Enabled = dgvMappedFields.RowCount != 0 ? true : false;
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtName, resourceManager.GetResource("COMMON_NameCannotBeEmpty_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(txtName, String.Empty);
            }
        }

        private void tabPageProperties_Layout(object sender, LayoutEventArgs e)
        {
            if (cboObject.IsHandleCreated && objectBindingSource == null)
                FillDraggedSFObjects();

            if (cboSortby.IsHandleCreated && sortbyBindingSource == null)
                ApplySortByComboDataSource();

            if (cboGroupBy.IsHandleCreated && groupbyBindingSource == null)
            {
                //cboGroupBy.SelectedIndexChanged -= cboGroupBy_SelectedIndexChanged;
                ApplyGroupByDataSource();
                //cboGroupBy.SelectedIndexChanged += cboGroupBy_SelectedIndexChanged;

                //Used to update the controls in edit mode only.
                RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
                if (repGroup != null)
                    UpdateControls(repGroup);
            }
        }

        private void FillDraggedSFObjects()
        {
            objectBindingSource = new BindingSource();
            RefreshSFObjectDataSource();

            cboObject.DataSource = objectBindingSource;
        }

        private void ApplySortByComboDataSource()
        {
            sortbyBindingSource = new BindingSource();
            RepeatingGroup obj = cboObject.SelectedItem as RepeatingGroup;
            if (obj == null)
                return;

            RefreshSortByDataSource(obj.AppObject);
        }

        private bool IsRetrieveFieldSortByField(RetrieveField rField, bool bFirstField)
        {
            RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
            if (repGroup == null)
                return false;

            RetrieveField field = (bFirstField ? cboSortby.SelectedItem : cboSortByField2.SelectedItem) as RetrieveField;
            if (field != null)
                return rField.FieldId.Equals(field.FieldId) && rField.AppObject.Equals(field.AppObject) && rField.DataType == field.DataType;

            return false;
        }

        private bool IsRetrieveFieldGroupByField(RetrieveField rField, bool bFirstField)
        {
            RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
            if (repGroup == null)
                return false;

            RetrieveField field = (bFirstField ? cboGroupBy.SelectedItem : cboGroupByField2.SelectedItem) as RetrieveField;
            if (field != null)
                return rField.FieldId.Equals(field.FieldId) && rField.AppObject.Equals(field.AppObject) && rField.DataType == field.DataType;

            return false;
        }

        private void RemoveFieldFromBindingList(RetrieveField rField)
        {
            try
            {
                RemoveComboboxChangeEvents();
                if (sortbyBindingSource != null)
                {
                    List<RetrieveField> sortByList = sortbyBindingSource.DataSource as List<RetrieveField>;
                    RetrieveField rf = cboSortby.SelectedItem as RetrieveField;
                    bool bIsRetrieveFieldSortByField = IsRetrieveFieldSortByField(rField, true);

                    if (sortByList != null)
                    {
                        sortByList.Remove(rField);
                        sortbyBindingSource.ResetBindings(false);
                    }
                    if (bIsRetrieveFieldSortByField)
                    {
                        cboSortby.SelectedIndex = -1;
                        cboSortByField2.SelectedIndex = -1;
                        lstTotalTields.Enabled = cboSortByField2.Enabled = false;
                    }
                    else
                        cboSortby.SelectedItem = rf;
                }

                if (groupbyBindingSource != null)
                {
                    List<RetrieveField> groupByList = groupbyBindingSource.DataSource as List<RetrieveField>;

                    RetrieveField rf = cboGroupBy.SelectedItem as RetrieveField;
                    bool bIsRetrieveFieldGroupByField = IsRetrieveFieldGroupByField(rField, true);

                    if (groupByList != null)
                    {
                        groupByList.Remove(rField);
                        groupbyBindingSource.ResetBindings(false);
                    }

                    if (bIsRetrieveFieldGroupByField)
                    {
                        cboGroupBy.SelectedIndex = -1;
                        cboGroupByField2.SelectedIndex = -1;
                        cboGroupByField2.Enabled = false;
                    }
                    else
                        cboGroupBy.SelectedItem = rf;
                }

                if (sortbyBindingSource2 != null)
                {
                    List<RetrieveField> sortByList = sortbyBindingSource2.DataSource as List<RetrieveField>;
                    RetrieveField rf = cboSortByField2.SelectedItem as RetrieveField;
                    bool bIsRetrieveFieldSortByField = IsRetrieveFieldSortByField(rField, false);

                    if (sortByList != null)
                    {
                        sortByList.Remove(rField);
                        sortbyBindingSource2.ResetBindings(false);
                    }
                    if (bIsRetrieveFieldSortByField)
                        cboSortByField2.SelectedIndex = -1;
                    else
                        cboSortByField2.SelectedItem = rf;
                }

                if (groupbyBindingSource2 != null)
                {
                    List<RetrieveField> groupByList = groupbyBindingSource2.DataSource as List<RetrieveField>;
                    RetrieveField rf = cboGroupByField2.SelectedItem as RetrieveField;
                    bool bIsRetrieveFieldGroupByField = IsRetrieveFieldGroupByField(rField, false);

                    if (groupByList != null)
                    {
                        groupByList.Remove(rField);
                        groupbyBindingSource2.ResetBindings(false);
                    }
                    if (bIsRetrieveFieldGroupByField)
                        cboGroupByField2.SelectedIndex = -1;
                    else
                        cboGroupByField2.SelectedItem = rf;
                }

                if (totalsBindingSource != null)
                {
                    List<RetrieveField> totalFields = totalsBindingSource.DataSource as List<RetrieveField>;
                    if (totalFields != null)
                        totalFields.RemoveAll(rf => rf.FieldId.Equals(rField.FieldId));
                    totalsBindingSource.ResetBindings(false);
                }
            }
            finally
            {
                AddComboboxChangeEvents();
            }
        }

        private void RefreshSortByDataSource(Guid AppObjectId)
        {
            if (sortbyBindingSource != null)
            {
                sortbyBindingSource.Dispose();
                sortbyBindingSource = new BindingSource();
            }
            cboSortby.DisplayMember = RETRIEVE_MAP_DISPLAY_MEMBER;
            cboSortby.ValueMember = RETRIEVE_MAP_VALUE_MEMBER;

            List<RetrieveField> sortByList = controller.GetSortByFields(AppObjectId);
            sortByList.Insert(0, new RetrieveField());
            sortbyBindingSource.DataSource = sortByList;

            cboSortby.DataSource = sortbyBindingSource;

            String selectedValue = controller.GetSelectedSortByField(AppObjectId);
            if (!String.IsNullOrEmpty(selectedValue))
            {
                cboSortby.SelectedValue = selectedValue;
                cboSortDirectionField1.SelectedIndex = controller.GetSortDirectionForField1(AppObjectId) == RepeatingGroupSortDirection.Ascending ? 0 : 1;
            }
        }

        private void RefreshSortByField2(Guid AppObjectId)
        {
            if (sortbyBindingSource2 != null)
                sortbyBindingSource2.Dispose();

            sortbyBindingSource2 = new BindingSource();

            cboSortByField2.DisplayMember = RETRIEVE_MAP_DISPLAY_MEMBER;
            cboSortByField2.ValueMember = RETRIEVE_MAP_VALUE_MEMBER;

            List<RetrieveField> sortByList = controller.GetSortByFields(AppObjectId);
            //remove the field which is already part of first sortby combobox.
            sortByList.RemoveAll(rf => rf.TargetNamedRange.Equals((cboSortby.SelectedItem as RetrieveField).TargetNamedRange));

            //insert a blank retrieve field
            sortByList.Insert(0, new RetrieveField());

            sortbyBindingSource2.DataSource = sortByList;

            cboSortByField2.DataSource = sortbyBindingSource2;

            String selectedValue = controller.GetSelectedSortByField2(AppObjectId);
            if (!String.IsNullOrEmpty(selectedValue))
            {
                cboSortByField2.SelectedValue = selectedValue;
                cboSortDirectionField2.SelectedIndex = controller.GetSortDirectionForField2(AppObjectId) == RepeatingGroupSortDirection.Ascending ? 0 : 1;
            }
        }

        private void ApplyGroupByDataSource()
        {
            groupbyBindingSource = new BindingSource();
            RepeatingGroup obj = cboObject.SelectedItem as RepeatingGroup;
            if (obj == null)
                return;

            RefreshGroupByDataSource(obj.AppObject);
        }

        private void RefreshGroupByDataSource(Guid AppObjectId)
        {
            if (groupbyBindingSource != null)
            {
                groupbyBindingSource.Dispose();
                groupbyBindingSource = new BindingSource();
            }

            cboGroupBy.DisplayMember = RETRIEVE_MAP_DISPLAY_MEMBER;
            cboGroupBy.ValueMember = RETRIEVE_MAP_VALUE_MEMBER;

            List<RetrieveField> groupByList = controller.GetGroupByFields(AppObjectId);

            groupByList.Insert(0, new RetrieveField());
            groupbyBindingSource.DataSource = groupByList;

            cboGroupBy.DataSource = groupbyBindingSource;

            String selectedValue = controller.GetSelectedGroupByField(AppObjectId);
            if (!String.IsNullOrEmpty(selectedValue))
                cboGroupBy.SelectedValue = selectedValue;
        }

        private void RefreshTotalsBinding(Guid AppObjectId)
        {
            if (totalsBindingSource != null)
                totalsBindingSource.Dispose();

            totalsBindingSource = new BindingSource();

            List<RetrieveField> totalFields = controller.GetCalculatableTotalFields(AppObjectId);

            totalsBindingSource.DataSource = totalFields;
            lstTotalTields.DataSource = totalsBindingSource;

            lstTotalTields.DisplayMember = RETRIEVE_MAP_DISPLAY_MEMBER;
            lstTotalTields.ValueMember = RETRIEVE_MAP_VALUE_MEMBER;

            foreach (string totalFieldID in controller.GetSelectedTotalFields(AppObjectId))
            {
                RetrieveField totalField = totalFields.Where(rf => rf.FieldId.Equals(totalFieldID)).FirstOrDefault();
                int index = lstTotalTields.Items.IndexOf(totalField);
                lstTotalTields.SetItemChecked(index, true);
            }
        }

        private void RefreshGroupByField2(Guid AppObjectId)
        {
            if (groupbyBindingSource2 != null)
                groupbyBindingSource2.Dispose();

            groupbyBindingSource2 = new BindingSource();

            cboGroupByField2.DisplayMember = RETRIEVE_MAP_DISPLAY_MEMBER;
            cboGroupByField2.ValueMember = RETRIEVE_MAP_VALUE_MEMBER;

            List<RetrieveField> groupByList = controller.GetGroupByFields(AppObjectId);
            //remove the group by field which is already part of cbogroupby combobox.
            groupByList.RemoveAll(rf => rf.TargetNamedRange.Equals((cboGroupBy.SelectedItem as RetrieveField).TargetNamedRange));

            //insert a blank item.
            groupByList.Insert(0, new RetrieveField());
            groupbyBindingSource2.DataSource = groupByList;

            cboGroupByField2.DataSource = groupbyBindingSource2;

            String selectedValue = controller.GetSelectedGroupByField2(AppObjectId);
            if (!String.IsNullOrEmpty(selectedValue))
                cboGroupByField2.SelectedValue = selectedValue;
        }

        private void UpdateBindings(Guid AppObjectId, bool bUpdateObject = true)
        {
            if (objectBindingSource != null && bUpdateObject)
            {
                RefreshSFObjectDataSource();
                objectBindingSource.ResetBindings(false);
            }

            if (sortbyBindingSource != null)
            {
                RefreshSortByDataSource(AppObjectId);
                sortbyBindingSource.ResetBindings(false);
            }

            if (groupbyBindingSource != null)
            {
                RefreshGroupByDataSource(AppObjectId);
                groupbyBindingSource.ResetBindings(false);
            }

            if (sortbyBindingSource2 != null)
            {
                RefreshSortByField2(AppObjectId);
                sortbyBindingSource2.ResetBindings(false);
            }

            if (groupbyBindingSource2 != null)
            {
                RefreshGroupByField2(AppObjectId);
                groupbyBindingSource2.ResetBindings(false);
            }

            RefreshTotalsBinding(AppObjectId);

        }

        public void PopulateSpacingValues()
        {
            cboSpacing.Items.Add("1");
            cboSpacing.Items.Add("2");
            cboSpacing.Items.Add("3");
        }

        private void cboGroupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboGroupByField2.Enabled = cboGroupBy.SelectedIndex > 0;
            lstTotalTields.Enabled = cboGroupBy.SelectedIndex > 0;

            if (cboGroupBy.SelectedIndex <= 0)
                cboGroupByField2.SelectedIndex = -1;
            cboSpacing.Enabled = cboGroupBy.SelectedIndex <= 0 ? false : true;
            if (cboSpacing.Enabled == false)
                cboSpacing.SelectedIndex = -1;
            else
                cboSpacing.SelectedIndex = cboSpacing.SelectedIndex == -1 ? 0 : cboSpacing.SelectedIndex;

            if (cboGroupByField2.Enabled)
            {
                RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
                if (repGroup == null)
                    return;

                RefreshGroupByField2(repGroup.AppObject);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            controller.Close();
        }

        private void RefreshSFObjectDataSource()
        {
            cboObject.DisplayMember = "ObjectName";
            cboObject.ValueMember = "AppObject";
            objectBindingSource.DataSource = controller.RetrieveMap.RepeatingGroups;
        }

        private void UpdateControls(RepeatingGroup repGroup)
        {
            txtGridHeader.Text = repGroup.GridHeader;
            cboSpacing.SelectedIndex = repGroup.GroupSpacing - 1;

            string layout = Convert.ToString(repGroup.Layout);
            bool bIsLayoutHorizontal = layout == null ? false : layout.Equals("Horizontal");
            txtLayout.Text = bIsLayoutHorizontal ? "Horizontal" : "Vertical";
            rdbTop.Checked = repGroup.GroupingLayout == GroupingLayout.Top;
            rdbBottom.Checked = repGroup.GroupingLayout == GroupingLayout.Bottom;

            if (bIsLayoutHorizontal)
                rboHorizontal.Checked = true;
            else
                rboVertical.Checked = true;
        }

        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
            if (repGroup != null)
            {
                UpdateBindings(repGroup.AppObject, false);
                UpdateControls(repGroup);
            }
        }

        /// <summary>
        /// This property will be used by the Repeating Group Controller to assign Layout to individual RepeatingGroup Layout property
        /// </summary>
        public string SelectedLayout {
            get {
                txtLayout.Text = rboVertical.Checked ? "Vertical" : "Horizontal";
                return txtLayout.Text;
            }
        }

        private void rboVertical_CheckedChanged(object sender, EventArgs e)
        {
            if (rboVertical.Checked && controller != null)
                controller.SetCellValidatorLayout(RepeatingCellLayout.Vertical);
        }

        private void rboHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            if (rboHorizontal.Checked && controller != null)
                controller.SetCellValidatorLayout(RepeatingCellLayout.Horizontal);
        }

        private void tvObjectField_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node.Parent == null)
                return;

            ApttusObject obj = e.Node.Parent.Tag as ApttusObject;
            RepeatingGroup repGroup = controller.GetRepeatingGroupFromAppObjectId(obj.UniqueId);
            if (repGroup != null)
            {
                cboObject.SelectedItem = repGroup;
                rboHorizontal.Checked = repGroup.Layout.Equals("Horizontal");
                rboVertical.Checked = !rboHorizontal.Checked;
            }
        }

        internal void RemoveObject(RepeatingGroup repGroup)
        {
            if (objectBindingSource != null)
                objectBindingSource.Remove(repGroup);

            controller.RetrieveMap.RepeatingGroups.Remove(repGroup);
        }

        internal string CanRemoveFields()
        {
            List<string> msgs = new List<string>();
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            for (int i = dgvMappedFields.Rows.Count - 1; i >= 0; i--)
            {
                if ((bool)dgvMappedFields.Rows[i].Cells["chkMapped"].FormattedValue)
                {
                    RetrieveField rField = dgvMappedFields.Rows[i].DataBoundItem as RetrieveField;

                    if (checkedFields.Contains(rField))
                        checkedFields.Remove(rField);

                    foreach (SaveMap sMap in configManager.SaveMaps)
                    {
                        foreach (SaveField sf in sMap.SaveFields)
                        {
                            if (rField.FieldId.Equals(sf.FieldId) && rField.Type == sf.Type && rField.AppObject.Equals(sf.AppObject) && rField.TargetNamedRange.Equals(sf.TargetNamedRange))
                            {
                                string msg = new StringBuilder(rField.FieldName).Append(" Exists in ").Append(sMap.Name).Append(" - SaveMap.").ToString();
                                msgs.Add(msg);
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (string msg in msgs)
                sb.Append(msg).Append("\n");

            return sb.ToString();
        }

        internal void RemoveFieldFromSaveMap(RetrieveField rField)
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            foreach (SaveMap sMap in configManager.SaveMaps)
            {
                for (int i = 0; i < sMap.SaveFields.Count; ++i)
                {
                    SaveField sf = sMap.SaveFields[i];
                    if (rField.FieldId.Equals(sf.FieldId) && rField.Type == sf.Type && rField.AppObject.Equals(sf.AppObject) && rField.TargetNamedRange.Equals(sf.TargetNamedRange))
                    {
                        sMap.SaveFields.Remove(sf);
                    }
                }
            }
        }

        internal void UpdateAllSaveMaps()
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            foreach (SaveMap sMap in configManager.SaveMaps)
            {
                for (int i = 0; i < sMap.SaveGroups.Count; ++i)
                {
                    SaveGroup sg = sMap.SaveGroups[i];
                    int nCount = sMap.SaveFields.Where(sf => sf.GroupId.Equals(sg.GroupId)).Count();
                    if (nCount == 0)
                    {
                        sMap.SaveGroups.Remove(sg);
                    }
                }
            }
        }

        private static bool IsGroupByFieldPartOfSaveMap(RepeatingGroup repGroup, ComboBox cboGroupBy, out string groupFieldID)
        {
            string groupbyFieldLabel = GetFieldName(cboGroupBy.SelectedItem);
            string groupByFieldID = Convert.ToString(cboGroupBy.SelectedValue);

            groupFieldID = groupByFieldID;

            if (string.IsNullOrEmpty(groupbyFieldLabel))
                groupbyFieldLabel = groupByFieldID;

            bool bOK = true;
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            List<SaveMap> saveMaps = (from sm in configManager.SaveMaps
                                      from sg in sm.SaveGroups
                                      where sg != null && sg.TargetNamedRange.Equals(repGroup.TargetNamedRange) && repGroup.Layout.Equals(sg.Layout) && repGroup.AppObject.Equals(sg.AppObject)
                                      select sm).ToList();

            foreach (SaveMap saveMap in saveMaps)
            {
                SaveField saveField = (from sf in saveMap.SaveFields
                                       where sf.Type == ObjectType.Repeating && sf.SaveFieldType == SaveType.RetrievedField && sf.FieldId.Equals(groupByFieldID)
                                       select sf).FirstOrDefault();

                //If saveField is not null, groupby field is part of a save map, which is incorrect.
                if (saveField != null)
                {
                    //StringBuilder sb = new StringBuilder(groupbyFieldLabel).Append(" Field is part of a SaveMap : ").Append(saveMap.Name).Append(". GroupBy Field cannot be part of a save map. ")
                    //    .Append(groupbyFieldLabel).Append(" field will be removed from SaveMap.").Append(Environment.NewLine).Append("Do you want to proceed ?");

                    //if (MessageBox.Show(sb.ToString(), "GroupBy Field", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    bOK = saveMap.SaveFields.Remove(saveField);
                    //else
                    //{
                    //    bOK = false;
                    //    break;
                    //}
                }
            }
            return bOK;
        }

        private static string GetFieldName(object item)
        {
            RetrieveField rf = item as RetrieveField;
            if (rf == null)
                return string.Empty;

            ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(rf.AppObject);
            if (obj == null)
                return string.Empty;

            return obj.Fields.Where(fld => fld.Id.Equals(rf.FieldId)).Select(fld => fld.Name).FirstOrDefault();
        }

        private void dgvMappedFields_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                if (dgvMappedFields.Rows[e.RowIndex].DataBoundItem is RetrieveField field && field.FieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                {
                    RetrieveFieldMapper mapper = new RetrieveFieldMapper(field)
                    {
                        MappedRetrieveMap = controller.RetrieveMap
                    };
                    LookAheadDesignerController lookAheadController = new LookAheadDesignerController(mapper, FieldMapperType.RetrieveField);
                    lookAheadController.ShowView();
                }
                //LookAheadPropUI UI = new LookAheadPropUI();
                //UI.FldMapper = mapper;
                //LookAheadExcelSourceController ctroller = new LookAheadExcelSourceController(UI);
                //UI.TopMost = true;
                //UI.Show();
            }
        }

        private void dgvMappedFields_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvMappedFields.Columns[e.ColumnIndex].Name == "RetrievalType")
            {
                ObjectTypeLabel(e);
            }
        }

        internal void ObjectTypeLabel(DataGridViewCellFormattingEventArgs objType)
        {
            if (objType.Value != null)
            {
                try
                {
                    string fieldObjectType = Utils.GetEnumDescription(objType.Value, string.Empty);
                    objType.Value = fieldObjectType;
                    objType.FormattingApplied = true;
                }
                catch (FormatException)
                {
                    // Set to false in case there are other handlers interested trying to 
                    // format this DataGridViewCellFormattingEventArgs instance.
                    objType.FormattingApplied = false;
                }
            }
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
                if (currentItem is ApttusObject)
                    newWidth = (int)g.MeasureString(((ApttusObject)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is RetrieveField)
                    newWidth = (int)g.MeasureString(((RetrieveField)currentItem).FieldName, font).Width + vertScrollBarWidth;
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        private bool resetSortFieldDirection1;

        private void cboSortby_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSortby.SelectedIndex <= 0)
            {
                cboSortByField2.SelectedIndex = -1;
                cboSortDirectionField1.SelectedIndex = -1;
                resetSortFieldDirection1 = true;
            }
            else if (resetSortFieldDirection1)
            {
                cboSortDirectionField1.SelectedIndex = 0;
                resetSortFieldDirection1 = false;
            }

            bool bEnable = cboSortby.SelectedIndex > 0;
            cboSortByField2.Enabled = bEnable;
            cboSortDirectionField1.Enabled = bEnable;

            if (cboSortByField2.Enabled)
            {
                RepeatingGroup repGroup = cboObject.SelectedItem as RepeatingGroup;
                if (repGroup == null)
                    return;
                RefreshSortByField2(repGroup.AppObject);
            }
        }

        private bool resetSortFieldDirection2;

        private void cboSortByField2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSortByField2.SelectedIndex <= 0)
            {
                cboSortDirectionField2.SelectedIndex = -1;
                resetSortFieldDirection2 = true;
            }
            else if (resetSortFieldDirection2)
            {
                cboSortDirectionField2.SelectedIndex = 0;
                resetSortFieldDirection2 = false;
            }

            cboSortDirectionField2.Enabled = cboSortByField2.SelectedIndex > 0;
        }

        private void tbSearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchString = tbSearchBox.Text.Trim();
                BindingList<RetrieveField> filterResults = new BindingList<RetrieveField>((from x in listOfRetrieveFields
                                                                                           where x.FieldName.ToLower().Contains(searchString.ToLower())
                                                                                           select x).ToList());
                dgvMappedFields.DataSource = filterResults;
                for (int i = dgvMappedFields.Rows.Count - 1; i >= 0; i--)
                {
                    RetrieveField ret = dgvMappedFields.Rows[i].DataBoundItem as RetrieveField;
                    if (checkedFields.Contains(ret))
                    {
                        dgvMappedFields.Rows[i].Cells["chkMapped"].Value = true;
                    }
                }
                ObjectType value;
                Enum.TryParse<ObjectType>(cmbType.SelectedValue.ToString(), out value);
                BindTreeView(value, searchString);
                RefreshLookAheadIcons();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void lblClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tbSearchBox.Text = string.Empty;
            tbSearchBox.Focus();
        }

        private void dgvMappedFields_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvMappedFields.IsCurrentCellDirty)
            {
                dgvMappedFields.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvMappedFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMappedFields.Columns[e.ColumnIndex].Name == "chkMapped")
            {
                RetrieveField field = dgvMappedFields.Rows[e.RowIndex].DataBoundItem as RetrieveField;
                if ((bool)dgvMappedFields.Rows[e.RowIndex].Cells["chkMapped"].FormattedValue)
                {
                    checkedFields.Add(field);
                }
                else
                {
                    if (checkedFields.Contains(field))
                    {
                        checkedFields.Remove(field);
                    }
                }
            }
        }
    }

    public class CellLocation
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Formula { get; set; }
    }
}