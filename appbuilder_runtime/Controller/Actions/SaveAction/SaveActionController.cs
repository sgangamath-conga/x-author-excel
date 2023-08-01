/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppRuntime
{
    public class SaveActionController
    {
        // Get DataManager instance
        DataManager dataManager = DataManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApttusDataSet repeatingDataSet = null;
        ApttusCrossTabDataSet crossTabDataSet = null;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        AttachmentsDataManager attachmentManager = AttachmentsDataManager.GetInstance;
        //private SaveActionView View;
        private SaveMap Model;
        public ActionResult Result { get; private set; }

        private BackgroundWorker BackgroundWorker1 = new BackgroundWorker();
        private WaitWindowView waitWindow;
        private bool suppressSaveMessage;
        private bool EnableRowHighlightErrors;
        private StringBuilder saveSummaryAcrossWorkFlow;
        private int BatchSize;
        private ISaveHelper crmSaveHelper;

        // Define Constructor , this constructor call from workflow
        public SaveActionController(SaveActionModel model, bool SuppressSaveMessage, ref StringBuilder SaveSummaryAcrossWorkFlow)
        {
            // Get Instance of MapID
            Model = configurationManager.SaveMaps.SingleOrDefault(m => m.Id == model.SaveMapId);
            //View = new SaveActionView();
            Result = new ActionResult();
            this.EnableRowHighlightErrors = model.EnableRowHighlightErrors;
            suppressSaveMessage = SuppressSaveMessage;
            saveSummaryAcrossWorkFlow = SaveSummaryAcrossWorkFlow;
            if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                crmSaveHelper = Globals.ThisAddIn.GetSaveHelper();
            else
                crmSaveHelper = new SalesforceSaveHelper();
        }


        public ActionResult Execute()
        {
            waitWindow = new WaitWindowView(resourceManager.GetResource("SAVEACTION_WaitWindow_Msg"), false);
            if (ObjectManager.RuntimeMode == RuntimeMode.Batch)
            {
                try
                {

                    ExecuteAsync();
                }
                catch { }
                finally { waitWindow = null; }

            }
            else
            {
                //Create the Wait Form
                waitWindow.StartPosition = FormStartPosition.CenterParent;

                try
                {
                    InitializeBackgroundWorker();
                    BackgroundWorker1.RunWorkerAsync();
                    waitWindow.ShowDialog();
                }
                catch
                {
                    // Exception is already logged in ExecuteAsync so not logged here.
                    waitWindow.CloseWaitWindow();
                }
            }

            return Result;
        }

        private void InitializeBackgroundWorker()
        {
            // Attach event handlers to the BackgroundWorker object.
            BackgroundWorker1.DoWork +=
                new DoWorkEventHandler(BackgroundWorker1_DoWork);
            BackgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Return the value through the Result property.
            e.Result = ExecuteAsync();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Access the result through the Result property. 
            Result = (ActionResult)e.Result;
            if (waitWindow != null)
                waitWindow.CloseWaitWindow();
        }

        private bool IsFieldEditable(SaveField sf)
        {
            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(sf.AppObject);
            if (sf.FieldId.Equals(apttusObject.IdAttribute))
                return true;

            bool bIsVisible = false;
            bool bIsReadOnly = true;

            string fieldId = sf.FieldId;
            if (!string.IsNullOrEmpty(sf.MultiLevelFieldId) && sf.MultiLevelFieldId.IndexOf(Constants.HYPHEN) > 0)
            {
                string[] splitFieldId = sf.MultiLevelFieldId.Split(new char[] { '-' });
                fieldId = splitFieldId[1];
                if (fieldId.Equals(apttusObject.IdAttribute))
                    return true;
                //fieldId = sf.MultiLevelFieldId.Substring(sf.MultiLevelFieldId.IndexOf(Constants.HYPHEN) + 1, (sf.MultiLevelFieldId.Length - sf.MultiLevelFieldId.IndexOf(Constants.HYPHEN)) - 1);
            }
            fieldLevelSecurity.GetFieldVisibleAndReadOnlyStatus(sf.AppObject, fieldId, out bIsVisible, out bIsReadOnly);

            return bIsVisible && !bIsReadOnly;
        }

        /// <summary>
        /// Call Execute method from wf 
        /// </summary>
        /// <returns></returns>
        public ActionResult ExecuteAsync()
        {
            try
            {
#if DEBUG
                // Perfomance Test Code
                StringBuilder timerLog = new StringBuilder();
                Stopwatch timer = new Stopwatch();
#endif
                Model.SaveFields.RemoveAll(sf => sf.SaveFieldType != SaveType.MatrixField && !IsFieldEditable(sf));

                var uniqueAppObjects = Model.SaveFields.Select(s => new { s.AppObject, s.Type }).Distinct();

                List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
                SaveActionModel saveAction = configurationManager.Actions.OfType<SaveActionModel>().Where(sf => sf.SaveMapId.Equals(Model.Id)).FirstOrDefault();
                List<ApttusSaveRecord> AttachmentRecords = new List<ApttusSaveRecord>();

                /************************************************************************/
                /*************************** I N D E P E N D E N T **********************/
                /************************************************************************/
                // 1. For each AppObject execute Update
                foreach (var uniqueAppObj in uniqueAppObjects)
                {
                    ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject);
                    #region Independent
                    if (uniqueAppObj.Type == ObjectType.Independent)
                    {
                        HashSet<string> RecordIds = new HashSet<string>();
                        bool isIndependentAttachmentDirty = false;
                        // 2. Create a subset of fields for the App Object from the Save Map
                        List<SaveField> currentObjectSaveFields = (from f in Model.SaveFields
                                                                   where f.AppObject == uniqueAppObj.AppObject
                                                                   select f).ToList();

                        // 3. Get the dataset for the independent field. 
                        // There are vague chance that there could be multiple dataset for one objects independent fields.
                        ApttusDataSet DataSet = dataManager.GetDataSetFromLocation(currentObjectSaveFields[0].TargetNamedRange);

                        if (DataSet == null)
                        {
                            DataSet = CreateDataSetForIndependent(uniqueAppObj.AppObject, currentObjectSaveFields);
                            dataManager.AssociateChildDatasets(DataSet.Id);
                        }
                        else if (DataSet.DataTable != null && DataSet.DataTable.Rows.Count == 0)
                        {
                            DataRow dr = DataSet.DataTable.NewRow();
                            DataSet.DataTable.Rows.Add(dr);
                            DataSet.DataTable.AcceptChanges();
                        }

                        ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                        {
                            SaveFields = new List<ApttusSaveField>(),
                            ApttusDataSetId = DataSet.Id,
                            ObjectName = applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject).Id,
                            RecordRowNo = 0,
                            RecordColumnNo = -1,
                            ObjectType = ObjectType.Independent
                        };

                        // 3.5. Find all Lookup Name fields and add Ids to DataManager
                        //List<KeyValuePair<SaveField, string>> LookupNameAndIdFields = (from sf in currentObjectSaveFields
                        //                                                               where sf.FieldId.EndsWith(Constants.APPENDLOOKUPID)
                        //                                                               select new KeyValuePair<SaveField, string>
                        //                                                                (sf, applicationDefinitionManager.GetLookupIdFromLookupName(sf.FieldId))).ToList();
                        //StringBuilder LookupErrors = new StringBuilder();
                        //foreach (var lookup in LookupNameAndIdFields)
                        //{
                        //    string distinctLookupName = SaveHelper.TranslateData(applicationDefinitionManager.GetField(lookup.Key.AppObject, lookup.Key.FieldId).Datatype,
                        //       Convert.ToString(ExcelHelper.GetRange(lookup.Key.TargetNamedRange).Value2));

                        //    if (!string.IsNullOrEmpty(distinctLookupName))
                        //    {
                        //        string ErrorLookupNames = string.Empty;
                        //        if (!dataManager.FindLookupNames(applicationDefinitionManager.GetField(uniqueAppObj.AppObject, lookup.Value).LookupObject, new List<string> { distinctLookupName }, out ErrorLookupNames, true))
                        //        {
                        //            string ErrorMessage = String.Format(resourceManager.GetResource("COMMON_LookUpInvalid_Msg"), lookup.Key.FieldId, ErrorLookupNames);
                        //            LookupErrors.Append(ErrorMessage);
                        //        }
                        //    }
                        //}
                        //if (LookupErrors.Length > 0)
                        //{
                        //    ApttusMessageUtil.ShowError(resourceManager.GetResource("SAVEACTIONCTL_SaveFields_ErrorMsg") + LookupErrors.ToString(), resourceManager.GetResource("SAVEACTIONCTL_SaveFieldsCap_ErrorMsg"));
                        //    Result.Status = ActionResultStatus.Failure;
                        //    return Result;
                        //}

                        // foreach savefields excluding the "Id" field which cannot be saved.

                        bool bIsRichTextEditingDisabled = configurationManager.IsRichTextEditingDisabled;
                        foreach (SaveField saveField in currentObjectSaveFields.Where(f => f.FieldId != apttusObject.IdAttribute && f.Type == ObjectType.Independent))
                        {
                            ApttusField ApttusFieldSaveField = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId);
                            //Datatype saveFieldDatatype = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId).Datatype;

                            // 4. Dirty Field Check - Check if the field value has changed.
                            string sourceValue = string.Empty;
                            //string changeValue = TranslateData(saveField, Convert.ToString(ExcelHelper.GetRange(saveField.TargetNamedRange).Value2));
                            string changeValue = string.Empty;
                            bool isValueDirty = false;

                            // Handle Rich Text dirty check when !bIsRichTextEditingDisabled else it will be normal dirty check
                            if (ApttusFieldSaveField.Datatype == Datatype.Rich_Textarea && !bIsRichTextEditingDisabled)
                            {
                                string richTextValue;
                                string changeRecordId = DataSet.DataTable.Rows[0][Constants.ID_ATTRIBUTE].ToString();
                                if (!String.IsNullOrEmpty(changeRecordId))
                                {
                                    RichTextDataManager richTextManager = RichTextDataManager.Instance;
                                    isValueDirty = richTextManager.IsRecordDirty(changeRecordId, saveField.FieldId, out richTextValue);
                                    if (isValueDirty)
                                        DataSet.DataTable.Rows[0][saveField.FieldId] = changeValue = richTextValue;
                                }
                            }
                            else
                            {
                                sourceValue = Convert.ToString(DataSet.DataTable.Rows[0][saveField.FieldId]);
                                changeValue = Convert.ToString(ExcelHelper.GetRange(saveField.TargetNamedRange).Value2);
                                isValueDirty = SaveHelper.IsDirty(ApttusFieldSaveField.Datatype, sourceValue, changeValue);
                            }

                            if (isValueDirty)
                            {
                                if (ApttusFieldSaveField.Datatype.Equals(Datatype.Attachment))
                                {
                                    // We can't create Guid on rendering time for independent case , so create Guid for independent case when nothing on cell at the time of rendering
                                    if (string.IsNullOrEmpty(sourceValue) && !string.IsNullOrEmpty(changeValue))
                                        sourceValue = attachmentManager.CreateAttachmentRecordID(saveField, DataSet.DataTable.Rows[0], ObjectType.Independent);
                                    attachmentManager.MarkAttachmentRecordDirty(SaveRecord, sourceValue, changeValue, ObjectType.Independent, out isIndependentAttachmentDirty, Convert.ToString(DataSet.DataTable.Rows[0][apttusObject.IdAttribute]));
                                }
                                else
                                    changeValue = SaveHelper.TranslateData(ApttusFieldSaveField.Datatype, changeValue);

                                // Handle DBNull for DataTable
                                if (string.IsNullOrEmpty(changeValue) && Constants.DatatypesWithExplicitColumnDatatypes.Contains(ApttusFieldSaveField.Datatype))
                                    DataSet.DataTable.Rows[0][saveField.FieldId] = DBNull.Value;
                                else
                                {
                                    if (ApttusFieldSaveField.Datatype != Datatype.Attachment && (ApttusFieldSaveField.Datatype != Datatype.Rich_Textarea || bIsRichTextEditingDisabled))
                                        DataSet.DataTable.Rows[0][saveField.FieldId] = changeValue;
                                }
                                // Handle Lookup Name field
                                bool bLookupNameField = false;
                                string lookupFieldId = string.Empty;
                                if (saveField.FieldId.EndsWith(Constants.APPENDLOOKUPID))
                                {
                                    bLookupNameField = true;
                                    lookupFieldId = applicationDefinitionManager.GetLookupIdFromLookupName(saveField.FieldId);
                                    //changeValue = dataManager.GetLookupIdFromLookupName(applicationDefinitionManager.GetField(saveField.AppObject, lookupFieldId).LookupObject, changeValue);
                                }

                                if (!ApttusFieldSaveField.Datatype.Equals(Datatype.Attachment))
                                {
                                    SaveRecord.SaveFields.Add(new ApttusSaveField
                                    {
                                        //FieldId = bLookupNameField ? lookupFieldId : saveField.FieldId,
                                        FieldId = saveField.FieldId,
                                        Value = changeValue,
                                        DataType = ApttusFieldSaveField.Datatype,
                                        CRMDataType = ApttusFieldSaveField.CRMDataType,
                                        TargetNamedRange = saveField.TargetNamedRange,
                                        SaveFieldType = saveField.SaveFieldType,
                                        ExternalId = ApttusFieldSaveField.ExternalId,
                                        LookupNameField = bLookupNameField,
                                        Required = ApttusFieldSaveField.Required // For MS CRM (AB-2505), Validating blank value for Requried fields on Individual object
                                    });
                                }

                                // 5. Add the field and its id to the collection
                                // dataset will be null in case of totally independent save fields
                                string RecordId = DataSet == null ? string.Empty : dataManager.GetRecordId(DataSet, 0);
                                RecordIds.Add(RecordId);

                                // 5.1 DECISION TO BE MADE. Either add ExternalId field to External Id Collection
                                // OR Indicate in ApttusSaveRecord.
                                //bSaveExternalIdField = true;
                            }
                        }
                        if (RecordIds.Count > 1)
                        {
                            ApttusMessageUtil.ShowError(resourceManager.GetResource("SAVEACTIONCTL_SaveFields_ErrorMsg"), resourceManager.GetResource("SAVEACTIONCTL_SaveFieldsCap_ErrorMsg"));
                            Result.Status = ActionResultStatus.Failure;
                            return Result;
                        }

                        // 6. Check if Record Id exists
                        if (SaveRecord.SaveFields.Count > 0 & RecordIds.Count == 1)
                        {
                            SaveRecord.RecordId = RecordIds.First();
                            SaveRecord.AppObject = uniqueAppObj.AppObject;
                            SaveRecords.Add(SaveRecord);
                        }
                        // 7. Find all Lookup Name fields and add Ids to DataManager
                        StringBuilder LookupErrors = new StringBuilder();
                        if (SaveRecord.SaveFields.Count > 0 && RecordIds.Count == 1)
                        {
                            List<ApttusSaveRecord> individualSaveRecords = new List<ApttusSaveRecord>();
                            individualSaveRecords.Add(SaveRecord);
                            List<ApttusSaveRecord> validateLookupSaveRecords = (from sr in individualSaveRecords where sr.SaveFields.Exists(sf => sf.LookupNameField) select sr).ToList();

                            if (validateLookupSaveRecords.Count > 0)
                            {
                                ApttusObject currentAppObject = applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject);
                                SaveHelper.ValidateLookupNamesBeforeSave(validateLookupSaveRecords, currentAppObject, out LookupErrors);
                            }
                        }
                        if (LookupErrors.Length > 0)
                        {
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncLookUpError_ErrorMsg"), LookupErrors.ToString(), resourceManager.CRMName), resourceManager.GetResource("SAVEACTCTL_ExecuteAsyncCAP_InfoMsg"));
                            Result.Status = ActionResultStatus.Failure;
                            return Result;
                        }
                        if (isIndependentAttachmentDirty && RecordIds.Count == 1) //For attachment records
                        {
                            SaveRecord.RecordId = RecordIds.First();
                            SaveRecord.AppObject = uniqueAppObj.AppObject;
                            AttachmentRecords.Add(SaveRecord);
                        }
                        // 7. Add Attachment for Independent object
                        if (RecordIds.Count == 1)
                        {
                            if (Model.IncludeAttachment && Model.AttachAppObjectUniqueId == uniqueAppObj.AppObject)
                            {
                                string AttachmentName = Model.Filename +
                                    (Model.AppendTimestamp ? Constants.UNDERSCORE + DateTime.Now.ToString("yyyyMMddHHmmssffff") : string.Empty)
                                    + Model.Extension;
                                string AttachmentFullName = Utils.GetTempFileName(configurationManager.Definition.UniqueId, AttachmentName);
                                ApttusSaveRecord AttachmentRecord = new ApttusSaveRecord
                                {
                                    OperationType = QueryTypes.INSERT,
                                    HasAttachment = true
                                };

                                AttachmentRecord.Attachments = new List<ApttusSaveAttachment>
                                {
                                    new ApttusSaveAttachment{
                                        AttachmentName = AttachmentName,
                                        Base64EncodedString = ExcelHelper.SaveACopyAndEncodeWorkbook(Globals.ThisAddIn.Application.ActiveWorkbook, AttachmentFullName),
                                        ParentId = SaveRecord.RecordId
                                    }
                                };
                                SaveRecords.Add(AttachmentRecord);
                            }
                        }
                    }
                    #endregion
                    /************************************************************************/
                    /*************************** R E P E A T I N G **************************/
                    /************************************************************************/
                    #region Repeating
                    else if (uniqueAppObj.Type == ObjectType.Repeating)
                    {
                        var bRepeatingFieldsPresent = (from s in Model.SaveFields
                                                       where s.AppObject == uniqueAppObj.AppObject &&
                                                       (s.SaveFieldType == SaveType.RetrievedField || s.SaveFieldType == SaveType.SaveOnlyField)
                                                       select s).Any();

                        if (bRepeatingFieldsPresent)
                        {
#if DEBUG
                            // Start Perfomance Test Code
                            timerLog.AppendLine(string.Empty);
                            timer.Start();

                            timer.Stop();
                            timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "******* Performance Check - Begin Save for Repeating *******");
                            timer.Start();
#endif
                            // 2. Create a subset of fields for the App Object from the Save Map
                            List<ApttusSaveRecord> CurrentSaveRecords = new List<ApttusSaveRecord>();
                            ApttusObject currentAppObject = applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject);
                            List<SaveField> currentObjectSaveFields = (from f in Model.SaveFields
                                                                       where f.AppObject == uniqueAppObj.AppObject
                                                                       select f).ToList();
                            SaveGroup currentObjectSaveGroup = (from sg in Model.SaveGroups
                                                                where sg.AppObject == uniqueAppObj.AppObject
                                                                select sg).FirstOrDefault();

                            // 3. Get the Source Datatable and dataset for the first repeating field (all repeating fields should belong to same AppObject)
                            repeatingDataSet = dataManager.GetDataSetFromLocation(currentObjectSaveGroup.TargetNamedRange);
                            if (repeatingDataSet != null)
                            {
                                waitWindow.Message = string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncSavePrepare_Msg"), currentAppObject.Name);

                                DataTable sourceDataTable = repeatingDataSet.DataTable;
                                LayoutType Ltype = LayoutType.Vertical; // Vertical is default
                                if (currentObjectSaveGroup.Layout != null && currentObjectSaveGroup.Layout.Equals("Horizontal"))
                                    Ltype = LayoutType.Horizontal;
#if DEBUG
                                // Perfomance Test Code
                                timer.Stop();
                                timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "Before Range to Datatable. ");
                                ExceptionLogHelper.InfoLog(timerLog.ToString());
                                timer.Start();
#endif
                                // 4. Get the Change Datatable from TargetNamedRange
                                DataTable changedDataTable = null;
                                ExcelHelper.RangeToDataTable(ExcelHelper.GetRange(currentObjectSaveGroup.TargetNamedRange), ref changedDataTable, Ltype);
#if DEBUG
                                // Perfomance Test Code
                                timer.Stop();
                                timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After Range to Datatable. Record Count - " + changedDataTable.Rows.Count.ToString());
                                ExceptionLogHelper.InfoLog(timerLog.ToString());
                                timer.Start();
#endif

                                waitWindow.Message = string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncDataValCheck_Msg"), currentAppObject.Name);

                                // Validation 1 : Record count validation
                                if (sourceDataTable.Rows.Count != changedDataTable.Rows.Count)
                                {
                                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEACTION_SaveFields_ErrorMsg"), sourceDataTable.Rows.Count, currentAppObject.Name, changedDataTable.Rows.Count), resourceManager.GetResource("SAVEACTION_SaveFieldsCap_ErrorMsg"));
                                    Result.Status = ActionResultStatus.Failure;
                                    return Result;
                                }

                                // 5. Update the Column names in changedDataTable - use Save Fields in the Model (use ColumnIndex)
                                RepeatingGroup currentObjectRepeatingGroup = (from rm in configurationManager.RetrieveMaps
                                                                              from rg in rm.RepeatingGroups
                                                                              where rg.TargetNamedRange == currentObjectSaveGroup.TargetNamedRange
                                                                              select rg).FirstOrDefault();

                                crmSaveHelper.SetColumnNamesOfChangedDataTable(currentObjectRepeatingGroup, currentObjectSaveFields, ref changedDataTable, apttusObject, repeatingDataSet.AppObjectUniqueID);

                                //for (int i = 0; i < changedDataTable.Columns.Count; i++)
                                //{
                                //    if (currentObjectSaveFields.Exists(s => s.TargetColumnIndex.Equals(i + 1)))
                                //        changedDataTable.Columns[i].ColumnName = currentObjectSaveFields.SingleOrDefault(s => s.TargetColumnIndex == i + 1).FieldId;
                                //    else if (currentObjectRepeatingGroup != null && currentObjectRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == apttusObject.IdAttribute)
                                //        && currentObjectRepeatingGroup.RetrieveFields.FirstOrDefault(rf => rf.FieldId == apttusObject.IdAttribute).TargetColumnIndex.Equals(i + 1))
                                //        changedDataTable.Columns[i].ColumnName = apttusObject.IdAttribute;
                                //    else
                                //        changedDataTable.Columns[i].ColumnName = "blank-" + (i + 1).ToString();
                                //}

                                // Id Validations
                                bool bCancel = false;
                                string ValidationMessage = string.Empty;
                                if (changedDataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
                                {
                                    var NonBlankChangedTableRows = (from r in changedDataTable.AsEnumerable()
                                                                    where !string.IsNullOrEmpty(r.Field<string>(Constants.ID_ATTRIBUTE))
                                                                    select r);

                                    HashSet<string> NonBlankSourceIds = new HashSet<string>(from r in sourceDataTable.AsEnumerable()
                                                                                            where !string.IsNullOrEmpty(r.Field<string>(Constants.ID_ATTRIBUTE))
                                                                                            select r.Field<string>(Constants.ID_ATTRIBUTE));

                                    HashSet<string> NonBlankChangeIds = new HashSet<string>(from r in NonBlankChangedTableRows.AsEnumerable()
                                                                                            where !string.IsNullOrEmpty(r.Field<string>(Constants.ID_ATTRIBUTE))
                                                                                            select r.Field<string>(Constants.ID_ATTRIBUTE));

                                    if (!bCancel)
                                    {
                                        // Validation 2 : Id's Tampering Check
                                        var tamperedIds = NonBlankChangeIds.Except(NonBlankSourceIds);
                                        if (tamperedIds.Any())
                                        {
                                            bCancel = true;
                                            StringBuilder ValidationMessages = new StringBuilder();
                                            ValidationMessages.Append(string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncIdNotFound_Msg"), currentAppObject.Name));
                                            int firstRow = ExcelHelper.GetRange(currentObjectSaveGroup.TargetNamedRange).Row;
                                            foreach (var tamperedId in tamperedIds.Distinct())
                                            {
                                                ValidationMessages.Append(tamperedId + resourceManager.GetResource("SAVEACTION_ExecuteAsyncFoundOnRow_Msg"));
                                                List<DataRow> UnknownIdRows = changedDataTable.Select("Id = '" + tamperedId + "'").ToList();
                                                foreach (var dr in UnknownIdRows)
                                                    ValidationMessages.Append(Constants.SPACE + (firstRow + changedDataTable.Rows.IndexOf(dr) + 2).ToString());
                                                ValidationMessages.Append("." + Environment.NewLine);
                                            }
                                            ValidationMessage = ValidationMessages.ToString();
                                        }
                                    }

                                    // Missing Ids collection is required for duplicate id check as well as missing id check.
                                    var missingIds = NonBlankSourceIds.Except(NonBlankChangeIds);
                                    if (!bCancel)
                                    {
                                        // Validation 3 : Duplicate Id check. Logic is:
                                        // If Change Id are duplicates then there would be some Missing Ids as well.
                                        // AND count of source id would be more than change id.
                                        if (missingIds.Any() && NonBlankSourceIds.Count > NonBlankChangeIds.Count)
                                        {
                                            List<string> AllChangeIds = (from r in NonBlankChangedTableRows.AsEnumerable()
                                                                         where !string.IsNullOrEmpty(r.Field<string>(apttusObject.IdAttribute))
                                                                         select r.Field<string>(apttusObject.IdAttribute)).ToList();

                                            var duplicateIds = AllChangeIds
                                                .GroupBy(x => x)
                                                .Where(g => g.Count() > 1)
                                                .Select(y => y.Key)
                                                .ToList();

                                            if (duplicateIds.Any())
                                            {
                                                bCancel = true;
                                                StringBuilder ValidationMessages = new StringBuilder();
                                                ValidationMessages.Append(string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncDupRecordId_Msg"), currentAppObject.Name));
                                                int firstRow = ExcelHelper.GetRange(currentObjectSaveGroup.TargetNamedRange).Row;
                                                foreach (var duplicateId in duplicateIds)
                                                {
                                                    ValidationMessages.Append(duplicateId + resourceManager.GetResource("SAVEACTION_ExecuteAsyncFoundOnRow_Msg"));
                                                    List<DataRow> DuplicateIdRows = changedDataTable.Select("Id = '" + duplicateId + "'").ToList();
                                                    foreach (var dr in DuplicateIdRows)
                                                        ValidationMessages.Append(Constants.SPACE + (firstRow + changedDataTable.Rows.IndexOf(dr) + 2).ToString());
                                                    ValidationMessages.Append("." + Environment.NewLine);
                                                }
                                                ValidationMessage = ValidationMessages.ToString();
                                            }
                                        }
                                    }

                                    if (!bCancel)
                                    {
                                        // Validation 4 : Missing Id Check
                                        if (missingIds.Any())
                                        {
                                            bCancel = true;
                                            StringBuilder ValidationMessages = new StringBuilder();
                                            ValidationMessages.Append(string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncIdMissing_Msg"), currentAppObject.Name));
                                            foreach (var missingId in missingIds)
                                                ValidationMessages.Append(missingId + Constants.SPACE);
                                            ValidationMessages.Append(resourceManager.GetResource("SAVEACTION_ExecuteAsyncAddDelFeature_Msg"));
                                            ValidationMessage = ValidationMessages.ToString();
                                        }
                                    }

                                }
                                else
                                {
                                    bCancel = true;
                                    ValidationMessage = string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncColMissing_Msg"), currentAppObject.Name);
                                }
                                if (bCancel)
                                {
                                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEACTION_DataIntegrity_ErrorMsg"), ValidationMessage), resourceManager.GetResource("SAVEACTION_DataIntegrityCap_ErrorMsg"));
                                    Result.Status = ActionResultStatus.Failure;
                                    return Result;
                                }

#if DEBUG
                                // Perfomance Test Code
                                timer.Stop();
                                timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After all 4 validation. ");
                                ExceptionLogHelper.InfoLog(timerLog.ToString());
                                timer.Start();
#endif

                                bool bIsGroupingOn = false;
                                if (currentObjectRepeatingGroup != null)
                                    bIsGroupingOn = !String.IsNullOrEmpty(currentObjectRepeatingGroup.GroupByField);

                                // Set an Index if there no null values
                                long rowsWithNullIds = sourceDataTable.Select("Id is null").Count();
                                bool bAllSourceTableRowsWithNullIds = (rowsWithNullIds == sourceDataTable.Rows.Count);
                                if (rowsWithNullIds == 0)
                                    sourceDataTable.PrimaryKey = new DataColumn[] { sourceDataTable.Columns[Constants.ID_ATTRIBUTE] };

                                // 6. Dirty Field Check and Create Save Records - Get all changed rows and fields for repeating.
                                for (int i = 0; i < changedDataTable.Rows.Count; i++)
                                {
                                    string changeRecordId = string.Empty;
                                    if (changedDataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
                                        changeRecordId = Convert.ToString(changedDataTable.Rows[i][Constants.ID_ATTRIBUTE]);

                                    // Since after sorting or filter sourceDatatable will not have records in same order, get row by ChangedRecordID
                                    DataRow sourceTableRow = string.IsNullOrEmpty(changeRecordId) ? sourceDataTable.Rows[i] : sourceDataTable.Select("Id = '" + changeRecordId + "'").FirstOrDefault();

                                    //Below condition is needed to skip the grouping row while the data is grouped. If a user tries to change something in the grouped row
                                    //and then tries to save the data, this grouping row should be skiped. 
                                    if (sourceTableRow != null && !String.IsNullOrEmpty(sourceTableRow.RowError) && sourceTableRow.RowError.Contains(Constants.GROUPING_ROW_ERRORMSG_ONLY))
                                        continue;

                                    ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                                    {
                                        SaveFields = new List<ApttusSaveField>(),
                                        ApttusDataSetId = repeatingDataSet.Id,
                                        ObjectName = currentAppObject.Id,
                                        RecordRowNo = Ltype.Equals(LayoutType.Vertical) ? i : -1,
                                        RecordColumnNo = Ltype.Equals(LayoutType.Horizontal) ? i : -1,
                                        TargetNameRange = currentObjectRepeatingGroup != null ? currentObjectRepeatingGroup.TargetNamedRange : currentObjectSaveGroup.TargetNamedRange,
                                        ObjectType = ObjectType.Repeating
                                    };

                                    bool bGroupedFieldsAdded = false;
                                    bool doRelationalSaveFieldExisit = false;
                                    string relationalSaveFieldId = string.Empty;
                                    string originalRelationalSaveFieldId = string.Empty;
                                    // foreach savefields excluding the "Id" field which cannot be saved.
                                    foreach (SaveField saveField in currentObjectSaveFields.Where(f => f.FieldId != apttusObject.IdAttribute && f.Type == ObjectType.Repeating))
                                    {
                                        bool isRelationalSaveField = false;
                                        if (changedDataTable.Columns.Contains(saveField.FieldId))
                                        {
                                            string sfieldId = saveField.MultiLevelFieldId;
                                            if (string.IsNullOrEmpty(sfieldId))
                                                sfieldId = saveField.FieldId;
                                            else
                                            {
                                                string[] split = saveField.MultiLevelFieldId.Split(new char[] { '-' });
                                                if (split != null && split.Count() > 0)
                                                    sfieldId = split[split.Count() - 1];
                                            }

                                            ApttusField ApttusFieldSaveField = applicationDefinitionManager.GetField(saveField.AppObject, sfieldId);
                                            if (!string.Equals(ApttusFieldSaveField.Id, saveField.FieldId) && saveField.FieldId.IndexOf('.') != -1)
                                            {
                                                originalRelationalSaveFieldId = saveField.FieldId;
                                                relationalSaveFieldId = ApttusFieldSaveField.Id;
                                                isRelationalSaveField = true;
                                                doRelationalSaveFieldExisit = true;
                                            }
                                            // Source Value for the dirty check is found either of the 2 ways:
                                            // 1. If source table row based on record id (changeRecordId) is present, find the matching row and field from sourceDataTable
                                            // 2. If source table row is null (in case of newly added row), then set source value as string.Empty
                                            string sourceValue = string.Empty;
                                            if (sourceTableRow != null)
                                            {
                                                sourceValue = Convert.ToString(sourceTableRow[saveField.FieldId]);
                                            }
                                            string changeValue = Convert.ToString(changedDataTable.Rows[i][saveField.FieldId]);
                                            bool isValueDirty = false;
                                            bool bIsRichTextEditingDisabled = configurationManager.IsRichTextEditingDisabled;

                                            if (ApttusFieldSaveField.Datatype != Datatype.Rich_Textarea || bIsRichTextEditingDisabled)
                                            {
                                                // Handle dirty checking for all datatypes except Rich Text
                                                if (SaveHelper.IsDirty(ApttusFieldSaveField.Datatype, sourceValue, changeValue))
                                                {
                                                    isValueDirty = true;
                                                    changeValue = SaveHelper.TranslateData(ApttusFieldSaveField.Datatype, changeValue);
                                                    // Update the Source Field with Changed value.
                                                    if (string.IsNullOrEmpty(changeValue) && Constants.DatatypesWithExplicitColumnDatatypes.Contains(ApttusFieldSaveField.Datatype))
                                                        sourceTableRow[saveField.FieldId] = DBNull.Value;
                                                    else
                                                    {
                                                        if (ApttusFieldSaveField.Datatype != Datatype.Attachment)
                                                            sourceTableRow[saveField.FieldId] = changeValue;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Handle Rich Text dirty check
                                                string richTextValue;
                                                if (!String.IsNullOrEmpty(changeRecordId))
                                                {
                                                    RichTextDataManager richTextManager = RichTextDataManager.Instance;
                                                    isValueDirty = richTextManager.IsRecordDirty(changeRecordId, saveField.FieldId, out richTextValue);
                                                    if (isValueDirty)
                                                        sourceTableRow[saveField.FieldId] = changeValue = richTextValue;
                                                }
                                                else if (String.IsNullOrEmpty(changeRecordId)) //Case of AddRow with RichText Field
                                                {
                                                    string richTextId = sourceTableRow[saveField.FieldId] as string;
                                                    RichTextDataManager richTextManager = RichTextDataManager.Instance;
                                                    isValueDirty = richTextManager.IsRecordDirty(richTextId, saveField.FieldId, out richTextValue);
                                                    if (isValueDirty)
                                                        changeValue = richTextValue;
                                                    //    sourceTableRow[saveField.FieldId] = changeValue = richTextValue;
                                                }

                                            }

                                            if (isValueDirty)
                                            {
                                                if (ApttusFieldSaveField.Datatype.Equals(Datatype.Attachment))
                                                {
                                                    bool isAttachmentdirty = false;
                                                    if (!string.IsNullOrEmpty(changeValue))
                                                        AttachmentsDataManager.GetInstance.MarkAttachmentRecordDirty(SaveRecord, sourceValue, changeValue, ObjectType.Repeating, out isAttachmentdirty, sourceTableRow[apttusObject.IdAttribute].ToString());
                                                    if (isAttachmentdirty)
                                                        AttachmentRecords.Add(SaveRecord);
                                                }
                                                else if (ApttusFieldSaveField.Datatype.Equals(Datatype.Base64))
                                                {
                                                    // Convert file from FilePath to Base64String
                                                    SaveRecord.SaveFields.Add(new ApttusSaveField
                                                    {
                                                        FieldId = isRelationalSaveField ? relationalSaveFieldId : saveField.FieldId,
                                                        Value = Utils.FileToBase64String(changeValue),
                                                        DataType = ApttusFieldSaveField.Datatype,
                                                        CRMDataType = ApttusFieldSaveField.CRMDataType,
                                                        SaveFieldType = saveField.SaveFieldType,
                                                        ExternalId = ApttusFieldSaveField.ExternalId,
                                                        LookupNameField = saveField.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                                                        LookupObjectId = ApttusFieldSaveField.Datatype == Datatype.Lookup ? ApttusFieldSaveField.LookupObject.Id : string.Empty,
                                                        Required = ApttusFieldSaveField.Required
                                                    });
                                                }
                                                else
                                                {

                                                    if ((String.IsNullOrEmpty(changeRecordId) && ApttusFieldSaveField.Creatable) || ApttusFieldSaveField.Updateable)
                                                        // 7. Add the field and its id to the collection
                                                        SaveRecord.SaveFields.Add(new ApttusSaveField
                                                        {
                                                            FieldId = isRelationalSaveField ? relationalSaveFieldId : saveField.FieldId,
                                                            Value = changeValue,
                                                            DataType = ApttusFieldSaveField.Datatype,
                                                            CRMDataType = ApttusFieldSaveField.CRMDataType,
                                                            SaveFieldType = saveField.SaveFieldType,
                                                            ExternalId = ApttusFieldSaveField.ExternalId,
                                                            LookupNameField = saveField.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                                                            LookupObjectId = ApttusFieldSaveField.Datatype == Datatype.Lookup ? ApttusFieldSaveField.LookupObject.Id : string.Empty,
                                                            Required = ApttusFieldSaveField.Required
                                                        });
                                                }
                                                //If grouping is on and new rows are added, then groupby fields by default will become part of save fields for every save record.
                                                if (bIsGroupingOn && !bGroupedFieldsAdded && String.IsNullOrEmpty(changeRecordId))
                                                {
                                                    bGroupedFieldsAdded = true; //Grouped fields are added for this record.

                                                    List<RetrieveField> groupbyRetrieveFields = currentObjectRepeatingGroup.RetrieveFields.
                                                                                                Where(rf => rf.FieldId.Equals(currentObjectRepeatingGroup.GroupByField) || rf.FieldId.Equals(currentObjectRepeatingGroup.GroupByField2)).ToList();

                                                    foreach (RetrieveField rf in groupbyRetrieveFields)
                                                    {
                                                        //We need the if condition because there is a possibility that a given field may not belong to the object as we
                                                        //allow multi level objects in the same repeating group.
                                                        //For e.g An opportunity repeating group can always contain Account object fields, hence groupbyfields belonging to
                                                        //Account object will be ignored and cannot become part of opportunity save record.
                                                        if (rf.AppObject.Equals(currentObjectRepeatingGroup.AppObject) && rf.DataType != Datatype.Formula)
                                                        {
                                                            SaveRecord.SaveFields.Add(new ApttusSaveField
                                                            {
                                                                FieldId = rf.FieldId,
                                                                Value = sourceTableRow[rf.FieldId] as string,
                                                                DataType = rf.DataType,
                                                                CRMDataType = ApttusFieldSaveField.CRMDataType,
                                                                SaveFieldType = SaveType.RetrievedField,
                                                                LookupNameField = rf.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                                                                TargetNamedRange = rf.TargetNamedRange,
                                                                Required = ApttusFieldSaveField.Required
                                                            });
                                                        }
                                                        //While doing add-row, this value was set in the ApttusDataSet->DataTable, so that we can assign the value in the savefield 
                                                        //when it becomes part of saverecord. Once the value is set, reset the value to empty as grouped field column in datatable is always empty.
                                                        sourceTableRow[rf.FieldId] = string.Empty;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // For MS CRM Only
                                    // Add Requried Fields Forcefully if those are added into Save map and user have not provided values into particular fields                                    
                                    if (CRMContextManager.Instance.ActiveCRM == CRM.DynamicsCRM && SaveRecord.SaveFields.Count > 0)
                                    {
                                        var sourceValue = string.Empty;
                                        foreach (SaveField saveField in currentObjectSaveFields.Where(f => f.FieldId != apttusObject.IdAttribute))
                                        {
                                            ApttusField ApttusFieldSaveField = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId);
                                            if (ApttusFieldSaveField.Required)
                                            {
                                                if (sourceTableRow != null)
                                                    sourceValue = Convert.ToString(sourceTableRow[saveField.FieldId]);

                                                if (!SaveRecord.SaveFields.Any(f => f.FieldId == ApttusFieldSaveField.Id) && string.IsNullOrEmpty(sourceValue))
                                                {
                                                    SaveRecord.SaveFields.Add(new ApttusSaveField
                                                    {
                                                        FieldId = ApttusFieldSaveField.Id,
                                                        Value = string.Empty,
                                                        DataType = ApttusFieldSaveField.Datatype,
                                                        CRMDataType = ApttusFieldSaveField.CRMDataType,
                                                        SaveFieldType = saveField.SaveFieldType,
                                                        ExternalId = ApttusFieldSaveField.ExternalId,
                                                        LookupNameField = saveField.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                                                        LookupObjectId = ApttusFieldSaveField.Datatype == Datatype.Lookup ? ApttusFieldSaveField.LookupObject.Id : string.Empty,
                                                        Required = ApttusFieldSaveField.Required
                                                    });
                                                }
                                            }
                                        }
                                    }

                                    SaveRecord.RecordId = string.Empty;
                                    if (changedDataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
                                    {
                                        SaveRecord.RecordId = string.IsNullOrEmpty(changeRecordId)
                                            ? changedDataTable.Rows[i][Constants.ID_ATTRIBUTE].ToString() :
                                            changeRecordId;
                                    }

                                    // 7. Check if atleast one save field was added to the save record, then add to SaveRecords collection.
                                    if (SaveRecord.SaveFields.Count > 0)
                                    {
                                        // 8. Set Lookup (reference) field if atleast one save field is added.
                                        if (repeatingDataSet.Parent != Guid.Empty && repeatingDataSet.AppObjectUniqueID == currentAppObject.UniqueId)
                                        {
                                            ApttusDataSet parentDataSet = dataManager.GetDataByDataSetId(repeatingDataSet.Parent);
                                            ApttusObject parentAppObject = applicationDefinitionManager.GetAppObject(parentDataSet.AppObjectUniqueID);

                                            // Make sure either Unique Id or Id match.
                                            if ((parentAppObject.UniqueId == currentAppObject.Parent.UniqueId)
                                                & (parentAppObject.Id == applicationDefinitionManager.GetAppObject(currentAppObject.Parent.UniqueId).Id)
                                                & parentAppObject.ObjectType == ObjectType.Independent)
                                            {
                                                ApttusField currentAppLookupField = (from f in currentAppObject.Fields
                                                                                     where f.Datatype == Datatype.Lookup && f.LookupObject.Id == parentAppObject.Id
                                                                                     select f).FirstOrDefault();

                                                // Check if the Parent column exists
                                                if (currentAppLookupField != null && sourceTableRow != null && sourceTableRow.Table.Columns.Contains(currentAppLookupField.Id))
                                                {
                                                    string currentAppLookupFieldName = applicationDefinitionManager.GetLookupNameFromLookupId(currentAppLookupField.Id);
                                                    // Add Child Lookup (to Parent) if the Parent Lookup field is blank and SaveField for the parent lookup doesn't exist.
                                                    if (string.IsNullOrEmpty(sourceTableRow[currentAppLookupField.Id].ToString().Trim()) &
                                                        !SaveRecord.SaveFields.Exists(sf => sf.FieldId == currentAppLookupField.Id) &
                                                        !SaveRecord.SaveFields.Exists(sf => sf.FieldId == currentAppLookupFieldName))
                                                    {
                                                        SaveRecord.SaveFields.Add(new ApttusSaveField
                                                        {
                                                            FieldId = currentAppLookupField.Id,
                                                            Value = parentDataSet.DataTable.Rows[0][parentAppObject.IdAttribute].ToString(),
                                                            DataType = currentAppLookupField.Datatype,
                                                            SaveFieldType = SaveType.None
                                                        });

                                                        sourceTableRow[currentAppLookupField.Id] = parentDataSet.DataTable.Rows[0][parentAppObject.IdAttribute];
                                                    }
                                                }
                                            }
                                        }

                                        // use changedDataTable to get ID, as it should always ID now from Range.ID property.                                        

                                        // For Relation object records, use lookup ID field
                                        if (repeatingDataSet.AppObjectUniqueID != currentAppObject.UniqueId)
                                        {
                                            #region Old Save code for relational fields
                                            //ApttusObject repDataSetObject = applicationDefinitionManager.GetAppObject(repeatingDataSet.AppObjectUniqueID);
                                            //ApttusField lookupField = repDataSetObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(currentAppObject.Id)).FirstOrDefault();
                                            //string lookUpIdField = lookupField == null ? string.Empty : lookupField.Id;
                                            //// 
                                            //if (string.IsNullOrEmpty(lookUpIdField) && !string.IsNullOrEmpty(originalRelationalSaveFieldId))
                                            //{
                                            //    string relationalFieldLookupName = string.Empty;
                                            //    if (relationalSaveFieldId.EndsWith(Constants.APPENDLOOKUPID))
                                            //    {
                                            //        string untilLastDot = originalRelationalSaveFieldId.Substring(0, originalRelationalSaveFieldId.LastIndexOf(Constants.DOT));
                                            //        relationalFieldLookupName = originalRelationalSaveFieldId.Substring(0, untilLastDot.LastIndexOf(Constants.DOT) + 1);
                                            //    }
                                            //    else
                                            //        relationalFieldLookupName = originalRelationalSaveFieldId.Substring(0, originalRelationalSaveFieldId.LastIndexOf(Constants.DOT) + 1);
                                            //    relationalFieldLookupName = relationalFieldLookupName + Constants.NAME_ATTRIBUTE;
                                            //    lookUpIdField = applicationDefinitionManager.GetLookupIdFromLookupName(relationalFieldLookupName);
                                            //}
                                            #endregion
                                            string lookUpIdField = crmSaveHelper.GetLookupIdFromRelationalFieldID(repeatingDataSet.AppObjectUniqueID, currentAppObject, originalRelationalSaveFieldId, relationalSaveFieldId);

                                            if (!string.IsNullOrEmpty(lookUpIdField))
                                            {
                                                SaveRecord.RecordId = string.IsNullOrEmpty(Convert.ToString(sourceTableRow[lookUpIdField]))
                                                    ? SaveRecord.RecordId : sourceTableRow[lookUpIdField].ToString();
                                            }

                                            // In case of insert record, make all fields blank to be treated as blank record
                                            if (doRelationalSaveFieldExisit && string.IsNullOrEmpty(SaveRecord.RecordId))
                                                SaveRecord.SaveFields.ForEach(sf => sf.Value = string.Empty);
                                        }

                                        if (string.IsNullOrEmpty(SaveRecord.RecordId) && SaveRecord.SaveFields.Any(a => a.ExternalId))
                                        {
                                            foreach (SaveField saveField in currentObjectSaveFields.Where(f => f.FieldId != apttusObject.IdAttribute && f.Type == ObjectType.Repeating))
                                            {
                                                string sfieldId = saveField.MultiLevelFieldId;
                                                if (string.IsNullOrEmpty(sfieldId))
                                                    sfieldId = saveField.FieldId;
                                                else
                                                {
                                                    string[] split = saveField.MultiLevelFieldId.Split(new char[] { '-' });
                                                    if (split != null && split.Count() > 0)
                                                        sfieldId = split[split.Count() - 1];
                                                }
                                                ApttusField ApttusFieldSaveField = applicationDefinitionManager.GetField(saveField.AppObject, sfieldId);
                                                var isRelationalSaveField = !string.Equals(ApttusFieldSaveField.Id, saveField.FieldId) && saveField.FieldId.IndexOf('.') != -1;
                                                string fieldId = isRelationalSaveField ? relationalSaveFieldId : saveField.FieldId;
                                                if (!SaveRecord.SaveFields.Any(a => a.FieldId == fieldId))
                                                {
                                                    SaveRecord.SaveFields.Add(new ApttusSaveField
                                                    {
                                                        FieldId = fieldId,
                                                        Value = null,
                                                        DataType = ApttusFieldSaveField.Datatype,
                                                        CRMDataType = ApttusFieldSaveField.CRMDataType,
                                                        SaveFieldType = saveField.SaveFieldType,
                                                        ExternalId = ApttusFieldSaveField.ExternalId,
                                                        LookupNameField = saveField.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                                                        LookupObjectId = ApttusFieldSaveField.Datatype == Datatype.Lookup ? ApttusFieldSaveField.LookupObject.Id : string.Empty,
                                                        Required = ApttusFieldSaveField.Required
                                                    });
                                                }
                                            }
                                        }

                                            SaveRecord.AppObject = uniqueAppObj.AppObject;
                                        CurrentSaveRecords.Add(SaveRecord);
                                    }
#if DEBUG
                                    // Perfomance Test Code
                                    if ((i + 1) % 4000 == 0)
                                    {
                                        timer.Stop();
                                        timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After dirty checks complete - " + i.ToString());
                                        ExceptionLogHelper.InfoLog(timerLog.ToString());
                                        timer.Start();
                                    }
#endif

                                }
                                // Remove the Primary Key from Source Datatable
                                sourceDataTable.PrimaryKey = null;
                                sourceDataTable.Columns[Constants.ID_ATTRIBUTE].AllowDBNull = true;
                                sourceDataTable.Columns[Constants.ID_ATTRIBUTE].Unique = false;

                                // Resolve MultiLookup Entity References
                                if (CRMContextManager.Instance.ActiveCRM == CRM.DynamicsCRM || CRMContextManager.Instance.ActiveCRM == CRM.AIC)
                                {
                                    SaveHelper.ValidateMultiLookupEntitiesReferenceBeforeSave(CurrentSaveRecords);
                                }

                                StringBuilder LookupErrors = new StringBuilder();
                                SaveHelper.ValidateLookupNamesBeforeSave(
                                    (from sr in CurrentSaveRecords where sr.SaveFields.Exists(sf => sf.LookupNameField) select sr).ToList(), currentAppObject, out LookupErrors);

                                if (LookupErrors.Length > 0)
                                {
                                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SAVEACTION_ExecuteAsyncLookUpError_ErrorMsg"), LookupErrors.ToString(), resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
                                    Result.Status = ActionResultStatus.Failure;
                                    sourceDataTable.RejectChanges();
                                    return Result;
                                }
                                else
                                {

                                    SaveRecords.AddRange(CurrentSaveRecords);
                                }
                            }
                        }
                    }
                    #endregion

                }

                #region CrossTab
                /************************************************************************/
                /*************************** C R O S S - T A B **************************/
                /************************************************************************/
                var CrossTabSaveFields = (from s in Model.SaveFields
                                          where s.Type == ObjectType.CrossTab
                                          select s);

                foreach (SaveField CrossTabSaveField in CrossTabSaveFields)
                {
                    Datatype CrossTabSaveFieldDatatype = CrossTabSaveField.CrossTab.DataField.DataType;
                    crossTabDataSet = dataManager.GetCrossTabDataSetFromLocation(CrossTabSaveField.CrossTab.DataField.TargetNamedRange);
                    DataTable crossTabIDTable = crossTabDataSet.IDTable;

                    // 1. Dirty Field Check - Get all changed rows for repeating.
                    ////DataTable dataChanged = crossTabDataSet.DataTable.GetChanges();
                    // 2. Create ChangeTable using 1. Columns Names (step 2) and Excel Range data
                    DataTable changedcrossDataTable = ExcelHelper.RangeToDataTableForCrossTab(ExcelHelper.GetRange(CrossTabSaveField.CrossTab.DataField.TargetNamedRange));

                    string currentValue = string.Empty;
                    for (int i = 1; i < crossTabDataSet.DataTable.Columns.Count; i++)
                    {
                        for (int j = 1; j < changedcrossDataTable.Columns.Count; j++)
                        {
                            if (crossTabDataSet.DataTable.Columns[i].ColumnName.Equals(changedcrossDataTable.Columns[j].ColumnName))
                            {
                                for (int k = 0; k < changedcrossDataTable.Rows.Count; k++)
                                {
                                    //if (Convert.ToString(crossTabDataSet.DataTable.Rows[k][i]) != Convert.ToString(changedcrossDataTable.Rows[k][j]))
                                    if (SaveHelper.IsDirty(CrossTabSaveFieldDatatype, Convert.ToString(crossTabDataSet.DataTable.Rows[k][i]), Convert.ToString(changedcrossDataTable.Rows[k][j])))
                                    {
                                        crossTabDataSet.DataTable.Rows[k][i] = changedcrossDataTable.Rows[k][j];
                                        currentValue = Convert.ToString(crossTabDataSet.DataTable.Rows[k][i]);
                                        DataRow IDTableRow = crossTabIDTable.Rows[k];
                                        DataColumn IDTableColumn = crossTabIDTable.Columns[i];

                                        string rowID = Convert.ToString(IDTableRow[0]);
                                        string colID = IDTableColumn.ColumnName;
                                        string transactionalDataID = Convert.ToString(IDTableRow[IDTableColumn]);

                                        ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                                        {
                                            SaveFields = new List<ApttusSaveField>(),
                                            ApttusDataSetId = crossTabDataSet.Id,
                                            ObjectName = applicationDefinitionManager.GetAppObject(CrossTabSaveField.CrossTab.DataField.AppObject).Id,
                                            RecordId = transactionalDataID,
                                            RecordRowNo = k,
                                            RecordColumnNo = i,
                                            ObjectType = ObjectType.CrossTab,
                                            TargetNameRange = CrossTabSaveField.CrossTab.DataField.TargetNamedRange
                                        };


                                        if (String.IsNullOrEmpty(transactionalDataID))
                                        {
                                            // Row Field
                                            SaveRecord.SaveFields.Add(new ApttusSaveField
                                            {
                                                FieldId = CrossTabSaveField.CrossTab.RowLookupFieldId,
                                                Value = rowID,
                                                DataType = applicationDefinitionManager.GetField(CrossTabSaveField.CrossTab.RowField.AppObject, CrossTabSaveField.CrossTab.RowField.FieldId).Datatype,
                                                SaveFieldType = SaveType.RetrievedField
                                            });

                                            // Column Field
                                            SaveRecord.SaveFields.Add(new ApttusSaveField
                                            {
                                                FieldId = CrossTabSaveField.CrossTab.ColLookupFieldId,
                                                Value = colID,
                                                DataType = applicationDefinitionManager.GetField(CrossTabSaveField.CrossTab.ColField.AppObject, CrossTabSaveField.CrossTab.ColField.FieldId).Datatype,
                                                SaveFieldType = SaveType.RetrievedField
                                            });
                                        }

                                        // Data Field
                                        SaveRecord.SaveFields.Add(new ApttusSaveField
                                        {
                                            FieldId = CrossTabSaveField.CrossTab.DataField.FieldId,
                                            Value = Convert.ToString(currentValue),
                                            DataType = applicationDefinitionManager.GetField(CrossTabSaveField.CrossTab.DataField.AppObject, CrossTabSaveField.CrossTab.DataField.FieldId).Datatype,
                                            SaveFieldType = SaveType.RetrievedField
                                        });
                                        SaveRecords.Add(SaveRecord);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Matrix
                /************************************************************************/
                /*************************** MATRIX SAVE FIELDS *************************/
                /************************************************************************/
                var matrixSaveFields = (from field in Model.SaveFields
                                        where field.SaveFieldType == SaveType.MatrixField && field.Type == ObjectType.Repeating
                                        select field).ToList();

                if (matrixSaveFields.Count > 0)
                {
                    MatrixSaveHelper helper = new MatrixSaveHelper();
                    ActionResult resultStatus = Result;
                    SaveRecords.AddRange(helper.CreateSaveRecords(matrixSaveFields, out resultStatus));
                    if (resultStatus.Status == ActionResultStatus.Failure)
                        return Result;
                }
                #endregion

                // 7. Use the AppRecord to execute Save
                if (SaveRecords.Count > 0 || AttachmentRecords.Count > 0)
                {
                    // Assign Batch Size for CRUD operations.
                    BatchSize = saveAction.BatchSize == 0 ? Constants.SAVE_ACTION_BATCH_SIZE : saveAction.BatchSize;

                    (from sr in SaveRecords where string.IsNullOrEmpty(sr.RecordId) select sr).ToList().ForEach(sr => sr.OperationType = QueryTypes.INSERT);
                    (from sr in SaveRecords where !string.IsNullOrEmpty(sr.RecordId) select sr).ToList().ForEach(sr => sr.OperationType = QueryTypes.UPDATE);

                    // Delete Insert Records which have all empty fields
                    List<ApttusSaveRecord> RemoveInsertRecords = RemoveEmptyInsertRecords((from s in SaveRecords where s.OperationType == QueryTypes.INSERT select s).ToList());
                    foreach (ApttusSaveRecord rm in RemoveInsertRecords)
                        SaveRecords.Remove(rm);

                    // Verify valid picklist values
                    if (configurationManager.Application.Definition.AppSettings != null)
                    {
                        if (!configurationManager.Application.Definition.AppSettings.IgnorePicklistValidation)
                            SaveHelper.ValidatePicklistValues(SaveRecords);
                    }
                    else
                        SaveHelper.ValidatePicklistValues(SaveRecords);

                    // EXECUTE SAVE OPERATION
                    if (SaveRecords.Exists(s => s.OperationType == QueryTypes.INSERT))
                    {
                        // 7.1 Insert - All records which don't have record id or external id
                        List<ApttusSaveRecord> InsertRecords = (from s in SaveRecords
                                                                where s.OperationType == QueryTypes.INSERT & string.IsNullOrEmpty(s.ErrorMessage) &
                                                               !(from sf in s.SaveFields where sf.ExternalId select sf).Any()
                                                                select s).ToList();
                        objectManager.Insert(InsertRecords, saveAction.EnablePartialSave, BatchSize, waitWindow);

                        // 7.2 Upsert - All records which don't have record id but have external id
                        // If the upsert records are matched successfully then the operation type will be set from Insert to Upsert.
                        List<ApttusSaveRecord> UpsertRecords = (from s in SaveRecords
                                                                where s.OperationType == QueryTypes.INSERT & string.IsNullOrEmpty(s.ErrorMessage) &
                                                               (from sf in s.SaveFields where sf.ExternalId select sf).Any()
                                                                select s).ToList();
                        objectManager.Upsert(UpsertRecords, saveAction.EnablePartialSave, BatchSize, waitWindow);
                    }

#if DEBUG
                    // Perfomance Test Code
                    timer.Stop();
                    timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After Insert/Upsert");
                    ExceptionLogHelper.InfoLog(timerLog.ToString());
                    timer.Start();
#endif

                    // 7.3 Update - All records which have record id (In this case external id field will be treated like a regular save field)
                    if (SaveRecords.Exists(s => s.OperationType == QueryTypes.UPDATE))
                    {
                        List<ApttusSaveRecord> UpdateRecords = (from s in SaveRecords where s.OperationType == QueryTypes.UPDATE & string.IsNullOrEmpty(s.ErrorMessage) select s).ToList();
                        if (saveAction != null && saveAction.EnableCollisionDetection)
                            SaveHelper.CheckDataForCollisionDetection(UpdateRecords);
                        objectManager.Update(UpdateRecords, saveAction.EnablePartialSave, BatchSize, waitWindow);
                    }

#if DEBUG
                    // Perfomance Test Code
                    timer.Stop();
                    timerLog.AppendLine(Utils.TimeSpantoString(timer.Elapsed) + Constants.SPACE + "After Update, Before ProcessResults");
                    ExceptionLogHelper.InfoLog(timerLog.ToString());
                    timer.Start();
#endif
                    // assign correct ParentId to all Attachments save records and execute
                    if (AttachmentRecords.Count > 0)
                    {
                        AttachmentRecords = attachmentManager.ProcessAttachments(AttachmentRecords, saveAction.EnablePartialSave, BatchSize, waitWindow);
                        SaveRecords.AddRange(AttachmentRecords);
                    }

                    ApttusSaveSummary SaveSummary = SaveHelper.ProcessResults(SaveRecords, true, ref saveSummaryAcrossWorkFlow, null, null, suppressSaveMessage, EnableRowHighlightErrors, Model);
                    Result.Status = ActionResultStatus.Success;

                    //Highlight the Ignored records during Save Pre-check
                    HighlightExcelRows(SaveRecords);
                }
                else
                {
                    if (EnableRowHighlightErrors)
                    {
                        string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
                        ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);
                        if (appInstance != null)
                        {
                            SaveHelper.ClearErrorHighlighting(appInstance.RowHighLightingData, false, Model);
                        }
                    }
                    // Default message will be used for Crosstab as unique object collection will be 0.
                    string noRecordsMessage = resourceManager.GetResource("SAVEACTCTL_ExecuteAsyncNoRecordFound_InfoMsg");
                    if (uniqueAppObjects != null && uniqueAppObjects.Count() > 0)
                    {
                        StringBuilder noRecordObjects = new StringBuilder();
                        foreach (var uniqueAppObj in uniqueAppObjects)
                            noRecordObjects.AppendLine(Constants.HYPHEN + Constants.SPACE +
                                applicationDefinitionManager.GetAppObject(uniqueAppObj.AppObject).Name);

                        noRecordsMessage = string.Format(resourceManager.GetResource("SAVEACTCTL_ExecuteAsyncNoRecordFoundObj_InfoMsg"), noRecordObjects.ToString());
                    }
                    // Suppress Message as per user's app settings
                    if (!SaveHelper.IsSuppressSaveMessage())
                        ApttusMessageUtil.ShowInfo(noRecordsMessage, resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));

                    Result.Status = ActionResultStatus.Success;
                }
#if DEBUG
                // Close Out - Perfomance Test Code
                ExceptionLogHelper.InfoLog(timerLog.ToString());
                timerLog.Clear();
#endif

            }
            catch (Exception ex)
            {
                if (repeatingDataSet != null)
                    repeatingDataSet.DataTable.RejectChanges();

                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveRecords"></param>
        private void HighlightExcelRows(List<ApttusSaveRecord> saveRecords)
        {
            foreach (ApttusSaveRecord record in saveRecords.Where(s => s.Ignore == true))
            {
                if (record.ObjectType == ObjectType.Repeating)
                {
                    if (!String.IsNullOrEmpty(record.TargetNameRange))
                    {
                        if (record.RecordColumnNo > -1) // Highlight horizontal
                        {
                            Excel.Range dataRange = ExcelHelper.GetRange(record.TargetNameRange);
                            int nExcelColToSkip = 3; //Skip the Label Row, Formula Row and array indexing starts from 0, so add 1 more and count comes to 3.
                            Excel.Range col = dataRange.Columns[record.RecordColumnNo + nExcelColToSkip] as Excel.Range;
                            HightlightExcelRange(col);
                        }
                        else if (record.RecordRowNo > -1) // Highlight vertical
                        {
                            Excel.Range dataRange = ExcelHelper.GetRange(record.TargetNameRange);
                            int nExcelRowToSkip = 3; //Skip the Label Row, Formula Row and array indexing starts from 0, so add 1 more and count comes to 3.
                            Excel.Range row = dataRange.Rows[record.RecordRowNo + nExcelRowToSkip] as Excel.Range;
                            HightlightExcelRange(row);
                        }
                    }
                }
                else if (record.ObjectType == ObjectType.Independent)
                {
                    foreach (ApttusSaveField field in record.SaveFields)
                    {
                        if (!String.IsNullOrEmpty(field.TargetNamedRange))
                        {
                            Excel.Range dataRange = ExcelHelper.GetRange(field.TargetNamedRange);
                            HightlightExcelRange(dataRange);
                        }
                    }
                }
                else if (record.ObjectType == ObjectType.CrossTab)
                {
                    Excel.Range dataRange = ExcelHelper.GetRange(record.TargetNameRange);
                    Excel.Range cell = dataRange.Cells[record.RecordRowNo + 1, record.RecordColumnNo];
                    HightlightExcelRange(cell);
                }
            }
        }

        public static void HightlightExcelRange(Excel.Range dataRange)
        {
            ExcelHelper.SetCellBorderLineStyle(dataRange, Excel.XlLineStyle.xlContinuous);
            ExcelHelper.SetBorderThemeColor(dataRange, Excel.XlThemeColor.xlThemeColorAccent6);
            ExcelHelper.SetBorderWeight(dataRange, Excel.XlBorderWeight.xlMedium);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppObject"></param>
        /// <param name="SaveFields"></param>
        /// <returns></returns>
        private ApttusDataSet CreateDataSetForIndependent(Guid AppObject, List<SaveField> SaveFields)
        {
            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(AppObject);
            ////SaveField attachmentField = null;
            string DataSetName = applicationDefinitionManager.GetAppObject(AppObject).Name + "_" + Model.Name;
            ApttusDataSet newDataSet = new ApttusDataSet
            {
                Name = DataSetName,
                AppObjectUniqueID = AppObject,
                DataTable = new DataTable()
            };

            // Add it datamanager
            dataManager.AddData(newDataSet);

            // Add columns to datatable
            foreach (SaveField sField in SaveFields)
            {
                if (!newDataSet.DataTable.Columns.Contains(sField.FieldId))
                {
                    newDataSet.DataTable.Columns.Add(sField.FieldId);
                    // Add data tracker entry
                    dataManager.AddDataTracker(new ApttusDataTracker
                    {
                        Location = sField.TargetNamedRange,
                        DataSetId = newDataSet.Id,
                        Type = sField.Type
                    });
                }

                ////if (applicationDefinitionManager.GetField(AppObject, sField.FieldId).Datatype == Datatype.Attachment)
                ////    attachmentField = sField;
            }

            // Add the Id Column if it's not present from SaveFields
            if (!newDataSet.DataTable.Columns.Contains(apttusObject.IdAttribute))
            {
                DataColumn idColumn = new DataColumn(apttusObject.IdAttribute);
                newDataSet.DataTable.Columns.Add(idColumn);
            }

            // Create a empty row
            DataRow dr = newDataSet.DataTable.NewRow();
            ////// if Attachment field exists on Independent Object, create attachment entry as well
            ////if (attachmentField != null)
            ////    AttachmentsDataManager.GetInstance.CreateAttachmentGuid(attachmentField, dr, ObjectType.Independent);
            newDataSet.DataTable.Rows.Add(dr);
            newDataSet.DataTable.AcceptChanges();

            return newDataSet;
        }

        /// <summary>
        /// This function is called before Save and removes the ApttusSaveRecord which has all empty fields.
        /// </summary>
        /// <param name="SaveRecords"></param>
        private List<ApttusSaveRecord> RemoveEmptyInsertRecords(List<ApttusSaveRecord> SaveRecords)
        {
            List<ApttusSaveRecord> RemoveInsertRecords = new List<ApttusSaveRecord>();
            foreach (ApttusSaveRecord sr in SaveRecords)
            {
                bool isEmpty = true;
                foreach (ApttusSaveField sf in sr.SaveFields)
                {
                    if (!string.IsNullOrEmpty(sf.Value))
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty)
                    RemoveInsertRecords.Add(sr);
            }
            return RemoveInsertRecords;
        }

    }
}