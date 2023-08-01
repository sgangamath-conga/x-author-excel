/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Excel = Microsoft.Office.Interop.Excel;
using Apttus.ChatterWebControl.Modules;
using Apttus.XAuthor.Core;


namespace Apttus.XAuthor.AppRuntime.Modules
{

    /// <summary>
    /// 
    /// </summary>
    public class ApttusXAuthorForChatterManager
    {


        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        static XAuthorSalesforceLogin oAuthWrapper = Globals.ThisAddIn.GetLoginObject() as XAuthorSalesforceLogin;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="sbBody"></param>
        /// <param name="oActiveWb"></param>
        /// <returns></returns>
        private static bool ShareSelectedContentAsWorkSheet(string parentId, StringBuilder sbBody, Excel.Workbook oActiveWb, ChatterWebBrowserManager manager)
        {

            Excel.Worksheet oActiveSheet = (Excel.Worksheet)oActiveWb.ActiveSheet;

            Excel.Range range = Globals.ThisAddIn.Application.Selection as Excel.Range;

            string sFileName = oActiveWb.Name;
            string sFileExt = ApttusObjectManager.GetFileNameExt(oActiveWb.Name);

            if (String.IsNullOrEmpty(sFileExt))
            {

                sFileExt = "xlsx";
            }

            string sTempDir = ApttusObjectManager.GetTempDirectory();
            sFileName = System.IO.Path.Combine(sTempDir, ApttusObjectManager.GetFileNamePrefix(oActiveWb.Name) + "_" + oActiveSheet.Name + "." + sFileExt);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(sFileName);

            if (fileInfo.Exists)
                fileInfo.Delete();

            Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.Workbooks.Add();

            Excel.Worksheet oSheet = (Excel.Worksheet)oWorkbook.Worksheets[1];
            object oCell1 = "A1";

            range.Copy();
            // Resolve wrong font, bolded, wrong header, not space correctly, numbers cut off issues
            oSheet.Range[oCell1].PasteSpecial(Excel.XlPasteType.xlPasteValuesAndNumberFormats |
                                              Excel.XlPasteType.xlPasteColumnWidths, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone);
            if (sFileExt.Equals("xlsm"))
            {
                oWorkbook.SaveAs(sFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled);
            }
            else
            {
                oWorkbook.SaveAs(sFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            }

            string sTitle = ApttusObjectManager.GetFileNamePrefix(oWorkbook.Name);

            //base64 encoded the document
            byte[] fileBody = ApttusObjectManager.EncodeByteArray(sFileName, oWorkbook);
            string sFileNameWithFullPath = sFileName;
            sFileName = oWorkbook.Name;

            oWorkbook.Close();

            fileInfo = new System.IO.FileInfo(sFileNameWithFullPath);

            if (fileInfo.Exists)
                fileInfo.Delete();

            //return ShareDocumentViaChatter(parentId, sTitle, sFileName, sBody, sbBody.ToString());
            ChatterRestApi api = new ChatterRestApi(manager, oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
            return api.ShareContentViaChatter(parentId, sbBody.ToString(), fileBody, sTitle, sFileName);
            //return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool ShareHighlightedContentWithChatter(string parentId, string sAdditionalComments, ChatterWebBrowserManager manager)
        {

            bool bReturn = false;
            StringBuilder sbBody = new StringBuilder();
            try
            {
                ////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);

                if (!String.IsNullOrEmpty(sAdditionalComments))
                {
                    sbBody.AppendLine(sAdditionalComments);
                }

                if (sbBody.Length > 1000)
                {

                    MessageBox.Show(resourceManager.GetResource("CHATTERMGR_Post_ShowMsg"), resourceManager.GetResource("CHATTERMGR_PostCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return bReturn;
                }

                ApttusCommandBarManager.g_IsSharingActiveSheet = true;
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                Excel.Workbook oActiveWb = Globals.ThisAddIn.Application.ActiveWorkbook;

                //bReturn = ShareSelectedContentAsImage(sObjectID, sbBody, oActiveWb);
                bReturn = ShareSelectedContentAsWorkSheet(parentId, sbBody, oActiveWb, manager);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Share Highlighted Content");
            }
            finally
            {

                Globals.ThisAddIn.Application.ScreenUpdating = true;
                ApttusCommandBarManager.g_IsSharingActiveSheet = false;
            }

            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool ShareCommentsWithChatter(string parentId, string sAdditionalComments, ChatterWebBrowserManager manager)
        {

            bool bReturn = false;
            StringBuilder sbBody = new StringBuilder();

            try
            {
                ////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);

                if (!String.IsNullOrEmpty(sAdditionalComments))
                {
                    sbBody.AppendLine(sAdditionalComments);
                }

                if (sbBody.Length > 1000)
                {

                    MessageBox.Show(resourceManager.GetResource("CHATTERMGR_Post_ShowMsg"), resourceManager.GetResource("CHATTERMGR_PostCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return bReturn;
                }

                ChatterRestApi api = new ChatterRestApi(manager, oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
                bReturn = api.ShareContentViaChatter(parentId, sAdditionalComments, null, null, null);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Share Comments");
            }

            return bReturn;
        }

        /// <summary>
        /// Share the highlighted content to chatter feed
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="sTitle"></param>
        /// <param name="sFileName"></param>
        /// <param name="sBody"></param>
        /// <returns></returns>
        private static bool ShareHighlightedContentViaChatter(string parentId, string sBody)
        {
            bool bReturn = false;

            try
            {
                ////SaveResult[] SaveResultObj = new SaveResult[1];
                ////sObject[] sObjs = new sObject[1];
                ////sObject Attach = new sObject();

                /* create xmlelements for
                 * 1. ParentId
                 * 2. Body
                 * 3. Type - ContentPost
                 * */

                // Clean up data
                sBody = sBody.Replace("\v", string.Empty).Replace("\f", string.Empty).Replace("\r\a", "\r");

                System.Xml.XmlElement[] acct = new System.Xml.XmlElement[3];
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                acct[0] = doc.CreateElement("ParentId"); acct[0].InnerText = parentId;
                ////acct[1] = doc.CreateElement("Body"); acct[1].InnerText = XmlAuthorUtil.SanitizeXmlString(sBody);
                acct[2] = doc.CreateElement("Type"); acct[2].InnerText = "TextPost";

                ////Attach.type = "FeedItem";
                ////Attach.Any = acct;
                ////sObjs[0] = Attach;

                ////SaveResultObj = ApttusSessionManager.SalesForceSession.create(sObjs);
                string chatterId = "";
                string sErrorDetails = string.Empty;

                ////for (int i = 0; i < SaveResultObj.Length; i++) {
                ////    if (SaveResultObj[i].success) {
                ////        chatterId = SaveResultObj[i].id;
                ////        bReturn = true;
                ////    }
                ////    else {
                ////        Error[] errors = SaveResultObj[i].errors;

                ////        for (int k = 0; k < errors.Length; k++) {
                ////            string sFields = string.Empty;

                ////            if (errors[k].fields != null) {

                ////                for (int h = 0; h < errors[k].fields.Length; h++) {
                ////                    sFields += errors[k].fields[h].ToString();
                ////                }
                ////            }

                ////            if (sErrorDetails.Equals(string.Empty))
                ////                if (sFields.Equals(string.Empty))
                ////                    sErrorDetails = String.Format("Status Code: {0}\nError during {1}\nError Message: {2}\n", errors[k].statusCode, "Share document via Chatter", errors[k].message);
                ////                else
                ////                    sErrorDetails = String.Format("Status Code: {0}\nError during {1}\nError Message: {2}\nFields: {3}\n", errors[k].statusCode, "Share document via Chatter", errors[k].message, sFields);
                ////            else
                ////                if (sFields.Equals(string.Empty))
                ////                    sErrorDetails += String.Format("Status Code: {0}\nError during {1}\nError Message: {2}\n", errors[k].statusCode, "Share document via Chatter", errors[k].message);
                ////                else
                ////                    sErrorDetails += String.Format("Status Code: {0}\nError during {1}\nError Message: {2}\nFields: {3}\n", errors[k].statusCode, "Share document via Chatter", errors[k].message, sFields);
                ////        }
                ////    }
                ////}

                if (!sErrorDetails.Equals(string.Empty))
                {

                    //Globals.ThisAddIn.Log.error(sErrorDetails);
                    MessageBox.Show(sErrorDetails, resourceManager.GetResource("CHATTMAN_ShareHighlightedContentViaChatter_InfoMsg"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Share Highlighted Content");
            }

            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bShowConfirmation"></param>
        /// <returns></returns>
        public static bool ShareActiveWorksheetWithChatter(string parentId, string sAdditionalComments, ChatterWebBrowserManager manager)
        {

            bool bReturn = false;

            try
            {
                ////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);

                //string sBody = string.Empty;
                StringBuilder sBody = new StringBuilder();
                ApttusCommandBarManager.bCreatePDF = false;
                ApttusCommandBarManager.g_IsSharingActiveSheet = true;

                if (!String.IsNullOrEmpty(sAdditionalComments))
                {
                    sBody.AppendLine(sAdditionalComments);
                }

                if (sBody.Length > 1000)
                {

                    MessageBox.Show(resourceManager.GetResource("CHATTERMGR_Post_ShowMsg"), resourceManager.GetResource("CHATTERMGR_PostCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return bReturn;
                }

                Globals.ThisAddIn.Application.ScreenUpdating = false;

                //save the workbook in place or in the user's temporary folder
                Excel.Workbook oActiveWb = Globals.ThisAddIn.Application.ActiveWorkbook;

                Excel.Worksheet oActiveSheet = (Excel.Worksheet)oActiveWb.ActiveSheet;

                string sFileName = oActiveWb.Name;
                string sFileExt = ApttusObjectManager.GetFileNameExt(oActiveWb.Name);

                if (String.IsNullOrEmpty(sFileExt))
                {

                    sFileExt = "xlsx";
                }

                string sTempDir = ApttusObjectManager.GetTempDirectory();
                sFileName = System.IO.Path.Combine(sTempDir, ApttusObjectManager.GetFileNamePrefix(oActiveWb.Name) + "_" + oActiveSheet.Name + "." + sFileExt);

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(sFileName);

                if (fileInfo.Exists)
                    fileInfo.Delete();

                Excel.Workbook oWorkbook = Globals.ThisAddIn.Application.Workbooks.Add();
                oActiveSheet.Copy(Before: oWorkbook.Sheets[1]);

                if (sFileExt.Equals("xlsm"))
                {
                    oWorkbook.SaveAs(sFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled);
                }
                else
                {
                    oWorkbook.SaveAs(sFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                }

                string sTitle = ApttusObjectManager.GetFileNamePrefix(oWorkbook.Name);
                sFileName = oWorkbook.Name;

                byte[] fileBody = ApttusObjectManager.EncodeByteArray(sFileName, oWorkbook);
                oWorkbook.Close();

                ChatterRestApi api = new ChatterRestApi(manager, oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
                bReturn = api.ShareContentViaChatter(parentId, sAdditionalComments, fileBody, sTitle, sFileName);

            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Share Active Worksheet");
            }
            finally
            {

                Globals.ThisAddIn.Application.ScreenUpdating = true;
                ApttusCommandBarManager.g_IsSharingActiveSheet = false;
            }

            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bShowConfirmation"></param>
        /// <returns></returns>
        public static bool ShareCurrentDocumentWithChatter(string parentId, string sAdditionalComments, ChatterWebBrowserManager manager)
        {

            bool bReturn = false;

            try
            {
                //////GetUserInfoResult userInfo = (ApttusObjectManager.IsSessionValid().Equals(false) ? ApttusSessionManager.SalesForceSession.getUserInfo() : null);

                StringBuilder sBody = new StringBuilder();
                ApttusCommandBarManager.bCreatePDF = false;

                if (!String.IsNullOrEmpty(sAdditionalComments))
                {
                    sBody.AppendLine(sAdditionalComments);
                }

                if (sBody.Length > 1000)
                {

                    MessageBox.Show(resourceManager.GetResource("CHATTERMGR_Post_ShowMsg"), resourceManager.GetResource("CHATTERMGR_PostCap_ShowMsg"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return bReturn;
                }

                //save the workbook in place or in the user's temporary folder
                Excel.Workbook oActiveWb = Globals.ThisAddIn.Application.ActiveWorkbook;
                ApttusObjectManager.SaveInPlaceOrAsTempDocument(oActiveWb, oActiveWb.Name);

                string sFileName = oActiveWb.Name;
                string sTitle = ApttusObjectManager.GetFileNamePrefix(sFileName);

                //base64 encoded the document
                //sBody = ApttusObjectManager.Encode();

                ChatterRestApi api = new ChatterRestApi(manager, oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
                bReturn = api.ShareContentViaChatter(parentId, sAdditionalComments, ApttusObjectManager.EncodeByteArray(), sTitle, sFileName);
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex, true, ex.Message, "Share Current Document");
            }

            return bReturn;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckChatterFeedEnabled()
        {
            bool isEnabled = false;

            ////DescribeSObjectResult objSObject = ApttusSessionManager.SalesForceSession.describeSObject("User");

            ////foreach (Field item in objSObject.fields) {
            ////    if (item.name.ToLower().Equals("FullPhotoUrl".ToLower())) {
            ////        isEnabled = true;
            ////        break;
            ////    }
            ////}

            return isEnabled;
        }


    }
}
