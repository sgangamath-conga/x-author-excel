using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataMigrationWizard : Form
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

        List<ApttusObject> ApttusObjects;

        DataMigrationController controller;
        DataMigrationModel Model;

        BackgroundWorker bgwRenderObjects;
        BackgroundWorker bgwFillForm;
        WaitWindowView waitWindow = null;
        bool isAppSaved = false;

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public DataMigrationWizard()
        {
            InitializeComponent();
            SetCultureData();
            SetDMDescription();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            isAppSaved = false;
        }

        void SetDMDescription()
        {
            richTextBox1.Text = resourceManager.GetResource("DM_DashaboardDescription_Text") + " ";

            LinkLabel link = new LinkLabel();
            link.Text = resourceManager.GetResource("DM_ClickHere_Text");
            link.AutoSize = true;
            link.Click += Link_Click;
            link.Location = this.richTextBox1.GetPositionFromCharIndex(this.richTextBox1.TextLength);
            this.richTextBox1.Controls.Add(link);
        }

        private void Link_Click(object sender, EventArgs e)
        {
            DataMigrationHelp DMHelp = new DataMigrationHelp();
            DMHelp.ShowDialog();
        }

        private void SetCultureData()
        {
            //lnkClickhere.Text = resourceManager.GetResource("DM_ClickHere_Text");
            btnFinish.Text = resourceManager.GetResource("DM_CreateApp_Text");
            btnLookups.Text = resourceManager.GetResource("DM_AddLookups_Text");
            btnObject.Text = resourceManager.GetResource("DM_AddObject_Text");
            btnSettings.Text = resourceManager.GetResource("DM_btnSettings_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            lblAppName.Text = resourceManager.GetResource("DM_AppName_Text");
            lblDataMigrationDashboard.Text = resourceManager.GetResource("DM_DashbordTitle_Text");
            //lblDMDescriptionText.Text = resourceManager.GetResource("DM_DashaboardDescription_Text");


            lblObject.Text = resourceManager.GetResource("DM_WizlblObject_Text");
            lblName.Text = resourceManager.GetResource("DM_WizlblName_Text");
            lblFields.Text = resourceManager.GetResource("DM_WizlblFields_Text");
            lblLookup.Text = resourceManager.GetResource("DM_WizlblLookup_Text");
            lblSequence.Text = resourceManager.GetResource("DM_WizlblSequence_Text");
            lblAction.Text = resourceManager.GetResource("DM_WizlblAction_Text");
            lblFilter.Text = resourceManager.GetResource("DM_WizlblFilter_Text");
            lblExternalID.Text = resourceManager.GetResource("DM_WizlblExternalID_Text");
            lblCopy.Text = resourceManager.GetResource("DM_WizlblCopy_Text");
            lblRemove.Text = resourceManager.GetResource("DM_WizlblRemove_Text");
            lblDisplay.Text = resourceManager.GetResource("COMMON_Display_Text");
        }

        public void SetController(DataMigrationController controller, DataMigrationModel Model)
        {
            this.controller = controller;
            this.Model = Model;
            this.controller.SetView();
        }

        internal void FillForm(DataMigrationModel model)
        {
            try
            {
                bgwFillForm = new BackgroundWorker();
                bgwFillForm.DoWork += bgwFillForm_DoWork;
                bgwFillForm.RunWorkerAsync(model);
                bgwFillForm.RunWorkerCompleted += bgwFillForm_RunWorkerCompleted;
                waitWindow = new WaitWindowView(resourceManager.GetResource("DM_FetchingObjectDetail_Text"), true);
                waitWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                waitWindow.CloseWaitWindow();
                ApttusObjectManager.HandleError(ex);
            }
            finally
            {
                waitWindow = null;
                bgwFillForm.Dispose();
                bgwFillForm = null;
            }
        }

        void SetAppName()
        {
            txtAppName.Text = Model.AppName;
            txtAppName.Enabled = false;
        }

        #region events


        void bgwFillForm_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RenderMigrationObjectsInUI((e.Result as DataMigrationModel).MigrationObjects);
            SetAppName();
            btnFinish.Text = resourceManager.GetResource("DM_UpdateApp_Text");
            waitWindow.CloseWaitWindow();
        }

        void bgwFillForm_DoWork(object sender, DoWorkEventArgs e)
        {
            ApttusObjects = controller.GetAllApptusObjects();
            e.Result = e.Argument;
        }

        private void btnObject_Click(object sender, EventArgs e)
        {
            ApttusObjects = controller.GetAllApptusObjects();
            List<ApttusObject> selectedObjects = new List<ApttusObject>();
            selectedObjects = ApttusObjects.Join(Model.MigrationObjects, a => a.Id, b => b.ObjectId, (a, b) => new { selectedvalues = a }).Select(x => x.selectedvalues).Distinct().ToList();
            List<ApttusObject> sourceObjects = ApttusObjects.Except(selectedObjects).ToList();
            List<ApttusObject> previousSelectedObjects = new List<ApttusObject>(selectedObjects);

            using (MultiSelectListForm view = new MultiSelectListForm(sourceObjects, new List<ApttusObject>(), resourceManager.GetResource("DM_AddObject_Text")))
            {
                if (view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<ApttusObject> recentSelectedObjects = ((List<ApttusObject>)(view.SelectedListSource.List)).Except(previousSelectedObjects).ToList();
                    RenderData(recentSelectedObjects);
                }
            }
        }

        void RenderData(List<ApttusObject> recentSelectedObjects)
        {
            try
            {
                bgwRenderObjects = new BackgroundWorker();
                bgwRenderObjects.DoWork += bgwRenderObjects_DoWork;
                bgwRenderObjects.RunWorkerAsync(recentSelectedObjects);
                bgwRenderObjects.RunWorkerCompleted += bgwRenderObjects_RunWorkerCompleted;
                waitWindow = new WaitWindowView(resourceManager.GetResource("DM_FetchingObjectDetail_Text"), true);
                waitWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                waitWindow.CloseWaitWindow();
                ApttusObjectManager.HandleError(ex);
            }
            finally
            {
                waitWindow = null;
                bgwRenderObjects.Dispose();
                bgwRenderObjects = null;
            }
        }

        void bgwRenderObjects_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            waitWindow.CloseWaitWindow();
            RenderMigrationObjectsInUI(e.Result as List<MigrationObject>);
        }

        void bgwRenderObjects_DoWork(object sender, DoWorkEventArgs e)
        {
            List<ApttusObject> recentSelectedObjects = e.Argument as List<ApttusObject>;
            e.Result = controller.PopulateObjects(recentSelectedObjects);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void btnClone_Click(object sender, EventArgs e)
        {
            UpdateData();

            MigrationObject migrationObject = (sender as Label).Tag as MigrationObject;

            MigrationObject newmigrationObject = controller.CloneMigrationObject(migrationObject);

            tblExpressions.SuspendLayout();

            try
            {
                CreateExpressionRow(newmigrationObject);
            }
            finally
            {
                tblExpressions.ResumeLayout();
                tblExpressions.PerformLayout();
            }

            newmigrationObject.Sequence = Model.MigrationObjects.Max(x => x.Sequence) + 1;

            UpdateUI();
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(resourceManager.GetResource("DM_ObjectRemovalConfirmation_Text"), resourceManager.GetResource("DM_ObjectRemovalConfirmationCaption_Text"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                List<HoldingCell> tempHolding = new List<HoldingCell>();
                HoldingCell cell;

                int row = tblExpressions.GetRow(sender as Label);

                MigrationObject migrationObjectToDelete = (sender as Label).Tag as MigrationObject;

                if (!controller.CanDeleteMigrationObject(migrationObjectToDelete))
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("DM_FilterDependencyForObjectRemoval_Text"), resourceManager.GetResource("DM_ObjectRemovalConfirmationCaption_Text"));
                    return;
                }

                // If the last row is deleted, then reset the expression builder
                if (row == 1 && tblExpressions.RowCount == 2)
                {
                    ResetExpressionBuilder();
                }
                else
                {
                    tblExpressions.SuspendLayout();

                    try
                    {
                        // Delete all controls on selected row 
                        for (int columnIndex = 0; columnIndex < tblExpressions.ColumnCount; columnIndex++)
                        {
                            var control = tblExpressions.GetControlFromPosition(columnIndex, row);
                            tblExpressions.Controls.Remove(control);
                        }

                        // Temporarily store the positions of all controls below the selected row
                        for (Int32 rowholding = row + 1; rowholding <= tblExpressions.RowCount; rowholding++)
                        {
                            for (Int32 col = 0; col <= tblExpressions.ColumnCount - 1; col++)
                            {
                                cell = new HoldingCell();
                                cell.cntrl = tblExpressions.GetControlFromPosition(col, rowholding);
                                //setup position for restore = current row -1
                                cell.tableLayoutPanelCellPosition = new TableLayoutPanelCellPosition(col, rowholding - 1);
                                tempHolding.Add(cell);
                            }
                        }

                        // Adjust control positions of all controls below the selected row
                        foreach (HoldingCell holdingCell in tempHolding)
                        {
                            cell = holdingCell;
                            if (cell.cntrl != null)
                            {
                                tblExpressions.SetCellPosition(cell.cntrl, cell.tableLayoutPanelCellPosition);
                            }
                        }
                        tempHolding = null;

                        // Delete the row and reduce the RowCount
                        tblExpressions.RowCount--;

                        //Delete Migartion Object from Model
                        controller.DeleteMigrationObject(migrationObjectToDelete);
                        UpdateUI();
                    }
                    finally
                    {
                        tblExpressions.ResumeLayout();
                        tblExpressions.PerformLayout();
                    }
                }
            }

        }

        void btnFilter_Click(object sender, EventArgs e)
        {
            MigrationObject migrationObject = (sender as Label).Tag as MigrationObject;
            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            using (DMExpressionBuilderHost DMView = new DMExpressionBuilderHost(apttusObject, migrationObject))
            {
                DMView.ShowDialog();
            }
        }

        private void lnkFields_Click(object sender, EventArgs e)
        {
            MigrationObject migrationObject = (sender as LinkLabel).Tag as MigrationObject;
            DataMigrationFieldsViewer Sfview = new DataMigrationFieldsViewer(migrationObject, controller);
            Sfview.ShowDialog();
            UpdateUI();
        }

        void lnkLookups_Click(object sender, EventArgs e)
        {
            UpdateData();
            MigrationObject migrationObject = (sender as LinkLabel).Tag as MigrationObject;
            DataMigrationLookupViewer Sfview = new DataMigrationLookupViewer(migrationObject, controller, Model);
            Sfview.ShowDialog();
            controller.ResolveObjectDependency();
            UpdateUI();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            UpdateData();

            string errMessage = ValidateAppSave();
            if (!string.IsNullOrEmpty(errMessage))
            {
                MessageBox.Show(errMessage, resourceManager.GetResource("DM_ErrorInSavingAppCaption_Text"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (controller.FormOpenMode == FormOpenMode.Create)
            {
                BaseApplicationController applicationController = Globals.ThisAddIn.GetApplicationController();
                if (applicationController.AppNameExists(txtAppName.Text))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("COREAPPCONTL_AppExists_ErrorMsg"), resourceManager.GetResource("COREAPPCONTL_AppCreatCap_ErrorMsg"));
                    return;
                }

                CreateNewMigrationApp();
            }
            else if (controller.FormOpenMode == FormOpenMode.Edit)
                EditMigrationApp();



            isAppSaved = true;
            Close();
        }


        private void EditMigrationApp()
        {
            controller.EditMigrationApp();
        }

        private void CreateNewMigrationApp()
        {
            controller.CreateMigrationApp(txtAppName.Text);
        }

        string ValidateAppSave()
        {
            if (string.IsNullOrEmpty(txtAppName.Text))
                return resourceManager.GetResource("DM_AppNameValidation_Text");
            if (applicationDefinitionManager.AppObjects.Count == 0)
                return resourceManager.GetResource("DM_AtleastOneObjectNeededValidation_Text");
            if (Model.MigrationObjects.Exists(x => string.IsNullOrEmpty(x.ExternalID)))
                return resourceManager.GetResource("DM_ExternalIDValidation_Text");
            if (!CheckIfSequenceNumberUnique())
                return resourceManager.GetResource("DM_SequenceNumberUniqueValidation_Text");
            if (!CheckIfObjectNamesUnique())
                return resourceManager.GetResource("DM_UniqueObjectNameValidation_Text");
            if (!CheckAllObjectNamesExists())
                return resourceManager.GetResource("DM_ObjectNameValidation_Text");
            string seqMessage = CheckValidObjectSequence();
            if (!string.IsNullOrEmpty(seqMessage))
                return seqMessage;
            if (!checkIfSequenceIncrementedByOne())
                return resourceManager.GetResource("DM_Increment_by_one_ErrMsg");

            return string.Empty;
        }

        private bool CheckAllObjectNamesExists()
        {
            return !Model.MigrationObjects.Exists(x => string.IsNullOrEmpty(x.ObjectName));
        }

        string CheckValidObjectSequence()
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (MigrationObject migrationObject in Model.MigrationObjects)
            {
                foreach (MigrationObject dependentObject in migrationObject.Dependencies)
                {
                    if (dependentObject.Sequence > migrationObject.Sequence)
                    {
                        sb.AppendFormat(resourceManager.GetResource("DM_SequenceNumberValidation_Text"), migrationObject.ObjectName, dependentObject.ObjectName, i);
                        i++;
                    }
                }
            }

            return sb.ToString();
        }
        bool CheckIfObjectNamesUnique()
        {
            List<string> query = Model.MigrationObjects.GroupBy(x => x.ObjectName)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
            {
                return false;
            }

            return true;
        }

        bool CheckIfSequenceNumberUnique()
        {
            List<int> query = Model.MigrationObjects.GroupBy(x => x.Sequence)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
            {
                return false;
            }

            return true;
        }

        bool checkIfSequenceIncrementedByOne()
        {
            int minSequence = Model.MigrationObjects.Min(o => o.Sequence);
            int maxSequence = Model.MigrationObjects.Max(o => o.Sequence);

            return minSequence == 1 && maxSequence == Model.MigrationObjects.Count;
        }

        #endregion

        void RenderMigrationObjectsInUI(List<MigrationObject> recentMigrationObject)
        {
            try
            {
                tblExpressions.SuspendLayout();

                foreach (MigrationObject migrationObject in recentMigrationObject)
                    CreateExpressionRow(migrationObject);

                UpdateUI();
            }
            finally
            {
                tblExpressions.ResumeLayout(true);
                tblExpressions.PerformLayout();
            }
        }

        public void CreateExpressionRow(MigrationObject migrationObject)
        {
            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            int rowNo = tblExpressions.RowCount;

            // Column 1 - Object Name
            Label lblObject = new Label();
            lblObject.AutoSize = true;
            lblObject.Text = apttusObject.Name;
            lblObject.CreateControl();

            // Column 2 - Object and Field Text
            TextBox txtObject = new TextBox();
            txtObject.Dock = DockStyle.Fill;
            txtObject.BackColor = Color.White;
            txtObject.Text = migrationObject.ObjectName;
            txtObject.CreateControl();
            txtObject.Enabled = GetEnableStatus(migrationObject);

            // Column 3 - Fields
            LinkLabel lnkFields = new LinkLabel();
            lnkFields.Text = migrationObject.Fields.Count.ToString();
            lnkFields.TextAlign = ContentAlignment.MiddleCenter;
            lnkFields.Click += lnkFields_Click;
            lnkFields.Tag = migrationObject;
            lnkFields.CreateControl();

            // Column 4 - Lookups
            Control lnkLookups;
            int Lookupcounts = controller.GetLookUPCount(apttusObject, Model);
            if (Lookupcounts > 0)
            {
                lnkLookups = new LinkLabel();
                ((LinkLabel)lnkLookups).TextAlign = ContentAlignment.MiddleCenter;
                lnkLookups.Text = Lookupcounts.ToString();
                lnkLookups.Click += lnkLookups_Click;
                lnkLookups.Tag = migrationObject;
                lnkLookups.CreateControl();
            }
            else
            {
                lnkLookups = new Label();
                ((Label)lnkLookups).TextAlign = ContentAlignment.MiddleCenter;
                lnkLookups.Text = Lookupcounts.ToString();
                lnkLookups.CreateControl();
            }
            // Column 5 - Sequence
            TextBox txtSequence = new TextBox();
            txtSequence.Dock = DockStyle.Fill;
            txtSequence.BackColor = Color.White;
            txtSequence.Text = migrationObject.Sequence.ToString(); // migrationObject.Sequence.ToString();
            txtSequence.TextAlign = HorizontalAlignment.Center;
            txtSequence.CreateControl();

            // Column 6 - Action
            ComboBox cboActions = new ComboBox();
            cboActions.Dock = DockStyle.Fill;
            cboActions.FlatStyle = FlatStyle.Standard;
            cboActions.DropDownStyle = ComboBoxStyle.DropDownList;
            cboActions.Items.Add("Search & Select");
            cboActions.Items.Add("Query");
            cboActions.CreateControl();
            cboActions.Enabled = GetEnableStatus(migrationObject);

            // Column 7 - Filter
            Label btnFilter = new Label();
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.BackColor = Color.Transparent;
            btnFilter.Image = Properties.Resources.levels;
            btnFilter.Tag = migrationObject;
            btnFilter.Click += btnFilter_Click;
            btnFilter.Size = new System.Drawing.Size(24, 24);
            btnFilter.ImageAlign = ContentAlignment.MiddleCenter;
            btnFilter.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            var margin = btnFilter.Margin;
            margin.All = 2;
            btnFilter.Margin = margin;
            Control lblExternalID;
            // Column 6 - External ID
            if (controller.GetAllExternalIDs(apttusObject) > 1)
            {
                LinkLabel linkExternalID = new LinkLabel();
                linkExternalID.AutoSize = true;
                linkExternalID.Text = "Not Set";

                linkExternalID.Links[0].LinkData = migrationObject;
                linkExternalID.LinkClicked += DataMigrationWizard_LinkClicked;

                linkExternalID.CreateControl();

                lblExternalID = linkExternalID;
                (lblExternalID as LinkLabel).TextAlign = ContentAlignment.MiddleCenter;
                lblExternalID.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            }
            else
            {
                lblExternalID = new Label();
                lblExternalID.AutoSize = true;
                lblExternalID.Text = controller.IsExternalIDExists(apttusObject) ? "Set" : "Not Set";
                lblExternalID.CreateControl();
                (lblExternalID as Label).TextAlign = ContentAlignment.MiddleCenter;
                lblExternalID.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            }

            SetTooltipForExternalID(migrationObject, lblExternalID);
            lblExternalID.Enabled = GetEnableStatus(migrationObject) && !migrationObject.IsCloned;

            // Column 8 - Clone
            Label btnClone = new Label();
            btnClone.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            btnClone.FlatStyle = FlatStyle.Flat;
            btnClone.BackColor = Color.Transparent;

            stdole.IPictureDisp disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("Copy", 16, 16);
            btnClone.Image = Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal);

            //btnClone.Size = new System.Drawing.Size(24, 24);
            btnClone.ImageAlign = ContentAlignment.MiddleCenter;
            //btnClone.Anchor = AnchorStyles.None;
            btnClone.Click += btnClone_Click;
            btnClone.Tag = migrationObject;
            btnClone.Margin = margin;

            // Column 7 - Delete
            Label btnDelete = new Label();
            btnDelete.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.BackColor = Color.Transparent;

            disp = Globals.ThisAddIn.Application.CommandBars.GetImageMso("AdpDiagramDeleteTable", 16, 16);
            btnDelete.Image = Image.FromHbitmap((IntPtr)disp.Handle, (IntPtr)disp.hPal);

            //btnDelete.Size = new System.Drawing.Size(30, 24);
            btnDelete.ImageAlign = ContentAlignment.MiddleCenter;
            //btnDelete.Anchor = AnchorStyles.None;
            btnDelete.Click += btnDelete_Click;
            btnDelete.Tag = migrationObject;
            btnDelete.Margin = margin;

            // Create Work Sheet
            CheckBox chkCreateWorkSheet = new CheckBox();
            chkCreateWorkSheet.FlatStyle = FlatStyle.Standard;
            chkCreateWorkSheet.BackColor = Color.Transparent;
            chkCreateWorkSheet.Margin = margin;
            chkCreateWorkSheet.CheckAlign = ContentAlignment.MiddleCenter;
            chkCreateWorkSheet.Enabled = GetEnableStatus(migrationObject);

            tblExpressions.Controls.Add(lblObject, 0, rowNo);
            tblExpressions.Controls.Add(txtObject, 1, rowNo);
            tblExpressions.Controls.Add(lnkFields, 2, rowNo);
            tblExpressions.Controls.Add(lnkLookups, 3, rowNo);
            tblExpressions.Controls.Add(txtSequence, 4, rowNo);
            tblExpressions.Controls.Add(cboActions, 5, rowNo);
            tblExpressions.Controls.Add(btnFilter, 6, rowNo);
            tblExpressions.Controls.Add(lblExternalID, 7, rowNo);
            tblExpressions.Controls.Add(btnClone, 8, rowNo);
            tblExpressions.Controls.Add(btnDelete, 9, rowNo);
            tblExpressions.Controls.Add(chkCreateWorkSheet, 10, rowNo);

            tblExpressions.RowCount++;
        }

        public bool GetEnableStatus(MigrationObject obj)
        {
            if (controller.FormOpenMode == FormOpenMode.Create)
                return true;
            else if (controller.FormOpenMode == FormOpenMode.Edit)
                return !ConfigurationManager.GetInstance.Application.MigrationModel.MigrationObjects.Exists(o => o.Id == obj.Id);
            return true;
        }

        private void SetTooltipForExternalID(MigrationObject migrationObject, Control lblExternalID)
        {
            var externalid = migrationObject.Fields.Where(x => x.FieldId == migrationObject.ExternalID).FirstOrDefault();
            string tooltip = externalid != null ? externalid.FieldName : string.Empty;

            toolTip1.SetToolTip(lblExternalID, tooltip);
        }

        private void DataMigrationWizard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataMigrationExternalIDViewer DM = new DataMigrationExternalIDViewer((MigrationObject)e.Link.LinkData, controller, Model);
            DM.ShowDialog();
        }

        public void ResetExpressionBuilder()
        {
            try
            {
                tblExpressions.SuspendLayout();

                while (tblExpressions.RowCount > 1)
                {
                    int row = tblExpressions.RowCount - 1;
                    for (int i = 0; i < tblExpressions.ColumnCount; i++)
                    {
                        Control c = tblExpressions.GetControlFromPosition(i, row);
                        if (c != null)
                        {
                            tblExpressions.Controls.Remove(c);
                            c.Dispose();
                        }
                    }

                    tblExpressions.RowCount--;
                }

            }
            finally
            {
                tblExpressions.ResumeLayout();
                tblExpressions.PerformLayout();
            }
        }

        internal void UpdateUI()
        {
            int row = 1;

            tblExpressions.SuspendLayout();
            try
            {
                foreach (MigrationObject migrationObject in Model.MigrationObjects)
                {

                    row++;
                    ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

                    LinkLabel fieldControl = tblExpressions.GetControlFromPosition(2, row) as LinkLabel;
                    if (fieldControl != null)
                        fieldControl.Text = migrationObject.Fields.Count.ToString();

                    Control lnkLookups = tblExpressions.GetControlFromPosition(3, row) as Control;
                    int Lookupcounts = controller.GetLookUPCount(apttusObject, Model);
                    if (lnkLookups != null)
                    {



                        tblExpressions.Controls.Remove(lnkLookups);

                        if (Lookupcounts > 0)
                        {
                            lnkLookups = new LinkLabel();
                            ((LinkLabel)lnkLookups).TextAlign = ContentAlignment.MiddleCenter;
                            lnkLookups.Click += lnkLookups_Click;
                            lnkLookups.Tag = migrationObject;
                        }
                        else
                        {
                            lnkLookups = new Label();
                            ((Label)lnkLookups).TextAlign = ContentAlignment.MiddleCenter;
                        }

                        lnkLookups.Text = Lookupcounts.ToString();
                        lnkLookups.CreateControl();

                        tblExpressions.Controls.Add(lnkLookups, 3, row);

                    }

                    TextBox txtSequence = tblExpressions.GetControlFromPosition(4, row) as TextBox;
                    if (txtSequence != null)
                        txtSequence.Text = migrationObject.Sequence.ToString();

                    Label lblExternalID = tblExpressions.GetControlFromPosition(7, row) as Label;
                    if (lblExternalID != null)
                    {
                        if (controller.GetAllExternalIDs(apttusObject) > 1)
                        {
                            if (string.IsNullOrEmpty(migrationObject.ExternalID))
                            {
                                lblExternalID.Text = "Not Set";
                            }
                            else
                            {
                                lblExternalID.Text = "Set";
                            }
                        }
                        else
                        {
                            lblExternalID.Text = controller.IsExternalIDExists(apttusObject) ? "Set" : "Not Set";
                        }
                        SetTooltipForExternalID(migrationObject, lblExternalID);
                    }

                    ComboBox cboActions = tblExpressions.GetControlFromPosition(5, row) as ComboBox;
                    if (cboActions != null)
                    {
                        if (migrationObject.DataRetrievalAction != null && migrationObject.DataRetrievalAction.Action != ActionType.None)
                        {
                            if (migrationObject.DataRetrievalAction.Action == ActionType.Query)
                            {
                                cboActions.SelectedIndex = 1;
                            }
                            else
                            {
                                cboActions.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (txtSequence.Text == "1")
                                cboActions.SelectedIndex = 0;
                            else
                                cboActions.SelectedIndex = 1;
                        }
                    }

                    CheckBox chkCreateWorksheet = tblExpressions.GetControlFromPosition(10, row) as CheckBox;
                    if (chkCreateWorksheet != null)
                    {
                        chkCreateWorksheet.Checked = migrationObject.CreateWorksheet;
                    }

                    controller.BuildQueryFilters(migrationObject);
                }
            }
            finally
            {
                tblExpressions.ResumeLayout();
                tblExpressions.PerformLayout();
            }
        }

        private void DataMigrationWizard_Load(object sender, EventArgs e)
        {
            typeof(TableLayoutPanel).InvokeMember("DoubleBuffered",
             BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, tblExpressions, new object[] { true });
        }

        private void UpdateData()
        {
            for (int row = 2; row < tblExpressions.RowCount; ++row)
            {
                MigrationObject obj = tblExpressions.GetControlFromPosition(2, row).Tag as MigrationObject;

                TextBox txtObjName = tblExpressions.GetControlFromPosition(1, row) as TextBox;
                if (txtObjName != null)
                {
                    obj.ObjectName = txtObjName.Text;
                }

                TextBox txtSequence = tblExpressions.GetControlFromPosition(4, row) as TextBox;
                if (txtSequence != null)
                    obj.Sequence = Int32.Parse(txtSequence.Text);

                ComboBox cboAction = tblExpressions.GetControlFromPosition(5, row) as ComboBox;
                if (cboAction != null)
                    obj.DataRetrievalAction.Action = cboAction.SelectedIndex == 0 ? ActionType.SearchAndSelect : ActionType.Query;

                CheckBox chkCreateWorksheet = tblExpressions.GetControlFromPosition(10, row) as CheckBox;
                if (chkCreateWorksheet != null)
                {
                    obj.CreateWorksheet = chkCreateWorksheet.Checked;
                }
            }
        }

        private void btnLookups_Click(object sender, EventArgs e)
        {
            DMDependencyIdentifier DM = new DMDependencyIdentifier();
            DM.SetController(controller, Model);
            DM.ShowDialog();
        }

        private void DataMigrationWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAppSaved)
            {
                if (MessageBox.Show(resourceManager.GetResource("DM_WizardCloseConfirmation_Text"), resourceManager.GetResource("DM_ObjectRemovalConfirmationCaption_Text"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                controller.ResetAppDefManager();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            DataMigrationSettings settings = new DataMigrationSettings(Model);
            settings.ShowDialog();
        }

        private void tblExpressions_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
