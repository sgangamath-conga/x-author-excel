/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class SaveMapMatrixField
    {
        public bool Included { get; set; }
        public ObjectType Type { get; set; }
        public Guid MatrixMapId { get; set; }
        public string MatrixMapName { get; set; }
        public Guid AppObjectUniqueId { get; set; }
        public Guid MatrixComponentId { get; set; }
        public string MatrixComponentName { get; set; }
        public string MatrixObjectName { get; set; }
        public string TargetNamedRange { get; set; }
    }
}
