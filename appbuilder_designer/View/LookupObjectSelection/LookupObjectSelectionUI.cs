/*
 * Apttus X-Author for Excel
 * © 2016 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;
using System.Drawing;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class LookupObjectSelectionUI : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public ApttusFieldDS appApttusFieldDS;
        public ApttusField standardApttusField;
        public string appObjectId;
        List<AppObjectFields> selectedFields = new List<AppObjectFields>();
        List<DataSetObjectFields> availableFields = new List<DataSetObjectFields>();

        public LookupObjectSelectionUI()
        {
            InitializeComponent();
            //SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;

        }

        private void SetCultureData()
        {
            btnSave.Text = resourceManager.GetResource("LookupObjectSelectionUI_btnSave_Text");
            btnCancel.Text = resourceManager.GetResource("LookupObjectSelectionUI_btnCancel_Text");
            lblSelectObjects.Text = resourceManager.GetResource("LookupObjectSelectionUI_lblSelectObjects_Text");
            lblSelectObjectDesc.Text = resourceManager.GetResource("LookupObjectSelectionUI_lblSelectObjectDesc_Text");
            //
        }

        private static LookupObjectSelectionUI inst;
        public static LookupObjectSelectionUI GetLookupObjectSelectionUI
        {
            get
            {
                if (inst == null || inst.IsDisposed)
                {
                    inst = new LookupObjectSelectionUI();
                }
                return inst;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void BindDataSource()
        {

            // Sort Available List Objects
            if (availableFields.Count > 0)
                availableFields.Sort(delegate(DataSetObjectFields AO1, DataSetObjectFields AO2)
                {
                    return AO1.FieldName.CompareTo(AO2.FieldName);
                });


            lstAvailableObjects.DataSource = null;
            lstAvailableObjects.DataSource = availableFields;
            lstAvailableObjects.DisplayMember = "FieldName";
            lstAvailableObjects.ValueMember = "FieldId";

            // Sort Selected List Objects
            if (selectedFields.Count > 0)
                selectedFields.Sort(delegate(AppObjectFields AO1, AppObjectFields AO2)
                {
                    return AO1.FieldName.CompareTo(AO2.FieldName);
                });

            lstSelectedObjects.DataSource = null;
            lstSelectedObjects.DataSource = selectedFields;
            lstSelectedObjects.DisplayMember = "FieldName";
            lstSelectedObjects.ValueMember = "FieldId";

        }
        private void LookupObjectSelectionUI_Load(object sender, EventArgs e)
        {
            if (appApttusFieldDS != null)
            {

                // Set Selected Object List First
                if (standardApttusField != null)
                {
                    foreach (ApttusObject item in standardApttusField.MultiLookupObjects)
                        selectedFields.Add(new AppObjectFields { FieldId = item.Id, FieldName = item.Name });
                }

                // Set All Available Object List except Selected Objects

                foreach (ApttusObject item in appApttusFieldDS.MultiLookupObjects)
                {
                    if (!selectedFields.Any(f => f.FieldId == item.Id))
                        availableFields.Add(new DataSetObjectFields { FieldId = item.Id, FieldName = item.Name });
                }

                BindDataSource();
            }
        }


        private void AddObjects()
        {
            var selectedItems = lstAvailableObjects.SelectedItems;
            foreach (var item in selectedItems)
            {
                DataSetObjectFields dataSetObject = (DataSetObjectFields)item;
                selectedFields.Add(new AppObjectFields { FieldId = dataSetObject.FieldId, FieldName = dataSetObject.FieldName });
            }
            foreach (var item in selectedItems)
                availableFields.Remove(item as DataSetObjectFields);

            BindDataSource();
        }

        private void RemoveObjects()
        {         
            var selectedItems = lstSelectedObjects.SelectedItems;

            foreach (var item in selectedItems)
            {
                AppObjectFields appObjectItem = item as AppObjectFields;
                availableFields.Add(new DataSetObjectFields { FieldId = appObjectItem.FieldId, FieldName = appObjectItem.FieldName });
            }
            foreach (var item in selectedItems)
                selectedFields.Remove(item as AppObjectFields);

            BindDataSource();
        }
        private void btnAddObjects_Click(object sender, EventArgs e)
        {
            AddObjects();

        }

        private void btnRemoveObjects_Click(object sender, EventArgs e)
        {
            RemoveObjects();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            //AB-2660 : MSD: Validation to select atleast one entity in case of multilookup. 
            if (selectedFields.Count <= 0)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("LOOKUPOBJECTSELECTION_LookupObjectSelection_ErrorMsg"), resourceManager.GetResource("LOOKUPOBJECTSELECTION_LookupObjectSelection__CkickCAP_ErrorMsg"));
                return;
            }

            List<ApttusObject> intermediateLookupList = new List<ApttusObject>();
            foreach (AppObjectFields item in selectedFields)
            {
                ApttusObject lookupObject = appApttusFieldDS.MultiLookupObjects.FirstOrDefault(f => f.Id == item.FieldId);
                if (!intermediateLookupList.Any(f => f.Id == lookupObject.Id))
                    intermediateLookupList.Add(lookupObject);
            }

            var tempObject = ApplicationDefinitionManager.GetInstance.GetAppObjectById(appObjectId);
            ApttusObject appObject = null;
            if (tempObject != null && tempObject.Count > 0)
                appObject = ApplicationDefinitionManager.GetInstance.GetAppObject(tempObject[0].UniqueId);

            if (standardApttusField != null)
            {
                standardApttusField.MultiLookupObjects = intermediateLookupList;
                ApttusField lookupField;
                if (appObject != null)
                {
                    if (appObject.Fields.Any(f => f.Id == standardApttusField.Id))
                    {
                        lookupField = appObject.Fields.FirstOrDefault(f => f.Id == standardApttusField.Id);
                        lookupField.MultiLookupObjects = standardApttusField.MultiLookupObjects;

                        // Reset Lookup.Name Field MultiLookupObjects Attribute as well
                        if (appObject.Fields.Any(f => f.Id == standardApttusField.Id + Constants.APPENDLOOKUPID && f.LookupObject != null))
                        {
                            /*  AB - 2657 - MSD: User input with IN filter in query action throws error. 
                             * &&&
                             * AB - 2658 - MSD: Using 'Not equal to Blank' filter on multilookup field  throws error. 
                            */
                            ApttusField lookupNameField = appObject.Fields.FirstOrDefault(f => f.Id == standardApttusField.Id + Constants.APPENDLOOKUPID);
                            lookupNameField.MultiLookupObjects = standardApttusField.MultiLookupObjects;
                        }
                    }
                }
            }
            else
            {
                ApttusField newAppField = new ApttusField()
                {
                    Id = appApttusFieldDS.Id,
                    Name = appApttusFieldDS.Name,
                    Datatype = appApttusFieldDS.Datatype,
                    LookupObject = appApttusFieldDS.LookupObject,
                    MultiLookupObjects = intermediateLookupList
                };

                if (appObject != null && (!appObject.Fields.Any(f => f.Id == appApttusFieldDS.Id)))
                    appObject.Fields.Add(newAppField);
                else
                    appApttusFieldDS.MultiLookupObjects = intermediateLookupList;
            }

            ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKUPOBJECTSELECTION_Success_ShowMsg"), resourceManager.GetResource("LOOKUPOBJECTSELECTION_LookupObjectSelection__CkickCAP_ErrorMsg"));
            this.Close();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstAvailableObjects_DoubleClick(object sender, EventArgs e)
        {
            AddObjects();
        }

        private void lstSelectedObjects_DoubleClick(object sender, EventArgs e)
        {
            RemoveObjects();
        }
    }

    public class AppObjectFields
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
    }

    public class DataSetObjectFields
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
    }
}
