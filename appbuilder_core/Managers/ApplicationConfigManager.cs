using System;
using Microsoft.Win32;


namespace Apttus.XAuthor.Core
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
            {
                LoadAppWizardDefaultSettings();
                if (!applicationSettings.AppVersion.Equals(currentVersion))
                {
                    applicationSettings.AppVersion = currentVersion;
                    ClearAutoOpenCountKeys();
                }
                applicationSettings.Save();
                return;
            }

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

                LoadAppWizardDefaultSettings();

                // oAuth Connections
                applicationSettings.AppLogin.Name = Constants.PRODUCT_NAME;
                applicationSettings.AppLogin.LastUsedConnection = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForExcel.ToString());
                applicationSettings.AppLogin.Connections = new SettingsManager.SerializableDictionary<string, SettingsManager.OAuthConnection>();

                string[] userNames = ApttusRegistryManager.GetAvailableUserNames(RegistryHive.CurrentUser, RegistryBase);

                if (userNames != null && userNames.Length > 0)
                {
                    for (int i = 0; i < userNames.Length; i++)
                    {
                        SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection
                        {
                            ConnectionName = userNames[i]
                        };
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

        private void LoadAppWizardDefaultSettings()
        {
            string KeyVal = string.Empty;
            #region Fill Color
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayLabelFillColor, ApttusGlobals.IndividualDisplayLabelFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayLabelFillColor, ApttusGlobals.ListDisplayLabelFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayDataFillColor, ApttusGlobals.IndividualDisplayDataFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayDataFillColor, ApttusGlobals.ListDisplayDataFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveLabelFillColor, ApttusGlobals.IndividualSaveLabelFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveLabelFillColor, ApttusGlobals.ListSaveLabelFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveDataFillColor, ApttusGlobals.IndividualSaveDataFillColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataFillColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveDataFillColor, ApttusGlobals.ListSaveDataFillColorValue.ToString());
            #endregion

            #region Text Color
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayLabelTextColor, ApttusGlobals.IndividualDisplayLabelTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayDataTextColor, ApttusGlobals.IndividualDisplayDataTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayLabelTextColor, ApttusGlobals.ListDisplayLabelTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayDataTextColor, ApttusGlobals.ListDisplayDataTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveLabelTextColor, ApttusGlobals.IndividualSaveLabelTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveDataTextColor, ApttusGlobals.IndividualSaveDataTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveLabelTextColor, ApttusGlobals.ListSaveLabelTextColorValue.ToString());
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataTextColor);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveDataTextColor, ApttusGlobals.ListSaveDataTextColorValue.ToString());
            #endregion

            #region Font
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayLabelFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayLabelFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualDisplayDataFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListDisplayDataFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveLabelFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveLabelFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.IndividualSaveDataFont, ApttusGlobals.DisplayFontValue);
            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataFont);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.ListSaveDataFont, ApttusGlobals.DisplayFontValue);
            #endregion

            KeyVal = applicationSettings.GetExtendedProperty(ApttusGlobals.DisplayFieldsMinWidth);
            if (string.IsNullOrEmpty(KeyVal))
                applicationSettings.AddExtendedProperty(ApttusGlobals.DisplayFieldsMinWidth, ApttusGlobals.DisplayFieldsMinWidthValue.ToString());
        }

        private void ClearAutoOpenCountKeys()
        {
            applicationSettings.RemoveExtendedProperty(ApttusGlobals.WhatsNewDesignerDialogAutoOpenCount);
            applicationSettings.RemoveExtendedProperty(ApttusGlobals.WhatsNewRuntimeDialogAutoOpenCount);
            applicationSettings.RemoveExtendedProperty(ApttusGlobals.WhatsNewPainlessMapOpenCount);
            applicationSettings.RemoveExtendedProperty(ApttusGlobals.WhatsNewWorkFlowOpenCount);
            applicationSettings.RemoveExtendedProperty(ApttusGlobals.WhatsNewCentralAdminOpenCount);
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
