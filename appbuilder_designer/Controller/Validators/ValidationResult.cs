/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class ValidationResult
    {
        public bool Success { get; set; }

        public EntityType EntityType { get; set; }

        public string EntityName { get; set; }

        public List<string> Messages { get; set; }
    }


}
