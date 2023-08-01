/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Microsoft.Office.Tools.Ribbon;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.AppRuntime;
using Apttus.ChatterWebControl.UserControls;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;


// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace Apttus.XAuthor.AppRuntime
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        string isRuntimeFile;

        private ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private BaseApplicationController AppController = null;
        public MenuBuilder Builder;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private UC_ChatterWeb ucChatterControl;
        private static ContextMenuHelper contextMenuInstance = ContextMenuHelper.GetInstance;

        public Ribbon()
        {

        }
        public MenuBuilder MenuBuilder
        {
            get;
            set;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Apttus.XAuthor.AppRuntime.Ribbon.xml");
        }

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region "Event handlers for Excel Events"

        public void ApttusSortDescending(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    Excel.Range Selection = Globals.ThisAddIn.Application.Selection;
                    SortActionController sortActionController = new AppRuntime.SortActionController(Selection);
                    sortActionController.HandleSort();
                    cancel = sortActionController.bCancelSort;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusSortAscending(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    Excel.Range Selection = Globals.ThisAddIn.Application.Selection;
                    SortActionController sortActionController = new AppRuntime.SortActionController(Selection);
                    sortActionController.HandleSort();
                    cancel = sortActionController.bCancelSort;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusSortCustom(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    Excel.Range Selection = Globals.ThisAddIn.Application.Selection;
                    SortActionController sortActionController = new AppRuntime.SortActionController(Selection);
                    sortActionController.HandleSort();
                    cancel = sortActionController.bCancelSort;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusSortDialog(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    Excel.Range Selection = Globals.ThisAddIn.Application.Selection;
                    SortActionController sortActionController = new AppRuntime.SortActionController(Selection);
                    sortActionController.HandleSort();
                    cancel = sortActionController.bCancelSort;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Restrict Send File at runtime
        /// </summary>
        /// <param name="control"></param>
        /// <param name="cancel"></param>
        public void ApttusFileSendAsAttachment(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (OfflineHelper.IsDisableRuntimeLocalSaveFile())
                    cancel = true;
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusCopy(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    cancel = false;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusPaste(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    cancel = false;
                }
                else
                    cancel = false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void ApttusSheetCellsInsert(Office.IRibbonControl control, ref bool cancel)
        {
            try
            {
                if (!ApttusObjectManager.IsActiveWorkbookRuntime())
                {
                    cancel = false;
                    return;
                }

                if (!contextMenuInstance.CanDisplayNativeInsertDeleteCommands(Globals.ThisAddIn.Application.Selection))
                {
                    ApttusMessageUtil.ShowInfo(string.Format(@resourceManager.GetResource("WBEVENTMAN_Application_SheetChange_InfoMsg"),resourceManager.CRMName), Constants.RUNTIME_PRODUCT_NAME);
                    cancel = true;
                }
                else
                {
                    cancel = false;
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        #endregion

        #region "Access Actions"

        public void btnConnect_Click(Office.IRibbonControl ctl)
        {
            try
            {
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

                // Get list of user entries 
                int userNames = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Count;

                // Get last login for Microsoft Office Application
                string lastLoginUser = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;

                // If no user entries are available, throw Manage Connections form.
                if (userNames == 0 || string.IsNullOrEmpty(lastLoginUser))
                {
                    commandBarManager.ShowManageConnection(this);
                }
                else
                {
                    commandBarManager.ShowLoginForm(this);
                }

                // Refresh connections, in case of first login connection list was not being populated.
                //LoadUserConnections();

                LoadAppBuilder();

                this.ribbon.Invalidate();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public string mnuSwitchConnection_GetContent(Office.IRibbonControl ctl)
        {

            StringBuilder sbConnections = new StringBuilder(@"<menu xmlns=""http://schemas.microsoft.com/office/2006/01/customui"" >");
            string manageConnectionTitle = resourceManager.GetResource("MANAGECONNECTION_Ribbon_Title");
            try
            {
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();
                int userNames = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Count;

                if (userNames == 0)
                {
                    sbConnections.Append(@"<button id=""MANAGE_CONNECTION"" label=""Manage Connection..."" onAction=""switchConnectionButton_Click""  imageMso=""DataOptionsMenu"" />");
                    sbConnections.Append(@"</menu>");
                    return sbConnections.ToString().Replace("Manage Connection...", manageConnectionTitle);
                }

                sbConnections.Append(@"<button id=""MANAGE_CONNECTION"" label=""Manage Connection..."" onAction=""switchConnectionButton_Click""  imageMso=""DataOptionsMenu"" />");

                if (userNames > 0)
                {
                    sbConnections.Append(@"<menuSeparator id=""menusep1"" />");

                    // Get the last login value
                    //string sLastLogin = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;

                    //for (int i = 0; i < userNames.Length; i++)
                    int i = 0;
                    foreach (string sConnectionName in Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Keys)
                    {
                        //CreateRibbonButton(userNames[i], userNames[i], (commandBar.IsLoggedIn() ? userNames[i] != sLastLogin : true), true, "_" + (i + 1), false, false);
                        String sConnection = @"<button id=""{0}"" label=""{1}"" imageMso=""{2}"" getEnabled=""GetConnectionEnabled"" onAction=""switchConnectionButton_Click"" />";
                        sConnection = String.Format(sConnection, "_" + (i + 1).ToString(), sConnectionName, "_" + (i + 1).ToString());
                        sbConnections.Append(sConnection);

                        i++;
                    }
                    sbConnections.Append(@"</menu>");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
            return sbConnections.ToString().Replace("Manage Connection...", manageConnectionTitle);
        }

        public void switchConnectionButton_Click(Office.IRibbonControl ctl)
        {
            try
            {
                string selectedButton = ctl.Id;
                Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.LoadApplicationSettings();

                switch (selectedButton)
                {
                    case "MANAGE_CONNECTION":
                        commandBarManager.ShowManageConnection(this);
                        break;

                    default:
                        int connIndex = int.Parse(ctl.Id.Replace("_", ""));
                        string uName = string.Empty;

                        // Get connection name from Ribbon button index
                        int i = 0;
                        foreach (string sConnectionName in Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Keys)
                        {
                            if (i == connIndex - 1)
                            {
                                uName = sConnectionName;
                                break;
                            }
                            i++;
                        }

                        //string[] userNames = ApttusRegistryManager.GetAvailableUserNames(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase);
                        string sHost = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections[uName].ServerHost;
                        string sLastLoginHint = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections[uName].LastLoginHint;

                        if (sHost.Length == 0 || sHost == string.Empty)
                        {
                            ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_InvalConnetion_ErrorMsg"), resourceManager.GetResource("RIBBON_InvalConnetionCap_ErrorMsg"));
                            //MessageBox.Show("Invalid connection URL, please provide valid URL and try again.", "Switch Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        commandBarManager.SwitchConnection(uName);

                        break;
                }

                //LoadUserConnections();

                LoadAppBuilder();
                //This code has been placed here because switch connection does not enables App Menu -> AB-3075
                ribbon.Invalidate();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
                //RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Switch Connection");
            }
        }

        public void btnAbout_Click(Office.IRibbonControl ctl)
        {
            commandBarManager.DoAboutBox();
        }

        public void btnCRMSwitcher_Click(Office.IRibbonControl ctl)
        {
            CRMSwitcher crmSwitcher = new CRMSwitcher();
            crmSwitcher.ShowDialog();
        }

        public void AccessGroup_DialogLauncherClick(Office.IRibbonControl ctl)
        {
            try
            {
                commandBarManager.ShowOptions();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        #endregion

        #region "Edit & Social Actions"

        public void btnAddRow_Click(Office.IRibbonControl ctl)
        {
            bool IsMatrixRowEnabled = true;
            AddRows(1, IsMatrixRowEnabled);
        }

        public void Add3Rows_Click(Office.IRibbonControl ctl)
        {
            AddRows(3);
        }

        public void Add5Rows_Click(Office.IRibbonControl ctl)
        {
            AddRows(5);
        }

        public void Add10Rows_Click(Office.IRibbonControl ctl)
        {
            AddRows(10);
        }

        public void AddCustomRows_Click(Office.IRibbonControl ctl)
        {
            AddCustomRowsView View = new AddCustomRowsView();
            AddCustomRowsController Controller = new AddCustomRowsController(View);
            int RowsToAdd = Controller.GetCustomRows();
            if (RowsToAdd > 0)
                AddRows(RowsToAdd);
        }

        public void AddRows(int RowsToAdd, bool IsMatrixRowEnabled = false)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                if (!ExcelHelper.GetEditMode())
                {
                    Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                    Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
                    if (NamedRange != null)
                    {
                        string result = configurationManager.GetMapbyNamedRange(NamedRange.Name.Name);
                        if (result == Constants.DISPLAYMAP_NAME)
                        {
                            RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(Selection, NamedRange);
                            runtimeEditActionController.AddRow(RowsToAdd);
                        }
                        else if (result == Constants.MATRIXMAP_NAME)
                        {
                            if (IsMatrixRowEnabled)
                            {
                                RuntimeMatrixEditActionController runtimeMatrixEditActionController = new AppRuntime.RuntimeMatrixEditActionController(Selection, NamedRange);
                                runtimeMatrixEditActionController.AddRow(1);
                            }
                        }
                    }
                }
                else
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("CONST_MESSAGE_EDITMODE"), resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Add Row");
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        public void PasteValuesOnly_Click(Office.IRibbonControl ctl)
        {
            Paste(Excel.XlPasteType.xlPasteValues);

        }

        public void PasteAll_Click(Office.IRibbonControl ctl)
        {
            Paste(Excel.XlPasteType.xlPasteAll);
        }

        private void Paste(Excel.XlPasteType pasteType)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                Globals.ThisAddIn.Application.EnableEvents = false;

                if (!ExcelHelper.GetEditMode())
                {

                    // Declares an IDataObject to hold the data returned from the clipboard. 
                    // Retrieves the data from the clipboard.
                    IDataObject iData = Clipboard.GetDataObject();

                    // Determines whether the data is in a RTF format - Excel data is copied in CSV and RTF formats.
                    // Reason for getting data as RTF is so actual values with comma don't split up (as in case of CSV)
                    if (iData.GetDataPresent(DataFormats.CommaSeparatedValue) && iData.GetDataPresent(DataFormats.Rtf))
                    {
                        Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                        Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
                        if (NamedRange != null)
                        {
                            RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(Selection, NamedRange);
                            runtimeEditActionController.Paste(iData, pasteType);
                        }
                        else
                            ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_PasteData_ErrorMsg"), resourceManager.GetResource("RIBBON_PasteDataCap_ErrorMsg"));
                    }
                    else
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMONRUNTIME_NoDataCopied_ErrorMsg"), resourceManager.GetResource("RIBBON_PasteDataCap_ErrorMsg"));
                }
                else
                    ApttusMessageUtil.ShowError(Constants.MESSAGE_EDITMODE, resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Paste Row");
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        public void PasteWithMapping_Click(Office.IRibbonControl ctl)
        {
            try
            {
                new PasteHelper().StartPasteMapping();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void btnRemoveRow_Click(Office.IRibbonControl ctl)
        {
            RemoveRows(1);
        }

        public void RemoveRows(int RowsToRemove)
        {
            Globals.ThisAddIn.Application.ScreenUpdating = false;
            try
            {
                if (!ExcelHelper.GetEditMode())
                {

                    Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                    Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
                    if (NamedRange != null)
                    {
                        RuntimeEditActionController runtimeEditActionController = new AppRuntime.RuntimeEditActionController(Selection, NamedRange);
                        runtimeEditActionController.RemoveRow(RowsToRemove);
                    }
                }
                else
                    ApttusMessageUtil.ShowError(Constants.MESSAGE_EDITMODE, resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        public void btnChatterFeed_Click(Office.IRibbonControl ctl, bool flip)
        {

            try
            {
                string id = ctl.Id;
                ////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);
                if (flip)
                {
                    //Globals.ThisAddIn.oAuthWrapper.ApplicationInfo.ApplicationType.ToString() ==> 2
                    ApttusObjectManager.ShowBusyCursor("Retrieving Chatter Feeds...");
                    ucChatterControl = new UC_ChatterWeb(new ChatterWebAdapterExcel(),
                            commandBarManager.GetFrontDoorURL(),
                            "/apex/" + "Apttus_Collab__ChatterHtmlFeed" + "?app=",
                            "ChatterForExcel");

                    // Assign proxy object for the Manager class 
                    ////if (ApttusSessionManager.SalesForceSession.Proxy != null)
                    ////    ucChatterControl.Manager.Proxy = ApttusSessionManager.SalesForceSession.Proxy;

                    ////if (ApttusObjectManager.GetDocumentType() == "File")
                    ucChatterControl.Manager.InitPage();
                    ////else
                    //// ucChatterControl.Manager.InitPage(ApttusObjectManager.GetObjectId());

                    Microsoft.Office.Tools.CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes.Add(ucChatterControl, "Chatter", Globals.ThisAddIn.Application.ActiveWindow);
                    ctp.Control.Tag = commandBarManager.GetActiveWorkbookFullName();
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
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Chatter Feed");
            }
        }

        void ctp_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Tools.CustomTaskPane taskPane = sender as Microsoft.Office.Tools.CustomTaskPane;

                if (taskPane != null)
                {
                    //bool bShowTaskPane = Apttus.Common.Utilities.IsFileMenuClicked(new IntPtr(Globals.ThisAddIn.Application.Hwnd));
                    //btnChatterFeed.Checked = taskPane.Visible; //|| bShowTaskPane;

                    if (taskPane.Visible == false) //&& bShowTaskPane == false)
                    {
                        Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
                        taskPane.Dispose();
                        taskPane = null;
                    }
                }

                this.ribbon.Invalidate();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void btnAddMatrixColumn_Click(Office.IRibbonControl ctl)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                if (!ExcelHelper.GetEditMode())
                {
                    Excel.Range Selection = (Excel.Range)Globals.ThisAddIn.Application.Selection;
                    Excel.Range NamedRange = ExcelHelper.GetNamedRangeFromCell(Selection);
                    if (NamedRange != null)
                    {
                        RuntimeMatrixEditActionController runtimeMatrixEditActionController = new AppRuntime.RuntimeMatrixEditActionController(Selection, NamedRange);
                        runtimeMatrixEditActionController.AddColumn(1);
                    }
                }
                else
                    ApttusMessageUtil.ShowError(Constants.MESSAGE_EDITMODE, resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Add Matrix Row");
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        #endregion

        #region "App Actions"

        public void btnSettings_Click(Office.IRibbonControl ctl)
        {
            CentralAdmin centralAdmin = CentralAdmin.Instance;
            centralAdmin.SetCurrentAddIn(ApplicationMode.Runtime);
            centralAdmin.SetVersionNumber(ApttusCommandBarManager.GetInstance().GetVersion());
            centralAdmin.IsAppFileOpen = !string.IsNullOrEmpty(MetadataManager.GetInstance.GetAppUniqueId());
            centralAdmin.ShowDialog();
            if (centralAdmin.IsAutoUpdateClose)
            {
                foreach (Excel.Workbook _wb in Globals.ThisAddIn.Application.Workbooks)
                {
                    if (!_wb.Saved)
                        _wb.Saved = true;
                }
                Globals.ThisAddIn.Application.Quit();
            }
        }
        public string mnuApplication_GetContent(Office.IRibbonControl ctl)
        {
            try
            {
                StringBuilder sbApplications = new StringBuilder(@"<menu xmlns=""http://schemas.microsoft.com/office/2006/01/customui"" >");

                // Use Config manager method to retrieve available applications and load Ribbon Menu
                List<ApttusObject> lstApps = Globals.ThisAddIn.objectManager.GetAppList(9, null, null);
                string label = resourceManager.GetResource("COMMON_STARTMENU_OPENAPP");
                if (lstApps == null || lstApps.Count == 0)
                {
                    sbApplications.Append(@"<button id=""OPEN_APPLICATION"" label=""Open App"" onAction=""switchApplicationButton_Click""  imageMso=""DataOptionsMenu"" />");
                    sbApplications.Append(@"</menu>");
                    return sbApplications.ToString().Replace("Open App", label);
                }

                sbApplications.Append(@"<button id=""OPEN_APPLICATION"" label=""Open App"" onAction=""switchApplicationButton_Click""  imageMso=""DataOptionsMenu"" />");

                if (lstApps.Count > 0)
                {
                    sbApplications.Append(@"<menuSeparator id=""menusep1"" />");

                    // Load Application name through a method
                    for (int i = 0; i < lstApps.Count; i++)
                    {
                        String sApp = @"<button id=""{0}"" label=""{1}"" imageMso=""{2}"" onAction=""switchApplicationButton_Click"" />";
                        sApp = String.Format(sApp, "_" + lstApps[i].Id, System.Security.SecurityElement.Escape(lstApps[i].Name), "_" + (i + 1).ToString());
                        sbApplications.Append(sApp);
                    }
                    sbApplications.Append(@"</menu>");
                }
                return sbApplications.ToString().Replace("Open App", label);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Session"))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExp_InfoMsg"), Constants.RUNTIME_PRODUCT_NAME);
                    commandBarManager.DoLogout(false);
                }
                RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, "Application - Get Content");
            }
            return string.Empty;
        }

        public void switchApplicationButton_Click(Office.IRibbonControl ctl)
        {
            bool blnOpenApp = false;
            try
            {
                string selectedButton = ctl.Id;
                string selectedAppId = string.Empty;

                if (selectedButton.Equals("OPEN_APPLICATION"))
                {
                    blnOpenApp = true;
                    ApplicationBrowserView view = new ApplicationBrowserView();
                    view.Text = Constants.RUNTIME_PRODUCT_NAME;
                    ApplicationBrowserController controller = new ApplicationBrowserController(view, commandBarManager.IsBasicEdition());
                    if (string.IsNullOrEmpty(controller.selectedAppId))
                        return;
                    else
                        selectedAppId = controller.selectedAppId;
                }
                else
                    selectedAppId = ctl.Id.Remove(0, 1);

                //WaitWindowResult result = WaitWindow.Show(this.LoadApplicationWrapper, "Processing...", selectedAppId);

                //if (ExcelHelper.DetectAndExitEditMode()) // Exiting the edit mode doesn't work consistently. Once it is fixed then uncomment
                if (!ExcelHelper.GetEditMode())
                {
                    //blnOpenApp = true;
                    ApplicationObject selectedApplication = LoadApplication(selectedAppId, String.Empty);
                    if (selectedApplication != null)
                    {
                        blnOpenApp = commandBarManager.OpenTemplate(selectedApplication);
                        if (blnOpenApp)
                        {
                            commandBarManager.DoPostLoad(this, false);

                            // Add Runtime file property
                            ApttusObjectManager.SetDocProperty(Constants.APPBUILDER_RUNTIME_FILE, "true");
                            ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_DESIGNER_FILE);

                            // Build menus from here when App is being launched from menu or Open App UI
                            if (MenuBuilder == null)
                                MenuBuilder = new MenuBuilder();
                            ActivateTab("tabAppBuilderRuntime");

                            MenuBuilder.Config = configurationManager;
                            MenuBuilder.InvalidateMenus(this, true);
                            MenuBuilder.BuildMenusFromXml(this);
                        }
                    }
                    else
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_NotLoad_ErrorMsg"), resourceManager.GetResource("RIBBON_NotLoadCap_ErrorMsg"));
                }
                else
                    ApttusMessageUtil.ShowError(Constants.MESSAGE_EDITMODE, resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The password you supplied is not correct."))
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_NotProtectSheet_ErrorMsg"), Constants.RUNTIME_PRODUCT_NAME);
                else if (ex.Message.Contains("Session"))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExp_InfoMsg"), Constants.RUNTIME_PRODUCT_NAME);
                    commandBarManager.DoLogout(false);
                }
                else if (blnOpenApp)
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("RIBBON_LoadApp_ErrorMsg"), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version), resourceManager.GetResource("RIBBON_LoadAppCap_ErrorMsg"));

                RuntimeExceptionLogHelper.ErrorLog(ex, false, ex.Message, "Switch App");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appUniqueId"></param>
        /// <returns></returns>
        private ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            ApplicationObject selectedApplication = null;
            AppController = Globals.ThisAddIn.GetApplicationController();
            if (!(string.IsNullOrEmpty(appId) & string.IsNullOrEmpty(appUniqueId)))
                selectedApplication = AppController.LoadApplication(appId, appUniqueId);

            return selectedApplication;
        }

        private void LoadAppBuilder()
        {
            if (commandBarManager.IsLoggedIn() && ApttusObjectManager.IsActiveWorkbookRuntime())
            {
                // Load any application if the template is open
                string currentAppUniqueId = LoadApplicationFromCurrentFile();
                if (!string.IsNullOrEmpty(currentAppUniqueId))
                {
                    LoadApplication(string.Empty, currentAppUniqueId);
                    commandBarManager.DoPostLoad(this, true);
                }
            }
        }

        private string LoadApplicationFromCurrentFile()
        {
            MetadataManager metadataManager = MetadataManager.GetInstance;
            return metadataManager.GetAppUniqueId();
        }

        /// <summary>
        /// This method will activate tab which is passed as an argument.
        /// </summary>
        /// <param name="TabName">tab to activate.</param>
        public void ActivateTab(string TabName)
        {
            int appVersion = Convert.ToInt32(Globals.ThisAddIn.Application.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);
            if (appVersion >= 14 && !string.IsNullOrEmpty(TabName))
            {
                this.ribbon.ActivateTab(TabName);
            }
            else if (appVersion == 12)  // Specific to Office 2007 only.
            {
                SendKeys.Send("%XAE%");
            }
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, select the Ribbon XML item in Solution Explorer and then press F1

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;


        }
        public Office.IRibbonUI RibbonUIXml
        {
            get { return this.ribbon; }
        }

        public bool GetLoginEnabled(Office.IRibbonControl control)
        {
            return commandBarManager.IsLoggedIn();
        }

        public bool GetLoginDisabled(Office.IRibbonControl control)
        {
            return !commandBarManager.IsLoggedIn();
        }

        public bool GetCRMSwitcherDisabled(Office.IRibbonControl control)
        {
            return !commandBarManager.IsLoggedIn();
        }

        public Bitmap GetImage(Office.IRibbonControl control)
        {
            if (MenuBuilder != null)
                return MenuBuilder.onGetImage(control.Id);

            return null;
        }

        public string onGetLabel(Office.IRibbonControl control)
        {
            if (MenuBuilder != null)
                return MenuBuilder.onGetLabel(control.Id);

            return "";
        }


        /// <summary>
        /// Get Ribbon labels from resource files
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public string OnGetLabelResource(Office.IRibbonControl control)
        {
            if (control.Id.Equals("AccessGroup"))
                return resourceManager.GetResource("RIBBON_AccessGroup_Text");
            else if (control.Id.Equals("btnConnect"))
                return resourceManager.GetResource("CONNENTRY_btnConnect_Text");
            else if (control.Id.Equals("mnuSwitchConnection"))
                return resourceManager.GetResource("RIBBON_InvalConnetionCap_ErrorMsg");
            else if (control.Id.Equals("btnGallerySupport"))
                return resourceManager.GetResource("RIBBON_btnGallerySupport_Text").PadRight(17);
            else if (control.Id.Equals("btnAbout"))
                return resourceManager.GetResource("RIBBON_btnAbout_Text");
            else if (control.Id.Equals("btnGeneral"))
                return resourceManager.GetResource("RIBBON_btnGeneral_Text");
            else if (control.Id.Equals("SocialGroup"))
                return resourceManager.GetResource("RIBBON_SocialGroup_Text");
            else if (control.Id.Equals("btnChatterFeed"))
                return resourceManager.GetResource("OPTIONSFORM_tabChatter_Text");
            else if (control.Id.Equals("ApplicationGroup"))
                return resourceManager.GetResource("RIBBON_ApplicationGroup_Text");
            else if (control.Id.Equals("mnuApplication"))
                return resourceManager.GetResource("RIBBON_mnuApplication_Text").PadRight(9);
            else if (control.Id.Equals("EditGroup"))
                return resourceManager.GetResource("RIBBON_EditGroup_Text");
            else if (control.Id.Equals("btnAddRow__btn"))
                return resourceManager.GetResource("RIBBON_btnAddRow_Text");
            else if (control.Id.Equals("Add3Rows"))
                return resourceManager.GetResource("RIBBON_Add3Rows_Text");
            else if (control.Id.Equals("Add5Rows"))
                return resourceManager.GetResource("RIBBON_Add5Rows_Text");
            else if (control.Id.Equals("Add10Rows"))
                return resourceManager.GetResource("RIBBON_Add10Rows_Text");
            else if (control.Id.Equals("AddCustomRows"))
                return resourceManager.GetResource("RIBBON_AddCustomRows_Text");
            else if (control.Id.Equals("btnAddMatrixColumn"))
                return resourceManager.GetResource("RIBBON_btnAddMatrixColumn_Text");
            else if (control.Id.Equals("mnuPaste"))
                return resourceManager.GetResource("RIBBON_mnuPaste_Text");
            else if (control.Id.Equals("btnPasteValuesOnly"))
                return resourceManager.GetResource("RIBBON_btnPasteValuesOnly_Text");
            else if (control.Id.Equals("btnPasteAll"))
                return resourceManager.GetResource("RIBBON_btnPasteAll_Text");
            else if (control.Id.Equals("btnPasteWithMapping"))
                return resourceManager.GetResource("RIBBON_btnPasteWithMapping_Text");
            else if (control.Id.Equals("btnRemoveRow"))
                return string.Format(resourceManager.GetResource("RIBBON_btnRemoveRow_Text"),resourceManager.CRMName);
            else if (control.Id.Equals("btnCRMSwitcher"))
                return resourceManager.GetResource("RIBBON_ChangeCRM_Text");
            else if (control.Id.Equals("btnSettings"))
                return resourceManager.GetResource("COMMON_Settings_Text");


            return "";
        }

        public string OnSreenTip(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.OnSreenTip(control.Id);

            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return "";
        }

        public bool GetVisible(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.Visible(control.Id);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public bool GetEnabled(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.Enable(control.Id);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public void OnMenuclick(Office.IRibbonControl ctrl)
        {
            try
            {
                if (MenuBuilder != null)
                    MenuBuilder.onClick(ctrl.Id);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public bool GetEditVisible(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.EditVisible();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public bool GetAddRowEnabled(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.AddRowEnabled();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public bool GetPasteEnabled(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.PasteEnabled();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return false;
        }

        public bool GetPasteWithMappingEnabled(Office.IRibbonControl control)
        {
            try
            {
                return LicenseActivationManager.GetInstance.IsFeatureAvailable("PasteFromMapping");
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return false;
        }

        public bool GetDelRowEnabled(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.DelRowEnabled();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public bool GetSocialVisible(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.SocialVisible();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        public Bitmap GetSocialImage(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.SocialImage();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return null;
        }

        public Bitmap GetAboutImage(Office.IRibbonControl control)
        {
            return Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon.ToBitmap();
        }

        public bool GetSocialPressed(Office.IRibbonControl control)
        {
            try
            {
                Microsoft.Office.Tools.CustomTaskPane ctp = null;
                ctp = TaskPaneHelper.getTaskPaneInstance("Chatter");
                if (ctp != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return false;
        }

        public bool GetConnectionEnabled(Office.IRibbonControl control)
        {
            try
            {
                //string sLastLogin = ApttusRegistryManager.GetKeyValue(Microsoft.Win32.RegistryHive.CurrentUser, ApttusGlobals.ApttusRegistryBase, Apttus.OAuthLoginControl.Modules.ApttusGlobals.Application.ChatterForExcel.ToString());
                string sLastLogin = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.LastUsedConnection;

                int connIndex = int.Parse(control.Id.Replace("_", ""));
                string[] userNames = Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppLogin.Connections.Keys.ToArray();

                if (commandBarManager.IsLoggedIn())
                    return userNames[connIndex - 1] != sLastLogin;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return true;
        }

        public bool GetAddMatrixColumnEnabled(Office.IRibbonControl control)
        {
            try
            {
                if (MenuBuilder != null)
                    return MenuBuilder.AddMatrixColumnEnabled();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }

            return false;
        }

        #endregion


    }
}
