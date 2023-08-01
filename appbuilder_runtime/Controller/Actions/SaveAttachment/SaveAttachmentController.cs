/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class SaveAttachmentController
    {
        // Model
        private SaveAttachmentModel Model;
        public bool InputData { get; set; }
        public string[] InputDataName { get; set; }

        public ActionResult Result { get; private set; }
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveAttachmentController(SaveAttachmentModel model, string[] inputDataName)
        {
            this.Model = model;
            this.InputDataName = inputDataName;

            Result = new ActionResult();
        }

        public ActionResult Execute()
        {
            try
            {
                Result.Status = ActionResultStatus.Pending_Execution;
                DataManager manager = DataManager.GetInstance;
                List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
                ApttusDataSet objectData = null;

                // Nike use case - if InputDataName is Empty, resolve it by self
                if (InputDataName == null)
                {
                    objectData = manager.AppData.Where(d => d.AppObjectUniqueID.Equals(Model.AppObjectId)).FirstOrDefault();
                }
                else
                {
                    // Get input data set from datamanager
                    objectData = manager.GetDatasetsByNames(InputDataName).FirstOrDefault();
                }

                string recordId = string.Empty;

                if (objectData != null)
                    recordId = objectData.DataTable.Rows[0][Constants.ID_ATTRIBUTE].ToString();

                if (!string.IsNullOrEmpty(recordId))
                {
                    string activeWorkbookName = commandBar.GetActivWorkbookName();
                    string attachmentName = string.Empty, extension = string.Empty, base64String = string.Empty;

                    // 1. Get the attachment name
                    // AddNew means use User provided Filename || 
                    // if the savetype = overwrite and mode = batch, we should treat as new other wise 
                    // there will be runtime exception happen in MS because there is no user input and 
                    // excel will try to save the file using the same filename.

                    if (Model.SaveType.Equals(FileSaveType.AddNew) || ObjectManager.RuntimeMode == RuntimeMode.Batch)
                    {
                        if (!Model.IsFileNameUsingCellRef)
                        {
                            attachmentName = Model.FileName + Constants.UNDERSCORE + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt");
                        }
                        else
                        {
                            try
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(ExcelHelper.GetRange(Model.FileName).Value)))
                                {
                                    ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("SAVEACTIONCTL_CellRefEmpty_InfoMsg"), ExcelHelper.GetAddress(ExcelHelper.GetRange(Model.FileName))), resourceManager.GetResource("SAVEACTIONCTL_CellRefEmptyCap_InfoMsg"), resourceManager.GetResource("SAVEACTIONCTL_CellRefEmptyInstruction_InfoMsg"), string.Empty);
                                    return null;
                                }
                            }
                            catch (Exception e)
                            {
                                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("SAVEACTIONCTL_CellRefEmpty_InfoMsg"), ""), resourceManager.GetResource("SAVEACTIONCTL_CellRefEmptyCap_InfoMsg"), resourceManager.GetResource("SAVEACTIONCTL_CellRefEmptyInstruction_InfoMsg"), string.Empty);
                                return null;
                            }
                            attachmentName = Convert.ToString(ExcelHelper.GetRange(Model.FileName).Value);
                        }
                    }

                    // Overwrite means use Runtime Filename
                    else if (Model.SaveType.Equals(FileSaveType.Overwrite))
                    {
                        //string currentFileName = activeWorkbookName.Substring(0, activeWorkbookName.LastIndexOf("."));
                        string currentFileName = Path.GetFileNameWithoutExtension(activeWorkbookName);

                        if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                        {
                            SaveAttachmentFileNameView dlg = new SaveAttachmentFileNameView(currentFileName);
                            dlg.ShowDialog();
                            if (string.IsNullOrEmpty(dlg.FileName))
                            {
                                Result.Status = ActionResultStatus.Failure;
                                return Result;
                            }
                            attachmentName = dlg.FileName;
                        }
                        else if (ObjectManager.RuntimeMode == RuntimeMode.Batch)
                        {
                            attachmentName = currentFileName;
                        }
                    }

                    // 2. Get the extension
                    if (Model.AttachmentFormat == null || Model.AttachmentFormat == AttachmentFormat.Excel)
                    {
                        extension = Path.GetExtension(activeWorkbookName);
                    }
                    else if (Model.AttachmentFormat == AttachmentFormat.PDF)
                    {
                        extension = Constants.DOT + "pdf";
                    }
                    // if there is any special chr's remove them                    
                    if (attachmentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) //if invalid characters in cell make error log and continue
                    {
                        string tempAttachmentName = Utils.ReplaceInvalidChrs(attachmentName);
                        ExceptionLogHelper.ErrorLog("Invalid file name, replacing " + attachmentName + " with " + tempAttachmentName);
                        attachmentName = tempAttachmentName;
                    }
                    string attachmentFullName = Utils.GetTempFileName(configurationManager.Definition.UniqueId, attachmentName + extension);

                    // 3. Get the attachment base64 encoded bytes
                    if (Model.HasCustomSheets)
                    {
                        try
                        {
                            string appWorkbookName = ExcelHelper.ExcelApp.ActiveWorkbook.Name;
                            ExcelHelper.ExcelApp.DisplayAlerts = false;

                            // Create a new workbook
                            var attachmentWorkbook = ExcelHelper.ExcelApp.Workbooks.Add(1);

                            // Rename default sheets of new workbook
                            List<string> sheetsToRemove = new List<string>();
                            foreach (Excel.Worksheet sheet in attachmentWorkbook.Sheets)
                            {
                                sheet.Name = sheet.Name + DateTime.Now.ToString("_HHmmss");
                                sheetsToRemove.Add(sheet.Name);
                            }

                            foreach (string includeSheet in Model.IncludedSheets)
                            {
                                int sheetCount = attachmentWorkbook.Sheets.Count;
                                ExcelHelper.ExcelApp.Workbooks[appWorkbookName].Sheets[includeSheet].Copy(Type.Missing, attachmentWorkbook.Sheets[sheetCount]);
                            }

                            // Delete default sheets
                            foreach (string excludeSheet in sheetsToRemove)
                                attachmentWorkbook.Sheets[excludeSheet].Delete();


                            switch (Model.AttachmentFormat)
                            {
                                case AttachmentFormat.Excel:
                                    attachmentWorkbook.Save();
                                    attachmentWorkbook.SaveAs(
                                        attachmentFullName,                                                 //filename to save as
                                        ExcelHelper.ExcelApp.Workbooks[appWorkbookName].FileFormat,		    //file format
                                        ApttusGlobals.oMissing,			                                    //password
                                        ApttusGlobals.oMissing,			                                    //WriteResPassword
                                        ApttusGlobals.oMissing,			                                    //ReadOnlyRecommended
                                        ApttusGlobals.oMissing,			                                    //CreateBackup
                                        Excel.XlSaveAsAccessMode.xlNoChange,	                            //AccessMode
                                        ApttusGlobals.oMissing,			                                    //ConflictResolution
                                        ApttusGlobals.oFalse,			                                    //AddToMru
                                        ApttusGlobals.oMissing,			                                    //TextCodepage
                                        ApttusGlobals.oMissing,		                                        //TextVisualLayout
                                        ApttusGlobals.oMissing			                                    //Local
                                        );
                                    attachmentWorkbook.Close();
                                    base64String = Utils.FileToBase64String(attachmentFullName);
                                    Utils.DeleteFile(attachmentFullName);
                                    break;
                                case AttachmentFormat.PDF:
                                    attachmentWorkbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, attachmentFullName, Excel.XlFixedFormatQuality.xlQualityStandard);
                                    attachmentWorkbook.Saved = true;
                                    attachmentWorkbook.Close();
                                    base64String = Utils.FileToBase64String(attachmentFullName);
                                    Utils.DeleteFile(attachmentFullName);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Save Attachment Action");

                        }

                        finally
                        {
                            ExcelHelper.ExcelApp.DisplayAlerts = true;
                        }
                    }
                    else
                    {
                        // Save the Active 
                        bool bVal = false;
                        ApttusGlobals.blnExecute = true;
                        WorkbookEventManager.GetInstance.Application_WorkbookBeforeSave(ExcelHelper.ExcelApp.ActiveWorkbook, false, ref bVal);

                        switch (Model.AttachmentFormat)
                        {
                            case AttachmentFormat.Excel:
                                base64String = ExcelHelper.SaveACopyAndEncodeWorkbook(ExcelHelper.ExcelApp.ActiveWorkbook, attachmentFullName);
                                break;
                            case AttachmentFormat.PDF:
                                ExcelHelper.ExcelApp.ActiveWorkbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, attachmentFullName, Excel.XlFixedFormatQuality.xlQualityStandard);
                                base64String = Utils.FileToBase64String(attachmentFullName);
                                Utils.DeleteFile(attachmentFullName);
                                ExcelHelper.ExcelApp.ActiveWorkbook.ActiveSheet.DisplayPageBreaks = false;
                                break;
                        }
                    }

                    // 4. Create Save Record
                    ApttusSaveRecord AttachmentRecord = new ApttusSaveRecord
                    {
                        OperationType = QueryTypes.INSERT,
                        HasAttachment = true,
                        Attachments = new List<ApttusSaveAttachment>() {
                            new ApttusSaveAttachment {
                                AttachmentName = attachmentName + extension,
                                Base64EncodedString = base64String,
                                ParentId = recordId,
                                ObjectId = ApplicationDefinitionManager.GetInstance.GetObjectIdFromAppObjectId(Model.AppObjectId)
                            }
                        }
                    };
                    SaveRecords.Add(AttachmentRecord);

                    // 5. Execute Insert Attachment
                    objectManager.Insert(SaveRecords, false);

                    Result.Status = ActionResultStatus.Success;
                    ApttusGlobals.blnExecute = false;
                }
                else
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("SAVEATTCTL_NoRecord_ErrorMsg"), resourceManager.GetResource("SAVEATTCTL_NoRecordCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                    ApttusGlobals.blnExecute = false;

                }

                return Result;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Save Attachment Action");
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }
        }

    }
}
