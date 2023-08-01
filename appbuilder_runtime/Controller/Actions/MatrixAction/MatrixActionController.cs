/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class MatrixActionController
    {
        // Get DataManager instance
        DataManager dataManager = DataManager.GetInstance;
        private MatrixMap Model;
        private MatrixActionView View;
        public ActionResult Result { get; private set; }
        public bool InputData { get; set; }

        public string[] InputDataName { get; set; }

        public MatrixActionController(RetrieveActionModel model)
        {
            Model = ConfigurationManager.GetInstance.MatrixMaps.SingleOrDefault(x => x.Id == model.RetrieveMapId);
            View = new MatrixActionView();
            Result = new ActionResult();
        }

        /// <summary>
        /// Call Execute method from wf 
        /// </summary>
        /// <returns></returns>
        public ActionResult Execute()
        {
            try
            {
                if (Model != null)
                {
                    WorkbookEventManager.GetInstance.UnsubscribeEvents();

                    //foreach (Worksheet oSheet in ExcelHelper.ExcelApp.Worksheets)
                    //{
                    //    // If sheet is protect by user then it will unprotect for rendering
                    //    if (ExcelHelper.IsUserSheetProtection(oSheet))
                    //        ExcelHelper.UserSheetProtection(oSheet, false);
                    //}

                    ExcelHelper.UpdateApplicableSheetsForUserProtection(false);

                    // TODO:: Call View class method to render matrix data
                    View.FillData(Model, InputData, InputDataName);

                    //foreach (Worksheet oSheet in ExcelHelper.ExcelApp.Worksheets)
                    //{
                    //    // Unprotect after rendering finish
                    //    ExcelHelper.UserSheetProtection(oSheet, true);
                    //}


                }
                Result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Matrix Action");
                Result.Status = ActionResultStatus.Failure;
            }
            finally
            {
                ExcelHelper.UpdateApplicableSheetsForUserProtection(true);
                WorkbookEventManager.GetInstance.SubscribeEvents();
            }
            return Result;
        }

    }
}
