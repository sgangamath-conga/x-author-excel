/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Apttus.XAuthor.Core
{
    public class ExternalDataSet
    {
        public List<Dictionary<string, object>> Record { get; set; }
        public string ObjectName { get; set; }
        public string DataSetName { get; set; }

        private bool parsed = false;

        public DataTable ParseRecords(ApttusObject appObject)
        {
            if (!parsed)
            {
                foreach (Dictionary<string, object> recordFields in Record)
                {
                    recordFields.Remove("attributes");

                    List<string> fields = new List<string>(recordFields.Keys);
                    foreach (string field in fields)
                    {
                        //Find Lookup records
                        if (recordFields[field] is IDictionary)
                        {
                            try
                            {
                                //Add name field
                                recordFields.Add(field + Constants.APPENDLOOKUPID, ((IDictionary)recordFields[field])[Constants.NAME_ATTRIBUTE]);
                                recordFields.Remove(field);
                            }
                            catch (Exception e)
                            {
                                //Name field is missing
                            }
                        }
                    }
                }

                parsed = true;
            }

            return JsonHelper.DictToDataTable(Record, appObject);
        }
    }
}
