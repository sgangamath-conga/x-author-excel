/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ObjectSelectionPage : ApttusWizardPageView
    {
        private ApplicationDefinitionManager applicationDefinitionManager;
        private ObjectManager objectManager;
        List<ApttusFieldDS> Fields = new List<ApttusFieldDS>();
        ApttusObject ChildParentObject; //Child's Master Object. For Master Object this will always be null.
        public bool IsWizardInEditMode { get; set; }
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private List<ApttusObject> AllApttusObjects;

        public ObjectSelectionPage(Panel view, WizardModel model, PageIndex index, bool editMode)
            : base(view, model, index)
        {
            IsWizardInEditMode = editMode;
            InitializeComponent();
            SetCultureData();
            applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
            objectManager = ObjectManager.GetInstance;
            threadToUpdateFields.DoWork += threadToUpdateFields_DoWork;
            AllApttusObjects = objectManager.GetAllStandardObjects().OrderBy(field => field.Name).ToList();
        }

        private void SetCultureData()
        {
            lblLoadingFields.Text = resourceManager.GetResource("QAOBJSEL_lblLoadingFields_Text");
            lblObject.Text = resourceManager.GetResource("QAOBJSEL_lblObject_Text");
        }

        private void ObjectSelectionPage_Load(object sender, EventArgs e)
        {
            this.apttusFieldsView1.SetCultureData();
            if (PageNumber == PageIndex.ParentObjectSelectionPage)
                LoadParentObjects();

            cboObject.SelectedIndex = -1;

            this.cboObject.SelectedIndexChanged += new System.EventHandler(this.cboObject_SelectedIndexChanged);
            this.ApttusWizardPageActivate += ObjectSelectionPage_ApttusWizardPageActivate;
            this.ActiveControl = cboObject;

            if (IsWizardInEditMode)
            {
                if (PageNumber == PageIndex.ParentObjectSelectionPage)
                {
                    List<ApttusObject> apttusObjects = (cboObject.DataSource as List<ApttusObject>);
                    cboObject.SelectedItem = apttusObjects.Find(obj => obj.Id.Equals(Model.Object.Id));
                    cboObject.Enabled = false;
                }
            }
        }

        private void LoadParentObjects()
        {
            //AllApttusObjects = objectManager.GetAllStandardObjects().OrderBy(field => field.Name).ToList();
            cboObject.DataSource = AllApttusObjects;
            cboObject.DisplayMember = "Name";
            cboObject.ValueMember = "UniqueId";
        }

        private void LoadChildObjects(WizardModel model)
        {
            lblObject.Text = resourceManager.GetResource("COMMON_ChildObject_Text");

            bool bLoadChildObject = false;
            if (Model.Object.Children.Count == 0)
                bLoadChildObject = true;
            else if (Model.Object != ChildParentObject)
                bLoadChildObject = true;

            if (bLoadChildObject)
            {
                ChildParentObject = Model.Object;

                this.cboObject.SelectedIndexChanged -= new System.EventHandler(this.cboObject_SelectedIndexChanged);

                ApttusObject parentObject = objectManager.GetApttusObject(model.Object.Id, false);

                // Get All Child Object's name

                foreach (ApttusObject appObject in parentObject.Children)
                {
                    if (AllApttusObjects.Any(f => f.Id == appObject.Id))
                    {
                        ApttusObject standardAppObject = AllApttusObjects.FirstOrDefault(f => f.Id == appObject.Id);
                        appObject.Name = standardAppObject.Name;
                        appObject.IdAttribute = standardAppObject.IdAttribute;
                        appObject.NameAttribute = standardAppObject.NameAttribute;
                    }
                }

                List<ApttusObject> childObjects = parentObject.Children.OrderBy(field => field.Name).ToList();
                cboObject.DataSource = childObjects;
                cboObject.DisplayMember = "NamePlusLookupName";

                apttusFieldsView1.UpdateDataSource(null);
                cboObject.SelectedIndex = -1;

                this.cboObject.SelectedIndexChanged += new System.EventHandler(this.cboObject_SelectedIndexChanged);
            }

            if (IsWizardInEditMode)
            {
                cboObject.SelectedItem = (cboObject.DataSource as List<ApttusObject>).Find(obj => obj.Id.Equals(Model.Object.Children[0].Id));
                cboObject.Enabled = false;
            }
        }

        private void ObjectSelectionPage_ApttusWizardPageActivate()
        {
            if (Model.AppType == QuickAppType.ListApp)
                lblObject.Text = resourceManager.GetResource("COMMON_Object_Text"); // "Object :";
            else if (Model.AppType == QuickAppType.ParentChildApp)
            {
                if (PageNumber == PageIndex.ParentObjectSelectionPage)
                    lblObject.Text = resourceManager.GetResource("QAOBJSEL_lblObject_Text"); //  "Parent Object :";
            }

            if (PageNumber == PageIndex.ChildObjectSelectionPage)
                LoadChildObjects(Model);
        }

        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboObject.Enabled = false;
            apttusFieldsView1.UpdateDataSource(null);
            lblLoadingFields.Visible = true;
            ApttusObject objWithoutFields = cboObject.SelectedItem as ApttusObject;
            threadToUpdateFields.RunWorkerAsync(objWithoutFields);
        }

        void threadToUpdateFields_DoWork(object sender, DoWorkEventArgs e)
        {
            ApttusObject objWithoutFields = e.Argument as ApttusObject;
            LoadFields(objWithoutFields);
        }

        private void LoadFields(ApttusObject apttusObj)
        {
            Fields.Clear();
            ApttusObject objWithFields = objectManager.GetApttusObject(apttusObj.Id, false);
            objWithFields.LookupName = apttusObj.LookupName;

            if (PageNumber == PageIndex.ParentObjectSelectionPage && !IsWizardInEditMode)
            {
                ResetModel();

                Model.Object = objWithFields.DeepCopy();

                if (Model.Object.Children != null)
                    Model.Object.Children.Clear();
            }

            if (objWithFields != null)
            {
                foreach (ApttusField field in objWithFields.Fields)
                {
                    ApttusFieldDS fieldDS = applicationDefinitionManager.CloneApttusField(field);
                    if(applicationDefinitionManager.DefaultFields(objWithFields).Contains(field.Id))
                        fieldDS.Included = true;
                    else if (IsWizardInEditMode)
                    {
                        if (PageNumber == PageIndex.ParentObjectSelectionPage && Model.Object.Fields.Exists(fld => fld.Id.Equals(fieldDS.Id)))
                            fieldDS.Included = true;
                        else if (PageNumber == PageIndex.ChildObjectSelectionPage && Model.Object.Children[0].Fields.Exists(fld => fld.Id.Equals(fieldDS.Id)))
                            fieldDS.Included = true;
                    }
                    Fields.Add(fieldDS);
                }

                List<ApttusFieldDS> lsfields = Fields.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
                apttusFieldsView1.AppObject = objWithFields;
                apttusFieldsView1.Invoke(new System.Action(() => 
                { 
                    apttusFieldsView1.RenderFieldsGrid(ref lsfields, applicationDefinitionManager.DefaultFields(objWithFields));
                }),null);
            }
        }

        private void UpdateParentObject()
        {
            if (!IsWizardInEditMode)
            {
                Model.Object.UniqueId = Model.Object.UniqueId == Guid.Empty ? Guid.NewGuid() : Model.Object.UniqueId;

                if (Model.AppType == QuickAppType.ListApp)
                    Model.Object.ObjectType = ObjectType.Repeating;
                else if (Model.AppType == QuickAppType.ParentChildApp)
                    Model.Object.ObjectType = ObjectType.Independent;

                // If Parent Object has changed clear child collection else not
                ApttusObject currentObject = cboObject.SelectedItem as ApttusObject;
                if (currentObject.Id != Model.Object.Id)
                    Model.Object.Children.Clear();
            }

            UpdateFields(Model.Object);

            UpdateApplicationDefination();
        }

        private void UpdateApplicationDefination()
        {
            switch (Model.AppType)
            {
                case QuickAppType.ListApp:
                    AddParentObjectToAppDef();
                    break;
                case QuickAppType.ParentChildApp:
                    if (PageNumber == PageIndex.ChildObjectSelectionPage)
                        AddChildObjectToAppDef();
                    break;
            }
        }

        private void AddParentObjectToAppDef()
        {
            applicationDefinitionManager.AppObjects.Clear();

            ApttusObject parentObj = applicationDefinitionManager.AddObject(Model.Object);

            applicationDefinitionManager.UpdateFields(parentObj, Model.Object.Fields);
        }

        /// <summary>
        /// This should only be invoked in Master Object Context, otherwise everything will get cleared.
        /// </summary>
        private void ResetModel()
        {
            Model.Object = null;
            Model.Filters.Clear();
            Model.DisplayFields.Clear();
            Model.Actions.Clear();
        }

        private void UpdateFields(ApttusObject apttusObject)
        {
            List<ApttusField> selection = (from f in Fields
                                           where f.Included == true
                                           select f).Cast<ApttusField>().ToList<ApttusField>();
            if (apttusObject.Fields == null)
                apttusObject.Fields = new List<ApttusField>();
            else
                apttusObject.Fields.Clear();

            // Get All Lookup Fields from User Selection & get Primary Name field for each Lookup and set under ApttusField
            //List<ApttusField> lookupFields = selection.Where(r => r.Datatype == Datatype.Lookup).ToList();
            //foreach (ApttusField item in lookupFields)
            //{
            //    ApttusObject fieldLookupObject = AllApttusObjects.Where(r => r.Id == item.LookupObject.Id).FirstOrDefault();
            //    // Assign Id and Name attribute of the object
            //    item.LookupObject.IdAttribute = string.IsNullOrEmpty(fieldLookupObject.IdAttribute) ? Constants.ID_ATTRIBUTE : fieldLookupObject.IdAttribute;
            //    item.LookupObject.NameAttribute = string.IsNullOrEmpty(fieldLookupObject.NameAttribute) ? Constants.NAME_ATTRIBUTE : fieldLookupObject.NameAttribute;

            //    // Multiple Entity Reference Field
            //    if (item.MultiLookupObjects != null && item.MultiLookupObjects.Count > 0)
            //    {
            //        foreach (ApttusObject referenceObject in item.MultiLookupObjects)
            //        {
            //            ApttusObject parentRefObject = AllApttusObjects.Where(r => r.Id == referenceObject.Id).FirstOrDefault();
            //            referenceObject.IdAttribute = string.IsNullOrEmpty(parentRefObject.IdAttribute) ? Constants.ID_ATTRIBUTE : parentRefObject.IdAttribute;
            //            referenceObject.NameAttribute = string.IsNullOrEmpty(parentRefObject.NameAttribute) ? Constants.NAME_ATTRIBUTE : parentRefObject.NameAttribute;
            //            referenceObject.Name = parentRefObject.Name; // To Display on Search and Select Drop down list
            //        }
            //    }
            //}

            apttusObject.Fields.AddRange(selection);
        }

        private void UpdateChildObject()
        {
            if (!IsWizardInEditMode)
            {
                ApttusObject childObject = (cboObject.SelectedItem as ApttusObject);

                if (childObject.UniqueId == Guid.Empty)
                    childObject.UniqueId = Guid.NewGuid();

                childObject.ObjectType = ObjectType.Repeating;

                UpdateFields(childObject);

                Model.Object.Children.Clear();

                Model.Object.Children.Add(childObject);

                UpdateApplicationDefination();
            }
            else
            {
                ApttusObject childObject = Model.Object.Children[0];
                UpdateFields(childObject);

                ApttusObject parentObject = Model.Object;

                applicationDefinitionManager.UpdateFields(applicationDefinitionManager.AppObjects[0], Model.Object.Fields);
                applicationDefinitionManager.UpdateFields(applicationDefinitionManager.AppObjects[0].Children[0], childObject.Fields);
            }
        }

        private void AddChildObjectToAppDef()
        {
            applicationDefinitionManager.AppObjects.Clear();

            ApttusObject obj = applicationDefinitionManager.AddObject(Model.Object);
            applicationDefinitionManager.UpdateFields(obj, Model.Object.Fields);
            applicationDefinitionManager.UpdateFields(obj.Children[0], Model.Object.Children[0].Fields);
        }


        public override void ProcessPage()
        {
            if (PageNumber == PageIndex.ParentObjectSelectionPage)
            {
                UpdateParentObject();

                //Check if some new fields were added.
                List<QuickAppFieldAttribute> parentFields = Model.DisplayFields.Where(df => df.FieldRelation == RelationalObject.ParentObject).ToList();
                foreach (ApttusField field in Model.Object.Fields)
                {
                    //Check whether the selected fields in the grid already exists in Model. If not grid is dirty.
                    Model.IsParentDirty = !parentFields.Exists(fld => fld.FieldId.Equals(field.Id));
                    if (Model.IsParentDirty)
                        break;
                }

                //Check if some fields were removed.
                foreach (QuickAppFieldAttribute field in parentFields)
                {
                    //Check whether the selected fields in the grid already exists in Model. If not grid is dirty.
                    Model.IsParentDirty = Model.IsParentDirty || !Model.Object.Fields.Exists(fld => fld.Id.Equals(field.FieldId));
                    if (Model.IsParentDirty)
                        break;
                }
            }
            else if (PageNumber == PageIndex.ChildObjectSelectionPage)
            {
                UpdateChildObject();

                //Check if some new fields were added.
                List<QuickAppFieldAttribute> childFields = Model.DisplayFields.Where(df => df.FieldRelation == RelationalObject.ChildObject).ToList();
                foreach (ApttusField field in Model.Object.Children[0].Fields)
                {
                    //Check whether the selected fields in the grid already exists in Model. If not grid is dirty.
                    Model.IsChildDirty = !childFields.Exists(fld => fld.FieldId.Equals(field.Id));
                    if (Model.IsChildDirty)
                        break;
                }

                //check if some fields were removed.
                foreach (QuickAppFieldAttribute field in childFields)
                {
                    //Check whether the selected fields in the grid already exists in Model. If not grid is dirty.
                    Model.IsChildDirty = Model.IsChildDirty || !Model.Object.Children[0].Fields.Exists(fld => fld.Id.Equals(field.FieldId));
                    if (Model.IsChildDirty)
                        break;
                }
            }
        }

        private QuickAppErrorCode ValidateThisPage()
        {
            if (cboObject.SelectedIndex == -1)
                return QuickAppErrorCode.NoObjectSelected;
            return QuickAppErrorCode.Success;
        }

        public override bool ValidatePage()
        {
            if (threadToUpdateFields.IsBusy)
                return false;

            QuickAppErrorCode errorCode = ValidateThisPage();
            switch (errorCode)
            {
                case QuickAppErrorCode.NoObjectSelected:
                    string text = PageNumber == PageIndex.ParentObjectSelectionPage ? "Parent" : "Child";
                    string errorMsg = String.Format(resourceManager.GetResource("OBJSELPAGE_ValidatePageSel_WarnMsg"), text);
                    ApttusMessageUtil.ShowError(errorMsg, resourceManager.GetResource("OBJSELPAGE_ValidatePageSel_Text"));
                    ActiveControl = cboObject;
                    break;
            }
            return errorCode == QuickAppErrorCode.Success;
        }

        private void threadToUpdateFields_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblLoadingFields.Visible = false;
            if (!IsWizardInEditMode)
                cboObject.Enabled = true;
        }
    }
}
