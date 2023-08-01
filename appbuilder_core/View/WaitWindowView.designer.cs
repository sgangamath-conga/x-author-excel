namespace Apttus.XAuthor.Core
{
    partial class WaitWindowView
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
            this.MessageLabel = new System.Windows.Forms.Label();
            this.progressIndicator = new Apttus.ProgressIndicator.ProgressIndicator();
            this.SuspendLayout();
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessageLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.MessageLabel.Location = new System.Drawing.Point(73, 24);
            this.MessageLabel.MaximumSize = new System.Drawing.Size(370, 0);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(90, 16);
            this.MessageLabel.TabIndex = 15;
            this.MessageLabel.Text = "Processing...";
            this.MessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MessageLabel.UseWaitCursor = true;
            // 
            // progressIndicator
            // 
            this.progressIndicator.AnimationSpeed = 50;
            this.progressIndicator.AutoStart = true;
            this.progressIndicator.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(138)))), ((int)(((byte)(61)))));
            this.progressIndicator.Location = new System.Drawing.Point(12, 10);
            this.progressIndicator.Name = "progressIndicator";
            this.progressIndicator.Percentage = 0F;
            this.progressIndicator.Size = new System.Drawing.Size(45, 45);
            this.progressIndicator.TabIndex = 14;
            this.progressIndicator.TabStop = false;
            this.progressIndicator.UseWaitCursor = true;
            // 
            // WaitWindowView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 67);
            this.ControlBox = false;
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.progressIndicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitWindowView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ApttusWaitProcess";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label MessageLabel;
        public ProgressIndicator.ProgressIndicator progressIndicator;
    }
}