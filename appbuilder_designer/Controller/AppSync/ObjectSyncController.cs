using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;
using System.Collections.Concurrent;

namespace Apttus.XAuthor.AppDesigner
{
    internal sealed class ObjectSyncController
    {
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ObjectManager objectManager = ObjectManager.GetInstance;

        internal ConcurrentBag<ApttusObject> RemovedObjects { get; private set; }
        internal ConcurrentBag<ApttusObject> ValidObjects { get; private set; }

        internal ObjectSyncController()
        {
        }

        public void VerifyObjectsExistsInCRM()
        {
            RemovedObjects = new ConcurrentBag<ApttusObject>();
            ValidObjects = new ConcurrentBag<ApttusObject>();

            List<ApttusObject> objectsInCRM = objectManager.GetAllStandardObjects();
            //As all objects have difference field collection maintained in config file - we are taking all objects here, like if user have choosed Opportunity as individual as well as part of account, then we need both objects to be synced.
            List<ApttusObject> objectsUsedInAppById = applicationDefinitionManager.GetAllObjects();//.GroupBy(s => s.Id).Select(s => s.First()).ToList();

            foreach(ApttusObject objInApp in objectsUsedInAppById)
            {
                if (objectsInCRM.Exists(objInCRM => objInCRM.Id == objInApp.Id))
                    ValidObjects.Add(objInApp);
                else
                    RemovedObjects.Add(objInApp);
            }
        }
    }
}
