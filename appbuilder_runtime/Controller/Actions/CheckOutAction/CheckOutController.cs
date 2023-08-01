/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class CheckOutController
    {
        // Model
        private CheckOutModel Model;
        public bool InputData { get; set; }
        public string[] InputDataName { get; set; }
        public ActionResult Result { get; private set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        private ICheckOutProvider checkOutProvider;

        public CheckOutController(CheckOutModel model, string[] inputDataName)
        {
            Model = model;
            InputDataName = inputDataName;
            checkOutProvider = Globals.ThisAddIn.GetCheckOutProvider();
            Result = new ActionResult();
        }

        public ActionResult Execute()
        {
            try
            {
                DataManager manager = DataManager.GetInstance;
                ConfigurationManager configManager = ConfigurationManager.GetInstance;

                ApttusDataSet objectData = null;

                // Get input data set from datamanager
                objectData = manager.GetDatasetsByNames(InputDataName).FirstOrDefault();
                ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(objectData.AppObjectUniqueID);
                string recordId = string.Empty;

                if (objectData == null)
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("CHECKOUTCTL_NoAttachment_ErrorMsg"), resourceManager.GetResource("COMMON_FileCheckedOutCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
                recordId = objectData.DataTable.Rows[0][appObject.IdAttribute].ToString();

                // Present UI to select Check Out file
                CheckOutView view = new CheckOutView(checkOutProvider);

                var FileQuery = checkOutProvider.GetFileQuery(recordId);
                CheckOutViewController controller = new CheckOutViewController(view, FileQuery);

                if (string.IsNullOrEmpty(controller.selectedDocId))
                {
                    Result.Status = ActionResultStatus.Success;
                    return Result;
                }

                if (controller.selectedAppFileLocked && (controller.selectedAppFileUserId != ApttusCommandBarManager.GetInstance().GetUserInfo().UserId))
                {
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CHECKOUTCTL_FileCheckedOut_ErrorMsg"), controller.selectedAppFileUserName), resourceManager.GetResource("COMMON_FileCheckedOutCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Success;
                    return Result;
                }

                // Get File blob using docID from Attachments
                var FileBlobQuery = checkOutProvider.GetFileBlobFromAttachmentsQuery(controller.selectedDocId);
                objectData = objectManager.QueryDataSet(FileBlobQuery);

                if (objectData != null)
                {
                    if (objectData.DataTable.Rows.Count == 0)
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("CHECKOUTCTL_FileDeleted_ErrorMsg"), resourceManager.GetResource("COMMON_FileCheckedOutCap_ErrorMsg"));
                        Result.Status = ActionResultStatus.Success;
                        return Result;
                    }
                    string fileName = checkOutProvider.GetFileNameFromDataTable(objectData.DataTable);
                    string base64Data = checkOutProvider.GetBodyStringFromDataTable(objectData.DataTable);

                    //string filePath = Utils.GetTempFileName(configManager.Definition.UniqueId, configManager.Definition.Name) + "-CheckOut" + Path.GetExtension(fileName);
                    string filePath = Utils.GetTempFileName(configManager.Definition.UniqueId, fileName);
                    //string checkoutfileName = configManager.Definition.Name + "-CheckOut" + Path.GetExtension(fileName);
                    string runtimefileName = configManager.Definition.Name + "-Runtime" + Path.GetExtension(fileName);
                    string runtimefileNameEditInExcel = configManager.Definition.Name + "-" + DateTime.Now.ToString("yyyyMMdd");

                    Microsoft.Office.Interop.Excel.Workbook workbook = ExcelHelper.CheckIfFileOpened(fileName);
                    if (File.Exists(filePath) && workbook != null)
                    {
                        Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("CHECKOUTCTL_Execute_WarnMsg"), fileName), resourceManager.GetResource("CHECKOUTCTL_ExecuteCAP_WarnMsg"), ApttusMessageUtil.YesNo);
                        if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
                        {
                            CloseWorkbook(workbook);
                        }
                        else if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No)
                        {
                            Result.Status = ActionResultStatus.Success;
                            return Result;
                        }
                    }

                    Utils.StreamToFile(Convert.FromBase64String(base64Data), filePath, true);

                    // Marks file being checked out as locked
                    MarkFileAsLocked(controller.selectedAppFileId);

                    // Close already open runtime, as multiple instances of same runtime is not supported
                    if (Globals.ThisAddIn.Application.ActiveWorkbook != null && (Globals.ThisAddIn.Application.ActiveWorkbook.Name.Equals(runtimefileName) || Globals.ThisAddIn.Application.ActiveWorkbook.Name.StartsWith(runtimefileNameEditInExcel)))
                    //if (ExcelHelper.CheckIfAppOpened(configManager.Definition.UniqueId))
                    {
                        //Globals.ThisAddIn.Application.ActiveWorkbook.Save();
                        CloseWorkbook(Globals.ThisAddIn.Application.ActiveWorkbook);
                    }

                    // Open checkout file for user
                    ApttusObjectManager.OpenFile(filePath);

                    // Set property for app file ID, when user checks in back, use this ID to remove lock status
                    ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_CHECKOUT_FILE_ID, controller.selectedAppFileId);

                    ApttusCommandBarManager.EditInExcelMode = false;
                    ApttusCommandBarManager.GetInstance().DoPostLoad(null, false);
                }

                Result.Status = ActionResultStatus.Success;
                return Result;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                Result.Status = ActionResultStatus.Failure;
                ApttusMessageUtil.ShowError(ex.Message.ToString(), resourceManager.GetResource("CHECKOUTCTL_ExecuteCatch_InfoMsg"));
                return Result;
            }
            finally
            {
                // On Check out action we close prevoiusly opened Runtime file and open file Checked in by Some other user
                // now ApttusObject and Field level security may differ between two users (user who checked in file and user who is checking out file)
                // hence clear refreshed objects list and describeSobject again to get correct Field level security of logged in user.
                FieldLevelSecurityManager.Instance.RefreshedObjectsList.Clear();
            }
        }

        public static void CloseWorkbook(Microsoft.Office.Interop.Excel.Workbook wb)
        {
            try
            {
                if (wb != null)
                {
                    try
                    {
                        ApttusCommandBarManager.g_IsUserClosingRuntimeApp = true;
                        wb.Close();
                    }
                    finally
                    {
                        ApttusCommandBarManager.g_IsUserClosingRuntimeApp = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

        }
        internal void MarkFileAsLocked(string appFileId)
        {
            DataManager manager = DataManager.GetInstance;
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();

            ApttusSaveRecord updateFileLockedRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField>(),
                ObjectName = checkOutProvider.GetObjectName(),
                RecordRowNo = 0,
                RecordColumnNo = -1,
                ObjectType = ObjectType.Independent,
                RecordId = appFileId,
                OperationType = QueryTypes.UPDATE
            };

            //updateFileLockedRecord.SaveFields.Add(new ApttusSaveField
            //{
            //    FieldId = checkOutProvider.GetObjectPrimaryId(),
            //    Value = appFileId,
            //    DataType = Datatype.String
            //});

            updateFileLockedRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkOutProvider.GetIsFileLocked(),
                Value = "true",
                DataType = checkOutProvider.GetIsFileLockedDataType()
            });

            SaveRecords.Add(updateFileLockedRecord);

            objectManager.Update(SaveRecords, false);
        }
    }
}
