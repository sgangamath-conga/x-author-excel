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
    public class SaveMapValidator : IValidator
    {
        private static SaveMapValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static SaveMapValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new SaveMapValidator();
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
        public List<ValidationResult> Validate<T>(T SaveMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<SaveMap> maps = new List<SaveMap>();

            if (SaveMap == null)
                maps = ConfigurationManager.GetInstance.SaveMaps;
            else
                maps = new List<SaveMap> { (SaveMap as SaveMap) };

            foreach (SaveMap saveMap in maps)
            {
                List<ValidationResult> saveMapResults = ValidateSaveMap(saveMap);

                // Add ValidationResult to result collection on validation failure.
                if (saveMapResults.Count > 0)
                    results.AddRange(saveMapResults);
            }

            return results;
        }
        private ValidationResult ValidateDocumentObject(string ObjectName,string SaveMapName, List<SaveField> saveGroupFields)
        {
            ValidationResult result = new ValidationResult { Messages = new List<string>(), Success = true };
            //Below If condition is to check for document object's Body field
            //If body field is present, Name and Type field is necessary because body is base64
            if (ObjectName.Equals(Constants.DOCUMENT_OBJECTID))
            {
                if (saveGroupFields.Exists(f => f.FieldId.Equals(Constants.DOCUMENT_BODY)))
                {
                    if (!saveGroupFields.Exists(f => f.FieldId.Equals(Constants.DOCUMENT_TYPE)) || !saveGroupFields.Exists(f => f.FieldId.Equals(Constants.NAME_ATTRIBUTE)))
                    {
                        result = new ValidationResult();
                        result.EntityName = SaveMapName;
                        result.EntityType = EntityType.SaveMap;
                        String msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateDocumentRepeatingGroup_ErrMsg"), Constants.DOCUMENT_TYPE_FIELD_NAME, Constants.DOCUMENT_NAME_FIELD_NAME, ObjectName, Constants.SAVEMAP_NAME, Constants.DOCUMENT_BODY);
                        if (result.Messages == null)
                            result.Messages = new List<string>();
                        result.Messages.Add(msg);
                        result.Success = false;
                    }
                }
            }
            return result;
        }

        private bool IsRelationalObjectIdAttributeExist(ApttusObject appObject, RepeatingGroup currentRepeatingGroup)
        {
            bool isExists = false;
            if (CRMContextManager.Instance.ActiveCRM != CRM.DynamicsCRM)
                return isExists;


            List<RetrieveField> appObjectRetrieveFields = null;
            if (currentRepeatingGroup.RetrieveFields.Exists(f => f.AppObject.Equals(appObject.UniqueId)))
                appObjectRetrieveFields = currentRepeatingGroup.RetrieveFields.Where(f => f.AppObject.Equals(appObject.UniqueId)).ToList();

            if (appObjectRetrieveFields.Exists(f => f.FieldId.Equals(appObject.IdAttribute)))
                isExists = true;
            else if (appObjectRetrieveFields.Exists(f => f.FieldId.Contains(Constants.DOT)))
            {
                List<string> filteredRtrvFields = appObjectRetrieveFields.Where(f => f.FieldId.Contains(Constants.DOT)).Select(f => f.FieldId).ToList();
                foreach (string item in filteredRtrvFields)
                {
                    string fieldName = item.Split('.')[item.Split('.').Length - 1];
                    if (fieldName == appObject.IdAttribute)
                    {
                        isExists = true;
                        break;
                    }
                }
            }
            // Check for Lookup field
            if (!isExists)
            {
                List<Guid> appObjectIds = currentRepeatingGroup.RetrieveFields.Select(field => field.AppObject).Distinct().ToList();
                foreach (Guid item in appObjectIds)
                {
                    ApttusObject repeatingGrpAppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(item);
                    if (repeatingGrpAppObject.Fields.Exists(f => f.LookupObject != null))
                    {
                        List<ApttusField> repeatingGroupAppObjFields = repeatingGrpAppObject.Fields.Where(f => f.LookupObject != null).ToList();
                        if (repeatingGroupAppObjFields.Exists(f => f.LookupObject.Id.Equals(appObject.Id) && !f.Id.EndsWith(Constants.APPENDLOOKUPID)))
                        {
                            string fieldId = repeatingGroupAppObjFields.Where(f => f.LookupObject.Id.Equals(appObject.Id) && !f.Id.EndsWith(Constants.APPENDLOOKUPID)).Select(f => f.Id).FirstOrDefault();
                            if (currentRepeatingGroup.RetrieveFields.Exists(f => f.FieldId.EndsWith(fieldId)))
                            {
                                isExists = true;
                                break;
                            }
                        }
                    }
                }
            }

            //ApttusObject parentAppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(currentRepeatitingGroup.AppObject);
            //List<ApttusField> fields = parentAppObject.Fields.Where(fld => currentRepeatitingGroup.RetrieveFields.Any(rfld => rfld.FieldId == fld.Id)).ToList();

            //if (fields.Exists(fld => fld.LookupObject != null &&  fld.LookupObject.IdAttribute == appObject.IdAttribute))
            //    isExists = true;

            return isExists;
        }

        private List<ValidationResult> ValidateSaveMap(SaveMap saveMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            //Validate Base64 Objects
            results = DocumentObjectValidator.GetInstance.Validate(saveMap);

            // #1 Validate ID : Find out the repeating map for the field and check if the ID is missing from Save Other or Retrieve Map
            foreach (SaveGroup saveGroup in saveMap.SaveGroups)
            {
                ValidationResult result = new ValidationResult { Messages = new List<string>(), Success = true };
                List<SaveField> saveGroupSaveFields = saveMap.SaveFields.Where(saveField => saveField.GroupId.Equals(saveGroup.GroupId)).ToList();
                ApttusObject saveObject = ApplicationDefinitionManager.GetInstance.GetAppObject(saveGroup.AppObject);
                string ObjectName = saveObject.Name;

                ////Check for document object
                //results.Add(ValidateDocumentObject(ObjectName, saveMap.Name, saveGroupSaveFields));
                
                // Skip Validation if ID exists in SaveGroup - Save Other scenario
                if (!saveGroupSaveFields.Exists(sf => sf.FieldId.Equals(saveObject.IdAttribute)))
                {
                    RetrieveMap currentRetrieveMap = configManager.GetRetrieveMapbyTargetNamedRange(saveGroup.TargetNamedRange);
                    RepeatingGroup currentRepeatingGroup = configManager.GetRepeatingGroupbyTargetNamedRange(saveGroup.TargetNamedRange);

                    // Skip Validation if ID exists in RepeatingGroup - Save of Display fields scenario
                    if (currentRepeatingGroup != null)
                    {
                        if (!currentRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId.Equals(saveObject.IdAttribute)) && !IsRelationalObjectIdAttributeExist(saveObject, currentRepeatingGroup))
                        {
                            result.EntityName = ObjectName;
                            result.EntityType = EntityType.DisplayMap;
                            result.Messages.Add(String.Format(resourceManager.GetResource("SAVEMAPVALIDATOR_ValidateSaveMapDIS_InfoMsg"),currentRetrieveMap.Name,ObjectName));
                            result.Success = false;

                            results.Add(result);
                        }
                        else
                        {
                            // ID Exists in Repeating Group
                            result.Success = true;
                        }
                    }
                    else
                        result.Success = false;
                }
                else
                {
                    // ID Exists in Save Group
                    result.Success = true;
                }

                if (!result.Success && result.EntityName == null)
                {
                    // Save Other fields
                    result.EntityName = saveMap.Name; ;
                    result.EntityType = EntityType.SaveMap;
                    result.Messages.Add(String.Format(resourceManager.GetResource("SAVEMAPVALIDATOR_ValidateSaveMapSave_InfoMsg"),saveMap.Name,ObjectName));
                    result.Success = false;
                    results.Add(result);
                }

                // #2 Validate Target Column/Row Index : Find out for all the repeating save fields if the target column index is 0 or less
                int TargetIndex = 0;
                bool bIsVerticalLayout = saveGroup.Layout.Equals("Vertical");
                foreach (SaveField field in saveGroupSaveFields)
                {
                    if (bIsVerticalLayout)
                        TargetIndex = ExcelHelper.GetColumnIndex(saveGroup.TargetNamedRange, field.TargetNamedRange);
                    else
                        TargetIndex = ExcelHelper.GetRowIndex(saveGroup.TargetNamedRange, field.TargetNamedRange);
                    if (TargetIndex <= 0)
                    {
                        ApttusField currentField = ApplicationDefinitionManager.GetInstance.GetField(saveObject.UniqueId, field.FieldId);
                        result.EntityName = saveMap.Name;
                        result.EntityType = EntityType.SaveMap;
                        String msg = String.Format(resourceManager.GetResource("SAVEMAPVALID_ValidateSaveMapDelete_ErrMsg"), currentField.Name, ObjectName);
                        if (result.Messages == null)
                            result.Messages = new List<string>();
                        result.Messages.Add(msg);
                        result.Success = false;
                        results.Add(result);
                        break;
                    }
                    field.TargetColumnIndex = TargetIndex;
                    field.DesignerLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));
                }
            }
            return results;
        }


        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            SaveMap saveMap = t as SaveMap;

            ValidationResult result = new ValidationResult();
            result.Success = true;
            result.EntityType = EntityType.SaveMap;
            result.EntityName = saveMap.Name;

            List<Core.Action> allSaveActions = ValidationManager.GetActionsByType(Constants.SAVE_ACTION);
            List<Core.Action> allClearActions = ValidationManager.GetActionsByType(Constants.CLEAR_ACTION);
            if (allSaveActions.Count == 0 && allClearActions.Count == 0)
                return results;

            List<Core.SaveActionModel> saveActions = allSaveActions.OfType<SaveActionModel>().Where(saveAction => saveAction.SaveMapId.Equals(saveMap.Id)).ToList();
            List<Core.ClearActionModel> clearActions = allClearActions.OfType<ClearActionModel>().Where(clearAction => clearAction.SaveMapId.Equals(saveMap.Id)).ToList();
                        
            if (saveActions.Count > 0)
            {
                //There exists a save action which uses this save map.
                //Build Error Message and let the user know which save action(s) are using this SaveMap.

                //Set the success to false
                result.Success = false;

                //Build & Set the Error Message
                StringBuilder errorMsgBuilder = new StringBuilder(saveMap.Name);
                errorMsgBuilder.Append(resourceManager.GetResource("SAVEMAPVALID_ValidateDeleteSave_ErrMsg"));

                bool bAddComma = false;

                foreach (Core.SaveActionModel saveAction in saveActions)
                {
                    if (bAddComma)
                        errorMsgBuilder.Append(",");
                    errorMsgBuilder.Append(saveAction.Name);
                    bAddComma = true;
                }
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_HenceCanNot_Msg"));

                result.Messages = new List<string>();
                result.Messages.Add(errorMsgBuilder.ToString());

                results.Add(result);
            }
            else if (clearActions.Count > 0)
            {
                //Set the success to false
                result.Success = false;

                //Build & Set the Error Message
                StringBuilder errorMsgBuilder = new StringBuilder(saveMap.Name);
                errorMsgBuilder.Append(resourceManager.GetResource("SAVEMAPVALID_ValidateDeleteClear_ErrMsg"));

                bool bAddComma = false;

                foreach (Core.ClearActionModel clearAction in clearActions)
                {
                    if (bAddComma)
                        errorMsgBuilder.Append(",");
                    errorMsgBuilder.Append(clearAction.Name);
                    bAddComma = true;
                }
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_HenceCanNot_Msg"));

                result.Messages = new List<string>();
                result.Messages.Add(errorMsgBuilder.ToString());

                results.Add(result);
            }
            else
            {
                return results;
            }
            return results;
        }
    }
}
