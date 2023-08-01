namespace Apttus.XAuthor.AppDesigner
{
    partial class AppSyncView
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lnkMissingObjects = new System.Windows.Forms.Label();
            this.lnkMissingFields = new System.Windows.Forms.Label();
            this.lnkFieldsMismatch = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btnSyncApp = new System.Windows.Forms.Button();
            this.lblSyncMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSyncAppTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 44);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(882, 365);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.lblSummary);
            this.flowLayoutPanel1.Controls.Add(this.lnkMissingObjects);
            this.flowLayoutPanel1.Controls.Add(this.lnkMissingFields);
            this.flowLayoutPanel1.Controls.Add(this.lnkFieldsMismatch);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(124, 359);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSummary.Location = new System.Drawing.Point(5, 5);
            this.lblSummary.Margin = new System.Windows.Forms.Padding(5);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Padding = new System.Windows.Forms.Padding(10);
            this.lblSummary.Size = new System.Drawing.Size(112, 35);
            this.lblSummary.TabIndex = 3;
            this.lblSummary.TabStop = true;
            this.lblSummary.Text = "Summary";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSummary.Click += new System.EventHandler(this.lblSummary_Click);
            // 
            // lnkMissingObjects
            // 
            this.lnkMissingObjects.AutoSize = true;
            this.lnkMissingObjects.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lnkMissingObjects.Location = new System.Drawing.Point(5, 50);
            this.lnkMissingObjects.Margin = new System.Windows.Forms.Padding(5);
            this.lnkMissingObjects.Name = "lnkMissingObjects";
            this.lnkMissingObjects.Padding = new System.Windows.Forms.Padding(10);
            this.lnkMissingObjects.Size = new System.Drawing.Size(112, 35);
            this.lnkMissingObjects.TabIndex = 0;
            this.lnkMissingObjects.TabStop = true;
            this.lnkMissingObjects.Text = "Objects ";
            this.lnkMissingObjects.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkMissingObjects.Click += new System.EventHandler(this.lnkMissingObjects_Click);
            // 
            // lnkMissingFields
            // 
            this.lnkMissingFields.AutoSize = true;
            this.lnkMissingFields.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lnkMissingFields.Location = new System.Drawing.Point(5, 95);
            this.lnkMissingFields.Margin = new System.Windows.Forms.Padding(5);
            this.lnkMissingFields.Name = "lnkMissingFields";
            this.lnkMissingFields.Padding = new System.Windows.Forms.Padding(10);
            this.lnkMissingFields.Size = new System.Drawing.Size(112, 35);
            this.lnkMissingFields.TabIndex = 1;
            this.lnkMissingFields.TabStop = true;
            this.lnkMissingFields.Text = "Fields";
            this.lnkMissingFields.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkMissingFields.Click += new System.EventHandler(this.lnkMissingFields_Click);
            // 
            // lnkFieldsMismatch
            // 
            this.lnkFieldsMismatch.AutoSize = true;
            this.lnkFieldsMismatch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lnkFieldsMismatch.Location = new System.Drawing.Point(5, 140);
            this.lnkFieldsMismatch.Margin = new System.Windows.Forms.Padding(5);
            this.lnkFieldsMismatch.Name = "lnkFieldsMismatch";
            this.lnkFieldsMismatch.Padding = new System.Windows.Forms.Padding(10);
            this.lnkFieldsMismatch.Size = new System.Drawing.Size(112, 35);
            this.lnkFieldsMismatch.TabIndex = 2;
            this.lnkFieldsMismatch.TabStop = true;
            this.lnkFieldsMismatch.Text = "Field Data Types";
            this.lnkFieldsMismatch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkFieldsMismatch.Click += new System.EventHandler(this.lnkFieldsMismatch_Click);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.ColumnCount = 3;
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 702F));
            this.panel2.Controls.Add(this.btnFinish, 0, 0);
            this.panel2.Controls.Add(this.btnSyncApp, 1, 0);
            this.panel2.Controls.Add(this.lblSyncMessage, 2, 0);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 412);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.panel2.Size = new System.Drawing.Size(888, 45);
            this.panel2.TabIndex = 2;
            // 
            // btnFinish
            // 
            this.btnFinish.AutoSize = true;
            this.btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnFinish.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnFinish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinish.Location = new System.Drawing.Point(798, 8);
            this.btnFinish.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFinish.Size = new System.Drawing.Size(87, 27);
            this.btnFinish.TabIndex = 6;
            this.btnFinish.Text = "OK";
            this.btnFinish.UseVisualStyleBackColor = true;
            // 
            // btnSyncApp
            // 
            this.btnSyncApp.AutoSize = true;
            this.btnSyncApp.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnSyncApp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSyncApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncApp.Location = new System.Drawing.Point(705, 8);
            this.btnSyncApp.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnSyncApp.Name = "btnSyncApp";
            this.btnSyncApp.Size = new System.Drawing.Size(87, 27);
            this.btnSyncApp.TabIndex = 5;
            this.btnSyncApp.Text = "Sync App";
            this.btnSyncApp.UseVisualStyleBackColor = true;
            this.btnSyncApp.Click += new System.EventHandler(this.btnSyncApp_Click);
            // 
            // lblSyncMessage
            // 
            this.lblSyncMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSyncMessage.AutoSize = true;
            this.lblSyncMessage.Location = new System.Drawing.Point(3, 3);
            this.lblSyncMessage.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblSyncMessage.Name = "lblSyncMessage";
            this.lblSyncMessage.Padding = new System.Windows.Forms.Padding(2);
            this.lblSyncMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSyncMessage.Size = new System.Drawing.Size(4, 19);
            this.lblSyncMessage.TabIndex = 7;
            this.lblSyncMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.lblSyncAppTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 41);
            this.panel1.TabIndex = 1;
            // 
            // lblSyncAppTitle
            // 
            this.lblSyncAppTitle.AutoSize = true;
            this.lblSyncAppTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSyncAppTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblSyncAppTitle.Location = new System.Drawing.Point(3, 3);
            this.lblSyncAppTitle.Name = "lblSyncAppTitle";
            this.lblSyncAppTitle.Size = new System.Drawing.Size(99, 30);
            this.lblSyncAppTitle.TabIndex = 1;
            this.lblSyncAppTitle.Text = "Sync App";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(888, 457);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // AppSyncView
            // 
            this.AcceptButton = this.btnFinish;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnFinish;
            this.ClientSize = new System.Drawing.Size(888, 457);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppSyncView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "App Sync";
            this.Load += new System.EventHandler(this.AppSyncView_Load);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void LnkMissingObjects_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void LnkFieldsMismatch_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void LnkMissingFields_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lnkMissingObjects;
        private System.Windows.Forms.Label lnkMissingFields;
        private System.Windows.Forms.Label lnkFieldsMismatch;
        private System.Windows.Forms.TableLayoutPanel panel2;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblSyncAppTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnSyncApp;
        private System.Windows.Forms.Label lblSyncMessage;
    }
}