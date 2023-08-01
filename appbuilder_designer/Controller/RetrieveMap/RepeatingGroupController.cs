/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    class RepeatingGroupController
    {
        RetrieveMap Model;
        ucRetrieveMap View;

        internal Predicate<RetrieveField> SupportedFieldsInGroupBy = rField => rField.DataType == Datatype.String || rField.DataType == Datatype.Lookup || rField.DataType == Datatype.Date || rField.DataType == Datatype.DateTime
                                                                               || rField.DataType == Datatype.Picklist || rField.DataType == Datatype.Editable_Picklist
                                                                               || rField.DataType == Datatype.Picklist_MultiSelect || (rField.DataType == Datatype.Formula && IsFormulaFieldGroupable(rField));

        internal Predicate<RetrieveField> SupportedFieldsInSortBy = rField => rField.DataType == Datatype.String || rField.DataType == Datatype.Lookup || rField.DataType == Datatype.Date || rField.DataType == Datatype.DateTime
                                                                    || rField.DataType == Datatype.Picklist || rField.DataType == Datatype.Decimal || rField.DataType == Datatype.Double || rField.DataType == Datatype.Editable_Picklist
                                                                    || rField.DataType == Datatype.Picklist_MultiSelect || (rField.DataType == Datatype.Formula && IsFormulaFieldSortable(rField));

        internal Predicate<RetrieveField> SupportedFieldInTotals = rfield => rfield.DataType == Datatype.Decimal || rfield.DataType == Datatype.Double || (rfield.DataType == Datatype.Formula && IsFormulaFieldCalculatable(rfield));

        ConfigurationManager configManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public RepeatingGroupController(RetrieveMap model, ucRetrieveMap view)
        {
            Model = model;
            View = view;
        }

        public RepeatingGroup GetRepeatingGroup(Guid AppObjectId)
        {
            return Model.RepeatingGroups.Where(s => s.AppObject.Equals(AppObjectId)).FirstOrDefault();
        }

        public void AddRepeatingCellField(ApttusObject apttusObject, ApttusField apttusField, string targetLocation, string namedRange = "", TreeNode selectedNode = null)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(apttusObject.UniqueId);
            if (repGroup == null)
            {
                repGroup = new RepeatingGroup(apttusObject.UniqueId);
                Model.RepeatingGroups.Add(repGroup);
                repGroup.Layout = View != null ? View.SelectedLayout : "Vertical";
            }

            // To get correct relatedFieldId, actual ApttusObject of apttusField is required instead of root object.
            // e.g. if object hierarchy is Quote<-Opportunity<-Account and if Account Id field is being dragged & dropped,
            // instead of passing IdAttribute of Quote it should pass IdAttribute of Account object
            ApttusObject actualApttusObject = selectedNode == null ? apttusObject : selectedNode.Parent.Tag as ApttusObject;
            string relatedFieldId = GetRelationalFieldId(apttusField, selectedNode, string.Empty, actualApttusObject.IdAttribute);
            // ID columns of lookup object are required to be added to Save Map for some use cases
            // so if field being added is ID field, instead of adding ID field from self Object , add lookupfield from parent object.
            // e.g. if Opportunity ID field is being drag & drop, add OpportunityID lookup field from Account object.
            // This allows ID field to be added to Save Map.
            Guid appObjId = apttusObject.UniqueId;
            if (selectedNode != null) //For quick app selectedNode will be null
                appObjId = (selectedNode.Parent.Tag as ApttusObject).UniqueId;

            string fullHierarchyName = string.Empty;
            if (repGroup.AppObject != appObjId) //If it is a relational field, then only get the Full Hierarcy Name, otherwise apply the direct parent object's full name.
                fullHierarchyName = GetFullHierarchyName(apttusField, selectedNode);

            if ((selectedNode != null) && !String.IsNullOrEmpty(selectedNode.Name))
            {
                ApttusObject selectedApttusObject = selectedNode.Parent.Tag as ApttusObject;
                if (apttusField.Id == selectedApttusObject.IdAttribute)
                {
                    if (selectedNode.Parent.Parent != null)
                        appObjId = (selectedNode.Parent.Parent.Tag as ApttusObject).UniqueId;

                    apttusField = ApplicationDefinitionManager.GetInstance.GetField(appObjId, selectedNode.Name);
                }
            }
            RetrieveField rField = new RetrieveField();
            rField.FieldId = string.IsNullOrEmpty(relatedFieldId) ? apttusField.Id : relatedFieldId + apttusField.Id;
            rField.MultiLevelFieldId = string.IsNullOrEmpty(relatedFieldId) ? apttusField.Id : relatedFieldId + "-" + apttusField.Id;

            rField.TargetLocation = targetLocation;
            rField.Type = ObjectType.Repeating;
            rField.DataType = apttusField.Datatype;
            rField.AppObject = string.IsNullOrEmpty(relatedFieldId) ? apttusObject.UniqueId : appObjId;
            // Assign correct Object Hierarchy Name
            ApttusObject rFieldObject = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
            if (string.IsNullOrEmpty(fullHierarchyName))
                fullHierarchyName = apttusObject.Name;
            rField.FieldName = fullHierarchyName + "." + apttusField.Name;

            if (!String.IsNullOrEmpty(namedRange))
                rField.TargetNamedRange = namedRange;

            repGroup.AddField(rField);

            //Refresh the view
            if (View != null)
                View.AddRow(rField);
        }

        /// <summary>
        /// Provide full hierarchy name for the apttus field.
        /// It has been make public to support duplicate field validation
        /// </summary>
        /// <param name="apttusField"></param>
        /// <param name="node"></param>
        /// <returns>ApttusFieldId</returns>
        public string GetFullHierarchyName(ApttusField apttusField, TreeNode node)
        {
            if (node == null)
                return string.Empty;
            string name = string.Empty;
            Stack<string> names = new Stack<string>();
            TreeNode currentNode = apttusField.Id.Equals(Constants.ID_ATTRIBUTE) ? node.Parent.Parent : node.Parent;

            bool bAddDot = false;
            while (currentNode != null)
            {
                if (bAddDot)
                    names.Push(Constants.DOT);

                string onlyName = (currentNode.Tag as ApttusObject).Name;
                string[] split = onlyName.Split(new char[] { '.' });
                if (split != null && split.Length > 0)
                    onlyName = split[split.Length - 1];
                names.Push(onlyName);
                currentNode = currentNode.Parent;
                bAddDot = true;
            }
            StringBuilder sb = new StringBuilder();
            foreach (string s in names)
                name += s;
            return name;
        }

        /// <summary>
        /// Gets the Id of a relational field.
        /// It has been make public to support duplicate field validation
        /// </summary>
        /// <param name="apttusField"></param>
        /// <param name="node"></param>
        /// <param name="relationalField"></param>
        /// <param name="currentObjectIdAttribute"></param>
        /// <returns>RelationalFieldId</returns>
        public string GetRelationalFieldId(ApttusField apttusField, TreeNode node, string relationalField = "", string currentObjectIdAttribute = "")
        {
            string fieldId = relationalField;
            string lookUpFieldName;
            // Quick App won't have UI and hence no treenode value, in that case return empty string
            if (node == null)
                return string.Empty;
            // In case of single level hierarchy use apttusfield.id, return empty
            if (node.Parent.Parent == null)
                return fieldId;
            else if (apttusField.Id == Constants.ID_ATTRIBUTE || apttusField.Id == currentObjectIdAttribute)
            {
                // In Expanded Tree Parent will always be look up field of the Object
                lookUpFieldName = node.Parent.Parent.Name;
                if (!string.IsNullOrEmpty(lookUpFieldName))
                {
                    // Use parent Tag to Get ApttusObject and then get lookupField from the Parent Object
                    ApttusObject parentObj = (ApttusObject)node.Parent.Parent.Parent.Tag;
                    ApttusField lookUpField = parentObj.Fields.Where(f => f.Name == lookUpFieldName).FirstOrDefault();

                    // Append field with hierarchy
                    fieldId = ApplicationDefinitionManager.GetInstance.GetLookupReference__R(lookUpField.Id) + "." + fieldId;
                    // Traverse through hierarchy of Tree till top level
                    fieldId = GetRelationalFieldId(apttusField, node.Parent, fieldId, currentObjectIdAttribute);
                }
            }
            // in case of Multi-level hierarchy traverse through hierarchy and create field
            else if (node.Parent.Parent != null && node.Parent.Parent.Tag.GetType() == typeof(ApttusObject))
            {
                // In Expanded Tree Parent will always be look up field of the Object
                lookUpFieldName = node.Parent.Name;

                // Use parent Tag to Get ApttusObject and then get lookupField from the Parent Object
                ApttusObject parentObj = (ApttusObject)node.Parent.Parent.Tag;
                ApttusField lookUpField = parentObj.Fields.Where(f => f.Name == lookUpFieldName).FirstOrDefault();

                // Append field with hierarchy
                fieldId = ApplicationDefinitionManager.GetInstance.GetLookupReference__R(lookUpField.Id) + "." + fieldId;
                // Traverse through hierarchy of Tree till top level
                fieldId = GetRelationalFieldId(apttusField, node.Parent, fieldId, currentObjectIdAttribute);
            }
            return fieldId;
        }

        private bool IsRetrieveFieldPartOfSaveMapRemoved(RetrieveField rField)
        {
            bool bCanRemove = true;
            bool bFieldFound = false;
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult result = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.No;
            Dictionary<SaveMap, SaveField> fieldsToRemoveFromSaveMap = new Dictionary<SaveMap, SaveField>();
            foreach (SaveMap sMap in configManager.SaveMaps)
            {
                foreach (SaveField sf in sMap.SaveFields)
                {
                    if (rField.FieldId.Equals(sf.FieldId) && rField.Type == sf.Type && rField.AppObject.Equals(sf.AppObject) && rField.TargetNamedRange.Equals(sf.TargetNamedRange))
                    {
                        bFieldFound = true;
                        fieldsToRemoveFromSaveMap.Add(sMap, sf);
                        break;
                    }
                }
            }

            if (bFieldFound)
            {
                result = ApttusMessageUtil.Show(String.Format(resourceManager.GetResource("REPEATINGGRPCTL_IsRetrieveFieldPartOfSaveMapRemoved_InfoMsg"), rField.FieldName),
                            resourceManager.GetResource("REPEATINGGRPCTL_IsRetrieveFieldPartOfSaveMapRemovedCAP_InfoMsg"), string.Empty, Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardIcon.Information,
                            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.Yes | Microsoft.WindowsAPICodePack.Dialogs.TaskDialogStandardButtons.No,
                            string.Empty, Globals.ThisAddIn.Application.Hwnd);

                bCanRemove = result == Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes;
                if (bCanRemove)
                {
                    foreach (KeyValuePair<SaveMap, SaveField> fieldsPerSaveMap in fieldsToRemoveFromSaveMap)
                        fieldsPerSaveMap.Key.SaveFields.Remove(fieldsPerSaveMap.Value);
                }
            }
            return bCanRemove;
        }

        public bool UpdateRepeatingCellField(TreeNode node, string targetLocation, string targetNamedRange)
        {
            ApttusObject apttusObject = View.GetRootObject(node.Parent);   //((ApttusObject)node.Parent.Tag);
            bool bRetrieveFieldRemoved = false;
            RepeatingGroup cellMap = GetRepeatingGroup(apttusObject.UniqueId);
            if (cellMap == null)
                return bRetrieveFieldRemoved;


            ApttusField apttusField = node.Tag as ApttusField;
            string relatedFieldId = GetRelationalFieldId(apttusField, node);

            Guid appObjId = (node.Parent.Tag as ApttusObject).UniqueId;

            string fullHierarchyName = string.Empty;
            if (cellMap.AppObject != appObjId) //If it is a relational field, then only get the Full Hierarchy Name, otherwise apply the direct parent object's full name.
                fullHierarchyName = GetFullHierarchyName(apttusField, node);

            if (apttusField.Id.Equals(Constants.ID_ATTRIBUTE) && !string.IsNullOrEmpty(node.Name))
            {
                if (node.Parent.Parent != null)
                    appObjId = (node.Parent.Parent.Tag as ApttusObject).UniqueId;

                apttusField = ApplicationDefinitionManager.GetInstance.GetField(appObjId, node.Name);
            }

            RetrieveField rField = (from retrieveField in cellMap.RetrieveFields
                                    where retrieveField.TargetNamedRange.Equals(targetNamedRange)
                                    select retrieveField).FirstOrDefault();

            bRetrieveFieldRemoved = IsRetrieveFieldPartOfSaveMapRemoved(rField);
            if (bRetrieveFieldRemoved)
            {
                rField.FieldId = string.IsNullOrEmpty(relatedFieldId) ? apttusField.Id : relatedFieldId + apttusField.Id;
                rField.MultiLevelFieldId = string.IsNullOrEmpty(relatedFieldId) ? apttusField.Id : relatedFieldId + "-" + apttusField.Id;

                rField.TargetLocation = targetLocation;
                rField.Type = ObjectType.Repeating;
                rField.DataType = apttusField.Datatype;
                rField.AppObject = string.IsNullOrEmpty(relatedFieldId) ? apttusObject.UniqueId : appObjId;

                ApttusObject rFieldObject = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
                if (string.IsNullOrEmpty(fullHierarchyName))
                    fullHierarchyName = apttusObject.Name;
                rField.FieldName = fullHierarchyName + "." + apttusField.Name;
            }
            return bRetrieveFieldRemoved;
        }

        public List<RetrieveField> GetSortByFields(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return null;
            return repGroup.RetrieveFields.Where(rField => SupportedFieldsInSortBy(rField)).ToList();
        }

        public List<RetrieveField> GetGroupByFields(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return null;
            return repGroup.RetrieveFields.Where(rField => SupportedFieldsInGroupBy(rField)).ToList();
        }

        internal List<RetrieveField> GetCalculatableTotalFields(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return null;
            return repGroup.RetrieveFields.Where(rField => SupportedFieldInTotals(rField)).ToList();
        }

        public string GetSelectedGroupByField(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return string.Empty;
            return repGroup.GroupByField;
        }

        public string GetSelectedSortByField(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return string.Empty;
            return repGroup.SortByField;
        }

        public string GetSelectedSortByField2(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return string.Empty;
            return repGroup.SortByField2;
        }

        public string GetSelectedGroupByField2(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return string.Empty;
            return repGroup.GroupByField2;
        }

        public void RemoveFieldFromRepeatingGroup(RepeatingGroup repGroup, RetrieveField rField)
        {
            if (repGroup == null)
                return;
            List<RetrieveField> fields = repGroup.RetrieveFields;
            int nFieldCount = fields.Count();
            if (nFieldCount == 1)
            {
                ExcelHelper.RemoveNamedRange(repGroup.TargetNamedRange);
                View.RemoveObject(repGroup);
            }
            if (nFieldCount >= 1)
                fields.Remove(rField);
        }

        internal List<string> GetSelectedTotalFields(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return null;
            return repGroup.TotalFields;
        }

        internal List<RetrieveField> GetRetreiveFields(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return null;
            return repGroup.RetrieveFields.ToList();
        }

        internal RepeatingGroupSortDirection GetSortDirectionForField1(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return RepeatingGroupSortDirection.Ascending;
            return repGroup.SortDirectionField1;
        }

        internal RepeatingGroupSortDirection GetSortDirectionForField2(Guid AppObjectId)
        {
            RepeatingGroup repGroup = GetRepeatingGroup(AppObjectId);
            if (repGroup == null)
                return RepeatingGroupSortDirection.Ascending;
            return repGroup.SortDirectionField2;
        }

        private static bool IsFormulaFieldSortable(RetrieveField rField)
        {
            bool isSortable = false;
            ApttusObject apttusObj = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
            if (apttusObj != null)
            {
                ApttusField apttusfield = apttusObj.GetField(rField.FieldId);
                if (apttusfield.Datatype == Datatype.Formula)
                {
                    switch (apttusfield.FormulaType)
                    {
                        case Constants.FORMULATYPECURRENCY:
                        case Constants.FORMULATYPEDOUBLE:
                        case Constants.FORMULATYPESTRING:
                            isSortable = true;
                            break;
                    }
                }
            }
            return isSortable;
        }

        private static bool IsFormulaFieldCalculatable(RetrieveField rField)
        {
            bool isCalculatable = false;
            ApttusObject apttusObj = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
            if (apttusObj != null)
            {
                ApttusField apttusfield = apttusObj.GetField(rField.FieldId);
                if (apttusfield.Datatype == Datatype.Formula)
                {
                    switch (apttusfield.FormulaType)
                    {
                        case Constants.FORMULATYPECURRENCY:
                        case Constants.FORMULATYPEDOUBLE:
                            isCalculatable = true;
                            break;
                    }
                }
            }
            return isCalculatable;
        }

        public static bool IsFormulaFieldGroupable(RetrieveField rField)
        {
            bool isGroupable = false;
            ApttusObject apttusObj = ApplicationDefinitionManager.GetInstance.GetAppObject(rField.AppObject);
            if (apttusObj != null)
            {
                ApttusField apttusfield = apttusObj.GetField(rField.FieldId);
                if (apttusfield.Datatype == Datatype.Formula)
                {
                    switch (apttusfield.FormulaType)
                    {
                        case Constants.FORMULATYPESTRING:
                            isGroupable = true;
                            break;
                    }
                }
            }
            return isGroupable;
        }
    }
}
