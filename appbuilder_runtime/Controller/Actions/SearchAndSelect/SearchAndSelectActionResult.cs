/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Apttus.WebBrowserControl;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;

namespace Apttus.XAuthor.AppRuntime
{
    class SearchAndSelectActionResult : WebBrowserResult
    {
        private bool parsed = false;

        public int Count;
        public List<Dictionary<string, object>> SObjects;

        public DataTable ParseSObjects()
        {
            if (!parsed)
            {
                foreach (Dictionary<string, object> sobj in SObjects)
                {
                    sobj.Remove("attributes");
                    
                    List<string> fields = new List<string>(sobj.Keys);
                    
                    foreach (string f in fields)
                    {
                        //Find child records
                        if (sobj[f] is IDictionary)
                        {
                            try
                            {
                                //Add name field
                                sobj.Add(f + ".Name", ((IDictionary)sobj[f])["Name"]);
                                sobj.Remove(f);
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

            return JsonHelper.DictToDataTable(SObjects, null);
        }
    }
}
