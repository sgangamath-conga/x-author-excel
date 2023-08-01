using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// 
    /// </summary>    
    public abstract class Action
    {
        public Action()
        {

        }

        [XmlAttribute("Name")]
        [Required(ErrorMessage = "Action Name is required.")]
        public string Name { get; set; }

        [XmlAttribute("Id")]
        [Required(ErrorMessage = "Action ID is required.")]
        public string Id { get; set; }

        [XmlAttribute("Type")]
        [Required(ErrorMessage = "Action Type is required.")]
        public string Type { get; set; }

        [XmlAttribute("IsActionExternal")]
        public bool IsActionExternal { get; set; }

        //For Future use
        internal void Save()
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Will be used in Runtime
    /// </summary>
    public interface IActionRuntime
    {
        ActionResponse Execute(ActionRequest request);
    }
}
