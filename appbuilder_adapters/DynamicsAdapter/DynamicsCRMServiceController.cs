using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Sdk.Messages;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using System.ServiceModel.Description;

namespace Apttus.XAuthor.DynamicsAdapter
{
    public sealed class DynamicsCRMServiceController
    {
        // Static variables
        private static DynamicsCRMServiceController instance;
        private static object syncRoot = new Object();

        public IOrganizationService orgServiceProxy;
        public string DynamicsCRMVersion = string.Empty;
        public const int QUERY_IN_COUNT_LIMIT = 2000;

        public static DynamicsCRMServiceController GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                        instance = new DynamicsCRMServiceController();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ConnectWithDynamicsCRM(IOrganizationService oServiceProxy)
        {
            //// Obtain the target organization's Web address and client logon 
            //// credentials from the user.
            //ServerConnection serverConnect = new ServerConnection();
            //// TODO:: this function currently uses C:\Users\gsoni\AppData\Roaming\CrmServer\Credentials.xml to read server config and login.
            ////        Create a generic Plug & Play control like OAuthLogin for Dynamics and do necessary changes in this function.
            //ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration();

            //// The using statement assures that the service proxy will be properly disposed.
            //orgServiceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials);

            this.orgServiceProxy = oServiceProxy;
            //this.orgServiceProxy.Timeout = new TimeSpan(0, 5, 0);
            // This statement is required to enable early-bound type support.
            //this.orgServiceProxy.EnableProxyTypes();


            // Get Dynamics CRM Version No
            RetrieveVersionRequest requestVerson = new RetrieveVersionRequest();
            RetrieveVersionResponse responseVersion = (RetrieveVersionResponse)orgServiceProxy.Execute(requestVerson);
            //assigns the version to a string
            this.DynamicsCRMVersion = responseVersion.Version;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="endpointUrl"></param>
        /// <returns></returns>
        public bool ConnectWithDynamicsCRM(string token, string endpointUrl)
        {
            OrganizationWebProxyClient webServiceProxy = new OrganizationWebProxyClient(new Uri(endpointUrl), false);
            webServiceProxy.HeaderToken = token;

            this.orgServiceProxy = webServiceProxy;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IOrganizationService ConnectWithDynamicsCRMSliently(string userName, string password, string instanceURL)
        {
            //// Obtain the target organization's Web address and client logon 
            //// credentials from the user.
            //ServerConnection serverConnect = new ServerConnection();
            //// TODO:: this function currently uses C:\Users\gsoni\AppData\Roaming\CrmServer\Credentials.xml to read server config and login.
            ////        Create a generic Plug & Play control like OAuthLogin for Dynamics and do necessary changes in this function.
            //ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration();

            //// The using statement assures that the service proxy will be properly disposed.
            //orgServiceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials);

            Uri serviceUri = new Uri(instanceURL);
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = userName;
            credentials.UserName.Password = password;
            OrganizationServiceProxy oServiceProxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);

            this.orgServiceProxy = oServiceProxy;
            //this.orgServiceProxy.Timeout = new TimeSpan(0, 2, 0);
            // This statement is required to enable early-bound type support.
            //this.orgServiceProxy.EnableProxyTypes();
            return this.orgServiceProxy;
        }

        /// <summary>
        /// Retrieves all Entities 
        /// </summary>
        /// <returns></returns>
        //public RetrieveAllEntitiesResponse RetrieveAllEntities()
        //{
        //    RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
        //    {
        //        EntityFilters = EntityFilters.Entity,
        //        RetrieveAsIfPublished = true
        //    };

        //    // Retrieve the MetaData
        //    RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)orgServiceProxy.Execute(request);

        //    return response;
        //}
        public RetrieveMetadataChangesResponse RetrieveAllEntities()
        {
            var properties = new MetadataPropertiesExpression();
            properties.PropertyNames.AddRange("LogicalName", "DisplayName", "PrimaryIdAttribute", "PrimaryNameAttribute", "SchemaName");

            //An entity query expression to combine the filter expressions and property expressions for the query.
            var entityQueryExpression = new EntityQueryExpression { Properties = properties, };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)orgServiceProxy.Execute(retrieveMetadataChangesRequest);

            return response;
        }

        /// <summary>
        /// Retrieves list Entities passed over in List of string along with it's attribute.
        /// </summary>
        /// <param name="entityNames"></param>
        /// <returns></returns>
        public List<RetrieveEntityResponse> RetrieveEntities(List<string> entityNames)
        {
            List<RetrieveEntityResponse> entityResponses = new List<RetrieveEntityResponse>();
            var multipleRequest = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },


                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            foreach (string entityName in entityNames)
            {
                RetrieveEntityRequest updateRequest = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Entity | EntityFilters.Attributes,
                    LogicalName = entityName,
                    RetrieveAsIfPublished = true
                };
                multipleRequest.Requests.Add(updateRequest);
            }

            ExecuteMultipleResponse response = (ExecuteMultipleResponse)orgServiceProxy.Execute(multipleRequest);

            foreach (var item in response.Responses)
            {
                entityResponses.Add((RetrieveEntityResponse)item.Response);
            }

            return entityResponses;
        }

        /// <summary>
        /// Retrieves list Entities passed over in List of string along with it's attribute and relationship metadata.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        //public RetrieveEntityResponse RetrieveEntity(string entityName)
        //{
        //    RetrieveEntityRequest request = new RetrieveEntityRequest()
        //    {
        //        EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
        //        LogicalName = entityName,
        //        RetrieveAsIfPublished = true
        //    };

        //    RetrieveEntityResponse response = (RetrieveEntityResponse)orgServiceProxy.Execute(request);

        //    return response;
        //}
        public RetrieveMetadataChangesResponse RetrieveEntity(string entityName)
        {
            MetadataFilterExpression EntityFilter = new MetadataFilterExpression(LogicalOperator.And);

            // Filter Entity Metadata by Logical Name
            EntityFilter.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityName));

            MetadataPropertiesExpression EntityProperties = new MetadataPropertiesExpression() { AllProperties = false };

            // Only add Alternate keys properties when Dynamics CRM Version is above 2015 Update 1 since, Dynamics have introduce Alternate key feature after 2015 Update 1
            bool isKeyCompatibleVersion = isCompatibleVersionforKeysAttribute(this.DynamicsCRMVersion);
            // Entity Properties
            EntityProperties.PropertyNames.AddRange(new string[] { "Attributes", "OneToManyRelationships", "LogicalName", "DisplayName", "PrimaryIdAttribute", "PrimaryNameAttribute", "SchemaName" });

            if (isKeyCompatibleVersion)
                EntityProperties.PropertyNames.Add("Keys");

            // Attribute Properties
            MetadataPropertiesExpression AttributeProperties = new MetadataPropertiesExpression() { AllProperties = false };
            AttributeProperties.PropertyNames.AddRange("AttributeType", "LogicalName", "DisplayName", "SchemaName", "AttributeType", "IsPrimaryName", "IsValidForUpdate", "RequiredLevel", "OptionSet", "Targets", "IsPrimaryId",
                "MaxValue", "MinValue", "Precision", "FormulaDefinition", "AttributeOf","Format");

            // 1-N Relationship Properties
            MetadataPropertiesExpression RelationalProperties = new MetadataPropertiesExpression() { AllProperties = false };
            RelationalProperties.PropertyNames.AddRange(" ReferencingEntity", "ReferencingAttribute");

            // Entity Query Expression Request to retrieve ONLY required data
            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = EntityFilter,
                Properties = EntityProperties,
                AttributeQuery = new AttributeQueryExpression()
                {
                    Properties = AttributeProperties
                },
                RelationshipQuery = new RelationshipQueryExpression() { Properties = RelationalProperties }
            };

            RetrieveMetadataChangesRequest req = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression
            };


            RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)orgServiceProxy.Execute(req);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public RetrieveEntityResponse RetrieveEntityAndAttributes(string entityName)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes,
                LogicalName = entityName,
                RetrieveAsIfPublished = true
            };

            RetrieveEntityResponse response = (RetrieveEntityResponse)orgServiceProxy.Execute(request);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Entity GetUserInfo()
        {
            WhoAmIRequest who = new WhoAmIRequest();
            WhoAmIResponse whoResp = (WhoAmIResponse)orgServiceProxy.Execute(who);

            // Retrieve a user.
            Entity user = orgServiceProxy.Retrieve("systemuser",
                whoResp.UserId, new ColumnSet(new String[] { "systemuserid", "firstname", "lastname", "fullname", "domainname", "internalemailaddress", "organizationid" }));
            //                                                                                                     User Name,      Primary Email
            return user;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>

        public DataCollection<Entity> ExecuteQueryExpression(QueryExpression query)
        {
            DataCollection<Entity> entityCollection = null;

            var conversionRequest = new QueryExpressionToFetchXmlRequest
            {
                Query = query
            };
            var conversionResponse = (QueryExpressionToFetchXmlResponse)orgServiceProxy.Execute(conversionRequest);

            // Use the converted query to make a retrieve multiple request to Microsoft Dynamics CRM.
            String fetchXml = conversionResponse.FetchXml;

            EntityCollection EntityList = new EntityCollection();
            if (query.TopCount > 0)
            {
                EntityList = orgServiceProxy.RetrieveMultiple(query);
                entityCollection = EntityList.Entities;
            }
            else
            {
                int pageNumber = 1;
                RetrieveMultipleRequest multiRequest;
                RetrieveMultipleResponse multiResponse = new RetrieveMultipleResponse();
                do
                {
                    query.PageInfo.Count = 5000;
                    query.PageInfo.PagingCookie = (pageNumber == 1) ? null : multiResponse.EntityCollection.PagingCookie;
                    query.PageInfo.PageNumber = pageNumber++;
                    multiRequest = new RetrieveMultipleRequest();
                    multiRequest.Query = query;
                    multiResponse = (RetrieveMultipleResponse)orgServiceProxy.Execute(multiRequest);
                    EntityList.Entities.AddRange(multiResponse.EntityCollection.Entities);
                }
                while (multiResponse.EntityCollection.MoreRecords);

                entityCollection = EntityList.Entities;
            }

            return entityCollection;
        }


        public DataCollection<Entity> RetrieveMultiple(QueryExpression query)
        {
            List<QueryExpression> splittedQueryExpressionList = updateQueryExpressionIntoList(query);

            DataCollection<Entity> lstsObjects = null;

            foreach (QueryExpression chunkQuery in splittedQueryExpressionList)
            {
                DataCollection<Entity> chuncklstObjects = ExecuteQueryExpression(chunkQuery);
                if (lstsObjects == null)
                    lstsObjects = chuncklstObjects;
                else
                    lstsObjects.AddRange(chuncklstObjects);

                // Handle Search & Select Query Case where first 500 records only needs to be show
                if (splittedQueryExpressionList.Count > 1 && chunkQuery.TopCount > 0 && lstsObjects.Count >= chunkQuery.TopCount)
                    return lstsObjects;
            }

            return lstsObjects;
        }

        private QueryExpression CloneQueryExpression(QueryExpression actualQuery)
        {
            // XML Serialization and Deserialization is used here just to create a copy of QueryExpression object
            QueryExpression newQueryExpression = new QueryExpression();
            string queryString = string.Empty;
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(actualQuery.GetType());
            using (System.IO.StringWriter textWriter = new System.IO.StringWriter())
            {
                xmlSerializer.Serialize(textWriter, actualQuery);
                queryString = textWriter.ToString();
            }
            newQueryExpression = (QueryExpression)xmlSerializer.Deserialize(new System.IO.StringReader(queryString));
            return newQueryExpression;
        }

        private List<QueryExpression> updateQueryExpressionIntoList(QueryExpression actualQuery)
        {
            List<QueryExpression> splittedQueryExpression = new List<QueryExpression>();
            int totalInValuesCount = 0;
            if (validateAndUpdateSingleQueryExpression(actualQuery, out totalInValuesCount) != 1)
                splittedQueryExpression.Add(actualQuery);
            else
            {
                int recordCounter = totalInValuesCount;
                int startPosition = 0;
                while (recordCounter > 0)
                {
                    /* Create Copy from Actual Query Expression and modify it */
                    QueryExpression newQueryExpression = CloneQueryExpression(actualQuery);
                    int chunkRecordCount = 0;
                    validateAndUpdateSingleQueryExpression(newQueryExpression, out chunkRecordCount, startPosition);
                    splittedQueryExpression.Add(newQueryExpression);
                    startPosition = startPosition + chunkRecordCount; // Set New Start Index;
                    recordCounter = recordCounter - chunkRecordCount;
                }
            }
            return splittedQueryExpression;
        }


        private int validateAndUpdateSingleQueryExpression(QueryExpression actualQuery, out int totalInValues, int startIndex = -1)
        {
            int exceedingInValuesConditions = 0;
            totalInValues = 0;
            List<QueryExpression> splittedQueries = new List<QueryExpression>();
            // First Check if Query Has Filter with IN / NOT IN Operator
            if (actualQuery.Criteria.Filters.Count > 0)
            {
                foreach (FilterExpression filter in actualQuery.Criteria.Filters)
                {
                    if (filter.Conditions.Count > 0)
                    {
                        for (int cCount = 0; cCount < filter.Conditions.Count; cCount++)
                        {
                            ConditionExpression condition = filter.Conditions[cCount];
                            if ((condition.Operator == ConditionOperator.In || condition.Operator == ConditionOperator.NotIn) && condition.Values.Count > QUERY_IN_COUNT_LIMIT)
                            {
                                exceedingInValuesConditions = exceedingInValuesConditions + 1;
                                totalInValues = condition.Values.Count;
                                if (startIndex > -1)
                                {
                                    filter.Conditions[cCount] = new ConditionExpression(condition.AttributeName, condition.Operator, getSplittedInValues(condition.Values.ToList(), startIndex, totalInValues).ToArray());
                                    totalInValues = filter.Conditions[cCount].Values.Count;
                                }
                            }
                        }
                    }
                }
            }
            else if (actualQuery.Criteria.Conditions.Count > 0)
            {
                for (int cCount = 0; cCount < actualQuery.Criteria.Conditions.Count; cCount++)
                {
                    ConditionExpression condition = actualQuery.Criteria.Conditions[cCount];
                    if ((condition.Operator == ConditionOperator.In || condition.Operator == ConditionOperator.NotIn) && condition.Values.Count > QUERY_IN_COUNT_LIMIT)
                    {
                        exceedingInValuesConditions = exceedingInValuesConditions + 1;
                        totalInValues = condition.Values.Count;
                        if (startIndex > -1)
                        {
                            actualQuery.Criteria.Conditions[cCount] = new ConditionExpression(condition.AttributeName, condition.Operator, getSplittedInValues(condition.Values.ToList(), startIndex, totalInValues).ToArray());
                            totalInValues = actualQuery.Criteria.Conditions[cCount].Values.Count;
                        }
                    }
                }
            }
            // Check with Link Entites Filters
            if (actualQuery.LinkEntities.Count > 0)
            {
                foreach (LinkEntity linkEntity in actualQuery.LinkEntities)
                    validateAndUpdateLinkEntityFilters(linkEntity, ref totalInValues, ref exceedingInValuesConditions, startIndex);
            }
            return exceedingInValuesConditions;
        }

        private void validateAndUpdateLinkEntityFilters(LinkEntity linkEntity, ref int totalInValues, ref int exceedingInValuesConditions, int startIndex = -1)
        {
            if (linkEntity.LinkCriteria.Filters.Count > 0)
            {
                foreach (FilterExpression filter in linkEntity.LinkCriteria.Filters)
                {
                    if (filter.Conditions.Count > 0)
                    {
                        for (int cCount = 0; cCount < filter.Conditions.Count; cCount++)
                        {
                            ConditionExpression condition = filter.Conditions[cCount];

                            if ((condition.Operator == ConditionOperator.In || condition.Operator == ConditionOperator.NotIn) && condition.Values.Count > QUERY_IN_COUNT_LIMIT)
                            {
                                exceedingInValuesConditions = exceedingInValuesConditions + 1;
                                totalInValues = condition.Values.Count;
                                if (startIndex > -1)
                                {
                                    filter.Conditions[cCount] = new ConditionExpression(condition.AttributeName, condition.Operator, getSplittedInValues(condition.Values.ToList(), startIndex, totalInValues).ToArray());
                                    totalInValues = filter.Conditions[cCount].Values.Count;
                                }
                            }
                        }
                    }
                }
            }
            else if (linkEntity.LinkCriteria.Conditions.Count > 0)
            {
                for (int cCount = 0; cCount < linkEntity.LinkCriteria.Conditions.Count; cCount++)
                {
                    ConditionExpression condition = linkEntity.LinkCriteria.Conditions[cCount];
                    if ((condition.Operator == ConditionOperator.In || condition.Operator == ConditionOperator.NotIn) && condition.Values.Count > QUERY_IN_COUNT_LIMIT)
                    {
                        exceedingInValuesConditions = exceedingInValuesConditions + 1;
                        totalInValues = condition.Values.Count;
                        if (startIndex > -1)
                        {
                            linkEntity.LinkCriteria.Conditions[cCount] = new ConditionExpression(condition.AttributeName, condition.Operator, getSplittedInValues(condition.Values.ToList(), startIndex, totalInValues).ToArray());
                            totalInValues = linkEntity.LinkCriteria.Conditions[cCount].Values.Count;
                        }
                    }
                }
            }
            if (linkEntity.LinkEntities.Count > 0)
                foreach (LinkEntity childlinkEntity in linkEntity.LinkEntities)
                    validateAndUpdateLinkEntityFilters(childlinkEntity, ref totalInValues, ref exceedingInValuesConditions, startIndex);
        }

        private List<object> getSplittedInValues(List<object> inValueList, int startIndex, int totalInValues)
        {
            int takeValues = 0;
            if (startIndex + QUERY_IN_COUNT_LIMIT > totalInValues)
                takeValues = totalInValues - startIndex;
            else
                takeValues = QUERY_IN_COUNT_LIMIT;
            return inValueList.Skip(startIndex).Take(takeValues).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toInsert"></param>
        /// <param name="enablePartialSave"></param>
        /// <returns></returns>
        public ExecuteMultipleResponse Insert(EntityCollection toInsert, bool enablePartialSave)
        {
            // Create an ExecuteMultipleRequest object.
            ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a CreateRequest for each entity to the request collection.
            foreach (var entity in toInsert.Entities)
            {
                CreateRequest createRequest = new CreateRequest { Target = entity };
                requestWithResults.Requests.Add(createRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)orgServiceProxy.Execute(requestWithResults);

            return responseWithResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toInsert"></param>
        /// <param name="enablePartialSave"></param>
        /// <returns></returns>
        public ExecuteMultipleResponse Update(EntityCollection toUpdate, bool enablePartialSave)
        {
            // Create an ExecuteMultipleRequest object.
            ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a updateRequest for each entity to the request collection.
            foreach (var entity in toUpdate.Entities)
            {
                UpdateRequest updateRequest = new UpdateRequest { Target = entity };
                requestWithResults.Requests.Add(updateRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)orgServiceProxy.Execute(requestWithResults);

            return responseWithResults;
        }


        public ExecuteMultipleResponse Upsert(EntityCollection toUpsert, bool enablePartialSave)
        {
            // Create an ExecuteMultipleRequest object.
            ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a updateRequest for each entity to the request collection.
            foreach (var entity in toUpsert.Entities)
            {
                UpsertRequest upsertRequest = new UpsertRequest { Target = entity };
                requestWithResults.Requests.Add(upsertRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)orgServiceProxy.Execute(requestWithResults);

            return responseWithResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toDelete"></param>
        /// <param name="enablePartialSave"></param>
        /// <returns></returns>
        public ExecuteMultipleResponse Delete(EntityCollection toDelete, bool enablePartialSave)
        {
            // Create an ExecuteMultipleRequest object.
            ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a updateRequest for each entity to the request collection.
            foreach (var entity in toDelete.Entities)
            {
                DeleteRequest deleteRequest = new DeleteRequest { Target = entity.ToEntityReference() };
                requestWithResults.Requests.Add(deleteRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)orgServiceProxy.Execute(requestWithResults);

            return responseWithResults;
        }


        private bool isCompatibleVersionforKeysAttribute(string versionNo)
        {
            List<string> majorAndMinorVersion = versionNo.Split('.').ToList();

            double majorVersion = Convert.ToDouble(majorAndMinorVersion[0] + "." + majorAndMinorVersion[1]);

            if (majorVersion > 7.0)
                return true;
            else
                return false;

        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            string entityName = string.Empty;
            if (objectNames.Count > 1)
            {
                foreach (string item in objectNames)
                {
                    try
                    {
                        Entity Output = orgServiceProxy.Retrieve(item, primaryID, new ColumnSet());
                        entityName = Output.LogicalName;
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else // AB- 2646 MSD : Validating Multi Lookup field with ID value takes longer time to validate data when more than 25K record
                entityName = objectNames[0];

            return entityName;
        }

    }
}
