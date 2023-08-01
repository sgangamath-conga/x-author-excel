namespace Apttus.XAuthor.AppDesigner
{
    partial class MatrixFieldView
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
            this.matrixPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.rboRow = new System.Windows.Forms.RadioButton();
            this.rboCol = new System.Windows.Forms.RadioButton();
            this.rboData = new System.Windows.Forms.RadioButton();
            this.tblMainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMatrixEntity = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.entityLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblObjectName = new System.Windows.Forms.Label();
            this.lblFieldName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tblMainPanel.SuspendLayout();
            this.pnlMatrixEntity.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.entityLayoutPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // matrixPropertyGrid
            // 
            this.matrixPropertyGrid.CommandsVisibleIfAvailable = false;
            this.matrixPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matrixPropertyGrid.HelpBackColor = System.Drawing.SystemColors.Window;
            this.matrixPropertyGrid.HelpVisible = false;
            this.matrixPropertyGrid.Location = new System.Drawing.Point(3, 105);
            this.matrixPropertyGrid.Name = "matrixPropertyGrid";
            this.matrixPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.matrixPropertyGrid.Size = new System.Drawing.Size(340, 147);
            this.matrixPropertyGrid.TabIndex = 0;
            this.matrixPropertyGrid.ToolbarVisible = false;
            // 
            // rboRow
            // 
            this.rboRow.AutoSize = true;
            this.rboRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rboRow.Location = new System.Drawing.Point(40, 3);
            this.rboRow.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.rboRow.Name = "rboRow";
            this.rboRow.Size = new System.Drawing.Size(47, 19);
            this.rboRow.TabIndex = 1;
            this.rboRow.Tag = "0";
            this.rboRow.Text = "Row";
            this.rboRow.UseVisualStyleBackColor = true;
            this.rboRow.CheckedChanged += new System.EventHandler(this.rboRow_CheckedChanged);
            // 
            // rboCol
            // 
            this.rboCol.AutoSize = true;
            this.rboCol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rboCol.Location = new System.Drawing.Point(130, 3);
            this.rboCol.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.rboCol.Name = "rboCol";
            this.rboCol.Size = new System.Drawing.Size(67, 19);
            this.rboCol.TabIndex = 2;
            this.rboCol.Tag = "1";
            this.rboCol.Text = "Column";
            this.rboCol.UseVisualStyleBackColor = true;
            this.rboCol.CheckedChanged += new System.EventHandler(this.rboCol_CheckedChanged);
            // 
            // rboData
            // 
            this.rboData.AutoSize = true;
            this.rboData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rboData.Location = new System.Drawing.Point(240, 3);
            this.rboData.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.rboData.Name = "rboData";
            this.rboData.Size = new System.Drawing.Size(48, 19);
            this.rboData.TabIndex = 3;
            this.rboData.Tag = "2";
            this.rboData.Text = "Data";
            this.rboData.UseVisualStyleBackColor = true;
            this.rboData.CheckedChanged += new System.EventHandler(this.rboData_CheckedChanged);
            // 
            // tblMainPanel
            // 
            this.tblMainPanel.AutoSize = true;
            this.tblMainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMainPanel.ColumnCount = 1;
            this.tblMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMainPanel.Controls.Add(this.matrixPropertyGrid, 0, 1);
            this.tblMainPanel.Controls.Add(this.pnlMatrixEntity, 0, 0);
            this.tblMainPanel.Controls.Add(this.panel2, 0, 2);
            this.tblMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMainPanel.Location = new System.Drawing.Point(0, 0);
            this.tblMainPanel.Name = "tblMainPanel";
            this.tblMainPanel.RowCount = 3;
            this.tblMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMainPanel.Size = new System.Drawing.Size(346, 294);
            this.tblMainPanel.TabIndex = 4;
            // 
            // pnlMatrixEntity
            // 
            this.pnlMatrixEntity.AutoSize = true;
            this.pnlMatrixEntity.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMatrixEntity.Controls.Add(this.tableLayoutPanel1);
            this.pnlMatrixEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMatrixEntity.Location = new System.Drawing.Point(3, 3);
            this.pnlMatrixEntity.Name = "pnlMatrixEntity";
            this.pnlMatrixEntity.Size = new System.Drawing.Size(340, 96);
            this.pnlMatrixEntity.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.entityLayoutPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(340, 96);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Create a Matrix Mapping";
            // 
            // entityLayoutPanel
            // 
            this.entityLayoutPanel.AutoSize = true;
            this.entityLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.entityLayoutPanel.Controls.Add(this.rboRow);
            this.entityLayoutPanel.Controls.Add(this.rboCol);
            this.entityLayoutPanel.Controls.Add(this.rboData);
            this.entityLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityLayoutPanel.Location = new System.Drawing.Point(3, 20);
            this.entityLayoutPanel.Name = "entityLayoutPanel";
            this.entityLayoutPanel.Size = new System.Drawing.Size(334, 25);
            this.entityLayoutPanel.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.lblObjectName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblFieldName, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 51);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(334, 42);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // lblObjectName
            // 
            this.lblObjectName.AutoSize = true;
            this.lblObjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectName.Location = new System.Drawing.Point(92, 3);
            this.lblObjectName.Margin = new System.Windows.Forms.Padding(3);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(239, 15);
            this.lblObjectName.TabIndex = 6;
            this.lblObjectName.UseMnemonic = false;
            // 
            // lblFieldName
            // 
            this.lblFieldName.AutoSize = true;
            this.lblFieldName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFieldName.Location = new System.Drawing.Point(92, 24);
            this.lblFieldName.Margin = new System.Windows.Forms.Padding(3);
            this.lblFieldName.Name = "lblFieldName";
            this.lblFieldName.Size = new System.Drawing.Size(239, 15);
            this.lblFieldName.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Object Name :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Field Name :";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel2.Location = new System.Drawing.Point(3, 258);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(340, 33);
            this.panel2.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(262, 3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(181, 3);
            this.btnOK.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // MatrixFieldView
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(346, 294);
            this.Controls.Add(this.tblMainPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MatrixFieldView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Matrix ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tblMainPanel.ResumeLayout(false);
            this.tblMainPanel.PerformLayout();
            this.pnlMatrixEntity.ResumeLayout(false);
            this.pnlMatrixEntity.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.entityLayoutPanel.ResumeLayout(false);
            this.entityLayoutPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid matrixPropertyGrid;
        private System.Windows.Forms.RadioButton rboRow;
        private System.Windows.Forms.RadioButton rboCol;
        private System.Windows.Forms.RadioButton rboData;
        private System.Windows.Forms.TableLayoutPanel tblMainPanel;
        private System.Windows.Forms.Panel pnlMatrixEntity;
        private System.Windows.Forms.Label lblObjectName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFieldName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel entityLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}

