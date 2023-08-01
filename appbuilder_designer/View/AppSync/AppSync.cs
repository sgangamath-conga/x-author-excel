using Apttus.XAuthor.AppDesigner;
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class AppSyncView : Form
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;

        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        ObjectsMissingViewer objectsMissingViewer;
        FieldsMissingViewer fieldsMissingViewer;
        FieldsMismatchViewer fieldsMismatchViewer;
        SummaryViewer summaryViewer;
        DesignerAppSyncController appSyncController;

        public AppSyncView(DesignerAppSyncController designerSyncController)
        {
            appSyncController = designerSyncController;

            InitializeComponent();

            fieldsMissingViewer = new FieldsMissingViewer(designerSyncController);
            objectsMissingViewer = new ObjectsMissingViewer(designerSyncController);
            fieldsMismatchViewer = new FieldsMismatchViewer(designerSyncController);
            summaryViewer = new SummaryViewer(designerSyncController);
            summaryViewer.actionClicked += SummaryViewer_actionClicked;
            Icon = Properties.Resources.XA_AppBuilder_Icon;
            Text = Constants.DESIGNER_PRODUCT_NAME;
            SetCultureData();

            lblSummary_Click(null, null);

            designerSyncController.SetView(this);
        }

        private void SummaryViewer_actionClicked(object sender, EventArgs e)
        {
            switch (summaryViewer.currentAction)
            {
                case SummaryViewer.ControlActions.MISSINGOBJECTS:
                    {
                        lnkMissingObjects_Click(this, null);
                        break;
                    }
                case SummaryViewer.ControlActions.MISSINGFIELDS:
                    {
                        lnkMissingFields_Click(this, null);
                        break;
                    }
                case SummaryViewer.ControlActions.DATATYPEMISMATCH:
                    {
                        lnkFieldsMismatch_Click(this, null);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void SetCultureData()
        {
            btnFinish.Text = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Text");
            lblSyncAppTitle.Text = resourceManager.GetResource("RIBBON_btnValidate_Label");
            lnkFieldsMismatch.Text = resourceManager.GetResource("AppSync_lnkFieldsMismatch_Text");
            lnkMissingFields.Text = resourceManager.GetResource("DM_WizlblFields_Text");
            lnkMissingObjects.Text = resourceManager.GetResource("AppSync_lnkMissingObjects_Text");
            lblSummary.Text = resourceManager.GetResource("AppSync_lblSummary_Text");
            btnSyncApp.Text = resourceManager.GetResource("RIBBON_btnValidate_Label");
            lblSyncMessage.Text = resourceManager.GetResource("APPSYNC_SyncMessage_Info");
        }

        private void SwitchToControl(Control ctrl)
        {
            try
            {
                tableLayoutPanel3.SuspendLayout();
                if (tableLayoutPanel3.Controls.Count > 1)
                    tableLayoutPanel3.Controls.RemoveAt(1);
                tableLayoutPanel3.Controls.Add(ctrl, 1, 0);
                ctrl.Dock = DockStyle.Fill;
                ctrl.Show();
            }
            finally
            {
                tableLayoutPanel3.ResumeLayout();
            }
        }

        internal void UpdateLabels()
        {
            int nMissingObjects = appSyncController.GetMissingObjects().Count;
            lnkMissingObjects.Text = string.Format("{0} {1} ", nMissingObjects, resourceManager.GetResource("AppSync_lnkMissingObjects_Text"));

            int nMissingFields = appSyncController.GetMissingFieldInfo().Count;
            lnkMissingFields.Text = string.Format("{0} {1}", nMissingFields, resourceManager.GetResource("DM_WizlblFields_Text"));

            int nDataTypeMisMatch = appSyncController.GetDataTypeMismatchFieldInfo().Count;
            lnkFieldsMismatch.Text = string.Format("{0} {1}", nDataTypeMisMatch, resourceManager.GetResource("AppSync_lnkFieldsMismatch_Text"));
        }

        private void lnkMissingObjects_Click(object sender, EventArgs e)
        {
            SwitchToControl(objectsMissingViewer);
            objectsMissingViewer.RefreshGrid();
            SetActiveLabel(lnkMissingObjects);
        }

        private void lnkMissingFields_Click(object sender, EventArgs e)
        {
            SwitchToControl(fieldsMissingViewer);
            fieldsMissingViewer.RefreshGrid();
            SetActiveLabel(lnkMissingFields);
        }

        private void lnkFieldsMismatch_Click(object sender, EventArgs e)
        {
            SwitchToControl(fieldsMismatchViewer);
            fieldsMismatchViewer.RefreshGrid();
            SetActiveLabel(lnkFieldsMismatch);
        }

        void SetActiveLabel(Control Currentctrl)
        {
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                ctrl.BackColor = Color.White;
                ctrl.ForeColor = Color.Black;
            }
            Currentctrl.BackColor = Color.FromArgb(00, 0x78, 0xd7);
            Currentctrl.ForeColor = Color.White;
        }

        private void AppSyncView_Load(object sender, EventArgs e)
        {
            UpdateLabels();
            lblSyncMessage.ForeColor = Color.DarkOrange;
        }

        private void lblSummary_Click(object sender, EventArgs e)
        {
            SwitchToControl(summaryViewer);
            summaryViewer.RefreshData();
            SetActiveLabel(lblSummary);
        }
        public void RefreshSummaryData()
        {
            summaryViewer.RefreshData();
        }
        private void btnSyncApp_Click(object sender, EventArgs e)
        {
            AppSyncBackgroundWorker worker = new AppSyncBackgroundWorker(appSyncController);
        }
    }
}
