/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// This class is used to generate the XML skeleton for the config file with all necessary child objects of App tag.
    /// App tag is represented by App class.
    /// </summary>
    [Serializable]
    [XmlRoot("Application")]
    public class Application
    {
        [XmlElement(ElementName = "Definition")]
        public Definition Definition { get; set; }

        [XmlElement(ElementName = "Menus")]
        public Menus AppMenus { get; set; }

        public List<Action> Actions { get; set; }

        public List<Workflow> Workflows { get; set; }

        public List<RetrieveMap> RetrieveMaps { get; set; }

        public List<SaveMap> SaveMaps { get; set; }

        public List<MatrixMap> MatrixMaps { get; set; }

        public WizardModel QuickAppModel { get; set; }

        public List<ExternalLibrary> ExternalLibraries { get; set; }

        public DataMigrationModel MigrationModel { get; set; }

        public Application()
        {
            Definition = new Definition();
            Actions = new List<Action>();
            Workflows = new List<Workflow>();
            RetrieveMaps = new List<RetrieveMap>();
            SaveMaps = new List<SaveMap>();
            MatrixMaps = new List<MatrixMap>();
            ExternalLibraries = new List<ExternalLibrary>();
        }
        // Launch from sales force doesn't work in offline due to 
        // the file name being unique. The serialization code lookfor appname.runtime.xml and 
        // Edit in Excel feature append timestamp with the file name so store the timestamp
        // and use the unique code to identify the xml
        [XmlIgnore]
        public  string EditInExcelAppUniquFileId
        {
            get;
            set;
        }
    }

    [Serializable]
    public class Definition
    {
        [XmlAttribute]
        public string Id { get; set; }

        public string UniqueId { get; set; }
        
        public string Name { get; set; }

        public string Version { get; set; }

        //public AppEdition Edition { get; set; }

        public string DesignerVersion { get; set; }

        public ApplicationType Type { get; set; }

        public List<Client> Clients { get; set; }

        public List<ApttusObject> AppObjects { get; set; }

        public List<CrossTabDef> CrossTabs { get; set; }
        
        public DataTransferMapping Mapping { get; set; }

        public AppSettings AppSettings { get; set; }
        [XmlIgnore()]
        public byte[] Schema { get; set; }
        public string EditionType { get; set; }

        public bool? IsMultiSelectPickListExistsInApp { get; set; }
        //IsActive is added to persist app details's changes. It does not have to do anything with App.
        //Active flag relates to record level attribute  
        [XmlIgnore()]
        public bool IsActive { get; set; }
        public Definition()
        {
            Clients = new List<Client>();
            AppObjects = new List<ApttusObject>();
            CrossTabs = new List<CrossTabDef>();
            AppSettings = new AppSettings();
        }
    }

    [Serializable]
    [XmlType("Client")]
    public class Client
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string WorkingLocation { get; set; }
    }

    public class CrossTabDef
    {
        public string Name { get; set; }
        public Guid UniqueId { get; set; }
        public ApttusObject RowHeaderObject { get; set; }
        public ApttusObject ColHeaderObject { get; set; }
        public ApttusObject DataObject { get; set; }
    }
}
