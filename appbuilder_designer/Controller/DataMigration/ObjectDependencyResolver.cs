using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    class ObjectDependencyResolver
    {
        private List<MigrationObject> MigratedObjects;
        private HashSet<MigrationObject> OrderedObjects = new HashSet<MigrationObject>();
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

        public ObjectDependencyResolver(List<MigrationObject> migratedObjects)
        {
            MigratedObjects = migratedObjects;
        }

        public void ResolveObjectDependencies()
        {
            //Step 1.
            PopulateDependencies();
            //Step 2.
            ResolveDependenciesOrder(MigratedObjects);
            //Step 3.
            UpdateOrderSequence();
        }

        private void UpdateOrderSequence()
        {
            int sequence = 1;
            foreach (MigrationObject orderedObject in OrderedObjects)
                orderedObject.Sequence = sequence++;
        }


        private void PopulateDependencies()
        {
            foreach(MigrationObject obj in MigratedObjects)
            {
                List<MigrationField> lookupfields = obj.Fields.Where(fld => fld.DataType == Datatype.Lookup && MigratedObjects.Exists(x1 => x1.ObjectId == applicationDefinitionManager.GetField(obj.AppObjectUniqueId, fld.FieldId).LookupObject.Id)).ToList();

                obj.Dependencies.Clear();

                foreach (MigrationField field in lookupfields)
                {
                    //ApttusField apttusField = applicationDefinitionManager.GetField(obj.AppObjectUniqueId, field.FieldId);
                    //if (apttusField == null)
                    //    continue;

                    MigrationObject dependentLookupObject = MigratedObjects.Find(obj1 => obj1.Id == obj.Lookup.Where(x => x.LookupId == field.FieldId).FirstOrDefault().WorkflowInputMigrationObjectId);
                    if (dependentLookupObject != null && obj != dependentLookupObject)
                    {
                        obj.Dependencies.Add(dependentLookupObject);
                    }
                }
            }
        }

        private void ResolveDependenciesOrder(IEnumerable<MigrationObject> objects)
        {
            foreach(MigrationObject obj in objects)
            {
                if (!OrderedObjects.Contains(obj))
                {
                    ResolveDependenciesOrder(obj.Dependencies);
                    OrderedObjects.Add(obj);
                }
            }            
        }
    }
}
