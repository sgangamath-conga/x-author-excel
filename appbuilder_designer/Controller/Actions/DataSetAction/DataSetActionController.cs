using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class DataSetActionController
    {
        private DataSetActionModel Model;
        private DataSetActionView View;
        internal FormOpenMode FormOpenMode;
        public DataSetActionController(DataSetActionView view, DataSetActionModel model, FormOpenMode formOpenMode)
        {
            FormOpenMode = formOpenMode;
            View = view;
            Model = model;

            if (View != null)
            {
                View.SetController(this, model);
                View.ShowDialog();
            }
        }

        internal void Save()
        {
            if (FormOpenMode == FormOpenMode.Create)
                Model.Write();
        }
    }
}
