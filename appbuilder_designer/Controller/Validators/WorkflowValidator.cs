/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class WorkflowValidator : IValidator
    {
        private static WorkflowValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static WorkflowValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new WorkflowValidator();
                        configManager = ConfigurationManager.GetInstance;
                    }
                }
                return instance;
            }
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            Workflow workflow = t as Workflow;

            ValidationResult result = new ValidationResult();
            result.Success = true;
            result.EntityType = EntityType.Workflow;
            result.EntityName = workflow.Name;

            if (ConfigurationManager.GetInstance.Menus == null)
                return results;

            List<Core.MenuGroup> lstMenuGroup = ConfigurationManager.GetInstance.Menus.MenuGroupsCollection;
            if (lstMenuGroup.Count == 0)
                return results;

            // Fetch all menu item from group
            List<Core.MenuItem> lstMenuItem = (from menuGroup in lstMenuGroup
                          from menuitem in menuGroup.MenuItems
                          where menuitem.WorkFlowID.Equals(workflow.Id)
                          select menuitem).ToList();

            
            if (lstMenuItem.Count == 0)
                return results;

            //There exists a save action which uses this save map.
            //Build Error Message and let the user know which save action(s) are using this SaveMap.

            //Set the success to false
            result.Success = false;

            //Build & Set the Error Message
            StringBuilder errorMsgBuilder = new StringBuilder(workflow.Name);
            errorMsgBuilder.Append(resourceManager.GetResource("WORKFLOWVALID_ValidateDeleteUser_ErrMsg"));

            bool bAddComma = false;

            foreach (Core.MenuItem menuItem in lstMenuItem)
            {
                if (bAddComma)
                    errorMsgBuilder.Append(",");
                errorMsgBuilder.Append(menuItem.Name);
                bAddComma = true;
            }
            errorMsgBuilder.Append(resourceManager.GetResource("COMMON_HenceCanNot_Msg"));

            result.Messages = new List<string>();
            result.Messages.Add(errorMsgBuilder.ToString());

            results.Add(result);

            return results;
        }

        public List<ValidationResult> Validate<T>(T t)
        {
            throw new NotImplementedException();
        }

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
