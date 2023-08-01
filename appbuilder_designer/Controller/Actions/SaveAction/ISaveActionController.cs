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
    public interface ISaveActionController
    {
        void SetView();

        bool Save(string ActionName, Guid SaveMapId, bool bEnableCollisionDetection, bool bEnablePartialSave, bool bEnableRowHighlightErrors, int BatchSize);

        void Cancel();

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
