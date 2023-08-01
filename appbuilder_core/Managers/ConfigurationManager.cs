/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Apttus.XAuthor.Core.CPQ;
using Apttus.XAuthor.Core.Model.Actions.CPQAction;
using System.Data;

namespace Apttus.XAuthor.Core
{
    public sealed class ConfigurationManager
    {
        private static ConfigurationManager instance;
        private static object syncRoot = new Object();
        private Application application;
        private List<RangeMap> RangeMaps;

        private ConfigurationManager()
        {
        }

        //public void AssignSchema(byte[] schema)
        //{
        //    application.Schema = schema;
        //}

        public static ConfigurationManager GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ConfigurationManager();
                    }
                }

                return instance;
            }
        }

        #region "Application Methods: List, Create, Save"

        /// <summary>
        /// Creats new configuration file for an App.
        /// </summary>
        /// <param name="appName">Application Name</param>
        /// <returns>bool</returns>
        public bool SetApplication(Application NewApp)
        {
            try
            {
                application = NewApp;
# if DEBUG
                SaveLocal(string.Empty, false);
#endif
                return true;
            }
            catch
            {
                return false; //Config Creation Failed.
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public ApplicationObject LoadApp(string appId, string appUniqueId)
        //{
        //    ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();
        //    ApplicationObject app = salesForceAdapter.LoadApplication(appId);

        //    if (app != null)
        //    {
        //        UTF8Encoding encoding = new UTF8Encoding();
        //        string configXml = encoding.GetString(app.Config);

        //        application = ApttusXmlSerializerUtil.Deserialize<Application>(configXml);
        //        // Assign appl def objects to appdef manager, so that actions, maps and others can use the same.
        //        ApplicationDefinitionManager.GetInstance.AppObjects = application.Definition.AppObjects;
        //        ApplicationDefinitionManager.GetInstance.CrossTabDefinitions = application.Definition.CrossTabs;
        //        /*
        //        if (application.Definition.CrossTabs.Count > 0)
        //        {
        //            foreach(CrossTabDef xDef in application.Definition.CrossTabs)
        //            {
        //                ApplicationDefinitionManager.GetInstance.AppObjects.Add(xDef.RowHeaderObject);
        //                ApplicationDefinitionManager.GetInstance.AppObjects.Add(xDef.ColHeaderObject);
        //                ApplicationDefinitionManager.GetInstance.AppObjects.Add(xDef.DataObject);



        //            }
        //        }*/
        //    }
        //    return app;
        //}

        /// <summary>
        /// This method saves the App Builder configuration to a local folder
        /// </summary>
        public void SaveLocal(string SavePath, bool Encrypted)
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                SavePath = Utils.GetApplicationPath(application.Definition.UniqueId);
                SavePath = Utils.GetTempFileName(SavePath, application.Definition.Name + Constants.XML);
            }

            if (Encrypted)
            {
                string EncryptedXml = EncryptionHelper.Encrypt(ApttusXmlSerializerUtil.Serialize<Application>(application));
                File.WriteAllText(SavePath, EncryptedXml);
            }
            else
                ApttusXmlSerializerUtil.SerializeToFile<Application>(application, SavePath);
        }

        /// <summary>
        /// TODO:: this function can be used to load existing App, do changes if any required 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        public void LoadAppFromLocal(string appId, string appName)
        {
            string appTempFile = Utils.GetApplicationPath(appId) + "\\" + appName + ".xml";
            string configXml = System.IO.File.ReadAllText(appTempFile);

            application = ApttusXmlSerializerUtil.Deserialize<Application>(EncryptionHelper.Decrypt(configXml));

            // Assign appl def objects to appdef manager, so that actions, maps and others can use the same.
            ApplicationDefinitionManager.GetInstance.AppObjects = application.Definition.AppObjects;
            ApplicationDefinitionManager.GetInstance.CrossTabDefinitions = application.Definition.CrossTabs;
        }

        internal void SetIsAppActive(bool isAppActive)
        {
            Definition.IsActive = isAppActive;
        }

        /// <summary>
        /// TODO:: this function can be used to load existing App, do changes if any required 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        public void LoadAppOffline(string configXml)
        {
            application = ApttusXmlSerializerUtil.Deserialize<Application>(EncryptionHelper.Decrypt(configXml));

            // Assign appl def objects to appdef manager, so that actions, maps and others can use the same.
            ApplicationDefinitionManager.GetInstance.AppObjects = application.Definition.AppObjects;
            ApplicationDefinitionManager.GetInstance.CrossTabDefinitions = application.Definition.CrossTabs;
        }

        #endregion

        #region "Add"

        public void UpdateAppDefinition(List<ApttusObject> objects)
        {
            application.Definition.AppObjects = objects;
        }

        public void UpdateCrossTabDefinition(List<CrossTabDef> objects)
        {
            application.Definition.CrossTabs = objects;
        }

        // ToDo: Refeekh to replace this Menu model once its moved to Core
        public string AddMenus(Menus mnuObject)
        {
            //retireveMap.Id = Guid.NewGuid().ToString();
            application.AppMenus = mnuObject;
            //return retireveMap.Id;
            return null;
        }

        public string AddAction(Action action)
        {
            action.Id = Guid.NewGuid().ToString();
            application.Actions.Add(action);
            return action.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public Guid AddWorkflow(Workflow workflow)
        {
            if (workflow.Id == Guid.Empty)
                workflow.Id = Guid.NewGuid();
            application.Workflows.Add(workflow);
            return workflow.Id;
        }

        /// <summary>
        /// Add Retrieve Map
        /// </summary>
        /// <param name="retrieveMap"></param>
        /// <returns></returns>
        public Guid AddRetrieveMap(RetrieveMap retrieveMap)
        {
            if (retrieveMap.Id == Guid.Empty)
                retrieveMap.Id = Guid.NewGuid();
            application.RetrieveMaps.Add(retrieveMap);
            return retrieveMap.Id;
        }

        /// <summary>
        /// Add Matrix Map
        /// </summary>
        /// <param name="matrixMap"></param>
        /// <returns></returns>
        public Guid AddMatrixMap(MatrixMap matrixMap)
        {
            if (matrixMap.Id == Guid.Empty)
            {
                matrixMap.Id = Guid.NewGuid();
                application.MatrixMaps.Add(matrixMap);
            }
            return matrixMap.Id;
        }

        public Guid AddCrossTabRetrieveMap(RetrieveMap retrieveMap, CrossTabRetrieveMap crossTabRetrieveMap)
        {
            if (crossTabRetrieveMap.Id == Guid.Empty)
                crossTabRetrieveMap.Id = Guid.NewGuid();
            if (retrieveMap.CrossTabMaps == null)
            {
                retrieveMap.CrossTabMaps = new List<CrossTabRetrieveMap>();
            }
            else
            {
                CrossTabRetrieveMap ctMap = retrieveMap.CrossTabMaps.Find(item => item.Id.Equals(crossTabRetrieveMap.Id));
                if (ctMap != null)
                    retrieveMap.CrossTabMaps.Remove(ctMap);
            }
            retrieveMap.CrossTabMaps.Add(crossTabRetrieveMap);
            return crossTabRetrieveMap.Id;
        }

        public void RemoveCrossTabRetrieveMap(CrossTabRetrieveMap retrieveMap)
        {
            if (retrieveMap.Id == Guid.Empty)
                retrieveMap.Id = Guid.NewGuid();

            RetrieveMap map = application.RetrieveMaps.Find(item => item.Id.Equals(retrieveMap.Id));
            if (map != null)
                application.RetrieveMaps.Remove(map);

        }

        // ToDo: Replace with SaveMap class when its developed
        public Guid AddSaveMap(SaveMap saveMap)
        {
            saveMap.Id = Guid.NewGuid();
            application.SaveMaps.Add(saveMap);
            return saveMap.Id;
        }

        public void SaveMapping(DataTransferMapping mapping)
        {
            if (application.Definition.Mapping == null)
                application.Definition.Mapping = new DataTransferMapping();

            application.Definition.Mapping.SourceFile = mapping.SourceFile;
            application.Definition.Mapping.DataTransferRanges.Clear();
            application.Definition.Mapping.DataTransferRanges.AddRange(mapping.DataTransferRanges);
        }

        public void AssignConfig(byte[] config)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string configXml = encoding.GetString(config);

            // Assign Config Manager and Application Definition objects.
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            Application = ApttusXmlSerializerUtil.Deserialize<Application>(EncryptionHelper.Decrypt(configXml));
            applicationDefinitionManager.AppObjects = Application.Definition.AppObjects;
            applicationDefinitionManager.CrossTabDefinitions = Application.Definition.CrossTabs;
            string EditionType = null;

            if (!string.IsNullOrEmpty(Application.Definition.EditionType))
            {
                EditionType = Application.Definition.EditionType;
            }
            else // should never get here. default to basic and may wanto to log an exception. 
            {

                EditionType = Constants.ENTERPRISE_EDITION;
            }



        }

        /// <summary>
        /// Add App Settings
        /// </summary>
        /// <param name="retrieveMap"></param>
        /// <returns></returns>
        public void SaveAppSettings(AppSettings appSettings)
        {

            List<SheetProtect> lst = new List<SheetProtect>();
            lst.AddRange(appSettings.SheetProtectSettings);

            if (application.Definition.AppSettings == null)
            {
                application.Definition.AppSettings = new AppSettings();
            }
            foreach (SheetProtect item in lst)
            {
                SheetProtect sheetProtect = application.Definition.AppSettings.SheetProtectSettings.Find(s => s.SheetName.Equals(item.SheetName));
                if (sheetProtect != null)
                    application.Definition.AppSettings.SheetProtectSettings.Remove(sheetProtect);

                application.Definition.AppSettings.SheetProtectSettings.Add(item);
            }
            application.Definition.AppSettings.DisableRichTextEditing = appSettings.DisableRichTextEditing;
            application.Definition.AppSettings.DisablePrint = appSettings.DisablePrint;
            application.Definition.AppSettings.DisableLocalSaveFile = appSettings.DisableLocalSaveFile;
            application.Definition.AppSettings.IgnorePicklistValidation = appSettings.IgnorePicklistValidation;

            // Suppress Message for data migration app
            application.Definition.AppSettings.SuppressNoOfRecords = appSettings.SuppressNoOfRecords;
            application.Definition.AppSettings.SuppressDependent = appSettings.SuppressDependent;
            application.Definition.AppSettings.SuppressSave = appSettings.SuppressSave;

            //set row highlighting options
            application.Definition.AppSettings.EnableRowHighlight = appSettings.EnableRowHighlight;
            application.Definition.AppSettings.RowErrorColor = appSettings.RowErrorColor;
        }

        #endregion

        #region "Get"

        public Definition Definition {
            get { return application.Definition; }
        }

        public Menus Menus {
            get { return application.AppMenus; }
        }

        public List<Action> Actions {
            get { return application.Actions; }
        }

        public Application Application {
            get { return application; }
            set { this.application = value; }
        }

        public List<Workflow> Workflows {
            get { return application.Workflows; }
        }

        public List<RetrieveMap> RetrieveMaps {
            get { return application.RetrieveMaps; }
        }

        public List<MatrixMap> MatrixMaps {
            get { return application.MatrixMaps; }
        }

        public List<SaveMap> SaveMaps {
            get { return application.SaveMaps; }
        }

        public DataTable GetDataTableFromAppFields(ApttusObject appObject, List<string> appFields)
        {
            DataTable dataTable = new DataTable();

            foreach (var appField in appFields)
            {
                ApttusField apttusField = appObject.Fields.FirstOrDefault(f => f.Id == appField);
                bool bIsVisible = FieldLevelSecurityManager.Instance.IsFieldVisible(appObject.UniqueId, apttusField.Id);

                if (apttusField != null && bIsVisible)
                {
                    // Append DataColumn to the datatable
                    DataColumn dc = new DataColumn(apttusField.Id);
                    dc.DataType = apttusField.Datatype == Datatype.Decimal || apttusField.Datatype == Datatype.Double ? typeof(double) : typeof(string);
                    dataTable.Columns.Add(dc);
                }
            }
            return dataTable;
        }

        public DataTable GetDataTableFromAllAppFields(ApttusObject apttusObject, bool includeSaveOtherFields, bool includeRelationalFields, out List<string> fields)
        {
            DataTable dataTable = new DataTable();
            fields = new List<string>();

            List<ApplicationField> appFields = GetAllAppFields(apttusObject, includeSaveOtherFields, includeRelationalFields);
            foreach (ApplicationField appField in appFields)
            {
                // Add Data Column
                DataColumn dc = new DataColumn(appField.FieldId);
                switch (appField.DataType)
                {
                    case Datatype.Decimal:
                    case Datatype.Double:
                        dc.DataType = typeof(double);
                        break;
                    //case Datatype.Date: Date type only is not available in .NET and it fa
                    case Datatype.DateTime:
                        dc.DataType = typeof(DateTime);
                        break;
                    default:
                        dc.DataType = typeof(string);
                        break;
                }
                dataTable.Columns.Add(dc);

                // Add Field to collection
                fields.Add(appField.FieldId);
            }
            // This block is INTENTIONALLY added
            // We are adding Id Coulmn specifically to stop Id related desctipancies in Retrive and Save Action in case of dynamics
            if (!dataTable.Columns.Contains(Constants.ID_ATTRIBUTE))
            {
                DataColumn dc = new DataColumn(Constants.ID_ATTRIBUTE)
                {
                    DataType = typeof(string)
                };
                dataTable.Columns.Add(dc);
            }
            return dataTable;
        }

        /// <summary>
        /// Get all unique fields in a comma separated format to get data during runtime
        /// </summary>
        /// <param name="apttusObject">application object</param>
        /// <returns></returns>
        public List<ApplicationField> GetAllAppFields(ApttusObject apttusObject, bool includeSaveOtherFields, bool includeRelationalFields = false)
        {
            Guid targetObjectUniqueId = apttusObject.UniqueId;
            List<ApplicationField> Fields = new List<ApplicationField>();
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            FieldLevelSecurityManager fieldAccess = FieldLevelSecurityManager.Instance;
            // ToDo: Fetch the union of the following set of fields of the TargetObject

            // 1. Add Id as the 1st column
            //Fields.Add(new ApplicationField { FieldId = Constants.ID_ATTRIBUTE, DataType = Datatype.String, AppObject = apttusObject.UniqueId });
            Fields.Add(new ApplicationField { FieldId = apttusObject.IdAttribute, DataType = Datatype.String, AppObject = apttusObject.UniqueId });

            // 2. Add Lookupfield as the 2nd column
            if (apttusObject.ObjectType == ObjectType.Repeating & apttusObject.Parent != null)
            {
                if (apttusObject.LookupName != null)
                    Fields.Add(new ApplicationField { FieldId = apttusObject.LookupName, DataType = Datatype.String, AppObject = apttusObject.UniqueId });
            }

            // 3. Any other WF condition - ToDo

            // 4. Any Retrieve Map field (independent or repeating)
            foreach (RetrieveMap rMap in application.RetrieveMaps)
            {
                List<ApplicationField> RetrieveMapFields = (from f in rMap.RetrieveFields
                                                            where f.AppObject == targetObjectUniqueId && fieldAccess.IsFieldVisible(targetObjectUniqueId, f.FieldId)
                                                            select new ApplicationField { FieldId = f.FieldId, DataType = f.DataType, AppObject = f.AppObject }
                                                            ).ToList();
                Fields.AddRange(RetrieveMapFields);

                // 4.1 Get All Repeating Group fields for this AppObject
                foreach (RepeatingGroup rg in rMap.RepeatingGroups)
                {
                    if (includeRelationalFields && rg.AppObject.Equals(apttusObject.UniqueId))
                    {
                        //Fields.AddRange(rg.RetrieveFields.Select(s => s.FieldId));
                        Fields.AddRange(rg.RetrieveFields.Where(f => fieldAccess.IsFieldVisible(f.AppObject, f.FieldId, rg.AppObject.ToString())).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObject }));
                    }
                    else
                    {
                        // Get fields with for given app object 
                        Fields.AddRange(rg.RetrieveFields.Where(f => f.AppObject == targetObjectUniqueId && fieldAccess.IsFieldVisible(targetObjectUniqueId, f.FieldId, rg.AppObject.ToString())).
                            Select(s => new ApplicationField
                            {
                                FieldId = (rg.AppObject != targetObjectUniqueId && s.FieldId.IndexOf(Constants.DOT) > 0) ? s.FieldId.Substring(s.FieldId.LastIndexOf(Constants.DOT) + 1) : s.FieldId,
                                DataType = s.DataType,
                                AppObject = s.AppObject
                            }));
                    }
                }
                // 4.2 Any Cross Tab fields
                if (rMap.CrossTabMaps != null)
                {
                    foreach (CrossTabRetrieveMap crMap in rMap.CrossTabMaps)
                    {
                        //crMap = rMap.CrossTabMap;
                        if (crMap.RowField.AppObject == targetObjectUniqueId)
                        {
                            Fields.Add(new ApplicationField { FieldId = crMap.RowField.FieldId, DataType = crMap.RowField.DataType, AppObject = crMap.RowField.AppObject });

                            // Get row sort field as apttusfield
                            ApttusField rowSortField = applicationDefinitionManager.GetField(targetObjectUniqueId, crMap.RowSortByFieldId);
                            Fields.Add(new ApplicationField { FieldId = rowSortField.Id, DataType = rowSortField.Datatype, AppObject = crMap.RowField.AppObject });
                        }
                        if (crMap.ColField.AppObject == targetObjectUniqueId)
                        {
                            Fields.Add(new ApplicationField { FieldId = crMap.ColField.FieldId, DataType = crMap.ColField.DataType, AppObject = crMap.ColField.AppObject });

                            // Get col sort field as apttusfield
                            ApttusField colSortField = applicationDefinitionManager.GetField(targetObjectUniqueId, crMap.ColSortByFieldId);
                            Fields.Add(new ApplicationField { FieldId = colSortField.Id, DataType = colSortField.Datatype, AppObject = crMap.ColField.AppObject });
                        }

                        if (crMap.DataField.AppObject == targetObjectUniqueId)
                        {
                            // 1. Add Data field which stores the value
                            ApttusObject dataObject = applicationDefinitionManager.GetAppObject(targetObjectUniqueId);
                            Fields.Add(new ApplicationField { FieldId = crMap.DataField.FieldId, DataType = crMap.DataField.DataType, AppObject = crMap.DataField.AppObject });

                            // 2. Add the Row Lookup field
                            ApttusObject rowObject = applicationDefinitionManager.GetAppObject(crMap.RowField.AppObject);
                            ApttusField rowLookUpField = dataObject.Fields.Where(s => s.Datatype == Datatype.Lookup).Where(s => s.LookupObject.Id.Equals(rowObject.Id)).Select(s => s).FirstOrDefault();
                            Fields.Add(new ApplicationField { FieldId = rowLookUpField.Id, DataType = rowLookUpField.Datatype, AppObject = dataObject.UniqueId });

                            // 3. Add the Column Lookup field
                            ApttusObject colObject = applicationDefinitionManager.GetAppObject(crMap.ColField.AppObject);
                            ApttusField colLookUpField = dataObject.Fields.Where(s => s.Datatype == Datatype.Lookup).Where(s => s.LookupObject.Id.Equals(colObject.Id)).Select(s => s).FirstOrDefault();
                            Fields.Add(new ApplicationField { FieldId = colLookUpField.Id, DataType = colLookUpField.Datatype, AppObject = dataObject.UniqueId });
                        }
                    }
                }
            }

            // 4.5 return all Matrix map fields
            foreach (MatrixMap mMap in application.MatrixMaps)
            {
                // Add all row fields
                Fields.AddRange(mMap.MatrixRow.MatrixFields.Where(mf => fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId)).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add sortfield for each matrix row field 
                Fields.AddRange(mMap.MatrixRow.MatrixFields.Where(mf => !string.IsNullOrEmpty(mf.SortFieldId) && fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId))
                    .Select(s => new ApplicationField { FieldId = s.SortFieldId, DataType = applicationDefinitionManager.GetField(s.AppObjectUniqueID, s.SortFieldId).Datatype, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all column fields
                Fields.AddRange(mMap.MatrixColumn.MatrixFields.Where(mf => fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId)).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // // Add sortfield for each matrix column field 
                Fields.AddRange(mMap.MatrixColumn.MatrixFields.Where(mf => !string.IsNullOrEmpty(mf.SortFieldId) && fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId))
                    .Select(s => new ApplicationField { FieldId = s.SortFieldId, DataType = applicationDefinitionManager.GetField(s.AppObjectUniqueID, s.SortFieldId).Datatype, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Data fields
                Fields.AddRange(mMap.MatrixData.MatrixDataFields.Where(mf => fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId)).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Individual fields
                Fields.AddRange(mMap.IndependentFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Lookup fields for a given data field Object
                foreach (MatrixDataField mdf in mMap.MatrixData.MatrixDataFields.Where(mf => fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId)))
                {
                    if (mdf.AppObjectUniqueID == targetObjectUniqueId)
                    {
                        ApttusObject dataObj = applicationDefinitionManager.GetAppObject(mdf.AppObjectUniqueID);
                        Fields.AddRange(dataObj.Fields.Select(s => new ApplicationField { FieldId = s.Id, DataType = s.Datatype, AppObject = dataObj.UniqueId })
                            .Where(f => f.DataType.Equals(Datatype.Lookup)));
                    }
                }

                // Add all Matrix Component filter fields
                List<MatrixComponent> mComps = mMap.MatrixComponents.Where(comp => comp.AppObjectUniqueID == targetObjectUniqueId).ToList();
                // Loop through all Matrix Components and their respective Filters and filter fields
                foreach (MatrixComponent mComp in mComps)
                {
                    if ((mComp.WhereFilterGroups != null && mComp.WhereFilterGroups.Count > 0) &&
                        (mComp.WhereFilterGroups[0].Filters != null && mComp.WhereFilterGroups[0].Filters.Count > 0))
                    {
                        foreach (SearchFilter filter in mComp.WhereFilterGroups[0].Filters)
                        {
                            // Get apptus field for searchfilter field
                            if (fieldAccess.IsFieldVisible(filter.AppObjectUniqueId, filter.FieldId))
                            {
                                ApttusField filterField = applicationDefinitionManager.GetField(filter.AppObjectUniqueId, filter.FieldId);
                                Fields.Add(new ApplicationField { FieldId = filterField.Id, DataType = filterField.Datatype, AppObject = filter.AppObjectUniqueId });
                            }
                        }
                    }
                }

                foreach (MatrixField colField in mMap.MatrixColumn.MatrixFields)
                {
                    if (colField.MatrixGroupedFields != null)
                    {
                        Fields.AddRange(colField.MatrixGroupedFields.Where(mf => fieldAccess.IsFieldVisible(mf.AppObjectUniqueID, mf.FieldId)).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                            .Where(f => f.AppObject.Equals(targetObjectUniqueId)));
                    }
                }

            }

            // 5. Any Save Map field (based on the input parameter)
            if (includeSaveOtherFields)
            {
                foreach (SaveMap sMap in application.SaveMaps)
                {
                    // Add fields
                    // No need to add retrieved fields of save since those are already added from RetrieveMap loop
                    List<string> SaveMapFields = (from f in sMap.SaveFields
                                                  where f.AppObject == targetObjectUniqueId && f.SaveFieldType == SaveType.SaveOnlyField &&
                                                  fieldAccess.IsFieldVisible(targetObjectUniqueId, f.FieldId)
                                                  select f.FieldId).ToList();
                    ApttusField saveField = null;
                    foreach (string s in SaveMapFields)
                    {
                        saveField = apttusObject.Fields.Where(f => f.Id.Equals(s)).FirstOrDefault();
                        Fields.Add(new ApplicationField { FieldId = saveField.Id, DataType = saveField.Datatype, AppObject = targetObjectUniqueId });
                    }
                    //Fields.AddRange(SaveMapFields);
                }
            }

            // 6. Any Form Action field

            // 7. All filter fields
            foreach (Action action in this.Actions)
            {
                Dictionary<ApttusObject, HashSet<ApttusField>> fieldMap = new Dictionary<ApttusObject, HashSet<ApttusField>>();
                switch (action.Type)
                {
                    case Constants.SEARCH_AND_SELECT_ACTION:
                        SearchAndSelect searchSelectModel = (SearchAndSelect)action;

                        foreach (SearchFilterGroup filterGroup in searchSelectModel.SearchFilterGroups)
                            ExpressionBuilderHelper.GetFilterGroupFields(filterGroup, fieldMap);

                        if (searchSelectModel.TargetObject == apttusObject.UniqueId)
                        {
                            foreach (ResultField rf in searchSelectModel.ResultFields)
                            {
                                ApttusField apttusField = applicationDefinitionManager.GetField(apttusObject.UniqueId, rf.Id);
                                if (apttusField != null && fieldMap.ContainsKey(apttusObject))
                                    fieldMap[apttusObject].Add(apttusField);
                            }
                        }
                        break;
                    case Constants.EXECUTE_QUERY_ACTION:
                        QueryActionModel queryModel = (QueryActionModel)action;
                        if (queryModel.WhereFilterGroups != null)
                        {
                            foreach (SearchFilterGroup filterGroup in queryModel.WhereFilterGroups)
                                ExpressionBuilderHelper.GetFilterGroupFields(filterGroup, fieldMap);
                        }

                        break;

                    default:
                        break;
                }

                if (fieldMap.ContainsKey(apttusObject))
                {
                    foreach (ApttusField field in fieldMap[apttusObject])
                    {
                        if (fieldAccess.IsFieldVisible(apttusObject.UniqueId, field.Id))
                            Fields.Add(new ApplicationField { FieldId = field.Id, DataType = field.Datatype, AppObject = targetObjectUniqueId });
                    }
                }
            }


            // 8. Lookup Name fields for all Lookup (id) fields included. Use unique list of fields.
            List<ApplicationField> UniqueFields = Fields.GroupBy(f => f.FieldId).Select(s => s.First()).ToList();


            // Include Lookup Names if Id exists.
            List<string> LookupNames = (from f in apttusObject.Fields
                                        where UniqueFields.Any(af => af.FieldId.Equals(f.Id)) & f.Datatype == Datatype.Lookup
                                        && f.LookupObject != null && !(Constants.ObjectsWithoutLookupName.Contains(f.LookupObject.Id)) // Exclude objects which dont have name
                                        && !UniqueFields.Any(af => af.FieldId.Equals(applicationDefinitionManager.GetLookupNameFromLookupId(f.Id)))
                                        select applicationDefinitionManager.GetLookupNameFromLookupId(f.Id)).ToList();

            foreach (string nameFields in LookupNames)
            {
                UniqueFields.Add(new ApplicationField { FieldId = nameFields, DataType = Datatype.String, AppObject = targetObjectUniqueId });
            }
            //UniqueFields.AddRange(LookupNames);

            // Include Lookup Id if Name exists.
            List<string> NotIncludedLookupIds = (from s in UniqueFields
                                                 where s.FieldId.EndsWith(Constants.APPENDLOOKUPID)
                                                 && !UniqueFields.Any(af => af.FieldId.Equals(applicationDefinitionManager.GetLookupIdFromLookupName(s.FieldId)))
                                                 select applicationDefinitionManager.GetLookupIdFromLookupName(s.FieldId)).ToList();

            foreach (string IdField in NotIncludedLookupIds)
            {
                UniqueFields.Add(new ApplicationField { FieldId = IdField, DataType = Datatype.String, AppObject = targetObjectUniqueId });
            }

            if (includeRelationalFields)
            {
                List<ApplicationField> lstLookupIds = new List<ApplicationField>();
                //If there is any relational field for e.g If there is a repeating-group for Opportunity Product and we reference Account Region field by
                //following hierarchy OpportunityProduct.Opportunity.Account.Region, then we need to add the ID field of Account in the datatable, so that 
                //save works as expected. 
                //If we have OpportunityProduct.Opportunity.Account.Region in the repeating group, FieldID of Region Field will be 'Opportunity.Account.Region'.
                //Now how do we get the LookupID field of Account in this case ? Extract the last index of '.' and append '.Name'. The newly created field will be
                //'Opportunity.Account.Name' and get/resolve the LookupId using LookupName and finally add the same to the UniqueFields list.
                foreach (ApplicationField field in UniqueFields)
                {
                    if (field.AppObject != apttusObject.UniqueId) //If the Field's AppObject is different than Object's UniqueId, it is a relational side by side field.
                    {
                        int lastIndex = field.FieldId.LastIndexOf('.');
                        if (lastIndex != -1)
                        {
                            string relationalFieldId = field.FieldId.Substring(0, lastIndex);
                            relationalFieldId += Constants.APPENDLOOKUPID;
                            string lookupName = applicationDefinitionManager.GetLookupIdFromLookupName(relationalFieldId);
                            if (!string.IsNullOrEmpty(lookupName) && !UniqueFields.Exists(f => f.FieldId.Equals(lookupName)) && !lstLookupIds.Exists(f => f.FieldId.Equals(lookupName)))
                                lstLookupIds.Add(new ApplicationField { FieldId = lookupName, AppObject = targetObjectUniqueId, DataType = Datatype.String });
                        }
                    }
                }
                UniqueFields.AddRange(lstLookupIds);
            }

            //UniqueFields.AddRange(NotIncludedLookupIds);

            // return the list of Fields
            //return string.Join(",", UniqueFields.ToArray());
            //int intcnt = UniqueFields.RemoveAll(x => x.DataType == Datatype.Attachment);
            return UniqueFields;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="apttusObject"></param>
        /// <param name="includeSaveOtherFields"></param>
        /// <param name="includeRelationalFields"></param>
        /// <returns></returns>
        public List<ApplicationField> GetUsedFieldsOfObject(ApttusObject apttusObject, bool includeSaveOtherFields, bool includeRelationalFields = false)
        {
            Guid targetObjectUniqueId = apttusObject.UniqueId;
            List<ApplicationField> Fields = new List<ApplicationField>();
            ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

            // 1. Any Retrieve Map field (independent or repeating)
            foreach (RetrieveMap rMap in application.RetrieveMaps)
            {
                List<ApplicationField> RetrieveMapFields = (from f in rMap.RetrieveFields
                                                            where f.AppObject == targetObjectUniqueId
                                                            select new ApplicationField { FieldId = f.FieldId, DataType = f.DataType, AppObject = f.AppObject }
                                                            ).ToList();
                Fields.AddRange(RetrieveMapFields);

                // 1.1 Get All Repeating Group fields for this AppObject
                foreach (RepeatingGroup rg in rMap.RepeatingGroups)
                {
                    if (includeRelationalFields && rg.AppObject.Equals(apttusObject.UniqueId))
                    {
                        Fields.AddRange(rg.RetrieveFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObject }));
                        //Fields.AddRange(rg.RetrieveFields.Where(f => fieldAccess.IsFieldVisible(f.AppObject, f.FieldId, rg.AppObject.ToString())).Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObject }));
                    }
                    else
                    {
                        // Get fields with for given app object 
                        Fields.AddRange(rg.RetrieveFields.Where(f => f.AppObject == targetObjectUniqueId).
                            Select(s => new ApplicationField
                            {
                                FieldId = (rg.AppObject != targetObjectUniqueId && s.FieldId.IndexOf(Constants.DOT) > 0) ? s.FieldId.Substring(s.FieldId.LastIndexOf(Constants.DOT) + 1) : s.FieldId,
                                DataType = s.DataType,
                                AppObject = s.AppObject
                            }));
                    }
                }

                if (rMap.CrossTabMaps == null)
                    continue;
                // 1.2 Any Cross Tab fields
                foreach (CrossTabRetrieveMap crMap in rMap.CrossTabMaps)
                {
                    //crMap = rMap.CrossTabMap;
                    if (crMap.RowField.AppObject == targetObjectUniqueId)
                    {
                        Fields.Add(new ApplicationField { FieldId = crMap.RowField.FieldId, DataType = crMap.RowField.DataType, AppObject = crMap.RowField.AppObject });

                        // Get row sort field as apttusfield
                        ApttusField rowSortField = applicationDefinitionManager.GetField(targetObjectUniqueId, crMap.RowSortByFieldId);
                        Fields.Add(new ApplicationField { FieldId = rowSortField.Id, DataType = rowSortField.Datatype, AppObject = crMap.RowField.AppObject });
                    }
                    if (crMap.ColField.AppObject == targetObjectUniqueId)
                    {
                        Fields.Add(new ApplicationField { FieldId = crMap.ColField.FieldId, DataType = crMap.ColField.DataType, AppObject = crMap.ColField.AppObject });

                        // Get col sort field as apttusfield
                        ApttusField colSortField = applicationDefinitionManager.GetField(targetObjectUniqueId, crMap.ColSortByFieldId);
                        Fields.Add(new ApplicationField { FieldId = colSortField.Id, DataType = colSortField.Datatype, AppObject = crMap.ColField.AppObject });
                    }

                    if (crMap.DataField.AppObject == targetObjectUniqueId)
                    {
                        // 1. Add Data field which stores the value
                        ApttusObject dataObject = applicationDefinitionManager.GetAppObject(targetObjectUniqueId);
                        Fields.Add(new ApplicationField { FieldId = crMap.DataField.FieldId, DataType = crMap.DataField.DataType, AppObject = crMap.DataField.AppObject });

                        // 2. Add the Row Lookup field
                        ApttusObject rowObject = applicationDefinitionManager.GetAppObject(crMap.RowField.AppObject);
                        ApttusField rowLookUpField = dataObject.Fields.Where(s => s.Datatype == Datatype.Lookup).Where(s => s.LookupObject.Id.Equals(rowObject.Id)).Select(s => s).FirstOrDefault();
                        Fields.Add(new ApplicationField { FieldId = rowLookUpField.Id, DataType = rowLookUpField.Datatype, AppObject = dataObject.UniqueId });

                        // 3. Add the Column Lookup field
                        ApttusObject colObject = applicationDefinitionManager.GetAppObject(crMap.ColField.AppObject);
                        ApttusField colLookUpField = dataObject.Fields.Where(s => s.Datatype == Datatype.Lookup).Where(s => s.LookupObject.Id.Equals(colObject.Id)).Select(s => s).FirstOrDefault();
                        Fields.Add(new ApplicationField { FieldId = colLookUpField.Id, DataType = colLookUpField.Datatype, AppObject = dataObject.UniqueId });
                    }
                }
            }

            // 1.3 return all Matrix map fields
            foreach (MatrixMap mMap in application.MatrixMaps)
            {
                // Add all row fields
                Fields.AddRange(mMap.MatrixRow.MatrixFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add sortfield for each matrix row field 
                Fields.AddRange(mMap.MatrixRow.MatrixFields.Where(mf => !string.IsNullOrEmpty(mf.SortFieldId))
                    .Select(s => new ApplicationField { FieldId = s.SortFieldId, DataType = applicationDefinitionManager.GetField(s.AppObjectUniqueID, s.SortFieldId).Datatype, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all column fields
                Fields.AddRange(mMap.MatrixColumn.MatrixFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // // Add sortfield for each matrix column field 
                Fields.AddRange(mMap.MatrixColumn.MatrixFields.Where(mf => !string.IsNullOrEmpty(mf.SortFieldId))
                    .Select(s => new ApplicationField { FieldId = s.SortFieldId, DataType = applicationDefinitionManager.GetField(s.AppObjectUniqueID, s.SortFieldId).Datatype, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Data fields
                Fields.AddRange(mMap.MatrixData.MatrixDataFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Individual fields
                Fields.AddRange(mMap.IndependentFields.Select(s => new ApplicationField { FieldId = s.FieldId, DataType = s.DataType, AppObject = s.AppObjectUniqueID })
                    .Where(f => f.AppObject.Equals(targetObjectUniqueId)));

                // Add all Lookup fields for a given data field Object
                foreach (MatrixDataField mdf in mMap.MatrixData.MatrixDataFields)
                {
                    if (mdf.AppObjectUniqueID == targetObjectUniqueId)
                    {
                        ApttusObject dataObj = applicationDefinitionManager.GetAppObject(mdf.AppObjectUniqueID);
                        Fields.AddRange(dataObj.Fields.Select(s => new ApplicationField { FieldId = s.Id, DataType = s.Datatype, AppObject = dataObj.UniqueId })
                            .Where(f => f.DataType.Equals(Datatype.Lookup)));
                    }
                }

                // Add all Matrix Component filter fields
                List<MatrixComponent> mComps = mMap.MatrixComponents.Where(comp => comp.AppObjectUniqueID == targetObjectUniqueId).ToList();
                // Loop through all Matrix Components and their respective Filters and filter fields
                foreach (MatrixComponent mComp in mComps)
                {
                    if ((mComp.WhereFilterGroups != null && mComp.WhereFilterGroups.Count > 0) &&
                        (mComp.WhereFilterGroups[0].Filters != null && mComp.WhereFilterGroups[0].Filters.Count > 0))
                    {
                        foreach (SearchFilter filter in mComp.WhereFilterGroups[0].Filters)
                        {
                            ApttusField filterField = applicationDefinitionManager.GetField(filter.AppObjectUniqueId, filter.FieldId);
                            Fields.Add(new ApplicationField { FieldId = filterField.Id, DataType = filterField.Datatype, AppObject = filter.AppObjectUniqueId });
                        }
                    }
                }

            }

            // 2. Any Save Map field (based on the input parameter)
            if (includeSaveOtherFields)
            {
                foreach (SaveMap sMap in application.SaveMaps)
                {
                    // Add fields
                    // No need to add retrieved fields of save since those are already added from RetrieveMap loop
                    List<string> SaveMapFields = (from f in sMap.SaveFields
                                                  where f.AppObject == targetObjectUniqueId && f.SaveFieldType == SaveType.SaveOnlyField //&&
                                                  //fieldAccess.IsFieldVisible(targetObjectUniqueId, f.FieldId)
                                                  select f.FieldId).ToList();
                    ApttusField saveField = null;
                    foreach (string s in SaveMapFields)
                    {
                        saveField = apttusObject.Fields.Where(f => f.Id.Equals(s)).FirstOrDefault();
                        Fields.Add(new ApplicationField { FieldId = saveField.Id, DataType = saveField.Datatype, AppObject = targetObjectUniqueId });
                    }
                    //Fields.AddRange(SaveMapFields);
                }
            }

            // 3. All filter fields
            foreach (Action action in this.Actions)
            {
                Dictionary<ApttusObject, HashSet<ApttusField>> fieldMap = new Dictionary<ApttusObject, HashSet<ApttusField>>();
                switch (action.Type)
                {
                    case Constants.SEARCH_AND_SELECT_ACTION:
                        SearchAndSelect searchSelectModel = (SearchAndSelect)action;
                        if (searchSelectModel.SearchFilterGroups != null)
                        {
                            foreach (SearchFilterGroup filterGroup in searchSelectModel.SearchFilterGroups)
                                ExpressionBuilderHelper.GetFilterGroupFields(filterGroup, fieldMap);
                        }
                        break;
                    case Constants.EXECUTE_QUERY_ACTION:
                        QueryActionModel queryModel = (QueryActionModel)action;
                        if (queryModel.WhereFilterGroups != null)
                        {
                            foreach (SearchFilterGroup filterGroup in queryModel.WhereFilterGroups)
                                ExpressionBuilderHelper.GetFilterGroupFields(filterGroup, fieldMap);
                        }
                        break;

                    default:
                        break;
                }

                if (fieldMap.ContainsKey(apttusObject))
                {
                    foreach (ApttusField field in fieldMap[apttusObject])
                        Fields.Add(new ApplicationField { FieldId = field.Id, DataType = field.Datatype, AppObject = targetObjectUniqueId });
                }
            }

            // 4. Lookup Name fields for all Lookup (id) fields included. Use unique list of fields.
            List<ApplicationField> UniqueFields = Fields.GroupBy(f => f.FieldId).Select(s => s.First()).ToList();


            // Include Lookup Names if Id exists.
            List<string> LookupNames = (from f in apttusObject.Fields
                                        where UniqueFields.Any(af => af.FieldId.Equals(f.Id)) & f.Datatype == Datatype.Lookup
                                        && f.LookupObject != null && !(Constants.ObjectsWithoutLookupName.Contains(f.LookupObject.Id)) // Exclude objects which dont have name
                                        && !UniqueFields.Any(af => af.FieldId.Equals(applicationDefinitionManager.GetLookupNameFromLookupId(f.Id)))
                                        select applicationDefinitionManager.GetLookupNameFromLookupId(f.Id)).ToList();

            foreach (string nameFields in LookupNames)
            {
                UniqueFields.Add(new ApplicationField { FieldId = nameFields, DataType = Datatype.String, AppObject = targetObjectUniqueId });
            }

            // Include Lookup Id if Name exists.
            List<string> NotIncludedLookupIds = (from s in UniqueFields
                                                 where s.FieldId.EndsWith(Constants.APPENDLOOKUPID)
                                                 && !UniqueFields.Any(af => af.FieldId.Equals(applicationDefinitionManager.GetLookupIdFromLookupName(s.FieldId)))
                                                 select applicationDefinitionManager.GetLookupIdFromLookupName(s.FieldId)).ToList();

            foreach (string IdField in NotIncludedLookupIds)
            {
                UniqueFields.Add(new ApplicationField { FieldId = IdField, DataType = Datatype.String, AppObject = targetObjectUniqueId });
            }

            // return the list of Fields
            //return string.Join(",", UniqueFields.ToArray());
            //int intcnt = UniqueFields.RemoveAll(x => x.DataType == Datatype.Attachment);
            return UniqueFields;
        }

        public RetrieveMap GetRetrieveMapbyTargetNamedRange(string TargetNamedRange)
        {
            RetrieveMap result = null;
            // Match with Repeating fields
            var RetrieveMapRepeating = (from rm in RetrieveMaps
                                        from rg in rm.RepeatingGroups
                                        where rg.TargetNamedRange == TargetNamedRange
                                        select rm);
            if (!RetrieveMapRepeating.Any())
            {
                // Match with Independent fields
                var RetrieveMapIndependent = (from rm in RetrieveMaps
                                              from rf in rm.RetrieveFields
                                              where FieldLevelSecurityManager.Instance.IsFieldVisible(rf.AppObject, rf.FieldId) && rf.TargetNamedRange == TargetNamedRange
                                              select rm);
                if (RetrieveMapIndependent.Any())
                {
                    result = RetrieveMapIndependent.FirstOrDefault();
                }
            }
            else
            {
                result = RetrieveMapRepeating.FirstOrDefault();
            }

            return result;
        }

        public RepeatingGroup GetRepeatingGroupbyTargetNamedRange(string TargetNamedRange)
        {
            RepeatingGroup result = null;
            var RepeatingGroups = (from rm in RetrieveMaps
                                   from rg in rm.RepeatingGroups
                                   where rg.TargetNamedRange == TargetNamedRange
                                   select rg);


            if (RepeatingGroups.Any())
            {
                result = RepeatingGroups.FirstOrDefault();
            }

            return result;
        }

        public RetrieveField GetRetrieveFieldbyTargetNamedRange(string TargetNamedRange)
        {
            RetrieveField result = null;
            var RetrieveFields = (from rm in RetrieveMaps
                                  from f in rm.RetrieveFields
                                  where f.TargetNamedRange == TargetNamedRange && FieldLevelSecurityManager.Instance.IsFieldVisible(f.AppObject, f.FieldId)
                                  select f);


            if (RetrieveFields.Any())
            {
                result = RetrieveFields.FirstOrDefault();
            }

            return result;
        }

        public SaveMap GetSaveMapbyTargetNamedRange(string TargetNamedRange)
        {
            SaveMap result = null;

            // Match with Repeating fields
            var SaveMapRepeating = (from sm in SaveMaps
                                    from sg in sm.SaveGroups
                                    where sg.TargetNamedRange == TargetNamedRange
                                    select sm);
            if (!SaveMapRepeating.Any())
            {
                // Match with Independent fields
                var SaveMapIndependent = (from sm in SaveMaps
                                          from sf in sm.SaveFields
                                          where sf.Type == ObjectType.Independent &&
                                          sf.TargetNamedRange == TargetNamedRange
                                          select sm);
                if (SaveMapIndependent.Any())
                {
                    result = SaveMapIndependent.FirstOrDefault();
                }
            }
            else
            {
                result = SaveMapRepeating.FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// Get Save Map details base on target name range
        /// </summary>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        public SaveMap GetMatrixSaveMapbyTargetNamedRange(string TargetNamedRange)
        {
            SaveMap result = null;
            foreach (MatrixMap mMap in MatrixMaps)
            {
                // Add all rows fields
                if (mMap.MatrixRow.MatrixFields.Exists(sg => sg.TargetNamedRange == TargetNamedRange))
                {
                    result = (from sm in SaveMaps
                              from sf in sm.SaveFields
                              where sf.MatrixMapId == mMap.Id
                              select sm).FirstOrDefault();
                }

                // Get Alll Columns fields
                if (mMap.MatrixColumn.MatrixFields.Exists(sg => sg.TargetNamedRange == TargetNamedRange))
                {
                    result = (from sm in SaveMaps
                              from sf in sm.SaveFields
                              where sf.MatrixMapId == mMap.Id
                              select sm).FirstOrDefault();
                }
            }
            return result;
        }


        /// <summary>
        /// Get MatrixMapID by Range
        /// </summary>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        public MatrixMap GetMatrixMapIDTargetNamedRange(string TargetNamedRange)
        {
            MatrixMap matrixMap = new MatrixMap();
            foreach (MatrixMap mMap in MatrixMaps)
            {
                // check all row fields
                if (mMap.MatrixRow.MatrixFields.Exists(sg => sg.TargetNamedRange == TargetNamedRange))
                {
                    matrixMap.Id = mMap.Id;
                    matrixMap.MatrixData = mMap.MatrixData;
                    matrixMap.MatrixRow = mMap.MatrixRow;
                    matrixMap.MatrixColumn = mMap.MatrixColumn;
                    matrixMap.MatrixComponents = mMap.MatrixComponents;
                }
                // check all column fields
                if (mMap.MatrixColumn.MatrixFields.Exists(sg => sg.TargetNamedRange == TargetNamedRange))
                {
                    matrixMap.Id = mMap.Id;
                    matrixMap.MatrixData = mMap.MatrixData;
                    matrixMap.MatrixRow = mMap.MatrixRow;
                    matrixMap.MatrixColumn = mMap.MatrixColumn;
                    matrixMap.MatrixComponents = mMap.MatrixComponents;
                }
            }
            return matrixMap;
        }

        public SaveGroup GetSaveGroupbyTargetNamedRange(string TargetNamedRange)
        {
            SaveGroup result = null;
            foreach (SaveMap sm in SaveMaps)
            {
                if (sm.SaveGroups.Exists(sg => sg.TargetNamedRange == TargetNamedRange))
                {
                    result = (from sg in sm.SaveGroups
                              where sg.TargetNamedRange == TargetNamedRange
                              select sg).FirstOrDefault();
                    break;
                }
            }
            return result;
        }

        public SaveField GetSaveFieldbyTargetNamedRange(string TargetNamedRange)
        {
            SaveField result = null;
            foreach (SaveMap sm in SaveMaps)
            {
                if (sm.SaveFields.Exists(sf => sf.TargetNamedRange == TargetNamedRange))
                {
                    result = (from sf in sm.SaveFields
                              where sf.TargetNamedRange == TargetNamedRange
                              select sf).FirstOrDefault();
                    break;
                }
            }
            return result;
        }

        public int GetIdColumnIndex(string TargetNamedRange)
        {
            RepeatingGroup currentRepeatingGroup = GetRepeatingGroupbyTargetNamedRange(TargetNamedRange);
            SaveMap currentSaveMap = GetSaveMapbyTargetNamedRange(TargetNamedRange);
            SaveGroup currentSaveGroup = GetSaveGroupbyTargetNamedRange(TargetNamedRange);

            // in Save Only field, current repeating group will be null - so for those instances we need to take appobject from savegroup. 
            ApttusObject obj = null;
            if (currentRepeatingGroup != null) obj = ApplicationDefinitionManager.GetInstance.GetAppObject(currentRepeatingGroup.AppObject);
            else if (currentSaveGroup != null) obj = ApplicationDefinitionManager.GetInstance.GetAppObject(currentSaveGroup.AppObject);

            int IdColumnIndex = 0;
            //if (currentRepeatingGroup != null && currentRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == Constants.ID_ATTRIBUTE))
            if (currentRepeatingGroup != null && currentRepeatingGroup.RetrieveFields.Exists(rf => rf.FieldId == obj.IdAttribute))
                // In case of Save of Displayed and Id Column is in Repeating Group
                IdColumnIndex = (from rf in currentRepeatingGroup.RetrieveFields where rf.FieldId == obj.IdAttribute select rf.TargetColumnIndex).FirstOrDefault();
            else if (currentSaveMap != null && currentSaveGroup != null && currentSaveMap.SaveFields.Exists(sf => sf.FieldId == obj.IdAttribute))
                // In case of Save Other field and Id Column is in Save Field
                IdColumnIndex = (from sf in currentSaveMap.SaveFields where sf.FieldId == obj.IdAttribute & sf.GroupId == currentSaveGroup.GroupId select sf.TargetColumnIndex).FirstOrDefault();

            return IdColumnIndex;
        }

        public List<string> GetSheetNames()
        {
            HashSet<string> sheetNames = new HashSet<string>();

            foreach (RetrieveMap rm in RetrieveMaps)
            {
                // Independent
                foreach (RetrieveField rf in rm.RetrieveFields)
                    sheetNames.Add(rf.TargetLocation.Substring(0, rf.TargetLocation.LastIndexOf(Constants.SHEET_DELIMITER)));

                // Repeating
                foreach (RepeatingGroup rg in rm.RepeatingGroups)
                    foreach (RetrieveField rf in rg.RetrieveFields)
                        sheetNames.Add(rf.TargetLocation.Substring(0, rf.TargetLocation.LastIndexOf(Constants.SHEET_DELIMITER)));
            }
            foreach (SaveMap sm in SaveMaps)
            {
                foreach (SaveField sf in sm.SaveFields.Where(sf => sf.SaveFieldType.Equals(SaveType.SaveOnlyField)))
                {
                    sheetNames.Add(sf.DesignerLocation.Substring(0, sf.DesignerLocation.LastIndexOf(Constants.SHEET_DELIMITER)));
                }
            }
            foreach (MatrixMap mm in MatrixMaps)
            {
                foreach (MatrixField mf in mm.MatrixRow.MatrixFields)
                    sheetNames.Add(mf.TargetLocation.Substring(0, mf.TargetLocation.LastIndexOf(Constants.SHEET_DELIMITER)));

                foreach (MatrixField mf in mm.MatrixColumn.MatrixFields)
                    sheetNames.Add(mf.TargetLocation.Substring(0, mf.TargetLocation.LastIndexOf(Constants.SHEET_DELIMITER)));

                foreach (MatrixField mf in mm.MatrixData.MatrixDataFields)
                    sheetNames.Add(mf.TargetLocation.Substring(0, mf.TargetLocation.LastIndexOf(Constants.SHEET_DELIMITER)));
            }

            return sheetNames.ToList();
        }

        public List<RangeMap> GetRangeMaps()
        {
            RangeMaps = new List<RangeMap>();
            FieldLevelSecurityManager manager = FieldLevelSecurityManager.Instance;

            // 1. Add all Save Map fields
            foreach (SaveMap sm in SaveMaps)
            {
                // Independent
                foreach (SaveField sf in sm.SaveFields.Where(sf => sf.GroupId.Equals(Guid.Empty) && sf.Type.Equals(ObjectType.Independent) &&
                    manager.IsFieldVisible(sf.AppObject, sf.FieldId)))
                {
                    RangeMaps.Add(new RangeMap
                    {
                        RangeName = sf.TargetNamedRange,
                        Type = sf.Type,
                        SaveMapId = sm.Id
                    });
                }

                // Repeating
                foreach (SaveGroup sg in sm.SaveGroups)
                {
                    RangeMaps.Add(new RangeMap
                    {
                        RangeName = sg.TargetNamedRange,
                        Type = ObjectType.Repeating,
                        SaveMapId = sm.Id
                    });
                }
            }

            // 2. Add all Display Map fields
            foreach (RetrieveMap rm in RetrieveMaps)
            {
                // Independent
                foreach (RetrieveField rf in rm.RetrieveFields.Where(fld => FieldLevelSecurityManager.Instance.IsFieldVisible(fld.AppObject, fld.FieldId)))
                {
                    if (!RangeMaps.Exists(r => r.RangeName == rf.TargetNamedRange))
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = rf.TargetNamedRange,
                            Type = rf.Type,
                            RetrieveMapId = rm.Id
                        });
                    }
                    else
                    {
                        // Update the retrieve map id to the existing Range Map (created from Save Field)
                        RangeMap existingRM = (from r in RangeMaps
                                               where r.RangeName == rf.TargetNamedRange
                                               select r).FirstOrDefault();
                        existingRM.RetrieveMapId = rm.Id;
                    }
                }

                // Repeating
                foreach (RepeatingGroup rg in rm.RepeatingGroups)
                {
                    if (!RangeMaps.Exists(r => r.RangeName == rg.TargetNamedRange))
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = rg.TargetNamedRange,
                            Type = ObjectType.Repeating,
                            RetrieveMapId = rm.Id
                        });
                    }
                    else
                    {
                        // Update the retrieve map id to the existing Range Map (created from Save Group)
                        RangeMap existingRM = (from r in RangeMaps
                                               where r.RangeName == rg.TargetNamedRange
                                               select r).FirstOrDefault();
                        existingRM.RetrieveMapId = rm.Id;
                    }
                }
            }


            // 3. Add all Matrix Map fields
            foreach (MatrixMap mm in MatrixMaps)
            {
                foreach (MatrixField mf in mm.MatrixRow.MatrixFields)
                {
                    if (!RangeMaps.Exists(r => r.RangeName == mf.TargetNamedRange))
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = mf.TargetNamedRange,
                            MatrixMapId = mf.Id
                        });
                    }

                }
                foreach (MatrixField mf in mm.MatrixColumn.MatrixFields)
                {
                    if (!RangeMaps.Exists(r => r.RangeName == mf.TargetNamedRange))
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = mf.TargetNamedRange,
                            MatrixMapId = mf.Id
                        });
                    }
                }

                foreach (MatrixField mf in mm.MatrixData.MatrixDataFields)
                {
                    if (!RangeMaps.Exists(r => r.RangeName == mf.TargetNamedRange))
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = mf.TargetNamedRange,
                            MatrixMapId = mf.Id
                        });
                    }
                }

            }

            return RangeMaps;
        }

        /// <summary>
        /// This function overrrides GetRangeMaps, but additionally also adds Repeating fields of Retrieve and Save Maps
        /// </summary>
        /// <returns></returns>
        public List<RangeMap> GetAllRangeMaps()
        {
            List<RangeMap> allRangeMaps = GetRangeMaps();

            foreach (SaveMap sm in SaveMaps)
            {
                // Independent
                foreach (SaveField sf in sm.SaveFields.Where(sf => !sf.GroupId.Equals(Guid.Empty) && sf.SaveFieldType.Equals(SaveType.SaveOnlyField) && sf.Type.Equals(ObjectType.Repeating)))
                {
                    RangeMaps.Add(new RangeMap
                    {
                        RangeName = sf.TargetNamedRange,
                        Type = sf.Type,
                        SaveMapId = sm.Id
                    });
                }
            }

            foreach (RetrieveMap rm in RetrieveMaps)
            {
                foreach (RepeatingGroup rg in rm.RepeatingGroups)
                {
                    foreach (RetrieveField rf in rg.RetrieveFields)
                    {
                        RangeMaps.Add(new RangeMap
                        {
                            RangeName = rf.TargetNamedRange,
                            Type = rf.Type,
                            RetrieveMapId = rm.Id
                        });
                    }
                }
            }

            return allRangeMaps;
        }

        /// <summary>
        /// Returns Named Ranges within the App.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppNamedRanges()
        {
            List<string> namedRanges = new List<string>();

            //Repeating Group named range
            foreach (RetrieveMap rMap in ConfigurationManager.GetInstance.RetrieveMaps)
            {
                foreach (RepeatingGroup repGroup in rMap.RepeatingGroups)
                {
                    namedRanges.Add(repGroup.TargetNamedRange);
                }
            }

            //Save Group named range where fields belong to SaveOnlyField.
            foreach (SaveMap saveMap in ConfigurationManager.GetInstance.SaveMaps)
            {
                foreach (SaveGroup saveGroup in saveMap.SaveGroups)
                {
                    List<SaveField> saveFields = saveMap.SaveFields.Where(sf => sf.GroupId.Equals(saveGroup.GroupId)).ToList();
                    if (saveFields.Where(sf => sf.SaveFieldType != SaveType.SaveOnlyField).Count() == 0)
                        namedRanges.Add(saveGroup.TargetNamedRange);
                }
            }
            return namedRanges;
        }
        #endregion

        #region "Find, Sort"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Action GetActionById(string Id)
        {
            IEnumerable<Action> tags = from action in application.Actions
                                       where action.Id.Equals(Id)
                                       select action;
            if (Id.StartsWith(CPQBase.CPQID))
                return CPQBase.GetCPQAction(Id);
            if (tags.Count() == 1)
                return tags.SingleOrDefault();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Action GetActionByName(string name)
        {
            IEnumerable<Action> tags = from action in application.Actions
                                       where action.Name.Equals(name)
                                       select action;

            if (tags.Count() == 1)
                return tags.SingleOrDefault();
            return null;
        }

        /// <summary>
        /// This function returns List of Objects being used by an Action
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<string> GetActionObjectsById(string Id)
        {
            List<string> lstObjects = new List<string>();

            Action objAction = GetActionById(Id);

            if (objAction.GetType() == typeof(SearchAndSelect))
            {
                SearchAndSelect searchAndSelect = objAction as SearchAndSelect;
                if (searchAndSelect != null)
                {
                    lstObjects = searchAndSelect.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(RetrieveActionModel))
            {
                RetrieveActionModel retrieveAction = objAction as RetrieveActionModel;
                if (retrieveAction == null)
                    return lstObjects;

                //Get the retrieve map to which the retrieve action is reffering to.
                RetrieveMap rMap = RetrieveMaps.Where(retrieveMap => retrieveMap.Id.Equals(retrieveAction.RetrieveMapId)).FirstOrDefault();
                // CrossTabRetrieveMap crossTabMap = CrossTabRetrieveMaps.Where(crMap => crMap.Id.Equals(retrieveAction.RetrieveMapId)).FirstOrDefault();

                MatrixMap mMap = MatrixMaps.Where(matrixMap => matrixMap.Id.Equals(retrieveAction.RetrieveMapId)).FirstOrDefault();

                List<CrossTabRetrieveMap> crossTabMaps = rMap != null ? rMap.CrossTabMaps : null;
                //if (rMap == null && crossTabMaps == null)
                //    return lstObjects;

                //Create a dictionary to object to keep track of which all elements are part of the list.
                Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();

                if (rMap != null)
                {
                    //Independent Retrieve Fields
                    foreach (RetrieveField rField in rMap.RetrieveFields)
                    {
                        if (!dictionary.ContainsKey(rField.AppObject))
                        {
                            ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
                            if (obj != null)
                            {
                                dictionary[rField.AppObject] = obj.Name;
                                lstObjects.Add(obj.UniqueId.ToString());
                            }
                        }
                    }

                    //Add Repeating Group Objects
                    lstObjects.AddRange(rMap.RepeatingGroups.Select(s => s.AppObject.ToString()));
                }

                if (crossTabMaps != null)
                {
                    foreach (CrossTabRetrieveMap CrossTabRetrieveMap in crossTabMaps)
                    {
                        ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabRetrieveMap.RowField.AppObject);
                        lstObjects.Add(obj.UniqueId.ToString());

                        obj = ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabRetrieveMap.ColField.AppObject);
                        lstObjects.Add(obj.UniqueId.ToString());

                        obj = ApplicationDefinitionManager.GetInstance.GetAppObject(CrossTabRetrieveMap.DataField.AppObject);
                        lstObjects.Add(obj.UniqueId.ToString());
                    }
                }

                if (mMap != null)
                {
                    /*  // Old Logic to get Matrix Input objects
                    List<string> matrixObjects = new List<string>();
                    foreach (MatrixField mField in mMap.MatrixRow.MatrixFields)
                        matrixObjects.Add(mField.AppObjectUniqueID.ToString());

                    foreach (MatrixField mField in mMap.MatrixColumn.MatrixFields)
                        matrixObjects.Add(mField.AppObjectUniqueID.ToString());

                    foreach (MatrixField mField in mMap.MatrixData.MatrixDataFields)
                        matrixObjects.Add(mField.AppObjectUniqueID.ToString());

                    foreach (MatrixField mField in mMap.IndependentFields)
                        matrixObjects.Add(mField.AppObjectUniqueID.ToString());

                    lstObjects.AddRange(matrixObjects.Distinct().ToList());
                    */

                    List<string> matrixObjects = new List<string>();

                    // Add all Unique Objects from Data, Row and Column
                    foreach (MatrixDataField dataField in mMap.MatrixData.MatrixDataFields)
                    {
                        MatrixField rowField = mMap.MatrixRow.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();
                        MatrixField colField = mMap.MatrixColumn.MatrixFields.Where(f => f.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();

                        if (rowField.AppObjectUniqueID != dataField.AppObjectUniqueID)
                        {
                            if (!matrixObjects.Exists(m => m.Equals(rowField.AppObjectUniqueID.ToString())))
                                matrixObjects.Add(rowField.AppObjectUniqueID.ToString());
                        }
                        if (colField.AppObjectUniqueID != dataField.AppObjectUniqueID)
                        {
                            if (!matrixObjects.Exists(m => m.Equals(colField.AppObjectUniqueID.ToString())))
                                matrixObjects.Add(colField.AppObjectUniqueID.ToString());
                        }
                        // Check if Row and Column both belongs to same object, but have different Lookups with Data Object.
                        if (rowField.AppObjectUniqueID == colField.AppObjectUniqueID && dataField.RowLookupId != dataField.ColumnLookupId)
                        {
                            // if Yes, add it to the list, Row / Column app object id can be used, since both are same.
                            matrixObjects.Add(rowField.AppObjectUniqueID.ToString());
                        }

                        if (!matrixObjects.Exists(m => m.Equals(dataField.AppObjectUniqueID.ToString())))
                            matrixObjects.Add(dataField.AppObjectUniqueID.ToString());
                    }

                    // Add all Individual Objects
                    foreach (MatrixField individualField in mMap.IndependentFields)
                    {
                        if (!matrixObjects.Exists(m => m.Equals(individualField.AppObjectUniqueID.ToString())))
                            matrixObjects.Add(individualField.AppObjectUniqueID.ToString());
                    }

                    //Add all GroupedColumn Objects
                    foreach (MatrixField colField in mMap.MatrixColumn.MatrixFields)
                        foreach (MatrixField groupedColumn in colField.MatrixGroupedFields)
                        {
                            string appObjId = groupedColumn.AppObjectUniqueID.ToString();
                            if (!matrixObjects.Exists(m => m.Equals(appObjId)))
                                matrixObjects.Add(appObjId);
                        }

                    lstObjects.AddRange(matrixObjects);
                }

                dictionary.Clear();
                dictionary = null;
            }
            else if (objAction.GetType() == typeof(QueryActionModel))
            {
                QueryActionModel model = objAction as QueryActionModel;
                if (model != null)
                {
                    if (model.QueryType == QueryTypes.SELECT)
                        lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(DeleteActionModel))
            {
                DeleteActionModel model = objAction as DeleteActionModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(CallProcedureModel))
            {
                CallProcedureModel model = objAction as CallProcedureModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(SaveAttachmentModel))
            {
                SaveAttachmentModel model = objAction as SaveAttachmentModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(CheckInModel))
            {
                CheckInModel model = objAction as CheckInModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(CheckOutModel))
            {
                CheckOutModel model = objAction as CheckOutModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }
            }
            else if (objAction.GetType() == typeof(AddRowActionModel))
            {
                AddRowActionModel model = objAction as AddRowActionModel;
                if (model != null)
                    lstObjects = model.GetInputObjects();
            }
            else if (objAction.GetType() == typeof(DataSetActionModel))
            {
                DataSetActionModel dataSetAction = objAction as DataSetActionModel;
                if (dataSetAction != null)
                    lstObjects = dataSetAction.GetInputObjects();
            }
            else if (objAction.Id.StartsWith(CPQBase.CPQID))
            {
                lstObjects = CPQBase.GetCPQModel(objAction);
            }


            return lstObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Guid GetActionTargetObjectsById(string Id)
        {
            Action objAction = GetActionById(Id);

            if (objAction.GetType() == typeof(SearchAndSelect))
            {
                SearchAndSelect model = objAction as SearchAndSelect;
                if (model != null)
                    return model.TargetObject;
            }
            else if (objAction.GetType() == typeof(QueryActionModel))
            {
                QueryActionModel model = objAction as QueryActionModel;
                if (model != null)
                    return model.TargetObject;
            }
            else if (objAction.GetType() == typeof(CallProcedureModel))
            {
                // TODO:: Call Proc do not have targetObject, check options on what can be done for it.
                CallProcedureModel model = objAction as CallProcedureModel;
                if (model != null)
                    return model.ReturnObject;
            }
            else if (objAction.GetType() == typeof(CreateCartModel))
            {
                // TODO:: Call Proc do not have targetObject, check options on what can be done for it.
                CreateCartModel model = objAction as CreateCartModel;
                if (model != null)
                    return model.ReturnObject;
            }
            else if (objAction.GetType() == typeof(ApplyPricingModel))
            {
                // TODO:: Call Proc do not have targetObject, check options on what can be done for it.
                ApplyPricingModel model = objAction as ApplyPricingModel;
                if (model != null)
                    return model.ReturnObject;
            }
            else if (objAction.GetType() == typeof(FinalizeCartModel))
            {
                // TODO:: Call Proc do not have targetObject, check options on what can be done for it.
                FinalizeCartModel model = objAction as FinalizeCartModel;
                if (model != null)
                    return model.ReturnObject;
            }
            else if (objAction.GetType() == typeof(CreateEmptyCartModel))
            {
                // TODO:: Call Proc do not have targetObject, check options on what can be done for it.
                CreateEmptyCartModel model = objAction as CreateEmptyCartModel;
                if (model != null)
                    return model.ReturnObject;
            }
            else if (objAction.GetType() == typeof(DataSetActionModel))
            {
                DataSetActionModel model = objAction as DataSetActionModel;
                if (model != null)
                    return model.TargetObject;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appObjectId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetWorkflowOutputData()
        {
            Dictionary<string, string> lstPersistedData = new Dictionary<string, string>();

            foreach (Workflow wfconfig in this.Workflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfconfig;

                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.WorkflowActionData != null && objAction.WorkflowActionData.OutputPersistData)
                                lstPersistedData.Add(objAction.WorkflowActionData.Id.ToString(), objAction.WorkflowActionData.OutputDataName);
                        }
                    }
                }
            }
            return lstPersistedData;
        }


        public WorkflowActionData GetWorkflowOutputDataById(string wfDataId)
        {
            foreach (Workflow wfconfig in this.Workflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfconfig;

                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.WorkflowActionData != null && objAction.WorkflowActionData.OutputPersistData && objAction.WorkflowActionData.Id == Guid.Parse(wfDataId))
                                return objAction.WorkflowActionData;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<WorkflowActionData> GetWorkflowActionData()
        {
            List<WorkflowActionData> lstActionData = new List<WorkflowActionData>();

            foreach (Workflow wfconfig in this.Workflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfconfig;

                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.WorkflowActionData != null)
                                lstActionData.Add(objAction.WorkflowActionData);
                        }
                    }
                }
            }
            return lstActionData;
        }

        public WorkflowAction GetWorkflowActionByWorkflowDataId(string wfDataId)
        {
            foreach (Workflow wfconfig in this.Workflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfconfig;

                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.WorkflowActionData != null && objAction.WorkflowActionData.Id == Guid.Parse(wfDataId))
                                return objAction;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public List<SwitchConnectionActionModel> GetWorkflowSwitchConnectionActions(WorkflowStructure workflow)
        {
            List<SwitchConnectionActionModel> connections = new List<SwitchConnectionActionModel>();

            foreach (Step s in workflow.Steps)
            {
                foreach (Condition c in s.Conditions)
                {
                    foreach (WorkflowAction objAction in c.WorkflowActions)
                    {
                        if (objAction.WorkflowActionData != null)
                        {
                            Action wfAction = GetActionById(objAction.ActionId);
                            if (wfAction != null && wfAction.Type.Equals(Constants.SWITCH_CONNECTION_ACTION))
                                connections.Add(wfAction as SwitchConnectionActionModel);
                        }
                    }
                }
            }

            return connections;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Workflow GetWorkflowById(string Id)
        {
            IEnumerable<Workflow> tags = from workflow in application.Workflows
                                         where workflow.Id.ToString().Equals(Id)
                                         select workflow;
            if (tags.Count() == 1)
                return tags.SingleOrDefault();

            return null;
        }

        public Workflow GetWorkflowById(Guid Id)
        {
            IEnumerable<Workflow> tags = from workflow in application.Workflows
                                         where workflow.Id == Id
                                         select workflow;
            if (tags.Count() == 1)
                return tags.SingleOrDefault();

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Workflow GetWorkflowByName(string wfName)
        {
            IEnumerable<Workflow> tags = from workflow in application.Workflows
                                         where workflow.Name.Equals(wfName)
                                         select workflow;
            if (tags.Count() == 1)
                return tags.SingleOrDefault();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsWorkflowOutputDataExists(string outputDataName)
        {
            bool count = false;

            foreach (Workflow wf in application.Workflows)
            {
                WorkflowStructure wfStructure = (WorkflowStructure)wf;

                count = wfStructure.Steps.Any(
                                s => s.Conditions.Any(
                                    c => c.WorkflowActions.Any(
                                        w => w.WorkflowActionData == null || w.WorkflowActionData.OutputDataName == null ? false : w.WorkflowActionData.OutputDataName.Equals(outputDataName))));

                if (count)
                    return count;
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputId"></param>
        /// <returns></returns>
        public string GetOutputDataName(string outputId)
        {
            string dataName = string.Empty;

            foreach (Workflow wfConfig in application.Workflows)
            {
                WorkflowStructure wf = (WorkflowStructure)wfConfig;
                foreach (Step s in wf.Steps)
                {
                    foreach (Condition c in s.Conditions)
                    {
                        foreach (WorkflowAction objAction in c.WorkflowActions)
                        {
                            if (objAction.WorkflowActionData != null && objAction.WorkflowActionData.Id == Guid.Parse(outputId))
                                return objAction.WorkflowActionData.OutputDataName;
                        }
                    }
                }
            }

            return dataName;
        }

        #endregion

        #region "Delete"

        public bool DeleteAction(string Id)
        {
            IEnumerable<Action> tags = from action in application.Actions
                                       where action.Id.Equals(Id.ToString())
                                       select action;
            if (tags.Count() == 1)
            {
                Action action = tags.SingleOrDefault();
                return application.Actions.Remove(action);
            }
            return false;
        }

        #endregion

        public Dictionary<Guid, string> GetApttusObjectsWithObjectType()
        {
            Dictionary<Guid, string> list = new Dictionary<Guid, string>();
            foreach (ApttusObject obj in ApplicationDefinitionManager.GetInstance.GetAllObjects())
            {
                string nameWithType = new StringBuilder(obj.Name).Append(" (").Append(obj.ObjectType.GetDescription()).Append(")").ToString();
                list.Add(obj.UniqueId, nameWithType);
            }
            return list;
        }

        public bool IsRichTextEditingDisabled {
            get {
                bool bIsRichTextEditingDisabled = true;
                //For Version Before 3.0, AppSettings will not be available, hence check whether AppSetting is there or not.
                bool bIsAppSettingsAvailable = Application != null && Application.Definition.AppSettings != null;
                if (bIsAppSettingsAvailable)
                    bIsRichTextEditingDisabled = Application.Definition.AppSettings.DisableRichTextEditing;
                return bIsRichTextEditingDisabled;
            }
        }

        /// <summary>
        /// This method is used for Get retrieveMap
        /// </summary>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        public RetrieveMap GetRetrieveMapbyFieldNamedRange(string TargetNamedRange)
        {
            RetrieveMap result = null;
            // Match with Repeating fields

            var RetrieveMapRepeating = (from rm in RetrieveMaps
                                        from rg in rm.RepeatingGroups
                                        from f in rg.RetrieveFields
                                        where f.TargetNamedRange == TargetNamedRange && FieldLevelSecurityManager.Instance.IsFieldVisible(f.AppObject, f.FieldId, rg.AppObject.ToString())
                                        select rm);



            if (!RetrieveMapRepeating.Any())
            {

            }
            else
            {
                result = RetrieveMapRepeating as RetrieveMap;
            }

            return result;
        }

        /// <summary>
        /// This method is used for Save other case
        /// </summary>
        /// <param name="TargetNameRange"></param>
        /// <returns></returns>
        public SaveMap GetSaveOtherFieldNameRange(string TargetNameRange)
        {
            SaveMap result = null;
            result = (from sm in SaveMaps
                      from sg in sm.SaveGroups
                      from sf in sm.SaveFields
                      where sg.TargetNamedRange == TargetNameRange
                      && sf.SaveFieldType == SaveType.SaveOnlyField
                      select sm).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Check whether this range belongs to Retrieve Map or Matrix app
        /// </summary>
        /// <param name="TargetNamedRange"></param>
        /// <returns></returns>
        public string GetMapbyNamedRange(string TargetNamedRange)
        {
            string mapName = string.Empty;
            RetrieveMap retrieveResult = null;
            SaveMap saveMapResult = null;
            MatrixMap mapResult = null;
            // This method is used for Display map case
            retrieveResult = GetRetrieveMapbyTargetNamedRange(TargetNamedRange);
            // This method is used for Save other case
            saveMapResult = GetSaveOtherFieldNameRange(TargetNamedRange);

            if (retrieveResult != null && retrieveResult.Id != Guid.Empty)
                mapName = Constants.DISPLAYMAP_NAME;
            else if (saveMapResult != null && saveMapResult.Id != Guid.Empty)
                mapName = Constants.DISPLAYMAP_NAME;
            else
            {
                mapResult = GetMatrixMapIDTargetNamedRange(TargetNamedRange);
                if (mapResult != null && mapResult.Id != Guid.Empty)
                    mapName = Constants.MATRIXMAP_NAME;
            }
            return mapName;
        }

        public string AddExternalLibrary(ExternalLibrary externalAction)
        {
            if (string.IsNullOrEmpty(externalAction.Id))
                externalAction.Id = Guid.NewGuid().ToString().Replace("-", string.Empty);

            application.ExternalLibraries.Add(externalAction);
            return externalAction.Id;
        }

        internal string GetIdAttribute(ApttusObject appObject, bool p)
        {
            throw new NotImplementedException();
        }
    }
}
