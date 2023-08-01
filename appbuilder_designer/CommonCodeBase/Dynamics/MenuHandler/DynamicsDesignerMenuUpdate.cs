using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    class DynamicsDesignerMenuUpdate : IXAuthorDesignerMenuUpdate
    {
        public void ChangeMenuLabels()
        {
            
        }

        public void UpdateActions()
        {
            //Remove Salesforce Method Action from Actions Menu
            Microsoft.Office.Tools.Ribbon.RibbonSplitButton actionSplitButton = Globals.Ribbons.ApttusRibbon.groupBuild.Items[4] as Microsoft.Office.Tools.Ribbon.RibbonSplitButton;
            Microsoft.Office.Tools.Ribbon.RibbonControl control = actionSplitButton.Items.ElementAt(11);
            control.Visible = false;
        }

        public void UpdateAboutText()
        {

        }
    }
}
