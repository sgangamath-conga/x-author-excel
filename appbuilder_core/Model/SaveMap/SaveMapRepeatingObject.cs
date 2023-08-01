/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class SaveMapRepeatingObject
    {
        public Guid SaveGroupId { get; set; }

        public string Name { get; set; }

        public int LoadedRows { get; set; }

        public string NamePlusLoadedRow
        {
            get
            {
                return LoadedRows > 0 ? Name + Constants.SPACE + Constants.OPEN_BRACKET + LoadedRows.ToString() + Constants.CLOSE_BRACKET : Name;
            }
        }

    }
}
