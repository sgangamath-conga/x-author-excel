using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Apttus.OAuthLoginControl.Modules {

    /// <summary>
    /// 
    /// </summary>
    public class ApttusUserSettingsBase : ApplicationSettingsBase {

        /// <summary>
        /// 
        /// </summary>
        private static ApttusUserSettingsBase _defaultInstace = (ApttusUserSettingsBase)ApplicationSettingsBase.Synchronized(new ApttusUserSettingsBase());

        /// <summary>
        /// 
        /// </summary>
        public static ApttusUserSettingsBase Default {

            get { return _defaultInstace; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userLogin"></param>
        public void AddUserLoginInformation(ApttusGlobals.Application application, ApttusUserLoginInformation userLoginInfo) {

            if (this.UserLogin == null || !this.UserLogin.ContainsKey(application)) {

                ApttusUserLogin _UserLogin = new ApttusUserLogin();
                _UserLogin.UserLoginInfo.Add(userLoginInfo);

                if (this.UserLogin == null) {

                    this.UserLogin = new Dictionary<ApttusGlobals.Application, ApttusUserLogin>();
                }

                this.UserLogin.Add(application, _UserLogin);
            }
            else {

                ApttusUserLogin _UserLogin = this.UserLogin[application];
                _UserLogin.UserLoginInfo.Add(userLoginInfo);
            }
        }

        /// <summary>
        /// Stores list of logins for each of the application
        /// </summary>
		[UserScopedSetting]
		[SettingsSerializeAs(SettingsSerializeAs.Binary)]
		[DefaultSettingValue("")]
		private Dictionary<ApttusGlobals.Application, ApttusUserLogin> UserLogin {

            get { return this["UserLogin"] as Dictionary<ApttusGlobals.Application, ApttusUserLogin>; }
            set { this["UserLogin"] = value; }
		}
    }
}
