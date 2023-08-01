using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Apttus.OAuthLoginControl.Modules;

namespace Apttus.OAuthLoginControl.Forms
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ApttusManageConnections : Form
    {

        ApttusOAuth OAuthModule = null;
        string sErrorMessage = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOAuth"></param>
        public ApttusManageConnections(ApttusOAuth objOAuth)
        {

            InitializeComponent();


            this.OAuthModule = objOAuth;
            this.Icon = objOAuth.ApplicationInfo.ApplicationIcon == null ? Icon.FromHandle(((Bitmap)objOAuth.ApplicationInfo.ApplicationLogo).GetHicon()) :
                            objOAuth.ApplicationInfo.ApplicationIcon;

            // For internalization support
            SetCultureData();

        }

        /// <summary>
        /// Set Culture Data
        /// </summary>
        private void SetCultureData()
        {
            if (this.OAuthModule.ApplicationInfo.UseResources)
            {
                groupBox1.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_lblAvailableConnections");
                this.Text = OAuthModule.ApplicationInfo.ApplicationName + " - " + this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_Title");
                CueProvider.SetCue(this.txtName, this.OAuthModule.ApplicationInfo.ResourceManager.GetString("COMMON_Name"));
                CueProvider.SetCue(this.txtLoginURL, this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_LoginURL_Text"));
                groupBox2.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_btn_CreateNew");
                btnAuthorize.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_btn_Authorize");
                label1.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("COMMON_Examples");
                btnCloseWindow.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("COMMON_btnClose_Text");
            }
            else
            {
                CueProvider.SetCue(this.txtName, "Name");
                CueProvider.SetCue(this.txtLoginURL, "Login URL");
                this.Text = OAuthModule.ApplicationInfo.ApplicationName + " - Manage Connections";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseWindow_Click(object sender, EventArgs e)
        {

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
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
        /// Populate Login info using registry key values
        /// </summary>
        private void PopulateConnectionInfo()
        {

            // Get the last login value
            string sLastLogin = OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection;

            int i = 0;
            System.Windows.Forms.Padding padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            System.Drawing.Size btnSize = new System.Drawing.Size(85, 25);

            foreach (string sConnectionName in OAuthModule.ApplicationSettings.AppLogin.Connections.Keys)
            {
                string sHost = OAuthModule.ApplicationSettings.AppLogin.Connections[sConnectionName].ServerHost;
               // CheckWidestRowIndex(sConnectionName, sHost, i);
                // Create Username label
                Label lblUserName = new Label();
                lblUserName.Text = sConnectionName;
                lblUserName.Dock = DockStyle.Fill;
                lblUserName.Margin = padding;
                lblUserName.AutoEllipsis = true;

                // Create Connection info label
                Label lblConnectionInfo = new Label();
                lblConnectionInfo.Text = sHost;
                lblConnectionInfo.Dock = DockStyle.Fill;
                lblConnectionInfo.Margin = padding;
                lblConnectionInfo.AutoEllipsis = true;

                // Create HyperLink for User Info label to enable Connect functionality from Manage Connections
                Button btnConnect = new Button();
                btnConnect.Size = btnSize;
                btnConnect.Click += new EventHandler(btnConnect_Click);
                btnConnect.Name = lblUserName.Text + "_" + i.ToString();
                btnConnect.FlatStyle = FlatStyle.Flat;

                if (OAuthModule.ApplicationInfo.UseResources)
                    btnConnect.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("CONNENTRY_btnConnect_Text");
                else
                    btnConnect.Text = "Connect";

                btnConnect.Enabled = !OAuthModule.IsLoggedIn ? true : !(sConnectionName.Equals(sLastLogin));
                btnConnect.Tag = sHost;

                // Create Connect button
                Button btnRevoke = new Button();
                btnRevoke.Size = btnSize;
                btnRevoke.Click += new EventHandler(btnRevoke_Click);
                btnRevoke.Name = sConnectionName;
                btnRevoke.FlatStyle = FlatStyle.Flat;
                if (OAuthModule.ApplicationInfo.UseResources)
                    btnRevoke.Text = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_btn_revoke");
                else
                    btnRevoke.Text = "Revoke";
                btnRevoke.Enabled = !OAuthModule.IsLoggedIn ? true : !(sConnectionName.Equals(sLastLogin));
                btnRevoke.Tag = sHost;

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

                // Add updated info as retrieved from Registry
                tblConnections.Controls.Add(lblUserName, 0, i);
                tblConnections.Controls.Add(lblConnectionInfo, 1, i);
                tblConnections.Controls.Add(btnConnect, 2, i);
                tblConnections.Controls.Add(btnRevoke, 3, i);

                tblConnections.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                i++;
            }
            // Remove last item, in case of delete
            if (tblConnections.RowStyles.Count > OAuthModule.ApplicationSettings.AppLogin.Connections.Count)
            {

                tblConnections.RowStyles.RemoveAt(OAuthModule.ApplicationSettings.AppLogin.Connections.Count);

                for (int columnIndex = 0; columnIndex < tblConnections.ColumnCount; columnIndex++)
                {

                    var control = tblConnections.GetControlFromPosition(columnIndex, OAuthModule.ApplicationSettings.AppLogin.Connections.Count);
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
        void btnConnect_Click(object sender, EventArgs e)
        {

            string userName = ((Button)sender).Name;

            if (!string.IsNullOrEmpty(userName))
            {

                userName = userName.Substring(0, userName.LastIndexOf("_"));
                OAuthModule.LastLoginHint = OAuthModule.ApplicationSettings.AppLogin.Connections[userName].LastLoginHint;
                OAuthModule.SwitchLoginId = userName;
                OAuthModule.ConnectionEndPoint = ((Button)sender).Tag as string;

                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevoke_Click(object sender, EventArgs e)
        {

            string revokeUserAccess = string.Empty;
            string revoleUserAccessCap = string.Empty;

            if (OAuthModule.ApplicationInfo.UseResources)
            {
                revokeUserAccess = string.Format(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_RevokeVal_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName);
                revoleUserAccessCap = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_RevokeValCap_ErrorMsg");
            }
            else
            {
                revokeUserAccess = "If you revoke this user's access, this user must authenticate this installation of " + OAuthModule.ApplicationInfo.ApplicationName + " again before logging in. Would you like to Continue?";
                revoleUserAccessCap = " - Revoke Access";
            }

            if (MessageBox.Show(revokeUserAccess, OAuthModule.ApplicationInfo.ApplicationName + revoleUserAccessCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

                Button btn = (Button)sender;

                string loginInfo = btn.Name;

                // Verify if OAuth token value for last logged in user exists, if yes deserialize token value
                string OAuthToken = OAuthModule.ApplicationSettings.AppLogin.Connections[loginInfo].OAuthToken;

                if (!String.IsNullOrEmpty(OAuthToken))
                {

                    // Decrypt OAuth Token value
                    OAuthToken = Helpers.StringCipher.decryptString(OAuthToken);

                    System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                    TokenResponse token = ser.Deserialize<TokenResponse>(OAuthToken);

                    if (token != null && OAuthModule.RevokeAccess(btn.Tag as string, token.refresh_token))
                    {

                        if (OAuthModule.ApplicationInfo.UseResources)
                            MessageBox.Show(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_UserRevoke_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("User Access Revoked.", OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (OAuthModule.ApplicationInfo.UseResources)
                {
                    if (MessageBox.Show(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_RemoveList_ErrorMsg"), string.Format(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_RemoveListCap_ErrorMsg"), OAuthModule.ApplicationInfo.ApplicationName), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {

                        OAuthModule.ApplicationSettings.AppLogin.Connections.Remove(loginInfo);

                        if (OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection == loginInfo)
                            OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection = string.Empty;
                    }
                }
                else
                {
                    if (MessageBox.Show("Do you like to remove this connection from the list?", OAuthModule.ApplicationInfo.ApplicationName + " - Remove Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {

                        OAuthModule.ApplicationSettings.AppLogin.Connections.Remove(loginInfo);

                        if (OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection == loginInfo)
                            OAuthModule.ApplicationSettings.AppLogin.LastUsedConnection = string.Empty;
                    }
                }
                TableLayoutPanelCellPosition pos = tblConnections.GetPositionFromControl(btn);
                int ind = pos.Row * 4;
                for (int i = 0; i < 4; i++)
                {
                    tblConnections.Controls[ind].Dispose();
                }
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

                OAuthModule.SwitchLoginId = txtName.Text;
                OAuthModule.ConnectionEndPoint = txtLoginURL.Text;

                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.Close();
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

                MessageBox.Show(sErrorMessage, OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else if (!bValidServerHost)
            {

                MessageBox.Show(sErrorMessage, OAuthModule.ApplicationInfo.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                if (OAuthModule.ApplicationInfo.UseResources)
                    sErrorMessage = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_ConnectionName_ErrorMsg");
                else
                    sErrorMessage = "Please enter a connection name.";

                errorProvider.SetError(txtName, sErrorMessage);
                return false;
            }
            else if (!ValidateConnectionNameIsUnique())
            {

                if (OAuthModule.ApplicationInfo.UseResources)
                    sErrorMessage = string.Format(this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_ConnectionNameExists_ErrorMsg"), txtName.Text);
                else
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

            if (string.IsNullOrEmpty(txtLoginURL.Text))
            {

                if (OAuthModule.ApplicationInfo.UseResources)
                    sErrorMessage = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_ValidURL_ErrorMsg");
                else
                    sErrorMessage = "Please enter a valid URL.";

                errorProvider.SetError(txtLoginURL, sErrorMessage);
                return false;
            }
            else if (!ValidateURL())
            {

                if (OAuthModule.ApplicationInfo.UseResources)
                    sErrorMessage = this.OAuthModule.ApplicationInfo.ResourceManager.GetString("MANAGECONNECTION_QualifiedURL_ErrorMsg");
                else
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

            return !OAuthModule.ApplicationSettings.AppLogin.Connections.ContainsKey(txtName.Text);
        }

        /// <summary>
        /// Authorize given Login Id and URL by creating registry entry
        /// </summary>
        private void SaveOptions()
        {

            if (!OAuthModule.ApplicationSettings.AppLogin.Connections.ContainsKey(txtName.Text))
            {

                SettingsManager.OAuthConnection oAuthConnection = new SettingsManager.OAuthConnection();
                oAuthConnection.ConnectionName = txtName.Text;
                oAuthConnection.ServerHost = txtLoginURL.Text;

                OAuthModule.ApplicationSettings.AppLogin.Connections.Add(txtName.Text, oAuthConnection);
            }
            else
            {

                OAuthModule.ApplicationSettings.AppLogin.Connections[txtName.Text].OAuthToken = string.Empty;
                OAuthModule.ApplicationSettings.AppLogin.Connections[txtName.Text].ServerHost = txtLoginURL.Text;
            }

            OAuthModule.CurrentUser = txtName.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusManageConnections_FormClosing(object sender, FormClosingEventArgs e)
        {

            OAuthModule.UserClosedForm = true;
            OAuthModule.ApplicationSettings.Save();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnklblProduction_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            txtLoginURL.Text = lnklblProduction.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnklblSandbox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            txtLoginURL.Text = lnklblSandbox.Text;
        }
    }
}
