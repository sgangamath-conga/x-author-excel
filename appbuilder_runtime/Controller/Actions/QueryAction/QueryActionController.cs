/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using System.Data;
using Apttus.XAuthor.AppRuntime.Helpers;
using System.Web.Script.Serialization;
using Apttus.XAuthor.Core.Model.GoogleActions;

namespace Apttus.XAuthor.AppRuntime
{
    class QueryActionController
    {
        IQueryAction QueryAction;
        public ActionResult Result { get; private set; }
        public bool InputData { get; set; }
        public string[] InputDataName { get; set; }
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }
        public bool OutputPersistData { get; set; }
        public string OutputDataName { get; set; }
        private QueryActionModel model;

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        DataManager dataManager = DataManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public GoogleActionQueryController googleQueryController
        {
            get;
            set;
        }

        public QueryActionController(QueryActionModel model, string[] inputDataName)
        {
            //ExceptionLogHelper.DebugLog("query action controller :");

            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                QueryAction = Globals.ThisAddIn.GetQueryActionProvider(model, inputDataName);
            else
                QueryAction = new SalesforceQueryAction(model, inputDataName);

            this.model = model;
            InputDataName = inputDataName;
            Result = new ActionResult();
        }

        private bool AreFilterFieldsVisible(out StringBuilder sb)
        {
            sb = new StringBuilder();
            bool bVisible = true;
            SearchFilterGroup searchFilterGroup = model.WhereFilterGroups.FirstOrDefault();
            if (searchFilterGroup == null) return bVisible;
            foreach (SearchFilter filter in searchFilterGroup.Filters)
            {
                bool bTempVisible = fieldLevelSecurity.IsFieldVisible(filter.AppObjectUniqueId, filter.FieldId);
                bVisible = bVisible && bTempVisible;
                if (!bTempVisible)
                    sb.Append(filter.FieldId).Append(", ");
            }
            return bVisible;
        }

# region Old PrepareWhereClause
        //private void PrepareWhereClause()
        //{
        //    string whereClause = string.Empty;
        //    List<KeyValuePair<string, Guid>> UsedDataSets;

        //    // Replace the Excel Cell Reference values in the Search Filter before building the expression
        //    ExcelHelper.SetCellReferenceFilterValue(model.WhereFilterGroups);
        //    ValidExpression = ExpressionBuilderHelper.GetExpression(model.Id, model.WhereFilterGroups, appObject, InputDataName, out UsedDataSets,
        //        out whereClause, out chunkValues, out fullWhereClauseChunks, true);

        //    if (ValidExpression)
        //    {
        //        InputDataSetMap = UsedDataSets;
        //        switch (model.QueryType)
        //        {
        //            case QueryTypes.SELECT:

        //                // Get select fields and handle lookups
        //                List<string> appFields = new List<string>();
        //                dataTable = configurationManager.GetDataTableFromAllAppFields(appObject, false, true, out appFields);
        //                string selectFields = string.Join(Constants.COMMA, appFields);

        //                // Build query
        //                StringBuilder sb = new StringBuilder("SELECT ");
        //                sb.Append(selectFields);
        //                sb.Append(" FROM ");
        //                sb.Append(appObject.Id);
        //                if (!string.IsNullOrEmpty(whereClause))
        //                {
        //                    sb.Append(" WHERE ");
        //                    queryWithoutWhereClause = sb.ToString();
        //                    sb.Append(whereClause);
        //                }
        //                if (model.MaxRecords != QueryActionModel.MAX_RECORDS_NOLIMIT)
        //                    sb.Append(" LIMIT " + model.MaxRecords);

        //                queryString = sb.ToString();

        //                //Memory Optimization
        //                appFields.Clear();
        //                selectFields = null;

        //                appFields = null;
        //                sb = null;
        //                whereClause = null;

        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //}
#endregion
        private ActionResult LauchUserInputForm()
        {
            if (googleQueryController != null)
            {
                return Result;
            }
            int nCount = (from searchFilterGroup in model.WhereFilterGroups
                          from searchFilter in searchFilterGroup.Filters
                          where searchFilter.ValueType.Equals(ExpressionValueTypes.UserInput)
                          select searchFilter).Count();
            if (nCount > 0)
            {
                UserInputController controller = new UserInputController(model.Id, model.WhereFilterGroups, model.AllowSavingFilters);
                ActionResult result = controller.ShowInputForm();
                if (result.Status != ActionResultStatus.Success)
                    return result;
            }
            return Result;
        }

        public ActionResult Execute()
        {
            Result.Status = ActionResultStatus.Pending_Execution;

            //Step 1. Check visibility status of fields.
            StringBuilder invisibleFields;
            if (!AreFilterFieldsVisible(out invisibleFields))
            {
                //ApttusMessageUtil.ShowInfo("Filter Field(s) like :  " + invisibleFields.ToString() + " are invisible for the logged-in user, hence cannot execute the query.", "Field Level Security");
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("SEARCHNSELECTCTL_Execute_InfoMsg"), invisibleFields.ToString(), Globals.ThisAddIn.userInfo.UserFullName,resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }

            //Step 2. Launch UserInput Form if needed.
            ActionResult result = LauchUserInputForm();
            if (result.Status == ActionResultStatus.Failure)
                return result;

            //Step 3. Perpare Where Clause
            QueryAction.PrepareWhereClause();

            if (QueryAction.ValidExpression)
            {
                //Step 4. Execute the query.
                ObjectManager manager = ObjectManager.GetInstance;
                ApttusDataSet resultData = null;
                #region Old QueryAction Code
                //try
                //{

                //    bool bUsedSubQuery = false;
                //    if (model.WhereFilterGroups != null && queryString.Length > Constants.SOQL_MAX_LENGTH)
                //    {
                //        if (model.WhereFilterGroups.Count > 0)
                //        {
                //            // 1. If the filter text exists and doesn't contain any OR Clause, then use subquery.
                //            // 2. If the filter is empty, by default it is AND clause, hence use sub-query
                //            if (string.IsNullOrEmpty(model.WhereFilterGroups[0].FilterLogicText) || !model.WhereFilterGroups[0].FilterLogicText.ToUpper().Contains("OR"))
                //            {
                //                bUsedSubQuery = true;
                //                ExpressionBuilderHelper.UseSubQuery = true;
                //                PrepareWhereClause();
                //                ExpressionBuilderHelper.UseSubQuery = false;
                //            }
                //        }

                //        // 3. Now check if either the SubQuery or Regular Query (where SubQuery was not possible) have exceeded 20K limit
                //        if (queryString.Length > Constants.SOQL_MAX_LENGTH)
                //        {
                //            bUsedSubQuery = false;
                //            bool bChunkingExceddedSOQLLength = false;

                //            ExpressionBuilderHelper.UseChunking = true;
                //            PrepareWhereClause();
                //            ExpressionBuilderHelper.UseChunking = false;

                //            if (chunkValues && ValidExpression)
                //            {
                //                ChunkQueries queries = new ChunkQueries();
                //                queryStrings = queries.PrepareChunkedQueries(fullWhereClauseChunks, queryWithoutWhereClause, out bChunkingExceddedSOQLLength);

                //                if (bChunkingExceddedSOQLLength)
                //                {
                //                    // Inform the user that chunking is also exceeding Salesforce limit. Use Dataset Action.
                //                    ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking failed or could not be applied for QueryAction: " + model.Name);
                //                    ExceptionLogHelper.InfoLog("SOQL Over-Limit: Since chunking has failed or could not be applied for the query, please use Dataset Action.");
                //                    ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking failed or could be not be applied for the query" + queryStrings[0]);

                //                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("QueryActionController_Execute_SOQL_Limit_ErrorMsg"), resourceManager.GetResource("COMMON_SOQL_Limit_Msg"));
                //                    result.Status = ActionResultStatus.Failure;
                //                    return result;
                //                }
                //            }
                //            else
                //            {
                //                // Here the chunking was not applied by PrepareWhereClause(). It could be due to various reasons like:
                //                // - The individual filters didnot exceed the 
                //                // Inform the user that chunking is also exceeding Salesforce limit. Use Dataset Action.
                //                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking could not be applied for QueryAction: " + model.Name);
                //                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Since chunking could not be applied for the query, please use Dataset Action.");
                //                ExceptionLogHelper.InfoLog("SOQL Over-Limit: Chunking could be not be applied for the query:" + Environment.NewLine + queryString);

                //                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("QueryActionController_Execute_SOQL_Chunking_Limit_ErrorMsg"), resourceManager.GetResource("COMMON_SOQL_Limit_Msg"));
                //                result.Status = ActionResultStatus.Failure;
                //                return result;
                //            }
                //        }
                //    }

                //    if (!chunkValues)
                //    {
                //        queryString = queryString.Replace(Constants.ATTACHMENT, Constants.ATTACHMENT_SUBQUERY);
                //        resultData = manager.QueryDataSet( new SalesforceQuery { SOQL = queryString, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });
                //        //Salesforce only supports one level of sub-query, nested sub-queries are not supported. Hence if the current queryString is 
                //        //using sub-query to retrieve the data, this sub-query cannot be used-again, hence set SOQL property of the dataset to null, so that
                //        //this SOQL is not used in nested sub-queries again.
                //        if (bUsedSubQuery)
                //            resultData.SOQL = null;
                //    }
                //    else if (queryStrings != null && queryStrings.Count > 0) //Use Chunking
                //    {
                //        queryString = null; //Memory Optimization
                //        resultData = new ApttusDataSet();
                //        resultData.DataTable = new DataTable();
                //        resultData.SOQL = string.Empty;
                //        var comparer = new DataRowComparer();

                //        foreach (string query in queryStrings)
                //        {
                //            ApttusDataSet tempDataSet = manager.QueryDataSet(new SalesforceQuery { SOQL = query, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });
                //            IEnumerable<DataRow> rows = resultData.DataTable.AsEnumerable().Union(tempDataSet.DataTable.AsEnumerable(), comparer);
                //            if (rows.Count() > 0)
                //                resultData.DataTable = rows.CopyToDataTable();

                //            tempDataSet.SOQL = null;
                //            tempDataSet.DataTable.Dispose();
                //            tempDataSet.DataTable = null;
                //            tempDataSet = null;
                //            rows = null;
                //        }

                //        //Memory Optimization
                //        queryStrings.Clear();
                //        queryStrings.Capacity = 0;
                //        queryStrings = null;

                //        if (fullWhereClauseChunks != null)
                //        {
                //            fullWhereClauseChunks.Clear();
                //            fullWhereClauseChunks.Capacity = 0;
                //            fullWhereClauseChunks = null;
                //        }
                //    }

                //    // If the resultData is a child dataset, then set the Parent property to the parent dataset
                //    if (InputDataSetMap.Count == 1 && resultData != null)
                //        resultData.Parent = dataManager.GetParentDataSetId(model.TargetObject);
                //}
                //catch (Exception ex)
                //{
                //    ExceptionLogHelper.InfoLog("Query:" + Environment.NewLine + queryString);
                //    RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Query Action");
                //    Result.Status = ActionResultStatus.Failure;
                //    return Result;
                //}
                //finally
                //{
                //    ExpressionBuilderHelper.UseSubQuery = false;
                //    ExpressionBuilderHelper.UseChunking = false;
                //    queryString = null; //Memory Optimization
                //    queryWithoutWhereClause = null;
                //}
                #endregion

                Result = QueryAction.GetResultDataSet(out resultData);
                if (Result.Status == ActionResultStatus.Success)
                {
                    // If the resultData is a child dataset, then set the Parent property to the parent dataset
                    if (QueryAction.InputDataSetMap.Count == 1 && resultData != null)
                        resultData.Parent = dataManager.GetParentDataSetId(model.TargetObject);
                    if (OutputPersistData)
                    {
                        resultData.Name = OutputDataName;
                        resultData.AppObjectUniqueID = QueryAction.AppObject.UniqueId;
                        QueryAction.SetDisplayableWhereClause(ref resultData);
                        dataManager.AddData(resultData);
                        //ExceptionLogHelper.DebugLog("Query Action Completed Succesfully - " + model.Name);
                    }
                }
               // Result.Status = ActionResultStatus.Success;
            }
            else
            {
                Result.Status = ActionResultStatus.Success;
            }
            // ABB performance issue
            GC.Collect();
            return Result;
        }
    }

    public class GoogleActionQueryController : GoogleActionController
    {
        public string actionId;
        public string inputDataNames;
        //public string uniqueAppId; //unused?
        //public string configxml; //to be used instead of downloading config file from SFDC
        //public string strActionObj;
        public GoogleActionQueryController(string obj, string inputDataNames)
        {
            this.actionId = obj;
            this.inputDataNames = inputDataNames;
        }
        public string soqlQuery
        {
            get;
            set;
        }
        //creates sets the config manager, creates a query action controller and returns the generated soqlQuery
        public string Execute()
        {
            //get the action model and deserialize input names 
            JavaScriptSerializer js = new JavaScriptSerializer();
            ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
            Core.QueryActionModel model = (Core.QueryActionModel)configurationManager.GetActionById(this.actionId);
            string[] InputDataName = js.Deserialize<string[]>(inputDataNames);

            //suppress any messages
            AppSettings appSettings = new AppSettings();
            appSettings.SuppressDependent = true;
            configurationManager.Definition.AppSettings = appSettings;

            //temporarily change userinput filters to static
            List<Core.SearchFilter> filters;
            List<int> sequenceNoChangedList = new List<int>();
            if (model.WhereFilterGroups.Count() > 0)
            {
                filters = model.WhereFilterGroups[0].Filters;
                foreach (Core.SearchFilter currentFilter in filters)
                {
                    if (currentFilter.ValueType == ExpressionValueTypes.UserInput)
                    {
                        currentFilter.ValueType = ExpressionValueTypes.Static;
                        sequenceNoChangedList.Add(currentFilter.SequenceNo);
                    }
                }
            }
            //build a google action controller and execute
            QueryActionController queryAction = new QueryActionController(model, InputDataName);
            queryAction.googleQueryController = this;
            queryAction.Execute();
            int currentChangedFilter = 0;

            //change the filters back to user input
            if (model.WhereFilterGroups.Count() > 0 && sequenceNoChangedList.Count() > 0)
            {
                filters = model.WhereFilterGroups[0].Filters;
                foreach (Core.SearchFilter currentFilter in filters)
                {
                    if (currentChangedFilter >= sequenceNoChangedList.Count) //if changed back all filters, break
                    {
                        break;
                    }
                    if (currentFilter.ValueType == ExpressionValueTypes.Static && currentFilter.SequenceNo == sequenceNoChangedList[currentChangedFilter])
                    {
                        currentFilter.ValueType = ExpressionValueTypes.UserInput;
                        currentChangedFilter++;
                    }
                }
            }
            return soqlQuery;
        }
    }
}
