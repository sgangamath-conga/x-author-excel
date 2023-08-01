namespace Apttus.XAuthor.AppRuntime
{
    partial class UserInputLookupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInputLookupForm));
            this.tpnlMain = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ObjectName = new System.Windows.Forms.DataGridViewLinkColumn();
            this.IDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tpnlLookUp = new System.Windows.Forms.TableLayoutPanel();
            this.lblLookUp = new System.Windows.Forms.Label();
            this.cmbLookUp = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.tpnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tpnlLookUp.SuspendLayout();
            this.SuspendLayout();
            // 
            // tpnlMain
            // 
            this.tpnlMain.AutoSize = true;
            this.tpnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tpnlMain.ColumnCount = 1;
            this.tpnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tpnlMain.Controls.Add(this.dataGridView1, 0, 3);
            this.tpnlMain.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tpnlMain.Controls.Add(this.lblTitle, 0, 0);
            this.tpnlMain.Controls.Add(this.tpnlLookUp, 0, 1);
            this.tpnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpnlMain.Location = new System.Drawing.Point(2, 2);
            this.tpnlMain.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMain.Name = "tpnlMain";
            this.tpnlMain.RowCount = 4;
            this.tpnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnlMain.Size = new System.Drawing.Size(373, 353);
            this.tpnlMain.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ObjectName,
            this.IDColumn});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 113);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(367, 238);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ObjectName
            // 
            this.ObjectName.DataPropertyName = "Name";
            this.ObjectName.HeaderText = "Name";
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.ReadOnly = true;
            this.ObjectName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ObjectName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // IDColumn
            // 
            this.IDColumn.DataPropertyName = "id";
            this.IDColumn.HeaderText = "ID";
            this.IDColumn.Name = "IDColumn";
            this.IDColumn.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.txtSearch, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSearch, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnGo, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 78);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(367, 29);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // btnGo
            // 
            this.btnGo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGo.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnGo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGo.Location = new System.Drawing.Point(331, 3);
            this.btnGo.Margin = new System.Windows.Forms.Padding(5, 3, 0, 0);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(34, 23);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(62, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(261, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Location = new System.Drawing.Point(5, 5);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(81, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Lookup";
            // 
            // tpnlLookUp
            // 
            this.tpnlLookUp.AutoSize = true;
            this.tpnlLookUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tpnlLookUp.ColumnCount = 2;
            this.tpnlLookUp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tpnlLookUp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tpnlLookUp.Controls.Add(this.lblLookUp, 0, 0);
            this.tpnlLookUp.Controls.Add(this.cmbLookUp, 1, 0);
            this.tpnlLookUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlLookUp.Location = new System.Drawing.Point(3, 43);
            this.tpnlLookUp.Name = "tpnlLookUp";
            this.tpnlLookUp.RowCount = 1;
            this.tpnlLookUp.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnlLookUp.Size = new System.Drawing.Size(367, 29);
            this.tpnlLookUp.TabIndex = 6;
            // 
            // lblLookUp
            // 
            this.lblLookUp.AutoSize = true;
            this.lblLookUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLookUp.Location = new System.Drawing.Point(3, 3);
            this.lblLookUp.Margin = new System.Windows.Forms.Padding(3);
            this.lblLookUp.Name = "lblLookUp";
            this.lblLookUp.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lblLookUp.Size = new System.Drawing.Size(53, 23);
            this.lblLookUp.TabIndex = 0;
            this.lblLookUp.Text = "Lookup";
            // 
            // cmbLookUp
            // 
            this.cmbLookUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbLookUp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLookUp.FormattingEnabled = true;
            this.cmbLookUp.Location = new System.Drawing.Point(62, 3);
            this.cmbLookUp.Name = "cmbLookUp";
            this.cmbLookUp.Size = new System.Drawing.Size(302, 23);
            this.cmbLookUp.TabIndex = 1;
            this.cmbLookUp.SelectedIndexChanged += new System.EventHandler(this.cmbLookUp_SelectedIndexChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(3, 7);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(3, 7, 8, 3);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lblSearch.Size = new System.Drawing.Size(48, 15);
            this.lblSearch.TabIndex = 2;
            this.lblSearch.Text = "Search";
            // 
            // UserInputLookupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(377, 357);
            this.Controls.Add(this.tpnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserInputLookupForm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Shown += new System.EventHandler(this.UserInputLookupForm_Shown);
            this.tpnlMain.ResumeLayout(false);
            this.tpnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tpnlLookUp.ResumeLayout(false);
            this.tpnlLookUp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tpnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridViewLinkColumn ObjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDColumn;
        private System.Windows.Forms.TableLayoutPanel tpnlLookUp;
        private System.Windows.Forms.Label lblLookUp;
        private System.Windows.Forms.ComboBox cmbLookUp;
        private System.Windows.Forms.Label lblSearch;
    }
}