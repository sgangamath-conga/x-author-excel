namespace Apttus.XAuthor.AppDesigner
{
    partial class DeleteActionView
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
            this.tlpMainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.gboActionDetails = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblActionName = new System.Windows.Forms.Label();
            this.lblObject = new System.Windows.Forms.Label();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cboMap = new System.Windows.Forms.ComboBox();
            this.lblMap = new System.Windows.Forms.Label();
            this.gboExpressionBuilder = new System.Windows.Forms.GroupBox();
            this.dataTableExpressionBuilder = new Apttus.XAuthor.AppDesigner.DataTableExpressionBuilderView();
            this.tlpFooterPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tlpWarningPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkPromptMessage = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tlpMainPanel.SuspendLayout();
            this.gboActionDetails.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gboExpressionBuilder.SuspendLayout();
            this.tlpFooterPanel.SuspendLayout();
            this.tlpWarningPanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMainPanel
            // 
            this.tlpMainPanel.AutoSize = true;
            this.tlpMainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMainPanel.ColumnCount = 1;
            this.tlpMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMainPanel.Controls.Add(this.lblTitle, 0, 0);
            this.tlpMainPanel.Controls.Add(this.gboActionDetails, 0, 1);
            this.tlpMainPanel.Controls.Add(this.gboExpressionBuilder, 0, 2);
            this.tlpMainPanel.Controls.Add(this.tlpFooterPanel, 0, 3);
            this.tlpMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainPanel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tlpMainPanel.Location = new System.Drawing.Point(0, 0);
            this.tlpMainPanel.Name = "tlpMainPanel";
            this.tlpMainPanel.RowCount = 4;
            this.tlpMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainPanel.Size = new System.Drawing.Size(616, 546);
            this.tlpMainPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Green;
            this.lblTitle.Location = new System.Drawing.Point(5, 5);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(137, 30);
            this.lblTitle.TabIndex = 19;
            this.lblTitle.Text = "Delete Action";
            // 
            // gboActionDetails
            // 
            this.gboActionDetails.AutoSize = true;
            this.gboActionDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gboActionDetails.Controls.Add(this.tableLayoutPanel1);
            this.gboActionDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboActionDetails.Location = new System.Drawing.Point(3, 43);
            this.gboActionDetails.Name = "gboActionDetails";
            this.gboActionDetails.Size = new System.Drawing.Size(610, 109);
            this.gboActionDetails.TabIndex = 20;
            this.gboActionDetails.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblActionName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblObject, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cboObject, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cboMap, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblMap, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 87);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // lblActionName
            // 
            this.lblActionName.AutoSize = true;
            this.lblActionName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblActionName.Location = new System.Drawing.Point(5, 5);
            this.lblActionName.Margin = new System.Windows.Forms.Padding(5);
            this.lblActionName.Name = "lblActionName";
            this.lblActionName.Size = new System.Drawing.Size(83, 15);
            this.lblActionName.TabIndex = 0;
            this.lblActionName.Text = "Action Name :";
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblObject.Location = new System.Drawing.Point(5, 63);
            this.lblObject.Margin = new System.Windows.Forms.Padding(5);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(48, 15);
            this.lblObject.TabIndex = 3;
            this.lblObject.Text = "Object :";
            // 
            // cboObject
            // 
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(96, 61);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(235, 23);
            this.cboObject.TabIndex = 5;
            this.cboObject.SelectedIndexChanged += new System.EventHandler(this.cboObject_SelectedIndexChanged);
            this.cboObject.Validating += new System.ComponentModel.CancelEventHandler(this.cboObject_Validating);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtName.Location = new System.Drawing.Point(96, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(235, 23);
            this.txtName.TabIndex = 1;
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // cboMap
            // 
            this.cboMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboMap.FormattingEnabled = true;
            this.cboMap.Location = new System.Drawing.Point(96, 32);
            this.cboMap.Name = "cboMap";
            this.cboMap.Size = new System.Drawing.Size(235, 23);
            this.cboMap.TabIndex = 4;
            this.cboMap.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            this.cboMap.SelectedIndexChanged += new System.EventHandler(this.cboMap_SelectedIndexChanged);
            this.cboMap.Validating += new System.ComponentModel.CancelEventHandler(this.cboMap_Validating);
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMap.Location = new System.Drawing.Point(5, 34);
            this.lblMap.Margin = new System.Windows.Forms.Padding(5);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(64, 15);
            this.lblMap.TabIndex = 2;
            this.lblMap.Text = "Save Map :";
            // 
            // gboExpressionBuilder
            // 
            this.gboExpressionBuilder.AutoSize = true;
            this.gboExpressionBuilder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gboExpressionBuilder.Controls.Add(this.dataTableExpressionBuilder);
            this.gboExpressionBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gboExpressionBuilder.Location = new System.Drawing.Point(3, 158);
            this.gboExpressionBuilder.Name = "gboExpressionBuilder";
            this.gboExpressionBuilder.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.gboExpressionBuilder.Size = new System.Drawing.Size(610, 315);
            this.gboExpressionBuilder.TabIndex = 21;
            this.gboExpressionBuilder.TabStop = false;
            // 
            // dataTableExpressionBuilder
            // 
            this.dataTableExpressionBuilder.AutoSize = true;
            this.dataTableExpressionBuilder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dataTableExpressionBuilder.BackColor = System.Drawing.SystemColors.Window;
            this.dataTableExpressionBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTableExpressionBuilder.IsExpressionBuilderLaunchedFromQuickApp = false;
            this.dataTableExpressionBuilder.Location = new System.Drawing.Point(3, 16);
            this.dataTableExpressionBuilder.Margin = new System.Windows.Forms.Padding(0);
            this.dataTableExpressionBuilder.Model = null;
            this.dataTableExpressionBuilder.Name = "dataTableExpressionBuilder";
            this.dataTableExpressionBuilder.Size = new System.Drawing.Size(604, 296);
            this.dataTableExpressionBuilder.TabIndex = 0;
            this.dataTableExpressionBuilder.TargetObject = null;
            this.dataTableExpressionBuilder.Load += new System.EventHandler(this.expressionBuilder_Load);
            // 
            // tlpFooterPanel
            // 
            this.tlpFooterPanel.AutoSize = true;
            this.tlpFooterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpFooterPanel.ColumnCount = 1;
            this.tlpFooterPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFooterPanel.Controls.Add(this.tlpWarningPanel, 0, 0);
            this.tlpFooterPanel.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tlpFooterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFooterPanel.Location = new System.Drawing.Point(0, 476);
            this.tlpFooterPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFooterPanel.Name = "tlpFooterPanel";
            this.tlpFooterPanel.RowCount = 2;
            this.tlpFooterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFooterPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFooterPanel.Size = new System.Drawing.Size(616, 70);
            this.tlpFooterPanel.TabIndex = 22;
            // 
            // tlpWarningPanel
            // 
            this.tlpWarningPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tlpWarningPanel.AutoSize = true;
            this.tlpWarningPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpWarningPanel.ColumnCount = 1;
            this.tlpWarningPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWarningPanel.Controls.Add(this.lblWarning, 0, 0);
            this.tlpWarningPanel.Location = new System.Drawing.Point(0, 0);
            this.tlpWarningPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tlpWarningPanel.Name = "tlpWarningPanel";
            this.tlpWarningPanel.RowCount = 1;
            this.tlpWarningPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWarningPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tlpWarningPanel.Size = new System.Drawing.Size(6, 15);
            this.tlpWarningPanel.TabIndex = 1;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblWarning.Location = new System.Drawing.Point(3, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(0, 15);
            this.lblWarning.TabIndex = 7;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.chkPromptMessage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(610, 49);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // chkPromptMessage
            // 
            this.chkPromptMessage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkPromptMessage.AutoSize = true;
            this.chkPromptMessage.Checked = true;
            this.chkPromptMessage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPromptMessage.Location = new System.Drawing.Point(3, 17);
            this.chkPromptMessage.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.chkPromptMessage.Name = "chkPromptMessage";
            this.chkPromptMessage.Size = new System.Drawing.Size(15, 14);
            this.chkPromptMessage.TabIndex = 9;
            this.chkPromptMessage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panel1.Location = new System.Drawing.Point(24, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(583, 43);
            this.panel1.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(493, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(400, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 27);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // DeleteActionView
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(616, 546);
            this.Controls.Add(this.tlpMainPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteActionView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DeleteActionView";
            this.Load += new System.EventHandler(this.DeleteActionView_Load);
            this.tlpMainPanel.ResumeLayout(false);
            this.tlpMainPanel.PerformLayout();
            this.gboActionDetails.ResumeLayout(false);
            this.gboActionDetails.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gboExpressionBuilder.ResumeLayout(false);
            this.gboExpressionBuilder.PerformLayout();
            this.tlpFooterPanel.ResumeLayout(false);
            this.tlpFooterPanel.PerformLayout();
            this.tlpWarningPanel.ResumeLayout(false);
            this.tlpWarningPanel.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMainPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox gboActionDetails;
        private System.Windows.Forms.Label lblActionName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblObject;
        private System.Windows.Forms.Label lblMap;
        private System.Windows.Forms.ComboBox cboObject;
        private System.Windows.Forms.ComboBox cboMap;
        private System.Windows.Forms.GroupBox gboExpressionBuilder;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private DataTableExpressionBuilderView dataTableExpressionBuilder;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.TableLayoutPanel tlpFooterPanel;
        private System.Windows.Forms.TableLayoutPanel tlpWarningPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkPromptMessage;
    }
}
