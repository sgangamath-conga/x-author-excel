namespace Apttus.XAuthor.AppDesigner
{
    partial class FieldsMissingViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblMissingFields = new System.Windows.Forms.Label();
            this.DgvMissingFields = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Object = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveMap = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchFilters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDoubleClicktoViewDetails = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DgvMissingFields)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMissingFields
            // 
            this.lblMissingFields.AutoSize = true;
            this.lblMissingFields.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMissingFields.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblMissingFields.Location = new System.Drawing.Point(5, 5);
            this.lblMissingFields.Margin = new System.Windows.Forms.Padding(5);
            this.lblMissingFields.Name = "lblMissingFields";
            this.lblMissingFields.Size = new System.Drawing.Size(284, 30);
            this.lblMissingFields.TabIndex = 5;
            this.lblMissingFields.Text = "Missing or Inaccessible Fields";
            // 
            // DgvMissingFields
            // 
            this.DgvMissingFields.AllowUserToOrderColumns = true;
            this.DgvMissingFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DgvMissingFields.BackgroundColor = System.Drawing.Color.White;
            this.DgvMissingFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvMissingFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Object,
            this.Field,
            this.DisplayMap,
            this.SaveMap,
            this.SearchFilters});
            this.DgvMissingFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvMissingFields.GridColor = System.Drawing.Color.White;
            this.DgvMissingFields.Location = new System.Drawing.Point(3, 82);
            this.DgvMissingFields.Name = "DgvMissingFields";
            this.DgvMissingFields.RowHeadersVisible = false;
            this.DgvMissingFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvMissingFields.Size = new System.Drawing.Size(822, 150);
            this.DgvMissingFields.TabIndex = 7;
            this.DgvMissingFields.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvFieldTypeMismatch_CellDoubleClick);
            // 
            // Select
            // 
            this.Select.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Select.FalseValue = "0";
            this.Select.FillWeight = 5F;
            this.Select.Frozen = true;
            this.Select.HeaderText = "";
            this.Select.Name = "Select";
            this.Select.TrueValue = "1";
            this.Select.Width = 35;
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
            // DisplayMap
            // 
            this.DisplayMap.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DisplayMap.DataPropertyName = "DisplayMap";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.DisplayMap.DefaultCellStyle = dataGridViewCellStyle1;
            this.DisplayMap.FillWeight = 15F;
            this.DisplayMap.HeaderText = "Display Map";
            this.DisplayMap.Name = "DisplayMap";
            this.DisplayMap.ReadOnly = true;
            // 
            // SaveMap
            // 
            this.SaveMap.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SaveMap.DataPropertyName = "SaveMap";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SaveMap.DefaultCellStyle = dataGridViewCellStyle2;
            this.SaveMap.FillWeight = 15F;
            this.SaveMap.HeaderText = "Save Map";
            this.SaveMap.Name = "SaveMap";
            this.SaveMap.ReadOnly = true;
            // 
            // SearchFilters
            // 
            this.SearchFilters.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SearchFilters.DataPropertyName = "SearchFilters";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.SearchFilters.DefaultCellStyle = dataGridViewCellStyle3;
            this.SearchFilters.FillWeight = 15F;
            this.SearchFilters.HeaderText = "Search Filters";
            this.SearchFilters.Name = "SearchFilters";
            this.SearchFilters.ReadOnly = true;
            this.SearchFilters.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SearchFilters.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblDoubleClicktoViewDetails, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.DgvMissingFields, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblMissingFields, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
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
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // lblDoubleClicktoViewDetails
            // 
            this.lblDoubleClicktoViewDetails.AutoSize = true;
            this.lblDoubleClicktoViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoubleClicktoViewDetails.Location = new System.Drawing.Point(5, 240);
            this.lblDoubleClicktoViewDetails.Margin = new System.Windows.Forms.Padding(5);
            this.lblDoubleClicktoViewDetails.Name = "lblDoubleClicktoViewDetails";
            this.lblDoubleClicktoViewDetails.Size = new System.Drawing.Size(818, 15);
            this.lblDoubleClicktoViewDetails.TabIndex = 11;
            this.lblDoubleClicktoViewDetails.Text = "Double click to view details.";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.94668F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.05332F));
            this.tableLayoutPanel2.Controls.Add(this.btnRemove, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(822, 33);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(747, 3);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 27);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // FieldsMissingViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FieldsMissingViewer";
            this.Size = new System.Drawing.Size(828, 260);
            ((System.ComponentModel.ISupportInitialize)(this.DgvMissingFields)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMissingFields;
        private System.Windows.Forms.DataGridView DgvMissingFields;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn Object;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn SearchFilters;
        private System.Windows.Forms.Label lblDoubleClicktoViewDetails;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnRemove;
    }
}
