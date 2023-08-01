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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DMDependencyIdentifier : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        DataMigrationController controller;
        DataMigrationModel Model;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        public void SetController(DataMigrationController controller, DataMigrationModel Model)
        {
            this.controller = controller;
            this.Model = Model;
        }

        public DMDependencyIdentifier()
        {
            InitializeComponent();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnSave.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnSave_Text");
            btnCancel.Text = resourceManager.GetResource("MULTISELECTPICKLIST_btnCancel_Text");
            lblTitle.Text = resourceManager.GetResource("DM_AddLookups_Text");
            lblLookupDescription.Text = resourceManager.GetResource("DM_LookupDescription_Text");
        }

        void PopulateLookupObjects()
        {
            foreach (MigrationObject migrationObject in Model.MigrationObjects.Where(x => !x.IsCloned))
            {
                List<MigrationLookup> lookupList = controller.GetExcludedLookUpObjects(migrationObject);

                TreeNode migrationObjectnode = new TreeNode(migrationObject.ObjectName);
                migrationObjectnode.ToolTipText = migrationObject.ObjectId;
                migrationObjectnode.Tag = migrationObject;

                foreach (MigrationLookup lookupObj in lookupList)
                {
                    TreeNode node = migrationObjectnode.Nodes.Add(string.Format("{0} ({1})", lookupObj.LookupName, lookupObj.LookupId));
                    node.ToolTipText = lookupObj.LookupObjectId;
                    node.Tag = lookupObj;
                }

                if (migrationObjectnode.Nodes.Count > 0)
                    trLookup.Nodes.Add(migrationObjectnode);
            }
        }

        private void DMDependencyIdentifier_Load(object sender, EventArgs e)
        {
            PopulateLookupObjects();
            trLookup.ExpandAll();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;
            foreach (TreeNode node in trLookup.Nodes)
            {
                MigrationObject migrationObject = node.Tag as MigrationObject;
                ApttusObject aObj = ObjectManager.GetInstance.GetApttusObject(migrationObject.ObjectId, false, false);
                ApttusObject appDefObj = applicationDefinitionManager.GetAppObject(migrationObject.AppObjectUniqueId);

                foreach (TreeNode childNode in node.Nodes)
                {
                    if (childNode.Checked)
                    {
                        MigrationLookup lookup = childNode.Tag as MigrationLookup;

                        ApttusField lookupField = aObj.Fields.Find(fld => fld.Datatype == Datatype.Lookup &&
                            fld.LookupObject != null &&
                            fld.Id == lookup.LookupId &&
                            fld.Name == lookup.LookupName &&
                            fld.LookupObject.Id == lookup.LookupObjectId);

                        if (lookupField == null)
                            continue;

                        MigrationObject cyclicDependentObject;
                        ApttusField cyclicDependentField;

                        if (controller.CyclicDependencyExistsFor(migrationObject, new List<ApttusField>() { lookupField }, out cyclicDependentObject, out cyclicDependentField))
                        {
                            string circularReferenceMsg = resourceManager.GetResource("CIRCULAR_REFERENCE_MSG");
                            string errorMessage = string.Format(resourceManager.GetResource("APTTUSFIELDS_CIRCULARREFERENCE_ErrMsg"), migrationObject.ObjectName, cyclicDependentObject.ObjectName, cyclicDependentField.Name, cyclicDependentField.Id);
                            ApttusMessageUtil.ShowInfo(errorMessage, circularReferenceMsg, this.Handle.ToInt64());
                        }
                        else
                        {
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            appDefObj.Fields.Add(lookupField);

                            migrationObject.Fields.Add
                                (
                                    new MigrationField() { MigrationObjectId = migrationObject.Id, FieldId = lookupField.Id, FieldName = lookupField.Name, DataType = lookupField.Datatype }
                                );
                            controller.SyncFieldsForClonedObjects(appDefObj, migrationObject, controller.GetFieldsList(migrationObject, appDefObj.Fields).ToList());
                            controller.SyncMigrationLookup(migrationObject);
                        }
                    }
                }
            }
            controller.ResolveObjectDependency();
            controller.UpdateWizardView();
        }
    }
}
