using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    public interface IQueryAction
    {
        QueryActionModel Model { get; set; }
        ApttusObject AppObject { get; set; }
        string[] InputDataName { get; set; }
        List<KeyValuePair<string, Guid>> InputDataSetMap { get; set; }
        bool ValidExpression { get; set; }
        DataTable AppFieldsDataTable { get; set; }
        void PrepareWhereClause();
        ActionResult GetResultDataSet(out ApttusDataSet DataSet);
        void SetDisplayableWhereClause(ref ApttusDataSet DataSet);

    }
}
