using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SavedFilter
    {
        public string Name { get; set; }
        public string ActionID { get; set; }
        public ApttusDataSet Data { get; set; }
    }

    [Serializable]
    public class SavedFilters
    {
        public List<SavedFilter> UserDefinedFilters { get; set; }

        public SavedFilters()
        {
            UserDefinedFilters = new List<SavedFilter>();
        }
    }
}
