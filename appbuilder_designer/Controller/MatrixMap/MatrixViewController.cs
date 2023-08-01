/*
 * Apttus X-Author for Excel
 * © 2015 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public class MatrixViewController
    {
        internal MatrixMap Model { get; set; }

        private ucMatrixView View;
        private FormOpenMode Mode;
        private Dictionary<string, MatrixField> namedRangeToRowField;
        private Dictionary<string, MatrixField> namedRangeToColField;
        private Dictionary<string, MatrixField> namedRangeToGroupedColField;

        private string TaskPaneName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="model"></param>
        /// <param name="mode"></param>
        public MatrixViewController(ucMatrixView view, MatrixMap model, FormOpenMode mode)
        {
            TaskPaneName = ApttusResourceManager.GetInstance.GetResource("RIBBON_btnMatrixMap_Label");// Constants.MATRIXMAP_NAME;
            View = view;
            Model = model;
            Mode = mode;

            namedRangeToRowField = new Dictionary<string, MatrixField>();
            namedRangeToColField = new Dictionary<string, MatrixField>();
            namedRangeToGroupedColField = new Dictionary<string, MatrixField>();

            if (mode == FormOpenMode.Edit)
            {
                foreach (MatrixField mRow in model.MatrixRow.MatrixFields)
                    namedRangeToRowField.Add(mRow.TargetNamedRange, mRow);

                foreach (MatrixField mCol in model.MatrixColumn.MatrixFields)
                {
                    namedRangeToColField.Add(mCol.TargetNamedRange, mCol);

                    namedRangeToGroupedColField[mCol.TargetNamedRange] = mCol;
                    foreach (MatrixField groupedField in mCol.MatrixGroupedFields)
                        namedRangeToGroupedColField.Add(groupedField.TargetNamedRange, groupedField);
                }
            }

            if (View != null)
            {
                View.SetController(this);
                LoadCustomTaskPane(View, TaskPaneName);
            }
        }

        internal FormOpenMode OpenMode
        {
            get
            {
                return Mode;
            }
        }

        /// <summary>
        /// Load Custom task pane with Matrix Map
        /// </summary>
        /// <param name="View"></param>
        /// <param name="title"></param>
        public void LoadCustomTaskPane(UserControl View, string title)
        {
            CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes.Add(View, title, Globals.ThisAddIn.Application.ActiveWindow);
            ctp.VisibleChanged += VisibleChangeEvent;
            ctp.Width = 640;
            ctp.Visible = true;
        }

        /// <summary>
        /// Close task pane
        /// </summary>
        public void Close()
        {
            TaskPaneHelper.RemoveCustomPane(TaskPaneName);
        }


        /// <summary>
        /// Get intersecting row and column id, for a given data field 
        /// this function is used get corresponding row and column of a given matrix data field
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="rowId"></param>
        /// <param name="colId"></param>
        public void GetIntersectingRowAndColumnId(int row, int col, out Guid rowId, out Guid colId)
        {
            colId = rowId = Guid.Empty;

            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToRowField)
            {
                if (ExcelHelper.GetRange(pair.Key).Row == row)
                {
                    rowId = pair.Value.Id;
                    break;
                }
            }

            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToColField)
            {
                if (ExcelHelper.GetRange(pair.Key).Column == col )
                {
                    colId = pair.Value.Id;
                    break;
                }
            }
        }

        public MatrixField GetRowMatrixField(int row, int column)
        {
            MatrixField field = null;
            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToRowField)
            {
                if (ExcelHelper.GetRange(pair.Key).Row == row && ExcelHelper.GetRange(pair.Key).Column == column)
                {
                    field = pair.Value;
                    break;
                }
            }
            return field;
        }


        public MatrixField GetColumnMatrixField(int column, int row)
        {
            MatrixField field = null;
            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToColField)
            {
                if (ExcelHelper.GetRange(pair.Key).Column == column && ExcelHelper.GetRange(pair.Key).Row == row)
                {
                    field = pair.Value;
                    break;
                }
            }
            return field;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public MatrixField GetIntersectingRowMatrixField(int row)
        {
            MatrixField field = null;
            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToRowField)
            {
                if (ExcelHelper.GetRange(pair.Key).Row == row )
                {
                    field = pair.Value;
                    break;
                }
            }
            return field;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public MatrixField GetIntersectingColumnMatrixField(int column)
        {
            MatrixField field = null;
            foreach (KeyValuePair<string, MatrixField> pair in namedRangeToColField)
            {
                if (ExcelHelper.GetRange(pair.Key).Column == column)
                {
                    field = pair.Value;
                    break;
                }
            }
            return field;
        }

        public MatrixComponent GetMatrixComponentFromDataField(MatrixDataField dataField)
        {
            return Model.MatrixComponents.Where(cmp => cmp.Id.Equals(dataField.MatrixComponentId)).FirstOrDefault();
        }

        public MatrixDataField GetMatrixDataFieldByRowColumnId(Guid rowId, Guid colId)
        {
            return Model.MatrixData.MatrixDataFields.Where(df => df.MatrixRowId.Equals(rowId) && df.MatrixColumnId.Equals(colId)).FirstOrDefault();
        }

        public MatrixField GetMatrixFieldFromColumnId(Guid columnId)
        {
            return Model.MatrixColumn.MatrixFields.Where(col => col.Id.Equals(columnId)).FirstOrDefault();
        }

        public MatrixField GetMatrixFieldFromRowId(Guid rowId)
        {
            return Model.MatrixRow.MatrixFields.Where(row => row.Id.Equals(rowId)).FirstOrDefault();
        }

        public MatrixField CloneMatrixFieldFromExistingField(MatrixField field, string targetLocation, string targetNamedRange)
        {
            MatrixField newField = new MatrixField();

            newField.Id = Guid.NewGuid();
            newField.AppObjectUniqueID = field.AppObjectUniqueID;
            newField.DataType = field.DataType;
            newField.FieldId = field.FieldId;
            newField.FieldName = field.FieldName;
            newField.RenderingType = field.RenderingType;
            newField.ValueType = field.ValueType;
            newField.TargetLocation = targetLocation;
            newField.TargetNamedRange = targetNamedRange;
            newField.SortFieldId = field.SortFieldId;

            return newField;
        }

        public ApttusObject GetApttusObjectFromMatrixField(MatrixField field)
        {
            return ApplicationDefinitionManager.GetInstance.GetAppObject(field.AppObjectUniqueID);
        }

        /// <summary>
        /// Validate data field if it has corresponding row and column fields
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="DataFieldAppObjectId"></param>
        /// <returns></returns>
        public bool ValidateDataField(int row, int column, Guid DataFieldAppObjectId)
        {
            MatrixField rowField = GetIntersectingRowMatrixField(row);
            MatrixField columnField = GetIntersectingColumnMatrixField(column);

            ApttusObject rowObj = GetApttusObjectFromMatrixField(rowField);
            ApttusObject colObj = GetApttusObjectFromMatrixField(columnField);
            ApttusObject dataObj = ApplicationDefinitionManager.GetInstance.GetAppObject(DataFieldAppObjectId);

            if (rowObj != null && colObj != null && dataObj != null)
            {
                //1. First Validation-Check whether all 3 belong to same object
                if (rowObj.UniqueId == dataObj.UniqueId && colObj.UniqueId == dataObj.UniqueId)
                    return true;
                else
                {
                    bool bDataFieldHasRowRelation = rowObj.UniqueId == dataObj.UniqueId;
                    if (!bDataFieldHasRowRelation) //If row and data objects are not equal, check whether the data object contains 
                    {
                        var RowLookups = dataObj.Fields.Where(fld => fld.LookupObject != null && fld.Datatype == Datatype.Lookup && fld.LookupObject.Id.Equals(rowObj.Id)).Select(field => field.Name).ToList();
                        bDataFieldHasRowRelation = RowLookups.Count > 0;
                    }
                    bool bDataFieldHasColumnRelation = colObj.UniqueId == dataObj.UniqueId;
                    if (!bDataFieldHasColumnRelation)
                    {
                        var ColumnLookups = dataObj.Fields.Where(fld => fld.LookupObject != null && fld.Datatype == Datatype.Lookup && fld.LookupObject.Id.Equals(colObj.Id)).Select(field => field.Name).ToList();
                        bDataFieldHasColumnRelation = ColumnLookups.Count > 0;
                    }

                    return bDataFieldHasRowRelation && bDataFieldHasColumnRelation;
                }
            }
            return false;
        }

        public List<string> GetLookupValues(Guid dataFieldAppObjId, MatrixEntity entity, Guid Id)
        {
            ApttusObject dataObj = ApplicationDefinitionManager.GetInstance.GetAppObject(dataFieldAppObjId);
            string lookupId = string.Empty;
            switch (entity)
            {
                case MatrixEntity.Row:
                    MatrixField rowField = GetMatrixFieldFromRowId(Id);
                    lookupId = GetApttusObjectFromMatrixField(rowField).Id;
                    break;
                case MatrixEntity.Column:
                    MatrixField colField = GetMatrixFieldFromColumnId(Id);
                    lookupId = GetApttusObjectFromMatrixField(colField).Id;
                    break;
            }
            return dataObj.Fields.Where(fld => fld.LookupObject != null && fld.Datatype == Datatype.Lookup && fld.LookupObject.Id.Equals(lookupId)).Select(field => field.Name).ToList();
        }

        public void AddMatrixFieldToRow(MatrixField rowField)
        {
            namedRangeToRowField[rowField.TargetNamedRange] = rowField;
            Model.MatrixRow.MatrixFields.Add(rowField);
        }

        public void AddMatrixFieldToColumn(MatrixField colField)
        {
            namedRangeToColField[colField.TargetNamedRange] = colField;
            namedRangeToGroupedColField[colField.TargetNamedRange] = colField;
            Model.MatrixColumn.MatrixFields.Add(colField);
        }

        public void AddMatrixFieldAsDataField(MatrixDataField dataField)
        {
            Model.MatrixData.MatrixDataFields.Add(dataField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="field"></param>
        public void RemoveMatrixField(MatrixEntity objType, MatrixField field)
        {
            switch (objType)
            {
                case MatrixEntity.Column:
                    namedRangeToColField.Remove(field.TargetNamedRange);
                    namedRangeToGroupedColField.Remove(field.TargetNamedRange);
                    Model.MatrixColumn.MatrixFields.RemoveAll(fld => fld.Id.Equals(field.Id));
                    break;
                case MatrixEntity.Data:
                    Model.MatrixData.MatrixDataFields.RemoveAll(fld => fld.Id.Equals(field.Id));
                    break;
                case MatrixEntity.Row:
                    namedRangeToRowField.Remove(field.TargetNamedRange);
                    Model.MatrixRow.MatrixFields.RemoveAll(fld => fld.Id.Equals(field.Id));
                    break;
                case MatrixEntity.Individual:
                    Model.IndependentFields.Remove(field as MatrixIndividualField);
                    break;
                case MatrixEntity.GroupedColumn:
                    Excel.Range groupedFieldRange = ExcelHelper.GetRange(field.TargetNamedRange);
                    MatrixField sibling = FindClosestSibling(groupedFieldRange.Row, groupedFieldRange.Column);
                    if (sibling != null)
                        sibling.MatrixGroupedParentId = Guid.Empty;
                    MatrixField colField = GetIntersectingColumnMatrixField(groupedFieldRange.Column);
                    colField.MatrixGroupedFields.RemoveAll(fld => fld.Id == field.Id);
                    namedRangeToGroupedColField.Remove(field.TargetNamedRange);
                    break;
            }
        }

        internal void RemoveMatrixComponent(Guid componentId)
        {
            Model.MatrixComponents.RemoveAll(cmp => cmp.Id.Equals(componentId));
        }

        public bool IsRowBoundedToDataField(MatrixField field)
        {
            return Model.MatrixData.MatrixDataFields.Count > 0 && Model.MatrixData.MatrixDataFields.Where(fld => fld.MatrixRowId.Equals(field.Id)).Count() > 0;
        }

        public bool IsColumnBoundedToDataField(MatrixField field)
        {
            return Model.MatrixData.MatrixDataFields.Count > 0 && Model.MatrixData.MatrixDataFields.Where(fld => fld.MatrixColumnId.Equals(field.Id)).Count() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool CanRemoveMatrixField(MatrixEntity entity, MatrixField field)
        {
            bool bCanRemove = false;
            switch (entity)
            {
                case MatrixEntity.Row:
                    bCanRemove = !IsRowBoundedToDataField(field);
                    break;
                case MatrixEntity.Column:
                    bCanRemove = !IsColumnBoundedToDataField(field);
                    break;
                case MatrixEntity.GroupedColumn:
                    bCanRemove = CanRemoveGroupedField(field);
                    break;
            }
            return bCanRemove;
        }

        public bool CanRemoveGroupedField(MatrixField field)
        {
            return (from pair in namedRangeToGroupedColField
                    where pair.Value.MatrixGroupedParentId == Guid.Empty
                    && pair.Key == field.TargetNamedRange
                    && ExcelHelper.GetRange(pair.Key).Column == ExcelHelper.GetRange(field.TargetNamedRange).Column
                    select pair).Count() > 0;
        }

        public bool CanRemoveMatrixComponent(Guid componentId)
        {
            return Model.MatrixData.MatrixDataFields.Where(fld => fld.MatrixComponentId.Equals(componentId)).Count() == 0;
        }

        /// <summary>
        /// Add Matrix map to Application 
        /// </summary>
        /// <param name="mapName"></param>
        public void Save(string mapName)
        {
            Model.Name = mapName;
            ConfigurationManager.GetInstance.AddMatrixMap(Model);
        }

        internal MatrixComponent GetMatrixComponentById(Guid componentId)
        {
            return Model.MatrixComponents.Where(cmp => cmp.Id.Equals(componentId)).FirstOrDefault();
        }

        public bool CreateMatrixComponent(Guid AppObjectUniqueID, string componentName, out Guid componentId)
        {
            componentId = Guid.Empty;

            bool bCreateNew = false;
            MatrixComponent component = Model.MatrixComponents.Where(cmp => cmp.Name.Equals(componentName)).FirstOrDefault();

            if (component == null)
            {
                bCreateNew = true;
                component = new MatrixComponent();
                component.AppObjectUniqueID = AppObjectUniqueID;
                component.Name = componentName;
                Model.MatrixComponents.Add(component);
            }

            componentId = component.Id;

            return bCreateNew;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibleChangeEvent(object sender, EventArgs e)
        {
            CustomTaskPane taskPane = sender as CustomTaskPane;

            if (taskPane != null)
            {
                if (taskPane.Visible == false)
                {
                    if (View.highlightedComponentId != Guid.Empty)
                        View.UnHighlightComponent(View.highlightedComponentId);

                    Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
                    taskPane.Dispose();
                    taskPane = null;
                }
            }
        }

        internal List<MatrixField> GetMatrixFieldsByComponentId(Guid componentId)
        {
            List<MatrixField> returnFields = new List<MatrixField>();

            MatrixComponent currentComponent = GetMatrixComponentById(componentId);

            if (currentComponent != null)
            {
                string matrixComponentName = currentComponent.Name;
                List<MatrixDataField> matrixDataFields = Model.MatrixData.MatrixDataFields.Where(df => df.Name.Equals(matrixComponentName)).ToList();

                returnFields.AddRange(matrixDataFields.ToList<MatrixField>());

                foreach (MatrixDataField dataField in matrixDataFields)
                {
                    returnFields.Add(Model.MatrixRow.MatrixFields.Where(rf => rf.Id.Equals(dataField.MatrixRowId)).FirstOrDefault());
                    returnFields.Add(Model.MatrixColumn.MatrixFields.Where(cf => cf.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault());
                }
            }
            return returnFields;
        }

        internal Guid GetMatrixComponentIdFromDataFieldNamedRanged(string namedRange)
        {
            return Model.MatrixData.MatrixDataFields.Where(df => df.TargetNamedRange.Equals(namedRange)).Select(df => df.MatrixComponentId).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ApttusObject> GetObjects()
        {
            return ApplicationDefinitionManager.GetInstance.GetAllObjects();
        }

        internal void AddIndividualField(MatrixIndividualField field)
        {
            Model.IndependentFields.Add(field);
        }

        internal void UpdateTargetLocations()
        {
            for (int i = 0; i < namedRangeToColField.Count; i++)
            {
                var field = namedRangeToColField.ElementAt(i);
                string address = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.Key));
                field.Value.TargetLocation = address;
            }
            for (int i = 0; i < namedRangeToRowField.Count; i++)
            {
                var field = namedRangeToRowField.ElementAt(i);
                string address = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.Key));
                field.Value.TargetLocation = address;
            }
            for (int i = 0; i < Model.MatrixData.MatrixDataFields.Count; i++)
            {
                var field = Model.MatrixData.MatrixDataFields.ElementAt(i);
                string address = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));
                field.TargetLocation = address;
            }
        }

        internal MatrixField FindClosestSibling(int Row,int Col)
        {
            UpdateTargetLocations();
            MatrixField siblingColumnField = null;
            int lastDiff = 999999;
            for (int i = 0; i < namedRangeToGroupedColField.Count; i++)
            {
                var field = namedRangeToGroupedColField.ElementAt(i);

                Excel.Range fieldRange = ExcelHelper.GetRange(field.Key);
                int RowInd = fieldRange.Row;

                if (fieldRange.Column == Col && ((RowInd - Row - 1) <= lastDiff && (RowInd - Row - 1) >= 0))
                {
                    lastDiff = ExcelHelper.GetRowIndexByTargetNamedRange(field.Key) - Row;
                    siblingColumnField = field.Value;
                }
            }
            return siblingColumnField;
        }

        internal GroupedColumnValidation ValidateGroupedField(int Row, int Column, Guid groupedColFieldAppObjectId, string FieldId)
        {
            if (namedRangeToGroupedColField.Where(fld => fld.Value.FieldId == FieldId && groupedColFieldAppObjectId == fld.Value.AppObjectUniqueID && ExcelHelper.GetRange(fld.Key).Column == Column).Count() > 0)
                return GroupedColumnValidation.GroupedFieldDuplication;
            
            MatrixField siblingColumnField = FindClosestSibling(Row, Column);
            if (siblingColumnField.RenderingType == MatrixRenderingType.Static)
                return GroupedColumnValidation.ColumnTypeIsStatic;

            // If Sibling Cell Exists
            if (siblingColumnField != null)
            {
                ApttusObject groupedFieldObj = ApplicationDefinitionManager.GetInstance.GetAppObject(groupedColFieldAppObjectId);
                ApttusObject siblingColumnObj = GetApttusObjectFromMatrixField(siblingColumnField);

                if (siblingColumnObj != null && groupedFieldObj != null)
                {
                    //1. First Validation-Check whether all 2 belong to same object
                    if (groupedFieldObj.UniqueId == siblingColumnObj.UniqueId)
                        return GroupedColumnValidation.Success;
                    else
                    {
                        bool bGroupedFieldHasColRelation = false;
                        var groupedColLookups = siblingColumnObj.Fields.Where(fld => fld.LookupObject != null && fld.Datatype == Datatype.Lookup && fld.LookupObject.Id.Equals(groupedFieldObj.Id)).Select(field => field.Name);
                        bGroupedFieldHasColRelation = groupedColLookups.Count() > 0;
                        return bGroupedFieldHasColRelation ? GroupedColumnValidation.Success : GroupedColumnValidation.NoRelationExistsWithSiblingField;
                    }
                }

            }
            return GroupedColumnValidation.NoRelationExistsWithSiblingField; 
        }

        internal void AddMatrixGroupedFieldToColumn(MatrixField groupedField)
        {
            namedRangeToGroupedColField[groupedField.TargetNamedRange] = groupedField;
            Excel.Range groupedFieldRange = ExcelHelper.GetRange(groupedField.TargetNamedRange);
            MatrixField colField = FindClosestSibling(groupedFieldRange.Row, groupedFieldRange.Column);
            if (colField != null)
            {
                groupedField.MatrixGroupedParentId = colField.MatrixGroupedParentId;
                colField.MatrixGroupedParentId = groupedField.Id;
            }
        }
    }
}
