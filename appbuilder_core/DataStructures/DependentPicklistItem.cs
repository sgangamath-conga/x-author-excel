/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class DependentPicklistItem
    {
        public string ControllingValue { get; set; }
        public List<string> PicklistValues { get; set; }

    }
}
