namespace Apttus.XAuthor.AppDesigner
{
    partial class MacroActionView
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblActionName = new System.Windows.Forms.Label();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.lblMacroName = new System.Windows.Forms.Label();
            this.cmbMacro = new System.Windows.Forms.ComboBox();
            this.chkDisableExcelEvents = new System.Windows.Forms.CheckBox();
            this.chkMacroOutput = new System.Windows.Forms.CheckBox();
            this.lblTip = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSave.Location = new System.Drawing.Point(158, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 27);
            this.btnSave.TabIndex = 3;
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
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(268, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 27);
            this.btnCancel.TabIndex = 4;
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
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(397, 210);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Macro Action";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 178);
            this.groupBox1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkDisableExcelEvents, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.chkMacroOutput, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblTip, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(391, 178);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.lblActionName, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblMacroName, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.cmbMacro, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(385, 58);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // lblActionName
            // 
            this.lblActionName.AutoSize = true;
            this.lblActionName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActionName.Location = new System.Drawing.Point(5, 5);
            this.lblActionName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lblActionName.Name = "lblActionName";
            this.lblActionName.Size = new System.Drawing.Size(83, 15);
            this.lblActionName.TabIndex = 0;
            this.lblActionName.Text = "Action Name :";
            // 
            // txtActionName
            // 
            this.txtActionName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(96, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(245, 23);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // lblMacroName
            // 
            this.lblMacroName.AutoSize = true;
            this.lblMacroName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblMacroName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMacroName.Location = new System.Drawing.Point(5, 34);
            this.lblMacroName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lblMacroName.Name = "lblMacroName";
            this.lblMacroName.Size = new System.Drawing.Size(82, 15);
            this.lblMacroName.TabIndex = 1;
            this.lblMacroName.Text = "Macro Name :";
            // 
            // cmbMacro
            // 
            this.cmbMacro.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbMacro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMacro.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbMacro.FormattingEnabled = true;
            this.cmbMacro.Location = new System.Drawing.Point(96, 32);
            this.cmbMacro.Name = "cmbMacro";
            this.cmbMacro.Size = new System.Drawing.Size(245, 23);
            this.cmbMacro.TabIndex = 1;
            this.cmbMacro.Validating += new System.ComponentModel.CancelEventHandler(this.cmbMacro_Validating);
            // 
            // chkDisableExcelEvents
            // 
            this.chkDisableExcelEvents.AutoSize = true;
            this.chkDisableExcelEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkDisableExcelEvents.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDisableExcelEvents.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDisableExcelEvents.Location = new System.Drawing.Point(10, 108);
            this.chkDisableExcelEvents.Margin = new System.Windows.Forms.Padding(10, 2, 5, 2);
            this.chkDisableExcelEvents.Name = "chkDisableExcelEvents";
            this.chkDisableExcelEvents.Size = new System.Drawing.Size(376, 19);
            this.chkDisableExcelEvents.TabIndex = 6;
            this.chkDisableExcelEvents.Text = "Disable excel events before macro execution";
            this.chkDisableExcelEvents.UseVisualStyleBackColor = true;
            // 
            // chkMacroOutput
            // 
            this.chkMacroOutput.AutoSize = true;
            this.chkMacroOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkMacroOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMacroOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMacroOutput.Location = new System.Drawing.Point(10, 66);
            this.chkMacroOutput.Margin = new System.Windows.Forms.Padding(10, 2, 5, 2);
            this.chkMacroOutput.Name = "chkMacroOutput";
            this.chkMacroOutput.Size = new System.Drawing.Size(376, 19);
            this.chkMacroOutput.TabIndex = 2;
            this.chkMacroOutput.Text = "Terminate Action Flow conditionally";
            this.chkMacroOutput.UseVisualStyleBackColor = true;
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTip.Location = new System.Drawing.Point(7, 89);
            this.lblTip.Margin = new System.Windows.Forms.Padding(7, 2, 5, 2);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(379, 15);
            this.lblTip.TabIndex = 5;
            this.lblTip.Text = "[ If the macro returns true, action flow will be terminated. ]";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 132);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(385, 43);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // MacroActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(397, 210);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MacroActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cmbMacro;
        private System.Windows.Forms.Label lblMacroName;
        private System.Windows.Forms.Label lblActionName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkMacroOutput;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.CheckBox chkDisableExcelEvents;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}