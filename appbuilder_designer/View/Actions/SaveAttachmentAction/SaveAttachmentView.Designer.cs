namespace Apttus.XAuthor.AppDesigner
{
    partial class SaveAttachmentView
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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAction = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoOverrite = new System.Windows.Forms.RadioButton();
            this.rdoAddNew = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFileName = new System.Windows.Forms.TableLayoutPanel();
            this.rbRunTimeFileName = new System.Windows.Forms.RadioButton();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.getCellRefBtn = new System.Windows.Forms.Button();
            this.rbCellReferenceFileName = new System.Windows.Forms.RadioButton();
            this.rbManualFileName = new System.Windows.Forms.RadioButton();
            this.cbCustomSheets = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbExcel = new System.Windows.Forms.RadioButton();
            this.rbPDF = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbSheets = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbSaveAttachment = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlAction.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pnlFileName.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbSaveAttachment.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.pnlAction, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gbSaveAttachment, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(430, 491);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pnlAction
            // 
            this.pnlAction.AutoSize = true;
            this.pnlAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlAction.Controls.Add(this.btnCancel);
            this.pnlAction.Controls.Add(this.btnOK);
            this.pnlAction.Controls.Add(this.rdoOverrite);
            this.pnlAction.Controls.Add(this.rdoAddNew);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAction.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlAction.Location = new System.Drawing.Point(3, 440);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Padding = new System.Windows.Forms.Padding(5);
            this.pnlAction.Size = new System.Drawing.Size(424, 78);
            this.pnlAction.TabIndex = 1;
            // 
            // rdoOverrite
            // 
            this.rdoOverrite.AutoSize = true;
            this.rdoOverrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoOverrite.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoOverrite.Location = new System.Drawing.Point(98, 8);
            this.rdoOverrite.Name = "rdoOverrite";
            this.rdoOverrite.Size = new System.Drawing.Size(93, 24);
            this.rdoOverrite.TabIndex = 4;
            this.rdoOverrite.TabStop = true;
            this.rdoOverrite.Text = "Overwrite";
            this.rdoOverrite.UseVisualStyleBackColor = true;
            this.rdoOverrite.Visible = false;
            // 
            // rdoAddNew
            // 
            this.rdoAddNew.AutoSize = true;
            this.rdoAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoAddNew.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoAddNew.Location = new System.Drawing.Point(320, 46);
            this.rdoAddNew.Name = "rdoAddNew";
            this.rdoAddNew.Size = new System.Drawing.Size(91, 24);
            this.rdoAddNew.TabIndex = 5;
            this.rdoAddNew.TabStop = true;
            this.rdoAddNew.Text = "Add New";
            this.rdoAddNew.UseVisualStyleBackColor = true;
            this.rdoAddNew.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOK.Location = new System.Drawing.Point(197, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 32);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(307, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.pnlFileName, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.cbCustomSheets, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.cboObject, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 40);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(418, 256);
            this.tableLayoutPanel2.TabIndex = 21;
            // 
            // pnlFileName
            // 
            this.pnlFileName.AutoSize = true;
            this.pnlFileName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlFileName.ColumnCount = 2;
            this.pnlFileName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlFileName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlFileName.Controls.Add(this.rbRunTimeFileName);
            this.pnlFileName.Controls.Add(this.txtFileName, 0, 4);
            this.pnlFileName.Controls.Add(this.getCellRefBtn, 1, 4);
            this.pnlFileName.Controls.Add(this.rbCellReferenceFileName, 0, 3);
            this.pnlFileName.Controls.Add(this.rbManualFileName, 0, 1);
            this.pnlFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFileName.Location = new System.Drawing.Point(112, 106);
            this.pnlFileName.Name = "pnlFileName";
            this.pnlFileName.RowCount = 5;
            this.pnlFileName.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFileName.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFileName.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFileName.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFileName.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFileName.Size = new System.Drawing.Size(303, 117);
            this.pnlFileName.TabIndex = 20;
            // 
            // rbRunTimeFileName
            // 
            this.rbRunTimeFileName.AutoSize = true;
            this.rbRunTimeFileName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbRunTimeFileName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbRunTimeFileName.Location = new System.Drawing.Point(3, 3);
            this.rbRunTimeFileName.Name = "rbRunTimeFileName";
            this.rbRunTimeFileName.Size = new System.Drawing.Size(185, 22);
            this.rbRunTimeFileName.TabIndex = 33;
            this.rbRunTimeFileName.Tag = "Runtime";
            this.rbRunTimeFileName.Text = "Use Runtime File Name";
            this.rbRunTimeFileName.UseVisualStyleBackColor = true;
            this.rbRunTimeFileName.CheckedChanged += new System.EventHandler(this.rbRunTimeFileName_CheckedChanged);
            // 
            // txtFileName
            // 
            this.txtFileName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFileName.Location = new System.Drawing.Point(3, 87);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(257, 27);
            this.txtFileName.TabIndex = 30;
            this.txtFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtFileName_Validating);
            // 
            // getCellRefBtn
            // 
            this.getCellRefBtn.AutoSize = true;
            this.getCellRefBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getCellRefBtn.Enabled = false;
            this.getCellRefBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.getCellRefBtn.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.cellSelectionIcon;
            this.getCellRefBtn.Location = new System.Drawing.Point(266, 87);
            this.getCellRefBtn.Name = "getCellRefBtn";
            this.getCellRefBtn.Size = new System.Drawing.Size(29, 26);
            this.getCellRefBtn.TabIndex = 34;
            this.getCellRefBtn.UseVisualStyleBackColor = true;
            this.getCellRefBtn.Click += new System.EventHandler(this.getCellRefBtn_Click);
            // 
            // rbCellReferenceFileName
            // 
            this.rbCellReferenceFileName.AutoSize = true;
            this.rbCellReferenceFileName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbCellReferenceFileName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbCellReferenceFileName.Location = new System.Drawing.Point(3, 59);
            this.rbCellReferenceFileName.Name = "rbCellReferenceFileName";
            this.rbCellReferenceFileName.Size = new System.Drawing.Size(247, 22);
            this.rbCellReferenceFileName.TabIndex = 31;
            this.rbCellReferenceFileName.TabStop = true;
            this.rbCellReferenceFileName.Tag = "CellRef";
            this.rbCellReferenceFileName.Text = "Use Cell Reference as File Name";
            this.rbCellReferenceFileName.UseVisualStyleBackColor = true;
            this.rbCellReferenceFileName.CheckedChanged += new System.EventHandler(this.rbCellReferenceFileName_CheckedChanged);
            // 
            // rbManualFileName
            // 
            this.rbManualFileName.AutoSize = true;
            this.rbManualFileName.Checked = true;
            this.rbManualFileName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbManualFileName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbManualFileName.Location = new System.Drawing.Point(3, 31);
            this.rbManualFileName.Name = "rbManualFileName";
            this.rbManualFileName.Size = new System.Drawing.Size(211, 22);
            this.rbManualFileName.TabIndex = 32;
            this.rbManualFileName.TabStop = true;
            this.rbManualFileName.Tag = "Manual";
            this.rbManualFileName.Text = "Manually Provide File Name";
            this.rbManualFileName.UseVisualStyleBackColor = true;
            this.rbManualFileName.CheckedChanged += new System.EventHandler(this.rbManualFileName_CheckedChanged);
            // 
            // cbCustomSheets
            // 
            this.cbCustomSheets.AutoSize = true;
            this.cbCustomSheets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbCustomSheets.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbCustomSheets.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbCustomSheets.Location = new System.Drawing.Point(112, 229);
            this.cbCustomSheets.Name = "cbCustomSheets";
            this.cbCustomSheets.Size = new System.Drawing.Size(183, 24);
            this.cbCustomSheets.TabIndex = 18;
            this.cbCustomSheets.Text = "Custom Sheet Selection";
            this.cbCustomSheets.UseVisualStyleBackColor = true;
            this.cbCustomSheets.CheckedChanged += new System.EventHandler(this.cbCustomSheets_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action Name :";
            // 
            // txtActionName
            // 
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(112, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(238, 27);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(3, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Object :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(3, 106);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "File Name :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(3, 70);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Format :";
            // 
            // cboObject
            // 
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(112, 36);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(238, 28);
            this.cboObject.TabIndex = 3;
            this.cboObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboObject_Validating);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.rbExcel);
            this.flowLayoutPanel1.Controls.Add(this.rbPDF);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(112, 70);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(303, 30);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // rbExcel
            // 
            this.rbExcel.AutoSize = true;
            this.rbExcel.Checked = true;
            this.rbExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbExcel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbExcel.Location = new System.Drawing.Point(3, 3);
            this.rbExcel.Name = "rbExcel";
            this.rbExcel.Size = new System.Drawing.Size(63, 24);
            this.rbExcel.TabIndex = 0;
            this.rbExcel.TabStop = true;
            this.rbExcel.Text = "Excel";
            this.rbExcel.UseVisualStyleBackColor = true;
            // 
            // rbPDF
            // 
            this.rbPDF.AutoSize = true;
            this.rbPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPDF.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbPDF.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbPDF.Location = new System.Drawing.Point(72, 3);
            this.rbPDF.Name = "rbPDF";
            this.rbPDF.Size = new System.Drawing.Size(55, 24);
            this.rbPDF.TabIndex = 1;
            this.rbPDF.Text = "PDF";
            this.rbPDF.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.ColumnCount = 2;
            this.panel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel1.Controls.Add(this.lbSheets, 1, 0);
            this.panel1.Controls.Add(this.label5, 0, 0);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 308);
            this.panel1.Name = "panel1";
            this.panel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel1.Size = new System.Drawing.Size(424, 126);
            this.panel1.TabIndex = 2;
            // 
            // lbSheets
            // 
            this.lbSheets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSheets.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbSheets.FormattingEnabled = true;
            this.lbSheets.ItemHeight = 20;
            this.lbSheets.Location = new System.Drawing.Point(124, 3);
            this.lbSheets.Name = "lbSheets";
            this.lbSheets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSheets.Size = new System.Drawing.Size(297, 120);
            this.lbSheets.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(5, 5);
            this.label5.Margin = new System.Windows.Forms.Padding(5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Include Sheets :";
            // 
            // gbSaveAttachment
            // 
            this.gbSaveAttachment.AutoSize = true;
            this.gbSaveAttachment.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbSaveAttachment.Controls.Add(this.pnlMain);
            this.gbSaveAttachment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSaveAttachment.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.gbSaveAttachment.ForeColor = System.Drawing.Color.Green;
            this.gbSaveAttachment.Location = new System.Drawing.Point(3, 3);
            this.gbSaveAttachment.Name = "gbSaveAttachment";
            this.gbSaveAttachment.Size = new System.Drawing.Size(424, 299);
            this.gbSaveAttachment.TabIndex = 0;
            this.gbSaveAttachment.Text = "Save Attachment Action";
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.RowCount = 2;
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlMain.Size = new System.Drawing.Size(424, 299);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(297, 37);
            this.lblTitle.TabIndex = 19;
            this.lblTitle.Text = "Save Attachment Action";
            // 
            // SaveAttachmentView
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(430, 491);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveAttachmentView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Shown += new System.EventHandler(this.SaveAttachmentView_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlAction.ResumeLayout(false);
            this.pnlAction.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pnlFileName.ResumeLayout(false);
            this.pnlFileName.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbSaveAttachment.ResumeLayout(false);
            this.gbSaveAttachment.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel pnlMain;
        private System.Windows.Forms.RadioButton rdoAddNew;
        private System.Windows.Forms.RadioButton rdoOverrite;
        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel gbSaveAttachment;
        private System.Windows.Forms.FlowLayoutPanel pnlAction;
        private System.Windows.Forms.TableLayoutPanel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbPDF;
        private System.Windows.Forms.RadioButton rbExcel;
        private System.Windows.Forms.ListBox lbSheets;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbCustomSheets;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel pnlFileName;
        private System.Windows.Forms.RadioButton rbRunTimeFileName;
        private System.Windows.Forms.RadioButton rbManualFileName;
        private System.Windows.Forms.RadioButton rbCellReferenceFileName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button getCellRefBtn;



    }
}