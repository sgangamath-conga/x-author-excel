namespace Apttus.XAuthor.AppDesigner
{
    partial class SaveMapView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.overallTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSaveFields = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddRetrieveFields = new System.Windows.Forms.Button();
            this.btnAddMatrixFields = new System.Windows.Forms.Button();
            this.btnAddOtherFields = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.llPreloadedRows = new System.Windows.Forms.LinkLabel();
            this.gbPreloadOptions = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddPreloadOptions = new System.Windows.Forms.Button();
            this.lblRepeatingObjects = new System.Windows.Forms.Label();
            this.txtPreloadRows = new System.Windows.Forms.TextBox();
            this.lblPreloadRows = new System.Windows.Forms.Label();
            this.cmbIndependentObjects = new System.Windows.Forms.ComboBox();
            this.lblMappedFields = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkMapped = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DesignerLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveConditionText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LookAhead = new System.Windows.Forms.DataGridViewImageColumn();
            this.Condition = new System.Windows.Forms.DataGridViewButtonColumn();
            this.overallTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveFields)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.gbPreloadOptions.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // overallTableLayout
            // 
            this.overallTableLayout.AutoSize = true;
            this.overallTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.overallTableLayout.ColumnCount = 1;
            this.overallTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.overallTableLayout.Controls.Add(this.dgvSaveFields, 0, 3);
            this.overallTableLayout.Controls.Add(this.tableLayoutPanel3, 0, 4);
            this.overallTableLayout.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.overallTableLayout.Controls.Add(this.gbPreloadOptions, 0, 1);
            this.overallTableLayout.Controls.Add(this.lblMappedFields, 0, 2);
            this.overallTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overallTableLayout.Location = new System.Drawing.Point(0, 0);
            this.overallTableLayout.Name = "overallTableLayout";
            this.overallTableLayout.RowCount = 5;
            this.overallTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overallTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overallTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overallTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overallTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overallTableLayout.Size = new System.Drawing.Size(624, 475);
            this.overallTableLayout.TabIndex = 0;
            // 
            // dgvSaveFields
            // 
            this.dgvSaveFields.AllowUserToAddRows = false;
            this.dgvSaveFields.AllowUserToDeleteRows = false;
            this.dgvSaveFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSaveFields.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSaveFields.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSaveFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSaveFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaveFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkMapped,
            this.FieldName,
            this.DesignerLocation,
            this.Type,
            this.FieldType,
            this.SaveConditionText,
            this.LookAhead,
            this.Condition});
            this.dgvSaveFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSaveFields.EnableHeadersVisualStyles = false;
            this.dgvSaveFields.GridColor = System.Drawing.Color.Silver;
            this.dgvSaveFields.Location = new System.Drawing.Point(3, 203);
            this.dgvSaveFields.Name = "dgvSaveFields";
            this.dgvSaveFields.RowHeadersVisible = false;
            this.dgvSaveFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaveFields.Size = new System.Drawing.Size(618, 230);
            this.dgvSaveFields.TabIndex = 8;
            this.dgvSaveFields.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSaveFields_CellContentClick);
            this.dgvSaveFields.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSaveFields_CellFormatting);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.btnRemove, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnClose, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSave, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 439);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(618, 33);
            this.tableLayoutPanel3.TabIndex = 19;
            // 
            // btnRemove
            // 
            this.btnRemove.AutoSize = true;
            this.btnRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(273, 3);
            this.btnRemove.MinimumSize = new System.Drawing.Size(110, 25);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(110, 27);
            this.btnRemove.TabIndex = 9;
            this.btnRemove.Text = "Remove Fields";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(505, 3);
            this.btnClose.MinimumSize = new System.Drawing.Size(110, 25);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 27);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(389, 3);
            this.btnSave.MinimumSize = new System.Drawing.Size(110, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 27);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.llPreloadedRows, 0, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(618, 95);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnAddRetrieveFields, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddMatrixFields, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddOtherFields, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 38);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(612, 33);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // btnAddRetrieveFields
            // 
            this.btnAddRetrieveFields.AutoSize = true;
            this.btnAddRetrieveFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddRetrieveFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddRetrieveFields.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddRetrieveFields.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddRetrieveFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddRetrieveFields.Location = new System.Drawing.Point(3, 3);
            this.btnAddRetrieveFields.MinimumSize = new System.Drawing.Size(180, 27);
            this.btnAddRetrieveFields.Name = "btnAddRetrieveFields";
            this.btnAddRetrieveFields.Size = new System.Drawing.Size(180, 27);
            this.btnAddRetrieveFields.TabIndex = 2;
            this.btnAddRetrieveFields.Text = "Add Display Map Fields";
            this.btnAddRetrieveFields.UseVisualStyleBackColor = true;
            this.btnAddRetrieveFields.Click += new System.EventHandler(this.btnAddRetrieveFields_Click);
            // 
            // btnAddMatrixFields
            // 
            this.btnAddMatrixFields.AutoSize = true;
            this.btnAddMatrixFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddMatrixFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddMatrixFields.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddMatrixFields.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddMatrixFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddMatrixFields.Location = new System.Drawing.Point(189, 3);
            this.btnAddMatrixFields.MinimumSize = new System.Drawing.Size(180, 27);
            this.btnAddMatrixFields.Name = "btnAddMatrixFields";
            this.btnAddMatrixFields.Size = new System.Drawing.Size(180, 27);
            this.btnAddMatrixFields.TabIndex = 17;
            this.btnAddMatrixFields.Text = "Add Matrix Map Sections";
            this.btnAddMatrixFields.UseVisualStyleBackColor = true;
            this.btnAddMatrixFields.Click += new System.EventHandler(this.btnAddMatrixFields_Click);
            // 
            // btnAddOtherFields
            // 
            this.btnAddOtherFields.AutoSize = true;
            this.btnAddOtherFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddOtherFields.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAddOtherFields.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddOtherFields.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddOtherFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddOtherFields.Location = new System.Drawing.Point(375, 3);
            this.btnAddOtherFields.MinimumSize = new System.Drawing.Size(180, 27);
            this.btnAddOtherFields.Name = "btnAddOtherFields";
            this.btnAddOtherFields.Size = new System.Drawing.Size(180, 27);
            this.btnAddOtherFields.TabIndex = 3;
            this.btnAddOtherFields.Text = "Add Other Fields";
            this.btnAddOtherFields.UseVisualStyleBackColor = true;
            this.btnAddOtherFields.Click += new System.EventHandler(this.btnAddOtherFields_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblName);
            this.flowLayoutPanel1.Controls.Add(this.txtName);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(612, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 15);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name :";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(54, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(501, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // llPreloadedRows
            // 
            this.llPreloadedRows.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.llPreloadedRows.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.llPreloadedRows, 2);
            this.llPreloadedRows.Location = new System.Drawing.Point(250, 77);
            this.llPreloadedRows.Margin = new System.Windows.Forms.Padding(3);
            this.llPreloadedRows.Name = "llPreloadedRows";
            this.llPreloadedRows.Size = new System.Drawing.Size(117, 15);
            this.llPreloadedRows.TabIndex = 20;
            this.llPreloadedRows.TabStop = true;
            this.llPreloadedRows.Text = "Show Preloaded Grid";
            this.llPreloadedRows.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.llPreloadedRows.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llPreloadedRows_LinkClicked);
            // 
            // gbPreloadOptions
            // 
            this.gbPreloadOptions.AutoSize = true;
            this.gbPreloadOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbPreloadOptions.Controls.Add(this.tableLayoutPanel5);
            this.gbPreloadOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPreloadOptions.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.gbPreloadOptions.Location = new System.Drawing.Point(3, 104);
            this.gbPreloadOptions.Name = "gbPreloadOptions";
            this.gbPreloadOptions.Size = new System.Drawing.Size(618, 72);
            this.gbPreloadOptions.TabIndex = 3;
            this.gbPreloadOptions.TabStop = false;
            this.gbPreloadOptions.Text = "Preloaded Grid";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.btnAddPreloadOptions, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.lblRepeatingObjects, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.txtPreloadRows, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.lblPreloadRows, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.cmbIndependentObjects, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(612, 50);
            this.tableLayoutPanel5.TabIndex = 20;
            // 
            // btnAddPreloadOptions
            // 
            this.btnAddPreloadOptions.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddPreloadOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddPreloadOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPreloadOptions.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.ok_icon;
            this.btnAddPreloadOptions.Location = new System.Drawing.Point(382, 24);
            this.btnAddPreloadOptions.Name = "btnAddPreloadOptions";
            this.btnAddPreloadOptions.Size = new System.Drawing.Size(26, 23);
            this.btnAddPreloadOptions.TabIndex = 17;
            this.btnAddPreloadOptions.UseVisualStyleBackColor = true;
            this.btnAddPreloadOptions.Click += new System.EventHandler(this.btnAddPreloadOptions_Click);
            // 
            // lblRepeatingObjects
            // 
            this.lblRepeatingObjects.AutoSize = true;
            this.lblRepeatingObjects.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRepeatingObjects.Location = new System.Drawing.Point(3, 3);
            this.lblRepeatingObjects.Margin = new System.Windows.Forms.Padding(3);
            this.lblRepeatingObjects.Name = "lblRepeatingObjects";
            this.lblRepeatingObjects.Size = new System.Drawing.Size(158, 15);
            this.lblRepeatingObjects.TabIndex = 16;
            this.lblRepeatingObjects.Text = "List Object (Preloaded Rows)";
            // 
            // txtPreloadRows
            // 
            this.txtPreloadRows.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPreloadRows.Location = new System.Drawing.Point(285, 24);
            this.txtPreloadRows.Name = "txtPreloadRows";
            this.txtPreloadRows.Size = new System.Drawing.Size(91, 23);
            this.txtPreloadRows.TabIndex = 12;
            // 
            // lblPreloadRows
            // 
            this.lblPreloadRows.AutoSize = true;
            this.lblPreloadRows.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreloadRows.Location = new System.Drawing.Point(285, 3);
            this.lblPreloadRows.Margin = new System.Windows.Forms.Padding(3);
            this.lblPreloadRows.Name = "lblPreloadRows";
            this.lblPreloadRows.Size = new System.Drawing.Size(91, 15);
            this.lblPreloadRows.TabIndex = 15;
            this.lblPreloadRows.Text = "Preloaded Rows";
            // 
            // cmbIndependentObjects
            // 
            this.cmbIndependentObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIndependentObjects.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbIndependentObjects.FormattingEnabled = true;
            this.cmbIndependentObjects.Location = new System.Drawing.Point(3, 24);
            this.cmbIndependentObjects.Name = "cmbIndependentObjects";
            this.cmbIndependentObjects.Size = new System.Drawing.Size(276, 23);
            this.cmbIndependentObjects.TabIndex = 11;
            this.cmbIndependentObjects.SelectedIndexChanged += new System.EventHandler(this.cmbIndependentObjects_SelectedIndexChanged);
            // 
            // lblMappedFields
            // 
            this.lblMappedFields.AutoSize = true;
            this.lblMappedFields.Location = new System.Drawing.Point(3, 182);
            this.lblMappedFields.Margin = new System.Windows.Forms.Padding(3);
            this.lblMappedFields.Name = "lblMappedFields";
            this.lblMappedFields.Size = new System.Drawing.Size(84, 15);
            this.lblMappedFields.TabIndex = 20;
            this.lblMappedFields.Text = "Mapped Fields";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // chkMapped
            // 
            this.chkMapped.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.chkMapped.DataPropertyName = "Included";
            this.chkMapped.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMapped.HeaderText = "";
            this.chkMapped.MinimumWidth = 20;
            this.chkMapped.Name = "chkMapped";
            this.chkMapped.Width = 20;
            // 
            // FieldName
            // 
            this.FieldName.DataPropertyName = "FieldName";
            this.FieldName.FillWeight = 111.1111F;
            this.FieldName.HeaderText = "Name";
            this.FieldName.Name = "FieldName";
            this.FieldName.ReadOnly = true;
            // 
            // DesignerLocation
            // 
            this.DesignerLocation.DataPropertyName = "DesignerLocation";
            this.DesignerLocation.FillWeight = 88.88889F;
            this.DesignerLocation.HeaderText = "Location";
            this.DesignerLocation.Name = "DesignerLocation";
            this.DesignerLocation.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            // 
            // FieldType
            // 
            this.FieldType.DataPropertyName = "SaveFieldTypeDesc";
            this.FieldType.HeaderText = "Field Type";
            this.FieldType.Name = "FieldType";
            this.FieldType.ReadOnly = true;
            this.FieldType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FieldType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SaveConditionText
            // 
            this.SaveConditionText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SaveConditionText.DataPropertyName = "SaveConditionText";
            this.SaveConditionText.HeaderText = "Condition";
            this.SaveConditionText.Name = "SaveConditionText";
            this.SaveConditionText.Visible = false;
            this.SaveConditionText.Width = 85;
            // 
            // LookAhead
            // 
            this.LookAhead.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.LookAhead.HeaderText = "";
            this.LookAhead.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.LA_Blank;
            this.LookAhead.MinimumWidth = 20;
            this.LookAhead.Name = "LookAhead";
            this.LookAhead.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.LookAhead.Width = 20;
            // 
            // Condition
            // 
            this.Condition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Condition.HeaderText = "";
            this.Condition.Name = "Condition";
            this.Condition.Visible = false;
            this.Condition.Width = 5;
            // 
            // SaveMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.overallTableLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SaveMapView";
            this.Size = new System.Drawing.Size(624, 475);
            this.overallTableLayout.ResumeLayout(false);
            this.overallTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaveFields)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.gbPreloadOptions.ResumeLayout(false);
            this.gbPreloadOptions.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel overallTableLayout;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnAddRetrieveFields;
        private System.Windows.Forms.Button btnAddOtherFields;
        private System.Windows.Forms.GroupBox gbPreloadOptions;
        private System.Windows.Forms.Label lblRepeatingObjects;
        private System.Windows.Forms.ComboBox cmbIndependentObjects;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox txtPreloadRows;
        private System.Windows.Forms.Label lblPreloadRows;
        private System.Windows.Forms.Button btnAddPreloadOptions;
        private System.Windows.Forms.Button btnAddMatrixFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvSaveFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lblMappedFields;
        private System.Windows.Forms.LinkLabel llPreloadedRows;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkMapped;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DesignerLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldType;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveConditionText;
        private System.Windows.Forms.DataGridViewImageColumn LookAhead;
        private System.Windows.Forms.DataGridViewButtonColumn Condition;
    }
}
