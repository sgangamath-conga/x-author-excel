/*
 * Apttus X-Author for Excel
 * © 2015 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Apttus.XAuthor.Core;

using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    class MatrixSaveHelper
    {
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private DataManager dataManager = DataManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private MatrixMap matrixMap = null;
        private FieldLevelSecurityManager fieldLevelSecurity = FieldLevelSecurityManager.Instance;
        StringBuilder LookupErrors = new StringBuilder();
        public ActionResult Result { get; private set; }
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public MatrixSaveHelper()
        {
            Result = new ActionResult();
        }

        public List<ApttusSaveRecord> CreateSaveRecords(List<SaveField> matrixSaveFields, out ActionResult resultStatus)
        {
            resultStatus = Result;
            Dictionary<string, Guid> AppObjUniqueIdFromRecordId = new Dictionary<string, Guid>();
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
            List<Guid> saveRecordObjectsIds = new List<Guid>();
            ApttusMatrixDataSet matrixDataSet = null;
            //Get All MatrixMapIDs in this savemap.
            List<Guid> matrixMapIds = matrixSaveFields.GroupBy(field => field.MatrixMapId).Select(g => g.First()).ToList().Select(sf => sf.MatrixMapId).ToList();

            matrixMapIds.RemoveAll(id => Guid.Empty.Equals(id));

            foreach (Guid matrixMapId in matrixMapIds)
            {
                this.matrixMap = configurationManager.MatrixMaps.Where(map => map.Id.Equals(matrixMapId)).FirstOrDefault();
                matrixDataSet = dataManager.GetMatrixDataSetByMatrixMap(matrixMapId);
                try
                {
                    if (matrixDataSet != null)
                    {
                        DataTable originalDataTable = matrixDataSet.MatrixDataTable;

                        // Add Row and Column Data in Matrix Data table - Sync datatable with salesforce IDs
                        AddRowColumnDataInTable(matrixDataSet, out LookupErrors);
                       
                        //Create a matrix datatable from excel
                        using (DataTable changedDataTable = CreateMatrixDataTableFromExcel(matrixMap, matrixSaveFields))
                        {
                            //Loop through all changed rows.
                            foreach (DataRow changedRow in changedDataTable.Rows)
                            {
                                Guid dataFieldId = Guid.Parse((changedRow[Constants.MATRIX_DATAFIELDID] as string));

                                MatrixDataField dataField = matrixMap.MatrixData.MatrixDataFields.Where(df => df.Id.Equals(dataFieldId)).FirstOrDefault();

                                if (fieldLevelSecurity.IsFieldReadOnly(dataField.AppObjectUniqueID, dataField.FieldId))
                                    continue;

                                MatrixField rowField = matrixMap.MatrixRow.MatrixFields.Where(rf => rf.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();
                                MatrixField colField = matrixMap.MatrixColumn.MatrixFields.Where(cf => cf.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();

                                //Create a filter on dataField, rowIndex and colIndex
                                string filter = Constants.MATRIX_DATAFIELDID + " = '" + dataField.Id + "' and " + Constants.MATRIX_DATAROWINDEX + " = '" + changedRow[Constants.MATRIX_DATAROWINDEX] +
                                                    "' and " + Constants.MATRIX_DATACOLINDEX + " = '" + changedRow[Constants.MATRIX_DATACOLINDEX] + "'";

                                //Apply the filter on originalDataTable
                                DataRow[] originalRows = originalDataTable.Select(filter);

                                if (originalRows != null && originalRows.Count() > 0)
                                {
                                    DataRow originalRow = originalRows[0];
                                    string originalValue = originalRow[Constants.VALUE_COLUMN] as string;
                                    string changedValue = changedRow[Constants.VALUE_COLUMN] as string;
                                    string recordId = originalRow[Constants.ID_ATTRIBUTE] as string;
                                    if (SaveHelper.IsDirty(dataField.DataType, originalValue, changedValue))
                                    {
                                        changedValue = SaveHelper.TranslateData(dataField.DataType, changedValue);
                                        //SaveRecords.Add(CreateSaveRecord(matrixDataSet, originalRow, dataField, recordId, changedValue));
                                        if (!AppObjUniqueIdFromRecordId.ContainsKey(recordId))
                                            AppObjUniqueIdFromRecordId.Add(recordId, dataField.AppObjectUniqueID);

                                        originalRow[Constants.VALUE_COLUMN] = changedValue;
                                        saveRecordObjectsIds.Add(dataField.AppObjectUniqueID);
                                    }

                                }
                            }
                        }

                        // Now create save records based on dirty records in original datatable.
                        DataTable dirtyTable = originalDataTable.GetChanges();
                        if (dirtyTable != null && dirtyTable.Rows.Count > 0)
                        {
                            List<string> recordIds = dirtyTable.AsEnumerable().GroupBy(dt => dt.Field<string>(Constants.ID_ATTRIBUTE)).Select(dt => dt.Key).ToList();
                            foreach (string recordId in recordIds)
                            {
                                string filter = Constants.ID_ATTRIBUTE + "= '" + recordId + "'";
                                // Get all rows from Dirty Matrix data table that come together to create one Salesforce Record.
                                DataRow[] rows = dirtyTable.Select(filter);
                                if (rows != null && rows.Count() > 0)
                                {
                                    Guid result;
                                    QueryTypes operationType = Guid.TryParse(recordId, out result) ? QueryTypes.INSERT : QueryTypes.UPDATE;

                                    ApttusSaveRecord record = CreateSaveRecord(matrixDataSet, AppObjUniqueIdFromRecordId[recordId], recordId, operationType);

                                    // Add Data fields for each dirty row in Save Record
                                    foreach (DataRow row in rows)
                                    {
                                        MatrixDataField dataField = matrixMap.MatrixData.MatrixDataFields.Where(df => df.Id.Equals(Guid.Parse(row[Constants.MATRIX_DATAFIELDID] as string))).FirstOrDefault();
                                        AddMatrixDataFieldsToSaveRecord(record, dataField, row);                                        
                                    }
                                    if (operationType == QueryTypes.INSERT)
                                    {
                                        AddMatrixComponentFilterFields(record, rows[0], operationType, originalDataTable);
                                        AddMatrixParentRecordField(record, rows[0], matrixDataSet);
                                        AddMatrixComponentRelatedFields(record, rows[0], matrixDataSet);
                                    }
                                    SaveRecords.Add(record);                                                                         
                                }
                            }
                        }
                    }
                }
        
                catch (Exception ex)
                {
                    matrixDataSet.MatrixDataTable.RejectChanges();
                    RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
                }
            }

            List<Guid> uniqueObjects = saveRecordObjectsIds.Distinct().ToList();

            foreach (Guid uniqueObj in uniqueObjects)
            {
                ApttusObject currentObj = appDefManager.GetAppObject(uniqueObj);

                //StringBuilder LookupErrors = new StringBuilder();
                if (LookupErrors.Length == 0 || LookupErrors == null)
                    SaveHelper.ValidateLookupNamesBeforeSave(
                        (from sr in SaveRecords where sr.ObjectName.Equals(currentObj.Id) && sr.SaveFields.Exists(sf => sf.LookupNameField) select sr).ToList(), currentObj, out LookupErrors);

                if (LookupErrors.Length > 0)
                {
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("MATRIXSAVEHLP_LookVal_ErrorMsg"), LookupErrors.ToString(),resourceManager.CRMName), resourceManager.GetResource("COMMONRUNTIME_SaveHelper_ErrorMsg"));
                    resultStatus.Status = ActionResultStatus.Failure;
                    matrixDataSet.MatrixDataTable.RejectChanges();
                    SaveRecords.Clear();
                    return SaveRecords;
                }
            }
            resultStatus.Status = ActionResultStatus.Success;
            return SaveRecords;
        }        

        private ApttusSaveRecord CreateSaveRecord(ApttusMatrixDataSet dataSet, Guid AppObjectUniqueID, string recordId, QueryTypes operationType)
        {
            ApttusSaveRecord saveRecord = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField>(),
                ApttusDataSetId = dataSet.Id,
                RecordId = operationType == QueryTypes.UPDATE ? recordId : string.Empty,
                OperationType = operationType,
                TargetNameRange = recordId,// Targetnamedrange is re-purposed here, to stored record ID, which helps make decision in ProcessResult for Accept / Reject changes.
                RecordRowNo = -1,
                RecordColumnNo = -1,
                ObjectType = ObjectType.Repeating,
                ObjectName = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectUniqueID).Id,
            };

            return saveRecord;
        }

        private void AddMatrixDataFieldsToSaveRecord(ApttusSaveRecord saveRecord, MatrixDataField dataField, DataRow dirtyRow)
        {
            // Add data field value to Save record
            saveRecord.SaveFields.Add(new ApttusSaveField
            {
                DataType = appDefManager.GetField(dataField.AppObjectUniqueID, dataField.FieldId).Datatype,
                FieldId = dataField.FieldId,
                SaveFieldType = SaveType.MatrixField,
                TargetNamedRange = dataField.TargetNamedRange,
                Value = Convert.ToString(dirtyRow[Constants.VALUE_COLUMN]),
                LookupNameField = dataField.FieldId.EndsWith(Constants.APPENDLOOKUPID)
            });
            saveRecord.AppObject = dataField.AppObjectUniqueID;
            saveRecord.MatrixColIndex = Convert.ToInt32(dirtyRow[Constants.MATRIX_DATACOLINDEX].ToString());
            saveRecord.MatrixRowIndex = Convert.ToInt32(dirtyRow[Constants.MATRIX_DATAROWINDEX].ToString());

        }

        /// <summary>
        /// Adds all related fields of a particular datafield. We get all the datafields which are grouped via matrix component. For Project Planning Usecase
        /// </summary>
        /// <param name="saveRecord"></param>
        /// <param name="dirtyRow"></param>
        /// <param name="dataSet"></param>
        private void AddMatrixComponentRelatedFields(ApttusSaveRecord saveRecord, DataRow dirtyRow, ApttusMatrixDataSet dataSet)
        {
            MatrixDataField dataField = matrixMap.MatrixData.MatrixDataFields.Find(df => df.Id.Equals(Guid.Parse(dirtyRow[Constants.MATRIX_DATAFIELDID] as string)));

            List<MatrixDataField> dataFields = matrixMap.MatrixData.MatrixDataFields.Where(df => df.Id != dataField.Id &&
                                                                                                 df.MatrixComponentId == dataField.MatrixComponentId).ToList();
            foreach (MatrixDataField relatedDataField in dataFields)
            {
                if (saveRecord.SaveFields.Exists(sf => sf.FieldId.Equals(relatedDataField.FieldId)))
                    continue;
                
                Excel.Range dataFieldRange = ExcelHelper.GetRange(dataField.TargetNamedRange);
                Excel.Range relatedDataFieldRange = ExcelHelper.GetRange(relatedDataField.TargetNamedRange);
                // In case of Static by Static, Matrix data table will always have similar Row and Column, so check Excel row index as well
                if (dataFieldRange.Row != relatedDataFieldRange.Row)
                    continue;

                string filter = Constants.MATRIX_DATAFIELDID + " = '" + relatedDataField.Id + "' and " +
                                Constants.MATRIX_DATAROWINDEX + " = '" + dirtyRow[Constants.MATRIX_DATAROWINDEX] + "'";

                //Apply the filter on originalDataTable
                DataRow[] originalRows = dataSet.MatrixDataTable.Select(filter);

                if (originalRows == null)
                    continue;
                if (originalRows.Count() != 1)
                    continue;

                string value = Convert.ToString(originalRows[0][Constants.VALUE_COLUMN]);
                if (string.IsNullOrEmpty(value))
                    continue;

                saveRecord.SaveFields.Add(new ApttusSaveField
                {
                    DataType = relatedDataField.DataType,
                    FieldId = relatedDataField.FieldId,
                    SaveFieldType = SaveType.MatrixField,
                    LookupNameField = relatedDataField.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                    TargetNamedRange = relatedDataField.TargetNamedRange,
                    Value = value
                });
            }
        }

        private void AddMatrixParentRecordField(ApttusSaveRecord saveRecord, DataRow dirtyRow, ApttusMatrixDataSet dataSet)
        {
            MatrixDataField dataField = matrixMap.MatrixData.MatrixDataFields.Where(df => df.Id.Equals(Guid.Parse(dirtyRow[Constants.MATRIX_DATAFIELDID] as string))).FirstOrDefault();

            foreach (MatrixField independentField in matrixMap.IndependentFields)
            {
                // Get parent App Object & data set
                ApttusDataSet parentObjectDataSet = dataManager.GetDataById(independentField.AppObjectUniqueID);
                if (parentObjectDataSet == null)
                    continue;

                ApttusObject parentObject = appDefManager.GetAppObject(independentField.AppObjectUniqueID);

                // Get Data Field App Object
                ApttusObject dataObject = appDefManager.GetAppObject(dataField.AppObjectUniqueID);
                ApttusField parentLookupField = dataObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(parentObject.Id)).FirstOrDefault();

                if (parentLookupField == null)
                    continue;

                // Matrix Map can have more than one fields of Individual Object as Matrix Individual field, if Lookupfield for Individual object is already part of Save Fields continue
                if (saveRecord.SaveFields.Exists(f => f.FieldId.Equals(parentLookupField.Id)))
                    continue;

                if (parentLookupField != null && parentObjectDataSet.DataTable != null)
                {
                    string value = string.Empty;

                    if (parentObjectDataSet.DataTable.Rows.Count == 1)
                        value = Convert.ToString(parentObjectDataSet.DataTable.Rows[0][Constants.ID_ATTRIBUTE]);

                    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                        continue;

                    // Add Save Field for Parent
                    saveRecord.SaveFields.Add(new ApttusSaveField
                    {
                        DataType = parentLookupField.Datatype,
                        FieldId = parentLookupField.Id,
                        SaveFieldType = SaveType.MatrixField,
                        LookupNameField = parentLookupField.Id.EndsWith(Constants.APPENDLOOKUPID),
                        TargetNamedRange = dataField.TargetNamedRange,
                        Value = value
                    });
                }
            }
        }


        private void AddMatrixComponentFilterFields(ApttusSaveRecord saveRecord, DataRow dirtyRow, QueryTypes operationType, DataTable originalTable)
        {
            MatrixDataField dataField = matrixMap.MatrixData.MatrixDataFields.Where(df => df.Id.Equals(Guid.Parse(dirtyRow[Constants.MATRIX_DATAFIELDID] as string))).FirstOrDefault();
            if (operationType == QueryTypes.INSERT)
            {
                // Get Row and Column field of data Field
                MatrixField rowField = this.matrixMap.MatrixRow.MatrixFields.Where(rf => rf.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();
                MatrixField colField = this.matrixMap.MatrixColumn.MatrixFields.Where(cf => cf.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();

                // Get data field object
                ApttusObject dataObject = appDefManager.GetAppObject(dataField.AppObjectUniqueID);

                // Get row and column values from original rendered data table
                if (rowField.ValueType == MatrixValueType.FieldValue)
                {
                    string rowValue = Convert.ToString(dirtyRow[Constants.MATRIX_DATAROWVALUE]);
                    // Add row field value to Save record
                    if (dataField.AppObjectUniqueID == rowField.AppObjectUniqueID)
                    {
                        // If Row field is Dynamic and Lookup name field use it's lookupId field
                        string rowFieldId = rowField.RenderingType.Equals(MatrixRenderingType.Dynamic) && rowField.FieldId.EndsWith(Constants.APPENDLOOKUPID) ?
                            appDefManager.GetLookupIdFromLookupName(rowField.FieldId) : rowField.FieldId;

                        bool bIsLookup = rowField.RenderingType.Equals(MatrixRenderingType.Dynamic) && rowField.FieldId.EndsWith(Constants.APPENDLOOKUPID);

                        if (bIsLookup && dirtyRow[Constants.MATRIX_ISCOLUMNVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISCOLUMNVALUE]) == true)
                        {
                            rowValue = GetLookupFieldId(rowField.AppObjectUniqueID, dirtyRow, rowFieldId, rowValue);
                            if (!string.IsNullOrEmpty(rowValue))
                                UpdateOriginalDataTable(originalTable, dirtyRow, rowValue, MatrixEntity.Row);                            
                        }

                        // get Row field as apttusfield to use datatype and other properties
                        ApttusField apttusRowField = appDefManager.GetField(rowField.AppObjectUniqueID, rowFieldId);

                        saveRecord.SaveFields.Add(new ApttusSaveField
                        {
                            DataType = appDefManager.GetField(rowField.AppObjectUniqueID, rowFieldId).Datatype,
                            FieldId = rowFieldId,
                            SaveFieldType = SaveType.MatrixField,
                            LookupNameField = rowFieldId.EndsWith(Constants.APPENDLOOKUPID),
                            TargetNamedRange = rowField.TargetNamedRange,
                            Value = SaveHelper.TranslateData(apttusRowField.Datatype, rowValue)
                        });
                    }
                    else if (dataField.AppObjectUniqueID != rowField.AppObjectUniqueID)
                    {
                        ApttusObject rowObject = appDefManager.GetAppObject(rowField.AppObjectUniqueID);

                        ApttusField rowLookupField = null;
                        if (string.IsNullOrEmpty(dataField.RowLookupId)) //For Backward Compatability. For those apps whose config doesn't have value of MatrixDataField.RowLookupId
                            rowLookupField = dataObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(rowObject.Id)).FirstOrDefault();
                        else
                            rowLookupField = dataObject.Fields.Where(fld => fld.Id.Equals(dataField.RowLookupId)).FirstOrDefault();

                        //string lookupIdField = appDefManager.GetLookupIdFromLookupName(rowField.FieldId);

                        if ((dirtyRow[Constants.MATRIX_ISCOLUMNVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISCOLUMNVALUE]) == true) ||
                            (dirtyRow[Constants.MATRIX_ISROWVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISROWVALUE]) == true))
                        {
                            string id = GetLookupFieldId(dataField.AppObjectUniqueID, dirtyRow, rowLookupField.Id, rowValue);
                            if (!String.IsNullOrEmpty(id))
                            {
                                UpdateOriginalDataTable(originalTable, dirtyRow, id, MatrixEntity.Row);
                                rowValue = id;
                            }
                        }
                        saveRecord.SaveFields.Add(new ApttusSaveField
                        {
                            DataType = rowLookupField.Datatype,
                            FieldId = rowLookupField.Id,
                            SaveFieldType = SaveType.MatrixField,
                            LookupNameField = rowLookupField.Id.EndsWith(Constants.APPENDLOOKUPID),
                            TargetNamedRange = rowField.TargetNamedRange,
                            Value = rowValue
                        });
                    }
                }

                if (colField.ValueType == MatrixValueType.FieldValue)
                {
                    string colValue = Convert.ToString(dirtyRow[Constants.MATRIX_DATACOLVALUE]);
                    // Add column field value to Save record
                    if (dataField.AppObjectUniqueID == colField.AppObjectUniqueID)
                    {
                        // If Column field is a Lookup name field use it's lookupId field
                        bool bIsLookup = colField.RenderingType.Equals(MatrixRenderingType.Dynamic) && colField.FieldId.EndsWith(Constants.APPENDLOOKUPID);
                        string colFieldId = bIsLookup ? appDefManager.GetLookupIdFromLookupName(colField.FieldId) : colField.FieldId;

                        //If row is added and a column is a lookup field, then resolve the column lookup.
                        //We do this because while adding a row, we specify the column value as defined in excel cell.
                        if (bIsLookup && dirtyRow[Constants.MATRIX_ISROWVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISROWVALUE]) == true)
                        {
                            colValue = GetLookupFieldId(colField.AppObjectUniqueID, dirtyRow, colFieldId, colValue);
                            if (!string.IsNullOrEmpty(colValue))
                                UpdateOriginalDataTable(originalTable, dirtyRow, colValue, MatrixEntity.Column);                            
                        }
                        // get Column field as apttusfield to use datatype and other properties
                        ApttusField apttusColField = appDefManager.GetField(colField.AppObjectUniqueID, colFieldId);

                        saveRecord.SaveFields.Add(new ApttusSaveField
                        {
                            DataType = appDefManager.GetField(colField.AppObjectUniqueID, colFieldId).Datatype,
                            FieldId = colFieldId,
                            SaveFieldType = SaveType.MatrixField,
                            LookupNameField = colFieldId.EndsWith(Constants.APPENDLOOKUPID),
                            TargetNamedRange = colField.TargetNamedRange,
                            Value = SaveHelper.TranslateData(apttusColField.Datatype, colValue)
                        });
                    }
                    else if (dataField.AppObjectUniqueID != colField.AppObjectUniqueID)
                    {
                        ApttusObject colObject = appDefManager.GetAppObject(colField.AppObjectUniqueID);
                        ApttusField colLookupField = null;
                        if (string.IsNullOrEmpty(dataField.ColumnLookupId)) //For Backward Compatability. For those apps whose config doesn't have value of MatrixDataField.ColumnLookupId
                            colLookupField = dataObject.Fields.Where(fld => fld.LookupObject != null && fld.LookupObject.Id.Equals(colObject.Id)).FirstOrDefault();
                        else
                            colLookupField = dataObject.Fields.Where(fld => fld.Id.Equals(dataField.ColumnLookupId)).FirstOrDefault();

                        //string lookupIdField = appDefManager.GetLookupIdFromLookupName(colf);

                        if ((dirtyRow[Constants.MATRIX_ISCOLUMNVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISCOLUMNVALUE]) == true) ||
                            (dirtyRow[Constants.MATRIX_ISROWVALUE] != DBNull.Value && Convert.ToBoolean(dirtyRow[Constants.MATRIX_ISROWVALUE]) == true))
                        {
                            string id = GetLookupFieldId(dataField.AppObjectUniqueID, dirtyRow, colLookupField.Id, colValue);
                            if (!String.IsNullOrEmpty(id))
                            {
                                UpdateOriginalDataTable(originalTable, dirtyRow, id, MatrixEntity.Column);
                                colValue = id;
                            }
                        }

                        saveRecord.SaveFields.Add(new ApttusSaveField
                        {
                            DataType = colLookupField.Datatype,
                            FieldId = colLookupField.Id,
                            SaveFieldType = SaveType.MatrixField,
                            LookupNameField = colLookupField.Id.EndsWith(Constants.APPENDLOOKUPID),
                            TargetNamedRange = colField.TargetNamedRange,
                            Value = colValue
                        });
                    }
                }
            }

            // Get Matrix Component from Data Field
            MatrixComponent dataFieldComponent = this.matrixMap.MatrixComponents.Where(cp => cp.Id.Equals(dataField.MatrixComponentId)).FirstOrDefault();

            // Get all filter fields where operator is Equals
            if ((dataFieldComponent.WhereFilterGroups != null && dataFieldComponent.WhereFilterGroups.Count > 0) &&
                (dataFieldComponent.WhereFilterGroups[0].Filters != null && dataFieldComponent.WhereFilterGroups[0].Filters.Count > 0))
            {
                // Check if this is Efficient place for replacing Cell Reference values
                ExcelHelper.SetCellReferenceFilterValue(dataFieldComponent.WhereFilterGroups);

                List<SearchFilter> filterFields = dataFieldComponent.WhereFilterGroups[0].Filters.Where(f => f.Operator.Equals(Constants.EQUALS)).ToList();

                // Add all filter field value to Save record
                foreach (SearchFilter filter in filterFields)
                {
                    if (saveRecord.SaveFields.Exists(sf => sf.FieldId.Equals(filter.FieldId)))
                        continue;

                    saveRecord.SaveFields.Add(new ApttusSaveField
                    {
                        DataType = appDefManager.GetField(filter.AppObjectUniqueId, filter.FieldId).Datatype,
                        FieldId = filter.FieldId,
                        SaveFieldType = SaveType.MatrixField,
                        LookupNameField = filter.FieldId.EndsWith(Constants.APPENDLOOKUPID),
                        TargetNamedRange = string.Empty,
                        Value = filter.Value
                    });
                }
            }

        }

        private string GetLookupFieldId(Guid AppObjectUniqueId, DataRow dirtyRow, string fieldId, string colValueToResolve)
        {
            string Id = string.Empty;
            ApttusObject oApttusObject = appDefManager.GetAppObject(AppObjectUniqueId);
            ApttusField lookupField = oApttusObject.Fields.Where(f => f.Id == fieldId).FirstOrDefault();

            Id = dataManager.MatrixFindLookupNames(lookupField.LookupObject, new List<string> { colValueToResolve });

            if (string.IsNullOrEmpty(Id))
            {
                string ErrorMessage = String.Format(resourceManager.GetResource("COMMON_LookUpInvalid_Msg"), oApttusObject.Fields.Where(f => f.Id == fieldId).FirstOrDefault().Name, colValueToResolve);
                LookupErrors.Append(ErrorMessage);                
            }
            return Id;
        }

        private void UpdateOriginalDataTable(DataTable originalDataTable, DataRow dirtyDataTableDataRow, string value, MatrixEntity entity)
        {
            string filter = Constants.MATRIX_DATAFIELDID + " = '" + dirtyDataTableDataRow[Constants.MATRIX_DATAFIELDID] + "' and " +
                                                Constants.MATRIX_DATAROWINDEX + " = '" + dirtyDataTableDataRow[Constants.MATRIX_DATAROWINDEX] + "' and " +
                                                Constants.MATRIX_DATACOLINDEX + " = '" + dirtyDataTableDataRow[Constants.MATRIX_DATACOLINDEX] + "'";

            DataRow[] rows = originalDataTable.Select(filter);

            if (rows != null && rows.Count() == 1)
            {
                DataRow row = rows[0];
                if (entity == MatrixEntity.Column)
                {
                    row[Constants.MATRIX_ISROWVALUE] = false;
                    row[Constants.MATRIX_DATACOLVALUE] = value;
                }
                else if (entity == MatrixEntity.Row)
                {
                    row[Constants.MATRIX_ISCOLUMNVALUE] = false;
                    row[Constants.MATRIX_DATAROWVALUE] = value;
                }
            }
        }

        private static void CreateDataTableEntry(DataTable matrixData, object value, Guid dataFieldId, int rowIndex, int colIndex)
        {
            //DataRow dr = matrixData.NewRow();
            //dr[Constants.MATRIX_DATAFIELDID] = dataFieldId;
            //dr[Constants.VALUE_COLUMN] = value;
            //dr[Constants.MATRIX_DATAROWINDEX] = rowIndex;
            //dr[Constants.MATRIX_DATACOLINDEX] = colIndex;
            //matrixData.Rows.Add(dr);

            object[][] objArray = new object[1][];
            objArray[0] = new object[4];

            objArray[0][0] = value;
            objArray[0][1] = dataFieldId;
            objArray[0][2] = rowIndex;
            objArray[0][3] = colIndex;

            matrixData.LoadDataRow(objArray[0], false);
        }

        public static DataTable CreateMatrixDataTableFromExcel(MatrixMap model, List<SaveField> matrixSaveFields)
        {
            List<SaveField> fields = matrixSaveFields.Where(field => field.MatrixMapId.Equals(model.Id)).ToList();

            // Get all Matrix Data fields from savefield.MatrixComponentId
            var matrixDataFieldsAsSavefields = (from fld in fields
                                                from datafield in model.MatrixData.MatrixDataFields
                                                where fld.MatrixComponentId.Equals(datafield.MatrixComponentId)
                                                select datafield).ToList();

            DataTable matrixExcelDataTable = new DataTable();
            matrixExcelDataTable.Columns.Add(new DataColumn(Constants.VALUE_COLUMN));
            matrixExcelDataTable.Columns.Add(new DataColumn(Constants.MATRIX_DATAFIELDID));
            matrixExcelDataTable.Columns.Add(new DataColumn(Constants.MATRIX_DATAROWINDEX));
            matrixExcelDataTable.Columns.Add(new DataColumn(Constants.MATRIX_DATACOLINDEX));

            matrixExcelDataTable.BeginLoadData();

            foreach (MatrixDataField dataField in matrixDataFieldsAsSavefields)
            {
                MatrixField rowField = model.MatrixRow.MatrixFields.Where(row => row.Id.Equals(dataField.MatrixRowId)).FirstOrDefault();
                MatrixField colField = model.MatrixColumn.MatrixFields.Where(col => col.Id.Equals(dataField.MatrixColumnId)).FirstOrDefault();

                DataFieldRendering rendering = MatrixActionView.QueryDataFieldRendering(rowField, colField);
                Excel.Range dataRange = ExcelHelper.GetRange(dataField.TargetNamedRange);

                switch (rendering)
                {
                    case DataFieldRendering.RenderDataOnCell:
                        {
                            CreateDataTableEntry(matrixExcelDataTable, dataRange.Value, dataField.Id, 1, 1);
                        }
                        break;
                    case DataFieldRendering.RenderDataVertically:
                        {
                            //int rowIndex = 0, colIndex = 0;
                            //foreach (Excel.Range cell in dataRange.Cells)
                            //{
                            //    CreateDataTableEntry(matrixExcelDataTable, cell.Value, dataField.Id, rowIndex, colIndex);
                            //    rowIndex++;
                            //}

                            object[,] objArray = (object[,])dataRange.Value2;
                            int rowCount = dataRange.Rows.Count;

                            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
                                CreateDataTableEntry(matrixExcelDataTable, objArray[rowIndex + 1, 1], dataField.Id, rowIndex, 0);
                        }
                        break;
                    case DataFieldRendering.RenderDataHorizontally:
                        {
                            //int colIndex = 0, rowIndex = 0; 
                            //foreach (Excel.Range cell in dataRange.Cells)
                            //{
                            //    CreateDataTableEntry(matrixExcelDataTable, cell.Value, dataField.Id, rowIndex, colIndex);
                            //    colIndex++;
                            //}

                            object[,] objArray = (object[,])dataRange.Value2;
                            int colCount = dataRange.Columns.Count;

                            for (int colIndex = 0; colIndex < colCount; ++colIndex)
                                CreateDataTableEntry(matrixExcelDataTable, objArray[1, colIndex + 1], dataField.Id, 0, colIndex);
                        }
                        break;
                    case DataFieldRendering.RenderDataHorizontallyAndVertically:
                        {
                            //int rowIndex = 0, colIndex = 0;
                            //Excel.Worksheet sheet = dataRange.Worksheet;
                            //foreach (Excel.Range Row in dataRange.Rows)
                            //{
                            //    foreach (Excel.Range Col in dataRange.Columns)
                            //    {
                            //        Excel.Range cell = dataRange.Cells[rowIndex + 1, colIndex + 1] as Excel.Range;
                            //        CreateDataTableEntry(matrixExcelDataTable, cell.Value, dataField.Id, rowIndex, colIndex);
                            //        colIndex++;
                            //    }
                            //    rowIndex++;
                            //    // Reset column index
                            //    colIndex = 0;
                            //}

                            object[,] objArray = (object[,])dataRange.Value2;
                            int dataRangeRowCount = dataRange.Rows.Count;
                            int dataRangeColCount = dataRange.Columns.Count;
                            for (int r = 0; r < dataRangeRowCount; r++)
                            {
                                for (int c = 0; c < dataRangeColCount; c++)
                                {
                                    CreateDataTableEntry(matrixExcelDataTable, Convert.ToString(objArray[r + 1,c + 1]), dataField.Id, r, c);
                                }
                            }

                            objArray = null;
                        }
                        break;
                }
            }

            matrixExcelDataTable.EndLoadData();
            matrixExcelDataTable.AcceptChanges();
            return matrixExcelDataTable;
        }



        /// <summary>
        /// Add Row and Column Data in MAtrix data table.
        /// </summary>
        private void AddRowColumnDataInTable(ApttusMatrixDataSet matrixDataSet, out StringBuilder LookupErrors)
        {
            LookupErrors = new StringBuilder();
            if (matrixDataSet != null)
            {
                string newlyAddedRowsFilter = Constants.MATRIX_ISROWVALUE + "= 'true'";
                DataRow[] matrixdataRows = matrixDataSet.MatrixDataTable.Select(newlyAddedRowsFilter);
                if (matrixdataRows != null && matrixdataRows.Count() > 0)
                {
                    foreach (MatrixField field in this.matrixMap.MatrixRow.MatrixFields)
                    {
                        if (field.RenderingType == MatrixRenderingType.Static)
                            continue;

                        Excel.Range rowNameRange = ExcelHelper.GetRange(field.TargetNamedRange);
                        int rowIndex = 0;
                        string fieldID = string.Empty;
                        foreach (DataRow matrixdataRow in matrixdataRows)
                        {
                            rowIndex = Convert.ToInt32(matrixdataRow[Constants.MATRIX_DATAROWINDEX]);
                            string oRowValue = Convert.ToString((rowNameRange.Cells[rowIndex + 1, 1] as Excel.Range).Value);
                            // Count is used for get value of datarow and when Add row and column simultaneously add in matrix
                            if (string.IsNullOrEmpty(matrixdataRow[Constants.MATRIX_DATACOLVALUE].ToString()))
                            {
                                DataRow[] dataColIndex = matrixDataSet.MatrixDataTable.Select(Constants.MATRIX_DATAROWINDEX + "= '" + 0 + "' and " + Constants.MATRIX_ISCOLUMNVALUE + "= '" + false + "' and " + Constants.MATRIX_DATACOLINDEX + "= '" + matrixdataRow[Constants.MATRIX_DATACOLINDEX] + "'");
                                if (dataColIndex.Length > 0)
                                    matrixdataRow[Constants.MATRIX_DATACOLVALUE] = dataColIndex[0].ItemArray[5];
                            }

                            if (field.FieldId.EndsWith(Constants.APPENDLOOKUPID))
                            {
                                string Id = string.Empty;
                                string lookupIdField = appDefManager.GetLookupIdFromLookupName(field.FieldId);

                                ApttusObject oApttusObject = appDefManager.GetAppObject(field.AppObjectUniqueID);
                                ApttusField lookupField = oApttusObject.Fields.Where(f => f.Id == lookupIdField).FirstOrDefault();

                                Id = dataManager.MatrixFindLookupNames(lookupField.LookupObject, new List<string> { Convert.ToString(oRowValue) });
                                if (!string.IsNullOrEmpty(Id))
                                    matrixdataRow[Constants.MATRIX_DATAROWVALUE] = Id;
                                else
                                {
                                    string ErrorMessage = String.Format(resourceManager.GetResource("COMMON_LookUpInvalid_Msg"), oApttusObject.Fields.Where(f => f.Id == field.FieldId).FirstOrDefault().Name, oRowValue);
                                    LookupErrors.Append(ErrorMessage);
                                    break;
                                }
                            }
                            else
                            {
                                string changeValue = string.Empty;
                                changeValue = SaveHelper.TranslateData(field.DataType, Convert.ToString(oRowValue));
                                matrixdataRow[Constants.MATRIX_DATAROWVALUE] = changeValue;
                            }
                        }
                    }
                }

                string newlyAddedColumnsFilter = Constants.MATRIX_ISCOLUMNVALUE + "= 'true'";
                DataRow[] matrixdataCols = matrixDataSet.MatrixDataTable.Select(newlyAddedColumnsFilter);
                if (matrixdataCols != null && matrixdataCols.Count() > 0)
                {
                    // Get newly inserted columns data
                    foreach (MatrixField field in this.matrixMap.MatrixColumn.MatrixFields)
                    {
                        if (field.RenderingType == MatrixRenderingType.Static)
                            continue;

                        Excel.Range columnNameRange = ExcelHelper.GetRange(field.TargetNamedRange);
                        int colIndex = 0;
                        string fieldID = string.Empty;
                        foreach (DataRow matrixdataCol in matrixdataCols)
                        {
                            colIndex = Convert.ToInt32(matrixdataCol[Constants.MATRIX_DATACOLINDEX]);
                            string oColValue = Convert.ToString((columnNameRange.Cells[1, colIndex + 1] as Excel.Range).Value);
                            // Count is used for get value of datarow and when Add row and column simultaneously add in matrix
                            if (string.IsNullOrEmpty(matrixdataCol[Constants.MATRIX_DATAROWVALUE].ToString()))
                            {
                                DataRow[] dataRowIndex = matrixDataSet.MatrixDataTable.Select(Constants.MATRIX_DATACOLINDEX + "= '" + 0 + "' and " + Constants.MATRIX_ISROWVALUE + "= '" + false + "' and " + Constants.MATRIX_DATAROWINDEX + "= '" + matrixdataCol[Constants.MATRIX_DATAROWINDEX] + "'");
                                if (dataRowIndex.Length > 0)
                                    matrixdataCol[Constants.MATRIX_DATAROWVALUE] = dataRowIndex[0].ItemArray[5];
                            }

                            if (field.FieldId.EndsWith(Constants.APPENDLOOKUPID))
                            {
                                string Id = string.Empty;
                                string lookupIdField = appDefManager.GetLookupIdFromLookupName(field.FieldId);

                                ApttusObject oApttusObject = appDefManager.GetAppObject(field.AppObjectUniqueID);
                                ApttusField lookupField = oApttusObject.Fields.Where(f => f.Id == lookupIdField).FirstOrDefault();

                                Id = dataManager.MatrixFindLookupNames(lookupField.LookupObject, new List<string> { Convert.ToString(oColValue) });
                                if (!string.IsNullOrEmpty(Id))
                                    matrixdataCol[Constants.MATRIX_DATACOLVALUE] = Id;
                                else
                                {
                                    string ErrorMessage = String.Format(resourceManager.GetResource("COMMON_LookUpInvalid_Msg"), oApttusObject.Fields.Where(f => f.Id == field.FieldId).FirstOrDefault().Name, oColValue);
                                    LookupErrors.Append(ErrorMessage);
                                    break;
                                }
                            }
                            else
                            {
                                string changeValue = string.Empty;
                                changeValue = SaveHelper.TranslateData(field.DataType, Convert.ToString(oColValue));
                                matrixdataCol[Constants.MATRIX_DATACOLVALUE] = changeValue;
                            }
                        }
                        colIndex++;
                    }
                }
                /*===*/
                matrixDataSet.MatrixDataTable.AcceptChanges();
            }
        }
    }
}
