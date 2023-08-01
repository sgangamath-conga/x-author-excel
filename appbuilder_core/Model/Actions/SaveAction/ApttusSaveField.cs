/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ApttusSaveField
    {
        public string FieldId { get; set; }
        public string Value { get; set; }
        public Datatype DataType { get; set; }
        public string TargetNamedRange { get; set; }
        public SaveType SaveFieldType { get; set; }
        public bool ExternalId { get; set; }
        public bool LookupNameField { get; set; }

        public string CRMDataType { get; set; }
        public string LookupObjectId { get; set; }
        public bool Required { get; set; }
    }
}
