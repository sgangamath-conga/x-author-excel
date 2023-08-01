/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    public class ApttusSaveRecord
    {
        public Guid AppObject { get; set; }

        public string RecordId { get; set; }

        public string ObjectName { get; set; }

        public List<ApttusSaveField> SaveFields { get; set; }

        public Guid ApttusDataSetId { get; set; }

        public int MatrixRowIndex { get; set; }

        public int MatrixColIndex { get; set; }

        public int RecordRowNo { get; set; }

        public int RecordColumnNo { get; set; }

        public QueryTypes OperationType { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        // This property is added as part of Attachments support to identify number of Attachment records associated with Each Save Record.
        public List<string> AttachmentIds { get; set; }

        public bool HasAttachment { get; set; }

        public List<ApttusSaveAttachment> Attachments { get; set; }

        public bool Ignore { get; set; }

        public string TargetNameRange { get; set; }

        public ObjectType ObjectType { get; set; }

        public ApttusSaveRecord()
        {
            AttachmentIds = new List<string>();
        }
    }
}
