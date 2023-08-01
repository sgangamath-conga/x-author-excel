using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for AppAssignment.xaml
    /// </summary>
    enum AppAssignmentMode
    {
        Create,
        Edit
    }

    public class CRMSpecificMetadata
    {
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
        public string ProfileId { get; set; }
        public string AssignmentName { get; set; }
        public string AssignmentObjectId { get; set; }
        public Datatype ApplicationIdDataType { get; set; }
        public CRMSpecificMetadata()
        {
            switch (CRMContextManager.Instance.ActiveCRM)
            {
                case CRM.Salesforce:
                    ApplicationId = "Apttus_XApps__ApplicationId__c";
                    UserId = "Apttus_XApps__UserId__c";
                    ProfileId = "Apttus_XApps__ProfileId__c";
                    AssignmentName = "Name";
                    AssignmentObjectId = "Apttus_XApps__UserXAuthorApp__c";
                    ApplicationIdDataType = Datatype.String;
                    break;
                case CRM.DynamicsCRM:
                    ApplicationId = "apttus_xapps_applicationidid";
                    UserId = "apttus_xapps_useridid";
                    ProfileId = "apttus_xapps_profileid";
                    AssignmentName = "apttus_name";
                    AssignmentObjectId = "apttus_xapps_userxauthorapp";
                    ApplicationIdDataType = Datatype.Lookup;
                    break;
                case CRM.AIC:
                    ApplicationId = "ApplicationId";
                    UserId = "UserId";
                    ProfileId = "Profile";
                    AssignmentName = "Name";
                    AssignmentObjectId = "xae_AppAssignment";
                    ApplicationIdDataType = Datatype.String;
                    break;
            }
        }
    }
    public partial class AppAssignment : UserControl
    {

        private static AppAssignment _instance;
        private ObjectManager objectManager = ObjectManager.GetInstance;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private AppAssignmentModel CurrentAssignment;
        private AppAssignmentMode Mode;
        private List<User> CheckedUserCollection;
        private List<Profile> CheckedProfileCollection;
        private bool HasAccessToProfilesToCurrentUser = true;
        private bool HasAccessToUsersToCurrentUser = true;
        private CRMSpecificMetadata CRMSpecificMetadata = new CRMSpecificMetadata();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static AppAssignment Instance {
            get {
                if (_instance == null)
                    _instance = new AppAssignment();

                return _instance;
            }
        }
        private AppAssignment()
        {
            InitializeComponent();
            SetCultureInfo();
        }

        private void SetCultureInfo()
        {
            lblTitle.Content = resourceManager.GetResource("CAD_AppAssignment");
            lblAppAssignDetails.Content = resourceManager.GetResource("CAD_AppAssignment_AppAssignDetails");
            lblAppName.Content = resourceManager.GetResource("COREAPPBROWSEVIEW_AppName_HeaderText");
            lblAppAssigned.Content = resourceManager.GetResource("CAD_AppAssignment");
            lblSectionHeader.Content = resourceManager.GetResource("CAD_AppAssignment_CreateNewAssign");
            grpUsers.Header = resourceManager.GetResource("CAD_AppAssignment_UserAssignments");
            grpProfiles.Header = resourceManager.GetResource("CAD_AppAssignment_ProfileAssignments");
            btnAddRecordsByIds.Content = resourceManager.GetResource("CAD_AppAssignment_AddRecordsById");
            btnSearchUsers.Content = resourceManager.GetResource("COREAPPBROWSEVIEW_btnSearch_Text");
            btnProfile.Content = resourceManager.GetResource("COREAPPBROWSEVIEW_btnSearch_Text");
            colUserName.Header = resourceManager.GetResource("CAD_GeneralInfo_UserName");
            colUserEmail.Header = resourceManager.GetResource("CAD_GeneralInfo_UserEmail");
            colProfileHeader.Header = resourceManager.GetResource("CAD_AppAssignment_ProfileName");
            lblAddLineSepId.Content = resourceManager.GetResource("CAD_AppAssignment_AddLineSepIds");
            btnSaveAssignments.Content = resourceManager.GetResource("CAD_AppAssignment_SaveAssignment");

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadAppAssignmentDetails();
                Mode = AppAssignmentMode.Create;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void LoadAppAssignmentDetails()
        {
            CheckedUserCollection = new List<User>();
            CheckedProfileCollection = new List<Profile>();
            //Get Current assignments to app
            CurrentAssignment = objectManager.GetAppAssignmnetDetails(configurationManager.Definition.Id);
            lblAppNameDefinition.Content = configurationManager.Definition.Name;

            //Update Hyperlink
            double assigned_users = CurrentAssignment.Assignments.Count(t => t.User != null);
            double assigned_profiles = CurrentAssignment.Assignments.Count(t => t.Profile != null);

            Hyperlink hyperLink = new Hyperlink();
            hyperLink.Inlines.Add(string.Format(resourceManager.GetResource("CAD_AppAssignment_ClickToEdit"), assigned_users, assigned_profiles));
            hyperLink.Click += hlAppAssigned_Click;
            lblAppAssignedCount.Content = hyperLink;

            //Load Users
            string ExceptIds = GetCurrentAssignedIds(true);

            if (GetListOfUsers(string.Empty, ExceptIds, out List<User> users))
                dgUsers.ItemsSource = users;

            //Load Profiles
            if (GetListOfProfiles(string.Empty, string.Empty, out List<Profile> profiles))
            {
                foreach (var assignment in CurrentAssignment.Assignments)
                {
                    if (assignment.Profile != null)
                    {
                        assignment.Profile.Name = profiles.Where(t => t.Id.Equals(assignment.Profile.Id)).Select(t => t.Name).FirstOrDefault();
                        profiles.RemoveAll(t => t.Id.Equals(assignment.Profile.Id));
                    }
                }
                dgProfiles.ItemsSource = profiles;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _instance = (null);
        }

        private bool GetListOfUsers(string serachStr, string ExceptIds, out List<User> users)
        {
            users = new List<User>();
            try
            {
                if (HasAccessToUsersToCurrentUser)
                {
                    users = objectManager.GetUserList(serachStr, ExceptIds);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex is System.ServiceModel.FaultException faultEx && faultEx.Code.Name.Equals("INVALID_TYPE"))
                {
                    HasAccessToUsersToCurrentUser = false;
                    grdSplitter.Visibility = Visibility.Collapsed;
                    grpUsers.Visibility = Visibility.Collapsed;
                    ColumnDefinition columnDefinition = grdMain.ColumnDefinitions.First();
                    columnDefinition.Width = GridLength.Auto;
                    string Message = "User Assignment is disabled for the user '{0}' because it does not have access to list of users. Please contact your Salesforce Administrator to get access";
                    ShowRestrictionMessage(Message, "User Assignment Disabled");
                }
                else
                {
                    ExceptionLogHelper.ErrorLog(ex, true);
                }
                return false;
            }
        }
        private void ShowRestrictionMessage(string Message, string Caption)
        {
            ApttusUserInfo apttusUserInfo = ObjectManager.GetInstance.getUserInfo();
            MessageBox.Show(CentralAdmin.Instance, string.Format(Message, apttusUserInfo.UserName), Caption, MessageBoxButton.OK);
        }
        private bool GetListOfProfiles(string serachStr, string ExceptIds, out List<Profile> profiles)
        {
            profiles = new List<Profile>();
            try
            {
                if (HasAccessToProfilesToCurrentUser)
                {
                    profiles = objectManager.GetProfileList(serachStr, ExceptIds);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex is System.ServiceModel.FaultException faultEx && faultEx.Code.Name.Equals("INVALID_TYPE"))
                {
                    HasAccessToProfilesToCurrentUser = false;
                    grdSplitter.Visibility = Visibility.Collapsed;
                    grpProfiles.Visibility = Visibility.Collapsed;
                    grdMain.ColumnDefinitions.RemoveAt(2);
                    grdMain.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    string Message = "Profile Assignment is disabled for the user '{0}' because it does not have privilege to access list of profiles. Please contact your Salesforce Administrator to get access";
                    ShowRestrictionMessage(Message, "Profile Assignment Disabled");
                }
                else
                {
                    ExceptionLogHelper.ErrorLog(ex, true);
                }
                return false;
            }
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            string searchStr = txtSearchUser.Text.Trim();

            if (Mode.Equals(AppAssignmentMode.Create))
            {
                string ExceptIds = GetCurrentAssignedIds(true);
                if (GetListOfUsers(searchStr, ExceptIds, out List<User> users))
                {
                    CheckPreviouslyCheckedUsers(users);
                    dgUsers.ItemsSource = users;
                }
            }
            else
            {
                dgUsers.ItemsSource = CurrentAssignment.Assignments.Where(t => (t.User != null && (t.User.Name.ToLower().Contains(searchStr.ToLower()) || t.User.Email.ToLower().Contains(searchStr.ToLower()))))
                                      .Select(t => t.User)
                                      .ToList();
            }

        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            string searchStr = txtSearchProfile.Text.Trim();

            if (Mode.Equals(AppAssignmentMode.Create))
            {
                string ExceptIds = GetCurrentAssignedIds(false);
                if (GetListOfProfiles(searchStr, ExceptIds, out List<Profile> profiles))
                {
                    CheckPreviouslyCheckedProfiles(profiles);
                    dgProfiles.ItemsSource = profiles;
                }
            }
            else
            {
                dgProfiles.ItemsSource = CurrentAssignment.Assignments.Where(t => (t.Profile != null && t.Profile.Name.ToLower().Contains(searchStr.ToLower())))
                                      .Select(t => t.Profile)
                                      .ToList();
            }

        }

        private void hlAppAssigned_Click(object sender, RoutedEventArgs e)
        {
            if (Mode.Equals(AppAssignmentMode.Create))
            {
                dgUsers.ItemsSource = CurrentAssignment.Assignments.Where(t => t.User != null).Select(t => t.User).ToList();
                dgProfiles.ItemsSource = CurrentAssignment.Assignments.Where(t => t.Profile != null).Select(t => t.Profile).ToList();
                Mode = AppAssignmentMode.Edit;
                ChangeHyperLinkText(Mode);
            }
            else
            {
                string ExceptIds = GetCurrentAssignedIds(true);
                if (GetListOfUsers(string.Empty, ExceptIds, out List<User> users))
                {
                    CheckPreviouslyCheckedUsers(users);
                    dgUsers.ItemsSource = users;
                }

                ExceptIds = GetCurrentAssignedIds(false);
                if (GetListOfProfiles(string.Empty, ExceptIds, out List<Profile> profiles))
                {
                    CheckPreviouslyCheckedProfiles(profiles);
                    dgProfiles.ItemsSource = profiles;
                }
                Mode = AppAssignmentMode.Create;
                ChangeHyperLinkText(Mode);
            }
        }

        private void CheckPreviouslyCheckedUsers(List<User> users)
        {
            foreach (var user in users)
            {
                if (CheckedUserCollection.Exists(t => user.Id.Equals(t.Id)))
                {
                    user.Checked = true;
                }
            }
        }
        private void CheckPreviouslyCheckedProfiles(List<Profile> profiles)
        {
            foreach (var profile in profiles)
            {
                if (CheckedProfileCollection.Exists(t => profile.Id.Equals(t.Id)))
                {
                    profile.Checked = true;
                }
            }
        }
        private void ChangeHyperLinkText(AppAssignmentMode mode)
        {
            if (mode.Equals(AppAssignmentMode.Edit))
            {
                Hyperlink link = lblAppAssignedCount.Content as Hyperlink;
                link.Inlines.Clear();
                link.Inlines.Add(resourceManager.GetResource("CAD_AppAssignment_ClickToAdd"));
                lblSectionHeader.Content = resourceManager.GetResource("CAD_AppAssignment_EditExisting");
                btnAddRecordsByIds.Visibility = Visibility.Collapsed;
                if (stkAddRecords.Visibility == Visibility.Visible)
                {
                    btnAddRecordsByIds_Click(null, null);
                }
            }
            else
            {
                Hyperlink link = lblAppAssignedCount.Content as Hyperlink;
                link.Inlines.Clear();

                double assigned_users = CurrentAssignment.Assignments.Count(t => t.User != null);
                double assigned_profiles = CurrentAssignment.Assignments.Count(t => t.Profile != null);

                link.Inlines.Add(string.Format(resourceManager.GetResource("CAD_AppAssignment_ClickToEdit"), assigned_users, assigned_profiles));
                btnAddRecordsByIds.Visibility = Visibility.Visible;
                lblSectionHeader.Content = resourceManager.GetResource("CAD_AppAssignment_CreateNewAssign");
            }
        }

        private string GetCurrentAssignedIds(bool isUser)
        {
            string ExceptIds = string.Empty;
            if (isUser)
            {
                string[] CurrentAssignedUserIDs = CurrentAssignment.Assignments.Where(t => t.User != null).Select(t => t.User.Id).ToArray();
                ExceptIds = "'" + string.Join("','", CurrentAssignedUserIDs) + "'";
            }
            else
            {
                string[] CurrentAssignedProfileIDs = CurrentAssignment.Assignments.Where(t => t.Profile != null).Select(t => t.Profile.Id).ToArray();
                ExceptIds = "'" + string.Join("','", CurrentAssignedProfileIDs) + "'";
            }
            return ExceptIds;
        }

        private void btnSaveAssignments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppMessageBox appMessageBox = AppMessageBox.Instance;
                appMessageBox.Show();

                List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
                ApttusSaveField ApplicationId = new ApttusSaveField()
                {
                    FieldId = CRMSpecificMetadata.ApplicationId,
                    DataType = CRMSpecificMetadata.ApplicationIdDataType,
                    Value = CurrentAssignment.ApplicationId
                };
                if (CRMContextManager.Instance.ActiveCRM == CRM.AIC) ApplicationId.CRMDataType = "Lookup";
                //Create New User Assignment
                if (CheckedUserCollection.Count > 0)
                {
                    foreach (var user in CheckedUserCollection)
                    {
                        ApttusSaveField UserId = new ApttusSaveField()
                        {
                            FieldId = CRMSpecificMetadata.UserId,
                            DataType = Datatype.Lookup,
                            Value = user.Id
                        };
                        ApttusSaveField Name = new ApttusSaveField()
                        {
                            FieldId = CRMSpecificMetadata.AssignmentName,
                            DataType = Datatype.String,
                            Value = user.Name
                        };
                        SaveRecords.Add(new ApttusSaveRecord()
                        {
                            ObjectName = CRMSpecificMetadata.AssignmentObjectId,
                            SaveFields = new List<ApttusSaveField>() { ApplicationId, UserId, Name },
                            OperationType = QueryTypes.INSERT
                        });
                    }
                }

                //Create New User Assignment from Record IDs
                string richText = new TextRange(txtAddRecordsByUserIds.Document.ContentStart, txtAddRecordsByUserIds.Document.ContentEnd).Text;
                if (!string.IsNullOrEmpty(richText))
                {
                    string[] RecordIdsList = richText.Replace('\n', ' ').Trim().Split('\r');
                    foreach (var recordId in RecordIdsList.Where(t => !string.IsNullOrEmpty(t)))
                    {
                        if ((CurrentAssignment.Assignments.Count(t => t.User == null) == CurrentAssignment.Assignments.Select(t => t.User).Count())
                          || !CurrentAssignment.Assignments.Any(t => t.User != null && t.User.Id.Equals(recordId)))
                        {
                            ApttusSaveField UserId = new ApttusSaveField()
                            {
                                FieldId = CRMSpecificMetadata.UserId,
                                DataType = Datatype.Lookup,
                                Value = recordId.Trim()
                            };
                            ApttusSaveField Name = new ApttusSaveField()
                            {
                                FieldId = CRMSpecificMetadata.AssignmentName,
                                DataType = Datatype.String,
                                Value = "User " + recordId.Trim()
                            };
                            SaveRecords.Add(new ApttusSaveRecord()
                            {
                                ObjectName = CRMSpecificMetadata.AssignmentObjectId,
                                SaveFields = new List<ApttusSaveField>() { ApplicationId, UserId, Name },
                                OperationType = QueryTypes.INSERT
                            });
                        }
                    }
                }

                //Create New Profile Assignemnt
                if (CheckedProfileCollection.Count > 0)
                {
                    foreach (var profile in CheckedProfileCollection)
                    {
                        ApttusSaveField ProfileId = new ApttusSaveField()
                        {
                            FieldId = CRMSpecificMetadata.ProfileId,
                            DataType = Datatype.String,
                            Value = profile.Id
                        };
                        ApttusSaveField Name = new ApttusSaveField()
                        {
                            FieldId = CRMSpecificMetadata.AssignmentName,
                            DataType = Datatype.String,
                            Value = profile.Name
                        };
                        SaveRecords.Add(new ApttusSaveRecord()
                        {
                            ObjectName = CRMSpecificMetadata.AssignmentObjectId,
                            SaveFields = new List<ApttusSaveField>() { ApplicationId, ProfileId, Name },
                            OperationType = QueryTypes.INSERT
                        });
                    }
                }
                //Remove Currently Assigned User from Assignment
                if (CurrentAssignment.Assignments.Any(t => t.User != null && !t.User.Checked))
                {
                    foreach (var RemovedUserAssignment in CurrentAssignment.Assignments.Where(t => t.User != null && !t.User.Checked))
                    {
                        SaveRecords.Add(new ApttusSaveRecord()
                        {
                            ObjectName = CRMSpecificMetadata.AssignmentObjectId,
                            RecordId = RemovedUserAssignment.AssignmentId,
                            OperationType = QueryTypes.DELETE
                        });
                    }
                }

                //Remove Currently Assigned Profile from Assignment
                if (CurrentAssignment.Assignments.Any(t => t.Profile != null && !t.Profile.Checked))
                {
                    foreach (var RemovedUserAssignment in CurrentAssignment.Assignments.Where(t => t.Profile != null && !t.Profile.Checked))
                    {
                        SaveRecords.Add(new ApttusSaveRecord()
                        {
                            ObjectName = CRMSpecificMetadata.AssignmentObjectId,
                            RecordId = RemovedUserAssignment.AssignmentId,
                            OperationType = QueryTypes.DELETE
                        });
                    }
                }

                if (SaveRecords.Exists(s => s.OperationType == QueryTypes.INSERT))
                {
                    List<ApttusSaveRecord> InsertRecords = SaveRecords.Where(t => t.OperationType.Equals(QueryTypes.INSERT)).ToList();
                    objectManager.Insert(InsertRecords, true, 200);
                }

                if (SaveRecords.Exists(s => s.OperationType == QueryTypes.DELETE))
                {
                    List<ApttusSaveRecord> DeleteRecords = SaveRecords.Where(t => t.OperationType.Equals(QueryTypes.DELETE)).ToList();
                    objectManager.Delete(DeleteRecords, true);
                }
                int InsertCount = SaveRecords.Where(t => t.OperationType.Equals(QueryTypes.INSERT) && t.Success).Count();
                int DeleteCount = SaveRecords.Where(t => t.OperationType.Equals(QueryTypes.DELETE) && t.Success).Count();
                StringBuilder ErrorMessage = new StringBuilder();

                foreach (var SaveRecord in SaveRecords.Where(t => !t.Success))
                {
                    ErrorMessage.AppendLine("***** Error *****");
                    ErrorMessage.AppendLine(SaveRecord.ErrorMessage);
                    ErrorMessage.AppendLine("*****************");
                }
                appMessageBox.SetContent(InsertCount, DeleteCount, ErrorMessage.ToString());
                LoadAppAssignmentDetails();
                if (Mode == AppAssignmentMode.Edit)
                {
                    Mode = AppAssignmentMode.Create;
                    hlAppAssigned_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void txtSearchUser_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                btnUsers_Click(null, null);
            }
        }

        private void txtSearchProfile_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                btnProfile_Click(null, null);
            }
        }
        private void chkUserAssignChecked(object sender, RoutedEventArgs e)
        {
            User user = (sender as CheckBox).DataContext as User;
            if (Mode.Equals(AppAssignmentMode.Create))
                CheckedUserCollection.Add(user);

        }
        private void chkUserAssignUnChecked(object sender, RoutedEventArgs e)
        {
            User user = (sender as CheckBox).DataContext as User;
            if (Mode.Equals(AppAssignmentMode.Create))
                CheckedUserCollection.RemoveAll(t => t.Id.Equals(user.Id));

        }
        private void chkProfileAssignChecked(object sender, RoutedEventArgs e)
        {
            Profile profile = (sender as CheckBox).DataContext as Profile;
            if (Mode.Equals(AppAssignmentMode.Create))
                CheckedProfileCollection.Add(profile);

        }
        private void chkProfileAssignUnChecked(object sender, RoutedEventArgs e)
        {
            Profile profile = (sender as CheckBox).DataContext as Profile;
            if (Mode.Equals(AppAssignmentMode.Create))
                CheckedProfileCollection.RemoveAll(t => t.Id.Equals(profile.Id));

        }

        private void btnAddRecordsByIds_Click(object sender, RoutedEventArgs e)
        {
            Visibility visibility = dgUsers.Visibility;
            dgUsers.Visibility = stkAddRecords.Visibility;
            stkAddRecords.Visibility = visibility;

            if (stkAddRecords.Visibility.Equals(Visibility.Visible))
            {
                btnAddRecordsByIds.Content = resourceManager.GetResource("CAD_AppAssignment_AddRecordsBySelection");
                txtSearchUser.Visibility = Visibility.Collapsed;
                btnSearchUsers.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtAddRecordsByUserIds.Document.Blocks.Clear();
                btnAddRecordsByIds.Content = resourceManager.GetResource("CAD_AppAssignment_AddRecordsById");
                txtSearchUser.Visibility = Visibility.Visible;
                btnSearchUsers.Visibility = Visibility.Visible;
            }
        }
    }
}
