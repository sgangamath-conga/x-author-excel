using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Apttus.XAuthor.Core;
using System.IO;

namespace Apttus.XAuthor.AppRuntime
{
    class AttachmentsDataManager
    {
        private static AttachmentsDataManager instance;
        private static object syncRoot = new Object();
        ObjectManager objectManager = ObjectManager.GetInstance;
        DataManager dataManager = DataManager.GetInstance;
        DataTable attachmentDataTable;
        ApplicationDefinitionManager appdefManager = ApplicationDefinitionManager.GetInstance;
        private static string PARENT_ID = "ParentId";
        private static string ATTACHMENT_PATH = "AttachmentPath";
        private static string OBJECT_TYPE = "Type";

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;

        public static AttachmentsDataManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new AttachmentsDataManager();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private AttachmentsDataManager()
        {
            CreateAttachmentsDataTable();
        }

        ///// <summary>
        ///// Clear Attachment Data
        ///// </summary>
        //public void ClearAttachmentData()
        //{
        //    // Clear Attachment data
        //    if (attachmentDataTable.Rows.Count > 0)
        //        attachmentDataTable.Clear();            
        //}

        /// <summary>
        /// Create Attachment Table
        /// </summary>
        private void CreateAttachmentsDataTable()
        {
            attachmentDataTable = new DataTable("AttachmentDataTable");

            DataColumn idColumn = new DataColumn(Constants.ID_ATTRIBUTE, typeof(string));
            attachmentDataTable.Columns.Add(idColumn);

            DataColumn parentIdColumn = new DataColumn(PARENT_ID, typeof(string));
            attachmentDataTable.Columns.Add(parentIdColumn);

            DataColumn nameColumn = new DataColumn(Constants.NAME_ATTRIBUTE, typeof(string));
            attachmentDataTable.Columns.Add(nameColumn);

            DataColumn attachmentPathColumn = new DataColumn(ATTACHMENT_PATH, typeof(string));
            attachmentDataTable.Columns.Add(attachmentPathColumn);

            DataColumn typeColumn = new DataColumn(OBJECT_TYPE, typeof(System.String));
            attachmentDataTable.Columns.Add(typeColumn);

            attachmentDataTable.PrimaryKey = new DataColumn[] { idColumn };
        }

        /// <summary>
        /// Render all attachments for data migration
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="type"></param>
        /// <param name="dsAttachment"></param>
        /// <param name="noOfAttachmentsRemaining">No of Attachments remaining to be processed.</param>
        public void GetAttachments(ApttusDataSet ds)
        {
            string parentIDs = string.Empty;
            int MaxAttachmentRecords = 200;

            string objectId = appdefManager.GetObjectIdFromAppObjectId(ds.AppObjectUniqueID);

            if (ds != null && ds.DataTable != null && ds.DataTable.Rows.Count > 0)
            {
                List<string> recordIds = new List<string>();

                //Bring all recordIds in Memory.
                foreach (DataRow row in ds.DataTable.Rows)
                    recordIds.Add(string.Format("'{0}'", Convert.ToString(row[Constants.ID_ATTRIBUTE])));

                int noOfChunks = ds.DataTable.Rows.Count / MaxAttachmentRecords;
                for (int i = 0; i < noOfChunks; ++i)
                {
                    List<string> rows = recordIds.GetRange(i * MaxAttachmentRecords, MaxAttachmentRecords);
                    parentIDs = string.Join(Constants.COMMA, rows);
                    ChunkDataSet(parentIDs, objectId);
                }

                int nRemainingRecords = ds.DataTable.Rows.Count % MaxAttachmentRecords;
                if (nRemainingRecords != 0)
                {
                    List<string> rows = recordIds.GetRange(noOfChunks * MaxAttachmentRecords, nRemainingRecords);
                    parentIDs = string.Join(Constants.COMMA, rows);
                    ChunkDataSet(parentIDs, objectId);
                }

                recordIds.Clear();
                recordIds = null;
            }
        }

        /// <summary>
        /// Chunk Dataset because of maximum query limit
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="parentIDs"></param>
        /// <param name="dsAttachment"></param>
        /// <param name="type"></param>
        private void ChunkDataSet(string parentIDs, string objectId)
        {
            ApttusDataSet tempDataSet = objectManager.GetAttachmentsDataSet(parentIDs, objectId);
            if (tempDataSet != null && tempDataSet.DataTable != null && tempDataSet.DataTable.Rows.Count > 0)
            {
                DataColumn dcAttachmentPath = new DataColumn("AttachmentPath", typeof(System.String));
                tempDataSet.DataTable.Columns.Add(dcAttachmentPath);

                foreach (DataRow dr in tempDataSet.DataTable.Rows)
                {
                    string Id = dr[Constants.ID_ATTRIBUTE].ToString();
                    string fileName = dr[Constants.NAME_ATTRIBUTE].ToString();
                    // Replace Invalid Windows File Path Characters
                    fileName = string.Join("", fileName.Split(Path.GetInvalidFileNameChars()));
                    string base64Data = dr["Body"].ToString();
                    string parentID = dr[PARENT_ID].ToString();
                    string parentFolderName = string.Empty;
                    string parentName = string.Empty;
                    string fileNameExtension = Path.GetExtension(fileName);
                    // ABC Foods__<id>
                    parentName = dr["Parent.Name"].ToString();
                    // Replace Invalid Windows File Path Characters
                    parentName = string.Join("", parentName.Split(Path.GetInvalidFileNameChars()));
                    parentFolderName = parentName + Constants.ATTACHMENTSEPARATOR + parentID;
                    // Filename__id.extension (double underscore)

                    //string filePath = Utils.GetTempFileName(parentFolderName, Path.GetFileNameWithoutExtension(fileName) + Constants.ATTACHMENTSEPARATOR + Id + fileNameExtension, true);
                    string filePath = Utils.GetTempFileName(parentFolderName, Path.GetFileNameWithoutExtension(fileName) + fileNameExtension, true, true);
                    string newFilePath = filePath;
                    int count = 1;
                    while (File.Exists(newFilePath))
                    {
                        string tempFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(fileName), count++);
                        newFilePath = Utils.GetTempFileName(parentFolderName, tempFileName + fileNameExtension, true, true);
                    }

                    Utils.StreamToFile(Convert.FromBase64String(base64Data), newFilePath, true);

                    dr[dcAttachmentPath] = filePath;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSaveMap"></param>
        /// <param name="currentSaveGroup"></param>
        /// <returns></returns>
        public SaveField IsAttachmentFieldPartOfSaveMap(SaveMap currentSaveMap, SaveGroup currentSaveGroup)
        {
            SaveField attachmentField = (from sf in currentSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(currentSaveGroup.GroupId))
                                         where appdefManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Attachment
                                         select sf).FirstOrDefault();

            return attachmentField;
        }

        /// <summary>
        /// Create one attachment record at the time of Add row for each corresponding Object record.
        /// In case Object record has more than one Attachment, newer entries will be created in AtachmentDataset in MarkAttachmentRecordDirty()
        /// </summary>
        /// <param name="dr"></param>
        public string CreateAttachmentRecordID(SaveField attachmentSaveField, DataRow dr, ObjectType objectType)
        {
            string newAttachmentGuid = string.Empty;
            if (attachmentSaveField != null)
            {
                newAttachmentGuid = Guid.NewGuid().ToString();
                if (attachmentSaveField.SaveFieldType != SaveType.SaveOnlyField)
                    dr[Constants.ATTACHMENT] = newAttachmentGuid;

                DataRow row = attachmentDataTable.NewRow();

                row[Constants.ID_ATTRIBUTE] = newAttachmentGuid;
                row[PARENT_ID] = dr[Constants.ID_ATTRIBUTE];
                row[ATTACHMENT_PATH] = string.Empty;
                row[OBJECT_TYPE] = objectType.ToString();
                row[Constants.NAME_ATTRIBUTE] = string.Empty;

                attachmentDataTable.Rows.Add(row);
                attachmentDataTable.AcceptChanges();
            }
            return newAttachmentGuid;
        }


        /// <summary>
        /// Create one attachment record at the time of Add row for each corresponding Object record.
        /// In case Object record has more than one Attachment, newer entries will be created in AtachmentDataset in MarkAttachmentRecordDirty()
        /// </summary>
        /// <param name="dr"></param>
        public void CreateRetrieveAttachmentGuid(Guid guid, DataRow dr, ObjectType objectType)
        {
            dr[Constants.ATTACHMENT] = guid;

            DataRow row = attachmentDataTable.NewRow();

            row[Constants.ID_ATTRIBUTE] = guid;
            row[PARENT_ID] = dr[Constants.ID_ATTRIBUTE];
            row[ATTACHMENT_PATH] = string.Empty;
            row[OBJECT_TYPE] = objectType;
            row[Constants.NAME_ATTRIBUTE] = string.Empty;
            attachmentDataTable.AcceptChanges();

            attachmentDataTable.Rows.Add(row);
        }

        private void RemoveSpaceFromAttachmentPath(ref string[] changeAttachments, ref string[] sourceAttachments)
        {
            for (int i = 0; i < changeAttachments.Length; ++i)
                changeAttachments[i] = changeAttachments[i].Trim();

            for (int i = 0; i < sourceAttachments.Length; ++i)
                sourceAttachments[i] = sourceAttachments[i].Trim();
        }

        // Add attachments in existing attachment columns with '|'
        private void InsertAttachmentRecords(string sourceValue, string changeValue, ObjectType objectType, ApttusSaveRecord SaveRecord, out bool IsAttachmentDirty, long salesforceFileSizeUploadLimit, string recordId = "")
        {
            IsAttachmentDirty = false;

            if (!string.IsNullOrEmpty(changeValue))
            {
                string[] changeAttachments = changeValue.Split('|');
                string[] sourceAttachments = sourceValue.Split('|');

                RemoveSpaceFromAttachmentPath(ref changeAttachments, ref sourceAttachments);

                string[] finalResults = changeAttachments.Except(sourceAttachments).ToArray();

                for (int i = 0; i < finalResults.Length; ++i)
                {
                    if (File.Exists(finalResults[i]))
                    {
                        //If the file being attached is more than the salesforce configured file size limit, then reject that file.
                        FileInfo fileInfo = new FileInfo(finalResults[i]);
                        if (fileInfo.Length <= salesforceFileSizeUploadLimit)
                        {
                            IsAttachmentDirty = true;
                            DataRow row = attachmentDataTable.NewRow();

                            string attachmentId = Guid.NewGuid().ToString();
                            row[PARENT_ID] = recordId;
                            row[OBJECT_TYPE] = objectType;
                            row[Constants.ID_ATTRIBUTE] = attachmentId;
                            row[ATTACHMENT_PATH] = finalResults[i];
                            row[Constants.NAME_ATTRIBUTE] = Path.GetFileName(finalResults[i]);

                            attachmentDataTable.Rows.Add(row);

                            SaveRecord.AttachmentIds.Add(attachmentId);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaveRecord"></param>
        /// <param name="sourceValue"></param>
        /// <param name="changeValue"></param>
        public void MarkAttachmentRecordDirty(ApttusSaveRecord SaveRecord, string sourceValue, string changeValue, ObjectType objectType, out bool IsAttachmentDirty, string sourceId = "")
        {
            long salesforceFileSizeUploadLimit = GetSalesforceFileSizeUploadLimit();

            IsAttachmentDirty = false;
            Guid result;
            List<ApttusSaveRecord> AttachmentRecords = new List<ApttusSaveRecord>();

            if (Guid.TryParse(sourceValue, out result)) //this will only be executed if 
            {
                string[] attachments = changeValue.Split('|');

                DataRow row = attachmentDataTable.Select(Constants.ID_ATTRIBUTE + "= '" + sourceValue + "'").FirstOrDefault();

                bool bValueSet = false;
                string attachmentId = string.Empty;
                for (int cnt = 0; cnt < attachments.Length; cnt++)
                {
                    if (System.IO.File.Exists(attachments[cnt]))
                    {
                        //If the file being attached is more than the salesforce configured file size limit, then reject that file.
                        FileInfo fileInfo = new FileInfo(attachments[cnt]);
                        if (fileInfo.Length <= salesforceFileSizeUploadLimit)
                        {
                            IsAttachmentDirty = true;

                            if (row != null && !bValueSet)
                            {
                                bValueSet = true;
                                attachmentId = row[Constants.ID_ATTRIBUTE] as string;
                                row[ATTACHMENT_PATH] = attachments[cnt];
                                row[Constants.NAME_ATTRIBUTE] = Path.GetFileName(attachments[cnt]);
                            }
                            else
                            {
                                attachmentId = Guid.NewGuid().ToString();
                                DataRow newrow = attachmentDataTable.NewRow();
                                newrow[PARENT_ID] = sourceId;
                                newrow[OBJECT_TYPE] = objectType;
                                newrow[Constants.ID_ATTRIBUTE] = attachmentId;
                                newrow[ATTACHMENT_PATH] = attachments[cnt];
                                newrow[Constants.NAME_ATTRIBUTE] = Path.GetFileName(attachments[cnt]);
                                attachmentDataTable.Rows.Add(newrow);
                            }
                            SaveRecord.AttachmentIds.Add(attachmentId);
                        }
                    }
                }
            }

            // Add attachments in existing attachment columns with '|' 
            if (!IsAttachmentDirty)
                InsertAttachmentRecords(sourceValue, changeValue, objectType, SaveRecord, out IsAttachmentDirty, salesforceFileSizeUploadLimit, sourceId);
        }

        /// <summary>
        /// Remove records from table and physical file system
        /// </summary>
        /// <param name="parentId"></param>
        internal void RemoveRecord(ApttusDataSet dataSet, string parentId)
        {
            try
            {
                DataRow[] rows = attachmentDataTable.Select(PARENT_ID + "= '" + parentId + "'");

                DataRow[] rowsInDataTable = dataSet.DataTable.Select(Constants.ID_ATTRIBUTE + "= '" + parentId + "'");

                //Remove row if exists from attachmentDataTable
                if (rows != null && rows.Length > 0)
                {
                    attachmentDataTable.Rows.Remove(rows[0]);
                    attachmentDataTable.AcceptChanges();
                }
                //Remove the files from the local directory. We can get the local files which are stored in DataManager's DataSet.
                if (!string.IsNullOrEmpty(parentId) && rowsInDataTable != null && rowsInDataTable.Length > 0 && dataSet.DataTable.Columns.Contains(Constants.ATTACHMENT))
                {
                    string filePaths = Convert.ToString(rowsInDataTable[0][Constants.ATTACHMENT]);
                    if (string.IsNullOrEmpty(filePaths))
                        return;

                    string[] files = filePaths.Split('|');
                    if (files != null && files.Count() > 0)
                    {
                        string filePath = files[0];

                        if (Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Utils.DeleteDirectory(Path.GetDirectoryName(filePath));
                        }
                        //foreach (string filePath in files)
                        //{
                        //    if (Directory.Exists(Path.GetDirectoryName(filePath)))
                        //    {                                
                        //        string folderPath = Path.GetDirectoryName(filePath);
                        //        Utils.DeleteDirectory(folderPath);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Save Attachments for data migration
        /// </summary>
        /// <param name="saveRecords"></param>
        /// <param name="SaveFields"></param>
        internal List<ApttusSaveRecord> CreateAttachmentRecords(List<ApttusSaveRecord> ObjectRecords, long salesforceFileSizeUploadLimit, ref int startSaveRecordChunkingAt, ref int startAttachmentChunkingAt)
        {
            List<ApttusSaveRecord> attachmentSaveRecords = new List<ApttusSaveRecord>();

            int iSaveRecordIndex = 0;

            long lSalesforceRequestSizeLimit = salesforceFileSizeUploadLimit + 1;
            long lChunkRequestSize = 0;
            int nAtttachmentRecordsCount = ObjectRecords.Count;
            bool bChunkingExceededLimit = false;

            for (iSaveRecordIndex = startSaveRecordChunkingAt; iSaveRecordIndex < nAtttachmentRecordsCount; ++iSaveRecordIndex)
            {
                ApttusSaveRecord saveRecord = ObjectRecords[iSaveRecordIndex];
                int nPerRecordAttachmentCount = saveRecord.AttachmentIds.Count;

                for (int iAttachmentIndex = startAttachmentChunkingAt; iAttachmentIndex < nPerRecordAttachmentCount; ++iAttachmentIndex)
                {
                    string attachmentId = saveRecord.AttachmentIds[iAttachmentIndex];

                    DataRow[] rows = attachmentDataTable.Select(Constants.ID_ATTRIBUTE + "= '" + attachmentId + "'");

                    foreach (DataRow row in rows)
                    {
                        string filePath = row[ATTACHMENT_PATH] as string;

                        //Get file size
                        long lFileSize = new FileInfo(filePath).Length;
                        lChunkRequestSize += lFileSize;

                        //Test whether all the attachments created till now has exceeded the salesforce request size limit.
                        if (lChunkRequestSize > lSalesforceRequestSizeLimit)
                        {
                            bChunkingExceededLimit = true;
                            startAttachmentChunkingAt = saveRecord.AttachmentIds.IndexOf(attachmentId);
                            break;
                        }

                        row[PARENT_ID] = saveRecord.RecordId;

                        ApttusSaveRecord AttachmentRecord = new ApttusSaveRecord
                        {
                            OperationType = QueryTypes.INSERT,
                            HasAttachment = true,
                            ApttusDataSetId = saveRecord.ApttusDataSetId,
                            RecordRowNo = saveRecord.RecordRowNo,
                            ObjectName = Constants.ATTACHMENT,
                            SaveFields = new List<ApttusSaveField>()
                        };

                        AttachmentRecord.AttachmentIds.Add(row[Constants.ID_ATTRIBUTE] as string);

                        AttachmentRecord.Attachments = new List<ApttusSaveAttachment>();

                        AttachmentRecord.Attachments.Add(

                            new ApttusSaveAttachment
                            {
                                AttachmentName = Path.GetFileName(filePath),
                                Base64EncodedString = ExcelHelper.EncodeFileToString(filePath),
                                ParentId = saveRecord.RecordId,
                                ObjectId = saveRecord.ObjectName
                            }
                        );

                        attachmentSaveRecords.Add(AttachmentRecord);
                    }
                    if (bChunkingExceededLimit)
                        break;
                }
                if (bChunkingExceededLimit)
                    break;
                startAttachmentChunkingAt = 0; //Reset the attachment chunking index.
            }
            startSaveRecordChunkingAt = iSaveRecordIndex;
            return attachmentSaveRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSetId"></param>
        internal void ProcessSuccessAttachmentRecords(Guid DataSetId, IEnumerable<ApttusSaveRecord> SuccessRecords)
        {
            ApttusDataSet dataSet = dataManager.GetDataByDataSetId(DataSetId);
            if (dataSet != null)
            {
                foreach (ApttusSaveRecord record in SuccessRecords)
                {
                    foreach (string attachmentId in record.AttachmentIds)
                    {
                        DataRow[] attachmentDataTableRows = attachmentDataTable.Select(Constants.ID_ATTRIBUTE + " ='" + attachmentId + "'");
                        if (attachmentDataTableRows.Count() > 0)
                        {
                            DataRow[] repeatingDataTableRows = dataSet.DataTable.Select(Constants.ID_ATTRIBUTE + " ='" + record.Attachments[0].ParentId + "'");
                            if (repeatingDataTableRows.Count() == 1)
                            {
                                DataRow repeatingDataTableRow = repeatingDataTableRows[0];
                                string attachment = repeatingDataTableRow[Constants.ATTACHMENT] as string;

                                foreach (DataRow attachmentDataTableRow in attachmentDataTableRows)
                                {
                                    Guid attachmentID;
                                    if (Guid.TryParse(attachment, out attachmentID))
                                        attachment = attachmentDataTableRow[ATTACHMENT_PATH] as string;
                                    else
                                        attachment = attachment + " | " + (attachmentDataTableRow[ATTACHMENT_PATH] as string);

                                    attachmentDataTable.Rows.Remove(attachmentDataTableRow);
                                }

                                repeatingDataTableRow[Constants.ATTACHMENT] = attachment;

                                repeatingDataTableRow.AcceptChanges();

                            }
                        }
                    }
                }
            }
            attachmentDataTable.AcceptChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSetId"></param>
        internal StringBuilder ProcessFailureAttachmentRecords(Guid DataSetId, List<ApttusSaveRecord> ErrorAttachmentRecords)
        {
            StringBuilder errorMessage = new StringBuilder(100);
            //If attachment records fails we don't need to reject the changes from our DataManager's datatable because it is not even updated with the changed value.
            //It only gets updated, if the attachment succeeds.
            foreach (ApttusSaveRecord ErrorRecord in ErrorAttachmentRecords)
            {
                int repeatingGridRowStart = 0;
                if (ErrorRecord.ObjectType == ObjectType.Repeating)
                {
                    // Find the starting row number for the error record to display to the user.
                    ApttusDataTracker repeatingDataTracker = (from dt in dataManager.AppDataTracker
                                                              where dt.DataSetId.Equals(ErrorRecord.ApttusDataSetId) && dt.Type == ObjectType.Repeating
                                                              select dt).FirstOrDefault();
                    if (repeatingDataTracker != null)
                        repeatingGridRowStart = ExcelHelper.GetRange(repeatingDataTracker.Location).Row + 2;
                }
                int errorRecordRow = ErrorRecord.RecordRowNo + repeatingGridRowStart;
                errorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsInsertError_ErrMsg"), errorRecordRow.ToString(), ErrorRecord.ErrorMessage));

                foreach (string attachmentId in ErrorRecord.AttachmentIds)
                {
                    DataRow[] rows = attachmentDataTable.Select(Constants.ID_ATTRIBUTE + " = '" + attachmentId + "'");
                    foreach (DataRow row in rows)
                        attachmentDataTable.Rows.Remove(row);
                }
            }
            return errorMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttachmentRecords"></param>
        /// <param name="bEnablePartialSave"></param>
        /// <param name="waitWindow"></param>
        /// <returns></returns>
        internal List<ApttusSaveRecord> ProcessAttachments(List<ApttusSaveRecord> AttachmentRecords, bool bEnablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            int startRecordChunkingAt = 0;
            int startAttachmentChunkingAt = 0;
            int processedRecordCount = 0;
            int nAttachmentRecordCount = 0;
            AttachmentRecords.ForEach(rec => nAttachmentRecordCount += rec.AttachmentIds.Count);

            long salesforceFileSizeUploadLimit = GetSalesforceFileSizeUploadLimit();
            List<ApttusSaveRecord> ProcessedAttachments = new List<ApttusSaveRecord>();
            ChunkAndExecuteAttachments(AttachmentRecords, bEnablePartialSave, BatchSize, salesforceFileSizeUploadLimit, waitWindow, ref startRecordChunkingAt, ref startAttachmentChunkingAt, ref ProcessedAttachments, processedRecordCount, nAttachmentRecordCount.ToString());
            return ProcessedAttachments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttachmentRecords"></param>
        /// <param name="bEnablePartialSave"></param>
        /// <param name="salesforceFileSizeUploadLimit"></param>
        /// <param name="waitWindow"></param>
        /// <param name="startRecordChunkingAt"></param>
        /// <param name="startAttachmentChunkingAt"></param>
        /// <param name="ProcessedAttachments"></param>
        internal void ChunkAndExecuteAttachments(List<ApttusSaveRecord> AttachmentRecords, bool bEnablePartialSave, int BatchSize, long salesforceFileSizeUploadLimit, WaitWindowView waitWindow, ref int startRecordChunkingAt, ref int startAttachmentChunkingAt, ref List<ApttusSaveRecord> ProcessedAttachments, int processedRecordCount, string totalAttachmentRecords)
        {
            List<ApttusSaveRecord> chunkedAttachmentRecords = CreateAttachmentRecords(AttachmentRecords, salesforceFileSizeUploadLimit, ref startRecordChunkingAt, ref startAttachmentChunkingAt);
            if (chunkedAttachmentRecords.Count > 0)
            {
                processedRecordCount += chunkedAttachmentRecords.Count;
                waitWindow.Message = String.Format(resourceManager.GetResource("COREABSADAPTER_SetWaitMessage_Attachments_Msg"), QueryTypes.INSERT.ToString(), processedRecordCount.ToString(), totalAttachmentRecords,resourceManager.CRMName);
                objectManager.Insert(chunkedAttachmentRecords, bEnablePartialSave, BatchSize);

                ProcessedAttachments.AddRange(chunkedAttachmentRecords);

                if (startRecordChunkingAt != AttachmentRecords.Count)
                    ChunkAndExecuteAttachments(AttachmentRecords, bEnablePartialSave, BatchSize, salesforceFileSizeUploadLimit, waitWindow, ref startRecordChunkingAt, ref startAttachmentChunkingAt, ref ProcessedAttachments, processedRecordCount, totalAttachmentRecords);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private long GetSalesforceFileSizeUploadLimit()
        {
            long salesforceFileSizeUploadLimit = 25 * 1024 * 1024; //By default it is 25 MB.
            if (configManager.Application != null && configManager.Application.Definition != null && configManager.Application.Definition.AppSettings != null)
            {
                long fileSize = ((long)configManager.Definition.AppSettings.MaxAttachmentSize) * 1024 * 1024; //size in MB.
                if (fileSize != 0)
                    salesforceFileSizeUploadLimit = fileSize;
            }
            return salesforceFileSizeUploadLimit;
        }
    }
}
