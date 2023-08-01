using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner{

    class PasteRowActionController
    {
        private PasteRowActionView View;
        private PasteRowActionModel Model;
        private FormOpenMode FormOpenMode;

        public PasteRowActionController(PasteRowActionView view, PasteRowActionModel model, FormOpenMode formOpenMode)
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

        public void SetView()
        {
            View.FillObjects();
            if (FormOpenMode == Core.FormOpenMode.Edit)
                View.UpdateControls(Model); 
        }

        public void Save(PasteRowActionModel Model)
        {
            Model.Type = Constants.PASTEROW_ACTION;

            if (FormOpenMode == Core.FormOpenMode.Create)
                Model.Write();
        }

        public void ValidateAction(Control control, System.ComponentModel.CancelEventArgs e, string errorMsg)
        {
            //String errorMsg = "Name cannot be empty";
            if (string.IsNullOrEmpty(control.Text))
            {
                e.Cancel = true;
                View.SetError(control, errorMsg);
            }
            else
            {
                e.Cancel = false;
                View.SetError(control, string.Empty);
            }
        }
    }
}
