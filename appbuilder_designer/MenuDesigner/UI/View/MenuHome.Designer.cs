namespace Apttus.XAuthor.AppDesigner
{
    partial class MenuHome
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
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.cmbMenuSelector = new System.Windows.Forms.ComboBox();
            this.MnuTree = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProperties = new System.Windows.Forms.Label();
            this.cmboWf = new System.Windows.Forms.ComboBox();
            this.lblMenuDesigner = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.lblWF = new System.Windows.Forms.Label();
            this.chkShowAddRow = new System.Windows.Forms.CheckBox();
            this.ChkShowChatter = new System.Windows.Forms.CheckBox();
            this.ChkShowDeleteRow = new System.Windows.Forms.CheckBox();
            this.chkMatrixAddColumn = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDelete.Location = new System.Drawing.Point(201, 3);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnDelete.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(98, 27);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Tag = "btnRemove";
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCreate.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCreate.Location = new System.Drawing.Point(209, 3);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(50, 23);
            this.btnCreate.TabIndex = 8;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Visible = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // cmbMenuSelector
            // 
            this.cmbMenuSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMenuSelector.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbMenuSelector.FormattingEnabled = true;
            this.cmbMenuSelector.Location = new System.Drawing.Point(3, 3);
            this.cmbMenuSelector.Name = "cmbMenuSelector";
            this.cmbMenuSelector.Size = new System.Drawing.Size(200, 23);
            this.cmbMenuSelector.TabIndex = 7;
            this.cmbMenuSelector.Visible = false;
            this.cmbMenuSelector.SelectedIndexChanged += new System.EventHandler(this.cmbMenuSelector_SelectedIndexChanged);
            // 
            // MnuTree
            // 
            this.MnuTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MnuTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MnuTree.Location = new System.Drawing.Point(3, 55);
            this.MnuTree.Name = "MnuTree";
            this.MnuTree.Size = new System.Drawing.Size(299, 172);
            this.MnuTree.TabIndex = 10;
            this.MnuTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MnuTree_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 59);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(315, 133);
            this.propertyGrid1.TabIndex = 12;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSave.Location = new System.Drawing.Point(146, 10);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnSave.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 14;
            this.btnSave.Tag = "btnSave";
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(227, 10);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Tag = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProperties
            // 
            this.lblProperties.AutoSize = true;
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProperties.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblProperties.Location = new System.Drawing.Point(3, 0);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(309, 15);
            this.lblProperties.TabIndex = 17;
            this.lblProperties.Tag = "lblProperties";
            this.lblProperties.Text = "label1";
            // 
            // cmboWf
            // 
            this.cmboWf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmboWf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboWf.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmboWf.FormattingEnabled = true;
            this.cmboWf.Location = new System.Drawing.Point(46, 3);
            this.cmboWf.Name = "cmboWf";
            this.cmboWf.Size = new System.Drawing.Size(266, 23);
            this.cmboWf.TabIndex = 20;
            this.cmboWf.SelectedIndexChanged += new System.EventHandler(this.cmboWf_SelectedIndexChanged);
            // 
            // lblMenuDesigner
            // 
            this.lblMenuDesigner.AutoSize = true;
            this.lblMenuDesigner.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMenuDesigner.ForeColor = System.Drawing.Color.Green;
            this.lblMenuDesigner.Location = new System.Drawing.Point(3, 0);
            this.lblMenuDesigner.Name = "lblMenuDesigner";
            this.lblMenuDesigner.Size = new System.Drawing.Size(65, 30);
            this.lblMenuDesigner.TabIndex = 25;
            this.lblMenuDesigner.Tag = "TitleMenuDesigner";
            this.lblMenuDesigner.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 13);
            this.label1.TabIndex = 26;
            this.label1.Tag = "mnuDesignerAddLabel";
            this.label1.Text = "label1";
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.AutoSize = true;
            this.btnAddGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddGroup.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddGroup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddGroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAddGroup.Location = new System.Drawing.Point(0, 3);
            this.btnAddGroup.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnAddGroup.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(96, 27);
            this.btnAddGroup.TabIndex = 27;
            this.btnAddGroup.Tag = "btnAddGroup";
            this.btnAddGroup.Text = "Add Group";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // btnAddItem
            // 
            this.btnAddItem.AutoSize = true;
            this.btnAddItem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddItem.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddItem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAddItem.Location = new System.Drawing.Point(102, 3);
            this.btnAddItem.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(93, 27);
            this.btnAddItem.TabIndex = 28;
            this.btnAddItem.Tag = "btnAddButton";
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // lblWF
            // 
            this.lblWF.AutoSize = true;
            this.lblWF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWF.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblWF.Location = new System.Drawing.Point(3, 3);
            this.lblWF.Margin = new System.Windows.Forms.Padding(3);
            this.lblWF.Name = "lblWF";
            this.lblWF.Size = new System.Drawing.Size(37, 23);
            this.lblWF.TabIndex = 31;
            this.lblWF.Tag = "lblWorkflow";
            this.lblWF.Text = "label1";
            // 
            // chkShowAddRow
            // 
            this.chkShowAddRow.AutoSize = true;
            this.chkShowAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkShowAddRow.Location = new System.Drawing.Point(3, 0);
            this.chkShowAddRow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkShowAddRow.Name = "chkShowAddRow";
            this.chkShowAddRow.Size = new System.Drawing.Size(104, 17);
            this.chkShowAddRow.TabIndex = 32;
            this.chkShowAddRow.Text = "Display Add Row";
            this.chkShowAddRow.UseVisualStyleBackColor = true;
            // 
            // ChkShowChatter
            // 
            this.ChkShowChatter.AutoSize = true;
            this.ChkShowChatter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkShowChatter.Location = new System.Drawing.Point(126, 17);
            this.ChkShowChatter.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ChkShowChatter.Name = "ChkShowChatter";
            this.ChkShowChatter.Size = new System.Drawing.Size(94, 17);
            this.ChkShowChatter.TabIndex = 33;
            this.ChkShowChatter.Text = "Display Chatter";
            this.ChkShowChatter.UseVisualStyleBackColor = true;
            // 
            // ChkShowDeleteRow
            // 
            this.ChkShowDeleteRow.AutoSize = true;
            this.ChkShowDeleteRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkShowDeleteRow.Location = new System.Drawing.Point(126, 0);
            this.ChkShowDeleteRow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ChkShowDeleteRow.Name = "ChkShowDeleteRow";
            this.ChkShowDeleteRow.Size = new System.Drawing.Size(192, 17);
            this.ChkShowDeleteRow.TabIndex = 34;
            this.ChkShowDeleteRow.Text = "Display Delete Row from Salesforce";
            this.ChkShowDeleteRow.UseVisualStyleBackColor = true;
            // 
            // chkMatrixAddColumn
            // 
            this.chkMatrixAddColumn.AutoSize = true;
            this.chkMatrixAddColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkMatrixAddColumn.Location = new System.Drawing.Point(3, 17);
            this.chkMatrixAddColumn.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkMatrixAddColumn.Name = "chkMatrixAddColumn";
            this.chkMatrixAddColumn.Size = new System.Drawing.Size(117, 17);
            this.chkMatrixAddColumn.TabIndex = 36;
            this.chkMatrixAddColumn.Text = "Display Add Column";
            this.chkMatrixAddColumn.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel8, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 328);
            this.tableLayoutPanel1.TabIndex = 37;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 36);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(638, 236);
            this.tableLayoutPanel2.TabIndex = 38;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.MnuTree, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(305, 230);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Controls.Add(this.btnAddGroup, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnAddItem, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnDelete, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(299, 33);
            this.tableLayoutPanel4.TabIndex = 27;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.propertyGrid1, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(314, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(321, 230);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.lblProperties, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 38);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(315, 15);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.lblWF, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.cmboWf, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 198);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(315, 29);
            this.tableLayoutPanel7.TabIndex = 13;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.cmbMenuSelector);
            this.flowLayoutPanel3.Controls.Add(this.btnCreate);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(315, 29);
            this.flowLayoutPanel3.TabIndex = 14;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblMenuDesigner);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(638, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel9, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 278);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(638, 47);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.AutoSize = true;
            this.tableLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.Controls.Add(this.chkShowAddRow, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.ChkShowChatter, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.chkMatrixAddColumn, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.ChkShowDeleteRow, 1, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.Size = new System.Drawing.Size(321, 41);
            this.tableLayoutPanel9.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnSave);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(330, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(305, 41);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // MenuHome
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(644, 328);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuHome";
            this.Tag = "AppTitle";
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.MenuHome_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.ComboBox cmbMenuSelector;
        private System.Windows.Forms.TreeView MnuTree;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.ComboBox cmboWf;
        private System.Windows.Forms.Label lblMenuDesigner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Label lblWF;
        private System.Windows.Forms.CheckBox chkShowAddRow;
        private System.Windows.Forms.CheckBox ChkShowChatter;
        private System.Windows.Forms.CheckBox ChkShowDeleteRow;
        private System.Windows.Forms.CheckBox chkMatrixAddColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
    }
}