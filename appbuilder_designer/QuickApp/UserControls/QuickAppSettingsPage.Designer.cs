namespace Apttus.XAuthor.AppDesigner
{
    partial class QuickAppSettingsPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblSettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkEditInExcel = new System.Windows.Forms.CheckBox();
            this.chkSaveSheets = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMenuGroupName = new System.Windows.Forms.TextBox();
            this.chkAddRow = new System.Windows.Forms.CheckBox();
            this.txtSaveMenuButton = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRetrieveMenuButton = new System.Windows.Forms.TextBox();
            this.chkDeleteRow = new System.Windows.Forms.CheckBox();
            this.groupBoxWorksheet = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtWorksheetTitle = new System.Windows.Forms.TextBox();
            this.chkViewGridLines = new System.Windows.Forms.CheckBox();
            this.txtMaxColumnWidth = new System.Windows.Forms.TextBox();
            this.lblMaxColWidth = new System.Windows.Forms.Label();
            this.chkAutoFilter = new System.Windows.Forms.CheckBox();
            this.tblSettingsPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxWorksheet.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblSettingsPanel
            // 
            this.tblSettingsPanel.AutoSize = true;
            this.tblSettingsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSettingsPanel.ColumnCount = 1;
            this.tblSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblSettingsPanel.Controls.Add(this.groupBox1, 0, 0);
            this.tblSettingsPanel.Controls.Add(this.groupBox2, 0, 2);
            this.tblSettingsPanel.Controls.Add(this.groupBoxWorksheet, 0, 1);
            this.tblSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSettingsPanel.Location = new System.Drawing.Point(0, 0);
            this.tblSettingsPanel.Name = "tblSettingsPanel";
            this.tblSettingsPanel.RowCount = 3;
            this.tblSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSettingsPanel.Size = new System.Drawing.Size(435, 354);
            this.tblSettingsPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.chkEditInExcel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkSaveSheets, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(423, 25);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // chkEditInExcel
            // 
            this.chkEditInExcel.AutoSize = true;
            this.chkEditInExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkEditInExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkEditInExcel.Location = new System.Drawing.Point(3, 3);
            this.chkEditInExcel.Name = "chkEditInExcel";
            this.chkEditInExcel.Size = new System.Drawing.Size(210, 19);
            this.chkEditInExcel.TabIndex = 2;
            this.chkEditInExcel.Text = "Enable App Launch from Salesforce";
            this.chkEditInExcel.UseVisualStyleBackColor = true;
            // 
            // chkSaveSheets
            // 
            this.chkSaveSheets.AutoSize = true;
            this.chkSaveSheets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkSaveSheets.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkSaveSheets.Location = new System.Drawing.Point(219, 3);
            this.chkSaveSheets.Name = "chkSaveSheets";
            this.chkSaveSheets.Size = new System.Drawing.Size(100, 19);
            this.chkSaveSheets.TabIndex = 3;
            this.chkSaveSheets.Text = "chkSaveSheets";
            this.chkSaveSheets.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.BackColor = System.Drawing.SystemColors.Window;
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupBox2.Location = new System.Drawing.Point(3, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(429, 159);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Menu Settings";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtMenuGroupName, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkAddRow, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.txtSaveMenuButton, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.txtRetrieveMenuButton, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkDeleteRow, 0, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(423, 137);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Group Label :";
            // 
            // txtMenuGroupName
            // 
            this.txtMenuGroupName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMenuGroupName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMenuGroupName.Location = new System.Drawing.Point(161, 3);
            this.txtMenuGroupName.MaxLength = 50;
            this.txtMenuGroupName.Name = "txtMenuGroupName";
            this.txtMenuGroupName.Size = new System.Drawing.Size(259, 23);
            this.txtMenuGroupName.TabIndex = 6;
            // 
            // chkAddRow
            // 
            this.chkAddRow.AutoSize = true;
            this.chkAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAddRow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAddRow.Location = new System.Drawing.Point(3, 90);
            this.chkAddRow.Name = "chkAddRow";
            this.chkAddRow.Size = new System.Drawing.Size(109, 19);
            this.chkAddRow.TabIndex = 0;
            this.chkAddRow.Text = "Enable Add Row";
            this.chkAddRow.UseVisualStyleBackColor = true;
            // 
            // txtSaveMenuButton
            // 
            this.txtSaveMenuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSaveMenuButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSaveMenuButton.Location = new System.Drawing.Point(161, 61);
            this.txtSaveMenuButton.MaxLength = 50;
            this.txtSaveMenuButton.Name = "txtSaveMenuButton";
            this.txtSaveMenuButton.Size = new System.Drawing.Size(259, 23);
            this.txtSaveMenuButton.TabIndex = 8;
            this.txtSaveMenuButton.Text = "Save";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Retrieve Data Button Label :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(3, 61);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Save Data Button Label :";
            // 
            // txtRetrieveMenuButton
            // 
            this.txtRetrieveMenuButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRetrieveMenuButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRetrieveMenuButton.Location = new System.Drawing.Point(161, 32);
            this.txtRetrieveMenuButton.MaxLength = 50;
            this.txtRetrieveMenuButton.Name = "txtRetrieveMenuButton";
            this.txtRetrieveMenuButton.Size = new System.Drawing.Size(259, 23);
            this.txtRetrieveMenuButton.TabIndex = 7;
            this.txtRetrieveMenuButton.Text = "Retrieve";
            // 
            // chkDeleteRow
            // 
            this.chkDeleteRow.AutoSize = true;
            this.chkDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkDeleteRow.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkDeleteRow.Location = new System.Drawing.Point(3, 115);
            this.chkDeleteRow.Name = "chkDeleteRow";
            this.chkDeleteRow.Size = new System.Drawing.Size(120, 19);
            this.chkDeleteRow.TabIndex = 9;
            this.chkDeleteRow.Text = "Enable Delete Row";
            this.chkDeleteRow.UseVisualStyleBackColor = true;
            // 
            // groupBoxWorksheet
            // 
            this.groupBoxWorksheet.AutoSize = true;
            this.groupBoxWorksheet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxWorksheet.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxWorksheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxWorksheet.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.groupBoxWorksheet.Location = new System.Drawing.Point(3, 56);
            this.groupBoxWorksheet.Name = "groupBoxWorksheet";
            this.groupBoxWorksheet.Size = new System.Drawing.Size(429, 130);
            this.groupBoxWorksheet.TabIndex = 2;
            this.groupBoxWorksheet.TabStop = false;
            this.groupBoxWorksheet.Text = "Worksheet Settings";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lblTitle, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtWorksheetTitle, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkViewGridLines, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtMaxColumnWidth, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblMaxColWidth, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkAutoFilter, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(423, 108);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTitle.Location = new System.Drawing.Point(3, 3);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 15);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Worksheet Title :";
            // 
            // txtWorksheetTitle
            // 
            this.txtWorksheetTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWorksheetTitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWorksheetTitle.Location = new System.Drawing.Point(125, 3);
            this.txtWorksheetTitle.MaxLength = 250;
            this.txtWorksheetTitle.Name = "txtWorksheetTitle";
            this.txtWorksheetTitle.Size = new System.Drawing.Size(295, 23);
            this.txtWorksheetTitle.TabIndex = 0;
            // 
            // chkViewGridLines
            // 
            this.chkViewGridLines.AutoSize = true;
            this.chkViewGridLines.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkViewGridLines.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkViewGridLines.Location = new System.Drawing.Point(3, 61);
            this.chkViewGridLines.Name = "chkViewGridLines";
            this.chkViewGridLines.Size = new System.Drawing.Size(97, 19);
            this.chkViewGridLines.TabIndex = 3;
            this.chkViewGridLines.Text = "View Gridlines";
            this.chkViewGridLines.UseVisualStyleBackColor = true;
            // 
            // txtMaxColumnWidth
            // 
            this.txtMaxColumnWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMaxColumnWidth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaxColumnWidth.Location = new System.Drawing.Point(125, 32);
            this.txtMaxColumnWidth.MaxLength = 250;
            this.txtMaxColumnWidth.Name = "txtMaxColumnWidth";
            this.txtMaxColumnWidth.Size = new System.Drawing.Size(295, 23);
            this.txtMaxColumnWidth.TabIndex = 5;
            this.txtMaxColumnWidth.Text = "50";
            // 
            // lblMaxColWidth
            // 
            this.lblMaxColWidth.AutoSize = true;
            this.lblMaxColWidth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaxColWidth.Location = new System.Drawing.Point(3, 32);
            this.lblMaxColWidth.Margin = new System.Windows.Forms.Padding(3);
            this.lblMaxColWidth.Name = "lblMaxColWidth";
            this.lblMaxColWidth.Size = new System.Drawing.Size(116, 15);
            this.lblMaxColWidth.TabIndex = 4;
            this.lblMaxColWidth.Text = "Max Column Width :";
            // 
            // chkAutoFilter
            // 
            this.chkAutoFilter.AutoSize = true;
            this.chkAutoFilter.Checked = true;
            this.chkAutoFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkAutoFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoFilter.Location = new System.Drawing.Point(3, 86);
            this.chkAutoFilter.Name = "chkAutoFilter";
            this.chkAutoFilter.Size = new System.Drawing.Size(95, 19);
            this.chkAutoFilter.TabIndex = 6;
            this.chkAutoFilter.Text = "Display Filters";
            this.chkAutoFilter.UseVisualStyleBackColor = true;
            // 
            // QuickAppSettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tblSettingsPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "QuickAppSettingsPage";
            this.Size = new System.Drawing.Size(435, 354);
            this.Load += new System.EventHandler(this.SpreadSheetFormattingPage_Load);
            this.tblSettingsPanel.ResumeLayout(false);
            this.tblSettingsPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBoxWorksheet.ResumeLayout(false);
            this.groupBoxWorksheet.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblSettingsPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtWorksheetTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkEditInExcel;
        private System.Windows.Forms.CheckBox chkAddRow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSaveMenuButton;
        private System.Windows.Forms.TextBox txtRetrieveMenuButton;
        private System.Windows.Forms.TextBox txtMenuGroupName;
        private System.Windows.Forms.CheckBox chkViewGridLines;
        private System.Windows.Forms.CheckBox chkDeleteRow;
        private System.Windows.Forms.TextBox txtMaxColumnWidth;
        private System.Windows.Forms.Label lblMaxColWidth;
        private System.Windows.Forms.CheckBox chkAutoFilter;
        private System.Windows.Forms.GroupBox groupBoxWorksheet;
        private System.Windows.Forms.CheckBox chkSaveSheets;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
