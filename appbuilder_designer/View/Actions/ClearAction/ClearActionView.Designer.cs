namespace Apttus.XAuthor.AppDesigner
{
    partial class ClearActionView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.chkDisableExcelEvents = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.cboMap = new System.Windows.Forms.ComboBox();
            this.chkAppObjects = new System.Windows.Forms.CheckedListBox();
            this.panel4 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvSelectedObjects = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveMapName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppObject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedObjects)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(455, 461);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.ColumnCount = 2;
            this.panel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel1.Controls.Add(this.txtName, 1, 0);
            this.panel1.Controls.Add(this.label2, 0, 0);
            this.panel1.Controls.Add(this.chkDisableExcelEvents, 0, 1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 50);
            this.panel1.Name = "panel1";
            this.panel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel1.Size = new System.Drawing.Size(449, 33);
            this.panel1.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(149, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(240, 27);
            this.txtName.TabIndex = 0;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // chkDisableExcelEvents
            // 
            this.chkDisableExcelEvents.AutoSize = true;
            this.chkDisableExcelEvents.Checked = true;
            this.chkDisableExcelEvents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.panel1.SetColumnSpan(this.chkDisableExcelEvents, 2);
            this.chkDisableExcelEvents.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDisableExcelEvents.Location = new System.Drawing.Point(5, 142);
            this.chkDisableExcelEvents.Margin = new System.Windows.Forms.Padding(5, 5, 3, 3);
            this.chkDisableExcelEvents.Name = "chkDisableExcelEvents";
            this.chkDisableExcelEvents.Size = new System.Drawing.Size(289, 24);
            this.chkDisableExcelEvents.TabIndex = 11;
            this.chkDisableExcelEvents.Text = "Disable Excel Events during execution";
            this.chkDisableExcelEvents.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(9, 5, 34, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Action Name :";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.panel2.Controls.Add(this.chkAppObjects, 0, 1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 89);
            this.panel2.Name = "panel2";
            this.panel2.RowCount = 2;
            this.panel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel2.Size = new System.Drawing.Size(449, 133);
            this.panel2.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cboMap, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(443, 31);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 5);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Display / Save Map :";
            // 
            // cboMap
            // 
            this.cboMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMap.FormattingEnabled = true;
            this.cboMap.Location = new System.Drawing.Point(157, 3);
            this.cboMap.Name = "cboMap";
            this.cboMap.Size = new System.Drawing.Size(240, 28);
            this.cboMap.TabIndex = 1;
            this.cboMap.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cboMap.SelectedIndexChanged += new System.EventHandler(this.cboSaveMap_SelectedIndexChanged);
            this.cboMap.Validating += new System.ComponentModel.CancelEventHandler(this.cboSaveMap_Validating);
            // 
            // chkAppObjects
            // 
            this.chkAppObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chkAppObjects.CheckOnClick = true;
            this.chkAppObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAppObjects.FormattingEnabled = true;
            this.chkAppObjects.Location = new System.Drawing.Point(3, 40);
            this.chkAppObjects.MinimumSize = new System.Drawing.Size(2, 60);
            this.chkAppObjects.Name = "chkAppObjects";
            this.chkAppObjects.Size = new System.Drawing.Size(443, 90);
            this.chkAppObjects.TabIndex = 2;
            this.chkAppObjects.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkAppObjects_ItemCheck);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel4.Controls.Add(this.dgvSelectedObjects, 0, 1);
            this.panel4.Controls.Add(this.label6, 0, 0);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 228);
            this.panel4.Name = "panel4";
            this.panel4.RowCount = 2;
            this.panel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel4.Size = new System.Drawing.Size(449, 202);
            this.panel4.TabIndex = 2;
            // 
            // dgvSelectedObjects
            // 
            this.dgvSelectedObjects.AllowUserToAddRows = false;
            this.dgvSelectedObjects.AllowUserToDeleteRows = false;
            this.dgvSelectedObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSelectedObjects.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvSelectedObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSelectedObjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.SaveMapName,
            this.AppObject,
            this.ObjectType});
            this.dgvSelectedObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSelectedObjects.Location = new System.Drawing.Point(3, 33);
            this.dgvSelectedObjects.Name = "dgvSelectedObjects";
            this.dgvSelectedObjects.RowHeadersVisible = false;
            this.dgvSelectedObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSelectedObjects.Size = new System.Drawing.Size(443, 166);
            this.dgvSelectedObjects.TabIndex = 0;
            // 
            // Index
            // 
            this.Index.DataPropertyName = "Index";
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Index.Visible = false;
            // 
            // SaveMapName
            // 
            this.SaveMapName.DataPropertyName = "MapName";
            this.SaveMapName.HeaderText = "Map Name";
            this.SaveMapName.Name = "SaveMapName";
            this.SaveMapName.ReadOnly = true;
            // 
            // AppObject
            // 
            this.AppObject.DataPropertyName = "AppObject";
            this.AppObject.HeaderText = "Object";
            this.AppObject.Name = "AppObject";
            this.AppObject.ReadOnly = true;
            // 
            // ObjectType
            // 
            this.ObjectType.DataPropertyName = "ObjectType";
            this.ObjectType.HeaderText = "Object Type";
            this.ObjectType.Name = "ObjectType";
            this.ObjectType.ReadOnly = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label6.Location = new System.Drawing.Point(5, 5);
            this.label6.Margin = new System.Windows.Forms.Padding(5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Selected Objects";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnOK);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(5, 438);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(445, 38);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(349, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(250, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Clear Fields Action";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ClearActionView
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(455, 461);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClearActionView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Shown += new System.EventHandler(this.ClearActionView_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSelectedObjects)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TableLayoutPanel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboMap;
        private System.Windows.Forms.TableLayoutPanel panel4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dgvSelectedObjects;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveMapName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppObject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectType;
        private System.Windows.Forms.CheckedListBox chkAppObjects;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkDisableExcelEvents;
    }
}