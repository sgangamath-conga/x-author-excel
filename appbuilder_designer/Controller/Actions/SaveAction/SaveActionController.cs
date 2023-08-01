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

    public class SaveActionController : ISaveActionController
    {
        private SaveActionModel model;
        private SaveActionView view;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private FormOpenMode formOpenMode;

        public SaveActionController(SaveActionModel model, SaveActionView view, FormOpenMode formOpenMode)
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
            view.SetSaveMapsList(configurationManager.SaveMaps);
            switch (formOpenMode)
            {
                case FormOpenMode.Create:
                    view.SetDefaultValues();
                    break;
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;
                default:
                    break;
            }
        }

        public bool Save(string ActionName, Guid SaveMapId, bool bEnableCollisionDetection, bool bEnablePartialSave, bool bEnableRowHighlightErrors, int BatchSize)
        {
            //// Controls collection can be the whole form or just a panel or group
            //if (ApttusFormValidator.hasValidationErrors(view.Controls))
            //    return;

            try
            {
                // if we get here means the validation passed. Save the data
                model.Type = Constants.SAVE_ACTION;
                model.Name = ActionName;
                model.SaveMapId = SaveMapId;
                model.EnableCollisionDetection = bEnableCollisionDetection;
                model.EnablePartialSave = bEnablePartialSave;
                model.EnableRowHighlightErrors = bEnableRowHighlightErrors;
                model.BatchSize = BatchSize == 0 ? Constants.SAVE_ACTION_BATCH_SIZE : BatchSize;
                if (formOpenMode == FormOpenMode.Create)
                    model.Write();

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }

            //if (view != null)
            //    view.Dispose();
        }

        public void Cancel()
        {
            view.Dispose();
        }

        public void ValidateAction(Control action, CancelEventArgs e, string errorMsg)
        {
            if (action.Name == "txtBatchSize")
            {
                int result;
                if (!(int.TryParse(action.Text, out result) && (result > 0 & result <= Constants.SAVE_ACTION_BATCH_SIZE)))
                {
                    view.SetError(action, errorMsg);
                    e.Cancel = true;
                }
            }
            else if (action.Text.Trim() == String.Empty)
            {
                view.SetError(action, errorMsg);
                //errorProvider1.SetError(action, "Last Name is Required");
                e.Cancel = true;
            }
            else
                //Remove error
                view.SetError(action, string.Empty);
            //errorProvider1.SetError(textBox1, string.Empty);

            //if (action.Text.Trim() == String.Empty)
            //{
            //    view.SetError(action, "Action Name is Required");
            //    e.Cancel = true;
            //}
        }
    }
}
