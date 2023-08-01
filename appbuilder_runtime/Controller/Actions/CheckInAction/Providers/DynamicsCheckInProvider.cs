using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    class DynamicsCheckInProvider :CheckInOutCommonAttributes, ICheckInProvider
    {
        public DynamicsCheckInProvider() : base(CRM.DynamicsCRM) { }

        private string FileId = "apttus_xapps_fileid";
        private string FileType = "apttus_xapps_filetype";
        private string ParentRecordId = "apttus_xapps_parentid";
        private string ParentObjectName = "apttus_xapps_objectname";
        private string NameAttribute = "apttus_name";

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
