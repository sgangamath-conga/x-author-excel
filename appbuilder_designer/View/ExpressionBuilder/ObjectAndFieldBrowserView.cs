/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ObjectAndFieldBrowserView : Form
    {
        ObjectAndFieldBrowserController controller;
        const string IdColumnName = "dcId";
        const string NameColumnName = "dcName";
        const string DatatypeColumnName = "dcDatatype";
        SearchFilter WIPFilter = new SearchFilter();
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        bool LookupClick = false;

        public ObjectAndFieldBrowserView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            dgvFields.AutoGenerateColumns = false;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("OBJFIELDBROWSEVIEW_btnSave_Text");
            dcDatatype.HeaderText = resourceManager.GetResource("COMMON_DataType_Text");
            dcId.HeaderText = resourceManager.GetResource("COMMON_FieldId_Text");
            dcName.HeaderText = resourceManager.GetResource("COMMON_FieldName_Text");
        }

        internal void SetController(ObjectAndFieldBrowserController controller)
        {
            this.controller = controller;
        }

        private void ObjectAndFieldBrowserView_Load(object sender, EventArgs e)
        {
            //controller.InitializeUIObjects();
            IntializeAndBindView();
        }

        private void IntializeAndBindView()
        {
            btnSave.Enabled = false;

            // Initialize WIPFilter with 3 variables
            // 1. AppObject Unique Id (of the final selected object)
            // 2. Field Id for the AppObject
            // 3. Current Search Filter Objects collection (if null, initialize new one)
            if (controller.SearchFilter.QueryObjects != null)
            {
                WIPFilter.AppObjectUniqueId = controller.SearchFilter.AppObjectUniqueId;
                WIPFilter.FieldId = controller.SearchFilter.FieldId;
                //WIPFilter.QueryObjects = controller.SearchFilter.QueryObjects;
                WIPFilter.QueryObjects = controller.SearchFilter.QueryObjects.Select(x => x.Clone()).ToList();
            }
            else
            {
                WIPFilter.AppObjectUniqueId = controller.AppObject.UniqueId;
                WIPFilter.FieldId = null;
                WIPFilter.QueryObjects = new List<QueryObject>();
                // Add the current object
                QueryObject newQO = CreateNewQueryObject(controller.AppObject);
                WIPFilter.QueryObjects.Add(newQO);
            }

            // Render Bread Crumb, Object Tree and Fields Grid
            RenderBreadCrumbsAndObjects(WIPFilter.QueryObjects, applicationDefinitionManager.GetAppObject(WIPFilter.AppObjectUniqueId), WIPFilter.FieldId);

        }

        private QueryObject CreateNewQueryObject(ApttusObject apttusObject)
        {
            QueryObject queryObject = new QueryObject
            {
                SequenceNo = WIPFilter.QueryObjects.Count + 1,
                AppObjectUniqueId = apttusObject.UniqueId,
                LookupFieldId = string.Empty,
                RelationshipType = QueryRelationshipType.None,
                BreadCrumbLabel = apttusObject.Name,
                BreadCrumbTooltip = apttusObject.Name,
                DistanceFromChild = 0,
                LeafAppObjectUniqueId = apttusObject.UniqueId
            };

            return queryObject;
        }
        private void RenderBreadCrumbsAndObjects(List<QueryObject> QueryObjects, ApttusObject appObject, string fieldId)
        {
            // Render Bread Crumbs
            RenderBreadCrumbs(QueryObjects);

            // Render Object Tree
            RenderObjects(appObject, fieldId);
        }

        private void RenderBreadCrumbs(List<QueryObject> queryObjects)
        {
            pnlBreadCrumb.Controls.Clear();
            //bool firstBreadCrumbAdded = false;

            for (int i = 0; i < queryObjects.Count; i++)
            {
                QueryObject queryObject = queryObjects[i];

                // Add Delimiter from the 2nd Search Filter
                if (i > 0)
                {
                    Label lblDelimiter = new Label()
                    {
                        AutoSize = true,
                        Text = Constants.BREADCRUMB_DELIMITER
                    };
                    pnlBreadCrumb.Controls.Add(lblDelimiter);
                }

                // Last link will be current object so it will not be clickable - add label. Otherwise add linklabel
                if (i != queryObjects.Count - 1)
                {
                    // Add QueryObject Bread Crumb
                    LinkLabel llQO = new LinkLabel()
                    {
                        AutoSize = true,
                        Text = queryObject.BreadCrumbLabel,
                        Tag = queryObject,
                        LinkColor = Color.Orange
                    };

                    llQO.LinkClicked += new LinkLabelLinkClickedEventHandler(ll_LinkClicked);
                    pnlBreadCrumb.Controls.Add(llQO);
                }
                else
                {
                    Label lblLastBreadCrumb = new Label()
                    {
                        AutoSize = true,
                        Text = queryObject.BreadCrumbLabel
                    };
                    pnlBreadCrumb.Controls.Add(lblLastBreadCrumb);
                }
            }

        }

        private void ll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel llQO = sender as LinkLabel;
            QueryObject queryObject = llQO.Tag as QueryObject;

            // Remove the objects till the sequence number which was clicked.
            QueryObject lastQO = null;
            for (int i = WIPFilter.QueryObjects.Count - 1; i > queryObject.SequenceNo - 1; i--)
            {
                lastQO = WIPFilter.QueryObjects[i];
                WIPFilter.QueryObjects.RemoveAt(i);
            }

            // Prepare the last QO for rendering
            lastQO = WIPFilter.QueryObjects.Last();
            lastQO.AppObjectUniqueId = lastQO.LeafAppObjectUniqueId;
            lastQO.LookupFieldId = string.Empty;
            lastQO.RelationshipType = QueryRelationshipType.None;
            //BreadCrumbLabel = controller.AppObject.Name,
            //BreadCrumbTooltip = controller.AppObject.Name,
            lastQO.DistanceFromChild = 0;
            RenderBreadCrumbsAndObjects(WIPFilter.QueryObjects, applicationDefinitionManager.GetAppObject(lastQO.LeafAppObjectUniqueId), null);

            btnSave.Enabled = false;
        }

        private void RenderObjects(ApttusObject appObject, string fieldId)
        {
            tvObjects.Nodes.Clear();
            tvObjects.Nodes.Add(RenderObjectsTree(appObject, null));
            SetObjectAndFieldSelection(appObject, fieldId);
        }

        private TreeNode RenderObjectsTree(ApttusObject appObject, TreeNode childNode)
        {
            TreeNode parentNode = new TreeNode
            {
                ToolTipText = appObject.Id,
                Text = appObject.Name,
                Tag = appObject
            };

            if (childNode != null)
                parentNode.Nodes.Add(childNode);

            if (appObject.Parent != null)
                parentNode = RenderObjectsTree(applicationDefinitionManager.GetAppObject(appObject.Parent.UniqueId), parentNode);

            return parentNode;
        }

        private void SetObjectAndFieldSelection(ApttusObject apttusObject, string fieldId)
        {

            tvObjects.ExpandAll();

            // Object Selection - Below line which sets the SelectedNode will fire the tvObjects_AfterSelect routine which Renders the Fields grid
            tvObjects.SelectedNode = FindLeafNode(tvObjects.Nodes[0]);

            // Set the field selection
            if (fieldId != null)
            {
                DataGridViewRow row = dgvFields.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Cells[IdColumnName].Value.ToString().Equals(fieldId))
                    .First();

                row.Selected = true;
            }
        }

        private TreeNode FindLeafNode(TreeNode oParentNode)
        {
            if (oParentNode.Nodes.Count == 0)
                return oParentNode;
            else
            {
                // Use recursion to find the leaf node.
                // Note: Index of 0 is used as the assumption is that user always see's a tree with 0 or 1 child nodes, not more.
                return FindLeafNode(oParentNode.Nodes[0]);
            }
        }

        private void tvObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvObjects.SelectedNode.Tag.GetType() == typeof(ApttusObject))
            {
                ApttusObject selectedAppObject = tvObjects.SelectedNode.Tag as ApttusObject;

                if (!LookupClick)
                {
                    // Update the last QO in the WIPFilter and Render Bread Crumb
                    QueryObject lastQO = WIPFilter.QueryObjects.Last();
                    // Sequence No should not be changed
                    lastQO.AppObjectUniqueId = selectedAppObject.UniqueId;
                    lastQO.LookupFieldId = string.Empty;
                    lastQO.RelationshipType = lastQO.AppObjectUniqueId.Equals(lastQO.LeafAppObjectUniqueId)
                        ? QueryRelationshipType.None
                        : QueryRelationshipType.Parent;
                    lastQO.BreadCrumbLabel = selectedAppObject.Name;
                    lastQO.BreadCrumbTooltip = selectedAppObject.Name;
                    lastQO.DistanceFromChild = GetDistanceFromChild();
                    // Leaf AppObject Unique Id should not be changed

                    RenderBreadCrumbs(WIPFilter.QueryObjects);
                }

                // Render Fields
                RenderFields(selectedAppObject);
            }
        }

        private void RenderFields(ApttusObject selectedAppObject)
        {
            dgvFields.SelectionChanged -= new EventHandler(dgvFields_SelectionChanged);
            // Exclude fields of type Textarea and Rich Textarea since they are not supposed in SOQL where clause
            SortableBindingList<ApttusField> fieldsBindingSource;

            //Exclude fields of type Picklist_MultiSelect for AIC as they are not supported yet
            if (CRMContextManager.Instance.ActiveCRM == CRM.AIC)
            {
                fieldsBindingSource = new SortableBindingList<ApttusField>(selectedAppObject.Fields.Where(f => f.Datatype != Datatype.Picklist_MultiSelect && f.Datatype != Datatype.Textarea && f.Datatype != Datatype.Rich_Textarea && f.Datatype != Datatype.Attachment && f.Datatype != Datatype.Base64
                                                                                                                                            && f.Datatype != Datatype.Formula || f.FormulaType == Constants.FORMULATYPESTRING || f.FormulaType == Constants.FORMULATYPEDOUBLE || f.FormulaType == Constants.FORMULATYPEBOOLEAN || f.FormulaType == Constants.FORMULATYPECURRENCY || f.FormulaType == Constants.FORMULATYPEPERCENT || f.FormulaType == Constants.FORMULATYPEDATE && f.Datatype != Datatype.AnyType).Select(g => g).OrderBy(f => f.Name));
            }
            else
            {
                fieldsBindingSource = new SortableBindingList<ApttusField>(selectedAppObject.Fields.Where(f => f.Datatype != Datatype.Textarea && f.Datatype != Datatype.Rich_Textarea && f.Datatype != Datatype.Attachment && f.Datatype != Datatype.Base64
                                                                                                                                            && f.Datatype != Datatype.Formula || f.FormulaType == Constants.FORMULATYPESTRING || f.FormulaType == Constants.FORMULATYPEDOUBLE || f.FormulaType == Constants.FORMULATYPEBOOLEAN || f.FormulaType == Constants.FORMULATYPECURRENCY || f.FormulaType == Constants.FORMULATYPEPERCENT || f.FormulaType == Constants.FORMULATYPEDATE && f.Datatype != Datatype.AnyType).Select(g => g).OrderBy(f => f.Name));
            }
            dgvFields.DataSource = fieldsBindingSource;
            dgvFields.ClearSelection();
            dgvFields.SelectionChanged += new EventHandler(dgvFields_SelectionChanged);
        }

        private void dgvFields_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (tvObjects.SelectedNode != null)
            {
                ApttusObject selectedAppObject = tvObjects.SelectedNode.Tag as ApttusObject;
                foreach (DataGridViewRow r in dgvFields.Rows)
                {
                    if ((Datatype)r.Cells[DatatypeColumnName].Value == Datatype.Lookup)
                    {
                        DataGridViewLinkCell linkCell = new DataGridViewLinkCell
                        {
                            LinkColor = Color.Orange,
                            Tag = r.Cells[IdColumnName]
                        };
                        r.Cells[NameColumnName] = linkCell;
                    }
                }
            }
        }

        private void dgvFields_SelectionChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;

            foreach (DataGridViewCell cell in dgvFields.SelectedCells)
            {
                if (tvObjects.SelectedNode != null)
                {
                    ApttusObject currentAppObject = tvObjects.SelectedNode.Tag as ApttusObject;
                    string selectedFieldId = (string)dgvFields.Rows[cell.RowIndex].Cells[IdColumnName].Value;

                    WIPFilter.AppObjectUniqueId = currentAppObject.UniqueId;
                    WIPFilter.FieldId = selectedFieldId;
                    btnSave.Enabled = true;
                }
            }
        }

        private void dgvFields_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && tvObjects.SelectedNode != null)
            {
                ApttusObject currentAppObject = tvObjects.SelectedNode.Tag as ApttusObject;
                string selectedFieldId = (string)dgvFields.Rows[e.RowIndex].Cells[IdColumnName].Value;

                if (dgvFields.Columns[e.ColumnIndex].Name == NameColumnName && (Datatype)dgvFields.Rows[e.RowIndex].Cells[DatatypeColumnName].Value == Datatype.Lookup)
                {
                    LookupClick = true;
                    AddLookupQueryObject(currentAppObject, selectedFieldId, GetDistanceFromChild());
                    LookupClick = false;
                    btnSave.Enabled = false;
                }
            }
            else
            {
                // ToDo: Maybe redundant
                btnSave.Enabled = false;
            }
        }

        private void AddLookupQueryObject(ApttusObject currentAppObject, string lookupFieldId, int distanceFromChild)
        {
            // Validation if atleast 1 lookup object is found in App Definition.
            ApttusField lookupAppField = currentAppObject.Fields.FirstOrDefault(f => f.Id == lookupFieldId);
            ApttusObject lookupAppObject = lookupAppField.LookupObject;
            List<ApttusObject> allLookupAppObject = applicationDefinitionManager.GetAppObjectById(lookupAppObject.Id);

            if (allLookupAppObject.Count > 0)
            {
                // Use the first instance of the lookup object
                // ToDo: Future enhancement could be a object picker for the end user
                ApttusObject fullLookupAppObject = allLookupAppObject.FirstOrDefault();

                // Update the last QO before adding the new QO
                QueryObject lastQO = WIPFilter.QueryObjects.Last();
                lastQO.AppObjectUniqueId = fullLookupAppObject.UniqueId;
                lastQO.LookupFieldId = lookupFieldId;
                lastQO.RelationshipType = QueryRelationshipType.Lookup;

                // Add new QO to the collection
                QueryObject newQO = CreateNewQueryObject(fullLookupAppObject);
                newQO.BreadCrumbLabel = lookupAppField.Name;

                WIPFilter.QueryObjects.Add(newQO);

                // Render Object and Field Browser
                RenderBreadCrumbsAndObjects(WIPFilter.QueryObjects, fullLookupAppObject, null);
            }
            else
            {
                // Inform the user to add the lookup object in Application Definition                
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("OBJECTFIELDVEW_Lookup_ErrorMsg"), lookupAppField.Name, lookupAppObject.Id), resourceManager.GetResource("OBJECTFIELDVEW_LookupCap_ErrorMsg"));
            }
        }

        private int GetDistanceFromChild()
        {
            return GetDeepestChildNodeLevel(tvObjects.Nodes[0]) - tvObjects.SelectedNode.Level - 1;
        }

        private int GetDeepestChildNodeLevel(TreeNode node)
        {
            var subLevel = node.Nodes.Cast<TreeNode>().Select<TreeNode, int>(subNode => GetDeepestChildNodeLevel(subNode));
            return subLevel.Count<int>() == 0 ? 1 : subLevel.Max() + 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ProcessFinalObjectIdAsLookup();
            controller.SearchFilter.QueryObjects = WIPFilter.QueryObjects;
            controller.SearchFilter.AppObjectUniqueId = WIPFilter.AppObjectUniqueId;
            controller.SearchFilter.FieldId = WIPFilter.FieldId;
            controller.SearchFilter.SearchFilterLabel = applicationDefinitionManager.GetField(WIPFilter.AppObjectUniqueId, WIPFilter.FieldId).Name;
            this.DialogResult = DialogResult.OK;
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }

        private void dgvFields_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvFields.SelectedCells.Count > 0)
                btnSave_Click(sender, e);
        }

        private void ProcessFinalObjectIdAsLookup()
        {
            ApttusObject obj = applicationDefinitionManager.GetAppObject(WIPFilter.AppObjectUniqueId);
            // The below condition is the entry condition to Process Final Object Id as Lookup
            // Field Id equals "Id" AND 
            //      (either more than one query objects are involed OR
            //      (single query object with Distance from Child more than 0)
            if (WIPFilter.FieldId == obj.IdAttribute &&
                (WIPFilter.QueryObjects.Count > 1 || (WIPFilter.QueryObjects.Count == 1 && WIPFilter.QueryObjects[0].DistanceFromChild > 0)))
            {
                bool bRemoveLastQQ = (WIPFilter.QueryObjects.Count > 1 ? true : false);
                int countQQ = WIPFilter.QueryObjects.Count;
                List<QueryObject> copyQueryObjects = WIPFilter.QueryObjects.Select(x => x.Clone()).ToList();
                QueryObject copyQQ = bRemoveLastQQ ? copyQueryObjects[countQQ - 2] : copyQueryObjects[countQQ - 1];
                int WIPQQIndex = bRemoveLastQQ ? countQQ - 2 : countQQ - 1;

                // Find new App Object. It will either be the Parent of 2nd Last QQ's Leaf App Object if Distance > 0, else the Leaf App Object itself.
                Guid newAppObjectId = copyQQ.LeafAppObjectUniqueId;
                if (copyQQ.DistanceFromChild > 0)
                {
                    if (copyQQ.RelationshipType == QueryRelationshipType.Lookup)
                    {
                        // In case of Lookup use the parent of the leaf
                        newAppObjectId = applicationDefinitionManager.GetAppObject(newAppObjectId).Parent.UniqueId;
                    }
                    else if (copyQQ.RelationshipType == QueryRelationshipType.Parent)
                    {
                        // In case of Parent traverse one level less than the root
                        for (int i = 0; i < copyQQ.DistanceFromChild - 1; i++)
                        {
                            ApttusObject childAppObject = applicationDefinitionManager.GetAppObject(newAppObjectId);
                            newAppObjectId = childAppObject.Parent.UniqueId;
                        }
                    }
                }
                ApttusObject newAppObject = applicationDefinitionManager.GetAppObject(newAppObjectId);

                // Now start making changes to the WIPFilter
                // 1. Assign new App Object to Filter App Object
                WIPFilter.AppObjectUniqueId = newAppObject.UniqueId;

                // 2. Assign new App Object to 2nd Last QQ's App Object
                WIPFilter.QueryObjects[WIPQQIndex].AppObjectUniqueId = newAppObject.UniqueId;

                // 3. Clear 2nd Last QQ's App Object's LookupFieldId
                WIPFilter.QueryObjects[WIPQQIndex].LookupFieldId = string.Empty;

                // 4. Assign 2nd Last QQ's App Object's RelationshipType
                WIPFilter.QueryObjects[WIPQQIndex].RelationshipType = (copyQQ.DistanceFromChild > 0 ? QueryRelationshipType.Parent : QueryRelationshipType.None);

                // 5. Assign Last QQ's App Object's DistanceFromChild in case of bRemoveLastQQ
                if (!bRemoveLastQQ)
                    WIPFilter.QueryObjects[WIPQQIndex].DistanceFromChild -= 1;

                // 6. Assign field as 2nd Last QQ's App Object's LookupFieldId
                WIPFilter.FieldId = bRemoveLastQQ ? copyQQ.LookupFieldId : newAppObject.LookupName;

                // 7. Delete the last QQ
                if (bRemoveLastQQ)
                    WIPFilter.QueryObjects.RemoveAt(countQQ - 1);

            }
        }

    }
}
