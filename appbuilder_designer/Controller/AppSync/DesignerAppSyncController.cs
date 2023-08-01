using Apttus.XAuthor.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Apttus.XAuthor.AppDesigner
{
    public class DesignerAppSyncController
    {
        private ObjectManager objectManager = ObjectManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private FieldSynchronization fieldSynchronization;
        private ObjectSyncController objectSyncController;
        private FieldDataMismatchController fieldDataMismatchController;
        private RecordTypeSyncController recordTypeSyncController;
        private AppSyncView View;
        WaitWindowView waitWindow;
        public CRM ActiveCRM = CRMContextManager.Instance.ActiveCRM;
        public DesignerAppSyncController()
        {

        }

        internal void ValidateApp(WaitWindowView waitWindow)
        {
            this.waitWindow = waitWindow;
            objectSyncController = new ObjectSyncController();
            objectSyncController.VerifyObjectsExistsInCRM();

            fieldSynchronization = new FieldSynchronization();
            fieldDataMismatchController = new FieldDataMismatchController(fieldSynchronization.fieldUsageInApp);

            recordTypeSyncController = new RecordTypeSyncController();
            Parallel.ForEach(objectSyncController.ValidObjects, obj => SyncObjectAndFields(obj));
            recordTypeSyncController.UpdateAppDef();
        }

        internal ConcurrentBag<ApttusObject> GetMissingObjects()
        {
            if (objectSyncController == null)
            {
                return new ConcurrentBag<ApttusObject>();
            }
            else
                return objectSyncController.RemovedObjects;
        }

        internal ConcurrentBag<MissingFieldItem> GetMissingFieldInfo()
        {
            if (fieldSynchronization == null)
            {
                return new ConcurrentBag<MissingFieldItem>();
            }
            else
                return fieldSynchronization.GetMissingFieldInfo();
        }

        internal ConcurrentBag<DataTypeChangeInfo> GetDataTypeMismatchFieldInfo()
        {
            if (fieldSynchronization == null)
            {
                return new ConcurrentBag<DataTypeChangeInfo>();
            }
            else
                return fieldSynchronization.GetDataTypeMismatchFieldInfo();
        }
        internal RecordTypeSyncInfo GetRecordTypeSyncInfo()
        {
            if (recordTypeSyncController == null)
            {
                return new RecordTypeSyncInfo();
            }
            else
                return recordTypeSyncController.GetRecordTypeSyncInfo();
        }
        internal int GetPickListMismatchInfo()
        {
            if (fieldSynchronization == null)
            {
                return 0;
            }
            else
                return fieldSynchronization.GetPickListMismatchInfo();
        }

        private void SyncObjectAndFields(ApttusObject obj)
        {
            ApttusObject synchedObject = objectManager.GetApttusObject(obj.Id, true, false);
            fieldSynchronization.SynchronizeFields(synchedObject, obj);
            recordTypeSyncController.SyncRecordTypes(synchedObject, obj);
        }

        internal void RemoveFields(List<MissingFieldItem> removedFields)
        {
            fieldSynchronization.RemoveFields(removedFields);
            View.UpdateLabels();
        }

        internal void ChangeDatatypeofFields(List<DataTypeChangeInfo> mismatchField)
        {
            fieldDataMismatchController.ChangeDatatypeofFields(mismatchField);
            View.UpdateLabels();
        }

        internal void SetView(AppSyncView appSyncView)
        {
            View = appSyncView;
        }

        internal void RefreshSummaryData()
        {
            View.RefreshSummaryData();
        }
        internal void UpdateViewLabels()
        {
            View.UpdateLabels();
        }
    }


}

