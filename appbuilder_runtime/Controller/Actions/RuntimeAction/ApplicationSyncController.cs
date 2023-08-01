/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Apttus.XAuthor.Core;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;


namespace Apttus.XAuthor.AppRuntime
{
    public class ApplicationSyncController
    {
        DataManager dataManager = DataManager.GetInstance;
        ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        MetadataManager metadataManager = MetadataManager.GetInstance;
        FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        public bool blnConfigChanged = false;

        public ApplicationSyncController()
        {

        }

        public List<AppChange> SyncAppConfig(bool SyncAppConfig, bool PicklistValues)
        {
            List<AppChange> appChanges = new List<AppChange>();
            // #1 This function will be used in future to sync the App Config structure with Salesforce 
            // to detect if objects and fields have changed.

            // #2 Sync Picklist for the App
            if (PicklistValues)
                appChanges = SyncPicklist();

            return appChanges;
        }

        private List<AppChange> SyncPicklist()
        {
            List<AppChange> appChanges = new List<AppChange>();

            var allAppDefObjectsWithPicklist = (from AppObject in appDefManager.GetFullHierarchyObjects(null)
                                                from f in AppObject.Fields
                                                where f.Datatype == Datatype.Picklist || f.Datatype == Datatype.Picklist_MultiSelect || f.Datatype == Datatype.Editable_Picklist
                                                select AppObject).Distinct();

            var distinctObjectIdsWithPicklist = (from AppObject in allAppDefObjectsWithPicklist
                                                 select AppObject.Id).Distinct();

            foreach (var distinctObjectId in distinctObjectIdsWithPicklist)
            {
                ApttusObject singleAppDefObject = allAppDefObjectsWithPicklist.FirstOrDefault(o => o.Id == distinctObjectId);
                ApttusObject oApttusObject = objectManager.GetApttusObject(distinctObjectId, false, false);

                // Assign Id and Name attribute again if missing
                singleAppDefObject.IdAttribute = oApttusObject.IdAttribute;
                singleAppDefObject.NameAttribute = oApttusObject.NameAttribute;

                if (singleAppDefObject.Fields.Exists(f => f.RecordType))
                    objectManager.FillRecordTypeMetadata(oApttusObject);


                // AB-2637 : MSD: Modifying field settings as 'Required' or 'Not required' does not reflect directly in the app.
                //overwrite requried field flag on each object fields                                
                foreach (ApttusField field in singleAppDefObject.Fields)
                {
                    //AB - 2647 MSD : Picklist Sync is not working while import/export App & App has Lookup fields selected
                    if (oApttusObject.Fields.Exists(f => f.Id == field.Id))
                        field.Required = oApttusObject.Fields.FirstOrDefault(f => f.Id == field.Id).Required;
                }


                var otherObjectsWithSameId = (from AppObject in appDefManager.GetFullHierarchyObjects(null)
                                              where AppObject.Id == singleAppDefObject.Id
                                              select AppObject).ToList();

                //Gets new fields from the object with same id as singleAppDefObject and adds to usedFields
                List<string> newUsedFields = new List<string>();
                foreach (ApttusObject appObject in otherObjectsWithSameId)
                    newUsedFields.AddRange(configManager.GetAllAppFields(appObject, true).Where(f => f.AppObject == appObject.UniqueId).Select(s => s.FieldId).ToList());

                List<string> usedFields = newUsedFields.Distinct().ToList();



                foreach (ApttusField appDefPicklistField in singleAppDefObject.Fields.Where(f => (f.Datatype == Datatype.Picklist || f.Datatype == Datatype.Picklist_MultiSelect) && usedFields.Contains(f.Id)))
                {
                    // Field is deleted from Salesforce. Ignore this field from sync.
                    if (oApttusObject.Fields.Exists(f => f.Id == appDefPicklistField.Id))

                        // Update all occurences of Picklist across same App Object
                        foreach (var appDefObject in allAppDefObjectsWithPicklist.Where(a => a.Id == distinctObjectId))
                        {
                            // Sync #1 - Compare Regular Picklist and Multi-Select Picklist
                            if ((appDefPicklistField.Datatype == Datatype.Picklist || appDefPicklistField.Datatype == Datatype.Picklist_MultiSelect)
                                && (appDefPicklistField.PicklistType == PicklistType.None || appDefPicklistField.PicklistType == PicklistType.Regular || appDefPicklistField.PicklistType == PicklistType.TwoOption))
                            {
                                ApttusField newappDefPicklistField = oApttusObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id);
                                List<string> newPicklistValues = newappDefPicklistField.PicklistValues;

                                // TODO:: remove
                                //appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistValues = newPicklistValues;
                                appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistKeyValues = newappDefPicklistField.PicklistKeyValues;

                                // Sync if changes are found
                                if (!appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistValues.SequenceEqual(newPicklistValues))
                                {
                                    appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistValues = newPicklistValues;
                                    // For dynamics Sync picklist key value pairs
                                    appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistKeyValues = newappDefPicklistField.PicklistKeyValues;

                                    // Add App Change if it doesnt exist - In case same object is used in different hierarchies then we don't want it add it twice.
                                    if (!appChanges.Exists(ac => ac.ChangeItemType == ChangeItemType.Picklist && ac.ObjectId == appDefObject.Id &&
                                        ac.PicklistFieldId == appDefPicklistField.Id))
                                        appChanges.Add(new AppChange
                                        {
                                            ChangeItemType = ChangeItemType.Picklist, // Set ChangeItemType as Picklist for both "Regular" Picklist and "Multi-Select" Picklist
                                            ChangeType = QueryTypes.UPDATE,
                                            ObjectId = appDefObject.Id,
                                            PicklistFieldId = appDefPicklistField.Id
                                        });
                                }
                            }
                            // Sync #2 - Compare Dependent Picklist
                            if (appDefPicklistField.Datatype == Datatype.Picklist && appDefPicklistField.PicklistType == PicklistType.Dependent)
                            {
                                // #2.1 - First Check if the list of Picklist Values changed.
                                ApttusField newDependentPicklistField = oApttusObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id);

                                // Sync if changes are found
                                if (!appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistValues.SequenceEqual(newDependentPicklistField.PicklistValues))
                                {
                                    appDefObject.Fields.FirstOrDefault(f => f.Id == appDefPicklistField.Id).PicklistValues = newDependentPicklistField.PicklistValues;

                                    // Add App Change if it doesnt exist - In case same object is used in different hierarchies then we don't want it add it twice.
                                    if (!appChanges.Exists(ac => ac.ChangeItemType == ChangeItemType.Picklist && ac.ObjectId == appDefObject.Id &&
                                        ac.PicklistFieldId == appDefPicklistField.Id))
                                        appChanges.Add(new AppChange
                                        {
                                            ChangeItemType = ChangeItemType.DependentPicklist,
                                            ChangeType = QueryTypes.UPDATE,
                                            ObjectId = appDefObject.Id,
                                            PicklistFieldId = appDefPicklistField.Id
                                        });
                                }

                                // #2.2 - Secondly Sync all Dependent Picklists for Changes.
                                foreach (var newDependentPicklist in newDependentPicklistField.DependentPicklistValues)
                                {
                                    if (appDefPicklistField.DependentPicklistValues.Any(a => a.ControllingValue.Equals(newDependentPicklist.ControllingValue)))
                                    {
                                        // If the Controlling Value exists in App Def, Sync Picklist Values if changes are found
                                        if (!appDefPicklistField.DependentPicklistValues.FirstOrDefault(a => a.ControllingValue.Equals(newDependentPicklist.ControllingValue)).PicklistValues.SequenceEqual(newDependentPicklist.PicklistValues))
                                        {
                                            appDefPicklistField.DependentPicklistValues.FirstOrDefault(a => a.ControllingValue.Equals(newDependentPicklist.ControllingValue)).PicklistValues = newDependentPicklist.PicklistValues;

                                            // Add App Change if it doesnt exist - In case same object is used in different hierarchies then we don't want it add it twice.
                                            if (!appChanges.Exists(ac => ac.ChangeItemType == ChangeItemType.DependentPicklist && ac.ObjectId == appDefObject.Id &&
                                                ac.PicklistFieldId == appDefPicklistField.Id && ac.ControllingValue == newDependentPicklist.ControllingValue))

                                                appChanges.Add(new AppChange
                                                {
                                                    ChangeItemType = ChangeItemType.DependentPicklist,
                                                    ChangeType = QueryTypes.UPDATE,
                                                    ObjectId = appDefObject.Id,
                                                    PicklistFieldId = appDefPicklistField.Id,
                                                    ControllingValue = newDependentPicklist.ControllingValue
                                                });
                                        }
                                    }
                                    else
                                    {
                                        // If the Controlling Value does not exists in App Def, then add it to App Def
                                        appDefPicklistField.DependentPicklistValues.Add(newDependentPicklist);

                                        // Add App Change entry
                                        appChanges.Add(new AppChange
                                        {
                                            ChangeItemType = ChangeItemType.DependentPicklist,
                                            ChangeType = QueryTypes.INSERT,
                                            ObjectId = appDefObject.Id,
                                            PicklistFieldId = appDefPicklistField.Id,
                                            ControllingValue = newDependentPicklist.ControllingValue
                                        });
                                    }

                                }
                            }
                        }
                }

                // Sync #3 - Compare Record Type and Record Type Picklists
                if (singleAppDefObject.RecordTypes.Count > 0)
                {
                    // Sync if any new Record Types changes are found
                    //if (!(from rt in singleAppDefObject.RecordTypes select rt.Name).SequenceEqual(from rt in oApttusObject.RecordTypes select rt.Name))
                    //{
                    // 3.1. Find all new Record Types not current in App Definition - Add them to App Change (Insert) and App Definition
                    var newRecordTypeNames = (from rt in oApttusObject.RecordTypes
                                              select rt.Name)
                                          .Except
                                          (from rt in singleAppDefObject.RecordTypes
                                           select rt.Name);
                    if (newRecordTypeNames.Any())
                    {
                        List<ApttusRecordType> newRecordTypes = oApttusObject.RecordTypes.Where(rt => newRecordTypeNames.Contains(rt.Name)).ToList();

                        // 3.1.1 Add each picklist for each new record type in appChanges collection for Sync.
                        foreach (var newRecordType in newRecordTypes)
                            foreach (var newRecordTypePicklist in newRecordType.RecordTypePicklists)
                                appChanges.Add(new AppChange
                                {
                                    ChangeItemType = ChangeItemType.RecordType,
                                    ChangeType = QueryTypes.INSERT,
                                    ObjectId = singleAppDefObject.Id,
                                    PicklistFieldId = newRecordTypePicklist.PicklistFieldId,
                                    RecordType = newRecordType.Name
                                });

                        // 3.1.2 Update all occurences of the App Def object with new Record Types
                        foreach (var appDefObject in allAppDefObjectsWithPicklist.Where(a => a.Id == distinctObjectId))
                            appDefObject.RecordTypes.AddRange(newRecordTypes);
                    }

                    // 3.2. Find all unchanged Record Types currently present in App Defintion - Update App Change and App Definition
                    var unchangedRecordTypeNames = (from rt in oApttusObject.RecordTypes
                                                    select rt.Name)
                                                .Intersect
                                                (from rt in singleAppDefObject.RecordTypes
                                                 select rt.Name);
                    if (unchangedRecordTypeNames.Any())
                    {
                        foreach (string unchangedRecordTypeName in unchangedRecordTypeNames)
                        {
                            foreach (var appDefRecordType in singleAppDefObject.RecordTypes.Where(rt => rt.Name == unchangedRecordTypeName))
                            {
                                ApttusRecordType newRecordType = oApttusObject.RecordTypes.FirstOrDefault(rt => rt.Name == unchangedRecordTypeName);

                                // 3.2.1 Check for changes in the existing Record Type Picklists
                                foreach (var appDefRecordTypePicklist in appDefRecordType.RecordTypePicklists)
                                {
                                    // Check if the Record Type Picklist still exists in the new Record Type
                                    if (newRecordType.RecordTypePicklists.Exists(rtp => rtp.PicklistFieldId == appDefRecordTypePicklist.PicklistFieldId))
                                    {
                                        // Sync if changes are found
                                        if (!appDefRecordTypePicklist.PicklistValues.SequenceEqual(newRecordType.RecordTypePicklists.FirstOrDefault(rtp => rtp.PicklistFieldId == appDefRecordTypePicklist.PicklistFieldId).PicklistValues))
                                        {
                                            appChanges.Add(new AppChange
                                            {
                                                ChangeItemType = ChangeItemType.RecordType,
                                                ChangeType = QueryTypes.UPDATE,
                                                ObjectId = singleAppDefObject.Id,
                                                PicklistFieldId = appDefRecordTypePicklist.PicklistFieldId,
                                                RecordType = newRecordType.Name
                                            });

                                            // 3.1.2 Update all occurences of the App Def object with new Record Types
                                            foreach (var appDefObject in allAppDefObjectsWithPicklist.Where(a => a.Id == distinctObjectId))
                                                appDefObject.RecordTypes.FirstOrDefault(rt => rt.Name == unchangedRecordTypeName).RecordTypePicklists
                                                    .FirstOrDefault(rtp => rtp.PicklistFieldId == appDefRecordTypePicklist.PicklistFieldId).PicklistValues
                                                    = newRecordType.RecordTypePicklists.FirstOrDefault(rtp => rtp.PicklistFieldId == appDefRecordTypePicklist.PicklistFieldId).PicklistValues;
                                        }
                                    }
                                }

                                // 3.2.2 Add any new Record Type Picklists
                                var newRecordTypePicklistIds = (from rtp in newRecordType.RecordTypePicklists
                                                                select rtp.PicklistFieldId)
                                                                .Except
                                                                (from rtp in appDefRecordType.RecordTypePicklists
                                                                 select rtp.PicklistFieldId);
                                if (newRecordTypePicklistIds.Any())
                                {
                                    foreach (string newRecordTypePicklistId in newRecordTypePicklistIds)
                                    {
                                        ApttusRecordTypePicklist recordTypePicklist = (from rtp in newRecordType.RecordTypePicklists
                                                                                       where rtp.PicklistFieldId == newRecordTypePicklistId
                                                                                       select rtp).FirstOrDefault();
                                        appChanges.Add(new AppChange
                                        {
                                            ChangeItemType = ChangeItemType.RecordType,
                                            ChangeType = QueryTypes.INSERT,
                                            ObjectId = singleAppDefObject.Id,
                                            PicklistFieldId = newRecordTypePicklistId,
                                            RecordType = newRecordType.Name
                                        });

                                        // 3.1.2 Update all occurences of the App Def object with new Record Types
                                        foreach (var appDefObject in allAppDefObjectsWithPicklist.Where(a => a.Id == distinctObjectId))
                                            appDefObject.RecordTypes.FirstOrDefault(rt => rt.Name == newRecordType.Name).RecordTypePicklists.Add(recordTypePicklist);
                                    }
                                }
                            }
                        }
                        //}
                    }

                }
            }
            return appChanges;
        }

        /// <summary>
        /// Checks for dataype change or field deletion related information. If chnage found then returns true.
        /// </summary>
        /// <param name="singleAppDefObject"></param>
        /// <param name="synchedObject"></param>
        /// <param name="unsynchedField"></param>
        /// <returns></returns>
        private bool IsDataTypeChanged(ApttusObject singleAppDefObject, ApttusObject synchedObject, ApttusField unsynchedField)
        {
            bool bDatatypeChange = false;
            ApttusField synchedField = synchedObject.GetField(unsynchedField.Id);
            if (synchedField == null)
            {
                bDatatypeChange = true;
                blnConfigChanged = true;
                //ExceptionLogHelper.DebugLog("Field '" + unsynchedField.Name + "' of object '" + singleAppDefObject.Name + " may be Deleted from your Salseforce Org. Contact your Salseforce Administrator or remap the field in X-Author Designer");
            }
            else if (synchedField.Datatype != unsynchedField.Datatype)
            {
                blnConfigChanged = true;
                bDatatypeChange = true;
                //ExceptionLogHelper.DebugLog("Datatype of the field '" + unsynchedField.Name + "' of object '" + singleAppDefObject.Name + "' has been chnaged from '" + unsynchedField.Datatype + "' to '" + synchedField.Datatype + "'. Contact your Salseforce Administrator or remap the field in X-Author Designer");

            }
            return bDatatypeChange;
        }

        public void WritePicklistData(bool bSyncJustChanges, List<AppChange> appChanges)
        {
            try
            {
                ExcelHelper.ExcelApp.EnableEvents = false;

                // First step is to create the named range for Invalid Picklist.
                // This will be a static value which indicates Invalid Picklist selection for Dependent Picklist.
                metadataManager.CreateNamedRangeForInvalidPicklist();

                List<ApttusObject> allAppDefObjectsWithPicklist = new List<ApttusObject>();

                if (bSyncJustChanges)
                {
                    // If Sync Flag is passed then sync for only the objects which have changed.
                    allAppDefObjectsWithPicklist = (from appObject in appDefManager.GetFullHierarchyObjects(null)
                                                    from ac in appChanges
                                                    where ac.ObjectId == appObject.Id //&&
                                                    //(ac.ChangeItemType == ChangeItemType.Picklist || ac.ChangeItemType == ChangeItemType.DependentPicklist)
                                                    select appObject).ToList();
                }
                else
                {
                    // If Sync Flag is false then sync all objects with picklist - Done first time only
                    allAppDefObjectsWithPicklist = (from AppObject in appDefManager.GetFullHierarchyObjects(null)
                                                    from f in AppObject.Fields
                                                    where f.Datatype == Datatype.Picklist || f.Datatype == Datatype.Picklist_MultiSelect || f.Datatype == Datatype.Editable_Picklist
                                                    select AppObject).ToList();
                }

                List<string> distinctObjectIdsWithPicklist = (from AppObject in allAppDefObjectsWithPicklist
                                                              select AppObject.Id).Distinct().ToList();

                foreach (var DistinctObjectId in distinctObjectIdsWithPicklist)
                {
                    ApttusObject singleAppDefObject = allAppDefObjectsWithPicklist.FirstOrDefault(o => o.Id == DistinctObjectId);

                    // Get All AppFields may now return fields having different AppObjectID, for Sync purpose we go Object by Object
                    // so get only those fields whose AppObject matches with current Object being processed.
                    var otherObjectsWithSameId = (from AppObject in appDefManager.GetFullHierarchyObjects(null)
                                                  where AppObject.Id == singleAppDefObject.Id
                                                  select AppObject).ToList();

                    //Gets new fields from the object with same id as singleAppDefObject and adds to usedFields
                    List<string> newUsedFields = new List<string>();
                    foreach (ApttusObject appObject in otherObjectsWithSameId)
                        newUsedFields.AddRange(configManager.GetAllAppFields(appObject, true).Where(f => f.AppObject == appObject.UniqueId).Select(s => s.FieldId).ToList());

                    List<string> usedFields = newUsedFields.Distinct().ToList();

                    List<ApttusField> PicklistFieldsToWrite = new List<ApttusField>();
                    if (bSyncJustChanges)
                    {
                        // If Sync Flag is passed then sync for only the objects which have changed.
                        PicklistFieldsToWrite = (from f in singleAppDefObject.Fields
                                                 from ac in appChanges
                                                 where ac.ObjectId == singleAppDefObject.Id && ac.PicklistFieldId == f.Id &&
                                                     //(ac.ChangeItemType == ChangeItemType.Picklist || ac.ChangeItemType == ChangeItemType.DependentPicklist) && 
                                                 usedFields.Contains(f.Id)
                                                 select f).Distinct().ToList();
                    }
                    else
                    {
                        // If Sync Flag is false then sync all objects with picklist - Done first time only
                        PicklistFieldsToWrite = (from f in singleAppDefObject.Fields
                                                 where (f.Datatype == Datatype.Picklist || f.Datatype == Datatype.Picklist_MultiSelect || f.Datatype == Datatype.Editable_Picklist) &&
                                                 usedFields.Contains(f.Id)
                                                 select f).ToList();
                    }

                    foreach (ApttusField PicklistField in PicklistFieldsToWrite)
                    {
                        List<AppChange> currentPicklistAppChange = null;
                        if (bSyncJustChanges)
                            currentPicklistAppChange = (from ac in appChanges
                                                        where ac.ObjectId == singleAppDefObject.Id && ac.PicklistFieldId == PicklistField.Id
                                                        select ac).ToList();

                        WriteSingleObjectSinglePicklistData(DistinctObjectId, singleAppDefObject, usedFields, PicklistField, bSyncJustChanges, currentPicklistAppChange);
                    }
                }
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        private void WriteSingleObjectSinglePicklistData(string DistinctObjectId, ApttusObject SingleAppDefObject, List<string> UsedFields, ApttusField PicklistField, bool bSyncJustChanges, List<AppChange> appChanges)
        {
            AppChange appChange = null;

            switch (PicklistField.PicklistType)
            {
                case PicklistType.None:
                case PicklistType.Regular:
                case PicklistType.TwoOption:
                    if (bSyncJustChanges)
                        appChange = (from ac in appChanges
                                     where ac.ChangeItemType == ChangeItemType.Picklist
                                     select ac).FirstOrDefault();

                    WriteAndTrackSinglePicklistData(DistinctObjectId, string.Empty, PicklistField.Id,
                        DistinctObjectId + Constants.DOT + PicklistField.Id,
                        DistinctObjectId + Constants.DOT + PicklistField.Id, string.Empty, PicklistField.PicklistType, PicklistField.PicklistValues, appChanges == null ? null : appChange);

                    // The following conditions have to be true to execute Record Type based Picklist logic:
                    // 1. Record Type field should be present in the Apttus Object (in App Def).
                    // 2. Record Type field should be present in UsedFields collections (basically it should be in Dispaly or Save Other map).
                    // 3. At least one Record Type is specified for the Apttus Object.
                    // 4. Current Picklist is configured as a part of at least one Record Type.
                    if (bSyncJustChanges)
                    {
                        foreach (var ac in (from ac in appChanges
                                            where ac.ChangeItemType == ChangeItemType.RecordType &&
                                            ac.ObjectId == DistinctObjectId && ac.PicklistFieldId == PicklistField.Id
                                            select ac))
                        {
                            ApttusRecordTypePicklist recordTypePicklist = (from RecordType in SingleAppDefObject.RecordTypes
                                                                           from RecordTypePicklist in RecordType.RecordTypePicklists
                                                                           where RecordType.Name == ac.RecordType && RecordTypePicklist.PicklistFieldId == PicklistField.Id
                                                                           select RecordTypePicklist).FirstOrDefault();
                            WriteAndTrackSinglePicklistData(DistinctObjectId, ac.RecordType, PicklistField.Id,
                                       DistinctObjectId + Constants.DOT + ac.RecordType + Constants.DOT + PicklistField.Id,
                                       DistinctObjectId + Constants.DOT + ac.RecordType + Constants.DOT + PicklistField.Id, string.Empty, PicklistField.PicklistType, recordTypePicklist.PicklistValues, ac);
                        }
                    }
                    else
                    {
                        // If Sync flag is false, loop through all record types and the current picklist passed to the function.
                        ApttusField RecordTypeField = SingleAppDefObject.Fields.FirstOrDefault(f => f.RecordType);
                        if (RecordTypeField != null && UsedFields.Contains(RecordTypeField.Id) && SingleAppDefObject.RecordTypes.Count > 0
                            && (from RecordType in SingleAppDefObject.RecordTypes
                                from RecordTypePicklist in RecordType.RecordTypePicklists
                                where RecordTypePicklist.PicklistFieldId == PicklistField.Id
                                select RecordTypePicklist).Any())
                        {
                            foreach (ApttusRecordType apttusRecordType in (from RecordType in SingleAppDefObject.RecordTypes
                                                                           from RecordTypePicklist in RecordType.RecordTypePicklists
                                                                           where RecordTypePicklist.PicklistFieldId == PicklistField.Id
                                                                           select RecordType))
                            {
                                ApttusRecordTypePicklist recordTypePicklist = apttusRecordType.RecordTypePicklists.FirstOrDefault(rtp => rtp.PicklistFieldId == PicklistField.Id);

                                WriteAndTrackSinglePicklistData(DistinctObjectId, apttusRecordType.Name, PicklistField.Id,
                                   DistinctObjectId + Constants.DOT + apttusRecordType.Name + Constants.DOT + PicklistField.Id,
                                   DistinctObjectId + Constants.DOT + apttusRecordType.Name + Constants.DOT + PicklistField.Id, string.Empty, PicklistField.PicklistType, recordTypePicklist.PicklistValues, appChange);
                            }
                        }
                    }

                    break;

                case PicklistType.Dependent:

                    if (PicklistField.PicklistValues == null)
                        return;
                    // 1. Add Dependent Picklist as a regular picklist first - Single Entry
                    if (bSyncJustChanges)
                        appChange = (from ac in appChanges
                                     where ac.ChangeItemType == ChangeItemType.DependentPicklist && string.IsNullOrEmpty(ac.ControllingValue)
                                     select ac).FirstOrDefault();

                    WriteAndTrackSinglePicklistData(DistinctObjectId, string.Empty, PicklistField.Id,
                        DistinctObjectId + Constants.DOT + PicklistField.Id, DistinctObjectId + Constants.DOT + PicklistField.Id, string.Empty, PicklistField.PicklistType, PicklistField.PicklistValues, appChange);

                    // Sync Dependent 1.1
                    // Syncs Record type specific Dependent Picklist 

                    //Commenting the below piece of code and will it make it configurable in the future.
                    
                    ApttusObject oApttusObject = objectManager.GetApttusObject(SingleAppDefObject.Id, false, false);
                    ApttusField RecordTypeFieldDependent = SingleAppDefObject.Fields.FirstOrDefault(f => f.RecordType);
                    //Check if any record type exist in current object
                    if (RecordTypeFieldDependent != null && UsedFields.Contains(RecordTypeFieldDependent.Id) && SingleAppDefObject.RecordTypes.Count > 0
                        && (from RecordType in SingleAppDefObject.RecordTypes
                            from RecordTypePicklist in RecordType.RecordTypePicklists
                            where RecordTypePicklist.PicklistFieldId == PicklistField.Id
                            select RecordTypePicklist).Any())
                    {
                        // For every record type, gte the record type specific dependent picklist and assign only that picklist to it's parent picklist
                        foreach (ApttusRecordType apttusRecordType in (from RecordType in SingleAppDefObject.RecordTypes
                                                                       from RecordTypePicklist in RecordType.RecordTypePicklists
                                                                       where RecordTypePicklist.PicklistFieldId == PicklistField.Id
                                                                       select RecordType))
                        {
                            var DependentPicklistOfCurrentRecordType = (from ap in apttusRecordType.RecordTypePicklists
                                                                        where ap.PicklistFieldId == PicklistField.Id
                                                                        select ap).FirstOrDefault();

                            List<string> pickListsNotInRecordType = oApttusObject.Fields.Where(i => i.Id == PicklistField.Id).FirstOrDefault().PicklistValues.Except(DependentPicklistOfCurrentRecordType.PicklistValues).ToList();

                            foreach (var DependentPicklistItem in PicklistField.DependentPicklistValues)
                            {
                                WriteAndTrackSinglePicklistData(DistinctObjectId, apttusRecordType.Name, PicklistField.Id,
                                           DistinctObjectId + Constants.DOT + apttusRecordType.Name + Constants.DOT + PicklistField.Id + Constants.DOT + DependentPicklistItem.ControllingValue,
                                           DistinctObjectId + Constants.DOT + apttusRecordType.Name + Constants.DOT + PicklistField.Id + Constants.DOT + DependentPicklistItem.ControllingValue, 
                                           DependentPicklistItem.ControllingValue,
                                            PicklistField.PicklistType, DependentPicklistItem.PicklistValues.Except(pickListsNotInRecordType).ToList(), null);
                            }
                        }
                    }

                    int counter = 0;
                    // 2. Add Dependent Picklist - Multiple Entries
                    foreach (var DependentPicklistItem in PicklistField.DependentPicklistValues)
                    {
                        string controllingValue = Utils.ContainsUnicodeCharacter(DependentPicklistItem.ControllingValue) ? "PV" + counter++.ToString() : DependentPicklistItem.ControllingValue;
                        
                        if (bSyncJustChanges)
                        {
                            appChange = (from ac in appChanges
                                         where ac.ChangeItemType == ChangeItemType.DependentPicklist && ac.ControllingValue == DependentPicklistItem.ControllingValue
                                         select ac).FirstOrDefault();

                            // Update only the dependent picklists which are present in app changes.
                            if (appChange != null)
                                WriteAndTrackSinglePicklistData(DistinctObjectId, string.Empty, PicklistField.Id,
                                    DistinctObjectId + Constants.DOT + PicklistField.Id + Constants.DOT + controllingValue,
                                    DistinctObjectId + Constants.DOT + PicklistField.Id + Constants.DOT + DependentPicklistItem.ControllingValue, DependentPicklistItem.ControllingValue,
                                     PicklistField.PicklistType, DependentPicklistItem.PicklistValues, appChange);
                        }
                        else
                            WriteAndTrackSinglePicklistData(DistinctObjectId, string.Empty, PicklistField.Id,
                                DistinctObjectId + Constants.DOT + PicklistField.Id + Constants.DOT + controllingValue,
                                DistinctObjectId + Constants.DOT + PicklistField.Id + Constants.DOT + DependentPicklistItem.ControllingValue, DependentPicklistItem.ControllingValue,
                                 PicklistField.PicklistType, DependentPicklistItem.PicklistValues, null);
                    }
                    break;

                default:
                    break;
            }
        }

        private void WriteAndTrackSinglePicklistData(string ObjectId, string recordType, string FieldId, string NamedRange, string formulaNamedRange, string controllingValue, PicklistType PicklistType,
            List<string> PicklistValues, AppChange appChange)
        {
            // Create or Find a Picklist Tracker Entry
            PicklistTrackerEntry Tracker = null;
            bool picklistTrackerExists = (from pt in dataManager.PicklistTracker
                       where pt.ObjectId == ObjectId && pt.FieldId == FieldId &&
                       pt.RecordType == recordType && pt.FormulaNamedRange == NamedRange // FormulaNamedRange is unchanged so compare that with NamedRange
                       select pt).Any();

            // If tracker already exists for ObjectId, fieldId and Named range combination, means picklist is already written in metdata Sheet.
            // In case of AppChange not null, meaning picklist values might have change, so proceed with picklist write in apttusmetadatasheet.
            if (picklistTrackerExists && appChange == null)
                return;

            if (appChange == null || (appChange != null && appChange.ChangeType == QueryTypes.INSERT))
            {
                Tracker = new PicklistTrackerEntry
                {
                    ObjectId = ObjectId,
                    RecordType = recordType,
                    FieldId = FieldId,
                    NamedRange = Regex.Replace(NamedRange, @"[^a-zA-Z0-9_.]", Constants.UNDERSCORE),
                    FormulaNamedRange = formulaNamedRange,
                    ControllingValue = controllingValue,
                    AbsoluteColumnNo = GetNextColumnNo(),
                    Type = PicklistType
                };
            }
            else
            {

                Tracker = (from pt in dataManager.PicklistTracker
                           where pt.ObjectId == appChange.ObjectId && pt.FieldId == appChange.PicklistFieldId &&
                           pt.RecordType == recordType && pt.FormulaNamedRange == NamedRange // FormulaNamedRange is unchanged so compare that with NamedRange
                           select pt).FirstOrDefault();

            }

            // Perform actual write in apttus metadata       
            metadataManager.WritePicklistData(PicklistValues, Tracker);

            // Add the tracker to Collection
            dataManager.AddPicklistTracker(Tracker);

        }

        private int GetNextColumnNo()
        {
            int result = Constants.STARTCOLUMN_PICKLISTDATA;
            if (dataManager.PicklistTracker.Count > 0)
                result = (from pt in dataManager.PicklistTracker
                          select pt.AbsoluteColumnNo).Max() + 1;
            return result;
        }

        public void PrepRuntimeExcel()
        {
            List<RangeMap> allRanges = configManager.GetRangeMaps();

            // 1. Apply Data Validation for Independent cells
            foreach (var range in allRanges.Where(r => r.Type == ObjectType.Independent))
            {
                // Fetch the corresponding Map
                if (!range.RetrieveMapId.Equals(Guid.Empty))
                {
                    RetrieveMap rm = configManager.GetRetrieveMapbyTargetNamedRange(range.RangeName);
                    RetrieveField rf = configManager.GetRetrieveFieldbyTargetNamedRange(range.RangeName);
                    ApttusObject currentAppObject = appDefManager.GetAppObject(rf.AppObject);
                    List<RetrieveField> currentObjectFields = (from f in rm.RetrieveFields
                                                               where f.Type == ObjectType.Independent &&
                                                               f.AppObject == rf.AppObject && fieldLevelSecurity.IsFieldVisible(f.AppObject, f.FieldId)
                                                               select f).ToList();

                    RetrieveHelper.PopulateCellIndependent(null, appDefManager.GetAppObject(rf.AppObject), currentObjectFields, rf, false);
                }
                else if (!range.SaveMapId.Equals(Guid.Empty))
                {
                    SaveMap sm = configManager.GetSaveMapbyTargetNamedRange(range.RangeName);
                    SaveField sf = configManager.GetSaveFieldbyTargetNamedRange(range.RangeName);
                    ApttusObject currentAppObject = appDefManager.GetAppObject(sf.AppObject);
                    List<SaveField> currentObjectFields = (from f in sm.SaveFields
                                                           where f.Type == ObjectType.Independent &&
                                                           f.AppObject == sf.AppObject
                                                           select f).ToList();

                    RetrieveHelper.PopulateCellIndependent(null, appDefManager.GetAppObject(sf.AppObject), currentObjectFields, sf, false);
                }
            }

            // 2. Hide the Formula row for repeating cells
            RetrieveHelper.UpdateNamedRangeVisibility(true);

            // 3. Create Dataset and Tracker entries for Pre-Loaded Data
            var preLoadedSaveGroups = (from sm in configManager.SaveMaps
                                       from sg in sm.SaveGroups
                                       where sg.LoadedRows > 0
                                       select sg);

            foreach (var sg in preLoadedSaveGroups)
            {
                SaveMap preLoadedSaveMap = configManager.GetSaveMapbyTargetNamedRange(sg.TargetNamedRange);
                InitializeDatasetAndTrackers(preLoadedSaveMap, sg);

                // Get all SaveFields for Current Save Group
                List<SaveField> sgSaveFields = preLoadedSaveMap.SaveFields.Where(sf => sf.GroupId.Equals(sg.GroupId)).ToList();

                // If there is Picklist field in Preloaded grid populate it
                foreach (SaveField sf in sgSaveFields)
                {
                    ApttusField saveField = appDefManager.GetField(sf.AppObject, sf.FieldId);
                    if (saveField.Datatype == Datatype.Picklist)
                    {
                        ApttusObject currentFieldObject = appDefManager.GetAppObject(sf.AppObject);

                        List<RetrieveField> repeatingCellFields = (from f in preLoadedSaveMap.SaveFields
                                                                   where f.GroupId.Equals(sg.GroupId)
                                                                   select new RetrieveField
                                                                   {
                                                                       AppObject = f.AppObject,
                                                                       FieldId = f.FieldId,
                                                                       Type = f.Type,
                                                                       TargetLocation = f.DesignerLocation,
                                                                       TargetNamedRange = f.TargetNamedRange,
                                                                       TargetColumnIndex = f.TargetColumnIndex,
                                                                       DataType = saveField.Datatype
                                                                   }).ToList();

                        RetrieveField repeatingCellField = repeatingCellFields.Where(f => f.FieldId.Equals(saveField.Id)).FirstOrDefault();

                        Excel.Range repeatingRange = ExcelHelper.GetRange(sg.TargetNamedRange);

                        RetrieveHelper.PopulateCellRepeating(currentFieldObject, saveField, repeatingCellFields, repeatingCellField, repeatingRange, sg.LoadedRows);
                    }
                }

                // Do Protect after picklist validations are added
                ClearAndProtectIdColumn(sg);
            }

        }

        /// <summary>
        /// X-Author creates Named ranges using PickList Options, but in case of picklist option having special characters it would fail.
        /// We now replace all special chars with "_", but that break dependent picklist logic.
        /// So maintain Key Value pair of original value vs cleansed value and use it in Excel VLookUp formula to correct it.
        /// </summary>
        public void WritePicklistNamedRangePair()
        {
            try
            {
                ExcelHelper.ExcelApp.EnableEvents = false;

                List<PicklistTrackerEntry> allPicklists = new List<PicklistTrackerEntry>();
                List<PicklistTrackerEntry> dependentPickLists = dataManager.PicklistTracker.Where(p => p.Type == PicklistType.Dependent).ToList();
                List<PicklistTrackerEntry> recordTypePickLists = dataManager.PicklistTracker.Where(p => !string.IsNullOrEmpty(p.RecordType)).ToList();
                allPicklists.AddRange(dependentPickLists);
                allPicklists.AddRange(recordTypePickLists);

                object[,] cellData = new object[allPicklists.Count, 2];
                for (int i = 0; i < allPicklists.Count; i++)
                {
                    if (i < dependentPickLists.Count)
                    {
                        if ((!string.IsNullOrEmpty(allPicklists[i].ControllingValue)) && allPicklists[i].FormulaNamedRange.Contains(allPicklists[i].ControllingValue))
                        {
                            int lastIndex = allPicklists[i].FormulaNamedRange.LastIndexOf(allPicklists[i].ControllingValue);

                            cellData[i, 0] = allPicklists[i].FormulaNamedRange.Substring(lastIndex);
                            cellData[i, 1] = allPicklists[i].NamedRange.Substring(lastIndex);
                        }
                        else
                        {
                            int lastIndex = allPicklists[i].NamedRange.LastIndexOf(".") + 1;
                            int dataLength = allPicklists[i].NamedRange.Length - lastIndex;

                            cellData[i, 0] = allPicklists[i].FormulaNamedRange.Substring(lastIndex, dataLength);
                            cellData[i, 1] = allPicklists[i].NamedRange.Substring(lastIndex, dataLength);
                        }
                    }
                    else if (i >= dependentPickLists.Count && i < allPicklists.Count)
                    {
                        cellData[i, 0] = allPicklists[i].RecordType;
                        cellData[i, 1] = Regex.Replace(allPicklists[i].RecordType, @"[^a-zA-Z0-9_.]", Constants.UNDERSCORE);
                    }
                }

                metadataManager.WritePicklistNamedRangePair(cellData, allPicklists.Count);
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
            }
        }

        private void InitializeDatasetAndTrackers(SaveMap saveMap, SaveGroup saveGroup)
        {
            ApttusObject apttusObject = appDefManager.GetAppObject(saveGroup.AppObject);

            // 1. Get all fields (retrieve +  save field)
            HashSet<string> allFields = new HashSet<string> { apttusObject.IdAttribute };

            // 1.1 Add Save Fields

            foreach (var saveField in saveMap.SaveFields.Where(sf => sf.GroupId == saveGroup.GroupId))
                allFields.Add(saveField.FieldId);

            // 1.2 Add Retrieve Fields
            var repeatingGroup = configManager.GetRepeatingGroupbyTargetNamedRange(saveGroup.TargetNamedRange);
            if (repeatingGroup != null)
                foreach (var retrieveField in repeatingGroup.RetrieveFields.Where(rf => fieldLevelSecurity.IsFieldVisible(rf.AppObject, rf.FieldId)))
                    allFields.Add(retrieveField.FieldId);


            // 2. Create an ApttusDataSet with blank rows
            string dataSetName = appDefManager.GetAppObject(saveGroup.AppObject).Name + "_" + DateTime.Now.ToString("hhmmssff") + "_Preloaded";
            DataTable datatable = new DataTable();

            // 2.1 Add all the datatable columns. set typeof(double) for Double and Decimanl datatypes
            foreach (string field in allFields)
            {
                ApttusField apttusField = apttusObject.Fields.Where(f => f.Id.Equals(field)).FirstOrDefault();
                DataColumn dc = new DataColumn(field);
                dc.DataType = apttusField.Datatype == Datatype.Decimal || apttusField.Datatype == Datatype.Double ? typeof(double) : typeof(string);
                datatable.Columns.Add(dc);
            }

            // 2.2 Insert blank rows
            for (int i = 0; i < saveGroup.LoadedRows; i++)
            {
                DataRow dr = datatable.NewRow();
                datatable.Rows.Add(dr);
            }

            // 2.3 Create dataset
            ApttusDataSet dataSet = new ApttusDataSet
            {
                Name = dataSetName,
                AppObjectUniqueID = saveGroup.AppObject,
                DataTable = datatable
            };

            // 2.4 Add the new dataset to data manager. Retrieve the newly added dataset which should contain the Id.
            dataManager.AddData(dataSet);
            dataSet = dataManager.AppData.Where(d => d.Name.Equals(dataSet.Name)).FirstOrDefault();

            // 2.5 Assign parent dataset.
            if (dataSet.Parent == Guid.Empty)
                dataSet.Parent = dataManager.GetParentDataSetId(saveGroup.AppObject);


            // 3. Add the named range data tracker entry for repeating cells
            dataManager.AddDataTracker(new ApttusDataTracker
            {
                Location = saveGroup.TargetNamedRange,
                DataSetId = dataSet.Id,
                Type = ObjectType.Repeating
            });
        }

        private void ClearAndProtectIdColumn(SaveGroup saveGroup)
        {
            WorkbookProtectionManager workbookProtectionManager = WorkbookProtectionManager.GetInstance;
            Excel.Range oRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);
            workbookProtectionManager.ProtectRows(saveGroup.TargetNamedRange, oRange.Cells[3, 1], saveGroup.LoadedRows);
        }

        public void CreateObjectsWithoutLookupName()
        {
            List<ApttusObject> appObjects = appDefManager.GetAllObjects();
            foreach (ApttusObject appObject in appObjects)
            {
                if (appObject.Fields.Any(f => !f.Id.Equals(Constants.NAME_ATTRIBUTE) || (f.NameField && !f.Id.Equals(Constants.NAME_ATTRIBUTE))))
                {
                    if (!Constants.ObjectsWithoutLookupName.Exists(f => f.Equals(appObject.Id)))
                        Constants.ObjectsWithoutLookupName.Add(appObject.Id);
                }
            }

        }
    }
}
