/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{

    public class MenuItem : AbstractMenu, IActionable
    {
     // private Visibility visible = new Visibility();
      
     
       /*
       [Browsable(false)]
        public override Permission PermissionList
        {
            get { return permission; }
            set { }
        }
       /*
        public override Visibility VisibleWhen
        {

            get { return visible; }
            set { }
        } 
        
       */
       string icon;
       [CategoryAttribute("General"), DescriptionAttribute("Icon for the control"),LocalDisplayName("MNUHOME_tblIcon")]
       public string Icon
       {
           get
           {
               return icon;
           }
           set
           {
               icon = value;
           }

       }
       private string tooltip;
       [CategoryAttribute("General"), DescriptionAttribute("Tool Tip for the control"),LocalDisplayName("MNUHOME_tblToolTip")]
       public string ToolTip
       {
           get
           {
               return tooltip;
           }
           set
           {
               tooltip = value;
           }

       }

       [Browsable(false)]
       public Guid WorkFlowID
       {
           get;
           set;
       }
       [XmlIgnore]
       [Browsable(false)]
       public string WorkFlowName
       {
           get;
           set;
       }
        public void SetAction(Workflow wf)
        {
            WorkFlowID = wf.Id;
            WorkFlowName = wf.Name;
        }
    }
    

}
