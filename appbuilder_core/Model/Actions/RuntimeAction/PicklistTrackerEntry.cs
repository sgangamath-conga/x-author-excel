/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class PicklistTrackerEntry
    {
        public string ObjectId { get; set; }

        public string RecordType { get; set; }

        public string FieldId { get; set; }

        public string NamedRange { get; set; }

        public string FormulaNamedRange { get; set; }

        public string ControllingValue { get; set; }

        public int AbsoluteColumnNo { get; set; }

        public PicklistType Type { get; set; }
    }
}
