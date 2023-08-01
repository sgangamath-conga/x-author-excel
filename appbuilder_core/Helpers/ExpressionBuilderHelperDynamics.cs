using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

using EBHelper = Apttus.XAuthor.Core.ExpressionBuilderHelper;

namespace Apttus.XAuthor.Core
{
    public class ExpressionBuilderHelperDynamics
    {
        static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        static LinkEntity leastNodeLinkEntity = null;
        static FilterExpression lastLinkFilterExpression = null;
        public static string DisplayableWhereClause {
            get {
                string s = displayableWhereClause;
                displayableWhereClause = string.Empty;
                return s;
            }
        }

        private static string displayableWhereClause;
        private static string FindLinkEntityofParentApttusObject(ApttusObject sourceObject, string lookupName, string linkFromEntity = null)
        {
            string linkEntityName = string.Empty;

            if (sourceObject != null)
            {
                if (sourceObject.Fields.Exists(f => f.Id == lookupName))
                {
                    ApttusField lookupField = sourceObject.Fields.Where(f => f.Id == lookupName).FirstOrDefault();
                    if (lookupField.LookupObject != null)
                        linkEntityName = lookupField.LookupObject.Id;
                }
                else if (sourceObject.Fields.Exists(f => f.Id == lookupName + Constants.ID_ATTRIBUTE.ToLower()))
                {
                    ApttusField lookupField = sourceObject.Fields.Where(f => f.Id == lookupName).FirstOrDefault();
                    if (lookupField.LookupObject != null)
                        linkEntityName = lookupField.LookupObject.Id;
                }
                if (string.IsNullOrEmpty(linkEntityName))
                {
                    if (sourceObject.Parent != null)
                        return FindLinkEntityofParentApttusObject(sourceObject.Parent, lookupName);

                    // find relation from All List Object by linkFromEntity and get relation
                    else if (!string.IsNullOrEmpty(linkFromEntity))
                        linkEntityName = applicationDefinitionManager.GetAppObjectById(linkFromEntity).FirstOrDefault().Fields.Where(f => f.Id == lookupName).FirstOrDefault().LookupObject.Id;
                }
            }
            return linkEntityName;
        }

        private static bool getLinkFromAttributeandLinkToEntity(ApttusObject currentAppObject, ApplicationField currentAppField, string entityOrFieldName, ref string linkToEntity, ref string linkFromAttribute, ref string lookUpName)
        {
            bool linkFound = false;

            if (currentAppObject != null)
            {
                if (currentAppObject.Fields.Any(f => f.Id.Contains(entityOrFieldName) && f.LookupObject != null))
                {

                    ApttusField field = currentAppObject.Fields.FirstOrDefault(f => f.Id.Contains(entityOrFieldName) && f.LookupObject != null);
                    if (field.LookupObject != null)
                    {
                        lookUpName = field.Id;
                        linkFromAttribute = lookUpName;
                        linkToEntity = field.LookupObject.Id;
                        linkFound = true;
                    }
                }
                else if (currentAppObject.Fields.Any(f => f.MultiLookupObjects != null && f.LookupObject != null))
                {
                    List<ApttusField> multiLookupFields = currentAppObject.Fields.Where(f => f.LookupObject != null).ToList();
                    foreach (ApttusField field in multiLookupFields)
                    {
                        if (field.MultiLookupObjects.Count > 0 && !field.Id.EndsWith(Constants.APPENDLOOKUPID))
                        {
                            if (field.MultiLookupObjects.Any(f => f.Id.Equals(entityOrFieldName)))
                            {
                                lookUpName = field.MultiLookupObjects.FirstOrDefault(f => f.Id.Contains(entityOrFieldName)).Id;
                                linkFromAttribute = field.Id;
                                linkToEntity = lookUpName;
                                linkFound = true;
                                break;
                            }
                        }
                        else if (field.LookupObject != null)
                        {
                            if (field.LookupObject.Id.Equals(entityOrFieldName))
                            {
                                lookUpName = field.LookupObject.Id;
                                linkFromAttribute = field.Id;
                                linkToEntity = lookUpName;
                                linkFound = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                //First Lookup For Object Hirerchy if Field Found or Not
                ApttusObject currentFieldAppObject = applicationDefinitionManager.GetAppObject(currentAppField.AppObject);
                ApttusField lookupField = getAppFieldFromObjectHirerchy(currentFieldAppObject, entityOrFieldName);

                if (lookupField == null)
                {
                    // Lookup for All Available Object into Application to find matching field(this would be used on Stand Alone Object Case)
                    foreach (ApttusObject appObject in applicationDefinitionManager.GetAllObjects())
                    {
                        if (appObject.Fields.Exists(f => f.Id.Equals(entityOrFieldName) && f.LookupObject != null && !f.Id.EndsWith(Constants.APPENDLOOKUPID)))
                        {
                            lookupField = appObject.Fields.Where(f => f.Id.Equals(entityOrFieldName)).FirstOrDefault();
                            break;
                        }
                    }
                }
                if (lookupField != null)
                {
                    lookUpName = lookupField.LookupObject.Id;
                    linkFromAttribute = lookupField.Id;
                    linkToEntity = lookUpName;
                    linkFound = true;
                }
            }

            return linkFound;
        }

        private static ApttusField getAppFieldFromObjectHirerchy(ApttusObject currentAppObject, string fieldName)
        {
            ApttusField matchingField = null;
            if (currentAppObject.Fields.Exists(f => f.Id.Equals(fieldName) && f.LookupObject != null && !f.Id.EndsWith(Constants.APPENDLOOKUPID)))
            {
                matchingField = currentAppObject.Fields.Where(f => f.Id.Equals(fieldName) && f.LookupObject != null && !f.Id.EndsWith(Constants.APPENDLOOKUPID)).FirstOrDefault();
                return matchingField;
            }
            else
            {
                if (currentAppObject.Children != null && currentAppObject.Children.Count > 0)
                    foreach (ApttusObject childAppObject in currentAppObject.Children)
                        return getAppFieldFromObjectHirerchy(childAppObject, fieldName);
            }
            return matchingField;
        }
        private static void BuildLinkEnityCollection(ApttusObject sourceObject, ref List<LinkEntity> linkEntityCollection, ApplicationField appField)
        {
            LinkEntity currentLinkEntity = null;
            string lookUpName = string.Empty;
            string linkEntityAliasName = string.Empty;
            if (!appField.FieldId.Contains(Constants.DOT))
                return;
            List<string> entityLevel = appField.FieldId.Split('.').ToList();
            string linkFromEntity = string.Empty;
            string linkFromAttribute = string.Empty;
            string linkToEntity = string.Empty;
            string linkToAttibute = string.Empty;
            for (int count = 0; count < entityLevel.Count; count++)
            {
                bool blnFieldFound = false;
                if (count == entityLevel.Count - 1) // treat this as a actual column name                                
                {
                    leastNodeLinkEntity.Columns.AddColumn(entityLevel[count]);
                    continue;
                }

                // Set Link Entity Alias Name to match with DataTable Column Name
                if (!string.IsNullOrEmpty(linkEntityAliasName))
                    linkEntityAliasName = linkEntityAliasName + Constants.DOT + entityLevel[count];
                else
                    linkEntityAliasName = entityLevel[count];
                if (count == 0) // create link entity from root level object
                {
                    linkFromEntity = sourceObject.Id;
                    /* In case of Parent Child Relationship, Get Link of Attribure from Lookup Name,
                     * Get Lookup Field Directly from a source object to find a link entity attributes                     
                    */
                    if (sourceObject.Fields.Any(f => f.Id.Contains(entityLevel[count]) && f.LookupObject != null))
                    {

                        ApttusField field = sourceObject.Fields.FirstOrDefault(f => f.Id.Contains(entityLevel[count]) && f.LookupObject != null);
                        if (field.LookupObject != null)
                        {
                            lookUpName = field.Id;
                            linkFromAttribute = lookUpName;
                            blnFieldFound = true;
                        }
                    }
                    /* In case of Multi Lookup field iterate through each lookup object to find matching field
                     * */
                    else if (sourceObject.Fields.Any(f => f.MultiLookupObjects != null && f.LookupObject != null))
                    {
                        List<ApttusField> multiLookupFields = sourceObject.Fields.Where(f => f.LookupObject != null).ToList();
                        foreach (ApttusField field in multiLookupFields)
                        {
                            if (field.MultiLookupObjects.Count > 0 && !field.Id.EndsWith(Constants.APPENDLOOKUPID))
                            {
                                if (field.MultiLookupObjects.Any(f => f.Id.Contains(entityLevel[count])))
                                {
                                    lookUpName = field.MultiLookupObjects.FirstOrDefault(f => f.Id.Contains(entityLevel[count])).Id;
                                    linkFromAttribute = field.Id;
                                    linkToEntity = lookUpName;
                                    blnFieldFound = true;
                                    break;
                                }
                            }
                            else if (field.LookupObject != null)
                            {
                                if (field.LookupObject.Id.Contains(entityLevel[count]))
                                {
                                    lookUpName = field.LookupObject.Id;
                                    linkFromAttribute = field.Id;
                                    linkToEntity = lookUpName;
                                    blnFieldFound = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!blnFieldFound) // Set Link From Attribute only when blnFieldFound field false otherwise use assignment from above logic
                    {
                        if (!string.IsNullOrEmpty(sourceObject.LookupName))
                            linkFromAttribute = sourceObject.LookupName;
                        else
                            linkFromAttribute = entityLevel[count] + Constants.ID_ATTRIBUTE.ToLower();

                        lookUpName = linkFromAttribute;

                    }

                }
                else
                {
                    // This case will happen when there are multiple link entities exist as an example for a field like : Quote.Opportunity.Account.AccountCode
                    /* This logic is currently only applicable to handle apps create before 10 Nov 16
                     * Earlier Display field were create with following sequence
                     * ******Quote.Opportunity.Account.accountcode********
                     * and each value before "." was an object name which helps to directly find object name from ApttusObject collection
                     * 
                     * However currently this sequence is change to align with Salefroce logic and hence new field sequence is based on the lookup Field name instead of Object Name so the new sequence for all app created after 10 Nov 16 is as following
                     * ******QuoteId.OpportunityId.parentAccountId.accountCode
                     * FindLinkEntityofParentApttusObject Function is used to find exact Lookup Name, Link To Entity and Link From Attribute for all new application which contains provision for supporting old and new apps.
                     * */
                    var AppObjectCollection = applicationDefinitionManager.GetAppObjectById(entityLevel[count - 1]);
                    ApttusObject matchingAppObject = null;
                    if (AppObjectCollection != null)
                        matchingAppObject = AppObjectCollection.FirstOrDefault();


                    //Find Parent Link Entity from Parent Child Relationship or from List Object Field List
                    // This has been handeld to take care both old and new apps to get correct LinkFromEntity Value
                    linkFromEntity = FindLinkEntityofParentApttusObject(sourceObject, entityLevel[count - 1]);


                    //Find Link To Entity and Link From Attribute by earlier Object's Lookup Field
                    getLinkFromAttributeandLinkToEntity(matchingAppObject, appField, entityLevel[count], ref linkToEntity, ref linkFromAttribute, ref lookUpName);


                    // Attempt 3 - Only for List Object and case where Source Object does not contain direct reference of a lookup Field into a Field Lists
                    // This case is currently only supporting old apps created before 10 Nov 16, for all new app this case will not happen
                    if (string.IsNullOrEmpty(linkFromEntity))
                        linkFromEntity = linkToEntity;
                    if (string.IsNullOrEmpty(linkFromAttribute))
                    {
                        linkFromAttribute = entityLevel[count] + Constants.ID_ATTRIBUTE.ToLower();
                        lookUpName = linkFromAttribute;
                    }
                }

                /*
                 * For All Old apps since Field Sequence is change for some cases LinkTo Entity Does not return any value, Hence refer FindLinkEntityofParentApttusObject to Use correct link connection.
                 * */
                if (string.IsNullOrEmpty(linkToEntity))
                    linkToEntity = FindLinkEntityofParentApttusObject(sourceObject, lookUpName, linkFromEntity);

                linkToAttibute = linkToEntity + Constants.ID_ATTRIBUTE.ToLower(); // Link To Attribute is always  Object Name + Id value

                currentLinkEntity = new LinkEntity(linkFromEntity, linkToEntity, linkFromAttribute, linkToAttibute, JoinOperator.LeftOuter);
                currentLinkEntity.EntityAlias = linkEntityAliasName;
                /*
                 * Find Current Link Entity into Entire LinkEntityCollection Object, this will check Direct & Nested link entities as well to find the copy of current Link Entity
                 * */
                LinkEntity existingLinkEntity = getExistingLinkEntity(linkEntityCollection, null, currentLinkEntity);
                if (existingLinkEntity == null)
                {
                    if (leastNodeLinkEntity != null)
                    {
                        LinkEntity parentLinkEntity = null;
                        /*
                         * Associate a new link Entity with the correct chain, it's highly possible that new link entity is requried to be added into one of the nested link entites, 
                         * hence following function getExistingLinkEntity will find correct location of the link entity to be added by returning correct parent Link Entity
                         * */
                        if (currentLinkEntity.LinkFromEntityName != leastNodeLinkEntity.LinkFromEntityName)
                            parentLinkEntity = getExistingLinkEntity(linkEntityCollection, null, leastNodeLinkEntity);
                        // Find parent Link Entity of current Entity
                        if (parentLinkEntity != null)
                            parentLinkEntity.LinkEntities.Add(currentLinkEntity);
                        else
                            linkEntityCollection.Add(currentLinkEntity); // if No Link Entity Hirerchy is found add new Link Entity at root level
                    }
                    else
                        linkEntityCollection.Add(currentLinkEntity);
                }
                else
                    currentLinkEntity = existingLinkEntity;
                leastNodeLinkEntity = currentLinkEntity; // Remember the last Link Entity occurence in order to add columns at the end of this function or to find the associated child/parent entites
            }
        }


        private static void BuildLinkEnityAnnotation(ApttusObject sourceObject, ref List<LinkEntity> linkEntityCollection)
        {
            LinkEntity currentLinkEntity = new LinkEntity(sourceObject.Id, Constants.ATTACHMENTOBJECT_DYNAMICS, sourceObject.IdAttribute, "objectid", JoinOperator.LeftOuter);

            currentLinkEntity.Columns.AddColumn("filename");
            currentLinkEntity.Columns.AddColumn("annotationid");

            currentLinkEntity.EntityAlias = "annotationid";

            linkEntityCollection.Add(currentLinkEntity);
        }

        public static bool GetExpression(string actionId, List<SearchFilterGroup> FilterGroups, ApttusObject SourceObject, string[] InputDataNames, out List<KeyValuePair<string, Guid>> UsedDataSets, out QueryExpression queryExpr, bool parseUserInput = false, bool recordLimits = false, string globalFilter = "")
        {
            List<LinkEntity> linkEntityCollection = new List<LinkEntity>();
            // Create Query Expression object
            queryExpr = new QueryExpression(SourceObject.Id);

            leastNodeLinkEntity = null;
            // Change to get all Field since List layout also contains related object fields.
            //List<string> appFields = configurationManager.GetAllAppFields(SourceObject, false, true).Where(f => f.AppObject == SourceObject.UniqueId).Select(s => s.FieldId).ToList();
            List<ApplicationField> appFieldList = configurationManager.GetAllAppFields(SourceObject, false, true);
            List<string> appFields = appFieldList.Select(s => s.FieldId).ToList();

            // Remove ID field for Dynamics, as Dynamics doesn't have Id on all Entities
            appFields.Remove(Constants.ID_ATTRIBUTE);

            // Remove Lookup.Name fields as EntityReference provides name value and is take care in QueryDataSet Func()
            appFields.RemoveAll(f => f.EndsWith(Constants.APPENDLOOKUPID));

            // Remove Temporarly
            appFields.Remove(Constants.NAME_ATTRIBUTE);

            // Remove blank fields
            appFields.Remove(string.Empty);

            // Remove Attachments if present
            if (appFields.Exists(af => af.Equals(Constants.ATTACHMENT)))
            {
                BuildLinkEnityAnnotation(SourceObject, ref linkEntityCollection);
                appFields.Remove(Constants.ATTACHMENT);
            }

            // filter current object columns
            List<string> currentObjectFields = appFields.Where(f => f.Contains(Constants.DOT) == false).ToList();
            List<ApplicationField> relatedObjectAppFields = appFieldList.Where(f => !f.FieldId.EndsWith(Constants.APPENDLOOKUPID) && f.FieldId.Contains(Constants.DOT)).ToList();

            if (relatedObjectAppFields.Count > 0)
            {
                // Prepare Link Entity and Add related fields into respective link entities
                foreach (ApplicationField item in relatedObjectAppFields)
                    BuildLinkEnityCollection(SourceObject, ref linkEntityCollection, item);


            }

            UsedDataSets = new List<KeyValuePair<string, Guid>>();

            FilterExpression AndExpression = new FilterExpression();
            FilterExpression OrExpression = new FilterExpression();
            AndExpression.FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And;
            OrExpression.FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.Or;

            // Only all columns of current object
            //queryExpr.ColumnSet = new ColumnSet(appFields.ToArray());
            queryExpr.ColumnSet = new ColumnSet(currentObjectFields.ToArray());

            if (recordLimits)
                queryExpr.TopCount = 500;

            if (FilterGroups.Count == 1)
            {
                List<InputData> inputData = EBHelper.GetInputDataName(InputDataNames);
                SearchFilterGroup searchFilterGroup = FilterGroups.FirstOrDefault();

                //prepareLinkEntityCollection(SourceObject, searchFilterGroup, ref linkEntityCollection);

                string filterLogic = string.Empty;
                if (String.IsNullOrEmpty(searchFilterGroup.FilterLogicText))
                    filterLogic = EBHelper.GetDefaultFilterLogic(searchFilterGroup);
                else
                    filterLogic = searchFilterGroup.FilterLogicText;

                ConditionExpression whereClause = null;
                string lastConditionOprand = string.Empty;

                string RegExPattern = @"(and)+|(or)+|\(|\)|[0-9]+";
                bool includeInLinkEntity = false;
                SearchFilter searchFilter = null;
                ApttusField nameLookupField = null;
                LinkEntity currentLinkEntity = null;
                bool isValidCriteria = false;
                foreach (Match matchFilterLogicChar in Regex.Matches(filterLogic, RegExPattern, RegexOptions.IgnoreCase))
                {
                    int sequenceNo;
                    if (int.TryParse(matchFilterLogicChar.Value, out sequenceNo))
                    {
                        searchFilter = searchFilterGroup.Filters.FirstOrDefault(f => f.SequenceNo == sequenceNo);
                        // Create Link entity and Filter Expression under that                
                        if (searchFilter.QueryObjects.Count > 1)
                            includeInLinkEntity = true;
                        if (searchFilter.FieldId.Contains(Constants.APPENDLOOKUPID))
                        {
                            if (SourceObject.UniqueId == searchFilter.AppObjectUniqueId)
                                nameLookupField = SourceObject.Fields.FirstOrDefault(field => field.Id == searchFilter.FieldId);
                            else
                            {
                                ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(searchFilter.QueryObjects.Last().AppObjectUniqueId);
                                nameLookupField = apttusObject.Fields.FirstOrDefault(field => field.Id == searchFilter.FieldId);
                            }
                            string linkFrom = string.Empty;
                            linkFrom = nameLookupField.Id.Split('.')[0];
                            if (nameLookupField.MultiLookupObjects == null || nameLookupField.MultiLookupObjects.Count <= 0)
                            {
                                LinkEntity linkEntity = new LinkEntity(SourceObject.Id, nameLookupField.LookupObject.Id, linkFrom, nameLookupField.LookupObject.IdAttribute, JoinOperator.Inner);

                                LinkEntity existingLinkEntity = getExistingLinkEntity(linkEntityCollection, null, linkEntity);
                                if (existingLinkEntity != null)
                                    linkEntity = existingLinkEntity;
                                else
                                    linkEntityCollection.Add(linkEntity);

                                currentLinkEntity = linkEntity;
                            }
                        }

                        // If filter is of type User Input and Evaluate flag is flase continue.
                        if (searchFilter.ValueType == ExpressionValueTypes.UserInput && !parseUserInput)
                        {
                            // In case when we use Userinput in S&S action OR Query Action with Advance filters than no need skip loop                            
                            if (!searchFilterGroup.IsAddFilter)
                                continue;
                        }

                        whereClause = GetSingleWhereClause(actionId, searchFilter, SourceObject, inputData, ref UsedDataSets, false, searchFilterGroup);

                        if (nameLookupField != null && whereClause != null)
                        {
                            if (nameLookupField.MultiLookupObjects != null && nameLookupField.MultiLookupObjects.Count > 0)
                            {
                                //modify current where clause
                                whereClause = ChangeConditionExpressionforMultiObject(nameLookupField, whereClause);
                            }
                        }
                        else if (whereClause == null)
                            currentLinkEntity = null;

                        // In case whereClause is Blank for UserInput type of filter, skip to the next expression - User did not specify any value.
                        if (object.ReferenceEquals(whereClause, null) && (searchFilter.ValueType == ExpressionValueTypes.UserInput || searchFilter.ValueType == ExpressionValueTypes.CellReference))
                        {
                            isValidCriteria = true;//Anil's Change by Maulik
                            continue;
                        }
                        else if (object.ReferenceEquals(whereClause, null))
                        {
                            //return false;
                        }
                        //else
                        //    Expression.AddCondition(whereClause);
                    }
                    else if (!object.ReferenceEquals(whereClause, null) && matchFilterLogicChar.Value.ToUpper() == Constants.AND) // AND Clause
                    {
                        if (includeInLinkEntity)
                        {
                            BuildLinkEntityCollection(searchFilter, ref linkEntityCollection, whereClause, appFields, lastConditionOprand);
                            includeInLinkEntity = false;
                        }
                        else if (searchFilter.FieldId.EndsWith(Constants.APPENDLOOKUPID) && currentLinkEntity != null)
                        {
                            if (nameLookupField.MultiLookupObjects == null || nameLookupField.MultiLookupObjects.Count <= 0)
                            {
                                currentLinkEntity.LinkCriteria.AddCondition(whereClause);
                                //linkEntityCollection.Add(currentLinkEntity);
                            }

                            includeInLinkEntity = false;
                            currentLinkEntity = null;
                        }
                        else
                        {
                            AndExpression.AddCondition(whereClause);
                        }
                        lastConditionOprand = Constants.AND;
                        whereClause = null;
                        isValidCriteria = true;
                    }
                    else if (!object.ReferenceEquals(whereClause, null) && matchFilterLogicChar.Value.ToUpper() == Constants.OR) // OR Clause
                    {
                        if (includeInLinkEntity)
                        {
                            BuildLinkEntityCollection(searchFilter, ref linkEntityCollection, whereClause, appFields, lastConditionOprand);
                            includeInLinkEntity = false;
                        }
                        else if (searchFilter.FieldId.EndsWith(Constants.APPENDLOOKUPID) && currentLinkEntity != null)
                        {
                            if (nameLookupField.MultiLookupObjects == null || nameLookupField.MultiLookupObjects.Count <= 0)
                            {
                                currentLinkEntity.LinkCriteria.AddCondition(whereClause);
                                //linkEntityCollection.Add(currentLinkEntity);
                            }

                            includeInLinkEntity = false;
                            currentLinkEntity = null;
                        }
                        else
                        {
                            OrExpression.AddCondition(whereClause);
                        }
                        lastConditionOprand = Constants.OR;
                        whereClause = null;
                        isValidCriteria = true;
                    }
                    else if (object.ReferenceEquals(whereClause, null) && (matchFilterLogicChar.Value.ToUpper() == Constants.OR || matchFilterLogicChar.Value.ToUpper() == Constants.AND))
                    {
                        lastConditionOprand = matchFilterLogicChar.Value.ToUpper() == Constants.OR ? Constants.OR : Constants.AND;
                    }
                    else if (matchFilterLogicChar.Value == Constants.CLOSE_BRACKET && whereClause != null)
                    {
                        if (includeInLinkEntity)
                            BuildLinkEntityCollection(searchFilter, ref linkEntityCollection, whereClause, appFields, lastConditionOprand, true);

                        else if (searchFilter.FieldId.EndsWith(Constants.APPENDLOOKUPID) && currentLinkEntity != null)
                        {
                            if (nameLookupField.MultiLookupObjects == null || nameLookupField.MultiLookupObjects.Count <= 0)
                            {
                                currentLinkEntity.LinkCriteria.AddCondition(whereClause);
                                //linkEntityCollection.Add(currentLinkEntity);
                            }

                            includeInLinkEntity = false;
                            currentLinkEntity = null;
                        }
                        else
                        {
                            if (lastConditionOprand.Equals(Constants.AND))
                                AndExpression.AddCondition(whereClause);
                            else if (lastConditionOprand.Equals(Constants.OR))
                                OrExpression.AddCondition(whereClause);
                            else if (searchFilterGroup.Filters.Count == 1)
                                AndExpression.AddCondition(whereClause);
                        }
                        whereClause = null;
                        isValidCriteria = true;
                    }
                }

                if (isValidCriteria == false && !string.IsNullOrEmpty(filterLogic) && string.IsNullOrEmpty(globalFilter))
                    return false;

                // Apply global Filter to All Searchable Fields with OR Condition expression
                if (!string.IsNullOrEmpty(globalFilter))
                {
                    FilterExpression globalFilterExp = getGlobalFilterExpression(globalFilter, searchFilterGroup, SourceObject);
                    if (globalFilterExp != null)
                        AndExpression.AddFilter(globalFilterExp);
                }

                // Add Filter criterias to Query Expression
                if (AndExpression.Conditions.Count > 0 || AndExpression.Filters.Count > 0)
                    queryExpr.Criteria.Filters.Add(AndExpression);
                if (OrExpression.Conditions.Count > 0 || OrExpression.Filters.Count > 0)
                    queryExpr.Criteria.Filters.Add(OrExpression);

            }

            // set displayableWhereClause - so show filter functionality can display where clause in sheet.
            displayableWhereClause = GetFilterLogicClause(queryExpr.Criteria.Filters);

            // if there are no Filters, but relational attributes are added, create linkEntityCollection which should be added in QueryExpression
            foreach (LinkEntity item in linkEntityCollection)
                queryExpr.LinkEntities.Add(item);


            return true;
        }

        /// <summary>
        /// used to create displayable where clause from Dynamics query structure.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static string GetFilterLogicClause(DataCollection<FilterExpression> filters)
        {
            StringBuilder filterlogic = new StringBuilder();
            try
            {
                foreach (var x in filters)
                {
                    filterlogic.Append("(");
                    foreach (var con in x.Conditions)
                    {
                        string str = string.Format("{0} {1} {2}", con.AttributeName, con.Operator, string.Join(",", con.Values));
                        filterlogic.Append(str);
                        if (x.Conditions.IndexOf(con) != x.Conditions.Count - 1)
                        {
                            filterlogic.Append(string.Format(" {0} ", x.FilterOperator));
                        }
                    }
                    filterlogic.Append(")");
                    if (filters.IndexOf(x) != filters.Count - 1)
                    {
                        filterlogic.Append(string.Format(" {0} ", filters[filters.IndexOf(x) + 1].FilterOperator));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

            return filterlogic.ToString();
        }
        private static ConditionExpression ChangeConditionExpressionforMultiObject(ApttusField field, ConditionExpression currentCondExpression)
        {
            List<ApttusDataSet> dataSet = new List<ApttusDataSet>();
            foreach (ApttusObject apttusObject in field.MultiLookupObjects)
            {
                QueryExpression query = new QueryExpression(apttusObject.Id);

                /*  AB - 2657 - MSD: User input with IN filter in query action throws error. 
                 * &&&
                 * AB - 2658 - MSD: Using 'Not equal to Blank' filter on multilookup field  throws error. 
                */
                if (currentCondExpression.Operator == ConditionOperator.In || currentCondExpression.Operator == ConditionOperator.NotIn)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.Operator, currentCondExpression.Values.ToArray());
                else if (currentCondExpression.Operator == ConditionOperator.Null || currentCondExpression.Operator == ConditionOperator.NotNull)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.Operator);
                else if (currentCondExpression.Values != null && currentCondExpression.Values.Count == 1)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.Operator, currentCondExpression.Values[0]);

                dataSet.Add(ObjectManager.GetInstance.QueryDataSet(new DynamicsQuery { Query = query, DataTable = null, Object = null, UserInfo = null }));
            }
            List<Guid> recordIds = new List<Guid>();
            foreach (ApttusDataSet item in dataSet)
            {
                if (item.DataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in item.DataTable.Rows)
                    {
                        Guid id = Guid.Empty;
                        if (Guid.TryParse(row[0].ToString(), out id))
                            recordIds.Add(id);
                    }
                }
            }
            if (recordIds.Count > 0)
                currentCondExpression = new ConditionExpression(field.Id.Split('.')[0], ConditionOperator.In, recordIds.ToArray());
            return currentCondExpression;
        }

        private static void BuildLinkEntityCollection(SearchFilter searchFilters, ref List<LinkEntity> linkEntityCollection, ConditionExpression filterConditionExression, List<string> appFields, string lastCondOperator, bool bCloseExpression = false)
        {
            LinkEntity lastLinkEntity = null;

            // Set Link Entity Join Operator based on the Filter Operator, this is import part since which will decide to make link entity relation with Left Outer or with Inner Join
            Microsoft.Xrm.Sdk.Query.LogicalOperator crmCondOperator = (lastCondOperator == Constants.OR ? Microsoft.Xrm.Sdk.Query.LogicalOperator.Or : Microsoft.Xrm.Sdk.Query.LogicalOperator.And);

            /*
             * Make New Link Entity if required based on Query Objects mention inside Search Filter, this is an other way of creating Link Entities and Adding Filter into that, however this will refer to Same LinkEntityCollection Object 
             * Which at the end will make entire link Expression
             * */
            for (int level = 0; level < searchFilters.QueryObjects.Count; level++)
            {
                if (string.IsNullOrEmpty(searchFilters.QueryObjects[level].LookupFieldId) && level == searchFilters.QueryObjects.Count - 1)
                {
                    FilterExpression linkExpression = null;

                    if (lastLinkEntity.LinkCriteria.Filters.Count == 0)
                    {
                        lastLinkEntity.JoinOperator = lastCondOperator == Constants.OR ? JoinOperator.LeftOuter : JoinOperator.Inner;
                        linkExpression = new FilterExpression();
                        lastLinkEntity.LinkCriteria.AddFilter(linkExpression);
                    }
                    else
                    {
                        if (lastLinkFilterExpression != null && lastLinkEntity.LinkCriteria.Filters.Count == 1)
                            linkExpression = lastLinkEntity.LinkCriteria.Filters[0];
                        else
                        {
                            if (lastLinkEntity.LinkCriteria.Filters.Any(f => f.FilterOperator == crmCondOperator))
                                linkExpression = lastLinkEntity.LinkCriteria.Filters.LastOrDefault(f => f.FilterOperator == crmCondOperator);
                            else
                            {
                                linkExpression = new FilterExpression(crmCondOperator);
                                lastLinkEntity.LinkCriteria.AddFilter(linkExpression);
                            }
                        }
                    }

                    if (linkExpression.Conditions.Count > 0)
                    {
                        if (linkExpression.Conditions.Count == 1)
                            linkExpression.FilterOperator = crmCondOperator;
                        linkExpression.Conditions.Add(filterConditionExression);
                    }
                    else
                        linkExpression.Conditions.Add(filterConditionExression);


                    if (bCloseExpression)
                        lastLinkFilterExpression = null;
                    else
                        lastLinkFilterExpression = linkExpression;

                    continue;

                }

                /*
                 * Get Link From, Link To Entity and Link From & Link To Attribute Directly from the Query Objects
                 * */
                // Get Primary Link Entity Object from where Link Start, that will helps to generate LinkFrom Entity and Link From Attribute
                ApttusObject primanyApttusObject = applicationDefinitionManager.GetAppObject(searchFilters.QueryObjects[level].LeafAppObjectUniqueId);
                // Get Secondary Link Entity Object from where Link End, that will helps to genereate LinkTo Entity and Link To Attribute
                Guid secondaryObjectId = searchFilters.QueryObjects.Where(objectid => objectid.LeafAppObjectUniqueId == searchFilters.QueryObjects[level].AppObjectUniqueId).Select(objectid => objectid.LeafAppObjectUniqueId).FirstOrDefault();
                ApttusObject secondaryApttusObject = applicationDefinitionManager.GetAppObject(secondaryObjectId);
                string linkFromAttribute = searchFilters.QueryObjects[level].LookupFieldId;
                string linkToAttribute = secondaryApttusObject.IdAttribute;
                string linkFromEntity = primanyApttusObject.Id;
                string linkToEntity = secondaryApttusObject.Id;

                // Use Common Function getExistingLinkEntity to find associated matching link entity into Entire LinkEntityCollection
                LinkEntity existingLinkEntity = getExistingLinkEntity(linkEntityCollection, searchFilters.QueryObjects[level]);
                lastLinkEntity = existingLinkEntity;

                if (existingLinkEntity == null)
                {
                    // Initiate new Link Entity Object
                    lastLinkEntity = new LinkEntity { LinkFromEntityName = linkFromEntity, LinkToEntityName = linkToEntity, LinkFromAttributeName = linkFromAttribute, LinkToAttributeName = linkToAttribute };

                    // get All Link Entity Columns from All Application Fields
                    List<string> linkEntityColumns = getAppFieldsbyObject(lastLinkEntity.LinkToEntityName, appFields);
                    if (linkEntityColumns.Count > 0)
                        lastLinkEntity.Columns = new ColumnSet(linkEntityColumns.ToArray()); // Add All App fields Columns to Link Entities

                    if (level > 0) // When Link Starts with Level 0 it means it should be a Root Level Link Entity which is directly going to associated with Query Expression on Source Object
                    {
                        // get Immediate parent relationship
                        LinkEntity parentLinkEntity = getExistingLinkEntity(linkEntityCollection, searchFilters.QueryObjects[level - 1]);
                        if (parentLinkEntity != null)
                            parentLinkEntity.LinkEntities.Add(lastLinkEntity);
                        else
                            // if no parent link Entity found create new link Entity which will add this link entity onto Root Level Link Entity Collection                   
                            linkEntityCollection.Add(lastLinkEntity);
                    }
                    else
                        linkEntityCollection.Add(lastLinkEntity); // if no parent link Entity found create new link Entity which will add this link entity onto Root Level Link Entity Collection                   
                }
            }
        }

        private static LinkEntity getChildMostLinkEntity(LinkEntity parentLinkEntity)
        {
            if (parentLinkEntity.LinkEntities != null && parentLinkEntity.LinkEntities.Count > 0)
            {
                return getChildMostLinkEntity(parentLinkEntity.LinkEntities[0]);
            }
            else
            {
                return parentLinkEntity;
            }
        }

        private static void prepareLinkEntityCollection(ApttusObject sourceObject, SearchFilterGroup searchFilterGroup, ref List<LinkEntity> linkEntityCollection)
        {

            foreach (SearchFilter item in searchFilterGroup.Filters)
            {
                if (item.QueryObjects.Count <= 1)
                    continue;
                for (int count = 0; count < item.QueryObjects.Count; count++)
                {
                    if (string.IsNullOrEmpty(item.QueryObjects[count].LookupFieldId))
                        continue;
                    ApttusObject primanyApttusObject = applicationDefinitionManager.GetAppObject(item.QueryObjects[count].LeafAppObjectUniqueId);
                    Guid secondaryObjectId = item.QueryObjects.Where(objectid => objectid.LeafAppObjectUniqueId == item.QueryObjects[count].AppObjectUniqueId).Select(objectid => objectid.LeafAppObjectUniqueId).FirstOrDefault();
                    ApttusObject secondaryApttusObject = applicationDefinitionManager.GetAppObject(secondaryObjectId);
                    string linkFromAttribute = item.QueryObjects[count].LookupFieldId;
                    string linkToAttribute = secondaryApttusObject.IdAttribute;
                    string linkFromEntity = primanyApttusObject.Id;
                    string linkToEntity = secondaryApttusObject.Id;

                    LinkEntity existingLinkEntity = getExistingLinkEntity(linkEntityCollection, item.QueryObjects[count]);
                    if (existingLinkEntity == null)
                    {
                        if (count > 0)
                        {
                            // get Immediate parent relationship
                            LinkEntity parentLinkEntity = getExistingLinkEntity(linkEntityCollection, item.QueryObjects[count - 1]);
                            if (parentLinkEntity != null)
                            {
                                parentLinkEntity.LinkEntities.Add(new LinkEntity { LinkFromEntityName = linkFromEntity, LinkToEntityName = linkToEntity, LinkFromAttributeName = linkFromAttribute, LinkToAttributeName = linkToAttribute });
                            }
                            else
                            {
                                // if no parent link Entity found create new link Entity
                                linkEntityCollection.Add(new LinkEntity { LinkFromEntityName = linkFromEntity, LinkToEntityName = linkToEntity, LinkFromAttributeName = linkFromAttribute, LinkToAttributeName = linkToAttribute });
                            }
                        }
                        else
                        {
                            linkEntityCollection.Add(new LinkEntity { LinkFromEntityName = linkFromEntity, LinkToEntityName = linkToEntity, LinkFromAttributeName = linkFromAttribute, LinkToAttributeName = linkToAttribute });
                        }
                    }
                }
            }
        }

        private static LinkEntity getExistingLinkEntity(List<LinkEntity> linkEntityCollection, QueryObject parentQueryObjects, LinkEntity searchLinkEntity = null)
        {
            if (searchLinkEntity == null)
            {
                ApttusObject primanyApttusObject = applicationDefinitionManager.GetAppObject(parentQueryObjects.LeafAppObjectUniqueId);
                Guid secondaryObjectId = parentQueryObjects.AppObjectUniqueId;
                ApttusObject secondaryApttusObject = applicationDefinitionManager.GetAppObject(secondaryObjectId);
                string linkFromAttribute = parentQueryObjects.LookupFieldId;
                string linkToAttribute = secondaryApttusObject.IdAttribute;
                string linkFromEntity = primanyApttusObject.Id;
                string linkToEntity = secondaryApttusObject.Id;
                searchLinkEntity = new LinkEntity(linkFromEntity, linkToEntity, linkFromAttribute, linkToAttribute, JoinOperator.Inner);
            }

            foreach (LinkEntity item in linkEntityCollection)
            {
                if (item.LinkFromEntityName == searchLinkEntity.LinkFromEntityName && item.LinkToEntityName == searchLinkEntity.LinkToEntityName && item.LinkFromAttributeName == searchLinkEntity.LinkFromAttributeName && item.LinkToAttributeName == searchLinkEntity.LinkToAttributeName)
                    return item;
            }

            foreach (LinkEntity item in linkEntityCollection)
            {
                if (item.LinkEntities.Count > 0)
                    return getExistingLinkEntity(item.LinkEntities.ToList(), parentQueryObjects, searchLinkEntity);
            }

            return null;
        }

        private static void upateLinkEntityCollection(ref List<LinkEntity> linkEntityCollection, ConditionExpression condExpression, SearchFilter searchFilters)
        {

            Guid leafObjectId = searchFilters.QueryObjects.Last().AppObjectUniqueId;
            QueryObject parentQueryRelation = searchFilters.QueryObjects.FirstOrDefault(field => field.AppObjectUniqueId == leafObjectId);
            LinkEntity matchLinkEntity = getExistingLinkEntity(linkEntityCollection, parentQueryRelation);
            matchLinkEntity.LinkCriteria.AddCondition(condExpression);
        }

        private static List<string> getAppFieldsbyObject(string objectName, List<string> appFields)
        {
            List<string> linkEntityColumns = new List<string>();
            if (appFields.Exists(f => f.StartsWith(objectName + Constants.DOT)))
                linkEntityColumns = appFields.Where(f => f.StartsWith(objectName + Constants.DOT)).ToList();

            //Remove Object Prefix
            linkEntityColumns = linkEntityColumns.Select(f => f.Replace((objectName + Constants.DOT), "")).ToList();
            return linkEntityColumns;
        }

        private static FilterExpression getGlobalFilterExpression(string globalFilter, SearchFilterGroup FilterGroups, ApttusObject sourceObject)
        {
            FilterExpression globalFilterExpression = null;


            globalFilterExpression = new FilterExpression(Microsoft.Xrm.Sdk.Query.LogicalOperator.Or);

            List<SearchFilter> filterList = FilterGroups.Filters.OrderBy(z => z.SequenceNo).ToList();

            foreach (SearchFilter item in filterList)
            {
                ApttusField apttusFields = sourceObject.Fields.FirstOrDefault(f => f.Id == item.FieldId);

                if (apttusFields.Datatype != Datatype.Lookup)
                {
                    if (apttusFields.LookupObject == null)
                    {
                        ConditionExpression expression = GetConditionExpression(apttusFields, "like %#FILTERVALUE%", apttusFields.Datatype, globalFilter);
                        //ConditionExpression expression = GetLikeConditionExpressionBasedOnDataType(globalFilter, apttusFields);
                        if (expression != null)
                            globalFilterExpression.AddCondition(expression);
                    }
                }
            }


            return globalFilterExpression;

        }

        public static ConditionExpression GetLikeConditionExpressionBasedOnDataType(string searchCriteria, ApttusField apttusField)
        {
            string LHSPlusOperator = string.Empty, value = string.Empty, whereClause = string.Empty;
            ConditionExpression expression = null;
            GetLikeConditionExpressionBasedOnDataType(searchCriteria, apttusField.Id, apttusField.Datatype, ref LHSPlusOperator, ref value, ref expression);
            return expression;
        }
        public static void GetLikeConditionExpressionBasedOnDataType(string searchCriteria, string LHSField, Datatype datatype, ref string LHSPlusOperator, ref string value, ref ConditionExpression conditionExpression)
        {
            switch (datatype)
            {
                case Datatype.Date:
                case Datatype.DateTime:
                    string validDate = Utils.IsValidDate(searchCriteria, datatype);
                    if (!string.IsNullOrEmpty(validDate))
                    {
                        conditionExpression = new ConditionExpression(LHSField, ConditionOperator.Equal, Convert.ToDateTime(validDate));
                        LHSPlusOperator = LHSField + " = ?";
                        value = validDate;
                    }
                    break;
                case Datatype.Decimal:
                case Datatype.Double:
                    Decimal validNumber;
                    if (Decimal.TryParse(searchCriteria, out validNumber))
                    {
                        conditionExpression = new ConditionExpression(LHSField, ConditionOperator.Equal, validNumber);
                        LHSPlusOperator = LHSField + " = ?";
                        value = validNumber.ToString();
                    }
                    break;
                case Datatype.String:
                    conditionExpression = new ConditionExpression(LHSField, ConditionOperator.Like, "%" + searchCriteria + "%");
                    value = searchCriteria;
                    break;
                case Datatype.Picklist:
                    if (!string.IsNullOrEmpty(searchCriteria))
                    {
                        conditionExpression = new ConditionExpression(LHSField, ConditionOperator.Equal, searchCriteria);
                        value = searchCriteria;
                    }
                    break;
                // Lookup Id field need to be exact match.
                case Datatype.Lookup:
                    Guid id = Guid.Empty;
                    Guid.TryParse(searchCriteria, out id);
                    if (id != Guid.Empty)
                    {
                        conditionExpression = new ConditionExpression(LHSField, ConditionOperator.Equal, id);
                        value = searchCriteria;
                    }
                    break;
                // Added case for Boolean as it was missing after logic change, it compares without quotes
                //case Datatype.Boolean:
                //    bool isBoolean;
                //    if (Boolean.TryParse(searchCriteria, out isBoolean))
                //    {
                //        whereClause = LHSField + " = " + searchCriteria + " ";
                //        LHSPlusOperator = LHSField + " = ?";
                //        value = searchCriteria;
                //    }
                //    break;
                case Datatype.NotSupported:
                case Datatype.Textarea:
                    break;
                default:
                    break;
            }
        }

        private static ConditionExpression GetSingleWhereClause(string actionId, SearchFilter searchFilter, ApttusObject SourceObject, List<InputData> inputData, ref List<KeyValuePair<string, Guid>> UsedDataSets, bool Designer, SearchFilterGroup searchFilterGroup)
        {
            string filterResult = string.Empty;
            bool isValidWhereClause = true;
            ConditionExpression conditionExpr = null;
            ApttusObject apttusObject = null;
            string LHSObjectField = string.Empty;
            string notOperator = string.Empty;
            List<string> chunkRecordIds = new List<string>();
            // Set the Apttus Object
            Guid appObjectUId = searchFilter.QueryObjects.Count > 0 ? searchFilter.QueryObjects.Last().AppObjectUniqueId : searchFilter.AppObjectUniqueId;
            apttusObject = applicationDefinitionManager.GetAppObject(appObjectUId);

            // Get the N level left hand side query
            LHSObjectField = EBHelper.GetLHSQueryTextFromQueryObjects(searchFilter);

            // Get data type of the field
            ApttusField currentApttusField = apttusObject.Fields.FirstOrDefault(f => f.Id == searchFilter.FieldId);
            //Datatype dataType = apttusObject.Fields.FirstOrDefault(f => f.Id == searchFilter.FieldId).Datatype;
            Datatype dataType = currentApttusField.Datatype;

            // id Attribute is defined as String DataType however it has to be treated as Lookup/Guid field for conditional expression
            if (SourceObject.IdAttribute == currentApttusField.Id)
                dataType = Datatype.Lookup;

            // Operator
            string Operator = searchFilter.Operator;

            // When filter condition is for .Name field use virtual ApttusField instead which link to correct Entity attribute
            ApttusField apttusField = new ApttusField();
            if (currentApttusField.Id.EndsWith(Constants.APPENDLOOKUPID))
            {
                string nameAttribute = currentApttusField.LookupObject.NameAttribute;
                apttusField.Id = nameAttribute;
            }
            else
                apttusField = currentApttusField;

            // TODO:: filterResult needs to be converted based on dataType for Dynamics, Current EBHelper methods will return it based on SFDC standards.
            bool invalidDataType = false;
            if (searchFilter.ValueType == ExpressionValueTypes.Static)
            {
                // Escape and Get Filter Value
                filterResult = EBHelper.EscapeAndGetFilterValue(searchFilter.Value, Operator, dataType, true, ValueSeparator.MultiLine, true, out invalidDataType);
                conditionExpr = GetConditionExpression(apttusField, searchFilter.Operator, dataType, filterResult, true);

            }
            else if (searchFilter.ValueType == ExpressionValueTypes.SystemVariables)
            {
                filterResult = EBHelper.getSysVariableFilterValue(searchFilter, dataType);
                conditionExpr = GetConditionExpression(apttusField, searchFilter.Operator, dataType, filterResult);
                //conditionExpr = new ConditionExpression(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.Input)
            {
                isValidWhereClause = getInputFilterValue(searchFilter, SourceObject, inputData, ref UsedDataSets, out filterResult, Designer);

                conditionExpr = GetConditionExpression(apttusField, searchFilter.Operator, dataType, filterResult);
                //conditionExpr = new ConditionExpression(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.UserInput)
            {
                DataManager dataManager = DataManager.GetInstance;
                List<ApttusDataSet> dataSets = dataManager.GetDatasetsByNames(new string[] { Constants.USERINPUT_DATASETNAME });
                if (dataSets.Count <= 0)
                    isValidWhereClause = false;
                else if (dataSets.Count > 0)
                {
                    ApttusDataSet dataSet = dataSets.ElementAt(0);
                    string key = new StringBuilder(actionId).Append(Constants.DOT).Append(searchFilter.SequenceNo).Append(Constants.DOT).Append(searchFilter.FieldId).ToString();
                    DataRow dr = dataSet.DataTable.Rows.Find(key);
                    if (dr == null)
                        isValidWhereClause = false;
                    else
                    {
                        // Escape and Get Filter Value
                        filterResult = EBHelper.EscapeAndGetFilterValue(Convert.ToString(dr[Constants.VALUE_COLUMN]), Operator, dataType, true, ValueSeparator.MultiLine, false, out invalidDataType);

                        if (invalidDataType || string.IsNullOrEmpty(filterResult))
                            isValidWhereClause = false;
                        else // for dynamics create conditionExpr
                            conditionExpr = GetConditionExpression(apttusField, Operator, dataType, filterResult);
                        //conditionExpr = new ConditionExpression(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
                    }
                }
                // when we use Userinput in S&S action OR Query Action with Advance filters than get default values and pass in query
                if (searchFilterGroup.IsAddFilter && !isValidWhereClause && string.IsNullOrEmpty(filterResult))
                {
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);

                    conditionExpr = new ConditionExpression(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
                    isValidWhereClause = true;
                }
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.CellReference)
            {
                // Escape and Get Filter Value                
                filterResult = EBHelper.EscapeAndGetFilterValue(searchFilter.Value, Operator, dataType, true, ValueSeparator.MultiLine, true, out invalidDataType, cellReferenceValueType: Constants.CELLREFERENCE);

                if (invalidDataType)
                    isValidWhereClause = false;
                if (string.IsNullOrEmpty(filterResult))
                {
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
                }
                // Create Condition Expression anyways , since dynamics needs it for filtering data
                //conditionExpr = new ConditionExpression(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
                conditionExpr = GetConditionExpression(apttusField, Operator, dataType, filterResult);
            }

            notOperator = Operator;
            // Only in case of "Starts With" , "Contains" and "not contains", the operator has to be set to "Like". In all other cases, it will be Operator
            if (Operator.Equals("like #FILTERVALUE%") || Operator.Equals("like %#FILTERVALUE%") || Operator.Equals("like %#NOTFILTERVALUE%"))
                Operator = "LIKE";
            else if (Operator.Equals("includes ('#FILTERVALUE')"))
                Operator = "includes";
            else if (Operator.Equals("excludes ('#FILTERVALUE')"))
                Operator = "excludes";

            if (isValidWhereClause)
            {
                // for Dynamics where clause field is appended in conditionExpr, so no need to append it in result.

                //values.Add(filterResult);
                // Append "Not" for not contains clause
                //if (notOperator.Equals("like %#NOTFILTERVALUE%"))
                //    result = Constants.OPEN_BRACKET + "Not" + Constants.SPACE + LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + "?" + Constants.CLOSE_BRACKET;
                //else
                //    result = LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + "?";
            }
            else
                conditionExpr = null;

            return conditionExpr;
        }

        /// <summary>
        /// Resolve input filters
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="inputDataNames">Input names</param>
        /// <param name="usedDataSets">Used datasets</param>
        /// <returns>Filter value</returns>
        public static bool getInputFilterValue(SearchFilter filter, ApttusObject SourceObject, List<InputData> inputData, ref List<KeyValuePair<string, Guid>> usedDataSets, out string inputFilterValue, bool Designer = false)
        {
            DataManager dataManager = DataManager.GetInstance;
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

            inputFilterValue = string.Empty;
            bool result = true;
            string[] split = filter.Value.Split(Constants.DOT.ToCharArray());
            Guid inputObjectGuid = Guid.Empty;

            inputObjectGuid = Guid.Parse(split[0]);

            ApttusObject inputObject = appDefManager.GetAppObject(inputObjectGuid);
            string inputField = (filter.ValueType == ExpressionValueTypes.Static ||
                                    filter.ValueType == ExpressionValueTypes.SystemVariables) ? filter.FieldId : split[1];

            ApttusDataSet dataSet = null;
            string inputNme = null;
            if (Designer)
            {
                inputNme = EBHelper.GetInputDataName(inputData.Select(id => id.Name).ToArray(), inputObject);
            }
            else
            {
                if (inputData != null)
                    dataSet = dataManager.ResolveInput(inputData, inputObject);

                if (dataSet == null || dataSet.DataTable.Rows.Count == 0)
                    dataSet = dataManager.ResolveInput(inputData, SourceObject);


                if (dataSet == null || dataSet.DataTable.Rows.Count == 0)
                {
                    // If there is no data is passed for Input dataset, insert a FALSE condition like
                    // "Id = NULL" or "Id in (NULL)"
                    // Suppress Message as per user's app settings
                    if (configurationManager.Definition.AppSettings != null)
                    {
                        if (!configurationManager.Definition.AppSettings.SuppressDependent)
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("COREEXPREBUILDHELPER_InputFilt_ErrorMsg"), SourceObject.Name, inputObject.Name), resourceManager.GetResource("COREEXPREBUILDHELPER_InputFiltCap_ErrorMsg"));
                    }
                    else
                        ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("COREEXPREBUILDHELPER_InputFilt_ErrorMsg"), SourceObject.Name, inputObject.Name), resourceManager.GetResource("COREEXPREBUILDHELPER_InputFiltCap_ErrorMsg"));

                    inputFilterValue = filter.Operator.ToUpper() == "IN" ? "(NULL)" : "NULL";
                    result = true;
                    return result;
                }
            }

            StringBuilder ContainsExpression = new StringBuilder();
            if (filter.Operator.ToUpper() == "IN" || filter.Operator.ToUpper() == "NOT IN")
            {
                if (Designer) //If in designer do not want to resolve the inputFilterValue. Queries and datasets are not guaranteed during designer. 
                {
                    return result;
                }
                ContainsExpression.Append(Constants.OPEN_BRACKET);
                HashSet<string> uniqueValues = new HashSet<string>();

                foreach (DataRow dr in dataSet.DataTable.Rows)
                {
                    uniqueValues.Add(Constants.QUOTE + dr[inputField] + Constants.QUOTE);
                }
                ContainsExpression.Append(string.Join(Constants.COMMA, uniqueValues));

                ContainsExpression.Append(Constants.CLOSE_BRACKET);
                inputFilterValue = ContainsExpression.ToString();
            }
            else
            {
                if (string.IsNullOrEmpty(inputNme))
                {
                    usedDataSets.Add(new KeyValuePair<string, Guid>(dataSet.Name, dataSet.Id));
                    ContainsExpression.Append(dataSet.DataTable.Rows[0][inputField].ToString());
                }
                else
                {
                    ContainsExpression.Append(inputNme + "." + inputField.ToString());
                }

                inputFilterValue = ContainsExpression.ToString();
            }

            return result;
        }

        /// <summary>
        /// TODO:: may be GetconditionOperator needs to be enhanced to return appropriate value based on Operator, verify on Runtime
        /// </summary>
        /// <param name="filterOperator"></param>
        /// 
        private static ConditionOperator GetConditionOperator(string filterOperator)
        {
            ConditionOperator condOpr = ConditionOperator.Null;

            switch (filterOperator)
            {
                case "=":
                    condOpr = ConditionOperator.Equal;
                    break;
                case "!=":
                    condOpr = ConditionOperator.NotEqual;
                    break;
                case "<":
                    condOpr = ConditionOperator.LessThan;
                    break;
                case "<=":
                    condOpr = ConditionOperator.LessEqual;
                    break;
                case ">":
                    condOpr = ConditionOperator.GreaterThan;
                    break;
                case ">=":
                    condOpr = ConditionOperator.GreaterEqual;
                    break;
                case "in":
                case "includes ('#FILTERVALUE')":
                    condOpr = ConditionOperator.In;
                    break;
                case "not in":
                case "excludes ('#FILTERVALUE')":
                    condOpr = ConditionOperator.NotIn;
                    break;
                case "like #FILTERVALUE%":
                case "like %#FILTERVALUE%":
                    condOpr = ConditionOperator.Like;
                    break;
                case "like %#NOTFILTERVALUE%":
                    condOpr = ConditionOperator.NotLike;
                    break;
                case "on":
                    condOpr = ConditionOperator.On;
                    break;
                default:
                    condOpr = ConditionOperator.Null;
                    break;
            }

            return condOpr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FieldId"></param>
        /// <param name="Operator"></param>
        /// <param name="FilterResult"></param>
        /// <returns></returns>
        public static ConditionExpression GetConditionExpression(string fieldId, string filterOperator, Datatype dataType, string filterResult)
        {
            ConditionExpression result = null;
            ConditionOperator condOpr = GetConditionOperator(filterOperator);



            switch (dataType)
            {
                case Datatype.String:
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        string[] inValues = filterResult.Split(',');
                        result = new ConditionExpression(fieldId, condOpr, inValues);
                    }
                    else
                        result = new ConditionExpression(fieldId, condOpr, filterResult);
                    break;
                case Datatype.Decimal:
                    result = new ConditionExpression(fieldId, condOpr, new Money(Convert.ToDecimal(filterResult)));
                    break;
                case Datatype.Double:
                    result = new ConditionExpression(fieldId, condOpr, Convert.ToInt32(filterResult));
                    break;
                case Datatype.Date:
                case Datatype.DateTime:
                    result = new ConditionExpression(fieldId, condOpr, Convert.ToDateTime(filterResult));
                    break;
                case Datatype.Boolean:
                    result = new ConditionExpression(fieldId, condOpr, Convert.ToBoolean(filterResult));
                    break;
                case Datatype.Lookup:
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        filterResult = filterResult.Replace("'", "");
                        Guid[] inValues = filterResult.Split(',').Where(g => { Guid temp; return Guid.TryParse(g, out temp); }).Select(g => Guid.Parse(g)).ToArray();
                        result = new ConditionExpression(fieldId, condOpr, inValues);
                    }
                    else
                    {
                        result = new ConditionExpression(fieldId, condOpr, filterResult);
                    }
                    break;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FieldId"></param>
        /// <param name="Operator"></param>
        /// <param name="FilterResult"></param>
        /// <returns></returns>
        public static ConditionExpression GetConditionExpression(ApttusField apttusField, string filterOperator, Datatype dataType, string filterResult, bool isStaticValue = false)
        {
            ConditionExpression result = null;
            ConditionOperator condOpr = GetConditionOperator(filterOperator);
            string fieldId = apttusField.Id;

            // Remove Open Bracket and Close Bracket Generated from ExpressionBuilderHelper -> EscapeAndGetFilterValue as Dynamics CRM Condition Expression does not allow this.
            if (!string.IsNullOrEmpty(filterResult) && (filterResult.StartsWith(Constants.OPEN_BRACKET) || filterResult.EndsWith(Constants.CLOSE_BRACKET)))
            {
                if (filterResult.StartsWith(Constants.OPEN_BRACKET))
                    filterResult = filterResult.Substring(1, filterResult.Length - 1);
                if (filterResult.EndsWith(Constants.CLOSE_BRACKET))
                    filterResult = filterResult.Substring(0, filterResult.Length - 1);
            }


            switch (dataType)
            {
                case Datatype.String:
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        string[] inValues = filterResult.Split(',');
                        result = new ConditionExpression(fieldId, condOpr, inValues);
                    }
                    else if (condOpr == ConditionOperator.Like)
                    {
                        if (!filterResult.StartsWith(Constants.PERCENT) && !filterResult.EndsWith(Constants.PERCENT))
                            filterResult = Constants.PERCENT + filterResult + Constants.PERCENT;
                        result = new ConditionExpression(fieldId, condOpr, filterResult);
                    }
                    else
                        result = new ConditionExpression(fieldId, condOpr, filterResult);
                    break;
                case Datatype.Decimal:
                    decimal inDecValue;
                    if (Decimal.TryParse(filterResult, out inDecValue))
                        result = new ConditionExpression(fieldId, condOpr, inDecValue);
                    break;
                case Datatype.Double:
                    if (apttusField.CRMDataType == "Integer")
                    {
                        int inIntValue;
                        if (Int32.TryParse(filterResult, out inIntValue))
                            result = new ConditionExpression(fieldId, condOpr, inIntValue);
                    }
                    else
                    {
                        Double inDblValue;
                        if (Double.TryParse(filterResult, out inDblValue))
                            result = new ConditionExpression(fieldId, condOpr, inDblValue); // Need to change this type conversion
                    }
                    break;
                case Datatype.Date:
                case Datatype.DateTime:
                    if (condOpr == ConditionOperator.Equal) // DateTime field does not support 'equal' operator , 'ON' is an equivalent operator for DateTime fields
                        condOpr = ConditionOperator.On;
                    if (condOpr == ConditionOperator.In || condOpr == ConditionOperator.NotIn)
                    {
                        string[] inValues = filterResult.Split(',').Where(g => { DateTime temp; return DateTime.TryParse(g, out temp); }).Select(g => DateTime.Parse(g).ToShortDateString()).ToArray();
                        result = new ConditionExpression(fieldId, condOpr, inValues);
                    }
                    else
                    {
                        DateTime inDateValue;
                        if (DateTime.TryParse(filterResult, out inDateValue))
                            result = new ConditionExpression(fieldId, condOpr, inDateValue.ToShortDateString());
                    }
                    break;
                case Datatype.Boolean:
                    result = new ConditionExpression(fieldId, condOpr, Convert.ToBoolean(filterResult));
                    break;
                case Datatype.Picklist:
                    // Remove Open and Close Bracket first before converting                    
                    if (condOpr == ConditionOperator.Like || condOpr == ConditionOperator.NotLike
                        || condOpr == ConditionOperator.BeginsWith || condOpr == ConditionOperator.DoesNotBeginWith)
                        result = new ConditionExpression(fieldId + "name", condOpr, filterResult);
                    else
                    {
                        List<bool> twoOptionValues = new List<bool>();
                        List<string> regularOptionValues = new List<string>();
                        GetPickListOptionSetvalues(filterResult, apttusField, out regularOptionValues, out twoOptionValues);
                        if (apttusField.PicklistType == PicklistType.TwoOption)
                        {
                            if (twoOptionValues.Count > 0)
                            {
                                if (condOpr == ConditionOperator.In || condOpr == ConditionOperator.NotIn)
                                    result = new ConditionExpression(fieldId, condOpr, twoOptionValues.ToArray());
                                else
                                    result = new ConditionExpression(fieldId, condOpr, twoOptionValues[0]);
                            }
                        }
                        else if (apttusField.PicklistType == PicklistType.Regular)
                        {
                            if (regularOptionValues.Count > 0)
                            {
                                if (condOpr == ConditionOperator.In || condOpr == ConditionOperator.NotIn)
                                    result = new ConditionExpression(fieldId, condOpr, regularOptionValues.Select(v => Convert.ToInt32(v)).ToArray());
                                else
                                    result = new ConditionExpression(fieldId, condOpr, Convert.ToInt32(regularOptionValues[0]));
                            }
                        }
                    }
                    break;
                case Datatype.Lookup:
                    filterResult = filterResult.Replace(Constants.QUOTE, "");
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        Guid[] inValues = filterResult.Split(',').Where(g => { Guid temp; return Guid.TryParse(g, out temp); }).Select(g => Guid.Parse(g)).ToArray();
                        if (inValues.Length > 0)
                            result = new ConditionExpression(fieldId, condOpr, inValues);
                        //else if (filterResult.Contains("NULL"))
                        //    result = new ConditionExpression(fieldId, ConditionOperator.Null);
                    }
                    else
                    {
                        Guid lookupId = Guid.Empty;
                        if (Guid.TryParse(filterResult, out lookupId))
                            result = new ConditionExpression(fieldId, condOpr, lookupId);
                    }
                    break;
            }

            /* Create ConditionExpression when User have provided Blank Static Value
             * JIRA Ticker Reference # : AB-2501
            */
            if (isStaticValue && string.IsNullOrEmpty(filterResult))
            {
                if (condOpr == ConditionOperator.Equal || condOpr == ConditionOperator.On)
                    result = new ConditionExpression(fieldId, ConditionOperator.Null);
                else if (condOpr == ConditionOperator.NotEqual)
                    result = new ConditionExpression(fieldId, ConditionOperator.NotNull);
            }

            return result;
        }

        private static void GetPickListOptionSetvalues(string filterResult, ApttusField apttusField, out List<string> regularOptionSetValues, out List<bool> twoOptionSetValues)
        {
            twoOptionSetValues = new List<bool>();
            regularOptionSetValues = new List<string>();
            string[] optionText = filterResult.Split(',');
            foreach (string item in optionText)
            {
                if (apttusField.PicklistKeyValues.Exists(r => r.optionValue.Equals(item, StringComparison.CurrentCultureIgnoreCase))) // AB-2652 : MSD: Filter not working with case insensitive filter values
                {
                    if (apttusField.PicklistType == PicklistType.TwoOption)
                    {
                        // AB-2652 : MSD: Filter not working with case insensitive filter values
                        bool optionValue = apttusField.PicklistKeyValues.FirstOrDefault(r => r.optionValue.Equals(item, StringComparison.CurrentCultureIgnoreCase)).optionKey == "1" ? true : false;
                        twoOptionSetValues.Add(optionValue);
                    }
                    else if (apttusField.PicklistType == PicklistType.Regular)
                    {
                        // AB-2652 : MSD: Filter not working with case insensitive filter values
                        string optionValue = apttusField.PicklistKeyValues.FirstOrDefault(r => r.optionValue.Equals(item, StringComparison.CurrentCultureIgnoreCase)).optionKey;
                        regularOptionSetValues.Add(optionValue);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchFilterGroup"></param>
        /// <returns>ErrorMessage</returns>
        public static void ValidateFiltersLogicalGrouping(List<SearchFilterGroup> searchFilterGroupList, string filterLogicText, out StringBuilder errorMessages)
        {
            errorMessages = new StringBuilder();
            SearchFilterGroup searchFilterGroup = searchFilterGroupList.FirstOrDefault();
            List<string> logicalFilterGroupList = retrieveLogicalFilterGroups(filterLogicText);

            if (logicalFilterGroupList.Count <= 1)
                return;

            foreach (string logicalFilterGroup in logicalFilterGroupList)
            {
                int[] seqNos = logicalFilterGroup.Split('|').Select(str => int.Parse(str)).ToArray();
                List<SearchFilter> searchFilters = searchFilterGroup.Filters.Where(f => seqNos.Contains(f.SequenceNo)).ToList();
                List<Guid> appObjectUniqueIds = searchFilters.Select(f => f.AppObjectUniqueId).ToList();


                if (appObjectUniqueIds.Distinct().ToList().Count > 1)
                {
                    Guid appObjectUniqueId = appObjectUniqueIds.GroupBy(g => g).Select(t => new { count = t.Count(), key = t.Key }).OrderBy(f => f.count).Select(f => f.key).FirstOrDefault();
                    List<int> notLogicalSeqNo = searchFilters.Where(f => f.AppObjectUniqueId == appObjectUniqueId).Select(f => f.SequenceNo).ToList();

                    foreach (int item in notLogicalSeqNo)
                        errorMessages.AppendLine("Row #" + (item).ToString());

                    // break;
                }
            }
        }

        private static List<string> retrieveLogicalFilterGroups(string filterLogicText)
        {
            List<string> groupList = new List<string>();
            var charArray = filterLogicText.ToCharArray();

            string groupItem = string.Empty;
            int openBracketcnt = 0;
            for (int count = 0; count < charArray.Length; count++)
            {
                if (charArray[count] == '(')
                {
                    openBracketcnt++;
                    if (openBracketcnt <= 1)
                    {
                        if (!string.IsNullOrEmpty(groupItem))
                            groupList.Add(groupItem);
                        groupItem = string.Empty;
                    }
                }
                else if (charArray[count] == ')')
                {
                    openBracketcnt--;
                    if (openBracketcnt == 0)
                    {
                        groupList.Add(groupItem);
                        groupItem = string.Empty;
                    }
                }
                else
                    groupItem = groupItem + charArray[count].ToString();
            }
            if (groupList.Count == 0)
                groupList.Add(groupItem);

            List<string> NumericItemList = groupList.Where(f => System.Text.RegularExpressions.Regex.IsMatch(f, @"\d") == true).ToList();
            List<string> validGroupList = new List<string>();
            for (int count = 0; count < NumericItemList.Count; count++)
            {
                string realVal = string.Empty;
                string item = NumericItemList[count];
                string[] numbers = System.Text.RegularExpressions.Regex.Split(item, @"\D+");
                foreach (string value in numbers)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        int i = int.Parse(value);
                        realVal = realVal + i.ToString() + "|";
                    }
                }
                if (!string.IsNullOrEmpty(realVal))
                {
                    realVal = realVal.Substring(0, realVal.Length - 1);
                    validGroupList.Add(realVal);
                }
            }
            return validGroupList;
        }

        public static QueryExpression GetAnnotationsForObjectId(List<string> objectIds)
        {
            QueryExpression noteattachmentQuery = new QueryExpression()
            {
                EntityName = Constants.ATTACHMENTOBJECT_DYNAMICS,
                ColumnSet = new ColumnSet("filename", "documentbody", "objectid"),

                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("objectid", ConditionOperator.In, objectIds),
                                new ConditionExpression("isdocument", ConditionOperator.Equal, true)
                            }
                        }
                    }
                }
            };

            return noteattachmentQuery;
        }

    }
}
