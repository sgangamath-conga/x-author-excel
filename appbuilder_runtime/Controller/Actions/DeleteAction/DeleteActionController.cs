using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class DeleteActionController
    {
        private DeleteActionModel Model;
        private DataManager dataManger = DataManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private string[] InputDataNames;
        private DataTableExpressionBuilder expressionEvaluator = null;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ObjectManager objectManager = ObjectManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public DeleteActionController(DeleteActionModel model, string[] inputDataNames)
        {
            Model = model;
            InputDataNames = inputDataNames;
        }

        public ActionResult Execute()
        {
            ActionResult res = new ActionResult();
            res.Status = ActionResultStatus.Pending_Execution;

            ApttusObject obj = appDefManager.GetAppObject(Model.AppObjectUniqueId);
            ApttusDataSet dataSet = dataManger.ResolveInput(InputDataNames, obj);
            Excel.Worksheet sheet = null;
            if (dataSet == null)
            {
                res.Status = ActionResultStatus.Failure;
                ExceptionLogHelper.ErrorLog("No Dataset found for delete action.");
                return res;
            }

            try
            {
                ExcelHelper.ExcelApp.EnableEvents = false;

                SearchFilterGroup searchFilterGroup = Model.DeleteFilterGroups[0];
                
                expressionEvaluator = new DataTableExpressionBuilder(obj);

                SaveMap saveMap = configManager.SaveMaps.Find(sm => sm.Id == Model.MapId);
                if (saveMap != null)
                {
                    SaveGroup saveGroup = saveMap.SaveGroups.Find(sg => sg.GroupId == Model.SaveGroupId);
                    if (saveGroup != null)
                    {
                        DataTable excelDataTable = new DataTable();
                        Excel.Range targetNamedRange = ExcelHelper.GetRange(saveGroup.TargetNamedRange);
                        sheet = targetNamedRange.Worksheet;
                        if (ExcelHelper.IsSheetProtectedByUser(sheet))
                        {
                            ExcelHelper.UserSheetProtection(sheet, false);
                        }
                        RepeatingGroup repeatingGroup = (from rm in configManager.RetrieveMaps
                                                         from rg in rm.RepeatingGroups
                                                         where rg.TargetNamedRange == saveGroup.TargetNamedRange
                                                         select rg).FirstOrDefault();

                        if (repeatingGroup != null)
                        {
                            bool bSearchFiltersExists = searchFilterGroup.Filters != null && searchFilterGroup.Filters.Count > 0;
                            if (bSearchFiltersExists)
                                ExcelHelper.ApplyEmptyColumnNamesToSearchFilter(searchFilterGroup, repeatingGroup, targetNamedRange);

                            RetrieveField IDField = repeatingGroup.RetrieveFields.Find(rf => rf.FieldId == Constants.ID_ATTRIBUTE);
                            int nColumns = targetNamedRange.Columns.Count;

                            for (int iColumnIndex = 1; iColumnIndex <= nColumns; ++iColumnIndex)
                            {
                                RetrieveField field = repeatingGroup.RetrieveFields.Find(rf => rf.TargetColumnIndex == iColumnIndex);
                                DataColumn dataColumn = null;
                                if (field != null)
                                    dataColumn = new DataColumn(field.FieldId, GetSystemTypeFromApttusFieldType(field));
                                else //Field doesn't exist, which means it is an Empty Column
                                {
                                    //Check whether the given empty column is used in search filter so that datatype could be assigned and expression could be evaluated.
                                    string emptyColumnFieldId = string.Format(Constants.EMPTYCOLUMNFIELDIDFORMAT, iColumnIndex);
                                    dataColumn = new DataColumn(emptyColumnFieldId);

                                    if (bSearchFiltersExists)
                                    {
                                        SearchFilter searchFilter = searchFilterGroup.Filters.Find(sf => sf.FieldId == emptyColumnFieldId);
                                        if (searchFilter != null)
                                            ApplyDataTypeToDataColumn(dataColumn, searchFilter);
                                    }
                                }
                                excelDataTable.Columns.Add(dataColumn);
                            }

                            ExcelHelper.RangeToDataTableWithPreparedColumns(targetNamedRange, ref excelDataTable, LayoutType.Vertical);

                            string deleteConditionExpression;
                            expressionEvaluator.EvaluateExpression(Model.DeleteFilterGroups, out deleteConditionExpression);

                            DataRow[] rowsToDelete = excelDataTable.Select(deleteConditionExpression);

                            Dictionary<int, string> visibleRowNumbers = new Dictionary<int, string>();

                            foreach (DataRow excelDataRow in rowsToDelete)
                            {
                                string recordId = excelDataRow[obj.IdAttribute] as string;
                                if (string.IsNullOrEmpty(recordId))
                                    continue;

                                int excelDataRowIndex = excelDataTable.Rows.IndexOf(excelDataRow);
                                if (excelDataRowIndex == -1) // This condition should actually never hold true. But kept it, because accidentally, no records should get deleted.
                                    continue; 

                                int actualExcelRowIndex = targetNamedRange.Row + excelDataRowIndex + 2; //Why 2 ?? This shouldn't be a question.

                                visibleRowNumbers.Add(actualExcelRowIndex, recordId);
                            }

                            if (targetNamedRange != null && visibleRowNumbers.Count > 0)
                            {
                                string DeleteMessage = string.Empty;

                                if (Model.PromptConfirmationDialog)
                                    DeleteMessage = String.Format(resourceManager.GetResource("RUNTIMEEDITACT_RemoveRow_WarnMsg"), visibleRowNumbers.Count.ToString(),resourceManager.CRMName);

                                RuntimeEditActionController controller = new RuntimeEditActionController(null, targetNamedRange);
                                controller.RemoveRecords(DeleteMessage, targetNamedRange.Worksheet, visibleRowNumbers, dataSet, targetNamedRange.Cells[1, 1].Row, dataSet.Id, saveMap, saveGroup);
                            }
                        }
                    }
                }
                res.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                res.Status = ActionResultStatus.Failure;
            }
            finally
            {
                ExcelHelper.ExcelApp.EnableEvents = true;
                if (sheet != null && ExcelHelper.IsSheetProtectedByUser(sheet))
                {
                    ExcelHelper.UserSheetProtection(sheet, true);
                }
            }
            return res;
        }

        /// <summary>
        /// Converts CRM datatype into .Net System Type.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private Type GetSystemTypeFromApttusFieldType(RetrieveField field)
        {
            Type dataType = typeof(string);
            ApttusField apttusField = appDefManager.GetField(field.AppObject, field.FieldId);
            if (apttusField == null)
                return dataType;

            switch(apttusField.Datatype)
            {
                case Datatype.Decimal:
                case Datatype.Double:
                    dataType = typeof(Decimal);
                    break;
                case Datatype.Date:
                    dataType = typeof(Double);
                    break;
                case Datatype.Boolean:
                    dataType = typeof(Boolean);
                    break;
                default:
                    dataType = typeof(string);
                    break;
            }
            return dataType;
        }

        /// <summary>
        /// Based on the searchfilter value, apply the datatype, if the column is empty.
        /// </summary>
        /// <param name="dataColumn"></param>
        /// <param name="searchFilter"></param>
        private void ApplyDataTypeToDataColumn(DataColumn dataColumn, SearchFilter searchFilter)
        {
            Type dataType = typeof(string);

            int iResult;
            DateTime dateTime;
            double dResult;
            bool bResult;

            if (string.IsNullOrEmpty(searchFilter.Value))
                dataType = typeof(string);
            else if (int.TryParse(searchFilter.Value, out iResult))
                dataType = typeof(int);
            else if (double.TryParse(searchFilter.Value, out dResult))
                dataType = typeof(double);
            else if (DateTime.TryParse(searchFilter.Value, out dateTime))
                dataType = typeof(double);
            else if (Boolean.TryParse(searchFilter.Value, out bResult))
                dataType = typeof(bool);

            dataColumn.DataType = dataType;
        }
    }
}
