/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Windows.Forms;


namespace Apttus.XAuthor.Core
{
    public class MultiLineCaptureController
    {
        private ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        private MultiLineCaptureView View;
        public ApttusObject AppObject { get; set; }
        public ApttusField AppField { get; set; }
        public string MultiLineValue { get; set; }

        public MultiLineCaptureController(MultiLineCaptureView view, ApttusObject currentAppObject, ApttusField currentAppField, string value)
        {
            this.View = view;
            this.AppObject = currentAppObject;
            this.AppField = currentAppField;
            this.MultiLineValue = value;
            view.SetController(this);
        }

        public bool Browse()
        {
            DialogResult dr = this.View.ShowDialog();
            // Return true if the user clicked Apply on Object and Field dialog
            if (dr == DialogResult.OK)
                return true;
            else
                return false;
        }

    }
}
