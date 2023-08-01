/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public class UserInputController
    {
        public ActionResult Result { get; private set; }
        public List<SearchFilterGroup> SearchFilterGroups;
        private DataManager dataManager;
        private string ActionId;
        public bool FilterSetSupported;
        public bool AutoExecutionForSearchFilters;
        private ApplicationDefinitionManager applicationDefinitionManager;
        public UserInputController(string actionId, List<SearchFilterGroup> searchFilterGroups, bool filterSetSupported = false, bool autoExecutionForSearchFilters = false)
        {
            ActionId = actionId;
            SearchFilterGroups = searchFilterGroups;
            Result = new ActionResult();
            dataManager = DataManager.GetInstance;
            FilterSetSupported = filterSetSupported;
            AutoExecutionForSearchFilters = autoExecutionForSearchFilters;
            applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        }

        public ActionResult ShowInputForm()
        {
            using (UserInputForm view = new UserInputForm(this, ActionId, SearchFilterGroups))
            {
                if (view.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Result.Status = ActionResultStatus.Success;
                else
                    Result.Status = ActionResultStatus.Failure;
            }
            //if (Model.InputActionVariables.Any(var => var.VariableType == InputVariableType.ExcelCellReference))
            //{
            //    ApttusDataSet dataSet = GetInputVariablesDataSet();
            //    foreach (InputActionVariable excelCellVariable in Model.InputActionVariables.Where(var => var.VariableType == InputVariableType.ExcelCellReference))
            //    {
            //        try
            //        {
            //            string []values = excelCellVariable.ExcelCellReference.Split(new char[] { '!' });
            //            Excel.Worksheet sheet = Globals.ThisAddIn.Application.Sheets[values[0]];
            //            Excel.Range cell = sheet.Range[values[1]];
            //            AddDataInDataSet(dataSet, excelCellVariable, cell.Value);
            //        }
            //        catch (Exception ex)
            //        {
            //            ExceptionLogHelper.ErrorLog(ex);
            //            Result.Status = ActionResultStatus.Failure;
            //            break;
            //        }
            //    }
            //}
            return Result;
        }

        /// <summary>
        /// This function creates an ApttusDataSet for InputAction Variables. If the DataSet exists it will return an existing dataset, if the dataset doesn't exist, it will create one and return the same.
        /// </summary>
        /// <returns></returns>
        internal ApttusDataSet GetInputVariablesDataSet()
        {
            List<ApttusDataSet> dataSets = dataManager.GetDatasetsByNames(new string[] { Constants.USERINPUT_DATASETNAME });
            if (dataSets.Count > 0)
                return dataSets.ElementAt(0);

            //DataSet doesn't exist, so create a data for input action.
            ApttusDataSet dataSet = new ApttusDataSet
            {
                ColumnNames = new List<string>() { Constants.KEY_COLUMN, Constants.VALUE_COLUMN, Constants.Display_COLUMN },
                DataTable = new DataTable(Constants.USERINPUT_DATASETNAME),
                Name = Constants.USERINPUT_DATASETNAME,
                Id = Guid.NewGuid()
            };

            foreach (string colname in dataSet.ColumnNames)
                dataSet.DataTable.Columns.Add(colname);

            dataSet.DataTable.PrimaryKey = new DataColumn[] { dataSet.DataTable.Columns[0] };

            dataManager.AddData(dataSet);

            return dataSet;
        }

        public string CreateKey(int sequenceNo, string fieldId)
        {
            return new StringBuilder(ActionId).Append(Constants.DOT).Append(sequenceNo).Append(Constants.DOT).Append(fieldId).ToString();
        }

        /// <summary>
        /// Adds data in InputAction DataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="filter"></param>
        /// <param name="data"></param>
        internal void AddDataInDataSet(ApttusDataSet dataSet, SearchFilter filter, string data)
        {
            string key = CreateKey(filter.SequenceNo, filter.FieldId);
            DataRow dr = dataSet.DataTable.Rows.Find(key);
            if (dr == null)
            {
                dr = dataSet.DataTable.NewRow();
                dr[Constants.KEY_COLUMN] = key;
                dataSet.DataTable.Rows.Add(dr);
            }

            ApttusField af = applicationDefinitionManager.GetField(filter.AppObjectUniqueId, filter.FieldId);

            if (af.Datatype == Datatype.Lookup)
            {
                string[] values = data.Split(';');
                dr[Constants.VALUE_COLUMN] = values[0];
                dr[Constants.Display_COLUMN] = values[1];
            }
            else
                dr[Constants.VALUE_COLUMN] = data;

        }

        internal ApttusDataSet InputActionDataSet
        {
            get
            {
                ApttusDataSet dataSet = null;
                List<ApttusDataSet> dataSets = dataManager.GetDatasetsByNames(new string[] { Constants.USERINPUT_DATASETNAME });
                if (dataSets.Count > 0)
                    dataSet = dataSets[0];
                return dataSet;
            }
        }
    }
}
