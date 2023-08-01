/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    public class ApttusFieldDS : ApttusField
    {
        [XmlIgnore()]
        public bool Included { get; set; }

        public ApttusFieldDS() { base.Visible = true; }

        public ApttusFieldDS(string Id, string Name, Datatype datatype, bool bUpdateable, string FormulaType,string CRMDatatype,bool bCreatable)
        {
            base.Id = Id;
            base.Name = Name;
            base.Datatype = datatype;
            base.Visible = true;
            base.Updateable = bUpdateable;
            base.FormulaType = FormulaType;
            base.CRMDataType = CRMDataType;
            base.Creatable = bCreatable;
        }       
    }
}
