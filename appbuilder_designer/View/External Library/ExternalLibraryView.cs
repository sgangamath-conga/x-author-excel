using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Apttus.XAuthor.Core;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ExternalLibraryView : Form
    {
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private string ExternalLibraryPath;
        private SortableBindingList<ExternalLibGridModel> gridModel;
        private BindingSource gridSource;
        private List<Type> actionAlreadyInUseByApp = new List<Type>();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ExternalLibraryView()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            SetCultureData();
            ExternalLibraryPath = Utils.GetDirectoryForExternalLib();
            gridModel = new SortableBindingList<ExternalLibGridModel>();
            gridSource = new BindingSource();
        }
        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("EXTERNALACTIONVIEW_lblTitle_Text");
            lblLoadExternalLibrary.Text = resourceManager.GetResource("EXTERNALACTIONVIEW_lblLoadExternalLibrary_Text");
            ExternalActionName.HeaderText = resourceManager.GetResource("EXTERNALACTIONVIEW_ExternalActionName_Text");
            ExternalLibraryName.HeaderText = resourceManager.GetResource("EXTERNALACTIONVIEW_ExternalLibraryName_Text");
            btnOK.Text = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Text");

        }
        private bool ValidateAssembly(string fileName)
        {
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.AssemblyName.GetAssemblyName(fileName);
            }
            catch (System.BadImageFormatException)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_BadImage_ErrMsg"), resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_Caption_ErrMsg"));
                return false;
            }
            catch (System.IO.FileNotFoundException)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_FileNotFound_ErrMsg"), resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_Caption_ErrMsg"));
                return false;
            }
            catch (System.IO.FileLoadException)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_FileLoad_ErrMsg"), resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_Caption_ErrMsg"));
                return false;
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("EXTERNALACTIONVIEW_ValidateAssembly_Unknown_ErrMsg"), resourceManager.GetResource("QUICKAPP_CreatingCap_ErrorMsg"));
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
            return true;
        }

        private List<Type> GetExportedTypesFromAssembly(string fileNameWithPath)
        {
            //Step 1. Check whether the given dll is a .Net Assembly or Not
            if (ValidateAssembly(fileNameWithPath))
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(fileNameWithPath);
                System.Type[] exportedTypes = assembly.GetExportedTypes();
                return exportedTypes.ToList();
            }
            return null;
        }

        private bool PopulateActionsFromAssembly(List<Type> exportedTypes, ExternalLibrary externalLibrary, ref string externalLibraryId)
        {
            List<Core.Action> invalidActions = null;
           
            bool isExternalLibraryInUse = false; //If any one of the external library is in use, then set it.
           
            List<ExternalAction> exportedExternalActions = new List<ExternalAction>();

            foreach (Type externalClassType in exportedTypes)
            {
                if (!externalClassType.IsClass || actionAlreadyInUseByApp.Contains(externalClassType))
                    continue;

                //Only those types which conform to X-Author Action standard, will only be loaded in memory and used.
                //2.1 Does the type in the external action assembly implements Core.Action and IActionRuntime Interface.
                if (typeof(Core.Action).IsAssignableFrom(externalClassType) && typeof(Core.IActionRuntime).IsAssignableFrom(externalClassType))
                {
                    //2.2 Create an instance of external action.
                    Core.Action externalAction = Activator.CreateInstance(externalClassType) as Core.Action;

                    //2.3 Does the action has a Name attribute.
                    bool isActionValid = !string.IsNullOrEmpty(externalAction.Name);
                    if (isActionValid)
                    {
                        isExternalLibraryInUse = true;

                        ExternalAction action = new ExternalAction
                        {
                            Name = externalAction.Name,
                            TypeName = externalClassType.Name,
                            TypeFullName = externalClassType.FullName
                        };

                        configManager.AddAction(action);

                        externalLibrary.Actions.Add(action.Id);

                        exportedExternalActions.Add(action); //Used to assign the externalLibraryId;

                        ExternalLibGridModel gridRow = new ExternalLibGridModel { ExternalActionName = action.Name, 
                            ExternalLibName = externalLibrary.ExternalLibraryName, ActionId = action.Id };
                        gridModel.Add(gridRow);
                    }
                    else
                    {
                        if (invalidActions == null)
                            invalidActions = new List<Core.Action>();
                        invalidActions.Add(externalAction);
                    }
                }
            }

            if (isExternalLibraryInUse) //If the library is in use, add it in the config.
            {
                if (string.IsNullOrEmpty(externalLibraryId)) //If library doesn't exist in config, create it.
                    externalLibraryId = configManager.AddExternalLibrary(externalLibrary);

                foreach (ExternalAction externalAction in exportedExternalActions)
                    externalAction.ExternalLibraryId = externalLibraryId;
            }
            return !string.IsNullOrEmpty(externalLibraryId); //If the library id exists, library is in use.
        }

        private ExternalLibrary LibraryExistsInApp(string libraryName)
        {
            return configManager.Application.ExternalLibraries.Find(lib => lib.ExternalLibraryName.Equals(libraryName));
        }

        private bool CanUpdateLibrary(ExternalLibrary library, List<Type> exportedTypes)
        {
            bool bUpdateAssembly = false;

            //Get All external actions which belong to this library.
            List<ExternalAction> externalActions = (from act in configManager.Actions.OfType<ExternalAction>().Where(libId => libId.ExternalLibraryId == library.Id)
                                                    select act).ToList();

            List<ValidationResult> results = new List<ValidationResult>();
            bUpdateAssembly = true;
            List<string> externalActionsToBeRemoved = new List<string>();

            foreach (var externalAction in externalActions)
            {
                Type exportedType = exportedTypes.Where(type => type.IsClass && type.Name.Equals(externalAction.TypeName) && type.FullName.Equals(externalAction.TypeFullName)).FirstOrDefault();
                if (exportedType == null) //External Action doesn't exist in the updated Assembly.
                {
                    //Check whether this external action is used by a workflow. If it is used by any workflow, it cannot be removed.
                    Core.ActionProperties prop = new ActionProperties { ActionId = externalAction.Id, ActionName = externalAction.Name, ActionType = Constants.EXTERNAL_ACTION, TargetObject = string.Empty };
                    List<ValidationResult> actionExists = ValidationManager.ValidateDelete(prop);
                    if (actionExists.Count == 0)
                        externalActionsToBeRemoved.Add(externalAction.Id);

                    results.AddRange(actionExists);
                    bUpdateAssembly = bUpdateAssembly && results.Count == 0;
                }
                else
                    actionAlreadyInUseByApp.Add(exportedType);
            }

            if (bUpdateAssembly)
            {
                foreach (string act in externalActionsToBeRemoved)
                {
                    library.Actions.Remove(act); //Remove the action from the model as well as from the Grid, as this action is no longer exported by the Assembly.
                    ExternalLibGridModel row = gridModel.Where(r => r.ActionId.Equals(act)).FirstOrDefault();
                    if (row != null)
                        gridModel.Remove(row);
                }
            }
            else
            {
                ExceptionLogHelper.DebugLog("External library cannot be updated.");               
                StringBuilder builder = new StringBuilder(100);
                foreach(ValidationResult result in results)
                {
                    foreach (string msg in result.Messages)
                        builder.AppendLine(msg);
                }
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("EXTERNALACTIONVIEW_CanUpdateLibrary_ErrMsg"), resourceManager.GetResource("COMMON_Update_Text"), resourceManager.GetResource("EXTERNALACTIONVIEW_CanUpdateLibrary_Cap_ErrMsg"), 
                    builder.ToString(), Globals.ThisAddIn.Application.Hwnd);
                ExceptionLogHelper.DebugLog(builder.ToString());
            }

            return bUpdateAssembly;
        }
        
        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            actionAlreadyInUseByApp.Clear();

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "External Action Files(*.dll) | *.dll";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                txtFileName.Text = dialog.FileName;

                string fileNameWithPath = dialog.FileName;
                string onlyFileName = dialog.SafeFileName;

                ExternalLibrary externalLibrary = null;
                List<Type> exportedTypes = GetExportedTypesFromAssembly(fileNameWithPath);
                if (exportedTypes == null)
                    return;

                if ((externalLibrary = LibraryExistsInApp(onlyFileName)) != null)
                {
                    Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = ApttusMessageUtil.Show(String.Format(resourceManager.GetResource("EXTERNALACTIONVIEW_btnFileOpen_Click_InfoMsg"), externalLibrary.ExternalLibraryName),
                        resourceManager.GetResource("EXTERNALACTIONVIEW_btnFileOpen_Click_Cap_InfoMsg"), string.Empty, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
                        Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes | Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No,
                        string.Empty, Globals.ThisAddIn.Application.Hwnd);

                    if (result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No)
                        return;

                    bool bCanReload = CanUpdateLibrary(externalLibrary, exportedTypes);
                    if (!bCanReload)
                        return;
                }

                //Step 2. Extract all types in that assembly. It will give all public types and see if the library exports any actions 
                string externalLibraryId = externalLibrary == null ? string.Empty : externalLibrary.Id;

                if (externalLibrary == null)
                {
                    externalLibrary = new ExternalLibrary
                    {
                        ExternalLibraryName = onlyFileName,
                    };
                }

                bool isExternalLibraryInUse = PopulateActionsFromAssembly(exportedTypes, externalLibrary, ref externalLibraryId);

                if (isExternalLibraryInUse)
                {
                    // Step 3. Copy the assembly in the destination folder, so that it could be loaded and used at runtime
                    CopyAssemblyInDestinationFolder(fileNameWithPath, onlyFileName);

                    //Step 4. Embed the dll as part of app in metadata sheet.
                    EmbedAssemblyInMetadataSheet(fileNameWithPath, externalLibraryId);
                }
            }
        }

        private void EmbedAssemblyInMetadataSheet(string fileNameWithPath, string uniqueId)
        {
            Excel.Worksheet oSheet = ExcelHelper.GetWorkSheet(Constants.METADATA_SHEETNAME);
            //Remove the DLL if it already exists.
            Excel.Shape shape = ExcelHelper.GetShape(oSheet, uniqueId);
            if (shape != null)
                shape.Delete();
            ExcelHelper.AddObjectToSheet(oSheet, fileNameWithPath, uniqueId);
        }

        private void CopyAssemblyInDestinationFolder(string fileNameWithPath, string fileName)
        {
            string destinationFileNameWithPath = ExternalLibraryPath + Path.DirectorySeparatorChar + fileName;
            File.Copy(fileNameWithPath, destinationFileNameWithPath, true);
        }
        
        private void ExternalAction_Load(object sender, EventArgs e)
        {
            dgvExternalActions.AutoGenerateColumns = false;

            IEnumerable<ExternalAction> externalActions = configManager.Actions.OfType<ExternalAction>();
            if (externalActions.Count() > 0)
            {
                var list = from act in externalActions
                           select new ExternalLibGridModel { ExternalActionName = act.Name, 
                               ExternalLibName = configManager.Application.ExternalLibraries.Find(x => x.Id == act.ExternalLibraryId).ExternalLibraryName, 
                               ActionId  = act.Id };

                list.ToList().ForEach(x => gridModel.Add(x));
            }
            gridSource.DataSource = gridModel;
            dgvExternalActions.DataSource = gridSource;
        }
    }

    internal class ExternalLibGridModel
    {
        public string ActionId { get; set; }
        public string ExternalLibName { get; set; }
        public string ExternalActionName { get; set; }
    }
}
