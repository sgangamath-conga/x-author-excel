namespace Apttus.XAuthor.AppDesigner.Forms
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchAndSelectActionView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.SearchUp1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.DefaultValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSearchConfiguration = new System.Windows.Forms.TabControl();
            this.tabSearch = new System.Windows.Forms.TabPage();
            this.tblpnlSearch = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSearch = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.rbMultipleRecords = new System.Windows.Forms.RadioButton();
            this.rbSingleRecord = new System.Windows.Forms.RadioButton();
            this.pnlResult = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnResultUp = new System.Windows.Forms.Button();
            this.btnResultDown = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.ResultSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ResultName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultSort = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.dgSearch = new System.Windows.Forms.DataGridView();
            this.SearchCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SearchFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchFieldLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchDataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchDefaultValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBasicControls = new System.Windows.Forms.Panel();
            this.groupActionHeader = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAllowSavingFilters = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.cboSearchObject = new System.Windows.Forms.ComboBox();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.lblSearchObject = new System.Windows.Forms.Label();
            this.tabFilters = new System.Windows.Forms.TabPage();
            this.pnlExpressionBuilder = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tabSearchConfiguration.SuspendLayout();
            this.tabSearch.SuspendLayout();
            this.tblpnlSearch.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlResult.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSearch)).BeginInit();
            this.pnlBasicControls.SuspendLayout();
            this.groupActionHeader.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabFilters.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // SearchUp1
            // 
            this.SearchUp1.HeaderText = "Up";
            this.SearchUp1.Name = "SearchUp1";
            this.SearchUp1.Text = "Up";
            this.SearchUp1.ToolTipText = "Up";
            this.SearchUp1.UseColumnTextForButtonValue = true;
            this.SearchUp1.Width = 40;
            // 
            // DefaultValue
            // 
            this.DefaultValue.HeaderText = "Default Value";
            this.DefaultValue.Name = "DefaultValue";
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Datatype";
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 30;
            // 
            // Label
            // 
            this.Label.DataPropertyName = "Label";
            this.Label.HeaderText = "Field Label";
            this.Label.Name = "Label";
            // 
            // DataType
            // 
            this.DataType.DataPropertyName = "Datatype";
            this.DataType.HeaderText = "DataType";
            this.DataType.Name = "DataType";
            this.DataType.ReadOnly = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabSearchConfiguration, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(727, 640);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 30);
            this.label1.TabIndex = 5;
            this.label1.Text = "Search And Select";
            // 
            // tabSearchConfiguration
            // 
            this.tabSearchConfiguration.Controls.Add(this.tabSearch);
            this.tabSearchConfiguration.Controls.Add(this.tabFilters);
            this.tabSearchConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSearchConfiguration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSearchConfiguration.Location = new System.Drawing.Point(3, 30);
            this.tabSearchConfiguration.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tabSearchConfiguration.MinimumSize = new System.Drawing.Size(700, 550);
            this.tabSearchConfiguration.Name = "tabSearchConfiguration";
            this.tabSearchConfiguration.Padding = new System.Drawing.Point(10, 3);
            this.tabSearchConfiguration.SelectedIndex = 0;
            this.tabSearchConfiguration.Size = new System.Drawing.Size(721, 550);
            this.tabSearchConfiguration.TabIndex = 0;
            this.tabSearchConfiguration.Tag = "";
            // 
            // tabSearch
            // 
            this.tabSearch.AutoScroll = true;
            this.tabSearch.Controls.Add(this.tblpnlSearch);
            this.tabSearch.Location = new System.Drawing.Point(4, 24);
            this.tabSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tabSearch.Name = "tabSearch";
            this.tabSearch.Size = new System.Drawing.Size(713, 522);
            this.tabSearch.TabIndex = 0;
            this.tabSearch.Text = "Options";
            // 
            // tblpnlSearch
            // 
            this.tblpnlSearch.AutoSize = true;
            this.tblpnlSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblpnlSearch.BackColor = System.Drawing.SystemColors.HighlightText;
            this.tblpnlSearch.ColumnCount = 1;
            this.tblpnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblpnlSearch.Controls.Add(this.pnlSearch, 0, 2);
            this.tblpnlSearch.Controls.Add(this.pnlResult, 0, 3);
            this.tblpnlSearch.Controls.Add(this.panel1, 0, 1);
            this.tblpnlSearch.Controls.Add(this.pnlBasicControls, 0, 0);
            this.tblpnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblpnlSearch.Location = new System.Drawing.Point(0, 0);
            this.tblpnlSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tblpnlSearch.Name = "tblpnlSearch";
            this.tblpnlSearch.RowCount = 4;
            this.tblpnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblpnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblpnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblpnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblpnlSearch.Size = new System.Drawing.Size(713, 522);
            this.tblpnlSearch.TabIndex = 7;
            // 
            // pnlSearch
            // 
            this.pnlSearch.AutoSize = true;
            this.pnlSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearch.BackColor = System.Drawing.SystemColors.Window;
            this.pnlSearch.ColumnCount = 3;
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearch.Controls.Add(this.label4);
            this.pnlSearch.Controls.Add(this.rbMultipleRecords);
            this.pnlSearch.Controls.Add(this.rbSingleRecord);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(3, 286);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSearch.Size = new System.Drawing.Size(707, 19);
            this.pnlSearch.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Result Selection :";
            // 
            // rbMultipleRecords
            // 
            this.rbMultipleRecords.AutoSize = true;
            this.rbMultipleRecords.Enabled = false;
            this.rbMultipleRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMultipleRecords.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbMultipleRecords.Location = new System.Drawing.Point(105, 0);
            this.rbMultipleRecords.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.rbMultipleRecords.Name = "rbMultipleRecords";
            this.rbMultipleRecords.Size = new System.Drawing.Size(68, 19);
            this.rbMultipleRecords.TabIndex = 5;
            this.rbMultipleRecords.TabStop = true;
            this.rbMultipleRecords.Tag = "Multiple";
            this.rbMultipleRecords.Text = "Multiple";
            this.rbMultipleRecords.UseVisualStyleBackColor = true;
            // 
            // rbSingleRecord
            // 
            this.rbSingleRecord.AutoSize = true;
            this.rbSingleRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbSingleRecord.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSingleRecord.Location = new System.Drawing.Point(179, 0);
            this.rbSingleRecord.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.rbSingleRecord.Name = "rbSingleRecord";
            this.rbSingleRecord.Size = new System.Drawing.Size(56, 19);
            this.rbSingleRecord.TabIndex = 4;
            this.rbSingleRecord.TabStop = true;
            this.rbSingleRecord.Tag = "Single";
            this.rbSingleRecord.Text = "Single";
            this.rbSingleRecord.UseVisualStyleBackColor = true;
            // 
            // pnlResult
            // 
            this.pnlResult.ColumnCount = 2;
            this.pnlResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.pnlResult.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.pnlResult.Controls.Add(this.label3, 0, 0);
            this.pnlResult.Controls.Add(this.dgResult, 0, 1);
            this.pnlResult.Location = new System.Drawing.Point(1, 309);
            this.pnlResult.Margin = new System.Windows.Forms.Padding(1);
            this.pnlResult.Name = "pnlResult";
            this.pnlResult.RowCount = 2;
            this.pnlResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlResult.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlResult.Size = new System.Drawing.Size(711, 157);
            this.pnlResult.TabIndex = 12;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnResultUp);
            this.flowLayoutPanel1.Controls.Add(this.btnResultDown);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(672, 16);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(1, 62, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(38, 140);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // btnResultUp
            // 
            this.btnResultUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResultUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResultUp.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.UpArrow;
            this.btnResultUp.Location = new System.Drawing.Point(4, 65);
            this.btnResultUp.Name = "btnResultUp";
            this.btnResultUp.Size = new System.Drawing.Size(30, 30);
            this.btnResultUp.TabIndex = 7;
            this.btnResultUp.UseVisualStyleBackColor = true;
            this.btnResultUp.Click += new System.EventHandler(this.btnResultUp_Click);
            // 
            // btnResultDown
            // 
            this.btnResultDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResultDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResultDown.Image = ((System.Drawing.Image)(resources.GetObject("btnResultDown.Image")));
            this.btnResultDown.Location = new System.Drawing.Point(4, 101);
            this.btnResultDown.Name = "btnResultDown";
            this.btnResultDown.Size = new System.Drawing.Size(30, 30);
            this.btnResultDown.TabIndex = 8;
            this.btnResultDown.UseVisualStyleBackColor = true;
            this.btnResultDown.Click += new System.EventHandler(this.btnResultDown_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Fields to Display in Search Results";
            // 
            // dgResult
            // 
            this.dgResult.AllowUserToAddRows = false;
            this.dgResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgResult.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgResult.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResultSelect,
            this.ResultName,
            this.ResultHeader,
            this.ResultSort});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.MenuText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgResult.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgResult.Location = new System.Drawing.Point(2, 17);
            this.dgResult.Margin = new System.Windows.Forms.Padding(2);
            this.dgResult.MultiSelect = false;
            this.dgResult.Name = "dgResult";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResult.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgResult.RowHeadersVisible = false;
            this.dgResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgResult.Size = new System.Drawing.Size(667, 114);
            this.dgResult.TabIndex = 9;
            this.dgResult.TabStop = false;
            this.dgResult.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResult_RowEnter);
            // 
            // ResultSelect
            // 
            this.ResultSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResultSelect.DataPropertyName = "IsSelected";
            this.ResultSelect.FalseValue = "false";
            this.ResultSelect.FillWeight = 36.54821F;
            this.ResultSelect.HeaderText = "Display";
            this.ResultSelect.IndeterminateValue = "null";
            this.ResultSelect.Name = "ResultSelect";
            this.ResultSelect.TrueValue = "true";
            // 
            // ResultName
            // 
            this.ResultName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResultName.DataPropertyName = "Id";
            this.ResultName.FillWeight = 121.1505F;
            this.ResultName.HeaderText = "Field Name";
            this.ResultName.Name = "ResultName";
            this.ResultName.ReadOnly = true;
            // 
            // ResultHeader
            // 
            this.ResultHeader.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResultHeader.DataPropertyName = "HeaderName";
            this.ResultHeader.FillWeight = 121.1505F;
            this.ResultHeader.HeaderText = "Header Name";
            this.ResultHeader.Name = "ResultHeader";
            // 
            // ResultSort
            // 
            this.ResultSort.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ResultSort.DataPropertyName = "IsSortField";
            this.ResultSort.FalseValue = "false";
            this.ResultSort.FillWeight = 121.1505F;
            this.ResultSort.HeaderText = "Sort By";
            this.ResultSort.IndeterminateValue = "null";
            this.ResultSort.Name = "ResultSort";
            this.ResultSort.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ResultSort.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ResultSort.TrueValue = "true";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dgSearch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 89);
            this.panel1.MinimumSize = new System.Drawing.Size(700, 155);
            this.panel1.Name = "panel1";
            this.panel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel1.Size = new System.Drawing.Size(707, 191);
            this.panel1.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Searchable Fields";
            // 
            // dgSearch
            // 
            this.dgSearch.AllowUserToAddRows = false;
            this.dgSearch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgSearch.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgSearch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgSearch.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SearchCheckBox,
            this.SearchFieldName,
            this.SearchFieldLabel,
            this.SearchDataType,
            this.SearchDefaultValue});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgSearch.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgSearch.Location = new System.Drawing.Point(3, 18);
            this.dgSearch.MaximumSize = new System.Drawing.Size(0, 170);
            this.dgSearch.MultiSelect = false;
            this.dgSearch.Name = "dgSearch";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgSearch.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgSearch.RowHeadersVisible = false;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dgSearch.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgSearch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgSearch.Size = new System.Drawing.Size(701, 170);
            this.dgSearch.TabIndex = 3;
            this.dgSearch.TabStop = false;
            this.dgSearch.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSearch_CellClick);
            this.dgSearch.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSearch_CellContentClick);
            this.dgSearch.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSearch_CellEndEdit);
            this.dgSearch.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSearch_CellValueChanged);
            // 
            // SearchCheckBox
            // 
            this.SearchCheckBox.DataPropertyName = "IsSelected";
            this.SearchCheckBox.FalseValue = "false";
            this.SearchCheckBox.FillWeight = 55.83755F;
            this.SearchCheckBox.HeaderText = "Searchable";
            this.SearchCheckBox.IndeterminateValue = "null";
            this.SearchCheckBox.Name = "SearchCheckBox";
            this.SearchCheckBox.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SearchCheckBox.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SearchCheckBox.TrueValue = "true";
            // 
            // SearchFieldName
            // 
            this.SearchFieldName.DataPropertyName = "Id";
            this.SearchFieldName.FillWeight = 111.0406F;
            this.SearchFieldName.HeaderText = "Field Name";
            this.SearchFieldName.Name = "SearchFieldName";
            this.SearchFieldName.ReadOnly = true;
            // 
            // SearchFieldLabel
            // 
            this.SearchFieldLabel.DataPropertyName = "Label";
            this.SearchFieldLabel.FillWeight = 111.0406F;
            this.SearchFieldLabel.HeaderText = "Field Label";
            this.SearchFieldLabel.Name = "SearchFieldLabel";
            // 
            // SearchDataType
            // 
            this.SearchDataType.DataPropertyName = "Datatype";
            this.SearchDataType.FillWeight = 111.0406F;
            this.SearchDataType.HeaderText = "Data Type";
            this.SearchDataType.Name = "SearchDataType";
            this.SearchDataType.ReadOnly = true;
            // 
            // SearchDefaultValue
            // 
            this.SearchDefaultValue.DataPropertyName = "DefaultValue";
            this.SearchDefaultValue.FillWeight = 111.0406F;
            this.SearchDefaultValue.HeaderText = "Default Value";
            this.SearchDefaultValue.Name = "SearchDefaultValue";
            this.SearchDefaultValue.Visible = false;
            // 
            // pnlBasicControls
            // 
            this.pnlBasicControls.AutoSize = true;
            this.pnlBasicControls.Controls.Add(this.groupActionHeader);
            this.pnlBasicControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBasicControls.Location = new System.Drawing.Point(3, 3);
            this.pnlBasicControls.Name = "pnlBasicControls";
            this.pnlBasicControls.Size = new System.Drawing.Size(707, 80);
            this.pnlBasicControls.TabIndex = 15;
            // 
            // groupActionHeader
            // 
            this.groupActionHeader.AutoSize = true;
            this.groupActionHeader.Controls.Add(this.tableLayoutPanel2);
            this.groupActionHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupActionHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupActionHeader.Location = new System.Drawing.Point(0, 0);
            this.groupActionHeader.Margin = new System.Windows.Forms.Padding(5);
            this.groupActionHeader.Name = "groupActionHeader";
            this.groupActionHeader.Size = new System.Drawing.Size(707, 80);
            this.groupActionHeader.TabIndex = 14;
            this.groupActionHeader.TabStop = false;
            this.groupActionHeader.Text = "Search and Select Action";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tableLayoutPanel2.Controls.Add(this.chkAllowSavingFilters, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cboSearchObject, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSearchObject, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(701, 58);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // chkAllowSavingFilters
            // 
            this.chkAllowSavingFilters.AutoSize = true;
            this.chkAllowSavingFilters.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAllowSavingFilters.Location = new System.Drawing.Point(384, 3);
            this.chkAllowSavingFilters.Name = "chkAllowSavingFilters";
            this.chkAllowSavingFilters.Size = new System.Drawing.Size(140, 19);
            this.chkAllowSavingFilters.TabIndex = 14;
            this.chkAllowSavingFilters.Text = "Enable Favorite Filters";
            this.chkAllowSavingFilters.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.chkAllowSavingFilters.UseVisualStyleBackColor = false;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(5, 5);
            this.lblName.Margin = new System.Windows.Forms.Padding(5);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(83, 15);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Action Name :";
            // 
            // cboSearchObject
            // 
            this.cboSearchObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSearchObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSearchObject.FormattingEnabled = true;
            this.cboSearchObject.Location = new System.Drawing.Point(96, 32);
            this.cboSearchObject.Name = "cboSearchObject";
            this.cboSearchObject.Size = new System.Drawing.Size(282, 23);
            this.cboSearchObject.TabIndex = 1;
            this.cboSearchObject.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cboSearchObject.SelectedIndexChanged += new System.EventHandler(this.cboSearchObject_SelectedIndexChanged);
            // 
            // txtActionName
            // 
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActionName.Location = new System.Drawing.Point(96, 3);
            this.txtActionName.MaxLength = 50;
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(282, 23);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // lblSearchObject
            // 
            this.lblSearchObject.AutoSize = true;
            this.lblSearchObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchObject.Location = new System.Drawing.Point(5, 34);
            this.lblSearchObject.Margin = new System.Windows.Forms.Padding(5);
            this.lblSearchObject.Name = "lblSearchObject";
            this.lblSearchObject.Size = new System.Drawing.Size(48, 15);
            this.lblSearchObject.TabIndex = 0;
            this.lblSearchObject.Text = "Object :";
            // 
            // tabFilters
            // 
            this.tabFilters.Controls.Add(this.pnlExpressionBuilder);
            this.tabFilters.Location = new System.Drawing.Point(4, 24);
            this.tabFilters.Name = "tabFilters";
            this.tabFilters.Size = new System.Drawing.Size(713, 522);
            this.tabFilters.TabIndex = 1;
            this.tabFilters.Text = "Filters";
            this.tabFilters.UseVisualStyleBackColor = true;
            // 
            // pnlExpressionBuilder
            // 
            this.pnlExpressionBuilder.AutoSize = true;
            this.pnlExpressionBuilder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlExpressionBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlExpressionBuilder.Location = new System.Drawing.Point(0, 0);
            this.pnlExpressionBuilder.Name = "pnlExpressionBuilder";
            this.pnlExpressionBuilder.Size = new System.Drawing.Size(713, 522);
            this.pnlExpressionBuilder.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.CausesValidation = false;
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnSave);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 583);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(721, 54);
            this.flowLayoutPanel2.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(616, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(506, 5);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // SearchAndSelectActionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(727, 640);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchAndSelectActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.SearchConfiguration_Load);
            this.Shown += new System.EventHandler(this.SearchConfiguration_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabSearchConfiguration.ResumeLayout(false);
            this.tabSearch.ResumeLayout(false);
            this.tabSearch.PerformLayout();
            this.tblpnlSearch.ResumeLayout(false);
            this.tblpnlSearch.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlResult.ResumeLayout(false);
            this.pnlResult.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSearch)).EndInit();
            this.pnlBasicControls.ResumeLayout(false);
            this.pnlBasicControls.PerformLayout();
            this.groupActionHeader.ResumeLayout(false);
            this.groupActionHeader.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabFilters.ResumeLayout(false);
            this.tabFilters.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridViewButtonColumn SearchUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefaultValue;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataType;
        private System.Windows.Forms.TabControl tabSearchConfiguration;
        private System.Windows.Forms.TabPage tabSearch;
        private System.Windows.Forms.TabPage tabFilters;
        private System.Windows.Forms.DataGridViewButtonColumn SearchUp1;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Panel pnlExpressionBuilder;
        private System.Windows.Forms.TableLayoutPanel tblpnlSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbMultipleRecords;
        private System.Windows.Forms.RadioButton rbSingleRecord;
        private System.Windows.Forms.DataGridView dgSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel panel1;
        private System.Windows.Forms.TableLayoutPanel pnlSearch;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel pnlResult;
        private System.Windows.Forms.DataGridView dgResult;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnResultUp;
        private System.Windows.Forms.Button btnResultDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlBasicControls;
        private System.Windows.Forms.GroupBox groupActionHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ComboBox cboSearchObject;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label lblSearchObject;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ResultSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultHeader;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ResultSort;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SearchCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchFieldLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchDefaultValue;
        private System.Windows.Forms.CheckBox chkAllowSavingFilters;
    }
}