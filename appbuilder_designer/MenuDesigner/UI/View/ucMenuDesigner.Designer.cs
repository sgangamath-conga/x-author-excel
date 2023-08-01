namespace Apttus.XAuthor.AppDesigner.MenuDesigner.UI.View
{
    partial class ucMenuDesigner
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.MnuTree = new System.Windows.Forms.TreeView();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblProp = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.cmbMenuSelector = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.Controls.Add(this.MnuTree);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.lblProp);
            this.panel1.Controls.Add(this.propertyGrid1);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnCreate);
            this.panel1.Controls.Add(this.cmbMenuSelector);
            this.panel1.Location = new System.Drawing.Point(28, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(299, 660);
            this.panel1.TabIndex = 16;
            // 
            // MnuTree
            // 
            this.MnuTree.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.MnuTree.Location = new System.Drawing.Point(23, 38);
            this.MnuTree.Name = "MnuTree";
            this.MnuTree.Size = new System.Drawing.Size(261, 243);
            this.MnuTree.TabIndex = 18;
            this.MnuTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MnuTree_AfterSelect_1);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(223, 594);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(61, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Tag = "btnSave";
            this.btnSave.Text = "xlate me";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblProp
            // 
            this.lblProp.AutoSize = true;
            this.lblProp.Location = new System.Drawing.Point(26, 270);
            this.lblProp.Name = "lblProp";
            this.lblProp.Size = new System.Drawing.Size(72, 13);
            this.lblProp.TabIndex = 16;
            this.lblProp.Tag = "lblProperties";
            this.lblProp.Text = "Properties      ";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.propertyGrid1.Location = new System.Drawing.Point(23, 287);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(261, 289);
            this.propertyGrid1.TabIndex = 11;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged_1);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Location = new System.Drawing.Point(225, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 23);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Tag = "btnDelete";
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCreate.Location = new System.Drawing.Point(159, 0);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(59, 23);
            this.btnCreate.TabIndex = 13;
            this.btnCreate.Tag = "btnCreate";
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // cmbMenuSelector
            // 
            this.cmbMenuSelector.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbMenuSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMenuSelector.FormattingEnabled = true;
            this.cmbMenuSelector.Location = new System.Drawing.Point(24, 1);
            this.cmbMenuSelector.Name = "cmbMenuSelector";
            this.cmbMenuSelector.Size = new System.Drawing.Size(129, 21);
            this.cmbMenuSelector.TabIndex = 8;
            this.cmbMenuSelector.SelectedIndexChanged += new System.EventHandler(this.cmbMenuSelector_SelectedIndexChanged_1);
            // 
            // ucMenuDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Controls.Add(this.panel1);
            this.Name = "ucMenuDesigner";
            this.Size = new System.Drawing.Size(355, 684);
            this.Tag = "";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProp;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.ComboBox cmbMenuSelector;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TreeView MnuTree;
    }
}
