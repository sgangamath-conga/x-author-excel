using Apttus.XAuthor.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    class AutoDataTypeConversion
    {
        public List<Core.Datatype> TextDataTypes { get; private set; }
        public List<Core.Datatype> NumericDataTypes { get; private set; }
        public List<Core.Datatype> DateDataTypes { get; private set; }

        public AutoDataTypeConversion()
        {
            TextDataTypes = new List<Core.Datatype>();
            NumericDataTypes = new List<Core.Datatype>();
            DateDataTypes = new List<Core.Datatype>();

            TextDataTypes.Add(Core.Datatype.String);
            TextDataTypes.Add(Core.Datatype.Picklist);
            TextDataTypes.Add(Core.Datatype.Email);
            TextDataTypes.Add(Core.Datatype.Editable_Picklist);

            NumericDataTypes.Add(Core.Datatype.Double);
            NumericDataTypes.Add(Core.Datatype.Decimal);

            DateDataTypes.Add(Core.Datatype.Date);
            DateDataTypes.Add(Core.Datatype.DateTime);
        }
    }

    public class FieldDataMismatchController
    {
        private AutoDataTypeConversion dataTypeConversion = new AutoDataTypeConversion();
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
        private FieldUsageInApp fieldUsageInApp;
        public FieldDataMismatchController(FieldUsageInApp fieldUsageInApp)
        {
            this.fieldUsageInApp = fieldUsageInApp;
        }

        public bool CanConvertDataType(Core.Datatype From, Core.Datatype To)
        {
            if (dataTypeConversion.TextDataTypes.Contains(From))
                return dataTypeConversion.TextDataTypes.Contains(To);
            else if (dataTypeConversion.NumericDataTypes.Contains(From))
                return dataTypeConversion.NumericDataTypes.Contains(To);
            else if (dataTypeConversion.DateDataTypes.Contains(From))
                return dataTypeConversion.DateDataTypes.Contains(To);
            else
                return false;
        }

        internal void ChangeDatatypeofFields(List<DataTypeChangeInfo> mismatchFields)
        {
            mismatchFields.ForEach(field =>
            {
                if (field.FieldUsedInActions.Count == 0)
                {
                    UpdateDisplayMaps(field);
                    UpdateAppDef(field);
                    fieldUsageInApp.mismatchFields = new ConcurrentBag<DataTypeChangeInfo>(fieldUsageInApp.mismatchFields.Except(new[] { field }));
                }
            });
        }

        void UpdateDisplayMaps(DataTypeChangeInfo mismatchField)
        {
            List<RetrieveMap> displayMapstobeRemoved = new List<RetrieveMap>();
            mismatchField.FieldUsedInDisplayMaps.ForEach((frmap) =>
            {
                RetrieveMapController retrieveMapController = new RetrieveMapController(null, null);
                RetrieveMap displayMap = configManager.RetrieveMaps.Find(rMap => rMap.Id == frmap.Id);
                retrieveMapController.Initialize(displayMap, MapMode.RetrieveMap);

                RetrieveField rField = displayMap.RetrieveFields.Find(rf => rf.FieldId == mismatchField.FieldId);
                if (rField != null)
                {
                    rField.DataType = mismatchField.To;
                }
                else
                {
                    RepeatingGroup rGroup = displayMap.RepeatingGroups.Find(rg => appDefManager.GetAppObject(rg.AppObject).Id == mismatchField.AppObjectId);
                    if (rGroup != null)
                    {
                        rField = rGroup.RetrieveFields.Find(rf => rf.FieldId == mismatchField.FieldId);
                        if (rField != null)
                        {
                            rField.DataType = mismatchField.To;
                        }
                    }
                }

                displayMapstobeRemoved.Add(frmap);
            });
           
        }

        void UpdateAppDef(DataTypeChangeInfo mismatchField)
        {
            appDefManager.UpdateDataType(appDefManager.GetAppObjectById(mismatchField.AppObjectId).FirstOrDefault(), mismatchField.FieldId, mismatchField.SynchedField, appDefManager.AppObjects);
        }

    }
}