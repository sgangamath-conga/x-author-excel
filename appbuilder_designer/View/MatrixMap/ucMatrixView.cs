/*
 * Apttus X-Author for Excel
 * © 2015 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ucMatrixView : UserControl
    {
        private MatrixViewController controller;
        private TreeNode selectedNode;
        private bool bItemDragged;
        private BindingList<MatrixFieldGridModel> listOfObjects;
        private BindingList<MatrixComponentGridModel> matrixComponents;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        public Guid highlightedComponentId;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        /// <summary>
        /// Initialize gloabl variables used across Map
        /// </summary>
        public ucMatrixView()
        {
            InitializeComponent();
            SetCultureData();
            bItemDragged = false;
            selectedNode = null;
            listOfObjects = new BindingList<MatrixFieldGridModel>();
            matrixComponents = new BindingList<MatrixComponentGridModel>();
            listOfObjects.AllowNew = true;
            highlightedComponentId = Guid.Empty;
        }

        private void SetCultureData()
        {
            Applyfilter.HeaderText = resourceManager.GetResource("UCMATRIXVIEW_Applyfilter_HeaderText");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnExtendComponent.Text = resourceManager.GetResource("UCMATRIXVIEW_btnExtendComponent_Text");
            btnRemove.Text = resourceManager.GetResource("COMMON_Remove_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //cboObjectType.Text = resourceManager.GetResource("UCMATRIXVIEW_cboObjectType_Items");
            colRenderingType.HeaderText = resourceManager.GetResource("UCMATRIXVIEW_colRenderingType_HeaderText");
            ComponentName.HeaderText = resourceManager.GetResource("COMMON_SectionName_Text");
            ComponentsTabPage.Text = resourceManager.GetResource("UCMATRIXVIEW_ComponentsTabPage_Text");
            Entity.HeaderText = resourceManager.GetResource("COMMON_FieldType_Text");
            Edit.HeaderText = resourceManager.GetResource("COMMON_Edit_Text");
            FieldName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
            label1.Text = resourceManager.GetResource("COMMON_Name_Text");
            label2.Text = resourceManager.GetResource("COMMON_Type_Text");
            label3.Text = resourceManager.GetResource("COMMON_Object_Text");
            MatrixMapTabPage.Text = resourceManager.GetResource("COMMON_Fields_Text");
            ObjectName.HeaderText = resourceManager.GetResource("COMMON_ObjectName_Text");
            TargetLocation.HeaderText = resourceManager.GetResource("COMMON_Location_Text");
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Globals.ThisAddIn.Application.SheetChange += Application_SheetChange;
            Globals.ThisAddIn.Application.SheetSelectionChange += Application_SheetSelectionChange;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Globals.ThisAddIn.Application.SheetChange -= Application_SheetChange;
            Globals.ThisAddIn.Application.SheetSelectionChange -= Application_SheetSelectionChange;
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Used to select a row in matrixmap grid, when a matrixfield is selected in excel.
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        void Application_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            try
            {
                Excel.Name targetNamedRange = Target.Name as Excel.Name;
                string namedRange = targetNamedRange.Name as string;

                if (String.IsNullOrEmpty(namedRange))
                    return;

                if (tabMatrixControl.SelectedTab == MatrixMapTabPage)
                {
                    dgvMatrixMap.ClearSelection();

                    DataGridViewRow row = dgvMatrixMap.Rows.Cast<DataGridViewRow>().Where(r => (r.Tag as MatrixField).TargetNamedRange.Equals(namedRange)).FirstOrDefault();
                    if (row == null)
                        return;

                    dgvMatrixMap.FirstDisplayedScrollingRowIndex = row.Index;
                    row.Selected = true;

                }
                else
                {
                    dgvMatrixComponent.ClearSelection();
                    Guid componentId = controller.GetMatrixComponentIdFromDataFieldNamedRanged(namedRange);
                    if (componentId != Guid.Empty)
                    {
                        DataGridViewRow row = dgvMatrixComponent.Rows.Cast<DataGridViewRow>().Where(r => (Guid.Parse(r.Tag.ToString())).Equals(componentId)).FirstOrDefault();
                        if (row == null)
                            return;

                        dgvMatrixMap.FirstDisplayedScrollingRowIndex = row.Index;
                        row.Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Drag & Drop a Matrix field on Excel using AddMatrixObjectInExcel
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        void Application_SheetChange(object Sh, Excel.Range Target)
        {
            if (!bItemDragged)
                return;

            bItemDragged = false;
            if (selectedNode != null)
            {
                if (cboObjectType.SelectedIndex == 0) //Individual Fields
                {
                    ApttusField apttusfield = selectedNode.Tag as ApttusField;

                    MatrixIndividualField field = new MatrixIndividualField();
                    field.Id = Guid.NewGuid();
                    field.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
                    field.DataType = apttusfield.Datatype;
                    field.FieldId = apttusfield.Id;
                    field.FieldName = apttusfield.Name;
                    field.TargetLocation = ExcelHelper.GetAddress(Target);
                    field.TargetNamedRange = CreateNamedRangeForMatrixField(Target);
                    field.ValueType = MatrixValueType.FieldValue;
                    field.RenderingType = MatrixRenderingType.Dynamic;
                    field.SortFieldId = null;//Set this value explicitly to null. 
                    Target.Value = string.Empty;

                    Excel.Range cell = ExcelHelper.NextHorizontalCell(Target, -1);
                    if (cell != null)
                        cell.Value = apttusfield.Name;

                    controller.AddIndividualField(field);
                    AddMatrixFieldToGrid(field, MatrixEntity.Individual);
                }
                else if (cboObjectType.SelectedIndex == 1)
                {
                    using (MatrixFieldView fieldView = new MatrixFieldView(FormOpenMode.Create, controller, Target.Row, Target.Column))
                    {
                        fieldView.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
                        fieldView.ObjectName = (cboObject.SelectedItem as ApttusObject).Name;
                        fieldView.FieldName = (selectedNode.Tag as ApttusField).Name;
                        fieldView.FieldID = (selectedNode.Tag as ApttusField).Id;
                        fieldView.Components = controller.Model.MatrixComponents;

                        if (fieldView.ShowDialog() == DialogResult.OK)
                            AddMatrixObjectInExcel(fieldView, Target);
                        else
                            Globals.ThisAddIn.Application.Undo();
                    }
                }
                selectedNode = null;
            }
        }

        public string CreateNamedRangeForMatrixField(Excel.Range Target)
        {
            // Get named range for target cell
            string targetNamedRange = ExcelHelper.GetExcelCellName(Target);
            if (string.IsNullOrEmpty(targetNamedRange))
            {
                targetNamedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, targetNamedRange);
            }
            return targetNamedRange;
        }

        /// <summary>
        /// Adds corresponding Matrix Object (row, column or data) on Excel
        /// </summary>
        /// <param name="fieldView"></param>
        /// <param name="Target"></param>
        private void AddMatrixObjectInExcel(MatrixFieldView fieldView, Excel.Range Target)
        {
            try
            {
                if (fieldView.RenderingType == MatrixRenderingType.Static)
                {
                    Globals.ThisAddIn.Application.Undo();

                    //After Undo operation, check whether the given cell contains any text. If yes, don't do anything. If no, provide the field name in that cell.
                    string value = Target.Value as string;
                    if (string.IsNullOrEmpty(value))
                        Target.Value = (selectedNode.Tag as ApttusField).Name;
                }
                // Get named range for target cell
                string targetNamedRange = CreateNamedRangeForMatrixField(Target);

                MatrixField field = null;
                string targetLocation = ExcelHelper.GetAddress(Target);

                MatrixEntity selectedMatrixEntity = MatrixEntity.Row;

                // Based on Matrix type create row, column or data fields and add it matrix map model
                switch (fieldView.MatrixEntityType)
                {
                    case MatrixEntity.Row:
                        {
                            field = CreateRowMatrixField(fieldView, targetNamedRange, targetLocation);
                            selectedMatrixEntity = MatrixEntity.Row;
                            break;
                        }
                    case MatrixEntity.GroupedColumn:
                        {
                            MatrixField colField = controller.GetIntersectingColumnMatrixField(Target.Column);
                            field = CreateGroupedColumnMatrixField(fieldView, targetNamedRange, targetLocation, colField);
                            selectedMatrixEntity = MatrixEntity.GroupedColumn;
                            break;
                        }
                    case MatrixEntity.Column:
                        {

                            field = CreateColumnMatrixField(fieldView, targetNamedRange, targetLocation);
                            selectedMatrixEntity = MatrixEntity.Column;
                            break;
                        }
                    case MatrixEntity.Data:
                        {
                            string componentName = fieldView.ComponentName;
                            MatrixDataField dataField;
                            field = dataField = CreateDataMatrixField(Target.Row, Target.Column, targetNamedRange, targetLocation, componentName);
                            Guid componentId;
                            bool bAddComponent = controller.CreateMatrixComponent(dataField.AppObjectUniqueID, componentName, out componentId);
                            if (bAddComponent)
                                AddMatrixComponentToGrid(componentName, dataField.AppObjectUniqueID, componentId);
                            dataField.MatrixComponentId = componentId;
                            dataField.RowLookupId = fieldView.RowLookupId;
                            dataField.ColumnLookupId = fieldView.ColumnLookupId;
                            selectedMatrixEntity = MatrixEntity.Data;
                        }
                        break;
                }

                AddMatrixFieldToGrid(field, selectedMatrixEntity);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private MatrixField CreateGroupedColumnMatrixField(MatrixFieldView fieldView, string targetNamedRange, string targetLocation, MatrixField colField)
        {
            ApttusField apttusField = selectedNode.Tag as ApttusField;

            MatrixField groupedField = new MatrixField();

            groupedField.Id = Guid.NewGuid();
            groupedField.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
            groupedField.FieldId = apttusField.Id;
            groupedField.DataType = apttusField.Datatype;
            groupedField.FieldName = apttusField.Name;
            groupedField.RenderingType = fieldView.RenderingType;
            groupedField.TargetLocation = targetLocation;
            groupedField.TargetNamedRange = targetNamedRange;
            groupedField.ValueType = fieldView.ValueType;
            groupedField.SortFieldId = fieldView.SortFieldId;
            groupedField.MatrixGroupedParentId = Guid.Empty;

            colField.MatrixGroupedFields.Add(groupedField);

            controller.AddMatrixGroupedFieldToColumn(groupedField);

            return groupedField;
        }

        /// <summary>
        /// Adds a row representing a matrix component in matrix component grid.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="dataField"></param>
        /// <param name="componentName"></param>
        /// <param name="componentId"></param>
        private void AddMatrixComponentToGrid(string componentName, Guid componentAppObjectUniqueId, Guid componentId)
        {
            MatrixComponentGridModel componentGridModel = new MatrixComponentGridModel();

            componentGridModel.ComponentName = componentName;
            componentGridModel.ObjectName = ApplicationDefinitionManager.GetInstance.GetAppObject(componentAppObjectUniqueId).Name;

            matrixComponents.Add(componentGridModel);

            dgvMatrixComponent.Rows[dgvMatrixComponent.Rows.Count - 1].Tag = componentId;

            dgvMatrixComponent.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="dataField"></param>
        /// <param name="entityType"></param>
        private void AddMatrixFieldToGrid(MatrixField field, MatrixEntity entityType)
        {
            //Add MatrixEntity in Grid.
            listOfObjects.Add(new MatrixFieldGridModel
            {
                RenderType = field.RenderingType,
                EntityType = entityType,
                FieldName = field.FieldName,
                Location = field.TargetLocation,
                FieldValueType = field.ValueType,
            });

            dgvMatrixMap.Rows[dgvMatrixMap.Rows.Count - 1].Tag = field;
        }

        /// <summary>
        /// Creates Matrix Row field
        /// </summary>
        /// <param name="fieldView"></param>
        /// <param name="targetNamedRange"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        private MatrixField CreateRowMatrixField(MatrixFieldView fieldView, string targetNamedRange, string targetLocation)
        {
            ApttusField apttusField = selectedNode.Tag as ApttusField;

            MatrixField field = new MatrixField();

            field.Id = Guid.NewGuid();

            field.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
            field.DataType = apttusField.Datatype;
            field.FieldId = apttusField.Id;
            field.FieldName = apttusField.Name;
            field.RenderingType = fieldView.RenderingType;
            field.TargetLocation = targetLocation;
            field.TargetNamedRange = targetNamedRange;
            field.ValueType = fieldView.ValueType;
            field.SortFieldId = fieldView.SortFieldId;

            controller.AddMatrixFieldToRow(field);

            return field;
        }

        /// <summary>
        /// Creates Matrix Column field
        /// </summary>
        /// <param name="details"></param>
        /// <param name="targetNamedRange"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        private MatrixField CreateColumnMatrixField(MatrixFieldView fieldView, string targetNamedRange, string targetLocation)
        {
            ApttusField apttusField = selectedNode.Tag as ApttusField;

            MatrixField field = new MatrixField();

            field.Id = Guid.NewGuid();
            field.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
            field.FieldId = apttusField.Id;
            field.DataType = apttusField.Datatype;
            field.FieldName = apttusField.Name;
            field.RenderingType = fieldView.RenderingType;
            field.TargetLocation = targetLocation;
            field.TargetNamedRange = targetNamedRange;
            field.ValueType = fieldView.ValueType;
            field.SortFieldId = fieldView.SortFieldId;

            controller.AddMatrixFieldToColumn(field);

            return field;
        }

        /// <summary>
        /// Creates matrix datafield from existing datafield
        /// </summary>
        /// <param name="targetNamedRange"></param>
        /// <param name="targetLocation"></param>
        /// <param name="datafield"></param>
        /// <returns></returns>
        private MatrixDataField CreateDataFieldFromExistingDataField(string targetNamedRange, string targetLocation, MatrixDataField datafield)
        {
            MatrixDataField newdataField = new MatrixDataField();

            newdataField.Name = datafield.Name;
            newdataField.Id = Guid.NewGuid();
            newdataField.AppObjectUniqueID = datafield.AppObjectUniqueID;
            newdataField.DataType = datafield.DataType;
            newdataField.FieldId = datafield.FieldId;
            newdataField.FieldName = datafield.FieldName;
            newdataField.MatrixComponentId = datafield.MatrixComponentId;
            newdataField.TargetLocation = targetLocation;
            newdataField.TargetNamedRange = targetNamedRange;

            controller.AddMatrixFieldAsDataField(newdataField);

            return newdataField;
        }

        /// <summary>
        /// Create Matrix Data field
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="targetNamedRange"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        private MatrixDataField CreateDataMatrixField(int row, int column, string targetNamedRange, string targetLocation, string componentName)
        {
            MatrixDataField dataField = new MatrixDataField();

            Guid rowId, columnId;
            controller.GetIntersectingRowAndColumnId(row, column, out rowId, out columnId);

            if (!Guid.Empty.Equals(rowId) && !Guid.Empty.Equals(columnId))
            {
                ApttusField apttusfield = selectedNode.Tag as ApttusField;

                dataField.Name = componentName;

                dataField.Id = Guid.NewGuid();
                dataField.AppObjectUniqueID = (Guid)cboObject.SelectedValue;
                dataField.FieldId = apttusfield.Id;
                dataField.DataType = apttusfield.Datatype;
                dataField.FieldName = apttusfield.Name;
                dataField.TargetNamedRange = targetNamedRange;
                dataField.TargetLocation = targetLocation;
                dataField.MatrixRowId = rowId;
                dataField.MatrixColumnId = columnId;
                dataField.MatrixComponentId = Guid.Empty;

                controller.AddMatrixFieldAsDataField(dataField);
            }

            return dataField;
        }

        public void SetController(MatrixViewController controller)
        {
            this.controller = controller;
        }

        private void LoadObjects(ObjectType type)
        {
            List<ApttusObject> objects = controller.GetObjects().Where(obj => obj.ObjectType == type).ToList();
            cboObject.DisplayMember = Constants.NAME_ATTRIBUTE;
            cboObject.ValueMember = Constants.UNIQUEID_ATTRIBUTE;
            cboObject.DataSource = objects;

            if (objects.Count == 0)
                tvFields.Nodes.Clear();
        }

        /// <summary>
        /// Load matrix view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucMatrixView_Load(object sender, EventArgs e)
        {
            dgvMatrixMap.AutoGenerateColumns = false;

            dgvMatrixMap.DataSource = listOfObjects;

            tabMatrixControl.SelectedTab = ComponentsTabPage;
            ComponentsTabPage.Show();

            tabMatrixControl.SelectedTab = MatrixMapTabPage;
            MatrixMapTabPage.Show();

            dgvMatrixMap.Columns[Edit.Name].DisplayIndex = 4;

            dgvMatrixComponent.AutoGenerateColumns = false;
            dgvMatrixComponent.DataSource = matrixComponents;

            if (controller.OpenMode == Core.FormOpenMode.Edit)
            {
                txtName.Text = controller.Model.Name;

                foreach (MatrixField field in controller.Model.MatrixRow.MatrixFields)
                    AddMatrixFieldToGrid(field, Core.MatrixEntity.Row);

                foreach (MatrixField field in controller.Model.MatrixColumn.MatrixFields)
                {
                    AddMatrixFieldToGrid(field, Core.MatrixEntity.Column);
                    foreach (MatrixField groupedField in field.MatrixGroupedFields)
                        AddMatrixFieldToGrid(groupedField, MatrixEntity.GroupedColumn);
                }
                foreach (MatrixDataField field in controller.Model.MatrixData.MatrixDataFields)
                    AddMatrixFieldToGrid(field, Core.MatrixEntity.Data);

                foreach (MatrixComponent component in controller.Model.MatrixComponents)
                    AddMatrixComponentToGrid(component.Name, component.AppObjectUniqueID, component.Id);

                foreach (MatrixField field in controller.Model.IndependentFields)
                    AddMatrixFieldToGrid(field, MatrixEntity.Individual);
            }

            cboObjectType.SelectedIndex = 1;
            if (controller.GetObjects().Where(obj => obj.ObjectType == ObjectType.Repeating).Count() > 0)
                cboObject.SelectedIndex = 0;
        }

        /// <summary>
        /// Populate treeview based on Object selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            tvFields.Nodes.Clear();
            if (cboObject.SelectedIndex == -1)
                return;

            Guid objUniqueId = (Guid)cboObject.SelectedValue;
            ApttusObject apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(objUniqueId);
            if (apttusObject != null)
            {
                TreeNode rootNode = new TreeNode(apttusObject.Name);
                List<ApttusField> sortedFields = apttusObject.Fields.OrderBy(field => Convert.ToString(field.Name)).ToList();
                foreach (ApttusField field in sortedFields)
                {
                    if (field.Id.Equals(apttusObject.IdAttribute) || field.Datatype == Datatype.Rich_Textarea || field.Datatype == Datatype.Attachment || field.Datatype == Datatype.Textarea)
                        continue;
                    TreeNode fieldNode = rootNode.Nodes.Add(field.Name);
                    fieldNode.Tag = field;
                }

                rootNode.Tag = null;
                tvFields.Nodes.Add(rootNode);

                rootNode.Expand();
            }
        }

        private void tvFields_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
            {
                selectedNode = null;
                return;
            }

            TreeNode node = e.Item as TreeNode;
            if (node != null && node.Tag != null)
                selectedNode = node;
        }

        private void tvFields_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && selectedNode != null)
            {
                if (selectedNode.Level != 0 && selectedNode.FirstNode == null)
                {
                    if (tvFields.DoDragDrop(((ApttusField)selectedNode.Tag).Name, DragDropEffects.Copy) == DragDropEffects.Copy)
                        bItemDragged = true;
                }
                else
                    tvFields.DoDragDrop(selectedNode.Text, DragDropEffects.None);
            }
        }

        /// <summary>
        /// Save Matrix map model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ApttusFormValidator.hasValidationErrors(this.Controls))
            {
                if (controller != null)
                    controller.Save(txtName.Text);
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("UCMATRIXVIEW_btnSave_Click_InfoMsg"), resourceManager.GetResource("COMMON_btnSave_Text"));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            controller.Close();
        }

        private void dgvMatrixMap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Edit.Index)
            {
                MatrixFieldGridModel model = listOfObjects[e.RowIndex];
                if (model.EntityType == MatrixEntity.Individual) //No editing for Individual Fields
                    return;

                using (MatrixFieldView fieldView = new MatrixFieldView(FormOpenMode.Edit, controller, -1, -1))
                {
                    MatrixField field = dgvMatrixMap.Rows[e.RowIndex].Tag as MatrixField;
                    fieldView.AppObjectUniqueID = field.AppObjectUniqueID;
                    fieldView.EditingMatrixField = field;

                    if (model.EntityType == MatrixEntity.Data)
                    {
                        MatrixDataField dataField = (field as MatrixDataField);
                        fieldView.ComponentName = dataField.Name;
                        fieldView.RowLookupId = dataField.RowLookupId;
                        fieldView.ColumnLookupId = dataField.ColumnLookupId;
                    }

                    fieldView.Components = controller.Model.MatrixComponents;
                    fieldView.ObjectName = appDefManager.GetAppObject(field.AppObjectUniqueID).Name;
                    fieldView.FieldName = field.FieldName;
                    fieldView.GridModel = model;
                    fieldView.SortFieldId = field.SortFieldId;
                    fieldView.ShowDialog();
                    dgvMatrixMap.Refresh();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvMatrixMap.SelectedRows != null && dgvMatrixMap.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMatrixMap.SelectedRows[0];
                MatrixFieldGridModel model = listOfObjects[row.Index];
                if (model != null)
                {
                    MatrixField field = row.Tag as MatrixField;
                    switch (model.EntityType)
                    {
                        case MatrixEntity.Individual:
                        case MatrixEntity.Data:
                            RemoveMatrixField(model, field, model.EntityType);
                            break;
                        case MatrixEntity.Row:
                        case MatrixEntity.GroupedColumn:
                        case MatrixEntity.Column:
                            {
                                //Check whether the row and column entity are bounded to any DataField
                                if (controller.CanRemoveMatrixField(model.EntityType, field))
                                    RemoveMatrixField(model, field, model.EntityType);
                                else
                                {
                                    ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("UCMATRIXVIEW_btnRemove_Click_WarnMsg"), model.EntityType), resourceManager.GetResource("UCMATRIXVIEW_btnRemove_ClickCAP_WarnMsg"), Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a matrix field from excel, grid and the model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveMatrixField(MatrixFieldGridModel model, MatrixField field, MatrixEntity entity)
        {
            //Remove matrix field from model
            controller.RemoveMatrixField(model.EntityType, field);
            // Remove Matrix Independant field label
            if (entity == MatrixEntity.Individual)
            {
                Excel.Range oLabelRange = ExcelHelper.NextHorizontalCell(ExcelHelper.GetRange(field.TargetNamedRange), -1);
                if (oLabelRange != null)
                    oLabelRange.ClearContents();
            }
            //Remove matrix field from Excel. Also Remove the NamedRange
            ExcelHelper.RemoveNamedRange(field.TargetNamedRange, true);
            //Remove matrix field from Grid
            listOfObjects.Remove(model);

            if (model.EntityType == MatrixEntity.Data)
            {
                //remove matrix component if any
                Guid componentId = (field as MatrixDataField).MatrixComponentId;

                if (controller.CanRemoveMatrixComponent(componentId))
                    RemoveMatrixComponent(componentId);
            }
        }

        /// <summary>
        /// Removes a matrix component if the data field count representing that component goes to zero
        /// </summary>
        /// <param name="componentId"></param>
        private void RemoveMatrixComponent(Guid componentId)
        {
            string componentName = dgvMatrixComponent.Rows.Cast<DataGridViewRow>().Where(row => Guid.Parse(row.Tag.ToString()).Equals(componentId)).FirstOrDefault().Cells[0].Value as string;
            MatrixComponentGridModel gridModel = matrixComponents.Where(cmp => cmp.ComponentName.Equals(componentName)).FirstOrDefault();
            if (gridModel != null)
                matrixComponents.Remove(gridModel);

            controller.RemoveMatrixComponent(componentId);
        }

        private void dgvMatrixMap_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            btnRemove.Enabled = true;
        }

        private void dgvMatrixMap_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            btnRemove.Enabled = listOfObjects.Count() > 0;
        }

        private void dgvMatrixComponent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMatrixComponent.Columns[e.ColumnIndex] is DataGridViewImageColumn && e.RowIndex >= 0)
            {
                using (MatrixComponentFilterWindow filterWindow = new MatrixComponentFilterWindow())
                {
                    DataGridViewRow row = dgvMatrixComponent.Rows[e.RowIndex];
                    Guid componentId = Guid.Parse(row.Tag.ToString());
                    MatrixComponent component = controller.GetMatrixComponentById(componentId);
                    filterWindow.ApttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(component.AppObjectUniqueID);
                    if (component != null)
                        filterWindow.WhereFilterGroups = component.WhereFilterGroups;
                    
                    if (filterWindow.ShowDialog() == DialogResult.OK)
                        component.WhereFilterGroups = filterWindow.WhereFilterGroups;
                }
            }
        }

        private void btnExtendComponent_Click(object sender, EventArgs e)
        {
            //Only those entities are extended whose MatrixValueType is FieldValue.
            Excel.Range rg = Globals.ThisAddIn.Application.Selection as Excel.Range;
            if (rg != null)
            {
                //1. Check whether the given selection contains more than one row & one column.
                if (rg.Rows.Count > 1 && rg.Columns.Count > 1)
                    return;

                //2. validate whether the first cell in the selection is intersecting either a row or column field.
                Excel.Range firstCell = rg.Cells[1, 1];
                Guid rowId, colId;
                controller.GetIntersectingRowAndColumnId(firstCell.Row, firstCell.Column, out rowId, out colId);

                if (Guid.Empty.Equals(rowId) || Guid.Empty.Equals(colId)) //If any of them is empty
                {
                    //return failure msg;
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("UCMATRIXVIEW_btnExtendComponent_ClickEMPTY_InfoMsg"), resourceManager.GetResource("COMMON_MatrixSection_Text"));
                    return;
                }

                //3.Check if there is a component selected from the componentgrid or not.
                if (dgvMatrixComponent.SelectedRows.Count != 1)
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("UCMATRIXVIEW_btnExtendComponent_ClickSELECTION_InfoMsg"), resourceManager.GetResource("COMMON_MatrixSection_Text"));
                    return;
                }
                //4.Check what is the selection type the user has selected. 
                Selection currSelectionType;
                if (rg.Rows.Count == 1)
                    currSelectionType = Selection.SingleRowMultiColumn;
                else
                    currSelectionType = Selection.SingleColMultiRow;

                //5.Get the datafield to drag & copy based on the rowId & colId
                MatrixDataField dataFieldToCopy = controller.GetMatrixDataFieldByRowColumnId(rowId, colId);
                if (dataFieldToCopy == null)
                    return;

                MatrixField rowField = controller.GetMatrixFieldFromRowId(rowId);
                MatrixField colField = controller.GetMatrixFieldFromColumnId(colId);

                if (rowField == null || colField == null)
                    return;

                //6.Based on the current selection, validate whether the given selection is correct to extend the selected datafield.
                bool bSelectionValid = false;
                switch (currSelectionType)
                {
                    case Selection.SingleColMultiRow:
                        if (rowField.ValueType == MatrixValueType.FieldValue && colField.ValueType == MatrixValueType.FieldLabel)
                            bSelectionValid = true;
                        else if (rowField.ValueType == MatrixValueType.FieldValue && colField.ValueType == MatrixValueType.FieldValue)
                            bSelectionValid = true;
                        break;
                    case Selection.SingleRowMultiColumn:
                        if (colField.ValueType == MatrixValueType.FieldValue && rowField.ValueType == MatrixValueType.FieldLabel)
                            bSelectionValid = true;
                        else if (colField.ValueType == MatrixValueType.FieldValue && rowField.ValueType == MatrixValueType.FieldValue)
                            bSelectionValid = true;
                        break;
                }

                if (!bSelectionValid)
                    return;

                //Get the component id to copy to.
                DataGridViewRow row = dgvMatrixComponent.SelectedRows[0];
                Guid componentId = Guid.Parse(row.Tag.ToString());
                MatrixComponent component = controller.GetMatrixComponentById(componentId);
                if (component == null)
                    return;

                foreach (Excel.Range cell in rg.Cells)
                {
                    //7. Check whether the given cell has intersecting MatrixRow & MatrixColum
                    Guid rId, cId;
                    controller.GetIntersectingRowAndColumnId(cell.Row, cell.Column, out rId, out cId);
                    if (Guid.Empty.Equals(rId) && Guid.Empty.Equals(cId))  //If there is no row or column, just end the loop.
                        break;

                    if (Guid.Empty.Equals(cId)) //If there is no column, create it
                        cId = ExtendColumn(dataFieldToCopy, cell);
                    else if (Guid.Empty.Equals(rId)) //If there is no row, create it
                        rId = ExtendRow(dataFieldToCopy, cell);

                    if (controller.GetMatrixDataFieldByRowColumnId(rId,cId) == null) //If there is no data create it.
                        ExtendData(cell, dataFieldToCopy, rId, cId, componentId);
                }
                HighlightComponent(componentId);
            }
       }

        /// <summary>
        /// Replicates an existing data field in the target cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="dataField"></param>
        /// <param name="rowId"></param>
        /// <param name="colId"></param>
        /// <param name="componentId"></param>
        private void ExtendData(Excel.Range cell, MatrixDataField dataField, Guid rowId, Guid colId, Guid componentId)
        {
            if (controller.ValidateDataField(cell.Row, cell.Column, dataField.AppObjectUniqueID))
            {
                string namedRange = CreateNamedRangeForMatrixField(cell);
                string targetLocation = ExcelHelper.GetAddress(cell);
                MatrixDataField newDataField = CreateDataFieldFromExistingDataField(namedRange, targetLocation, dataField);
                newDataField.MatrixRowId = rowId;
                newDataField.MatrixColumnId = colId;
                newDataField.MatrixComponentId = componentId;
                AddMatrixFieldToGrid(newDataField, MatrixEntity.Data);
                cell.Value = newDataField.FieldName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFieldToCopy"></param>
        /// <param name="cell"></param>
        private Guid ExtendRow(MatrixDataField dataFieldToCopy, Excel.Range cell)
        {
            MatrixField existingRowField = controller.GetMatrixFieldFromRowId(dataFieldToCopy.MatrixRowId);

            //Calculate the cell index and create the named range on the cell
            Excel.Range rowRange = ExcelHelper.GetRange(existingRowField.TargetNamedRange);
            Excel.Range rowCell = ExcelHelper.NextVerticalCell(rowRange, cell.Row - rowRange.Row);
            string newNamedRange = CreateNamedRangeForMatrixField(rowCell);
            MatrixField newRowField = controller.CloneMatrixFieldFromExistingField(existingRowField, ExcelHelper.GetAddress(rowCell), newNamedRange);
            controller.AddMatrixFieldToRow(newRowField);
            AddMatrixFieldToGrid(newRowField, MatrixEntity.Row);

            return newRowField.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFieldToCopy"></param>
        /// <param name="cell"></param>
        private Guid ExtendColumn(MatrixDataField dataFieldToCopy, Excel.Range cell)
        {
            MatrixField existingColField = controller.GetMatrixFieldFromColumnId(dataFieldToCopy.MatrixColumnId);

            //Calculate the cell index and create the named range on the cell
            Excel.Range colRange = ExcelHelper.GetRange(existingColField.TargetNamedRange);
            Excel.Range colCell = ExcelHelper.NextHorizontalCell(colRange, cell.Column - colRange.Column);
            string newNamedRange = CreateNamedRangeForMatrixField(colCell);
            MatrixField newcolField = controller.CloneMatrixFieldFromExistingField(existingColField, ExcelHelper.GetAddress(colCell), newNamedRange);
            controller.AddMatrixFieldToColumn(newcolField);
            AddMatrixFieldToGrid(newcolField, MatrixEntity.Column);

            return newcolField.Id;
        }

        private void dgvMatrixComponent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMatrixComponent.Rows[e.RowIndex];
                Guid componentId = Guid.Parse(row.Tag.ToString());

                HighlightComponent(componentId);
            }
        }

        private void HighlightComponent(Guid componentId)
        {
            if (highlightedComponentId != Guid.Empty)
                UnHighlightComponent(highlightedComponentId);

            List<MatrixField> dataFields = controller.GetMatrixFieldsByComponentId(componentId);
            foreach (MatrixField dataField in dataFields)
            {
                Excel.Range dataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);
                ExcelHelper.ApplyBorderToRange(dataRange);
            }
            highlightedComponentId = componentId;
        }

        public void UnHighlightComponent(Guid componentId)
        {
            List<MatrixField> dataFields = controller.GetMatrixFieldsByComponentId(componentId);
            foreach (MatrixField dataField in dataFields)
            {
                Excel.Range dataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);
                ExcelHelper.RemoveBorderFromRange(dataRange);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 0)
            {
                if (highlightedComponentId != Guid.Empty)
                    UnHighlightComponent(highlightedComponentId);
            }
        }

        private void cboObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboObjectType.SelectedIndex < 0)
                return;
            ObjectType type = cboObjectType.SelectedIndex == 0 ? ObjectType.Independent : ObjectType.Repeating;
            LoadObjects(type);
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
    }

    internal class MatrixFieldGridModel
    {
        public MatrixRenderingType RenderType { get; set; }
        public MatrixEntity EntityType { get; set; }
        public string FieldName { get; set; }
        public string Location { get; set; }
        public MatrixValueType FieldValueType { get; set; }
    }

    internal class MatrixComponentGridModel
    {
        public string ComponentName { get; set; }
        public string ObjectName { get; set; }
    }

    enum EntityExtension
    {
        ExtendRow,
        ExtendColumn,
        ExtendData
    }

    enum GroupedColumnValidation
    {
        Success,
        GroupedFieldDuplication,
        NoRelationExistsWithSiblingField,
        ColumnTypeIsStatic
    }

    enum Selection
    {
        SingleRowMultiColumn,
        SingleColMultiRow,
        SingleCell,
    }
}
