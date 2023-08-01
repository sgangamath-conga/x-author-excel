/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ExternalAction : Action
    {
        public string ExternalLibraryId { get; set; }

        public string TypeName { get; set; }
        public string TypeFullName { get; set; }

        public ExternalAction()
        {
            IsActionExternal = true;
            Type = Constants.EXTERNAL_ACTION;            
        }
    }

    //[Serializable]
    //[XmlInclude(typeof(SearchAndSelect)), XmlInclude(typeof(RetrieveActionModel)), XmlInclude(typeof(SaveActionModel)), XmlInclude(typeof(QueryActionModel)), XmlInclude(typeof(CallProcedureModel)),
    //XmlInclude(typeof(MacroActionModel)), XmlInclude(typeof(SaveAttachmentModel)), XmlInclude(typeof(ClearActionModel)), XmlInclude(typeof(CheckInModel)), XmlInclude(typeof(CheckOutModel)), XmlInclude(typeof(InputActionModel)),
    //XmlInclude(typeof(PasteSourceDataActionModel)), XmlInclude(typeof(SwitchConnectionActionModel)), XmlInclude(typeof(AddRowActionModel)), XmlInclude(typeof(PasteRowActionModel))]
    //public abstract class Action
    //{
    //    [XmlAttribute("Name")]
    //    [Required(ErrorMessage = "Action Name is required.")]
    //    public string Name { get; set; }

    //    [XmlAttribute("Id")]
    //    [Required(ErrorMessage = "Action ID is required.")]
    //    public string Id { get; set; }

    //    [XmlAttribute("Type")]
    //    [Required(ErrorMessage = "Action Type is required.")]
    //    public string Type;

    //    public virtual void Save() { }
    //}

    //public class InternalAction : Action
    //{
    //    ConfigurationManager configManager = ConfigurationManager.GetInstance;

    //    public override void Save()
    //    {
    //        configManager.AddAction(this);
    //    }
    //}
}
