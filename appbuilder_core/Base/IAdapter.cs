/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace Apttus.XAuthor.Core
{
    public interface IAdapter
    {
        bool IsDesigner();

        bool Connect(IXAuthorCRMLogin login);

        List<ApttusObject> GetAllStandardObjects();

        ApttusObject GetObjectAndFields(string objectName, bool refreshChildObjects, int noofChildObjectsLoaded);

        void FillRecordTypeMetadata(ApttusObject apttusObject);
        ApttusUserInfo UserInfo { get; }

        List<ApttusObject> GetAppList(int? maxRecords, string searchName, string ownerId);

        ApttusDataSet GetAppDataSet(int? maxRecords, string searchName, string ownerId, ApttusUserInfo userInfo);

        List<ApttusObject> Query<T>(XAuthorQuery query);

        ApttusDataSet QueryDataSet(XAuthorQuery query);

        void Insert(List<ApttusSaveRecord> objects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow);

        void Update(List<ApttusSaveRecord> objects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow);

        void Upsert(List<ApttusSaveRecord> objects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow);

        void Delete(List<ApttusSaveRecord> objects, bool enablePartialSave, int BatchSize, WaitWindowView waitWindow);

        object[] DataSetToObject(ApttusDataSet dataSet);

        string getNamespacePrefix();

        ApttusDataSet Search(string soql, ApttusUserInfo userInfo);

        string EscapeQueryString(string queryString);

        string UnescapeQueryString(string queryString);

        string GetExceptionCode(Exception ex);

        string GetPartnerAPIVersion();

        bool IsAdminUser();
        string GetLicenseDetail();
        string GetFeatureDetail();
        bool IsSandBox();
        bool IsAdminPackageEditionUser();
        string GetLicenseFilePath();
        bool HasCollisionDetection();
        ILookupIdAndLookupNameProvider GetLookUpIdAndNameProvider();
        void SetAppDataTableProperties(ref DataTable table);
        ApttusDataSet GetAttachmentsDataSet(string ParentIds, string ObjectId);
        ApttusDataSet GetLookUpDetails(ApttusObject apttusObject, List<string> LookupNames);
        ApttusDataSet GetDataSetByLookupSearch(ApttusObject lookupObj, string search);
        string GetObjectNameByPrimaryId(Guid primaryID, List<string> objectNames);
        void RefreshConnection();
        AppAssignmentModel GetAppAssignmentDetails(string AppId);
        List<User> GetUsersList(string searchStr, string ExceptIds);
        List<Profile> GetProfileList(string searchStr, string ExceptIds);
    }
}
