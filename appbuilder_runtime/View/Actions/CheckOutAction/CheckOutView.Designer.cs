namespace Apttus.XAuthor.AppRuntime
{
    partial class CheckOutView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlGrid = new System.Windows.Forms.TableLayoutPanel();
            this.dgvAppFiles = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DocId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Locked = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedById = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAction = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppFiles)).BeginInit();
            this.pnlAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(403, 3);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(101, 31);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "&Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(510, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 31);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.pnlSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlGrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pnlAction, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(620, 437);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pnlSearch
            // 
            this.pnlSearch.AutoSize = true;
            this.pnlSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(3, 3);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(614, 33);
            this.pnlSearch.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI Semilight", 14.75F);
            this.lblSearch.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSearch.Location = new System.Drawing.Point(5, 0);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(5);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(202, 28);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Select Application File :";
            // 
            // pnlGrid
            // 
            this.pnlGrid.AutoSize = true;
            this.pnlGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlGrid.Controls.Add(this.dgvAppFiles);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(3, 42);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlGrid.Size = new System.Drawing.Size(614, 347);
            this.pnlGrid.TabIndex = 2;
            // 
            // dgvAppFiles
            // 
            this.dgvAppFiles.AllowUserToAddRows = false;
            this.dgvAppFiles.AllowUserToDeleteRows = false;
            this.dgvAppFiles.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAppFiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAppFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAppFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.DocId,
            this.AppName,
            this.Locked,
            this.LastModifiedBy,
            this.ModifiedOn,
            this.LastModifiedById});
            this.dgvAppFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAppFiles.Location = new System.Drawing.Point(3, 3);
            this.dgvAppFiles.MultiSelect = false;
            this.dgvAppFiles.Name = "dgvAppFiles";
            this.dgvAppFiles.RowHeadersVisible = false;
            this.dgvAppFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppFiles.Size = new System.Drawing.Size(608, 341);
            this.dgvAppFiles.TabIndex = 6;
            this.dgvAppFiles.TabStop = false;
            this.dgvAppFiles.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvAppFiles_CellFormatting);
            this.dgvAppFiles.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAppFiles_CellMouseDoubleClick);
            // 
            // Id
            // 
            this.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Visible = false;
            // 
            // DocId
            // 
            this.DocId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DocId.DataPropertyName = "Apttus_XApps__FileId__c";
            this.DocId.HeaderText = "DocId";
            this.DocId.Name = "DocId";
            this.DocId.Visible = false;
            // 
            // AppName
            // 
            this.AppName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AppName.DataPropertyName = "Name";
            this.AppName.FillWeight = 140.4915F;
            this.AppName.HeaderText = "File Name";
            this.AppName.Name = "AppName";
            this.AppName.ReadOnly = true;
            // 
            // Locked
            // 
            this.Locked.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Locked.DataPropertyName = "Apttus_XApps__IsLocked__c";
            this.Locked.FillWeight = 70.53762F;
            this.Locked.HeaderText = "Locked";
            this.Locked.Name = "Locked";
            // 
            // LastModifiedBy
            // 
            this.LastModifiedBy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LastModifiedBy.DataPropertyName = "LastModifiedBy.Name";
            this.LastModifiedBy.FillWeight = 95.8226F;
            this.LastModifiedBy.HeaderText = "Last Modified By";
            this.LastModifiedBy.Name = "LastModifiedBy";
            // 
            // ModifiedOn
            // 
            this.ModifiedOn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ModifiedOn.DataPropertyName = "LastModifiedDate";
            dataGridViewCellStyle4.Format = "g";
            dataGridViewCellStyle4.NullValue = null;
            this.ModifiedOn.DefaultCellStyle = dataGridViewCellStyle4;
            this.ModifiedOn.FillWeight = 85.14823F;
            this.ModifiedOn.HeaderText = "Modified On";
            this.ModifiedOn.Name = "ModifiedOn";
            // 
            // LastModifiedById
            // 
            this.LastModifiedById.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LastModifiedById.DataPropertyName = "LastModifiedBy.Id";
            this.LastModifiedById.HeaderText = "LastModifiedById";
            this.LastModifiedById.Name = "LastModifiedById";
            this.LastModifiedById.Visible = false;
            // 
            // pnlAction
            // 
            this.pnlAction.AutoSize = true;
            this.pnlAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlAction.Controls.Add(this.btnCancel);
            this.pnlAction.Controls.Add(this.btnSelect);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAction.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlAction.Location = new System.Drawing.Point(3, 395);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Size = new System.Drawing.Size(614, 39);
            this.pnlAction.TabIndex = 3;
            // 
            // CheckOutView
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(620, 437);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckOutView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CheckOutView";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppFiles)).EndInit();
            this.pnlAction.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TableLayoutPanel pnlGrid;
        private System.Windows.Forms.DataGridView dgvAppFiles;
        private System.Windows.Forms.FlowLayoutPanel pnlAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn DocId;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Locked;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifiedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedById;
    }
}