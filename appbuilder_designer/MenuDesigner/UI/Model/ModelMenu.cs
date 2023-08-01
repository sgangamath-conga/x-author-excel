/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    class ModelMenu : IModelMenu
    {
        private MenuManager MnuMgr;
        public ModelMenu()
        {
            MnuMgr = new MenuManager();
        }

        public Menus GetMenus()
        {
            return MnuMgr.APPMenu;
        }

        public List<MenuGroup> GetMenusCollection()
        {
            return MnuMgr.APPMenu.MenuGroupsCollection;
        }

        public void RemoveMenu(string id)
        {
            throw new NotImplementedException();
        }

        public MenuGroup AddMenu()
        {
            MenuGroup grp = new MenuGroup();
            // MenuGroupWrapper();
            grp.Name = "Default_Group";

            return MnuMgr.AddMenu((MenuGroup)grp);
        }

        public MenuItem AddMenuItem(MenuGroup grp)
        {
            MenuItem menu = new MenuItem();
            menu.Name = "Default_Item";

            return MnuMgr.AddMenuItem(grp, menu);
        }

        public bool UpdateMenuItem(MenuGroup grp, MenuItem item)
        {
            return MnuMgr.UpdateMenuItem(grp, item);
        }

        public bool UpdateMenu(MenuGroup mnu)
        {
            return MnuMgr.UpdateMenuGroup(mnu);
        }
        public bool DeleteMenuItem(MenuGroup grp, MenuItem item)
        {
            return MnuMgr.DeleteMenuItem(grp, item);
        }

        public bool DeleteMenu(MenuGroup grp)
        {
            return MnuMgr.DeleteMenuGroup(grp);
        }

        public void SaveMenu(bool bAddRow, bool bDeleteRow, bool bChatter, bool bAddMatrixColumn)
        {
            MnuMgr.WriteMenu(bAddRow, bDeleteRow, bChatter, bAddMatrixColumn);
        }
    }
}
