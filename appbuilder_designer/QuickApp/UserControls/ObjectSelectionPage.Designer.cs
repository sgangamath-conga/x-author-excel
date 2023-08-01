namespace Apttus.XAuthor.AppDesigner
{
    partial class ObjectSelectionPage
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblObject = new System.Windows.Forms.Label();
            this.cboObject = new System.Windows.Forms.ComboBox();
            this.threadToUpdateFields = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.apttusFieldsView1 = new Apttus.XAuthor.AppDesigner.ApttusFieldsView();
            this.lblLoadingFields = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Window;
            this.pnlMain.Controls.Add(this.tableLayoutPanel1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(558, 324);
            this.pnlMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 324);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.ColumnCount = 2;
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panel2.Controls.Add(this.lblObject);
            this.panel2.Controls.Add(this.cboObject, 1, 0);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 1);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.Name = "panel2";
            this.panel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panel2.Size = new System.Drawing.Size(556, 34);
            this.panel2.TabIndex = 1;
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Location = new System.Drawing.Point(7, 6);
            this.lblObject.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(105, 20);
            this.lblObject.TabIndex = 0;
            this.lblObject.Text = "Parent Object :";
            // 
            // cboObject
            // 
            this.cboObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboObject.FormattingEnabled = true;
            this.cboObject.Location = new System.Drawing.Point(122, 3);
            this.cboObject.Name = "cboObject";
            this.cboObject.Size = new System.Drawing.Size(240, 28);
            this.cboObject.TabIndex = 2;
            // 
            // threadToUpdateFields
            // 
            this.threadToUpdateFields.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.threadToUpdateFields_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.lblLoadingFields);
            this.panel1.Controls.Add(this.apttusFieldsView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 37);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 286);
            this.panel1.TabIndex = 0;
            // 
            // apttusFieldsView1
            // 
            this.apttusFieldsView1.AutoScroll = true;
            this.apttusFieldsView1.AutoSize = true;
            this.apttusFieldsView1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.apttusFieldsView1.BackColor = System.Drawing.SystemColors.Window;
            this.apttusFieldsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apttusFieldsView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apttusFieldsView1.Location = new System.Drawing.Point(0, 0);
            this.apttusFieldsView1.Name = "apttusFieldsView1";
            this.apttusFieldsView1.Size = new System.Drawing.Size(556, 286);
            this.apttusFieldsView1.TabIndex = 0;
            // 
            // lblLoadingFields
            // 
            this.lblLoadingFields.AutoSize = true;
            this.lblLoadingFields.BackColor = System.Drawing.SystemColors.Window;
            this.lblLoadingFields.Location = new System.Drawing.Point(28, 76);
            this.lblLoadingFields.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLoadingFields.Name = "lblLoadingFields";
            this.lblLoadingFields.Size = new System.Drawing.Size(197, 20);
            this.lblLoadingFields.TabIndex = 5;
            this.lblLoadingFields.Text = "Loading Fields. Please Wait...";
            this.lblLoadingFields.Visible = false;
            // 
            // ObjectSelectionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ObjectSelectionPage";
            this.Size = new System.Drawing.Size(558, 324);
            this.Load += new System.EventHandler(this.ObjectSelectionPage_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblObject;
        private System.Windows.Forms.ComboBox cboObject;
        private System.ComponentModel.BackgroundWorker threadToUpdateFields;
        private System.Windows.Forms.TableLayoutPanel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblLoadingFields;
        private ApttusFieldsView apttusFieldsView1;

    }
}
