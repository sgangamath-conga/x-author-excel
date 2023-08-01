/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    //Manages the current menu structure


    public class MenuManager
    {

        private Menus AppBuilderMenus;
        public MenuManager()
        {

            if (ConfigurationManager.GetInstance.Menus != null)
            {
                AppBuilderMenus = ConfigurationManager.GetInstance.Menus;
            }
            else
                AppBuilderMenus = new Menus();

        }
        public Menus APPMenu
        {
            get { return AppBuilderMenus; }
        }
        public MenuGroup AddMenu(MenuGroup MenuGroup)
        {

            // if (!(ItemExists(MenuGroup.Name)))
            // {
            AppBuilderMenus.MenuGroupsCollection.Add(MenuGroup);
            // }
            // else
            // {
            //    string caption = ApttusResourceMgr.GetManager().GetString("msgDuplicate");

            //     ApttusMessageUtil.ShowError(caption,ApttusResourceMgr.GetManager().GetString("TitleDuplicate"));
            //     return null;

            //}
            return MenuGroup;
        }

        public MenuItem AddMenuItem(MenuGroup MenuGroup, MenuItem menuItem)
        {

            //add menu and 
            //do basic validation
            if (MenuGroup == null || MenuGroup == null)
            {
                throw new ArgumentNullException(MenuGroup.Name);
            }
            else if (menuItem == null)
                throw new ArgumentNullException(menuItem.Name);

            if (!IsEmpty())
            {
                // first get the grp
                // add the item in the menu list
                MenuGroup grp = FindMenuGrp(MenuGroup);
                if (grp != null)
                {
                    MenuItem item = grp.MenuItems.Find(Item => Item.Name.Equals(menuItem.Name));
                    if (item != null)
                    {
                        string caption = ApttusResourceManager.GetInstance.GetResource("MNUHOME_msgDuplicate");

                        ApttusMessageUtil.ShowError(caption, ApttusResourceManager.GetInstance.GetResource("MNUHOME_TitleDuplicate"));
                        return null;
                    }
                    return grp.AddItem(menuItem);
                }
                else //TODO report error
                {
                }
            }

            return menuItem;
        }

        public bool IsEmpty()
        {
            return AppBuilderMenus.MenuGroupsCollection.Count <= 0 ? true : false;
        }

        private MenuGroup FindMenuGrp(MenuGroup grp)
        {
            MenuGroup mnuGroup = null;
            if (!IsEmpty())
            {
                return AppBuilderMenus.MenuGroupsCollection.Find(Item => Item.Id.Equals(grp.Id));
            }
            return mnuGroup;
        }

        public bool DeleteMenuGroup(MenuGroup grp)
        {
            if (grp != null)
                return (AppBuilderMenus.MenuGroupsCollection.Remove(grp));
            return false;
        }

        public bool DeleteMenuItem(MenuGroup grp, MenuItem item)
        {
            MenuGroup currentGrp = FindMenuGrp(grp);
            if (currentGrp != null)
            {
                return currentGrp.DeleteItem(item);

            }
            return true;
        }

        public bool UpdateMenuGroup(MenuGroup grp)
        {
            if (AppBuilderMenus.MenuGroupsCollection.Remove(grp))
            {
                AppBuilderMenus.MenuGroupsCollection.Add(grp);
            }

            return true;
        }

        public bool UpdateMenuItem(MenuGroup grp, MenuItem item)
        {
            MenuGroup currentGrp = FindMenuGrp(grp);
            if (currentGrp != null)
            {
                return currentGrp.EditItem(item);

            }
            else
            { //TODO report error
            }

            return false;
        }

        private bool ItemExists(string id)
        {
            var names = new HashSet<String>();
            foreach (MenuGroup grp in AppBuilderMenus.MenuGroupsCollection)
                if (grp.Name.Equals(id))
                    return true;
            return false;
        }

        public void FindMenuItem(AbstractMenu grp)
        {
            AbstractMenu menu;
            if (!IsEmpty())
            {
                menu = AppBuilderMenus.MenuGroupsCollection.Find(Item => Item.Id.Equals(grp.Id));
            }
        }

        public void WriteMenu(bool bAddRow, bool bDeleteRow, bool bChatter, bool bAddMatrixColumn)
        {
            AppBuilderMenus.EnableAddRowMenu = bAddRow;
            AppBuilderMenus.EnableDeleteRowMenu = bDeleteRow;
            AppBuilderMenus.EnableChatterMenu = bChatter;            
            AppBuilderMenus.EnableAddMatrixColumnMenu = bAddMatrixColumn;            
            AppBuilderMenus.WriteMenu();
        }

    }
}
