using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    class SearchAndSelectSalesforceAction : ISearchAndSelectAction
    {
        public SearchAndSelect model;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        public ApttusObject appObject;
        DataManager dataManager = DataManager.GetInstance;
        public string whereClause = string.Empty;
        ObjectManager objectManager = ObjectManager.GetInstance;
        public string[] InputDataName { get; set; }
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        List<string> selectFields;
        List<ResultField> resultFields;
        bool ValidExpression = false;
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }

        private List<ResultField> GetResultFields()
        {
            List<ResultField> fields = new List<ResultField>();
            fields.AddRange((from field in model.ResultFields
                             where fieldLevelSecurity.IsFieldVisible(appObject.UniqueId, field.Id)
                             select field).ToList());
            return fields;
        }

        private Dictionary<string, object> GetWhereClause(string globalSearchString, string advancedFilterWhereClause, bool bClear)
        {
            List<KeyValuePair<string, Guid>> UsedDataSets;
            bool chunkValues;
            List<string> fullWhereClauseChunks;

            ValidExpression = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out UsedDataSets, out whereClause, out chunkValues, out fullWhereClauseChunks);
            if (ValidExpression)
            {
                InputDataSetMap = UsedDataSets;
            }

            // Prepare Select Fields List and Select Clause
            string selectClause = string.Join(Constants.COMMA, selectFields);

            // Prepare the "Order By" Clause
            string orderByClause = string.Empty;
            if (resultFields.Any(r => r.IsSortField))
                orderByClause = " ORDER BY " + resultFields.FirstOrDefault(r => r.IsSortField).Id;

            // Prepare user Where Clause
            // Removing combination of Advanced search as search will happens as EITHER OR, i.e. Either Global search OR Advanced Search
            string userGlobalWhereClause = GetGlobalSearchWhereClause(globalSearchString);
            //string userWhereClause = userGlobalWhereClause.Length > 0 ? userGlobalWhereClause : string.Empty;
            string userWhereClause = userGlobalWhereClause.Length > 0 && advancedFilterWhereClause.Length > 0
                ? userGlobalWhereClause + " AND " + advancedFilterWhereClause
                : userGlobalWhereClause + advancedFilterWhereClause; // Either one or both have length of 0, so AND is not required

            // Get Where clause for Advanced search from User Input fields combined with other Filters
            string uiWhereClause = whereClause;
            if (!bClear)
            {
                List<KeyValuePair<string, Guid>> tempUsedDataSets;
                bool bValid = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out tempUsedDataSets, out uiWhereClause, out chunkValues, out fullWhereClauseChunks, true);

                // Incase of advance filter logic no need to replace where clause with "And"
                SearchFilterGroup searchFilterGroup = model.SearchFilterGroups.FirstOrDefault();
                if (searchFilterGroup != null)
                {
                    if (!searchFilterGroup.IsAddFilter)
                    {
                        if (bValid && !(string.IsNullOrEmpty(uiWhereClause)))
                            uiWhereClause = uiWhereClause.Replace(" OR ", " AND "); // replace any OR with AND
                        else
                            uiWhereClause = whereClause;
                    }
                }
            }

            string combinedWhereClause = uiWhereClause.Length > 0 && userWhereClause.Length > 0
                ? uiWhereClause + " AND " + userWhereClause
                : uiWhereClause + userWhereClause; // Either one or both have length of 0, so AND is not required

            string soqlQuery = "SELECT " + selectClause + " FROM " + appObject.Id +
                (combinedWhereClause.Length > 0
                ? " WHERE " + combinedWhereClause
                : string.Empty) +
                orderByClause +
                " LIMIT " + Convert.ToString(Constants.QUERYRESULT_RECORDLIMIT);
            Dictionary<string, object> whereClauses = new Dictionary<string, object>();
            whereClauses["soqlQuery"] = soqlQuery;
            whereClauses["combinedWhereClause"] = combinedWhereClause;
            whereClauses["selectFields"] = selectFields;
            return whereClauses;
        }

        public SearchAndSelectData GetResultData(SearchAndSelect model, ApttusObject appObject, List<string> selectFields, List<ResultField> resultFields, string globalSearchString, string advancedFilterWhereClause, bool bClear, string[] InputDataName)
        {
            this.model = model;
            this.appObject = appObject;
            this.selectFields = selectFields;
            this.resultFields = resultFields;
            this.InputDataName = InputDataName;


            Dictionary<string, object> whereClauses = GetWhereClause(globalSearchString, advancedFilterWhereClause, bClear);
            string combinedWhereClause = whereClauses["combinedWhereClause"].ToString();
            string soqlQuery = whereClauses["soqlQuery"].ToString();


            DataTable dataTable = configurationManager.GetDataTableFromAppFields(appObject, selectFields);
            ApttusDataSet resultData = objectManager.QueryDataSet(new SalesforceQuery { SOQL = soqlQuery, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });

            return new SearchAndSelectData
            {
                ResultDataTable = resultData.DataTable,
                whereClause = combinedWhereClause
            };
        }

        private string GetGlobalSearchWhereClause(string searchCriteria)
        {
            string userWhereClause = string.Empty;
            if (!string.IsNullOrEmpty(searchCriteria))
            {
                searchCriteria = objectManager.EscapeQueryString(searchCriteria);

                List<string> whereClauses = new List<string>();
                List<string> fieldsForWhereClause = new List<string>();

                // This section is for searches by the SearchFields of S&S
                foreach (SearchField sf in GetSearchFields())
                {
                    ApttusField apttusField = appObject.Fields.FirstOrDefault(f => f.Id == sf.Id);
                    // Ignore Lookup Id field from Global Search
                    if (apttusField.Datatype != Datatype.Lookup)
                    {
                        string fieldWhereClause = ExpressionBuilderHelper.GetLikeWhereClauseBasedOnDataType(searchCriteria, apttusField);
                        // Add Where clause only if it was constructed
                        if (!string.IsNullOrEmpty(fieldWhereClause))
                        {
                            whereClauses.Add(fieldWhereClause);
                            fieldsForWhereClause.Add(apttusField.Id);
                        }
                    }
                }

                // This section is for searches by User Input fields
                if (this.model.SearchFilterGroups.Count > 0 && this.model.SearchFilterGroups[0].Filters.Count > 0)
                {
                    // There is only one SearchFilterGroup which contains list of SearchFilters hence use 0 index
                    var userInputSearchFilters = this.model.SearchFilterGroups[0].Filters.Where(f => f.ValueType == ExpressionValueTypes.UserInput);
                    if (userInputSearchFilters.Any())
                        foreach (SearchFilter filter in userInputSearchFilters)
                        {
                            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(filter.AppObjectUniqueId);
                            ApttusField apttusField = apttusObject.Fields.Where(f => f.Id.Equals(filter.FieldId)).FirstOrDefault();
                            // Ignore Lookup Id field from Global Search
                            if (apttusField.Datatype != Datatype.Lookup)
                            {
                                string LHSObjectField = ExpressionBuilderHelper.GetLHSQueryTextFromQueryObjects(filter);
                                // Don't add the same field if its added as a part of Search Field
                                if (!fieldsForWhereClause.Contains(LHSObjectField))
                                {
                                    string fieldWhereClause = ExpressionBuilderHelper.GetLikeWhereClauseBasedOnDataType(searchCriteria, LHSObjectField, apttusField.Datatype);
                                    // Add Where clause only if it was constructed
                                    if (!string.IsNullOrEmpty(fieldWhereClause))
                                    {
                                        whereClauses.Add(fieldWhereClause);
                                        fieldsForWhereClause.Add(LHSObjectField);
                                    }
                                }
                            }
                        }
                }
                if (whereClauses.Count > 0)
                {
                    userWhereClause = Constants.OPEN_BRACKET;
                    userWhereClause += string.Join<string>(" OR ", whereClauses);
                    userWhereClause += Constants.CLOSE_BRACKET;
                }
            }
            return userWhereClause;
        }

        private List<SearchField> GetSearchFields()
        {
            List<SearchField> fields = new List<SearchField>();
            fields.AddRange((from field in model.SearchFields
                             where fieldLevelSecurity.IsFieldVisible(appObject.UniqueId, field.Id)
                             select field).ToList());
            return fields;
        }

        public ApttusDataSet GetData(SearchAndSelectActionView view, ActionResult Result)
        {
            List<string> appFields = new List<string>();
            DataTable dataTable = configurationManager.GetDataTableFromAllAppFields(appObject, false, true, out appFields);
            string selectFields = string.Join(Constants.COMMA, appFields);
            string queryString = string.Empty;
            // Create query string
            if ((!view.searchAndSelectData.SelectAllInSalesforce) && !string.IsNullOrEmpty(view.searchAndSelectData.selectedId))
                queryString = "SELECT " + selectFields + " FROM " + appObject.Id + " WHERE Id in (" + view.searchAndSelectData.selectedId + ")";
            else if (view.searchAndSelectData.SelectAllInSalesforce)
                queryString = "SELECT " + selectFields + " FROM " + appObject.Id
                    + (string.IsNullOrEmpty(view.searchAndSelectData.whereClause) ? string.Empty : " WHERE " + view.searchAndSelectData.whereClause);

            ApttusDataSet dataset = null;
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.Replace(Constants.ATTACHMENT, Constants.ATTACHMENT_SUBQUERY);
                dataset = objectManager.QueryDataSet(new SalesforceQuery
                {
                    SOQL = queryString,
                    Object = appObject,
                    DataTable = dataTable,
                    UserInfo = Globals.ThisAddIn.userInfo
                });
            }
            else
            {
                Result.Status = ActionResultStatus.Failure;
            }
            return dataset;
        }

        public bool GetValidExpression(string Id, List<SearchFilterGroup> SearchFilterGroups, ApttusObject appObject, string[] InputDataName, out List<KeyValuePair<string, Guid>> UsedDataSets)
        {
            bool chunkValues;
            List<string> fullWhereClauseChunk;
            return ExpressionBuilderHelper.GetExpression(Id, SearchFilterGroups, appObject, InputDataName, out UsedDataSets, out whereClause, out chunkValues, out fullWhereClauseChunk);
        }
    }
}
