/*
/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{

    #region "Enums"

    public enum FormOpenMode
    {
        Create = 1,
        Edit = 2,
        ReadOnly = 3
    };

    // For any new DataType being added, Add case in UserControls/Filters.cs class to enable support in Expression Builder
    public enum Datatype
    {
        String = 1,
        Picklist = 2,
        Lookup = 3,
        Double = 4,
        DateTime = 5,
        Boolean = 6,
        Decimal = 7,
        NotSupported = 8,
        Textarea = 9,
        Picklist_MultiSelect = 10,
        Date = 11,
        Rich_Textarea = 12,
        Attachment = 13,
        Email = 14,
        Formula = 15,
        AnyType = 16,
        EmptyField = 17,
        Editable_Picklist = 18,
        Base64 = 19,
        Composite = 20
    };

    public enum PicklistType
    {
        None = 0,
        Regular = 1,
        Controlling = 2,
        Dependent = 3,
        TwoOption = 4
    }

    public enum CRM
    {
        Salesforce = 1,
        DynamicsCRM = 2,
        Oracle = 3,
        AIC = 4,
        AICV2 = 5
    };

    public enum SalesforceEndpoint
    {
        Login = 1,
        Test = 2
    };

    public enum ApplicationType
    {
        RepeatingCells = 1,
        CrossTab = 2
    }

    /// <summary>
    /// Define Enum
    /// </summary>
    public enum ObjectType
    {
        [Description("Individual")]
        Independent = 1,
        [Description("List")]
        Repeating = 2,
        [Description("CrossTab")]
        CrossTab = 3
    }

    public enum CrossTabObjectType
    {
        Row = 1,
        Col = 2,
        Data = 3
    }

    /// <summary>
    /// Define Enum for layout
    /// </summary>
    public enum LayoutType
    {
        [Description("Horizontal")]
        Horizontal = 1,
        [Description("Vertical")]
        Vertical = 2
    }

    /// <summary>
    /// Define Expression Builder Logical Operators
    /// </summary>
    public enum LogicalOperator
    {
        [Description("AND")]
        AND = 1,
        [Description("OR")]
        OR = 2
    }

    public enum QueryRelationshipType
    {
        Parent = 1,
        Lookup = 2,
        Child = 3,
        None = 4
    }

    public enum ListScreenType
    {
        [Description("Display Map")]
        DisplayMap = 1,
        [Description("Save Map")]
        SaveMap = 2,
        [Description("Action Flow")]
        Actionflow = 3,
        [Description("Matrix Map")]
        MatrixMap = 4
    }

    public enum QueryTypes
    {
        SELECT,
        INSERT,
        UPDATE,
        UPSERT,
        DELETE
    };

    public enum MapMode
    {
        RetrieveMap = 1,
        SaveMap = 2
    };

    public enum ExpressionValueTypes
    {
        [XmlEnum("0")]
        [Description("Input")]
        Input = 0,
        [XmlEnum("1")]
        [Description("Static")]
        Static = 1,
        [XmlEnum("2")]
        [Description("System Variables")]
        SystemVariables = 2,
        [XmlEnum("3")]
        [Description("User Input")]
        UserInput = 3,
        [XmlEnum("4")]
        [Description("Cell Reference")]
        CellReference = 4
    };

    public enum ExpressionSystemVariables
    {
        CurrentUser = 0,
        CurrentDate = 1,
        ExportRecordId = 2
    };

    public enum EntityType
    {
        DisplayMap,
        SaveMap,
        Actions,
        Workflow,
        Menu,
        ApplicationDefnition,
        MatrixMap,
        DataSource
    };

    public enum ApplicationMode
    {
        Designer = 0,
        Runtime = 1
    };

    public enum RuntimeMode
    {
        AddIn = 0,
        Batch = 1
    };

    public enum SaveType
    {
        [Description("None")]
        None,
        [Description("Display")]
        RetrievedField,
        [Description("SaveOnly")]
        SaveOnlyField,
        [Description("Matrix")]
        MatrixField
    };

    public enum FileSaveType
    {
        Overwrite,
        AddNew
    }

    public enum AttachmentFormat
    {
        Excel = 0,
        PDF = 1
    }

    public enum ChangeItemType
    {
        Picklist = 0,
        DependentPicklist = 1,
        RecordType = 2
    };

    [Serializable]
    public enum GroupingLayout
    {
        Top,
        Bottom
    }

    public enum ValueSeparator
    {
        CommaSeparated,
        MultiLine
    }

    public enum StringOperations
    {
        Default,
        StartWith,
        Contains
    }

    public enum AppEdition
    {
        None,
        Basic,
        Advanced
    }

    [Serializable]
    public enum AddRowLocation
    {
        Top,
        Bottom
    }

    [Serializable]
    public enum AddRowInputType
    {
        ActionFlowStepInput,
        CellReference,
        StaticInput,
        UserInput
    }

    [Serializable]
    public enum PasteType
    {
        PasteValues,
        PasteAll
    }

    [Serializable]
    public enum RepeatingGroupSortDirection
    {
        Ascending,
        Descending
    }

    [Serializable]
    public enum CellReferenceType
    {
        CellLocation,
        NamedRange
    }

    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Information
    }
    public enum RecordTypeChange
    {
        RecordTypeDisplayNameChanged,
        RecordTypeAdded,
        RecordTypeDeleted,
        PicklistValuesModified,
        ObjectHasNoRecordTypes
    }

    #endregion

    public static class Constants
    {
        public const string PRODUCT_NAME = "X-Author for Excel";
        public const string PRODUCT_NAME_DCRM = "X-Author for Excel_DCRM";
        public const string PRODUCT_NAME_AIC = "X-Author for Excel_AIC";

        public const string PRODUCT_NAME_CONFIG = "X-Author for Excel.config";
        public const string PRODUCT_NAME_DCRM_CONFIG = "X-Author for Excel_DCRM.config";
        public const string PRODUCT_NAME_AIC_CONFIG = "X-Author for Excel_AIC.config";
        public const string COMMON_CONFIG_FILE = "X-Author for Excel Common.config";
        public const string PRODUCT_NAME_AICV2 = "X-Author for Excel_AICV2";
        public const string ACTIVE_CRM_CONFIG_FILE = "ActiveCRM.config";

        public static string CURRENT_USER_ID = string.Empty;
        public static string NAMESPACE_PREFIX = string.Empty;
        public const string DESIGNER_PRODUCT_NAME = "X-Author Designer";
        public const string RUNTIME_PRODUCT_NAME = "X-Author for Excel";
        public const string BATCH_PRODUCT_NAME = "X-Author Batch";
        public const string METADATAEXPLORER_PRODUCT_NAME = "X-Author Metadata Explorer";

        public const string BASIC_EDITION = "Essentials";
        public const string ADMIN_EDITION = "Power Admin";
        public const string ENTERPRISE_EDITION = "Enterprise";
        //public const int BASIC_EDITION_NO_APPS = 3;
        // TODO :: While creating Build for Basic Version uncomment below line
        //public static string PRODUCT_EDITION = BASIC_EDITION;
        public static string PRODUCT_EDITION = string.Empty;

        public const string XML_VERSION = "1.0";
        public const string XML_ENCODING = "UTF-8";

        public const string ROOT_ELEMENT = "App";
        public const string DEFINITION_TAG = "Definition";
        public const string CLIENTS_TAG = "Clients";
        public const string CLIENT_TAG = "Client";
        public const string LAYOUT_TAG = "Layout";
        public const string FILE_TAG = "File";
        public const string FILENAME_TAG = "FileName";
        public const string MENUS_TAG = "Menus";
        public const string MENUGROUPS_TAG = "MenuGroups";
        public const string MENUITEMS_TAG = "MenuItems";
        public const string ACTIONS_TAG = "Actions";
        public const string WORKFLOWS_TAG = "Workflows";
        public const string RETRIEVEMAPS_TAG = "DisplayMaps";
        public const string SAVEMAPS_TAG = "SaveMaps";
        public const string TARGETOBJECT_TAG = "TargetObject";
        public const string ACTION_TAG = "Action";
        public const string RECORDTYPE_TAG = "RecordType";
        public const string SEARCHFIELDS_TAG = "SearchFields";
        public const string SEARCHFIELD_TAG = "SearchField";
        public const string LABEL_TAG = "Label";
        public const string DATATYPE_TAG = "DataType";
        public const string DEFAULTVALUE_TAG = "DefaultValue";
        public const string SEQUENCENO_TAG = "SequenceNo";
        public const string RESULTFIELDS_TAG = "ResultFields";
        public const string RESULTFIELD_TAG = "ResultField";
        public const string SORTFIELD_TAG = "IsSortField";

        // Action Types
        public const string SEARCH_AND_SELECT_ACTION = "SearchAndSelect";
        public const string FORM_ACTION = "Form";
        public const string EXECUTE_QUERY_ACTION = "Query";
        public const string DELETE_ACTION = "Delete";
        public const string CALL_PROCEDURE_ACTION = "SalesforceMethod";
        public const string SAVE_ACTION = "Save";
        public const string RETRIEVE_ACTION = "Display";
        public const string MACRO_ACTION = "Macro";
        public const string SAVE_ATTACHMENT_ACTION = "SaveAttachment";
        public const string CLEAR_ACTION = "Clear";
        public const string CHECK_IN_ACTION = "CheckIn";
        public const string CHECK_OUT_ACTION = "CheckOut";
        public const string INPUT_ACTION = "InputAction";
        public const string PASTESOURCEDATA_ACTION = "PasteSourceDataAction";
        public const string PASTEROW_ACTION = "PasteRow";
        public const string SWITCH_CONNECTION_ACTION = "SwitchConnection";
        public const string ADDROW_ACTION = "AddRow";
        public const string DataSetAction = "DataSetAction";
        public const string EXTERNAL_ACTION = "ExternalAction";
        public static AppEdition APP_EDITION = AppEdition.None;

        // Map Attributes
        public const string DISPLAYMAP_NAME = "Display Map";
        public const string SAVEMAP_NAME = "Save Map";
        public const string MATRIXMAP_NAME = "Matrix Map";
        public const string SAVEMAP_PRELOADEDLIST_DISPLAY = "NamePlusLoadedRow";
        public const string SAVEMAP_PRELOADEDLIST_VALUE = "SaveGroupId";

        // Attributes
        public const string ID_ATTRIBUTE = "Id";
        public const string UNIQUEID_ATTRIBUTE = "UniqueId";
        public const string NAME_ATTRIBUTE = "Name";
        public const string NAME_ATTRIBUTE_WITH_TYPE = "Name";
        public const string EXCEL_APP_NAME = "Excel";
        public const string ACTION_TYPE_ATTRIBUTE = "Type";
        public const string PAGESIZE_ATTRIBUTE = "PageSize";
        public const string APPENDLOOKUPID = ".Name";
        public const string APPENDLOOKUPNAME = " (Name)";
        public const string APPENDLOOKUPNAME_WITH_PARENTHESIS = " (Name)";
        public const string CustomObjectReference__c = "__c";
        public const string CustomObjectReference__r = "__r";
        public const string LOOKUPDATASETEXTENSION = "-Master";
        public const string CELL_LOCATION = "CellLocation";
        public const string FieldId = "FieldId";

        // APPLICATION
        public const string ATTACHMENT = "Attachment";
        public const string ATTACHMENT_SUBQUERY = "(Select ParentId,Id,Name,Parent.Name from Attachments Order By LastModifiedDate DESC)";
        public static List<string> OBJECTSEXCLUDEDFROMATTACHMENT = new List<string> { "Attachment", "Document" };
        public const string ATTACHMENTOBJECT = "Attachment";
        public const string ATTACHMENTOBJECT_DYNAMICS = "annotation";
        public const string ATTACHMENTOBJECT_BODY = "Body";
        public const string ATTACHMENTOBJECT_PARENTID = "ParentId";
        public const string ATTACHMENTOBJECT_AIC = "cmn_Attachment";
        public const string APPLICATIONOBJECT = "Application__c";
        public const string APPLICATIONOBJECT_UNIQUEID = "UniqueId__c";
        public const string REPEATING_CELLS = "Repeating Cells";
        public const string CROSS_TAB = "Cross Tab";
        public const string METADATA_SHEETNAME = "apttusmetadata";
        public const string HISTORY_OBJECT_SUFFIX = "History";
        //public static List<string> ObjectsWithoutLookupName = new List<string> { "Case", "CaseComment", "CaseContactRole", "CaseShare", "CaseSolution" };
        public static List<string> ObjectsWithoutLookupName = new List<string>();
        // DOCUMENT OBJECT CONSTANTS
        public const string DOCUMENT_OBJECTID = "Document";
        public const string DOCUMENT_BODY = "Body";
        public const string DOCUMENT_TYPE = "Type";
        public const string DOCUMENT_TYPE_FIELD_NAME = "File Extension";
        public const string DOCUMENT_NAME_FIELD_NAME = "Document Name";

        // ACTION CONSTANTS
        public const int CALL_PROCEDURE_MAX_ROWS = 7;
        public const int EXPRESSION_BUILDER_MAX_ROWS = 20;
        public const int DECISION_EDITOR_MAX_ROWS = 20;
        public const int CRITERIA_EDITOR_MAX_ROWS = 20;
        public const int EXPRESSION_BUILDER_MAX_ROWS_SF_BASIC_EDITION = 3;
        public const string SHEET_DELIMITER = "!";
        public const int SAVE_ACTION_BATCH_SIZE = 200;

        // Expression Builder
        public const string WHERE_CLAUSE = "WHERE";
        public const string SPACE = " ";
        public const string NOSPACE = "";
        public const string OPEN_BRACKET = "(";
        public const string CLOSE_BRACKET = ")";
        public const string DOT = ".";
        public const string EQUALS = "=";
        public const string QUOTE = "'";
        public const string COMMA = ",";
        public const string COLON = ":";
        public const string SEMICOLON = ";";
        public const string PERCENT = "%";
        public const string UNDERSCORE = "_";
        public const string AIC_ALIAS_SEPARATOR = "##";
        public const string HYPHEN = "-";
        public const string AMPERSAND = "&";
        public const string AND = "AND";
        public const string OR = "OR";
        public const string BREADCRUMB_DELIMITER = ">";
        public const string ATTACHMENTSEPARATOR = "__";
        public const int MAX_RECORDID_COUNT_BEFORE_CHUNKING = 750;
        public const string CHUNK_PLACEHOLDER = "<>CHUNK_PLACEHOLDER<>";

        // Matrix 
        public const string MATRIX_ROWID = "MatrixRowID";
        public const string MATRIX_COLUMNID = "MatrixColumnID";
        public const string MATRIX_DATAFIELDID = "MatrixDataFieldID";
        public const string MATRIX_DATAROWINDEX = "MatrixDataRowIndex";
        public const string MATRIX_DATAROWVALUE = "MatrixDataRowValue";
        public const string MATRIX_DATACOLINDEX = "MatrixDataColIndex";
        public const string MATRIX_DATACOLVALUE = "MatrixDataColValue";
        public const string GROUPEDCOLUMNFILTER = "GroupedColumnFilter";
        // General
        public const string XLPROTECTSHEET_PASSWORD = "apttuscrm1";
        public const string APPBUILDER_DESIGNER_FILE = "APPBUILDER_DESIGNER_SAVE";
        public const string APPBUILDER_RUNTIME_FILE = "APPBUILDER_RUNTIME_FILE";
        public const string APPBUILDER_RUNTIME_ATTACHMENTFILE = "APPBUILDER_RUNTIME_ATTACHMENTFILE";
        public const string APPBUILDER_CHECKOUT_FILE_ID = "APPBUILDER_CHECKOUT_FILE_ID";
        public const string XLSX = ".xlsx";
        public const string XLS = ".xls";
        public const string XLSM = ".xlsm";
        public const string XML = ".xml";
        public static string TEMPLATENAME = "Template";
        public const string XLSXWITHOUTDOT = "xlsx";
        public const string XLSMWITHOUTDOT = "xlsm";
        public const string APTTUSAPPBUILDERLOG = "X-Author for Excel.log";
        public const string APTTUSAPPBUILDERCLIENTLOG = "X-Author for Excel Client.log";
        public const string APTTUSAPPBUILDERSAVEMESSAGELOG = "X-Author for Excel Save Messages.log";
        public const string DESIGNER_LOG_NAME = "X-Author Designer Logs";
        public const string RUNTIME_LOG_NAME = "X-Author Runtime Logs";
        public const string APTTUSAPPBUILDERSERVICELOG = "X-Author for Excel Service.log";
        public const string SERVICE_LOG_NAME = "X-Author Service Logs";
        public const string XAUTHORBATCH_LOG_NAME = "X-Author Batch Logs";
        public const string XATTACHMENTPATH = "X-Author Attachments";
        public const string LogOn = "On";
        public const string LogOff = "Off";
        public const int STARTCOLUMN_PICKLISTDATA = 27;
        public const int STARTCOLUMN_PICKLISTKEYVALUEPAIR = 25;
        public const string NAMEDRANGE_PICKLISTKEYVALUEPAIR = "PICKLISTKEYVALUEPAIR";
        public const string INVALIDPICKLISTDATA_RANGENAME = "XAE_Invalid_PLData";
        public const string INVALIDPICKLISTDATA_TEXT = "     ";//This is kept intentionally for picklist validation to get fired. See AB-1617.
        public const string RECORDTYPE_OBJECTID = "RecordType";
        public static List<string> INVALIDEXCELFORMULATEXT = new List<string> { "#NAME", "#REF", "#DIV", "#N/A", "#NULL", "#NUM", "#VALUE" };
        public static string COLUMNNAME_PREFIX = "Column";
        public const int EXPRESSION_BUILDER_LABELHOVERTIMEOUT = 200;
        public const int EXPRESSION_BUILDER_LABELMAXLENGTH = 25;
        public const string RICH_TEXT_TASKPANE_NAME = "Rich Text Editor";
        public const string RICH_TEXT_VF_PAGE_NAME = "RichTextEditor";
        // This tag is used by license manager module to determine, Richtext is enabled or not
        public const string RICH_TEXT_EDIT_TAG = "RichTextEdit";
        public const string CHATTER_IN_EXCEL_TAG = "ChatterInExcel";
        public const string CPQ_API_ACTIONFLOW_TAG = "CPQAPI";

        public const string RICHTEXT_FORMULA_EDIT = "=IF(1=2,\"{0}\", HYPERLINK(\"\", \"Click here to Edit\"))";
        public const string RICHTEXT_FORMULA_ADD = "=IF(1=2,\"{0}\", HYPERLINK(\"\", \"Click here to Add\"))";
        public const string RICHTEXT_FORMULA_VIEW = "=IF(1=2,\"{0}\", HYPERLINK(\"\", \"Click here to View\"))";
        //Appending Guid here because this name has to be unique as any name given here shouldn't conflict with the names provided by user input in workflow inputvariables.
        public static string USERINPUT_DATASETNAME = "InputActionVariables_08E30521_312A_4EF4_BB80_03648D33A063";
        public static string ATTACHMENTDATA_DATASETNAME = "ATTACHMENTDATA_08E30521_312A_4EF4_BB80_03648D33A063";
        public static string ATTACHMENTDELETEDATA_DATASETNAME = "ATTACHMENTDELETEDATA_08E30521_312A_4EF4_BB80_03648D33A063";
        public static string KEY_COLUMN = "Key";
        public static string VALUE_COLUMN = "Value";
        public static string Display_COLUMN = "Display";

        public static string GROUPING_ROW_ERRORMSG_ONLY = "Donottouch";
        public static string GROUPING_UNIQUE_SEPARATOR = "#^|~";
        public static string GROUPING_ROW_ERRORMSG_WITHFIELDID = GROUPING_ROW_ERRORMSG_ONLY + GROUPING_UNIQUE_SEPARATOR + "{0}";
        public const string QUERYRESULT_SINGLE = "Single";
        public const string QUERYRESULT_MULTIPLE = "Multiple";
        public const int QUERYRESULT_RECORDLIMIT = 500;
        public const double VERSION_TILL_DESIGNER_FLAG_IN_TEMPLATE = 3.5;

        public const string EXCEL_DEFAULTNUMBERFORMAT = "General";
        public const int EXCEL_NUMBEROFCOLUMNS = 16384;
        public const string EXCEL_CONTEXT_ABOUTDESIGNER = "About " + DESIGNER_PRODUCT_NAME;
        public const string EXCEL_CONTEXT_ABOUTRUNTIME = "About " + RUNTIME_PRODUCT_NAME;
        public const string EXCEL_CONTEXT_OPTIONS = "Options";
        public const string EXCEL_CONTEXT_VIEWDESIGNERLOG = "Product Error Log";
        public const string EXCEL_CONTEXT_VIEWRUNTIMELOG = "Product Error Log";
        public const string EXCEL_CONTEXT_VIEWRUNTIMESAVEMESSAGES = "Save Message Log";
        public const string EXCEL_CONTEXT_CLEAR = "Clear";
        public const string EXCEL_CONTEXT_CLEARLOGS = "Clear Product Error Log";
        public const string EXCEL_CONTEXT_CLEARSAVEMESSAGES = "Clear Save Message Log";
        public const string PASTESOURCEDATA_READY_FILEPATH = "{0}X-Author for Excel\\Paste From Mapping - Ready\\";
        public const string PASTESOURCEDATA_COMPLETE_FILEPATH = "{0}X-Author for Excel\\Paste From Mapping - Complete\\";
        public const string PASTESOURCEDATA_READY_FOLDER = "Paste From Mapping - Ready";
        public const string PASTESOURCEDATA_COMPLETE_FOLDER = "Paste From Mapping - Complete";
        public const string ACTIONSAVEMESSAGE = "Action Saved Successfully.";
        public const string ACTIONSCAPTIONMESSAGE = "Save";
        public const string MATRIX_ISROWVALUE = "IsMatrixRowValue";
        public const string MATRIX_ISCOLUMNVALUE = "IsMatrixColumnValue";       

        public const string CELLREFERENCE = "CellReference";
        public const string FORMULATYPESTRING = "string";
        public const string FORMULATYPEDOUBLE = "double";
        public const string FORMULATYPECURRENCY = "currency";
        public const string FORMULATYPEBOOLEAN = "boolean";
        public const string FORMULATYPEPERCENT = "percent";
        public const string FORMULATYPEDATE = "date";


        public const string RESOURCE_ENGLISH = "English";
        public const string RESOURCE_JAPANESE = "Japanese";

        //What's New
        public const string RESOURCE_PACK_URI = @"pack://application:,,,/Apttus.XAuthor.Core;Component/Resources/";
        public const int WHATSNEW_AUTOTHRESHOLD_COUNT = 3;
        public const int WHATSNEW_AUTODELAYSECONDS = 3;
        public const int WHATSNEW_FEATUREAUTODELAYSECONDS = 1;

        public static List<Datatype> DatatypesWithExplicitColumnDatatypes
        {
            get
            {
                return new List<Datatype>() { Datatype.Decimal, Datatype.Double, Datatype.DateTime };
            }
        }

        // Error Handling
        public const int BALLOON_TIMEOUT = 1000;
        public const string ERRORMESSAGE_WINDOWTITLE = "X-Author Error Message";
        public const string SF_CLIENT_EXCEPTION = "Client";
        public const string SF_INVALID_SESSION_ID = "INVALID_SESSION_ID";
        public const string SF_INVALID_LOGIN = "INVALID_LOGIN";
        public const string MESSAGE_EDITMODE = "X-Author detected that an Excel cell has focus and is being edited. Please exit editing the Excel cell before continuing with this action.";

        // Batch
        public const string BATCH_EXE = "Apttus.XAuthor.Batch.exe";
        public const string XML_DATASETNAME = "DataSetName";
        public const string JSON = ".json";
        public const string EXTERNALSCHEMA = "ExternalSchema";

        // Check in Check Out constants
        public const string APP_FILE = "AppFile__c";
        public const string APP_FILE_FileId__c = "FileId__c";
        public const string APP_FILE_FileType__c = "FileType__c";
        public const string APP_FILE_IsLocked__c = "IsLocked__c";
        public const string APP_FILE_ObjectName__c = "ObjectName__c";
        public const string APP_FILE_ParentId__c = "ParentId__c";
        public const string APP_FILE_LastModifiedBy = "LastModifiedBy";

        // Registry
        public const string ApttusRegistryBase = "Software\\Apttus\\ApttusAppBuilder\\Logins";
        public const string ServerHostKey = "ServerHost";

        // LMA 
        public const string LMA_LICENSE_DETAIL = "lmadetail.xml";
        public const string LMA_FEATURE_DETAIL = "lmafeaturedetail.xml";
        public const string DESIGNER_DLL_NAME = "Apttus.XAuthor.AppDesigner.dll";
        public const string RUNTIME_DLL_NAME = "Apttus.XAuthor.AppRuntime.dll";

        //Chunking
        public static int MAX_RECORDS_WHILE_CHUNKING = 750;

        //Emtpy Column in Delete Action
        public static string EMPTYCOLUMNFIELDIDFORMAT = "Column{0}";

        //Display Map Lookup Recursion Limit
        public const int HIERARCHY_RECURSION_COUNTER = 10;
        #region "SOQL"

        public static string CALLPROCEDURE_SELECT = "Select Id, Name From ApexClass where Status = 'Active'";

        //public static string APPLICATIONBROWSER_FIRSTPART = "SELECT Id, Name, Owner.Name, LastModifiedDate FROM " + Constants.NAMESPACE_PREFIX + "Application__c WHERE Name LIKE";
        //public static string APPLICATIONBROWSER_SECONDPART = "ORDER BY LastModifiedDate DESC LIMIT 100";
        //public static string ANDCONDITION = "and Owner.ID =";
        //public static string APP_UNIQUENAMEANDID_CHECK_FIRSTPART = "SELECT Name, " + Constants.NAMESPACE_PREFIX + "UniqueId__c FROM " + Constants.NAMESPACE_PREFIX + "Application__c WHERE Name = ";
        //public static string APP_UNIQUENAMEANDID_CHECK_SECONDPART = " or " + Constants.NAMESPACE_PREFIX + "UniqueId__c = ";
        //public static string APP_FETCHAPPBYUNIQUEID = "SELECT Id, Name FROM " + Constants.NAMESPACE_PREFIX + "Application__c WHERE " + Constants.NAMESPACE_PREFIX + "UniqueId__c = ";
        public static string APP_FETCHCONFIG = "SELECT Id, Body FROM Attachment WHERE Name = 'AppDefinition.xml' AND ParentId = ";
        public static string APP_FETCHTEMPLATE = "SELECT Id, Body FROM Attachment WHERE Name LIKE 'Template%' AND ParentId = ";
        public static int SOQL_MAX_LENGTH = 20000;
        public static int EXCEL_NUMBEROFROWS = 1048576;
        public static double EXCEL_MAJOR_VERSION_DATA_VALIDATION_ISSUE = 16;
        public static double EXCEL_MINOR_VERSION_DATA_VALIDATION_ISSUE_START = 10827;
        public static double EXCEL_MINOR_VERSION_DATA_VALIDATION_ISSUE_END = 11900;
        public static int EXCEL_DATA_VALIDATION_ISSUE_LIMIT = 255;

        public static string AppDefDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

                ////sb.Append("Apttus X-Author for Excel delivers interaction \n").Append("between Microsoft Excel, Chatter, and Salesforce. \n")
                ////  .Append("Configurable ribbons embedded in Microsoft Excel \n").Append("enable intelligent interaction with Salesforce.  \n")
                ////  .Append("The technology allows for activity in a Microsoft Excel \n").Append("document to be shared and fully recorded inside any  \n")
                ////  .Append("Structured Data System including Salesforce. \n\n")
                ////  .Append("Microsoft Excel users do not need to go into Salesforce or \n")
                ////  .Append("Chatter to perform any tasks.  All tasks, including updating Salesforce data and")
                ////  .Append(" viewing a Chatter feed can be done from inside the Microsoft Excel document. ");
                sb.Append(resourceManager.GetResource("ABOUT_lblAboutDescription_Text"));
                return sb.ToString();
            }
        }

        #endregion

        public const int EXCEL_CELL_LENGTH = 32767;
        public static bool? ISADMINPACKAGEINSTALLED = null;
        public static int WHERECLAUSE_PLUS_COMMACHARS_LENGTH = 13500;
        public static int ROWS_PER_COLUMN_DEFAULTVALUE = 5;
        public static int BLANK_COLUMNS_DEFAULT_VALUE = 1;
        public const int FIELD_SEARCH_SELECTALL_THRESHOLD_VALUE = 20;
        public static int[] RowsPerColumnList
        {
            get
            {
                return Enumerable.Range(1, 20).ToArray();
            }
        }

        public static int[] BlankColumnList
        {
            get
            {
                return Enumerable.Range(0, 4).ToArray();
            }
        }
    }
}
