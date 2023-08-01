using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.OAuthLoginControl.Modules
{
    public static class ApttusGlobals
    {
        // Registry keys for XAuthor Login sub keys
        //public const string ApttusXAuthorRegistryBase = "Software\\Apttus\\ApttusXAuthorForChatter\\Logins";
        public const string ServerHostKey = "ServerHost";
        public const string OAuthTokenKey = "OAuthToken";
        public const string lastLoginHintKey = "LoginHint";

        /// <summary>
        /// 
        /// </summary>
        public enum LoginMode {

            Login = 0,
            SwitchLogin = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ManageConnectionResult {

            DoNothing = 0,
            SwitchLogin = 1
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Application { 

            AuthorForWord = 0,
            ChatterForWord = 1,
            ChatterForExcel = 2,
            ChatterForPowerpoint = 3,
            ChatterForOutlook = 4,
            CPQForExcel = 5
        }
    }
}
