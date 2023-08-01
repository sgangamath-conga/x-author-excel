using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class DataTypeChangeInfo
    {
        public string AppObjectId { get; set; }
        public string FieldId { get; set; }
        public Datatype From { get; set; }
        public Datatype To { get; set; }
        public bool CanUpdateDataType { get; set; }
        public ApttusField SynchedField { get; set; }

        public List<RetrieveMap> FieldUsedInDisplayMaps { get; private set; }
        public List<Core.Action> FieldUsedInActions { get; private set; }

        public List<SaveMap> FieldsUsedInSaveMaps { get; private set; }
        public DataTypeChangeInfo()
        {
            FieldUsedInDisplayMaps = new List<RetrieveMap>();
            FieldUsedInActions = new List<Core.Action>();
            FieldsUsedInSaveMaps = new List<SaveMap>();
        }

        internal void Add(IEnumerable<SaveMap> sMaps)
        {
            FieldsUsedInSaveMaps.AddRange(sMaps);
        }

        internal void Add(IEnumerable<RetrieveMap> rMaps)
        {
            FieldUsedInDisplayMaps.AddRange(rMaps);
        }

        internal void Add(IEnumerable<Core.Action> actions)
        {
            FieldUsedInActions.AddRange(actions);
        }
    }
}
