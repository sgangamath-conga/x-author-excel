/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Excel = Microsoft.Office.Interop.Excel;
using VSTO = Microsoft.Office.Tools.Excel;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public static class OfflineHelper
    {
        public static DataManager dataManager = DataManager.GetInstance;
        public static ConfigurationManager configManager = ConfigurationManager.GetInstance;
        public static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// 
        /// </summary>
        public static void SerializeData()
        {
            string objectToSerialize = string.Empty;

            try
            {
                string isAttachmentFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_ATTACHMENTFILE);
                if (!string.IsNullOrEmpty(isAttachmentFile) && isAttachmentFile.Equals("true"))
                    return;

                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                // Runtime flag is empty, it's not runtime file, do not serialize anything
                if (string.IsNullOrEmpty(isRuntimeFile) || configManager.Application == null)
                    return;

                string appId = GetAppId();
                if (string.IsNullOrEmpty(appId))
                    return;

                Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
                //oSheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;

                // Serialize ID data
                //SerializeIdData();

                // Serialize App config
                objectToSerialize = "Apttus_App";
                if (!ExcelHelper.FindShape(oSheet, "Apttus_App"))
                {
                    string appTempFile;
                    if (configManager.Application.EditInExcelAppUniquFileId != null)
                        appTempFile = configManager.Application.EditInExcelAppUniquFileId + ".xml";
                    else
                        appTempFile = Utils.GetApplicationPath(appId) + "\\" + configManager.Application.Definition.Name + "-Runtime.xml";

                    if (!File.Exists(appTempFile) && configManager.Application != null)
                    {
                        string configXml = ApttusXmlSerializerUtil.Serialize(configManager.Application);
                        byte[] configFile = new UTF8Encoding().GetBytes(EncryptionHelper.Encrypt(configXml));
                        File.WriteAllBytes(appTempFile, configFile);
                    }
                    ExcelHelper.AddObjectToSheet(oSheet, appTempFile, "App");
                }
                // Serialize ID datasets
                //if (dataManager.IdData.Count > 0)
                //    SerializeObject(dataManager.IdData, "IdData", appId, oSheet);

                // Serialize ApttusDataSets
                if (dataManager.AppData.Count > 0)
                {
                    objectToSerialize = "AppData";
                    SerializeObject(dataManager.AppData, "AppData", appId, oSheet);
                }
                // Serialize CrossTab datasets
                if (dataManager.CrossTabData.Count > 0)
                {
                    objectToSerialize = "CrossTabData";
                    SerializeObject(dataManager.CrossTabData, "CrossTabData", appId, oSheet);
                }
                // Serialize ApttusDataTracker
                if (dataManager.AppDataTracker.Count > 0)
                {
                    objectToSerialize = "AppDataTracker";
                    SerializeObject(dataManager.AppDataTracker, "AppDataTracker", appId, oSheet);
                }
                // Serialize Data Protection object
                if (dataManager.DataProtection.Count > 0)
                {
                    objectToSerialize = "DataProtection";
                    SerializeObject(dataManager.DataProtection, "DataProtection", appId, oSheet);
                }
                //Marshal.ReleaseComObject(oSheet);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ExceptionLogHelper.DebugLog("Offline: Error while serializing " + objectToSerialize + " Error Message: " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <param name="objectName"></param>
        internal static void SerializeObject(object objectToSerialize, string objectName, string appId, Excel.Worksheet oSheet)
        {
            BinaryFormatter serializationFormatter = new BinaryFormatter();
            MemoryStream buffer = new MemoryStream();

            serializationFormatter.Serialize(buffer, objectToSerialize);
            byte[] serializedData = buffer.ToArray();

            string filePath = Utils.GetTempFileName(appId, objectName);
            Utils.StreamToFile(serializedData, filePath);
            Utils.AddTempFile(filePath);

            ExcelHelper.AddObjectToSheet(oSheet, filePath, objectName);
        }

        /// <summary>
        /// Serialize ID data for each repeating group and take it offline
        /// </summary>
        //internal static void SerializeIdData()
        //{
        //    ApttusDataSet idDataSet = null;
        //    dataManager.IdData.Clear();
        //    foreach (RetrieveMap rMap in configManager.RetrieveMaps)
        //    {
        //        foreach (RepeatingGroup rGroup in rMap.RepeatingGroups)
        //        {
        //            idDataSet = new ApttusDataSet();
        //            idDataSet.DataTable = ExcelHelper.RangeToIdDataTable(ExcelHelper.GetRange(rGroup.TargetNamedRange));
        //            idDataSet.Name = rGroup.TargetNamedRange;

        //            dataManager.IdData.Add(idDataSet);
        //        }
        //    }
        //}

        /// <summary>
        /// Deserialize ID data and put it back to Excel.range ID property
        /// </summary>
        //internal static void DeserializeIdData()
        //{
        //    foreach (ApttusDataSet idData in dataManager.IdData)
        //    {
        //        ExcelHelper.IdDataTableToRange(idData.DataTable);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public static void DeSerializeData()
        {
            string appId = GetAppId();
            if (string.IsNullOrEmpty(appId))
                return;

            // if application is already and user is logged in, do not deserialize use in memory data
            //if (ApplicationManager.GetInstance.Exists(appId, ApplicationMode.Runtime) && ApttusCommandBarManager.g_IsLoggedIn)
            if (Utils.ExistsApplicationInstance(appId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime) && ApttusCommandBarManager.g_IsLoggedIn)
                return;

            Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);

            // Comment this line before commit
            //oSheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;

            List<string> lstSerializedData = ExcelHelper.GetObjectsFromSheet(oSheet);

            if (lstSerializedData.Count == 0)
                return;

            // Deserialize back and verify
            for (int i = 0; i < lstSerializedData.Count; i++)
            {
                //List<ApttusDataSet> IdData = new List<ApttusDataSet>();
                List<ApttusDataSet> AppData = new List<ApttusDataSet>();
                List<ApttusCrossTabDataSet> CrossTabData = new List<ApttusCrossTabDataSet>();
                List<DataProtectionModel> DataProtection = new List<DataProtectionModel>();

                BinaryFormatter deserializationFormatter = new BinaryFormatter();

                MemoryStream msData = ExcelHelper.OleObjectToByte(oSheet, lstSerializedData[i]);
                byte[] tableData = OLEContentHelper.GetOLEContentInBytes(msData);

                MemoryStream debuffer = new MemoryStream(tableData);    // serializedTableData

                if (lstSerializedData[i].Equals("Apttus_AppDataTracker"))
                {
                    List<ApttusDataTracker> appDataTracker = (List<ApttusDataTracker>)deserializationFormatter.Deserialize(debuffer);
                    dataManager.AppDataTracker = appDataTracker;
                }
                else if (lstSerializedData[i].Equals("Apttus_AppData"))
                {
                    AppData = (List<ApttusDataSet>)deserializationFormatter.Deserialize(debuffer);
                    dataManager.AppData = AppData;
                }
                else if (lstSerializedData[i].Equals("Apttus_CrossTabData"))
                {
                    CrossTabData = (List<ApttusCrossTabDataSet>)deserializationFormatter.Deserialize(debuffer);
                    dataManager.CrossTabData = CrossTabData;
                }
                else if (lstSerializedData[i].Equals("Apttus_DataProtection"))
                {
                    DataProtection = (List<DataProtectionModel>)deserializationFormatter.Deserialize(debuffer);
                    dataManager.DataProtection = DataProtection;
                }
                else if (lstSerializedData[i].Equals("Apttus_App"))
                {
                    StreamReader sr = new StreamReader(debuffer);
                    configManager.LoadAppOffline(sr.ReadToEnd());
                }
            }

            // Moved here from Workbookeventmanager -> Application_WorkbookOpen
            Utils.AddApplicationInstance(ApplicationMode.Runtime, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, true);
            // Deserialize IDdata table and back to range
            //DeserializeIdData();
        }

        public static void BuildMenus(Ribbon ribbonxMLUI)
        {

            if (ConfigurationManager.GetInstance.Application != null)
            {
                if (ribbonxMLUI.MenuBuilder == null)
                {
                    ribbonxMLUI.MenuBuilder = new MenuBuilder();
                    ribbonxMLUI.MenuBuilder.Config = ConfigurationManager.GetInstance;
                }
                ribbonxMLUI.MenuBuilder.BuildOfflineMenus(ribbonxMLUI);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static string GetAppId()
        {
            Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
            if (oSheet == null)
                return string.Empty;

            // Get Application Id
            MetadataManager metadataManager = MetadataManager.GetInstance;
            return metadataManager.GetAppUniqueId();
        }

        /// <summary>
        /// Check whether save runtime file by user or not base on app settings by user
        /// </summary>
        /// <returns></returns>
        public static bool IsDisableRuntimeLocalSaveFile()
        {
            string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
            if (!string.IsNullOrEmpty(isRuntimeFile))
            {
                if (configManager.Definition.AppSettings != null)
                {
                    if (!ApttusGlobals.blnExecute && configManager.Definition.AppSettings.DisableLocalSaveFile)
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("OFFLINEHELPER_DisableRunLocal_ErrorMsg"), Constants.RUNTIME_PRODUCT_NAME);
                        ApttusGlobals.blnExecute = false;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether print option by user or not base on app settings by user
        /// </summary>
        /// <returns></returns>
        public static bool IsDisableRuntimePrint()
        {

            string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
            if (!string.IsNullOrEmpty(isRuntimeFile))
            {
                if (configManager.Definition.AppSettings != null)
                {
                    if (configManager.Definition.AppSettings.DisablePrint)
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("OFFLINEHELPER_DisableRuntimePrint_ErrorMsg"), Constants.RUNTIME_PRODUCT_NAME);
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
