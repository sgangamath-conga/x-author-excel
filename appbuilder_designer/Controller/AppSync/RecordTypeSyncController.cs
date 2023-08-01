using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.AppDesigner
{
    internal sealed class RecordTypeSyncController
    {
        private ObjectManager objectManager = ObjectManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private List<RecordTypeSyncModel> recordTypeChanges;
        internal RecordTypeSyncController()
        {
            recordTypeChanges = new List<RecordTypeSyncModel>();
        }
        public void UpdateAppDef()
        {
            foreach (var recordTypeChange in recordTypeChanges)
            {
                appDefManager.UpdateRecordType(recordTypeChange.ObjectId, recordTypeChange.ChangedRecordType, recordTypeChange.OriginalRecordType, appDefManager.AppObjects, recordTypeChange.RecordTypeChange);
            }
        }
        public RecordTypeSyncInfo GetRecordTypeSyncInfo()
        {
            int UpdatedRecordTypes = recordTypeChanges.Count(rt => rt.RecordTypeChange.Equals(RecordTypeChange.RecordTypeDisplayNameChanged));
            int AddedRecordTypes = recordTypeChanges.Count(rt => rt.RecordTypeChange.Equals(RecordTypeChange.RecordTypeAdded));
            int DeletedRecordTypes = recordTypeChanges.Count(rt => rt.RecordTypeChange.Equals(RecordTypeChange.RecordTypeDeleted));

            return new RecordTypeSyncInfo(UpdatedRecordTypes, AddedRecordTypes, DeletedRecordTypes);
        }
        /// <summary>
        /// Synchronize record types of all the objects present in Application Definition
        /// </summary>
        /// <param name="ApttusObjects"></param>
        /// <returns>void</returns>
        public void SyncRecordTypes(ApttusObject CRMObject, ApttusObject currentObject)
        {
            try
            {

                if (CRMObject.RecordTypes != null)
                {

                    objectManager.FillRecordTypeMetadata(CRMObject);

                    List<ApttusRecordType> CurrentObjectRecrodTypes = currentObject.RecordTypes;
                    if (CurrentObjectRecrodTypes == null) CurrentObjectRecrodTypes = new List<ApttusRecordType>();

                    List<ApttusRecordType> CRMObjectRecrodTypes = CRMObject.RecordTypes;

                    #region Scenario 1 :- Check for DisplayName change with or without PicklistValue change
                    foreach (ApttusRecordType currentObjectRecordType in CurrentObjectRecrodTypes)
                    {
                        ApttusRecordType updatedRecordType = (from CRMRecordType in CRMObjectRecrodTypes
                                                              where CRMRecordType.Id.Equals(currentObjectRecordType.Id) && !CRMRecordType.Name.Equals(currentObjectRecordType.Name)
                                                              select CRMRecordType).FirstOrDefault();

                        if (updatedRecordType != null)
                        {
                            RecordTypeSyncModel recordTypeChange = new RecordTypeSyncModel(currentObjectRecordType, updatedRecordType, currentObject.Id, updatedRecordType.Id, RecordTypeChange.RecordTypeDisplayNameChanged);
                            if (!IsRecordTypeChangeExists(recordTypeChange))
                            {
                                recordTypeChanges.Add(recordTypeChange);
                            }
                        }
                    }
                    #endregion

                    #region Scenario 2 :- New RecordTypes Added
                    List<string> NewRecordTypesIds = (from CRMObjectRecrodType in CRMObjectRecrodTypes select CRMObjectRecrodType.Id)
                                                     .Except
                                                     (from currentObjectRecrodType in CurrentObjectRecrodTypes select currentObjectRecrodType.Id).ToList();

                    foreach (var newRecordTypesId in NewRecordTypesIds)
                    {
                        ApttusRecordType recordTypeToAdd = CRMObjectRecrodTypes.FirstOrDefault(recordType => recordType.Id.Equals(newRecordTypesId));
                        RecordTypeSyncModel recordTypeChange = new RecordTypeSyncModel(null, recordTypeToAdd, currentObject.Id, recordTypeToAdd.Id, RecordTypeChange.RecordTypeAdded);

                        if (!IsRecordTypeChangeExists(recordTypeChange))
                        {
                            recordTypeChanges.Add(recordTypeChange);
                        }
                    };
                    #endregion

                    #region Scenario 3 :- Existing RecordTypes Deleted
                    List<string> DeletedRecordTypesIds = (from currentObjectRecrodType in CurrentObjectRecrodTypes select currentObjectRecrodType.Id)
                                                         .Except
                                                         (from CRMObjectRecrodType in CRMObjectRecrodTypes select CRMObjectRecrodType.Id).ToList();

                    foreach (var deletedRecordTypesId in DeletedRecordTypesIds)
                    {
                        ApttusRecordType recordTypeToDelete = CurrentObjectRecrodTypes.FirstOrDefault(recordType => recordType.Id.Equals(deletedRecordTypesId));
                        RecordTypeSyncModel recordTypeChange = new RecordTypeSyncModel(recordTypeToDelete, null, currentObject.Id, recordTypeToDelete.Id, RecordTypeChange.RecordTypeDeleted);

                        if (!IsRecordTypeChangeExists(recordTypeChange))
                        {
                            recordTypeChanges.Add(recordTypeChange);
                        }
                    };
                    #endregion

                    #region Scenario 4 :- Only Picklist Values Changed
                    foreach (ApttusRecordType currentObjectRecordType in CurrentObjectRecrodTypes)
                    {
                        ApttusRecordType updatedRecordType = (from CRMRecordType in CRMObjectRecrodTypes
                                                              where CRMRecordType.Id.Equals(currentObjectRecordType.Id) && CRMRecordType.Name.Equals(currentObjectRecordType.Name)
                                                              select CRMRecordType).FirstOrDefault();

                        // Currently code is just replacing all the matched existing record types with the new record type beacuse computation cost for comparing
                        // each and every picklist is too high in place of just replacing the whole record type. On top of that comparing every picklist doesn't serve any purpose.
                        // updatedRecordType will be null if recordType is deleted.
                        if (updatedRecordType != null)
                        {
                            RecordTypeSyncModel recordTypeChange = new RecordTypeSyncModel(currentObjectRecordType, updatedRecordType, currentObject.Id, updatedRecordType.Id, RecordTypeChange.PicklistValuesModified);
                            if (!IsRecordTypeChangeExists(recordTypeChange))
                            {
                                recordTypeChanges.Add(recordTypeChange);
                            }
                        }
                    }
                    #endregion
                }
                else
                #region Scenario 5 :- Object now doesn't have any record types
                {
                    if (CRMObject.RecordTypes == null && currentObject.RecordTypes != null && currentObject.RecordTypes.Count > 0)
                    {
                        RecordTypeSyncModel recordTypeChange = new RecordTypeSyncModel(null, null, currentObject.Id, string.Empty, RecordTypeChange.ObjectHasNoRecordTypes);
                        recordTypeChanges.Add(recordTypeChange);
                    }
                }
                #endregion

            }

            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, false, ex.Message, "Record Type sync failed");
            }
        }

        private bool IsRecordTypeChangeExists(RecordTypeSyncModel recordTypeSync)
        {
            return recordTypeChanges.Any(rt => rt.RecordTypeId.Equals(recordTypeSync.RecordTypeId) && rt.ObjectId.Equals(recordTypeSync.ObjectId));
        }
    }
}
