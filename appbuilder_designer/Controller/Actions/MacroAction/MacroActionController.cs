/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{

    public class MacroActionController : IMacroActionController
    {
        private MacroActionModel model;
        private MacroActionView view;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private FormOpenMode formOpenMode;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public MacroActionController(MacroActionModel model, MacroActionView view, FormOpenMode formOpenMode)
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
        /// <summary>
        /// Set View
        /// </summary>
        public void SetView()
        {
            
            view.SetMacroList();
            
            switch (formOpenMode)
            {
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="ActionName"></param>
        /// <param name="MacroId"></param>
        public void Save(string ActionName, string MacroName, bool MacroOutput, bool ExcelEventsDisabled)
        {
            //// Controls collection can be the whole form or just a panel or group
            //if (ApttusFormValidator.hasValidationErrors(view.Controls))
            //    return;

            // if we get here means the validation passed. Save the data            
            model.Name = ActionName;
            model.MacroName = MacroName;
            model.MacroOuput = MacroOutput;
            model.ExcelEventsDisabled = ExcelEventsDisabled;
            model.Type = Constants.MACRO_ACTION;
            if (formOpenMode == FormOpenMode.Create)
                model.Write();
            if (view != null)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), view.Handle.ToInt32());
                view.Dispose();
            }
        }

        public void Cancel()
        {
            view.Dispose();
        }

        public void ValidateAction(Control action, CancelEventArgs e, string errorMsg)
        {
            if (action.Text.Trim() == String.Empty)
            {
                view.SetError(action, errorMsg);
                e.Cancel = true;
            }
            else
                //Remove error
                view.SetError(action, string.Empty);            
        }
    }
}
