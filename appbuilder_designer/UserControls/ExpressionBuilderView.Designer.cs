namespace Apttus.XAuthor.AppDesigner
{
    partial class ExpressionBuilderView
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
            this.pnlExpressionBuilder = new System.Windows.Forms.TableLayoutPanel();
            this.tblExpressions = new System.Windows.Forms.TableLayoutPanel();
            this.lblObjectAndFields = new System.Windows.Forms.Label();
            this.lblValueType = new System.Windows.Forms.Label();
            this.lblOperator = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.pnlOptions = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFilterLogic = new System.Windows.Forms.FlowLayoutPanel();
            this.txtFilterLogic = new System.Windows.Forms.TextBox();
            this.lblFilterLogic = new System.Windows.Forms.Label();
            this.lnkClearAll = new System.Windows.Forms.LinkLabel();
            this.lnkAddRemoveFilterLogic = new System.Windows.Forms.LinkLabel();
            this.lnkAddRow = new System.Windows.Forms.LinkLabel();
            this.FilterByPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFilterBy = new System.Windows.Forms.Label();
            this.pnlExpressionBuilder.SuspendLayout();
            this.tblExpressions.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.pnlFilterLogic.SuspendLayout();
            this.FilterByPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlExpressionBuilder
            // 
            this.pnlExpressionBuilder.AutoSize = true;
            this.pnlExpressionBuilder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlExpressionBuilder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlExpressionBuilder.Controls.Add(this.tblExpressions, 0, 1);
            this.pnlExpressionBuilder.Controls.Add(this.pnlOptions, 0, 2);
            this.pnlExpressionBuilder.Controls.Add(this.FilterByPanel, 0, 0);
            this.pnlExpressionBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlExpressionBuilder.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlExpressionBuilder.Location = new System.Drawing.Point(0, 0);
            this.pnlExpressionBuilder.Name = "pnlExpressionBuilder";
            this.pnlExpressionBuilder.RowCount = 3;
            this.pnlExpressionBuilder.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlExpressionBuilder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlExpressionBuilder.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlExpressionBuilder.Size = new System.Drawing.Size(562, 271);
            this.pnlExpressionBuilder.TabIndex = 22;
            // 
            // tblExpressions
            // 
            this.tblExpressions.AutoScroll = true;
            this.tblExpressions.AutoSize = true;
            this.tblExpressions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblExpressions.ColumnCount = 7;
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblExpressions.Controls.Add(this.lblObjectAndFields, 1, 0);
            this.tblExpressions.Controls.Add(this.lblValueType, 3, 0);
            this.tblExpressions.Controls.Add(this.lblOperator, 4, 0);
            this.tblExpressions.Controls.Add(this.lblValue, 5, 0);
            this.tblExpressions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblExpressions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblExpressions.Location = new System.Drawing.Point(3, 28);
            this.tblExpressions.MinimumSize = new System.Drawing.Size(556, 200);
            this.tblExpressions.Name = "tblExpressions";
            this.tblExpressions.RowCount = 2;
            this.tblExpressions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblExpressions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblExpressions.Size = new System.Drawing.Size(556, 200);
            this.tblExpressions.TabIndex = 18;
            // 
            // lblObjectAndFields
            // 
            this.lblObjectAndFields.AutoSize = true;
            this.lblObjectAndFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectAndFields.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectAndFields.Location = new System.Drawing.Point(3, 3);
            this.lblObjectAndFields.Margin = new System.Windows.Forms.Padding(3);
            this.lblObjectAndFields.Name = "lblObjectAndFields";
            this.lblObjectAndFields.Size = new System.Drawing.Size(383, 13);
            this.lblObjectAndFields.TabIndex = 1;
            this.lblObjectAndFields.Text = "Salesforce Object and Field";
            this.lblObjectAndFields.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblValueType
            // 
            this.lblValueType.AutoSize = true;
            this.lblValueType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblValueType.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValueType.Location = new System.Drawing.Point(392, 3);
            this.lblValueType.Margin = new System.Windows.Forms.Padding(3);
            this.lblValueType.Name = "lblValueType";
            this.lblValueType.Size = new System.Drawing.Size(60, 13);
            this.lblValueType.TabIndex = 2;
            this.lblValueType.Text = "Value Type";
            this.lblValueType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOperator
            // 
            this.lblOperator.AutoSize = true;
            this.lblOperator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOperator.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOperator.Location = new System.Drawing.Point(458, 3);
            this.lblOperator.Margin = new System.Windows.Forms.Padding(3);
            this.lblOperator.Name = "lblOperator";
            this.lblOperator.Size = new System.Drawing.Size(54, 13);
            this.lblOperator.TabIndex = 3;
            this.lblOperator.Text = "Operator";
            this.lblOperator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblValue.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(518, 3);
            this.lblValue.Margin = new System.Windows.Forms.Padding(3);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(35, 13);
            this.lblValue.TabIndex = 4;
            this.lblValue.Text = "Value";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoSize = true;
            this.pnlOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOptions.ColumnCount = 4;
            this.pnlOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlOptions.Controls.Add(this.pnlFilterLogic, 3, 0);
            this.pnlOptions.Controls.Add(this.lnkClearAll);
            this.pnlOptions.Controls.Add(this.lnkAddRemoveFilterLogic, 2, 0);
            this.pnlOptions.Controls.Add(this.lnkAddRow, 0, 0);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlOptions.Location = new System.Drawing.Point(3, 234);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pnlOptions.Size = new System.Drawing.Size(556, 34);
            this.pnlOptions.TabIndex = 21;
            // 
            // pnlFilterLogic
            // 
            this.pnlFilterLogic.AutoSize = true;
            this.pnlFilterLogic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlFilterLogic.Controls.Add(this.txtFilterLogic);
            this.pnlFilterLogic.Controls.Add(this.lblFilterLogic);
            this.pnlFilterLogic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFilterLogic.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlFilterLogic.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlFilterLogic.Location = new System.Drawing.Point(288, 3);
            this.pnlFilterLogic.Name = "pnlFilterLogic";
            this.pnlFilterLogic.Size = new System.Drawing.Size(265, 28);
            this.pnlFilterLogic.TabIndex = 5;
            this.pnlFilterLogic.Visible = false;
            // 
            // txtFilterLogic
            // 
            this.txtFilterLogic.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilterLogic.Location = new System.Drawing.Point(78, 3);
            this.txtFilterLogic.Name = "txtFilterLogic";
            this.txtFilterLogic.Size = new System.Drawing.Size(184, 22);
            this.txtFilterLogic.TabIndex = 3;
            // 
            // lblFilterLogic
            // 
            this.lblFilterLogic.AutoSize = true;
            this.lblFilterLogic.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterLogic.Location = new System.Drawing.Point(3, 5);
            this.lblFilterLogic.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.lblFilterLogic.Name = "lblFilterLogic";
            this.lblFilterLogic.Size = new System.Drawing.Size(69, 13);
            this.lblFilterLogic.TabIndex = 4;
            this.lblFilterLogic.Text = "Filter Logic :";
            // 
            // lnkClearAll
            // 
            this.lnkClearAll.AutoSize = true;
            this.lnkClearAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkClearAll.Location = new System.Drawing.Point(63, 8);
            this.lnkClearAll.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.lnkClearAll.Name = "lnkClearAll";
            this.lnkClearAll.Size = new System.Drawing.Size(49, 13);
            this.lnkClearAll.TabIndex = 0;
            this.lnkClearAll.TabStop = true;
            this.lnkClearAll.Text = "Clear All";
            this.lnkClearAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkClearAll_LinkClicked);
            // 
            // lnkAddRemoveFilterLogic
            // 
            this.lnkAddRemoveFilterLogic.AutoSize = true;
            this.lnkAddRemoveFilterLogic.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkAddRemoveFilterLogic.Location = new System.Drawing.Point(118, 8);
            this.lnkAddRemoveFilterLogic.Margin = new System.Windows.Forms.Padding(3, 8, 80, 3);
            this.lnkAddRemoveFilterLogic.Name = "lnkAddRemoveFilterLogic";
            this.lnkAddRemoveFilterLogic.Size = new System.Drawing.Size(87, 13);
            this.lnkAddRemoveFilterLogic.TabIndex = 1;
            this.lnkAddRemoveFilterLogic.TabStop = true;
            this.lnkAddRemoveFilterLogic.Text = "Add Filter Logic";
            this.lnkAddRemoveFilterLogic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddRemoveFilterLogic_LinkClicked);
            // 
            // lnkAddRow
            // 
            this.lnkAddRow.AutoSize = true;
            this.lnkAddRow.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkAddRow.Location = new System.Drawing.Point(3, 8);
            this.lnkAddRow.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.lnkAddRow.Name = "lnkAddRow";
            this.lnkAddRow.Size = new System.Drawing.Size(54, 13);
            this.lnkAddRow.TabIndex = 0;
            this.lnkAddRow.TabStop = true;
            this.lnkAddRow.Text = "Add Row";
            this.lnkAddRow.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddRow_LinkClicked);
            // 
            // FilterByPanel
            // 
            this.FilterByPanel.AutoSize = true;
            this.FilterByPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FilterByPanel.Controls.Add(this.lblFilterBy);
            this.FilterByPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterByPanel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilterByPanel.Location = new System.Drawing.Point(3, 3);
            this.FilterByPanel.Name = "FilterByPanel";
            this.FilterByPanel.Size = new System.Drawing.Size(556, 19);
            this.FilterByPanel.TabIndex = 20;
            // 
            // lblFilterBy
            // 
            this.lblFilterBy.AutoSize = true;
            this.lblFilterBy.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterBy.Location = new System.Drawing.Point(3, 3);
            this.lblFilterBy.Margin = new System.Windows.Forms.Padding(3);
            this.lblFilterBy.Name = "lblFilterBy";
            this.lblFilterBy.Size = new System.Drawing.Size(44, 13);
            this.lblFilterBy.TabIndex = 0;
            this.lblFilterBy.Text = "Filters :";
            // 
            // ExpressionBuilderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pnlExpressionBuilder);
            this.DoubleBuffered = true;
            this.Name = "ExpressionBuilderView";
            this.Size = new System.Drawing.Size(562, 271);
            this.Load += new System.EventHandler(this.ExpressionBuilderView_Load);
            this.pnlExpressionBuilder.ResumeLayout(false);
            this.pnlExpressionBuilder.PerformLayout();
            this.tblExpressions.ResumeLayout(false);
            this.tblExpressions.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlFilterLogic.ResumeLayout(false);
            this.pnlFilterLogic.PerformLayout();
            this.FilterByPanel.ResumeLayout(false);
            this.FilterByPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlExpressionBuilder;
        private System.Windows.Forms.TableLayoutPanel tblExpressions;
        private System.Windows.Forms.LinkLabel lnkClearAll;
        private System.Windows.Forms.TableLayoutPanel pnlOptions;
        private System.Windows.Forms.LinkLabel lnkAddRow;
        private System.Windows.Forms.LinkLabel lnkAddRemoveFilterLogic;
        private System.Windows.Forms.Label lblFilterLogic;
        private System.Windows.Forms.TextBox txtFilterLogic;
        private System.Windows.Forms.FlowLayoutPanel pnlFilterLogic;
        private System.Windows.Forms.Label lblObjectAndFields;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.FlowLayoutPanel FilterByPanel;
        private System.Windows.Forms.Label lblFilterBy;
    }
}
