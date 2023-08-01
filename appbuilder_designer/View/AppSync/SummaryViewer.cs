using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class SummaryViewer : UserControl
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        DesignerAppSyncController appValidator;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public event EventHandler actionClicked;
        public ControlActions currentAction;
        public enum ControlActions
        {
            NONE,
            MISSINGOBJECTS,
            MISSINGFIELDS,
            DATATYPEMISMATCH,
            RECORDTYPES
        }
        public SummaryViewer(DesignerAppSyncController appValidator)
        {
            InitializeComponent();
            SetCultureData();
            this.appValidator = appValidator;
        }

        private void SetCultureData()
        {
            lblSummary.Text = resourceManager.GetResource("AppSync_lblSummary_Text");
            lblSummaryDescription.Text = string.Format(resourceManager.GetResource("AppSync_Summary_lblSummaryDescription_Text"),resourceManager.CRMName);
            lblObjects.Text = resourceManager.GetResource("UCRETRIEVEMAP_lblGridName_Text");
            lblPicklists.Text = resourceManager.GetResource("AppSync_Picklist_Text") + " : ";
            lblFieldDataTypes.Text = resourceManager.GetResource("AppSync_lnkFieldsMismatch_Text") + " : ";
            lblFields.Text = resourceManager.GetResource("COMMON_Fields_Text") + " : ";
            lblRecordTypes.Text = resourceManager.GetResource("AppSync_lblRecordTypes_Text") + " : "; 

        }
        internal void RefreshData()
        {
            UpdateObjects();
            UpdateMissingFields();
            UpdatedataTypeMismatch();
            UpdatePicklist();
            //Update/Show recrod type info only incase of salesforce CRM.
            if (appValidator.ActiveCRM.Equals(CRM.Salesforce))
            {
                lblRecordTypes.Visible = true;
                UpdateRecordTypes();
            }
        }

        void UpdateObjects()
        {
            int missingObjectCount = appValidator.GetMissingObjects().Count;
            string text = string.Format("{0} {1}", missingObjectCount, resourceManager.GetResource("AppSync_Summary_Missing_Text"));
            AddObject(flpObjects, text, missingObjectCount > 0, ControlActions.MISSINGOBJECTS);
        }

        void UpdateMissingFields()
        {
            int missingFieldCount = appValidator.GetMissingFieldInfo().Count;
            string text = string.Format("{0} {1}", missingFieldCount, resourceManager.GetResource("AppSync_Summary_Missing_Text"));
            AddObject(flpFields, text, missingFieldCount > 0, ControlActions.MISSINGFIELDS);
        }

        void UpdatedataTypeMismatch()
        {
            int datatypeMismatchFieldCount = appValidator.GetDataTypeMismatchFieldInfo().Count;
            string text = string.Format("{0} {1}", datatypeMismatchFieldCount, resourceManager.GetResource("AppSync_Summary_Mismatch_Text"));
            AddObject(flpFieldDataTypes, text, datatypeMismatchFieldCount > 0, ControlActions.DATATYPEMISMATCH);
        }

        void UpdatePicklist()
        {
            int picklistCount = appValidator.GetPickListMismatchInfo();
            string text = string.Format("{0} {1}", picklistCount, resourceManager.GetResource("AppSync_Summary_Synced_Text"));
            AddObject(flpPicklists, text, false, ControlActions.NONE);
        }

        void UpdateRecordTypes()
        {
            RecordTypeSyncInfo recordTypeSyncInfo = appValidator.GetRecordTypeSyncInfo();
            string text = string.Format(resourceManager.GetResource("AppSync_UpdateRecordTypes_Text"), recordTypeSyncInfo.Added, recordTypeSyncInfo.Updated, recordTypeSyncInfo.Deleted);
            AddObject(flpRecordType, text, false, ControlActions.RECORDTYPES);
        }
        void AddObject(Control ctrl, string text, bool isLinkLabel, ControlActions action)
        {
            Control lbl;

            if (isLinkLabel)
            {
                lbl = new LinkLabel();
                lbl.Tag = action;
                lbl.Click += Lbl_Click;
            }
            else
                lbl = new Label();

            lbl.Text = text;
            lbl.AutoSize = true;
            lbl.Margin = new Padding(0, 5, 5, 2);

            if (ctrl.Controls.Count > 1)
                ctrl.Controls.RemoveAt(1);

            ctrl.Controls.Add(lbl);
        }

        private void Lbl_Click(object sender, EventArgs e)
        {
            if (this.actionClicked != null)
            {
                currentAction = (ControlActions)((sender as LinkLabel).Tag);
                this.actionClicked(this, new EventArgs());
            }
        }
    }
}
