using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.Core
{
    public class ActionRequest
    {
        public ActionRequest()
        {
            RequestHeader = new ActionRequestHeader();
        }

        public ActionRequestHeader RequestHeader { get; private set; }
    }

    public class ActionRequestHeader
    {
        public string InstanceUrl { get; set; }
        public string AccessToken { get; set; }
        public string ApiVersion { get; set; }
    }
}
