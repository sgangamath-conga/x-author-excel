/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    public class ApplicationManager
    {
        private static ApplicationManager instance;
        private static object syncRoot = new Object();

        public List<ApplicationInstance> appInstances = new List<ApplicationInstance>();

        private ApplicationManager()
        {
        }

        public static ApplicationManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        /// <returns></returns>
        public ApplicationInstance Get(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            return appInstances.Where(app => app.UniqueId.Equals(appUniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode)).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appInstance"></param>
        public void Add(ApplicationInstance appInstance, string appFileName, ApplicationMode appMode)
        {
            if (appInstances.Exists(app => app.UniqueId.Equals(appInstance.UniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode)))
            {
                ApplicationInstance existsApp = appInstances.Where(app => app.UniqueId.Equals(appInstance.UniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode)).FirstOrDefault();

                if (existsApp.UniqueId.Equals(appInstance.UniqueId) && existsApp.AppFileName.Equals(appFileName) && existsApp.AppMode.Equals(appMode))
                    appInstances.RemoveAll(app => app.UniqueId.Equals(appInstance.UniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode));
            }
            appInstances.Add(appInstance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        public void Remove(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            if (appInstances.Exists(app => app.UniqueId.Equals(appUniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode)))
            {
                ApplicationInstance appInstance = appInstances.Where(app => app.UniqueId.Equals(appUniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode)).FirstOrDefault();
                appInstances.RemoveAll(app => app.UniqueId.Equals(appInstance.UniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        /// <param name="appMode"></param>
        public bool Exists(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            return appInstances.Exists(app => app.UniqueId.Equals(appUniqueId) & app.AppFileName.Equals(appFileName) & app.AppMode.Equals(appMode));
        }

    }
}
