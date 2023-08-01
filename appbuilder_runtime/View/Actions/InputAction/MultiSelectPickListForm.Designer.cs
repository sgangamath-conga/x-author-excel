namespace Apttus.XAuthor.AppRuntime
{
    partial class MultiSelectPickListForm
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
            this.pnlSaveCancel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tplSearch = new System.Windows.Forms.TableLayoutPanel();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tplListBoxes = new System.Windows.Forms.TableLayoutPanel();
            this.pnlShiftButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDeselect = new System.Windows.Forms.Button();
            this.grpAvailableValues = new System.Windows.Forms.GroupBox();
            this.lstSource = new System.Windows.Forms.ListBox();
            this.grpSelectedValues = new System.Windows.Forms.GroupBox();
            this.lstSelected = new System.Windows.Forms.ListBox();
            this.tplMain.SuspendLayout();
            this.pnlSaveCancel.SuspendLayout();
            this.tplSearch.SuspendLayout();
            this.tplListBoxes.SuspendLayout();
            this.pnlShiftButtons.SuspendLayout();
            this.grpAvailableValues.SuspendLayout();
            this.grpSelectedValues.SuspendLayout();
            this.SuspendLayout();
            // 
            // tplMain
            // 
            this.tplMain.AutoSize = true;
            this.tplMain.ColumnCount = 1;
            this.tplMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplMain.Controls.Add(this.pnlSaveCancel, 0, 3);
            this.tplMain.Controls.Add(this.tplSearch, 0, 1);
            this.tplMain.Controls.Add(this.lblTitle, 0, 0);
            this.tplMain.Controls.Add(this.tplListBoxes, 0, 2);
            this.tplMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tplMain.Location = new System.Drawing.Point(2, 2);
            this.tplMain.Name = "tplMain";
            this.tplMain.RowCount = 4;
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplMain.Size = new System.Drawing.Size(463, 462);
            this.tplMain.TabIndex = 1;
            // 
            // pnlSaveCancel
            // 
            this.pnlSaveCancel.AutoSize = true;
            this.pnlSaveCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSaveCancel.Controls.Add(this.btnCancel);
            this.pnlSaveCancel.Controls.Add(this.btnSave);
            this.pnlSaveCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaveCancel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlSaveCancel.Location = new System.Drawing.Point(0, 430);
            this.pnlSaveCancel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSaveCancel.Name = "pnlSaveCancel";
            this.pnlSaveCancel.Size = new System.Drawing.Size(463, 32);
            this.pnlSaveCancel.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(373, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 25);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(280, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 25);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tplSearch
            // 
            this.tplSearch.AutoSize = true;
            this.tplSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tplSearch.ColumnCount = 3;
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.Controls.Add(this.btnGo, 0, 0);
            this.tplSearch.Controls.Add(this.txtSearch, 0, 0);
            this.tplSearch.Controls.Add(this.btnClear, 2, 0);
            this.tplSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplSearch.Location = new System.Drawing.Point(3, 43);
            this.tplSearch.Name = "tplSearch";
            this.tplSearch.RowCount = 1;
            this.tplSearch.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplSearch.Size = new System.Drawing.Size(457, 33);
            this.tplSearch.TabIndex = 5;
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.AutoSize = true;
            this.btnGo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGo.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnGo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGo.Location = new System.Drawing.Point(219, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(34, 27);
            this.btnGo.TabIndex = 6;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 5);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(210, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(259, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(46, 27);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
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
            this.lblTitle.Size = new System.Drawing.Size(192, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Multi-Select Picklist";
            // 
            // tplListBoxes
            // 
            this.tplListBoxes.AutoSize = true;
            this.tplListBoxes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tplListBoxes.ColumnCount = 3;
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplListBoxes.Controls.Add(this.pnlShiftButtons, 1, 0);
            this.tplListBoxes.Controls.Add(this.grpAvailableValues, 0, 0);
            this.tplListBoxes.Controls.Add(this.grpSelectedValues, 2, 0);
            this.tplListBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplListBoxes.Location = new System.Drawing.Point(3, 82);
            this.tplListBoxes.Name = "tplListBoxes";
            this.tplListBoxes.RowCount = 1;
            this.tplListBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tplListBoxes.Size = new System.Drawing.Size(457, 345);
            this.tplListBoxes.TabIndex = 6;
            // 
            // pnlShiftButtons
            // 
            this.pnlShiftButtons.AutoSize = true;
            this.pnlShiftButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlShiftButtons.Controls.Add(this.btnSelect);
            this.pnlShiftButtons.Controls.Add(this.btnDeselect);
            this.pnlShiftButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShiftButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlShiftButtons.Location = new System.Drawing.Point(206, 3);
            this.pnlShiftButtons.Name = "pnlShiftButtons";
            this.pnlShiftButtons.Padding = new System.Windows.Forms.Padding(0, 120, 0, 0);
            this.pnlShiftButtons.Size = new System.Drawing.Size(46, 339);
            this.pnlShiftButtons.TabIndex = 6;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(3, 123);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(40, 25);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = ">>";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeselect
            // 
            this.btnDeselect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDeselect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnDeselect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnDeselect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeselect.Location = new System.Drawing.Point(3, 154);
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.Size = new System.Drawing.Size(40, 25);
            this.btnDeselect.TabIndex = 5;
            this.btnDeselect.Text = "<<";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // grpAvailableValues
            // 
            this.grpAvailableValues.AutoSize = true;
            this.grpAvailableValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpAvailableValues.Controls.Add(this.lstSource);
            this.grpAvailableValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAvailableValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpAvailableValues.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAvailableValues.Location = new System.Drawing.Point(3, 3);
            this.grpAvailableValues.Name = "grpAvailableValues";
            this.grpAvailableValues.Size = new System.Drawing.Size(197, 339);
            this.grpAvailableValues.TabIndex = 9;
            this.grpAvailableValues.TabStop = false;
            this.grpAvailableValues.Text = "Available Values";
            // 
            // lstSource
            // 
            this.lstSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSource.FormattingEnabled = true;
            this.lstSource.ItemHeight = 15;
            this.lstSource.Location = new System.Drawing.Point(1, 17);
            this.lstSource.Name = "lstSource";
            this.lstSource.Size = new System.Drawing.Size(190, 300);
            this.lstSource.TabIndex = 8;
            // 
            // grpSelectedValues
            // 
            this.grpSelectedValues.AutoSize = true;
            this.grpSelectedValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpSelectedValues.Controls.Add(this.lstSelected);
            this.grpSelectedValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectedValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpSelectedValues.Location = new System.Drawing.Point(258, 3);
            this.grpSelectedValues.Name = "grpSelectedValues";
            this.grpSelectedValues.Size = new System.Drawing.Size(196, 339);
            this.grpSelectedValues.TabIndex = 10;
            this.grpSelectedValues.TabStop = false;
            this.grpSelectedValues.Text = "Selected Values";
            // 
            // lstSelected
            // 
            this.lstSelected.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSelected.FormattingEnabled = true;
            this.lstSelected.ItemHeight = 15;
            this.lstSelected.Location = new System.Drawing.Point(3, 19);
            this.lstSelected.Margin = new System.Windows.Forms.Padding(0);
            this.lstSelected.Name = "lstSelected";
            this.lstSelected.Size = new System.Drawing.Size(190, 317);
            this.lstSelected.TabIndex = 9;
            // 
            // MultiSelectPickListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(467, 466);
            this.Controls.Add(this.tplMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiSelectPickListForm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.tplMain.ResumeLayout(false);
            this.tplMain.PerformLayout();
            this.pnlSaveCancel.ResumeLayout(false);
            this.tplSearch.ResumeLayout(false);
            this.tplSearch.PerformLayout();
            this.tplListBoxes.ResumeLayout(false);
            this.tplListBoxes.PerformLayout();
            this.pnlShiftButtons.ResumeLayout(false);
            this.grpAvailableValues.ResumeLayout(false);
            this.grpSelectedValues.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tplMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tplSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TableLayoutPanel tplListBoxes;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.FlowLayoutPanel pnlShiftButtons;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDeselect;
        private System.Windows.Forms.GroupBox grpSelectedValues;
        private System.Windows.Forms.ListBox lstSelected;
        private System.Windows.Forms.GroupBox grpAvailableValues;
        private System.Windows.Forms.ListBox lstSource;
        private System.Windows.Forms.FlowLayoutPanel pnlSaveCancel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
    }
}