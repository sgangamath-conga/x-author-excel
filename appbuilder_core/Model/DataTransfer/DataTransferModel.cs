/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class DataTransferRange
    {
        public string SourceSheet { get; set; }
        public string TargetSheet { get; set; }
        public string SourceRange { get; set; }
        public string TargetRange { get; set; }
    }

    [Serializable]
    public class DataTransferMapping
    {
        public string SourceFile { get; set; }
        public List<DataTransferRange> DataTransferRanges { get; set; }

        public DataTransferMapping()
        {
            DataTransferRanges = new List<DataTransferRange>(); 
        }
    }
}
