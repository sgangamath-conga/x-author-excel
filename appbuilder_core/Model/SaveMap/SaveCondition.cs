/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    public class SaveCondition
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        // ToDo: Add additional attributes when Save Condition is being developed
    }
}
