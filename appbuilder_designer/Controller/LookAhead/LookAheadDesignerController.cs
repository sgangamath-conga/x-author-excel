using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.AppDesigner
{
    enum FieldMapperType
    {
        RetrieveField,
        SaveField
    }
    class LookAheadDesignerController
    {
        public FieldMapper FieldMapper;
        public LookAheadFieldMapperHelper FieldMapperHelper;
        public FieldMapperType FieldMapperType;
        Microsoft.Office.Interop.Excel.Range CurrentRange;

        public LookAheadDesignerController(FieldMapper FieldMapper, FieldMapperType FieldMapperType)
        {
            this.FieldMapper = FieldMapper;
            this.FieldMapperType = FieldMapperType;
            FieldMapperHelper = new LookAheadFieldMapperHelper(this.FieldMapper);
        }
        public void ShowView()
        {
            LookAheadDesignerView lookAheadDesignerView = LookAheadDesignerView.GetInstance;
            lookAheadDesignerView.Controller = this;
            if (!lookAheadDesignerView.IsFormOpen)
            {
                lookAheadDesignerView.Show();
            }
            else
            {
                lookAheadDesignerView.DisplayMessage(ApttusResourceManager.GetInstance.GetResource("LookAheadDesignerController_ShowView_ScreenAlreadyOpen"));
            }
        }
        public LookAheadDataSource GetDataSourceOrDefault()
        {
            if (FieldMapper != null)
            {
                if (FieldMapper.Property != null) return FieldMapper.Property.DataSource;
                if (FieldMapper.PropertyCollection != null && FieldMapper.PropertyCollection.Exists(p => p.DataSource.Equals(LookAheadDataSource.Excel))) return LookAheadDataSource.Excel;
                if (FieldMapper.PropertyCollection != null && FieldMapper.PropertyCollection.Exists(p => p.DataSource.Equals(LookAheadDataSource.SearchAndSelect))) return LookAheadDataSource.SearchAndSelect;
            }
            return LookAheadDataSource.SearchAndSelect;
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string colName = String.Empty;
            int mod;

            while (dividend > 0)
            {
                mod = (dividend - 1) % 26;
                colName = Convert.ToChar(65 + mod).ToString() + colName;
                dividend = (dividend - mod) / 26;
            }

            return colName;
        }
        internal bool IsValidRowsSelected()
        {
            return CurrentRange != null && CurrentRange.Rows.Count >= 2;
        }
        internal List<string> GetAllColumnNames(Microsoft.Office.Interop.Excel.Range SelectedRange = null)
        {
            if (SelectedRange == null)
                CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            else
                CurrentRange = SelectedRange;

            List<string> ColumnNames = new List<string>();

            foreach (Microsoft.Office.Interop.Excel.Range rng in CurrentRange.Columns)
            {
                ColumnNames.Add(GetExcelColumnName(rng.Column));
            }

            return ColumnNames;
        }
        internal bool IsLookAheadConfigured()
        {
            return FieldMapper != null && FieldMapper.PropertyCollection != null && (FieldMapper.PropertyCollection.Count > 0 || FieldMapper.Property != null);
        }

        internal LookAheadProperty GetExcelLookAheadProperty()
        {
            if (FieldMapper.PropertyCollection != null && FieldMapper.PropertyCollection.Count > 0)
                return FieldMapper.PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.Excel));
            else
                return FieldMapper.Property;
        }

        internal SSActionDataSource GetSearchAndSelectLookAheadProperty()
        {
            SSActionDataSource dataSource = null;
            if (FieldMapper.PropertyCollection != null && FieldMapper.PropertyCollection.Count > 0)
            {
                LookAheadProperty prop = FieldMapper.PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.SearchAndSelect));
                if (prop != null)
                {
                    dataSource = prop as SSActionDataSource;
                    FieldMapper.Property = prop;
                }
            }
            return dataSource;
        }
        internal List<RetrieveField> GetRetrieveFields()
        {
            if (FieldMapper != null)
            {
                RetrieveFieldMapper fldMapper = FieldMapper as RetrieveFieldMapper;
                List<RetrieveField> fields = fldMapper.GetAllFieldsFromMap().ToList();
                RetrieveField field = new RetrieveField { FieldId = string.Empty, TargetColumnIndex = -1, TargetNamedRange = string.Empty };
                fields.Insert(0, field);
                return fields;
            }
            return null;
        }
        internal List<SaveFieldBound> GetSaveFields()
        {
            if (FieldMapper != null)
            {
                SaveFieldMapper fldMapper = FieldMapper as SaveFieldMapper;
                List<SaveField> resultFields = new List<SaveField>();
                resultFields.AddRange(fldMapper.GetAllFieldsFromMap());

                // Convert SaveField to SaveFieldBound, so that we can get Field Name property, showing FieldId would not make much sense for Designers
                SaveMapController saveMapcontroller = new SaveMapController(fldMapper.MappedSaveMap, null, FormOpenMode.Edit);
                List<SaveFieldBound> boundSaveFields = saveMapcontroller.GenerateBindingList(resultFields);

                // Insert a blank field as 1st field
                SaveFieldBound blankField = new SaveFieldBound { FieldId = " ", FieldName = " ", TargetColumnIndex = -1, TargetNamedRange = string.Empty };
                boundSaveFields.Insert(0, blankField);

                return boundSaveFields;
            }
            return null;
        }

        internal string GetCurrentRangeCellName()
        {
            string namedRange = string.Empty;
            namedRange = ExcelHelper.GetExcelCellName(CurrentRange);
            if (string.IsNullOrEmpty(namedRange))
            {
                namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(CurrentRange, namedRange, false);
            }
            return namedRange;
        }

        internal bool Save(LookAheadProperty property)
        {
            bool result = false;
            if (property != null)
            {
                if (FieldMapper.AddProperty(property))
                {
                    return true;
                }
            }
            return result;
        }

    }
}
