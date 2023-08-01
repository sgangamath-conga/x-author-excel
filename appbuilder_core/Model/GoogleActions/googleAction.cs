/* Apttus X-Author for Excel
* © 2014 Apttus Inc. All rights reserved.
*/
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace Apttus.XAuthor.Core
{        
    /*
     * class for every google action.
     */
    public class googleAction
    {
        public string actionID;
        public string appObjId;
        public string queryString;
        public string parentSOQLQuery;
        public string parentQueryObjectName;
        public string outputDataName;
        public List<string> inputDataName;
        public RetrieveMap rMap;
        public SaveMap sMap;
        public string actionType;
        public string recordType;
        public string sheetName;
        public string parentLookupId;
        public Dictionary<string, string> ssDisplayFields;
        public List<SearchFilterGroup> WhereFilterGroups;
        //public List<KeyValuePair<string, string>> ssDisplayFields;
        public List<string> ssSortFields;
        public Dictionary<string, string> ssSearchFields;
        public string queryObject;
        public List<string> filters;//first index: fieldId, second index: operator, third index: datatype
        public List<Core.SearchFilter> searchFilters;
        public int rowStart;
        public int colStart;
        public Dictionary<string, string> rFieldIdToColIndex;
        public Dictionary<string, string> rFieldNameToColIndex;
        public Dictionary<string, Datatype> rFieldColIndexToType;
        public Dictionary<string, int> saveFieldToColIndex;
        public Dictionary<string, int> parentSaveFieldColIndex;
        public Dictionary<string, string> saveFieldToType; //type as a number
        public Dictionary<string, string> ssSearchFieldTypes;
        public Dictionary<string, Dictionary<string, string>> lookAheadColMapping;//maps col to look ahead data
        public Dictionary<string, List<Dictionary<string, string>>> googSaveMap; //maps sObjectId to the save fieldId, desLocation, and datatype
        public List<string> headerNames;
        public string[] headers;
        
        
        public Boolean Encrypted = true;

        /* all constructors*/
        public googleAction(string ID)
        {
            this.actionID = ID;
        }
        
        //Query Action
        public googleAction(string ID, string query, string outputDataName, List<SearchFilterGroup> whereFilterGroups)
        {
            actionType = "query";
            this.actionID = ID;
            this.queryString = query;
            if(this.Encrypted)
            {
                this.queryString = EncryptQueryString(query);
            }
            this.WhereFilterGroups = whereFilterGroups;
            this.outputDataName = outputDataName;
        }

        //Retrieve/display action
        public googleAction(string ID, RetrieveMap map, List<string> inputDataName)
        {
            actionType = "retrieve";
            this.actionID = ID;
            this.rMap = map;
            this.inputDataName = inputDataName;
        }

        //improved display actoin
        public googleAction(GoogleAppConfig gConfig, Core.RetrieveActionModel objAction, RetrieveMap rMap, List<string> inputDataName, out List<string> lookAheadSSActions)
        {
            actionType = "retrieve";
            this.actionID = objAction.Id;
            this.inputDataName = inputDataName;
            this.rMap = rMap;


            Dictionary<string, string> rFieldTypes = new Dictionary<string, string>();
            Dictionary<string, Datatype> rFieldTypesInt = new Dictionary<string, Datatype>();
            Dictionary<string, string> rFieldIdToColIndex = new Dictionary<string, string>();
            Dictionary<string, string> rFieldNameToColIndex = new Dictionary<string, string>();
            Dictionary<string, Datatype> rFieldColIndexToType = new Dictionary<string, Datatype>();
            Dictionary<string, string> parentrFieldTypes = new Dictionary<string, string>();

            Dictionary<string, Dictionary<string, string>> lookAheadColMapping = new Dictionary<string, Dictionary<string, string>>();
            lookAheadSSActions = new List<string>();

            List<string> headerNames = new List<string>();
            List<int> colList = new List<int>();
            string targetLocationRowStart = "";
            int firstDel = 0;
            int lastDel = 0;

            //get the data types and rFields. Then check for a look ahead. Create a mapping from rFieldId to the target column, as well as a new search and select action as needed
            foreach (Core.RetrieveField rField in rMap.RepeatingGroups[0].RetrieveFields)
            {
                //string val;
                //if (rFieldTypes.TryGetValue(rField.FieldId, out val))//if the rfield has already been added, skip
                //{
                //    continue;
                //}
                rFieldTypes.Add(rField.FieldId, rField.DataType.ToString());
                rFieldTypesInt.Add(rField.FieldId, rField.DataType);
                rFieldIdToColIndex.Add(rField.FieldId, rField.TargetColumnIndex.ToString());
                //rFieldNameToColIndex.Add(rField.FieldName, rField.TargetColumnIndex.ToString());

                //parse out where the row starts. Gathers a list of all the col index and will take the lwoest number. The result is the coordinate of the upper left corner for x-author data                            
                firstDel = rField.TargetLocation.IndexOf("$");
                lastDel = rField.TargetLocation.LastIndexOf("$");
                targetLocationRowStart = rField.TargetLocation.Substring(lastDel + 1, rField.TargetLocation.Length - (lastDel + 1));
                colList.Add(Convert.ToInt32(ExcelColumnNameToNumber(rField.TargetLocation.Substring(firstDel + 1, (lastDel - firstDel - 1) ))));

                headerNames.Add(rField.FieldId.ToString());

                rFieldColIndexToType.Add(rField.TargetColumnIndex.ToString(), rField.DataType);
                parentrFieldTypes.Add(rField.FieldId.ToString(), rField.DataType.ToString());
                if (rField.LookAheadProp != null) //if there is a look ahead
                {                    
                    //save the ss action
                    Core.SSActionDataSource lookAheadData = (Core.SSActionDataSource)rField.LookAheadProp;
                    lookAheadSSActions.Add(lookAheadData.ActionID);
                    
                    string targetCol = lookAheadData.ReturnColumnData.DataCollection[0].TargetColumn.ToString();
                    string col = rField.TargetColumnIndex.ToString();
                    string fieldId = rField.FieldId;
                    string ssActionId = lookAheadData.ActionID;

                    //map the column to the fieldId for that column, the target col, and the ss action
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("fieldId", fieldId);
                    data.Add("targetCol", targetCol);
                    data.Add("ssActionId", ssActionId);
                    lookAheadColMapping.Add(col, data);
                    
                }
            }
            //assign data to action
            string sheetName = rMap.RepeatingGroups[0].RetrieveFields[0].TargetLocation;
            this.sheetName = sheetName.Substring(0, sheetName.IndexOf("!"));            
            this.lookAheadColMapping = lookAheadColMapping;
            this.rowStart = Convert.ToInt32(targetLocationRowStart);
            this.colStart = colList.Min();
            this.rFieldIdToColIndex = rFieldIdToColIndex;
            this.rFieldNameToColIndex = rFieldNameToColIndex;
            this.rFieldColIndexToType = rFieldColIndexToType;

            //build the headers
            string[] headers;
            headers = new string[colList.Max()];
            foreach (Core.RetrieveField rField in rMap.RepeatingGroups[0].RetrieveFields)
            {
                int lastDot = rField.FieldName.LastIndexOf(".");
                headers[rField.TargetColumnIndex + this.colStart - 2] = rField.FieldName.Substring(lastDot + 1, rField.FieldName.Length - (lastDot + 1));
            }
            this.headers = headers;
            
            //assign information to the config file for easy access in gSheets.
            ApplicationDefinitionManager appDefinitionManager = ApplicationDefinitionManager.GetInstance;
            ApttusObject appObj = appDefinitionManager.GetAppObject(rMap.RepeatingGroups[0].AppObject);
            string tempAppObjId = String.Empty;
            if (!gConfig.namedRangeToObject.TryGetValue(rMap.RepeatingGroups[0].TargetNamedRange, out tempAppObjId))
            {
                gConfig.namedRangeToObject.Add(rMap.RepeatingGroups[0].TargetNamedRange, appObj.Id);//add sObject id
            }
            gConfig.rFieldTypes = rFieldTypes;
            gConfig.rFieldTypesInt = rFieldTypesInt;
            gConfig.childAppObjectId = rMap.RepeatingGroups[0].AppObject.ToString(); //save child object id
            if (rMap.RetrieveFields.Count() > 0) //if there is a parent object
            {
                gConfig.parentAppObjectId = rMap.RetrieveFields[0].AppObject.ToString();
                gConfig.parentrFieldTypes = parentrFieldTypes;
            }

        }

        //Save action
        public googleAction(string ID, SaveMap map)
        {
            actionType = "save";
            this.actionID = ID;
            this.sMap = map;
        }
        //Search and Select action
        public googleAction(Core.Action objAction, string outputDataName, Dictionary<string, object> ssData, List<string> inputDataName)
        {
            actionType = "searchAndSelect";
            this.actionID = objAction.Id;
            this.queryString = (string)ssData["soqlQuery"];
            this.recordType = ((Core.SearchAndSelect)objAction).RecordType;
            this.ssDisplayFields = (Dictionary<string, string>)ssData["displayFields"];
            this.ssSortFields = new List<string>();
            this.ssSearchFields = (Dictionary<string, string>)ssData["searchFields"];            
            this.queryObject = ApplicationDefinitionManager.GetInstance.AppObjects[0].Id.ToString();//get the parent query object                                                            
            this.inputDataName = inputDataName;
            this.parentSOQLQuery = (string)ssData["parentSOQLQuery"];
            this.parentQueryObjectName = (string)ssData["parentObjectName"];
            this.ssSearchFieldTypes = (Dictionary<string, string>)ssData["fieldTypes"];
            this.outputDataName = outputDataName;
            this.recordType = (string)ssData["recordType"];
            this.appObjId = (string)ssData["appObjId"];
            this.parentLookupId = (string)ssData["parentLookupId"];

            if(this.Encrypted)
            {
                this.queryString = EncryptQueryString(this.queryString);
                this.parentSOQLQuery = EncryptQueryString(this.parentSOQLQuery);
            }

        }

        /* getters and setters */
        public void setrMap(RetrieveMap rmap)
        {
            this.rMap = rmap;
        }

        public void setQueryObject(string str)
        {
            this.queryObject = str;
        }


        /* helper methods */

        /// <summary>
        /// Helper method that encrypts query strings. Appends the IV to the encrypted byte array, then encodes to b64 string for sfdc
        /// </summary>
        private string EncryptQueryString(string stringToEncrypt)
        {
            string B64Key = "K4yBEwATyoQ+lm1P+AefJRI58Z030fcboDtgjfiASYg=";
            string B64IV = "QLkvK6F/lu+bLoibri7aBQ==";
            AesManaged aes = new AesManaged();
            byte[] key = Convert.FromBase64String(B64Key);
            byte[] IV = Convert.FromBase64String(B64IV);
            ICryptoTransform encryptor = aes.CreateEncryptor(key, IV);
            byte[] byteEncryptedString;            
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(stringToEncrypt);
                    }
                    byteEncryptedString = msEncrypt.ToArray();                    
                }
            }
            byte[] IVAndData = new byte[IV.Length + byteEncryptedString.Length];
            IV.CopyTo(IVAndData, 0);
            byteEncryptedString.CopyTo(IVAndData, IV.Length);
            return Convert.ToBase64String(IVAndData);
        }

        /// <summary>
        /// takes in a letter and returns the column index
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private string ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum.ToString();
        }


        /// <summary>
        /// returns true if there are user input filters. Out parameter filter logic text
        /// </summary>
        /// <param name="model"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool getUserInputFilters(Core.QueryActionModel model, out string filterLogicText, ApttusObject apttusObject)
        {
            bool userInputFiltersExist = false;
            filterLogicText = "";
            if (model.WhereFilterGroups != null && (model.WhereFilterGroups.Count() > 0))
            {
                //select filters that require userinput
                List<Core.SearchFilter> filterList = (from userFilters in model.WhereFilterGroups[0].Filters
                                                      where userFilters.ValueType.ToString().Equals("UserInput")
                                                      select userFilters).ToList();
                if (filterList.Count() > 0) //there exist filters that require user input
                {
                    this.searchFilters = filterList;
                    filters = new List<string>();
                    //whereFilters = (model).WhereFilterGroups[0].Filters;
                    for (int iter = 0; iter < filterList.Count(); iter++)//go through the filters and add the datatype to a list
                    {
                        //filters.Add(filterList[iter].FieldId);
                        //filters.Add(filterList[iter].Operator);
                        apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(filterList[iter].AppObjectUniqueId);
                        Datatype dataType = apttusObject.Fields.FirstOrDefault(f => f.Id == filterList[iter].FieldId).Datatype;
                        filters.Add(dataType.ToString());
                        userInputFiltersExist = true;
                        filterLogicText = model.WhereFilterGroups[0].FilterLogicText;
                    }
                }

            }
            return userInputFiltersExist;
        }
    }
}
