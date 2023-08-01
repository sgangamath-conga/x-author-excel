/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class MacroActionModel : Action
    {        
       
        public string MacroName { get; set; }
        public bool MacroOuput { get; set; }
        public bool ExcelEventsDisabled { get; set; }

        //public string ActionName { get; set; }
        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }


       
    }
}
