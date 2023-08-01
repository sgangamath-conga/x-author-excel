using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Apttus.OAuthLoginControl;

namespace Apttus.XAuthor.AppRuntime
{
    public interface IXAuthorRibbonLogin
    {
        void Login();
        ApttusUserInfo UserInfo { get; }
        void SwitchConnection(string userName);
        void ShowManageConnection();
        void DoLogout(bool bLogout);
        string GetApplicationURI(string appUniqueId);
        void ResetProxy();
        void ResetToken();
        TokenResponse StartXAuthorAppFromStartupParameters(string[] msg);
        string GetFrontDoorURL();
        bool OpenApp();

        event EventHandler OnConnected;
        event EventHandler OnLogout;
    }
}
