/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{    

    interface IApttusWizardPage
    {
        void ProcessPage();
        bool ValidatePage();
    }

    public class ApttusWizardPageView : UserControl, IApttusWizardPage
    {
        public string PageName { get; set; }
        public PageIndex PageNumber { get; set; }
        private Panel ParentControl;

        protected WizardModel Model;
        private static ErrorProvider errorProvider;

        internal delegate void evtPageActivate();
        internal event evtPageActivate ApttusWizardPageActivate;

        public ApttusWizardPageView()
        {
            if (errorProvider == null)
                errorProvider = new ErrorProvider();
        }

        public ApttusWizardPageView(Panel view, WizardModel model, PageIndex index)
        {
            ParentControl = view;
            Model = model;
            PageNumber = index;
            if (errorProvider == null)
                errorProvider = new ErrorProvider();
        }

        internal void ActivatePage()
        {
            SuspendLayout();

            foreach (Control ctrl in Controls)
            {
                ctrl.Enabled = true;
                ctrl.Visible = true;
            }
            Visible = true;
            Enabled = true;
            Parent = ParentControl;
            if (Dock != DockStyle.Fill)
                Dock = DockStyle.Fill;
            Show();

            if (ApttusWizardPageActivate != null)
                ApttusWizardPageActivate();

            ResumeLayout();
        }

        internal void DeActivatePage()
        {
            SuspendLayout();

            foreach (Control ctrl in Controls)
            {
                ctrl.Enabled = false;
                ctrl.Visible = false;
            }
            Visible = false;
            Enabled = false;
            Hide();

            ResumeLayout();
        }

        internal void DisplayError(Control control, string errorMsg, ErrorIconAlignment iconAlignment = ErrorIconAlignment.MiddleRight)
        {
            errorProvider.Clear();
            errorProvider.SetIconAlignment(control, iconAlignment);
            errorProvider.SetError(control, errorMsg);
        }

        internal void ClearErrorProvider()
        {
            errorProvider.Clear();
        }

        virtual public void ProcessPage() { }
        virtual public bool ValidatePage() { return false; }
    }

}
