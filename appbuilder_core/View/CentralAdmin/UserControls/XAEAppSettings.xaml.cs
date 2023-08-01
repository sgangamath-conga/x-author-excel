using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for XAEAppSettings.xaml
    /// </summary>
    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAssistant), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false));

        private static void OnBoundPasswordChanged(System.Windows.DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(System.Windows.DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event

            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);
            // push the new password into the BoundPassword property
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(System.Windows.DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        public static bool GetBindPassword(System.Windows.DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        public static string GetBoundPassword(System.Windows.DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(System.Windows.DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(System.Windows.DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(System.Windows.DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
    }
    public partial class XAEAppSettings : UserControl
    {
        private static XAEAppSettings _instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        XAENumericUpDown numericUpDown = new XAENumericUpDown(CentralUserControls.AppSettings);
        AppSettings AppSettingsModel = ConfigurationManager.GetInstance.Application != null ? ConfigurationManager.GetInstance.Definition.AppSettings : null;
        public bool IsProtectSheetDirty;
        public bool IsPasswordMismatch;
        public static XAEAppSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new XAEAppSettings();

                return _instance;
            }
        }
        private XAEAppSettings()
        {
            InitializeComponent();
            SetCultureData();
            IsProtectSheetDirty = false;
            IsPasswordMismatch = false;
            numericHost.Child = numericUpDown;
            numericUpDown.NumericUpDownEventHandler += numericUpDown_ValueChanged;
        }

        private void SetCultureData()
        {
            lblAppSettings.Content = resourceManager.GetResource("COMMON_AppSettings_Text");
            chkDisableSaveLocalFile.Content = resourceManager.GetResource("APPSETTVIEW_chkDisableSaveLocalFile_Text");
            btnGeneral.Content = resourceManager.GetResource("COMMON_General_Text");
            chkIgnorePicklistValidation.Content = resourceManager.GetResource("APPSETTVIEW_ValidatePicklist_Text");
            lblMaxAttachmentSize.Content = resourceManager.GetResource("APPSETTVIEW_lblMaxAttachmentSize_Text");
            chkDisablePrint.Content = resourceManager.GetResource("APPSETTVIEW_chkDisablePrint_Text");
            chkDisableRichtextEditing.Content = resourceManager.GetResource("APPSETTVIEW_chkDisableRichtextEditing_Text");
            btnFormat.Content = resourceManager.GetResource("COMMON_Format_Text");
            chkShowFilters.Content = resourceManager.GetResource("COMMON_DisplayFilters_Text");
            chkAutoSizeColumn.Content = resourceManager.GetResource("APPSETTVIEW_chkAutoSizeColumn_Text");
            lblMaxColWidth.Content = resourceManager.GetResource("COMMON_MaxColumnWidth_Text");
            setRowColorlbl.Content = resourceManager.GetResource("APPSETTVIEW_setRowColor_Text");
            btnSupressMsg.Content = resourceManager.GetResource("APPSETTVIEW_grpDataMigration_Text");
            chkAllRecordsSaveSuccess.Content = resourceManager.GetResource("APPSETTVIEW_chkAllRecordsSaveSuccess_Text");
            chkSuppressDependent.Content = resourceManager.GetResource("APPSETTVIEW_chkSuppressDependent_Text");
            chkSuppressSave.Content = resourceManager.GetResource("APPSETTVIEW_chkSuppressSave_Text");
            chkSuppressNoOfRecords.Content = resourceManager.GetResource("APPSETTVIEW_chkSuppressNoOfRecords_Text");
            btnProtectSheet.Content = resourceManager.GetResource("APPSETTVIEW_grpProtectSheet_Text");
            dgNameColumn.Header = resourceManager.GetResource("COMMON_Name_Text").Replace(':', ' ');
            dgPasswordColumn.Header = resourceManager.GetResource("COMMON_Password_Text");
        }

        private void rowHighlightColor_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBox.Background = new SolidColorBrush(Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
                AppSettingsModel.RowErrorColor = dialog.Color.ToArgb().ToString();
            }
        }


        private void UCAppSettings_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CentralAdmin.Instance.InvokeCheckAppSettings(this);
                BindProtectSheets();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void BindProtectSheets()
        {
            try
            {
                if (AppSettingsModel == null)
                {
                    AppSettingsModel = new AppSettings();
                }
                chkDisableSaveLocalFile.IsChecked = AppSettingsModel.DisableLocalSaveFile;
                chkDisablePrint.IsChecked = AppSettingsModel.DisablePrint;
                chkDisableRichtextEditing.IsChecked = AppSettingsModel.DisableRichTextEditing;
                chkIgnorePicklistValidation.IsChecked = AppSettingsModel.IgnorePicklistValidation;
                if (AppSettingsModel.MaxAttachmentSize < 1 || AppSettingsModel.MaxAttachmentSize > 100)
                {
                    numericUpDown.SetValue(25);
                }
                else
                {
                    numericUpDown.SetValue(AppSettingsModel.MaxAttachmentSize);
                }

                // Suppress Messages for datamigration app
                chkSuppressNoOfRecords.IsChecked = AppSettingsModel.SuppressNoOfRecords;
                chkSuppressDependent.IsChecked = AppSettingsModel.SuppressDependent;
                chkSuppressSave.IsChecked = AppSettingsModel.SuppressSave;
                chkAllRecordsSaveSuccess.IsChecked = AppSettingsModel.SuppressAllRecordsSaveSuccess;

                //Format Settings
                chkShowFilters.IsChecked = AppSettingsModel.ShowFilters;
                chkAutoSizeColumn.IsChecked = AppSettingsModel.AutoSizeColumns;

                if (AppSettingsModel.RowErrorColor != null)
                {
                    System.Drawing.Color clr = System.Drawing.Color.FromArgb(Convert.ToInt32(AppSettingsModel.RowErrorColor));
                    rowHighlightColor.Background = new SolidColorBrush(Color.FromArgb(clr.A, clr.R, clr.G, clr.B));
                }
                else
                {
                    AppSettingsModel.RowErrorColor = "-3342490";
                }

                if (chkAutoSizeColumn.IsChecked.HasValue)
                {
                    txtMaxColumnWidth.TextChanged -= txtMaxColumnWidth_TextChanged;
                    txtMaxColumnWidth.IsEnabled = chkAutoSizeColumn.IsChecked.Value;
                    if (chkAutoSizeColumn.IsChecked.Value)
                        txtMaxColumnWidth.Text = Convert.ToString(AppSettingsModel.MaxColumnWidth);
                    txtMaxColumnWidth.TextChanged += txtMaxColumnWidth_TextChanged;
                }
                else
                {
                    txtMaxColumnWidth.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

        }

        private void UCAppSettings_Unloaded(object sender, RoutedEventArgs e)
        {
            _instance = null;
        }

        public void SetDataSource(List<SheetProtect> sheetProtectSettings)
        {
            if (sheetProtectSettings.Count > 0)
                dgProtectSheet.ItemsSource = sheetProtectSettings;
        }

        private void chkDisableSaveLocalFile_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.DisableLocalSaveFile = chkDisableSaveLocalFile.IsChecked.HasValue && chkDisableSaveLocalFile.IsChecked.Value;
        }

        private void chkDisablePrint_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.DisablePrint = chkDisablePrint.IsChecked.HasValue && chkDisablePrint.IsChecked.Value;
        }

        private void chkDisableRichtextEditing_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.DisableRichTextEditing = chkDisableRichtextEditing.IsChecked.HasValue && chkDisableRichtextEditing.IsChecked.Value;
        }

        private void chkIgnorePicklistValidation_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.IgnorePicklistValidation = chkIgnorePicklistValidation.IsChecked.HasValue && chkIgnorePicklistValidation.IsChecked.Value;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            decimal value = numericUpDown.GetValue();
            AppSettingsModel.MaxAttachmentSize = value;

        }
        private void chkAutoSizeColumn_Checked(object sender, RoutedEventArgs e)
        {
            txtMaxColumnWidth.IsEnabled = chkAutoSizeColumn.IsChecked.Value;
            AppSettingsModel.AutoSizeColumns = chkAutoSizeColumn.IsChecked.Value;
            if (chkAutoSizeColumn.IsChecked.Value)
            {
                txtMaxColumnWidth.Text = Convert.ToString(AppSettingsModel.MaxColumnWidth);
            }
            else
            {
                AppSettingsModel.MaxColumnWidth = 0;
                txtMaxColumnWidth.Text = AppSettingsModel.MaxColumnWidth.ToString();
            }
        }

        internal static void SetFocusToColWidth()
        {
            Instance.txtMaxColumnWidth.Focus();
        }

        private void chkShowFilters_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.ShowFilters = chkShowFilters.IsChecked.Value;
        }

        private void txtMaxColumnWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(txtMaxColumnWidth.Text,out int maxColumnWidth);
            AppSettingsModel.MaxColumnWidth = maxColumnWidth;
        }

        private void chkAllRecordsSaveSuccess_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.SuppressAllRecordsSaveSuccess = chkAllRecordsSaveSuccess.IsChecked.Value;
        }

        private void chkSuppressDependent_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.SuppressDependent = chkSuppressDependent.IsChecked.Value;
        }

        private void chkSuppressSave_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.SuppressSave = chkSuppressSave.IsChecked.Value;
        }

        private void chkSuppressNoOfRecords_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsModel.SuppressNoOfRecords = chkSuppressNoOfRecords.IsChecked.Value;
        }

        private void pasSheet_PasswordChanged(object sender, RoutedEventArgs e)
        {
            IsProtectSheetDirty = true;
        }

        public void ValidateSheetProtection(bool blnProtectSheetValidation, List<string> lstSheet)
        {
            IsPasswordMismatch = blnProtectSheetValidation;
            if (blnProtectSheetValidation)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_SheetProtect_ErrorMsg") + string.Join(",", lstSheet.ToArray()), Constants.DESIGNER_PRODUCT_NAME);
            }
        }
    }
}
