using Apttus.XAuthor.Core;
using Autofac;
using System;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ThisAddIn
    {
        private class CRMFactory
        {
            private static CRMFactory _instance;
            private CRMContextManager crmManager = CRMContextManager.Instance;
            private IContainer crmContainer;
            private IXAuthorRibbonLogin xauthorLogin;
            private BaseApplicationController appController;
            private ContainerBuilder builder;

            private CRMFactory()
            {
                RegisterCRM();
            }

            public static CRMFactory Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new CRMFactory();
                    return _instance;
                }
            }

            private void RegisterCRM()
            {
                try
                {
                    builder = new ContainerBuilder();

                    switch (crmManager.ActiveCRM)
                    {
                        case CRM.Salesforce:
                            ObjectManager.SetCRM(CRM.Salesforce);
                            RegisterSalesforceTypes();
                            break;
                        case CRM.DynamicsCRM:
                            ObjectManager.SetCRM(CRM.DynamicsCRM);
                            RegisterDynamicsTypes();
                            break;
                        case CRM.AIC:
                            ObjectManager.SetCRM(CRM.AIC);
                            RegisterAICTypes();
                            break;
                        case CRM.AICV2:
                            ObjectManager.SetCRM(CRM.AICV2);
                            RegisterAICV2Types();
                            break;
                    }

                    crmContainer = builder.Build();
                }
                catch (Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }
            }

            private void RegisterAICV2Types()
            {
                builder.RegisterType<XAuthorAICV2Login>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<AICApplicationController>().As<BaseApplicationController>();
            }

            private void RegisterAICTypes()
            {
                builder.RegisterType<XAuthorAICLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<AICApplicationController>().As<BaseApplicationController>();
            }

            private void RegisterDynamicsTypes()
            {
                builder.RegisterType<XAuthorDynamicsLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<DynamicsApplicationController>().As<BaseApplicationController>();
            }

            private void RegisterSalesforceTypes()
            {
                builder.RegisterType<XAuthorSalesforceLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<SalesforceApplicationController>().As<BaseApplicationController>();
            }

            public IXAuthorRibbonLogin GetLoginObject()
            {
                if (xauthorLogin == null)
                    xauthorLogin = crmContainer.Resolve<IXAuthorRibbonLogin>();
                return xauthorLogin;
            }

            public BaseApplicationController GetApplicationController()
            {
                if (appController == null)
                    appController = crmContainer.Resolve<BaseApplicationController>();
                return appController;
            }
        }
    }
}
