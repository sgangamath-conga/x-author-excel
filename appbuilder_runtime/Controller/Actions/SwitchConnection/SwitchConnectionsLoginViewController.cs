/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class SwitchConnectionsLoginViewController
    {
        private SwitchConnectionsLoginView view;
        public List<SwitchConnectionActionModel> connections;
        public bool allConnectionsLoggedIn;

        public SwitchConnectionsLoginViewController(List<SwitchConnectionActionModel> connections, SwitchConnectionsLoginView view)
        {
            this.connections = connections;
            this.allConnectionsLoggedIn = false;
            this.view = view;
            this.view.SetController(this);
            this.view.ShowDialog();
        }


    }
}
