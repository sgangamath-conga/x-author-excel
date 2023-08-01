/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class UserInputLookupForm : Form
    {
        ObjectManager objectManager;
        SearchFilter Filter;
        ApttusObject lookupObject;
        public string LookupName { get; private set; }
        public string LookupId { get; private set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public UserInputLookupForm(SearchFilter filter)
        {
            InitializeComponent();
            SetCultureData();
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.RUNTIME_PRODUCT_NAME;
            Filter = filter;
            objectManager = ObjectManager.GetInstance;
        }
        private void SetCultureData()
        {
            btnGo.Text = resourceManager.GetResource("INPUTACTLOOKUP_btnGo_Text");
            IDColumn.HeaderText = resourceManager.GetResource("INPUTACTLOOKUP_IDColumn_HeaderText");
            lblTitle.Text = resourceManager.GetResource("INPUTACTLOOKUP_label1_Text");
            lblLookUp.Text = lblTitle.Text;
            ObjectName.HeaderText = resourceManager.GetResource("COMMON_Name");
            lblSearch.Text = resourceManager.GetResource("COREAPPBROWSEVIEW_btnSearch_Text");
        }
        private void UserInputLookupForm_Shown(object sender, EventArgs e)
        {
            ActiveControl = txtSearch;
            ApttusObject filterObj = ApplicationDefinitionManager.GetInstance.GetAppObject(Filter.AppObjectUniqueId);
            ApttusField lookupField = filterObj.Fields.FirstOrDefault(f => f.Id.Equals(Filter.FieldId));
            if (lookupField.MultiLookupObjects != null && lookupField.MultiLookupObjects.Count > 1)
            {
                foreach (ApttusObject item in lookupField.MultiLookupObjects)
                    cmbLookUp.Items.Add(item.Name);
                cmbLookUp.SelectedIndex = 0;
                lookupObject = lookupField.MultiLookupObjects.FirstOrDefault(f => f.Name == cmbLookUp.SelectedItem.ToString());
            }
            else
            {
                tpnlLookUp.Visible = false;
                lookupObject = filterObj.Fields.Where(field => field.Id.Equals(Filter.FieldId)).Select(field => field.LookupObject).FirstOrDefault();
                if (lookupObject != null)
                {
                    ApttusDataSet userInputDataSet = objectManager.GetDataSetByLookupSearch(lookupObject);
                    if (userInputDataSet != null && userInputDataSet.DataTable != null)
                    {
                        dataGridView1.Columns[0].DataPropertyName = lookupObject.NameAttribute;
                        dataGridView1.Columns[1].DataPropertyName = lookupObject.IdAttribute;

                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = userInputDataSet.DataTable;
                    }
                    #region Old SOQL Code
                    //string soql = @"select id, name from  RecentlyViewed 
                    //WHERE Type IN ('" + lookupObject.Id + "') ORDER BY LastViewedDate limit 20";
                    //ApttusDataSet ds = objectManager.QueryDataSet(new SalesforceQuery { SOQL = soql });
                    //if (ds != null && ds.DataTable != null)
                    //{
                    //    if (ds.DataTable.Rows.Count < 5)
                    //    {
                    //        soql = string.Format("select id, name from {0}", lookupObject.Id);
                    //        ds = objectManager.QueryDataSet(new SalesforceQuery { SOQL = soql });
                    //    }
                    //    dataGridView1.AutoGenerateColumns = false;
                    //    dataGridView1.DataSource = ds.DataTable;
                    //}
#endregion
                }
            }
        }

        private void Search()
        {
            ApttusObject filterObj = ApplicationDefinitionManager.GetInstance.GetAppObject(Filter.AppObjectUniqueId);
            ApttusField lookupField = filterObj.Fields.Where(field => field.Id.Equals(Filter.FieldId)).FirstOrDefault();

            if (lookupField.MultiLookupObjects != null && lookupField.MultiLookupObjects.Count > 0)
            {
                lookupObject = lookupField.MultiLookupObjects.FirstOrDefault(f => f.Name == cmbLookUp.SelectedItem.ToString());
                //Need to change DataPropertyName of grid on combo selection change
            }

            if (lookupObject == null)
                return;
            ApttusDataSet ds = objectManager.GetDataSetByLookupSearch(lookupObject, txtSearch.Text.Trim());

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds.DataTable;
            if (ds != null && ds.DataTable != null && ds.DataTable.Rows.Count >= 1)
            {
                dataGridView1.Columns[0].DataPropertyName = lookupObject.NameAttribute;
                dataGridView1.Columns[1].DataPropertyName = lookupObject.IdAttribute;
            }
            dataGridView1.Focus();

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ObjectName.Index)
            {
                LookupId = dataGridView1.Rows[e.RowIndex].Cells[IDColumn.Name].Value.ToString();
                LookupName = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cmbLookUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            Search();
        }
    }
}
