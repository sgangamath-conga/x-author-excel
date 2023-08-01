/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// This class manages the object operations with underlying CRM systems like Salesforce
    /// </summary>
    public class ObjectManager
    {
        // Intialize Adapter
        private static object syncRoot = new Object();
        private static ObjectManager instance;

        private static IAdapter adapter;
        private ILookupIdAndLookupNameProvider lookupIdAndNameProvider;

        private static Dictionary<String, ApttusObject> _ApttusObjectCache = null;

        private static RuntimeMode runtimeMode;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        private ObjectManager()
        {
            _ApttusObjectCache = null;
        }

        public IAdapter GetAdapter()
        {
            return adapter;
        }
        public static void ResetObjectCache()
        {
            _ApttusObjectCache = null;
        }
        public void RefreshConnection()
        {
            adapter.RefreshConnection();
        }
        private static void InitializeObjectCache()
        {
            _ApttusObjectCache = new Dictionary<String, ApttusObject>();
        }

        public static void SetCRM(CRM crm)
        {
            switch (crm)
            {
                case CRM.Salesforce:
                    adapter = new ABSalesforceAdapter();
                    break;
                case CRM.DynamicsCRM:
                    adapter = new ABDynamicsCRMAdapter();
                    break;
                case CRM.AIC:
                    adapter = new ABAICAAdapter();
                    break;
                case CRM.AICV2:
                    adapter = new ABAICV2Adapter();
                    break;
            }
        }

        public static RuntimeMode RuntimeMode {
            get { return runtimeMode; }
            set { runtimeMode = value; }
        }

        public static ObjectManager GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ObjectManager();
                    }
                }

                return instance;
            }
        }

        #region "Public Methods"

        public bool IsDesigner()
        {
            return adapter.IsDesigner();
        }
        public bool IsSandBox()
        {
            return adapter.IsSandBox();
        }
        public object[] DataSetToObject(ApttusDataSet dataSet)
        {
            return adapter.DataSetToObject(dataSet);
        }

        public bool Connect(IXAuthorCRMLogin login)
        {
            return adapter.Connect(login);
        }

        public string GetPartnerAPIVersion()
        {
            return adapter.GetPartnerAPIVersion();
        }

        public ApttusUserInfo getUserInfo()
        {
            return adapter.UserInfo;
        }

        public List<ApttusObject> GetAppList(int? maxRecords, string searchName, string ownerId)
        {
            RefreshConnection();
            return adapter.GetAppList(maxRecords, searchName, ownerId);
        }

        public List<ApttusObject> GetAllStandardObjects()
        {
            return adapter.GetAllStandardObjects();
        }

        public string GetNamespacePrefix()
        {
            return adapter.getNamespacePrefix();
        }

        // Fresh copy should be "True" only for "Runtime" during app sync
        public ApttusObject GetApttusObject(string ObjectId, bool freshCopy, bool refreshChildObjects = true, int noofChildObjectsLoaded = 0)
        {
            if (_ApttusObjectCache == null)
                InitializeObjectCache();

            ApttusObject rApttusObject = null;

            if ((!freshCopy) && _ApttusObjectCache.ContainsKey(ObjectId))
            {
                ApttusObject instance = new ApttusObject();
                instance = _ApttusObjectCache[ObjectId];
                rApttusObject = instance;
            }
            else
            {
                ApttusObject apttusObject = adapter.GetObjectAndFields(ObjectId, refreshChildObjects, noofChildObjectsLoaded);
                AddAttachmentField(apttusObject);
                // This condition is added for race condition when object is added 
                // when adapter.GetObjectAndFields is executing. Remove and Add new version.
                // if object is fully loaded then only add it to cache else let system make salesforce call to rertrieve all child objects.
                if (apttusObject.IsFullyLoaded == true)
                {
                    if (_ApttusObjectCache.ContainsKey(ObjectId))
                        _ApttusObjectCache.Remove(ObjectId);
                    _ApttusObjectCache.Add(ObjectId, apttusObject);
                }

                rApttusObject = apttusObject;
            }

            return rApttusObject.DeepCopy();
        }

        private void AddAttachmentField(ApttusObject apttusObject)
        {
            if (!Constants.OBJECTSEXCLUDEDFROMATTACHMENT.Exists(o => o == apttusObject.Id))
            {
                apttusObject.Fields.Add(new ApttusField
                {
                    Id = Constants.ATTACHMENT,
                    Name = Constants.ATTACHMENT,
                    Datatype = Datatype.Attachment,
                    Updateable = true,
                    Visible = true
                });
            }
        }
        public ApttusDataSet GetDataSetByLookupSearch(ApttusObject lookupObj, string search = "")
        {
            return adapter.GetDataSetByLookupSearch(lookupObj, search);
        }
        public void FillRecordTypeMetadata(ApttusObject oApttusObject)
        {
            adapter.FillRecordTypeMetadata(oApttusObject);
        }

        public List<ApttusObject> Query(XAuthorQuery query)
        {
            return adapter.Query<ApttusObject>(query);
        }

        public ApttusDataSet QueryDataSet(XAuthorQuery query)
        {
            return adapter.QueryDataSet(query);
        }
        public ApttusDataSet GetLookUpDetails(ApttusObject apttusObject, List<string> LookupNames)
        {
            return adapter.GetLookUpDetails(apttusObject, LookupNames);
        }
        public ApttusDataSet GetAttachmentsDataSet(string ParentIds, string ObjectId)
        {
            return adapter.GetAttachmentsDataSet(ParentIds, ObjectId);
        }
        public void Insert(List<ApttusSaveRecord> InsertObjects, bool enablePartialSave, int BatchSize = Constants.SAVE_ACTION_BATCH_SIZE, WaitWindowView waitWindow = null)
        {
            adapter.Insert(InsertObjects, enablePartialSave, BatchSize, waitWindow);
        }

        public void Update(List<ApttusSaveRecord> UpdateObjects, bool enablePartialSave, int BatchSize = Constants.SAVE_ACTION_BATCH_SIZE, WaitWindowView waitWindow = null)
        {
            adapter.Update(UpdateObjects, enablePartialSave, BatchSize, waitWindow);
        }

        public void Upsert(List<ApttusSaveRecord> UpsertObjects, bool enablePartialSave, int BatchSize = Constants.SAVE_ACTION_BATCH_SIZE, WaitWindowView waitWindow = null)
        {
            adapter.Upsert(UpsertObjects, enablePartialSave, BatchSize, waitWindow);
        }
        public void Delete(List<ApttusSaveRecord> DeleteObjects, bool enablePartialSave, WaitWindowView waitWindow = null)
        {
            // Delete Action is not currently required to implement Batch Size configuration so default value is set.
            adapter.Delete(DeleteObjects, enablePartialSave, Constants.SAVE_ACTION_BATCH_SIZE, waitWindow);
        }

        public ApttusDataSet Search(string soql, ApttusUserInfo userInfo)
        {
            return adapter.Search(soql, userInfo);
        }

        public string EscapeQueryString(string queryString)
        {
            return adapter.EscapeQueryString(queryString);
        }

        public string UnescapeQueryString(string queryString)
        {
            return adapter.UnescapeQueryString(queryString);
        }

        public string GetExceptionCode(Exception ex)
        {
            return adapter.GetExceptionCode(ex);
        }

        public bool IsAdminUser()
        {
            return adapter.IsAdminUser();
        }

        internal ApttusDataSet GetAppDataSet(int? maxRecords, string searchName, string ownerId, ApttusUserInfo userInfo)
        {
            RefreshConnection();
            var dataSet = adapter.GetAppDataSet(maxRecords, searchName, ownerId, userInfo);
            var dataTable = dataSet.DataTable;
            adapter.SetAppDataTableProperties(ref dataTable);
            return dataSet;
        }
        #endregion

        public object ExcelApp {
            get;
            set;
        }

        public ILookupIdAndLookupNameProvider GetLookUpIdAndNameProvider()
        {
            return adapter.GetLookUpIdAndNameProvider();
        }

        public string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames)
        {
            return adapter.GetObjectNameByPrimaryId(primaryID, objectNames);
        }

        public AppAssignmentModel GetAppAssignmnetDetails(string AppId)
        {
            return adapter.GetAppAssignmentDetails(AppId);
        }
        public List<User> GetUserList(string searchStr, string ExceptIds = "")
        {
            return adapter.GetUsersList(searchStr, ExceptIds);
        }
        public List<Profile> GetProfileList(string searchStr, string ExceptIds = "")
        {
            return adapter.GetProfileList(searchStr, ExceptIds);
        }
    }
}
