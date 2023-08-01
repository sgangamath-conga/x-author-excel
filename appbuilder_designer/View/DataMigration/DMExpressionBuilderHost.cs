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
    public partial class DMExpressionBuilderHost : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ApttusObject apttusObject;
        MigrationObject migrationObject;
        public DMExpressionBuilderHost(ApttusObject appObj, MigrationObject mObj)
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            apttusObject = appObj;
            migrationObject = mObj;
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
            string errormessage;
            List<SearchFilterGroup> currentFilters = ExpressionControl.SaveExpression(out errormessage);
            ManageRemovedFilters(currentFilters);
            migrationObject.DataRetrievalAction.SearchFilter = currentFilters;
            this.Close();

        }

        /// <summary>
        /// add user removed filters in collection,so we can have list of filters removed by user manually.
        /// </summary>
        /// <param name="currentFilters"></param>
        void ManageRemovedFilters(List<SearchFilterGroup> currentFilters)
        {
            migrationObject.DataRetrievalAction.SearchFilter[0].Filters.ForEach((filter) =>
            {
                if (!currentFilters[0].Filters.Exists(x => x.FieldId == filter.FieldId))
                {
                    migrationObject.RemovedFilters.Add(filter.FieldId);
                }
            });
        }

        private void DMExpressionBuilderHost_Load(object sender, EventArgs e)
        {
            ExpressionControl.SetCultureData();
            ExpressionControl.TargetObject = apttusObject;

            List<SearchFilterGroup> filters = migrationObject.DataRetrievalAction.SearchFilter.Count == 0 ? null : migrationObject.DataRetrievalAction.SearchFilter;
            ExpressionControl.SetExpressionBuilder(filters, apttusObject);
        }


    }
}
