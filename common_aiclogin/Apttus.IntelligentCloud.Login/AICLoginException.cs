using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.IntelligentCloud.LoginControl
{
    class AICLoginException : Exception
    {
        private string ExceptionMessage;

        public AICLoginException()
        {
            ExceptionMessage = base.Message;
        }
        public AICLoginException(string Message)
        {
            ExceptionMessage = Message;
        }

        public override string Message {
            get {
                return ExceptionMessage;
            }
        }
    }
}
