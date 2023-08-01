namespace Apttus.XAuthor.AppRuntime
{
    partial class SearchAndSelectActionView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.tlpSearch = new System.Windows.Forms.TableLayoutPanel();
			   this.userInputUserControl1 = new Apttus.XAuthor.AppRuntime.UserInputUserControl();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblSearch = new System.Windows.Forms.Label();
            this.llShowFilters = new System.Windows.Forms.LinkLabel();
            this.pnlResultHeader = new System.Windows.Forms.TableLayoutPanel();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.pnlSelectAll = new System.Windows.Forms.FlowLayoutPanel();
            this.rbSelectAllGrid = new System.Windows.Forms.RadioButton();
            this.rbSelectAllSalesforce = new System.Windows.Forms.RadioButton();
            this.pnlAction = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlResults = new System.Windows.Forms.TableLayoutPanel();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.MultiSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tlpMain.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.tlpSearch.SuspendLayout();
            this.pnlResultHeader.SuspendLayout();
            this.pnlSelectAll.SuspendLayout();
            this.pnlAction.SuspendLayout();
            this.pnlResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.AutoSize = true;
            this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMain.Controls.Add(this.pnlSearch, 0, 1);
			this.tlpMain.Controls.Add(this.userInputUserControl1, 0, 2);
            this.tlpMain.Controls.Add(this.pnlResultHeader, 0, 3);
            this.tlpMain.Controls.Add(this.pnlAction, 0, 5);
            this.tlpMain.Controls.Add(this.lblTitle, 0, 0);
            this.tlpMain.Controls.Add(this.pnlResults, 0, 4);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 6;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.Size = new System.Drawing.Size(716, 476);
            this.tlpMain.TabIndex = 0;
            // 
            // pnlSearch
            // 
            this.pnlSearch.AutoSize = true;
            this.pnlSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearch.Controls.Add(this.tlpSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(3, 33);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(710, 29);
            this.pnlSearch.TabIndex = 0;
            // 
            // tlpSearch
            // 
            this.tlpSearch.AutoSize = true;
            this.tlpSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpSearch.ColumnCount = 7;
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSearch.Controls.Add(this.btnSearch, 4, 0);
            this.tlpSearch.Controls.Add(this.txtSearch, 1, 0);
            this.tlpSearch.Controls.Add(this.btnClear, 5, 0);
            this.tlpSearch.Controls.Add(this.lblSearch, 0, 0);
            this.tlpSearch.Controls.Add(this.llShowFilters, 6, 0);
            this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearch.Location = new System.Drawing.Point(0, 0);
            this.tlpSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tlpSearch.Name = "tlpSearch";
            this.tlpSearch.RowCount = 1;
            this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSearch.Size = new System.Drawing.Size(710, 29);
            this.tlpSearch.TabIndex = 16;
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(404, 3);
            this.btnSearch.MaximumSize = new System.Drawing.Size(85, 23);
            this.btnSearch.MinimumSize = new System.Drawing.Size(85, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(85, 23);
            this.btnSearch.TabIndex = 16;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(129, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(269, 23);
            this.txtSearch.TabIndex = 4;
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(495, 3);
            this.btnClear.MaximumSize = new System.Drawing.Size(85, 23);
            this.btnClear.MinimumSize = new System.Drawing.Size(85, 23);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(85, 23);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSearch.Location = new System.Drawing.Point(3, 3);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(3);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(120, 15);
            this.lblSearch.TabIndex = 5;
            this.lblSearch.Text = "Search Across Filters :";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // llShowFilters
            // 
            this.llShowFilters.AutoSize = true;
            this.llShowFilters.Location = new System.Drawing.Point(586, 3);
            this.llShowFilters.Margin = new System.Windows.Forms.Padding(3);
            this.llShowFilters.Name = "llShowFilters";
            this.llShowFilters.Size = new System.Drawing.Size(70, 15);
            this.llShowFilters.TabIndex = 19;
            this.llShowFilters.TabStop = true;
            this.llShowFilters.Text = "Show Filters";
            this.llShowFilters.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llShowFilters_LinkClicked);
            // 
            // pnlResultHeader
            // 
            this.pnlResultHeader.AutoSize = true;
            this.pnlResultHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlResultHeader.ColumnCount = 3;
            this.pnlResultHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlResultHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlResultHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlResultHeader.Controls.Add(this.chkSelectAll, 0, 0);
            this.pnlResultHeader.Controls.Add(this.lblRecordCount, 2, 0);
            this.pnlResultHeader.Controls.Add(this.pnlSelectAll, 1, 0);
            this.pnlResultHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResultHeader.Location = new System.Drawing.Point(3, 68);
            this.pnlResultHeader.Name = "pnlResultHeader";
            this.pnlResultHeader.RowCount = 1;
            this.pnlResultHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlResultHeader.Size = new System.Drawing.Size(710, 25);
            this.pnlResultHeader.TabIndex = 0;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(3, 3);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(74, 19);
            this.chkSelectAll.TabIndex = 15;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRecordCount.Location = new System.Drawing.Point(707, 3);
            this.lblRecordCount.Margin = new System.Windows.Forms.Padding(3);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(0, 19);
            this.lblRecordCount.TabIndex = 0;
            this.lblRecordCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSelectAll
            // 
            this.pnlSelectAll.AutoSize = true;
            this.pnlSelectAll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSelectAll.Controls.Add(this.rbSelectAllGrid);
            this.pnlSelectAll.Controls.Add(this.rbSelectAllSalesforce);
            this.pnlSelectAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSelectAll.Location = new System.Drawing.Point(83, 3);
            this.pnlSelectAll.Name = "pnlSelectAll";
            this.pnlSelectAll.Size = new System.Drawing.Size(169, 19);
            this.pnlSelectAll.TabIndex = 16;
            this.pnlSelectAll.Visible = false;
            // 
            // rbSelectAllGrid
            // 
            this.rbSelectAllGrid.AutoSize = true;
            this.rbSelectAllGrid.Checked = true;
            this.rbSelectAllGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSelectAllGrid.Location = new System.Drawing.Point(5, 0);
            this.rbSelectAllGrid.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rbSelectAllGrid.Name = "rbSelectAllGrid";
            this.rbSelectAllGrid.Size = new System.Drawing.Size(59, 19);
            this.rbSelectAllGrid.TabIndex = 0;
            this.rbSelectAllGrid.TabStop = true;
            this.rbSelectAllGrid.Text = "in Grid";
            this.rbSelectAllGrid.UseVisualStyleBackColor = true;
            // 
            // rbSelectAllSalesforce
            // 
            this.rbSelectAllSalesforce.AutoSize = true;
            this.rbSelectAllSalesforce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSelectAllSalesforce.Location = new System.Drawing.Point(74, 0);
            this.rbSelectAllSalesforce.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rbSelectAllSalesforce.Name = "rbSelectAllSalesforce";
            this.rbSelectAllSalesforce.Size = new System.Drawing.Size(90, 19);
            this.rbSelectAllSalesforce.TabIndex = 0;
            this.rbSelectAllSalesforce.Text = "in Salesforce";
            this.rbSelectAllSalesforce.UseVisualStyleBackColor = true;
            this.rbSelectAllSalesforce.CheckedChanged += new System.EventHandler(this.rbSelectAllSalesforce_CheckedChanged);
            // 
            // pnlAction
            // 
            this.pnlAction.AutoSize = true;
            this.pnlAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlAction.Controls.Add(this.btnCancel);
            this.pnlAction.Controls.Add(this.btnNext);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAction.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlAction.Location = new System.Drawing.Point(3, 429);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.pnlAction.Size = new System.Drawing.Size(710, 44);
            this.pnlAction.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(620, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Location = new System.Drawing.Point(527, 6);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(87, 27);
            this.btnNext.TabIndex = 13;
            this.btnNext.Text = "&Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblTitle
            // 
			
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Location = new System.Drawing.Point(5, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(177, 30);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "Search and Select";
			//
			// userInputUserControl1
            // 
            this.userInputUserControl1.AutoSize = true;
            this.userInputUserControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.userInputUserControl1.BackColor = System.Drawing.SystemColors.Window;
            this.userInputUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userInputUserControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userInputUserControl1.Location = new System.Drawing.Point(3, 76);
            this.userInputUserControl1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.userInputUserControl1.Name = "userInputUserControl1";
            this.userInputUserControl1.Size = new System.Drawing.Size(710, 9);
            this.userInputUserControl1.TabIndex = 4;
            this.userInputUserControl1.Load += new System.EventHandler(this.userInputUserControl1_Load);
            // 
            // pnlResults
            // 
            this.pnlResults.AutoSize = true;
            this.pnlResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlResults.Controls.Add(this.dgvResults, 0, 0);
            this.pnlResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResults.Location = new System.Drawing.Point(3, 99);
            this.pnlResults.Name = "pnlResults";
            this.pnlResults.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.pnlResults.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlResults.Size = new System.Drawing.Size(710, 324);
            this.pnlResults.TabIndex = 2;
            // 
            // dgvResults
            // 
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvResults.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MultiSelect});
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvResults.Location = new System.Drawing.Point(3, 8);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(704, 308);
            this.dgvResults.TabIndex = 10;
            this.dgvResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResults_CellContentClick);
            this.dgvResults.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResults_CellDoubleClick);
            // 
            // MultiSelect
            // 
            this.MultiSelect.FalseValue = "false";
            this.MultiSelect.HeaderText = "";
            this.MultiSelect.Name = "MultiSelect";
            this.MultiSelect.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MultiSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.MultiSelect.TrueValue = "true";
            this.MultiSelect.Visible = false;
            // 
            // SearchAndSelectActionView
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(716, 476);
            this.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchAndSelectActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author For Excel";
            this.Load += new System.EventHandler(this.SearchAndSelectActionView_Load);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.tlpSearch.ResumeLayout(false);
            this.tlpSearch.PerformLayout();
            this.pnlResultHeader.ResumeLayout(false);
            this.pnlResultHeader.PerformLayout();
            this.pnlSelectAll.ResumeLayout(false);
            this.pnlSelectAll.PerformLayout();
            this.pnlAction.ResumeLayout(false);
            this.pnlResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TableLayoutPanel pnlResults;
        private System.Windows.Forms.FlowLayoutPanel pnlAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TableLayoutPanel pnlResultHeader;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MultiSelect;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private UserInputUserControl userInputUserControl1;
        private System.Windows.Forms.FlowLayoutPanel pnlSelectAll;
        private System.Windows.Forms.RadioButton rbSelectAllSalesforce;
        private System.Windows.Forms.RadioButton rbSelectAllGrid;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.LinkLabel llShowFilters;
    }
}