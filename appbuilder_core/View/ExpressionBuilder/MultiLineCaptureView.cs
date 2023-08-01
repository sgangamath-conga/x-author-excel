/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.Core
{
    public partial class MultiLineCaptureView : Form
    {
        MultiLineCaptureController controller;
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public MultiLineCaptureView()
        {
            InitializeComponent();
            SetCultureData();
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COREMULTILINEVIEW_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COREMULTILINEVIEW_btnSave_Text");
            lblValue.Text = resourceManager.GetResource("COREMULTILINEVIEW_lblValue_Text");
        }
        internal void SetController(MultiLineCaptureController controller)
        {
            this.controller = controller;
        }

        private void ObjectAndFieldBrowserView_Load(object sender, EventArgs e)
        {
            IntializeAndBindView();
        }

        private void IntializeAndBindView()
        {
            // 1. Set the Label
            lblHeader.Text = String.Format(resourceManager.GetResource("COREMULTILINECAPTVIEW_IntializeAndBindView_Msg"),controller.AppObject.Name ,controller.AppField.Name);
            // 2. Set the Value
            txtValue.Text = controller.MultiLineValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput(txtValue.Text))
            {
                controller.MultiLineValue = txtValue.Text;
                this.DialogResult = DialogResult.OK;
                this.Dispose();
            }
        }

        private bool ValidateInput(string value)
        {
            bool isValid = false;
            string warningMessage = string.Empty;
            string errorMessage = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(value))
            {
                switch (controller.AppField.Datatype)
                {
                    case Datatype.Picklist:
                    case Datatype.Lookup:
                    case Datatype.String:
                    case Datatype.Email:
                        foreach (var item in value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            if (item.Contains(Constants.COMMA))
                                sb.AppendLine(item);

                        if (sb.Length > 0)
                        {
                            warningMessage = String.Format(resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputString_ErrMsg"), sb.ToString());
                            TaskDialogResult dr = ApttusMessageUtil.ShowWarning(warningMessage, resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputStringCAP_ErrMsg"), ApttusMessageUtil.YesNo, this.Handle.ToInt32());
                            if (dr == TaskDialogResult.Yes)
                                isValid = true;
                        }
                        else
                            isValid = true;

                        break;
                    case Datatype.Decimal:
                    case Datatype.Double:
                        Decimal decimalNumber;
                        foreach (var item in value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            if (!Decimal.TryParse(item, out decimalNumber))
                                sb.AppendLine(item);

                        if (sb.Length > 0)
                            errorMessage = String.Format(resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputnumeric_ErrMsg"),sb.ToString());
                        else
                            isValid = true;
                        break;

                    case Datatype.Date:
                    case Datatype.DateTime:
                        foreach (var item in value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            if (string.IsNullOrEmpty(Utils.IsValidDate(item, controller.AppField.Datatype)))
                                sb.AppendLine(item);

                        if (sb.Length > 0)
                            errorMessage = String.Format(resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputDate_ErrMsg"), sb.ToString());
                        else
                            isValid = true;
                        break;

                    case Datatype.Boolean:
                        Boolean booleanValue;
                        foreach (var item in value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                            if (!Boolean.TryParse(item, out booleanValue))
                                sb.AppendLine(item);

                        if (sb.Length > 0)
                            errorMessage = String.Format(resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputBool_ErrMsg"),sb.ToString());
                        else
                            isValid = true;
                        break;
                    case Datatype.Formula:
                        foreach (var item in value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            if(!Utils.IsValidFormula(item, controller.AppField.FormulaType))
                            {
                                sb.AppendLine(item);                                
                            }
                        }
                        if(sb.Length>0)
                        {
                            errorMessage = String.Format(resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputnumeric_ErrMsg"), sb.ToString());
                        }
                        else
                        {
                            isValid = true;
                        }
                        break;
                    case Datatype.Picklist_MultiSelect:
                    case Datatype.NotSupported:
                    case Datatype.Textarea:
                    case Datatype.Rich_Textarea:
                        errorMessage = resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputInOp_ErrMsg");
                        break;
                    default:
                        break;
                }
            }
            else
                isValid = true;

            if (!string.IsNullOrEmpty(errorMessage) && !isValid)
                ApttusMessageUtil.ShowError(errorMessage, resourceManager.GetResource("MULTILINECAPTVIEW_ValidateInputStringCAP_ErrMsg"), this.Handle.ToInt32());

            return isValid;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }


    }
}
