/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{

    public class RetrieveHelper
    {
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static ConfigurationManager configManager = ConfigurationManager.GetInstance;

        #region "POPULATE CELL - INDEPENDENT"

        public static void PopulateCellIndependent(object Value, ApttusObject independentApttusObject, List<RetrieveField> retrieveFields, RetrieveField retrieveField, bool assignValue)
        {
            ApttusField apttusField = applicationDefinitionManager.GetField(retrieveField.AppObject, retrieveField.FieldId);

            // Dependent Picklist Scenario - Calculate the location of Controlling Picklist which is referred as ReferenceNamedRange
            Excel.Range ReferenceNamedRange = null;
            string ReferenceIndependentCellFieldId = string.Empty;
            RetrieveHelper.GetValidationInputForDependentPicklistIndependent(retrieveFields, null, apttusField,
                        ref ReferenceNamedRange, ref ReferenceIndependentCellFieldId);

            // Record Type Scenario - Calculate the location of Record Type (Name) column which is referred as RecordTypeNamedRange
            Excel.Range RecordTypeNamedRange = null;
            RetrieveHelper.GetValidationInputForRecordTypeIndependent(independentApttusObject, retrieveFields, null, apttusField,
                ref RecordTypeNamedRange);

            ExcelHelper.PopulateCell(ExcelHelper.GetRange(retrieveField.TargetNamedRange), apttusField, Value, assignValue,
                new CellValidationInput
                {
                    ObjectId = applicationDefinitionManager.GetAppObject(retrieveField.AppObject).Id,
                    ObjectType = ObjectType.Independent,
                    ReferenceNamedRange = ReferenceNamedRange,
                    ControllingPicklistFieldId = ReferenceIndependentCellFieldId,
                    RecordTypeNamedRange = RecordTypeNamedRange
                });
        }

        public static void PopulateCellIndependent(object Value, ApttusObject independentApttusObject, List<SaveField> saveFields, SaveField saveField, bool assignValue)
        {
            ApttusField apttusField = applicationDefinitionManager.GetField(saveField.AppObject, saveField.FieldId);

            // Dependent Picklist Scenario - Calculate the location of Controlling Picklist which is referred as ReferenceNamedRange
            Excel.Range ReferenceNamedRange = null;
            string ReferenceIndependentCellFieldId = string.Empty;
            RetrieveHelper.GetValidationInputForDependentPicklistIndependent(null, saveFields, apttusField,
                        ref ReferenceNamedRange, ref ReferenceIndependentCellFieldId);

            // Record Type Scenario - Calculate the location of Record Type (Name) column which is referred as RecordTypeNamedRange
            Excel.Range RecordTypeNamedRange = null;
            RetrieveHelper.GetValidationInputForRecordTypeIndependent(independentApttusObject, null, saveFields, apttusField,
                ref RecordTypeNamedRange);

            ExcelHelper.PopulateCell(ExcelHelper.GetRange(saveField.TargetNamedRange), apttusField, Value, assignValue,
                new CellValidationInput
                {
                    ObjectId = applicationDefinitionManager.GetAppObject(saveField.AppObject).Id,
                    ObjectType = ObjectType.Independent,
                    ReferenceNamedRange = ReferenceNamedRange,
                    ControllingPicklistFieldId = ReferenceIndependentCellFieldId,
                    RecordTypeNamedRange = RecordTypeNamedRange
                });
        }

        public static bool GetValidationInputForDependentPicklistIndependent(List<RetrieveField> independentRetrieveFields, List<SaveField> independentSaveFields,
            ApttusField apttusField, ref Excel.Range ReferenceNamedRange, ref string ReferenceIndependentCellFieldId)
        {
            bool result = false;
            if (apttusField.Datatype == Datatype.Picklist && apttusField.PicklistType == PicklistType.Dependent)
            {
                // Using Retrieve Fields
                if (independentRetrieveFields != null)
                {
                    // Find Reference Name Range in case of Dependent Picklist.
                    RetrieveField ReferenceIndependentCellField = independentRetrieveFields.FirstOrDefault(f => f.FieldId == apttusField.ControllingPicklistFieldId);
                    // If field is dependent but controller field is not present on sheet
                    if (ReferenceIndependentCellField != null)
                    {
                        ReferenceIndependentCellFieldId = ReferenceIndependentCellField.FieldId;
                        ReferenceNamedRange = ExcelHelper.GetRange(ReferenceIndependentCellField.TargetNamedRange);
                    }
                }
                // Using Save Fields
                else if (independentSaveFields != null)
                {
                    // Find Reference Name Range in case of Dependent Picklist.
                    SaveField ReferenceIndependentCellField = independentSaveFields.FirstOrDefault(f => f.FieldId == apttusField.ControllingPicklistFieldId);
                    // If field is dependent but controller field is not present on sheet
                    if (ReferenceIndependentCellField != null)
                    {
                        ReferenceIndependentCellFieldId = ReferenceIndependentCellField.FieldId;
                        ReferenceNamedRange = ExcelHelper.GetRange(ReferenceIndependentCellField.TargetNamedRange);
                    }
                }
            }

            return result;
        }

        public static bool GetValidationInputForRecordTypeIndependent(ApttusObject apttusObject, List<RetrieveField> independentRetrieveFields, List<SaveField> independentSaveFields,
           ApttusField apttusField, ref Excel.Range RecordTypeNamedRange)
        {
            bool result = false;

            if ((apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Picklist_MultiSelect) && apttusObject.RecordTypes.Count > 0)
            {
                ApttusField RecordTypeField = apttusObject.Fields.FirstOrDefault(f => f.RecordType);
                // Proceed if Record Type has been included in the App Definition.
                if (RecordTypeField != null)
                {
                    string RecordTypeLookupName = applicationDefinitionManager.GetLookupNameFromLookupId(RecordTypeField.Id);

                    // Using Retrieve Fields
                    if (independentRetrieveFields != null)
                    {
                        // Check if the Lookup (Name) of Record Type is rendered.
                        if (independentRetrieveFields.Exists(f => f.FieldId == RecordTypeLookupName))
                        {
                            RetrieveField RecordTypeRetrieveField = independentRetrieveFields.FirstOrDefault(f => f.FieldId == RecordTypeLookupName);
                            RecordTypeNamedRange = ExcelHelper.GetRange(RecordTypeRetrieveField.TargetNamedRange);
                        }
                    }
                    // Using Save Fields
                    else if (independentSaveFields != null)
                    {
                        // Check if the Lookup (Name) of Record Type is rendered.
                        if (independentSaveFields.Exists(f => f.FieldId == RecordTypeLookupName))
                        {
                            SaveField RecordTypeSaveField = independentSaveFields.FirstOrDefault(f => f.FieldId == RecordTypeLookupName);
                            RecordTypeNamedRange = ExcelHelper.GetRange(RecordTypeSaveField.TargetNamedRange);
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region "POPULATE CELL - REPEATING"

        public static void PopulateCellRepeating(ApttusObject repeatingApttusObject, ApttusField apttusField, List<RetrieveField> retrieveFields, RetrieveField retrieveField, Excel.Range repeatingRange, int loadedRows)
        {
            Excel.Range TargetColumnRange = ExcelHelper.GetColumnRange(ExcelHelper.NextVerticalCell(repeatingRange, 1), retrieveField.TargetColumnIndex);
            Excel.Range TargetColumnRangeRows = TargetColumnRange.Resize[loadedRows + 1];

            // Dependent Picklist Scenario - Calculate the location of Controlling Picklist which is referred as ReferenceNamedRange
            Excel.Range ReferenceNamedRange = null;
            string ReferenceRepeatingCellFieldId = string.Empty;
            RetrieveHelper.GetValidationInputForDependentPicklistRepeating(retrieveFields, retrieveField, apttusField,
                TargetColumnRange, ref ReferenceNamedRange, ref ReferenceRepeatingCellFieldId);

            // Record Type Scenario - Calculate the location of Record Type (Name) column which is referred as RecordTypeNamedRange
            Excel.Range RecordTypeNamedRange = null;
            RetrieveHelper.GetValidationInputForRecordTypeRepeating(repeatingApttusObject, retrieveFields, retrieveField, apttusField,
                TargetColumnRange, ref RecordTypeNamedRange);

            ExcelHelper.AddCellRangeValidation(TargetColumnRangeRows, apttusField, new CellValidationInput
            {
                ObjectId = repeatingApttusObject.Id,
                ObjectType = ObjectType.Repeating,
                LayoutType = LayoutType.Vertical,
                ReferenceNamedRange = ReferenceNamedRange,
                ControllingPicklistFieldId = ReferenceRepeatingCellFieldId,
                RecordTypeNamedRange = RecordTypeNamedRange
            });
        }

        public static void PopulateCellRepeating(ApttusObject repeatingApttusObject, ApttusField apttusField, List<RetrieveField> retrieveFields, RetrieveField retrieveField, Excel.Range repeatingRange)
        {
            Excel.Range TargetColumnRange = ExcelHelper.GetColumnRange(ExcelHelper.NextVerticalCell(repeatingRange, 1), retrieveField.TargetColumnIndex);

            // Dependent Picklist Scenario - Calculate the location of Controlling Picklist which is referred as ReferenceNamedRange
            Excel.Range ReferenceNamedRange = null;
            string ReferenceRepeatingCellFieldId = string.Empty;
            RetrieveHelper.GetValidationInputForDependentPicklistRepeating(retrieveFields, retrieveField, apttusField,
                TargetColumnRange, ref ReferenceNamedRange, ref ReferenceRepeatingCellFieldId);

            // Record Type Scenario - Calculate the location of Record Type (Name) column which is referred as RecordTypeNamedRange
            Excel.Range RecordTypeNamedRange = null;
            RetrieveHelper.GetValidationInputForRecordTypeRepeating(repeatingApttusObject, retrieveFields, retrieveField, apttusField,
                TargetColumnRange, ref RecordTypeNamedRange);

            ExcelHelper.AddCellRangeValidation(TargetColumnRange, apttusField, new CellValidationInput
            {
                ObjectId = repeatingApttusObject.Id,
                ObjectType = ObjectType.Repeating,
                LayoutType = LayoutType.Vertical,
                ReferenceNamedRange = ReferenceNamedRange,
                ControllingPicklistFieldId = ReferenceRepeatingCellFieldId,
                RecordTypeNamedRange = RecordTypeNamedRange
            });
        }

        public static bool GetValidationInputForDependentPicklistRepeating(List<RetrieveField> repeatingCellFields, RetrieveField repeatingCellField,
            ApttusField apttusField, Excel.Range TargetColumnRange, ref Excel.Range ReferenceNamedRange, ref string ReferenceRepeatingCellFieldId)
        {
            bool result = false;

            if (apttusField.Datatype == Datatype.Picklist && apttusField.PicklistType == PicklistType.Dependent)
            {
                // Find Reference Name Range in case of Dependent Picklist.
                RetrieveField ReferenceRepeatingCellField = repeatingCellFields.FirstOrDefault(f => f.FieldId == apttusField.ControllingPicklistFieldId);
                // If field is dependent but controller field is not present on sheet
                if (ReferenceRepeatingCellField != null)
                {
                    ReferenceRepeatingCellFieldId = ReferenceRepeatingCellField.FieldId;
                    ReferenceNamedRange = ExcelHelper.NextHorizontalCell(TargetColumnRange, ReferenceRepeatingCellField.TargetColumnIndex - repeatingCellField.TargetColumnIndex);
                    result = true;
                }
            }
            return result;
        }

        public static bool GetValidationInputForRecordTypeRepeating(ApttusObject apttusObject, List<RetrieveField> repeatingCellFields, RetrieveField repeatingCellField,
            ApttusField apttusField, Excel.Range TargetColumnRange, ref Excel.Range RecordTypeNamedRange)
        {
            bool result = false;
            // Record Types are applicable for Multi select picklist fields as well, hence adding OR condition
            if ((apttusField.Datatype == Datatype.Picklist || apttusField.Datatype == Datatype.Picklist_MultiSelect) && apttusObject.RecordTypes.Count > 0)
            {
                ApttusField RecordTypeField = apttusObject.Fields.FirstOrDefault(f => f.RecordType);
                if (RecordTypeField != null)
                {
                    string RecordTypeLookupName = applicationDefinitionManager.GetLookupNameFromLookupId(RecordTypeField.Id);
                    // Check if the Lookup (Name) of Record Type is rendered.
                    if (repeatingCellFields.Exists(f => f.FieldId == RecordTypeLookupName))
                    {
                        RetrieveField RecordTypeRetrieveField = repeatingCellFields.FirstOrDefault(f => f.FieldId == RecordTypeLookupName);
                        RecordTypeNamedRange = ExcelHelper.NextHorizontalCell(TargetColumnRange, RecordTypeRetrieveField.TargetColumnIndex - repeatingCellField.TargetColumnIndex);
                        result = true;
                    }
                }
            }
            return result;
        }

        #endregion

        #region "NAMED RANGE VISIBILITY - HIDE UNHIDE FORMULA ROW"

        public static void UpdateNamedRangeVisibility(bool bHidden)
        {
            List<RangeMap> allRanges = configManager.GetRangeMaps();
            foreach (var range in allRanges.Where(r => r.Type == ObjectType.Repeating))
            {
                Excel.Range oRange = ExcelHelper.GetRange(range.RangeName);
                LayoutType layout = GetLayoutFromRangeMap(range);

                if (layout == LayoutType.Vertical)
                {
                    Excel.Range oFormulaRow = oRange.Rows[2];
                    if (!oFormulaRow.Hidden)
                        oFormulaRow.Hidden = bHidden;
                    else if (!bHidden)
                        oFormulaRow.Hidden = bHidden;
                }
                else
                {
                    Excel.Range oFormulaColumn = oRange.Columns[2];
                    if (!oFormulaColumn.Hidden)
                        oFormulaColumn.Hidden = bHidden;
                    else if (!bHidden)
                        oFormulaColumn.Hidden = bHidden;
                }
            }
        }

        private static LayoutType GetLayoutFromRangeMap(RangeMap range)
        {
            LayoutType result = LayoutType.Vertical;

            if (!range.RetrieveMapId.Equals(Guid.Empty))
            {
                RepeatingGroup rg = configManager.GetRepeatingGroupbyTargetNamedRange(range.RangeName);
                result = Utils.GetEnumDescription(LayoutType.Horizontal, string.Empty) == rg.Layout ? LayoutType.Horizontal : LayoutType.Vertical;
            }
            else if (!range.SaveMapId.Equals(Guid.Empty))
            {
                SaveGroup sg = configManager.GetSaveGroupbyTargetNamedRange(range.RangeName);
                result = Utils.GetEnumDescription(LayoutType.Horizontal, string.Empty) == sg.Layout ? LayoutType.Horizontal : LayoutType.Vertical;
            }
            return result;
        }

        #endregion
    }
}
