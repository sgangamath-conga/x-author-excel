using Apttus.XAuthor.Core;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class LookAheadDesignerView : Form
    {

        private static LookAheadDesignerView instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private LookAheadDesignerController controller;
        internal LookAheadDesignerController Controller { get => controller; set => controller = value; }
        private LookAheadDataSource CurrentDataSource;
        internal bool IsFormOpen;
        private ToolTip lnkAddSearchAndSelectToolTip;
        private static object syncRoot = new Object();
        private LookAheadDesignerView()
        {
            lnkAddSearchAndSelectToolTip = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.None,
                ShowAlways = true
            };
            InitializeComponent();
            SetCultureData();
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.DESIGNER_PRODUCT_NAME;
            TopMost = true;
            IsFormOpen = false;

        }
        public static LookAheadDesignerView GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LookAheadDesignerView();
                    }
                }
                return instance;
            }
        }
        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("LOOKAHEADPROPUI_Title");
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            btnRemove.Text = resourceManager.GetResource("COMMON_Remove_Text");
            rdbExcel.Text = resourceManager.GetResource("LOOKAHEADPROPUI_tbExcel_Text");
            rdbSearchAndSelect.Text = resourceManager.GetResource("LookAhead_SearchandSelect_Text");
            rdbMultiple.Text = resourceManager.GetResource("LOOKAHEADPROPUI_radAdvanced_Text");
            rdbSingle.Text = resourceManager.GetResource("LOOKAHEADPROPUI_radBasic_Text");
            gbColumnMapping.Text = resourceManager.GetResource("LOOKAHEADPROPUI_groupBox4_Text");
            lblPrimaryColumn.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label5_Text");
            lblSecondaryColumn.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label8_Text");
            lblTargetField.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label7_Text");
            lblCurrentSelectedRangeText.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label2_Text");
            lblHighlightText.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label3_Text");
            btnCurrentSelection.Text = resourceManager.GetResource("LOOKAHEADPROPUI_cmdCurrentSelection_Text");
            chkRefreshData.Text = resourceManager.GetResource("LOOKAHEADPROPUI_chkRefreshData_Text");
            lblSearchAndSelectActions.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label4_Text");
            chkFieldMapping.Text = resourceManager.GetResource("LOOKAHEADPROPUI_chkFieldMapping_Text");
            lblSSRecordIDText.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label10_Text");
            lblMappedRecordId.Text = resourceManager.GetResource("LOOKAHEADPROPUI_label9_Text");
            gbRangeSelection.Text = resourceManager.GetResource("LookAheadUI_gbRangeSelection_Text");
            lblConfigureSS.Text = resourceManager.GetResource("LookAheadDesigner_lblConfigureSS_Text");
            lblConfigureExcel.Text = resourceManager.GetResource("LookAheadDesigner_lblConfigureExcel_Text");
            lnkAddSearchAndSelect.Text = resourceManager.GetResource("LookAhead_CreateNew_Action");
            lnkAddSearchAndSelectToolTip.SetToolTip(lnkAddSearchAndSelect, resourceManager.GetResource("LookAhead_CreateNew_Action_ToolTip"));
            lblGetCurrentSelectionText.Text = string.Empty;
        }
        private void LookAheadDesigner_Load(object sender, EventArgs e)
        {
            IsFormOpen = true;
            tblMultiColumn.Visible = false;
            gbColumnMapping.Visible = false;
            if (Controller.GetDataSourceOrDefault().Equals(LookAheadDataSource.Excel))
                rdbExcel.Checked = true;
            else rdbSearchAndSelect.Checked = true;
            lblMappedFieldName.Text = Controller.FieldMapper.GetFieldName();
        }

        private void SwitchSourceView(LookAheadDataSource dataSource)
        {
            CurrentDataSource = dataSource;
            tblMain.SuspendLayout();
            switch (dataSource)
            {
                case LookAheadDataSource.SearchAndSelect:
                    tblSearchAndSelect.Visible = true;
                    tblExcel.Visible = false;
                   // lookAheadFieldMappingView.SetCultureData();
                  //  lookAheadFieldMappingView.Visible = false;
                    PopulateSearchAndSelectUI();
                    break;
                case LookAheadDataSource.Excel:
                default:
                    tblSearchAndSelect.Visible = false;
                    tblExcel.Visible = true;
                    if (Controller.IsLookAheadConfigured())
                        PopulateExcelUI();
                    break;
            }
            tblMain.ResumeLayout();
        }

        private void PopulateExcelUI()
        {
            LookAheadProperty CurrentLookAheadProperty = Controller.GetExcelLookAheadProperty();

            if (CurrentLookAheadProperty == null) return;

            if (CurrentLookAheadProperty is ExcelDataSource CurrentDataSource)
            {
                chkRefreshData.Checked = CurrentDataSource.RefreshData;
                lblGetCurrentSelectionText.Text = CurrentDataSource.SheetReference;

                if (!string.IsNullOrEmpty(CurrentDataSource.TargetRange))
                {
                    Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetRange(CurrentDataSource.TargetRange);
                    if (CurrentRange != null)
                    {
                        gbColumnMapping.Visible = true;
                        CurrentRange.Worksheet.Select();
                        CurrentRange.Select();
                        List<string> PrimaryColumnDataSource = Controller.GetAllColumnNames(CurrentRange);
                        cmbPrimary.DataSource = PrimaryColumnDataSource.ToList();

                        if (CurrentDataSource.MultiCol && CurrentDataSource is ExcelMultiColumProp MultiColumnDataSource)//Hanlde multi column data source
                        {
                            rdbMultiple.Checked = CurrentDataSource.MultiCol;
                            ReturnDataCollection SecondaryReturnColumnData = MultiColumnDataSource.ReturnColumnData;
                            string FieldId = string.Empty;
                            string ExcelColumn = string.Empty;

                            List<string> SecondaryColumnDataSource = PrimaryColumnDataSource.ToList();
                            SecondaryColumnDataSource.Insert(0, string.Empty);
                            cmbSecondaryColumn.DataSource = SecondaryColumnDataSource;

                            if (SecondaryReturnColumnData != null && SecondaryReturnColumnData.DataCollection != null && SecondaryReturnColumnData.DataCollection.Count > 0)
                            {
                                FieldId = SecondaryReturnColumnData.DataCollection.FirstOrDefault().FieldID;
                                ExcelColumn = SecondaryReturnColumnData.DataCollection.FirstOrDefault().ExcelDataSourceColumn;
                            }

                            if (!string.IsNullOrEmpty(MultiColumnDataSource.ReturnCol))
                                cmbPrimary.SelectedItem = MultiColumnDataSource.ReturnCol;
                            if (!string.IsNullOrEmpty(ExcelColumn))
                                cmbSecondaryColumn.SelectedIndex = cmbSecondaryColumn.FindStringExact(ExcelColumn);

                            PopulateTargetField(FieldId);

                        }
                        else
                        {
                            cmbPrimary.Visible = false;
                            lblPrimaryColumnText.Visible = true;
                            lblPrimaryColumnText.Text = string.Format(resourceManager.GetResource("LookAheadDesignerUI_PrimaryColumnSelected_text"), PrimaryColumnDataSource.FirstOrDefault());
                        }
                    }
                }
            }

        }
        private void PopulateTargetField(string FieldId)
        {
            switch (Controller.FieldMapperType)
            {
                case FieldMapperType.RetrieveField:
                    List<RetrieveField> RetrieveFields = Controller.GetRetrieveFields();
                    if (RetrieveFields != null)
                    {
                        cmbTargetField.DataSource = RetrieveFields;
                        cmbTargetField.DisplayMember = "FieldName";
                        cmbTargetField.ValueMember = "TargetColumnIndex";
                        if (!string.IsNullOrEmpty(FieldId))
                            cmbTargetField.SelectedIndex = RetrieveFields.FindIndex(item => item.FieldId.Equals(FieldId));
                    }
                    break;
                case FieldMapperType.SaveField:
                    List<SaveFieldBound> SaveFields = Controller.GetSaveFields();
                    if (SaveFields != null)
                    {
                        cmbTargetField.DataSource = SaveFields;
                        cmbTargetField.DisplayMember = "FieldName";
                        cmbTargetField.ValueMember = "TargetColumnIndex";
                        if (!string.IsNullOrEmpty(FieldId))
                            cmbTargetField.SelectedIndex = SaveFields.FindIndex(item => item.FieldId.Equals(FieldId));
                    }
                    break;
            }
        }

        private bool PopulateSearchAndSelectActions(bool FieldMappingEnabled = false)
        {
            bool result = false;
            if (Controller.FieldMapperHelper.AppObjectUniqueId != Guid.Empty)
            {
                ApttusObject AppObject = ApplicationDefinitionManager.GetInstance.GetAppObject(Controller.FieldMapperHelper.AppObjectUniqueId);

                List<SearchAndSelect> AllSearchAndSelectActions = ConfigurationManager.GetInstance.Actions.OfType<SearchAndSelect>().ToList();
                List<SearchAndSelect> FilteredSearchAndSelectActions = new List<SearchAndSelect>();
                if (FieldMappingEnabled)
                {
                    FilteredSearchAndSelectActions = (from ssOtherObject in AllSearchAndSelectActions
                                                      where !ssOtherObject.TargetObject.Equals(AppObject.UniqueId)
                                                      select ssOtherObject).ToList();
                }
                else
                {
                    FilteredSearchAndSelectActions = (from ssCurrentObject in AllSearchAndSelectActions
                                                      where ssCurrentObject.TargetObject.Equals(AppObject.UniqueId)
                                                         && ssCurrentObject.RecordType.Equals(Constants.QUERYRESULT_SINGLE)
                                                      select ssCurrentObject).ToList();
                }
                if (FilteredSearchAndSelectActions.Count > 0)
                {
                    cmboActionName.Enabled = true;
                    cmboActionName.DataSource = FilteredSearchAndSelectActions;
                    lnkAddSearchAndSelect.Visible = false;
                }
                else
                {
                    cmboActionName.Enabled = false;
                    lnkAddSearchAndSelect.Visible = true;
                }
                if ((AppObject.Id.Equals("Task") || AppObject.Id.Equals("Event")) && Controller.FieldMapper is RetrieveFieldMapper retrieveFieldMapper)
                {
                    string fieldId = retrieveFieldMapper.MappedRetrieveField.FieldId;
                    if (fieldId.Equals("WhatId") || fieldId.Equals("WhoId") || fieldId.Equals("What.Name") || fieldId.Equals("Who.Name"))
                    {
                        cmboActionName.DataSource = AllSearchAndSelectActions;
                    }
                }

                cmboActionName.DisplayMember = "Name";
                cmboActionName.ValueMember = "Id";
                chkFieldMapping.Enabled = FieldMappingEnabled;
                result = true;
            }
            else
            {
                lnkAddSearchAndSelect.Visible = true;
                //ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("LOOKAHEADPROPCTL_PopulateUI_InfoMsg"), resourceManager.CRMName), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                //cmboActionName.Enabled = false;
                //chkFieldMapping.Enabled = false;
            }
            return result;
        }
        private void PopulateSearchAndSelectUI()
        {
            if (PopulateSearchAndSelectActions())
            {
                SSActionDataSource SearchAndSelectProperty = Controller.GetSearchAndSelectLookAheadProperty();
                string FieldId = string.Empty;

                if (SearchAndSelectProperty != null)
                {
                    if (SearchAndSelectProperty.ReturnColumnData != null)
                    {
                        ReturnDataCollection RetData = SearchAndSelectProperty.ReturnColumnData;
                        FieldId = RetData.DataCollection[0].FieldID;
                    }
                    if (!string.IsNullOrEmpty(SearchAndSelectProperty.ActionName))
                        cmboActionName.SelectedIndex = cmboActionName.FindStringExact(SearchAndSelectProperty.ActionName);
                    if (SearchAndSelectProperty.ObjectMapping != null)
                    {
                        if (!chkFieldMapping.Checked)
                            chkFieldMapping.Checked = true;
                        else
                            PopulateSearchAndSelectActions(true);
                        cmboActionName.SelectedValue = SearchAndSelectProperty.ObjectMapping.SearchAndSelectActionId;
                       // lookAheadFieldMappingView.LoadMappingFields(SearchAndSelectProperty.ObjectMapping);
                    }
                }

                cmbRecordId.SelectedIndex = -1;
                if (Controller.FieldMapper is RetrieveFieldMapper retrieveFieldMapper)
                {
                    List<RetrieveField> retrieveFields = Controller.GetRetrieveFields();
                    cmbRecordId.DataSource = retrieveFields;
                    if (!string.IsNullOrEmpty(FieldId))
                    {
                        RetrieveField retrieveField = retrieveFields.Find(ret => ret.FieldId.Equals(FieldId));
                        if (retrieveField != null)
                            cmbRecordId.SelectedIndex = retrieveFields.FindIndex(item => item.FieldId.Equals(FieldId));
                    }
                }
                else
                {
                    List<SaveFieldBound> saveFields = Controller.GetSaveFields();
                    cmbRecordId.DataSource = saveFields;
                    if (!string.IsNullOrEmpty(FieldId))
                    {
                        SaveFieldBound saveField = saveFields.Find(ret => ret.FieldId.Equals(FieldId));
                        if (saveField != null)
                            cmbRecordId.SelectedIndex = saveFields.FindIndex(item => item.FieldId.Equals(FieldId));
                    }
                }
                cmbRecordId.DisplayMember = "FieldName";
                cmbRecordId.ValueMember = "TargetColumnIndex";
            }
        }
        private void chkFieldMapping_CheckedChanged(object sender, EventArgs e)
        {
            PopulateSearchAndSelectActions(chkFieldMapping.Checked);
           // lookAheadFieldMappingView.Visible = chkFieldMapping.Checked;
        }
        private void cmboActionName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchAndSelect searchAndSelect = cmboActionName.SelectedItem as SearchAndSelect;
            if (searchAndSelect != null && chkFieldMapping.Checked)
            {
                //if (Controller.FieldMapper is RetrieveFieldMapper retrieveFieldMapper)
                //{
                //    lookAheadFieldMappingView.RetrieveMap = retrieveFieldMapper.MappedRetrieveMap;
                //    lookAheadFieldMappingView.FieldAppObject = retrieveFieldMapper.MappedRetrieveField.AppObject;
                //    lookAheadFieldMappingView.SearchAndSelectAction = searchAndSelect;
                //}
            }
            if (cmboActionName.SelectedIndex != -1)
                lookAheadErrorProvider.SetError(cmboActionName, String.Empty);
        }
        internal void DisplayMessage(string message)
        {
            Hide();
            ApttusMessageUtil.ShowInfo(message, resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
            Show();
        }
        private void RdbExcel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbExcel.Checked)
                SwitchSourceView(LookAheadDataSource.Excel);
        }

        private void RdbSearchAndSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSearchAndSelect.Checked)
                SwitchSourceView(LookAheadDataSource.SearchAndSelect);
        }


        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RdbSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSingle.Checked)
            {
                Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetCurrentSelectedCol();
                if (CurrentRange != null && CurrentRange.Columns.Count == 1)
                {
                    tblMultiColumn.Visible = false;
                    cmbPrimary.Visible = false;
                    lblPrimaryColumnText.Visible = true;
                }
                else
                {
                    rdbMultiple.Checked = true;
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_GetSingleColumn_InfoMsg"));
                }
            }
        }

        private void RdbMultiple_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbMultiple.Checked)
            {
                Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetCurrentSelectedCol();
                if (CurrentRange != null && CurrentRange.Columns.Count > 1)
                {
                    tblMultiColumn.Visible = true;
                    cmbPrimary.Visible = true;
                    lblPrimaryColumnText.Visible = false;
                }
                else
                {
                    rdbSingle.Checked = true;
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_GetMultiColumns_InfoMsg"));
                }
            }
        }

        private void BtnCurrentSelection_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            if (CurrentRange != null && CurrentRange.Rows.Count >= 2)
            {
                lblGetCurrentSelectionText.Text = ExcelHelper.GetAddress(CurrentRange);
                gbColumnMapping.Visible = true;
                List<string> ColumnNames = Controller.GetAllColumnNames(CurrentRange);
                cmbPrimary.DataSource = ColumnNames;

                if (CurrentRange.Columns.Count > 1)
                {
                    if (ColumnNames != null && ColumnNames.Count > 1)
                    {
                        rdbMultiple.Checked = true;
                        List<string> SecondaryColumnNames = ColumnNames.ToList();
                        SecondaryColumnNames.Insert(0, string.Empty);
                        cmbSecondaryColumn.DataSource = SecondaryColumnNames;
                        PopulateTargetField(string.Empty);
                    }
                }
                else
                {
                    rdbSingle.Checked = true;
                    RdbSingle_CheckedChanged(rdbSingle, null);
                    lblPrimaryColumnText.Text = string.Format(resourceManager.GetResource("LookAheadDesignerUI_PrimaryColumnSelected_text"), ColumnNames.FirstOrDefault());
                }
            }
            else
            {
                DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_Select2Rows_InfoMsg"));
            }
        }
        private void cmbTargetField_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            using (Graphics g = senderComboBox.CreateGraphics())
            {
                Font font = senderComboBox.Font;
                int vertScrollBarWidth = (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth = 0;
                foreach (object currentItem in ((ComboBox)sender).Items)
                {
                    if (currentItem is RetrieveField)
                        newWidth = (int)g.MeasureString(((RetrieveField)currentItem).FieldName, font).Width + vertScrollBarWidth;
                    else if (currentItem is SaveFieldBound)
                        newWidth = (int)g.MeasureString(((SaveFieldBound)currentItem).FieldName, font).Width + vertScrollBarWidth;

                    if (width < newWidth)
                        width = newWidth + 16;
                }
            }
            senderComboBox.DropDownWidth = width;
        }

        private void cmbSecondaryColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSecondaryColumn.SelectedIndex == 0)
            {
                cmbSecondaryColumn.SelectedIndex = -1;
                cmbTargetField.Enabled = false;
            }
            else
                cmbTargetField.Enabled = true;
        }

        private void lblMappedFieldName_TextChanged(object sender, EventArgs e)
        {
            lblPrimaryColumn.Text = lblMappedFieldName.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (rdbExcel.Checked)
            {
                if (!HasValidateErrors(tblExcel.Controls))
                {
                    SaveExcelSource();
                }
            }
            else
            {
                if (!HasValidateErrors(tblSearchAndSelect.Controls))
                {
                    SaveSearchAndSelectSource();
                }
            }

        }
        private void SaveSearchAndSelectSource()
        {
            Core.Action action = cmboActionName.SelectedItem as Core.Action;
            SSActionDataSource property = new SSActionDataSource
            {
                IsActive = true,
                ActionID = action.Id,
                ActionName = action.Name
            };

            ReturnColumnData returnColumnData = new ReturnColumnData();

            if (cmbRecordId.SelectedItem is RetrieveField retrieveField)
            {
                returnColumnData.TargetColumn = retrieveField.TargetColumnIndex;
                returnColumnData.FieldID = retrieveField.FieldId;
                returnColumnData.TargetNR = retrieveField.TargetNamedRange;
                returnColumnData.FldType = string.IsNullOrEmpty(retrieveField.TargetNamedRange) ? ObjectType.Independent : retrieveField.Type;
            }
            else if (cmbRecordId.SelectedItem is SaveField saveField)// save other field
            {
                returnColumnData.TargetColumn = saveField.TargetColumnIndex;
                returnColumnData.FieldID = saveField.FieldId;
                returnColumnData.TargetNR = saveField.TargetNamedRange;
                returnColumnData.FldType = string.IsNullOrEmpty(saveField.TargetNamedRange) ? ObjectType.Independent : saveField.Type;
            }

            ReturnDataCollection returnDataCollection = new ReturnDataCollection();
            returnDataCollection.DataCollection.Add(returnColumnData);
            property.ReturnColumnData = returnDataCollection;
            //if (chkFieldMapping.Checked)
            //    property.ObjectMapping = lookAheadFieldMappingView.Save();

            if (Controller.Save(property))
            {
                Hide();
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("LOOKAHEADPROPUIFORM_btnApply_Click_InfoMsg"), Controller.FieldMapper.GetFieldName()), resourceManager.GetResource("RETRIEVEMAP_SuccessCap_ShowMsg"));
                Close();
            }
        }

        private void SaveExcelSource()
        {

            if (!Controller.IsValidRowsSelected())
            {
                DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_Select2Rows_InfoMsg"));
                return;
            }
            ExcelDataSource property = new ExcelDataSource();

            if (rdbMultiple.Checked)
            {
                ReturnColumnData returnColumnData = new ReturnColumnData();
                ReturnDataCollection returnDataCollection;
                property = new ExcelMultiColumProp
                {
                    MultiCol = rdbMultiple.Checked,
                    ReturnCol = cmbPrimary.SelectedItem.ToString()
                };
                switch (Controller.FieldMapperType)
                {
                    case FieldMapperType.RetrieveField:
                        RetrieveField retrieveField = cmbTargetField.SelectedItem as RetrieveField;
                        returnColumnData = new ReturnColumnData(
                            retrieveField.TargetColumnIndex,
                            cmbSecondaryColumn.SelectedItem.ToString(),
                            retrieveField.FieldId,
                            retrieveField.TargetNamedRange,
                            retrieveField.Type);
                        break;
                    case FieldMapperType.SaveField:
                        SaveField saveField = cmbTargetField.SelectedItem as SaveField;
                        returnColumnData = new ReturnColumnData(
                            saveField.TargetColumnIndex,
                            cmbSecondaryColumn.SelectedItem.ToString(),
                            saveField.FieldId,
                            saveField.TargetNamedRange,
                            saveField.Type);
                        break;
                }
                returnDataCollection = new ReturnDataCollection();
                returnDataCollection.DataCollection.Add(returnColumnData);
                ((ExcelMultiColumProp)property).ReturnColumnData = returnDataCollection;
            }

            property.TargetRange = Controller.GetCurrentRangeCellName();
            property.SheetReference = lblGetCurrentSelectionText.Text;
            property.RefreshData = chkRefreshData.Checked;
            property.IsActive = true;

            if (Controller.Save(property))
            {
                Hide();
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("LOOKAHEADPROPUIFORM_btnApply_Click_InfoMsg"),Controller.FieldMapper.GetFieldName()), resourceManager.GetResource("RETRIEVEMAP_SuccessCap_ShowMsg"));
                Close();
            }
        }
        private bool HasValidateErrors(TableLayoutControlCollection controls)
        {
            return ApttusFormValidator.hasValidationErrors(controls);
        }
        private void lblGetCurrentSelectionText_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(lblGetCurrentSelectionText.Text))
            {
                e.Cancel = true;
                lookAheadErrorProvider.SetError(lblGetCurrentSelectionText, resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyGetCurrent_InfoMsg"));
            }
            else
            {
                e.Cancel = false;
                lookAheadErrorProvider.SetError(lblGetCurrentSelectionText, String.Empty);
            }
        }

        private void lblGetCurrentSelectionText_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblGetCurrentSelectionText.Text))
            {
                lookAheadErrorProvider.SetError(lblGetCurrentSelectionText, String.Empty);
            }
        }

        private void cmbSecondaryColumn_Validating(object sender, CancelEventArgs e)
        {
            if (tblMultiColumn.Visible && cmbSecondaryColumn.SelectedIndex == -1)
            {
                e.Cancel = true;
                lookAheadErrorProvider.SetError(cmbSecondaryColumn, resourceManager.GetResource("LOOKAHEAD_ValidVal_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                lookAheadErrorProvider.SetError(cmbSecondaryColumn, String.Empty);
            }
        }

        private void cmbTargetField_Validating(object sender, CancelEventArgs e)
        {
            if (tblMultiColumn.Visible && cmbTargetField.SelectedIndex == 0)
            {
                e.Cancel = true;
                lookAheadErrorProvider.SetError(cmbTargetField, resourceManager.GetResource("LOOKAHEAD_ValidVal_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                lookAheadErrorProvider.SetError(cmbTargetField, String.Empty);
            }
        }

        private void cmboActionName_Validating(object sender, CancelEventArgs e)
        {
            if (cmboActionName.SelectedIndex == -1)
            {
                e.Cancel = true;
                lookAheadErrorProvider.SetError(cmboActionName, resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyValidAction_InfoMsg"));
            }
            else
            {
                e.Cancel = false;
                lookAheadErrorProvider.SetError(cmboActionName, String.Empty);
            }
        }

        private void cmbRecordId_Validating(object sender, CancelEventArgs e)
        {
            if (cmbRecordId.SelectedIndex <= 0 && !chkFieldMapping.Checked)
            {
                e.Cancel = true;
                lookAheadErrorProvider.SetError(cmbRecordId, resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyValidField_InfoMsg"));
            }
            else
            {
                e.Cancel = false;
                lookAheadErrorProvider.SetError(cmbRecordId, String.Empty);
            }
        }

        private void cmbRecordId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRecordId.SelectedIndex >= 0)
                lookAheadErrorProvider.SetError(cmbRecordId, String.Empty);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (rdbExcel.Checked)
            {
                lblGetCurrentSelectionText.Text = string.Empty;
                chkRefreshData.Checked = false;
                gbColumnMapping.Visible = false;
                Controller.FieldMapper.PropertyCollection.RemoveAll(item => item.DataSource.Equals(LookAheadDataSource.Excel));
            }
            else
            {
                if (chkFieldMapping.Checked) chkFieldMapping.Checked = false;
                else PopulateSearchAndSelectActions(false);
                cmbRecordId.SelectedIndex = 0;
                Controller.FieldMapper.PropertyCollection.RemoveAll(item => item.DataSource.Equals(LookAheadDataSource.SearchAndSelect));
            }
            LookAheadProperty property = Controller.FieldMapper.Property;
            if (property != null && Controller.FieldMapper.RemoveProperty(property))
            {
                Hide();
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKAHEADPROPCTL_Detach_InfoMsg"), resourceManager.GetResource("RETRIEVEMAP_SuccessCap_ShowMsg"));
                Close();
            }
            else
            {
                Hide();
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("LOOKAHEADPROPCTL_DetachNoDataSource_InfoMsg"),Controller.FieldMapper.GetFieldName()), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                Show();
            }
        }

        private void LookAheadDesignerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsFormOpen = false;
            instance.Dispose(true);
            instance = null;
        }

        private void lnkAddSearchAndSelect_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Hide();
                ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

                ApttusObject appobject = applicationDefinitionManager.GetAppObject(Controller.FieldMapperHelper.AppObjectUniqueId);
                if (appobject == null)
                {

                    ApttusObject parentObject = null;
                    ApttusField targetField = null;
                    switch (Controller.FieldMapperType)
                    {
                        case FieldMapperType.RetrieveField:
                            RetrieveFieldMapper retrieveFieldMapper = Controller.FieldMapper as RetrieveFieldMapper;
                            parentObject = applicationDefinitionManager.GetAppObject(retrieveFieldMapper.MappedRetrieveField.AppObject);
                            targetField = parentObject.GetField(retrieveFieldMapper.MappedRetrieveField.FieldId);
                            break;

                        case FieldMapperType.SaveField:
                            SaveFieldMapper saveFieldMapper = Controller.FieldMapper as SaveFieldMapper;
                            parentObject = applicationDefinitionManager.GetAppObject(saveFieldMapper.MappedSaveField.AppObject);
                            targetField = parentObject.GetField(saveFieldMapper.MappedSaveField.FieldId);
                            break;
                    }

                    TaskDialogResult dr = ApttusMessageUtil.ShowWarning(string.Format(resourceManager.GetResource("LookAhead_lnkAddSearchAndSelect_LinkClicked_ObjectMissing"), targetField.LookupObject.Id),
                                                                            resourceManager.GetResource("LOOKAHEADPROPUI_Title"),
                                                                            ApttusMessageUtil.YesNo);
                    if (dr == TaskDialogResult.Yes)
                    {
                        ApttusObject apttusObjectToAdd = ObjectManager.GetInstance.GetApttusObject(targetField.LookupObject.Id, true, false);
                        apttusObjectToAdd.ObjectType = ObjectType.Repeating;
                        apttusObjectToAdd = applicationDefinitionManager.AddObject(apttusObjectToAdd);
                        Controller.FieldMapperHelper.AppObjectUniqueId = apttusObjectToAdd.UniqueId;
                    }
                    else if (dr == TaskDialogResult.No)
                    {
                        Show();
                        return;
                    }
                }
                Forms.SearchAndSelectActionView apttusSearchForm = new Forms.SearchAndSelectActionView(Controller.FieldMapperHelper.AppObjectUniqueId);
                DialogResult result = apttusSearchForm.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Cancel)
                {
                    PopulateSearchAndSelectUI();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                Show();
            }
        }
    }
}
