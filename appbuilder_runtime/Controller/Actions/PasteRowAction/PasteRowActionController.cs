using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    class PasteRowActionController
    {
        private PasteRowActionModel Model;
        public ActionResult Result { get; private set; }
        private DataManager dataManager = DataManager.GetInstance;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private Excel.Range targetNamedRange;
        private Excel.Worksheet worksheet;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public PasteRowActionController(PasteRowActionModel model)
        {
            Model = model;
            targetNamedRange = ExcelHelper.GetRange(model.SaveGroupTargetNamedRange);
            worksheet = targetNamedRange.Worksheet;
            Result = new ActionResult();
        }

        /// <summary>
        /// Returns first or last cell within the given namedrange. This namedrange either belongs to displaymap or saveother map.
        /// </summary>
        /// <returns></returns>
        private Excel.Range GetCellByLocation()
        {
            Excel.Range cellInRange = null;
            if (Model.RowLocation == AddRowLocation.Top)
                cellInRange = targetNamedRange.Cells[1, 1] as Excel.Range; //first Cell in range
            else if (Model.RowLocation == AddRowLocation.Bottom)
            {
                int rowCount = targetNamedRange.Rows.Count;
                cellInRange = targetNamedRange[rowCount, 1] as Excel.Range; //last cell in range
            }
            return cellInRange;
        }

        public ActionResult Execute()
        {
            Result.Status = ActionResultStatus.Pending_Execution;
            try
            {
                Globals.ThisAddIn.Application.EnableEvents = false;
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

                if (dataObject == null)
                {
                    ExceptionLogHelper.InfoLog("Paste Row Action: No Data found on clipboard to execute paste row action", null);
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }

                if (dataObject.GetDataPresent(DataFormats.CommaSeparatedValue) && dataObject.GetDataPresent(DataFormats.Rtf))
                {
                    //Active the Worksheet as this sheet may be not the active sheet in the workbook.
                    worksheet.Activate();

                    //This cell is the first or last cell in the target range. Target Range either refers to a retrievemap or a saveother map.
                    Excel.Range firstOrLastCellInTargetRange = GetCellByLocation();
                    if (firstOrLastCellInTargetRange == null)
                    {
                        Result.Status = ActionResultStatus.Failure;
                        return Result;
                    }
                    firstOrLastCellInTargetRange.Select(); //Select the cell in the target namedrange, so that we can add rows in that range. This is mandatory.

                    Excel.XlPasteType pasteType = Model.PasteType == PasteType.PasteAll ? Excel.XlPasteType.xlPasteAll : Excel.XlPasteType.xlPasteValues;

                    RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(firstOrLastCellInTargetRange, targetNamedRange);
                    runtimeEditActionController.Paste(dataObject, pasteType);

                    //Clear the clipboard once the paste operation is performed as this data is no longer usable.
                    Clipboard.Clear();
                }
                else
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("COMMONRUNTIME_NoDataCopied_ErrorMsg"), resourceManager.GetResource("PASTEROWACTCTL_ExecuteCAP_InfoMsg"), Globals.ThisAddIn.Application.Hwnd);

                Result.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                Result.Status = ActionResultStatus.Failure;
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
            return Result;
        }

    }
}
