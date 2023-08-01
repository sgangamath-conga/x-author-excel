/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class SwitchConnectionController
    {
        private SwitchConnectionActionModel model;
        private SwitchConnectionActionView view;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private FormOpenMode formOpenMode;

        public SwitchConnectionController(SwitchConnectionActionModel model, SwitchConnectionActionView view, FormOpenMode formOpenMode)
        {
            this.model = model;
            this.view = view;
            this.formOpenMode = formOpenMode;
            if (view != null)
            {
                this.view.SetController(this);
                this.view.ShowDialog();
            }
        }

        public void SetView()
        {
            if (formOpenMode == FormOpenMode.Edit)
                view.UpdateControls(model);
        }

        public void Save(string ActionName, string SwitchToConnectionName)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(view.PanelControls))
                return;

            model.SwitchToConnectionName = SwitchToConnectionName;
            model.Name = ActionName;
            model.Type = Constants.SWITCH_CONNECTION_ACTION;

            if (formOpenMode == Core.FormOpenMode.Create)
                model.Write();
            if(view != null)
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), view.Handle.ToInt32());

            view.Dispose();
        }

        public void ValidateAction(Control control, CancelEventArgs e, string errorMsg)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                e.Cancel = true;
                view.SetError(control, errorMsg);
            }
            else
            {
                e.Cancel = false;
                view.SetError(control, string.Empty);
            }
        }

    }
}
