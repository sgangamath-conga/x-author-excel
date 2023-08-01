using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppRuntime
{
    public interface ISearchAndSelectAction
    {
        SearchAndSelectData GetResultData(SearchAndSelect model, ApttusObject appObject, List<string> selectFields, List<ResultField> resultFields,
            string globalSearchString, string advancedFilterWhereClause, bool bClear, string[] InputDataName);
        ApttusDataSet GetData(SearchAndSelectActionView view, ActionResult Result);
        bool GetValidExpression(string Id, List<SearchFilterGroup> SearchFilterGroups, ApttusObject appObject, string[] InputDataName, out List<KeyValuePair<string, Guid>> UsedDataSets);
    }
}
