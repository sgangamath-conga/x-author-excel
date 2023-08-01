/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class CheckOutController : ICheckOutController
    {
        private CheckOutView View;
        private CheckOutModel Model;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private FormOpenMode FormOpenMode;

        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public CheckOutController(CheckOutView view, CheckOutModel model, FormOpenMode formOpenMode)
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

        public void Save(string ActionName, ApttusObject obj)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(View.PanelControls))
                return;

            Model.AppObjectId = obj.UniqueId;
            Model.Name = ActionName;
            Model.Type = Constants.CHECK_OUT_ACTION;

            if (FormOpenMode == Core.FormOpenMode.Create)
                Model.Write();
            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), View.Handle.ToInt32());
            View.Dispose();
        }

        public void ValidateAction(Control control, CancelEventArgs e, string errorMsg)
        {
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
