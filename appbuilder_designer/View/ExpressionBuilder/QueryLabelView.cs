/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class QueryLabelView : Form
    {
        public bool LeftParent { get; set; }

        private SearchFilter currentSearchFilter;
        ExpressionBuilderView parentExpressionBuilder;
        int currentRow;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SearchFilter SearchFilter
        {
            get { return currentSearchFilter; }
        }

        public QueryLabelView()
        {
            InitializeComponent();
          //  SetCultureData();
            timer.Interval = Constants.EXPRESSION_BUILDER_LABELHOVERTIMEOUT;
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            lblLabel.Text = resourceManager.GetResource("QUERYLABELVIEW_lblLabel_Text");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (LeftParent && !this.ContainsFocus)
            {
                this.Hide();
                LeftParent = false;
            }
            else
            {
                timer.Start();
            }
        }

        public void ShowForm(SearchFilter searchFilter, ExpressionBuilderView expressionBuilderView, int row)
        {
            ResetSearchFilter(searchFilter);

            parentExpressionBuilder = expressionBuilderView;
            currentRow = row;

            timer.Stop();
            timer.Start();

            this.Show();

        }

        public void ResetSearchFilter(SearchFilter searchFilter)
        {
            currentSearchFilter = searchFilter;
            txtLabel.Text = string.IsNullOrEmpty(currentSearchFilter.SearchFilterLabel)
                ? ExpressionBuilderHelper.GetDisplayTextFromQueryObjects(currentSearchFilter)
                : currentSearchFilter.SearchFilterLabel;
            txtLabel.Focus();

        }

        private void btnSaveLabel_Click(object sender, EventArgs e)
        {
            int labelLength = txtLabel.Text.Trim().Length;
            if (labelLength > 0)
            {
                if (labelLength <= Constants.EXPRESSION_BUILDER_LABELMAXLENGTH)
                {
                    currentSearchFilter.SearchFilterLabel = txtLabel.Text.Trim();
                    parentExpressionBuilder.UpdateCurrentExpression(currentRow, currentSearchFilter);
                    txtLabel.Clear();

                    this.Hide();
                }
                else
                    ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("QUERYLBLVEW_LableLength_ErrorMsg"), labelLength.ToString(), Constants.EXPRESSION_BUILDER_LABELMAXLENGTH.ToString()), resourceManager.GetResource("QUERYLBLVEW_LableLengthCap_ErrorMsg"));                
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void QueryLabelView_Load(object sender, EventArgs e)
        {
            SetCultureData();
        }

    }
}
