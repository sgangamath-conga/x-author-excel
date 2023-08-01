/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Apttus.XAuthor.Core
{
    public class SaveFieldBound : SaveField
    {
        [XmlIgnore()]
        public bool Included { get; set; }

        [XmlIgnore()]
        public string FieldName { get; set; }

        [XmlIgnore()]
        public string SaveConditionText { get; set; }

        [XmlIgnore]
        public string SaveFieldTypeDesc { get { return this.SaveFieldType.GetDescription(); } }
    }    
}


