/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public interface IMacroActionController
    {
        void SetView();

        void Save(string ActionName, string MacroName, bool MacroOutput, bool ExcelEventsDisabled);

        void Cancel();

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
