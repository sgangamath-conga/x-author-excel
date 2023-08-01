/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Linq;
using System.ComponentModel;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public enum InputVariableType
    {
        [Description("User Input")]
        UserInput,
        [Description("ExcelCell Reference")]
        ExcelCellReference
    }

    [Serializable]
    public class InputActionVariable
    {
        public string VariableName { get; set; }
        public string ObjectName { get; set; }
        public Guid ObjectUniqueId { get; set; }
        public string FieldName { get; set; }
        public string FieldId { get; set; }
        public InputVariableType VariableType { get; set; }
        public string ExcelCellReference { get; set; }

        public InputActionVariable Clone()
        {           
            return new InputActionVariable
            {
                VariableName = this.VariableName,
                ObjectName = this.ObjectName,
                FieldName = this.FieldName,
                VariableType = this.VariableType,
                FieldId = this.FieldId,
                ObjectUniqueId = this.ObjectUniqueId,
                ExcelCellReference = this.ExcelCellReference
            };
        }
    }

    [Serializable]
    public class InputActionModel : Action
    {
        public InputActionModel()
        {
            InputActionVariables = new BindingList<InputActionVariable>();
        }

        public BindingList<InputActionVariable> InputActionVariables { get; private set; }

        public void AddVariable(InputActionVariable entry)
        {
            InputActionVariables.Add(entry);
        }

        public bool VariableExists(string name, ApttusObject apttusObject, ApttusField apttusField)
        {
            //Check if name exists
            bool bVariableExists = InputActionVariables.Where(var => var.VariableName.Equals(name)).Count() != 0;
            //if (apttusObject == null || apttusField == null)
                return bVariableExists;

            //There can be only one var per field, Check if the field name exists.
        //    return bVariableExists || InputActionVariables.Where(var => var.ObjectName.Equals(apttusObject.Name) && var.FieldId.Equals(apttusField.Id)).Count() != 0;
        }

        public void Save()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }

        public InputActionModel Clone()
        {
            InputActionModel dupCopy = new InputActionModel
            {
                Id = this.Id,
                Name = this.Name,
                Type = Constants.INPUT_ACTION
            };

            foreach (InputActionVariable variable in InputActionVariables)
                dupCopy.AddVariable(variable.Clone());
            return dupCopy;
        }
    }
}
