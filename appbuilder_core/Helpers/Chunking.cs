using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Apttus.XAuthor.Core
{
    public class ChunkQueries
    {
        private string fullQuery;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private DataManager dataManager = DataManager.GetInstance;

        public ChunkQueries()
        {

        }

        public ChunkQueries(string fullQueryWithWhereClause)
        {
            fullQuery = fullQueryWithWhereClause;
            int lenStart = fullQuery.IndexOf("('");
            fullQuery = fullQuery.Substring(0, lenStart);
        }

        /// <summary>
        /// chunk queries using list of values passed and query passed to it with consideration of 20K limitations.
        /// </summary>
        /// <param name="values">list of values needs to be filled in where clause</param>
        /// <param name="queryWithoutWhereClause">query without where clause</param>
        /// <param name="filtercolumnName">column name which shall be used as filter column</param>
        /// <returns></returns>
        public List<string> PrepareChunkedQueriesFromList(List<string> values, string queryWithoutWhereClause, string filtercolumnName)
        {
            List<string> chunkedQueries = new List<string>();
            int maxSizeofWhereClause = Constants.SOQL_MAX_LENGTH - queryWithoutWhereClause.Length - (" WHERE " + filtercolumnName + " IN ").Length;
            StringBuilder whereclausechunk = new StringBuilder();
            foreach (string value in values)
            {
                bool addQuerytoList = false;

                //check if adding value in whereclausechunk cross maxSizeofWhereClause, IF NO then add value in whereclausechunk ELSE whereclausechunk is full and its time to add it into querylist
                if ((whereclausechunk + Constants.QUOTE + value + Constants.QUOTE + Constants.COMMA).Length < maxSizeofWhereClause)
                    whereclausechunk.AppendFormat("{0}{1}{2}{3}", Constants.QUOTE, value, Constants.QUOTE, Constants.COMMA);
                else
                    addQuerytoList = true;

                // check if whereclausechunk is full or if its last value of list
                if (addQuerytoList || values.IndexOf(value) == values.Count - 1)
                {
                    //remove last comma appended from whereclausechunk
                    string whereclause = whereclausechunk.ToString().Substring(0, whereclausechunk.Length - 1);

                    //build query
                    string tempquery = string.Format("{0}{1}{2}{3}{4}{5}{6}", queryWithoutWhereClause, " WHERE ", filtercolumnName, " IN ", Constants.OPEN_BRACKET, whereclause, Constants.CLOSE_BRACKET);

                    //add it to list and reset required values
                    chunkedQueries.Add(tempquery);
                    whereclausechunk.Clear();
                    addQuerytoList = false;
                }
            }

            return chunkedQueries;
        }

        public List<string> PrepareChunkedQueries(List<string> fullWhereClauseChunks, string queryWithoutWhereClause, out bool hasChunkingCrossed20K)
        {
            hasChunkingCrossed20K = false;
            List<string> chunkedQueries = new List<string>();
            chunkedQueries.Capacity = fullWhereClauseChunks.Count;

            int minBufferSizeForChunkedQuery = 14000;
            foreach (string whereClause in fullWhereClauseChunks)
            {
                StringBuilder queryBuilder = new System.Text.StringBuilder(minBufferSizeForChunkedQuery);
                queryBuilder.Append(queryWithoutWhereClause).Append(" ").Append(whereClause);

                string query = queryBuilder.ToString();
                query = query.Replace(Constants.ATTACHMENT, Constants.ATTACHMENT_SUBQUERY);

                hasChunkingCrossed20K = queryBuilder.Length > Constants.SOQL_MAX_LENGTH;

                chunkedQueries.Add(query);

                if (hasChunkingCrossed20K)
                {
                    //Memory optimization
                    queryBuilder.Clear();
                    queryBuilder.Capacity = 0;
                    queryBuilder = null;
                    break;
                }

                //Memory optimization
                queryBuilder.Clear();
                queryBuilder.Capacity = 0;
                queryBuilder = null;
            }
            return chunkedQueries;
        }

        public List<string> GetChunkedQueries(string[] InputDataNames, SearchFilter filter)
        {
            List<string> chunkedQueries = new List<string>();
            List<InputData> inputData = ExpressionBuilderHelper.GetInputDataName(InputDataNames);
            string[] split = filter.Value.Split(Constants.DOT.ToCharArray());
            Guid inputObjectGuid = Guid.Parse(split[0]);
            ApttusObject inputObject = appDefManager.GetAppObject(inputObjectGuid);
            string inputField = (filter.ValueType == ExpressionValueTypes.Static ||
                                    filter.ValueType == ExpressionValueTypes.SystemVariables) ? filter.FieldId : split[1];

            ApttusDataSet dataSet = null;
            if (inputData != null)
                dataSet = dataManager.ResolveInput(inputData, inputObject);

            if (dataSet == null || dataSet.DataTable == null)
                return chunkedQueries;

            HashSet<string> uniqueRecordIds = new HashSet<string>();
            foreach (DataRow row in dataSet.DataTable.Rows)
            {
                uniqueRecordIds.Add(Constants.QUOTE + Utils.ConvertTo15CharRecordId(Convert.ToString(row[inputField])) + Constants.QUOTE);

                if (uniqueRecordIds.Count % Constants.MAX_RECORDS_WHILE_CHUNKING == 0)
                {
                    StringBuilder query = new System.Text.StringBuilder(fullQuery).Append(Constants.OPEN_BRACKET).
                        Append(string.Join(Constants.COMMA, uniqueRecordIds)).Append(Constants.CLOSE_BRACKET).Append(Constants.CLOSE_BRACKET);
                    chunkedQueries.Add(query.ToString());
                    query.Clear();
                    query = null;
                    uniqueRecordIds.Clear();
                }
            }

            if (uniqueRecordIds.Count > 0) //there are record-id's left.
            {
                StringBuilder query = new System.Text.StringBuilder(fullQuery).Append(Constants.OPEN_BRACKET).
                        Append(string.Join(Constants.COMMA, uniqueRecordIds)).Append(Constants.CLOSE_BRACKET).Append(Constants.CLOSE_BRACKET);
                chunkedQueries.Add(query.ToString());
                query.Clear();
                query = null;
            }
            uniqueRecordIds.Clear();
            uniqueRecordIds = null;
            return chunkedQueries;
        }        
    }
}
