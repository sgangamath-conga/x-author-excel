using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppDesigner
{
    internal class DataMigrationMigrateSheetController
    {
        int IndexCount = 1;
        Dictionary<Guid, string> names = new Dictionary<Guid, string>();
        private static DataMigrationMigrateSheetController controller;

        public static DataMigrationMigrateSheetController GetInstance
        {
            get
            {
                if (controller == null)
                    controller = new DataMigrationMigrateSheetController();
                return controller;
            }
        }        

        private DataMigrationMigrateSheetController()
        {
            //names.Add("Product", "Product");
            //names.Add("Category", "Category");
            //names.Add("Product Classification", "ProdClass");
            //names.Add("Product Option Group", "ProdOptionGroup");
            //names.Add("Cat Hiarchy L0", "CatL0");
            //names.Add("Cat Hiarchy L1", "CatL1");
            //names.Add("Cat Hiarchy L2", "CatL2");
            //names.Add("Cat Hiarchy L3", "CatL3");
            //names.Add("Product Option Component", "ProdOptionComp");
            //names.Add("Product Group", "ProdGroup");
            //names.Add("Product Group Member", "ProdGroupMember");
            //names.Add("Product Att Group", "ProdAttGroup");
            //names.Add("Product Att", "ProdAtt");
            //names.Add("Product Att Group Member", "ProdAttGroupMember");
        }

        public string this[Guid migrationObjectUniqueId] 
        {
            get
            {
                if(names.ContainsKey(migrationObjectUniqueId))
                {
                    return names[migrationObjectUniqueId];
                }
                return string.Empty;
            }
        }

        public string[] AllNames
        {
            get
            {
                return names.Values.ToArray();
            }
        }

        public void ResetNames()
        {
            names.Clear();
        }

        public string CreateName(string sheetName, Guid migrationObjectUniqueId)
        {
            string name = GetName(sheetName, migrationObjectUniqueId);
            if (names.ContainsValue(name))
            {
                names[migrationObjectUniqueId] = name;
            }
            else
                names.Add(migrationObjectUniqueId, name);

            return name;
        }
        public String GetName(string sheetName, Guid migrationObjectUniqueId)
        {
            sheetName = Regex.Replace(sheetName, "[^a-zA-Z0-9_.]+", "_", RegexOptions.Compiled);

            int nameLength = sheetName.Length;
            int maxlength = nameLength > 30 ? 30 : nameLength;
            string name = sheetName.Substring(0, maxlength);
            if (names.ContainsValue(name))
            {
                name = name + "_" + IndexCount;
                ++IndexCount;
            }

            return name;
        }

    }
}
