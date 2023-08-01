/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner.Modules;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using Apttus.XAuthor.Core.Model.GoogleActions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;


namespace Apttus.XAuthor.AppDesigner
{
    /* This class will generate data in json which represents everything needed for google sheets
     * the json data will be stored as an attachment with app
     */
    public class GoogleSchemaUtil
    {
        List<googleWorkflow> googleWorkFlows = new List<googleWorkflow>(); //list of all google workflows
        Dictionary<string, googleAction> standAloneActions = new Dictionary<string, googleAction>(); //Maps action Ids to actions that can be executed on its own
        GoogleAppConfig gConfig = new GoogleAppConfig();//total package for everything for google sheets
        private Dictionary<string, string> actionInOutMap = new Dictionary<string, string>(); //dictionary to help map the input and output names
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        //List<WorkflowActionData> OutPutNames = new List<WorkflowActionData>();
        //ObjectManager objectManager = ObjectManager.GetInstance;
        byte[] Config;
        bool useSubQuery = false;
        public GoogleSchemaUtil()
        {
        }
        /* 
         * Generates json for for google sheets. Contains much of the same information in appDefinition.xml 
         */
        public Byte[] GetGoogleSchema()
        {
            Microsoft.Office.Core.COMAddIn runtimeAddIn = Globals.ThisAddIn.GetRuntimeAddin(); //get the runtimeAddin         
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (runtimeAddIn != null)
            {
                List<SaveMap> SaveMap = configurationManager.SaveMaps; //list of all savemaps
                List<Workflow> AllWorkflows = configurationManager.Application.Workflows; //get all the workflows 
                List<WorkflowStructure> AllWorkFlowStructures = new List<WorkflowStructure>();
                gConfig.AppMenu = configurationManager.Menus; //runtime menu data
                gConfig.googleActionFlows = googleWorkFlows; //all workflows

                //add all the workflows to googleworkflow
                for (int i = 0; i < AllWorkflows.Count(); i++)
                {
                    AllWorkFlowStructures.Add((WorkflowStructure)AllWorkflows[i]);
                    googleWorkflow workflow = new googleWorkflow((AllWorkFlowStructures[i]).Id.ToString());
                    googleWorkFlows.Add(workflow);
                }
                
                for (int i = 0; i < AllWorkFlowStructures.Count(); i++) //go through the workflow structures and add all actions to the correct google workflow
                {
                    for (int stCnt = 0; stCnt < AllWorkFlowStructures[i].Steps.Count; stCnt++)
                    {

                        for (int condCnt = 0; condCnt < AllWorkFlowStructures[i].Steps[stCnt].Conditions.Count; condCnt++)
                        {
                            for (int actionCnt = 0; actionCnt < AllWorkFlowStructures[i].Steps[stCnt].Conditions[condCnt].WorkflowActions.Count; actionCnt++)
                            {
                                if (AllWorkFlowStructures[i].Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].Type.Equals("Action"))
                                {
                                    string sActionId = AllWorkFlowStructures[i].Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt].ActionId;
                                    Core.Action objAction = configurationManager.GetActionById(sActionId);
                                    ActionResult res = DoAction(objAction, AllWorkFlowStructures[i].Steps[stCnt].Conditions[condCnt].WorkflowActions[actionCnt], i, runtimeAddIn);                               
                                }
                            }
                        }
                    }              
                }   
            }
            // select all app objects
            List<ApttusObject> pickListObject = configurationManager.Definition.AppObjects;
            /*
            List<ApttusObject> pickListObject = (from appObj in configurationManager.Definition.AppObjects
                                                 from c in appObj.Fields
                                                 where c.Datatype == Datatype.Picklist
                                                 select appObj).ToList();
            */
            List<ApttusObject> ObjAndPLFields = new List<ApttusObject>();
            List<ApttusObject> childObjAndPLFields = new List<ApttusObject>();

            //look for multi picklists
            List<ApttusObject> allMultiFields = (from appObj in configurationManager.Definition.AppObjects 
                                                 from c in appObj.Fields
                                                 where c.Datatype == Datatype.Picklist_MultiSelect
                                                 select appObj).ToList();
            List<ApttusObject> multiObjAndPLFields = new List<ApttusObject>();
            
            foreach (ApttusObject obj in pickListObject) //make a copy of the app objects (which includes the parent obj picklist data)
            {
                ApttusObject tempObj = new ApttusObject();
                tempObj.UniqueId = obj.UniqueId;
                List<ApttusField> flds = obj.Fields;
                tempObj.Fields = flds;
                ObjAndPLFields.Add(tempObj);
            }

            if (allMultiFields.Count() > 0) //if there are multi picklists, grab the data and put into schema
            {
                gConfig.hasMultiPicklist = true;
                Dictionary<string, List<string>> multiData = new Dictionary<string, List<string>>();
                foreach (ApttusObject obj in allMultiFields) //go through all the multi data
                {
                    ApttusObject tempObj = new ApttusObject();
                    tempObj.UniqueId = obj.UniqueId;
                    List<ApttusField> flds = obj.Fields.FindAll(item => item.Datatype == Datatype.Picklist_MultiSelect).ToList(); //seperate the fields that are multi picklists
                    tempObj.Fields = flds;
                    multiObjAndPLFields.Add(tempObj);
                }
                List<RetrieveField> multirFields = (from rField in configurationManager.RetrieveMaps[0].RepeatingGroups[0].RetrieveFields //now search the rMap for the picklist data. assumes only one rMap
                                                    where (rField.DataType == Datatype.Picklist_MultiSelect && rField.AppObject == multiObjAndPLFields[0].UniqueId)
                                                    select rField).ToList();
                foreach (RetrieveField rField in multirFields) //for each rfield that has multi picklist data
                {
                    List<List<string>> plValues = (from field in multiObjAndPLFields[0].Fields //get the pl values
                                                           where field.Id == rField.FieldId
                                                           select field.PicklistValues).ToList();
                    multiData.Add(convertTargetLocationToColIndex(rField.TargetLocation.ToString()), plValues[0]); //store in a dict
                }
                gConfig.multiPicklistData = multiData;
            }


            Dictionary<string, string> fieldToLocations = new Dictionary<string, string>();
            
            if (configurationManager.Definition.AppObjects[0].Children.Count() > 0)  //if picklist information is required for both parent and child objects
            {
                childObjAndPLFields = (from appObj in configurationManager.Definition.AppObjects[0].Children
                                       select appObj).ToList();
                                      

                /*
                List<ApttusObject> secondPickListObject = (from appObj in configurationManager.Definition.AppObjects[0].Children
                                                           from c in appObj.Fields
                                                           //where c.Datatype == Datatype.Picklist
                                                           select appObj).ToList();

                foreach (ApttusObject obj in secondPickListObject) //if there is a child object, get the child object data too
                {
                    ApttusObject tempObj = new ApttusObject();
                    tempObj.UniqueId = obj.UniqueId;
                    List<ApttusField> flds = obj.Fields.FindAll(item => item.Datatype == Datatype.Picklist).ToList();
                    tempObj.Fields = flds;
                    childObjAndPLFields.Add(tempObj);
                }
                 */
            }
             
            List<RetrieveField> rfields = configurationManager.RetrieveMaps[0].RepeatingGroups[0].RetrieveFields;
            foreach (RetrieveField field in rfields) //create a map of fields and field types used in the rMap
            {
                if (field.DataType == Datatype.Picklist)
                {
                    int start = field.TargetLocation.IndexOf("$");
                    int end = field.TargetLocation.LastIndexOf("$");
                    fieldToLocations.Add(field.FieldId, field.TargetLocation.Substring(start + 1, end - start - 1));
                }
            }

            Dictionary<string, string> colorSettings = GetColorInfo(); //get the color settings from the quick app settings            
            gConfig.colorSettings = colorSettings;
            gConfig.AppObjects = ObjAndPLFields; //add the picklist data to google data
            gConfig.ChildAppObjects = childObjAndPLFields; //add child picklist data (will add null if there is none)
            gConfig.controllingFieldToLocation = fieldToLocations;
            string stringofplData = js.Serialize(ObjAndPLFields);
            //string json = js.Serialize(sch);
            string json = js.Serialize(gConfig);
            //return json;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(json);
            return bytes;
        }

        /// <summary>
        /// takes in a designer location in A1 notation. Returns the column index for that location as a string
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        private string convertTargetLocationToColIndex(string targetLocation)
        {
            int start = targetLocation.IndexOf("$");
            int end = targetLocation.LastIndexOf("$");
            string s = targetLocation.Substring(start+1, end - start - 1);
            string returnNum = ExcelColumnNameToNumber(s);
            return returnNum;
        }

        /// <summary>
        /// takes in a letter and returns the column index
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum.ToString();
        }

        /// <summary>
        /// Gets the color information from the quick app settings. Returns a dict with each value as hex
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetColorInfo()
        {
            Dictionary<string, string> colorSettings = new Dictionary<string, string>();            
            // Display only fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsColor))
            {
                colorSettings.Add("dispFillCol", aargbToHex(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsColor]));
                //Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsColor]
            }          
      
            // Save fields fill color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsColor))
            {
                colorSettings.Add("saveFillCol", aargbToHex(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsColor]));                
            }
            // Display only fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.DisplayFieldsTextColor))
            {
                colorSettings.Add("dispFontCol", aargbToHex(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.DisplayFieldsTextColor]));                
            }                
            // Save fields text color
            if (Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(ApttusGlobals.SaveFieldsTextColor))
            {
                colorSettings.Add("saveFontCol", aargbToHex(Globals.ThisAddIn.applicationConfigManager.ApplicationSettings.AppExtendedProperties[ApttusGlobals.SaveFieldsTextColor]));                
            }                
            return colorSettings;
        }
        /// <summary>
        /// Takes a color in aargb format and returns the string representation in hex
        /// </summary>
        /// <param name="num"></param>
        /// <returns>hex value as a string</returns>
        private string aargbToHex(string num)
        {
            System.Drawing.Color c = System.Drawing.Color.FromArgb(Convert.ToInt32(num));
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }


        /*
         * TODO: clean up each if condition into its own method. QueryAction - remove string parsing for child object and use app object instead 
         *      
         * Prepares information needed to execute an action, but stops before actual execution. Stores data into a googleAction which is then added
         * to the apropriate googleWorkflow. Currently only query, retrieve, and save are supported
         */
        public ActionResult DoAction(Core.Action objAction, Core.WorkflowAction wfAction, int i, Microsoft.Office.Core.COMAddIn runtimeAddIn)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string[] inputDataNames = null;
            List<string> pair = new List<string>();
            pair.Add(i.ToString());
            pair.Add(gConfig.googleActionFlows[i].actionList.Count().ToString());
            //Dictionary<string, string> pair = new Dictionary<string,string>();
            //pair.Add(i.ToString(), gConfig.googleActionFlows[i].actionList.Count().ToString());
            if (!gConfig.getActionByIdMap.ContainsKey(objAction.Id))
                gConfig.getActionByIdMap.Add(objAction.Id, pair);
            googleAction action; 
            string targetObjectId = "";
            if(!String.IsNullOrEmpty(wfAction.WorkflowActionData.OutputDataName))
            {
                actionInOutMap.Add(wfAction.WorkflowActionData.Id.ToString(), wfAction.WorkflowActionData.OutputDataName);
            }            
            ActionResult actionRes = null;
            try
            {                
                if (objAction.GetType() == typeof(Core.SearchAndSelect))
                {
                    string strInputDataNames = js.Serialize(inputDataNames);
                    List<string> inputDataName = resolveInputOutputName(wfAction.WorkflowActionData.InputDataName, actionInOutMap);

                    bool currentSetting = configurationManager.Definition.AppSettings.SuppressDependent;
                    if(currentSetting == false) //set messages to suppressed
                    {
                        configurationManager.Definition.AppSettings.SuppressDependent = true;
                    }

                    action = createGoogSSAction(runtimeAddIn, objAction, wfAction.WorkflowActionData.OutputDataName, inputDataName, strInputDataNames);
                    googleWorkFlows[i].addAction(action);

                    if(!currentSetting) //if messages were suppressed, revert change
                    {
                        configurationManager.Definition.AppSettings.SuppressDependent = currentSetting;
                    }
                    /*
                    //get information for runtime addin
                    string configxml = ApttusXmlSerializerUtil.Serialize<Application>(configurationManager.Application);
                    string actionId = objAction.Id;
                    string strInputDataNames = js.Serialize(inputDataNames);

                    string uniqueAppId = configurationManager.Definition.UniqueId;
                    string instance_url = Globals.ThisAddIn.oAuthWrapper.token.instance_url;
                    string access_token = Globals.ThisAddIn.oAuthWrapper.token.access_token;

                    //get search and select data for google sheets
                    Dictionary<string, object> ssData = runtimeAddIn.Object.GetSearchAndSelectData(actionId, strInputDataNames, uniqueAppId, instance_url, access_token, configxml);                    

                    List<string> inputDataName = resolveInputOutputName(wfAction.WorkflowActionData.InputDataName, actionInOutMap);                    

                    action = new googleAction(objAction, wfAction, ssData, inputDataName);                    
                    googleWorkFlows[i].addAction(action);
                     */
                }

                else if (objAction.GetType() == typeof(Core.RetrieveActionModel)) //saves the rMap from the config manager. If parent child app, saves the child object id
                {
                    RetrieveMap rMap = configurationManager.RetrieveMaps.Where(retrieveMap => retrieveMap.Id.Equals(((Core.RetrieveActionModel)objAction).RetrieveMapId)).FirstOrDefault();                    
                    if (rMap != null)
                    {                                                
                        string sheet = rMap.RepeatingGroups[0].RetrieveFields[0].TargetLocation;
                        sheet = sheet.Substring(0, sheet.IndexOf("!"));
                        Microsoft.Office.Interop.Excel._Worksheet excelSheet = ExcelHelper.GetWorkSheet(sheet);
                        string title = excelSheet.Cells[1, 1].value2;
                        if(title != null)
                        {
                            gConfig.workSheetTitle = title;
                        } 
                        
                        List<string> inputDataName = resolveInputOutputName(wfAction.WorkflowActionData.InputDataName, actionInOutMap);
                        List<string> lookAheadSSActions;
                        action = new googleAction(gConfig, (Core.RetrieveActionModel)objAction, rMap, inputDataName, out lookAheadSSActions);

                        if(lookAheadSSActions.Count > 0) //if there is are look ahead fields
                        {
                            gConfig.standAloneActions = new Dictionary<string, googleAction>();
                            //create a google ss action. Add the new action to the config and map it to the current action's lookup fields
                            foreach(string ssActionId in lookAheadSSActions)
                            {
                                Core.Action ssAction = configurationManager.GetActionById(ssActionId);                                
                                googleAction googSSAction = createGoogSSAction(runtimeAddIn, ssAction, "", null, "");
                                gConfig.standAloneActions.Add(ssActionId, googSSAction);
                            }                            
                        }

                        googleWorkFlows[i].addAction(action);
                                  
                    }
                }
                else if (objAction.GetType() == typeof(Core.SaveActionModel))//add the save map to the schema
                {
                    SaveMap smap = new SaveMap();
                    smap = configurationManager.SaveMaps.SingleOrDefault(m => m.Id == ((Core.SaveActionModel)objAction).SaveMapId);
                    action = new googleAction(objAction.Id.ToString(), smap);

                    Dictionary<string, List<Dictionary<string, string>>> googSaveMap = new Dictionary<string, List<Dictionary<string, string>>>();
                    if(smap != null)
                    {
                        if(smap.SaveFields.Count() > 0)
                        {
                            Dictionary<string, int> saveFieldColIndex = new Dictionary<string, int>();
                            Dictionary<string, int> parentSaveFieldColIndex = new Dictionary<string, int>();    
                            //need to map each sObject to a group of save fields. each field needs to know its own fieldId and data type
                            List<ApttusObject> appObjects = configurationManager.Definition.AppObjects;                            
                            foreach(Core.SaveField saveField in smap.SaveFields)
                            {
                                //configurationManager.GetAllAppFields()
                                List<ApttusObject> appObjList = (from appObj in appObjects
                                                                 where appObj.UniqueId == saveField.AppObject
                                                                 select appObj).ToList();
                                ApttusObject saveFieldAppObj = null;
                                if(appObjList.Count > 0) //grabs parent obj
                                {
                                    saveFieldAppObj = appObjList[0];
                                }
                                else //check if there is a child appobject (only works if looking at a child object
                                {
                                    saveFieldAppObj = (from appObj in appObjects
                                                       from child in appObj.Children
                                                       where child.UniqueId == saveField.AppObject
                                                       select child).FirstOrDefault();
                                }
                                if (saveFieldAppObj != null)
                                {
                                    if(saveFieldAppObj.Children.Count > 0) //if there are children, then it is an individual object
                                    {
                                        action.parentQueryObjectName = saveFieldAppObj.Id;
                                    }                                                                                                          

                                    string saveFieldId = saveField.FieldId;
                                    string designerLocation = saveField.DesignerLocation;
                                    List<ApttusField> saveFields = (from field in saveFieldAppObj.Fields
                                                                        where field.Id == saveField.FieldId
                                                                        select field).ToList();
                                    string dataType = null;
                                    List<string> sObjectIds = null;
                                    string sObjectId = saveFieldAppObj.Id;
                                    string lookUpObjId = null;

                                    if(saveFields != null && saveFields.Count > 0)
                                    {
                                        if(saveFields[0].LookupObject != null) //field is name of a lookup field
                                        {
                                            //use xa sheets lookup to indicate the need to resolve the name of the lookupfield
                                            dataType = "xasLookup";

                                            //get the lookup object
                                            sObjectIds = (from field in saveFieldAppObj.Fields
                                                          where field.Id == saveFieldId
                                                          select field.LookupObject.Id).ToList();
                                            if (sObjectIds.Count > 0)
                                            {
                                                lookUpObjId = sObjectIds[0];
                                            }
                                        }
                                        else //use field's data type
                                        {
                                            dataType = "xa"+saveFields[0].Datatype.ToString();
                                        }
                                    }                                                                                                                                                                                                                      

                                    List<Dictionary<string, string>> sMapData = null;
                                    Dictionary<string, string> saveFieldData = new Dictionary<string, string>();
                                    saveFieldData.Add("sObjectId", lookUpObjId);
                                    saveFieldData.Add("saveFieldId", saveFieldId);
                                    saveFieldData.Add("designerLocation", designerLocation);
                                    saveFieldData.Add("dataType", dataType);
                                    saveFieldData.Add("appObject", saveField.AppObject.ToString());
                                    saveFieldData.Add("relativeCol", saveField.TargetColumnIndex.ToString());
                                    List<string> saveGroupTargetRange = new List<string>(); 
                                    if(smap.SaveGroups.Count == 1)
                                    {
                                        saveGroupTargetRange.Add(smap.SaveGroups[0].TargetNamedRange);
                                    }
                                    else
                                    {
                                        saveGroupTargetRange = (from svgrp in smap.SaveGroups
                                                                where svgrp.GroupId == saveField.GroupId
                                                                select svgrp.TargetNamedRange).ToList();
                                    }                                    
                                    string targetRange = saveGroupTargetRange[0];

                                    //create a list of save fields with the necessary information
                                    if (googSaveMap.TryGetValue(sObjectId + "," + targetRange, out sMapData)) //object already added,
                                    {                                                                                                                       
                                        sMapData.Add(saveFieldData);
                                    }
                                    else //brand new sObject
                                    {                                        
                                        sMapData = new List<Dictionary<string, string>>();
                                        sMapData.Add(saveFieldData);
                                        Dictionary<string, string> idFieldData = new Dictionary<string, string>();

                                        foreach(RetrieveMap rMap in configurationManager.RetrieveMaps)
                                        {
                                            List<RetrieveField> idFields = (from rField in rMap.RepeatingGroups[0].RetrieveFields
                                                                             where ( rMap.RepeatingGroups[0].TargetNamedRange.Equals(targetRange) && rField.FieldId.Equals("Id") )
                                                                             select rField).ToList();
                                            if (idFields.Count > 0)
                                            {
                                                idFieldData.Add("saveFieldId", "Id");
                                                idFieldData.Add("designerLocation", idFields[0].TargetLocation);
                                                idFieldData.Add("dataType", "String");
                                                idFieldData.Add("relativeCol", idFields[0].TargetColumnIndex.ToString());
                                                sMapData.Add(idFieldData);
                                            }                                            
                                        }
                                        googSaveMap.Add(sObjectId + "," + targetRange, sMapData);
                                    }


                                }
                                if (saveField.Type == ObjectType.Repeating)
                                {
                                    int someInt = 0;
                                    if (!saveFieldColIndex.TryGetValue(saveField.FieldId, out someInt))
                                    {
                                        saveFieldColIndex.Add(saveField.FieldId, Convert.ToInt32(convertTargetLocationToColIndex(saveField.DesignerLocation)));
                                    }
                                }
                                else
                                {
                                    parentSaveFieldColIndex.Add(saveField.FieldId, Convert.ToInt32(convertTargetLocationToColIndex(saveField.DesignerLocation)));
                                }
                            }
                            //action.saveFieldToColIndex = saveFieldColIndex;
                            //gConfig.saveFieldToColIndex = saveFieldColIndex;
                            gConfig.saveFieldToColIndex = saveFieldColIndex;
                            action.parentSaveFieldColIndex = parentSaveFieldColIndex;
                            action.googSaveMap = googSaveMap;
                        }
                    }                    
                    googleWorkFlows[i].addAction(action);
                }
                else if (objAction.GetType() == typeof(Core.QueryActionModel))
                {             
                    string configxml = ApttusXmlSerializerUtil.Serialize<Application>(configurationManager.Application);

                    string actionId = objAction.Id;
                    string strInputDataNames = js.Serialize(inputDataNames);
                 
                    string uniqueAppId = configurationManager.Definition.UniqueId;
                    string instance_url = string.Empty;
                    string access_token = string.Empty;                    

                    string queryString = runtimeAddIn.Object.GetQueryActionString(actionId, strInputDataNames, uniqueAppId, instance_url, access_token, configxml);//get the query string from the runtime
                    if(queryString == null) //failed to generate query
                    {
                        ApttusResourceManager resourceManager =  ApttusResourceManager.GetInstance;
                        ApttusMessageUtil.ShowError(resourceManager.GetResource("GOOGSCHEMA_InvalidQuery_Msg"), resourceManager.GetResource("GOOGSCHEMA_InvalidQuery_Cap"));
                        return actionRes;
                    }
                    string outputName = "";
                    int fromIndex = queryString.IndexOf("FROM");
                    string childQueryObject = queryString.Substring(fromIndex + 5);
                    string[] childObjectList = childQueryObject.Split(' ');
                    childQueryObject = childObjectList[0];

                    gConfig.childQueryObject = childQueryObject;
                    if (wfAction.WorkflowActionData.OutputPersistData)
                    {
                        outputName = wfAction.WorkflowActionData.OutputDataName;
                    }
                    action = new googleAction(objAction.Id.ToString(), queryString, outputName, ((QueryActionModel)objAction).WhereFilterGroups);

                    ApplicationDefinitionManager appdefman = ApplicationDefinitionManager.GetInstance;
                    ApttusObject apttusObject = appdefman.GetAppObject(((QueryActionModel)objAction).TargetObject);
                    action.queryObject = apttusObject.Id.ToString();
                    action.inputDataName = resolveInputOutputName(wfAction.WorkflowActionData.InputDataName, actionInOutMap);

                    List<string> filters = new List<string>();
                    string filterLogicText = "";
                    //check if need to pre-empt workflow by prompting for user input


                    gConfig.getUserInput = action.getUserInputFilters((Core.QueryActionModel)objAction, out filterLogicText, apttusObject);

                    if (gConfig.getUserInput == true)//save user input filters to schema
                    {
                        gConfig.userInputWorkflowsandActions.Add(gConfig.googleActionFlows[i].workflowID.ToString());
                        gConfig.userInputWorkflowsandActions.Add(objAction.Id.ToString());
                        gConfig.filterLogicText = filterLogicText;
                    }
                    targetObjectId = ((Core.QueryActionModel)objAction).TargetObject.ToString();
                    googleWorkFlows[i].addAction(action);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true, ex.Message, ApttusResourceManager.GetInstance.GetResource("COMMON_ActionFlow_Text"));
            }                        
            return actionRes;
        }
        public List<string> resolveInputOutputName(string[] inputNames, Dictionary<string, string> inputOutputMap)
        {
            List<string> inputNamesForAction = new List<string>();
            
            if ( (null != inputNames && inputNames.Length > 0) && (null != inputOutputMap && inputOutputMap.Count > 0) )
            {
                string outPutName = "";
                foreach (string inputName in inputNames)
                {
                    actionInOutMap.TryGetValue(inputName, out outPutName);
                    inputNamesForAction.Add(outPutName);
                }
            }
            return inputNamesForAction;
        }

        public googleAction createGoogSSAction(Microsoft.Office.Core.COMAddIn runtimeAddIn, Core.Action objAction, string outputDataName, List<string> inputDataName, string strInputDataNames)
        {
            if(objAction.GetType() == typeof(Core.SearchAndSelect))
            {
                //get information for runtime addin
                string configxml = ApttusXmlSerializerUtil.Serialize<Application>(configurationManager.Application);
                string actionId = objAction.Id;

                string uniqueAppId = configurationManager.Definition.UniqueId;
                string instance_url = string.Empty;
                string access_token = string.Empty;
                //get search and select data for google sheets
                Dictionary<string, object> ssData = runtimeAddIn.Object.GetSearchAndSelectData(actionId, strInputDataNames, uniqueAppId, instance_url, access_token, configxml);                

                return new googleAction(objAction, outputDataName, ssData, inputDataName);
            }
            else
            {
                return null;
            }
        }
    }
}