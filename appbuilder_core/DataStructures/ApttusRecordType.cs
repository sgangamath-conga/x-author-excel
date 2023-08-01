/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class ApttusRecordType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }
        public List<ApttusRecordTypePicklist> RecordTypePicklists { get; set; }
    }

    [Serializable]
    public class ApttusRecordTypePicklist
    {
        public string PicklistFieldId { get; set; }
        public List<string> PicklistValues { get; set; }
    }
}
