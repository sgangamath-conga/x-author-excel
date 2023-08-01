using System;
using System.IO;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class ActiveCRMInfo
    {
        private CRM activeCRM;

        public string ApplicationConfigFileName { get; set; }

        public CRM ActiveCRM
        {
            get
            {
                return activeCRM;
            }
            set
            {
                activeCRM = value;
                updateApplicationConfigFileName(value);
            }
        }

        private void updateApplicationConfigFileName(CRM value)
        {
            switch (value)
            {
                case CRM.Salesforce:
                    ApplicationConfigFileName = Constants.PRODUCT_NAME;
                    break;
                case CRM.DynamicsCRM:
                    ApplicationConfigFileName = Constants.PRODUCT_NAME_DCRM;
                    break;
                case CRM.AIC:
                    ApplicationConfigFileName = Constants.PRODUCT_NAME_AIC;
                    break;
                case CRM.AICV2:
                    ApplicationConfigFileName = Constants.PRODUCT_NAME_AICV2;
                    break;
            }
        }
    }

    public class CRMContextManager
    {
        private static CRMContextManager _instance;
        private string ActiveCRMConfigFilePath;
        private ActiveCRMInfo crmInfo;

        private CRMContextManager()
        {
            try
            {
                ActiveCRMConfigFilePath = Utils.GetActiveCRMConfigFilePath();
                CRMChangedEvent.OnCRMChanged += CRMChangedEvent_OnCRMChanged;

                if (File.Exists(ActiveCRMConfigFilePath))
                {
                    crmInfo = ApttusXmlSerializerUtil.SerializeFromFile<ActiveCRMInfo>(ActiveCRMConfigFilePath);
                }
                else
                {
                    crmInfo = new ActiveCRMInfo();
                    crmInfo.ActiveCRM = CRM.Salesforce;
                }
            }
            catch(Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        void CRMChangedEvent_OnCRMChanged(ActiveCRMInfo activeCRMInfo)
        {
            crmInfo = activeCRMInfo;
        }

        public static CRMContextManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CRMContextManager();
                return _instance;
            }
        }
     
        public CRM ActiveCRM
        {
            get
            {
                return crmInfo.ActiveCRM;
            }
        }

        public string ApplicationConfigFile
        {
            get
            {
                return crmInfo.ApplicationConfigFileName;
            }
        }
    }
}
