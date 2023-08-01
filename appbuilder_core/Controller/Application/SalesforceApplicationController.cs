/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace Apttus.XAuthor.Core
{
    public class SalesforceApplicationController : BaseApplicationController
    {
        public string APP_UNIQUENAMEANDID_CHECK_FIRSTPART {
            get { return "SELECT Id, Name, " + Constants.NAMESPACE_PREFIX + "UniqueId__c FROM " + Constants.NAMESPACE_PREFIX + "Application__c WHERE Name = "; }
        }

        public string APP_UNIQUENAMEANDID_CHECK_SECONDPART {
            get { return " or " + Constants.NAMESPACE_PREFIX + "UniqueId__c = "; }
        }

        public string APP_FETCHAPPBYUNIQUEID {
            get { return "SELECT Id, Name FROM " + Constants.NAMESPACE_PREFIX + "Application__c WHERE " + Constants.NAMESPACE_PREFIX + "UniqueId__c = "; }
        }

        public SalesforceApplicationController()
        {
        }

        public override bool AppNameExists(string AppName)
        {
            // 1. Check if Name and Unique Id exists
            string query = APP_UNIQUENAMEANDID_CHECK_FIRSTPART + Constants.QUOTE + AppName + Constants.QUOTE;
            var App = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });

            return App != null && App.DataTable != null && App.DataTable.Rows.Count > 0 && App.DataTable.Rows[0][Constants.NAME_ATTRIBUTE].ToString().ToUpper() == AppName.ToUpper();
        }

        public override bool CreateApplication(Core.Application NewApp)
        {
            bool result = false;

            // 1. Check if Name and Unique Id exists
            string query = APP_UNIQUENAMEANDID_CHECK_FIRSTPART + Constants.QUOTE + NewApp.Definition.Name + Constants.QUOTE
                + APP_UNIQUENAMEANDID_CHECK_SECONDPART + Constants.QUOTE + NewApp.Definition.UniqueId + Constants.QUOTE;

            var App = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });

            if (App != null && App.DataTable.Rows.Count == 0)
            {
                // 2. Create Application
                ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                {
                    SaveFields = new List<ApttusSaveField> {
                        new ApttusSaveField{
                            FieldId = Constants.NAME_ATTRIBUTE,
                            Value = NewApp.Definition.Name,
                            DataType = Datatype.String
                        },
                        new ApttusSaveField{
                            FieldId = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID,
                            Value = NewApp.Definition.UniqueId,
                            DataType = Datatype.String
                        }
                    },
                    ObjectName = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT,
                    OperationType = QueryTypes.INSERT
                };

                objectManager.Insert(new List<ApttusSaveRecord> { SaveRecord }, false);
                if (SaveRecord.Success)
                {
                    NewApp.Definition.Id = SaveRecord.RecordId;
                    result = true;
                }
                else
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("COREAPPCONTL_AppInsertErr_ErrorMsg"), SaveRecord.ErrorMessage), resourceManager.GetResource("COREAPPCONTL_AppCreatCap_ErrorMsg"));

            }
            else if (App != null && App.DataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in App.DataTable.Rows)
                {
                    if (dr[Constants.NAME_ATTRIBUTE].ToString().ToUpper() == NewApp.Definition.Name.ToUpper())
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("COREAPPCONTL_AppExists_ErrorMsg"), resourceManager.GetResource("COREAPPCONTL_AppCreatCap_ErrorMsg"));
                    else if (dr[Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID].ToString() == NewApp.Definition.UniqueId)
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("COREAPPCONTL_InternalError_ErrorMsg"), resourceManager.GetResource("COREAPPCONTL_AppCreatCap_ErrorMsg"));
                    break;
                }
            }

            return result;
        }

        public override ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            ApplicationObject appObject = null;

            if (string.IsNullOrEmpty(appId))
            {
                appId = GetAppIdByUniqueId(appUniqueId);
            }

            if (!string.IsNullOrEmpty(appId))
            {
                ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();
                appObject = salesForceAdapter.LoadApplication(appId);
                bool IsAppActive = IsAppActivated(appId);
                if (appObject != null)
                {
                    configManager.AssignConfig(appObject.Config);
                    configManager.SetIsAppActive(IsAppActive);
                }
            }
            return appObject;
        }


        public override bool Save(byte[] xlsxTemplate, string TemplateName, byte[] jsonSchema, byte[] googleSheetSchema, string Edition, bool IsMacroExists = false, string CurrentExtension = "")
        {
            bool IsDelete = false;
            bool result = false;
            configManager.Application.Definition.DesignerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();
            configManager.Application.Definition.Schema = jsonSchema;
            configManager.Application.Definition.IsMultiSelectPickListExistsInApp = applicationDefinitionManager.CheckifMultiSelectPickListExistsInApp();
            string xml = ApttusXmlSerializerUtil.Serialize<Application>(configManager.Application);

            UTF8Encoding encoding = new UTF8Encoding();
            string appId = GetAppIdByUniqueId(configManager.Application.Definition.UniqueId);
            // Check If MacroExists and CurrentExtension is XLSX
            if (IsMacroExists && CurrentExtension == Constants.XLSX)
            {
                IsDelete = DeleteTemplate(appId);
            }
            using (AppDetails instance = AppDetails.Instance)
            {
                if (AppDetails.Instance.IsAppDetailsDirty)
                {
                    string ChangedAppName = configManager.Definition.Name;
                    bool IsAppActive = configManager.Definition.IsActive;
                    ApttusSaveField fldName = new ApttusSaveField()
                    {
                        FieldId = "Name",
                        Value = ChangedAppName,
                        DataType = Datatype.String
                    };
                    ApttusSaveField fldIsActive = new ApttusSaveField()
                    {
                        FieldId = "Apttus_XApps__Activated__c",
                        Value = IsAppActive.ToString(),
                        DataType = Datatype.Boolean
                    };
                    ApttusSaveRecord apttusSaveRecord = new ApttusSaveRecord()
                    {
                        ObjectName = "Apttus_XApps__Application__c",
                        RecordId = configManager.Definition.Id,
                        SaveFields = new List<ApttusSaveField>() { fldName, fldIsActive }
                    };

                    salesForceAdapter.Update(new List<ApttusSaveRecord>() { apttusSaveRecord }, false, 1, null);
                    if (!apttusSaveRecord.Success)
                    {
                        ExceptionLogHelper.ErrorLog(apttusSaveRecord.ErrorMessage);
                    }
                }
            }

            result = salesForceAdapter.saveApplication(appId, encoding.GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, jsonSchema, Edition, googleSheetSchema);
            return result;
        }

        public override string GetAppIdByUniqueId(string appUniqueId)
        {
            // Fetch AppId based on Unique Id
            string result = string.Empty;
            string query = APP_FETCHAPPBYUNIQUEID + Constants.QUOTE + appUniqueId + Constants.QUOTE;
            var App = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });

            if (App != null && App.DataTable.Rows.Count == 1)
                result = App.DataTable.Rows[0][Constants.ID_ATTRIBUTE].ToString();
            return result;
        }

        public string GoogleGetAppIdByUniqueId(string appUniqueId)
        {
            return GetAppIdByUniqueId(appUniqueId);
        }

        public override bool DeleteTemplate(string appId)
        {
            //"SELECT Id FROM Attachment WHERE ParentId = 'a1ri0000000ZV6KAAW' and Name like 'Template%'"
            // Fetch AppId based on Unique Id
            bool result = false;
            string RecordId = string.Empty;
            try
            {
                string query = "SELECT Id FROM Attachment WHERE ParentId =" + Constants.QUOTE + appId + Constants.QUOTE + "and Name = 'Template.xlsx'";
                var App = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });
                if (App != null && App.DataTable.Rows.Count == 1)
                    RecordId = App.DataTable.Rows[0][Constants.ID_ATTRIBUTE].ToString();
                if (!string.IsNullOrEmpty(RecordId))
                {
                    ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                    {
                        OperationType = QueryTypes.DELETE,
                        ObjectName = Constants.ATTACHMENTOBJECT,
                        RecordId = RecordId
                    };

                    objectManager.Delete(new List<ApttusSaveRecord> { SaveRecord }, false);
                    if (SaveRecord.Success == true)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return result;
        }


        public override bool CloneApp(Application application, string CloneWorkbookName)
        {
            try
            {
                bool bResult = false;
                string appCloneName = application.Definition.Name;
                string appCloneUniqueId = application.Definition.UniqueId;
                //set the edition .
                if (string.IsNullOrEmpty(application.Definition.EditionType))
                {
                    application.Definition.EditionType = LicenseActivationManager.GetInstance.SetEditionForApps(null);
                }


                // 1. Check if Name and Unique Id exists
                string query = APP_UNIQUENAMEANDID_CHECK_FIRSTPART + Constants.QUOTE + appCloneName + Constants.QUOTE
                + APP_UNIQUENAMEANDID_CHECK_SECONDPART + Constants.QUOTE + appCloneUniqueId + Constants.QUOTE;
                var App = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });

                if (App != null && App.DataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in App.DataTable.Rows)
                    {
                        string OverwriteMessage = String.Format(resourceManager.GetResource("APPCTL_CloneApp_InfoMsg"), appCloneName);

                        // Scenario 1: Name matches and Unique Id matches
                        if (dr[Constants.NAME_ATTRIBUTE].ToString() == appCloneName & dr[Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID].ToString() == appCloneUniqueId)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, resourceManager.GetResource("APPCTL_CloneAppOverwrite_InfoMsg"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = OverrideApplication(application, CloneWorkbookName, appCloneName, dr[Constants.ID_ATTRIBUTE].ToString(), dr[Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID].ToString());
                            }
                        }
                        // Scenario 2: Name matches and but Unique Id does not match
                        else if (dr[Constants.NAME_ATTRIBUTE].ToString() == appCloneName)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, resourceManager.GetResource("APPCTL_CloneAppOverwrite_InfoMsg"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = OverrideApplication(application, CloneWorkbookName, appCloneName, dr[Constants.ID_ATTRIBUTE].ToString(), appCloneUniqueId);
                            }
                        }
                        // Scenario 3: Name does not match but Unique Id matches
                        else if (dr[Constants.NAME_ATTRIBUTE].ToString() != appCloneName & dr[Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID].ToString() == application.Definition.UniqueId)
                        {
                            bResult = CreateAppAndSaveAttachments(application, CloneWorkbookName, appCloneName);
                        }
                        break;
                    }
                }
                // Scenario 4: Name and Unique Id do not match
                else if (App != null && App.DataTable.Rows.Count == 0)
                {
                    bResult = CreateAppAndSaveAttachments(application, CloneWorkbookName, appCloneName);
                }

                if (bResult)
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("COREAPPCONTL_CloneApp_InfoMsg"), resourceManager.GetResource("COREAPPCONTL_CloneAppCAP_InfoMsg"));
                return bResult;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, resourceManager.GetResource("COREAPPCONTL_CloneCaption_ErrorMsg"));
                return false;
            }
        }

        public override bool OverrideApplication(Application application, string templateNameWithPath, string appName, string AppId, string uniqueId, Byte[] schema = null)
        {
            //Over-ride the existing AppName and Id with the newly Specified Application Name
            application.Definition.Id = AppId;
            application.Definition.Name = appName;
            application.Definition.UniqueId = uniqueId;

            //Update the Existing AppName with the Newly Specified AppName
            if (!string.IsNullOrEmpty(AppId))
            {
                ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                {
                    OperationType = QueryTypes.UPDATE,
                    ObjectName = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT,
                    RecordId = AppId
                };
                SaveRecord.SaveFields = new List<ApttusSaveField>();

                ApttusSaveField saveFieldName = new ApttusSaveField()
                {
                    DataType = Datatype.String,
                    FieldId = Constants.NAME_ATTRIBUTE,
                    Value = appName
                };
                SaveRecord.SaveFields.Add(saveFieldName);

                ApttusSaveField saveFieldUniqueID = new ApttusSaveField()
                {
                    DataType = Datatype.String,
                    FieldId = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID,
                    Value = application.Definition.UniqueId
                };
                SaveRecord.SaveFields.Add(saveFieldUniqueID);

                objectManager.Update(new List<ApttusSaveRecord> { SaveRecord }, false);
            }

            //Upload or Save the user specified template and xml config.
            return SaveAttachments(application, templateNameWithPath, schema);
        }

        public override bool CreateAppAndSaveAttachments(Application application, string templateNameWithPath, string appName, byte[] schema = null)
        {
            //Create an Application Instance
            bool result = false;
            ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField> {
                        new ApttusSaveField{
                            FieldId = Constants.NAME_ATTRIBUTE,
                            Value = appName,
                            DataType = Datatype.String
                        },
                        new ApttusSaveField{
                            FieldId = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID,
                            Value = application.Definition.UniqueId,
                            DataType = Datatype.String
                        }
                    },
                ObjectName = Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT,
                OperationType = QueryTypes.INSERT
            };

            objectManager.Insert(new List<ApttusSaveRecord> { SaveRecord }, false);

            if (SaveRecord.Success)
            {
                application.Definition.Id = SaveRecord.RecordId;
                result = SaveAttachments(application, templateNameWithPath, schema);
            }
            return result;
        }

        public override bool SaveAttachments(Application application, string templateNameWithPath, byte[] schema = null)
        {
            bool result = false;
            using (FileStream fs = new FileStream(templateNameWithPath, FileMode.Open, FileAccess.Read))
            {
                byte[] xlsxTemplate = new byte[fs.Length];
                fs.Read(xlsxTemplate, 0, System.Convert.ToInt32(fs.Length));

                String TemplateName = Constants.TEMPLATENAME + Path.GetExtension(templateNameWithPath);

                ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();
                string xml = ApttusXmlSerializerUtil.Serialize<Application>(application);

                result = salesForceAdapter.saveApplication(application.Definition.Id, new UTF8Encoding().GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, schema, application.Definition.EditionType);


                xml = null; //Important
                xlsxTemplate = null; //Important
            }
            return result;
        }

        public override ApttusDataSet GetAppbyNameOrUniqueId(string appImportName, string uniqueId)
        {
            string queryString = APP_UNIQUENAMEANDID_CHECK_FIRSTPART + Constants.QUOTE + appImportName + Constants.QUOTE
                + APP_UNIQUENAMEANDID_CHECK_SECONDPART + Constants.QUOTE + uniqueId + Constants.QUOTE;

            return objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
        }

        public override string GetApplicationObjectUniqueIdAttribute()
        {
            return Constants.NAMESPACE_PREFIX + Constants.APPLICATIONOBJECT_UNIQUEID;
        }

        public override string GetApplicationObjectIdAttribute()
        {
            return Constants.ID_ATTRIBUTE;
        }

        public override string GetApplicationObjectNameAttribute()
        {
            return Constants.NAME_ATTRIBUTE;
        }

        public bool IsCurrentUserHasAccessToApp(string AppUniqueId)
        {
            /*
            Select Apttus_XApps__ApplicationId__c 
            From Apttus_XApps__UserXAuthorApp__c 
            Where (Apttus_XApps__ProfileId__c = '00ei00000017DmHAAU' OR Apttus_XApps__UserId__c = '005i0000001g04IAAQ')
	        And (Apttus_XApps__ApplicationId__c = 'a2ui00000008yzUAAQ')*/


            if (string.IsNullOrEmpty(AppUniqueId)) return false;
            ApttusUserInfo userInfo = objectManager.getUserInfo();
            string AppId = GetAppIdByUniqueId(AppUniqueId);

            string queryStr = "Select Apttus_XApps__ApplicationId__c"
                             + " From Apttus_XApps__UserXAuthorApp__c  "
                             + " WHERE ("
                            + Constants.NAMESPACE_PREFIX + "ProfileId__c = '" + userInfo.ProfileId
                            + Constants.NAMESPACE_PREFIX + "UserId__c = '" + userInfo.UserId + "')"
                            + "AND (" + Constants.NAMESPACE_PREFIX + "ApplicationId__c = '" + AppId + "')";
            // if the current user has access to the app 1 record will be return;
            return objectManager.Query(new SalesforceQuery { SOQL = queryStr }).Count > 0 ? true : false;
        }
        public override bool IsAppActivated(string AppId)
        {
            /*
            Select Apttus_XApps__ApplicationId__c 
            From Apttus_XApps__UserXAuthorApp__c 
            Where (Apttus_XApps__ProfileId__c = '00ei00000017DmHAAU' OR Apttus_XApps__UserId__c = '005i0000001g04IAAQ')
	        And (Apttus_XApps__ApplicationId__c = 'a2ui00000008yzUAAQ')*/


            if (string.IsNullOrEmpty(AppId)) return false;

            string queryStr = "Select Apttus_XApps__Activated__c"
                             + " From Apttus_XApps__Application__c  "
                             + " WHERE ("
                             + "id = '" + AppId + "')";

            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryStr });
            string IsActivated = DS.DataTable.Rows[0]["Apttus_XApps__Activated__c"] as string;
            try
            {
                return Convert.ToBoolean(IsActivated);
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool DeactiVateApp(string AppUniqueId, bool Activate)
        {

            if (string.IsNullOrEmpty(AppUniqueId)) return false;

            string AppId = GetAppIdByUniqueId(AppUniqueId);
            ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();

            try
            {
                return salesForceAdapter.deactivateApplication(AppId);
            }

            catch (Exception ex)
            {
                return false;
            }

            return false;


        }
    }
}