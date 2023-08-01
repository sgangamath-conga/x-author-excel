/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    // Menu class to be displayed in the property grid
    /// </summary>
    /// 
    [DefaultPropertyAttribute("Name")]
    public abstract class AbstractMenu
    {
        string name;

        string label;
        string tooltip;
       
        bool visible;

        [CategoryAttribute("General"), DescriptionAttribute("Name"), LocalDisplayName("COMMON_Name_Text")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }

        }

        protected AbstractMenu()
        {
            Id = GetGUID();
            Visible = true;
        }

        [XmlIgnoreAttribute] //TODO remove this attribite b /c id is required
        [CategoryAttribute("General"), DescriptionAttribute("Id")]
        [Browsable(false)]
        public string Id
        {
            get;
            set;

        }

        string order;
        [CategoryAttribute("General"), DescriptionAttribute("Sequence"), LocalDisplayName("MNUHOME_tblOrder")]
        public string Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }

        }
       // [CategoryAttribute("General"), DescriptionAttribute("Label to be displayed")]
        [Browsable(false)]
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }

        }

        
       

        //[CategoryAttribute("General"), DescriptionAttribute("Enable or Disable")]
        [Browsable(false)]
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }

        }


        [CategoryAttribute("Advanced"), DescriptionAttribute("Enable or Disable")]
        /*
         public abstract Visibility VisibleWhen
         {

             get;
             set;
         }
        */

        [XmlArray("UserProfiles")]
        [Browsable(false)]
        public List<ApttusUserProfile> Profiles
        {
            get;
            set;
        }

        public string GetGUID()
        {
            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }

    }
}
