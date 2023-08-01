namespace Apttus.XAuthor.AppDesigner
{
    partial class SaveMapRetrieveFieldView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cmbFilterBy = new System.Windows.Forms.ComboBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.actionPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.btnInclude = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.retrieveFieldsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSaveMapRetrieveField = new System.Windows.Forms.DataGridView();
            this.Included = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.RetrieveFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DesignerLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RetrieveMapName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.actionPanel.SuspendLayout();
            this.retrieveFieldsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveMapRetrieveField)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.topPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.actionPanel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.retrieveFieldsPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 518);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.AutoSize = true;
            this.topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.topPanel.ColumnCount = 2;
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topPanel.Controls.Add(this.cmbFilterBy, 1, 0);
            this.topPanel.Controls.Add(this.lblFilter, 0, 0);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(3, 49);
            this.topPanel.Name = "topPanel";
            this.topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.topPanel.Size = new System.Drawing.Size(603, 29);
            this.topPanel.TabIndex = 0;
            // 
            // cmbFilterBy
            // 
            this.cmbFilterBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterBy.FormattingEnabled = true;
            this.cmbFilterBy.Location = new System.Drawing.Point(65, 3);
            this.cmbFilterBy.Name = "cmbFilterBy";
            this.cmbFilterBy.Size = new System.Drawing.Size(261, 23);
            this.cmbFilterBy.TabIndex = 1;
            this.cmbFilterBy.SelectedIndexChanged += new System.EventHandler(this.cmbFilterBy_SelectedIndexChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(5, 5);
            this.lblFilter.Margin = new System.Windows.Forms.Padding(5);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(52, 15);
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "Filter by:";
            // 
            // actionPanel
            // 
            this.actionPanel.AutoSize = true;
            this.actionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.actionPanel.Controls.Add(this.chkSelectAll);
            this.actionPanel.Controls.Add(this.btnInclude);
            this.actionPanel.Controls.Add(this.btnClose);
            this.actionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionPanel.Location = new System.Drawing.Point(3, 467);
            this.actionPanel.Name = "actionPanel";
            this.actionPanel.Padding = new System.Windows.Forms.Padding(5);
            this.actionPanel.Size = new System.Drawing.Size(603, 48);
            this.actionPanel.TabIndex = 1;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSelectAll.Location = new System.Drawing.Point(8, 8);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(71, 19);
            this.chkSelectAll.TabIndex = 0;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // btnInclude
            // 
            this.btnInclude.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnInclude.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnInclude.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnInclude.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInclude.Location = new System.Drawing.Point(372, 8);
            this.btnInclude.Margin = new System.Windows.Forms.Padding(290, 3, 3, 3);
            this.btnInclude.Name = "btnInclude";
            this.btnInclude.Size = new System.Drawing.Size(99, 27);
            this.btnInclude.TabIndex = 1;
            this.btnInclude.Text = "Apply";
            this.btnInclude.UseVisualStyleBackColor = true;
            this.btnInclude.Click += new System.EventHandler(this.btnInclude_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(477, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(99, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // retrieveFieldsPanel
            // 
            this.retrieveFieldsPanel.AutoScroll = true;
            this.retrieveFieldsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.retrieveFieldsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.retrieveFieldsPanel.Controls.Add(this.dgvSaveMapRetrieveField);
            this.retrieveFieldsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.retrieveFieldsPanel.Location = new System.Drawing.Point(3, 84);
            this.retrieveFieldsPanel.Name = "retrieveFieldsPanel";
            this.retrieveFieldsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.retrieveFieldsPanel.Size = new System.Drawing.Size(603, 377);
            this.retrieveFieldsPanel.TabIndex = 2;
            // 
            // dgvSaveMapRetrieveField
            // 
            this.dgvSaveMapRetrieveField.AllowUserToAddRows = false;
            this.dgvSaveMapRetrieveField.AllowUserToDeleteRows = false;
            this.dgvSaveMapRetrieveField.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSaveMapRetrieveField.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSaveMapRetrieveField.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaveMapRetrieveField.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Included,
            this.RetrieveFieldName,
            this.DesignerLocation,
            this.Type,
            this.RetrieveMapName});
            this.dgvSaveMapRetrieveField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSaveMapRetrieveField.GridColor = System.Drawing.Color.Silver;
            this.dgvSaveMapRetrieveField.Location = new System.Drawing.Point(3, 3);
            this.dgvSaveMapRetrieveField.Name = "dgvSaveMapRetrieveField";
            this.dgvSaveMapRetrieveField.RowHeadersVisible = false;
            this.dgvSaveMapRetrieveField.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaveMapRetrieveField.Size = new System.Drawing.Size(597, 371);
            this.dgvSaveMapRetrieveField.TabIndex = 2;
            this.dgvSaveMapRetrieveField.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSaveMapRetrieveField_CellFormatting);
            // 
            // Included
            // 
            this.Included.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Included.DataPropertyName = "Included";
            this.Included.FalseValue = "false";
            this.Included.HeaderText = "";
            this.Included.Name = "Included";
            this.Included.TrueValue = "true";
            this.Included.Width = 5;
            // 
            // RetrieveFieldName
            // 
            this.RetrieveFieldName.DataPropertyName = "RetrieveFieldName";
            this.RetrieveFieldName.HeaderText = "Field";
            this.RetrieveFieldName.Name = "RetrieveFieldName";
            this.RetrieveFieldName.Width = 325;
            // 
            // DesignerLocation
            // 
            this.DesignerLocation.DataPropertyName = "DesignerLocation";
            this.DesignerLocation.HeaderText = "Location";
            this.DesignerLocation.Name = "DesignerLocation";
            this.DesignerLocation.Width = 80;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.Width = 80;
            // 
            // RetrieveMapName
            // 
            this.RetrieveMapName.DataPropertyName = "RetrieveMapName";
            this.RetrieveMapName.HeaderText = "Display Map";
            this.RetrieveMapName.Name = "RetrieveMapName";
            this.RetrieveMapName.Width = 102;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(603, 40);
            this.panel1.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.Green;
            this.lblTitle.Location = new System.Drawing.Point(4, 5);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(180, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Add Display Fields";
            // 
            // SaveMapRetrieveFieldView
            // 
            this.AcceptButton = this.btnInclude;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(609, 518);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveMapRetrieveFieldView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Display Fields";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.actionPanel.ResumeLayout(false);
            this.actionPanel.PerformLayout();
            this.retrieveFieldsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveMapRetrieveField)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel topPanel;
        private System.Windows.Forms.TableLayoutPanel retrieveFieldsPanel;
        private System.Windows.Forms.DataGridView dgvSaveMapRetrieveField;
        private System.Windows.Forms.ComboBox cmbFilterBy;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.FlowLayoutPanel actionPanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnInclude;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Included;
        private System.Windows.Forms.DataGridViewTextBoxColumn RetrieveFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DesignerLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn RetrieveMapName;
    }
}