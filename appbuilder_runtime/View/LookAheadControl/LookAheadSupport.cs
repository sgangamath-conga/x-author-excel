/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.AppRuntime;
using System.Data;
using Apttus.XAuthor.Core;

namespace LookAhead
{

    public class LookAheadSupport
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
        private RichTextBox mGUIFormula;
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        private static extern bool GetCursorPos(out Point lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        public static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);
        enum ShowWindowCommand : int
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window 
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>       
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value 
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
            /// the window is not actived.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position. 
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level 
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is 
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or 
            /// maximized, the system restores it to its original size and position. 
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
            /// that owns the window is not responding. This flag should only be 
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        const string LHTitle = Apttus.XAuthor.Core.Constants.RUNTIME_PRODUCT_NAME;

        public Excel.Range LARange = null;

        public LookAheadSupport(RichTextBox GUI, System.Drawing.Point location)
        {
            mGUIFormula = GUI;

        }
        public virtual void SearchValues(string txtSearchValue, DataGridView dgvSource) { ; }
        public virtual void ClearValues(DataGridView dgvSource) { ; }
        public virtual object ReadSelectedValuesfromGrid(int row) { return null; }


        public List<string> LookUpData
        {
            get;
            set;
        }
        public LookAheadSupport()
        {

        }
        string outputval;
        public void output(string val)
        {
            outputval = val;
        }
        public string outVal
        {
            get { return outputval; }
        }

        public System.Drawing.Point GetCellPosition(Range range)
        {
            Worksheet ws = range.Worksheet;
            IntPtr hdc = GetDC((IntPtr)0);
            long px = GetDeviceCaps(hdc, LOGPIXELSX);
            long py = GetDeviceCaps(hdc, LOGPIXELSY);
            ReleaseDC((IntPtr)0, hdc);
            double zoom = range.Application.ActiveWindow.Zoom;

            var pointsPerInch = range.Application.Application.InchesToPoints(1); // usually 72 
            var zoomRatio = zoom / 100;
            var x = range.Application.ActiveWindow.PointsToScreenPixelsX(0);

            // Coordinates of current column 
            x = Convert.ToInt32(x + range.Left * zoomRatio * px / pointsPerInch);

            // Coordinates of next column 
            //x = Convert.ToInt32(x + (((Range)(ws.Columns)[range.Column]).Width + range.Left) * zoomRatio * px / pointsPerInch); 
            var y = range.Application.ActiveWindow.PointsToScreenPixelsY(0);
            y = Convert.ToInt32(y + range.Top * zoomRatio * py / pointsPerInch);

            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(range);

            return new System.Drawing.Point(x, y);
        }
        protected LookupSupport UI
        {
            get;
            set;
        }
        public virtual void KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                UI.Close();
                e.Handled = true;

            }
        }
        IntPtr Mehwnd;

        public void Hide()
        {
            Mehwnd = FindWindow(null, LHTitle);
            ShowWindow(Mehwnd, ShowWindowCommand.Hide);
        }

        public void Show(Range Target)
        {
            Display(Target, null);
        }

        public void Display(Range Target, List<string> DataSource)
        {
            LARange = Target;

            LookAheadResult LHResult = new LookAheadResult(Target);
            if (UI == null)
            {
                UI = new LookupSupport(LHResult);
                UI.DataIn = DataSource;
                UI.Supp = this;

            }
            // default add the current value in the text
            if (Target.Value2 != null)
            {
                UI.TxtSearch.Text = Target.Value2;
            }

            //int w = Screen.PrimaryScreen.WorkingArea.Width;
            //int bootm = Screen.PrimaryScreen.WorkingArea.Bottom;
            //int Right = Screen.PrimaryScreen.WorkingArea.Right;

            //  ShowWindow(lFormulaBarhwnd, ShowWindowCommand.Hide);

            //UI.Location = LA.GetCellPosition(Target);
            //Excel.Range rng = Target.Application.Cells[Target.Row + 1, Target.Column];
            //System.Drawing.Point p1 = GetCellPosition(rng);

            //int x1 = p1.X  + 60; ;
            //int y1 = p1.Y  + 50;
            //if (y1 > 600)
            //{

            //    y1 = 500;
            //}
            //if (x1 > 1400)
            //{

            //    x1 = Screen.PrimaryScreen.WorkingArea.Right - 200;
            //}
            //UI.Show();
            //UI.TopMost = true;

            //ShowWindow(Mehwnd, ShowWindowCommand.Hide);
            //  UI.Hide();
            UI.Supp = this;

            //ShowWindow(Mehwnd, ShowWindowCommand.Show);
            //SwitchToThisWindow(Mehwnd, true);
            //SetForegroundWindow(Mehwnd);
            //UI.Focus();
            //UI.TxtSearch.Select();
            //UI.TxtSearch.Focus();
            UI.Parent = Control.FromHandle(new IntPtr(Globals.ThisAddIn.Application.Hwnd));
            UI.ShowDialog();
        }

        public void MoveLookAheadWindow()
        {
            Excel.Range rng = LARange.Application.Cells[LARange.Row + 1, LARange.Column];
            System.Drawing.Point p1 = GetCellPosition(rng);

            int x1 = p1.X + 5;
            int y1 = p1.Y + 5;
            if (y1 > 600)
                y1 = 500;

            if (x1 > 1400)
                x1 = Screen.PrimaryScreen.WorkingArea.Right - 200;

            IntPtr Mehwnd = FindWindow(null, LHTitle);

            MoveWindow(Mehwnd, x1, y1, UI.Width, UI.Height, true);
        }

    }

    public interface LookAheadCallBack
    {
        void GetCurrentSelection(string selection);
        void GetCurrentSelectionFromMultiCol(int row, LooAheadMultiColSupportHelper Support);
    }
    public class LookAheadResult : LookAheadCallBack
    {
        Excel.Range TargetValue;
        public LookAheadResult(Excel.Range Target)
        {
            TargetValue = Target;
        }
        public void GetCurrentSelection(string selectedValue)
        {
            if (!string.IsNullOrEmpty(selectedValue))
            {
                TargetValue.Value2 = selectedValue;
            }
        }

        public void GetCurrentSelectionFromMultiCol(int row, LooAheadMultiColSupportHelper Support)
        {
            object SelectedValue = Support.ReadSelectedValuesfromGrid(row);
            ReturnDataCollection retData = Support.LookAheadSelection;

            if (SelectedValue != null)
            {
                TargetValue.Value = SelectedValue;
                if (retData != null && retData.DataCollection != null && retData.DataCollection.Count > 0)
                {
                    // update data into other cells from the return data collection
                    // the col index will be available for each data and row can be determind
                    // from the TargetValue
                    foreach (ReturnColDataResult retResult in retData.DataCollection)
                    {
                        int TargetCol; // = retResult.TargetColumn;
                        int TargetRow;
                        string TgtResult = retResult.ResultData;

                        Range TargetRange;
                        if (retResult.TargetNR != null)
                        {
                            TargetRange = ExcelHelper.GetRange(retResult.TargetNR);
                            // get the same row and col for independent
                            if (retResult.FldType == ObjectType.Independent)
                            {
                                TargetCol = TargetRange.Column;
                                TargetRow = TargetRange.Row;
                            }
                            else // repeating needs to take the Target Range row and Target Value col because the row will remain the same 
                            {
                                TargetCol = TargetRange.Column;
                                TargetRow = TargetValue.Row;
                            }

                            Range DestRange = TargetValue.Application.Cells[TargetRow, TargetCol];
                            DestRange.Value2 = TgtResult;
                        }
                    }
                }
            }
        }
    }

    public class LooAheadMultiColSupportHelper : LookAheadSupport
    {
        private System.Data.DataTable masterDataSource;
        private System.Data.DataTable currentDataSource;
        private List<string> mColumnNames = new List<string>();
        private ReturnDataCollection LookAheadResutls = new ReturnDataCollection();

        public ReturnDataCollection LookAheadSelection
        {
            get { return LookAheadResutls; }
            set { LookAheadSelection = LookAheadResutls; }
        }

        public static string ColumnFilter
        {
            get;
            set;
        }

        public ExcelMultiColumProp LHProp
        {
            get;
            set;
        }

        public override void SearchValues(string txtSearchValue, DataGridView dgvSource)
        {
            if (string.IsNullOrEmpty(txtSearchValue)) return;
            if (masterDataSource == null || masterDataSource.Columns.Count == 0) return;

            DataRow[] FoundRows = Search(txtSearchValue);
            System.Data.DataTable ResultTable = new System.Data.DataTable();
            if (FoundRows != null && FoundRows.Length > 0)
            {
                currentDataSource = FoundRows.CopyToDataTable();
                if (dgvSource != null)
                {
                    int indRow = masterDataSource.Rows.IndexOf(FoundRows[0]);
                    dgvSource.DataSource = currentDataSource;
                }
            }
            else
            {
                // no rows found, clear DS
                dgvSource.DataSource = null;
            }
        }

        public override void ClearValues(DataGridView dgvSource)
        {
            // clear the search field and reset the data source
            if (dgvSource != null)
            {
                dgvSource.DataSource = masterDataSource;
                currentDataSource = masterDataSource;
            }
        }

        private int GetColumnNumer(string RangeVal, string col)
        {
            Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetRange(RangeVal);
            // since the data table column name is not the same as the designer col name
            // find out the matching col with designer
            int counter = -1;
            foreach (Microsoft.Office.Interop.Excel.Range rng in CurrentRange.Columns)
            {
                counter++;
                string ColumnLetter = ExcelHelper.NumberToColum(rng.Column);
                if (ColumnLetter.Equals(col))
                    break;

            }
            return counter;
        }

        public override object ReadSelectedValuesfromGrid(int row)
        {
            // value could be a record id, so return as object
            try
            {
                int colNumber = -1;
                object rowValue = null;
                if (LHProp != null && masterDataSource != null && masterDataSource.Rows.Count > 0)
                {
                    // Use currentDataSource here, as after Search Row index has changed, using mDataSource was giving incorrect results in runtime.
                    // i.e. user had selected Record X, but on Excel System used populate Record Y values.
                    DataRow dRow = currentDataSource.Rows[row];
                    colNumber = GetColumnNumer(LHProp.TargetRange, LHProp.ReturnCol);
                    if (colNumber > -1)
                        rowValue = dRow[colNumber];

                    if (LHProp.ReturnColumnData != null && LHProp.ReturnColumnData.DataCollection.Count > 0)
                    {// populate the result into the collection
                        foreach (ReturnColumnData retCol in LHProp.ReturnColumnData.DataCollection)
                        {
                            ReturnColDataResult retResult = new ReturnColDataResult();
                            colNumber = GetColumnNumer(LHProp.TargetRange, retCol.ExcelDataSourceColumn);

                            retResult.FieldID = retCol.FieldID;
                            if (colNumber > -1)
                                retResult.ResultData = dRow[colNumber] as string;
                            retResult.TargetColumn = retCol.TargetColumn;
                            retResult.TargetNR = retCol.TargetNR;
                            retResult.FldType = retCol.FldType;
                            LookAheadSelection.DataCollection.Add(retResult);
                        }
                    }
                }
                return rowValue;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return null;
            }
        }

        public void DisplayMultiCol(Range Target, System.Data.DataTable DataSource, ExcelMultiColumProp prop)
        {
            // MultiCol
            LARange = Target;
            masterDataSource = DataSource;
            currentDataSource = DataSource;
            LHProp = prop;
            LookAheadResult LHResult = new LookAheadResult(Target);
            if (UI == null)
            {
                UI = new LookupSupport(LHResult);
                UI.PopulateMultiColData(DataSource);
                UI.Supp = this;
                if (ColumnFilter == null || ColumnFilter.Length == 0)
                {
                    BuildSearchFilter();
                }
            }

            UI.Parent = Control.FromHandle(new IntPtr(Globals.ThisAddIn.Application.Hwnd));
            UI.ShowDialog();
        }

        private void BuildSearchFilter()
        {
            StringBuilder query = new StringBuilder();
            bool UseContains = true;
            int colCount = masterDataSource.Columns.Count;

            string likeStatement = (UseContains) ? "Like '%{0}%'" : " Like '{0}%'";
            for (int i = 0; i < colCount; i++)
            {
                string colName = "[" + masterDataSource.Columns[i].ColumnName + "]";
                query.Append(string.Concat("Convert(", colName, ", 'System.String')", likeStatement));
                if (i != colCount - 1)
                    query.Append(" OR ");
            }

            ColumnFilter = query.ToString();
        }

        private DataRow[] Search(string SearchVal)
        {
            DataRow[] tmpRows = null;
            try
            {
                string currFilter = string.Format(ColumnFilter, SearchVal);
                tmpRows = masterDataSource.Select(currFilter);
            }
            catch (Exception ex)
            {
                string s = ex.Message.ToString();
            }
            return tmpRows;
        }

    }
}
