namespace LookAhead
{
    partial class LookupSupport
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.pnlSearchFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblSearchFields = new System.Windows.Forms.Label();
            this.txtSearchFields = new System.Windows.Forms.TextBox();
            this.lnkClear = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tbLayoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.pnlSearchFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(326, 23);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 32);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(326, 244);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(346, 313);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(338, 285);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Type ahead";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(332, 279);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbLayoutMain);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(338, 285);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Type ahead";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbLayoutMain
            // 
            this.tbLayoutMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tbLayoutMain.ColumnCount = 1;
            this.tbLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbLayoutMain.Controls.Add(this.dgvFields, 0, 1);
            this.tbLayoutMain.Controls.Add(this.pnlSearchFields, 0, 0);
            this.tbLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLayoutMain.Location = new System.Drawing.Point(3, 3);
            this.tbLayoutMain.Name = "tbLayoutMain";
            this.tbLayoutMain.RowCount = 2;
            this.tbLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tbLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tbLayoutMain.Size = new System.Drawing.Size(332, 279);
            this.tbLayoutMain.TabIndex = 0;
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToAddRows = false;
            this.dgvFields.AllowUserToDeleteRows = false;
            this.dgvFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFields.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvFields.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFields.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvFields.Location = new System.Drawing.Point(4, 36);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.RowHeadersVisible = false;
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFields.Size = new System.Drawing.Size(327, 239);
            this.dgvFields.TabIndex = 11;
            this.dgvFields.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellDoubleClick);
            // 
            // pnlSearchFields
            // 
            this.pnlSearchFields.AutoSize = true;
            this.pnlSearchFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearchFields.ColumnCount = 4;
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.Controls.Add(this.lblSearchFields);
            this.pnlSearchFields.Controls.Add(this.txtSearchFields);
            this.pnlSearchFields.Controls.Add(this.lnkClear, 3, 0);
            this.pnlSearchFields.Controls.Add(this.linkLabel1, 2, 0);
            this.pnlSearchFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchFields.Location = new System.Drawing.Point(2, 2);
            this.pnlSearchFields.Margin = new System.Windows.Forms.Padding(1);
            this.pnlSearchFields.Name = "pnlSearchFields";
            this.pnlSearchFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSearchFields.Size = new System.Drawing.Size(331, 29);
            this.pnlSearchFields.TabIndex = 10;
            // 
            // lblSearchFields
            // 
            this.lblSearchFields.AutoSize = true;
            this.lblSearchFields.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblSearchFields.Location = new System.Drawing.Point(5, 5);
            this.lblSearchFields.Margin = new System.Windows.Forms.Padding(5);
            this.lblSearchFields.Name = "lblSearchFields";
            this.lblSearchFields.Size = new System.Drawing.Size(82, 15);
            this.lblSearchFields.TabIndex = 12;
            this.lblSearchFields.Text = "Search Fields :";
            // 
            // txtSearchFields
            // 
            this.txtSearchFields.Location = new System.Drawing.Point(95, 3);
            this.txtSearchFields.Name = "txtSearchFields";
            this.txtSearchFields.Size = new System.Drawing.Size(137, 23);
            this.txtSearchFields.TabIndex = 9;
            this.txtSearchFields.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearchFields_KeyPress);
            // 
            // lnkClear
            // 
            this.lnkClear.AutoSize = true;
            this.lnkClear.Location = new System.Drawing.Point(292, 5);
            this.lnkClear.Margin = new System.Windows.Forms.Padding(5);
            this.lnkClear.Name = "lnkClear";
            this.lnkClear.Size = new System.Drawing.Size(34, 15);
            this.lnkClear.TabIndex = 11;
            this.lnkClear.TabStop = true;
            this.lnkClear.Text = "Clear";
            this.lnkClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClear_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(240, 5);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(5);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(42, 15);
            this.linkLabel1.TabIndex = 13;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Search";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // LookupSupport
            // 
            this.AcceptButton = this.linkLabel1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(346, 313);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LookupSupport";
            this.ShowIcon = false;
            this.Text = "Type ahead";
            this.Load += new System.EventHandler(this.LookupSupport_Load);
            this.Shown += new System.EventHandler(this.LookupSupport_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LookupSupport_KeyPress);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tbLayoutMain.ResumeLayout(false);
            this.tbLayoutMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.pnlSearchFields.ResumeLayout(false);
            this.pnlSearchFields.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tbLayoutMain;
        private System.Windows.Forms.TableLayoutPanel pnlSearchFields;
        private System.Windows.Forms.Label lblSearchFields;
        private System.Windows.Forms.TextBox txtSearchFields;
        private System.Windows.Forms.LinkLabel lnkClear;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}