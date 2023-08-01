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
    public partial class ExpressionBuilderHost : Form
    {
        private RecordSelectionPage Page;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ExpressionBuilderHost(RecordSelectionPage page)
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            Page = page;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");
            btnSave.Text = resourceManager.GetResource("COMMON_btnSave_Text");
        }

        public ExpressionBuilderView ExpressionControl
        {
            get
            {
                return expressionBuilderView1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Page.UpdateExpressionModel();
        }

        private void expressionBuilderView1_Load(object sender, EventArgs e)
        {
            expressionBuilderView1.IsExpressionBuilderLaunchedFromQuickApp = true;
            expressionBuilderView1.SetCultureData();
            ApttusObject obj = Page.TargetObject;
            ExpressionControl.TargetObject = obj;

            List<SearchFilterGroup> filters = Page.expressionModel.Filters.Count == 0 ? null : Page.expressionModel.Filters;
            //if (filters == null && Page.PageNumber == PageIndex.ChildObjectRecordSelectionPage)
            //    filters = Page.CreateChildLookupFilter();
            ExpressionControl.SetExpressionBuilder(filters, obj);
        }

        private void ExpressionBuilderHost_Shown(object sender, EventArgs e)
        {
            expressionBuilderView1.MaximumSize = new Size(expressionBuilderView1.PreferredSize.Width, expressionBuilderView1.PreferredSize.Height);
        }     
    }
}
