using Apttus.XAuthor.Core;
using System.Data;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppRuntime
{
    public interface ICheckOutProvider
    {
        XAuthorQuery GetFileQuery(string RecordId);
        XAuthorQuery GetFileBlobFromAttachmentsQuery(string selectedDocId);
        string GetObjectName();
        string GetIsFileLocked();
        Datatype GetIsFileLockedDataType();
        string GetObjectPrimaryId();
        string GetFileNameFromDataTable(DataTable table);
        string GetBodyStringFromDataTable(DataTable table);
        void SetDataPropertyOfGrid(ref DataGridView grid);
    }
}
