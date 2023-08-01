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
    public interface IRetrieveActionController
    {
        void SetView();

        bool Save(string ActionName, Guid RetrieveMapId, List<ShowQueryFilterObjectConfiguration> ShowQueryFilterObjectConfigurations, bool disableExcelEvents = true);

        void Cancel();

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
