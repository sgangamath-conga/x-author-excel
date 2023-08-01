using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.DataAccess.Common.Model;

namespace Apttus.XAuthor.AppRuntime
{
    class AICQueryAction : IQueryAction
    {

        public AICQueryAction(QueryActionModel Model, string[] InputDataName)
        {
            this.Model = Model;
            this.InputDataName = InputDataName;
            AppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(this.Model.TargetObject);
            aicQuery = new AICQuery() { Object = AppObject, UserInfo = ApttusCommandBarManager.GetInstance().GetUserInfo() };
            ValidExpression = false;
        }

        public QueryActionModel Model { get ; set ; }
        public ApttusObject AppObject { get ; set ; }
        public string[] InputDataName { get ; set ; }
        public List<KeyValuePair<string, Guid>> InputDataSetMap { get ; set ; }
        public bool ValidExpression { get ; set ; }
        public DataTable AppFieldsDataTable { get ; set ; }

        public AICQuery aicQuery { get; set; }

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ActionResult GetResultDataSet(out ApttusDataSet resultData)
        {
            ActionResult result = new ActionResult();
            ObjectManager manager = ObjectManager.GetInstance;
            resultData = null;
            try
            {
                if (Model.MaxRecords != QueryActionModel.MAX_RECORDS_NOLIMIT)
                    aicQuery.Query.TopRecords = Model.MaxRecords;

                resultData = manager.QueryDataSet(aicQuery);
                result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.InfoLog("Query:" + Environment.NewLine + aicQuery.Query.ToString());
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Query Action");
                result.Status = ActionResultStatus.Failure;
                return result;
            }
            return result;
        }

        public void PrepareWhereClause()
        {
            // Replace the Excel Cell Reference values in the Search Filter before building the expression
            ExcelHelper.SetCellReferenceFilterValue(Model.WhereFilterGroups);
            List<KeyValuePair<string, Guid>> UsedDataSets;
            Query queryExpr;
            ValidExpression = AICExpressionBuilderHelper.GetExpression(Model.Id, Model.WhereFilterGroups, AppObject, InputDataName, out UsedDataSets, out queryExpr, true);
            if (ValidExpression)
            {
                List<string> appFields;
                AppFieldsDataTable = ConfigurationManager.GetInstance.GetDataTableFromAllAppFields(AppObject, false, true, out appFields);
                aicQuery.DataTable = AppFieldsDataTable;
                InputDataSetMap = UsedDataSets;
                aicQuery.Query = queryExpr;
            }
        }

        public void SetDisplayableWhereClause(ref ApttusDataSet DataSet)
        {
            string filterclausetodisplay, whereclause;
            whereclause = AICExpressionBuilderHelper.DisplayableWhereClause;

            if (string.IsNullOrEmpty(whereclause))
                filterclausetodisplay = resourceManager.GetResource("QueryActionController_EmptyFiltersText");
            else
                filterclausetodisplay = whereclause;

            DataSet.DisplayableWhereClause = new StringBuilder(AppObject.Name).Append(Constants.SPACE).Append(resourceManager.GetResource("QueryActionController_FiltersText")).Append(filterclausetodisplay).ToString();

        }
    }
}
