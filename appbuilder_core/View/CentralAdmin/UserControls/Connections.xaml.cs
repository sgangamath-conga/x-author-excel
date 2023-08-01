using Apttus.OAuthLoginControl;
using Apttus.OAuthLoginControl.Helpers;
using Apttus.SettingsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for Connections.xaml
    /// </summary>

    public partial class Connections : UserControl
    {

        private static Connections _instance;
        private ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static Connections Instance {
            get {
                if (_instance == null)
                    _instance = new Connections();

                return _instance;
            }
        }
        private Connections()
        {
            if (applicationConfigManager.ApplicationSettings == null) applicationConfigManager.LoadApplicationSettings();
            InitializeComponent();
            SetCultureInfo();
        }

        private void SetCultureInfo()
        {
            lblTitle.Content = resourceManager.GetResource("CAD_Connections");
            lblAvailableConn.Content = resourceManager.GetResource("MANAGECONNECTION_lblAvailableConnections");
            colConnectionName.Header = resourceManager.GetResource("CAD_Connections_ConnectionName");
            colConnectionHost.Header = resourceManager.GetResource("CAD_Connections_ConnectionHost");
            colDeleteBtn.Header = resourceManager.GetResource("CAD_Connections_DeleteBTN");
        }

        private void UCConnections_Unloaded(object sender, RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.Save();
            _instance = (null);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string revokeUserAccess = string.Format(ApttusResourceManager.GetInstance.GetResource("MANAGECONNECTION_RevokeVal_ErrorMsg"), Constants.PRODUCT_NAME);
            string revoleUserAccessCap = ApttusResourceManager.GetInstance.GetResource("MANAGECONNECTION_RevokeValCap_ErrorMsg");

            if (MessageBox.Show(revokeUserAccess, Constants.PRODUCT_NAME + revoleUserAccessCap, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OAuthConnection connection = btn.Tag as OAuthConnection;
                string loginInfo = connection.ConnectionName;
                // Verify if OAuth token value for last logged in user exists, if yes deserialize token value
                string OAuthToken = applicationConfigManager.ApplicationSettings.AppLogin.Connections[loginInfo].OAuthToken;

                if (!String.IsNullOrEmpty(OAuthToken))
                {
                    // Decrypt OAuth Token value
                    OAuthToken = StringCipher.decryptString(OAuthToken);

                    System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                    TokenResponse token = ser.Deserialize<TokenResponse>(OAuthToken);
                    ApttusOAuth oauth = new ApttusOAuth(applicationConfigManager.ApplicationSettings);
                    oauth.RevokeAccess(connection.ServerHost, token.refresh_token);
                }
                applicationConfigManager.ApplicationSettings.AppLogin.Connections.Remove(loginInfo);

                if (applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection == loginInfo)
                    applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection = string.Empty;

                LoadConnections();
            }

        }
        private void UCConnections_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConnections();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void LoadConnections()
        {
            dgConnectionList.ItemsSource = applicationConfigManager.ApplicationSettings.AppLogin.Connections.Values.ToList();
        }


        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ValidateAndSaveConnection(sender);
            }
        }
        private void ValidateAndSaveConnection(object sender)
        {
            try
            {
                if (sender is TextBox currentTextBox)
                {
                    OAuthConnection connection = currentTextBox.DataContext as OAuthConnection;
                    if (applicationConfigManager.ApplicationSettings.AppLogin.Connections.ContainsKey(connection.ConnectionName))
                    {
                        //string empty check
                        if (string.IsNullOrEmpty(currentTextBox.Text.Trim()))
                        {
                            MessageBox.Show("Connection name can not be empty", "Error", MessageBoxButton.OK);
                            currentTextBox.Text = connection.ConnectionName;
                        }
                        //Duplicate Check
                        else if (applicationConfigManager.ApplicationSettings.AppLogin.Connections.ContainsKey(currentTextBox.Text.Trim()))
                        {
                            MessageBox.Show(string.Format("Connection entry with the name '{0}' already exists", currentTextBox.Text.Trim()), "Error", MessageBoxButton.OK);
                            currentTextBox.Text = connection.ConnectionName;
                        }
                        else
                        {
                            applicationConfigManager.ApplicationSettings.AppLogin.Connections.Remove(connection.ConnectionName);
                            connection.ConnectionName = currentTextBox.Text.Trim();
                            applicationConfigManager.ApplicationSettings.AppLogin.Connections.Add(currentTextBox.Text.Trim(), connection);
                            MessageBox.Show(string.Format("Connection name '{0}' saved successfully", currentTextBox.Text.Trim()), "Info", MessageBoxButton.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void TextBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focusable = true;
                textBox.Focus();
                textBox.SelectionStart = textBox.Text.Length;// add some logic if length is 0
                textBox.SelectionLength = 0;
            }
        }

        private void TextBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBox currentTextBox)
            {
                OAuthConnection connection = currentTextBox.DataContext as OAuthConnection;
                currentTextBox.Text = connection.ConnectionName;
            }
        }
    }
}
