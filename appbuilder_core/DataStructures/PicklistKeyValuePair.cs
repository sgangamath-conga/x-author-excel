/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class PicklistKeyValuePair
    {
        /// <summary>
        /// Using string for all CRM's.
        /// </summary>
        public string optionKey { get; set; }
        /// <summary>
        /// Picklist Option Display value
        /// </summary>
        public string optionValue { get; set; }
    }
}
