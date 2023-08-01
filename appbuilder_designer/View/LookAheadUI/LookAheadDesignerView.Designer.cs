namespace Apttus.XAuthor.AppDesigner
{
    partial class LookAheadDesignerView
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
            this.components = new System.ComponentModel.Container();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.flpBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tblMappedFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblMappedFieldName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tblDataSourceSelection = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdbSearchAndSelect = new System.Windows.Forms.RadioButton();
            this.rdbExcel = new System.Windows.Forms.RadioButton();
            this.tblLookAheadSource = new System.Windows.Forms.TableLayoutPanel();
            this.tblExcel = new System.Windows.Forms.TableLayoutPanel();
            this.lblConfigureExcel = new System.Windows.Forms.Label();
            this.gbColumnMapping = new System.Windows.Forms.GroupBox();
            this.tblColumnMapping = new System.Windows.Forms.TableLayoutPanel();
            this.tblSingleMultiCol = new System.Windows.Forms.TableLayoutPanel();
            this.rdbMultiple = new System.Windows.Forms.RadioButton();
            this.rdbSingle = new System.Windows.Forms.RadioButton();
            this.tblMappingContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tblSingleColumn = new System.Windows.Forms.TableLayoutPanel();
            this.lblPrimaryColumn = new System.Windows.Forms.Label();
            this.flpPrimaryColumn = new System.Windows.Forms.FlowLayoutPanel();
            this.cmbPrimary = new System.Windows.Forms.ComboBox();
            this.lblPrimaryColumnText = new System.Windows.Forms.Label();
            this.tblMultiColumn = new System.Windows.Forms.TableLayoutPanel();
            this.cmbTargetField = new System.Windows.Forms.ComboBox();
            this.cmbSecondaryColumn = new System.Windows.Forms.ComboBox();
            this.lblTargetField = new System.Windows.Forms.Label();
            this.lblSecondaryColumn = new System.Windows.Forms.Label();
            this.gbRangeSelection = new System.Windows.Forms.GroupBox();
            this.tblCurrentSelectedRange = new System.Windows.Forms.TableLayoutPanel();
            this.flpCurrentSelectedRangeLables = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCurrentSelectedRangeText = new System.Windows.Forms.Label();
            this.lblGetCurrentSelectionText = new System.Windows.Forms.Label();
            this.flpCurrentSelectionRangeButton = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHighlightText = new System.Windows.Forms.Label();
            this.btnCurrentSelection = new System.Windows.Forms.Button();
            this.flpRefreshData = new System.Windows.Forms.FlowLayoutPanel();
            this.chkRefreshData = new System.Windows.Forms.CheckBox();
            this.tblSearchAndSelect = new System.Windows.Forms.TableLayoutPanel();
            this.chkFieldMapping = new System.Windows.Forms.CheckBox();
            this.lblConfigureSS = new System.Windows.Forms.Label();
            this.tblSSEnableFieldMapping = new System.Windows.Forms.TableLayoutPanel();
            this.lblSSRecordIDText = new System.Windows.Forms.Label();
            this.flpMappedRecordId = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMappedRecordId = new System.Windows.Forms.Label();
            this.cmbRecordId = new System.Windows.Forms.ComboBox();
            this.flpActions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearchAndSelectActions = new System.Windows.Forms.Label();
            this.cmboActionName = new System.Windows.Forms.ComboBox();
            this.lnkAddSearchAndSelect = new System.Windows.Forms.LinkLabel();
            this.lookAheadErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.lookAheadFieldMappingView = new Apttus.XAuthor.AppDesigner.FieldMappingView();
            this.tblMain.SuspendLayout();
            this.flpBottom.SuspendLayout();
            this.tblMappedFields.SuspendLayout();
            this.tblDataSourceSelection.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tblLookAheadSource.SuspendLayout();
            this.tblExcel.SuspendLayout();
            this.gbColumnMapping.SuspendLayout();
            this.tblColumnMapping.SuspendLayout();
            this.tblSingleMultiCol.SuspendLayout();
            this.tblMappingContainer.SuspendLayout();
            this.tblSingleColumn.SuspendLayout();
            this.flpPrimaryColumn.SuspendLayout();
            this.tblMultiColumn.SuspendLayout();
            this.gbRangeSelection.SuspendLayout();
            this.tblCurrentSelectedRange.SuspendLayout();
            this.flpCurrentSelectedRangeLables.SuspendLayout();
            this.flpCurrentSelectionRangeButton.SuspendLayout();
            this.flpRefreshData.SuspendLayout();
            this.tblSearchAndSelect.SuspendLayout();
            this.tblSSEnableFieldMapping.SuspendLayout();
            this.flpMappedRecordId.SuspendLayout();
            this.flpActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lookAheadErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.AutoSize = true;
            this.tblMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMain.Controls.Add(this.flpBottom, 0, 3);
            this.tblMain.Controls.Add(this.tblMappedFields, 0, 0);
            this.tblMain.Controls.Add(this.tblDataSourceSelection, 0, 1);
            this.tblMain.Controls.Add(this.tblLookAheadSource, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(979, 422);
            this.tblMain.TabIndex = 0;
            // 
            // flpBottom
            // 
            this.flpBottom.AutoSize = true;
            this.flpBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpBottom.CausesValidation = false;
            this.flpBottom.Controls.Add(this.btnClose);
            this.flpBottom.Controls.Add(this.btnRemove);
            this.flpBottom.Controls.Add(this.btnSave);
            this.flpBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpBottom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.flpBottom.Location = new System.Drawing.Point(3, 375);
            this.flpBottom.Name = "flpBottom";
            this.flpBottom.Size = new System.Drawing.Size(973, 44);
            this.flpBottom.TabIndex = 17;
            // 
            // btnClose
            // 
            this.btnClose.CausesValidation = false;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.Location = new System.Drawing.Point(872, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 31);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.CausesValidation = false;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRemove.Location = new System.Drawing.Point(768, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(98, 31);
            this.btnRemove.TabIndex = 9;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSave.Location = new System.Drawing.Point(668, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 31);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tblMappedFields
            // 
            this.tblMappedFields.AutoSize = true;
            this.tblMappedFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMappedFields.ColumnCount = 2;
            this.tblMappedFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMappedFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMappedFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMappedFields.Controls.Add(this.lblMappedFieldName, 1, 0);
            this.tblMappedFields.Controls.Add(this.lblTitle, 0, 0);
            this.tblMappedFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMappedFields.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblMappedFields.Location = new System.Drawing.Point(3, 3);
            this.tblMappedFields.Name = "tblMappedFields";
            this.tblMappedFields.RowCount = 1;
            this.tblMappedFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMappedFields.Size = new System.Drawing.Size(973, 27);
            this.tblMappedFields.TabIndex = 17;
            // 
            // lblMappedFieldName
            // 
            this.lblMappedFieldName.AutoSize = true;
            this.lblMappedFieldName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblMappedFieldName.Location = new System.Drawing.Point(118, 3);
            this.lblMappedFieldName.Margin = new System.Windows.Forms.Padding(3);
            this.lblMappedFieldName.Name = "lblMappedFieldName";
            this.lblMappedFieldName.Size = new System.Drawing.Size(150, 21);
            this.lblMappedFieldName.TabIndex = 11;
            this.lblMappedFieldName.Text = "MappedFieldName";
            this.lblMappedFieldName.TextChanged += new System.EventHandler(this.lblMappedFieldName_TextChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTitle.ForeColor = System.Drawing.Color.Green;
            this.lblTitle.Location = new System.Drawing.Point(3, 3);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(109, 21);
            this.lblTitle.TabIndex = 16;
            this.lblTitle.Text = "LookAheadUI";
            // 
            // tblDataSourceSelection
            // 
            this.tblDataSourceSelection.AutoSize = true;
            this.tblDataSourceSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblDataSourceSelection.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetPartial;
            this.tblDataSourceSelection.ColumnCount = 1;
            this.tblDataSourceSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblDataSourceSelection.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tblDataSourceSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblDataSourceSelection.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblDataSourceSelection.Location = new System.Drawing.Point(3, 36);
            this.tblDataSourceSelection.Name = "tblDataSourceSelection";
            this.tblDataSourceSelection.RowCount = 1;
            this.tblDataSourceSelection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblDataSourceSelection.Size = new System.Drawing.Size(973, 31);
            this.tblDataSourceSelection.TabIndex = 18;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.rdbSearchAndSelect);
            this.flowLayoutPanel2.Controls.Add(this.rdbExcel);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(967, 25);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // rdbSearchAndSelect
            // 
            this.rdbSearchAndSelect.AutoSize = true;
            this.rdbSearchAndSelect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdbSearchAndSelect.Location = new System.Drawing.Point(3, 3);
            this.rdbSearchAndSelect.Name = "rdbSearchAndSelect";
            this.rdbSearchAndSelect.Size = new System.Drawing.Size(119, 19);
            this.rdbSearchAndSelect.TabIndex = 1;
            this.rdbSearchAndSelect.Text = "Search And Select";
            this.rdbSearchAndSelect.UseVisualStyleBackColor = true;
            this.rdbSearchAndSelect.CheckedChanged += new System.EventHandler(this.RdbSearchAndSelect_CheckedChanged);
            // 
            // rdbExcel
            // 
            this.rdbExcel.AutoSize = true;
            this.rdbExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdbExcel.Location = new System.Drawing.Point(128, 3);
            this.rdbExcel.Name = "rdbExcel";
            this.rdbExcel.Size = new System.Drawing.Size(51, 19);
            this.rdbExcel.TabIndex = 0;
            this.rdbExcel.Text = "Excel";
            this.rdbExcel.UseVisualStyleBackColor = true;
            this.rdbExcel.CheckedChanged += new System.EventHandler(this.RdbExcel_CheckedChanged);
            // 
            // tblLookAheadSource
            // 
            this.tblLookAheadSource.AutoSize = true;
            this.tblLookAheadSource.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblLookAheadSource.ColumnCount = 2;
            this.tblLookAheadSource.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLookAheadSource.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLookAheadSource.Controls.Add(this.tblExcel, 0, 0);
            this.tblLookAheadSource.Controls.Add(this.tblSearchAndSelect, 1, 0);
            this.tblLookAheadSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLookAheadSource.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblLookAheadSource.Location = new System.Drawing.Point(0, 70);
            this.tblLookAheadSource.Margin = new System.Windows.Forms.Padding(0);
            this.tblLookAheadSource.Name = "tblLookAheadSource";
            this.tblLookAheadSource.RowCount = 1;
            this.tblLookAheadSource.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblLookAheadSource.Size = new System.Drawing.Size(979, 302);
            this.tblLookAheadSource.TabIndex = 19;
            // 
            // tblExcel
            // 
            this.tblExcel.AutoSize = true;
            this.tblExcel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblExcel.ColumnCount = 1;
            this.tblExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExcel.Controls.Add(this.lblConfigureExcel, 0, 0);
            this.tblExcel.Controls.Add(this.gbColumnMapping, 0, 2);
            this.tblExcel.Controls.Add(this.gbRangeSelection, 0, 1);
            this.tblExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblExcel.Location = new System.Drawing.Point(3, 3);
            this.tblExcel.Name = "tblExcel";
            this.tblExcel.RowCount = 3;
            this.tblExcel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblExcel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblExcel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblExcel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblExcel.Size = new System.Drawing.Size(450, 296);
            this.tblExcel.TabIndex = 18;
            this.tblExcel.Visible = false;
            // 
            // lblConfigureExcel
            // 
            this.lblConfigureExcel.AutoSize = true;
            this.lblConfigureExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblConfigureExcel.Location = new System.Drawing.Point(3, 0);
            this.lblConfigureExcel.Name = "lblConfigureExcel";
            this.lblConfigureExcel.Size = new System.Drawing.Size(171, 15);
            this.lblConfigureExcel.TabIndex = 16;
            this.lblConfigureExcel.Text = "Configure an Excel Data Source";
            // 
            // gbColumnMapping
            // 
            this.gbColumnMapping.AutoSize = true;
            this.gbColumnMapping.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbColumnMapping.Controls.Add(this.tblColumnMapping);
            this.gbColumnMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbColumnMapping.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.gbColumnMapping.Location = new System.Drawing.Point(3, 133);
            this.gbColumnMapping.Name = "gbColumnMapping";
            this.gbColumnMapping.Size = new System.Drawing.Size(444, 160);
            this.gbColumnMapping.TabIndex = 18;
            this.gbColumnMapping.TabStop = false;
            this.gbColumnMapping.Text = "Column Mapping";
            // 
            // tblColumnMapping
            // 
            this.tblColumnMapping.AutoSize = true;
            this.tblColumnMapping.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblColumnMapping.ColumnCount = 1;
            this.tblColumnMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblColumnMapping.Controls.Add(this.tblSingleMultiCol, 0, 0);
            this.tblColumnMapping.Controls.Add(this.tblMappingContainer, 0, 1);
            this.tblColumnMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblColumnMapping.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblColumnMapping.Location = new System.Drawing.Point(3, 19);
            this.tblColumnMapping.Name = "tblColumnMapping";
            this.tblColumnMapping.RowCount = 2;
            this.tblColumnMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblColumnMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblColumnMapping.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblColumnMapping.Size = new System.Drawing.Size(438, 138);
            this.tblColumnMapping.TabIndex = 0;
            // 
            // tblSingleMultiCol
            // 
            this.tblSingleMultiCol.AutoSize = true;
            this.tblSingleMultiCol.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSingleMultiCol.ColumnCount = 2;
            this.tblSingleMultiCol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSingleMultiCol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSingleMultiCol.Controls.Add(this.rdbMultiple, 1, 0);
            this.tblSingleMultiCol.Controls.Add(this.rdbSingle, 0, 0);
            this.tblSingleMultiCol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSingleMultiCol.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblSingleMultiCol.Location = new System.Drawing.Point(3, 3);
            this.tblSingleMultiCol.Name = "tblSingleMultiCol";
            this.tblSingleMultiCol.RowCount = 1;
            this.tblSingleMultiCol.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSingleMultiCol.Size = new System.Drawing.Size(432, 19);
            this.tblSingleMultiCol.TabIndex = 31;
            // 
            // rdbMultiple
            // 
            this.rdbMultiple.AutoSize = true;
            this.rdbMultiple.Enabled = false;
            this.rdbMultiple.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbMultiple.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdbMultiple.Location = new System.Drawing.Point(118, 0);
            this.rdbMultiple.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.rdbMultiple.Name = "rdbMultiple";
            this.rdbMultiple.Size = new System.Drawing.Size(98, 19);
            this.rdbMultiple.TabIndex = 30;
            this.rdbMultiple.Text = "Multi Column";
            this.rdbMultiple.UseVisualStyleBackColor = true;
            this.rdbMultiple.CheckedChanged += new System.EventHandler(this.RdbMultiple_CheckedChanged);
            // 
            // rdbSingle
            // 
            this.rdbSingle.AutoSize = true;
            this.rdbSingle.Checked = true;
            this.rdbSingle.Enabled = false;
            this.rdbSingle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdbSingle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdbSingle.Location = new System.Drawing.Point(10, 0);
            this.rdbSingle.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.rdbSingle.Name = "rdbSingle";
            this.rdbSingle.Size = new System.Drawing.Size(102, 19);
            this.rdbSingle.TabIndex = 29;
            this.rdbSingle.TabStop = true;
            this.rdbSingle.Text = "Single Column";
            this.rdbSingle.UseVisualStyleBackColor = true;
            this.rdbSingle.CheckedChanged += new System.EventHandler(this.RdbSingle_CheckedChanged);
            // 
            // tblMappingContainer
            // 
            this.tblMappingContainer.AutoSize = true;
            this.tblMappingContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMappingContainer.ColumnCount = 1;
            this.tblMappingContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMappingContainer.Controls.Add(this.tblSingleColumn, 0, 0);
            this.tblMappingContainer.Controls.Add(this.tblMultiColumn, 0, 1);
            this.tblMappingContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMappingContainer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblMappingContainer.Location = new System.Drawing.Point(3, 28);
            this.tblMappingContainer.MinimumSize = new System.Drawing.Size(296, 107);
            this.tblMappingContainer.Name = "tblMappingContainer";
            this.tblMappingContainer.RowCount = 2;
            this.tblMappingContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMappingContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMappingContainer.Size = new System.Drawing.Size(432, 107);
            this.tblMappingContainer.TabIndex = 32;
            // 
            // tblSingleColumn
            // 
            this.tblSingleColumn.AutoSize = true;
            this.tblSingleColumn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSingleColumn.ColumnCount = 1;
            this.tblSingleColumn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSingleColumn.Controls.Add(this.lblPrimaryColumn, 0, 0);
            this.tblSingleColumn.Controls.Add(this.flpPrimaryColumn, 0, 1);
            this.tblSingleColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSingleColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblSingleColumn.Location = new System.Drawing.Point(3, 3);
            this.tblSingleColumn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tblSingleColumn.Name = "tblSingleColumn";
            this.tblSingleColumn.RowCount = 2;
            this.tblSingleColumn.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSingleColumn.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSingleColumn.Size = new System.Drawing.Size(426, 50);
            this.tblSingleColumn.TabIndex = 0;
            // 
            // lblPrimaryColumn
            // 
            this.lblPrimaryColumn.AutoSize = true;
            this.lblPrimaryColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPrimaryColumn.Location = new System.Drawing.Point(3, 0);
            this.lblPrimaryColumn.Name = "lblPrimaryColumn";
            this.lblPrimaryColumn.Size = new System.Drawing.Size(94, 15);
            this.lblPrimaryColumn.TabIndex = 33;
            this.lblPrimaryColumn.Text = "Primary Column";
            // 
            // flpPrimaryColumn
            // 
            this.flpPrimaryColumn.AutoSize = true;
            this.flpPrimaryColumn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpPrimaryColumn.Controls.Add(this.cmbPrimary);
            this.flpPrimaryColumn.Controls.Add(this.lblPrimaryColumnText);
            this.flpPrimaryColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpPrimaryColumn.Location = new System.Drawing.Point(3, 18);
            this.flpPrimaryColumn.Name = "flpPrimaryColumn";
            this.flpPrimaryColumn.Size = new System.Drawing.Size(420, 29);
            this.flpPrimaryColumn.TabIndex = 11;
            // 
            // cmbPrimary
            // 
            this.cmbPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrimary.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPrimary.FormattingEnabled = true;
            this.cmbPrimary.Location = new System.Drawing.Point(0, 3);
            this.cmbPrimary.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.cmbPrimary.Name = "cmbPrimary";
            this.cmbPrimary.Size = new System.Drawing.Size(200, 23);
            this.cmbPrimary.TabIndex = 34;
            // 
            // lblPrimaryColumnText
            // 
            this.lblPrimaryColumnText.AutoSize = true;
            this.lblPrimaryColumnText.Location = new System.Drawing.Point(206, 0);
            this.lblPrimaryColumnText.Name = "lblPrimaryColumnText";
            this.lblPrimaryColumnText.Size = new System.Drawing.Size(0, 15);
            this.lblPrimaryColumnText.TabIndex = 35;
            // 
            // tblMultiColumn
            // 
            this.tblMultiColumn.AutoSize = true;
            this.tblMultiColumn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMultiColumn.ColumnCount = 2;
            this.tblMultiColumn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMultiColumn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMultiColumn.Controls.Add(this.cmbTargetField, 1, 1);
            this.tblMultiColumn.Controls.Add(this.cmbSecondaryColumn, 0, 1);
            this.tblMultiColumn.Controls.Add(this.lblTargetField, 1, 0);
            this.tblMultiColumn.Controls.Add(this.lblSecondaryColumn, 0, 0);
            this.tblMultiColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMultiColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblMultiColumn.Location = new System.Drawing.Point(3, 53);
            this.tblMultiColumn.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tblMultiColumn.Name = "tblMultiColumn";
            this.tblMultiColumn.RowCount = 2;
            this.tblMultiColumn.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMultiColumn.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMultiColumn.Size = new System.Drawing.Size(426, 51);
            this.tblMultiColumn.TabIndex = 11;
            // 
            // cmbTargetField
            // 
            this.cmbTargetField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetField.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTargetField.FormattingEnabled = true;
            this.cmbTargetField.Location = new System.Drawing.Point(216, 18);
            this.cmbTargetField.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbTargetField.Name = "cmbTargetField";
            this.cmbTargetField.Size = new System.Drawing.Size(200, 23);
            this.cmbTargetField.TabIndex = 41;
            this.cmbTargetField.DropDown += new System.EventHandler(this.cmbTargetField_DropDown);
            this.cmbTargetField.Validating += new System.ComponentModel.CancelEventHandler(this.cmbTargetField_Validating);
            // 
            // cmbSecondaryColumn
            // 
            this.cmbSecondaryColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecondaryColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbSecondaryColumn.FormattingEnabled = true;
            this.cmbSecondaryColumn.Location = new System.Drawing.Point(3, 18);
            this.cmbSecondaryColumn.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmbSecondaryColumn.Name = "cmbSecondaryColumn";
            this.cmbSecondaryColumn.Size = new System.Drawing.Size(200, 23);
            this.cmbSecondaryColumn.TabIndex = 40;
            this.cmbSecondaryColumn.SelectedIndexChanged += new System.EventHandler(this.cmbSecondaryColumn_SelectedIndexChanged);
            this.cmbSecondaryColumn.Validating += new System.ComponentModel.CancelEventHandler(this.cmbSecondaryColumn_Validating);
            // 
            // lblTargetField
            // 
            this.lblTargetField.AutoSize = true;
            this.lblTargetField.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTargetField.Location = new System.Drawing.Point(216, 0);
            this.lblTargetField.Name = "lblTargetField";
            this.lblTargetField.Size = new System.Drawing.Size(172, 15);
            this.lblTargetField.TabIndex = 39;
            this.lblTargetField.Text = "Secondary Column Target Field";
            // 
            // lblSecondaryColumn
            // 
            this.lblSecondaryColumn.AutoSize = true;
            this.lblSecondaryColumn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSecondaryColumn.Location = new System.Drawing.Point(3, 0);
            this.lblSecondaryColumn.Name = "lblSecondaryColumn";
            this.lblSecondaryColumn.Size = new System.Drawing.Size(108, 15);
            this.lblSecondaryColumn.TabIndex = 38;
            this.lblSecondaryColumn.Text = "Secondary Column";
            // 
            // gbRangeSelection
            // 
            this.gbRangeSelection.AutoSize = true;
            this.gbRangeSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbRangeSelection.Controls.Add(this.tblCurrentSelectedRange);
            this.gbRangeSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRangeSelection.Location = new System.Drawing.Point(3, 18);
            this.gbRangeSelection.Name = "gbRangeSelection";
            this.gbRangeSelection.Size = new System.Drawing.Size(444, 109);
            this.gbRangeSelection.TabIndex = 20;
            this.gbRangeSelection.TabStop = false;
            this.gbRangeSelection.Text = "Range Selection";
            // 
            // tblCurrentSelectedRange
            // 
            this.tblCurrentSelectedRange.AutoSize = true;
            this.tblCurrentSelectedRange.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblCurrentSelectedRange.ColumnCount = 1;
            this.tblCurrentSelectedRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCurrentSelectedRange.Controls.Add(this.flpCurrentSelectedRangeLables, 0, 0);
            this.tblCurrentSelectedRange.Controls.Add(this.flpCurrentSelectionRangeButton, 0, 1);
            this.tblCurrentSelectedRange.Controls.Add(this.flpRefreshData, 0, 2);
            this.tblCurrentSelectedRange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCurrentSelectedRange.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblCurrentSelectedRange.Location = new System.Drawing.Point(3, 19);
            this.tblCurrentSelectedRange.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tblCurrentSelectedRange.Name = "tblCurrentSelectedRange";
            this.tblCurrentSelectedRange.RowCount = 3;
            this.tblCurrentSelectedRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurrentSelectedRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurrentSelectedRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurrentSelectedRange.Size = new System.Drawing.Size(438, 87);
            this.tblCurrentSelectedRange.TabIndex = 38;
            // 
            // flpCurrentSelectedRangeLables
            // 
            this.flpCurrentSelectedRangeLables.AutoSize = true;
            this.flpCurrentSelectedRangeLables.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpCurrentSelectedRangeLables.Controls.Add(this.lblCurrentSelectedRangeText);
            this.flpCurrentSelectedRangeLables.Controls.Add(this.lblGetCurrentSelectionText);
            this.flpCurrentSelectedRangeLables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCurrentSelectedRangeLables.Location = new System.Drawing.Point(3, 3);
            this.flpCurrentSelectedRangeLables.Name = "flpCurrentSelectedRangeLables";
            this.flpCurrentSelectedRangeLables.Size = new System.Drawing.Size(432, 19);
            this.flpCurrentSelectedRangeLables.TabIndex = 42;
            // 
            // lblCurrentSelectedRangeText
            // 
            this.lblCurrentSelectedRangeText.AutoSize = true;
            this.lblCurrentSelectedRangeText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentSelectedRangeText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCurrentSelectedRangeText.Location = new System.Drawing.Point(3, 0);
            this.lblCurrentSelectedRangeText.Name = "lblCurrentSelectedRangeText";
            this.lblCurrentSelectedRangeText.Size = new System.Drawing.Size(136, 19);
            this.lblCurrentSelectedRangeText.TabIndex = 36;
            this.lblCurrentSelectedRangeText.Text = "Current Selected Range :";
            // 
            // lblGetCurrentSelectionText
            // 
            this.lblGetCurrentSelectionText.AutoSize = true;
            this.lblGetCurrentSelectionText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGetCurrentSelectionText.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblGetCurrentSelectionText.Location = new System.Drawing.Point(145, 0);
            this.lblGetCurrentSelectionText.Name = "lblGetCurrentSelectionText";
            this.lblGetCurrentSelectionText.Size = new System.Drawing.Size(93, 19);
            this.lblGetCurrentSelectionText.TabIndex = 42;
            this.lblGetCurrentSelectionText.Text = "NamedRange";
            this.lblGetCurrentSelectionText.TextChanged += new System.EventHandler(this.lblGetCurrentSelectionText_TextChanged);
            this.lblGetCurrentSelectionText.Validating += new System.ComponentModel.CancelEventHandler(this.lblGetCurrentSelectionText_Validating);
            // 
            // flpCurrentSelectionRangeButton
            // 
            this.flpCurrentSelectionRangeButton.AutoSize = true;
            this.flpCurrentSelectionRangeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpCurrentSelectionRangeButton.Controls.Add(this.lblHighlightText);
            this.flpCurrentSelectionRangeButton.Controls.Add(this.btnCurrentSelection);
            this.flpCurrentSelectionRangeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCurrentSelectionRangeButton.Location = new System.Drawing.Point(3, 28);
            this.flpCurrentSelectionRangeButton.Name = "flpCurrentSelectionRangeButton";
            this.flpCurrentSelectionRangeButton.Size = new System.Drawing.Size(432, 31);
            this.flpCurrentSelectionRangeButton.TabIndex = 43;
            // 
            // lblHighlightText
            // 
            this.lblHighlightText.AutoSize = true;
            this.lblHighlightText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHighlightText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHighlightText.Location = new System.Drawing.Point(3, 6);
            this.lblHighlightText.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.lblHighlightText.Name = "lblHighlightText";
            this.lblHighlightText.Size = new System.Drawing.Size(227, 25);
            this.lblHighlightText.TabIndex = 40;
            this.lblHighlightText.Text = "Highlight the Excel cell range and click on\r\n";
            // 
            // btnCurrentSelection
            // 
            this.btnCurrentSelection.AutoSize = true;
            this.btnCurrentSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCurrentSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCurrentSelection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCurrentSelection.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCurrentSelection.Location = new System.Drawing.Point(233, 3);
            this.btnCurrentSelection.MaximumSize = new System.Drawing.Size(135, 25);
            this.btnCurrentSelection.Name = "btnCurrentSelection";
            this.btnCurrentSelection.Size = new System.Drawing.Size(131, 25);
            this.btnCurrentSelection.TabIndex = 41;
            this.btnCurrentSelection.Text = "Get Current Selection";
            this.btnCurrentSelection.UseVisualStyleBackColor = true;
            this.btnCurrentSelection.Click += new System.EventHandler(this.BtnCurrentSelection_Click);
            // 
            // flpRefreshData
            // 
            this.flpRefreshData.AutoSize = true;
            this.flpRefreshData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpRefreshData.Controls.Add(this.chkRefreshData);
            this.flpRefreshData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRefreshData.Location = new System.Drawing.Point(3, 62);
            this.flpRefreshData.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flpRefreshData.Name = "flpRefreshData";
            this.flpRefreshData.Size = new System.Drawing.Size(432, 25);
            this.flpRefreshData.TabIndex = 44;
            // 
            // chkRefreshData
            // 
            this.chkRefreshData.AutoSize = true;
            this.chkRefreshData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkRefreshData.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkRefreshData.Location = new System.Drawing.Point(5, 3);
            this.chkRefreshData.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.chkRefreshData.Name = "chkRefreshData";
            this.chkRefreshData.Size = new System.Drawing.Size(162, 19);
            this.chkRefreshData.TabIndex = 40;
            this.chkRefreshData.Text = "Refresh data on every load";
            this.chkRefreshData.UseVisualStyleBackColor = true;
            // 
            // tblSearchAndSelect
            // 
            this.tblSearchAndSelect.AutoSize = true;
            this.tblSearchAndSelect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSearchAndSelect.ColumnCount = 1;
            this.tblSearchAndSelect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSearchAndSelect.Controls.Add(this.chkFieldMapping, 0, 2);
            this.tblSearchAndSelect.Controls.Add(this.lblConfigureSS, 0, 0);
            this.tblSearchAndSelect.Controls.Add(this.tblSSEnableFieldMapping, 0, 3);
            this.tblSearchAndSelect.Controls.Add(this.flpActions, 0, 1);
            this.tblSearchAndSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSearchAndSelect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblSearchAndSelect.Location = new System.Drawing.Point(461, 5);
            this.tblSearchAndSelect.Margin = new System.Windows.Forms.Padding(5);
            this.tblSearchAndSelect.Name = "tblSearchAndSelect";
            this.tblSearchAndSelect.RowCount = 4;
            this.tblSearchAndSelect.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSearchAndSelect.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSearchAndSelect.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSearchAndSelect.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSearchAndSelect.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblSearchAndSelect.Size = new System.Drawing.Size(513, 292);
            this.tblSearchAndSelect.TabIndex = 17;
            this.tblSearchAndSelect.Visible = false;
            // 
            // chkFieldMapping
            // 
            this.chkFieldMapping.AutoSize = true;
            this.chkFieldMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkFieldMapping.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkFieldMapping.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkFieldMapping.Location = new System.Drawing.Point(5, 59);
            this.chkFieldMapping.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.chkFieldMapping.Name = "chkFieldMapping";
            this.chkFieldMapping.Size = new System.Drawing.Size(505, 19);
            this.chkFieldMapping.TabIndex = 20;
            this.chkFieldMapping.Text = "Enable Field Mapping";
            this.chkFieldMapping.UseVisualStyleBackColor = true;
            this.chkFieldMapping.Visible = false;
            this.chkFieldMapping.CheckedChanged += new System.EventHandler(this.chkFieldMapping_CheckedChanged);
            // 
            // lblConfigureSS
            // 
            this.lblConfigureSS.AutoSize = true;
            this.lblConfigureSS.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblConfigureSS.Location = new System.Drawing.Point(3, 3);
            this.lblConfigureSS.Margin = new System.Windows.Forms.Padding(3);
            this.lblConfigureSS.Name = "lblConfigureSS";
            this.lblConfigureSS.Size = new System.Drawing.Size(230, 15);
            this.lblConfigureSS.TabIndex = 17;
            this.lblConfigureSS.Text = "Configure a Search and Select Data Source";
            // 
            // tblSSEnableFieldMapping
            // 
            this.tblSSEnableFieldMapping.AutoSize = true;
            this.tblSSEnableFieldMapping.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSSEnableFieldMapping.ColumnCount = 1;
            this.tblSSEnableFieldMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSSEnableFieldMapping.Controls.Add(this.lblSSRecordIDText, 0, 0);
            this.tblSSEnableFieldMapping.Controls.Add(this.flpMappedRecordId, 0, 1);
            this.tblSSEnableFieldMapping.Controls.Add(this.lookAheadFieldMappingView, 0, 2);
            this.tblSSEnableFieldMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSSEnableFieldMapping.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tblSSEnableFieldMapping.Location = new System.Drawing.Point(3, 84);
            this.tblSSEnableFieldMapping.Name = "tblSSEnableFieldMapping";
            this.tblSSEnableFieldMapping.RowCount = 3;
            this.tblSSEnableFieldMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSSEnableFieldMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSSEnableFieldMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSSEnableFieldMapping.Size = new System.Drawing.Size(507, 205);
            this.tblSSEnableFieldMapping.TabIndex = 21;
            // 
            // lblSSRecordIDText
            // 
            this.lblSSRecordIDText.AutoSize = true;
            this.lblSSRecordIDText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSSRecordIDText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSSRecordIDText.Location = new System.Drawing.Point(3, 3);
            this.lblSSRecordIDText.Margin = new System.Windows.Forms.Padding(3);
            this.lblSSRecordIDText.MaximumSize = new System.Drawing.Size(415, 30);
            this.lblSSRecordIDText.Name = "lblSSRecordIDText";
            this.lblSSRecordIDText.Size = new System.Drawing.Size(415, 30);
            this.lblSSRecordIDText.TabIndex = 8;
            this.lblSSRecordIDText.Text = "At runtime, the mapped field below will be populated with the record ID \r\nfrom th" +
    "e above Search and Select Action";
            // 
            // flpMappedRecordId
            // 
            this.flpMappedRecordId.AutoSize = true;
            this.flpMappedRecordId.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpMappedRecordId.Controls.Add(this.lblMappedRecordId);
            this.flpMappedRecordId.Controls.Add(this.cmbRecordId);
            this.flpMappedRecordId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMappedRecordId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.flpMappedRecordId.Location = new System.Drawing.Point(0, 39);
            this.flpMappedRecordId.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.flpMappedRecordId.Name = "flpMappedRecordId";
            this.flpMappedRecordId.Size = new System.Drawing.Size(504, 29);
            this.flpMappedRecordId.TabIndex = 9;
            // 
            // lblMappedRecordId
            // 
            this.lblMappedRecordId.AutoSize = true;
            this.lblMappedRecordId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMappedRecordId.Location = new System.Drawing.Point(5, 6);
            this.lblMappedRecordId.Margin = new System.Windows.Forms.Padding(5, 6, 3, 0);
            this.lblMappedRecordId.Name = "lblMappedRecordId";
            this.lblMappedRecordId.Size = new System.Drawing.Size(131, 15);
            this.lblMappedRecordId.TabIndex = 9;
            this.lblMappedRecordId.Text = "Mapped Record ID field";
            // 
            // cmbRecordId
            // 
            this.cmbRecordId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbRecordId.FormattingEnabled = true;
            this.cmbRecordId.Location = new System.Drawing.Point(151, 3);
            this.cmbRecordId.Margin = new System.Windows.Forms.Padding(12, 3, 15, 3);
            this.cmbRecordId.Name = "cmbRecordId";
            this.cmbRecordId.Size = new System.Drawing.Size(250, 23);
            this.cmbRecordId.TabIndex = 10;
            this.cmbRecordId.SelectedIndexChanged += new System.EventHandler(this.cmbRecordId_SelectedIndexChanged);
            this.cmbRecordId.Validating += new System.ComponentModel.CancelEventHandler(this.cmbRecordId_Validating);
            // 
            // flpActions
            // 
            this.flpActions.AutoSize = true;
            this.flpActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpActions.Controls.Add(this.lblSearchAndSelectActions);
            this.flpActions.Controls.Add(this.cmboActionName);
            this.flpActions.Controls.Add(this.lnkAddSearchAndSelect);
            this.flpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpActions.Location = new System.Drawing.Point(3, 24);
            this.flpActions.Name = "flpActions";
            this.flpActions.Size = new System.Drawing.Size(507, 29);
            this.flpActions.TabIndex = 11;
            // 
            // lblSearchAndSelectActions
            // 
            this.lblSearchAndSelectActions.AutoSize = true;
            this.lblSearchAndSelectActions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSearchAndSelectActions.Location = new System.Drawing.Point(5, 7);
            this.lblSearchAndSelectActions.Margin = new System.Windows.Forms.Padding(5, 7, 3, 3);
            this.lblSearchAndSelectActions.Name = "lblSearchAndSelectActions";
            this.lblSearchAndSelectActions.Size = new System.Drawing.Size(142, 15);
            this.lblSearchAndSelectActions.TabIndex = 18;
            this.lblSearchAndSelectActions.Text = "Search and Select Actions";
            // 
            // cmboActionName
            // 
            this.cmboActionName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmboActionName.FormattingEnabled = true;
            this.cmboActionName.ItemHeight = 15;
            this.cmboActionName.Location = new System.Drawing.Point(153, 3);
            this.cmboActionName.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cmboActionName.Name = "cmboActionName";
            this.cmboActionName.Size = new System.Drawing.Size(250, 23);
            this.cmboActionName.TabIndex = 19;
            this.cmboActionName.SelectedIndexChanged += new System.EventHandler(this.cmboActionName_SelectedIndexChanged);
            this.cmboActionName.Validating += new System.ComponentModel.CancelEventHandler(this.cmboActionName_Validating);
            // 
            // lnkAddSearchAndSelect
            // 
            this.lnkAddSearchAndSelect.AutoSize = true;
            this.lnkAddSearchAndSelect.Location = new System.Drawing.Point(416, 7);
            this.lnkAddSearchAndSelect.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lnkAddSearchAndSelect.Name = "lnkAddSearchAndSelect";
            this.lnkAddSearchAndSelect.Size = new System.Drawing.Size(79, 15);
            this.lnkAddSearchAndSelect.TabIndex = 20;
            this.lnkAddSearchAndSelect.TabStop = true;
            this.lnkAddSearchAndSelect.Text = "Create Action";
            this.lnkAddSearchAndSelect.Visible = false;
            this.lnkAddSearchAndSelect.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddSearchAndSelect_LinkClicked);
            // 
            // lookAheadErrorProvider
            // 
            this.lookAheadErrorProvider.ContainerControl = this;
            // 
            // lookAheadFieldMappingView
            // 
            this.lookAheadFieldMappingView.AutoSize = true;
            this.lookAheadFieldMappingView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lookAheadFieldMappingView.BackColor = System.Drawing.SystemColors.Window;
            this.lookAheadFieldMappingView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lookAheadFieldMappingView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lookAheadFieldMappingView.FieldAppObject = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.lookAheadFieldMappingView.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lookAheadFieldMappingView.Location = new System.Drawing.Point(3, 74);
            this.lookAheadFieldMappingView.Name = "lookAheadFieldMappingView";
            this.lookAheadFieldMappingView.RetrieveMap = null;
            this.lookAheadFieldMappingView.SearchAndSelectAction = null;
            this.lookAheadFieldMappingView.Size = new System.Drawing.Size(501, 128);
            this.lookAheadFieldMappingView.TabIndex = 12;
            this.lookAheadFieldMappingView.Visible = false;
            // 
            // LookAheadDesignerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(979, 422);
            this.Controls.Add(this.tblMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Light", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LookAheadDesignerView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Look Ahead";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LookAheadDesignerView_FormClosing);
            this.Load += new System.EventHandler(this.LookAheadDesigner_Load);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.flpBottom.ResumeLayout(false);
            this.tblMappedFields.ResumeLayout(false);
            this.tblMappedFields.PerformLayout();
            this.tblDataSourceSelection.ResumeLayout(false);
            this.tblDataSourceSelection.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.tblLookAheadSource.ResumeLayout(false);
            this.tblLookAheadSource.PerformLayout();
            this.tblExcel.ResumeLayout(false);
            this.tblExcel.PerformLayout();
            this.gbColumnMapping.ResumeLayout(false);
            this.gbColumnMapping.PerformLayout();
            this.tblColumnMapping.ResumeLayout(false);
            this.tblColumnMapping.PerformLayout();
            this.tblSingleMultiCol.ResumeLayout(false);
            this.tblSingleMultiCol.PerformLayout();
            this.tblMappingContainer.ResumeLayout(false);
            this.tblMappingContainer.PerformLayout();
            this.tblSingleColumn.ResumeLayout(false);
            this.tblSingleColumn.PerformLayout();
            this.flpPrimaryColumn.ResumeLayout(false);
            this.flpPrimaryColumn.PerformLayout();
            this.tblMultiColumn.ResumeLayout(false);
            this.tblMultiColumn.PerformLayout();
            this.gbRangeSelection.ResumeLayout(false);
            this.gbRangeSelection.PerformLayout();
            this.tblCurrentSelectedRange.ResumeLayout(false);
            this.tblCurrentSelectedRange.PerformLayout();
            this.flpCurrentSelectedRangeLables.ResumeLayout(false);
            this.flpCurrentSelectedRangeLables.PerformLayout();
            this.flpCurrentSelectionRangeButton.ResumeLayout(false);
            this.flpCurrentSelectionRangeButton.PerformLayout();
            this.flpRefreshData.ResumeLayout(false);
            this.flpRefreshData.PerformLayout();
            this.tblSearchAndSelect.ResumeLayout(false);
            this.tblSearchAndSelect.PerformLayout();
            this.tblSSEnableFieldMapping.ResumeLayout(false);
            this.tblSSEnableFieldMapping.PerformLayout();
            this.flpMappedRecordId.ResumeLayout(false);
            this.flpMappedRecordId.PerformLayout();
            this.flpActions.ResumeLayout(false);
            this.flpActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lookAheadErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flpBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tblDataSourceSelection;
        private System.Windows.Forms.RadioButton rdbExcel;
        private System.Windows.Forms.RadioButton rdbSearchAndSelect;
        private System.Windows.Forms.TableLayoutPanel tblLookAheadSource;
        private System.Windows.Forms.TableLayoutPanel tblExcel;
        private System.Windows.Forms.Label lblConfigureSS;
        private System.Windows.Forms.TableLayoutPanel tblSearchAndSelect;
        private System.Windows.Forms.Label lblConfigureExcel;
        private System.Windows.Forms.TableLayoutPanel tblMappedFields;
        private System.Windows.Forms.Label lblMappedFieldName;
        private System.Windows.Forms.GroupBox gbColumnMapping;
        private System.Windows.Forms.TableLayoutPanel tblColumnMapping;
        private System.Windows.Forms.RadioButton rdbSingle;
        private System.Windows.Forms.RadioButton rdbMultiple;
        private System.Windows.Forms.TableLayoutPanel tblSingleMultiCol;
        private System.Windows.Forms.TableLayoutPanel tblMappingContainer;
        private System.Windows.Forms.TableLayoutPanel tblSingleColumn;
        private System.Windows.Forms.Label lblPrimaryColumn;
        private System.Windows.Forms.ComboBox cmbPrimary;
        private System.Windows.Forms.Label lblSecondaryColumn;
        private System.Windows.Forms.Label lblTargetField;
        private System.Windows.Forms.ComboBox cmbSecondaryColumn;
        private System.Windows.Forms.ComboBox cmbTargetField;
        private System.Windows.Forms.CheckBox chkRefreshData;
        private System.Windows.Forms.TableLayoutPanel tblMultiColumn;
        private System.Windows.Forms.Label lblSearchAndSelectActions;
        private System.Windows.Forms.ComboBox cmboActionName;
        private System.Windows.Forms.CheckBox chkFieldMapping;
        private System.Windows.Forms.TableLayoutPanel tblSSEnableFieldMapping;
        private System.Windows.Forms.Label lblSSRecordIDText;
        private System.Windows.Forms.FlowLayoutPanel flpMappedRecordId;
        private System.Windows.Forms.Label lblMappedRecordId;
        private System.Windows.Forms.ComboBox cmbRecordId;
        private FieldMappingView lookAheadFieldMappingView;
        private System.Windows.Forms.GroupBox gbRangeSelection;
        private System.Windows.Forms.TableLayoutPanel tblCurrentSelectedRange;
        private System.Windows.Forms.FlowLayoutPanel flpCurrentSelectedRangeLables;
        private System.Windows.Forms.Label lblCurrentSelectedRangeText;
        private System.Windows.Forms.Label lblGetCurrentSelectionText;
        private System.Windows.Forms.FlowLayoutPanel flpCurrentSelectionRangeButton;
        private System.Windows.Forms.Label lblHighlightText;
        private System.Windows.Forms.Button btnCurrentSelection;
        private System.Windows.Forms.FlowLayoutPanel flpRefreshData;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flpPrimaryColumn;
        private System.Windows.Forms.Label lblPrimaryColumnText;
        private System.Windows.Forms.ErrorProvider lookAheadErrorProvider;
        private System.Windows.Forms.FlowLayoutPanel flpActions;
        private System.Windows.Forms.LinkLabel lnkAddSearchAndSelect;
    }
}