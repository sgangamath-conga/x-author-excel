using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.Core
{
    class AICLookupIdAndNameProvider : ILookupIdAndLookupNameProvider
    {
        string ILookupIdAndLookupNameProvider.GetLookupIdFromLookupName(string LookupName)
        {
            string result = string.Empty;

            if (LookupName.EndsWith(Constants.APPENDLOOKUPID))
            {
                string ReferenceObject = LookupName.Substring(0, LookupName.IndexOf(Constants.APPENDLOOKUPID));
                result = GetLookupReference__C(ReferenceObject);
            }

            return result;
        }

        string ILookupIdAndLookupNameProvider.GetLookupNameFromLookupId(string LookupId)
        {
            string result = GetLookupReference__R(LookupId);
            if (!string.IsNullOrEmpty(result))
                result += Constants.APPENDLOOKUPID;
            return result;
        }

        bool ILookupIdAndLookupNameProvider.IsLookupField(ApttusField field)
        {
            return ((!field.Id.Equals(Constants.ID_ATTRIBUTE)) && // Exclude Id field as its not a lookup
                             (
                             field.Id.EndsWith(Constants.APPENDLOOKUPID) || // Include ending with ".Name"
                             (field.Datatype == Datatype.Lookup) // Include ending with "__c" and is Lookup datatype
                             ));
        }

        bool ILookupIdAndLookupNameProvider.IsLookupIdField(ApttusObject obj, ApttusField apttusField)
        {
            return apttusField.Datatype == Datatype.Lookup;
        }
        public string GetLookupReference__R(string LookupId__C)
        {
            return LookupId__C;            
        }
        public string GetLookupReference__C(string LookupId__R)
        {
            return LookupId__R;
        }
    }
}
