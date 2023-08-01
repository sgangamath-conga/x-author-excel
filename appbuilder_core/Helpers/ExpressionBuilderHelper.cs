/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace Apttus.XAuthor.Core
{
    public class ExpressionBuilderHelper
    {
        static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        static ObjectManager objectManager = ObjectManager.GetInstance;
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
      
        public static bool UseSubQuery { get; set; }
        public static bool UseChunking { get; set; }

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

        public static string filterLogicText = string.Empty;
        //TODO: refactor to combine with GetFilterGroupFields
        public static void GetFilterGroupObjects(SearchFilterGroup filterGroup, List<string> objectSet)
        {
            if (filterGroup.Groups != null)
            {
                foreach (SearchFilterGroup childGroup in filterGroup.Groups)
                    GetFilterGroupObjects(childGroup, objectSet);
            }

            foreach (SearchFilter filter in filterGroup.Filters)
            {
                if (filter.Value == null)
                    continue;

                ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(filter.AppObjectUniqueId);
                ApttusField filterField = appObject.GetField(filter.FieldId);

                //if (filterField.Datatype == Datatype.Lookup || (appObject.Children.Count > 0 && filterField.Id == Constants.ID_ATTRIBUTE))
                if (filterField.Datatype == Datatype.Lookup ||filterField.Datatype == Datatype.Composite || filterField.Id == appObject.IdAttribute)
                {
                    // Add to Input Objects HashSet, if filter is marked as input only. As Static & SystemVariable can't have input objects.
                    if (filter.ValueType == ExpressionValueTypes.Input)
                    {
                        Guid LookupObjectGuid = Guid.Parse(filter.Value.Split(Constants.DOT.ToCharArray())[0]);
                        if (LookupObjectGuid != null)
                            objectSet.Add(ApplicationDefinitionManager.GetInstance.GetAppObject(LookupObjectGuid).UniqueId.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Get all objects and fields used in a filter group
        /// </summary>
        /// <param name="filterGroup">Filter group</param>
        /// <param name="fieldMap">Result map</param>
        public static void GetFilterGroupFields(SearchFilterGroup filterGroup, Dictionary<ApttusObject, HashSet<ApttusField>> fieldMap)
        {
            if (filterGroup.Groups != null)
            {
                foreach (SearchFilterGroup childGroup in filterGroup.Groups)
                    GetFilterGroupFields(childGroup, fieldMap);
            }

            foreach (SearchFilter filter in filterGroup.Filters)
            {
                if (filter.Value == null)
                    continue;

                ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(filter.AppObjectUniqueId);
                ApttusField filterField = appObject.GetField(filter.FieldId);

                if (filterField.Datatype == Datatype.Lookup || filterField.Id == appObject.IdAttribute)
                {
                    // Add field on which static where condition is defined
                    if (filter.ValueType == ExpressionValueTypes.Static || filter.ValueType == ExpressionValueTypes.SystemVariables)
                    {
                        ApttusObject filterObject = ApplicationDefinitionManager.GetInstance.GetAppObject(filter.AppObjectUniqueId);
                        if (fieldMap.ContainsKey(filterObject))
                            fieldMap[filterObject].Add(filterObject.GetField(filter.FieldId));
                        else
                            fieldMap.Add(filterObject, new HashSet<ApttusField> { filterObject.GetField(filter.FieldId) });
                    }
                    else if (filter.ValueType == ExpressionValueTypes.Input)
                    {
                        string[] split = filter.Value.Split(Constants.DOT.ToCharArray());
                        Guid LookupObjectGuid = Guid.Parse(split[0]);

                        if (LookupObjectGuid != null)
                        {
                            ApttusObject lookupObject = ApplicationDefinitionManager.GetInstance.GetAppObject(LookupObjectGuid);
                            if (fieldMap.ContainsKey(lookupObject))
                            {
                                ApttusField lookupObjectField = lookupObject.GetField(filter.FieldId) != null ? lookupObject.GetField(filter.FieldId) : lookupObject.GetField(split[1]);
                                fieldMap[lookupObject].Add(lookupObjectField);
                            }
                            else
                                fieldMap.Add(lookupObject, new HashSet<ApttusField> { lookupObject.GetField(split[1]) });
                        }
                    }
                }
                else
                {
                    if (fieldMap.ContainsKey(appObject))
                        fieldMap[appObject].Add(filterField);
                    else
                        fieldMap.Add(appObject, new HashSet<ApttusField> { filterField });
                }
            }
        }

        public static string GetDisplayTextFromQueryObjects(SearchFilter currentFilter)
        {
            string result = string.Empty;

            if (currentFilter != null && currentFilter.QueryObjects != null && currentFilter.QueryObjects.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var queryObject in currentFilter.QueryObjects)
                {
                    if (!(queryObject.AppObjectUniqueId.Equals(queryObject.LeafAppObjectUniqueId) && queryObject.RelationshipType == QueryRelationshipType.None))
                        sb.Append(GetDisplayNamesFromSingleQueryObject(queryObject));
                }
                sb.Append(applicationDefinitionManager.GetField(currentFilter.AppObjectUniqueId, currentFilter.FieldId).Name);
                result = sb.ToString();
            }
            return result;
        }

        private static string GetDisplayNamesFromSingleQueryObject(QueryObject queryObject)
        {
            StringBuilder sb = new StringBuilder();
            Guid leafAppObjectId = queryObject.LeafAppObjectUniqueId;

            if (queryObject.DistanceFromChild == 0)
            {
                if (queryObject.RelationshipType == QueryRelationshipType.Lookup)
                {
                    ApttusObject leafAppObject = applicationDefinitionManager.GetAppObject(leafAppObjectId);
                    var lookupAppField = leafAppObject.Fields.FirstOrDefault(f => f.Datatype == Datatype.Lookup && f.Id == queryObject.LookupFieldId);
                    sb.Append(lookupAppField.Name + Constants.DOT);
                }
            }
            else
            {
                for (int i = 0; i < queryObject.DistanceFromChild; i++)
                {
                    ApttusObject childAppObject = applicationDefinitionManager.GetAppObject(leafAppObjectId);
                    ApttusObject parentAppObject = childAppObject.Parent;
                    var childAppField = childAppObject.Fields.FirstOrDefault(f => f.Datatype == Datatype.Lookup && f.LookupObject.Id == parentAppObject.Id);
                    sb.Append(childAppField.Name + Constants.DOT);
                    leafAppObjectId = parentAppObject.UniqueId;
                }
            }
            return sb.ToString();
        }

        public static string GetLHSQueryTextFromQueryObjects(SearchFilter currentFilter)
        {
            string result = string.Empty;
            if (currentFilter != null && currentFilter.QueryObjects != null && currentFilter.QueryObjects.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var queryObject in currentFilter.QueryObjects)
                {
                    if (!(queryObject.AppObjectUniqueId.Equals(queryObject.LeafAppObjectUniqueId) && queryObject.RelationshipType == QueryRelationshipType.None))
                    {
                        // 1. Get all the object names if Distance is greater than 0 - Parent Relationship Type Scenario
                        if (queryObject.DistanceFromChild > 0)
                        {
                            sb.Append(GetQueryNamesFromSingleQueryObject(queryObject));
                            sb.Append(Constants.DOT);
                        }

                        // 2. Get the lookup object reference from lookup field id - Lookup Relationship Type Scenario
                        if (queryObject.RelationshipType == QueryRelationshipType.Lookup)
                        {
                            string lookupFieldForQuery = applicationDefinitionManager.GetLookupReference__R(queryObject.LookupFieldId);
                            sb.Append(lookupFieldForQuery);
                            sb.Append(Constants.DOT);
                        }
                    }
                }
                sb.Append(currentFilter.FieldId);
                result = sb.ToString();
            }
            return result;
        }

        private static string GetQueryNamesFromSingleQueryObject(QueryObject queryObject)
        {
            StringBuilder sb = new StringBuilder();
            Guid childAppObjectId = queryObject.LeafAppObjectUniqueId;
            bool bFirstLookupAdded = false;

            for (int i = 0; i < queryObject.DistanceFromChild; i++)
            {
                if (bFirstLookupAdded)
                    sb.Append(Constants.DOT);
                else
                    bFirstLookupAdded = true;

                ApttusObject childAppObject = applicationDefinitionManager.GetAppObject(childAppObjectId);
                ApttusObject parentAppObject = childAppObject.Parent;
                var childAppField = childAppObject.Fields.FirstOrDefault(f => f.Datatype == Datatype.Lookup && f.LookupObject.Id == parentAppObject.Id);
                sb.Append(applicationDefinitionManager.GetLookupReference__R(childAppField.Id));
                childAppObjectId = parentAppObject.UniqueId;
            }
            return sb.ToString();
        }

        public static bool GetExpression(string actionId, List<SearchFilterGroup> FilterGroups, ApttusObject SourceObject, string[] InputDataNames, out List<KeyValuePair<string, Guid>> UsedDataSets, out string fullWhereClause, out bool chunkValues, out List<string> fullWhereClauseChunks, bool parseUserInput = false)
        {
            List<string> values;
            fullWhereClause = string.Empty;
            bool result = GetPreparedExpression(actionId, FilterGroups, SourceObject, InputDataNames, out UsedDataSets, out values, out fullWhereClause, out chunkValues, out fullWhereClauseChunks, false, parseUserInput);

            if (result)
            {
                if (!chunkValues) // Non - Chunked operation to replace ? with actual values
                {
                    foreach (string val in values)
                    {
                        string valToInsert = val.Replace("$", "$$");//escape '$'. Expecting user to mean literal '$' and not to use regular expression injections. 
                        Regex regex = new Regex(Regex.Escape("?"));
                        fullWhereClause = regex.Replace(fullWhereClause, valToInsert, 1);
                        displayableWhereClause = regex.Replace(displayableWhereClause, valToInsert, 1);
                    }
                }
                else // Chunked opetiona to replace ? with actual values
                {
                    for (int i = 0; i < fullWhereClauseChunks.Count; i++)
                    {
                        foreach (string val in values)
                        {
                            Regex regex = new Regex(Regex.Escape("?"));
                            fullWhereClauseChunks[i] = regex.Replace(fullWhereClauseChunks[i], val, 1);
                        }
                    }
                }
            }
            return result;
        }

        public static bool GetPreparedExpression(string actionId, List<SearchFilterGroup> searchFilterGroups, ApttusObject SourceObject, string[] inputDataNames,
            out List<KeyValuePair<string, Guid>> UsedDataSets, out List<string> values, out string fullWhereClause, out bool chunkValues,
            out List<string> fullWhereClauseChunks, bool Designer = false, bool parseUserInput = false)
        {
            StringBuilder Expression = new StringBuilder();
            bool result = true;
            List<string> tempChunkRecordIds = null;
            bool isAnyFilterChunked = false;
            List<string> chunkedFilterRecordIds = null;
            StringBuilder WhereClauseNameFieldExpression = new StringBuilder();
            // Initialize all the out variables
            fullWhereClause = string.Empty;
            chunkValues = false;
            fullWhereClauseChunks = null;
            UsedDataSets = new List<KeyValuePair<string, Guid>>();
            values = new List<string>();

            // Based on the current implementation there will always be one Search Filter Group 
            if (searchFilterGroups != null && searchFilterGroups.Count == 1)
            {
                List<InputData> inputData = GetInputDataName(inputDataNames);
                SearchFilterGroup searchFilterGroup = searchFilterGroups.FirstOrDefault();
                string filterResult = string.Empty;
                bool bFirstFilterAdded = false;

                string filterLogic = string.Empty;
                if (String.IsNullOrEmpty(searchFilterGroup.FilterLogicText))
                    filterLogic = GetDefaultFilterLogic(searchFilterGroup);
                else
                    filterLogic = searchFilterGroup.FilterLogicText;

                filterLogicText = filterLogic;
                string whereClause = string.Empty;
                string whereClauseNameField = string.Empty;

                string RegExPattern = @"(and)+|(or)+|\(|\)|[0-9]+";
                foreach (Match matchFilterLogicChar in Regex.Matches(filterLogic, RegExPattern, RegexOptions.IgnoreCase))
                {
                //foreach (char filterLogicChar in filterLogic)
                //{
                    int sequenceNo;
                    if (matchFilterLogicChar.Value == Constants.OPEN_BRACKET)
                    {
                        Expression.Append(Constants.OPEN_BRACKET);
                        WhereClauseNameFieldExpression.Append(Constants.OPEN_BRACKET);
                    }
                    else if (int.TryParse(matchFilterLogicChar.Value, out sequenceNo))
                    {
                        SearchFilter searchFilter = searchFilterGroup.Filters.FirstOrDefault(f => f.SequenceNo == sequenceNo);
                        // If filter is of type User Input and Evaluate flag is flase continue.
                        if (searchFilter.ValueType == ExpressionValueTypes.UserInput && !parseUserInput)
                        {
                            // In case when we use Userinput in S&S action OR Query Action with Advance filters than no need skip loop                            
                            if (!searchFilterGroup.IsAddFilter)
                                continue;
                        }
                        whereClause = GetSingleWhereClause(actionId, searchFilter, SourceObject, inputData, ref UsedDataSets, ref values, Designer, searchFilterGroup, out chunkValues, out tempChunkRecordIds, out whereClauseNameField);

                        if (chunkValues)
                        {
                            if (isAnyFilterChunked)
                            {
                                //We cannot chunk multiple filter values. More than one filter which has crossed the limit of 20K cannot be chunked.
                                ExceptionLogHelper.DebugLog("\nSOQL Over-Limit: More than one filter has crossed the limit of 20K. Hence chunking cannot be used. Please use dataset action.");
                                result = false;
                                break;
                            }
                            else
                            {
                                isAnyFilterChunked = chunkValues;
                                chunkedFilterRecordIds = tempChunkRecordIds;
                            }
                        }
                        // In case whereClause is Blank for UserInput type of filter, skip to the next expression - User did not specify any value.
                        if (string.IsNullOrEmpty(whereClause) && (searchFilter.ValueType == ExpressionValueTypes.UserInput ||
                                                                  searchFilter.ValueType == ExpressionValueTypes.CellReference))
                            continue;
                        else if (string.IsNullOrEmpty(whereClause))
                            return false;
                        else
                        {
                            Expression.Append(whereClause);
                            if (!chunkValues)
                                WhereClauseNameFieldExpression.Append(whereClauseNameField);
                        }
                    }
                    else if (!string.IsNullOrEmpty(whereClause) && matchFilterLogicChar.Value.ToUpper() == Constants.AND) // AND Clause
                    {
                        Expression.Append(Constants.SPACE + Constants.AND + Constants.SPACE);
                        whereClause = string.Empty;
                        if (!chunkValues)
                        {
                            WhereClauseNameFieldExpression.Append(Constants.SPACE).Append(Constants.AND).Append(Constants.SPACE).Append("\n");
                            whereClauseNameField = string.Empty;
                        }
                    }
                    else if (!string.IsNullOrEmpty(whereClause) && matchFilterLogicChar.Value.ToUpper() == Constants.OR) // OR Clause
                    {
                        Expression.Append(Constants.SPACE + Constants.OR + Constants.SPACE);
                        whereClause = string.Empty;
                        if (!chunkValues)
                        {
                            WhereClauseNameFieldExpression.Append(Constants.SPACE).Append(Constants.OR).Append(Constants.SPACE).Append("\n");
                            whereClauseNameField = string.Empty;
                        }
                    }
                    else if (matchFilterLogicChar.Value == Constants.CLOSE_BRACKET)
                    {
                        Expression.Append(Constants.CLOSE_BRACKET);
                        if (!chunkValues)
                            WhereClauseNameFieldExpression.Append(Constants.CLOSE_BRACKET);
                    }
                }

                if (Expression.ToString().EndsWith(Constants.SPACE + Constants.AND + Constants.SPACE + Constants.CLOSE_BRACKET))
                {
                    Expression.Remove(Expression.Length - 6, 5);
                    WhereClauseNameFieldExpression.Remove(WhereClauseNameFieldExpression.Length - 6, 5);
                }

                else if (Expression.ToString().EndsWith(Constants.SPACE + Constants.OR + Constants.SPACE))
                {
                    Expression.Remove(Expression.Length - 5, 4);
                    WhereClauseNameFieldExpression.Remove(WhereClauseNameFieldExpression.Length - 5, 4);
                }
                // As of now Search Filter Groups are not added as a child of a Search Filter Group so the below block is more for future use.
                if (searchFilterGroup.Groups != null && searchFilterGroup.Groups.Count > 0)
                {
                    if (bFirstFilterAdded)
                        Expression.Append(Utils.GetEnumDescription(searchFilterGroup.LogicalOperator, string.Empty));
                    Expression.Append(GetExpression(actionId, searchFilterGroup.Groups, SourceObject, inputDataNames, out UsedDataSets, out fullWhereClause, out chunkValues, out fullWhereClauseChunks));
                    bFirstFilterAdded = true;
                }
            }

            string expressionString = Expression.ToString();
            string originalWhereClause = expressionString.Equals("()") ? string.Empty : expressionString;

            if (!isAnyFilterChunked) // Non - Chunked operation to return single where clause.
            {
                fullWhereClause = originalWhereClause;
                displayableWhereClause = expressionString.Equals("()") ? string.Empty : WhereClauseNameFieldExpression.ToString();
            }
            else if(result == true)
            {
                WhereClauseNameFieldExpression.Clear();
                // Chunking of SOQL queries based on "IN" and "NOT IN" input filter to overcome 20K Salesforce Limitation.
                // This will create "fullWhereClauseChunks" collection
                chunkValues = true;              

                List<int> whereClauseChunkSizes = CreateWhereClauseChunkSizes(chunkedFilterRecordIds);
                fullWhereClauseChunks = new List<string>(whereClauseChunkSizes.Count);

                int TotalChunks = whereClauseChunkSizes.Count;
                int ChunkSize = 0;
                for (int iChunkIndex = 0; iChunkIndex < TotalChunks; ++iChunkIndex)
                {
                    string recordValuesChunk = string.Join(Constants.COMMA, chunkedFilterRecordIds.GetRange(ChunkSize, whereClauseChunkSizes[iChunkIndex]));
                    fullWhereClauseChunks.Add(originalWhereClause.Replace(Constants.CHUNK_PLACEHOLDER, recordValuesChunk));
                    ChunkSize = ChunkSize + whereClauseChunkSizes[iChunkIndex];
                }
            }
            return result;
        }

        private static List<int> CreateWhereClauseChunkSizes(List<string> chunkedFilterRecords)
        {
            List<int> chunkSizes = new List<int>(10);
            int recordLength = 0;
            int nRecords = 0;

            foreach (string record in chunkedFilterRecords)
            {
                recordLength = recordLength + record.Length;
                if (recordLength == 12750) //Chunking of Record Ids with the the length of 17 characters and 750 records.
                {
                    chunkSizes.Add(nRecords + 1);
                    recordLength = 0;
                    nRecords = 0;
                }
                else if (recordLength > 12750)
                {
                    chunkSizes.Add(nRecords);
                    recordLength = record.Length;
                    nRecords = 1;
                }
                else
                    ++nRecords;
            }

            if (nRecords > 0)
                chunkSizes.Add(nRecords);

            return chunkSizes;
        }

        public static string GetDefaultFilterLogic(SearchFilterGroup filterGroup)
        {
            string result = string.Empty;
            if (filterGroup != null && filterGroup.Filters != null && filterGroup.Filters.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Constants.OPEN_BRACKET);
                sb.Append(string.Join((Constants.SPACE + Constants.AND + Constants.SPACE),
                    (from sf in filterGroup.Filters
                     select sf.SequenceNo.ToString())));
                sb.Append(Constants.CLOSE_BRACKET);

                result = sb.ToString();
            }
            return result;
        }

        private static string GetSingleWhereClause(string actionId, SearchFilter searchFilter, ApttusObject SourceObject, List<InputData> inputData,
            ref List<KeyValuePair<string, Guid>> UsedDataSets, ref List<string> values, bool Designer, SearchFilterGroup searchFilterGroup, out bool chunkValues, out List<string> chunkRecordIds, out string whereClauseNameField)
        {
            string result = string.Empty;
            string filterResult = string.Empty;
            chunkRecordIds = null;
            bool isValidWhereClause = true;
            chunkValues = false;
            // Object and Field (Left Hand Side)
            ApttusObject apttusObject = null;
            string LHSObjectField = string.Empty;
            string notOperator = string.Empty;
            whereClauseNameField = string.Empty;

            if (searchFilter.QueryObjects == null || searchFilter.QueryObjects.Count == 0)
            {
                // Backward Compatibility Code for Query built before "Query Builder Enhancement #1"
                apttusObject = (searchFilter.AppObjectUniqueId == SourceObject.UniqueId
                    ? SourceObject : applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId));

                // Field (Left Hand Side)

                // In case of Parent or Child query
                if (searchFilter.AppObjectUniqueId != SourceObject.UniqueId)
                {
                    LHSObjectField += applicationDefinitionManager.GetAppObject(searchFilter.AppObjectUniqueId).Id;
                    LHSObjectField += Constants.DOT;
                }
                LHSObjectField += searchFilter.FieldId;
            }
            else
            {
                // Set the Apttus Object
                apttusObject = applicationDefinitionManager.GetAppObject(searchFilter.QueryObjects.Last().AppObjectUniqueId);

                // Get the N level left hand side query
                LHSObjectField = ExpressionBuilderHelper.GetLHSQueryTextFromQueryObjects(searchFilter);
            }

            Datatype dataType = apttusObject.Fields.FirstOrDefault(f => f.Id == searchFilter.FieldId).Datatype;
            
            // Allow formula data type for string , numeric and boolean for where clause.
            if (dataType == Datatype.Formula)
            {
                string formulaType = apttusObject.Fields.FirstOrDefault(f => f.Id == searchFilter.FieldId).FormulaType;
                if (!string.IsNullOrEmpty(formulaType))
                {
                    dataType = GetDataTypeFromFormulaType(formulaType);
                }
            }
            // Operator
            string Operator = searchFilter.Operator;

            // Value (Right Hand Side)
            bool invalidDataType = false;
            if (searchFilter.ValueType == ExpressionValueTypes.Static)
            {
                // Escape and Get Filter Value
                filterResult = EscapeAndGetFilterValue(searchFilter.Value, Operator, dataType, ValueSeparator.MultiLine, true, out invalidDataType,
                    out chunkValues, out chunkRecordIds);
                if (invalidDataType)
                    isValidWhereClause = false;
                else if (string.IsNullOrEmpty(filterResult))
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.SystemVariables)
            {
                filterResult = getSysVariableFilterValue(searchFilter, dataType);
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.Input)
            {
                if (dataType == Datatype.Picklist)
                {
                    // Backward Compatibility - ValueType is Input for Picklist will be treated as Static.
                    filterResult = EscapeAndGetFilterValue(searchFilter.Value, Operator, dataType, ValueSeparator.MultiLine, true, out invalidDataType,
                        out chunkValues, out chunkRecordIds);
                    if (invalidDataType)
                        isValidWhereClause = false;
                    else if (string.IsNullOrEmpty(filterResult))
                        filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
                }
                else
                {
                    isValidWhereClause = getInputFilterValue(searchFilter, SourceObject, inputData, ref UsedDataSets, out filterResult, out chunkValues, out chunkRecordIds, Designer);
                    // If there is no data is passed for Input dataset, insert a FALSE condition like
                    // "Id = NULL" or "Id in (NULL)". Set LHS Object Field as "Id"
                    // In case of SubQuery, replacement of LHSObjectField should not happen.
                    if (filterResult.Contains("NULL") && !UseSubQuery)
                        LHSObjectField = Constants.ID_ATTRIBUTE;
                }
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
                        filterResult = EscapeAndGetFilterValue(Convert.ToString(dr[Constants.VALUE_COLUMN]), Operator, dataType, ValueSeparator.MultiLine, false,
                            out invalidDataType, out chunkValues, out chunkRecordIds);

                        if (invalidDataType || string.IsNullOrEmpty(filterResult))
                            isValidWhereClause = false;
                    }
                }
                // when we use Userinput in S&S action OR Query Action with Advance filters than get default values and pass in query
                if (searchFilterGroup.IsAddFilter && !isValidWhereClause && string.IsNullOrEmpty(filterResult))
                {
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
                    isValidWhereClause = true;
                }
            }
            else if (searchFilter.ValueType == ExpressionValueTypes.CellReference)
            {
                // Escape and Get Filter Value                
                filterResult = EscapeAndGetFilterValue(searchFilter.Value, Operator, dataType, ValueSeparator.MultiLine, true, out invalidDataType,
                    out chunkValues, out chunkRecordIds, cellReferenceValueType: Constants.CELLREFERENCE);

                if (invalidDataType)
                    isValidWhereClause = false;
                if (string.IsNullOrEmpty(filterResult))
                    filterResult = Utils.GetDefaultAndNullValues(dataType, Operator);
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
                if (!chunkValues)
                {
                    values.Add(filterResult);
                    // Append "Not" for not contains clause
                    if (notOperator.Equals("like %#NOTFILTERVALUE%"))
                        result = Constants.OPEN_BRACKET + "Not" + Constants.SPACE + LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + "?" + Constants.CLOSE_BRACKET;
                    else
                        result = LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + "?";

                    //string LHSObjectFieldName = apttusObject.Fields.Find(fld => fld.Id == searchFilter.FieldId).Name;
                    string LHSObjectFieldName = getFieldName(apttusObject, SourceObject, searchFilter);

                    if (notOperator.Equals("like %#NOTFILTERVALUE%"))
                        whereClauseNameField = Constants.OPEN_BRACKET + "Not" + Constants.SPACE + LHSObjectFieldName + Constants.SPACE + TranslateOperatorToDisplay(notOperator) + Constants.SPACE + "?" + Constants.CLOSE_BRACKET;
                    else
                    {
                        string filterResultValue = "?";
                        if (searchFilter.ValueType == ExpressionValueTypes.SystemVariables && searchFilter.Value.Equals(ExpressionSystemVariables.CurrentUser.ToString()))
                        {
                            ApttusUserInfo userInfo = objectManager.getUserInfo();
                            if (userInfo != null)
                                filterResultValue = userInfo.UserFullName;
                        }
                        whereClauseNameField = LHSObjectFieldName + Constants.SPACE + TranslateOperatorToDisplay(Operator) + Constants.SPACE + filterResultValue;
                    }
                }
                else // Chunking of SQL queries
                {
                    // Append "Not" for not contains clause
                    if (notOperator.Equals("like %#NOTFILTERVALUE%"))
                        result = Constants.OPEN_BRACKET + "Not" + Constants.SPACE + LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + filterResult + Constants.CLOSE_BRACKET;
                    else
                        result = LHSObjectField + Constants.SPACE + Operator + Constants.SPACE + filterResult;
                }
            }
            else
                result = string.Empty;

            return result;
        }

        private static string getFieldName(ApttusObject searchFilterObjectId, ApttusObject targetObjectUniqueId, SearchFilter filter)
        {
            if (searchFilterObjectId.UniqueId == targetObjectUniqueId.UniqueId)
            {
                ApttusField field = targetObjectUniqueId.Fields.Find(fld => fld.Id == filter.FieldId);
                if (field != null)
                    return field.Name;
                else
                    return filter.SearchFilterLabel;
            }
            else
                return filter.SearchFilterLabel;
        }

        

        private static string TranslateOperatorToDisplay(string displayOperator)
        {
            string value = displayOperator;
            switch(displayOperator)
            {
                case "LIKE":
                    value = "contains";
                    break;
                case "like %#NOTFILTERVALUE%":
                    value = "does not contains";
                    break;                
            }
            return value;
        }
        public static string EscapeAndGetFilterValue(string filterValue, string filterOperator, Datatype fieldDataType, bool overrideappendQuote, ValueSeparator valueSeparator, bool blankOrNullAllowed, out bool invalidDataType, string cellReferenceValueType = Constants.CELLREFERENCE)
        {
            string result = string.Empty;
            invalidDataType = false;

            bool appendQuote = fieldDataType.Equals(Datatype.Date) || fieldDataType.Equals(Datatype.DateTime) ||
                                fieldDataType.Equals(Datatype.Double) || fieldDataType.Equals(Datatype.Decimal) ||
                                fieldDataType.Equals(Datatype.Boolean) ? false : true;

            // For dynamics string values do not need to append Quote
            appendQuote = overrideappendQuote ? false : appendQuote;

            if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
            {
                StringBuilder sbResult = new StringBuilder();
                var separator = string.Empty;
                if (valueSeparator == ValueSeparator.CommaSeparated)
                    separator = Constants.COMMA;
                else if (valueSeparator == ValueSeparator.MultiLine)
                    separator = Environment.NewLine;

                if (!string.IsNullOrEmpty(filterValue) && filterValue.LastIndexOf(separator) > 0)
                {
                    bool bFirstVal = true;

                    sbResult.Append(Constants.OPEN_BRACKET);
                    HashSet<string> uniqueValues = new HashSet<string>(from r in filterValue.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                                       where !string.IsNullOrEmpty(r)
                                                                       select r.Trim());
                    foreach (string uniqueValue in uniqueValues)
                    {
                        if (!bFirstVal && !sbResult.ToString().Equals(Constants.OPEN_BRACKET))
                            sbResult.Append(Constants.COMMA);

                        sbResult.Append(Utils.GetValidValue(uniqueValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                        if (cellReferenceValueType.Equals(Constants.CELLREFERENCE) && uniqueValue.Equals("''"))
                            sbResult.Append(Utils.SetNullValues(fieldDataType, filterOperator));
                        else if (cellReferenceValueType.Equals(Constants.CELLREFERENCE) && invalidDataType)
                        {
                            string strResult = string.Empty;
                            strResult = sbResult.ToString().Trim().TrimEnd(Constants.COMMA.ToCharArray());
                            sbResult.Clear();
                            sbResult.Append(strResult);
                            invalidDataType = false;
                        }
                        else if (invalidDataType)
                            return string.Empty;

                        bFirstVal = false;
                    }
                    sbResult.Append(Constants.CLOSE_BRACKET);
                }
                else if (!string.IsNullOrEmpty(filterValue) || (string.IsNullOrEmpty(filterValue) && blankOrNullAllowed))
                {
                    sbResult.Append(Constants.OPEN_BRACKET);
                    sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                    sbResult.Append(Constants.CLOSE_BRACKET);
                }

                result = sbResult.ToString();
            }
            else if (filterOperator.Equals("includes ('#FILTERVALUE')"))
            {
                StringBuilder sbResult = new StringBuilder(Constants.OPEN_BRACKET);
                sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                sbResult.Append(Constants.CLOSE_BRACKET);
                result = sbResult.ToString();
            }
            else if (filterOperator.Equals("excludes ('#FILTERVALUE')"))
            {
                StringBuilder sbResult = new StringBuilder(Constants.OPEN_BRACKET);
                sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                sbResult.Append(Constants.CLOSE_BRACKET);
                result = sbResult.ToString();
            }
            else if (!string.IsNullOrEmpty(filterValue) && (filterOperator.Equals("like #FILTERVALUE%") || filterOperator.Equals("like %#FILTERVALUE%") || filterOperator.Equals("like %#NOTFILTERVALUE%")))
            {
                StringOperations stringOperation = filterOperator.Equals("like #FILTERVALUE%") ? StringOperations.StartWith : StringOperations.Contains;
                result = Utils.GetValidValue(filterValue, fieldDataType, appendQuote, stringOperation, out invalidDataType);

                //result = filterOperator.Equals("like #FILTERVALUE%") ? filterValue + "%" : "%" + filterValue + "%";
                //Operator = "LIKE";
            }
            else if (!string.IsNullOrEmpty(filterValue))
                result = Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterValue"></param>
        /// <param name="filterOperator"></param>
        /// <param name="fieldDataType"></param>
        /// <returns></returns>
        public static string EscapeAndGetFilterValue(string filterValue, string filterOperator, Datatype fieldDataType, ValueSeparator valueSeparator, bool blankOrNullAllowed,
                                    out bool invalidDataType, out bool ChunkValues, out List<string> inputStringValues, string cellReferenceValueType = Constants.CELLREFERENCE)
        {
            string result = string.Empty;
            invalidDataType = false;
            ChunkValues = false;
            inputStringValues = null;

            bool appendQuote = fieldDataType.Equals(Datatype.Date) || fieldDataType.Equals(Datatype.DateTime) ||
                                fieldDataType.Equals(Datatype.Double) || fieldDataType.Equals(Datatype.Decimal) ||
                                fieldDataType.Equals(Datatype.Boolean) ? false : true;

            if (filterOperator.ToUpper().Equals("IN") || filterOperator.ToUpper().Equals("NOT IN"))
            {
                StringBuilder sbResult = new StringBuilder();
                var separator = string.Empty;
                if (valueSeparator == ValueSeparator.CommaSeparated)
                    separator = Constants.COMMA;
                else if (valueSeparator == ValueSeparator.MultiLine)
                    separator = Environment.NewLine;

                if (!string.IsNullOrEmpty(filterValue) && filterValue.LastIndexOf(separator) > 0)
                {
                    bool bFirstVal = true;

                    sbResult.Append(Constants.OPEN_BRACKET);
                    HashSet<string> uniqueValues = new HashSet<string>(from r in filterValue.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                                       where !string.IsNullOrEmpty(r)
                                                                       select r.Trim());

                    if (UseChunking && string.Join(",", uniqueValues).Length >= Constants.WHERECLAUSE_PLUS_COMMACHARS_LENGTH /* && uniqueValues.Count > Constants.MAX_RECORDID_COUNT_BEFORE_CHUNKING */)
                    {
                        ChunkValues = true;
                        inputStringValues = new List<string>();
                        sbResult.Append(Constants.CHUNK_PLACEHOLDER);
                    }

                    foreach (string uniqueValue in uniqueValues)
                    {
                        if (ChunkValues)
                        {
                            string value = Utils.GetValidValue(uniqueValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType);
                            if (!invalidDataType)
                                inputStringValues.Add(value);
                            else if (cellReferenceValueType.Equals(Constants.CELLREFERENCE) && uniqueValue.Equals("''") && fieldDataType != Datatype.Lookup)
                                inputStringValues.Add(Utils.SetNullValues(fieldDataType, filterOperator));
                        }
                        else
                        {
                            if (!bFirstVal)
                                sbResult.Append(Constants.COMMA);

                            sbResult.Append(Utils.GetValidValue(uniqueValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                            if (cellReferenceValueType.Equals(Constants.CELLREFERENCE) && uniqueValue.Equals("''"))
                                sbResult.Append(Utils.SetNullValues(fieldDataType, filterOperator));
                            else if (cellReferenceValueType.Equals(Constants.CELLREFERENCE) && invalidDataType)
                            {
                                string strResult = string.Empty;
                                strResult = sbResult.ToString().Trim().TrimEnd(Constants.COMMA.ToCharArray());
                                sbResult.Clear();
                                sbResult.Append(strResult);
                                invalidDataType = false;
                            }
                            else if (invalidDataType)
                                return string.Empty;

                            bFirstVal = false;
                        }
                    }
                    sbResult.Append(Constants.CLOSE_BRACKET);
                }
                else if (!string.IsNullOrEmpty(filterValue) || (string.IsNullOrEmpty(filterValue) && blankOrNullAllowed))
                {
                    sbResult.Append(Constants.OPEN_BRACKET);
                    sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                    sbResult.Append(Constants.CLOSE_BRACKET);
                }

                result = sbResult.ToString();
            }
            else if (filterOperator.Equals("includes ('#FILTERVALUE')"))
            {
                StringBuilder sbResult = new StringBuilder(Constants.OPEN_BRACKET);
                sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                sbResult.Append(Constants.CLOSE_BRACKET);
                result = sbResult.ToString();
            }
            else if (filterOperator.Equals("excludes ('#FILTERVALUE')"))
            {
                StringBuilder sbResult = new StringBuilder(Constants.OPEN_BRACKET);
                sbResult.Append(Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType));
                sbResult.Append(Constants.CLOSE_BRACKET);
                result = sbResult.ToString();
            }
            else if (!string.IsNullOrEmpty(filterValue) && (filterOperator.Equals("like #FILTERVALUE%") || filterOperator.Equals("like %#FILTERVALUE%") || filterOperator.Equals("like %#NOTFILTERVALUE%")))
            {
                StringOperations stringOperation = filterOperator.Equals("like #FILTERVALUE%") ? StringOperations.StartWith : StringOperations.Contains;
                result = Utils.GetValidValue(filterValue, fieldDataType, appendQuote, stringOperation, out invalidDataType);

                //result = filterOperator.Equals("like #FILTERVALUE%") ? filterValue + "%" : "%" + filterValue + "%";
                //Operator = "LIKE";
            }
            else if (!string.IsNullOrEmpty(filterValue))
                result = Utils.GetValidValue(filterValue, fieldDataType, appendQuote, StringOperations.Default, out invalidDataType);

            return result;
        }

        /// <summary>
        /// Resolve input filters
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="inputDataNames">Input names</param>
        /// <param name="usedDataSets">Used datasets</param>
        /// <returns>Filter value</returns>
        private static bool getInputFilterValue(SearchFilter filter, ApttusObject SourceObject, List<InputData> inputData, ref List<KeyValuePair<string, Guid>> usedDataSets,
            out string inputFilterValue, out bool chunkValues, out List<string> inputFilterRecordIds, bool Designer = false)
        {
            DataManager dataManager = DataManager.GetInstance;
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

            inputFilterValue = string.Empty;
            inputFilterRecordIds = null;
            chunkValues = false;
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
                inputNme = GetInputDataName(inputData.Select(id => id.Name).ToArray(), inputObject);
            }
            else
            {
                if (inputData != null)
                    dataSet = dataManager.ResolveInput(inputData, inputObject);

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

                string currentDataSOQL = dataSet.SOQL;
                // use subquery if sub query flag is true and OR operator is not used
                if (UseSubQuery && (!string.IsNullOrEmpty(currentDataSOQL) && !filterLogicText.Contains(Constants.OR)))
                {
                    string selectIdSOQL = currentDataSOQL.Substring(currentDataSOQL.LastIndexOf("FROM"));
                    string selectFieldId = split[1];
                    selectIdSOQL = "Select " + selectFieldId + " " + selectIdSOQL;
                    ContainsExpression.Append(selectIdSOQL);
                }
                else
                {
                    foreach (DataRow dr in dataSet.DataTable.Rows)
                    {
                        uniqueValues.Add(Constants.QUOTE + Utils.ConvertTo15CharRecordId(dr[inputField].ToString()) + Constants.QUOTE);
                    }

                    // UseChunking property is set to true by the caller.
                    if (UseChunking && string.Join(",", uniqueValues).Length >= Constants.WHERECLAUSE_PLUS_COMMACHARS_LENGTH /* && uniqueValues.Count > Constants.MAX_RECORDID_COUNT_BEFORE_CHUNKING */)
                    {
                        chunkValues = true;
                        inputFilterRecordIds = uniqueValues.ToList();
                        ContainsExpression.Append(Constants.CHUNK_PLACEHOLDER);
                    }
                    else
                    {
                        ContainsExpression.Append(string.Join(Constants.COMMA, uniqueValues));
                    }
                }

                ContainsExpression.Append(Constants.CLOSE_BRACKET);
                inputFilterValue = ContainsExpression.ToString();
                //Memory Optimization
                uniqueValues.Clear();
                uniqueValues = null;
            }
            else
            {
                if (string.IsNullOrEmpty(inputNme))
                {
                    usedDataSets.Add(new KeyValuePair<string, Guid>(dataSet.Name, dataSet.Id));
                    ContainsExpression.Append(Constants.QUOTE);
                    ContainsExpression.Append(dataSet.DataTable.Rows[0][inputField].ToString());
                    ContainsExpression.Append(Constants.QUOTE);
                }
                else
                {
                    ContainsExpression.Append(inputNme + "." + inputField.ToString());
                }

                inputFilterValue = ContainsExpression.ToString();
            }
            ContainsExpression.Clear();
            ContainsExpression.Capacity = 0;
            ContainsExpression = null;
            return result;
        }

        /// <summary>
        /// Resolve system variable
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>Filter value</returns>
        public static string getSysVariableFilterValue(SearchFilter filter, Datatype dataType)
        {
            if (filter.Value.Equals(ExpressionSystemVariables.CurrentUser.ToString()))
                return (Constants.QUOTE + Constants.CURRENT_USER_ID + Constants.QUOTE);
            else if (filter.Value.Equals(ExpressionSystemVariables.CurrentDate.ToString()))
            {
                if (dataType.Equals(Datatype.Date))
                    return System.DateTime.Today.ToString("yyyy-MM-dd");
                else if (dataType.Equals(Datatype.DateTime))
                    return System.DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00Z");
                else
                    return filter.Value;
            }
            else if (filter.Value.Equals(ExpressionSystemVariables.ExportRecordId.ToString()))
            {
                if (DataManager.GetInstance.StartParameters != null)
                {
                    if (string.IsNullOrEmpty(DataManager.GetInstance.StartParameters.ExportRecordId))
                        throw new Exception("Export Record ID not set");
                    return (Constants.QUOTE + DataManager.GetInstance.StartParameters.ExportRecordId + Constants.QUOTE);
                }
                else // app might have opened in xauthor , external id wont be available, send dummmy acct
                {
                    return (Constants.QUOTE + Constants.CURRENT_USER_ID + Constants.QUOTE);
                }
            }
            else
                return filter.Value;
        }

        public static string GetInputDataName(string[] inputDataNames, ApttusObject inputObject)
        {
            List<Workflow> WFs = null;
            WFs = configurationManager.Workflows;

            foreach (string id in inputDataNames)
            {
                Guid guid = new Guid(id);
                List<WorkflowActionData> wfactdta = (from wf in WFs
                                                     from s in ((WorkflowStructure)wf).Steps
                                                     from cond in s.Conditions
                                                     from act in cond.WorkflowActions
                                                     where act.WorkflowActionData.Id.Equals(guid)
                                                     select act.WorkflowActionData).ToList();

                if (wfactdta != null & wfactdta.Count > 0)
                {
                    if (wfactdta[0].AppObjectId.Equals(inputObject.UniqueId))
                        return wfactdta[0].OutputDataName;

                }
            }
            return null;
        }

        public static string GetLikeWhereClauseBasedOnDataType(string searchCriteria, ApttusField apttusField)
        {
            string LHSPlusOperator = string.Empty, value = string.Empty, whereClause = string.Empty;
            GetLikeWhereClauseBasedOnDataType(searchCriteria, apttusField.Id, apttusField.Datatype, ref LHSPlusOperator, ref value, ref whereClause);
            return whereClause;
        }

        public static string GetLikeWhereClauseBasedOnDataType(string searchCriteria, string LHSField, Datatype datatype)
        {
            string LHSPlusOperator = string.Empty, value = string.Empty, whereClause = string.Empty;
            GetLikeWhereClauseBasedOnDataType(searchCriteria, LHSField, datatype, ref LHSPlusOperator, ref value, ref whereClause);
            return whereClause;
        }

        public static void GetLikeWhereClauseBasedOnDataType(string searchCriteria, string LHSField, Datatype datatype, ref string LHSPlusOperator, ref string value, ref string whereClause)
        {
            switch (datatype)
            {
                case Datatype.Date:
                case Datatype.DateTime:
                    string validDate = Utils.IsValidDate(searchCriteria, datatype);
                    if (!string.IsNullOrEmpty(validDate))
                    {
                        whereClause = LHSField + " = " + validDate;
                        LHSPlusOperator = LHSField + " = ?";
                        value = validDate;
                    }
                    break;
                case Datatype.Decimal:
                case Datatype.Double:
                    Decimal validNumber;
                    if (Decimal.TryParse(searchCriteria, out validNumber))
                    {
                        whereClause = LHSField + " = " + validNumber;
                        LHSPlusOperator = LHSField + " = ?";
                        value = validNumber.ToString();
                    }
                    break;
                case Datatype.String:
                case Datatype.Picklist:
                case Datatype.Email:
                    whereClause = LHSField + " like '%" + searchCriteria + "%'";
                    LHSPlusOperator = LHSField + " like '%?%'";
                    value = searchCriteria;
                    break;
                // Lookup Id field need to be exact match.
                case Datatype.Lookup:
                    if (!string.IsNullOrEmpty(searchCriteria))
                    {
                        whereClause = LHSField + " = '" + searchCriteria + "'";
                        LHSPlusOperator = LHSField + " = '?'";
                        value = searchCriteria;
                    }
                    break;
                // Multi-Select Picklist need to be exact match. Like doesn't work for Mult-Select Picklist
                case Datatype.Picklist_MultiSelect:
                    whereClause = LHSField + " = '" + searchCriteria + "'";
                    LHSPlusOperator = LHSField + " = '?'";
                    value = searchCriteria;
                    break;
                // Added case for Boolean as it was missing after logic change, it compares without quotes
                case Datatype.Boolean:
                    bool isBoolean;
                    if (Boolean.TryParse(searchCriteria, out isBoolean))
                    {
                        whereClause = LHSField + " = " + searchCriteria + " ";
                        LHSPlusOperator = LHSField + " = ?";
                        value = searchCriteria;
                    }
                    break;
                case Datatype.NotSupported:
                case Datatype.Textarea:
                    break;
                default:
                    break;
            }
        }

        public static ApttusObject GetSearchFilterObject(SearchFilter searchFilter)
        {
            ApttusObject apttusObject = null;
            if (searchFilter != null && searchFilter.FieldId != null && searchFilter.QueryObjects != null && searchFilter.QueryObjects.Count > 0)
            {
                Guid leafAppObject = searchFilter.QueryObjects.Last().AppObjectUniqueId;
                if (leafAppObject != null && !leafAppObject.Equals(Guid.Empty))
                {
                    apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(leafAppObject);
                }
            }
            return apttusObject;
        }

        public static ApttusField GetSearchFilterField(SearchFilter searchFilter)
        {
            ApttusField apttusField = null;
            if (searchFilter != null && searchFilter.FieldId != null && searchFilter.QueryObjects != null && searchFilter.QueryObjects.Count > 0)
            {
                Guid leafAppObject = searchFilter.QueryObjects.Last().AppObjectUniqueId;
                if (leafAppObject != null && !leafAppObject.Equals(Guid.Empty))
                {
                    ApttusObject apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(leafAppObject);
                    apttusField = (ApttusField)apttusObject.Fields.FirstOrDefault(f => f.Id == searchFilter.FieldId);
                }
            }
            return apttusField;
        }

        public static List<InputData> GetInputDataName(string[] inputDataNames)
        {
            List<InputData> result = new List<InputData>();
            if (inputDataNames != null)
                foreach (var inputDataName in inputDataNames)
                    result.Add(new InputData { Name = inputDataName, Accessed = false });

            return result;
        }

        public static Datatype GetDataTypeFromFormulaType(string formulaType)
        {
            Datatype dataType = Datatype.AnyType;

            if (!string.IsNullOrEmpty(formulaType))
            {
                if (formulaType.Equals(Constants.FORMULATYPESTRING))
                    dataType = Datatype.String;
                else if (formulaType.Equals(Constants.FORMULATYPEDOUBLE))
                    dataType = Datatype.Double;
                else if (formulaType.Equals(Constants.FORMULATYPEBOOLEAN))
                    dataType = Datatype.Boolean;
                else if (formulaType.Equals(Constants.FORMULATYPECURRENCY))
                    dataType = Datatype.Double;
                else if (formulaType.Equals(Constants.FORMULATYPEPERCENT))
                    dataType = Datatype.Double;
                else if (formulaType.Equals(Constants.FORMULATYPEDATE))
                    dataType = Datatype.Date;
            }

            return dataType;
        }

    }
}
