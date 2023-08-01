using System;
using System.Data;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    class SalesforceCheckOutProvider :CheckInOutCommonAttributes, ICheckOutProvider
    {
        public SalesforceCheckOutProvider() : base(CRM.Salesforce) { }

        public string GetBodyStringFromDataTable(DataTable table)
        {
            return table.Rows[0]["Body"].ToString();
        }

        public string GetFileNameFromDataTable(DataTable table)
        {
            return table.Rows[0]["Name"].ToString();
        }

        public XAuthorQuery GetFileBlobFromAttachmentsQuery(string selectedDocId)
        {
            return new SalesforceQuery() { SOQL = "Select Name, Body from Attachment where Id = '" + selectedDocId + "'" };
        }

        public XAuthorQuery GetFileQuery(string RecordId)
        {
            var soql = "Select Id, " + Constants.NAMESPACE_PREFIX + "FileId__c, Name, " + Constants.NAMESPACE_PREFIX + "IsLocked__c, LastModifiedBy.Id, LastModifiedBy.Name, LastModifiedDate " +
                                  "from " + Constants.NAMESPACE_PREFIX + "AppFile__c " +
                                  "where " + Constants.NAMESPACE_PREFIX + "ParentId__c = '" + RecordId + "' " +
                                  "Order By LastModifiedDate desc";

            return new SalesforceQuery() { SOQL = soql };
        }

        public string GetIsFileLocked()
        {
            return FileLocked;
        }

        public string GetObjectName()
        {
            return ObjectName;
        }

        public string GetObjectPrimaryId()
        {
            return IdAttribute;
        }

        public void SetDataPropertyOfGrid(ref DataGridView grid)
        {
        }

        public Datatype GetIsFileLockedDataType()
        {
            return Datatype.String;
        }
    }
}
