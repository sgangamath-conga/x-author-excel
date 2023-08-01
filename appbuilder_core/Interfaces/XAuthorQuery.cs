using System.Data;

namespace Apttus.XAuthor.Core
{
    public class XAuthorQuery
    {
        public ApttusUserInfo UserInfo { get; set; }
        public ApttusObject Object { get; set; }
        public DataTable DataTable { get; set; }
    }

    public class SalesforceQuery : XAuthorQuery
    {
        public string SOQL { get; set; }
    }

    public class DynamicsQuery : XAuthorQuery
    {
        public Microsoft.Xrm.Sdk.Query.QueryExpression Query { get; set; }
    }

    public class AICQuery : XAuthorQuery
    {
        public Apttus.DataAccess.Common.Model.Query Query { get; set; }
    }
}
