namespace Apttus.XAuthor.AppDesigner
{
    partial class QueryActionView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.expressionBuilder = new Apttus.XAuthor.AppDesigner.ExpressionBuilderView();
            this.groupActionDetail = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAllowSavingFilters = new System.Windows.Forms.CheckBox();
            this.txtMaxRecords = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblObject = new System.Windows.Forms.Label();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupActionDetail.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(586, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 32);
            this.btnSave.TabIndex = 4;
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
            this.btnCancel.Location = new System.Drawing.Point(679, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 32);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.expressionBuilder);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(763, 315);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // expressionBuilder
            // 
            this.expressionBuilder.AutoSize = true;
            this.expressionBuilder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.expressionBuilder.BackColor = System.Drawing.SystemColors.Window;
            this.expressionBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expressionBuilder.IsExpressionBuilderLaunchedFromQuickApp = false;
            this.expressionBuilder.Location = new System.Drawing.Point(3, 19);
            this.expressionBuilder.Margin = new System.Windows.Forms.Padding(4);
            this.expressionBuilder.Model = null;
            this.expressionBuilder.Name = "expressionBuilder";
            this.expressionBuilder.Size = new System.Drawing.Size(757, 293);
            this.expressionBuilder.TabIndex = 18;
            this.expressionBuilder.TargetObject = null;
            this.expressionBuilder.Load += new System.EventHandler(this.expressionBuilder_Load);
            // 
            // groupActionDetail
            // 
            this.groupActionDetail.AutoSize = true;
            this.groupActionDetail.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupActionDetail.Controls.Add(this.tableLayoutPanel2);
            this.groupActionDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupActionDetail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupActionDetail.Location = new System.Drawing.Point(3, 33);
            this.groupActionDetail.Name = "groupActionDetail";
            this.groupActionDetail.Size = new System.Drawing.Size(763, 80);
            this.groupActionDetail.TabIndex = 19;
            this.groupActionDetail.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.chkAllowSavingFilters, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtMaxRecords, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblObject, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.cboObject, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(757, 58);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // chkAllowSavingFilters
            // 
            this.chkAllowSavingFilters.AutoSize = true;
            this.chkAllowSavingFilters.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAllowSavingFilters.Location = new System.Drawing.Point(422, 3);
            this.chkAllowSavingFilters.Name = "chkAllowSavingFilters";
            this.chkAllowSavingFilters.Size = new System.Drawing.Size(140, 19);
            this.chkAllowSavingFilters.TabIndex = 14;
            this.chkAllowSavingFilters.Text = "Enable Favorite Filters";
            this.chkAllowSavingFilters.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.chkAllowSavingFilters.UseVisualStyleBackColor = false;
            // 
            // txtMaxRecords
            // 
            this.txtMaxRecords.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxRecords.Location = new System.Drawing.Point(422, 32);
            this.txtMaxRecords.Name = "txtMaxRecords";
            this.txtMaxRecords.Size = new System.Drawing.Size(69, 23);
            this.txtMaxRecords.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(5, 5);
            this.lblName.Margin = new System.Windows.Forms.Padding(5);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(83, 15);
            this.lblName.TabIndex = 17;
            this.lblName.Text = "Action Name :";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(337, 34);
            this.label5.Margin = new System.Windows.Forms.Padding(5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Max records :";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(96, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(233, 23);
            this.txtName.TabIndex = 0;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // lblObject
            // 
            this.lblObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblObject.AutoSize = true;
            this.lblObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObject.Location = new System.Drawing.Point(40, 34);
            this.lblObject.Margin = new System.Windows.Forms.Padding(5);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(48, 15);
            this.lblObject.TabIndex = 1;
            this.lblObject.Text = "Object :";
            // 
            // cboObject
            // 
            this.cboObject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(96, 32);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(233, 23);
            this.cboObject.TabIndex = 1;
            this.cboObject.SelectedIndexChanged += new System.EventHandler(this.cboObject_SelectedIndexChanged);
            this.cboObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboObject_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 30);
            this.label1.TabIndex = 18;
            this.label1.Text = "Query Action";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupActionDetail, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(769, 500);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.CausesValidation = false;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 437);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(769, 63);
            this.flowLayoutPanel1.TabIndex = 21;
            // 
            // QueryActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(769, 500);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QueryActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.QueryActionView_Load);
            this.Shown += new System.EventHandler(this.QueryActionView_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupActionDetail.ResumeLayout(false);
            this.groupActionDetail.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.Label lblObject;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxRecords;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private ExpressionBuilderView expressionBuilder;
        private System.Windows.Forms.GroupBox groupActionDetail;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkAllowSavingFilters;
    }
}