namespace Apttus.XAuthor.AppDesigner.Forms
{
    partial class ApplicationDefinition
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
            this.tlpApplicationDefinition = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFields = new System.Windows.Forms.TableLayoutPanel();
            this.apttusFieldsView = new Apttus.XAuthor.AppDesigner.ApttusFieldsView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFields = new System.Windows.Forms.Label();
            this.lblFilterFieldsGrid = new System.Windows.Forms.Label();
            this.lblObjectMetadata = new System.Windows.Forms.Label();
            this.lblFieldsGrid = new System.Windows.Forms.Label();
            this.pnlObjectSelection = new System.Windows.Forms.TableLayoutPanel();
            this.grpHierarchical = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddParentObject = new System.Windows.Forms.Button();
            this.btnAddObject = new System.Windows.Forms.Button();
            this.btnRemoveObject = new System.Windows.Forms.Button();
            this.tvObjects = new System.Windows.Forms.TreeView();
            this.pnlSelectedObjects = new System.Windows.Forms.TableLayoutPanel();
            this.tvSelectedObjects = new System.Windows.Forms.TreeView();
            this.lblSelectedObjects = new System.Windows.Forms.Label();
            this.lblObjectSelection = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLegend = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveAndClose = new System.Windows.Forms.Button();
            this.btnSaveFields = new System.Windows.Forms.Button();
            this.grpCrossTab = new System.Windows.Forms.GroupBox();
            this.txtCrossTabName = new System.Windows.Forms.TextBox();
            this.lblCrossTabName = new System.Windows.Forms.Label();
            this.btnAddCrossTab = new System.Windows.Forms.Button();
            this.lblData = new System.Windows.Forms.Label();
            this.lstData = new System.Windows.Forms.ListBox();
            this.lblRH = new System.Windows.Forms.Label();
            this.cmboColHeader = new System.Windows.Forms.ComboBox();
            this.lblCH = new System.Windows.Forms.Label();
            this.cmboRowHeader = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tlpApplicationDefinition.SuspendLayout();
            this.pnlFields.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pnlObjectSelection.SuspendLayout();
            this.grpHierarchical.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.pnlSelectedObjects.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.grpCrossTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpApplicationDefinition
            // 
            this.tlpApplicationDefinition.AutoSize = true;
            this.tlpApplicationDefinition.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpApplicationDefinition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tlpApplicationDefinition.ColumnCount = 1;
            this.tlpApplicationDefinition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpApplicationDefinition.Controls.Add(this.pnlFields, 0, 2);
            this.tlpApplicationDefinition.Controls.Add(this.pnlObjectSelection, 0, 1);
            this.tlpApplicationDefinition.Controls.Add(this.pnlHeader, 0, 0);
            this.tlpApplicationDefinition.Controls.Add(this.tableLayoutPanel1, 0, 3);
            this.tlpApplicationDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpApplicationDefinition.Location = new System.Drawing.Point(0, 0);
            this.tlpApplicationDefinition.Name = "tlpApplicationDefinition";
            this.tlpApplicationDefinition.RowCount = 4;
            this.tlpApplicationDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpApplicationDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpApplicationDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpApplicationDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpApplicationDefinition.Size = new System.Drawing.Size(717, 658);
            this.tlpApplicationDefinition.TabIndex = 0;
            // 
            // pnlFields
            // 
            this.pnlFields.AutoSize = true;
            this.pnlFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlFields.ColumnCount = 1;
            this.tlpApplicationDefinition.SetColumnSpan(this.pnlFields, 2);
            this.pnlFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlFields.Controls.Add(this.apttusFieldsView, 0, 1);
            this.pnlFields.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.pnlFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFields.Location = new System.Drawing.Point(3, 332);
            this.pnlFields.Name = "pnlFields";
            this.pnlFields.RowCount = 2;
            this.pnlFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFields.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlFields.Size = new System.Drawing.Size(743, 325);
            this.pnlFields.TabIndex = 7;
            // 
            // apttusFieldsView
            // 
            this.apttusFieldsView.AppObject = null;
            this.apttusFieldsView.AutoSize = true;
            this.apttusFieldsView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.apttusFieldsView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.apttusFieldsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apttusFieldsView.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.apttusFieldsView.Location = new System.Drawing.Point(3, 31);
            this.apttusFieldsView.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.apttusFieldsView.Name = "apttusFieldsView";
            this.apttusFieldsView.Size = new System.Drawing.Size(735, 291);
            this.apttusFieldsView.TabIndex = 14;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lblFields, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblFilterFieldsGrid, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblObjectMetadata, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblFieldsGrid, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(743, 28);
            this.tableLayoutPanel2.TabIndex = 15;
            // 
            // lblFields
            // 
            this.lblFields.AutoSize = true;
            this.lblFields.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lblFields.Location = new System.Drawing.Point(5, 0);
            this.lblFields.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblFields.Name = "lblFields";
            this.lblFields.Size = new System.Drawing.Size(124, 28);
            this.lblFields.TabIndex = 11;
            this.lblFields.Text = "Select Fields";
            // 
            // lblFilterFieldsGrid
            // 
            this.lblFilterFieldsGrid.AutoSize = true;
            this.lblFilterFieldsGrid.Location = new System.Drawing.Point(384, 0);
            this.lblFilterFieldsGrid.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblFilterFieldsGrid.Name = "lblFilterFieldsGrid";
            this.lblFilterFieldsGrid.Size = new System.Drawing.Size(54, 25);
            this.lblFilterFieldsGrid.TabIndex = 1;
            this.lblFilterFieldsGrid.Text = "Filter:";
            this.lblFilterFieldsGrid.Visible = false;
            // 
            // lblObjectMetadata
            // 
            this.lblObjectMetadata.AutoSize = true;
            this.lblObjectMetadata.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectMetadata.Location = new System.Drawing.Point(355, 0);
            this.lblObjectMetadata.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblObjectMetadata.Name = "lblObjectMetadata";
            this.lblObjectMetadata.Size = new System.Drawing.Size(19, 25);
            this.lblObjectMetadata.TabIndex = 12;
            this.lblObjectMetadata.Text = "-";
            // 
            // lblFieldsGrid
            // 
            this.lblFieldsGrid.AutoSize = true;
            this.lblFieldsGrid.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.lblFieldsGrid.Location = new System.Drawing.Point(139, 0);
            this.lblFieldsGrid.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblFieldsGrid.Name = "lblFieldsGrid";
            this.lblFieldsGrid.Size = new System.Drawing.Size(206, 25);
            this.lblFieldsGrid.TabIndex = 2;
            this.lblFieldsGrid.Text = "Fields for Selected Object";
            // 
            // pnlObjectSelection
            // 
            this.pnlObjectSelection.AutoSize = true;
            this.pnlObjectSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlObjectSelection.ColumnCount = 2;
            this.pnlObjectSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlObjectSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlObjectSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlObjectSelection.Controls.Add(this.grpHierarchical, 0, 1);
            this.pnlObjectSelection.Controls.Add(this.pnlSelectedObjects, 0, 1);
            this.pnlObjectSelection.Controls.Add(this.lblSelectedObjects, 1, 0);
            this.pnlObjectSelection.Controls.Add(this.lblObjectSelection, 0, 0);
            this.pnlObjectSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlObjectSelection.Location = new System.Drawing.Point(0, 45);
            this.pnlObjectSelection.Margin = new System.Windows.Forms.Padding(0);
            this.pnlObjectSelection.Name = "pnlObjectSelection";
            this.pnlObjectSelection.RowCount = 2;
            this.pnlObjectSelection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlObjectSelection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlObjectSelection.Size = new System.Drawing.Size(749, 284);
            this.pnlObjectSelection.TabIndex = 4;
            // 
            // grpHierarchical
            // 
            this.grpHierarchical.AutoSize = true;
            this.grpHierarchical.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpHierarchical.ColumnCount = 2;
            this.grpHierarchical.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.grpHierarchical.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.grpHierarchical.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.grpHierarchical.Controls.Add(this.tvObjects, 0, 1);
            this.grpHierarchical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHierarchical.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.grpHierarchical.Location = new System.Drawing.Point(3, 31);
            this.grpHierarchical.Name = "grpHierarchical";
            this.grpHierarchical.RowCount = 1;
            this.grpHierarchical.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.grpHierarchical.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.grpHierarchical.Size = new System.Drawing.Size(465, 250);
            this.grpHierarchical.TabIndex = 16;
            this.grpHierarchical.Text = "Objects Hierarchy";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnAddParentObject);
            this.flowLayoutPanel2.Controls.Add(this.btnAddObject);
            this.flowLayoutPanel2.Controls.Add(this.btnRemoveObject);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(275, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(5, 55, 2, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(190, 250);
            this.flowLayoutPanel2.TabIndex = 17;
            // 
            // btnAddParentObject
            // 
            this.btnAddParentObject.AutoSize = true;
            this.btnAddParentObject.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddParentObject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddParentObject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddParentObject.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnAddParentObject.Location = new System.Drawing.Point(8, 58);
            this.btnAddParentObject.Name = "btnAddParentObject";
            this.btnAddParentObject.Size = new System.Drawing.Size(177, 40);
            this.btnAddParentObject.TabIndex = 13;
            this.btnAddParentObject.Text = "Add Parent Object *";
            this.btnAddParentObject.UseVisualStyleBackColor = true;
            this.btnAddParentObject.Click += new System.EventHandler(this.btnAddParentObject_Click);
            // 
            // btnAddObject
            // 
            this.btnAddObject.AutoSize = true;
            this.btnAddObject.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAddObject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnAddObject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddObject.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnAddObject.Location = new System.Drawing.Point(8, 104);
            this.btnAddObject.Name = "btnAddObject";
            this.btnAddObject.Size = new System.Drawing.Size(167, 40);
            this.btnAddObject.TabIndex = 14;
            this.btnAddObject.Text = "Add Child Object *";
            this.btnAddObject.UseVisualStyleBackColor = true;
            this.btnAddObject.Click += new System.EventHandler(this.btnAddObject_Click);
            // 
            // btnRemoveObject
            // 
            this.btnRemoveObject.AutoSize = true;
            this.btnRemoveObject.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRemoveObject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnRemoveObject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveObject.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnRemoveObject.Location = new System.Drawing.Point(8, 150);
            this.btnRemoveObject.Name = "btnRemoveObject";
            this.btnRemoveObject.Size = new System.Drawing.Size(152, 40);
            this.btnRemoveObject.TabIndex = 15;
            this.btnRemoveObject.Text = "Remove Object *";
            this.btnRemoveObject.UseVisualStyleBackColor = true;
            this.btnRemoveObject.Click += new System.EventHandler(this.btnRemoveObject_Click);
            // 
            // tvObjects
            // 
            this.tvObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjects.Location = new System.Drawing.Point(5, 0);
            this.tvObjects.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.tvObjects.MaximumSize = new System.Drawing.Size(270, 250);
            this.tvObjects.MinimumSize = new System.Drawing.Size(270, 250);
            this.tvObjects.Name = "tvObjects";
            this.tvObjects.ShowNodeToolTips = true;
            this.tvObjects.Size = new System.Drawing.Size(270, 250);
            this.tvObjects.TabIndex = 16;
            this.tvObjects.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvObjects_AfterExpand);
            // 
            // pnlSelectedObjects
            // 
            this.pnlSelectedObjects.AutoSize = true;
            this.pnlSelectedObjects.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSelectedObjects.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlSelectedObjects.Controls.Add(this.tvSelectedObjects, 0, 0);
            this.pnlSelectedObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSelectedObjects.Location = new System.Drawing.Point(471, 28);
            this.pnlSelectedObjects.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSelectedObjects.Name = "pnlSelectedObjects";
            this.pnlSelectedObjects.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlSelectedObjects.Size = new System.Drawing.Size(278, 256);
            this.pnlSelectedObjects.TabIndex = 6;
            // 
            // tvSelectedObjects
            // 
            this.tvSelectedObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSelectedObjects.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.tvSelectedObjects.Location = new System.Drawing.Point(3, 3);
            this.tvSelectedObjects.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.tvSelectedObjects.MaximumSize = new System.Drawing.Size(270, 250);
            this.tvSelectedObjects.MinimumSize = new System.Drawing.Size(270, 250);
            this.tvSelectedObjects.Name = "tvSelectedObjects";
            this.tvSelectedObjects.ShowNodeToolTips = true;
            this.tvSelectedObjects.Size = new System.Drawing.Size(270, 250);
            this.tvSelectedObjects.TabIndex = 4;
            this.tvSelectedObjects.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvSelectedObjects_MouseUp);
            // 
            // lblSelectedObjects
            // 
            this.lblSelectedObjects.AutoSize = true;
            this.lblSelectedObjects.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lblSelectedObjects.Location = new System.Drawing.Point(476, 0);
            this.lblSelectedObjects.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSelectedObjects.Name = "lblSelectedObjects";
            this.lblSelectedObjects.Size = new System.Drawing.Size(163, 28);
            this.lblSelectedObjects.TabIndex = 2;
            this.lblSelectedObjects.Text = "Selected Objects";
            // 
            // lblObjectSelection
            // 
            this.lblObjectSelection.AutoSize = true;
            this.lblObjectSelection.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.lblObjectSelection.Location = new System.Drawing.Point(5, 0);
            this.lblObjectSelection.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblObjectSelection.Name = "lblObjectSelection";
            this.lblObjectSelection.Size = new System.Drawing.Size(140, 28);
            this.lblObjectSelection.TabIndex = 11;
            this.lblObjectSelection.Text = "Select Objects";
            // 
            // pnlHeader
            // 
            this.pnlHeader.AutoSize = true;
            this.pnlHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpApplicationDefinition.SetColumnSpan(this.pnlHeader, 2);
            this.pnlHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlHeader.Controls.Add(this.lblHeader, 0, 0);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlHeader.Size = new System.Drawing.Size(749, 45);
            this.pnlHeader.TabIndex = 8;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Green;
            this.lblHeader.Location = new System.Drawing.Point(5, 0);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(267, 45);
            this.lblHeader.TabIndex = 11;
            this.lblHeader.Text = "Salesforce Objects";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblLegend, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 663);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(743, 49);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // lblLegend
            // 
            this.lblLegend.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLegend.AutoSize = true;
            this.lblLegend.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLegend.Location = new System.Drawing.Point(3, 13);
            this.lblLegend.Name = "lblLegend";
            this.lblLegend.Size = new System.Drawing.Size(222, 23);
            this.lblLegend.TabIndex = 13;
            this.lblLegend.Text = "* operations are auto-saved";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveAndClose);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveFields);
            this.flowLayoutPanel1.Controls.Add(this.grpCrossTab);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(231, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(509, 43);
            this.flowLayoutPanel1.TabIndex = 14;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnCancel.Location = new System.Drawing.Point(421, 3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(85, 27);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 37);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveAndClose
            // 
            this.btnSaveAndClose.AutoSize = true;
            this.btnSaveAndClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveAndClose.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSaveAndClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSaveAndClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAndClose.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnSaveAndClose.Location = new System.Drawing.Point(276, 3);
            this.btnSaveAndClose.MinimumSize = new System.Drawing.Size(125, 27);
            this.btnSaveAndClose.Name = "btnSaveAndClose";
            this.btnSaveAndClose.Size = new System.Drawing.Size(139, 37);
            this.btnSaveAndClose.TabIndex = 8;
            this.btnSaveAndClose.Text = "Save and Close";
            this.btnSaveAndClose.UseVisualStyleBackColor = true;
            this.btnSaveAndClose.Click += new System.EventHandler(this.btnSaveAndClose_Click);
            // 
            // btnSaveFields
            // 
            this.btnSaveFields.AutoSize = true;
            this.btnSaveFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveFields.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSaveFields.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSaveFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveFields.Font = new System.Drawing.Font("Segoe UI Semilight", 9F);
            this.btnSaveFields.Location = new System.Drawing.Point(185, 3);
            this.btnSaveFields.MinimumSize = new System.Drawing.Size(85, 27);
            this.btnSaveFields.Name = "btnSaveFields";
            this.btnSaveFields.Size = new System.Drawing.Size(85, 37);
            this.btnSaveFields.TabIndex = 9;
            this.btnSaveFields.Text = "Apply";
            this.btnSaveFields.UseVisualStyleBackColor = true;
            this.btnSaveFields.Click += new System.EventHandler(this.btnSaveFields_Click);
            // 
            // grpCrossTab
            // 
            this.grpCrossTab.BackColor = System.Drawing.SystemColors.ControlText;
            this.grpCrossTab.Controls.Add(this.txtCrossTabName);
            this.grpCrossTab.Controls.Add(this.lblCrossTabName);
            this.grpCrossTab.Controls.Add(this.btnAddCrossTab);
            this.grpCrossTab.Controls.Add(this.lblData);
            this.grpCrossTab.Controls.Add(this.lstData);
            this.grpCrossTab.Controls.Add(this.lblRH);
            this.grpCrossTab.Controls.Add(this.cmboColHeader);
            this.grpCrossTab.Controls.Add(this.lblCH);
            this.grpCrossTab.Controls.Add(this.cmboRowHeader);
            this.grpCrossTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpCrossTab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCrossTab.Location = new System.Drawing.Point(147, 3);
            this.grpCrossTab.Name = "grpCrossTab";
            this.grpCrossTab.Size = new System.Drawing.Size(32, 18);
            this.grpCrossTab.TabIndex = 20;
            this.grpCrossTab.TabStop = false;
            this.grpCrossTab.Text = "Cross Tab ";
            // 
            // txtCrossTabName
            // 
            this.txtCrossTabName.Location = new System.Drawing.Point(102, 20);
            this.txtCrossTabName.Name = "txtCrossTabName";
            this.txtCrossTabName.Size = new System.Drawing.Size(192, 31);
            this.txtCrossTabName.TabIndex = 16;
            this.txtCrossTabName.Validating += new System.ComponentModel.CancelEventHandler(this.txtCrossTabName_Validating);
            // 
            // lblCrossTabName
            // 
            this.lblCrossTabName.AutoSize = true;
            this.lblCrossTabName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrossTabName.Location = new System.Drawing.Point(7, 23);
            this.lblCrossTabName.Name = "lblCrossTabName";
            this.lblCrossTabName.Size = new System.Drawing.Size(63, 25);
            this.lblCrossTabName.TabIndex = 15;
            this.lblCrossTabName.Text = "Name:";
            // 
            // btnAddCrossTab
            // 
            this.btnAddCrossTab.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCrossTab.Location = new System.Drawing.Point(314, 18);
            this.btnAddCrossTab.Name = "btnAddCrossTab";
            this.btnAddCrossTab.Size = new System.Drawing.Size(110, 27);
            this.btnAddCrossTab.TabIndex = 14;
            this.btnAddCrossTab.Text = "Add Cross Tab *";
            this.btnAddCrossTab.UseVisualStyleBackColor = true;
            // 
            // lblData
            // 
            this.lblData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.Location = new System.Drawing.Point(7, 110);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(87, 30);
            this.lblData.TabIndex = 13;
            this.lblData.Text = "Transaction:";
            // 
            // lstData
            // 
            this.lstData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstData.FormattingEnabled = true;
            this.lstData.ItemHeight = 25;
            this.lstData.Location = new System.Drawing.Point(102, 110);
            this.lstData.Name = "lstData";
            this.lstData.Size = new System.Drawing.Size(192, 104);
            this.lstData.TabIndex = 12;
            // 
            // lblRH
            // 
            this.lblRH.AutoSize = true;
            this.lblRH.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRH.Location = new System.Drawing.Point(7, 53);
            this.lblRH.Name = "lblRH";
            this.lblRH.Size = new System.Drawing.Size(112, 25);
            this.lblRH.TabIndex = 11;
            this.lblRH.Text = "Row Header:";
            // 
            // cmboColHeader
            // 
            this.cmboColHeader.Location = new System.Drawing.Point(0, 0);
            this.cmboColHeader.Name = "cmboColHeader";
            this.cmboColHeader.Size = new System.Drawing.Size(121, 33);
            this.cmboColHeader.TabIndex = 17;
            // 
            // lblCH
            // 
            this.lblCH.AutoSize = true;
            this.lblCH.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCH.Location = new System.Drawing.Point(6, 81);
            this.lblCH.Name = "lblCH";
            this.lblCH.Size = new System.Drawing.Size(140, 25);
            this.lblCH.TabIndex = 9;
            this.lblCH.Text = "Column Header:";
            // 
            // cmboRowHeader
            // 
            this.cmboRowHeader.Location = new System.Drawing.Point(0, 0);
            this.cmboRowHeader.Name = "cmboRowHeader";
            this.cmboRowHeader.Size = new System.Drawing.Size(121, 33);
            this.cmboRowHeader.TabIndex = 18;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ApplicationDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(717, 658);
            this.Controls.Add(this.tlpApplicationDefinition);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationDefinition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ApplicationDefinition_Load);
            this.tlpApplicationDefinition.ResumeLayout(false);
            this.tlpApplicationDefinition.PerformLayout();
            this.pnlFields.ResumeLayout(false);
            this.pnlFields.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pnlObjectSelection.ResumeLayout(false);
            this.pnlObjectSelection.PerformLayout();
            this.grpHierarchical.ResumeLayout(false);
            this.grpHierarchical.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.pnlSelectedObjects.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.grpCrossTab.ResumeLayout(false);
            this.grpCrossTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpApplicationDefinition;
        private System.Windows.Forms.Button btnSaveFields;
        private System.Windows.Forms.Label lblFieldsGrid;
        private System.Windows.Forms.Label lblFilterFieldsGrid;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSaveAndClose;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.Label lblObjectSelection;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblObjectMetadata;
        private System.Windows.Forms.Label lblLegend;
        private System.Windows.Forms.TreeView tvObjects;
        private System.Windows.Forms.Button btnAddParentObject;
        private System.Windows.Forms.Button btnAddObject;
        private System.Windows.Forms.GroupBox grpCrossTab;
        private System.Windows.Forms.TextBox txtCrossTabName;
        private System.Windows.Forms.Label lblCrossTabName;
        private System.Windows.Forms.Button btnAddCrossTab;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.ListBox lstData;
        private System.Windows.Forms.Label lblRH;
        private System.Windows.Forms.ComboBox cmboColHeader;
        private System.Windows.Forms.Label lblCH;
        private System.Windows.Forms.ComboBox cmboRowHeader;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private ApttusFieldsView apttusFieldsView;
        private System.Windows.Forms.TreeView tvSelectedObjects;
        private System.Windows.Forms.Button btnRemoveObject;
        private System.Windows.Forms.Label lblSelectedObjects;
        private System.Windows.Forms.TableLayoutPanel pnlHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel pnlObjectSelection;
        private System.Windows.Forms.TableLayoutPanel grpHierarchical;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel pnlSelectedObjects;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel pnlFields;

    }
}