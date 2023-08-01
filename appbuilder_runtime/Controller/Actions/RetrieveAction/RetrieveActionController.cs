/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppRuntime
{
    public class RetrieveAction : IInputActionData
    {
        // Get DataManager instance
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;

        private RetrieveActionView View;
        private RetrieveMap Model;
        private RetrieveActionModel retrieveAction;
        public ActionResult Result { get; private set; }

        public bool InputData { get; set; }

        public string[] InputDataName { get; set; }

        // Define Constructor , this constructor call from workflow
        public RetrieveAction(RetrieveActionModel model)
        {
            retrieveAction = model;
            // Get Instance of MapID
            Model = ConfigurationManager.GetInstance.RetrieveMaps.SingleOrDefault(m => m.Id == model.RetrieveMapId);
           // if (Model != null && Model.CrossTabMaps != null)
           //     CrossTabModels = Model.CrossTabMaps;
            //ConfigurationManager.GetInstance.CrossTabRetrieveMaps.SingleOrDefault(x => x.Id == model.RetrieveMapId);
            View = new RetrieveActionView();
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

                    ExcelHelper.UpdateApplicableSheetsForUserProtection(false, Model);
                    //foreach (Worksheet oSheet in ExcelHelper.ExcelApp.Worksheets)
                    //{
                    //    // If sheet is protect by user then it will unprotect for rendering
                    //    if (ExcelHelper.IsUserSheetProtection(oSheet))
                    //        ExcelHelper.UserSheetProtection(oSheet, false);
                    //}

                    // Clear Data
                    View.ClearData(retrieveAction, Model, InputData, InputDataName);
                    View.FillData(retrieveAction, Model, InputData, InputDataName);
                    //if (CrossTabModels != null)
                    //    View.FillCrossTabData(CrossTabModels, InputData, InputDataName);

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
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Display Action");
                Result.Status = ActionResultStatus.Failure;
            }
            finally
            {
                ExcelHelper.UpdateApplicableSheetsForUserProtection(true, Model);
                WorkbookEventManager.GetInstance.SubscribeEvents();
            }
            return Result;
        }
    }
}
