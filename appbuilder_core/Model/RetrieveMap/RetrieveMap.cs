/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class RetrieveMap
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        public List<RepeatingGroup> RepeatingGroups = new List<RepeatingGroup>();

        public List<RetrieveField> RetrieveFields = new List<RetrieveField>();

        public List<CrossTabRetrieveMap> CrossTabMaps { get; set; }

        public List<string> UniqueTargetNamedRanges { get; set; }

        public void AddField(RetrieveField field)
        {
            RetrieveFields.Add(field);
        }
    }

    [Serializable]
    public class RepeatingGroup
    {
        public RepeatingGroup()
        {
            AppObject = Guid.Empty;
            ObjectName = string.Empty;
            Layout = string.Empty;
            SortByField = string.Empty;
            GroupByField = string.Empty;
            SortByField2 = string.Empty;
            GroupByField2 = string.Empty;
            TotalFields = new List<string>();
        }

        public RepeatingGroup(Guid appObject)
        {
            AppObject = appObject;
            ObjectName = ApplicationDefinitionManager.GetInstance.GetParentAndChildObjects(ApplicationDefinitionManager.GetInstance.GetAppObjects()).Where(s => s.UniqueId.Equals(AppObject)).FirstOrDefault().Name;
            Layout = string.Empty;
            SortByField = string.Empty;
            GroupByField = string.Empty;
            SortByField2 = string.Empty;
            GroupByField2 = string.Empty;
            TotalFields = new List<string>();
            GroupingLayout = Core.GroupingLayout.Top;
        }

        [XmlAttribute("AppObject")]
        public Guid AppObject { get; set; }

        public string TargetNamedRange { get; set; }

        public string ObjectName { get; set; }

        public string GridHeader { get; set; }

        public string Layout { get; set; }

        public string SortByField { get; set; }

        public string SortByField2 { get; set; }

        public string GroupByField { get; set; }

        public string GroupByField2 { get; set; }

        public int GroupSpacing { get; set; }

        public GroupingLayout GroupingLayout { get; set; }

        public List<string> TotalFields { get; private set; }

        public List<RetrieveField> RetrieveFields = new List<RetrieveField>();

        public RepeatingGroupSortDirection SortDirectionField1 { get; set; }

        public RepeatingGroupSortDirection SortDirectionField2 { get; set; }

        public void AddField(RetrieveField field)
        {
            RetrieveFields.Add(field);
        }
        
        //public void RemoveField(RetrieveField field)
        //{
        //    RetrieveFields.Remove(field);
        //}

        //public void AddTotalField(RetrieveField totalField)
        //{
        //    TotalFields.Add(totalField);
        //}

        //public void RemoveTotalField(RetrieveField totalField)
        //{
        //    TotalFields.Remove(totalField);
        //}
    }
}
