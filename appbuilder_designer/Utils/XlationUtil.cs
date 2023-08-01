/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppDesigner
{
     class XlationUtil
    {
         List<Control> ControlList;
         public  XlationUtil()
         {
             ControlList = new List<Control>();
         }
       

          IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

          
        public void GetAllControls(Control container)
        {
            
            foreach (Control c in container.Controls)
            {
                GetAllControls(c);
                if (c.Tag != null)
                ControlList.Add(c);
            }
        }

        public void XlateAll(Control c)
        {
            GetAllControls(c);
            foreach (Control ctrl in ControlList)
            {
                ctrl.Text = ctrl.Text;
            }
            if (c.Tag!= null)
                c.Text = c.Text;
        }
    }
      
    
}
