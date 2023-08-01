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
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class GenericListUIView : Form, IGenericListUIView
    {
        IGenericListUIController controller;
        private ListScreenType screenType;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public GenericListUIView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            grdCommon.AutoGenerateColumns = false;
        }

        private void SetCultureData()
        {
            btnAdd.Text = resourceManager.GetResource("COMMON_Create_AMPText");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnDelete.Text = resourceManager.GetResource("COMMON_btnDelete_AMPText");
            btnEdit.Text = resourceManager.GetResource("COMMON_btnEdit_AMPText");
        }

        public GenericListUIView(List<Button> customButtons)
            : this()
        {
            foreach (Button b in customButtons)
            {
                b.Font = new System.Drawing.Font("Segoe UI", 9.75f);
                b.UseVisualStyleBackColor = true;
                b.Height = 27;

                panelCustomButtons.Controls.Add(b);
            }
        }

        public void SetController(IGenericListUIController controller)
        {
            this.controller = controller;
            controller.SetView();
        }

        #region "Event Handler"

        private void btnAdd_Click(object sender, EventArgs e)
        {
            controller.Add();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            controller.Edit(grdCommon.SelectedRows);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            controller.Delete(grdCommon.SelectedRows);
        }

        public DataGridViewSelectedRowCollection GetSelectedRows()
        {
            return grdCommon.SelectedRows;
        }

        #endregion

        #region "Mediator Methods"

        public void SetViewTitle(ListScreenType sType)
        {
            screenType = sType;
            switch (screenType)
            {
                case ListScreenType.DisplayMap:
                    //lblHeader.Text = "Open or Create a Retrieve Map";
                    lblPageTitle.Text = resourceManager.GetResource("COMMON_DisplayMap_Text");
                    break;
                case ListScreenType.SaveMap:
                    //lblHeader.Text = "Open or Create a Save Map";
                    lblPageTitle.Text = resourceManager.GetResource("RIBBON_btnSaveMap_Label");
                    break;
                case ListScreenType.Actionflow:
                    //lblHeader.Text = "Open or Create Action Flow";
                    lblPageTitle.Text = resourceManager.GetResource("COMMON_ActionFlow_Text");
                    break;
                case ListScreenType.MatrixMap:
                    lblPageTitle.Text = resourceManager.GetResource("RIBBON_btnMatrixMap_Label");
                    break;
            }
        }

        public void BindGrid(List<GenericListUIModel> list)
        {
            if (list.Count > 0)
            {
                grdCommon.DataSource = list.OrderBy(a => a.Name).ToList();

                btnDelete.Enabled = true;
                btnEdit.Enabled = true;
            }
            else
            {
                grdCommon.DataSource = list.OrderBy(a => a.Name).ToList();
                btnDelete.Enabled = false;
                btnEdit.Enabled = false;
            }
        }

        #endregion

        #region "Private Methods"

        private void InitializeGrid()
        {
            DataGridViewColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "Name";
            nameColumn.Name = resourceManager.GetResource("COMMON_Name"); // "Name";
            nameColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            nameColumn.ReadOnly = true;
            nameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdCommon.Columns.Add(nameColumn);


            DataGridViewColumn listTypeColumn = new DataGridViewTextBoxColumn();
            listTypeColumn.DataPropertyName = "ListType";
            listTypeColumn.Name = resourceManager.GetResource("COMMON_Type"); // "Type";
            listTypeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            listTypeColumn.ValueType = typeof(ListScreenType);
            listTypeColumn.ReadOnly = true;
            listTypeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grdCommon.Columns.Add(listTypeColumn);

            if (screenType == ListScreenType.Actionflow)
            {
                DataGridViewCheckBoxColumn autoExecuteRegularModeCol = new DataGridViewCheckBoxColumn();
                autoExecuteRegularModeCol.FlatStyle = FlatStyle.Flat;
                autoExecuteRegularModeCol.ReadOnly = false;
                autoExecuteRegularModeCol.Name = "AutoExecuteRegularMode";
                autoExecuteRegularModeCol.HeaderText = resourceManager.GetResource("COMMON_AutoExecuteRegular_Text");  // "Auto Execute" + Environment.NewLine + " (Regular)";
                autoExecuteRegularModeCol.ValueType = typeof(bool);
                autoExecuteRegularModeCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                autoExecuteRegularModeCol.DataPropertyName = "AutoExecuteRegularMode";
                autoExecuteRegularModeCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                autoExecuteRegularModeCol.Resizable = DataGridViewTriState.True;
                autoExecuteRegularModeCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grdCommon.CellContentClick += grdCommon_CellContentClick;
                grdCommon.Columns.Add(autoExecuteRegularModeCol);

                DataGridViewCheckBoxColumn autoExecuteEditInExcelModeCol = new DataGridViewCheckBoxColumn();
                autoExecuteEditInExcelModeCol.FlatStyle = FlatStyle.Flat;
                autoExecuteEditInExcelModeCol.ReadOnly = false;
                autoExecuteEditInExcelModeCol.Name = "AutoExecuteEditInExcelMode";
                autoExecuteEditInExcelModeCol.HeaderText = string.Format(resourceManager.GetResource("COMMON_AutoExecuteSalesforce_Text"),resourceManager.CRMName);  // "Auto Execute" + Environment.NewLine + "(Launch from Salesforce)";
                autoExecuteEditInExcelModeCol.ValueType = typeof(bool);
                autoExecuteEditInExcelModeCol.SortMode = DataGridViewColumnSortMode.NotSortable;
                autoExecuteEditInExcelModeCol.DataPropertyName = "AutoExecuteEditInExcelMode";
                autoExecuteEditInExcelModeCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                autoExecuteEditInExcelModeCol.Resizable = DataGridViewTriState.True;
                autoExecuteEditInExcelModeCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grdCommon.CellContentClick += grdCommon_CellContentClick;
                grdCommon.Columns.Add(autoExecuteEditInExcelModeCol);
            }
        }

        void grdCommon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 2 | e.ColumnIndex == 3) && e.RowIndex != -1) //Auto Execute Column Index 2 (Regular) or 3 (Edit in Excel)
            {
                for (int i = 0; i < grdCommon.Rows.Count; ++i)
                {
                    if (e.RowIndex != i)
                        grdCommon.Rows[i].Cells[e.ColumnIndex].Value = false;
                }
                grdCommon.EndEdit();

                Guid AutoExecuteRegularModeWorkflowId = Guid.Empty;
                Guid AutoExecuteEditInExcelModeWorkflowId = Guid.Empty;

                foreach (DataGridViewRow row in grdCommon.Rows)
                {
                    // Auto Execute Regular Mode
                    if (Convert.ToBoolean(row.Cells["AutoExecuteRegularMode"].Value) == true)
                        AutoExecuteRegularModeWorkflowId = (row.DataBoundItem as GenericListUIModel).Id;
                    // Auto Execute Edit in Excel Mode
                    if (Convert.ToBoolean(row.Cells["AutoExecuteEditInExcelMode"].Value) == true)
                        AutoExecuteEditInExcelModeWorkflowId = (row.DataBoundItem as GenericListUIModel).Id;
                }

                SetAutoExecuteWorkflow(AutoExecuteRegularModeWorkflowId, AutoExecuteEditInExcelModeWorkflowId);

                //if (grdCommon.SelectedRows != null && grdCommon.SelectedRows.Count > 0)
                //{
                //    Guid workflowId = (grdCommon.SelectedRows[0].DataBoundItem as GenericListUIModel).Id;
                //    setAutoExecuteWorkflow(workflowId, (bool) grdCommon.Rows[e.RowIndex].Cells[2].Value);
                //}
            }
        }

        private void SetAutoExecuteWorkflow(Guid AutoExecuteRegularModeWorkflowId, Guid AutoExecuteEditInExcelModeWorkflowId)
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            foreach (WorkflowStructure wf in configManager.Workflows.OfType<WorkflowStructure>())
            {
                wf.Triggers = null;

                if (wf.Id.Equals(AutoExecuteRegularModeWorkflowId))
                    wf.AutoExecuteRegularMode = true;
                else
                    wf.AutoExecuteRegularMode = false;

                if (wf.Id.Equals(AutoExecuteEditInExcelModeWorkflowId))
                    wf.AutoExecuteEditInExcelMode = true;
                else
                    wf.AutoExecuteEditInExcelMode = false;
            }
        }

        //private void setAutoExecuteWorkflow(Guid workflowId, bool bSetAutoExecute)
        //{
        //    //Worst way of handling AutoExecute. Should be changed in near future. Can't do now because it will lead to change in config, hence existing apps with
        //    //customer will get affected.

        //    ConfigurationManager configManager = ConfigurationManager.GetInstance;
        //    foreach (WorkflowStructure wf in configManager.Workflows.OfType<WorkflowStructure>())
        //    {
        //        if (bSetAutoExecute && wf.Id.Equals(workflowId))
        //        {
        //            wf.Triggers = new HashSet<WorkflowStructure.Trigger>();
        //            wf.Triggers.Add(WorkflowStructure.Trigger.AppLoad);
        //        }
        //        else
        //        {
        //            wf.Triggers = null;
        //        }
        //    }
        //}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        private void GenericListUIView_Load(object sender, EventArgs e)
        {
            InitializeGrid();
        }

    }
}
