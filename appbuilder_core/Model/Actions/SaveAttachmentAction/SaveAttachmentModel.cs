/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SaveAttachmentModel : Action
    {
        public string FileName { get; set; }
        public Guid AppObjectId { get; set; }
        public FileSaveType SaveType { get; set; }
        public AttachmentFormat AttachmentFormat { get; set; }
        public bool HasCustomSheets { get; set; }
        public List<string> IncludedSheets { get; set; }
        public bool IsFileNameUsingCellRef { get; set; }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();

            objectList.Add(AppObjectId.ToString());

            return objectList.ToList();
        }

    }
}
