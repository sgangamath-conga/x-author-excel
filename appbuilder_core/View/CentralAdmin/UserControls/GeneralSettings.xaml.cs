using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettings : System.Windows.Controls.UserControl
    {

        private static GeneralSettings _instance;
        private ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        private ActiveCRMInfo activeCRMInfo = null;
        private Dictionary<string, CRM> CRMTypes = new Dictionary<string, CRM>();
        private string ActiveCRMConfigFilePath;

        public static GeneralSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GeneralSettings();

                return _instance;
            }
        }
        private GeneralSettings()
        {
            if (applicationConfigManager.ApplicationSettings == null)
                applicationConfigManager.LoadApplicationSettings();
            InitializeComponent();
            SetCultureData();
            if (!CRMContextManager.Instance.ActiveCRM.Equals(CRM.Salesforce))
            {
                chkStoreToken.Visibility = System.Windows.Visibility.Collapsed;
                imgInfo.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SetCultureData()
        {
            lblTitle.Content = resourceManager.GetResource("CAD_Settings_Title");
            lblGeneral.Content = resourceManager.GetResource("COMMON_General_Text");
            chkStoreToken.Content = resourceManager.GetResource("CAD_Settings_StoreToken");
            imgInfo.ToolTip = resourceManager.GetResource("CAD_Settings_StoreToken_Info");
            lblActiveSource.Content = resourceManager.GetResource("CAD_Settings_ActiveSource");
            lblLanguage.Content = resourceManager.GetResource("OPTIONSFORM_lblLanguage_Text");
            lblDisplay.Content = resourceManager.GetResource("COMMON_Display_Text");
            chkSequentialActionFlowDesigner.Content = resourceManager.GetResource("CAD_Settings_SequentialActionFlowDesigner");
            chkHideFormulaRowDisplayMap.Content = resourceManager.GetResource("CAD_Settings_HideFormulaRowDisplayMap");
            lblAppWizard.Content = resourceManager.GetResource("CAD_Settings_AppWizard");
            lblAppliesToInd.Content = resourceManager.GetResource("CAD_Settings_AppliesToInd");
            lblAppliesToRep.Content = resourceManager.GetResource("CAD_Settings_AppliesToRep");
            lblIndFill.Content = resourceManager.GetResource("OPTIONSFORM_label5_Text");
            lblRepFill.Content = resourceManager.GetResource("OPTIONSFORM_label5_Text");
            lblIndText.Content = resourceManager.GetResource("OPTIONSFORM_label6_Text");
            lblRepText.Content = resourceManager.GetResource("OPTIONSFORM_label6_Text");
            lblIndFont.Content = resourceManager.GetResource("OPTIONSFORM_label3_Text");
            lblRepFont.Content = resourceManager.GetResource("OPTIONSFORM_label3_Text");
            lblDisplayLabel.Content = resourceManager.GetResource("CAD_Settings_DisplayLabel");
            lblDisplayData.Content = resourceManager.GetResource("CAD_Settings_DisplayData");
            lblSaveLabel.Content = resourceManager.GetResource("CAD_Settings_SaveLabel");
            lblSaveData.Content = resourceManager.GetResource("CAD_Settings_SaveData");
            lblMinDisplayWidth.Content = resourceManager.GetResource("CAD_Settings_MinDisplayWidth");
            lblProxySettings.Content = resourceManager.GetResource("OPTIONSFORM_groupBox1_Text");
            rdNoProxy.Content = resourceManager.GetResource("OPTIONSFORM_rbNoProxy_Text");
            rdSystemProxy.Content = resourceManager.GetResource("OPTIONSFORM_rbSystemProxy_Text");
            rdManualProxy.Content = resourceManager.GetResource("OPTIONSFORM_rbManualProxy_Text");
            lblProxyAddress.Content = resourceManager.GetResource("OPTIONSFORM_lblProxyHost_Text");
            lblProxyPort.Content = resourceManager.GetResource("OPTIONSFORM_lblProxyPort_Text");
            lblAuthSettings.Content = resourceManager.GetResource("CAD_Settings_AuthSettings");
            chkAuthenticated.Content = resourceManager.GetResource("OPTIONSFORM_chkProxyAuthenticated_Text");
            lblUsername.Content = resourceManager.GetResource("OPTIONSFORM_lblProxyUserName_Text");
            lblPassword.Content = resourceManager.GetResource("COMMON_Password_Text");
            lblChatterSettings.Content = resourceManager.GetResource("OPTIONSFORM_tabChatter_Text");
            lblChatterRefreshDuration.Content = resourceManager.GetResource("OPTIONSFORM_lblChatterDuration_Text");
        }

        private void rdProxy_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton rd = e.Source as System.Windows.Controls.RadioButton;
            if (rd.Name.Equals("rdManualProxy") && rd.IsChecked.Value)
            {
                gridProxy.Visibility = System.Windows.Visibility.Visible;
                applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.Custom;
            }
            else
            {
                gridProxy.Visibility = System.Windows.Visibility.Collapsed;
                if (rd.Name.Equals("rdNoProxy") && rd.IsChecked.Value)
                {
                    applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.None;
                }
                else if (rd.Name.Equals("rdSystemProxy") && rd.IsChecked.Value)
                {
                    applicationConfigManager.ApplicationSettings.ProxyType = SettingsManager.ProxyTypeEnum.System;
                }
            }
        }

        private void UCGeneralSettings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (CentralAdmin.Instance.CurrentAddIn.Equals(ApplicationMode.Designer))
                {
                    stkChatter.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    stkChatter.Visibility = System.Windows.Visibility.Visible;
                    stkAppWizards.Visibility = System.Windows.Visibility.Collapsed;
                    PopulateChatterDurationDropDown();
                    LoadChatterDuration();
                }
                LoadOptions();
                LoadCRMComboBox();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void LoadCRMComboBox()
        {
            ActiveCRMConfigFilePath = Utils.GetActiveCRMConfigFilePath();
            bool doCRMConfigFileExists = File.Exists(ActiveCRMConfigFilePath);
            if (!doCRMConfigFileExists)
            {
                activeCRMInfo = new ActiveCRMInfo
                {
                    ActiveCRM = CRM.Salesforce
                };

                SaveCRMInfo(doCRMConfigFileExists);
            }

            CRMTypes["Salesforce"] = CRM.Salesforce;
            CRMTypes["Microsoft Dynamics CRM"] = CRM.DynamicsCRM;
            CRMTypes["Apttus Omni"] = CRM.AIC;
            CRMTypes["Apttus Omni V2"] = CRM.AICV2;

            cmbActiveCRM.DataContext = null;
            cmbActiveCRM.Items.Clear();

            cmbActiveCRM.ItemsSource = new BindingSource(CRMTypes, null);
            cmbActiveCRM.DisplayMemberPath = "Key";
            cmbActiveCRM.SelectedValuePath = "Value";

            activeCRMInfo = ApttusXmlSerializerUtil.SerializeFromFile<ActiveCRMInfo>(ActiveCRMConfigFilePath);
            cmbActiveCRM.SelectedValue = activeCRMInfo.ActiveCRM;

            cmbActiveCRM.SelectionChanged += cmbActiveCRM_SelectionChanged;
        }

        private void SaveCRMInfo(bool doCRMConfigFileExists = true)
        {
            ApttusXmlSerializerUtil.SerializeToFile(activeCRMInfo, ActiveCRMConfigFilePath);
            CRMChangedEvent.FireCRMChangedHandler(activeCRMInfo);
            if (doCRMConfigFileExists)
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CHANGECRM_ApplyMessage_Text"), resourceManager.GetResource("RIBBON_ChangeCRM_Text"));
        }

        private void LoadChatterDuration()
        {
            //string ChatterKeyVal = ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ChatterRefresh);
            string ChatterValue = Convert.ToString(applicationConfigManager.ApplicationSettings.ChatterRefreshDuration);

            //List<KeyValuePair<string, string>> chatterDurations = cboChatterDuration.DataSource as List<KeyValuePair<string, string>>;

            //If chatter value is nothing
            if (!string.IsNullOrEmpty(ChatterValue))
                cboChatterDuration.SelectedValue = ChatterValue;
            else
                cboChatterDuration.SelectedValue = 0;
        }
        private void PopulateChatterDurationDropDown()
        {

            // Create a List to store our KeyValuePairs
            List<KeyValuePair<string, string>> chatterDuration = new List<KeyValuePair<string, string>>
            {
                // Add data to the List
                new KeyValuePair<string, string>("Manual", "0"),
                new KeyValuePair<string, string>("1 Minute", "60000"),
                new KeyValuePair<string, string>("5 Minutes", "300000"),
                new KeyValuePair<string, string>("15 Minutes", "900000"),
                new KeyValuePair<string, string>("30 Minutes", "1800000"),
                new KeyValuePair<string, string>("60 Minutes", "3600000")
            };

            // Clear the combobox
            cboChatterDuration.DataContext = null;
            cboChatterDuration.Items.Clear();

            // Bind the combobox
            cboChatterDuration.ItemsSource = new BindingSource(chatterDuration, null);
            cboChatterDuration.DisplayMemberPath = "Key";
            cboChatterDuration.SelectedValuePath = "Value";
        }
        private void AddExtendedProperty(string key, string value)
        {
            if (applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(key))
                applicationConfigManager.ApplicationSettings.AppExtendedProperties[key] = value;
            else
                applicationConfigManager.ApplicationSettings.AppExtendedProperties.Add(key, value);
        }
        private void LoadOptions()
        {
            rdSystemProxy.IsChecked = (applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.System);
            rdManualProxy.IsChecked = (applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.Custom);
            rdNoProxy.IsChecked = (applicationConfigManager.ApplicationSettings.ProxyType == SettingsManager.ProxyTypeEnum.None);

            if (!String.IsNullOrEmpty(applicationConfigManager.ApplicationSettings.ProxyHostName))
                txtProxyAddress.Text = applicationConfigManager.ApplicationSettings.ProxyHostName;

            txtProxyPort.Text = applicationConfigManager.ApplicationSettings.ProxyPort.ToString();

            chkAuthenticated.IsChecked = applicationConfigManager.ApplicationSettings.ProxyAuthenticate;

            if (!String.IsNullOrEmpty(applicationConfigManager.ApplicationSettings.ProxyUserName))
                txtUsername.Text = applicationConfigManager.ApplicationSettings.ProxyUserName;

            if (!String.IsNullOrEmpty(applicationConfigManager.ApplicationSettings.ProxyFurtiveCode))
                txtPassword.Password = OAuthLoginControl.Helpers.StringCipher.decryptString(applicationConfigManager.ApplicationSettings.ProxyFurtiveCode);

            chkStoreToken.IsChecked = applicationConfigManager.ApplicationSettings.SaveTokenLocally;

            bool boolValue;
            string initialValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.SequentialActionFlowDesigner);
            if (string.IsNullOrEmpty(initialValue))
                chkSequentialActionFlowDesigner.IsChecked = true;
            else
            {
                bool.TryParse(initialValue, out boolValue);
                chkSequentialActionFlowDesigner.IsChecked = boolValue;
            }
            bool.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.HideFormulaRowDisplayMap), out boolValue);
            chkHideFormulaRowDisplayMap.IsChecked = boolValue;

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.DisplayFieldsMinWidth), out int minWidth);
            txtDisplayFieldsMinWidth.Text = minWidth.ToString();

            // Quick App settings
            // Fill colors
            System.Drawing.Color clr;
            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelFillColor), out int colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndDisplayLabelFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListDisplayLabelFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndDisplayDataFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListDisplayDataFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndSaveLabelFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListSaveLabelFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndSaveDataFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataFillColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListSaveDataFillColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            //Text colors
            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndDisplayLabelTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndDisplayDataTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListDisplayLabelTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListDisplayDataTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndSaveLabelTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtIndSaveDataTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListSaveLabelTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            int.TryParse(applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataTextColor), out colorValue);
            clr = System.Drawing.Color.FromArgb(colorValue);
            txtListSaveDataTextColor.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));

            //Fonts
            string propertyValue = string.Empty;
            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayLabelFont);
            txtIndDisplayLabelFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayLabelFont);
            txtListDisplayLabelFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualDisplayDataFont);
            txtIndDisplayDataFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListDisplayDataFont);
            txtListDisplayDataFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveLabelFont);
            txtIndSaveLabelFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveLabelFont);
            txtListSaveLabelFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.IndividualSaveDataFont);
            txtIndSaveDataFont.Text = propertyValue;

            propertyValue = applicationConfigManager.ApplicationSettings.GetExtendedProperty(ApttusGlobals.ListSaveDataFont);
            txtListSaveDataFont.Text = propertyValue;

            //Lanuage Preference
            if (applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference))
                cmbLanguage.SelectedIndex = applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference].Equals("English") ? 0 : 1;
            else
                cmbLanguage.SelectedIndex = 0;
        }
        private void chkAuthenticated_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            bool isChecked = chkAuthenticated.IsChecked.HasValue && chkAuthenticated.IsChecked.Value;
            txtUsername.IsEnabled = isChecked;
            txtPassword.IsEnabled = isChecked;
            applicationConfigManager.ApplicationSettings.ProxyAuthenticate = isChecked;
            if (!isChecked)
            {
                txtUsername.Text = string.Empty;
                txtPassword.Password = string.Empty;
            }
        }

        private void UCGeneralSettings_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.Save();
            _instance = (null);
        }

        private void TextBoxColor_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox txtBox = sender as System.Windows.Controls.TextBox;
            ColorDialog dialog = new ColorDialog();
            SolidColorBrush solidColorBrush = txtBox.Background as SolidColorBrush;
            dialog.Color = System.Drawing.Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
                applicationConfigManager.ApplicationSettings.AddExtendedProperty(txtBox.Tag.ToString(), dialog.Color.ToArgb().ToString());
            }
        }

        private void TextBoxFont_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox txtBox = sender as System.Windows.Controls.TextBox;
            FontDialog dialog = new FontDialog();

            dialog.Font = Utils.ConvertFontFromString(applicationConfigManager.ApplicationSettings.GetExtendedProperty(txtBox.Tag.ToString()));
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fntStr = Utils.ConvertFontToString(dialog.Font);
                if (fntStr != null)
                {
                    txtBox.Text = fntStr;
                    applicationConfigManager.ApplicationSettings.AddExtendedProperty(txtBox.Tag.ToString(), fntStr);
                }
            }
        }

        private void txtProxyPort_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        private void chkStoreToken_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.SaveTokenLocally = chkStoreToken.IsChecked.HasValue && chkStoreToken.IsChecked.Value;
        }

        private void chkSequentialActionFlowDesigner_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.AddExtendedProperty(ApttusGlobals.SequentialActionFlowDesigner, chkSequentialActionFlowDesigner.IsChecked.GetValueOrDefault().ToString());
        }

        private void chkHideFormulaRowDisplayMap_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.AddExtendedProperty(ApttusGlobals.HideFormulaRowDisplayMap, chkHideFormulaRowDisplayMap.IsChecked.GetValueOrDefault().ToString());

        }

        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLanguage = ((ListBoxItem)cmbLanguage.SelectedItem).Content.ToString();

            //Display Alert of Closing Excel to get effect of Language Change.
            if ((applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference) && selectedLanguage != applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference]))
            {
                MessageBox.Show(ApttusResourceManager.GetInstance.GetResource("OPTIONSFORM_ExcelCloseMessage_Text"), ApttusResourceManager.GetInstance.GetResource("OPTIONSFORM_grpLanguage_Text"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            AddExtendedProperty(ApttusGlobals.ResourcePreference, selectedLanguage);
        }

        private void txtProxyAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.ProxyHostName = txtProxyAddress.Text;
        }

        private void txtProxyPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProxyPort.Text))
                applicationConfigManager.ApplicationSettings.ProxyPort = Convert.ToDecimal(txtProxyPort.Text);
            else applicationConfigManager.ApplicationSettings.ProxyPort = 80;
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.ProxyUserName = txtUsername.Text;
        }

        private void txtPassword_TextChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            applicationConfigManager.ApplicationSettings.ProxyFurtiveCode = OAuthLoginControl.Helpers.StringCipher.encryptString(txtPassword.Password);
        }

        private void cboChatterDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<string, string> chatterPair = (KeyValuePair<string, string>)cboChatterDuration.SelectedItem;
            applicationConfigManager.ApplicationSettings.ChatterRefreshDuration = Convert.ToInt32(chatterPair.Value);
        }

        private void cmbActiveCRM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<string, CRM> selectedCRM = (KeyValuePair<string, CRM>)cmbActiveCRM.SelectedItem;
            activeCRMInfo.ActiveCRM = selectedCRM.Value;
            SaveCRMInfo();
        }

        private void UCGeneralSettings_Initialized(object sender, EventArgs e)
        {
            if (CentralAdmin.Instance.IsLoadedFirstTime && CRMContextManager.Instance.ActiveCRM.Equals(CRM.Salesforce))
            {
                CentralAdmin.Instance.IsLoadedFirstTime = false;
                MessageBox.Show("'Remember me' will be checked by default in new settings.", "General Settings", MessageBoxButtons.OK);
            }
        }

        private void txtDisplayFieldsMinWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = txtDisplayFieldsMinWidth.Text;
            if (string.IsNullOrEmpty(value) || Convert.ToInt32(value) == 0)
            {
                value = ApttusGlobals.DisplayFieldsMinWidthValue.ToString();
            }
            applicationConfigManager.ApplicationSettings.AddExtendedProperty(ApttusGlobals.DisplayFieldsMinWidth, value);
        }

        private void txtDisplayFieldsMinWidth_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private bool IsNumeric(string text)
        {
            return text.ToCharArray().All(c => char.IsDigit(c) || char.IsControl(c));
        }

        private void txtDisplayFieldsMinWidth_Pasting(object sender, System.Windows.DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text1 = (string)e.DataObject.GetData(typeof(string));
                if (!IsNumeric(text1)) e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
