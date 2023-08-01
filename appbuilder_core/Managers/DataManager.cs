/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Apttus.XAuthor.Core
{
    public sealed class DataManager
    {
        private static DataManager instance;
        private static object syncRoot = new Object();
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ObjectManager objectManager = ObjectManager.GetInstance;

        public List<ApttusDataSet> IdData = new List<ApttusDataSet>();
        public List<ApttusDataSet> AppData = new List<ApttusDataSet>();
        public List<ApttusMatrixDataSet> MatrixDataSets = new List<ApttusMatrixDataSet>();

        public List<ApttusDataTracker> AppDataTracker = new List<ApttusDataTracker>();
        public List<ApttusCrossTabDataSet> CrossTabData = new List<ApttusCrossTabDataSet>();
        public List<DataProtectionModel> DataProtection = new List<DataProtectionModel>();
        public List<PicklistTrackerEntry> PicklistTracker = new List<PicklistTrackerEntry>();
        public StartupParameters StartParameters
        {
            get;
            set;
        }

        private DataManager()
        {
        }

        public Guid GetParentDataSetId(Guid AppObjectID)
        {
            Guid result = Guid.Empty;
            ApttusObject targetObject = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectID);
            if (targetObject != null && targetObject.Parent != null)
            {
                ApttusDataSet parentDataSet = GetDataById(targetObject.Parent.UniqueId);
                result = parentDataSet == null ? Guid.Empty : parentDataSet.Id;
            }
            return result;
        }

        public static DataManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new DataManager();
                    }
                }
                return instance;
            }
        }

        #region "Public Methods"

        #region "Retrieve"

        public List<ApttusDataSet> GetAllData()
        {
            return AppData;
        }

        #region Retrieve DataSet By Object Id.

        public ApttusDataSet GetDataById(Guid ObjectId)
        {
            return AppData.FirstOrDefault(s => s.AppObjectUniqueID == ObjectId);
        }

        public ApttusCrossTabDataSet GetCrossTabDataById(Guid ObjectId)
        {
            return CrossTabData.FirstOrDefault(s => s.CrossTabID == ObjectId);
        }

        #endregion

        #region Retrieve DataSet by DataSet ID.

        public ApttusMatrixDataSet GetMatrixDataSetByMatrixMap(Guid matrixMapId)
        {
            return MatrixDataSets.FirstOrDefault(s => s.MatrixMapId.Equals(matrixMapId));
        }

        public ApttusMatrixDataSet GetMatrixDataSetById(Guid dataSetId)
        {
            return MatrixDataSets.FirstOrDefault(s => s.Id.Equals(dataSetId));
        }

        public ApttusDataSet GetDataByDataSetId(Guid DataSetId)
        {
            return AppData.FirstOrDefault(s => s.Id == DataSetId);
        }

        public ApttusCrossTabDataSet GetCrossTabDataByDataSetId(Guid DataSetId)
        {
            return CrossTabData.FirstOrDefault(s => s.Id == DataSetId);
        }

        #endregion

        #region Retrieve DataSet by Location
        public ApttusDataSet GetDataSetFromLocation(string Location)
        {
            ApttusDataTracker Match = AppDataTracker.FirstOrDefault(s => s.Location == Location);
            if (Match == null)
                return null;
            else
                return GetDataByDataSetId(Match.DataSetId);
        }

        public ApttusCrossTabDataSet GetCrossTabDataSetFromLocation(string Location)
        {
            ApttusDataTracker Match = AppDataTracker.FirstOrDefault(s => s.Location == Location);
            if (Match == null)
                return null;
            else
                return GetCrossTabDataByDataSetId(Match.DataSetId);
        }
        #endregion

        #region Retrieve MultipleDataSets by Name
        public List<ApttusDataSet> GetDatasetsByNames(string[] names)
        {
            List<ApttusDataSet> lstDataSet = new List<ApttusDataSet>();
            foreach (String name in names)
            {
                ApttusDataSet dataSet = AppData.Where(ds => ds.Name == name).FirstOrDefault();
                if (dataSet != null)
                    lstDataSet.Add(dataSet);
            }
            return lstDataSet;
        }

        /// <summary>
        /// Returns ApttusDataSet for a particular ApttusObject from a set of names
        /// </summary>
        /// <param name="inputNames"></param>
        /// <param name="appObject"></param>
        /// <returns></returns>
        public ApttusDataSet ResolveInput(string[] inputDataNames, ApttusObject inputObject)
        {
            List<ApttusDataSet> dataSets = GetDatasetsByNames(inputDataNames);

            ApttusDataSet res;

            res = dataSets.Where(s => s.AppObjectUniqueID == inputObject.UniqueId).FirstOrDefault();

            // If no match found by GUID, try by name
            if (res == null)
            {
                foreach (ApttusDataSet ds in dataSets)
                {
                    ApttusObject dsObject = ApplicationDefinitionManager.GetInstance.GetAppObject(ds.AppObjectUniqueID);

                    if (dsObject.Id == inputObject.Id)
                    {
                        res = ds;
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns ApttusDataSet for a particular ApttusObject from a set of names
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="appObject"></param>
        /// <returns></returns>
        public ApttusDataSet ResolveInput(List<InputData> inputData, ApttusObject inputObject)
        {
            ApttusDataSet result = null;

            List<ApttusDataSet> dataSets = GetDatasetsByNames(inputData.Select(i => i.Name).ToArray());
            var inputObjectDatasets = dataSets.Where(s => s.AppObjectUniqueID == inputObject.UniqueId);

            if (inputObjectDatasets != null)
            {
                InputData nextInputData = null;

                if (inputObjectDatasets.Count() > 1)
                {
                    List<InputData> inputObjectInputData = (from id in inputData
                                                            where inputObjectDatasets.Select(ids => ids.Name).Contains(id.Name)
                                                            select id).ToList();

                    // Find the Next Input Data in round-robin fashion
                    nextInputData = inputObjectInputData.FirstOrDefault(id => id.Accessed == false);

                    // If all are accessed, Update the Accessed to false and restart round-robin
                    if (nextInputData == null)
                    {
                        foreach (var id in inputObjectInputData)
                            id.Accessed = false;

                        // Pick the first Input Data to restart round-robin
                        nextInputData = inputObjectInputData.FirstOrDefault();
                    }

                    result = inputObjectDatasets.FirstOrDefault(ds => ds.Name == nextInputData.Name);
                    nextInputData.Accessed = true;

                }
                else if (inputObjectDatasets.Count() == 1)
                {
                    // Single Match, Update the Accessed to true
                    result = inputObjectDatasets.FirstOrDefault();

                    nextInputData = inputData.FirstOrDefault(id => id.Name == result.Name);
                    nextInputData.Accessed = true;
                }
            }
            else if (inputObjectDatasets == null)
            {
                // If No Match found, Try to find any Input Object Dataset
                foreach (ApttusDataSet ds in dataSets)
                {
                    ApttusObject dsObject = ApplicationDefinitionManager.GetInstance.GetAppObject(ds.AppObjectUniqueID);

                    if (dsObject.Id == inputObject.Id)
                    {
                        result = ds;
                        break;
                    }
                }
            }

            return result;
        }
        public List<ApttusCrossTabDataSet> GetCrossTabDatasetsByNames(string[] names)
        {
            List<ApttusCrossTabDataSet> lstDataSet = new List<ApttusCrossTabDataSet>();
            foreach (String name in names)
            {
                ApttusCrossTabDataSet dataSet = CrossTabData.Where(ds => ds.Name == name).FirstOrDefault();
                if (dataSet != null)
                    lstDataSet.Add(dataSet);
            }
            return lstDataSet;
        }
        #endregion

        public string GetRecordId(ApttusDataSet dataSet, int rowNumber)
        {
            return dataSet.DataTable.Rows[rowNumber][Constants.ID_ATTRIBUTE].ToString();
        }

        public string GetFieldValue(DataTable dataTable, int rowNumber, string FieldId)
        {
            return dataTable.Rows[rowNumber][FieldId].ToString();
        }

        #endregion

        #region Add

        public void AddData(ApttusMatrixDataSet oDataSet)
        {
            if (ContainsDataSet(oDataSet))
            {
                ApttusMatrixDataSet dataSet = MatrixDataSets.FirstOrDefault(s => s.MatrixMapId.Equals(oDataSet.MatrixMapId));
                UpdateDataSet(dataSet, oDataSet.MatrixDataTable);
            }
            else
            {
                MatrixDataSets.Add(oDataSet);
            }
        }        

        public void AddData(ApttusDataSet oDataSet)
        {
            if (oDataSet != null)
            {
                if (ContainsDataSet(oDataSet))
                {
                    ApttusDataSet existingDataSet = AppData.FirstOrDefault(s => s.Name.Equals(oDataSet.Name));
                    UpdateDataSet(existingDataSet, oDataSet);
                }
                else
                {
                    if (oDataSet.Id == Guid.Empty)
                        oDataSet.Id = Guid.NewGuid();

                    AppData.Add(oDataSet);
                }
            }
        }

        /// <summary>
        /// This method adds the Apttus Dataset from a json string which can passed externally to X-Author
        /// </summary>
        /// <param name="json"></param>
        public void AddJsonData(string json)
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;

            WorkflowStructure prestoWorkflow = (from wf in configManager.Workflows.OfType<WorkflowStructure>()
                                                where wf.AutoExecuteRegularMode
                                                select wf).FirstOrDefault();
            if (prestoWorkflow == null)
                return;

            WorkflowAction workflowAction = (from step in prestoWorkflow.Steps
                                             from condition in step.Conditions
                                             from wfAction in condition.WorkflowActions
                                             where configManager.GetActionById(wfAction.ActionId).Type.Equals(Constants.SEARCH_AND_SELECT_ACTION)
                                             select wfAction).FirstOrDefault();

            if (!string.IsNullOrEmpty(json))
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ExternalDataSets extDataSets = ser.Deserialize<ExternalDataSets>(json);
                foreach (var ds in extDataSets.DataSet)
                {
                    if (workflowAction != null && workflowAction.WorkflowActionData.OutputDataName == ds.DataSetName)
                    {
                        ExceptionLogHelper.DebugLog("Search And Select Action Found with dataset Name :"+ds.DataSetName);

                        ApttusObject AppObject = applicationDefinitionManager.GetAppObjectById(ds.ObjectName).FirstOrDefault();
                        ApttusDataSet AppObjectDataSet = new ApttusDataSet
                        {
                            Name = ds.DataSetName,
                            AppObjectUniqueID = AppObject.UniqueId,
                            DataTable = ds.ParseRecords(AppObject)
                        };

                        AddData(AppObjectDataSet);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This method adds the Apttus Dataset from a XDocument which can passed externally to X-Author
        /// </summary>
        /// <param name="InputData"></param>
        public void AddXmlData(XDocument InputData)
        {
            if (InputData != null && InputData.Elements().Count() > 0)
            {
                XElement XAuthorDataElement = InputData.Elements().FirstOrDefault();
                List<string> UniqueAppObjectNames = (from e in XAuthorDataElement.Elements()
                                                     select e.Name.LocalName).Distinct().ToList();

                foreach (string AppObjectName in UniqueAppObjectNames)
                {
                    IEnumerable<XElement> AllAppObjectElements = XAuthorDataElement.Elements(AppObjectName);
                    foreach (XElement AppObjectElement in AllAppObjectElements)
                    {
                        ApttusObject AppObject = applicationDefinitionManager.GetAppObjectById(AppObjectName).FirstOrDefault();
                        ApttusDataSet AppObjectDataSet = new ApttusDataSet
                        {
                            Name = AppObjectElement.Attribute(Constants.XML_DATASETNAME).Value,
                            AppObjectUniqueID = AppObject.UniqueId,
                            DataTable = XElementToDataTable(AppObjectElement)
                        };

                        AddData(AppObjectDataSet);
                    }
                }
            }
        }

        public DataTable XElementToDataTable(XElement x)
        {
            DataTable dt = new DataTable();

            XElement setup = (from p in x.Descendants() select p).First();
            foreach (XElement xe in setup.Descendants()) // build your DataTable
                dt.Columns.Add(new DataColumn(xe.Name.ToString(), typeof(string))); // add columns to your dt

            var all = from p in x.Descendants(setup.Name.ToString()) select p;
            foreach (XElement xe in all)
            {
                DataRow dr = dt.NewRow();
                foreach (XElement xe2 in xe.Descendants())
                    dr[xe2.Name.ToString()] = xe2.Value; //add in the values
                dt.Rows.Add(dr);
            }
            return dt;
        }
        
        public ApttusCrossTabDataSet AddData(ApttusCrossTabDataSet oDataSet)
        {
            if (oDataSet != null)
            {
                if (ContainsDataSet(oDataSet))
                {
                    ApttusCrossTabDataSet existingDataSet = CrossTabData.FirstOrDefault(s => s.Name.Equals(oDataSet.Name));
                    existingDataSet.DataTable = oDataSet.DataTable;
                    existingDataSet.IDTable = oDataSet.IDTable;
                    return existingDataSet;
                }
                else
                {
                    if (oDataSet.Id == Guid.Empty)
                        oDataSet.Id = Guid.NewGuid();

                    CrossTabData.Add(oDataSet);
                    return oDataSet;
                }
            }
            return null;
        }

        public void AddDataTracker(ApttusDataTracker oDataTracker)
        {
            if (oDataTracker != null)
            {
                if (AppDataTracker.Exists(e => e.Location.Equals(oDataTracker.Location)))
                    AppDataTracker.Remove(AppDataTracker.FirstOrDefault(e => e.Location == oDataTracker.Location));

                AppDataTracker.Add(oDataTracker);
            }
        }

        public void AddDataProtection(DataProtectionModel oDataProtectionModel)
        {
            if (oDataProtectionModel != null)
            {
                var ExistingDataProtection = from dp in DataProtection
                                             where dp.SaveMapId == oDataProtectionModel.SaveMapId & dp.SaveGroupAppObject == oDataProtectionModel.SaveGroupAppObject
                                             select dp;

                if (!ExistingDataProtection.Any())
                    DataProtection.Add(oDataProtectionModel);
            }
        }

        public void AddPicklistTracker(PicklistTrackerEntry oPicklistTracker)
        {
            if (oPicklistTracker != null)
            {
                var ExistingPicklistEntry = from pt in PicklistTracker
                                            where pt.ObjectId == oPicklistTracker.ObjectId && pt.FieldId == oPicklistTracker.FieldId && pt.NamedRange == oPicklistTracker.NamedRange
                                            select pt;

                if (!ExistingPicklistEntry.Any())
                    PicklistTracker.Add(oPicklistTracker);
            }
        }
        
#endregion

#region Update

        private void UpdateDataSet(ApttusMatrixDataSet existingDataSet, DataTable newDataTable)
        {
            if (existingDataSet.MatrixDataTable != null)
            {
                existingDataSet.MatrixDataTable.Dispose();
                existingDataSet.MatrixDataTable = null;
            }
            existingDataSet.MatrixDataTable = newDataTable;
        }

        private void UpdateDataSet(ApttusDataSet existingDataSet, ApttusDataSet newDataSet)
        {
            if (existingDataSet.DataTable != null)
            {
                // ABB memory optimization issue
                existingDataSet.DataTable.Clear();
                existingDataSet.DataTable.Dispose();
                existingDataSet.DataTable = null;
            }
            existingDataSet.DataTable = newDataSet.DataTable;
            existingDataSet.DisplayableWhereClause = null;
            existingDataSet.DisplayableWhereClause = newDataSet.DisplayableWhereClause;
            newDataSet.DisplayableWhereClause = null;
        }

        public void AssociateChildDatasets(Guid ParentDataSetId)
        {
            ApttusDataSet parentDataSet = GetDataByDataSetId(ParentDataSetId);

            if (parentDataSet != null)
            {
                Guid ParentAppObjectId = parentDataSet.AppObjectUniqueID;
                foreach (var oChild in applicationDefinitionManager.GetAppObject(ParentAppObjectId).Children)
                {
                    // Find all Orphan Child DataSets
                    var OrphanChildDataSets = (from d in AppData where d.AppObjectUniqueID.Equals(oChild.UniqueId) & d.Parent.Equals(Guid.Empty) select d);
                    foreach (var OrphanChildDataSet in OrphanChildDataSets)
                        OrphanChildDataSet.Parent = ParentDataSetId;
                }
            }
        }

#endregion

#region Remove

        public bool RemoveDataSet(ApttusMatrixDataSet dataSet)
        {
            dataSet.MatrixDataTable.Dispose();
            dataSet.MatrixDataTable = null;

            return MatrixDataSets.Remove(dataSet);
        }

        public bool RemoveDataSet(ApttusDataSet dataSet)
        {
            //Remove the Data Tracker Entry
            int Match = AppDataTracker.RemoveAll(dataTracker => dataTracker.DataSetId.Equals(dataSet.Id));

            //Dispose the Data Table
            dataSet.DataTable.Dispose();

            //Finally remove the DataSet from the DataSet List.
            return AppData.Remove(dataSet);
        }

        /// <summary>
        /// Remove Entry from tracker
        /// </summary>
        /// <param name="location"></param>
        public void RemoveDataTracker(string location, Guid dataSetId)
        {
            if (AppDataTracker.Exists(e => e.Location.Equals(location)))
            {
                if (!Guid.Empty.Equals(dataSetId))
                {
                    ApttusDataTracker tracker = AppDataTracker.FirstOrDefault(e => e.Location == location && e.DataSetId != dataSetId);
                    if (tracker != null)
                    {
                        ApttusDataSet dataSet = GetDataByDataSetId(tracker.DataSetId);
                        if (dataSet != null)
                            RemoveDataSet(dataSet);

                        AppDataTracker.Remove(tracker);
                    }
                    else
                    {
                        tracker = AppDataTracker.FirstOrDefault(t => t.Location.Equals(location));
                        AppDataTracker.Remove(tracker);
                    }
                }
            }
        }
#endregion

        public void Save()
        {
            string xml = ApttusXmlSerializerUtil.Serialize<List<ApttusDataSet>>(AppData);
        }

#region "Lookup"

        public string GetLookupIdFromLookupName(ApttusObject oApttusObject, string LookupNameValue)
        {
            string result = string.Empty;
            foreach (var ds in AppData)
            {
                string objectId = ds.AppObjectUniqueID.Equals(Guid.Empty)
                    ? ds.Name.IndexOf(Constants.LOOKUPDATASETEXTENSION) > -1 ? ds.Name.Substring(0, ds.Name.IndexOf(Constants.LOOKUPDATASETEXTENSION)) : string.Empty
                    : applicationDefinitionManager.GetAppObject(ds.AppObjectUniqueID).Id;
                if (objectId == oApttusObject.Id && ds.DataTable.Columns.Contains(oApttusObject.NameAttribute))
                {
                    var Match = (from dr in ds.DataTable.AsEnumerable()
                                     // Adding ToLower Case to match values without cases, AB-2621 - MSD: While Validating Lookup Name fields it should be case insensitive
                                     // AB-2704 - MSD : ValidateLookup feature breaks on Save Action when Excel contain some of the blank rows
                                 where !string.IsNullOrEmpty(dr.Field<string>(oApttusObject.NameAttribute)) && dr.Field<string>(oApttusObject.NameAttribute).Equals(LookupNameValue, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(dr.Field<string>(oApttusObject.IdAttribute))
                                 select dr.Field<string>(oApttusObject.IdAttribute));
                    if (Match.Any())
                    {
                        result = Match.FirstOrDefault();
                        break;
                    }
                }
            }
            return result;
        }

        public bool FindLookupNames(ApttusObject oApttusObject, List<string> distinctLookupNames, out string ErrorMessage, bool multiLookupObject = false)
        {
            bool result = false;
            ErrorMessage = string.Empty;

            List<string> notFoundLookupNamesInAppData = distinctLookupNames;
            foreach (var ds in AppData)
            {
                if (ds.Name.Equals(Constants.ATTACHMENTDATA_DATASETNAME))
                    continue;

                if (ds.DataTable != null && ds.DataTable.Rows.Count > 0
                    && ds.DataTable.Columns.Contains(oApttusObject.IdAttribute)
                    && ds.DataTable.Columns.Contains(oApttusObject.NameAttribute))
                {
                    string objectId = ds.AppObjectUniqueID.Equals(Guid.Empty)
                        ? ds.Name.Substring(0, ds.Name.IndexOf(Constants.LOOKUPDATASETEXTENSION))
                        : applicationDefinitionManager.GetAppObject(ds.AppObjectUniqueID).Id;
                    if (objectId == oApttusObject.Id)
                    {
                        List<string> toRemove = new List<string>();
                        foreach (string LookupNameValue in notFoundLookupNamesInAppData)
                        {
                            var Match = (from dr in ds.DataTable.AsEnumerable()
                                             // Adding ToLower Case to match values without cases, AB-2621 - MSD: While Validating Lookup Name fields it should be case insensitive
                                             // AB-2704 - MSD : ValidateLookup feature breaks on Save Action when Excel contain some of the blank rows
                                         where !string.IsNullOrEmpty(dr.Field<string>(oApttusObject.NameAttribute)) 
                                               && dr.Field<string>(oApttusObject.NameAttribute).Equals(LookupNameValue, StringComparison.CurrentCultureIgnoreCase) 
                                               && !string.IsNullOrEmpty(dr.Field<string>(oApttusObject.IdAttribute))
                                               select dr.Field<string>(oApttusObject.IdAttribute));
                            if (Match.Any())
                                toRemove.Add(LookupNameValue);
                        }
                        toRemove.ForEach(ln => notFoundLookupNamesInAppData.Remove(ln));
                    }
                }
            }

            // These lookupnames were not found in any dataset
            if (notFoundLookupNamesInAppData.Count > 0)
            {
                // Escape the input names.
                for (int i = 0; i < notFoundLookupNamesInAppData.Count; i++)
                    notFoundLookupNamesInAppData[i] = objectManager.EscapeQueryString(notFoundLookupNamesInAppData[i]);

                //notFoundLookupNamesInAppData.ForEach(s => s = objectManager.EscapeQueryString(s));
                //.ForEach(sr => sr.OperationType = QueryTypes.INSERT)

                //string query = "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + oApttusObject.Id
                //    + " WHERE " + Constants.NAME_ATTRIBUTE + " IN "
                //    + Constants.OPEN_BRACKET + Constants.QUOTE +
                //    String.Join(Constants.QUOTE + Constants.COMMA + Constants.QUOTE, notFoundLookupNamesInAppData.ToArray()) +
                //    Constants.QUOTE + Constants.CLOSE_BRACKET;

                ApttusDataSet App = objectManager.GetLookUpDetails(oApttusObject, notFoundLookupNamesInAppData);

                if (App != null)
                {
                    // Check if all Lookupnames are found
                    List<string> notFoundLookupNamesInMaster = new List<string>();
                    if (App.DataTable.Rows.Count > 0)
                    {
                        foreach (string LookupNameValue in notFoundLookupNamesInAppData)
                        {
                            var Match = (from dr in App.DataTable.AsEnumerable()
                                         // Adding ToLower Case to match values without cases, AB-2621 - MSD: While Validating Lookup Name fields it should be case insensitive
                                         where dr.Field<string>(oApttusObject.NameAttribute).ToLower() == objectManager.UnescapeQueryString(LookupNameValue).ToLower()
                                         select dr);
                            if (!Match.Any())
                                notFoundLookupNamesInMaster.Add(LookupNameValue);
                        }
                        // If all records were found in Salesforce then create a ApttusDataset
                        /* Added additional condition for multiLookupObject, for multiple Lookup object field some of the lookup names may be reffering to another lookup object which will be resolve in another iterations.
                        * Such fields are : customerid, ownerid, regardingobjectid
                        */
                        if (notFoundLookupNamesInMaster.Count == 0 && multiLookupObject == true)
                        {
                            string datasetName = oApttusObject.Id + Constants.LOOKUPDATASETEXTENSION;
                            if (AppData.Exists(d => d.Name == datasetName))
                            {
                                // Append new lookup records to the existing master
                                ApttusDataSet lookupDataset = AppData.FirstOrDefault(d => d.Name == datasetName);
                                foreach (DataRow dr in App.DataTable.Rows)
                                    lookupDataset.DataTable.ImportRow(dr);
                            }
                            else
                                // Create new lookup records master
                                AddData(new ApttusDataSet
                                {
                                    // AppObjectUniqueID is set to blank for Master Datasets
                                    //AppObjectUniqueID = applicationDefinitionManager.GetAppObjectById(oApttusObject.Id).FirstOrDefault().UniqueId,
                                    ColumnNames = new List<string> { oApttusObject.IdAttribute, oApttusObject.NameAttribute},
                                    DataTable = App.DataTable,
                                    Id = Guid.NewGuid(),
                                    Name = datasetName
                                });
                        }
                    }
                    else if (App.DataTable.Rows.Count == 0)
                        notFoundLookupNamesInMaster = notFoundLookupNamesInAppData;

                    if (notFoundLookupNamesInMaster.Count > 0)
                    {
                        // Unescape the input names before adding to the ErrorMessage
                        for (int i = 0; i < notFoundLookupNamesInMaster.Count; i++)
                            notFoundLookupNamesInMaster[i] = objectManager.UnescapeQueryString(notFoundLookupNamesInMaster[i]);

                        ErrorMessage = String.Join(Constants.COMMA, notFoundLookupNamesInMaster);
                    }
                    else
                        result = true;
                }
            }
            else
                result = true;

            return result;
        }

        /// <summary>
        /// GetLookUpDetails Method used to get lookup details 
        /// </summary>
        /// <param name="tableName">TableName is table to make query</param>
        /// <param name="LookupNames">LookupNames is list of values which will be passed as parameter to filter data.</param>
        /// <returns></returns>
        ApttusDataSet GetLookUpDetails(string tableName, List<string> LookupNames)
        {
            //build query
            string query = "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + tableName
                   + " WHERE " + Constants.NAME_ATTRIBUTE + " IN "
                   + Constants.OPEN_BRACKET + Constants.QUOTE +
                   String.Join(Constants.QUOTE + Constants.COMMA + Constants.QUOTE, LookupNames.ToArray()) +
                   Constants.QUOTE + Constants.CLOSE_BRACKET;

            //check if query length crosses 20k then chunk & execute it else simply execute it
            ApttusDataSet resultData = null;
            if (query.Length >= Constants.SOQL_MAX_LENGTH)
            {
                ChunkQueries CH = new ChunkQueries();
                List<string> queries = CH.PrepareChunkedQueriesFromList(LookupNames, "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + tableName, Constants.NAME_ATTRIBUTE);
                return ExecuteQueries(queries);
            }
            else
            {
                resultData = objectManager.QueryDataSet(new SalesforceQuery { SOQL = query });
            }

            return resultData;
        }

        /// <summary>
        /// ExecuteQueries method used to execute bunch of select queries and merge result in one dataset.
        /// </summary>
        /// <param name="queries">list of queries needs to be executed</param>
        /// <returns>returns ApttusDataSet by merging result of all queries passed as parameter.</returns>
        ApttusDataSet ExecuteQueries(List<string> queries)
        {
            ApttusDataSet ds = new ApttusDataSet();
            ds.DataTable = new DataTable();
            foreach (string query in queries)
            {
                ds.DataTable.Merge(objectManager.QueryDataSet(new SalesforceQuery { SOQL = query }).DataTable);
            }
            return ds;
        }

        /// <summary>
        /// This method use for Matrix LookupName for Row and Column case of Add Row, Add Column
        /// </summary>
        /// <param name="oApttusObject"></param>
        /// <param name="distinctLookupNames"></param>
        /// <returns></returns>
        public string MatrixFindLookupNames(ApttusObject oApttusObject, List<string> distinctLookupNames)
        {
            string Id = string.Empty;
            List<string> notFoundLookupNamesInAppData = distinctLookupNames;
            foreach (var ds in AppData)
            {
                if (ds.Name.Equals(Constants.ATTACHMENTDATA_DATASETNAME))
                    continue;

                if (ds.DataTable != null && ds.DataTable.Rows.Count > 0
                    && ds.DataTable.Columns.Contains(oApttusObject.IdAttribute)
                    && ds.DataTable.Columns.Contains(oApttusObject.NameAttribute))
                {
                    string objectId = ds.AppObjectUniqueID.Equals(Guid.Empty)
                        ? ds.Name.Substring(0, ds.Name.IndexOf(Constants.LOOKUPDATASETEXTENSION))
                        : applicationDefinitionManager.GetAppObject(ds.AppObjectUniqueID).Id;
                    if (objectId == oApttusObject.Id)
                    {
                        List<string> toRemove = new List<string>();
                        foreach (string LookupNameValue in notFoundLookupNamesInAppData)
                        {
                            List<string> Match = (from dr in ds.DataTable.AsEnumerable()
                                         where dr.Field<string>(oApttusObject.NameAttribute) == LookupNameValue & !string.IsNullOrEmpty(dr.Field<string>(oApttusObject.IdAttribute))
                                         select dr.Field<string>(oApttusObject.IdAttribute)).ToList();
                            if (Match.Any())
                            {
                                Id = Match[0];
                                toRemove.Add(LookupNameValue);
                            }
                        }
                        toRemove.ForEach(ln => notFoundLookupNamesInAppData.Remove(ln));
                    }
                }
            }

            // These lookupnames were not found in any dataset
            if (notFoundLookupNamesInAppData.Count > 0)
            {
                // Escape the input names.
                for (int i = 0; i < notFoundLookupNamesInAppData.Count; i++)
                    notFoundLookupNamesInAppData[i] = objectManager.EscapeQueryString(notFoundLookupNamesInAppData[i]);

                //notFoundLookupNamesInAppData.ForEach(s => s = objectManager.EscapeQueryString(s));
                //.ForEach(sr => sr.OperationType = QueryTypes.INSERT)

                //string query = "SELECT " + Constants.ID_ATTRIBUTE + Constants.COMMA + Constants.NAME_ATTRIBUTE + " FROM " + oApttusObject.Id
                //    + " WHERE " + Constants.NAME_ATTRIBUTE + " IN "
                //    + Constants.OPEN_BRACKET + Constants.QUOTE +
                //    String.Join(Constants.QUOTE + Constants.COMMA + Constants.QUOTE, notFoundLookupNamesInAppData.ToArray()) +
                //    Constants.QUOTE + Constants.CLOSE_BRACKET;

                var App = objectManager.GetLookUpDetails(oApttusObject,notFoundLookupNamesInAppData);
                if (App != null)
                {
                    // Check if all Lookupnames are found
                    List<string> notFoundLookupNamesInMaster = new List<string>();
                    if (App.DataTable.Rows.Count > 0)
                    {
                        foreach (string LookupNameValue in notFoundLookupNamesInAppData)
                        {
                            var Match = (from dr in App.DataTable.AsEnumerable()
                                         where dr.Field<string>(oApttusObject.NameAttribute) == objectManager.UnescapeQueryString(LookupNameValue)
                                         select dr);
                            if (!Match.Any())
                                notFoundLookupNamesInMaster.Add(LookupNameValue);
                        }
                        // If all records were found in Salesforce then create a ApttusDataset
                        if (notFoundLookupNamesInMaster.Count == 0)
                        {
                            string datasetName = oApttusObject.Id + Constants.LOOKUPDATASETEXTENSION;
                            if (AppData.Exists(d => d.Name == datasetName))
                            {
                                // Append new lookup records to the existing master
                                ApttusDataSet lookupDataset = AppData.FirstOrDefault(d => d.Name == datasetName);
                                foreach (DataRow dr in App.DataTable.Rows)
                                    lookupDataset.DataTable.ImportRow(dr);
                            }
                            else
                                // Create new lookup records master
                                AddData(new ApttusDataSet
                                {
                                    // AppObjectUniqueID is set to blank for Master Datasets
                                    //AppObjectUniqueID = applicationDefinitionManager.GetAppObjectById(oApttusObject.Id).FirstOrDefault().UniqueId,
                                    ColumnNames = new List<string> { oApttusObject.IdAttribute, oApttusObject.NameAttribute },
                                    DataTable = App.DataTable,
                                    Id = Guid.NewGuid(),
                                    Name = datasetName
                                });
                        }

                        Id = App.DataTable.Rows[0][oApttusObject.IdAttribute].ToString();
                    }
                    else if (App.DataTable.Rows.Count == 0)
                        notFoundLookupNamesInMaster = notFoundLookupNamesInAppData;
                }
            }

            return Id;            
        }

#endregion

#endregion

#region "Private Methods"

        private bool ContainsDataSet(ApttusDataSet dataSet)
        {
            String name = dataSet.Name;
            if (String.IsNullOrEmpty(name))
                return false;
            return AppData.Where(s => s.Name.Equals(name)).FirstOrDefault() != null;
        }

        private bool ContainsDataSet(ApttusCrossTabDataSet dataSet)
        {
            String name = dataSet.Name;
            if (String.IsNullOrEmpty(name))
                return false;
            return CrossTabData.Where(s => s.Name.Equals(name)).FirstOrDefault() != null;
        }

        private bool ContainsDataSet(ApttusMatrixDataSet dataSet)
        {
            return MatrixDataSets.Where(s => s.MatrixMapId.Equals(dataSet.MatrixMapId)).FirstOrDefault() != null;
        }
#endregion

    }
}
