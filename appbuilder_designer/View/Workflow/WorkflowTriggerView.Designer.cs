namespace Apttus.XAuthor.AppDesigner
{
    partial class WorkflowTriggerView
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.chkAppLaunch = new System.Windows.Forms.CheckBox();
            this.radioFirstAppLaunch = new System.Windows.Forms.RadioButton();
            this.radioAppLaunch = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(226, 17);
            this.lblHeader.TabIndex = 12;
            this.lblHeader.Text = "Triggered Execution of Action Flow";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(204, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(123, 227);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // chkAppLaunch
            // 
            this.chkAppLaunch.AutoSize = true;
            this.chkAppLaunch.Location = new System.Drawing.Point(15, 47);
            this.chkAppLaunch.Name = "chkAppLaunch";
            this.chkAppLaunch.Size = new System.Drawing.Size(136, 17);
            this.chkAppLaunch.TabIndex = 18;
            this.chkAppLaunch.Text = "Execute on app launch";
            this.chkAppLaunch.UseVisualStyleBackColor = true;
            this.chkAppLaunch.CheckedChanged += new System.EventHandler(this.chkAppLaunch_CheckedChanged);
            // 
            // radioFirstAppLaunch
            // 
            this.radioFirstAppLaunch.AutoSize = true;
            this.radioFirstAppLaunch.Enabled = false;
            this.radioFirstAppLaunch.Location = new System.Drawing.Point(35, 93);
            this.radioFirstAppLaunch.Name = "radioFirstAppLaunch";
            this.radioFirstAppLaunch.Size = new System.Drawing.Size(155, 17);
            this.radioFirstAppLaunch.TabIndex = 19;
            this.radioFirstAppLaunch.Text = "Execute on first launch only";
            this.radioFirstAppLaunch.UseVisualStyleBackColor = true;
            // 
            // radioAppLaunch
            // 
            this.radioAppLaunch.AutoSize = true;
            this.radioAppLaunch.Checked = true;
            this.radioAppLaunch.Enabled = false;
            this.radioAppLaunch.Location = new System.Drawing.Point(35, 70);
            this.radioAppLaunch.Name = "radioAppLaunch";
            this.radioAppLaunch.Size = new System.Drawing.Size(143, 17);
            this.radioAppLaunch.TabIndex = 20;
            this.radioAppLaunch.TabStop = true;
            this.radioAppLaunch.Text = "Execute on every launch";
            this.radioAppLaunch.UseVisualStyleBackColor = true;
            // 
            // WorkflowTriggerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(291, 262);
            this.Controls.Add(this.radioAppLaunch);
            this.Controls.Add(this.radioFirstAppLaunch);
            this.Controls.Add(this.chkAppLaunch);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblHeader);
            this.Name = "WorkflowTriggerView";
            this.Text = "Add Trigger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox chkAppLaunch;
        private System.Windows.Forms.RadioButton radioFirstAppLaunch;
        private System.Windows.Forms.RadioButton radioAppLaunch;
    }
}