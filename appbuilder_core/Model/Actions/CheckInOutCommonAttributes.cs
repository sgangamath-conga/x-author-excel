namespace Apttus.XAuthor.Core
{
    public class CheckInOutCommonAttributes
    {
        protected Datatype FileLockedDataType;
        protected string IdAttribute;
        protected string ObjectName;
        protected string FileLocked;

        public CheckInOutCommonAttributes(CRM CRMType)
        {
            switch (CRMType)
            {
                case CRM.Salesforce:
                    FileLockedDataType = Datatype.String;
                    IdAttribute = Constants.ID_ATTRIBUTE;
                    ObjectName = Constants.NAMESPACE_PREFIX + Constants.APP_FILE;
                    FileLocked = Constants.NAMESPACE_PREFIX + Constants.APP_FILE_IsLocked__c;
                    break;
                case CRM.DynamicsCRM:
                    FileLockedDataType = Datatype.Boolean;
                    IdAttribute = "apttus_xapps_appfileid";
                    ObjectName = "apttus_xapps_appfile";
                    FileLocked = "apttus_xapps_islocked";
                    break;
                default: break;
            }
        }
    }
}
