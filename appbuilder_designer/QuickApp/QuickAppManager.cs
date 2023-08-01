/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{    
    public sealed class QuickAppManager
    {
        private ApplicationHelper applicationHelper;
        private IQuickApplication quickApplication;
        private ApttusCommandBarManager commandBar;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public QuickAppManager()
        {
            applicationHelper = ApplicationHelper.GetInstance;
            commandBar = ApttusCommandBarManager.GetInstance();
        }

        private void CreateNewApplication(string appName)
        {
            bool bCreateQuickApp = true;
            ApplicationHelper.GetInstance.EditionType = Constants.BASIC_EDITION;
            applicationHelper.CreateApplication(appName, string.Empty, true, bCreateQuickApp);
        }

        private void SaveApplication(bool saveForSheets)
        {
            if (applicationHelper.EditionType == null) // during edit of QAPP, the edition is not set. set it explicitly
            {
                ApplicationHelper.GetInstance.EditionType = Constants.BASIC_EDITION;
            }
            applicationHelper.SaveApplication(false, string.Empty, saveForSheets);
        }

        private void BuildDataRetrievalAction(WizardModel Model)
        {
            foreach (ActionSelectionFilter Action in Model.Actions)
            {
                switch (Action.ActionType)
                {
                    case Core.ActionType.Query:
                        {
                            quickApplication.BuildQueryAction(Model, Action);
                        }
                        break;
                    case Core.ActionType.SearchAndSelect:
                        {
                            quickApplication.BuildSearchAndSelectAction(Model, Action);
                        }
                        break;
                }
            }
        }

        private void BuildSalesforceObjects(WizardModel Model)
        {
            quickApplication.BuildSalesforceObjects(Model);
        }

        private void BuilDisplayMap(WizardModel Model)
        {
            quickApplication.BuildDisplayMap(Model);
        }

        private void BuildSaveMap(WizardModel Model)
        {
            quickApplication.BuildSaveMap(Model, -1);
        }

        private void BuildWorkflow(WizardModel Model)
        {
            quickApplication.ConfigureWorkflow(Model);
        }

        private void BuildMenu(WizardModel Model)
        {
            quickApplication.BuildMenu(Model);
        }

        private void BuildAppSettings(WizardModel Model)
        {
            quickApplication.BuildAppSettings(Model);
        }

        private void InitializeApplication(WizardModel Model)
        {
            switch (Model.AppType)
            {
                case QuickAppType.ListApp:
                    quickApplication = new ListApp();
                    break;
                case QuickAppType.ParentChildApp:
                    quickApplication = new ParentChildApp();
                    break;
            }
        }

        private void updateStatusMessage(WaitWindowView waitForm, string msg)
        {
            waitForm.Message = msg;
        }

        public void BuildApplication(WizardModel Model, WaitWindowView waitForm)
        {
            try
            {
                InitializeApplication(Model);

                if (quickApplication != null)
                {
                    Globals.ThisAddIn.Application.ScreenUpdating = false;

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureNewApp_Msg"));
                    CreateNewApplication(Model.AppName);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigure_Msg"));
                    BuildSalesforceObjects(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureDisplayMap_Msg"));
                    BuilDisplayMap(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureSaveMap_Msg"));
                    BuildSaveMap(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureRetrieval_Msg"));
                    BuildDataRetrievalAction(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureWork_Msg"));
                    BuildWorkflow(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureMenu_Msg"));
                    BuildMenu(Model);

                    ExcelHelper.ShowGridLines(Model.ShowGridLines);
                    
                    ConfigurationManager.GetInstance.Application.QuickAppModel = Model;

                    BuildAppSettings(Model);


                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureSaveApp_Msg"));
                    SaveApplication(Model.AllowSaveForSheets);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                throw ex;
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// This method will be invoked in edit mode to clear the formatting applied on retrieve map.
        /// </summary>
        private void ClearRetrieveMapFormatting()
        {
            ConfigurationManager config = ConfigurationManager.GetInstance;
            Excel.Range retrieveMapRange = ExcelHelper.GetRange(config.RetrieveMaps[0].RepeatingGroups[0].TargetNamedRange);
            retrieveMapRange.Clear();
            RetrieveMapController.DeleteRetrieveMapNamedRange(config.RetrieveMaps[0]);
        }

        /// <summary>
        /// We create a new instance of Application with existing appname and uniqueId in edit mode, so that we don't need to modify the existing config.        
        /// </summary>
        /// <param name="AppName"></param>
        /// <param name="uniqueId"></param>
        public void SetNewApplication(string AppName, string uniqueId)
        {
            Core.Application NewApp = new Application();
            NewApp.Definition.UniqueId = uniqueId;
            NewApp.Definition.Name = AppName;
            NewApp.Definition.Version = "1.0";
            NewApp.Definition.Type = ApplicationType.RepeatingCells;
            NewApp.Definition.EditionType = Constants.BASIC_EDITION;

            ConfigurationManager.GetInstance.SetApplication(NewApp);

            Utils.AddApplicationInstance(ApplicationMode.Designer, Globals.ThisAddIn.Application.ActiveWorkbook.Name, false, true);
        }

        internal void UpdateApplication(WizardModel Model, WaitWindowView waitForm, string appName, string uniqueId)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                InitializeApplication(Model);

                if (quickApplication != null)
                {
                    ClearRetrieveMapFormatting();

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPP_Update_InfoMsg") + appName);
                    SetNewApplication(appName, uniqueId);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPP_UpdateSalesObj_InfoMsg"));
                    BuildSalesforceObjects(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPP_UpdateDM_InfoMsg"));
                    BuilDisplayMap(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPP_UpdateSaveMP_InfoMsg"));
                    BuildSaveMap(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPP_UpdateDataRetrie_InfoMsg"));
                    BuildDataRetrievalAction(Model);

                    BuildWorkflow(Model);

                    BuildMenu(Model);

                    ExcelHelper.ShowGridLines(Model.ShowGridLines);

                    ConfigurationManager.GetInstance.Application.QuickAppModel = Model;

                    BuildAppSettings(Model);

                    updateStatusMessage(waitForm, resourceManager.GetResource("QUICKAPPMAN_BuildApplicationConfigureSaveApp_Msg") + appName);
                    SaveApplication(Model.AllowSaveForSheets);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                throw ex;
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }
    }
}
