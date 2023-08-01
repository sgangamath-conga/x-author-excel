namespace Apttus.XAuthor.AppDesigner
{
    partial class QueryLabelView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryLabelView));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblLabel = new System.Windows.Forms.Label();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveLabel = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlMain.Controls.Add(this.lblLabel);
            this.pnlMain.Controls.Add(this.txtLabel);
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnSaveLabel);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(386, 45);
            this.pnlMain.TabIndex = 0;
            // 
            // lblLabel
            // 
            this.lblLabel.AutoSize = true;
            this.lblLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblLabel.Location = new System.Drawing.Point(4, 2);
            this.lblLabel.Margin = new System.Windows.Forms.Padding(0);
            this.lblLabel.Name = "lblLabel";
            this.lblLabel.Size = new System.Drawing.Size(262, 13);
            this.lblLabel.TabIndex = 20;
            this.lblLabel.Text = "Assign a label for this filter (applies to User Input)";
            // 
            // txtLabel
            // 
            this.txtLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLabel.Location = new System.Drawing.Point(4, 20);
            this.txtLabel.Margin = new System.Windows.Forms.Padding(0);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(312, 20);
            this.txtLabel.TabIndex = 19;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.cancel_icon;
            this.btnCancel.Location = new System.Drawing.Point(351, 16);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(26, 26);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveLabel
            // 
            this.btnSaveLabel.FlatAppearance.BorderColor = System.Drawing.Color.DarkGreen;
            this.btnSaveLabel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            this.btnSaveLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveLabel.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.ok_icon;
            this.btnSaveLabel.Location = new System.Drawing.Point(321, 16);
            this.btnSaveLabel.Margin = new System.Windows.Forms.Padding(0);
            this.btnSaveLabel.Name = "btnSaveLabel";
            this.btnSaveLabel.Size = new System.Drawing.Size(26, 26);
            this.btnSaveLabel.TabIndex = 18;
            this.btnSaveLabel.UseVisualStyleBackColor = true;
            this.btnSaveLabel.Click += new System.EventHandler(this.btnSaveLabel_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // QueryLabelView
            // 
            this.AcceptButton = this.btnSaveLabel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(386, 45);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QueryLabelView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "QueryLabel";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.QueryLabelView_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnSaveLabel;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblLabel;
    }
}