

using System;
using System.Collections.Generic;
using System.Data;
using Apttus.XAuthor.Core;
using Microsoft.Xrm.Sdk.Query;
using Apttus.XAuthor.AppRuntime.Modules;
using System.Text;

namespace Apttus.XAuthor.AppRuntime
{
    class DynamicsQueryAction : IQueryAction
    {
        public DynamicsQueryAction(QueryActionModel Model, string[] InputDataName)
        {
            this.Model = Model;
            this.InputDataName = InputDataName;
            AppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(this.Model.TargetObject);
            dynamicsQuery = new DynamicsQuery() { Object = AppObject, UserInfo = ApttusCommandBarManager.GetInstance().GetUserInfo() };
            ValidExpression = false;
        }
        public QueryActionModel Model { get; set; }
        public ApttusObject AppObject { get; set; }
        public string[] InputDataName { get; set; }
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }
        public bool ValidExpression { get; set; }
        public DataTable AppFieldsDataTable { get; set; }
        DynamicsQuery dynamicsQuery;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public void PrepareWhereClause()
        {
            string whereClause = string.Empty;
            // Replace the Excel Cell Reference values in the Search Filter before building the expression
            ExcelHelper.SetCellReferenceFilterValue(Model.WhereFilterGroups);
            List<KeyValuePair<string, Guid>> UsedDataSets;
            QueryExpression queryExpr;
            ValidExpression = ExpressionBuilderHelperDynamics.GetExpression(Model.Id, Model.WhereFilterGroups, AppObject, InputDataName, out UsedDataSets, out queryExpr, true);
            if (ValidExpression)
            {
                List<string> appFields;
                AppFieldsDataTable = ConfigurationManager.GetInstance.GetDataTableFromAllAppFields(AppObject, false, true, out appFields);
                dynamicsQuery.DataTable = AppFieldsDataTable;
                InputDataSetMap = UsedDataSets;
                dynamicsQuery.Query = queryExpr;
            }
        }
        public ActionResult GetResultDataSet(out ApttusDataSet resultData)
        {
            ActionResult result = new ActionResult();
            ObjectManager manager = ObjectManager.GetInstance;
            resultData = null;
            try
            {
                if (Model.MaxRecords != QueryActionModel.MAX_RECORDS_NOLIMIT)
                    dynamicsQuery.Query.TopCount = Model.MaxRecords;

                resultData = manager.QueryDataSet(dynamicsQuery);
                result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.InfoLog("Query:" + Environment.NewLine + dynamicsQuery.Query.ToString());
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Query Action");
                result.Status = ActionResultStatus.Failure;
                return result;
            }
            return result;
        }
        public void SetDisplayableWhereClause(ref ApttusDataSet DataSet)
        {

            string filterclausetodisplay, whereclause;
            whereclause = ExpressionBuilderHelperDynamics.DisplayableWhereClause;

            if (string.IsNullOrEmpty(whereclause))
                filterclausetodisplay = resourceManager.GetResource("QueryActionController_EmptyFiltersText");
            else
                filterclausetodisplay = whereclause;

            DataSet.DisplayableWhereClause = new StringBuilder(AppObject.Name).Append(Constants.SPACE).Append(resourceManager.GetResource("QueryActionController_FiltersText")).Append(filterclausetodisplay).ToString();
        }
    }
}
