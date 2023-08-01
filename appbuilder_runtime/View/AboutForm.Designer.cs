namespace Apttus.XAuthor.AppRuntime.View
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCopyrightInfo = new System.Windows.Forms.Label();
            this.lblAboutDescription = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.version = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(534, 344);
            this.panel2.TabIndex = 10;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.btnClose);
            this.panel3.Controls.Add(this.lblCopyrightInfo);
            this.panel3.Controls.Add(this.lblAboutDescription);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(180, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(354, 342);
            this.panel3.TabIndex = 8;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(274, 315);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblCopyrightInfo
            // 
            this.lblCopyrightInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblCopyrightInfo.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblCopyrightInfo.Location = new System.Drawing.Point(16, 302);
            this.lblCopyrightInfo.Name = "lblCopyrightInfo";
            this.lblCopyrightInfo.Size = new System.Drawing.Size(243, 37);
            this.lblCopyrightInfo.TabIndex = 7;
            this.lblCopyrightInfo.Text = "Copyright Information...";
            this.lblCopyrightInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAboutDescription
            // 
            this.lblAboutDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblAboutDescription.Location = new System.Drawing.Point(16, 15);
            this.lblAboutDescription.Name = "lblAboutDescription";
            this.lblAboutDescription.Size = new System.Drawing.Size(333, 260);
            this.lblAboutDescription.TabIndex = 6;
            this.lblAboutDescription.Text = resources.GetString("lblAboutDescription.Text");
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackgroundImage = global::Apttus.XAuthor.AppRuntime.Properties.Resources.splash_narrow_grey;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Controls.Add(this.version);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 344);
            this.panel1.TabIndex = 7;
            // 
            // version
            // 
            this.version.BackColor = System.Drawing.Color.Transparent;
            this.version.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.version.Location = new System.Drawing.Point(26, 302);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(151, 37);
            this.version.TabIndex = 3;
            this.version.Text = "Version:";
            this.version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(534, 344);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 383);
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "X-Author Designer";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblCopyrightInfo;
        private System.Windows.Forms.Label lblAboutDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label version;

    }
}