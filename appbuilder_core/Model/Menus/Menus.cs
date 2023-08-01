/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Apttus.XAuthor.Core
{
    [XmlRoot("Menus")]
    public class Menus
    {
        private List<MenuGroup> MenuGrpsCollection;

        public Menus()
        {
            MenuGrpsCollection = new List<MenuGroup>();
        }
        [DataMember]
        [XmlArray("MenuGroups")]
        public List<MenuGroup> MenuGroupsCollection
        {
            get { return MenuGrpsCollection; }
            set { MenuGrpsCollection = value; }
        }

        public bool EnableAddRowMenu { get; set; }

        public bool EnableDeleteRowMenu { get; set; }
        
        public bool EnableChatterMenu { get; set; }

        public bool EnableAddMatrixRowMenu { get; set; }

        public bool EnableAddMatrixColumnMenu { get; set; }

        [ScriptIgnore]
        public Menus MenuData
        {
            get { return this; }
        }
        

        public void WriteMenu()
        { 
            try
            {
                ConfigurationManager.GetInstance.AddMenus(this);
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), "Save Menu");
            }
        }

    }
}
