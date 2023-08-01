using Apttus.XAuthor.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    class FieldSynchronization
    {
        public FieldUsageInApp fieldUsageInApp = new FieldUsageInApp();
        private ConcurrentBag<PicklistTypeChangeInfo> picklistTypeChanges = new ConcurrentBag<PicklistTypeChangeInfo>();
        int picklistValuesSynced;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private object ExcelUILock;

        public FieldSynchronization()
        {
            ExcelUILock = new object();
        }

        internal void SynchronizeFields(ApttusObject synchedObject, ApttusObject unsynchedObject)
        {
            unsynchedObject.Fields.ForEach(field =>
            {
                ApttusField unsynchedField = appDefManager.GetField(unsynchedObject.UniqueId, field.Id);
                ApttusField synchedField = synchedObject.Fields.Find(fld => fld.Id == unsynchedField.Id);
                if (synchedField == null)
                {
                    bool bFieldMissing = false;
                    if (unsynchedField.LookupObject != null)
                    {
                        if (!IsLookupNameField(unsynchedField.Id))
                        {
                            if (unsynchedField.Datatype == Datatype.Lookup)
                            {
                                string lookupName = appDefManager.GetLookupNameFromLookupId(unsynchedField.Id);
                                ApttusFieldDS lookupField = unsynchedObject.GetField(lookupName) as ApttusFieldDS;
                                if (lookupField != null)
                                    fieldUsageInApp.CheckMissingFieldUsageInApp(synchedObject, unsynchedObject, lookupField);
                            }
                            bFieldMissing = true;
                        }
                    }
                    else
                        bFieldMissing = true;

                    if (bFieldMissing)
                        fieldUsageInApp.CheckMissingFieldUsageInApp(synchedObject, unsynchedObject, unsynchedField);
                }
                else if (synchedField.Datatype != unsynchedField.Datatype)
                {
                    DataTypeChangeInfo dataTypeChange = new DataTypeChangeInfo();
                    dataTypeChange.AppObjectId = unsynchedObject.Id;
                    dataTypeChange.FieldId = synchedField.Id;
                    dataTypeChange.From = unsynchedField.Datatype;
                    dataTypeChange.To = synchedField.Datatype;
                    dataTypeChange.SynchedField = synchedField;
                    fieldUsageInApp.CheckFieldUsageInApp(dataTypeChange);
                }
                else if (synchedField.Datatype == Datatype.Picklist)
                {
                    if (synchedField.PicklistType != unsynchedField.PicklistType)
                    {
                        //Picklist Type has changed
                        PicklistTypeChangeInfo picklistTypeChange = new PicklistTypeChangeInfo();
                        picklistTypeChange.AppObjectUniqueId = unsynchedObject.UniqueId;
                        picklistTypeChange.FieldId = synchedField.Id;
                        picklistTypeChange.From = unsynchedField.PicklistType;
                        picklistTypeChange.To = synchedField.PicklistType;

                        picklistTypeChanges.Add(picklistTypeChange);
                    }
                    else
                    {
                        //Need to make a delta of picklist values and see whether any removed picklist value is being used in SearchFilter.
                        if (!fieldUsageInApp.IsRemovedFieldPicklistValueUsedInSearchFilter(unsynchedObject.Id, unsynchedField.Id, synchedField.PicklistValues))
                        {
                            if (IsPicklistValuesChanged(synchedField.PicklistValues, unsynchedField.PicklistValues))
                                Interlocked.Add(ref picklistValuesSynced, 1);
                            unsynchedField.PicklistValues = synchedField.PicklistValues;
                        }
                    }
                }
                else if ((synchedField.Datatype == Datatype.Double || synchedField.Datatype == Datatype.Decimal))
                {
                    unsynchedField.Precision = synchedField.Precision;
                    unsynchedField.Scale = synchedField.Scale;
                }
            }
            );
        }
        bool IsPicklistValuesChanged(List<string> a, List<string> b)
        {
            return !(a.All(b.Contains) && b.All(a.Contains));
        }
        private bool IsLookupNameField(string name)
        {
            return (name.Contains(Constants.APPENDLOOKUPID) || name.Contains(Constants.APPENDLOOKUPNAME) || name.Contains(Constants.APPENDLOOKUPNAME_WITH_PARENTHESIS));
        }

        internal ConcurrentBag<MissingFieldItem> GetMissingFieldInfo()
        {
            return fieldUsageInApp.GetMissingFieldInfo();
        }

        internal ConcurrentBag<DataTypeChangeInfo> GetDataTypeMismatchFieldInfo()
        {
            return fieldUsageInApp.GetMismatchFieldInfo();
        }
        internal int GetPickListMismatchInfo()
        {
            return picklistValuesSynced;
        }
        private void RemoveFieldFromSaveMap(SaveMap saveMap, MissingFieldItem missingField)
        {
            List<SaveField> Savefields = (from sGroup in saveMap.SaveGroups
                                          from sField in saveMap.SaveFields
                                          where (
                                               (sField.FieldId == missingField.FieldId && (fieldUsageInApp.MatchObjectId(missingField.ObjectID, sField.AppObject))
                                          || (sField.AppObject == sGroup.AppObject && fieldUsageInApp.IsFieldRelational(missingField.ObjectID, sField.AppObject, missingField.FieldId, sField.MultiLevelFieldId)
                                          )))
                                          select sField).ToList();

            foreach (SaveField saveField in Savefields.ToList())
            {
                if (saveField.SaveFieldType == SaveType.RetrievedField)
                    saveMap.SaveFields.Remove(saveField);
                else if (saveField.SaveFieldType == SaveType.SaveOnlyField)
                {
                    SaveMapController controller = new SaveMapController(saveMap, null, FormOpenMode.Edit);
                    controller.ClearSaveField(saveField);
                    saveMap.SaveFields.Remove(saveField);
                }
            }
        }

        private void RemoveFieldFromDisplayMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MissingFieldItem missingField)
        {
            RetrieveField rField = displayMap.RetrieveFields.Find(rf => rf.FieldId == missingField.FieldId);
            if (rField != null)
            {
                Microsoft.Office.Interop.Excel.Range oFieldRange = ExcelHelper.GetRange(rField.TargetNamedRange);
                Excel.Range oLabelRange = ExcelHelper.NextHorizontalCell(oFieldRange, -1);
                oFieldRange.ClearContents();
                oLabelRange.ClearContents();
                retrieveMapController.RemoveField(null, rField);
            }

            var repeatingGroupRetrieveField = (from rGroup in displayMap.RepeatingGroups
                                               from rgField in rGroup.RetrieveFields
                                               where (
                                                    (rgField.FieldId == missingField.FieldId && (fieldUsageInApp.MatchObjectId(missingField.ObjectID, rgField.AppObject))
                                               || (rgField.AppObject != rGroup.AppObject && fieldUsageInApp.IsFieldRelational(missingField.ObjectID, rgField.AppObject, missingField.FieldId, rgField.MultiLevelFieldId)
                                               )))
                                               select new { rGroup, rgField }).FirstOrDefault();

            if (repeatingGroupRetrieveField != null && repeatingGroupRetrieveField.rgField != null)
            {

                lock (ExcelUILock)
                {
                    Excel.Range oFieldRange = ExcelHelper.GetRange(repeatingGroupRetrieveField.rgField.TargetNamedRange);
                    Excel.Range oLabelRange = ExcelHelper.NextVerticalCell(oFieldRange, -1);
                    oFieldRange.ClearContents();
                    oLabelRange.ClearContents();
                    retrieveMapController.RemoveField(repeatingGroupRetrieveField.rGroup, repeatingGroupRetrieveField.rgField);
                }
            }
        }

        protected void RemoveFieldFromSSQueryActionFilters(SearchAndSelect ssqueryaction, string fieldId)
        {
            SearchFilterGroup sg = ssqueryaction.SearchFilterGroups.FirstOrDefault();
            if (string.IsNullOrEmpty(sg.FilterLogicText))
            {
                sg.Filters.RemoveAll(x => x.FieldId == fieldId && x.ValueType == ExpressionValueTypes.UserInput);
                ResetSequenceNumbersforFilters(sg);
            }

            ssqueryaction.ResultFields.RemoveAll(x => x.Id == fieldId);
            if (!sg.Filters.Exists(x => x.FieldId == fieldId))
                ssqueryaction.SearchFields.RemoveAll(x => x.Id == fieldId);
        }

        protected void RemoveFieldFromQueryActionFilters(QueryActionModel queryaction, string fieldId)
        {
            string actionName = queryaction.Name;
            List<SearchFilterGroup> list = queryaction.WhereFilterGroups;

            SearchFilterGroup sg = list.FirstOrDefault();
            if (string.IsNullOrEmpty(sg.FilterLogicText))
            {
                sg.Filters.RemoveAll(x => x.FieldId == fieldId && x.ValueType == ExpressionValueTypes.UserInput);
                ResetSequenceNumbersforFilters(sg);
            }
        }

        void ResetSequenceNumbersforFilters(SearchFilterGroup searchFilterGroup)
        {
            int seqNo = 1;
            foreach (SearchFilter sf in searchFilterGroup.Filters.OrderBy(x => x.SequenceNo))
            {
                sf.SequenceNo = seqNo;
                seqNo++;
            }
        }

        internal void RemoveFields(List<MissingFieldItem> removedFields)
        {
            if (removedFields == null)
                return;

            removedFields.ForEach(removeField =>
            {
                RemoveFieldsFromDisplayMap(removeField);
                RemoveFieldsFromSaveMap(removeField);
                RemoveFieldsFromActionFilters(removeField);

                if (removeField.FieldMissingInDisplayMaps.Count == 0 && removeField.FieldMissingInSaveMaps.Count == 0 && removeField.FieldMissingInActions.Count == 0)
                {
                    fieldUsageInApp.missingFields = new ConcurrentBag<MissingFieldItem>(fieldUsageInApp.missingFields.Except(new[] { removeField }));
                    RemoveFieldsFromAppDef(removeField);
                }
            });
        }

        private void RemoveFieldsFromAppDef(MissingFieldItem removeField)
        {
            appDefManager.RemoveField(appDefManager.GetAppObjectById(removeField.ObjectID).FirstOrDefault(), removeField.FieldId, appDefManager.AppObjects);
        }

        private void RemoveFieldsFromDisplayMap(MissingFieldItem missingField)
        {
            List<RetrieveMap> displayMapstobeRemoved = new List<RetrieveMap>();
            missingField.FieldMissingInDisplayMaps.ForEach((frmap) =>
            {
                RetrieveMapController retrieveMapController = new RetrieveMapController(null, null);
                RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == frmap.Id);
                retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                RemoveFieldFromDisplayMap(retrieveMapController, displayMap, missingField);

                lock (ExcelUILock)
                    retrieveMapController.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, retrieveMapController.RetrieveMap.RepeatingGroups[0]);

                displayMapstobeRemoved.Add(frmap);
            });
            UpdateFieldMissingInDisplayMaps(missingField, displayMapstobeRemoved);
        }

        private void RemoveFieldsFromSaveMap(MissingFieldItem missingField)
        {
            List<SaveMap> saveMapstobeRemoved = new List<SaveMap>();
            missingField.FieldMissingInSaveMaps.ForEach((fsmap) =>
            {
                SaveMap saveMap = configManager.SaveMaps.Find(sMap => sMap.Id == fsmap.Id);
                RemoveFieldFromSaveMap(saveMap, missingField);
                saveMapstobeRemoved.Add(fsmap);
            });
            UpdateFieldMissingInSaveMaps(missingField, saveMapstobeRemoved);
        }

        private void RemoveFieldsFromActionFilters(MissingFieldItem missingField)
        {
            List<Core.Action> actionstobeRemoved = new List<Core.Action>();
            missingField.FieldMissingInActions.ForEach((action) =>
            {
                if (action.Type == Constants.SEARCH_AND_SELECT_ACTION)
                    RemoveFieldFromSSQueryActionFilters(action as SearchAndSelect, missingField.FieldId);
                else
                    RemoveFieldFromQueryActionFilters(action as QueryActionModel, missingField.FieldId);

                if (!fieldUsageInApp.IsMissingFieldPartOfActionFilters(action, missingField))
                    actionstobeRemoved.Add(action);
            });
            UpdateFieldMissingInActions(missingField, actionstobeRemoved);
        }

        private void UpdateFieldMissingInDisplayMaps(MissingFieldItem missingField, List<RetrieveMap> displayMapstobeRemoved)
        {
            foreach (RetrieveMap rMap in displayMapstobeRemoved)
                missingField.FieldMissingInDisplayMaps.Remove(rMap);
        }

        private void UpdateFieldMissingInSaveMaps(MissingFieldItem missingField, List<SaveMap> saveMapstobeRemoved)
        {
            foreach (SaveMap sMap in saveMapstobeRemoved)
                missingField.FieldMissingInSaveMaps.Remove(sMap);
        }

        void UpdateFieldMissingInActions(MissingFieldItem missingField, List<Core.Action> actionstobeRemoved)
        {
            foreach (Core.Action action in actionstobeRemoved)
                missingField.FieldMissingInActions.Remove(action);
        }
    }
}
