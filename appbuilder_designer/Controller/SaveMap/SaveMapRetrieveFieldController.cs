/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public class SaveMapRetrieveFieldController
    {
        public List<SaveMapRetrieveField> Model { get; set; }
        private SaveMapRetrieveFieldView View;
        private SaveMap CurrentSaveMap;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveMapRetrieveFieldController(SaveMap saveMap, SaveMapRetrieveFieldView view)
        {
            CurrentSaveMap = saveMap;
            if (view != null)
            {
                // Set the view
                this.View = view;

                this.View.SetController(this);
            }
        }

        public void SetView(SaveMap saveMapModel)
        {
            //1. Get list of Retrieve Maps
            List<SaveMapRetrieveMap> maps = (from m in configurationManager.RetrieveMaps
                                             select new SaveMapRetrieveMap
                                             {
                                                 RetrieveMapId = m.Id,
                                                 RetrieveMapName = m.Name
                                             }).ToList();

            maps.Add(new SaveMapRetrieveMap { RetrieveMapId = Guid.NewGuid(), RetrieveMapName = resourceManager.GetResource("ShowAll_Msg") });

            //2. Bind Filter by dropdown
            View.BindFilterBy(maps);

            //3. Get model based on dropdown selection
            View.SetInitialRetrieveMap();

        }

        public void RetrieveMapSelectionChange(SaveMapRetrieveMap selection)
        {
            if (selection.RetrieveMapName == resourceManager.GetResource("ShowAll_Msg") || selection.RetrieveMapName == string.Format(DataMigrationConstants.SaveMapNameFormat, "ExternalID"))
            {
                List<SaveMapRetrieveField> results = new List<SaveMapRetrieveField>();
                foreach (RetrieveMap rMap in configurationManager.RetrieveMaps)
                {
                    results.AddRange(GenerateModel(CurrentSaveMap, rMap.Id));
                }
                Model = results;
            }
            else
            {
                Model = GenerateModel(CurrentSaveMap, selection.RetrieveMapId);
            }

            // Bind data grid with model.
            if (View != null)
                View.BindGrid(Model);
        }

        private List<SaveMapRetrieveField> GenerateModel(SaveMap SaveMap, Guid RetrieveMapId)
        {
            RetrieveMap rMap = configurationManager.RetrieveMaps.SingleOrDefault(m => m.Id == RetrieveMapId);
            
            List<SaveMapRetrieveField> results = new List<SaveMapRetrieveField>();
            if (rMap != null)
            {
                // 1. Find fields for Non-Repeating object types
                results = (from rf in rMap.RetrieveFields
                           where !(from sf in SaveMap.SaveFields
                                   where sf.AppObject == rf.AppObject &
                                   sf.FieldId == rf.FieldId
                                   select sf.FieldId).Contains(rf.FieldId)
                           select new SaveMapRetrieveField
                           {
                               Included = false,
                               Type = rf.Type,
                               AppObjectUniqueId = rf.AppObject,
                               RetrieveFieldId = rf.FieldId,
                               RetrieveFieldName = rf.FieldName,
                               RetrieveMapId = rMap.Id,
                               RetrieveMapName = rMap.Name,
                               DesignerLocation = rf.TargetLocation,
                               TargetNamedRange = rf.TargetNamedRange,
                               TargetColumnIndex = rf.TargetColumnIndex,
                               MultiLevelFieldId = rf.MultiLevelFieldId
                           }).ToList();

                // 2. Add Repeating Group fields to the result
                results = results.Union((from rg in rMap.RepeatingGroups
                                         from rf in rg.RetrieveFields
                                         where //(rg.AppObject == rf.AppObject) &&
                                                !(from sf in SaveMap.SaveFields
                                                  where sf.AppObject == rf.AppObject &
                                                  sf.FieldId == rf.FieldId
                                                  select sf.FieldId).Contains(rf.FieldId)
                                         select new SaveMapRetrieveField
                                         {
                                             Included = false,
                                             Type = rf.Type,
                                             AppObjectUniqueId = rf.AppObject,
                                             RetrieveFieldId = rf.FieldId,
                                             RetrieveFieldName = rf.FieldName,
                                             RetrieveMapId = rMap.Id,
                                             RetrieveMapName = rMap.Name,
                                             DesignerLocation = rf.TargetLocation,
                                             TargetNamedRange = rf.TargetNamedRange,
                                             TargetColumnIndex = rf.TargetColumnIndex,
                                             Layout = rg.Layout,
                                             MultiLevelFieldId = rf.MultiLevelFieldId
                                         })).ToList();

                //Group By Field cannot be part of save map, thus will be removed from the list.
                foreach (RepeatingGroup rg in rMap.RepeatingGroups)
                {
                    if (!String.IsNullOrEmpty(rg.GroupByField))
                        results.RemoveAll(field => field.RetrieveFieldId.Equals(rg.GroupByField));
                    if (!String.IsNullOrEmpty(rg.GroupByField2))
                        results.RemoveAll(field => field.RetrieveFieldId.Equals(rg.GroupByField2));
                }
            }           

            if (rMap != null && rMap.CrossTabMaps != null)
            {
                // Find fields for CrossTab object type
                List<SaveMapRetrieveField> crossTabFields = (from ct in rMap.CrossTabMaps
                                                             where !(from sf in SaveMap.SaveFields
                                                                     where sf.Type == ObjectType.CrossTab
                                                                     select sf.CrossTab.Id).Contains(ct.Id)
                                                             select new SaveMapRetrieveField
                                                             {
                                                                 Included = false,
                                                                 Type = ObjectType.CrossTab,
                                                                 RetrieveFieldName = ct.Name,
                                                                 DesignerLocation = ct.DataField.TargetLocation,
                                                                 RetrieveMapId = rMap.Id,
                                                                 RetrieveMapName = rMap.Name,
                                                                 CrossTab = ct
                                                             }).ToList();

                results.AddRange(crossTabFields);
            }
            return results;

        }

        private bool IsSaveFieldPartOfRepeatingGroup(string repeatingGroupNameRange, string savefieldNameRange, string layout ="Vertical")
        {
            if (layout.Equals("Vertical"))
            {
                Excel.Range fieldNameRange = ExcelHelper.GetRange(savefieldNameRange);
                Excel.Range repGroupRange = ExcelHelper.GetRange(repeatingGroupNameRange);
                if (fieldNameRange.Worksheet == repGroupRange.Worksheet)
                    return Globals.ThisAddIn.Application.Intersect(repGroupRange, ExcelHelper.NextVerticalCell(fieldNameRange, -1)) != null;
            }
            else
            {
                Excel.Range fieldNameRange = ExcelHelper.GetRange(savefieldNameRange);
                Excel.Range repGroupRange = ExcelHelper.GetRange(repeatingGroupNameRange);
                if (fieldNameRange.Worksheet == repGroupRange.Worksheet)
                    return Globals.ThisAddIn.Application.Intersect(repGroupRange, ExcelHelper.NextHorizontalCell(fieldNameRange, -1)) != null;
            }
            return false;
        }

        internal void AddRetrieveFieldsToSaveMap()
        {
            // Add Non-Repeating Fields
            List<SaveField> IndependentFields = (from f in Model
                                                 where f.Included == true && (f.Type == ObjectType.Independent || f.Type == ObjectType.CrossTab)
                                                 select new SaveField
                                                 {
                                                     AppObject = f.AppObjectUniqueId,
                                                     FieldId = f.RetrieveFieldId,
                                                     Type = f.Type,
                                                     SaveCondition = null,
                                                     DesignerLocation = f.DesignerLocation,
                                                     TargetNamedRange = f.TargetNamedRange,
                                                     TargetColumnIndex = f.TargetColumnIndex,
                                                     CrossTab = f.CrossTab,
                                                     SaveFieldType = SaveType.RetrievedField,
                                                     SaveFieldName = f.RetrieveFieldName,
                                                     MultiLevelFieldId = f.MultiLevelFieldId
                                                 }).ToList();

            // Add Independent Save Fields to Save Map
            CurrentSaveMap.SaveFields.AddRange(IndependentFields);

            // Repeating section
            // 1. Create all Save Groups
            List<SaveGroup> SaveGroups = (from f in Model
                                          from rm in configurationManager.RetrieveMaps
                                          from rg in rm.RepeatingGroups
                                          from rf in rg.RetrieveFields
                                          where rm.Id == f.RetrieveMapId && f.Type == ObjectType.Repeating
                                          && f.Included == true //& f.AppObjectUniqueId == rg.AppObject
                                          && f.RetrieveFieldId == rf.FieldId      
                                          && IsSaveFieldPartOfRepeatingGroup(rg.TargetNamedRange, f.TargetNamedRange, rg.Layout)
                                          select new SaveGroup
                                          {
                                              AppObject = f.AppObjectUniqueId,
                                              TargetNamedRange = rg.TargetNamedRange,
                                              Layout = rg.Layout
                                          }).Distinct().ToList();
            // 1.1 Create Distinct Save Groups
            // Now by Targetnamedrange plus appobject
            List<SaveGroup> DistinctSaveGroups = SaveGroups
                .GroupBy(p => new { p.TargetNamedRange, p.AppObject } )
                .Select(g => g.First())
                .ToList();

            // 1.2 Exclude Save Groups already existing in the Save Map
            //DistinctSaveGroups.RemoveAll(sg => (from tnr in CurrentSaveMap.SaveGroups select tnr.TargetNamedRange).Contains(sg.TargetNamedRange));
            for (int i = 0; i < DistinctSaveGroups.Count; i++)
            {
                // Check if similar save group exists in current save map
                if (CurrentSaveMap.SaveGroups.Exists(sg => sg.TargetNamedRange.Equals(DistinctSaveGroups[i].TargetNamedRange) && sg.AppObject.Equals(DistinctSaveGroups[i].AppObject)))
                    DistinctSaveGroups.RemoveAt(i);
            }

            // 1.3 Assign Guid to new Save Groups
            foreach (var dsg in DistinctSaveGroups)
                dsg.GroupId = Guid.NewGuid();

            // 1.4 Add New and Unique Save Groups to Save Map
            CurrentSaveMap.SaveGroups.AddRange(DistinctSaveGroups);

            // 2. Add Repeating Fields for each Save Group
            List<SaveField> RepeatingFields = (from sg in CurrentSaveMap.SaveGroups
                                               from f in Model
                                               from rm in configurationManager.RetrieveMaps
                                               from rg in rm.RepeatingGroups
                                               where f.RetrieveMapId == rm.Id & f.Included == true
                                               & f.Type == ObjectType.Repeating & sg.AppObject == f.AppObjectUniqueId
                                               //& sg.AppObject == rg.AppObject 
                                               & sg.TargetNamedRange == rg.TargetNamedRange
                                               select new SaveField
                                               {
                                                   GroupId = sg.GroupId,
                                                   AppObject = f.AppObjectUniqueId,
                                                   FieldId = f.RetrieveFieldId,
                                                   Type = f.Type,
                                                   SaveCondition = null,
                                                   DesignerLocation = f.DesignerLocation,
                                                   TargetNamedRange = f.TargetNamedRange,
                                                   TargetColumnIndex = f.TargetColumnIndex,
                                                   CrossTab = f.CrossTab,
                                                   SaveFieldType = SaveType.RetrievedField,
                                                   SaveFieldName = f.RetrieveFieldName,
                                                   MultiLevelFieldId = f.MultiLevelFieldId
                                               }).ToList();


            // In case of look up fields we have Either Or scenario, 
            // i.e. If you add ID field of Lookup you cannot add Name field and vice versa.
            // This validation needs to happen on add of Save Fields.
            List<SaveField> lookupFieldToRemove = new List<SaveField>();
            bool lookupFieldsToRemove = ValidationManager.ValidationLookup(RepeatingFields, CurrentSaveMap.SaveFields, ref lookupFieldToRemove);
            if (lookupFieldsToRemove && lookupFieldToRemove.Count > 0)
            {
                foreach (SaveField field in lookupFieldToRemove)
                {
                    var removeSF = RepeatingFields.FirstOrDefault(item => item.AppObject.Equals(field.AppObject) && item.FieldId.Equals(field.FieldId));
                    RepeatingFields.Remove(removeSF);
                }
            }

            // 2.1 Add Repeating Save Fields to Save Map
            CurrentSaveMap.SaveFields.AddRange(RepeatingFields);
            
            //View.Dispose();
        }

        internal void ToggleSelectAll(bool SelectAll)
        {
            if (SelectAll)
                Model.FindAll(f => f.Included = true).Where(f => f.Included == false);
            else
                Model.FindAll(f => f.Included = false).Where(f => f.Included == true);

            // Bind data grid with model.
            View.BindGrid(Model);
        }

        internal void Close()
        {
            View.Dispose();
        }
    }
}
