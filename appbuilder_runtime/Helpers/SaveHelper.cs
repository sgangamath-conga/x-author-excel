/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class SaveHelper
    {
        static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        static DataManager dataManager = DataManager.GetInstance;
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static ObjectManager objectManager = ObjectManager.GetInstance;
        static FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        static CultureInfo currentCultureInfo = Utils.GetLatestCulture;
        static string[] DateFormats = { "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "M-d-yyyy", "M/d/yyyy h:mm:ss tt","yyyy-MM-dd",
                                   currentCultureInfo.DateTimeFormat.ShortDatePattern,
                                   currentCultureInfo.DateTimeFormat.ShortDatePattern + Constants.SPACE + currentCultureInfo.DateTimeFormat.LongTimePattern,
                                   "MM/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt",
                                   currentCultureInfo.DateTimeFormat.FullDateTimePattern};

#if DEBUG
        // Perfomance Test Code
        static StringBuilder timerLog = null;
        static Stopwatch timer = null;
#endif
        /// <summary>
        /// This constructor should be used when you dont want to save processed results to display save sucessive summarry later on.
        /// </summary>
        /// <param name="SaveRecords"></param>
        /// <param name="bDisplayMessage"></param>
        /// <param name="tm"></param>
        /// <param name="tl"></param>
        /// <returns></returns>

        public static ApttusSaveSummary ProcessResults(List<ApttusSaveRecord> SaveRecords, bool bDisplayMessage, Stopwatch tm = null, StringBuilder tl = null)
        {
            StringBuilder saveSummaryAcrossWorkFlow = new StringBuilder();
            return ProcessResults(SaveRecords, bDisplayMessage, ref saveSummaryAcrossWorkFlow);
        }

        /// <summary>
        /// This constructor should be used when you want to save processed results to display save sucessive summarry later on. 
        /// to do so you need to passed saveSummaryAcrossWorkFlow stringbuilder varible which can hold successive messages.
        /// </summary>
        /// <param name="SaveRecords"></param>
        /// <param name="bDisplayMessage"></param>
        /// <param name="saveSummaryAcrossWorkFlow"></param>
        /// <param name="tm"></param>
        /// <param name="tl"></param>
        /// <param name="suppressSaveMessage">suppressSaveMessage Configuration made at workflow level to decide shall savesuccess message displayed or not. it will be ignored
        /// if user have set suppressSaveMessage = true at application level.
        /// </param>
        /// <returns></returns>
        public static ApttusSaveSummary ProcessResults(List<ApttusSaveRecord> SaveRecords, bool bDisplayMessage, ref StringBuilder saveSummaryAcrossWorkFlow, Stopwatch tm = null, StringBuilder tl = null, bool suppressSaveMessage = false, bool EnableRowHighlightErrors = false, SaveMap model = null)
        {
#if DEBUG
            // Perfomance Test Code
            if (tm != null)
            {
                timer = tm;
                timerLog = tl;
                timer.Stop();
            }
            else
            {
                timer = new Stopwatch();
                timerLog = new StringBuilder();
                timerLog.AppendLine(string.Empty);
            }

            timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "** Begin Process Results **");
            timer.Start();
#endif

            ApttusSaveSummary SaveSummary = new ApttusSaveSummary();

            var SuccessRecords = from sr in SaveRecords where sr.Success select sr;
            var ErrorRecords = from sr in SaveRecords where !sr.Success select sr;

            //clear old highlighting if necessary
            bool IsHighlightingEnabled = EnableRowHighlightErrors;
            string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
            //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
            ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);
            bool rowsAdded = false;
            if (appInstance != null)
            {
                rowsAdded = appInstance.RowAdded;
                if (IsHighlightingEnabled && appInstance.RowHighLightingData == null)
                {
                    appInstance.RowHighLightingData = new RowHighLighting();
                }
                if (IsHighlightingEnabled && !appInstance.RowHighLightingData.isCleared)
                {
                    ClearErrorHighlighting(appInstance.RowHighLightingData, rowsAdded, model);
                    appInstance.RowHighLightingData.Clear(model.Id.ToString());
                    appInstance.RowHighLightingData.isCleared = true;
                }
            }

            // Handle Successful Records (exclude attachment records)
            var SuccessDataSetIds = (from sr in SuccessRecords where sr.HasAttachment == false select sr.ApttusDataSetId).Distinct();
            foreach (Guid DataSetId in SuccessDataSetIds)
            {
                DataTable RepeatingDatatable = null, CrossTabIDDatatable = null, matrixDataTable = null;
                ApttusCrossTabDataSet CrossTabDataSet = null;
                var CurrentSuccessRecords = (from sr in SuccessRecords where sr.ApttusDataSetId.Equals(DataSetId) && sr.HasAttachment == false select sr);

                // Initialize Repeating and CrossTab Datatables
                if (dataManager.GetDataByDataSetId(DataSetId) != null)
                    RepeatingDatatable = dataManager.GetDataByDataSetId(DataSetId).DataTable;
                else if (dataManager.GetCrossTabDataByDataSetId(DataSetId) != null)
                {
                    CrossTabDataSet = dataManager.GetCrossTabDataByDataSetId(DataSetId);
                    CrossTabIDDatatable = CrossTabDataSet.IDTable;
                }
                else if (dataManager.GetMatrixDataSetById(DataSetId) != null)
                    matrixDataTable = dataManager.GetMatrixDataSetById(DataSetId).MatrixDataTable;

                // Update RecordId back to Datatable in case of Insert or Upsert
                var SuccessInsertUpsertRecords = from sr in CurrentSuccessRecords where sr.OperationType == QueryTypes.INSERT || sr.OperationType == QueryTypes.UPSERT select sr;
                foreach (var SuccessInsertUpsertRecord in SuccessInsertUpsertRecords)
                {
                    if (SuccessInsertUpsertRecord.RecordColumnNo < 0)
                    {
                        // Repeating
                        if (RepeatingDatatable != null)
                        {
                            var apttusObject = applicationDefinitionManager.GetAppObject(SuccessInsertUpsertRecord.AppObject);
                            //we have duplicate colums for id attribute in source datatable, hence both the columns needs to be updated after insert/upsert record
                            RepeatingDatatable.Rows[SuccessInsertUpsertRecord.RecordRowNo][apttusObject.IdAttribute] = SuccessInsertUpsertRecord.RecordId;
                            RepeatingDatatable.Rows[SuccessInsertUpsertRecord.RecordRowNo][Constants.ID_ATTRIBUTE] = SuccessInsertUpsertRecord.RecordId;
                        }
                        else if (matrixDataTable != null)
                        {
                            // For MatrixMap, Targetnamedrange is re-purposed here, to stored record ID, which helps make decision in ProcessResult for Accept / Reject changes.
                            string guid = SuccessInsertUpsertRecord.TargetNameRange;
                            Guid result;
                            if (Guid.TryParse(guid, out result))
                            {
                                DataRow[] rows = matrixDataTable.Select(Constants.ID_ATTRIBUTE + "= '" + guid + "'");
                                if (rows != null && rows.Count() > 0)
                                {
                                    foreach (DataRow row in rows)
                                    {
                                        //Replace the Guid by actual salesforce record id
                                        row[Constants.ID_ATTRIBUTE] = SuccessInsertUpsertRecord.RecordId;
                                        row.AcceptChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                        // Cross Tab
                        CrossTabIDDatatable.Rows[SuccessInsertUpsertRecord.RecordRowNo][SuccessInsertUpsertRecord.RecordColumnNo] = SuccessInsertUpsertRecord.RecordId;
                }

                // Update RecordId back to the Excel in case of Insert (applies only to Independent and Repeating)
                if (SuccessInsertUpsertRecords.Count() > 0 && dataManager.GetDataByDataSetId(DataSetId) != null)
                    UpdateExcelWithInsertedIDs(DataSetId, SuccessInsertUpsertRecords.Where(ir => ir.RecordColumnNo < 0).ToList());

                // Accept changes
                if (RepeatingDatatable != null)
                {
                    // If the same object's Dataset has both success and error records, then accept changes at row level (instead of Datatable.AcceptChanges)
                    if (ErrorRecords.Any(er => er.ApttusDataSetId == CurrentSuccessRecords.FirstOrDefault().ApttusDataSetId))
                    {
                        foreach (var CurrentSuccessRecord in CurrentSuccessRecords)
                            RepeatingDatatable.Rows[CurrentSuccessRecord.RecordRowNo].AcceptChanges();
                    }
                    else
                    {
                        if (!ConfigurationManager.GetInstance.IsRichTextEditingDisabled) //If richtext editing is enabled.
                        {
                            //Check if there was a richtext field, if a richtext field is part of saverecords, then just remove them from richtext data table.
                            var recordIDsWithRichText = (from asr in SaveRecords.Where(rt => rt.ApttusDataSetId.Equals(DataSetId))
                                                         from sf in asr.SaveFields.Where(sf => sf.DataType == Datatype.Rich_Textarea)
                                                         select new { asr.RecordId, sf.FieldId, asr.RecordRowNo, sf.Value, asr.OperationType }).ToList();

                            if (recordIDsWithRichText.Count > 0)
                            {
                                bool bRecordsInserted = SuccessInsertUpsertRecords.Count() > 0;
                                foreach (var item in recordIDsWithRichText)
                                {
                                    DataRow dr = RepeatingDatatable.Rows[item.RecordRowNo];
                                    string id = string.Empty;
                                    if (bRecordsInserted && SuccessInsertUpsertRecords.Where(sr => sr.RecordId.Equals(item.RecordId)).Count() > 0) //inserted records
                                        id = dr[item.FieldId] as string;
                                    else //updated records
                                        id = dr[Constants.ID_ATTRIBUTE] as string;

                                    dr[item.FieldId] = item.Value;
                                    RichTextDataManager.Instance.RemoveRecord(id, item.FieldId);
                                }
                                var updateOperationRecordIDs = recordIDsWithRichText.Where(rid => rid.OperationType == QueryTypes.UPDATE).ToList();
                                if (updateOperationRecordIDs.Count > 0)
                                {
                                    var saveFields = (from asr in SaveRecords.Where(rt => rt.ApttusDataSetId.Equals(DataSetId))
                                                      from sf in asr.SaveFields.Where(sf => sf.DataType == Datatype.Rich_Textarea)
                                                      select sf).ToList();

                                    UpdateRichTextFieldInExcel(DataSetId, updateOperationRecordIDs, saveFields);
                                }
                            }
                        }
                        RepeatingDatatable.AcceptChanges();
                    }
                }
                else if (CrossTabIDDatatable != null)
                {
                    CrossTabIDDatatable.AcceptChanges();
                    CrossTabDataSet.DataTable.AcceptChanges();
                }
                else if (matrixDataTable != null)
                {
                    // If the same object's Dataset has both success and error records, then accept changes at row level (instead of Datatable.AcceptChanges)
                    if (ErrorRecords.Any(er => er.ApttusDataSetId == CurrentSuccessRecords.FirstOrDefault().ApttusDataSetId))
                    {
                        foreach (var CurrentSuccessRecord in CurrentSuccessRecords)
                        {
                            DataRow[] rows = matrixDataTable.Select(Constants.ID_ATTRIBUTE + "= '" + CurrentSuccessRecord.TargetNameRange + "'");
                            if (rows != null && rows.Count() > 0)
                            {
                                foreach (DataRow row in rows)
                                    row.AcceptChanges();
                            }
                        }
                    }
                    else
                        matrixDataTable.AcceptChanges();
                }
            }

            // Handle Successful Records (exclude attachment records)
            var SuccessAttachmentsDataSetIds = (from sr in SuccessRecords where sr.HasAttachment == true select sr.ApttusDataSetId).Distinct();
            foreach (Guid DataSetId in SuccessAttachmentsDataSetIds)
            {
                var successAttachmentRecordsPerDataSet = from sr in SuccessRecords
                                                         where sr.HasAttachment == true && sr.ApttusDataSetId == DataSetId
                                                         select sr;
                AttachmentsDataManager.GetInstance.ProcessSuccessAttachmentRecords(DataSetId, successAttachmentRecordsPerDataSet);
            }

            // Update Success Counts
            SaveSummary.InsertCount = SuccessRecords.Where(sr => sr.OperationType == QueryTypes.INSERT).Count();
            SaveSummary.UpdateCount = SuccessRecords.Where(sr => sr.OperationType == QueryTypes.UPDATE).Count();
            SaveSummary.UpsertCount = SuccessRecords.Where(sr => sr.OperationType == QueryTypes.UPSERT).Count();
            SaveSummary.DeleteCount = SuccessRecords.Where(sr => sr.OperationType == QueryTypes.DELETE).Count();

            /************************************************************************/
            /*************** H A N D L E   E R R O R    R E C O R D S ***************/
            /********* (E X C L U D E   A T T A C H M E N T   R E C O R D S) ********/
            /************************************************************************/
            var ErrorDatasetIds = (from er in ErrorRecords where er.HasAttachment == false select er.ApttusDataSetId).Distinct();

            SaveSummary.ErrorMessage = string.Empty;
            bool bCaptureFirstError = false;
            foreach (Guid ErrorDatasetId in ErrorDatasetIds)
            {
                var CurrentErrorRecords = (from er in ErrorRecords where er.ApttusDataSetId.Equals(ErrorDatasetId) select er);
                int repeatingGridRowStart = 0;

                // Reject changes
                if (dataManager.GetDataByDataSetId(ErrorDatasetId) != null)
                {
                    ApttusObject ErrorAppObject = applicationDefinitionManager.GetAppObject(dataManager.GetDataByDataSetId(ErrorDatasetId).AppObjectUniqueID);
                    DataTable ErrorDataTable = dataManager.GetDataByDataSetId(ErrorDatasetId).DataTable;
                    if (ErrorAppObject.ObjectType == ObjectType.Independent)
                        // Reject changes for Independent
                        ErrorDataTable.RejectChanges();
                    else if (ErrorAppObject.ObjectType == ObjectType.Repeating)
                    {
                        // Find the starting row number for the error record to display to the user.
                        ApttusDataTracker repeatingDataTracker = (from dt in dataManager.AppDataTracker
                                                                  where dt.DataSetId.Equals(ErrorDatasetId) & dt.Type == ObjectType.Repeating
                                                                  select dt).FirstOrDefault();
                        if (repeatingDataTracker != null)
                        {
                            repeatingGridRowStart = ExcelHelper.GetRange(repeatingDataTracker.Location).Row;
                        }

                        // Clear all columns (for newly added rows) and accept changes for Repeating.
                        // This will ensure that those new rows will be marked as dirty after more changes in sheet.
                        foreach (DataRow dr in ErrorDataTable.Rows)
                        {
                            if (dr.RowState == DataRowState.Added)
                            {
                                for (int col = 0; col < ErrorDataTable.Columns.Count; col++)
                                    dr[col] = (ErrorDataTable.Columns[col].DataType == typeof(double) ||
                                        ErrorDataTable.Columns[col].DataType == typeof(DateTime)) ? DBNull.Value : null;
                                dr.AcceptChanges();
                            }
                        }
                        ErrorDataTable.RejectChanges();
                    }
                }
                else if (dataManager.GetCrossTabDataByDataSetId(ErrorDatasetId) != null)
                {
                    // Reject changes for CrossTab
                    dataManager.GetCrossTabDataByDataSetId(ErrorDatasetId).DataTable.RejectChanges();
                }
                else if (dataManager.GetMatrixDataSetById(ErrorDatasetId) != null)
                {
                    //reject changes for Matrix
                    dataManager.GetMatrixDataSetById(ErrorDatasetId).MatrixDataTable.RejectChanges();
                }

                // Compile Error Message(s)
                StringBuilder ErrorMessage = new StringBuilder();
                ApttusDataSet dataSet = dataManager.GetDataByDataSetId(ErrorDatasetId);
                if (dataSet != null)
                    ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResults_ErrMsg"), applicationDefinitionManager.GetAppObject(dataSet.AppObjectUniqueID).Name));
                else
                {
                    ApttusCrossTabDataSet crossTabDataSet = dataManager.GetCrossTabDataByDataSetId(ErrorDatasetId);
                    if (crossTabDataSet != null)
                        ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResults_ErrMsg"), applicationDefinitionManager.CrossTabDefinitions.Where(def => def.UniqueId.Equals(crossTabDataSet.CrossTabID)).FirstOrDefault().Name));
                }

                foreach (ApttusSaveRecord ErrorRecord in CurrentErrorRecords)
                {
                    if (IsHighlightingEnabled)
                    {
                        if (ErrorRecord.ObjectType == ObjectType.Independent)
                        {
                            SaveHelper.HighLightIndependentObjErrors(appInstance, model, ErrorRecord, rowsAdded);
                        }
                        else if (ErrorRecord.ObjectType == ObjectType.Repeating)
                        {
                            SaveHelper.HighLightRepeatingObjErrors(appInstance, model, ErrorRecord, rowsAdded);
                        }
                    }

                    int errorRecordRow = ErrorRecord.RecordRowNo + repeatingGridRowStart + 2;
                    switch (ErrorRecord.OperationType)
                    {
                        case QueryTypes.INSERT:
                            ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsInsertError_ErrMsg"), errorRecordRow.ToString(), ErrorRecord.ErrorMessage));
                            break;
                        case QueryTypes.UPDATE:
                            ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsUpdateError_ErrMsg"), errorRecordRow.ToString(), ErrorRecord.RecordId, ErrorRecord.ErrorMessage));
                            break;
                        case QueryTypes.UPSERT:
                            ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsUpsertError_ErrMsg"), errorRecordRow.ToString(), ErrorRecord.RecordId, ErrorRecord.ErrorMessage));
                            break;
                        case QueryTypes.DELETE:
                            ErrorMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsDeleteError_ErrMsg"), errorRecordRow.ToString(), ErrorRecord.RecordId, ErrorRecord.ErrorMessage));
                            break;
                    }
                    if (!bCaptureFirstError)
                    {
                        SaveSummary.FirstErrorMessage = ErrorRecord.ErrorMessage;
                        bCaptureFirstError = true;
                    }
                }
                ErrorMessage.AppendLine("******************************************");

                SaveSummary.ErrorMessage += ErrorMessage.ToString();
            }
#if DEBUG
            // Perfomance Test Code
            timer.Stop();
            timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "Just before Save Dialog.");
            timer.Start();
#endif

            // Handle Successful Records (exclude attachment records)
            // TODO:: how to fail Attachment Records.
            var ErrorAttachmentsDatasetIds = (from er in ErrorRecords where er.HasAttachment == true select er.ApttusDataSetId).Distinct();
            foreach (Guid ErrorDataSetId in ErrorAttachmentsDatasetIds)
            {
                List<ApttusSaveRecord> currentErrorRecords = (from er in ErrorRecords where er.ApttusDataSetId.Equals(ErrorDataSetId) select er).ToList<ApttusSaveRecord>();
                StringBuilder ErrorMessage = AttachmentsDataManager.GetInstance.ProcessFailureAttachmentRecords(ErrorDataSetId, currentErrorRecords);
                ErrorMessage.AppendLine("******************************************");
                SaveSummary.ErrorMessage += ErrorMessage.ToString();
            }

            // ToDo: Handle the error scenario where HasAttachment == true and add to SaveSummary.ErrorMessage.
            if (bDisplayMessage)
            {
                StringBuilder SuccessMessage = new StringBuilder();
                StringBuilder ErrorMessage = new StringBuilder();
                string strDataExceed = "All remaining error messages are logged in Save Message Log.";
                if (SaveSummary.InsertCount + SaveSummary.UpdateCount + SaveSummary.UpsertCount + SaveSummary.DeleteCount > 0)
                {
                    // Group wise object summary
                    var saveObjectSummary = SuccessRecords.GroupBy(a => a.ObjectName).ToList();

                    SuccessMessage.AppendLine(resourceManager.GetResource("SAVEHELPER_ProcessResultsDetails_Text"));
                    for (int i = 0; i < saveObjectSummary.Count; i++)
                    {
                        List<ApttusObject> lookupObjects = applicationDefinitionManager.GetAppObjectById(saveObjectSummary[i].Key);
                        string objectName = saveObjectSummary[i].Key.Equals(Constants.ATTACHMENT) ? Constants.ATTACHMENT : lookupObjects[0].Name.Split('.').Last();
                        SuccessMessage.AppendLine(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsSummery_Text"), objectName, saveObjectSummary[i].Where(sr => sr.OperationType == QueryTypes.INSERT).Count(), saveObjectSummary[i].Where(sr => sr.OperationType == QueryTypes.UPDATE).Count(), saveObjectSummary[i].Where(sr => sr.OperationType == QueryTypes.UPSERT).Count(), saveObjectSummary[i].Where(sr => sr.OperationType == QueryTypes.DELETE).Count()));

                        ////SuccessMessage.AppendLine(
                        ////    "Save details: " + Environment.NewLine +
                        ////    "- No of records inserted: " + SaveSummary.InsertCount + Environment.NewLine +
                        ////    "- No of records updated: " + SaveSummary.UpdateCount + Environment.NewLine +
                        ////    "- No of records upserted: " + SaveSummary.UpsertCount + Environment.NewLine +
                        ////    "- No of records deleted: " + SaveSummary.DeleteCount + Environment.NewLine);
                    }
                }

                if (!string.IsNullOrEmpty(SaveSummary.ErrorMessage))
                    ErrorMessage.AppendLine(SaveSummary.ErrorMessage);

                if (SuccessMessage.Length > 0 & ErrorMessage.Length == 0)
                {
                    if (SaveSummary.DeleteCount > 0 & SaveSummary.InsertCount == 0 & SaveSummary.UpdateCount == 0 & SaveSummary.UpsertCount == 0)
                        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("SAVEHELP_ProcessResultsDELETE_InfoMsg"), resourceManager.GetResource("SAVEHELP_ProcessResultsDELETECAP_InfoMsg"), String.Empty, SuccessMessage.ToString());
                    else
                    {
                        saveSummaryAcrossWorkFlow.Append(SuccessMessage.ToString());
                        // For Suppress
                        //if (!IsSuppressSaveMessage())
                        if (!CanSuppressSuccessSaveMsg(suppressSaveMessage))
                        {
                            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("SAVEHELP_ProcessResultsSAVE_InfoMsg"), resourceManager.GetResource("CORESEARCHNSELECT_SaveActionCap_ErrorMsg"), String.Empty, saveSummaryAcrossWorkFlow.ToString());
                            saveSummaryAcrossWorkFlow.Clear();
                        }
                    }
                }
                else if (SuccessMessage.Length == 0 & ErrorMessage.Length > 0)
                {
                    ExceptionLogHelper.SaveMessageLog(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsSaveLogIF_ErrMsg"), ErrorMessage.ToString()));

                    // For Suppress
                    //if (!IsSuppressSaveMessage())
                    {
                        // Handle message string issue, if it will beyond 50k
                        if (ErrorMessage.Length > 50000)
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEHELPER_RecordFailed_ErrorMsg"), SaveSummary.FirstErrorMessage), resourceManager.GetResource("SAVEHELPER_RecordFailedCap_ErrorMsg"), String.Empty, ErrorMessage.ToString().Substring(0, 50000) + Environment.NewLine + Environment.NewLine + strDataExceed);
                        else
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEHELPER_RecordFailed_ErrorMsg"), SaveSummary.FirstErrorMessage), resourceManager.GetResource("SAVEHELPER_RecordFailedCap_ErrorMsg"), String.Empty, ErrorMessage.ToString());
                    }
                }
                else if (SuccessMessage.Length > 0 & ErrorMessage.Length > 0)
                {
                    ExceptionLogHelper.SaveMessageLog(String.Format(resourceManager.GetResource("SAVEHELPER_ProcessResultsSaveLogELSEIF_ErrMsg"), SuccessMessage.ToString(), ErrorMessage.ToString()));
                    // For Suppress
                    //if (!IsSuppressSaveMessage())
                    {
                        // Handle message string issue, if it will beyond 50k
                        if (ErrorMessage.Length > 50000)
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEHELPER_RecordFailedDetails_ErrorMsg"), SaveSummary.FirstErrorMessage), resourceManager.GetResource("SAVEHELPER_RecordFailedCap_ErrorMsg"), String.Empty, ErrorMessage.ToString().Substring(0, 50000) + Environment.NewLine + Environment.NewLine + strDataExceed);
                        else
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEHELPER_RecordFailedDetails_ErrorMsg"), SaveSummary.FirstErrorMessage), resourceManager.GetResource("SAVEHELPER_RecordFailedCap_ErrorMsg"), String.Empty, SuccessMessage.ToString() + ErrorMessage.ToString());
                    }
                }
            }

            return SaveSummary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaveRecords"></param>
        public static void ValidatePicklistValues(List<ApttusSaveRecord> SaveRecords)
        {
            foreach (ApttusSaveRecord saveRecord in SaveRecords)
            {
                // Get all Picklist fields from save records
                List<ApttusSaveField> picklistFields = saveRecord.SaveFields.Where(f => f.DataType == Datatype.Picklist).ToList<ApttusSaveField>();
                foreach (ApttusSaveField saveField in picklistFields)
                {
                    // Get ApttusField for Picklist save field and verify against Picklist options
                    ApttusObject apttusObj = applicationDefinitionManager.GetAppObjectById(saveRecord.ObjectName).FirstOrDefault();
                    ApttusField apttusPickListField = applicationDefinitionManager.GetField(apttusObj.UniqueId, saveField.FieldId);

                    if (apttusPickListField == null && saveField.FieldId.IndexOf('.') != -1)
                    {
                        string[] splitFieldId = saveField.FieldId.Split(new char[] { '.' });
                        string fieldId = splitFieldId[splitFieldId.Length - 1];
                        apttusPickListField = applicationDefinitionManager.GetField(apttusObj.UniqueId, fieldId);
                    }

                    if (apttusPickListField != null && (!string.IsNullOrEmpty(saveField.Value) && !apttusPickListField.PicklistValues.Contains(saveField.Value)))
                    {
                        if (string.IsNullOrEmpty(saveRecord.ErrorMessage))
                            saveRecord.ErrorMessage = String.Format(resourceManager.GetResource("SAVEHELP_ValidatePicklistValues_Msg"), saveField.Value, apttusPickListField.Name);
                        else if (saveRecord.ErrorMessage.Length > 0)
                            saveRecord.ErrorMessage = saveRecord.ErrorMessage + ", '" + saveField.Value + "' for field '" + apttusPickListField.Name + "'";
                    }
                    // assign key value of picklist for Dynamics before save
                    else if (CRMContextManager.Instance.ActiveCRM == CRM.DynamicsCRM || CRMContextManager.Instance.ActiveCRM == CRM.AIC)
                    {
                        if (apttusPickListField.PicklistKeyValues != null && apttusPickListField.PicklistKeyValues.Count > 0 && !string.IsNullOrEmpty(saveField.Value))
                        {
                            // TODO:: this is to handle multi select picklist converted to picklist in Dynamics CRM
                            // Remove this after data Migration
                            if (saveField.Value.IndexOf(';') > 0)
                                saveField.Value = string.Empty;
                            else
                            {
                                saveField.Value = apttusPickListField.PicklistKeyValues.Where(o => o.optionValue.Equals(saveField.Value, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().optionKey.ToString();
                            }
                        }
                        if (apttusPickListField.PicklistType == PicklistType.TwoOption)
                        {
                            saveField.DataType = Datatype.Boolean;
                            bool boolValue = saveField.Value == "1";
                            saveField.Value = Convert.ToString(boolValue);
                        }
                    }
                }
            }
        }

        public static string GetRelationalFieldId(ApttusObject saveFieldObject, ApttusObject repDataSetObject, string relationalField = "")
        {
            string fieldId = relationalField;

            ApttusField lookupField = repDataSetObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(saveFieldObject.Id)).FirstOrDefault();
            if (string.IsNullOrEmpty(relationalField) && lookupField != null)
                return lookupField.Id;
            else if (repDataSetObject.Parent != null)
            {
                lookupField = repDataSetObject.Parent.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(saveFieldObject.Id)).FirstOrDefault();
                //fieldId = saveFieldObject.Id + "." + lookupField.Id == null ? ;
                fieldId = GetRelationalFieldId(saveFieldObject, repDataSetObject.Parent, fieldId);
            }
            return fieldId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsSuppressSaveMessage()
        {
            bool blnSuppressMessage = false;
            // Suppress Message as per user's app settings
            if (configurationManager.Definition.AppSettings != null)
            {
                if (configurationManager.Definition.AppSettings.SuppressSave)
                    blnSuppressMessage = true;
            }
            return blnSuppressMessage;
        }

        public static bool CanSuppressSuccessSaveMsg(bool surpressSaveMessage)
        {
            if (configurationManager.Definition.AppSettings == null || (configurationManager.Definition.AppSettings != null && !configurationManager.Definition.AppSettings.SuppressAllRecordsSaveSuccess))
            {
                return surpressSaveMessage;
            }
            else
            {
                return configurationManager.Definition.AppSettings != null && configurationManager.Definition.AppSettings.SuppressAllRecordsSaveSuccess;
            }
        }

        private static void UpdateRichTextFieldInExcel(Guid DataSetId, dynamic recordIDsWithRichText, List<ApttusSaveField> richTextSaveFields)
        {
            ApttusDataTracker tracker = (from track in DataManager.GetInstance.AppDataTracker
                                         where track.DataSetId.Equals(DataSetId)
                                         select track).FirstOrDefault();
            if (tracker != null)
            {
                Excel.Range oRange = ExcelHelper.GetRange(tracker.Location);
                //For repeating Fields
                if (oRange != null && oRange.Rows.Count > 2)
                {
                    RepeatingGroup currentRepeatingGroup = configurationManager.GetRepeatingGroupbyTargetNamedRange(tracker.Location);
                    SaveMap saveMap = configurationManager.GetSaveMapbyTargetNamedRange(tracker.Location);
                    dynamic richTextFieldsColumnIndexesWithFieldName = null;
                    if (currentRepeatingGroup != null)
                    {
                        richTextFieldsColumnIndexesWithFieldName = (from rf in currentRepeatingGroup.RetrieveFields
                                                                    where rf.DataType == Datatype.Rich_Textarea && fieldLevelSecurity.IsFieldVisible(rf.AppObject, rf.FieldId)
                                                                    select new { rf.TargetColumnIndex, rf.FieldId }).ToList();
                    }
                    else if (saveMap != null) //Only SaveOther
                    {
                        SaveGroup saveGroup = configurationManager.GetSaveGroupbyTargetNamedRange(tracker.Location);
                        if (saveGroup != null)
                        {
                            richTextFieldsColumnIndexesWithFieldName = (from sf in saveMap.SaveFields.Where(fld => fld.GroupId.Equals(saveGroup.GroupId) &&
                                                           ApplicationDefinitionManager.GetInstance.GetField(fld.AppObject, fld.FieldId).Datatype == Datatype.Rich_Textarea)
                                                                        select new { sf.TargetColumnIndex, sf.FieldId }).ToList();

                        }
                    }

                    if (richTextFieldsColumnIndexesWithFieldName != null)
                    {
                        foreach (var richTextField in richTextFieldsColumnIndexesWithFieldName)
                        {
                            foreach (var rec in recordIDsWithRichText)
                            {
                                ApttusSaveField currentSaveField = richTextSaveFields.Where(rsf => rsf.FieldId.Equals(rec.FieldId)).FirstOrDefault();

                                if (rec.FieldId.Equals(richTextField.FieldId) && !currentSaveField.SaveFieldType.Equals(SaveType.SaveOnlyField))
                                {
                                    Excel.Range richTextCell = oRange.Rows[rec.RecordRowNo + 3].Cells[richTextField.TargetColumnIndex];
                                    if (String.IsNullOrEmpty(rec.Value))
                                        richTextCell.Value = string.Format(Constants.RICHTEXT_FORMULA_ADD, rec.RecordId);
                                    else
                                        richTextCell.Value = string.Format(Constants.RICHTEXT_FORMULA_EDIT, rec.RecordId);
                                }
                            }
                        }
                    }
                }
                //For Individual fields
                else if (oRange != null && oRange.Rows.Count == 1)
                {
                    foreach (var rec in recordIDsWithRichText)
                    {
                        ApttusSaveField currentSaveField = richTextSaveFields.Where(rsf => rsf.FieldId.Equals(rec.FieldId)).FirstOrDefault();
                        if (currentSaveField != null && !currentSaveField.SaveFieldType.Equals(SaveType.SaveOnlyField))
                        {
                            Excel.Range richTextCell = ExcelHelper.GetRange(currentSaveField.TargetNamedRange);
                            if (String.IsNullOrEmpty(rec.Value))
                                richTextCell.Value = string.Format(Constants.RICHTEXT_FORMULA_ADD, rec.RecordId);
                            else
                                richTextCell.Value = string.Format(Constants.RICHTEXT_FORMULA_EDIT, rec.RecordId);
                        }
                    }
                }
            }
        }

        private static void UpdateExcelWithInsertedIDs(Guid DataSetId, List<ApttusSaveRecord> InsertRecords)
        {
            bool bIsRichTextEditingEnabled = !configurationManager.IsRichTextEditingDisabled;
            ApttusObject currentAppObject = applicationDefinitionManager.GetAppObject(dataManager.GetDataByDataSetId(DataSetId).AppObjectUniqueID);
            if (currentAppObject.ObjectType == ObjectType.Repeating)
            {
                // 1. Insert ID for Repeating Record
                ApttusDataTracker RepeatingDataTracker = (from dt in dataManager.AppDataTracker
                                                          where dt.DataSetId.Equals(DataSetId) & dt.Type == ObjectType.Repeating
                                                          select dt).FirstOrDefault();
                if (RepeatingDataTracker != null)
                {
                    string TargetNamedRange = RepeatingDataTracker.Location;
                    Excel.Range oRange = ExcelHelper.GetRange(TargetNamedRange);
                    if (oRange.Rows.Count > 2)
                    {
                        int IdColumnIndex = 0;
                        dynamic richTextFieldsColumnIndexes = null;
                        Guid AppObject = dataManager.GetDataByDataSetId(DataSetId).AppObjectUniqueID;

                        // 1. Find the Id Column Index based on TargetNamedRange
                        SaveMap currentSaveMap = configurationManager.GetSaveMapbyTargetNamedRange(TargetNamedRange);
                        RepeatingGroup currentRepeatingGroup = (from rm in configurationManager.RetrieveMaps
                                                                from rg in rm.RepeatingGroups
                                                                where rg.TargetNamedRange == TargetNamedRange
                                                                select rg).FirstOrDefault();
                        if (currentRepeatingGroup != null && currentRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == currentAppObject.IdAttribute))
                        {
                            // In case of Save of Displayed and Id Column is in Repeating Group
                            IdColumnIndex = (from rf in currentRepeatingGroup.RetrieveFields where rf.FieldId == currentAppObject.IdAttribute select rf.TargetColumnIndex).FirstOrDefault();
                            if (bIsRichTextEditingEnabled)
                                richTextFieldsColumnIndexes = (from rf in currentRepeatingGroup.RetrieveFields
                                                               where rf.DataType == Datatype.Rich_Textarea && fieldLevelSecurity.IsFieldVisible(rf.AppObject, rf.FieldId)
                                                               select new { rf.TargetColumnIndex, rf.FieldId }).ToList();
                        }
                        else if (currentSaveMap != null)
                        {
                            SaveGroup currentSaveGroup = configurationManager.GetSaveGroupbyTargetNamedRange(TargetNamedRange);
                            if (currentSaveGroup != null && currentSaveMap.SaveFields.Exists(sf => sf.FieldId == currentAppObject.IdAttribute))
                            {
                                // In case of Save Other field and Id Column is in Save Field
                                IdColumnIndex = (from sf in currentSaveMap.SaveFields where sf.FieldId == currentAppObject.IdAttribute & sf.GroupId == currentSaveGroup.GroupId select sf.TargetColumnIndex).FirstOrDefault();
                                if (bIsRichTextEditingEnabled)
                                    richTextFieldsColumnIndexes = (from sf in currentSaveMap.SaveFields where applicationDefinitionManager.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Rich_Textarea & sf.GroupId == currentSaveGroup.GroupId select new { sf.TargetColumnIndex, sf.FieldId }).ToList();
                            }
                        }

                        if (IdColumnIndex > 0)
                        {
                            // 2. Unprotect Sheet to enable cells locking/unlocking
                            ExcelHelper.UpdateSheetProtection(oRange.Worksheet, false);

                            //3. Unlock the cells
                            ChunkAndUpdateId(InsertRecords, oRange, IdColumnIndex, richTextFieldsColumnIndexes);
#if DEBUG
                            // Perfomance Test Code
                            timer.Stop();
                            timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After Id Insert into Excel Repeating - Dataset Id:" + DataSetId.ToString());
                            timer.Start();
#endif
                            // 4. Update Data Protection Lock cell count
                            DataProtectionModel dpm = dataManager.DataProtection.FirstOrDefault(dp => dp.WorksheetName.Equals(oRange.Worksheet.Name)
                                & dp.SaveMapId.Equals(currentSaveMap.Id) & dp.SaveGroupAppObject.Equals(AppObject));

                            if (dpm != null)
                            {
                                dpm.LockCount -= InsertRecords.Count;

                                // 5. Remove from DataProtection collection if LockCount is less than 1.
                                if (dpm.LockCount < 1)
                                    dataManager.DataProtection.Remove(dpm);
                            }
                            // 6. Add sheet protection if DataProtection still exists
                            if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(oRange.Worksheet.Name)))
                                ExcelHelper.UpdateSheetProtection(oRange.Worksheet, true);

                        }
                        //At the end protect the sheet if user has protected sheet 
                        if (ExcelHelper.IsSheetProtectedByUser(oRange.Worksheet))
                        {
                            ExcelHelper.UpdateSheetProtection(oRange.Worksheet, true);
                        }
                    }
                }
            }
            else if (currentAppObject.ObjectType == ObjectType.Independent)
            {
                // 2. Insert ID for Independent Record
                foreach (var tracker in dataManager.AppDataTracker.Where(dt => dt.DataSetId == DataSetId))
                {
                    RetrieveField rf = configurationManager.GetRetrieveFieldbyTargetNamedRange(tracker.Location);
                    if (rf != null && rf.FieldId == currentAppObject.IdAttribute)
                    {
                        Excel.Range oRange = ExcelHelper.GetRange(rf.TargetNamedRange);
                        // In a single save map, there will only one record for a unique object.
                        // So the first record's is picked below.
                        oRange.Value2 = InsertRecords[0].RecordId;
                        break;
                    }
                    else
                    {
                        SaveField sf = configurationManager.GetSaveFieldbyTargetNamedRange(tracker.Location);
                        if (sf != null && sf.FieldId == currentAppObject.IdAttribute)
                        {
                            Excel.Range oRange = ExcelHelper.GetRange(sf.TargetNamedRange);
                            // In a single save map, there will only one record for a unique object.
                            // So the first record's is picked below.
                            oRange.Value2 = InsertRecords[0].RecordId;
                            break;
                        }
                    }
                }
            }
        }

        private static void ChunkAndUpdateId(List<ApttusSaveRecord> InsertRecords, Excel.Range oRange, int IdColumnIndex, dynamic rtFieldsColumnIndexes)
        {
            try
            {
                ExcelHelper.ExcelApp.Calculation = Excel.XlCalculation.xlCalculationManual;

                // Sort Insert Records to determine continous range
                List<ApttusSaveRecord> SortedInsertRecords = InsertRecords;
                SortedInsertRecords.Sort(delegate (ApttusSaveRecord one, ApttusSaveRecord two)
                {
                    return (one.RecordRowNo.CompareTo(two.RecordRowNo));
                });

                List<ApttusSaveRecord> SortedChunkRecords = new List<ApttusSaveRecord>();
                foreach (var SortedInsertRecord in SortedInsertRecords)
                {
                    if (SortedChunkRecords.Count == 0)
                        SortedChunkRecords.Add(SortedInsertRecord);
                    else
                    {
                        if (SortedInsertRecord.RecordRowNo - SortedChunkRecords[SortedChunkRecords.Count - 1].RecordRowNo == 1)
                        {
                            // Continous Range
                            SortedChunkRecords.Add(SortedInsertRecord);
                        }
                        else
                        {
                            UnlockAndUpdateId(oRange, IdColumnIndex, SortedChunkRecords, rtFieldsColumnIndexes);
                            SortedChunkRecords.Clear();
                            SortedChunkRecords.Add(SortedInsertRecord);
                        }
                    }
                }
                if (SortedChunkRecords.Count > 0)
                {
                    UnlockAndUpdateId(oRange, IdColumnIndex, SortedChunkRecords, rtFieldsColumnIndexes);
                }
            }
            finally
            {
                ExcelHelper.ExcelApp.Calculation = Excel.XlCalculation.xlCalculationAutomatic;
            }
        }

        private static void UnlockAndUpdateId(Excel.Range oRange, int IdColumnIndex, List<ApttusSaveRecord> SortedChunkRecords, dynamic rtFieldsColumnIndexes)
        {
            Excel.Range startCell = oRange.Cells[SortedChunkRecords[0].RecordRowNo + 3, IdColumnIndex];
            Excel.Range endCell = oRange.Cells[SortedChunkRecords[SortedChunkRecords.Count - 1].RecordRowNo + 3, IdColumnIndex];
            Excel.Range dataRange = oRange.Worksheet.Range[startCell, endCell];
            ExcelHelper.UpdateCellLock(dataRange, false);
            int row = 0;
            object[,] chunkData = new object[SortedChunkRecords.Count, 1];

            foreach (var SortedChunkRecord in SortedChunkRecords)
                chunkData[row++, 0] = SortedChunkRecord.RecordId;
            dataRange.Value = chunkData;

            chunkData = null;//Important
            if (rtFieldsColumnIndexes != null)
            {
                foreach (var richTextField in rtFieldsColumnIndexes)
                {
                    startCell = oRange.Cells[SortedChunkRecords[0].RecordRowNo + 3, richTextField.TargetColumnIndex];
                    endCell = oRange.Cells[SortedChunkRecords[SortedChunkRecords.Count - 1].RecordRowNo + 3, richTextField.TargetColumnIndex];
                    dataRange = oRange.Worksheet.Range[startCell, endCell];
                    row = 0;
                    object[,] chunkRichTextData = new object[SortedChunkRecords.Count, 1];
                    foreach (var SortedChunkRecord in SortedChunkRecords)
                    {
                        string value = SortedChunkRecord.SaveFields.Where(sf => sf.FieldId.Equals(richTextField.FieldId)).Select(sf => sf.Value).FirstOrDefault();
                        if (String.IsNullOrEmpty(value))
                            chunkRichTextData[row++, 0] = string.Format(Constants.RICHTEXT_FORMULA_ADD, SortedChunkRecord.RecordId);
                        else
                            chunkRichTextData[row++, 0] = string.Format(Constants.RICHTEXT_FORMULA_EDIT, SortedChunkRecord.RecordId);
                    }
                    dataRange.Value = chunkRichTextData;
                    chunkRichTextData = null; //Important
                }
            }
            chunkData = null; //Important
        }

        private static string GetCollisionSOQL(dynamic saveRecords, string Key, out Guid dataSetId)
        {
            bool bAddComma = false;
            dataSetId = Guid.Empty;

            StringBuilder QueryBuilder = new StringBuilder("SELECT LastModifiedBy.Id,").Append(Constants.ID_ATTRIBUTE);
            HashSet<string> SaveFields = new HashSet<string>();
            StringBuilder ids = new StringBuilder();

            foreach (var saveRecord in saveRecords)  //Loop Per SaveRecord
            {
                dataSetId = saveRecord.ApttusDataSetId;
                //Build Query based on SaveFields in the SaveRecord
                foreach (var sf in saveRecord.SaveFields)  //Loop per SaveField
                {
                    if (SaveFields.Contains(sf.FieldId))
                        continue;
                    QueryBuilder.Append(",").Append(sf.FieldId);
                    SaveFields.Add(sf.FieldId);
                }

                if (bAddComma)
                    ids.Append(",");

                ids.Append("'").Append(saveRecord.RecordId).Append("'");

                bAddComma = true;
            }
            QueryBuilder.Append(" FROM ").Append(Key).Append(" WHERE ").Append(Constants.ID_ATTRIBUTE).Append(" IN ( ");
            QueryBuilder.Append(ids).Append(" )");

            ids.Clear();
            SaveFields.Clear();

            return QueryBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UpdateObjects"></param>
        public static void CheckDataForCollisionDetection(List<ApttusSaveRecord> UpdateObjects)
        {
            CheckDataForCollisionDetectionCrossTab(UpdateObjects);
            CheckDataForCollisionDetectionIndAndRep(UpdateObjects);
        }

        public static void CheckDataForCollisionDetectionIndAndRep(List<ApttusSaveRecord> UpdateObjects)
        {
            try
            {
                var listOfSaveRecordsByObjectNames = UpdateObjects.Where(sr => sr.ObjectType == ObjectType.Independent || sr.ObjectType == ObjectType.Repeating).
                                                        Select(s => new { s.RecordId, s.ObjectName, s.ApttusDataSetId, s.SaveFields }).GroupBy(s => s.ObjectName).ToList();

                foreach (var saveRecords in listOfSaveRecordsByObjectNames) //Loop Per ObjectName
                {
                    Guid dataSetId;
                    string collisionSQOL = GetCollisionSOQL(saveRecords, saveRecords.Key, out dataSetId);
                    if (Utils.IsValidSOQL(collisionSQOL))
                    {
                        //Retrieve the latest values from salesforce
                        ApttusDataSet latestDataSet = objectManager.QueryDataSet(new SalesforceQuery { SOQL = collisionSQOL, Object = null, DataTable = null, UserInfo = Globals.ThisAddIn.userInfo });

                        //this is the Original Dataset which was retrieved for the first time
                        ApttusDataSet originalDataSet = DataManager.GetInstance.GetDataByDataSetId(dataSetId);

                        foreach (ApttusSaveRecord record in UpdateObjects.Where(rec => rec.ObjectName.Equals(saveRecords.Key)))
                        {
                            DataRow[] newRows;
                            if (IsRecordIgnored(latestDataSet, record, out newRows))
                                continue;

                            if (Globals.ThisAddIn.userInfo.UserId.Equals(Convert.ToString(newRows[0]["LastModifiedBy.Id"])))
                                continue;

                            foreach (ApttusSaveField field in record.SaveFields)
                            {
                                if (field.SaveFieldType == SaveType.SaveOnlyField)
                                    continue; //Ignore save other fields

                                string latestValue = Convert.ToString(newRows[0][field.FieldId]);
                                DataRow[] rows = originalDataSet.DataTable.Select(Constants.ID_ATTRIBUTE + "='" + record.RecordId + "'");
                                if (rows.Count() == 1)
                                {
                                    string originalValue = Convert.ToString(rows[0][field.FieldId, DataRowVersion.Original]);
                                    if (IsDirty(field.DataType, originalValue, latestValue))
                                        IgnoreSaveRecord(record);
                                }
                            }
                        }

                        //Dispose the DataTable
                        latestDataSet.DataTable.Dispose();
                        latestDataSet = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message, resourceManager.GetResource("SAVEHELPER_CheckDataForCollisionDetectionIndAndRep_ErrorMsg"));
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public static bool IsRecordIgnored(ApttusDataSet latestDataSet, ApttusSaveRecord record, out DataRow[] newRows)
        {
            newRows = null;
            bool bRecordIgnored = false;
            if (latestDataSet.DataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
            {
                newRows = latestDataSet.DataTable.Select(Constants.ID_ATTRIBUTE + "='" + record.RecordId + "'");
                if (newRows.Count() == 0)
                {
                    record.Ignore = true;
                    record.ErrorMessage = resourceManager.GetResource("SAVEHELPER_IsRecordIgnored_WarnMsg");
                    bRecordIgnored = true;
                }
            }
            else
            {
                //Since there are no columns, which means there are no records, hence ignore all records
                record.Ignore = true;
                record.ErrorMessage = resourceManager.GetResource("SAVEHELPER_IsRecordIgnored_WarnMsg");
                bRecordIgnored = true;
            }
            return bRecordIgnored;
        }

        public static void CheckDataForCollisionDetectionCrossTab(List<ApttusSaveRecord> UpdateObjects)
        {
            try
            {
                // Condition 1: Collision Detection is not supported for CrossTab. sr.RecordColumnNo < 0 ignores CrossTab.
                var listOfSaveRecordsByObjectNames = UpdateObjects.Where(sr => sr.ObjectType == ObjectType.CrossTab).Select(s => new { s.RecordId, s.ObjectName, s.ApttusDataSetId, s.SaveFields }).GroupBy(s => s.ObjectName).ToList();

                foreach (var saveRecords in listOfSaveRecordsByObjectNames) //Loop Per ObjectName
                {
                    Guid dataSetId;
                    string collisionSQOL = GetCollisionSOQL(saveRecords, saveRecords.Key, out dataSetId);
                    if (Utils.IsValidSOQL(collisionSQOL))
                    {
                        //Retrieve the latest values from salesforce
                        ApttusDataSet latestDataSet = objectManager.QueryDataSet(new SalesforceQuery { SOQL = collisionSQOL, Object = null, DataTable = null, UserInfo = Globals.ThisAddIn.userInfo });

                        //this is the Original Dataset which was retrieved for the first time
                        ApttusCrossTabDataSet originalDataSet = DataManager.GetInstance.GetCrossTabDataByDataSetId(dataSetId);

                        foreach (ApttusSaveRecord record in UpdateObjects.Where(rec => rec.ObjectName.Equals(saveRecords.Key)))
                        {
                            DataRow[] newRows;
                            if (IsRecordIgnored(latestDataSet, record, out newRows))
                                continue;

                            if (Globals.ThisAddIn.userInfo.UserId.Equals(Convert.ToString(newRows[0]["LastModifiedBy.Id"])))
                                continue;

                            foreach (ApttusSaveField field in record.SaveFields)
                            {
                                string latestValue = Convert.ToString(newRows[0][field.FieldId]);
                                string colName = originalDataSet.DataTable.Columns[record.RecordColumnNo].ColumnName;
                                string originalValue = originalDataSet.DataTable.Rows[record.RecordRowNo][colName, DataRowVersion.Original] as string;
                                if (IsDirty(field.DataType, originalValue, latestValue))
                                    IgnoreSaveRecord(record);
                            }
                        }

                        //Dispose the DataTable
                        latestDataSet.DataTable.Dispose();
                        latestDataSet = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message, "Save Failed During Pre-check");
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private static void IgnoreSaveRecord(ApttusSaveRecord record)
        {
            record.Ignore = true;
            record.ErrorMessage = resourceManager.GetResource("SAVEHELPER_IgnoreSaveRecord_WarnMsg");
        }

        public static bool IsDirty(Datatype dataType, string originalValue, string changedValue)
        {
            bool IsDirty = false;
            //Datatype saveFieldDataType;
            //if (saveField.Type == ObjectType.CrossTab)
            //    saveFieldDataType = saveField.CrossTab.DataField.DataType;
            //else
            //    saveFieldDataType = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId).Datatype;

            switch (dataType)
            {
                case Datatype.Double:
                    //double dChangedValue = Convert.ToDouble(changedValue);
                    double doubleChangedValue;
                    double? dChangedValue = double.TryParse((string)changedValue, out doubleChangedValue) ? doubleChangedValue : (double?)null;

                    //double dOriginalValue = Convert.ToDouble(originalValue);
                    double doubleOriginalValue;
                    double? dOriginalValue = double.TryParse((string)originalValue, out doubleOriginalValue) ? doubleOriginalValue : (double?)null;

                    IsDirty = !double.Equals(dChangedValue, dOriginalValue);
                    break;

                case Datatype.Decimal:
                    decimal decimalChangedValue;
                    decimal? deChangedValue = decimal.TryParse((string)changedValue, out decimalChangedValue) ? decimalChangedValue : (decimal?)null;

                    decimal decimalOriginalValue;
                    decimal? deOriginalValue = decimal.TryParse((string)originalValue, out decimalOriginalValue) ? decimalOriginalValue : (decimal?)null;

                    IsDirty = !Decimal.Equals(deChangedValue, deOriginalValue);
                    break;

                case Datatype.Date:
                case Datatype.DateTime:
                    DateTime datetimeChangedValue;
                    DateTime? dtChangedValue = DateTime.TryParse((string)TranslateData(dataType, changedValue), out datetimeChangedValue) ? datetimeChangedValue : (DateTime?)null;

                    DateTime datetimeOriginalValue;
                    DateTime? dtOriginalValue = DateTime.TryParse((string)TranslateData(dataType, originalValue), out datetimeOriginalValue) ? datetimeOriginalValue : (DateTime?)null;

                    IsDirty = !DateTime.Equals(dtChangedValue, dtOriginalValue);
                    break;

                case Datatype.Boolean:
                    {
                        bool tempLatestValue;
                        bool? boolLatestValue = bool.TryParse(changedValue, out tempLatestValue) ? tempLatestValue : (bool?)null;

                        bool tempOriginalValue;
                        bool? boolOriginalValue = bool.TryParse(originalValue, out tempOriginalValue) ? tempOriginalValue : (bool?)null;

                        IsDirty = !bool.Equals(boolLatestValue, boolOriginalValue);
                    }
                    break;

                case Datatype.String:
                case Datatype.Email:
                case Datatype.Textarea:
                case Datatype.Rich_Textarea:
                case Datatype.Composite:
                default:
                    if (String.IsNullOrEmpty(changedValue) && string.IsNullOrEmpty(originalValue))
                        IsDirty = false;
                    else
                        IsDirty = !string.Equals(changedValue, originalValue);
                    break;
            }
            return IsDirty;
        }

        /// <summary>
        /// Translate data (i.e. Save values) to be sent to Salesforce (or CRM).
        /// </summary>
        /// <param name="saveField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TranslateData(Datatype dataType, string value)
        {
            switch (dataType)
            {
                case Datatype.Date:
                case Datatype.DateTime:
                    DateTime OriginalDateFormat;
                    //To Handle Salesforce dates
                    if (DateTime.TryParse(value, out OriginalDateFormat))
                    {
                        value = XmlConvert.ToString(OriginalDateFormat, XmlDateTimeSerializationMode.Local);
                    }
                    //To Handle AIC dates
                    else if (DateTime.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out OriginalDateFormat))
                    {
                        value = XmlConvert.ToString(OriginalDateFormat, XmlDateTimeSerializationMode.Local);
                    }
                    else
                    {
                        double ExcelDateTimeSerialNo;
                        if (double.TryParse(value, out ExcelDateTimeSerialNo))
                        {
                            if (ExcelDateTimeSerialNo > 59) //Excel/Lotus 2/29/1900 bug
                                value = XmlConvert.ToString(new DateTime(1899, 12, 31).AddDays(ExcelDateTimeSerialNo - 1), XmlDateTimeSerializationMode.Local);
                            else
                                value = null;

                        }
                        else
                            value = null;
                    }
                    break;
                default:
                    break;
            }
            return value;
        }

        public static void ValidateLookupNamesBeforeSave(List<ApttusSaveRecord> SaveRecords, ApttusObject AppObject, out StringBuilder LookupErrors)
        {
            ISaveHelper crmSaveHelper;
            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                crmSaveHelper = Globals.ThisAddIn.GetSaveHelper();
            else
                crmSaveHelper = new SalesforceSaveHelper();

            crmSaveHelper.ValidateLookupNamesBeforeSave(SaveRecords, AppObject, out LookupErrors);
        }
        /// <summary>
        /// clears comments and highlighting data from failed saves for the current workflow only. 
        /// </summary>
        /// <param name="errorData"></param>
        /// <returns></returns>
        public static bool ClearErrorHighlighting(RowHighLighting errorData, bool rowsAdded, SaveMap saveMap)
        {
            //List<string> rangeList = errorData.GetRanges();
            try
            {
                ExcelHelper.ExcelApp.ScreenUpdating = false;
                HashSet<string> rangeSet = errorData.GetRangesBysMapId(saveMap.Id.ToString());
                bool cellUnlocked = false;
                if (rangeSet != null)
                {
                    foreach (string a1Range in rangeSet)
                    {
                        Excel.Range rangeToClear;
                        Excel.Range currentSaveGroupRange;
                        Excel.Range formulaRow = null;
                        try
                        {
                            rangeToClear = ExcelHelper.GetRangeByLocation(a1Range);
                            foreach (var sm in saveMap.SaveGroups)
                            {
                                currentSaveGroupRange = ExcelHelper.GetRange(sm.TargetNamedRange);
                                if (Globals.ThisAddIn.Application.Intersect(currentSaveGroupRange, rangeToClear) != null)
                                {
                                    formulaRow = currentSaveGroupRange.Rows[2][1];
                                    break;
                                }
                            }
                            if (rowsAdded)//if (ExcelHelper.IsUserSheetProtection(rangeToClear.Worksheet)) //remove protection if necessary
                            {
                                ExcelHelper.UpdateSheetProtection(rangeToClear.Worksheet, false);
                                cellUnlocked = true;
                            }
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                        if (formulaRow != null)
                        {
                            formulaRow.Copy();
                            rangeToClear.PasteSpecial(Excel.XlPasteType.xlPasteFormats, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone);
                        }
                        else
                            rangeToClear.Interior.Color = errorData.GetColor(a1Range);

                        int colNum = errorData.GetColNum(a1Range);
                        if (colNum != -1) //the range is not a single cell and a column must be specified
                        {
                            try
                            {
                                rangeToClear.Cells[colNum].ClearComments();
                                if (cellUnlocked) //re-add protection if removed
                                {
                                    if (dataManager.DataProtection.Exists(dp => dp.WorksheetName.Equals(rangeToClear.Worksheet.Name)))
                                        ExcelHelper.UpdateSheetProtection(rangeToClear.Worksheet, true);
                                }
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }
                        else//the range is a single cell, no column needed
                        {
                            rangeToClear.ClearComments();
                        }
                    }
                    Globals.ThisAddIn.Application.CutCopyMode = Excel.XlCutCopyMode.xlCopy;
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
            }
            finally
            {
                ExcelHelper.ExcelApp.ScreenUpdating = true;
            }

            return true;
        }
        /// <summary>
        /// takes in an error message from salesforce. Checks if it is a failed update due to missinge field. Returns the error code and the missing fieldIds
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        /// 

        //TODO: Update for non english error messages
        public static List<string> ParseErrorMessage(string errorMsg, out errorCodes errorCode)
        {
            string[] errorFields;
            List<string> errorFieldList = new List<string>();
            errorCode = errorCodes.UNKNOWN;
            try
            {
                if (errorMsg.IndexOf(':') != -1) //a required field is missing
                {
                    string[] errorSplit = errorMsg.Split(':');
                    string strErrorCode;
                    strErrorCode = errorSplit[1];
                    strErrorCode = strErrorCode.Substring(2, 22);
                    string fields = "";
                    if (errorCode.Equals("REQUIRED_FIELD_MISSING"))
                    {
                        errorCode = errorCodes.REQUIRED_FIELD_MISSING;
                        fields = errorSplit[4];
                        fields = fields.Trim();
                        errorFields = fields.Split(',');
                        errorFieldList = errorFields.ToList();
                        //int backSlash = errorFieldList[errorFieldList.Count - 1].IndexOf('\r');
                        //errorFieldList[errorFieldList.Count - 1] = errorFieldList[errorFieldList.Count - 1].Substring(0, backSlash).Trim();
                    }
                }
                else //invalid picklist option, or unknown error
                {
                    if (errorMsg.Contains("Invalid Picklist option"))
                    {
                        errorCode = errorCodes.INVALID_PICKLIST_OPTION;
                        string[] errorMsgSplit = errorMsg.Split(',');
                        string delimitor = "for field";
                        foreach (string error in errorMsgSplit) //loop through bad picklist values
                        {
                            int start = error.IndexOf(delimitor);
                            int length = delimitor.Length + 2;
                            int end = error.LastIndexOf("\'");
                            string errorField = error.Substring(start + length, end - (start + length)).Trim();
                            errorFieldList.Add(errorField);
                        }
                    }
                }
                if (errorFieldList.Count == 0)
                {
                    errorCode = errorCodes.UNKNOWN;
                }
            }
            catch (Exception e)
            {
                errorCode = errorCodes.UNKNOWN;
                ExceptionLogHelper.ErrorLog(e.Message);
            }
            return errorFieldList;
        }
        /// <summary>
        /// error codes used when parsing erorr messages for unsaved records.
        /// </summary>
        public enum errorCodes
        {
            UNKNOWN = 1,
            REQUIRED_FIELD_MISSING = 2,
            INVALID_PICKLIST_OPTION = 3
        }
        /// <summary>
        /// adds error comment and highlight error to range. Saves previous color to memory in order to save later. 
        /// </summary>
        /// <param name="ErrorData"></param>
        /// <param name="rowToHighlight"></param>
        /// <param name="a1Range"></param>
        /// <param name="previousColor"></param>
        /// <param name="ErrorRecord"></param>
        /// <param name="isMatrix"></param>
        /// <param name="rowsAdded"></param>
        public static void AddHighLightAndCommentToRange(RowHighLighting ErrorData, Excel.Range rowToHighlight, string a1Range, double previousColor, ApttusSaveRecord ErrorRecord, bool isMatrix, bool rowsAdded)
        {
            try
            {

                ExcelHelper.ExcelApp.ScreenUpdating = false;
                rowToHighlight.Interior.Color = GetRowHighlightColor(); //set row or cell color.
                bool cellUnlocked = false;
                int count = rowToHighlight.Columns.Count;
                for (int colNum = 1; colNum <= count; colNum++)  //try every col until leftmost visible non recordId col is found
                {
                    Excel.Range errorCell = rowToHighlight.Columns[colNum];
                    if (!errorCell.Hidden) //if cell is not hidden
                    {
                        string errorCellValue = string.Empty;
                        if (errorCell.Value2 != null) errorCellValue = errorCell.Value2;

                        if (!ErrorRecord.RecordId.Equals(errorCellValue)) //if not recordId cell
                        {
                            try
                            {
                                Excel.Worksheet oSheet = rowToHighlight.Worksheet;

                                if (rowsAdded)//if (ExcelHelper.IsUserSheetProtection(oSheet)) //remove protection if necessary
                                {
                                    ExcelHelper.UpdateSheetProtection(oSheet, false);
                                    cellUnlocked = true;
                                }
                                errorCell.ClearComments(); //remove previous comment
                                errorCell.AddComment(ErrorRecord.ErrorMessage);//add error message as comment
                                ErrorData.SetCommentCol(a1Range, colNum);
                                ErrorData.SetColor(a1Range, previousColor);
                                errorCell.Comment.Shape.TextFrame.AutoSize = true;
                                colNum = -1;
                                if (cellUnlocked) //re-add protection if removed
                                {
                                    ExcelHelper.UpdateSheetProtection(rowToHighlight.Worksheet, true);
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                ExcelHelper.ExcelApp.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// highlights and add comments for repeating objects. Any list object or matrix fits this category
        /// </summary>
        /// <param name="appInstance"></param>
        /// <param name="model"></param>
        /// <param name="ErrorRecord"></param>
        /// <param name="rowsAdded"></param>
        public static void HighLightRepeatingObjErrors(ApplicationInstance appInstance, SaveMap model, ApttusSaveRecord ErrorRecord, bool rowsAdded)
        {
            RowHighLighting ErrorData = appInstance.RowHighLightingData;
            Excel.Range range = null;
            HashSet<string> rangeSeen = new HashSet<string>();
            SaveMap sMap;
            List<SaveField> sList = new List<SaveField>();
            bool isMatrixSave = false;
            sMap = configurationManager.GetSaveMapbyTargetNamedRange(ErrorRecord.TargetNameRange);
            //string errorCode = "";
            errorCodes errorCode = errorCodes.UNKNOWN;
            List<string> errorFields = new List<string>();
            try
            {
                errorFields = ParseErrorMessage(ErrorRecord.ErrorMessage, out errorCode);
            }
            catch (Exception e)
            {

            }
            if (sMap == null) //if sMap is null then is matrix map
            {
                sMap = model;
            }
            string sMapId = sMap.Id.ToString();
            //get the sList from the model and get all of the save fields.
            sList = (from sf in sMap.SaveFields
                     where sf.AppObject == ErrorRecord.AppObject
                     select sf).ToList();
            if (sList[0].SaveFieldType == SaveType.MatrixField)
            {
                isMatrixSave = true;
            }

            string saveRange = ErrorRecord.TargetNameRange; //matrix field ErrorRecords do not have a targetNameRange
            range = ExcelHelper.GetRange(saveRange);
            if (range == null) //is matrix map and need to use save record for location
            {
                saveRange = sList[0].TargetNamedRange;
                range = ExcelHelper.GetRange(saveRange);
            }
            if (isMatrixSave)
            {
                Excel.Range matrixRange;
                try //get range for matrix highlight
                {
                    matrixRange = range.Cells[ErrorRecord.MatrixColIndex + 1][ErrorRecord.MatrixRowIndex + 1];  //add one to the index to go from 0-index to 1-indexed counting
                }
                catch (Exception e)
                {
                    return;
                }
                try//add comment, set color and save previous color
                {
                    string a1Range = ExcelHelper.GetAddress(matrixRange);
                    double previousColor = matrixRange.Interior.Color;
                    ErrorData.CreateNewRange(a1Range, sMapId);

                    SaveHelper.AddHighLightAndCommentToRange(ErrorData, matrixRange, a1Range, previousColor, ErrorRecord, isMatrixSave, rowsAdded);

                    //ErrorData.SetColor(a1Range, matrixRange.Interior.Color);
                    //matrixRange.Interior.Color = GetRowHighlightColor();
                    //if(rowsAdded) //unlock sheet if row has been added
                    //{
                    //    ExcelHelper.UpdateSheetProtection(matrixRange.Worksheet, false);
                    //    cellUnlocked = true;
                    //}
                    //matrixRange.AddComment(ErrorRecord.ErrorMessage);
                    //matrixRange.Comment.Shape.TextFrame.AutoSize = true;
                    //if(cellUnlocked)
                    //{
                    //    ExcelHelper.UpdateSheetProtection(matrixRange.Worksheet, true);
                    //}
                }
                catch (Exception e)
                {

                }
            }
            else //is repeating object
            {
                int recordRowNum = ErrorRecord.RecordRowNo;
                Excel.Range rowToHighlight;
                int rowRecordNum = ErrorRecord.RecordRowNo + 3;
                rowToHighlight = range.Rows[rowRecordNum]; //add 1 for hidden row under header, add 1 for header, add 1 for 0-index to 1-index conversion                                                 
                string a1Range = ExcelHelper.GetAddress(rowToHighlight);

                int count = range.Columns.Count;
                int commentColNum = 0;
                List<string> rangeList = new List<string>();
                if (!errorCode.Equals(errorCodes.UNKNOWN)) //If errorcode is parsed
                {
                    if (errorCode.Equals(errorCodes.REQUIRED_FIELD_MISSING)) //if missing field
                    {
                        //loop through slist, match fieldIds and comment highlight a single cell
                        List<string> fieldIdList = new List<string>();
                        fieldIdList = (from sf in sList where errorFields.Contains(sf.FieldId) select sf.FieldId).ToList();
                        rangeList = (from sf in sList where errorFields.Contains(sf.FieldId) select sf.DesignerLocation).ToList();
                    }
                    else if (errorCode.Equals(errorCodes.INVALID_PICKLIST_OPTION)) //if invalid picklist option
                    {
                        foreach (SaveField sf in sList)
                        {
                            int dot = sf.SaveFieldName.IndexOf(".");
                            if (dot != -1)
                            {
                                string[] saveNameSplit = sf.SaveFieldName.Split('.');
                                string fieldName = saveNameSplit[saveNameSplit.Length - 1];
                                if (errorFields.Contains(fieldName))
                                {
                                    rangeList.Add(sf.DesignerLocation);
                                }
                            }
                        }
                    }
                    foreach (string designerLocation in rangeList) //for each designer location, add offset to reach correct cell, add comment change color, save data to ErrorData
                    {
                        string[] location = designerLocation.Split('$');
                        location[2] = (Convert.ToInt32(location[2]) + ErrorRecord.RecordRowNo + 1) + "";
                        a1Range = string.Join("$", location);
                        rowToHighlight = ExcelHelper.GetRangeByLocation(a1Range);
                        double previousColor = rowToHighlight.Interior.Color;
                        ErrorData.CreateNewRange(a1Range, sMapId);
                        AddHighLightAndCommentToRange(ErrorData, rowToHighlight, a1Range, previousColor, ErrorRecord, isMatrixSave, rowsAdded);
                    }
                }
                else //error code is unknown
                {
                    double previousColor = rowToHighlight.Interior.Color;
                    //rowToHighlight.Interior.Color = GetRowHighlightColor();
                    ErrorData.CreateNewRange(a1Range, sMapId);
                    AddHighLightAndCommentToRange(ErrorData, rowToHighlight, a1Range, previousColor, ErrorRecord, isMatrixSave, rowsAdded);
                }
            }
        }

        /// <summary>
        /// Sets color and adds comments for save errors for individual/independent objects. 
        /// </summary>
        /// <param name="appInstance"></param>
        /// <param name="model"></param>
        /// <param name="ErrorRecord"></param>
        /// <param name="rowsAdded"></param>
        public static void HighLightIndependentObjErrors(ApplicationInstance appInstance, SaveMap model, ApttusSaveRecord ErrorRecord, bool rowsAdded)
        {
            RowHighLighting ErrorData = appInstance.RowHighLightingData;
            Excel.Range range = null;
            SaveMap sMap;
            List<SaveField> sList = new List<SaveField>();
            List<string> saveFieldIdList;

            //get the save map based on the error record's target named range
            sMap = configurationManager.GetSaveMapbyTargetNamedRange(ErrorRecord.SaveFields[0].TargetNamedRange);
            string sMapId = sMap.Id.ToString();
            saveFieldIdList = (from sf in ErrorRecord.SaveFields select sf.FieldId).ToList();
            //get the save fields that potentially have an error
            sList = (from sf in sMap.SaveFields
                     where sf.AppObject == ErrorRecord.AppObject
                     && saveFieldIdList.Contains(sf.FieldId)
                     select sf).ToList();
            //grab the first range
            Excel.Range rowToHighlight;
            range = ExcelHelper.GetRange(ErrorRecord.SaveFields[0].TargetNamedRange);
            var errorColor = GetRowHighlightColor();
            try
            {
                ExcelHelper.UpdateSheetProtection(range.Worksheet, false);
                foreach (SaveField sf in sList) //loop through every field in the save list, add data to errorData and highlight with an error
                {
                    rowToHighlight = ExcelHelper.GetRange(sf.TargetNamedRange);
                    string a1Range = ExcelHelper.GetAddress(rowToHighlight);
                    //Add Error Data and prevous color to collection
                    ErrorData.CreateNewRange(a1Range, sMapId);
                    ErrorData.SetColor(a1Range, rowToHighlight.Interior.Color);

                    //Highlight Excel range and add comment
                    rowToHighlight.ClearComments();
                    rowToHighlight.Interior.Color = errorColor;
                    rowToHighlight.AddComment(ErrorRecord.ErrorMessage);//add error message as comment   
                    rowToHighlight.Comment.Shape.TextFrame.AutoSize = true;

                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
            }
            finally
            {
                ExcelHelper.UpdateSheetProtection(range.Worksheet, true);
            }
        }

        /// <summary>
        /// gets the color for save errors. It is set by the designer.
        /// </summary>
        /// <returns></returns>
        public static Color GetRowHighlightColor()
        {
            if (configurationManager.Definition.AppSettings != null)
            {
                if (configurationManager.Definition.AppSettings.RowErrorColor != null)
                {
                    return Color.FromArgb(Convert.ToInt32(configurationManager.Definition.AppSettings.RowErrorColor));
                    //color is saved as a string because System.Color is not xml serializable
                }
            }
            return Color.Red;  //return default color          
        }

        /// <summary>
        /// checks in the settings if row highlighting is enabled for save errors. This value is set in the desinger. 
        /// </summary>
        /// <returns></returns>
        public static bool IsRowHighlightingEnabled()
        {
            if (configurationManager.Definition.AppSettings != null)
            {
                return configurationManager.Definition.AppSettings.EnableRowHighlight;
            }
            else
            {
                return false;
            }
        }
        public static void ValidateMultiLookupEntitiesReferenceBeforeSave(List<ApttusSaveRecord> SaveRecords)
        {
            foreach (ApttusSaveRecord saveRecord in SaveRecords)
            {
                if (saveRecord.SaveFields.Any(f => (f.DataType == Datatype.Lookup || f.DataType == Datatype.Composite) && !f.FieldId.EndsWith(Constants.APPENDLOOKUPID)))
                {
                    List<ApttusSaveField> saveFields = saveRecord.SaveFields.Where(f => (f.DataType == Datatype.Lookup || f.DataType == Datatype.Composite) && !f.FieldId.EndsWith(Constants.APPENDLOOKUPID)).ToList();
                    foreach (ApttusSaveField savefield in saveFields)
                    {
                        ApttusField apttusField = applicationDefinitionManager.GetAppObjectById(saveRecord.ObjectName).FirstOrDefault().Fields.FirstOrDefault(f => f.Id == savefield.FieldId);
                        if (apttusField.MultiLookupObjects.Count > 0)
                        {
                            Guid primaryId = Guid.Empty;
                            Guid.TryParse(savefield.Value, out primaryId);
                            if (primaryId != Guid.Empty)
                                savefield.LookupObjectId = objectManager.GetObjectNameByPrimaryId(primaryId, apttusField.MultiLookupObjects.Select(f => f.Id).ToList());
                        }
                    }
                }
            }
        }

    }
}
