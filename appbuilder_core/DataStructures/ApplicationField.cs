/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class ApplicationField
    {
        public string FieldId { get; set; }
        public Datatype DataType { get; set; }
        public Guid AppObject { get; set; } 
    }
}
