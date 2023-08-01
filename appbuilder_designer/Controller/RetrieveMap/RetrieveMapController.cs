/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using Microsoft.Office.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public class RetrieveMapController
    {
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ucRetrieveMap View;
        private RetrieveMap retrieveMap;
        private FormOpenMode formOpenMode;
        private RetrieveMapCellValidator cellValidator;
        public MapMode mapMode;
        private string CurrentTaskPane = string.Empty;
        private SaveMap CurrentSaveMap;
        private SaveMapController CurrentSaveMapController;
        private string CurrentSaveMapName;
        private RepeatingGroupController repeatingCellMapController;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        // For Dirty check
        public List<RetrieveField> origFields = null;

        public RetrieveMapController(ucRetrieveMap ucView, SaveMap SaveMapModel, SaveMapController SMController, string MapName = "Display Map")
        {
            View = ucView;
            CurrentTaskPane = MapName;
            LoadCustomTaskPane(View, CurrentTaskPane);
            this.CurrentSaveMap = SaveMapModel;
            this.CurrentSaveMapController = SMController;
        }

        public RetrieveMapController(ucRetrieveMap ucView, string MapName = "Display Map")
        {
            View = ucView;
            CurrentTaskPane = resourceManager.GetResource("COMMON_DisplayMap_Text");
            if (View != null)
                LoadCustomTaskPane(View, CurrentTaskPane);
        }

        public void LoadCustomTaskPane(UserControl View, string title)
        {
            CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes.Add(View, title, Globals.ThisAddIn.Application.ActiveWindow);
            ctp.VisibleChanged += VisibleChangeEvent;
            ctp.Width = 640;
            ctp.Visible = true;
        }

        private void VisibleChangeEvent(object sender, EventArgs e)
        {
            CustomTaskPane taskPane = sender as CustomTaskPane;
            TaskDialogResult dResult = TaskDialogResult.None;

            if (taskPane != null)
            {
                if (taskPane.Visible == false)
                {
                    if (IsRetrieveMapDirty())
                    {
                        dResult = ApttusMessageUtil.ShowWarning(resourceManager.GetResource("COMMONS_AreYouSure_ShowMsg"), Constants.DESIGNER_PRODUCT_NAME, ApttusMessageUtil.YesNo);
                        if (dResult == TaskDialogResult.Yes)
                        {
                            // Revert changes
                            RemoveNonSavedFields();
                        }
                        else if (dResult == TaskDialogResult.No)
                        {
                            //taskPane.Visible = true;
                            //LoadCustomTaskPane(View, CurrentTaskPane);
                            if (taskPane != null)
                            {
                                Dispatcher.CurrentDispatcher.BeginInvoke(new System.Action(() => { taskPane.Visible = true; }));
                            }
                        }
                    }

                    if (dResult == TaskDialogResult.None || dResult == TaskDialogResult.Yes)
                    {
                        Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
                        taskPane.Dispose();
                        taskPane = null;
                    }
                }
            }
        }

        public bool IsRetrieveMapDirty()
        {
            bool bIsDirty = false;
            int currentFieldCount = retrieveMap.RetrieveFields.Count;

            foreach (RepeatingGroup rg in retrieveMap.RepeatingGroups)
            {
                currentFieldCount = currentFieldCount + rg.RetrieveFields.Count;
            }

            if (currentFieldCount > origFields.Count)
            {
                bIsDirty = true;
                return bIsDirty;
            }
            else if (currentFieldCount == origFields.Count)
            {
                // Independent
                for (int k = 0; k < retrieveMap.RetrieveFields.Count; k++)
                {
                    bIsDirty = !origFields.Exists(o => o.TargetNamedRange.Equals(retrieveMap.RetrieveFields[k].TargetNamedRange));
                    if (bIsDirty)
                        return bIsDirty;
                }

                // Repeating
                for (int g = 0; g < retrieveMap.RepeatingGroups.Count; g++)
                {
                    RepeatingGroup currentGroup = retrieveMap.RepeatingGroups[g];
                    for (int f = 0; f < currentGroup.RetrieveFields.Count; f++)
                    {
                        bIsDirty = !origFields.Exists(o => o.TargetNamedRange.Equals(currentGroup.RetrieveFields[f].TargetNamedRange));
                        if (bIsDirty)
                            return bIsDirty;
                    }
                }
            }

            return bIsDirty;
        }

        public void RemoveNonSavedFields()
        {
            Excel.Range oFieldRange = null;
            Excel.Range oLabelRange = null;

            // Remove Independent
            int indCount = retrieveMap.RetrieveFields.Count;
            //for (int i = 0; i <= indCount; i++)
            while (indCount > 0)
            {
                RetrieveField currentField = retrieveMap.RetrieveFields[indCount - 1];
                if (!origFields.Exists(o => o.TargetNamedRange.Equals(currentField.TargetNamedRange)))
                {
                    retrieveMap.RetrieveFields.Remove(currentField);
                    oFieldRange = ExcelHelper.GetRange(currentField.TargetNamedRange);
                    oLabelRange = ExcelHelper.NextHorizontalCell(oFieldRange, -1);
                    oFieldRange.ClearContents();
                    RemoveField(null, currentField);
                    oLabelRange.ClearContents();
                }

                indCount--;
            }
            //while (indCount > 0);

            // Remove Repeating
            for (int r = 0; r < retrieveMap.RepeatingGroups.Count; r++)
            {
                RepeatingGroup currentGroup = retrieveMap.RepeatingGroups[r];

                int repCount = currentGroup.RetrieveFields.Count;
                while (repCount > 0)
                {
                    RetrieveField currentRepField = currentGroup.RetrieveFields[repCount - 1];
                    if (!origFields.Exists(o => o.TargetNamedRange.Equals(currentRepField.TargetNamedRange)))
                    {
                        oFieldRange = ExcelHelper.GetRange(currentRepField.TargetNamedRange);
                        oLabelRange = ExcelHelper.NextVerticalCell(oFieldRange, -1);
                        oFieldRange.ClearContents();
                        RemoveField(currentGroup, currentRepField);
                        oLabelRange.ClearContents();
                    }

                    repCount--;
                }
                //while (repCount > 0);

            }
        }

        private void AddFieldsToCachedList(RetrieveMap map)
        {
            // get list of Fields and save in Global var
            if (origFields == null)
                origFields = new List<RetrieveField>();

            if (map != null)
            {
                // Add Independent fields
                map.RetrieveFields.ForEach((item) =>
                {
                    if (!origFields.Exists(e => e.TargetNamedRange.Equals(item.TargetNamedRange)))
                        origFields.Add((RetrieveField)item.Clone());
                });

                // Add Repeating fields
                map.RepeatingGroups.ForEach(((item) =>
                {
                    item.RetrieveFields.ForEach((itemField) =>
                    {
                        if (!origFields.Exists(e => e.TargetNamedRange.Equals(itemField.TargetNamedRange)))
                            origFields.Add((RetrieveField)itemField.Clone());
                    });
                }));
            }

        }

        /// <summary>
        /// Get All Standard Objects
        /// </summary>
        /// <returns></returns>
        public List<ApttusObject> GetAppObjects()
        {
            return applicationDefinitionManager.GetParentAndChildObjects(applicationDefinitionManager.GetAppObjects());
        }

        /// <summary>
        /// Updates the Save Map
        /// </summary>
        public void updateSaveMap()
        {
            AddSaveFields();
            //SaveMapController.BindView(CurrentSaveMap.SaveFields);
            this.Close(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save(string sRetrieveMapName)
        {
            if (retrieveMap == null)
                return;

            retrieveMap.Name = sRetrieveMapName;
            if (formOpenMode == FormOpenMode.Create)
            {
                if (retrieveMap.Id != Guid.Empty)
                    configurationManager.RetrieveMaps.Remove(retrieveMap);
                configurationManager.AddRetrieveMap(retrieveMap);
            }

            RetrieveMapValidator.GetInstance.Validate<RetrieveMap>(retrieveMap);

            // Update cached field on click of Save
            this.AddFieldsToCachedList(retrieveMap);

            UpdateUniqueTargetNamedRanges();
        }

        private void UpdateUniqueTargetNamedRanges()
        {
            List<Microsoft.Office.Interop.Excel.Worksheet> worksheets = new List<Microsoft.Office.Interop.Excel.Worksheet>();
            Dictionary<string, string> uniqueTargetNamedRanges = new Dictionary<string, string>();

            foreach (RetrieveField field in retrieveMap.RetrieveFields)
            {
                string sheetName = field.TargetLocation.Split(new char[] { '!' })[0];

                if (!uniqueTargetNamedRanges.ContainsKey(sheetName))
                {
                    uniqueTargetNamedRanges.Add(sheetName, field.TargetNamedRange);
                }
            }

            foreach (RepeatingGroup rg in retrieveMap.RepeatingGroups)
            {
                foreach (RetrieveField field in rg.RetrieveFields)
                {
                    string sheetName = field.TargetLocation.Split(new char[] { '!' })[0];

                    if (!uniqueTargetNamedRanges.ContainsKey(sheetName))
                    {
                        uniqueTargetNamedRanges.Add(sheetName, field.TargetNamedRange);
                    }
                }
            }

            retrieveMap.UniqueTargetNamedRanges = uniqueTargetNamedRanges.Values.ToList();
        }

        private void AddSaveFields()
        {
            // Add Independent Fields
            List<SaveField> NonRepeatingFields = (from f in retrieveMap.RetrieveFields
                                                  where f.Type != ObjectType.Repeating
                                                  select new SaveField
                                                  {
                                                      AppObject = f.AppObject,
                                                      FieldId = f.FieldId,
                                                      Type = f.Type,
                                                      SaveCondition = null,
                                                      DesignerLocation = f.TargetLocation,
                                                      TargetNamedRange = f.TargetNamedRange,
                                                      TargetColumnIndex = f.TargetColumnIndex,
                                                      SaveFieldType = SaveType.SaveOnlyField,
                                                      SaveFieldName = f.FieldName,
                                                      MultiLevelFieldId = f.MultiLevelFieldId
                                                  }).ToList();


            // Add Independent to Save Map
            CurrentSaveMap.SaveFields.AddRange(NonRepeatingFields);

            // Repeating section
            // 1. Create all Save Groups
            List<SaveGroup> SaveGroups = (from rg in retrieveMap.RepeatingGroups
                                          select new SaveGroup
                                          {
                                              GroupId = Guid.NewGuid(),
                                              AppObject = rg.AppObject,
                                              TargetNamedRange = rg.TargetNamedRange,
                                              Layout = rg.Layout
                                          }).ToList();

            // 1.1 Exclude Save Groups already existing in the Save Map
            SaveGroups.RemoveAll(sg => (from tnr in CurrentSaveMap.SaveGroups select tnr.TargetNamedRange).Contains(sg.TargetNamedRange));

            // 1.2 Add New and Unique Save Groups to Save Map
            CurrentSaveMap.SaveGroups.AddRange(SaveGroups);

            // 2. Add Repeating Fields for each Save Group
            List<SaveField> RepeatingFields = (from sg in CurrentSaveMap.SaveGroups
                                               from rg in retrieveMap.RepeatingGroups
                                               from f in rg.RetrieveFields
                                               where sg.AppObject == rg.AppObject & sg.TargetNamedRange == rg.TargetNamedRange
                                               select new SaveField
                                               {
                                                   GroupId = sg.GroupId,
                                                   AppObject = f.AppObject,
                                                   FieldId = f.FieldId,
                                                   Type = f.Type,
                                                   SaveCondition = null,
                                                   DesignerLocation = f.TargetLocation,
                                                   TargetNamedRange = f.TargetNamedRange,
                                                   TargetColumnIndex = f.TargetColumnIndex,
                                                   SaveFieldType = SaveType.SaveOnlyField,
                                                   SaveFieldName = f.FieldName,
                                                   MultiLevelFieldId = f.MultiLevelFieldId
                                               }).ToList();


            // 2.1 Add Repeating Save Fields to Save Map
            CurrentSaveMap.SaveFields.AddRange(RepeatingFields);


            // In case of look up fields we have Either Or scenario, 
            // i.e. If you add ID field of Lookup you cannot add Name field and vice versa.
            // This validation needs to happen on add of Save Fields.
            List<SaveField> lookupFieldToRemove = new List<SaveField>();
            ValidationManager.ValidationLookup(NonRepeatingFields, CurrentSaveMap.SaveFields, ref lookupFieldToRemove);
            if (lookupFieldToRemove.Count > 0)
            {
                foreach (SaveField field in lookupFieldToRemove)
                {
                    CurrentSaveMapController.ClearSaveField(field);
                    SaveField toRemove = NonRepeatingFields.Find(item => item.AppObject.Equals(field.AppObject) && item.FieldId.Equals(field.FieldId));
                    CurrentSaveMap.SaveFields.Remove(toRemove);
                    NonRepeatingFields.Remove(toRemove);
                }
            }
        }


        /// <summary>
        /// Depending on the MapMode, either the named range will be newly created or will get extended
        /// </summary>
        /// <param name="mapMode"></param>
        /// <param name="repeatingCellGroup"></param>
        public void ApplyNameRangeToRepeatingCells(MapMode mapMode, RepeatingGroup repeatingCellGroup)
        {
            if (repeatingCellGroup == null)
                return;

            string GroupbyField = repeatingCellGroup.GroupByField;

            List<RetrieveField> repeatingCellFields = repeatingCellGroup.RetrieveFields.Where(s => s.Type == ObjectType.Repeating).ToList();
            if (repeatingCellFields.Count() == 0)
                return;

            int targetRowOrColumn;
            string targetWorksheet = targetWorksheet = ExcelHelper.GetSheetNameByNamedRange(repeatingCellFields[0].TargetNamedRange); ;
            // Sort and Prepare Repeating Group
            if (repeatingCellGroup.Layout.Equals("Horizontal"))
            {
                repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                {
                    return (ExcelHelper.GetRowIndexByTargetNamedRange(rField1.TargetNamedRange).CompareTo(ExcelHelper.GetRowIndexByTargetNamedRange(rField2.TargetNamedRange)));
                });
                targetRowOrColumn = ExcelHelper.GetColumnIndexByTargetNamedRange(repeatingCellFields[0].TargetNamedRange);

            }
            else
            {
                repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                {
                    return (ExcelHelper.GetColumnIndexByTargetNamedRange(rField1.TargetNamedRange).CompareTo(ExcelHelper.GetColumnIndexByTargetNamedRange(rField2.TargetNamedRange)));
                });
                targetRowOrColumn = ExcelHelper.GetRowIndexByTargetNamedRange(repeatingCellFields[0].TargetNamedRange);
            }

            if (repeatingCellFields.Count() > 0 && !String.IsNullOrEmpty(GroupbyField) && !repeatingCellFields[0].FieldId.Equals(GroupbyField))
                MessageBox.Show(resourceManager.GetResource("RETRIEVEMAPCTL_Group_ShowMsg"), resourceManager.GetResource("RETRIEVEMAPCTL_GroupCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            string sNamedRange = string.Empty;

            // Create list of existing repeating fields for current repeating group.
            List<RetrieveField> ExistingSaveOtherfields = new List<RetrieveField>();
            List<RetrieveField> ExistingRetrievefields = new List<RetrieveField>();
            SaveGroup currentSaveGroup = null;

            if (mapMode == MapMode.SaveMap)
            {
                // Add Current save other fields if any
                if (CurrentSaveMap != null && CurrentSaveMap.SaveFields.Count > 0)
                {
                    currentSaveGroup = CurrentSaveMap.SaveGroups.Where(sg => sg.AppObject == repeatingCellGroup.AppObject).FirstOrDefault();
                    if (currentSaveGroup != null)
                    {
                        // Get all SaveOther type of Save Fields
                        ExistingSaveOtherfields = (from sf in CurrentSaveMap.SaveFields
                                                   where sf.GroupId == currentSaveGroup.GroupId && sf.SaveFieldType == SaveType.SaveOnlyField
                                                   select new RetrieveField
                                                   {
                                                       AppObject = sf.AppObject,
                                                       FieldId = sf.FieldId,
                                                       Type = sf.Type,
                                                       TargetLocation = sf.DesignerLocation,
                                                       TargetNamedRange = sf.TargetNamedRange,
                                                       TargetColumnIndex = sf.TargetColumnIndex
                                                   }).ToList();

                        if (ExistingSaveOtherfields.Count > 0)
                        {
                            sNamedRange = currentSaveGroup.TargetNamedRange;
                            repeatingCellFields.AddRange(ExistingSaveOtherfields.ToList());
                            repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                            {
                                return (ExcelHelper.GetColumnIndexByTargetNamedRange(rField1.TargetNamedRange).CompareTo(ExcelHelper.GetColumnIndexByTargetNamedRange(rField2.TargetNamedRange)));
                            });
                        }
                    }
                }

                // In Save Other Mode, add first repeating group's retrieve fields to repeatingCellFields collection
                foreach (RetrieveMap rm in configurationManager.RetrieveMaps)
                {
                    foreach (RepeatingGroup repGroup in rm.RepeatingGroups)
                    {
                        ExistingRetrievefields = (from f in repGroup.RetrieveFields
                                                  where f.AppObject == repGroup.AppObject
                                                  && ExcelHelper.GetRowIndexByTargetNamedRange(f.TargetNamedRange) == targetRowOrColumn
                                                  && ExcelHelper.GetSheetNameByNamedRange(f.TargetNamedRange).Equals(targetWorksheet)
                                                  select f).ToList();
                        if (ExistingRetrievefields.Count > 0)
                        {
                            sNamedRange = repGroup.TargetNamedRange;
                            repeatingCellFields.AddRange(ExistingRetrievefields.ToList());
                            repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                            {
                                return ExcelHelper.GetColumnIndexByTargetNamedRange(rField1.TargetNamedRange).CompareTo(ExcelHelper.GetColumnIndexByTargetNamedRange(rField2.TargetNamedRange));
                            });
                            break;
                        }
                    }
                }
            }
            else if (mapMode == MapMode.RetrieveMap)
            {
                // In Retrieve Map Mode, add first save group's save other fields to repeatingCellFields collection
                ExistingSaveOtherfields = (from sm in configurationManager.SaveMaps
                                           from sg in sm.SaveGroups
                                           from sf in sm.SaveFields
                                           where sg.AppObject == repeatingCellGroup.AppObject &&
                                           sf.GroupId.Equals(sg.GroupId) && sf.SaveFieldType == SaveType.SaveOnlyField
                                           && ExcelHelper.GetRowIndexByTargetNamedRange(sf.TargetNamedRange) == targetRowOrColumn
                                           && ExcelHelper.GetSheetNameByNamedRange(sf.TargetNamedRange).Equals(targetWorksheet)
                                           select new RetrieveField
                                           {
                                               AppObject = sf.AppObject,
                                               FieldId = sf.FieldId,
                                               Type = sf.Type,
                                               TargetLocation = sf.DesignerLocation,
                                               TargetNamedRange = sf.TargetNamedRange,
                                               TargetColumnIndex = sf.TargetColumnIndex
                                           }).ToList();

                if (ExistingSaveOtherfields.Count > 0)
                {
                    repeatingCellFields.AddRange(ExistingSaveOtherfields.ToList());
                    // Always sort repeating or save other fields using ExcelHelper.GetColumnIndexByTargetNamedRange, do not use retrieveField.TargetColumnIndex
                    repeatingCellFields.Sort(delegate (RetrieveField rField1, RetrieveField rField2)
                    {
                        return (ExcelHelper.GetColumnIndexByTargetNamedRange(rField1.TargetNamedRange).CompareTo(ExcelHelper.GetColumnIndexByTargetNamedRange(rField2.TargetNamedRange)));
                    });
                }
            }
            string comment = string.Empty;
            if (String.IsNullOrEmpty(sNamedRange))
            {
                //sNamedRange = repeatingCellGroup.RetrieveFields.Select(s => s.TargetNamedRange).FirstOrDefault();
                // AB-296 do not use retrieveFields namedrange and create new one for repeating group if not exists.
                sNamedRange = repeatingCellGroup.TargetNamedRange;
                if (String.IsNullOrEmpty(sNamedRange))
                {
                    sNamedRange = ExcelHelper.CreateUniqueNameRange();
                    comment = mapMode == MapMode.RetrieveMap ? "Display Map" : "Save Map";
                }
            }

            //To create a named range for a repeating cell we need the first and last field
            RetrieveField rFieldFirst = repeatingCellFields.First();
            RetrieveField rFieldLast = repeatingCellFields.Last();

            Excel.Range firstCell = ExcelHelper.GetRange(rFieldFirst.TargetNamedRange);
            Excel.Range lastCell = ExcelHelper.GetRange(rFieldLast.TargetNamedRange);
            Excel.Worksheet sheet = firstCell.Worksheet;

            // For Preloaded Grids                        
            if (mapMode == MapMode.SaveMap && !string.IsNullOrEmpty(sNamedRange) && currentSaveGroup != null && currentSaveGroup.LoadedRows > 0)
            {
                Excel.Range preloadedGridRange = ExcelHelper.GetRange(sNamedRange);
                int nRows = preloadedGridRange.Rows.Count;
                if (nRows > 2)
                {
                    //We have a preloaded grid of data. 
                    //Just reset the lastCell value, and when the headerRange will be evaluated below, namedRange will get extended to added fields.
                    Excel.Range newLastRowLastColumnCell = ExcelHelper.NextVerticalCell(lastCell, nRows - 1);
                    lastCell = newLastRowLastColumnCell;
                }
            }

            Excel.Range headerRange;
            if (repeatingCellGroup.Layout.Equals("Horizontal"))
                headerRange = sheet.Range[ExcelHelper.NextHorizontalCell(firstCell, -1), ExcelHelper.NextHorizontalCell(lastCell, -1)];
            else
                headerRange = sheet.Range[ExcelHelper.NextVerticalCell(firstCell, -1), ExcelHelper.NextVerticalCell(lastCell, -1)];

            if (string.IsNullOrEmpty(comment))
                ExcelHelper.AssignNameToRange(headerRange, sNamedRange);
            else
                ExcelHelper.AssignNameToRange(headerRange, sNamedRange, comment);

            repeatingCellGroup.TargetNamedRange = sNamedRange;

            // Calculate Target Column Index
            foreach (RetrieveField rfield in repeatingCellFields)
            {
                if (repeatingCellGroup.Layout.Equals("Horizontal"))
                {
                    int Index = ExcelHelper.GetRowIndex(rFieldFirst.TargetNamedRange, rfield.TargetNamedRange);
                    if (Index <= 0)
                    {
                        // there are times the target map get 0d or -1, couldn't replicate the issue so added
                        // the following validation to avoid infinite loop at runtime
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("RETRIEVEMAP_TargetInd_ErrorMsg"), resourceManager.GetResource("RETRIEVEMAP_TargetIndCap_ErrorMsg"));
                        return;
                    }
                    rfield.TargetColumnIndex = Index;

                }
                else
                    rfield.TargetColumnIndex = ExcelHelper.GetColumnIndex(rFieldFirst.TargetNamedRange, rfield.TargetNamedRange);
            }

            // Recalculate Target Column Index for Save Other type of Save Fields
            if (ExistingSaveOtherfields.Count > 0 && currentSaveGroup != null)
                foreach (var sf in CurrentSaveMap.SaveFields.Where(sf => sf.GroupId == currentSaveGroup.GroupId))
                {
                    if (repeatingCellGroup.Layout.Equals("Vertical"))
                        sf.TargetColumnIndex = ExcelHelper.GetColumnIndex(rFieldFirst.TargetNamedRange, sf.TargetNamedRange);
                    else if (repeatingCellGroup.Layout.Equals("Horizontal"))
                        sf.TargetColumnIndex = ExcelHelper.GetRowIndex(rFieldFirst.TargetNamedRange, sf.TargetNamedRange);
                }
        }

        /// <summary>
        /// A RetrieveField will be removed from the RetrieveMap in case of independent retrieve field.
        /// A RetrieveField will be removed from the RepeatingGroup in case of repeating field.
        /// For every retrieve field its appropriate named range will also be removed.
        /// </summary>
        /// <param name="repGroup"></param>
        /// <param name="rField"></param>
        public void RemoveField(RepeatingGroup repGroup, RetrieveField rField)
        {
            ObjectType cellType = rField.Type;

            //Remove the Validation
            if (cellValidator != null)
                cellValidator.RemoveFieldFromValidator(repGroup == null ? rField.AppObject.ToString() : repGroup.AppObject.ToString(), rField.TargetLocation, cellType);

            //Remove the Range of this field
            if (cellType == ObjectType.Independent)
            {
                ExcelHelper.RemoveNamedRange(rField.TargetNamedRange);
                retrieveMap.RetrieveFields.Remove(rField);
            }
            else if (cellType == ObjectType.Repeating)
            {
                // AB-296
                ExcelHelper.RemoveNamedRange(rField.TargetNamedRange);
                repeatingCellMapController.RemoveFieldFromRepeatingGroup(repGroup, rField);
            }
        }

        internal void Initialize(RetrieveMap map, MapMode mapMode = MapMode.RetrieveMap, string saveMapName = "", ObjectType? objectTypeVal = null)
        {
            if (map == null)
            {
                retrieveMap = new RetrieveMap();
                formOpenMode = FormOpenMode.Create;
            }
            else
            {
                retrieveMap = map;
                formOpenMode = FormOpenMode.Edit;
            }
            this.mapMode = mapMode;
            this.CurrentSaveMapName = saveMapName;

            this.AddFieldsToCachedList(map);
            if (View != null)
            {
                View.LoadControls(formOpenMode, retrieveMap, mapMode, saveMapName, objectTypeVal);
                RepeatingCellLayout layout = View.SelectedLayout.Equals("Vertical") ? RepeatingCellLayout.Vertical : RepeatingCellLayout.Horizontal;
                cellValidator = new RetrieveMapCellValidator(retrieveMap, layout, CurrentSaveMap);
            }
            repeatingCellMapController = new RepeatingGroupController(retrieveMap, View);
        }

        /// <summary>
        /// Gets a dummmy retrive field for validation
        /// Added to support RetrieveMapCellPermission.DenyOverridingSameFieldInGrid validation
        /// </summary>
        /// <param name="selectedNode"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private RetrieveField GetRetrieveFieldForSelectedNode(TreeNode selectedNode, ObjectType objectType, string rootObjectUniqueId)
        {
            ApttusField apttusField = (ApttusField)selectedNode.Tag;
            ApttusObject apttusObject = (ApttusObject)selectedNode.Parent.Tag;
            RetrieveField retField = null;
            if (apttusField != null && apttusObject != null)
            {
                string relationalFieldId = repeatingCellMapController.GetRelationalFieldId(apttusField, selectedNode, string.Empty, apttusObject.IdAttribute);
                string fieldName = string.Empty;

                if (!string.IsNullOrEmpty(relationalFieldId))
                {
                    fieldName = repeatingCellMapController.GetFullHierarchyName(apttusField, selectedNode);
                }
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = apttusObject.Name;
                }
                fieldName = fieldName + "." + apttusField.Name;
                if (relationalFieldId.Equals(string.Empty) && !apttusObject.UniqueId.ToString().Equals(rootObjectUniqueId) && apttusField.Id == apttusObject.IdAttribute)
                {
                    if (selectedNode.Parent.Parent != null)
                    {
                        apttusObject = (selectedNode.Parent.Parent.Tag as ApttusObject);
                    }
                    apttusField = ApplicationDefinitionManager.GetInstance.GetField(apttusObject.UniqueId, selectedNode.Name);
                    fieldName = repeatingCellMapController.GetFullHierarchyName(apttusField, selectedNode);
                }
                retField = new RetrieveField
                {
                    FieldId = string.IsNullOrEmpty(relationalFieldId) ? apttusField.Id : relationalFieldId + apttusField.Id,
                    FieldName = fieldName,
                    AppObject = apttusObject.UniqueId,
                    Type = objectType,
                };
            }
            return retField;
        }
        /// <summary>
        /// Define Validation for indepandent and repeating map
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="cellType"></param>
        /// <param name="secondLevelObjectGuid"></param>
        /// <returns></returns>
        public bool ValidateCell(Excel.Range Target, ObjectType cellType, String secondLevelObjectGuid, TreeNode selectedNode)
        {
            string firstObject, secondObject;
            RetrieveField retField = GetRetrieveFieldForSelectedNode(selectedNode, cellType, secondLevelObjectGuid);
            //Cell validator also validates duplicate fields AB-3107
            RetrieveMapCellPermission permission = cellValidator.Validate(Target, cellType, secondLevelObjectGuid, mapMode, out firstObject, out secondObject, retField);

            switch (permission)
            {
                case RetrieveMapCellPermission.AllowAdd:
                    View.AddField(Target, selectedNode);
                    return false;

                case RetrieveMapCellPermission.AllowUpdate:
                    View.UpdateField(Target);
                    return false;

                case RetrieveMapCellPermission.DenyOverridingDifferentCellType:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCell_InfoMsg"), cellType == ObjectType.Independent ? "An" : "A"
                            , cellType.ToString(), cellType == ObjectType.Independent ? ObjectType.Repeating.ToString() :
                            ObjectType.Independent.ToString());
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                    break;

                case RetrieveMapCellPermission.DenyAddingDifferentSecondLevelObject:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string[] names = applicationDefinitionManager.GetParentAndChildObjects(applicationDefinitionManager.GetAppObjects()).Where(s => s.UniqueId.ToString().Equals(firstObject) || s.UniqueId.ToString().Equals(secondObject)).Select(s => s.Name).ToArray();
                        string message;
                        if (names.Length == 2)
                        {
                            message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellOBJAdd_InfoMsg"), names[1], names[0]);
                            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return true;
                    }

                case RetrieveMapCellPermission.DenyOverridingSecondLevelObject:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string[] names = applicationDefinitionManager.GetParentAndChildObjects(applicationDefinitionManager.GetAppObjects()).Where(s => s.UniqueId.ToString().Equals(firstObject) || s.UniqueId.ToString().Equals(secondObject)).Select(s => s.Name).ToArray();

                        string message;
                        if (names.Length == 2)
                        {
                            message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellOBJOverRide_InfoMsg"), names[0], names[1]);
                            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return true;
                    }

                case RetrieveMapCellPermission.DenyAddingExistingObjectToNewRow:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string name = (from appObject in configurationManager.Definition.AppObjects
                                       from appChildObject in appObject.Children
                                       where appChildObject.UniqueId.ToString().Equals(secondLevelObjectGuid)
                                       select appChildObject.Name).FirstOrDefault();

                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellROW_InfoMsg"), name);
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }

                case RetrieveMapCellPermission.DenyOverridingRetrieveFields:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        //string message = String.Format("A Display Field already exists on cell and cannot be overwritten by Save Field.");
                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellDenyOverride_ErrMsg"));
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return true;
                    }

                case RetrieveMapCellPermission.DenyOverridingSaveFields:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        //string message = String.Format("A Save Field already exists on cell and cannot be overwritten by Display Field.");
                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellDenyOverrideSave_ErrMsg"));
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return true;
                    }
                // Deny to use fields in cross display map.
                case RetrieveMapCellPermission.DenyAddingFieldsToDifferentDisplayMap:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_ValidateCellDenyFieldsTODiffDM_ErrMsg"));
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return true;

                    }
                //Deny adding same field in the grid
                case RetrieveMapCellPermission.DenyOverridingSameFieldInGrid:
                    {
                        Globals.ThisAddIn.Application.Undo();

                        string message = String.Format(resourceManager.GetResource("RETRIEVEMAPCTL_DenyOverridingSameFieldInGrid_ErrMsg"), retField.FieldName, retField.TargetLocation);
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return true;
                    }
            }
            return false;

        }

        public void AddField(ApttusObject apttusObj, ApttusField apttusField, string targetLocation, ObjectType type, string namedRange = "", TreeNode selectedNode = null)
        {
            if (type == ObjectType.Independent)
            {
                RetrieveField rField = new RetrieveField();
                rField.FieldId = apttusField.Id;
                rField.TargetLocation = targetLocation;
                rField.Type = type;
                rField.DataType = apttusField.Datatype;
                rField.AppObject = apttusObj.UniqueId;

                rField.FieldName = apttusObj.Name + "." + apttusField.Name;
                if (!String.IsNullOrEmpty(namedRange))
                    rField.TargetNamedRange = namedRange;

                retrieveMap.RetrieveFields.Add(rField);
                //Refresh the view
                if (View != null)
                    View.AddRow(rField);
            }
            else if (type == ObjectType.Repeating)
                repeatingCellMapController.AddRepeatingCellField(apttusObj, apttusField, targetLocation, namedRange, selectedNode);
        }

        public bool UpdateField(TreeNode node, string targetLocation, ObjectType type, string targetNamedRange)
        {
            bool bRetrieveFieldUpdated = true;
            ApttusField apttusField = (ApttusField)node.Tag;
            if (type == ObjectType.Independent)
            {
                RetrieveField rField = (from retrieveField in retrieveMap.RetrieveFields
                                        where retrieveField.TargetNamedRange == targetNamedRange
                                        select retrieveField).FirstOrDefault();

                rField.FieldId = apttusField.Id;
                rField.TargetLocation = targetLocation;
                rField.Type = type;
                rField.DataType = apttusField.Datatype;

                rField.FieldName = node.Parent.Parent == null ? ((ApttusObject)node.Parent.Tag).Name + "." + apttusField.Name : node.FullPath.Replace("\\", ".");
            }
            else if (type == ObjectType.Repeating)
                bRetrieveFieldUpdated = repeatingCellMapController.UpdateRepeatingCellField(node, targetLocation, targetNamedRange);

            View.RefreshGrid();
            return bRetrieveFieldUpdated;
        }

        public RetrieveMap RetrieveMap {
            get {
                return retrieveMap;
            }
        }

        public FormOpenMode FormOpenMode {
            get {
                return formOpenMode;
            }
        }

        internal void Close(bool isAddToSaveMap = false)
        {
            // do dirty check
            if (IsRetrieveMapDirty() && !isAddToSaveMap)
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

            switch (mapMode)
            {
                case MapMode.RetrieveMap:
                    TaskPaneHelper.RemoveCustomPane(CurrentTaskPane);
                    break;
                case MapMode.SaveMap:
                    TaskPaneHelper.RemoveCustomPane(CurrentTaskPane);
                    //TaskPaneHelper.Visible(Constants.SAVEMAP_NAME, true);
                    SaveMapView saveMapView = new SaveMapView();
                    CurrentSaveMapController = new SaveMapController(CurrentSaveMap, saveMapView, FormOpenMode.Edit, CurrentSaveMapController.origFields, CurrentSaveMapController.SaveFieldBoundModel);
                    saveMapView.SetController(CurrentSaveMapController);

                    CurrentSaveMapController.LoadControls(CurrentSaveMapName);

                    break;
            }
        }

        public RepeatingGroup GetRepeatingGroupByRetrieveField(RetrieveField field)
        {
            // Display Map enhancement
            //return retrieveMap.RepeatingGroups.Where(s => s.AppObject.Equals(field.AppObject)).FirstOrDefault();

            RepeatingGroup currentGroup = null;
            var repGroup = from rg in retrieveMap.RepeatingGroups
                           from rf in rg.RetrieveFields
                           where rf.TargetNamedRange.Equals(field.TargetNamedRange)
                           select rg;

            if (repGroup.Count() == 1)
                currentGroup = (RepeatingGroup)repGroup.FirstOrDefault();

            return currentGroup;
        }

        public void SetCellValidatorLayout(RepeatingCellLayout layout)
        {
            cellValidator.UpdateLayout(layout);
        }

        public RepeatingGroup GetRepeatingGroupFromAppObjectId(Guid AppObjectId)
        {
            return repeatingCellMapController.GetRepeatingGroup(AppObjectId);
        }

        public List<RetrieveField> GetSortByFields(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSortByFields(AppObjectId);
        }

        public List<RetrieveField> GetGroupByFields(Guid AppObjectId)
        {
            return repeatingCellMapController.GetGroupByFields(AppObjectId);
        }

        public string GetSelectedGroupByField(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSelectedGroupByField(AppObjectId);
        }

        public string GetSelectedSortByField(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSelectedSortByField(AppObjectId);
        }

        internal string GetSelectedSortByField2(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSelectedSortByField2(AppObjectId);
        }

        internal string GetSelectedGroupByField2(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSelectedGroupByField2(AppObjectId);
        }

        internal RepeatingGroupSortDirection GetSortDirectionForField1(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSortDirectionForField1(AppObjectId);
        }

        internal RepeatingGroupSortDirection GetSortDirectionForField2(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSortDirectionForField2(AppObjectId);
        }

        internal List<string> GetSelectedTotalFields(Guid AppObjectId)
        {
            return repeatingCellMapController.GetSelectedTotalFields(AppObjectId);
        }

        internal static void DeleteRetrieveMapNamedRange(RetrieveMap retrieveMap)
        {
            //We need a try..catch block for each NamedRange Delete, because if the named range doesn't exist and exception will be throw, hence the loop will be terminated,
            //and all the named ranges associated with this retrieve map won't be removed and will keep the excel template in an unmanaged way, where some named range(s) are deleted  
            //and some are not, so to make sure that the all the named range within that retrieve map are removed, we will continue to loop all elements and the exception if
            //thrown will be caught and continue the loop to remove all the named ranges.

            bool bClearContents = true; //For increased readability

            //Delete NamedRange for Independent Fields
            foreach (RetrieveField independentRF in retrieveMap.RetrieveFields)
            {
                try
                {
                    Excel.Range labelCell = ExcelHelper.NextHorizontalCell(ExcelHelper.GetRange(independentRF.TargetNamedRange), -1);
                    if (labelCell != null)
                        labelCell.Value = string.Empty;

                    ExcelHelper.RemoveNamedRange(independentRF.TargetNamedRange, bClearContents);
                }
                catch (Exception)
                {
                    //Even if an exception is thrown, we should continue to loop through all the named ranges and delete them individually.
                }
            }
            retrieveMap.RetrieveFields.Clear();

            //Delete NamedRange for Repeating Fields
            foreach (RepeatingGroup repGroup in retrieveMap.RepeatingGroups)
            {
                try
                {
                    //First Delete Individual Retrieve Fields of Repeating Group
                    foreach (RetrieveField repeatingField in repGroup.RetrieveFields)
                    {
                        try
                        {
                            ExcelHelper.RemoveNamedRange(repeatingField.TargetNamedRange, bClearContents);
                        }
                        catch (Exception)
                        {
                            //Even if an exception is thrown, we should continue to loop through all the named ranges and delete them individually.
                        }
                    }
                    //Then Delete the NamedRange of entire Repeating Group
                    ExcelHelper.RemoveNamedRange(repGroup.TargetNamedRange, bClearContents);
                    repGroup.RetrieveFields.Clear();
                }
                catch (Exception)
                {
                    //Even if an exception is thrown, we should continue to loop through all the named ranges and delete them individually.
                }
            }
            retrieveMap.RepeatingGroups.Clear();
        }

        /// <summary>
        /// Delete Matrix Map Range
        /// </summary>
        /// <param name="matrixMap"></param>
        internal static void DeleteMatrixMapNamedRange(MatrixMap matrixMap)
        {
            //We need a try..catch block for each NamedRange Delete, because if the named range doesn't exist and exception will be throw, hence the loop will be terminated,
            //and all the named ranges associated with this retrieve map won't be removed and will keep the excel template in an unmanaged way, where some named range(s) are deleted  
            //and some are not, so to make sure that the all the named range within that retrieve map are removed, we will continue to loop all elements and the exception if
            //thrown will be caught and continue the loop to remove all the named ranges.

            bool bClearContents = true;

            // For DataFields 
            var results = (from mdata in matrixMap.MatrixData.MatrixDataFields
                           select new
                           {
                               TargetNamedRange = mdata.TargetNamedRange,
                               RenderingType = mdata.RenderingType
                           }).ToList();

            // For Column fields
            results = results.Union((from mFieldColumn in matrixMap.MatrixColumn.MatrixFields
                                     select new
                                     {
                                         TargetNamedRange = mFieldColumn.TargetNamedRange,
                                         RenderingType = mFieldColumn.RenderingType
                                     })).ToList();

            // For Row fields
            results = results.Union((from mFieldRow in matrixMap.MatrixRow.MatrixFields
                                     select new
                                     {
                                         TargetNamedRange = mFieldRow.TargetNamedRange,
                                         RenderingType = mFieldRow.RenderingType
                                     })).ToList();

            // For loop on final results and delete name range. in case of static , we will not remove contents of fields
            if (results != null)
            {
                results.ForEach((item) =>
                {
                    try
                    {
                        if (item.RenderingType == MatrixRenderingType.Static)
                            ExcelHelper.RemoveNamedRange(item.TargetNamedRange, false);
                        else
                            ExcelHelper.RemoveNamedRange(item.TargetNamedRange, bClearContents);
                    }
                    catch (Exception)
                    {
                        //Even if an exception is thrown, we should continue to loop through all the named ranges and delete them individually.
                    }

                });
            }
            matrixMap.MatrixData.MatrixDataFields.Clear();
            matrixMap.MatrixRow.MatrixFields.Clear();
            matrixMap.MatrixColumn.MatrixFields.Clear();
        }

        internal List<RetrieveField> GetRetreiveFields(Guid AppObjectId)
        {
            return repeatingCellMapController.GetRetreiveFields(AppObjectId);
        }

        internal List<RetrieveField> GetCalculatableTotalFields(Guid AppObjectId)
        {
            return repeatingCellMapController.GetCalculatableTotalFields(AppObjectId);
        }
    }
}
