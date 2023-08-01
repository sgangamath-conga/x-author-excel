using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner.Forms.CrossTab
{
    public class CrossTabAppdefController
    {
        AppDefCommon View;

       
        private AppDefinitionCommonModel model;
        public CrossTabAppdefController(AppDefCommon view)
        {
            model = new CrossTabAppDefModel();
            View = view;
            View.SetController(this);
           
        }
        public void RegisterControls(ComboBox colHeader,
            ComboBox rowHeader, ComboBox TopLevel)
        {
            //this.cmbocolHeader, this.cmboRowHeader, this.cmbotopLevel

            List<ApttusObject> ApttusObjects = model.GetAllStandardObjects();

            // Bind ComboBox control
            TopLevel.DisplayMember = Constants.NAME_ATTRIBUTE;
            TopLevel.ValueMember = Constants.ID_ATTRIBUTE;
            TopLevel.DataSource = ApttusObjects;
        }
    }



}
