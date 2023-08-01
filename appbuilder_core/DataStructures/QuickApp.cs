using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public enum ActionType
    {
        None,
        SearchAndSelect,
        Form,
        SalesforceMethod,
        Query,
        Save,
        Display,
        Macro,
        SaveAttachment,
        Clear,
        CheckIn,
        CheckOut,
        InputAction,
        PasteSourceDataAction,
        SwitchConnection,
        AddRow,
        PasteRowAction,
        DataSetAction,
        Delete
    }

    [Serializable]
    public enum QuickAppType
    {
        ListApp = 0,
        ParentChildApp
    }

    [Serializable]
    public enum RelationalObject
    {
        ParentObject,
        ChildObject
    }

    public enum MapType
    {
        DisplayMap = 0,
        SaveOnlyMap,
    }

    public enum QuickAppErrorCode
    {
        Success,
        NoAppTypeSelected,
        NoObjectSelected,
        IncorrectOrdering,
        NoDisplayFieldSelected,
        NoSearchFieldSelected,
        NoResultFieldSelected,
        UnsupportedFieldTypeSelected
    }

    public class QuickAppConstants
    {
        public const int RepeatingGroupStartRow = 4;
        public const int RepeatingGroupStartCol = 1;
        public const int IndependentFieldStartRow = 3;
        public const int IndependentFieldStartColumn = 2;
        public const int NextSuccessiveIndependentFieldStartColumnDiff = 3;
        public const int MaximumSequentialIndependentFields = 5;
        public const string DisplayMapName = "Main";
        public const string SaveMapName = "Main";
        public const string DisplayActionName = "Display";
        public const string SaveActionName = "Save";
        public const string ActionDisplayMember = "ActionTypeValue";
        public const string RetrieveMenuName = "Retrieve";
        public const string SaveMenuName = "Save";
        public const string RetrieveWorkflowName = "RetrieveWorkflow";
        public const string RetrieveWorkflowEditInExcelName = "RetrieveWorkflow_LaunchFromSF";
        public const string SaveWorkflowName = "SaveWorkflow";
        public const string RetrieveMenuIcon = "GetExternalDataFromOtherSources";
        public const string SaveMenuIcon = "SaveObjectAs";
    }

    [Serializable]
    public class ActionSelectionFilter
    {
        public QuickAppType AppType { get; set; }
        public RelationalObject ObjectType { get; set; }
        public ActionType ActionType { get; set; }
        public string ActionTypeValue { get; set; }

        public ActionSelectionFilter()
        {

        }

        public ActionSelectionFilter(QuickAppType appType, RelationalObject objectType, ActionType actionType, string actionTypeValue)
        {
            AppType = appType;
            ActionType = actionType;
            ObjectType = objectType;
            ActionTypeValue = actionTypeValue;
        }

        public static List<ActionSelectionFilter> GetSupportedRecordSelection()
        {
            List<ActionSelectionFilter> filters = new List<ActionSelectionFilter>();
            filters.Add(new ActionSelectionFilter(QuickAppType.ListApp, RelationalObject.ParentObject, ActionType.Query, "Query"));
            filters.Add(new ActionSelectionFilter(QuickAppType.ListApp, RelationalObject.ParentObject, ActionType.SearchAndSelect, "User Selection"));
            filters.Add(new ActionSelectionFilter(QuickAppType.ParentChildApp, RelationalObject.ParentObject, ActionType.SearchAndSelect, "User Selection"));
            filters.Add(new ActionSelectionFilter(QuickAppType.ParentChildApp, RelationalObject.ChildObject, ActionType.Query, "Query"));
            filters.Add(new ActionSelectionFilter(QuickAppType.ParentChildApp, RelationalObject.ChildObject, ActionType.SearchAndSelect, "User Selection"));

            return filters;
        }
    }

    [Serializable]
    public class QuickAppFieldAttribute
    {
        public QuickAppFieldAttribute()
        {
        }

        public string ObjectId { get; set; }
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string DisplayOrder { get; set; }
        public bool Display { get; set; }
        public bool Save { get; set; }
        public bool SearchFields { get; set; }
        public bool ResultFields { get; set; }
        public RelationalObject FieldRelation { get; set; }
    }      

    [Serializable]
    public class ExpressionBuilderModel
    {
        public ExpressionBuilderModel()
        {
            Filters = new List<SearchFilterGroup>();
        }
        public RelationalObject RelationObject { get; set; }
        public ApttusObject Object { get; set; }
        public List<SearchFilterGroup> Filters { get; set; }
    }

    [Serializable]
    public class WizardModel
    {
        public string AppName { get; set; }

        public QuickAppType AppType { get; set; }

        public ApttusObject Object { get; set; }
        
        public List<QuickAppFieldAttribute> DisplayFields { get; set; } //All fields including parent and child will be stored in the list. The distinction between parent fields and child fields is based on object id.

        public List<ActionSelectionFilter> Actions { get; set; }

        public List<ExpressionBuilderModel> Filters { get; set; }

        public string WorksheetTitle { get; set; }
        public int MaxColumnWidth { get; set; }
        public string MenuGroupName { get; set; }
        public string DisplayMenuButtonName { get; set; }
        public string SaveMenuButtonName { get; set; }

        public bool AllowAddRow { get; set; }
        public bool AllowDeleteRow { get; set; }
        public bool AllowEditInExcel { get; set; }
        public bool AllowSaveForSheets { get; set; }
        public bool ShowGridLines { get; set; }
        public bool ShowAutoFilter { get; set; }

        public WizardModel()
        {
            WorksheetTitle = string.Empty;
            DisplayFields = new List<QuickAppFieldAttribute>();
            Actions = new List<ActionSelectionFilter>();
            Filters = new List<ExpressionBuilderModel>();
        }

        [XmlIgnore]
        public bool IsParentDirty { get; set; }
        [XmlIgnore]
        public bool IsChildDirty { get; set; }
    }
}
