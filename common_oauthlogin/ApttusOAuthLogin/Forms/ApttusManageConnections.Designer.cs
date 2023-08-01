namespace Apttus.OAuthLoginControl.Forms
{
    partial class ApttusManageConnections
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
            this.btnCloseWindow = new System.Windows.Forms.Button();
            this.btnAuthorize = new System.Windows.Forms.Button();
            this.txtLoginURL = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.tblConnections = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lnklblSandbox = new System.Windows.Forms.LinkLabel();
            this.lnklblProduction = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCloseWindow
            // 
            this.btnCloseWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseWindow.AutoSize = true;
            this.btnCloseWindow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCloseWindow.BackColor = System.Drawing.Color.White;
            this.btnCloseWindow.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCloseWindow.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnCloseWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseWindow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseWindow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCloseWindow.Location = new System.Drawing.Point(478, 434);
            this.btnCloseWindow.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnCloseWindow.Name = "btnCloseWindow";
            this.btnCloseWindow.Size = new System.Drawing.Size(75, 27);
            this.btnCloseWindow.TabIndex = 16;
            this.btnCloseWindow.Text = "Close";
            this.btnCloseWindow.UseVisualStyleBackColor = false;
            this.btnCloseWindow.Click += new System.EventHandler(this.btnCloseWindow_Click);
            // 
            // btnAuthorize
            // 
            this.btnAuthorize.AutoSize = true;
            this.btnAuthorize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAuthorize.BackColor = System.Drawing.Color.White;
            this.btnAuthorize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAuthorize.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnAuthorize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuthorize.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnAuthorize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAuthorize.Location = new System.Drawing.Point(465, 3);
            this.btnAuthorize.MinimumSize = new System.Drawing.Size(75, 25);
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Size = new System.Drawing.Size(76, 27);
            this.btnAuthorize.TabIndex = 15;
            this.btnAuthorize.Text = "Authorize";
            this.btnAuthorize.UseVisualStyleBackColor = false;
            this.btnAuthorize.Click += new System.EventHandler(this.btnAuthorize_Click);
            // 
            // txtLoginURL
            // 
            this.txtLoginURL.BackColor = System.Drawing.Color.White;
            this.txtLoginURL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLoginURL.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLoginURL.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLoginURL.Location = new System.Drawing.Point(234, 3);
            this.txtLoginURL.Margin = new System.Windows.Forms.Padding(3, 3, 13, 3);
            this.txtLoginURL.Name = "txtLoginURL";
            this.txtLoginURL.Size = new System.Drawing.Size(215, 23);
            this.txtLoginURL.TabIndex = 14;
            this.txtLoginURL.Enter += new System.EventHandler(this.txtLoginURL_Enter);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtName.Location = new System.Drawing.Point(3, 3);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 3, 13, 3);
            this.txtName.MaxLength = 25;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(215, 23);
            this.txtName.TabIndex = 13;
            // 
            // tblConnections
            // 
            this.tblConnections.AutoScroll = true;
            this.tblConnections.AutoSize = true;
            this.tblConnections.ColumnCount = 4;
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblConnections.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblConnections.Location = new System.Drawing.Point(3, 19);
            this.tblConnections.MaximumSize = new System.Drawing.Size(0, 300);
            this.tblConnections.MinimumSize = new System.Drawing.Size(0, 300);
            this.tblConnections.Name = "tblConnections";
            this.tblConnections.RowCount = 1;
            this.tblConnections.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblConnections.Size = new System.Drawing.Size(544, 300);
            this.tblConnections.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tblConnections);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 322);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Available connections";
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 331);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 97);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create a new connection";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.txtName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lnklblSandbox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtLoginURL, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lnklblProduction, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnAuthorize, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(544, 75);
            this.tableLayoutPanel2.TabIndex = 18;
            // 
            // lnklblSandbox
            // 
            this.lnklblSandbox.AutoSize = true;
            this.lnklblSandbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.lnklblSandbox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnklblSandbox.Location = new System.Drawing.Point(234, 57);
            this.lnklblSandbox.Margin = new System.Windows.Forms.Padding(3);
            this.lnklblSandbox.Name = "lnklblSandbox";
            this.lnklblSandbox.Size = new System.Drawing.Size(225, 15);
            this.lnklblSandbox.TabIndex = 17;
            this.lnklblSandbox.TabStop = true;
            this.lnklblSandbox.Text = "https://test.salesforce.com";
            this.lnklblSandbox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblSandbox_LinkClicked);
            // 
            // lnklblProduction
            // 
            this.lnklblProduction.AutoSize = true;
            this.lnklblProduction.Dock = System.Windows.Forms.DockStyle.Top;
            this.lnklblProduction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnklblProduction.Location = new System.Drawing.Point(234, 36);
            this.lnklblProduction.Margin = new System.Windows.Forms.Padding(3);
            this.lnklblProduction.Name = "lnklblProduction";
            this.lnklblProduction.Size = new System.Drawing.Size(225, 15);
            this.lnklblProduction.TabIndex = 17;
            this.lnklblProduction.TabStop = true;
            this.lnklblProduction.Text = "https://login.salesforce.com";
            this.lnklblProduction.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblProduction_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.label1.Location = new System.Drawing.Point(168, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Examples:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCloseWindow, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(556, 466);
            this.tableLayoutPanel1.TabIndex = 20;
            // 
            // ApttusManageConnections
            // 
            this.AcceptButton = this.btnCloseWindow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCloseWindow;
            this.ClientSize = new System.Drawing.Size(556, 466);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApttusManageConnections";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Connections";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApttusManageConnections_FormClosing);
            this.Load += new System.EventHandler(this.ApttusManageConnections_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCloseWindow;
        private System.Windows.Forms.Button btnAuthorize;
        private System.Windows.Forms.TextBox txtLoginURL;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TableLayoutPanel tblConnections;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.LinkLabel lnklblSandbox;
        private System.Windows.Forms.LinkLabel lnklblProduction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}