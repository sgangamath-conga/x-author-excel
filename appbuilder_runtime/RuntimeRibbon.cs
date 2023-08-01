using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Resources;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.AppRuntime;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.Text.RegularExpressions;
using System.Xml;
using Apttus.ChatterWebControl.UserControls;
using Microsoft.Win32;


namespace Apttus.XAuthor.AppRuntime
{
    public partial class RuntimeRibbon
    {
        private ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ApplicationController AppController = null;
        public MenuBuilder Builder;

        private UC_ChatterWeb ucChatterControl;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RuntimeRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            // Create a new icon from the handle. 
            System.Drawing.Icon newIcon = Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            btnChatterFeed.Image = Apttus.XAuthor.AppRuntime.Properties.Resources.chatterSelected.ToBitmap();
            btnAbout.Image = newIcon.ToBitmap();

            apttusNotification.Text = Constants.RUNTIME_PRODUCT_NAME;
            apttusNotification.Icon = newIcon;
            apttusNotification.Tag = null;
            Builder = new MenuBuilder(Globals.Ribbons.RuntimeRibbon.Tabs[0]);
            //Set Default Manual Option if there will no any settings in Registry
            string ChatterValue = string.Empty;
            ChatterValue = (ApttusRegistryManager.GetKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ChatterRefresh));
            if (ChatterValue == string.Empty)
                ApttusRegistryManager.UpdateKeyValue(RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, ApttusGlobals.ChatterRefresh, "0");
            //End
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            // Get list of user entries 
            string[] userNames = ApttusRegistryManager.GetAvailableUserNames(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase);
            // Get last login for Microsoft Office Application
            string lastLoginUser = ApttusRegistryManager.GetKeyValue(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, Globals.ThisAddIn.oAuthWrapper.ApplicationInfo.ApplicationType.ToString());

            // If no user entries are available, throw Manage Connections form.
            if ((userNames == null || userNames.Length == 0) || string.IsNullOrEmpty(lastLoginUser))
            {
                commandBar.ShowManageConnection();
            }
            else
            {
                commandBar.ShowLoginForm();
            }

            // Refresh connections, in case of first login connection list was not being populated.
            LoadUserConnections();

            LoadAppBuilder();
        }

        private void LoadAppBuilder()
        {
            if (commandBar.IsLoggedIn())
            {
                // If logged in successfully then do InitalizeStartMenu
                if (Globals.ThisAddIn.oAuthWrapper.token != null)
                    InitializeStartMenu();

                // Load any application if the template is open
                string currentAppUniqueId = LoadApplicationFromCurrentFile();
                if (!string.IsNullOrEmpty(currentAppUniqueId))
                    LoadApplication(string.Empty, currentAppUniqueId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, RibbonControlEventArgs e)
        {
            commandBar.DoAboutBox();
        }

        /// <summary>
        /// Load available connections
        /// </summary>
        private void LoadUserConnections()
        {
            mnuSwitchConnection.Items.Clear();
            string[] userNames = ApttusRegistryManager.GetAvailableUserNames(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase);

            if (userNames == null || userNames.Length == 0)
            {
                CreateRibbonButton("Manage Connection...", "MANAGE_CONNECTION", true, true, "DataOptionsMenu", false, false);
                return;
            }

            CreateRibbonButton("Manage Connection...", "MANAGE_CONNECTION", true, true, "DataOptionsMenu", true, false);

            if (userNames.Length > 0)
            {
                // Get the last login value
                string sLastLogin = ApttusRegistryManager.GetKeyValue(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForWord.ToString());

                for (int i = 0; i < userNames.Length; i++)
                {
                    CreateRibbonButton(userNames[i], userNames[i], (commandBar.IsLoggedIn() ? userNames[i] != sLastLogin : true), true, "_" + (i + 1), false, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sLabel"></param>
        /// <param name="sTag"></param>
        /// <param name="bEnabled"></param>
        /// <param name="bShowImage"></param>
        /// <param name="bIncludeSep"></param>
        private void CreateRibbonButton(string sLabel, string sTag, bool bEnabled, bool bShowImage, string officeImageId, bool bIncludeSep, bool bIsApplicationButton)
        {
            RibbonButton switchConnectionButton = this.Factory.CreateRibbonButton();
            switchConnectionButton.Label = sLabel;
            switchConnectionButton.Tag = sTag;
            switchConnectionButton.Enabled = bEnabled;
            switchConnectionButton.ShowImage = bShowImage;
            switchConnectionButton.OfficeImageId = officeImageId;

            if (bIsApplicationButton)
            {
                switchConnectionButton.Click += switchApplicationButton_Click;
                mnuApplication.Items.Add(switchConnectionButton);
            }
            else
            {
                switchConnectionButton.Click += new RibbonControlEventHandler(switchConnectionButton_Click);
                mnuSwitchConnection.Items.Add(switchConnectionButton);
            }

            if (bIncludeSep)
            {
                RibbonSeparator sep = this.Factory.CreateRibbonSeparator();
                sep.Name = "sep_" + sTag;

                if (bIsApplicationButton)
                    mnuApplication.Items.Add(sep);
                else
                    mnuSwitchConnection.Items.Add(sep);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchConnectionButton_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                RibbonButton selectedButton = sender as RibbonButton;

                switch (selectedButton.Tag.ToString())
                {
                    case "MANAGE_CONNECTION":
                        commandBar.ShowManageConnection();
                        break;

                    default:
                        string uName = selectedButton.Label;
                        string sHost = ApttusRegistryManager.GetKeyValue(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase + "\\" + uName, ApttusGlobals.ServerHostKey);

                        if (sHost.Length == 0 || sHost == string.Empty)
                        {
                            MessageBox.Show("Invalid connection URL, please provide valid URL and try again.", "Switch Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        Globals.ThisAddIn.oAuthWrapper.UserClosedForm = false;
                        Globals.ThisAddIn.oAuthWrapper.token = null;
                        Globals.ThisAddIn.oAuthWrapper.SwitchLoginId = uName;
                        Globals.ThisAddIn.oAuthWrapper.ConnectionEndPoint = sHost;

                        Globals.ThisAddIn.oAuthWrapper.SwitchLogin();

                        if (Globals.ThisAddIn.oAuthWrapper.token != null)
                        {
                            // Complete post login tasks
                            if (Globals.ThisAddIn.DoPostLogin())
                            {
                                Globals.ThisAddIn.NotifyLogin();
                            }
                        }
                        else if (Globals.ThisAddIn.oAuthWrapper.token == null)
                        {
                            Globals.ThisAddIn.oAuthWrapper.SwitchLoginId = string.Empty;
                        }

                        // Close login form after post login.
                        Globals.ThisAddIn.oAuthWrapper.CloseLoginForm();
                        Globals.ThisAddIn.oAuthWrapper.UserClosedForm = false;

                        break;
                }

                LoadUserConnections();

                LoadAppBuilder();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeStartMenu()
        {
            mnuApplication.Items.Clear();

            // Use Config manager method to retrieve available applications and load Ribbon Menu
            List<ApttusObject> lstApps = Globals.ThisAddIn.objectManager.GetAppList();

            if (lstApps == null || lstApps.Count == 0)
            {
                CreateRibbonButton("Open Application", "OPEN_APPLICATION", true, true, "DataOptionsMenu", false, true);
                return;
            }

            CreateRibbonButton("Open Application", "OPEN_APPLICATION", true, true, "DataOptionsMenu", true, true);

            if (lstApps.Count > 0)
            {
                // Load Application name through a method
                for (int i = 0; i < lstApps.Count; i++)
                {
                    //CreateRibbonButton("Demo Application 1", "Application_ID1", true, true, "_" + (i + 1), false, true);
                    CreateRibbonButton(lstApps[i].Name, lstApps[i].Id, true, true, "_" + (i + 1), false, true);
                }
            }
        }

        private string LoadApplicationFromCurrentFile()
        {
            MetadataReader mReader = new MetadataReader();
            return mReader.GetAppUniqueId();
        }

        private ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            ApplicationObject selectedApplication = null;
            AppController = new ApplicationController();
            if (!(string.IsNullOrEmpty(appId) & string.IsNullOrEmpty(appUniqueId)))
            {
                //selectedApplication = configurationManager.LoadApp(appId, appUniqueId);
                selectedApplication = AppController.LoadApplication(appId, appUniqueId);

                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                if (selectedApplication != null && (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true")))
                {
                    Builder.Config = configurationManager;
                    Builder.ClearMenus();
                    Builder.BuildMenus();
                }
            }
            return selectedApplication;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchApplicationButton_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                RibbonButton selectedButton = sender as RibbonButton;
                ApplicationObject selectedApplication = null;
                string AppName = string.Empty;

                switch (selectedButton.Tag.ToString())
                {
                    case "OPEN_APPLICATION":
                        string AppId = string.Empty;
                        ApplicationBrowserView view = new ApplicationBrowserView();
                        ApplicationBrowserController controller = new ApplicationBrowserController(view);
                        if (!string.IsNullOrEmpty(controller.selectedAppId))
                        {
                            selectedApplication = LoadApplication(controller.selectedAppId, string.Empty);
                            configurationManager.Definition.Name = AppName = controller.selectedAppName;
                        }
                        break;
                    default:
                        selectedApplication = LoadApplication(selectedButton.Tag.ToString(), string.Empty);
                        configurationManager.Definition.Name = AppName = selectedButton.Label;
                        break;
                }

                if (selectedApplication != null)
                {
                    string filePath = Utils.GetTempFileName(configurationManager.Definition.UniqueId, AppName + "-Runtime");
                    Utils.StreamToFile(selectedApplication.Config, filePath + Constants.XML);
                    string extension = Path.GetExtension(selectedApplication.TemplateName);

                    Microsoft.Office.Interop.Excel.Workbook workbook = ExcelHelper.CheckIfFileOpened(AppName + "-Runtime" + extension);
                    if (workbook != null) // WB is open
                    {
                        string IsRuntime = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
                        if (!string.IsNullOrEmpty(IsRuntime))
                        {
                            // Prompt user that the app is already open
                            ApttusMessageUtil.ShowError("Application " + workbook.Name + " is already open. Please close the file and reopen the application", "Application Open");
                            return;
                        }

                    }

                    Utils.StremToXlsx(selectedApplication.Template, filePath + extension);

                    // Add Current application instance
                    Utils.AddApplicationInstance(ApplicationMode.Runtime);

                    ApttusObjectManager.OpenFile(filePath + extension);

                    // Remove Designer property from Template 
                    ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_DESIGNER_SAVE);
                    // Add Runtime file property
                    ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_RUNTIME_FILE, "true");

                    // Render menus here 
                    Builder.Config = configurationManager;
                    Builder.ClearMenus();
                    Builder.BuildMenus();

                    ThisRibbonCollection ribb = Globals.Ribbons;
                    ribb.RuntimeRibbon.ABGroup1.RibbonUI.ActivateTab("tabAppBuilderRuntime");

                    ApttusCommandBarManager.g_IsAppOpen = true;
                    commandBar.EnableMenus();
                }
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuSwitchConnection_ItemsLoading(object sender, RibbonControlEventArgs e)
        {
            LoadUserConnections();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccessGroup_DialogLauncherClick(object sender, RibbonControlEventArgs e)
        {
            commandBar.ShowOptions();
        }


        private void Preview_Click(object sender, RibbonControlEventArgs e)
        {
            // TODO :: Load application data 
            //configurationManager.LoadAppFromLocal("a1ri0000000Y66mAAC", "Repo123");
            //WorkflowEngine wf = new WorkflowEngine();
            //// TODO : ID will pass from runtime menu
            //wf.Execute("8759198d-3c8e-4d2b-b67f-9c5acce14550");


        }

        #region "Edit Actions"

        private void btnAddRow_Click(object sender, RibbonControlEventArgs e)
        {
            AddRows(1);
        }

        private void Add3Rows_Click(object sender, RibbonControlEventArgs e)
        {
            AddRows(3);
        }

        private void Add5Rows_Click(object sender, RibbonControlEventArgs e)
        {
            AddRows(5);
        }

        private void Add10Rows_Click(object sender, RibbonControlEventArgs e)
        {
            AddRows(10);
        }

        private void AddCustomRows_Click(object sender, RibbonControlEventArgs e)
        {
            AddCustomRowsView View = new AddCustomRowsView();
            AddCustomRowsController Controller = new AddCustomRowsController(View);
            int RowsToAdd = Controller.GetCustomRows();
            if (RowsToAdd > 0)
                AddRows(RowsToAdd);
        }

        private void AddRows(int RowsToAdd)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
                if (NamedRange != null)
                {
                    RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(Selection, NamedRange);
                    runtimeEditActionController.AddRow(RowsToAdd);
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message, "AddRow Error");
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        private void RemoveRows(int RowsToRemove)
        {
            Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
            Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
            if (NamedRange != null)
            {
                RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(Selection, NamedRange);
                runtimeEditActionController.RemoveRow(RowsToRemove);
            }
        }

        #endregion

        private void btnChatterFeed_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);


                if (((RibbonToggleButton)sender).Checked)
                {
                    ApttusObjectManager.ShowBusyCursor("Retrieving Chatter Feeds...");
                    ucChatterControl = new UC_ChatterWeb(new ChatterWebAdapterExcel(),
                            XmlAuthorUtil.GetFrontDoorURL(Globals.ThisAddIn.oAuthWrapper.token.instance_url, Globals.ThisAddIn.oAuthWrapper.token.access_token, ""),
                            "/apex/" + "Apttus_Collab__ChatterHtmlFeed" + "?app=",
                            Globals.ThisAddIn.oAuthWrapper.ApplicationInfo.ApplicationType.ToString());

                    // Assign proxy object for the Manager class 
                    ////if (ApttusSessionManager.SalesForceSession.Proxy != null)
                    ////    ucChatterControl.Manager.Proxy = ApttusSessionManager.SalesForceSession.Proxy;

                    ////if (ApttusObjectManager.GetDocumentType() == "File")
                    ucChatterControl.Manager.InitPage(string.Empty);
                    ////else
                    //// ucChatterControl.Manager.InitPage(ApttusObjectManager.GetObjectId());

                    Microsoft.Office.Tools.CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes.Add(ucChatterControl, "Chatter", Globals.ThisAddIn.Application.ActiveWindow);
                    ctp.Control.Tag = commandBar.GetActiveWorkbookFullName();
                    ctp.VisibleChanged += ctp_VisibleChanged;
                    ctp.Width = 500;
                    ctp.Visible = true;
                    ApttusObjectManager.ShowNormalCursor();
                }
                else
                {
                    Microsoft.Office.Tools.CustomTaskPane ctp = null;


                    ctp = TaskPaneHelper.getTaskPaneInstance("Chatter");

                    if (ctp != null)
                    {
                        ctp.Visible = false;
                        Globals.ThisAddIn.CustomTaskPanes.Remove(ctp);
                        ctp.Dispose();
                        ctp = null;
                    }
                }
                // Subscribe Event for remove chatte pane
                //// ucChatterControl.RemoveChatterPaneEvent += new UC_ChatterWeb.RemoveChatterPaneEventHandler(RemoveChatterFeedPane);
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        void ctp_VisibleChanged(object sender, EventArgs e)
        {
            Microsoft.Office.Tools.CustomTaskPane taskPane = sender as Microsoft.Office.Tools.CustomTaskPane;

            if (taskPane != null)
            {
                //bool bShowTaskPane = Apttus.Common.Utilities.IsFileMenuClicked(new IntPtr(Globals.ThisAddIn.Application.Hwnd));

                btnChatterFeed.Checked = taskPane.Visible; //|| bShowTaskPane;

                if (taskPane.Visible == false) //&& bShowTaskPane == false)
                {
                    Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
                    taskPane.Dispose();
                    taskPane = null;
                }
            }
        }
        // Fire Remove Row Event
        private void btnRemoveRow_Click(object sender, RibbonControlEventArgs e)
        {
            RemoveRows(1);
        }


    }
}
