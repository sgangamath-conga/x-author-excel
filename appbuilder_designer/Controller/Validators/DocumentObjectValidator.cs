using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppDesigner
{

    public class DocumentObjectValidator : IValidator
    {
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private static DocumentObjectValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        private List<string> validDocumentObjects = new List<string> { "Document", "MailmergeTemplate", "ContentVersion", "FeedItem", "Attachment" };
        public static DocumentObjectValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new DocumentObjectValidator();
                        configManager = ConfigurationManager.GetInstance;
                    }
                }
                return instance;
            }
        }

        public List<ValidationResult> Validate<T>(T t)
        {
            ValidationResult result = null;
            List<ValidationResult> results = new List<ValidationResult>();
            if (t is RetrieveMap)
            {
                RetrieveMap rMap = t as RetrieveMap;

                foreach (RepeatingGroup repeatingGroup in rMap.RepeatingGroups)
                {

                    string objectName = ApplicationDefinitionManager.GetInstance.GetAppObject(repeatingGroup.AppObject).Name;
                    DocumentObject obj = DocumentObject.GetDocumentObject(ApplicationDefinitionManager.GetInstance.GetAppObject(repeatingGroup.AppObject).Id);

                    if (obj == null)
                        continue;

                    //Below If condition is to check for document object's Body field
                    //If body field is present, Name and Type field is necessary because body is base64
                    if (validDocumentObjects.Contains(obj.ObjectName))
                    {
                        if (repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals(obj.Field_FileData_ID)))
                        {
                            if (!repeatingGroup.RetrieveFields
                                               .Exists(f => f.FieldId.Equals(obj.Field_FileType_ID)) || !repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals(obj.Field_FileName_ID))
                                               || (obj.ObjectName.Equals("ContentVersion") && !repeatingGroup.RetrieveFields.Exists(f => f.FieldId.Equals("PathOnClient"))))
                            {
                                result = new ValidationResult
                                {
                                    EntityName = rMap.Name,
                                    EntityType = EntityType.DisplayMap
                                };
                                String msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateDocumentRepeatingGroup_ErrMsg"), obj.Field_FileType_Name, obj.Field_FileName_Name, objectName, Constants.DISPLAYMAP_NAME, obj.Field_FileData_ID);
                                if(obj.ObjectName.Equals("ContentVersion"))
                                    msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateContentDocumentRepeatingGroup_ErrMsg"), obj.Field_FileType_Name, obj.Field_FileName_Name,"Path On Client", objectName, Constants.DISPLAYMAP_NAME, obj.Field_FileData_ID);

                                if (result.Messages == null)
                                    result.Messages = new List<string>();
                                result.Messages.Add(msg);
                                result.Success = false;
                                results.Add(result);
                            }
                        }
                    }
                }
            }

            if (t is SaveMap)
            {
                SaveMap saveMap = t as SaveMap;

                foreach (SaveGroup saveGroup in saveMap.SaveGroups)
                {
                    string objectName = ApplicationDefinitionManager.GetInstance.GetAppObject(saveGroup.AppObject).Name;
                    DocumentObject obj = DocumentObject.GetDocumentObject(ApplicationDefinitionManager.GetInstance.GetAppObject(saveGroup.AppObject).Id);

                    if (obj == null)
                        continue;

                    if (validDocumentObjects.Contains(obj.ObjectName))
                    {
                        List<SaveField> saveGroupFields = saveMap.SaveFields.Where(saveField => saveField.GroupId.Equals(saveGroup.GroupId)).ToList();

                        if (saveGroupFields.Exists(f => f.FieldId.Equals(obj.Field_FileData_ID)))
                        {
                            if (!saveGroupFields.Exists(f => f.FieldId.Equals(obj.Field_FileType_ID)) || !saveGroupFields.Exists(f => f.FieldId.Equals(obj.Field_FileName_ID))
                                                        || (obj.ObjectName.Equals("ContentVersion") && !saveGroupFields.Exists(f => f.FieldId.Equals("PathOnClient"))))
                            {
                                result = new ValidationResult
                                {
                                    EntityName = saveMap.Name,
                                    EntityType = EntityType.SaveMap
                                };
                                String msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateDocumentRepeatingGroup_ErrMsg"), obj.Field_FileType_Name, obj.Field_FileName_Name, objectName, Constants.SAVEMAP_NAME, obj.Field_FileData_ID);
                                if (obj.ObjectName.Equals("ContentVersion"))
                                    msg = String.Format(resourceManager.GetResource("RETRIEVEMAPVALID_ValidateContentDocumentRepeatingGroup_ErrMsg"), obj.Field_FileType_Name, obj.Field_FileName_Name, "Path On Client", objectName, Constants.SAVEMAP_NAME, obj.Field_FileData_ID);

                                if (result.Messages == null)
                                    result.Messages = new List<string>();
                                result.Messages.Add(msg);
                                result.Success = false;
                                results.Add(result);
                            }
                        }
                    }
                }
            }

            return results;
        }

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
