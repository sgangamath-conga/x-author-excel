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
using System.Threading;
using Apttus.XAuthor.AppDesigner.Modules;

namespace Apttus.XAuthor.AppDesigner
{
    public enum PageIndex
    {
        AppSelectionPage = 0,
        ParentObjectSelectionPage = 1,
        ChildObjectSelectionPage = 2,
        ParentObjectRecordSelectionPage = 3,
        ChildObjectRecordSelectionPage = 4,
        QuickAppSettingsPage = 5
    }

    public partial class QuickAppWizardView : Form
    {
        private WaitWindowView waitForm;
        private PageIndex _currentPage;
        private WizardModel Model;
        private List<ApttusWizardPageView> WizardPages;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        private PageIndex[] ApplicablePagesForListApp = new PageIndex[] { PageIndex.AppSelectionPage, PageIndex.ParentObjectSelectionPage, PageIndex.ParentObjectRecordSelectionPage, PageIndex.QuickAppSettingsPage };

        private PageIndex[] ApplicablePagesForParentChildApp = new PageIndex[] { PageIndex.AppSelectionPage, PageIndex.ParentObjectSelectionPage, 
                                                                            PageIndex.ChildObjectSelectionPage, PageIndex.ParentObjectRecordSelectionPage,
                                                                            PageIndex.ChildObjectRecordSelectionPage, PageIndex.QuickAppSettingsPage };
        public bool IsQuickAppInEditMode { get; private set;  }

        private QuickAppEditModeData quickAppEditMode;        


        public QuickAppWizardView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
            IsQuickAppInEditMode = false;
            ObjectManager.ResetObjectCache();
        }


        private void SetCultureData()
        {
            btnBack.Text = resourceManager.GetResource("QAWIZARD_btnBack_Text");
            btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            btnNext.Text = resourceManager.GetResource("QAWIZARD_btnNext_Text");
            lblPageTitle.Text = resourceManager.GetResource("QAWIZARD_lblPageTitle_Text");
        }

        public QuickAppWizardView(WizardModel model, string appName, string uniqueId ) : this()
        {
            if (model != null)
            {
                IsQuickAppInEditMode = true;
                Model = model;
                quickAppEditMode = new QuickAppEditModeData { EditingAppName = appName, EditingUniqueId = uniqueId };
                SetCultureData();
            }
        }

        private void QuickAppWizard_Load(object sender, EventArgs e)
        {
            WizardPages = new List<ApttusWizardPageView>();

            if (!IsQuickAppInEditMode)
                Model = new WizardModel();

            LoadWizardPages();

            lblPageTitle.Text = IsQuickAppInEditMode ? resourceManager.GetResource("QAWIZARD_lblPageTitleEdit_Text") +" "+ quickAppEditMode.EditingAppName : resourceManager.GetResource("QAWIZARD_lblPageTitle_Text");

            //Set the first page(Application Selection Page) when the wizard is made visible for the first time.
            if (!IsQuickAppInEditMode)
            {
                //Set the first page(Application Selection Page) when the wizard is made visible for the first time.
                CurrentPage = PageIndex.AppSelectionPage;
                SetCurrentPage(WizardPages[0]);
            }
            else
            {
                CurrentPage = PageIndex.ParentObjectSelectionPage;
                SetCurrentPage(WizardPages[0]);
            }
        }

        private PageIndex CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                PageIndex oldValue = _currentPage;
                updatePageSubTitle(value);
                _currentPage = value;
                OnPageNumberChanged();
            }
        }

        private void OnPageNumberChanged()
        {
            if (_currentPage == PageIndex.AppSelectionPage || (IsQuickAppInEditMode && _currentPage == PageIndex.ParentObjectSelectionPage)) //For First Page.
            {
                btnBack.Enabled = false;
                btnNext.Enabled = true;
            }
            else
            {
                PageIndex pageIndex = GetLastPage();
                if (_currentPage == pageIndex)
                    btnNext.Enabled = false;
                else
                    btnNext.Enabled = true;
                btnBack.Enabled = true;
            }
        }


        private void LoadWizardPages()
        {
            if (!IsQuickAppInEditMode)
            {
                QuickAppSelectionPage appSelectionPage = new QuickAppSelectionPage(pnlPage, Model, PageIndex.AppSelectionPage, IsQuickAppInEditMode);
                WizardPages.Add(appSelectionPage);
            }

            ObjectSelectionPage parentObjectSelectionPage = new ObjectSelectionPage(pnlPage, Model, PageIndex.ParentObjectSelectionPage, IsQuickAppInEditMode);
            WizardPages.Add(parentObjectSelectionPage);

            ObjectSelectionPage childObjectSelectionPage = new ObjectSelectionPage(pnlPage, Model, PageIndex.ChildObjectSelectionPage, IsQuickAppInEditMode);
            WizardPages.Add(childObjectSelectionPage);

            RecordSelectionPage parentObjectRecordSelectionPage = new RecordSelectionPage(pnlPage, Model, PageIndex.ParentObjectRecordSelectionPage, IsQuickAppInEditMode);
            WizardPages.Add(parentObjectRecordSelectionPage);

            RecordSelectionPage childObjectRecordSelectionPage = new RecordSelectionPage(pnlPage, Model, PageIndex.ChildObjectRecordSelectionPage, IsQuickAppInEditMode);
            WizardPages.Add(childObjectRecordSelectionPage);

            QuickAppSettingsPage settingsPage = new QuickAppSettingsPage(pnlPage, Model, PageIndex.QuickAppSettingsPage, IsQuickAppInEditMode);
            WizardPages.Add(settingsPage);
        }

        /// <summary>
        /// Loads the page(UserControl) in the wizard and makes the usercontrol active and visible.
        /// </summary>
        /// <param name="currentPage"></param>
        private void SetCurrentPage(ApttusWizardPageView currentPage)
        {
            foreach (ApttusWizardPageView wizardPage in WizardPages)
            {
                if (wizardPage.PageNumber == currentPage.PageNumber)
                    wizardPage.ActivatePage();
                else
                    wizardPage.DeActivatePage();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.SuspendLayout();
                GoBack();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
                if (ex.Message.Contains("INVALID_SESSION_ID"))
                    this.Close();
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// performs the back operation in the wizard
        /// </summary>
        private void GoBack()
        {
            switch (Model.AppType)
            {
                case QuickAppType.ListApp:
                    LoadPreviousPageForApp(ApplicablePagesForListApp);
                    break;

                case QuickAppType.ParentChildApp:
                    LoadPreviousPageForApp(ApplicablePagesForParentChildApp);
                    break;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                GoNext();
            }
            catch (Exception ex)
            {
                ApttusObjectManager.HandleError(ex);
                if (ex.Message.Contains("INVALID_SESSION_ID"))
                    this.Close();
            }
        }

        /// <summary>
        /// Performs the next operation in the wizard
        /// </summary>
        private void GoNext()
        {
            switch (Model.AppType)
            {
                case QuickAppType.ListApp:
                    LoadNextPageForApp(ApplicablePagesForListApp);
                    break;

                case QuickAppType.ParentChildApp:
                    LoadNextPageForApp(ApplicablePagesForParentChildApp);
                    break;
            }
           
        }

        /// <summary>
        /// updates the page index and loads the next page on next button click
        /// </summary>
        /// <param name="AppApplicablePages"></param>
        private void LoadNextPageForApp(PageIndex[] AppApplicablePages)
        {
            ApttusWizardPageView currPage = WizardPages.Where(page => page.PageNumber == CurrentPage).FirstOrDefault();
            if (currPage.ValidatePage())
            {
                currPage.ProcessPage();

                for (int i = 0; i < WizardPages.Count; ++i)
                {
                    if (CurrentPage == AppApplicablePages[i])
                    {
                        CurrentPage = AppApplicablePages[i + 1];
                        break;
                    }
                }
                ApttusWizardPageView newPage = WizardPages.Where(page => page.PageNumber == CurrentPage).FirstOrDefault();
                SetCurrentPage(newPage);
            }
            updateCancelButton();
        }

        /// <summary>
        /// Updates the page index and loads the previous page on back button click.
        /// </summary>
        /// <param name="AppApplicablePages"></param>
        private void LoadPreviousPageForApp(PageIndex[] AppApplicablePages)
        {
            ApttusWizardPageView currPage = WizardPages.Where(page => page.PageNumber == CurrentPage).FirstOrDefault();
            for (int i = AppApplicablePages.Count() - 1; i > 0; --i)
            {
                if (CurrentPage == AppApplicablePages[i])
                {
                    CurrentPage = AppApplicablePages[i - 1];
                    break;
                }
            }
            ApttusWizardPageView newPage = WizardPages.Where(page => page.PageNumber == CurrentPage).FirstOrDefault();
            SetCurrentPage(newPage);
            updateCancelButton();
        }

        /// <summary>
        /// Gets the LastPage per app type.
        /// </summary>
        /// <returns></returns>
        private PageIndex GetLastPage()
        {
            return PageIndex.QuickAppSettingsPage;
        }

        /// <summary>
        /// Dynamically updates the button to Cancel or OK based on the PageIndex. 
        /// If the user is at the last page, btnCancel will display as Finish and the DialogResult will be updated accordingly.
        /// If the user is at the page other than last page, btnCancel will act as a Cancel Button.
        /// </summary>
        private void updateCancelButton()
        {
            if (CurrentPage == GetLastPage())
            {
                btnCancel.DialogResult = System.Windows.Forms.DialogResult.None;
                btnCancel.Text = resourceManager.GetResource("COMMON_Finish_Text");
            }
            else
            {
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Text = resourceManager.GetResource("COMMON_Cancel_AMPText");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnCancel.DialogResult == System.Windows.Forms.DialogResult.None)
            {
                //Invoke ProcessPage for Last Page.
                ApttusWizardPageView lastPage = WizardPages.Where(page => page.PageNumber == GetLastPage()).FirstOrDefault();
                if (lastPage != null && lastPage.ValidatePage())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    lastPage.ProcessPage();

                    RunWorkerThreadToCreateApp();
                }
            }
            else
            {
                if (!IsQuickAppInEditMode)
                    ApplicationDefinitionManager.GetInstance.AppObjects.Clear();
            }
        }

        private void RunWorkerThreadToCreateApp()
        {
            Thread thread = null;

            if (IsQuickAppInEditMode)
                thread = new Thread(new ThreadStart(UpdateApp));                
            else
                thread = new Thread(new ThreadStart(BuildApp));

            thread.SetApartmentState(ApartmentState.STA);

            waitForm = new WaitWindowView();
            waitForm.StartPosition = FormStartPosition.CenterParent;

            thread.Start();
            waitForm.ShowDialog();
        }

        [STAThread]
        private void UpdateApp()
        {
            try
            {
                new QuickAppManager().UpdateApplication(Model, waitForm,  quickAppEditMode.EditingAppName, quickAppEditMode.EditingUniqueId);
            }
            catch (Exception ex)
            {
                Invoke(new MethodInvoker(() =>
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("QUICKAPP_Creating_ErrorMsg"), resourceManager.GetResource("QUICKAPP_CreatingCap_ErrorMsg"), resourceManager.GetResource("QUICKAPP_CreatingInstru_ErrorMsg"), ex.Message, this.Handle.ToInt32());
                }));

            }
            finally
            {
                if (waitForm != null)
                {
                    waitForm.CloseWaitWindow();
                    waitForm = null;
                }
            }
        }

        [STAThread]
        private void BuildApp()
        {
            try
            {
                new QuickAppManager().BuildApplication(Model, waitForm);
            }
            catch (Exception ex)
            {
                Invoke(new MethodInvoker(() =>
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("QUICKAPP_Creating_ErrorMsg"), resourceManager.GetResource("QUICKAPP_CreatingCap_ErrorMsg"), resourceManager.GetResource("QUICKAPP_CreatingInstru_ErrorMsg"), ex.Message, this.Handle.ToInt32());
                }));
            }
            finally
            {
                if (waitForm != null)
                {
                    waitForm.CloseWaitWindow();
                    waitForm = null;
                }
            }
        }

        private void updatePageSubTitle(PageIndex currentPageIndex)
        {
            switch (currentPageIndex)
            {
                case PageIndex.AppSelectionPage:
                    lblPageSubTitle.Text = resourceManager.GetResource("QAWIZARD_lblPageSubTitle_AppSelection_Text");
                    break;
                case PageIndex.ParentObjectSelectionPage:
                case PageIndex.ChildObjectSelectionPage:
                    lblPageSubTitle.Text = resourceManager.GetResource("QAWIZARD_lblPageSubTitle_ObjectSelection_Text");
                    break;
                case PageIndex.ParentObjectRecordSelectionPage:
                case PageIndex.ChildObjectRecordSelectionPage:
                    lblPageSubTitle.Text = resourceManager.GetResource("QAWIZARD_lblPageSubTitle_RecordSelection_Text");
                    break;
                case PageIndex.QuickAppSettingsPage:
                    lblPageSubTitle.Text = resourceManager.GetResource("QAWIZARD_lblPageSubTitle_AppSetting_Text");
                    break;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.DarkGray,e.ClipRectangle.Left,e.ClipRectangle.Top,e.ClipRectangle.Width - 1,e.ClipRectangle.Height - 1); base.OnPaint(e);
        }
    }

    class QuickAppEditModeData
    {
        public string EditingAppName { get; set; }
        public string EditingUniqueId { get; set; }
    }
}
