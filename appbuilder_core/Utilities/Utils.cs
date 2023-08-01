/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Apttus.XAuthor.Core
{
    public class Utils
    {
        static Mutex mut = new Mutex();
        //keep track of temporary files
        private static ArrayList gTempFiles = new ArrayList();
        // for managing template file

        static List<string> tempFileNames = new List<string>();
        static List<string> tempDirNames = new List<string>();
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public static CultureInfo GetLatestCulture
        {
            get
            {
                CultureInfo result = null;

                Thread.CurrentThread.CurrentCulture.ClearCachedData();
                Thread oThread = new Thread(new ThreadStart(DummyMethod));
                result = oThread.CurrentCulture;
                oThread.Abort();
                oThread = null;

                return result;
            }
        }

        private static void DummyMethod()
        {
            // Do Nothing in this method. This method is added to fetch 
        }

        /// <summary>
        /// Adds a temporary file to a list so we can delete it later
        /// </summary>
        /// <param name="fname"></param>
        public static void AddTempFile(string fname)
        {
            try
            {
                gTempFiles.Add(fname);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Deletes temporary files
        /// </summary>
        public static void DeleteTempFiles()
        {
            foreach (string filename in gTempFiles)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void ClearTempFiles()
        {
            try
            {
                gTempFiles.Clear();
            }
            catch (Exception)
            {
            }
        }

        public static bool DeleteFile(string filename)
        {
            bool result = false;

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static bool DeleteDirectory(string folderPath)
        {
            bool result = false;

            try
            {
                if (Directory.Exists(folderPath))
                {
                    // Delete subfolder
                    Directory.Delete(folderPath, true);
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationTempDirectory(bool blnAppAttachments = false)
        {
            mut.WaitOne();
            string sPath = Path.GetTempPath();
            if (!sPath.EndsWith("Temp"))
            {
                if (blnAppAttachments)
                    sPath = Path.Combine(sPath, Constants.XATTACHMENTPATH + Path.DirectorySeparatorChar.ToString());
                else
                    sPath = Path.Combine(sPath, Constants.PRODUCT_NAME + Path.DirectorySeparatorChar.ToString());

                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
            }
            mut.ReleaseMutex();
            return sPath;
        }


        /// <summary>
        /// Gets the full path to the file in specified subdir of temp directory.
        /// Creates directory in temp directory if it does not exists
        /// </summary>
        /// <param name="dir">subdirectory in temp directory</param>
        /// <param name="fname">file name, include suffix</param>
        /// <returns>full path to the file</returns>
        public static string GetTempFileName(string dir, string fname, bool blnAppAttachments = false, bool blnCreateDirectory = true)
        {
            mut.WaitOne();
            string sTempDir = string.Empty;

            if (blnAppAttachments)
                sTempDir = GetApplicationTempDirectory(blnAppAttachments);
            else
                sTempDir = GetApplicationTempDirectory();

            string sPath = Path.Combine(sTempDir, dir);
            if (blnCreateDirectory && !Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
                tempDirNames.Add(sPath);
            }

            sPath = Path.Combine(sPath, fname);
            tempFileNames.Add(sPath);
            mut.ReleaseMutex();
            return sPath;
        }

        internal static string TimeAgo(DateTime lastUpdated)
        {
            string result = string.Empty;
            TimeSpan diff = DateTime.Now.Subtract(lastUpdated);
            if (diff <= TimeSpan.FromSeconds(60))
            {
                result = "a moments ago";
            }
            else if (diff <= TimeSpan.FromMinutes(60))
            {
                result = diff.Minutes > 1 ?
                    String.Format("about {0} minutes ago", diff.Minutes) :
                    "about a minute ago";
            }
            else if (diff <= TimeSpan.FromHours(24))
            {
                result = diff.Hours > 1 ?
                    String.Format("about {0} hours ago", diff.Hours) :
                    "about an hour ago";
            }
            else if (diff <= TimeSpan.FromDays(30))
            {
                result = diff.Days > 1 ?
                    String.Format("about {0} days ago", diff.Days) :
                    "yesterday";
            }
            else if (diff <= TimeSpan.FromDays(365))
            {
                result = diff.Days > 30 ?
                    String.Format("about {0} months ago", diff.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = diff.Days > 365 ?
                    String.Format("about {0} years ago", diff.Days / 365) :
                    "about a year ago";
            }
            return result;
        }

        public static string FileToBase64String(string sFilename)
        {
            string result = string.Empty;
            byte[] bytes = FileToBytes(sFilename);
            result = Convert.ToBase64String(bytes, 0, bytes.Length);
            return result;
        }

        /// <summary>
        /// Reads a file and returns the content in a byte array.
        /// </summary>
        /// <param name="sFilename">File name</param>
        /// <returns>File content</returns>
        public static byte[] FileToBytes(string sFilename)
        {
            FileStream fs = new FileStream(sFilename, FileMode.Open, FileAccess.Read);
            long bytelength = fs.Length;
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Flush();
            fs.Dispose();
            fs.Close();

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baStream"></param>
        /// <param name="sFilename"></param>
        public static void StreamToXlsx(byte[] baStream, string sFilename)
        {
            using (FileStream fs = new FileStream(sFilename, FileMode.Create, FileAccess.Write))
            {

                byte[] bytearray = new byte[16 * 1024];

                bytearray = baStream;

                fs.Write(bytearray, 0, bytearray.Length);
                fs.Close();
                fs.Dispose();
            }
        }

        internal static int GetOpenExcelCounts()
        {
            int count = 0;
            foreach (Process theprocess in Process.GetProcesses())
            {
                if (theprocess.ProcessName == "EXCEL")
                {
                    count++;
                }
            }
            return count - 1;
        }

        public static Version GetExcelVersion()
        {
            var process = Process.GetProcessesByName("excel").First();
            string fileVersionInfo = process.MainModule.FileVersionInfo.FileVersion;
            var version = new Version(fileVersionInfo);
            return version;
        }
        /// <summary>
        /// Writes the content of a byte array to a file with the given name.
        /// </summary>
        /// <param name="baStream">Binary content</param>
        /// <param name="sFilename">Name of the output file</param>
        public static void StreamToFile(byte[] baStream, string sFilename, bool removeExisting = false)
        {
            if (removeExisting)
            {
                if (File.Exists(sFilename))
                    File.Delete(sFilename);
            }
            FileStream fs = new FileStream(sFilename, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(baStream);
            w.Close();
            fs.Close();
        }

        /// <summary>
        /// Get Application Default Path
        /// </summary>
        /// <param name="sAppID"></param>
        /// <returns></returns>
        public static string GetApplicationPath(string sAppID)
        {
            mut.WaitOne();
            string sTempDir = GetApplicationTempDirectory();
            string sPath = Path.Combine(sTempDir, sAppID);
            mut.ReleaseMutex();
            return sPath;
        }

        public static void CleanupXAuthorFiles()
        {
            try
            {
# if !DEBUG // Preserve X-Author files in DEBUG Mode
                string directoryPath = GetApplicationTempDirectory();
                StringBuilder deleteFailures = ClearDirectory(directoryPath);
                // Do log only if there was delete failures
                if (!string.IsNullOrEmpty(deleteFailures.ToString()))
                    ExceptionLogHelper.InfoLog("Following items didn't get deleted:" + Environment.NewLine + deleteFailures.ToString());
#endif
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }


        public static void CreateOrClearDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                ClearDirectory(directoryPath);
            }
            else
                Directory.CreateDirectory(directoryPath);
        }

        public static StringBuilder ClearDirectory(string directoryPath)
        {
            StringBuilder failures = new StringBuilder();

            if (Directory.Exists(directoryPath))
            {
                // Delete all files and folder
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException)
                    {
                        failures.AppendLine("File: " + file.Name);
                    }
                }
                var childDirectories = (from dir in di.GetDirectories()
                                        where !(dir.Name.Equals(Constants.DESIGNER_LOG_NAME)
                                        || dir.Name.Equals(Constants.RUNTIME_LOG_NAME)
                                        || dir.Name.Equals(Constants.SERVICE_LOG_NAME)
                                        || dir.Name.Equals(Constants.XAUTHORBATCH_LOG_NAME)
                                        || dir.Name.Equals(Constants.PASTESOURCEDATA_READY_FOLDER)
                                        || dir.Name.Equals(Constants.PASTESOURCEDATA_COMPLETE_FOLDER))
                                        select dir);

                foreach (var dir in childDirectories)
                    try
                    {
                        dir.Delete(true);
                    }
                    catch (IOException)
                    {
                        failures.AppendLine("Dir : " + dir.Name);
                    }
            }
            return failures;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        public static void WriteStringToFile(string filePath, string lines)
        {
            File.WriteAllText(filePath, lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadStringFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Get's the Description attribute for the Enum object
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="defaultDescription"></param>
        /// <returns></returns>
        public static string GetEnumDescription(object enumValue, string defaultDescription)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defaultDescription;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetObjectTypeDescription()
        {
            Dictionary<string, string> listDesc = new Dictionary<string, string>();

            foreach (ObjectType val in Enum.GetValues(typeof(ObjectType)))
            {
                listDesc.Add(val.ToString(), GetEnumDescription(val, string.Empty));
            }
            return listDesc;
        }

        /// <summary>
        /// Get extension from folder
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetExtensionsfromDirectory(string filePath)
        {
            string fileExtension = string.Empty;
            string[] fileExtensions = Directory.GetFiles(filePath).Select(p => Path.GetExtension(p)).Distinct().OrderBy(p => p).ToArray();

            foreach (string extension in fileExtensions)
            {
                switch (extension)
                {
                    case Constants.XLSX:
                        fileExtension = extension;
                        break;
                    case Constants.XLSM:
                        fileExtension = extension;
                        break;
                    case Constants.XLS:
                        fileExtension = extension;
                        break;
                    default:
                        break;
                }
            }

            return fileExtension;
        }

        public static object[,] DataTableToArray(DataTable dt)
        {
            object[,] ObjectData = new object[dt.Rows.Count, dt.Columns.Count];

            for (int RowIndex = 0; RowIndex < dt.Rows.Count; RowIndex++)
                for (int ColumnIndex = 0; ColumnIndex < dt.Columns.Count; ColumnIndex++)
                    ObjectData[RowIndex, ColumnIndex] = dt.Rows[RowIndex][ColumnIndex];

            return ObjectData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appMode"></param>
        public static void AddApplicationInstance(ApplicationMode appMode, string appFileName, bool isOffline = false, bool isQuickApp = false)
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            DataManager dataManager = DataManager.GetInstance;
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

            // Add to Application instance
            if (configManager.Application != null)
            {
                ApplicationInstance appInstance = new ApplicationInstance();
                appInstance.UniqueId = configManager.Application.Definition.UniqueId;
                appInstance.AppFileName = appFileName;
                appInstance.App = configManager.Application;
                appInstance.AppMode = appMode;

                if (isOffline)
                {
                    appInstance.AppData = dataManager.AppData;
                    appInstance.CrossTabData = dataManager.CrossTabData;
                    appInstance.AppDataTracker = dataManager.AppDataTracker;
                    appInstance.DataProtection = dataManager.DataProtection;
                    appInstance.PicklistTracker = dataManager.PicklistTracker;
                    appInstance.RefreshedObjects = FieldLevelSecurityManager.Instance.RefreshedObjectsList;
                    appInstance.ApttusMatrixDataSet = dataManager.MatrixDataSets;
                    appInstance.RepeatingGroupRangesPerWorkSheet = new Dictionary<object, List<string>>();

                }
                else
                {
                    appInstance.AppData = new List<ApttusDataSet>();
                    appInstance.CrossTabData = new List<ApttusCrossTabDataSet>();
                    appInstance.AppDataTracker = new List<ApttusDataTracker>();
                    appInstance.DataProtection = new List<DataProtectionModel>();
                    appInstance.PicklistTracker = new List<PicklistTrackerEntry>();
                    appInstance.RefreshedObjects = new List<Guid>();
                    appInstance.ApttusMatrixDataSet = new List<ApttusMatrixDataSet>();
                    appInstance.RepeatingGroupRangesPerWorkSheet = new Dictionary<Object, List<string>>();
                }

                dataManager.AppData = appInstance.AppData;
                dataManager.CrossTabData = appInstance.CrossTabData;
                dataManager.AppDataTracker = appInstance.AppDataTracker;
                dataManager.DataProtection = appInstance.DataProtection;
                dataManager.PicklistTracker = appInstance.PicklistTracker;
                dataManager.MatrixDataSets = appInstance.ApttusMatrixDataSet;

                FieldLevelSecurityManager.Instance.RefreshedObjectsList = appInstance.RefreshedObjects;

                //In Quick App we create the appDefManager object on the fly and once the user clicks the finish button, the appDefManager is already containing objects,
                //hence we cannot clear AppObjects within appDefManager. Thus if it is a quickapp don't clear or set the AppObjects, otherwise do the default.
                if (!isQuickApp)
                    appDefManager.AppObjects = configManager.Application.Definition.AppObjects;

                appDefManager.CrossTabDefinitions = configManager.Application.Definition.CrossTabs;

                ApplicationManager.GetInstance.Add(appInstance, appFileName, appMode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        /// <param name="appFileName"></param>
        /// <param name="appMode"></param>
        public static void RemoveApplicationInstance(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            ApplicationManager.GetInstance.Remove(appUniqueId, appFileName, appMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        /// <param name="appFileName"></param>
        /// <param name="appMode"></param>
        public static ApplicationInstance GetApplicationInstance(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            return ApplicationManager.GetInstance.Get(appUniqueId, appFileName, appMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appUniqueId"></param>
        /// <param name="appFileName"></param>
        /// <param name="appMode"></param>
        /// <returns></returns>
        public static bool ExistsApplicationInstance(string appUniqueId, string appFileName, ApplicationMode appMode)
        {
            return ApplicationManager.GetInstance.Exists(appUniqueId, appFileName, appMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appInstance"></param>
        public static void SetApplicationInstance(ApplicationInstance appInstance)
        {
            ConfigurationManager configManager = ConfigurationManager.GetInstance;
            DataManager dataManager = DataManager.GetInstance;
            ApplicationDefinitionManager appDefManager = ApplicationDefinitionManager.GetInstance;

            // Set Application objects to respective classses
            if (appInstance != null)
            {
                configManager.Application = appInstance.App;
                dataManager.AppData = appInstance.AppData;
                dataManager.CrossTabData = appInstance.CrossTabData;
                dataManager.AppDataTracker = appInstance.AppDataTracker;
                dataManager.DataProtection = appInstance.DataProtection;
                dataManager.PicklistTracker = appInstance.PicklistTracker;
                dataManager.MatrixDataSets = appInstance.ApttusMatrixDataSet;
                FieldLevelSecurityManager.Instance.RefreshedObjectsList = appInstance.RefreshedObjects;
                appDefManager.AppObjects = configManager.Application.Definition.AppObjects;
                appDefManager.CrossTabDefinitions = configManager.Application.Definition.CrossTabs;
            }
        }

        public static string TimeSpantoString(TimeSpan ts)
        {
            string result = string.Empty;
            if (ts != null)
                result = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            return result;
        }

        /// <summary>
        /// Convert list to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ConvertListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static bool IsValidSOQL(string soql)
        {
            bool result = false;
            if (soql.Length <= Constants.SOQL_MAX_LENGTH)
                result = true;
            return result;
        }

        public static bool IsValidFormula(string formula, string formulaType)
        {
            bool isValid = false;
            double doubleresult;
            bool boolresult;
            DateTime dateresult;
            if (!string.IsNullOrEmpty(formulaType))
            {
                switch (formulaType)
                {
                    case Constants.FORMULATYPEDOUBLE:
                    case Constants.FORMULATYPECURRENCY:
                    case Constants.FORMULATYPEPERCENT:
                        isValid = Double.TryParse(formula, out doubleresult);
                        break;
                    case Constants.FORMULATYPEBOOLEAN:
                        isValid = Boolean.TryParse(formula, out boolresult);
                        break;
                    case Constants.FORMULATYPEDATE:
                        isValid = DateTime.TryParse(formula, out dateresult);
                        break;
                    case Constants.FORMULATYPESTRING:
                        isValid = true;
                        break;
                    default:
                        break;
                }
            }
            return isValid;
        }
        public static string GetDefaultAndNullValues(Datatype dataType, string Operator)
        {
            string result = string.Empty;

            switch (dataType)
            {
                case Datatype.String:
                case Datatype.Email:
                case Datatype.Picklist:
                case Datatype.Picklist_MultiSelect:
                    if (Operator.Equals("like #FILTERVALUE%") || Operator.Equals("like %#FILTERVALUE%") || Operator.Equals("like %#NOTFILTERVALUE%"))
                        result = Constants.QUOTE + Constants.PERCENT + Constants.PERCENT + Constants.QUOTE; // '%%' is the default for LIKE
                    else if (Operator.Equals("not in") || Operator.Equals("in"))
                        result = "(NULL)";
                    else
                        result = "NULL";
                    break;
                case Datatype.Lookup:
                case Datatype.Double:
                case Datatype.Decimal:
                case Datatype.Date:
                case Datatype.DateTime:
                    if (Operator.Equals("not in") || Operator.Equals("in"))
                        result = "(NULL)";
                    else
                        result = "NULL";
                    break;
                case Datatype.Boolean:
                    result = "false";
                    break;
            }

            return result;
        }

        public static string GetValidValue(string value, Datatype dataType, bool appendQuote, StringOperations stringOperation, out bool invalidValue)
        {
            StringBuilder sbResult = new StringBuilder();
            invalidValue = false;
            ObjectManager objectManager = ObjectManager.GetInstance;

            sbResult.Append(appendQuote ? Constants.QUOTE : string.Empty);

            switch (dataType)
            {
                case Datatype.Lookup:
                    {
                        string result = objectManager.EscapeQueryString(value);
                        //Lookup Id cannot be empty.
                        if (!string.IsNullOrEmpty(result))
                        {
                            result = (stringOperation == StringOperations.StartWith
                                        ? result + Constants.PERCENT
                                        : (stringOperation == StringOperations.Contains
                                            ? Constants.PERCENT + result + Constants.PERCENT
                                            : result));
                            sbResult.Append(Utils.ConvertTo15CharRecordId(result));
                        }
                        else
                            invalidValue = true;
                    }
                    break;
                case Datatype.String:
                case Datatype.Picklist:
                case Datatype.Editable_Picklist:
                case Datatype.Email:
                    {
                        string result = objectManager.EscapeQueryString(value);
                        result = (stringOperation == StringOperations.StartWith
                                    ? result + Constants.PERCENT
                                    : (stringOperation == StringOperations.Contains
                                        ? Constants.PERCENT + result + Constants.PERCENT
                                        : result));
                        sbResult.Append(result);
                    }
                    break;
                case Datatype.Picklist_MultiSelect:
                    value = value.Replace(Environment.NewLine, Constants.SEMICOLON);
                    sbResult.Append(objectManager.EscapeQueryString(value));
                    break;
                case Datatype.Double:
                case Datatype.Decimal:
                    double dblValue;
                    if (Double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out dblValue))
                        sbResult.Append(dblValue.ToString(CultureInfo.InvariantCulture));
                    else
                        invalidValue = true;
                    break;
                case Datatype.Date:
                case Datatype.DateTime:
                    if (string.IsNullOrEmpty(value)) value = string.Empty;

                    if (string.IsNullOrEmpty(Utils.IsValidDate(value.Trim(), dataType)))
                        invalidValue = true;
                    else
                        sbResult.Append(Utils.IsValidDate(value.Trim(), dataType));
                    break;
                case Datatype.Boolean:
                    bool bValue;
                    if (bool.TryParse(value, out bValue))
                        sbResult.Append(bValue.ToString());
                    else
                        invalidValue = true;
                    break;
            }

            sbResult.Append(appendQuote ? Constants.QUOTE : string.Empty);
            return sbResult.ToString();
        }

        public static string IsValidDate(string parseDateTime, Datatype dataType)
        {
            string result = string.Empty;
            CultureInfo currentCultureInfo = GetLatestCulture;
            string cultureDateFormat = GetLatestCulture.DateTimeFormat.ShortDatePattern;
            string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "M-d-yyyy", "M/d/yyyy h:mm:ss tt","yyyy-MM-dd",
                                   currentCultureInfo.DateTimeFormat.ShortDatePattern,
                                   currentCultureInfo.DateTimeFormat.ShortDatePattern + Constants.SPACE + currentCultureInfo.DateTimeFormat.LongTimePattern,
                                   "MM/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt",
                                   currentCultureInfo.DateTimeFormat.FullDateTimePattern};

            DateTime parsedDateTime;

            if (DateTime.TryParseExact(parseDateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
            {
                if (dataType == Datatype.Date)
                    result = parsedDateTime.ToString("yyyy-MM-dd");
                else if (dataType == Datatype.DateTime)
                    // SOQL query on DateTime will require the conversion of DateTime to UTC format before query. No conversion needed for Date.
                    // Changed format from hh:mm:ss -> HH:mm:ssm, which returns time value in 24-hour format.
                    // Always use timeofday (or 24-hour time), while evaluating against DateTime value with SOQL.
                    result = parsedDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else if (DateTime.TryParse(parseDateTime, out parsedDateTime))
            {
                if (dataType == Datatype.Date)
                    result = parsedDateTime.ToString("yyyy-MM-dd");
                else if (dataType == Datatype.DateTime)
                    // SOQL query on DateTime will require the conversion of DateTime to UTC format before query. No conversion needed for Date.
                    // Changed format from hh:mm:ss -> HH:mm:ssm, which returns time value in 24-hour format.
                    // Always use timeofday (or 24-hour time), while evaluating against DateTime value with SOQL.
                    result = parsedDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            return result;
        }

        public static bool IsInsertDeleteRowAddress(string rangeAddress)
        {
            Regex rangeRegex = new Regex(@"(\$)(\d+)(:)(\$)(\d+)");
            Match match = rangeRegex.Match(rangeAddress);

            return match.Success;
        }

        /* Validate excel file name for special characters */

        private static readonly Regex InvalidFileRegex = new Regex(
            string.Format("[{0}]", Regex.Escape(@"<>:""/\|?*")));

        public static string ReplaceInvalidChrs(string fileName)
        {
            string ValidFileName;
            if (InvalidFileRegex.IsMatch(fileName))
            {
                ValidFileName = InvalidFileRegex.Replace(fileName, string.Empty);
                return ValidFileName;
            }

            return fileName;
        }

        /* to serialzie font object , convert into string */
        public static string ConvertFontToString(System.Drawing.Font objFont)
        {
            if (objFont != null)
            {
                var cvt = new System.Drawing.FontConverter();
                string sFont = string.Empty;
                try
                {
                    sFont = cvt.ConvertToInvariantString(objFont);
                }
                catch (Exception ex)
                {
                    ApttusMessageUtil.ShowError(ex.Message.ToString(), resourceManager.GetResource("COREUTILS_ConvertFontCap_ErrorMsg"));
                }
                return sFont;
            }
            else
                return null;
        }

        /* to de-serialzie font object , convert from  string */
        public static System.Drawing.Font ConvertFontFromString(string sFont)
        {
            if (!string.IsNullOrEmpty(sFont))
            {
                var cvt = new System.Drawing.FontConverter();
                System.Drawing.Font objFont = null;
                try
                {
                    objFont = cvt.ConvertFromInvariantString(sFont) as System.Drawing.Font;
                }
                catch (Exception ex)
                {
                    ApttusMessageUtil.ShowError(ex.Message.ToString(), "Convert Font");
                }
                return objFont;
            }
            else
                return null;
        }

        /// <summary>
        /// This method is used in only cell reference, set null values
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string SetNullValues(Datatype dataType, string Operator)
        {
            string result = string.Empty;
            switch (dataType)
            {
                case Datatype.Double:
                case Datatype.Decimal:
                case Datatype.Date:
                case Datatype.DateTime:
                    result = "NULL";
                    break;
                case Datatype.Boolean:
                    result = "false";
                    break;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string ConvertTo15CharRecordId(string recordId)
        {
            string result = recordId;
            if (recordId.Length == 18)
                result = recordId.Substring(0, 15);
            return result;
        }

        public static string GetDirectoryForExternalLib()
        {
            string folderName = string.Empty;
            try
            {
                folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus" + Path.DirectorySeparatorChar + "External Libraries");

                DirectoryInfo di = new DirectoryInfo(folderName);
                if (!di.Exists)
                    di.Create();
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
            return folderName;
        }

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        public static string GetActiveCRMConfigFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus", Constants.ACTIVE_CRM_CONFIG_FILE);
        }
        public static string GetApttusApplicationDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus");
        }

        //public static string GetAppLogsFolderPath()
        //{
        //    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Apttus", ServiceConstants.AppLogsFolder);
        //}
    }
}
