/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

using Core = Apttus.XAuthor.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;

namespace Apttus.XAuthor.AppRuntime.Modules
{
    public class ApttusObjectManager
    {
        public static string gObjectId = "";
        public static string gObjectType;
        static System.Threading.Mutex mut = new System.Threading.Mutex();

        //keep track of temporary files
        private static ArrayList gTempFiles = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowBusyCursor(string msg)
        {
            if (Core.ObjectManager.RuntimeMode == Core.RuntimeMode.AddIn)
            {
                Globals.ThisAddIn.Application.StatusBar = msg;
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowNormalCursor()
        {
            Cursor.Current = Cursors.Arrow;
            if (Core.ObjectManager.RuntimeMode == Core.RuntimeMode.AddIn)
                Globals.ThisAddIn.Application.StatusBar = "";
            Application.DoEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objId"></param>
        public static void SetObjectId(string objId)
        {
            gObjectId = objId;
            if (objId != "")
            {
                SetDocProperty(ApttusGlobals.OBJECT_ID, objId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetObjectId()
        {
            string id = GetDocProperty(ApttusGlobals.OBJECT_ID);
            return (id != "") ? id : gObjectId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propVal"></param>
        public static void SetDocProperty(string propName, string propVal)
        {
            try
            {

                if (Apttus.XAuthor.Core.ObjectManager.RuntimeMode == Apttus.XAuthor.Core.RuntimeMode.Batch)
                {
                    RemoveDocPropertyInterOp(propName, ExcelHelper.ExcelApp.ActiveWorkbook);
                    SetDocPropertyInterOp(propName, propVal, ExcelHelper.ExcelApp.ActiveWorkbook);
                    return;
                }

                bool PropertyExists = false;

                Microsoft.Office.Core.DocumentProperties properties = (Microsoft.Office.Core.DocumentProperties)
                    Globals.ThisAddIn.Application.ActiveWorkbook.CustomDocumentProperties;

                foreach (Microsoft.Office.Core.DocumentProperty Property in properties)
                {
                    if (Property.Name.ToString().Equals(propName))
                    {
                        //assume property exists and update it
                        properties[propName].Value = propVal;
                        PropertyExists = true;
                    }
                }
                if (PropertyExists == false)
                {
                    properties.Add(propName, false, Microsoft.Office.Core.MsoDocProperties.msoPropertyTypeString, propVal, false);
                    Globals.ThisAddIn.Application.ActiveWorkbook.Saved = false;
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Set Doc Property");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string GetDocProperty(string propName)
        {
            try
            {
                string sProperty = "";
                if (Globals.ThisAddIn.Application.ActiveWorkbook != null)
                {
                    Microsoft.Office.Core.DocumentProperties properties = (Microsoft.Office.Core.DocumentProperties)
                        Globals.ThisAddIn.Application.ActiveWorkbook.CustomDocumentProperties;

                    foreach (Microsoft.Office.Core.DocumentProperty p in properties)
                    {
                        if (propName == p.Name)
                        {
                            sProperty = "" + properties[propName].Value;
                            break;
                        }
                    }
                }
                return sProperty;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //string isRuntimeFile = ApttusObjectManager.GetDocProperty(Constants.APPBUILDER_RUNTIME_FILE);
        public static bool IsActiveWorkbookRuntime()
        {
            string isRuntimeFile = GetDocProperty(Core.Constants.APPBUILDER_RUNTIME_FILE);
            return !string.IsNullOrEmpty(isRuntimeFile);
        }

        public static void RemoveDesignerFlag(string fileName)
        {
            Excel.Application ExcelApp = new Excel.Application();

            ExcelApp.EnableEvents = false;

            Excel.Workbook ExcelWb = ExcelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing);

            // Remove Designer property from Template 
            RemoveDocPropertyInterOp(Apttus.XAuthor.Core.Constants.APPBUILDER_DESIGNER_FILE, ExcelWb);

            ExcelWb.Save();
            ExcelWb.Close(false);
            ExcelApp.EnableEvents = true;
            ExcelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelApp);
        }

        public static void SetDocPropertyInterOp(string propName, string propVal, Excel.Workbook wb = null)
        {
            try
            {
                object oDocCustomProps = wb.CustomDocumentProperties;
                Type typeDocCustomProps = oDocCustomProps.GetType();
                object[] oArgs = {propName,false,
                 MsoDocProperties.msoPropertyTypeString,
                 propVal};

                typeDocCustomProps.InvokeMember("Add", BindingFlags.Default |
                                           BindingFlags.InvokeMethod, null,
                                           oDocCustomProps, oArgs);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Set Doc Property Interop");
            }
        }
        public static void RemoveDocPropertyInterOp(string propName, Excel.Workbook wb = null)
        {
            try
            {
                //Microsoft.Office.Core.DocumentProperties properties = null;
                dynamic properties = null;

                properties = wb.GetType().InvokeMember("CustomDocumentProperties", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, wb, null);

                foreach (dynamic Property in properties)
                {
                    //property exists 
                    if (Property.Name.ToString().Equals(propName))
                        Property.Delete();
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Remove Doc Property Interop");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propVal"></param>
        public static void RemoveDocProperty(string propName, Excel.Workbook wb = null)
        {
            Microsoft.Office.Core.DocumentProperties properties = null;
            try {
                if (Apttus.XAuthor.Core.ObjectManager.RuntimeMode == Apttus.XAuthor.Core.RuntimeMode.Batch)
                {
                    RemoveDocPropertyInterOp(propName, ExcelHelper.ExcelApp.ActiveWorkbook);
                    return;
                }

                if (wb != null)
                    properties = (Microsoft.Office.Core.DocumentProperties)wb.CustomDocumentProperties;
                else
                {
                    if (Globals.ThisAddIn != null && Globals.ThisAddIn.Application != null && Globals.ThisAddIn.Application.ActiveWorkbook != null)
                        properties = (Microsoft.Office.Core.DocumentProperties)Globals.ThisAddIn.Application.ActiveWorkbook.CustomDocumentProperties;
                    else
                    {
                        RuntimeExceptionLogHelper.DebugLog("\n Globals.ThisAddin or any one of the properties is null.");
                    }
                }
                if (properties != null && properties.Count > 0)
                {
                    for (int i = 1; i <= properties.Count; i++)
                    {
                        Microsoft.Office.Core.DocumentProperty Property = properties[i];
                        if (Property != null)
                        {
                            if (Property.Name != null && Property.Name.ToString().Equals(propName))
                            {
                                Property.Delete();
                            }
                          if(Property != null)
                                Marshal.ReleaseComObject(Property);
                        }
                    }
                }
                else
                {
                    RuntimeExceptionLogHelper.DebugLog("\n DocumentProperties is null.");
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.DebugLog(ex.Message);
            } 
            finally {
                if(properties != null) Marshal.ReleaseComObject(properties);
                if (wb != null) Marshal.ReleaseComObject(wb);

            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ex"></param>
        public static void HandleError(Exception ex)
        {
            Core.ExceptionLogHelper.ErrorLog(ex);
            ApttusObjectManager.ShowNormalCursor();
            // Set token value to empty, so that Manage connection correctly enables Connect & Revoke buttons

            ApttusCommandBarManager CommandBar = ApttusCommandBarManager.GetInstance();
            CommandBar.ResetToken();

            System.Threading.Thread th = System.Threading.Thread.CurrentThread;
            string thName = th.Name != null ? th.Name : th.GetHashCode().ToString();

            try
            {
                switch (ex.GetType().ToString())
                {
                    case "System.Web.Services.Protocols.SoapException":
                        {
                            if (((System.Web.Services.Protocols.SoapException)(ex)).Code.Name == "INVALID_SESSION_ID")
                            {
                                if (ex.Source != "LOGOFF")
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Session Expired", "Your current session has timed out. Please login again to continue with your work." + System.Environment.NewLine + System.Environment.NewLine + "Click here to login.", ToolTipIcon.Warning);
                                    Globals.ThisAddIn.apttusNotification.Tag = "SESSION_EXPIRED";
                                }
                                CommandBar.DoLogout(true);
                            }
                            else if (((System.Web.Services.Protocols.SoapException)(ex)).Code.Name == "INVALID_LOGIN")
                            {
                                if (ex.GetHashCode().ToString() == "54875957")
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Check your settings!", "Unable to send request to server. \rThe server name or address could not be resolved.", ToolTipIcon.Warning);
                                }
                                else
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Invalid login credentials!", "Invalid username, password, security token; or user locked out.", ToolTipIcon.Warning);
                                }
                            }
                            else if (((System.Web.Services.Protocols.SoapException)(ex)).Code.Name == "INVALID_SEARCH")
                            {
                                //Globals.Ribbons.RuntimeRibbon.apttusNotification.ShowBalloonTip(1000, "Check your search criteria!", ex.Message, ToolTipIcon.Warning);
                            }
                            else
                            {
                                if (thName.ToUpper().Equals("VSTA_MAIN"))
                                    //MessageBox.Show(ex.Message, ApttusGlobals.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Core.ExceptionLogHelper.ErrorLog(ex, true);
                            }
                        }
                        break;

                    case "System.Runtime.InteropServices.COMException":
                        {
                            if (((System.Runtime.InteropServices.COMException)(ex)).ErrorCode.ToString() == "-2146823135")
                            {
                                // Word doc trying to be saved is already opened with the same name
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "X-Author Chatter!", "You have another Word document open with the same name. Please close that Word document and try to save again.", ToolTipIcon.Warning);
                            }
                            else
                            {
                                if (CommandBar.IsLoggedIn())
                                    if (thName.ToUpper().Equals("VSTA_MAIN"))
                                        //MessageBox.Show(ex.Message, ApttusGlobals.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        Core.ExceptionLogHelper.ErrorLog(ex, true);
                            }
                        }
                        break;

                    case "System.Net.WebException":
                        {
                            if (ex.Message.Contains("The request failed with HTTP status 404: Not Found."))
                            {
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Check your settings!", "Conga Server Host could not be resolved. Check your settings in the options dialog." + System.Environment.NewLine + System.Environment.NewLine + "Click here to open Options dialog.", ToolTipIcon.Warning);
                                Globals.ThisAddIn.apttusNotification.Tag = "OPTIONS_DIALOG";
                            }
                            else if (ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect to the remote server") || ex.Message.Contains("The underlying connection was closed"))
                            {
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Check your internet connectivity!", "Conga X-Author Chatter could not connect to Salesforce. Check whether your computer is online.", ToolTipIcon.Error);
                                CommandBar.DoLogout(false);
                            }
                            else if (ex.Message.Contains("Proxy Authentication Required."))
                            {
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Check your Conga proxy settings!", "Your proxy requires authentication. Please provide credentials at the login dialog.", ToolTipIcon.Error);
                            }
                            else
                            {
                                //MessageBox.Show(ex.Message, ApttusGlobals.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Core.ExceptionLogHelper.ErrorLog(ex, true);
                            }
                        }
                        break;

                    case "Apttus.IntelligentCloud.LoginControl.AICLoginException":
                        if (ex.Message.Contains("XAE_Config_Missing"))
                        {
                            string Message = Core.ApttusResourceManager.GetInstance.GetResource("ApttusObjectManager_HandleError_AIC_XAE_Config_Missing");
                            string Caption = Core.ApttusResourceManager.GetInstance.GetResource("ADDIMIMPL_LoginFailedInstr_ErrorMsg");
                            Core.ExceptionLogHelper.ErrorLog(ex, true, Message, Caption);
                        }
                        else
                        {
                            Core.ExceptionLogHelper.ErrorLog(ex, true);
                        }
                        break;

                    default:
                        {
                            if (ex.GetHashCode().ToString() == "52136803")
                            {
                                //MessageBox.Show(ex.Message, ApttusGlobals.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Core.ExceptionLogHelper.ErrorLog(ex, true);
                                if (CommandBar.IsLoggedIn())
                                {
                                    CommandBar.DoLogout(true);
                                }
                            }
                            else if (ex.Message.Equals("Not found"))
                            {
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, "Check your settings!", "Conga Server Host could not be resolved. Check your settings in the login dialog.", ToolTipIcon.Warning);
                            }
                            else
                            {
                                if (thName.ToUpper().Equals("VSTA_MAIN"))
                                    Core.ExceptionLogHelper.ErrorLog(ex, true);
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                if (CommandBar.IsLoggedIn())
                    if (thName.ToUpper().Equals("VSTA_MAIN"))
                        //MessageBox.Show(ex.Message, ApttusGlobals.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Core.ExceptionLogHelper.ErrorLog(ex, true);
            }
        }


        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="oxl"></param>
        /// <param name="fileName"></param>
        public static void SaveAs(Excel.Workbook oxl, string fileName)
        {
            object oSaveFormat = ApttusGlobals.oSaveFormat;
            if (Apttus.XAuthor.Core.ObjectManager.RuntimeMode != Apttus.XAuthor.Core.RuntimeMode.Batch)
            {
                Globals.ThisAddIn.Application.DisplayAlerts = false;
            }
            else
            {
                ExcelHelper.ExcelApp.Application.DisplayAlerts = false;
            }


            if (fileName.ToLower().EndsWith(Apttus.XAuthor.Core.Constants.XLSXWITHOUTDOT))
                oSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;
            else if (fileName.ToLower().EndsWith(Apttus.XAuthor.Core.Constants.XLSMWITHOUTDOT))
                oSaveFormat = Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled;
            else
                oSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;

            object oFileName = fileName;
            oxl.SaveAs(
                oFileName,						        //filename to save as
                oSaveFormat,		                    //file format
                ApttusGlobals.oMissing,			        //password
                ApttusGlobals.oMissing,			        //WriteResPassword
                ApttusGlobals.oMissing,			        //ReadOnlyRecommended
                ApttusGlobals.oMissing,			        //CreateBackup
                Excel.XlSaveAsAccessMode.xlNoChange,	//AccessMode
                ApttusGlobals.oMissing,			        //ConflictResolution
                ApttusGlobals.oFalse,			        //AddToMru
                ApttusGlobals.oMissing,			        //TextCodepage
                ApttusGlobals.oMissing,		            //TextVisualLayout
                ApttusGlobals.oMissing			        //Local
                );
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="filename"></param>
        public static void OpenFile(string filename)
        {
            Excel.Application app = (Excel.Application)Globals.ThisAddIn.Application;
            Excel.Workbook xWb = app.Workbooks.Open(
                                filename,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing,
                                ApttusGlobals.oMissing);
        }

        /// <summary>
        /// Get url of the current session from the salesforce service
        /// </summary>
        /// <param name="bRemoveApi"></param>
        /// <returns></returns>
        //public static String GetSessionServerUrl(bool bRemoveApi)
        //{
        //    try
        //    {
        //        String sUrl, strTemp = "";
        //        int intPos, strLen;

        //        sUrl = Globals.ThisAddIn.oAuthWrapper.token.instance_url;
        //        if ((sUrl.IndexOf("salesforce.com/")) > 0)
        //        {
        //            intPos = sUrl.IndexOf("salesforce.com/");
        //            strLen = ("salesforce.com/").Length;
        //            strTemp = sUrl.Substring(0, intPos + strLen - 1);

        //            //optionally remove "-api" from the url
        //            if (bRemoveApi)
        //            {
        //                strTemp = strTemp.Replace("-api", "");
        //            }
        //        }

        //        //remove any trailing / characters from the url
        //        strTemp = ApttusObjectManager.RemoveTrailingPathSeparator(strTemp);
        //        return strTemp;
        //    }
        //    catch (Exception ex)
        //    {
        //        RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Get Server URL");
        //        return "https://www.salesforce.com";
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveTrailingPathSeparator(string s)
        {
            if (String.IsNullOrEmpty(s))
                return null;

            try
            {
                if (s.Substring(s.Length - 1, 1) == "/")
                {
                    s = s.Remove(s.Length - 1);
                }
                return s;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        public static string GetFileNamePrefix(string fPath)
        {
            try
            {
                int intPos;
                string strTemp;

                if (fPath.LastIndexOf(".") > 0)
                {
                    //return the portion of the string before the last "."
                    intPos = fPath.LastIndexOf(".");
                    strTemp = fPath.Substring(0, intPos);
                }
                else
                {
                    strTemp = fPath;
                }
                return strTemp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        public static string GetFileNameExt(string fPath)
        {
            try
            {
                int intPos;
                string strTemp;

                if (fPath.LastIndexOf(".") > 0)
                {
                    //return the portion of the string after the last "."
                    intPos = fPath.LastIndexOf(".") + 1;
                    strTemp = fPath.Substring(intPos);
                }
                else
                {
                    strTemp = "";
                }
                return strTemp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] EncodeByteArray()
        {

            //get the name of the active document
            ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
            string activeWbPath = commandBar.GetActivWorkbookName();
            // Get extension
            string extension = Path.GetExtension(activeWbPath);

            //get the temporary files folder path and create the name where we will put the encoded output
            string defaultPath = Path.GetTempPath();
            string encodedFileName = "_temp_encoded_byte." + extension;

            //construct the name of the file we will base64 encode the output to
            string encodedFileNamePath = defaultPath + encodedFileName;

            //if the encoded file exists then delete it
            FileInfo file = new FileInfo(encodedFileNamePath);
            if (file.Exists)
            {
                file.Delete();
            }

            // Since File.Copy is not being reliable sometimes (refer case # 00003587)
            // Using Save as to create the document needed for encoding
            if (Apttus.XAuthor.Core.ObjectManager.RuntimeMode == Apttus.XAuthor.Core.RuntimeMode.Batch)
            {
                SaveAs(ExcelHelper.ExcelApp.ActiveWorkbook, encodedFileNamePath);
            }
            else
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, encodedFileNamePath);

            // Saving back to the original document
            if (Apttus.XAuthor.Core.ObjectManager.RuntimeMode == Apttus.XAuthor.Core.RuntimeMode.Batch)
            {
                SaveAs(ExcelHelper.ExcelApp.ActiveWorkbook, activeWbPath);
            }
            else
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, activeWbPath);

            //read binary data from the file we are going to encode
            byte[] bytesToEncode = Core.Utils.FileToBytes(encodedFileNamePath);

            //FileStream fs = new FileStream(encodedFileNamePath, FileMode.Open, FileAccess.Read);
            //byte[] encodedata = new byte[fs.Length];
            //fs.Read(encodedata, 0, System.Convert.ToInt32(fs.Length));

            //fs.Flush();
            //fs.Dispose();
            //fs.Close();

            //delete the temporary file
            file = new FileInfo(encodedFileNamePath);

            if (file.Exists)
            {
                file.Delete();
            }

            //return the base64 binary encoded data
            return bytesToEncode;
        }

        /// <summary>
        /// Return byte array of workbook
        /// </summary>
        /// <param name="encodedFileNamePath"></param>
        /// <returns></returns>
        public static byte[] EncodeByteArray(string activeWbPath, Excel.Workbook oWorkbook)
        {

            //get the temporary files folder path and create the name where we will put the encoded output
            string defaultPath = Path.GetTempPath();
            // Get extension
            string extension = Path.GetExtension(activeWbPath);

            string encodedFileName = "_temp_encoded." + extension;

            //if the encoded file exists then delete it
            FileInfo file = new FileInfo(Path.Combine(defaultPath, encodedFileName));
            if (file.Exists)
            {
                file.Delete();
            }

            //construct the name of the file we will base64 encode the output to
            string encodedFileNamePath = Path.Combine(defaultPath, encodedFileName);

            // Since File.Copy is not being reliable sometimes (refer case # 00003587)
            // Using Save as to create the document needed for encoding
            SaveAs(oWorkbook, encodedFileNamePath);

            // Saving back to the original document
            SaveAs(oWorkbook, activeWbPath);

            //read binary data from the file we are going to encode
            FileStream fs = new FileStream(encodedFileNamePath, FileMode.Open, FileAccess.Read);
            byte[] encodedata = new byte[fs.Length];
            fs.Read(encodedata, 0, System.Convert.ToInt32(fs.Length));

            fs.Flush();
            fs.Dispose();
            fs.Close();

            //delete the temporary file
            file = new FileInfo(encodedFileNamePath);

            if (file.Exists)
            {
                file.Delete();
            }

            //return the base64 binary encoded data
            return encodedata;
        }

        /// <summary>
        /// Get TempDirectory
        /// </summary>
        /// <returns></returns>
        public static string GetTempDirectory()
        {
            mut.WaitOne();
            string sPath = Path.GetTempPath();
            if (!sPath.EndsWith("Temp"))
            {
                sPath = Path.Combine(sPath, "Temp");
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
            }
            mut.ReleaseMutex();
            return sPath;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xl"></param>
        /// <param name="saveAsName"></param>
        public static void SaveInPlaceOrAsTempDocument(Excel.Workbook xl, string saveAsName)
        {

            //get document name
            string xlWorkbookName = (saveAsName == null || saveAsName == "")
                ? System.Web.HttpUtility.UrlDecode(xl.Name)
                : saveAsName;

            //get document path
            string xlWorkbookPath = xl.Path;

            //get the location of the user's temporary folder
            string tempFilePath = Path.GetTempPath();

            //determine how the doc is currently saved
            bool isTempFolderFile = xlWorkbookPath.ToLower().Contains(tempFilePath.ToLower());
            bool isTempIEFile = xlWorkbookPath.ToLower().Contains("temporary internet files");
            bool isNewDoc = (xlWorkbookPath == null || xlWorkbookPath == "");

            //test for web format using servlet.FileDownload
            if (ApttusObjectManager.WorkbookFullNameIsURL(xl.FullName))
            {

                xlWorkbookName = xlWorkbookName + ".xlsx";

                //create a path to the user's temporary folder
                xlWorkbookPath = Path.Combine(tempFilePath, xlWorkbookName);

                //save the document to the temporary folder so we don't have to prompt the user to save
                SaveAs(xl, xlWorkbookPath);

                //keep track of it so we can delete it later
                AddTempFile(xlWorkbookPath);
            }

            //if the document is temporary then save it to the temporary folder
            else if (isTempFolderFile || isTempIEFile)
            {

                //create filename in temporary folder
                string sFilename = Path.Combine(tempFilePath, xlWorkbookName);

                //check if the file exists
                FileInfo fileInfo = new FileInfo(sFilename);
                if (fileInfo.Exists)
                {
                    if (fileInfo.IsReadOnly)
                    {
                        //cant unlock since already opened as readonly thus need to create new file
                        //so we save it in a nested temporary folder
                        string sTempDir = ApttusObjectManager.GetTempDirectory();
                        sFilename = Path.Combine(sTempDir, xlWorkbookName);
                    }

                    //save as temporary file
                    ApttusObjectManager.SaveAs(xl, sFilename);

                    //keep track of it so we can delete it later
                    AddTempFile(sFilename);
                }
                else
                {
                    //the file does not exist, so save it in temporary folder
                    ApttusObjectManager.SaveAs(xl, sFilename);

                    //keep track of it so we can delete it later
                    AddTempFile(sFilename);
                }
            }
            else if (isNewDoc)
            {

                //create filename in temporary folder
                string sFilename = Path.Combine(tempFilePath, xlWorkbookName);

                //save the new document in the temporary folder
                ApttusObjectManager.SaveAs(xl, sFilename);

                //keep track of it so we can delete it later
                AddTempFile(sFilename);
            }
            else
            {
                //otherwise just save it in place
                xl.Save();
            }
        }

        /// <summary>
        /// Determines if the full pathname for a word document is a web-based URL
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static bool WorkbookFullNameIsURL(string fullName)
        {
            return (fullName.Contains("http:") || fullName.Contains("https:"))
                ? true
                : false;
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
    }


}
