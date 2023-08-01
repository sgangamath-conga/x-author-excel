namespace Apttus.XAuthor.AppDesigner
{
    partial class AppSettingView
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
            this.tblAppSettings = new System.Windows.Forms.TableLayoutPanel();
            this.grpDataMigration = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.chkSuppressDependent = new System.Windows.Forms.CheckBox();
            this.chkSuppressSave = new System.Windows.Forms.CheckBox();
            this.chkSuppressNoOfRecords = new System.Windows.Forms.CheckBox();
            this.chkAllRecordsSaveSuccess = new System.Windows.Forms.CheckBox();
            this.grpProtectSheet = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvProtectSheet = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAppSettings = new System.Windows.Forms.Panel();
            this.lblAppSetting = new System.Windows.Forms.Label();
            this.grpLocalSettings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkIgnorePicklistValidation = new System.Windows.Forms.CheckBox();
            this.chkDisableRichtextEditing = new System.Windows.Forms.CheckBox();
            this.numMaxAttachmentSize = new System.Windows.Forms.NumericUpDown();
            this.lblMaxAttachmentSize = new System.Windows.Forms.Label();
            this.chkDisableSaveLocalFile = new System.Windows.Forms.CheckBox();
            this.chkDisablePrint = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAutoSizeColumn = new System.Windows.Forms.CheckBox();
            this.setRowColorlbl = new System.Windows.Forms.Label();
            this.chkShowFilters = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMaxColumnWidth = new System.Windows.Forms.TextBox();
            this.rowHighlightColor = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tblAppSettings.SuspendLayout();
            this.grpDataMigration.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.grpProtectSheet.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProtectSheet)).BeginInit();
            this.pnlAppSettings.SuspendLayout();
            this.grpLocalSettings.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAttachmentSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblAppSettings
            // 
            this.tblAppSettings.AutoSize = true;
            this.tblAppSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblAppSettings.ColumnCount = 1;
            this.tblAppSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblAppSettings.Controls.Add(this.grpDataMigration, 0, 3);
            this.tblAppSettings.Controls.Add(this.grpProtectSheet, 0, 4);
            this.tblAppSettings.Controls.Add(this.pnlAppSettings, 0, 0);
            this.tblAppSettings.Controls.Add(this.grpLocalSettings, 0, 1);
            this.tblAppSettings.Controls.Add(this.groupBox1, 0, 2);
            this.tblAppSettings.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tblAppSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAppSettings.Location = new System.Drawing.Point(0, 0);
            this.tblAppSettings.Name = "tblAppSettings";
            this.tblAppSettings.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.tblAppSettings.RowCount = 6;
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAppSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblAppSettings.Size = new System.Drawing.Size(495, 585);
            this.tblAppSettings.TabIndex = 12;
            // 
            // grpDataMigration
            // 
            this.grpDataMigration.AutoSize = true;
            this.grpDataMigration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpDataMigration.Controls.Add(this.tableLayoutPanel3);
            this.grpDataMigration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDataMigration.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.grpDataMigration.Location = new System.Drawing.Point(5, 232);
            this.grpDataMigration.Margin = new System.Windows.Forms.Padding(5);
            this.grpDataMigration.Name = "grpDataMigration";
            this.grpDataMigration.Padding = new System.Windows.Forms.Padding(5);
            this.grpDataMigration.Size = new System.Drawing.Size(487, 126);
            this.grpDataMigration.TabIndex = 14;
            this.grpDataMigration.TabStop = false;
            this.grpDataMigration.Text = "Suppress Messages";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.chkSuppressDependent, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkSuppressSave, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkSuppressNoOfRecords, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkAllRecordsSaveSuccess, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(5, 21);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(477, 100);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // chkSuppressDependent
            // 
            this.chkSuppressDependent.AutoSize = true;
            this.chkSuppressDependent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSuppressDependent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSuppressDependent.Location = new System.Drawing.Point(5, 28);
            this.chkSuppressDependent.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkSuppressDependent.Name = "chkSuppressDependent";
            this.chkSuppressDependent.Size = new System.Drawing.Size(392, 19);
            this.chkSuppressDependent.TabIndex = 7;
            this.chkSuppressDependent.Text = "When no records are retrieved for a parent object from a Query Action";
            this.chkSuppressDependent.UseVisualStyleBackColor = true;
            // 
            // chkSuppressSave
            // 
            this.chkSuppressSave.AutoSize = true;
            this.chkSuppressSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSuppressSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSuppressSave.Location = new System.Drawing.Point(5, 53);
            this.chkSuppressSave.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkSuppressSave.Name = "chkSuppressSave";
            this.chkSuppressSave.Size = new System.Drawing.Size(294, 19);
            this.chkSuppressSave.TabIndex = 6;
            this.chkSuppressSave.Text = "When no records are found to save in a Save Action";
            this.chkSuppressSave.UseVisualStyleBackColor = true;
            // 
            // chkSuppressNoOfRecords
            // 
            this.chkSuppressNoOfRecords.AutoSize = true;
            this.chkSuppressNoOfRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSuppressNoOfRecords.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSuppressNoOfRecords.Location = new System.Drawing.Point(5, 78);
            this.chkSuppressNoOfRecords.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkSuppressNoOfRecords.Name = "chkSuppressNoOfRecords";
            this.chkSuppressNoOfRecords.Size = new System.Drawing.Size(292, 19);
            this.chkSuppressNoOfRecords.TabIndex = 5;
            this.chkSuppressNoOfRecords.Text = "When no records are retrieved from a Query Action";
            this.chkSuppressNoOfRecords.UseVisualStyleBackColor = true;
            // 
            // chkAllRecordsSaveSuccess
            // 
            this.chkAllRecordsSaveSuccess.AutoSize = true;
            this.chkAllRecordsSaveSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAllRecordsSaveSuccess.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAllRecordsSaveSuccess.Location = new System.Drawing.Point(5, 3);
            this.chkAllRecordsSaveSuccess.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkAllRecordsSaveSuccess.Name = "chkAllRecordsSaveSuccess";
            this.chkAllRecordsSaveSuccess.Size = new System.Drawing.Size(316, 19);
            this.chkAllRecordsSaveSuccess.TabIndex = 8;
            this.chkAllRecordsSaveSuccess.Text = "When all records are saved successfully in a Save Action";
            this.chkAllRecordsSaveSuccess.UseVisualStyleBackColor = true;
            // 
            // grpProtectSheet
            // 
            this.grpProtectSheet.AutoSize = true;
            this.grpProtectSheet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpProtectSheet.Controls.Add(this.tableLayoutPanel5);
            this.grpProtectSheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProtectSheet.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.grpProtectSheet.Location = new System.Drawing.Point(5, 368);
            this.grpProtectSheet.Margin = new System.Windows.Forms.Padding(5);
            this.grpProtectSheet.Name = "grpProtectSheet";
            this.grpProtectSheet.Padding = new System.Windows.Forms.Padding(5);
            this.grpProtectSheet.Size = new System.Drawing.Size(487, 168);
            this.grpProtectSheet.TabIndex = 13;
            this.grpProtectSheet.TabStop = false;
            this.grpProtectSheet.Text = "Protect Sheet Setting ";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.dgvProtectSheet, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(5, 21);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(477, 142);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // dgvProtectSheet
            // 
            this.dgvProtectSheet.AllowUserToAddRows = false;
            this.dgvProtectSheet.AllowUserToDeleteRows = false;
            this.dgvProtectSheet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProtectSheet.BackgroundColor = System.Drawing.Color.White;
            this.dgvProtectSheet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvProtectSheet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProtectSheet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvProtectSheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProtectSheet.Location = new System.Drawing.Point(5, 5);
            this.dgvProtectSheet.Margin = new System.Windows.Forms.Padding(5);
            this.dgvProtectSheet.Name = "dgvProtectSheet";
            this.dgvProtectSheet.RowHeadersVisible = false;
            this.dgvProtectSheet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProtectSheet.Size = new System.Drawing.Size(467, 132);
            this.dgvProtectSheet.TabIndex = 1;
            this.dgvProtectSheet.Tag = "WorkbookProtection";
            this.dgvProtectSheet.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvProtectSheet_CellFormatting);
            this.dgvProtectSheet.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvProtectSheet_EditingControlShowing);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "SheetName";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn1.FillWeight = 210.6081F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Password";
            this.dataGridViewTextBoxColumn2.HeaderText = "Password";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // pnlAppSettings
            // 
            this.pnlAppSettings.AutoSize = true;
            this.pnlAppSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlAppSettings.Controls.Add(this.lblAppSetting);
            this.pnlAppSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAppSettings.Location = new System.Drawing.Point(5, 5);
            this.pnlAppSettings.Margin = new System.Windows.Forms.Padding(5);
            this.pnlAppSettings.Name = "pnlAppSettings";
            this.pnlAppSettings.Size = new System.Drawing.Size(487, 33);
            this.pnlAppSettings.TabIndex = 11;
            // 
            // lblAppSetting
            // 
            this.lblAppSetting.AutoSize = true;
            this.lblAppSetting.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblAppSetting.ForeColor = System.Drawing.Color.Green;
            this.lblAppSetting.Location = new System.Drawing.Point(3, 3);
            this.lblAppSetting.Name = "lblAppSetting";
            this.lblAppSetting.Size = new System.Drawing.Size(130, 30);
            this.lblAppSetting.TabIndex = 0;
            this.lblAppSetting.Text = "App Settings";
            // 
            // grpLocalSettings
            // 
            this.grpLocalSettings.AutoSize = true;
            this.grpLocalSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpLocalSettings.Controls.Add(this.tableLayoutPanel1);
            this.grpLocalSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLocalSettings.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.grpLocalSettings.Location = new System.Drawing.Point(5, 48);
            this.grpLocalSettings.Margin = new System.Windows.Forms.Padding(5);
            this.grpLocalSettings.Name = "grpLocalSettings";
            this.grpLocalSettings.Padding = new System.Windows.Forms.Padding(5);
            this.grpLocalSettings.Size = new System.Drawing.Size(487, 80);
            this.grpLocalSettings.TabIndex = 10;
            this.grpLocalSettings.TabStop = false;
            this.grpLocalSettings.Text = "`";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.chkIgnorePicklistValidation, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.chkDisableRichtextEditing, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numMaxAttachmentSize, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMaxAttachmentSize, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkDisableSaveLocalFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkDisablePrint, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 21);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(477, 54);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // chkIgnorePicklistValidation
            // 
            this.chkIgnorePicklistValidation.AutoSize = true;
            this.chkIgnorePicklistValidation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkIgnorePicklistValidation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkIgnorePicklistValidation.Location = new System.Drawing.Point(162, 31);
            this.chkIgnorePicklistValidation.Margin = new System.Windows.Forms.Padding(5, 2, 2, 2);
            this.chkIgnorePicklistValidation.Name = "chkIgnorePicklistValidation";
            this.chkIgnorePicklistValidation.Size = new System.Drawing.Size(219, 19);
            this.chkIgnorePicklistValidation.TabIndex = 4;
            this.chkIgnorePicklistValidation.Text = "Ignore Picklist Validation During Save";
            this.chkIgnorePicklistValidation.UseVisualStyleBackColor = true;
            // 
            // chkDisableRichtextEditing
            // 
            this.chkDisableRichtextEditing.AutoSize = true;
            this.chkDisableRichtextEditing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDisableRichtextEditing.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisableRichtextEditing.Location = new System.Drawing.Point(5, 32);
            this.chkDisableRichtextEditing.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkDisableRichtextEditing.Name = "chkDisableRichtextEditing";
            this.chkDisableRichtextEditing.Size = new System.Drawing.Size(152, 19);
            this.chkDisableRichtextEditing.TabIndex = 3;
            this.chkDisableRichtextEditing.Text = "Disable Rich Text editing";
            this.chkDisableRichtextEditing.UseVisualStyleBackColor = true;
            // 
            // numMaxAttachmentSize
            // 
            this.numMaxAttachmentSize.Location = new System.Drawing.Point(386, 3);
            this.numMaxAttachmentSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxAttachmentSize.Name = "numMaxAttachmentSize";
            this.numMaxAttachmentSize.Size = new System.Drawing.Size(56, 23);
            this.numMaxAttachmentSize.TabIndex = 5;
            this.numMaxAttachmentSize.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // lblMaxAttachmentSize
            // 
            this.lblMaxAttachmentSize.AutoSize = true;
            this.lblMaxAttachmentSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaxAttachmentSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMaxAttachmentSize.Location = new System.Drawing.Point(159, 7);
            this.lblMaxAttachmentSize.Margin = new System.Windows.Forms.Padding(2, 7, 2, 2);
            this.lblMaxAttachmentSize.Name = "lblMaxAttachmentSize";
            this.lblMaxAttachmentSize.Size = new System.Drawing.Size(169, 15);
            this.lblMaxAttachmentSize.TabIndex = 7;
            this.lblMaxAttachmentSize.Text = "Max. Attachment Size (in MB) :";
            // 
            // chkDisableSaveLocalFile
            // 
            this.chkDisableSaveLocalFile.AutoSize = true;
            this.chkDisableSaveLocalFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDisableSaveLocalFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisableSaveLocalFile.Location = new System.Drawing.Point(5, 3);
            this.chkDisableSaveLocalFile.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkDisableSaveLocalFile.Name = "chkDisableSaveLocalFile";
            this.chkDisableSaveLocalFile.Size = new System.Drawing.Size(137, 19);
            this.chkDisableSaveLocalFile.TabIndex = 1;
            this.chkDisableSaveLocalFile.Text = "Disable local Save File";
            this.chkDisableSaveLocalFile.UseVisualStyleBackColor = true;
            // 
            // chkDisablePrint
            // 
            this.chkDisablePrint.AutoSize = true;
            this.chkDisablePrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDisablePrint.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisablePrint.Location = new System.Drawing.Point(386, 31);
            this.chkDisablePrint.Margin = new System.Windows.Forms.Padding(3, 2, 2, 2);
            this.chkDisablePrint.Name = "chkDisablePrint";
            this.chkDisablePrint.Size = new System.Drawing.Size(89, 19);
            this.chkDisablePrint.TabIndex = 2;
            this.chkDisablePrint.Text = "Disable Print";
            this.chkDisablePrint.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupBox1.Location = new System.Drawing.Point(5, 138);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(487, 84);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Format";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.chkAutoSizeColumn, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.setRowColorlbl, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkShowFilters, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtMaxColumnWidth, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.rowHighlightColor, 2, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 21);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(477, 58);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // chkAutoSizeColumn
            // 
            this.chkAutoSizeColumn.AutoSize = true;
            this.chkAutoSizeColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAutoSizeColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoSizeColumn.Location = new System.Drawing.Point(5, 3);
            this.chkAutoSizeColumn.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkAutoSizeColumn.Name = "chkAutoSizeColumn";
            this.chkAutoSizeColumn.Size = new System.Drawing.Size(153, 19);
            this.chkAutoSizeColumn.TabIndex = 17;
            this.chkAutoSizeColumn.Text = "Auto Size Column Width";
            this.chkAutoSizeColumn.UseVisualStyleBackColor = true;
            this.chkAutoSizeColumn.CheckedChanged += new System.EventHandler(this.chkAutoWidth_CheckedChanged);
            // 
            // setRowColorlbl
            // 
            this.setRowColorlbl.AutoSize = true;
            this.setRowColorlbl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.setRowColorlbl.Location = new System.Drawing.Point(163, 34);
            this.setRowColorlbl.Margin = new System.Windows.Forms.Padding(5);
            this.setRowColorlbl.Name = "setRowColorlbl";
            this.setRowColorlbl.Size = new System.Drawing.Size(150, 15);
            this.setRowColorlbl.TabIndex = 20;
            this.setRowColorlbl.Text = "Color for Unsaved Records:";
            // 
            // chkShowFilters
            // 
            this.chkShowFilters.AutoSize = true;
            this.chkShowFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowFilters.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkShowFilters.Location = new System.Drawing.Point(5, 32);
            this.chkShowFilters.Margin = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.chkShowFilters.Name = "chkShowFilters";
            this.chkShowFilters.Size = new System.Drawing.Size(95, 19);
            this.chkShowFilters.TabIndex = 16;
            this.chkShowFilters.Text = "Display Filters";
            this.chkShowFilters.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(163, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Max Column Width :";
            // 
            // txtMaxColumnWidth
            // 
            this.txtMaxColumnWidth.Enabled = false;
            this.txtMaxColumnWidth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaxColumnWidth.Location = new System.Drawing.Point(321, 3);
            this.txtMaxColumnWidth.MaxLength = 3;
            this.txtMaxColumnWidth.Name = "txtMaxColumnWidth";
            this.txtMaxColumnWidth.Size = new System.Drawing.Size(36, 23);
            this.txtMaxColumnWidth.TabIndex = 19;
            // 
            // rowHighlightColor
            // 
            this.rowHighlightColor.BackColor = System.Drawing.Color.Crimson;
            this.rowHighlightColor.Location = new System.Drawing.Point(321, 32);
            this.rowHighlightColor.Name = "rowHighlightColor";
            this.rowHighlightColor.ReadOnly = true;
            this.rowHighlightColor.Size = new System.Drawing.Size(37, 23);
            this.rowHighlightColor.TabIndex = 21;
            this.rowHighlightColor.Click += new System.EventHandler(this.rowHighlightColor_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 544);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(491, 37);
            this.flowLayoutPanel1.TabIndex = 16;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(376, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 27);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(256, 5);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 27);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // AppSettingView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(495, 585);
            this.Controls.Add(this.tblAppSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSettingView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.AppSettingView_Load);
            this.tblAppSettings.ResumeLayout(false);
            this.tblAppSettings.PerformLayout();
            this.grpDataMigration.ResumeLayout(false);
            this.grpDataMigration.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.grpProtectSheet.ResumeLayout(false);
            this.grpProtectSheet.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProtectSheet)).EndInit();
            this.pnlAppSettings.ResumeLayout(false);
            this.pnlAppSettings.PerformLayout();
            this.grpLocalSettings.ResumeLayout(false);
            this.grpLocalSettings.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAttachmentSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpLocalSettings;
        private System.Windows.Forms.TableLayoutPanel tblAppSettings;
        private System.Windows.Forms.Panel pnlAppSettings;
        private System.Windows.Forms.Label lblAppSetting;
        private System.Windows.Forms.CheckBox chkDisableSaveLocalFile;
        private System.Windows.Forms.CheckBox chkDisablePrint;
        private System.Windows.Forms.CheckBox chkDisableRichtextEditing;
        private System.Windows.Forms.GroupBox grpDataMigration;
        private System.Windows.Forms.GroupBox grpProtectSheet;
        private System.Windows.Forms.DataGridView dgvProtectSheet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkIgnorePicklistValidation;
        private System.Windows.Forms.Label lblMaxAttachmentSize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown numMaxAttachmentSize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkAutoSizeColumn;
        private System.Windows.Forms.Label setRowColorlbl;
        private System.Windows.Forms.CheckBox chkShowFilters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMaxColumnWidth;
        private System.Windows.Forms.TextBox rowHighlightColor;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox chkSuppressDependent;
        private System.Windows.Forms.CheckBox chkSuppressSave;
        private System.Windows.Forms.CheckBox chkSuppressNoOfRecords;
        private System.Windows.Forms.CheckBox chkAllRecordsSaveSuccess;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}