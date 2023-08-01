using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AICAdapter
{
    public class AICRecord
    {
        public AICRecord(string objectId)
        {
            this.ObjectId = objectId;
            this.Record = new Dictionary<string, object>();
        }

        public string ObjectId { get; set; }

        public string RecordId { get; set; }

        public Dictionary<string, object> Record { get; set; }

        public bool HasAttachment { get; set; }

        public string FileName { get; set; }

        public byte[] FileStream { get; set; }
    }
}
