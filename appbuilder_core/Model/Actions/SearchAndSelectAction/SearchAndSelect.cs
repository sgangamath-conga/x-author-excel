/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class SearchAndSelect : Action
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public Guid TargetObject { get; set; }
        public string RecordType { get; set; }
        public string PageSize { get; set; }

        public bool AllowSavingFilters { get; set; }

        // Selected SearchFields List
        public List<SearchField> SearchFields { get; set; }

        // Selected ResultFields List
        public List<ResultField> ResultFields { get; set; }

        //Complete List of SearchFields;
        [XmlIgnore()]
        public List<SearchField> AllSearchFields { get; set; }
        [XmlIgnore()]
        public List<ResultField> AllResultFields { get; set; }

        // Selected SearchGroups
        public List<SearchFilterGroup> SearchFilterGroups { get; set; }
        public SearchAndSelect()
        {
            Type = Constants.SEARCH_AND_SELECT_ACTION;
            AllSearchFields = new List<SearchField>();
            AllResultFields = new List<ResultField>();
        }

        public SearchAndSelect(SearchAndSelect obj)
        {
            Id = obj.Id;
            Name = obj.Name;
            Type = Constants.SEARCH_AND_SELECT_ACTION;

            TargetObject = obj.TargetObject;
            RecordType = obj.RecordType;
            PageSize = obj.PageSize;

            AllSearchFields = new List<SearchField>();
            AllResultFields = new List<ResultField>();
        }

        //public override ActionResult Execute()
        //{
        //    throw new NotImplementedException();
        //}

        // Fill Objects
        public List<ApttusObject> GetAppObjects()
        {
            //return ApplicationDefinitionManager.GetInstance.GetParentAndChildObjects();
            return ApplicationDefinitionManager.GetInstance.GetAllObjects();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public List<SearchField> GetSearchFieldsList(Guid objectUniqueId)
        {
            AllSearchFields.Clear();
            AllSearchFields = null;
            AllSearchFields = new List<SearchField>();

            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(objectUniqueId);
            foreach (ApttusField field in apttusObject.Fields)
            {
                // Do not include ID and Textarea fields
                if (field.Id == apttusObject.IdAttribute || field.Datatype == Datatype.Textarea || field.Datatype == Datatype.Rich_Textarea ||
                    field.Datatype == Datatype.Attachment)
                    continue;

                if (CRMContextManager.Instance.ActiveCRM == CRM.AIC && field.Datatype == Datatype.Picklist_MultiSelect)
                    continue;
                
                    // Allow formula data type for string ,numeric and boolean for where clause.
                    if (field.Datatype == Datatype.Formula)
                {
                    if (!string.IsNullOrEmpty(field.FormulaType))   
                    {
                        if (ExpressionBuilderHelper.GetDataTypeFromFormulaType(field.FormulaType) == Datatype.AnyType)
                            continue;
                    }
                    else
                        continue;
                }
                AllSearchFields.Add(new SearchField(field));
            }
            return AllSearchFields;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public List<ResultField> GetResultFieldsList(Guid objectUniqueId)
        {
            AllResultFields.Clear();
            AllResultFields = null;
            AllResultFields = new List<ResultField>();

            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(objectUniqueId);
            foreach (ApttusField field in apttusObject.Fields)
            {
                if (field.Id == apttusObject.IdAttribute || field.Datatype == Datatype.Rich_Textarea || field.Datatype == Datatype.Attachment)
                    continue;
                AllResultFields.Add(new ResultField(field));
            }
            return AllResultFields;

        }

        public List<string> GetInputObjects()
        {
            List<string> objectSet = new List<string>();
            foreach (SearchFilterGroup filterGroup in this.SearchFilterGroups)
                ExpressionBuilderHelper.GetFilterGroupObjects(filterGroup, objectSet);

            return objectSet.ToList();
        }

        

        /// <summary>
        /// Saves action and its properties to the config file.
        /// </summary>
        /// <returns></returns>
        public bool SaveAction()
        {
            bool blnResultFields,blnSearchFields = false;
            blnSearchFields = AddSelectedSearchFields();
            blnResultFields = AddSelectedResultFields();
            if (!blnSearchFields || !blnResultFields)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("CORESEARCHNSELECT_SaveAction_ErrorMsg"), resourceManager.GetResource("CORESEARCHNSELECT_SaveActionCap_ErrorMsg"));
                return true;
            }
            else
            {
                //If there is no Id for this action which means the action needs to be added into the config.
                //If there is a valid Id availabe, the action is being edited and nothing needs to be specified into the config.
                if (Id == null)
                    ConfigurationManager.GetInstance.AddAction(this);


                return false;
            }
        }

        /// <summary>
        /// returns selected search fields from list
        /// </summary>
        private bool AddSelectedSearchFields()
        {
            int sequenceNumber = 1;
            if (SearchFields != null)
            {
                SearchFields.Clear();
                SearchFields = null;
            }

            SearchFields = new List<SearchField>();

            IEnumerable<SearchField> selectedFieldsInSearchGrid = from searchField in AllSearchFields
                                                                  where searchField.IsSelected.Equals(true)
                                                                  select searchField;
            List<SearchField> fields = selectedFieldsInSearchGrid.ToList<SearchField>();

            if (fields.Count > 0)
            {
                //TODO - Need to look into a better way for generating sequence numbers.
                foreach (SearchField searchField in fields)
                {
                    searchField.SequenceNo = sequenceNumber;
                    ++sequenceNumber;

                    SearchFields.Add(searchField);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// returns selected result fields from the list
        /// </summary>
        private bool AddSelectedResultFields()
        {
            int sequenceNumber = 1;

            if (ResultFields != null)
            {
                ResultFields.Clear();
                ResultFields = null;
            }
            ResultFields = new List<ResultField>();

            IEnumerable<ResultField> selectedFieldsInResultGrid = from resultField in AllResultFields
                                                                  where resultField.IsSelected == true
                                                                  select resultField;
            List<ResultField> fields = selectedFieldsInResultGrid.ToList<ResultField>();

            if (fields.Count > 0)
            {
                //TODO - Need to look into a better way for generating sequence numbers.
                foreach (ResultField resultField in fields)
                {
                    resultField.SequenceNo = sequenceNumber;
                    ++sequenceNumber;

                    ResultFields.Add(resultField);
                }
            }
            else
            {
                return false;
            }
            return true;
        }


    }




    [Serializable]
    public class SearchField
    {
        //Default constructor for deserialization process.
        public SearchField()
        {
        }

        public SearchField(ApttusField field)
        {
            Datatype = field.Datatype;
            Id = field.Id;
            Label = field.Name;
            DefaultValue = String.Empty;
        }
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }

        [XmlElement("DataType")]
        public Datatype Datatype { get; set; }

        [XmlIgnore()]
        public bool IsSelected { get; set; }

        [XmlElement("DefaultValue")]
        public string DefaultValue { get; set; }

        [XmlElement("SequenceNo")]
        public int SequenceNo { get; set; }
    }

    [Serializable]
    public class ResultField
    {
        //Default constructor for deserialization process.
        public ResultField()
        {
        }

        public ResultField(ApttusField field)
        {
            Id = field.Id;
            HeaderName = field.Name;
        }

        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("Label")]
        public string HeaderName { get; set; }

        [XmlElement("IsSortField")]
        public bool IsSortField { get; set; }

        [XmlIgnore()]
        public bool IsSelected { get; set; }

        [XmlElement("SequenceNo")]
        public int SequenceNo { get; set; }
    }

}
