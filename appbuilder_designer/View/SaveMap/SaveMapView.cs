/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class SaveMapView : UserControl
    {
        SaveMapController controller;
        bool hidePreLoadOptions = true;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public SaveMapView()
        {
            InitializeComponent();
            SetCultureData();
            gbPreloadOptions.Visible = false;
        }
        private void SetCultureData()
        {
            btnAddMatrixFields.Text = resourceManager.GetResource("SAVEMAPVIEW_btnAddMatrixFields_Text");
            btnAddOtherFields.Text = resourceManager.GetResource("SAVEMAPVIEW_btnAddOtherFields_Text");
            btnAddRetrieveFields.Text = resourceManager.GetResource("SAVEMAPVIEW_btnAddRetrieveFields_Text");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnRemove.Text = resourceManager.GetResource("COMMON_RemoveField_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            DesignerLocation.HeaderText = resourceManager.GetResource("COMMON_Location_Text");
            FieldName.HeaderText = resourceManager.GetResource("COMMON_Name_Text").Replace(':', ' ');
            FieldType.HeaderText = resourceManager.GetResource("COMMON_FieldType_Text");
            gbPreloadOptions.Text = resourceManager.GetResource("SAVEMAPVIEW_gbPreloadOptions_Text");
            lblMappedFields.Text = resourceManager.GetResource("SAVEMAPVIEW_gbSaveFields_Text");
            lblName.Text = resourceManager.GetResource("COMMON_Name_Text");
            lblPreloadRows.Text = resourceManager.GetResource("SAVEMAPVIEW_lblPreloadRows_Text");
            lblRepeatingObjects.Text = resourceManager.GetResource("SAVEMAPVIEW_lblRepeatingObjects_Text");
            llPreloadedRows.Text = resourceManager.GetResource("SAVEMAPVIEW_llPreloadedRows_Text");
            SaveConditionText.HeaderText = resourceManager.GetResource("COMMON_Condition_Text");
            Type.HeaderText = resourceManager.GetResource("COMMON_Type_Text").Replace(':', ' ');
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            Globals.ThisAddIn.Application.SheetSelectionChange += Application_SheetSelectionChange;
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Globals.ThisAddIn.Application.SheetSelectionChange -= Application_SheetSelectionChange;
            base.OnHandleDestroyed(e);
        }

        public void SetController(SaveMapController controller)
        {
            this.controller = controller;
        }

        private void btnAddRetrieveFields_Click(object sender, EventArgs e)
        {
            controller.LoadRetrieveFields();
        }

        private void btnAddOtherFields_Click(object sender, EventArgs e)
        {
            controller.AddSaveFields(txtName.Text);
        }

        //Model.IncludeAttachment, Model.AttachAppObjectUniqueId, Model.Filename, Model.Extension, Model.AppendTimestamp
        internal void LoadControls(string saveMapName, string name)
        {
            txtName.Text = name;
            if (!saveMapName.Equals(string.Empty) && (txtName.Text != saveMapName))
                txtName.Text = saveMapName;

            RenderPreLoadedRepeatingObjects(null);
        }

        internal void RenderPreLoadedRepeatingObjects(SaveMapRepeatingObject selectedObject)
        {
            cmbIndependentObjects.DisplayMember = Constants.SAVEMAP_PRELOADEDLIST_DISPLAY;
            cmbIndependentObjects.ValueMember = Constants.SAVEMAP_PRELOADEDLIST_VALUE;
            cmbIndependentObjects.DataSource = controller.GetRepeatingObjects();

            // Set selected object
            if (selectedObject != null)
                cmbIndependentObjects.SelectedValue = selectedObject.SaveGroupId;
        }

        internal void LoadSaveFieldsGrid(List<SaveFieldBound> fields)
        {
            //Filter ID field in Add save field and Add other field popup
            //fields = fields.Where(id => id.FieldId != Constants.ID_ATTRIBUTE).ToList();
            ((DataGridViewImageColumn)this.dgvSaveFields.Columns["LookAhead"]).DefaultCellStyle.NullValue = null;

            dgvSaveFields.AutoGenerateColumns = false;
            dgvSaveFields.DataSource = fields;
            dgvSaveFields.Refresh();

            RefreshLookAheadIcons();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        public void RefreshLookAheadIcons()
        {
            if (dgvSaveFields.DataSource == null)
                return;

            int rowIndex = -1;
            foreach (SaveFieldBound field in (dgvSaveFields.DataSource as List<SaveFieldBound>))
            {
                ++rowIndex;
                if (field.SaveFieldType == SaveType.SaveOnlyField && field.FieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                {
                    if (field.LookAheadProps != null && field.LookAheadProps.Count > 0 || field.LookAheadProp != null)
                        ((DataGridViewImageCell)dgvSaveFields.Rows[rowIndex].Cells[LookAhead.Name]).Value = Properties.Resources.LA_Filled;
                    else
                        ((DataGridViewImageCell)dgvSaveFields.Rows[rowIndex].Cells[LookAhead.Name]).Value = Properties.Resources.LA_Empty;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            controller.RemoveFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            controller.Close();
        }

        private void llPreloadedRows_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TogglePreLoadOptionsView();
        }

        private void btnAddPreloadOptions_Click(object sender, EventArgs e)
        {
            // 1. Validate Preloaded Grid Input
            int preloadedRows = ValidatePreloadOptions();
            if (preloadedRows > -1 && cmbIndependentObjects.SelectedItem != null)
            {
                // 2. Invoke Save
                bool saveSuccess = Save(true);
                if (saveSuccess)
                {
                    // 3. Insert Preloaded Rows
                    SaveMapRepeatingObject selectedObject = (SaveMapRepeatingObject)cmbIndependentObjects.SelectedItem;
                    SaveGroup saveGroup = controller.Model.SaveGroups.FirstOrDefault(sg => sg.GroupId == selectedObject.SaveGroupId);
                    controller.UpdateRows(saveGroup, preloadedRows);

                    // 4. Update Save Group
                    saveGroup.LoadedRows = preloadedRows;

                    // 5. Re-render Preloaded Options
                    RenderPreLoadedRepeatingObjects(selectedObject);
                }
            }
        }

        /// <summary>
        /// Sheet Selection Change Event
        /// </summary>
        /// <param name="Sh"></param>
        /// <param name="Target"></param>
        void Application_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            string targetAddress = ExcelHelper.GetAddress(Target);
            foreach (DataGridViewRow dr in dgvSaveFields.Rows)
            {
                if (Convert.ToString(dr.Cells["DesignerLocation"].Value) == targetAddress)
                {
                    dr.Selected = true;
                    dgvSaveFields.FirstDisplayedScrollingRowIndex = dr.Index;
                }
                else
                    dr.Selected = false;
            }
        }

        #region "Private Helper Functions"

        //private bool ValidateAttachmentUI()
        //{
        //    bool result = true;
        //    if (chkAddTemplate.Checked)
        //    {
        //        result = false;

        //        List<string> InvalidControls = new List<string>();
        //        if (cmbIndependentObjects.SelectedItem == null)
        //            InvalidControls.Add("Add to");
        //        if (string.IsNullOrEmpty(txtFilename.Text.Trim()))
        //            InvalidControls.Add("Filename");

        //        if (InvalidControls.Count > 0)
        //            ApttusMessageUtil.ShowError("Please provide a value for " + string.Join(",", InvalidControls.ToArray()) + ".", "Save Map : Validation");
        //        else
        //            result = true;
        //    }
        //    return result;
        //}

        //private void ToggleAttachmentView(bool IncludeAttachment)
        //{
        //    cmbIndependentObjects.Enabled = IncludeAttachment;
        //    txtFilename.Enabled = IncludeAttachment;
        //    chkAppendTimestamp.Enabled = IncludeAttachment;
        //}

        private void TogglePreLoadOptionsView()
        {
            if (hidePreLoadOptions)
            {
                // If FALSE then show the view, as it is currently closed.
                llPreloadedRows.Text = resourceManager.GetResource("SAVEMAPVIEW_HidePreloadedRows_Text");
                gbPreloadOptions.Visible = hidePreLoadOptions;
                hidePreLoadOptions = false;
            }
            else
            {
                // If TRUE then hide the view, as it is currently shown
                llPreloadedRows.Text = resourceManager.GetResource("SAVEMAPVIEW_llPreloadedRows_Text");
                gbPreloadOptions.Visible = hidePreLoadOptions;
                hidePreLoadOptions = true;
            }
        }

        private int ValidatePreloadOptions()
        {
            int result;
            if (int.TryParse(txtPreloadRows.Text, out result) && (result >= 0 & result <= 10000))
            {
                // Valid Row Count
            }
            else
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("SAVEMAPVEW_PreRowLoad_ErrorMsg"), resourceManager.GetResource("SAVEMAPVEW_PreRowLoadCap_ErrorMsg"));
                result = -1;
            }
            return result;
        }
        #endregion

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtName, resourceManager.GetResource("COMMON_NameCannotBeEmpty_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(txtName, String.Empty);
            }
        }

        private bool Save(bool silent)
        {
            bool result = false;
            if (!HasValidateErrors())
            {
                // In case of look up fields we have Either Or scenario, 
                // i.e. If you add ID field of Lookup you cannot add Name field and vice versa.
                // This validation needs to happen on add of Save Fields.
                List<SaveField> lookupFieldToRemove = new List<SaveField>();
                if (!ValidationManager.ValidationLookup(controller.Model.SaveFields, null, ref lookupFieldToRemove))
                {
                    controller.Save(txtName.Text, silent);
                    result = true;
                }
                else
                {
                    if (lookupFieldToRemove.Count > 0)
                    {
                        foreach (SaveField field in lookupFieldToRemove)
                        {
                            var removeSF = controller.Model.SaveFields.FirstOrDefault(item => item.AppObject.Equals(field.AppObject) && item.FieldId.Equals(field.FieldId));
                            controller.Model.SaveFields.Remove(removeSF);
                        }
                        controller.BindView(controller.Model.SaveFields);
                    }
                }
            }
            return result;
        }

        private bool HasValidateErrors()
        {
            return ApttusFormValidator.hasValidationErrors(this.Controls);
        }

        private void dgvSaveFields_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 6 && e.RowIndex >= 0)
            {

                List<SaveFieldBound> fields = dgvSaveFields.DataSource as List<SaveFieldBound>;

                SaveFieldBound fld = fields[e.RowIndex] as SaveFieldBound;
                // allow lookahead only for Save other fields
                // save repeating fields could set up the lookahead in the display map itself. 
                //TODO : disable button for repeating fields. 
                if (fld.SaveFieldType == SaveType.SaveOnlyField && fld.FieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                {

                    SaveFieldMapper mapper = new SaveFieldMapper(fld)
                    {
                        MappedSaveMap = controller.Model
                    };
                    LookAheadDesignerController lookAheadController = new LookAheadDesignerController(mapper, FieldMapperType.SaveField);
                    lookAheadController.ShowView();
                    //LookAheadPropUI UI = new LookAheadPropUI();
                    //UI.FldMapper = mapper;
                    //LookAheadExcelSourceController ctroller = new LookAheadExcelSourceController(UI);
                    //UI.TopMost = true;
                    //UI.Show();
                }


            }
        }

        private void dgvSaveFields_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvSaveFields.Columns[e.ColumnIndex].Name == "Type")
            {
                ObjectTypeLabel(e);
            }
        }

        internal void ObjectTypeLabel(DataGridViewCellFormattingEventArgs objType)
        {
            if (objType.Value != null)
            {
                try
                {
                    string fieldObjectType = Utils.GetEnumDescription(objType.Value, string.Empty);
                    objType.Value = fieldObjectType;
                    objType.FormattingApplied = true;
                }
                catch (FormatException)
                {
                    // Set to false in case there are other handlers interested trying to 
                    // format this DataGridViewCellFormattingEventArgs instance.
                    objType.FormattingApplied = false;
                }
            }
        }

        private void cmbIndependentObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIndependentObjects.SelectedItem != null)
            {
                txtPreloadRows.Enabled = true;
                btnAddPreloadOptions.Enabled = true;
                SaveMapRepeatingObject selectedObject = (SaveMapRepeatingObject)cmbIndependentObjects.SelectedItem;
                txtPreloadRows.Text = selectedObject.LoadedRows.ToString();
            }
            else
            {
                txtPreloadRows.Text = string.Empty;
                txtPreloadRows.Enabled = false;
                btnAddPreloadOptions.Enabled = false;
            }
        }

        private void btnAddMatrixFields_Click(object sender, EventArgs e)
        {
            controller.LoadMatrixFields();
        }

    }
}
