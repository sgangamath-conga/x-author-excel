/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{

    public class GenericListUIController : IGenericListUIController
    {
        public List<GenericListUIModel> Model { get; set; }
        private GenericListUIView view;
        private ListScreenType currentScreenType;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public GenericListUIController(List<dynamic> model, GenericListUIView view, ListScreenType screenType)
        {
            this.currentScreenType = screenType;
            // 1. Set the Model
            GenerateModel(model);
            // 2. Set the view
            this.view = view;
            this.view.SetController(this);
        }

        public void SetView()
        {
            view.SetViewTitle(currentScreenType);
            view.BindGrid(Model);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Add()
        {
            switch (currentScreenType)
            {
                case ListScreenType.DisplayMap:
                    // Remove CustomPane
                    TaskPaneHelper.RemoveCustomPane("Display Map");
                    //if (configurationManager.Definition.Type == ApplicationType.CrossTab)
                    //{
                    //    CrossTabRetrieveUI CrossTabRetUI = new CrossTabRetrieveUI();
                    //    CrossTabRetrievalController controller = new CrossTabRetrievalController(CrossTabRetUI);
                    //    controller.initialize(null);

                    //}
                    //else
                    {
                        ucRetrieveMap ucRetrieveMapView = new ucRetrieveMap();
                        RetrieveMapController controller = new RetrieveMapController(ucRetrieveMapView);
                        ucRetrieveMapView.SetController(controller);

                        controller.Initialize(null);
                    }
                    view.Dispose();
                    break;
                case ListScreenType.SaveMap:
                    // Remove CustomPane
                    TaskPaneHelper.RemoveCustomPane(Constants.SAVEMAP_NAME);

                    SaveMap saveMapModel = new SaveMap();
                    SaveMapView saveMapView = new SaveMapView();
                    SaveMapController saveMapcontroller = new SaveMapController(saveMapModel, saveMapView, FormOpenMode.Create);
                    saveMapView.SetController(saveMapcontroller);

                    saveMapcontroller.LoadControls();
                    view.Dispose();
                    break;
                case ListScreenType.Actionflow:

                    WorkflowDesigner wfDesignerView = new WorkflowDesigner(string.Empty);
                    WorkflowController wfcontroller = new WorkflowController();
                    wfDesignerView.SetController(wfcontroller);

                    wfcontroller.Initialize(null, wfDesignerView);
                    view.Dispose();
                    break;

                case ListScreenType.MatrixMap:

                    TaskPaneHelper.RemoveCustomPane(Constants.MATRIXMAP_NAME);

                    MatrixMap matrixModel = new MatrixMap();
                    ucMatrixView matrixView = new ucMatrixView();
                    MatrixViewController matrixController = new MatrixViewController(matrixView, matrixModel, FormOpenMode.Create);
                    view.Dispose();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        public void Edit(DataGridViewSelectedRowCollection rows)
        {
            if (rows.Count != 1)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("GENERICLISTUICTL_Edit_WarnMsg"), Utils.GetEnumDescription(currentScreenType, string.Empty));
                return;
            }

            var model = (from DataGridViewRow r in rows
                         select ((GenericListUIModel)r.DataBoundItem).ListItem).FirstOrDefault();

            switch (currentScreenType)
            {
                case ListScreenType.DisplayMap:
                    // Remove CustomPane
                    TaskPaneHelper.RemoveCustomPane("Display Map");

                    //if (model != null && model.GetType() == typeof(CrossTabRetrieveMap))
                    //{
                    //    CrossTabRetrieveUI CrossTabRetUI = new CrossTabRetrieveUI();
                    //    CrossTabRetrievalController controller = new CrossTabRetrievalController(CrossTabRetUI);
                    //    controller.initialize(model as CrossTabRetrieveMap);
                    //}
                    //else
                    //{
                    ucRetrieveMap ucRetrieveMapView = new ucRetrieveMap();
                    RetrieveMapController controller = new RetrieveMapController(ucRetrieveMapView);
                    ucRetrieveMapView.SetController(controller);

                    controller.Initialize((RetrieveMap)model);
                    //}
                    view.Dispose();
                    break;
                case ListScreenType.SaveMap:
                    // Remove CustomPane
                    TaskPaneHelper.RemoveCustomPane(Constants.SAVEMAP_NAME);

                    SaveMapView saveMapView = new SaveMapView();
                    SaveMapController saveMapcontroller = new SaveMapController((SaveMap)model, saveMapView, FormOpenMode.Edit);
                    saveMapView.SetController(saveMapcontroller);

                    saveMapcontroller.LoadControls();
                    view.Dispose();
                    break;
                case ListScreenType.Actionflow:

                    WorkflowDesigner wfDesignerView = new WorkflowDesigner(string.Empty);
                    WorkflowController wfcontroller = new WorkflowController();
                    wfDesignerView.SetController(wfcontroller);

                    wfcontroller.Initialize((WorkflowStructure)model, wfDesignerView);
                    view.Dispose();
                    break;

                case ListScreenType.MatrixMap:

                    TaskPaneHelper.RemoveCustomPane(Constants.MATRIXMAP_NAME);

                    ucMatrixView matrixView = new ucMatrixView();
                    MatrixViewController matrixController = new MatrixViewController(matrixView, (MatrixMap)model, FormOpenMode.Edit);
                    view.Dispose();
                    break;

                default:
                    break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        public void Delete(DataGridViewSelectedRowCollection rows)
        {
            if (rows.Count == 0)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("GENERICLISTUICTL_DeleteSelect_WarnMsg"), Utils.GetEnumDescription(currentScreenType, string.Empty));
                return;
            }
            else if (rows.Count >= 1)
            {

                var model = from DataGridViewRow r in rows
                            select ((GenericListUIModel)r.DataBoundItem).ListItem;

                switch (currentScreenType)
                {
                    case ListScreenType.DisplayMap:
                        foreach (RetrieveMap m in model)
                        {
                            List<ValidationResult> results = ValidationManager.ValidateDelete(m);
                            if (ValidationManager.VerifyResults(results))
                            {
                                if (ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("GENERICLISTUICTL_Delete_WarnMsg"),Utils.GetEnumDescription(currentScreenType, string.Empty)), 
                                                                        Utils.GetEnumDescription(currentScreenType, string.Empty),
                                                                        ApttusMessageUtil.YesNo)
                                    == TaskDialogResult.Yes)
                                {
                                    configurationManager.RetrieveMaps.Remove(m);
                                    RetrieveMapController.DeleteRetrieveMapNamedRange(m);
                                    //CrossTabRetrievalController.DeleteCrossTabMapNamedRange(m);

                                    GenerateModel(configurationManager.RetrieveMaps.ToList<dynamic>());
                                }
                            }
                            else
                            {
                                DisplayErrorViewer(results, "Error(s) while deleting a display map...");                                
                            }
                        }
                        break;
                    
                    case ListScreenType.SaveMap:
                        foreach (SaveMap m in model)
                        {
                            List<ValidationResult> results = ValidationManager.ValidateDelete(m);
                            if (ValidationManager.VerifyResults(results))
                            {
                                if (ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("GENERICLISTUICTL_Delete_WarnMsg"), Utils.GetEnumDescription(currentScreenType, string.Empty)), 
                                                                        Utils.GetEnumDescription(currentScreenType, string.Empty),
                                                                        ApttusMessageUtil.YesNo)
                                    == TaskDialogResult.Yes)
                                {
                                    configurationManager.SaveMaps.Remove(m);
                                    GenerateModel(configurationManager.SaveMaps.ToList<dynamic>());
                                }
                            }
                            else
                            {
                                DisplayErrorViewer(results, "Error(s) while deleting a save map...");
                            }
                        }
                        break;
                    case ListScreenType.Actionflow:
                        foreach (Workflow m in model)
                        {
                            List<ValidationResult> results = ValidationManager.ValidateDelete(m);
                            if (ValidationManager.VerifyResults(results))
                            {
                                if (ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("GENERICLISTUICTL_Delete_WarnMsg"), Utils.GetEnumDescription(currentScreenType, string.Empty)), 
                                                                    Utils.GetEnumDescription(currentScreenType, string.Empty),
                                                                    ApttusMessageUtil.YesNo)
                                    == TaskDialogResult.Yes)
                                {
                                    configurationManager.Workflows.Remove(m);
                                    GenerateModel(configurationManager.Workflows.ToList<dynamic>());
                                }
                            }
                            else
                            {
                                DisplayErrorViewer(results, "Error(s) while deleting a action flow...");
                            }
                        }
                        break;
                    case ListScreenType.MatrixMap:
                    
                        foreach (MatrixMap m in model)
                        {
                            List<ValidationResult> results = ValidationManager.ValidateDelete(m);
                            if (ValidationManager.VerifyResults(results))
                            {
                                if (ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("GENERICLISTUICTL_Delete_WarnMsg"), Utils.GetEnumDescription(currentScreenType, string.Empty)),
                                                                        Utils.GetEnumDescription(currentScreenType, string.Empty),
                                                                        ApttusMessageUtil.YesNo)
                                    == TaskDialogResult.Yes)
                                {
                                    configurationManager.MatrixMaps.Remove(m);
                                    RetrieveMapController.DeleteMatrixMapNamedRange(m);

                                    GenerateModel(configurationManager.MatrixMaps.ToList<dynamic>());
                                }
                            }
                            else
                            {
                                DisplayErrorViewer(results, "Error(s) while deleting a matrix map...");
                            }
                        }
                        
                        break;
                    default:
                        break;
                }
                view.BindGrid(Model);                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        private void GenerateModel(List<dynamic> list)
        {
            if (currentScreenType == ListScreenType.Actionflow)
            {
                Model = (from r in list.OfType<WorkflowStructure>()
                         select new GenericListUIModel
                         {
                             Id = r.Id,
                             Name = r.Name,
                             ListType = currentScreenType,
                             ListItem = r as Workflow,
                             //AutoExecute = r.Triggers != null && r.Triggers.Count > 0
                             AutoExecuteRegularMode = r.AutoExecuteRegularMode || (r.Triggers != null && r.Triggers.Count > 0),
                             AutoExecuteEditInExcelMode = r.AutoExecuteEditInExcelMode || (r.Triggers != null && r.Triggers.Count > 0)
                         }).ToList();
            }
            else
            {
                Model = (from r in list
                         select new GenericListUIModel
                         {
                             Id = r.Id,
                             Name = r.Name,
                             ListType = currentScreenType,
                             ListItem = r
                         }).ToList();
            }
        }

        private void DisplayErrorViewer(List<ValidationResult> results, string messageHeader)
        {
            ErrorMessageViewer viewer = new ErrorMessageViewer();
            viewer.ErrorMessageHeader = messageHeader;
            viewer.AddResults(results);
            viewer.ShowDialog();
            viewer = null;
        }

        public DataGridViewSelectedRowCollection GetSelectedRows()
        {
            return view.GetSelectedRows();
        }
    }
}
