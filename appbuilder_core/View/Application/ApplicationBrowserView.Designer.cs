namespace Apttus.XAuthor.Core
{
    partial class ApplicationBrowserView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationBrowserView));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.chkMyApps = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.pnlGrid = new System.Windows.Forms.TableLayoutPanel();
            this.dgvAppList = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAction = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppList)).BeginInit();
            this.pnlAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.pnlSearch, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pnlGrid, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pnlAction, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(613, 446);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pnlSearch
            // 
            this.pnlSearch.AutoSize = true;
            this.pnlSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearch.ColumnCount = 3;
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.Controls.Add(this.lblSearch, 0, 0);
            this.pnlSearch.Controls.Add(this.txtSearch, 1, 0);
            this.pnlSearch.Controls.Add(this.chkMyApps, 0, 1);
            this.pnlSearch.Controls.Add(this.btnSearch, 2, 0);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(3, 43);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.RowCount = 2;
            this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSearch.Size = new System.Drawing.Size(607, 56);
            this.pnlSearch.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Location = new System.Drawing.Point(5, 5);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(5);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(124, 21);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search by App Name :";
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(137, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(322, 23);
            this.txtSearch.TabIndex = 2;
            // 
            // chkMyApps
            // 
            this.chkMyApps.AutoSize = true;
            this.chkMyApps.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMyApps.Location = new System.Drawing.Point(5, 34);
            this.chkMyApps.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.chkMyApps.Name = "chkMyApps";
            this.chkMyApps.Size = new System.Drawing.Size(70, 19);
            this.chkMyApps.TabIndex = 3;
            this.chkMyApps.Text = "My Apps";
            this.chkMyApps.UseVisualStyleBackColor = true;
            this.chkMyApps.CheckedChanged += new System.EventHandler(this.chkMyApps_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Location = new System.Drawing.Point(465, 3);
            this.btnSearch.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // pnlGrid
            // 
            this.pnlGrid.AutoSize = true;
            this.pnlGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlGrid.Controls.Add(this.dgvAppList);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(3, 105);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGrid.Size = new System.Drawing.Size(607, 299);
            this.pnlGrid.TabIndex = 2;
            // 
            // dgvAppList
            // 
            this.dgvAppList.AllowUserToAddRows = false;
            this.dgvAppList.AllowUserToDeleteRows = false;
            this.dgvAppList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAppList.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvAppList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAppList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAppList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAppList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.AppName,
            this.ResultHeader,
            this.ModifiedOn});
            this.dgvAppList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAppList.Location = new System.Drawing.Point(5, 5);
            this.dgvAppList.Margin = new System.Windows.Forms.Padding(5);
            this.dgvAppList.MultiSelect = false;
            this.dgvAppList.Name = "dgvAppList";
            this.dgvAppList.RowHeadersVisible = false;
            this.dgvAppList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppList.Size = new System.Drawing.Size(597, 289);
            this.dgvAppList.TabIndex = 6;
            this.dgvAppList.TabStop = false;
            this.dgvAppList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvAppList_CellFormatting);
            this.dgvAppList.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAppList_CellMouseDoubleClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Visible = false;
            // 
            // AppName
            // 
            this.AppName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AppName.DataPropertyName = "Name";
            this.AppName.FillWeight = 175.0503F;
            this.AppName.HeaderText = "App Name";
            this.AppName.Name = "AppName";
            this.AppName.ReadOnly = true;
            // 
            // ResultHeader
            // 
            this.ResultHeader.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResultHeader.DataPropertyName = "Owner.Name";
            this.ResultHeader.FillWeight = 68.66187F;
            this.ResultHeader.HeaderText = "Owner";
            this.ResultHeader.Name = "ResultHeader";
            // 
            // ModifiedOn
            // 
            this.ModifiedOn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ModifiedOn.DataPropertyName = "LastModifiedDate";
            dataGridViewCellStyle2.Format = "g";
            dataGridViewCellStyle2.NullValue = null;
            this.ModifiedOn.DefaultCellStyle = dataGridViewCellStyle2;
            this.ModifiedOn.FillWeight = 69.34622F;
            this.ModifiedOn.HeaderText = "Modified On";
            this.ModifiedOn.Name = "ModifiedOn";
            // 
            // pnlAction
            // 
            this.pnlAction.AutoSize = true;
            this.pnlAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlAction.Controls.Add(this.btnCancel);
            this.pnlAction.Controls.Add(this.btnSelect);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAction.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlAction.Location = new System.Drawing.Point(3, 410);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Size = new System.Drawing.Size(607, 33);
            this.pnlAction.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(514, 3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(90, 25);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 27);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(418, 3);
            this.btnSelect.MinimumSize = new System.Drawing.Size(90, 25);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(90, 27);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "&Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.Green;
            this.lblTitle.Location = new System.Drawing.Point(5, 5);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(111, 30);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Select App";
            // 
            // ApplicationBrowserView
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(613, 446);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationBrowserView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Browser";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppList)).EndInit();
            this.pnlAction.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel pnlSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TableLayoutPanel pnlGrid;
        private System.Windows.Forms.DataGridView dgvAppList;
        private System.Windows.Forms.CheckBox chkMyApps;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.FlowLayoutPanel pnlAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifiedOn;
    }
}