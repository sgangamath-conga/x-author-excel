namespace Apttus.XAuthor.AppDesigner
{
    partial class AddRowActionView
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpRecords = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.cboInputType = new System.Windows.Forms.ComboBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.cboAllObjects = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.grpAddRowAction = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboSaveMap = new System.Windows.Forms.ComboBox();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoTop = new System.Windows.Forms.RadioButton();
            this.rdoBottom = new System.Windows.Forms.RadioButton();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkDisableExcelEvents = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpRecords.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpAddRowAction.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(401, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 32);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(487, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 32);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.grpRecords, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpAddRowAction, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(541, 286);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // grpRecords
            // 
            this.grpRecords.AutoSize = true;
            this.grpRecords.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpRecords.Controls.Add(this.tableLayoutPanel3);
            this.grpRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpRecords.Location = new System.Drawing.Point(3, 240);
            this.grpRecords.Name = "grpRecords";
            this.grpRecords.Size = new System.Drawing.Size(570, 70);
            this.grpRecords.TabIndex = 1;
            this.grpRecords.TabStop = false;
            this.grpRecords.Text = "Number Of Records";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 23);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(564, 44);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 12, 5, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "Input Type :";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.cboInputType);
            this.flowLayoutPanel2.Controls.Add(this.lblValue);
            this.flowLayoutPanel2.Controls.Add(this.cboAllObjects);
            this.flowLayoutPanel2.Controls.Add(this.txtValue);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(98, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(463, 38);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // cboInputType
            // 
            this.cboInputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInputType.FormattingEnabled = true;
            this.cboInputType.Items.AddRange(new object[] {
            "Action Flow Step Input",
            "Cell Reference",
            "Number of Rows",
            "User Input"});
            this.cboInputType.Location = new System.Drawing.Point(5, 5);
            this.cboInputType.Margin = new System.Windows.Forms.Padding(5);
            this.cboInputType.Name = "cboInputType";
            this.cboInputType.Size = new System.Drawing.Size(132, 28);
            this.cboInputType.TabIndex = 0;
            this.cboInputType.SelectedIndexChanged += new System.EventHandler(this.cboInputType_SelectedIndexChanged);
            this.cboInputType.Validating += new System.ComponentModel.CancelEventHandler(this.cboInputType_Validating);
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(147, 8);
            this.lblValue.Margin = new System.Windows.Forms.Padding(5, 8, 5, 5);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(78, 20);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "labelValue";
            this.lblValue.Visible = false;
            // 
            // cboAllObjects
            // 
            this.cboAllObjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAllObjects.FormattingEnabled = true;
            this.cboAllObjects.Location = new System.Drawing.Point(235, 5);
            this.cboAllObjects.Margin = new System.Windows.Forms.Padding(5);
            this.cboAllObjects.Name = "cboAllObjects";
            this.cboAllObjects.Size = new System.Drawing.Size(132, 28);
            this.cboAllObjects.TabIndex = 4;
            this.cboAllObjects.Visible = false;
            this.cboAllObjects.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(377, 5);
            this.txtValue.Margin = new System.Windows.Forms.Padding(5);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(81, 27);
            this.txtValue.TabIndex = 2;
            this.txtValue.Visible = false;
            this.txtValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtValue_Validating);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel1.Location = new System.Drawing.Point(3, 316);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 38);
            this.panel1.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Green;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(5, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(203, 37);
            this.label6.TabIndex = 4;
            this.label6.Text = "Add Row Action";
            // 
            // grpAddRowAction
            // 
            this.grpAddRowAction.AutoSize = true;
            this.grpAddRowAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpAddRowAction.Controls.Add(this.tableLayoutPanel2);
            this.grpAddRowAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAddRowAction.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.grpAddRowAction.Location = new System.Drawing.Point(5, 37);
            this.grpAddRowAction.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.grpAddRowAction.Name = "grpAddRowAction";
            this.grpAddRowAction.Size = new System.Drawing.Size(566, 195);
            this.grpAddRowAction.TabIndex = 0;
            this.grpAddRowAction.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.cboSaveMap, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cboObject, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.chkDisableExcelEvents, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 23);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(560, 169);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name : ";
            // 
            // txtActionName
            // 
            this.txtActionName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(99, 3);
            this.txtActionName.MaxLength = 100;
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(356, 27);
            this.txtActionName.TabIndex = 4;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(5, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Save Map :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(5, 109);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 8, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Location :";
            // 
            // cboSaveMap
            // 
            this.cboSaveMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSaveMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboSaveMap.FormattingEnabled = true;
            this.cboSaveMap.Location = new System.Drawing.Point(99, 36);
            this.cboSaveMap.Name = "cboSaveMap";
            this.cboSaveMap.Size = new System.Drawing.Size(356, 28);
            this.cboSaveMap.TabIndex = 6;
            this.cboSaveMap.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cboSaveMap.SelectedIndexChanged += new System.EventHandler(this.cboSaveMap_SelectedIndexChanged);
            this.cboSaveMap.Validating += new System.ComponentModel.CancelEventHandler(this.cboSaveMap_Validating);
            // 
            // cboObject
            // 
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(99, 70);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(356, 28);
            this.cboObject.TabIndex = 8;
            this.cboObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboObject_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(5, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "List Object :";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.rdoTop);
            this.flowLayoutPanel1.Controls.Add(this.rdoBottom);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(99, 104);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(356, 30);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // rdoTop
            // 
            this.rdoTop.AutoSize = true;
            this.rdoTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoTop.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoTop.Location = new System.Drawing.Point(3, 3);
            this.rdoTop.Name = "rdoTop";
            this.rdoTop.Size = new System.Drawing.Size(54, 24);
            this.rdoTop.TabIndex = 11;
            this.rdoTop.TabStop = true;
            this.rdoTop.Text = "Top";
            this.rdoTop.UseVisualStyleBackColor = true;
            // 
            // rdoBottom
            // 
            this.rdoBottom.AutoSize = true;
            this.rdoBottom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoBottom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rdoBottom.Location = new System.Drawing.Point(63, 3);
            this.rdoBottom.Name = "rdoBottom";
            this.rdoBottom.Size = new System.Drawing.Size(79, 24);
            this.rdoBottom.TabIndex = 10;
            this.rdoBottom.TabStop = true;
            this.rdoBottom.Text = "Bottom";
            this.rdoBottom.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // chkDisableExcelEvents
            // 
            this.chkDisableExcelEvents.AutoSize = true;
            this.chkDisableExcelEvents.Checked = true;
            this.chkDisableExcelEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel2.SetColumnSpan(this.chkDisableExcelEvents, 2);
            this.chkDisableExcelEvents.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDisableExcelEvents.Location = new System.Drawing.Point(5, 142);
            this.chkDisableExcelEvents.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkDisableExcelEvents.Name = "chkDisableExcelEvents";
            this.chkDisableExcelEvents.Size = new System.Drawing.Size(289, 24);
            this.chkDisableExcelEvents.TabIndex = 11;
            this.chkDisableExcelEvents.Text = "Disable Excel Events during execution";
            this.chkDisableExcelEvents.UseVisualStyleBackColor = true;
            // 
            // AddRowActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(541, 286);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddRowActionView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AddRowActionView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpRecords.ResumeLayout(false);
            this.grpRecords.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grpAddRowAction.ResumeLayout(false);
            this.grpAddRowAction.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpAddRowAction;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboSaveMap;
        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grpRecords;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ComboBox cboInputType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdoTop;
        private System.Windows.Forms.RadioButton rdoBottom;
        private System.Windows.Forms.ComboBox cboAllObjects;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox chkDisableExcelEvents;
    }
}