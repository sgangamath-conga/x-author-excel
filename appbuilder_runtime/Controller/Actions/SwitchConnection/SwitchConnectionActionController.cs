/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class SwitchConnectionActionController
    {
        private SwitchConnectionActionModel Model;
        public ActionResult Result { get; private set; }

        ConnectionManager connectionManager = ConnectionManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();

        public SwitchConnectionActionController(SwitchConnectionActionModel model)
        {
            this.Model = model;
            Result = new ActionResult();
        }

        public ActionResult Execute()
        {
            try
            {
                Result.Status = ActionResultStatus.Pending_Execution;

                ConnectionInstance currentConnection = connectionManager.Get(Model.Id);
                if (currentConnection != null)
                {
                    commandBar.SwitchConnection(currentConnection.UserInfo.UserName);
                    //System.Net.IWebProxy proxyObject = Helpers.ProxyHelper.GetProxyObject(Globals.ThisAddIn.oAuthWrapper.ConnectionEndPoint);
                    //// perform Do post login steps
                    ////objectManager.Connect(currentConnection.OAuthSession.token.access_token, currentConnection.OAuthSession.token.instance_url, proxyObject);

                    //// get user info of current connection and assign it Global userinfo
                    //currentConnection.UserInfo = objectManager.getUserInfo();
                    //Globals.ThisAddIn.userInfo = currentConnection.UserInfo;

                    //// Assign username to command bar manager
                    //ApttusCommandBarManager.g_UserName = currentConnection.UserInfo.UserName;
                    //Constants.CURRENT_USER_ID = currentConnection.UserInfo.UserId;

                    //// Assign token to global this addin token
                    //Globals.ThisAddIn.oAuthWrapper = currentConnection.OAuthSession;

                    //// Notify change connection
                    //// TODO:: check why registry entries are being updated stop, on login page popup, provide connection to improve connecting as message.
                    //Globals.ThisAddIn.NotifyLogin(false);

                    Result.Status = ActionResultStatus.Success;
                }

                return Result;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Action Flow Execution Message : Save Attachment Action");
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }
        }
    }
}
