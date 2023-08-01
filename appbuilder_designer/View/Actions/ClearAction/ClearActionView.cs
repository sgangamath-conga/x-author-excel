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
    public partial class ClearActionView : Form
    {
        class GridRow
        {
            public string MapName { get; set; }
            public string AppObject { get; set; }
            public ObjectType ObjectType { get; set; }
            public int Index { get; set; }
        }

        class ClearActionMapSource
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public Core.ClearActionMapType MapType { get; set; }
        }

        private IClearActionController controller;
        private ClearActionModel Model;
        private BindingList<GridRow> gridBindingSource; 
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ClearActionView()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            gridBindingSource = new BindingList<GridRow>();
            gridBindingSource.AllowRemove = true;
            btnCancel.CausesValidation = false;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnOK.Text = resourceManager.GetResource("COMMON_Save_AMPText");
            Index.HeaderText = resourceManager.GetResource("CLEARACTION_Index_HeaderText");
            label1.Text = resourceManager.GetResource("CLEARACTION_label1_Text");
            label2.Text = resourceManager.GetResource("COMMON_ActionName_Text");
            label3.Text = resourceManager.GetResource("CLEARACTION_label3_Text");
            //label5.Text = resourceManager.GetResource("CLEARACTION_label5_Text");
            label6.Text = resourceManager.GetResource("COMMON_SelectedObjects_Text");
            ObjectType.HeaderText = resourceManager.GetResource("CLEARACTION_ObjectType_HeaderText");
            SaveMapName.HeaderText = resourceManager.GetResource("COMMON_MapName_HeaderText");
            AppObject.HeaderText = resourceManager.GetResource("CLEARACTION_AppObject_HeaderText");
            chkDisableExcelEvents.Text = resourceManager.GetResource("Designer_chkDisableExcelEvents_Text");
        }

        internal void SetController(IClearActionController clearActionController)
        {
            controller = clearActionController;
        }

        internal void FillObjects()
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            
            // Add SaveMaps
            List<ClearActionMapSource> maps = (from saveMap in configManager.SaveMaps
                            select new ClearActionMapSource {Name = saveMap.Name + " - (Save)", Id = saveMap.Id, MapType = Core.ClearActionMapType.SaveMap }).Distinct().ToList();

            //Add RetrieveMaps
             maps.AddRange ((from retrieveMap in configManager.RetrieveMaps
                             select new ClearActionMapSource { Name = retrieveMap.Name + " - (Display)", Id = retrieveMap.Id, MapType = Core.ClearActionMapType.DisplayMap }).Distinct().ToList());

             cboMap.DisplayMember = "Name";
             cboMap.ValueMember = "Id";
             cboMap.DataSource = maps;
           

            dgvSelectedObjects.DataSource = gridBindingSource;
        }

        internal void UpdateControls(ClearActionModel model)
        {
            Model = model;
            txtName.Text = model.Name;
            chkDisableExcelEvents.Checked = model.DisableExcelEvents;
            cboMap.SelectedValue = model.SaveMapId;
             
            SaveMap saveMap = ConfigurationManager.GetInstance.SaveMaps.Where(sm => sm.Id.Equals(model.SaveMapId)).FirstOrDefault();
            if (saveMap != null)
                LoadCheckBoxItems(saveMap);
            else
            {
                RetrieveMap rMap = ConfigurationManager.GetInstance.RetrieveMaps.Where(rm => rm.Id.Equals(model.SaveMapId)).FirstOrDefault();
                LoadCheckBoxItems(rMap);
            }
            foreach (Guid AppObject in model.AppObjects)
            {
                ApttusObject Obj = chkAppObjects.Items.Cast<ApttusObject>().Where(obj => obj.UniqueId.Equals(AppObject)).FirstOrDefault();
                int indexOfObj = chkAppObjects.Items.IndexOf(Obj);
                chkAppObjects.SetItemChecked(indexOfObj, true);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ApttusFormValidator.hasValidationErrors(this.Controls))
                return;

            if (Model == null)
                Model = new ClearActionModel();

            Model.AppObjects.Clear();

            ClearActionMapSource map = cboMap.SelectedItem as ClearActionMapSource;
            Model.Name = txtName.Text;
            Model.DisableExcelEvents = chkDisableExcelEvents.Checked;
            Model.SaveMapId = map.Id;
            Model.MapType = map.MapType;

            foreach (ApttusObject obj in chkAppObjects.CheckedItems)
            {
                Model.AppObjects.Add(obj.UniqueId);
            }
            controller.Save(Model);
        }

        private void cboSaveMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearActionMapSource Map = cboMap.SelectedItem as ClearActionMapSource;
            Guid MapId = Map.Id;
            switch(Map.MapType)
            {
                case Core.ClearActionMapType.SaveMap:
                SaveMap saveMap = ConfigurationManager.GetInstance.SaveMaps.Where(sm => sm.Id.Equals(MapId)).FirstOrDefault();
                if (saveMap == null)
                    return;
                LoadCheckBoxItems(saveMap);
                break;

                case Core.ClearActionMapType.DisplayMap:
                RetrieveMap rMap = ConfigurationManager.GetInstance.RetrieveMaps.Where(rm => rm.Id.Equals(MapId)).FirstOrDefault();
                if (rMap == null)
                    return;
                LoadCheckBoxItems(rMap);
                break;
            }
        }

        private void LoadCheckBoxItems(RetrieveMap rMap)
        {
            try
            {
                this.SuspendLayout();
                ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

                List<Guid> AppObjectsId = rMap.RetrieveFields.GroupBy(rm => rm.AppObject).Select(rm => rm.Key).Distinct().ToList();
                AppObjectsId.AddRange(rMap.RepeatingGroups.GroupBy(rm => rm.AppObject).Select(rm => rm.Key).Distinct().ToList());

                List<ApttusObject> appObjects = new List<ApttusObject>();
                foreach (Guid AppObjectId in AppObjectsId)
                {
                    ApttusObject obj = appDefManager.GetAppObject(AppObjectId);
                    appObjects.Add(obj);
                }

                chkAppObjects.DataBindings.Clear();
                chkAppObjects.DataSource = null;

                chkAppObjects.DataSource = appObjects;
                chkAppObjects.DisplayMember = "Name";
                chkAppObjects.ValueMember = "UniqueId";

                gridBindingSource.Clear();
                AppObjectsId.Clear();
                appObjects.Clear();
            }
            finally
            {                
                this.ResumeLayout();
            }
        }

        private void ClearActionView_Shown(object sender, EventArgs e)
        {
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            btnCancel.CausesValidation = false;
            controller.SetView();
        }

        private void chkAppObjects_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ApttusObject obj = chkAppObjects.Items[e.Index] as ApttusObject;
            if (e.NewValue == CheckState.Checked)
            {
                ClearActionMapSource map = (cboMap.SelectedItem as ClearActionMapSource);
                GridRow row = new GridRow
                {
                    MapName = map.Name,
                    AppObject = obj.Name,
                    ObjectType = obj.ObjectType,
                    Index = e.Index,
                };
                gridBindingSource.Add(row);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                GridRow row = gridBindingSource.Where(saveMapRow => saveMapRow.Index == e.Index).FirstOrDefault();
                if (row != null)
                    gridBindingSource.Remove(row);
            }
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(txtName, e, "Action Name is Required");
        }

        internal void SetError(Control control, string errorMsg)
        {
            errorProvider1.SetError(control, errorMsg);
        }

        private void LoadCheckBoxItems(SaveMap saveMap, bool bUpdate = true)
        {
            try
            {
                this.SuspendLayout();
                ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

                List<Guid> saveMapAppObjects = saveMap.SaveFields.GroupBy(sm => sm.AppObject).Select(sm => sm.Key).Distinct().ToList();
                List<ApttusObject> appObjects = new List<ApttusObject>();
                foreach (Guid AppObjectId in saveMapAppObjects)
                {
                    ApttusObject obj = appDefManager.GetAppObject(AppObjectId);
                    appObjects.Add(obj);
                }

                chkAppObjects.DataBindings.Clear();
                chkAppObjects.DataSource = null;

                chkAppObjects.DataSource = appObjects;
                chkAppObjects.DisplayMember = "Name";
                chkAppObjects.ValueMember = "UniqueId";

                gridBindingSource.Clear();
                saveMapAppObjects.Clear();
                appObjects.Clear();
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private void cboSaveMap_Validating(object sender, CancelEventArgs e)
        {
            controller.ValidateAction(cboMap, e, resourceManager.GetResource("PASTEROWACTVIEW_cboSaveMap_Validating_InfoMsg"));
        }

        void AdjustWidth_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in senderComboBox.Items)
            {
                if (currentItem is ClearActionMapSource)
                    newWidth = (int)g.MeasureString(((ClearActionMapSource)currentItem).Name, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }
            senderComboBox.DropDownWidth = width;
        }
    }
}
