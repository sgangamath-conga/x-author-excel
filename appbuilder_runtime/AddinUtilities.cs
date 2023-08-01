using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Apttus.XAuthor.AppRuntime
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAddinUtilities
    {
        void DisplayMessage(string msx);
        void LoadApplicationWithParam(string appParams);
        string GetQueryActionString(string obj, string inputDataNames, string uniqueAppId, string instance_url, string access_token, string configxml);
        void prepRunTime(string access_token, string instance_url, string uniqueAppId, string configxml);
        Dictionary<string, object> GetSearchAndSelectData(string obj, string inputDataNames, string uniqueAppId, string instance_url, string access_token, string configxml);       
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AddinUtilities :
        StandardOleMarshalObject,
        IAddinUtilities
    {
        void IAddinUtilities.DisplayMessage(string msg)
        {
            try
            {
                string[] inVal = new string[2];
                inVal[0] = msg;
                Globals.ThisAddIn.Application.StatusBar = "Please wait ....";
                Globals.ThisAddIn.CallEditinExcel(inVal);
                Globals.ThisAddIn.Application.StatusBar = "Done";
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        void IAddinUtilities.LoadApplicationWithParam(string appParams)
        {
            try
            {
                string[] inParams = new string[2];
                inParams[0] = appParams;
                Globals.ThisAddIn.Application.StatusBar = "Please wait ....";
                Globals.ThisAddIn.LoadAppWithParam(inParams);
                Globals.ThisAddIn.Application.StatusBar = "Done";
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
        }

        private static IAdapter adapter;

        //returns the querystring for a query action.
        string IAddinUtilities.GetQueryActionString(string obj, string inputDataNames, string uniqueAppId, string instance_url, string access_token, string configxml)
        {
            try
            {
                prepRunTime(access_token, instance_url, uniqueAppId, configxml);
                GoogleActionQueryController GQA = new GoogleActionQueryController(obj, inputDataNames);
                return GQA.Execute();
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return string.Empty;
        }

        //gets SS data for google sheets. Returns a dict with the soqlQuery, SearchFields and DisplayFields (doesn't currently work)
        Dictionary<string, object> IAddinUtilities.GetSearchAndSelectData(string obj, string inputDataNames, string uniqueAppId, string instance_url, string access_token, string configxml)
        {
            Dictionary<string, object> ssData = new Dictionary<string, object>();

            try
            {
                prepRunTime(access_token, instance_url, uniqueAppId, configxml);
                GoogleSearchAndSelectActionController gssAction = new GoogleSearchAndSelectActionController(obj, inputDataNames);
                gssAction.Execute();
                ssData["parentLookupId"] = gssAction.parentLookupId;
                ssData["parentObjectName"] = gssAction.parentObjectName;
                ssData["soqlQuery"] = gssAction.soqlQuery;
                ssData["parentSOQLQuery"] = gssAction.parentSOQLQuery;
                ssData["searchFields"] = gssAction.searchFields;
                ssData["displayFields"] = gssAction.displayFields;
                ssData["fieldTypes"] = gssAction.fieldTypes;
                ssData["recordType"] = gssAction.recordType;
                ssData["appObjId"] = gssAction.appObjId;
            }
            catch (Exception ex)
            {
                RuntimeExceptionLogHelper.ErrorLog(ex);
            }
            return ssData;
        }

        //helper method for getting query actions and ssData
        public void prepRunTime(string access_token, string instance_url, string uniqueAppId, string configxml)
        {
            //TODO - revisit it later.
			//adapter = new ABSalesforceAdapter();
            //adapter.Connect(access_token, instance_url,null);
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //Core.ApplicationController appController = new ApplicationController();
            //string appId = appController.GoogleGetAppIdByUniqueId(uniqueAppId);
            //ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
            //Core.Application app = ApttusXmlSerializerUtil.Deserialize<Core.Application>(configxml);
            //ApplicationDefinitionManager.GetInstance.AppObjects = app.Definition.AppObjects;
            //configurationManager.SetApplication(app);
        }
    }
}

