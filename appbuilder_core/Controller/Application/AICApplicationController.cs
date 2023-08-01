using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using Apttus.DataAccess.Common.Model;
using System.Data;
using System.Reflection;
using System.IO;

namespace Apttus.XAuthor.Core
{
    public class AICApplicationController : BaseApplicationController
    {
        private ABAICAAdapter aicAdapter = new ABAICAAdapter();
        public static string APP_OBJECT = "xae_App";
        public static string APP_OBJECT_ID_ATTRIBUTE = Constants.ID_ATTRIBUTE;
        public static string APP_OBJECT_NAME_ATTRIBUTE = Constants.NAME_ATTRIBUTE;
        public static string APP_OBJECT_UNIQUEID_ATTRIBUTE = "UniqueId";

        public AICApplicationController()
        {
            if (CRMContextManager.Instance.ActiveCRM.Equals(CRM.AICV2))
                aicAdapter = new ABAICV2Adapter();
        }
        public override bool CreateApplication(Core.Application NewApp)
        {
            bool result = false;

            Query query = GetAppNameExistsQueryExpression(NewApp.Definition.Name, NewApp.Definition.UniqueId);

            var App = objectManager.QueryDataSet(new AICQuery { Query = query });

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

        public Query GetAppNameExistsQueryExpression(string appName, string appUniqueId)
        {
            Query getAppQuery = new Query(APP_OBJECT, false);
            List<string> selectCols = new List<string> { APP_OBJECT_ID_ATTRIBUTE, APP_OBJECT_NAME_ATTRIBUTE, APP_OBJECT_UNIQUEID_ATTRIBUTE, "Edition", "Activated" };
            getAppQuery.Columns = selectCols;

            Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.OR);

            filter.AddCondition(APP_OBJECT_NAME_ATTRIBUTE, DataAccess.Common.Enums.FilterOperator.Equal, appName);
            filter.AddCondition(APP_OBJECT_UNIQUEID_ATTRIBUTE, DataAccess.Common.Enums.FilterOperator.Equal, appUniqueId);

            getAppQuery.Criteria = filter;
            //getAppQuery.SetCriteria(filter);

            return getAppQuery;
        }

        public override bool Save(byte[] xlsxTemplate, string TemplateName, byte[] jsonSchema, byte[] googleSheetSchema, string Edition, bool IsMacroExists = false, string CurrentExtension = "")
        {
            bool result = false;

            configManager.Application.Definition.DesignerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            configManager.Application.Definition.Schema = jsonSchema;
            string xml = ApttusXmlSerializerUtil.Serialize<Application>(configManager.Application);

            UTF8Encoding encoding = new UTF8Encoding();
            string appId = GetAppIdByUniqueId(configManager.Application.Definition.UniqueId);

            DeleteTemplate(appId);

            result = aicAdapter.saveApplication(appId, encoding.GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, jsonSchema);

            return result;
        }

        public override ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            ApplicationObject appObject = null;

            if (string.IsNullOrEmpty(appId))
            {
                appId = GetAppIdByUniqueId(appUniqueId);
            }
            // TODO:: remove this once Generic API attachment support is there
            if (string.IsNullOrEmpty(appUniqueId))
            {
                appUniqueId = GetAppUniqueIdByAppId(appId);
            }
            
            if (!string.IsNullOrEmpty(appId))
            {
                //appObject = aicAdapter.LoadApplication(appId);
                // TODO:: remove this once Generic API attachment support is there, and uncomment above line
                Guid appUniqueGuid;
                Guid.TryParse(appUniqueId, out appUniqueGuid);
                appObject = aicAdapter.LoadApplication(appId, appUniqueGuid);
                if (appObject != null)
                {
                    configManager.AssignConfig(appObject.Config);
                }
                else // AB-2636 : MSD: While trying to open a particular app getting the error of 'Index was out of range' in the logs. 
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_SwitchApp_ErrorMsg"), resourceManager.GetResource("RIBBON_TemplateMissing_ErrorMsg"));
            }
            return appObject;

        }

        // TODO:: remove this once Generic API attachment support is there, and uncomment above line
        private string GetAppUniqueIdByAppId(string appId)
        {
            // Fetch AppUniqueId based on App Id
            string result = string.Empty;
            Query query = new Query(APP_OBJECT, false);
            query.Columns = new List<string> { APP_OBJECT_ID_ATTRIBUTE, APP_OBJECT_NAME_ATTRIBUTE, APP_OBJECT_UNIQUEID_ATTRIBUTE };

            Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.AND);
            filter.AddCondition(APP_OBJECT_ID_ATTRIBUTE, DataAccess.Common.Enums.FilterOperator.Equal, appId);

            query.Criteria = filter;

            var App = objectManager.QueryDataSet(new AICQuery { Query = query });

            if (App != null && App.DataTable.Rows.Count == 1)
                result = App.DataTable.Rows[0][APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString();
            return result;
        }

        public override string GetAppIdByUniqueId(string appUniqueId)
        {
            // Fetch AppId based on Unique Id
            string result = string.Empty;
            Query query = new Query(APP_OBJECT, false);
            query.Columns = new List<string> { APP_OBJECT_ID_ATTRIBUTE, APP_OBJECT_NAME_ATTRIBUTE };

            Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.AND);
            filter.AddCondition(APP_OBJECT_UNIQUEID_ATTRIBUTE, DataAccess.Common.Enums.FilterOperator.Equal, appUniqueId);

            query.Criteria = filter;

            var App = objectManager.QueryDataSet(new AICQuery { Query = query });

            if (App != null && App.DataTable.Rows.Count == 1)
                result = App.DataTable.Rows[0][APP_OBJECT_ID_ATTRIBUTE].ToString();
            return result;
        }

        public override string GetApplicationObjectIdAttribute()
        {
            return APP_OBJECT_ID_ATTRIBUTE;
        }

        public override string GetApplicationObjectNameAttribute()
        {
            return APP_OBJECT_NAME_ATTRIBUTE;
        }

        public override string GetApplicationObjectUniqueIdAttribute()
        {
            return APP_OBJECT_UNIQUEID_ATTRIBUTE;
        }

        public override bool AppNameExists(string AppName)
        {
            var App = GetAppbyNameOrUniqueId(AppName, string.Empty);
            return App != null && App.DataTable != null && App.DataTable.Rows.Count > 0 && App.DataTable.Rows[0][Constants.NAME_ATTRIBUTE].ToString().ToUpper() == AppName.ToUpper();
        }

        public override ApttusDataSet GetAppbyNameOrUniqueId(string appImportName, string uniqueId)
        {
            Query queryExpr = GetAppNameExistsQueryExpression(appImportName, uniqueId);
            return objectManager.QueryDataSet(new AICQuery { Query = queryExpr });
        }

        public override bool DeleteTemplate(string appId)
        {
            aicAdapter.DeleteAppAttachment(appId);
            return true;
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
                ApttusDataSet App = GetAppbyNameOrUniqueId(appCloneName, appCloneUniqueId);

                if (App != null && App.DataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in App.DataTable.Rows)
                    {
                        string OverwriteMessage = String.Format(resourceManager.GetResource("APPCTL_CloneApp_InfoMsg"), appCloneName);

                        // Scenario 1: Name matches and Unique Id matches
                        if (dr[Constants.NAME_ATTRIBUTE].ToString() == appCloneName & dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString() == appCloneUniqueId)
                        {
                            if (ApttusMessageUtil.ShowWarning(OverwriteMessage, resourceManager.GetResource("APPCTL_CloneAppOverwrite_InfoMsg"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
                            {
                                // Overwrite existing app
                                bResult = OverrideApplication(application, CloneWorkbookName, appCloneName, dr[Constants.ID_ATTRIBUTE].ToString(), dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString());
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
                        else if (dr[Constants.NAME_ATTRIBUTE].ToString() != appCloneName & dr[APP_OBJECT_UNIQUEID_ATTRIBUTE].ToString() == application.Definition.UniqueId)
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
                    ObjectName = APP_OBJECT,
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
                    FieldId = APP_OBJECT_UNIQUEID_ATTRIBUTE,
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

                string xml = ApttusXmlSerializerUtil.Serialize<Application>(application);

                result = aicAdapter.saveApplication(application.Definition.Id, new UTF8Encoding().GetBytes(EncryptionHelper.Encrypt(xml)), xlsxTemplate, TemplateName, schema);

                xml = null; //Important
                xlsxTemplate = null; //Important
            }
            return result;
        }
    }
}
