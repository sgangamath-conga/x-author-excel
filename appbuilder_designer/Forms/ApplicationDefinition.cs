/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner.Forms
{
    public partial class ApplicationDefinition : Form
    {
        ObjectManager objectManager = ObjectManager.GetInstance;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        List<ApttusFieldDS> currentFieldsDataSource = null;
        private Dictionary<string, string> FieldsFilter = new Dictionary<string, string>();
        private static TreeNode rightClickTreeNode = null;
        ObjectLayout oLayout = ObjectLayout.Hierarchical; // Hierarchical by default
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        List<ApttusObject> AllStandardApttusObjects = new List<ApttusObject>();

        private enum ObjectLayout
        {
            Hierarchical = 1,
            CrossTab = 2
        };

        public ApplicationDefinition()
        {
            InitializeComponent();
            InitializeValues();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;

            ObjectManager.ResetObjectCache();

            SetCultureData();

            if (applicationDefinitionManager.GetAppObjects().Count == 0)
                RenderForm(FormOpenMode.Create);
            else
                RenderForm(FormOpenMode.Edit);
        }

        private void SetCultureData()
        {
            btnAddCrossTab.Text = resourceManager.GetResource("APPDEF_btnAddCrossTab_Text");
            btnAddObject.Text = resourceManager.GetResource("APPDEF_btnAddObject_Text");
            btnAddParentObject.Text = resourceManager.GetResource("APPDEF_btnAddParentObject_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnRemoveObject.Text = resourceManager.GetResource("APPDEF_btnRemoveObject_Text");
            btnSaveAndClose.Text = resourceManager.GetResource("APPDEF_btnSaveAndClose_Text");
            btnSaveFields.Text = resourceManager.GetResource("COMMON_Apply_Text");
            grpCrossTab.Text = resourceManager.GetResource("APPDEF_grpCrossTab_Text");
            grpHierarchical.Text = resourceManager.GetResource("APPDEF_grpHierarchical_Text");
            lblCH.Text = resourceManager.GetResource("APPDEF_lblCH_Text");
            lblCrossTabName.Text = resourceManager.GetResource("COMMON_Name_Text");
            lblData.Text = resourceManager.GetResource("APPDEF_lblData_Text");
            lblFields.Text = resourceManager.GetResource("APPDEF_lblFields_Text");
            lblFieldsGrid.Text = resourceManager.GetResource("APPDEF_lblFieldsGrid_Text");
            lblFilterFieldsGrid.Text = resourceManager.GetResource("APPDEF_lblFilterFieldsGrid_Text");
            lblHeader.Text = resourceManager.GetResource("COMMON_SalesforceObject_Text",true);
            lblLegend.Text = resourceManager.GetResource("APPDEF_lblLegend_Text");
            lblObjectSelection.Text = resourceManager.GetResource("APPDEF_lblObjectSelection_Text");
            lblRH.Text = resourceManager.GetResource("APPDEF_lblRH_Text");
            lblSelectedObjects.Text = resourceManager.GetResource("COMMON_SelectedObjects_Text");

        }

        private void InitializeValues()
        {
            // 1. Populate Fields Filter
            FieldsFilter.Add("All", "Show All");
            FieldsFilter.Add("App", "Show App Fields");
            FieldsFilter.Add("NonApp", "Show Non-App Fields");
        }
        #region "Event Handlers"

        private void tvObjects_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode targetNode = e.Node;
            if (targetNode.Nodes.Count == 1 && targetNode.Nodes[0].Text == "dummy" && targetNode.Nodes[0].Tag == null)
            {
                tvObjects.BeginUpdate();

                // Clear the dummy node.
                targetNode.Nodes.Clear();

                // Get the full ApttusObject
                var ApttusObject = objectManager.GetApttusObject((targetNode.Tag as ApttusObject).Id, false);

                ((ApttusObject)targetNode.Tag).NoofChildObjectsLoaded = ApttusObject.NoofChildObjectsLoaded;

                FillObjectsTree(targetNode, ApttusObject.Children.ToList(), !ApttusObject.IsFullyLoaded);

                tvObjects.EndUpdate();
            }

            if (targetNode.Nodes.Count == 1 && targetNode.Tag != null && targetNode.Tag.ToString() == "LoadMore")
            {
                ApttusObject obj = targetNode.Parent.Tag as ApttusObject;

                tvObjects.BeginUpdate();

                // Clear the dummy node.
                targetNode.Nodes.Clear();

                // Get the ApttusObject
                var ApttusObject = objectManager.GetApttusObject(obj.Id, false, true, obj.NoofChildObjectsLoaded);

                // Update parent tag 
                ((ApttusObject)targetNode.Parent.Tag).NoofChildObjectsLoaded = ApttusObject.NoofChildObjectsLoaded;

                FillObjectsTree(targetNode.Parent, ApttusObject.Children.ToList(), !ApttusObject.IsFullyLoaded);

                //remove clicked load more node from main object tree
                targetNode.Remove();

                tvObjects.EndUpdate();
            }
        }

        private void btnAddParentObject_Click(object sender, EventArgs e)
        {
            ApttusObject oAddObject = applicationDefinitionManager.AddObject(GetSelectedObject(false));
            RenderApplicationObjects(oAddObject);
        }

        private void btnAddObject_Click(object sender, EventArgs e)
        {
            ApttusObject oAddObject = applicationDefinitionManager.AddObject(GetSelectedObject(true));
            RenderApplicationObjects(oAddObject);
        }

        //private void btnAddCrossTab_Click(object sender, EventArgs e)
        //{
        //    AddCrossTabObject();
        //}

        private void btnRemoveObject_Click(object sender, EventArgs e)
        {
            if (tvSelectedObjects.SelectedNode == null) return;

            if (!CanRemoveObject(tvSelectedObjects.SelectedNode.Tag as ApttusObject))
            {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("APPDEFFORM_btnRemoveObject_Click_InfoMsg"), resourceManager.GetResource("APPDEFFORM_btnRemoveObjectCAP_Click_InfoMsg"), this.Handle.ToInt32());
                return;
            }
            RemoveObject();
            RenderApplicationObjects(null);
        }

        private void tvSelectedObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvSelectedObjects.SelectedNode.Tag.GetType() == typeof(ApttusObject))
                RenderFields((ApttusObject)tvSelectedObjects.SelectedNode.Tag);
            else
                ClearFields();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            btnSaveFields_Click(sender, e);
            btnCancel_Click(sender, e);
        }

        internal bool CanUpdateFields(string objectId, Guid AppObjectId)
        {
            ApttusObject objectInCRM = objectManager.GetApttusObject(objectId, false);
            ApttusObject objectInApp = applicationDefinitionManager.GetAppObject(AppObjectId);

            List<ApplicationField> usedFields = ConfigurationManager.GetInstance.GetUsedFieldsOfObject(objectInApp, true);
            foreach (ApplicationField usedField in usedFields)
            {
                
                ApttusField apttusField = objectInApp.GetField(usedField.FieldId);
                if (applicationDefinitionManager.IsLookupField(apttusField) || usedField.FieldId.EndsWith(Constants.APPENDLOOKUPID))
                    continue;
                if (objectInCRM.Fields.Find(fldInCRM => usedField.FieldId == fldInCRM.Id) == null)
                    return false;
            }
            return true;
        }

        private void btnSaveFields_Click(object sender, EventArgs e)
        {
            if (currentFieldsDataSource != null && tvSelectedObjects.SelectedNode != null)
            {
                List<ApttusField> selection = (from f in currentFieldsDataSource
                                               where f.Included == true
                                               select f).Cast<ApttusField>().ToList<ApttusField>();

                // Get All Lookup Fields from User Selection & get Primary Name field for each Lookup and set under ApttusField
                //List<ApttusField> lookupFields = selection.Where(r => r.Datatype == Datatype.Lookup).ToList();
                //lookupFields.AddRange(((ApttusObject)tvSelectedObjects.SelectedNode.Tag).Fields.Where(r => r.Datatype == Datatype.Lookup).ToList());
                //foreach (ApttusField item in lookupFields)
                //{
                //    ApttusObject fieldLookupObject = AllStandardApttusObjects.Where(r => r.Id == item.LookupObject.Id).FirstOrDefault();
                //    // Assign Id and Name attribute of the object
                //    item.LookupObject.IdAttribute = string.IsNullOrEmpty(fieldLookupObject.IdAttribute) ? string.Empty : fieldLookupObject.IdAttribute;
                //    item.LookupObject.NameAttribute = string.IsNullOrEmpty(fieldLookupObject.NameAttribute) ? string.Empty : fieldLookupObject.NameAttribute;

                //    // Multiple Entity Reference Field
                //    if (item.MultiLookupObjects != null && item.MultiLookupObjects.Count > 0)
                //    {
                //        if (tvSelectedObjects.SelectedNode.Tag is ApttusObject)
                //        {
                //            ApttusObject currentAppObject = (ApttusObject)tvSelectedObjects.SelectedNode.Tag;
                //            List<ApttusObject> appObjectsList = applicationDefinitionManager.GetAppObjectById(currentAppObject.Id);
                //            if (!appObjectsList.Any(f => f.Parent == null) && currentAppObject.LookupName == item.Id)
                //            {
                //                List<ApttusObject> multiLookupObjects = new List<ApttusObject>();
                //                foreach (ApttusObject appObject in appObjectsList)
                //                {
                //                    if (currentAppObject.Parent != null)
                //                        multiLookupObjects.Add(item.MultiLookupObjects.FirstOrDefault(f => f.Id == currentAppObject.Parent.Id));
                //                }
                //                //item.MultiLookupObjects = multiLookupObjects;
                //            }
                //        }
                //        foreach (ApttusObject referenceObject in item.MultiLookupObjects)
                //        {
                //            ApttusObject parentRefObject = AllStandardApttusObjects.Where(r => r.Id == referenceObject.Id).FirstOrDefault();
                //            referenceObject.IdAttribute = string.IsNullOrEmpty(parentRefObject.IdAttribute) ? string.Empty : parentRefObject.IdAttribute;
                //            referenceObject.NameAttribute = string.IsNullOrEmpty(parentRefObject.NameAttribute) ? string.Empty : parentRefObject.NameAttribute;
                //            referenceObject.Name = parentRefObject.Name; // To Display on Search and Select Drop down list
                //        }
                //    }
                //}

                ApttusObject oApttusObject = tvSelectedObjects.SelectedNode.Tag as ApttusObject;

                if (!CanUpdateFields(oApttusObject.Id, oApttusObject.UniqueId))
                {
                    string message = resourceManager.GetResource("APPDEF_ObjectNotInSync_InfoMsg");
                    ApttusMessageUtil.ShowInfo(message, Constants.DESIGNER_PRODUCT_NAME);
                    return;
                }

                FillRecordTypeMetadata(selection, oApttusObject); // Pull Record type metadata if Record Type field is added.
                applicationDefinitionManager.UpdateFields(oApttusObject, (List<ApttusField>)selection);
                applicationDefinitionManager.Save();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //private void cmboRowHeader_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    FillDataSection(GetDataSection());
        //}

        //private void cmboColHeader_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    FillDataSection(GetDataSection());
        //}

        private void independentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToogleObjectType(rightClickTreeNode, ObjectType.Independent);
            rightClickTreeNode = null;
        }

        private void repeatingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToogleObjectType(rightClickTreeNode, ObjectType.Repeating);
            rightClickTreeNode = null;
        }

        private void tvSelectedObjects_MouseUp(object sender, MouseEventArgs e)
        {
            rightClickTreeNode = tvSelectedObjects.GetNodeAt(e.X, e.Y);
        }

        private void txtCrossTabName_Validating(object sender, CancelEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(txtCrossTabName.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtCrossTabName, resourceManager.GetResource("COMMON_NameCannotBeEmpty_ErrorMsg"));
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtCrossTabName, String.Empty);
            }
        }

        private void rbLayout_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbLayout = sender as RadioButton;

            oLayout = rbLayout.Text.Equals(ObjectLayout.Hierarchical.ToString()) ? ObjectLayout.Hierarchical : ObjectLayout.CrossTab;
            ShowHideObjects(oLayout);
        }

        #endregion

        #region "Private Methods"

        private void RenderForm(FormOpenMode formOpenMode)
        {
            RenderFieldsFilter();

            switch (formOpenMode)
            {
                case FormOpenMode.Create:
                    RenderObjects(string.Empty);
                    break;
                case FormOpenMode.Edit:
                    RenderObjects(string.Empty);
                    RenderApplicationObjects(null);
                    break;
                case FormOpenMode.ReadOnly:
                    break;
            }

            // Show Hierarchical Layout by default.
            ShowHideObjects(ObjectLayout.Hierarchical);
        }

        private void RenderObjects(string selectedObject)
        {
            // If selectedObject is null, default rendering
            if (string.IsNullOrEmpty(selectedObject))
            {
                AllStandardApttusObjects = objectManager.GetAllStandardObjects();

                FillObjectsTree(null, AllStandardApttusObjects); // Bind TreeView
                //FillCrossTabDropdown(AllStandardApttusObjects); // Bind CrossTab Dropdowns
            }
            // If selectedObject is not null, render all objects and select selectedObject
            else
            {

            }
        }

        private void RenderApplicationObjects(ApttusObject selectedAppObject)
        {
            TreeNode selectedNode = null;
            tvSelectedObjects.Nodes.Clear();
            // 1. Display all Hierarchical Objects
            foreach (ApttusObject oRootApttusObject in applicationDefinitionManager.GetAppObjects())
            {
                TreeNode root = AddRoot(oRootApttusObject, tvSelectedObjects);

                if (oRootApttusObject.Children.Count > 0)
                    foreach (ApttusObject oChildObject in oRootApttusObject.Children)
                        AddChild(oChildObject, root);
            }
            // 2. Display all CrossTab Objects
            //foreach (CrossTabDef xDef in applicationDefinitionManager.CrossTabDefinitions)
            //{
            //    DisplayCrossTabs(xDef);
            //}

            //3. Set selection for App Objects Tree
            if (tvSelectedObjects.Nodes.Count > 0)
            {
                selectedNode = GetTreeNode(tvSelectedObjects.Nodes, selectedAppObject);
                tvSelectedObjects.HideSelection = false;
                tvSelectedObjects.SelectedNode = selectedNode;
                selectedNode.ExpandAll();
            }
            else
            {
                apttusFieldsView.UpdateDataSource(null);
                currentFieldsDataSource = null;
                //dgvFields.DataSource = ;
            }
        }

        private void FillRecordTypeMetadata(List<ApttusField> fields, ApttusObject apttusObject)
        {
            bool bAddRecordTypeMetadata = false;

            if (apttusObject.Fields == null)
                bAddRecordTypeMetadata = true;
            else
            {
                // Add only IF Record Type is selected to be added AND IF Record Type doesn't already exist in the App Object
                if (fields.Exists(f => f.RecordType) && !apttusObject.Fields.Exists(f => f.RecordType))
                    bAddRecordTypeMetadata = true;
            }

            if (bAddRecordTypeMetadata)
                objectManager.FillRecordTypeMetadata(apttusObject);
        }

        //private void DisplayCrossTabs(CrossTabDef xTab)
        //{
        //    //open an existing xtab app needs to show the xtab in the ui
        //    //if (xTab == null) return;
        //    //CrossTabName.Text = xTab.Name;
        //    //init = true; // selected index change will trigger the filldata section()
        //    //RowHeader.SelectedIndex = RowHeader.FindStringExact(xTab.RowHeaderObject.Name);
        //    //ColHeader.SelectedIndex = ColHeader.FindStringExact(xTab.ColHeaderObject.Name);
        //    //init = false;
        //    //DataList.DataSource = null;

        //    //List<ApttusObject> DataObj = new List<ApttusObject>();
        //    //DataObj.Add(xTab.DataObject);
        //    FillDataSection(new List<ApttusObject> { xTab.DataObject });
        //    ShowXTabinTree(xTab.RowHeaderObject, xTab.ColHeaderObject, xTab.DataObject, xTab);
        //}

        //private void FillDataSection(List<ApttusObject> dataSection)
        //{
        //    if (dataSection == null) return;
        //    //if (init) return;

        //    lstData.DataSource = null;
        //    if (dataSection.Count > 0)
        //    {
        //        lstData.DataSource = dataSection;
        //        lstData.DisplayMember = Constants.NAME_ATTRIBUTE;
        //        lstData.ValueMember = Constants.ID_ATTRIBUTE;
        //    }

        //}

        //private void ShowXTabinTree(ApttusObject cbRow, ApttusObject cbCol, ApttusObject cbData, CrossTabDef cb)
        //{
        //    TreeNode tnRow = new TreeNode
        //    {
        //        ToolTipText = cbRow.LookupName,
        //        Text = cbRow.Name + " (" + cbRow.Id + ")",
        //        Tag = cbRow
        //    };
        //    TreeNode tnCol = new TreeNode
        //    {
        //        ToolTipText = cbCol.LookupName,
        //        Text = cbCol.Name + " (" + cbCol.Id + ")",
        //        Tag = cbCol
        //    };
        //    TreeNode tnData = new TreeNode
        //    {
        //        ToolTipText = cbData.LookupName,
        //        Text = cbData.Name + " (" + cbData.Id + ")",
        //        Tag = cbData
        //    };

        //    TreeNode[] array = new TreeNode[] { tnRow, tnCol, tnData };
        //    TreeNode CTNode = new TreeNode();
        //    CTNode.Text = cb.Name;
        //    CTNode.Tag = cb;
        //    CTNode.Nodes.Add(tnRow);
        //    CTNode.Nodes.Add(tnCol);
        //    CTNode.Nodes.Add(tnData);
        //    tvSelectedObjects.Nodes.Add(CTNode);
        //}

        //private List<ApttusObject> GetDataSection()
        //{
        //    //TODO: list.instersect is not working for some reason, need
        //    //to do some R&D on this, added workaround by addin hashset
        //    string ColObjectId = cmboColHeader.SelectedValue as string;
        //    string RowObjectId = cmboRowHeader.SelectedValue as string;

        //    if (string.IsNullOrEmpty(RowObjectId) || string.IsNullOrEmpty(ColObjectId))
        //        return null;
        //    if (ColObjectId.Equals(RowObjectId))
        //        return null;

        //    // Set cursor as hourglass
        //    Cursor.Current = Cursors.WaitCursor;
        //    try
        //    {
        //        var oRowObject = objectManager.GetApttusObject(RowObjectId, false);
        //        var oColObject = objectManager.GetApttusObject(ColObjectId, false);
        //        List<ApttusObject> oDataObjects = (from row in oRowObject.Children
        //                                           join col in oColObject.Children on row.Id equals col.Id
        //                                           select row).ToList();

        //        oDataObjects.Sort(delegate(ApttusObject AO1, ApttusObject AO2)
        //        {
        //            return AO1.Name.CompareTo(AO2.Name);
        //        });

        //        return oDataObjects;

        //        //List<ApttusObject> data1 = oRowObject.Children.ToList();
        //        //List<ApttusObject> data2 = oColObject.Children.ToList();
        //        //List<ApttusObject> data3 = data1.Concat(data2).ToList();
        //        //List<ApttusObject> dup = new List<ApttusObject>();

        //        //var hashset = new HashSet<string>();
        //        //foreach (var item in data3)
        //        //{
        //        //    if (!hashset.Add(item.Name))
        //        //        dup.Add(item);
        //        //}
        //        //return dup;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        // Set cursor as hourglass
        //        Cursor.Current = Cursors.Default;
        //    }
        //}

        private TreeNode GetTreeNode(TreeNodeCollection tNodes, ApttusObject selectedAppObject)
        {
            TreeNode selectedTreeNode = null;
            if (selectedAppObject == null)
            {
                selectedTreeNode = tNodes[0];
            }
            else
            {
                foreach (TreeNode tn in tNodes)
                {
                    if ((tn.Tag as ApttusObject).UniqueId == selectedAppObject.UniqueId)
                    {
                        selectedTreeNode = tn;
                        break;
                    }
                    else if (tn.Nodes.Count > 0)
                    {
                        selectedTreeNode = GetTreeNode(tn.Nodes, selectedAppObject);
                        if (selectedTreeNode != null)
                            break;
                    }
                }
            }
            return selectedTreeNode;
        }

        private void FillObjectsTree(TreeNode targetNode, List<ApttusObject> apttusObjects, bool shallAddLoadMoreLink = false)
        {
            //foreach (ApttusObject oApttusObject in apttusObjects)
            //{
            //    ApttusObject appObject = oApttusObject;
            //    if (ApttusObjects.Any(f => f.Id == oApttusObject.Id))
            //    {
            //        appObject = ApttusObjects.FirstOrDefault(f => f.Id == oApttusObject.Id);
            //        oApttusObject.Name = appObject.Name;
            //    }
            //}

            apttusObjects.Sort(delegate (ApttusObject AO1, ApttusObject AO2)
            {
                return AO1.Name.CompareTo(AO2.Name);
            });

            foreach (ApttusObject oApttusObject in apttusObjects)
            {
                oApttusObject.Parent = null;
                TreeNode tn = new TreeNode
                {
                    ToolTipText = oApttusObject.Id,
                    Text = targetNode == null ? oApttusObject.Name : oApttusObject.NamePlusLookupName,
                    Tag = oApttusObject
                };
                TreeNode dummy = new TreeNode("dummy");
                tn.Nodes.Add(dummy);

                if (targetNode == null)
                    tvObjects.Nodes.Add(tn);
                else
                    targetNode.Nodes.Add(tn);
            }

            /* if Object is not fully loaded with child object in that case to fetch remaining objects system will add addmorelink 
            as last child object. */
            if (shallAddLoadMoreLink)
                AddLoadMoreLink(targetNode);

        }

        void AddLoadMoreLink(TreeNode targetNode)
        {
            TreeNode tn = new TreeNode
            {
                ToolTipText = resourceManager.GetResource("APPDEF_AddLoadMoreLink_Text") + targetNode.ToolTipText,
                Text = resourceManager.GetResource("APPDEF_AddLoadMoreLink_Text"),
                Tag = "LoadMore",
                BackColor = Color.AliceBlue
            };

            TreeNode dummy = new TreeNode("LoadMore");
            tn.Nodes.Add(dummy);

            targetNode.Nodes.Add(tn);
        }

        //private void FillCrossTabDropdown(List<ApttusObject> apttusObjects)
        //{
        //    apttusObjects.Sort(delegate(ApttusObject AO1, ApttusObject AO2)
        //    {
        //        return AO1.Name.CompareTo(AO2.Name);
        //    });

        //    cmboRowHeader.BindingContext = new BindingContext();
        //    cmboRowHeader.DataSource = apttusObjects;
        //    cmboRowHeader.DisplayMember = Constants.NAME_ATTRIBUTE;
        //    cmboRowHeader.ValueMember = Constants.ID_ATTRIBUTE;


        //    cmboColHeader.BindingContext = new BindingContext();
        //    cmboColHeader.DataSource = apttusObjects;
        //    cmboColHeader.DisplayMember = Constants.NAME_ATTRIBUTE;
        //    cmboColHeader.ValueMember = Constants.ID_ATTRIBUTE;
        //}

        //public void AddCrossTabObject()
        //{
        //    ApttusObject cbRow = null;
        //    ApttusObject cbCol = null;
        //    ApttusObject cbData = null;


        //    if (cmboRowHeader.SelectedValue != null)
        //    {
        //        cbRow = objectManager.GetApttusObject((string)cmboRowHeader.SelectedValue, false);
        //        cbRow = applicationDefinitionManager.GetCleanCrossTabObject(cbRow);
        //        cbRow.ObjectType = ObjectType.CrossTab;
        //    }
        //    if (cmboColHeader.SelectedValue != null)
        //    {
        //        cbCol = objectManager.GetApttusObject((string)cmboColHeader.SelectedValue, false);
        //        cbCol = applicationDefinitionManager.GetCleanCrossTabObject(cbCol);
        //        cbCol.ObjectType = ObjectType.CrossTab;
        //    }
        //    if (lstData.SelectedValue != null)
        //    {
        //        cbData = objectManager.GetApttusObject((string)lstData.SelectedValue, false);
        //        cbData = applicationDefinitionManager.GetCleanCrossTabObject(cbData);
        //        cbData.ObjectType = ObjectType.CrossTab;
        //    }

        //    if (cbRow != null && cbData != null && cbCol != null)
        //    {
        //        CrossTabDef cb = new CrossTabDef();
        //        cb.UniqueId = Guid.NewGuid();
        //        cb.RowHeaderObject = cbRow;
        //        cb.ColHeaderObject = cbCol;
        //        cb.DataObject = cbData;
        //        cb.Name = txtCrossTabName.Text;
        //        if (ObjectExists(cb))
        //        {
        //            ApttusMessageUtil.ShowError(resourceManager.GetResource("MNUHOME_msgDuplicate"), resourceManager.GetResource("MNUHOME_TitleDuplicate"));
        //            return;
        //        }
        //        applicationDefinitionManager.AddCrossTabObject(cb);

        //        ShowXTabinTree(cbRow, cbCol, cbData, cb);
        //    }
        //}

        private bool ObjectExists(CrossTabDef def)
        {
            CrossTabDef defexists = (applicationDefinitionManager.CrossTabDefinitions.Find(item => item.Name.Equals(def.Name)));
            if (defexists != null)
                return true;
            else
                return false;

        }

        private TreeNode AddRoot(ApttusObject oApttusObject, TreeView Parent)
        {
            TreeNode tn = new TreeNode
            {
                ToolTipText = oApttusObject.Id,
                Tag = oApttusObject
            };
            CreateContextMenu(tn);
            UpdateTreeNodeAndContextMenu(tn);

            Parent.Nodes.Add(tn);
            return tn;
        }

        private void AddChild(ApttusObject oApttusObject, TreeNode Parent)
        {
            TreeNode tn = new TreeNode
            {
                ToolTipText = oApttusObject.LookupName,
                Tag = oApttusObject
            };
            CreateContextMenu(tn);
            UpdateTreeNodeAndContextMenu(tn);

            Parent.Nodes.Add(tn);
            if (oApttusObject.Children.Count > 0)
                foreach (ApttusObject oChildObject in oApttusObject.Children)
                    AddChild(oChildObject, tn);
        }

        private void ToogleObjectType(TreeNode tn, ObjectType requestedObjectType)
        {
            if (tn != null)
            {
                ApttusObject selectedObject = (ApttusObject)tn.Tag;
                switch (requestedObjectType)
                {
                    case ObjectType.Independent:
                        if (selectedObject.ObjectType == ObjectType.Repeating)
                            selectedObject.ObjectType = ObjectType.Independent;
                        break;
                    case ObjectType.Repeating:
                        if (selectedObject.ObjectType == ObjectType.Independent)
                            selectedObject.ObjectType = ObjectType.Repeating;
                        break;
                    default:
                        break;
                }
                UpdateTreeNodeAndContextMenu(tn);
            }
        }

        private void CreateContextMenu(TreeNode tn)
        {
            ApttusObject oApttusObject = tn.Tag as ApttusObject;
            ContextMenuStrip ContextMenu = new ContextMenuStrip();

            List<ToolStripMenuItem> ContextMenuItems = new List<ToolStripMenuItem>();

            ObjectType objType = ObjectType.Independent;
            ToolStripMenuItem Independent = new ToolStripMenuItem() { Text = Utils.GetEnumDescription(objType, string.Empty) };
            Independent.Click += new EventHandler(this.independentToolStripMenuItem_Click);

            objType = ObjectType.Repeating;
            ToolStripMenuItem Repeating = new ToolStripMenuItem() { Text = Utils.GetEnumDescription(objType, string.Empty) };
            Repeating.Click += new EventHandler(this.repeatingToolStripMenuItem_Click);

            ContextMenuItems.Add(Independent);
            ContextMenuItems.Add(Repeating);

            ContextMenu.Items.AddRange(ContextMenuItems.ToArray());
            tn.ContextMenuStrip = ContextMenu;
        }

        private void UpdateTreeNodeAndContextMenu(TreeNode tn)
        {
            ApttusObject oApttusObject = tn.Tag as ApttusObject;
            ContextMenuStrip contextMenu = tn.ContextMenuStrip;

            // 1. Set Context Menu Checked property
            if (contextMenu != null)
            {
                switch (oApttusObject.ObjectType)
                {
                    case ObjectType.Independent:
                        foreach (ToolStripItem item in contextMenu.Items)
                        {
                            if (item.Text == Utils.GetEnumDescription(ObjectType.Independent, string.Empty))
                                (item as ToolStripMenuItem).Checked = true;
                            if (item.Text == Utils.GetEnumDescription(ObjectType.Repeating, string.Empty))
                                (item as ToolStripMenuItem).Checked = false;
                        }
                        break;
                    case ObjectType.Repeating:
                        foreach (ToolStripItem item in contextMenu.Items)
                        {
                            if (item.Text == Utils.GetEnumDescription(ObjectType.Independent, string.Empty))
                                (item as ToolStripMenuItem).Checked = false;
                            if (item.Text == Utils.GetEnumDescription(ObjectType.Repeating, string.Empty))
                                (item as ToolStripMenuItem).Checked = true;
                        }
                        break;
                    default:
                        break;
                }

                // 2. Set the TreeNode Text
                tn.Text = oApttusObject.Name + " (" + Utils.GetEnumDescription(oApttusObject.ObjectType, string.Empty) + ")";
            }
        }

        private void RenderFieldsFilter()
        {
            //cmbFilterFields.DataSource = new BindingSource(FieldsFilter, null);
            //cmbFilterFields.DisplayMember = "value";
            //cmbFilterFields.ValueMember = "key";

            //// ToDo: remove this code once other filter values are implemented
            //cmbFilterFields.Enabled = false;
            //cmbFilterFields.Visible = false;
        }

        private List<ApplicationField> GetUsedFieldsOfObject(ApttusObject obj)
        {
            List<ApplicationField> usedFields = ConfigurationManager.GetInstance.GetUsedFieldsOfObject(obj, true, true);
            return usedFields;
        }

        private List<ApplicationField> GetUsedFields(ApttusObject obj)
        {
            List<ApplicationField> usedFields = new List<ApplicationField>();
            foreach (ApttusObject item in applicationDefinitionManager.GetAppObjectById(obj.Id))
            {
                usedFields.AddRange(ConfigurationManager.GetInstance.GetUsedFieldsOfObject(item, true, true));
            }
            usedFields = usedFields.GroupBy(f => f.FieldId).Select(s => s.First()).ToList();
            return usedFields;
        }

        private void RenderFields(ApttusObject AppObject)
        {
            if (AllStandardApttusObjects != null && AllStandardApttusObjects.Exists(standardObject => standardObject.Id == AppObject.Id))
            {
                ApttusObject fullObject = objectManager.GetApttusObject(AppObject.Id, false);
                // Render Object Metadata
                RenderObjectMetadata(AppObject);

                // Update record Types , if records types were created after Object has already been added to App.
                if (AppObject.RecordTypes != null && fullObject.RecordTypes != null && AppObject.RecordTypes.Count != fullObject.RecordTypes.Count)
                    AppObject.RecordTypes = fullObject.RecordTypes;

                // Render Fields Grid
                currentFieldsDataSource = GetFieldsList(AppObject, fullObject.Fields);
                List<string> usedFields = applicationDefinitionManager.DefaultFields(AppObject);
                List<ApplicationField> appFields = GetUsedFields(AppObject);
                usedFields.AddRange(fullObject.Fields.Where(fld => appFields.Exists(appField => appField.FieldId.Equals(fld.Id))).Select(fld => fld.Id).ToList());
                apttusFieldsView.AppObject = fullObject;
                apttusFieldsView.RenderFieldsGrid(ref currentFieldsDataSource, usedFields);
            }
            else
            {
                string msg = string.Format(resourceManager.GetResource("APPDEF_Object_Doesn't_Exist_Msg"), AppObject.Name, AppObject.Id);
                ApttusMessageUtil.ShowInfo(msg, Constants.DESIGNER_PRODUCT_NAME);
            }
        }

        private void RenderObjectMetadata(ApttusObject AppObject)
        {
            if (AppObject == null)
                // Clear Metadata if not object is selected
                lblObjectMetadata.Text = string.Empty;
            else
                lblObjectMetadata.Text = "Name: " + AppObject.Name + " [Id: " + AppObject.Id + "]";
        }

        public void ClearFields()
        {
            RenderObjectMetadata(null);
            currentFieldsDataSource = null;
            apttusFieldsView.UpdateDataSource(currentFieldsDataSource);
            //dgvFields.DataSource = ;
        }

        private List<ApttusFieldDS> GetFieldsList(ApttusObject AppObject, List<ApttusField> allFields)
        {
            List<ApttusFieldDS> list = new List<ApttusFieldDS>();
            if (AppObject.Fields != null)
            {
                foreach (ApttusField field in allFields)
                {
                    ApttusFieldDS FieldDS = applicationDefinitionManager.CloneApttusField(field);
                    if (AppObject.Fields.Exists(a => a.Id == field.Id))
                        FieldDS.Included = true;
                    list.Add(FieldDS);
                }
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }
            else
            {
                foreach (ApttusField field in allFields)
                {
                    ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype, field.Updateable, field.FormulaType, field.CRMDataType, field.Creatable);
                    list.Add(FieldDS);
                }
                return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
            }

        }

        private ApttusObject GetSelectedObject(bool FullHierarchy)
        {
            ApttusObject oApttusObject = null;
            if (tvObjects.SelectedNode != null)
            {
                oApttusObject = GetObjectFromTree(tvObjects.SelectedNode, null, FullHierarchy);

                // Set default hierarchy
                SetDefaultHierarchy(oApttusObject);
            }
            return oApttusObject;
        }

        private ApttusObject GetObjectFromTree(TreeNode treeNode, ApttusObject oChildApttusObject, bool FullHierarchy)
        {
            // Get Full Object with fields and child relationships upfront.
            ApttusObject treeNodeApttusObject = treeNode.Tag as ApttusObject;
            ApttusObject oApttusObject = objectManager.GetApttusObject(treeNodeApttusObject.Id, false);
            oApttusObject.Children.Clear();
            oApttusObject.Parent = null;

            if (oChildApttusObject != null)
            {
                oApttusObject.Children.Add(oChildApttusObject);
            }

            // Set Parent Child relationship if Parent exists AND FullHierarchy is specified.
            if (treeNode.Parent != null & FullHierarchy)
            {
                // Add the LookupName field (we are in this block, as we are dealing with Parent Child Hierarchy in App Definition)
                oApttusObject.LookupName = treeNodeApttusObject.LookupName;
                oApttusObject = GetObjectFromTree(treeNode.Parent, oApttusObject, FullHierarchy);
            }

            return oApttusObject;
        }

        private void SetDefaultHierarchy(ApttusObject oApttusObject)
        {
            if (oApttusObject.Children.Count == 0)
                // Set the leaf object as Repeating object type
                oApttusObject.ObjectType = ObjectType.Repeating;
            else
            {
                // Set all objects as Independent object type except the leaf object
                oApttusObject.ObjectType = ObjectType.Independent;
                SetDefaultHierarchy(oApttusObject.Children[0]);
            }
        }

        private void dgvFields_DataSourceChanged(object sender, EventArgs e)
        {

        }

        public bool CanRemoveObject(ApttusObject obj)
        {
            bool bCanRemoveObject = false;
            List<ApplicationField> usedFields = GetUsedFieldsOfObject(obj);
            if (usedFields.Count == 0) //If there are no used fields, we can remove the object, but there is another catch.
            {
                bCanRemoveObject = true;
                List<ApttusObject> childObjectsFullHierarchy = applicationDefinitionManager.GetFullHierarchyObjects(obj);
                foreach (ApttusObject childObj in childObjectsFullHierarchy)
                {
                    bCanRemoveObject = bCanRemoveObject && GetUsedFieldsOfObject(childObj).Count == 0;
                    if (!bCanRemoveObject)
                        break;
                }
            }
            return bCanRemoveObject;
        }

        public void RemoveObject()
        {
            if (tvSelectedObjects.SelectedNode == null) return;
            if (tvSelectedObjects.SelectedNode.Tag != null)
            {

                //if (tvSelectedObjects.SelectedNode.Tag.GetType().Equals(typeof(CrossTabDef)))
                //{  //xtab object
                //    applicationDefinitionManager.RemoveCrossTabObject(tvSelectedObjects.SelectedNode.Tag as CrossTabDef);
                //    tvSelectedObjects.SelectedNode.Remove();
                //}
                //else if (
                //    tvSelectedObjects.SelectedNode.Parent != null && tvSelectedObjects.SelectedNode.Parent.Tag.GetType().Equals(typeof(CrossTabDef)))
                //{
                //    // child nodes
                //    applicationDefinitionManager.RemoveCrossTabObject(tvSelectedObjects.SelectedNode.Parent.Tag as CrossTabDef);
                //    tvSelectedObjects.SelectedNode.Parent.Remove();
                //    //tvSelectedObjects.Nodes.Clear();

                //}
                //else
                {
                    applicationDefinitionManager.RemoveObject((ApttusObject)tvSelectedObjects.SelectedNode.Tag);
                    tvSelectedObjects.SelectedNode.Remove();
                }

            }
            else // should never get here...
            {
                tvSelectedObjects.SelectedNode.Remove();
                applicationDefinitionManager.RemoveObject((ApttusObject)tvSelectedObjects.SelectedNode.Tag);

            }
        }

        private void ShowHideObjects(ObjectLayout oLayout)
        {
            switch (oLayout)
            {
                case ObjectLayout.Hierarchical:
                    grpHierarchical.Visible = true;
                    grpCrossTab.Visible = false;
                    break;
                case ObjectLayout.CrossTab:
                    grpHierarchical.Visible = false;
                    grpCrossTab.Visible = true;
                    if (string.IsNullOrEmpty(txtCrossTabName.Text))
                        txtCrossTabName.Focus();
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void ApplicationDefinition_Load(object sender, EventArgs e)
        {
            apttusFieldsView.SetCultureData();
            if (tvSelectedObjects.SelectedNode != null)
            {
                ApttusObject obj = tvSelectedObjects.SelectedNode.Tag as ApttusObject;
                if (obj != null)
                    RenderFields(obj);
            }
            tvSelectedObjects.AfterSelect += tvSelectedObjects_AfterSelect;
        }
    }
}
