using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ObjectsMissingViewer : UserControl
    {
        DesignerAppSyncController appValidator;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public ObjectsMissingViewer(DesignerAppSyncController appValidator)
        {
            InitializeComponent();
            SetCultureData();
            this.appValidator = appValidator;
        }
        private void SetCultureData()
        {
            lblMissingObjects.Text = resourceManager.GetResource("ObjectMissingViewer_lblMissingObjects_Text");
            lblMissingObjectDescription.Text = string.Format(resourceManager.GetResource("ObjectMissingViewer_lblMissingObjectDescription_Text"),resourceManager.CRMName);
            ObjectName.HeaderText = resourceManager.GetResource("COMMON_ObjectName_Text");
            ObjectID.HeaderText = resourceManager.GetResource("ObjectMissingViewer_ObjectID_Text");
        }
        internal void RefreshGrid()
        {
            DgvObjectsMissing.AutoGenerateColumns = false;
            DgvObjectsMissing.DataSource = appValidator.GetMissingObjects().ToList();
            SetDescriptionText();
        }
        void SetDescriptionText()
        {
            lblMissingObjectDescription.Text = DgvObjectsMissing.RowCount > 0 ?
                string.Format(resourceManager.GetResource("ObjectMissingViewer_lblMissingObjectDescription_Text"),resourceManager.CRMName) : resourceManager.GetResource("AppSync_ObjectsMissingViewer_NoData_Text");
        }

        private void ObjectsMissingViewer_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}
