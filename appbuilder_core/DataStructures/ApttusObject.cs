/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    public class ApttusObject : IChildItem<ApttusObject>
    {
        public ChildItemCollection<ApttusObject, ApttusObject> Children { get; private set; }

        [XmlIgnore]
        [ScriptIgnore]
        public ApttusObject Parent { get; set; }

        public Guid UniqueId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string LookupName { get; set; }
        public ObjectType ObjectType { get; set; }

        [XmlIgnore]
        public string NamePlusLookupName { get { return this.Name + " (" + this.LookupName + ")"; } }

        public List<ApttusField> Fields { get; set; }
        public List<ApttusRecordType> RecordTypes { get; set; }

        [XmlIgnore]
        public bool IsFullyLoaded { get; set; }
        [XmlIgnore]
        public int NoofChildObjectsLoaded { get; set; }

        public ApttusObject()
        {
            this.Children = new ChildItemCollection<ApttusObject, ApttusObject>(this);
            IdAttribute = Constants.ID_ATTRIBUTE;
            NameAttribute = Constants.NAME_ATTRIBUTE;
        }

        ApttusObject IChildItem<ApttusObject>.Parent
        {
            get { return this.Parent; }
            set { this.Parent = value; }
        }

        public ApttusObject GetChild(string id)
        {
            return Children.First(i => i.Id == id);
        }

        public void Add(ApttusObject Item)
        {
            if (Item.Parent != null)
                Item.Parent.Children.Remove(Item);

            Item.Parent = this;
            this.Children.Add(Item);
        }


        //public IEnumerator<ApttusObject> GetEnumerator()
        //{
        //    return this._childs.GetEnumerator();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return this.GetEnumerator();
        //}

        public ApttusObject DeepCopy()
        {
            ApttusObject NewApttusObject = (ApttusObject)this.MemberwiseClone();

            // Deep copy fields
            if (this.Fields != null)
            {
                int fieldCount = this.Fields.Count;
                ApttusField NewApttusField = null;
                NewApttusObject.Fields = null;
                NewApttusObject.Fields = new List<ApttusField>();
                for (int i = 0; i < fieldCount; i++)
                {
                    NewApttusField = Fields[i];
                    NewApttusField = NewApttusField.DeepCopy();
                    NewApttusObject.Fields.Add(NewApttusField);
                }
            }

            // Deep copy child objects
            if (this.Children != null)
            {
                int childCount = this.Children.Count;
                ApttusObject cApttusObject = null;
                NewApttusObject.Children = null;
                NewApttusObject.Children = new ChildItemCollection<ApttusObject, ApttusObject>(NewApttusObject);
                for (int i = 0; i < childCount; i++)
                {
                    cApttusObject = Children[i];
                    cApttusObject = cApttusObject.DeepCopy();
                    NewApttusObject.Children.Add(cApttusObject);
                }
            }

            return NewApttusObject;
        }

        public ApttusField GetField(string fieldId)
        {
            ApttusField retFieldId = Fields.FirstOrDefault(f => f.Id.ToLower() == fieldId.ToLower());

            if (retFieldId == null && fieldId.IndexOf('.') != -1)
            {
                string[] splitFieldId = fieldId.Split(new char[] { '.' });
                fieldId = splitFieldId[splitFieldId.Length - 1];
                retFieldId = Fields.FirstOrDefault(f => f.Id.ToLower() == fieldId.ToLower());
            }

            return retFieldId;
        }


        public string IdAttribute { get; set; }

        public string NameAttribute { get; set; }
    }

}
