/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    [XmlInclude(typeof(ApttusFieldDS))]
    [XmlType("ApttusField")]
    public class ApttusField
    {
        public ApttusField()
        {
            Visible = true;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public Datatype Datatype { get; set; }
        public ApttusObject LookupObject { get; set; }
        public List<string> PicklistValues { get; set; }
        public PicklistType PicklistType { get; set; }
        public string ControllingPicklistFieldId { get; set; }
        public List<DependentPicklistItem> DependentPicklistValues { get; set; }
        public int Scale { get; set; }
        public int Precision { get; set; }
        public bool ExternalId { get; set; }
        public bool RecordType { get; set; }
        public bool NameField { get; set; }
        public bool Updateable { get; set; }
        public bool Creatable { get; set; }
        public bool Visible { get; set; }
        public string FormulaType { get; set; }
        public string CRMDataType { get; set; }

        // Dynamics have key value pair as part of Picklist data type, save will always accept key value, so need store this in config
        public List<PicklistKeyValuePair> PicklistKeyValues { get; set; }
        public List<ApttusObject> MultiLookupObjects { get; set; }

        public ApttusField DeepCopy()
        {
            ApttusField rApttusField = (ApttusField)this.MemberwiseClone();
            rApttusField.Visible = true;

            Datatype dt = new Core.Datatype();
            dt = this.Datatype;
            rApttusField.Datatype = dt;
            
            PicklistType pt = new PicklistType();
            pt = this.PicklistType;
            rApttusField.PicklistType = pt;

            if (this.LookupObject != null)
            {
                ApttusObject rLookupObject = this.LookupObject.DeepCopy();
                rApttusField.LookupObject = rLookupObject;
            }

            return rApttusField;
        }

        public bool Required { get; set; }
    }
}
