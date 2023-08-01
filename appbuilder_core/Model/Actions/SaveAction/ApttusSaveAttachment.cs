/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ApttusSaveAttachment
    {
        public string RecordId { get; set; }

        public string Base64EncodedString { get; set; }

        public string ParentId { get; set; }

        public string AttachmentName { get; set; }

        public string ObjectId { get; set; }
    }
}
