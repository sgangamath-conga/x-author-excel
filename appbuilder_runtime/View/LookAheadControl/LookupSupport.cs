using Apttus.XAuthor.Core;
/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace LookAhead
{
    public partial class LookupSupport : Form
    {
        List<string> list = new List<string>();
        LookAheadSupport LA = new LookAheadSupport();
        LookAheadCallBack Targetobj;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public LookupSupport(LookAheadCallBack CallBackObj)
        {
            InitializeComponent();
            SetCultureData();

            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Apttus.XAuthor.Core.Constants.RUNTIME_PRODUCT_NAME;

            list.Clear();
            Targetobj = CallBackObj;
        }

        private void SetCultureData()
        {
            lblSearchFields.Text = resourceManager.GetResource("LOOKUPSUPPORT_lblSearchFields_Text");
            linkLabel1.Text = resourceManager.GetResource("LOOKUPSUPPORT_linkLabel1_Text");
            lnkClear.Text = resourceManager.GetResource("LOOKUPSUPPORT_lnkClear_Text");
            tabPage1.Text = resourceManager.GetResource("COMMONRUNTIME_LookUpSupport_Text");
            tabPage2.Text = resourceManager.GetResource("COMMONRUNTIME_LookUpSupport_Text");
        }
        public void DisplayUI()
        {
            

        }
        List<string> BackupData;
        public void PopulateMultiColData(System.Data.DataTable dt)
        {
            this.dgvFields.DataSource = dt;
            this.Width = 637;
            this.tabControl1.Width = 613;
            HideTabPage(this.tabControl1.TabPages[0]);

        }
        public List<string> DataIn
        {
            get { return list; }
            set
            {
                list = value;
                BackupData = list;
              //  listBox1.DataSource = list;
                                       
                //TODO : this could be a performance issue
                listBox1.BeginUpdate();
                if (listBox1.Items.Count > 0)
                {
                    listBox1.Items.Clear();
                }
                foreach (String str in list)
                {
                    listBox1.Items.Add(str);
                }
                listBox1.EndUpdate();
                //this.tabControl1.Width = 613;
                //this.Width = 637;
                HideTabPage(this.tabControl1.TabPages[1]);
            }
        }
        public string DataOut;
        public LookAheadSupport Supp
        {
            get;
            set;
        }
        public System.Windows.Forms.TextBox TxtSearch
        {
            get { return this.txtSearch; }
            set { ;}
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

           if (String.IsNullOrEmpty(txtSearch.Text.Trim()) == false)
            {


                listBox1.Items.Clear();
                foreach (string str in list)
                {
                    if (str.StartsWith(txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        listBox1.Items.Add(str);
                    }
                }

            }
            else if (txtSearch.Text.Trim() == "")
            {
                listBox1.Items.Clear();
                
                foreach (string str in list)
                {
                    listBox1.Items.Add(str);
                }
            }
        }
       
        public Range Target
        {
            get;
            set;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               // DataOut = listBox1.SelectedItem as string;
                Targetobj.GetCurrentSelection(listBox1.SelectedItem as string);
                //Supp.output(DataOut);
              //  Globals.ThisAddIn.LOut = DataOut;
                this.Close();
            }
            catch (Exception ex)
            {

            }
            //Target.Application.ActiveCell.Value2 = listBox1.SelectedItem as string;
          //  Target.Value2 = listBox1.SelectedItem as string;
          
        }

        private void LookupSupport_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            Supp.KeyPressEvent(sender, e);
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {

            Supp.KeyPressEvent(sender, e);
        }

        private void LookupSupport_Shown(object sender, EventArgs e)
        {
            Supp.MoveLookAheadWindow();
        }

        private void LookupSupport_Load(object sender, EventArgs e)
        {
            System.Action action = () => txtSearch.Focus();
            this.BeginInvoke(action);

            //Dispose the Dialog, if the user presses the ESC key.
            System.Windows.Forms.Button cancelBtn = new System.Windows.Forms.Button();
            cancelBtn.Size = new Size(0, 0);
            cancelBtn.TabStop = false;
            Controls.Add(cancelBtn);
            cancelBtn.Click += (object sender1, EventArgs e1) => { Close(); };

            //Set the cancelbutton property to dispose the dialog, when the user presses the ESC Key.
            this.CancelButton = cancelBtn;

            foreach (DataGridViewColumn column in dgvFields.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }   

        private void HideTabPage(TabPage tp)
        {
            if (tabControl1.TabPages.Contains(tp))
                tabControl1.TabPages.Remove(tp);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Supp.SearchValues(txtSearchFields.Text, this.dgvFields);
        }

        private void lnkClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtSearchFields.Text = "";
            Supp.ClearValues( this.dgvFields);
        }

        private void dgvFields_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Targetobj.GetCurrentSelectionFromMultiCol(e.RowIndex, Supp as LooAheadMultiColSupportHelper);
            this.Close();
        }

        private void txtSearchFields_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Supp.SearchValues(txtSearchFields.Text, this.dgvFields);
            }
        }
    }
}
