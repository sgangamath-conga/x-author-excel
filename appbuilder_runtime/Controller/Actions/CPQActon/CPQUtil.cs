/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
namespace Apttus.XAuthor.AppRuntime.Controller.Actions.CPQActon
{
    /* used for A4A navigation */
    public class CPQUtil
    {

        private Apttus.WebBrowserControl.WebBrowserControl browser;
        public const string CPQURL = "https://connect-drive-3393--apttus-qpconfig.na15.visual.force.com";

        private Dictionary<string, string> pageParams = new Dictionary<string, string>();
        public const string CPQPAge = "SprintCPQConsole";
        protected XAuthorSalesforceLogin oAuthWrapper;
        private string FrontDoorURLPrefix = "/secur/frontdoor.jsp?sid=";
        private Apttus.WebBrowserControl.WebBrowserWindowUserControl brw;
        private Apttus.WebBrowserControl.ExtendedWebBrowser brwE;
        private Apttus.WebBrowserControl.ExtendedWebBrowserPopup pop;
        public CPQUtil()
        {
            oAuthWrapper = Globals.ThisAddIn.GetLoginObject() as XAuthorSalesforceLogin;
        }
        public string OppId
        {
            get;
            set;
        }
        public string PriceListName
        {
            get;
            set;
        }
        public string CartId
        {
            get;
            set;
        }

        public string GetEditURL(string InstanceURL, string SessionId, string QuoteId)
        {
            // string QuoteId = null;
            // string QuoteId2 = null;
            String sRetURL = null;
            String urlString = InstanceURL + FrontDoorURLPrefix + System.Web.HttpUtility.UrlEncode(SessionId);
            if (CartId != null) // edit cart
            {
                //https://connect-drive-3393--apttus-config2.na15.visual.force.com/apex/CartDetailView?id=a1qi0000000rLJNAA2&method=quickAdd&
                //configRequestId=a2Ki0000000M50NEAS
                //  sRetURL = InstanceURL + "/apex/CartDetailView?id=" + QuoteId + "&method=quickAdd&configRequestId=" + CartId;
                sRetURL = InstanceURL + "/apex/Apttus_QPConfig__ProposalConfiguration?id=" + CartId;
                ///apex/Apttus_QPConfig__ProposalConfiguration?id=" &Id


            }
            else
                sRetURL = InstanceURL + "//" + QuoteId;
            urlString += "&retURL=" + System.Web.HttpUtility.UrlEncode(sRetURL);

            return urlString;


        }

        public static string GetRuntimeDir()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }
        public static void ResultCallback(int lineCount)
        {
            Console.WriteLine(
                "Independent task printed {0} lines.", lineCount);
        }
        public string LoadCtrl(string InstanceURL, string SessionId)
        {

            string oppId = null;
            string PLName = null;

            // "/apex/Apttus_QPConfig__OpptyQuoteCreate?id=006i000000VfDjX&recordTypeName=Proposal&PriceListName=Tier 1 Software US&method=quickAdd&useAdvancedApproval=false&defaultQuantity=1&defaultSellingTerm=1";//"/apex/Apttus_SprntCPQ__SprintCPQConsole";
            String urlString = InstanceURL + FrontDoorURLPrefix + System.Web.HttpUtility.UrlEncode(SessionId);
            // String urlString = "https://connect-drive-3393--apttus-sprntcpq.na15.visual.force.com" + FrontDoorURLPrefix + System.Web.HttpUtility.UrlEncode(SessionId);
            if (OppId != null)
            {
                oppId = OppId;
                PLName = PriceListName;
            }
            else
            {
                Excel._Worksheet ws = ExcelHelper.GetWorkSheet("CPQMeta");

                oppId = ws.Cells[4, 3].value2; //Globals.ThisAddIn.Application.ActiveSheet.cells(7,25).value2;
                PLName = ws.Cells[5, 3].value2; //Globals.ThisAddIn.Application.ActiveSheet.cells(8,25).value2;
            }

            String sRetURL = InstanceURL + "/apex/Apttus_QPConfig__OpptyQuoteCreate?id=" + oppId + "&recordTypeName=Proposal&PriceListName=" + PLName + "&method=quickAdd&useAdvancedApproval=false&defaultQuantity=1&defaultSellingTerm=1";//"/apex/Apttus_SprntCPQ__SprintCPQConsole";
            urlString += "&retURL=" + System.Web.HttpUtility.UrlEncode(sRetURL);

            return urlString;

        }



        //  private Apttus.WebBrowserControl.ExtendedWebBrowser browser; 
        public void StartQuote()
        {
            LoadCtrl(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
        }

        public static string RemoeVectorString(string val)
        {
            // for some reason the return value contains "Vector smash protection is enabled" string and need to remove it. 
            //Vector smash protection is enabled.a0ii000000I1znrAAB
            const string stupidVectorString = "Vector smash protection is enabled";
            if (!string.IsNullOrEmpty(val))
            {
                if (val.Contains(stupidVectorString))
                {
                    return val.Split('.')[1];
                }
            }
            //Vector smash protection is enabled.a0ii000000I1znrAAB
            return val;
        }
    }

}
