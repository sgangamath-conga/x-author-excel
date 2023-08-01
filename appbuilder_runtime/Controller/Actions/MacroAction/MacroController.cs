/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
namespace Apttus.XAuthor.AppRuntime
{
    public class MacroController
    {
        public ActionResult Result { get; private set; }        
        private MacroActionModel model;
        private string queryString;
        private ApttusObject appObject;
        DataManager dataManager = DataManager.GetInstance;

        // Constructor
        public MacroController(MacroActionModel model)
        {
            this.model = model;           
            Result = new ActionResult();
        }
        // Execute from workflow for macro action
        public ActionResult Execute()
        {
            try
            {
                if (model.ExcelEventsDisabled == true)
                    ExcelHelper.ExcelApp.EnableEvents = false;

                Excel.Application app = null;
                if (ObjectManager.RuntimeMode != RuntimeMode.Batch)
                     app = (Excel.Application)Globals.ThisAddIn.Application;
                else
                    app = ObjectManager.GetInstance.ExcelApp as Excel.Application;
                if(!String.IsNullOrEmpty(this.model.MacroName) && app != null)
                {
                    // Call Macro with Name
                    dynamic retVal = app.Run(this.model.MacroName);
                    if (model.MacroOuput == true && retVal != null)
                    {
                        if (retVal is bool && Convert.ToBoolean(retVal) == true)
                        {
                            Result.Status = ActionResultStatus.Failure;
                            return Result;
                        }
                    }
                    Result.Status = ActionResultStatus.Success;
                }                
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Macro Action");
                Result.Status = ActionResultStatus.Failure;
            }
            finally
            {
                if (model.ExcelEventsDisabled == true)
                    ExcelHelper.ExcelApp.EnableEvents = true;
            }

            return Result;
        }        
    }
}
