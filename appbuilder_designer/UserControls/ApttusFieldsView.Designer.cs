namespace Apttus.XAuthor.AppDesigner
{
    partial class ApttusFieldsView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lnkClear = new System.Windows.Forms.LinkLabel();
            this.txtSearchFields = new System.Windows.Forms.TextBox();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.dcCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dcId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcDatatype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tblayoutSearchFields = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSearchFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblSearchFields = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.tblayoutSearchFields.SuspendLayout();
            this.pnlSearchFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnkClear
            // 
            this.lnkClear.AutoSize = true;
            this.lnkClear.Location = new System.Drawing.Point(295, 5);
            this.lnkClear.Margin = new System.Windows.Forms.Padding(5);
            this.lnkClear.Name = "lnkClear";
            this.lnkClear.Size = new System.Drawing.Size(34, 15);
            this.lnkClear.TabIndex = 11;
            this.lnkClear.TabStop = true;
            this.lnkClear.Text = "Clear";
            this.lnkClear.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClear_LinkClicked);
            // 
            // txtSearchFields
            // 
            this.txtSearchFields.Location = new System.Drawing.Point(92, 1);
            this.txtSearchFields.Margin = new System.Windows.Forms.Padding(1);
            this.txtSearchFields.Name = "txtSearchFields";
            this.txtSearchFields.Size = new System.Drawing.Size(197, 23);
            this.txtSearchFields.TabIndex = 9;
            this.txtSearchFields.TextChanged += new System.EventHandler(this.txtSearchFields_TextChanged);
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToAddRows = false;
            this.dgvFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFields.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvFields.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvFields.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcCheck,
            this.dcId,
            this.dcName,
            this.dcDatatype});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFields.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.GridColor = System.Drawing.Color.Silver;
            this.dgvFields.Location = new System.Drawing.Point(3, 28);
            this.dgvFields.Name = "dgvFields";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFields.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFields.RowHeadersVisible = false;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            this.dgvFields.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFields.Size = new System.Drawing.Size(550, 250);
            this.dgvFields.TabIndex = 8;
            this.dgvFields.DataSourceChanged += new System.EventHandler(this.dgvFields_DataSourceChanged);
            this.dgvFields.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFields_CellMouseClick);
            this.dgvFields.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellValueChanged);
            this.dgvFields.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFields_ColumnHeaderMouseClick);
            this.dgvFields.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvFields_CurrentCellDirtyStateChanged);
            // 
            // dcCheck
            // 
            this.dcCheck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dcCheck.DataPropertyName = "Included";
            this.dcCheck.FillWeight = 16.24366F;
            this.dcCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dcCheck.HeaderText = "";
            this.dcCheck.Name = "dcCheck";
            this.dcCheck.Width = 5;
            // 
            // dcId
            // 
            this.dcId.DataPropertyName = "Id";
            this.dcId.FillWeight = 127.9188F;
            this.dcId.HeaderText = "Field Id";
            this.dcId.Name = "dcId";
            this.dcId.ReadOnly = true;
            // 
            // dcName
            // 
            this.dcName.DataPropertyName = "Name";
            this.dcName.FillWeight = 127.9188F;
            this.dcName.HeaderText = "Field Name";
            this.dcName.Name = "dcName";
            this.dcName.ReadOnly = true;
            // 
            // dcDatatype
            // 
            this.dcDatatype.DataPropertyName = "Datatype";
            this.dcDatatype.FillWeight = 127.9188F;
            this.dcDatatype.HeaderText = "Data Type";
            this.dcDatatype.Name = "dcDatatype";
            this.dcDatatype.ReadOnly = true;
            // 
            // tblayoutSearchFields
            // 
            this.tblayoutSearchFields.AutoSize = true;
            this.tblayoutSearchFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblayoutSearchFields.BackColor = System.Drawing.SystemColors.Window;
            this.tblayoutSearchFields.ColumnCount = 1;
            this.tblayoutSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblayoutSearchFields.Controls.Add(this.dgvFields, 0, 1);
            this.tblayoutSearchFields.Controls.Add(this.pnlSearchFields, 0, 0);
            this.tblayoutSearchFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblayoutSearchFields.Location = new System.Drawing.Point(0, 0);
            this.tblayoutSearchFields.Margin = new System.Windows.Forms.Padding(0);
            this.tblayoutSearchFields.Name = "tblayoutSearchFields";
            this.tblayoutSearchFields.RowCount = 2;
            this.tblayoutSearchFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutSearchFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblayoutSearchFields.Size = new System.Drawing.Size(556, 281);
            this.tblayoutSearchFields.TabIndex = 9;
            // 
            // pnlSearchFields
            // 
            this.pnlSearchFields.AutoSize = true;
            this.pnlSearchFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSearchFields.ColumnCount = 3;
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSearchFields.Controls.Add(this.lblSearchFields);
            this.pnlSearchFields.Controls.Add(this.txtSearchFields);
            this.pnlSearchFields.Controls.Add(this.lnkClear);
            this.pnlSearchFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearchFields.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlSearchFields.Location = new System.Drawing.Point(0, 0);
            this.pnlSearchFields.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSearchFields.Name = "pnlSearchFields";
            this.pnlSearchFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSearchFields.Size = new System.Drawing.Size(556, 25);
            this.pnlSearchFields.TabIndex = 9;
            // 
            // lblSearchFields
            // 
            this.lblSearchFields.AutoSize = true;
            this.lblSearchFields.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSearchFields.Location = new System.Drawing.Point(5, 5);
            this.lblSearchFields.Margin = new System.Windows.Forms.Padding(5);
            this.lblSearchFields.Name = "lblSearchFields";
            this.lblSearchFields.Size = new System.Drawing.Size(81, 15);
            this.lblSearchFields.TabIndex = 12;
            this.lblSearchFields.Text = "Search Fields :";
            // 
            // ApttusFieldsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tblayoutSearchFields);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ApttusFieldsView";
            this.Size = new System.Drawing.Size(556, 281);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.tblayoutSearchFields.ResumeLayout(false);
            this.tblayoutSearchFields.PerformLayout();
            this.pnlSearchFields.ResumeLayout(false);
            this.pnlSearchFields.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.TextBox txtSearchFields;
        private System.Windows.Forms.LinkLabel lnkClear;
        private System.Windows.Forms.TableLayoutPanel tblayoutSearchFields;
        private System.Windows.Forms.Label lblSearchFields;
        private System.Windows.Forms.TableLayoutPanel pnlSearchFields;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dcCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcDatatype;
    }
}
