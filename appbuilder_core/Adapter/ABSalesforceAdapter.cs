/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.SalesforceAdapter;
using Apttus.XAuthor.SalesforceAdapter.AppBuilderWS;
using Apttus.XAuthor.SalesforceAdapter.SForce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace Apttus.XAuthor.Core
{
    public class ABSalesforceAdapter : IAdapter
    {
        public const string NAMESPACE_PREFIX = "Apttus_XApps";
        public const string NAMESPACE_PREFIX_DESIGNER = "Apttus_XAppsDS";
        private ILookupIdAndLookupNameProvider LookupIdAndLookupNameProvider;
        // This variables use for internal cache purpose for attachments records
        private static string ParentId = string.Empty;
        private static string Id = string.Empty;
        private static string Name = string.Empty;
        private static string ParentName = string.Empty;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        // Intiliaze Salesforce Connector class
        SForcePartnerServiceController salesForceConnector = SForcePartnerServiceController.GetInstance;
        AppBuilderWsController appBuilderWsController = AppBuilderWsController.GetInstance;
        private ApttusUserInfo apttusUserInfo;
        private bool _IsSandBox;
        private bool _IsDesigner = false;
        // Connect with Salesforce
        public bool Connect(string OAuthToken, string InstanceURL, System.Net.IWebProxy proxyObject)
        {
            if (OAuthToken != null)
            {
                salesForceConnector.ConnectWithSalesoforce(OAuthToken, InstanceURL, proxyObject);

                // Once Logged in get Namespace prefix
                Constants.NAMESPACE_PREFIX = getNamespacePrefix();
                appBuilderWsController.ConnectWithSalesforce(OAuthToken, InstanceURL, Constants.NAMESPACE_PREFIX, proxyObject);

                if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
                    IsAdminPackageEditionUser();

                apttusUserInfo = getUserInfo();
                _IsSandBox = appBuilderWsController.IsSandBox();
                _IsDesigner = appBuilderWsController.IsDesigner();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Return Partner API version from Salesforce Adapter project
        public string GetPartnerAPIVersion()
        {
            if (salesForceConnector != null)
                return salesForceConnector.PARTNER_API_VERSION;

            return string.Empty;
        }

        // Connect with Salesforce
        public bool Connect(string username, string password, string token, SalesforceEndpoint endpoint)
        {
            string endpointURL = string.Empty;
            switch (endpoint)
            {
                case SalesforceEndpoint.Login:
                    endpointURL = "https://login.salesforce.com/services/Soap/u/27.0";
                    break;
                case SalesforceEndpoint.Test:
                    endpointURL = "https://test.salesforce.com/services/Soap/u/27.0";
                    break;
            }

            salesForceConnector.ConnectWithSalesoforce(username, password, token, endpointURL);
            Constants.NAMESPACE_PREFIX = getNamespacePrefix();

            string topLevelUrl = new Uri(salesForceConnector.EndPointURL).GetLeftPart(UriPartial.Authority);
            appBuilderWsController.ConnectWithSalesforce(SForcePartnerServiceController.sessionHeader.sessionId, topLevelUrl, Constants.NAMESPACE_PREFIX, null);

            return true;
        }

        public ApttusUserInfo UserInfo { get { return apttusUserInfo; } }

        // Get User details
        private ApttusUserInfo getUserInfo()
        {
            apttusUserInfo = new ApttusUserInfo();
            GetUserInfoResult result = salesForceConnector.getUserInfo();
            apttusUserInfo.UserId = result.userId;
            apttusUserInfo.UserName = result.userName;
            apttusUserInfo.UserEmail = result.userEmail;
            apttusUserInfo.UserFullName = result.userFullName;
            apttusUserInfo.ProfileId = result.profileId;
            apttusUserInfo.Locale = result.userLocale;
            apttusUserInfo.Language = result.userLanguage;
            apttusUserInfo.OrganizationId = result.organizationId;
            apttusUserInfo.OrganizationName = result.organizationName;
            return apttusUserInfo;
        }

        private List<ApttusObject> Query<T>(string soql)
        {
            List<ApttusObject> lstApttusObjects = new List<ApttusObject>();
            try
            {
                List<sObject> lstsObjects = salesForceConnector.Query<sObject>(soql);
                foreach (sObject apttusObjects in lstsObjects)
                {
                    ApttusObject obj = new ApttusObject();

                    obj.Id = apttusObjects.Id;

                    if (apttusObjects.Any.Length > 1)
                        obj.Name = apttusObjects.Any[1].LocalName.Equals("Name") ? apttusObjects.Any[1].InnerText : string.Empty;

                    lstApttusObjects.Add(obj);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return lstApttusObjects;
        }

        private ApttusDataSet QueryDataSet(string soql, ApttusObject appObject, DataTable dataTable, ApttusUserInfo userInfo)
        {
            ApttusDataSet dataSet = new ApttusDataSet();
            List<sObject> lstsObjects = salesForceConnector.Query<sObject>(soql);

            dataSet.DataTable = new DataTable();
            dataSet.DataTable.MinimumCapacity = lstsObjects.Count;
            dataSet.SOQL = soql;

            if (lstsObjects.Count == 0)
            {
                if (dataTable != null)
                    dataSet.DataTable = dataTable;
                return dataSet;
            }

            if (dataTable == null)
            {
                // 1. Add Datatable Columns based on 1st record
                foreach (XmlElement field in lstsObjects[0].Any)
                {
                    // HasChildNodes is true for fields which have non-null value.
                    if (field.HasChildNodes)
                    {
                        // SF field for the object
                        if (field.ChildNodes[0].NodeType == XmlNodeType.Text)
                        {
                            dataSet.DataTable.Columns.Add(field.LocalName);
                        }
                        // SF field for the lookup object
                        else
                        {
                            for (int i = 2; i < field.ChildNodes.Count; i++)
                            {
                                if (field.ChildNodes[i].NodeType == XmlNodeType.Element)
                                {
                                    string fieldName = field.LocalName + Constants.DOT + field.ChildNodes[i].LocalName;
                                    dataSet.DataTable.Columns.Add(fieldName);
                                }
                            }
                        }
                    }
                    // HasChildNodes is false for fields which have null value.
                    else
                    {
                        dataSet.DataTable.Columns.Add(field.LocalName);
                    }
                }

                // 2. Set DataColumn Datatypes
                if (appObject != null)
                {
                    foreach (DataColumn dc in dataSet.DataTable.Columns)
                    {
                        ApttusField apttusField = appObject.Fields.Where(s => s.Id == dc.ColumnName).FirstOrDefault();
                        if (apttusField == null)
                            continue;

                        if (apttusField.Datatype == Datatype.Decimal || apttusField.Datatype == Datatype.Double)
                            dataSet.DataTable.Columns[dc.ColumnName].DataType = typeof(double);
                    }
                }
            }
            else
                dataSet.DataTable = dataTable;

            // 3. Add data records to Datatable
            foreach (Apttus.XAuthor.SalesforceAdapter.SForce.sObject sObject in lstsObjects)
            {
                DataRow row = dataSet.DataTable.NewRow();

                foreach (XmlElement field in sObject.Any)
                {
                    if (field.HasChildNodes)
                    {
                        // SF field for the object
                        if (field.ChildNodes[0].NodeType == XmlNodeType.Text)
                        {
                            SetSFFieldValue(dataSet.DataTable.Columns[field.LocalName], row, field.LocalName, field.InnerText, userInfo);
                        }
                        // SF field for the lookup object
                        else
                        {
                            for (int i = 2; i < field.ChildNodes.Count; i++)
                            {
                                if (field.ChildNodes[i].NodeType == XmlNodeType.Element)
                                {
                                    string columnName = field.LocalName + Constants.DOT + field.ChildNodes[i].LocalName;

                                    PopulateChildObjectData(field.ChildNodes[i], row, dataSet, columnName, userInfo);
                                    // Display Map enhancement
                                    //SetSFFieldValue(dataSet.DataTable.Columns[columnName], row, columnName, field.ChildNodes[i].InnerText);
                                }
                            }
                        }
                    }
                }

                dataSet.DataTable.Rows.Add(row);
            }

            //Free the Memory
            for (int i = 0; i < lstsObjects.Count; ++i)
            {
                sObject sObj = lstsObjects.ElementAt(i);
                sObj = null;
            }
            lstsObjects.Clear();
            lstsObjects.Capacity = 0;
            lstsObjects = null;
            return dataSet;
        }

        internal void PopulateChildObjectData(XmlNode field, DataRow row, ApttusDataSet dataSet, string columnName, ApttusUserInfo userInfo)
        {
            string colName = string.Empty;
            // SF field for the object
            if ((!(field as XmlElement).HasAttributes) || (field.HasChildNodes && field.ChildNodes.Count == 1))
            {
                colName = columnName;
                SetSFFieldValue(dataSet.DataTable.Columns[colName], row, colName, field.InnerText, userInfo);
            }
            else if (field.HasChildNodes && field.ChildNodes.Count > 1)
            {
                for (int i = 2; i < field.ChildNodes.Count; i++)
                {
                    colName = columnName + Constants.DOT + field.ChildNodes[i].LocalName;
                    PopulateChildObjectData(field.ChildNodes[i], row, dataSet, colName, userInfo);
                }
            }
        }

        public void Insert(List<ApttusSaveRecord> InsertObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (InsertObjects != null && InsertObjects.Count > 0)
                {
                    List<sObject> SFObjects = new List<sObject>();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (ApttusSaveRecord InsertObject in InsertObjects)
                    {
                        if (InsertObject.HasAttachment)
                            SFObjects.AddRange(GetAttachments(InsertObject));
                        else
                        {
                            sObject SFObject = new sObject()
                            {
                                type = InsertObject.ObjectName,
                                Any = GetXMLElement(InsertObject.SaveFields, currentCultureInfo, enUSCulture)
                            };
                            SFObjects.Add(SFObject);
                        }
                    }

                    List<SaveResult> allResults = ChunkAndExecute<SaveResult>(SFObjects, true, string.Empty, waitWindow, QueryTypes.INSERT, 0, SFObjects.Count, enablePartialSave, BatchSize);
                    ProcessInsertResults(allResults, InsertObjects);

                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void Update(List<ApttusSaveRecord> UpdateObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (UpdateObjects != null && UpdateObjects.Count > 0)
                {
                    List<sObject> SFObjects = new List<sObject>();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (ApttusSaveRecord UpdateObject in UpdateObjects.Where(s => s.Ignore == false))
                    {
                        if (UpdateObject.HasAttachment)
                            SFObjects.AddRange(GetAttachments(UpdateObject));
                        else
                        {
                            sObject SFObject = new sObject()
                            {
                                Id = UpdateObject.RecordId,
                                type = UpdateObject.ObjectName,
                                Any = GetXMLElement(UpdateObject.SaveFields, currentCultureInfo, enUSCulture)
                            };

                            // Add Fields to Null for Update
                            string[] FieldsToNull = GetNullFields(UpdateObject.SaveFields);
                            if (FieldsToNull.Length > 0)
                                SFObject.fieldsToNull = FieldsToNull;

                            SFObjects.Add(SFObject);
                        }
                    }

                    List<SaveResult> allResults = ChunkAndExecute<SaveResult>(SFObjects, false, string.Empty, waitWindow, QueryTypes.UPDATE, 0, SFObjects.Count, enablePartialSave, BatchSize);
                    ProcessUpdateResults(allResults, UpdateObjects);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void Upsert(List<ApttusSaveRecord> UpsertObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (UpsertObjects != null && UpsertObjects.Count > 0)
                {
                    // Upsert requires single object type to be passed.
                    string ExternalId = string.Empty;
                    List<string> UniqueUpsertObjects = UpsertObjects.Select(s => s.ObjectName).Distinct().ToList();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (string UniqueUpsertObject in UniqueUpsertObjects)
                    {
                        // Find ExternalId column
                        ApttusSaveRecord FirstUniqueObjectSaveRecord = UpsertObjects.Where(s => s.ObjectName == UniqueUpsertObject).Where(s => s.Ignore == false).FirstOrDefault();
                        ExternalId = FirstUniqueObjectSaveRecord.SaveFields.FirstOrDefault(sf => sf.ExternalId).FieldId;

                        List<sObject> SFObjects = new List<sObject>();
                        foreach (ApttusSaveRecord UpsertObject in UpsertObjects.Where(s => s.ObjectName == UniqueUpsertObject).Where(s => s.Ignore == false))
                        {
                            if (UpsertObject.HasAttachment)
                                SFObjects.AddRange(GetAttachments(UpsertObject));
                            else
                            {
                                sObject SFObject = new sObject()
                                {
                                    Id = UpsertObject.RecordId,
                                    type = UpsertObject.ObjectName,
                                    Any = GetXMLElement(UpsertObject.SaveFields, currentCultureInfo, enUSCulture)
                                };

                                // Add Fields to Null for Upsert
                                string[] FieldsToNull = GetNullFields(UpsertObject.SaveFields);
                                if (FieldsToNull.Length > 0)
                                    SFObject.fieldsToNull = FieldsToNull;

                                SFObjects.Add(SFObject);
                            }
                        }

                        List<UpsertResult> allResults = ChunkAndExecute<UpsertResult>(SFObjects, false, ExternalId, waitWindow, QueryTypes.UPSERT, 0, SFObjects.Count, enablePartialSave, BatchSize);
                        ProcessUpsertResults(allResults, UpsertObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public ApttusDataSet Search(string soql, ApttusUserInfo userInfo)
        {
            ApttusDataSet dataSet = new ApttusDataSet();
            dataSet.DataTable = new DataTable();

            SearchResult searchRes = salesForceConnector.Search(soql);

            if (searchRes.searchRecords == null)
                return dataSet;

            // Create column in data table
            foreach (XmlElement field in searchRes.searchRecords[0].record.Any)
            {
                if (field.HasChildNodes)
                {
                    if (field.ChildNodes[0].NodeType == XmlNodeType.Text)
                        dataSet.DataTable.Columns.Add(field.LocalName);
                    else
                    {
                        for (int i = 1; i < field.ChildNodes.Count; i++)
                        {
                            if (field.ChildNodes[i].NodeType == XmlNodeType.Element &&
                                 (field.ChildNodes[i].LocalName.Equals(Constants.ID_ATTRIBUTE) || field.ChildNodes[i].LocalName.Equals(Constants.NAME_ATTRIBUTE)))
                            {
                                if (!dataSet.DataTable.Columns.Contains(field.LocalName + "." + field.ChildNodes[i].LocalName))
                                    dataSet.DataTable.Columns.Add(field.LocalName + "." + field.ChildNodes[i].LocalName);
                            }
                        }
                    }
                }
            }

            // Add data to table
            foreach (SearchRecord record in searchRes.searchRecords)
            {
                DataRow row = dataSet.DataTable.NewRow();

                foreach (XmlElement field in record.record.Any)
                {
                    if (field.HasChildNodes)
                    {
                        // SF field for the object
                        if (field.ChildNodes[0].NodeType == XmlNodeType.Text)
                            SetSFFieldValue(dataSet.DataTable.Columns[field.LocalName], row, field.LocalName, field.InnerText, userInfo);
                        // SF field for the lookup object
                        else
                        {
                            for (int i = 1; i < field.ChildNodes.Count; i++)
                            {
                                if (field.ChildNodes[i].NodeType == XmlNodeType.Element &&
                                    (field.ChildNodes[i].LocalName.Equals(Constants.ID_ATTRIBUTE) || field.ChildNodes[i].LocalName.Equals(Constants.NAME_ATTRIBUTE)))
                                    SetSFFieldValue(dataSet.DataTable.Columns[field.LocalName + "." + field.ChildNodes[i].LocalName], row, field.LocalName + "." + field.ChildNodes[i].LocalName, field.ChildNodes[i].InnerText, userInfo);
                            }
                        }
                    }
                }

                dataSet.DataTable.Rows.Add(row);
            }

            return dataSet;
        }

        public void Delete(List<ApttusSaveRecord> DeleteObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (DeleteObjects != null && DeleteObjects.Count > 0)
                {
                    List<sObject> SFObjects = new List<sObject>();
                    foreach (ApttusSaveRecord DeleteObject in DeleteObjects)
                    {
                        sObject SFObject = new sObject()
                        {
                            Id = DeleteObject.RecordId
                        };
                        SFObjects.Add(SFObject);
                    }

                    List<DeleteResult> deleteResults = ChunkAndExecute<DeleteResult>(SFObjects, false, string.Empty, waitWindow, QueryTypes.DELETE, 0, SFObjects.Count, enablePartialSave, BatchSize);
                    ProcessDeleteResults(deleteResults, DeleteObjects);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private List<T> ChunkAndExecute<T>(List<sObject> sObjects, bool Insert, string ExternalId, WaitWindowView waitWindow, QueryTypes queryType, int current, int total, bool enablePartialSave, int BatchSize)
        {
            List<T> chunkResult = new List<T>();
            List<T> allResults = new List<T>();
            int MaxTransactionRecords = BatchSize;
            int MaxTransactionUniqueObjects = 10;

            try
            {
                if (sObjects.Count <= MaxTransactionRecords && sObjects.Select(s => s.type).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    SetWaitMessage(waitWindow, queryType, current + sObjects.Count, total);
                    allResults = ExecuteChunk<T>(sObjects, Insert, ExternalId, enablePartialSave);
                }
                else if (sObjects.Count > MaxTransactionRecords && sObjects.Select(s => s.type).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    List<sObject> MaxRecordsChunk = sObjects.GetRange(0, MaxTransactionRecords);

                    // Part 1 - Chunk of Max Records
                    SetWaitMessage(waitWindow, queryType, current + MaxRecordsChunk.Count, total);
                    chunkResult = ExecuteChunk<T>(MaxRecordsChunk, Insert, ExternalId, enablePartialSave);
                    allResults.AddRange(chunkResult);

                    // Part 2 - Chunk of Remaining Records - Recursion
                    List<sObject> RemainingRecords = sObjects.GetRange(MaxTransactionRecords, sObjects.Count - MaxTransactionRecords);
                    chunkResult = ChunkAndExecute<T>(RemainingRecords, Insert, ExternalId, waitWindow, queryType, current + MaxRecordsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);
                }
                else if (sObjects.Select(s => s.type).Distinct().Count() > MaxTransactionUniqueObjects)
                {
                    List<string> AllUniqueObjects = (from s in sObjects select s.type).Distinct().ToList();
                    List<string> MaxUniqueObjects = new List<string>();
                    for (int i = 0; i < MaxTransactionUniqueObjects; i++)
                        MaxUniqueObjects.Add(AllUniqueObjects[i]);

                    // Part 1 - Chunk of Max Unique Objects - Recursion
                    List<sObject> MaxUniqueObjectsChunk = sObjects.Where(s => MaxUniqueObjects.Contains(s.type)).ToList();
                    chunkResult = ChunkAndExecute<T>(MaxUniqueObjectsChunk, Insert, ExternalId, waitWindow, queryType, current + MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);

                    // Part 2 - Chunk of Remaining Unique Objects - Recursion
                    chunkResult = ChunkAndExecute<T>(sObjects.Where(s => !MaxUniqueObjects.Contains(s.type)).ToList(), Insert, ExternalId, waitWindow, queryType, current + +MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return allResults;
        }

        private List<T> ExecuteChunk<T>(List<sObject> Chunk, bool Insert, string ExternalId, bool enablePartialSave)
        {
            List<T> chunkResult = new List<T>();
            try
            {
                switch (typeof(T).Name)
                {
                    case "SaveResult":
                        SaveResult[] saveResult;
                        if (Insert)
                            saveResult = salesForceConnector.Insert(Chunk.ToArray(), enablePartialSave);
                        else
                            saveResult = salesForceConnector.Update(Chunk.ToArray(), enablePartialSave);
                        chunkResult = saveResult.Cast<T>().ToList();
                        break;
                    case "UpsertResult":
                        UpsertResult[] upsertResult = salesForceConnector.Upsert(Chunk.ToArray(), ExternalId, enablePartialSave);
                        chunkResult = upsertResult.Cast<T>().ToList();
                        break;
                    case "DeleteResult":
                        DeleteResult[] deleteResult = salesForceConnector.Delete(Chunk.Select(c => c.Id).ToArray(), enablePartialSave);
                        chunkResult = deleteResult.Cast<T>().ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return chunkResult;
        }

        private void ProcessInsertResults(List<SaveResult> allResults, List<ApttusSaveRecord> SaveRecords)
        {
            try
            {
                int cnt = 0;
                foreach (var result in allResults)
                {
                    ApttusSaveRecord SaveRecord = SaveRecords[cnt++];
                    SaveRecord.Success = result.success;
                    if (result.success)
                    {
                        SaveRecord.RecordId = result.id;
                    }
                    else
                    {
                        StringBuilder errorMessage = new StringBuilder();
                        foreach (var err in result.errors)
                        {
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsCode_Msg") + " " + err.statusCode.ToString());
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsMsg_Msg") + " " + err.message);
                            if (err.fields != null)
                                errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsField_Msg") + string.Join(",", err.fields));
                        }
                        SaveRecord.ErrorMessage = errorMessage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void ProcessUpdateResults(List<SaveResult> allResults, List<ApttusSaveRecord> SaveRecords)
        {
            try
            {
                int cnt = 0;
                foreach (var result in allResults)
                {
                    ApttusSaveRecord SaveRecord = SaveRecords[cnt++];
                    SaveRecord.Success = result.success;
                    if (!result.success)
                    {
                        StringBuilder errorMessage = new StringBuilder();
                        foreach (var err in result.errors)
                        {
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsCode_Msg") + " " + err.statusCode.ToString());
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsMsg_Msg") + " " + err.message);
                            if (err.fields != null)
                                errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsField_Msg") + string.Join(",", err.fields));
                        }
                        SaveRecord.ErrorMessage = errorMessage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void ProcessUpsertResults(List<UpsertResult> allResults, List<ApttusSaveRecord> SaveRecords)
        {
            try
            {
                int cnt = 0;
                foreach (var result in allResults)
                {
                    ApttusSaveRecord SaveRecord = SaveRecords[cnt++];
                    SaveRecord.Success = result.success;

                    if (result.success)
                    {
                        SaveRecord.RecordId = result.id;

                        // If record was not created but an existing record was found then
                        // change the operation type to Upsert.
                        if (!result.created)
                            SaveRecord.OperationType = QueryTypes.UPSERT;
                    }
                    else
                    {
                        StringBuilder errorMessage = new StringBuilder();
                        foreach (var err in result.errors)
                        {
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsCode_Msg") + " " + err.statusCode.ToString());
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsMsg_Msg") + " " + err.message);
                            if (err.fields != null)
                                errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsField_Msg") + string.Join(",", err.fields));
                        }
                        SaveRecord.ErrorMessage = errorMessage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void ProcessDeleteResults(List<DeleteResult> deleteResults, List<ApttusSaveRecord> SaveRecords)
        {
            try
            {
                int cnt = 0;
                foreach (var result in deleteResults)
                {
                    ApttusSaveRecord SaveRecord = SaveRecords[cnt++];
                    SaveRecord.Success = result.success;
                    if (!result.success)
                    {
                        StringBuilder errorMessage = new StringBuilder();
                        foreach (var err in result.errors)
                        {
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsCode_Msg") + " " + err.statusCode.ToString());
                            errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsMsg_Msg") + " " + err.message);
                            if (err.fields != null)
                                errorMessage.AppendLine(resourceManager.GetResource("COMMON_ProcessUpdateResultsField_Msg") + string.Join(",", err.fields));
                        }
                        SaveRecord.ErrorMessage = errorMessage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private XmlElement[] GetXMLElement(List<ApttusSaveField> saveFields, CultureInfo currentCultureInfo, CultureInfo targetCultureInfo)
        {
            XmlDocument xmlDocument = new XmlDocument();
            List<XmlElement> xmlElements = new List<XmlElement>();

            foreach (ApttusSaveField saveField in saveFields)
            {
                XmlElement ValidatedXmlElement = ValidateXmlElement(xmlDocument, saveField, currentCultureInfo, targetCultureInfo);
                if (!string.IsNullOrEmpty(ValidatedXmlElement.InnerText))
                    xmlElements.Add(ValidatedXmlElement);
            }

            return xmlElements.ToArray();
        }

        private XmlElement ValidateXmlElement(XmlDocument xmlDocument, ApttusSaveField saveField, CultureInfo currentCultureInfo, CultureInfo targetCultureInfo)
        {
            XmlElement xmlElement = xmlDocument.CreateElement(saveField.FieldId);

            if (!string.IsNullOrEmpty(saveField.Value))
                switch (saveField.DataType)
                {
                    case Datatype.Decimal:
                    case Datatype.Double:
                        double value;
                        // if number seperator and group seperate are switched from DOT to COMMA
                        if (currentCultureInfo.Name == "en-US" && currentCultureInfo.NumberFormat.NumberDecimalSeparator.Equals(Constants.COMMA))
                            saveField.Value = saveField.Value.Replace(Constants.COMMA, Constants.DOT);

                        if (double.TryParse(saveField.Value, System.Globalization.NumberStyles.Float, currentCultureInfo, out value))
                            xmlElement.InnerText = value.ToString(targetCultureInfo);
                        else
                            xmlElement.InnerText = saveField.Value;
                        break;
                    default:
                        xmlElement.InnerText = saveField.Value;
                        break;
                }
            else
            {
                switch (saveField.DataType)
                {
                    case Datatype.Decimal:
                    case Datatype.Double:
                    case Datatype.Date:
                    case Datatype.DateTime:
                        xmlElement.IsEmpty = true;

                        XmlAttribute xmlNullAttribute = xmlDocument.CreateAttribute("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance");
                        xmlNullAttribute.Value = "true";
                        xmlElement.SetAttributeNode(xmlNullAttribute);
                        break;
                    default:
                        xmlElement.InnerText = string.Empty;
                        break;
                }
            }

            return xmlElement;
        }

        private string[] GetNullFields(List<ApttusSaveField> saveFields)
        {
            List<string> NullFields = new List<string>();
            foreach (ApttusSaveField saveField in saveFields)
                if (string.IsNullOrEmpty(saveField.Value))
                    NullFields.Add(saveField.FieldId);
            return NullFields.ToArray();
        }

        private List<sObject> GetAttachments(ApttusSaveRecord SaveRecord)
        {
            List<sObject> Attachments = new List<sObject>();
            foreach (var SaveAttachment in SaveRecord.Attachments)
            {
                sObject Attachment = new sObject()
                {
                    type = Constants.ATTACHMENTOBJECT,
                    Any = GetAttachmentXMLElement(SaveAttachment)
                };
                Attachments.Add(Attachment);
            }
            return Attachments;
        }

        private XmlElement[] GetAttachmentXMLElement(ApttusSaveAttachment SaveAttachment)
        {
            XmlDocument xmlDocument = new XmlDocument();
            List<XmlElement> xmlElements = new List<XmlElement>();

            //Name
            XmlElement xmlNameElement = xmlDocument.CreateElement(Constants.NAME_ATTRIBUTE);
            xmlNameElement.InnerText = SaveAttachment.AttachmentName;
            xmlElements.Add(xmlNameElement);

            // Body
            XmlElement xmlBodyElement = xmlDocument.CreateElement(Constants.ATTACHMENTOBJECT_BODY);
            xmlBodyElement.InnerText = SaveAttachment.Base64EncodedString;
            xmlElements.Add(xmlBodyElement);

            // Parent Id
            XmlElement xmlParentIdElement = xmlDocument.CreateElement(Constants.ATTACHMENTOBJECT_PARENTID);
            xmlParentIdElement.InnerText = SaveAttachment.ParentId;
            xmlElements.Add(xmlParentIdElement);

            // Content Type
            string contentType = GetContentType(SaveAttachment.AttachmentName);
            if (!string.IsNullOrEmpty(contentType))
            {
                XmlElement xmlContentTypeElement = xmlDocument.CreateElement("ContentType");
                xmlContentTypeElement.InnerText = contentType;
                xmlElements.Add(xmlContentTypeElement);
            }

            return xmlElements.ToArray();
        }

        private string GetContentType(string attachmentName)
        {
            return System.Web.MimeMapping.GetMimeMapping(attachmentName);
        }

        private static void SetSFFieldValue(DataColumn dataColumn, DataRow row, string fieldName, string value, ApttusUserInfo userInfo)
        {
            // For Attachments
            if (fieldName.Contains(Constants.ATTACHMENT) && dataColumn == null)
            {
                if (fieldName.Contains("Attachments.records.ParentId"))
                    ParentId = value;
                else if (fieldName.Contains("Attachments.records.Id"))
                    Id = value;
                else if (fieldName.Contains("Attachments.records.Name"))
                    Name = value;
                else if (fieldName.Contains("Attachments.records.Parent.Name"))
                {
                    ParentName = value;
                    if (!string.IsNullOrEmpty(ParentId) || !string.IsNullOrEmpty(Id) || !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(ParentName))
                    {
                        // Replace Invalid Windows File Path Characters
                        ParentName = string.Join("", ParentName.Split(Path.GetInvalidFileNameChars()));
                        Name = string.Join("", Name.Split(Path.GetInvalidFileNameChars()));

                        string parentFolderName = ParentName + Constants.ATTACHMENTSEPARATOR + ParentId;
                        string fileNameExtension = Path.GetExtension(Name);
                        string filePathWithExtension = Utils.GetTempFileName(parentFolderName, Path.GetFileNameWithoutExtension(Name) + fileNameExtension, true, false);
                        // If there 2 files with Same name rename second one to (1), third one to (2) and so on.
                        int count = 1;
                        string newFilePathWithExtension = filePathWithExtension;
                        string attachmentRowValue = row[Constants.ATTACHMENT].ToString();
                        while (!string.IsNullOrEmpty(attachmentRowValue) && attachmentRowValue.Contains(newFilePathWithExtension))
                        {
                            string tempFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(Name), count++);
                            newFilePathWithExtension = Utils.GetTempFileName(parentFolderName, tempFileName + fileNameExtension, true, false);
                        }
                        //
                        row[Constants.ATTACHMENT] = string.IsNullOrEmpty(attachmentRowValue) ? newFilePathWithExtension : row[Constants.ATTACHMENT].ToString() + "|" + newFilePathWithExtension;
                    }
                }
                return;
            }

            if (string.IsNullOrEmpty(value) && (dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(DateTime)))
            {
                row[fieldName] = DBNull.Value;
            }
            else if (dataColumn.DataType == typeof(double) && userInfo != null)
            {
                string[] UserLocale = userInfo.Locale.Split(Constants.UNDERSCORE.ToCharArray());
                if (UserLocale.Count() == 2 && UserLocale[0].Length == 2 && UserLocale[1].Length == 2)
                {
                    //CultureInfo userCulture = new CultureInfo(UserLocale[0] + "-" + UserLocale[1]); // "en-US"
                    // ToDo: Even if the user locale is set to de-DE, the amount comes back with dot as decimal separator and not comma.
                    // Hardcoding it to en-US for now.
                    CultureInfo userCulture = new CultureInfo("en-US");
                    if (userCulture.NumberFormat.NumberDecimalSeparator.Equals(Constants.COMMA))
                        value = value.Replace(Constants.DOT, Constants.COMMA);
                    row[fieldName] = double.Parse(value, userCulture.NumberFormat).ToString();
                }
                else
                {
                    row[fieldName] = value;
                }
            }
            else
            {
                row[fieldName] = value;
            }
        }
        public object[] DataSetToObject(ApttusDataSet dataSet)
        {
            ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(dataSet.AppObjectUniqueID);

            List<object> sObjList = new List<object>();
            foreach (DataRow row in dataSet.DataTable.Rows)
            {
                sObject sObj = new sObject();
                sObj.type = appObject.Id;
                sObj.Id = row["Id"].ToString();

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                List<XmlElement> fieldList = new List<XmlElement>();

                XmlElement field;
                foreach (DataColumn col in dataSet.DataTable.Columns)
                {
                    field = doc.CreateElement(col.ColumnName);
                    field.InnerText = row[col].ToString();
                    fieldList.Add(field);
                }

                sObj.Any = fieldList.ToArray();
                sObjList.Add(sObj);
            }

            return sObjList.ToArray();
        }

        // Get all parnter objects
        public List<ApttusObject> GetAllStandardObjects()
        {
            DescribeGlobalResult results = salesForceConnector.GetAllStandardObjects();
            List<ApttusObject> lstApttusObject = new List<ApttusObject>();
            // Filter unnecessary object such like History , Log etc.
            // Include History Objects for Audit trail like Apps
            List<DescribeGlobalSObjectResult> sFilteredObjects = results.sobjects.Cast<DescribeGlobalSObjectResult>()
                .Where(s => s.keyPrefix != null || s.name.EndsWith(Constants.HISTORY_OBJECT_SUFFIX)).ToList();

            foreach (var item in sFilteredObjects)
            {
                ApttusObject apttusObject = new ApttusObject { Id = item.name, Name = item.label };
                lstApttusObject.Add(apttusObject);
            }
            return lstApttusObject;
        }

        // Get all objects fields
        public ApttusObject GetObjectAndFields(string objectName, bool refreshChildObjects, int noofChildObjectsLoaded)
        {
            DescribeSObjectResult sObject = salesForceConnector.GetObjectAndFields(objectName);
            ApttusObject apttusObject = FillObject(sObject);

            apttusObject.IsFullyLoaded = true;

            if (!refreshChildObjects)
                return apttusObject;

            //Fetch the child objects.
            if (sObject.childRelationships != null)
            {
                List<string> childObjectIds = (from c in sObject.childRelationships
                                               select c.childSObject).ToList();

                List<string> UniquechildObjectIds = new HashSet<string>(childObjectIds).ToList();
                int childObjectCount = UniquechildObjectIds.Count;
                List<DescribeSObjectResult> sChildObjects = new List<DescribeSObjectResult>();
                List<DescribeSObjectResult> sChildObjectsChunk;
                int noOfRecordsTobeRetrieved;
                int noofRecordsRetrieved = noofChildObjectsLoaded;

                if (childObjectCount > 100)
                {
                    childObjectCount = childObjectCount - noofRecordsRetrieved;

                    if (childObjectCount > 100)
                    {
                        noOfRecordsTobeRetrieved = 100;
                        apttusObject.IsFullyLoaded = false;
                    }
                    else
                    {
                        noOfRecordsTobeRetrieved = childObjectCount;
                        apttusObject.IsFullyLoaded = true;
                    }

                    List<string> FirstHundredChilds = UniquechildObjectIds.GetRange(noofRecordsRetrieved, noOfRecordsTobeRetrieved).ToList();
                    sChildObjectsChunk = salesForceConnector.GetObjectsAndFields(FirstHundredChilds);
                    sChildObjects.AddRange(sChildObjectsChunk);
                    FirstHundredChilds.Clear();
                    noofRecordsRetrieved += noOfRecordsTobeRetrieved;
                    apttusObject.NoofChildObjectsLoaded = noofRecordsRetrieved;
                }
                else
                {
                    sChildObjects = salesForceConnector.GetObjectsAndFields(UniquechildObjectIds);
                    apttusObject.IsFullyLoaded = true;
                }

                // Filter unnecessary object such like History, Group Log etc.
                List<DescribeSObjectResult> sChildFilteredObjects = sChildObjects.Where(s => s.keyPrefix != null).ToList();

                foreach (var childRelationship in sObject.childRelationships)
                {
                    DescribeSObjectResult sChildObject = sChildFilteredObjects.FirstOrDefault(c => c.name == childRelationship.childSObject);
                    if (sChildObject != null)
                    {
                        ApttusObject apttusChildObject = FillObject(sChildObject);
                        apttusChildObject.LookupName = childRelationship.field;
                        apttusObject.Children.Add(apttusChildObject);
                    }
                }
            }

            return apttusObject;
        }

        public void FillRecordTypeMetadata(ApttusObject apttusObject)
        {
            // Fetch LayoutResult to fill RecordType
            DescribeLayoutResult LayoutResult = salesForceConnector.GetObjectLayout(apttusObject.Id, apttusObject.RecordTypes.Select(rt => rt.Id).ToArray());

            foreach (ApttusRecordType apttusRecordType in apttusObject.RecordTypes)
            {
                RecordTypeMapping RecordTypeMapping = LayoutResult.recordTypeMappings.Where(rtm => rtm.recordTypeId == apttusRecordType.Id).FirstOrDefault();
                if (RecordTypeMapping != null && RecordTypeMapping.picklistsForRecordType != null)
                {
                    foreach (PicklistForRecordType PicklistForRecordType in RecordTypeMapping.picklistsForRecordType)
                    {
                        apttusRecordType.RecordTypePicklists.Add(new ApttusRecordTypePicklist
                        {
                            PicklistFieldId = PicklistForRecordType.picklistName,
                            PicklistValues = GetPicklistValues(PicklistForRecordType.picklistValues)
                        });
                    }
                }
            }
        }

        // Create new application
        public string createApplication(Guid uniqueId, string name)
        {
            return appBuilderWsController.createApplication(uniqueId, name);
        }

        // Save (update) the application stored in SFDC
        public bool saveApplication(string recordId, byte[] config, byte[] template, string templateName, byte[] schema, string edition, byte[] googleSheetSchema = null)
        {
            edition = ValidateEdition(edition);
            return appBuilderWsController.saveApplication(recordId, Guid.Empty, config, template, templateName, schema, edition, googleSheetSchema);
        }
        public bool deactivateApplication(string recordId)
        {
            return appBuilderWsController.deactivateApplication(recordId);
        }
        public bool saveApplication(Guid uniqueId, byte[] config, byte[] template, string templateName, byte[] schema, string edition)
        {
            edition = ValidateEdition(edition);
            return appBuilderWsController.saveApplication(null, uniqueId, config, template, templateName, schema, edition);
        }

        private string ValidateEdition(string edition)
        {
            if (string.IsNullOrEmpty(edition)) // apps created in <3.9 build will not have edition, set the default to eterprise
            {
                edition = Constants.ENTERPRISE_EDITION;

            }
            else if (edition.Equals("Reporting"))
            {
                // edition = "One-Way";
            }
            else if (edition.Equals("Data Migration"))
            {
                edition = Constants.ADMIN_EDITION;
            }
            return edition;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ApplicationObject LoadApplication(string recordId, Guid uniqueId)
        {
            ApplicationObject App = null;
            LoadAppResult result = appBuilderWsController.loadApplication(recordId, uniqueId);
            if (result != null)
            {
                App = new ApplicationObject();
                App.Config = result.config;
                App.Template = result.xlstemplate;
                App.TemplateName = result.templateName;
            }
            return App;
        }

        public ApplicationObject LoadApplication(string recordId)
        {
            return LoadApplication(recordId, Guid.Empty);
        }

        public ApplicationObject LoadApplication(Guid uniqueId)
        {
            return LoadApplication(null, uniqueId);
        }

        public List<ApttusUserProfile> GetUserProfiles()
        {
            List<ApttusUserProfile> profiles = new List<ApttusUserProfile>();
            string sql = "SELECT Id,Name FROM Profile";
            List<Apttus.XAuthor.SalesforceAdapter.SForce.sObject> lstsObjects = salesForceConnector.Query<Apttus.XAuthor.SalesforceAdapter.SForce.sObject>(sql);

            foreach (var apttusObjects in lstsObjects)
            {
                ApttusUserProfile userProf = new ApttusUserProfile();
                userProf.UserId = apttusObjects.Any.ToList()[0].InnerText;
                userProf.UserName = apttusObjects.Any.ToList()[1].InnerText;
                profiles.Add(userProf);
            }

            return profiles;
        }

        private ApttusObject FillObject(DescribeSObjectResult SFObject)
        {
            List<ApttusField> lstApttusFields = new List<ApttusField>();
            ApttusObject apttusObject = new ApttusObject { Id = SFObject.name, Name = SFObject.label };

            if (SFObject.fields != null)
            {
                if (SFObject.fields.Any(f => f.nameField && !f.name.Equals(Constants.NAME_ATTRIBUTE)))
                {
                    bool bIsNameFieldPresent = false;
                    if (SFObject.fields.Count(f => f.nameField) > 1)
                        bIsNameFieldPresent = SFObject.fields.Any(f => f.nameField && f.name.Equals(Constants.NAME_ATTRIBUTE));

                    if (!bIsNameFieldPresent && !Constants.ObjectsWithoutLookupName.Exists(f => f.Equals(apttusObject.Id)))
                        Constants.ObjectsWithoutLookupName.Add(apttusObject.Id);
                }

                foreach (Field field in SFObject.fields)
                {
                    ApttusField apttusField = new ApttusField
                    {
                        Id = field.name,
                        Name = field.label,
                        Datatype = TranslateSFDataType(field.type, field.extraTypeInfo, field.calculated),
                        PicklistType = TranslateSFPicklist(field),
                        Scale = field.scale,
                        Precision = field.precision,
                        ExternalId = field.externalId,
                        NameField = field.nameField,
                        Updateable = field.updateable,
                        Creatable = field.createable,
                        FormulaType = field.calculated ? field.type.ToString() : string.Empty,
                        Required = (field.nillable == false) && field.createable && field.updateable && (field.permissionable == false)
                    };

                    if (apttusField.Datatype == Datatype.Lookup && field.referenceTo != null)
                    {
                        // For Master-Child relationship, updateable flag is flase as it cannot be reparented, but creatable is true.
                        // so to allow Inserts for now updateable is marked as true, for long term we need additional attribute on ApttusField named Creatable.
                        if (field.createable)
                            apttusField.Updateable = true;

                        if (!field.custom && field.label.EndsWith(" " + Constants.ID_ATTRIBUTE.ToUpper()))
                        {
                            apttusField.Name = apttusField.Name.Replace(" " + Constants.ID_ATTRIBUTE.ToUpper(), "");
                        }
                        apttusField.LookupObject = new ApttusObject
                        {
                            Id = field.referenceTo[GetLookupIndex(field)],
                            ObjectType = ObjectType.Independent
                        };
                        if (field.referenceTo[0] == Constants.RECORDTYPE_OBJECTID && SFObject.recordTypeInfos != null)
                        {
                            apttusField.RecordType = true;
                            AssignRecordTypes(SFObject, apttusObject);
                        }
                    }
                    else if (apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Picklist_MultiSelect || apttusField.Datatype == Datatype.Editable_Picklist)
                    {
                        if (field.picklistValues != null)
                        {
                            apttusField.PicklistValues = GetPicklistValues(field.picklistValues);

                            if (apttusField.Datatype == Datatype.Editable_Picklist)
                                apttusField.PicklistType = PicklistType.Regular;

                            if (apttusField.PicklistType == PicklistType.Dependent)
                            {
                                // Fill DependentPicklist Values
                                bool bSuccess = AssignDependentPicklist(field, apttusField, SFObject);
                                // If Dependent Picklist cannot access the Controling Picklist field, treat it as a Regular Picklist.
                                // This scenario has been observed in Apttus Prod Org, Live Nation Prod Org and is a valid scenario controlled by Field Level security of Controling Picklist.
                                if (!bSuccess)
                                    apttusField.PicklistType = PicklistType.Regular;
                            }
                        }
                    }
                    lstApttusFields.Add(apttusField);
                }
                apttusObject.Fields = lstApttusFields;
            }
            return apttusObject;

        }

        int GetLookupIndex(Field field)
        {
            if (field.name == "OwnerId")
            {
                int index = Array.IndexOf(field.referenceTo, "User");
                return index == -1 ? 0 : index;
            }
            else
            {
                return 0;
            }
        }

        private List<string> GetPicklistValues(PicklistEntry[] picklistEntries)
        {
            List<string> PicklistValues = new List<string>();
            foreach (PicklistEntry PicklistEntry in picklistEntries)
                PicklistValues.Add(PicklistEntry.value);

            return PicklistValues;
        }

        /// <summary>
        /// This function will set the following values for the Dependent Apttus Field
        /// 1. ControllingPicklistFieldId
        /// 2. DependentPicklistValues
        /// </summary>
        /// <param name="field">Current Salesforce Field</param>
        /// <param name="apttusField"> Current Dependent Apttus Field</param>
        /// <param name="SFObject">Current Salesforce Object</param>
        /// <param name="apttusObject">Current Apttus Object which is being filled</param>
        private bool AssignDependentPicklist(Field field, ApttusField apttusField, DescribeSObjectResult SFObject)
        {
            bool bSupportedDependentPicklist = false;
            //apttusField.PicklistValues = new List<string>(); // Assign memory to picklist values.
            PicklistEntry[] PicklistEntries = field.picklistValues;

            // 1. Find the Controlling Picklist Field Id.
            string ControllingFieldId = SFObject.fields.FirstOrDefault(f => f.name == apttusField.Id).controllerName;
            List<DependentPicklistItem> DependentPicklistValues = new List<DependentPicklistItem>();

            // 2.0 Create PicklistItems collection based on the valid values
            if (!string.IsNullOrEmpty(ControllingFieldId) && PicklistEntries.Count() > 0)
            {
                // Valid Dependent Picklist if Controlling Picklist found and at least one value is present.
                bSupportedDependentPicklist = true;

                // 2.1 Find the controller field
                Field Controller = SFObject.fields.FirstOrDefault(f => f.name == ControllingFieldId);

                if (Controller != null)
                {
                    foreach (PicklistEntry PicklistEntry in PicklistEntries)
                    {
                        // 2.2 For each PicklistEntry: find all controlling values for which it is valid.
                        Bitset ValidForBitset = new Bitset(PicklistEntry.validFor);

                        // 2.3 If thre Controlling and Dependent are both Picklist types.
                        // Salesforce also supports Dependent Picklist for Checkbox. This is not implemented.
                        if (field.type == Controller.type)
                        {
                            for (int index = 0; index < ValidForBitset.size(); index++)
                            {
                                if (ValidForBitset.testBit(index))
                                {
                                    string CurrentControllingValue = Controller.picklistValues[index].label;
                                    DependentPicklistItem DependentPicklistItem = DependentPicklistValues.FirstOrDefault(dpv => dpv.ControllingValue == CurrentControllingValue);
                                    if (DependentPicklistItem == null)
                                    {
                                        DependentPicklistItem = new DependentPicklistItem
                                        {
                                            ControllingValue = CurrentControllingValue,
                                            PicklistValues = new List<string>()
                                        };
                                        DependentPicklistValues.Add(DependentPicklistItem);
                                    }
                                    DependentPicklistItem.PicklistValues.Add(PicklistEntry.value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Controller not found for the dependent picklist
                    bSupportedDependentPicklist = false;
                }
            }
            if (bSupportedDependentPicklist)
            {
                apttusField.ControllingPicklistFieldId = ControllingFieldId;
                apttusField.DependentPicklistValues = DependentPicklistValues;
            }

            return bSupportedDependentPicklist;
        }

        private void AssignRecordTypes(DescribeSObjectResult SFObject, ApttusObject apttusObject)
        {
            RecordTypeInfo[] RecordTypeInfoCollection = SFObject.recordTypeInfos;
            if (RecordTypeInfoCollection.Length > 0)
            {
                apttusObject.RecordTypes = new List<ApttusRecordType>();
                foreach (RecordTypeInfo RecordTypeInfo in RecordTypeInfoCollection.Where(rt => rt.available))
                {
                    ApttusRecordType apttusRecordType = new ApttusRecordType
                    {
                        Id = RecordTypeInfo.recordTypeId,
                        Name = RecordTypeInfo.name,
                        Default = RecordTypeInfo.defaultRecordTypeMapping,
                        RecordTypePicklists = new List<ApttusRecordTypePicklist>()
                    };
                    apttusObject.RecordTypes.Add(apttusRecordType);
                }

                //// Fetch LayoutResult to fill RecordType
                //DescribeLayoutResult LayoutResult = salesForceConnector.GetObjectLayout(apttusObject.Id, apttusObject.RecordTypes.Select(rt => rt.Id).ToArray());
                //FillRecordTypePicklist(LayoutResult, apttusObject);
            }
        }

        private Datatype TranslateSFDataType(fieldType fieldType, string extraTypeInfo, bool isCalculatedField)
        {
            Datatype dt = new Datatype();

            if (isCalculatedField)
            {
                dt = Datatype.Formula;
                return dt;
            }

            switch (fieldType)
            {
                case SalesforceAdapter.SForce.fieldType.@string:
                case SalesforceAdapter.SForce.fieldType.id:
                case SalesforceAdapter.SForce.fieldType.phone:
                case SalesforceAdapter.SForce.fieldType.url:
                    dt = Datatype.String;
                    break;
                case SalesforceAdapter.SForce.fieldType.picklist:
                    dt = Datatype.Picklist;
                    break;
                case SalesforceAdapter.SForce.fieldType.multipicklist:
                    dt = Datatype.Picklist_MultiSelect;
                    break;
                case SalesforceAdapter.SForce.fieldType.reference:
                    dt = Datatype.Lookup;
                    break;
                case SalesforceAdapter.SForce.fieldType.@double:
                case SalesforceAdapter.SForce.fieldType.@int:
                case SalesforceAdapter.SForce.fieldType.percent:
                    dt = Datatype.Double;
                    break;
                case SalesforceAdapter.SForce.fieldType.currency:
                    dt = Datatype.Decimal;
                    break;
                case SalesforceAdapter.SForce.fieldType.date:
                    dt = Datatype.Date;
                    break;
                case SalesforceAdapter.SForce.fieldType.datetime:
                    dt = Datatype.DateTime;
                    break;
                case SalesforceAdapter.SForce.fieldType.boolean:
                    dt = Datatype.Boolean;
                    break;
                case SalesforceAdapter.SForce.fieldType.textarea:
                    if (string.IsNullOrEmpty(extraTypeInfo) || Convert.ToString(extraTypeInfo).Equals("plaintextarea"))
                        dt = Datatype.Textarea;
                    else if (Convert.ToString(extraTypeInfo).Equals("richtextarea"))
                        dt = Datatype.Rich_Textarea;
                    break;
                case SalesforceAdapter.SForce.fieldType.email:
                    dt = Datatype.Email;
                    break;
                case SalesforceAdapter.SForce.fieldType.anyType:
                    dt = Datatype.AnyType;
                    break;
                case SalesforceAdapter.SForce.fieldType.combobox:
                    dt = Datatype.Editable_Picklist;
                    break;
                case SalesforceAdapter.SForce.fieldType.base64:
                    dt = Datatype.Base64;
                    break;
                default:
                    dt = Datatype.NotSupported;
                    break;
            }
            return dt;
        }


        private PicklistType TranslateSFPicklist(Field field)
        {
            PicklistType pt = new PicklistType();

            if (field.type == fieldType.picklist)
                if (field.dependentPicklist)
                    pt = PicklistType.Dependent;
                else
                    pt = PicklistType.Regular;

            return pt;
        }

        private string getAppQueryStr(int? maxRecords, string searchName, string ownerId)
        {
            bool hasWhere = false;

            string queryStr = "SELECT Id, Name, Owner.Id, Owner.Name, LastModifiedDate, "
                        + Constants.NAMESPACE_PREFIX + "UniqueId__c FROM "
                        + Constants.NAMESPACE_PREFIX
                        + "Application__c";

            // Only return activated if user is not designer
            if (!IsDesigner() && !IsAdminPackageEditionUser()) // for not for admin, admin should be able to run any app without activation
            {
                hasWhere = true;

                ApttusUserInfo userInfo = getUserInfo();

                queryStr += " WHERE " + Constants.NAMESPACE_PREFIX + "Activated__c = true"
                            + " AND Id IN (SELECT " + Constants.NAMESPACE_PREFIX + "ApplicationId__c FROM "
                            + Constants.NAMESPACE_PREFIX + "UserXAuthorApp__c WHERE "
                            + Constants.NAMESPACE_PREFIX + "ProfileId__c = '" + userInfo.ProfileId + "' OR "
                            + Constants.NAMESPACE_PREFIX + "UserId__c = '" + userInfo.UserId + "')";
            }

            if (!String.IsNullOrEmpty(searchName))
            {
                // Escape user input
                searchName.Replace("%", "/%");
                searchName.Replace("'", "/'");

                if (hasWhere)
                    queryStr += " AND Name LIKE '%" + searchName + "%'";
                else
                    queryStr += " WHERE Name LIKE '%" + searchName + "%'";

                hasWhere = true;
            }


            if (!String.IsNullOrEmpty(ownerId))
            {
                if (hasWhere)
                    queryStr += " AND Owner.Id = '" + ownerId + "'";
                else
                    queryStr += " WHERE Owner.Id = '" + ownerId + "'";
            }

            queryStr += " ORDER BY LastModifiedDate DESC";

            if (maxRecords != null)
                queryStr += " LIMIT " + maxRecords;

            return queryStr;
        }

        public List<ApttusObject> GetAppList(int? maxRecords, string searchName, string ownerId)
        {
            return Query<ApttusObject>(getAppQueryStr(maxRecords, searchName, ownerId));
        }

        public ApttusDataSet GetAppDataSet(int? maxRecords, string searchName, string ownerId, ApttusUserInfo userInfo)
        {
            return QueryDataSet(getAppQueryStr(maxRecords, searchName, ownerId), null, null, userInfo);
        }


        public String getNamespacePrefix()
        {
            try
            {
                QueryResult res = salesForceConnector.Query("SELECT count() FROM ApexClass where NamespacePrefix='"
                            + NAMESPACE_PREFIX + "'");

                if (res.size > 0)
                    return NAMESPACE_PREFIX + "__";
                else
                    return String.Empty;
            }
            catch (Exception e)
            {
                // If user can't access ApexClass object, assume we are on a managed org
                return NAMESPACE_PREFIX + "__";
            }
        }

        public bool IsDesigner()
        {
            return _IsDesigner;
        }

        public string GetClientVersion()
        {
            return appBuilderWsController.GetClientVersion();
        }

        public string GetFeatureDetail()
        {
            return appBuilderWsController.GetFeatureDetail();
        }

        public string GetLicenseDetail()
        {
            return appBuilderWsController.GetLicenseDetail();
        }

        /* this call is not required. Server handle the logic
        public void SubmitSyncLicenseAndFeatureDetail()
        {
            appBuilderWsController.SubmitSyncLicenseAndFeatureDetail();
        }

        public void SyncLicenseAndFeatureDetail()
        {
            appBuilderWsController.SyncLicenseAndFeatureDetail();
        }

        */

        public string EscapeQueryString(string queryString)
        {
            string result = queryString;

            if (!string.IsNullOrEmpty(queryString))
            {
                Dictionary<string, string> EscapeMapping = new Dictionary<string, string> {
                {@"\", @"\\"}, // This has to be the 1st key value pair.
                {@"'", @"\'"}
                };

                foreach (var entry in EscapeMapping)
                {
                    // Allow Null values in cell reference
                    if (result.Equals(@"''"))
                        result = result.Replace(@"''", @"");
                    else
                        result = result.Replace(entry.Key, entry.Value);

                }
            }

            return result;
        }

        public string UnescapeQueryString(string queryString)
        {
            string result = queryString;
            if (!string.IsNullOrEmpty(queryString))
            {
                Dictionary<string, string> UnescapeMapping = new Dictionary<string, string> {
                { @"\'",@"'"},
                {@"\\",@"\" }, // This has to be the last key value pair.
                };

                foreach (var entry in UnescapeMapping)
                    result = result.Replace(entry.Key, entry.Value);
            }
            return result;
        }

        private void SetWaitMessage(WaitWindowView waitWindow, QueryTypes queryType, int current, int total)
        {
            if (waitWindow != null)
                waitWindow.Message = String.Format(resourceManager.GetResource("COREABSADAPTER_SetWaitMessage_Msg"), queryType.ToString(), current.ToString(), total.ToString(), resourceManager.CRMName);
        }

        public string GetExceptionCode(Exception ex)
        {
            string result = string.Empty;

            if (ex.Source != null && ex.Source.ToLower() == "autofac")
            {
                Exception sfdcException = ex.InnerException;
                while (sfdcException != null)
                {
                    result = GetExceptionCodeValue(sfdcException);
                    if (!string.IsNullOrEmpty(result))
                        break;
                    sfdcException = sfdcException.InnerException;
                }
            }
            else
                result = GetExceptionCodeValue(ex);

            return result;
        }

        private string GetExceptionCodeValue(Exception ex)
        {
            string result = string.Empty;

            if (ex != null && ex.GetType() == typeof(System.ServiceModel.FaultException<SalesforceAdapter.SForce.UnexpectedErrorFault>))
            {
                var faultException = (System.ServiceModel.FaultException<SalesforceAdapter.SForce.UnexpectedErrorFault>)ex;
                result = faultException.Code.Name;
            }
            return result;
        }

        // Return if current user has Admin Pacakge permission 
        //true - User has access to Admin Package 
        public bool IsAdminPackageEditionUser()
        {
            if (Constants.ISADMINPACKAGEINSTALLED == null)
                Constants.ISADMINPACKAGEINSTALLED = appBuilderWsController.IsAdminPackageEditionUser();

            bool bIsAdminPackageInstalled = Constants.ISADMINPACKAGEINSTALLED.HasValue ? Constants.ISADMINPACKAGEINSTALLED.Value : false;
            return bIsAdminPackageInstalled;
        }

        // Return if Org is a sandbox or not 
        // true -> Sandbox
        public bool IsSandBox()
        {
            return _IsSandBox;
        }
        public string GetAppEdition(string recId, string uniqueID)
        {
            /* 3.9 all apps should have a edition and read the edition from the app object */
            /* <3.9 will have enterprise as the default edition */
            string sql = null;
            string Edition = null;

            string queryStr = "SELECT "
                        + Constants.NAMESPACE_PREFIX + "Edition__c FROM "
                        + Constants.NAMESPACE_PREFIX
                        + "Application__c"
                        + " Where Id =" + "'" + recId + "'";


            try
            {
                List<Apttus.XAuthor.SalesforceAdapter.SForce.sObject> lstsObjects = salesForceConnector.Query<Apttus.XAuthor.SalesforceAdapter.SForce.sObject>(queryStr);
                Edition = lstsObjects[0].Any[0].InnerText;
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowInfo(ex.Message.ToString(), "GetAppEdition");
            }

            return Edition;
        }

        public bool Connect(IXAuthorCRMLogin login)
        {
            IXAuthorSalesforceLogin sfLogin = login as IXAuthorSalesforceLogin;
            return Connect(sfLogin.AccessToken, sfLogin.InstanceURL, sfLogin.Proxy);
        }

        public bool IsAdminUser()
        {
            return IsAdminPackageEditionUser();
        }

        public List<ApttusObject> Query<T>(XAuthorQuery query)
        {
            SalesforceQuery sfQuery = query as SalesforceQuery;
            return Query<T>(sfQuery.SOQL);
        }

        public ApttusDataSet QueryDataSet(XAuthorQuery query)
        {
            SalesforceQuery sfQuery = query as SalesforceQuery;
            return QueryDataSet(sfQuery.SOQL, sfQuery.Object, sfQuery.DataTable, sfQuery.UserInfo);
        }

        public string GetLicenseFilePath()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus\\");

            DirectoryInfo di = new DirectoryInfo(folderName);
            if (!di.Exists)
                di.Create();

            return folderName;
        }

        public bool HasCollisionDetection()
        {
            return true;
        }

        public ILookupIdAndLookupNameProvider GetLookUpIdAndNameProvider()
        {
            if (LookupIdAndLookupNameProvider == null)
                LookupIdAndLookupNameProvider = new SalesforceLookupIdAndNameProvider();
            return LookupIdAndLookupNameProvider;
        }

        public void SetAppDataTableProperties(ref DataTable table)
        { }

        public ApttusDataSet GetAttachmentsDataSet(string ParentIds, string ObjectId)
        {
            string SOQL = "SELECT Id, ParentId, Name, Body,Parent.Name  FROM Attachment WHERE ParentId In (" + ParentIds + ")";
            return QueryDataSet(new SalesforceQuery() { SOQL = SOQL });
        }

        public ApttusDataSet GetLookUpDetails(ApttusObject apttusObject, List<string> LookupNames)
        {
            string query = "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + apttusObject.Id
                 + " WHERE " + Constants.NAME_ATTRIBUTE + " IN "
                 + Constants.OPEN_BRACKET + Constants.QUOTE +
                 String.Join(Constants.QUOTE + Constants.COMMA + Constants.QUOTE, LookupNames.ToArray()) +
                 Constants.QUOTE + Constants.CLOSE_BRACKET;

            //check if query length crosses 20k then chunk & execute it else simply execute it
            ApttusDataSet resultData = null;
            if (query.Length >= Constants.SOQL_MAX_LENGTH)
            {
                ChunkQueries CH = new ChunkQueries();
                List<string> queries = CH.PrepareChunkedQueriesFromList(LookupNames, "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + apttusObject.Id, Constants.NAME_ATTRIBUTE);

                ApttusDataSet ds = new ApttusDataSet()
                {
                    DataTable = new DataTable()
                };
                foreach (string sfQuery in queries)
                {
                    ds.DataTable.Merge(QueryDataSet(new SalesforceQuery { SOQL = sfQuery }).DataTable);
                }
                return ds;
            }
            else
            {
                resultData = QueryDataSet(new SalesforceQuery { SOQL = query });
            }

            return resultData;
        }

        public ApttusDataSet GetDataSetByLookupSearch(ApttusObject lookupObj, string search)
        {
            ApttusDataSet ds = new ApttusDataSet();
            var soql = string.Empty;
            if (string.IsNullOrEmpty(search))
            {
                soql = string.Format("select id, name from  RecentlyViewed WHERE Type IN ('{0}') ORDER BY LastViewedDate limit 20", lookupObj.Id);
                ds = QueryDataSet(new SalesforceQuery() { SOQL = soql });
                if (ds != null && ds.DataTable != null)
                {
                    if (ds.DataTable.Rows.Count < 5)
                    {
                        soql = string.Format("select id, name from {0}", lookupObj.Id);
                        ds = QueryDataSet(new SalesforceQuery() { SOQL = soql });
                    }
                }
            }
            else
            {
                soql = string.Format("select id, name from {0} where name like '{1}%'", lookupObj.Id, search);
                ds = QueryDataSet(new SalesforceQuery() { SOQL = soql });
            }
            return ds;
        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            return string.Empty;
        }

        public void RefreshConnection()
        {
        }

        public AppAssignmentModel GetAppAssignmentDetails(string AppId)
        {
            string[] Columns = { "Id",
                                 "Name",
                                 "Apttus_XApps__ApplicationId__c",
                                 "Apttus_XApps__ProfileId__c",
                                 "Apttus_XApps__UserId__r.Name",
                                 "Apttus_XApps__UserId__r.Id",
                                 "Apttus_XApps__UserId__r.Email"
            };
            string SOQL = "SELECT " + string.Join(",", Columns) +
                          " FROM Apttus_XApps__UserXAuthorApp__c WHERE " +
                          "Apttus_XApps__ApplicationId__c='" + AppId + "'";
            DataTable dataTable = new DataTable();
            foreach (string col in Columns)
            {
                dataTable.Columns.Add(new DataColumn(col));
            }
            ApttusDataSet apttusDataSet = QueryDataSet(new SalesforceQuery() { SOQL = SOQL, UserInfo = UserInfo, DataTable = dataTable });

            AppAssignmentModel Model = new AppAssignmentModel
            {
                ApplicationId = AppId
            };
            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                Model.Assignments.Add(new Assignments()
                {
                    AssignmentId = row["Id"].ToString(),
                    AssignmentName = row["Name"].ToString(),
                    User = (apttusDataSet.DataTable.Columns.Contains("Apttus_XApps__UserId__r.Id") && !string.IsNullOrEmpty(row["Apttus_XApps__UserId__r.Id"].ToString())) ? new User()
                    {
                        Id = row["Apttus_XApps__UserId__r.Id"].ToString(),
                        Name = row["Apttus_XApps__UserId__r.Name"].ToString(),
                        Email = row["Apttus_XApps__UserId__r.Email"].ToString(),
                        Checked = true
                    } : null,
                    Profile = !string.IsNullOrEmpty(row["Apttus_XApps__ProfileId__c"].ToString()) ? new Profile()
                    {
                        Id = row["Apttus_XApps__ProfileId__c"].ToString(),
                        Name = null,
                        Checked = true
                    } : null,
                });
            }
            return Model;
        }

        public List<User> GetUsersList(string searchStr, string ExceptIds)
        {
            string SOQL = "Select Name,Id,Email FROM User ";
            bool hasWhere = false;
            if (!string.IsNullOrEmpty(searchStr))
            {
                hasWhere = true;
                SOQL += "WHERE (Name Like '%" + searchStr + "%' OR Email Like '%" + searchStr + "%')";
            }
            if (!string.IsNullOrEmpty(ExceptIds))
            {
                if (hasWhere)
                {
                    SOQL += " AND Id NOT IN (" + ExceptIds + ")";
                }
                else
                {
                    SOQL += " WHERE Id NOT IN (" + ExceptIds + ")";
                }
            }
            SOQL += "ORDER BY Name ASC LIMIT 500";
            ApttusDataSet apttusDataSet = QueryDataSet(new SalesforceQuery() { SOQL = SOQL, UserInfo = UserInfo });
            List<User> users = new List<User>();

            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                users.Add(new User
                {
                    Id = row["Id"].ToString(),
                    Name = row["Name"].ToString(),
                    Email = row["Email"].ToString(),
                    Checked = false
                });
            }
            return users;
        }

        public List<Profile> GetProfileList(string searchStr, string ExceptIds)
        {
            string SOQL = "Select Name,Id FROM Profile ";
            bool hasWhere = false;
            if (!string.IsNullOrEmpty(searchStr))
            {
                hasWhere = true;
                SOQL += "WHERE Name Like '%" + searchStr + "%' ";
            }

            if (!string.IsNullOrEmpty(ExceptIds))
            {
                if (hasWhere)
                {
                    SOQL += " AND Id NOT IN (" + ExceptIds + ")";
                }
                else
                {
                    SOQL += " WHERE Id NOT IN (" + ExceptIds + ")";
                }
            }
            SOQL += "ORDER BY Name ASC";

            ApttusDataSet apttusDataSet = QueryDataSet(new SalesforceQuery() { SOQL = SOQL, UserInfo = UserInfo });
            List<Profile> profiles = new List<Profile>();
            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                profiles.Add(new Profile
                {
                    Id = row["Id"].ToString(),
                    Name = row["Name"].ToString(),
                    Checked = false
                });
            }
            return profiles;
        }
    }

    public class Bitset
    {
        // helper class to decode a "validFor" bitset class Bitset
        byte[] data;
        public Bitset(byte[] data)
        {
            this.data = data == null ? new byte[0] : data;
        }
        public bool testBit(int n)
        {
            return (data[n >> 3] & (0x80 >> n % 8)) != 0;
        }
        public int size()
        {
            return data.Length * 8;
        }
    }
}
