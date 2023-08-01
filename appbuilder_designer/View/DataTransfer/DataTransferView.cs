/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataTransferView : Form
    {
        private DataTransferController controller;
        private DataTransferMapping ViewModel;
        private BindingSource gridSource;
        private ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        DataSourceValidator dsValidator = new DataSourceValidator();

        public DataTransferView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;            
            gridSource = new BindingSource();
            ViewModel = new DataTransferMapping();
            configManager = ConfigurationManager.GetInstance;
        }


        private void SetCultureData()
        {
            lblTitle.Text = resourceManager.GetResource("DATATRANSVIEW_Title");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
            label1.Text = resourceManager.GetResource("DATATRANSVIEW_label1_Text");
            SourceRange.HeaderText = resourceManager.GetResource("DATATRANSVIEW_SourceRange_HeaderText");
            SourceSheet.HeaderText = resourceManager.GetResource("DATATRANSVIEW_SourceSheet_HeaderText");
            TargetRange.HeaderText = resourceManager.GetResource("DATATRANSVIEW_TargetRange_HeaderText");
            TargetSheet.HeaderText = resourceManager.GetResource("DATATRANSVIEW_TargetSheet_HeaderText");
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Microsoft Excel(xlsx)|*.xlsx";
            dialog.Title = resourceManager.GetResource("DATATRANSVW_Transfer_Msg");

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ClearMappings();
                txtFilePath.Text = dialog.FileName;
                ViewModel.SourceFile = txtFilePath.Text;                
                PopulateGrid(File.Exists(txtFilePath.Text));
            }
        }

        void ClearMappings()
        {
            ViewModel.SourceFile = string.Empty;
            ViewModel.DataTransferRanges.Clear();
        }

        private void LoadMappings()
        {
            
            DataTransferMapping mapping = configManager.Definition.Mapping;
            if (mapping != null)
            {
                bool bFileExists = File.Exists(mapping.SourceFile);
                if (bFileExists)
                    ViewModel.SourceFile = mapping.SourceFile;
                ViewModel.DataTransferRanges.AddRange(mapping.DataTransferRanges);

                txtFilePath.Text = ViewModel.SourceFile;
                PopulateGrid(bFileExists);
            }
        }

        private void PopulateGrid(bool bFileExists)
        {
            List<string> sourceSheets = new List<string>();
            List<string> destSheets = new List<string>();
            
            Excel.Application inMemoryExcelApp = null;
            Excel.Workbooks books = null;
            Excel.Workbook workbook = null;
            try
            {
                if (bFileExists)
                {
                    inMemoryExcelApp = new Excel.Application();
                    inMemoryExcelApp.EnableEvents = false;
                    inMemoryExcelApp.Visible = false;
                    books = inMemoryExcelApp.Workbooks;
                    workbook = books.Open(txtFilePath.Text);
                    //Populate Source Worksheet
                    if (workbook != null)
                    {
                        foreach (Excel.Worksheet sheet in workbook.Worksheets)
                        {
                            if (!sheet.Name.Equals(Constants.METADATA_SHEETNAME))
                                sourceSheets.Add(sheet.Name);
                        }
                    }

                    //Populate Destination Worksheet
                    if (Globals.ThisAddIn.Application.ActiveWorkbook != null)
                    {
                        foreach (Excel.Worksheet sheet in Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets)
                        {
                            if (!sheet.Name.Equals(Constants.METADATA_SHEETNAME))
                                destSheets.Add(sheet.Name);
                        }
                    }

                    dgvDataLoader.Rows.Clear();

                    DataGridViewComboBoxColumn source = dgvDataLoader.Columns["SourceSheet"] as DataGridViewComboBoxColumn;
                    source.FlatStyle = FlatStyle.Standard;
                    source.DataSource = sourceSheets;

                    DataGridViewComboBoxColumn dest = dgvDataLoader.Columns["TargetSheet"] as DataGridViewComboBoxColumn;
                    dest.DataSource = destSheets;
                    dest.FlatStyle = FlatStyle.Standard;

                    if (gridSource.DataSource != null)                    
                        gridSource.Clear();
                }
                else
                {
                    foreach (DataTransferRange rg in ViewModel.DataTransferRanges)
                        sourceSheets.Add(rg.SourceSheet);

                    foreach (DataTransferRange rg in ViewModel.DataTransferRanges)
                        destSheets.Add(rg.TargetSheet);

                    DataGridViewComboBoxColumn source = dgvDataLoader.Columns["SourceSheet"] as DataGridViewComboBoxColumn;
                    source.FlatStyle = FlatStyle.Standard;
                    source.DataSource = sourceSheets;

                    DataGridViewComboBoxColumn dest = dgvDataLoader.Columns["TargetSheet"] as DataGridViewComboBoxColumn;
                    dest.FlatStyle = FlatStyle.Standard;
                    dest.DataSource = destSheets;

                }
                dgvDataLoader.AutoGenerateColumns = false;
                dsValidator.assginSheets(sourceSheets,destSheets);
                List<ValidationResult> result = dsValidator.Validate<DataTransferMapping>(ViewModel);
                if (result.Where(res => res.Success == false).Count() != 0)
                {
                    ErrorMessageViewer viewer = new ErrorMessageViewer();
                    viewer.ErrorMessageHeader = resourceManager.GetResource("RIBBON_btnDataLoader_Label") + " " + resourceManager.GetResource("QUICKAPP_CreatingCap_ErrorMsg");
                    viewer.AddResults(result);
                    viewer.ShowDialog();
                    viewer = null;
                    gridSource.DataSource = dsValidator.source.DataTransferRanges;
                }
                else
                {

                    gridSource.DataSource = ViewModel.DataTransferRanges;
                }
                dgvDataLoader.DataSource = gridSource;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(false);
                    Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }
                if (books != null)
                {
                    Marshal.ReleaseComObject(books);
                    books = null;
                }
                if (inMemoryExcelApp != null)
                {
                    inMemoryExcelApp.Quit();
                    Marshal.ReleaseComObject(inMemoryExcelApp);
                    inMemoryExcelApp = null;
                }
            }
            AddRecordsToGrid();
        }

        private void AddRecordsToGrid()
        {
            List<DataTransferRange> ranges = gridSource.DataSource as List<DataTransferRange>;

            DataGridViewComboBoxColumn source = dgvDataLoader.Columns["SourceSheet"] as DataGridViewComboBoxColumn;
            source.FlatStyle = FlatStyle.Standard;
            List<string> sourceSheets = source.DataSource as List<string>;

            DataGridViewComboBoxColumn target = dgvDataLoader.Columns["TargetSheet"] as DataGridViewComboBoxColumn;
            target.FlatStyle = FlatStyle.Standard;
            List<string> targetSheets = target.DataSource as List<string>;

            foreach (string sourceSheetName in sourceSheets)
            {
                if (targetSheets.Contains(sourceSheetName) && ranges.Where(rg => rg.SourceSheet.Equals(sourceSheetName)).Count() == 0)
                {
                    DataTransferRange rg = new DataTransferRange();
                    rg.SourceSheet = sourceSheetName;
                    rg.TargetSheet = sourceSheetName;
                    rg.TargetRange = GetTargetLocation(sourceSheetName); //sourceSheetName & targetsheetname are both same.
                    gridSource.Add(rg);
                }
            }
        }

        public string GetTargetLocation(string targetSheetName)
        {
            string targetLocation = string.Empty;
            try
            {
                string targetNamedRange = (from rm in configManager.RetrieveMaps
                                           from rg in rm.RepeatingGroups
                                           where rg.RetrieveFields.Where(rf => rf.TargetLocation.Contains(targetSheetName)).FirstOrDefault() != null
                                           select rg.TargetNamedRange).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(targetNamedRange))
                {
                    Guid namedRange = (from sm in configManager.SaveMaps
                                       from sg in sm.SaveGroups
                                       from sf in sm.SaveFields
                                       where sf.DesignerLocation.Contains(targetSheetName) && sf.GroupId.Equals(sg.GroupId)
                                       select sg.GroupId).FirstOrDefault();

                    if (namedRange.Equals(Guid.Empty))
                        return targetLocation;

                    targetNamedRange = namedRange.ToString();

                }
                Excel.Range range = ExcelHelper.GetRange(targetNamedRange);
                if (range == null)
                    return string.Empty;

                int nNumberOfRowsToSkip = 2; //We skip 2 rows. 1. First row to skip is Label Row. 2. Second row to skip is formula row. Data Transfer will start immediately 
                //after skipping 2 rows. 
                //IMPORTANT : Changing the value to other than 2, will effect the runtime and most likely the runtime of data transfer will also fail.
                targetLocation = ExcelHelper.NextVerticalCell(range, nNumberOfRowsToSkip).AddressLocal;
            }
            catch (Exception ex)
            {
                targetLocation = string.Empty;
            }
            return targetLocation;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Remove all DataTranferRange which have sourcerange and targetrange as empty.
            dsValidator.Validate<DataTransferMapping>(ViewModel);
            ViewModel = dsValidator.source;
            int nRemovedElements = ViewModel.DataTransferRanges.RemoveAll(rg => String.IsNullOrWhiteSpace(rg.SourceRange) || String.IsNullOrWhiteSpace(rg.TargetRange));
            controller.Save(ViewModel);
        }

        internal void SetController(DataTransferController dataTransferController)
        {
            controller = dataTransferController;
        }

        private void DataTransferView_Load(object sender, EventArgs e)
        {
            LoadMappings();
        }    
    }
}
