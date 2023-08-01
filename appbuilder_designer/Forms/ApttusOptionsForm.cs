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

using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class ApttusOptionsForm : Form
    {
        private bool isFormDirty = false;
        private bool IsBasicEdition;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ApttusOptionsForm(bool bIsBasicEdition = false)
        {
            IsBasicEdition = bIsBasicEdition;
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Apttus.XAuthor.Core.Constants.DESIGNER_PRODUCT_NAME;

            LoadOptions();
            isFormDirty = false;
            EnableDisableButtons();
        }

        private void SetCultureData()
        {
            btnApply.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnDone.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            chkProxyAuthenticated.Text = resourceManager.GetResource("OPTIONSFORM_chkProxyAuthenticated_Text");
            chkSaveTokenLocally.Text = resourceManager.GetResource("OPTIONSFORM_chkSaveTokenLocally_Text");
            groupBox1.Text = resourceManager.GetResource("OPTIONSFORM_groupBox1_Text");
            groupBox3.Text = resourceManager.GetResource("OPTIONSFORM_groupBox3_Text");
            grpAppSettings.Text = resourceManager.GetResource("OPTIONSFORM_grpAppSettings_Text");
            grpBoxHeader.Text = resourceManager.GetResource("COMMON_Header_Text");
            
            label1.Text = resourceManager.GetResource("COMMON_DisplayFields_Text");
            label2.Text = resourceManager.GetResource("COMMON_SaveFields_Text");
            label3.Text = resourceManager.GetResource("OPTIONSFORM_label3_Text");
            // label4.Text = resourceManager.GetResource("OPTIONSFORM_label4_Text");
            label5.Text = resourceManager.GetResource("OPTIONSFORM_label5_Text");
            label6.Text = resourceManager.GetResource("OPTIONSFORM_label6_Text");
            lblProxyHost.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyHost_Text");
            lblProxyPassword.Text = resourceManager.GetResource("COMMON_Password_Text") + ":";
            lblProxyPort.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyPort_Text");
            lblProxyUserName.Text = resourceManager.GetResource("OPTIONSFORM_lblProxyUserName_Text");
            lblTitleColor.Text = resourceManager.GetResource("COMMON_WorkSheetTitle_Text");
            rbManualProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbManualProxy_Text");
            rbNoProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbNoProxy_Text");
            rbSystemProxy.Text = resourceManager.GetResource("OPTIONSFORM_rbSystemProxy_Text");
            tabGeneral.Text = resourceManager.GetResource("COMMON_General_Text");
            tabQuickApp.Text = resourceManager.GetResource("COMMON_QuickApp_Text");
            txtFontTitle.Text = resourceManager.GetResource("OPTIONSFORM_txtFont_Text");
            txtFontDisplay.Text = resourceManager.GetResource("COMMON_Calibri11_Text");
            txtFontSave.Text = resourceManager.GetResource("COMMON_Calibri11_Text");

            grpSelectLanguage.Text = resourceManager.GetResource("OPTIONSFORM_grpLanguage_Text");
            lblLanguage.Text = resourceManager.GetResource("OPTIONSFORM_lblLanguage_Text");
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadOptions()
        {
            rbSystemProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.System);
            rbManualProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.Custom);
            rbNoProxy.Checked = (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.None);

            if (!String.IsNullOrEmpty(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyHostName))
                txtProxyHost.Text = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyHostName;

            txtProxyPort.Value = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyPort;

            chkProxyAuthenticated.Checked = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyAuthenticate;

            if (!String.IsNullOrEmpty(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyUserName))
                txtProxyUserName.Text = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyUserName;

            if (!String.IsNullOrEmpty(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyFurtiveCode))
                txtProxyPassword.Text = Apttus.OAuthLoginControl.Helpers.StringCipher.decryptString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyFurtiveCode);

            chkSaveTokenLocally.Checked = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.SaveTokenLocally;

            // Quick App settings

            // Display only fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsColor))
                txtDisplayFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsColor]));
            // Save fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsColor))
                txtSaveFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsColor]));
            // Worksheet fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleColor))
                txtTitleColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleColor]));

            // Display only fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsTextColor))
                txtDisplayFieldsTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsTextColor]));
            // Save fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsTextColor))
                txtSaveFieldsTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsTextColor]));
            // Worksheet text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleTextColor))
                txtTitleTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleTextColor]));

            // Display only fields fore color
            string strFont = string.Empty;
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsFont))
            {
                strFont = Convert.ToString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsFont]);
                txtFontDisplay.Text = strFont;
                txtFontDisplay.Tag = Utils.ConvertFontFromString(strFont);
            }
            // Save fields fore color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsFont))
            {
                strFont = Convert.ToString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsFont]);
                txtFontSave.Text = strFont;
                txtFontSave.Tag = Utils.ConvertFontFromString(strFont);
            }
            // Worksheet fore color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleFont))
            {
                strFont = Convert.ToString(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleFont]);
                txtFontTitle.Text = strFont;
                txtFontTitle.Tag = Utils.ConvertFontFromString(strFont);
            }

            LoadResourcePreferences();

            #region "old code"

            /*
            try
            {

                UserQAPSettings = ApttusXmlSerializerUtil.Deserialize<Apttus.XAuthor.Core.Model.AppSettings.QuickAppSettings>(KeyVal);

                KeyVal = UserQAPSettings.ListAppPosition.StartRow;
                if (!String.IsNullOrEmpty(KeyVal))
                    listAppStartRow.Value = Convert.ToDecimal(KeyVal);

                KeyVal = UserQAPSettings.ListAppPosition.StartCol;
                if (!String.IsNullOrEmpty(KeyVal))
                    listAppStartCol.Value = Convert.ToDecimal(KeyVal);

                KeyVal = UserQAPSettings.ParentChildAppPosition.StartRow;
                if (!String.IsNullOrEmpty(KeyVal))
                    parentChildStartRow.Value = Convert.ToDecimal(KeyVal);

                KeyVal = UserQAPSettings.ParentChildAppPosition.StartCol;
                if (!String.IsNullOrEmpty(KeyVal))
                    parentChildStartCol.Value = Convert.ToDecimal(KeyVal);

                KeyVal = UserQAPSettings.AppTitle.FormatAttribute.AppFontAndColor.AppFillColor; // ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListTitleColor);
                if (!String.IsNullOrEmpty(KeyVal))
                    txtTitleColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                else
                    txtTitleColor.BackColor = Color.FromArgb(220, 220, 255); //Light Sky Blue

                KeyVal = UserQAPSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.AppFillColor; // ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.DisplayFieldsColor);
                if (!String.IsNullOrEmpty(KeyVal))
                    txtDisplayFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                else
                    txtDisplayFieldsColor.BackColor = Color.White;

                KeyVal = UserQAPSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.AppFillColor; // ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SaveFieldsColor);
                if (!String.IsNullOrEmpty(KeyVal))
                    txtSaveFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                else
                    txtSaveFieldsColor.BackColor = Color.White;

                KeyVal = UserQAPSettings.AppTitle.FormatAttribute.AppFontAndColor.AppTextColor;

                if (!String.IsNullOrEmpty(KeyVal))
                {
                    txtTitleTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                }
                else
                    txtTitleTxtColor.BackColor = Color.White;


                KeyVal = UserQAPSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.AppTextColor;

                if (!String.IsNullOrEmpty(KeyVal))
                {
                    txtDisplayFieldsTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                }
                else
                    txtDisplayFieldsTxtColor.BackColor = Color.White;


                KeyVal = UserQAPSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.AppTextColor;

                if (!String.IsNullOrEmpty(KeyVal))
                {
                    txtSaveFieldsTxtColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
                }
                else
                    txtSaveFieldsTxtColor.BackColor = Color.White;

                KeyVal = UserQAPSettings.AppTitle.FormatAttribute.AppFontAndColor.FontStr;

                if (!String.IsNullOrEmpty(KeyVal))
                {

                    txtFont.Text = KeyVal;
                    txtFont.Tag = Utils.ConvertFontFromString(KeyVal);
                }
                else
                    txtFont.Text = UserQAPSettings.AppTitle.GetDefaultFont();


                KeyVal = UserQAPSettings.SaveOnlyField.FormatAttribute.AppFontAndColor.FontStr;

                if (!String.IsNullOrEmpty(KeyVal))
                {
                    txtFontSave.Text = KeyVal;
                    txtFontSave.Tag = Utils.ConvertFontFromString(KeyVal);
                }
                else
                    txtFontSave.Text = UserQAPSettings.SaveOnlyField.GetDefaultFont();

                KeyVal = UserQAPSettings.DisplayOnlyField.FormatAttribute.AppFontAndColor.FontStr;

                if (!String.IsNullOrEmpty(KeyVal))
                {
                    txtFontDisplay.Text = KeyVal;
                    txtFontDisplay.Tag = Utils.ConvertFontFromString(KeyVal);
                }
                else
                    txtFontDisplay.Text = UserQAPSettings.DisplayOnlyField.GetDefaultFont();





            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Options");
            }



            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListAppStartRow);
            if (!String.IsNullOrEmpty(KeyVal))
                listAppStartRow.Value = Convert.ToDecimal(KeyVal);

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListAppStartCol);
            if (!String.IsNullOrEmpty(KeyVal))
                listAppStartCol.Value = Convert.ToDecimal(KeyVal);

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ParentChildStartRow);
            if (!String.IsNullOrEmpty(KeyVal))
                parentChildStartRow.Value = Convert.ToDecimal(KeyVal);

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ParentChildStartCol);
            if (!String.IsNullOrEmpty(KeyVal))
                parentChildStartCol.Value = Convert.ToDecimal(KeyVal);

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.ListTitleColor);
            if (!String.IsNullOrEmpty(KeyVal))
                txtTitleColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
            else
                txtTitleColor.BackColor = Color.FromArgb(220, 220, 255); //Light Sky Blue

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.DisplayFieldsColor);
            if (!String.IsNullOrEmpty(KeyVal))
                txtDisplayFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
            else
                txtDisplayFieldsColor.BackColor = Color.White;

            KeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, RegistryBase, ApttusGlobals.SaveFieldsColor);
            if (!String.IsNullOrEmpty(KeyVal))
                txtSaveFieldsColor.BackColor = Color.FromArgb(Convert.ToInt32(KeyVal));
            else
                txtSaveFieldsColor.BackColor = Color.White;
             * 
             */
            #endregion

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

            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyHostName = txtProxyHost.Text;
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyPort = txtProxyPort.Value;
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyAuthenticate = chkProxyAuthenticated.Checked;
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyUserName = txtProxyUserName.Text;
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.ProxyFurtiveCode = Apttus.OAuthLoginControl.Helpers.StringCipher.encryptString(txtProxyPassword.Text);
            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.SaveTokenLocally = chkSaveTokenLocally.Checked;

            // List App Start Row
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ListAppStartRow))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ListAppStartRow] = Convert.ToString(5);
            // List App Start Col
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ListAppStartCol))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ListAppStartCol] = Convert.ToString(1);

            // Parent child App Start Row
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ParentChildStartRow))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ParentChildStartRow] = Convert.ToString(1);
            // Parent child App Start Col
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ParentChildStartCol))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ParentChildStartCol] = Convert.ToString(2);

            // Display only fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsColor] = txtDisplayFieldsColor.BackColor.ToArgb().ToString();
            // Save fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsColor] = txtSaveFieldsColor.BackColor.ToArgb().ToString();
            // Worksheet fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleColor] = txtTitleColor.BackColor.ToArgb().ToString();

            // Display only fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsTextColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsTextColor] = txtDisplayFieldsTxtColor.BackColor.ToArgb().ToString();
            // Save fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsTextColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsTextColor] = txtSaveFieldsTxtColor.BackColor.ToArgb().ToString();
            // Worksheet text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleTextColor))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleTextColor] = txtTitleTxtColor.BackColor.ToArgb().ToString();

            // Display only fields fore color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsFont))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsFont] = Utils.ConvertFontToString(txtFontDisplay.Tag as Font);
            // Save fields fore color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsFont))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsFont] = Utils.ConvertFontToString(txtFontSave.Tag as Font);
            // Worksheet fore color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.WorksheetTitleFont))
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleFont] = Utils.ConvertFontToString(txtFontTitle.Tag as Font);

            string selectedLanguage = cmbLanguage.SelectedItem.ToString();

            //Display Alert of Closing Excel to get effect of Language Change.
            if ((Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference) && selectedLanguage != Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference]) || cmbLanguage.SelectedIndex != 0)
            {
                MessageBox.Show(resourceManager.GetResource("OPTIONSFORM_ExcelCloseMessage_Text"), resourceManager.GetResource("OPTIONSFORM_grpLanguage_Text"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            AddExtendedProperty(ApttusGlobals.ResourcePreference, selectedLanguage);


            Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.Save();

            Helpers.ProxyHelper.ResetProxy();
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

        private void ApttusOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isFormDirty)
            {
                if (MessageBox.Show(resourceManager.GetResource("COMMONS_AreYouSure_ShowMsg"), Constants.DESIGNER_PRODUCT_NAME + resourceManager.GetResource("OPTIONS_OptionsCap_ShowMsg"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
        }

        private void listAppStartRow_ValueChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void listAppStartCol_ValueChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void parentChildStartRow_ValueChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void parentChildStartCol_ValueChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }

        private void txtTitleColor_Click(object sender, EventArgs e)
        {
            SetColor(txtTitleColor);
        }

        private void txtDisplayFieldsColor_Click(object sender, EventArgs e)
        {
            SetColor(txtDisplayFieldsColor);
        }

        private void txtSaveFieldsColor_Click(object sender, EventArgs e)
        {
            SetColor(txtSaveFieldsColor);
        }

        private void SetColor(TextBox textbox)
        {

            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textbox.BackColor = dialog.Color;
                isFormDirty = true;
                EnableDisableButtons();
            }

        }

        internal void HideQuickAppSettings()
        {
            tabApttus.TabPages.Remove(tabQuickApp);
        }

        internal void HideGeneral()
        {
            tabApttus.TabPages.Remove(tabGeneral);
        }

        private void txtTitleColor_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFont_Click(object sender, EventArgs e)
        {

            SetFont(txtFontTitle);
        }
        private void SetFont(TextBox textbox)
        {
            FontDialog dialog = new FontDialog();
            if (textbox.Tag != null)
            {
                dialog.Font = textbox.Tag as Font;
            }
            else if (textbox.Text.Length > 0) // default value
            {
                try
                {
                    dialog.Font = Utils.ConvertFontFromString(textbox.Text);
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }
            }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Font fnt = dialog.Font;
                textbox.Tag = fnt;
                string fntStr = Utils.ConvertFontToString(fnt);
                if (fntStr != null)
                {
                    textbox.Text = fntStr;
                }


                isFormDirty = true;
                EnableDisableButtons();
            }

        }

        private void txtFontSave_Click(object sender, EventArgs e)
        {
            SetFont(txtFontSave);
        }

        private void txtFontDisplay_Click(object sender, EventArgs e)
        {
            SetFont(txtFontDisplay);
        }

        private void txtTitleTxtColor_Click(object sender, EventArgs e)
        {
            SetColor(txtTitleTxtColor);
        }

        private void txtDisplayFieldsTxtColor_Click(object sender, EventArgs e)
        {
            SetColor(txtDisplayFieldsTxtColor);
        }

        private void txtSaveFieldsTxtColor_Click(object sender, EventArgs e)
        {
            SetColor(txtSaveFieldsTxtColor);
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            isFormDirty = true;
            EnableDisableButtons();
        }
    }
}
