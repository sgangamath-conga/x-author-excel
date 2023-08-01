using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Concurrent;
namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationMapEditController : DataMigrationMapController
    {
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private DataMigrationMigrateSheetController NameManager = DataMigrationMigrateSheetController.GetInstance;
        private DataMigrationModel originalModel;
        private DataMigrationModel editModeModel;
        HashSet<RetrieveMap> changedRetreiveMaps = new HashSet<RetrieveMap>();
        HashSet<SaveMap> changedSaveMaps = new HashSet<SaveMap>();
        public List<MigrationObject> ModifiedObjects = new List<MigrationObject>();
        private object ExcelUILock;

        public DataMigrationMapEditController(DataMigrationModel model, object excelUILock)
            : base(model)
        {
            editModeModel = model;
            originalModel = configManager.Application.MigrationModel;
            ExcelUILock = excelUILock;
        }

        internal void UpdateDisplayMaps(List<MigrationObject> addedObjects, List<MigrationObject> removedObjects)
        {
            if (removedObjects != null)
            {
                foreach (MigrationObject obj in removedObjects.Where(x => x.CreateWorksheet))
                {
                    RemoveDisplayMap(obj);
                    ExcelHelper.DeleteWorksheet(obj.SheetName);
                }
            }

            if (addedObjects != null)
            {
                foreach (MigrationObject obj in addedObjects.Where(x => x.CreateWorksheet))
                {
                    CreateDisplayMap(obj);
                }
            }


            foreach (RetrieveMap rMap in changedRetreiveMaps)
                RetrieveMapValidator.ValidateRepeatingGroup(rMap);
        }

        internal void UpdateSaveMaps(List<MigrationObject> addedObjects, List<MigrationObject> removedObjects)
        {
            if (removedObjects != null)
            {
                foreach (MigrationObject obj in removedObjects.Where(x => x.CreateWorksheet))
                {
                    RemoveSaveMap(obj);
                }
            }

            if (addedObjects != null)
            {
                foreach (MigrationObject obj in addedObjects.Where(x => x.CreateWorksheet))
                {
                    CreateSaveMap(obj);
                }
            }

            foreach (SaveMap sMap in changedSaveMaps)
                SaveMapValidator.GetInstance.Validate<SaveMap>(sMap);
        }

        void RemoveNormalFields(List<MigrationField> removedFields)
        {
            if (removedFields == null)
                return;

            List<MigrationField> fieldstoBeRemoved = removedFields;
            var fieldsPerObjects = (from fld in fieldstoBeRemoved
                                    group fld by fld.MigrationObjectId into g
                                    select new { MigrationObjectId = g.Key, Fields = g.ToList() }).ToList();

            fieldsPerObjects.ForEach(fieldsPerObject =>
            {
                MigrationObject migrationObj = originalModel.MigrationObjects.Find(o => o.CreateWorksheet && o.Id == fieldsPerObject.MigrationObjectId);
                if (migrationObj != null)
                {
                    RetrieveMapController retrieveMapController = new RetrieveMapController(null);
                    RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == migrationObj.DisplayMapId);
                    retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                    SaveMap saveMap = configManager.SaveMaps.Find(sMap => sMap.Id == migrationObj.SaveMapId);
                    SaveMapController saveMapController = new SaveMapController(saveMap, null, FormOpenMode.Edit);

                    foreach (MigrationField field in fieldsPerObject.Fields)
                    {
                        if (IsMigrationFieldSpecialLookupField(migrationObj, field))
                            RemoveLookupFieldFromDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, field, saveMap);
                        else
                            RemoveFieldFromDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, field, saveMap);
                    }
                    lock (ExcelUILock)
                        retrieveMapController.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, retrieveMapController.RetrieveMap.RepeatingGroups[0]);

                    AddDisplayMapAndSaveMapToChangeList(displayMap, saveMap, migrationObj);
                }
            });
        }

        private void RemoveFieldFromDisplayMapAndSaveMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MigrationObject migrationObj, MigrationField field, SaveMap saveMap)
        {
            //1. Remove field from Save Map                        
            RemoveFieldFromSaveMap(saveMap, field);
            //2. Remove field from Display Map
            RemoveFieldFromDisplayMap(retrieveMapController, displayMap, field);
        }

        private void RemoveLookupFieldFromDisplayMapAndSaveMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MigrationObject migrationObj, MigrationField field, SaveMap saveMap)
        {
            RemoveFormulaFieldFromSheet(field);
            RemoveFieldFromDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, field, saveMap);
        }

        private void RemoveFieldFromSaveMap(SaveMap saveMap, MigrationField field)
        {
            SaveField saveField = saveMap.SaveFields.Find(sf => sf.FieldId == field.FieldId && sf.SaveFieldType == SaveType.RetrievedField);
            if (saveField != null)
                saveMap.SaveFields.Remove(saveField);
        }

        private void RemoveFieldFromDisplayMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MigrationField field)
        {
            RepeatingGroup rGroup = displayMap.RepeatingGroups[0];
            RetrieveField rField = rGroup.RetrieveFields.Find(rf => rf.FieldId == field.FieldId && rf.DataType == field.DataType);
            lock (ExcelUILock)
            {
                ExcelHelper.GetRange(rField.TargetNamedRange).EntireColumn.Delete();
                retrieveMapController.RemoveField(rGroup, rField);
            }
        }

        void AddNormalFields(List<MigrationField> addedFields)
        {
            if (addedFields == null)
                return;

            List<MigrationField> fieldstoBeAdded = addedFields;
            var fieldsPerObjects = (from fld in fieldstoBeAdded
                                    group fld by fld.MigrationObjectId into g
                                    select new { MigrationObjectId = g.Key, Fields = g.ToList() }).ToList();

            fieldsPerObjects.ForEach(fieldsPerObject =>
            {
                MigrationObject migrationObj = editModeModel.MigrationObjects.Find(o => o.CreateWorksheet && o.Id == fieldsPerObject.MigrationObjectId);
                if (migrationObj != null)
                {
                    RetrieveMapController retrieveMapController = new RetrieveMapController(null);
                    RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == migrationObj.DisplayMapId);
                    retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                    SaveMap saveMap = configManager.SaveMaps.Find(sMap => sMap.Id == migrationObj.SaveMapId);
                    SaveMapController saveMapController = new SaveMapController(saveMap, null, FormOpenMode.Edit);

                    foreach (MigrationField field in fieldsPerObject.Fields)
                    {
                        if (IsMigrationFieldSpecialLookupField(migrationObj, field))
                            AddLookupFieldInDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, field, saveMap);
                        else
                            AddFieldInDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, field, saveMap);
                    }
                    lock (ExcelUILock)
                        retrieveMapController.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, retrieveMapController.RetrieveMap.RepeatingGroups[0]);

                    AddDisplayMapAndSaveMapToChangeList(displayMap, saveMap, migrationObj);
                }
            });


        }

        private void AddLookupFieldInDisplayMapAndSaveMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MigrationObject migrationObject, MigrationField field, SaveMap saveMap)
        {
            RepeatingGroup repGroup = displayMap.RepeatingGroups[0];
            //Insert a new column.
            RetrieveField externalIdField = repGroup.RetrieveFields.Find(fld => fld.FieldId == migrationObject.ExternalID);
            string targetNameRangeOfExternalIDField = externalIdField.TargetNamedRange;

            ApttusObject obj = appDefManager.GetAppObject(migrationObject.AppObjectUniqueId);
            ApttusField apttusField = appDefManager.GetField(migrationObject.AppObjectUniqueId, field.FieldId);

            lock (ExcelUILock)
            {
                //Insert a column at the left side of ExternalIdfield
                Excel.Range externalIdRange = ExcelHelper.GetRange(targetNameRangeOfExternalIDField);
                externalIdRange.EntireColumn.Insert();

                //Create a formula lookup field in the newly inserted column as done above.
                Excel.Range TargetFormulaLookupField = ExcelHelper.NextHorizontalCell(externalIdRange, -1);
                CreateSpecialLookupFormulaField(field, TargetFormulaLookupField);

                //Insert a column at the right side of externalIdfield
                Excel.Range cellAfterExternalId = ExcelHelper.NextHorizontalCell(externalIdRange, 1);
                cellAfterExternalId.EntireColumn.Insert();

                //Create a retrieve lookup field in the newly inserted column as done above.
                Excel.Range TargetRetrieveLookupField = ExcelHelper.NextHorizontalCell(externalIdRange, 1);

                string namedRange = ExcelHelper.CreateUniqueNameRange();
                CreateSpecialLookupRetrieveField(field, TargetRetrieveLookupField, namedRange);

                TargetRetrieveLookupField.Formula = CreateFormulaForRetrieveLookupField(migrationObject, field, TargetFormulaLookupField);
                TargetFormulaLookupField.Formula = CreateFormulaForLookUpFormulaField(migrationObject, field, TargetRetrieveLookupField);

                retrieveMapController.AddField(obj, apttusField, ExcelHelper.GetAddress(TargetRetrieveLookupField), ObjectType.Repeating, namedRange);
            }

            RetrieveField rFieldAdded = repGroup.RetrieveFields.Find(fld => fld.FieldId == apttusField.Id && fld.DataType == apttusField.Datatype);
            AddFieldInSaveMap(obj, apttusField, saveMap, rFieldAdded);
        }

        private void AddFieldInDisplayMapAndSaveMap(RetrieveMapController retrieveMapController, RetrieveMap displayMap, MigrationObject migrationObject, MigrationField field, SaveMap saveMap)
        {
            RepeatingGroup repGroup = displayMap.RepeatingGroups[0];
            //Insert a new column.
            
            ApttusObject obj = appDefManager.GetAppObject(migrationObject.AppObjectUniqueId);
            ApttusField apttusField = appDefManager.GetField(migrationObject.AppObjectUniqueId, field.FieldId);

            RetrieveField idField = repGroup.RetrieveFields.Find(fld => fld.FieldId == obj.IdAttribute);
            string targetNameRangeOfIDField = idField.TargetNamedRange;

            lock (ExcelUILock)
            {
                ExcelHelper.GetRange(targetNameRangeOfIDField).EntireColumn.Insert();
                //Get the address of new column.
                Excel.Range fieldRange = ExcelHelper.NextHorizontalCell(ExcelHelper.GetRange(targetNameRangeOfIDField), -1);
                string address = ExcelHelper.GetAddress(fieldRange);
                //Add the field in retrieve map
                string targetNamedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(fieldRange, targetNamedRange);
                retrieveMapController.AddField(obj, apttusField, address, ObjectType.Repeating, targetNamedRange);

                Excel.Range FieldLabel = ExcelHelper.NextVerticalCell(fieldRange, -1);
                string fieldName = field.FieldName;
                if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                    fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();
                FieldLabel.Value = fieldName;
            }

            RetrieveField rFieldAdded = repGroup.RetrieveFields.Find(fld => fld.FieldId == apttusField.Id && fld.DataType == apttusField.Datatype);
            AddFieldInSaveMap(obj, apttusField, saveMap, rFieldAdded);
        }

        private void AddFieldInSaveMap(ApttusObject obj, ApttusField apttusField, SaveMap saveMap, RetrieveField rField)
        {
            SaveGroup saveGroup = saveMap.SaveGroups[0];

            SaveField saveField = new SaveField
            {
                AppObject = obj.UniqueId,
                DesignerLocation = rField.TargetLocation,
                FieldId = apttusField.Id,
                GroupId = saveGroup.GroupId,
                SaveFieldName = apttusField.Name,
                SaveFieldType = SaveType.RetrievedField,
                TargetColumnIndex = rField.TargetColumnIndex,
                TargetNamedRange = rField.TargetNamedRange,
                Type = ObjectType.Repeating
            };
            saveMap.SaveFields.Add(saveField);
        }

        private void RemoveFormulaFieldFromSheet(MigrationField field)
        {
            if (string.IsNullOrEmpty(field.SpecialLookupFormulaNameRange))
                return;

            lock (ExcelUILock)
            {
                Excel.Range formulaFieldRange = ExcelHelper.GetRange(field.SpecialLookupFormulaNameRange);
                if (formulaFieldRange != null)
                    formulaFieldRange.EntireColumn.Delete();
            }
        }

        private void RemoveSaveMap(MigrationObject obj)
        {
            SaveMap m = configManager.SaveMaps.Where(x => x.Id == obj.SaveMapId).FirstOrDefault();
            configManager.SaveMaps.Remove(m);
        }

        private void RemoveDisplayMap(MigrationObject obj)
        {
            RetrieveMap m = configManager.RetrieveMaps.Where(x => x.Id == obj.DisplayMapId).FirstOrDefault();
            configManager.RetrieveMaps.Remove(m);
            RetrieveMapController.DeleteRetrieveMapNamedRange(m);
        }

        internal void UpdateNormalFields(List<MigrationField> addedFields, List<MigrationField> removedFields)
        {
            RemoveNormalFields(removedFields);
            AddNormalFields(addedFields);
        }

        internal void UpdateSpecialLookupFields(List<MigrationField> specialLookupFields, List<MigrationField> normalLookupFields)
        {
            //A normal lookup field got converted into special lookup field, when an object got added.
            if (specialLookupFields != null)
            {
                specialLookupFields.ForEach(specialLookupField =>
                {
                    MigrationObject migrationObj = editModeModel.MigrationObjects.Find(o => o.CreateWorksheet && o.Id == specialLookupField.MigrationObjectId);
                    if (migrationObj != null)
                    {
                        RetrieveMapController retrieveMapController = new RetrieveMapController(null);
                        RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == migrationObj.DisplayMapId);
                        retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                        SaveMap saveMap = configManager.SaveMaps.Find(sMap => sMap.Id == migrationObj.SaveMapId);
                        RemoveFieldFromSaveMap(saveMap, specialLookupField);
                        RemoveFieldFromDisplayMap(retrieveMapController, displayMap, specialLookupField);

                        AddLookupFieldInDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, specialLookupField, saveMap);

                        AddDisplayMapAndSaveMapToChangeList(displayMap, saveMap, migrationObj);
                    }
                });
            }

            //Special lookup field got converted into normal lookup field, when an object is removed.
            if (normalLookupFields != null)
            {
                normalLookupFields.ForEach(normalLookupField =>
                {
                    MigrationObject migrationObj = editModeModel.MigrationObjects.Find(o => o.CreateWorksheet && o.Id == normalLookupField.MigrationObjectId);
                    if (migrationObj != null)
                    {
                        RetrieveMapController retrieveMapController = new RetrieveMapController(null);
                        RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == migrationObj.DisplayMapId);
                        retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                        SaveMap saveMap = configManager.SaveMaps.Find(sMap => sMap.Id == migrationObj.SaveMapId);
                        RemoveFieldFromSaveMap(saveMap, normalLookupField);
                        RemoveFieldFromDisplayMap(retrieveMapController, displayMap, normalLookupField);
                        RemoveFormulaFieldFromSheet(normalLookupField);

                        AddFieldInDisplayMapAndSaveMap(retrieveMapController, displayMap, migrationObj, normalLookupField, saveMap);
                        AddDisplayMapAndSaveMapToChangeList(displayMap, saveMap, migrationObj);
                    }
                });
            }
        }

        private void AddDisplayMapAndSaveMapToChangeList(RetrieveMap displayMap, SaveMap saveMap, MigrationObject migrationObject)
        {
            lock (this)
            {
                changedRetreiveMaps.Add(displayMap);
                changedSaveMaps.Add(saveMap);
                ModifiedObjects.Add(migrationObject);
            }
        }


    }
}
