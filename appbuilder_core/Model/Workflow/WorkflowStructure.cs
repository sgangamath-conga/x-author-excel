/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class WorkflowStructure : Workflow, ICloneable<WorkflowStructure>
    {
        // List of Steps inside a Workflow
        public List<Step> Steps { get; set; }

        // Action flow triggers
        public enum Trigger { FirstAppLoad, AppLoad }
        public HashSet<Trigger> Triggers;

        public bool AutoExecuteRegularMode { get; set; }
        public bool AutoExecuteEditInExcelMode { get; set; }


        public WorkflowStructure()
        {
            Steps = new List<Step>();
        }

        /// <summary>
        /// 
        /// </summary>
        //public override void Execute()
        //{
        //    //throw new NotImplementedException();
        //}

        /// <summary>
        /// 
        /// </summary>
        public void SaveWorkflow()
        {
            //If there is no Id for this workflow which means the workflow needs to be added into the config.
            //If there is a valid Id availabe, the action is being edited and nothing needs to be specified into the config.
            //if (Id == Guid.Empty)
            Write();
        }

        public WorkflowStructure Clone()
        {
            WorkflowStructure result = (WorkflowStructure)MemberwiseClone();
            if (Steps != null)
                result.Steps = Steps.Select(x => x.Clone()).ToList();

            return result;
        }
    }

    /// <summary>
    /// Class defining Step of a workflow
    /// </summary>
    [Serializable]
    public class Step : ICloneable<Step>
    {
        // Default constructor for deserialization process.
        public Step()
        {
            Conditions = new List<Condition>();
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlElement("SequenceNo")]
        public int SequenceNo { get; set; }

        // List of Conditions inside Step
        public List<Condition> Conditions { get; private set; }

        public Step Clone()
        {
            Step result = (Step)MemberwiseClone();
            if (Conditions != null)
                result.Conditions = Conditions.Select(x => x.Clone()).ToList();

            return result;
        }
    }

    /// <summary>
    /// Class defining condition of a Step
    /// </summary>
    [Serializable]
    public class Condition : ICloneable<Condition>
    {
        // Default constructor for deserialization process.
        public Condition()
        {
            WorkflowActions = new List<WorkflowAction>();
        }

        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("SequenceNo")]
        public int SequenceNo { get; set; }

        [XmlElement("Criteria")]
        public string Criteria { get; set; }

        // List of Actions inside Condition or Step
        public List<WorkflowAction> WorkflowActions { get; private set; }

        public Condition Clone()
        {
            Condition result = (Condition)MemberwiseClone();
            if (WorkflowActions != null)
                result.WorkflowActions = WorkflowActions.Select(x => x.Clone()).ToList();

            return result;
        }
    }

    /// <summary>
    /// Class defining actions inside condition or step
    /// </summary>
    [Serializable]
    public class WorkflowAction : ICloneable<WorkflowAction>
    {
        // Default constructor for deserialization process.
        public WorkflowAction()
        { 
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("ActionId")]
        public string ActionId { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("SequenceNo")]
        public int SequenceNo { get; set; }

        //[XmlElement("OutputDataName")]
        //public string OutputDataName { get; set; }

        [XmlElement("WorkflowActionData")]
        public WorkflowActionData WorkflowActionData { get; set; }

        public WorkflowAction Clone()
        {
            WorkflowAction result = (WorkflowAction)MemberwiseClone();
            if (WorkflowActionData != null)
                result.WorkflowActionData = WorkflowActionData.Clone();

            return result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WorkflowActionData : ICloneable<WorkflowActionData>
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlElement("AppObjectId")]
        public Guid AppObjectId { get; set; }

        [XmlElement("OutputPersistData")]
        public bool OutputPersistData { get; set; }

        [XmlElement("OutputDataName")]
        public string OutputDataName { get; set; }

        [XmlElement("OutputIsGlobal")]
        public bool OutputIsGlobal { get; set; }

        [XmlElement("InputData")]
        public bool InputData { get; set; }

        [XmlElement("InputIsExternal")]
        public bool InputIsExternal { get; set; }

        [XmlElement("InputDataName")]
        public string[] InputDataName { get; set; }

        [XmlElement("SupressSaveMessage")]
        public bool SupressSaveMessage { get; set; }

        public WorkflowActionData Clone()
        {
            return new WorkflowActionData
            {
                Id = Id,
                AppObjectId = AppObjectId,
                OutputPersistData = OutputPersistData,
                OutputDataName = OutputDataName,
                OutputIsGlobal = OutputIsGlobal,
                InputData = InputData,
                InputIsExternal = InputIsExternal,
                InputDataName = InputDataName,
                SupressSaveMessage = SupressSaveMessage
            };
        }
    }

}
