using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class PasteSourceDataActionModel : Action
    {
        public bool InputUserForFileName { get; set; }
        public PasteSourceDataActionModel()
        {
        }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);
        }
    }
}
