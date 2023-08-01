/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class ConnectionEntry : UserControl
    {
        private SwitchConnectionActionModel connection;
        private SwitchConnectionsLoginView view;
        string sErrorMessage = string.Empty;

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ConnectionManager connectionManager = ConnectionManager.GetInstance;

        public ConnectionEntry(SwitchConnectionActionModel connection, SwitchConnectionsLoginView view)
        {
            InitializeComponent();
            SetCultureData();

            this.connection = connection;
            this.view = view;
        }
        private void SetCultureData()
        {
            btnConnect.Text = resourceManager.GetResource("CONNENTRY_btnConnect_Text");
            label1.Text = resourceManager.GetResource("CONNENTRY_label1_Text");
            lblConnectStatus.Text = resourceManager.GetResource("CONNENTRY_lblConnectStatus_Text");
        }
        private void ConnectionEntry_Load(object sender, EventArgs e)
        {
            txtName.Text = connection.SwitchToConnectionName;
        }

        private void lnklblProduction_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtLoginURL.Text = lnklblProduction.Text;
        }

        private void lnklblSandbox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtLoginURL.Text = lnklblSandbox.Text;
        }

        private void txtLoginURL_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoginURL.Text))
                txtLoginURL.Text = "https://";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(ValidateConnection().ToString() + " : " + this.connection.SwitchToConnectionName);
            try
            {
                if (ValidateConnection())
                {
                    ConnectionInstance newConnection = new ConnectionInstance();
                    connectionManager.InitializeOAuthSession(newConnection, txtLoginURL.Text.Trim());

                    newConnection.OAuthSession.UserClosedForm = false;
                    newConnection.OAuthSession.token = null;


                    // Modified: changing in Pattern of generating SwitchLoginID, removing LoginURL from it. 
                    newConnection.OAuthSession.SwitchLoginId = this.connection.SwitchToConnectionName;

                    // Added : Its about to make sure that before calling switch connection make sure that requested connection exists & if its not then create it   
                    CreateConnectionifnotExists(this.connection.SwitchToConnectionName, txtLoginURL.Text.Trim());

                    newConnection.OAuthSession.SwitchLogin();

                    // Close login form after post login.
                    newConnection.OAuthSession.CloseLoginForm();
                    newConnection.OAuthSession.UserClosedForm = false;

                    if (newConnection.OAuthSession.token != null)
                    {
                        newConnection.SwitchConnectionActionId = this.connection.Id;
                        connectionManager.Add(newConnection);
                        lblConnectStatus.Text = resourceManager.GetResource("CONNENTRY_btnConnect_Click_Text");

                        if (connectionManager.connectionInstances.Count == view.controller.connections.Count)
                            view.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        ///  Creates new connection if its not exists
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="serverHost"></param>
        void CreateConnectionifnotExists(string connectionName, string serverHost)
        {
            if (!CheckIfConnectionExists(connectionName))
                CreateNewConnection(connectionName, serverHost);

        }

        /// <summary>
        /// check if requested connection exists or not 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        bool CheckIfConnectionExists(string connectionName)
        {
            return Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.ContainsKey(connectionName);
        }

        /// <summary>
        /// Create new connection
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="serverHost"></param>
        void CreateNewConnection(string connectionName, string serverHost)
        {
            SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
            oAuthConnection.ConnectionName = connectionName;
            oAuthConnection.ServerHost = serverHost;
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Add(oAuthConnection.ConnectionName, oAuthConnection);
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();
        }

        /// <summary>
        /// Validates Connection information for a new connection
        /// </summary>
        /// <returns></returns>
        internal bool ValidateConnection()
        {
            bool bValidServerHost = ValidateServerHost();

            if (!bValidServerHost)
            {
                MessageBox.Show(sErrorMessage, Constants.RUNTIME_PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Validates server host URL
        /// </summary>
        /// <returns></returns>
        internal bool ValidateServerHost()
        {
            if (string.IsNullOrEmpty(txtLoginURL.Text))
            {

                sErrorMessage = resourceManager.GetResource("MANAGECONNECTION_ValidURL_ErrorMsg");
                errorProvider.SetError(txtLoginURL, sErrorMessage);
                return false;
            }
            else if (!ValidateURL())
            {
                sErrorMessage = resourceManager.GetResource("MANAGECONNECTION_QualifiedURL_ErrorMsg");
                errorProvider.SetError(txtLoginURL, sErrorMessage);
                return false;
            }
            else
            {
                errorProvider.SetError(txtLoginURL, "");
                return true;
            }
        }

        /// <summary>
        /// Validates URL is valid and starts with https
        /// </summary>
        /// <returns></returns>
        internal bool ValidateURL()
        {
            txtLoginURL.Text = txtLoginURL.Text.Trim();

            System.Text.RegularExpressions.Regex regExURL = new System.Text.RegularExpressions.Regex(@"^https\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$");
            return regExURL.IsMatch(txtLoginURL.Text.Trim());
        }

    }
}
