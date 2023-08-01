/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Data;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// This class holds the runtime data for the App Builder.
    /// </summary>
    [Serializable]
    public class ApttusCrossTabDataSet
    {
        public ApttusCrossTabDataSet() { }

        public ApttusCrossTabDataSet(String name)
        {
            Name = String.Empty;
            if (!String.IsNullOrEmpty(name))
                Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CrossTabID { get; set; }

        public DataTable DataTable { get; set; }

        public DataTable IDTable { get; set; }

    }
}
