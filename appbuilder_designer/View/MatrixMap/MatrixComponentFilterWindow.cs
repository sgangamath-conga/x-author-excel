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
    public partial class MatrixComponentFilterWindow : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public MatrixComponentFilterWindow()
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnSave.Text = resourceManager.GetResource("COMMON_Save_AMPText");
        }

        public ApttusObject ApttusObject { get; set; }
        public List<SearchFilterGroup> WhereFilterGroups { get; set; }

        private void MatrixComponentFilterWindow_Load(object sender, EventArgs e)
        {
            expressionBuilderView.TargetObject = ApttusObject;
            expressionBuilderView.SetCultureData();
            expressionBuilderView.SetExpressionBuilder(WhereFilterGroups, ApttusObject);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string errMsg;
            WhereFilterGroups = expressionBuilderView.SaveExpression(out errMsg);
            DialogResult = DialogResult.OK;
        }
    }
}
