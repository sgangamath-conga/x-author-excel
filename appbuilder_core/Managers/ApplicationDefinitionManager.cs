/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    public sealed class ApplicationDefinitionManager
    {
        private static ApplicationDefinitionManager instance;
        private static ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private static object syncRoot = new Object();

        public List<ApttusObject> AppObjects = new List<ApttusObject>();
        public List<CrossTabDef> CrossTabDefinitions = new List<CrossTabDef>();

        private ILookupIdAndLookupNameProvider lookupIdAndNameProvider;

        private ApplicationDefinitionManager()
        {
            lookupIdAndNameProvider = ObjectManager.GetInstance.GetLookUpIdAndNameProvider();
        }

        public static ApplicationDefinitionManager GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationDefinitionManager();
                    }
                }
                return instance;
            }
        }

        public List<ApttusObject> GetAppObjects()
        {
            return AppObjects;
        }

        //public List<CrossTabDef> GetCrossTabObjects()
        //{
        //    return CrossTabDefinitions;
        //}

        public List<ApttusObject> GetAppObjectById(string ObjectId)
        {
            List<ApttusObject> allObjects = GetParentAndChildObjects(AppObjects).Where(o => o.Id == ObjectId).ToList();

            //if (CrossTabDefinitions.Count > 0)
            //{

            //    foreach (CrossTabDef def in CrossTabDefinitions)
            //    {
            //        if (def.ColHeaderObject.Id.Equals(ObjectId))
            //            allObjects.Add(def.ColHeaderObject);

            //        if (def.RowHeaderObject.Id.Equals(ObjectId))
            //            allObjects.Add(def.RowHeaderObject);

            //        if (def.DataObject.Id.Equals(ObjectId))
            //            allObjects.Add(def.DataObject);

            //    }

            //}
            //TODO Add an exception for this case b/c this should never happen
            return allObjects;
        }

        public ApttusObject GetChildObject(Guid objectId, ApttusObject rootObject)
        {
            ApttusObject objectToReturn = null;

            if (rootObject.UniqueId.Equals(objectId))
                return rootObject;

            foreach (ApttusObject child in rootObject.Children)
            {
                if (child.UniqueId.Equals(objectId))
                {
                    objectToReturn = child;
                    break;
                }
                if (child.Children != null)
                {
                    objectToReturn = GetChildObject(objectId, child);
                    if (objectToReturn != null)
                        break;
                }
            }
            return objectToReturn;
        }

        public ApttusObject GetAppObject(Guid ObjectUniqueId)
        {
            foreach (ApttusObject rootObj in AppObjects)
            {
                ApttusObject appObj = GetChildObject(ObjectUniqueId, rootObj);
                if (appObj != null)
                    return appObj;
            }

            //if (CrossTabDefinitions.Count > 0)
            //{
            //    foreach (CrossTabDef def in CrossTabDefinitions)
            //    {
            //        if (def.ColHeaderObject.UniqueId.Equals(ObjectUniqueId))
            //            return def.ColHeaderObject;

            //        if (def.RowHeaderObject.UniqueId.Equals(ObjectUniqueId))
            //            return def.RowHeaderObject;

            //        if (def.DataObject.UniqueId.Equals(ObjectUniqueId))
            //            return def.DataObject;
            //    }

            //}
            //TODO Add an exception for this case b/c this should never happen
            return null;
        }

        private ApttusObject GetAppObject(ApttusObject AppObject, Guid ObjectUniqueId)
        {
            var Match = AppObject.Children.Select(s => s.UniqueId.Equals(ObjectUniqueId));
            if (Match.Any())
            {
                return AppObject.Children.FirstOrDefault(s => s.UniqueId.Equals(ObjectUniqueId));
            }
            else
            {
                foreach (var Child in AppObject.Children)
                {
                    ApttusObject MatchingAppObject = GetAppObject(Child, ObjectUniqueId);
                    if (MatchingAppObject != null)
                        return MatchingAppObject;
                }
            }
            return null;
        }

        public List<ApttusObject> GetAllObjects()
        {
            List<ApttusObject> objects = GetParentAndChildObjects(AppObjects);

            //foreach (CrossTabDef def in CrossTabDefinitions)
            //{
            //    objects.Add(def.DataObject);
            //    objects.Add(def.ColHeaderObject);
            //    objects.Add(def.RowHeaderObject);
            //}

            return objects;
        }

        public List<ApttusObject> GetFullHierarchyObjects(ApttusObject startObject)
        {
            List<ApttusObject> oApttusObjects = new List<ApttusObject>();

            var rootObjects = startObject == null ? AppObjects : startObject.Children.ToList();
            foreach (var rootObject in rootObjects)
            {
                oApttusObjects.Add(rootObject);
                if (rootObject.Children.Count > 0)
                    foreach (var childObject in rootObject.Children)
                    {
                        oApttusObjects.Add(childObject);
                        oApttusObjects.AddRange(GetFullHierarchyObjects(childObject));
                    }
            }
            return oApttusObjects;
        }

        public List<ApttusObject> GetParentAndChildObjects(List<ApttusObject> appObjects, List<ApttusObject> allObjects = null)
        {
            if (allObjects == null)
                allObjects = new List<ApttusObject>();

            foreach (ApttusObject rootObj in appObjects)
            {
                if (!allObjects.Any(o => o.UniqueId.Equals(rootObj.UniqueId)))
                    allObjects.Add(rootObj);
                foreach (ApttusObject childObj in rootObj.Children)
                {
                    ApttusObject tempChildObj = childObj.DeepCopy();
                    string parentNames = GetParentNames(rootObj);
                    tempChildObj.Name = parentNames + rootObj.Name + "." + tempChildObj.Name;
                    allObjects.Add(tempChildObj);

                    if (childObj.Children != null)
                    {
                        List<ApttusObject> child = new List<ApttusObject>();
                        child.Add(childObj);
                        GetParentAndChildObjects(child, allObjects);
                    }
                }
            }

            return allObjects;
        }

        public ApttusObject GetTopLevelParent(ApttusObject searchAppObject)
        {
            ApttusObject appObject;
            if (searchAppObject.Parent == null)
                appObject = searchAppObject;
            else
                appObject = GetTopLevelParent(searchAppObject.Parent);
            return appObject;
        }

        public string GetFullFieldName(Guid AppObjectId, string FieldId)
        {
            string result = string.Empty;
            ApttusObject appObject = GetAppObject(AppObjectId);
            ApttusField appField = GetField(AppObjectId, FieldId);
            // In case fields belonging objects up in hierarchy split field Id
            if (appField == null && FieldId.IndexOf('.') != -1)
            {
                string[] splitFieldId = FieldId.Split(new char[] { '.' });
                FieldId = splitFieldId[splitFieldId.Length - 1];
                appField = GetField(AppObjectId, FieldId);
            }
            string appObjectFullName = GetFullObjectName(appObject);
            result = appObjectFullName + Constants.DOT + appField.Name;
            return result;
        }

        public string GetFullObjectName(ApttusObject appObject)
        {
            string objectName = appObject.Name;
            if (appObject.Parent != null)
            {
                objectName = GetFullObjectName(appObject.Parent) + Constants.DOT + objectName;
            }

            return objectName;
        }

        public string GetParentNames(ApttusObject rootObj, string parentNames = "")
        {
            if (rootObj.Parent != null)
            {
                parentNames = string.IsNullOrEmpty(parentNames) ? rootObj.Parent.Name + "." : parentNames + "." + rootObj.Parent.Name;

                if (rootObj.Parent.Parent != null)
                {
                    parentNames = rootObj.Parent.Parent.Name + "." + parentNames;
                    GetParentNames(rootObj.Parent.Parent, parentNames);
                }
            }
            return parentNames;
        }

        /// <summary>
        /// Get field name
        /// </summary>
        /// <param name="AppObjectId"></param>
        /// <param name="FieldId"></param>
        /// <returns></returns>
        public ApttusField GetField(Guid AppObjectId, string FieldId)
        {
            ApttusField result = null;
            ApttusObject targetObject = null;
            if (GetAppObject(AppObjectId) != null)
            {
                // Find in Top Level
                targetObject = GetAppObject(AppObjectId);
            }
            else
            {
                // Find in Repeating Level
                foreach (ApttusObject AppObject in AppObjects)
                {
                    if (AppObject.Children.LongCount(a => a.UniqueId == AppObjectId) > 0)
                    {
                        targetObject = AppObject.Children.FirstOrDefault(a => a.UniqueId == AppObjectId);
                        break;
                    }
                }
            }
            if (targetObject != null)
            {
                result = targetObject.GetField(FieldId);
            }
            return result;
        }

        public double GetFieldMaxValue(ApttusField apttusField)
        {
            // SF Documentation
            // ◾scale: Maximum number of digits to the right of the decimal place.
            // ◾precision: Total number of digits, including those to the left and the right of the decimal place

            double result = 999999999999999;
            if (apttusField.Precision > 0)
            {
                double leftOfDecimal = 0.0; double rightOfDecimal = 0.0;
                int Length = apttusField.Precision - apttusField.Scale;
                leftOfDecimal = Math.Pow(10, Length) - 1;
                if (apttusField.Scale > 0)
                    rightOfDecimal = (1 - (1 / Math.Pow(10, apttusField.Scale)));
                result = leftOfDecimal + rightOfDecimal;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppObjectId"></param>
        /// <returns></returns>
        public string GetObjectNameById(Guid AppObjectId)
        {
            string objectName = GetAppObject(AppObjectId).Name;

            //if (string.IsNullOrEmpty(objectName))
            //{
            //    foreach (CrossTabDef def in CrossTabDefinitions)
            //    {
            //        if (def.ColHeaderObject.UniqueId.Equals(AppObjectId))
            //        {
            //            objectName = def.ColHeaderObject.Name;
            //            break;
            //        }
            //        if (def.RowHeaderObject.UniqueId.Equals(AppObjectId))
            //        {
            //            objectName = def.RowHeaderObject.Name;
            //            break;
            //        }
            //        if (def.DataObject.UniqueId.Equals(AppObjectId))
            //        {
            //            objectName = def.DataObject.Name;
            //            break;
            //        }
            //    }
            //}

            return objectName;
        }

        //public void RemoveCrossTabObject(CrossTabDef crossTab)
        //{
        //    if (crossTab == null) return;
        //    CrossTabDefinitions.Remove(crossTab);
        //}

        //public void AddCrossTabObject(CrossTabDef crossTab)
        //{
        //    if (crossTab != null)
        //    {
        //        CrossTabDefinitions.Add(crossTab);

        //    }
        //}

        public ApttusObject AddObject(ApttusObject oApttusObject)
        {
            ApttusObject AppObjectHierarchy = null;
            KeyValuePair<ApttusObject, ApttusObject> MatchingObjects = HierarchyMatch(null, oApttusObject);
            // Matching AppObject is returned
            if (MatchingObjects.Key != null)
            {
                ApttusObject LeafAppObject = MatchingObjects.Key;
                ApttusObject oChildHierarchyToAdd = MatchingObjects.Value;

                foreach (var oChild in oChildHierarchyToAdd.Children)
                {
                    // HierarchyMatch match found and children are available to be added - Add Split Hierarchy
                    ApttusObject SplitHierarchy = oChild.DeepCopy();
                    SplitHierarchy = CleanObject(SplitHierarchy);
                    SplitHierarchy = SyncFieldsWithOtherInstance(SplitHierarchy);

                    LeafAppObject.Add(SplitHierarchy);
                }

                AppObjectHierarchy = LeafAppObject;
            }
            else
            {
                // No HierarchyMatch - Add Full Hierarchy
                ApttusObject FullHierarchy = oApttusObject.DeepCopy();
                FullHierarchy = CleanObject(FullHierarchy);
                FullHierarchy = SyncFieldsWithOtherInstance(FullHierarchy);

                AppObjects.Add(FullHierarchy);

                AppObjectHierarchy = FullHierarchy;
            }
            return AppObjectHierarchy;
        }

        private ApttusObject SyncFieldsWithOtherInstance(ApttusObject oApttusObject)
        {

            if (oApttusObject.Children != null)
                foreach (ApttusObject child in oApttusObject.Children)
                {
                    SyncFieldsWithOtherInstance(child);
                }

            var AllOtherInstances = from o in GetFullHierarchyObjects(null)
                                    where o.Id == oApttusObject.Id & o.UniqueId != oApttusObject.UniqueId
                                    select o;

            if (AllOtherInstances.Any())
                oApttusObject.Fields = AllOtherInstances.First().Fields;

            return oApttusObject;

        }

        /// <summary>
        /// This function will handle hierarchy matching between app objects and new objects to be added.
        /// </summary>
        /// <param name="AppObject">App Object from AppObjects which are already added to the app.</param>
        /// <param name="oApttusObject">App Object which requires to be added to App Objects</param>
        /// <returns>
        /// 1. null,null - This means no hierarchy match was found
        /// 2. matching app object and matching object to be added - This result has 2 interpretations
        ///     i.  Matching app object has no child but the object to be added has child
        ///     ii. Both matching app object and object to added have no further child
        /// </returns>
        private KeyValuePair<ApttusObject, ApttusObject> HierarchyMatch(ApttusObject AppObject, ApttusObject oApttusObject)
        {
            KeyValuePair<ApttusObject, ApttusObject> result = new KeyValuePair<ApttusObject, ApttusObject>();
            //ApttusObject result = null;
            List<ApttusObject> MatchList;

            if (AppObject == null)
            {
                MatchList = (from ao in AppObjects
                             where ao.Id == oApttusObject.Id &
                             ao.ObjectType == oApttusObject.ObjectType
                             select ao).ToList();
                // Return null result if no matching root level object
                if (MatchList.Count == 0)
                    return result;
                else
                {
                    result = HierarchyMatch(MatchList[0], oApttusObject);
                    return result;
                }
            }
            else
            {
                MatchList = (from c in AppObject.Children
                             where c.Id == oApttusObject.Children[0].Id &
                             c.ObjectType == oApttusObject.Children[0].ObjectType
                             select c).ToList();

                // Return the last matching hierarchy if no children match. 
                // When HierarchyMatch is called and AppObject is not null then input objects are always a match so return that pair.
                if (MatchList.Count == 0)
                    result = new KeyValuePair<ApttusObject, ApttusObject>(AppObject, oApttusObject);

            }

            foreach (var Match in MatchList)
            {
                // There is more traversing to do as both Match(AppObject) and oApttusObject(object to be added)
                if (Match.Children.Count > 0 && oApttusObject.Children[0].Children.Count > 0)
                {
                    List<ApttusObject> MatchChildList = (from c in Match.Children
                                                         where c.Id == oApttusObject.Children[0].Children[0].Id &
                                                         c.ObjectType == oApttusObject.Children[0].Children[0].ObjectType
                                                         select c).ToList();

                    if (MatchChildList.Count > 0)
                    {
                        // Perform HierarchyMatch on Child Matches
                        foreach (var MatchChild in MatchChildList)
                        {
                            result = HierarchyMatch(MatchChild, oApttusObject.Children[0].Children[0]);
                            if (result.Key != null)
                                break;
                        }
                    }
                    else
                    {
                        // Return the Match and oApttusObject, in case none of the child match.
                        result = new KeyValuePair<ApttusObject, ApttusObject>(Match, oApttusObject.Children[0]);
                    }
                }
                // More child are present for Match(AppObject) but no more child for oApttusObject(object to be added)
                // set result as the last matching level objects
                else if (Match.Children.Count > 0 && oApttusObject.Children[0].Children.Count == 0)
                {
                    result = new KeyValuePair<ApttusObject, ApttusObject>(Match, oApttusObject.Children[0]);
                }
                // No more child are present for Match(AppObject) but more child present for oApttusObject(object to be added)
                // set result as the last matching level objects
                else if (Match.Children.Count == 0 && oApttusObject.Children[0].Children.Count > 0)
                {
                    result = new KeyValuePair<ApttusObject, ApttusObject>(Match, oApttusObject.Children[0]);
                }
                // No more child are present for Match(AppObject) and no more child for oApttusObject(object to be added)
                // set result with both leaf objects
                else if (Match.Children.Count == 0 && oApttusObject.Children[0].Children.Count == 0)
                {
                    result = new KeyValuePair<ApttusObject, ApttusObject>(Match, oApttusObject.Children[0]);
                }
            }

            return result;
        }

        public void RemoveObject(ApttusObject oApttusObject)
        {
            if (oApttusObject.Parent == null)
            {
                AppObjects.Remove(oApttusObject);
            }
            else
            {
                ApttusObject oParentObject = oApttusObject.Parent;
                oParentObject.Children.Remove(oApttusObject);
            }
        }

        public void Save()
        {
            if (AppObjects.Count > 0)
                configurationManager.UpdateAppDefinition(AppObjects);
            if (CrossTabDefinitions.Count > 0)
                configurationManager.UpdateCrossTabDefinition(CrossTabDefinitions);
        }

        //public ApttusObject GetCleanCrossTabObject(ApttusObject oApttusObject)
        //{
        //    // Add just the top level object to app definition
        //    ApttusObject parent = oApttusObject.DeepCopy();
        //    // Set Default Object Type
        //    parent.ObjectType = ObjectType.CrossTab;
        //    parent.Children.Clear();
        //    // parent.Children.ToList().ForEach(c => c.ObjectType = ObjectType.CrossTab);

        //    return CleanObject(parent);
        //}

        public void UpdateFields(ApttusObject oApttusObject, List<ApttusField> fields)
        {
            AppendLookupName(fields);

            if (oApttusObject.Fields != null)
            {
                // Make a delta collection of removeitems which are not present in fields (to be added)
                var removeitems = from i in oApttusObject.Fields
                                  where !(from s in fields
                                          select s.Id).Contains(i.Id)
                                  select i.Id;

                // Remove fields
                if (removeitems != null)
                {
                    oApttusObject.Fields.RemoveAll(f => removeitems.Contains(f.Id));
                }

                // Make a delta collection of fields to be added which don't already exist.
                var additems = from s in fields
                               where !(from i in oApttusObject.Fields
                                       select i.Id).Contains(s.Id)
                               select s;

                foreach (var a in additems)
                    oApttusObject.Fields.Add(a);

            }
            else
            {
                oApttusObject.Fields = new List<ApttusField>();
                foreach (var a in fields)
                    oApttusObject.Fields.Add(a);
            }

            var AllOtherInstances = from o in GetFullHierarchyObjects(null)
                                    where o.Id == oApttusObject.Id & o.UniqueId != oApttusObject.UniqueId
                                    select o;
            if (AllOtherInstances.Any())
                foreach (var OtherInstance in AllOtherInstances)
                    OtherInstance.Fields = oApttusObject.Fields;

        }

        private void AppendLookupName(List<ApttusField> fields)
        {
            var lookupFields = fields.Where(f => f.Datatype == Datatype.Lookup);
            List<ApttusField> lookupNameFields = new List<ApttusField>();

            foreach (ApttusField field in lookupFields)
            {
                ApttusField lookupNameField = field.DeepCopy();
                if (field.LookupObject != null && Constants.ObjectsWithoutLookupName.Contains(field.LookupObject.Id))
                {
                    ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;
                    ApttusObject lookupObject = appDefManager.GetAppObjectById(field.LookupObject.Id).FirstOrDefault();
                    ApttusField lookupObjectField = lookupObject.Fields.FirstOrDefault(f => f.NameField && f.Id != Constants.NAME_ATTRIBUTE);
                    if (lookupObjectField != null)
                    {
                        lookupNameField.Id = GetLookupReference__R(lookupNameField.Id) + Constants.DOT + lookupObjectField.Id;
                        lookupNameField.Name += " (" + lookupObjectField.Name + ")";
                        lookupNameField.Datatype = Datatype.String;

                        lookupNameFields.Add(lookupNameField);
                    }
                }
                else
                {
                    lookupNameField.Id = GetLookupNameFromLookupId(lookupNameField.Id);
                    lookupNameField.Name += Constants.APPENDLOOKUPNAME;
                    lookupNameField.Datatype = Datatype.String;

                    lookupNameFields.Add(lookupNameField);
                }
            }
            fields.AddRange(lookupNameFields);
        }

        public string GetLookupNameFromLookupId(string LookupId)
        {
            //string result = GetLookupReference__R(LookupId);
            //if (!string.IsNullOrEmpty(result))
            //    result += Constants.APPENDLOOKUPID;
            //return result;
            return lookupIdAndNameProvider.GetLookupNameFromLookupId(LookupId);
        }

        public string GetLookupReference__R(string LookupId__C)
        {
            //string result = string.Empty;

            //if (LookupId__C.EndsWith(Constants.ID_ATTRIBUTE))
            //    // Standard Object reference fields. Always end in Id. 
            //    // Fields can be accessed by removing Id and appending any field id of the lookup object.
            //    result = LookupId__C.Substring(0, LookupId__C.Length - 2);
            //else if (LookupId__C.EndsWith(Constants.CustomObjectReference__c))
            //    // Custom Object reference fields. Always end in __c.
            //    // Fields can be accessed by replacing __c with __r and appending any field id of the lookup object.
            //    result = LookupId__C.Substring(0, LookupId__C.Length - 3) + Constants.CustomObjectReference__r;

            //return result;
            return lookupIdAndNameProvider.GetLookupReference__R(LookupId__C);
        }

        public string GetLookupIdFromLookupName(string LookupName)
        {
            //string result = string.Empty;

            //if (LookupName.EndsWith(Constants.APPENDLOOKUPID))
            //{
            //    string ReferenceObject = LookupName.Substring(0, LookupName.IndexOf(Constants.APPENDLOOKUPID));
            //    result = GetLookupReference__C(ReferenceObject);
            //}

            //return result;
            return lookupIdAndNameProvider.GetLookupIdFromLookupName(LookupName);
        }

        public string GetLookupReference__C(string LookupId__R)
        {
            //string result = string.Empty;

            //if (LookupId__R.EndsWith(Constants.CustomObjectReference__r))
            //    // Custom Object reference fields. Replace __r with __c.
            //    result = LookupId__R.Substring(0, LookupId__R.Length - 3) + Constants.CustomObjectReference__c;
            //else
            //    // Standard Object reference fields. Add Id at the end.
            //    result = LookupId__R + Constants.ID_ATTRIBUTE;

            //return result;

            return lookupIdAndNameProvider.GetLookupReference__C(LookupId__R);
        }

        public bool Remove(ApttusObject oApttusObject)
        {
            return AppObjects.Remove(oApttusObject);
        }

        public ApttusFieldDS CloneApttusField(ApttusField field)
        {
            ApttusFieldDS FieldDS = new ApttusFieldDS
            {
                Id = field.Id,
                Label = field.Label,
                Name = field.Name,
                Datatype = field.Datatype,
                CRMDataType = field.CRMDataType,
                LookupObject = field.LookupObject,
                PicklistValues = field.PicklistValues,
                PicklistKeyValues = field.PicklistKeyValues,
                Scale = field.Scale,
                Precision = field.Precision,
                ExternalId = field.ExternalId,
                PicklistType = field.PicklistType,
                ControllingPicklistFieldId = field.ControllingPicklistFieldId,
                DependentPicklistValues = field.DependentPicklistValues,
                RecordType = field.RecordType,
                NameField = field.NameField,
                Visible = true,
                Updateable = field.Updateable,
                Creatable = field.Creatable,
                FormulaType = field.FormulaType,
                MultiLookupObjects = field.MultiLookupObjects,
                Required = field.Required
            };
            return FieldDS;
        }

        /// <summary>
        /// This method will check weather multiselectpicklist exists in app or not by scanning displaymap, matrixmaps & savemaps.
        /// </summary>
        /// <returns>true/false</returns>
        internal bool CheckifMultiSelectPickListExistsInApp()
        {
            bool bRetrieveMapsResult = configurationManager.Application.RetrieveMaps.Exists
                 (x => x.RetrieveFields.Exists(x1 => x1.DataType == Datatype.Picklist_MultiSelect)
                    || x.RepeatingGroups.Exists(X2 => X2.RetrieveFields.Exists(X3 => X3.DataType == Datatype.Picklist_MultiSelect)));

            if (bRetrieveMapsResult == true)
                return true;

            bool bMatrixMapsResult = configurationManager.Application.MatrixMaps.Exists
                 (x => x.IndependentFields.Exists(x1 => x1.DataType == Datatype.Picklist_MultiSelect)
                    || x.MatrixColumn.MatrixFields.Exists(X => X.DataType == Datatype.Picklist_MultiSelect)
                    || x.MatrixRow.MatrixFields.Exists(X => X.DataType == Datatype.Picklist_MultiSelect)
                    || x.MatrixData.MatrixDataFields.Exists(X => X.DataType == Datatype.Picklist_MultiSelect)
                );

            if (bMatrixMapsResult == true)
                return true;

            bool bMSaveMapsResult = configurationManager.Application.SaveMaps.Exists(
                    x => x.SaveFields.Exists(sf => IsFieldMultiSelect(sf))
                );

            if (bMSaveMapsResult == true)
                return true;

            return false;
        }

        private bool IsFieldMultiSelect(SaveField sf)
        {
            ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(sf.AppObject);
            if (obj != null)
            {
                ApttusField field = obj.GetField(sf.FieldId);
                if (field != null)
                    return field.Datatype == Datatype.Picklist_MultiSelect;
            }
            return false;
        }
        /// <summary>
        /// this method will return IsMultiSelectPickListExistsInApp configured in definitions, in case of having value null it will evaluate value.
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsMultiSelectPickListExistsInApp()
        {
            if (!configurationManager.Definition.IsMultiSelectPickListExistsInApp.HasValue)
            {
                configurationManager.Definition.IsMultiSelectPickListExistsInApp = CheckifMultiSelectPickListExistsInApp();
            }

            return configurationManager.Definition.IsMultiSelectPickListExistsInApp.HasValue ? configurationManager.Definition.IsMultiSelectPickListExistsInApp.Value : false;
        }

        #region "Private Methods"

        private ApttusObject CleanObject(ApttusObject oApttusObject)
        {
            if (oApttusObject.Children != null)
                foreach (ApttusObject child in oApttusObject.Children)
                    CleanObject(child);

            // Add default fields to a new object.
            List<string> defaultFields = DefaultFields(oApttusObject);
            oApttusObject.Fields.RemoveAll(f => !defaultFields.Contains(f.Id));
            oApttusObject.UniqueId = oApttusObject.UniqueId == Guid.Empty ? Guid.NewGuid() : oApttusObject.UniqueId;
            return oApttusObject;
        }

        public List<string> DefaultFields(ApttusObject oApttusObject)
        {
            var defaultFields = new List<string> {
                oApttusObject.IdAttribute,
                oApttusObject.NameAttribute
            };
            if (!string.IsNullOrEmpty(oApttusObject.LookupName))
                defaultFields.Add(oApttusObject.LookupName);
            return defaultFields;
        }

        //ToDo: remove this temp method Ser()
        private void Ser()
        {

            string xml;
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
            writerSettings.CloseOutput = true;

            StringBuilder sb = new StringBuilder();
            XmlWriter xmlwriter = XmlWriter.Create(sb, writerSettings);
            XmlSerializer xs = new XmlSerializer(typeof(ApttusObject));
            xs.Serialize(xmlwriter, AppObjects[0]);
            xmlwriter.Flush();
            xml = sb.ToString();

            Console.WriteLine(xml);
            Console.ReadLine();
        }
        #endregion


        public void RemoveField(ApttusObject appObject, string fieldID, List<ApttusObject> appObjects)
        {
            foreach (ApttusObject rootObj in appObjects)
            {
                if (rootObj.Id == appObject.Id)
                {
                    // As objects are sharing common list of fields, removal of field from one object instance will automatically put impact on other object's field lists too.
                    RemoveField(rootObj, fieldID);
                    break;
                }
                if (rootObj.Children != null && rootObj.Children.Count > 0)
                    RemoveField(appObject, fieldID, rootObj.Children.ToList());
            }
        }
        public void RemoveField(ApttusObject appObject, string fieldID)
        {
            appObject.Fields.RemoveAll(x => x.Id == fieldID);
        }
        public void UpdateDataType(ApttusObject appObject, string fieldID, ApttusField apttusField, List<ApttusObject> appObjects)
        {
            foreach (ApttusObject rootObj in appObjects)
            {
                if (rootObj.Id == appObject.Id)
                {
                    // As objects are sharing common list of fields, removal/addition of field from one object instance will automatically put impact on other object's field lists too.
                    RemoveField(rootObj, fieldID);
                    rootObj.Fields.Add(apttusField);
                    break;
                }
                if (rootObj.Children != null && rootObj.Children.Count > 0)
                    UpdateDataType(appObject, fieldID, apttusField, rootObj.Children.ToList());
            }

        }

        public void RemoveField(ApttusObject appObject, Predicate<ApttusField> predicate)
        {
            appObject.Fields.RemoveAll(predicate);
        }

        public void AddField(ApttusObject appObject, ApttusField apttusField)
        {
            appObject.Fields.Add(apttusField);
        }

        /// <summary>
        /// Retruns CRM(Salesforce or Dynamics) ID based on AppObjectId
        /// </summary>
        /// <param name="AppObjectId"></param>
        /// <returns></returns>
        public string GetObjectIdFromAppObjectId(Guid AppObjectId)
        {
            return GetAllObjects().Find(obj => obj.UniqueId == AppObjectId).Id;
        }

        public bool IsLookupField(ApttusField apttusField)
        {
            return lookupIdAndNameProvider.IsLookupField(apttusField);
        }

        public bool IsLookupIdField(ApttusObject obj, ApttusField apttusField)
        {
            return lookupIdAndNameProvider.IsLookupIdField(obj, apttusField);
        }
        public void UpdateRecordType(string ObjectId, ApttusRecordType ChangedRecordType, ApttusRecordType OriginalRecordType, List<ApttusObject> AllObjects, RecordTypeChange recordTypeChange)
        {
            foreach (var appObj in AllObjects)
            {
                if (appObj.Id.Equals(ObjectId))
                {
                    if (recordTypeChange == RecordTypeChange.ObjectHasNoRecordTypes)
                        appObj.RecordTypes = null;
                    else
                    {
                        if (OriginalRecordType != null)
                        {
                            appObj.RecordTypes.RemoveAll(rt => rt.Id.Equals(OriginalRecordType.Id));
                        }
                        if (ChangedRecordType != null)
                        {
                            if (appObj.RecordTypes == null) appObj.RecordTypes = new List<ApttusRecordType>();
                            appObj.RecordTypes.Add(ChangedRecordType);
                        }
                    }
                }
                if (appObj.Children != null && appObj.Children.Count > 0)
                    UpdateRecordType(ObjectId, ChangedRecordType, OriginalRecordType, appObj.Children.ToList(), recordTypeChange);
            }
        }
    }
}

