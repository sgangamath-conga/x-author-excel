using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Apttus.XAuthor.Core
{
    public class LicenseActivationManager
    {
        private static LicenseActivationManager instance;
        private static object syncRoot = new Object();
        public XmlDocument lmaDoc = null;

        private XmlNode returnNode = null;
        //public XAuthorForExcel licenceMetadata = new XAuthorForExcel();
        IAdapter adapter;
        private static string EditionNode = "//Edition__c";
        private static string FeatureDataModifiedNode = "//PackageFeatureDataModifiedDate";
        private static string SyncDateNode = "//SyncDate__c";
        // This flag is use for avoid repeating call to check sandbox
        /// <summary>
        /// 
        /// </summary>
        private LicenseActivationManager()
        {
            lmaDoc = new XmlDocument();
            adapter = ObjectManager.GetInstance.GetAdapter();
            //licenceMetadata = new XAuthorForExcel();
        }

        /// <summary>
        /// 
        /// </summary>
        public static LicenseActivationManager GetInstance {
            get {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LicenseActivationManager();
                    }
                }

                return instance;
            }
        }

        #region "traverse XML nodes approach"

        /// <summary>
        /// This can be very flexible approach to enable / disable feature without being dependent on class structure.
        /// </summary>
        /// <param name="featureName"></param>
        /// <returns></returns>
        public bool IsFeatureAvailable(string featureName)
        {       
            ////ApttusLicenseMetadata licenceMetadata = new ApttusLicenseMetadata();
            if (lmaDoc == null || lmaDoc.DocumentElement == null || string.IsNullOrEmpty(featureName))
                return false;

            bool returnValue = false;

            // The base XML node in the document.
            //XmlNode job = lmaDoc.DocumentElement;

            // get Base Node for current edition
            XmlNode baseFeatureNode = lmaDoc.SelectSingleNode("//Package[@Edition='" + Constants.PRODUCT_EDITION + "']");

            if (baseFeatureNode == null)
                return false;

            // get Edition Node
            foreach (XmlNode node in baseFeatureNode.ChildNodes)
            {
                findAllNodes(node, featureName);
                if (returnNode != null)
                {
                    string nodeText = returnNode.InnerText;
                    // if major feature read attribute value, else node value
                    if (returnNode.Attributes.Count == 1)
                        nodeText = returnNode.Attributes["IsAvailable"].InnerText;

                    Boolean.TryParse(nodeText, out returnValue);
                    break;
                }
            }

            // reset returnNode to null for next call
            returnNode = null;
            return returnValue;
        }

        private void findAllNodes(XmlNode node, string value)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                if (!string.IsNullOrEmpty(n.Name) && n.Name.Equals(value))
                {
                    returnNode = n;
                    break;
                }
                findAllNodes(n, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckEdition(out bool doFeatureUpdate)
        {
            doFeatureUpdate = false;
            // Get license detail from CRM
            string licenseXML = adapter.GetLicenseDetail();
            if (!string.IsNullOrEmpty(licenseXML))
            {
                string licenseXMLClient = string.Empty;

                // Get license detail XML from install folder
                string installDirectory = adapter.GetLicenseFilePath() + Constants.LMA_LICENSE_DETAIL;
                if (File.Exists(installDirectory))
                {
                    string localLicenseXml = File.ReadAllText(installDirectory);
                    licenseXMLClient = EncryptionHelper.Decrypt(localLicenseXml, "Response");
                }
                else // Or create license details XML
                {
                    string encryptedLicenseXML = EncryptionHelper.Encrypt(licenseXML, "Response");
                    File.WriteAllText(installDirectory, encryptedLicenseXML);
                }

                // Get Edition value from CRM and Client and compare them
                string CRMEdition = string.Empty;
                //string clientEdition = string.Empty;

                // Create an XmlDocument object from CRM response XML and fetch Edition.
                XmlDocument xmlCRMDoc = new XmlDocument();
                XmlDocument xmlClientDoc = new XmlDocument();

                xmlCRMDoc.LoadXml(licenseXML);

                // if the license xml has Admin editiion
                // check if the user has  installed the admin pacakge and current user has access to the package
                // there might be a chance that LMA updated with Admin but user forgot to install
                // Admin package. If admin package is installed and user has access then user has admin edition
                // for admin edition there may not be LMA update and getLicense will send the default starter details

                if (IsAdminEdition())
                {   //ignore the GetLocalLicenseDetail resp
                    Constants.PRODUCT_EDITION = CRMEdition = Constants.ADMIN_EDITION;
                    DoesOrgHasAdminPackageInstalled = true;
                }
                else
                {
                    // If it’s a sandbox, If it's sandbox and no Admin package is present, always make it Enterprise.
                    if (adapter.IsSandBox())
                        Constants.PRODUCT_EDITION = CRMEdition = Constants.ENTERPRISE_EDITION;
                    else
                        Constants.PRODUCT_EDITION = CRMEdition = FindNodeValue(xmlCRMDoc, EditionNode);
                }

                // Create an XmlDocument object from Local XML and fetch Edition.
                if (!string.IsNullOrEmpty(licenseXMLClient))
                {
                    xmlClientDoc.LoadXml(licenseXMLClient);
                    // Always use edition recieved from CRM 
                    //Constants.PRODUCT_EDITION = clientEdition = FindNodeValue(xmlClientDoc, EditionNode);
                }

                //# if DEBUG
                //            //Constants.PRODUCT_EDITION = "Standard";
                //#endif

                // check feature data modified date value and return if feature data needs to be updated or not
                doFeatureUpdate = !string.Equals(FindNodeValue(xmlCRMDoc, FeatureDataModifiedNode), FindNodeValue(xmlClientDoc, FeatureDataModifiedNode));

                /* this call is not required since server handles this logic. ie. once in every 24 hours, local license info will
                 * be pulled from LMA
                // check last sync date when feature edition was updated, if it's > 24 hrs submit async call to update
                //bool doSyncFeatureEdition = CheckLastSyncDateFeatureEdition(FindNodeValue(xmlSFDCDoc, SyncDateNode));
                //if (doSyncFeatureEdition)
                //    adapter.SubmitSyncLicenseAndFeatureDetail();
                */
                // if feature update is true mean, Edition details has changed update existing file on local machine
                if (doFeatureUpdate)
                {
                    string encryptedLicenseXML = EncryptionHelper.Encrypt(licenseXML, "Response");
                    File.WriteAllText(installDirectory, encryptedLicenseXML);
                }
            }
            // if feature update is true or syncfeature has been done
            //doFeatureUpdate = doFeatureUpdate || doSyncFeatureEdition;
        }
        #region " no need to call this , moved to server"
        /*NOT REQUIRED IN CLIENT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastSyncDate"></param>
        /// <returns></returns>
        internal bool CheckLastSyncDateFeatureEdition(string lastSyncDate)
        {
            bool bReturnVal = false;
            DateTime lastSyncDateTime;

            // if not valid date do sync feature edition
            if (!DateTime.TryParse(lastSyncDate, out lastSyncDateTime))
                return true;

            DateTime now = DateTime.Now;
            // if date valid, check last sync was done before 24 hours, if yes return true
            if (now.AddHours(-24) > lastSyncDateTime && lastSyncDateTime <= now)
                bReturnVal = true;

            return bReturnVal;
        }

        */
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        internal string FindNodeValue(XmlDocument xmlDoc, string nodeToFind)
        {
            string editionValue = string.Empty;
            XmlNode root = xmlDoc.DocumentElement;
            if (root != null)
            {
                XmlNode node = root.SelectSingleNode(nodeToFind);
                if (node != null)
                    editionValue = node.InnerText;
            }

            return editionValue;
        }

        public void GetFeatureDetails(bool bUpdateFeatureDetails)
        {
            string featureXML = adapter.GetFeatureDetail();
            if (!string.IsNullOrEmpty(featureXML))
            {
                // Get feature detail XML from install folder or CRM
                string installDirectory = adapter.GetLicenseFilePath() + Constants.LMA_FEATURE_DETAIL;
                if (bUpdateFeatureDetails || !File.Exists(installDirectory))
                {
                    // Encrypt it store it install folder
                    File.WriteAllText(installDirectory, EncryptionHelper.Encrypt(featureXML, "Response"));
                    lmaDoc.LoadXml(featureXML);

                    XmlNode baseFeatureNode = lmaDoc.SelectSingleNode("//EditionRule");
                }
                else if (File.Exists(installDirectory))
                {
                    string featureXMLClient = EncryptionHelper.Decrypt(File.ReadAllText(installDirectory), "Response");
                    lmaDoc.LoadXml(featureXMLClient);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal string GetLicenseFilePath()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus\\");

            DirectoryInfo di = new DirectoryInfo(folderName);
            if (!di.Exists)
                di.Create();

            return folderName;
        }

        public bool CheckEditionRules(string appType, string edition)
        {

            /*
             * AppType	Edition Supported
            Essentials	Essentials
	            Enterprise
	            Power Admin
	            Reporting
	
            Reporting	Enterprise
	            Power Admin
	            Reporting
	
            Presto	Presto
	
            Data Migration	Power Admin
            */

            if (appType.Equals(Constants.ADMIN_EDITION))
            {
                appType = "Data Migration";
            }



            if (string.IsNullOrEmpty(edition))
            {
                return false;
            }
            if (string.IsNullOrEmpty(appType))
            {
                return false;
            }

            if (appType.Equals(edition)) return true;

            try
            {
                XmlDocument xmldc = new XmlDocument();
                //xmldc.Load("C:\\AppBuilder\\Licensing\\FeatureXML.xml"); //change to generic location
                XmlNode baseFeatureNode = lmaDoc.SelectSingleNode("//EditionRule");
                XmlNodeList Editions = baseFeatureNode.SelectNodes("//Rule[@AppType='" + appType + "']");

                //  if (string.IsNullOrEmpty(Editions[0].InnerXml)) return false;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Editions[0].InnerXml);
                XmlElement root = doc.DocumentElement;
                XmlNodeList elements = root.ChildNodes;
                bool editionFound = false;
                string EditionData = null;
                for (int i = 0; i < elements.Count; i++)
                {
                    EditionData = elements[i].InnerXml;
                    if (EditionData.Equals(edition))
                    {
                        editionFound = true;
                        break;
                    }

                }
                return editionFound;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

        #region "Serialize & Deserialize"

        /*
        public void Serialize()
        {
            licenceMetadata = ApttusXmlSerializerUtil.Deserialize<XAuthorForExcel>(lmaDoc.InnerXml);
        }

        public void Deserialize()
        {
            XAuthorForExcel licenceMetadata = new XAuthorForExcel();

            licenceMetadata.Package.Edition = "Standard";
            //licenceMetadata.Package.UpdatedDate = "";
            licenceMetadata.Package.Features.Designer.AppCreate.ExistingTemplate = "true";
            licenceMetadata.Package.Features.Designer.QuickApp = "true";
            licenceMetadata.Package.Features.Designer.SalesForceObjects.CrossTab = "true";
            licenceMetadata.Package.Features.Designer.DisplayMap.MultiSelect = "true";
            licenceMetadata.Package.Features.Designer.SaveMap.MultiSelect = "true";

            licenceMetadata.Package.Features.Designer.Actions.AddRow = "true";
            licenceMetadata.Package.Features.Designer.Actions.Checkin = "true";
            licenceMetadata.Package.Features.Designer.Actions.Checkout = "true";
            licenceMetadata.Package.Features.Designer.Actions.Clear = "true";
            licenceMetadata.Package.Features.Designer.Actions.Display = "true";
            licenceMetadata.Package.Features.Designer.Actions.Macro = "false";
            licenceMetadata.Package.Features.Designer.Actions.Paste = "true";
            licenceMetadata.Package.Features.Designer.Actions.PasteSourceData = "true";
            licenceMetadata.Package.Features.Designer.Actions.Query = "true";
            licenceMetadata.Package.Features.Designer.Actions.SalesforceMethod = "true";
            licenceMetadata.Package.Features.Designer.Actions.Save = "true";
            licenceMetadata.Package.Features.Designer.Actions.SaveAttachment = "true";
            licenceMetadata.Package.Features.Designer.Actions.SearchandSelect = "true";
            licenceMetadata.Package.Features.Designer.Actions.SwitchConnection = "true";

            licenceMetadata.Package.Features.Designer.ActionFlow.AddTrigger = "true";
            licenceMetadata.Package.Features.Designer.SourceData = "true";
            licenceMetadata.Package.Features.Designer.ApplicationSetting.RuntimeSetting = "true";

            licenceMetadata.Package.Features.Runtime.PasteFromMapping = "true";

            string serializedXML = ApttusXmlSerializerUtil.Serialize<XAuthorForExcel>(licenceMetadata);
        }
        */

        /* if the current user has admin user license or not
         * true -> Admin Edition
         */
        public bool IsAdminEdition()
        {
            //first check if the user is a sand box user
            //IsSandBox = adapter.IsSandBox();
            //if (IsSandBox)
            //{  
            //    return adapter.IsAdminPackageEditionUser();
            //}
            return adapter.IsAdminPackageEditionUser();



        }

        #endregion
        public bool DoesOrgHasAdminPackageInstalled {
            get;
            set;
        }

        public string SetEditionForApps(string CurrentEdition)
        {
            //set the edition 
            if (CurrentEdition != null) return CurrentEdition;
            // if current Edition == null and admin package installed, then set PA or ENt
            if (DoesOrgHasAdminPackageInstalled)
                return Constants.ADMIN_EDITION;
            else
                return Constants.ENTERPRISE_EDITION;

        }
        /**************-<EditionRule>
        -<Rule AppType="Essentials">
        -<Editions>
        <Edition>Essentials</Edition>
        <Edition>Enterprise</Edition>
        <Edition>Power Admin</Edition>
        </Editions>
        </Rule>
        -<Rule AppType="Reporting">
        <Editions>
        <Edition>Enterprise</Edition>
        <Edition>Power Admin</Edition>
        <Edition>Reporting</Edition>
        </Editions>
        </Rule>
        -<Rule AppType="Enterprise">
        -<Editions>
        <Edition>Enterprise</Edition>
        <Edition>Power Admin</Edition>
        </Editions>
        </Rule>
        -<Rule AppType="Presto">
        -<Editions>
        <Edition>Presto</Edition>
        </Editions>
        </Rule>
        -<Rule AppType="Data Migration">
        -<Editions>
        <Edition>Power Admin</Edition>
        </Editions>
        </Rule>
        -<Rule AppType="Power Admin">
        -<Editions>
        <Edition>Power Admin</Edition>
        </Editions>
        </Rule>
</EditionRule>***************/

        /* keep edition name and valid editions in a dictionary keyed
         * by the edition name. this list is used while changing the app to enforce the forward edition rule
         * i.e apps can be changed upwards, not backwards. The xml is coming from LMA and this can be changed anytime
         */
        public LicenseEditionRuleStruct GetEditionRules()
        {
            XmlDocument xmldc = new XmlDocument();
            LicenseEditionRuleStruct EditionRule = new LicenseEditionRuleStruct();
            // xmldc.Load("C:\\AppBuilder\\Licensing\\FeatureXML.xml");
            // Change to generic location
            // XmlNode baseFeatureNode = lmaDoc.SelectSingleNode("//EditionRule");
            if (lmaDoc.HasChildNodes)
            {
                XmlNodeList Editions = lmaDoc.SelectNodes("//EditionRule");

                foreach (XmlNode node in Editions)
                {
                    foreach (XmlNode nd in node.ChildNodes)
                    {
                        string nam = nd.Attributes.Item(0).Value;
                        List<string> rule = new List<string>();
                        if (EditionRule.EditionRuleUI.ContainsKey(nam)) continue;
                        EditionRule.EditionRuleUI.Add(nam, rule);
                        foreach (XmlNode nde in nd.ChildNodes)
                        {
                            foreach (XmlNode nde2 in nde.ChildNodes)
                            {
                                string ed = nde2.InnerText;
                                rule.Add(nde2.InnerText);
                            }
                        }
                    }
                    // Editions.Add(new LicenseEditionStruct(node["Id"].InnerText, node["Text"].InnerText));

                }
            }
            return EditionRule;
        }

        public List<LicenseEditionStruct> GetEditionsForUI()
        {
            XmlDocument xmldc = new XmlDocument();
            //  xmldc.Load("C:\\AppBuilder\\Licensing\\FeatureXML.xml"); //change to generic location
            // XmlNode baseFeatureNode = lmaDoc.SelectSingleNode("//EditionRule");
            //XmlNode baseFeatureNode2 = lmaDoc.SelectSingleNode("//EditionsUI");


            //List<LicenseEditionStruct> Editions = new List<LicenseEditionStruct>();
            //foreach (XmlNode node in baseFeatureNode2)
            //{
            //    Editions.Add(new LicenseEditionStruct(node["Id"].InnerText, node["Text"].InnerText));

            //}
            /*hard code the values since the xml from LMA doesn't have this info
             * and the new license URL will be an issue without the 4.64 package*/

            List<LicenseEditionStruct> Editions = new List<LicenseEditionStruct>();
            Editions.Add(new LicenseEditionStruct("Reporting", "Reporting"));
            Editions.Add(new LicenseEditionStruct("Enterprise", "Enterprise"));
            Editions.Add(new LicenseEditionStruct("Presto", "Presto"));
            Editions.Add(new LicenseEditionStruct("Power Admin", "Power Admin"));
            // Editions.Add(new LicenseEditionStruct(node["Id"].InnerText, node["Text"].InnerText));
            return Editions;
        }

    }
    public class LicenseEditionStruct
    {
        public string ID {
            get;
            set;
        }
        public string Name {
            get;
            set;
        }
        public LicenseEditionStruct(string id, string text)
        {
            ID = id;
            Name = text;
        }
    }
    public class LicenseEditionRuleStruct
    {
        private Dictionary<string, List<string>> EditionRules;
        public LicenseEditionRuleStruct()
        {
            EditionRules = new Dictionary<string, List<string>>();
        }
        public Dictionary<string, List<string>> EditionRuleUI {
            get { return EditionRules; }

        }

    }

}


