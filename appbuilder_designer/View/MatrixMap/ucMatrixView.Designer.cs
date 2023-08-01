namespace Apttus.XAuthor.AppDesigner
{
    partial class ucMatrixView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabMatrixControl = new System.Windows.Forms.TabControl();
            this.MatrixMapTabPage = new System.Windows.Forms.TabPage();
            this.tblMatrixMap = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tvFields = new System.Windows.Forms.TreeView();
            this.panel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvMatrixMap = new System.Windows.Forms.DataGridView();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Entity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRenderingType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ComponentsTabPage = new System.Windows.Forms.TabPage();
            this.tblMatrixComponent = new System.Windows.Forms.TableLayoutPanel();
            this.dgvMatrixComponent = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExtendComponent = new System.Windows.Forms.Button();
            this.tblMatrixView = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cboObjectType = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ComponentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Applyfilter = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabMatrixControl.SuspendLayout();
            this.MatrixMapTabPage.SuspendLayout();
            this.tblMatrixMap.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMatrixMap)).BeginInit();
            this.ComponentsTabPage.SuspendLayout();
            this.tblMatrixComponent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMatrixComponent)).BeginInit();
            this.panel1.SuspendLayout();
            this.tblMatrixView.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tabMatrixControl
            // 
            this.tabMatrixControl.Controls.Add(this.MatrixMapTabPage);
            this.tabMatrixControl.Controls.Add(this.ComponentsTabPage);
            this.tabMatrixControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMatrixControl.Location = new System.Drawing.Point(3, 34);
            this.tabMatrixControl.Name = "tabMatrixControl";
            this.tabMatrixControl.SelectedIndex = 0;
            this.tabMatrixControl.Size = new System.Drawing.Size(600, 428);
            this.tabMatrixControl.TabIndex = 0;
            this.tabMatrixControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // MatrixMapTabPage
            // 
            this.MatrixMapTabPage.AutoScroll = true;
            this.MatrixMapTabPage.Controls.Add(this.tblMatrixMap);
            this.MatrixMapTabPage.Location = new System.Drawing.Point(4, 24);
            this.MatrixMapTabPage.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.MatrixMapTabPage.Name = "MatrixMapTabPage";
            this.MatrixMapTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.MatrixMapTabPage.Size = new System.Drawing.Size(592, 400);
            this.MatrixMapTabPage.TabIndex = 0;
            this.MatrixMapTabPage.Text = "Fields";
            this.MatrixMapTabPage.UseVisualStyleBackColor = true;
            // 
            // tblMatrixMap
            // 
            this.tblMatrixMap.AutoScroll = true;
            this.tblMatrixMap.AutoSize = true;
            this.tblMatrixMap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMatrixMap.ColumnCount = 1;
            this.tblMatrixMap.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMatrixMap.Controls.Add(this.panel2, 0, 0);
            this.tblMatrixMap.Controls.Add(this.panel3, 0, 1);
            this.tblMatrixMap.Controls.Add(this.panel4, 0, 3);
            this.tblMatrixMap.Controls.Add(this.dgvMatrixMap, 0, 2);
            this.tblMatrixMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMatrixMap.Location = new System.Drawing.Point(3, 3);
            this.tblMatrixMap.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tblMatrixMap.Name = "tblMatrixMap";
            this.tblMatrixMap.RowCount = 4;
            this.tblMatrixMap.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMatrixMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tblMatrixMap.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tblMatrixMap.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMatrixMap.Size = new System.Drawing.Size(586, 394);
            this.tblMatrixMap.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.ColumnCount = 2;
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.Controls.Add(this.cboObject, 1, 0);
            this.panel2.Controls.Add(this.label3, 0, 0);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel2.Size = new System.Drawing.Size(580, 29);
            this.panel2.TabIndex = 1;
            // 
            // cboObject
            // 
            this.cboObject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(61, 3);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(516, 23);
            this.cboObject.TabIndex = 3;
            this.cboObject.SelectedIndexChanged += new System.EventHandler(this.cboObject_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(5, 5);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "Object :";
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel3.Controls.Add(this.tvFields);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 35);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel3.Size = new System.Drawing.Size(586, 197);
            this.panel3.TabIndex = 2;
            // 
            // tvFields
            // 
            this.tvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFields.Location = new System.Drawing.Point(3, 3);
            this.tvFields.Name = "tvFields";
            this.tvFields.Size = new System.Drawing.Size(580, 191);
            this.tvFields.TabIndex = 0;
            this.tvFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvFields_ItemDrag);
            this.tvFields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvFields_MouseMove);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Controls.Add(this.btnClose);
            this.panel4.Controls.Add(this.btnRemove);
            this.panel4.Controls.Add(this.btnSave);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel4.Location = new System.Drawing.Point(0, 363);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(586, 31);
            this.panel4.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(508, 3);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnClose.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.AutoSize = true;
            this.btnRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(427, 3);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnRemove.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 27);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(346, 3);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnSave.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvMatrixMap
            // 
            this.dgvMatrixMap.AllowUserToAddRows = false;
            this.dgvMatrixMap.AllowUserToDeleteRows = false;
            this.dgvMatrixMap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMatrixMap.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvMatrixMap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvMatrixMap.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMatrixMap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMatrixMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMatrixMap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FieldName,
            this.Entity,
            this.colRenderingType,
            this.TargetLocation,
            this.Edit});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMatrixMap.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMatrixMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMatrixMap.EnableHeadersVisualStyles = false;
            this.dgvMatrixMap.GridColor = System.Drawing.SystemColors.Window;
            this.dgvMatrixMap.Location = new System.Drawing.Point(3, 235);
            this.dgvMatrixMap.MultiSelect = false;
            this.dgvMatrixMap.Name = "dgvMatrixMap";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMatrixMap.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMatrixMap.RowHeadersVisible = false;
            this.dgvMatrixMap.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMatrixMap.Size = new System.Drawing.Size(580, 125);
            this.dgvMatrixMap.TabIndex = 4;
            this.dgvMatrixMap.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMatrixMap_CellContentClick);
            this.dgvMatrixMap.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvMatrixMap_RowsAdded);
            this.dgvMatrixMap.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvMatrixMap_RowsRemoved);
            // 
            // FieldName
            // 
            this.FieldName.DataPropertyName = "FieldName";
            this.FieldName.FillWeight = 90.7856F;
            this.FieldName.HeaderText = "Field Name";
            this.FieldName.Name = "FieldName";
            this.FieldName.ReadOnly = true;
            // 
            // Entity
            // 
            this.Entity.DataPropertyName = "EntityType";
            this.Entity.FillWeight = 90.7856F;
            this.Entity.HeaderText = "Field Type";
            this.Entity.Name = "Entity";
            this.Entity.ReadOnly = true;
            // 
            // colRenderingType
            // 
            this.colRenderingType.DataPropertyName = "RenderType";
            this.colRenderingType.FillWeight = 100.7856F;
            this.colRenderingType.HeaderText = "Render Type";
            this.colRenderingType.Name = "colRenderingType";
            this.colRenderingType.ReadOnly = true;
            // 
            // TargetLocation
            // 
            this.TargetLocation.DataPropertyName = "Location";
            this.TargetLocation.FillWeight = 90.7856F;
            this.TargetLocation.HeaderText = "Location";
            this.TargetLocation.Name = "TargetLocation";
            this.TargetLocation.ReadOnly = true;
            // 
            // Edit
            // 
            this.Edit.FillWeight = 50.7865F;
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Edit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ComponentsTabPage
            // 
            this.ComponentsTabPage.Controls.Add(this.tblMatrixComponent);
            this.ComponentsTabPage.Location = new System.Drawing.Point(4, 24);
            this.ComponentsTabPage.Name = "ComponentsTabPage";
            this.ComponentsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ComponentsTabPage.Size = new System.Drawing.Size(592, 400);
            this.ComponentsTabPage.TabIndex = 1;
            this.ComponentsTabPage.Text = "Sections";
            this.ComponentsTabPage.UseVisualStyleBackColor = true;
            // 
            // tblMatrixComponent
            // 
            this.tblMatrixComponent.AutoSize = true;
            this.tblMatrixComponent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMatrixComponent.ColumnCount = 1;
            this.tblMatrixComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMatrixComponent.Controls.Add(this.dgvMatrixComponent, 0, 0);
            this.tblMatrixComponent.Controls.Add(this.panel1, 0, 1);
            this.tblMatrixComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMatrixComponent.Location = new System.Drawing.Point(3, 3);
            this.tblMatrixComponent.Name = "tblMatrixComponent";
            this.tblMatrixComponent.RowCount = 2;
            this.tblMatrixComponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMatrixComponent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMatrixComponent.Size = new System.Drawing.Size(586, 394);
            this.tblMatrixComponent.TabIndex = 1;
            // 
            // dgvMatrixComponent
            // 
            this.dgvMatrixComponent.AllowUserToAddRows = false;
            this.dgvMatrixComponent.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvMatrixComponent.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMatrixComponent.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvMatrixComponent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMatrixComponent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ComponentName,
            this.ObjectName,
            this.Applyfilter});
            this.dgvMatrixComponent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMatrixComponent.Location = new System.Drawing.Point(3, 3);
            this.dgvMatrixComponent.MultiSelect = false;
            this.dgvMatrixComponent.Name = "dgvMatrixComponent";
            this.dgvMatrixComponent.RowHeadersVisible = false;
            this.dgvMatrixComponent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMatrixComponent.ShowRowErrors = false;
            this.dgvMatrixComponent.Size = new System.Drawing.Size(580, 350);
            this.dgvMatrixComponent.TabIndex = 1;
            this.dgvMatrixComponent.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMatrixComponent_CellClick);
            this.dgvMatrixComponent.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMatrixComponent_CellContentClick);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnExtendComponent);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel1.Location = new System.Drawing.Point(3, 359);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(580, 32);
            this.panel1.TabIndex = 2;
            // 
            // btnExtendComponent
            // 
            this.btnExtendComponent.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnExtendComponent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnExtendComponent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtendComponent.Location = new System.Drawing.Point(460, 3);
            this.btnExtendComponent.Name = "btnExtendComponent";
            this.btnExtendComponent.Size = new System.Drawing.Size(117, 26);
            this.btnExtendComponent.TabIndex = 0;
            this.btnExtendComponent.Text = "Extend Section";
            this.btnExtendComponent.UseVisualStyleBackColor = true;
            this.btnExtendComponent.Click += new System.EventHandler(this.btnExtendComponent_Click);
            // 
            // tblMatrixView
            // 
            this.tblMatrixView.AutoSize = true;
            this.tblMatrixView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMatrixView.ColumnCount = 1;
            this.tblMatrixView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMatrixView.Controls.Add(this.panel5, 0, 0);
            this.tblMatrixView.Controls.Add(this.tabMatrixControl, 0, 1);
            this.tblMatrixView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMatrixView.Location = new System.Drawing.Point(0, 0);
            this.tblMatrixView.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tblMatrixView.Name = "tblMatrixView";
            this.tblMatrixView.RowCount = 2;
            this.tblMatrixView.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMatrixView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMatrixView.Size = new System.Drawing.Size(606, 465);
            this.tblMatrixView.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.ColumnCount = 4;
            this.panel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel5.Controls.Add(this.label1, 0, 0);
            this.panel5.Controls.Add(this.label2, 2, 0);
            this.panel5.Controls.Add(this.txtName, 1, 0);
            this.panel5.Controls.Add(this.cboObjectType, 3, 0);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(1, 1);
            this.panel5.Margin = new System.Windows.Forms.Padding(1);
            this.panel5.Name = "panel5";
            this.panel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel5.Size = new System.Drawing.Size(604, 29);
            this.panel5.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "Name :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(311, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type :";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(58, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(245, 23);
            this.txtName.TabIndex = 7;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // cboObjectType
            // 
            this.cboObjectType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObjectType.FormattingEnabled = true;
            this.cboObjectType.Items.AddRange(new object[] {
            "Individual",
            "List"});
            this.cboObjectType.Location = new System.Drawing.Point(357, 3);
            this.cboObjectType.Name = "cboObjectType";
            this.cboObjectType.Size = new System.Drawing.Size(244, 23);
            this.cboObjectType.TabIndex = 5;
            this.cboObjectType.SelectedIndexChanged += new System.EventHandler(this.cboObjectType_SelectedIndexChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ComponentName
            // 
            this.ComponentName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ComponentName.DataPropertyName = "ComponentName";
            this.ComponentName.FillWeight = 107.1373F;
            this.ComponentName.HeaderText = "Section Name";
            this.ComponentName.Name = "ComponentName";
            this.ComponentName.ReadOnly = true;
            // 
            // ObjectName
            // 
            this.ObjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ObjectName.DataPropertyName = "ObjectName";
            this.ObjectName.FillWeight = 80.34219F;
            this.ObjectName.HeaderText = "Object Name";
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.ReadOnly = true;
            // 
            // Applyfilter
            // 
            this.Applyfilter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Applyfilter.FillWeight = 45.39606F;
            this.Applyfilter.HeaderText = "Filter";
            this.Applyfilter.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.levels;
            this.Applyfilter.Name = "Applyfilter";
            this.Applyfilter.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ucMatrixView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tblMatrixView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Name = "ucMatrixView";
            this.Size = new System.Drawing.Size(606, 465);
            this.Load += new System.EventHandler(this.ucMatrixView_Load);
            this.tabMatrixControl.ResumeLayout(false);
            this.MatrixMapTabPage.ResumeLayout(false);
            this.MatrixMapTabPage.PerformLayout();
            this.tblMatrixMap.ResumeLayout(false);
            this.tblMatrixMap.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMatrixMap)).EndInit();
            this.ComponentsTabPage.ResumeLayout(false);
            this.ComponentsTabPage.PerformLayout();
            this.tblMatrixComponent.ResumeLayout(false);
            this.tblMatrixComponent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMatrixComponent)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tblMatrixView.ResumeLayout(false);
            this.tblMatrixView.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMatrixControl;
        private System.Windows.Forms.TabPage MatrixMapTabPage;
        private System.Windows.Forms.TableLayoutPanel tblMatrixMap;
        private System.Windows.Forms.TableLayoutPanel panel2;
        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel panel4;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvMatrixMap;
        private System.Windows.Forms.TabPage ComponentsTabPage;
        private System.Windows.Forms.TableLayoutPanel tblMatrixComponent;
        private System.Windows.Forms.DataGridView dgvMatrixComponent;
        private System.Windows.Forms.TableLayoutPanel tblMatrixView;
        private System.Windows.Forms.TableLayoutPanel panel5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.Button btnExtendComponent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboObjectType;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Entity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRenderingType;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetLocation;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.TableLayoutPanel panel3;
        private System.Windows.Forms.TreeView tvFields;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComponentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectName;
        private System.Windows.Forms.DataGridViewImageColumn Applyfilter;


    }
}
