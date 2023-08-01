using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{
    public class DataMigrationController
    {
        private DataMigrationModel Model;
        private DataMigrationWizard View;
        public FormOpenMode FormOpenMode { get; private set; }

        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configManager = ConfigurationManager.GetInstance;

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public bool isMigrationCompleted = false;

        ObjectManager objectManager = ObjectManager.GetInstance;
        private List<ApttusObject> clonedObjects = new List<ApttusObject>();
        Core.Application DataMigrationApplication = null;

        public DataMigrationController(DataMigrationModel model, DataMigrationWizard view, FormOpenMode formOpenMode)
        {
            this.Model = model;
            this.View = view;
            this.FormOpenMode = formOpenMode;
            if (view != null)
            {
                if (formOpenMode == FormOpenMode.Edit && IsAnyEntityMissingInAppDef())
                {
                    ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RIBBON_btnDataMigrationAppEdit_Click_InfoMsg1"), Constants.DESIGNER_PRODUCT_NAME);
                    return;
                }

                this.View.SetController(this, model);
                this.View.ShowDialog();
            }
        }

        public void SetView()
        {
            switch (FormOpenMode)
            {
                case Core.FormOpenMode.Create:
                    //AB-2870 :: Objects of previously opened app, appear in newly created data migration app. 
                    //The below dummy app is needed so that objects of previously opened app don't get acquired in second opened app.
                    //Add the instance of this App and remove it once the dialog gets closed. At a time a user can only open 1 datamigration dialog to create a DM App.

                    DataMigrationApplication = new Core.Application();
                    DataMigrationApplication.Definition.UniqueId = Guid.NewGuid().ToString();
                    DataMigrationApplication.Definition.Name = "DummyApp";
                    DataMigrationApplication.Definition.Version = "1.0";
                    DataMigrationApplication.Definition.Type = ApplicationType.RepeatingCells;

                    ConfigurationManager.GetInstance.SetApplication(DataMigrationApplication);

                    Utils.AddApplicationInstance(ApplicationMode.Designer, string.Empty);
                    break;

                case FormOpenMode.Edit:
                    View.FillForm(Model);
                    foreach (ApttusObject obj in applicationDefinitionManager.AppObjects)
                        clonedObjects.Add(obj.DeepCopy());
                    break;
                default:
                    break;
            }
        }

        public List<MigrationField> GetFields(ApttusObject AppObj, MigrationObject migrationObject)
        {
            List<MigrationField> migrationFields = new List<Core.MigrationField>();
            bool singleExternalIDexists = GetAllExternalIDs(AppObj) == 1 ? true : false;
            List<ApttusField> fieldstoinclude = AppObj.Fields.Where(x => x.Id == AppObj.NameAttribute || x.Id == AppObj.IdAttribute || (x.ExternalId && singleExternalIDexists)).ToList();
            //|| (x.IsMandatory == true && x.Updateable == true)).ToList();
            foreach (ApttusField afObj in fieldstoinclude)
            {
                MigrationField migrationField = new Core.MigrationField
                {
                    MigrationObjectId = migrationObject.Id,
                    FieldId = afObj.Id,
                    FieldName = afObj.Name,
                    DataType = afObj.Datatype,
                };

                migrationFields.Add(migrationField);
            }

            ApttusObject tempobj = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            applicationDefinitionManager.UpdateFields(tempobj, fieldstoinclude);

            return migrationFields;
        }               

        public int GetLookUPCount(ApttusObject appObj, DataMigrationModel migrationModel)
        {
            return appObj.Fields.Where(x => x.Datatype == Datatype.Lookup && migrationModel.MigrationObjects.Exists(x1 => x1.ObjectId == x.LookupObject.Id)).Count();
        }

        public bool IsExternalIDExists(ApttusObject appObj)
        {
            return GetAllExternalIDs(appObj) > 0;
        }

        public int GetAllExternalIDs(ApttusObject appObj)
        {
            ApttusObject apttusObject = objectManager.GetApttusObject(appObj.Id, false, false);
            return apttusObject.Fields.Where(x => x.ExternalId == true).Count();
        }

        public void BuildQueryFilters(MigrationObject migrationObject)
        {
            ApttusObject appObj = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

            List<ApttusField> lstFilterlookup = appObj.Fields.Where(x => x.Datatype == Datatype.Lookup
                && Model.MigrationObjects.Exists(x1 => x1.ObjectId == x.LookupObject.Id)).ToList();

            List<SearchFilterGroup> list = new List<SearchFilterGroup>();
            SearchFilterGroup searchFilterGroup = new SearchFilterGroup();
            searchFilterGroup.Filters = new List<SearchFilter>();
            searchFilterGroup.LogicalOperator = LogicalOperator.AND;
            list.Add(searchFilterGroup);
            int seqno = 1;

            foreach (ApttusField appfield in lstFilterlookup)
            {
                //if user has manually removed filter then lets not add it again.
                if (!migrationObject.RemovedFilters.Contains(appfield.Id))
                {
                    ApttusObject lookupObject = applicationDefinitionManager.GetAppObjectById(appfield.LookupObject.Id).FirstOrDefault();

                    if ((lookupObject.UniqueId == migrationObject.AppObjectUniqueId && migrationObject.IsCloned == true) || lookupObject.UniqueId != migrationObject.AppObjectUniqueId)
                    {
                        SearchFilter searchFilter = new SearchFilter();
                        searchFilter.AppObjectUniqueId = migrationObject.AppObjectUniqueId;
                        searchFilter.FieldId = appfield.Id;
                        searchFilter.ValueType = ExpressionValueTypes.Input;
                        searchFilter.Value = lookupObject.UniqueId + Constants.DOT + lookupObject.Fields.FirstOrDefault(f => f.Id == lookupObject.IdAttribute).Id;
                        searchFilter.Operator = "in";
                        searchFilter.QueryObjects = new List<QueryObject>();
                        searchFilter.SequenceNo = seqno++;
                        list[0].Filters.Add(searchFilter);
                    }
                }
            }

            SynchronizeFilters(migrationObject, list);
        }

        void SynchronizeFilters(MigrationObject migrationObject, List<SearchFilterGroup> searchfilters)
        {
            if (migrationObject.DataRetrievalAction.SearchFilter.Count == 0)
                migrationObject.DataRetrievalAction.SearchFilter = searchfilters;

            foreach (SearchFilter sf in searchfilters[0].Filters)
            {
                if (!migrationObject.DataRetrievalAction.SearchFilter[0].Filters.Exists(x => x.AppObjectUniqueId == sf.AppObjectUniqueId && x.FieldId == sf.FieldId))
                {
                    sf.SequenceNo = migrationObject.DataRetrievalAction.SearchFilter.Count > 0 ? migrationObject.DataRetrievalAction.SearchFilter[0].Filters.Count + 1 : 1;
                    migrationObject.DataRetrievalAction.SearchFilter[0].Filters.Add(sf);
                }
            }
        }

        public bool CanDeleteMigrationObject(MigrationObject migrationObject)
        {
            bool result = false;
            result = Model.MigrationObjects.Exists(x => x.DataRetrievalAction.SearchFilter.Count > 0 && x.DataRetrievalAction.SearchFilter[0].Filters.Exists(
                                               x1 => applicationDefinitionManager.GetField(x1.AppObjectUniqueId, x1.FieldId).Datatype == Datatype.Lookup
                                                   && applicationDefinitionManager.GetField(x1.AppObjectUniqueId, x1.FieldId).LookupObject.Id == migrationObject.ObjectId)
                                                   && x.DataRetrievalAction.SearchFilter[0].Filters.Count() > 0);
            return !result;
        }       

        public List<MigrationObject> PopulateObjects(List<ApttusObject> SelectedObjects)
        {
            ConcurrentBag<MigrationObject> recentMigrationObjects = new ConcurrentBag<MigrationObject>();
            Parallel.ForEach(SelectedObjects, selectedObject =>
            {
                ApttusObject apttusObject = objectManager.GetApttusObject(selectedObject.Id, false, false, 0);
                apttusObject.ObjectType = ObjectType.Repeating;

                MigrationObject migrationObject = new Core.MigrationObject();
                migrationObject.Id = Guid.NewGuid();

                apttusObject.UniqueId = migrationObject.AppObjectUniqueId = applicationDefinitionManager.AddObject(apttusObject).UniqueId;
                migrationObject.ObjectId = apttusObject.Id;
                migrationObject.ObjectName = apttusObject.Name;
                migrationObject.Fields = GetFields(apttusObject, migrationObject);
                migrationObject.ExternalID = GetExternalIDDetails(apttusObject).Count == 1 ? GetExternalIDDetails(apttusObject).FirstOrDefault().Id : string.Empty;
                migrationObject.CreateWorksheet = true;
                recentMigrationObjects.Add(migrationObject);
            });

            Model.MigrationObjects.AddRange(recentMigrationObjects);

            UpdateLookups();

            ResolveObjectDependency();

            return recentMigrationObjects.ToList();
        }

        void UpdateLookups()
        {
            foreach (var migrationObject in Model.MigrationObjects)
                SyncMigrationLookup(migrationObject);
        }

        public void ResetAppDefManager()
        {
            if (DataMigrationApplication != null)
                Utils.RemoveApplicationInstance(DataMigrationApplication.Definition.UniqueId, string.Empty, ApplicationMode.Designer);

            if (ConfigurationManager.GetInstance.Application == null || clonedObjects.Count == 0)
                return;
            applicationDefinitionManager.AppObjects.Clear();
            applicationDefinitionManager.AppObjects.AddRange(clonedObjects);
        }

        internal void ResolveObjectDependency()
        {
            ObjectDependencyResolver dependency = new ObjectDependencyResolver(Model.MigrationObjects);
            dependency.ResolveObjectDependencies();
        }

        public List<ApttusObject> GetAllApptusObjects()
        {
            return objectManager.GetAllStandardObjects();
        }

        public MigrationObject CloneMigrationObject(MigrationObject migrationObject)
        {
            MigrationObject newmigrationObject = migrationObject.PartialClone() as MigrationObject;
            Model.MigrationObjects.Add(newmigrationObject);
            return newmigrationObject;
        }

        public void DeleteMigrationObject(MigrationObject migrationObjectToDelete)
        {
            Model.MigrationObjects.Remove(migrationObjectToDelete);
            UpdateAppDefForDeletion(migrationObjectToDelete);
            ResolveObjectDependency();
        }

        void UpdateAppDefForDeletion(MigrationObject migrationObject)
        {
            bool anydependentinstanceexists = Model.MigrationObjects.Exists(x => x.AppObjectUniqueId == migrationObject.AppObjectUniqueId);

            if (anydependentinstanceexists) return;

            ApttusObject apttusObject = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);
            applicationDefinitionManager.RemoveObject(apttusObject);
        }

        public List<MigrationLookup> GetLookUpDetails(ApttusObject appObj)
        {
            List<MigrationLookup> lstLookup = new List<MigrationLookup>();
            List<ApttusField> lstFilterlookup = appObj.Fields.Where(x => x.Datatype == Datatype.Lookup && Model.MigrationObjects.Exists(x1 => x1.ObjectId == x.LookupObject.Id)).ToList();

            if (lstFilterlookup != null && lstFilterlookup.Count > 0)
                foreach (var lookup in lstFilterlookup)
                    lstLookup.Add(new MigrationLookup() { LookupId = lookup.Id, LookupObjectId = lookup.LookupObject.Id, LookupName = lookup.Name, WorkflowInputMigrationObjectId = Model.MigrationObjects.Find(x => x.ObjectId == lookup.LookupObject.Id).Id });

            return lstLookup;
        }

        public List<MigrationObject> GetLookUpInputDetails(ApttusObject appObj, string lookupFieldID)
        {
            List<MigrationObject> lstObjects = new List<MigrationObject>();
            ApttusField lstFilterlookup = appObj.Fields.Where(x => x.Id == lookupFieldID).FirstOrDefault();

            lstObjects = Model.MigrationObjects.Where(x => x.ObjectId == lstFilterlookup.LookupObject.Id).ToList();

            return lstObjects;
        }


        public List<ApttusField> GetExternalIDDetails(ApttusObject appObj)
        {
            ApttusObject apttusObject = objectManager.GetApttusObject(appObj.Id, false);
            List<ApttusField> lstFields = apttusObject.Fields.Where(x => x.ExternalId == true).ToList();
            return lstFields;
        }

        public List<MigrationLookup> GetExcludedLookUpObjects(MigrationObject migrationObject)
        {
            List<MigrationLookup> lstLookup = new List<MigrationLookup>();
            ApttusObject appObj = objectManager.GetApttusObject(migrationObject.ObjectId, false);

            List<ApttusField> lookupFields = (from field in appObj.Fields.Where(x => x.Datatype == Datatype.Lookup && x.LookupObject != null)
                                              where Model.MigrationObjects.Exists(obj => obj.ObjectId == field.LookupObject.Id)
                                              where !migrationObject.Fields.Exists(fld => fld.FieldId == field.Id)
                                              select field).ToList();

            if (lookupFields != null)
                foreach (var lookup in lookupFields)
                    lstLookup.Add(new MigrationLookup() { LookupObjectId = lookup.LookupObject.Id, LookupName = lookup.Name, LookupId = lookup.Id });

            return lstLookup;
        }

        public List<ApttusFieldDS> GetFieldsList(MigrationObject MObject, List<ApttusField> allFields)
        {
            ConcurrentBag<ApttusFieldDS> list = new ConcurrentBag<ApttusFieldDS>();
            if (MObject.Fields != null)
            {
                allFields.ForEach(field =>
                {
                    ApttusFieldDS FieldDS = applicationDefinitionManager.CloneApttusField(field);
                    if (MObject.Fields.Exists(a => a.FieldId == field.Id))
                        FieldDS.Included = true;
                    list.Add(FieldDS);
                });
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }
            else
            {
                allFields.ForEach(field =>
                {
                    ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype, field.Updateable, field.FormulaType,field.CRMDataType, field.Creatable);
                    list.Add(FieldDS);
                });
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }

        }

        public void UpdateFields(ApttusObject AppObject, MigrationObject MObject, List<ApttusFieldDS> selFields)
        {
            MObject.Fields.Clear();
            foreach (ApttusFieldDS fld in selFields)
            {
                if (!fld.Id.EndsWith(Constants.APPENDLOOKUPID))
                {
                    MObject.Fields.Add
                        (
                            new MigrationField() { MigrationObjectId = MObject.Id, FieldId = fld.Id, FieldName = fld.Name, DataType = fld.Datatype }
                        );
                }
            }
            applicationDefinitionManager.UpdateFields(AppObject, selFields.OfType<ApttusField>().ToList());

            SyncMigrationLookup(MObject);



            View.UpdateUI();
        }

        public void SyncMigrationLookup(MigrationObject MObject)
        {

            ApttusObject AppObj = applicationDefinitionManager.GetAppObject(MObject.AppObjectUniqueId);
            List<MigrationLookup> lookupdetails = GetLookUpDetails(AppObj);

            foreach (var lookup in lookupdetails)
            {
                if (!MObject.Lookup.Exists(x => x.LookupId == lookup.LookupId))
                    MObject.Lookup.Add(lookup);
            }

            MObject.Lookup.RemoveAll(X1 => !lookupdetails.Exists(x => x.LookupId == X1.LookupId));


        }

        internal bool CyclicDependencyExistsFor(MigrationObject selectedObject, List<ApttusField> selectedObjectFields, out MigrationObject cyclicDependentObject, out ApttusField cyclicDependentField)
        {
            cyclicDependentObject = null;
            cyclicDependentField = null;

            bool bCyclicDependencyExist = false;

            IEnumerable<ApttusField> lookupFields = selectedObjectFields.Where(fld => fld.Datatype == Datatype.Lookup);
            foreach (ApttusField lookupField in lookupFields)
            {
                if (lookupField.LookupObject != null)
                {
                    MigrationObject migrationObject = Model.MigrationObjects.Find(obj => obj.ObjectId == lookupField.LookupObject.Id);
                    if (migrationObject != null && migrationObject.ObjectId != selectedObject.ObjectId)
                    {
                        bCyclicDependencyExist = bCyclicDependencyExist || CyclicDependencyExists(migrationObject, selectedObject, selectedObjectFields, out cyclicDependentField);
                        if (bCyclicDependencyExist)
                        {
                            cyclicDependentObject = migrationObject;
                            break;
                        }
                    }
                }
            }

            return bCyclicDependencyExist;
        }

        private bool CyclicDependencyExists(MigrationObject selectedLookupFieldObject, MigrationObject selectedObject, List<ApttusField> selectedObjectFields, out ApttusField cyclicDependentField)
        {
            cyclicDependentField = null;
            bool bObject1DependencyExist = false, bObject2DependencyExist = false;

            List<MigrationField> lookupfields = selectedLookupFieldObject.Fields.Where(fld => fld.DataType == Datatype.Lookup).ToList();
            foreach (MigrationField field in lookupfields)
            {
                ApttusField apttusField = applicationDefinitionManager.GetField(selectedLookupFieldObject.AppObjectUniqueId, field.FieldId);
                if (apttusField == null)
                    continue;
                if (apttusField.LookupObject == null)
                    continue;

                bObject1DependencyExist = selectedObject.ObjectId == apttusField.LookupObject.Id;
                if (bObject1DependencyExist)
                    break;
            }

            if (bObject1DependencyExist)
            {
                List<ApttusField> selectedLookupfields = selectedObjectFields.Where(fld => fld.Datatype == Datatype.Lookup).ToList();
                foreach (ApttusField apttusField in selectedLookupfields)
                {
                    if (apttusField.LookupObject == null)
                        continue;
                    bObject2DependencyExist = selectedLookupFieldObject.ObjectId == apttusField.LookupObject.Id;
                    if (bObject2DependencyExist)
                    {
                        cyclicDependentField = apttusField;
                        break;
                    }
                }
            }
            return bObject1DependencyExist && bObject2DependencyExist;
        }

        public void UpdateWizardView()
        {
            if (View != null)
                View.UpdateUI();
        }

        internal void CreateMigrationApp(string appName)
        {
            Model.AppName = appName;
            DataMigrationManager dataMigrationManager = new DataMigrationManager(Model);
            dataMigrationManager.CreateMigrationApp();
        }

        internal void EditMigrationApp()
        {
            DataMigrationEditManager dataMigrationManager = new DataMigrationEditManager(Model);
            dataMigrationManager.EditMigrationApp();
        }

        internal void SyncFieldsForClonedObjects(ApttusObject AppObject, MigrationObject MObject, List<ApttusFieldDS> selFields)
        {
            IEnumerable<MigrationObject> objectsWithSameUniqueId = Model.MigrationObjects.Where(o => o.AppObjectUniqueId == MObject.AppObjectUniqueId);
            foreach (MigrationObject obj in objectsWithSameUniqueId)
            {
                if (obj != MObject)
                    UpdateFields(AppObject, obj, selFields);
            }
            View.UpdateUI();
        }

        /// <summary>
        /// validate data migration application stability by making sure that all objects,maps,actions and workflows exist in ApplicationDefinitionManager.
        /// </summary>
        internal bool IsAnyEntityMissingInAppDef()
        {
            // Missing Objects 
            bool missingObjects = (from mobj in Model.MigrationObjects
                                   where (!applicationDefinitionManager.AppObjects.Exists(x => x.UniqueId == mobj.AppObjectUniqueId))
                                   select mobj).Count() > 0;

            // Missing Display Maps
            bool missingdisplaymaps = (from mobj in Model.MigrationObjects.Where(x => x.CreateWorksheet)
                                       where (!configManager.RetrieveMaps.Exists(x => x.Id == mobj.DisplayMapId))
                                       select mobj.DisplayMapId).Count() > 0;

            // Missing save Maps
            bool missingsavemaps = (from mobj in Model.MigrationObjects.Where(x => x.CreateWorksheet)
                                    where (!configManager.SaveMaps.Exists(x => x.Id == mobj.SaveMapId || x.Id == mobj.ExternalIdSaveMap))
                                    select mobj.DisplayMapId).Count() > 0;

            // Missing Display Actions
            bool missingDisplayActions = (from mobj in Model.MigrationObjects.Where(x => x.CreateWorksheet)
                                          where (!configManager.Actions.Exists(x => x.Id == mobj.DisplayActionId))
                                          select mobj.DisplayActionId).Count() > 0;

            // Missing save Actions
            bool missingSaveActions = (from mobj in Model.MigrationObjects.Where(x => x.CreateWorksheet)
                                       where (!configManager.Actions.Exists(x => x.Id == mobj.SaveActionId))
                                       select mobj.SaveActionId).Count() > 0;

            // Missing Workflows
            bool missingWorkFlows = configManager.Workflows.Where(mactionFlows => (mactionFlows.Id == Model.MigrationActionFlows.AllActionFlowId)
                                    || (mactionFlows.Id == Model.MigrationActionFlows.AppInfoActionFlowId)
                                    || (mactionFlows.Id == Model.MigrationActionFlows.SaveActionFlowId)
                                    || (mactionFlows.Id == Model.MigrationActionFlows.SaveSourceDataActionFlowId)
                                    || (mactionFlows.Id == Model.MigrationActionFlows.SelectiveActionFlowId)).Count() != 5;

            return (missingObjects || missingdisplaymaps || missingsavemaps || missingDisplayActions || missingSaveActions || missingWorkFlows);

        }

    }
}
