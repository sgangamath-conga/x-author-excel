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
    public class InputActionController : IInputActionController
    {
        private InputActionView View;
        private InputActionModel Model;
        public FormOpenMode FormMode;

        public InputActionController(InputActionView view, InputActionModel model, FormOpenMode mode)
        {
            View = view;
            Model = model;
            FormMode = mode;

            View.SetController(this);
            View.Show();
        }

        public void SetView()
        {
            View.FillObjects();
            if (FormMode == Core.FormOpenMode.Edit)
                View.UpdateControls(Model); 
        }

        public void Save(Core.InputActionModel model)
        {
            Model.Name = model.Name;
            Model.InputActionVariables.Clear();
            foreach (InputActionVariable var in model.InputActionVariables)
                Model.InputActionVariables.Add(var);

            if (FormMode == Core.FormOpenMode.Create)
            {
                Model.Type = Constants.INPUT_ACTION;
                Model.Save();
            }
            View.Dispose();
        }

        public void ValidateAction(System.Windows.Forms.Control control, System.ComponentModel.CancelEventArgs e, string errorMsg)
        {
            //String errorMsg = "Name cannot be empty";
            if (string.IsNullOrEmpty(control.Text))
            {
                e.Cancel = true;
                View.SetError(control, errorMsg);
            }
            else
            {
                e.Cancel = false;
                View.SetError(control, string.Empty);
            }
        }
    }
}
