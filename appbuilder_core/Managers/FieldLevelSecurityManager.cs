using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    public class FieldLevelSecurityManager
    {
        private static FieldLevelSecurityManager _instance;
        private List<Guid> RefreshedObjects = new List<Guid>();
        ApplicationDefinitionManager appdefManager = ApplicationDefinitionManager.GetInstance;
        private ObjectManager objectManager = ObjectManager.GetInstance;

        private FieldLevelSecurityManager()
        {
        }

        public List<Guid> RefreshedObjectsList
        {
            get
            {
                return RefreshedObjects;
            }
            set
            {
                RefreshedObjects = value;
            }
        }

        public static FieldLevelSecurityManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FieldLevelSecurityManager();
                return _instance;
            }
        }

        private void updateFieldVisibleAndReadOnlyStatus(ApttusObject apttusObj, ApttusField fieldInAppDef, List<ApttusField> allFields)
        {
            if (fieldInAppDef.Datatype == Datatype.Attachment)
            {
                fieldInAppDef.Updateable = true;
                fieldInAppDef.Visible = true;
                return;
            }

            ApttusField field = allFields.Find(sf => sf.Id.Equals(fieldInAppDef.Id));
            if (field != null)
            {
                fieldInAppDef.Updateable = field.Updateable;
                if (field.Datatype == Datatype.Lookup) //Update the name lookup field
                {
                    string nameFieldOfLookup = field.Name + Constants.APPENDLOOKUPNAME;
                    ApttusField nameLookupField = apttusObj.Fields.Find(fld => fld.Name.Equals(nameFieldOfLookup));
                    
                    // Due to change in lookup name from (Name) to Name, adding backward compatibility code.
                    if (nameLookupField == null)
                    {
                        nameFieldOfLookup = fieldInAppDef.Name + Constants.APPENDLOOKUPNAME_WITH_PARENTHESIS;
                        nameLookupField = apttusObj.Fields.Find(fld => fld.Name.Equals(nameFieldOfLookup));
                    }
                    if (nameLookupField != null)
                        nameLookupField.Updateable = field.Updateable;
                }
            }
            else
            {
                if (fieldInAppDef.Datatype == Datatype.Lookup)
                {
                    fieldInAppDef.Visible = false;
                    //If the lookup object is invisble, then make the name field of lookup field invisible.
                    string nameFieldOfLookup = fieldInAppDef.Name + Constants.APPENDLOOKUPNAME;
                    ApttusField nameLookupField = apttusObj.Fields.Find(fld => fld.Name.Equals(nameFieldOfLookup));

                    // Due to change in lookup name from (Name) to Name, adding backward compatibility code.
                    if (nameLookupField == null)
                    {
                        nameFieldOfLookup = fieldInAppDef.Name + Constants.APPENDLOOKUPNAME_WITH_PARENTHESIS;
                        nameLookupField = apttusObj.Fields.Find(fld => fld.Name.Equals(nameFieldOfLookup));
                    }
                    if (nameLookupField != null)
                        nameLookupField.Visible = false;
                }
                //Below use-case is needed when a user doesn't have access(invisible) to that field. 'If' condition block is needed because,
                //this specifies the name field of lookup object which will not be part of salesforce fields list must be excluded and shouldn't be set to invisible and rest of all
                //other fields are invisible
                else if(fieldInAppDef.LookupObject == null)
                    fieldInAppDef.Visible = false;
            }
        }

        public bool IsFieldVisible(Guid uniqueId, string fieldId, string repeatingGroupUniqueId = "")
        {
            bool bVisible = false;

            // if NAMESPACE_PREFIX is blank user is Offline, return true
            if (string.IsNullOrEmpty(Constants.NAMESPACE_PREFIX))
                return true;

            // Get current App Object
            ApttusObject currentObject = appdefManager.GetAppObject(uniqueId);
            string objectId = currentObject.Id;

            if (!String.IsNullOrEmpty(repeatingGroupUniqueId))
            {
                Guid repeatingUniqueId;
                bool bParsed = Guid.TryParse(repeatingGroupUniqueId, out repeatingUniqueId);
                if (bParsed && repeatingUniqueId != Guid.Empty && repeatingUniqueId != uniqueId)
                {
                    if (fieldId.IndexOf('.') != -1)
                    {
                        string[] splitFieldId = fieldId.Split(new char[] { '.' });
                        fieldId = splitFieldId[splitFieldId.Length -1];
                    }
                }
            }

            // If AppObject is not Refreshed in Runtime, do Refresh
            if (!RefreshedObjects.Contains(uniqueId))
            {
                //List<ApttusField> fields = salesforceAdapter.GetFieldsForObject(objectId);
                List<ApttusField> fields = objectManager.GetApttusObject(objectId, true, false).Fields;
                currentObject.Fields.ForEach(fieldInAppDef => { updateFieldVisibleAndReadOnlyStatus(currentObject, fieldInAppDef, fields); });
                RefreshedObjects.Add(uniqueId);
            }

            ApttusField field = currentObject.Fields.Find(fld => fld.Id.Equals(fieldId, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                bVisible = field.Visible;

            return bVisible;
        }

        public bool IsFieldReadOnly(Guid uniqueId, string fieldId)
        {
            bool bReadOnly = false;

            // if NAMESPACE_PREFIX is blank user is Offline, return true
            if (string.IsNullOrEmpty(Constants.NAMESPACE_PREFIX))
                return true;

            // Get current App Object
            ApttusObject currentObject = appdefManager.GetAppObject(uniqueId);
            string objectId = currentObject.Id;

            // If AppObject is not Refreshed in Runtime, do Refresh
            if (!RefreshedObjects.Contains(uniqueId))
            {
                List<ApttusField> fields = objectManager.GetApttusObject(objectId, true, false).Fields;
                //List<ApttusField> fields = salesforceAdapter.GetFieldsForObject(objectId);
                currentObject.Fields.ForEach(fld => { updateFieldVisibleAndReadOnlyStatus(currentObject, fld, fields); });
                RefreshedObjects.Add(uniqueId);
            }

            ApttusField field = currentObject.Fields.Find(sf => sf.Id.Equals(fieldId));
            // In case fields belonging objects up in hierarchy split field Id
            if (field == null && fieldId.IndexOf('.') != -1)
            {
                string[] splitFieldId = fieldId.Split(new char[] { '.' });
                fieldId = splitFieldId[splitFieldId.Length - 1];
                field = currentObject.Fields.Find(sf => sf.Id.Equals(fieldId));
            }

            bReadOnly = field == null ? true : !field.Updateable;
            return bReadOnly;
        }

        public void GetFieldVisibleAndReadOnlyStatus(Guid uniqueId, string fieldId, out bool bVisible, out bool bReadOnly)
        {
            bVisible = false;
            bReadOnly = true;
            List<ApttusField> fields = new List<ApttusField>();

            // if NAMESPACE_PREFIX is blank user is Offline, return true
            if (string.IsNullOrEmpty(Constants.NAMESPACE_PREFIX))
                return;

            // Get current App Object
            ApttusObject currentObject = appdefManager.GetAppObject(uniqueId);
            string objectId = currentObject.Id;

            // If AppObject is not Refreshed in Runtime, do Refresh
            if (!RefreshedObjects.Contains(uniqueId))
            {
                fields = objectManager.GetApttusObject(objectId, true, false).Fields;
                //List<ApttusField> fields = salesforceAdapter.GetFieldsForObject(objectId);
                currentObject.Fields.ForEach(fld => { updateFieldVisibleAndReadOnlyStatus(currentObject, fld, fields); });
                RefreshedObjects.Add(uniqueId);
            }

            ApttusField field = currentObject.Fields.Find(sf => sf.Id.Equals(fieldId));
            // In case fields belonging objects up in hierarchy split field Id
            if (field == null && fieldId.IndexOf('.') != -1)
            {
                string[] splitFieldId = fieldId.Split(new char[] { '.' });
                fieldId = splitFieldId[splitFieldId.Length - 1];
                field = currentObject.Fields.Find(sf => sf.Id.Equals(fieldId));
            }

            bReadOnly = field == null ? true : !(field.Updateable || field.Creatable);

            bVisible = field == null ? false : field.Visible;
        }
    }
}
