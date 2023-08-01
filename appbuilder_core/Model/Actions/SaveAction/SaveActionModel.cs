/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SaveActionModel : Action
    {
        public Guid SaveMapId { get; set; }
        public bool EnableCollisionDetection { get; set; }
        public bool EnablePartialSave { get; set; }
        public bool EnableRowHighlightErrors { get; set; }
        public int BatchSize { get; set; }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);
        }
    }
}
