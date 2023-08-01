using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class PicklistTypeChangeInfo
    {
        public Guid AppObjectUniqueId { get; set; }
        public string FieldId { get; set; }
        public PicklistType From { get; set; }
        public PicklistType To { get; set; }
        public bool CanUpdateDataType { get; set; }
    }
}
