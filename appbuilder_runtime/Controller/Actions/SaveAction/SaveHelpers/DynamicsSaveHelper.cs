using System;
using System.Collections.Generic;
using System.Text;
using Apttus.XAuthor.Core;
using System.Linq;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    class DynamicsSaveHelper : ISaveHelper
    {


        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DataManager dataManager = DataManager.GetInstance;

        public string GetLookupIdFromRelationalFieldID(Guid AppObjectUniqueID, ApttusObject currentAppObject, string originalRelationalSaveFieldId, string relationalSaveFieldId)
        {
            ApttusObject repDataSetObject = applicationDefinitionManager.GetAppObject(AppObjectUniqueID);
            ApttusField lookupField = repDataSetObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(currentAppObject.Id)).FirstOrDefault();
            string lookUpIdField = lookupField == null ? string.Empty : lookupField.Id;
            // 
            if (string.IsNullOrEmpty(lookUpIdField) && !string.IsNullOrEmpty(originalRelationalSaveFieldId))
            {
                string relationalFieldLookupName = originalRelationalSaveFieldId.Substring(0, originalRelationalSaveFieldId.LastIndexOf(Constants.DOT) + 1);
                relationalFieldLookupName = relationalFieldLookupName + Constants.NAME_ATTRIBUTE;
                lookUpIdField = applicationDefinitionManager.GetLookupIdFromLookupName(relationalFieldLookupName);
            }
            return lookUpIdField;
        }

        public void SetColumnNamesOfChangedDataTable(RepeatingGroup currentObjectRepeatingGroup, List<SaveField> currentObjectSaveFields, ref DataTable changedDataTable, ApttusObject apttusObject, Guid repeatingDataSetAppObjectUniqueID)
        {
            ApttusObject currentAppObjectForCompare = apttusObject;
            if (repeatingDataSetAppObjectUniqueID != apttusObject.UniqueId)
            {
                currentAppObjectForCompare = applicationDefinitionManager.GetAppObject(repeatingDataSetAppObjectUniqueID);
            }


            for (int i = 0; i < changedDataTable.Columns.Count; i++)
            {
                if (currentObjectSaveFields.Exists(s => s.TargetColumnIndex.Equals(i + 1)))
                {
                    var fieldId = currentObjectSaveFields.SingleOrDefault(s => s.TargetColumnIndex == i + 1).FieldId;
                    changedDataTable.Columns[i].ColumnName = fieldId;
                    if (changedDataTable.Columns[i].ColumnName.Equals((currentAppObjectForCompare.Id + Constants.ID_ATTRIBUTE).ToLower()))
                        changedDataTable.Columns[i].ColumnName = Constants.ID_ATTRIBUTE;
                    else if (changedDataTable.Columns[i].ColumnName.Equals(currentAppObjectForCompare.IdAttribute, StringComparison.OrdinalIgnoreCase))
                        changedDataTable.Columns[i].ColumnName = Constants.ID_ATTRIBUTE;
                }
                else if (currentObjectRepeatingGroup != null && (currentObjectRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == currentAppObjectForCompare.IdAttribute)
                    && currentObjectRepeatingGroup.RetrieveFields.FirstOrDefault(rf => rf.FieldId == currentAppObjectForCompare.IdAttribute).TargetColumnIndex.Equals(i + 1)))
                    changedDataTable.Columns[i].ColumnName = Constants.ID_ATTRIBUTE;
                else
                    changedDataTable.Columns[i].ColumnName = "blank-" + (i + 1).ToString();
            }
        }

        public void ValidateLookupNamesBeforeSave(List<ApttusSaveRecord> SaveRecords, ApttusObject AppObject, out StringBuilder LookupErrors)
        {
            LookupErrors = new StringBuilder();

            if (SaveRecords.Count > 0)
            {

                // Find all distinct Lookup Name fields
                List<string> distinctLookupNameFields = (from sr in SaveRecords
                                                         from sf in sr.SaveFields
                                                         where sf.LookupNameField
                                                         select sf.FieldId).Distinct().ToList();

                // for each Lookup Name field, validate if all Lookup Names exists
                foreach (string lookupNameField in distinctLookupNameFields)
                {
                    List<string> distinctLookupNames = (from sr in SaveRecords
                                                        from sf in sr.SaveFields
                                                        where sf.LookupNameField && sf.FieldId == lookupNameField && !string.IsNullOrEmpty(sf.Value)
                                                        select sf.Value).Distinct().ToList();
                    string lookupIdField = applicationDefinitionManager.GetLookupIdFromLookupName(lookupNameField);
                    ApttusField lookupField = AppObject.Fields.Where(f => f.Id == lookupIdField).FirstOrDefault();
                    string ErrorLookupNames = string.Empty;

                    List<string> lookupNameChunck = new List<string>();
                    int proccessedRecord = 0;

                    while (getLookupNameRecordsChunck(ref proccessedRecord, distinctLookupNames, ref lookupNameChunck).Count > 0)
                    {
                        int chunckSize = lookupNameChunck.Count;
                        // Handle Multiple Entity List as a Lookup
                        if (lookupField.MultiLookupObjects != null && lookupField.MultiLookupObjects.Count > 0)
                        {
                            bool matchFound = false;
                            lookupField.MultiLookupObjects = ReOrderLookupObjectList(lookupField.MultiLookupObjects);
                            foreach (ApttusObject lookupObject in lookupField.MultiLookupObjects)
                            {
                                if (dataManager.FindLookupNames(lookupObject, lookupNameChunck, out ErrorLookupNames, true))
                                {
                                    matchFound = true;
                                    break;
                                }
                                // Remove found Records
                                if (!string.IsNullOrEmpty(ErrorLookupNames))
                                    lookupNameChunck = ErrorLookupNames.Split(',').ToList();
                            }
                            if (!matchFound)
                            {
                                string ErrorMessage = "Lookup field: " + AppObject.Fields.Where(f => f.Id == lookupNameField).FirstOrDefault().Name + Environment.NewLine + "Invalid Values: " + ErrorLookupNames + Environment.NewLine;
                                LookupErrors.Append(ErrorMessage);
                            }
                        }
                        else
                        {
                            if (!dataManager.FindLookupNames(lookupField.LookupObject, lookupNameChunck, out ErrorLookupNames, true))
                            {
                                string ErrorMessage = "Lookup field: " + AppObject.Fields.Where(f => f.Id == lookupNameField).FirstOrDefault().Name + Environment.NewLine + "Invalid Values: " + ErrorLookupNames + Environment.NewLine;
                                LookupErrors.Append(ErrorMessage);
                            }
                        }
                        proccessedRecord = proccessedRecord + chunckSize;
                    }

                    //Memory Optimization 
                    distinctLookupNames.Clear();
                    distinctLookupNames = new List<string>();
                }

                if (LookupErrors.Length > 0)
                {
                    //ApttusMessageUtil.ShowError("Following lookup values were not found in Salesforce. Please correct the errors and retry."
                    //    + Environment.NewLine + Environment.NewLine + LookupErrors.ToString(), "Apttus Workflow Execution Message : Save Action");
                    return;
                }
                else
                {
                    // Lookup Name validation was successful. Replace the lookup id's in Save Records.
                    // for each Lookup Name field, update Apttus Save Records
                    foreach (string lookupNameField in distinctLookupNameFields)
                    {
                        List<ApttusSaveRecord> currentSaveRecords = (from sr in SaveRecords
                                                                     from sf in sr.SaveFields
                                                                     where sf.LookupNameField && sf.FieldId == lookupNameField
                                                                     select sr).ToList();
                        string lookupIdField = applicationDefinitionManager.GetLookupIdFromLookupName(lookupNameField);
                        ApttusField lookupField = AppObject.Fields.Where(f => f.Id == lookupIdField).FirstOrDefault();

                        foreach (ApttusSaveRecord SaveRecord in currentSaveRecords)
                        {
                            ApttusSaveField currentSaveField = SaveRecord.SaveFields.FirstOrDefault(sf => sf.FieldId == lookupNameField);
                            if (currentSaveField != null)
                            {
                                currentSaveField.FieldId = applicationDefinitionManager.GetLookupIdFromLookupName(lookupNameField);

                                // Handle Case of Multi reference lookup field like CustomerId, OwnerId & RegardingObjectId
                                if (lookupField.MultiLookupObjects != null && lookupField.MultiLookupObjects.Count > 0)
                                {
                                    foreach (ApttusObject lookupObject in lookupField.MultiLookupObjects)
                                    {
                                        string lookupValue = dataManager.GetLookupIdFromLookupName(lookupObject, currentSaveField.Value);
                                        if (!string.IsNullOrEmpty(lookupValue))
                                        {
                                            currentSaveField.Value = lookupValue;
                                            currentSaveField.LookupObjectId = lookupObject.Id;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    currentSaveField.Value = dataManager.GetLookupIdFromLookupName(lookupField.LookupObject, currentSaveField.Value);
                                    // Assign lookupobject id to save field as dynamics needs lookup object id while saving back lookups
                                    currentSaveField.LookupObjectId = lookupField.LookupObject.Id;
                                }
                                // Since fieldID and Value both has changed from .Name to lookupId, change the datatype as well
                                // for SFDC we create XMLElements so savefield datatype doesn't matter,
                                // for Dynamics we need to save value based on datatype so that we can have cleaner save routine
                                currentSaveField.DataType = Datatype.Lookup;
                            }
                        }
                    }
                }
            }
        }
        private List<string> getLookupNameRecordsChunck(ref int proccessedRecord, List<string> distinctLookupList, ref List<string> lookupNameChunck)
        {
            lookupNameChunck = new List<string>();
            int recordLimit = 2000;
            if (proccessedRecord >= distinctLookupList.Count)
                return lookupNameChunck;

            if (distinctLookupList.Count <= recordLimit)
            {
                lookupNameChunck = distinctLookupList;
                return lookupNameChunck;
            }

            int startIndex = proccessedRecord;
            int endIndext = proccessedRecord + recordLimit > distinctLookupList.Count ? distinctLookupList.Count : proccessedRecord + recordLimit;

            for (int count = startIndex; count < endIndext; count++)
                lookupNameChunck.Add(distinctLookupList[count]);

            return lookupNameChunck;

        }
        private List<ApttusObject> ReOrderLookupObjectList(List<ApttusObject> lookupObjectList)
        {
            if (lookupObjectList.Count <= 2)
                return lookupObjectList;  // For Customer and Owner field not required to re-order Lookup List
            List<ApttusObject> allApttusObject = applicationDefinitionManager.GetAllObjects();
            List<ApttusObject> orderedLookupObjectList = new List<ApttusObject>();
            foreach (ApttusObject appObject in allApttusObject)
            {
                foreach (ApttusObject lookupObject in lookupObjectList)
                {
                    if (appObject.Id == lookupObject.Id)
                    {
                        orderedLookupObjectList.Add(lookupObject);
                        break;
                    }
                }
            }
            foreach (ApttusObject lookupObject in lookupObjectList)
            {
                if (!orderedLookupObjectList.Any(f => f.Id == lookupObject.Id))
                    orderedLookupObjectList.Add(lookupObject);
            }

            return orderedLookupObjectList;

        }
    }
}
