using Apttus.XAuthor.Core;
using System;
using System.Linq;

namespace Apttus.XAuthor.AppDesigner
{
    public class LookAheadFieldMapperHelper
    {
        private FieldMapper fldMapper = null;
        ApplicationDefinitionManager appdefManager = ApplicationDefinitionManager.GetInstance;
        public Guid AppObjectUniqueId { get; set; }
        //ctor
        public LookAheadFieldMapperHelper(FieldMapper fieldMapper)
        {
            fldMapper = fieldMapper;
            // set the guid so that the guidAppObject property can be
            // called n times. 
            SetGuidofAppObject();

            //
            if (AppObjectUniqueId == Guid.Empty)
            {
                SetGuidofTaskAndEventObject();
            }
        }

        //This function needs to removed, the moment we add support for multi-object lookup.
        private void SetGuidofTaskAndEventObject()
        {
            if (fldMapper is RetrieveFieldMapper retrieveFieldMapper)
            {
                Guid targetAppObjectId = Guid.Empty;
                if (retrieveFieldMapper == null)
                    return;

                RepeatingGroup currentRepGroup = (from repGroup in retrieveFieldMapper.MappedRetrieveMap.RepeatingGroups
                                                  from retField in repGroup.RetrieveFields
                                                  where retField.TargetNamedRange.Equals((retrieveFieldMapper.MappedRetrieveField.TargetNamedRange))
                                                  select repGroup).FirstOrDefault();

                // If currentRepGroup is null, check if field is Individual
                if (currentRepGroup == null)
                    targetAppObjectId = appdefManager.GetAppObject(retrieveFieldMapper.MappedRetrieveField.AppObject).UniqueId;
                else
                    targetAppObjectId = currentRepGroup.AppObject;

                ApttusObject obj = appdefManager.GetAppObject(targetAppObjectId);
                if (obj != null && (obj.Id.Equals("Task") || obj.Id.Equals("Event")))
                {
                    if (retrieveFieldMapper.MappedRetrieveField.FieldId.Equals("WhatId")
                        || retrieveFieldMapper.MappedRetrieveField.FieldId.Equals("WhoId")
                        || retrieveFieldMapper.MappedRetrieveField.FieldId.Equals("What.Name")
                        || retrieveFieldMapper.MappedRetrieveField.FieldId.Equals("Who.Name"))
                    {
                        AppObjectUniqueId = targetAppObjectId;
                    }
                }
            }
        }
        private void SetGuidofAppObject()
        {
            if (fldMapper != null)
            {
                if (fldMapper is RetrieveFieldMapper retrieveFieldMapper)
                {
                    if (retrieveFieldMapper.MappedRetrieveField.FieldId.EndsWith(Constants.APPENDLOOKUPID) || retrieveFieldMapper.MappedRetrieveField.DataType == Datatype.Lookup)
                        AppObjectUniqueId = GetLookupObjectUniqueId(fldMapper, retrieveFieldMapper.MappedRetrieveField.DataType);
                    else
                        AppObjectUniqueId = retrieveFieldMapper.MappedRetrieveField.AppObject;

                    return;
                }
                else if (fldMapper is SaveFieldMapper saveFieldMapper)
                {
                    ApttusObject currentObject = appdefManager.GetAppObject(saveFieldMapper.MappedSaveField.AppObject);
                    ApttusField currentField = currentObject.Fields.Where(f => f.Id.Equals(saveFieldMapper.MappedSaveField.FieldId)).FirstOrDefault();

                    if (saveFieldMapper.MappedSaveField.FieldId.EndsWith(Constants.APPENDLOOKUPID) || currentField.Datatype == Datatype.Lookup)
                        AppObjectUniqueId = GetLookupObjectUniqueId(fldMapper, currentField.Datatype);
                    else
                        AppObjectUniqueId = saveFieldMapper.MappedSaveField.AppObject;

                    return;
                }
            }
            // return null
            AppObjectUniqueId = Guid.Empty;
        }

        /// <summary>
        /// In case of Lookahead is being configured on Lookupfields, get lookupobject of those fields
        /// </summary>
        /// <param name="fldMapper"></param>
        /// <returns></returns>
        private Guid GetLookupObjectUniqueId(FieldMapper fldMapper, Datatype dataType)



        {
            Guid returnGuid = Guid.Empty;

            string lookupIdField = string.Empty;
            ApttusObject lookAheadFieldAppObject = null;

            if (fldMapper is RetrieveFieldMapper retrieveFieldMapper)
            {
                // Handle case of Display Map enhancement, where fields of Multiple object from Hierarchy can be part of one Display Map
                // Here get appropriate Object Unique Id to be used for Search & Select Lookahead.
                RepeatingGroup currentRepGroup = (from repGroup in retrieveFieldMapper.MappedRetrieveMap.RepeatingGroups
                                                  from retField in repGroup.RetrieveFields
                                                  where retField.TargetNamedRange.Equals(retrieveFieldMapper.MappedRetrieveField.TargetNamedRange)
                                                  select repGroup).FirstOrDefault();


                // if field is LookupId field use field itself as lookfield, else get lookupID field
                lookupIdField = dataType == Datatype.Lookup ? retrieveFieldMapper.MappedRetrieveField.FieldId :
                                                appdefManager.GetLookupIdFromLookupName(retrieveFieldMapper.MappedRetrieveField.FieldId);


                // if the target object is indenpendent 
                // need to find out the filed 
                if (currentRepGroup == null)
                {
                    lookAheadFieldAppObject = appdefManager.GetAppObject(retrieveFieldMapper.MappedRetrieveField.AppObject);

                }
                else if (retrieveFieldMapper.MappedRetrieveField.AppObject == currentRepGroup.AppObject)    // If appObject of retrieve field is different than of repeating group , 
                                                                                                            // use Repeating group app object as Repeating group app object is the base object         
                    lookAheadFieldAppObject = appdefManager.GetAppObject(retrieveFieldMapper.MappedRetrieveField.AppObject);
                else
                    lookAheadFieldAppObject = appdefManager.GetAppObject(currentRepGroup.AppObject);
            }
            else if (fldMapper is SaveFieldMapper saveFieldMapper)
            {
                lookupIdField = dataType == Datatype.Lookup ? saveFieldMapper.MappedSaveField.FieldId :
                                                appdefManager.GetLookupIdFromLookupName(saveFieldMapper.MappedSaveField.FieldId);
                lookAheadFieldAppObject = appdefManager.GetAppObject(saveFieldMapper.MappedSaveField.AppObject);
            }

            ApttusField lookupField = lookAheadFieldAppObject.Fields.Where(f => f.Id == lookupIdField).FirstOrDefault();

            if (lookupField != null)
            {
                ApttusObject lookupObject = appdefManager.GetAppObjectById(lookupField.LookupObject.Id).FirstOrDefault();
                returnGuid = lookupObject != null ? lookupObject.UniqueId : Guid.Empty;
            }
            return returnGuid;
        }
    }
}
