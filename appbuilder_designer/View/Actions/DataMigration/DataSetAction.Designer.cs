namespace Apttus.XAuthor.AppDesigner
{
    partial class DataSetActionView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSetActionView));
            this.rboANDOperation = new System.Windows.Forms.RadioButton();
            this.rboOROperation = new System.Windows.Forms.RadioButton();
            this.cboTargetObject = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblOperation = new System.Windows.Forms.Label();
            this.lblTargetObject = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lbDataSet = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // rboANDOperation
            // 
            resources.ApplyResources(this.rboANDOperation, "rboANDOperation");
            this.rboANDOperation.Name = "rboANDOperation";
            this.rboANDOperation.UseVisualStyleBackColor = true;
            // 
            // rboOROperation
            // 
            resources.ApplyResources(this.rboOROperation, "rboOROperation");
            this.rboOROperation.Checked = true;
            this.rboOROperation.Name = "rboOROperation";
            this.rboOROperation.TabStop = true;
            this.rboOROperation.UseVisualStyleBackColor = true;
            // 
            // cboTargetObject
            // 
            resources.ApplyResources(this.cboTargetObject, "cboTargetObject");
            this.cboTargetObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetObject.FormattingEnabled = true;
            this.cboTargetObject.Name = "cboTargetObject";
            this.cboTargetObject.DropDown += new System.EventHandler(this.AdjustWidth_DropDown);
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
            // 
            // lblOperation
            // 
            resources.ApplyResources(this.lblOperation, "lblOperation");
            this.lblOperation.Name = "lblOperation";
            // 
            // lblTargetObject
            // 
            resources.ApplyResources(this.lblTargetObject, "lblTargetObject");
            this.lblTargetObject.Name = "lblTargetObject";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // lbDataSet
            // 
            resources.ApplyResources(this.lbDataSet, "lbDataSet");
            this.lbDataSet.ForeColor = System.Drawing.Color.Green;
            this.lbDataSet.Name = "lbDataSet";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lbDataSet, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTargetObject, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblOperation, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.cboTargetObject, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.rboOROperation);
            this.flowLayoutPanel1.Controls.Add(this.rboANDOperation);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Name = "panel3";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(244)))), ((int)(((byte)(224)))));
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(240)))), ((int)(((byte)(224)))));
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // DataSetActionView
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataSetActionView";
            this.Load += new System.EventHandler(this.DataSetAction_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rboANDOperation;
        private System.Windows.Forms.RadioButton rboOROperation;
        private System.Windows.Forms.ComboBox cboTargetObject;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label lblTargetObject;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lbDataSet;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

    }
}