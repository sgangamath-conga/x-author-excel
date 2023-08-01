/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    /*
     <CrossTab>
 <TargetLocation>Sheet3!$D$13</TargetLocation>
 <TargetNamedRange>_f6bb2100_4bad_4c3a_9fd7_a31424d3a9e2</TargetNamedRange>
<Name>Retrieve repeating </Name>
 <Header/>
 <RowLayout>Top</RowLayout>
 <ColLayout>Left</ColLayout>
<RowField>
 <AppObject>6f51ad8d-070e-4a8e-b5a0-b5529c343c6d</AppObject>
 <FieldId>Type</FieldId><FieldName>Account.Opportunity.Opportunity Type</FieldName>
 <DataType>Picklist</DataType>
 </RowField>
<ColField>
 <AppObject>6f51ad8d-070e-4a8e-b5a0-b5529c343c6d</AppObject>
 <FieldId>Type</FieldId><FieldName>Account.Opportunity.Opportunity Type</FieldName>
 <DataType>Picklist</DataType>
 </ColField>
<DataField>
 <AppObject>6f51ad8d-070e-4a8e-b5a0-b5529c343c6d</AppObject>
 <FieldId>Type</FieldId><FieldName>Account.Opportunity.Opportunity Type</FieldName>
 <DataType>Picklist</DataType>
 </DataField>
 </CrossTab>

     * 
     * 
     *  [XmlAttribute("Type")]
        public ObjectType Type { get; set; }
        public Guid AppObject { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string TargetLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public int TargetColumnIndex { get; set; }
        public Datatype DataType { get; set; }
    */

    /*  public class CrossTabRetrieveField
      {

        

       
          public Guid AppObject { get; set; }
          public string FieldId { get; set; }
          public string FieldName { get; set; }
          public string TargetLocation { get; set; }
          public string TagetRangeName { get; set; }
          public Datatype FieldDataType { get; set; }
          public CrossTabRetrieveField( Guid id, string  Fid, string name, string Tloc, string TRangeName,
              Datatype Dtype)
      {
        
          AppObject = id;
          FieldId = Fid;
          FieldName = name;
          TargetLocation = Tloc;
          TagetRangeName = TRangeName;
          FieldDataType = Dtype; 

      }
      }*/

    public enum ColHeaderLayout
    {
        Top = 1,
        Botton = 2,
    }

    public enum RowHeaderLayout
    {
        Left = 1,
        Right = 2
    }

    public class CrossTabRetrieveMap
    {
        public string Name { get; set; }
        public RetrieveField RowField { get; set; }
        public RetrieveField ColField { get; set; }
        public RetrieveField DataField { get; set; }
        public string RowLookupFieldId { get; set; }
        public string ColLookupFieldId { get; set; }
        public string RowSortByFieldId { get; set; }
        public string ColSortByFieldId { get; set; }
        public string TargetLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public RowHeaderLayout RowLayout { get; set; }
        public ColHeaderLayout ColLayout { get; set; }
        public Guid CrossTabAppDefinitionId { get; set; }
        public ObjectType Type { get; set; }
        [XmlAttribute("Id")]
        public Guid Id { get; set; }
    }
}
