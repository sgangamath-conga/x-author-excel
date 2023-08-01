using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    interface IPartialClone<T>
    {
        T PartialClone();
    }

    public struct DataMigrationConstants
    {
        public const string DisplayMapNameFormat = "{0} DM";
        public const string SaveMapNameFormat = "{0} SM";

        public const string SearchAndSelectQueryNameFormat = "Search {0}";
        public const string QueryNameFormat = "{0} Query";
        public const string QueryNameFormat_All = "All - {0} Query";
        public const string DisplayActionNameFormat = "Display {0}";
        public const string SaveActionNameFormat = "Save {0}";

        public const string RetrieveActionFlowName = "Export Selective";
        public const string RetrieveActionFlowName_ALL = "Export All";

        public const string RetrieveMenuIcon = "SynchronizeData";
        public const string RetrieveMenuIcon_ALL = "PivotTablePublishChanges";

        public const string SaveActionFlowName = "Save All";
        public const string SaveMenuIcon = "SaveObjectAs";

        public const string AppInfoActionFlowName = "App Info";
        public const string AppInfoIcon = "Info";

        public const string SaveSourceDataActionFlowName = "Save Source Data";
        public const string SaveSourceData_UpdateExternalID_ActionName = "Update External Ids";
        public const string SaveSourceData_CopyAndDeleteLookUPColumns_ActionName = "Copy and Delete Lookup Columns";
        public const string SaveSourceData_SaveAsXLSX_ActionName = "Save as XLSX";
        public const string SaveSourceData_SaveExternalID_ActionName = "Save External IDs";
        public const string SaveSourceDataActionFlowName_MenuIcon = "SaveWebPartToDisk";

        public const string ExportMenuButtonName = "Export {0}";
        public const string ExternalIdSaveMapFormat = "External Id SM {0}";
        public const string ExternalIdSaveActionFormat = "Save External Id {0}";

    }

    public class DataMigrationModel : ICloneable<DataMigrationModel>
    {
        public string AppName { get; set; }
        public List<MigrationObject> MigrationObjects { get; set; }
        public MigrationActionFlows MigrationActionFlows { get; set; }
        public string ExportAllSuffix { get; set; }
        public string ExportSelectiveSuffix { get; set; }
        public Guid MasterExternalIdSaveMap { get; set; }

        public DataMigrationModel()
        {
            MigrationObjects = new List<MigrationObject>();
            MigrationActionFlows = new MigrationActionFlows();
        }

        public DataMigrationModel Clone()
        {
            return new DataMigrationModel
            {
                AppName = this.AppName,
                MigrationActionFlows = this.MigrationActionFlows.Clone(),
                MigrationObjects = this.MigrationObjects.ConvertAll<MigrationObject>(obj => obj.Clone()),
                ExportSelectiveSuffix = this.ExportSelectiveSuffix,
                ExportAllSuffix = this.ExportAllSuffix,
                MasterExternalIdSaveMap = this.MasterExternalIdSaveMap
            };
        }
    }

    public abstract class DependencyObject
    {
        [XmlIgnore]
        public HashSet<MigrationObject> Dependencies { get; set; }
    }

    public class MigrationObject : DependencyObject, ICloneable<MigrationObject>, IPartialClone<MigrationObject>
    {
        [XmlAttribute("Id")]
        public Guid Id { get; set; }
        public Guid AppObjectUniqueId { get; set; }
        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string SheetName { get; set; }
        public string ExternalID { get; set; }
        public string DataRetrievalActionSelectiveId { get; set; }
        public string DataRetrievalActionAllId { get; set; }
        public string DisplayActionId { get; set; }
        public Guid DisplayMapId { get; set; }
        public string SaveActionId { get; set; }
        public Guid SaveMapId { get; set; }
        public MigrationQuery DataRetrievalAction { get; set; }
        public List<MigrationField> Fields { get; set; }
        public int Sequence { get; set; }
        public Guid ExternalIdSaveMap { get; set; }
        public bool IsCloned { get; set; }
        public List<MigrationLookup> Lookup { get; set; }

        public bool CreateWorksheet { get; set; }

        public List<string> RemovedFilters { get; set; }

        public MigrationObject()
        {
            Fields = new List<MigrationField>();
            Dependencies = new HashSet<MigrationObject>();
            DataRetrievalAction = new MigrationQuery();
            Lookup = new List<MigrationLookup>();
            RemovedFilters = new List<string>();
        }

        public MigrationObject Clone()
        {
            return new MigrationObject()
            {
                Id = this.Id,
                ObjectId = this.ObjectId,
                ObjectName = this.ObjectName,
                Fields = this.Fields.ConvertAll<MigrationField>(field => field.Clone(Id)),
                Sequence = this.Sequence,
                AppObjectUniqueId = this.AppObjectUniqueId,
                DataRetrievalActionAllId = this.DataRetrievalActionAllId,
                DataRetrievalActionSelectiveId = this.DataRetrievalActionSelectiveId,
                DisplayActionId = this.DisplayActionId,
                SaveActionId = this.SaveActionId,
                DisplayMapId = this.DisplayMapId,
                SaveMapId = this.SaveMapId,
                SheetName = this.SheetName,
                ExternalID = this.ExternalID,
                DataRetrievalAction = this.DataRetrievalAction.Clone(),
                ExternalIdSaveMap = this.ExternalIdSaveMap,
                IsCloned = this.IsCloned,
                Lookup = this.Lookup.ConvertAll<MigrationLookup>(lookup => lookup.Clone()),
                CreateWorksheet = this.CreateWorksheet
            };
        }

        /// <summary>
        /// Partial Clone creates a copy of an object with a new MigrationObjectUniqueId.
        /// </summary>
        /// <returns></returns>
        public MigrationObject PartialClone()
        {
            MigrationObject clonedObject = new MigrationObject()
            {
                Id = Guid.NewGuid(),
                ObjectId = this.ObjectId,
                ObjectName = this.ObjectName,
                DataRetrievalAction = new MigrationQuery(),
                Sequence = this.Sequence,
                AppObjectUniqueId = this.AppObjectUniqueId,
                ExternalID = this.ExternalID,
                IsCloned = true
            };

            clonedObject.Fields = this.Fields.ConvertAll(field => field.Clone(clonedObject.Id));
            clonedObject.Lookup = this.Lookup.ConvertAll<MigrationLookup>(lookup => lookup.Clone());
            return clonedObject;
        }
    }


    public enum EditModeOperation
    {
        None,
        FieldAdded,
        FieldRemoved,
        ObjectAdded,
        ObjectRemoved,
        FilterChanged
    };

    public class MigrationField
    {
        public Guid MigrationObjectId { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public Datatype DataType { get; set; }
        public string SpecialLookupFormulaNameRange { get; set; }

        public MigrationField()
        {
        }

        public MigrationField Clone(Guid migrationObjectId)
        {
            return new MigrationField
            {
                MigrationObjectId = migrationObjectId,
                FieldId = this.FieldId,
                FieldName = this.FieldName,
                DataType = this.DataType,
                SpecialLookupFormulaNameRange = this.SpecialLookupFormulaNameRange,
            };
        }
    }

    public class MigrationQuery : ICloneable<MigrationQuery>
    {
        public ActionType Action { get; set; }
        public List<SearchFilterGroup> SearchFilter { get; set; }

        public MigrationQuery()
        {
            SearchFilter = new List<SearchFilterGroup>();
        }

        public MigrationQuery Clone()
        {
            return new MigrationQuery
            {
                Action = this.Action,
                SearchFilter = this.SearchFilter.ConvertAll<SearchFilterGroup>(group => group.Clone())
            };
        }
    }

    public class MigrationActionFlows : ICloneable<MigrationActionFlows>
    {
        public Guid SelectiveActionFlowId { get; set; }
        public Guid AllActionFlowId { get; set; }
        public Guid SaveActionFlowId { get; set; }
        public Guid AppInfoActionFlowId { get; set; }
        public Guid SaveSourceDataActionFlowId { get; set; }

        public MigrationActionFlows()
        {

        }
        public MigrationActionFlows Clone()
        {
            return new MigrationActionFlows
            {
                AllActionFlowId = this.AllActionFlowId,
                SelectiveActionFlowId = this.SelectiveActionFlowId,
                AppInfoActionFlowId = this.AppInfoActionFlowId,
                SaveActionFlowId = this.SaveActionFlowId,
                SaveSourceDataActionFlowId = this.SaveSourceDataActionFlowId
            };
        }
    }

    public class MigrationLookup : ICloneable<MigrationLookup>
    {
        public string LookupId { get; set; }
        public string LookupName { get; set; }
        public string LookupObjectId { get; set; }
        public Guid WorkflowInputMigrationObjectId { get; set; }

        public MigrationLookup Clone()
        {
            return new MigrationLookup
            {
                LookupId = this.LookupId,
                LookupName = this.LookupName,
                LookupObjectId = this.LookupObjectId,
                WorkflowInputMigrationObjectId = this.WorkflowInputMigrationObjectId
            };
        }
    }
}

