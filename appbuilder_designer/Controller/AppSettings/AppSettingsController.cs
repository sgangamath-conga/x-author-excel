/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner
{
    public class AppSettingsController 
    {
        AppSettingView View;
        AppSettings Model;
        FormOpenMode FormOpenMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="view"></param>
        /// <param name="formOpenMode"></param>
        public AppSettingsController(AppSettings model, AppSettingView view, FormOpenMode formOpenMode)
        {
            View = view;
            Model = model;
            FormOpenMode = formOpenMode;
            if (View != null)
            {
                View.SetController(this);
                View.ShowDialog();
            }
        }
        /// <summary>
        /// Save Method
        /// </summary>
        /// <param name="Model"></param>
        internal bool Save(AppSettings Model)
        {
            try
            {
                if (Model != null)
                    ConfigurationManager.GetInstance.SaveAppSettings(Model);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
        }
    }
}
