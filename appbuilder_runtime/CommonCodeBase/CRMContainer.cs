using System;
using Apttus.XAuthor.Core;
using Autofac;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class ThisAddIn
    {
        private class CRMFactory
        {
            private static CRMFactory _instance;
            private CRMContextManager crmManager = CRMContextManager.Instance;
            private Autofac.IContainer crmContainer;
            private IXAuthorRibbonLogin xauthorLogin;
            private BaseApplicationController applicationController;
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
                }
                catch(System.Exception ex)
                {
                    ExceptionLogHelper.ErrorLog(ex);
                }
            }

            private void RegisterAICV2Types()
            {
                var builder = new ContainerBuilder();
                // TODO:: Add others
                builder.RegisterType<AICApplicationController>().As<BaseApplicationController>();
                builder.RegisterType<XAuthorAICV2Login>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<SearchAndSelectAICAction>().As<ISearchAndSelectAction>();
                builder.RegisterType<AICSaveHelper>().As<ISaveHelper>();
                builder.RegisterType<AICQueryAction>().As<IQueryAction>();
                crmContainer = builder.Build();
            }

            private void RegisterDynamicsTypes()
            {
                var builder = new ContainerBuilder();
                builder.RegisterType<XAuthorDynamicsLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<DynamicsApplicationController>().As<BaseApplicationController>();
                builder.RegisterType<DynamicsCheckInProvider>().As<ICheckInProvider>();
                builder.RegisterType<DynamicsCheckOutProvider>().As<ICheckOutProvider>();
                builder.RegisterType<SearchAndSelectDynamicAction>().As<ISearchAndSelectAction>();
                builder.RegisterType<DynamicsQueryAction>().As<IQueryAction>();
                builder.RegisterType<DynamicsSaveHelper>().As<ISaveHelper>();
                crmContainer = builder.Build();
            }

            private void RegisterSalesforceTypes()
            {
                var builder = new ContainerBuilder();
                builder.RegisterType<XAuthorSalesforceLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<SalesforceApplicationController>().As<BaseApplicationController>();
                builder.RegisterType<SalesforceCheckInProvider>().As<ICheckInProvider>();
                builder.RegisterType<SalesforceCheckOutProvider>().As<ICheckOutProvider>();
                builder.RegisterType<SearchAndSelectSalesforceAction>().As<ISearchAndSelectAction>();
                builder.RegisterType<SalesforceQueryAction>().As<IQueryAction>();
                builder.RegisterType<SalesforceSaveHelper>().As<ISaveHelper>();
                crmContainer = builder.Build();
            }

            private void RegisterAICTypes()
            {
                var builder = new ContainerBuilder();
                // TODO:: Add others
                builder.RegisterType<AICApplicationController>().As<BaseApplicationController>();
                builder.RegisterType<XAuthorAICLogin>().As<IXAuthorRibbonLogin>();
                builder.RegisterType<SearchAndSelectAICAction>().As<ISearchAndSelectAction>();
                builder.RegisterType<AICSaveHelper>().As<ISaveHelper>();
                builder.RegisterType<AICQueryAction>().As<IQueryAction>();
                crmContainer = builder.Build();
            }

            public IXAuthorRibbonLogin GetLoginObject()
            {
                if (xauthorLogin == null)
                {
                    crmContainer.BeginLifetimeScope();
                    xauthorLogin = crmContainer.Resolve<IXAuthorRibbonLogin>();
                }
                return xauthorLogin;
            }

            public BaseApplicationController GetApplicationController()
            {
                if (applicationController == null)
                    applicationController = crmContainer.Resolve<BaseApplicationController>();
                return applicationController;
            }

            public ICheckInProvider GetCheckInProvider() 
            {
                return crmContainer.Resolve<ICheckInProvider>();
            }

            public ICheckOutProvider GetCheckOutProvider()
            {
                return crmContainer.Resolve<ICheckOutProvider>();
            }

            public ISearchAndSelectAction GetSearchAndSelectActionController()
            {
                return crmContainer.Resolve<ISearchAndSelectAction>();
            }

            public IQueryAction GetQueryActionProvider(QueryActionModel model, string[] inputDataName)
            {
                return crmContainer.Resolve<IQueryAction>(new NamedParameter("Model",model), new NamedParameter("InputDataName", inputDataName));
            }
            public ISaveHelper GetSaveHelper()
            {
                return crmContainer.Resolve<ISaveHelper>();
            }
        }
    }
}