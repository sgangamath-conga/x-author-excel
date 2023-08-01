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
    public interface ISaveAttachmentController
    {
        void SetView();

        bool Save(string ActionName, string FileName, FileSaveType SaveType, ApttusObject obj, AttachmentFormat format, bool hasCustomSheets, List<string> sheets,bool IsFileNameusingCellRef);

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);

        string GetActiveColReference();
    }
}
