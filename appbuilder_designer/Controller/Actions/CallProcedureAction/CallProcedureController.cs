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
    public class CallProcedureController
    {
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

        public CallProcedureModel model;
        private CallProcedure view;
        private FormOpenMode formOpenMode;

        public CallProcedureController(CallProcedureModel model, CallProcedure view, FormOpenMode formMode)
        {
            this.model = model;
            this.view = view;
            this.formOpenMode = formMode;
            this.view.SetController(this);
            this.view.ShowDialog();
        }

        public void SetView()
        {
            view.PopulateClass();
            switch (formOpenMode)
            {
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;
                default:
                    break;
            }
        }

        public List<ApttusObject> GetApplicationObjects()
        {
            return appDefManager.GetAllObjects();
        }

        public bool Save(string actionName, string className, string methodName,bool enableCache, List<MethodParam> sParams, Guid returnObject)
        {
            model.Name = actionName;
            model.Type = Constants.CALL_PROCEDURE_ACTION;
            model.ClassName = className;
            model.MethodName = methodName;
            model.EnableCache = enableCache;
            model.MethodParams = sParams;
            model.ReturnObject = returnObject;

            if (formOpenMode == FormOpenMode.Create)
                model.Write();

            return true;
        }

        internal void ValidateAction(Control control, System.ComponentModel.CancelEventArgs e, string errorMsg)
        {
            //String errorMsg = "Name cannot be empty";
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
