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

namespace Apttus.XAuthor.AppDesigner.Forms
{
    /* after user select the app, user will be presented with different edition / product type
     * and this ui will capture the product type.
     * and menus will be enabled based on the product type
     * Data migration and power admin are treated the same. 
     * During the activation, we will check the license and this will allow users to explore XAE capabilities. 
     * 
     */
    public partial class ApplicationTypeView : Form
    {
        AppTypeController controller;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public ApplicationTypeView(AppTypeController AppTypeController)
        {
            InitializeComponent();
            controller = AppTypeController;
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }
        private void SetCultureData()
        {

            //this.rbPowerAdmin.Text = resourceManager.GetResource("EDITION_POWERADMIN");
            //this.rbReadOnlyEdn.Text = resourceManager.GetResource("EDITION_READONLY");
            //this.rbEnterprise.Text = resourceManager.GetResource("EDTION_ENTERPRISE");
            //this.rbPreso.Text = resourceManager.GetResource("EDITION_PRESTO");
            //this.rbDataMigration.Text = resourceManager.GetResource("EDITION_DATAMIGRATION");

            string AppType = null;
            int cLeft = 1;
            if (controller.EditionsUI.Count > 0)
            {
                foreach (LicenseEditionStruct Lic in controller.EditionsUI)
                {
                    System.Windows.Forms.RadioButton rboAppType = new System.Windows.Forms.RadioButton();
                    if (Lic.Name.Equals("Data Migration")) continue;
                    this.panel1.Controls.Add(rboAppType);
                    rboAppType.AutoSize = true;
                    rboAppType.Top = cLeft * 20;
                    rboAppType.Left = 100;
                    rboAppType.Text = Lic.Name;
                    rboAppType.Tag = Lic.ID;
                    cLeft += 1;
                    //ii.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
                }
                this.panel1.AutoScroll = true;

            }
            btnCreate.Text = resourceManager.GetResource("COMMON_Create_AMPText");
            lblTitle.Text = resourceManager.GetResource("EDITION_SELECT_APP_TYPE");
            btnCancel.Text = resourceManager.GetResource("COMMON_btnCancel_Text");

        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string AppType = null;
            if (CRMContextManager.Instance.ActiveCRM == CRM.Salesforce)
            {
                foreach (Control control in this.panel1.Controls)
                {
                    if (control is RadioButton)
                    {
                        RadioButton radio = control as RadioButton;
                        if (radio.Checked)
                        {
                            AppType = radio.Tag.ToString();
                            break;
                        }
                    }
                }
            }
            else
                AppType = Constants.ADMIN_EDITION;

            if (AppType == null)
            {
                ApttusMessageUtil.ShowInfo(string.Format(resourceManager.GetResource("EDITIONUI_ERROR"), resourceManager.CRMName), resourceManager.GetResource("EDITION_APP_TYPE"));
                return;
            }
            try
            {

                if (controller != null)
                {
                    if (AppType.Equals("DATAMIGRATION"))
                        AppType = Constants.ADMIN_EDITION;

                    ApplicationHelper.GetInstance.EditionType = AppType;
                    ApplicationHelper.GetInstance.CreateApplication(controller.AppName, controller.FilePath, controller.NewOrExisting);
                }
                this.Dispose();

            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, "Application Create : Error");
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
