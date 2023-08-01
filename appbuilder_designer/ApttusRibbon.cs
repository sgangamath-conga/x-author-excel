/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using System.Runtime.InteropServices;

using Apttus.XAuthor.AppDesigner.Modules;
using System.Resources;
using Apttus.XAuthor.AppDesigner.Forms;
using Apttus.XAuthor.AppDesigner.MenuDesigner;
using Apttus.XAuthor.Core;
using System.IO;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing;
using Autofac;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ApttusRibbon
    {
        private ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private ContextMenuStrip notifyContextMenuStrip = new ContextMenuStrip();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private BaseApplicationController AppController;
        CentralAdmin centralAdmin = null;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);


        /// <summary>
        /// Sets Culture data for localization 
        /// </summary>
        private void SetCultureData()
        {
            AccessGroup.Label = resourceManager.GetResource("RIBBON_AccessGroup_Label");
            ApplicationGroup.Label = resourceManager.GetResource("COMMON _btnCreate_Text");
            btnAbout.Label = resourceManager.GetResource("RIBBON_btnAbout_Label");
            btnGeneral.Label = resourceManager.GetResource("RIBBON_btnGeneral_Label");
            btnAddRowAction.Label = resourceManager.GetResource("COMMON_AddRow_Text");
            btnAppDef.Label = resourceManager.GetResource("COMMON_SalesforceObject_Text",true);
            btnAppSetting.Label = resourceManager.GetResource("COMMON_AppSettings_Text");
            btnCallProcedure.Label = resourceManager.GetResource("RIBBON_btnCallProcedure_Label");
            btnCheckIn.Label = resourceManager.GetResource("RIBBON_btnCheckIn_Label");
            btnCheckOut.Label = resourceManager.GetResource("RIBBON_btnCheckOut_Label");
            btnDataSet.Label = resourceManager.GetResource("RIBBON_btnDataSet_Label");
            btnClearAction.Label = resourceManager.GetResource("COMMON_Clear_Text");
            btnConnect.Label = resourceManager.GetResource("CONNENTRY_btnConnect_Text").PadRight(8);
            btnDataLoader.Label = resourceManager.GetResource("RIBBON_btnDataLoader_Label");
            btnExecuteQuery.Label = resourceManager.GetResource("RIBBON_btnExecuteQuery_Label");
            btnExport.Label = resourceManager.GetResource("RIBBON_btnExport_Label");
            btnForm.Label = resourceManager.GetResource("RIBBON_btnForm_Label");
            btnExternalLibrary.Label = resourceManager.GetResource("RIBBON_btnExternalLibrary_Label");
            //add extra space at end
            btnGallerySupport.Label = resourceManager.GetResource("RIBBON_btnGallerySupport_Label").PadRight(17);
            btnImport.Label = resourceManager.GetResource("COMMON_Import_Text");
            btnMacroAction.Label = resourceManager.GetResource("RIBBON_btnMacroAction_Label");
            btnMatrixMap.Label = resourceManager.GetResource("RIBBON_btnMatrixMap_Label");
            btnMenuDesigner.Label = resourceManager.GetResource("COMMON_UserMenu_Text");
            btnPasteRow.Label = resourceManager.GetResource("RIBBON_btnPasteRow_Label");
            btnpasteSourceData.Label = resourceManager.GetResource("COMMON_PasteSourceData_Text");
            //add spaces to force fit in jpn translation
            btnPreview.Label = resourceManager.GetResource("COMMON_Preview_Text").PadRight(27);
            btnQuickAppEdit.Label = resourceManager.GetResource("COMMON_Edit_Text");
            btnQuickAppNew.Label = resourceManager.GetResource("COMMON_New_Text");
            btnQuickAppSettings.Label = resourceManager.GetResource("COMMON_Settings_Text");
            btnRetrieve.Label = resourceManager.GetResource("COMMON_Display_Text");
            btnRetrieveMap.Label = resourceManager.GetResource("COMMON_DisplayMap_Text");
            btnSaveAction.Label = resourceManager.GetResource("COMMON_btnSave_Text");
            btnSaveAs.Label = resourceManager.GetResource("RIBBON_btnSaveAs_Label");
            btnSaveAttachmentAction.Label = resourceManager.GetResource("RIBBON_btnSaveAttachmentAction_Label");
            btnSaveMap.Label = resourceManager.GetResource("RIBBON_btnSaveMap_Label");
            btnSearchSelect.Label = resourceManager.GetResource("RIBBON_btnSearchSelect_Label");
            btnSwitchConnection.Label = resourceManager.GetResource("COMMON_SwitchConnection_Text");
            btnUserInput.Label = resourceManager.GetResource("COMMON_UserInput_Text");
            btnValidate.Label = resourceManager.GetResource("RIBBON_btnValidate_Label");
            btnWorkflow.Label = resourceManager.GetResource("COMMON_ActionFlow_Text");
            groupBuild.Label = resourceManager.GetResource("RIBBON_groupBuild_Label");
            groupMigrate.Label = resourceManager.GetResource("RIBBON_groupMigrate_Label");
            groupPreview.Label = resourceManager.GetResource("COMMON_Preview_Text");
            groupSaveApp.Label = resourceManager.GetResource("COMMON_btnSave_Text");
            grpAppSettings.Label = resourceManager.GetResource("COMMON_Settings_Text");
            //added extra spaces to force to fit in jpn
            mnuApplication.Label = resourceManager.GetResource("RIBBON_mnuApplication_Label").PadRight(9);
            mnuSwitchConnection.Label = resourceManager.GetResource("COMMON_SwitchConnection_Text");
            //added extra spaces to force to fit in jpn
            splitBtnActions.Label = resourceManager.GetResource("COMMON_Actions_Text").PadRight(20);
            splitBtnQuickApp.Label = resourceManager.GetResource("COMMON_QuickApp_Text");
            splitbtnSave.Label = resourceManager.GetResource("RIBBON_splitbtnSave_Label");

            btnSetCRM.Label = resourceManager.GetResource("RIBBON_ChangeCRM_Text");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApttusRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            try
            {
                if (Globals.ThisAddIn.applicationConfigManager == null)
                {
                    Globals.ThisAddIn.applicationConfigManager = ApplicationConfigManager.GetInstance();
                    Globals.ThisAddIn.applicationConfigManager.LoadApplicationSettings();
                }

                UpdateResourceManager();

                // Create a new icon from the handle. 
                System.Drawing.Icon newIcon = Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
                SetCultureData();
                btnAbout.Image = newIcon.ToBitmap();

                apttusNotification.Text = Constants.DESIGNER_PRODUCT_NAME;
                apttusNotification.Icon = newIcon;
                apttusNotification.Tag = null;

                notifyContextMenuStrip.ShowImageMargin = true;
                notifyContextMenuStrip.ShowItemToolTips = false;

                //notifyContextMenuStrip.Items.Add(Constants.EXCEL_CONTEXT_ABOUTDESIGNER);
                //notifyContextMenuStrip.Items.Add(Constants.EXCEL_CONTEXT_OPTIONS);

                ToolStripMenuItem productName = new ToolStripMenuItem(Constants.EXCEL_CONTEXT_ABOUTDESIGNER);
                productName.Font = new Font(productName.Font, productName.Font.Style | FontStyle.Bold);

                notifyContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                productName,
                new ToolStripSeparator(),
                new ToolStripMenuItem(Constants.EXCEL_CONTEXT_VIEWDESIGNERLOG),
                new ToolStripMenuItem(Constants.EXCEL_CONTEXT_CLEAR)
            });

                (notifyContextMenuStrip.Items[3] as ToolStripMenuItem).DropDownItems.Add(
                    Constants.EXCEL_CONTEXT_CLEARLOGS, null, new EventHandler(Clear_ItemClicked));

                notifyContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(MainMenu_ItemClicked);

                apttusNotification.ContextMenuStrip = notifyContextMenuStrip;

                commandBar.InitializeCRM();

                AppController = Globals.ThisAddIn.GetApplicationController();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void UpdateResourceManager()
        {
            try
            {
                if (!Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.ResourcePreference))
                    return;

                string Language = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.ResourcePreference];
                resourceManager.UpdateResourceManager(Language);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //if (Globals.ThisAddIn.HasActiveWorkbook)
            //{
            //    if (Globals.ThisAddIn.Application.ActiveWindow.WindowState == Excel.XlWindowState.xlMinimized)
            //    {
            //        Globals.ThisAddIn.Application.ActiveWindow.WindowState = Excel.XlWindowState.xlMaximized;
            //    }
            //}

            //if (Globals.ThisAddIn.Application.Windows.Count > 0)
            //    Globals.ThisAddIn.Application.ActiveWindow.Activate();

            try
            {
                if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_ABOUTDESIGNER)
                {
                    commandBar.DoAboutBox();
                }
                else if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_OPTIONS)
                {
                    commandBar.ShowOptions();
                }
                else if (e.ClickedItem.Text == Constants.EXCEL_CONTEXT_VIEWDESIGNERLOG)
                {
                    Process pi = new Process();
                    pi.StartInfo.FileName = ExceptionLogHelper.LogFileFullPath;
                    pi.Start();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void Clear_ItemClicked(object sender, EventArgs e)
        {
            try
            {
                if ((sender as ToolStripItem).Text == Constants.EXCEL_CONTEXT_CLEARLOGS)
                {
                    File.WriteAllText(ExceptionLogHelper.LogFileFullPath, "");
                }
                else if ((sender as ToolStripItem).Text == Constants.EXCEL_CONTEXT_CLEARSAVEMESSAGES)
                {
                    File.WriteAllText(ExceptionLogHelper.SaveMessageLogFullPath, "");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // Get list of user entries 
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

                int userNames = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Count;

                // Get last login for Microsoft Office Application
                string lastLoginUser = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;

                // If no user entries are available, throw Manage Connections form.
                if (userNames == 0 || string.IsNullOrEmpty(lastLoginUser))
                {
                    commandBar.ShowManageConnection();
                }
                else
                {
                    commandBar.ShowLoginForm();
                }

                // Refresh connections, in case of first login connection list was not being populated.
                LoadUserConnections();

                // If logged in successfully then do InitalizeStartMenu
                if (commandBar.IsLoggedIn())
                {
                    InitializeStartMenu();
                    // After login if user is already on Designer template file enable menus again.
                    // In case of Session timeout or other similar scenarios, where user is working on App they might have many change locally
                    // if we do not enablemenus, all those changes are gone and designer will have to start over again, 
                    // this change will allow designer to save whatever changes, they did before their Session timedout.
                    if (ApttusObjectManager.GetDocProperty(Apttus.XAuthor.Core.Constants.APPBUILDER_DESIGNER_FILE) == "true")
                    {
                        ApttusCommandBarManager.g_IsAppOpen = true;
                        commandBar.EnableMenus();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                commandBar.DoAboutBox();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Load available connections
        /// </summary>
        private void LoadUserConnections()
        {
            mnuSwitchConnection.Items.Clear();
            int userNames = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Count;

            if (userNames == 0)
            {
                CreateRibbonButton(resourceManager.GetResource("MANAGECONNECTION_Ribbon_Title"), "MANAGE_CONNECTION", true, true, "DataOptionsMenu", false, false);
                return;
            }

            CreateRibbonButton(resourceManager.GetResource("MANAGECONNECTION_Ribbon_Title"), "MANAGE_CONNECTION", true, true, "DataOptionsMenu", true, false);

            if (userNames > 0)
            {
                // Get the last login value
                string sLastLogin = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;
                int i = 0;

                foreach (string sConnectionName in Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Keys)
                //for (int i = 0; i < userNames.Length; i++)
                {
                    CreateRibbonButton(sConnectionName, sConnectionName, (commandBar.IsLoggedIn() ? sConnectionName != sLastLogin : true), true, "_" + (i + 1), false, false);
                    i++;
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
        private void CreateRibbonButton(string sLabel, string sTag, bool bEnabled, bool bShowImage, string officeImageId, bool bIncludeSep, bool bIsApplicationButton, string Name = null)
        {
            RibbonButton switchConnectionButton = this.Factory.CreateRibbonButton();
            switchConnectionButton.Label = sLabel;
            switchConnectionButton.Tag = sTag;
            switchConnectionButton.Enabled = bEnabled;
            switchConnectionButton.ShowImage = bShowImage;
            switchConnectionButton.OfficeImageId = officeImageId;
            if (!string.IsNullOrEmpty(Name))
            {
                switchConnectionButton.Name = Name;
            }
            
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
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

                RibbonButton selectedButton = sender as RibbonButton;

                switch ((string)selectedButton.Tag.ToString())
                {
                    case "MANAGE_CONNECTION":
                        commandBar.ShowManageConnection();
                        break;

                    default:
                        string uName = selectedButton.Label;
                        commandBar.SwitchConnection(uName);
                        break;
                }

                LoadUserConnections();
                if (commandBar.IsLoggedIn())
                    InitializeStartMenu();
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
            ObjectManager objectManager = ObjectManager.GetInstance;

            List<ApttusObject> lstApps = objectManager.GetAppList(9, null, null);
            string newAppText = resourceManager.GetResource("COMMON_STARTMENU_NEWAPP");
            string openAppText = resourceManager.GetResource("COMMON_STARTMENU_OPENAPP");
            string EditAppTypeText = resourceManager.GetResource("COMMON_STARTMENU_CHANGE_EDITION");
            if (lstApps == null || lstApps.Count == 0)
            {
                if (!commandBar.IsBasicEdition())
                {
                    CreateRibbonButton(newAppText, "NEW_APPLICATION", true, true, "SmartArtEditIn2D", false, true);
                }
                return;
            }

            if (!commandBar.IsBasicEdition())
                CreateRibbonButton(newAppText, "NEW_APPLICATION", true, true, "SmartArtEditIn2D", true, true);
            CreateRibbonButton(openAppText, "OPEN_APPLICATION", true, true, "DataOptionsMenu", true, true);
            //CreateRibbonButton(EditAppTypeText, "EDIT_APPTYPE", false, true, "DataOptionsMenu", true, true, "EDIT_APPTYPE");
            

            if (lstApps.Count > 0)
            {
                for (int i = 0; i < lstApps.Count; i++)
                {
                    CreateRibbonButton(lstApps[i].Name, lstApps[i].Id, true, true, "_" + (i + 1), false, true);
                }
            }
        }

        private ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            ApplicationObject selectedApplication = null;
            //AppController = new SalesforceApplicationController();

            if (!(string.IsNullOrEmpty(appId) & string.IsNullOrEmpty(appUniqueId)))
            {
                // selectedApplication = configurationManager.LoadApp(appId, appUniqueId);
                selectedApplication = AppController.LoadApplication(appId, appUniqueId);
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
            bool blnOpenApp = false;
            try
            {
                RibbonButton selectedButton = sender as RibbonButton;
                ApplicationObject selectedApplication = null;
                string AppName = string.Empty;

                //if (ExcelHelper.DetectAndExitEditMode()) // Exiting the edit mode doesn't work consistently. Once it is fixed then uncomment
                if (!ExcelHelper.GetEditMode())
                {
                    switch ((string)selectedButton.Tag.ToString())
                    {
                        case "OPEN_APPLICATION":
                            string AppId = string.Empty;
                            blnOpenApp = true;
                            ApplicationBrowserView view = new ApplicationBrowserView();
                            view.Text = Constants.DESIGNER_PRODUCT_NAME;
                            ApplicationBrowserController controller = new ApplicationBrowserController(view, commandBar.IsBasicEdition());
                            if (!string.IsNullOrEmpty(controller.selectedAppId))
                            {
                                selectedApplication = LoadApplication(controller.selectedAppId, string.Empty);
                                if (selectedApplication != null)
                                    configurationManager.Definition.Name = AppName = controller.selectedAppName;
                            }
                            break;
                        case "NEW_APPLICATION":
                            NewApplication newApp = new NewApplication();
                            newApp.ShowDialog();
                            setScreenToForeground();
                            break;
                        case "QUICK_APPLICATION":
                            LaunchQuickApp();
                            break;
                        //case "EDIT_APPTYPE":
                        //    LaunchEditAppType();
                            break;
                        default:
                            selectedApplication = LoadApplication(selectedButton.Tag.ToString(), string.Empty);
                            if (selectedApplication != null)
                                configurationManager.Definition.Name = AppName = selectedButton.Label;
                            break;
                    }

                    if (selectedApplication != null)
                    {
                        string filePath = Utils.GetTempFileName(configurationManager.Definition.UniqueId, AppName);
                        Utils.StreamToFile(selectedApplication.Config, filePath + Constants.XML);
                        string extension = Path.GetExtension(selectedApplication.TemplateName);
                        // Set extension in write file

                        Microsoft.Office.Interop.Excel.Workbook workbook = ExcelHelper.CheckIfFileOpened(AppName + extension);
                        if (workbook != null) // WB is open
                        {
                            ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("RIBBON_SwitchCon_ErrorMsg"), workbook.Name), resourceManager.GetResource("RIBBON_SwitchConCap_ErrorMsg"));
                            return;
                        }

                        Utils.StreamToXlsx(selectedApplication.Template, filePath + extension);

                        if (selectedApplication.Schema != null)
                        {
                            FileInfo fInfo = new FileInfo(filePath);
                            String dirName = fInfo.DirectoryName;
                            Utils.StreamToXlsx(selectedApplication.Schema, dirName + "\\" + Constants.EXTERNALSCHEMA + Constants.JSON);
                        }

                        // Add Current application instance
                        Utils.AddApplicationInstance(ApplicationMode.Designer, AppName + extension);

                        // Open template file with set extension
                        ApttusObjectManager.OpenFile(filePath + extension);

                        // Add Designer flag on load 
                        ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_DESIGNER_FILE, "true");                       
                        ApttusRibbon.ActivateTab();

                        ApttusCommandBarManager.g_IsAppOpen = true;
                        commandBar.EnableMenus();
                    }
                }
                else
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("CONST_MESSAGE_EDITMODE"), resourceManager.GetResource("RIBBON_EditModeCap_ErrorMsg"));
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
            try
            {
                LoadUserConnections();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void splitBtnActions_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ApttusAddModifyActions apttusActions = new ApttusAddModifyActions(String.Empty);
                apttusActions.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        // Call Search and Select Form
        private void btnSearchSelect_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                SearchAndSelectActionView apttusSearchForm = new SearchAndSelectActionView();
                apttusSearchForm.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnAppDef_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                AppDefinitionUtil util = new AppDefinitionUtil();
                util.DisplayAppDefnUI();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        private void btnForm_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ApttusAddModifyActions apttusActions = new ApttusAddModifyActions(btnForm.Tag.ToString());
                apttusActions.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnCallProcedure_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                CallProcedureModel model = new CallProcedureModel();
                CallProcedure view = new CallProcedure();
                CallProcedureController controller = new CallProcedureController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnExecuteQuery_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                QueryActionModel model = new QueryActionModel();
                QueryActionView view = new QueryActionView();
                QueryActionController controller = new QueryActionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnRetrieve_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                RetrieveActionView view = new RetrieveActionView();
                RetrieveActionModel model = new RetrieveActionModel();
                RetrieveActionController controller = new RetrieveActionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnSaveAction_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                SaveActionView view = new SaveActionView();
                SaveActionModel model = new SaveActionModel();
                SaveActionController controller = new SaveActionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnWorkflow_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Button b = new Button();
                b.Text = "Add Trigger";
                b.AutoSize = true;
                b.Visible = false; //Will have to cleanup this entire piece of Trigger module and move it to appsettings.

                GenericListUIView view = new GenericListUIView(new List<Button>() { b });
                List<Workflow> model = configurationManager.Workflows;
                GenericListUIController controller = new GenericListUIController(model.ToList<dynamic>(), view, ListScreenType.Actionflow);

                b.Tag = controller;
                b.Click += btnWorkflowTrigger_Click;

                view.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnWorkflowTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                Button b = sender as Button;

                if (b == null)
                    return;

                GenericListUIController listController = (GenericListUIController)b.Tag;

                WorkflowStructure model = (from DataGridViewRow r in listController.GetSelectedRows()
                                           select ((GenericListUIModel)r.DataBoundItem).ListItem).FirstOrDefault()
                                            as WorkflowStructure;

                if (model == null)
                    return;

                WorkflowTriggerView view = new WorkflowTriggerView();
                WorkflowTriggerController controller = new WorkflowTriggerController(model, view);
                view.Show();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnMenuDesigner_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                MenuUtilcs MnuUtil = new MenuUtilcs();
                MnuUtil.ShowMenuDesigner();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnNewApplication_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                NewApplication newApp = new NewApplication();
                newApp.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }


        private void btnRetrieveMap_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                GenericListUIView view = new GenericListUIView();
                List<RetrieveMap> model = configurationManager.RetrieveMaps;
                GenericListUIController controller = new GenericListUIController(model.ToList<dynamic>(), view, ListScreenType.DisplayMap);
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnSaveMap_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                GenericListUIView view = new GenericListUIView();
                List<SaveMap> model = configurationManager.SaveMaps;
                GenericListUIController controller = new GenericListUIController(model.ToList<dynamic>(), view, ListScreenType.SaveMap);
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void splitbtnSave_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                //Was this app desiged by quick app, if yes then reset the quick app model as this app is no longer editable from quick app edit mode.
                configurationManager.Application.QuickAppModel = null;

                if (SaveApplication())
                {
                    IntPtr handler = FindWindow(null, Globals.ThisAddIn.Application.Caption);
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RIBBON_splitbtnSave_Click_InfoMsg"), resourceManager.GetResource("RIBBON_splitbtnSave_ClickCAP_InfoMsg"), handler.ToInt32());
                }
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex, string.Empty, "App Save");
                IntPtr handler = FindWindow(null, Globals.ThisAddIn.Application.Caption);
                ApttusMessageUtil.ShowError(ex.Message.ToString(), Constants.DESIGNER_PRODUCT_NAME, handler.ToInt32());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ImportAppView view = new ImportAppView();
                ImportAppController controller = new ImportAppController(view);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                // 1. Save Application before Export
                if (SaveApplication())
                {
                    // 2. Fetch Export path
                    SaveFileDialog ExportDialog = new SaveFileDialog();
                    ExportDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    ExportDialog.Filter = "X-Author App|*.app";
                    ExportDialog.Title = resourceManager.GetResource("RIBBON_btnExport_ClickCAP_InfoMsg");
                    ExportDialog.FileName = configurationManager.Application.Definition.Name;

                    DialogResult dr = ExportDialog.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        // 3. Save a Copy of Template
                        string ActiveWorkbookName = Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
                        string ExportDirectory = Path.GetDirectoryName(ActiveWorkbookName) + Path.DirectorySeparatorChar + "Export_Directory";
                        string WorkbookCopy = ExportDirectory + Path.DirectorySeparatorChar + Path.GetFileName(ActiveWorkbookName);

                        Utils.CreateOrClearDirectory(ExportDirectory);
                        ApttusObjectManager.SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, WorkbookCopy);
                        ApttusObjectManager.SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, ActiveWorkbookName);

                        // 4. Save a Copy of Config
                        configurationManager.SaveLocal(ExportDirectory + Path.DirectorySeparatorChar + configurationManager.Application.Definition.Name + Constants.XML, true);
                        // 5 save schema file 
                        if (configurationManager.Application.Definition.Schema != null && configurationManager.Application.Definition.Schema.Length > 0)
                            System.IO.File.WriteAllBytes(ExportDirectory + Path.DirectorySeparatorChar + Constants.EXTERNALSCHEMA + Constants.JSON, configurationManager.Application.Definition.Schema);

                        // 6. Export App (Zip Files to .app file)
                        if (AppController.ExportApp(ExportDirectory, ExportDialog.FileName))
                            ApttusMessageUtil.ShowInfo(String.Format(resourceManager.GetResource("RIBBON_btnExport_Click_InfoMsg"), ExportDialog.FileName), resourceManager.GetResource("RIBBON_btnExport_ClickCAP_InfoMsg"));

                        // 7. Clear Export Directory
                        Utils.CreateOrClearDirectory(ExportDirectory);
                        Directory.Delete(ExportDirectory);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAs_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                //Was this app desiged by quick app, if yes then reset the quick app model as this app is no longer editable from quick app edit mode.
                configurationManager.Application.QuickAppModel = null;

                // 1. Save Application before Export                
                if (SaveApplication())
                {
                    CloneAppView view = new CloneAppView();
                    CloneAppController controller = new CloneAppController(view);
                }
# if DEBUG
                configurationManager.SaveLocal(string.Empty, false);
#endif
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnMacroAction_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                MacroActionView view = new MacroActionView();
                MacroActionModel model = new MacroActionModel();
                MacroActionController controller = new MacroActionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private bool SaveApplication()
        {
            bool result = false;

            try
            {
                if (ValidationManager.ValidateApp())
                {
                    string currentExtension = string.Empty;

                    // Check if a Macro exists in the Excel Template
                    bool IsMacroExists = ValidationManager.IsMacroExists(out currentExtension);
                    if (IsMacroExists)
                    {
                        DialogResult dr = MessageBox.Show(resourceManager.GetResource("RIBBON_MacroExists_ShowMsg"), Constants.PRODUCT_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr != DialogResult.Yes)
                            return result;
                    }

                    List<string> resultSheet = new List<string>();
                    // Check if a App Settings data aren't sync
                    bool blnProtectSheetValidation = ValidationManager.SyncSheetProtectSettingsData(out resultSheet);

                    if (blnProtectSheetValidation)
                    {
                        //ApttusMessageUtil.ShowError(string.Join(",", resultSheet.ToArray()) + " not provided password in App Settings.", Constants.DESIGNER_PRODUCT_NAME);
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_SheetProtect_ErrorMsg") + string.Join(",", resultSheet.ToArray()), resourceManager.GetResource("CONST_DESIGNER_PRODUCT_NAME"));
                        return result;
                    }

                    ApplicationHelper.GetInstance.SaveApplication(IsMacroExists, currentExtension);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return result;
        }

        private void btnSaveAttachmentAction_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                SaveAttachmentView view = new SaveAttachmentView();
                SaveAttachmentModel model = new SaveAttachmentModel();
                SaveAttachmentController controller = new SaveAttachmentController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnClearAction_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ClearActionView view = new ClearActionView();
                ClearActionModel model = new ClearActionModel();
                ClearActionController controller = new ClearActionController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnCheckIn_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                CheckInView view = new CheckInView();
                CheckInModel model = new CheckInModel();
                CheckInController controller = new CheckInController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnCheckOut_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                CheckOutView view = new CheckOutView();
                CheckOutModel model = new CheckOutModel();
                CheckOutController controller = new CheckOutController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void LaunchQuickApp()
        {
            try
            {
                QuickAppWizardView wizard = new QuickAppWizardView();
                wizard.ShowDialog();
                setScreenToForeground();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }
        //private void LaunchEditAppType()
        //{
        //    // first check if there is an app opened
        //    // if so, check the edition of the app
        //    // set the edition correctly in the UI.


        //    // Designer
        //    // Check if metadata sheet exists
        //    Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
        //    if (oSheet == null)
        //    {// no app is opened currently

        //        ApttusMessageUtil.ShowInfo(resourceManager.GetResource("EDIT_APPTYPE_WARNING_MSG"), resourceManager.GetResource("COMMON_STARTMENU_CHANGE_EDITION"));
        //        return;
        //    }
            
        //    // Get application Unique Id
        //    Excel.Range rng = oSheet.Cells[1, 1];
        //    string appUniqueId = oSheet.Cells[1, 1].Value2 as string;

        //    // Get Application instance
        //    //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Designer);
        //    ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, Globals.ThisAddIn.Application.ActiveWorkbook.Name, ApplicationMode.Designer);

        //    if (appInstance != null)
        //    {
        //        Apttus.XAuthor.AppDesigner.Controller.AppEditionType.AppTypeController AppTypeCtr =
        //            new Apttus.XAuthor.AppDesigner.Controller.AppEditionType.AppTypeController(appInstance, true);
        //        ApplicationTypeView Apptype = new ApplicationTypeView(AppTypeCtr);
        //        Apptype.ShowDialog();
        //     }

        //}
        private void btnDataLoader_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                DataTransferView view = new DataTransferView();
                DataTransferMapping model = new DataTransferMapping();
                DataTransferController controller = new DataTransferController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnUserInput_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                InputActionView view = new InputActionView();
                InputActionModel model = new InputActionModel();
                InputActionController controller = new InputActionController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnAppSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                AppSettingView view = new AppSettingView();
                AppSettings model = new AppSettings();
                AppSettingsController controller = new AppSettingsController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

        }

        private void btnMatrixMap_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                GenericListUIView view = new GenericListUIView();
                List<MatrixMap> matrixMaps = configurationManager.MatrixMaps;
                GenericListUIController controller = new GenericListUIController(matrixMaps.ToList<dynamic>(), view, ListScreenType.MatrixMap);
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnpasteSourceData_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                PasteSourceDataActionModel model;

                //We can only have 1 and only 1 Paste Action.
                model = configurationManager.Actions.OfType<PasteSourceDataActionModel>().FirstOrDefault();
                FormOpenMode mode = model == null ? FormOpenMode.Create : FormOpenMode.Edit;
                if (model == null)
                    model = new PasteSourceDataActionModel();
                PasteSourceDataActionController controller = new PasteSourceDataActionController(model, new PasteSourceDataActionView(), mode);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnSwitchConnection_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                SwitchConnectionActionView view = new SwitchConnectionActionView();
                SwitchConnectionActionModel model = new SwitchConnectionActionModel();
                SwitchConnectionController controller = new SwitchConnectionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnPreview_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Microsoft.Office.Core.COMAddIns comAddIns = Globals.ThisAddIn.Application.COMAddIns;
                Microsoft.Office.Core.COMAddIn runtimeAddIn = null;

                foreach (Microsoft.Office.Core.COMAddIn myAddin in comAddIns)
                {
                    if (myAddin.ProgId.Equals("Apttus.X-Author.Apps") || myAddin.ProgId.Equals("Apttus.XAuthor.AppRuntime"))
                    {
                        runtimeAddIn = myAddin;
                        break;
                    }
                }

                if (runtimeAddIn != null)
                {
                    string applicationURI = commandBar.GetApplicationURI(configurationManager.Definition.UniqueId);
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("/AppId=").Append(configurationManager.Definition.UniqueId).Append("|").Append("/ExportId=").Append(string.Empty)
                    //               .Append("|").Append("/InstanceURL=").Append(Globals.ThisAddIn.oAuthWrapper.token.instance_url).Append("|").Append("/SessionId=").Append(Globals.ThisAddIn.oAuthWrapper.token.access_token);

                    runtimeAddIn.Object.LoadApplicationWithParam(applicationURI);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnQuickAppNew_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                LaunchQuickApp();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
            }
        }

        private void btnQuickAppEdit_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Definition definition = configurationManager.Application.Definition;
                if (definition != null && configurationManager.Application.QuickAppModel != null)
                {
                    string xml = ApttusXmlSerializerUtil.Serialize<WizardModel>(configurationManager.Application.QuickAppModel);

                    QuickAppWizardView quickAppView = new QuickAppWizardView(configurationManager.Application.QuickAppModel, definition.Name, definition.UniqueId);
                    DialogResult result = quickAppView.ShowDialog();

                    if (result != DialogResult.OK)
                        configurationManager.Application.QuickAppModel = (WizardModel)ApttusXmlSerializerUtil.Deserialize<WizardModel>(xml);
                }
                else
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RIBBON_btnQuickAppEdit_Click_InfoMsg"), Constants.DESIGNER_PRODUCT_NAME);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnQuickAppSettings_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                commandBar.ShowOptions(true, false);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnAddRowAction_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                AddRowActionModel model = new AddRowActionModel();
                AddRowActionView view = new AddRowActionView();
                AddRowActionController controller = new AddRowActionController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }
        private void setScreenToForeground()
        {
            try
            {
                int appVersion = Convert.ToInt32(Globals.ThisAddIn.Application.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);

                if (appVersion >= 15)
                {
                    string sCaption = Globals.ThisAddIn.Application.Caption;
                    IntPtr handler = FindWindow(null, sCaption);

                    SetForegroundWindow(handler);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }


        private void btnPasteRow_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                PasteRowActionModel model = new PasteRowActionModel();
                PasteRowActionView view = new PasteRowActionView();
                PasteRowActionController controller = new PasteRowActionController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }



        private void btnValidate_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                DesignerAppSyncController controller = new DesignerAppSyncController();
                //AppSyncBackgroundWorker worker = new AppSyncBackgroundWorker(controller);
                AppSyncView appSync = new AppSyncView(controller);
                appSync.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnDataSet_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                DataSetActionModel model = new DataSetActionModel();
                DataSetActionView view = new DataSetActionView();
                DataSetActionController controller = new DataSetActionController(view, model, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnExternalLibrary_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                using (ExternalLibraryView view = new ExternalLibraryView())
                {
                    view.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }
        private void btnDelete_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                DeleteActionModel model = new DeleteActionModel();
                DeleteActionView view = new DeleteActionView();
                DeleteActionController controller = new DeleteActionController(model, view, FormOpenMode.Create);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }

        }

        private void btnGeneral_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                commandBar.ShowOptions(false, true);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// This method will activate tab which is passed as an argument.
        /// </summary>
        /// <param name="TabName">tab to activate.</param>
        public static void ActivateTab(bool isQuickQpp = false)
        {
            try
            {
                int appVersion = Convert.ToInt32(Globals.ThisAddIn.Application.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                if (appVersion >= 14)
                {
                    ThisRibbonCollection ribb = Globals.Ribbons;
                    if (ribb != null && ribb.ApttusRibbon != null && ribb.ApttusRibbon.ApplicationGroup != null && ribb.ApttusRibbon.ApplicationGroup.RibbonUI != null)
                        ribb.ApttusRibbon.ApplicationGroup.RibbonUI.ActivateTab("tabAppBuilderDesigner");
                }
                else if (appVersion == 12)  // Specific to Office 2007 only.
                {
                    if (isQuickQpp)
                        SendKeys.SendWait("%XAD%");
                    else
                        SendKeys.Send("%XAD%");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        private void btnNewDataMigrationApp_Click(object sender, RibbonControlEventArgs e)
        {
            DataMigrationWizard view = new DataMigrationWizard();
            DataMigrationModel model;
            DataMigrationController controller;

            model = new DataMigrationModel();
            controller = new DataMigrationController(model, view, FormOpenMode.Create);            
        }

        private void btnEditDataMirationApp_Click(object sender, RibbonControlEventArgs e)
        {
            DataMigrationWizard view = new DataMigrationWizard();
            DataMigrationModel model;
            DataMigrationController controller;

            if (configurationManager.Application != null && configurationManager.Application.MigrationModel != null)
            {
                model = configurationManager.Application.MigrationModel;
                DataMigrationModel clonedModel = model.Clone(); //Don't pass the original model.
                controller = new DataMigrationController(clonedModel, view, FormOpenMode.Edit);
            }
            else
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RIBBON_btnDataMigrationAppEdit_Click_InfoMsg1"), Constants.DESIGNER_PRODUCT_NAME);
            }
        }

        private void btnSetCRM_Click(object sender, RibbonControlEventArgs e)
        {
            CRMSwitcher switcher = new CRMSwitcher();
            switcher.ShowDialog();
        }

        private void btnCentralAdmin_Click(object sender, RibbonControlEventArgs e)
        {
            centralAdmin = CentralAdmin.Instance;
            centralAdmin.SetCurrentAddIn(ApplicationMode.Designer);
            centralAdmin.SetVersionNumber(ApttusCommandBarManager.GetInstance().GetVersion());
            centralAdmin.AppSettingsEventHandler += ExcelHelper.CheckAppSettings;
            centralAdmin.AppSettingsValidationEventHandler += ExcelHelper.ValidateSheetProtection;
            centralAdmin.IsAppFileOpen = !string.IsNullOrEmpty(ExcelHelper.GetAppUniqueId());
            centralAdmin.ShowDialog();
            if (centralAdmin.IsAutoUpdateClose)
            {
                foreach (Microsoft.Office.Interop.Excel.Workbook _wb in Globals.ThisAddIn.Application.Workbooks)
                {
                    if (!_wb.Saved)
                        _wb.Saved = true;
                }
                Globals.ThisAddIn.Application.Quit();
            }
        }
    }
}

