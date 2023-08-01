/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SaveGroup
    {
        public Guid GroupId { get; set; }

        public Guid AppObject { get; set; }

        public string TargetNamedRange { get; set; }

        public string Layout { get; set; }

        public int LoadedRows { get; set; }
    }
}


