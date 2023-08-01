/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ApttusUserInfo : ApttusUserBase
    {
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public string ProfileId { get; set; }
        public string Locale { get; set; }
        public string Language { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }

    public class ApttusUserBase
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }

    public class ApttusUserProfile : ApttusUserBase
    {

        public override string ToString()
        {
            return UserName;
        }
        // for now justhave id and name. 
    }
}
