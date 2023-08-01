using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using System.Text.RegularExpressions;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class AddRowActionView : Form
    {
        private AddRowActionController Controller;
        private AddRowActionModel Model;
        private ConfigurationManager congifurationManager;
        private ApplicationDefinitionManager appDefManager;
        private ApttusResourceManager resourceManager;


        public AddRowActionView()
        {
            congifurationManager = ConfigurationManager.GetInstance;
            appDefManager = ApplicationDefinitionManager.GetInstance;
            resourceManager = ApttusResourceManager.GetInstance;

            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }


        private void SetCultureData()
        {
            label6.Text = resourceManager.GetResource("COMMON_AddRowAction_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            //cboInputType.Text = resourceManager.GetResource("ADDROWACTION_cboInputType_Items");

           // Remaining Items 
            //cboInputType = resourceManager.GetResource("ADDROWACTION_cboInputType_Items1");
            // cboInputType = resourceManager.GetResource("COMMON_NumberOfRows_Text");
            // cboInputType = resourceManager.GetResource("COMMON_UserInput_Text");
            // cboInputType = resourceManager.GetResource("COMMON_AddRowAction_Text");
            
            grpRecords.Text = resourceManager.GetResource("ADDROWACTION_grpRecords_Text");
            label1.Text = resourceManager.GetResource("COMMON_Name_Text");
            label2.Text = resourceManager.GetResource("COMMON_SaveMap_Text");
            label3.Text = resourceManager.GetResource("COMMON_ListObject_Text");
            label4.Text = resourceManager.GetResource("COMMON_Location_Text") + " :";
            label5.Text = resourceManager.GetResource("ADDROWACTION_label5_Text");
            rdoBottom.Text = resourceManager.GetResource("COMMON_Bottom_Text");
            rdoTop.Text = resourceManager.GetResource("COMMON_Top_Text");
            chkDisableExcelEvents.Text = resourceManager.GetResource("Designer_chkDisableExcelEvents_Text");
        }

        public bool ValidateStaticInputValue()
        {
            if (cboInputType.SelectedIndex == (int)AddRowInputType.StaticInput)
            {
                int nRowsToAdd = 0;
                if (Int32.TryParse(txtValue.Text, out nRowsToAdd))
                {
                    if (nRowsToAdd <= 0 || nRowsToAdd > 10000)
                    {
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("COREADDROW_CustomRow_ErrorMsg"), resourceManager.GetResource("COREADDROW_CustomRowCap_ErrorMsg"));
                        txtValue.Text = string.Empty;
                        ActiveControl = txtValue;
                        return false;
                    }
                }
                else
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("ADDROWACTVIEW_ValidateStaticInputValue_InfoMsg"), resourceManager.GetResource("COMMON_NumberOfRows_Text"));
                    txtValue.Text = string.Empty;
                    ActiveControl = txtValue;
                    return false;
                }
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;

            if (ApttusFormValidator.hasValidationErrors(grpAddRowAction.Controls) || ApttusFormValidator.hasValidationErrors(grpRecords.Controls))
                return;

            //if (cboInputType.SelectedIndex == (int)AddRowInputType.CellReference)
            //{
            //    Regex regex = new Regex(@"^([A-Z]{1,3}[0-9]{1,3})(:[A-Z]{1,3}[0-9]{1,3})?$");
            //    bool isCellReferenceValid = regex.IsMatch(txtValue.Text);
            //    if (!isCellReferenceValid)
            //    {
            //        ApttusMessageUtil.ShowInfo("Invalid CellReference Value", "Excel Cell");
            //        ActiveControl = txtValue;
            //        return;
            //    }
            //}

            if (!ValidateStaticInputValue())
                return;

            ObjectInSaveMap obj = cboObject.SelectedItem as ObjectInSaveMap;
            if (obj != null)
            {
                Model.Name = txtActionName.Text;
                Model.DisableExcelEvents = chkDisableExcelEvents.Checked;
                Model.SaveMapAppObjectUniqueId = obj.AppObjectUniqueId;
                Model.SaveGroupTargetNamedRange = obj.TargetNamedRange;
                Model.SaveMapId = (cboSaveMap.SelectedItem as SaveMap).Id;
                Model.InputType = (AddRowInputType)cboInputType.SelectedIndex;
                Model.Location = rdoBottom.Checked == true ? AddRowLocation.Bottom : AddRowLocation.Top;
                if (Model.InputType == AddRowInputType.CellReference || Model.InputType == AddRowInputType.StaticInput)
                    Model.InputValue = txtValue.Text;
                else if (Model.InputType == AddRowInputType.ActionFlowStepInput)
                    Model.InputValue = (cboAllObjects.SelectedValue).ToString();
                Controller.Save();

                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_AddRow_Text"));
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Controller.Cancel();
        }

        private void AddRowActionView_Load(object sender, EventArgs e)
        {
            ActiveControl = txtActionName;
            rdoBottom.Checked = true; //By default, when the form is launched in create mode, set the default to bottom.

            List<SaveMap> saveMaps = (from saveMap in congifurationManager.SaveMaps
                                      from sg in saveMap.SaveGroups
                                      where sg.GroupId != Guid.Empty
                                      select saveMap).Distinct().ToList();

            List<ObjectInSaveMap> objectsInSaveMaps = (from sm in saveMaps
                                                       from sg in sm.SaveGroups
                                                       where sg.GroupId != Guid.Empty
                                                       select new ObjectInSaveMap() { AppObjectUniqueId = sg.AppObject, SaveMap = sm, TargetNamedRange = sg.TargetNamedRange }).ToList();


            cboAllObjects.DisplayMember = "Name";
            cboAllObjects.ValueMember = "UniqueId";
            cboAllObjects.DataSource = appDefManager.GetAllObjects().Where(obj => obj.ObjectType == ObjectType.Repeating).ToList();

            cboSaveMap.DisplayMember = "Name";
            cboSaveMap.ValueMember = "Id";
            cboSaveMap.DataSource = objectsInSaveMaps.GroupBy(obj => obj.SaveMap).Select(o => o.Key).ToList();

            cboInputType.SelectedIndex = cboObject.SelectedIndex = cboSaveMap.SelectedIndex = -1;

            if (Controller.FormOpenMode == FormOpenMode.Edit)
            {
                cboSaveMap.SelectedValue = Model.SaveMapId;
                cboObject.SelectedValue = Model.SaveMapAppObjectUniqueId;
                rdoBottom.Checked = Model.Location == AddRowLocation.Bottom;
                rdoTop.Checked = Model.Location == AddRowLocation.Top;
                txtActionName.Text = Model.Name;
                chkDisableExcelEvents.Checked = Model.DisableExcelEvents;
                cboInputType.SelectedIndex = (int)Model.InputType;

                if (Model.InputType == AddRowInputType.CellReference || Model.InputType == AddRowInputType.StaticInput)
                    txtValue.Text = Convert.ToString(Model.InputValue);
                else if (Model.InputType == AddRowInputType.ActionFlowStepInput)
                    cboAllObjects.SelectedValue = Guid.Parse(Model.InputValue);
            }
        }

        internal void SetController(AddRowActionController addRowActionController, AddRowActionModel model)
        {
            Controller = addRowActionController;
            Model = model;
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
                if (currentItem is SaveMap)
                    newWidth = (int)g.MeasureString((currentItem as SaveMap).Name, font).Width + vertScrollBarWidth;
                if (currentItem is ApttusObject)
                    newWidth = (int)g.MeasureString((currentItem as ApttusObject).Name, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        private void cboSaveMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSaveMap.SelectedIndex == -1)
                return;

            SaveMap sm = cboSaveMap.SelectedItem as SaveMap;

            List<ObjectInSaveMap> objectsInSaveMaps = (from sg in sm.SaveGroups
                                                       where sg.GroupId != Guid.Empty
                                                       select new ObjectInSaveMap() { AppObjectUniqueId = sg.AppObject, SaveMap = sm, TargetNamedRange = sg.TargetNamedRange }).ToList();

            cboObject.DataSource = null;

            List<ApttusObject> objects = appDefManager.GetAllObjects().Where(obj => objectsInSaveMaps.Exists(obj1 => obj1.AppObjectUniqueId == obj.UniqueId)).ToList();
            foreach (ObjectInSaveMap obj in objectsInSaveMaps)
                obj.ObjectName = objects.Find(o => o.UniqueId == obj.AppObjectUniqueId).Name;

            cboObject.DisplayMember = "ObjectName";
            cboObject.ValueMember = "AppObjectUniqueId";
            cboObject.DataSource = objectsInSaveMaps;
        }

        private void txtActionName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtActionName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtActionName, resourceManager.GetResource("ADDROWACTVIEW_txtActionName_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtActionName, string.Empty);
            }
        }

        private void cboSaveMap_Validating(object sender, CancelEventArgs e)
        {
            if (cboSaveMap.SelectedIndex < 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(cboSaveMap, resourceManager.GetResource("ADDROWACTVIEW_cboSaveMap_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cboSaveMap, string.Empty);
            }
        }

        private void cboObject_Validating(object sender, CancelEventArgs e)
        {
            if (cboObject.SelectedIndex < 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(cboObject, 	resourceManager.GetResource("ADDROWACTVIEW_cboObject_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cboObject, string.Empty);
            }
        }

        private void cboInputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboInputType.SelectedIndex == (int)AddRowInputType.ActionFlowStepInput)
            {
                lblValue.Visible = txtValue.Visible = false;
                cboAllObjects.Visible = true;
            }
            else if (cboInputType.SelectedIndex == (int)AddRowInputType.UserInput)
                cboAllObjects.Visible = lblValue.Visible = txtValue.Visible = false;
            else
            {
                cboAllObjects.Visible = false;
                lblValue.Visible = txtValue.Visible = true;
                if (cboInputType.SelectedIndex == (int)AddRowInputType.CellReference)
                {
                    lblValue.Text = resourceManager.GetResource("ADDROWACTION_cboInputType_Items1") + " :";
                    Modules.CueProvider.SetCue(txtValue, "e.g. Sheet1!A1");
                }
                else
                {
                    Modules.CueProvider.ClearCue(txtValue);
                    lblValue.Text = resourceManager.GetResource("COMMON_NumberOfRows_Text") + " :";
                    ActiveControl = txtValue;
                }
            }
        }

        private void txtValue_Validating(object sender, CancelEventArgs e)
        {
            if (txtValue.Visible == false)
                e.Cancel = false;
            else
            {
                if (string.IsNullOrEmpty(txtValue.Text) || string.IsNullOrWhiteSpace(txtValue.Text))
                {
                    e.Cancel = true;
                    string errorMsg = (Model.InputType == AddRowInputType.CellReference ? resourceManager.GetResource("ADDROWACTION_cboInputType_Items1") : resourceManager.GetResource("ADDROWACTVIEW_txtValue_ValidatingStaticInput_ErrorMsg"))+" "+ resourceManager.GetResource("ADDROWACTVIEW_txtValue_ValidatingCannotEmpty_ErrorMsg");
                    errorProvider1.SetError(txtValue, errorMsg);
                }
                else if (cboInputType.SelectedIndex == (int)AddRowInputType.CellReference)
                {
                    bool isCellReferenceValid = IsValidCellReference(txtValue.Text);
                    if (!isCellReferenceValid)
                    {
                        e.Cancel = true;
                        errorProvider1.SetError(txtValue, resourceManager.GetResource("ADDROWACTVIEW_txtValue_ValidatingInvalidCell_ErrorMsg"));
                    }
                }
                else
                {
                    e.Cancel = false;
                    errorProvider1.SetError(txtValue, string.Empty);
                }
            }
        }

        private bool IsValidCellReference(string value)
        {
            bool isCellReferenceValid = false;
            string cellReferenceValue = txtValue.Text;
            if ((!string.IsNullOrEmpty(cellReferenceValue)) && cellReferenceValue.LastIndexOf(@"!") > 0
                && cellReferenceValue.LastIndexOf(@"!") < cellReferenceValue.Length)
            {
                int sheetNameEnd = cellReferenceValue.LastIndexOf(@"!");
                string sheetName = cellReferenceValue.Substring(0, sheetNameEnd);
                var workSheet = ExcelHelper.GetWorkSheet(sheetName);
                if (workSheet != null)
                {
                    // Worksheet exists, Match the cell reference pattern
                    string cellLocation = cellReferenceValue.Substring(sheetNameEnd + 1);
                    Regex regex = new Regex(@"(\$?[A-Za-z]{1,3})(\$?[0-9]{1,6})");
                    isCellReferenceValid = regex.IsMatch(cellLocation);
                }
            }
            return isCellReferenceValid;
        }

        private void cboInputType_Validating(object sender, CancelEventArgs e)
        {
            if (cboInputType.SelectedIndex < 0)
            {
                e.Cancel = true;
                errorProvider1.SetError(cboInputType, resourceManager.GetResource("ADDROWACTVIEW_cboInputType_Validating_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cboInputType, string.Empty);
            }
        }
    }

    public class ObjectInSaveMap
    {
        public Guid AppObjectUniqueId { get; set; }
        public SaveMap SaveMap { get; set; }
        public string TargetNamedRange { get; set; }
        public string ObjectName { get; set; }
    }
}
