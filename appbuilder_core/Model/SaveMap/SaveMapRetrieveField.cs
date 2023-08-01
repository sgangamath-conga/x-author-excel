/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class SaveMapRetrieveField
    {
        public bool Included { get; set; }

        public ObjectType Type { get; set; }

        public Guid RetrieveMapId { get; set; }
        public string RetrieveMapName { get; set; }

        public Guid AppObjectUniqueId { get; set; }

        public string RetrieveFieldId { get; set; }
        public string RetrieveFieldName { get; set; }

        public string DesignerLocation { get; set; }
        public string TargetNamedRange { get; set; }
        public int TargetColumnIndex { get; set; }
        public string Layout { get; set; }
        public CrossTabRetrieveMap CrossTab { get; set; }
        public string MultiLevelFieldId { get; set; }

    }
}
