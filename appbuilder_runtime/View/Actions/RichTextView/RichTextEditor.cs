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

namespace Apttus.XAuthor.AppRuntime
{
    public partial class RichTextEditor : UserControl
    {
        private WebBrowserControl.WebBrowserUserControl browser;
        private Dictionary<string, string> pageParams = new Dictionary<string, string>();
        private RichTextDataManager richTextDataManager = RichTextDataManager.Instance;
        private XAuthorSalesforceLogin oAuthWrapper = Globals.ThisAddIn.GetLoginObject() as XAuthorSalesforceLogin;
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public RichTextEditor()
        {
            InitializeComponent();
        }

        public void LoadRichTextPage(string objectName, string fieldID, string recID, bool readOnly,bool isSaveOtherField)
        {
            pageParams.Clear();

            pageParams.Add("ObjectName", objectName);
            pageParams.Add("FieldName", fieldID);
            pageParams.Add("RecID", recID);
            pageParams.Add("ReadOnly", readOnly.ToString());
            pageParams.Add("SaveOtherField", isSaveOtherField.ToString()); //For Future use of richtext datatype as saveother field.

            browser.ProcessRequest(Constants.NAMESPACE_PREFIX + Constants.RICH_TEXT_VF_PAGE_NAME, pageParams);
        }

        private void RichTextEditor_Load(object sender, EventArgs e)
        {
            browser = new WebBrowserControl.WebBrowserUserControl(Constants.RUNTIME_PRODUCT_NAME, oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
            browser.Dock = DockStyle.Fill;
            this.Controls.Add(browser);

            browser.ResponseRecieved += browser_ResponseRecieved;
        }

        private void browser_ResponseRecieved(string responseHtml)
        {
            if (richTextDataManager.Save(responseHtml)) {
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("RichTextEditor_browser_ResponseRecieved"), resourceManager.GetResource("RichTextEditor_browser_ResponseRecieved_CAP"));
            };
        }
    }
}
