using System;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    class SalesforceCheckInProvider : CheckInOutCommonAttributes, ICheckInProvider
    {
        public SalesforceCheckInProvider() : base(CRM.Salesforce) { }

        private string FileId = Constants.NAMESPACE_PREFIX + Constants.APP_FILE_FileId__c;
        private string FileType = Constants.NAMESPACE_PREFIX + Constants.APP_FILE_FileType__c;
        private string ParentRecordId = Constants.NAMESPACE_PREFIX + Constants.APP_FILE_ParentId__c;
        private string ParentObjectName = Constants.NAMESPACE_PREFIX + Constants.APP_FILE_ObjectName__c;
        private string NameAttribute = Constants.NAME_ATTRIBUTE;

        public string GetFileId()
        {
            return FileId;
        }

        public Datatype GetFileLockedDataType()
        {
            return FileLockedDataType;
        }

        public string GetFileType()
        {
            return FileType;
        }

        public string GetIDAttribute()
        {
            return IdAttribute;
        }

        public string GetNameAttribute()
        {
            return NameAttribute;
        }

        public string GetObjectName()
        {
            return ObjectName;
        }

        public string GetParentObjectName()
        {
            return ParentObjectName;
        }

        public string GetParentRecordId()
        {
            return ParentRecordId;
        }

        public string IsFileLocked()
        {
            return FileLocked;
        }
    }
}
