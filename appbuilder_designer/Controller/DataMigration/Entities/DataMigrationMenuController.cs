using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationMenuController
    {
        private DataMigrationModel Model;
        private ModelMenu Menu;
        private ConfigurationManager configManager = ConfigurationManager.GetInstance;

        internal DataMigrationMenuController(DataMigrationModel model)
        {
            Model = model;
        }

        internal void BuildMenu()
        {
            Menu = new ModelMenu();

            BuildAppInfoMenu();
            BuildSourceOrgMenu();
            BuildTargetOrgMenu();
        }

        void BuildAppInfoMenu()
        {
            MenuGroup menuGroup = Menu.AddMenu();
            menuGroup.Name = "Information";

            AddMenuItem(Menu, menuGroup, DataMigrationConstants.AppInfoIcon, DataMigrationConstants.AppInfoActionFlowName, Model.MigrationActionFlows.AppInfoActionFlowId);

            Menu.SaveMenu(true, true, false, false);
        }

        void BuildSourceOrgMenu()
        {
            MenuGroup menuGroup = Menu.AddMenu();
            menuGroup.Name = "Source Org";

            if (string.IsNullOrEmpty(Model.ExportAllSuffix))
                Model.ExportAllSuffix = "All";
            if (string.IsNullOrEmpty(Model.ExportSelectiveSuffix))
                Model.ExportSelectiveSuffix = "Selective";

            string retrieveSelective = string.Format(DataMigrationConstants.ExportMenuButtonName, Model.ExportSelectiveSuffix);
            string retrieveAll = string.Format(DataMigrationConstants.ExportMenuButtonName, Model.ExportAllSuffix);

            AddMenuItem(Menu, menuGroup, DataMigrationConstants.RetrieveMenuIcon, retrieveSelective, Model.MigrationActionFlows.SelectiveActionFlowId);
            AddMenuItem(Menu, menuGroup, DataMigrationConstants.RetrieveMenuIcon_ALL, retrieveAll, Model.MigrationActionFlows.AllActionFlowId);
            AddMenuItem(Menu, menuGroup, DataMigrationConstants.SaveSourceDataActionFlowName_MenuIcon, DataMigrationConstants.SaveSourceDataActionFlowName, Model.MigrationActionFlows.SaveSourceDataActionFlowId);

            Menu.SaveMenu(true, true, false, false);
        }

        void BuildTargetOrgMenu()
        {
            MenuGroup menuGroup = Menu.AddMenu();
            menuGroup.Name = "Target Org";
             
            AddMenuItem(Menu, menuGroup, DataMigrationConstants.SaveMenuIcon, DataMigrationConstants.SaveActionFlowName, Model.MigrationActionFlows.SaveActionFlowId);

            Menu.SaveMenu(true, true, false, false);
        }

        void AddMenuItem(ModelMenu Menu, MenuGroup menuGroup, string Icon, string Name, Guid workFlowId)
        {
            MenuItem menuItem = Menu.AddMenuItem(menuGroup);
            menuItem.Icon = Icon;
            menuItem.Name = Name;
            menuItem.WorkFlowID = configManager.GetWorkflowById(workFlowId).Id;
        }

        public void UpdateMenu()
        {
            try
            {
                string retrieveSelective = string.Format(DataMigrationConstants.ExportMenuButtonName, Model.ExportSelectiveSuffix);
                string retrieveAll = string.Format(DataMigrationConstants.ExportMenuButtonName, Model.ExportAllSuffix);

                Menus menu = configManager.Menus;
                List<MenuGroup> menuGroups = menu.MenuGroupsCollection;
                List<MenuItem> menuItems = menuGroups[1].MenuItems;
                menuItems[0].Name = retrieveSelective;
                menuItems[1].Name = retrieveAll;
            }
            catch(Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }
    }
}
