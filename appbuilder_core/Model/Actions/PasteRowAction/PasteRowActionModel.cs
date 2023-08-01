using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class PasteRowActionModel : Action
    {
        public Guid SaveMapId { get; set; }
        public Guid ListObjectId { get; set; }
        public AddRowLocation RowLocation { get; set; }
        public string SaveGroupTargetNamedRange { get; set; }
        public PasteType PasteType { get; set; }

        public void Write()
        {
			ConfigurationManager.GetInstance.AddAction(this);
        }
    }
}
