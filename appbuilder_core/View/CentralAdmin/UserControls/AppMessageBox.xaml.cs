using System;
using System.Windows;
using System.Windows.Interop;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class AppMessageBox : Window
    {

        private static AppMessageBox _instance;
        private AppMessageBox()
        {
            InitializeComponent();
            Owner = CentralAdmin.Instance;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        public static AppMessageBox Instance {
            get {
                if (_instance == null)
                    _instance = new AppMessageBox();

                return _instance;
            }
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CentralAdmin.Instance.IsEnabled = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CentralAdmin.Instance.IsEnabled = false;
            grdContent.Visibility = Visibility.Collapsed;
            lblErrors.Text = "Saving Records. Please Wait...";
            btnOK.Visibility = Visibility.Collapsed;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _instance = (null);
        }

        public void SetContent(int InsertCount, int DeleteCount, string ErrorMessages = "")
        {
            lblErrors.Text = "";
            lblUsersAdded.Text = InsertCount.ToString();
            lblUsersRemoved.Text = DeleteCount.ToString();

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                lblErrors.Text = ErrorMessages;
            }
            grdContent.Visibility = Visibility.Visible;
            btnOK.Visibility = Visibility.Visible;
            this.Top = Owner.Top + (Owner.Height - this.ActualHeight - 150) / 2;
        }
    }
}
