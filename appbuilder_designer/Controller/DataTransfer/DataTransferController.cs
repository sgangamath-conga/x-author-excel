/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class DataTransferController
    {
        DataTransferView View;
        DataTransferMapping Model;
        FormOpenMode FormOpenMode;
        public DataTransferController(DataTransferMapping model, DataTransferView view, FormOpenMode formOpenMode)
        {
            View = view;
            Model = model;
            FormOpenMode = formOpenMode;
            if (View != null)
            {
                View.SetController(this);
                View.ShowDialog();
            }
        }

        internal void Save(DataTransferMapping Model)
        {
            if (Model != null)
                ConfigurationManager.GetInstance.SaveMapping(Model);            
        }
    }
}
