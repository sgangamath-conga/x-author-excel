namespace Apttus.XAuthor.AppDesigner
{
    partial class ShowFilterObjectView
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
            this.tblObjectConfiguration = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tblObjectConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tblObjectConfiguration
            // 
            this.tblObjectConfiguration.AutoSize = true;
            this.tblObjectConfiguration.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblObjectConfiguration.ColumnCount = 3;
            this.tblObjectConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblObjectConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblObjectConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblObjectConfiguration.Controls.Add(this.label1, 2, 0);
            this.tblObjectConfiguration.Controls.Add(this.label2, 1, 0);
            this.tblObjectConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblObjectConfiguration.Location = new System.Drawing.Point(0, 0);
            this.tblObjectConfiguration.Name = "tblObjectConfiguration";
            this.tblObjectConfiguration.RowCount = 1;
            this.tblObjectConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblObjectConfiguration.Size = new System.Drawing.Size(251, 25);
            this.tblObjectConfiguration.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter Location in Excel";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "DisplayMap Object";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ShowFilterObjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tblObjectConfiguration);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ShowFilterObjectView";
            this.Size = new System.Drawing.Size(251, 25);
            this.Load += new System.EventHandler(this.ShowFilterObjectView_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.ShowFilterObjectView_Validating);
            this.tblObjectConfiguration.ResumeLayout(false);
            this.tblObjectConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblObjectConfiguration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
