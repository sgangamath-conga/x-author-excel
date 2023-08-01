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
    public interface ICheckInController
    {
        void SetView();

        void Save(string ActionName, string FileName, string cellReference, ApttusObject obj);

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);

        string GetActiveColReference();    
    }
}
