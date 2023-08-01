using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataMigrationLookupViewer : Form
    {
        MigrationObject mObject;
        ApttusObject appObject;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DataMigrationController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        DataMigrationModel migrationModel;
        public DataMigrationLookupViewer()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        void processDataGrid()
        {
            foreach (DataGridViewRow dgvr in dgvLookup.Rows)
            {
                DataGridViewComboBoxCell cmb = (DataGridViewComboBoxCell)dgvr.Cells["LookupSource"];
                string LookupfieldID = dgvr.Cells["LookupFieldId"].Value.ToString();
                List<MigrationObject> data = controller.GetLookUpInputDetails(appObject, LookupfieldID);
                cmb.DataSource = data;
                cmb.DisplayMember = "ObjectName";
                cmb.ValueMember = "Id";

                Guid selid = mObject.Lookup.Where(x => x.LookupId == LookupfieldID).FirstOrDefault().WorkflowInputMigrationObjectId;

                if (selid != Guid.Empty)
                    cmb.Value = selid;
            }
        }

        private void SetCultureData()
        {
            btnSave.Text = resourceManager.GetResource("COMMON_Apply_Text"); 
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            LookupFieldId.HeaderText = resourceManager.GetResource("DM_LookupID_Text");
            LookupFieldName.HeaderText = resourceManager.GetResource("DM_LookupName_Text");
            LookupObjectName.HeaderText = resourceManager.GetResource("DM_lookupObjectName_Text");
            LookupSource.HeaderText = resourceManager.GetResource("DM_lookupSource_Text");
        }

        string checkDependencyExists(List<MigrationLookup> Lookup)
        {
            foreach (var lookup in Lookup)
            {
                var inputObj = migrationModel.MigrationObjects.Where(x => x.Id == lookup.WorkflowInputMigrationObjectId && x.Id != mObject.Id).FirstOrDefault();

                if (inputObj != null && inputObj.Lookup.Exists(x1 => x1.WorkflowInputMigrationObjectId == mObject.Id))
                    return string.Format("System does not support cyclical dependency.Hence remove it between lookup '{0}' from '{1}' and '{2}'.", lookup.LookupName, lookup.LookupObjectId, migrationModel.MigrationObjects.Find(x => x.Id == lookup.WorkflowInputMigrationObjectId).ObjectName);

            }
            return string.Empty;
        }

        public DataMigrationLookupViewer(MigrationObject migrationObject, DataMigrationController controller, DataMigrationModel model)
            : this()
        {
            this.mObject = migrationObject;
            this.controller = controller;
            appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            dgvLookup.AutoGenerateColumns = false;
            migrationModel = model;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        List<MigrationLookup> UpdateLookupSource()
        {
            List<MigrationLookup> lstLookup = new List<MigrationLookup>();
            foreach (DataGridViewRow dgvr in dgvLookup.Rows)
            {
                lstLookup.Add(new MigrationLookup()
                {
                    LookupId = dgvr.Cells["LookupFieldId"].Value.ToString(),
                    LookupName = dgvr.Cells["LookupFieldName"].Value.ToString(),
                    LookupObjectId = dgvr.Cells["LookupObjectName"].Value.ToString(),
                    WorkflowInputMigrationObjectId = (Guid)dgvr.Cells["LookupSource"].Value
                });
            }
            return lstLookup;
        }

        private void DataMigrationLookupViewer_Load(object sender, EventArgs e)
        {
            this.lblHeader.Text = mObject.ObjectName.ToString() + " Lookup";
            RenderLookupInformation();
            processDataGrid();
        }

        private void RenderLookupInformation()
        {
            dgvLookup.DataSource = mObject.Lookup;
        }

        private void dgvLookup_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }

        private void dgvLookup_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<MigrationLookup> lstLookup = UpdateLookupSource();

            string errormsg = checkDependencyExists(lstLookup);
            if (!string.IsNullOrEmpty(errormsg))
            {
                ApttusMessageUtil.ShowInfo(errormsg, Constants.DESIGNER_PRODUCT_NAME, this.Handle.ToInt32());
                DialogResult = DialogResult.None;
            }
            else
            {
                DialogResult = DialogResult.OK;
                mObject.Lookup = lstLookup;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
