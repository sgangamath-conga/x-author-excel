namespace Apttus.XAuthor.AppDesigner
{
    partial class ObjectAndFieldBrowserView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlObjects = new System.Windows.Forms.Panel();
            this.tvObjects = new System.Windows.Forms.TreeView();
            this.pnlFields = new System.Windows.Forms.Panel();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.pnlAction = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlBreadCrumb = new System.Windows.Forms.FlowLayoutPanel();
            this.tblObjectAndFieldBrowser = new System.Windows.Forms.TableLayoutPanel();
            this.dcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcDatatype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlObjects.SuspendLayout();
            this.pnlFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.pnlAction.SuspendLayout();
            this.tblObjectAndFieldBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlObjects
            // 
            this.pnlObjects.AutoSize = true;
            this.pnlObjects.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlObjects.Controls.Add(this.tvObjects);
            this.pnlObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlObjects.Location = new System.Drawing.Point(3, 14);
            this.pnlObjects.Name = "pnlObjects";
            this.pnlObjects.Padding = new System.Windows.Forms.Padding(3);
            this.pnlObjects.Size = new System.Drawing.Size(222, 272);
            this.pnlObjects.TabIndex = 0;
            // 
            // tvObjects
            // 
            this.tvObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvObjects.Location = new System.Drawing.Point(6, 6);
            this.tvObjects.MinimumSize = new System.Drawing.Size(210, 260);
            this.tvObjects.Name = "tvObjects";
            this.tvObjects.Size = new System.Drawing.Size(210, 260);
            this.tvObjects.TabIndex = 0;
            this.tvObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvObjects_AfterSelect);
            // 
            // pnlFields
            // 
            this.pnlFields.AutoSize = true;
            this.pnlFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlFields.Controls.Add(this.dgvFields);
            this.pnlFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFields.Location = new System.Drawing.Point(231, 14);
            this.pnlFields.Name = "pnlFields";
            this.pnlFields.Size = new System.Drawing.Size(431, 272);
            this.pnlFields.TabIndex = 1;
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToAddRows = false;
            this.dgvFields.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFields.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvFields.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvFields.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcName,
            this.dcId,
            this.dcDatatype});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFields.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.GridColor = System.Drawing.Color.Silver;
            this.dgvFields.Location = new System.Drawing.Point(0, 0);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFields.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvFields.RowHeadersVisible = false;
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFields.Size = new System.Drawing.Size(431, 272);
            this.dgvFields.TabIndex = 9;
            this.dgvFields.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellContentClick);
            this.dgvFields.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFields_CellDoubleClick);
            this.dgvFields.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvFields_DataBindingComplete);
            this.dgvFields.SelectionChanged += new System.EventHandler(this.dgvFields_SelectionChanged);
            // 
            // pnlAction
            // 
            this.pnlAction.AutoSize = true;
            this.pnlAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblObjectAndFieldBrowser.SetColumnSpan(this.pnlAction, 2);
            this.pnlAction.Controls.Add(this.btnCancel);
            this.pnlAction.Controls.Add(this.btnSave);
            this.pnlAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAction.Location = new System.Drawing.Point(3, 292);
            this.pnlAction.Name = "pnlAction";
            this.pnlAction.Size = new System.Drawing.Size(659, 34);
            this.pnlAction.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(563, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(470, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 27);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Select";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlBreadCrumb
            // 
            this.pnlBreadCrumb.AutoSize = true;
            this.pnlBreadCrumb.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblObjectAndFieldBrowser.SetColumnSpan(this.pnlBreadCrumb, 2);
            this.pnlBreadCrumb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBreadCrumb.Font = new System.Drawing.Font("Segoe UI Semilight", 10F);
            this.pnlBreadCrumb.ForeColor = System.Drawing.Color.DarkGreen;
            this.pnlBreadCrumb.Location = new System.Drawing.Point(3, 3);
            this.pnlBreadCrumb.Name = "pnlBreadCrumb";
            this.pnlBreadCrumb.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlBreadCrumb.Size = new System.Drawing.Size(659, 5);
            this.pnlBreadCrumb.TabIndex = 3;
            // 
            // tblObjectAndFieldBrowser
            // 
            this.tblObjectAndFieldBrowser.AutoSize = true;
            this.tblObjectAndFieldBrowser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblObjectAndFieldBrowser.ColumnCount = 2;
            this.tblObjectAndFieldBrowser.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblObjectAndFieldBrowser.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblObjectAndFieldBrowser.Controls.Add(this.pnlObjects, 0, 1);
            this.tblObjectAndFieldBrowser.Controls.Add(this.pnlAction, 0, 2);
            this.tblObjectAndFieldBrowser.Controls.Add(this.pnlBreadCrumb, 0, 0);
            this.tblObjectAndFieldBrowser.Controls.Add(this.pnlFields, 1, 1);
            this.tblObjectAndFieldBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblObjectAndFieldBrowser.Location = new System.Drawing.Point(0, 0);
            this.tblObjectAndFieldBrowser.Name = "tblObjectAndFieldBrowser";
            this.tblObjectAndFieldBrowser.RowCount = 3;
            this.tblObjectAndFieldBrowser.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblObjectAndFieldBrowser.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblObjectAndFieldBrowser.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblObjectAndFieldBrowser.Size = new System.Drawing.Size(665, 329);
            this.tblObjectAndFieldBrowser.TabIndex = 4;
            // 
            // dcName
            // 
            this.dcName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcName.DataPropertyName = "Name";
            this.dcName.FillWeight = 124.7208F;
            this.dcName.HeaderText = "Field Name";
            this.dcName.Name = "dcName";
            this.dcName.ReadOnly = true;
            // 
            // dcId
            // 
            this.dcId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcId.DataPropertyName = "Id";
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dcId.DefaultCellStyle = dataGridViewCellStyle10;
            this.dcId.FillWeight = 124.7208F;
            this.dcId.HeaderText = "Field Id";
            this.dcId.Name = "dcId";
            this.dcId.ReadOnly = true;
            // 
            // dcDatatype
            // 
            this.dcDatatype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcDatatype.DataPropertyName = "Datatype";
            this.dcDatatype.FillWeight = 65.55837F;
            this.dcDatatype.HeaderText = "Data Type";
            this.dcDatatype.Name = "dcDatatype";
            this.dcDatatype.ReadOnly = true;
            // 
            // ObjectAndFieldBrowserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(665, 329);
            this.Controls.Add(this.tblObjectAndFieldBrowser);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ObjectAndFieldBrowserView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.ObjectAndFieldBrowserView_Load);
            this.pnlObjects.ResumeLayout(false);
            this.pnlFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.pnlAction.ResumeLayout(false);
            this.tblObjectAndFieldBrowser.ResumeLayout(false);
            this.tblObjectAndFieldBrowser.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlObjects;
        private System.Windows.Forms.Panel pnlFields;
        private System.Windows.Forms.Panel pnlAction;
        private System.Windows.Forms.TreeView tvObjects;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.FlowLayoutPanel pnlBreadCrumb;
        private System.Windows.Forms.TableLayoutPanel tblObjectAndFieldBrowser;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcDatatype;
    }
}