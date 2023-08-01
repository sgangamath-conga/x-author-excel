/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner.Forms;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner
{
    public class AppDefinitionUtil
    {

        public void DisplayAppDefnUI()
        {
            // Display combined Salesforce Objects UI, since CrossTab is now merged in single screen
            ApplicationDefinition apttusActions = new ApplicationDefinition();
            apttusActions.ShowDialog();
        }

        public ApplicationDefinition DisplayAppDefnUI(bool ShowRepeating)
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetInstance;

            ApplicationDefinition apttusActions = new ApplicationDefinition();
            return apttusActions;
        }
    }
}
