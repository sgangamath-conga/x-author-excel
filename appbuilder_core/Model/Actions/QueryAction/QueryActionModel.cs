/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class QueryActionModel : Action
    {
        public const int MAX_RECORDS_NOLIMIT = -1;
        public QueryTypes QueryType;
        public Guid TargetObject;
        public List<SearchFilterGroup> WhereFilterGroups { get; set; }
        public int MaxRecords;
        public bool AllowSavingFilters { get; set; }

        public List<string> GetInputObjects()
        {
            List<string> objectSet = new List<string>();
            if (WhereFilterGroups != null)
            {
                foreach (SearchFilterGroup filterGroup in WhereFilterGroups)
                    ExpressionBuilderHelper.GetFilterGroupObjects(filterGroup, objectSet);
            }
            return objectSet.ToList();
        }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }
    }
}
