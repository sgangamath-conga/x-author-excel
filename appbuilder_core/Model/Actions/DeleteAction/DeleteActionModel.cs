using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class DeleteActionModel : Action
    {
        public Guid AppObjectUniqueId { get; set; }
        public Guid MapId { get; set; }
        public Guid SaveGroupId { get; set; }

        public List<SearchFilterGroup> DeleteFilterGroups { get; set; }
        public bool PromptConfirmationDialog { get; set; }

        public List<string> GetInputObjects()
        {
            List<string> objectSet = new List<string>();
            objectSet.Add(AppObjectUniqueId.ToString());
            return objectSet;
        }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);
        }
    }
}
