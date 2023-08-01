/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppRuntime
{


    public class CallProcedureController
    {
        public ActionResult Result { get; private set; }
        public bool InputData { get; set; }
        public string[] InputDataName { get; set; }
        public bool OutputPersistData { get; set; }
        public string OutputDataName { get; set; }
        private CallProcedureModel model;       
        DataManager dataManager = DataManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        SearchAndSelect searchAndSelect;
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        // Constructor
        public CallProcedureController(CallProcedureModel model, string[] inputDataName)
        {
            this.model = model;
            this.InputDataName = inputDataName;
            Result = new ActionResult();
            searchAndSelect = new SearchAndSelect();
            
        }
        private void UpdateSearchFilterForBackwardCompatibility(SearchFilter searchFilter)
        {
            if (searchFilter != null && searchFilter.QueryObjects != null && searchFilter.QueryObjects.Count == 0
                && !searchFilter.AppObjectUniqueId.Equals(Guid.Empty) && !string.IsNullOrEmpty(searchFilter.FieldId))
            {
                ApttusObject appObject = applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId);

                searchFilter.QueryObjects.Add(new QueryObject
                {
                    SequenceNo = searchFilter.QueryObjects.Count + 1,
                    AppObjectUniqueId = searchFilter.AppObjectUniqueId,
                    LookupFieldId = string.Empty,
                    RelationshipType = QueryRelationshipType.None,
                    BreadCrumbLabel = appObject.Name,
                    BreadCrumbTooltip = appObject.Name,
                    DistanceFromChild = 0,
                    LeafAppObjectUniqueId = searchFilter.AppObjectUniqueId
                });
            }
        }
        private ActionResult LauchUserInputForm()
        {
            int nCount = (from methodParams in model.MethodParams
                          where methodParams.Type.Equals(MethodParam.ParamType.UserInput)
                          select methodParams).Count();
            if (nCount > 0)
            {
                string strKey = string.Empty;
                SearchFilterGroup searchFilterGroup = new SearchFilterGroup();
                searchFilterGroup.Filters = new List<SearchFilter>();
                searchAndSelect.SearchFilterGroups = new List<SearchFilterGroup>();
                searchAndSelect.SearchFilterGroups.Add(searchFilterGroup);
                foreach (MethodParam param in model.MethodParams)
                {
                    if (param.Type.Equals(MethodParam.ParamType.UserInput))
                    {
                        SearchFilter searchFilter = new SearchFilter();
                        searchFilter.FieldId = param.ParamField;
                        searchFilter.AppObjectUniqueId = Guid.Parse(param.ParamObject);
                        searchFilter.ValueType = ExpressionValueTypes.UserInput;
                        searchFilter.Value = string.Empty;
                        searchFilter.Operator = Constants.EQUALS;
                        searchFilter.SequenceNo = 0;
                        searchFilter.SearchFilterLabel = param.ParamName;
                        searchFilter.QueryObjects = new List<QueryObject>();
                        UpdateSearchFilterForBackwardCompatibility(searchFilter);
                        searchAndSelect.SearchFilterGroups.FirstOrDefault().Filters.Add(searchFilter);
                    }
                    //strKey = new StringBuilder(model.Id).Append(Constants.DOT).Append(searchFilter.SequenceNo).Append(Constants.DOT).Append(searchFilter.FieldId).ToString();
                }
                UserInputController controller = new UserInputController(model.Id, searchAndSelect.SearchFilterGroups);
                ActionResult result = controller.ShowInputForm();

                foreach (MethodParam param in model.MethodParams)
                {
                    if (param.Type.Equals(MethodParam.ParamType.UserInput))
                    {
                        DataManager dataManager = DataManager.GetInstance;
                        List<ApttusDataSet> dataSets = dataManager.GetDatasetsByNames(new string[] { Constants.USERINPUT_DATASETNAME });
                        if (dataSets.Count > 0)
                        {
                            ApttusDataSet dataSet = dataSets.ElementAt(0);
                            DataRow dr = dataSet.DataTable.Rows.Find(new StringBuilder(model.Id).Append(Constants.DOT).Append(0).Append(Constants.DOT).Append(param.ParamField).ToString());
                            param.ParamValue = string.IsNullOrEmpty(Convert.ToString(dr[Constants.VALUE_COLUMN])) ? string.Empty : objectManager.EscapeQueryString(Convert.ToString(dr[Constants.VALUE_COLUMN]));
                        }
                    }
                }
                if (result.Status != ActionResultStatus.Success)
                    return result;
            }
            return Result;
        }
        // Execute from workflow for call procedure action
        public ActionResult Execute()
        {
            try
            {
                Result.Status = ActionResultStatus.Pending_Execution;

                ActionResult result = LauchUserInputForm();              

                if (result.Status == ActionResultStatus.Failure)
                    return result;

                CallProcedureInvoker invoker = new CallProcedureInvoker(this.model);

                invoker.InvokeWSDL(this.model, this.InputDataName, OutputPersistData, OutputDataName);

                if (invoker.Result.Status == ActionResultStatus.Success)
                Result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CALLPROC_Action_ErrorMsg"), ex.Message), resourceManager.GetResource("CALLPROC_ActionCap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
            }

            return Result;
        }
    }
}
