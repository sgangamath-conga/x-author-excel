/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Apttus.XAuthor.Core;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class ApttusAddModifyActions : Form
    {
        enum ActionOperation
        {
            AddAction,
            EditAction,
            DeleteAction
        }

        private String actionType;
        SortableBindingList<ActionProperties> actionsBindingSource = null;
        ActionManager actionManager = ActionManager.GetInstance();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ApttusAddModifyActions(String action)
        {
            actionType = action;
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
           
        }

        private void SetCultureData()
        {
            btnAddAction.Text = resourceManager.GetResource("COMMON _btnCreate_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnDelete.Text = resourceManager.GetResource("COMMON_btnDelete_AMPText");
            btnEditAction.Text = resourceManager.GetResource("COMMON_btnEdit_AMPText");
            colActionName.HeaderText = resourceManager.GetResource("COMMON_ActionName_Text").Replace(':',' ');
            colType.HeaderText = resourceManager.GetResource("ADDMODACTION_colType_HeaderText");
            lblTitle.Text = resourceManager.GetResource("COMMON_Actions_Text");
        }
        private void ApttusAddModifyActions_Load(object sender, EventArgs e)
        {
            btnEditAction.Enabled = false;
            btnDelete.Enabled = false;
            InitializeGrid();
            if (String.IsNullOrEmpty(actionType))
                btnAddAction.Image = Properties.Resources.DownArrowActions;
            //actionManager.FormDataLoadingCompleted += ApttusAddModifyActions_FormDataLoadingCompleted;
            //Update the Form Title to reflect what to type of action is selected.
            //Text += String.IsNullOrEmpty(actionType) ? "Action" : actionType + " Action";
        }

        private void ApttusAddModifyActions_FormClosing(object sender, FormClosingEventArgs e)
        {
            //actionManager.FormDataLoadingCompleted -= ApttusAddModifyActions_FormDataLoadingCompleted;
        }

        private void SetupGridColumns()
        {
            //All columns are of string datatype. This will be useful when sorting comes into play.
            foreach (DataGridViewColumn column in grdAddModifyAction.Columns)
                column.ValueType = typeof(String);

            grdAddModifyAction.AutoGenerateColumns = false;
            grdAddModifyAction.Columns[0].Name = "ActionLabel";
            grdAddModifyAction.Columns[1].Name = "ActionType";
        }

        private void FillGrid()
        {
            if (ActionManager.GetInstance().GetActionsList(actionType).Count > 0)
            {
                SortableBindingList<ActionProperties> actionsBindingSource = new SortableBindingList<ActionProperties>(ActionManager.GetInstance().GetActionsList(actionType).OrderBy(a => a.ActionName));
                grdAddModifyAction.DataSource = actionsBindingSource;
            }
        }

        private void InitializeGrid()
        {
            SetupGridColumns();

            FillGrid();
        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(actionType))
                ShowAddActionContextMenu();
            else
                LoadAction(ActionOperation.AddAction);
        }

        /// <summary>
        /// Shows a context menu when AddAction button is clicked.
        /// </summary>
        private void ShowAddActionContextMenu()
        {
            LicenseActivationManager licenceManager = LicenseActivationManager.GetInstance;
            ContextMenuStrip strip = new ContextMenuStrip();

            strip.Opening += strip_Opening;
            strip.Closing += strip_Closing;
            strip.ItemClicked += strip_ItemClicked;


            stdole.IPictureDisp disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("CellsInsertDialog", 16, 16);
            ToolStripItem item = strip.Items.Add(resourceManager.GetResource("COMMON_AddRow_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.ADDROW_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.ADDROW_ACTION);
            int refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("ExportExcel", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnCheckIn_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.CHECK_IN_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable("Checkin"); //Don't replace the hard-coded string, it is used by LicenseManager
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("ImportExcel", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnCheckOut_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.CHECK_OUT_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable("Checkout"); //Don't replace the hard-coded string, it is used by LicenseManager
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("ReviewRejectChange", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("COMMON_Clear_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.CLEAR_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.CLEAR_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("DatabaseObjectDependencies", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("DATASETACTION_btnOK_Click_Cap_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.DataSetAction;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.DataSetAction);
            refCount = Marshal.ReleaseComObject(disp);


            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("CellsDelete", 16, 16);
            item = strip.Items.Add(Constants.DELETE_ACTION, Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.DELETE_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.DELETE_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("TableOfAuthoritiesInsert", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("COMMON_Display_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.RETRIEVE_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.RETRIEVE_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("FileSaveAsExcelXlsxMacro", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnMacroAction_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.MACRO_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.MACRO_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("Paste", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnPasteRow_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.PASTEROW_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable("Paste"); //Don't replace the hard-coded string, it is used by LicenseManager
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("MailMergeMatchFields", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("COMMON_PasteSourceData_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.PASTESOURCEDATA_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.PASTESOURCEDATA_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("QueryBuilder", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnExecuteQuery_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.EXECUTE_QUERY_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.EXECUTE_QUERY_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("MoviePlayAutomatically", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnCallProcedure_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.CALL_PROCEDURE_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.CALL_PROCEDURE_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("RecordsSaveRecord", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("COMMON_btnSave_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.SAVE_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.SAVE_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("SaveAttachments", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnSaveAttachmentAction_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.SAVE_ATTACHMENT_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.SAVE_ATTACHMENT_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("FileWorkflowTasks", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("RIBBON_btnSearchSelect_Label"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.SEARCH_AND_SELECT_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable("SearchandSelect"); //Don't replace the hard-coded string, it is used by LicenseManager
            refCount = Marshal.ReleaseComObject(disp); //all refCount values should be 0.

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("DirectRepliesTo", 16, 16);
            item = strip.Items.Add(resourceManager.GetResource("COMMON_SwitchConnection_Text"), Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            item.Tag = Constants.SWITCH_CONNECTION_ACTION;
            item.Enabled = licenceManager.IsFeatureAvailable(Constants.SWITCH_CONNECTION_ACTION);
            refCount = Marshal.ReleaseComObject(disp);

            //disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("TableDrawTable", 16, 16);
            //item = strip.Items.Add("Input Action", Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            //item.Tag = Constants.INPUT_ACTION;            
            //refCount = Marshal.ReleaseComObject(disp); //all refCount values should be 0.

            //disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("ImportSharePointList", 16, 16);
            //item = strip.Items.Add("Forms", Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal));
            //item.Tag = "Form";
            //refCount = Marshal.ReleaseComObject(disp);

            strip.Show(btnAddAction, new Point(0, btnAddAction.Height));
        }

        private void strip_Opening(object sender, CancelEventArgs e)
        {
            btnAddAction.Image = Properties.Resources.UpArrowActions;
        }

        private void strip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            btnAddAction.Image = Properties.Resources.DownArrowActions;
        }

        private void strip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            actionType = e.ClickedItem.Tag.ToString();
            LoadAction(ActionOperation.AddAction);
            this.Dispose();
        }

        private void btnEditAction_Click(object sender, EventArgs e)
        {
            LoadAction(ActionOperation.EditAction);
            this.Dispose();
        }        

        private void LoadAction(ActionOperation operation)
        {
            switch (operation)
            {
                case ActionOperation.EditAction:
                    actionManager.EditAction((ActionProperties)grdAddModifyAction.SelectedRows[0].DataBoundItem);
                    break;
                case ActionOperation.AddAction:
                    actionManager.AddAction(actionType);
                    break;
            }
        }

        private void grdAddModifyAction_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            btnEditAction.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)      
        {
            var model = (ActionProperties)grdAddModifyAction.SelectedRows[0].DataBoundItem;
            List<ValidationResult> results = ValidationManager.ValidateDelete(model);
            if (ValidationManager.VerifyResults(results))
            {
                if (ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("ADDMODACT_btnDelete_Click_WarnMsg"), model.ActionName), 
                                                        model.ActionName, ApttusMessageUtil.YesNo)
                    == TaskDialogResult.Yes)
                {                    
                    actionManager.DeleteAction((ActionProperties)grdAddModifyAction.SelectedRows[0].DataBoundItem);
                    this.Dispose();
                }
            }
            else
            {
                DisplayErrorViewer(results, resourceManager.GetResource("ADDMODACTION_btnDelete_Click_ErrorMsg"));
            }
        }

        /// <summary>
        /// Display Error Viewer
        /// </summary>
        /// <param name="results"></param>
        /// <param name="messageHeader"></param>
        private void DisplayErrorViewer(List<ValidationResult> results, string messageHeader)
        {
            ErrorMessageViewer viewer = new ErrorMessageViewer();
            viewer.ErrorMessageHeader = messageHeader;
            viewer.AddResults(results);
            viewer.ShowDialog();
            viewer = null;
        }
    }
}
