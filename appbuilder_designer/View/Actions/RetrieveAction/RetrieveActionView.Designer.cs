namespace Apttus.XAuthor.AppDesigner
{
    partial class RetrieveActionView
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
            this.txtActionName = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cmbRetrieveMap = new System.Windows.Forms.ComboBox();
            this.lblRetrieveMap = new System.Windows.Forms.Label();
            this.lblActionName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.displayActionTablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.chkShowQueryFilters = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlObjectFilterview = new System.Windows.Forms.Panel();
            this.chkDisableExcelEvents = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mainDisplayActionTablePanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.displayActionTablePanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.mainDisplayActionTablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSave.Location = new System.Drawing.Point(115, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(104, 32);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtActionName
            // 
            this.txtActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtActionName.Location = new System.Drawing.Point(112, 3);
            this.txtActionName.Name = "txtActionName";
            this.txtActionName.Size = new System.Drawing.Size(222, 27);
            this.txtActionName.TabIndex = 0;
            this.txtActionName.Validating += new System.ComponentModel.CancelEventHandler(this.txtActionName_Validating);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // cmbRetrieveMap
            // 
            this.cmbRetrieveMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRetrieveMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbRetrieveMap.FormattingEnabled = true;
            this.cmbRetrieveMap.Location = new System.Drawing.Point(112, 36);
            this.cmbRetrieveMap.Name = "cmbRetrieveMap";
            this.cmbRetrieveMap.Size = new System.Drawing.Size(222, 28);
            this.cmbRetrieveMap.TabIndex = 1;
            this.cmbRetrieveMap.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cmbRetrieveMap.SelectedIndexChanged += new System.EventHandler(this.cmbRetrieveMap_SelectedIndexChanged);
            this.cmbRetrieveMap.Validating += new System.ComponentModel.CancelEventHandler(this.cmbRetrieveMap_Validating);
            // 
            // lblRetrieveMap
            // 
            this.lblRetrieveMap.AutoSize = true;
            this.lblRetrieveMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRetrieveMap.Location = new System.Drawing.Point(3, 38);
            this.lblRetrieveMap.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblRetrieveMap.Name = "lblRetrieveMap";
            this.lblRetrieveMap.Size = new System.Drawing.Size(99, 20);
            this.lblRetrieveMap.TabIndex = 1;
            this.lblRetrieveMap.Text = "Display Map :";
            // 
            // lblActionName
            // 
            this.lblActionName.AutoSize = true;
            this.lblActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActionName.Location = new System.Drawing.Point(3, 5);
            this.lblActionName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblActionName.Name = "lblActionName";
            this.lblActionName.Size = new System.Drawing.Size(103, 20);
            this.lblActionName.TabIndex = 0;
            this.lblActionName.Text = "Action Name :";
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
            this.btnCancel.Location = new System.Drawing.Point(225, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(104, 32);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.displayActionTablePanel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 37);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.groupBox1.Size = new System.Drawing.Size(453, 201);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // displayActionTablePanel
            // 
            this.displayActionTablePanel.AutoSize = true;
            this.displayActionTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.displayActionTablePanel.ColumnCount = 3;
            this.displayActionTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.displayActionTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.displayActionTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.displayActionTablePanel.Controls.Add(this.lblActionName, 0, 0);
            this.displayActionTablePanel.Controls.Add(this.chkShowQueryFilters, 1, 3);
            this.displayActionTablePanel.Controls.Add(this.txtActionName, 1, 0);
            this.displayActionTablePanel.Controls.Add(this.lblRetrieveMap, 0, 1);
            this.displayActionTablePanel.Controls.Add(this.cmbRetrieveMap, 1, 1);
            this.displayActionTablePanel.Controls.Add(this.flowLayoutPanel1, 1, 5);
            this.displayActionTablePanel.Controls.Add(this.pnlObjectFilterview, 1, 4);
            this.displayActionTablePanel.Controls.Add(this.chkDisableExcelEvents, 1, 2);
            this.displayActionTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayActionTablePanel.Location = new System.Drawing.Point(3, 20);
            this.displayActionTablePanel.Margin = new System.Windows.Forms.Padding(1);
            this.displayActionTablePanel.Name = "displayActionTablePanel";
            this.displayActionTablePanel.RowCount = 6;
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.displayActionTablePanel.Size = new System.Drawing.Size(447, 181);
            this.displayActionTablePanel.TabIndex = 5;
            // 
            // chkShowQueryFilters
            // 
            this.chkShowQueryFilters.AutoSize = true;
            this.chkShowQueryFilters.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowQueryFilters.Location = new System.Drawing.Point(114, 104);
            this.chkShowQueryFilters.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkShowQueryFilters.Name = "chkShowQueryFilters";
            this.chkShowQueryFilters.Size = new System.Drawing.Size(153, 24);
            this.chkShowQueryFilters.TabIndex = 4;
            this.chkShowQueryFilters.Text = "Show Query Filters";
            this.chkShowQueryFilters.UseVisualStyleBackColor = true;
            this.chkShowQueryFilters.CheckedChanged += new System.EventHandler(this.chkShowQueryFilters_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.displayActionTablePanel.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSave);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(112, 140);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(332, 38);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // pnlObjectFilterview
            // 
            this.pnlObjectFilterview.AutoSize = true;
            this.pnlObjectFilterview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlObjectFilterview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlObjectFilterview.Location = new System.Drawing.Point(112, 134);
            this.pnlObjectFilterview.Name = "pnlObjectFilterview";
            this.pnlObjectFilterview.Size = new System.Drawing.Size(282, 1);
            this.pnlObjectFilterview.TabIndex = 6;
            // 
            // chkDisableExcelEvents
            // 
            this.chkDisableExcelEvents.AutoSize = true;
            this.chkDisableExcelEvents.Checked = true;
            this.chkDisableExcelEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDisableExcelEvents.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDisableExcelEvents.Location = new System.Drawing.Point(114, 72);
            this.chkDisableExcelEvents.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkDisableExcelEvents.Name = "chkDisableExcelEvents";
            this.chkDisableExcelEvents.Size = new System.Drawing.Size(280, 24);
            this.chkDisableExcelEvents.TabIndex = 7;
            this.chkDisableExcelEvents.Text = "Disable Excel Events during execution";
            this.chkDisableExcelEvents.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Display Action";
            // 
            // mainDisplayActionTablePanel
            // 
            this.mainDisplayActionTablePanel.AutoSize = true;
            this.mainDisplayActionTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainDisplayActionTablePanel.ColumnCount = 1;
            this.mainDisplayActionTablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainDisplayActionTablePanel.Controls.Add(this.groupBox1, 0, 1);
            this.mainDisplayActionTablePanel.Controls.Add(this.label1, 0, 0);
            this.mainDisplayActionTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainDisplayActionTablePanel.Location = new System.Drawing.Point(0, 0);
            this.mainDisplayActionTablePanel.Name = "mainDisplayActionTablePanel";
            this.mainDisplayActionTablePanel.RowCount = 2;
            this.mainDisplayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainDisplayActionTablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainDisplayActionTablePanel.Size = new System.Drawing.Size(379, 182);
            this.mainDisplayActionTablePanel.TabIndex = 4;
            // 
            // RetrieveActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(379, 182);
            this.Controls.Add(this.mainDisplayActionTablePanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RetrieveActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.RetrieveActionView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.displayActionTablePanel.ResumeLayout(false);
            this.displayActionTablePanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.mainDisplayActionTablePanel.ResumeLayout(false);
            this.mainDisplayActionTablePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtActionName;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ComboBox cmbRetrieveMap;
        private System.Windows.Forms.Label lblRetrieveMap;
        private System.Windows.Forms.Label lblActionName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel mainDisplayActionTablePanel;
        private System.Windows.Forms.CheckBox chkShowQueryFilters;
        private System.Windows.Forms.TableLayoutPanel displayActionTablePanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel pnlObjectFilterview;
        private System.Windows.Forms.CheckBox chkDisableExcelEvents;
    }
}