using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Apttus.XAuthor.AppRuntime
{
    public interface ISaveHelper
    {
        void ValidateLookupNamesBeforeSave(List<ApttusSaveRecord> SaveRecords, ApttusObject AppObject, out StringBuilder LookupErrors);
        void SetColumnNamesOfChangedDataTable(RepeatingGroup currentObjectRepeatingGroup, List<SaveField> currentObjectSaveFields, ref DataTable changedDataTable, ApttusObject apttusObject, Guid repeatingDataSetAppObjectUniqueID);
        string GetLookupIdFromRelationalFieldID(Guid AppObjectUniqueID, ApttusObject currentAppObject, string originalRelationalSaveFieldId, string relationalSaveFieldId);
    }
}
