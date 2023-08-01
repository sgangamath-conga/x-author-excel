/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    interface IClearActionController
    {
        void SetView();

        void Save(Core.ClearActionModel Model);

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
