namespace Apttus.XAuthor.AppDesigner
{
    partial class DMDependencyIdentifier
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
            this.tplMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblLookupDescription = new System.Windows.Forms.Label();
            this.pnlSaveCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.trLookup = new System.Windows.Forms.TreeView();
            this.tplMain.SuspendLayout();
            this.pnlSaveCancel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tplMain
            // 
            this.tplMain.ColumnCount = 1;
            this.tplMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplMain.Controls.Add(this.lblLookupDescription, 0, 1);
            this.tplMain.Controls.Add(this.pnlSaveCancel, 0, 3);
            this.tplMain.Controls.Add(this.lblTitle, 0, 0);
            this.tplMain.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tplMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tplMain.Location = new System.Drawing.Point(2, 2);
            this.tplMain.Margin = new System.Windows.Forms.Padding(0);
            this.tplMain.Name = "tplMain";
            this.tplMain.RowCount = 4;
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.Size = new System.Drawing.Size(471, 459);
            this.tplMain.TabIndex = 1;
            // 
            // lblLookupDescription
            // 
            this.lblLookupDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLookupDescription.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLookupDescription.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblLookupDescription.Location = new System.Drawing.Point(3, 38);
            this.lblLookupDescription.Margin = new System.Windows.Forms.Padding(3);
            this.lblLookupDescription.Name = "lblLookupDescription";
            this.lblLookupDescription.Size = new System.Drawing.Size(465, 40);
            this.lblLookupDescription.TabIndex = 5;
            this.lblLookupDescription.Text = "Use this screen to view and add the suggested lookup fields for the objects inclu" +
    "ded in the dashboard.";
            this.lblLookupDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSaveCancel
            // 
            this.pnlSaveCancel.Controls.Add(this.btnCancel);
            this.pnlSaveCancel.Controls.Add(this.btnSave);
            this.pnlSaveCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaveCancel.Location = new System.Drawing.Point(0, 425);
            this.pnlSaveCancel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSaveCancel.Name = "pnlSaveCancel";
            this.pnlSaveCancel.Padding = new System.Windows.Forms.Padding(3);
            this.pnlSaveCancel.Size = new System.Drawing.Size(471, 34);
            this.pnlSaveCancel.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(383, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(288, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Add";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Location = new System.Drawing.Point(5, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(133, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Add Lookups";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.trLookup, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 84);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 338F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(465, 338);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // trLookup
            // 
            this.trLookup.CheckBoxes = true;
            this.trLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trLookup.Location = new System.Drawing.Point(3, 3);
            this.trLookup.Name = "trLookup";
            this.trLookup.ShowNodeToolTips = true;
            this.trLookup.Size = new System.Drawing.Size(465, 332);
            this.trLookup.TabIndex = 9;
            // 
            // DMDependencyIdentifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(475, 463);
            this.Controls.Add(this.tplMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DMDependencyIdentifier";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.DMDependencyIdentifier_Load);
            this.tplMain.ResumeLayout(false);
            this.tplMain.PerformLayout();
            this.pnlSaveCancel.ResumeLayout(false);
            this.pnlSaveCancel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tplMain;
        private System.Windows.Forms.Panel pnlSaveCancel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLookupDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView trLookup;
    }
}