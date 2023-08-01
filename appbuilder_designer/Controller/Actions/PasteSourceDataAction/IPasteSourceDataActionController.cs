using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    interface IPasteSourceDataActionController
    {
        void SetView();

        void Save(PasteSourceDataActionModel Model);

        void ValidateAction(Control action, CancelEventArgs e, string errorMsg);
    }
}
