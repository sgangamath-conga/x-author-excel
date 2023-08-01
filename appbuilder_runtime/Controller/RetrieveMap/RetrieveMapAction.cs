using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppRuntime
{
    public class RetrieveMapAction
    {


        public void getData()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
            configurationManager.LoadAppFromLocal("a1ri0000000XkOSAA0", "MyFirstApps");

            Apttus.XAuthor.Core.Action action = configurationManager.Actions[0];
            SearchAndSelect config = (SearchAndSelect)action;
            SearchAndSelectAction sas = new SearchAndSelectAction(config);
            sas.Execute();

        }
    }
}
