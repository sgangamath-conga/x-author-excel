using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for CentralAdmin.xaml
    /// </summary>
    public enum CentralUserControls
    {
        General,
        Settings,
        Logs,
        SalesforceMessages,
        Connections,
        AppSettings,
        AppDetails,
        LicenseInfo,
        AutoUpdate,
        HealthCheck,
        AppAssignment,
        ServiceLogs
    }
    public class AppSettingsEventArgs : EventArgs
    {
        private readonly XAEAppSettings _xaeAppSettings;

        public AppSettingsEventArgs(XAEAppSettings xaeAppSettings)
        {
            _xaeAppSettings = xaeAppSettings;
        }

        public XAEAppSettings xaeAppSettings
        {
            get { return _xaeAppSettings; }
        }
    }
    public partial class CentralAdmin : Window
    {
        private static CentralAdmin _instance;
        private ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        public ApplicationMode CurrentAddIn;
        public string AddInVersionNumber;
        public event EventHandler<AppSettingsEventArgs> AppSettingsEventHandler;
        public event EventHandler<AppSettingsEventArgs> AppSettingsValidationEventHandler;
        private Button LastActiveButton;
        public bool IsLoadedFirstTime = false;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private CentralUserControls activeCentralUserUI;
        public bool IsAutoUpdateClose = false;
        public static CentralAdmin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CentralAdmin();

                return _instance;
            }
        }

        public bool IsAppFileOpen { get; set; }

        public void SetCurrentAddIn(ApplicationMode mode)
        {
            CurrentAddIn = mode;
        }
        public void SetVersionNumber(string AddInVersionNumber)
        {
            this.AddInVersionNumber = AddInVersionNumber;
        }
        private CentralAdmin()
        {
            InitializeComponent();
            SetCultureData();
            double ActualScreenHeight = SystemParameters.PrimaryScreenHeight;
            double ActualScreenWidth = SystemParameters.PrimaryScreenWidth;
            Height = ActualScreenHeight * 0.90; // Covers 90% of the avialable height
            Width = ActualScreenWidth * 0.90;   // Covers 90% of the avialable width
            Top = ActualScreenHeight * 0.05;    // Keep 5% height from top 
            Left = ActualScreenWidth * 0.05;    // Keep 5% height from left
        }

        private void SetCultureData()
        {
            btnSalesforceMessages.Content = resourceManager.GetResource("CAD_SaveMessages", true);
            btnGeneral.Content = resourceManager.GetResource("CAD_GeneralInfo");
            btnSettings.Content = resourceManager.GetResource("COMMON_Settings_Text");
            btnLogs.Content = resourceManager.GetResource("CAD_ProductLogs");
            // btnServiceLogs.Content = resourceManager.GetResource("CAD_ServiceLogs");
            //btnAutoUpdate.Content = resourceManager.GetResource("CAD_AutoUpdate");
            // btnHealthCheck.Content = resourceManager.GetResource("CAD_HealthCheck");
            //btnLicenseInfo.Content = resourceManager.GetResource("CAD_LicenseInfo");
            btnConnections.Content = resourceManager.GetResource("CAD_Connections");
            btnApp.Content = resourceManager.GetResource("CAD_App");
            btnAppDetails.Content = resourceManager.GetResource("COMMON_AppDetails_Text");
            btnAppSettings.Content = resourceManager.GetResource("COMMON_AppSettings_Text");
            btnAppAssignment.Content = resourceManager.GetResource("CAD_AppAssignment");

        }
        private void ActivateButton(Button button)
        {
            if (LastActiveButton != null)
            {
                LastActiveButton.Background = null;
                LastActiveButton.FontWeight = FontWeights.Normal;
            }
            byte BackGround = Convert.ToByte(230);
            button.Background = new SolidColorBrush(Color.FromRgb(BackGround, BackGround, BackGround));
            button.FontWeight = FontWeights.SemiBold;
            LastActiveButton = button;
        }
        private void ChangeView(CentralUserControls centralUserControl)
        {
            activeCentralUserUI = centralUserControl;
            grdUserControl.Children.Clear();
            switch (centralUserControl)
            {
                case CentralUserControls.General:
                default:
                    ApttusUserInfo ApttusUserInfo = ObjectManager.GetInstance.getUserInfo();
                    GeneralInformation.Instance.SetGeneralInformation(ApttusUserInfo);
                    grdUserControl.Children.Add(GeneralInformation.Instance);
                    break;
                case CentralUserControls.Settings:
                    grdUserControl.Children.Add(GeneralSettings.Instance);
                    break;
                case CentralUserControls.Logs:
                case CentralUserControls.SalesforceMessages:
                case CentralUserControls.ServiceLogs:
                    Logs logsInstance = Logs.Instance;
                    grdUserControl.Children.Add(logsInstance);
                    logsInstance.LoadControl(centralUserControl);
                    break;
                case CentralUserControls.Connections:
                    grdUserControl.Children.Add(Connections.Instance);
                    break;
                case CentralUserControls.AppSettings:
                    grdUserControl.Children.Add(XAEAppSettings.Instance);
                    break;
                case CentralUserControls.AppDetails:
                    grdUserControl.Children.Add(AppDetails.Instance);
                    break;
                //case CentralUserControls.LicenseInfo:
                //    grdUserControl.Children.Add(LicenseDetails.Instance);
                //    break;
                //case CentralUserControls.AutoUpdate:
                //    grdUserControl.Children.Add(AutoUpdate.Instance);
                //    break;
                //case CentralUserControls.HealthCheck:
                //    grdUserControl.Children.Add(HealthCheck.Instance);
                //    break;
                case CentralUserControls.AppAssignment:
                    grdUserControl.Children.Add(AppAssignment.Instance);
                    break;
            }
        }

        internal void InvokeCheckAppSettings(XAEAppSettings instance)
        {
            AppSettingsEventHandler?.Invoke(this, new AppSettingsEventArgs(instance));
        }

        private void CollapseAppSection()
        {
            btnApp.Visibility = Visibility.Collapsed;
            btnAppAssignment.Visibility = Visibility.Collapsed;
            btnAppDetails.Visibility = Visibility.Collapsed;
            btnAppSettings.Visibility = Visibility.Collapsed;
        }
        private void Central_Admin_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                applicationConfigManager.LoadApplicationSettings();
                CheckAndInitializeFirstTimeLoad();
                if (CurrentAddIn.Equals(ApplicationMode.Runtime) || (CurrentAddIn.Equals(ApplicationMode.Designer) && !IsAppFileOpen))
                {
                    CollapseAppSection();
                }

                ApttusUserInfo ApttusUserInfo = ObjectManager.GetInstance.getUserInfo();
                //if (FreemuimLicenseManager.FreemiumInfo == null) btnLicenseInfo.Visibility = Visibility.Collapsed;

                //App Assignment is only for designer users
                if (IsAppFileOpen && !ObjectManager.GetInstance.IsDesigner())
                    btnAppAssignment.Visibility = Visibility.Collapsed;

                ChangeView(CentralUserControls.General);
                ActivateButton(btnGeneral);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void CheckAndInitializeFirstTimeLoad()
        {
            string FirstLoad = GetProperty(ApttusGlobals.FirstTimeLoadCentralAdmin);
            if (string.IsNullOrEmpty(FirstLoad))
            {
                SaveProperty(ApttusGlobals.IsAutoUpdateOn, "true");
                SaveProperty(ApttusGlobals.AutoUpdateInterval, "15");
                SaveProperty(ApttusGlobals.AutoUpdateTime, "10:00");
                SaveProperty(ApttusGlobals.FirstTimeLoadCentralAdmin, "false");
                SaveProperty(ApttusGlobals.LastAutoUpdatedOn, "");
                SaveProperty(ApttusGlobals.LastDownloadedOn, "");
                //SaveProperty(ApttusGlobals.UpdateStatus, UpdateStatus.None.ToString());
                SaveProperty(ApttusGlobals.DownloadDirectoryPath, "");
                SaveProperty(ApttusGlobals.LastDownloadedOn, "");
                SaveProperty(ApttusGlobals.IsAddInEnableChecked, "true");
                SaveProperty(ApttusGlobals.AddInEnableCheckInterval, "15");
                SaveProperty(ApttusGlobals.IsClearLogFileChecked, "true");
                SaveProperty(ApttusGlobals.ClearLogFileInterval, "10");
                SaveProperty(ApttusGlobals.SequentialActionFlowDesigner, "true");
                IsLoadedFirstTime = true;
                applicationConfigManager.ApplicationSettings.SaveTokenLocally = true;
                applicationConfigManager.ApplicationSettings.Save();
            }
        }

        private string GetProperty(string property)
        {
            string result;
            if (applicationConfigManager.ApplicationSettings.AppExtendedProperties.TryGetValue(property, out result))
            {
                return result;
            }
            result = string.Empty;
            return result;
        }
        public void SaveProperty(string property, string value)
        {
            if (!applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(property))
                applicationConfigManager.ApplicationSettings.AppExtendedProperties.Add(property, value);
            else applicationConfigManager.ApplicationSettings.AppExtendedProperties[property] = value;
            applicationConfigManager.ApplicationSettings.Save();
        }
        private void Central_Admin_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                grdUserControl.Children.Clear();
                _instance = null;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void Central_Admin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (CurrentAddIn.Equals(ApplicationMode.Designer) && IsAppFileOpen)
                {
                    if (XAEAppSettings.Instance.IsProtectSheetDirty)
                    {
                        AppSettingsValidationEventHandler?.Invoke(this, new AppSettingsEventArgs(XAEAppSettings.Instance));
                        if (XAEAppSettings.Instance.IsPasswordMismatch)
                        {
                            e.Cancel = true;
                            if (activeCentralUserUI != CentralUserControls.AppSettings)
                            {
                                ChangeView(CentralUserControls.AppSettings);
                                ActivateButton(btnAppSettings);
                            }
                        }
                    }
                    if (ConfigurationManager.GetInstance.Definition.AppSettings.AutoSizeColumns &&
                        (ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth == 0
                        || ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth > 255))
                    {
                        e.Cancel = true;
                        if (activeCentralUserUI != CentralUserControls.AppSettings)
                        {
                            ChangeView(CentralUserControls.AppSettings);
                            ActivateButton(btnAppSettings);
                        }
                        XAEAppSettings.SetFocusToColWidth();
                        ShowColumnWidthMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private bool CanChangeView()
        {
            bool result = true;
            if (CurrentAddIn.Equals(ApplicationMode.Designer) && IsAppFileOpen && activeCentralUserUI == CentralUserControls.AppSettings)
            {
                if (XAEAppSettings.Instance.IsProtectSheetDirty)
                {
                    AppSettingsValidationEventHandler?.Invoke(this, new AppSettingsEventArgs(XAEAppSettings.Instance));
                    if (XAEAppSettings.Instance.IsPasswordMismatch)
                    {
                        result = false;
                    }
                }
                if (ConfigurationManager.GetInstance.Definition.AppSettings.AutoSizeColumns &&
                    (ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth == 0
                    || ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth > 255))
                {
                    result = false;
                    ShowColumnWidthMessage();
                }
            }
            return result;
        }

        public bool ShowColumnWidthMessage()
        {
            if (ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth == 0)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPSETVIEW_ValidateColumnWidth_InfoMsg"), resourceManager.GetResource("COMMON_AppSettings_Text"));
                return false;
            }
            else if (ConfigurationManager.GetInstance.Definition.AppSettings.MaxColumnWidth > 255)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPSETVIEW_ValidateColumnWidthMAX_InfoMsg"), resourceManager.GetResource("COMMON_AppSettings_Text"));
                return false;
            }
            return true;
        }

        private void btnGeneral_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.General);
                ActivateButton(btnGeneral);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.Settings);
                ActivateButton(btnSettings);
            }
        }
        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.Logs);
                ActivateButton(btnLogs);
            }
        }

        private void btnSalesforceMessages_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.SalesforceMessages);
                ActivateButton(btnSalesforceMessages);
            }
        }

        private void btnConnections_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.Connections);
                ActivateButton(btnConnections);
            }
        }

        private void btnAppSettings_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.AppSettings);
                ActivateButton(btnAppSettings);
            }
        }

        private void btnAppDetails_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.AppDetails);
                ActivateButton(btnAppDetails);
            }
        }

        //private void btnLicenseInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeView(CentralUserControls.LicenseInfo);
        //    ActivateButton(btnLicenseInfo);
        //}

        //private void btnAutoUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeView(CentralUserControls.AutoUpdate);
        //    ActivateButton(btnAutoUpdate);
        //}

        //private void btnHealthCheck_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeView(CentralUserControls.HealthCheck);
        //    ActivateButton(btnHealthCheck);
        //}

        private void btnAppAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (CanChangeView())
            {
                ChangeView(CentralUserControls.AppAssignment);
                ActivateButton(btnAppAssignment);
            }
        }

        //private void btnServiceLogs_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeView(CentralUserControls.ServiceLogs);
        //    ActivateButton(btnServiceLogs);
        //}
    }
}
