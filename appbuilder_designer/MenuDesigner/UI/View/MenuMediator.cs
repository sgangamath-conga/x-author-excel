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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Apttus.XAuthor.AppDesigner
{
    public class MenuMediator
    {
        private TreeView CurrentView;
        private PropertyGrid PropGrid;
        private ComboBox MenuSelector;
        private Button CmdCreate;

        private Button CmdDelete;
        private Button CmdSave;
        private Button cmdAttachWF;
        private Label grpWFlow;
        private Button CmdPermission;
        private Button cmdAvailability;
        private ComboBox cmboWorkFlow;
        private Button cmdButtonAddItem;
        public CheckBox chkAddRow;
        public CheckBox chkDeleteRow;
        public CheckBox chkChatter;
        public CheckBox chkMatrixAddRow;
        public CheckBox chkMatrixAddColumn;


        public MenuMediator(TreeView Tvw, PropertyGrid grid, ComboBox MnuSelector, Button create,
            Button delete, Button Save, Button WF, Label grpWF, Button permission, Button availability, ComboBox cmboWf,
             Button btnAdditem, CheckBox addRow, CheckBox deleteRow, CheckBox chatter, CheckBox MatrixAddColumn)
        {
            //cmdButtonAddItem =btnAdditem;
            CurrentView = Tvw;
            PropGrid = grid;
            MenuSelector = MnuSelector;
            CmdCreate = btnAdditem;
            CmdDelete = delete;
            grpWFlow = grpWF;
            CmdPermission = permission;
            cmdAvailability = availability;
            CmdSave = Save;

            cmdAttachWF = WF;
            DisableEnableControls(false);
            cmboWorkFlow = cmboWf;
            chkAddRow = addRow;
            chkDeleteRow = deleteRow;
            chkChatter = chatter;            
            chkMatrixAddColumn = MatrixAddColumn;

        }

        public ComboBox Workflow
        {
            get { return cmboWorkFlow; }
        }

        public void DiableEnableForMenuItem(bool flg)
        {
            CmdSave.Enabled = flg;
            CmdDelete.Enabled = flg;
            CmdPermission.Enabled = flg;
            cmdAvailability.Enabled = flg;
            cmdAttachWF.Enabled = flg;
        }

        public void DisableEnableControls(bool flg)
        {
            CmdDelete.Enabled = flg;
            CmdSave.Enabled = flg;
            cmdAttachWF.Enabled = flg;
            grpWFlow.Enabled = flg;
            if (cmboWorkFlow != null)
                cmboWorkFlow.Enabled = flg;
            CmdPermission.Enabled = flg;
            cmdAvailability.Enabled = flg;
        }

        public void EnableDisableForGroup(bool flg)
        {
            grpWFlow.Enabled = flg;
            if (cmboWorkFlow != null)
                cmboWorkFlow.Enabled = flg;
            CmdPermission.Enabled = flg;
            cmdAvailability.Enabled = flg;
        }

        public bool DoesTreeHasNodes()
        {
            return CurrentView.Nodes.Count > 0 ? true : false;
        }

        public void HandleMenutypeChange()
        {
            TreeNode node = CurrentView.SelectedNode;
            if (MenuSelector.SelectedIndex == 1 && CurrentView.GetNodeCount(true) == 0)
            {// user selected menu item from the combo box and there is no menu group added. this is invalid. 
                CmdCreate.Enabled = false;
                cmdAttachWF.Enabled = true;
            }
            else
            {
                EnableControl();
                //setNextItem();
            }
            SetItem(node);
        }

        public void EnableControl()
        {
            // take care of enable /disable controls
            if (!CmdCreate.Enabled)
            {
                CmdCreate.Enabled = true;
            }
        }

        public void EnableSave(bool flg)
        {
            CmdSave.Enabled = flg;
        }

        public void AddMenuItem(Apttus.XAuthor.Core.MenuItem item, MenuGroup grp, bool menuLoad)
        {
            TreeNode node = new TreeNode(item.Name);
            node.Tag = item;
            TreeNode selectedNode = null;
            if (menuLoad)
            {// when menu items are added through the add command, parent is already selected
                // but when the menus are loaded from xml, we need to use the parent to find the right 
                //group

                TreeNode[] nodes = CurrentView.Nodes.Find(grp.Name, false);
                if (nodes.Length > 0)
                {
                    TreeNode parent = nodes[0];
                    selectedNode = parent;
                    CurrentView.SelectedNode = parent;
                }

            }
            else

                selectedNode = CurrentView.SelectedNode;
            if (IsCurrentNodeGroup(selectedNode))
            {
                CurrentView.SelectedNode.Nodes.Add(node);
                CurrentView.SelectedNode = node;
                PropGrid.SelectedObject = item;
                CurrentView.Select();
                CurrentView.ExpandAll();
                DiableEnableForMenuItem(true);
            }
            else { }
        }

        public void AddMenuGroup(MenuGroup menu)
        {

            TreeNode treeNode = new TreeNode(menu.Name);
            treeNode.Name = menu.Name;

            treeNode.Tag = menu;
            CurrentView.Nodes.Add(treeNode);

            MenuGroup gr = PropGrid.SelectedObject as MenuGroup;
            PropGrid.SelectedObject = menu;
            CurrentView.SelectedNode = treeNode;
            CurrentView.Select();
            CurrentView.ExpandAll();
            //SetSaveFlag(true);
            EnableDisableForGroup(false);
            //
        }

        public void AddPermissions(List<ApttusUserProfile> Permissions)
        {
            ((AbstractMenu)PropGrid.SelectedObject).Profiles = Permissions;
        }

        public void SetSaveFlag(bool flg)
        {
            CmdSave.Enabled = flg;
            CmdDelete.Enabled = flg;
        }

        public AbstractMenu GetCurrentItem()
        {
            return PropGrid.SelectedObject as AbstractMenu;
        }

        private bool IsCurrentNodeGroup(TreeNode node)
        {
            if (node != null)
            {
                object menutype = node.Tag.GetType();


                if ((node.Tag.GetType()) == typeof(MenuGroup))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSelectNode()
        {
            TreeNode selectedNode = CurrentView.SelectedNode;
            if (selectedNode != null)
            {
                CmdDelete.Enabled = true;
                PropGrid.SelectedObject = selectedNode.Tag;
                if (selectedNode.Tag.GetType() == typeof(Apttus.XAuthor.Core.MenuItem))
                {
                    Apttus.XAuthor.Core.MenuItem mnuitem = selectedNode.Tag as Apttus.XAuthor.Core.MenuItem;
                    if (mnuitem.WorkFlowID != null)
                    {
                        List<Core.Workflow> wfs = cmboWorkFlow.DataSource as List<Core.Workflow>;
                        Core.Workflow wf = wfs.Find(Item => Item.Id.Equals(mnuitem.WorkFlowID));
                        if (wf != null)
                            cmboWorkFlow.SelectedItem = wf;
                    }
                }

                if (selectedNode.Parent != null)
                {
                    CmdCreate.Enabled = false;
                    EnableDisableForGroup(true);
                }
                else
                {
                    CmdCreate.Enabled = true;
                    EnableDisableForGroup(false);
                }
            }
            else
                CmdDelete.Enabled = false;
        }

        public void SetItem(TreeNode node)
        {
            if (node != null && CurrentView.Nodes.Count != 0)
            {
                CurrentView.SelectedNode = node;
                CurrentView.Select();
            }
        }

        public void setNextItem()
        {
            if (CurrentView.Nodes.Count != 0)
            {

                CurrentView.SelectedNode = CurrentView.Nodes[CurrentView.Nodes.Count - 1];
                if (CurrentView.SelectedNode.Nodes.Count > 0)
                    CurrentView.SelectedNode = CurrentView.SelectedNode.LastNode;
                CurrentView.Select();
            }
        }

        public void ResetPRoperties()
        {
            if (CurrentView.Nodes.Count == 0)
                PropGrid.SelectedObject = null;
        }

        public MenuGroup GetCurrentGroup()
        {
            TreeNode selectedNode = CurrentView.SelectedNode;
            if (IsCurrentNodeGroup(selectedNode))
            {
                return selectedNode.Tag as MenuGroup;
            }
            else
            {

            }
            return null;
        }

        public MenuGroup GetLastGroup()
        {
            TreeNodeCollection nodes = CurrentView.Nodes;
            int count = CurrentView.Nodes.Count;
            if (count == 0) return null;
            if (CurrentView.Nodes[count - 1].Tag == typeof(MenuGroup))
                return CurrentView.Nodes[count - 1].Tag as MenuGroup;
            else
                return null;

        }

        public void CmboMenuType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public TreeNode PropertyGridValChanged()
        {
            TreeNode selectedNode = CurrentView.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Text = ((AbstractMenu)PropGrid.SelectedObject).Name;
            }
            return selectedNode;
        }

        public bool DeleteNode(out List<object> DeletedNodes)
        {
            //Limitation ::Multi select is not allowed. delete one node at a time..
            TreeNode selectedNode = CurrentView.SelectedNode;

            DeletedNodes = null;
            TreeNode Parent = null;

            if (selectedNode != null)
            {
                DeletedNodes = new List<object>();
                DeletedNodes.Add(selectedNode);

                if (selectedNode.Parent == null)
                {

                    if (selectedNode.GetNodeCount(true) > 0)
                    {
                        //promptforDeleteMenuGroup
                        //DeleteUICaption
                        TaskDialogResult result = ApttusMessageUtil.ShowWarning(selectedNode.Text + " " + ApttusResourceManager.GetInstance.GetResource("MNUHOME_promptforDeleteMenuGroup"),
                                                                                    ApttusResourceManager.GetInstance.GetResource("COMMON_Delete_Text"),
                                                                                    ApttusMessageUtil.YesNo);
                        if (result == TaskDialogResult.No)
                        {
                            SetItem(selectedNode);
                            DeletedNodes.Remove(selectedNode); // remove from the bucket. 
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public AbstractMenu ActiveMenu()
        {
            TreeNode selectedNode = CurrentView.SelectedNode;
            if (selectedNode != null)
            {
                return selectedNode.Tag as AbstractMenu;
            }
            return null;
        }

        public void SetTreeView(List<MenuGroup> grp)
        {
            List<Apttus.XAuthor.Core.MenuItem> menuItems;
            foreach (MenuGroup mnugrp in grp)
            {
                AddMenuGroup(mnugrp);
                menuItems = mnugrp.MenuItems;
                foreach (Apttus.XAuthor.Core.MenuItem mnuitem in menuItems)
                {
                    AddMenuItem(mnuitem, mnugrp, true);
                }
            }
        }

        public void SetMenuOptions(Menus menus)
        {
            chkAddRow.Checked = menus.EnableAddRowMenu;
            chkDeleteRow.Checked = menus.EnableDeleteRowMenu;
            chkChatter.Checked = menus.EnableChatterMenu;            
            chkMatrixAddColumn.Checked = menus.EnableAddMatrixColumnMenu;
        }
    }
}
