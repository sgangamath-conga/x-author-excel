/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Apttus.XAuthor.Core;
using System.ComponentModel;

namespace Apttus.XAuthor.AppDesigner
{
    public abstract class LookAheadPropController
    {
        protected LookAheadPropUI View;
        protected ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public LookAheadPropController(LookAheadPropUI view)
        {
            View = view;
            View.Controller = this;
        }

        public virtual void PopulateUI() { ;}
        public virtual void PopulateActions(bool allActions) {}
        public abstract LookAheadProperty Apply();
        public abstract void Detach();
        public LookAheadProperty LookAheadProp
        {
            get;
            set;
        }

        protected void DisplayMessage(string message)
        {
            View.Hide();
            ApttusMessageUtil.ShowInfo(message, resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
            View.Show();
        }

        public List<RetrieveField> FillRetrieveFields()
        {
            if (View.FldMapper != null)
            {
                RetrieveFieldMapper fldMapper = View.FldMapper as RetrieveFieldMapper;
                List<RetrieveField> fields = fldMapper.GetAllFieldsFromMap().ToList();
                RetrieveField field = new RetrieveField { FieldId = " ", TargetColumnIndex = -1, TargetNamedRange = string.Empty };
                fields.Insert(0,field);
                return fields;
            }
            return null;
        }

        // Lookahead multi column should have
        // target fields displayed so that users could assign
        // return col to a target field
        public List<SaveFieldBound> FillSaveFields()
        {
            if (View.FldMapper != null)
            {
                SaveFieldMapper fldMapper = View.FldMapper as SaveFieldMapper;
                List<SaveField> resultFields = new List<SaveField>();
                resultFields.AddRange(fldMapper.GetAllFieldsFromMap());

                // Convert SaveField to SaveFieldBound, so that we can get Field Name property, showing FieldId would not make much sense for Designers
                SaveMapController saveMapcontroller = new SaveMapController(fldMapper.MappedSaveMap, null, FormOpenMode.Edit);
                List<SaveFieldBound> boundSaveFields = saveMapcontroller.GenerateBindingList(resultFields);

                // Insert a blank field as 1st field
                SaveFieldBound blankField = new SaveFieldBound { FieldId = " ", FieldName = " ", TargetColumnIndex = -1, TargetNamedRange = string.Empty } ;
                boundSaveFields.Insert(0, blankField);

                return boundSaveFields;
            }
            return null;
        }
    }

    /* Controller class for Search and Select action */
    public class LookAheadSearchSelectController : LookAheadPropController
    {
        CheckBox IsActive;
        CheckBox chkFieldMapping;
        // Combo box to show SS actions
        public ListBox SSUI
        {
            get;
            set;
        }

        List<SearchAndSelect> SSActions = new List<SearchAndSelect>();
        ComboBox cmboReturnIDfieldData;
        ListBox lstActions;
        FieldMappingView ucMappingView;

        public LookAheadSearchSelectController(LookAheadPropUI view,
                CheckBox chkActive, CheckBox chkXLActive, CheckBox chkrefresh, ComboBox cmboRetIDfld, CheckBox cFieldMapping, ListBox ActionsList, FieldMappingView mappingview)
            : base(view)
        {
            IsActive = chkActive;
            this.SSUI = view.ActionUI;
            // for SS DS, make the active and refresh visible = false
            // b/c these chks are added outside the tab
            chkXLActive.Visible = false;
            chkrefresh.Visible = false;
            cmboReturnIDfieldData = cmboRetIDfld;
            chkFieldMapping = cFieldMapping;
            lstActions = ActionsList;
            ucMappingView = mappingview;
            PopulateUI();
        }

        private Guid GetAppObjectIdFromFieldMapper(FieldMapper fldMapper)
        {
            Guid appObjectId = Guid.Empty;
            if (fldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
            {
                RepeatingGroup currentRepGroup = (from repGroup in ((RetrieveFieldMapper)fldMapper).MappedRetrieveMap.RepeatingGroups
                                              from retField in repGroup.RetrieveFields
                                              where retField.TargetNamedRange.Equals(((RetrieveFieldMapper)fldMapper).MappedRetrieveField.TargetNamedRange)
                                              select repGroup).FirstOrDefault();
                if (currentRepGroup != null)
                    appObjectId = currentRepGroup.AppObject;
            }
            return appObjectId;
        }

        public override void PopulateActions(bool allActions)
        {
            if (allActions)
            {
                Guid appObjectId = GetAppObjectIdFromFieldMapper(View.FldMapper);

                List<Apttus.XAuthor.Core.SearchAndSelect> SearchSelActions = ConfigurationManager.GetInstance.Actions.OfType<SearchAndSelect>().ToList();
                SSActions = (from ssmdl in SearchSelActions
                             where !ssmdl.TargetObject.Equals(appObjectId)
                            select ssmdl).ToList();

                SSUI.DataSource = (SSActions);
                SSUI.DisplayMember = "Name";
                SSUI.ValueMember = "Id";
            }
            else
            { //Normal case
                LookAheadFieldMapperHelper mapperHelper = new LookAheadFieldMapperHelper(View.FldMapper);
                if (mapperHelper.AppObjectUniqueId == Guid.Empty)
                    return;

                List<SearchAndSelect> SearchSelActions = ConfigurationManager.GetInstance.Actions.OfType<SearchAndSelect>().ToList();
                SSActions = (from ssmdl in SearchSelActions                         
                             where ssmdl.TargetObject.Equals(mapperHelper.AppObjectUniqueId) && ssmdl.RecordType.Equals(Constants.QUERYRESULT_SINGLE)
                             select ssmdl).ToList();

                SSUI.DataSource = (SSActions);
                SSUI.DisplayMember = "Name";
                SSUI.ValueMember = "Id";
            }
        }

        /* find the search and select actions based on the current field's object 
         * and show the actions in the drop down */
        public override void PopulateUI()
        {
            // first check if the field mapper is set
            if (View.FldMapper != null)
            {
                LookAheadFieldMapperHelper mapperHelper = new LookAheadFieldMapperHelper(View.FldMapper);
                if (mapperHelper.AppObjectUniqueId != Guid.Empty)
                {
                    try
                    {
                        Guid AppObjectId = mapperHelper.AppObjectUniqueId;

                        ApttusObject obj = ApplicationDefinitionManager.GetInstance.GetAppObject(AppObjectId);

                        List<SearchAndSelect> SearchSelActions = ConfigurationManager.GetInstance.Actions.OfType<SearchAndSelect>().ToList();
                        IEnumerable<SearchAndSelect> SearchSelActions2 = null;

                        // just for vaidation message object is used. 

                        if (obj == null)
                        {
                            ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("LOOKAHEADPROPCTL_PopulateUI_InfoMsg"),resourceManager.CRMName), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                            return;
                        }

                        //Start Remove 
                        if (obj.Id.Equals("Task") || obj.Id.Equals("Event"))
                        {
                            RetrieveFieldMapper rFieldMapper = View.FldMapper as RetrieveFieldMapper;
                            if (rFieldMapper.MappedRetrieveField.FieldId.Equals("WhatId")
                                || rFieldMapper.MappedRetrieveField.FieldId.Equals("WhoId")
                                || rFieldMapper.MappedRetrieveField.FieldId.Equals("What.Name")
                                || rFieldMapper.MappedRetrieveField.FieldId.Equals("Who.Name"))

                                SearchSelActions2 = SearchSelActions;
                        }
                        else //End Remove
                        {
                            
                            // find search and select action from all actions
                            SearchSelActions2 = from ssmdl in SearchSelActions
                                                where ssmdl.TargetObject.Equals(AppObjectId) && ssmdl.RecordType.Equals(Constants.QUERYRESULT_SINGLE)
                                                select ssmdl;

                            //Field Mapping will only be enabled for list objects.
                        }

                        chkFieldMapping.Enabled = obj.ObjectType == ObjectType.Repeating;

                        //Start Remove
                        if (SearchSelActions2 == null)
                            return;
                        //end Remove

                        foreach (var ssMDL in SearchSelActions2)
                        {
                            SSActions.Add((SearchAndSelect)ssMDL);
                        }

                        if (SSActions.Count > 0)
                        {
                            SSUI.DataSource = (SSActions);
                            SSUI.DisplayMember = "Name";
                            SSUI.ValueMember = "Id";
                            SSUI.SelectedIndex = -1;
                        }
                        //else
                        //{
                        //    DisplayMessage("Please create Search and Selection action for " + obj.Name);
                        //}

                        if (View.FldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
                        {                           
                            List<RetrieveField> RetFld = FillRetrieveFields();                            
                            cmboReturnIDfieldData.DataSource = RetFld;
                            cmboReturnIDfieldData.DisplayMember = "FieldName";
                            cmboReturnIDfieldData.ValueMember = "TargetColumnIndex";

                            cmboReturnIDfieldData.SelectedIndex = -1;
                        }
                        else // save only
                        {                        
                            List<SaveFieldBound> RetFld = FillSaveFields();
                            cmboReturnIDfieldData.DataSource = RetFld;
                            cmboReturnIDfieldData.DisplayMember = "FieldName";
                            cmboReturnIDfieldData.ValueMember = "TargetColumnIndex";
                        }

                        if (View.FldMapper.PropertyCollection != null)
                        {
                            // since the field can have Excel DS & SS DS, show the corresponding 
                            // DS in the UI. for example, here show SS DS. There can be only one DS for each type
                            // so show only the SS action

                            // Set the LH property based on the tab selected. 
                            LookAheadProperty prop = View.FldMapper.PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.SearchAndSelect));
                            SSActionDataSource SSActionProp = prop as SSActionDataSource;
                            string FldID = null;
                            if (SSActionProp != null && SSActionProp.ReturnColumnData != null)
                            {
                                ReturnDataCollection RetData = SSActionProp.ReturnColumnData;
                                FldID = RetData.DataCollection[0].FieldID;
                            }
                            if (SSActionProp != null)
                            {
                                View.FldMapper.Property = View.FldMapper.PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.SearchAndSelect));
                                // set the property so that remove can read the same. 
                                LookAheadProp = View.FldMapper.Property;
                                
                                string name = ((SSActionDataSource)View.FldMapper.Property).ActionName;
                                IsActive.Checked = ((SSActionDataSource)View.FldMapper.Property).IsActive;
                                if (!string.IsNullOrEmpty(name))
                                    SSUI.SelectedIndex = SSUI.FindStringExact(name);
                            }

                            // TODO : this code needs to be in a separate function since this is applicable in all LH properties. 
                            if (View.FldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
                            {
                                // TODO: need to optimize this code. this data is already retrieved, 
                                List<RetrieveField> RetFld = FillRetrieveFields();
                               
                                if (FldID != null)
                                {
                                    cmboReturnIDfieldData.SelectedIndex = RetFld.FindIndex(item => item.FieldId.Equals(FldID));
                                }
                            }
                            else // save only
                            {
                                List<SaveFieldBound> RetFld = FillSaveFields();
                                if (FldID != null)
                                {
                                    cmboReturnIDfieldData.SelectedIndex = RetFld.FindIndex(item => item.FieldId.Equals(FldID));
                                }
                            }

                            if (SSActionProp != null && SSActionProp.ObjectMapping != null)
                            {
                                //don't change the sequence of below 3 lines.
                                chkFieldMapping.Checked = true;
                                lstActions.SelectedValue = SSActionProp.ObjectMapping.SearchAndSelectActionId;
                                ucMappingView.LoadMappingFields(SSActionProp.ObjectMapping);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        DisplayMessage(ex.Message.ToString());
                      //  ApttusMessageUtil.ShowError(ex.Message.ToString(), "Look ahead property - Search and Select Action");
                        return;
                    }
                }
            }
        }

        public override LookAheadProperty Apply()
        {
            //this call should calll Model's save method but 
            // kept it in the controller since there is not
            // enough processing takes place in the model. 

            // Id of the selected action
            //Guid Id;
            if (SSUI.SelectedItem == null) return null;
            // read the selected action id
            string ActionId = ((Apttus.XAuthor.Core.Action)(SSUI.SelectedItem)).Id;
            // store name so that when an existing LH field is selected, the action name can be set in the ui
            string ActionName = ((Apttus.XAuthor.Core.Action)(SSUI.SelectedItem)).Name;
            
            if (string.IsNullOrEmpty(ActionId))
            {
                DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyValidAction_InfoMsg"));
                return null;
            }

            if (!chkFieldMapping.Checked)
            {
                if (cmboReturnIDfieldData.SelectedIndex < 0)
                {
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyValidField_InfoMsg"));
                    return null;
                }
            }

            SSActionDataSource prop = new SSActionDataSource();
            prop.IsActive = IsActive.Checked ? true : false;
            prop.ActionID = ActionId;
            prop.ActionName = ActionName;
            LookAheadProp = prop;
          
            string FieldId;
            string TargetNR;
            int TarColIndex = -1;
            ObjectType fldtype;
            if (cmboReturnIDfieldData.SelectedItem.GetType().Equals(typeof(RetrieveField)))
            {
                RetrieveField RetFld = cmboReturnIDfieldData.SelectedItem as RetrieveField;
                TarColIndex = RetFld.TargetColumnIndex;
                FieldId = RetFld.FieldId;
                TargetNR = RetFld.TargetNamedRange;
                fldtype = string.IsNullOrEmpty(RetFld.TargetNamedRange) ? ObjectType.Independent : RetFld.Type;
            }
            else // save other field
            {
                SaveField RetFld = cmboReturnIDfieldData.SelectedItem as SaveField;

                TarColIndex = RetFld.TargetColumnIndex;
                FieldId = RetFld.FieldId;
                TargetNR = RetFld.TargetNamedRange;
                fldtype = string.IsNullOrEmpty(RetFld.TargetNamedRange) ? ObjectType.Independent : RetFld.Type;
            }

            // second col's values target field index
            ReturnColumnData RetColData = new ReturnColumnData(TarColIndex, null, FieldId, TargetNR, fldtype);
            ReturnDataCollection retColCollection = new ReturnDataCollection();
            retColCollection.DataCollection.Add(RetColData);
            prop.ReturnColumnData = retColCollection;

            if (chkFieldMapping.Checked)
                prop.ObjectMapping = View.MappingView.Save();
            return LookAheadProp;
        }

        public override void Detach()
        {
            if (SSUI.SelectedItem == null)
            {
                DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_DetachNoDataSource_InfoMsg"));
                return;
            }
            string sActionId = ((Apttus.XAuthor.Core.Action)(SSUI.SelectedItem)).Id;
            if (string.IsNullOrEmpty(sActionId))
            {
                return;
            }
            else
            {
                if (View.FldMapper.RemoveProperty(LookAheadProp))
                {
                    if (SSActions != null && SSActions.Count > 0)
                    {
                        SSUI.SelectedIndex = -1;
                        cmboReturnIDfieldData.SelectedIndex = -1;
                    }
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_Detach_InfoMsg"));
                }
            }
        }
    }


    public class LookAheadExcelSourceController : LookAheadPropController
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        protected ComboBox SheetInfo;
        protected TextBox referenceinfo;
        protected TextBox CurrentSelection;

        protected TextBox field;
        protected ComboBox ReturnCol;
        protected ListBox Displaycols;
        protected CheckBox RefreshData;
        protected CheckBox IsActive;
        protected RadioButton radBasicProp;
        protected RadioButton radAdvanceProp;
        protected GroupBox grpMultiColumnGroupBox;
        protected ComboBox cmboTargetField;
        protected ComboBox cmboReturnCol2;

        public LookAheadExcelSourceController(LookAheadPropUI view)
            : base(view)
        {
            PopulateUI();
        }

        public void RegisterControls(TextBox reference, TextBox CurrentSel, TextBox fld,
             ListBox Displayfields, ComboBox lstDis, CheckBox chkRefresh, CheckBox chkActive,
            RadioButton radBasic, RadioButton radAdvanced, GroupBox grpMultiColumn,
        ComboBox TgtField, ComboBox cmboRetCol2)
        {
            CurrentSelection = CurrentSel;
            referenceinfo = reference;
            field = fld;

            Displaycols = Displayfields;
            ReturnCol = lstDis;
            RefreshData = chkRefresh;
            IsActive = chkActive;
            chkRefresh.Visible = true;
            chkActive.Visible = true;
            radBasicProp = radBasic;
            radAdvanceProp = radAdvanced;
            grpMultiColumnGroupBox = grpMultiColumn;
            cmboTargetField = TgtField; // Target field storage
            cmboReturnCol2 = cmboRetCol2;
        }

        public override void PopulateUI()
        {
            CurrentSelection.Text = "";

            if (View.FldMapper != null)
            {
                field.Text = View.FldMapper.GetFieldName();
                // cmboTargetField.DataSource = View.FldMapper.GetFieldName

                if (View.FldMapper.PropertyCollection == null && View.FldMapper.Property == null)
                {  // set the default to Basic
                    radBasicProp.Select();
                    RadioBasicSelect();
                    return;
                }
                LookAheadProperty prop = null;
               
                if (View.FldMapper.PropertyCollection != null && View.FldMapper.PropertyCollection.Count > 0)
                    prop = View.FldMapper.PropertyCollection.Find(item => item.DataSource.Equals(LookAheadDataSource.Excel));
                else
                    prop = View.FldMapper.Property; // backward compatibility

                if (prop == null)
                {
                    radBasicProp.Select();
                    RadioBasicSelect();
                    return;
                }

                if (prop != null)
                {
                    ExcelDataSource fld = prop as ExcelDataSource;
                    if (fld != null)
                    {
                        LookAheadProp = prop;
                        IsActive.Checked = prop.IsActive;

                        if (fld.RefreshData != null)
                            RefreshData.Checked = fld.RefreshData;
                        // display the current selection in the currect selection UI.
                        CurrentSelection.Text = fld.SheetReference;
                        if (!fld.MultiCol)
                        {
                            radBasicProp.Select();
                            RadioBasicSelect();
                        }
                        else // multi col 
                        {
                            radAdvanceProp.Select();
                            RadioAdvanceSelect();

                            // fill the display col with current selection
                            // make the return col combo with current value
                            // first get the sheetreference frm the property
                            // get the range object using the name
                            if (fld.TargetRange != null)
                            {
                                // if the current selection is not the same as the prop range, force the range to select
                                Microsoft.Office.Interop.Excel.Range CurrentRange = ExcelHelper.GetRange(fld.TargetRange);
                                if (CurrentRange != null)
                                {
                                    CurrentRange.Worksheet.Select();
                                    CurrentRange.Select();
                                }
                                List<string> data = GetAllCols(CurrentRange);
                                List<string> data2 = data.ToList();
                                Displaycols.DataSource = data;
                                ReturnCol.DataSource = data2;

                                ExcelMultiColumProp multiColumnProperty = fld as ExcelMultiColumProp;
                                if (multiColumnProperty != null)
                                {
                                    List<string> DataSourceForCol = data.ToList();
                                    DataSourceForCol.Insert(0, string.Empty);
                                    cmboReturnCol2.DataSource = DataSourceForCol;

                                    if (!string.IsNullOrEmpty(multiColumnProperty.ReturnCol))
                                        ReturnCol.SelectedItem = multiColumnProperty.ReturnCol;

                                    ReturnDataCollection RetData = multiColumnProperty.ReturnColumnData;

                                    // fill up the Target field with all the ret map fields
                                    // set the correct field as the selected item
                                    // display col data with all cols
                                    // set the corrct col as selected col

                                    // we could use Data but this set is already used as a DS so using the same 
                                    // will set the same index value. 
                                    string FldID = string.Empty;
                                    string ExcelCol = string.Empty;
                                    if (RetData != null)
                                    {
                                        FldID = RetData.DataCollection[0].FieldID;
                                        ExcelCol = RetData.DataCollection[0].ExcelDataSourceColumn;
                                    }

                                    if (View.FldMapper.GetType().Equals(typeof(RetrieveFieldMapper)))
                                    {
                                        List<RetrieveField> RetFld = FillRetrieveFields();
                                        cmboTargetField.DataSource = RetFld;
                                        cmboTargetField.DisplayMember = "FieldName";
                                        cmboTargetField.ValueMember = "TargetColumnIndex";
                                        if (!string.IsNullOrEmpty(FldID))
                                            cmboTargetField.SelectedIndex = RetFld.FindIndex(item => item.FieldId.Equals(FldID));
                                    }
                                    else // save only
                                    {
                                        List<SaveFieldBound> RetFld = FillSaveFields();
                                        cmboTargetField.DataSource = RetFld;
                                        cmboTargetField.DisplayMember = "FieldName";
                                        cmboTargetField.ValueMember = "TargetColumnIndex";
                                        if (!string.IsNullOrEmpty(FldID))
                                            cmboTargetField.SelectedIndex = RetFld.FindIndex(item => item.FieldId.Equals(FldID));
                                    }

                                    if (!string.IsNullOrEmpty(ExcelCol))
                                        cmboReturnCol2.SelectedIndex = cmboReturnCol2.FindStringExact(ExcelCol);
                                }
                            }
                        }
                    }
                }
            }
        }

        Microsoft.Office.Interop.Excel.Range CurrentRange;
        public string GetActiveColReference()
        {
            //if (radBasicProp.Checked)
            CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            return (ExcelHelper.GetAddress(CurrentRange));
        }

        public void RadioBasicSelect()
        {
            // disable advanced grop
            //grpBasicProp.Enabled = true;
            //grpAdvanceProp.Enabled = false;
            //MakeGroupControlsReadOnly(grpAdvanceProp.Controls);
            EnableMultiColumnControls(grpMultiColumnGroupBox.Controls, false);
        }

        public void RadioAdvanceSelect()
        {
            //grpBasicProp.Enabled = false;
            //grpAdvanceProp.Enabled = true;
            //MakeGroupControlsReadOnly(grpBasicProp.Controls);
            EnableMultiColumnControls(grpMultiColumnGroupBox.Controls, true);
        }

        private void EnableMultiColumnControls(Control.ControlCollection controlCollection, bool bEnable)
        {
            foreach (ListControl c in controlCollection.OfType<ListControl>())
                c.Enabled = bEnable;

            foreach (Label l in controlCollection.OfType<Label>())
                l.Enabled = bEnable;
        }

        public void GetMultiColums()
        {   
            // when user is on the advanced LH UI, if the current selection
            // is more than a single col, collect the column info 
            List<string> data = GetAllCols();
            List<string> data2 = data.ToList();
            if (data != null)
            {
                if (data.Count < 2)
                {
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_GetMultiColumns_InfoMsg"));
                    return;
                }

                Displaycols.DataSource = data;
                ReturnCol.DataSource = data;
                data2.Insert(0, String.Empty);
                cmboReturnCol2.DataSource = data2;
            }
        }

        /*
        // Lookahead multi column should have
        // target fields displayed so that users could assign
        // return col to a target field
        public List<RetrieveField> FillRetrieveFields()
        {
            if (View.FldMapper != null)

            {
                
                RetrieveFieldMapper fldMapper = View.FldMapper as RetrieveFieldMapper;
                return fldMapper.GetAllFieldsFromMap();
                
               
            }
            return null;
        }

        */


        /*
        // Lookahead multi column should have
        // target fields displayed so that users could assign
        // return col to a target field
        public List<SaveField> FillSaveields()
        {
            if (View.FldMapper != null)
            {
                    SaveFieldMapper fldMapper = View.FldMapper as SaveFieldMapper;
                    return fldMapper.GetAllFieldsFromMap();

                
            }
            return null;
        }

        */
        // in Editing a LH property, for Multi col prperty
        // pass the range to GetAllCols so that the currrent selection can be displayed
        public List<string> GetAllCols(Microsoft.Office.Interop.Excel.Range rngMulti = null)
        {
            if (rngMulti == null)
                CurrentRange = ExcelHelper.GetCurrentSelectedCol();
            else
                CurrentRange = rngMulti;

            List<string> Cols = new List<string>();

            foreach (Microsoft.Office.Interop.Excel.Range rng in CurrentRange.Columns)
            {
                Cols.Add(NumberToColum(rng.Column));
            }

            return Cols;
        }
        public override void Detach()
        {
            bool MultiColProp = false;
            if (CurrentSelection.Text == null || CurrentSelection.Text.Length == 0 && radBasicProp.Checked)
            {
                DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_DetachNoDataSource_InfoMsg"));
                return;
            }
            if (radAdvanceProp.Checked && Displaycols.Items.Count > 0)
            {
                MultiColProp = true;
            }
            if (View.FldMapper.RemoveProperty(LookAheadProp))
            {
                if (MultiColProp)
                {
                    // clear the list box and col dropdown
                    Displaycols.DataSource = null;
                    Displaycols.Items.Clear();
                    ReturnCol.DataSource = null;
                    ReturnCol.Items.Clear();
                }
                else
                    CurrentSelection.Text = "";
                View.Hide();
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("LOOKAHEADPROPCTL_Detach_InfoMsg"), resourceManager.GetResource("COMMON_DataSource_Text"));

                View.Show();
            }
        }

        public override LookAheadProperty Apply()
        {
            //this call should calll Model's save method but 
            // kept it in the controller since there is not
            // enough processing takes place in the model. 
            int TotalColSelected = GetAllCols().Count;
            if (radBasicProp.Checked)
            {
                if (string.IsNullOrEmpty(referenceinfo.Text)
                    && referenceinfo.Text.Length == 0)
                {
                    View.Hide();
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("LOOKAHEAD_ValidVal_ErrorMsg"), resourceManager.GetResource("COMMON_LookAheadProperties_ErrorMsg"));
                    View.Show();
                    return null;
                }
                else if (TotalColSelected > 1)
                {
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_ApplySingleColumn_InfoMsg"));
                    return null;
                }
            }

            if (radAdvanceProp.Checked)
            {
                if (TotalColSelected < 2 && Displaycols.Items.Count < 1 )
                {
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyGetCurrent_InfoMsg"));
                    return null;
                }
                // user changed the selection while saving the prop
                if (TotalColSelected < 2 && Displaycols.Items.Count >= 2)
                {
                    DisplayMessage(resourceManager.GetResource("LOOKAHEADPROPCTL_ApplyInvalidRange_InfoMsg"));
                    return null;
                }
            }

            ExcelDataSource prop = new ExcelDataSource();
            string namedRange = ExcelHelper.GetExcelCellName(CurrentRange);

            if (TotalColSelected > 1 && radAdvanceProp.Checked)
            {    // if the current selection has more than one col, set
                //  multi col propery set to true so that 
                //  run time UI can be populated with multi column
                prop = new ExcelMultiColumProp();
                prop.MultiCol = true;
                ((ExcelMultiColumProp)prop).ReturnCol = ReturnCol.SelectedItem.ToString();

                if (cmboTargetField.Enabled && cmboTargetField.SelectedIndex >= 1)
                {
                    // second col value
                    string SecondCol = cmboReturnCol2.SelectedItem.ToString();
                    string FieldId;
                    string TargetNR;
                    int TarColIndex = -1;
                    ObjectType fldtype;
                    if (cmboTargetField.SelectedItem.GetType().Equals(typeof(RetrieveField)))
                    {
                        RetrieveField RetFld = cmboTargetField.SelectedItem as RetrieveField;
                        TarColIndex = RetFld.TargetColumnIndex;
                        FieldId = RetFld.FieldId;
                        TargetNR = RetFld.TargetNamedRange;
                        fldtype = RetFld.Type;
                    }
                    else // save other field
                    {
                        SaveField RetFld = cmboTargetField.SelectedItem as SaveField;

                        TarColIndex = RetFld.TargetColumnIndex;
                        FieldId = RetFld.FieldId;
                        TargetNR = RetFld.TargetNamedRange;
                        fldtype = RetFld.Type;
                    }

                    // second col's values target field index
                    ReturnColumnData RetColData = new ReturnColumnData(TarColIndex, SecondCol, FieldId, TargetNR, fldtype);
                    ReturnDataCollection retColCollection = new ReturnDataCollection();
                    retColCollection.DataCollection.Add(RetColData);

                    // retColCollection.RetrieveMapID = View.FldMapper
                    ((ExcelMultiColumProp)prop).ReturnColumnData = retColCollection;
                }
            }

            if (namedRange != null && !string.IsNullOrEmpty(namedRange))
            {
                namedRange = CurrentRange.Name.Name;
            }
            else
            {
                namedRange = ExcelHelper.CreateUniqueNameRange();
                ExcelHelper.AssignNameToRange(CurrentRange, namedRange, false);
            }
            prop.TargetRange = namedRange;
            prop.SheetReference = ExcelHelper.GetAddress(CurrentRange);

            if (RefreshData.Checked)
            {
                prop.RefreshData = true;
            }
            else
                prop.RefreshData = false;
            prop.IsActive = IsActive.Checked ? true : false;

            LookAheadProp = prop;
            return LookAheadProp;
        }

        private static string NumberToColum(int col)
        {
            if (col < 1 || col > 16384) //Excel columns are one-based (one = 'A')
                throw new ArgumentException("col must be >= 1 and <= 16384");

            if (col <= 26) //one character
                return ((char)(col + 'A' - 1)).ToString();

            else if (col <= 702) //two characters
            {
                char firstChar = (char)((int)((col - 1) / 26) + 'A' - 1);
                char secondChar = (char)(col % 26 + 'A' - 1);

                if (secondChar == '@') //Excel is one-based, but modulo operations are zero-based
                    secondChar = 'Z'; //convert one-based to zero-based

                return string.Format("{0}{1}", firstChar, secondChar);
            }

            else //three characters
            {
                char firstChar = (char)((int)((col - 1) / 702) + 'A' - 1);
                char secondChar = (char)((col - 1) / 26 % 26 + 'A' - 1);
                char thirdChar = (char)(col % 26 + 'A' - 1);

                if (thirdChar == '@') //Excel is one-based, but modulo operations are zero-based
                    thirdChar = 'Z'; //convert one-based to zero-based

                return string.Format("{0}{1}{2}", firstChar, secondChar, thirdChar);
            }
        }

    }
}

