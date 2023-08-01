/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SwitchConnectionActionModel : Action
    {
        public string SwitchToConnectionName { get; set; }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }
    }
}
