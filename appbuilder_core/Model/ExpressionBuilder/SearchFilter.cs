/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;


namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SearchFilter : ICloneable<SearchFilter>
    {
        public int SequenceNo { get; set; }

        [XmlElement("ObjectName")]
        public Guid AppObjectUniqueId { get; set; }

        public List<QueryObject> QueryObjects { get; set; }

        public string FieldId { get; set; }

        [XmlElement("Operator")]
        public string Operator { get; set; }

        [XmlElement("ValueType")]
        public ExpressionValueTypes ValueType { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        public string SearchFilterLabel { get; set; }

        public SearchFilter Clone()
        {
            SearchFilter result = (SearchFilter)MemberwiseClone();
            if (QueryObjects != null)
                result.QueryObjects = QueryObjects.Select(x => x.Clone()).ToList();

            return result;
        }
    }
}
