/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime.View
{
    public partial class OptionsForm : Form
    {
        private bool isFormDirty = false;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public OptionsForm()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Apttus.XAuthor.Core.Constants.RUNTIME_PRODUCT_NAME;

            isFormDirty = false;
            EnableDisableButtons();
        }
        private void SetCultureData()
        {
            btnApply.Text = resourceManager.GetResource("OPTIONSFORM_btnApply_Text");
            btnDone.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            chkProxyAuthenticated.Text = resourceManager.GetResource("OPTIONSFORM_chkProxyAuthenticated_Text");
            chkSaveTokenLocally.Text = resourceManager.GetResource("OPTIONSFORM_chkSaveTokenLocally_Text");
            groupBox1.Text = resourceManager.GetResource("OPTIONSFORM_groupBox1_Text");
            groupBox3.Text = resourceManager.GetResource("OPTIONSFORM_groupBox3_Text");
            grpBoxChatter.Text = resourceManager.GetResource("OPTIONSFORM_grpBoxChatter_Text");
            label4.Text = resourceManager.GetResource("OPTIONSFORM_label4_Text");
            lblChatterDuration.Text = resourceManager.GetResource("OPTIONSFORM_lblChatterDuration_Text");
            lblProxyHost.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyHost_Text");
            lblProxyPassword.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyPassword_Text");
            lblProxyPort.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyPort_Text");
            lblProxyUserName.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyUserName_Text");
            rbManualProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbManualProxy_Text");
            rbNoProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbNoProxy_Text");
            rbSystemProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbSystemProxy_Text");
            tabChatter.Text = resourceManager.GetResource("OPTIONSFORM_tabChatter_Text");
            tabGeneral.Text = resourceManager.GetResource("OPTIONSFORM_tabGeneral_Text");

            grpSelectLanguage.Text = resourceManager.GetResource("OPTIONSFORM_grpLanguage_Text");
            lblLanguage.Text = resourceManager.GetResource("OPTIONSFORM_lblLanguage_Text");

        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadOptions()
        {
            string KeyVal = string.Empty;
            string RegistryBase = string.Empty;

            rbSystemProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.System);
            rbManualProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.Custom);
            rbNoProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.None);

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyHostNameKey);
            KeyVal = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyHostName;
            if (!string.IsNullOrEmpty(KeyVal)) txtProxyHost.Text = KeyVal;

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyPortKey);
            txtProxyPort.Value = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyPort;

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyIsAuthenticatedKey);
            chkProxyAuthenticated.Checked = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyAuthenticate;

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyUserNameKey);
            KeyVal = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyUserName;
            if (!string.IsNullOrEmpty(KeyVal)) txtProxyUserName.Text = KeyVal;

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ProxyPasswordKey);
            KeyVal = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyFurtiveCode;
            if (!string.IsNullOrEmpty(KeyVal)) txtProxyPassword.Text = Apttus.OAuthLoginControl.Helpers.StringCipher.decryptString(KeyVal);

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SaveTokenLocally);
            chkSaveTokenLocally.Checked = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.SaveTokenLocally;

            //KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.LogSettings);
            //if (KeyVal != "")
            //{
            //    if (KeyVal == Constants.LogOn)
            //        rbLogOn.Checked = true;
            //    else if (KeyVal == Constants.LogOff)
            //        rbLogOff.Checked = true;
            //}

            //For chatter refresh settings
            populateChatterDurationDropDown();
            LoadChatterDuration();
            LoadResourcePreferences();
        }

        private void LoadResourcePreferences()
        {
            try
            {
                if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference))
                    cmbLanguage.SelectedItem = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference];
                else
                    cmbLanguage.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// populateChatterDurationDropDown
        /// </summary>
        private void populateChatterDurationDropDown()
        {

            // Create a List to store our KeyValuePairs
            List<KeyValuePair<string, string>> chatterDuration = new List<KeyValuePair<string, string>>();

            // Add data to the List
            chatterDuration.Add(new KeyValuePair<string, string>("Manual", "0"));
            chatterDuration.Add(new KeyValuePair<string, string>("1 Minute", "60000"));
            chatterDuration.Add(new KeyValuePair<string, string>("5 Minutes", "300000"));
            chatterDuration.Add(new KeyValuePair<string, string>("15 Minutes", "900000"));
            chatterDuration.Add(new KeyValuePair<string, string>("30 Minutes", "1800000"));
            chatterDuration.Add(new KeyValuePair<string, string>("60 Minutes", "3600000"));

            // Clear the combobox
            cboChatterDuration.DataSource = null;
            cboChatterDuration.Items.Clear();

            // Bind the combobox
            cboChatterDuration.DataSource = new BindingSource(chatterDuration, null);
            cboChatterDuration.DisplayMember = "Key";
            cboChatterDuration.ValueMember = "Value";
        }



        /// <summary>
        /// 
        /// </summary>
        public void SaveOptions()
        {
            //save network tab options
            string sProxyType = string.Empty;
            string sLogSetting = string.Empty;

            if (rbNoProxy.Checked)
            {
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.None;
            }
            else if (rbSystemProxy.Checked)
            {
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.System;
            }
            else
            {
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.Custom;
            }

            //// Exception Log Settings
            //if (rbLogOn.Checked)
            //    sLogSetting = Constants.LogOn;
            //else
            //    sLogSetting = Constants.LogOff;

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyTypeKey, sProxyType);
            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyHostNameKey, txtProxyHost.Text);
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyHostName = txtProxyHost.Text;

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyPortKey, txtProxyPort.Value.ToString());
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyPort = txtProxyPort.Value;

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyIsAuthenticatedKey, chkProxyAuthenticated.Checked.ToString());
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyAuthenticate = chkProxyAuthenticated.Checked;

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyUserNameKey, txtProxyUserName.Text);
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyUserName = txtProxyUserName.Text;

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ProxyPasswordKey, Apttus.OAuthLoginControl.Helpers.StringCipher.encryptString(txtProxyPassword.Text));
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyFurtiveCode = Apttus.OAuthLoginControl.Helpers.StringCipher.encryptString(txtProxyPassword.Text);

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.SaveTokenLocally, chkSaveTokenLocally.Checked.ToString());
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.SaveTokenLocally = chkSaveTokenLocally.Checked;

            // Get the selected item in the combobox
            KeyValuePair<string, string> chatterPair = (KeyValuePair<string, string>)cboChatterDuration.SelectedItem;
            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ChatterRefresh, Convert.ToString(chatterPair.Value));
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ChatterRefreshDuration = Convert.ToInt32(chatterPair.Value);

            //ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.LogSettings, sLogSetting);
            Helpers.ProxyHelper.ResetProxy();

            string selectedLanguage = cmbLanguage.SelectedItem.ToString();

            //Display Alert of Closing Excel to get effect of Language Change.
            if ((Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference) && selectedLanguage != Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference]) || cmbLanguage.SelectedIndex != 0)
            {
                MessageBox.Show(resourceManager.GetResource("OPTIONSFORM_ExcelCloseMessage_Text"), resourceManager.GetResource("OPTIONSFORM_grpLanguage_Text"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            AddExtendedProperty(ApttusGlobals.ResourcePreference, selectedLanguage);

            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();
        }

        private void AddExtendedProperty(string key, string value)
        {
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(key))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[key] = value;
            else
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.Add(key, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            // Validate if new entry
            string sChosenUserName = string.Empty;

            SaveOptions();

            isFormDirty = false;
            EnableDisableButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ApttusGlobals.ProxyTypeEnum GetProxyType()
        {
            ApttusGlobals.ProxyTypeEnum ProxyType = ApttusGlobals.ProxyTypeEnum.SystemProxy;

            if (rbNoProxy.Checked)
            {
                ProxyType = ApttusGlobals.ProxyTypeEnum.NoProxy;
            }
            else if (rbSystemProxy.Checked)
            {
                ProxyType = ApttusGlobals.ProxyTypeEnum.SystemProxy;
            }
            else
            {
                ProxyType = ApttusGlobals.ProxyTypeEnum.ManualProxy;
            }

            return ProxyType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetProxyHostName()
        {
            return txtProxyHost.Text.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetProxyPort()
        {
            return Convert.ToInt32(txtProxyPort.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetIsProxyAuthenticated()
        {
            return chkProxyAuthenticated.Checked;
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetProxyUserName()
        {
            return txtProxyUserName.Text.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetProxyPassword()
        {
            return txtProxyPassword.Text.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetFormDirtyFlag()
        {
            isFormDirty = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetSaveTokenLocally()
        {
            return chkSaveTokenLocally.Checked;
        }

        private void EnableDisableButtons()
        {
            this.btnApply.Enabled = isFormDirty;
        }

        private void chkSaveTokenLocally_CheckedChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void chkProxyAuthenticated_CheckedChanged(object sender, EventArgs e)
        {
            lblProxyUserName.Enabled = chkProxyAuthenticated.Enabled && chkProxyAuthenticated.Checked;
            lblProxyPassword.Enabled = chkProxyAuthenticated.Enabled && chkProxyAuthenticated.Checked;

            txtProxyUserName.Enabled = chkProxyAuthenticated.Enabled && chkProxyAuthenticated.Checked;
            txtProxyPassword.Enabled = chkProxyAuthenticated.Enabled && chkProxyAuthenticated.Checked;

            isFormDirty = true;
            EnableDisableButtons();
        }

        private void rbNoProxy_CheckedChanged(object sender, EventArgs e)
        {
            lblProxyHost.Enabled = false;
            lblProxyPort.Enabled = false;
            lblProxyUserName.Enabled = false;
            lblProxyPassword.Enabled = false;

            txtProxyHost.Enabled = false;
            txtProxyPort.Enabled = false;
            chkProxyAuthenticated.Enabled = false;
            txtProxyUserName.Enabled = false;
            txtProxyPassword.Enabled = false;

            isFormDirty = true;
            EnableDisableButtons();
        }

        private void rbSystemProxy_CheckedChanged(object sender, EventArgs e)
        {
            lblProxyHost.Enabled = false;
            lblProxyPort.Enabled = false;
            lblProxyUserName.Enabled = false;
            lblProxyPassword.Enabled = false;

            txtProxyHost.Enabled = false;
            txtProxyPort.Enabled = false;
            chkProxyAuthenticated.Enabled = false;
            txtProxyUserName.Enabled = false;
            txtProxyPassword.Enabled = false;

            isFormDirty = true;
            EnableDisableButtons();
        }

        private void rbManualProxy_CheckedChanged(object sender, EventArgs e)
        {
            lblProxyHost.Enabled = true;
            lblProxyPort.Enabled = true;
            lblProxyUserName.Enabled = chkProxyAuthenticated.Checked;
            lblProxyPassword.Enabled = chkProxyAuthenticated.Checked;

            txtProxyHost.Enabled = true;
            txtProxyPort.Enabled = true;
            chkProxyAuthenticated.Enabled = true;
            txtProxyUserName.Enabled = chkProxyAuthenticated.Checked;
            txtProxyPassword.Enabled = chkProxyAuthenticated.Checked;

            isFormDirty = true;
            EnableDisableButtons();
        }

        private void txtProxyHost_TextChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void txtProxyPort_ValueChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void txtProxyUserName_TextChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void txtProxyPassword_TextChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void cboChatterDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void ApttusOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isFormDirty)
            {
                if (MessageBox.Show(resourceManager.GetResource("OPTIONS_Options_ShowMsg"), ApttusGlobals.APPLICATION_NAME + resourceManager.GetResource("OPTIONS_OptionsCap_ShowMsg"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Load Chatter Duration function when tab changed for chatter
        /// </summary>
        private void LoadChatterDuration()
        {
            //string ChatterKeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ChatterRefresh);
            string ChatterValue = Convert.ToString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ChatterRefreshDuration);

            //List<KeyValuePair<string, string>> chatterDurations = cboChatterDuration.DataSource as List<KeyValuePair<string, string>>;

            //If chatter value is nothing
            if (!string.IsNullOrEmpty(ChatterValue))
                cboChatterDuration.SelectedValue = ChatterValue;
            else
                cboChatterDuration.SelectedValue = 0;
        }
        private void OptionsForm_Load(object sender, EventArgs e)
        {
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();
            if (this.Size.Height > this.PreferredSize.Height)
                this.Size = new System.Drawing.Size(this.PreferredSize.Width, this.PreferredSize.Height - 40);
            else
                this.Size = new System.Drawing.Size(this.PreferredSize.Width, this.PreferredSize.Height + 10);
            LoadOptions();
            isFormDirty = false;
            EnableDisableButtons();
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }



    }
}
