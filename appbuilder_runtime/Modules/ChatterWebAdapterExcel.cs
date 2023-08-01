/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.ChatterWebControl.Modules;
using Apttus.ChatterWebControl.Interfaces;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppRuntime.Modules
{
    class ChatterWebAdapterExcel : IChatterWebAdapter
    {
        public enum PublisherOption { none, selected, worksheet, workbook };

        private PublisherOption selectedOption = PublisherOption.none;
        private ChatterWebBrowserManager browserManager;

        public void SetManager(ChatterWebBrowserManager manager)
        {
            browserManager = manager;
        }

        public void Submit(string text)
        {
            var scriptcall = browserManager.getContextId();
            scriptcall.OnDone(() =>
            {

                if (scriptcall.Exception != null)
                {
                    MessageBox.Show("Error submitting to Chatter\n" + scriptcall.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string contextId = scriptcall.Result.ToString();
                switch (selectedOption)
                {
                    case PublisherOption.none:
                        // Rest XML issue
                        if (String.IsNullOrEmpty(text))
                            return;
                        ApttusXAuthorForChatterManager.ShareCommentsWithChatter(contextId, text, browserManager);
                        break;
                    case PublisherOption.selected:
                        ApttusXAuthorForChatterManager.ShareHighlightedContentWithChatter(contextId, text, browserManager);
                        break;
                    case PublisherOption.worksheet:
                        ApttusXAuthorForChatterManager.ShareActiveWorksheetWithChatter(contextId, text, browserManager);
                        break;
                    case PublisherOption.workbook:
                        ApttusXAuthorForChatterManager.ShareCurrentDocumentWithChatter(contextId, text, browserManager);
                        break;
                }
            });
        }

        public void SetSelectedPublisherOption(string optionStr)
        {
            PublisherOption option;

            if (Enum.TryParse<PublisherOption>(optionStr, out option))
                selectedOption = option;
        }
    }
}
