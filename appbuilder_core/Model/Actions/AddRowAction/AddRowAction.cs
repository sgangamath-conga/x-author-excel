using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class AddRowActionModel : Action
    {
        public AddRowLocation Location { get; set; }
        public AddRowInputType InputType { get; set; }
        public Guid SaveMapId { get; set; }
        public Guid SaveMapAppObjectUniqueId { get; set; }
        public string InputValue { get; set; }
        public string SaveGroupTargetNamedRange { get; set; }
        public bool DisableExcelEvents { get; set; }
        public AddRowActionModel()
        {
            Type = Constants.ADDROW_ACTION;
        }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }

        internal List<string> GetInputObjects()
        {
            List<string> inputObjects = new List<String>();
            if (InputType == AddRowInputType.ActionFlowStepInput)
                inputObjects.Add(InputValue);
            return inputObjects;
        }
    }
}
