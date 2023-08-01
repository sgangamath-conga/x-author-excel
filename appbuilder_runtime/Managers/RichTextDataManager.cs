/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    class RichTextDataManager
    {
        private static RichTextDataManager _this;
        private Microsoft.Office.Tools.CustomTaskPane _taskPane;
        private RichTextEditor ucRichTextView;
        private System.Data.DataRow dataRow; //At a given time, RichTextManager will always be associated with one row only.
        private string ApttusFieldID;
        private DataTable richTextDataTable;
        private static string RichTextDataTableColumn = "RichTextValue";
        private Guid AppObjectId;
        private bool bIsRichTextFieldPartOfSaveMap = false;
        private string targetNamedRange = string.Empty;
        private FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private RichTextDataManager()
        {
            CreateRichTextDataTable();
            //if (ObjectManager.RuntimeMode == RuntimeMode.AddIn)
            //    Modules.ApttusRegistryManager.CreateOrUpdateKeyForIECompatibility(true);
        }

        public void Dispose()
        {
            richTextDataTable.Dispose();
            richTextDataTable = null;
        }

        internal bool IsSelectionRichTextField(Excel.Range namedRange, Excel.Range Target, out string colName)
        {
            colName = string.Empty;
            bool isRichTextField = false;
            string nameRange = namedRange.Name.Name;
            targetNamedRange = string.Empty;
            if (namedRange != null)
            {
                // Look through all repeating groups
                RepeatingGroup currentRepeatingGroup = ConfigurationManager.GetInstance.GetRepeatingGroupbyTargetNamedRange(nameRange);
                if (currentRepeatingGroup != null)
                {
                    targetNamedRange = currentRepeatingGroup.TargetNamedRange;
                    List<RetrieveField> richTextFields = currentRepeatingGroup.RetrieveFields.Where(f => f.DataType == Datatype.Rich_Textarea && fieldLevelSecurity.IsFieldVisible(f.AppObject, f.FieldId, currentRepeatingGroup.AppObject.ToString())).ToList();
                    if (currentRepeatingGroup.Layout.Equals("Vertical"))
                    {
                        foreach (RetrieveField rf in richTextFields)
                        {
                            if (ExcelHelper.GetRange(rf.TargetNamedRange).Column == Target.Column)
                            {
                                colName = rf.FieldId;
                                isRichTextField = true;
                                AppObjectId = rf.AppObject;
                                break;
                            }
                        }
                    }
                    else if (currentRepeatingGroup.Layout.Equals("Horizontal"))
                    {
                        foreach (RetrieveField rf in richTextFields)
                        {
                            if (ExcelHelper.GetRange(rf.TargetNamedRange).Row == Target.Row)
                            {
                                isRichTextField = true;
                                AppObjectId = rf.AppObject;
                                break;
                            }
                        }
                    }
                }

                // Look through all save groups
                SaveMap currentSaveMap = ConfigurationManager.GetInstance.GetSaveMapbyTargetNamedRange(nameRange);
                if (!isRichTextField && currentSaveMap != null)
                {
                    SaveGroup currentSaveGroup = ConfigurationManager.GetInstance.GetSaveGroupbyTargetNamedRange(nameRange);
                    List<SaveField> richTextFields = currentSaveMap.SaveFields.Where(sf => sf.SaveFieldType == SaveType.SaveOnlyField &&
                                                                                    ApplicationDefinitionManager.GetInstance.GetField(sf.AppObject, sf.FieldId).Datatype == Datatype.Rich_Textarea).ToList();

                    if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Vertical"))
                    {
                        targetNamedRange = currentSaveGroup.TargetNamedRange;

                        foreach (SaveField sf in richTextFields)
                        {
                            ApttusField apttusField = ApplicationDefinitionManager.GetInstance.GetField(currentSaveGroup.AppObject, sf.FieldId);
                            if (apttusField != null && apttusField.Datatype == Datatype.Rich_Textarea)
                            {
                                if (ExcelHelper.GetRange(sf.TargetNamedRange).Column == Target.Column)
                                {
                                    colName = sf.FieldId;
                                    isRichTextField = true;
                                    AppObjectId = sf.AppObject;
                                    break;
                                }
                            }
                        }
                    }
                    else if (currentSaveGroup != null && currentSaveGroup.Layout.Equals("Horizontal"))
                    {
                        foreach (SaveField sf in richTextFields)
                        {
                            ApttusField apttusField = ApplicationDefinitionManager.GetInstance.GetField(currentSaveGroup.AppObject, sf.FieldId);
                            if (apttusField.Datatype == Datatype.Rich_Textarea)
                            {
                                if (ExcelHelper.GetRange(sf.TargetNamedRange).Row == Target.Row)
                                {
                                    isRichTextField = true;
                                    AppObjectId = sf.AppObject;
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            // Look through all Individual Display fields
            RetrieveField retField = ConfigurationManager.GetInstance.GetRetrieveFieldbyTargetNamedRange(nameRange);
            if (retField != null && retField.DataType == Datatype.Rich_Textarea)
            {
                isRichTextField = true;
                colName = retField.FieldId;
                AppObjectId = retField.AppObject;
            }

            // Look through all Individual Save Other fields
            SaveField saveField = ConfigurationManager.GetInstance.GetSaveFieldbyTargetNamedRange(nameRange);
            if (saveField != null)
            {
                ApttusField aptsSaveField = ApplicationDefinitionManager.GetInstance.GetField(saveField.AppObject, saveField.FieldId);
                if (aptsSaveField != null && aptsSaveField.Datatype == Datatype.Rich_Textarea)
                {
                    isRichTextField = true;
                    colName = saveField.FieldId;
                    AppObjectId = saveField.AppObject;
                }
            }

            return isRichTextField;
        }

        public static RichTextDataManager Instance {
            get {
                if (_this == null)
                    _this = new RichTextDataManager();
                return _this;
            }
        }

        public bool Save(string htmlText)
        {
            try
            {
                if (String.IsNullOrEmpty(ApttusFieldID))
                    return false;
                if (dataRow == null)
                    return false;

                string id = dataRow[Constants.ID_ATTRIBUTE] as string;
                DataRow row = GetRowFromRichTextTable(id, ApttusFieldID);
                if (row == null) // If the record doesn't exist then create a new row, else update the existing one.
                {
                    DataRow newRow = richTextDataTable.NewRow();
                    newRow[Constants.ID_ATTRIBUTE] = id;
                    newRow[Constants.FieldId] = ApttusFieldID;
                    newRow[RichTextDataTableColumn] = htmlText;
                    newRow["SaveFieldType"] = SaveType.RetrievedField;
                    richTextDataTable.Rows.Add(newRow);
                }
                else
                    row[RichTextDataTableColumn] = htmlText;

                richTextDataTable.AcceptChanges();
                return true;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, resourceManager.GetResource("RichTextEditor_browser_ResponseRecieved_CAP"));
                return false;
            }
        }

        internal bool IsRecordDirty(string recordId, string fieldId, out string value)
        {
            value = string.Empty;
            DataRow row = GetRowFromRichTextTable(recordId, fieldId);
            bool bIsDirty = false;
            if (row != null)
            {
                value = row[RichTextDataTableColumn] as string;
                Guid parsedGuid;
                if (Guid.TryParse(recordId, out parsedGuid))
                {
                    if (!String.IsNullOrEmpty(value))
                        bIsDirty = true;
                }
                else
                    bIsDirty = true;
            }
            return bIsDirty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recId">salesforce record id</param>
        /// <param name="dataSetLocation"></param>
        /// <param name="fieldId">colName is nothing but an alias for salesforce fieldId</param>
        public void LoadRichTextData(string recId, string dataSetLocation, string fieldId)
        {
            if (_taskPane == null)
                ShowRichTextPane();

            DataManager dataManager = DataManager.GetInstance;
            ApttusDataSet dataSet = dataManager.GetDataSetFromLocation(dataSetLocation);
            SaveField outSaveField = null;
            if (dataSet != null)
            {
                dataRow = dataSet.DataTable.Select(Constants.ID_ATTRIBUTE + "= '" + recId + "'").FirstOrDefault();
                if (dataRow == null)  //Case of addrow.
                {
                    dataRow = GetRowFromRichTextTable(recId, fieldId);
                    recId = string.Empty;
                }

                ApttusObject apttusObj = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectId);

                if (ApttusFieldID != fieldId)
                {
                    ApttusFieldID = fieldId;
                }
                bIsRichTextFieldPartOfSaveMap = IsRichTextFieldPartOfSaveMap(fieldId, out outSaveField) && !fieldLevelSecurity.IsFieldReadOnly(apttusObj.UniqueId, fieldId);

                bool isSaveOtherField = outSaveField != null ? outSaveField.SaveFieldType.Equals(SaveType.SaveOnlyField) : false;

                ucRichTextView.LoadRichTextPage(apttusObj.Id, fieldId, recId, !bIsRichTextFieldPartOfSaveMap, isSaveOtherField);
            }
        }

        private bool IsRichTextFieldPartOfSaveMap(string fieldId, out SaveField outSaveField)
        {
            foreach (SaveMap saveMap in ConfigurationManager.GetInstance.SaveMaps)
            {
                foreach (SaveGroup saveGroup in saveMap.SaveGroups)
                {
                    foreach (SaveField saveField in saveMap.SaveFields.Where(sf => sf.GroupId.Equals(saveGroup.GroupId)))
                    {
                        ApttusField field = ApplicationDefinitionManager.GetInstance.GetField(saveField.AppObject, saveField.FieldId);
                        if (field != null && field.Datatype == Datatype.Rich_Textarea && field.Id.Equals(fieldId) && saveGroup.TargetNamedRange.Equals(targetNamedRange))
                        {
                            outSaveField = saveField;
                            return true;
                        }
                    }
                }
                //For Individual fields where GroupId and TargetNamedRange will be empty
                foreach (SaveField saveField in saveMap.SaveFields.Where(sf => sf.GroupId.Equals(Guid.Empty)))
                {
                    ApttusField field = ApplicationDefinitionManager.GetInstance.GetField(saveField.AppObject, saveField.FieldId);
                    if (field != null && field.Datatype == Datatype.Rich_Textarea && field.Id.Equals(fieldId))
                    {
                        outSaveField = saveField;
                        return true;
                    }
                }
            }
            outSaveField = null;
            return false;
        }

        private void CreateRichTextDataTable()
        {
            if (richTextDataTable != null)
                return;

            richTextDataTable = new DataTable("RichTextDataTable");

            DataColumn idColumn = new DataColumn(Constants.ID_ATTRIBUTE, typeof(string));
            richTextDataTable.Columns.Add(idColumn);

            DataColumn fieldIdColumn = new DataColumn(Constants.FieldId, typeof(string));
            richTextDataTable.Columns.Add(fieldIdColumn);

            DataColumn richTextValueColumn = new DataColumn(RichTextDataTableColumn, typeof(string));
            richTextDataTable.Columns.Add(richTextValueColumn);

            DataColumn saveFieldType = new DataColumn("SaveFieldType", typeof(Enum));
            richTextDataTable.Columns.Add(saveFieldType);

            richTextDataTable.PrimaryKey = new DataColumn[] { idColumn, fieldIdColumn };
        }

        private void ShowRichTextPane()
        {
            if (_taskPane != null)
                return;

            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;
                ucRichTextView = new RichTextEditor();
                _taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(ucRichTextView, resourceManager.GetResource("RICHTEXTDATAMAN_ShowRichTextPane_Text"), Globals.ThisAddIn.Application.ActiveWindow);
                _taskPane.VisibleChanged += _taskPane_VisibleChanged;
                _taskPane.Visible = true;
                _taskPane.Width = 400;
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
        }

        private void _taskPane_VisibleChanged(object sender, EventArgs e)
        {
            if (_taskPane != null && _taskPane.Visible == false)
            {
                dataRow = null;
                ApttusFieldID = string.Empty;
                ucRichTextView.Dispose();
                ucRichTextView = null;
                Globals.ThisAddIn.CustomTaskPanes.Remove(_taskPane);
                _taskPane.Dispose();
                _taskPane = null;
            }
        }

        private DataRow GetRowFromRichTextTable(string recordId, string fieldId)
        {
            if (richTextDataTable == null)
                return null;

            DataRow[] rows = richTextDataTable.Select(Constants.ID_ATTRIBUTE + "= '" + recordId + "' and " + Constants.FieldId + "= '" + fieldId + "'");
            if (rows != null && rows.Count() > 0)
                return rows[0];
            return null;
        }

        internal bool RemoveRecord(string recordId, string fieldId)
        {
            DataRow row = GetRowFromRichTextTable(recordId, fieldId);
            if (row != null)
            {
                try
                {
                    row.Delete();
                }
                finally
                {
                    richTextDataTable.AcceptChanges();
                }
                return true;
            }
            return false;
        }

        internal void AddRecord(string id, string fieldId)
        {
            DataRow row = GetRowFromRichTextTable(id, fieldId);
            if (row == null)
            {
                row = richTextDataTable.NewRow();

                row[Constants.ID_ATTRIBUTE] = id;
                row[Constants.FieldId] = fieldId;
                row[RichTextDataTableColumn] = string.Empty;
                row["SaveFieldType"] = SaveType.SaveOnlyField;

                richTextDataTable.Rows.Add(row);
                richTextDataTable.AcceptChanges();
            }
        }
    }
}
