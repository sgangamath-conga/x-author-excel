
namespace Apttus.IntelligentCloudV2.Login
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
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCloseWindow
            // 
            this.btnCloseWindow.BackColor = System.Drawing.Color.White;
            this.btnCloseWindow.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCloseWindow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCloseWindow.Location = new System.Drawing.Point(247, 384);
            this.btnCloseWindow.Name = "btnCloseWindow";
            this.btnCloseWindow.Size = new System.Drawing.Size(89, 26);
            this.btnCloseWindow.TabIndex = 16;
            this.btnCloseWindow.Text = "Close";
            this.btnCloseWindow.UseVisualStyleBackColor = false;
            this.btnCloseWindow.Click += new System.EventHandler(this.btnCloseWindow_Click);
            // 
            // btnAuthorize
            // 
            this.btnAuthorize.BackColor = System.Drawing.Color.White;
            this.btnAuthorize.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAuthorize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAuthorize.Location = new System.Drawing.Point(451, 23);
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Size = new System.Drawing.Size(101, 28);
            this.btnAuthorize.TabIndex = 15;
            this.btnAuthorize.Text = "Authorize";
            this.btnAuthorize.UseVisualStyleBackColor = false;
            this.btnAuthorize.Click += new System.EventHandler(this.btnAuthorize_Click);
            // 
            // txtLoginURL
            // 
            this.txtLoginURL.BackColor = System.Drawing.Color.White;
            this.txtLoginURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtLoginURL.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLoginURL.Location = new System.Drawing.Point(205, 25);
            this.txtLoginURL.Name = "txtLoginURL";
            this.txtLoginURL.Size = new System.Drawing.Size(224, 20);
            this.txtLoginURL.TabIndex = 14;
            this.txtLoginURL.Enter += new System.EventHandler(this.txtLoginURL_Enter);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtName.Location = new System.Drawing.Point(6, 25);
            this.txtName.MaxLength = 25;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(180, 20);
            this.txtName.TabIndex = 13;
            // 
            // tblConnections
            // 
            this.tblConnections.AutoScroll = true;
            this.tblConnections.ColumnCount = 4;
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tblConnections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblConnections.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.tblConnections.Location = new System.Drawing.Point(3, 17);
            this.tblConnections.Name = "tblConnections";
            this.tblConnections.RowCount = 1;
            this.tblConnections.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblConnections.Size = new System.Drawing.Size(559, 275);
            this.tblConnections.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tblConnections);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(8, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(565, 295);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Available connections";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtLoginURL);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.btnAuthorize);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(8, 313);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 65);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create a new connection";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ApttusManageConnections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(582, 418);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCloseWindow);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApttusManageConnections";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Connections";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApttusManageConnections_FormClosing);
            this.Load += new System.EventHandler(this.ApttusManageConnections_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

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

    }
}