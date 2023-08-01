using Apttus.XAuthor.AppDesigner.MenuDesigner.UI.Controller;
using Apttus.XAuthor.AppDesigner.MenuDesigner.UI.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
namespace Apttus.XAuthor.AppDesigner.MenuDesigner
{
    class MenuLoader
    {
        private MenuManager MnuMgr;
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        public MenuLoader()
        {
            MnuMgr = new MenuManager();
            //List<Control> ControlList = new List<Control>();
           
           // ShowMenuDesigner();
        }

        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }



        List<Control> ControlList = new List<Control>();
        private void GetAllControls(Control container)
        {
            foreach (Control c in container.Controls)
            {
                GetAllControls(c);
                if (c.Tag != null)
                ControlList.Add(c);
            }
        }

        public void XlateAll(Control c)
        {
            GetAllControls(c);
            int i = ControlList.Count;
            i = 0;
            foreach (Control ct in ControlList)
            {
                ct.Text = "hhh" + i++;
            }
        }

        public void ShowMenuDesigner()
        {
           
           // MenuHome MnuHome = new MenuHome();
            //AppBuilderMenuGroup MnuGrp = new AppBuilderMenuGroup();

            ucMenuDesigner uc = new ucMenuDesigner();

            MenuController ctrl = new MenuController(uc);
            var c = GetAll(uc,typeof(Label));
            foreach (Control ct in c)
            {
                Label l = ct as Label;
                
            }
        

            uc.SetController(ctrl);
            myCustomTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(uc, "Menu Designer");

            myCustomTaskPane.DockPosition =
       Office.MsoCTPDockPosition.msoCTPDockPositionFloating;
            myCustomTaskPane.Height = 500;
            myCustomTaskPane.Width = 500;

            myCustomTaskPane.DockPosition =
                Office.MsoCTPDockPosition.msoCTPDockPositionRight;
            myCustomTaskPane.Width = 300;
            myCustomTaskPane.Visible = true;


           
            // MnuGrp.Name = "hello";
          //  MenuController ctrl = new MenuController(MnuHome);
           // MnuHome.SetController(ctrl);
           // MnuHome.AttachPropertyGrid(MnuGrp);
           // MnuHome.Show();
        }
    }
    
}
