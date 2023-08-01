using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class ExternalLibrary
    {
        public string Id { get; set; }
        public string ExternalLibraryName { get; set; }
        public List<string> Actions { get; set; }

        public ExternalLibrary()
        {
            Actions = new List<string>();
        }
    }
}
