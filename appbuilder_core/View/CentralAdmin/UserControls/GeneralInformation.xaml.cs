using System;
using System.Windows;
using System.Windows.Controls;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for GeneralInformation.xaml
    /// </summary>
    public partial class GeneralInformation : UserControl
    {
        private static GeneralInformation _instance;
        private ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public static GeneralInformation Instance {
            get {
                if (_instance == null)
                    _instance = new GeneralInformation();

                return _instance;
            }
        }
        private void SetCultureInfo()
        {
            lblCopyright.Content = resourceManager.GetResource("ABOUT_lblCopyrightInfo_Text");
            lblAboutText.Text = resourceManager.GetResource("ABOUT_lblAboutDescription_Text", true);
            lblDesignerProductName.Content = Constants.DESIGNER_PRODUCT_NAME;
            lblRuntimeProductName.Content = Constants.RUNTIME_PRODUCT_NAME;
            lblTitle.Content = resourceManager.GetResource("CAD_GeneralInfo_Title");
            lblUserName.Content = resourceManager.GetResource("CAD_GeneralInfo_UserName");
            lblUserFullName.Content = resourceManager.GetResource("CAD_GeneralInfo_UserFullName");
            lblUserEmail.Content = resourceManager.GetResource("CAD_GeneralInfo_UserEmail");
            lblProfileId.Content = resourceManager.GetResource("CAD_GeneralInfo_ProfileId");
            lblUserId.Content = resourceManager.GetResource("CAD_GeneralInfo_UserId");
            lblOrganizationId.Content = resourceManager.GetResource("CAD_GeneralInfo_OrganizationId");
            lblOrganizationName.Content = resourceManager.GetResource("CAD_GeneralInfo_OrganizationName");
            btnProductInformation.Content = resourceManager.GetResource("CAD_GeneralInfo_ProductInformation");
            lblEdition.Content = resourceManager.GetResource("CAD_GeneralInfo_Edition");
            lblVersion.Content = resourceManager.GetResource("COMMON_Version_Text");
            lblAvailableAddIns.Content = resourceManager.GetResource("CAD_GeneralInfo_AvailableAddIns");
        }
        public void SetGeneralInformation(ApttusUserInfo apttusUserInfo)
        {
            try
            {
                if (apttusUserInfo != null)
                {
                    txtOrganizationId.Text = apttusUserInfo.OrganizationId;
                    txtProfileId.Text = apttusUserInfo.ProfileId;
                    txtUserEmail.Text = apttusUserInfo.UserEmail;
                    txtUserFullName.Text = apttusUserInfo.UserFullName;
                    txtUserName.Text = apttusUserInfo.UserName;
                    txtUserId.Text = apttusUserInfo.UserId;
                    txtEdition.Text = Constants.PRODUCT_EDITION;
                    txtOrganizationName.Text = apttusUserInfo.OrganizationName;
                }
                else
                {
                    brdAbout.Visibility = Visibility.Collapsed;
                    grdUserInfo.Visibility = Visibility.Collapsed;
                }

                txtVersion.Text = CentralAdmin.Instance.AddInVersionNumber;

                string value = string.Empty;
                value = GetProperty(ApttusGlobals.IsDesignerVisible);
                if (!string.IsNullOrEmpty(value) && !bool.Parse(value)) lblDesignerProductName.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
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
        private GeneralInformation()
        {
            InitializeComponent();
            if (applicationConfigManager.ApplicationSettings == null) applicationConfigManager.LoadApplicationSettings();
            SetCultureInfo();
        }

        // Ensure the instance is cleared when unloading
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            _instance = (null);
        }
    }
}
