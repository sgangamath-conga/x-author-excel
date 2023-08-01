/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Apttus.XAuthor.Core;
using System.Threading;
using System.Web;

namespace Apttus.XAuthor.AppDesigner.Modules
{
    public class ApttusObjectManager
    {
        public static string gObjectId = "";
        public static string gObjectType;
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowBusyCursor(string msg)
        {
            Globals.ThisAddIn.Application.StatusBar = msg;
            Cursor.Current = Cursors.WaitCursor;
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowNormalCursor()
        {
            Cursor.Current = Cursors.Arrow;
            Globals.ThisAddIn.Application.StatusBar = "";
            System.Windows.Forms.Application.DoEvents();
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
                bool PropertyExists = false;

                Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;

                dynamic properties = wb.GetType().InvokeMember("CustomDocumentProperties", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, wb, null);

                foreach (dynamic Property in properties)
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
                HandleError(ex);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propVal"></param>
        public static void RemoveDocProperty(string propName)
        {
            try
            {
                Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;

                dynamic properties = wb.GetType().InvokeMember("CustomDocumentProperties", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.GetProperty, null, wb, null);

                foreach (dynamic Property in properties)
                {
                    if (Property != null && Property.Name.ToString().Equals(propName))
                    {
                        Property.Delete();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void HandleError(Exception ex, string message = "", string messageWindowCaption = "")
        {
            ApttusCommandBarManager CommandBar = ApttusCommandBarManager.GetInstance();
            ObjectManager objectManager = ObjectManager.GetInstance;

            ShowNormalCursor();

            // Set token value to empty, so that Manage connection correctly enables Connect & Revoke buttons
            CommandBar.ResetToken();
            //Globals.ThisAddIn.oAuthWrapper.TokenValue = string.Empty;

            Thread th = Thread.CurrentThread;
            string thName = th.Name != null ? th.Name : th.GetHashCode().ToString();

            try
            {
                // If this was a Salesforce Fault Exception get Exception Code
                string exceptionCodeOrType = objectManager.GetExceptionCode(ex);

                //If exceptionCodeOrType is empty, assign type of exception to it
                if (string.IsNullOrEmpty(exceptionCodeOrType))
                {
                    exceptionCodeOrType = ex.GetType().ToString();
                }
                switch (exceptionCodeOrType)
                {
                    case Constants.SF_INVALID_SESSION_ID:
                        if (ex.Source != "LOGOFF")
                        {
                            Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExpTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExp_InfoMsg"), ToolTipIcon.Warning);
                            Globals.Ribbons.ApttusRibbon.apttusNotification.Tag = "SESSION_EXPIRED";
                        }
                        ApttusCommandBarManager.g_IsLoggedIn = false;
                        ApttusCommandBarManager.g_IsAppOpen = false;
                        CommandBar.DoLogout(false);
                        break;
                    case Constants.SF_INVALID_LOGIN:
                        if (ex.GetHashCode() == 54875957)
                        {
                            Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSetting_InfoMsg"), ToolTipIcon.Warning);
                        }
                        else
                        {
                            Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogLoginCredTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogLoginCredDes_InfoMsg"), ToolTipIcon.Warning);
                        }
                        break;
                    case Constants.SF_CLIENT_EXCEPTION:
                        ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("RIBBON_LoadApp_ErrorMsg"), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version), resourceManager.GetResource("RIBBON_LoadAppCap_ErrorMsg"));
                        break;
                    case "System.Web.HttpException":
                        {
                            HttpException httpEx = ex as HttpException;
                            if (httpEx.GetHttpCode() == (int)System.Net.HttpStatusCode.Forbidden)
                            {
                                ApttusMessageUtil.ShowError(httpEx.Message, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogForbiddenOperationTitle_InfoMs"));
                            }
                            else
                            {
                                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                exceptionCodeOrType, String.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogExc_InfoMsg"), exceptionCodeOrType, httpEx.Message, resourceManager.CRMName), ToolTipIcon.Error);
                            }
                        }
                        break;
                    case "System.Net.WebException":
                    case "System.ServiceModel.CommunicationException":
                    case "System.ServiceModel.EndpointNotFoundException":
                        {
                            if (ex.Message.Contains("The request failed with HTTP status 404: Not Found."))
                            {
                                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), string.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogLoginCred_InfoMsg"), resourceManager.CRMName), ToolTipIcon.Warning);
                                Globals.Ribbons.ApttusRibbon.apttusNotification.Tag = "OPTIONS_DIALOG";
                            }
                            else if (ex.InnerException.Message.Contains("Unable to connect to the remote server") || ex.InnerException.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect to the remote server") || ex.Message.Contains("The underlying connection was closed"))
                            {
                                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogConnFailedTitle_InfoMsg"), string.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogConnFailed_InfoMsg"), resourceManager.CRMName), ToolTipIcon.Error);
                                ApttusCommandBarManager.g_IsLoggedIn = false;
                                ApttusCommandBarManager.g_IsAppOpen = false;
                                CommandBar.DoLogout(false);
                            }
                            else if (ex.Message.Contains("Proxy Authentication Required."))
                            {
                                Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogProxyTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogProxy_InfoMsg"), ToolTipIcon.Error);
                            }
                        }
                        break;
                    case "Apttus.IntelligentCloud.LoginControl.AICLoginException":
                        if (ex.Message.Contains("XAE_Config_Missing"))
                        {
                            string Message = resourceManager.GetResource("ApttusObjectManager_HandleError_AIC_XAE_Config_Missing");
                            string Caption = resourceManager.GetResource("COMMON_LoginFailed_ErrorMsg");
                            ExceptionLogHelper.ErrorLog(ex, true, Message, Caption);
                        }
                        else
                        {
                            ExceptionLogHelper.ErrorLog(ex, true);
                        }
                        break;
                    default:
                        if (ex.GetHashCode() == 52136803)
                        {
                            if (CommandBar.IsLoggedIn())
                            {
                                ApttusCommandBarManager.g_IsLoggedIn = false;
                                ApttusCommandBarManager.g_IsAppOpen = false;
                                CommandBar.DoLogout(false);
                            }
                        }
                        else if (ex.Message.Equals("Not found"))
                        {
                            Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), resourceManager.GetResource("APTTUSOBJMAN_HandleErrorLoginDialog_InfoMsg"), ToolTipIcon.Warning);
                        }
                        else
                        {
                            Globals.Ribbons.ApttusRibbon.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                exceptionCodeOrType, String.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogExc_InfoMsg"), exceptionCodeOrType, message, resourceManager.CRMName), ToolTipIcon.Warning);
                        }
                        break;
                }
                // Don't show a pop-up message in finally as Balloon Notification is shown above for SF Exception Code
                // showMessage = false;


            }
            catch (Exception)
            {
                if (CommandBar.IsLoggedIn())
                    if (thName.ToUpper().Equals("VSTA_MAIN"))
                        ExceptionLogHelper.ErrorLog(ex);
            }
            finally
            {
                ExceptionLogHelper.ErrorLog(ex);
            }
        }


        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="oxl"></param>
        /// <param name="fileName"></param>
        public static void SaveAs(Excel.Workbook oxl, string fileName)
        {

            object oSaveFormat = Excel.XlFileFormat.xlWorkbookNormal;

            Globals.ThisAddIn.Application.DisplayAlerts = false;

            if (fileName.ToLower().EndsWith(Constants.XLSXWITHOUTDOT))
                oSaveFormat = Excel.XlFileFormat.xlWorkbookDefault;
            else if (fileName.ToLower().EndsWith(Constants.XLSMWITHOUTDOT))
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] EncodeByteArray(bool IsMacroExists = false)
        {
            FileInfo file;
            string encodedFileNamePath;
            // If Macro Exists then it will save with XLSM
            if (IsMacroExists)
            {
                //get the name of the active document
                ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
                string activeWbPath = commandBar.GetActiveWorkbookName();
                // Get extension
                string extension = Constants.XLSM;
                //get the temporary files folder path and create the name where we will put the encoded output
                string defaultPath = Path.GetTempPath();
                //string encodedFileName = "_temp_encoded_byte.xlsx";
                string encodedFileName = "_temp_encoded_byte" + extension;
                //construct the name of the file we will base64 encode the output to
                encodedFileNamePath = defaultPath + encodedFileName;

                //if the encoded file exists then delete it
                file = new FileInfo(encodedFileNamePath);
                if (file.Exists)
                {
                    file.Delete();
                }

                // Since File.Copy is not being reliable sometimes (refer case # 00003587)
                // Using Save as to create the document needed for encoding
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, encodedFileNamePath);

                // Saving back to the original document
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, Path.ChangeExtension(activeWbPath, extension));
            }
            else
            {
                //get the name of the active document
                ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
                string activeWbPath = commandBar.GetActiveWorkbookName();
                // Get extension
                string extension = Path.GetExtension(activeWbPath);
                //get the temporary files folder path and create the name where we will put the encoded output
                string defaultPath = Path.GetTempPath();
                //string encodedFileName = "_temp_encoded_byte.xlsx";
                string encodedFileName = "_temp_encoded_byte" + extension;
                //construct the name of the file we will base64 encode the output to
                encodedFileNamePath = defaultPath + encodedFileName;

                //if the encoded file exists then delete it
                file = new FileInfo(encodedFileNamePath);
                if (file.Exists)
                {
                    file.Delete();
                }

                // Since File.Copy is not being reliable sometimes (refer case # 00003587)
                // Using Save as to create the document needed for encoding
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, encodedFileNamePath);

                // Saving back to the original document
                SaveAs(Globals.ThisAddIn.Application.ActiveWorkbook, activeWbPath);
            }
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

        //insert metadata sheet in the currently opened workbook to store 
        // applciation id so that at runtime loadapplication() can be called
        // using the same id.
        public static bool InsertMetaSheet(Core.Application App, ref Excel.Workbook OutWorkBook, bool isExistingApp = false)
        {
            if (string.IsNullOrEmpty(App.Definition.UniqueId))
                return false;

            if (!isExistingApp)
                OutWorkBook = Globals.ThisAddIn.Application.Workbooks.Add();

            Excel.Workbook wb = Globals.ThisAddIn.Application.ActiveWorkbook;

            try
            {
                if (wb.Worksheets.Count > 0)
                {
                    if (isExistingApp)
                    {
                        foreach (Excel.Worksheet sheet2 in wb.Worksheets)
                        {
                            // Check if template has same unique id then it will create new workbook
                            if (sheet2.Name.Equals(Constants.METADATA_SHEETNAME))
                            {
                                Excel.Range rng2 = sheet2.Cells[1, 1];
                                rng2.Value2 = App.Definition.UniqueId;
                                return true;
                            }
                        }
                    }
                    CreateMetaSheet(App, OutWorkBook);
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(ex.Message.ToString(), resourceManager.GetResource("APTTUSOBJMAN_InsertMetaSheet_ErrMsg"));
                ExceptionLogHelper.ErrorLog(ex);
                return false;
            }
            return true;
        }

        static public void CreateMetaSheet(Core.Application App, Excel.Workbook OutWorkBook)
        {
            OutWorkBook = OutWorkBook.Application.ActiveWorkbook;
            Excel.Worksheet OActiveSheet = OutWorkBook.Application.ActiveSheet as Excel.Worksheet;
            object lastSheet = OutWorkBook.Worksheets[OutWorkBook.Worksheets.Count] as Excel.Worksheet;
            Excel.Worksheet workSheet = OutWorkBook.Worksheets.Add(Type.Missing, // before sheet
                             lastSheet,     // after sheet
                             1,            // number of sheets to add
                             Excel.XlSheetType.xlWorksheet) as Excel.Worksheet;
            workSheet.Visible = Microsoft.Office.Interop.Excel.XlSheetVisibility.xlSheetVeryHidden;
            workSheet.Name = Constants.METADATA_SHEETNAME;
            Excel.Range oRange = workSheet.Cells[1, 1];
            oRange.Value2 = App.Definition.UniqueId;
            Excel.Worksheet oSheet = (Excel.Worksheet)OutWorkBook.Worksheets[OActiveSheet.Name];
            oSheet.Select();
        }
    }
}
