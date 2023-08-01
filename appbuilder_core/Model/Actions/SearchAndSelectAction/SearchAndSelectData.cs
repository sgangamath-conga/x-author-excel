/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Data;

namespace Apttus.XAuthor.Core
{
    public class SearchAndSelectData
    {
        public DataTable ResultDataTable { get; set; }
        public bool SelectAllInSalesforce { get; set; }
        public string selectedId { get; set; }
        public string whereClause { get; set; }
    }
}

