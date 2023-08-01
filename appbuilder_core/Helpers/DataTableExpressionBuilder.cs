using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Apttus.XAuthor.Core
{
    interface IValueType
    {
        string EvaluateAndGetFilterValue(SearchFilter searchFilter);
    }

    public class DataTableExpressionBuilder
    {
        private static ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private StringBuilder ParsedDataTableExpression;
        public ApttusObject ApttusObject { get; private set; }

        private List<SearchFilterGroup> SearchFilterGroups;
        
        private const string OR_CLAUSE = " OR ";
        private const string AND_CLAUSE = " AND ";

        public DataTableExpressionBuilder(ApttusObject obj)
        {
            ApttusObject = obj;
            ParsedDataTableExpression = new StringBuilder(100);
        }

        public ApttusObject GetSearchFilterObject(SearchFilter searchFilter)
        {
            ApttusObject apttusObject = null;
            if (searchFilter != null && !string.IsNullOrEmpty(searchFilter.FieldId))
                apttusObject = appDefManager.GetAppObject(searchFilter.AppObjectUniqueId);
            
            return apttusObject;
        }

        public ApttusField GetSearchFilterField(SearchFilter searchFilter)
        {
            ApttusField apttusField = null;
            if (searchFilter != null && !string.IsNullOrEmpty(searchFilter.FieldId))
            {
                ApttusObject apttusObject = ApplicationDefinitionManager.GetInstance.GetAppObject(searchFilter.AppObjectUniqueId);
                string serachFilterFieldId = searchFilter.FieldId;

                apttusField = apttusObject.Fields.FirstOrDefault(f => f.Id == serachFilterFieldId);
                //Incase of relational field, last index will be actual field name
                if (apttusField == null && serachFilterFieldId.IndexOf(Constants.DOT) != -1)
                {
                    string[] splitFieldId = serachFilterFieldId.Split(new char[] { '.' });
                    serachFilterFieldId = splitFieldId[splitFieldId.Length - 1];
                    apttusField = apttusObject.Fields.FirstOrDefault(f => f.Id == serachFilterFieldId);
                }
                //In case of Empty Columns apttusField will be null.
                int targetColumnIndex;
                if (apttusField == null && int.TryParse(searchFilter.FieldId, out targetColumnIndex))
                {
                    //For Empty Field, Create a dummy ApttusField which is not part of ApttusObject
                    apttusField = new ApttusField { Datatype = Datatype.EmptyField, Id = targetColumnIndex.ToString(), Name = searchFilter.FieldId, Updateable = false, Visible = true };
                }
            }
            return apttusField;
        }

        public bool EvaluateExpression(List<SearchFilterGroup> searchFilterGroups, out string evaluatedExpression)
        {
            evaluatedExpression = string.Empty;

            if (!(searchFilterGroups != null && searchFilterGroups.Count == 1))
                return false;

            SearchFilterGroups = searchFilterGroups;

            string filterLogic = string.Empty;
            SearchFilterGroup filterGroup = searchFilterGroups[0];

            if (String.IsNullOrEmpty(filterGroup.FilterLogicText))
                filterLogic = GetDefaultFilterLogic(filterGroup);
            else
                filterLogic = filterGroup.FilterLogicText;

            string RegExPattern = @"(and)+|(or)+|\(|\)|[0-9]+";
            int sequenceNo;

            foreach (Match matchFilterLogicChar in Regex.Matches(filterLogic, RegExPattern, RegexOptions.IgnoreCase))
            {
                if (matchFilterLogicChar.Value == Constants.OPEN_BRACKET)
                    ParsedDataTableExpression.Append(Constants.OPEN_BRACKET);

                else if (int.TryParse(matchFilterLogicChar.Value, out sequenceNo))
                    ParsedDataTableExpression.Append(EvaluateFilter(filterGroup.Filters[sequenceNo - 1]));

                else if (matchFilterLogicChar.Value.ToUpper() == Constants.AND)
                    ParsedDataTableExpression.Append(AND_CLAUSE);

                else if (matchFilterLogicChar.Value.ToUpper() == Constants.OR)
                    ParsedDataTableExpression.Append(OR_CLAUSE);

                else if (matchFilterLogicChar.Value == Constants.CLOSE_BRACKET)
                    ParsedDataTableExpression.Append(Constants.CLOSE_BRACKET);
            }

            evaluatedExpression = ParsedDataTableExpression.ToString();

            return true;
        }

        private string GetLHSField(SearchFilter searchFilter)
        {
            return new StringBuilder(searchFilter.FieldId).Append(Constants.SPACE).Append(searchFilter.Operator).Append(Constants.SPACE).ToString();
        }

        private string EvaluateFilter(SearchFilter searchFilter)
        {
            string resultExpression = null;
            ParsedDataTableExpression.Append(GetLHSField(searchFilter));

            if (searchFilter.ValueType == ExpressionValueTypes.Static)
                resultExpression = new StaticValueType().EvaluateAndGetFilterValue(searchFilter);
           
            return resultExpression;
        }

        private string GetDefaultFilterLogic(SearchFilterGroup filterGroup)
        {
            string result = string.Empty;
            if (filterGroup != null && filterGroup.Filters != null && filterGroup.Filters.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Constants.OPEN_BRACKET);
                sb.Append(string.Join((Constants.SPACE + Constants.AND + Constants.SPACE),
                    (from sf in filterGroup.Filters
                     select sf.SequenceNo.ToString())));
                sb.Append(Constants.CLOSE_BRACKET);

                result = sb.ToString();
            }
            return result;
        }
    }

    internal class BaseValueType
    {
        protected BaseValueType()
        {

        }

        protected string TranslateData(Datatype dataType, string value, string format)
        {
            switch (dataType)
            {
                case Datatype.Date:
                    {
                        DateTime result;
                        if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                            value = result.ToOADate().ToString();
                    }
                    break;
                default:
                    break;
            }
            return value;
        }
    }

    internal class StaticValueType : BaseValueType, IValueType
    {
        internal StaticValueType()
        {

        }

        public string EvaluateAndGetFilterValue(SearchFilter searchFilter)
        {
            ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(searchFilter.AppObjectUniqueId);
            ApttusField field = obj.Fields.Find(fld => fld.Id == searchFilter.FieldId);

            Datatype fieldDataType;
            bool appendQuote = false;
            if (field != null)
            {
                fieldDataType = field.Datatype;
                appendQuote = fieldDataType.Equals(Datatype.DateTime) || fieldDataType.Equals(Datatype.Double)
                                   || fieldDataType.Equals(Datatype.Decimal) || fieldDataType.Equals(Datatype.Boolean) ? false : true;

            }
            else //For Empty field, apttusfield will be null.
            {
                fieldDataType = GetFieldDataTypeOnSearchFilterValue(searchFilter.Value);
                appendQuote = fieldDataType.Equals(Datatype.DateTime) || fieldDataType.Equals(Datatype.Double)
                                   || fieldDataType.Equals(Datatype.Decimal) || fieldDataType.Equals(Datatype.Boolean) ? false : true;
            }
            bool invalidValue;
            string value = Utils.GetValidValue(searchFilter.Value, fieldDataType, appendQuote, StringOperations.Default, out invalidValue);

            if (fieldDataType == Datatype.Date)
                value = TranslateData(fieldDataType, searchFilter.Value, searchFilter.SearchFilterLabel);

            return value;
        }

        private Datatype GetFieldDataTypeOnSearchFilterValue(string value)
        {
            Datatype dataType = Datatype.String;
            int result;
            double dResult;
            DateTime dateTime;
            bool bResult;

            if (string.IsNullOrEmpty(value))
                return dataType;
            else if (int.TryParse(value, out result))
                dataType = Datatype.Decimal;
            else if (double.TryParse(value, out dResult))
                dataType = Datatype.Double;
            else if (DateTime.TryParse(value, out dateTime))
                dataType = Datatype.Date;
            else if (bool.TryParse(value, out bResult))
                dataType = Datatype.Boolean;
            else
                dataType = Datatype.String;
            return dataType;
        }
    }    
}
