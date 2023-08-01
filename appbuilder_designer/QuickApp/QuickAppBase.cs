/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppDesigner.Modules;
using Microsoft.Win32;
using Apttus.SettingsManager;

namespace Apttus.XAuthor.AppDesigner
{
    interface IQuickApplication
    {
        void BuildSalesforceObjects(WizardModel Model);
        void BuildDisplayMap(WizardModel Model);
        void BuildSaveMap(WizardModel Model, int saveMapIndex);
        void BuildQueryAction(WizardModel Model, ActionSelectionFilter action);
        void ConfigureWorkflow(WizardModel Model);
        void BuildMenu(WizardModel Model);
        void BuildSearchAndSelectAction(WizardModel Model, ActionSelectionFilter action);
        void BuildAppSettings(WizardModel Model);
    }

    internal class QuickApplicationBase : IQuickApplication
    {
        protected ApplicationDefinitionManager applicationDefinitionManager;
        protected ConfigurationManager configManager;
        protected RetrieveMapController retrieveMapcontroller;
        protected SaveMapController saveMapController;
        protected List<WorkflowStructure> ActionFlows; //there could be multiple workflows, hence a list of WorkflowStructure
        protected List<SearchAndSelect> searchAndSelectActions;
        protected ModelMenu Menu;
        protected QueryActionController queryAction;
        protected ApttusCommandBarManager commandBar;
        protected Excel.Range salesforceIDColumn;
        protected SettingsManager.ApplicationSettings quickAppSettings;

        protected QuickApplicationBase()
        {
            applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            configManager = ConfigurationManager.GetInstance;
            quickAppSettings = ApplicationConfigManager.GetInstance().ApplicationSettings;

            retrieveMapcontroller = new RetrieveMapController(null);
            retrieveMapcontroller.Initialize(null);

            ActionFlows = new List<WorkflowStructure>();
            searchAndSelectActions = new List<SearchAndSelect>();

            SaveMap saveMapModel = new SaveMap();
            saveMapController = new SaveMapController(saveMapModel, null, FormOpenMode.Create);

            QueryActionModel queryActionModel = new QueryActionModel();
            queryAction = new QueryActionController(queryActionModel, null, FormOpenMode.Create);

            commandBar = ApttusCommandBarManager.GetInstance();
        }

        protected void BuildSaveWorkflow(WizardModel Model)
        {
            if (configManager.Actions.Where(action => action.Type.Equals(Constants.SAVE_ACTION)).Count() == 0)
                return;

            int stepCounter = 0;
            int conditionCounter = 0;
            int actionCounter = 0;
            Guid guidStep = Guid.Empty;

            WorkflowStructure actionFlow = new WorkflowStructure();
            actionFlow.Name = QuickAppConstants.SaveWorkflowName;

            //First Step
            Step step = new Step();
            step.Name = "Step 1";
            step.SequenceNo = ++stepCounter;
            step.Id = Guid.NewGuid();

            Condition condition = new Condition();
            condition.Id = Guid.NewGuid();
            condition.Name = "Step 1";
            condition.SequenceNo = ++conditionCounter;

            WorkflowAction wfAction = new WorkflowAction();
            wfAction.ActionId = configManager.Actions.Where(action => action.Type.Equals(Constants.SAVE_ACTION)).Select(action => action.Id).FirstOrDefault().ToString();
            Core.Action WFAction = configManager.GetActionById(wfAction.ActionId);
            wfAction.Name = WFAction.Name + " (" + WFAction.Type + ")";
            wfAction.SequenceNo = ++actionCounter;
            wfAction.Type = "Action";
            wfAction.WorkflowActionData = new WorkflowActionData();
            wfAction.WorkflowActionData.Id = guidStep = Guid.NewGuid();
            wfAction.WorkflowActionData.AppObjectId = configManager.GetActionTargetObjectsById(wfAction.ActionId);

            condition.WorkflowActions.Add(wfAction);

            step.Conditions.Add(condition);

            actionFlow.Steps.Add(step);

            ActionFlows.Add(actionFlow);
        }

        private void CreateDisplayAction(Guid retrieveMapId)
        {
            RetrieveActionModel model = new RetrieveActionModel();
            RetrieveActionController retrieveAction = new RetrieveActionController(model, null, FormOpenMode.Create);
            retrieveAction.Save(QuickAppConstants.DisplayActionName, retrieveMapId, null);
        }

        private void CreateSaveAction(Guid SaveMapId)
        {
            SaveActionModel model = new SaveActionModel();
            SaveActionController saveActionController = new SaveActionController(model, null, FormOpenMode.Create);
            saveActionController.Save(QuickAppConstants.SaveActionName, SaveMapId, false, false, false, Constants.SAVE_ACTION_BATCH_SIZE);
        }


        private void GetAppColorSettings(ref int titleColor, ref int displayfieldColor, ref int savefieldColor)
        {
            string KeyVal = quickAppSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleColor];
            if (!String.IsNullOrEmpty(KeyVal))
                titleColor = Convert.ToInt32(KeyVal);

            KeyVal = quickAppSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsColor];
            if (!String.IsNullOrEmpty(KeyVal))
                displayfieldColor = Convert.ToInt32(KeyVal);

            KeyVal = quickAppSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsColor];
            if (!String.IsNullOrEmpty(KeyVal))
                savefieldColor = Convert.ToInt32(KeyVal);
        }

        public virtual void BuildSalesforceObjects(WizardModel Model)
        {
            if (Model.AppType == QuickAppType.ListApp)
            {
                if (Model.Object.RecordTypes != null && Model.Object.RecordTypes.Count > 0 && Model.Object.Fields.Exists(f => f.RecordType))
                    ObjectManager.GetInstance.FillRecordTypeMetadata(Model.Object);
            }
            applicationDefinitionManager.Save();
        }

        public virtual void BuildDisplayMap(WizardModel Model)
        {
            //Apply the Named Range to the repeating group
            if (retrieveMapcontroller.RetrieveMap.RepeatingGroups.Count > 0)
            {
                retrieveMapcontroller.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, retrieveMapcontroller.RetrieveMap.RepeatingGroups[0]);

                //Save the RetrieveMap
                retrieveMapcontroller.Save(QuickAppConstants.DisplayMapName);

                //Set Color for repeating group range.
                try
                {
                    int titleBackColor = -1, displayFieldColor = -1, saveFieldColor = -1;
                    
                    GetAppColorSettings(ref titleBackColor, ref displayFieldColor, ref saveFieldColor);
                    Excel.Range repeatingGroupRange = ExcelHelper.GetRange(retrieveMapcontroller.RetrieveMap.RepeatingGroups[0].TargetNamedRange);

                    if (repeatingGroupRange != null)
                    {
                        Excel.Range titleRange = repeatingGroupRange.Worksheet.Cells[1, 1];
                        if (titleRange != null)
                        {
                            titleRange.Value = string.IsNullOrEmpty(Model.WorksheetTitle) ? "" : Model.WorksheetTitle;
                            System.Drawing.Color titleBackgroundColor = System.Drawing.Color.FromArgb(titleBackColor);
                            if ((titleBackgroundColor.R == 255 &&  titleBackgroundColor.G == 255 && titleBackgroundColor.B == 255) || titleBackColor == -1)
                            {
                                titleRange.Interior.Pattern = Excel.XlPattern.xlPatternNone;
                                titleRange.Interior.TintAndShade = 0;
                                titleRange.Interior.PatternTintAndShade = 0;
                            }
                            else
                                titleRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(titleBackgroundColor);
                            SetFont(titleRange, quickAppSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleFont], quickAppSettings.AppExtendedProperties[ApttusGlobals.WorksheetTitleTextColor]);
                        }
                    }

                    foreach (RetrieveField field in retrieveMapcontroller.RetrieveMap.RepeatingGroups[0].RetrieveFields)
                    {
                        ApttusObject obj = applicationDefinitionManager.GetAppObject(field.AppObject);
                        Excel.Range fieldrange = ExcelHelper.NextVerticalCell(ExcelHelper.GetRange(field.TargetNamedRange), -1);
                        QuickAppFieldAttribute fld = Model.DisplayFields.Find(sf => sf.FieldId.Equals(field.FieldId) && sf.ObjectId.Equals(obj.Id));
                        if (fld != null && fld.Save)
                        {
                            fieldrange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(saveFieldColor));
                            SetFont(fieldrange, quickAppSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsFont], quickAppSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsTextColor] );
                        }
                        else
                        {
                            fieldrange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(displayFieldColor));
                            SetFont(fieldrange, quickAppSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsFont], quickAppSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsTextColor]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }

                //Create a retrieve action for this retrieve map
                if (retrieveMapcontroller.RetrieveMap.Id != Guid.Empty)
                    CreateDisplayAction(retrieveMapcontroller.RetrieveMap.Id);

                //Hide the ID Column.
                if (salesforceIDColumn != null && salesforceIDColumn.Column > 2)
                {
                    Excel.Range column = salesforceIDColumn.Worksheet.Columns[salesforceIDColumn.Column];
                    column.Hidden = true;
                }
            }
        }

        private void SetFont(Excel.Range rng, string fontStr, string textColor)
        {
            if (rng != null)
            {
                try
                {
                    using (System.Drawing.Font fnt = Utils.ConvertFontFromString(fontStr)) //font is a system resource, needs to be disposed after the use.
                    {
                        rng.Font.Name = fnt.FontFamily.Name; // "Times New Roman";
                        rng.Font.Size = fnt.Size; //Set the font size of the repeating group title.
                        rng.Font.Bold = fnt.Bold;
                        int colorCode = Convert.ToInt32(textColor);
                        rng.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(colorCode));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }
            }
            
        }

        public virtual void BuildQueryAction(WizardModel Model, ActionSelectionFilter action)
        {

        }

        public virtual void ConfigureWorkflow(WizardModel Model)
        {
            foreach (WorkflowStructure workflow in ActionFlows)
                workflow.SaveWorkflow(); //Save all the Actionflows defined by the app.
        }

        public virtual void BuildMenu(WizardModel Model)
        {
            if (Menu == null)
                Menu = new ModelMenu();
            MenuGroup menuGroup = Menu.AddMenu();
            menuGroup.Name = Model.MenuGroupName;
            MenuItem menuItem = Menu.AddMenuItem(menuGroup);
            menuItem.Icon = QuickAppConstants.RetrieveMenuIcon;
            menuItem.Name = Model.DisplayMenuButtonName;
            menuItem.WorkFlowID = configManager.GetWorkflowByName(QuickAppConstants.RetrieveWorkflowName).Id;
            
            if (configManager.Workflows.Count > 1 && configManager.Actions.Where(action => action.Type.Equals(Constants.SAVE_ACTION)).Count() == 1)
            {
                menuItem = Menu.AddMenuItem(menuGroup);
                menuItem.Icon = QuickAppConstants.SaveMenuIcon;
                menuItem.Name = Model.SaveMenuButtonName;
                menuItem.WorkFlowID = configManager.GetWorkflowByName(QuickAppConstants.SaveWorkflowName).Id;
            }
            Menu.SaveMenu(Model.AllowAddRow, Model.AllowDeleteRow, false, false);
        }

        public virtual void BuildSaveMap(WizardModel Model, int saveMapIndex)
        {
            if (saveMapIndex == -1)
                return;

            if (configManager.SaveMaps.Count > saveMapIndex && configManager.SaveMaps.ElementAt(saveMapIndex).Id != Guid.Empty)
                CreateSaveAction(configManager.SaveMaps[saveMapIndex].Id);
        }

        public virtual void BuildSearchAndSelectAction(WizardModel Model, ActionSelectionFilter action)
        {
            foreach (SearchAndSelect searchAndSelect in searchAndSelectActions)
                searchAndSelect.SaveAction();
        }

        public virtual void BuildAppSettings(WizardModel Model)
        {
            configManager.Definition.AppSettings = new AppSettings();
            configManager.Definition.AppSettings.AutoSizeColumns = true;
            configManager.Definition.AppSettings.MaxColumnWidth = Model.MaxColumnWidth;
            configManager.Definition.AppSettings.ShowFilters = Model.ShowAutoFilter;
        }
    }
}
