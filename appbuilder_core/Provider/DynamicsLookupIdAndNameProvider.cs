using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.Core
{
    class DynamicsLookupIdAndNameProvider : ILookupIdAndLookupNameProvider
    {
        public string GetLookupIdFromLookupName(string LookupName)
        {
            string result = string.Empty;
            if (LookupName.EndsWith(Constants.APPENDLOOKUPID))
                result = LookupName.Substring(0, LookupName.IndexOf(Constants.APPENDLOOKUPID));
            return result;
        }

        public string GetLookupNameFromLookupId(string LookupId)
        {
            string result = LookupId;
            if (!string.IsNullOrEmpty(result))
                result += Constants.APPENDLOOKUPID;
            return result;
        }

        public string GetLookupReference__R(string LookupId__C)
        {
            return LookupId__C;
        }

        public string GetLookupReference__C(string LookupId__R)
        {
            string result = string.Empty;

            if (LookupId__R.EndsWith(Constants.CustomObjectReference__r))
                // Custom Object reference fields. Replace __r with __c.
                result = LookupId__R.Substring(0, LookupId__R.Length - 3) + Constants.CustomObjectReference__c;
            else
                // Standard Object reference fields. Add Id at the end.
                result = LookupId__R + Constants.ID_ATTRIBUTE.ToLower();
            return result;
        }


        public bool IsLookupField(ApttusField apttusField)
        {
            return apttusField.Datatype == Datatype.Lookup;
        }


        public bool IsLookupIdField(ApttusObject obj, ApttusField apttusField)
        {
            return apttusField.Datatype == Datatype.Lookup;
        }
    }
}
