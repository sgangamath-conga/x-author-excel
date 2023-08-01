using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.IO;
using Apttus.XAuthor.DynamicsAdapter;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

namespace Apttus.XAuthor.Core
{
    public class ABDynamicsCRMAdapter : IAdapter
    {
        DynamicsCRMServiceController dynamicsConnector = DynamicsCRMServiceController.GetInstance;
        private ILookupIdAndLookupNameProvider LookupIdAndLookupNameProvider;
        private List<ApttusObject> lstApttusObject;
        private ApttusUserInfo apttusUserInfo;


        #region " Dynamics Connector Methods "

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Connect(IOrganizationService orgServiceProxy)
        {
            bool isConnected = dynamicsConnector.ConnectWithDynamicsCRM(orgServiceProxy);
            return isConnected;
        }

        /// <summary>
        /// Connect with Dynamics CRM  
        /// </summary>
        /// <returns></returns>
        public bool Connect(string OAuthToken, string InstanceURL)
        {
            bool isConnected = dynamicsConnector.ConnectWithDynamicsCRM(OAuthToken, InstanceURL);
            apttusUserInfo = getUserInfo();
            return isConnected;
        }


        /// <summary>
        /// Connect with Dynamics CRM  
        /// </summary>
        /// <returns></returns>
        public IOrganizationService Connect(string userName, string password, string InstanceURL)
        {
            return dynamicsConnector.ConnectWithDynamicsCRMSliently(userName, password, InstanceURL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDesigner()
        {
            return true;
        }

        /// <summary>
        /// Same as appbuilderws
        /// </summary>
        /// <returns></returns>
        public string GetClientVersion()
        {
            return "1.0.0311";
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

                RetrieveMetadataChangesResponse allEntities = dynamicsConnector.RetrieveAllEntities();
                //// Filter unnecessary object such like History , Log etc.
                //// Include History Objects for Audit trail like Apps
                //List<DescribeGlobalSObjectResult> sFilteredObjects = results.sobjects.Cast<DescribeGlobalSObjectResult>()
                //    .Where(s => s.keyPrefix != null || s.name.EndsWith(Constants.HISTORY_OBJECT_SUFFIX)).ToList();

                foreach (EntityMetadata item in allEntities.EntityMetadata)
                {
                    ApttusObject apttusObject = new ApttusObject
                    {
                        Id = item.LogicalName,
                        Name = item.DisplayName.UserLocalizedLabel != null ? item.DisplayName.UserLocalizedLabel.Label : item.SchemaName,
                        IdAttribute = item.PrimaryIdAttribute,
                        NameAttribute = item.PrimaryNameAttribute
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
        /// <returns></returns>
        public ApttusObject GetObjectAndFields(string objectName, bool refreshChildObjects)
        {
            RetrieveMetadataChangesResponse entityResponse = dynamicsConnector.RetrieveEntity(objectName);

            ApttusObject apttusObject = FillObject(entityResponse);

            //TODO - as of now we are not adding any chunking mechanism here - so all objects will get loaded with its all children items
            apttusObject.IsFullyLoaded = true;

            if (!refreshChildObjects)
                return apttusObject;

            //if (entityResponse.EntityMetadata.ManyToOneRelationships != null || entityResponse.EntityMetadata.OneToManyRelationships != null)
            if (entityResponse.EntityMetadata[0].OneToManyRelationships != null)
            {
                // TODO:: check if we need ManyToManyRelations  
                //List<string> manyToOneChildObjectIds = (from c in entityResponse.EntityMetadata.ManyToOneRelationships
                //                                        select c.ReferencedEntity).ToList();

                List<string> oneToManyChildObjectIds = (from c in entityResponse.EntityMetadata[0].OneToManyRelationships
                                                        select c.ReferencingEntity).ToList();

                List<string> childObjectIds = new List<string>();
                //childObjectIds.AddRange(manyToOneChildObjectIds);
                childObjectIds.AddRange(oneToManyChildObjectIds);

                List<string> uniqueChildObjectIds = new HashSet<string>(childObjectIds).ToList();

                // Get all Child Objects
                //List<RetrieveEntityResponse> childObjects = dynamicsConnector.RetrieveEntities(uniqueChildObjectIds);

                // Many To One Relations
                //foreach (OneToManyRelationshipMetadata childRelationship in entityResponse.EntityMetadata.ManyToOneRelationships)
                //{
                //    RetrieveEntityResponse sChildObject =  (from c in childObjects
                //                                            where c.EntityMetadata.LogicalName.Equals(childRelationship.ReferencedEntity)
                //                                            select c).FirstOrDefault();

                //    if (sChildObject != null)
                //    {
                //        ApttusObject apttusChildObject = FillObject(sChildObject);
                //        apttusChildObject.LookupName = childRelationship.SchemaName;
                //        apttusObject.Children.Add(apttusChildObject);
                //    }
                //}

                // One To Many Relations
                foreach (OneToManyRelationshipMetadata childRelationship in entityResponse.EntityMetadata[0].OneToManyRelationships)
                {
                    //RetrieveEntityResponse sChildObject = (from c in childObjects
                    //                                       where c.EntityMetadata.LogicalName.Equals(childRelationship.ReferencingEntity)
                    //                                       select c).FirstOrDefault();

                    ApttusObject apttusChildObject = new ApttusObject
                    {
                        Id = childRelationship.ReferencingEntity,
                        Name = childRelationship.ReferencingEntity,
                        Fields = new List<ApttusField>(),
                        ObjectType = ObjectType.Repeating
                    };

                    apttusChildObject.LookupName = childRelationship.ReferencingAttribute;



                    apttusObject.Children.Add(apttusChildObject);
                    //if (sChildObject != null)
                    //{
                    //    ApttusObject apttusChildObject = FillObject(sChildObject);
                    //    apttusChildObject.LookupName = childRelationship.ReferencingAttribute;
                    //    apttusObject.Children.Add(apttusChildObject);
                    //}
                }
            }

            return apttusObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private ApttusObject FillObject(RetrieveMetadataChangesResponse entity)
        {
            if (lstApttusObject == null)
                GetAllStandardObjects();

            EntityMetadata entityMetadata = entity.EntityMetadata[0];

            List<ApttusField> lstApttusFields = new List<ApttusField>();
            ApttusObject apttusObject = new ApttusObject
            {
                Id = entityMetadata.LogicalName,
                Name = entityMetadata.DisplayName.UserLocalizedLabel != null ? entityMetadata.DisplayName.UserLocalizedLabel.Label : entityMetadata.SchemaName,
                IdAttribute = entityMetadata.PrimaryIdAttribute,
                NameAttribute = entityMetadata.PrimaryNameAttribute
            };

            if (entityMetadata.Attributes != null && entityMetadata.Attributes.Length > 0)
            {
                //var filterAttr = entityMetadata.Attributes.Where(a => a.IsValidForAdvancedFind.Value == true && a.AttributeType.Value != AttributeTypeCode.Virtual).ToList();
                //var filterAttr = entityMetadata.Attributes.Where(a => a.AttributeType.Value != AttributeTypeCode.Virtual).ToList();
                var filterAttr = entityMetadata.Attributes.Where(a => string.IsNullOrEmpty(a.AttributeOf)).ToList();

                // Some Expectional entity have Primary Key field as InvalidForAdvanceFind flag, in such case forcefully add Primary Key Attribute
                if (!filterAttr.Any(f => f.LogicalName == apttusObject.IdAttribute))
                    filterAttr.Add(entityMetadata.Attributes.FirstOrDefault(f => f.LogicalName == apttusObject.IdAttribute));

                // Some Expection entity have Primary name field as a virtual field which can be a part of any lookup control, in such case forcefully add Primany Name field
                if (!filterAttr.Any(f => f.LogicalName == apttusObject.NameAttribute))
                    if (!string.IsNullOrEmpty(apttusObject.NameAttribute))
                        filterAttr.Add(entityMetadata.Attributes.FirstOrDefault(f => f.LogicalName == apttusObject.NameAttribute));

                foreach (AttributeMetadata field in filterAttr)
                {
                    ApttusField apttusField = new ApttusField
                    {
                        Id = field.LogicalName,
                        Name = field.DisplayName.UserLocalizedLabel != null && !string.IsNullOrEmpty(field.DisplayName.UserLocalizedLabel.Label) ?
                                field.DisplayName.UserLocalizedLabel.Label : field.SchemaName,
                        Datatype = TranslateSFDataType(field.AttributeType, field),
                        CRMDataType = TranslateCRMDataType(field.AttributeType),
                        PicklistType = TranslateSFPicklist(field),
                        Scale = GetScaleValue(field),  // TODO:: test Scale calculation logic is correct or not
                        Precision = GetPrecisionValue(field),  // TODO:: see what is precision value, when configured as Currency Precision
                        ExternalId = isAlternateKeyField(entityMetadata, field.LogicalName),
                        NameField = Convert.ToBoolean(field.IsPrimaryName),
                        Updateable = Convert.ToBoolean(field.IsValidForUpdate),
                        Required = isRequiredField(field)
                    };

                    if ((apttusField.Datatype == Datatype.Lookup) && (field.GetType() == typeof(LookupAttributeMetadata)))
                    {
                        string referenceObject = ((LookupAttributeMetadata)field).Targets.Length > 0 ? ((LookupAttributeMetadata)field).Targets[0] : string.Empty;

                        ApttusObject lookupObj = lstApttusObject.Find(obj => obj.Id == referenceObject);

                        apttusField.LookupObject = new ApttusObject
                        {
                            Id = referenceObject,
                            ObjectType = ObjectType.Independent,
                            IdAttribute = lookupObj.IdAttribute,
                            NameAttribute = lookupObj.NameAttribute,
                            Name = lookupObj.Name
                        };

                        if (field.AttributeType == AttributeTypeCode.Customer || field.AttributeType == AttributeTypeCode.Owner || field.AttributeType == AttributeTypeCode.PartyList || field.AttributeType == AttributeTypeCode.Lookup)
                        {
                            apttusField.MultiLookupObjects = new List<ApttusObject>();
                            apttusField.MultiLookupObjects = GetMultiLookupObjectList(field);
                        }
                    }

                    else if (apttusField.Datatype == Datatype.Picklist) // No multi select picklist in Dynamics
                    {
                        if (field.GetType() == typeof(PicklistAttributeMetadata))
                        {
                            apttusField.PicklistValues = GetPicklistValues(((PicklistAttributeMetadata)field).OptionSet);
                            apttusField.PicklistKeyValues = GetPicklistKeyValues(((PicklistAttributeMetadata)field).OptionSet);
                        }
                        if (field.GetType() == typeof(StateAttributeMetadata))
                        {
                            apttusField.PicklistValues = GetPicklistValues(((StateAttributeMetadata)field).OptionSet);
                            apttusField.PicklistKeyValues = GetPicklistKeyValues(((StateAttributeMetadata)field).OptionSet);
                        }
                        if (field.GetType() == typeof(StatusAttributeMetadata))
                        {
                            apttusField.PicklistValues = GetPicklistValues(((StatusAttributeMetadata)field).OptionSet);
                            apttusField.PicklistKeyValues = GetPicklistKeyValues(((StatusAttributeMetadata)field).OptionSet);
                        }
                        if (field.GetType() == typeof(BooleanAttributeMetadata))
                        {
                            apttusField.PicklistValues = GetTrueFalseBoolValues(((BooleanAttributeMetadata)field).OptionSet);
                            apttusField.PicklistKeyValues = GetTrueFalseBoolKeyValues(((BooleanAttributeMetadata)field).OptionSet);
                            apttusField.PicklistType = PicklistType.TwoOption;
                        }
                    }
                    lstApttusFields.Add(apttusField);
                }

                apttusObject.Fields = lstApttusFields;

            }
            return apttusObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttributeMetadata"></param>
        /// <returns>bool</returns>
        private List<ApttusObject> GetMultiLookupObjectList(AttributeMetadata field)
        {
            List<ApttusObject> apttusObjectList = new List<ApttusObject>();

            List<string> metaData = ((LookupAttributeMetadata)field).Targets.ToList();

            if (metaData.Count > 1)
            {
                foreach (string entity in metaData)
                {
                    ApttusObject obj = lstApttusObject.Find(obj1 => obj1.Id == entity);
                    apttusObjectList.Add(new ApttusObject { Id = entity, ObjectType = ObjectType.Independent, Name = obj.Name, IdAttribute = obj.IdAttribute, NameAttribute = obj.NameAttribute });
                }
            }
            return apttusObjectList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttributeMetadata"></param>
        /// <returns>bool</returns>
        private bool isRequiredField(AttributeMetadata fieldAttribute)
        {
            bool isRequired = false;

            if ((fieldAttribute.RequiredLevel.Value == AttributeRequiredLevel.ApplicationRequired || fieldAttribute.RequiredLevel.Value == AttributeRequiredLevel.SystemRequired)
                && string.IsNullOrEmpty(fieldAttribute.AttributeOf) && fieldAttribute.IsPrimaryId == false)
            {
                // Exclude standard fields which are handled by Dynamics CRM
                string[] standardFields = { "ownerid", "transactioncurrencyid", "statecode", "createdon", "createdby", "modifiedon", "modifiedby" };

                if (!standardFields.Contains(fieldAttribute.LogicalName))
                    isRequired = true;
            }

            return isRequired;

            ////System Required & Application Required Fields are treated as compulsory fields
            //           (field.RequiredLevel.Value == AttributeRequiredLevel.ApplicationRequired || field.RequiredLevel.Value == AttributeRequiredLevel.SystemRequired)
            //           // Should not be a child attribute
            //           && (string.IsNullOrEmpty(field.AttributeOf))
            //           // Owner, Currency & StateCode & IdAttribute are default field on all entities, does not required users to provide input
            //           && (!field.LogicalName.Contains("ownerid") && !field.LogicalName.Contains("transactioncurrencyid") && !field.LogicalName.Contains("statecode") && field.IsPrimaryId == false)
        }

        private bool isAlternateKeyField(EntityMetadata entity, string fieldName)
        {
            bool isKeyField = false;

            // Added null check, as Alternate key are available only after CRM 2015 Update 1
            if (entity.Keys != null)
            {
                foreach (EntityKeyMetadata key in entity.Keys)
                {
                    if (key.KeyAttributes.Length == 1 && key.KeyAttributes[0].Equals(fieldName))
                    {
                        isKeyField = true;
                        break;
                    }
                }
            }

            return isKeyField;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="picklistEntries"></param>
        /// <returns></returns>
        private List<string> GetPicklistValues(OptionSetMetadata picklistEntries)
        {
            List<string> PicklistValues = new List<string>();
            for (int c = 0; c < picklistEntries.Options.Count; c++)
            {
                PicklistValues.Add(picklistEntries.Options[c].Label.UserLocalizedLabel.Label.ToString());
            }

            return PicklistValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="picklistEntries"></param>
        /// <returns></returns>
        private List<PicklistKeyValuePair> GetPicklistKeyValues(OptionSetMetadata picklistEntries)
        {
            List<PicklistKeyValuePair> PicklistKeyValues = new List<PicklistKeyValuePair>();
            for (int c = 0; c < picklistEntries.Options.Count; c++)
            {
                PicklistKeyValuePair picklistItem = new PicklistKeyValuePair();
                picklistItem.optionKey = picklistEntries.Options[c].Value.ToString();
                picklistItem.optionValue = picklistEntries.Options[c].Label.UserLocalizedLabel.Label.ToString();
                PicklistKeyValues.Add(picklistItem);
            }

            return PicklistKeyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="picklistEntries"></param>
        /// <returns></returns>
        private List<string> GetTrueFalseBoolValues(BooleanOptionSetMetadata picklistEntries)
        {
            List<string> PicklistValues = new List<string>();

            PicklistValues.Add(picklistEntries.FalseOption.Label.UserLocalizedLabel.Label);
            PicklistValues.Add(picklistEntries.TrueOption.Label.UserLocalizedLabel.Label);

            return PicklistValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="picklistEntries"></param>
        /// <returns></returns>
        private List<PicklistKeyValuePair> GetTrueFalseBoolKeyValues(BooleanOptionSetMetadata picklistEntries)
        {
            List<PicklistKeyValuePair> PicklistKeyValues = new List<PicklistKeyValuePair>();

            PicklistKeyValuePair falseOption = new PicklistKeyValuePair();
            falseOption.optionKey = picklistEntries.FalseOption.Value.ToString();
            falseOption.optionValue = picklistEntries.FalseOption.Label.UserLocalizedLabel.Label;
            PicklistKeyValues.Add(falseOption);

            PicklistKeyValuePair trueOption = new PicklistKeyValuePair();
            trueOption.optionKey = picklistEntries.TrueOption.Value.ToString();
            trueOption.optionValue = picklistEntries.TrueOption.Label.UserLocalizedLabel.Label;
            PicklistKeyValues.Add(trueOption);

            return PicklistKeyValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        private Datatype TranslateSFDataType(AttributeTypeCode? fieldType, AttributeMetadata fieldMetadata)
        {
            Datatype dt = new Datatype();

            // TODO:: go through rest of data types and assign appropriate datatype of X-Author
            switch (fieldType)
            {
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.Uniqueidentifier:
                case AttributeTypeCode.String:
                    dt = Datatype.String;
                    break;
                case AttributeTypeCode.Boolean: // Booleans are two option set, so treat them as picklist
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    dt = Datatype.Picklist;
                    break;
                case AttributeTypeCode.BigInt:
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Integer:
                    dt = Datatype.Double;
                    break;
                //case AttributeTypeCode.Boolean:
                //    dt = Datatype.Boolean;
                //    break;
                case AttributeTypeCode.DateTime:
                    DateTimeAttributeMetadata fldMetaData = fieldMetadata as DateTimeAttributeMetadata;
                    if (fldMetaData.Format == DateTimeFormat.DateAndTime)
                        dt = Datatype.DateTime;
                    else if (fldMetaData.Format == DateTimeFormat.DateOnly)
                        dt = Datatype.Date;
                    break;
                case AttributeTypeCode.Money:
                case AttributeTypeCode.Decimal:
                    dt = Datatype.Decimal;
                    break;
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                    dt = Datatype.Lookup;
                    break;
                case AttributeTypeCode.Memo:
                    dt = Datatype.Textarea;
                    break;
                default:
                    dt = Datatype.NotSupported;
                    break;
            }

            return dt;
        }

        private string TranslateCRMDataType(AttributeTypeCode? fieldType)
        {
            string msCRMDataType = string.Empty;

            // TODO:: go through rest of data types and assign appropriate datatype of X-Author
            switch (fieldType)
            {

                case AttributeTypeCode.BigInt:
                case AttributeTypeCode.Integer:
                    msCRMDataType = "Integer";
                    break;
                case AttributeTypeCode.Double:
                    msCRMDataType = "Double";
                    break;
                case AttributeTypeCode.Decimal:
                    msCRMDataType = "Decimal";
                    break;
                case AttributeTypeCode.Uniqueidentifier:
                    msCRMDataType = "Uniqueidentifier";
                    break;
            }

            return msCRMDataType;
        }

        /// <summary>
        /// Returns Precision value for Money and Decimal data types
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private int GetScaleValue(AttributeMetadata field)
        {
            int retVal = 0;

            if (field.GetType() == typeof(MoneyAttributeMetadata))
            {
                retVal = Convert.ToInt32(((MoneyAttributeMetadata)field).Precision);
            }
            else if (field.GetType() == typeof(DecimalAttributeMetadata))
            {
                retVal = Convert.ToInt32(((DecimalAttributeMetadata)field).Precision);
            }
            else if (field.GetType() == typeof(DoubleAttributeMetadata))
            {
                retVal = Convert.ToInt32(((DoubleAttributeMetadata)field).Precision);
            }

            return retVal;
        }

        /// <summary>
        /// Calculates Scale values based on MaxValue
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private int GetPrecisionValue(AttributeMetadata field)
        {
            int retVal = 0;
            double maxVal = 0;
            int precision = 0;

            if (field.GetType() == typeof(MoneyAttributeMetadata))
            {
                maxVal = Convert.ToDouble(((MoneyAttributeMetadata)field).MaxValue);
                precision = Convert.ToInt32(((MoneyAttributeMetadata)field).Precision);
            }
            else if (field.GetType() == typeof(DecimalAttributeMetadata))
            {
                maxVal = Convert.ToDouble(((DecimalAttributeMetadata)field).MaxValue);
                precision = Convert.ToInt32(((DecimalAttributeMetadata)field).Precision);
            }
            else if (field.GetType() == typeof(DoubleAttributeMetadata))
            {
                maxVal = Convert.ToDouble(((DoubleAttributeMetadata)field).MaxValue);
                precision = Convert.ToInt32(((DoubleAttributeMetadata)field).Precision);
            }

            if (maxVal > 0)
                retVal = ((int)Math.Floor(Math.Log10(maxVal)) + 1) + precision;

            // Calculate Scale from MaxValue
            return retVal;
        }

        /// <summary>
        /// TODO:: check if Dynamics have Dependent picklist and Record type like functionality
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private PicklistType TranslateSFPicklist(AttributeMetadata field)
        {
            return PicklistType.Regular;
        }

        public string GetExceptionCode(Exception ex)
        {
            if (ex != null && ex.InnerException != null && ex.InnerException.Message.Equals("The remote server returned an error: (401) Unauthorized."))
            {
                return Constants.SF_INVALID_SESSION_ID;
            }
            return string.Empty;
        }

        public string UnescapeQueryString(string queryString)
        {
            string result = queryString;
            return result;
        }

        public string EscapeQueryString(string queryString)
        {
            string result = queryString;
            return result;
        }

        public ApttusDataSet Search(string soql, ApttusUserInfo userInfo)
        {
            ApttusDataSet dataSet = new ApttusDataSet();
            return dataSet;
        }

        public String getNamespacePrefix()
        {
            return string.Empty;
        }

        public object[] DataSetToObject(ApttusDataSet dataSet)
        {
            return null;
        }

        public void Insert(List<ApttusSaveRecord> InsertObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (InsertObjects != null && InsertObjects.Count > 0)
                {
                    EntityCollection entityToInsert = new EntityCollection();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");

                    foreach (ApttusSaveRecord InsertObject in InsertObjects)
                    {
                        // Process attachment records if any
                        if (InsertObject.HasAttachment)
                        {
                            entityToInsert.Entities.AddRange(GetAttachments(InsertObject));
                            //InsertObject.Attachments.Clear();
                            //InsertObject.Attachments = null;
                        }
                        else if (!isRequiredFieldValueMissing(InsertObject))
                        {
                            Entity entity = new Entity()
                            {
                                LogicalName = InsertObject.ObjectName
                            };

                            GetEntityElements(entity, InsertObject.SaveFields, currentCultureInfo, enUSCulture);

                            entityToInsert.Entities.Add(entity);
                        }
                    }

                    ExecuteMultipleResponse saveResult = ChunkAndExecute(entityToInsert.Entities.ToList(), false, waitWindow, QueryTypes.INSERT, 0, entityToInsert.Entities.Count, enablePartialSave, BatchSize);
                    entityToInsert.Entities.Clear();
                    entityToInsert = null;

                    // Process save Results
                    ProcessResults(saveResult, InsertObjects);
                    // Clear Response - Memory Optimization
                    saveResult = null;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaveRecord"></param>
        /// <returns></returns>
        private List<Entity> GetAttachments(ApttusSaveRecord SaveRecord)
        {
            List<Entity> Attachments = new List<Entity>();
            foreach (var SaveAttachment in SaveRecord.Attachments)
            {
                Entity Attachment = new Entity()
                {
                    LogicalName = Constants.ATTACHMENTOBJECT_DYNAMICS,
                    Attributes = GetAttachmentXMLElement(SaveAttachment)
                };
                Attachments.Add(Attachment);
            }
            return Attachments;
        }

        private AttributeCollection GetAttachmentXMLElement(ApttusSaveAttachment SaveAttachment)
        {
            AttributeCollection xmlElements = new AttributeCollection();

            // Subject
            xmlElements.Add("subject", SaveAttachment.AttachmentName);

            // File Name
            xmlElements.Add("filename", SaveAttachment.AttachmentName);

            // Body
            xmlElements.Add("documentbody", SaveAttachment.Base64EncodedString);

            // TODO:: check if we need mimetype
            //configXML.Attributes.Add("mimetype", "text/xml");

            // Parent Id
            xmlElements.Add("objectid", new EntityReference(SaveAttachment.ObjectId, new Guid(SaveAttachment.ParentId)));

            return xmlElements;
        }

        private Entity GetEntityElements(Entity saveEntity, List<ApttusSaveField> saveFields, CultureInfo currentCultureInfo, CultureInfo targetCultureInfo)
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
                            if (saveField.CRMDataType != null && saveField.CRMDataType.Equals("Uniqueidentifier"))
                            {
                                Guid prsGuid;
                                if (Guid.TryParse(saveField.Value, out prsGuid))
                                    saveEntity[saveField.FieldId] = prsGuid;
                                else saveEntity[saveField.FieldId] = null;
                            }
                            else
                                saveEntity[saveField.FieldId] = saveField.Value;
                            break;
                        case Datatype.Decimal:
                            if (saveField.CRMDataType == "Decimal")
                                saveEntity[saveField.FieldId] = Convert.ToDecimal(saveField.Value == string.Empty ? "0" : saveField.Value);
                            else
                                saveEntity[saveField.FieldId] = new Money(Convert.ToDecimal(saveField.Value == string.Empty ? "0" : saveField.Value));
                            break;
                        case Datatype.Double:
                            if (saveField.CRMDataType == "Integer")
                                saveEntity[saveField.FieldId] = Convert.ToInt32(saveField.Value == string.Empty ? "0" : saveField.Value);
                            else if (saveField.CRMDataType == "Double")
                                saveEntity[saveField.FieldId] = Convert.ToDouble(saveField.Value == string.Empty ? "0" : saveField.Value);
                            else    // TODO:: this is temporary line of code to make sure SFDC -> Dynamics CRM converted Apps work fine, remove it once Apps are fixed.
                                saveEntity[saveField.FieldId] = Convert.ToDecimal(saveField.Value == string.Empty ? "0" : saveField.Value);
                            break;
                        case Datatype.Date:
                            saveEntity[saveField.FieldId] = string.IsNullOrEmpty(saveField.Value) ? (DateTime?)null : Convert.ToDateTime(Convert.ToDateTime(saveField.Value).ToShortDateString());
                            break;
                        case Datatype.DateTime:
                            saveEntity[saveField.FieldId] = string.IsNullOrEmpty(saveField.Value) ? (DateTime?)null : DateTime.SpecifyKind(Convert.ToDateTime(saveField.Value), DateTimeKind.Utc);
                            break;
                        case Datatype.Boolean:
                            saveEntity[saveField.FieldId] = Convert.ToBoolean(saveField.Value);
                            break;
                        case Datatype.Picklist:
                            if (!string.IsNullOrEmpty(saveField.Value))
                                saveEntity[saveField.FieldId] = new OptionSetValue(Convert.ToInt32(saveField.Value));
                            else
                                saveEntity[saveField.FieldId] = null;
                            break;
                        case Datatype.Lookup:
                            Guid parseGuid;
                            if (Guid.TryParse(saveField.Value, out parseGuid))
                                saveEntity[saveField.FieldId] = new EntityReference(saveField.LookupObjectId, parseGuid);
                            else if (string.IsNullOrEmpty(saveField.Value))
                                saveEntity[saveField.FieldId] = null;   // set lookup value to blank or null
                            break;
                        default:
                            saveEntity[saveField.FieldId] = saveField.Value;
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

        private void ProcessResults(ExecuteMultipleResponse allResults, List<ApttusSaveRecord> SaveRecords)
        {
            try
            {
                int cnt = 0;
                // AB-2625 - MSD : Showing Blank Error Message while Some of the records on Save Action are missing required field value
                List<ApttusSaveRecord> actualSaveRecords = SaveRecords.Where(f => string.IsNullOrEmpty(f.ErrorMessage)).ToList();
                foreach (ExecuteMultipleResponseItem result in allResults.Responses)
                {
                    ApttusSaveRecord SaveRecord = actualSaveRecords[cnt++];
                    Guid parseGuid;

                    // if response is null, Entity Insert / Update has failed
                    if (result.Fault == null)
                        SaveRecord.Success = true;
                    else if (result.Response != null && result.Response.Results.Count > 0)
                        SaveRecord.Success = Guid.TryParse(result.Response.Results["id"].ToString(), out parseGuid);
                    else if (result.Fault != null)
                        SaveRecord.Success = false;

                    if (SaveRecord.Success && result.Response.Results.Count == 1) // Insert
                        SaveRecord.RecordId = result.Response.Results["id"].ToString();
                    else if (SaveRecord.Success && result.Response.Results.Count == 2) // Upsert
                    {
                        SaveRecord.RecordId = ((EntityReference)result.Response.Results["Target"]).Id.ToString();

                        // If record was not created but an existing record was found then
                        // change the operation type to Upsert.
                        if (!Convert.ToBoolean(result.Response.Results["RecordCreated"]))
                            SaveRecord.OperationType = QueryTypes.UPSERT;
                    }
                    else if (result.Fault != null)
                    {
                        StringBuilder errorMessage = new StringBuilder();

                        errorMessage.AppendLine("Error Status Code: " + result.Fault.ErrorCode);
                        errorMessage.AppendLine("Error Message: " + result.Fault.Message);

                        // TODO:: do we need foreach for result.Fault.ErrorDetails collection
                        //
                        SaveRecord.ErrorMessage = errorMessage.ToString();
                    }

                    // Clear Response - Memory Optimization
                    result.Response = null;
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
                    EntityCollection entityToUpdate = new EntityCollection();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");
                    Guid parseGuid;

                    foreach (ApttusSaveRecord UpdateObject in UpdateObjects.Where(s => s.Ignore == false))
                    {
                        bool toUpdate = Guid.TryParse(UpdateObject.RecordId, out parseGuid);

                        if (toUpdate)
                        {
                            // Process attachment records if any
                            if (UpdateObject.HasAttachment)
                                entityToUpdate.Entities.AddRange(GetAttachments(UpdateObject));
                            else if (!isRequiredFieldValueMissing(UpdateObject))
                            {
                                Entity entity = new Entity()
                                {
                                    LogicalName = UpdateObject.ObjectName,
                                    Id = parseGuid
                                };

                                GetEntityElements(entity, UpdateObject.SaveFields, currentCultureInfo, enUSCulture);

                                entityToUpdate.Entities.Add(entity);
                            }
                        }
                    }

                    ExecuteMultipleResponse saveResult = ChunkAndExecute(entityToUpdate.Entities.ToList(), false, waitWindow, QueryTypes.UPDATE, 0, entityToUpdate.Entities.Count, enablePartialSave, BatchSize);

                    // Process save Results
                    ProcessResults(saveResult, UpdateObjects);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        //private string getApttusFieldDisplayName(string appObjectName, string saveFieldId)
        //{
        //    string fieldDisplayName = string.Empty;

        //    ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObjectById(appObjectName).FirstOrDefault();

        //    if (appObject.Fields.Exists(f => f.Id.Equals(saveFieldId)))
        //        fieldDisplayName = appObject.Fields.Where(f => f.Id.Equals(saveFieldId)).FirstOrDefault().Name;
        //    else
        //        fieldDisplayName = saveFieldId;

        //    return fieldDisplayName;
        //}

        private bool isRequiredFieldValueMissing(ApttusSaveRecord saveRecord)
        {
            bool valueMissing = false;
            // First, Check SaveRecordFields for missing required fields
            if (saveRecord.SaveFields.Any(f => f.Required == true && string.IsNullOrEmpty(f.Value)))
            {
                List<string> rFieldMissing = saveRecord.SaveFields.Where(f => f.Required == true && string.IsNullOrEmpty(f.Value)).Select(f => f.FieldId).ToList();
                //List<string> rFieldMissingDispName = new List<string>();
                //foreach (string missingField in rFieldMissing)
                //    rFieldMissingDispName.Add(getApttusFieldDisplayName(saveRecord.ObjectName, missingField));

                saveRecord.ErrorMessage = "Required fields : " + String.Join(Constants.COMMA, rFieldMissing) + " missing for Object : " + saveRecord.ObjectName;
                valueMissing = true;

            }

            ///* Required to check this only in case of Insert or Upsert.
            // * Second, Find all required field from ApttusObject of User selection
            // * */
            //if (!valueMissing & (saveRecord.OperationType == QueryTypes.INSERT || saveRecord.OperationType == QueryTypes.UPSERT))
            //{
            //    ApttusObject targetObject = ApplicationDefinitionManager.GetInstance.GetAppObjectById(saveRecord.ObjectName).FirstOrDefault();
            //    List<string> rFieldsOnSaverRecordsObject = saveRecord.SaveFields.Where(f => f.Required = true).Select(f => f.FieldId).ToList(); // All Required fields from ApttusSaveRecord 
            //    List<string> rFieldsOnObject = targetObject.Fields.Where(f => f.Required == true).Select(f => f.Id).ToList(); // All Required fields from User selection                
            //    List<string> rFieldMissingOnSaveObject = rFieldsOnObject.Except(rFieldsOnSaverRecordsObject).ToList(); // Non-Save required field list
            //    if (rFieldMissingOnSaveObject.Count > 0)
            //    {
            //        valueMissing = true;
            //        //saveRecord.e
            //        //saveRecord.ErrorMessage = "Required field fieldname missing";
            //    }


            //    /* Third, Get All required fields from CRM Entity and get List of fields which are not selected for Application */
            //    if (!valueMissing)
            //    {
            //        // Get Dynamics CRM Entity & Attributes and find all required fields 
            //        ApttusObject sourceApttusObject = null;
            //        if (!apttusObjectListwithAllFields.Any(f => f.Id == saveRecord.ObjectName))
            //        {
            //            sourceApttusObject = GetObjectAndFields(saveRecord.ObjectName, false);
            //            apttusObjectListwithAllFields.Add(sourceApttusObject);
            //        }
            //        else
            //            sourceApttusObject = apttusObjectListwithAllFields.Where(f => f.Id == saveRecord.ObjectName).FirstOrDefault();
            //        List<string> rFieldOnSourceObject = sourceApttusObject.Fields.Where(f => f.Required == true).Select(f => f.Id).ToList();
            //        List<string> rFieldMissingOnSourceObject = rFieldOnSourceObject.Except(rFieldMissingOnSaveObject).ToList();
            //        if (rFieldMissingOnSourceObject.Count > 0)
            //            valueMissing = true;
            //    }
            //}
            return valueMissing;
        }

        public void Upsert(List<ApttusSaveRecord> UpsertObjects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow)
        {
            try
            {
                if (UpsertObjects != null && UpsertObjects.Count > 0)
                {
                    EntityCollection entityToUpsert = new EntityCollection();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");
                    // Upsert requires single object type to be passed.
                    string alternateKeyId = string.Empty;
                    string alternateKeyValue = string.Empty;
                    List<string> UniqueUpsertObjects = UpsertObjects.Select(s => s.ObjectName).Distinct().ToList();

                    foreach (string UniqueUpsertObject in UniqueUpsertObjects)
                    {
                        // Find ExternalId column and add to keyattributecollection
                        KeyAttributeCollection alterateKeyAttributes = new KeyAttributeCollection();

                        foreach (ApttusSaveRecord UpsertObject in UpsertObjects.Where(s => s.ObjectName == UniqueUpsertObject).Where(s => s.Ignore == false))
                        {
                            // Process attachment records if any
                            if (UpsertObject.HasAttachment)
                                entityToUpsert.Entities.AddRange(GetAttachments(UpsertObject));
                            else if (!isRequiredFieldValueMissing(UpsertObject))
                            {
                                alterateKeyAttributes.Clear();
                                foreach (ApttusSaveField sf in UpsertObject.SaveFields.Where(f => f.ExternalId))
                                {
                                    alterateKeyAttributes.Add(new KeyValuePair<string, object>(sf.FieldId, sf.Value));
                                }

                                Entity entity = new Entity()
                                {
                                    LogicalName = UpsertObject.ObjectName,
                                    KeyAttributes = alterateKeyAttributes
                                };

                                GetEntityElements(entity, UpsertObject.SaveFields, currentCultureInfo, enUSCulture);

                                entityToUpsert.Entities.Add(entity);
                            }
                        }
                    }

                    ExecuteMultipleResponse saveResult = ChunkAndExecute(entityToUpsert.Entities.ToList(), false, waitWindow, QueryTypes.UPSERT, 0, entityToUpsert.Entities.Count, enablePartialSave, BatchSize);

                    // Process save Results
                    ProcessResults(saveResult, UpsertObjects);
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
                    EntityCollection entityToDelete = new EntityCollection();
                    CultureInfo currentCultureInfo = Utils.GetLatestCulture;
                    CultureInfo enUSCulture = new CultureInfo("en-US");
                    Guid parseGuid;

                    foreach (ApttusSaveRecord DeleteObject in DeleteObjects)
                    {
                        bool toDelete = Guid.TryParse(DeleteObject.RecordId, out parseGuid);

                        if (toDelete)
                        {
                            Entity entity = new Entity()
                            {
                                LogicalName = DeleteObject.ObjectName,
                                Id = parseGuid
                            };

                            entityToDelete.Entities.Add(entity);
                        }
                    }

                    ExecuteMultipleResponse saveResult = ChunkAndExecute(entityToDelete.Entities.ToList(), false, waitWindow, QueryTypes.DELETE, 0, entityToDelete.Entities.Count, enablePartialSave, BatchSize);

                    // Process save Results
                    ProcessResults(saveResult, DeleteObjects);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private ExecuteMultipleResponse ChunkAndExecute(List<Entity> sObjects, bool Insert, WaitWindowView waitWindow, QueryTypes queryType, int current, int total, bool enablePartialSave, int BatchSize)
        {
            if (sObjects.Count == 0)
                return new ExecuteMultipleResponse();

            ExecuteMultipleResponse allResults = null;
            ExecuteMultipleResponse chunkResult = new ExecuteMultipleResponse();
            int MaxTransactionRecords = BatchSize;
            int MaxTransactionUniqueObjects = 10;

            try
            {
                if (sObjects.Count <= MaxTransactionRecords && sObjects.Select(s => s.LogicalName).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    SetWaitMessage(waitWindow, queryType, current + sObjects.Count, total);
                    allResults = ExecuteChunk(sObjects, Insert, enablePartialSave, queryType);
                }
                else if (sObjects.Count > MaxTransactionRecords && sObjects.Select(s => s.LogicalName).Distinct().Count() <= MaxTransactionUniqueObjects)
                {
                    List<Entity> MaxRecordsChunk = sObjects.GetRange(0, MaxTransactionRecords);

                    // Part 1 - Chunk of Max Records
                    SetWaitMessage(waitWindow, queryType, current + MaxRecordsChunk.Count, total);
                    chunkResult = ExecuteChunk(MaxRecordsChunk, Insert, enablePartialSave, queryType);
                    allResults = appendExecuteResponse(allResults, chunkResult);

                    // Part 2 - Chunk of Remaining Records - Recursion
                    List<Entity> RemainingRecords = sObjects.GetRange(MaxTransactionRecords, sObjects.Count - MaxTransactionRecords);
                    chunkResult = ChunkAndExecute(RemainingRecords, Insert, waitWindow, queryType, current + MaxRecordsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults = appendExecuteResponse(allResults, chunkResult);
                }
                else if (sObjects.Select(s => s.LogicalName).Distinct().Count() > MaxTransactionUniqueObjects)
                {
                    List<string> AllUniqueObjects = (from s in sObjects select s.LogicalName).Distinct().ToList();
                    List<string> MaxUniqueObjects = new List<string>();
                    for (int i = 0; i < MaxTransactionUniqueObjects; i++)
                        MaxUniqueObjects.Add(AllUniqueObjects[i]);

                    // Part 1 - Chunk of Max Unique Objects - Recursion
                    List<Entity> MaxUniqueObjectsChunk = sObjects.Where(s => MaxUniqueObjects.Contains(s.LogicalName)).ToList();
                    chunkResult = ChunkAndExecute(MaxUniqueObjectsChunk, Insert, waitWindow, queryType, current + MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults = appendExecuteResponse(allResults, chunkResult);

                    // Part 2 - Chunk of Remaining Unique Objects - Recursion
                    chunkResult = ChunkAndExecute(sObjects.Where(s => !MaxUniqueObjects.Contains(s.LogicalName)).ToList(), Insert, waitWindow, queryType, current + +MaxUniqueObjectsChunk.Count, total, enablePartialSave, BatchSize);
                    allResults = appendExecuteResponse(allResults, chunkResult);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return allResults;
        }

        private ExecuteMultipleResponse ExecuteChunk(List<Entity> Chunk, bool Insert, bool enablePartialSave, QueryTypes queryType)
        {
            RefreshConnection();
            EntityCollection ChunkCollection = new EntityCollection();
            ExecuteMultipleResponse saveResult = new ExecuteMultipleResponse();
            if (Chunk.Count > 0)
            {
                foreach (Entity item in Chunk)
                    ChunkCollection.Entities.Add(item);
            }
            try
            {
                switch (queryType)
                {
                    case QueryTypes.INSERT:
                        saveResult = dynamicsConnector.Insert(ChunkCollection, enablePartialSave);
                        break;
                    case QueryTypes.UPDATE:
                        saveResult = dynamicsConnector.Update(ChunkCollection, enablePartialSave);
                        break;
                    case QueryTypes.UPSERT:
                        saveResult = dynamicsConnector.Upsert(ChunkCollection, enablePartialSave);
                        //UpsertResult[] upsertResult = dynamicsConnector..Upsert(Chunk.ToArray(), ExternalId, enablePartialSave);
                        //chunkResult = upsertResult.Cast<T>().ToList();
                        break;
                    case QueryTypes.DELETE:
                        saveResult = dynamicsConnector.Delete(ChunkCollection, enablePartialSave);
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            ChunkCollection = null;
            Chunk = null;
            return saveResult;
        }

        private ExecuteMultipleResponse appendExecuteResponse(ExecuteMultipleResponse allResults, ExecuteMultipleResponse chunkResult)
        {
            if (allResults == null)
            {
                allResults = new ExecuteMultipleResponse();
                allResults = chunkResult;
            }
            else
                allResults.Responses.AddRange(chunkResult.Responses);
            return allResults;
        }

        private void SetWaitMessage(WaitWindowView waitWindow, QueryTypes queryType, int current, int total)
        {
            if (waitWindow != null)
                waitWindow.Message = "Performing \"" + queryType.ToString() + "\" operation on Dynamics CRM. Processing " + current.ToString() + " of " + total.ToString() + "...";
        }

        public void FillRecordTypeMetadata(ApttusObject apttusObject)
        { }

        public ApttusUserInfo getUserInfo()
        {
            RefreshConnection();
            apttusUserInfo = new ApttusUserInfo();

            Entity result = dynamicsConnector.GetUserInfo();
            apttusUserInfo.UserId = Convert.ToString(result.Id);
            apttusUserInfo.UserName = result.GetAttributeValue<string>("domainname");
            apttusUserInfo.UserEmail = result.GetAttributeValue<string>("internalemailaddress");
            apttusUserInfo.UserFullName = result.GetAttributeValue<string>("fullname");
            apttusUserInfo.ProfileId = string.Empty;
            apttusUserInfo.Locale = string.Empty;
            apttusUserInfo.Language = string.Empty;
            apttusUserInfo.OrganizationId = Convert.ToString(result.GetAttributeValue<System.Nullable<System.Guid>>("organizationid"));
            return apttusUserInfo;
        }

        public List<ApttusObject> GetAppList(int? maxRecords, string searchName, string ownerId)
        {
            QueryExpression query = getAppQueryExpr(maxRecords, searchName, ownerId);

            return Query<ApttusObject>(query);
        }

        private QueryExpression getAppQueryExpr(int? maxRecords, string searchName, string ownerId)
        {
            // TODO:: make IsDesigner flag available for dynamics and add condition for it 
            //if (!IsDesigner())
            //ApttusUserInfo userInfo = getUserInfo();

            //queryStr += " WHERE " + Constants.NAMESPACE_PREFIX + "Activated__c = true"
            //            + " AND Id IN (SELECT " + Constants.NAMESPACE_PREFIX + "ApplicationId__c FROM "
            //            + Constants.NAMESPACE_PREFIX + "UserXAuthorApp__c WHERE "
            //            + Constants.NAMESPACE_PREFIX + "ProfileId__c = '" + userInfo.ProfileId
            //            + "' OR " + Constants.NAMESPACE_PREFIX + "UserId__c = '" + userInfo.UserId + "')";

            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "apttus_xapps_application",
                ColumnSet = new ColumnSet("apttus_name", "apttus_xapps_uniqueid", "apttus_xapps_activated", "modifiedon", "owninguser"),

                LinkEntities =
                {
                    new LinkEntity
                    {
                        JoinOperator = JoinOperator.LeftOuter,
                        LinkFromAttributeName = "ownerid",
                        LinkFromEntityName = "apttus_xapps_application",
                        LinkToAttributeName = "systemuserid",
                        LinkToEntityName = "systemuser",
                        Columns = new ColumnSet("fullname")
                    }
                },

                TopCount = maxRecords,
                //PageInfo = 
                //{
                //    Count = Convert.ToInt32(maxRecords),
                //    PageNumber = 1
                //}
            };

            FilterExpression filters = new FilterExpression();
            filters.FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And;

            if (!string.IsNullOrEmpty(searchName))
                filters.Conditions.Add(new ConditionExpression("apttus_name", ConditionOperator.Like, "%" + searchName + "%"));
            if (!string.IsNullOrEmpty(ownerId))
                filters.Conditions.Add(new ConditionExpression("ownerid", ConditionOperator.Equal, ownerId));

            if (filters.Conditions.Count > 0)
                query.Criteria.Filters.Add(filters);

            query.AddOrder("modifiedon", OrderType.Descending);

            return query;
        }

        public ApttusDataSet GetAppDataSet(int? maxRecords, string searchName, string ownerId, ApttusUserInfo userInfo)
        {
            QueryExpression query = getAppQueryExpr(maxRecords, searchName, ownerId);

            ApttusDataSet appData = QueryDataSet(query, null, null, userInfo);
            return appData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<ApttusObject> Query<T>(QueryExpression query)
        {
            List<ApttusObject> lstApttusObjects = new List<ApttusObject>();
            try
            {
                DataCollection<Entity> lstsObjects = dynamicsConnector.RetrieveMultiple(query);

                foreach (Entity entity in lstsObjects)
                {
                    ApttusObject obj = new ApttusObject();

                    obj.Id = entity.Id.ToString();

                    if (entity.Attributes.Count > 0)
                    {
                        // how to identify name field
                        string nameValue = entity.Attributes.Where(f => f.Key.Equals(Constants.NAME_ATTRIBUTE.ToLower()) || f.Key.EndsWith("_name")).FirstOrDefault().Value.ToString();
                        if (!string.IsNullOrEmpty(nameValue))
                            obj.Name = nameValue;
                    }

                    lstApttusObjects.Add(obj);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return lstApttusObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="appObject"></param>
        /// <param name="dataTable"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public ApttusDataSet QueryDataSet(QueryExpression query, ApttusObject appObject, DataTable dataTable, ApttusUserInfo userInfo)
        {
            ApttusDataSet dataSet = new ApttusDataSet();
            DataCollection<Entity> lstsObjects = dynamicsConnector.RetrieveMultiple(query);
            bool hasAttachmentColumn = false;

            dataSet.DataTable = new DataTable();

            if (lstsObjects.Count == 0)
            {
                if (dataTable != null)
                    dataSet.DataTable = dataTable;
                return dataSet;
            }

            if (dataTable == null)
            {
                // 1. Add Datatable Columns based on 1st record
                foreach (var attr in lstsObjects[0].Attributes)
                {
                    if (!string.IsNullOrEmpty(attr.Key))
                        dataSet.DataTable.Columns.Add(attr.Key);

                    // TODO:: handle lookupobject fields and data or LINKEntity data
                }

                // if appObject is null, for dynamics we need metadata of Entity, for Picklist conversion
                // since dynamics only returns picklist value Number and not the actual user facing value.
                // Correction:: Entity.FormattedValue does return picklist, TODO:: so evaluate if we need this section or not
                //if (object.ReferenceEquals(appObject, null))
                //{
                //    RetrieveEntityResponse entityResponse = dynamicsConnector.RetrieveEntityAndAttributes(query.EntityName);
                //    appObject = FillObject(entityResponse);
                //}

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
            foreach (Entity entity in lstsObjects)
            {
                DataRow row = dataSet.DataTable.NewRow();

                foreach (var attr in entity.Attributes)
                {
                    // field for the Entity
                    //var t1 = entity.GetAttributeValue<var>(attr.Key);
                    //string st1 = t1.ToString();
                    var field = attr;
                    string attrValue = string.Empty;

                    if (attr.Value.GetType() == typeof(AliasedValue))
                    {
                        field = new KeyValuePair<string, object>(attr.Key, ((AliasedValue)attr.Value).Value);
                        // for attachments, change attr.key , column name to Attchment
                        if (((AliasedValue)attr.Value).EntityLogicalName == Constants.ATTACHMENTOBJECT_DYNAMICS && ((AliasedValue)attr.Value).AttributeLogicalName == "filename")
                        {
                            string fullPathToAttachment = GetAttachmentFilePath(appObject, entity, attr);
                            field = new KeyValuePair<string, object>(Constants.ATTACHMENT, fullPathToAttachment);
                            hasAttachmentColumn = true;
                        }
                    }

                    if (field.Value.GetType() == typeof(OptionSetValue))
                        attrValue = entity.FormattedValues[field.Key];
                    else if (field.Value.GetType() == typeof(EntityReference))
                    {
                        // Check if lookup id field
                        if (field.Key.EndsWith(Constants.ID_ATTRIBUTE.ToLower()))
                        {
                            // Get lookup name field, Change endswith id in lowercase to Id
                            string lookupName = ApplicationDefinitionManager.GetInstance.GetLookupNameFromLookupId(field.Key.Substring(0, field.Key.Length - 2) + Constants.ID_ATTRIBUTE);
                            // Check if lookup Name field exists in dataTable, yes then populate Lookup name field
                            if (dataSet.DataTable.Columns.Contains(lookupName))
                            {
                                SetDynamicsFieldValue(dataSet.DataTable.Columns[lookupName], row, lookupName, ((EntityReference)field.Value).Name, userInfo);
                            }
                        }
                        else
                        {
                            string lookupName = field.Key + Constants.APPENDLOOKUPID;
                            if (dataSet.DataTable.Columns.Contains(lookupName))
                            {
                                SetDynamicsFieldValue(dataSet.DataTable.Columns[lookupName], row, lookupName, ((EntityReference)field.Value).Name, userInfo);
                            }

                        }
                        attrValue = ((EntityReference)field.Value).Id.ToString();
                    }

                    else if (field.Value.GetType() == typeof(AliasedValue))
                        attrValue = ((AliasedValue)field.Value).Value.ToString();
                    else if (field.Value.GetType() == typeof(Money))
                        attrValue = ((Money)field.Value).Value.ToString();
                    else if (attr.Value.GetType() == typeof(Boolean))
                    {
                        if (appObject != null)
                        {
                            // get true or false value and assign to attrValue
                            ApttusField apttusField = appObject.Fields.Where(s => s.Id == field.Key).FirstOrDefault();
                            if (apttusField != null)
                            {
                                attrValue = apttusField.PicklistKeyValues[Convert.ToInt32(field.Value)].optionValue;
                            }
                        }
                        else
                            attrValue = field.Value.ToString();
                    }
                    else
                        attrValue = Convert.ToString(field.Value);

                    SetDynamicsFieldValue(dataSet.DataTable.Columns[field.Key], row, field.Key, attrValue, userInfo);

                    // Populate ID column in Datatable for dynamics
                    if (appObject != null &&
                        (field.Key.Equals(appObject.IdAttribute) && field.Value.GetType() == typeof(Guid)))
                    {
                        SetDynamicsFieldValue(dataSet.DataTable.Columns[Constants.ID_ATTRIBUTE], row, Constants.ID_ATTRIBUTE, attrValue, userInfo);
                    }
                    // TODO:: field for the lookup object

                    //string columnName = field.LocalName + Constants.DOT + field.ChildNodes[i].LocalName;
                    //PopulateChildObjectData(field.ChildNodes[i], row, dataSet, columnName, userInfo);
                }

                // In case of multiple attachments on one record, we get multiple results for same record, so aggregate attachment filenames here instead of adding new rows
                if (hasAttachmentColumn)
                {
                    if (dataTable.Columns.Contains(Constants.ATTACHMENT)) // AB-2699 :- MSD : Search and Select Action breaks when having Attachment field into Display map
                    {
                        //string ID_ATTRIBUTE = ConfigurationManager.GetInstance.GetIdAttribute(appObject, false);
                        string ID_ATTRIBUTE = appObject.IdAttribute;
                        DataRow existsRow = dataSet.DataTable.Select(ID_ATTRIBUTE + " = '" + row[ID_ATTRIBUTE].ToString() + "'").FirstOrDefault();
                        if (existsRow != null)
                            existsRow[Constants.ATTACHMENT] = existsRow[Constants.ATTACHMENT] + "|" + row[Constants.ATTACHMENT];
                        else
                            dataSet.DataTable.Rows.Add(row);
                    }
                    else
                        dataSet.DataTable.Rows.Add(row);
                }
                else
                    dataSet.DataTable.Rows.Add(row);
            }

            return dataSet;
        }


        private static string GetAttachmentFilePath(ApttusObject appObject, Entity entity, KeyValuePair<string, object> attr)
        {
            string annotationId = ((AliasedValue)entity.Attributes["annotationid.annotationid"]).Value.ToString();
            string objectId = entity.Attributes[appObject.IdAttribute].ToString();
            string parentFolderName = appObject.Id + Constants.ATTACHMENTSEPARATOR + objectId;
            string fileNameExtension = Path.GetExtension(((AliasedValue)attr.Value).Value.ToString());
            string fullPathToAttachment = Utils.GetTempFileName(parentFolderName, Path.GetFileNameWithoutExtension(((AliasedValue)attr.Value).Value.ToString()) + fileNameExtension, true);
            return fullPathToAttachment;
        }

        private static void SetDynamicsFieldValue(DataColumn dataColumn, DataRow row, string fieldName, string value, ApttusUserInfo userInfo)
        {
            // dataColumn == null, is added since Dynamics returns attributes other than those in QueryExpression
            // e.g. transactioncurrencyid etc...
            if (dataColumn == null)
                return;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public ApplicationObject LoadApplication(string recordId, Guid uniqueId)
        {
            ApplicationObject App = null;

            QueryExpression noteattachmentQuery = new QueryExpression()
            {
                EntityName = "annotation",
                ColumnSet = new ColumnSet("filename", "documentbody"),

                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("objectid", ConditionOperator.Equal, recordId),
                                new ConditionExpression("isdocument", ConditionOperator.Equal, true)
                            }
                        }
                    }
                }
            };

            DataCollection<Entity> result = dynamicsConnector.RetrieveMultiple(noteattachmentQuery);

            if (result != null && result.Count > 0) // AB-2636 : MSD: While trying to open a particular app getting the error of 'Index was out of range' in the logs. 
            {
                App = new ApplicationObject();
                App.Config = Convert.ToString(result[0].Attributes["filename"]).Equals("AppDefinition.xml") ? Convert.FromBase64String(result[0].Attributes["documentbody"].ToString()) : null;
                App.Template = Convert.ToString(result[1].Attributes["filename"]).StartsWith("Template.xl") ? Convert.FromBase64String(result[1].Attributes["documentbody"].ToString()) : null;
                App.TemplateName = Convert.ToString(result[1].Attributes["filename"]);
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

        public bool saveApplication(string recordId, byte[] config, byte[] template, string templateName, byte[] schema)
        {
            bool result = false;

            EntityCollection annotationsToInsert = new EntityCollection();

            // Config XML attachment
            Entity configXML = new Entity("annotation");
            configXML.Attributes.Add("subject", "AppDefinition");
            configXML.Attributes.Add("filename", "AppDefinition.xml");
            configXML.Attributes.Add("documentbody", Convert.ToBase64String(config));
            configXML.Attributes.Add("mimetype", "text/xml");
            configXML.Attributes.Add("objectid", new EntityReference("apttus_xapps_application", new Guid(recordId)));

            annotationsToInsert.Entities.Add(configXML);

            // Template XLSX / XLSM attachment
            Entity templateXLS = new Entity("annotation");
            templateXLS.Attributes.Add("subject", "Template");
            templateXLS.Attributes.Add("filename", templateName);
            templateXLS.Attributes.Add("documentbody", Convert.ToBase64String(template));
            templateXLS.Attributes.Add("mimetype", "application/vnd.ms-excel");
            templateXLS.Attributes.Add("objectid", new EntityReference("apttus_xapps_application", new Guid(recordId)));

            annotationsToInsert.Entities.Add(templateXLS);

            dynamicsConnector.Insert(annotationsToInsert, false);

            // Update app record, so that it App List is ordered correctly
            EntityCollection currentApp = new EntityCollection();

            Entity app = new Entity("apttus_xapps_application");
            Guid parseId;
            if (Guid.TryParse(recordId, out parseId))
                app.Id = parseId;
            currentApp.Entities.Add(app);

            dynamicsConnector.Update(currentApp, false);

            result = true;

            return result;
        }

        public bool DeleteTemplate(EntityCollection appAttachments)
        {
            bool result;
            try
            {
                dynamicsConnector.Delete(appAttachments, false);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ExceptionLogHelper.ErrorLog(ex);
            }
            return result;
        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            return dynamicsConnector.GetObjectNameByPrimaryId(primaryID, objectNames);
        }

        #endregion

        public int CreateOptionSetValue(string entityname, string attributename, string optionsetvalue)
        {
            InsertOptionValueRequest insertOptionValueRequest =
                new InsertOptionValueRequest
                {
                    //OptionSetName = "accountratingcode",
                    AttributeLogicalName = attributename,
                    EntityLogicalName = entityname,
                    Label = new Label(optionsetvalue, 1033)
                };

            // Execute the request and store the newly inserted option value 
            // for cleanup, used in the later part of this sample.
            int _insertedOptionValue = ((InsertOptionValueResponse)dynamicsConnector.orgServiceProxy.Execute(
                insertOptionValueRequest)).NewOptionValue;

            return _insertedOptionValue;

        }

        public bool Connect(IXAuthorCRMLogin login)
        {
            IXAuthorDynamicsLogin dynamicsLogin = login as IXAuthorDynamicsLogin;
            bool bConnected = Connect(dynamicsLogin.OrganizationService);
            DynamicsRefreshConnection.GetInstance.SetRefreshSessionInfo(dynamicsLogin.RedirectUrl, dynamicsLogin.ClientId, dynamicsLogin.AuthorityURL, dynamicsLogin.TokenExpiresOn, dynamicsLogin.ConnectionEndPoint);
            //This is needed so that the field level security is working in case of dynamics. We need to set the namespace prefix so that we can come to know whether the user is in offline mode.
            if (bConnected)
                Constants.NAMESPACE_PREFIX = "Dummy_Namespace_Prefix Which is not used in dynamics";

            apttusUserInfo = getUserInfo();

            return bConnected;
        }

        public ApttusObject GetObjectAndFields(string objectName, bool refreshChildObjects, int noofChildObjectsLoaded)
        {
            return GetObjectAndFields(objectName, refreshChildObjects);
        }

        public string GetPartnerAPIVersion()
        {
            return string.Empty;
        }

        public bool IsAdminUser()
        {
            return true;
        }

        public List<ApttusObject> Query<T>(XAuthorQuery query)
        {
            DynamicsQuery dynQuery = query as DynamicsQuery;
            return Query<T>(dynQuery.Query);
        }

        public ApttusDataSet QueryDataSet(XAuthorQuery query)
        {
            RefreshConnection();
            DynamicsQuery dynQuery = query as DynamicsQuery;
            return QueryDataSet(dynQuery.Query, dynQuery.Object, dynQuery.DataTable, dynQuery.UserInfo);
        }

        public string GetLicenseDetail()
        {

            //This method is psudo implementation and it has to be replaced when Licensing is implemented for Dynamics
            string licenseDetails = "<XAuthor>ZmW4pWXnKmrQwbDwUuMq8O0DUdpw/WJpTH5Zx15iPSMgVZuy9D4gWJs80OvniF+68qmGtQtcoE2nZaWRGnO+xKRxl0Pg1p7wGrc0v/zvFoA4kXkhST3hRmk544ZTzgEClnnpaeMgLS48BDmoWlUPo9nYM6ZGqFW5VVDMFp5Ad+27suLbDyrhXbheI9Vwccpy6wZ6pAdbWeDc7TVAqV5y0m+Hx7MTucF2a/Txr5SDuUYpIiJ6ga82M9NYsGsk530+IEoTkBq0Xhh+oOiTGg1k3ZJB6V/lVr6rBV90BAB9VjglprF+FoMvFV5MxZbbsd9kuwyAGZButMw/npJKLzi6mNCrFFIqEiWN5Czj+yDoybPC+qWf3B+nVmTWH4JTKxl2Jo1KmRh/ZCylMu8rCzxMumJHA0dnack0zZGuYOaUsiolBdeP95/knjxM8GbQPTnKWcwOZaXFM22eVwbPau5gFTTN95sNk3rmo6WbrW0cQWe8zDfFIHDTK/syKBjdnH0Pl/RfKr2mvQZSo0LymjhvlcbHgEM4TsNOVHucicIR3Ie7lpxRYzFfT9IpUD45SfgiAQ1GqqjUsln0OE83TMWSHQmanmlmdXyhBirgKqgMTzry9Oodqow/69sKIU0qnkJAtZdht++SLZqHFqunJZiEkP4dy9Vi8Q6mcO4kc8YjGs+iMEeRjgKAJokUYfm9qC6RR5kzfFwnKwOrFNDT1Lm6MqU7/Wj26wNQTXmJokqqTzvTjvbi/+2dxnzENGlZgZWYRz3FZR+hDr1v2Zmei1edlGV7q3c4g67tBwLsOy0a9HfwtffR5hQ0fPQCTnM5HAmUgE4aFZEFaBKUFThXsgERXiT16rc/u6Nf3oS4d0MIggtMOb/ND5WWbp2BUeOElqee3Js+WQKiG2KjV3Gj3yNpPhmkvh6KL3q/1t4SR5Y/c/OQJ3k5C3F6rn5Mrme3fXL0Nld29A6WmgjCvQx1RoMcFVCvy+Zp0zsy5MNS8padauBah4D3xgIUzlIsYPU2vQr9lT3jnr11M28XgJNDcnR6Ih6TKzXJOK9kVzAW+wzYUM1mNep5w6sJeuzDUN4F5f3PZcnUT86oJXexD5ZBmTNOf/7xEERWksZkTQHjqvw9WKXrzwtKd8l5+difTR+4OZ2/dhBw1kSyhMXFeePfQ2l7wdEzM47bn7ry6w2UC9y9v+8FMKWtA5T8d72+YDcyYYJErWTjLUXXWtDrkrAMdpUR7uaAgQXJU7/lbKD4Xl5/YjzdhXcrXGnsAHdG5BwT7mj2d5Bic/7ewk/ygrLMKj2Nr5Wef7xDBRXJ3Znw4vYszyQWR3m+0Fm61WybK/4tENrW7Datg/AQO8BvF+8R9DFzE8BfiW7WiqIsEDebOtZkGk/PsRC/4lYcfh9EzGLNb8Moxvf7aysANzH7lh1wb1rYda/NXfFcbO58WekJFzNv0GZEvoZurfoPZZOQ5kqg3p5Mmvm6chVuJt81Xe0NB5SFd3bd19vrMMP8Ofo1DsoTsWMGWdhISCJ5/kNzBfsx3FJu+EkQV7nl68ZBZf+BYdpgSwctdjELUrSM43os5mnLYsH3BTLEEgE5Yv3XI+Drjjdxr+3jpnfdnpfqzSWpI+toZv1xemVlLb8jjncqhdh0tuIMq6eDMGsax8gxEB9vi2x7DHvNMGGuuDr0dmu/Zrq4zY+lXsEICGlckdkBdu4CSgE=</XAuthor>";
            string decryptedLicenseDetails = EncryptionHelper.Decrypt(licenseDetails);
            return decryptedLicenseDetails;
        }

        public string GetFeatureDetail()
        {
            //This method is psudo implementation and it has to be replaced when Licensing is implemented for Dynamics
            string featureDetails = "<XAuthor>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</XAuthor>";
            string decryptedFeatureDetails = EncryptionHelper.Decrypt(featureDetails);
            return decryptedFeatureDetails;
        }

        public bool IsAdminPackageEditionUser()
        {
            //This method is psudo implementation and it has to be replaced when Licensing is implemented for Dynamics 
            return true;
        }

        public bool IsSandBox()
        {
            //This method is psudo implementation and it has to be replaced when Licensing is implemented for Dynamics 
            return false;
        }

        public string GetLicenseFilePath()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus\\");

            DirectoryInfo di = new DirectoryInfo(folderName);
            if (!di.Exists)
                di.Create();

            return folderName + "MSDynamics_";
        }

        public bool HasCollisionDetection()
        {
            return false;
        }


        public ILookupIdAndLookupNameProvider GetLookUpIdAndNameProvider()
        {
            if (LookupIdAndLookupNameProvider == null)
                LookupIdAndLookupNameProvider = new DynamicsLookupIdAndNameProvider();

            return LookupIdAndLookupNameProvider;
        }

        public void SetAppDataTableProperties(ref DataTable table)
        {
            if (table.Columns.Contains("apttus_xapps_applicationid"))
                table.Columns["apttus_xapps_applicationid"].ColumnName = Constants.ID_ATTRIBUTE;

            if (table.Columns.Contains("apttus_name"))
                table.Columns["apttus_name"].ColumnName = Constants.NAME_ATTRIBUTE;

            if (table.Columns.Contains("systemuser1.fullname"))
                table.Columns["systemuser1.fullname"].ColumnName = "Owner.Name";

            if (table.Columns.Contains("modifiedon"))
                table.Columns["modifiedon"].ColumnName = "LastModifiedDate";
        }

        public ApttusDataSet GetAttachmentsDataSet(string ParentIds, string ObjectId)
        {
            string[] splitObjIds = ParentIds.Replace("'", string.Empty).Split(',');
            var QueryExpression = ExpressionBuilderHelperDynamics.GetAnnotationsForObjectId(splitObjIds.ToList());
            var tempDataSet = QueryDataSet(new DynamicsQuery() { Query = QueryExpression });
            // Transform Data table Column names
            if (tempDataSet.DataTable != null && tempDataSet.DataTable.Columns.Count == 5)
            {
                tempDataSet.DataTable.Columns[0].ColumnName = Constants.NAME_ATTRIBUTE;
                tempDataSet.DataTable.Columns[1].ColumnName = "Body";
                tempDataSet.DataTable.Columns[2].ColumnName = "ParentId";
                tempDataSet.DataTable.Columns[3].ColumnName = Constants.ID_ATTRIBUTE;
                tempDataSet.DataTable.Columns[4].ColumnName = "Parent.Name";
            }

            return tempDataSet;
        }

        public ApttusDataSet GetLookUpDetails(ApttusObject apttusObject, List<string> LookupNames)
        {
            QueryExpression queryExpr = new QueryExpression
            {
                Distinct = false,
                EntityName = apttusObject.Id,
                ColumnSet = new ColumnSet(apttusObject.NameAttribute),
                Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression(apttusObject.NameAttribute, ConditionOperator.In, LookupNames.ToArray())
                                    }
                                }
                            }
                        }
            };

            return QueryDataSet(new DynamicsQuery() { Query = queryExpr });
        }

        public ApttusDataSet GetDataSetByLookupSearch(ApttusObject lookupObj, string search)
        {
            ApttusDataSet ds = new ApttusDataSet();
            QueryExpression query = null;
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
            query = new QueryExpression(lookupObj.Id)
            {
                ColumnSet = new ColumnSet(fieldSet.ToArray())
            };
            if (!string.IsNullOrEmpty(search))
                query.Criteria.AddCondition(nameField, ConditionOperator.Like, search + Constants.PERCENT);

            ds = QueryDataSet(new DynamicsQuery() { Query = query });
            return ds;
        }

        public void RefreshConnection()
        {
            DynamicsRefreshConnection.GetInstance.RefreshConnection();
        }

        public AppAssignmentModel GetAppAssignmentDetails(string AppId)
        {
            AppAssignmentModel Model = new AppAssignmentModel
            {
                ApplicationId = AppId
            };
            string[] Columns = { "apttus_name", "apttus_xapps_applicationidid", "apttus_xapps_profileid", "apttus_xapps_useridid", "apttus_xapps_userxauthorappid" };
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "apttus_xapps_userxauthorapp",

                ColumnSet = new ColumnSet(Columns),

                LinkEntities =
                {
                    new LinkEntity
                    {
                        JoinOperator = JoinOperator.LeftOuter,
                        LinkFromAttributeName = "apttus_xapps_useridid",
                        LinkFromEntityName = "apttus_xapps_application",
                        LinkToAttributeName = "systemuserid",
                        LinkToEntityName = "systemuser",
                        Columns = new ColumnSet("fullname","domainname"),
                        EntityAlias="User"
                    }
                },

                TopCount = 500
            };
            FilterExpression filters = new FilterExpression
            {
                FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And
            };

            if (!string.IsNullOrEmpty(AppId))
                filters.Conditions.Add(new ConditionExpression("apttus_xapps_applicationidid", ConditionOperator.Equal, AppId));

            if (filters.Conditions.Count > 0)
                query.Criteria.Filters.Add(filters);
            DataTable dataTable = new DataTable();
            foreach (string col in Columns)
            {
                dataTable.Columns.Add(new DataColumn(col));
            }
            dataTable.Columns.Add("User.fullname");
            dataTable.Columns.Add("User.domainname");
            ApttusDataSet apttusDataSet = QueryDataSet(new DynamicsQuery() { Query = query, UserInfo = UserInfo, DataTable = dataTable });

            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                Model.Assignments.Add(new Assignments()
                {
                    AssignmentId = row["apttus_xapps_userxauthorappid"].ToString(),
                    AssignmentName = row["apttus_name"].ToString(),
                    User = (apttusDataSet.DataTable.Columns.Contains("apttus_xapps_useridid") && !string.IsNullOrEmpty(row["apttus_xapps_useridid"].ToString())) ? new User()
                    {
                        Id = row["apttus_xapps_useridid"].ToString(),
                        Name = row["User.fullname"].ToString(),
                        Email = row["User.domainname"].ToString(),
                        Checked = true
                    } : null,
                    Profile = !string.IsNullOrEmpty(row["apttus_xapps_profileid"].ToString()) ? new Profile()
                    {
                        Id = row["apttus_xapps_profileid"].ToString(),
                        Name = null,
                        Checked = true
                    } : null,
                });
            }
            return Model;
        }

        public List<User> GetUsersList(string searchStr, string ExceptIds)
        {
            string[] Columns = { "systemuserid", "fullname", "domainname" };
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "systemuser",
                ColumnSet = new ColumnSet(Columns),
                TopCount = 500
            };


            if (!string.IsNullOrEmpty(searchStr))
            {
                FilterExpression filters = new FilterExpression
                {
                    FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.Or
                };
                filters.Conditions.Add(new ConditionExpression("fullname", ConditionOperator.Like, string.Format("%{0}%", searchStr)));
                filters.Conditions.Add(new ConditionExpression("domainname", ConditionOperator.Like, string.Format("%{0}%", searchStr)));
                query.Criteria.Filters.Add(filters);
            }
            if (!string.IsNullOrEmpty(ExceptIds))
            {
                string[] IDs = ExceptIds.Replace('\'', ' ').Split(',');
                foreach (var id in IDs)
                {
                    if (Guid.TryParse(id.Trim(), out Guid userId))
                        query.Criteria.Conditions.Add(new ConditionExpression("systemuserid", ConditionOperator.NotEqual, userId));
                }
            }
            ApttusDataSet apttusDataSet = QueryDataSet(new DynamicsQuery() { Query = query, UserInfo = UserInfo });
            List<User> users = new List<User>();
            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                users.Add(new User
                {
                    Id = row["systemuserid"].ToString(),
                    Name = row["fullname"].ToString(),
                    Email = row["domainname"].ToString(),
                    Checked = false
                });
            }
            return users;
        }

        public List<Profile> GetProfileList(string searchStr, string ExceptIds)
        {
            string[] Columns = { "roleid", "name" };
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "role",
                ColumnSet = new ColumnSet(Columns),
                TopCount = 500
            };
            if (!string.IsNullOrEmpty(searchStr))
            {
                FilterExpression filters = new FilterExpression
                {
                    FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And
                };
                filters.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, string.Format("%{0}%", searchStr)));
                query.Criteria.Filters.Add(filters);
            }
            if (!string.IsNullOrEmpty(ExceptIds))
            {
                string[] IDs = ExceptIds.Replace('\'', ' ').Split(',');
                foreach (var id in IDs)
                {
                    if (Guid.TryParse(id.Trim(), out Guid roleId))
                        query.Criteria.Conditions.Add(new ConditionExpression("roleid", ConditionOperator.NotEqual, roleId));
                }
            }
            ApttusDataSet apttusDataSet = QueryDataSet(new DynamicsQuery() { Query = query, UserInfo = UserInfo });
            List<Profile> profiles = new List<Profile>();
            foreach (DataRow row in apttusDataSet.DataTable.Rows)
            {
                profiles.Add(new Profile
                {
                    Id = row["roleid"].ToString(),
                    Name = row["name"].ToString(),
                    Checked = false
                });
            }
            return profiles;

        }

        public ApttusUserInfo UserInfo { get { return apttusUserInfo; } }
    }
}
