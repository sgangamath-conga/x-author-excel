/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Data;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class CheckOutViewController
    {
        private CheckOutView view;
        ObjectManager objectManager = ObjectManager.GetInstance;
        private DataTable Model;
        public string selectedDocId { get; set; }
        public string selectedAppFileId { get; set; }
        public string selectedAppFileUserId { get; set; }
        public string selectedAppFileUserName { get; set; }
        public bool selectedAppFileLocked { get; set; }
        public XAuthorQuery Query { get; set; }

        public CheckOutViewController(CheckOutView view, XAuthorQuery Query)
        {
            this.view = view;
            this.Query = Query;
            this.view.SetController(this);
            this.view.ShowDialog();
        }

        public void Search()
        {
            ApttusDataSet appFiles = objectManager.QueryDataSet(Query);

            if (appFiles != null)
            {
                Model = appFiles.DataTable;
                view.BindAppsGrid(Model);
            }
        }

        public void ApplicationSelected(string docId, string appFileId, string appFileUserId, string appFileUserName, bool appFileLocked)
        {
            selectedDocId = docId;
            selectedAppFileId = appFileId;
            selectedAppFileUserId = appFileUserId;
            selectedAppFileUserName = appFileUserName;
            selectedAppFileLocked = appFileLocked;
            view.Dispose();
        }
    }
}
