using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class MissingFieldItem
    {
        public string ObjectID { get; set; }
        public string ObjectName { get; set; }
        public string FieldName { get; set; }
        public string FieldId { get; set; }
        public List<RetrieveMap> FieldMissingInDisplayMaps { get; private set; }
        public List<SaveMap> FieldMissingInSaveMaps { get; private set; }
        public List<Core.Action> FieldMissingInActions { get; private set; }

        public MissingFieldItem()
        {
            FieldMissingInDisplayMaps = new List<RetrieveMap>();
            FieldMissingInActions = new List<Core.Action>();
            FieldMissingInSaveMaps = new List<SaveMap>();
        }

        internal void Add(IEnumerable<RetrieveMap> rMaps)
        {
            FieldMissingInDisplayMaps.AddRange(rMaps);
        }

        internal void Add(IEnumerable<SaveMap> sMaps)
        {
            FieldMissingInSaveMaps.AddRange(sMaps);
        }

        internal void Add(IEnumerable<Core.Action> actions)
        {
            FieldMissingInActions.AddRange(actions);
        }
    }
}
