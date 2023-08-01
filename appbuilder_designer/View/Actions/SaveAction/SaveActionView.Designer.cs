namespace Apttus.XAuthor.AppDesigner
{
    partial class SaveActionView
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
            this.gbSaveAction = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblActionName = new System.Windows.Forms.Label();
            this.chkEnableRowHighlight = new System.Windows.Forms.CheckBox();
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.lblSaveMap = new System.Windows.Forms.Label();
            this.cmbSaveMap = new System.Windows.Forms.ComboBox();
            this.chkEnablePartialSave = new System.Windows.Forms.CheckBox();
            this.chkEnableCollisionDetection = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtBatchSize = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.gbSaveAction.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.btnSave.Location = new System.Drawing.Point(86, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(74, 32);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(166, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 32);
            this.btnCancel.TabIndex = 6;
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
            this.tableLayoutPanel1.Controls.Add(this.gbSaveAction, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(354, 251);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 30);
            this.label1.TabIndex = 5;
            this.label1.Text = "Save Action";
            // 
            // gbSaveAction
            // 
            this.gbSaveAction.AutoSize = true;
            this.gbSaveAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbSaveAction.Controls.Add(this.tableLayoutPanel2);
            this.gbSaveAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSaveAction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSaveAction.Location = new System.Drawing.Point(2, 30);
            this.gbSaveAction.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.gbSaveAction.Name = "gbSaveAction";
            this.gbSaveAction.Padding = new System.Windows.Forms.Padding(2);
            this.gbSaveAction.Size = new System.Drawing.Size(350, 228);
            this.gbSaveAction.TabIndex = 3;
            this.gbSaveAction.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lblActionName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkEnableRowHighlight, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.txtActionName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSaveMap, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.cmbSaveMap, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkEnablePartialSave, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.chkEnableCollisionDetection, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.lblBatchSize, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.lblMessage, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.txtBatchSize, 1, 5);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 18);
            this.tableLayoutPanel2.MinimumSize = new System.Drawing.Size(340, 170);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(346, 208);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // lblActionName
            // 
            this.lblActionName.AutoSize = true;
            this.lblActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActionName.Location = new System.Drawing.Point(5, 5);
            this.lblActionName.Margin = new System.Windows.Forms.Padding(5);
            this.lblActionName.Name = "lblActionName";
            this.lblActionName.Size = new System.Drawing.Size(83, 15);
            this.lblActionName.TabIndex = 0;
            this.lblActionName.Text = "Action Name :";
            // 
            // chkEnableRowHighlight
            // 
            this.chkEnableRowHighlight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnableRowHighlight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableRowHighlight.Location = new System.Drawing.Point(96, 96);
            this.chkEnableRowHighlight.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkEnableRowHighlight.Name = "chkEnableRowHighlight";
            this.chkEnableRowHighlight.Size = new System.Drawing.Size(221, 19);
            this.chkEnableRowHighlight.TabIndex = 7;
            this.chkEnableRowHighlight.Text = "HighLight Unsaved Records";
            this.chkEnableRowHighlight.UseVisualStyleBackColor = true;
            // 
            // txtActionName
            // 
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(96, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(221, 23);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // lblSaveMap
            // 
            this.lblSaveMap.AutoSize = true;
            this.lblSaveMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSaveMap.Location = new System.Drawing.Point(5, 34);
            this.lblSaveMap.Margin = new System.Windows.Forms.Padding(5);
            this.lblSaveMap.Name = "lblSaveMap";
            this.lblSaveMap.Size = new System.Drawing.Size(64, 15);
            this.lblSaveMap.TabIndex = 1;
            this.lblSaveMap.Text = "Save Map :";
            // 
            // cmbSaveMap
            // 
            this.cmbSaveMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSaveMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbSaveMap.FormattingEnabled = true;
            this.cmbSaveMap.Location = new System.Drawing.Point(96, 32);
            this.cmbSaveMap.Name = "cmbSaveMap";
            this.cmbSaveMap.Size = new System.Drawing.Size(221, 23);
            this.cmbSaveMap.TabIndex = 1;
            this.cmbSaveMap.Validating += new System.ComponentModel.CancelEventHandler(this.cmbSaveMap_Validating);
            // 
            // chkEnablePartialSave
            // 
            this.chkEnablePartialSave.Checked = true;
            this.chkEnablePartialSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnablePartialSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnablePartialSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnablePartialSave.Location = new System.Drawing.Point(96, 77);
            this.chkEnablePartialSave.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkEnablePartialSave.Name = "chkEnablePartialSave";
            this.chkEnablePartialSave.Size = new System.Drawing.Size(164, 19);
            this.chkEnablePartialSave.TabIndex = 4;
            this.chkEnablePartialSave.Text = "Enable Partial Save";
            this.chkEnablePartialSave.UseVisualStyleBackColor = true;
            // 
            // chkEnableCollisionDetection
            // 
            this.chkEnableCollisionDetection.AutoSize = true;
            this.chkEnableCollisionDetection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEnableCollisionDetection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnableCollisionDetection.Location = new System.Drawing.Point(96, 58);
            this.chkEnableCollisionDetection.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkEnableCollisionDetection.Name = "chkEnableCollisionDetection";
            this.chkEnableCollisionDetection.Size = new System.Drawing.Size(161, 19);
            this.chkEnableCollisionDetection.TabIndex = 3;
            this.chkEnableCollisionDetection.Text = "Enable Collision Detection";
            this.chkEnableCollisionDetection.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(96, 167);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(247, 38);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBatchSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBatchSize.Location = new System.Drawing.Point(3, 115);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(87, 29);
            this.lblBatchSize.TabIndex = 9;
            this.lblBatchSize.Text = "Batch Size :";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(96, 144);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(247, 20);
            this.lblMessage.TabIndex = 11;
            this.lblMessage.Text = "Range [1 - 200]";
            // 
            // txtBatchSize
            // 
            this.txtBatchSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBatchSize.Location = new System.Drawing.Point(96, 118);
            this.txtBatchSize.Name = "txtBatchSize";
            this.txtBatchSize.Size = new System.Drawing.Size(79, 23);
            this.txtBatchSize.TabIndex = 0;
            this.txtBatchSize.Validating += new System.ComponentModel.CancelEventHandler(this.txtBatchSize_Validating);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SaveActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 251);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveActionView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.SaveActionView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbSaveAction.ResumeLayout(false);
            this.gbSaveAction.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbSaveAction;
        private System.Windows.Forms.CheckBox chkEnablePartialSave;
        private System.Windows.Forms.CheckBox chkEnableCollisionDetection;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.Label lblActionName;
        private System.Windows.Forms.ComboBox cmbSaveMap;
        private System.Windows.Forms.Label lblSaveMap;
        private System.Windows.Forms.CheckBox chkEnableRowHighlight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtBatchSize;
    }
}