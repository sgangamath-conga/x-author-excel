using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.OAuthLoginControl.Modules {

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class ApttusUserLogin {

        public string LastUsedConnection { get; set; }
        public List<ApttusUserLoginInformation> UserLoginInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public ApttusUserLoginInformation GetUserLoginInformation(string loginName) {

            return UserLoginInfo.Where(ul => ul.LoginName.Equals(loginName)).FirstOrDefault();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal sealed class ApttusUserLoginInformation {

        public string LoginName { get; set; }
        public string LognHint { get; set; }
        public string OAuthToken { get; set; }
        public string ServerHost { get; set; }
    }
}
