/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class RetrieveActionModel : Action
    {
        public Guid RetrieveMapId { get; set; }
        public List<ShowQueryFilterObjectConfiguration> ShowQueryFilterObjectConfigurations { get; set; }
        public bool DisableExcelEvents { get; set; }
        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);
        }       
    }

    public class ShowQueryFilterObjectConfiguration
    {
        public Guid AppObjectUniqueId { get;set;}
        public string CellReferenceValue { get; set; }
        public CellReferenceType CellReferenceType { get; set; }
    }
}
