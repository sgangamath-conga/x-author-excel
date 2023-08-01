namespace Apttus.IntelligentCloudV2.Login
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtTenant = new System.Windows.Forms.TextBox();
            this.webBrowserControl = new EO.WinForm.WebControl();
            this.webView1 = new EO.WebBrowser.WebView();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.webBrowserControl, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(551, 569);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGo);
            this.panel1.Controls.Add(this.txtTenant);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(545, 1);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(360, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtTenant
            // 
            this.txtTenant.Location = new System.Drawing.Point(3, 3);
            this.txtTenant.Name = "txtTenant";
            this.txtTenant.Size = new System.Drawing.Size(351, 20);
            this.txtTenant.TabIndex = 0;
            // 
            // webBrowserControl
            // 
            this.webBrowserControl.BackColor = System.Drawing.Color.White;
            this.webBrowserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserControl.Location = new System.Drawing.Point(3, 3);
            this.webBrowserControl.Name = "webBrowserControl";
            this.webBrowserControl.Size = new System.Drawing.Size(545, 563);
            this.webBrowserControl.TabIndex = 1;
            this.webBrowserControl.Text = "webControl1";
            this.webBrowserControl.WebView = this.webView1;
            // 
            // webView1
            // 
            this.webView1.BeforeNavigate += new EO.WebBrowser.BeforeNavigateHandler(this.webView1_BeforeNavigate);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 569);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apttus X-Author for Excel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtTenant;
        private EO.WinForm.WebControl webBrowserControl;
        private EO.WebBrowser.WebView webView1;
    }
}