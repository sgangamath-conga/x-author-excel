using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Data;

namespace Apttus.XAuthor.AppRuntime
{
    class DataSetActionController
    {
        private DataSetActionModel Model;
        public string[] InputDataNames { get; set; }
        public string OutputDataName { get; set; }
        private DataManager dataManager;
        private DataRowComparer comparer;
        private int no;

        public DataSetActionController(DataSetActionModel model)
        {
            Model = model;
            dataManager = DataManager.GetInstance;
            comparer = new DataRowComparer();
        }

        private void DoUnion(List<ApttusDataSet> dataSets, ApttusDataSet outputDataSet)
        {
            if (dataSets.Count == 1)
                outputDataSet.DataTable = dataSets[0].DataTable;
            else if (dataSets.Count == 2)
            {
                IEnumerable<DataRow> rows = dataSets[0].DataTable.AsEnumerable().Union(dataSets[1].DataTable.AsEnumerable(), comparer);
                if (rows.LongCount() > 0)
                    outputDataSet.DataTable = rows.CopyToDataTable();
                else
                {
                    outputDataSet.DataTable = new DataTable();
                    foreach (DataColumn dc in dataSets[0].DataTable.Columns)
                        outputDataSet.DataTable.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
        }

        private void DoIntersection(List<ApttusDataSet> dataSets, ApttusDataSet outputDataSet)
        {
            if (dataSets.Count == 1)
                outputDataSet.DataTable = dataSets[0].DataTable;
            else if (dataSets.Count == 2)
            {
                IEnumerable<DataRow> rows = dataSets[0].DataTable.AsEnumerable().Intersect(dataSets[1].DataTable.AsEnumerable(), comparer);
                if (rows.LongCount() > 0)
                    outputDataSet.DataTable = rows.CopyToDataTable();
                else
                {
                    outputDataSet.DataTable = new DataTable();
                    foreach (DataColumn dc in dataSets[0].DataTable.Columns)
                        outputDataSet.DataTable.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
        }

        public ActionResult Execute()
        {
            ActionResult res = new ActionResult();
            res.Status = ActionResultStatus.Pending_Execution;
            try
            {
                List<ApttusDataSet> dataSets = dataManager.GetDatasetsByNames(InputDataNames);
                if (dataSets.Count == 0)
                {
                    res.Status = ActionResultStatus.Success;
                    return res;
                }

                ApttusDataSet dataSet = new ApttusDataSet();
                dataSet.Id = Guid.NewGuid();
                dataSet.Name = OutputDataName;
                dataSet.AppObjectUniqueID = Model.TargetObject;
                dataSet.Parent = dataManager.GetParentDataSetId(Model.TargetObject);

                switch (Model.Operation)
                {
                    case DataSetOperation.OR:
                        DoUnion(dataSets, dataSet);
                        break;
                    case DataSetOperation.AND:
                        DoIntersection(dataSets, dataSet);
                        break;
                }
                dataManager.AddData(dataSet);

                res.Status = ActionResultStatus.Success;
            }
            catch (Exception ex)
            {
                res.Status = ActionResultStatus.Failure;
                ExceptionLogHelper.ErrorLog(ex);
            }
            return res;
        }
    }

    class DataRowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(System.Data.DataRow x, System.Data.DataRow y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return ((string)x[Constants.ID_ATTRIBUTE]) == ((string)y[Constants.ID_ATTRIBUTE]);
        }

        public int GetHashCode(System.Data.DataRow obj)
        {
            //if (Object.ReferenceEquals(obj, null)) return 0;
            //int hashCode = obj[0] == null ? 0 : obj[Constants.ID_ATTRIBUTE].GetHashCode();
            int hashCode = obj[Constants.ID_ATTRIBUTE].GetHashCode();
            return hashCode;
        }
    }
}
