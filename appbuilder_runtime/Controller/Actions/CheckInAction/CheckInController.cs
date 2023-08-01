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
    public class CheckInController
    {
        // Model
        private CheckInModel Model;
        public bool InputData { get; set; }
        public string[] InputDataName { get; set; }
        private string attachmentId = string.Empty;
        private string attachmentName = string.Empty;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public ActionResult Result { get; private set; }
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        ObjectManager objectManager = ObjectManager.GetInstance;
        private ICheckInProvider checkInProvider;

        public CheckInController(CheckInModel model, string[] inputDataName)
        {
            Model = model;
            InputDataName = inputDataName;
            checkInProvider = Globals.ThisAddIn.GetCheckInProvider();
            Result = new ActionResult();
        }

        public ActionResult Execute()
        {
            try
            {
                Result.Status = ActionResultStatus.Pending_Execution;

                Result = AddFileAsAttachment();
                if (Result.Status == ActionResultStatus.Failure)
                    return Result;

                Result = CheckInFile();
                if (Result.Status == ActionResultStatus.Failure)
                    return Result;

                Result = RemoveLockStatus();
                if (Result.Status == ActionResultStatus.Failure)
                    return Result;

                return Result;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                Result.Status = ActionResultStatus.Failure;
                ApttusMessageUtil.ShowError(ex.Message.ToString(), resourceManager.GetResource("CHECKINCTL_Execute_InfoMsg"));
                return Result;
            }
        }

        internal ActionResult AddFileAsAttachment()
        {
            DataManager manager = DataManager.GetInstance;
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
            ApttusDataSet objectData = null;
            string objectId = string.Empty;

            // Get input data set from datamanager
            objectData = manager.GetDatasetsByNames(InputDataName).FirstOrDefault();

            string recordId = string.Empty;

            if (objectData != null)
            {
                ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(objectData.AppObjectUniqueID);
                recordId = objectData.DataTable.Rows[0][appObject.IdAttribute].ToString();
                objectId = appObject.Id;
            }
            if (!string.IsNullOrEmpty(recordId))
            {
                //attachmentName = Model.FileName + Path.GetExtension(commandBar.GetActivWorkbookName());
                string checkInFileName = string.Empty;
                if (Globals.ThisAddIn.Application.ActiveWorkbook.Name.Contains("Runtime"))
                {
                    if (!string.IsNullOrEmpty(Model.FileName)) //filename set manually
                    {
                        checkInFileName = Model.FileName;
                    }
                }
                else
                {
                    checkInFileName = Globals.ThisAddIn.Application.ActiveWorkbook.Name.Substring(0, Globals.ThisAddIn.Application.ActiveWorkbook.Name.LastIndexOf("."));
                }
                SaveAttachmentFileNameView dlg = new SaveAttachmentFileNameView(checkInFileName, true);

                if (string.IsNullOrEmpty(Model.FileName))//filename is set by cell reference or runtime file name, skip dialog
                {
                    if (!string.IsNullOrEmpty(Model.namedRange)) //use cell reference
                    {
                        string cellRef = "";
                        try
                        {
                            var range = ExcelHelper.GetRange(Model.namedRange);
                            attachmentName = Convert.ToString(range.get_Value());
                            cellRef = range.Worksheet.Name + "!" + range.get_Address();
                        }
                        catch (Exception e) //if cell ref does not exist, log error and set attachmentName to empty to enter next if statement
                        {
                            cellRef = "";
                            attachmentName = "";
                            ExceptionLogHelper.ErrorLog("cell ref does not exist");
                        }
                        if (string.IsNullOrEmpty(attachmentName)) //if empty cell reference
                        {
                            Result.Status = ActionResultStatus.Failure;
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CHECKINCTL_NoAttachmentFound_Msg"), cellRef), resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg2"));
                            return Result;
                        }
                        else if (attachmentName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) //if invalid characters in cell
                        {
                            string tempAttachmentName = Utils.ReplaceInvalidChrs(attachmentName);
                            ExceptionLogHelper.ErrorLog("Invalid file name, replacing " + attachmentName + " with " + tempAttachmentName);
                            attachmentName = tempAttachmentName;
                            //Result.Status = ActionResultStatus.Failure;
                            //ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CHECKINCTL_InvalidAtt_Msg"), cellRef), resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg2"));
                            //return Result;
                        }
                        attachmentName = attachmentName + Path.GetExtension(commandBar.GetActivWorkbookName());
                    }
                    else //use runtime file name
                    {
                        attachmentName = Globals.ThisAddIn.Application.ActiveWorkbook.Name;
                    }
                }
                else //file name is to be set manually
                {
                    dlg.ShowDialog();
                    if (string.IsNullOrEmpty(dlg.FileName))
                    {
                        Result.Status = ActionResultStatus.Failure;
                        return Result;
                    }
                    attachmentName = dlg.FileName + Path.GetExtension(commandBar.GetActivWorkbookName());
                }

                // if Attachment filename is > 80 chars, trim it.
                // App Files object uses standard Name field to store file names, by default Name field can have length of 80 chars only (SFDC limit)
                // So no other option but to trim file name here.
                if (attachmentName.Length > 80)
                {
                    string fileExtension = Path.GetExtension(attachmentName);
                    string tempAttachmentName = attachmentName.Substring(0, 75);
                    attachmentName = tempAttachmentName + fileExtension;
                }

                string attachmentFullName = Utils.GetTempFileName(configurationManager.Definition.UniqueId, attachmentName);


                ApttusSaveRecord AttachmentRecord = new ApttusSaveRecord
                {
                    OperationType = QueryTypes.INSERT,
                    HasAttachment = true
                };

                //Globals.ThisAddIn.Application.ActiveWorkbook.Save();
                bool bVal = false;
                ApttusGlobals.blnExecute = true;
                WorkbookEventManager.GetInstance.Application_WorkbookBeforeSave(Globals.ThisAddIn.Application.ActiveWorkbook, false, ref bVal);

                AttachmentRecord.Attachments = new List<ApttusSaveAttachment>
                    {
                        new ApttusSaveAttachment{
                            AttachmentName = attachmentName,
                            Base64EncodedString = ExcelHelper.SaveACopyAndEncodeWorkbook(Globals.ThisAddIn.Application.ActiveWorkbook, attachmentFullName, false),
                            ParentId = recordId,
                            ObjectId = objectId
                        }
                    };

                SaveRecords.Add(AttachmentRecord);
                objectManager.Insert(SaveRecords, false);

                if (!string.IsNullOrEmpty(AttachmentRecord.RecordId))
                {
                    attachmentId = AttachmentRecord.RecordId;
                    Result.Status = ActionResultStatus.Success;
                }
                else
                {
                    ApttusMessageUtil.ShowError(AttachmentRecord.ErrorMessage, resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                }
                ApttusGlobals.blnExecute = false;
            }
            else
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("CHECKINCTL_NoAttachment_ErrorMsg"), resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
                ApttusGlobals.blnExecute = false;
            }

            return Result;
        }

        internal ActionResult CheckInFile()
        {
            DataManager manager = DataManager.GetInstance;
            // Get input data set from datamanager
            ApttusDataSet objectData = manager.GetDatasetsByNames(InputDataName).FirstOrDefault();
            ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(objectData.AppObjectUniqueID);
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();

            ApttusSaveRecord CustomCheckInRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField>(),
                ObjectName = checkInProvider.GetObjectName(),
                RecordRowNo = 0,
                RecordColumnNo = -1,
                ObjectType = ObjectType.Independent,
                OperationType = QueryTypes.INSERT
            };

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.GetNameAttribute(),
                Value = attachmentName,
                DataType = Datatype.String
            });

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.GetParentObjectName(),
                Value = ApplicationDefinitionManager.GetInstance.GetAppObject(objectData.AppObjectUniqueID).Id,
                DataType = Datatype.String
            });

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.GetFileId(),
                Value = attachmentId,
                DataType = Datatype.String
            });

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.GetFileType(),
                Value = "Notes&Attachments",
                DataType = Datatype.String
            });

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.IsFileLocked(),
                Value = "false",
                DataType = checkInProvider.GetFileLockedDataType()
            });

            CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.GetParentRecordId(),
                Value = objectData.DataTable.Rows[0][appObject.IdAttribute].ToString(),
                DataType = Datatype.String
            });

            //CustomCheckInRecord.SaveFields.Add(new ApttusSaveField
            //{
            //    FieldId = "UserID__c",
            //    Value = Globals.ThisAddIn.userInfo.UserId,
            //    DataType = Datatype.String
            //});

            SaveRecords.Add(CustomCheckInRecord);

            objectManager.Insert(SaveRecords, false);

            if (!string.IsNullOrEmpty(CustomCheckInRecord.RecordId))
                Result.Status = ActionResultStatus.Success;
            else
            {
                ApttusMessageUtil.ShowError(CustomCheckInRecord.ErrorMessage, resourceManager.GetResource("CHECKINCTL_NoAttachmentCap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
            }
            return Result;
        }

        internal ActionResult RemoveLockStatus()
        {
            string appFileId = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_CHECKOUT_FILE_ID);

            if (string.IsNullOrEmpty(appFileId))
            {
                Result.Status = ActionResultStatus.Success;
                return Result;
            }

            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();

            ApttusSaveRecord updateFileLockedRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField>(),
                ObjectName = checkInProvider.GetObjectName(),
                RecordRowNo = 0,
                RecordColumnNo = -1,
                RecordId = appFileId,
                ObjectType = ObjectType.Independent,
                OperationType = QueryTypes.UPDATE
            };

            //updateFileLockedRecord.SaveFields.Add(new ApttusSaveField
            //{
            //    FieldId = checkInProvider.GetIDAttribute(),
            //    Value = appFileId,
            //    DataType = Datatype.String
            //});

            updateFileLockedRecord.SaveFields.Add(new ApttusSaveField
            {
                FieldId = checkInProvider.IsFileLocked(),
                Value = "false",
                DataType = checkInProvider.GetFileLockedDataType()
            });

            SaveRecords.Add(updateFileLockedRecord);

            objectManager.Update(SaveRecords, false);

            ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_CHECKOUT_FILE_ID);

            Result.Status = ActionResultStatus.Success;
            return Result;
        }

    }
}
