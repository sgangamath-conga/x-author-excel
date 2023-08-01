using Apttus.XAuthor.Core;
using System.Collections.Generic;

namespace Apttus.XAuthor.AppDesigner
{

    public class RecordTypeSyncInfo
    {
        public int Updated;
        public int Added;
        public int Deleted;
        public RecordTypeSyncInfo()
        {

        }

        public RecordTypeSyncInfo(int Updated, int Added,int Deleted)
        {
            this.Updated = Updated;
            this.Added = Added;
            this.Deleted = Deleted;
        }
    }
    public class RecordTypeSyncModel
    {
        public ApttusRecordType OriginalRecordType;
        public ApttusRecordType ChangedRecordType;
        public string RecordTypeId;
        public string ObjectId;
        public RecordTypeChange RecordTypeChange;

        public RecordTypeSyncModel()
        {
            OriginalRecordType = new ApttusRecordType();
            ChangedRecordType = new ApttusRecordType();
        }

        public RecordTypeSyncModel(ApttusRecordType OriginalRecordType, ApttusRecordType ChangedRecordType, string ObjectId, string RecordTypeId, RecordTypeChange RecordTypeChange)
        {
            this.OriginalRecordType = OriginalRecordType;
            this.ChangedRecordType = ChangedRecordType;
            this.ObjectId = ObjectId;
            this.RecordTypeChange = RecordTypeChange;
            this.RecordTypeId = RecordTypeId;
        }
    }
}
