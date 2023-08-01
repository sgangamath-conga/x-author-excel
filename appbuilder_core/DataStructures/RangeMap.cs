/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class RangeMap
    {
        public string RangeName { get; set; }
        public ObjectType Type { get; set; }
        public Guid RetrieveMapId { get; set; }
        public Guid MatrixMapId { get; set; }
        public Guid SaveMapId { get; set; }
    }
}
