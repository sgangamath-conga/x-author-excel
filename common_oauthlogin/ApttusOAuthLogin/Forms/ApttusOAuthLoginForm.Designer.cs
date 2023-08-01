namespace Apttus.OAuthLoginControl.Forms
{
    partial class ApttusOAuthLoginForm
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
            this.progressIndicator = new Apttus.ProgressIndicator.ProgressIndicator();
            this.lblProgressStatus = new System.Windows.Forms.Label();
            this.lblAboutDescription = new System.Windows.Forms.Label();
            this.pbApttusLogo = new System.Windows.Forms.PictureBox();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.WebViewContent = new EO.WebBrowser.WebView();
            this.UIWebBrowser = new EO.WinForm.WebControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbApttusLogo)).BeginInit();
            this.pnlProgress.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressIndicator
            // 
            this.progressIndicator.AnimationSpeed = 70;
            this.progressIndicator.BackColor = System.Drawing.Color.Transparent;
            this.progressIndicator.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(138)))), ((int)(((byte)(61)))));
            this.progressIndicator.Location = new System.Drawing.Point(47, 0);
            this.progressIndicator.Name = "progressIndicator";
            this.progressIndicator.Percentage = 0F;
            this.progressIndicator.Size = new System.Drawing.Size(71, 71);
            this.progressIndicator.TabIndex = 26;
            this.progressIndicator.TabStop = false;
            // 
            // lblProgressStatus
            // 
            this.lblProgressStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressStatus.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblProgressStatus.Location = new System.Drawing.Point(124, 0);
            this.lblProgressStatus.Name = "lblProgressStatus";
            this.lblProgressStatus.Size = new System.Drawing.Size(380, 71);
            this.lblProgressStatus.TabIndex = 27;
            this.lblProgressStatus.Text = "Logging into Conga, Please wait...";
            this.lblProgressStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAboutDescription
            // 
            this.lblAboutDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAboutDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblAboutDescription.Location = new System.Drawing.Point(129, 10);
            this.lblAboutDescription.Name = "lblAboutDescription";
            this.lblAboutDescription.Size = new System.Drawing.Size(366, 511);
            this.lblAboutDescription.TabIndex = 28;
            this.lblAboutDescription.Text = "About Text Here...";
            // 
            // pbApttusLogo
            // 
            this.pbApttusLogo.Location = new System.Drawing.Point(3, 10);
            this.pbApttusLogo.Name = "pbApttusLogo";
            this.pbApttusLogo.Size = new System.Drawing.Size(120, 120);
            this.pbApttusLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbApttusLogo.TabIndex = 29;
            this.pbApttusLogo.TabStop = false;
            // 
            // pnlProgress
            // 
            this.pnlProgress.Controls.Add(this.progressIndicator);
            this.pnlProgress.Controls.Add(this.lblProgressStatus);
            this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgress.Location = new System.Drawing.Point(0, 524);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(507, 79);
            this.pnlProgress.TabIndex = 30;
            // 
            // pnlAbout
            // 
            this.pnlAbout.Controls.Add(this.lblAboutDescription);
            this.pnlAbout.Controls.Add(this.pbApttusLogo);
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(0, 0);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(507, 524);
            this.pnlAbout.TabIndex = 31;
            // 
            // WebViewContent
            //
            this.WebViewContent.Url = "";
            this.WebViewContent.BeforeNavigate += new EO.WebBrowser.BeforeNavigateHandler(this.webViewContent_BeforeNavigate);
			this.WebViewContent.IsLoadingChanged += WebViewContent_IsLoadingChanged;
            this.WebViewContent.NewWindow += WebViewContent_NewWindow;
            this.WebViewContent.LoadFailed += webViewContent_LoadFailed;
            // 
            // UIWebBrowser
            // 
            this.UIWebBrowser.BackColor = System.Drawing.Color.White;
            this.UIWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UIWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.UIWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.UIWebBrowser.Name = "UIWebBrowser";
            this.UIWebBrowser.Size = new System.Drawing.Size(507, 603);
            this.UIWebBrowser.TabIndex = 25;
            this.UIWebBrowser.WebView = this.WebViewContent;
            // 
            // ApttusOAuthLoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(507, 603);
            this.Controls.Add(this.pnlAbout);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.UIWebBrowser);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApttusOAuthLoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApttusOAuthLoginForm_FormClosing);
            this.Load += new System.EventHandler(this.ApttusOAuthLoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbApttusLogo)).EndInit();
            this.pnlProgress.ResumeLayout(false);
            this.pnlAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        

        #endregion

        public Apttus.ProgressIndicator.ProgressIndicator progressIndicator;
        private System.Windows.Forms.Label lblProgressStatus;
        private System.Windows.Forms.Label lblAboutDescription;
        private System.Windows.Forms.PictureBox pbApttusLogo;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.Panel pnlAbout;
        private EO.WebBrowser.WebView WebViewContent;
        private EO.WinForm.WebControl UIWebBrowser;
    }
}