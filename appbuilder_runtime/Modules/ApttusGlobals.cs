/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public static class ApttusGlobals
    {
        // Declare Application Name
        public const string APPLICATION_NAME = "X-Author for Microsoft Excel";

        // Declare Registry Base for Apttus App Builder
        public const string ApttusRegistryBase = "Software\\Apttus\\ApttusAppBuilder\\Logins";
        public const string ServerHostKey = "ServerHost";

        public const string WSDLPATH = "/services/wsdl/class/";
        public const string FRONTDOORURLPREFIX = "/secur/frontdoor.jsp?sid=";
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

        // Is App Loaded
        public const string IsAppLoading = "IsAppLoading";

        // Document Properties
        public const string OBJECT_ID = "OBJECT_ID";

        // Excel file operation 
        public static object oMissing = System.Type.Missing;
        public static object oSaveFormat = Excel.XlFileFormat.xlWorkbookNormal;
        public static object oFalse = false;

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

        public const string ResourcePreference = "ResourcePreference";
    }
}
