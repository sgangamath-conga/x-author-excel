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
    public partial class DataMigrationExternalIDViewer : Form
    {
        MigrationObject mObject;
        ApttusObject appObject;
        DataMigrationModel model;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DataMigrationController controller;
        ObjectManager obm = ObjectManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public DataMigrationExternalIDViewer()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_Apply_Text");
            dcId.HeaderText = resourceManager.GetResource("COMMON_FieldId_Text");
            dcLookupName.HeaderText = resourceManager.GetResource("COMMON_FieldLabel_Text");
        }
        public DataMigrationExternalIDViewer(MigrationObject migrationObject, DataMigrationController controller, DataMigrationModel model)
            : this()
        {
            this.mObject = migrationObject;
            this.controller = controller;
            appObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            this.model = model;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mObject.ExternalID = dgvLookup.SelectedRows[0].Cells[0].Value.ToString();
            UpdateExternalID(mObject);
            this.Close();
        }

        private void DataMigrationLookupViewer_Load(object sender, EventArgs e)
        {
            this.lblHeader.Text = mObject.ObjectName.ToString() + " ExternalIDs";
            RenderExternalIDInformation();

        }

        private void RenderExternalIDInformation()
        {
            dgvLookup.AutoGenerateColumns = false;
            List<ApttusField> externalIDList = controller.GetExternalIDDetails(appObject);
            dgvLookup.DataSource = externalIDList;
            setSelectedData();
        }

        void setSelectedData()
        {
            if (string.IsNullOrEmpty(mObject.ExternalID))
            {
                dgvLookup.ClearSelection();
                return;
            }
            int i = 0;
            foreach (DataGridViewRow row in dgvLookup.Rows)
            {
                if (row.Cells[0].Value.ToString() == mObject.ExternalID)
                {
                    dgvLookup.Rows[i].Selected = true;
                }
                i++;
            }
        }

        private void dgvLookup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            mObject.ExternalID = dgvLookup.Rows[e.RowIndex].Cells[0].Value.ToString();
            UpdateExternalID(mObject);
            this.Close();
        }

        private void UpdateExternalID(MigrationObject mObj)
        {
            foreach (MigrationObject migrationObject in model.MigrationObjects.Where(x => x.AppObjectUniqueId == mObj.AppObjectUniqueId))
            {
                string externalid = mObj.ExternalID;

                migrationObject.ExternalID = externalid;

                //Remove all existing extenral id fields from migration object & apttusObject
                migrationObject.Fields.RemoveAll(x => applicationDefinitionManager.GetField(migrationObject.AppObjectUniqueId, x.FieldId).ExternalId == true);
                applicationDefinitionManager.RemoveField(appObject, (x => x.ExternalId == true));

                //add selected extenral id fields in apttusObject
                ApttusObject freshObj = obm.GetApttusObject(appObject.Id, false);
                applicationDefinitionManager.AddField(appObject, freshObj.Fields.Where(x => x.Id == externalid).FirstOrDefault());

                // Synchoronise migration object & update UI.
                List<ApttusFieldDS> selFields = controller.GetFieldsList(migrationObject, appObject.Fields).ToList();
                controller.UpdateFields(appObject, migrationObject, selFields);
            }
        }
    }

}
