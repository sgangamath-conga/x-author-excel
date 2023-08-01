namespace Apttus.XAuthor.AppDesigner
{
    partial class FieldMappingView
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
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.tblFieldsMapping = new System.Windows.Forms.TableLayoutPanel();
            this.lblSourceFields = new System.Windows.Forms.Label();
            this.lblTargetFields = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFieldsMapping = new System.Windows.Forms.Label();
            this.lblObject = new System.Windows.Forms.Label();
            this.tblMain.SuspendLayout();
            this.tblFieldsMapping.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblMain
            // 
            this.tblMain.AutoSize = true;
            this.tblMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMain.Controls.Add(this.tblFieldsMapping, 0, 1);
            this.tblMain.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(186, 57);
            this.tblMain.TabIndex = 0;
            // 
            // tblFieldsMapping
            // 
            this.tblFieldsMapping.AutoSize = true;
            this.tblFieldsMapping.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFieldsMapping.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblFieldsMapping.ColumnCount = 4;
            this.tblFieldsMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFieldsMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFieldsMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFieldsMapping.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFieldsMapping.Controls.Add(this.lblSourceFields, 1, 0);
            this.tblFieldsMapping.Controls.Add(this.lblTargetFields, 3, 0);
            this.tblFieldsMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFieldsMapping.Location = new System.Drawing.Point(3, 30);
            this.tblFieldsMapping.Name = "tblFieldsMapping";
            this.tblFieldsMapping.RowCount = 2;
            this.tblFieldsMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFieldsMapping.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFieldsMapping.Size = new System.Drawing.Size(180, 24);
            this.tblFieldsMapping.TabIndex = 3;
            // 
            // lblSourceFields
            // 
            this.lblSourceFields.AutoSize = true;
            this.lblSourceFields.Location = new System.Drawing.Point(5, 4);
            this.lblSourceFields.Margin = new System.Windows.Forms.Padding(3);
            this.lblSourceFields.Name = "lblSourceFields";
            this.lblSourceFields.Size = new System.Drawing.Size(76, 15);
            this.lblSourceFields.TabIndex = 0;
            this.lblSourceFields.Text = "Source Fields";
            // 
            // lblTargetFields
            // 
            this.lblTargetFields.AutoSize = true;
            this.lblTargetFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetFields.Location = new System.Drawing.Point(89, 4);
            this.lblTargetFields.Margin = new System.Windows.Forms.Padding(3);
            this.lblTargetFields.MaximumSize = new System.Drawing.Size(417, 180);
            this.lblTargetFields.Name = "lblTargetFields";
            this.lblTargetFields.Size = new System.Drawing.Size(87, 15);
            this.lblTargetFields.TabIndex = 0;
            this.lblTargetFields.Text = "Target Fields";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblFieldsMapping);
            this.flowLayoutPanel1.Controls.Add(this.lblObject);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(180, 21);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // lblFieldsMapping
            // 
            this.lblFieldsMapping.AutoSize = true;
            this.lblFieldsMapping.Location = new System.Drawing.Point(3, 3);
            this.lblFieldsMapping.Margin = new System.Windows.Forms.Padding(3);
            this.lblFieldsMapping.Name = "lblFieldsMapping";
            this.lblFieldsMapping.Size = new System.Drawing.Size(94, 15);
            this.lblFieldsMapping.TabIndex = 1;
            this.lblFieldsMapping.Text = "Field Mappings :";
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Location = new System.Drawing.Point(103, 3);
            this.lblObject.Margin = new System.Windows.Forms.Padding(3);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(74, 15);
            this.lblObject.TabIndex = 2;
            this.lblObject.Text = "ObjectName";
            // 
            // FieldMappingView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tblMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FieldMappingView";
            this.Size = new System.Drawing.Size(186, 57);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.tblFieldsMapping.ResumeLayout(false);
            this.tblFieldsMapping.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.TableLayoutPanel tblFieldsMapping;
        private System.Windows.Forms.Label lblSourceFields;
        private System.Windows.Forms.Label lblTargetFields;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblFieldsMapping;
        private System.Windows.Forms.Label lblObject;
    }
}
