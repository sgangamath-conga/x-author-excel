/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
/* Modified by RM
 * 1/15/2014 : changed LookAheadProp to List, see the comments in the code  for details
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    public interface ICloneable<T>
    {
        T Clone();
    }

    public class Book : ICloneable<Book>
    {
        public Book Clone()
        {
            return new Book { /* set properties */ };
        }
    }
    [Serializable]
    [XmlInclude(typeof(ExcelDataSource)), XmlInclude(typeof(ExcelMultiColumProp)), XmlInclude(typeof(ReturnColumnData)), XmlInclude(typeof(ReturnDataCollection)), XmlInclude(typeof(QueryDataSource)), XmlInclude(typeof(SSActionDataSource))]
    public class RetrieveField : ICloneable<RetrieveField>
    {
        [XmlAttribute("Type")]
        public ObjectType Type { get; set; }
        public Guid AppObject { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string TargetLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public int TargetColumnIndex { get; set; }
        public Datatype DataType { get; set; }
        public string MultiLevelFieldId { get; set; }

        // keeping it for backward compatibility
        public LookAheadProperty LookAheadProp {
            get;
            set;
        }
        // Same field can have LH prop set for offline and online use case
        // for example : offline Excel DS, online - SS action
        // Because of the above reason, changed to a List
        public List<LookAheadProperty> LookAheadProps {
            get;
            set;

        }
        public RetrieveField Clone()
        {
            return new RetrieveField
            {
                Type = this.Type,
                AppObject = this.AppObject,
                FieldId = this.FieldId,
                FieldName = this.FieldName,
                TargetLocation = this.TargetLocation,
                TargetNamedRange = this.TargetNamedRange,
                TargetColumnIndex = this.TargetColumnIndex,
                DataType = this.DataType,
                LookAheadProps = this.LookAheadProps,
                LookAheadProp = this.LookAheadProp,
                MultiLevelFieldId = this.MultiLevelFieldId
            };
        }
    }
}
