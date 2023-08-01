namespace Apttus.XAuthor.AppRuntime
{
    partial class ConnectionEntry
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
            this.components = new System.ComponentModel.Container();
            this.lnklblSandbox = new System.Windows.Forms.LinkLabel();
            this.lnklblProduction = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLoginURL = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblConnectStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnklblSandbox
            // 
            this.lnklblSandbox.AutoSize = true;
            this.lnklblSandbox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnklblSandbox.Location = new System.Drawing.Point(68, 30);
            this.lnklblSandbox.Margin = new System.Windows.Forms.Padding(3);
            this.lnklblSandbox.Name = "lnklblSandbox";
            this.lnklblSandbox.Size = new System.Drawing.Size(148, 15);
            this.lnklblSandbox.TabIndex = 22;
            this.lnklblSandbox.TabStop = true;
            this.lnklblSandbox.Text = "https://test.salesforce.com";
            this.lnklblSandbox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblSandbox_LinkClicked);
            // 
            // lnklblProduction
            // 
            this.lnklblProduction.AutoSize = true;
            this.lnklblProduction.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnklblProduction.Location = new System.Drawing.Point(68, 3);
            this.lnklblProduction.Margin = new System.Windows.Forms.Padding(3);
            this.lnklblProduction.Name = "lnklblProduction";
            this.lnklblProduction.Size = new System.Drawing.Size(156, 15);
            this.lnklblProduction.TabIndex = 23;
            this.lnklblProduction.TabStop = true;
            this.lnklblProduction.Text = "https://login.salesforce.com";
            this.lnklblProduction.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblProduction_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 21;
            this.label1.Text = "Examples:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtLoginURL
            // 
            this.txtLoginURL.BackColor = System.Drawing.Color.White;
            this.txtLoginURL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLoginURL.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLoginURL.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLoginURL.Location = new System.Drawing.Point(189, 5);
            this.txtLoginURL.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtLoginURL.Name = "txtLoginURL";
            this.txtLoginURL.Size = new System.Drawing.Size(180, 23);
            this.txtLoginURL.TabIndex = 19;
            this.txtLoginURL.Enter += new System.EventHandler(this.txtLoginURL_Enter);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtName.Location = new System.Drawing.Point(3, 5);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtName.MaxLength = 25;
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(180, 23);
            this.txtName.TabIndex = 18;
            // 
            // btnConnect
            // 
            this.btnConnect.AutoSize = true;
            this.btnConnect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConnect.BackColor = System.Drawing.Color.White;
            this.btnConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConnect.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnConnect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnConnect.Location = new System.Drawing.Point(375, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(64, 27);
            this.btnConnect.TabIndex = 20;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // lblConnectStatus
            // 
            this.lblConnectStatus.AutoSize = true;
            this.lblConnectStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblConnectStatus.Location = new System.Drawing.Point(230, 3);
            this.lblConnectStatus.Margin = new System.Windows.Forms.Padding(3);
            this.lblConnectStatus.Name = "lblConnectStatus";
            this.lblConnectStatus.Size = new System.Drawing.Size(86, 21);
            this.lblConnectStatus.TabIndex = 24;
            this.lblConnectStatus.Text = "Not connected";
            this.lblConnectStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblConnectStatus.UseCompatibleTextRendering = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(448, 93);
            this.tableLayoutPanel1.TabIndex = 25;
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
            this.tableLayoutPanel2.Controls.Add(this.txtLoginURL, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnConnect, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(442, 33);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lnklblSandbox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblConnectStatus, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.lnklblProduction, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 42);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(442, 48);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // ConnectionEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConnectionEntry";
            this.Size = new System.Drawing.Size(448, 93);
            this.Load += new System.EventHandler(this.ConnectionEntry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnklblSandbox;
        private System.Windows.Forms.LinkLabel lnklblProduction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLoginURL;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label lblConnectStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
