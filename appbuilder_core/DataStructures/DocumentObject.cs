using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.Core
{
    public class DocumentObject
    {
        public string ObjectName { get; set; }
        public string Field_FileName_ID { get; set; }
        public string Field_FileType_ID { get; set; }
        public string Field_FileData_ID { get; set; }
        public string Field_FileName_Name { get; set; }
        public string Field_FileType_Name { get; set; }

        public static DocumentObject GetDocumentObject(string ObjectName)
        {
            DocumentObject obj = new DocumentObject();

            if (ObjectName.Equals(Constants.DOCUMENT_OBJECTID))
            {
                obj.ObjectName = Constants.DOCUMENT_OBJECTID;
                obj.Field_FileName_ID = Constants.NAME_ATTRIBUTE;
                obj.Field_FileType_ID = Constants.DOCUMENT_TYPE;
                obj.Field_FileData_ID = Constants.DOCUMENT_BODY;
                obj.Field_FileName_Name = Constants.DOCUMENT_NAME_FIELD_NAME;
                obj.Field_FileType_Name = Constants.DOCUMENT_TYPE_FIELD_NAME;
            }

            if (ObjectName.Equals("ContentVersion"))
            {
                obj.ObjectName = "ContentVersion";
                obj.Field_FileName_ID = "Title";
                obj.Field_FileType_ID = "FileExtension";
                obj.Field_FileData_ID = "VersionData";
                obj.Field_FileName_Name = "Title";
                obj.Field_FileType_Name = "File Extension";
            }

            //if (ObjectName.Equals("MailmergeTemplate"))
            //{
            //    obj.ObjectName = "MailmergeTemplate";
            //    obj.Field_FileName_ID = "Name";
            //    obj.Field_FileType_ID = "Category";
            //    obj.Field_FileData_ID = "Body";
            //    obj.Field_FileName_Name = "Name";
            //    obj.Field_FileType_Name = "Document Type";
            //}

            if (ObjectName.Equals("FeedItem"))
            {
                obj.ObjectName = "FeedItem";
                obj.Field_FileName_ID = "Title";
                obj.Field_FileType_ID = "ContentType";
                obj.Field_FileData_ID = "Body";
                obj.Field_FileName_Name = "Title";
                obj.Field_FileType_Name = "Content File Type";
            }

            if (ObjectName.Equals("Attachment"))
            {
                obj.ObjectName = "Attachment";
                obj.Field_FileName_ID = "Name";
                obj.Field_FileType_ID = "ContentType";
                obj.Field_FileData_ID = "Body";
                obj.Field_FileName_Name = "File Name";
                obj.Field_FileType_Name = "Content Type";
            }

            return obj;
        }

    }
}
