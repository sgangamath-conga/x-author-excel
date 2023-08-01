using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ShowFilterObjectView : UserControl
    {
        private RetrieveMap retrieveMap { get; set; }
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private List<ShowQueryFilterObjectConfiguration> ShowQueryFilterObjectConfigurations;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ShowFilterObjectView(RetrieveMap rMap, List<ShowQueryFilterObjectConfiguration> showQueryFilterObjectConfigurations)
        {
            retrieveMap = rMap;
            ShowQueryFilterObjectConfigurations = showQueryFilterObjectConfigurations;
            InitializeComponent();
        }

        public List<ShowQueryFilterObjectConfiguration> GetObjectConfigurations()
        {
            if (ShowQueryFilterObjectConfigurations != null)
                ShowQueryFilterObjectConfigurations.Clear();

            for (int row = 1; row < tblObjectConfiguration.RowCount; ++row)
            {
                ShowQueryFilterObjectConfiguration obj = GetQueryFilterObject(row, 0);
                if (obj != null) //If it is null, it is not selected.
                {
                    obj.CellReferenceValue = GetValueFromControlPosition(row, 2);
                    ShowQueryFilterObjectConfigurations.Add(obj);
                }
            }
            return ShowQueryFilterObjectConfigurations;
        }

        private ShowQueryFilterObjectConfiguration GetObjectConfiguration(int row)
        {
            return tblObjectConfiguration.GetControlFromPosition(0, row).Tag as ShowQueryFilterObjectConfiguration;
        }

        private ShowQueryFilterObjectConfiguration GetQueryFilterObject(int row, int col)
        {
            CheckBox chk = tblObjectConfiguration.GetControlFromPosition(col, row) as CheckBox;
            return chk.Checked ? chk.Tag as ShowQueryFilterObjectConfiguration : null;
        }

        private string GetValueFromControlPosition(int row, int col)
        {
            return (tblObjectConfiguration.GetControlFromPosition(col, row) as TextBox).Text;
        }

        private void AddObjectConfiguration(Guid uniqueId, string objectType, int rowNo)
        {
            bool bInitiallyChecked = true;
            ShowQueryFilterObjectConfiguration obj = ShowQueryFilterObjectConfigurations.Find(o => o.AppObjectUniqueId == uniqueId); //= new ShowQueryFilterObjectConfiguration { AppObjectUniqueId = uniqueId };
            if (obj == null) //Create Mode it will be null. Edit Mode it won't be null
            {
                obj = new ShowQueryFilterObjectConfiguration { AppObjectUniqueId = uniqueId };
                obj.CellReferenceValue = string.Empty;
                bInitiallyChecked = false;
            }

            CheckBox chkSelection = new CheckBox();
            chkSelection.Checked = bInitiallyChecked;
            chkSelection.Text = string.Empty;
            chkSelection.Tag = obj;
            chkSelection.Dock = DockStyle.Fill;
            chkSelection.AutoSize = true;

            TextBox txtObject = new TextBox();
            txtObject.Dock = DockStyle.Fill;
            txtObject.Text = appDefManager.GetAppObject(uniqueId).Name + " (" + objectType + ")";
            txtObject.Tag = uniqueId;
            txtObject.ReadOnly = true;

            TextBox txtCellReference = new TextBox();
            txtCellReference.Dock = DockStyle.Fill;
            txtCellReference.BackColor = Color.White;
            txtCellReference.Enabled = bInitiallyChecked;
            txtCellReference.Text = obj.CellReferenceValue;

            tblObjectConfiguration.Controls.Add(chkSelection, 0, rowNo);
            tblObjectConfiguration.Controls.Add(txtObject, 1, rowNo);
            tblObjectConfiguration.Controls.Add(txtCellReference, 2, rowNo);
            chkSelection.CheckedChanged += chkSelection_CheckedChanged;

            tblObjectConfiguration.RowCount++;
        }

        public void ShowFilters()
        {
            if (ShowQueryFilterObjectConfigurations == null)
                ShowQueryFilterObjectConfigurations = new List<ShowQueryFilterObjectConfiguration>();

            List<Guid> independentAppObjectUniqueIds = retrieveMap.RetrieveFields.GroupBy(s => s.AppObject).Select(k => k.Key).ToList();
            int rowNo = 1;
            string individual = resourceManager.GetResource("UCMATRIXVIEW_cboObjectType_Items"); //Individual
            foreach(Guid uniqueId in independentAppObjectUniqueIds)
            {
                AddObjectConfiguration(uniqueId, individual, rowNo);
                rowNo++;                
            }

            string list = resourceManager.GetResource("UCMATRIXVIEW_cboObjectType_Items1"); //List
            foreach(RepeatingGroup rGroup in retrieveMap.RepeatingGroups)
            {
                AddObjectConfiguration(rGroup.AppObject, list, rowNo);
                rowNo++;
            }

            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tblObjectConfiguration.Padding = new Padding(0, 0, vertScrollWidth, 0);
        }

        void chkSelection_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkSelection = (sender as CheckBox);
            TableLayoutPanelCellPosition pos = tblObjectConfiguration.GetPositionFromControl(chkSelection);
            TextBox txtCellReference = tblObjectConfiguration.GetControlFromPosition(2, pos.Row) as TextBox;
            txtCellReference.Enabled = chkSelection.Checked;
        }


        private void ShowFilterObjectView_Load(object sender, EventArgs e)
        {
            ShowFilters();
        }

        private void ShowFilterObjectView_Validating(object sender, CancelEventArgs e)
        {
            bool bIsCellReferenceValid = true;
            for (int row = 1; row < tblObjectConfiguration.RowCount; ++row)
            {
                CheckBox chkSelected = tblObjectConfiguration.GetControlFromPosition(0, row) as CheckBox;
                if (chkSelected.Checked)
                {
                    TextBox txtCellReference = tblObjectConfiguration.GetControlFromPosition(2, row) as TextBox;
                    bIsCellReferenceValid = IsCellReferenceValid(chkSelected.Tag as ShowQueryFilterObjectConfiguration, txtCellReference.Text);
                    if (!bIsCellReferenceValid)
                    {
                        errorProvider.SetError(txtCellReference, "Invalid Cell Reference");
                        break;
                    }
                    else
                    {
                        errorProvider.SetError(txtCellReference, string.Empty);
                    }
                }
            }
            e.Cancel = !bIsCellReferenceValid;
        }

        private bool IsCellReferenceValid(ShowQueryFilterObjectConfiguration obj, string cellReferenceValue)
        {
            bool isCellReferenceValid = false;

            int lastIndexOfAt = cellReferenceValue.LastIndexOf(@"!");

            if ((!string.IsNullOrEmpty(cellReferenceValue)) && lastIndexOfAt > 0
                && lastIndexOfAt < cellReferenceValue.Length)
            {
                string sheetName = cellReferenceValue.Substring(0, lastIndexOfAt);
                var workSheet = ExcelHelper.GetWorkSheet(sheetName);
                if (workSheet != null)
                {
                    // Worksheet exists, Match the cell reference pattern
                    string cellLocation = cellReferenceValue.Substring(lastIndexOfAt + 1);
                    Regex regex = new Regex(@"(\$?[A-Za-z]{1,3})(\$?[0-9]{1,6})");
                    isCellReferenceValid = regex.IsMatch(cellLocation);
                    obj.CellReferenceType = CellReferenceType.CellLocation;
                }
            }

            if (!isCellReferenceValid)
            {
                Excel.Range oNameRange = ExcelHelper.GetRange(cellReferenceValue);
                isCellReferenceValid = oNameRange != null;
                obj.CellReferenceType = CellReferenceType.NamedRange;
            }
            return isCellReferenceValid;
        }
    }
}
