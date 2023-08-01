using System;
using System.Windows.Controls;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for AppDetails.xaml
    /// </summary>
    public partial class AppDetails : UserControl, IDisposable
    {

        private static AppDetails _instance;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public bool IsAppDetailsDirty;
        private bool FirstInitialized;
        public static AppDetails Instance {
            get {
                if (_instance == null)
                    _instance = new AppDetails();

                return _instance;
            }
        }
        private AppDetails()
        {
            InitializeComponent();
            IsAppDetailsDirty = false;
            FirstInitialized = false;
            SetCultureInfo();
        }

        private void SetCultureInfo()
        {
            lblTitle.Content = resourceManager.GetResource("COMMON_AppDetails_Text");
            lblAppName.Content = resourceManager.GetResource("COREAPPBROWSEVIEW_AppName_HeaderText");
            lblUniqueId.Content = resourceManager.GetResource("CAD_AppDetails_UniqueId");
            lblActive.Content = resourceManager.GetResource("COMMON_IsActive_Text");
            lblSaveTheApp.Content = resourceManager.GetResource("CAD_AppDetails_SaveApp");
        }

        private void UCAppDetails_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                DataContext = configurationManager.Definition;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void UCAppDetails_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsAppDetailsDirty)
                Dispose();
        }
        public void Dispose()
        {
            _instance = null;
        }
        private void chkActive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FirstInitialized)
            {
                lblSaveTheApp.Visibility = System.Windows.Visibility.Visible;
                IsAppDetailsDirty = true;
            }
            else FirstInitialized = true;
        }

        private void txtAppName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FirstInitialized)
            {
                lblSaveTheApp.Visibility = System.Windows.Visibility.Visible;
                IsAppDetailsDirty = true;
            }
            else FirstInitialized = true;
        }
    }
}
