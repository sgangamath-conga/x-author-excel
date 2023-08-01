namespace Apttus.XAuthor.AppDesigner
{
    partial class MultiSelectListForm
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
            this.pnlSaveCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tplSearch = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tplListBoxes = new System.Windows.Forms.TableLayoutPanel();
            this.pnlShiftButtons = new System.Windows.Forms.Panel();
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
            this.tplMain.ColumnCount = 1;
            this.tplMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplMain.Controls.Add(this.pnlSaveCancel, 0, 3);
            this.tplMain.Controls.Add(this.tplSearch, 0, 1);
            this.tplMain.Controls.Add(this.lblTitle, 0, 0);
            this.tplMain.Controls.Add(this.tplListBoxes, 0, 2);
            this.tplMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tplMain.Location = new System.Drawing.Point(2, 2);
            this.tplMain.Margin = new System.Windows.Forms.Padding(0);
            this.tplMain.Name = "tplMain";
            this.tplMain.RowCount = 4;
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tplMain.Size = new System.Drawing.Size(471, 448);
            this.tplMain.TabIndex = 1;
            // 
            // pnlSaveCancel
            // 
            this.pnlSaveCancel.Controls.Add(this.btnCancel);
            this.pnlSaveCancel.Controls.Add(this.btnSave);
            this.pnlSaveCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSaveCancel.Location = new System.Drawing.Point(0, 421);
            this.pnlSaveCancel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSaveCancel.Name = "pnlSaveCancel";
            this.pnlSaveCancel.Size = new System.Drawing.Size(471, 27);
            this.pnlSaveCancel.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(380, 0);
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
            this.btnSave.Location = new System.Drawing.Point(285, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 25);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Add";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tplSearch
            // 
            this.tplSearch.ColumnCount = 4;
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tplSearch.Controls.Add(this.btnClear, 0, 0);
            this.tplSearch.Controls.Add(this.btnGo, 0, 0);
            this.tplSearch.Controls.Add(this.txtSearch, 0, 0);
            this.tplSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplSearch.Location = new System.Drawing.Point(0, 37);
            this.tplSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tplSearch.Name = "tplSearch";
            this.tplSearch.RowCount = 1;
            this.tplSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplSearch.Size = new System.Drawing.Size(471, 30);
            this.tplSearch.TabIndex = 5;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.AutoSize = true;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(278, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 24);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.AutoSize = true;
            this.btnGo.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnGo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGo.Location = new System.Drawing.Point(218, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(54, 24);
            this.btnGo.TabIndex = 6;
            this.btnGo.Text = "Search";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(5, 3);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(5, 3, 0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(210, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F);
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Location = new System.Drawing.Point(5, 1);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(83, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Objects";
            // 
            // tplListBoxes
            // 
            this.tplListBoxes.ColumnCount = 3;
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tplListBoxes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tplListBoxes.Controls.Add(this.pnlShiftButtons, 1, 0);
            this.tplListBoxes.Controls.Add(this.grpAvailableValues, 0, 0);
            this.tplListBoxes.Controls.Add(this.grpSelectedValues, 2, 0);
            this.tplListBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tplListBoxes.Location = new System.Drawing.Point(3, 70);
            this.tplListBoxes.Name = "tplListBoxes";
            this.tplListBoxes.RowCount = 1;
            this.tplListBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tplListBoxes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 348F));
            this.tplListBoxes.Size = new System.Drawing.Size(465, 348);
            this.tplListBoxes.TabIndex = 6;
            // 
            // pnlShiftButtons
            // 
            this.pnlShiftButtons.Controls.Add(this.btnSelect);
            this.pnlShiftButtons.Controls.Add(this.btnDeselect);
            this.pnlShiftButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlShiftButtons.Location = new System.Drawing.Point(212, 3);
            this.pnlShiftButtons.Name = "pnlShiftButtons";
            this.pnlShiftButtons.Size = new System.Drawing.Size(40, 342);
            this.pnlShiftButtons.TabIndex = 6;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.AutoSize = true;
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSelect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(0, 140);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(40, 27);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = ">>";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeselect
            // 
            this.btnDeselect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDeselect.AutoSize = true;
            this.btnDeselect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnDeselect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnDeselect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeselect.Location = new System.Drawing.Point(0, 176);
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.Size = new System.Drawing.Size(40, 27);
            this.btnDeselect.TabIndex = 5;
            this.btnDeselect.Text = "<<";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // grpAvailableValues
            // 
            this.grpAvailableValues.Controls.Add(this.lstSource);
            this.grpAvailableValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAvailableValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpAvailableValues.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAvailableValues.Location = new System.Drawing.Point(2, 1);
            this.grpAvailableValues.Margin = new System.Windows.Forms.Padding(2, 1, 1, 1);
            this.grpAvailableValues.Name = "grpAvailableValues";
            this.grpAvailableValues.Padding = new System.Windows.Forms.Padding(1);
            this.grpAvailableValues.Size = new System.Drawing.Size(206, 346);
            this.grpAvailableValues.TabIndex = 9;
            this.grpAvailableValues.TabStop = false;
            this.grpAvailableValues.Text = "Available Objects";
            // 
            // lstSource
            // 
            this.lstSource.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSource.FormattingEnabled = true;
            this.lstSource.ItemHeight = 15;
            this.lstSource.Location = new System.Drawing.Point(1, 17);
            this.lstSource.Name = "lstSource";
            this.lstSource.Size = new System.Drawing.Size(204, 328);
            this.lstSource.TabIndex = 8;
            this.lstSource.DoubleClick += new System.EventHandler(this.lstSource_DoubleClick);
            // 
            // grpSelectedValues
            // 
            this.grpSelectedValues.Controls.Add(this.lstSelected);
            this.grpSelectedValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectedValues.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpSelectedValues.Location = new System.Drawing.Point(256, 1);
            this.grpSelectedValues.Margin = new System.Windows.Forms.Padding(1, 1, 2, 1);
            this.grpSelectedValues.Name = "grpSelectedValues";
            this.grpSelectedValues.Padding = new System.Windows.Forms.Padding(1);
            this.grpSelectedValues.Size = new System.Drawing.Size(207, 346);
            this.grpSelectedValues.TabIndex = 10;
            this.grpSelectedValues.TabStop = false;
            this.grpSelectedValues.Text = "Objects To Add";
            // 
            // lstSelected
            // 
            this.lstSelected.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSelected.FormattingEnabled = true;
            this.lstSelected.ItemHeight = 15;
            this.lstSelected.Location = new System.Drawing.Point(1, 17);
            this.lstSelected.Margin = new System.Windows.Forms.Padding(0);
            this.lstSelected.Name = "lstSelected";
            this.lstSelected.Size = new System.Drawing.Size(205, 328);
            this.lstSelected.TabIndex = 9;
            this.lstSelected.DoubleClick += new System.EventHandler(this.lstSelected_DoubleClick);
            // 
            // MultiSelectListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(475, 452);
            this.Controls.Add(this.tplMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiSelectListForm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.tplMain.ResumeLayout(false);
            this.tplMain.PerformLayout();
            this.pnlSaveCancel.ResumeLayout(false);
            this.tplSearch.ResumeLayout(false);
            this.tplSearch.PerformLayout();
            this.tplListBoxes.ResumeLayout(false);
            this.pnlShiftButtons.ResumeLayout(false);
            this.pnlShiftButtons.PerformLayout();
            this.grpAvailableValues.ResumeLayout(false);
            this.grpSelectedValues.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tplMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tplSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TableLayoutPanel tplListBoxes;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Panel pnlShiftButtons;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDeselect;
        private System.Windows.Forms.GroupBox grpSelectedValues;
        private System.Windows.Forms.ListBox lstSelected;
        private System.Windows.Forms.GroupBox grpAvailableValues;
        private System.Windows.Forms.ListBox lstSource;
        private System.Windows.Forms.Panel pnlSaveCancel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
    }
}