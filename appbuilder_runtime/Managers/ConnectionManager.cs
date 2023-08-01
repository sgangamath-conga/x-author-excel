/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class ConnectionManager
    {
        private static ConnectionManager instance;
        private static object syncRoot = new Object();

        public List<ConnectionInstance> connectionInstances = new List<ConnectionInstance>();

        private ConnectionManager()
        {
        }

        public static ConnectionManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ConnectionManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionInstance"></param>
        public void Add(ConnectionInstance connectionInstance)
        {
            if (connectionInstances.Exists(c => c.SwitchConnectionActionId.Equals(connectionInstance.SwitchConnectionActionId)))
            {
                ConnectionInstance existingConnection = connectionInstances.Where(c => c.SwitchConnectionActionId.Equals(connectionInstance.SwitchConnectionActionId)).FirstOrDefault();
                connectionInstances.Remove(existingConnection);
            }
            connectionInstances.Add(connectionInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="switchConnectionActionId"></param>
        /// <returns></returns>
        public ConnectionInstance Get(string switchConnectionActionId)
        {
            return connectionInstances.Where(c => c.SwitchConnectionActionId.Equals(switchConnectionActionId)).FirstOrDefault();
        }

        public void InitializeOAuthSession(ConnectionInstance connection, string serverHostURL)
        {
            try
            {
                if (connection.OAuthSession == null)
                    connection.OAuthSession = new OAuthLoginControl.ApttusOAuth(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings);

                connection.OAuthSession.ApplicationInfo.SaveInRegistry = false;
                connection.OAuthSession.ApplicationInfo.ApplicationRegistryBase = ApttusGlobals.ApttusRegistryBase;
                connection.OAuthSession.ApplicationInfo.ApplicationName = Constants.RUNTIME_PRODUCT_NAME;
                connection.OAuthSession.ApplicationInfo.ApplicationType = Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForExcel;
                connection.OAuthSession.ApplicationInfo.ApplicationLogo = global::Apttus.XAuthor.AppRuntime.Properties.Resources.Xauthor_ExcelLogo;
                connection.OAuthSession.ApplicationInfo.ApplicationIcon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
                connection.OAuthSession.ApplicationInfo.ApplicationDescription = Constants.AppDefDescription;

                connection.OAuthSession.clientID = "3MVG9rFJvQRVOvk4Cdn_X9wzEjWMW71zXT.aFEXdKcyZ3LZgAfq6_BagQqoiZ4PpZK9xqWZb9pNP1F634cj8U";
                connection.OAuthSession.clientSecret = "2031902323682913540";
                connection.OAuthSession.redirectURL = "xauthorexcel:callback";

                connection.OAuthSession.ConnectionEndPoint = serverHostURL;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

    }
}
