/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Apttus.XAuthor.Core
{
    public class StartupParameters
    {
        private const string regex = @"/.*=.*|";

        // Startup parameters
        public string AppId { get; private set; }
        public string ExportRecordId { get; private set; }
        public string SessionId { get; private set; }
        public string URLInstance { get; private set; }
        public string ActionFlowName { get; private set; }
        public void Parse(string[] args)
        {
           
            for (int i = 0; i < args.Length -1; ++i)
            {
                Match match = Regex.Match(args[i], regex);

                if (!match.Success)
                    continue;
                if (!args[i].Contains("|")) continue;
          
              //  string[] argSplit = args[i].Split(new char[] {'='}, 2);
                
                List<string> startUpParams = args[i].Split('|').ToList<string>();
                DataManager.GetInstance.StartParameters = this;
                foreach (string keyval in startUpParams)
                {
                    string[] argSplit = keyval.Split(new char[] { '=' }, 2);
                    switch (argSplit[0])
                    {
                        case "/AppId":
                          
                            AppId = argSplit[1];//tempAppID.Substring(0, indx);
                            break;
                        case "/ExportId":
                           
                            ExportRecordId = argSplit[1];//tempAppID.Substring(0, indx);
                            break;
                        case "/InstanceURL":
                            URLInstance = (argSplit[1]);
                            break;
                        case "/SessionId":
                            SessionId = validateId(argSplit[1]);
                            break;

                        case "/ActionflowName":
                            ActionFlowName = validateId(argSplit[1]);
                            break;
                        default:
                            break;
                    }


                }



              //  DataManager.GetInstance.StartParameters = this;
               
            }
            
         
        }
        private string[] GetValues(string val)
        {
            string[] valArray = new string[2];
            
            string tempAppID = validateId(val);
            int indx = tempAppID.IndexOf('|');
            valArray[0] =(tempAppID.Substring(indx + 1));
            valArray[1] = (tempAppID.Substring(0, indx));

            return valArray;
        }
        private string validateId(string id)
        {
            //TODO: validate that argument is ID
            return id;
        }
    }
}
