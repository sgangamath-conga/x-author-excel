/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner.MenuDesigner.UI.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Apttus.XAuthor.AppDesigner
{
    /* visible when condition = true / false */
    [Editor(typeof(VisibilityEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]

   public  class  Visibility
    {
        public override string ToString()
        {
            return "";
        }
        public string Condition
        {
            get;
            set;
        }
    }
    class VisibilityEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            Visibility visibilityObj = value as Visibility;
            if (svc != null && visibilityObj != null)
            {
                using (VisibleCondition form = new VisibleCondition())
                {
                    form.Value = visibilityObj.Condition;
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        visibilityObj.Condition = form.Value; // update object
                    } 
                   
                }

                
            }
            return value; // can also replace the wrapper object here
        }
    }
}
