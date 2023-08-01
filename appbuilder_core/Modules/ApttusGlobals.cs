/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.SettingsManager;

namespace Apttus.XAuthor.Core
{
    public static class ApttusGlobals
    {
        // Declare Registry Base for Apttus App Builder
        public const string ApttusRegistryBase = "Software\\Apttus\\ApttusAppBuilder\\Logins";
        public const string ServerHostKey = "ServerHost";

        // Proxy Type Enum
        public enum ProxyTypeEnum
        {
            NoProxy = 0,
            SystemProxy = 1,
            ManualProxy = 2
        }

        // Options form proxy type keys
        public const string ProxyTypeKey = "ProxyType";
        public const string ProxyHostNameKey = "ProxyHostName";
        public const string ProxyPortKey = "ProxyPort";
        public const string ProxyIsAuthenticatedKey = "ProxyAuthenticate";
        public const string ProxyUserNameKey = "ProxyUserName";
        public const string ProxyPasswordKey = "ProxyPassword";
        public const string SaveTokenLocally = "SaveTokenLocally";
        public const string ChatterRefresh = "ChatterRefresh";

        public const string LogSettings = "LogSettings";
        public const string ListAppStartRow = "ListAppStartRow";
        public const string ListAppStartCol = "ListAppStartCol";
        public const string ParentChildStartRow = "ParentChildStartRow";
        public const string ParentChildStartCol = "ParentChildStartCol";
        //public const string ListTitleColor = "ListTitleColor";
        public const string DisplayFieldsColor = "DisplayFieldsColor";
        public const string SaveFieldsColor = "SaveFieldsColor";
        public const string WorksheetTitleColor = "WorksheetColor";

        public const string DisplayFieldsTextColor = "DisplayFieldsTextColor";
        public const string SaveFieldsTextColor = "SaveFieldsTextColor";
        public const string WorksheetTitleTextColor = "WorksheetTitleTextColor";

        public const string DisplayFieldsFont = "DisplayFieldsFont";
        public const string SaveFieldsFont = "SaveFieldsFont";
        public const string WorksheetTitleFont = "WorksheetTitleFont";

        public const string QuickAppSettings = "QuickAppSettings";
        public const string SettingsMigratedKey = "SettingsMigrated";
        public const string IsAppLoading = "IsAppLoading";

        public const string SequentialActionFlowDesigner = "SequentialActionFlowDesigner";
        public const string HideFormulaRowDisplayMap = "HideFormulaRowDisplayMap";
        public const string DisplayFieldsMinWidth = "DisplayFieldsMinWidth";
        public const int DisplayFieldsMinWidthValue = 20;
        public const string IndividualDisplayLabelFillColor = "IndividualDisplayLabelFillColor";
        public const string IndividualDisplayLabelTextColor = "IndividualDisplayLabelTextColor";
        public const string IndividualDisplayLabelFont = "IndividualDisplayLabelFont";
        public const string IndividualDisplayDataFillColor = "IndividualDisplayDataFillColor";
        public const string IndividualDisplayDataTextColor = "IndividualDisplayDataTextColor";
        public const string IndividualDisplayDataFont = "IndividualDisplayDataFont";
        public const string IndividualSaveLabelFillColor = "IndividualSaveLabelFillColor";
        public const string IndividualSaveLabelTextColor = "IndividualSaveLabelTextColor";
        public const string IndividualSaveLabelFont = "IndividualSaveLabelFont";
        public const string IndividualSaveDataFillColor = "IndividualSaveDataFillColor";
        public const string IndividualSaveDataTextColor = "IndividualSaveDataTextColor";
        public const string IndividualSaveDataFont = "IndividualSaveDataFont";
        public const string ListDisplayLabelFillColor = "ListDisplayLabelFillColor";
        public const string ListDisplayLabelTextColor = "ListDisplayLabelTextColor";
        public const string ListDisplayLabelFont = "ListDisplayLabelFont";
        public const string ListDisplayDataFillColor = "ListDisplayDataFillColor";
        public const string ListDisplayDataTextColor = "ListDisplayDataTextColor";
        public const string ListDisplayDataFont = "ListDisplayDataFont";
        public const string ListSaveLabelFillColor = "ListSaveLabelFillColor";
        public const string ListSaveLabelTextColor = "ListSaveLabelTextColor";
        public const string ListSaveLabelFont = "ListSaveLabelFont";
        public const string ListSaveDataFillColor = "ListSaveDataFillColor";
        public const string ListSaveDataTextColor = "ListSaveDataTextColor";
        public const string ListSaveDataFont = "ListSaveDataFont";

        public const int IndividualDisplayLabelFillColorValue = -16756519;
        public const int ListDisplayLabelFillColorValue = -16756519;

        public const int IndividualDisplayDataFillColorValue = -12805889;
        public const int ListDisplayDataFillColorValue = -12805889;

        public const int ListSaveLabelFillColorValue = -14112955;
        public const int IndividualSaveLabelFillColorValue = -14112955;
        
        public const int ListSaveDataFillColorValue = -10102144;
        public const int IndividualSaveDataFillColorValue = -10102144;

        public const int IndividualDisplayLabelTextColorValue = -1;
        public const int ListDisplayLabelTextColorValue = -1;

        public const int IndividualDisplayDataTextColorValue = -16777216;
        public const int ListDisplayDataTextColorValue = -16777216;

        public const int IndividualSaveLabelTextColorValue = -1;
        public const int ListSaveLabelTextColorValue = -1;

        public const int IndividualSaveDataTextColorValue = -16777216;
        public const int ListSaveDataTextColorValue = -16777216;

        public const string DisplayFontValue = "Calibri, 11pt";

        //What's New properties
        public const string WhatsNewDesignerDialogAutoOpenCount = "WhatsNewDesignerDialogAutoOpenCount";
        public const string WhatsNewRuntimeDialogAutoOpenCount = "WhatsNewRuntimeDialogAutoOpenCount";
        public const string WhatsNewPainlessMapOpenCount = "WhatsNewPainlessMapOpenCount";
        public const string WhatsNewWorkFlowOpenCount = "WhatsNewWorkFlowOpenCount";
        public const string WhatsNewCentralAdminOpenCount = "WhatsNewCentralAdminOpenCount";
        public const string WhatsNewPainlessMap = "PainlessMap";
        public const string WhatsNewWorkFlow = "WorkFlow";
        public const string WhatsNewCentralAdmin = "CentralAdmin";

        // Document Properties
        public const string OBJECT_ID = "OBJECT_ID";

        // Excel file operation 
        public static object oMissing = System.Type.Missing;
        //public static object oSaveFormat = Excel.XlFileFormat.xlWorkbookNormal;
        public static object oFalse = false;

        public const string ResourcePreference = "ResourcePreference";

        //CentralAdmin Settings
        public const string LastLogZoomLevel = "LastLogZoomLevel";
        public const string CompleteLogZoomLevel = "CompleteLogZoomLevel";
        public const string FirstTimeLoadCentralAdmin = "FirstTimeLoadCentralAdmin";

        //HealthCheck Const
        public const string IsDiagnosticModeOn = "IsDiagnosticModeOn";
        public const string IsAddInEnableChecked = "IsAddInDisableChecked";
        public const string AddInEnableCheckInterval = "AddInEnableCheckInterval";
        public const string IsClearLogFileChecked = "IsClearLogFileChecked";
        public const string ClearLogFileInterval = "ClearLogFileInterval";

        //AutoUpdate Const
        public const string IsAutoUpdateOn = "IsAutoUpdateOn";
        public const string AutoUpdateInterval = "AutoUpdateInterval";
        public const string LastAutoUpdatedOn = "LastAutoUpdatedOn";
        public const string AutoUpdateTime = "AutoUpdateTime";
        public const string LastDownloadedOn = "LastDownloadedOn";
        public const string UpdateStatus = "UpdateStatus";
        public const string DownloadedVersion = "DownloadedVersion";
        public const string DownloadDirectoryPath = "DownloadDirectoryPath";

        public const string APPLICATION_NAME = "X-Author for Microsoft Excel";

        public const string WSDLPATH = "/services/wsdl/class/";
        public const string FRONTDOORURLPREFIX = "/secur/frontdoor.jsp?sid=";

        public const string ACTION_BROWSE_GROUPS = "BrowseGroups";
        public const string ACTION_BROWSE_PEOPLE = "BrowsePeople";
        public const string ACTION_BROWSE_SOBJECTS = "BrowseSObjects";
        public const string ACTION_BROWSE_TEMPLATES = "BrowseTemplates";
        public const string ACTION_CHATTER_PAGE = "ChatterPage";
        public const string ACTION_CHATTERFEED_PAGE = "ChatterHtmlFeed";
        public const string ACTION_CHATTERUSER_PAGE = "ChatterUserPage";

        //Attachment Object
        public const string TEMPLATE_ATTACHMENT = "Attachment";
        public const string ATTACHMENT_PARENT_ID = "ParentId";
        public const string ATTACHMENT_OWNER_ID = "OwnerId";
        public const string ATTACHMENT_BODY = "Body";
        public const string ATTACHMENT_ID = "AttachmentId";
        public const string ATTACHMENT_TYPE = "AttachmentType";
        public const string ATTACHMENT_READONLY = "AttachmentReadOnly";

        public static bool blnExecute = false;

        public const string IsDesignerVisible = "IsDesignerVisible";

        public static string GetExtendedProperty(this ApplicationSettings applicationSettings, string property)
        {
            string result;
            if (applicationSettings.AppExtendedProperties.TryGetValue(property, out result))
            {
                return result;
            }
            result = string.Empty;
            return result;
        }

        public static void RemoveExtendedProperty(this ApplicationSettings applicationSettings, string property)
        {
            if (applicationSettings.AppExtendedProperties.ContainsKey(property))
                applicationSettings.AppExtendedProperties.Remove(property);
        }

        public static void AddExtendedProperty(this ApplicationSettings applicationSettings, string property, string value)
        {
            if (!applicationSettings.AppExtendedProperties.ContainsKey(property))
                applicationSettings.AppExtendedProperties.Add(property, value);
            else applicationSettings.AppExtendedProperties[property] = value;
        }

        /// <summary>
        /// Get Private field using reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetFieldValue<T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }

        /// <summary>
        /// Set Private field value using reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetFieldValue<T>(this object obj, string name, T value)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            obj.GetType().GetField(name, bindingFlags).SetValue(obj, value);
        }
    }
}
