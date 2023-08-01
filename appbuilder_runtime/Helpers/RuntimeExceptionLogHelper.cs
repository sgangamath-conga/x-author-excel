/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Apttus.XAuthor.AppRuntime
{
    public class RuntimeExceptionLogHelper : ExceptionLogHelper
    {
        //public static void HandleError(Exception ex)
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static void ErrorLog(Exception ex, bool showMessage, string message = "", string messageWindowCaption = "")
        {
            ApttusCommandBarManager CommandBar = ApttusCommandBarManager.GetInstance();
            ObjectManager objectManager = ObjectManager.GetInstance;

            Thread th = Thread.CurrentThread;
            string thName = th.Name != null ? th.Name : th.GetHashCode().ToString();

            try
            {
                if (ObjectManager.RuntimeMode != RuntimeMode.Batch)
                {
                    ApttusObjectManager.ShowNormalCursor();

                    // Set token value to empty, so that Manage connection correctly enables Connect & Revoke buttons
                    CommandBar.ResetToken();

                    // If this was a Salesforce Fault Exception get Exception Code
                    string exceptionCode = objectManager.GetExceptionCode(ex);
                    if (!string.IsNullOrEmpty(exceptionCode))
                    {
                        switch (exceptionCode)
                        {
                            case Constants.SF_INVALID_SESSION_ID:
                                bool showLogoutBalloon = true;
                                if (ex.Source != "LOGOFF")
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                        resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExpTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionExp_InfoMsg"), ToolTipIcon.Warning);
                                    Globals.ThisAddIn.apttusNotification.Tag = "SESSION_EXPIRED";
                                    showLogoutBalloon = false;
                                }
                                CommandBar.DoLogout(showLogoutBalloon);
                                if (Globals.ThisAddIn.RuntimeRibbon.MenuBuilder != null)
                                {
                                    Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateMenus(Globals.ThisAddIn.RuntimeRibbon, true);
                                    Globals.ThisAddIn.RuntimeRibbon.MenuBuilder.InvalidateRibbon(Globals.ThisAddIn.RuntimeRibbon);
                                }
                                break;
                            case Constants.SF_INVALID_LOGIN:
                                if (ex.GetHashCode() == 54875957)
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                        resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSetting_InfoMsg"), ToolTipIcon.Warning);
                                }
                                else
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                        resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogLoginCredTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogLoginCred_InfoMsg"), ToolTipIcon.Warning);
                                }
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
                                        Globals.ThisAddIn.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                        exceptionCode, String.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogExc_InfoMsg"), exceptionCode, httpEx.Message, resourceManager.CRMName), ToolTipIcon.Error);
                                    }
                                }
                                break;
                            default:
                                Globals.ThisAddIn.apttusNotification.ShowBalloonTip(Constants.BALLOON_TIMEOUT,
                                    exceptionCode, String.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogExc_InfoMsg"), exceptionCode, message,resourceManager.CRMName), ToolTipIcon.Warning);
                                break;
                        }
                        // Don't show a pop-up message in finally as Balloon Notification is shown above for SF Exception Code
                        showMessage = false;
                    }

                    switch (ex.GetType().ToString())
                    {
                        case "System.Net.WebException":
                        case "System.ServiceModel.CommunicationException":
                        case "System.ServiceModel.EndpointNotFoundException":
                            {
                                if (ex.Message.Contains("The request failed with HTTP status 404: Not Found."))
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLog404_InfoMsg"), ToolTipIcon.Warning);
                                    Globals.ThisAddIn.apttusNotification.Tag = "OPTIONS_DIALOG";
                                }
                                else if (ex.InnerException.Message.Contains("Unable to connect to the remote server") || ex.InnerException.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect to the remote server") || ex.Message.Contains("The underlying connection was closed"))
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogConnFailedTitle_InfoMsg"), string.Format(resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogConnFailed_InfoMsg"),resourceManager.CRMName), ToolTipIcon.Error);
                                    CommandBar.DoLogout(false);
                                }
                                else if (ex.Message.Contains("Proxy Authentication Required."))
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogProxyTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogProxy_InfoMsg"), ToolTipIcon.Error);
                                }
                            }
                            break;

                        default:
                            {
                                if (ex.GetHashCode() == 52136803)
                                {
                                    if (CommandBar.IsLoggedIn())
                                    {
                                        CommandBar.DoLogout(true);
                                    }
                                }
                                else if (ex.Message.Equals("Not found"))
                                {
                                    Globals.ThisAddIn.apttusNotification.ShowBalloonTip(1000, resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLogSessionSettingTitle_InfoMsg"), resourceManager.GetResource("RUNTIMEEXCELOGHELP_ErrorLog404_InfoMsg"), ToolTipIcon.Warning);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                if (CommandBar.IsLoggedIn())
                    if (thName.ToUpper().Equals("VSTA_MAIN"))
                        ExceptionLogHelper.ErrorLog(ex, showMessage, message, messageWindowCaption);
            }
            finally
            {
                ExceptionLogHelper.ErrorLog(ex, showMessage, message, messageWindowCaption);
            }
        }
    }
}
