using Apttus.XAuthor.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class FieldUsageInApp
    {
        internal ConfigurationManager configManager = ConfigurationManager.GetInstance;
        internal ConcurrentBag<MissingFieldItem> missingFields { get; set; }
        internal ConcurrentBag<DataTypeChangeInfo> mismatchFields { get; set; }

        internal ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

        internal FieldUsageInApp()
        {
            missingFields = new ConcurrentBag<MissingFieldItem>();
            mismatchFields = new ConcurrentBag<DataTypeChangeInfo>();
        }

        internal bool IsRemovedFieldPicklistValueUsedInSearchFilter(string objectId, string fieldId, List<string> picklistValuesInSalesforce)
        {
            List<Core.Action> dataGetterActions = configManager.Actions.Where(act => act.Type == Constants.SEARCH_AND_SELECT_ACTION || act.Type == Constants.EXECUTE_QUERY_ACTION).ToList();

            var actionsWithFilter = (from action in dataGetterActions
                                     where GetSearchFilterGroupFromAction(action) != null
                                     from filterGroup in GetSearchFilterGroupFromAction(action)
                                     where filterGroup != null
                                     from filter in filterGroup.Filters
                                     where filter.FieldId == fieldId && MatchObjectId(objectId, filter.AppObjectUniqueId)
                                     select new { Action = action, Filter = filter });

            //we dont need to check filters - where values are expected runtime like user input, input or cell reference. so added condition to check filters only with value type = static.
            foreach (var actionWithFilter in actionsWithFilter.Where(x=>x.Filter.ValueType == ExpressionValueTypes.Static))
            {
                if (!picklistValuesInSalesforce.Contains(actionWithFilter.Filter.Value))
                    return true;
            }
            return false;
        }

        private List<SearchFilterGroup> GetSearchFilterGroupFromAction(Core.Action action)
        {
            switch (action.Type)
            {
                case Constants.EXECUTE_QUERY_ACTION:
                    return (action as QueryActionModel).WhereFilterGroups;
                case Constants.SEARCH_AND_SELECT_ACTION:
                    return (action as SearchAndSelect).SearchFilterGroups;
            }
            return null;
        }

        internal bool IsMissingFieldPartOfActionFilters(Core.Action dataGetterAction, MissingFieldItem missingFieldItem)
        {
            List<SearchFilterGroup> searchFilterGroups = GetSearchFilterGroupFromAction(dataGetterAction);
            if (searchFilterGroups != null && searchFilterGroups.Count > 0)
            {
                SearchFilterGroup searchFilterGroup = searchFilterGroups[0];
                if (searchFilterGroup.Filters != null && searchFilterGroup.Filters.Count > 0)
                    return searchFilterGroup.Filters.Exists(sf => sf.FieldId == missingFieldItem.FieldId && MatchObjectId(missingFieldItem.ObjectID, sf.AppObjectUniqueId));
            }
            return false;
        }

        internal IEnumerable<Core.Action> AddIfFieldIsPartOfSearchFilters(MissingFieldItem missingFieldItem)
        {
            IEnumerable<Core.Action> actions = IsFieldPartOfSearchFilters(missingFieldItem.FieldId, missingFieldItem.ObjectID);

            if (actions.Count() > 0)
                missingFieldItem.Add(actions);

            return actions;
        }

        private bool AddIfFieldIsPartOfRetrieveMap(MissingFieldItem missingFieldItem)
        {
            IEnumerable<RetrieveMap> rMaps;
            bool bIsPartOfRetrieveMaps = IsFieldPartOfRetrieveMap(missingFieldItem.FieldId, missingFieldItem.ObjectID, out rMaps);
            if (bIsPartOfRetrieveMaps)
            {
                missingFieldItem.Add(rMaps);
            }
            return bIsPartOfRetrieveMaps;
        }

        public bool MatchObjectId(string ObjectId, Guid fieldAppObjectId)
        {
            ApttusObject obj = applicationDefinitionManager.GetAppObject(fieldAppObjectId);
            return obj != null && obj.Id == ObjectId;
        }

        private bool IsMissingFieldPartOfSaveMap(DataTypeChangeInfo mismatchField)
        {
            IEnumerable<SaveMap> SaveMaps = IsMissingFieldPartOfSaveMap(mismatchField.FieldId, mismatchField.AppObjectId);

            if (SaveMaps.Count() > 0)
                mismatchField.Add(SaveMaps);

            return SaveMaps.Count() > 0;
        }

        private IEnumerable<SaveMap> IsMissingFieldPartOfSaveMap(string fieldID, string objectID)
        {
            IEnumerable<SaveMap> SaveMaps = (from saveMap in configManager.SaveMaps
                                             from sGroup in saveMap.SaveGroups
                                             from sField in saveMap.SaveFields
                                             where (
                                                  (sField.FieldId == fieldID && (MatchObjectId(objectID, sField.AppObject))
                                             || (sField.AppObject == sGroup.AppObject && IsFieldRelational(objectID, sField.AppObject, fieldID, sField.MultiLevelFieldId)
                                             )))
                                             select saveMap).Distinct();
            return SaveMaps;
        }

        private bool IsMissingFieldPartOfSaveMap(MissingFieldItem missingField)
        {
            IEnumerable<SaveMap> SaveMaps = IsMissingFieldPartOfSaveMap(missingField.FieldId, missingField.ObjectID);

            if (SaveMaps.Count() > 0)
                missingField.Add(SaveMaps);

            return SaveMaps.Count() > 0;
        }

        internal void CheckMissingFieldUsageInApp(ApttusObject synchedObject, ApttusObject unsynchedObject, ApttusField unsynchedField)
        {
            MissingFieldItem missingFieldItem = new MissingFieldItem { ObjectID = unsynchedObject.Id, ObjectName = unsynchedObject.Name, FieldName = unsynchedField.Name, FieldId = unsynchedField.Id };

            bool bIsPartOfSaveMap = IsMissingFieldPartOfSaveMap(missingFieldItem);
            bool bIsPartOfDisplayMap = AddIfFieldIsPartOfRetrieveMap(missingFieldItem);
            bool bIsPartOfFilters = AddIfFieldIsPartOfSearchFilters(missingFieldItem).Count() > 0;

            if (bIsPartOfDisplayMap || bIsPartOfSaveMap || bIsPartOfFilters)
                missingFields.Add(missingFieldItem);
        }

        internal ConcurrentBag<MissingFieldItem> GetMissingFieldInfo()
        {
            return missingFields;
        }

        #region DataTypeMismatch 
        internal ConcurrentBag<DataTypeChangeInfo> GetMismatchFieldInfo()
        {
            return mismatchFields;
        }

        internal IEnumerable<Core.Action> AddIfFieldIsPartOfSearchFilters(DataTypeChangeInfo datatypemismatchField)
        {
            IEnumerable<Core.Action> actions = IsFieldPartOfSearchFilters(datatypemismatchField.FieldId, datatypemismatchField.AppObjectId);

            if (actions.Count() > 0)
                datatypemismatchField.Add(actions);

            return actions;
        }

        internal IEnumerable<Core.Action> IsFieldPartOfSearchFilters(string fieldID, string objectID)
        {
            IEnumerable<Core.Action> dataGetterActions = configManager.Actions.Where(act => act.Type == Constants.SEARCH_AND_SELECT_ACTION || act.Type == Constants.EXECUTE_QUERY_ACTION);

            IEnumerable<Core.Action> actions = (from action in dataGetterActions
                                                where GetSearchFilterGroupFromAction(action) != null
                                                from filterGroup in GetSearchFilterGroupFromAction(action)
                                                where filterGroup != null
                                                from filter in filterGroup.Filters
                                                where filter.FieldId == fieldID && MatchObjectId(objectID, filter.AppObjectUniqueId)
                                                select action);


            return actions;
        }

        public bool IsFieldRelational(string objectId, Guid AppObjectId, string fieldId, string relationalMultilevelfieldId)
        {
            ApttusObject obj = applicationDefinitionManager.GetAppObject(AppObjectId);
            if (obj != null && relationalMultilevelfieldId != null)
            {
                if (obj.Id == objectId)
                {
                    string[] relationalFieldValues = relationalMultilevelfieldId.Split('-');
                    if (relationalFieldValues.Count() > 0)
                    {
                        string objFieldId = relationalFieldValues[relationalFieldValues.Count() - 1];
                        if (objFieldId == fieldId)
                            return true;
                    }
                }
            }
            return false;
        }

        private bool IsFieldPartOfRetrieveMap(string fieldID, string objectID, out IEnumerable<RetrieveMap> maps)
        {
            IEnumerable<RetrieveMap> rMaps = (from rm in configManager.RetrieveMaps
                                              from rf in rm.RetrieveFields
                                              where (rf.FieldId == fieldID && MatchObjectId(objectID, rf.AppObject))
                                              select rm).Distinct();

            //Individual Fields
            int nIndividualFieldsUsed = rMaps.Count();

            IEnumerable<RetrieveMap> rgMaps = (from rm in configManager.RetrieveMaps
                                               from rg in rm.RepeatingGroups
                                               from rf in rg.RetrieveFields
                                               where (
                                                    (rf.FieldId == fieldID && (MatchObjectId(objectID, rf.AppObject))
                                               || (rf.AppObject != rg.AppObject && IsFieldRelational(objectID, rf.AppObject, fieldID, rf.MultiLevelFieldId)
                                               )))
                                               select rm).Distinct();

            //RepeatingGroup Fields.
            int nRepeatingGroupFieldsUsed = rgMaps.Count();

            maps = rMaps.Union(rgMaps);

            return nIndividualFieldsUsed > 0 || nRepeatingGroupFieldsUsed > 0;
        }

        private bool AddIfFieldIsPartOfRetrieveMap(DataTypeChangeInfo mismatchFieldItem)
        {
            IEnumerable<RetrieveMap> rMaps;
            bool bIsPartOfRetrieveMaps = IsFieldPartOfRetrieveMap(mismatchFieldItem.FieldId, mismatchFieldItem.AppObjectId, out rMaps);
            if (bIsPartOfRetrieveMaps)
            {
                mismatchFieldItem.Add(rMaps);
            }
            return bIsPartOfRetrieveMaps;
        }

        internal void CheckFieldUsageInApp(DataTypeChangeInfo mismatchFieldItem)
        {
            bool bIsPartOfRetrieveMaps = AddIfFieldIsPartOfRetrieveMap(mismatchFieldItem);
            bool bIsPartOfFilters = AddIfFieldIsPartOfSearchFilters(mismatchFieldItem).Count() > 0;
            bool bIsPartOfSaveMap = IsMissingFieldPartOfSaveMap(mismatchFieldItem);
            mismatchFields.Add(mismatchFieldItem);
        }
        #endregion
    }
}
