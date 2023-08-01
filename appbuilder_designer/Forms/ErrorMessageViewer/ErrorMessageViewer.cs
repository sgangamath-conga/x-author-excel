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
    public partial class ErrorMessageViewer : Form
    {
        private BindingList<ErrorMessageModel> ModelList = new BindingList<ErrorMessageModel>();
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        int nErrorMsgCount;

        public ErrorMessageViewer()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnClose.Text = resourceManager.GetResource("COMMON_btnClose_Text");
            Description.HeaderText = resourceManager.GetResource("ERRVIEWER_Description_HeaderText");
            EntityName.HeaderText = resourceManager.GetResource("ERRVIEWER_EntityName_HeaderText");
            EntityType.HeaderText = resourceManager.GetResource("ERRVIEWER_EntityType_HeaderText");
            ErrorNo.HeaderText = resourceManager.GetResource("ERRVIEWER_ErrorNo_HeaderText");
        }

        private void ErrorMessageViewer_Shown(object sender, EventArgs e)
        {
            lblErrorMessageHeader.Text = ErrorMessageHeader;
            dgvErrors.DataSource = ModelList;
        }

        public string ErrorMessageHeader { get; set; }

        public void AddResult(ValidationResult result)
        {
            foreach (string errorMsg in result.Messages)
            {
                ErrorMessageModel model = new ErrorMessageModel();
                model.EntityType = result.EntityType;
                model.EntityName = result.EntityName;
                model.Description = errorMsg;
                model.No = ++nErrorMsgCount;

                ModelList.Add(model);
            }
        }

        public void AddResults(List<ValidationResult> results)
        {
            foreach (ValidationResult result in results)
            {
                AddResult(result);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ModelList.Clear();
            this.Dispose();
        }
    }
}
