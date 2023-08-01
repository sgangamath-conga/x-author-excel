using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.AppDesigner.MenuDesigner.UI.Controller;
using Apttus.XAuthor.AppDesigner.Resources;
using Apttus.XAuthor.AppDesigner.DesignerUtils;

namespace Apttus.XAuthor.AppDesigner.MenuDesigner.UI.View
{
    public partial class ucMenuDesigner : UserControl, IMenuView
    {
        private MenuMediator TvwAdapter;
        MenuController controller;
        public ucMenuDesigner()
        {
            InitializeComponent();
            TvwAdapter = new MenuMediator(this.MnuTree, this.propertyGrid1, this.cmbMenuSelector, this.btnCreate, this.btnDelete);
            XlationUtil xl = new XlationUtil();
            xl.XlateAll(this);

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            controller.AddMenu((int)cmbMenuSelector.SelectedIndex);

        }
        public MenuMediator GetMediator()
        {
            return TvwAdapter;
        }
        public void SetController(MenuDesigner.UI.Controller.MenuController menucontroller)
        {
            controller = menucontroller;
            controller.MenuControlsMediator = TvwAdapter;

            this.cmbMenuSelector.DataSource = controller.MenuComboDataset();

        }

        public void AttachPropertyGrid(MenuDesigner.UI.Model.AbstractMenu menu)
        {
            this.propertyGrid1.SelectedObject = menu;
        }

        private void MnuTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            controller.OnSelectNode();
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
           
        }

       

        private void btnDelete_Click(object sender, EventArgs e)
        {
            controller.DeleteItem();
        }

        private void cmbMenuSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblProperties_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            controller.Save();
        }

        private void propertyGrid1_Click_1(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_PropertyValueChanged_1(object s, PropertyValueChangedEventArgs e)
        {
            controller.PropertyGridValChanged();
        }

        private void MnuTree_AfterSelect_1(object sender, TreeViewEventArgs e)
        {

        }

        private void cmbMenuSelector_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

       

       

    

        
    }
}
