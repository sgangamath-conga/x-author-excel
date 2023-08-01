/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner;
using Apttus.XAuthor.Core;
using Microsoft.Office.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public class SaveMapController
    {
        public SaveMap Model { get; set; }
        public List<SaveFieldBound> SaveFieldBoundModel { get; set; }
        private SaveMapView View;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance; 
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private FormOpenMode formOpenMode;

        // For Dirty check
        public List<SaveField> origFields = null;

        public SaveMapController(SaveMap model, SaveMapView view, FormOpenMode formOpenMode, List<SaveField> originalFields = null, List<SaveFieldBound> saveBoundFields = null)
        {
            // 1. Set the Model
            this.Model = model;
            // 2. Set the view
            this.View = view;
            // Set Form Mode
            this.formOpenMode = formOpenMode;
            // Set Original fields collection
            this.origFields = originalFields;
            // Set save bound fields
            this.SaveFieldBoundModel = saveBoundFields;

            if (view != null)
            {
                // add cached field on click of Save
                if (originalFields == null)
                    this.AddFieldsToCachedList(model);

                this.View.SetController(this);
            }
        }

        private void AddFieldsToCachedList(SaveMap map)
        {
            // get list of Fields and save in Global var
            if (origFields == null)
                origFields = new List<SaveField>();

            if (map != null)
            {
                map.SaveFields.ForEach((item) =>
                {
                    if (!string.IsNullOrEmpty(item.TargetNamedRange) && !origFields.Exists(e => e.TargetNamedRange.Equals(item.TargetNamedRange)))
                        origFields.Add((SaveField)item.Clone());
                });
            }
        }

        private bool IsSaveMapDirty()
        {
            bool bIsDirty = false;

            if (Model.SaveFields.Count > origFields.Count)
            {
                bIsDirty = true;
                return bIsDirty;
            }
            else if (origFields.Count == Model.SaveFields.Count)
            {
                for (int i = 0; i < Model.SaveFields.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Model.SaveFields[i].TargetNamedRange))
                    {
                        bIsDirty = !origFields.Exists(o => o.TargetNamedRange.Equals(Model.SaveFields[i].TargetNamedRange));
                        if (bIsDirty)
                            return bIsDirty;
                    }
                }
            }

            return bIsDirty;
        }

        private void RemoveNonSavedFields()
        {
            int fieldCount = Model.SaveFields.Count;
            List<SaveField> removeFields = new List<SaveField>();

            while (fieldCount > 0)
            {
                SaveField currentField = Model.SaveFields[fieldCount - 1];
                if (!origFields.Exists(o => o.TargetNamedRange.Equals(currentField.TargetNamedRange)))
                {
                    removeFields.Add(currentField);
                    Model.SaveFields.Remove(currentField);
                    if (currentField.SaveFieldType == SaveType.SaveOnlyField)
                        ClearSaveField(currentField);
                }

                fieldCount--;
            }

            // Copied from removed fields
            List<Guid> groupIDs = removeFields.GroupBy(s => s.GroupId).Select(field => field.Key).ToList();
            groupIDs.RemoveAll(guid => guid.Equals(Guid.Empty)); //In case of independent fields Guid will be Empty, hence remove it.

            foreach (Guid groupID in groupIDs)
            {
                List<SaveField> saveFields = Model.SaveFields.Where(field => field.GroupId.Equals(groupID)).ToList();
                if (saveFields.Count == 0)
                {
                    SaveGroup saveGroup = Model.SaveGroups.Where(saveGroupObj => saveGroupObj.GroupId.Equals(groupID)).FirstOrDefault();
                    if (saveGroup != null)
                    {
                        //Remove the SaveGroup from SaveGroups List.
                        Model.SaveGroups.Remove(saveGroup);

                        //Clear the NamedRange associated with this SaveGroup.
                        if (!IsNamedRangePartOfRetrieveMap(saveGroup.TargetNamedRange))
                            ClearSaveGroup(saveGroup);
                    }
                    else
                    {
                        //If it comes here it must be an error.
                    }
                }
                else
                {
                    //Re-evaluate the Named Range within the fields of this SaveGroup
                    SaveGroup saveGroup = Model.SaveGroups.Where(saveGroupObj => saveGroupObj.GroupId.Equals(groupID)).FirstOrDefault();
                    if (!IsNamedRangePartOfRetrieveMap(saveGroup.TargetNamedRange))
                        ApplyNamedRange(saveGroup, saveFields);
                }
            }

        }

        public void LoadRetrieveFields()
        {
            SaveMapRetrieveFieldView retrieveFieldView = new SaveMapRetrieveFieldView();

            //Below variable (retrieveFieldModel) is not used. Hence commented it.
            //List<SaveMapRetrieveField> retrieveFieldModel = new List<SaveMapRetrieveField>();
            SaveMapRetrieveFieldController retrieveFieldController = new SaveMapRetrieveFieldController(Model, retrieveFieldView);
            retrieveFieldController.SetView(Model);
            DialogResult dr = retrieveFieldView.ShowDialog();
            if (dr == DialogResult.OK)
                this.BindView(Model.SaveFields);
        }

        public void LoadMatrixFields()
        {
            SaveMapMatrixFieldView matrixFieldView = new SaveMapMatrixFieldView();

            SaveMapMatrixFieldController matrixFieldController = new SaveMapMatrixFieldController(Model, matrixFieldView);
            matrixFieldController.SetView(Model);
            DialogResult dr = matrixFieldView.ShowDialog();
            if (dr == DialogResult.OK)
                this.BindView(Model.SaveFields);
        }

        public void AddSaveFields(string saveMapName)
        {
            TaskPaneHelper.Visible(Constants.SAVEMAP_NAME, false);

            ucRetrieveMap ucRetrieveMapView = new ucRetrieveMap();
            RetrieveMapController controller = new RetrieveMapController(ucRetrieveMapView, Model, this, resourceManager.GetResource("SAVEMAPVIEW_SubTitle"));
            ucRetrieveMapView.SetController(controller);

            controller.Initialize(null, MapMode.SaveMap, saveMapName);
        }

        internal void LoadControls(string saveMapName = "")
        {
            TaskPaneHelper.LoadCustomTaskPane(View, resourceManager.GetResource("RIBBON_btnSaveMap_Label"));

            switch (formOpenMode)
            {
                case FormOpenMode.Create:
                    break;
                case FormOpenMode.Edit:
                    View.LoadControls(saveMapName, Model.Name);
                    this.BindView(Model.SaveFields);
                    break;
                case FormOpenMode.ReadOnly:
                    break;
            }
        }

        private string GetFieldName(SaveField field)
        {
            if (field.SaveFieldType == SaveType.MatrixField)
                return field.FieldId;
            else
            {
                if (string.IsNullOrEmpty(field.SaveFieldName))
                    return applicationDefinitionManager.GetFullFieldName(field.AppObject, field.FieldId);
                return field.SaveFieldName;
            }
        }

        public List<SaveFieldBound> GenerateBindingList(List<SaveField> list)
        {
            List<SaveFieldBound> result = (from f in list
                                           select new SaveFieldBound
                                           {
                                               GroupId = f.GroupId,
                                               AppObject = f.AppObject,
                                               FieldId = f.FieldId,
                                               FieldName = GetFieldName(f),
                                               Type = f.Type,
                                               Included = false,
                                               SaveCondition = null,
                                               SaveConditionText = resourceManager.GetResource("COMMON_NoCondition_Text"),
                                               DesignerLocation = f.DesignerLocation,
                                               TargetNamedRange = f.TargetNamedRange,
                                               TargetColumnIndex = f.TargetColumnIndex,
                                               CrossTab = f.CrossTab,
                                               SaveFieldType = f.SaveFieldType,
                                               LookAheadProp = (f == null ? null : f.LookAheadProp),
                                               LookAheadProps = (f.LookAheadProps == null ? null : f.LookAheadProps ),
                                               MatrixMapId = f.MatrixMapId,
                                               MatrixComponentId = f.MatrixComponentId,
                                               SaveFieldName = f.SaveFieldName,
                                               MultiLevelFieldId = f.MultiLevelFieldId
                                           }).OrderBy(sfb=>sfb.FieldName).ToList();

            return result;
        }

        internal void UpdateRows(SaveGroup saveGroup, int RowsToAdd)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                //Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                Excel.Range NamedRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);
                if (NamedRange != null)
                {
                    DesignerEditActionController designerEditActionController = new DesignerEditActionController(NamedRange);
                    designerEditActionController.UpdateRows(saveGroup, RowsToAdd);
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message, resourceManager.GetResource("SAVEMAPCTL_UpdateRows_ErrMsg"));
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        internal void Save(string name, bool silent)
        {
            // 1. Save Map Name
            Model.Name = name;
            // 2. Redundant Fields - Added for Backward Compatibility
            Model.AttachAppObjectUniqueId = Guid.Empty;
            Model.Filename = string.Empty;
            Model.Extension = string.Empty;
            Model.AppendTimestamp = false;

            // 3. Update Save Fields
            if (SaveFieldBoundModel != null)
                Model.SaveFields = SaveFieldBoundModel.Cast<SaveField>().ToList<SaveField>();

            // 4. One Time - Add Save Map to Config
            if (Model.Id == Guid.Empty)
                configurationManager.AddSaveMap(Model);

            // 5. Quick App does not enter this block.
            if (View != null)
            {
                // Update cached field on click of Save
                this.AddFieldsToCachedList(Model);

                View.RefreshLookAheadIcons();
                if (!silent)
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("SAVEMAPCTL_Save_InfoMsg"), Constants.SAVEMAP_NAME);
            }
        }

        internal void RemoveFields()
        {
            /*List<SaveField> removeFields = (from sf in Model.SaveFields
                                            join sfb in SaveFieldBoundModel
                                            on new { sf.AppObject, sf.FieldId } equals new { sfb.AppObject, sfb.FieldId }
                                            where sfb.Included == true & (sfb.Type == ObjectType.Independent || sfb.Type == ObjectType.Repeating)
                                            select sf).ToList();*/
            //changed to use appObject and designer location to check for field equality. Assumes designer location is unique to every save field. Fixes jira ticket: AB-1106
            //changed to use appObject and designer location to check for field equality. Assumes designer location is unique to every save field
            List<SaveField> removeFields = (from sf in Model.SaveFields
                                            join sfb in SaveFieldBoundModel
                                            on new { sf.AppObject, sf.DesignerLocation } equals new { sfb.AppObject, sfb.DesignerLocation }
                                            where sfb.Included == true & (sfb.Type == ObjectType.Independent || sfb.Type == ObjectType.Repeating)
                                            select sf).ToList();

            if (Model.SaveFields.Exists(sf => sf.Type == ObjectType.CrossTab))
            {
                List<SaveField> removeCrossTabFields = (from sf in Model.SaveFields
                                                        join sfb in SaveFieldBoundModel
                                                        on sf.CrossTab.Id equals sfb.CrossTab.Id
                                                        where sfb.Included == true & sfb.Type == ObjectType.CrossTab
                                                        select sf).ToList();

                removeFields.AddRange(removeCrossTabFields);
            }

            //Create Collection of SaveGroup which are part of SaveField
            List<Guid> groupIDs = removeFields.GroupBy(s => s.GroupId).Select(field => field.Key).ToList();
            groupIDs.RemoveAll(guid => guid.Equals(Guid.Empty)); //In case of independent fields Guid will be Empty, hence remove it.

            foreach (SaveField removeField in removeFields)
            {
                Model.SaveFields.Remove(removeField);

                if (removeField.SaveFieldType == SaveType.SaveOnlyField)
                    ClearSaveField(removeField);
            }

            foreach (Guid groupID in groupIDs)
            {
                List<SaveField> saveFields = Model.SaveFields.Where(field => field.GroupId.Equals(groupID)).ToList();
                if (saveFields.Count == 0)
                {
                    SaveGroup saveGroup = Model.SaveGroups.Where(saveGroupObj => saveGroupObj.GroupId.Equals(groupID)).FirstOrDefault();
                    if (saveGroup != null)
                    {
                        //Remove the SaveGroup from SaveGroups List.
                        Model.SaveGroups.Remove(saveGroup);

                        //Clear the NamedRange associated with this SaveGroup.
                        if (!IsNamedRangePartOfRetrieveMap(saveGroup.TargetNamedRange))
                            ClearSaveGroup(saveGroup);
                    }
                    else
                    {
                        //If it comes here it must be an error.
                    }
                }
                else
                {
                    //Re-evaluate the Named Range within the fields of this SaveGroup
                    SaveGroup saveGroup = Model.SaveGroups.Where(saveGroupObj => saveGroupObj.GroupId.Equals(groupID)).FirstOrDefault();
                    if (!IsNamedRangePartOfRetrieveMap(saveGroup.TargetNamedRange))
                        ApplyNamedRange(saveGroup, saveFields);
                }
            }

            this.BindView(Model.SaveFields);
        }

        internal void Close()
        {
            if (IsSaveMapDirty())
            {
                TaskDialogResult dResult = ApttusMessageUtil.ShowWarning(resourceManager.GetResource("COMMONS_AreYouSure_ShowMsg"),
                                                                            Constants.DESIGNER_PRODUCT_NAME,
                                                                            ApttusMessageUtil.YesNo);
                if (dResult == TaskDialogResult.Yes)
                {
                    // Revert changes
                    RemoveNonSavedFields();
                }
                else if (dResult == TaskDialogResult.No)
                {
                    return;
                }
            }

            TaskPaneHelper.RemoveCustomPane(resourceManager.GetResource("RIBBON_btnSaveMap_Label"));
        }

        public void UpdateModel(List<SaveField> fields)
        {
            SaveFieldBoundModel = GenerateBindingList(fields);
        }

        public void BindView(List<SaveField> fields)
        {
            // Bind Grid
            UpdateModel(fields);
            View.LoadSaveFieldsGrid(SaveFieldBoundModel);
            // Bind Independent Objects dropdown
            //View.RenderIndependentObjects(Model.AttachAppObjectUniqueId);
        }

        public List<ApttusObject> GetIndependentObjects()
        {
            List<ApttusObject> IndependentObjects = new List<ApttusObject>();
            var IndependentObjectGuids = (from sf in Model.SaveFields
                                          where sf.Type == ObjectType.Independent
                                          select sf.AppObject).Distinct();
            IndependentObjects = (from o in applicationDefinitionManager.GetAllObjects()
                                  where IndependentObjectGuids.Contains(o.UniqueId)
                                  select o).ToList();

            return IndependentObjects;
        }

        public List<SaveMapRepeatingObject> GetRepeatingObjects()
        {
            var repeatingObjects = (from sg in Model.SaveGroups
                                    select new SaveMapRepeatingObject
                                    {
                                        SaveGroupId = sg.GroupId,
                                        Name = applicationDefinitionManager.GetAppObject(sg.AppObject).Name,
                                        LoadedRows = sg.LoadedRows
                                    }).ToList();

            return repeatingObjects;
        }

        /// <summary>
        /// Clears the NamedRange associated with this Save Group
        /// </summary>
        /// <param name="saveGroup"></param>
        private void ClearSaveGroup(SaveGroup saveGroup)
        {
            try
            {
                ExcelHelper.RemoveNamedRange(saveGroup.TargetNamedRange, true);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Clears the Label of the SaveOther Field
        /// </summary>
        /// <param name="sLayout"></param>
        /// <param name="sTargetNamedRange"></param>
        private void ClearLabel(String sLayout, string sTargetNamedRange)
        {
            bool bIsVerticalLayout = sLayout.Equals("Vertical");

            Excel.Range oFieldRange = ExcelHelper.GetRange(sTargetNamedRange);

            Excel.Range oLabelRange = null;

            if (bIsVerticalLayout)
                oLabelRange = ExcelHelper.NextVerticalCell(oFieldRange, -1);
            else
                oLabelRange = ExcelHelper.NextHorizontalCell(oFieldRange, -1);

            if (oLabelRange != null)
                oLabelRange.ClearContents();
        }

        /// <summary>
        /// Clears the Label as well as the NamedRange associated with this save field.
        /// </summary>
        /// <param name="saveField"></param>
        public void ClearSaveField(SaveField saveField)
        {
            try
            {
                switch (saveField.Type)
                {
                    case ObjectType.Repeating:
                        {
                            SaveGroup saveGroup = Model.SaveGroups.Where(sf => sf.GroupId.Equals(saveField.GroupId)).FirstOrDefault();
                            ClearLabel(saveGroup.Layout, saveField.TargetNamedRange);
                            break;
                        }
                    case ObjectType.Independent:
                        {
                            ClearLabel("Horizontal", saveField.TargetNamedRange);
                            break;
                        }
                }
                ExcelHelper.RemoveNamedRange(saveField.TargetNamedRange, true);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// determines whether the named range is part of any retrieve map within the App.
        /// </summary>
        /// <param name="sTargetNamedRange"></param>
        /// <returns></returns>
        private bool IsNamedRangePartOfRetrieveMap(string sTargetNamedRange)
        {
            return (from rMap in configurationManager.RetrieveMaps
                    from rg in rMap.RepeatingGroups
                    where rg.TargetNamedRange.Equals(sTargetNamedRange)
                    select rg.TargetNamedRange).ToList().Count > 0;
        }

        /// <summary>
        /// Extends or Contracts the named range after the save other fields are removed from the save group.
        /// </summary>
        /// <param name="saveGroup"></param>
        /// <param name="saveFields"></param>
        private void ApplyNamedRange(SaveGroup saveGroup, List<SaveField> saveFields)
        {
            bool bIsVerticalLayout = saveGroup.Layout.Equals("Vertical");
            if (bIsVerticalLayout)
            {
                saveFields.Sort(delegate(SaveField sf1, SaveField sf2)
                {
                    return (ExcelHelper.GetColumnIndexByTargetNamedRange(sf1.TargetNamedRange).CompareTo(ExcelHelper.GetColumnIndexByTargetNamedRange(sf2.TargetNamedRange)));
                });
            }
            else
            {
                saveFields.Sort(delegate(SaveField sf1, SaveField sf2)
                {
                    return (ExcelHelper.GetRowIndexByTargetNamedRange(sf1.TargetNamedRange).CompareTo(ExcelHelper.GetRowIndexByTargetNamedRange(sf2.TargetNamedRange)));
                });
            }

            SaveField rFieldFirst = saveFields.First();
            SaveField rFieldLast = saveFields.Last();

            Excel.Range firstCell = ExcelHelper.GetRange(rFieldFirst.TargetNamedRange);
            Excel.Range lastCell = ExcelHelper.GetRange(rFieldLast.TargetNamedRange);
            Excel.Worksheet sheet = firstCell.Worksheet;

            string sNamedRange = saveGroup.TargetNamedRange;

            Excel.Range preloadedGridRange = null;
            //For Preloaded Grids
            if (!string.IsNullOrEmpty(sNamedRange) && saveGroup != null && saveGroup.LoadedRows > 0)
            {
                preloadedGridRange = ExcelHelper.GetRange(sNamedRange);
                int nRows = preloadedGridRange.Rows.Count;
                if (nRows > 2)
                {
                    //We have a preloaded grid of data. 
                    Excel.Range newLastRowCell = ExcelHelper.NextVerticalCell(lastCell, nRows - 1);
                    lastCell = newLastRowCell;
                }
            }

            Excel.Range headerRange;
            if (saveGroup.Layout.Equals("Horizontal"))
                headerRange = sheet.Range[ExcelHelper.NextHorizontalCell(firstCell, -1), ExcelHelper.NextHorizontalCell(lastCell, -1)];
            else
                headerRange = sheet.Range[ExcelHelper.NextVerticalCell(firstCell, -1), ExcelHelper.NextVerticalCell(lastCell, -1)];

            if (saveGroup != null && saveGroup.LoadedRows > 0 && preloadedGridRange != null)
                ExcelHelper.RemoveBorderFromRange(preloadedGridRange);

            ExcelHelper.AssignNameToRange(headerRange, saveGroup.TargetNamedRange,true);

            foreach (SaveField sf in saveFields)
            {
                if (!bIsVerticalLayout)
                    sf.TargetColumnIndex = ExcelHelper.GetRowIndex(rFieldFirst.TargetNamedRange, sf.TargetNamedRange);
                else
                    sf.TargetColumnIndex = ExcelHelper.GetColumnIndex(rFieldFirst.TargetNamedRange, sf.TargetNamedRange);
            }
        }
    }
}
