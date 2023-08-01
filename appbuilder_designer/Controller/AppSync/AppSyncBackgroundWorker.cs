using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class AppSyncBackgroundWorker
    {
        DesignerAppSyncController appSynccontroller;
        WaitWindowView waitWindow;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public AppSyncBackgroundWorker(DesignerAppSyncController controller)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                appSynccontroller = controller;

                waitWindow = new WaitWindowView(resourceManager.GetResource("AppSyncBackgroundWorker_PleasewaitMsg"), false);

                BackgroundWorker worker = new BackgroundWorker();
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
                waitWindow.ShowDialog();
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            waitWindow.CloseWaitWindow();
            waitWindow = null;
            appSynccontroller.RefreshSummaryData();
            appSynccontroller.UpdateViewLabels();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            appSynccontroller.ValidateApp(waitWindow);
        }
    }
}
