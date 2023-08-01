using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SettingsManager = Apttus.SettingsManager;

namespace Apttus.IntelligentCloudV2.Login
{
    internal partial class ApttusManageConnections : Form
    {
        string sErrorMessage = string.Empty;
        private SettingsManager.ApplicationSettings ApplicationSettings;
        private AICWebLoginV2 AicLogin;

        /// <summary>
        /// 
        /// </summary>
        public ApttusManageConnections()
        {
            InitializeComponent();

            //this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            //this.Text = Apttus.XAuthor.Core.Constants.DESIGNER_PRODUCT_NAME;
        }

        public ApttusManageConnections(AICWebLoginV2 aicLogin)
        {
            InitializeComponent();

            AicLogin = aicLogin;

            //this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            //this.Text = Apttus.XAuthor.Core.Constants.DESIGNER_PRODUCT_NAME;

            this.ApplicationSettings = aicLogin.ApplicationSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetCultureData()
        {
            CueProvider.SetCue(txtName, "Name");
            CueProvider.SetCue(txtLoginURL, "Login URL");
            Text = "Manage Connections";
            groupBox1.Text = "Available connections";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseWindow_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusManageConnections_Load(object sender, EventArgs e)
        {
            PopulateConnectionInfo();
        }

        /// <summary>
        /// Populate Login info
        /// </summary>
        private void PopulateConnectionInfo()
        {
            // Get the last login value
            string sLastLogin = ApplicationSettings.AppLogin.LastUsedConnection;

            int i = 0;

            if (ApplicationSettings.AppLogin.Connections == null)
                ApplicationSettings.AppLogin.Connections = new SettingsManager.SerializableDictionary<string, SettingsManager.OAuthConnection>();

            // Sort the connection name in asc order
            var connections = from connection in ApplicationSettings.AppLogin.Connections
                              orderby connection.Key ascending
                              select connection;

            foreach (KeyValuePair<string, SettingsManager.OAuthConnection> connection in connections)
            {
                string sConnectionName = connection.Key;
                string sHost = connection.Value.ServerHost;

                // Create Username label
                Label lblUserName = new Label();
                lblUserName.Text = sConnectionName;
                lblUserName.Dock = DockStyle.Fill;

                // Create Connection info label
                Label lblConnectionInfo = new Label();
                lblConnectionInfo.Text = sHost;
                lblConnectionInfo.Dock = DockStyle.Fill;

                // Create HyperLink for User Info label to enable Connect functionality from Manage Connections
                Button btnConnect = new Button();
                btnConnect.Size = new Size(88, 28);
                btnConnect.Click += new EventHandler(btnConnect_Click);
                btnConnect.Name = lblUserName.Text + "_" + i.ToString();
                btnConnect.Text = "Connect";
                btnConnect.TextAlign = ContentAlignment.MiddleCenter;
                btnConnect.Enabled = !AicLogin.IsLoggedIn ? true : !(sConnectionName.Equals(sLastLogin));
                btnConnect.Tag = sHost;

                // Delete a connection
                Button btnDelete = new Button();
                btnDelete.Image = Apttus.IntelligentCloudV2.Login.Resource.erase_16x16;
                btnDelete.Name = lblUserName.Text + "_" + i.ToString();
                btnDelete.ImageAlign = ContentAlignment.MiddleCenter;
                btnDelete.Click += new EventHandler(btnDelete_Click);

                // Remove / Clear existing row & column if any
                if (tblConnections.RowStyles.Count > i)
                {
                    tblConnections.RowStyles.RemoveAt(i);

                    for (int columnIndex = 0; columnIndex < tblConnections.ColumnCount; columnIndex++)
                    {
                        var control = tblConnections.GetControlFromPosition(columnIndex, i);
                        tblConnections.Controls.Remove(control);
                    }
                }

                // Add updated info
                tblConnections.Controls.Add(lblUserName, 0, i);
                tblConnections.Controls.Add(lblConnectionInfo, 1, i);
                tblConnections.Controls.Add(btnConnect, 2, i);
                tblConnections.Controls.Add(btnDelete, 3, i);

                tblConnections.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

                i++;
            }

            // Remove last item, in case of delete
            if (tblConnections.RowStyles.Count > ApplicationSettings.AppLogin.Connections.Count)
            {
                tblConnections.RowStyles.RemoveAt(ApplicationSettings.AppLogin.Connections.Count);

                for (int columnIndex = 0; columnIndex < tblConnections.ColumnCount; columnIndex++)
                {
                    var control = tblConnections.GetControlFromPosition(columnIndex, ApplicationSettings.AppLogin.Connections.Count);
                    tblConnections.Controls.Remove(control);
                }
            }

            // Hide horizontal scroll bar
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tblConnections.Padding = new Padding(0, 0, vertScrollWidth, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string userName = ((Button)sender).Name;

            if (!string.IsNullOrEmpty(userName))
            {
                userName = userName.Substring(0, userName.LastIndexOf("_"));
                
                if (MessageBox.Show("Are you sure to delete the connection " + userName + "?", Constants.PRODUCT_NAME,MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    ApplicationSettings.AppLogin.Connections.Remove(userName);

                    if (ApplicationSettings.AppLogin.LastUsedConnection == userName)
                        ApplicationSettings.AppLogin.LastUsedConnection = string.Empty;
                }

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConnect_Click(object sender, EventArgs e)
        {
            string userName = ((Button)sender).Name;

            if (!string.IsNullOrEmpty(userName))
            {
                userName = userName.Substring(0, userName.LastIndexOf("_"));
                AicLogin.ConnectionEndPoint = new Tuple<string, string>(userName, ((Button)sender).Tag as string);

                DialogResult = DialogResult.Yes;
                Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            if (ValidateConnection())
            {
                SaveOptions();

                PopulateConnectionInfo();

                AicLogin.ConnectionEndPoint = new Tuple<string, string>(txtName.Text, txtLoginURL.Text);

                DialogResult = DialogResult.Yes;
                Close();
            }
        }

        /// <summary>
        /// Validates Connection information for a new connection
        /// </summary>
        /// <returns></returns>
        private bool ValidateConnection()
        {
            bool bValidServerName = ValidateConnectionName();
            bool bValidServerHost = ValidateServerHost();

            if (!bValidServerName)
            {
                MessageBox.Show(sErrorMessage, Constants.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else if (!bValidServerHost)
            {
                MessageBox.Show(sErrorMessage, Constants.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Validates Connection name
        /// </summary>
        /// <returns></returns>
        private bool ValidateConnectionName()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                sErrorMessage = "Please enter a connection name.";
                errorProvider.SetError(txtName, sErrorMessage);
                return false;
            }
            else if (!ValidateConnectionNameIsUnique())
            {
                sErrorMessage = "Connection name '" + txtName.Text + "' already exists, please choose another connection name.";
                errorProvider.SetError(txtName, sErrorMessage);
                return false;
            }
            else
            {
                errorProvider.SetError(txtName, "");
                return true;
            }
        }

        /// <summary>
        /// Validates server host URL
        /// </summary>
        /// <returns></returns>
        private bool ValidateServerHost()
        {
            return true;
            if (string.IsNullOrEmpty(txtLoginURL.Text))
            {
                sErrorMessage = "Please enter valid URL.";
                errorProvider.SetError(txtLoginURL, sErrorMessage);
                return false;
            }
            else if (!ValidateURL())
            {
                sErrorMessage = "Please enter a valid fully qualified URL that starts with https://";
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
        private bool ValidateURL()
        {
            txtLoginURL.Text = txtLoginURL.Text.Trim();

            System.Text.RegularExpressions.Regex regExURL = new System.Text.RegularExpressions.Regex(@"^https\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$");
            return regExURL.IsMatch(txtLoginURL.Text.Trim());
        }

        /// <summary>
        /// Validates whether Connection name being entered is Unique or not, if not prompt user for the same
        /// </summary>
        /// <returns></returns>
        private bool ValidateConnectionNameIsUnique()
        {
            return !ApplicationSettings.AppLogin.Connections.ContainsKey(txtName.Text);
        }

        /// <summary>
        /// Authorize given Login Id and URL by creating registry entry
        /// </summary>
        private void SaveOptions()
        {
            if (!ApplicationSettings.AppLogin.Connections.ContainsKey(txtName.Text))
            {
                SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
                oAuthConnection.ConnectionName = txtName.Text;
                oAuthConnection.ServerHost = txtLoginURL.Text;

                ApplicationSettings.AppLogin.Connections.Add(txtName.Text, oAuthConnection);
            }
            else
            {
                ApplicationSettings.AppLogin.Connections[txtName.Text].OAuthToken = string.Empty;
                ApplicationSettings.AppLogin.Connections[txtName.Text].ServerHost = txtLoginURL.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusManageConnections_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationSettings.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLoginURL_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoginURL.Text))
            {
                txtLoginURL.Text = "https://";
            }
        }
    }
}
