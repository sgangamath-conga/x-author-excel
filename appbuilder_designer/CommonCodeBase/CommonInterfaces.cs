using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    internal interface IXAuthorRibbonLogin
    {
        bool LoggedIn { get; }
        void Login();
        ApttusUserInfo UserInfo { get; }
        void SwitchConnection(string userName);
        void ShowManageConnection();
        void DoLogout(bool bLogout);
        string GetApplicationURI(string appUniqueId);
        void ResetProxy();
        void ResetToken();
        event EventHandler OnConnected;
        event EventHandler OnLogout;
    }   
}
