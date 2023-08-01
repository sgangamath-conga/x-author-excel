using Newtonsoft.Json;
using System;

namespace Apttus.IntelligentCloud.LoginControl {

    /// <summary>
    /// 
    /// </summary>
    public partial class ApttusUserInfo  {

        private Guid _userID;
        private string _userName;
        private string _firstName;
        private string _lastName;
        private string _fullName;
        private string _primaryEmail;
        private Guid _organizationID;

        /// <summary>
        /// 
        /// </summary>
        public ApttusUserInfo() {

        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public Guid UserID {

            get { return _userID; }
            set { _userID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserName {

            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName {

            get { return _firstName; }
            set { _firstName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string LastName {

            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "userFullName")]
        public string FullName {

            get { return _fullName; }
            set { _fullName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "EmailAddress")]
        public string PrimaryEmail {

            get { return _primaryEmail; }
            set { _primaryEmail = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationID {

            get { return _organizationID; }
            set { _organizationID = value; }
        }
    }
}
