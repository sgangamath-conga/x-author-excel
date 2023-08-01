/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Apttus.XAuthor.Core.Helpers
{
    public static class JsonHelper
    {
        public static DataTable DictToDataTable(List<Dictionary<string, object>> list, ApttusObject appObject)
        {
            DataTable result = new DataTable();
            if (list.Count == 0)
                return result;

            //var cols = list.SelectMany(dict => dict.Keys).Distinct();
            //result.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
            List<string> appFields = new List<string>();
            result = ConfigurationManager.GetInstance.GetDataTableFromAllAppFields(appObject, false, true, out appFields);

            foreach (var missingCol in list.SelectMany(dict => dict.Keys).Distinct())
            {
                if (!result.Columns.Contains(missingCol))
                    result.Columns.Add(missingCol.ToString());
            }

            foreach (Dictionary<string, object> item in list)
            {
                var row = result.NewRow();
                foreach (var key in item.Keys)
                {
                    row[key] = item[key];
                }

                result.Rows.Add(row);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objJSON"></param>
        /// <returns></returns>
        public static string JsonObjectToString(object objJSON)
        {
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objJSON);
        }
    }
}
