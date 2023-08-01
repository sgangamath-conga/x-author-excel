/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    [XmlInclude(typeof(WorkflowStructure))]
    public abstract class Workflow
    {
        [XmlIgnore]
        public ConfigurationManager configManager = ConfigurationManager.GetInstance;

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Write the workflow to configuration
        /// </summary>
        /// <returns></returns>
        protected Guid Write()
        {
            return configManager.AddWorkflow(this);
        }

        /// <summary>
        /// Execute Workflow
        /// TODO:: check if ActionResult can be used return type or need custom one
        /// </summary>
        //public abstract void Execute();

    }
}
