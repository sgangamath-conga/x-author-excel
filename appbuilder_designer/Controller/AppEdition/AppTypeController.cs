using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner
{
   public  class AppTypeController
    {      
       public string AppName
       {
           get;
           set;
       }
       public bool NewOrExisting
       {
           get;
           set;
       }
       public string FilePath
       {
           get;
           set;
       }
       public ApplicationInstance AppInst
       {
           get;
           set;
       }
       
       public List<LicenseEditionStruct> EditionsUI
       {
           get;
           set;
       }
       public LicenseEditionRuleStruct Editionrule
       {
           get;
           set;
       }
      
       public AppTypeController(string appName, string filePath, bool neworExisting)
       {
           AppName = appName;
           FilePath = filePath;
           NewOrExisting = neworExisting;
           init();
       }

       private void init()
       {
           
           Editionrule = LicenseActivationManager.GetInstance.GetEditionRules();
           EditionsUI = LicenseActivationManager.GetInstance.GetEditionsForUI();
       }
      
       //public bool ChangeAppType(string Edition)
       //{



       //     //check if the app is activated
       //    // if so warn user
       //    string currentEdition = null;
       //    ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
       //    if (AppInst != null)
       //    {
       //        // first check if the chage edition type is allowed or not
       //        // backward compatibility, old apps may not have eidtion type
       //        if (AppInst.App.Definition.EditionType != null)
       //        {
       //            currentEdition = AppInst.App.Definition.EditionType;
       //            if (!string.IsNullOrEmpty(currentEdition))
       //            { // check the rules , only forward is allowed , no backwards
       //                if (Editionrule.EditionRuleUI.Count > 0)
       //                {
       //                    try
       //                    {
       //                        List<string> RulesForEdition = Editionrule.EditionRuleUI[currentEdition];
       //                        if (RulesForEdition != null && RulesForEdition.Count >0)
       //                        {
       //                           string result =  RulesForEdition.Where(x => x.Contains(Edition)).FirstOrDefault();
       //                            if (result == null ) // invalid
       //                            {
       //                                string errorMessage = string.Format(ApttusResourceManager.GetInstance.GetResource("CHANGEAPPTYPE_EDITION_VIOLATION"), currentEdition, Edition);
       //                                ApttusMessageUtil.ShowInfo(errorMessage, resourceManager.GetResource("CHANGE_EDITION_CAPTION"));
       //                                return false;
                                       
       //                            }
       //                        }
       //                    }
       //                    catch (Exception ex)
       //                    {
       //                        ApttusMessageUtil.ShowInfo(ex.Message.ToString(), resourceManager.GetResource("CHANGE_EDITION_CAPTION"));
       //                    }
       //                }

       //            }
       //        }
               


       //        SalesforceApplicationController AppController = new SalesforceApplicationController();
       //        if (AppController.IsAppActivated(AppInst.App.Definition.UniqueId))
       //        {
       //            // if activated, ask the user for confirmation
       //            if (ApttusMessageUtil.ShowWarning(resourceManager.GetResource("CHANGE_APPTYPE_WARNING"), resourceManager.GetResource("CHANGE_APPTYPE_TITLE"), ApttusMessageUtil.YesNo) == TaskDialogResult.Yes)
       //            {
       //                // Overwrite existing app
       //                AppInst.App.Definition.EditionType = Edition;
       //                AppController.DeactiVateApp(AppInst.App.Definition.UniqueId, false);
       //                // inactivate app
       //            }
       //            else
       //                return false;

       //        }
       //        else // if it not activated, no issue
       //            AppInst.App.Definition.EditionType = Edition;

       //        if (!string.IsNullOrEmpty(Edition))
       //        {
       //            Constants.PRODUCT_EDITION = Edition;
       //            ApttusCommandBarManager.GetInstance().EnableMenus();
       //            Globals.ThisAddIn.Application.StatusBar = Constants.PRODUCT_EDITION;
       //            Globals.ThisAddIn.AppDesignerVersion = Constants.PRODUCT_EDITION; 
       //        }
            
       //    }
       //    return false;
       //}
    }
}
