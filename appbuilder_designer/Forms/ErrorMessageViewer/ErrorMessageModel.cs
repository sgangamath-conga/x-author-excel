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
    class ErrorMessageModel
    {
        public int No { get; set; }
        public EntityType EntityType { get; set; }
        public string EntityName { get; set; }
        public string Description { get; set; }
    }
}
