/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Diagnostics;
using System.Linq;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                if (this.applicationConfigManager == null)
                {
                    Globals.ThisAddIn.applicationConfigManager = ApplicationConfigManager.GetInstance();
                    Globals.ThisAddIn.applicationConfigManager.LoadApplicationSettings();
                }
                //this.oAuthWrapper = new OAuthLoginControl.ApttusOAuth(this.applicationConfigManager.ApplicationSettings);

                commandBar = Modules.ApttusCommandBarManager.GetInstance();
                commandBar.InitializeOptionsForm();

                InitializeApplicationInfo();
                commandBar.EnableMenus();

                SubscribeEvents();
            }
            catch (Exception ex)
            {
                Apttus.XAuthor.Core.ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            UnsubscribeEvents();

            // Close Notification Icon from Notification Area Icons
            if (Globals.Ribbons.ApttusRibbon.apttusNotification != null)
            {
                Globals.Ribbons.ApttusRibbon.apttusNotification.Visible = false;
                Globals.Ribbons.ApttusRibbon.apttusNotification.Icon = null;
                Globals.Ribbons.ApttusRibbon.apttusNotification.Dispose();
                Globals.Ribbons.ApttusRibbon.apttusNotification = null;
            }

        }
        //Edition info can be stored in about UI
        // this value will be set in workbook activate event. 
        public string AppDesignerVersion
        {
            get;
            set;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

    }
}
