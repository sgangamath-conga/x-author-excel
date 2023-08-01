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
    public partial class FieldMappingView : UserControl
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public FieldMappingView()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        public void SetCultureData()
        {
            lblFieldsMapping.Text = resourceManager.GetResource("UCFLDSMAPVIEW_label1_Text");
            lblTargetFields.Text = resourceManager.GetResource("UCFLDSMAPVIEW_label2_Text");
            lblSourceFields.Text = resourceManager.GetResource("UCFLDSMAPVIEW_label3_Text");
        }
        private SearchAndSelect searchAndSelectAction;
        private SearchAndSelectActionMapping Mapping;
        public Guid FieldAppObject { get; set; }
        public RetrieveMap RetrieveMap { get; set; }

        public SearchAndSelect SearchAndSelectAction {
            get {
                return searchAndSelectAction;
            }
            set {
                if (searchAndSelectAction != value)
                {
                    searchAndSelectAction = value;
                    PopulateFields();
                }
            }
        }



        private void CreateFieldMapping(ResultField field)
        {

            int rowNo = tblFieldsMapping.RowCount - 1;

            Label lblNo = new Label();
            lblNo.Text = rowNo.ToString();
            lblNo.Height = 24;
            lblNo.TextAlign = ContentAlignment.MiddleLeft;
            lblNo.AutoSize = true;

            TextBox txtSourceField = new TextBox();
            txtSourceField.Text = field.HeaderName;
            txtSourceField.Height = 24;
            txtSourceField.Dock = DockStyle.Fill;
            txtSourceField.ReadOnly = true;
            txtSourceField.Tag = field.Id;

            Label lblArrow = new Label();
            lblArrow.Text = "->";
            lblArrow.Height = 24;
            lblArrow.TextAlign = ContentAlignment.MiddleLeft;
            lblArrow.AutoSize = true;

            ComboBox cbo_TargetField = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo_TargetField.DropDown += cbo_DropDown;

            tblFieldsMapping.Controls.Add(lblNo, ExpressionItemType.SrNoLabel, rowNo);
            tblFieldsMapping.Controls.Add(txtSourceField, ExpressionItemType.ObjectFieldText, rowNo);
            tblFieldsMapping.Controls.Add(lblArrow, ExpressionItemType.ObjectFieldBrowse, rowNo);
            tblFieldsMapping.Controls.Add(cbo_TargetField, ExpressionItemType.ValueTypePicklist, rowNo);

            List<RetrieveField> fields = (from rg in RetrieveMap.RepeatingGroups.Where(rg => rg.AppObject == FieldAppObject)
                                          select rg.RetrieveFields.ToList()).FirstOrDefault();
            if (fields == null)
                fields = new List<RetrieveField>();

            fields.Insert(0, new RetrieveField { FieldId = string.Empty, FieldName = string.Empty });

            cbo_TargetField.DataSource = fields;
            cbo_TargetField.DisplayMember = "FieldName";
            cbo_TargetField.ValueMember = "FieldId";

            tblFieldsMapping.RowCount++;
        }

        void cbo_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth = 0;
            foreach (object currentItem in ((ComboBox)sender).Items)
            {
                if (currentItem is RetrieveField)
                    newWidth = (int)g.MeasureString(((RetrieveField)currentItem).FieldName, font).Width + vertScrollBarWidth;

                if (width < newWidth)
                    width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }

        public void ResetFieldMapping()
        {
            if (tblFieldsMapping.RowCount == 2)
                return;


            while (tblFieldsMapping.RowCount > 1)
            {
                int row = tblFieldsMapping.RowCount - 1;
                for (int i = 0; i < tblFieldsMapping.ColumnCount; i++)
                {
                    Control c = tblFieldsMapping.GetControlFromPosition(i, row);
                    if (c != null)
                    {
                        tblFieldsMapping.Controls.Remove(c);
                        c.Dispose();
                    }
                }

                tblFieldsMapping.RowCount--;
            }
            tblFieldsMapping.RowCount = 2;

        }

        private void PopulateFields()
        {
            try
            {
                lblObject.Text = ApplicationDefinitionManager.GetInstance.GetAppObject(searchAndSelectAction.TargetObject).Name;
                tblFieldsMapping.SuspendLayout();

                ResetFieldMapping();
                foreach (ResultField field in SearchAndSelectAction.ResultFields)
                    CreateFieldMapping(field);
            }
            finally
            {
                tblFieldsMapping.ResumeLayout();
            }
        }

        public SearchAndSelectActionMapping Save()
        {
            //RepeatingGroup of the field, on which the lookahead property is configured.
            RepeatingGroup repeatingGroup = RetrieveMap.RepeatingGroups.Where(rg => rg.AppObject == FieldAppObject).FirstOrDefault();
            if (repeatingGroup == null)
                return null;

            Mapping = new SearchAndSelectActionMapping();
            Mapping.SourceObject = searchAndSelectAction.TargetObject;
            Mapping.TargetObject = FieldAppObject;
            Mapping.SearchAndSelectActionId = searchAndSelectAction.Id;
            Mapping.RepeatingGroupTargetNamedRange = repeatingGroup.TargetNamedRange;

            Mapping.MappedFields = new List<MappedField>();

            for (int i = 1; i < tblFieldsMapping.RowCount - 1; ++i)
            {
                bool bAddItem = false;
                MappedField field = new MappedField();
                Control c = tblFieldsMapping.GetControlFromPosition(ExpressionItemType.ObjectFieldText, i);

                if (c != null && c.GetType().Equals(typeof(TextBox)))
                    field.SourceFieldId = Convert.ToString((c as TextBox).Tag);

                c = tblFieldsMapping.GetControlFromPosition(ExpressionItemType.ValueTypePicklist, i);
                if (c != null && c.GetType().Equals(typeof(ComboBox)))
                {
                    ComboBox cbo = c as ComboBox;
                    RetrieveField rf = cbo.SelectedItem as RetrieveField;
                    if (!String.IsNullOrEmpty(rf.FieldId))
                    {
                        field.TargetFieldId = rf.FieldId;
                        bAddItem = true;
                    }
                    if (bAddItem)
                        Mapping.MappedFields.Add(field);
                }
            }
            return Mapping;
        }


        internal void LoadMappingFields(SearchAndSelectActionMapping searchAndSelectActionMapping)
        {
            int start = 1;
            foreach (MappedField field in searchAndSelectActionMapping.MappedFields)
            {
                for (int i = start; i < tblFieldsMapping.RowCount - 1; ++i)
                {
                    Control c = tblFieldsMapping.GetControlFromPosition(ExpressionItemType.ObjectFieldText, i);
                    if (c != null && c.GetType().Equals(typeof(TextBox)))
                        if (field.SourceFieldId == Convert.ToString((c as TextBox).Tag))
                        {
                            c = tblFieldsMapping.GetControlFromPosition(ExpressionItemType.ValueTypePicklist, i);
                            if (c != null && c.GetType().Equals(typeof(ComboBox)))
                            {
                                ComboBox cbo = c as ComboBox;
                                if (!String.IsNullOrEmpty(field.TargetFieldId))
                                {
                                    cbo.SelectedValue = field.TargetFieldId;
                                    start = i + 1;
                                    break;
                                }
                            }
                        }
                }
            }
        }
    }
}
