using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataMigrationFieldsViewer : Form
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;
        List<ApttusFieldDS> currentFieldsDataSource = null;
        MigrationObject MObject;
        ApttusObject AppObject;
        DataMigrationController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public DataMigrationFieldsViewer(MigrationObject ObjMObject, DataMigrationController controller)
        {
            InitializeComponent();
            SetCultureData();
            MObject = ObjMObject;
            this.controller = controller;
            AppObject = objectManager.GetApttusObject(MObject.ObjectId, false);
            lblTitle.Text = AppObject.Name + " Fields";

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnFinish.Text = resourceManager.GetResource("COMMON_Apply_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnClose_Text");
        }

        private void RenderFields()
        {
            // Update record Types , if records types were created after Object has already been added to App.
            if (AppObject.RecordTypes != null && AppObject.RecordTypes != null && AppObject.RecordTypes.Count != AppObject.RecordTypes.Count)
                AppObject.RecordTypes = AppObject.RecordTypes;

            // Render Fields Grid
            currentFieldsDataSource = controller.GetFieldsList(MObject, AppObject.Fields);
            List<string> usedFields = applicationDefinitionManager.DefaultFields(AppObject);
            apttusFieldsView.AppObject = AppObject;
            apttusFieldsView.RenderFieldsGrid(ref currentFieldsDataSource, usedFields);
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            List<ApttusFieldDS> selFields = currentFieldsDataSource.Where(x => x.Included == true).ToList();
            //Are fields being removed
            IEnumerable<MigrationField> fieldsToBeRemoved = (from migrationField in MObject.Fields
                                                         where !selFields.Exists(fld => fld.Id == migrationField.FieldId && fld.Datatype == migrationField.DataType)
                                                         select migrationField);

            if (fieldsToBeRemoved.Count() > 0)
            {
                StringBuilder errorMessage;
                if (CanRemoveMigrationFields(fieldsToBeRemoved, out errorMessage))
                {
                    controller.UpdateFields(AppObject, MObject, selFields);
                    controller.SyncFieldsForClonedObjects(AppObject, MObject, selFields);
                    controller.ResolveObjectDependency();
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    ApttusMessageUtil.ShowInfo(errorMessage.ToString(), "Field Removal", this.Handle.ToInt64());
            }
            else
            {
                MigrationObject cyclicDependentObject;
                ApttusField cyclicDependentField;
                if (controller.CyclicDependencyExistsFor(MObject, selFields.ToList<ApttusField>(), out cyclicDependentObject, out cyclicDependentField))
                {
                    string circularReferenceMsg = resourceManager.GetResource("CIRCULAR_REFERENCE_MSG");
                    string errorMessage = string.Format(resourceManager.GetResource("APTTUSFIELDS_CIRCULARREFERENCE_ErrMsg"), MObject.ObjectName, cyclicDependentObject.ObjectName, cyclicDependentField.Name, cyclicDependentField.Id);
                    ApttusMessageUtil.ShowInfo(errorMessage, circularReferenceMsg, this.Handle.ToInt64());
                }
                else
                {
                    controller.UpdateFields(AppObject, MObject, selFields);
                    controller.SyncFieldsForClonedObjects(AppObject, MObject, selFields);
                    controller.ResolveObjectDependency();
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
        }

        private bool CanRemoveMigrationFields(IEnumerable<MigrationField> removedFields, out StringBuilder message)
        {
            message = new StringBuilder();
            IEnumerable<MigrationField> fieldsUsedInFilter = (from filterGroup in MObject.DataRetrievalAction.SearchFilter
                                                              where filterGroup != null && filterGroup.Filters != null && filterGroup.Filters.Count > 0
                                                              from filter in filterGroup.Filters
                                                              from removedField in removedFields
                                                              where filter.AppObjectUniqueId == MObject.AppObjectUniqueId && filter.FieldId == removedField.FieldId
                                                              select removedField);

            bool bCanRemove = fieldsUsedInFilter.Count() == 0;

            if (!bCanRemove)
            {
                message.Capacity = 100;
                message.Append("Fields : ");
                bool bCommaNeeded = false;
                foreach (MigrationField fieldUsedInFilter in fieldsUsedInFilter)
                {
                    message.Append(fieldUsedInFilter.FieldName).Append(" (").Append(fieldUsedInFilter.FieldId).Append(") ");
                    if (bCommaNeeded)
                        message.Append(",");
                    bCommaNeeded = true;
                }
                message.Append(fieldsUsedInFilter.Count() > 1 ? "are" : "is").Append(" used as search filter in data getter action, hence cannot be removed. To remove these fields, please remove fields from search filters.");
            }
            return bCanRemove;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void DataMigrationFieldsViewer_Load(object sender, EventArgs e)
        {
            RenderFields();
        }
    }
}
