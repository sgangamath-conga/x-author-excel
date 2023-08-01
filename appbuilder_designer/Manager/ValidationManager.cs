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
using Apttus.XAuthor.AppDesigner.Modules;
using System.IO;
using System.Windows.Forms;


namespace Apttus.XAuthor.AppDesigner
{
    public class ValidationManager
    {
        static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ValidateApp()
        {
            List<ValidationResult> results;

            // Validate RetrieveMaps
            results = RetrieveMapValidator.GetInstance.Validate<RetrieveMap>(null);

            // Validate Matrix
            results.AddRange(MatrixMapValidator.GetInstance.Validate<MatrixMap>(null));

            // Validate SaveMaps
            results.AddRange(SaveMapValidator.GetInstance.Validate<SaveMap>(null));

            //Validate SaveAttachment
            results.AddRange(ActionsValidator.GetInstance.Validate<Core.Action>(null));

            results.AddRange(new DataSourceValidator().Validate<DataTransferMapping>(null));

            bool bSuccess = VerifyResults(results);

            if (!bSuccess)
            {
                ErrorMessageViewer viewer = new ErrorMessageViewer();
                viewer.ErrorMessageHeader = resourceManager.GetResource("VALIDMAN_ValidateApp_ErrMsg");
                viewer.AddResults(results);
                viewer.ShowDialog();
                viewer = null;
            }

            return bSuccess;
        }

        /// <summary>
        /// Macro Exists or not
        /// </summary>
        /// <returns></returns>
        public static bool IsMacroExists(out string currentExtension)
        {
            bool IsMacroExists = false;
            ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
            string activeWbPath = commandBar.GetActiveWorkbookName();
            // 1. Get Extension of file            
            currentExtension = Path.GetExtension(activeWbPath);

            List<string> lstMacro = new List<string>();
            // 2. Check CurrentExtension and XLSX , if yes then it will get macro
            if (currentExtension == Constants.XLSX)
            {
                // Pass Optional Parameter for handle Exception
                lstMacro = ExcelHelper.GetMacroList(IsVBACodeAllowed: true);
            }
            // 3. If Macro exists and currentextension is XLSX 
            if (lstMacro.Count > 0 && currentExtension == Constants.XLSX)
            {
                IsMacroExists = true;
            }
            return IsMacroExists;
        }


        /// <summary>
        /// If sheet is removed by user and entries is not updated in config file when save application
        /// </summary>
        /// <returns></returns>
        public static bool SyncSheetProtectSettingsData(out List<string> resultSheet)
        {
            bool blnProtectSheetValidation = false;
            AppSettings ProtectSettingModel;
            ProtectSettingModel = configurationManager.Definition.AppSettings;
            List<string> lstConfigData = new List<string>();
            List<string> lstSheet = new List<string>();
            resultSheet = new List<string>();

            if (ProtectSettingModel != null)
            {

                blnProtectSheetValidation = ExcelHelper.ValidateSheetForPasswordMismatch(out resultSheet, out lstSheet);
            
                if (ProtectSettingModel.SheetProtectSettings.Count != lstSheet.Count)
                {
                    configurationManager.Definition.AppSettings.SheetProtectSettings.ForEach((item) => { lstConfigData.Add(item.SheetName); });

                    // Validation SheetProtection
                    resultSheet = lstSheet.Except(lstConfigData).ToList();

                    if (resultSheet.Count > 0)
                        blnProtectSheetValidation = true;
                }
                List<string> resultConfig = lstConfigData.Except(lstSheet).ToList();

                foreach (string strName in resultConfig)
                {
                    SheetProtect sheetProtect = configurationManager.Application.Definition.AppSettings.SheetProtectSettings.Find(s => s.SheetName.Equals(strName));
                    if (sheetProtect != null)
                        configurationManager.Application.Definition.AppSettings.SheetProtectSettings.Remove(sheetProtect);
                }
            }
            return blnProtectSheetValidation;
        }

        public static List<ValidationResult> ValidateDelete(RetrieveMap rMap)
        {
            return RetrieveMapValidator.GetInstance.ValidateDelete<RetrieveMap>(rMap);
        }

        public static List<ValidationResult> ValidateDelete(MatrixMap mMap)
        {
            return MatrixMapValidator.GetInstance.ValidateDelete<MatrixMap>(mMap);
        }

        public static List<ValidationResult> ValidateDelete(SaveMap saveMap)
        {
            return SaveMapValidator.GetInstance.ValidateDelete<SaveMap>(saveMap);
        }

        public static List<ValidationResult> ValidateDelete(Workflow workflow)
        {
            return WorkflowValidator.GetInstance.ValidateDelete<Workflow>(workflow);
        }

        public static List<ValidationResult> ValidateDelete(ActionProperties action)
        {
            return ActionsValidator.GetInstance.ValidateDelete<ActionProperties>(action);
        }

        internal static List<Core.Action> GetActionsByType(string actionType)
        {
            return ConfigurationManager.GetInstance.Actions.Where(action => action.Type.Equals(actionType)).ToList();
        }


        internal static List<Core.RetrieveActionModel> GetRetrieveActionsForRetrieveMap(RetrieveMap retrieveMap)
        {
            //As of now there is no restriction that a given retrieve map cannot be used by multiple retrieve actions, hence returning a list.
            //Going forward if we provide the restriction, which we should, we can return a RetrieveAction referring to this retrieveMap if it exists.

            return GetActionsByType(Constants.RETRIEVE_ACTION).OfType<RetrieveActionModel>().Where(retAction => retAction.RetrieveMapId.Equals(retrieveMap.Id)).ToList();
        }

        internal static List<Core.RetrieveActionModel> GetMatrixActionsForMatrixMap(MatrixMap matrixMap)
        {
            return GetActionsByType(Constants.RETRIEVE_ACTION).OfType<RetrieveActionModel>().Where(retAction => retAction.RetrieveMapId.Equals(matrixMap.Id)).ToList();
        }

        internal static bool VerifyResults(List<ValidationResult> results)
        {
            return results.Where(res => res.Success == false).Count() == 0;
        }

        /// <summary>
        /// Validation Lookup
        /// </summary>
        /// <param name="RepeatingFields"></param>
        public static bool ValidationLookup(List<SaveField> RepeatingFields, List<SaveField> ExistingFields, ref List<SaveField> lookUpFieldToRemove)
        {
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
            bool invalid = false;
            List<SaveField> cloneRepeatingFields = new List<SaveField>();
            RepeatingFields.ForEach((item) => { cloneRepeatingFields.Add(item.Clone()); });

            // In Edit Mode of existing Save Map, get already added fields.
            if (ExistingFields != null && ExistingFields.Count > 0)
            {
                // Add Unique fields to clonedrepeating fields.
                foreach (SaveField sf in ExistingFields)
                {
                    if (!cloneRepeatingFields.Exists(f => f.TargetNamedRange.Equals(sf.TargetNamedRange)))
                        cloneRepeatingFields.Add(sf);
                }
            }

            List<Guid> lstAppObjects = cloneRepeatingFields.GroupBy(sf => sf.AppObject).Select(sf => sf.Key).Distinct().ToList();
            Dictionary<SaveField, string> lstSaveFieldLookupObject = new Dictionary<SaveField, string>();

            foreach (Guid appObjectGuid in lstAppObjects)
            {
                lstSaveFieldLookupObject.Clear();
                List<SaveField> singleObjectRepeatingFields = cloneRepeatingFields.Where(rf => rf.AppObject.Equals(appObjectGuid)).ToList();
                ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(appObjectGuid);

                foreach (SaveField field in singleObjectRepeatingFields)
                {
                    string fieldId = string.Empty;
                    if (string.IsNullOrEmpty(field.MultiLevelFieldId))
                        fieldId = field.FieldId;
                    else
                    {
                        string[] fieldIds = field.MultiLevelFieldId.Split(new char[] { '-' });
                        if (fieldIds != null && fieldIds.Count() > 0)
                            fieldId = fieldIds[fieldIds.Length - 1];
                        else
                            fieldId = field.MultiLevelFieldId;
                    }
                    ApttusField filterField = appObject.GetField(fieldId);
                    //if ((!field.FieldId.Equals(Constants.ID_ATTRIBUTE)) && // Exclude Id field as its not a lookup
                    //         (field.FieldId.EndsWith(Constants.ID_ATTRIBUTE) || // Include ending with "Id"
                    //         field.FieldId.EndsWith(Constants.APPENDLOOKUPID) || // Include ending with ".Name"
                    //         (field.FieldId.Contains(Constants.CustomObjectReference__c) && filterField.Datatype == Datatype.Lookup) // Include ending with "__c" and is Lookup datatype
                    //         ))
                    //Discussed with Anil - for Matrix field this condition is not needed. 
                    if (field.SaveFieldType != SaveType.MatrixField && appDefManager.IsLookupField(filterField))
                    {
                        var existingField = lstSaveFieldLookupObject
                            .Where(x => x.Key.TargetNamedRange == field.TargetNamedRange)
                            .Select(e => (KeyValuePair<SaveField, string>?)e)
                            .FirstOrDefault();

                            if (existingField == null && filterField != null && filterField.LookupObject != null)
                                lstSaveFieldLookupObject.Add(field, filterField.LookupObject.Id);
                    }
                }

                List<SaveField> keysToDelete = new List<SaveField>();
                foreach (var item in lstSaveFieldLookupObject)
                {
                    // If field is Id field
                    ApttusField filterField = appObject.GetField(item.Key.FieldId);
                    ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(item.Key.AppObject);
                    //if (item.Key.FieldId.EndsWith(obj.IdAttribute) || item.Key.FieldId.EndsWith(Constants.CustomObjectReference__c))
                    if (appDefManager.IsLookupIdField(obj, filterField))
                    {
                        // Find if the name exists for the same lookup object
                        string lookupNameFieldId = appDefManager.GetLookupNameFromLookupId(item.Key.FieldId);
                        var existingField = lstSaveFieldLookupObject
                            .Where(x => x.Key.FieldId == lookupNameFieldId && x.Value == item.Value)
                            .Select(e => (KeyValuePair<SaveField, string>?)e)
                            .FirstOrDefault();

                        // If Name is not found add to collection to remove
                        if (existingField == null)
                            keysToDelete.Add(item.Key);
                    }
                    // Else if field is Lookup Name field
                    else if (item.Key.FieldId.EndsWith(Constants.APPENDLOOKUPID))
                    {
                        // Find if the Id exists for the same lookup object
                        string lookupIdFieldId = appDefManager.GetLookupIdFromLookupName(item.Key.FieldId);
                        var existingField = lstSaveFieldLookupObject
                            .Where(x => x.Key.FieldId == lookupIdFieldId && x.Value == item.Value)
                            .Select(e => (KeyValuePair<SaveField, string>?)e)
                            .FirstOrDefault();

                        // If Id is not found add to collection to remove
                        if (existingField == null)
                            keysToDelete.Add(item.Key);
                    }
                }

                // Remove keys for valid lookups
                foreach (var key in keysToDelete)
                    lstSaveFieldLookupObject.Remove(key);

                int currentObjectCounter = 0;
                foreach (var invalidLookupIdAndName in lstSaveFieldLookupObject)
                {
                    invalid = true;
                    if (RepeatingFields.Exists(sf => sf.AppObject.Equals(invalidLookupIdAndName.Key.AppObject) && sf.FieldId.Equals(invalidLookupIdAndName.Key.FieldId)))
                    {
                        lookUpFieldToRemove.Add(invalidLookupIdAndName.Key);
                        currentObjectCounter++;
                    }
                }

                if (lookUpFieldToRemove != null && lookUpFieldToRemove.Count > 0 && currentObjectCounter > 0)
                {
                    ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("VALIDMANG_ValidationLookup_WarnMsg"), currentObjectCounter.ToString(), appObject.Name), Constants.RUNTIME_PRODUCT_NAME, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Ok);
                    invalid = true;
                }
            }
            return invalid;
        }

    }
}
