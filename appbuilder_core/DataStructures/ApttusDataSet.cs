/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// This class holds the runtime data for the App Builder.
    /// </summary>
    [Serializable]
    public class ApttusDataSet
    {
        public ApttusDataSet() { }

        public ApttusDataSet(String name)
        {
            Name = String.Empty;
            if (!String.IsNullOrEmpty(name))
                Name = name;
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public Guid AppObjectUniqueID { get; set; }

        public DataTable DataTable { get; set; }

        public Guid Parent { get; set; }

        public List<string> ColumnNames { get; set; }

        public string SOQL { get; set; }

        public string DisplayableWhereClause { get; set; }
    }

    [Serializable]
    public class ApttusMatrixDataSet
    {
        public ApttusMatrixDataSet(Guid matrixMapId)
        {
            Id = Guid.NewGuid();
            MatrixMapId = matrixMapId;
        }

        public Guid Id { get; private set; }
        public DataTable MatrixDataTable { get; set; }
        public Guid MatrixMapId { get; private set; }
    }
}
