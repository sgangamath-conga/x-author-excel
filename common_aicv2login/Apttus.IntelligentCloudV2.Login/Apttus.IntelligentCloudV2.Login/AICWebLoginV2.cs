using System;

namespace Apttus.IntelligentCloudV2.Login
{
    public class AICWebLoginV2
    {
        internal Apttus.SettingsManager.ApplicationSettings ApplicationSettings;

        public Tuple<string, string> ConnectionEndPoint { get; set; }
        public System.Net.IWebProxy Proxy { get; set; }
        public string TokenResponseJson { get; set; }
        public AICWebLoginV2(SettingsManager.ApplicationSettings applicationSettings)
        {
            ApplicationSettings = applicationSettings;
        }
        public bool IsLoggedIn { get; private set; }

        public void Login()
        {
            try
            {
                var loginWindow = new LoginForm();
                loginWindow.Show();
                loginWindow.Login(ConnectionEndPoint.Item2);

                while (loginWindow.tokenResponseJson == null)
                {
                    System.Windows.Forms.Application.DoEvents();

                    if (loginWindow.userClosedForm)
                        break;
                }
                TokenResponseJson = loginWindow.tokenResponseJson;
            }
            catch (Exception ex)
            {
                if (!ex.InnerException.Message.Equals("User canceled authentication"))
                    throw ex;
            }
        }

        public ManageConnectionResult ShowManageConnections()
        {
            ManageConnectionResult result = ManageConnectionResult.DoNothing;
            ApttusManageConnections manageConnectionsForm = new ApttusManageConnections(this);
            if (manageConnectionsForm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                result = ManageConnectionResult.SwitchLogin;
            return result;
        }
    }

    public enum ManageConnectionResult
    {

        DoNothing = 0,
        SwitchLogin = 1
    }

    internal class Constants
    {
        internal const string PRODUCT_NAME = "X-Author for Excel";
    }
}
