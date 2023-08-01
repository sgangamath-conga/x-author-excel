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
    [XmlInclude(typeof(SaveFieldBound))]
    public class SaveField : ICloneable<SaveField>
    {
        public Guid GroupId { get; set; }
        public ObjectType Type { get; set; }
        public Guid AppObject { get; set; }
        public string FieldId { get; set; }
        public string DesignerLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public int TargetColumnIndex { get; set; }
        public SaveCondition SaveCondition { get; set; }
        public CrossTabRetrieveMap CrossTab { get; set; }
        public Guid MatrixMapId { get; set; }
        public Guid MatrixComponentId { get; set; }
        public SaveType SaveFieldType { get; set; }

        public string SaveFieldName { get; set; }
        public string MultiLevelFieldId { get; set; }

        public LookAheadProperty LookAheadProp
        {
            get;
            set;
        }
        public List<LookAheadProperty> LookAheadProps
        {
            get;
            set;
        }
        public SaveField Clone()
        {
            return new SaveField
            {
                GroupId = this.GroupId,
                Type = this.Type,
                AppObject = this.AppObject,
                FieldId = this.FieldId,
                DesignerLocation = this.DesignerLocation,
                TargetNamedRange = this.TargetNamedRange,
                TargetColumnIndex = this.TargetColumnIndex,
                SaveCondition = this.SaveCondition,
                CrossTab = this.CrossTab,
                SaveFieldType = this.SaveFieldType,
                LookAheadProp = this.LookAheadProp,
                LookAheadProps = this.LookAheadProps,
                SaveFieldName = this.SaveFieldName,
                MultiLevelFieldId = this.MultiLevelFieldId
            };
        }
    }
}


