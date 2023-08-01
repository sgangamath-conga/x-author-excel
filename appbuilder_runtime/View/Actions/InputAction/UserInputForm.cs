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

namespace Apttus.XAuthor.AppRuntime
{
    public partial class UserInputForm : Form
    {
        private UserInputController inputActionController;
        private List<SearchFilterGroup> SearchFilterGroups;
        private string ActionID;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;


        public UserInputForm()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }
        private void SetCultureData()
        {
            btnCancel.Text = resourceManager.GetResource("USERINPUTFORM_btnCancel_Text");
            btnNext.Text = resourceManager.GetResource("USERINPUTFORM_btnNext_Text");
        }
        public UserInputForm(UserInputController inputActionController, string actionID, List<SearchFilterGroup> searchFilterGroups)
        {
            ActionID = actionID;
            SearchFilterGroups = searchFilterGroups;
            this.inputActionController = inputActionController;

            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;
        }

        private void inputActionUserControl1_Load(object sender, EventArgs e)
        {
            inputActionUserControl1.setController(inputActionController);
            inputActionUserControl1.PopulateControls(ActionID, SearchFilterGroups[0].Filters);

            ResizeForm();
        }

        internal void ResizeForm()
        {
            //Form Height = TitlBarHeight + controlTableLayoutPanelHeight + DoneButtonPanelHeight + FormBorderHeight
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            int titleHeight = screenRectangle.Top - this.Top;

            Height = inputActionUserControl1.ControlPanelPrefferedHeight + titleHeight + (int)tableLayoutPanel1.RowStyles[1].Height + SystemInformation.FrameBorderSize.Height + 15;
            Width = inputActionUserControl1.ControlPanelPrefferedWidth + SystemInformation.FrameBorderSize.Width + 75;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            inputActionUserControl1.UpdateValuesInDataSet();
            Close();
        }            
    }
}
