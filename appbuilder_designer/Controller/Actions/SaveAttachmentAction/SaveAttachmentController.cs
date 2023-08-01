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
using System.ComponentModel;

namespace Apttus.XAuthor.AppDesigner
{
    class SaveAttachmentController : ISaveAttachmentController
    {
        private SaveAttachmentView View;
        private SaveAttachmentModel Model;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private FormOpenMode FormOpenMode;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveAttachmentController(SaveAttachmentView view, SaveAttachmentModel model, FormOpenMode formOpenMode)
        {
            View = view;
            Model = model;
            FormOpenMode = formOpenMode;
            View.SetController(this);
            //view.ShowDialog();
            View.TopMost = true;
            View.Show();            
        }

        public void SetView()
        {
            View.FillData();
            if (FormOpenMode == Core.FormOpenMode.Edit)
                View.UpdateControls(Model);
        }

        public bool Save(string ActionName, string FileName, FileSaveType SaveType, ApttusObject obj, AttachmentFormat format, bool hasCustomSheets, List<string> sheets, bool IsFileNameusingCellRef)
        {
            try
            {
                // Controls collection can be the whole form or just a panel or group
                if (ApttusFormValidator.hasValidationErrors(View.PanelControls))
                    return false;

                //create and save named range if using cell reference
                if (!string.IsNullOrEmpty(FileName) && IsFileNameusingCellRef == true)
                {
                    if (ExcelHelper.IsValidCellReference(FileName))
                    {
                        Microsoft.Office.Interop.Excel.Range range = ExcelHelper.GetRangeByLocation(FileName);
                        string namedRange = ExcelHelper.CreateUniqueNameRange();
                        ExcelHelper.AssignNameToRange(range, namedRange);
                        Model.FileName = namedRange;
                    }
                }
                else
                {
                    Model.FileName = FileName;
                }

                Model.AppObjectId = obj.UniqueId;
                Model.IsFileNameUsingCellRef = IsFileNameusingCellRef;
                Model.Name = ActionName;
                Model.SaveType = SaveType;
                Model.AttachmentFormat = format;
                Model.HasCustomSheets = hasCustomSheets;
                Model.IncludedSheets = sheets;
                Model.Type = Constants.SAVE_ATTACHMENT_ACTION;

                if (FormOpenMode == Core.FormOpenMode.Create)
                    Model.Write();

                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"), View.Handle.ToInt32());

                View.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
        }

        public void ValidateAction(Control control, CancelEventArgs e, string errorMsg)
        {
            //string errorMsg = string.Empty;
            //switch (control.Name)
            //{
            //    case "txtActionName":
            //        errorMsg = "Action Name is Required";                    
            //        break;
            //    case "txtFileName":
            //        errorMsg = "File Name is Required";
            //        break;
            //}

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
            CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            return (ExcelHelper.GetAddress(CurrentRange));
        }
    }
}
