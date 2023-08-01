/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;


namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class QueryObject : ICloneable<QueryObject>
    {
        public int SequenceNo { get; set; }

        public Guid AppObjectUniqueId { get; set; }

        public string LookupFieldId { get; set; }

        public QueryRelationshipType RelationshipType { get; set; }

        public string BreadCrumbLabel { get; set; }

        public string BreadCrumbTooltip { get; set; }

        public int DistanceFromChild { get; set; }

        public Guid LeafAppObjectUniqueId { get; set; }

        public QueryObject Clone()
        {
            return new QueryObject
            {
                SequenceNo = SequenceNo,
                AppObjectUniqueId = AppObjectUniqueId,
                LookupFieldId = LookupFieldId,
                RelationshipType = RelationshipType,
                BreadCrumbLabel = BreadCrumbLabel,
                BreadCrumbTooltip = BreadCrumbTooltip,
                DistanceFromChild = DistanceFromChild,
                LeafAppObjectUniqueId = LeafAppObjectUniqueId
            };
        }
    }
}
