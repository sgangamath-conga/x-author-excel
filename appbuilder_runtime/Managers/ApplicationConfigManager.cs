using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Microsoft.Xrm.Sdk;

namespace Apttus.XAuthor.AppRuntime
{
    public class ApplicationConfigManager
    {
        static ApplicationConfigManager instance = null;
        SettingsManager.ApplicationSettings applicationSettings = null;
        private CRMContextManager crmManager = CRMContextManager.Instance;

        /// <summary>
        /// Singleton
        /// </summary>
        private ApplicationConfigManager()
        {
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        /// <returns></returns>
        public static ApplicationConfigManager GetInstance()
        {

            if (instance == null)
            {

                instance = new ApplicationConfigManager();
            }

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public SettingsManager.ApplicationSettings ApplicationSettings
        {

            get
            {

                return applicationSettings;
            }
            set
            {
                applicationSettings = value;
            }
        }

        public void LoadApplicationSettings()
        {
            string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            applicationSettings = new SettingsManager.ApplicationSettings(crmManager.ApplicationConfigFile, currentVersion);

            // if the setting loaded from config file then exit out
            if (applicationSettings.LoadApplicationSettings())
                return;

            if (applicationSettings == null)
            {
                applicationSettings = new SettingsManager.ApplicationSettings(Constants.PRODUCT_NAME, currentVersion);
            }

            if (applicationSettings.AppExtendedProperties == null)
            {
                applicationSettings.AppExtendedProperties = new SettingsManager.SerializableDictionary<string, string>();
            }

            if (applicationSettings.AppLogin == null)
            {
                applicationSettings.AppLogin = new SettingsManager.OAuthLoginConnection();
            }

            if (applicationSettings.AppLogin.Connections == null)
            {
                applicationSettings.AppLogin.Connections = new SettingsManager.SerializableDictionary<string, SettingsManager.OAuthConnection>();
            }            

            // Migrate the settings from registry to the config file
            bool bApplicationSettingsMigratedFromRegistry = false;
            string KeyVal = string.Empty;
            string RegistryBase = Constants.ApttusRegistryBase;
            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SettingsMigratedKey); // Find whether if the settings are already migrated

            if (applicationSettings.AppName.Equals(Constants.PRODUCT_NAME_AIC) || applicationSettings.AppName.Equals(Constants.PRODUCT_NAME_DCRM))
            {
                // No need to do registry Migration for DCRM
                bApplicationSettingsMigratedFromRegistry = true;

                // TODO:: Duplicate code here , for EnableMenu, check again.                 
                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.IsAppLoading);
                AddExtendedProperty(ApttusGlobals.IsAppLoading, KeyVal);
            }

            if (KeyVal != "")
            {
                bApplicationSettingsMigratedFromRegistry = Convert.ToBoolean(KeyVal);
            }

            if (!bApplicationSettingsMigratedFromRegistry)
            {

                if (applicationSettings == null)
                {
                    applicationSettings = new SettingsManager.ApplicationSettings(Constants.PRODUCT_NAME, currentVersion);
                }

                if (applicationSettings.AppExtendedProperties == null)
                {
                    applicationSettings.AppExtendedProperties = new SettingsManager.SerializableDictionary<string, string>();
                }

                if (applicationSettings.AppLogin == null)
                {
                    applicationSettings.AppLogin = new SettingsManager.OAuthLoginConnection();
                }

                // Network tab options
                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyTypeKey);
                if (KeyVal != "")
                {
                    switch (KeyVal)
                    {
                        case "SystemProxy":
                            applicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.System;
                            break;
                        case "ManualProxy":
                            applicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.Custom;
                            break;
                        default:
                            applicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.System;
                            break;
                    }
                }
                else
                {
                    applicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.System;
                }

                //Designer Properties
                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyHostNameKey);
                if (KeyVal != "") applicationSettings.ProxyHostName = KeyVal;

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyPortKey);
                if (KeyVal != "") applicationSettings.ProxyPort = Convert.ToInt32(KeyVal);

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyIsAuthenticatedKey);
                if (KeyVal != "") applicationSettings.ProxyAuthenticate = Convert.ToBoolean(KeyVal);

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyUserNameKey);
                if (KeyVal != "") applicationSettings.ProxyUserName = KeyVal;

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyPasswordKey);
                if (KeyVal != "") applicationSettings.ProxyFurtiveCode = Apttus.OAuthLoginControl.Helpers.StringCipher.decryptString(KeyVal);

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SaveTokenLocally);
                if (KeyVal != "") applicationSettings.SaveTokenLocally = Convert.ToBoolean(KeyVal);

                // Get QuickAppSettings serialized class key value
                QuickAppSettings quickAppSettings = null;
                string QASettingsKeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.QuickAppSettings);
                if (!string.IsNullOrEmpty(QASettingsKeyVal))
                    quickAppSettings = ApttusXmlSerializerUtil.Deserialize<Apttus.XAuthor.Core.QuickAppSettings>(QASettingsKeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.ListAppPosition.StartRow) : QuickAppConstants.RepeatingGroupStartRow.ToString();
                AddExtendedProperty(ApttusGlobals.ListAppStartRow, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.ListAppPosition.StartCol) : QuickAppConstants.RepeatingGroupStartCol.ToString();
                AddExtendedProperty(ApttusGlobals.ListAppStartCol, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.ParentChildAppPosition.StartRow) : QuickAppConstants.IndependentFieldStartRow.ToString();
                AddExtendedProperty(ApttusGlobals.ParentChildStartRow, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.ParentChildAppPosition.StartCol) : QuickAppConstants.IndependentFieldStartColumn.ToString();
                AddExtendedProperty(ApttusGlobals.ParentChildStartCol, KeyVal);

                System.Drawing.Color defaultDisplayFillColor = System.Drawing.Color.FromArgb(0, 128, 192);
                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.AppFillColor) : defaultDisplayFillColor.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.DisplayFieldsColor, KeyVal);

                System.Drawing.Color defaultSaveFillColor = System.Drawing.Color.FromArgb(146, 208, 80);
                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.AppFillColor) : defaultSaveFillColor.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.SaveFieldsColor, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.AppTitle.FormatAttribute.AppFontAndColor.AppFillColor) : System.Drawing.Color.White.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.WorksheetTitleColor, KeyVal);

                System.Drawing.Color whiteColor = System.Drawing.Color.White;
                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.AppTextColor) : whiteColor.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.DisplayFieldsTextColor, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.AppTextColor) : whiteColor.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.SaveFieldsTextColor, KeyVal);

                System.Drawing.Color blackColor = System.Drawing.Color.Black;
                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.AppTitle.FormatAttribute.AppFontAndColor.AppTextColor) : blackColor.ToArgb().ToString();
                AddExtendedProperty(ApttusGlobals.WorksheetTitleTextColor, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.FontStr) : "Calibri, 11.25pt";
                AddExtendedProperty(ApttusGlobals.DisplayFieldsFont, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.FontStr) : "Calibri, 11.25pt";
                AddExtendedProperty(ApttusGlobals.SaveFieldsFont, KeyVal);

                KeyVal = quickAppSettings != null ? Convert.ToString(quickAppSettings.AppTitle.FormatAttribute.AppFontAndColor.FontStr) : "Calibri, 15.75pt, style=Bold";
                AddExtendedProperty(ApttusGlobals.WorksheetTitleFont, KeyVal);

                // oAuth Connections
                applicationSettings.AppLogin.Name = Constants.PRODUCT_NAME;
                applicationSettings.AppLogin.LastUsedConnection = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForExcel.ToString());
                applicationSettings.AppLogin.Connections = new SettingsManager.SerializableDictionary<string, SettingsManager.OAuthConnection>();

                string[] userNames = ApttusRegistryManager.GetAvailableUserNames(RegistryHive.CurrentUser, RegistryBase);

                if (userNames != null && userNames.Length > 0)
                {
                    for (int i = 0; i < userNames.Length; i++)
                    {
                        SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
                        oAuthConnection.ConnectionName = userNames[i];
                        oAuthConnection.ServerHost = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase + "\\" + oAuthConnection.ConnectionName, Apttus.OAuthLoginControl.Modules.ApttusGlobals.ServerHostKey);
                        oAuthConnection.LastLoginHint = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase + "\\" + oAuthConnection.ConnectionName, Apttus.OAuthLoginControl.Modules.ApttusGlobals.lastLoginHintKey);
                        oAuthConnection.OAuthToken = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase + "\\" + oAuthConnection.ConnectionName, Apttus.OAuthLoginControl.Modules.ApttusGlobals.OAuthTokenKey);

                        applicationSettings.AppLogin.Connections.Add(oAuthConnection.ConnectionName, oAuthConnection);
                    }
                }

                // Chatter tab options
                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ChatterRefresh);
                AddExtendedProperty(ApttusGlobals.ChatterRefresh, KeyVal);

                KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.IsAppLoading);
                AddExtendedProperty(ApttusGlobals.IsAppLoading, KeyVal);

                applicationSettings.Save();

                if (userNames != null && userNames.Length > 0)
                {
                    ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SettingsMigratedKey, "TRUE");
                }
            }
        }

        private void AddExtendedProperty(string key, string value)
        {
            if (applicationSettings.AppExtendedProperties.ContainsKey(key))
                applicationSettings.AppExtendedProperties[key] = value;
            else
                applicationSettings.AppExtendedProperties.Add(key, value);
        }

    }
}
