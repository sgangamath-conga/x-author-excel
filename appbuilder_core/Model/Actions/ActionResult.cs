/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ActionResult
    {
        private ActionResultStatus status;

        public ActionResultStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public ActionResult()
        {
            status = ActionResultStatus.Failure;
        }
    }
}
