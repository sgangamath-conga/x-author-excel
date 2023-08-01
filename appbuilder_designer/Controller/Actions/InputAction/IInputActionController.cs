/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;
using System.ComponentModel;

namespace Apttus.XAuthor.AppDesigner
{
    interface IInputActionController
    {

        void SetView();

        void Save(Core.InputActionModel Model);

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
