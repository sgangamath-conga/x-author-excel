/*
 * Apttus X-Author for Excel
 * © 2015 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public enum MatrixEntity
    {
        Row,
        Column,
        Data,
        Individual,
        GroupedColumn
    }
    [Serializable]
    public enum MatrixRenderingType
    {
        Static,
        Dynamic
    }

    [Serializable]
    public enum MatrixValueType
    {
        [Description("Field Label")]
        FieldLabel,
        [Description("Field Value")]
        FieldValue
    }

    /// <summary>
    /// Matrix Object
    /// </summary>
    [Serializable]
    public class MatrixObject
    {
        public MatrixObject()
        {

        }

        public MatrixObject(MatrixEntity type)
        {
            if (type != MatrixEntity.Data)
                MatrixFields = new List<MatrixField>();
        }
        public List<MatrixField> MatrixFields { get; set; }
    }

    /// <summary>
    /// Matrix Data Object
    /// </summary>
    [Serializable]
    public class MatrixDataObject
    {
        public MatrixDataObject()
        {
            MatrixDataFields = new List<MatrixDataField>();
        }
        public List<MatrixDataField> MatrixDataFields { get; set; }
    }

    /// <summary>
    /// Matrix Field
    /// </summary>
    [Serializable]
    public class MatrixField
    {
        [XmlAttribute("ID")]
        public Guid Id { get; set; }

        [XmlAttribute("RenderingType")]
        public MatrixRenderingType RenderingType { get; set; }

        [XmlAttribute("ValueType")]
        public MatrixValueType ValueType { get; set; }

        public Guid AppObjectUniqueID { get; set; }

        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string TargetLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public string SortFieldId { get; set; }

        public Datatype DataType { get; set; }

        public Guid MatrixGroupedParentId { get; set; }

        public List<MatrixField> MatrixGroupedFields { get; set; }

        public MatrixField()
        {
            MatrixGroupedFields = new List<MatrixField>();
        }
    }

    /// <summary>
    /// Matrix Data field
    /// </summary>
    [Serializable]
    public class MatrixDataField : MatrixField
    {
        public MatrixDataField()
        {
            ValueType = MatrixValueType.FieldValue;
            RenderingType = MatrixRenderingType.Dynamic;
            SortFieldId = null; 
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string RowLookupId { get; set; }
        public string ColumnLookupId { get; set; }

        public Guid MatrixRowId { get; set; }
        public Guid MatrixColumnId { get; set; }
        public Guid MatrixComponentId { get; set; }
        //DependentUpdate Property is of no use and have kept it future use, if any.
        [XmlElement(IsNullable = true)]
        public bool? DepedentUpdate { get; set; }
    }

    [Serializable]
    public class MatrixComponent
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Id")]
        public Guid Id { get; set; }
        public Guid AppObjectUniqueID { get; set; }

        public MatrixComponent()
        {
            Id = Guid.NewGuid();
        }

        public List<SearchFilterGroup> WhereFilterGroups { get; set; }
    }

    /// <summary>
    /// Matrix Map
    /// </summary>
    [Serializable]
    public class MatrixMap
    {
        public string Name { get; set; }

        [XmlAttribute("ID")]
        public Guid Id { get; set; }

        public MatrixObject MatrixRow { get; set; }
        public MatrixObject MatrixColumn { get; set; }
        public MatrixDataObject MatrixData { get; set; }
        
        public List<MatrixComponent> MatrixComponents { get; set; }
        public List<MatrixIndividualField> IndependentFields { get; set; }
        public MatrixMap()
        {
            MatrixRow = new MatrixObject(MatrixEntity.Row);
            MatrixColumn = new MatrixObject(MatrixEntity.Column);
            MatrixData = new MatrixDataObject();
            MatrixComponents = new List<MatrixComponent>();
            IndependentFields = new List<MatrixIndividualField>();
        }
    }

    [Serializable]
    public class MatrixIndividualField : MatrixField
    {
        public MatrixIndividualField()
        {
            SortFieldId = string.Empty;
        }
        public bool UsedInSave { get; set; }
    }
}
