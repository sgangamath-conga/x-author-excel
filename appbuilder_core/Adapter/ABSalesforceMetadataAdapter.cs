/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.SalesforceAdapter;
using SFMetadata = Apttus.XAuthor.SalesforceAdapter.MetadataSForce;

namespace Apttus.XAuthor.Core
{
    public class ABSalesforceMetadataAdapter
    {
        // Intiliaze Salesforce Connector class
        SForceMetadataServiceController metadataConnector = SForceMetadataServiceController.GetInstance;

        // Connect with Salesforce
        public bool Connect(string OAuthToken, string InstanceURL)
        {
            if (OAuthToken != null)
            {
                metadataConnector.ConnectWithSalesforce(OAuthToken, InstanceURL);
                return true;
            }
            else
            {
                return false;
            }
        }

        #region "Metadata service calls"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public string GetLookupDialogFields(string objectName)
        {
            List<string> lookUpFields = new List<string>();
            string type = "CustomObject";
            SFMetadata.Metadata[] objectMetadata = metadataConnector.readMetadata(type, new string[] { objectName });

            foreach (SFMetadata.Metadata md in objectMetadata)
            {
                if (md != null)
                {
                    SFMetadata.CustomObject cObj = (SFMetadata.CustomObject)md;
                    SFMetadata.SearchLayouts sLayouts = cObj.searchLayouts;
                    // Read lookup dialog fields from search layouts
                    lookUpFields = sLayouts.lookupDialogsAdditionalFields.ToList();
                }
            }

            return string.Join(",", lookUpFields.ToArray());
        }


        #endregion
    }
}
