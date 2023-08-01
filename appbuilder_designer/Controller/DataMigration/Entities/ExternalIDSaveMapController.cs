using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    class ExternalIDSaveMapController
    {
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;
        private DataMigrationModel Model;
        public List<Guid> ClonedObjectExternalIdSaveMaps = new List<Guid>();
        private ApplicationDefinitionManager appdefManger = ApplicationDefinitionManager.GetInstance;
        public Guid masterSaveMapId = Guid.Empty;

        public ExternalIDSaveMapController(DataMigrationModel model)
        {
            Model = model;
        }

        private RetrieveField GetExternalIdRetrieveField(MigrationObject obj)
        {
            RetrieveMap rMap = configManager.RetrieveMaps.Find(r => r.Id == obj.DisplayMapId);
            RetrieveField rField = rMap.RepeatingGroups[0].RetrieveFields.Find(rf => rf.FieldId == obj.ExternalID);
            return rField;
        }

        private Guid CreateSaveMapForClonedObject(int nSaveMapIndex, RetrieveField rField)
        {
            string saveMapName = string.Format(DataMigrationConstants.ExternalIdSaveMapFormat, nSaveMapIndex);
            Guid saveMapId = BuildSaveMapForExternalID(saveMapName, new List<RetrieveField>() { rField });
            return saveMapId;
        }

        public void CreateSaveMapsForExternalID()
        {
            List<RetrieveField> masterExternalIDSaveMapFields = new List<RetrieveField>(); //non-cloned object fields
            int nSaveMapIndex = 1;
            foreach (MigrationObject obj in Model.MigrationObjects.Where(obj => obj.CreateWorksheet))
            {
                RetrieveField rField = GetExternalIdRetrieveField(obj);

                if (obj.IsCloned)
                {
                    //If the object is cloned, Create new save map.
                    Guid saveMapId = CreateSaveMapForClonedObject(nSaveMapIndex, rField);
                    ++nSaveMapIndex;
                    if (saveMapId != Guid.Empty)
                    {
                        obj.ExternalIdSaveMap = saveMapId;
                        ClonedObjectExternalIdSaveMaps.Add(saveMapId);
                    }
                }
                else
                    masterExternalIDSaveMapFields.Add(rField);
            }

            Model.MasterExternalIdSaveMap = masterSaveMapId = BuildSaveMapForExternalID("ExternalID SM", masterExternalIDSaveMapFields);
            if (masterSaveMapId != Guid.Empty)
            {
                foreach (MigrationObject obj in Model.MigrationObjects)
                {
                    if (!obj.IsCloned)
                        obj.ExternalIdSaveMap = masterSaveMapId;
                }
            }

            //CreateSaveActionForExternalIDSaveMaps();
        }



        public Guid BuildSaveMapForExternalID(string saveMapName, List<RetrieveField> retreiveFields)
        {
            Guid saveMapId = Guid.Empty;
            SaveMap saveMapModel = new SaveMap();
            SaveMapController saveMapController = new SaveMapController(saveMapModel, null, FormOpenMode.Create);

            SaveMapRetrieveFieldController controller = new SaveMapRetrieveFieldController(saveMapController.Model, null);
            SaveMapRetrieveMap sm = new SaveMapRetrieveMap();

            sm.RetrieveMapId = Guid.NewGuid();
            sm.RetrieveMapName = "ExternalID SM"; //don't change this line

            controller.RetrieveMapSelectionChange(sm);

            IEnumerable<SaveMapRetrieveField> saveMapRetrieveFields = (from retrieveField in retreiveFields
                                                                       from saveMapRetrieveField in controller.Model
                                                                       where retrieveField.AppObject == saveMapRetrieveField.AppObjectUniqueId
                                                                       && retrieveField.FieldId == saveMapRetrieveField.RetrieveFieldId
                                                                       && retrieveField.TargetNamedRange == saveMapRetrieveField.TargetNamedRange
                                                                       select saveMapRetrieveField);

            foreach (SaveMapRetrieveField r in saveMapRetrieveFields)
                r.Included = true;

            controller.AddRetrieveFieldsToSaveMap();

            if (saveMapController.Model.SaveFields.Count > 0)
            {
                saveMapController.UpdateModel(saveMapController.Model.SaveFields);

                saveMapController.Save(saveMapName, true);

                saveMapId = saveMapModel.Id;
            }
            return saveMapId;
        }

        private void updateExistingExternalIdSaveMap(MigrationObject migrationObject)
        {
            SaveMap externalIdSaveMap = configManager.SaveMaps.Find(sm => sm.Id == Model.MasterExternalIdSaveMap);

            migrationObject.ExternalIdSaveMap = externalIdSaveMap.Id;

            SaveMap migrationObjSaveMap = configManager.SaveMaps.Find(x => x.Id == migrationObject.SaveMapId);

            SaveGroup migrationObjSaveGroup = migrationObjSaveMap.SaveGroups[0];

            MigrationField migrationField = migrationObject.Fields.Find(x1 => x1.FieldId == migrationObject.ExternalID);
            ApttusObject obj = appdefManger.GetAppObject(migrationObject.AppObjectUniqueId);
            ApttusField apttusField = obj.GetField(migrationField.FieldId);

            RepeatingGroup repGroup = (from rm in configManager.RetrieveMaps
                                       from rg in rm.RepeatingGroups
                                       where rg.TargetNamedRange == migrationObjSaveGroup.TargetNamedRange
                                       select rg).FirstOrDefault();

            RetrieveField rField = repGroup.RetrieveFields.Find(fld => fld.FieldId == migrationField.FieldId);

            Guid saveGroupId = Guid.NewGuid();

            SaveField saveField = new SaveField
            {
                AppObject = obj.UniqueId,
                DesignerLocation = rField.TargetLocation,
                FieldId = apttusField.Id,
                GroupId = saveGroupId,
                SaveFieldName = apttusField.Name,
                SaveFieldType = SaveType.RetrievedField,
                Type = ObjectType.Repeating,
                TargetColumnIndex = rField.TargetColumnIndex,
                TargetNamedRange = rField.TargetNamedRange
            };
            externalIdSaveMap.SaveFields.Add(saveField);

            SaveGroup saveGroup = new SaveGroup
            {
                AppObject = migrationObject.AppObjectUniqueId,
                GroupId = saveGroupId,
                Layout = "Vertical",
                LoadedRows = 0,
                TargetNamedRange = repGroup.TargetNamedRange
            };

            externalIdSaveMap.SaveGroups.Add(saveGroup);
        }

        public void UpdateExternalIdSaveMaps(List<MigrationObject> addedObjects, List<MigrationObject> removedObjects, List<MigrationObject> affectedObjects)
        {
            List<SaveMap> changedSaveMaps = new List<SaveMap>();
            int nSaveMapIndex = configManager.Application.MigrationModel.MigrationObjects.Where(x => x.IsCloned == true && x.CreateWorksheet == true).Count() + 1;
            if (addedObjects != null)
            {
                foreach (MigrationObject migrationObject in addedObjects)
                {
                    if (migrationObject.IsCloned && migrationObject.CreateWorksheet)
                    {
                        RetrieveField rField = GetExternalIdRetrieveField(migrationObject);
                        Guid saveMapId = CreateSaveMapForClonedObject(nSaveMapIndex, rField);
                        ++nSaveMapIndex;
                        if (saveMapId != Guid.Empty)
                        {
                            migrationObject.ExternalIdSaveMap = saveMapId;
                            ClonedObjectExternalIdSaveMaps.Add(saveMapId);
                        }
                    }
                    else if (migrationObject.CreateWorksheet)
                        updateExistingExternalIdSaveMap(migrationObject);
                }

            }

            if (removedObjects != null)
            {
                foreach (MigrationObject migrationObject in removedObjects)
                {
                    SaveMap sm = configManager.SaveMaps.Find(x => x.Id == migrationObject.ExternalIdSaveMap);
                    sm.SaveFields.RemoveAll(x => x.AppObject == migrationObject.AppObjectUniqueId && x.FieldId == migrationObject.ExternalID);
                    sm.SaveGroups.RemoveAll(x => x.AppObject == migrationObject.AppObjectUniqueId);
                }
            }

            if (affectedObjects != null)
            {
                foreach (MigrationObject migrationObject in affectedObjects)
                {
                    SaveMap sMap = configManager.SaveMaps.Where(x => x.Id == migrationObject.ExternalIdSaveMap).FirstOrDefault();
                    SaveMapValidator.GetInstance.Validate<SaveMap>(sMap);
                }
            }
        }

        public List<SaveMap> GetSaveMapsWithoutFields()
        {
            return configManager.SaveMaps.Where(x => x.SaveFields.Count == 0).ToList();
        }
    }
}
