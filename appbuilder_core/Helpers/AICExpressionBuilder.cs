using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

//using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Sdk.Client;
//using Microsoft.Xrm.Sdk.Query;

using EBHelper = Apttus.XAuthor.Core.ExpressionBuilderHelper;
using Apttus.DataAccess.Common.Model;
using Apttus.DataAccess.Common.Enums;
using Apttus.DataAccess.Common.CustomTypes;
using System.Collections;

namespace Apttus.XAuthor.Core
{
    public class AICExpressionBuilderHelper
    {
        static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        static Join leastNodeLinkEntity = null;
        static Expression lastLinkFilterExpression = null;
        public static string DisplayableWhereClause
        {
            get
            {
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
        private static void BuildLinkEnityCollection(ApttusObject sourceObject, ref List<Join> linkEntityCollection, ApplicationField appField)
        {
            Join currentLinkEntity = null;
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
                    //leastNodeLinkEntity.Columns.AddColumn(entityLevel[count]);
                    continue;
                }

                // Set Link Entity Alias Name to match with DataTable Column Name
                if (!string.IsNullOrEmpty(linkEntityAliasName))
                    linkEntityAliasName = linkEntityAliasName + Constants.AIC_ALIAS_SEPARATOR + entityLevel[count];
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
                    var targetObjField = sourceObject.Fields.Where(f => f.Id.Equals(entityLevel[count - 1])).FirstOrDefault();
                    var targetObjectName = string.Empty;
                    if (targetObjField != null) targetObjectName = targetObjField.LookupObject.Id;
                    else targetObjectName = entityLevel[count - 1];
                    var AppObjectCollection = applicationDefinitionManager.GetAppObjectById(targetObjectName);
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
                        linkFromAttribute = entityLevel[count];
                        lookUpName = linkFromAttribute;
                    }
                }

                /*
                 * For All Old apps since Field Sequence is change for some cases LinkTo Entity Does not return any value, Hence refer FindLinkEntityofParentApttusObject to Use correct link connection.
                 * */
                if (string.IsNullOrEmpty(linkToEntity))
                    linkToEntity = FindLinkEntityofParentApttusObject(sourceObject, lookUpName, linkFromEntity);

                linkToAttibute = Constants.ID_ATTRIBUTE; // Link To Attribute is always  Constants.ID_ATTRIBUTE

                currentLinkEntity = new Join(linkFromEntity, linkToEntity, linkFromAttribute, linkToAttibute, JoinType.LEFT);
                InitializeJoin(currentLinkEntity);
                currentLinkEntity.EntityAlias = linkEntityAliasName;
                /*
                 * Find Current Link Entity into Entire LinkEntityCollection Object, this will check Direct & Nested link entities as well to find the copy of current Link Entity
                 * */
                Join existingLinkEntity = getExistingLinkEntity(linkEntityCollection, null, currentLinkEntity);
                if (existingLinkEntity == null)
                {
                    if (leastNodeLinkEntity != null)
                    {
                        Join parentLinkEntity = null;
                        /*
                         * Associate a new link Entity with the correct chain, it's highly possible that new link entity is requried to be added into one of the nested link entites, 
                         * hence following function getExistingLinkEntity will find correct location of the link entity to be added by returning correct parent Link Entity
                         * */
                        if (currentLinkEntity.FromEntity != leastNodeLinkEntity.FromEntity)
                            parentLinkEntity = getExistingLinkEntity(linkEntityCollection, null, leastNodeLinkEntity);
                        // Find parent Link Entity of current Entity
                        if (parentLinkEntity != null)
                        {
                            currentLinkEntity.FromEntity = parentLinkEntity.EntityAlias;
                            existingLinkEntity = getExistingLinkEntity(linkEntityCollection, null, currentLinkEntity);
                            if (existingLinkEntity == null)
                                linkEntityCollection.Add(currentLinkEntity);
                            // parentLinkEntity.Joins.Add(currentLinkEntity);
                        }
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


        private static void BuildLinkEnityAnnotation(ApttusObject sourceObject, ref List<Join> linkEntityCollection)
        {
            //Join currentLinkEntity = new Join(sourceObject.Id, Constants.ATTACHMENTOBJECT_DYNAMICS, sourceObject.IdAttribute, "objectid", JoinType.LEFT);

            //currentLinkEntity.Columns.AddColumn("filename");
            //currentLinkEntity.Columns.AddColumn("annotationid");

            //currentLinkEntity.EntityAlias = "annotationid";

            //linkEntityCollection.Add(currentLinkEntity);
        }

        public static bool GetExpression(string actionId, List<SearchFilterGroup> FilterGroups, ApttusObject SourceObject, string[] InputDataNames, out List<KeyValuePair<string, Guid>> UsedDataSets, out Query queryExpr, bool parseUserInput = false, bool recordLimits = false, string globalFilter = "", List<SearchField> searchFields = null)
        {
            List<Join> linkEntityCollection = new List<Join>();
            // Create Query Expression object
            queryExpr = new Query(SourceObject.Id)
            {
                SortOrders = new List<OrderBy>(),

                Criteria = new Expression()
                {
                    Conditions = new List<DataAccess.Common.Model.Condition>(),
                    Filters = new List<Expression>()
                },
                Joins = new List<Join>(),
                PageInfo = new PageInfo()
            };
            leastNodeLinkEntity = null;
            // Change to get all Field since List layout also contains related object fields.
            //List<string> appFields = configurationManager.GetAllAppFields(SourceObject, false, true).Where(f => f.AppObject == SourceObject.UniqueId).Select(s => s.FieldId).ToList();
            List<ApplicationField> appFieldList = configurationManager.GetAllAppFields(SourceObject, false, true);
            List<string> appFields = appFieldList.Select(s => s.FieldId).ToList();

            // Remove ID field for Dynamics, as Dynamics doesn't have Id on all Entities
            //appFields.Remove(Constants.ID_ATTRIBUTE);

            // Remove Lookup.Name fields as EntityReference provides name value and is take care in QueryDataSet Func()
            appFields.RemoveAll(f => f.EndsWith(Constants.APPENDLOOKUPID));

            // Remove Temporarly
            //appFields.Remove(Constants.NAME_ATTRIBUTE);

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
                {
                    var fieldSplits = item.FieldId.Split('.');
                    if (fieldSplits.Count() > 2)
                    {
                        int lastInd = item.FieldId.LastIndexOf('.');
                        var relField = item.FieldId.Remove(lastInd, fieldSplits[fieldSplits.Count() - 1].Length + 1);
                        relField = relField.Replace(Constants.DOT, Constants.AIC_ALIAS_SEPARATOR);
                        relField = relField + Constants.DOT + fieldSplits[fieldSplits.Count() - 1];
                        currentObjectFields.Add(relField);
                    }
                    else
                    {
                        currentObjectFields.Add(item.FieldId);
                    }
                    BuildLinkEnityCollection(SourceObject, ref linkEntityCollection, item);
                }
            }

            UsedDataSets = new List<KeyValuePair<string, Guid>>();

            Expression AndExpression = new Expression()
            {
                Conditions = new List<DataAccess.Common.Model.Condition>(),
                Filters = new List<Expression>()
            };
            Expression OrExpression = new Expression()
            {
                Conditions = new List<DataAccess.Common.Model.Condition>(),
                Filters = new List<Expression>()
            };
            AndExpression.ExpressionOperator = ExpressionOperator.AND;
            OrExpression.ExpressionOperator = ExpressionOperator.OR;
            AndExpression.Conditions = new List<DataAccess.Common.Model.Condition>();
            OrExpression.Conditions = new List<DataAccess.Common.Model.Condition>();

            // Only all columns of current object
            //queryExpr.ColumnSet = new ColumnSet(appFields.ToArray());
            queryExpr.Columns = new List<string>(currentObjectFields.ToArray());

            if (recordLimits)
                queryExpr.TopRecords = 500;

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

                Apttus.DataAccess.Common.Model.Condition whereClause = null;

                string lastConditionOprand = string.Empty;

                string RegExPattern = @"(and)+|(or)+|\(|\)|[0-9]+";
                bool includeInLinkEntity = false;
                SearchFilter searchFilter = null;
                ApttusField nameLookupField = null;
                Join currentLinkEntity = null;
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
                                Join linkEntity = new Join(SourceObject.Id, nameLookupField.LookupObject.Id, linkFrom, nameLookupField.LookupObject.IdAttribute, JoinType.INNER);
                                InitializeJoin(linkEntity);
                                Join existingLinkEntity = getExistingLinkEntity(linkEntityCollection, null, linkEntity);
                                if (existingLinkEntity != null)
                                    linkEntity = existingLinkEntity;
                                else
                                    linkEntityCollection.Add(linkEntity);

                                currentLinkEntity = linkEntity;
                                if (currentLinkEntity.JoinCriteria == null) currentLinkEntity.JoinCriteria = new Expression();
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
                            //if (nameLookupField.MultiLookupObjects != null && nameLookupField.MultiLookupObjects.Count > 0)
                            //{
                            //    //modify current where clause
                            //    whereClause = ChangeConditionExpressionforMultiObject(nameLookupField, whereClause);
                            //}
                        }
                        else if (whereClause == null)
                            currentLinkEntity = null;

                        // In case whereClause is Blank for UserInput type of filter, skip to the next expression - User did not specify any value.
                        if (object.ReferenceEquals(whereClause, null) && (searchFilter.ValueType == ExpressionValueTypes.UserInput ||
                                                                  searchFilter.ValueType == ExpressionValueTypes.CellReference))
                        {
                            isValidCriteria = true;
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

                                whereClause.EntityName = nameLookupField.LookupObject.Id;
                                currentLinkEntity.JoinCriteria.AddCondition(whereClause);
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
                                whereClause.EntityName = nameLookupField.LookupObject.Id;
                                currentLinkEntity.JoinCriteria.AddCondition(whereClause);
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
                                if (nameLookupField.LookupObject != null) whereClause.EntityName = nameLookupField.LookupObject.Id;
                                currentLinkEntity.JoinCriteria.AddCondition(whereClause);
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

                // Apply global Filter to All Searchable Fields with OR Apttus.DataAccess.Common.Model.Condition expression
                if (!string.IsNullOrEmpty(globalFilter))
                {
                    Expression globalFilterExp = getGlobalFilterExpression(globalFilter, searchFilterGroup, SourceObject, searchFields);
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

            // if there are no Filters, but relational attributes are added, create linkEntityCollection which should be added in Query
            foreach (Join item in linkEntityCollection)
                queryExpr.Joins.Add(item);

            RemovePercentagesForContainsOperator(queryExpr);

            return true;
        }

        //TODO : Its temporary patch - as Like operator is not working in AQL as of now and thats why replaced Like with contains and remving % from values. like %xxx% => xxx
        static void RemovePercentagesForContainsOperator(Query query)
        {
            foreach (var filter in query.Criteria.Filters)
            {
                foreach (var x in filter.Conditions)
                {
                    if (x.FilterOperator == FilterOperator.Contains)
                    {
                        x.Value = x.Value.ToString().Substring(1, x.Value.ToString().Length - 2);
                    }
                }
            }
        }

        /// <summary>
        /// used to create displayable where clause from Dynamics query structure.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static string GetFilterLogicClause(List<Expression> filters)
        {
            StringBuilder filterlogic = new StringBuilder();
            try
            {
                foreach (var x in filters)
                {
                    filterlogic.Append("(");
                    foreach (var con in x.Conditions)
                    {
                        List<string> values = CastObjecttoList(con.Value, con.FilterOperator);
                        string str = string.Format("{0} {1} {2}", con.FieldName, con.FilterOperator, string.Join(",", values));
                        filterlogic.Append(str);
                        if (x.Conditions.IndexOf(con) != x.Conditions.Count - 1)
                        {
                            filterlogic.Append(string.Format(" {0} ", x.ExpressionOperator));
                        }
                    }
                    filterlogic.Append(")");
                    if (filters.IndexOf(x) != filters.Count - 1)
                    {
                        filterlogic.Append(string.Format(" {0} ", filters[filters.IndexOf(x) + 1].ExpressionOperator));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

            return filterlogic.ToString();
        }

        /// <summary>
        /// method used to convert object to list of string for filter values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filterOperator"></param>
        /// <returns></returns>
        public static List<string> CastObjecttoList(object value, FilterOperator filterOperator)
        {
            List<string> values = new List<string>();
            //Type valueType = value.GetType();
            if (filterOperator == FilterOperator.In || filterOperator == FilterOperator.NotIn)
            {
                var tovalues = ((IEnumerable)value).Cast<object>().ToList();
                foreach (var x in tovalues)
                {
                    values.Add(x.ToString());
                }
            }
            else
            {
                values.Add(value == null ? string.Empty : value.ToString());
            }
            return values;
        }
        private static Apttus.DataAccess.Common.Model.Condition ChangeConditionExpressionforMultiObject(ApttusField field, Apttus.DataAccess.Common.Model.Condition currentCondExpression)
        {
            List<ApttusDataSet> dataSet = new List<ApttusDataSet>();
            foreach (ApttusObject apttusObject in field.MultiLookupObjects)
            {
                Query query = new Query(apttusObject.Id);

                /*  AB - 2657 - MSD: User input with IN filter in query action throws error. 
                 * &&&
                 * AB - 2658 - MSD: Using 'Not equal to Blank' filter on multilookup field  throws error. 
                */
                if (currentCondExpression.FilterOperator == FilterOperator.In || currentCondExpression.FilterOperator == FilterOperator.NotIn)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.FilterOperator, (currentCondExpression.Value as List<string>).ToArray());
                else if (currentCondExpression.FilterOperator == FilterOperator.Null || currentCondExpression.FilterOperator == FilterOperator.NotNull)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.FilterOperator);
                else if (currentCondExpression.Value != null && (currentCondExpression.Value as List<string>).Count == 1)
                    query.Criteria.AddCondition(apttusObject.NameAttribute, currentCondExpression.FilterOperator, (currentCondExpression.Value as List<string>)[0]);

                dataSet.Add(ObjectManager.GetInstance.QueryDataSet(new AICQuery { Query = query, DataTable = null, Object = null, UserInfo = null }));
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
                currentCondExpression = new Apttus.DataAccess.Common.Model.Condition(field.Id.Split('.')[0], FilterOperator.In, recordIds.ToArray());
            return currentCondExpression;
        }

        private static void BuildLinkEntityCollection(SearchFilter searchFilters, ref List<Join> linkEntityCollection, Apttus.DataAccess.Common.Model.Condition filterConditionExression, List<string> appFields, string lastCondOperator, bool bCloseExpression = false)
        {
            Join lastLinkEntity = null;

            // Set Link Entity Join Operator based on the Filter Operator, this is import part since which will decide to make link entity relation with Left Outer or with Inner Join
            ExpressionOperator crmCondOperator = (lastCondOperator == Constants.OR ? ExpressionOperator.OR : ExpressionOperator.AND);

            /*
             * Make New Link Entity if required based on Query Objects mention inside Search Filter, this is an other way of creating Link Entities and Adding Filter into that, however this will refer to Same LinkEntityCollection Object 
             * Which at the end will make entire link Expression
             * */
            for (int level = 0; level < searchFilters.QueryObjects.Count; level++)
            {
                if (string.IsNullOrEmpty(searchFilters.QueryObjects[level].LookupFieldId) && level == searchFilters.QueryObjects.Count - 1)
                {
                    Expression linkExpression = null;
                    if (lastLinkEntity.JoinCriteria.Filters == null) lastLinkEntity.JoinCriteria.Filters = new List<Expression>();
                    if (lastLinkEntity.JoinCriteria.Filters.Count == 0)
                    {
                        lastLinkEntity.JoinType = lastCondOperator == Constants.OR ? JoinType.LEFT : JoinType.INNER;
                        linkExpression = new Expression();
                        lastLinkEntity.JoinCriteria.AddFilter(linkExpression);
                    }
                    else
                    {
                        if (lastLinkFilterExpression != null && lastLinkEntity.JoinCriteria.Filters.Count == 1)
                            linkExpression = lastLinkEntity.JoinCriteria.Filters[0];
                        else
                        {
                            if (lastLinkEntity.JoinCriteria.Filters.Any(f => f.ExpressionOperator == crmCondOperator))
                                linkExpression = lastLinkEntity.JoinCriteria.Filters.LastOrDefault(f => f.ExpressionOperator == crmCondOperator);
                            else
                            {
                                linkExpression = new Expression(crmCondOperator);
                                lastLinkEntity.JoinCriteria.AddFilter(linkExpression);
                            }
                        }
                    }
                    if (linkExpression.Conditions == null) linkExpression.Conditions = new List<DataAccess.Common.Model.Condition>();
                    filterConditionExression.EntityName = lastLinkEntity.EntityAlias;
                    if (linkExpression.Conditions.Count > 0)
                    {
                        if (linkExpression.Conditions.Count == 1)
                            linkExpression.ExpressionOperator = crmCondOperator;
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
                string linkFromEntity = (lastLinkEntity != null && primanyApttusObject.Id == lastLinkEntity.ToEntity) ? lastLinkEntity.EntityAlias : primanyApttusObject.Id;
                string linkToEntity = secondaryApttusObject.Id;

                // Use Common Function getExistingLinkEntity to find associated matching link entity into Entire LinkEntityCollection
                Join existingLinkEntity = getExistingLinkEntity(linkEntityCollection, searchFilters.QueryObjects[level], new Join { FromEntity = linkFromEntity, ToEntity = linkToEntity, FromAttribute = linkFromAttribute, ToAttribute = linkToAttribute });
                lastLinkEntity = existingLinkEntity;

                if (existingLinkEntity == null)
                {
                    // Initiate new Link Entity Object
                    lastLinkEntity = new Join { FromEntity = linkFromEntity, ToEntity = linkToEntity, FromAttribute = linkFromAttribute, ToAttribute = linkToAttribute, EntityAlias = linkToEntity };
                    InitializeJoin(lastLinkEntity);

                    // get All Link Entity Columns from All Application Fields
                    //List<string> linkEntityColumns = getAppFieldsbyObject(lastLinkEntity.ToEntity, appFields);
                    //if (linkEntityColumns.Count > 0)
                    //    lastLinkEntity.Columns = new List<string> ( linkEntityColumns.ToArray() ); // Add All App fields Columns to Link Entities

                    //if (level > 0) // When Link Starts with Level 0 it means it should be a Root Level Link Entity which is directly going to associated with Query Expression on Source Object
                    //{
                    //    // get Immediate parent relationship
                    //    //Join parentLinkEntity = getExistingLinkEntity(linkEntityCollection, searchFilters.QueryObjects[level - 1]);
                    //    //if (parentLinkEntity != null)
                    //    //    parentLinkEntity.Joins.Add(lastLinkEntity);
                    //    //else
                    //        // if no parent link Entity found create new link Entity which will add this link entity onto Root Level Link Entity Collection                   
                    //        linkEntityCollection.Add(lastLinkEntity);
                    //}
                    //else
                    linkEntityCollection.Add(lastLinkEntity); // if no parent link Entity found create new link Entity which will add this link entity onto Root Level Link Entity Collection                   
                }
            }
        }

        //private static Join getChildMostLinkEntity(Join parentLinkEntity)
        //{
        //    if (parentLinkEntity.Joins != null && parentLinkEntity.Joins.Count > 0)
        //    {
        //        return getChildMostLinkEntity(parentLinkEntity.Joins[0]);
        //    }
        //    else
        //    {
        //        return parentLinkEntity;
        //    }
        //}

        //private static void prepareLinkEntityCollection(ApttusObject sourceObject, SearchFilterGroup searchFilterGroup, ref List<Join> linkEntityCollection)
        //{

        //    foreach (SearchFilter item in searchFilterGroup.Filters)
        //    {
        //        if (item.QueryObjects.Count <= 1)
        //            continue;
        //        for (int count = 0; count < item.QueryObjects.Count; count++)
        //        {
        //            if (string.IsNullOrEmpty(item.QueryObjects[count].LookupFieldId))
        //                continue;
        //            ApttusObject primanyApttusObject = applicationDefinitionManager.GetAppObject(item.QueryObjects[count].LeafAppObjectUniqueId);
        //            Guid secondaryObjectId = item.QueryObjects.Where(objectid => objectid.LeafAppObjectUniqueId == item.QueryObjects[count].AppObjectUniqueId).Select(objectid => objectid.LeafAppObjectUniqueId).FirstOrDefault();
        //            ApttusObject secondaryApttusObject = applicationDefinitionManager.GetAppObject(secondaryObjectId);
        //            string linkFromAttribute = item.QueryObjects[count].LookupFieldId;
        //            string linkToAttribute = secondaryApttusObject.IdAttribute;
        //            string linkFromEntity = primanyApttusObject.Id;
        //            string linkToEntity = secondaryApttusObject.Id;

        //            Join existingLinkEntity = getExistingLinkEntity(linkEntityCollection, item.QueryObjects[count]);
        //            if (existingLinkEntity == null)
        //            {
        //                if (count > 0)
        //                {
        //                    // get Immediate parent relationship
        //                    Join parentLinkEntity = getExistingLinkEntity(linkEntityCollection, item.QueryObjects[count - 1]);
        //                    if (parentLinkEntity != null)
        //                    {
        //                        parentLinkEntity.Joins.Add(new Join { FromEntity = linkFromEntity, ToEntity = linkToEntity, FromAttribute = linkFromAttribute, ToAttribute = linkToAttribute });
        //                    }
        //                    else
        //                    {
        //                        // if no parent link Entity found create new link Entity
        //                        linkEntityCollection.Add(new Join { FromEntity = linkFromEntity, ToEntity = linkToEntity, FromAttribute = linkFromAttribute, ToAttribute = linkToAttribute });
        //                    }
        //                }
        //                else
        //                {
        //                    linkEntityCollection.Add(new Join { FromEntity = linkFromEntity, ToEntity = linkToEntity, FromAttribute = linkFromAttribute, ToAttribute = linkToAttribute });
        //                }
        //            }
        //        }
        //    }
        //}

        private static Join getExistingLinkEntity(List<Join> linkEntityCollection, QueryObject parentQueryObjects, Join searchLinkEntity = null)
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
                searchLinkEntity = new Join(linkFromEntity, linkToEntity, linkFromAttribute, linkToAttribute, JoinType.INNER);
                InitializeJoin(searchLinkEntity);
            }

            foreach (Join item in linkEntityCollection)
            {
                if (item.FromEntity == searchLinkEntity.FromEntity && item.ToEntity == searchLinkEntity.ToEntity && item.FromAttribute == searchLinkEntity.FromAttribute && item.ToAttribute == searchLinkEntity.ToAttribute)
                    return item;
            }

            //foreach (Join item in linkEntityCollection)
            //{
            //    if (item.Joins.Count > 0)
            //        return getExistingLinkEntity(item.Joins.ToList(), parentQueryObjects, searchLinkEntity);
            //}

            return null;
        }

        private static void upateLinkEntityCollection(ref List<Join> linkEntityCollection, Apttus.DataAccess.Common.Model.Condition condExpression, SearchFilter searchFilters)
        {

            Guid leafObjectId = searchFilters.QueryObjects.Last().AppObjectUniqueId;
            QueryObject parentQueryRelation = searchFilters.QueryObjects.FirstOrDefault(field => field.AppObjectUniqueId == leafObjectId);
            Join matchLinkEntity = getExistingLinkEntity(linkEntityCollection, parentQueryRelation);
            matchLinkEntity.JoinCriteria.AddCondition(condExpression);
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

        private static Expression getGlobalFilterExpression(string globalFilter, SearchFilterGroup FilterGroups, ApttusObject sourceObject, List<SearchField> searchFields)
        {
            Expression globalFilterExpression = null;


            globalFilterExpression = new Expression(ExpressionOperator.OR);

            List<SearchFilter> filterList = FilterGroups.Filters.OrderBy(z => z.SequenceNo).ToList();

            foreach (SearchFilter item in filterList)
            {
                ApttusField apttusFields = sourceObject.Fields.FirstOrDefault(f => f.Id == item.FieldId);

                if (apttusFields.Datatype != Datatype.Lookup)
                {
                    if (apttusFields.LookupObject == null)
                    {
                        Apttus.DataAccess.Common.Model.Condition expression = GetConditionExpression(apttusFields, "like %#FILTERVALUE%", apttusFields.Datatype, globalFilter);
                        //Apttus.DataAccess.Common.Model.Condition expression = GetLikeConditionExpressionBasedOnDataType(globalFilter, apttusFields);
                        if (expression != null)
                            globalFilterExpression.AddCondition(expression);
                    }
                }
            }

            foreach (SearchField field in searchFields)
            {
                ApttusField apttusFields = sourceObject.Fields.FirstOrDefault(f => f.Id == field.Id);

                if (apttusFields.Datatype != Datatype.Lookup)
                {
                    if (apttusFields.LookupObject == null)
                    {
                        Apttus.DataAccess.Common.Model.Condition expression = GetConditionExpression(apttusFields, "like %#FILTERVALUE%", apttusFields.Datatype, globalFilter);
                        //Apttus.DataAccess.Common.Model.Condition expression = GetLikeConditionExpressionBasedOnDataType(globalFilter, apttusFields);
                        if (expression != null)
                            globalFilterExpression.AddCondition(expression);
                    }
                }
            }




            return globalFilterExpression;

        }

        public static Apttus.DataAccess.Common.Model.Condition GetLikeConditionExpressionBasedOnDataType(string searchCriteria, ApttusField apttusField)
        {
            string LHSPlusOperator = string.Empty, value = string.Empty, whereClause = string.Empty;
            Apttus.DataAccess.Common.Model.Condition expression = null;
            GetLikeConditionExpressionBasedOnDataType(searchCriteria, apttusField.Id, apttusField.Datatype, ref LHSPlusOperator, ref value, ref expression);
            return expression;
        }
        public static void GetLikeConditionExpressionBasedOnDataType(string searchCriteria, string LHSField, Datatype datatype, ref string LHSPlusOperator, ref string value, ref Apttus.DataAccess.Common.Model.Condition conditionExpression)
        {
            switch (datatype)
            {
                case Datatype.Date:
                case Datatype.DateTime:
                    string validDate = Utils.IsValidDate(searchCriteria, datatype);
                    if (!string.IsNullOrEmpty(validDate))
                    {
                        conditionExpression = new Apttus.DataAccess.Common.Model.Condition(LHSField, FilterOperator.Equal, Convert.ToDateTime(validDate));
                        LHSPlusOperator = LHSField + " = ?";
                        value = validDate;
                    }
                    break;
                case Datatype.Decimal:
                case Datatype.Double:
                    Decimal validNumber;
                    if (Decimal.TryParse(searchCriteria, out validNumber))
                    {
                        conditionExpression = new Apttus.DataAccess.Common.Model.Condition(LHSField, FilterOperator.Equal, validNumber);
                        LHSPlusOperator = LHSField + " = ?";
                        value = validNumber.ToString();
                    }
                    break;
                case Datatype.String:
                    conditionExpression = new Apttus.DataAccess.Common.Model.Condition(LHSField, FilterOperator.Like, "%" + searchCriteria + "%");
                    value = searchCriteria;
                    break;
                case Datatype.Picklist:
                    if (!string.IsNullOrEmpty(searchCriteria))
                    {
                        conditionExpression = new Apttus.DataAccess.Common.Model.Condition(LHSField, FilterOperator.Equal, searchCriteria);
                        value = searchCriteria;
                    }
                    break;
                // Lookup Id field need to be exact match.
                case Datatype.Lookup:
                    Guid id = Guid.Empty;
                    Guid.TryParse(searchCriteria, out id);
                    if (id != Guid.Empty)
                    {
                        conditionExpression = new Apttus.DataAccess.Common.Model.Condition(LHSField, FilterOperator.Equal, id);
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

        private static Apttus.DataAccess.Common.Model.Condition GetSingleWhereClause(string actionId, SearchFilter searchFilter, ApttusObject SourceObject, List<InputData> inputData, ref List<KeyValuePair<string, Guid>> UsedDataSets, bool Designer, SearchFilterGroup searchFilterGroup)
        {
            string filterResult = string.Empty;
            bool isValidWhereClause = true;
            Apttus.DataAccess.Common.Model.Condition conditionExpr = null;
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
                //conditionExpr = new Apttus.DataAccess.Common.Model.Condition(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.Input)
            {
                isValidWhereClause = getInputFilterValue(searchFilter, SourceObject, inputData, ref UsedDataSets, out filterResult, Designer);

                conditionExpr = GetConditionExpression(apttusField, searchFilter.Operator, dataType, filterResult);
                //conditionExpr = new Apttus.DataAccess.Common.Model.Condition(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
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
                        //conditionExpr = new Apttus.DataAccess.Common.Model.Condition(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
                    }
                }
                // when we use Userinput in S&S action OR Query Action with Advance filters than get default values and pass in query
                if (searchFilterGroup.IsAddFilter && !isValidWhereClause && string.IsNullOrEmpty(filterResult))
                {
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);

                    conditionExpr = new Apttus.DataAccess.Common.Model.Condition(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
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
                    if (dataType == Datatype.Boolean) filterResult = "";
                    else
                        filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
                }
                // Create Apttus.DataAccess.Common.Model.Condition Expression anyways , since dynamics needs it for filtering data
                //conditionExpr = new Apttus.DataAccess.Common.Model.Condition(searchFilter.FieldId, GetConditionOperator(Operator), filterResult);
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
        private static FilterOperator GetConditionOperator(string filterOperator)
        {
            FilterOperator condOpr = FilterOperator.Null;

            switch (filterOperator)
            {
                case "=":
                    condOpr = FilterOperator.Equal;
                    break;
                case "!=":
                    condOpr = FilterOperator.NotEqual;
                    break;
                case "<":
                    condOpr = FilterOperator.LessThan;
                    break;
                case "<=":
                    condOpr = FilterOperator.LessEqual;
                    break;
                case ">":
                    condOpr = FilterOperator.GreaterThan;
                    break;
                case ">=":
                    condOpr = FilterOperator.GreaterEqual;
                    break;
                case "in":
                case "includes ('#FILTERVALUE')":
                    condOpr = FilterOperator.In;
                    break;
                case "not in":
                case "excludes ('#FILTERVALUE')":
                    condOpr = FilterOperator.NotIn;
                    break;
                case "like #FILTERVALUE%":
                case "like %#FILTERVALUE%":
                    condOpr = FilterOperator.Like;
                    break;
                case "like %#NOTFILTERVALUE%":
                    condOpr = FilterOperator.NotLike;
                    break;
                //case "on":
                //    condOpr = FilterOperator.On;
                //    break;
                default:
                    condOpr = FilterOperator.Null;
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
        public static Apttus.DataAccess.Common.Model.Condition GetConditionExpression(string fieldId, string filterOperator, Datatype dataType, string filterResult)
        {
            Apttus.DataAccess.Common.Model.Condition result = null;
            FilterOperator condOpr = GetConditionOperator(filterOperator);



            switch (dataType)
            {
                case Datatype.String:
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        string[] inValues = filterResult.Split(',');
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                    }
                    else
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, filterResult);
                    break;
                case Datatype.Decimal:
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, Convert.ToDecimal(filterResult));
                    break;
                case Datatype.Double:
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, Convert.ToInt32(filterResult));
                    break;
                case Datatype.Date:
                case Datatype.DateTime:
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, Convert.ToDateTime(filterResult));
                    break;
                case Datatype.Boolean:
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, Convert.ToBoolean(filterResult));
                    break;
                case Datatype.Lookup:
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        filterResult = filterResult.Replace("'", "");
                        Guid[] inValues = filterResult.Split(',').Where(g => { Guid temp; return Guid.TryParse(g, out temp); }).Select(g => Guid.Parse(g)).ToArray();
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                    }
                    else
                    {
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, filterResult);
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
        public static Apttus.DataAccess.Common.Model.Condition GetConditionExpression(ApttusField apttusField, string filterOperator, Datatype dataType, string filterResult, bool isStaticValue = false)
        {
            Apttus.DataAccess.Common.Model.Condition result = null;
            FilterOperator condOpr = GetConditionOperator(filterOperator);
            string fieldId = apttusField.Id;

            // Remove Open Bracket and Close Bracket Generated from ExpressionBuilderHelper -> EscapeAndGetFilterValue as Dynamics CRM Apttus.DataAccess.Common.Model.Condition Expression does not allow this.
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
                    if (!string.IsNullOrEmpty(filterResult))
                    {
                        if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                        {
                            string[] inValues = filterResult.Split(',');
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                        }
                        else if (condOpr == FilterOperator.Like)
                        {
                            if (!filterResult.StartsWith(Constants.PERCENT) && !filterResult.EndsWith(Constants.PERCENT))
                                filterResult = Constants.PERCENT + filterResult + Constants.PERCENT;
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, filterResult);
                        }
                        else
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, filterResult);
                    }
                    break;
                case Datatype.Decimal:
                    decimal inDecValue;
                    if (Decimal.TryParse(filterResult, out inDecValue))
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inDecValue);
                    break;
                case Datatype.Double:
                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                    {
                        string[] inValues = filterResult.Split(',');
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                    }
                    else if (apttusField.CRMDataType == "Integer")
                    {
                        int inIntValue;
                        if (Int32.TryParse(filterResult, out inIntValue))
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inIntValue);
                    }
                    else
                    {
                        Double inDblValue;
                        if (Double.TryParse(filterResult, out inDblValue))
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inDblValue); // Need to change this type conversion
                    }
                    break;
                case Datatype.Date:
                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                    {
                        if (!string.IsNullOrEmpty(filterResult))
                        {
                            string[] inValues = filterResult.Split(',').Where(g => { DateTime temp; return DateTime.TryParse(g, out temp); }).Select(g => DateTime.Parse(g).ToShortDateString()).ToArray();
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filterResult))
                        {
                            DateTime inDateValue;
                            if (DateTime.TryParse(filterResult, out inDateValue))
                                result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inDateValue.ToShortDateString());
                        }
                    }
                    break;
                case Datatype.DateTime:
                    //if (condOpr == FilterOperator.Equal) // DateTime field does not support 'equal' operator , 'ON' is an equivalent operator for DateTime fields
                    //condOpr = FilterOperator.On;
                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                    {
                        if (!string.IsNullOrEmpty(filterResult))
                        {
                            string[] inValues = filterResult.Split(',').Where(g => { DateTime temp; return DateTime.TryParse(g, out temp); }).Select(g => DateTime.Parse(g).ToString()).ToArray();
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filterResult))
                        {
                            DateTime inDateValue;
                            if (DateTime.TryParse(filterResult, out inDateValue))
                                result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inDateValue.ToString());
                        }
                    }
                    break;
                case Datatype.Boolean:
                    if (!string.IsNullOrEmpty(filterResult))
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, Convert.ToBoolean(filterResult));
                    break;
                case Datatype.Picklist:
                    // Remove Open and Close Bracket first before converting                    
                    if (condOpr == FilterOperator.Like || condOpr == FilterOperator.NotLike
                        || condOpr == FilterOperator.BeginsWith || condOpr == FilterOperator.DoesNotBeginWith)
                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, filterResult);
                    else
                    {
                        if (apttusField.PicklistKeyValues.Count > 0)
                        {
                            List<bool> twoOptionValues = new List<bool>();
                            List<string> regularOptionValues = new List<string>();
                            GetPickListOptionSetvalues(filterResult, apttusField, out regularOptionValues, out twoOptionValues);
                            if (apttusField.PicklistType == PicklistType.TwoOption)
                            {
                                if (twoOptionValues.Count > 0)
                                {
                                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, twoOptionValues.ToArray());
                                    else
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, twoOptionValues[0]);
                                }
                            }
                            else if (apttusField.PicklistType == PicklistType.Regular)
                            {
                                if (regularOptionValues.Count > 0)
                                {
                                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, regularOptionValues.ToArray());
                                    else
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, regularOptionValues[0]);
                                }
                            }
                        }
                        else if (apttusField.PicklistValues.Count > 0)
                        {
                            List<bool> twoOptionValues = new List<bool>();
                            List<string> regularOptionValues = new List<string>();
                            GetPickListValuesForEmptyKeys(filterResult, apttusField, out regularOptionValues, out twoOptionValues);
                            if (apttusField.PicklistType == PicklistType.TwoOption)
                            {
                                if (twoOptionValues.Count > 0)
                                {
                                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, twoOptionValues.ToArray());
                                    else
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, twoOptionValues[0]);
                                }
                            }
                            else if (apttusField.PicklistType == PicklistType.Regular)
                            {
                                if (regularOptionValues.Count > 0)
                                {
                                    if (condOpr == FilterOperator.In || condOpr == FilterOperator.NotIn)
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, regularOptionValues.ToArray());
                                    else
                                        result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, regularOptionValues[0]);
                                }
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
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                        //else if (filterResult.Contains("NULL"))
                        //    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, FilterOperator.Null);
                    }
                    else
                    {
                        Guid lookupId = Guid.Empty;
                        if (Guid.TryParse(filterResult, out lookupId))
                            result = new Apttus.DataAccess.Common.Model.Condition(fieldId, condOpr, lookupId);
                    }
                    break;
                case Datatype.Composite:
                    filterResult = filterResult.Replace(Constants.QUOTE, "");
                    if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
                    {
                        Guid temp = Guid.Empty;
                        object[] inValues = filterResult.Split(',').Where(g => { return Guid.TryParse(g, out temp); })
                                            .Select(g => new { Id = g, Type = apttusField.LookupObject.Id })
                                            .ToArray();

                        if (inValues.Length > 0)
                            result = new DataAccess.Common.Model.Condition(fieldId, condOpr, inValues);
                        //else if (filterResult.Contains("NULL"))
                        //    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, FilterOperator.Null);
                    }
                    else
                    {
                        Guid lookupId = Guid.Empty;
                        if (Guid.TryParse(filterResult, out lookupId))
                        {
                            object compositeObj = new { Id = lookupId, Type = apttusField.LookupObject.Id };
                            result = new DataAccess.Common.Model.Condition(fieldId, condOpr, compositeObj);
                        }
                    }
                    break;
            }

            /* Create Apttus.DataAccess.Common.Model.Condition when User have provided Blank Static Value
             * JIRA Ticker Reference # : AB-2501
            */
            if (isStaticValue && string.IsNullOrEmpty(filterResult))
            {
                if (condOpr == FilterOperator.Equal /*|| condOpr == FilterOperator.On*/)
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, FilterOperator.Null);
                else if (condOpr == FilterOperator.NotEqual)
                    result = new Apttus.DataAccess.Common.Model.Condition(fieldId, FilterOperator.NotNull);
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
        private static void GetPickListValuesForEmptyKeys(string filterResult, ApttusField apttusField, out List<string> regularOptionSetValues, out List<bool> twoOptionSetValues)
        {
            twoOptionSetValues = new List<bool>();
            regularOptionSetValues = new List<string>();
            string[] optionText = filterResult.Split(',');
            foreach (string item in optionText)
            {
                if (apttusField.PicklistValues.Exists(r => r.Equals(item, StringComparison.CurrentCultureIgnoreCase))) // AB-2652 : MSD: Filter not working with case insensitive filter values
                {
                    if (apttusField.PicklistType == PicklistType.TwoOption)
                    {
                        // AB-2652 : MSD: Filter not working with case insensitive filter values
                        bool optionValue = apttusField.PicklistValues.FirstOrDefault(r => r.Equals(item, StringComparison.CurrentCultureIgnoreCase)) == "1" ? true : false;
                        twoOptionSetValues.Add(optionValue);
                    }
                    else if (apttusField.PicklistType == PicklistType.Regular)
                    {
                        // AB-2652 : MSD: Filter not working with case insensitive filter values
                        var optionValue = apttusField.PicklistValues.FirstOrDefault(r => r.Equals(item, StringComparison.CurrentCultureIgnoreCase));
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

        public static Query GetAnnotationsForObjectId(List<string> objectIds, string objectId)
        {
            Query attachmentsQuery = new Query(Constants.ATTACHMENTOBJECT_AIC);
            attachmentsQuery.Columns = new List<string> { Constants.NAME_ATTRIBUTE, "Url", "ContextObject" };

            List<Composite> lstContxObj = new List<Composite>();
            foreach (string objId in objectIds)
            {
                Composite contxObj = new Composite();
                contxObj.Id = objId;
                contxObj.Type = objectId;
                lstContxObj.Add(contxObj);
            }
            Expression filters = new Expression(ExpressionOperator.AND);
            filters.AddCondition("ContextObject", FilterOperator.In, lstContxObj);
            filters.AddCondition("IsPrivate", FilterOperator.Equal, false);

            attachmentsQuery.Criteria = filters;

            return attachmentsQuery;
        }

        /// <summary>
        /// to initialise all collections of join object - as this is missing in constructor of Join.
        /// </summary>
        /// <param name="join"></param>
        public static void InitializeJoin(Join join)
        {
            join.JoinCriteria = new Expression();
        }

    }
}
