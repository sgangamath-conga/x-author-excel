/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Data;

namespace Apttus.XAuthor.Core
{

    public class ApplicationBrowserController
    {
        private ApplicationBrowserView view;
        ObjectManager objectManager = ObjectManager.GetInstance;
        private DataTable Model;
        public string selectedAppId { get; set; }
        public string selectedAppName { get; set; }
        public bool basicEdition { get; set; }
		
        public ApplicationBrowserController(ApplicationBrowserView view, bool basicEdition)
        {
            this.basicEdition = basicEdition;
            this.view = view;
            this.view.SetController(this);
            this.view.ShowDialog();
        }

        public void Search(string SearchName, bool MyApps)
        {
            ApttusUserInfo userInfo = objectManager.getUserInfo();
            ApttusDataSet apps = objectManager.GetAppDataSet(100, SearchName, MyApps ? userInfo.UserId : null, userInfo);
            if (apps != null)
            {
                Model = apps.DataTable;
                view.BindAppsGrid(Model);
            }
        }

        public void ApplicationSelected(string appId, string appName)
        {
            selectedAppId = appId;
            selectedAppName = appName;
            view.Dispose();
        }
    }
}

