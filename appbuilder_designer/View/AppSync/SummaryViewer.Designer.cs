namespace Apttus.XAuthor.AppDesigner
{
    partial class SummaryViewer
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSummaryDescription = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flpPicklists = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPicklists = new System.Windows.Forms.Label();
            this.flpFieldDataTypes = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFieldDataTypes = new System.Windows.Forms.Label();
            this.flpFields = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFields = new System.Windows.Forms.Label();
            this.flpObjects = new System.Windows.Forms.FlowLayoutPanel();
            this.lblObjects = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.flpRecordType = new System.Windows.Forms.FlowLayoutPanel();
            this.lblRecordTypes = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flpPicklists.SuspendLayout();
            this.flpFieldDataTypes.SuspendLayout();
            this.flpFields.SuspendLayout();
            this.flpObjects.SuspendLayout();
            this.flpRecordType.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSummary, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(631, 271);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.lblSummaryDescription, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(625, 25);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // lblSummaryDescription
            // 
            this.lblSummaryDescription.AutoSize = true;
            this.lblSummaryDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSummaryDescription.Location = new System.Drawing.Point(5, 5);
            this.lblSummaryDescription.Margin = new System.Windows.Forms.Padding(5);
            this.lblSummaryDescription.Name = "lblSummaryDescription";
            this.lblSummaryDescription.Size = new System.Drawing.Size(615, 15);
            this.lblSummaryDescription.TabIndex = 2;
            this.lblSummaryDescription.Text = "The following app components do not match those in your Salesforce instance. Clic" +
    "k a link to review or take action.";
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummary.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSummary.Location = new System.Drawing.Point(5, 5);
            this.lblSummary.Margin = new System.Windows.Forms.Padding(5);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(100, 30);
            this.lblSummary.TabIndex = 5;
            this.lblSummary.Text = "Summary";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.flpPicklists, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.flpFieldDataTypes, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.flpFields, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flpObjects, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.flpRecordType, 0, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 74);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(625, 194);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // flpPicklists
            // 
            this.flpPicklists.AutoSize = true;
            this.flpPicklists.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpPicklists.Controls.Add(this.lblPicklists);
            this.flpPicklists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpPicklists.Location = new System.Drawing.Point(3, 90);
            this.flpPicklists.Name = "flpPicklists";
            this.flpPicklists.Size = new System.Drawing.Size(619, 25);
            this.flpPicklists.TabIndex = 3;
            // 
            // lblPicklists
            // 
            this.lblPicklists.AutoSize = true;
            this.lblPicklists.Location = new System.Drawing.Point(5, 5);
            this.lblPicklists.Margin = new System.Windows.Forms.Padding(5);
            this.lblPicklists.Name = "lblPicklists";
            this.lblPicklists.Size = new System.Drawing.Size(55, 15);
            this.lblPicklists.TabIndex = 0;
            this.lblPicklists.Text = "Picklists :";
            // 
            // flpFieldDataTypes
            // 
            this.flpFieldDataTypes.Controls.Add(this.lblFieldDataTypes);
            this.flpFieldDataTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFieldDataTypes.Location = new System.Drawing.Point(3, 61);
            this.flpFieldDataTypes.Name = "flpFieldDataTypes";
            this.flpFieldDataTypes.Size = new System.Drawing.Size(619, 23);
            this.flpFieldDataTypes.TabIndex = 2;
            // 
            // lblFieldDataTypes
            // 
            this.lblFieldDataTypes.AutoSize = true;
            this.lblFieldDataTypes.Location = new System.Drawing.Point(5, 5);
            this.lblFieldDataTypes.Margin = new System.Windows.Forms.Padding(5);
            this.lblFieldDataTypes.Name = "lblFieldDataTypes";
            this.lblFieldDataTypes.Size = new System.Drawing.Size(98, 15);
            this.lblFieldDataTypes.TabIndex = 0;
            this.lblFieldDataTypes.Text = "Field Data Types :";
            // 
            // flpFields
            // 
            this.flpFields.Controls.Add(this.lblFields);
            this.flpFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFields.Location = new System.Drawing.Point(3, 32);
            this.flpFields.Name = "flpFields";
            this.flpFields.Size = new System.Drawing.Size(619, 23);
            this.flpFields.TabIndex = 1;
            // 
            // lblFields
            // 
            this.lblFields.AutoSize = true;
            this.lblFields.Location = new System.Drawing.Point(5, 5);
            this.lblFields.Margin = new System.Windows.Forms.Padding(5);
            this.lblFields.Name = "lblFields";
            this.lblFields.Size = new System.Drawing.Size(43, 15);
            this.lblFields.TabIndex = 0;
            this.lblFields.Text = "Fields :";
            // 
            // flpObjects
            // 
            this.flpObjects.Controls.Add(this.lblObjects);
            this.flpObjects.Controls.Add(this.linkLabel1);
            this.flpObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpObjects.Location = new System.Drawing.Point(3, 3);
            this.flpObjects.Name = "flpObjects";
            this.flpObjects.Size = new System.Drawing.Size(619, 23);
            this.flpObjects.TabIndex = 0;
            // 
            // lblObjects
            // 
            this.lblObjects.AutoSize = true;
            this.lblObjects.Location = new System.Drawing.Point(5, 5);
            this.lblObjects.Margin = new System.Windows.Forms.Padding(5);
            this.lblObjects.Name = "lblObjects";
            this.lblObjects.Size = new System.Drawing.Size(53, 15);
            this.lblObjects.TabIndex = 0;
            this.lblObjects.Text = "Objects :";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(66, 3);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(3);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(0, 15);
            this.linkLabel1.TabIndex = 1;
            // 
            // flpRecordType
            // 
            this.flpRecordType.AutoSize = true;
            this.flpRecordType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpRecordType.Controls.Add(this.lblRecordTypes);
            this.flpRecordType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpRecordType.Location = new System.Drawing.Point(3, 121);
            this.flpRecordType.Name = "flpRecordType";
            this.flpRecordType.Size = new System.Drawing.Size(619, 70);
            this.flpRecordType.TabIndex = 4;
            // 
            // lblRecordTypes
            // 
            this.lblRecordTypes.AutoSize = true;
            this.lblRecordTypes.Location = new System.Drawing.Point(5, 3);
            this.lblRecordTypes.Margin = new System.Windows.Forms.Padding(5, 3, 5, 5);
            this.lblRecordTypes.Name = "lblRecordTypes";
            this.lblRecordTypes.Size = new System.Drawing.Size(83, 15);
            this.lblRecordTypes.TabIndex = 0;
            this.lblRecordTypes.Text = "Record Types :";
            this.lblRecordTypes.Visible = false;
            // 
            // SummaryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SummaryViewer";
            this.Size = new System.Drawing.Size(631, 271);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flpPicklists.ResumeLayout(false);
            this.flpPicklists.PerformLayout();
            this.flpFieldDataTypes.ResumeLayout(false);
            this.flpFieldDataTypes.PerformLayout();
            this.flpFields.ResumeLayout(false);
            this.flpFields.PerformLayout();
            this.flpObjects.ResumeLayout(false);
            this.flpObjects.PerformLayout();
            this.flpRecordType.ResumeLayout(false);
            this.flpRecordType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblSummaryDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flpObjects;
        private System.Windows.Forms.Label lblObjects;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.FlowLayoutPanel flpPicklists;
        private System.Windows.Forms.Label lblPicklists;
        private System.Windows.Forms.FlowLayoutPanel flpFieldDataTypes;
        private System.Windows.Forms.Label lblFieldDataTypes;
        private System.Windows.Forms.FlowLayoutPanel flpFields;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.FlowLayoutPanel flpRecordType;
        private System.Windows.Forms.Label lblRecordTypes;
    }
}
