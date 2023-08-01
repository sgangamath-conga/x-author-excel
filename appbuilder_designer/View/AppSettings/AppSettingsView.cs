/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace Apttus.XAuthor.AppDesigner
{
    public partial class AppSettingView : Form
    {
        private ApplicationDefinitionManager applicationDefinitionManager;
        private AppSettingsController controller;
        AppSettings AppSettingsModel;
        List<SheetProtect> lstClone = new List<SheetProtect>();
        List<SheetProtect> lstMerged = new List<SheetProtect>();
        private BindingSource gridSource;
        private ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public AppSettingView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            configManager = ConfigurationManager.GetInstance;
            gridSource = new BindingSource();
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_Save_AMPText");
            chkAllRecordsSaveSuccess.Text = resourceManager.GetResource("APPSETTVIEW_chkAllRecordsSaveSuccess_Text");
            chkAutoSizeColumn.Text = resourceManager.GetResource("APPSETTVIEW_chkAutoSizeColumn_Text");
            chkDisablePrint.Text = resourceManager.GetResource("APPSETTVIEW_chkDisablePrint_Text");
            chkDisableRichtextEditing.Text = resourceManager.GetResource("APPSETTVIEW_chkDisableRichtextEditing_Text");
            chkDisableSaveLocalFile.Text = resourceManager.GetResource("APPSETTVIEW_chkDisableSaveLocalFile_Text");
            chkShowFilters.Text = resourceManager.GetResource("COMMON_DisplayFilters_Text");
            chkSuppressDependent.Text = resourceManager.GetResource("APPSETTVIEW_chkSuppressDependent_Text");
            chkSuppressNoOfRecords.Text = resourceManager.GetResource("APPSETTVIEW_chkSuppressNoOfRecords_Text");
            chkSuppressSave.Text = resourceManager.GetResource("APPSETTVIEW_chkSuppressSave_Text");
            dataGridViewTextBoxColumn1.HeaderText = resourceManager.GetResource("COMMON_Name_Text").Replace(':', ' ');
            dataGridViewTextBoxColumn2.HeaderText = resourceManager.GetResource("COMMON_Password_Text");
            groupBox1.Text = resourceManager.GetResource("COMMON_Format_Text");
            grpDataMigration.Text = resourceManager.GetResource("APPSETTVIEW_grpDataMigration_Text");
            grpLocalSettings.Text = resourceManager.GetResource("COMMON_General_Text");
            grpProtectSheet.Text = resourceManager.GetResource("APPSETTVIEW_grpProtectSheet_Text");
            label1.Text = resourceManager.GetResource("COMMON_MaxColumnWidth_Text");
            lblAppSetting.Text = resourceManager.GetResource("COMMON_AppSettings_Text");
            setRowColorlbl.Text = resourceManager.GetResource("APPSETTVIEW_setRowColor_Text");
            chkIgnorePicklistValidation.Text = resourceManager.GetResource("APPSETTVIEW_ValidatePicklist_Text");
            lblMaxAttachmentSize.Text = resourceManager.GetResource("APPSETTVIEW_lblMaxAttachmentSize_Text");
        }

        /// <summary>
        /// Get Sheet Control
        /// </summary>
        private void BindProtectSheets()
        {
            try
            {
                AppSettingsModel = configManager.Definition.AppSettings;
                if (AppSettingsModel == null)
                {
                    AppSettingsModel = new AppSettings();
                    chkDisableSaveLocalFile.Checked = AppSettingsModel.DisableLocalSaveFile;
                    chkDisablePrint.Checked = AppSettingsModel.DisablePrint;
                    chkDisableRichtextEditing.Checked = AppSettingsModel.DisableRichTextEditing;
                    chkIgnorePicklistValidation.Checked = AppSettingsModel.IgnorePicklistValidation;

                    if (AppSettingsModel.MaxAttachmentSize < 1 || AppSettingsModel.MaxAttachmentSize > 100)
                    {
                        numMaxAttachmentSize.Value = 25;
                    }
                    else
                    {
                        numMaxAttachmentSize.Value = AppSettingsModel.MaxAttachmentSize;
                    }

                    // Suppress Messages for datamigration app
                    chkSuppressNoOfRecords.Checked = AppSettingsModel.SuppressNoOfRecords;
                    chkSuppressDependent.Checked = AppSettingsModel.SuppressDependent;
                    chkSuppressSave.Checked = AppSettingsModel.SuppressSave;
                    chkAllRecordsSaveSuccess.Checked = AppSettingsModel.SuppressAllRecordsSaveSuccess;

                    Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
                    List<string> resultSheet = configManager.GetSheetNames();
                    if (wb != null)
                    {
                        Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                        foreach (Excel.Worksheet oSheet in sheets)
                        {
                            if (!oSheet.Name.Equals(Constants.METADATA_SHEETNAME) && oSheet.ProtectContents && resultSheet.Contains(oSheet.Name))
                            {
                                SheetProtect sheetProtect = new SheetProtect();
                                sheetProtect.SheetName = oSheet.Name;
                                sheetProtect.Password = string.Empty;
                                AppSettingsModel.SheetProtectSettings.Add(sheetProtect);
                            }
                        }
                    }
                    dgvProtectSheet.AutoGenerateColumns = false;
                    gridSource.DataSource = AppSettingsModel.SheetProtectSettings;
                    dgvProtectSheet.DataSource = gridSource;

                }
                else
                {
                    List<string> lstSheet = new List<string>();
                    chkDisableSaveLocalFile.Checked = AppSettingsModel.DisableLocalSaveFile;
                    chkDisablePrint.Checked = AppSettingsModel.DisablePrint;
                    chkDisableRichtextEditing.Checked = AppSettingsModel.DisableRichTextEditing;
                    chkIgnorePicklistValidation.Checked = AppSettingsModel.IgnorePicklistValidation;

                    if (AppSettingsModel.MaxAttachmentSize < 1 || AppSettingsModel.MaxAttachmentSize > 100)
                    {
                        numMaxAttachmentSize.Value = 25;
                    }
                    else
                    {
                        numMaxAttachmentSize.Value = AppSettingsModel.MaxAttachmentSize;
                    }

                    // Suppress message for data migration app
                    chkSuppressNoOfRecords.Checked = AppSettingsModel.SuppressNoOfRecords;
                    chkSuppressDependent.Checked = AppSettingsModel.SuppressDependent;
                    chkSuppressSave.Checked = AppSettingsModel.SuppressSave;
                    chkAllRecordsSaveSuccess.Checked = AppSettingsModel.SuppressAllRecordsSaveSuccess;

                    //Format Settings
                    chkShowFilters.Checked = AppSettingsModel.ShowFilters;
                    chkAutoSizeColumn.Checked = AppSettingsModel.AutoSizeColumns;

                    //row highlighting options
                    if (AppSettingsModel.RowErrorColor != null)
                        rowHighlightColor.BackColor = Color.FromArgb(Convert.ToInt32(AppSettingsModel.RowErrorColor));

                    if (chkAutoSizeColumn.Checked)
                        txtMaxColumnWidth.Text = Convert.ToString(AppSettingsModel.MaxColumnWidth);

                    configManager.Definition.AppSettings.SheetProtectSettings.ForEach((item) => { lstClone.Add(item.Clone()); });
                    AppSettingsModel.SheetProtectSettings.Clear();
                    Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;
                    List<string> resultSheet = configManager.GetSheetNames();

                    if (wb != null)
                    {
                        Excel.Sheets sheets = wb.Sheets as Excel.Sheets;
                        foreach (Excel.Worksheet oSheet in sheets)
                        {
                            if (!oSheet.Name.Equals(Constants.METADATA_SHEETNAME) && oSheet.ProtectContents && resultSheet.Contains(oSheet.Name))
                            {
                                if (!lstClone.Exists(e => e.SheetName.Equals(oSheet.Name)))
                                {
                                    SheetProtect sheetProtect = new SheetProtect();
                                    sheetProtect.SheetName = oSheet.Name;
                                    sheetProtect.Password = string.Empty;
                                    AppSettingsModel.SheetProtectSettings.Add(sheetProtect);
                                }
                            }
                            else
                            {
                                if (lstClone.Exists(e => e.SheetName.Equals(oSheet.Name)))
                                    lstClone.RemoveAll(x => x.SheetName == oSheet.Name);
                            }
                            lstSheet.Add(oSheet.Name);
                        }
                    }

                    for (int i = 0; i < lstClone.Count; i++)
                    {
                        if (!lstSheet.Exists(e => e.Equals(lstClone[i].SheetName)))
                            lstClone.RemoveAll(x => x.SheetName == lstClone[i].SheetName);
                    }
                    var dictProtectSettingModel = AppSettingsModel.SheetProtectSettings.ToDictionary(p => p.SheetName);
                    foreach (var sheetProtect in lstClone)
                    {
                        dictProtectSettingModel[sheetProtect.SheetName] = sheetProtect;
                    }
                    lstMerged = dictProtectSettingModel.Values.ToList();

                    dgvProtectSheet.AutoGenerateColumns = false;
                    gridSource.DataSource = lstMerged;
                    dgvProtectSheet.DataSource = gridSource;
                    AppSettingsModel.SheetProtectSettings = lstMerged;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Set Controller
        /// </summary>
        /// <param name="appSettingsController"></param>
        internal void SetController(AppSettingsController appSettingsController)
        {
            controller = appSettingsController;
        }

        private bool ValidateColumnWidth()
        {
            int maxColWidth = 0;
            bool bParsed = Int32.TryParse(txtMaxColumnWidth.Text, out maxColWidth);
            if (chkAutoSizeColumn.Checked)
            {
                if ((!bParsed || maxColWidth == 0))
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPSETVIEW_ValidateColumnWidth_InfoMsg"), resourceManager.GetResource("COMMON_AppSettings_Text"));
                    txtMaxColumnWidth.Text = string.Empty;
                    ActiveControl = txtMaxColumnWidth;
                    return false;
                }
                else if (maxColWidth > 255)
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPSETVIEW_ValidateColumnWidthMAX_InfoMsg"), resourceManager.GetResource("COMMON_AppSettings_Text"));
                    txtMaxColumnWidth.Text = string.Empty;
                    ActiveControl = txtMaxColumnWidth;
                    return false;
                }
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;

            if (!ValidateColumnWidth())
                return;

            bool bSaveResult = false;
            AppSettingsModel.DisableLocalSaveFile = chkDisableSaveLocalFile.Checked;
            AppSettingsModel.DisablePrint = chkDisablePrint.Checked;
            AppSettingsModel.DisableRichTextEditing = chkDisableRichtextEditing.Checked;
            AppSettingsModel.IgnorePicklistValidation = chkIgnorePicklistValidation.Checked;

            // Suppress Message for data migration app
            AppSettingsModel.SuppressNoOfRecords = chkSuppressNoOfRecords.Checked;
            AppSettingsModel.SuppressDependent = chkSuppressDependent.Checked;
            AppSettingsModel.SuppressSave = chkSuppressSave.Checked;
            AppSettingsModel.SuppressAllRecordsSaveSuccess = chkAllRecordsSaveSuccess.Checked;
            AppSettingsModel.MaxAttachmentSize = numMaxAttachmentSize.Value;

            //Password mismatch Check
            List<string> lstSheet = new List<string>();

            bool blnProtectSheetValidation = ExcelHelper.ValidateSheetForPasswordMismatch(out lstSheet);

            if (blnProtectSheetValidation)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("RIBBON_SheetProtect_ErrorMsg") + string.Join(",", lstSheet.ToArray()), Constants.DESIGNER_PRODUCT_NAME);
            }
            //Format Settings
            AppSettingsModel.AutoSizeColumns = chkAutoSizeColumn.Checked;
            if (AppSettingsModel.AutoSizeColumns)
                AppSettingsModel.MaxColumnWidth = Int32.Parse(txtMaxColumnWidth.Text);
            AppSettingsModel.ShowFilters = chkShowFilters.Checked;
            AppSettingsModel.RowErrorColor = rowHighlightColor.BackColor.ToArgb().ToString();

            if (AppSettingsModel != null && !blnProtectSheetValidation)
                bSaveResult = controller.Save(AppSettingsModel);

            if (bSaveResult)
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPSETVIEW_btnSave_Click_InfoMsg"), Constants.DESIGNER_PRODUCT_NAME);
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        /// <summary>
        /// For Password formatting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProtectSheet_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((((DataGridView)sender).CurrentCell.ColumnIndex == 1))
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.UseSystemPasswordChar = true;
                }
            }
        }

        /// <summary>
        /// For Cell Formatting for Password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvProtectSheet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (((e.Value != null)))
                {
                    e.Value = new string(Convert.ToChar("*"), e.Value.ToString().Length);
                }
            }
        }

        private void AppSettingView_Load(object sender, EventArgs e)
        {
            dgvProtectSheet.Enabled = LicenseActivationManager.GetInstance.IsFeatureAvailable(dgvProtectSheet.Tag.ToString());
            if (dgvProtectSheet.Enabled)
                BindProtectSheets();
        }

        private void chkAutoWidth_CheckedChanged(object sender, EventArgs e)
        {
            txtMaxColumnWidth.Enabled = chkAutoSizeColumn.Checked;
        }

        private void rowHighlightColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rowHighlightColor.BackColor = dialog.Color;
            }
        }
    }
}
