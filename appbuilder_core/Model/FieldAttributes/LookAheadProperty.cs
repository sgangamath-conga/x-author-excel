/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.Core
{
    public enum LookAheadDataSource
    {
        Query,
        Excel,
        SearchAndSelect
    }
    public abstract class LookAheadProperty
    {
        public LookAheadDataSource DataSource {
            get;
            set;
        }
        public bool RefreshData {
            get;
            set;
        }
        // only 1 prop can be active. 

        public bool IsActive {
            get;
            set;
        }

    }
    public class SSActionDataSource : LookAheadProperty
    {
        public SSActionDataSource() : base()
        {
            DataSource = LookAheadDataSource.SearchAndSelect;
            // default SS DS is active
            IsActive = true;

        }

        public string ActionID { get; set; }
        public string ActionName { get; set; }
        public ReturnDataCollection ReturnColumnData { get; set; }
        public SearchAndSelectActionMapping ObjectMapping { get; set; }
    }

    [Serializable]
    public class SearchAndSelectActionMapping
    {
        public string SearchAndSelectActionId { get; set; }
        public string RepeatingGroupTargetNamedRange { get; set; }
        public Guid SourceObject { get; set; }
        public Guid TargetObject { get; set; }
        public List<MappedField> MappedFields { get; set; }
    }

    [Serializable]
    public class MappedField
    {
        public string SourceFieldId { get; set; }
        public string TargetFieldId { get; set; }
    }

    public class ExcelDataSource : LookAheadProperty
    {
        public ExcelDataSource() : base() { DataSource = LookAheadDataSource.Excel; }
        public ExcelDataSource(bool IsMultiCol) : base() { DataSource = LookAheadDataSource.Excel; MultiCol = IsMultiCol; }

        public string SheetReference {
            get;
            set;
        }
        public List<string> GetData()
        {


            return null;
        }
        // If the data source is single col or multi column
        // capturing this info since this would help
        // building the ui.
        // this info can be derived by instpecting the range and 
        // iterate through the cols in the range but it might be costly
        // hence store it. 
        public bool MultiCol { get; set; }
        public string TargetRange {
            get;
            set;
        }


    }
    /* when user select multi col , the return col and the target field can be 
     * stored in a collection. This class hold the structure for the same
     */

    public class ReturnDataCollection
    {
        public string RetrieveMapID {
            get;
            set;
        }
        public ReturnDataCollection()
        {
            DataCollection = new List<ReturnColumnData>();
        }
        public List<ReturnColumnData> DataCollection {
            get;
            set;
        }
    }

    public class ReturnColumnData
    {

        public int TargetColumn {
            get;
            set;
        }
        public string FieldID {
            get;
            set;

        }
        public string ExcelDataSourceColumn {
            get;
            set;
        }

        public string TargetNR {
            get;
            set;
        }
        public ObjectType FldType {
            get;
            set;
        }
        public ReturnColumnData(int TarCol, string ExlDataSourceColumn, string fldId, string TgtRange, ObjectType feildType)
        {
            TargetColumn = TarCol;
            ExcelDataSourceColumn = ExlDataSourceColumn;
            FieldID = fldId;
            TargetNR = TgtRange;
            FldType = feildType;
        }

        public ReturnColumnData()
        {


        }
    }
    // used at runtime for storing the result
    public class ReturnColDataResult : ReturnColumnData
    {
        public ReturnColDataResult() : base()
        {

        }
        public string ResultData {
            get;
            set;
        }
    }
    public class ExcelMultiColumProp : ExcelDataSource
    {
        public ExcelMultiColumProp() : base()
        {
            MultiCol = true;

        }
        public string ReturnCol {

            get;
            set;
        }
        public bool IsIdField {
            get;
            set;

        }
        public string ReturnColIsIdField {
            get;
            set;

        }
        // there could be multiple cols that we can support
        // store it as a collection

        public ReturnDataCollection ReturnColumnData {
            get;
            set;
        }


    }
    public class QueryDataSource : LookAheadProperty
    {
        public QueryDataSource() : base() { DataSource = LookAheadDataSource.Query; }
        public string DisplayField {
            get;
            set;
        }

    }

    public abstract class FieldMapper
    {
        public LookAheadProperty Property {
            get;
            set;
        }
        public List<LookAheadProperty> PropertyCollection {
            get;
            set;
        }
        //  public abstract List<Object> GetAllFieldsFromMap();

        public abstract bool AddProperty(LookAheadProperty prop);

        public abstract string GetFieldName();

        public abstract bool RemoveProperty(LookAheadProperty prop);
        protected void SetActiveFlagForProp(LookAheadProperty prop)

        {
            if (PropertyCollection == null) return;
            // see if the curr prop is set as active and there is already a prop exists
            // only one prop can be active for a field .
            // turn off active flag for existing one. 
            if (prop.IsActive && PropertyCollection.Count > 0)
            {

                PropertyCollection[0].IsActive = false;
                return;
            }
            else if (PropertyCollection.Count == 0 && !prop.IsActive)
            { // before adding the prop, check if this is the first prop user set,
                // and check if the prop is based on excel data source
                // make it active if the user doesn't set the active flag.
                // by default ssaction prop are set active 
                // this is because, previous versions of XAE, only excel DS is available and it is always active

                prop.IsActive = true;
                return;
            }
            else if (PropertyCollection.Count > 1 && !prop.IsActive)
            {
                // 2 prop exists and user didn't set the active flag.
                // by default SS action prop is active

                LookAheadProperty ssprop = PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.Excel));
                if (!ssprop.IsActive)
                {
                    ssprop.IsActive = true;
                    return;
                }
            }
        }

        public virtual bool WarnInActiveProps()
        {
            // if user remove active flag from both properties
            // inform user that properties are not active

            if (PropertyCollection.Count > 1)
            {
                if (PropertyCollection.FindAll(item => !item.IsActive).Count > 1)
                {
                    return true;

                }
            }

            return false;
        }

    }
    public class RetrieveFieldMapper : FieldMapper
    {
        public RetrieveField MappedRetrieveField {
            get;
            set;
        }
        public RetrieveFieldMapper(RetrieveField fld)
        {
            MappedRetrieveField = fld;
            Property = fld.LookAheadProp;
            PropertyCollection = fld.LookAheadProps;
        }
        public RetrieveMap MappedRetrieveMap {

            get;
            set;
        }
        // for multi column, need to tie the return col
        // with a target field. 
        public List<RetrieveField> GetAllFieldsFromMap()
        {
            List<RetrieveField> flds = null;
            if (MappedRetrieveField.Type == ObjectType.Repeating)
            {
                if (MappedRetrieveMap != null)
                {
                    foreach (RepeatingGroup grp in MappedRetrieveMap.RepeatingGroups)
                    {
                        RetrieveField fld = grp.RetrieveFields.Find(item => item.TargetNamedRange.Equals(MappedRetrieveField.TargetNamedRange));
                        if (fld != null)
                        {
                            flds = grp.RetrieveFields;
                            break;
                        }
                    }
                }
            }
            else
            {

                flds = MappedRetrieveMap.RetrieveFields;
            }
            return flds;

        }
        public override bool RemoveProperty(LookAheadProperty prop)
        {
            if (MappedRetrieveField != null && PropertyCollection != null && prop != null)
            {
                PropertyCollection.RemoveAll(item => item.DataSource.Equals(prop.DataSource));

                //keeping it for backward compatibility
                if (PropertyCollection.Count > 0) // set the last value so that in the retrieve map the icon shows bold
                {
                    MappedRetrieveField.LookAheadProp = PropertyCollection[0];

                }
                else
                    MappedRetrieveField.LookAheadProp = null;


                MappedRetrieveField.LookAheadProps = PropertyCollection;
                return true;
            }
            return false;
        }
        public override string GetFieldName()
        {
            if (MappedRetrieveField != null)
                return MappedRetrieveField.FieldName;
            else
                return string.Empty;
        }
        public override bool AddProperty(LookAheadProperty prop)
        {

            if (MappedRetrieveField != null)
            {


                if (PropertyCollection == null)
                {

                    PropertyCollection = new List<LookAheadProperty>();
                }
                if (PropertyCollection.Count > 0)
                {// LH prop can not have more than one same data source allowed. ie. same field can't have 2 execl DS. 
                 // check if the same Data source exists and if so remove it

                    RemoveProperty(prop);

                }

                SetActiveFlagForProp(prop);

                /*
                // see if the curr prop is set as active and there is already a prop exists
                // only one prop can be active for a field .
                // turn off active flag for existing one. 
                if (prop.IsActive && LHPropertys.Count > 0)
                {
    
                    LHPropertys[0].IsActive = false;
                }

                // before adding the prop, check if this is the first prop user set,
                // and check if the prop is based on excel data source
                // make it active if the user doesn't set the active flag.
                // by default ssaction prop are set active 
                // this is because, previous versions of XAE, only excel DS is available and it is always active
                
                
                if (LHPropertys.Count ==0 && !prop.IsActive )
                {
                    prop.IsActive = true;
                }*/


                PropertyCollection.Add(prop);

                Property = prop;
                MappedRetrieveField.LookAheadProp = prop;
                MappedRetrieveField.LookAheadProps = PropertyCollection;

            }


            return true;
        }

    }
    public class SaveFieldMapper : FieldMapper
    {
        public SaveFieldBound MappedSaveField {
            get;
            set;
        }
        public SaveFieldMapper(SaveFieldBound fld)
        {
            MappedSaveField = fld;
            Property = fld.LookAheadProp;
            PropertyCollection = fld.LookAheadProps;
        }
        public SaveMap MappedSaveMap {
            get;
            set;

        }

        // for multi column, need to tie the return col
        // with a target field. 
        public List<SaveField> GetAllFieldsFromMap()
        {
            List<SaveField> fields = null;
            // for savemap support only independent fields
            if (MappedSaveMap != null)
                fields = MappedSaveMap.SaveFields.Where(field => field.AppObject.Equals(MappedSaveField.AppObject)).ToList();

            return fields;
        }

        public override string GetFieldName()
        {
            if (MappedSaveField != null)
                return MappedSaveField.FieldName;
            else
                return string.Empty;
        }
        public override bool AddProperty(LookAheadProperty prop)
        {
            if (MappedSaveField != null)
            {
                if (PropertyCollection == null)
                    PropertyCollection = new List<LookAheadProperty>();

                RemoveProperty(prop);
                if (prop.IsActive && PropertyCollection.Count > 0)
                {
                    // turn off active flag for existing one. 

                    PropertyCollection[0].IsActive = false;
                }
                SetActiveFlagForProp(prop);
                PropertyCollection.Add(prop);

                MappedSaveField.LookAheadProps = PropertyCollection;
                Property = prop;


            }
            return true;
        }
        public override bool RemoveProperty(LookAheadProperty prop)
        {

            if (MappedSaveField != null && PropertyCollection != null && prop != null)
            {
                PropertyCollection.RemoveAll(item => item.DataSource.Equals(prop.DataSource));
                if (PropertyCollection.Count > 0)
                {
                    MappedSaveField.LookAheadProp = PropertyCollection[0];
                }
                else
                    MappedSaveField.LookAheadProp = null;

                MappedSaveField.LookAheadProps = PropertyCollection;

                return true;
            }
            return false;


        }
    }
}
