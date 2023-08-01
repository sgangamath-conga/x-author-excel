/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class CallProcedure : Form
    {
        CallProcedureController controller;
        private List<ApttusObject> objectList = new List<ApttusObject>();

        private Dictionary<MethodParam.ParamType, string> paramType = new Dictionary<MethodParam.ParamType, string>();
        private static bool blnAddRow = false;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public CallProcedure()
        {
            // TODO:: Add validations using ErrorProvider
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;

            paramType.Add(MethodParam.ParamType.Static, "Static");
            paramType.Add(MethodParam.ParamType.Object, "Object");
            paramType.Add(MethodParam.ParamType.Field, "Field");
            paramType.Add(MethodParam.ParamType.UserInput, "UserInput");
            groupParams.Visible = false;
        }

        private void SetCultureData()
        {
            label3.Text = resourceManager.GetResource("CALLPROCACTION_label3_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            chkEnableCache.Text = resourceManager.GetResource("CALLPROCACTION_chkEnableCache_Text");
            chkHasParams.Text = resourceManager.GetResource("CALLPROCACTION_chkHasParams_Text");
            chkReturnValue.Text = resourceManager.GetResource("CALLPROCACTION_chkReturnValue_Text");
            groupBox1.Text = resourceManager.GetResource("CALLPROCACTION_groupBox1_Text");
            groupMethod.Text = resourceManager.GetResource("CALLPROCACTION_groupMethod_Text");
            groupParams.Text = resourceManager.GetResource("CALLPROCACTION_groupParams_Text");
            label1.Text = resourceManager.GetResource("CALLPROCACTION_label1_Text");
            label2.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            label3.Text = resourceManager.GetResource("CALLPROCACTION_label3_Text");
            lblClassName.Text = resourceManager.GetResource("CALLPROCACTION_lblClassName_Text");
            lblReturnType.Text = resourceManager.GetResource("CALLPROCACTION_lblReturnType_Text");
            lnkAddParam.Text = resourceManager.GetResource("CALLPROCACTION_lnkAddParam_Text");
            lnkClearAll.Text = resourceManager.GetResource("COMMON_ClearAll_Text");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public void SetController(CallProcedureController controller)
        {
            this.controller = controller;
            objectList = controller.GetApplicationObjects();
            PopulateObject(cboReturnObject);
            this.controller.SetView();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PopulateClass()
        {
            ObjectManager objectManager = ObjectManager.GetInstance;

            List<ApttusObject> lstClass = objectManager.Query(new SalesforceQuery { SOQL = Constants.CALLPROCEDURE_SELECT });

            List<ApttusObject> SortedList = lstClass.OrderBy(item => item.Name).ToList();
            
            //Dictionary<string, string> dictClass = lstClass.ToDictionary(x => x.Id, x => x.Name);
            //cboClass.DataSource = new BindingSource(dictClass, null);
            //cboClass.DisplayMember = "Value";
            //cboClass.ValueMember = "Key";


            cboClass.DataSource = new BindingSource(SortedList, null);
            
            cboClass.DisplayMember = Constants.NAME_ATTRIBUTE;
            cboClass.ValueMember = Constants.UNIQUEID_ATTRIBUTE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        internal void FillForm(CallProcedureModel model)
        {
            txtActionName.Text = model.Name;
            cboClass.SelectedIndex = cboClass.FindStringExact(model.ClassName);
            txtMethod.Text = model.MethodName;
            chkEnableCache.Checked = model.EnableCache;
            if (model.ReturnObject != Guid.Empty)
            {
                chkReturnValue.Checked = true;
                cboReturnObject.SelectedValue = model.ReturnObject;
            }


            if (model.MethodParams != null && model.MethodParams.Count > 0)
                chkHasParams.Checked = true;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        internal void PopulateParams(CallProcedureModel model, bool bAddRow = false)
        {
            try
            {
                if (!bAddRow)
                {
                    ResetTableLayout();
                    //tlpParams.Height = 30;
                }

                int paramCount = 1;  //model == null ? 1 : model.methodParams.Count;

                if (model != null && model.MethodParams.Count > 0)
                {
                    paramCount = model.MethodParams.Count;
                }

                for (int i = 0; i < paramCount; i++)
                {
                    TextBox txtParam = new TextBox();
                    txtParam.Name = "txtParam" + i.ToString();
                    txtParam.Text = string.Empty;
                    txtParam.Dock = DockStyle.Fill;
                    txtParam.Font = new System.Drawing.Font("Segoe UI", 9F);

                    ComboBox cmbType = new ComboBox();
                    cmbType.Name = "cmbType" + i.ToString();
                    cmbType.Dock = DockStyle.Fill;
                    cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbType.DropDown += new EventHandler(AdjustWidth_DropDown);
                    cmbType.DataSource = new BindingSource(paramType, null);
                    cmbType.DisplayMember = "value";
                    cmbType.ValueMember = "key";

                    TextBox txtParamVal = new TextBox();
                    txtParamVal.Name = "txtVal" + i.ToString();
                    txtParamVal.Text = string.Empty;
                    txtParamVal.Dock = DockStyle.Fill;
                    txtParamVal.Font = new System.Drawing.Font("Segoe UI", 9F);

                    ComboBox cmbObject = new ComboBox();
                    cmbObject.Name = "cmbObject" + i.ToString();
                    cmbObject.Dock = DockStyle.Fill;
                    cmbObject.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbObject.Font = new System.Drawing.Font("Segoe UI", 9F);
                    cmbObject.SelectedIndexChanged += new EventHandler(cmbObject_SelectedIndexChanged);
                    cmbObject.DropDown += new EventHandler(AdjustWidth_DropDown);
                    //cmbObject.CreateControl();

                    ComboBox cmbField = new ComboBox();
                    cmbField.Name = "cmbField" + i.ToString();
                    cmbField.Dock = DockStyle.Fill;
                    cmbField.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbField.DropDown += new EventHandler(AdjustWidth_DropDown);
                    cmbField.Font = new System.Drawing.Font("Segoe UI", 9F);
                    //cmbField.CreateControl();                

                    // Uncomment below to Enable Chunking
                    //ComboBox cmbChunkBy = new ComboBox();
                    //cmbChunkBy.Name = "txtChunkBy" + i.ToString();
                    //cmbChunkBy.Text = string.Empty;
                    //cmbChunkBy.Dock = DockStyle.Fill;
                    //cmbChunkBy.DropDownStyle = ComboBoxStyle.DropDownList;
                    //cmbChunkBy.Font = new System.Drawing.Font("Segoe UI", 9F);
                    //txtChunkBy.Visible = false;

                    tlpParams.Controls.Add(txtParam, 0, bAddRow ? tlpParams.RowCount - 1 : i);
                    tlpParams.Controls.Add(cmbType, 1, bAddRow ? tlpParams.RowCount - 1 : i);
                    tlpParams.Controls.Add(txtParamVal, 2, bAddRow ? tlpParams.RowCount - 1 : i);
                    tlpParams.Controls.Add(cmbObject, 3, bAddRow ? tlpParams.RowCount - 1 : i);
                    tlpParams.Controls.Add(cmbField, 4, bAddRow ? tlpParams.RowCount - 1 : i);
                    // Uncomment below to Enable Chunking
                    //tlpParams.Controls.Add(cmbChunkBy, 5, bAddRow ? tlpParams.RowCount - 1 : i);

                    tlpParams.RowCount++;
                    //tlpParams.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

                    PopulateObject(cmbObject);
                    PopulateFields(cmbObject);
                    cmbType.DataSource = Enum.GetValues(typeof(MethodParam.ParamType));
                    cmbType.SelectedValueChanged += cmbType_SelectedValueChanged;

                    // Uncomment below to Enable Chunking
                    //cmbChunkBy.Items.AddRange(new object[] { 1, 2, 3, 4, 5 });

                    if (model != null && model.MethodParams.Count > 0)
                    {
                        // Populate data here.
                        txtParam.Text = model.MethodParams[i].ParamName;
                        if (model.MethodParams[i].ParamObject != null)
                        {
                            cmbObject.SelectedItem = cmbObject.Items.OfType<ApttusObject>().SingleOrDefault(f => f.UniqueId.Equals(Guid.Parse(model.MethodParams[i].ParamObject)));
                        }
                        //cmbObject.SelectedIndex = cmbObject.FindStringExact(model.MethodParams[i].ParamObject);
                        if (model.MethodParams[i].ParamField != null)
                        {
                            //cmbField.SelectedIndex = cmbField.FindStringExact(model.MethodParams[i].ParamField);
                            cmbField.SelectedValue = model.MethodParams[i].ParamField;
                        }
                        txtParamVal.Text = model.MethodParams[i].ParamValue;

                        cmbType.SelectedItem = model.MethodParams[i].Type;
                        //cmbType_SelectedValueChanged.SelectedIndex = model.MethodParams[i].IsParamValue;                        
                    }

                    switch ((MethodParam.ParamType)cmbType.SelectedValue)
                    {
                        case (MethodParam.ParamType.Static):
                            cmbField.Visible = false;
                            //cboClass.Visible = false;
                            cmbObject.Visible = false;
                            txtParamVal.Visible = true;
                            // Uncomment below to Enable Chunking
                            //cmbChunkBy.Visible = false;
                            break;

                        default:
                            break;
                    }
                }
               // tlpParams.Height = tlpParams.Height + (25 * paramCount);
                lnkAddParam.Visible = tlpParams.RowCount <= Constants.CALL_PROCEDURE_MAX_ROWS;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        void cmbType_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cmbType = sender as ComboBox;
            int row = tlpParams.GetRow(cmbType);
            //ComboBox cmbObject = tlpParams.GetControlFromPosition(1, row) as ComboBox;            
            ComboBox cboObjects = tlpParams.GetControlFromPosition(3, row) as ComboBox;
            Control cboFields = tlpParams.GetControlFromPosition(4, row) as Control;
            TextBox txtParamVal = tlpParams.GetControlFromPosition(2, row) as TextBox;
            // Uncomment below to Enable Chunking
            //ComboBox cboChunkBy = tlpParams.GetControlFromPosition(5, row) as ComboBox;  


            
            switch ((MethodParam.ParamType)cmbType.SelectedValue)
            {
                case (MethodParam.ParamType.Static):
                    cboObjects.Visible = false;
                    cboFields.Visible = false;
                    //cmbObject.Visible = false;
                    txtParamVal.Visible = true;
                    // Uncomment below to Enable Chunking
                    //cboChunkBy.Visible = false;
                    break;

                case (MethodParam.ParamType.Object):
                    cboObjects.Visible = true;
                    cboFields.Visible = false;
                    txtParamVal.Visible = false;
                    break;

                case (MethodParam.ParamType.UserInput):
                    cboObjects.Visible = true;
                    cboFields.Visible = true;
                    txtParamVal.Visible = false;                   
                    break;
                case (MethodParam.ParamType.Field):
                    cboObjects.Visible = true;
                    cboFields.Visible = true;
                    if (cboFields.GetType() == typeof(CheckBox))
                    {
                        // Remove check box
                        tlpParams.Controls.Remove(cboFields);
                        // Add fields drop-down
                        ComboBox cmbField = new ComboBox();
                        cmbField.Name = "cmbField" + row.ToString();
                        cmbField.Dock = DockStyle.Fill;
                        cmbField.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbField.DropDown += new EventHandler(AdjustWidth_DropDown);
                        cmbField.Font = new System.Drawing.Font("Segoe UI", 9F);
                        tlpParams.Controls.Add(cmbField, 4, row);
                        PopulateFields(cboObjects);
                    }

                    txtParamVal.Visible = false;
                    // Uncomment below to Enable Chunking
                    //cboChunkBy.Visible = false;
                    break;

                default:
                    break;
            }
            
        }

        void chkChunk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkChunk = sender as CheckBox;
            int row = tlpParams.GetRow(chkChunk);

            ComboBox cboChunkBy = tlpParams.GetControlFromPosition(5, row) as ComboBox;
            cboChunkBy.Visible = chkChunk.Checked;

            if (chkChunk.Checked && row < controller.model.MethodParams.Count)
                cboChunkBy.SelectedItem = controller.model.MethodParams[row].ChunkParamBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cboObject"></param>
        private void PopulateObject(ComboBox cboObject)
        {
            BindingSource src = new BindingSource(objectList, null);

            cboObject.DataSource = src;
            cboObject.DisplayMember = Constants.NAME_ATTRIBUTE;
            cboObject.ValueMember = Constants.UNIQUEID_ATTRIBUTE;

            src.ResetBindings(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateFields(sender as ComboBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cboObject"></param>
        private void PopulateFields(ComboBox cboObject)
        {
            ApttusObject apttusObject = cboObject.SelectedItem as ApttusObject;

            if (apttusObject != null)
            {
                //SObjectDescribeInfo sObjectDescInfo = GetSObjectDescribeInfo(sObjectInfo);
                int row = tlpParams.GetRow(cboObject);

                if (row >= 0)
                {
                    ComboBox cboFields = tlpParams.GetControlFromPosition(4, row) as ComboBox;
                    if (cboFields != null)
                    {
                        cboFields.DataSource = new BindingSource(apttusObject.Fields, null);
                        cboFields.DisplayMember = Constants.NAME_ATTRIBUTE;
                        cboFields.ValueMember = Constants.ID_ATTRIBUTE;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetTableLayout()
        {
            tlpParams.RowStyles.Clear();
            tlpParams.Controls.Clear();
            tlpParams.RowCount = 1;
        }

        private void chkHasParams_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHasParams.Checked)
            {
                groupParams.Visible = true;
                tlpParams.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                PopulateParams(controller.model);
            }
            else
                groupParams.Visible = false;
        }

        private void chkReturnValue_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReturnValue.Checked)
            {
                lblReturnType.Visible = true;
                cboReturnObject.Visible = true;
            }
            else
            {
                lblReturnType.Visible = false;
                cboReturnObject.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ApttusFormValidator.hasValidationErrors(groupBox1.Controls) && !ApttusFormValidator.hasValidationErrors(groupMethod.Controls))
            {
                List<MethodParam> sParams = GetMethodParams();

                Guid returnObject = chkReturnValue.Checked ? (Guid)cboReturnObject.SelectedValue : Guid.Empty;

                bool bSave = controller.Save(txtActionName.Text, ((ApttusObject)cboClass.SelectedItem).Name, txtMethod.Text, chkEnableCache.Checked, sParams, returnObject);

                if (bSave)
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"),this.Handle.ToInt32());  
                    this.Close();
                }
            }
        }

        internal List<MethodParam> GetMethodParams()
        {
            if (!chkHasParams.Checked)
                return null;

            List<MethodParam> sParams = new List<MethodParam>();
            
            // TODO:: check why correct selected value of comboxbox is not returned by ctl.Text
            
            for (int i = 0; i < this.tlpParams.RowCount - 1; i++)
            {
                Control c = this.tlpParams.GetControlFromPosition(0, i);
                string name = ((TextBox)this.tlpParams.GetControlFromPosition(0, i)).Text;

                // If UserType than skip this condition 
                if (string.IsNullOrEmpty(name) && (MethodParam.ParamType)((ComboBox)this.tlpParams.GetControlFromPosition(1, i)).SelectedValue != MethodParam.ParamType.UserInput)
                    continue;

                MethodParam sParam = new MethodParam();

                sParam.ParamName = name;
                
                sParam.Type = (MethodParam.ParamType)((ComboBox)this.tlpParams.GetControlFromPosition(1, i)).SelectedValue;

                if (sParam.Type == MethodParam.ParamType.Static)
                    sParam.ParamValue = ((TextBox)this.tlpParams.GetControlFromPosition(2, i)).Text;

                if (sParam.Type == MethodParam.ParamType.Object || sParam.Type == MethodParam.ParamType.Field)
                {
                    sParam.ParamObject = ((ComboBox)this.tlpParams.GetControlFromPosition(3, i)).SelectedValue.ToString();
                    // Uncomment below to Enable Chunking
                    //if (sParam.Type == MethodParam.ParamType.Object)
                    //{
                    //    sParam.ChunkParam = ((CheckBox)this.tlpParams.GetControlFromPosition(4, i)).Checked;
                    //    //sParam.ChunkParamBy = sParam.ChunkParam ? ((ComboBox)this.tlpParams.GetControlFromPosition(5, i)).SelectedIndex + 1 : 0;
                    //    sParam.ChunkParamBy = sParam.ChunkParam ? Convert.ToInt32(((ComboBox)this.tlpParams.GetControlFromPosition(5, i)).SelectedItem) : 0; 
                    //}
                }
                if (sParam.Type == MethodParam.ParamType.Field)
                    sParam.ParamField = ((ComboBox)this.tlpParams.GetControlFromPosition(4, i)).SelectedValue.ToString();

                if (sParam.Type == MethodParam.ParamType.UserInput)
                {
                    sParam.ParamObject = ((ComboBox)this.tlpParams.GetControlFromPosition(3, i)).SelectedValue.ToString();
                    sParam.ParamField = ((ComboBox)this.tlpParams.GetControlFromPosition(4, i)).SelectedValue.ToString();
                }

                sParams.Add(sParam);
            }

            return sParams;
        }

        private void lnkAddParam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            blnAddRow = true;
            PopulateParams(null, true);            
        }
        private void lnkClearAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetTableLayout();
            PopulateParams(null);
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is ApttusObject)
                    newWidth = (int)g.MeasureString(((ApttusObject)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is ApttusField)
                    newWidth = (int)g.MeasureString(((ApttusField)currentItem).Name, font).Width + vertScrollBarWidth;
                else if (currentItem is EBLookupObject)
                    newWidth = (int)g.MeasureString(((EBLookupObject)currentItem).DisplayMember, font).Width + vertScrollBarWidth;
                else if (currentItem is string)
                    newWidth = (int)g.MeasureString((String)currentItem, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtActionName, e, resourceManager.GetResource("COMMON_NameCannotBeEmpty_ErrorMsg"));
        }

        internal void SetError(Control control, string errorMsg)
        {
            errorProvider.SetError(control, errorMsg);
        }

        private void CallProcedure_Load(object sender, EventArgs e)
        {
            btnCancel.CausesValidation = false;
        }

        private void txtMethod_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtMethod, e, resourceManager.GetResource("CALLPROCACTION_txtMethod_Validating_ErrorMsg"));
        }

        private void cboReturnObject_Validating(object sender, CancelEventArgs e)
        {
            if (chkReturnValue.Checked)
                controller.ValidateAction(cboReturnObject, e, resourceManager.GetResource("CALLPROCACTION_cboReturnObject_Validating_ErrorMsg"));
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
