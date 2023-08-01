/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    public class AppChange
    {
        public ChangeItemType ChangeItemType { get; set; }

        public QueryTypes ChangeType { get; set; }

        public string ObjectId { get; set; }

        public string PicklistFieldId { get; set; }

        public string ControllingValue { get; set; }

        public string RecordType { get; set; }

        public List<string> RecordTypePicklistFieldIds { get; set; }

    }
}
