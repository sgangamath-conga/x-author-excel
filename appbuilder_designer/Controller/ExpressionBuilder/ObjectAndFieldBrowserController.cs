/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace Apttus.XAuthor.AppDesigner
{
    public class ObjectAndFieldBrowserController
    {
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ObjectAndFieldBrowserView View;
        public ApttusObject AppObject { get; set; }
        public SearchFilter SearchFilter { get; set; }

        public ObjectAndFieldBrowserController(ObjectAndFieldBrowserView view, ApttusObject currentAppObject, SearchFilter currentSearchFilter)
        {
            this.View = view;
            this.AppObject = currentAppObject;
            this.SearchFilter = currentSearchFilter;
            view.SetController(this);
        }

        public bool Browse()
        {
            DialogResult dr = this.View.ShowDialog();
            // Return true if the user clicked Apply on Object and Field dialog
            if (dr == DialogResult.OK)
                return true;
            else
                return false;
        }

    }
}
