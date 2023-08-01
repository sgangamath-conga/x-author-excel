using System;
using System.Collections.Generic;
using System.Data;
using Apttus.XAuthor.Core;
using System.Text;
using System.Linq;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class SalesforceQueryAction : IQueryAction
    {
        private ObjectManager objectManager = ObjectManager.GetInstance;

        public SalesforceQueryAction(QueryActionModel Model, string[] InputDataName)
        {
            this.Model = Model;
            this.InputDataName = InputDataName;
            AppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(this.Model.TargetObject);
            salesforceQuery = new SalesforceQuery() { Object = AppObject, UserInfo = objectManager.getUserInfo() };
            ValidExpression = false;
            fullWhereClauseChunks = null;
            queryWithoutWhereClause = null;
        }

        public QueryActionModel Model { get; set; }
        public ApttusObject AppObject { get; set; }
        public string[] InputDataName { get; set; }
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }
        public bool ValidExpression { get; set; }
        public DataTable AppFieldsDataTable { get; set; }
        SalesforceQuery salesforceQuery;
        List<string> fullWhereClauseChunks;
        string queryWithoutWhereClause;
        bool chunkValues;
        List<string> queryStrings;

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public void PrepareWhereClause()
        {
            string whereClause = string.Empty;
            List<KeyValuePair<string, Guid>> UsedDataSets;

            // Replace the Excel Cell Reference values in the Search Filter before building the expression
            ExcelHelper.SetCellReferenceFilterValue(Model.WhereFilterGroups);
            ValidExpression = ExpressionBuilderHelper.GetExpression(Model.Id, Model.WhereFilterGroups, AppObject, InputDataName, out UsedDataSets,
                out whereClause, out chunkValues, out fullWhereClauseChunks, true);

            if (ValidExpression)
            {
                InputDataSetMap = UsedDataSets;
                switch (Model.QueryType)
                {
                    case QueryTypes.SELECT:

                        // Get select fields and handle lookups
                        List<string> appFields;
                        AppFieldsDataTable = configurationManager.GetDataTableFromAllAppFields(AppObject, false, true, out appFields);
                        salesforceQuery.DataTable = AppFieldsDataTable;
                        string selectFields = string.Join(Constants.COMMA, appFields);

                        // Build query
                        StringBuilder sb = new StringBuilder("SELECT ");
                        sb.Append(selectFields);
                        sb.Append(" FROM ");
                        sb.Append(AppObject.Id);
                        if (!string.IsNullOrEmpty(whereClause))
                        {
                            sb.Append(" WHERE ");
                            queryWithoutWhereClause = sb.ToString();
                            sb.Append(whereClause);
                        }
                        if (Model.MaxRecords != QueryActionModel.MAX_RECORDS_NOLIMIT)
                            sb.Append(" LIMIT " + Model.MaxRecords);

                        salesforceQuery.SOQL = sb.ToString();

                        //Memory Optimization
                        appFields.Clear();
                        selectFields = null;

                        appFields = null;
                        sb = null;
                        whereClause = null;

                        break;
                    default:
                        return;
                }
            }
        }

        public ActionResult GetResultDataSet(out ApttusDataSet resultData)
        {
            ActionResult result = new ActionResult();
            //Prepare Where Clause - Dont use chunking to begin with.
            ExpressionBuilderHelper.UseChunking = false;
            ObjectManager manager = ObjectManager.GetInstance;
            resultData = null;
            try
            {
                ApttusUserInfo userInfo = salesforceQuery.UserInfo;
                bool bUsedSubQuery = false;
                if (Model.WhereFilterGroups != null && salesforceQuery.SOQL.Length > Constants.SOQL_MAX_LENGTH)
                {
                    if (Model.WhereFilterGroups.Count > 0)
                    {
                        // 1. If the filter text exists and doesn't contain any OR Clause, then use subquery.
                        // 2. If the filter is empty, by default it is AND clause, hence use sub-query
                        if (string.IsNullOrEmpty(Model.WhereFilterGroups[0].FilterLogicText) || !Model.WhereFilterGroups[0].FilterLogicText.ToUpper().Contains("OR"))
                        {
                            bUsedSubQuery = true;
                            ExpressionBuilderHelper.UseSubQuery = true;
                            PrepareWhereClause();
                            ExpressionBuilderHelper.UseSubQuery = false;
                        }
                    }

                    // 3. Now check if either the SubQuery or Regular Query (where SubQuery was not possible) have exceeded 20K limit
                    if (salesforceQuery.SOQL.Length > Constants.SOQL_MAX_LENGTH)
                    {
                        bUsedSubQuery = false;
                        bool bChunkingExceddedSOQLLength = false;

                        ExpressionBuilderHelper.UseChunking = true;
                        PrepareWhereClause();
                        ExpressionBuilderHelper.UseChunking = false;

                        if (chunkValues && ValidExpression)
                        {
                            ChunkQueries queries = new ChunkQueries();
                            queryStrings = queries.PrepareChunkedQueries(fullWhereClauseChunks, queryWithoutWhereClause, out bChunkingExceddedSOQLLength);

                            if (bChunkingExceddedSOQLLength)
                            {
                                // Inform the user that chunking is also exceeding Salesforce limit. Use Dataset Action.
                                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking failed or could not be applied for QueryAction: " + Model.Name);
                                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Since chunking has failed or could not be applied for the query, please use Dataset Action.");
                                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking failed or could be not be applied for the query" + queryStrings[0]);

                                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("QueryActionController_Execute_SOQL_Limit_ErrorMsg"), resourceManager.GetResource("COMMON_SOQL_Limit_Msg"));
                                result.Status = ActionResultStatus.Failure;
                                return result;
                            }
                        }
                        else
                        {
                            // Here the chunking was not applied by PrepareWhereClause(). It could be due to various reasons like:
                            // - The individual filters didnot exceed the 
                            // Inform the user that chunking is also exceeding Salesforce limit. Use Dataset Action.
                            ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking could not be applied for QueryAction: " + Model.Name);
                            ExceptionLogHelper.InfoLog("SOQL Over-Limit: Since chunking could not be applied for the query, please use Dataset Action.");
                            ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking could be not be applied for the query:" + Environment.NewLine + salesforceQuery.SOQL);

                            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("QueryActionController_Execute_SOQL_Chunking_Limit_ErrorMsg"), resourceManager.GetResource("COMMON_SOQL_Limit_Msg"));
                            result.Status = ActionResultStatus.Failure;
                            return result;
                        }
                    }
                }

                if (!chunkValues)
                {
                    salesforceQuery.SOQL = salesforceQuery.SOQL.Replace(Constants.ATTACHMENT, Constants.ATTACHMENT_SUBQUERY);
                    resultData = manager.QueryDataSet(salesforceQuery);
                    //Salesforce only supports one level of sub-query, nested sub-queries are not supported. Hence if the current queryString is 
                    //using sub-query to retrieve the data, this sub-query cannot be used-again, hence set SOQL property of the dataset to null, so that
                    //this SOQL is not used in nested sub-queries again.
                    if (bUsedSubQuery)
                        resultData.SOQL = null;
                }
                else if (queryStrings != null && queryStrings.Count > 0) //Use Chunking
                {
                    salesforceQuery = null; //Memory Optimization
                    resultData = new ApttusDataSet()
                    {
                        DataTable = new DataTable(),
                        SOQL = string.Empty
                    };
                    var comparer = new DataRowComparer();

                    foreach (string query in queryStrings)
                    {
                        ApttusDataSet tempDataSet = manager.QueryDataSet(new SalesforceQuery { SOQL = query, Object = AppObject, DataTable = AppFieldsDataTable, UserInfo = userInfo });
                        IEnumerable<DataRow> rows = resultData.DataTable.AsEnumerable().Union(tempDataSet.DataTable.AsEnumerable(), comparer);
                        if (rows.Count() > 0)
                            resultData.DataTable = rows.CopyToDataTable();

                        tempDataSet.SOQL = null;
                        tempDataSet.DataTable.Dispose();
                        tempDataSet.DataTable = null;
                        tempDataSet = null;
                        rows = null;
                    }

                    //Memory Optimization
                    queryStrings.Clear();
                    queryStrings.Capacity = 0;
                    queryStrings = null;

                    if (fullWhereClauseChunks != null)
                    {
                        fullWhereClauseChunks.Clear();
                        fullWhereClauseChunks.Capacity = 0;
                        fullWhereClauseChunks = null;
                    }
                }
                result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                var Result = new ActionResult();
                ExceptionLogHelper.InfoLog("Query:" + Environment.NewLine + salesforceQuery.SOQL);
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Query Action");
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }
            finally
            {
                ExpressionBuilderHelper.UseSubQuery = false;
                ExpressionBuilderHelper.UseChunking = false;
                salesforceQuery = null; //Memory Optimization
                queryWithoutWhereClause = null;
            }

            return result;
        }

        public void SetDisplayableWhereClause(ref ApttusDataSet DataSet)
        {
            string filterclausetodisplay, whereclause;
            whereclause = ExpressionBuilderHelper.DisplayableWhereClause;

            if (string.IsNullOrEmpty(whereclause))
                filterclausetodisplay = resourceManager.GetResource("QueryActionController_EmptyFiltersText");
            else
                filterclausetodisplay = whereclause;

            DataSet.DisplayableWhereClause = new StringBuilder(AppObject.Name).Append(Constants.SPACE).Append(resourceManager.GetResource("QueryActionController_FiltersText")).Append(filterclausetodisplay).ToString();

        }
    }
}
