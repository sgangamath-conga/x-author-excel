using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Apttus.XAuthor.Core
{


    public partial class CRMSwitcher : Form
    {
        private ActiveCRMInfo activeCRMInfo = null;
        private Dictionary<string, CRM> CRMTypes = new Dictionary<string, CRM>();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        private string ActiveCRMConfigFilePath;

        public CRMSwitcher()
        {
            InitializeComponent();
            this.Text = resourceManager.GetResource("RIBBON_ChangeCRM_Text");
            btnApply.Text = resourceManager.GetResource("COREMULTILINEVIEW_btnSave_Text");
            btnClose.Text = resourceManager.GetResource("COREMULTILINEVIEW_btnCancel_Text");
            lblActiveSource.Text = resourceManager.GetResource("RIBBON_SelectCRM_Text");
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            KeyValuePair<string, CRM> selectedCRM = (KeyValuePair<string, CRM>)cboCRM.SelectedItem;
            activeCRMInfo.ActiveCRM = selectedCRM.Value;
            Save();
        }

        private void CRMSwitcher_Load(object sender, EventArgs e)
        {
            ActiveCRMConfigFilePath = Utils.GetActiveCRMConfigFilePath();
            bool doCRMConfigFileExists = File.Exists(ActiveCRMConfigFilePath);
            if (!doCRMConfigFileExists)
            {
                activeCRMInfo = new ActiveCRMInfo
                {
                    ActiveCRM = CRM.Salesforce
                };

                Save(doCRMConfigFileExists);
            }

            AddCRM();

            cboCRM.DataSource = new BindingSource(CRMTypes, null);
            cboCRM.DisplayMember = "Key";
            cboCRM.ValueMember = "Value";
        }

        private void CRMSwitcher_Shown(object sender, EventArgs e)
        {
            activeCRMInfo = ApttusXmlSerializerUtil.SerializeFromFile<ActiveCRMInfo>(ActiveCRMConfigFilePath);
            cboCRM.SelectedValue = activeCRMInfo.ActiveCRM;
        }

        private void AddCRM()
        {
            CRMTypes["Salesforce"] = CRM.Salesforce;
            CRMTypes["Microsoft Dynamics 365"] = CRM.DynamicsCRM;
            CRMTypes["Apttus Omni"] = CRM.AIC;
            CRMTypes["Apttus Omni V2"] = CRM.AICV2;
        }

        private void Save(bool doCRMConfigFileExists = true)
        {
            ApttusXmlSerializerUtil.SerializeToFile<ActiveCRMInfo>(activeCRMInfo, ActiveCRMConfigFilePath);
            CRMChangedEvent.FireCRMChangedHandler(activeCRMInfo);
            if (doCRMConfigFileExists)
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CHANGECRM_ApplyMessage_Text"), resourceManager.GetResource("RIBBON_ChangeCRM_Text"));
        }
    }
    public static class CRMChangedEvent
    {
        public delegate void CRMChangedHandler(ActiveCRMInfo activeCRMInfo);
        public static event CRMChangedHandler OnCRMChanged;

        public static void FireCRMChangedHandler(ActiveCRMInfo activeCRMInfo)
        {
            if (OnCRMChanged != null)
                OnCRMChanged(activeCRMInfo);
        }
    }
}
