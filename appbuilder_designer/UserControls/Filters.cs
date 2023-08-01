/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner
{

    public class Filters
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Datatype Datatype { get; set; }
        public ExpressionValueTypes[] SupportedValueTypes { get; set; }

        public Filters(string Key, string Value, Datatype dt)
        {
            this.Key = Key;
            this.Value = Value;
            this.Datatype = dt;
            UpdateSupportedValueTypes();
        }

        private void UpdateSupportedValueTypes()
        {
            switch (Datatype)
            {
                case Core.Datatype.Double:
                case Core.Datatype.Decimal:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.String:
                case Core.Datatype.Email:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Lookup:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Input, ExpressionValueTypes.Static, ExpressionValueTypes.SystemVariables, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Datatype.Composite:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Input, ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Boolean:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Picklist_MultiSelect:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Picklist:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Textarea:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Date:
                case Core.Datatype.DateTime:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.SystemVariables, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.Editable_Picklist:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static, ExpressionValueTypes.UserInput, ExpressionValueTypes.CellReference };
                    }
                    break;
                case Core.Datatype.NotSupported:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
            }
        }

        public static List<Filters> InitializeFilters()
        {
            List<Filters> filterOperators = new List<Filters>();

            // String
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.String));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.String));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.String));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.String));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.String));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.String));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.String));

            // Email
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Email));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Email));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Email));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Email));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Email));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Email));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Email));

            // Picklist
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Picklist));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Picklist));

            // Lookup
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Lookup));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Lookup));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Lookup));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Lookup));

            // Composite
            filterOperators.Add(new Filters("=", "equals", Datatype.Composite));
            filterOperators.Add(new Filters("!=", "not equal to", Datatype.Composite));
            filterOperators.Add(new Filters("in", "in", Datatype.Composite));
            filterOperators.Add(new Filters("not in", "not in", Datatype.Composite));

            // Double
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Double));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Double));
            filterOperators.Add(new Filters("<", "less than", Core.Datatype.Double));
            filterOperators.Add(new Filters("<=", "less or equal", Core.Datatype.Double));
            filterOperators.Add(new Filters(">", "greater than", Core.Datatype.Double));
            filterOperators.Add(new Filters(">=", "greater or equal", Core.Datatype.Double));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Double));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Double));

            // Date
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Date));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Date));
            filterOperators.Add(new Filters("<", "less than", Core.Datatype.Date));
            filterOperators.Add(new Filters("<=", "less or equal", Core.Datatype.Date));
            filterOperators.Add(new Filters(">", "greater than", Core.Datatype.Date));
            filterOperators.Add(new Filters(">=", "greater or equal", Core.Datatype.Date));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Date));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Date));

            // DateTime
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.DateTime));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.DateTime));
            filterOperators.Add(new Filters("<", "less than", Core.Datatype.DateTime));
            filterOperators.Add(new Filters("<=", "less or equal", Core.Datatype.DateTime));
            filterOperators.Add(new Filters(">", "greater than", Core.Datatype.DateTime));
            filterOperators.Add(new Filters(">=", "greater or equal", Core.Datatype.DateTime));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.DateTime));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.DateTime));

            // Boolean
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Boolean));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Boolean));

            // Decimal
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Decimal));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Decimal));
            filterOperators.Add(new Filters("<", "less than", Core.Datatype.Decimal));
            filterOperators.Add(new Filters("<=", "less or equal", Core.Datatype.Decimal));
            filterOperators.Add(new Filters(">", "greater than", Core.Datatype.Decimal));
            filterOperators.Add(new Filters(">=", "greater or equal", Core.Datatype.Decimal));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Decimal));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Decimal));

            // NotSupported
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("<", "less than", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("<=", "less or equal", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters(">", "greater than", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters(">=", "greater or equal", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.NotSupported));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.NotSupported));

            // TextArea
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Textarea));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Textarea));

            // MultiSelect Picklist
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Picklist_MultiSelect));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Picklist_MultiSelect));
            filterOperators.Add(new Filters("includes ('#FILTERVALUE')", "contains", Core.Datatype.Picklist_MultiSelect));
            filterOperators.Add(new Filters("excludes ('#FILTERVALUE')", "not contains", Core.Datatype.Picklist_MultiSelect));

            //Combobox
            filterOperators.Add(new Filters("=", "equals", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("!=", "not equal to", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("in", "in", Core.Datatype.Editable_Picklist));
            filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Editable_Picklist));

            return filterOperators;
        }
    }

    public class DataTableFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Datatype Datatype { get; set; }
        public ExpressionValueTypes[] SupportedValueTypes { get; set; }

        public DataTableFilter(string Key, string Value, Datatype dt)
        {
            this.Key = Key;
            this.Value = Value;
            this.Datatype = dt;
            UpdateSupportedValueTypes();
        }

        private void UpdateSupportedValueTypes()
        {
            switch (Datatype)
            {
                case Core.Datatype.Double:
                case Core.Datatype.Decimal:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.String:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Lookup:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Boolean:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Picklist_MultiSelect:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Picklist:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Textarea:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.Date:
                case Core.Datatype.DateTime:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.NotSupported:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                case Core.Datatype.EmptyField:
                    {
                        SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    }
                    break;
                default:
                    SupportedValueTypes = new ExpressionValueTypes[] { ExpressionValueTypes.Static };
                    break;
            }
        }

        public static List<DataTableFilter> InitializeFilters()
        {
            List<DataTableFilter> filterOperators = new List<DataTableFilter>();

            // String
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.String));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.String));
            //filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.String));
            //filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.String));
            //filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.String));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.String));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.String));

            // Picklist
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Picklist));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Picklist));
            //filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Picklist));
            //filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Picklist));
            //filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Picklist));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Picklist));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Picklist));

            // Lookup
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Lookup));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Lookup));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Lookup));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Lookup));

            // Double
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Double));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Double));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.Double));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.Double));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.Double));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.Double));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Double));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Double));

            // Date
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Date));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Date));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.Date));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.Date));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.Date));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.Date));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Date));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Date));

            // DateTime
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.DateTime));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.DateTime));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.DateTime));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.DateTime));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.DateTime));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.DateTime));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.DateTime));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.DateTime));

            // Boolean
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Boolean));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Boolean));

            // Decimal
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Decimal));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Decimal));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.Decimal));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.Decimal));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.Decimal));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.Decimal));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Decimal));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Decimal));

            // NotSupported
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.NotSupported));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.NotSupported));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.NotSupported));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.NotSupported));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.NotSupported));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.NotSupported));
            //filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.NotSupported));
            //filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.NotSupported));
            //filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.NotSupported));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.NotSupported));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.NotSupported));

            // TextArea
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Textarea));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Textarea));
            //filterOperators.Add(new Filters("like #FILTERVALUE%", "starts with", Core.Datatype.Textarea));
            //filterOperators.Add(new Filters("like %#FILTERVALUE%", "contains", Core.Datatype.Textarea));
            //filterOperators.Add(new Filters("like %#NOTFILTERVALUE%", "not contains", Core.Datatype.Textarea));
            //filterOperators.Add(new Filters("in", "in", Core.Datatype.Textarea));
            //filterOperators.Add(new Filters("not in", "not in", Core.Datatype.Textarea));

            // MultiSelect Picklist
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.Picklist_MultiSelect));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.Picklist_MultiSelect));
            //filterOperators.Add(new Filters("includes ('#FILTERVALUE')", "contains", Core.Datatype.Picklist_MultiSelect));
            //filterOperators.Add(new Filters("excludes ('#FILTERVALUE')", "not contains", Core.Datatype.Picklist_MultiSelect));

            //Empty Field
            filterOperators.Add(new DataTableFilter("=", "equals", Core.Datatype.EmptyField));
            filterOperators.Add(new DataTableFilter("<>", "not equal to", Core.Datatype.EmptyField));
            filterOperators.Add(new DataTableFilter("<", "less than", Core.Datatype.EmptyField));
            filterOperators.Add(new DataTableFilter("<=", "less or equal", Core.Datatype.EmptyField));
            filterOperators.Add(new DataTableFilter(">", "greater than", Core.Datatype.EmptyField));
            filterOperators.Add(new DataTableFilter(">=", "greater or equal", Core.Datatype.EmptyField));

            return filterOperators;
        }
    }

    public class ValueType
    {
        public ExpressionValueTypes Key { get; set; }
        public string Value { get; set; }
    }
}
