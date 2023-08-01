/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public enum ClearActionMapType
    {
        SaveMap,
        DisplayMap
    }

    [Serializable]
    public class ClearActionModel : Action
    {
        //SaveMapId can refer to either a RetrieveMap Id or a SaveMap Id. Cannot change the name of the variable SaveMapId, because all the previous apps will
        //fail to load.
        public Guid SaveMapId { get; set; }
        public List<Guid> AppObjects { get; set; }
        public ClearActionMapType MapType { get; set; }
        public bool DisableExcelEvents { get; set; }
        public ClearActionModel()
        {
            AppObjects = new List<Guid>();
        }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }
    }
}
