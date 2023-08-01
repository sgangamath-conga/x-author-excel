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

namespace Apttus.XAuthor.AppDesigner
{
    public partial class VisibleCondition : Form
    {
        public VisibleCondition()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        public string Value
        {
            get { return this.txtCondition.Text; }
            set { this.txtCondition.Text = value; }
        }
    }
}
