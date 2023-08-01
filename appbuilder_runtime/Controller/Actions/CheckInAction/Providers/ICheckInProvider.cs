using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public interface ICheckInProvider
    {
        string GetObjectName();
        string GetParentObjectName();
        string GetFileId();
        string GetFileType();
        string IsFileLocked();
        Datatype GetFileLockedDataType();
        string GetParentRecordId();
        string GetNameAttribute();
        string GetIDAttribute();
    }
}
