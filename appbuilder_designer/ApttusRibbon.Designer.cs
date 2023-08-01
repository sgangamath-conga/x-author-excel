namespace Apttus.XAuthor.AppDesigner
{
    partial class ApttusRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ApttusRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();

            //LoadUserConnections();

            //LoadApplications();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabAppBuilderDesigner = this.Factory.CreateRibbonTab();
            this.ApplicationGroup = this.Factory.CreateRibbonGroup();
            this.mnuApplication = this.Factory.CreateRibbonMenu();
            this.groupWizard = this.Factory.CreateRibbonGroup();
            this.splitBtnQuickApp = this.Factory.CreateRibbonSplitButton();
            this.btnQuickAppNew = this.Factory.CreateRibbonButton();
            this.btnQuickAppEdit = this.Factory.CreateRibbonButton();
            this.btnQuickAppSettings = this.Factory.CreateRibbonButton();
            this.btnDataMigration = this.Factory.CreateRibbonSplitButton();
            this.btnNewDataMigrationApp = this.Factory.CreateRibbonButton();
            this.btnEditDataMirationApp = this.Factory.CreateRibbonButton();
            this.groupBuild = this.Factory.CreateRibbonGroup();
            this.btnAppDef = this.Factory.CreateRibbonButton();
            this.btnRetrieveMap = this.Factory.CreateRibbonButton();
            this.btnMatrixMap = this.Factory.CreateRibbonButton();
            this.btnSaveMap = this.Factory.CreateRibbonButton();
            this.splitBtnActions = this.Factory.CreateRibbonSplitButton();
            this.btnAddRowAction = this.Factory.CreateRibbonButton();
            this.btnCheckIn = this.Factory.CreateRibbonButton();
            this.btnCheckOut = this.Factory.CreateRibbonButton();
            this.btnClearAction = this.Factory.CreateRibbonButton();
            this.btnDataSet = this.Factory.CreateRibbonButton();
            this.btnDelete = this.Factory.CreateRibbonButton();
            this.btnRetrieve = this.Factory.CreateRibbonButton();
            this.btnMacroAction = this.Factory.CreateRibbonButton();
            this.btnPasteRow = this.Factory.CreateRibbonButton();
            this.btnpasteSourceData = this.Factory.CreateRibbonButton();
            this.btnExecuteQuery = this.Factory.CreateRibbonButton();
            this.btnCallProcedure = this.Factory.CreateRibbonButton();
            this.btnSaveAction = this.Factory.CreateRibbonButton();
            this.btnSaveAttachmentAction = this.Factory.CreateRibbonButton();
            this.btnSearchSelect = this.Factory.CreateRibbonButton();
            this.btnSwitchConnection = this.Factory.CreateRibbonButton();
            this.btnUserInput = this.Factory.CreateRibbonButton();
            this.btnForm = this.Factory.CreateRibbonButton();
            this.btnWorkflow = this.Factory.CreateRibbonButton();
            this.btnMenuDesigner = this.Factory.CreateRibbonButton();
            this.groupSaveApp = this.Factory.CreateRibbonGroup();
            this.splitbtnSave = this.Factory.CreateRibbonSplitButton();
            this.btnSaveAs = this.Factory.CreateRibbonButton();
            this.grpAppSettings = this.Factory.CreateRibbonGroup();
            this.btnAppSetting = this.Factory.CreateRibbonButton();
            this.groupMigrate = this.Factory.CreateRibbonGroup();
            this.btnValidate = this.Factory.CreateRibbonButton();
            this.btnDataLoader = this.Factory.CreateRibbonButton();
            this.btnExternalLibrary = this.Factory.CreateRibbonButton();
            this.btnImport = this.Factory.CreateRibbonButton();
            this.btnExport = this.Factory.CreateRibbonButton();
            this.groupPreview = this.Factory.CreateRibbonGroup();
            this.btnPreview = this.Factory.CreateRibbonButton();
            this.AccessGroup = this.Factory.CreateRibbonGroup();
            this.btnConnect = this.Factory.CreateRibbonButton();
            this.mnuSwitchConnection = this.Factory.CreateRibbonMenu();
            this.btnGallerySupport = this.Factory.CreateRibbonMenu();
            this.btnGeneral = this.Factory.CreateRibbonButton();
            this.btnSetCRM = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.btnAbout = this.Factory.CreateRibbonButton();
            this.apttusNotification = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnCentralAdmin = this.Factory.CreateRibbonButton();
            this.tabAppBuilderDesigner.SuspendLayout();
            this.ApplicationGroup.SuspendLayout();
            this.groupWizard.SuspendLayout();
            this.groupBuild.SuspendLayout();
            this.groupSaveApp.SuspendLayout();
            this.grpAppSettings.SuspendLayout();
            this.groupMigrate.SuspendLayout();
            this.groupPreview.SuspendLayout();
            this.AccessGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabAppBuilderDesigner
            // 
            this.tabAppBuilderDesigner.Groups.Add(this.ApplicationGroup);
            this.tabAppBuilderDesigner.Groups.Add(this.groupWizard);
            this.tabAppBuilderDesigner.Groups.Add(this.groupBuild);
            this.tabAppBuilderDesigner.Groups.Add(this.groupSaveApp);
            this.tabAppBuilderDesigner.Groups.Add(this.grpAppSettings);
            this.tabAppBuilderDesigner.Groups.Add(this.groupMigrate);
            this.tabAppBuilderDesigner.Groups.Add(this.groupPreview);
            this.tabAppBuilderDesigner.Groups.Add(this.AccessGroup);
            this.tabAppBuilderDesigner.KeyTip = "XAD";
            this.tabAppBuilderDesigner.Label = "X-Author Designer";
            this.tabAppBuilderDesigner.Name = "tabAppBuilderDesigner";
            // 
            // ApplicationGroup
            // 
            this.ApplicationGroup.Items.Add(this.mnuApplication);
            this.ApplicationGroup.Label = "Create";
            this.ApplicationGroup.Name = "ApplicationGroup";
            // 
            // mnuApplication
            // 
            this.mnuApplication.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.mnuApplication.Dynamic = true;
            this.mnuApplication.Label = "Apps";
            this.mnuApplication.Name = "mnuApplication";
            this.mnuApplication.OfficeImageId = "AnimationPreview";
            this.mnuApplication.ShowImage = true;
            // 
            // groupWizard
            // 
            this.groupWizard.Items.Add(this.splitBtnQuickApp);
            this.groupWizard.Items.Add(this.btnDataMigration);
            this.groupWizard.Label = "Wizards";
            this.groupWizard.Name = "groupWizard";
            // 
            // splitBtnQuickApp
            // 
            this.splitBtnQuickApp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitBtnQuickApp.Items.Add(this.btnQuickAppNew);
            this.splitBtnQuickApp.Items.Add(this.btnQuickAppEdit);
            this.splitBtnQuickApp.Items.Add(this.btnQuickAppSettings);
            this.splitBtnQuickApp.Label = "Quick App";
            this.splitBtnQuickApp.Name = "splitBtnQuickApp";
            this.splitBtnQuickApp.OfficeImageId = "AccessFormWizard";
            this.splitBtnQuickApp.Tag = "QuickApp";
            // 
            // btnQuickAppNew
            // 
            this.btnQuickAppNew.Label = "New ";
            this.btnQuickAppNew.Name = "btnQuickAppNew";
            this.btnQuickAppNew.OfficeImageId = "ListCreateListFromTemplate";
            this.btnQuickAppNew.ShowImage = true;
            this.btnQuickAppNew.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnQuickAppNew_Click);
            // 
            // btnQuickAppEdit
            // 
            this.btnQuickAppEdit.Label = "Edit";
            this.btnQuickAppEdit.Name = "btnQuickAppEdit";
            this.btnQuickAppEdit.OfficeImageId = "EnableInlineEdit";
            this.btnQuickAppEdit.ShowImage = true;
            this.btnQuickAppEdit.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnQuickAppEdit_Click);
            // 
            // btnQuickAppSettings
            // 
            this.btnQuickAppSettings.Label = "Settings";
            this.btnQuickAppSettings.Name = "btnQuickAppSettings";
            this.btnQuickAppSettings.OfficeImageId = "ColumnListSetting";
            this.btnQuickAppSettings.ShowImage = true;
            this.btnQuickAppSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnQuickAppSettings_Click);
            // 
            // btnDataMigration
            // 
            this.btnDataMigration.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDataMigration.Items.Add(this.btnNewDataMigrationApp);
            this.btnDataMigration.Items.Add(this.btnEditDataMirationApp);
            this.btnDataMigration.Label = "Data Migration";
            this.btnDataMigration.Name = "btnDataMigration";
            this.btnDataMigration.OfficeImageId = "BusinessFormWizard";
            // 
            // btnNewDataMigrationApp
            // 
            this.btnNewDataMigrationApp.Label = "New";
            this.btnNewDataMigrationApp.Name = "btnNewDataMigrationApp";
            this.btnNewDataMigrationApp.OfficeImageId = "ListCreateListFromTemplate";
            this.btnNewDataMigrationApp.ShowImage = true;
            this.btnNewDataMigrationApp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnNewDataMigrationApp_Click);
            // 
            // btnEditDataMirationApp
            // 
            this.btnEditDataMirationApp.Enabled = false;
            this.btnEditDataMirationApp.Label = "Edit";
            this.btnEditDataMirationApp.Name = "btnEditDataMirationApp";
            this.btnEditDataMirationApp.OfficeImageId = "EnableInlineEdit";
            this.btnEditDataMirationApp.ShowImage = true;
            this.btnEditDataMirationApp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnEditDataMirationApp_Click);
            // 
            // groupBuild
            // 
            this.groupBuild.Items.Add(this.btnAppDef);
            this.groupBuild.Items.Add(this.btnRetrieveMap);
            this.groupBuild.Items.Add(this.btnMatrixMap);
            this.groupBuild.Items.Add(this.btnSaveMap);
            this.groupBuild.Items.Add(this.splitBtnActions);
            this.groupBuild.Items.Add(this.btnWorkflow);
            this.groupBuild.Items.Add(this.btnMenuDesigner);
            this.groupBuild.Label = "Build";
            this.groupBuild.Name = "groupBuild";
            // 
            // btnAppDef
            // 
            this.btnAppDef.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAppDef.Label = "Objects";
            this.btnAppDef.Name = "btnAppDef";
            this.btnAppDef.OfficeImageId = "DesignXml";
            this.btnAppDef.ShowImage = true;
            this.btnAppDef.Tag = "SalesForceObjects";
            this.btnAppDef.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAppDef_Click);
            // 
            // btnRetrieveMap
            // 
            this.btnRetrieveMap.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRetrieveMap.Label = "Display Map";
            this.btnRetrieveMap.Name = "btnRetrieveMap";
            this.btnRetrieveMap.OfficeImageId = "CacheListData";
            this.btnRetrieveMap.ShowImage = true;
            this.btnRetrieveMap.Tag = "DisplayMap";
            this.btnRetrieveMap.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRetrieveMap_Click);
            // 
            // btnMatrixMap
            // 
            this.btnMatrixMap.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMatrixMap.Label = "Matrix Map";
            this.btnMatrixMap.Name = "btnMatrixMap";
            this.btnMatrixMap.OfficeImageId = "PivotSwitchRowColumn";
            this.btnMatrixMap.ShowImage = true;
            this.btnMatrixMap.Tag = "MatrixMap";
            this.btnMatrixMap.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMatrixMap_Click);
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSaveMap.Label = "Save Map";
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.OfficeImageId = "FileSaveAsCurrentFileFormat";
            this.btnSaveMap.ShowImage = true;
            this.btnSaveMap.Tag = "SaveMap";
            this.btnSaveMap.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSaveMap_Click);
            // 
            // splitBtnActions
            // 
            this.splitBtnActions.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitBtnActions.Items.Add(this.btnAddRowAction);
            this.splitBtnActions.Items.Add(this.btnCheckIn);
            this.splitBtnActions.Items.Add(this.btnCheckOut);
            this.splitBtnActions.Items.Add(this.btnClearAction);
            this.splitBtnActions.Items.Add(this.btnDataSet);
            this.splitBtnActions.Items.Add(this.btnDelete);
            this.splitBtnActions.Items.Add(this.btnRetrieve);
            this.splitBtnActions.Items.Add(this.btnMacroAction);
            this.splitBtnActions.Items.Add(this.btnPasteRow);
            this.splitBtnActions.Items.Add(this.btnpasteSourceData);
            this.splitBtnActions.Items.Add(this.btnExecuteQuery);
            this.splitBtnActions.Items.Add(this.btnCallProcedure);
            this.splitBtnActions.Items.Add(this.btnSaveAction);
            this.splitBtnActions.Items.Add(this.btnSaveAttachmentAction);
            this.splitBtnActions.Items.Add(this.btnSearchSelect);
            this.splitBtnActions.Items.Add(this.btnSwitchConnection);
            this.splitBtnActions.Items.Add(this.btnUserInput);
            this.splitBtnActions.Items.Add(this.btnForm);
            this.splitBtnActions.Label = "Actions";
            this.splitBtnActions.Name = "splitBtnActions";
            this.splitBtnActions.OfficeImageId = "RegionLayoutMenu";
            this.splitBtnActions.Tag = "Actions";
            this.splitBtnActions.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.splitBtnActions_Click);
            // 
            // btnAddRowAction
            // 
            this.btnAddRowAction.Label = "Add Row ";
            this.btnAddRowAction.Name = "btnAddRowAction";
            this.btnAddRowAction.OfficeImageId = "CellsInsertDialog";
            this.btnAddRowAction.ShowImage = true;
            this.btnAddRowAction.Tag = "AddRow";
            this.btnAddRowAction.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAddRowAction_Click);
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Label = "Check-In";
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.OfficeImageId = "ExportExcel";
            this.btnCheckIn.ShowImage = true;
            this.btnCheckIn.Tag = "Checkin";
            this.btnCheckIn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCheckIn_Click);
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.Label = "Check-Out";
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.OfficeImageId = "ImportExcel";
            this.btnCheckOut.ShowImage = true;
            this.btnCheckOut.Tag = "Checkout";
            this.btnCheckOut.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCheckOut_Click);
            // 
            // btnClearAction
            // 
            this.btnClearAction.Label = "Clear";
            this.btnClearAction.Name = "btnClearAction";
            this.btnClearAction.OfficeImageId = "ReviewRejectChange";
            this.btnClearAction.ShowImage = true;
            this.btnClearAction.Tag = "Clear";
            this.btnClearAction.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnClearAction_Click);
            // 
            // btnDataSet
            // 
            this.btnDataSet.Label = "Data Set";
            this.btnDataSet.Name = "btnDataSet";
            this.btnDataSet.OfficeImageId = "DatabaseObjectDependencies";
            this.btnDataSet.ShowImage = true;
            this.btnDataSet.Tag = "DataSetAction";
            this.btnDataSet.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDataSet_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Label = "Delete";
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.OfficeImageId = "CellsDelete";
            this.btnDelete.ShowImage = true;
            this.btnDelete.Tag = "Delete";
            this.btnDelete.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDelete_Click);
            // 
            // btnRetrieve
            // 
            this.btnRetrieve.Label = "Display";
            this.btnRetrieve.Name = "btnRetrieve";
            this.btnRetrieve.OfficeImageId = "TableOfAuthoritiesInsert";
            this.btnRetrieve.ShowImage = true;
            this.btnRetrieve.Tag = "Display";
            this.btnRetrieve.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRetrieve_Click);
            // 
            // btnMacroAction
            // 
            this.btnMacroAction.Label = "Macro";
            this.btnMacroAction.Name = "btnMacroAction";
            this.btnMacroAction.OfficeImageId = "FileSaveAsExcelXlsxMacro";
            this.btnMacroAction.ShowImage = true;
            this.btnMacroAction.Tag = "Macro";
            this.btnMacroAction.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMacroAction_Click);
            // 
            // btnPasteRow
            // 
            this.btnPasteRow.Label = "Paste";
            this.btnPasteRow.Name = "btnPasteRow";
            this.btnPasteRow.OfficeImageId = "Paste";
            this.btnPasteRow.ShowImage = true;
            this.btnPasteRow.Tag = "Paste";
            this.btnPasteRow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnPasteRow_Click);
            // 
            // btnpasteSourceData
            // 
            this.btnpasteSourceData.Label = "Paste Source Data";
            this.btnpasteSourceData.Name = "btnpasteSourceData";
            this.btnpasteSourceData.OfficeImageId = "MailMergeMatchFields";
            this.btnpasteSourceData.ShowImage = true;
            this.btnpasteSourceData.Tag = "PasteSourceDataAction";
            this.btnpasteSourceData.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnpasteSourceData_Click);
            // 
            // btnExecuteQuery
            // 
            this.btnExecuteQuery.Label = "Query";
            this.btnExecuteQuery.Name = "btnExecuteQuery";
            this.btnExecuteQuery.OfficeImageId = "QueryBuilder";
            this.btnExecuteQuery.ShowImage = true;
            this.btnExecuteQuery.Tag = "Query";
            this.btnExecuteQuery.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExecuteQuery_Click);
            // 
            // btnCallProcedure
            // 
            this.btnCallProcedure.Label = "Salesforce Method";
            this.btnCallProcedure.Name = "btnCallProcedure";
            this.btnCallProcedure.OfficeImageId = "MoviePlayAutomatically";
            this.btnCallProcedure.ShowImage = true;
            this.btnCallProcedure.Tag = "SalesforceMethod";
            this.btnCallProcedure.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCallProcedure_Click);
            // 
            // btnSaveAction
            // 
            this.btnSaveAction.Label = "Save";
            this.btnSaveAction.Name = "btnSaveAction";
            this.btnSaveAction.OfficeImageId = "RecordsSaveRecord";
            this.btnSaveAction.ShowImage = true;
            this.btnSaveAction.Tag = "Save";
            this.btnSaveAction.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSaveAction_Click);
            // 
            // btnSaveAttachmentAction
            // 
            this.btnSaveAttachmentAction.Label = "Save Attachment";
            this.btnSaveAttachmentAction.Name = "btnSaveAttachmentAction";
            this.btnSaveAttachmentAction.OfficeImageId = "SaveAttachments";
            this.btnSaveAttachmentAction.ShowImage = true;
            this.btnSaveAttachmentAction.Tag = "SaveAttachment";
            this.btnSaveAttachmentAction.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSaveAttachmentAction_Click);
            // 
            // btnSearchSelect
            // 
            this.btnSearchSelect.Label = "Search and Select";
            this.btnSearchSelect.Name = "btnSearchSelect";
            this.btnSearchSelect.OfficeImageId = "FileWorkflowTasks";
            this.btnSearchSelect.ShowImage = true;
            this.btnSearchSelect.Tag = "SearchandSelect";
            this.btnSearchSelect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSearchSelect_Click);
            // 
            // btnSwitchConnection
            // 
            this.btnSwitchConnection.Label = "Switch Connection";
            this.btnSwitchConnection.Name = "btnSwitchConnection";
            this.btnSwitchConnection.OfficeImageId = "DirectRepliesTo";
            this.btnSwitchConnection.ShowImage = true;
            this.btnSwitchConnection.Tag = "SwitchConnection";
            this.btnSwitchConnection.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSwitchConnection_Click);
            // 
            // btnUserInput
            // 
            this.btnUserInput.Label = "User Input";
            this.btnUserInput.Name = "btnUserInput";
            this.btnUserInput.OfficeImageId = "TableDrawTable";
            this.btnUserInput.ShowImage = true;
            this.btnUserInput.Visible = false;
            this.btnUserInput.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnUserInput_Click);
            // 
            // btnForm
            // 
            this.btnForm.Label = "Forms";
            this.btnForm.Name = "btnForm";
            this.btnForm.OfficeImageId = "ImportSharePointList";
            this.btnForm.ShowImage = true;
            this.btnForm.Tag = "Form";
            this.btnForm.Visible = false;
            this.btnForm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnForm_Click);
            // 
            // btnWorkflow
            // 
            this.btnWorkflow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnWorkflow.Label = "Action Flow";
            this.btnWorkflow.Name = "btnWorkflow";
            this.btnWorkflow.OfficeImageId = "SmartArtAddShapeAbove";
            this.btnWorkflow.ShowImage = true;
            this.btnWorkflow.Tag = "ActionFlow";
            this.btnWorkflow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnWorkflow_Click);
            // 
            // btnMenuDesigner
            // 
            this.btnMenuDesigner.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMenuDesigner.Label = "User Menus";
            this.btnMenuDesigner.Name = "btnMenuDesigner";
            this.btnMenuDesigner.OfficeImageId = "WindowsArrangeIcons";
            this.btnMenuDesigner.ShowImage = true;
            this.btnMenuDesigner.Tag = "UserMenus";
            this.btnMenuDesigner.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMenuDesigner_Click);
            // 
            // groupSaveApp
            // 
            this.groupSaveApp.Items.Add(this.splitbtnSave);
            this.groupSaveApp.Label = "Save";
            this.groupSaveApp.Name = "groupSaveApp";
            // 
            // splitbtnSave
            // 
            this.splitbtnSave.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitbtnSave.Items.Add(this.btnSaveAs);
            this.splitbtnSave.Label = "Save App";
            this.splitbtnSave.Name = "splitbtnSave";
            this.splitbtnSave.OfficeImageId = "FileSaveAs";
            this.splitbtnSave.Tag = "SaveApp";
            this.splitbtnSave.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.splitbtnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Label = "Save As (Clone)";
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.OfficeImageId = "WindowSaveWorkspace";
            this.btnSaveAs.ShowImage = true;
            this.btnSaveAs.Tag = "Clone";
            this.btnSaveAs.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSaveAs_Click);
            // 
            // grpAppSettings
            // 
            this.grpAppSettings.Items.Add(this.btnAppSetting);
            this.grpAppSettings.Label = "Settings";
            this.grpAppSettings.Name = "grpAppSettings";
            this.grpAppSettings.Visible = false;
            // 
            // btnAppSetting
            // 
            this.btnAppSetting.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAppSetting.Label = "App Settings";
            this.btnAppSetting.Name = "btnAppSetting";
            this.btnAppSetting.OfficeImageId = "ComAddInsDialog";
            this.btnAppSetting.ShowImage = true;
            this.btnAppSetting.Tag = "ApplicationSetting";
            this.btnAppSetting.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAppSettings_Click);
            // 
            // groupMigrate
            // 
            this.groupMigrate.Items.Add(this.btnValidate);
            this.groupMigrate.Items.Add(this.btnDataLoader);
            this.groupMigrate.Items.Add(this.btnExternalLibrary);
            this.groupMigrate.Items.Add(this.btnImport);
            this.groupMigrate.Items.Add(this.btnExport);
            this.groupMigrate.Label = "Migrate";
            this.groupMigrate.Name = "groupMigrate";
            // 
            // btnValidate
            // 
            this.btnValidate.Image = global::Apttus.XAuthor.AppDesigner.Properties.Resources.x_auth_check;
            this.btnValidate.Label = "Synch App";
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.ShowImage = true;
            this.btnValidate.Tag = "AppValidate";
            this.btnValidate.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnValidate_Click);
            // 
            // btnDataLoader
            // 
            this.btnDataLoader.Label = "Source Data";
            this.btnDataLoader.Name = "btnDataLoader";
            this.btnDataLoader.OfficeImageId = "ViewNormalViewExcel";
            this.btnDataLoader.ShowImage = true;
            this.btnDataLoader.Tag = "SourceData";
            this.btnDataLoader.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDataLoader_Click);
            // 
            // btnExternalLibrary
            // 
            this.btnExternalLibrary.Label = "External Library";
            this.btnExternalLibrary.Name = "btnExternalLibrary";
            this.btnExternalLibrary.OfficeImageId = "GetExternalDataImportClassic";
            this.btnExternalLibrary.ShowImage = true;
            this.btnExternalLibrary.Tag = "ExternalAction";
            this.btnExternalLibrary.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExternalLibrary_Click);
            // 
            // btnImport
            // 
            this.btnImport.Label = "Import";
            this.btnImport.Name = "btnImport";
            this.btnImport.OfficeImageId = "FileCheckOut";
            this.btnImport.ShowImage = true;
            this.btnImport.Tag = "ImportApp";
            this.btnImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Label = "Export";
            this.btnExport.Name = "btnExport";
            this.btnExport.OfficeImageId = "FileCheckIn";
            this.btnExport.ShowImage = true;
            this.btnExport.Tag = "ExportApp";
            this.btnExport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExport_Click);
            // 
            // groupPreview
            // 
            this.groupPreview.Items.Add(this.btnPreview);
            this.groupPreview.Label = "Preview";
            this.groupPreview.Name = "groupPreview";
            // 
            // btnPreview
            // 
            this.btnPreview.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnPreview.Label = "Preview";
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.OfficeImageId = "MacroPlay";
            this.btnPreview.ShowImage = true;
            this.btnPreview.Tag = "PreviewApp";
            this.btnPreview.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnPreview_Click);
            // 
            // AccessGroup
            // 
            this.AccessGroup.Items.Add(this.btnConnect);
            this.AccessGroup.Items.Add(this.mnuSwitchConnection);
            this.AccessGroup.Items.Add(this.btnGallerySupport);
            this.AccessGroup.Items.Add(this.btnCentralAdmin);
            this.AccessGroup.Label = "Common";
            this.AccessGroup.Name = "AccessGroup";
            // 
            // btnConnect
            // 
            this.btnConnect.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConnect.Label = "Connect";
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.OfficeImageId = "DatabasePermissionsMenu";
            this.btnConnect.ShowImage = true;
            this.btnConnect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnConnect_Click);
            // 
            // mnuSwitchConnection
            // 
            this.mnuSwitchConnection.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.mnuSwitchConnection.Dynamic = true;
            this.mnuSwitchConnection.Label = "Switch Connection";
            this.mnuSwitchConnection.Name = "mnuSwitchConnection";
            this.mnuSwitchConnection.OfficeImageId = "DirectRepliesTo";
            this.mnuSwitchConnection.ShowImage = true;
            this.mnuSwitchConnection.ItemsLoading += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.mnuSwitchConnection_ItemsLoading);
            // 
            // btnGallerySupport
            // 
            this.btnGallerySupport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnGallerySupport.Items.Add(this.btnGeneral);
            this.btnGallerySupport.Items.Add(this.btnSetCRM);
            this.btnGallerySupport.Items.Add(this.separator1);
            this.btnGallerySupport.Items.Add(this.btnAbout);
            this.btnGallerySupport.Label = "Support";
            this.btnGallerySupport.Name = "btnGallerySupport";
            this.btnGallerySupport.OfficeImageId = "TentativeAcceptInvitation";
            this.btnGallerySupport.ShowImage = true;
            this.btnGallerySupport.Visible = false;
            // 
            // btnGeneral
            // 
            this.btnGeneral.Label = "General Settings";
            this.btnGeneral.Name = "btnGeneral";
            this.btnGeneral.OfficeImageId = "ViewDocumentActionsPane";
            this.btnGeneral.ShowImage = true;
            this.btnGeneral.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnGeneral_Click);
            // 
            // btnSetCRM
            // 
            this.btnSetCRM.Label = "Change CRM";
            this.btnSetCRM.Name = "btnSetCRM";
            this.btnSetCRM.OfficeImageId = "ViewDocumentActionsPane";
            this.btnSetCRM.ShowImage = true;
            this.btnSetCRM.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSetCRM_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // btnAbout
            // 
            this.btnAbout.Label = "About X-Author Excel...";
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.ShowImage = true;
            this.btnAbout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAbout_Click);
            // 
            // apttusNotification
            // 
            this.apttusNotification.Visible = true;
            // 
            // btnCentralAdmin
            // 
            this.btnCentralAdmin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnCentralAdmin.Label = "Settings";
            this.btnCentralAdmin.Name = "btnCentralAdmin";
            this.btnCentralAdmin.OfficeImageId = "TagsCustomize";
            this.btnCentralAdmin.ShowImage = true;
            this.btnCentralAdmin.Tag = "ApplicationSetting";
            this.btnCentralAdmin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCentralAdmin_Click);
            // 
            // ApttusRibbon
            // 
            this.Name = "ApttusRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabAppBuilderDesigner);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ApttusRibbon_Load);
            this.tabAppBuilderDesigner.ResumeLayout(false);
            this.tabAppBuilderDesigner.PerformLayout();
            this.ApplicationGroup.ResumeLayout(false);
            this.ApplicationGroup.PerformLayout();
            this.groupWizard.ResumeLayout(false);
            this.groupWizard.PerformLayout();
            this.groupBuild.ResumeLayout(false);
            this.groupBuild.PerformLayout();
            this.groupSaveApp.ResumeLayout(false);
            this.groupSaveApp.PerformLayout();
            this.grpAppSettings.ResumeLayout(false);
            this.grpAppSettings.PerformLayout();
            this.groupMigrate.ResumeLayout(false);
            this.groupMigrate.PerformLayout();
            this.groupPreview.ResumeLayout(false);
            this.groupPreview.PerformLayout();
            this.AccessGroup.ResumeLayout(false);
            this.AccessGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabAppBuilderDesigner;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup AccessGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConnect;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu btnGallerySupport;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnAbout;
        internal System.Windows.Forms.NotifyIcon apttusNotification;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu mnuSwitchConnection;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ApplicationGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupBuild;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAppDef;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnWorkflow;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRetrieveMap;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveMap;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupSaveApp;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitBtnActions;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSearchSelect;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCallProcedure;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExecuteQuery;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRetrieve;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveAction;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitbtnSave;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveAs;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMenuDesigner;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupPreview;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPreview;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu mnuApplication;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMigrate;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnImport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMacroAction;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSaveAttachmentAction;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnClearAction;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCheckIn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCheckOut;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDataLoader;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnUserInput;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpAppSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAppSetting;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMatrixMap;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnpasteSourceData;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSwitchConnection;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitBtnQuickApp;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnQuickAppNew;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnQuickAppEdit;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnQuickAppSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAddRowAction;


        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPasteRow;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnValidate;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDataSet;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExternalLibrary;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDelete;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnGeneral;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton btnDataMigration;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnNewDataMigrationApp;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnEditDataMirationApp;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupWizard;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSetCRM;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCentralAdmin;
    }

    partial class ThisRibbonCollection
    {
        internal ApttusRibbon ApttusRibbon
        {
            get { return this.GetRibbon<ApttusRibbon>(); }
        }
    }
}
