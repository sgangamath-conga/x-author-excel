/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Office.Tools.Ribbon;
using System.Drawing;

using Apttus.XAuthor.Core;
using Apttus.XAuthor.AppRuntime.Helpers;
using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime
{
    public class MenuBuilder
    {

        ConfigurationManager configurationManager;
        private Dictionary<string, AbstractMenuProperties> MenuObjects; // = new Dictionary<string, AbstractMenuProperties>();
        //private Dictionary<string, AbstractMenuProperties> OfflineMenuObjects;
        RibbonFactory ribbRactorty = Globals.Factory.GetRibbonFactory();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        //Microsoft.Office.Tools.Ribbon.RibbonTab tab1;

        public ConfigurationManager Config
        {
            set { configurationManager = value; }
        }

        public MenuBuilder()
        {
            MenuObjects = new Dictionary<string, AbstractMenuProperties>();
            //OfflineMenuObjects = new Dictionary<string, AbstractMenuProperties>();
        }

        #region "Old MenuBuilder"
        //public MenuBuilder(Microsoft.Office.Tools.Ribbon.RibbonTab tab)
        //{
        //    tab1 = tab;
        //}

        //public void ClearMenus()
        //{
        //    // Hide Runtime Edit group
        //    RibbonGroup EditGroup = (from g in tab1.Groups
        //                             where g.Name.Equals("EditGroup")
        //                             select g).FirstOrDefault();
        //    EditGroup.Visible = false;

        //    // Hide Social group
        //    RibbonGroup SocialGroup = (from g in tab1.Groups
        //                               where g.Name.Equals("SocialGroup")
        //                               select g).FirstOrDefault();
        //    SocialGroup.Visible = false;

        //    List<RibbonGroup> ABGroups = (from g in tab1.Groups
        //                                  where g.Name.Contains("ABGroup")
        //                                  select g).ToList();

        //    foreach (RibbonGroup ABGroup in ABGroups)
        //    {
        //        ABGroup.Label = string.Empty;
        //        ABGroup.Visible = false;
        //        int cnt = ABGroup.Items.Count;
        //        for (int i = 0; i < cnt; i++)
        //        {
        //            RibbonButton ribbonButton = ABGroup.Items[i] as RibbonButton;

        //            ribbonButton.Label = string.Empty;
        //            ribbonButton.Click -= varbtnname_Click;

        //            ribbonButton.Tag = null;
        //            ribbonButton.Visible = false;
        //            ribbonButton.OfficeImageId = string.Empty;
        //            ribbonButton.ScreenTip = string.Empty;
        //        }
        //    }
        //}

        //public void BuildMenus(Ribbon ribbonxMLUI)
        //{

        //    Menus runtimeMenu = configurationManager.Menus;
        //    if (runtimeMenu != null)
        //    {
        //        // Show Hide Runtime Ribbon buttons
        //        ShowHideRuntimeRibbon(runtimeMenu);

        //        List<MenuGroup> menuGroup = runtimeMenu.MenuGroupsCollection.OrderBy(item => item.Order).ToList();
        //        int menuGroupsCount = menuGroup.Count;
        //        if (menuGroupsCount == 0) return;
        //        int i = 0;

        //        foreach (MenuGroup grp in menuGroup)
        //        {
        //            i++;
        //            Microsoft.Office.Tools.Ribbon.RibbonGroup grp1 = (from g in tab1.Groups
        //                                                              where g.Name.Contains("ABGroup")

        //                                                              select g).ToList()[i];


        //            grp1.Label = grp.Name;
        //            grp1.Visible = true;

        //            try
        //            {
        //                int menuItemCount = 0;
        //                List<Apttus.XAuthor.Core.MenuItem> Sorteditms = grp.MenuItems.OrderBy(item => item.Order).ToList();
        //                foreach (Core.MenuItem item in Sorteditms)
        //                {
        //                    Microsoft.Office.Tools.Ribbon.RibbonButton varbtnname = grp1.Items[menuItemCount] as Microsoft.Office.Tools.Ribbon.RibbonButton;

        //                    varbtnname.Label = item.Name;
        //                    varbtnname.Click += varbtnname_Click;

        //                    varbtnname.Tag = item;
        //                    varbtnname.Visible = true;

        //                    varbtnname.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
        //                    if (!string.IsNullOrEmpty(item.Icon))
        //                        varbtnname.OfficeImageId = item.Icon;
        //                    if (item.ToolTip != null)
        //                        varbtnname.ScreenTip = item.ToolTip;
        //                    // tip.SetToolTip(varbtnname, item.ToolTip);
        //                    menuItemCount++;
        //                }
        //                i++;
        //            }
        //            catch (Exception ex)
        //            {
        //                ExceptionLogHelper.ErrorLog(ex);
        //            }
        //        }

        //    }

        //}

        //public void RemoveMenus()
        //{
        //    List<RibbonGroup> groups = (List<RibbonGroup>)tab1.Groups;
        //}

        //void varbtnname_Click(object sender, RibbonControlEventArgs e)
        //{
        //    Microsoft.Office.Tools.Ribbon.RibbonButton btn = sender as Microsoft.Office.Tools.Ribbon.RibbonButton;
        //    Core.MenuItem item;
        //    if (btn != null)
        //    {
        //        item = btn.Tag as Core.MenuItem;
        //        WorkflowEngine wf = new WorkflowEngine();
        //        // TODO : ID will pass from runtime menu 
        //        string id = item.WorkFlowID.ToString();
        //        wf.Execute(id);

        //    }
        //}

        //private RibbonGroup BuildGroup(MenuGroup grp)
        //{
        //    Microsoft.Office.Tools.Ribbon.RibbonGroup menuGrp = ribbRactorty.CreateRibbonGroup();
        //    menuGrp.Name = grp.Name;
        //    menuGrp.Label = grp.Name;
        //    foreach (Core.MenuItem item in grp.MenuItems)
        //    {
        //        Microsoft.Office.Tools.Ribbon.RibbonButton button = ribbRactorty.CreateRibbonButton();
        //        button.Name = item.Name;
        //        button.Tag = item;
        //        button.Label = item.Name;
        //        button.Click += button_Click;

        //        menuGrp.Items.Add(button);
        //    }
        //    return menuGrp;
        //}

        //void button_Click(object sender, RibbonControlEventArgs e)
        //{
        //}

        #endregion

        public void InvalidateMenus(Ribbon ribbonxMLUI, bool Clear)
        {
            //List<string> MenuIDs = null;
            if (MenuObjects.Count > 0)
            {
                foreach (KeyValuePair<string, AbstractMenuProperties> kvp in MenuObjects)
                {
                    ribbonxMLUI.RibbonUIXml.InvalidateControl(kvp.Key);
                }
                if (Clear) MenuObjects.Clear();
            }
        }

        public void BuildMenusFromXml(Ribbon ribbonxMLUI)
        {
            string GROUPNAME = "ABGroup";
            string BUTTONNAME = "button";

            AbstractMenuProperties prop = new AbstractMenuProperties();
            Menus runtimeMenu = configurationManager.Menus;
            //  InvalidateMenus(ribbonxMLUI, true);
            if (runtimeMenu != null)
            {
                // Show Hide Runtime Ribbon buttons
                //   ShowHideRuntimeRibbon(runtimeMenu);
                List<MenuGroup> menuGroup = runtimeMenu.MenuGroupsCollection.OrderBy(item => item.Order).ToList();
                int menuGroupsCount = menuGroup.Count;
                if (menuGroupsCount == 0) return;
                int i = 0;

                int menuItemCount = 0;
                foreach (MenuGroup grp in menuGroup)
                {
                    i++;
                    if (i > 1)
                        menuItemCount = (i * 10) - 9; // for each group there are 10 buttons so increment the button id by 10 for each group
                    else
                        menuItemCount += 1;

                    prop = new AbstractMenuProperties();
                    prop.Visible = true;
                    prop.Label = grp.Name;
                    prop.Enable = true;

                    string groupName = GROUPNAME + i;
                    prop.ID = groupName;

                    ribbonxMLUI.MenuBuilder = this;
                    MenuObjects.Add(prop.ID, prop);
                    //string s2 = ribbonxMLUI.GetCustomUI(prop.ID);

                    try
                    {
                        List<Apttus.XAuthor.Core.MenuItem> Sorteditms = grp.MenuItems.OrderBy(item => item.Order).ToList();
                        foreach (Core.MenuItem item in Sorteditms)
                        {
                            //  Microsoft.Office.Tools.Ribbon.RibbonButton varbtnname = grp1.Items[menuItemCount] as Microsoft.Office.Tools.Ribbon.RibbonButton;
                            string buttonName = BUTTONNAME + menuItemCount;
                            prop = new AbstractMenuProperties();
                            prop.Label = item.Name;
                            prop.ID = buttonName;
                            // varbtnname.Click += varbtnname_Click;
                            prop.Icon = item.Icon;
                            prop.objTag = item;
                            prop.Visible = true;
                            prop.Enable = true;
                            prop.Ttip = item.ToolTip;
                            MenuObjects.Add(prop.ID, prop);
                            ribbonxMLUI.RibbonUIXml.InvalidateControl(buttonName);
                            /*
                             if (!string.IsNullOrEmpty(item.Icon))
                                 varbtnname.OfficeImageId = item.Icon;
                             if (item.ToolTip != null)
                                 varbtnname.ScreenTip = item.ToolTip;
                             */
                            // tip.SetToolTip(varbtnname, item.ToolTip);
                            menuItemCount++;
                        }

                        ribbonxMLUI.RibbonUIXml.InvalidateControl(groupName);
                    }

                    catch (Exception ex)
                    {
                        ExceptionLogHelper.ErrorLog(ex);
                    }
                }

                // Add Edit Group
                prop = new AbstractMenuProperties();
                prop.Visible = true;
                prop.Label = "Edit";
                prop.Enable = true;
                prop.ID = "EditGroup";
                MenuObjects.Add(prop.ID, prop);

                // Add Edit Matrix Group
                prop = new AbstractMenuProperties();
                prop.Visible = true;
                prop.Label = "EditMatrix";
                prop.Enable = true;
                prop.ID = "EditMatrixGroup";
                MenuObjects.Add(prop.ID, prop);

                // Add Social Group
                prop = new AbstractMenuProperties();
                prop.Visible = true;
                prop.Label = "Social";
                prop.Enable = true;
                prop.ID = "SocialGroup";
                MenuObjects.Add(prop.ID, prop);

                ribbonxMLUI.RibbonUIXml.InvalidateControl("EditGroup");
                ribbonxMLUI.RibbonUIXml.InvalidateControl("EditMatrix");
                ribbonxMLUI.RibbonUIXml.InvalidateControl("SocialGroup");
            }
        }

        public void InvalidateRibbonControl(Ribbon ribbonxMLUI, string controlId)
        {
            ribbonxMLUI.RibbonUIXml.InvalidateControl(controlId);
        }

        public void InvalidateRibbon(Ribbon ribbonxMLUI)
        {
            ribbonxMLUI.RibbonUIXml.Invalidate();
        }

        public bool Visible(string id)
        {
            if (MenuObjects.ContainsKey(id))
                return MenuObjects[id].Visible;

            return false;
        }

        public bool Enable(string id)
        {
            if (MenuObjects.ContainsKey(id))
                return MenuObjects[id].Enable;
            else
                return false;
        }

        public bool AddRowEnabled()
        {
            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
                return configurationManager.Menus.EnableAddRowMenu;
            else
                return false;
        }

        public bool PasteEnabled()
        {
            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
                // This is same as Add Row currently. If Paste becomes parameterized then this method would read its specific flag from Config
                return configurationManager.Menus.EnableAddRowMenu;
            else
                return false;
        }

        public bool DelRowEnabled()
        {
            // If User is not loggedin disable Delete Row button
            if (!ApttusCommandBarManager.GetInstance().IsLoggedIn())
                return false;

            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
                return configurationManager.Menus.EnableDeleteRowMenu;
            else
                return false;
        }

        public bool EditVisible()
        {
            // If designer disable Edit Action
            if (!string.IsNullOrEmpty(ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_DESIGNER_FILE)))
                return false;

            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
            {
                if (!configurationManager.Menus.EnableAddRowMenu && !configurationManager.Menus.EnableDeleteRowMenu && !configurationManager.Menus.EnableAddMatrixColumnMenu)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public bool SocialVisible()
        {
            // If designer disable Chatter
            if (!string.IsNullOrEmpty(ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_DESIGNER_FILE)))
                return false;

            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
                return configurationManager.Menus.EnableChatterMenu;
            else
                return false;
        }

        ////public bool EditMatrixVisible()
        ////{
        ////    // If designer disable Matrix add row or add column
        ////    if (!string.IsNullOrEmpty(ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_DESIGNER_FILE)))
        ////        return false;

        ////    if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
        ////    {
        ////        if (!configurationManager.Menus.EnableAddMatrixRowMenu && !configurationManager.Menus.EnableAddMatrixColumnMenu)
        ////            return false;
        ////        else
        ////            return true;
        ////    }
        ////    else
        ////        return false;
        ////}
        

        public bool AddMatrixColumnEnabled()
        {
            // If User is not loggedin disable add matrix column button
            if (!ApttusCommandBarManager.GetInstance().IsLoggedIn())
                return false;

            if ((configurationManager != null && configurationManager.Menus != null) && ApttusObjectManager.IsActiveWorkbookRuntime())
                return configurationManager.Menus.EnableAddMatrixColumnMenu;
            else
                return false;
        }

        public void onClick(string id)
        {
            if (!ExcelHelper.GetEditMode())
            {

                if (MenuObjects.ContainsKey(id))
                {
                    Core.MenuItem item = MenuObjects[id].objTag as Core.MenuItem;
                    if (item != null)
                    {
                        WorkflowEngine wf = new WorkflowEngine();
                        // TODO : ID will pass from runtime menu 
                        string wid = item.WorkFlowID.ToString();
                        wf.Execute(wid);
                    }
                }
            }
            else
                ApttusMessageUtil.ShowError(Constants.MESSAGE_EDITMODE, resourceManager.GetResource("RIBBON_MsgEditModeCap_ErrorMsg"));
        }

        public string onGetLabel(string id)
        {
            if (MenuObjects.ContainsKey(id))
                return MenuObjects[id].Label;
            else
                return "";
        }

        public string OnSreenTip(string id)
        {
            if (MenuObjects.ContainsKey(id))
                return MenuObjects[id].Ttip;
            else
                return "";
        }

        public Bitmap onGetImage(string id)
        {
            if (MenuObjects.ContainsKey(id))
            {
                if (string.IsNullOrEmpty(MenuObjects[id].Icon))
                    return null;
                else
                {
                    stdole.IPictureDisp p = Globals.ThisAddIn.Application.CommandBars.GetImageMso(MenuObjects[id].Icon, 64, 64);
                    return OfficeIconHelper.ConvertPixelByPixel(p);
                }
            }
            else
                return null;
        }

        public Bitmap SocialImage()
        {
            if ((configurationManager != null && configurationManager.Menus != null) && configurationManager.Menus.EnableChatterMenu)
                return Properties.Resources.chatterSelected.ToBitmap();

            return null;
        }

        public void BuildOfflineMenus(Ribbon ribbonxMLUI)
        {
            Menus runtimeMenu = configurationManager.Menus;
            if (runtimeMenu != null)
            {
                // Show Hide Runtime Ribbon buttons
                //ShowHideRuntimeRibbon(runtimeMenu);
                MenuObjects.Clear();
                ribbonxMLUI.RibbonUIXml.InvalidateControl("EditGroup");
            }
        }

        private void ShowHideRuntimeRibbon(Menus menu)
        {
            // TODO:: show hide on Ribbon callbacks now

            // Edit Group
            //RibbonGroup EditGroup = (from g in tab1.Groups
            //                         where g.Name.Contains("EditGroup")
            //                         select g).FirstOrDefault();

            //RibbonSplitButton AddRowButton = EditGroup.Items[0] as RibbonSplitButton;
            //AddRowButton.Visible = menu.EnableAddRowMenu;

            //RibbonButton DeleteRowButton = EditGroup.Items[1] as RibbonButton;
            //DeleteRowButton.Visible = menu.EnableDeleteRowMenu;

            //EditGroup.Visible = menu.EnableAddRowMenu | menu.EnableDeleteRowMenu;

            //// Social Group
            //RibbonGroup SocialGroup = (from g in tab1.Groups
            //                           where g.Name.Contains("SocialGroup")
            //                           select g).FirstOrDefault();

            //RibbonToggleButton ChatterButton = SocialGroup.Items[0] as RibbonToggleButton;
            //ChatterButton.Visible = menu.EnableChatterMenu;

            //SocialGroup.Visible = menu.EnableChatterMenu;
        }


    }

    public class AbstractMenuProperties
    {
        public string Label { get; set; }
        public bool Visible { get; set; }
        public string ID { get; set; }
        public bool Enable { get; set; }
        public object objTag { get; set; }
        public string Ttip { get; set; }
        public string Icon { get; set; }

        public void clear()
        {
            Label = "";
            Visible = false;
            ID = "";
            objTag = null;
            Enable = false;
        }
    }

}
