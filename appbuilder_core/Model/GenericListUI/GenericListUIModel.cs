/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    public class GenericListUIModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ListScreenType ListType { get; set; }

        public object ListItem { get; set; }

        public bool AutoExecuteRegularMode { get; set; }

        public bool AutoExecuteEditInExcelMode { get; set; }

    }
}
