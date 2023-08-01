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

    public class RetrieveActionController : IRetrieveActionController
    {
        private RetrieveActionModel model;
        private RetrieveActionView view;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private FormOpenMode formOpenMode;

        public RetrieveActionController(RetrieveActionModel model, RetrieveActionView view, FormOpenMode formOpenMode)
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
            if (configurationManager.RetrieveMaps.Count > 0 || configurationManager.MatrixMaps.Count > 0)
            {
            //    view.SetRetrieveMapsList(configurationManager.CrossTabRetrieveMaps );
            //}
            //else
            //{
                view.SetRetrieveMapsList(configurationManager.RetrieveMaps, configurationManager.MatrixMaps);
            }
            switch (formOpenMode)
            {
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;
                default:
                    break;
            }
        }

        public bool Save(string ActionName, Guid RetrieveMapId, List<ShowQueryFilterObjectConfiguration> ShowQueryFilterObjectConfigurations, bool disableExcelEvents = true)
        {
            //// Controls collection can be the whole form or just a panel or group
            //if (ApttusFormValidator.hasValidationErrors(view.Controls))
            //    return;
            try
            {
                // if we get here means the validation passed. Save the data
                model.Type = Constants.RETRIEVE_ACTION;
                model.Name = ActionName;
                model.DisableExcelEvents = disableExcelEvents;
                model.RetrieveMapId = RetrieveMapId;
                model.ShowQueryFilterObjectConfigurations = ShowQueryFilterObjectConfigurations;

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
            if (action.Text.Trim() == String.Empty)
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
