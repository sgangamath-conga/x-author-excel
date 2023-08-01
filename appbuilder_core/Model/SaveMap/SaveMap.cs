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
    public class SaveMap
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        public List<SaveField> SaveFields = new List<SaveField>();

        public List<SaveGroup> SaveGroups = new List<SaveGroup>();

        public bool IncludeAttachment { get; set; }

        public Guid AttachAppObjectUniqueId { get; set; }

        public string Filename { get; set; }

        public string Extension { get; set; }

        public bool AppendTimestamp { get; set; }
    }
}
