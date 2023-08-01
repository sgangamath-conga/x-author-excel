namespace Apttus.XAuthor.AppDesigner
{
    partial class CallProcedure
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.cboClass = new System.Windows.Forms.ComboBox();
            this.lblClassName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupMethod = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMethod = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.chkReturnValue = new System.Windows.Forms.CheckBox();
            this.chkEnableCache = new System.Windows.Forms.CheckBox();
            this.chkHasParams = new System.Windows.Forms.CheckBox();
            this.cboReturnObject = new System.Windows.Forms.ComboBox();
            this.lblReturnType = new System.Windows.Forms.Label();
            this.groupParams = new System.Windows.Forms.GroupBox();
            this.tlpParams = new System.Windows.Forms.TableLayoutPanel();
            this.lnkClearAll = new System.Windows.Forms.LinkLabel();
            this.lnkAddParam = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupMethod.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupBox1.Location = new System.Drawing.Point(5, 45);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action Details";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(432, 29);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Action Name :";
            // 
            // txtActionName
            // 
            this.txtActionName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(96, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(313, 23);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // cboClass
            // 
            this.cboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboClass.FormattingEnabled = true;
            this.cboClass.Location = new System.Drawing.Point(68, 3);
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(313, 23);
            this.cboClass.TabIndex = 0;
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblClassName.Location = new System.Drawing.Point(5, 5);
            this.lblClassName.Margin = new System.Windows.Forms.Padding(5);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(40, 15);
            this.lblClassName.TabIndex = 0;
            this.lblClassName.Text = "Class :";
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(364, 3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSave.Location = new System.Drawing.Point(283, 3);
            this.btnSave.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupMethod
            // 
            this.groupMethod.AutoSize = true;
            this.groupMethod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupMethod.Controls.Add(this.tableLayoutPanel3);
            this.groupMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupMethod.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupMethod.Location = new System.Drawing.Point(5, 106);
            this.groupMethod.Margin = new System.Windows.Forms.Padding(5);
            this.groupMethod.Name = "groupMethod";
            this.groupMethod.Size = new System.Drawing.Size(438, 146);
            this.groupMethod.TabIndex = 1;
            this.groupMethod.TabStop = false;
            this.groupMethod.Text = "Salesforce Method Details";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(432, 124);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.lblClassName, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.cboClass, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtMethod, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(426, 58);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(5, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Method :";
            // 
            // txtMethod
            // 
            this.txtMethod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMethod.Location = new System.Drawing.Point(68, 32);
            this.txtMethod.Name = "txtMethod";
            this.txtMethod.Size = new System.Drawing.Size(313, 23);
            this.txtMethod.TabIndex = 1;
            this.txtMethod.Validating += new System.ComponentModel.CancelEventHandler(this.txtMethod_Validating);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.chkReturnValue, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.chkEnableCache, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.chkHasParams, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.cboReturnObject, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblReturnType, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 67);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(426, 54);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // chkReturnValue
            // 
            this.chkReturnValue.AutoSize = true;
            this.chkReturnValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkReturnValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkReturnValue.Location = new System.Drawing.Point(3, 3);
            this.chkReturnValue.Name = "chkReturnValue";
            this.chkReturnValue.Size = new System.Drawing.Size(109, 19);
            this.chkReturnValue.TabIndex = 0;
            this.chkReturnValue.Text = "Has return value";
            this.chkReturnValue.UseVisualStyleBackColor = true;
            this.chkReturnValue.CheckedChanged += new System.EventHandler(this.chkReturnValue_CheckedChanged);
            // 
            // chkEnableCache
            // 
            this.chkEnableCache.AutoSize = true;
            this.chkEnableCache.Checked = true;
            this.chkEnableCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnableCache.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkEnableCache.Location = new System.Drawing.Point(118, 32);
            this.chkEnableCache.Name = "chkEnableCache";
            this.chkEnableCache.Size = new System.Drawing.Size(94, 19);
            this.chkEnableCache.TabIndex = 7;
            this.chkEnableCache.Text = "Enable Cache";
            this.chkEnableCache.UseVisualStyleBackColor = true;
            // 
            // chkHasParams
            // 
            this.chkHasParams.AutoSize = true;
            this.chkHasParams.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkHasParams.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkHasParams.Location = new System.Drawing.Point(3, 32);
            this.chkHasParams.Name = "chkHasParams";
            this.chkHasParams.Size = new System.Drawing.Size(105, 19);
            this.chkHasParams.TabIndex = 1;
            this.chkHasParams.Text = "Has parameters";
            this.chkHasParams.UseVisualStyleBackColor = true;
            this.chkHasParams.CheckedChanged += new System.EventHandler(this.chkHasParams_CheckedChanged);
            // 
            // cboReturnObject
            // 
            this.cboReturnObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReturnObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboReturnObject.FormattingEnabled = true;
            this.cboReturnObject.Location = new System.Drawing.Point(218, 3);
            this.cboReturnObject.Name = "cboReturnObject";
            this.cboReturnObject.Size = new System.Drawing.Size(97, 23);
            this.cboReturnObject.TabIndex = 2;
            this.cboReturnObject.Visible = false;
            this.cboReturnObject.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cboReturnObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboReturnObject_Validating);
            // 
            // lblReturnType
            // 
            this.lblReturnType.AutoSize = true;
            this.lblReturnType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblReturnType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReturnType.Location = new System.Drawing.Point(120, 5);
            this.lblReturnType.Margin = new System.Windows.Forms.Padding(5);
            this.lblReturnType.Name = "lblReturnType";
            this.lblReturnType.Size = new System.Drawing.Size(74, 15);
            this.lblReturnType.TabIndex = 6;
            this.lblReturnType.Text = "Return type :";
            this.lblReturnType.Visible = false;
            // 
            // groupParams
            // 
            this.groupParams.AutoSize = true;
            this.groupParams.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupParams.Controls.Add(this.tableLayoutPanel6);
            this.groupParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupParams.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupParams.Location = new System.Drawing.Point(3, 260);
            this.groupParams.Name = "groupParams";
            this.groupParams.Size = new System.Drawing.Size(442, 59);
            this.groupParams.TabIndex = 2;
            this.groupParams.TabStop = false;
            this.groupParams.Text = "Specify Parameters";
            // 
            // tlpParams
            // 
            this.tlpParams.AutoSize = true;
            this.tlpParams.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpParams.ColumnCount = 6;
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpParams.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tlpParams.Location = new System.Drawing.Point(5, 5);
            this.tlpParams.Margin = new System.Windows.Forms.Padding(5);
            this.tlpParams.Name = "tlpParams";
            this.tlpParams.RowCount = 1;
            this.tlpParams.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpParams.Size = new System.Drawing.Size(426, 1);
            this.tlpParams.TabIndex = 0;
            // 
            // lnkClearAll
            // 
            this.lnkClearAll.AutoSize = true;
            this.lnkClearAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnkClearAll.Location = new System.Drawing.Point(75, 3);
            this.lnkClearAll.Margin = new System.Windows.Forms.Padding(3);
            this.lnkClearAll.Name = "lnkClearAll";
            this.lnkClearAll.Size = new System.Drawing.Size(51, 15);
            this.lnkClearAll.TabIndex = 3;
            this.lnkClearAll.TabStop = true;
            this.lnkClearAll.Text = "Clear All";
            this.lnkClearAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearAll_LinkClicked);
            // 
            // lnkAddParam
            // 
            this.lnkAddParam.AutoSize = true;
            this.lnkAddParam.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnkAddParam.Location = new System.Drawing.Point(3, 3);
            this.lnkAddParam.Margin = new System.Windows.Forms.Padding(3);
            this.lnkAddParam.Name = "lnkAddParam";
            this.lnkAddParam.Size = new System.Drawing.Size(66, 15);
            this.lnkAddParam.TabIndex = 1;
            this.lnkAddParam.TabStop = true;
            this.lnkAddParam.Text = "Add Param";
            this.lnkAddParam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddParam_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(5, 5);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(249, 30);
            this.label3.TabIndex = 0;
            this.label3.Text = "Salesforce Method Action";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupMethod, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupParams, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(445, 363);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel1.Location = new System.Drawing.Point(3, 325);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(442, 35);
            this.panel1.TabIndex = 3;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.tlpParams, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(436, 37);
            this.tableLayoutPanel6.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lnkAddParam);
            this.flowLayoutPanel1.Controls.Add(this.lnkClearAll);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 13);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(430, 21);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // CallProcedure
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(445, 363);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CallProcedure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.CallProcedure_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupMethod.ResumeLayout(false);
            this.groupMethod.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.groupParams.ResumeLayout(false);
            this.groupParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboClass;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupMethod;
        private System.Windows.Forms.TextBox txtMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkHasParams;
        private System.Windows.Forms.GroupBox groupParams;
        private System.Windows.Forms.TableLayoutPanel tlpParams;
        private System.Windows.Forms.LinkLabel lnkAddParam;
        private System.Windows.Forms.LinkLabel lnkClearAll;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkReturnValue;
        private System.Windows.Forms.Label lblReturnType;
        private System.Windows.Forms.ComboBox cboReturnObject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox chkEnableCache;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}