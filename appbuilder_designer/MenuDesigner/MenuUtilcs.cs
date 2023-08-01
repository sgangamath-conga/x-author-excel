/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Office = Microsoft.Office.Core;

namespace Apttus.XAuthor.AppDesigner
{
    class MenuUtilcs
    {
        private MenuManager MnuMgr;
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        public void ShowMenuDesigner()
        {

            // MenuHome MnuHome = new MenuHome();
            //AppBuilderMenuGroup MnuGrp = new AppBuilderMenuGroup();

            MenuHome uc = new MenuHome();

            MenuController ctrl = new MenuController(uc);
           

            uc.SetController(ctrl);
            //string caption = ApttusResourceMgr.GetManager().GetString("TitleMenuDesigner");
            uc.ShowDialog();


            // MnuGrp.Name = "hello";
            //  MenuController ctrl = new MenuController(MnuHome);
            // MnuHome.SetController(ctrl);
            // MnuHome.AttachPropertyGrid(MnuGrp);
            // MnuHome.Show();
        }
    }
}
