using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.Core
{
    class SalesforceLookupIdAndNameProvider : ILookupIdAndLookupNameProvider
    {
        public string GetLookupIdFromLookupName(string LookupName)
        {
            string result = string.Empty;

            if (LookupName.EndsWith(Constants.APPENDLOOKUPID))
            {
                string ReferenceObject = LookupName.Substring(0, LookupName.IndexOf(Constants.APPENDLOOKUPID));
                result = GetLookupReference__C(ReferenceObject);
            }

            return result;
        }

        public string GetLookupNameFromLookupId(string LookupId)
        {
            string result = GetLookupReference__R(LookupId);
            if (!string.IsNullOrEmpty(result))
                result += Constants.APPENDLOOKUPID;
            return result;
        }

        public string GetLookupReference__R(string LookupId__C)
        {
            string result = string.Empty;

            if (LookupId__C.EndsWith(Constants.ID_ATTRIBUTE))
                // Standard Object reference fields. Always end in Id. 
                // Fields can be accessed by removing Id and appending any field id of the lookup object.
                result = LookupId__C.Substring(0, LookupId__C.Length - 2);
            else if (LookupId__C.EndsWith(Constants.CustomObjectReference__c))
                // Custom Object reference fields. Always end in __c.
                // Fields can be accessed by replacing __c with __r and appending any field id of the lookup object.
                result = LookupId__C.Substring(0, LookupId__C.Length - 3) + Constants.CustomObjectReference__r;

            return result;
        }

        public string GetLookupReference__C(string LookupId__R)
        {
            string result = string.Empty;

            if (LookupId__R.EndsWith(Constants.CustomObjectReference__r))
                // Custom Object reference fields. Replace __r with __c.
                result = LookupId__R.Substring(0, LookupId__R.Length - 3) + Constants.CustomObjectReference__c;
            else
                // Standard Object reference fields. Add Id at the end.
                result = LookupId__R + Constants.ID_ATTRIBUTE;

            return result;
        }


        public bool IsLookupField(ApttusField field)
        {
            return ((!field.Id.Equals(Constants.ID_ATTRIBUTE)) && // Exclude Id field as its not a lookup
                             (field.Id.EndsWith(Constants.ID_ATTRIBUTE) || // Include ending with "Id"
                             field.Id.EndsWith(Constants.APPENDLOOKUPID) || // Include ending with ".Name"
                             (field.Id.Contains(Constants.CustomObjectReference__c) && field.Datatype == Datatype.Lookup) // Include ending with "__c" and is Lookup datatype
                             ));
        }


        public bool IsLookupIdField(ApttusObject obj, ApttusField apttusField)
        {
            return apttusField.Id.EndsWith(obj.IdAttribute) || apttusField.Id.EndsWith(Constants.CustomObjectReference__c);
        }
    }
}
