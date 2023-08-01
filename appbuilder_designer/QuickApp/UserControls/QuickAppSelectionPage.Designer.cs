namespace Apttus.XAuthor.AppDesigner
{
    partial class QuickAppSelectionPage
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.lblSelectAppType = new System.Windows.Forms.Label();
            this.lblListAppDescription = new System.Windows.Forms.Label();
            this.lblSingleObject = new System.Windows.Forms.Label();
            this.pbSingleObject = new System.Windows.Forms.PictureBox();
            this.rboListApp = new System.Windows.Forms.RadioButton();
            this.lblPCDescription = new System.Windows.Forms.Label();
            this.lblParentChild = new System.Windows.Forms.Label();
            this.pbParentChild = new System.Windows.Forms.PictureBox();
            this.rboMasterDetailApp = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pbSingleObject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParentChild)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "App Name :";
            // 
            // txtAppName
            // 
            this.txtAppName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtAppName.Location = new System.Drawing.Point(112, 3);
            this.txtAppName.Name = "txtAppName";
            this.txtAppName.Size = new System.Drawing.Size(340, 23);
            this.txtAppName.TabIndex = 2;
            // 
            // lblSelectAppType
            // 
            this.lblSelectAppType.AutoSize = true;
            this.lblSelectAppType.Location = new System.Drawing.Point(6, 35);
            this.lblSelectAppType.Margin = new System.Windows.Forms.Padding(6);
            this.lblSelectAppType.Name = "lblSelectAppType";
            this.lblSelectAppType.Size = new System.Drawing.Size(97, 15);
            this.lblSelectAppType.TabIndex = 4;
            this.lblSelectAppType.Text = "Select App Type :";
            // 
            // lblListAppDescription
            // 
            this.lblListAppDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblListAppDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListAppDescription.Location = new System.Drawing.Point(3, 67);
            this.lblListAppDescription.Margin = new System.Windows.Forms.Padding(3);
            this.lblListAppDescription.Name = "lblListAppDescription";
            this.lblListAppDescription.Size = new System.Drawing.Size(225, 200);
            this.lblListAppDescription.TabIndex = 1;
            this.lblListAppDescription.Text = "Allows for retrieval, editing, and saving of Salesforce data for a list of record" +
    "s for one object based on user selected fields.";
            this.lblListAppDescription.Click += new System.EventHandler(this.lblListAppDescription_Click);
            // 
            // lblSingleObject
            // 
            this.lblSingleObject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSingleObject.AutoSize = true;
            this.lblSingleObject.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblSingleObject.Location = new System.Drawing.Point(84, 19);
            this.lblSingleObject.Name = "lblSingleObject";
            this.lblSingleObject.Size = new System.Drawing.Size(125, 20);
            this.lblSingleObject.TabIndex = 0;
            this.lblSingleObject.Text = "Single Object List";
            this.lblSingleObject.Click += new System.EventHandler(this.lblSingleObject_Click);
            // 
            // pbSingleObject
            // 
            this.pbSingleObject.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.single_object_list;
            this.pbSingleObject.Location = new System.Drawing.Point(19, 0);
            this.pbSingleObject.Margin = new System.Windows.Forms.Padding(0);
            this.pbSingleObject.Name = "pbSingleObject";
            this.pbSingleObject.Size = new System.Drawing.Size(62, 58);
            this.pbSingleObject.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSingleObject.TabIndex = 0;
            this.pbSingleObject.TabStop = false;
            // 
            // rboListApp
            // 
            this.rboListApp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rboListApp.AutoSize = true;
            this.rboListApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rboListApp.Location = new System.Drawing.Point(3, 23);
            this.rboListApp.Name = "rboListApp";
            this.rboListApp.Size = new System.Drawing.Size(13, 12);
            this.rboListApp.TabIndex = 0;
            this.rboListApp.UseVisualStyleBackColor = true;
            this.rboListApp.CheckedChanged += new System.EventHandler(this.rboListApp_CheckedChanged);
            // 
            // lblPCDescription
            // 
            this.lblPCDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPCDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPCDescription.Location = new System.Drawing.Point(3, 67);
            this.lblPCDescription.Margin = new System.Windows.Forms.Padding(3);
            this.lblPCDescription.Name = "lblPCDescription";
            this.lblPCDescription.Size = new System.Drawing.Size(225, 200);
            this.lblPCDescription.TabIndex = 5;
            this.lblPCDescription.Text = "Allows for retrieval, editing, and saving of Salesforce data for both a parent re" +
    "cord and a list of child object records based on user selected fields.";
            this.lblPCDescription.Click += new System.EventHandler(this.lblPCDescription_Click);
            // 
            // lblParentChild
            // 
            this.lblParentChild.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblParentChild.AutoSize = true;
            this.lblParentChild.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblParentChild.Location = new System.Drawing.Point(84, 19);
            this.lblParentChild.Name = "lblParentChild";
            this.lblParentChild.Size = new System.Drawing.Size(93, 20);
            this.lblParentChild.TabIndex = 2;
            this.lblParentChild.Text = "Parent Child";
            this.lblParentChild.Click += new System.EventHandler(this.lblParentChild_Click);
            // 
            // pbParentChild
            // 
            this.pbParentChild.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.parent_child;
            this.pbParentChild.Location = new System.Drawing.Point(19, 0);
            this.pbParentChild.Margin = new System.Windows.Forms.Padding(0);
            this.pbParentChild.Name = "pbParentChild";
            this.pbParentChild.Size = new System.Drawing.Size(62, 58);
            this.pbParentChild.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbParentChild.TabIndex = 3;
            this.pbParentChild.TabStop = false;
            // 
            // rboMasterDetailApp
            // 
            this.rboMasterDetailApp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rboMasterDetailApp.AutoSize = true;
            this.rboMasterDetailApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rboMasterDetailApp.Location = new System.Drawing.Point(3, 23);
            this.rboMasterDetailApp.Name = "rboMasterDetailApp";
            this.rboMasterDetailApp.Size = new System.Drawing.Size(13, 12);
            this.rboMasterDetailApp.TabIndex = 4;
            this.rboMasterDetailApp.UseVisualStyleBackColor = true;
            this.rboMasterDetailApp.CheckedChanged += new System.EventHandler(this.rboMasterDetailApp_CheckedChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(489, 350);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.lblSelectAppType, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtAppName, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(483, 56);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel8, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 65);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(483, 282);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblListAppDescription, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 270);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Click += new System.EventHandler(this.pnlListApp_Click);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.rboListApp, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.lblSingleObject, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.pbSingleObject, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(225, 58);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.lblPCDescription, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(246, 6);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(231, 270);
            this.tableLayoutPanel8.TabIndex = 1;
            this.tableLayoutPanel8.Click += new System.EventHandler(this.pnlMasterChild_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.lblParentChild, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.pbParentChild, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.rboMasterDetailApp, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(225, 58);
            this.tableLayoutPanel5.TabIndex = 0;
            this.tableLayoutPanel5.Click += new System.EventHandler(this.pnlMasterChild_Click);
            // 
            // QuickAppSelectionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "QuickAppSelectionPage";
            this.Size = new System.Drawing.Size(489, 350);
            this.Load += new System.EventHandler(this.QuickAppSelectionPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbSingleObject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParentChild)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAppName;
        private System.Windows.Forms.Label lblSelectAppType;
        private System.Windows.Forms.Label lblListAppDescription;
        private System.Windows.Forms.Label lblSingleObject;
        private System.Windows.Forms.PictureBox pbSingleObject;
        private System.Windows.Forms.RadioButton rboListApp;
        private System.Windows.Forms.Label lblPCDescription;
        private System.Windows.Forms.Label lblParentChild;
        private System.Windows.Forms.PictureBox pbParentChild;
        private System.Windows.Forms.RadioButton rboMasterDetailApp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
    }
}
