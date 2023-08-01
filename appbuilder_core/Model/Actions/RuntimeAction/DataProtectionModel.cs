/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class DataProtectionModel
    {
        public string WorksheetName { get; set; }

        public Guid SaveMapId { get; set; }

        public Guid SaveGroupAppObject { get; set; }

        public int IdColumnIndex { get; set; }

        public int LockCount { get; set; }

    }
}
