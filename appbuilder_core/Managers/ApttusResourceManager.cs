using System;
using System.Resources;
using System.Globalization;
using System.Threading;
using System.IO;

namespace Apttus.XAuthor.Core
{
    public class ApttusResourceManager
    {
        private static ApttusResourceManager instance;
        public static ResourceManager resourceManager;
        private static object syncRoot = new Object();
        private static CultureInfo resourceCulture;
        private static string CRMPrefix;
        public string CRMName {
            get {
                string activeCRM = string.Empty;
                CRM TargetCRM = CRMContextManager.Instance.ActiveCRM;
                switch (TargetCRM)
                {
                    case CRM.Salesforce:
                        activeCRM = "Salesforce";
                        break;
                    case CRM.DynamicsCRM:
                        activeCRM = "Dynamics";
                        break;
                    case CRM.AIC:
                        activeCRM = "Apttus Omni";
                        break;
                    case CRM.AICV2:
                        activeCRM = "Apttus Omni V2";
                        break;
                    default:
                        activeCRM = "Salesforce";
                        break;
                }
                return activeCRM;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private ApttusResourceManager()
        {   
            string DirectoryPath = AssemblyDirectory;

            // Check if it's designer path then load designer resource file else runtime resource file.
            if (File.Exists(Path.Combine(DirectoryPath, Constants.DESIGNER_DLL_NAME)))
                resourceManager = ResourceManager.CreateFileBasedResourceManager("XAuthorDesignerResources", DirectoryPath, null);
            else if (File.Exists(Path.Combine(DirectoryPath, Constants.RUNTIME_DLL_NAME)))
                resourceManager = ResourceManager.CreateFileBasedResourceManager("XAuthorRuntimeResources", DirectoryPath, null);

            SetCultureInfo(Utils.GetLatestCulture.Name);
            SetTargetCRM();
        }

        private void SetTargetCRM()
        {
            CRM TargetCRM = CRMContextManager.Instance.ActiveCRM;
            switch (TargetCRM)
            {
                case CRM.Salesforce:
                    CRMPrefix = string.Empty;
                    break;
                case CRM.DynamicsCRM:
                    CRMPrefix = "MSDynamics_";
                    break;
                case CRM.AIC:
                case CRM.AICV2:
                    CRMPrefix = "AIC_";
                    break;
                default:
                    CRMPrefix = "";
                    break;
            }
        }

        /// <summary>
        /// Returns the cached ResourceManager instance used by inner classes.
        /// </summary>
        public static ApttusResourceManager GetInstance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    lock (syncRoot)
                    {
                        if (ReferenceEquals(resourceManager, null))
                        {
                            instance = new ApttusResourceManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Returns executing directory
        /// </summary>
        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        public void SetCultureInfo(string culture)
        {
            try
            {
                resourceCulture = new CultureInfo(culture);

                Thread.CurrentThread.CurrentCulture = resourceCulture;
                Thread.CurrentThread.CurrentUICulture = resourceCulture;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Returns resource value of given resource id 
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public string GetResource(string resourceID, bool CRMSpecific = false)
        {
            if (CRMSpecific) resourceID = CRMPrefix + resourceID;

            string resource = resourceManager.GetString(resourceID, resourceCulture);
            if (!string.IsNullOrEmpty(resource))
            {
                if (resource.Contains("\\n"))
                    resource = resource.Replace("\\n", "\n");
            }
            return (resource ?? string.Format(resourceManager.GetString("ERR_ResourceIdNotFound") + ": {0}", resourceID));
        }

         string GetResourceFileNameAsPerResourcePreference(string Language)
        {
            string resourcefile = string.Empty;

            string DirectoryPath = AssemblyDirectory;

            if (File.Exists(Path.Combine(DirectoryPath, Constants.DESIGNER_DLL_NAME)))
            {
                switch (Language)
                {
                    case Constants.RESOURCE_ENGLISH:
                        resourcefile = "XAuthorDesignerResources";
                        break;
                    case Constants.RESOURCE_JAPANESE:
                        resourcefile = "XAuthorDesignerResources.ja-JP";
                        break;
                    default:
                        resourcefile = "XAuthorDesignerResources";
                        break;
                }
            }

            else if (File.Exists(Path.Combine(DirectoryPath, Constants.RUNTIME_DLL_NAME)))
            {
                switch (Language)
                {
                    case Constants.RESOURCE_ENGLISH:
                        resourcefile = "XAuthorRuntimeResources";
                        break;
                    case Constants.RESOURCE_JAPANESE:
                        resourcefile = "XAuthorRuntimeResources.ja-JP";
                        break;
                    default:
                        resourcefile = "XAuthorRuntimeResources";
                        break;
                }
            }
            return resourcefile;
        }

        public  void UpdateResourceManager(string Language)
        {
            string DirectoryPath = AssemblyDirectory;
            resourceManager = ResourceManager.CreateFileBasedResourceManager(GetResourceFileNameAsPerResourcePreference(Language), DirectoryPath, null);
        }

    }
}
