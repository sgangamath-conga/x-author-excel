using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    /// <summary>
    /// This class contains properties/variables which are common to all CRM. Hence while declaring a variable, make sure it is going to be used across all CRMs.
    /// </summary>
    abstract public class XAuthorBaseCRMLogin : IXAuthorRibbonLogin
    {
        protected ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        protected ObjectManager objectManager = ObjectManager.GetInstance;
        protected ApttusUserInfo LoggedInUserInfo = null;
        protected ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public string ConnectionURL = string.Empty;
        protected XAuthorBaseCRMLogin()
        {

        }

        public virtual bool LoggedIn
        {
            get { return false; }
        }

        public virtual void Login()
        {
            
        }

        public virtual Core.ApttusUserInfo UserInfo
        {
            get { return LoggedInUserInfo; }
        }

        public virtual void SwitchConnection(string userName)
        {

        }
        
        protected virtual void NotifyLogin()
        {
            if (ApttusCommandBarManager.g_IsLoggedIn)
            {
                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("ADDINIMP_NotifyLoginConnected_ShowMsg"), string.Format(resourceManager.GetResource("ADDINIMP_NotifyLoginWelcome_ShowMsg"),UserInfo.UserFullName, ConnectionURL, UserInfo.UserName ), ToolTipIcon.Info);

                string sNofifyText = "X-Author Designer (" + UserInfo.UserName + ")";

                if (sNofifyText.Length >= 64)
                {
                    sNofifyText = sNofifyText.Substring(0, 59) + "...)";
                }

                Globals.Ribbons.ApttusRibbon.apttusNotification.Text = sNofifyText;

                // Get an Hicon for myBitmap. 
                Globals.Ribbons.ApttusRibbon.apttusNotification.Icon = Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;                

                // get runtime flag
                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                if (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true"))
                    ApttusCommandBarManager.g_IsAppOpen = false;

                if (OnConnected != null)
                {
                    EventArgs e = new EventArgs();
                    OnConnected(this, e);
                }

                //commandBar.EnableMenus();
            }
            else
            {
                ApttusObjectManager.ShowNormalCursor();
            }
        }

        public virtual void ShowManageConnection()
        {

        }

        public virtual string GetApplicationURI(string appUniqueId)
        {
            return string.Empty;
        }


        public virtual void ResetProxy()
        {
        }

        public virtual void DoLogout(bool showBalloonMessage)
        {
            if (showBalloonMessage)
                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("COMMON_Disconnected_Msg"), resourceManager.GetResource("COMMON_LogOutCAP_InfoMsg"), ToolTipIcon.Info);

            TaskPaneHelper.RemoveAllTaskPanes();

            Globals.Ribbons.ApttusRibbon.apttusNotification.Text = Apttus.XAuthor.Core.Constants.DESIGNER_PRODUCT_NAME;

            if (OnLogout != null)
            {
                EventArgs e = new EventArgs();
                OnLogout(this, e);
            }
        }

        public virtual void ResetToken()
        {

        }

        public event EventHandler OnConnected;
        public event EventHandler OnLogout;
    }

}
