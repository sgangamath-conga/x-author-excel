namespace Apttus.XAuthor.AppRuntime
{
    partial class UserInputUserControl
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.controlTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tblFilterTemplatePanel = new System.Windows.Forms.TableLayoutPanel();
            this.mainTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.Controls.Add(this.controlTableLayoutPanel, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.tblFilterTemplatePanel, 0, 0);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 2;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(6, 18);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // controlTableLayoutPanel
            // 
            this.controlTableLayoutPanel.AutoSize = true;
            this.controlTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlTableLayoutPanel.ColumnCount = 4;
            this.controlTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.controlTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.controlTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlTableLayoutPanel.Location = new System.Drawing.Point(3, 12);
            this.controlTableLayoutPanel.Name = "controlTableLayoutPanel";
            this.controlTableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.controlTableLayoutPanel.RowCount = 1;
            this.controlTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.controlTableLayoutPanel.Size = new System.Drawing.Size(1, 3);
            this.controlTableLayoutPanel.TabIndex = 5;
            // 
            // tblFilterTemplatePanel
            // 
            this.tblFilterTemplatePanel.AutoSize = true;
            this.tblFilterTemplatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFilterTemplatePanel.ColumnCount = 4;
            this.tblFilterTemplatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilterTemplatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblFilterTemplatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilterTemplatePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblFilterTemplatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFilterTemplatePanel.Location = new System.Drawing.Point(3, 3);
            this.tblFilterTemplatePanel.Name = "tblFilterTemplatePanel";
            this.tblFilterTemplatePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.tblFilterTemplatePanel.RowCount = 1;
            this.tblFilterTemplatePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilterTemplatePanel.Size = new System.Drawing.Size(1, 3);
            this.tblFilterTemplatePanel.TabIndex = 4;
            // 
            // UserInputUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UserInputUserControl";
            this.Size = new System.Drawing.Size(6, 18);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel controlTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tblFilterTemplatePanel;
    }
}