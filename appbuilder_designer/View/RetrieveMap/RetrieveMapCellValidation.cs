/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public enum RepeatingCellLayout
    {
        Vertical,
        Horizontal
    }

    public enum RetrieveMapCellPermission
    {
        AllowAdd,
        AllowUpdate,
        DenyOverridingDifferentCellType,
        DenyOverridingSecondLevelObject,
        DenyOverridingRetrieveFields,
        DenyOverridingSaveFields,
        DenyAddingDifferentSecondLevelObject,
        DenyAddingExistingObjectToNewRow,
        DenyAddingFieldsToDifferentDisplayMap,
        DenyOverridingSameFieldInGrid
    }

    class IndependentCellValidator
    {
        private Dictionary<string, string> SecondLevelObjectByRange;
        public RepeatingCellLayout CellLayout { get; set; }
        //Current retrieve map which has not been saved also needs to be checked for same field validation thus adding a member variable.
        public RetrieveMap retrieveMap = new RetrieveMap();
        private SaveMap CurrentSaveMap;
        public IndependentCellValidator(RetrieveMap retrieveMap, RepeatingCellLayout layout, SaveMap CurrentSaveMap)
        {
            CellLayout = layout;
            this.retrieveMap = retrieveMap;
            this.CurrentSaveMap = CurrentSaveMap;
            SecondLevelObjectByRange = new Dictionary<string, string>();
            if (retrieveMap.RetrieveFields.Count != 0)
            {
                List<RetrieveField> independentRetrieveFields = retrieveMap.RetrieveFields.Where(field => field.Type == ObjectType.Independent).ToList();
                foreach (RetrieveField rField in independentRetrieveFields)
                    SecondLevelObjectByRange[rField.TargetLocation] = rField.AppObject.ToString();
            }
        }

        /// <summary>
        /// Notifies whether we can add or update an independent cell 
        /// Added a new param retrieveField for pre validation process which takes place for duplicate field validation
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="objectGuid"></param>
        /// <returns></returns>
        public RetrieveMapCellPermission Validate(Excel.Range Target, string objectGuid, MapMode mapMode, RetrieveField retrieveField)
        {
            String mapKey = String.Format("{0}!{1}", Target.Worksheet.Name, Target.Address);

            // Check if Cell on which field is being dropped is part of existing Retrieve or Save Map, if yes Deny override
            string currentCellNamedRange = ExcelHelper.GetExcelCellName(Target);
            if (!string.IsNullOrEmpty(currentCellNamedRange))
            {
                RangeMap rangeMap = ConfigurationManager.GetInstance.GetAllRangeMaps().Where(r => r.RangeName.Equals(currentCellNamedRange)).FirstOrDefault();
                if (mapMode == MapMode.SaveMap && rangeMap.SaveMapId.Equals(Guid.Empty))
                    return RetrieveMapCellPermission.DenyOverridingRetrieveFields;

                if (mapMode == MapMode.RetrieveMap && rangeMap.RetrieveMapId.Equals(Guid.Empty))
                    return RetrieveMapCellPermission.DenyOverridingSaveFields;
            }
            bool SameFieldFound = ValidateOverridingIndependentFields(retrieveField, Target);
            if (SameFieldFound)
            {
                return RetrieveMapCellPermission.DenyOverridingSameFieldInGrid;
            }
            if (!SecondLevelObjectByRange.ContainsKey(mapKey))
            {
                SecondLevelObjectByRange[mapKey] = objectGuid;
                return RetrieveMapCellPermission.AllowAdd;
            }
            else
                return RetrieveMapCellPermission.AllowUpdate;
        }
        /// <summary>
        /// Validates a Independent field against all Display/Save Map with all Retrieve/Save/SaveOther Fields.
        /// If field is found on a Display/Save map on same sheet, RetrieveMapCellPermission.DenyOverridingSameFieldInGrid validation will be fired.
        /// </summary>
        /// <param name="retField"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool ValidateOverridingIndependentFields(RetrieveField retField, Excel.Range Target)
        {
            //Check Current Save Map 
            if (CurrentSaveMap != null)
            {
                List<SaveField> AllIndSaveOtherFields = CurrentSaveMap.SaveFields.Where(sf => sf.SaveFieldType.Equals(SaveType.SaveOnlyField) && sf.AppObject.Equals(retField.AppObject)).ToList();
                foreach (SaveField sf in AllIndSaveOtherFields)
                {
                    Excel.Range rangeOfField = ExcelHelper.GetRange(sf.TargetNamedRange);
                    if (sf.FieldId.Equals(retField.FieldId) && rangeOfField.Worksheet.Equals(Target.Worksheet))
                    {
                        retField.TargetLocation = rangeOfField.Worksheet.Name + "!" + rangeOfField.Address;
                        return true;
                    }
                }
            }
            //Check current ucRetrieve map which is yet to be saved.
            foreach (RetrieveField rf in retrieveMap.RetrieveFields)
            {
                Excel.Range rangeOfField = ExcelHelper.GetRange(rf.TargetNamedRange);
                if (rf.FieldId.Equals(retField.FieldId) && rangeOfField.Worksheet.Equals(Target.Worksheet))
                {
                    retField.TargetLocation = rf.TargetLocation;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks whether we can add a repeating cell in the target row as this row may be occupied by an independent cell.
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool CanAddRepeatingCell(Excel.Range Target)
        {
            string mapKey = CellLayout == RepeatingCellLayout.Vertical ? string.Format("${0}", Target.Row) : string.Format("{0}!{1}", Target.Worksheet.Name, Target.EntireColumn.Address.Split(':').ElementAt(0));
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = SecondLevelObjectByRange.Where(map => map.Key.Contains(mapKey));
            bool bCanAddRepeatingCell = true;
            if (keyValuePairs.Count() != 0)
            {
                foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
                {
                    bCanAddRepeatingCell = !keyValuePair.Key.Contains(Target.Worksheet.Name);
                    if (!bCanAddRepeatingCell)
                        break;
                }
            }
            return bCanAddRepeatingCell;
        }

        /// <summary>
        /// removes an independent field when it is removed from the sheet.
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <param name="secondLevelObjectGuid"></param>
        public void RemoveFieldFromValidator(string targetLocation, string secondLevelObjectGuid)
        {
            String key = SecondLevelObjectByRange.Where(map => map.Key.Equals(targetLocation) && map.Value.Equals(secondLevelObjectGuid)).Select(map => map.Key).FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(key))
                SecondLevelObjectByRange.Remove(key);
        }
    }

    class RepeatingCellValidator
    {
        private RetrieveMap retrieveMap;
        private Dictionary<string, string> SecondLevelObjectByRange;
        private Dictionary<string, List<RetrieveMap>> AllMapsOfObject;
        public RepeatingCellLayout CellLayout { get; set; }
        public RepeatingCellValidator(RetrieveMap map, RepeatingCellLayout layout)
        {
            CellLayout = layout;
            retrieveMap = map;
            SecondLevelObjectByRange = new Dictionary<string, string>();
            AllMapsOfObject = new Dictionary<string, List<RetrieveMap>>();
            LoadDictionary();
        }

        /// <summary>
        /// Will be invoked in case of retrieve map in edit mode.
        /// </summary>
        public void LoadDictionary()
        {
            foreach (RetrieveMap rMap in ConfigurationManager.GetInstance.RetrieveMaps)
                foreach (RepeatingGroup repGroup in rMap.RepeatingGroups)
                {
                    foreach (RetrieveField rField in repGroup.RetrieveFields)
                    {
                        string[] Values = rField.TargetLocation.Split('!');
                        if (Values.Count() == 2)
                        {
                            string sheetName = Values[0];
                            string[] rows = rField.TargetLocation.Split('$');
                            if (rows.Count() == 3)
                            {
                                String value = repGroup.Layout.Equals("Vertical") ? rows[2] : rows[1];
                                string mapKey = string.Format("{0}!${1}:${2}", sheetName, value, value);
                                if (!SecondLevelObjectByRange.ContainsKey(mapKey))
                                    // Display Map enhancements, since retrieve fields can belong to different AppObjects 
                                    // use RepGroup AppObject which belongs to base object on which repeating group has been created.
                                    SecondLevelObjectByRange[mapKey] = repGroup.AppObject.ToString();
                                break;
                            }
                        }
                    }
                    string appObjId = repGroup.AppObject.ToString();
                    if (!AllMapsOfObject.ContainsKey(appObjId))
                    {
                        List<RetrieveMap> maps = new List<RetrieveMap>();
                        maps.Add(rMap);
                        AllMapsOfObject.Add(appObjId, maps);
                    }
                    else
                    {
                        List<RetrieveMap> value = AllMapsOfObject[appObjId];
                        value.Add(rMap);
                    }
                }
        }
        /// <summary>
        ///  removes the key from the map when all the repeating cells occupied in a row are removed.
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <param name="secondLevelObjectGuid"></param>
        public void RemoveFieldFromValidator(string targetLocation, string secondLevelObjectGuid)
        {
            String[] splitValues = targetLocation.Split('!');
            if (splitValues.Count() != 2)
                return;

            if (retrieveMap.RepeatingGroups.Count == 0)
                return;

            String sheetName = splitValues[0];

            var fields = from rField in retrieveMap.RepeatingGroups.Where(s => s.AppObject.Equals(Guid.Parse(secondLevelObjectGuid))).FirstOrDefault().RetrieveFields
                         where rField.AppObject.ToString().Equals(secondLevelObjectGuid) && rField.TargetLocation.Contains(sheetName)
                         select rField;

            if (fields.Count() == 1)
            {
                string key = (from s in SecondLevelObjectByRange
                              where secondLevelObjectGuid.Equals(s.Value) && s.Key.Contains(sheetName)
                              select s.Key).FirstOrDefault();

                if (!String.IsNullOrEmpty(key))
                    SecondLevelObjectByRange.Remove(key);
            }
        }

        public bool ValidateUsedMap(RepeatingGroup repGroup, Excel.Range Target)
        {
            Excel.Range repeatingGroupRange = ExcelHelper.GetRange(repGroup.TargetNamedRange);

            if (repeatingGroupRange == null)
                return false;

            if (repGroup.Layout.Equals("Vertical"))
                return repeatingGroupRange.Worksheet == Target.Worksheet && repeatingGroupRange.Row == Target.Row;
            else
                return repeatingGroupRange.Worksheet == Target.Worksheet && repeatingGroupRange.Column == Target.Column;

        }

        /// <summary>
        /// Validates a Repeating/SaveOther field against all Display/Save Map with all Retrieve/Save/SaveOther Fields.
        /// If field is found on a Display/Save map on same sheet and grid, RetrieveMapCellPermission.DenyOverridingSameFieldInGrid validation will be fired.
        /// </summary>
        /// <param name="repeatingGroups"></param>
        /// <param name="Target"></param>
        /// <param name="retField"></param>
        /// <returns></returns>
        public bool ValidateOverridingRepeatingFields(List<RepeatingGroup> repeatingGroups, Excel.Range Target, RetrieveField retField)
        {
            bool result = false;
            //Validate all repeating groups
            foreach (RepeatingGroup rg in repeatingGroups)
            {
                Excel.Range repeatingGroupRange = ExcelHelper.GetRange(rg.TargetNamedRange);
                Excel.Range LabelRangeOfTargetField;
                int RepeatingGroupRowOrColumn;
                int TargetRowOrColumn;
                if (repeatingGroupRange == null)
                    return false;
                if (rg.Layout.Equals("Vertical"))
                {
                    LabelRangeOfTargetField = ExcelHelper.NextVerticalCell(Target, -1);
                    RepeatingGroupRowOrColumn = repeatingGroupRange.Row;
                    TargetRowOrColumn = LabelRangeOfTargetField.Row;
                }
                else
                {
                    LabelRangeOfTargetField = ExcelHelper.NextHorizontalCell(Target, -1);
                    RepeatingGroupRowOrColumn = repeatingGroupRange.Column;
                    TargetRowOrColumn = LabelRangeOfTargetField.Column;
                }
                if (repeatingGroupRange.Worksheet == Target.Worksheet && RepeatingGroupRowOrColumn == TargetRowOrColumn)
                {
                    foreach (RetrieveField rf in rg.RetrieveFields)
                    {
                        result = rf.FieldId.Equals(retField.FieldId) && rf.AppObject.Equals(retField.AppObject);
                        if (result)
                        {
                            retField.TargetLocation = rf.TargetLocation;
                            return result;
                        }
                    }
                }
            }
            //Check current repeating group and save maps
            result = CheckCurrentRepeatingGroupAndSaveMaps(retField, Target);
            return result;
        }

        private bool CheckCurrentRepeatingGroupAndSaveMaps(RetrieveField retField, Excel.Range Target)
        {
            bool result = false;
            foreach (RepeatingGroup crg in retrieveMap.RepeatingGroups)
            {
                foreach (RetrieveField crf in crg.RetrieveFields)
                {
                    result = crf.FieldId.Equals(retField.FieldId) && crf.AppObject.Equals(retField.AppObject);
                    if (result)
                    {
                        retField.TargetLocation = crf.TargetLocation;
                        return result;
                    }
                }
            }
            //Check all save maps for save other fields.
            List<SaveMap> AllSaveMaps = ConfigurationManager.GetInstance.SaveMaps;
            List<SaveField> AllTargetObjectSaveOnlyFields = new List<SaveField>();
            AllSaveMaps.ForEach(sm => AllTargetObjectSaveOnlyFields.AddRange(sm.SaveFields.Where(sf => sf.FieldId.Equals(retField.FieldId)
                                                                                                    && sf.AppObject.Equals(retField.AppObject)
                                                                                                    && sf.SaveFieldType.Equals(SaveType.SaveOnlyField))));
            foreach (SaveField sf in AllTargetObjectSaveOnlyFields)
            {
                Excel.Range range = ExcelHelper.NextVerticalCell(ExcelHelper.GetRange(sf.TargetNamedRange), -1);
                Excel.Range LabelRangeOfTargetField = ExcelHelper.NextVerticalCell(Target, -1);
                if (range.Worksheet.Equals(Target.Worksheet) && (range.Row) == LabelRangeOfTargetField.Row)
                {
                    retField.TargetLocation = range.Worksheet.Name + "!" + range.Address;
                    return true;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="secondLevelApttusObject"></param>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        public RetrieveMapCellPermission Validate(Excel.Range Target, string secondLevelApttusObject, MapMode mapMode, out string firstValue, out string secondValue, RetrieveField retField)
        {
            firstValue = secondValue = String.Empty;
            string address = CellLayout == RepeatingCellLayout.Vertical ? Target.EntireRow.Address : Target.EntireColumn.Address;
            string mapKey = string.Format("{0}!{1}", Target.Worksheet.Name, address);

            // Check if Cell on which field is being dropped is part of existing Retrieve or Save Map, if yes Deny override
            string currentCellNamedRange = ExcelHelper.GetExcelCellName(Target);
            if (!string.IsNullOrEmpty(currentCellNamedRange))
            {
                RangeMap rangeMap = ConfigurationManager.GetInstance.GetAllRangeMaps().Where(r => !string.IsNullOrEmpty(r.RangeName) && r.RangeName.Equals(currentCellNamedRange)).FirstOrDefault();
                if (mapMode == MapMode.SaveMap && rangeMap != null && rangeMap.SaveMapId.Equals(Guid.Empty))
                    return RetrieveMapCellPermission.DenyOverridingRetrieveFields;

                if (mapMode == MapMode.RetrieveMap && rangeMap != null && rangeMap.RetrieveMapId.Equals(Guid.Empty))
                    return RetrieveMapCellPermission.DenyOverridingSaveFields;
            }

            if (!SecondLevelObjectByRange.ContainsKey(mapKey))
            {
                if (!SecondLevelObjectByRange.ContainsValue(secondLevelApttusObject))
                    SecondLevelObjectByRange[mapKey] = secondLevelApttusObject;
                else if (retrieveMap.RepeatingGroups.Where(rg => rg.AppObject.ToString() == secondLevelApttusObject).Count() > 0)
                    return RetrieveMapCellPermission.DenyAddingExistingObjectToNewRow;
                else
                    SecondLevelObjectByRange[mapKey] = secondLevelApttusObject; //add this field in the different sheet
            }
            else if (mapMode == MapMode.RetrieveMap)
            {
                //Check whether the fields being dropped in the row/column, belong to correct repeating group of correct retrieve map.
                foreach (KeyValuePair<string, string> r in SecondLevelObjectByRange)
                {
                    if (AllMapsOfObject.ContainsKey(r.Value))
                    {
                        List<RetrieveMap> maps = AllMapsOfObject[r.Value];
                        RetrieveMap usedMap = (from map in maps
                                               from rGroup in map.RepeatingGroups
                                               where ValidateUsedMap(rGroup, Target)
                                               select map).FirstOrDefault();

                        if (usedMap != null && usedMap.Id != retrieveMap.Id)
                            return RetrieveMapCellPermission.DenyAddingFieldsToDifferentDisplayMap;
                    }
                }
            }

            string targetLocation = ExcelHelper.GetAddress(Target);
            RepeatingGroup repGroup = retrieveMap.RepeatingGroups.Where(s => s.AppObject.Equals(Guid.Parse(secondLevelApttusObject))).FirstOrDefault();
            RetrieveField field = null;
            RetrieveMap MapWithSameField = null;
            if (repGroup != null)
            {
                field = (from retrieveField in repGroup.RetrieveFields
                         where retrieveField.TargetLocation == targetLocation
                         select retrieveField).FirstOrDefault();
            }

            //Validates current field with all Retrieve/Save map to check duplicates.
            if (AllMapsOfObject.ContainsKey(secondLevelApttusObject))
            {
                List<RetrieveMap> AllMaps = AllMapsOfObject[secondLevelApttusObject];
                MapWithSameField = (from map in AllMaps
                                    where ValidateOverridingRepeatingFields(map.RepeatingGroups, Target, retField)
                                    select map).FirstOrDefault();

                //If any map is found with same field ,RetrieveMapCellPermission.DenyOverridingSameFieldInGrid validation will be fired.
                if (MapWithSameField != null)
                {
                    return RetrieveMapCellPermission.DenyOverridingSameFieldInGrid;
                }
            }
            else
            {
                if (CheckCurrentRepeatingGroupAndSaveMaps(retField, Target))
                {
                    return RetrieveMapCellPermission.DenyOverridingSameFieldInGrid;
                }
            }

            if (field == null) // field doesn't exist in Excel
            {
                string secondLevelObject;
                SecondLevelObjectByRange.TryGetValue(mapKey, out secondLevelObject);
                if (secondLevelObject.Equals(secondLevelApttusObject)) //Check whether this field is same second level object in this row
                    return RetrieveMapCellPermission.AllowAdd;
                else
                {
                    firstValue = secondLevelObject;
                    secondValue = secondLevelApttusObject;
                    return RetrieveMapCellPermission.DenyAddingDifferentSecondLevelObject;
                }
            }
            else // field exist in excel and both the cell types are repeating
            {
                string secondLevelObject;
                SecondLevelObjectByRange.TryGetValue(mapKey, out secondLevelObject);
                if (secondLevelObject.Equals(secondLevelApttusObject)) //Check whether this field is same second level object in this row
                    return RetrieveMapCellPermission.AllowUpdate;
                else
                {
                    firstValue = secondLevelObject;
                    secondValue = secondLevelApttusObject;
                    return RetrieveMapCellPermission.DenyOverridingSecondLevelObject;
                }
            }
        }

        /// <summary>
        /// Checks whether we can add an independent cell in the target row/column as this row/column may be occupied by a repeating cell.
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool CanAddIndependentCell(Excel.Range Target)
        {
            string mapKey = CellLayout == RepeatingCellLayout.Vertical ? "$" + Target.Row.ToString() : string.Format("{0}!{1}", Target.Worksheet.Name, Target.EntireColumn.Address.Split(':').ElementAt(0));
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = SecondLevelObjectByRange.Where(map => map.Key.Contains(mapKey));
            bool bCanAddRepeatingCell = true;
            if (keyValuePairs.Count() != 0)
            {
                foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
                {
                    bCanAddRepeatingCell = !keyValuePair.Key.Contains(Target.Worksheet.Name);
                    if (!bCanAddRepeatingCell)
                        break;
                }
            }
            return bCanAddRepeatingCell;
        }

        /// <summary>
        /// Clears the dictionary
        /// </summary>
        public void Clear()
        {
            SecondLevelObjectByRange.Clear();
        }
    }

    class RetrieveMapCellValidator
    {
        private RetrieveMap retrieveMap;
        private IndependentCellValidator independentCellValidator;
        private RepeatingCellValidator repeatingCellValidator;
        public RetrieveMapCellValidator(RetrieveMap retrieveMapObj, RepeatingCellLayout layout, SaveMap CurrentSaveMap)
        {
            retrieveMap = retrieveMapObj;
            independentCellValidator = new IndependentCellValidator(retrieveMapObj, layout, CurrentSaveMap);
            repeatingCellValidator = new RepeatingCellValidator(retrieveMapObj, layout);
        }

        /// <summary>
        /// Validates a field for RetrieveMapCellPermissions
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="cellType"></param>
        /// <param name="secondLevelApttusObject"></param>
        /// <returns></returns>
        public RetrieveMapCellPermission Validate(Excel.Range Target, ObjectType cellType, string secondLevelApttusObject, MapMode mapMode,
                                         out string firstValue, out string secondValue, RetrieveField retrieveField)
        {

            firstValue = secondValue = String.Empty;
            RetrieveMapCellPermission permission = RetrieveMapCellPermission.DenyOverridingDifferentCellType;
            switch (cellType)
            {
                case ObjectType.Independent:
                    if (repeatingCellValidator.CanAddIndependentCell(Target))
                        permission = independentCellValidator.Validate(Target, secondLevelApttusObject, mapMode, retrieveField);
                    break;
                case ObjectType.Repeating:
                    if (independentCellValidator.CanAddRepeatingCell(Target))
                        permission = repeatingCellValidator.Validate(Target, secondLevelApttusObject, mapMode, out firstValue, out secondValue, retrieveField);
                    break;
            }
            return permission;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secondLevelObjectGuid"></param>
        /// <param name="targetLocation"></param>
        /// <param name="cellType"></param>
        public void RemoveFieldFromValidator(string secondLevelObjectGuid, string targetLocation, ObjectType cellType)
        {
            switch (cellType)
            {
                case ObjectType.Independent:
                    independentCellValidator.RemoveFieldFromValidator(targetLocation, secondLevelObjectGuid);
                    break;

                case ObjectType.Repeating:
                    repeatingCellValidator.RemoveFieldFromValidator(targetLocation, secondLevelObjectGuid);
                    break;
            }
        }

        public void UpdateLayout(RepeatingCellLayout layout)
        {
            independentCellValidator.CellLayout = layout;
            repeatingCellValidator.CellLayout = layout;
        }

        public void ClearRepeatingCellValidation(RepeatingCellLayout layout)
        {
            repeatingCellValidator.Clear();
            independentCellValidator.CellLayout = layout;
            repeatingCellValidator.CellLayout = layout;
        }
    }
}
