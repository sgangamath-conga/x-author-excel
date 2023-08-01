using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Apttus.XAuthor.AICAdapter;
using MetadataDTO = Apttus.Metadata.Common.DTO.Runtime.V1;
using Apttus.DataAccess.Common.Model;
using AQLEnums = Apttus.DataAccess.Common.Enums;
using System.Globalization;

namespace Apttus.XAuthor.Core
{
    public class ABAICAAdapter : IAdapter
    {
        protected AICServiceController aicConnector = AICServiceController.GetInstance;
        private ILookupIdAndLookupNameProvider LookupIdAndLookupNameProvider;
        private List<ApttusObject> lstApttusObject;
        private static string APP_OBJECT = "xae_App";
        protected ApttusUserInfo apttusUserInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public virtual bool Connect(IXAuthorCRMLogin login)
        {
            IXAuthorAICLogin loginctl = login as IXAuthorAICLogin;
            var bConnected = aicConnector.ConnectWithAIC(loginctl.AuthenticationResult, loginctl.TenantURL, loginctl.Proxy);
            AICRefreshSession.GetInstance.SetRefreshSessionInfo(loginctl.ClientAppKey, loginctl.AuthorityURI, loginctl.ResourceAppKey);

            //This is needed so that the field level security is working in case of AIC. We need to set the namespace prefix so that we can come to know whether the user is in offline mode.
            if (bConnected)
                Constants.NAMESPACE_PREFIX = "Dummy_Namespace_Prefix Which is not used in aic";

            apttusUserInfo = getUserInfo();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ApttusObject> GetAllStandardObjects()
        {
            try
            {
                lstApttusObject = new List<ApttusObject>();
                string response = aicConnector.GetAllStandardObjects();

                List<MetadataDTO.ObjectMetadataDTO> allObjs = JsonConvert.DeserializeObject<List<MetadataDTO.ObjectMetadataDTO>>(response);

                foreach (MetadataDTO.ObjectMetadataDTO item in allObjs)
                {
                    ApttusObject apttusObject = new ApttusObject
                    {
                        Id = item.Name,
                        Name = item.DisplayName
                    };
                    lstApttusObject.Add(apttusObject);
                }
                return lstApttusObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="refreshChildObjects"></param>
        /// <param name="noofChildObjectsLoaded"></param>
        /// <returns></returns>
        public ApttusObject GetObjectAndFields(string objectName, bool refreshChildObjects, int noofChildObjectsLoaded)
        {
            string response = aicConnector.GetObjectAndFields(objectName);
            MetadataDTO.ObjectMetadataDTO obj = JsonConvert.DeserializeObject<MetadataDTO.ObjectMetadataDTO>(response);

            ApttusObject apttusObject = FillObject(obj);

            apttusObject.IsFullyLoaded = true;

            if (!refreshChildObjects)
                return apttusObject;

            //Fetch the child objects.
            if (obj.Relationships != null)
            {
                foreach (var childRelationship in obj.Relationships)
                {
                    ApttusObject relObj = lstApttusObject.Find(o => Convert.ToString(o.Id).Trim().ToLower() == Convert.ToString(childRelationship.Value.ObjectName).Trim().ToLower());
                    ApttusObject apttusChildObject = new ApttusObject
                    {
                        Id = childRelationship.Value.ObjectName,
                        Name = relObj.Name,
                        Fields = new List<ApttusField>(),
                        ObjectType = ObjectType.Repeating
                    };

                    apttusChildObject.LookupName = childRelationship.Value.FieldName.Split('.')[1];
                    apttusObject.Children.Add(apttusChildObject);
                }
            }
            return apttusObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectMetadata"></param>
        /// <returns></returns>
        private ApttusObject FillObject(MetadataDTO.ObjectMetadataDTO objectMetadata)
        {

            if (lstApttusObject == null)
                GetAllStandardObjects();

            List<ApttusField> lstApttusFields = new List<ApttusField>();
            ApttusObject apttusObject = new ApttusObject { Id = objectMetadata.Name, Name = objectMetadata.DisplayName };

            ///Guid objId;
            // Assign GUID from Object metadata only to be used for relational object later on for queries
            //if (Guid.TryParse(objectMetadata.Id, out objId))
            //{
            //    apttusObject.UniqueId = objId;
            //}
            //apttusObject.UniqueId = Guid.NewGuid();

            if (objectMetadata.Fields != null)
            {
                foreach (var field in objectMetadata.Fields)
                {
                    ApttusField apttusField = new ApttusField
                    {
                        Id = field.Key,
                        Name = field.Value.DisplayName,
                        Datatype = TranslateAICDataType(field.Value.Type, field.Value.IsCalculated),
                        PicklistType = TranslateAICPicklist(field),
                        Scale = field.Value.Scale,
                        Precision = field.Value.Precision,
                        ExternalId = field.Value.IsExternalId,
                        NameField = field.Value.IsPrimaryName,
                        Updateable = true,
                        Required = field.Value.IsRequired,
                        CRMDataType = field.Value.Type.GetDescription(),
                        FormulaType = field.Value.IsCalculated ? field.Value.Formula : string.Empty,
                        Visible = true
                    };

                    if (apttusField.Datatype == Datatype.Lookup)
                    {
                        if ((apttusField.CRMDataType.Equals("Lookup") || apttusField.CRMDataType.Equals("Child")) && field.Value.Relationship != null)
                        {
                            apttusField.LookupObject = new ApttusObject
                            {
                                Id = field.Value.Relationship.TargetObjectName,
                                ObjectType = ObjectType.Independent
                            };

                            // Fill multiObject lookup as well, for future use
                            if (field.Value.CompositeObjectTypes != null && field.Value.CompositeObjectTypes.Count > 1)
                                apttusField.MultiLookupObjects = GetMultiLookupObjectList(field, apttusField);
                        }
                    }
                    //Keeping this block outside of lookup logic as we added a new datatype for composite objects
                    else if (apttusField.Datatype == Datatype.Composite && field.Value.CompositeObjectTypes != null)
                    {
                        // This is multi object lookup in AIC called composite 
                        // TODO:: verify retrieve and save behaviour for this field
                        apttusField.MultiLookupObjects = GetMultiLookupObjectList(field, apttusField);

                        // This block has been added to initizlize the lookup objects so that it does not remain null for that repective field and code does not break at the time of mapping in designer
                        // The logic has been borrowed from ABDynamicsCRM Adaper where field's lookup object has been assigned from refernce object's 0th index
                        if (apttusField.MultiLookupObjects != null && apttusField.MultiLookupObjects.Count > 0)
                        {
                            apttusField.LookupObject = apttusField.MultiLookupObjects.FirstOrDefault();
                        }
                    }
                    else if (apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Picklist_MultiSelect)
                    {
                        if (field.Value.SelectOptionSet != null)
                        {
                            apttusField.PicklistValues = GetPicklistValues(field.Value.SelectOptionSet);
                            // earlier Metadata used to return keyValue pair for Picklist, but now sends both as string hence commenting below line.
                            apttusField.PicklistKeyValues = GetPicklistKeyValues(field.Value.SelectOptionSet);
                        }
                    }

                    lstApttusFields.Add(apttusField);
                }
                apttusObject.Fields = lstApttusFields;
            }

            return apttusObject;
        }

        private List<PicklistKeyValuePair> GetPicklistKeyValues(MetadataDTO.SelectOptionSetMetadataDTO selectOptionSet)
        {
            List<PicklistKeyValuePair> PicklistKeyValues = new List<PicklistKeyValuePair>();
            foreach (var optionValue in selectOptionSet.SelectOptions)
            {
                if (optionValue != null)
                {
                    PicklistKeyValuePair picklistItem = new PicklistKeyValuePair();
                    picklistItem.optionKey = optionValue.OptionKey;
                    picklistItem.optionValue = optionValue.OptionValue;
                    PicklistKeyValues.Add(picklistItem);
                }
            }
            return PicklistKeyValues;
        }

        private List<string> GetPicklistValues(MetadataDTO.SelectOptionSetMetadataDTO selectOptionSet)
        {
            List<string> picklistOptions = new List<string>();

            foreach (var optionValue in selectOptionSet.SelectOptions)
            {
                if (optionValue != null)
                    picklistOptions.Add(optionValue.OptionValue);
            }
            return picklistOptions;
        }

        private List<ApttusObject> GetMultiLookupObjectList(KeyValuePair<string, MetadataDTO.FieldMetadataDTO> field, ApttusField apttusField)
        {
            List<ApttusObject> apttusObjectList = new List<ApttusObject>();

            if (apttusField.Datatype == Datatype.Composite && field.Value.CompositeObjectTypes != null)
            {
                foreach (string lookupobjid in field.Value.CompositeObjectTypes)
                {
                    ApttusObject lookupObject = lstApttusObject.FirstOrDefault(o => o.Id.Equals(lookupobjid));
                    ApttusObject obj = new ApttusObject
                    {
                        Id = lookupobjid,
                        Name = lookupObject != null ? lookupObject.Name : string.Empty,
                        ObjectType = ObjectType.Independent
                    };

                    apttusObjectList.Add(obj);
                }
            }

            return apttusObjectList;
        }

        private PicklistType TranslateAICPicklist(KeyValuePair<string, MetadataDTO.FieldMetadataDTO> field)
        {
            return PicklistType.Regular;
        }

        private Datatype TranslateAICDataType(AQLEnums.DataType dataType, bool isCalculatedField)
        {
            Datatype dt = new Datatype();

            if (isCalculatedField)
            {
                dt = Datatype.Formula;
                return dt;
            }

            switch (dataType)
            {
                case AQLEnums.DataType.UniqueIdentifier:
                case AQLEnums.DataType.AutoNumber:
                case AQLEnums.DataType.String:
                case AQLEnums.DataType.RowVersion:
                    dt = Datatype.String;
                    break;
                case AQLEnums.DataType.Bool:
                    dt = Datatype.Boolean;
                    break;
                case AQLEnums.DataType.Date:
                    dt = Datatype.Date;
                    break;
                case AQLEnums.DataType.DateTime:
                    dt = Datatype.DateTime;
                    break;
                case AQLEnums.DataType.Decimal:
                case AQLEnums.DataType.Money:
                    dt = Datatype.Decimal;
                    break;
                case AQLEnums.DataType.Integer:
                case AQLEnums.DataType.Quantity:
                    dt = Datatype.Double;
                    break;
                case AQLEnums.DataType.LongString:
                    dt = Datatype.Textarea;
                    break;
                case AQLEnums.DataType.Lookup:
                case AQLEnums.DataType.Child:
                    dt = Datatype.Lookup;
                    break;
                case AQLEnums.DataType.Composite:
                    dt = Datatype.Composite;
                    break;
                case AQLEnums.DataType.MultiSelectOption:
                    dt = Datatype.Picklist_MultiSelect;
                    break;
                case AQLEnums.DataType.SelectOption:
                    dt = Datatype.Picklist;
                    break;
                default:
                    dt = Datatype.NotSupported;
                    break;
                    // TODO:: figure out about this data types what it does, currently will go as not supported
                    //case AQLEnums.DataType.Child:
            }

            return dt;
        }

        public void FillRecordTypeMetadata(ApttusObject apttusObject)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ApttusUserInfo getUserInfo()
        {
            apttusUserInfo = new ApttusUserInfo();

            string response = aicConnector.GetUserInfo();
            Apttus.DataAccess.Common.Model.UserInfo userInfo = JsonConvert.DeserializeObject<Apttus.DataAccess.Common.Model.UserInfo>(response);

            apttusUserInfo.UserId = Convert.ToString(userInfo.UserId);
            apttusUserInfo.UserName = userInfo.PrimaryEmail;
            apttusUserInfo.UserEmail = userInfo.PrimaryEmail;
            apttusUserInfo.UserFullName = userInfo.FirstName + " " + userInfo.LastName;
            apttusUserInfo.ProfileId = string.Empty;
            apttusUserInfo.Locale = string.Empty;
            apttusUserInfo.Language = string.Empty;
            apttusUserInfo.OrganizationId = userInfo.TenantId;
            return apttusUserInfo;
        }

        public bool IsDesigner()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxRecords"></param>
        /// <param name="searchName"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public List<ApttusObject> GetAppList(int? maxRecords, string searchName, string ownerId)
        {
            Query getAppQuery = getAppQueryExpr(maxRecords, searchName, ownerId);
            return Query<ApttusObject>(getAppQuery);
        }

        private Query getAppQueryExpr(int? maxRecords, string searchName, string ownerId)
        {
            Query searchAppQuery = new Query(APP_OBJECT, false);
            List<string> objcolumns = new List<string> { "Id", "Name", "UniqueId", "Activated", "ModifiedOn", "CreatedById", "CreatedById.Name" };
            searchAppQuery.Columns = objcolumns;
            searchAppQuery.AddSortOrder("ExternalLastUpdatedOn", DataAccess.Common.Enums.SortOrder.DESC);
            searchAppQuery.TopRecords = Convert.ToInt32(maxRecords);

            if (!string.IsNullOrEmpty(searchName) || !string.IsNullOrEmpty(ownerId))
            {
                Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.AND);

                if (!string.IsNullOrEmpty(searchName)) //Search criteria for name
                    filter.AddCondition(new DataAccess.Common.Model.Condition("Name", AQLEnums.FilterOperator.Like, "%" + searchName + "%"));
                if (!string.IsNullOrEmpty(ownerId))
                    filter.AddCondition(new DataAccess.Common.Model.Condition("CreatedById", AQLEnums.FilterOperator.Equal, ownerId));

                searchAppQuery.Criteria = filter;
            }

            // TODO:: add join with xae_AppAssignment for Runtime users and Search criteria for OwnerId

            return searchAppQuery;
        }

        public ApttusDataSet GetAppDataSet(int? maxRecords, string searchName, string ownerId, ApttusUserInfo userInfo)
        {
            Query query = getAppQueryExpr(maxRecords, searchName, ownerId);

            ApttusDataSet appData = QueryDataSet(query, null, null, userInfo);
            return appData;
        }

        public List<ApttusObject> Query<T>(XAuthorQuery query)
        {
            AICQuery aicQuery = query as AICQuery;
            return Query<T>(aicQuery.Query);
        }

        private List<ApttusObject> Query<T>(Query query)
        {
            List<ApttusObject> lstApttusObjects = new List<ApttusObject>();

            string serializedQuery = JsonConvert.SerializeObject(query);
            var response = aicConnector.Query(query.EntityName, serializedQuery);

            SearchResult results = JsonConvert.DeserializeObject<SearchResult>(response);

            foreach (Dictionary<string, object> record in results.SerializedResultEntities)
            {
                ApttusObject obj = new ApttusObject();

                obj.Id = Convert.ToString(record[Constants.ID_ATTRIBUTE]);
                obj.Name = Convert.ToString(record[Constants.NAME_ATTRIBUTE]);

                lstApttusObjects.Add(obj);
            }
            return lstApttusObjects;
        }

        public ApttusDataSet QueryDataSet(XAuthorQuery query)
        {
            AICQuery aicQuery = query as AICQuery;
            //AICRefreshSession.GetInstance.RefreshConnection();
            return QueryDataSet(aicQuery.Query, aicQuery.Object, aicQuery.DataTable, aicQuery.UserInfo);
        }

        private void SetNullForQueryParams(ref Query query)
        {
            if (query.Criteria != null)
            {
                if (query.Criteria.Conditions != null && query.Criteria.Conditions.Count == 0 &&
                    query.Criteria.Filters != null && query.Criteria.Filters.Count == 0) query.Criteria = null;
            }
            if (query.Joins != null && query.Joins.Count == 0) query.Joins = null;
            else if (query.Joins != null && query.Joins.Count > 0)
            {
                foreach (var join in query.Joins)
                {
                    if (join.JoinCriteria != null)
                    {
                        var criteria = join.JoinCriteria;
                        if (criteria.Conditions == null && criteria.Filters == null) join.JoinCriteria = null;
                    }
                }
            }
            else { }
            if (query.PageInfo != null && query.PageInfo.Count == 0) query.PageInfo = null;
            if (query.SortOrders != null && query.SortOrders.Count == 0) query.SortOrders = null;
        }

        private SearchResult ExecuteQuery(Query query)
        {
            SearchResult results = new SearchResult();
            results.SerializedResultEntities = new List<Dictionary<string, object>>();

            //This function has been added to avoid status:500 Error from AIC beacuse AIC expects null fields if count is 0
            SetNullForQueryParams(ref query);

            // No Need to add page info since toprecords is defined
            if (query.TopRecords > 0)
            {
                string serializedQuery = JsonConvert.SerializeObject(query);
                string response = aicConnector.Query(query.EntityName, serializedQuery);
                results = JsonConvert.DeserializeObject<SearchResult>(response);
            }
            else
            {
                int pageNumber = 1;
                int maxRecordPerQuery = 5000;
                int totalRecordsForQuery = 0;
                int totalRecordsRetrievedSoFar = 0;
                do
                {
                    query.PageInfo = new PageInfo();
                    query.PageInfo.PageNumber = pageNumber++;
                    query.PageInfo.Count = maxRecordPerQuery;
                    query.PageInfo.IsReturnTotalRecordCount = true;

                    string serializedQuery = JsonConvert.SerializeObject(query);
                    string response = aicConnector.Query(query.EntityName, serializedQuery);
                    SearchResult chunkResults = JsonConvert.DeserializeObject<SearchResult>(response);
                    results.SerializedResultEntities.AddRange(chunkResults.SerializedResultEntities);

                    totalRecordsForQuery = chunkResults.TotalNumberOfRecords;
                    totalRecordsRetrievedSoFar = totalRecordsRetrievedSoFar + maxRecordPerQuery;

                    // free up resources
                    response = null;
                    chunkResults = null;
                }
                while (totalRecordsForQuery >= totalRecordsRetrievedSoFar);
            }

            return results;
        }

        private ApttusDataSet QueryDataSet(Query query, ApttusObject appObject, DataTable dataTable, ApttusUserInfo userInfo)
        {
            ApttusDataSet dataSet = new ApttusDataSet();

            SearchResult results = ExecuteQuery(query);
            // TODO:: implement attachments, once support is there
            //bool hasAttachmentColumn = false;

            dataSet.DataTable = new DataTable();

            if (results.SerializedResultEntities.Count == 0)
            {
                if (dataTable != null)
                    dataSet.DataTable = dataTable;
                return dataSet;
            }

            if (dataTable == null)
            {
                // 1. Add Datatable Columns based on 1st record
                foreach (var col in query.Columns)
                {
                    if (!string.IsNullOrEmpty(col))
                        dataSet.DataTable.Columns.Add(col.ToString());
                }

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


            foreach (Dictionary<string, object> rec in results.SerializedResultEntities)
            {
                DataRow row = dataSet.DataTable.NewRow();
                foreach (var attr in rec.Keys)
                {
                    string attrValue = string.Empty;
                    bool IsCompositeORLookupDataType = false;
                    ApttusField apttusField = null;
                    if (appObject != null)
                    {
                        apttusField = appObject.GetField(attr);
                        if (apttusField != null)
                            IsCompositeORLookupDataType = apttusField.Datatype == Datatype.Composite || apttusField.Datatype == Datatype.Lookup;
                    }
                    // this is for lookup fields
                    if (rec[attr] != null)
                    {
                        if (rec[attr].GetType() == typeof(JObject))
                        {
                            JObject obj = JObject.Parse(Convert.ToString(rec[attr]));

                            foreach (JProperty prop in obj.Properties())
                            {
                                //if (prop.Name.Equals(Constants.NAME_ATTRIBUTE) && Convert.ToString(attr).EndsWith(Constants.ID_ATTRIBUTE) && !HasAliasInAttribute(attr))
                                //{
                                //    // Lookup Name value
                                //    string lookupNameField = GetLookUpIdAndNameProvider().GetLookupNameFromLookupId(attr);
                                //    SetSFFieldValue(dataSet.DataTable.Columns[lookupNameField], row, lookupNameField, Convert.ToString(prop.Value), userInfo);
                                //}

                                if (prop.Name.Equals(Constants.ID_ATTRIBUTE))
                                {
                                    // Lookup Id value
                                    attrValue = Convert.ToString(prop.Value);
                                    SetSFFieldValue(dataSet.DataTable.Columns[attr], row, attr, attrValue, userInfo);
                                }
                                else if (prop.Name.Equals("Value"))
                                {
                                    // PickList Value value
                                    attrValue = Convert.ToString(prop.Value);
                                    SetSFFieldValue(dataSet.DataTable.Columns[attr], row, attr, attrValue, userInfo);
                                }
                                else if (prop.Value != null && !HasAliasInAttribute(attr) && !IsCompositeORLookupDataType && appObject != null)
                                {
                                    attrValue = Convert.ToString(prop.Value);
                                    SetSFFieldValue(dataSet.DataTable.Columns[attr], row, attr, attrValue, userInfo);
                                }
                                else
                                {
                                    //All relational fields (including look-up name) will be populated here
                                    SetRelationFields(attr, prop, dataSet, row, userInfo);
                                }
                            }
                        }
                        //Used for MultiSelect PickList
                        else if (rec[attr].GetType() == typeof(JArray))
                        {
                            JArray obj = JArray.Parse(Convert.ToString(rec[attr]));

                            attrValue = string.Join(";", obj.Select(j => j.ToString()));
                            SetSFFieldValue(dataSet.DataTable.Columns[attr], row, attr, attrValue, userInfo);
                        }
                        else
                        {
                            if (rec[attr].GetType().ToString().Equals("System.DateTime"))
                            {
                                if (apttusField != null)
                                {
                                    if (apttusField.Datatype == Datatype.Date) attrValue = DateTime.Parse(rec[attr].ToString()).ToShortDateString();
                                    if (apttusField.Datatype == Datatype.DateTime) attrValue = attrValue = Convert.ToString(rec[attr]);

                                }
                                else
                                    attrValue = DateTime.Parse(rec[attr].ToString()).ToUniversalTime().ToString("dd-MM-yyyy");//"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"
                            }
                            else
                                attrValue = Convert.ToString(rec[attr]);
                            SetSFFieldValue(dataSet.DataTable.Columns[attr], row, attr, attrValue, userInfo);
                        }
                    }
                }
                dataSet.DataTable.Rows.Add(row);
            }

            return dataSet;
        }
        private bool HasAliasInAttribute(string attr)
        {
            return Convert.ToString(attr).Contains(Constants.AIC_ALIAS_SEPARATOR);
        }
        private string GetRelationalFieldFromAliasAttribute(string recordAttribute, JProperty objectProperty)
        {
            return Convert.ToString(recordAttribute).Replace(Constants.AIC_ALIAS_SEPARATOR, Constants.DOT) + Constants.DOT + objectProperty.Name;
        }
        private void SetRelationFields(string recordAttribute, JProperty objectProperty, ApttusDataSet dataSet, DataRow rowToAdd, ApttusUserInfo userInfo)
        {
            DataColumn col = null;
            string relationalField = string.Empty;
            if (HasAliasInAttribute(recordAttribute))
            {
                relationalField = GetRelationalFieldFromAliasAttribute(recordAttribute, objectProperty);
            }
            else
            {
                relationalField = Convert.ToString(recordAttribute) + Constants.DOT + objectProperty.Name;
            }
            col = dataSet.DataTable.Columns[relationalField];
            if (col != null)
            {
                string fieldValue = string.Empty;
                if (objectProperty.Value.GetType().ToString().Equals("Newtonsoft.Json.Linq.JValue"))
                {
                    fieldValue = objectProperty.Value.ToString();
                }
                else if (objectProperty.Value.GetType().ToString().Equals("Newtonsoft.Json.Linq.JObject"))
                {
                    if (objectProperty.Name.EndsWith(Constants.ID_ATTRIBUTE))
                    {
                        fieldValue = objectProperty.Value[Constants.ID_ATTRIBUTE].ToString();
                    }
                    else if (objectProperty.Value["Value"] != null)
                    {
                        fieldValue = objectProperty.Value["Value"].ToString();
                    }
                    else
                    {
                        if (objectProperty.Value["Id"] != null)
                            fieldValue = objectProperty.Value["Id"].ToString();
                        else
                            fieldValue = string.Empty;
                    }
                }
                SetSFFieldValue(col, rowToAdd, relationalField, fieldValue, userInfo);
            }
        }
        private void SetSFFieldValue(DataColumn dataColumn, DataRow row, string fieldName, string fieldValue, ApttusUserInfo userInfo)
        {
            if (dataColumn == null) return;
            // empty / blank values
            if (string.IsNullOrEmpty(fieldValue) && (dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(DateTime)))
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
                    row[fieldName] = double.Parse(fieldValue, userCulture.NumberFormat).ToString();
                }
                else
                {
                    row[fieldName] = fieldValue;
                }
            }
            else
            {
                row[fieldName] = fieldValue;
            }
        }

        public void Insert(List<ApttusSaveRecord> InsertObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (InsertObjects != null && InsertObjects.Count > 0)
                {
                    List<AICRecord> entityToInsert = new List<AICRecord>();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (ApttusSaveRecord InsertObject in InsertObjects)
                    {
                        // Process attachment records if any
                        if (InsertObject.HasAttachment)
                        {
                            entityToInsert.AddRange(GetAttachments(InsertObject));
                        }
                        else if (!isRequiredFieldValueMissing(InsertObject))
                        {
                            // Initialize AIC record
                            AICRecord entity = new AICRecord(InsertObject.ObjectName);

                            GetEntityElements(entity, InsertObject.SaveFields, currentCultureInfo, enUSCulture);

                            entityToInsert.Add(entity);
                        }
                    }

                    List<SaveResult> saveResult = ChunkAndExecute<SaveResult>(entityToInsert, true, string.Empty, waitWindow, QueryTypes.INSERT, 0, entityToInsert.Count, enablePartialSave, BatchSize);
                    entityToInsert.Clear();
                    entityToInsert = null;

                    // Process save Results
                    ProcessResults(saveResult, InsertObjects);

                    // Clear Response - Memory Optimization
                    saveResult = null;
                    // TODO:: Uncomment, if needed
                    //GC.Collect();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private List<AICRecord> GetAttachments(ApttusSaveRecord SaveRecord)
        {
            List<AICRecord> Attachments = new List<AICRecord>();
            foreach (var SaveAttachment in SaveRecord.Attachments)
            {
                AICRecord Attachment = new AICRecord(SaveAttachment.ObjectId);
                Attachment.HasAttachment = true;
                Attachment.RecordId = SaveAttachment.ParentId;
                Attachment.FileName = SaveAttachment.AttachmentName;
                Attachment.FileStream = Convert.FromBase64String(SaveAttachment.Base64EncodedString);
                Attachments.Add(Attachment);
            }
            return Attachments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveResult"></param>
        /// <param name="apttusSaveRecords"></param>
        private void ProcessResults(List<SaveResult> saveResult, List<ApttusSaveRecord> apttusSaveRecords)
        {
            int cnt = 0;

            foreach (SaveResult result in saveResult)
            {
                ApttusSaveRecord SaveRecord = apttusSaveRecords[cnt++];

                SaveRecord.RecordId = result.id;
                SaveRecord.Success = result.success;
                SaveRecord.ErrorMessage = result.error;
                if (result.isUpsert)
                    SaveRecord.OperationType = QueryTypes.UPSERT;
            }
        }

        private bool isRequiredFieldValueMissing(ApttusSaveRecord saveRecord)
        {
            bool valueMissing = false;
            // First, Check SaveRecordFields for missing required fields
            if (saveRecord.SaveFields.Any(f => f.Required == true && string.IsNullOrEmpty(f.Value)))
            {
                List<string> rFieldMissing = saveRecord.SaveFields.Where(f => f.Required == true && string.IsNullOrEmpty(f.Value)).Select(f => f.FieldId).ToList();

                saveRecord.ErrorMessage = "Required fields : " + String.Join(Constants.COMMA, rFieldMissing) + " missing for Object : " + saveRecord.ObjectName;
                valueMissing = true;

            }
            return valueMissing;
        }

        private AICRecord GetEntityElements(AICRecord saveEntity, List<ApttusSaveField> saveFields, CultureInfo currentCultureInfo, CultureInfo targetCultureInfo)
        {
            try
            {
                // check if we need ValidateXmlElement(SFDC) like function here or not
                foreach (ApttusSaveField saveField in saveFields)
                {
                    // TODO:: more research required, whether to ignore ExternalID fields or not.
                    //if (saveField.ExternalId)
                    //    continue;

                    switch (saveField.DataType)
                    {
                        case Datatype.String:
                            //CRM does not save GUIDs as string so that it must be converted to Guid.
                            if (saveField.CRMDataType != null && (saveField.CRMDataType.Equals("Uniqueidentifier") || saveField.CRMDataType.Equals("Lookup")))
                            {
                                Guid prsGuid;
                                if (Guid.TryParse(saveField.Value, out prsGuid))
                                    saveEntity.Record[saveField.FieldId] = prsGuid;
                                else
                                    saveEntity.Record[saveField.FieldId] = null;
                            }
                            else
                                saveEntity.Record[saveField.FieldId] = saveField.Value;
                            break;
                        case Datatype.Decimal:
                            saveEntity.Record[saveField.FieldId] = Convert.ToDecimal(saveField.Value == string.Empty ? "0" : saveField.Value);
                            break;
                        case Datatype.Double:
                            saveEntity.Record[saveField.FieldId] = Convert.ToDouble(saveField.Value == string.Empty ? "0" : saveField.Value);
                            break;
                        case Datatype.Date:
                            DateTime dateToSave = Convert.ToDateTime(saveField.Value).Date;
                            //Specigfy kind function is called beacuse it shouldnt be local specific.If it is not specified, it will save wrong dates
                            dateToSave = DateTime.SpecifyKind(dateToSave, DateTimeKind.Unspecified);
                            saveEntity.Record[saveField.FieldId] = dateToSave;
                            break;
                        case Datatype.DateTime:
                            DateTime dateTimeToSave = Convert.ToDateTime(saveField.Value);
                            //Specigfy kind function is called beacuse it shouldnt be local specific.If it is not specified, it will save wrong dates
                            dateTimeToSave = DateTime.SpecifyKind(dateTimeToSave, DateTimeKind.Unspecified);
                            saveEntity.Record[saveField.FieldId] = dateTimeToSave;
                            break;
                        case Datatype.Boolean:
                            saveEntity.Record[saveField.FieldId] = Convert.ToBoolean(saveField.Value);
                            break;
                        case Datatype.Picklist:
                            if (!string.IsNullOrEmpty(saveField.Value))
                                saveEntity.Record[saveField.FieldId] = saveField.Value;
                            else
                                saveEntity.Record[saveField.FieldId] = null;
                            break;
                        case Datatype.Lookup:
                            Guid lookupGuid;
                            if (Guid.TryParse(saveField.Value, out lookupGuid))
                                saveEntity.Record[saveField.FieldId] = lookupGuid;
                            else
                                saveEntity.Record[saveField.FieldId] = saveField.Value;
                            break;
                        case Datatype.Composite:
                            dynamic compsiteField = new JObject();
                            compsiteField.Id = saveField.Value;
                            compsiteField.Type = saveField.LookupObjectId;
                            saveEntity.Record[saveField.FieldId] = compsiteField;
                            break;
                        default:
                            saveEntity.Record[saveField.FieldId] = saveField.Value;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return saveEntity;
        }

        private List<T> ChunkAndExecute<T>(List<AICRecord> sObjects, bool Insert, string ExternalId, WaitWindowView waitWindow, QueryTypes queryType, int current, int total, bool enablePartialSave, int BatchSize)
        {
            List<T> chunkResult = new List<T>();
            List<T> allResults = new List<T>();
            int MaxTransactionRecords = BatchSize;
            // In AIC we have to send records of 1 object at a time, so set MaxTransactionUniqueObjects to 1
            int MaxTransactionUniqueObjects = 1;

            try
            {
                if (sObjects.Count <= MaxTransactionRecords && sObjects.Select(s => s.ObjectId).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    SetWaitMessage(waitWindow, queryType, current + sObjects.Count, total);
                    allResults = ExecuteChunk<T>(sObjects, Insert, ExternalId, enablePartialSave, queryType);
                }
                else if (sObjects.Count > MaxTransactionRecords && sObjects.Select(s => s.ObjectId).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    List<AICRecord> MaxRecordsChunk = sObjects.GetRange(0, MaxTransactionRecords);

                    // Part 1 - Chunk of Max Records
                    SetWaitMessage(waitWindow, queryType, current + MaxRecordsChunk.Count, total);
                    chunkResult = ExecuteChunk<T>(MaxRecordsChunk, Insert, ExternalId, enablePartialSave, queryType);
                    allResults.AddRange(chunkResult);

                    // Part 2 - Chunk of Remaining Records - Recursion
                    List<AICRecord> RemainingRecords = sObjects.GetRange(MaxTransactionRecords, sObjects.Count - MaxTransactionRecords);
                    chunkResult = ChunkAndExecute<T>(RemainingRecords, Insert, ExternalId, waitWindow, queryType, current + MaxRecordsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);
                }
                else if (sObjects.Select(s => s.ObjectId).Distinct().Count() > MaxTransactionUniqueObjects)
                {
                    List<string> AllUniqueObjects = (from s in sObjects select s.ObjectId).Distinct().ToList();
                    List<string> MaxUniqueObjects = new List<string>();
                    for (int i = 0; i < MaxTransactionUniqueObjects; i++)
                        MaxUniqueObjects.Add(AllUniqueObjects[i]);

                    // Part 1 - Chunk of Max Unique Objects - Recursion
                    List<AICRecord> MaxUniqueObjectsChunk = sObjects.Where(s => MaxUniqueObjects.Contains(s.ObjectId)).ToList();
                    chunkResult = ChunkAndExecute<T>(MaxUniqueObjectsChunk, Insert, ExternalId, waitWindow, queryType, current + MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);

                    // Part 2 - Chunk of Remaining Unique Objects - Recursion
                    chunkResult = ChunkAndExecute<T>(sObjects.Where(s => !MaxUniqueObjects.Contains(s.ObjectId)).ToList(), Insert, ExternalId, waitWindow, queryType, current + +MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults.AddRange(chunkResult);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return allResults;
        }

        private void SetWaitMessage(WaitWindowView waitWindow, QueryTypes queryType, int current, int total)
        {
            if (waitWindow != null)
                waitWindow.Message = "Performing \"" + queryType.ToString() + "\" operation on AIC. Processing " + current.ToString() + " of " + total.ToString() + "...";
        }

        private List<T> ExecuteChunk<T>(List<AICRecord> Chunk, bool Insert, string ExternalId, bool enablePartialSave, QueryTypes queryType)
        {
            List<T> chunkResult = new List<T>();
            List<SaveResult> saveResult;
            try
            {
                switch (queryType)
                {
                    case QueryTypes.INSERT:
                        // Seggregate Attachment Records and Insert records
                        List<AICRecord> insertChunk = Chunk.Where(c => !c.HasAttachment).ToList();
                        List<AICRecord> insertAttachmentChunk = Chunk.Where(c => c.HasAttachment).ToList();

                        if (insertChunk != null && insertChunk.Count > 0)
                        {
                            saveResult = aicConnector.Insert(insertChunk[0].ObjectId, insertChunk.Select(c => c.Record).ToList(), enablePartialSave);
                            chunkResult = saveResult.Cast<T>().ToList();
                        }

                        List<SaveResult> attachResult = ExecuteAttachmentChunk(insertAttachmentChunk);
                        chunkResult.AddRange(attachResult.Cast<T>().ToList());

                        break;
                    case QueryTypes.UPDATE:
                        saveResult = aicConnector.Update(Chunk[0].ObjectId, Chunk.Select(c => c.Record).ToList(), enablePartialSave);
                        chunkResult = saveResult.Cast<T>().ToList();
                        break;
                    case QueryTypes.UPSERT:
                        string response = aicConnector.Upsert(Chunk[0].ObjectId, Chunk.Select(c => c.Record).ToList(), ExternalId, enablePartialSave);
                        List<SaveResult> upsertResult = ProcessUpsertResult(response, Chunk, ExternalId);
                        chunkResult = upsertResult.Cast<T>().ToList();
                        break;
                    case QueryTypes.DELETE:
                        List<SaveResult> deleteResult = aicConnector.Delete(Chunk[0].ObjectId, Chunk.Select(c => c.Record[Constants.ID_ATTRIBUTE].ToString()).ToArray(), enablePartialSave);
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

        private List<SaveResult> ProcessUpsertResult(string response, List<AICRecord> upsertRecords, string externalId)
        {
            List<SaveResult> upsertResults = new List<SaveResult>();
            BulkOperationResult bulkUpsertResults = JsonConvert.DeserializeObject<BulkOperationResult>(response);

            foreach (AICRecord rec in upsertRecords)
            {
                SaveResult currentResult = new SaveResult();

                string aicRecordExternalId = Convert.ToString(rec.Record[externalId]);

                // 1. First find in InsertIds, means Record got inserted
                if (bulkUpsertResults.Inserted != null && bulkUpsertResults.Inserted.Count > 0)
                {
                    JObject currentReturnResultInsert = new JObject();
                    foreach (var insertId in bulkUpsertResults.Inserted)
                    {
                        JObject insertObj = insertId;
                        string resultRecordExtIdValue = insertObj.Value<string>(externalId);

                        if (aicRecordExternalId.Equals(resultRecordExtIdValue))
                        {
                            currentReturnResultInsert = insertObj;
                            break;
                        }
                        // TODO:: Add when ExternalId not ExternalID column but something else
                    }
                    if (currentReturnResultInsert.HasValues)
                    {
                        currentResult.id = currentReturnResultInsert.Value<string>(Constants.ID_ATTRIBUTE);
                        currentResult.success = true;
                        currentResult.error = string.Empty;
                        currentResult.isUpsert = false;
                        upsertResults.Add(currentResult);
                    }
                }
                // 2. Second find in UpdateIds, means Record got updated
                if (bulkUpsertResults.Updated != null && bulkUpsertResults.Updated.Count > 0)
                {
                    JObject currentReturnResultUpdate = new JObject();
                    foreach (var updateId in bulkUpsertResults.Updated)
                    {
                        JObject updateObj = updateId;
                        string resultRecordExtIdValue = updateObj.Value<string>(externalId);

                        if (aicRecordExternalId.Equals(resultRecordExtIdValue))
                        {
                            currentReturnResultUpdate = updateObj;
                            break;
                        }
                        // TODO:: Add when ExternalId not ExternalID column but something else
                    }
                    if (currentReturnResultUpdate.HasValues)
                    {
                        currentResult.id = currentReturnResultUpdate.Value<string>(Constants.ID_ATTRIBUTE);
                        currentResult.success = true;
                        currentResult.error = string.Empty;
                        currentResult.isUpsert = true;
                        upsertResults.Add(currentResult);
                    }
                }
                // 3. Thirdly find in Errors, means Record Errored out
                if (bulkUpsertResults.Errors.Count > 0)
                {
                    foreach (var errorInfo in bulkUpsertResults.Errors)
                    {
                        if (!string.IsNullOrEmpty(errorInfo.Message))
                        {
                            Dictionary<string, object> currentRecord = (Dictionary<string, object>)errorInfo.Record;
                            if (aicRecordExternalId.Equals(Convert.ToString(currentRecord[externalId])))
                            {
                                currentResult.id = currentRecord.ContainsKey(Constants.ID_ATTRIBUTE) ? Convert.ToString(currentRecord[Constants.ID_ATTRIBUTE]) : string.Empty;
                                currentResult.success = false;
                                currentResult.error = errorInfo.Message;
                                currentResult.isUpsert = false;
                                upsertResults.Add(currentResult);
                                break;
                            }
                        }
                    }
                }
            }
            return upsertResults;
        }

        private List<SaveResult> ExecuteAttachmentChunk(List<AICRecord> insertAttachmentChunk)
        {
            List<SaveResult> attachResults = new List<SaveResult>();
            if (insertAttachmentChunk != null && insertAttachmentChunk.Count > 0)
            {
                List<string> AllUniqueRecordIds = (from s in insertAttachmentChunk select s.RecordId).Distinct().ToList();

                foreach (string currentRecId in AllUniqueRecordIds)
                {
                    List<AICRecord> currentRecordAttachments = insertAttachmentChunk.Where(c => c.RecordId == currentRecId).ToList();
                    List<SaveResult> currentAttachmentResults = aicConnector.InsertAttachment(currentRecordAttachments[0].ObjectId, currentRecordAttachments[0].RecordId, currentRecordAttachments);
                    attachResults.AddRange(currentAttachmentResults);
                }
            }

            return attachResults;
        }

        public void Update(List<ApttusSaveRecord> UpdateObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (UpdateObjects != null && UpdateObjects.Count > 0)
                {
                    List<AICRecord> entityToUpdate = new List<AICRecord>();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (ApttusSaveRecord UpdateObject in UpdateObjects.Where(s => s.Ignore == false))
                    {
                        if (UpdateObject.HasAttachment)
                        {
                            entityToUpdate.AddRange(GetAttachments(UpdateObject));
                        }
                        else if (!isRequiredFieldValueMissing(UpdateObject))
                        {
                            // Initialize AIC record
                            AICRecord entity = new AICRecord(UpdateObject.ObjectName);
                            GetEntityElements(entity, UpdateObject.SaveFields, currentCultureInfo, enUSCulture);
                            // for update add Record Id to AICRecord
                            entity.Record.Add(Constants.ID_ATTRIBUTE, UpdateObject.RecordId);

                            entityToUpdate.Add(entity);
                        }
                    }

                    List<SaveResult> saveResult = ChunkAndExecute<SaveResult>(entityToUpdate, false, string.Empty, waitWindow, QueryTypes.UPDATE, 0, entityToUpdate.Count, enablePartialSave, BatchSize);
                    entityToUpdate.Clear();
                    entityToUpdate = null;

                    // Process save Results
                    ProcessResults(saveResult, UpdateObjects);

                    // Clear Response - Memory Optimization
                    saveResult = null;
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
                    string ExternalId = string.Empty;
                    List<AICRecord> entityToUpdate = new List<AICRecord>();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    // Take 0 record as in AIC only one unique object records are processed at a time.
                    ExternalId = UpsertObjects[0].SaveFields.FirstOrDefault(sf => sf.ExternalId).FieldId;

                    foreach (ApttusSaveRecord UpsertObject in UpsertObjects.Where(s => s.Ignore == false))
                    {
                        if (UpsertObject.HasAttachment)
                        {
                            entityToUpdate.AddRange(GetAttachments(UpsertObject));
                        }
                        else if (!isRequiredFieldValueMissing(UpsertObject))
                        {
                            // Initialize AIC record
                            AICRecord entity = new AICRecord(UpsertObject.ObjectName);

                            GetEntityElements(entity, UpsertObject.SaveFields, currentCultureInfo, enUSCulture);

                            entityToUpdate.Add(entity);
                        }
                    }

                    List<SaveResult> saveResult = ChunkAndExecute<SaveResult>(entityToUpdate, false, ExternalId, waitWindow, QueryTypes.UPSERT, 0, entityToUpdate.Count, enablePartialSave, BatchSize);
                    entityToUpdate.Clear();
                    entityToUpdate = null;

                    // Process save Results
                    ProcessResults(saveResult, UpsertObjects);

                    // Clear Response - Memory Optimization
                    saveResult = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void Delete(List<ApttusSaveRecord> DeleteObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (DeleteObjects != null && DeleteObjects.Count > 0)
                {
                    List<AICRecord> entityToDelete = new List<AICRecord>();
                    foreach (ApttusSaveRecord DeleteObject in DeleteObjects)
                    {
                        AICRecord toDelete = new AICRecord(DeleteObject.ObjectName);
                        toDelete.Record = new Dictionary<string, object>() { { Constants.ID_ATTRIBUTE, DeleteObject.RecordId } };
                        entityToDelete.Add(toDelete);
                    }

                    List<SaveResult> deleteResults = ChunkAndExecute<SaveResult>(entityToDelete, false, string.Empty, waitWindow, QueryTypes.DELETE, 0, entityToDelete.Count, enablePartialSave, BatchSize);
                    ProcessResults(deleteResults, DeleteObjects);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        public object[] DataSetToObject(ApttusDataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public string getNamespacePrefix()
        {
            return string.Empty;
        }

        public ApttusDataSet Search(string soql, ApttusUserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public string EscapeQueryString(string queryString)
        {
            string result = queryString;
            return result;
        }

        public string UnescapeQueryString(string queryString)
        {
            string result = queryString;
            return result;
        }

        public string GetExceptionCode(Exception ex)
        {
            return ex.GetType().ToString();
        }

        public string GetPartnerAPIVersion()
        {
            return string.Empty;
        }

        public bool IsAdminUser()
        {
            return true;
        }

        public string GetLicenseDetail()
        {
            // This method is psudo implementation and it has to be replaced when Licensing is implemented for AIC
            string licenseDetails = "<XAuthor>ZmW4pWXnKmrQwbDwUuMq8O0DUdpw/WJpTH5Zx15iPSMgVZuy9D4gWJs80OvniF+68qmGtQtcoE2nZaWRGnO+xKRxl0Pg1p7wGrc0v/zvFoA4kXkhST3hRmk544ZTzgEClnnpaeMgLS48BDmoWlUPo9nYM6ZGqFW5VVDMFp5Ad+27suLbDyrhXbheI9Vwccpy6wZ6pAdbWeDc7TVAqV5y0m+Hx7MTucF2a/Txr5SDuUYpIiJ6ga82M9NYsGsk530+IEoTkBq0Xhh+oOiTGg1k3ZJB6V/lVr6rBV90BAB9VjglprF+FoMvFV5MxZbbsd9kuwyAGZButMw/npJKLzi6mNCrFFIqEiWN5Czj+yDoybPC+qWf3B+nVmTWH4JTKxl2Jo1KmRh/ZCylMu8rCzxMumJHA0dnack0zZGuYOaUsiolBdeP95/knjxM8GbQPTnKWcwOZaXFM22eVwbPau5gFTTN95sNk3rmo6WbrW0cQWe8zDfFIHDTK/syKBjdnH0Pl/RfKr2mvQZSo0LymjhvlcbHgEM4TsNOVHucicIR3Ie7lpxRYzFfT9IpUD45SfgiAQ1GqqjUsln0OE83TMWSHQmanmlmdXyhBirgKqgMTzry9Oodqow/69sKIU0qnkJAtZdht++SLZqHFqunJZiEkP4dy9Vi8Q6mcO4kc8YjGs+iMEeRjgKAJokUYfm9qC6RR5kzfFwnKwOrFNDT1Lm6MqU7/Wj26wNQTXmJokqqTzvTjvbi/+2dxnzENGlZgZWYRz3FZR+hDr1v2Zmei1edlGV7q3c4g67tBwLsOy0a9HfwtffR5hQ0fPQCTnM5HAmUgE4aFZEFaBKUFThXsgERXiT16rc/u6Nf3oS4d0MIggtMOb/ND5WWbp2BUeOElqee3Js+WQKiG2KjV3Gj3yNpPhmkvh6KL3q/1t4SR5Y/c/OQJ3k5C3F6rn5Mrme3fXL0Nld29A6WmgjCvQx1RoMcFVCvy+Zp0zsy5MNS8padauBah4D3xgIUzlIsYPU2vQr9lT3jnr11M28XgJNDcnR6Ih6TKzXJOK9kVzAW+wzYUM1mNep5w6sJeuzDUN4F5f3PZcnUT86oJXexD5ZBmTNOf/7xEERWksZkTQHjqvw9WKXrzwtKd8l5+difTR+4OZ2/dhBw1kSyhMXFeePfQ2l7wdEzM47bn7ry6w2UC9y9v+8FMKWtA5T8d72+YDcyYYJErWTjLUXXWtDrkrAMdpUR7uaAgQXJU7/lbKD4Xl5/YjzdhXcrXGnsAHdG5BwT7mj2d5Bic/7ewk/ygrLMKj2Nr5Wef7xDBRXJ3Znw4vYszyQWR3m+0Fm61WybK/4tENrW7Datg/AQO8BvF+8R9DFzE8BfiW7WiqIsEDebOtZkGk/PsRC/4lYcfh9EzGLNb8Moxvf7aysANzH7lh1wb1rYda/NXfFcbO58WekJFzNv0GZEvoZurfoPZZOQ5kqg3p5Mmvm6chVuJt81Xe0NB5SFd3bd19vrMMP8Ofo1DsoTsWMGWdhISCJ5/kNzBfsx3FJu+EkQV7nl68ZBZf+BYdpgSwctdjELUrSM43os5mnLYsH3BTLEEgE5Yv3XI+Drjjdxr+3jpnfdnpfqzSWpI+toZv1xemVlLb8jjncqhdh0tuIMq6eDMGsax8gxEB9vi2x7DHvNMGGuuDr0dmu/Zrq4zY+lXsEICGlckdkBdu4CSgE=</XAuthor>";
            string decryptedLicenseDetails = EncryptionHelper.Decrypt(licenseDetails);
            return decryptedLicenseDetails;
        }

        public string GetFeatureDetail()
        {
            // This method is psudo implementation and it has to be replaced when Licensing is implemented for AIC
            string featureDetails = "<XAuthor>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</XAuthor>";
            string decryptedFeatureDetails = EncryptionHelper.Decrypt(featureDetails);
            return decryptedFeatureDetails;
        }

        public bool IsSandBox()
        {
            return false;
        }

        public bool IsAdminPackageEditionUser()
        {
            return true;
        }

        public string GetLicenseFilePath()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus\\");

            DirectoryInfo di = new DirectoryInfo(folderName);
            if (!di.Exists)
                di.Create();

            return folderName + "AIC_";
        }

        public bool HasCollisionDetection()
        {
            return false;
        }

        public ILookupIdAndLookupNameProvider GetLookUpIdAndNameProvider()
        {
            if (LookupIdAndLookupNameProvider == null)
                LookupIdAndLookupNameProvider = new AICLookupIdAndNameProvider();

            return LookupIdAndLookupNameProvider;
        }

        public void SetAppDataTableProperties(ref DataTable table)
        {
            if (table.Columns.Contains("CreatedById.Name"))
                table.Columns["CreatedById.Name"].ColumnName = "Owner.Name";

            if (table.Columns.Contains("ModifiedOn"))
                table.Columns["ModifiedOn"].ColumnName = "LastModifiedDate";
        }

        public ApttusDataSet GetAttachmentsDataSet(string ParentIds, string ObjectId)
        {
            string[] splitObjIds = ParentIds.Split(',');

            var QueryExpression = AICExpressionBuilderHelper.GetAnnotationsForObjectId(splitObjIds.ToList(), ObjectId);
            ApttusDataSet tempDataSet = QueryDataSet(new AICQuery() { Query = QueryExpression });

            //// Transform Data table Column names
            //if (tempDataSet.DataTable != null && tempDataSet.DataTable.Columns.Count == 5)
            //{
            //    tempDataSet.DataTable.Columns[0].ColumnName = Constants.NAME_ATTRIBUTE;
            //    tempDataSet.DataTable.Columns[1].ColumnName = "Body";
            //    tempDataSet.DataTable.Columns[2].ColumnName = "ParentId";
            //    tempDataSet.DataTable.Columns[3].ColumnName = Constants.ID_ATTRIBUTE;
            //    tempDataSet.DataTable.Columns[4].ColumnName = "Parent.Name";
            //}

            return null;
        }

        public ApttusDataSet GetLookUpDetails(ApttusObject apttusObject, List<string> LookupNames)
        {
            Query queryExpr = new Query(apttusObject.Id, false);
            queryExpr.Columns = new List<string> { Constants.ID_ATTRIBUTE, Constants.NAME_ATTRIBUTE };

            Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.AND);
            filter.AddCondition(apttusObject.NameAttribute, DataAccess.Common.Enums.FilterOperator.In, LookupNames);
            queryExpr.Criteria = filter;

            return QueryDataSet(new AICQuery() { Query = queryExpr });
        }

        public ApttusDataSet GetDataSetByLookupSearch(ApttusObject lookupObj, string search)
        {
            ApttusDataSet ds = new ApttusDataSet();
            Query query = null;
            string nameField = string.Empty;
            string idField = string.Empty;

            List<string> fieldSet = new List<string>();
            if (!string.IsNullOrEmpty(lookupObj.IdAttribute))
            {
                nameField = lookupObj.NameAttribute;
                idField = lookupObj.IdAttribute;
                fieldSet.Add(nameField);
                fieldSet.Add(idField);
                /* Change Grid Static Column names*/
            }
            query = new Query(lookupObj.Id);
            query.Columns = fieldSet;
            if (!string.IsNullOrEmpty(search))
            {
                Expression filter = new Expression(DataAccess.Common.Enums.ExpressionOperator.AND);
                // TODO:: check if search + Constants.PERCENT is required
                filter.AddCondition(nameField, DataAccess.Common.Enums.FilterOperator.Like, search);
                query.Criteria = filter;
            }

            ds = QueryDataSet(new AICQuery() { Query = query });
            return ds;

        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            return aicConnector.GetObjectNameByPrimaryId(primaryID, objectNames);
        }

        public bool saveApplication(string recordId, byte[] config, byte[] template, string templateName, byte[] schema)
        {
            bool result = false;

            List<AICRecord> appAttachments = new List<AICRecord>();
            appAttachments.Add(new AICRecord(APP_OBJECT) { HasAttachment = true, FileName = templateName, FileStream = template, RecordId = recordId });
            appAttachments.Add(new AICRecord(APP_OBJECT) { HasAttachment = true, FileName = "AppDefinition.xml", FileStream = config, RecordId = recordId });

            aicConnector.InsertAttachment(APP_OBJECT, recordId, appAttachments);

            // Update App so that it appears on top after Save
            List<Dictionary<string, object>> updateApp = new List<Dictionary<string, object>>();
            updateApp.Add(new Dictionary<string, object>() { { "ExternalLastUpdatedOn", DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) }, { Constants.ID_ATTRIBUTE, recordId } });
            aicConnector.Update(APP_OBJECT, updateApp, false);

            result = true;
            return result;
        }

        public ApplicationObject LoadApplication(string recordId)
        {
            return LoadApplication(recordId, Guid.Empty);
        }

        public ApplicationObject LoadApplication(Guid uniqueId)
        {
            return LoadApplication(null, uniqueId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public ApplicationObject LoadApplication(string recordId, Guid uniqueId)
        {
            ApplicationObject App = null;

            string templateId, configId;
            string templateName = aicConnector.GetAppAttachmentIds(recordId, out templateId, out configId);

            App = new ApplicationObject();

            App.Config = aicConnector.GetAttachment(configId);
            App.Template = aicConnector.GetAttachment(templateId);
            App.TemplateName = templateName;

            return App;
        }

        public void DeleteAppAttachment(string recordId)
        {
            aicConnector.DeleteAttachment(APP_OBJECT, recordId);
        }

        /// <summary>
        /// This method is used in NUnit test cases to test AIC APIs
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public byte[] GetAttachment(string recordId)
        {
            return aicConnector.GetAttachment(recordId);
        }

        /// <summary>
        /// This method is used in NUnit test cases to test AIC APIs
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="recordId"></param>
        public void DeleteAttachment(string objectid, string recordId)
        {
            aicConnector.DeleteAttachment(objectid, recordId);
        }

        public void RefreshConnection()
        {
            AICRefreshSession.GetInstance.RefreshConnection();
        }

        public AppAssignmentModel GetAppAssignmentDetails(string AppId)
        {
            AppAssignmentModel Model = new AppAssignmentModel
            {
                ApplicationId = AppId
            };
            List<string> Columns = new List<string> { "Id", "Name", "ApplicationId", "Profile", "Assignment##User.Name", "Assignment##User.Email" };
            Query AppAssignmentQuery = new Query("xae_AppAssignment", false)
            {
                Columns = Columns,
                TopRecords = 500,
            };
            Join UserJoin = new Join("xae_AppAssignment", "crm_User", "UserId", "Id", AQLEnums.JoinType.RIGHT, "Assignment##User");
            AppAssignmentQuery.Joins = new List<Join>
            {
                UserJoin
            };
            if (!string.IsNullOrEmpty(AppId))
            {
                Expression filter = new Expression(AQLEnums.ExpressionOperator.AND);
                filter.AddCondition(new DataAccess.Common.Model.Condition("ApplicationId", AQLEnums.FilterOperator.Equal, Guid.Parse(AppId)));
                AppAssignmentQuery.Criteria = filter;
            }
            DataTable dataTable = new DataTable();
            foreach (string col in Columns)
            {
                dataTable.Columns.Add(new DataColumn(col.Replace("##", ".")));
            }
            dataTable.Columns.Add(new DataColumn("Assignment##User"));
            ApttusDataSet apttusDataSet = QueryDataSet(new AICQuery() { Query = AppAssignmentQuery, UserInfo = UserInfo, DataTable = dataTable });

            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                Model.Assignments.Add(new Assignments()
                {
                    AssignmentId = row["Id"].ToString(),
                    AssignmentName = row["Name"].ToString(),
                    User = (apttusDataSet.DataTable.Columns.Contains("Assignment##User") && !string.IsNullOrEmpty(row["Assignment##User"].ToString())) ? new User()
                    {
                        Id = row["Assignment##User"].ToString(),
                        Name = row["Assignment.User.Name"].ToString(),
                        Email = row["Assignment.User.Email"].ToString(),
                        Checked = true
                    } : null,
                    Profile = !string.IsNullOrEmpty(row["Profile"].ToString()) ? new Profile()
                    {
                        Id = row["Profile"].ToString(),
                        Name = null,
                        Checked = true
                    } : null,
                });
            }
            return Model;
        }

        public List<User> GetUsersList(string searchStr, string ExceptIds)
        {
            List<string> Columns = new List<string> { "Id", "FirstName", "LastName", "Email" };
            Query UserQuery = new Query("crm_User", false)
            {
                Columns = Columns,
                TopRecords = 500,
            };
            if (UserQuery.Criteria == null) UserQuery.Criteria = new Expression(AQLEnums.ExpressionOperator.AND);
            if (UserQuery.Criteria.Conditions == null) UserQuery.Criteria.Conditions = new List<DataAccess.Common.Model.Condition>();
            if (UserQuery.Criteria.Filters == null) UserQuery.Criteria.Filters = new List<Expression>();

            if (!string.IsNullOrEmpty(searchStr))
            {
                Expression filter = new Expression(AQLEnums.ExpressionOperator.OR);
                filter.AddCondition(new DataAccess.Common.Model.Condition("FirstName", AQLEnums.FilterOperator.Like, string.Format("%{0}%", searchStr)));
                filter.AddCondition(new DataAccess.Common.Model.Condition("LastName", AQLEnums.FilterOperator.Like, string.Format("%{0}%", searchStr)));
                filter.AddCondition(new DataAccess.Common.Model.Condition("Email", AQLEnums.FilterOperator.Like, string.Format("%{0}%", searchStr)));
                UserQuery.Criteria.Filters.Add(filter);
            }
            if (!string.IsNullOrEmpty(ExceptIds))
            {
                string[] IDs = ExceptIds.Replace('\'', ' ').Split(',');
                foreach (var id in IDs)
                {
                    if (Guid.TryParse(id.Trim(), out Guid userId))
                        UserQuery.Criteria.Conditions.Add(new DataAccess.Common.Model.Condition("Id", AQLEnums.FilterOperator.NotEqual, userId));
                }
            }
            ApttusDataSet apttusDataSet = QueryDataSet(new AICQuery() { Query = UserQuery, UserInfo = UserInfo });
            List<User> users = new List<User>();
            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                users.Add(new User
                {
                    Id = row["Id"].ToString(),
                    Name = string.Format("{0} {1}", row["FirstName"].ToString(), row["LastName"].ToString()),
                    Email = row["Email"].ToString(),
                    Checked = false
                });
            }
            return users;
        }

        public List<Profile> GetProfileList(string searchStr, string ExceptIds)
        {
            List<string> Columns = new List<string> { "Id", "Name" };
            Query RoleQuery = new Query("cmn_Role", false)
            {
                Columns = Columns,
                TopRecords = 500,
            };
            if (RoleQuery.Criteria == null) RoleQuery.Criteria = new Expression(AQLEnums.ExpressionOperator.AND);
            if (RoleQuery.Criteria.Conditions == null) RoleQuery.Criteria.Conditions = new List<DataAccess.Common.Model.Condition>();
            if (RoleQuery.Criteria.Filters == null) RoleQuery.Criteria.Filters = new List<Expression>();

            if (!string.IsNullOrEmpty(searchStr))
            {
                Expression filter = new Expression(AQLEnums.ExpressionOperator.OR);
                filter.AddCondition(new DataAccess.Common.Model.Condition("Name", AQLEnums.FilterOperator.Like, string.Format("%{0}%", searchStr)));
                RoleQuery.Criteria.Filters.Add(filter);
            }
            if (!string.IsNullOrEmpty(ExceptIds))
            {
                string[] IDs = ExceptIds.Replace('\'', ' ').Split(',');
                foreach (var id in IDs)
                {
                    if (Guid.TryParse(id.Trim(), out Guid userId))
                        RoleQuery.Criteria.Conditions.Add(new DataAccess.Common.Model.Condition("Id", AQLEnums.FilterOperator.NotEqual, userId));
                }
            }
            ApttusDataSet apttusDataSet = QueryDataSet(new AICQuery() { Query = RoleQuery, UserInfo = UserInfo });
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

        public ApttusUserInfo UserInfo { get { return apttusUserInfo; } }
    }
}
