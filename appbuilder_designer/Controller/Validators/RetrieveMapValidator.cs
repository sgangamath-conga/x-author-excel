/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class RetrieveMapValidator : IValidator
    {
        private static RetrieveMapValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public static RetrieveMapValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new RetrieveMapValidator();
                        configManager = ConfigurationManager.GetInstance;
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        public List<ValidationResult> Validate<T>(T map)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<RetrieveMap> maps = new List<RetrieveMap>();

            if (map == null)
                maps = ConfigurationManager.GetInstance.RetrieveMaps;
            else
                maps = new List<RetrieveMap> {(map as RetrieveMap)};

            foreach (RetrieveMap rm in maps)
            {
                // 1. Validate Independent
                ValidationResult resultInd = ValidateIndependent(rm);
                if (resultInd != null)
                    results.Add(resultInd);

                // 2. Validate Repeating
                ValidationResult resultRep = ValidateRepeatingGroup(rm);
                if (resultRep != null)
                    results.Add(resultRep);

                // 3. Validate CrossTab
            }

            return results;
        }

        internal static ValidationResult ValidateIndependent(RetrieveMap rMap)
        {
            ValidationResult result = null;

            foreach (RetrieveField rField in rMap.RetrieveFields)
            {
                //Change the Designer Location.
                rField.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(rField.TargetNamedRange));
            }

            return result;
        }

        internal static ValidationResult ValidateRepeatingGroup(RetrieveMap rMap)
        {
            ValidationResult result = null;

            List<ValidationResult> Documentresult = DocumentObjectValidator.GetInstance.Validate(rMap);
             
            if (Documentresult.Count > 0)
            {
                return Documentresult.FirstOrDefault();
            }

            foreach (RepeatingGroup repeatingGroup in rMap.RepeatingGroups)
            {
                bool bVerticalRendering = repeatingGroup.Layout.Equals("Vertical");
                string objectName = ApplicationDefinitionManager.GetInstance.GetAppObject(repeatingGroup.AppObject).Name;

                ////Below If condition is to check for document object's Body field
                ////If body field is present, Name and Type field is necessary because body is base64
                //if (objectName.Equals(Constants.DOCUMENT_OBJECTID))
                //{
                //    if (repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals(Constants.DOCUMENT_BODY)))
                //    {
                //        if (!repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals(Constants.DOCUMENT_TYPE)) || !repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals(Constants.NAME_ATTRIBUTE)))
                //        {
                //            result = new ValidationResult();
                //            result.EntityName = rMap.Name;
                //            result.EntityType = EntityType.DisplayMap;
                //            String msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateDocumentRepeatingGroup_ErrMsg"), Constants.DOCUMENT_TYPE_FIELD_NAME, Constants.DOCUMENT_NAME_FIELD_NAME, objectName, Constants.DISPLAYMAP_NAME, Constants.DOCUMENT_BODY);
                //            if (result.Messages == null)
                //                result.Messages = new List<string>();
                //            result.Messages.Add(msg);
                //            result.Success = false;
                //            break;
                //        }
                //    }
                //}

                foreach (RetrieveField rField in repeatingGroup.RetrieveFields)
                {
                    int TargetIndex = -1;
                    if (bVerticalRendering)
                        TargetIndex = ExcelHelper.GetColumnIndex(repeatingGroup.TargetNamedRange, rField.TargetNamedRange);
                    else
                        TargetIndex = ExcelHelper.GetRowIndex(repeatingGroup.TargetNamedRange, rField.TargetNamedRange);

                    if (TargetIndex < 0)
                    {
                        result = new ValidationResult();
                        result.EntityName = rMap.Name;
                        result.EntityType = EntityType.DisplayMap;
                        String msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateRepeatingGroup_ErrMsg"), rField.FieldName, repeatingGroup.ObjectName);
                        if (result.Messages == null)
                            result.Messages = new List<string>();
                        result.Messages.Add(msg);
                        result.Success = false;
                        break;
                    }

                    //Change the Designer Location.
                    rField.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(rField.TargetNamedRange));

                    //Update the Target Index
                    rField.TargetColumnIndex = TargetIndex;
                }
            }
            return result;
        }

        private List<ValidationResult> IsSaveFieldInRetrieveMap(RetrieveMap rMap)
        {
            List<ValidationResult> results = IsIndependentSaveFieldInRetrieveMap(rMap);
            results.AddRange(IsRepeatingSaveFieldInRetrieveMap(rMap));
            return results;
        }

        private List<ValidationResult> IsIndependentSaveFieldInRetrieveMap(RetrieveMap rMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();           

            foreach (SaveMap saveMap in configManager.SaveMaps)
            {
                List<SaveField> saveFieldsInSaveGroup = saveMap.SaveFields.Where(field => field.Type == ObjectType.Independent).ToList();

                List<SaveField> saveFields = (from sf in saveFieldsInSaveGroup
                                              from rf in rMap.RetrieveFields
                                              where sf.FieldId.Equals(rf.FieldId)
                                              select sf).ToList();
                if (saveFields.Count != 0)
                {
                    ValidationResult result = new ValidationResult();
                    result.Success = true;
                    result.EntityName = rMap.Name;
                    result.EntityType = EntityType.DisplayMap;

                    //There are savefields for this retrieve map.
                    if (result.Messages == null)
                        result.Messages = new List<string>();

                    result.Success = false;

                    StringBuilder errorBuilder = new StringBuilder(rMap.Name).Append(resourceManager.GetResource("COMMON_RetrieveMapHas_Text"));
                    foreach (SaveField sf in saveFields)
                        errorBuilder.Append(sf.FieldId).Append(", ");
                    errorBuilder.Append(String.Format(resourceManager.GetResource("COMMON_PartOf_Msg"),saveMap.Name));

                    result.Messages.Add(errorBuilder.ToString());

                    results.Add(result);
                }
            }
            return results;
        }

        private List<ValidationResult> IsRepeatingSaveFieldInRetrieveMap(RetrieveMap rMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            foreach (SaveMap saveMap in configManager.SaveMaps)
            {
                foreach (SaveGroup saveGroup in saveMap.SaveGroups)
                {
                    RepeatingGroup repeatingGroup = rMap.RepeatingGroups.Where(repGroup => repGroup.TargetNamedRange.Equals(saveGroup.TargetNamedRange)
                                                                                && repGroup.AppObject.Equals(saveGroup.AppObject)
                                                                                && repGroup.Layout.Equals(saveGroup.Layout)).FirstOrDefault();
                    //If repeatingGroup is null which means this retrieve map is not part of this save group in this save map.
                    if (repeatingGroup != null)
                    {
                        ValidationResult result = new ValidationResult();
                        result.Success = false;
                        result.EntityType = EntityType.DisplayMap;
                        result.EntityName = rMap.Name;

                        if (result.Messages == null)
                            result.Messages = new List<string>();

                        //Get all the fields of this save map
                        List<SaveField> saveFields = saveMap.SaveFields.Where(saveField => saveField.GroupId.Equals(saveGroup.GroupId) && saveField.Type == ObjectType.Repeating).ToList();

                        StringBuilder errorBuilder = new StringBuilder(rMap.Name).Append(resourceManager.GetResource("COMMON_RetrieveMapHas_Text"));
                        foreach (SaveField sf in saveFields)
                            errorBuilder.Append(sf.FieldId).Append(", ");

                        errorBuilder.Append(String.Format(resourceManager.GetResource("COMMON_PartOf_Msg"), saveMap.Name));
                        result.Messages.Add(errorBuilder.ToString());

                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            RetrieveMap retrieveMap = t as RetrieveMap;

            List<ValidationResult> results = new List<ValidationResult>();           

            List<Core.RetrieveActionModel> retrieveActions = ValidationManager.GetRetrieveActionsForRetrieveMap(retrieveMap);

            //1. Are there any save fields referring to this retrieve map.
            results.AddRange(IsSaveFieldInRetrieveMap(retrieveMap));
            
            //2. There exists a retreive action which uses this retrieve map.
            //Build Error Message and let the user know which retrieve action(s) are using this RetrieveMap.
            if (retrieveActions.Count != 0)
            {
                ValidationResult result = new ValidationResult();
                result.EntityName = retrieveMap.Name;
                result.EntityType = EntityType.DisplayMap;
                result.Success = false;

                StringBuilder errorMsgBuilder = new StringBuilder(retrieveMap.Name);
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_IsUsedBy_Msg"));

                bool bAddComma = false;

                foreach (Core.RetrieveActionModel retreiveAction in retrieveActions)
                {
                    if (bAddComma)
                        errorMsgBuilder.Append(", ");
                    errorMsgBuilder.Append(retreiveAction.Name);
                    bAddComma = true;
                }
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_RetrieveACTValidate_Msg"));

                result.Messages = new List<string>();
                result.Messages.Add(errorMsgBuilder.ToString());

                results.Add(result);
            }
            return results;
        }

        public ValidationResult ValidateCrossTab(List<CrossTabRetrieveMap> crMaps)
        {
            ValidationResult result = null;

            foreach (CrossTabRetrieveMap crMap in crMaps)
            {
                //Change the Designer Location.
                crMap.ColField.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(crMap.ColField.TargetNamedRange));
                crMap.RowField.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(crMap.RowField.TargetNamedRange));
                crMap.DataField.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(crMap.DataField.TargetNamedRange));

                crMap.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(crMap.DataField.TargetNamedRange));
            }

            return result;
        }

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
