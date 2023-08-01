using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.Core
{
    public class ActionResponse
    {
        public ActionResponse()
        {
            Status = ActionResultStatus.Pending_Execution;
            FailureMessage = string.Empty;
        }

        public ActionResultStatus Status { get; set; }
        public string FailureMessage { get; set; }
    }
}
