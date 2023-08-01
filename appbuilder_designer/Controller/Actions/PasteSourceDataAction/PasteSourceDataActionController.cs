using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class PasteSourceDataActionController : IPasteSourceDataActionController
    {
        private PasteSourceDataActionModel Model;
        private PasteSourceDataActionView View;
        private FormOpenMode FormOpenMode;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public PasteSourceDataActionController(PasteSourceDataActionModel model, PasteSourceDataActionView view, FormOpenMode formOpenMode)
        {
            Model = model;
            View = view;

            FormOpenMode = formOpenMode;

            if (view != null)
            {
                view.SetController(this);
                view.ShowDialog();
            }
        }

        public void SetView()
        {
            View.UpdateControls(Model);
        }

        public void Save(PasteSourceDataActionModel Model)
        {
            Model.Type = Constants.PASTESOURCEDATA_ACTION;
            if (FormOpenMode == Core.FormOpenMode.Create)
                Model.Write();

            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CONST_ACTIONSAVEMESSAGE"), resourceManager.GetResource("COMMON_btnSave_Text"));

            View.Dispose();
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
