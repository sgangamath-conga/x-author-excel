/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ApttusSaveSummary
    {
        public int InsertCount { get; set; }

        public int UpdateCount { get; set; }

        public int UpsertCount { get; set; }

        public int DeleteCount { get; set; }

        public string ErrorMessage { get; set; }

        public string FirstErrorMessage { get; set; }

    }
}
