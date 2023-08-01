using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using System.Web.Script.Serialization;

namespace Apttus.XAuthor.AppDesigner
{

    public class AppSchema
    {
        public List<ActionFlow> ActionFlows;
        public AppSchema()
        {
            ActionFlows = new List<ActionFlow>();
        }
    }

    public class ActionFlow
    {
        public String Name;
        public List<DataSet> DataSets;
        public ActionFlow()
        {
            DataSets = new List<DataSet>();
        }
    }

    public class DataSet
    {
        public String Name;
        public String ObjectName;
        public List<String> Fields;
        public List<String> SaveFields;
        public String WhereClause;
        public List<WhereValue> WhereValues;

        public DataSet()
        {
            WhereClause = string.Empty;
            WhereValues = new List<WhereValue>();           
        }
    }

    public class WhereValue
    {
        public String Type;
        public String Value;
        public string Operator;
    }

    internal class PrestoSchema
    {
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        internal byte[] GeneratePrestoSchema()
        {
            AppSchema prestoSchema = new AppSchema();

            WorkflowStructure prestoWorkflow = (from wf in configurationManager.Workflows.OfType<WorkflowStructure>()
                                                where wf.AutoExecuteRegularMode
                                                select wf).FirstOrDefault();

            if (prestoWorkflow == null)
                return null;

            string prestoJson = string.Empty;

            WorkflowAction workflowAction = (from step in prestoWorkflow.Steps
                                             from condition in step.Conditions
                                             from wfAction in condition.WorkflowActions
                                             where configurationManager.GetActionById(wfAction.ActionId).Type.Equals(Constants.SEARCH_AND_SELECT_ACTION)
                                             select wfAction).FirstOrDefault();

            if (workflowAction != null)
            {
                SearchAndSelect searchAndSelect = configurationManager.GetActionById(workflowAction.ActionId) as SearchAndSelect;
                if (searchAndSelect != null)
                {
                    ActionFlow actionflow = new ActionFlow { Name = prestoWorkflow.Name };
                    ApttusObject obj = applicationDefinitionManager.GetAppObject(searchAndSelect.TargetObject);

                    WorkflowActionData actionData = workflowAction.WorkflowActionData;

                    DataSet dataSet = new DataSet();
                    dataSet.Name = actionData.OutputDataName;
                    dataSet.Fields = (from apttusField in obj.Fields select apttusField.Id).ToList();
                    dataSet.ObjectName = obj.Id;
                    dataSet.WhereClause = "(Id=?)";

                    WhereValue WhValue = new WhereValue();
                    WhValue.Type = "external";
                    WhValue.Value = "SelectedId";
                    WhValue.Operator = "=";

                    dataSet.WhereValues.Add(WhValue);

                    actionflow.DataSets.Add(dataSet);

                    prestoSchema.ActionFlows.Add(actionflow);
                    prestoJson = new JavaScriptSerializer().Serialize(prestoSchema);
                }
            }

            if (string.IsNullOrEmpty(prestoJson))
            {
                prestoJson = "{\"ActionFlows\":[{\"Name\":\"get\",\"DataSets\":[{\"Name\":\"F9892193_9350_4DC2_A5DA_61B77F5A7520\",\"ObjectName\":\"Account\",\"Fields\":[\"Id\",\"Name\"],\"SaveFields\":null,\"WhereClause\":\"(Name = ?)\",\"WhereValues\":[{\"Type\":\"static\",\"Value\":\"\u0027ABC\u0027\",\"Operator\":\"=\"}]}]}]}";
            }
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(prestoJson);
            return bytes;
        }
    }
}
