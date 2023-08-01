/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class MenuController
    {
        //  AbstractMenu mdlMenu;
        private IMenuView viewMenu;
        IModelMenu ModelMenu;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public MenuController(IMenuView view)
        {
            view.SetController(this);

            viewMenu = view;
            //  menu = mdlMenu;
            ModelMenu = new ModelMenu();
            if (ModelMenu.GetMenusCollection() != null)
            {
                MenuControlsMediator.SetTreeView(ModelMenu.GetMenusCollection());
            }

            MenuControlsMediator.SetMenuOptions(ModelMenu.GetMenus());
        }
        public MenuMediator MenuControlsMediator
        {
            get;
            set;
        }

        public void PropertyGridValChanged()
        {
            TreeNode node = MenuControlsMediator.PropertyGridValChanged();
            TreeNode parent = node.Parent;

            if (parent == null)
            {// update group
                MenuGroup grp = node.Tag as MenuGroup;
                if (String.IsNullOrEmpty(grp.Name))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("MENUCTL_PropertyChg_ErrorMsg"), resourceManager.GetResource("COMMON_Name_Text"));
                    return;
                }
                ModelMenu.UpdateMenu(node.Tag as MenuGroup);
            }
            else
            {// node is a child so update menu item and pass the parent.

                Apttus.XAuthor.Core.MenuItem item = node.Tag as Apttus.XAuthor.Core.MenuItem;
                if (String.IsNullOrEmpty(item.Name))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("MENUCTL_PropertyChg_ErrorMsg"), resourceManager.GetResource("COMMON_Name_Text"));
                    MenuControlsMediator.EnableSave(false);
                    return;
                }
                else
                {
                    MenuControlsMediator.EnableSave(true);
                }
                ModelMenu.UpdateMenuItem(parent.Tag as MenuGroup, node.Tag as Apttus.XAuthor.Core.MenuItem);
            }



        }
        public void MenuTypeChanged()
        {   // when the menu type changed from menu group / menu item
            // make the current item show as selected. 

            MenuControlsMediator.HandleMenutypeChange();


        }
        public void setWorkFlow(Workflow wf)
        {
            AbstractMenu item = MenuControlsMediator.GetCurrentItem();
            if (item != null)
            {

                ((Apttus.XAuthor.Core.MenuItem)MenuControlsMediator.GetCurrentItem()).WorkFlowID = wf.Id;
            }
        }
        public void ShowPermission()
        {

            UserProfileController ctrl = new UserProfileController();

            using (PermissionUI form = new PermissionUI(ctrl, MenuControlsMediator.GetCurrentItem().Profiles))
            {

                if (form.ShowDialog() == DialogResult.OK)
                {
                    MenuControlsMediator.GetCurrentItem().Profiles = form.Profiles;
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowId"></param>
        public void ShowWorkflow(string workflowId)
        {
            WorkflowDesigner wfDesignerView = new WorkflowDesigner(workflowId);
            WorkflowController wfcontroller = new WorkflowController();
            wfDesignerView.SetController(wfcontroller);

            wfcontroller.Initialize(null, wfDesignerView);
        }

        public void OnSelectNode()
        {
            MenuControlsMediator.OnSelectNode();
        }

        public void AddMenu(int index)
        {

            // UI had a combo with menu group and menu item and then changed to
            // add group / add menu item buttons. kept the same code and passed the index to differentiate
            // between add group and add buttons.

            if (index == 1) // menu
            {
                MenuGroup grp = ModelMenu.AddMenu();
                if (grp != null)
                    MenuControlsMediator.AddMenuGroup(grp);

            }
            else // menu item
            {
                MenuGroup grp = MenuControlsMediator.GetCurrentGroup();
                if (grp == null)
                {
                    grp = MenuControlsMediator.GetLastGroup();
                }
                if (grp != null)
                {
                    Guid ID;

                    Apttus.XAuthor.Core.MenuItem item = ModelMenu.AddMenuItem(grp);
                    if (MenuControlsMediator.Workflow.ValueMember != null)
                    {
                        Workflow wf = MenuControlsMediator.Workflow.SelectedItem as Workflow;
                        if (wf != null)
                        {
                            ID = wf.Id;
                            item.WorkFlowID = ID;
                        }
                    }

                    if (item != null)
                    {
                        MenuControlsMediator.AddMenuItem(item, grp, false);
                    }

                }
                else
                {//TODO display message to select group

                }
            }
        }
        public List<string> MenuComboDataset()
        {
            return new List<string>() { "Menu Group", "Menu Item" };

        }
        public List<Workflow> GetWF()
        {

            return ConfigurationManager.GetInstance.Workflows;

        }
        public AbstractMenu GetProperty(int selection)
        {
            if (selection < 2)
                return new Apttus.XAuthor.Core.MenuItem();
            else
                return new MenuGroup();
        }
        public void Save(bool AddRow, bool DeleteRow, bool Chatter, bool AddMatrixColumn)
        {
            if (MenuControlsMediator.DoesTreeHasNodes())
            {
                ModelMenu.SaveMenu(AddRow, DeleteRow, Chatter,AddMatrixColumn);
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("MENUCTL_Save_InfoMsg"), resourceManager.GetResource("COMMON_SaveMenu_Text"));
            }
        }

        public void DeleteItem()
        {
            //first delete from tree
            // delete from the MenuManager collection
            List<object> DeletedNodes;
            if (MenuControlsMediator.DeleteNode(out DeletedNodes))
            {
                if (DeletedNodes != null && DeletedNodes.Count > 0)
                {
                    foreach (object obj in DeletedNodes)
                    {
                        TreeNode node = obj as TreeNode;
                        if (node.Parent != null)
                        { // delete menu items
                            MenuGroup grp = (MenuGroup)node.Parent.Tag;
                            //delete menu item
                            ModelMenu.DeleteMenuItem(grp, node.Tag as Apttus.XAuthor.Core.MenuItem);

                        }
                        else // delete parent
                        {
                            ModelMenu.DeleteMenu((MenuGroup)node.Tag);

                        }
                        node.Remove();
                        MenuControlsMediator.setNextItem();
                    }
                    if (!MenuControlsMediator.DoesTreeHasNodes())
                    {
                        MenuControlsMediator.SetSaveFlag(false);
                        MenuControlsMediator.EnableDisableForGroup(false);
                        MenuControlsMediator.ResetPRoperties();
                    }

                }
                else
                {
                    //TOdo log / display error msg
                    //should never get here.
                }

            }

        }
    }
}
