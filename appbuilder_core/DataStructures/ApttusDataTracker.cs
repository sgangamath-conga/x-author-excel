/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class ApttusDataTracker
    {
        public string Location { get; set; }
        public Guid DataSetId { get; set; }
        public ObjectType Type { get; set; }
    }
}
