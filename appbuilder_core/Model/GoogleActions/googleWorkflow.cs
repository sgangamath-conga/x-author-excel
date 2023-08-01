 /* Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections.Generic;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;


/* 
 * googleWorkflow is a class which will contain all the information needed in google docs to execute an action. 
 * A googleWorkflow is composed of a workflow ID and a list of actions, googleAction.
 */
namespace Apttus.XAuthor.Core
{
    public class googleWorkflow
    {
        public string workflowID;
        public List<googleAction> actionList = new List<googleAction>();

        //constructor 1: creates an action as well as new workflow
        public googleWorkflow(string ID, string actionID)
        {
            this.workflowID = ID;
            googleAction action = new googleAction(actionID);
            actionList.Add(action);            
        }
        //constrcutor 2
        public googleWorkflow(string ID)
        {
            this.workflowID = ID;
        }

        //constrcutor 3
        public googleWorkflow(string ID, googleAction action)
        {
            this.workflowID = ID;
            this.addAction(action);
            //this.actionList.Add(action);
        }

        //return the googleaction that matches the argument ID. The id is the action's actionID.
        public googleAction getActionByID(string ID)
        {
            return actionList.Find(x => x.actionID == ID);
        }

        //adds the given action to this workflow's action list
        public void addAction(googleAction action)
        {
            this.actionList.Add(action);
        }
    }

    public class dTreeNode
    {
        String Id;
        String parentId;
        String parentColLetter;
        //List<String> childrenId;

        public dTreeNode(string Id)
        {
            this.Id = Id;            
        }
    }

    //wrapper to contain workflows, appobject data, and app menu data.
    public class GoogleAppConfig
    {
        public Boolean getUserInput;
        public Boolean hasMultiPicklist = false;
        public string childQueryObject;
        public Dictionary<string, List<string>> multiPicklistData;
        public Dictionary<string, string> controllingFieldToLocation;//key: contrlling fieldId, value: absolute col Letter (for dependent picklist)       
        public Dictionary<string, string> colorSettings;
        public Dictionary<string, string> rFieldTypes;
        public Dictionary<string, Datatype> rFieldTypesInt; //key: fieldId, value: type as integer
        public Dictionary<string, string> parentrFieldTypes;
        public Dictionary<string, int> saveFieldToColIndex;
        public Dictionary<string, googleAction> standAloneActions; //key is action id for an action that can be executed on its own
        public Dictionary<string, List<string>> getActionByIdMap = new Dictionary<string,List<string>>();//first key is the actionId, the dictionary is used as a pair where the key is the workflow sequence number, and the value is the action sequence number
        public Dictionary<string, string> namedRangeToObject = new Dictionary<string, string>(); //maps a rangename to its corresponding sObject
        public List<string> userInputWorkflowsandActions = new List<string>(); //even index: workflowId, odd index: actionId
        public string filterLogicText;
        public string childAppObjectId; //Id for the child object
        public string parentAppObjectId; //Id for the parent object
        public string workSheetTitle;

        //list of Actionflows for the app. 
        public List<googleWorkflow> googleActionFlows
        {
            get;
            set;
        }


        //user menu for the app 
        public Menus AppMenu
        {
            get;
            set;

        }

        public List<ApttusObject> AppObjects
        {
            get;
            set;
        }
        public List<ApttusObject> ChildAppObjects
        {
            get;
            set;
        }
    }

}
