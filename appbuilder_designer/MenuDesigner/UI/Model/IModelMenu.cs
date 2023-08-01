/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    interface IModelMenu
    {
        MenuGroup AddMenu();
      
        MenuItem AddMenuItem(MenuGroup grp);
        bool UpdateMenu(MenuGroup mnu);
        bool UpdateMenuItem(MenuGroup mnu, MenuItem menuitem);
        bool DeleteMenuItem(MenuGroup mnu, MenuItem menuitem);
        bool DeleteMenu(MenuGroup mnu);
        void SaveMenu(bool AddRow, bool DeleteRow, bool Chatter, bool AddMatrixColumn);
        List<MenuGroup> GetMenusCollection();
        Menus GetMenus();
    }
}
