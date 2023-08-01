namespace Apttus.XAuthor.AppDesigner
{
    partial class FieldsMismatchViewer
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDoubleClicktoViewDetails = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFieldTypeMismatchDescription = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblFieldTypeMismatch = new System.Windows.Forms.Label();
            this.dgvFieldTypeMismatch = new System.Windows.Forms.DataGridView();
            this.Selectchk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Object = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InApp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InSalesforce = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFieldTypeMismatch)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblDoubleClicktoViewDetails, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblFieldTypeMismatch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvFieldTypeMismatch, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(828, 260);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblDoubleClicktoViewDetails
            // 
            this.lblDoubleClicktoViewDetails.AutoSize = true;
            this.lblDoubleClicktoViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoubleClicktoViewDetails.Location = new System.Drawing.Point(5, 240);
            this.lblDoubleClicktoViewDetails.Margin = new System.Windows.Forms.Padding(5);
            this.lblDoubleClicktoViewDetails.Name = "lblDoubleClicktoViewDetails";
            this.lblDoubleClicktoViewDetails.Size = new System.Drawing.Size(818, 15);
            this.lblDoubleClicktoViewDetails.TabIndex = 10;
            this.lblDoubleClicktoViewDetails.Text = "Double click to view details.";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.94669F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.05332F));
            this.tableLayoutPanel2.Controls.Add(this.lblFieldTypeMismatchDescription, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnApply, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(822, 33);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // lblFieldTypeMismatchDescription
            // 
            this.lblFieldTypeMismatchDescription.AutoSize = true;
            this.lblFieldTypeMismatchDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFieldTypeMismatchDescription.Location = new System.Drawing.Point(5, 5);
            this.lblFieldTypeMismatchDescription.Margin = new System.Windows.Forms.Padding(5);
            this.lblFieldTypeMismatchDescription.Name = "lblFieldTypeMismatchDescription";
            this.lblFieldTypeMismatchDescription.Size = new System.Drawing.Size(721, 23);
            this.lblFieldTypeMismatchDescription.TabIndex = 2;
            this.lblFieldTypeMismatchDescription.Text = "The following field types in your app do not match with those in your Salesforce " +
    "org. Sync field types here or update them in Salesforce.";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnApply.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnApply.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(747, 3);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 27);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Sync Fields";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblFieldTypeMismatch
            // 
            this.lblFieldTypeMismatch.AutoSize = true;
            this.lblFieldTypeMismatch.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFieldTypeMismatch.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblFieldTypeMismatch.Location = new System.Drawing.Point(5, 5);
            this.lblFieldTypeMismatch.Margin = new System.Windows.Forms.Padding(5);
            this.lblFieldTypeMismatch.Name = "lblFieldTypeMismatch";
            this.lblFieldTypeMismatch.Size = new System.Drawing.Size(202, 30);
            this.lblFieldTypeMismatch.TabIndex = 5;
            this.lblFieldTypeMismatch.Text = "Field Type Mismatch\r\n";
            // 
            // dgvFieldTypeMismatch
            // 
            this.dgvFieldTypeMismatch.AllowUserToAddRows = false;
            this.dgvFieldTypeMismatch.AllowUserToDeleteRows = false;
            this.dgvFieldTypeMismatch.AllowUserToOrderColumns = true;
            this.dgvFieldTypeMismatch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFieldTypeMismatch.BackgroundColor = System.Drawing.Color.White;
            this.dgvFieldTypeMismatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFieldTypeMismatch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selectchk,
            this.Object,
            this.Field,
            this.InApp,
            this.InSalesforce});
            this.dgvFieldTypeMismatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFieldTypeMismatch.GridColor = System.Drawing.Color.White;
            this.dgvFieldTypeMismatch.Location = new System.Drawing.Point(3, 82);
            this.dgvFieldTypeMismatch.Name = "dgvFieldTypeMismatch";
            this.dgvFieldTypeMismatch.RowHeadersVisible = false;
            this.dgvFieldTypeMismatch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFieldTypeMismatch.Size = new System.Drawing.Size(822, 150);
            this.dgvFieldTypeMismatch.TabIndex = 6;
            this.dgvFieldTypeMismatch.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvFieldTypeMismatch_CellDoubleClick);
            // 
            // Selectchk
            // 
            this.Selectchk.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Selectchk.FalseValue = "0";
            this.Selectchk.HeaderText = "";
            this.Selectchk.Name = "Selectchk";
            this.Selectchk.TrueValue = "1";
            this.Selectchk.Width = 35;
            // 
            // Object
            // 
            this.Object.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Object.DataPropertyName = "Object";
            this.Object.FillWeight = 30F;
            this.Object.HeaderText = "Object";
            this.Object.Name = "Object";
            this.Object.ReadOnly = true;
            // 
            // Field
            // 
            this.Field.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Field.DataPropertyName = "Field";
            this.Field.FillWeight = 30F;
            this.Field.HeaderText = "Field";
            this.Field.Name = "Field";
            this.Field.ReadOnly = true;
            // 
            // InApp
            // 
            this.InApp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InApp.DataPropertyName = "AppDataType";
            this.InApp.FillWeight = 15F;
            this.InApp.HeaderText = "In App";
            this.InApp.Name = "InApp";
            this.InApp.ReadOnly = true;
            // 
            // InSalesforce
            // 
            this.InSalesforce.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InSalesforce.DataPropertyName = "SFDataType";
            this.InSalesforce.FillWeight = 15F;
            this.InSalesforce.HeaderText = "In Salesforce";
            this.InSalesforce.Name = "InSalesforce";
            this.InSalesforce.ReadOnly = true;
            // 
            // FieldsMismatchViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FieldsMismatchViewer";
            this.Size = new System.Drawing.Size(828, 260);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFieldTypeMismatch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblFieldTypeMismatch;
        private System.Windows.Forms.DataGridView dgvFieldTypeMismatch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblFieldTypeMismatchDescription;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selectchk;
        private System.Windows.Forms.DataGridViewTextBoxColumn Object;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn InApp;
        private System.Windows.Forms.DataGridViewTextBoxColumn InSalesforce;
        private System.Windows.Forms.Label lblDoubleClicktoViewDetails;
    }
}
