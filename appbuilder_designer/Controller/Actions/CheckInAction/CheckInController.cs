/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class CheckInController : ICheckInController
    {
        private CheckInView View;
        private CheckInModel Model;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private FormOpenMode FormOpenMode;

        public CheckInController(CheckInView view, CheckInModel model, FormOpenMode formOpenMode)
        {
            View = view;
            Model = model;
            FormOpenMode = formOpenMode;
            View.SetController(this);
            View.TopMost = true;
            View.Show();
        }

        public void SetView()
        {
            View.FillObjects();
            if (FormOpenMode == Core.FormOpenMode.Edit)
                View.UpdateControls(Model);
        }

        public void Save(string ActionName, string FileName, string cellRef, ApttusObject obj)
        {
            // Controls collection can be the whole form or just a panel or group
            if (ApttusFormValidator.hasValidationErrors(View.PanelControls))
                return;

            //create and save named range if using cell reference
            if(!string.IsNullOrEmpty(cellRef))
            {
                if(ExcelHelper.IsValidCellReference(cellRef))
                {
                    Microsoft.Office.Interop.Excel.Range range = ExcelHelper.GetRangeByLocation(cellRef);
                    string namedRange = ExcelHelper.CreateUniqueNameRange();
                    ExcelHelper.AssignNameToRange(range, namedRange);
                    Model.namedRange = namedRange;
                }                
            }            
            Model.AppObjectId = obj.UniqueId;
            Model.FileName = FileName;
            Model.Name = ActionName;
            Model.Type = Constants.CHECK_IN_ACTION;

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
        Microsoft.Office.Interop.Excel.Range CurrentRange;
        public string GetActiveColReference()
        {
            //if (radBasicProp.Checked)
            CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            return (ExcelHelper.GetAddress(CurrentRange));
        }
    }
}
