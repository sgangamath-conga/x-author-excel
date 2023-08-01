using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class AddRowActionController
    {
        private AddRowActionModel Model;
        private ActionResult Result { get; set; }
        private DataManager dataManager = DataManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private Excel.Range targetNamedRange;
        private Excel.Worksheet worksheet;

        public string[] InputDataName;

        public AddRowActionController(AddRowActionModel model)
        {
            InputDataName = null;
            Model = model;
            Result = new ActionResult();
            targetNamedRange = ExcelHelper.GetRange(model.SaveGroupTargetNamedRange);
            worksheet = targetNamedRange.Worksheet;
        }

        /// <summary>
        /// Returns first or last cell within the given namedrange. This namedrange either belongs to displaymap or saveother map.
        /// </summary>
        /// <returns></returns>
        private Excel.Range GetCellByLocation()
        {
            Excel.Range cellInRange = null;
            if (Model.Location == AddRowLocation.Top)
                cellInRange = targetNamedRange.Cells[1, 1] as Excel.Range; //first Cell in range
            else if (Model.Location == AddRowLocation.Bottom)
            {
                int rowCount = targetNamedRange.Rows.Count;
                if (rowCount <= 2) //why 2. First is the label row and second one is formula row. 
                    cellInRange = targetNamedRange.Cells[1, 1] as Excel.Range; //Since the user selected bottom option, but there are no rows rendered, hence select the first cell. 
                else
                    cellInRange = targetNamedRange[rowCount, 1] as Excel.Range; //last cell in range
            }
            return cellInRange;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellInRange"></param>
        private void ShowCustomRowsDialog(Excel.Range cellInRange)
        {
            AddCustomRowsView View = new AddCustomRowsView();
            AddCustomRowsController Controller = new AddCustomRowsController(View);
            int RowsToAdd = Controller.GetCustomRows();
            if (RowsToAdd > 0)
                AddRows(cellInRange, RowsToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellInRange"></param>
        private void AddRowFromStaticInput(Excel.Range cellInRange)
        {
            int nRowsToAdd = Convert.ToInt32(Model.InputValue);
            AddRows(cellInRange, nRowsToAdd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellInRange"></param>
        private void AddRowFromCellReference(Excel.Range cellInRange)
        {
            Excel.Range rg = ExcelHelper.GetSingleCellRange(Model.InputValue);
            if (rg != null)
            {
                int nRowsToAdd;
                if (Int32.TryParse(Convert.ToString(rg.Value), out nRowsToAdd) && (nRowsToAdd > 0 && nRowsToAdd < 10001))
                    AddRows(cellInRange, nRowsToAdd);
                else
                    ExceptionLogHelper.DebugLog("Invalid Excel cell reference value");
            }
            else
                ExceptionLogHelper.DebugLog("Invalid Excel cell reference value");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellInRange"></param>
        private void AddRowFromActionFlowStep(Excel.Range cellInRange)
        {
            ApttusObject obj = appDefManager.GetAppObject(Guid.Parse(Model.InputValue));
            if (obj == null)
                return;

            ApttusDataSet inputDataSet = dataManager.ResolveInput(InputDataName, obj);
            if (inputDataSet != null && inputDataSet.DataTable != null)
            {
                int nRowsToAdd = inputDataSet.DataTable.Rows.Count;
                if (nRowsToAdd > 0)
                    AddRows(cellInRange, nRowsToAdd);
            }

        }

        private void AddRows(Excel.Range cellInRange, int nRows)
        {
            RuntimeEditActionController addRows = new RuntimeEditActionController(cellInRange, targetNamedRange);
            addRows.AddRow(nRows);
        }

        public ActionResult Execute()
        {
            if (ObjectManager.RuntimeMode == RuntimeMode.Batch && Model.InputType == AddRowInputType.UserInput)
            {
                ExceptionLogHelper.DebugLog("User Input type is not supported in Batch Mode");
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }
            Excel.XlSheetVisibility originalSheetVisibleStatus = worksheet.Visible;
            try
            {
                if (Model.DisableExcelEvents == true)
                    ExcelHelper.ExcelApp.EnableEvents = false;

                ExcelHelper.ExcelApp.ScreenUpdating = false;

                Result.Status = ActionResultStatus.Pending_Execution;

                //Make the worksheet visible if it is not visible.
                if (originalSheetVisibleStatus != Excel.XlSheetVisibility.xlSheetVisible)
                    worksheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;

                //Active the Worksheet as this sheet may be not the active sheet in the workbook.
                worksheet.Activate();

                //This cell is the first cell in the target range. Target Range either refers to a retrievemap or a saveother map.
                Excel.Range firstOrLastCellInTargetRange = GetCellByLocation();

                if (firstOrLastCellInTargetRange == null)
                {
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
                firstOrLastCellInTargetRange.Select(); //Select the cell in the target namedrange, so that we can add rows in that range. This is mandatory.

                switch (Model.InputType)
                {
                    case AddRowInputType.UserInput: //User wants to pop up the custom add row dialog
                        ShowCustomRowsDialog(firstOrLastCellInTargetRange);
                        break;
                    case AddRowInputType.StaticInput: //User has specified the number of rows in Model.InputValue
                        AddRowFromStaticInput(firstOrLastCellInTargetRange);
                        break;
                    case AddRowInputType.CellReference:
                        AddRowFromCellReference(firstOrLastCellInTargetRange);
                        break;
                    case AddRowInputType.ActionFlowStepInput:
                        AddRowFromActionFlowStep(firstOrLastCellInTargetRange);
                        break;
                }
                Result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                Result.Status = ActionResultStatus.Failure;
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                //restore the original sheet visibility status
                worksheet.Visible = originalSheetVisibleStatus;

                if (Model.DisableExcelEvents == true)
                    ExcelHelper.ExcelApp.EnableEvents = true;

                ExcelHelper.ExcelApp.ScreenUpdating = true;
            }
            return Result;
        }
    }
}
