using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AICAdapter
{
    public class SaveResult
    {
        public string id { get; set; }

        public bool success { get; set; }

        public string error { get; set; }

        public bool isUpsert { get; set; }
    }
}
