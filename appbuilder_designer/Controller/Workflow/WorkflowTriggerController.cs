/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class WorkflowTriggerController
    {
        private WorkflowStructure model;
        public HashSet<WorkflowStructure.Trigger> Triggers { get { return model.Triggers; } }

        public WorkflowTriggerController(WorkflowStructure model, WorkflowTriggerView view)
        {
            this.model = model;
            view.SetController(this);
        }

        public void EnableTrigger(WorkflowStructure.Trigger trigger) 
        {
            if (model.Triggers != null)
                model.Triggers.Add(trigger);
        }

        public void DisableTrigger(WorkflowStructure.Trigger trigger)
        {
            if  (model.Triggers != null)
                model.Triggers.Remove(trigger);
        }
    }
}
