using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Microsoft.Xrm.Sdk.Query;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    class SearchAndSelectDynamicAction : ISearchAndSelectAction
    {
        public SearchAndSelect model;
        public ApttusObject appObject;
        ObjectManager objectManager = ObjectManager.GetInstance;
        public string[] InputDataName { get; set; }
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        List<string> selectFields;
        List<ResultField> resultFields;

        public SearchAndSelectData GetResultData(SearchAndSelect model, ApttusObject appObject, List<string> selectFields, List<ResultField> resultFields, string globalSearchString, string advancedFilterWhereClause, bool bClear, string[] InputDataName)
        {

            this.model = model;
            this.appObject = appObject;
            this.selectFields = selectFields;
            this.resultFields = resultFields;

            DataTable dataTable = configurationManager.GetDataTableFromAppFields(appObject, selectFields);
            QueryExpression crmQuery = null;
            List<KeyValuePair<string, Guid>> tempUsedDataSets;
            bool bValid = ExpressionBuilderHelperDynamics.GetExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out tempUsedDataSets, out crmQuery, !bClear, true, globalSearchString);
            ApttusDataSet resultData = objectManager.QueryDataSet(new DynamicsQuery { Query = crmQuery, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });

            return new SearchAndSelectData
            {
                ResultDataTable = resultData.DataTable,
                whereClause = string.Empty
            };
        }

        public ApttusDataSet GetData(SearchAndSelectActionView view, ActionResult Result)
        {
            List<string> appFields = new List<string>();
            DataTable dataTable = configurationManager.GetDataTableFromAllAppFields(appObject, false, true, out appFields);

            string ID_ATTRIBUTE = appObject.IdAttribute;
            List<SearchFilterGroup> searchFilterGroupList = null;
            if (!view.searchAndSelectData.SelectAllInSalesforce && !string.IsNullOrEmpty(view.searchAndSelectData.selectedId))
                searchFilterGroupList = prepareSearchFilterGroup(ID_ATTRIBUTE, "in", Datatype.Lookup, view.searchAndSelectData.selectedId);
            else if (view.searchAndSelectData.SelectAllInSalesforce)
                searchFilterGroupList = model.SearchFilterGroups;

            List<KeyValuePair<string, Guid>> tempUsedDataSets;
            QueryExpression query = null;
            bool isValid = ExpressionBuilderHelperDynamics.GetExpression(model.Id, searchFilterGroupList, appObject, InputDataName, out tempUsedDataSets, out query, true);
            ApttusDataSet dataset = objectManager.QueryDataSet(new DynamicsQuery { Query = query, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });

            return dataset;

        }
        private List<SearchFilterGroup> prepareSearchFilterGroup(string whereFieldId, string filterOperation, Datatype dataType, string filterValues)
        {
            /* Create Search Filter Collection of Filters to get QueryExpression from ExpressionBuilderHelperDynamics -> GetExpression */
            List<SearchFilterGroup> searchFilterGroupList = new List<SearchFilterGroup>();
            SearchFilterGroup searchFilterGroup = new SearchFilterGroup();
            SearchFilter searchFilter = new SearchFilter();
            QueryObject queryObject = new QueryObject();

            // Query Object Properies
            queryObject.AppObjectUniqueId = appObject.UniqueId;
            queryObject.LeafAppObjectUniqueId = appObject.UniqueId;
            queryObject.RelationshipType = QueryRelationshipType.None;
            queryObject.SequenceNo = 1;

            // Search Filter Properties
            searchFilter.AppObjectUniqueId = appObject.UniqueId;
            searchFilter.FieldId = whereFieldId;
            searchFilter.Operator = filterOperation;
            searchFilter.SequenceNo = 1;
            searchFilter.Value = filterValues;
            searchFilter.ValueType = ExpressionValueTypes.Static;
            searchFilter.QueryObjects = new List<QueryObject>();
            searchFilter.QueryObjects.Add(queryObject);

            // Search Filter Group Properties
            searchFilterGroup.Filters = new List<SearchFilter>();
            searchFilterGroup.Filters.Add(searchFilter);
            searchFilterGroup.LogicalOperator = Core.LogicalOperator.AND;

            searchFilterGroupList.Add(searchFilterGroup);

            return searchFilterGroupList;
        }

        public bool GetValidExpression(string Id, List<SearchFilterGroup> SearchFilterGroups, ApttusObject appObject, string[] InputDataName, out List<KeyValuePair<string, Guid>> UsedDataSets)
        {
            var queryExpr = new QueryExpression();
            ExpressionBuilderHelperDynamics.GetExpression(Id, SearchFilterGroups, appObject, InputDataName, out UsedDataSets, out queryExpr, false);
            return true;
        }
    }
}

