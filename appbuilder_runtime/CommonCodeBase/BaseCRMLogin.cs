using System;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.OAuthLoginControl;

namespace Apttus.XAuthor.AppRuntime
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

        protected XAuthorBaseCRMLogin()
        {
        }

        public virtual void Login()
        {

        }

        public virtual ApttusUserInfo UserInfo
        {
            get { return LoggedInUserInfo; }
        }

        public virtual void SwitchConnection(string userName)
        {

        }

        protected virtual void NotifyLogin(bool saveLoginOptions = true)
        {
            Globals.ThisAddIn.userInfo = UserInfo;
            Globals.ThisAddIn.NotifyLogin(UserInfo.UserName);
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
                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("COMMON_Disconnected_Msg"), resourceManager.GetResource("COMMON_LogOutCAP_InfoMsg"), ToolTipIcon.Info);

            TaskPaneHelper.RemoveAllTaskPanes();

            Globals.ThisAddIn.apttusNotification.Text = Constants.PRODUCT_NAME;

            if (OnLogout != null)
            {
                EventArgs e = new EventArgs();
                OnLogout(this, e);
            }
            ApttusCommandBarManager.g_IsLoggedIn = false;
            Globals.ThisAddIn.RuntimeRibbon.RibbonUIXml.Invalidate();
        }

        public virtual void ResetToken()
        {

        }

        public virtual TokenResponse StartXAuthorAppFromStartupParameters(string[] msg)
        {
            ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
            // System.Windows.Forms.MessageBox.Show("debug ");
            commandBarManager.StartupParameters.Parse(msg);
            //ApttusCommandBarManager.GetInstance().StartupParameters.Parse(Environment.GetCommandLineArgs());
            if (commandBarManager.StartupParameters.SessionId != null)
            {
                TokenResponse Tok = new TokenResponse();
                Tok.access_token = commandBarManager.StartupParameters.SessionId;
                // "00Di0000000fyop!ARgAQJOEJp5UM0zk9vFiOD6S87oSuPbRWeJOo0slgkAilQVk411GzXqWkifrFipcCsOOpnz2ZVsiQIOyQIKq7w6Wvz4OG4tt";
                //ApttusCommandBarManager.GetInstance().StartupParameters.SessionId;
                //TODO : get the correct session id from SFDC UI instead of hardcoding 
                Tok.instance_url = commandBarManager.StartupParameters.URLInstance; //"https://na15.salesforce.com";
                // objectManager.Connect(Tok.access_token, Tok.instance_url);
                //Globals.ThisAddIn.oAuthWrapper.token = Tok;
                Globals.ThisAddIn.Application.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMaximized;
                DataManager.GetInstance.StartParameters = commandBarManager.StartupParameters;
                return Tok;
            }
            return null;
        }

        public virtual bool OpenApp()
        {
            try
            {
                ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();

                // TODO:: for dynamics, Edit in Excel Support
                if (commandBarManager.StartupParameters.AppId != null)
                {
                    ApplicationObject appObject = null;
                    Globals.ThisAddIn.Application.StatusBar = "Loading template....";
                    BaseApplicationController appController = Globals.ThisAddIn.GetApplicationController();
                    appObject = appController.LoadApplication(string.Empty, commandBarManager.StartupParameters.AppId);

                    if (appObject == null)
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("ADDINIMPL_UnableLoad_ErrorMsg"), resourceManager.GetResource("ADDINIMPL_UnableLoadCap_ErrorMsg"));
                        return true;
                    }

                    commandBarManager.OpenTemplate(appObject);

                    //This flag is necessary otherwise the edit group menu wont be activated
                    ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_RUNTIME_FILE, "true");
                    ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_DESIGNER_FILE);
                    Globals.ThisAddIn.Application.StatusBar = "Executing Action Flow ....";
                    commandBarManager.DoPostLoad(Globals.ThisAddIn.RuntimeRibbon, false);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual string GetFrontDoorURL()
        {
            return string.Empty;
        }

        public event EventHandler OnConnected;
        public event EventHandler OnLogout;
    }
}
