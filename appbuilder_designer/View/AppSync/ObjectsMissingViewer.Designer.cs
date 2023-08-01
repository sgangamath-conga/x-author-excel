namespace Apttus.XAuthor.AppDesigner
{
    partial class ObjectsMissingViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectsMissingViewer));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.DgvObjectsMissing = new System.Windows.Forms.DataGridView();
            this.lblMissingObjects = new System.Windows.Forms.Label();
            this.lblMissingObjectDescription = new System.Windows.Forms.Label();
            this.ObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvObjectsMissing)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.DgvObjectsMissing, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblMissingObjects, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMissingObjectDescription, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1264, 263);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // DgvObjectsMissing
            // 
            this.DgvObjectsMissing.AllowUserToAddRows = false;
            this.DgvObjectsMissing.AllowUserToDeleteRows = false;
            this.DgvObjectsMissing.AllowUserToOrderColumns = true;
            this.DgvObjectsMissing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DgvObjectsMissing.BackgroundColor = System.Drawing.Color.White;
            this.DgvObjectsMissing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvObjectsMissing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ObjectName,
            this.ObjectID});
            this.DgvObjectsMissing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvObjectsMissing.GridColor = System.Drawing.Color.White;
            this.DgvObjectsMissing.Location = new System.Drawing.Point(3, 68);
            this.DgvObjectsMissing.Name = "DgvObjectsMissing";
            this.DgvObjectsMissing.ReadOnly = true;
            this.DgvObjectsMissing.RowHeadersVisible = false;
            this.DgvObjectsMissing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvObjectsMissing.Size = new System.Drawing.Size(1258, 192);
            this.DgvObjectsMissing.TabIndex = 7;
            // 
            // lblMissingObjects
            // 
            this.lblMissingObjects.AutoSize = true;
            this.lblMissingObjects.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMissingObjects.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblMissingObjects.Location = new System.Drawing.Point(5, 5);
            this.lblMissingObjects.Margin = new System.Windows.Forms.Padding(5);
            this.lblMissingObjects.Name = "lblMissingObjects";
            this.lblMissingObjects.Size = new System.Drawing.Size(302, 30);
            this.lblMissingObjects.TabIndex = 5;
            this.lblMissingObjects.Text = "Missing or Inaccessible Objects";
            // 
            // lblMissingObjectDescription
            // 
            this.lblMissingObjectDescription.AutoSize = true;
            this.lblMissingObjectDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMissingObjectDescription.Location = new System.Drawing.Point(5, 45);
            this.lblMissingObjectDescription.Margin = new System.Windows.Forms.Padding(5);
            this.lblMissingObjectDescription.Name = "lblMissingObjectDescription";
            this.lblMissingObjectDescription.Size = new System.Drawing.Size(1254, 15);
            this.lblMissingObjectDescription.TabIndex = 1;
            this.lblMissingObjectDescription.Text = resources.GetString("lblMissingObjectDescription.Text");
            // 
            // ObjectName
            // 
            this.ObjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ObjectName.DataPropertyName = "Name";
            this.ObjectName.HeaderText = "Object Name";
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.ReadOnly = true;
            // 
            // ObjectID
            // 
            this.ObjectID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ObjectID.DataPropertyName = "Id";
            this.ObjectID.HeaderText = "Object ID";
            this.ObjectID.Name = "ObjectID";
            this.ObjectID.ReadOnly = true;
            // 
            // ObjectsMissingViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ObjectsMissingViewer";
            this.Size = new System.Drawing.Size(1264, 263);
            this.Load += new System.EventHandler(this.ObjectsMissingViewer_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvObjectsMissing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblMissingObjects;
        private System.Windows.Forms.Label lblMissingObjectDescription;
        private System.Windows.Forms.DataGridView DgvObjectsMissing;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectID;
    }
}
