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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class MultiSelectListForm : Form
    {
        ObjectManager objectManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public BindingSource SourceListSource = new BindingSource();
        public BindingSource SelectedListSource = new BindingSource();

        string ScreenTitle;
        public MultiSelectListForm(List<ApttusObject> sourcevalues, List<ApttusObject> selectedvalues, string title)
        {
            InitializeComponent();
            ScreenTitle = title;
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            objectManager = ObjectManager.GetInstance;

            SourceListSource.DataSource = sourcevalues.OrderBy(x => x.Name).ToList();
            SelectedListSource.DataSource = selectedvalues.OrderBy(x => x.Name).ToList();

            PrepareUI();
        }

        void PrepareUI()
        {
            lstSource.SelectionMode = SelectionMode.MultiExtended;
            UpdatelstSourceDataSource();

            lstSource.SelectedIndex = -1;

            lstSelected.SelectionMode = SelectionMode.MultiExtended;
            UpdatelstSelectedDataSource();

            lstSelected.SelectedIndex = -1;

        }
        private void SetCultureData()
        {
            btnGo.Text = resourceManager.GetResource("DMMultiSelect_btnSearch_Text");
            btnClear.Text = resourceManager.GetResource("COMMON_Clear_Text");
            btnSelect.Text = resourceManager.GetResource("DMMultiSelect_btnSelect_Text");
            btnDeselect.Text = resourceManager.GetResource("DMMultiSelect_btnDeSelect_Text");
            btnSave.Text = resourceManager.GetResource("COREADDCUSTROWVIEW_btnAdd_Text");
            btnCancel.Text = resourceManager.GetResource("CORECOMMON_btnCancel_Text");
            lblTitle.Text = ScreenTitle;
            grpAvailableValues.Text = resourceManager.GetResource("DMMultiSelect_grpAvailableObjects_Text");
            grpSelectedValues.Text = resourceManager.GetResource("DMMultiSelect_grpObjectsToAdd_Text");
        }

        private void Search()
        {
            BindingSource tempSource = new BindingSource();
            tempSource.DataSource = SourceListSource.DataSource;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                SourceListSource.DataSource = ((IList<ApttusObject>)SourceListSource.List).Where(x => x.Name.ToLower().Contains(txtSearch.Text.ToLower())).OrderBy(x => x.Name).ToList();
            }
            else
            {
                SourceListSource.DataSource = ((IList<ApttusObject>)SourceListSource.List).OrderBy(x => x.Name).ToList();
            }

            UpdatelstSourceDataSource();

            SourceListSource = tempSource;
        }

        void UpdatelstSourceDataSource()
        {
            lstSource.DataSource = null;
            lstSource.DataSource = SourceListSource;
            lstSource.DisplayMember = "Name";
            lstSource.ValueMember = "Id";
            SourceListSource.Sort = "Name";
        }

        void UpdatelstSelectedDataSource()
        {
            lstSelected.DataSource = null;
            lstSelected.DataSource = SelectedListSource;
            lstSelected.DisplayMember = "Name";
            lstSelected.ValueMember = "Id";
            SelectedListSource.Sort = "Name";
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
                    SelectedListSource.List.Add(item);
                    SourceListSource.List.Remove(item);
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
                    SourceListSource.List.Add(item);
                    SelectedListSource.List.Remove(item);
                }
                updateSourceList();
            }
        }

        void updateSourceList()
        {
            Search();
            UpdatelstSelectedDataSource();
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

        private void lstSource_DoubleClick(object sender, EventArgs e)
        {
            btnSelect_Click(sender, e);
        }

        private void lstSelected_DoubleClick(object sender, EventArgs e)
        {
            btnDeselect_Click(sender, e);
        }

    }
}
