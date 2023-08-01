/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class SearchAndSelectActionView : Form
    {
        SearchAndSelectActionController controller;
        public SearchAndSelectData searchAndSelectData = new SearchAndSelectData();
        public List<string> recordIds = new List<string>();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;


        public SearchAndSelectActionView()
        {
            InitializeComponent();
            SetCultureData();

            Icon = Properties.Resources.XA_AppBuilder_Icon;
        }
        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnCancel_Text");
            btnClear.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnClear_Text");
            btnNext.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnNext_Text");
            btnSearch.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_btnSearch_Text");
            chkSelectAll.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_chkSelectAll_Text");
            lblSearch.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_lblSearch_Text");
            llShowFilters.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_llShowFilters_Text");
            rbSelectAllGrid.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_rbSelectAllGrid_Text");
            rbSelectAllSalesforce.Text = string.Format(resourceManager.GetResource("SEARCHNSELECTVIEW_rbSelectAllSalesforce_Text"),resourceManager.CRMName);
        }
        public void SetController(SearchAndSelectActionController controller)
        {
            this.controller = controller;
        }

        #region "UI Event Handlers"

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Call User Input control to populate any values selected by Runtime user, those will be used Expression builder to prepare where clause
            if (controller.GetUserInputCount() > 0)
                userInputUserControl1.UpdateValuesInDataSet();

            QueryUI(txtSearch.Text, GetAdvancedSearchWhereClause(), false);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            txtSearch.Focus();

            // Clear selection of User Input fields
            ClearUserInput();

            // Clear previously selected recordIds, on Clear button
            recordIds.Clear();

            // Re-Query and Bind UI again
            QueryUI(string.Empty, string.Empty, true);
        }

        private void llShowFilters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SuspendLayout();
            try
            {
                //AdjustSearchPanelHeight(controller.GetUserInputCount());
                if (userInputUserControl1.Visible)
                {
                    userInputUserControl1.Visible = false;
                    // User has choose to hide Advanced search criteria, so reset filter values.
                    userInputUserControl1.ClearControls();
                    llShowFilters.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_llShowFilters_Text");
                    llShowFilters.Visible = true;
                }
                else
                {
                    userInputUserControl1.Visible = true;
                    llShowFilters.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_llHideFilters_Text");
                    llShowFilters.Visible = true;
                }
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void dgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnNext_Click(sender, e);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Specific case when more than 500 records are retrieved and user "Selected All in Salesforce"
            if (this.controller.model.RecordType.Equals(Constants.QUERYRESULT_MULTIPLE) && rbSelectAllSalesforce.Checked)
            {
                searchAndSelectData.SelectAllInSalesforce = true;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                switch (this.controller.model.RecordType)
                {
                    case Constants.QUERYRESULT_MULTIPLE:
                        // Since recordIds are now populated on Click, no need to loop through datagridrows here
                        // This is implemented to remember selected records between multiple search
                        //foreach (DataGridViewRow row in dgvResults.Rows)
                        //{
                        //    DataGridViewCheckBoxCell checkBoxCell = row.Cells[0] as DataGridViewCheckBoxCell;

                        //    if (Convert.ToBoolean(checkBoxCell.Value))
                        //        recordIds.Add(Constants.QUOTE + row.Cells[this.controller.appObject.IdAttribute].Value.ToString() + Constants.QUOTE);
                        //}
                        break;

                    case Constants.QUERYRESULT_SINGLE:
                        recordIds.Add(Constants.QUOTE + dgvResults.SelectedRows[0].Cells[this.controller.appObject.IdAttribute].Value.ToString() + Constants.QUOTE);
                        break;
                }

                if (recordIds.Count > 0)
                {
                    searchAndSelectData.SelectAllInSalesforce = false;
                    searchAndSelectData.selectedId = string.Join<string>(Constants.COMMA, recordIds);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SEARCHNSELECTVIEW_SelectRecord_ErrorMsg"), controller.appObject.Name), resourceManager.GetResource("SEARCHNSELECTVIEW_SelectRecordCap_ErrorMsg"));
                }
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvResults.RowCount; i++)
            {
                dgvResults.Rows[i].Cells[0].Value = chkSelectAll.Checked;
                PoppulateRecordIds(chkSelectAll.Checked, Constants.QUOTE + Convert.ToString(dgvResults.Rows[i].Cells[this.controller.appObject.IdAttribute].Value) + Constants.QUOTE);
            }

            if (chkSelectAll.Checked && dgvResults.RowCount == Constants.QUERYRESULT_RECORDLIMIT)
            {
                pnlSelectAll.Visible = true;
                dgvResults.Columns[0].ReadOnly = rbSelectAllSalesforce.Checked;
            }
            else
                pnlSelectAll.Visible = false;
        }

        private void rbSelectAllSalesforce_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSelectAllSalesforce.Checked)
                for (int i = 0; i < dgvResults.RowCount; i++)
                    dgvResults.Rows[i].Cells[0].Value = chkSelectAll.Checked;

            dgvResults.Columns[0].ReadOnly = rbSelectAllSalesforce.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        public void PrepareSearchUI()
        {
            // 1. Set the UI Title and Header Info
            lblTitle.Text = controller.model.Name;
            //this.lblSearch.Text = "Search Across Filters";  // + controller.appObject.Name;

            // 2. Set Single Vs Multi-Select mode
            if (this.controller.model.RecordType.Equals(Constants.QUERYRESULT_MULTIPLE))
                chkSelectAll.Visible = true;
            else
                chkSelectAll.Visible = false;

            // 3. Show/Hide Filters
            if (controller.GetUserInputCount() == 0)
                llShowFilters.Visible = false;

            // 4. Render and Set Default value for Advanced Search Controls
            // Since this first load , display default data set 
            QueryUI(string.Empty, string.Empty, true);

        }

        private void QueryUI(string globalSearchString, string advancedFilterWhereClause, bool bClear)
        {
            // Query and Bind results
            searchAndSelectData = controller.GetResultData(globalSearchString, advancedFilterWhereClause, bClear);
            DataTable resultData = searchAndSelectData.ResultDataTable;
            BindDataGrid(resultData);

            // Clear Select All checkbox
            if (this.controller.model.RecordType.Equals(Constants.QUERYRESULT_MULTIPLE))
            {
                chkSelectAll.Checked = false;
                pnlSelectAll.Visible = false;
                dgvResults.Columns[0].ReadOnly = false;
            }
            // Display record count
            string rowCount = resultData.Rows.Count >= Constants.QUERYRESULT_RECORDLIMIT ? resultData.Rows.Count.ToString() + "+" : resultData.Rows.Count.ToString();
            lblRecordCount.Text = rowCount + " " + controller.appObject.Name + " " + resourceManager.GetResource("SEARCHNSELECTVIEW_RecordRet_Text");

        }

        private void BindDataGrid(DataTable resultData)
        {
            if (resultData.Rows.Count > 0)
            {
                dgvResults.DataSource = resultData;

                // Show Checkbox Column
                if (this.controller.model.RecordType.Equals(Constants.QUERYRESULT_MULTIPLE))
                {
                    dgvResults.Columns[0].Visible = true;
                    dgvResults.Columns[0].Width = 35;
                }

                int colStart = this.controller.model.RecordType.Equals(Constants.QUERYRESULT_MULTIPLE) ? 1 : 0;
                for (int cnt = colStart; cnt < dgvResults.Columns.Count; cnt++)
                {
                    dgvResults.Columns[cnt].HeaderCell.Style.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
                    dgvResults.Columns[cnt].ReadOnly = true;
                    dgvResults.Columns[cnt].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    ResultField currentField = this.controller.model.ResultFields.Where(rf => rf.Id.Equals(dgvResults.Columns[cnt].HeaderText)).FirstOrDefault();
                    if (currentField != null)
                        dgvResults.Columns[cnt].HeaderText = currentField.HeaderName;
                    else if (currentField == null && dgvResults.Columns[cnt].HeaderText.IndexOf(Constants.CustomObjectReference__r) > 0)
                        dgvResults.Columns[cnt].HeaderText = dgvResults.Columns[cnt].HeaderText.Replace(Constants.CustomObjectReference__r, "");
                }

                // Hide Id column - always 2nd column after checkbox column (which is first)
                dgvResults.Columns[1].Visible = false;
            }
            else
            {
                dgvResults.DataSource = null;
            }
        }

        private string GetAdvancedSearchWhereClause()
        {
            string result = string.Empty;
            // ToDo: Evaluate this search string from the Advanced Filter controls
            return result;
        }

        internal void AdjustSearchPanelHeight(int userInputCount)
        {
            if (userInputCount == 0)
            {
                llShowFilters.Visible = false;
            }
            else if (userInputCount > 0)
            {
                tlpMain.RowStyles[2].SizeType = SizeType.AutoSize;
                //tlpMain.RowStyles[2].Height = userInputUserControl1.ControlPanelPrefferedHeight + 30;

                tlpMain.RowStyles[4].SizeType = SizeType.AutoSize;
                //tlpMain.RowStyles[4].Height = tlpMain.RowStyles[4].Height = tlpMain.RowStyles[4].Height - (10 + userInputCount);
                llShowFilters.Text = resourceManager.GetResource("SEARCHNSELECTVIEW_llHideFilters_Text");
                llShowFilters.Visible = true;
            }
        }

        private void userInputUserControl1_Load(object sender, EventArgs e)
        {
            int userInputCount = controller.GetUserInputCount();

            if (userInputCount > 0)
            {
                UserInputController inputController = new UserInputController(controller.model.Id, controller.model.SearchFilterGroups, this.controller.model.AllowSavingFilters, true);
                userInputUserControl1.setController(inputController);
                userInputUserControl1.PopulateControls(this.controller.model.Id, this.controller.model.SearchFilterGroups[0].Filters, true);
                AdjustSearchPanelHeight(userInputCount);
                setControlAlignmentInFilterGroup();
                userInputUserControl1.SetControlAlignmentInFilterGroup();
            }
        }

        /// <summary>
        /// This method will align search label and lables of filter group - if length of label search text is more than length of largest filter group label text then 
        /// we will add space in all odd placed filter labels to make it aligned with search label and vice versa.
        /// </summary>
        void setControlAlignmentInFilterGroup()
        {
            try
            {
                int maxlen = this.controller.model.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0).Max(x => x.SearchFilterLabel.Length);
                string searchfilterLabel = this.controller.model.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0 && item.SearchFilterLabel.Length == maxlen).FirstOrDefault().SearchFilterLabel;
                Label lbl = (Label)this.Controls.Find("lbl_" + searchfilterLabel, true).FirstOrDefault();

                if (lbl != null && lbl.Right > lblSearch.Right)
                {
                    while (lbl.Right > lblSearch.Right)
                    {
                        lblSearch.Text = " " + lblSearch.Text;
                    }
                }
                else
                {
                    foreach (var searchfilter in this.controller.model.SearchFilterGroups[0].Filters.Where(x => x.ValueType == ExpressionValueTypes.UserInput).Where((item, index) => index % 2 == 0))
                    {
                        lbl = (Label)userInputUserControl1.Controls.Find("lbl_" + searchfilter.SearchFilterLabel, true).FirstOrDefault();
                        if (lbl != null)
                        {
                            while (lbl.Right < lblSearch.Right)
                            {
                                lbl.Text = " " + lbl.Text;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow row = dgvResults.Rows[e.RowIndex];
                DataGridViewCheckBoxCell checkBoxCell = row.Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                PoppulateRecordIds(Convert.ToBoolean(checkBoxCell.EditedFormattedValue), Constants.QUOTE + row.Cells[this.controller.appObject.IdAttribute].Value.ToString() + Constants.QUOTE);
            }
        }

        internal void PoppulateRecordIds(bool addRecordIds, string recordId)
        {
            if (addRecordIds)
            {
                if (!recordIds.Exists(r => r.Equals(recordId)))
                    recordIds.Add(recordId);
            }
            else
                recordIds.Remove(recordId);
        }
        private void ClearUserInput()
        {
            if (controller.GetUserInputCount() > 0)
            {
                userInputUserControl1.ClearControls();
                userInputUserControl1.UpdateValuesInDataSet();
            }
        }

        private void SearchAndSelectActionView_Load(object sender, EventArgs e)
        {
            ClearUserInput();
            recordIds.Clear();
        }
    }
}
