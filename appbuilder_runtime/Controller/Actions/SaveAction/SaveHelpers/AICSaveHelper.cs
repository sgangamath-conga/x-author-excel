using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    class AICSaveHelper : ISaveHelper
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DataManager dataManager = DataManager.GetInstance;

        string ISaveHelper.GetLookupIdFromRelationalFieldID(Guid AppObjectUniqueID, ApttusObject currentAppObject, string originalRelationalSaveFieldId, string relationalSaveFieldId)
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

        void ISaveHelper.SetColumnNamesOfChangedDataTable(RepeatingGroup currentObjectRepeatingGroup, List<SaveField> currentObjectSaveFields, ref DataTable changedDataTable, ApttusObject apttusObject, Guid repeatingDataSetAppObjectUniqueID)
        {
            for (int i = 0; i < changedDataTable.Columns.Count; i++)
            {
                if (currentObjectSaveFields.Exists(s => s.TargetColumnIndex.Equals(i + 1)))
                    changedDataTable.Columns[i].ColumnName = currentObjectSaveFields.SingleOrDefault(s => s.TargetColumnIndex == i + 1).FieldId;
                else if (currentObjectRepeatingGroup != null && currentObjectRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == apttusObject.IdAttribute)
                    && currentObjectRepeatingGroup.RetrieveFields.FirstOrDefault(rf => rf.FieldId == apttusObject.IdAttribute).TargetColumnIndex.Equals(i + 1))
                    changedDataTable.Columns[i].ColumnName = apttusObject.IdAttribute;
                else
                    changedDataTable.Columns[i].ColumnName = "blank-" + (i + 1).ToString();
            }
        }

        void ISaveHelper.ValidateLookupNamesBeforeSave(List<ApttusSaveRecord> SaveRecords, ApttusObject AppObject, out StringBuilder LookupErrors)
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
                    if (!dataManager.FindLookupNames(lookupField.LookupObject, distinctLookupNames, out ErrorLookupNames, true))
                    {
                        string ErrorMessage = "Lookup field: " + AppObject.Fields.Where(f => f.Id == lookupNameField).FirstOrDefault().Name + Environment.NewLine + "Invalid Values: " + ErrorLookupNames + Environment.NewLine;
                        LookupErrors.Append(ErrorMessage);
                    }
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
                                currentSaveField.Value = dataManager.GetLookupIdFromLookupName(lookupField.LookupObject, currentSaveField.Value);
                            }
                        }
                    }
                }
            }
        }

    }
}
