using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class SaveAndRestoreModel<T>
    {
        private T obj;
        private string xmlSerializedObj;

        public SaveAndRestoreModel(T obj)
        {
            this.obj = obj;
            Save();
        }

        public void Save()
        {
            xmlSerializedObj = ApttusXmlSerializerUtil.Serialize<T>(obj);
        }

        public void Restore()
        {
            obj = ApttusXmlSerializerUtil.Deserialize<T>(xmlSerializedObj);
        }
    }

    public class AddRowActionController
    {
        private AddRowActionView View;
        private AddRowActionModel Model;
        private FormOpenMode Mode;
        private SaveAndRestoreModel<AddRowActionModel> preSaveModel;

        public AddRowActionController(AddRowActionView view, AddRowActionModel model, FormOpenMode mode)
        {
            View = view;
            Model = model;
            Mode = mode;
            preSaveModel = new SaveAndRestoreModel<AddRowActionModel>(model);
            
            if (View != null)
            {
                View.SetController(this, Model);
                View.ShowDialog();
            }
        }

        public FormOpenMode FormOpenMode
        {
            get
            {
                return Mode;
            }
        }

        public void Save()
        {
            if (FormOpenMode == Core.FormOpenMode.Create)
                Model.Write();
        }

        public void Cancel()
        {
            preSaveModel.Restore();
        }
    }
}
