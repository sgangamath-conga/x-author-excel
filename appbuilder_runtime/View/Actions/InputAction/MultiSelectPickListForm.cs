/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class MultiSelectPickListForm : Form
    {
        ObjectManager objectManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public List<string> SourceList;
        public List<string> SelectedList;
        string ScreenTitle;
        public MultiSelectPickListForm(List<string> sourcevalues, List<string> selectedvalues, string title)
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
            objectManager = ObjectManager.GetInstance;
            SourceList = sourcevalues;
            SelectedList = selectedvalues;
            ScreenTitle = title;
            PrepareUI();
            SetCultureData();
        }

        void PrepareUI()
        {
            lstSource.SelectionMode = SelectionMode.MultiExtended;
            lstSource.DataSource = SourceList;
            lstSource.SelectedIndex = -1;

            lstSelected.SelectionMode = SelectionMode.MultiExtended;
            lstSelected.DataSource = SelectedList;
            lstSelected.SelectedIndex = -1;

        }
        private void SetCultureData()
        {
            btnGo.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnGo_Text");
            btnClear.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnClear_Text");
            btnSelect.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnSelect_Text");
            btnDeselect.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnDeselect_Text");
            btnSave.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnSave_Text");
            btnCancel.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnCancel_Text");
            lblTitle.Text = string.Format(resourceManager.GetResource("MULTISELECTPICKLIST_lblTitle_Text"), ScreenTitle);
            grpAvailableValues.Text = resourceManager.GetResource("MULTISELECTPICKLIST_lstSource_Text");
            grpSelectedValues.Text = resourceManager.GetResource("MULTISELECTPICKLIST_lstDestination_Text");

        }

        private void Search()
        {
            lstSource.DataSource = null;
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                lstSource.DataSource = SourceList.Where(x => x.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            }
            else
            {
                lstSource.DataSource = SourceList;
            }

        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                Search();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lstSource.SelectedIndex >= 0)
            {
                foreach (var item in lstSource.SelectedItems)
                {
                    SelectedList.Add(item.ToString());
                    SourceList.Remove(item.ToString());
                }
                updateSourceList();
            }
        }

        private void btnDeselect_Click(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndex >= 0)
            {
                foreach (var item in lstSelected.SelectedItems)
                {
                    SourceList.Add(item.ToString());
                    SelectedList.Remove(item.ToString());
                }
                updateSourceList();
            }
        }

        void updateSourceList()
        {
            Search();
            lstSelected.DataSource = null;
            lstSelected.DataSource = SelectedList;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }

    }
}
