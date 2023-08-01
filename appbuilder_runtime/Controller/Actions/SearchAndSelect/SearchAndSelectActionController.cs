/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Apttus.XAuthor.Core;
using System.Windows.Forms;
using Apttus.XAuthor.Core.Model.GoogleActions;

namespace Apttus.XAuthor.AppRuntime
{
    public class SearchAndSelectActionController : IOutputActionData, IInputActionData
    {
        private SearchAndSelectActionView view;

        public SearchAndSelect model;
        public string[] InputDataName { get; set; }
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }
        public bool InputData { get; set; }
        public ActionResult Result { get; private set; }
        public ApttusObject appObject;
        public Dictionary<ApttusField, string> persistedSearchValues;
        DataManager dataManager = DataManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ISearchAndSelectAction searchAndSelectsactionController = Globals.ThisAddIn.GetSSActionController();
        public GoogleSearchAndSelectActionController gssAction;
        //private const string PAGE_NAME = "searchselectaction";
        //private WebBrowserControl.WebBrowserControl browser;
        //private Dictionary<string, string> pageParams;
        //public enum SelectType { Single, Multi }

        public bool OutputPersistData { get; set; }
        public string OutputDataName { get; set; }
        // making whereClause Public var to be used at different places
        public string whereClause = string.Empty;

        bool ValidExpression = false;

        public SearchAndSelectActionController(SearchAndSelect model, string[] inputDataName)
        {
            this.model = model;
            appObject = applicationDefinitionManager.GetAppObject(this.model.TargetObject);
            persistedSearchValues = new Dictionary<ApttusField, string>();
            InputDataName = inputDataName;
            Result = new ActionResult();

            view = new SearchAndSelectActionView();
            view.SetController(this);

            List<KeyValuePair<string, Guid>> UsedDataSets;

            //string whereClause = string.Empty;
            ExcelHelper.SetCellReferenceFilterValue(model.SearchFilterGroups);

            //bool chunkValues;
            //List<string> fullWhereClauseChunks;
            //ValidExpression = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out UsedDataSets, out whereClause, out chunkValues, out fullWhereClauseChunks);
            ValidExpression = searchAndSelectsactionController.GetValidExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out UsedDataSets);
            if (ValidExpression)
            {
                InputDataSetMap = UsedDataSets;
            }

            #region "VF S&S - Old"
            //Result.Status = ActionResultStatus.Failure;
            //List<KeyValuePair<string, Guid>> UsedDataSets;

            //ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(model.TargetObject);
            //String TargetObject = obj.Id;

            //browser = new WebBrowserControl.WebBrowserControl(model.Name, Globals.ThisAddIn.oAuthWrapper.token.instance_url, Globals.ThisAddIn.oAuthWrapper.token.access_token);

            //string searchFields = "";
            //string resultFields = "";
            //string defaultValues = "";
            //string selectFields = "";
            //string sortFields = string.Empty;
            //string whereClause = string.Empty;

            //foreach (SearchField f in model.SearchFields) 
            //{
            //    searchFields += f.Id + ",";

            //    defaultValues += f.DefaultValue;
            //    defaultValues += ",";
            //}

            //foreach (ResultField f in model.ResultFields)
            //{
            //    resultFields += f.Id;
            //    resultFields += ",";

            //    if (f.IsSortField)
            //    {
            //        sortFields += f.Id;
            //        sortFields += ",";
            //    }
            //}

            //foreach (string fieldName in configurationManager.GetAllAppFields(obj, false)) 
            //{
            //    ApttusField f = obj.Fields.Where(s => s.Id.Equals(fieldName)).FirstOrDefault(); ;

            //    if (f != null)
            //    {
            //        selectFields += f.Id;
            //        selectFields += ",";
            //    }
            //}

            //searchFields = searchFields.Substring(0, searchFields.Length - 1);
            //resultFields = resultFields.Substring(0, resultFields.Length - 1);

            //// Handle null condition , if sortfield is not define in S&S action
            //if(!String.IsNullOrEmpty(sortFields) && sortFields.Length > 0)
            //    sortFields = sortFields.Substring(0, sortFields.Length - 1);

            //defaultValues = defaultValues.Substring(0, defaultValues.Length - 1);

            //List<string> whereValues;
            //ValidExpression = RuntimeExpressionBuilderHelper.GetPreparedExpression(model.SearchFilterGroups, obj, inputDataName, out UsedDataSets, out whereValues, out whereClause);
            //if (ValidExpression)
            //{
            //    InputDataSetMap = UsedDataSets;

            //    pageParams = new Dictionary<string, string>();
            //    if (model.PageSize == "All")
            //        pageParams.Add("pageSize", "0");
            //    else
            //        pageParams.Add("pageSize", model.PageSize);

            //    pageParams.Add("object", TargetObject);
            //    pageParams.Add("searchFields", searchFields);
            //    pageParams.Add("resultFields", resultFields);
            //    pageParams.Add("selectFields", selectFields);
            //    pageParams.Add("sortFields", sortFields);
            //    pageParams.Add("type", model.RecordType.ToLower());
            //    pageParams.Add("whereClause", HttpUtility.UrlEncode(whereClause));
            //    pageParams.Add("whereValues", HttpUtility.UrlEncode(String.Join(",", whereValues.ToArray())));

            //    pageParams.Add("defValues", HttpUtility.UrlEncode(defaultValues));
            //}
            #endregion

        }

        //public SearchAndSelectActionController(string objAction, string appObject, string inputDataNames)
        //{
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    SearchAndSelect ssAction = js.Deserialize<SearchAndSelect>(objAction);
        //    this.model = ssAction;
        //    this.appObject = js.Deserialize<ApttusObject>(appObject);
        //    this.InputDataName = js.Deserialize<string[]>(inputDataNames);
        //    Result = new ActionResult();
        //    List<KeyValuePair<string, Guid>> tempUsedDataSets;
        //    //ValidExpression = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, this.appObject, InputDataName, out UsedDataSets, out whereClause);
        //    bool chunkValues;
        //    List<string> fullWhereClauseChunks;
        //    String uiWhereClause = "";
        //    bool ValidExpression = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, this.appObject, InputDataName, out tempUsedDataSets, out uiWhereClause, out chunkValues, out fullWhereClauseChunks, true);
        //    if (ValidExpression)
        //    {
        //        InputDataSetMap = tempUsedDataSets;
        //    }
        //}

        private List<ResultField> GetResultFields()
        {
            List<ResultField> fields = new List<ResultField>();
            fields.AddRange((from field in model.ResultFields
                             where fieldLevelSecurity.IsFieldVisible(appObject.UniqueId, field.Id)
                             select field).ToList());
            return fields;
        }

        //private List<SearchField> GetSearchFields()
        //{
        //    List<SearchField> fields = new List<SearchField>();
        //    fields.AddRange((from field in model.SearchFields
        //                     where fieldLevelSecurity.IsFieldVisible(appObject.UniqueId, field.Id)
        //                     select field).ToList());
        //    return fields;
        //}

        //generates soqlQuery, and combinedWhereClause
        //public Dictionary<string, object> GetWhereClause(string globalSearchString, string advancedFilterWhereClause, bool bClear)
        //{
        //    // Prepare Select Fields List and Select Clause
        //    List<string> selectFields = new List<string> { Constants.ID_ATTRIBUTE };
        //    List<ResultField> resultFields = this.GetResultFields();
        //    selectFields.AddRange(resultFields.Select(rf => rf.Id));
        //    string selectClause = string.Join(Constants.COMMA, selectFields);

        //    // Prepare the "Order By" Clause
        //    string orderByClause = string.Empty;
        //    if (resultFields.Any(r => r.IsSortField))
        //        orderByClause = " ORDER BY " + resultFields.FirstOrDefault(r => r.IsSortField).Id;

        //    // Prepare user Where Clause
        //    // Removing combination of Advanced search as search will happens as EITHER OR, i.e. Either Global search OR Advanced Search
        //    string userGlobalWhereClause = GetGlobalSearchWhereClause(globalSearchString);
        //    //string userWhereClause = userGlobalWhereClause.Length > 0 ? userGlobalWhereClause : string.Empty;
        //    string userWhereClause = userGlobalWhereClause.Length > 0 && advancedFilterWhereClause.Length > 0
        //        ? userGlobalWhereClause + " AND " + advancedFilterWhereClause
        //        : userGlobalWhereClause + advancedFilterWhereClause; // Either one or both have length of 0, so AND is not required

        //    // Get Where clause for Advanced search from User Input fields combined with other Filters
        //    string uiWhereClause = whereClause;
        //    if (!bClear)
        //    {
        //        List<KeyValuePair<string, Guid>> tempUsedDataSets;
        //        bool chunkValues;
        //        List<string> fullWhereClauseChunks;
        //        bool bValid = ExpressionBuilderHelper.GetExpression(model.Id, model.SearchFilterGroups, appObject, InputDataName, out tempUsedDataSets, out uiWhereClause, out chunkValues, out fullWhereClauseChunks, true);

        //        // Incase of advance filter logic no need to replace where clause with "And"
        //        SearchFilterGroup searchFilterGroup = model.SearchFilterGroups.FirstOrDefault();
        //        if (searchFilterGroup != null)
        //        {
        //            if (!searchFilterGroup.IsAddFilter)
        //            {
        //                if (bValid && !(string.IsNullOrEmpty(uiWhereClause)))
        //                    uiWhereClause = uiWhereClause.Replace(" OR ", " AND "); // replace any OR with AND
        //                else
        //                    uiWhereClause = whereClause;
        //            }
        //        }
        //    }

        //    string combinedWhereClause = uiWhereClause.Length > 0 && userWhereClause.Length > 0
        //        ? uiWhereClause + " AND " + userWhereClause
        //        : uiWhereClause + userWhereClause; // Either one or both have length of 0, so AND is not required

        //    string soqlQuery = "SELECT " + selectClause + " FROM " + appObject.Id +
        //        (combinedWhereClause.Length > 0
        //        ? " WHERE " + combinedWhereClause
        //        : string.Empty) +
        //        orderByClause +
        //        " LIMIT " + Convert.ToString(Constants.QUERYRESULT_RECORDLIMIT);
        //    Dictionary<string, object> whereClauses = new Dictionary<string, object>();
        //    whereClauses["soqlQuery"] = soqlQuery;
        //    whereClauses["combinedWhereClause"] = combinedWhereClause;
        //    whereClauses["selectFields"] = selectFields;
        //    return whereClauses;
        //}

        private bool AreFilterFieldsVisible(out StringBuilder sb)
        {
            sb = new StringBuilder();
            bool bVisible = true;
            if (model.SearchFilterGroups == null)
                return bVisible;

            SearchFilterGroup searchFilterGroup = model.SearchFilterGroups.FirstOrDefault();
            if (searchFilterGroup != null && searchFilterGroup.Filters != null && searchFilterGroup.Filters.Count > 0)
            {
                foreach (SearchFilter filter in searchFilterGroup.Filters)
                {
                    bool bTempVisible = fieldLevelSecurity.IsFieldVisible(filter.AppObjectUniqueId, filter.FieldId);
                    bVisible = bVisible && bTempVisible;
                    if (!bTempVisible)
                        sb.Append(filter.FieldId).Append(", ");
                }
            }
            return bVisible;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalSearchString"></param>
        /// <param name="advancedFilterWhereClause"></param>
        /// <returns></returns>
        public SearchAndSelectData GetResultData(string globalSearchString, string advancedFilterWhereClause, bool bClear)
        {
            List<string> selectFields = new List<string>();
            List<ResultField> resultFields = this.GetResultFields();
            selectFields.Add(appObject.IdAttribute); 
            selectFields.AddRange(resultFields.Select(rf => rf.Id));
            return searchAndSelectsactionController.GetResultData(model, appObject, selectFields, resultFields, globalSearchString, advancedFilterWhereClause, bClear, this.InputDataName);

            //Dictionary<string, object> whereClauses = GetWhereClause(globalSearchString, advancedFilterWhereClause, bClear);
            //string combinedWhereClause = whereClauses["combinedWhereClause"].ToString();
            //string soqlQuery = whereClauses["soqlQuery"].ToString();
            //if(this.gssAction != null)
            //{
            //    gssAction.soqlQuery = soqlQuery;
            //    return null;
            //}
            //List<string> selectFields = (List<string>)whereClauses["selectFields"];

            //DataTable dataTable = configurationManager.GetDataTableFromAppFields(appObject, selectFields);
            //ApttusDataSet resultData = objectManager.QueryDataSet(new SalesforceQuery{ SOQL =  soqlQuery, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });

            //return new SearchAndSelectData
            //{
            //    ResultDataTable = resultData.DataTable,
            //    whereClause = combinedWhereClause
            //};
        }

        /// <summary>
        /// This method will where clause for generic search and not for Advanced search
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        //private string GetGlobalSearchWhereClause(string searchCriteria)
        //{
        //    string userWhereClause = string.Empty;
        //    if (!string.IsNullOrEmpty(searchCriteria))
        //    {
        //        searchCriteria = objectManager.EscapeQueryString(searchCriteria);

        //        List<string> whereClauses = new List<string>();
        //        List<string> fieldsForWhereClause = new List<string>();

        //        // This section is for searches by the SearchFields of S&S
        //        foreach (SearchField sf in GetSearchFields())
        //        {
        //            ApttusField apttusField = appObject.Fields.FirstOrDefault(f => f.Id == sf.Id);
        //            // Ignore Lookup Id field from Global Search
        //            if (apttusField.Datatype != Datatype.Lookup)
        //            {
        //                string fieldWhereClause = ExpressionBuilderHelper.GetLikeWhereClauseBasedOnDataType(searchCriteria, apttusField);
        //                // Add Where clause only if it was constructed
        //                if (!string.IsNullOrEmpty(fieldWhereClause))
        //                {
        //                    whereClauses.Add(fieldWhereClause);
        //                    fieldsForWhereClause.Add(apttusField.Id);
        //                }
        //            }
        //        }

        //        // This section is for searches by User Input fields
        //        if (this.model.SearchFilterGroups.Count > 0 && this.model.SearchFilterGroups[0].Filters.Count > 0)
        //        {
        //            // There is only one SearchFilterGroup which contains list of SearchFilters hence use 0 index
        //            var userInputSearchFilters = this.model.SearchFilterGroups[0].Filters.Where(f => f.ValueType == ExpressionValueTypes.UserInput);
        //            if (userInputSearchFilters.Any())
        //                foreach (SearchFilter filter in userInputSearchFilters)
        //                {
        //                    ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(filter.AppObjectUniqueId);
        //                    ApttusField apttusField = apttusObject.Fields.Where(f => f.Id.Equals(filter.FieldId)).FirstOrDefault();
        //                    // Ignore Lookup Id field from Global Search
        //                    if (apttusField.Datatype != Datatype.Lookup)
        //                    {
        //                        string LHSObjectField = ExpressionBuilderHelper.GetLHSQueryTextFromQueryObjects(filter);
        //                        // Don't add the same field if its added as a part of Search Field
        //                        if (!fieldsForWhereClause.Contains(LHSObjectField))
        //                        {
        //                            string fieldWhereClause = ExpressionBuilderHelper.GetLikeWhereClauseBasedOnDataType(searchCriteria, LHSObjectField, apttusField.Datatype);
        //                            // Add Where clause only if it was constructed
        //                            if (!string.IsNullOrEmpty(fieldWhereClause))
        //                            {
        //                                whereClauses.Add(fieldWhereClause);
        //                                fieldsForWhereClause.Add(LHSObjectField);
        //                            }
        //                        }
        //                    }
        //                }
        //        }
        //        if (whereClauses.Count > 0)
        //        {
        //            userWhereClause = Constants.OPEN_BRACKET;
        //            userWhereClause += string.Join<string>(" OR ", whereClauses);
        //            userWhereClause += Constants.CLOSE_BRACKET;
        //        }
        //    }
        //    return userWhereClause;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetPersistedSearchValue(ApttusField field)
        {
            string returnVal = string.Empty;

            if (persistedSearchValues.Count > 0)
                returnVal = persistedSearchValues.Where(p => p.Key == field).FirstOrDefault().Value;

            return returnVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="searchCriteria"></param>
        public void AddPersistedSearchValue(ApttusField field, string searchCriteria)
        {
            if (persistedSearchValues.ContainsKey(field))
                persistedSearchValues.Remove(field);

            if (!string.IsNullOrEmpty(searchCriteria))
                persistedSearchValues.Add(field, searchCriteria);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetUserInputCount()
        {
            var userInputFilters = from searchFilterGroup in this.model.SearchFilterGroups
                                   from searchFilter in searchFilterGroup.Filters
                                   where searchFilter.ValueType.Equals(ExpressionValueTypes.UserInput)
                                   select searchFilter;

            return userInputFilters.Count();
        }

        /// <summary>
        /// Execute method
        /// </summary>
        /// <returns></returns>
        public ActionResult Execute()
        {
            Result.Status = ActionResultStatus.Pending_Execution;
            try
            {
                if (ValidExpression)
                {
                    StringBuilder invisibleFields;
                    if (!AreFilterFieldsVisible(out invisibleFields))
                    {
                        //ApttusMessageUtil.ShowInfo("Filter Field(s) like :  " + invisibleFields.ToString() + " are invisible for the logged-in user, hence cannot execute the query.", "Field Level Security");
                        ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("SEARCHNSELECTCTL_Execute_InfoMsg"), invisibleFields.ToString(), Globals.ThisAddIn.userInfo.UserFullName,resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_FieldSecurity_InfoMsg"));
                        Result.Status = ActionResultStatus.Failure;
                        return Result;
                    }

                    view.PrepareSearchUI();
                    //if (this.gssAction != null)
                    //{
                    //    return null;
                    //}
                    DialogResult dr = view.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        ObjectManager manager = ObjectManager.GetInstance;

                        ApttusDataSet dataset = searchAndSelectsactionController.GetData(view, Result);

                        // Create query string
                        //string queryString = string.Empty;
                        //if ((!view.searchAndSelectData.SelectAllInSalesforce) && !string.IsNullOrEmpty(view.searchAndSelectData.selectedId))
                        //    queryString = "SELECT " + selectFields + " FROM " + appObject.Id + " WHERE Id in (" + view.searchAndSelectData.selectedId + ")";
                        //else if (view.searchAndSelectData.SelectAllInSalesforce)
                        //    queryString = "SELECT " + selectFields + " FROM " + appObject.Id
                        //        + (string.IsNullOrEmpty(view.searchAndSelectData.whereClause) ? string.Empty : " WHERE " + view.searchAndSelectData.whereClause);

                        //queryString = queryString.Replace(Constants.ATTACHMENT, Constants.ATTACHMENT_SUBQUERY);

                        //ApttusDataSet dataset = manager.QueryDataSet(new SalesforceQuery { SOQL = queryString, Object = appObject, DataTable = dataTable, UserInfo = Globals.ThisAddIn.userInfo });



                        //If the dataset is a child dataset, then set the Parent property to the parent dataset
                        if (InputDataSetMap.Count == 1 && dataset != null)
                            dataset.Parent = dataManager.GetParentDataSetId(model.TargetObject);

                        if (OutputPersistData)
                        {
                            dataset.Name = OutputDataName;
                            dataset.AppObjectUniqueID = appObject.UniqueId;
                            dataManager.AddData(dataset);
                        }

                        Result.Status = ActionResultStatus.Success;

                    }
                    else
                    {
                        Result.Status = ActionResultStatus.Failure;
                    }

                    #region "VF S&S - Old"
                    //string response = browser.ProcessRequest(Constants.NAMESPACE_PREFIX + PAGE_NAME, pageParams);
                    //browser.Dispose();

                    //try
                    //{
                    //    SearchAndSelectActionResult res = new JavaScriptSerializer().Deserialize<SearchAndSelectActionResult>(response);

                    //    if (res.Status == WebBrowserResult.ResponseStatus.Error || res.Status == WebBrowserResult.ResponseStatus.Cancelled)
                    //    {
                    //        Result.Status = ActionResultStatus.Failure;
                    //        return Result;
                    //    }

                    //    ApttusDataSet dataset = new ApttusDataSet(OutputDataName);
                    //    dataset.AppObjectUniqueID = model.TargetObject;
                    //    dataset.DataTable = res.ParseSObjects();

                    //    // Add empty columns manually (JSON does not return empty values)
                    //    ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(model.TargetObject);
                    //    List<string> fields = configurationManager.GetAllAppFields(obj, false);

                    //    foreach (string f in fields)
                    //    {
                    //        if (!dataset.DataTable.Columns.Contains(f))
                    //            dataset.DataTable.Columns.Add(new DataColumn(f));
                    //    }

                    //    // If the dataset is a child dataset, then set the Parent property to the parent dataset
                    //    if (InputDataSetMap.Count == 1 && dataset != null)
                    //        dataset.Parent = dataManager.GetParentDataSetId(model.TargetObject);

                    //    dataManager.AddData(dataset);

                    //    Result.Status = ActionResultStatus.Success;
                    //}
                    //catch (Exception ex)
                    //{
                    //    ExceptionLogHelper.InfoLog("JSON Response String :" + response);
                    //    ExceptionLogHelper.ErrorLog(ex);
                    //    //TODO: error handling
                    //    Result.Status = ActionResultStatus.Failure;
                    //}
                    #endregion
                }
                else
                {
                    Result.Status = ActionResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Search & Select Action");
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }

            return Result;
        }
    }
    /*
     * Google search and select controller. retrieves data required to perform a search and select action
     */
    public class GoogleSearchAndSelectActionController : GoogleActionController
    {
        private string actionId;
        private string inputDataNames;
        public SearchAndSelectData ssData;
        public string parentObjectName;
        public string soqlQuery;
        public string parentSOQLQuery;
        public string recordType;
        public string appObjId;
        public string parentLookupId;
        public Dictionary<string, string> displayFields = new Dictionary<string, string>();//maps id to display name     
        public Dictionary<string, string> searchFields = new Dictionary<string, string>();//maps id to search label
        public Dictionary<string, string> fieldTypes = new Dictionary<string, string>();//maps id to data type
        public GoogleSearchAndSelectActionController(string obj, string inputDataNames)
        {
            this.actionId = obj;
            this.inputDataNames = inputDataNames;
        }
        public string Execute()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
            Core.SearchAndSelect model = (Core.SearchAndSelect)configurationManager.GetActionById(this.actionId);
            string[] InputDataName = js.Deserialize<string[]>(inputDataNames);
            appObjId = model.TargetObject.ToString();

            SearchAndSelectActionController ssAction = new SearchAndSelectActionController(model, InputDataName);
            ssAction.gssAction = this;
            ssAction.GetResultData("", "", true);
            for (int i = 0; i < ssAction.model.ResultFields.Count(); i++) //get the display fields
            {
                displayFields[ssAction.model.ResultFields[i].Id] = ssAction.model.ResultFields[i].HeaderName;
            }
            //get the search fields and pair with its label
            for (int i = 0; i < ssAction.model.SearchFields.Count(); i++)
            {
                searchFields[ssAction.model.SearchFields[i].Id] = ssAction.model.SearchFields[i].Label;
                fieldTypes[ssAction.model.SearchFields[i].Id] = ssAction.model.SearchFields[i].Datatype.ToString();
            }
            List<string> parentLookupIdList = (from filter in ssAction.model.SearchFilterGroups[0].Filters
                                               where filter.ValueType == ExpressionValueTypes.Input
                                               select filter.FieldId).ToList();
            if (parentLookupIdList.Count > 0)
            {
                parentLookupId = parentLookupIdList[0];
            }
            // Create parent object query string   
            ApttusObject appObject = ssAction.appObject;
            List<string> appFields = new List<string>();
            DataTable dataTable = configurationManager.GetDataTableFromAllAppFields(appObject, false, true, out appFields);
            string selectFields = string.Join(Constants.COMMA, appFields);

            parentSOQLQuery = "SELECT " + selectFields + " FROM " + appObject.Id + " WHERE Id=NULL";

            //save the parent object name
            parentObjectName = appObject.Name;

            recordType = ssAction.model.RecordType;

            return null;
        }
    }
}
