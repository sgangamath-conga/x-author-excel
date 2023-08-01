using System;
using System.Data;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using Microsoft.Xrm.Sdk.Query;

namespace Apttus.XAuthor.AppRuntime
{
    class DynamicsCheckOutProvider : CheckInOutCommonAttributes, ICheckOutProvider
    {
        public DynamicsCheckOutProvider() : base(CRM.DynamicsCRM) { }

        public string GetBodyStringFromDataTable(DataTable table)
        {
            return table.Rows[0]["documentbody"].ToString();
        }
        public string GetFileNameFromDataTable(DataTable table)
        {
            return table.Rows[0]["filename"].ToString();
        }

        public XAuthorQuery GetFileBlobFromAttachmentsQuery(string selectedDocId)
        {
            var query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "annotation",
                ColumnSet = new ColumnSet("filename", "documentbody"),
                Criteria =
                        {
                            Filters =
                            {
                                new FilterExpression
                                {
                                    FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression("annotationid", ConditionOperator.Equal, selectedDocId)
                                        }
                                }
                            }
                        }

            };

            return new DynamicsQuery() { Query = query };
        }
        public XAuthorQuery GetFileQuery(string RecordId)
        {
            var query = new QueryExpression()
            {
                Distinct = true,
                EntityName = "apttus_xapps_appfile",
                ColumnSet = new ColumnSet("apttus_xapps_appfileid", "apttus_name", "apttus_xapps_fileid", "apttus_xapps_filetype", "apttus_xapps_islocked", "apttus_xapps_parentid", "apttus_xapps_objectname", "modifiedon", "modifiedby"),
                Criteria =
                            {
                                Filters =
                                {
                                    new FilterExpression
                                    {
                                        FilterOperator = Microsoft.Xrm.Sdk.Query.LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression("apttus_xapps_parentid", ConditionOperator.Equal, RecordId)
                                        }
                                    }
                                }
                            },
                LinkEntities =
                            {
                                new LinkEntity
                                {
                                    JoinOperator = JoinOperator.Inner,
                                    LinkFromAttributeName = "modifiedby",
                                    LinkFromEntityName = "apttus_xapps_appfile",
                                    LinkToAttributeName = "systemuserid",
                                    LinkToEntityName = "systemuser",
                                    Columns = new ColumnSet("fullname")
                                }
                            }
            };

            return new DynamicsQuery() { Query = query };
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
            grid.Columns["Id"].DataPropertyName = "apttus_xapps_appfileid";
            grid.Columns["DocId"].DataPropertyName = "apttus_xapps_fileid";
            grid.Columns["AppName"].DataPropertyName = "apttus_name";
            grid.Columns["Locked"].DataPropertyName = "apttus_xapps_islocked";
            grid.Columns["LastModifiedBy"].DataPropertyName = "systemuser1.fullname";
            grid.Columns["ModifiedOn"].DataPropertyName = "modifiedon";
            grid.Columns["LastModifiedById"].DataPropertyName = "modifiedby";
        }

        public Datatype GetIsFileLockedDataType()
        {
            return Datatype.Boolean;
        }
    }
}
