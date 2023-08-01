using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationMapController
    {
        private DataMigrationModel Model;
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private DataMigrationMigrateSheetController NameManager = DataMigrationMigrateSheetController.GetInstance;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appdefManger = ApplicationDefinitionManager.GetInstance;
        public DataMigrationMapController(DataMigrationModel model)
        {
            Model = model;
        }

        protected bool IsMigrationFieldSpecialLookupField(MigrationObject obj, MigrationField field)
        {
            if (field.DataType == Datatype.Lookup)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(obj.AppObjectUniqueId, field.FieldId);
                if (apttusField != null && apttusField.LookupObject != null)
                    return Model.MigrationObjects.Exists(migrationObj => migrationObj.ObjectId == apttusField.LookupObject.Id);
                else
                    return !string.IsNullOrEmpty(field.SpecialLookupFormulaNameRange);
            }
            return false;
        }

        /// <summary>
        /// Field which is not a name field
        /// Field which is not an ID field
        /// Field which is not an ExternalID field
        /// Field which is not a special lookup field
        /// </summary>
        /// <param name="migrationObj"></param>
        /// <param name="fld"></param>
        /// <returns></returns>
        private bool IsRegularMigrationField(MigrationObject migrationObj, MigrationField fld)
        {
            ApttusObject obj = applicationDefinitionManager.GetAppObject(migrationObj.AppObjectUniqueId);
            ApttusField field = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, fld.FieldId);
            return !IsMigrationFieldSpecialLookupField(migrationObj, fld)
                && fld.FieldId != obj.IdAttribute
                && !field.NameField
                && !field.ExternalId;
        }

        /// <summary>
        /// Creates a display map.
        /// </summary>
        /// <param name="migrationObj"></param>
        public void CreateDisplayMap(MigrationObject migrationObj)
        {
            RetrieveMapController retrieveMapcontroller = new RetrieveMapController(null);
            retrieveMapcontroller.Initialize(null);

            Excel.Worksheet sheet = DataMigrationExcelController.CreateWorksheet(NameManager[migrationObj.Id]);
            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(migrationObj.AppObjectUniqueId);

            int row = 2, col = 1;
            //1. Set up Name fields 
            IEnumerable<MigrationField> migrationFields = migrationObj.Fields.Where(fld => applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, fld.FieldId).NameField).OrderBy(fld => fld.FieldName);
            foreach (MigrationField field in migrationFields)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, field.FieldId);
                Excel.Range Target = sheet.Cells[row, col];

                string targetLocation = ExcelHelper.GetAddress(Target);
                string namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, namedRange);
                retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                Target = ExcelHelper.NextVerticalCell(Target, -1);
                SetFontNormalFields(Target);

                Target.Value = field.FieldName;

                Target.EntireColumn.AutoFit();
                ++col;
            }

            //2. Set up fields other than name fields.
            migrationFields = migrationObj.Fields.Where(fld => IsRegularMigrationField(migrationObj, fld)).OrderBy(fld => fld.FieldName);
            foreach (MigrationField field in migrationFields)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, field.FieldId);
                Excel.Range Target = sheet.Cells[row, col];

                string targetLocation = ExcelHelper.GetAddress(Target);
                string namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, namedRange);
                retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                Target = ExcelHelper.NextVerticalCell(Target, -1);
                SetFontNormalFields(Target);

                Target.Value = field.FieldName;

                Target.EntireColumn.AutoFit();
                ++col;
            }

            //3. Set ID Field
            Excel.Range idFieldRange = null;
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, apttusObject.IdAttribute);
                Excel.Range Target = sheet.Cells[row, col];
                string targetLocation = ExcelHelper.GetAddress(Target);
                string namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(Target, namedRange);
                retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                idFieldRange = Target = ExcelHelper.NextVerticalCell(Target, -1);

                Target.Font.Bold = true;
                Target.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));
                Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(226, 107, 10));

                Target.Value = "Target Record ID";

                Target.EntireColumn.AutoFit();
                ++col;
            }

            //4. Setup special lookup formula fields
            List<MigrationField> specialLookupFields = migrationObj.Fields.Where(fld => IsMigrationFieldSpecialLookupField(migrationObj, fld)).ToList();
            foreach (MigrationField field in specialLookupFields)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, field.FieldId);
                Excel.Range Target = sheet.Cells[row, col];

                //string fieldName = field.FieldName;
                //if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                //    fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();

                //Target = ExcelHelper.NextVerticalCell(Target, -1);

                //Target.Value = fieldName;

                //SetFontLookupFields(Target, LookupFieldType.FormulaField);

                //Target.EntireColumn.AutoFit();
                CreateSpecialLookupFormulaField(field, Target);

                Excel.Range formulaColAddress = formulaColAddress = sheet.Cells[row, col + (specialLookupFields.Count + 1)];
                Target.Formula = CreateFormulaForLookUpFormulaField(migrationObj, field, formulaColAddress);

                ++col;
            }

            //5. Setup external Id field
            Excel.Range externalIdFieldRange = null;
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, migrationObj.ExternalID);
                //ApttusField apttusField = apttusObject.Fields.Where(x => x.ExternalId == true).FirstOrDefault();
                if (apttusField != null)
                {
                    Excel.Range Target = sheet.Cells[row, col];
                    string targetLocation = ExcelHelper.GetAddress(Target);
                    string namedRange = ExcelHelper.CreateUniqueNameRange();
                    ExcelHelper.AssignNameToRange(Target, namedRange);
                    retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                    externalIdFieldRange = Target = ExcelHelper.NextVerticalCell(Target, -1);

                    Target.Font.Bold = true;
                    Target.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));
                    Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(226, 107, 10));
                    Target.Value = migrationObj.ExternalID;

                    Target.EntireColumn.AutoFit();
                    ++col;
                }
            }

            //6. Setup special lookup retrieve fields
            foreach (MigrationField field in specialLookupFields)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(migrationObj.AppObjectUniqueId, field.FieldId);
                Excel.Range Target = sheet.Cells[row, col];
                string targetLocation = ExcelHelper.GetAddress(Target);
                string namedRange = ExcelHelper.CreateUniqueNameRange();

                //ExcelHelper.AssignNameToRange(Target, namedRange);
                //retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                //Target = ExcelHelper.NextVerticalCell(Target, -1);

                //SetFontLookupFields(Target, LookupFieldType.RetrieveField);

                //string fieldName = field.FieldName;
                //if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                //    fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();

                //Target.Value = fieldName;
                //Target.EntireColumn.AutoFit();

                CreateSpecialLookupRetrieveField(field, Target, namedRange);

                retrieveMapcontroller.AddField(apttusObject, apttusField, targetLocation, ObjectType.Repeating, namedRange);

                //string refColAddress = sheet.Cells[row, col - (specialLookupFields.Count + 1)].Address.Replace("$", "");
                Excel.Range formulaColAddress = sheet.Cells[row, col - (specialLookupFields.Count + 1)];
                Target.Formula = CreateFormulaForRetrieveLookupField(migrationObj, field, formulaColAddress);

                ++col;
            }

            string uniqueNameInMigrateSheet = NameManager[migrationObj.Id];

            if (idFieldRange != null)
                CreateNamedRangeForID(uniqueNameInMigrateSheet, idFieldRange, IDType.ID);

            if (externalIdFieldRange != null)
                CreateNamedRangeForID(uniqueNameInMigrateSheet, externalIdFieldRange, IDType.XID);

            retrieveMapcontroller.ApplyNameRangeToRepeatingCells(MapMode.RetrieveMap, retrieveMapcontroller.RetrieveMap.RepeatingGroups[0]);

            retrieveMapcontroller.Save(string.Format(DataMigrationConstants.DisplayMapNameFormat, migrationObj.SheetName));

            migrationObj.DisplayMapId = retrieveMapcontroller.RetrieveMap.Id;
        }        

        protected void CreateSpecialLookupFormulaField(MigrationField field, Excel.Range Target)
        {
            string fieldName = field.FieldName;
            if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();

            Target = ExcelHelper.NextVerticalCell(Target, -1);

            Target.Value = fieldName;

            SetFontLookupFields(Target, LookupFieldType.FormulaField);

            Target.EntireColumn.AutoFit();
        }

        protected void CreateSpecialLookupRetrieveField(MigrationField field, Excel.Range Target, string namedRange)
        {
            ExcelHelper.AssignNameToRange(Target, namedRange);

            Target = ExcelHelper.NextVerticalCell(Target, -1);

            SetFontLookupFields(Target, LookupFieldType.RetrieveField);

            string fieldName = field.FieldName;
            if (!string.IsNullOrEmpty(fieldName) && fieldName.EndsWith(Constants.APPENDLOOKUPNAME))
                fieldName = new StringBuilder(fieldName).Replace("(", string.Empty).Replace(")", string.Empty).ToString();

            Target.Value = fieldName;
            Target.EntireColumn.AutoFit();
        }

        protected string CreateFormulaForRetrieveLookupField(MigrationObject obj, MigrationField field, Excel.Range formulaColAddress)
        {
            // Get Object Name
            string refColAddress = formulaColAddress.Address.Replace("$", "");

            ApttusField apttusField = applicationDefinitionManager.GetField(obj.AppObjectUniqueId, field.FieldId);
            string sheetName;
            string StandaraFormula = "IFERROR(INDEX({0}.ID,MATCH({1},{0}.XID,0),1),{2})";
            string Formula = "=" + StandaraFormula;
            int cnt = 0;
            foreach (MigrationObject migrationObject in Model.MigrationObjects.Where(x => x.ObjectId == apttusField.LookupObject.Id).ToList())
            {
                cnt++;
                sheetName = migrationObject.SheetName;
                Formula = String.Format(Formula, sheetName, refColAddress, cnt == Model.MigrationObjects.Where(x => x.ObjectId == apttusField.LookupObject.Id).Count() ? "\"\"" : StandaraFormula);
            }
            string specialLookupFormulaNameRange = ExcelHelper.CreateUniqueNameRange();
            ExcelHelper.AssignNameToRange(formulaColAddress, specialLookupFormulaNameRange);
            field.SpecialLookupFormulaNameRange = specialLookupFormulaNameRange;

            return Formula;
        }

        protected string CreateFormulaForLookUpFormulaField(MigrationObject obj, MigrationField field, Excel.Range formulaColAddress)
        {
            // Get Object Name
            string refColAddress = formulaColAddress.Address.Replace("$", "");

            ApttusField apttusField = applicationDefinitionManager.GetField(obj.AppObjectUniqueId, field.FieldId);
            string sheetName;
            string StandaraFormula = "IFERROR(INDEX({0}.XID,MATCH({1},{0}.ID,0),1),{2})";
            string Formula = "=" + StandaraFormula;
            int cnt = 0;
            foreach (MigrationObject migrationObject in Model.MigrationObjects.Where(x => x.ObjectId == apttusField.LookupObject.Id).ToList())
            {
                cnt++;
                sheetName = migrationObject.SheetName;
                Formula = String.Format(Formula, sheetName, refColAddress, cnt == Model.MigrationObjects.Where(x => x.ObjectId == apttusField.LookupObject.Id).Count() ? "\"\"" : StandaraFormula);
            }
            string specialLookupFormulaNameRange = ExcelHelper.CreateUniqueNameRange();
            ExcelHelper.AssignNameToRange(formulaColAddress, specialLookupFormulaNameRange);
            field.SpecialLookupFormulaNameRange = specialLookupFormulaNameRange;

            return Formula;
        }

        private void CreateNamedRangeForID(string uniqueNameInMigrateSheet, Excel.Range Target, IDType idType)
        {
            Excel.Worksheet sheet = Target.Worksheet;
            Excel.Range range = sheet.Range[Target, ExcelHelper.NextVerticalCell(Target, 2)];
            ExcelHelper.AssignNameToRange(range, uniqueNameInMigrateSheet + "." + idType.ToString());
        }

        private void SetFontLookupFields(Excel.Range Target, LookupFieldType fieldType)
        {
            if (fieldType == LookupFieldType.FormulaField)
            {
                Target.Font.Bold = true;
                Target.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));
                Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(196, 215, 155));
            }
            else if (fieldType == LookupFieldType.RetrieveField)
            {
                Target.Font.Bold = true;
                Target.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));
                Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(0, 176, 80));
            }
        }

        private void SetFontNormalFields(Excel.Range Target)
        {
            Target.Font.Bold = true;
            Target.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255, 255, 255));
            Target.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(83, 141, 213));
        }

        public void CreateSaveMap(MigrationObject migrationObject)
        {
            SaveMap saveMapModel = new SaveMap();
            SaveMapController saveMapController = new SaveMapController(saveMapModel, null, FormOpenMode.Create);

            List<MigrationField> saveFields = migrationObject.Fields;
            if (saveFields.Count == 0)
                return;

            SaveMapRetrieveFieldController controller = new SaveMapRetrieveFieldController(saveMapController.Model, null);
            SaveMapRetrieveMap sm = new SaveMapRetrieveMap();

            sm.RetrieveMapId = migrationObject.DisplayMapId;
            sm.RetrieveMapName = configManager.RetrieveMaps.Find(rMap => rMap.Id == migrationObject.DisplayMapId).Name;

            controller.RetrieveMapSelectionChange(sm);

            var r1 = (from f in saveFields
                      from r in controller.Model
                      where r.RetrieveFieldId.Equals(f.FieldId)
                      select r);

            foreach (SaveMapRetrieveField r in r1)
                r.Included = true;

            controller.AddRetrieveFieldsToSaveMap();
            saveMapController.UpdateModel(saveMapController.Model.SaveFields);

            saveMapController.Save(string.Format(DataMigrationConstants.SaveMapNameFormat, migrationObject.SheetName), true);

            migrationObject.SaveMapId = saveMapModel.Id;
        }        
    }
}
