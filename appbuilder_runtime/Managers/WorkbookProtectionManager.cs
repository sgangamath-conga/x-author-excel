/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class WorkbookProtectionManager
    {
        private static WorkbookProtectionManager instance;
        private static object syncRoot = new Object();
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        DataManager dataManager = DataManager.GetInstance;

        private WorkbookProtectionManager()
        {
        }

        public static WorkbookProtectionManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new WorkbookProtectionManager();
                    }
                }
                return instance;
            }
        }

        #region "Public Methods"

        internal void ProtectRows(string TargetNamedRange, Excel.Range rowStartCell, int RowsToAdd)
        {
            SaveMap currentSaveMap = configurationManager.GetSaveMapbyTargetNamedRange(TargetNamedRange);
            SaveGroup currentSaveGroup = configurationManager.GetSaveGroupbyTargetNamedRange(TargetNamedRange);
            Excel.Worksheet worksheet = rowStartCell.Worksheet;

            // 5.1 Find the Id Column Index
            int IdColumnIndex = configurationManager.GetIdColumnIndex(TargetNamedRange);

            if (IdColumnIndex > 0)
            {
                // 5.2 Calculate Range for Protection
                Excel.Range startCell = worksheet.Cells[rowStartCell.Row, rowStartCell.Column + IdColumnIndex - 1];
                Excel.Range endCell = ExcelHelper.NextVerticalCell(startCell, RowsToAdd - 1);
                Excel.Range dataProtectionRange = worksheet.Range[startCell, endCell];

                // 5.3 Unprotect Sheet to enable cells locking/unlocking
                ExcelHelper.UpdateSheetProtection(worksheet, false);

                // 5.4 If sheet is Protected and sheet name exists in DataProtection collection
                bool bAddDataProtection = false;
                if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(worksheet.Name)))
                {
                    // 5.5 Sheet is protected. Now check if current repeating grid is protected
                    if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(worksheet.Name)
                        & dp.SaveMapId.Equals(currentSaveMap.Id) & dp.SaveGroupAppObject.Equals(currentSaveGroup.AppObject)))
                    {
                        DataProtectionModel dpm = dataManager.DataProtection.FirstOrDefault(dp => dp.WorksheetName.Equals(worksheet.Name)
                            & dp.SaveMapId.Equals(currentSaveMap.Id) & dp.SaveGroupAppObject.Equals(currentSaveGroup.AppObject));
                        dpm.LockCount += RowsToAdd;
                    }
                    else
                    {
                        bAddDataProtection = true;
                    }
                    // 5.6 Lock the add row range
                    ExcelHelper.UpdateCellLock(dataProtectionRange, true);
                }
                else
                {
                    // 5.5 Unlock all cells of the worksheet
                    // Only if user has not ptotected sheet
                    if (!ExcelHelper.IsSheetProtectedByUser(worksheet))
                    {
                        ExcelHelper.UpdateCellLock(worksheet.Cells, false);
                    }

                    // 5.6 Lock the add row range
                    ExcelHelper.UpdateCellLock(dataProtectionRange, true);

                    bAddDataProtection = true;
                }

                // 6. Add Data Protection to Data Manager
                if (bAddDataProtection)
                {
                    //if (!ExcelHelper.IsUserSheetProtection(worksheet))
                    //{
                    dataManager.AddDataProtection(
                        new DataProtectionModel
                        {
                            SaveMapId = currentSaveMap.Id,
                            SaveGroupAppObject = currentSaveGroup.AppObject,
                            WorksheetName = startCell.Worksheet.Name,
                            IdColumnIndex = IdColumnIndex,
                            LockCount = RowsToAdd
                        });
                    //}
                }
                // 7. Protect Sheet
                ExcelHelper.UpdateSheetProtection(worksheet, true);
            }
        }

        #endregion

        #region "Private Helper Methods"

        #endregion

    }
}
