/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Modules;
using System.Runtime.InteropServices;

namespace Apttus.XAuthor.AppRuntime
{
    public class WorkbookEventManager
    {
        private static WorkbookEventManager instance;
        private static object syncRoot = new Object();
        private ApttusCommandBarManager commandBarManager = ApttusCommandBarManager.GetInstance();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ContextMenuHelper contextMenuInstance = ContextMenuHelper.GetInstance;
        private Excel.XlCalculation xlCalculation = Excel.XlCalculation.xlCalculationAutomatic;
        public bool bProtectedToRegularMode = false;
        private string oldFileNameBeforeSave = string.Empty;
        private WorkbookEventManager()
        {
        }
        public static WorkbookEventManager GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new WorkbookEventManager();
                    }
                }
                return instance;
            }
        }

        #region "Public Methods"

        /// <summary>
        /// 
        /// </summary>
        internal void SubscribeEvents()
        {
            try
            {
                // In case of multiple excel.exe, UnsubscribeEvents() events from previous events
                UnsubscribeEvents();

                ExcelHelper.ExcelApp.WorkbookBeforeClose += Application_WorkbookBeforeClose;
                ExcelHelper.ExcelApp.WorkbookOpen += Application_WorkbookOpen;
                ExcelHelper.ExcelApp.WorkbookActivate += Application_WorkbookActivate;
                ExcelHelper.ExcelApp.SheetChange += Application_SheetChange;
                ExcelHelper.ExcelApp.WorkbookBeforePrint += ExcelApp_WorkbookBeforePrint;
                ExcelHelper.ExcelApp.SheetSelectionChange += ExcelApp_SheetSelectionChange;
                ExcelHelper.ExcelApp.SheetBeforeRightClick += ExcelApp_SheetBeforeRightClick;
                ExcelHelper.ExcelApp.ProtectedViewWindowOpen += Application_ProtectedViewWindowOpen;
                ExcelHelper.ExcelApp.WorkbookAfterSave += Application_WorkbookAfterSave;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }
        /// <summary>
        /// Added in resolution of AB-3197
        /// </summary>
        /// <param name="Wb"></param>
        /// <param name="Success"></param>
        private void Application_WorkbookAfterSave(Excel.Workbook Wb, bool Success)
        {
            if (Success && !string.IsNullOrEmpty(oldFileNameBeforeSave))
            {
                string appUniqueId = MetadataManager.GetInstance.GetAppUniqueId();
                ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, oldFileNameBeforeSave, ApplicationMode.Runtime);
                oldFileNameBeforeSave = string.Empty;
                if (appInstance != null)
                    appInstance.AppFileName = ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name;
            }
            //To handle the case where file save failed but before save event populated olfFileNameBeforeSave param
            else oldFileNameBeforeSave = string.Empty;
        }

        private void ExcelApp_SheetBeforeRightClick(object Sh, Excel.Range Target, ref bool Cancel)
        {
            try
            {
                // Get runtime flag
                contextMenuInstance.ResetContextMenus();

                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
                if (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true"))
                {
                    if (contextMenuInstance.CanDisplayXAuthorMenu(Target))
                        contextMenuInstance.DisplayXAuthorMenu();

                    if (!contextMenuInstance.CanDisplayNativeInsertDeleteCommands(Target))
                        contextMenuInstance.RemoveNativeInsertDelete();
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UnsubscribeEvents()
        {
            try
            {
                ExcelHelper.ExcelApp.WorkbookBeforeClose -= Application_WorkbookBeforeClose;
                ExcelHelper.ExcelApp.WorkbookOpen -= Application_WorkbookOpen;
                ExcelHelper.ExcelApp.WorkbookActivate -= Application_WorkbookActivate;
                ExcelHelper.ExcelApp.SheetChange -= Application_SheetChange;
                ExcelHelper.ExcelApp.WorkbookBeforePrint -= ExcelApp_WorkbookBeforePrint;
                ExcelHelper.ExcelApp.SheetSelectionChange -= ExcelApp_SheetSelectionChange;
                ExcelHelper.ExcelApp.SheetBeforeRightClick -= ExcelApp_SheetBeforeRightClick;
                ExcelHelper.ExcelApp.ProtectedViewWindowOpen -= Application_ProtectedViewWindowOpen;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        internal void DisableExcelEventsRenderingCalculation()
        {
            ExcelHelper.ExcelApp.ScreenUpdating = false;
            ExcelHelper.ExcelApp.EnableEvents = false;
            this.xlCalculation = ExcelHelper.ExcelApp.Calculation;
            ExcelHelper.ExcelApp.Calculation = Excel.XlCalculation.xlCalculationManual;
        }

        internal void EnableExcelEventsRenderingCalculation()
        {
            ExcelHelper.ExcelApp.ScreenUpdating = true;
            ExcelHelper.ExcelApp.EnableEvents = true;
            ExcelHelper.ExcelApp.Calculation = this.xlCalculation;
        }

        internal void SubScribeBeforeSave()
        {
            ExcelHelper.ExcelApp.WorkbookBeforeSave += Application_WorkbookBeforeSave;
        }

        internal void UnSubScribeBeforeSave()
        {
            ExcelHelper.ExcelApp.WorkbookBeforeSave -= Application_WorkbookBeforeSave;
        }

        internal void SubScribeDoubleClick()
        {
            ExcelHelper.ExcelApp.SheetBeforeDoubleClick += ExcelApp_SheetBeforeDoubleClick;
        }

        internal void UnSubScribeDoubleClick()
        {
            ExcelHelper.ExcelApp.SheetBeforeDoubleClick -= ExcelApp_SheetBeforeDoubleClick;
        }

        void ExcelApp_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            try
            {
                if (Target.Cells.CountLarge > 1 || ConfigurationManager.GetInstance.IsRichTextEditingDisabled)
                    return;

                Excel.Range namedRange = ExcelHelper.GetNamedRangeFromCell(Target);
                if (namedRange != null)
                {
                    RichTextDataManager richTextManager = RichTextDataManager.Instance;
                    string colName;
                    if (richTextManager.IsSelectionRichTextField(namedRange, Target, out colName) && !String.IsNullOrEmpty(colName))
                    {
                        try
                        {
                            string[] values = (Target.Formula as string).Split(new char[] { '\"' });
                            if (values != null && values.Count() > 2 && !String.IsNullOrEmpty(values[1]))
                            {
                                string id = values[1];
                                richTextManager.LoadRichTextData(id, (namedRange.Name as Excel.Name).Name, colName);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogHelper.ErrorLog(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }
        #endregion

        #region "Workbook Event Handlers"
        void ExcelApp_WorkbookBeforePrint(Excel.Workbook Wb, ref bool Cancel)
        {
            try
            {
                if (!OfflineHelper.IsDisableRuntimePrint())
                { }
                else
                    Cancel = true;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        public void Application_WorkbookBeforeSave(Excel.Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            try
            {
                if (!OfflineHelper.IsDisableRuntimeLocalSaveFile())
                {
                    OfflineHelper.SerializeData();
                    // Added to support app switching between the SAME APP with DIFFERENT file name - AB-3197
                    // oldFileNameAfterSave will be used in Application_WorkbookAfterSave event
                    if (SaveAsUI)
                        oldFileNameBeforeSave = ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name;
                    //Remove temp files after serialization
                    Utils.DeleteTempFiles();
                    Utils.ClearTempFiles();
                }
                else
                {
                    Cancel = true;
                }
                //Wb.Saved = true;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        void Application_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            try
            {
                contextMenuInstance.ResetContextMenus();

                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                // Remove Application instance on workbook close
                if (ConfigurationManager.GetInstance.Application != null && (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true")))
                {
                    if (ObjectManager.RuntimeMode == RuntimeMode.AddIn && Globals.ThisAddIn != null)
                    {
                        if (!Globals.ThisAddIn.Application.ActiveWorkbook.Saved)
                        {
                            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes;
                            if (!commandBarManager.IsUserClosingRuntimeApp())
                                result = ApttusMessageUtil.ShowWarning(resourceManager.GetResource("OPTIONS_Options_ShowMsg"), Constants.PRODUCT_NAME, ApttusMessageUtil.YesNo);

                            if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
                            {
                                Globals.ThisAddIn.Application.ActiveWorkbook.Saved = true;
                                //ApplicationManager.GetInstance.Remove(ConfigurationManager.GetInstance.Application.Definition.UniqueId, ApplicationMode.Runtime);

                                if (Globals.ThisAddIn.RuntimeRibbon.MenuBuilder != null)
                                    Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                                Globals.ThisAddIn.AppVersion = "";
                            }
                            else if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No)
                            {
                                Cancel = true;
                            }
                        }

                        //ApplicationManager.GetInstance.Remove(ConfigurationManager.GetInstance.Application.Definition.UniqueId, ApplicationMode.Runtime);
                        Utils.RemoveApplicationInstance(ConfigurationManager.GetInstance.Application.Definition.UniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);
                        // Globals.ThisAddIn.Application.StatusBar = "";
                    }

                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                Marshal.ReleaseComObject(Wb);
            }
        }

        public void Application_WorkbookOpen(Excel.Workbook Wb)
        {
            try {
                // if sheet is not in protected mode, i.e. user didn't get Enable Editing on Excel info bar
                if(!bProtectedToRegularMode) {
                    // Remove attachment property added while adding it to notes and attachment section on workbook open
                    ApttusObjectManager.RemoveDocProperty(Constants.APPBUILDER_RUNTIME_ATTACHMENTFILE, Wb);

                    OfflineHelper.DeSerializeData();
                    if(ObjectManager.RuntimeMode == RuntimeMode.AddIn && Globals.ThisAddIn != null) {
                        string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                        if(!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true")) 
                            OfflineHelper.BuildMenus(Globals.ThisAddIn.RuntimeRibbon);
                        else if(string.IsNullOrEmpty(isRuntimeFile) && Globals.ThisAddIn.RuntimeRibbon.MenuBuilder != null) {
                            Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                            Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateRibbon(Globals.ThisAddIn.RuntimeRibbon);
                        }
                    }
                    // Add Current application instance
                    // Moved below statement to Deserialize data as this is specific to Offline only, bisOffline is sent as true
                    // Add application instance only if it is valid X-Author runtime file, with Offline data.
                    //Utils.AddApplicationInstance(ApplicationMode.Runtime, Wb.Name, true);
                }
            } 
            catch(Exception ex) {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            } 
        }

        private void Application_ProtectedViewWindowOpen(Excel.ProtectedViewWindow Pvw)
        {
            try
            {
                Excel.Workbook Wb = Pvw.Workbook;

                Excel.ProtectedViewWindow protectViewWindow = Globals.ThisAddIn.Application.ActiveProtectedViewWindow;
                if (protectViewWindow != null)
                {
                    // If metadata sheet is present consider this as X-Author workbook and EnableEditing programatically
                    if (ExcelHelper.GetWorkSheet(Wb, Constants.METADATA_SHEETNAME) != null)
                    {
                        bool bDisplayWorkbookTabs = true;
                        bool bDisplayVerticalScrollBar = true;
                        bool bDisplayHorizontalScrollBar = true;

                        // Adding null check on safe side.
                        Excel.Application protectViewWindowApplication = protectViewWindow.Workbook.Application;
                        if (protectViewWindowApplication != null)
                        {
                            bDisplayWorkbookTabs = protectViewWindowApplication.ActiveWindow.DisplayWorkbookTabs;
                            bDisplayVerticalScrollBar = protectViewWindowApplication.ActiveWindow.DisplayVerticalScrollBar;
                            bDisplayHorizontalScrollBar = protectViewWindowApplication.ActiveWindow.DisplayHorizontalScrollBar;
                        }

                        bProtectedToRegularMode = true;
                        Globals.ThisAddIn.Application.ActiveProtectedViewWindow.Edit();

                        // Adding null check on safe side.
                        if (Globals.ThisAddIn.Application != null)
                        {
                            Globals.ThisAddIn.Application.ActiveWindow.DisplayWorkbookTabs = bDisplayWorkbookTabs;
                            Globals.ThisAddIn.Application.ActiveWindow.DisplayVerticalScrollBar = bDisplayVerticalScrollBar;
                            Globals.ThisAddIn.Application.ActiveWindow.DisplayHorizontalScrollBar = bDisplayHorizontalScrollBar;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            try
            {
                // If control is coming after enable Editing done from Application_ProtectedViewWindowOpen, do WorkbookOpen again.
                // This is done since after coming out from portected mode of workbook (i.e. EnableEditing clicked), we do not get handle of active workbook
                if (bProtectedToRegularMode)
                {
                    bProtectedToRegularMode = false;
                    Application_WorkbookOpen(Wb);
                }

                // Runtime
                // Check if metadata sheet exists
                if (ObjectManager.RuntimeMode == RuntimeMode.AddIn && Globals.ThisAddIn != null)
                {
                    if (Core.ObjectManager.RuntimeMode == Core.RuntimeMode.AddIn)
                    {
                        Globals.ThisAddIn.AppVersion = "";
                        Globals.ThisAddIn.Application.StatusBar = "";
                    }
                    Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
                    if (oSheet == null)
                    {
                        if (Globals.ThisAddIn.RuntimeRibbon.MenuBuilder == null)
                            return;

                        Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                        Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateRibbon(Globals.ThisAddIn.RuntimeRibbon);
                        return;
                    }

                    // Get application Unique Id
                    MetadataManager metadataManager = MetadataManager.GetInstance;
                    string appUniqueId = metadataManager.GetAppUniqueId();

                    // Get Application instance
                    //ApplicationInstance appInstance = ApplicationManager.GetInstance.Get(appUniqueId, ApplicationMode.Runtime);
                    ApplicationInstance appInstance = Utils.GetApplicationInstance(appUniqueId, ExcelHelper.ExcelApp.Application.ActiveWorkbook.Name, ApplicationMode.Runtime);

                    //set Edition
                    string EditionType = null;
                    /*******************************************/
                    //when you open a file from designer this event will get called 
                    // and for designer files appinstance will be null so edition will not be read from app instance
                    // for runtime files, the instance wont be null and edition will be fetched from the runtime usng 
                    //following logic
                    /*********************************************/
                    if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null && appInstance.App.Definition.EditionType != null)
                    {
                        // apps created using the apptype ui will get the correct edition
                        EditionType = appInstance.App.Definition.EditionType;
                    }
                    else if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null && appInstance.App.QuickAppModel != null)
                    //ConfigurationManager.GetInstance.Application.QuickAppModel !=null) 
                    {
                        // apps created in previous build
                        EditionType = Constants.BASIC_EDITION;
                    }
                    else if (appInstance != null && appInstance.App != null && appInstance.App.Definition != null)
                    {
                        EditionType = Constants.ENTERPRISE_EDITION;
                    }

                    if (Core.ObjectManager.RuntimeMode == Core.RuntimeMode.AddIn)
                    {
                        if (!string.IsNullOrEmpty(EditionType))
                        {
                            Globals.ThisAddIn.Application.StatusBar = EditionType;
                            Globals.ThisAddIn.AppVersion = EditionType;
                        }
                    }

                    // Get runtime flag
                    string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
                    if (Globals.ThisAddIn.RuntimeRibbon.MenuBuilder == null)
                        return;

                    Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.Config = ConfigurationManager.GetInstance;
                    if (appInstance != null && (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true")))
                    {
                        // Set Application instance
                        Utils.SetApplicationInstance(appInstance);

                        if (commandBarManager.IsLoggedIn())
                        {
                            Globals.ThisAddIn.RuntimeRibbon.ActivateTab("tabAppBuilderRuntime");
                            Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                            Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.BuildMenusFromXml(Globals.ThisAddIn.RuntimeRibbon);

                        }
                    }
                    else if (string.IsNullOrEmpty(isRuntimeFile) || !isRuntimeFile.Equals("true"))  // This is Designer clear runtime Ribbon menus
                    {
                        Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                    }
                    Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateRibbon(Globals.ThisAddIn.RuntimeRibbon);
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        void Application_SheetChange(object Sh, Excel.Range Target)
        {
            try
            {
                string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);

                // Check if file is a runtime file
                if (ConfigurationManager.GetInstance.Application != null && (!string.IsNullOrEmpty(isRuntimeFile) && isRuntimeFile.Equals("true")))
                {
                    if ((Target.Cells.Count % Constants.EXCEL_NUMBEROFCOLUMNS) == 0)
                    {
                        bool isInsertRow = Utils.IsInsertDeleteRowAddress(Target.Address);
                        Excel.Range insertRange = ExcelHelper.GetNamedRangeFromCell(Target, true);
                        if (isInsertRow && insertRange != null)
                        {
                            ApttusMessageUtil.ShowInfo(string.Format(@resourceManager.GetResource("WBEVENTMAN_Application_SheetChange_InfoMsg"), resourceManager.CRMName), Constants.RUNTIME_PRODUCT_NAME);
                            ExcelHelper.UndoLastAction();
                            return;
                        }
                    }

                    if (Target.Cells.Count > 1)
                        return;

                    if (ApplicationDefinitionManager.GetInstance.IsMultiSelectPickListExistsInApp())
                    {
                        Excel.Range namedRange = ExcelHelper.GetNamedRangeFromCell(Target);
                        if (namedRange != null)
                        {
                            RuntimeEditActionController runtimeControllerMultiPicklist = new RuntimeEditActionController(Target, namedRange);
                            runtimeControllerMultiPicklist.AddMultiSelectPickListValues();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        void ExcelApp_SheetBeforeDoubleClick(object Sh, Excel.Range Target, ref bool Cancel)
        {
            try
            {
                Excel.Range namedRange = ExcelHelper.GetNamedRangeFromCell(Target);
                if (namedRange != null)
                {
                    LookAheadLauncher LHLauncher = new LookAheadLauncher(namedRange, Target);
                    LHLauncher.LauchUI();
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        #endregion

        #region "Private Helper Methods"

        #endregion

    }
}
