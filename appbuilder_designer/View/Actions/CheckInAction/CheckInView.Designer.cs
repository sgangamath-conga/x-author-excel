namespace Apttus.XAuthor.AppDesigner
{
    partial class CheckInView
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbCheckIn = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.controlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.getCellRefBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.cellReferenceBtn = new System.Windows.Forms.RadioButton();
            this.runtimeFileNameBtn = new System.Windows.Forms.RadioButton();
            this.manualNameBtn = new System.Windows.Forms.RadioButton();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.gbCheckIn.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(222, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 32);
            this.btnOK.TabIndex = 3;
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
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(332, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 32);
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
            this.tableLayoutPanel1.Controls.Add(this.gbCheckIn, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(457, 295);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // gbCheckIn
            // 
            this.gbCheckIn.AutoSize = true;
            this.gbCheckIn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbCheckIn.Controls.Add(this.panel1);
            this.gbCheckIn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbCheckIn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbCheckIn.Location = new System.Drawing.Point(3, 37);
            this.gbCheckIn.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.gbCheckIn.Name = "gbCheckIn";
            this.gbCheckIn.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.gbCheckIn.Size = new System.Drawing.Size(451, 258);
            this.gbCheckIn.TabIndex = 0;
            this.gbCheckIn.TabStop = false;
            this.gbCheckIn.Enter += new System.EventHandler(this.gbCheckIn_Enter);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(3, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 238);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.controlPanel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(445, 238);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 193);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(439, 42);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // controlPanel
            // 
            this.controlPanel.AutoSize = true;
            this.controlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlPanel.ColumnCount = 3;
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.Controls.Add(this.label1, 0, 0);
            this.controlPanel.Controls.Add(this.getCellRefBtn, 2, 5);
            this.controlPanel.Controls.Add(this.label3, 0, 1);
            this.controlPanel.Controls.Add(this.txtFileName, 1, 5);
            this.controlPanel.Controls.Add(this.cellReferenceBtn, 1, 4);
            this.controlPanel.Controls.Add(this.runtimeFileNameBtn, 1, 2);
            this.controlPanel.Controls.Add(this.manualNameBtn, 1, 3);
            this.controlPanel.Controls.Add(this.txtActionName, 1, 0);
            this.controlPanel.Controls.Add(this.cboObject, 1, 1);
            this.controlPanel.Controls.Add(this.label2, 0, 2);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Location = new System.Drawing.Point(3, 3);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.RowCount = 6;
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlPanel.Size = new System.Drawing.Size(439, 184);
            this.controlPanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action Name :";
            // 
            // getCellRefBtn
            // 
            this.getCellRefBtn.AutoSize = true;
            this.getCellRefBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.getCellRefBtn.Enabled = false;
            this.getCellRefBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.getCellRefBtn.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.cellSelectionIcon;
            this.getCellRefBtn.Location = new System.Drawing.Point(401, 154);
            this.getCellRefBtn.Name = "getCellRefBtn";
            this.getCellRefBtn.Size = new System.Drawing.Size(29, 26);
            this.getCellRefBtn.TabIndex = 9;
            this.getCellRefBtn.UseVisualStyleBackColor = true;
            this.getCellRefBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 38);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Object :";
            // 
            // txtFileName
            // 
            this.txtFileName.Enabled = false;
            this.txtFileName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFileName.Location = new System.Drawing.Point(116, 154);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(279, 27);
            this.txtFileName.TabIndex = 1;
            this.txtFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtFileName_Validating);
            // 
            // cellReferenceBtn
            // 
            this.cellReferenceBtn.AutoSize = true;
            this.cellReferenceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cellReferenceBtn.Location = new System.Drawing.Point(116, 126);
            this.cellReferenceBtn.Name = "cellReferenceBtn";
            this.cellReferenceBtn.Size = new System.Drawing.Size(247, 22);
            this.cellReferenceBtn.TabIndex = 6;
            this.cellReferenceBtn.Tag = "CellRef";
            this.cellReferenceBtn.Text = "Use Cell Reference as File Name";
            this.cellReferenceBtn.UseVisualStyleBackColor = true;
            this.cellReferenceBtn.CheckedChanged += new System.EventHandler(this.cellReferenceBtn_CheckedChanged);
            // 
            // runtimeFileNameBtn
            // 
            this.runtimeFileNameBtn.AutoSize = true;
            this.runtimeFileNameBtn.Checked = true;
            this.runtimeFileNameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runtimeFileNameBtn.Location = new System.Drawing.Point(116, 70);
            this.runtimeFileNameBtn.Name = "runtimeFileNameBtn";
            this.runtimeFileNameBtn.Size = new System.Drawing.Size(178, 22);
            this.runtimeFileNameBtn.TabIndex = 8;
            this.runtimeFileNameBtn.TabStop = true;
            this.runtimeFileNameBtn.Tag = "Runtime";
            this.runtimeFileNameBtn.Text = "Use Runtime Filename";
            this.runtimeFileNameBtn.UseVisualStyleBackColor = true;
            this.runtimeFileNameBtn.CheckedChanged += new System.EventHandler(this.runtimeFileNameBtn_CheckedChanged);
            // 
            // manualNameBtn
            // 
            this.manualNameBtn.AutoSize = true;
            this.manualNameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.manualNameBtn.Location = new System.Drawing.Point(116, 98);
            this.manualNameBtn.Name = "manualNameBtn";
            this.manualNameBtn.Size = new System.Drawing.Size(211, 22);
            this.manualNameBtn.TabIndex = 5;
            this.manualNameBtn.Tag = "Manual";
            this.manualNameBtn.Text = "Manually Provide File Name";
            this.manualNameBtn.UseVisualStyleBackColor = true;
            this.manualNameBtn.CheckedChanged += new System.EventHandler(this.manualNameBtn_CheckedChanged);
            // 
            // txtActionName
            // 
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActionName.Location = new System.Drawing.Point(116, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(279, 27);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.TextChanged += new System.EventHandler(this.txtActionName_TextChanged);
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // cboObject
            // 
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(116, 36);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(279, 28);
            this.cboObject.TabIndex = 2;
            this.cboObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboObject_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 72);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "File Name :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(5, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(339, 37);
            this.label4.TabIndex = 5;
            this.label4.Text = "Check In File as Attachment";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // CheckInView
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(457, 295);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckInView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Shown += new System.EventHandler(this.CheckInView_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbCheckIn.ResumeLayout(false);
            this.gbCheckIn.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbCheckIn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton runtimeFileNameBtn;
        private System.Windows.Forms.RadioButton cellReferenceBtn;
        private System.Windows.Forms.RadioButton manualNameBtn;
        private System.Windows.Forms.Button getCellRefBtn;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel controlPanel;

    }
}