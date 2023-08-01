/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public class ClearActionController : IClearActionController
    {
        private ClearActionView View; 
        private ClearActionModel Model;
        private FormOpenMode FormOpenMode;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ClearActionController(ClearActionView view, ClearActionModel model, FormOpenMode formOpenMode)
        {
            View = view;
            Model = model;
            FormOpenMode = formOpenMode;
            View.SetController(this);
            View.ShowDialog();
        }

        public void SetView()
        {
            View.FillObjects();
            if (FormOpenMode == Core.FormOpenMode.Edit)
                View.UpdateControls(Model); 
        }

        public void Save(ClearActionModel Model)
        {
            //if (ApttusFormValidator.hasValidationErrors(View.Controls))
            //    return;

            Model.Type = Constants.CLEAR_ACTION;

            if (FormOpenMode == Core.FormOpenMode.Create)
                Model.Write();
            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), View.Handle.ToInt32());
            View.Dispose();
        }

        public void ValidateAction(Control control, System.ComponentModel.CancelEventArgs e, string errorMsg)
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
