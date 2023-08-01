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
    public class QueryActionController
    {
        private QueryActionModel model;
        private QueryActionView view;

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;

        private FormOpenMode formOpenMode;

        public QueryActionController(QueryActionModel model, QueryActionView view, FormOpenMode formOpenMode)
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
            switch (formOpenMode)
            {
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;
                case FormOpenMode.Create:
                    view.LoadBlankForm();
                    break;
                default:
                    break;
            }
        }

        private List<ApttusFieldDS> GetFieldsList(ApttusObject AppObject, List<ApttusField> allFields)
        {
            List<ApttusFieldDS> list = new List<ApttusFieldDS>();
            if (AppObject.Fields != null)
            {
                foreach (ApttusField field in allFields)
                {
                    ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype, field.Updateable, field.FormulaType,field.CRMDataType,field.Creatable);
                    if (AppObject.Fields.Exists(a => a.Id == field.Id))
                        FieldDS.Included = true;
                    list.Add(FieldDS);
                }
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }
            else
            {
                foreach (ApttusField field in allFields)
                {
                    ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype, field.Updateable, field.FormulaType,field.CRMDataType, field.Creatable);
                    list.Add(FieldDS);
                }
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }
        }

        public List<ApttusObject> GetObjects()
        {
            //return ApplicationDefinitionManager.GetInstance.GetParentAndChildObjects();
            return ApplicationDefinitionManager.GetInstance.GetAllObjects();
        }

        public List<ApttusFieldDS> GetFieldList(ApttusObject appObject)
        {
            return GetFieldsList(appObject, appObject.Fields);
        }

        public List<SearchFilterGroup> GetFilterGroup()
        {
            return model.WhereFilterGroups;
        }

        public void Save(string name, ApttusObject appObject, List<SearchFilterGroup> whereFilter, string maxRecords, bool allowSavingFilters = false)
        {
            // Controls collection can be the whole form or just a panel or group
            if (view != null)
            {
                if (ApttusFormValidator.hasValidationErrors(view.Controls))
                    return;
            }
            // if we get here means the validation passed. Save the data
            model.Type = Constants.EXECUTE_QUERY_ACTION;
            model.Name = name;
            model.TargetObject = appObject.UniqueId;
            model.WhereFilterGroups = whereFilter;
            model.AllowSavingFilters = allowSavingFilters;
            try
            {
                model.MaxRecords = string.IsNullOrEmpty(maxRecords) ? QueryActionModel.MAX_RECORDS_NOLIMIT : Convert.ToInt32(maxRecords);
            }
            catch (Exception e)
            {
                model.MaxRecords = QueryActionModel.MAX_RECORDS_NOLIMIT;
            }

            if (formOpenMode == FormOpenMode.Create)
                model.Write();

            //view.Dispose();
        }

        public void Cancel()
        {
            view.Dispose();
        }

        public void ValidateAction(Control action, CancelEventArgs e)
        {
            /*
            if (action.Text.Trim() == String.Empty)
            {
                view.SetError(action, "Action Name is Required");
                //errorProvider1.SetError(action, "Last Name is Required");
                e.Cancel = true;
            }
            else
                //Remove error
                view.SetError(action, string.Empty);
            }
             * */
        }
    }
}
