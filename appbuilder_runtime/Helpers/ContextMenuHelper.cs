using Apttus.XAuthor.Core;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class ContextMenuHelper
    {
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private static ContextMenuHelper instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// Created Constructor private to implement singleton behaviour 
        /// </summary>
        private ContextMenuHelper()
        {
        }

        /// <summary>
        /// used to get singletone instance of class.
        /// </summary>
        public static ContextMenuHelper GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ContextMenuHelper();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Validate that selected range is part of repeating range or save field only
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool CanDisplayXAuthorMenu(Excel.Range Target)
        {
            Excel.Range namedRange = ExcelHelper.GetNamedRangeFromCell(Target);

            if (namedRange != null && (ConfigurationManager.GetInstance.GetRepeatingGroupbyTargetNamedRange(namedRange.Name.Name) != null || ConfigurationManager.GetInstance.GetSaveOtherFieldNameRange(namedRange.Name.Name) != null))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// validate that selected range is part of X-Author range or dsplaying native insert/delete can cause data integrity issue to aside or below range with respect of rendering type.
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool CanDisplayNativeInsertDeleteCommands(Excel.Range Target)
        {
            // if user is within X-Author range then return false
            Excel.Range namedRange = ExcelHelper.GetNamedRangeFromCell(Target, false);
            if (namedRange != null)
                return false;

            // Check for Bottom or Right Named Ranges, if XAuthor range exists below current selection then return false otherwise user can use shift cells down command which can 
            // cause data integrity issue.
            if (ExcelHelper.DoesNamedRangeExistsInBottomOrRight(Target))
                return false;

            return true;
        }

        /// <summary>
        /// Reset context menu, it will remove customization from context menu & make it make to original state.
        /// </summary>
        public void ResetContextMenus()
        {
            try
            {
                var cellContextMenu = ExcelHelper.ExcelApp.Application.CommandBars["cell"];
                if (cellContextMenu != null)
                    cellContextMenu.Reset();
            }
            catch(Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Remove Native Insert/Delete Commands from  Cell Context Menu
        /// </summary>
        public void RemoveNativeInsertDelete()
        {
            //Hide From Cell Menu
            try
            {
                var cellContextMenu = ExcelHelper.ExcelApp.Application.CommandBars["cell"];
                cellContextMenu.Controls["&Delete..."].Delete();
                cellContextMenu.Controls["&Insert..."].Delete();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Creates Add Row Commands for context menu  
        /// </summary>
        /// <param name="contextMenu"></param>
        public void DisplayXAuthorAddRowMenu(CommandBar contextMenu)
        {
            // Check if Add Row is enabled 
            if (ConfigurationManager.GetInstance.Menus == null || !ConfigurationManager.GetInstance.Menus.EnableAddRowMenu)
                return;

            MsoControlType menuItem = MsoControlType.msoControlButton;

            // Create Add Row Menu
            CommandBarPopup addMenuPopup = (CommandBarPopup)contextMenu.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            addMenuPopup.Caption = "Add Rows";

            var add1Rows = addMenuPopup.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
            add1Rows.Caption = "Add Row";
            add1Rows.Click += Add1Rows_Click;

            var add3Rows = addMenuPopup.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
            add3Rows.Caption = "Add 3 Rows";
            add3Rows.Click += Add3Rows_Click;

            var add5Rows = addMenuPopup.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
            add5Rows.Caption = "Add 5 Rows";
            add5Rows.Click += Add5Rows_Click;

            var add10Rows = addMenuPopup.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
            add10Rows.Caption = "Add 10 Rows";
            add10Rows.Click += Add10Rows_Click;

            var addCustomRows = addMenuPopup.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
            addCustomRows.Caption = "Add Custom Rows";
            addCustomRows.Click += AddCustomRows_Click;
        }

        /// <summary>
        /// Creates Remove Row Commands for context menu  
        /// </summary>
        /// <param name="contextMenu"></param>
        public void DisplayXAuthorRemoveRowMenu(CommandBar contextMenu)
        {
            // Check if Remove Row is enabled 
            if (ConfigurationManager.GetInstance.Menus == null || !ConfigurationManager.GetInstance.Menus.EnableDeleteRowMenu || !ApttusCommandBarManager.GetInstance().IsLoggedIn())
                return;

            MsoControlType menuItem = MsoControlType.msoControlButton;

            // Create Delete Row Menu
            var deleteRows = (CommandBarButton)contextMenu.Controls.Add(menuItem, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            deleteRows.Caption = "Delete Rows";
            deleteRows.Click += RemoveRow_Click;
        }

        /// <summary>
        /// Push XAuthor Insert/Delete Menu in cell & Row context menu
        /// </summary>
        public void DisplayXAuthorMenu()
        {
            var contextMenu = ExcelHelper.ExcelApp.Application.CommandBars["cell"];
            DisplayXAuthorAddRowMenu(contextMenu);
            DisplayXAuthorRemoveRowMenu(contextMenu);
        }


        #region Add/Remove Row Code

        private void Add1Rows_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            AddRows(1);
        }

        private void Add10Rows_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            AddRows(10);
        }

        private void Add5Rows_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            AddRows(5);
        }

        private void Add3Rows_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            AddRows(3);
        }

        private void AddCustomRows_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            AddCustomRowsView View = new AddCustomRowsView();
            AddCustomRowsController Controller = new AddCustomRowsController(View);
            int rowsToAdd = Controller.GetCustomRows();
            if (rowsToAdd > 0)
                AddRows(rowsToAdd);
        }

        private void AddRows(int RowsToAdd, bool IsMatrixRowEnabled = false)
        {
            Globals.ThisAddIn.RuntimeRibbon.AddRows(RowsToAdd, IsMatrixRowEnabled);
        }

        private void RemoveRow_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            RemoveRows(ExcelHelper.ExcelApp.Selection.Rows.Count);
        }

        private void RemoveRows(int RowsToRemove)
        {
            Globals.ThisAddIn.RuntimeRibbon.RemoveRows(RowsToRemove);
        }
        #endregion
    }
}
