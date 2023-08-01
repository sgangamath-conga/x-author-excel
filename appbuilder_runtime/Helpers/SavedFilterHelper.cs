using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apttus.XAuthor.AppRuntime
{
    public class SavedFilterHelper
    {
        /// <summary>
        /// Get data for search filers from file.
        /// </summary>
        /// <returns></returns>
        public static SavedFilters GetSavedFilters()
        {
            SavedFilters sfSource;
            sfSource = ApttusXmlSerializerUtil.SerializeFromFile<SavedFilters>(FilterFile);
            if (sfSource == null)
                sfSource = new SavedFilters();
            return sfSource;
        }

        /// <summary>
        /// Save user defined filters in file.
        /// </summary>
        /// <param name="sfSource">Collection of saved filters.</param>
        /// <param name="filtersetName">Name of filter set. </param>
        /// <param name="actionID">Action ID which holds filter sets.</param>
        /// <param name="apttusDataSet">ApttusDataSet, holds key/value pair data.</param>
        public static void SaveFilters(SavedFilters sfSource, string filtersetName, string actionID, ApttusDataSet apttusDataSet)
        {
            // remove if filter set exist with same name in source
            sfSource.UserDefinedFilters.RemoveAll(x => x.Name == filtersetName && x.ActionID == actionID);

            // add filter set in source
            sfSource.UserDefinedFilters.Add(new SavedFilter() { Name = filtersetName, ActionID = actionID, Data = apttusDataSet });

            // save file
            ApttusXmlSerializerUtil.SerializeToFile<SavedFilters>(sfSource, FilterFile);
        }

        public static void RemoveFilter(SavedFilters sfSource, string filtersetName, string actionID)
        {
            sfSource.UserDefinedFilters.RemoveAll(x => x.Name == filtersetName && x.ActionID == actionID);

            // save file
            ApttusXmlSerializerUtil.SerializeToFile<SavedFilters>(sfSource, FilterFile);
        }

        /// <summary>
        /// FilterFile refers to file which hold filter information. 
        /// </summary>
        public static string FilterFile
        {
            get
            {
                string filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Apttus\\SavedFilters";

                EnsurePathExists(filepath);

                filepath += @"\" + ConfigurationManager.GetInstance.Definition.UniqueId.ToString() + ".xml";

                return filepath;
            }
        }

        /// <summary>
        /// Used to ensure that directory exists, if not then create it.
        /// </summary>
        /// <param name="path"></param>
        public static void EnsurePathExists(string path)
        {
            try
            {
                // If the directory doesn't exist, create it.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
