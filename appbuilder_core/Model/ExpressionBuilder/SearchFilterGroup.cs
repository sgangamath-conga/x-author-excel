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
    public class SearchFilterGroup : ICloneable<SearchFilterGroup>
    {
        public List<SearchFilterGroup> Groups { get; set; }

        public List<SearchFilter> Filters { get; set; }

        [XmlAttribute("LogicalOperator")]
        public LogicalOperator LogicalOperator { get; set; }

        public string FilterLogicText { get; set; }

        public Boolean IsAddFilter { get; set; }

        public SearchFilterGroup Clone()
        {
            SearchFilterGroup result = (SearchFilterGroup)MemberwiseClone();
            if (Groups != null)
                result.Groups = Groups.Select(x => x.Clone()).ToList();
            if (Filters != null)
                result.Filters = Filters.Select(x => x.Clone()).ToList();

            return result;
        }
    }
}
