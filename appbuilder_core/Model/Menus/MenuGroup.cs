/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    //[System.Xml.Serialization.XmlRoot("MenuGroup")]
    public class MenuGroup : AbstractMenu
    {
        private List<MenuItem> ItemsCollection;
        //  private Visibility visible = new Visibility();

        public MenuGroup()
            : base()
        {
            ItemsCollection = new List<MenuItem>();


        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [Browsable(false)]
        [DataMember]
        [XmlArray("MenuItems")]
        public List<MenuItem> MenuItems
        {
            get { return ItemsCollection; }
            set { }
        }
        /*
                 [Browsable(false)]
                public override Permission PermissionList
                {
                    get { return permission; }
                    set { }
                }
               */
        public MenuItem AddItem(MenuItem mnu)
        {
            MenuItems.Add(mnu);
            return mnu;
        }

        public bool DeleteItem(MenuItem item)
        {
            if (MenuItems.Remove(item)) return true;
            return false;
        }

        public bool EditItem(MenuItem item)
        {
            if (DeleteItem(item))
            {
                MenuItems.Add(item);
                return true;
            }
            return false;
        }

        private bool ItemExists(string id)
        {
            return false;
        }
        /*
         public override Visibility VisibleWhen
         {

             get { return visible; }
             set { }
         }*/


    }
}
