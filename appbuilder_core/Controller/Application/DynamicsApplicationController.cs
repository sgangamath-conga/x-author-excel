using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Xrm.Sdk.Query;

namespace Apttus.XAuthor.Core
{
    public class DynamicsApplicationController : BaseApplicationController
    {
        private ABDynamicsCRMAdapter dynamicsAdapter = new ABDynamicsCRMAdapter();

        public static string APP_OBJECT = "apttus_xapps_application";
        public static string APP_OBJECT_ID_ATTRIBUTE = "apttus_xapps_applicationid";
        public static string APP_OBJECT_NAME_ATTRIBUTE = "apttus_name";
        public static string APP_OBJECT_UNIQUEID_ATTRIBUTE = "apttus_xapps_uniqueid";

        public override bool CreateApplication(Core.Application NewApp)
        {
            bool result = false;

            QueryExpression query = GetAppNameExistsQueryExpression(NewApp.Definition.Name, NewApp.Definition.UniqueId);
            //QueryExpression query = new QueryExpression()
            //{
            //    Distinct = false,
            //    EntityName = "apttus_xapps_application",
            //    ColumnSet = new ColumnSet("apttus_name", "apttus_xapps_uniqueid", "apttus_xapps_edition", "apttus_xapps_activated"),

            //    Criteria =
            //    {
            //        Filters = 
            //        {
            //            new FilterExpression
            //            {
            //                FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.Or,
            //                Conditions = 
            //                {
            //                    new ConditionExpression("apttus_name", ConditionOperator.Equal, NewApp.Definition.Name),
            //                    new ConditionExpression("apttus_xapps_uniqueid", ConditionOperator.Equal, NewApp.Definition.UniqueId)
            //                }
            //            }
            //        }
            //    }
            //};

            var App = objectManager.QueryDataSet(new DynamicsQuery { Query = query });

            if (App != null && App.DataTable.Rows.Count == 0)
            {
                // 2. Create Application
                ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
                {
                    SaveFields = new List<ApttusSaveField> { 
                        new ApttusSaveField{
                            FieldId = APP_OBJECT_NAME_ATTRIBUTE,
                            Value = NewApp.Definition.Name,
                            DataType = Datatype.String
                        },
                        new ApttusSaveField{
                            FieldId = APP_OBJECT_UNIQUEID_ATTRIBUTE,
                            Value = NewApp.Definition.UniqueId,
                            DataType = Datatype.String
                        }
                    },
                    ObjectName = APP_OBJECT,
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
                    if (dr[APP_OBJECT_NAME_ATTRIBUTE].ToString().ToUpper() == NewApp.Definition.Name.ToUpper())
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("COREAPPCONTL_AppExists_ErrorMsg"), resourceManager.GetResource("COREAPPCONTL_AppCreatCap_ErrorMsg"));
                    else if (dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString() == NewApp.Definition.UniqueId)
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
                appObject = dynamicsAdapter.LoadApplication(appId);
                if (appObject != null)
                {
                    configManager.AssignConfig(appObject.Config);
                }
                else // AB-2636 : MSD: While trying to open a particular app getting the error of 'Index was out of range' in the logs. 
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_SwitchApp_ErrorMsg"), resourceManager.GetResource("RIBBON_TemplateMissing_ErrorMsg"));
            }
            return appObject;

        }

        public override string GetAppIdByUniqueId(string appUniqueId)
        {
            // Fetch AppId based on Unique Id
            string result = string.Empty;
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = APP_OBJECT,
                ColumnSet = new ColumnSet(APP_OBJECT_NAME_ATTRIBUTE),

                Criteria =
                {
                    Filters = 
                    {
                        new FilterExpression
                        {
                            FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                            Conditions = 
                            {
                                new ConditionExpression(APP_OBJECT_UNIQUEID_ATTRIBUTE, ConditionOperator.Equal, appUniqueId),
                            }
                        }
                    }
                }
            };

            var App = objectManager.QueryDataSet(new DynamicsQuery { Query = query });

            if (App != null && App.DataTable.Rows.Count == 1)
                result = App.DataTable.Rows[0][APP_OBJECT_ID_ATTRIBUTE].ToString();
            return result;
        }

        public override bool Save(byte[] xlsxTemplate, string TemplateName, byte[] jsonSchema, byte[] googleSheetSchema, string Edition, bool IsMacroExists = false, string CurrentExtension = "")
        {
            bool IsDelete = false;
            bool result = false;

            configManager.Application.Definition.DesignerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            configManager.Application.Definition.Schema = jsonSchema;
            string xml = ApttusXmlSerializerUtil.Serialize<Application>(configManager.Application);

            UTF8Encoding encoding = new UTF8Encoding();
            string appId = GetAppIdByUniqueId(configManager.Application.Definition.UniqueId);
            //TODO:: Check If MacroExists and CurrentExtension is XLSX
            //if (IsMacroExists && CurrentExtension == Constants.XLSX)
            //{
            IsDelete = DeleteTemplate(appId);
            //}

            result = dynamicsAdapter.saveApplication(appId, encoding.GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, jsonSchema);

            return result;
        }

        public override bool DeleteTemplate(string appId)
        {
            bool result = false;
            string templateRecordId = string.Empty;
            string appDefRecordId = string.Empty;
            try
            {

                QueryExpression query = new QueryExpression()
                {
                    Distinct = false,
                    EntityName = Constants.ATTACHMENTOBJECT_DYNAMICS,

                    Criteria =
                    {
                        Filters = 
                        {
                            new FilterExpression
                            {
                                FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                                Conditions = 
                                {
                                    new ConditionExpression("objectid", ConditionOperator.Equal, new Guid(appId)),
                                }
                            }
                        }
                    }
                };

                var App = objectManager.QueryDataSet(new DynamicsQuery { Query = query });

                if (App != null && App.DataTable.Rows.Count > 1)
                {
                    templateRecordId = App.DataTable.Rows[0][0].ToString();
                    appDefRecordId = App.DataTable.Rows[1][0].ToString();
                }

                if (!string.IsNullOrEmpty(templateRecordId) && !string.IsNullOrEmpty(appDefRecordId))
                {
                    ApttusSaveRecord templateRecord = new ApttusSaveRecord()
                    {
                        OperationType = QueryTypes.DELETE,
                        ObjectName = "annotation",
                        RecordId = templateRecordId
                    };

                    ApttusSaveRecord appDefRecord = new ApttusSaveRecord()
                    {
                        OperationType = QueryTypes.DELETE,
                        ObjectName = "annotation",
                        RecordId = appDefRecordId
                    };

                    List<ApttusSaveRecord> annotations = new List<ApttusSaveRecord>() { templateRecord, appDefRecord };
                    objectManager.Delete(annotations, false);
                    if (annotations[0].Success == true && annotations[1].Success == true)
                        result = true;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return result;
        }

        public QueryExpression GetAppNameExistsQueryExpression(string appName, string appUniqueId)
        {
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = APP_OBJECT,
                ColumnSet = new ColumnSet(APP_OBJECT_NAME_ATTRIBUTE, APP_OBJECT_UNIQUEID_ATTRIBUTE, "apttus_xapps_edition", "apttus_xapps_activated"),

                Criteria =
                {
                    Filters = 
                    {
                        new FilterExpression
                        {
                            FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.Or,
                            Conditions = 
                            {
                                new ConditionExpression(APP_OBJECT_NAME_ATTRIBUTE, ConditionOperator.Equal, appName),
                                new ConditionExpression(APP_OBJECT_UNIQUEID_ATTRIBUTE, ConditionOperator.Equal, appUniqueId)
                            }
                        }
                    }
                }
            };

            return query;
        }

        public override bool OverrideApplication(Application application, string templateNameWithPath, string appName, string AppId, string uniqueId, Byte []schema = null)
        {
            bool retVal = false;

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
                    ObjectName = APP_OBJECT,
                    RecordId = AppId
                };
                SaveRecord.SaveFields = new List<ApttusSaveField>();

                ApttusSaveField saveFieldName = new ApttusSaveField()
                {
                    DataType = Datatype.String,
                    FieldId = APP_OBJECT_NAME_ATTRIBUTE,
                    Value = appName
                };
                SaveRecord.SaveFields.Add(saveFieldName);

                ApttusSaveField saveFieldUniqueID = new ApttusSaveField()
                {
                    DataType = Datatype.String,
                    FieldId = APP_OBJECT_UNIQUEID_ATTRIBUTE,
                    Value = application.Definition.UniqueId
                };
                SaveRecord.SaveFields.Add(saveFieldUniqueID);

                objectManager.Update(new List<ApttusSaveRecord> { SaveRecord }, false);
            }

            //Upload or Save the user specified template and xml config.
            if (DeleteTemplate(AppId))
                retVal = SaveAttachments(application, templateNameWithPath);

            return retVal;
        }

        public override bool CreateAppAndSaveAttachments(Application application, string templateNameWithPath, string appName, byte[] schema = null)
        {
            //Create an Application Instance
            bool result = false;
            ApttusSaveRecord SaveRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField> {
                        new ApttusSaveField{
                            FieldId = APP_OBJECT_NAME_ATTRIBUTE,
                            Value = appName,
                            DataType = Datatype.String
                        },
                        new ApttusSaveField{
                            FieldId = APP_OBJECT_UNIQUEID_ATTRIBUTE,
                            Value = application.Definition.UniqueId,
                            DataType = Datatype.String
                        }
                    },
                ObjectName = APP_OBJECT,
                OperationType = QueryTypes.INSERT
            };

            objectManager.Insert(new List<ApttusSaveRecord> { SaveRecord }, false);

            if (SaveRecord.Success)
            {
                application.Definition.Id = SaveRecord.RecordId;
                result = SaveAttachments(application, templateNameWithPath, null);
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

                string xml = ApttusXmlSerializerUtil.Serialize<Application>(application);

                result = dynamicsAdapter.saveApplication(application.Definition.Id, new UTF8Encoding().GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, application.Definition.Schema);

                xml = null; //Important
                xlsxTemplate = null; //Important
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

                QueryExpression query = GetAppNameExistsQueryExpression(appCloneName, appCloneUniqueId);
                var App = objectManager.QueryDataSet(new DynamicsQuery { Query = query });

                if (App != null && App.DataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in App.DataTable.Rows)
                    {
                        string OverwriteMessage = appCloneName + " already exists. Do you wish to overwrite the existing app with the cloned App?";

                        // Scenario 1: Name matches and Unique Id matches
                        if (dr[APP_OBJECT_NAME_ATTRIBUTE].ToString() == appCloneName & dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString() == appCloneUniqueId)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, "App Clone : Overwrite App", ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = OverrideApplication(application, CloneWorkbookName, appCloneName, dr[APP_OBJECT_ID_ATTRIBUTE].ToString(), dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString());
                            }
                        }
                        // Scenario 2: Name matches and but Unique Id does not match
                        else if (dr[APP_OBJECT_NAME_ATTRIBUTE].ToString() == appCloneName)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, "App Clone : Overwrite App", ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = OverrideApplication(application, CloneWorkbookName, appCloneName, dr[APP_OBJECT_ID_ATTRIBUTE].ToString(), appCloneUniqueId);
                            }
                        }
                        // Scenario 3: Name does not match but Unique Id matches
                        else if (dr[APP_OBJECT_NAME_ATTRIBUTE].ToString() != appCloneName & dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString() == application.Definition.UniqueId)
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
        public override bool AppNameExists(string AppName)
        {
            var App = GetAppbyNameOrUniqueId(AppName, string.Empty);
            return App != null && App.DataTable != null && App.DataTable.Rows.Count > 0 && App.DataTable.Rows[0][Constants.NAME_ATTRIBUTE].ToString().ToUpper() == AppName.ToUpper();
        }
        public override ApttusDataSet GetAppbyNameOrUniqueId(string appImportName, string uniqueId)
        {
            QueryExpression queryExpr = GetAppNameExistsQueryExpression(appImportName, uniqueId);
            return objectManager.QueryDataSet(new DynamicsQuery { Query = queryExpr });
        }

        public override string GetApplicationObjectUniqueIdAttribute()
        {
            return APP_OBJECT_UNIQUEID_ATTRIBUTE;
        }

        public override string GetApplicationObjectIdAttribute()
        {
            return APP_OBJECT_ID_ATTRIBUTE;
        }

        public override string GetApplicationObjectNameAttribute()
        {
            return APP_OBJECT_NAME_ATTRIBUTE;
        }
    }
}
