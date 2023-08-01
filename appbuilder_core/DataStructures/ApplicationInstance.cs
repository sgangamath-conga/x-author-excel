/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;


namespace Apttus.XAuthor.Core
{
    public class ApplicationInstance
    {
        public string UniqueId { get; set; }
        public string AppFileName { get; set; }
        public Application App { get; set; }
        public ApplicationMode AppMode { get; set; }         // Designer / Runtime
        public List<ApttusDataTracker> AppDataTracker { get; set; }
        public List<ApttusDataSet> AppData { get; set; }
        public List<ApttusCrossTabDataSet> CrossTabData { get; set; }
        public List<DataProtectionModel> DataProtection { get; set; }
        public List<PicklistTrackerEntry> PicklistTracker { get; set; }
        public List<Guid> RefreshedObjects { get; set; }
        public List<ApttusMatrixDataSet> ApttusMatrixDataSet { get; set; }
        public RowHighLighting RowHighLightingData { get; set; }
        public bool RowAdded { get; set; }
        public Dictionary<object, List<string>> RepeatingGroupRangesPerWorkSheet { get; set; }
    }
}
