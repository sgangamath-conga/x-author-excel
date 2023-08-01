/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppRuntime.Controller.Actions.CPQActon;

using Apttus.XAuthor.AppRuntime.CPQAPI;
using Apttus.XAuthor.AppRuntime.CPQSupport;
using Apttus.XAuthor.AppRuntime.Modules;
using Apttus.XAuthor.Core;
using Apttus.XAuthor.Core.Model.Actions.CPQAction;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppRuntime.Controller.Actions.CPQAction
{


    public class CartController : IOutputActionData, IInputActionData
    {
        // Model
        protected CreateCartModel Model;
        public bool InputData { get; set; }
        public bool OutputPersistData { get; set; }
        public string[] InputDataName { get; set; }
        public string OutputDataName { get; set; }
        private string attachmentId = string.Empty;
        private string attachmentName = string.Empty;
        protected const string PROPOSAL = "Apttus_Proposal__Proposal__c";
        protected const string CART = "Apttus_Config2__ProductConfiguration__c";
        protected const string PROPOSALLOOKAUP = "Apttus_QPConfig__Proposald__c";
        protected const string LINEITEM = "Apttus_Config2__PriceListItem__c";
        protected Apttus.XAuthor.AppRuntime.CPQSupport.AppBuilderCPQSupportService service;
        protected CPQAPI.CPQWebServiceService CPQAPIService;
        public ActionResult Result { get; protected set; }
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusCommandBarManager commandBar = ApttusCommandBarManager.GetInstance();
        protected ObjectManager objectManager = ObjectManager.GetInstance;
        protected ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        protected XAuthorSalesforceLogin oAuthWrapper;

        public CartController(CreateCartModel model, string[] inputDataName)
        {
            this.Model = model;
            this.InputDataName = inputDataName;
            Result = new ActionResult();
            oAuthWrapper = Globals.ThisAddIn.GetLoginObject() as XAuthorSalesforceLogin;
        }

        protected string GetProposalNavigator()
        {
            return CPQUtil.GetRuntimeDir() + "\\X-Author Proposal Navigator.Exe";
        }

        public virtual ActionResult Execute()
        {
            List<ApttusDataSet> DS = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity opp = null;
            List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c> Lines = null;
            foreach (ApttusDataSet ds in DS)
            {
                if (ds.Name == "opp")
                {
                    opp = GetOpp(ds.DataTable);
                }
                else if (ds.Name == "products")
                {
                    Lines = GetLines(ds.DataTable);
                }
            }

            string cartID = null;
            //Apttus.XAuthor.AppRuntime.CPQSupport.AppBuilderCPQSupportService service = new Apttus.XAuthor.AppRuntime.CPQSupport.AppBuilderCPQSupportService();
            Apttus.XAuthor.AppRuntime.PricingWebService.PricingWebServiceService PriceService = new PricingWebService.PricingWebServiceService();
            if (Lines != null && opp != null)
            {

                cartID = createCartForOpportunity2(opp, Lines);
                // cartID = GetCPQService().createCartForOpportunity2(opp, Lines.ToArray());
            }
            ApplyPricing(cartID);
            QueryCart(cartID);
            Result.Status = ActionResultStatus.Success;
            return Result;
        }


        protected List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c> getLineNumbers(string cartId)
        {


            StringBuilder sb = new StringBuilder("SELECT Id,  Apttus_Config2__LineNumber__c FROM Apttus_Config2__LineItem__c WHERE Apttus_Config2__ConfigurationId__c =");

            sb.Append("'").Append(cartId).Append("'");
            string queryString = sb.ToString();
            DataTable DT = new DataTable();
            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c> CartLineItems = new List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c>();

            for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
            {
                Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c LineItem = new Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c();
                string lineNum = DS.DataTable.Rows[icount]["Apttus_Config2__LineNumber__c"] as string;
                LineItem.Apttus_Config2__LineNumber__c = Convert.ToDouble(lineNum);
                LineItem.Id = DS.DataTable.Rows[icount]["Id"] as string;
                CartLineItems.Add(LineItem);

            }

            return CartLineItems;

        }
        protected string createCartForOpportunity2(Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity opptySO, List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c> priceListItems)
        {
            StringBuilder sb = new StringBuilder("SELECT Id, Name, AccountId, Description, Price_List__c FROM Opportunity WHERE Id =");

            sb.Append("'").Append(opptySO.Id).Append("'");
            string queryString = sb.ToString();
            DataTable DT = new DataTable();
            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity oppty = GetOpp(DS.DataTable);

            List<Apttus.XAuthor.AppRuntime.CPQAPI.Product2> products = new List<Apttus.XAuthor.AppRuntime.CPQAPI.Product2>();

            sb = new StringBuilder("SELECT Apttus_Config2__ProductId__c FROM Apttus_Config2__PriceListItem__c  WHERE Id =");
            foreach (Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c priceListItem in priceListItems)
            {
                // Re-query item as workaround for missing ID field
                Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c item = new Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c();


                sb.Append("'").Append(priceListItem.Id).Append("'");

                queryString = sb.ToString();
                DT = new DataTable();
                DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
                for (int i = 0; i < DS.DataTable.Rows.Count; i++)
                {
                    Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__PriceListItem__c line = new Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__PriceListItem__c();
                    line.Apttus_Config2__ProductId__c = DS.DataTable.Rows[i]["Apttus_Config2__ProductId__c"] as string;
                    sb = new StringBuilder("SELECT Id, Name FROM Product2 WHERE Id =");
                    sb.Append("'").Append(line.Apttus_Config2__ProductId__c).Append("'");
                    queryString = sb.ToString();
                    DT = new DataTable();
                    DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
                    for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
                    {
                        Apttus.XAuthor.AppRuntime.CPQAPI.Product2 p = new Apttus.XAuthor.AppRuntime.CPQAPI.Product2();
                        p.Id = DS.DataTable.Rows[icount]["Id"] as string;
                        p.Name = DS.DataTable.Rows[icount]["Name"] as string;
                        products.Add(p);
                    }

                }
            }
            sb = new StringBuilder("SELECT RecordTypeId FROM Apttus_Proposal__Proposal__c LIMIT 1");

            queryString = sb.ToString();
            DT = new DataTable();
            DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            string RecordTypeId = null;
            for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
            {
                RecordTypeId = DS.DataTable.Rows[icount]["RecordTypeId"] as string;
            }
            Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c proposalSO = createProposalFromOpportunity(oppty, RecordTypeId);

            //save proposal

            ApttusSaveRecord LineItem = new ApttusSaveRecord()
            {
                SaveFields = new List<ApttusSaveField>(),
                ObjectName = "Apttus_Proposal__Proposal__c",

                RecordRowNo = 0,
                RecordColumnNo = -1,
                ObjectType = ObjectType.Repeating,
                OperationType = QueryTypes.INSERT,

            };



            # region Save proposal
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();

            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "RecordTypeId",
                Value = proposalSO.RecordTypeId,
                DataType = Datatype.String
            });
            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__Proposal_Name__c",
                Value = proposalSO.Apttus_Proposal__Proposal_Name__c,
                DataType = Datatype.String
            });
            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__Opportunity__c",
                Value = proposalSO.Apttus_Proposal__Opportunity__c,
                DataType = Datatype.Lookup
            });
            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__Account__c",
                Value = proposalSO.Apttus_Proposal__Account__c,
                DataType = Datatype.Lookup
            });
            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__Primary__c",
                Value = "true",
                DataType = Datatype.Boolean
            });


            //LineItem.SaveFields.Add(new ApttusSaveField
            //{
            //    FieldId = "Apttus_QPConfig__PricingDate__c",
            //    Value = GetCurretDateAndTime(),
            //    DataType = Datatype.DateTime
            //});

            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_QPConfig__PriceListId__c",
                Value = proposalSO.Apttus_QPConfig__PriceListId__c,
                DataType = Datatype.Lookup
            });

            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__Valid_Until_Date__c",
                Value = GetCurretDateAndTime(),
                DataType = Datatype.DateTime
            });


            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__ReadyToGenerate__c",
                Value = "true",
                DataType = Datatype.Boolean
            });

            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "Apttus_Proposal__ReadyToPresent__c",
                Value = "true",
                DataType = Datatype.Boolean
            });

            LineItem.SaveFields.Add(new ApttusSaveField
            {
                FieldId = "OwnerId",
                Value = proposalSO.OwnerId,
                DataType = Datatype.Lookup
            });

            SaveRecords.Add(LineItem);
            objectManager.Insert(SaveRecords, false);
            if (SaveRecords.Count > 0 && SaveRecords[0].RecordId != null)
            {
                proposalSO.Id = SaveRecords[0].RecordId;
            }
            #endregion
            HashSet<string> productSet = new HashSet<string>();
            foreach (Apttus.XAuthor.AppRuntime.CPQAPI.Product2 p in products)
            {
                productSet.Add(p.Id);

            }
            List<ProductDO> priceListProducts = null;
            try
            {     // AB 1531 . no need to make this following call. 
                  // priceListProducts = GetCPQAPIService().getProductsForPriceList(proposalSO.Apttus_QPConfig__PriceListId__c).Products.ToList();
            }
            catch (Exception ex)
            {

            }
            List<ProductDO> selectedProducts = new List<ProductDO>();


            if (priceListProducts != null)
            {
                foreach (ProductDO p in priceListProducts)
                {
                    if (productSet.Contains(p.ProductId))
                        selectedProducts.Add(p);
                }
            }

            // STEP III - create the cart
            // create the cart request
            CreateCartRequestDO request = new CreateCartRequestDO();

            // add request parameters
            request.QuoteId = proposalSO.Id;

            // create a cart for the quote
            CreateCartResponseDO result = GetCPQAPIService().createCart(request);

            string cartId = result.CartId;


            // STEP IV - add products to cart
            if (selectedProducts.Count > 0)
            {
                // create the add multi-product request
                AddMultiProductRequestDO request2 = new AddMultiProductRequestDO();

                // add request parameters
                request2.CartId = cartId;

                int prodCount = 0;
                foreach (ProductDO productDO in selectedProducts)
                {
                    SelectedProductDO selectedDO = new SelectedProductDO();
                    selectedDO.ProductId = productDO.ProductId;
                    selectedDO.Quantity = 1;
                    selectedDO.SellingTerm = 1;
                    prodCount++;
                    // add the selected product to the collection
                    request2.SelectedProducts[prodCount] = (selectedDO);

                }


                // AB 1531 . no need to make this following call. 
                // add a bundle to the cart
                // GetCPQAPIService().addMultiProducts(request2);
                return cartId;

            }

            return cartId;
        }
        private string GetCurretDateAndTime()
        {

            DateTime time = DateTime.Now;              // Use current time
            string format = "MM-dd-yyyy";    // Use this format
            Console.WriteLine(time.ToString(format));
            //"MM-dd-yyyy"
            string validaDate = Utils.IsValidDate(time.ToString(format), Datatype.DateTime);
            return validaDate;
        }
        public String resizeText(String text)
        {
            int MAX_TEXT_SIZE = 255;
            // limit text to max size
            return (text != null && text.Length > MAX_TEXT_SIZE
                    ? text.Substring(0, (MAX_TEXT_SIZE - 3)) + "..."
                    : text);

        }
        private string getOpptyPrimaryContactId(string opptyId)
        {
            StringBuilder sb = new StringBuilder("SELECT ContactId FROM OpportunityContactRole WHERE OpportunityId =");
            sb.Append("'").Append(opptyId).Append("'").Append("AND IsPrimary = TRUE LIMIT 1");

            string queryString = sb.ToString();
            DataTable DT = new DataTable();
            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            string contactID = null;
            for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
            {
                contactID = DS.DataTable.Rows[icount]["ContactId"] as string;
            }
            return String.IsNullOrEmpty(contactID) ? null : contactID;



        }


        /**
     * Gets the primary contact id for the given account id
     * @param accountId the account id to get the primary contact id for
     * @return the primary contact id or null if there is no primary contact for the account
     */
        private string getAccountPrimaryContactId(string accountId)
        {

            // get the primary contact for the account


            StringBuilder sb = new StringBuilder("SELECT ContactId FROM AccountContactRole WHERE AccountId =");

            sb.Append("'").Append(accountId).Append("'").Append("AND IsPrimary = TRUE LIMIT 1");

            string queryString = sb.ToString();
            DataTable DT = new DataTable();
            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            string contactID = null;
            for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
            {
                contactID = DS.DataTable.Rows[icount]["ContactId"] as string;
            }

            return String.IsNullOrEmpty(contactID) ? null : contactID;
        }

        public Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c createProposalFromOpportunity(Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity opptySO, string recordTypeId)
        {

            // STEP I - create a new proposal sobject
            Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c proposalSO = new Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c();

            // initialize the proposal from the opportunity

            // STEP II - copy known opportunity fields
            // record type id
            if (recordTypeId != null)
            {
                proposalSO.RecordTypeId = recordTypeId;

            }

            // proposal name (max length = 255)
            proposalSO.Apttus_Proposal__Proposal_Name__c = resizeText(opptySO.Name);
            // opportunity
            proposalSO.Apttus_Proposal__Opportunity__c = opptySO.Id;
            // account
            proposalSO.Apttus_Proposal__Account__c = opptySO.AccountId;
            // primary contact
            if (opptySO.AccountId != null)
            {
                // get the primary contact from the opportunity
                proposalSO.Apttus_Proposal__Primary_Contact__c = getOpptyPrimaryContactId(opptySO.Id);
                if (proposalSO.Apttus_Proposal__Primary_Contact__c == null)
                {
                    // get the primary contact from the account
                    proposalSO.Apttus_Proposal__Primary_Contact__c = getAccountPrimaryContactId(opptySO.AccountId);

                }

            }
            // description
            proposalSO.Apttus_Proposal__Description__c = opptySO.Description;
            // primary
            proposalSO.Apttus_Proposal__Primary__c = true;//!checkIfPrimaryProposalsForOpportunity(opptySO.Id);
                                                          // price list id
            proposalSO.Apttus_QPConfig__PriceListId__c = opptySO.Price_List__c;
            // pricing date
            proposalSO.Apttus_QPConfig__PricingDate__c = DateTime.Now;
            // valid until date
            proposalSO.Apttus_Proposal__Valid_Until_Date__c = DateTime.Today.AddDays(30);
            // ready to generate
            proposalSO.Apttus_Proposal__ReadyToGenerate__c = true;
            // ready to present
            proposalSO.Apttus_Proposal__ReadyToPresent__c = true;
            // owner

            ApttusUserInfo userInfo = objectManager.getUserInfo();
            proposalSO.OwnerId = userInfo.UserId;
            //(DEFAULTOWNER_CURRENTUSER == Systemutil.getDefaultOpportunityQuoteOwner() 
            // current user
            //? UserInfo.getUserId()
            // opportunity owner
            //: opptySO.OwnerId);

            // set the currency code (if multi-currency enabled org)
            //if (userInfo..isMultiCurrencyOrganization()) {
            //    proposalSO.put('CurrencyIsoCode', opptySO.get('CurrencyIsoCode'));

            //}

            // STEP III - reset primary proposals for the opportunity
            resetPrimaryProposalsForOpportunity(opptySO.Id);

            // STEP IV - copy custom field based on copy spec

            // return the created proposal
            return proposalSO;

        }

        /**
            * Resets the primary proposal for the opportunity
            * @param opptyId the opportunity id associated with the quote/proposals
            */
        public void resetPrimaryProposalsForOpportunity(string opptyId)
        {

            // Sget draft proposals associated with the opportunity and make them non-primary
            List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c> proposals = new List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c>();

            StringBuilder sb = new StringBuilder("SELECT Id, Apttus_Proposal__Primary__c FROM Apttus_Proposal__Proposal__c  WHERE Apttus_Proposal__Opportunity__c =");

            sb.Append("'").Append(opptyId).Append("'").Append("AND Apttus_Proposal__Primary__c = TRUE ");

            string queryString = sb.ToString();
            DataTable DT = new DataTable();

            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });

            for (int icount = 0; icount < DS.DataTable.Rows.Count; icount++)
            {
                Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c proposalSO = new Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c();
                proposalSO.Id = DS.DataTable.Rows[icount]["Id"] as string;
                proposalSO.Apttus_Proposal__Primary__c = false;
                proposals.Add(proposalSO);
            }


            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
            foreach (Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Proposal__Proposal__c item in proposals)
            {

                ApttusSaveRecord LineItem = new ApttusSaveRecord()
                {
                    SaveFields = new List<ApttusSaveField>(),
                    ObjectName = "Apttus_Proposal__Proposal__c",

                    RecordRowNo = 0,
                    RecordColumnNo = -1,
                    ObjectType = ObjectType.Repeating,
                    OperationType = QueryTypes.UPDATE,

                };

                LineItem.SaveFields.Add(new ApttusSaveField
                {
                    FieldId = "Apttus_Proposal__Primary__c",
                    Value = "false",
                    DataType = Datatype.Boolean
                });

                LineItem.SaveFields.Add(new ApttusSaveField
                {
                    FieldId = Constants.ID_ATTRIBUTE,
                    Value = item.Id,
                    DataType = Datatype.String
                });

                SaveRecords.Add(LineItem);

            }
            objectManager.Update(SaveRecords, false);

            Result.Status = ActionResultStatus.Success;

        }

        protected void QueryCart(string cartID)
        {

            // Get select fields and handle lookups
            string selectFields = "";
            DataTable dataTable = new DataTable();
            string queryString;
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject appObject = appObjects.Find(item => item.Id.Equals(PROPOSAL));
            ApttusObject CartObj = appObjects.Find(item => item.Id.Equals(CART));

            bool IsProposalAdded = false;
            List<ApplicationField> appFields = configurationManager.GetAllAppFields(CartObj, true);

            foreach (ApplicationField field in appFields)
            {
                //ApttusField f = appObject.Fields.Where(s => s.Id.Equals(fieldName)).FirstOrDefault(); ;

                // Create select clause for the SOQL
                selectFields += field.FieldId;
                selectFields += ",";

                // Append DataColumn to the datatable
                DataColumn dc = new DataColumn(field.FieldId);
                dc.DataType = field.DataType == Datatype.Decimal || field.DataType == Datatype.Double ? typeof(double) : typeof(string);
                dataTable.Columns.Add(dc);
                if (field.FieldId.Equals(PROPOSALLOOKAUP))
                {
                    IsProposalAdded = true;
                }
            }
            if (!IsProposalAdded)
            {
                selectFields += PROPOSALLOOKAUP;
                selectFields += ",";

                // Append DataColumn to the datatable
                DataColumn dc = new DataColumn(PROPOSALLOOKAUP);
                dc.DataType = typeof(string);
                dataTable.Columns.Add(dc);
            }
            selectFields = selectFields.Remove(selectFields.Length - 1, 1);

            // Build query
            StringBuilder sb = new StringBuilder("SELECT ");
            sb.Append(selectFields);
            sb.Append(" FROM ");
            sb.Append(CartObj.Id);
            sb.Append(" WHERE Id =").Append("'").Append(cartID).Append("'");
            queryString = sb.ToString();
            DataTable DT = new DataTable();
            ApttusDataSet DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            DS.Name = this.OutputDataName;
            DS.AppObjectUniqueID = CartObj.UniqueId;
            DataManager dataManager = DataManager.GetInstance;
            dataManager.AddData(DS);

        }


        protected List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c> GetLines(DataTable DS)
        {
            List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c> lines = new List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c>();
            for (int i = 0; i < DS.Rows.Count; i++)
            {
                Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c line = new Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c();
                line.Id = DS.Rows[i]["Id"] as string;
                lines.Add(line);
            }

            return lines;
        }
        protected CPQAPI.CPQWebServiceService GetCPQAPIService()
        {
            if (CPQAPIService == null)
            {
                CPQAPIService = new CPQAPI.CPQWebServiceService();

                CPQAPI.SessionHeader ss = new CPQAPI.SessionHeader();
                ss.sessionId = oAuthWrapper.AccessToken;
                CPQAPIService.SessionHeaderValue = ss;
                CPQAPIService.Url = oAuthWrapper.InstanceURL + "//services//Soap//class//Apttus_CPQApi//CPQWebService";
            }
            return CPQAPIService;
        }
        public void FinalizeCart(string CartId)
        {
            CPQAPI.FinalizeCartRequestDO FinalizeRqst = new CPQAPI.FinalizeCartRequestDO();
            FinalizeRqst.CartId = CartId;
            GetCPQAPIService().finalizeCart(FinalizeRqst);
        }
        protected Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity GetOpp(DataTable DS)
        {
            Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity opp = new Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity();

            foreach (DataColumn c in DS.Columns)
            {
                string s = c.ColumnName;
            }
            foreach (DataRow row in DS.Rows)
            {

                opp.Name = row["Name"] as string;
                opp.Id = row["id"] as string;
                // opp.AccountId = row["AccountId"] as string;
                opp.Price_List__c = row["Price_List__c"] as string;
                try
                {
                    opp.AccountId = row["AccountId"] as string;
                }
                catch (Exception ex)
                {

                }
            }
            return opp;
        }
        /*
        public Apttus.XAuthor.AppRuntime.CPQSupport.AppBuilderCPQSupportService GetCPQService()
        {
            if (service == null)
            {
                service = new Apttus.XAuthor.AppRuntime.CPQSupport.AppBuilderCPQSupportService();
                Apttus.XAuthor.AppRuntime.CPQSupport.SessionHeader ss = new Apttus.XAuthor.AppRuntime.CPQSupport.SessionHeader();
                ss.sessionId = Globals.ThisAddIn.oAuthWrapper.token.access_token;
                service.SessionHeaderValue = ss;
                service.Url = Globals.ThisAddIn.oAuthWrapper.token.instance_url + "//services//Soap//class//AppBuilderCPQSupport";
            }

            return service;
        }
        */

        protected ApttusObject GetObject(string CPQObj)
        {
            ApttusObject appObject = ApplicationDefinitionManager.GetInstance.GetAllObjects().Find(item => item.Id.Equals(CPQObj));
            return appObject;
        }
        public virtual string ApplyPricing(string CartID)
        {
            Apttus.XAuthor.AppRuntime.PricingWebService.PricingWebServiceService PriceService = new PricingWebService.PricingWebServiceService();
            Apttus.XAuthor.AppRuntime.PricingWebService.SessionHeader PriceHEader = new PricingWebService.SessionHeader();
            PriceHEader.sessionId = oAuthWrapper.AccessToken;
            PriceService.SessionHeaderValue = PriceHEader;
            PriceService.Url = oAuthWrapper.InstanceURL + "//services//Soap//class//Apttus_Config2//PricingWebService";
            //get the line item object
            ApttusObject appObject = GetObject(LINEITEM);

            if (appObject != null)
            {
                // get the data set for the line item
                ApttusDataSet DS = DataManager.GetInstance.GetDataById(appObject.UniqueId);
                List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c> LineItems = getLineNumbers(CartID);
                //Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c[] LineItems = GetCPQService().getLineNumers(CartID);                
                PriceService.updatePriceForCart(CartID);
            }
            return null;
        }
    }

    public class ApplyPricingController : CartController
    {

        public override ActionResult Execute()
        {

            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            string CartId = DS.DataTable.Rows[0]["Id"] as string;
            if (DS != null)
            {

                if (CartId != null)
                {
                    ApplyPricing(CartId);
                    //  FinalizeCart(CartId);
                }
            }
            Result.Status = ActionResultStatus.Success;
            return Result;

        }
        public ApplyPricingController(CreateCartModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;
            Result = new ActionResult();
        }

    }

    public class FinalizeCartController : CartController
    {
        public override ActionResult Execute()
        {
            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            ApttusObject CartObject = GetObject(CART);

            try
            {
                string CartId = DS.DataTable.Rows[0]["Id"] as string;
                if (DS != null)
                {

                    if (CartId != null)
                    {
                        // ApplyPricing(CartId);
                        FinalizeCart(CartId);
                    }
                }

                Result.Status = ActionResultStatus.Success;
                ApttusMessageUtil.ShowInfo(resourceManager.GetResource("CARTCTL_Execute_InfoMsg"), resourceManager.GetResource("CARTCTL_ExecuteCAP_InfoMsg"));
                return Result;
            }
            catch (Exception ex)
            {
                Result.Status = ActionResultStatus.Failure;
                ApttusMessageUtil.ShowInfo(ex.Message.ToString(), resourceManager.GetResource("CARTCTL_ExecuteCAP_InfoMsg"));
                return Result;
            }

        }
        public FinalizeCartController(FinalizeCartModel model)
            : base(model, null)
        {
            Model = model;


            Result = new ActionResult();
        }
    }
    public class CreateEmptyCartController : CartController
    {

        public override ActionResult Execute()
        {

            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            Apttus.XAuthor.AppRuntime.CPQAPI.Opportunity opp = null;
            List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c> Lines = new List<Apttus.XAuthor.AppRuntime.CPQAPI.Apttus_Config2__PriceListItem__c>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject CartObj = appObjects.Find(item => item.Id.Equals(CART));
            ApttusDataSet DS = DSL[0];
            opp = GetOpp(DS.DataTable);
            string cartID;
            if (opp != null)
            {

                cartID = createCartForOpportunity2(opp, Lines);
                QueryCart(cartID);
            }

            Result.Status = ActionResultStatus.Success;
            return Result;

        }
        public CreateEmptyCartController(CreateCartModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;
            Result = new ActionResult();
        }

    }
    public class UpdateCartController : CartController
    {

        public override ActionResult Execute()
        {
            ObjectManager objectManager = ObjectManager.GetInstance;
            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            string CartId = DS.DataTable.Rows[0]["Id"] as string;
            List<ApttusSaveRecord> SaveRecords = new List<ApttusSaveRecord>();
            if (DS != null)
            {
                //  Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c[] LineItems = GetCPQService().getLineItems(CartId);
                List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c> LineItems = getLineNumbers(CartId);

                foreach (Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c item in LineItems)
                {
                    item.Apttus_Config2__IsPrimaryLine__c = true;
                    ApttusSaveRecord LineItem = new ApttusSaveRecord()
                    {
                        SaveFields = new List<ApttusSaveField>(),
                        ObjectName = "Apttus_Config2__LineItem__c",

                        RecordRowNo = 0,
                        RecordColumnNo = -1,
                        ObjectType = ObjectType.Repeating,
                        OperationType = QueryTypes.UPDATE,

                    };

                    LineItem.SaveFields.Add(new ApttusSaveField
                    {
                        FieldId = "Apttus_Config2__IsPrimaryLine__c",
                        Value = "true",
                        DataType = Datatype.Boolean
                    });
                    LineItem.SaveFields.Add(new ApttusSaveField
                    {
                        FieldId = Constants.ID_ATTRIBUTE,
                        Value = item.Id,
                        DataType = Datatype.String
                    });
                    string Line = System.Convert.ToString(item.Apttus_Config2__LineNumber__c);
                    LineItem.SaveFields.Add(new ApttusSaveField
                    {//(Decimal)item.Apttus_Config2__LineNumber__c)
                        FieldId = "Apttus_Config2__PrimaryLineNumber__c",
                        Value = Line,
                        DataType = Datatype.Decimal
                    });
                    SaveRecords.Add(LineItem);

                }

            }

            objectManager.Update(SaveRecords, false);

            Result.Status = ActionResultStatus.Success;
            return Result;

        }
        public UpdateCartController(UpdateCartModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;


            Result = new ActionResult();
        }


    }


    public class CreateProposalController : CartController
    {

        public override ActionResult Execute()
        {
            // call the external program to launch CPQUI
            // take the OppId and PL name from input


            ObjectManager objectManager = ObjectManager.GetInstance;
            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            string OppId = null;
            string PriceListName = null;


            //"C:\Program Files (x86)\Apttus\X-Author for Excel\Apps\X-Author URL Launcher.exe" "%1" 

            string path = GetProposalNavigator();// "c:\\temp\\XAuthor Proposal Navigator.Exe";

            try
            {
                OppId = DS.DataTable.Rows[0]["Id"] as string;
                PriceListName = DS.DataTable.Rows[0]["Price_List__r.Name"] as string;
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("CreateProposalController_Execute_ErrorMsg"), resourceManager.GetResource("CreateProposalController_ExecuteCap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }
            CPQUtil CPQUtilNav = new CPQUtil();
            CPQUtilNav.OppId = OppId;
            CPQUtilNav.PriceListName = PriceListName;
            string sURL = CPQUtilNav.LoadCtrl(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken);
            if (sURL != null)
            {
                var p = new System.Diagnostics.Process();
                // Get the current directory. 
                var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                //     string path = Directory.GetCurrentDirectory();
                p.StartInfo.FileName = path; // path + "\\" + "XAuthor Proposal Navigator.Exe"; // @"C:\Users\Refeekh Peermohammed\Documents\Visual Studio 2013\Projects\WindowsFormsApplication4\WindowsFormsApplication4\bin\Debug\WindowsFormsApplication4.exe";
                p.StartInfo.Arguments = sURL;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();

                string output = p.StandardOutput.ReadToEnd();

                string newVal = output.Replace("\r\n", string.Empty);
                string[] ReturnIDs = null;
                if (!string.IsNullOrEmpty(newVal))
                {
                    ReturnIDs = newVal.Split('|');
                }

                if (OutputPersistData && ReturnIDs.Length > 0)
                {
                    string CellRef = OutputDataName;
                    if (!string.IsNullOrEmpty(CellRef))
                    {
                        // first validate the cell refer
                        if (ExcelHelper.IsCellRefValid(CellRef))
                        {

                            Microsoft.Office.Interop.Excel.Range cellReferenceRange = ExcelHelper.GetRangeByLocation(CellRef);
                            if (cellReferenceRange != null)
                            {
                                //cellReferenceRange.Value2 = ReturnIDs[0];

                                foreach (string ids in ReturnIDs)
                                {
                                    cellReferenceRange.Value2 = CPQUtil.RemoeVectorString(ids);
                                    cellReferenceRange = ExcelHelper.NextHorizontalCell(cellReferenceRange, 1);
                                    // cellReferenceRange.Value2 = ids;
                                }

                            }

                        }
                        else
                        {

                            ApttusMessageUtil.ShowError(CellRef + resourceManager.GetResource("CreateProposalController_Execute_IsCellRef_ErrorMsg"), resourceManager.GetResource("COMMON_CellReference_Msg"));
                        }
                    }




                }

                // the output needs to be captured in excel so that the 
                // it can be referenced in the query to pull the lines


                Result.Status = ActionResultStatus.Success;

                //  Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c[] LineItems = GetCPQService().getLineItems(CartId);
                //  List<Apttus.XAuthor.AppRuntime.CPQSupport.Apttus_Config2__LineItem__c> LineItems = getLineNumbers(CartId);


            }
            //


            Result.Status = ActionResultStatus.Success;
            return Result;

        }
        public CreateProposalController(CreateProposalModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;


            Result = new ActionResult();
        }


    }





    public class ViewProposalController : CartController
    {
        public override ActionResult Execute()
        {
            // call the external program to launch CPQUI
            // take the OppId and PL name from input


            ObjectManager objectManager = ObjectManager.GetInstance;
            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            string propId = null;

            string path = GetProposalNavigator(); //"c:\\temp\\XAuthor Proposal Navigator.Exe";

            try
            {
                propId = DS.DataTable.Rows[0]["Id"] as string;
                if (string.IsNullOrEmpty(propId))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_Proposal_Execute_ErrorMsg"), resourceManager.GetResource("COMMON_Proposal_Cap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_Proposal_Execute_ErrorMsg"), resourceManager.GetResource("COMMON_Proposal_Cap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }


            CPQUtil CPQUtilNav = new CPQUtil();
            CPQUtilNav.GetEditURL(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken, propId);
            string sURL = CPQUtilNav.GetEditURL(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken, propId);
            if (sURL != null)
            {
                var p = new System.Diagnostics.Process();
                // Get the current directory. 
                var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                //     string path = Directory.GetCurrentDirectory();
                p.StartInfo.FileName = path; // path + "\\" + "XAuthor Proposal Navigator.Exe"; // @"C:\Users\Refeekh Peermohammed\Documents\Visual Studio 2013\Projects\WindowsFormsApplication4\WindowsFormsApplication4\bin\Debug\WindowsFormsApplication4.exe";
                p.StartInfo.Arguments = sURL;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();

                string output = p.StandardOutput.ReadToEnd();

                string newVal = output.Replace("\r\n", string.Empty);
                string[] ReturnIDs = null;
                if (!string.IsNullOrEmpty(newVal))
                {
                    ReturnIDs = newVal.Split('|');
                }

            }
            //


            Result.Status = ActionResultStatus.Success;
            return Result;

        }
        public ViewProposalController(ViewProposalModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;


            Result = new ActionResult();
        }

    }

    public class EditCartController : CartController
    {

        public EditCartController(EditCartModel model, string[] InputNames)
            : base(model, InputNames)
        {
            Model = model;


            Result = new ActionResult();
        }

        public override ActionResult Execute()
        {
            // call the external program to launch CPQUI
            // take the OppId and PL name from input


            ObjectManager objectManager = ObjectManager.GetInstance;
            if (!IsInputValid(InputDataName, resourceManager.GetResource("EditCartController_ExecuteCap_ErrorMsg")))
            {
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }

            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            ApttusDataSet DS = DSL[0];
            string propId = null;
            string cartId = null;
            string path = GetProposalNavigator();
            //CPQUtil.GetRuntimeDir() + "\\XAuthor Proposal Navigator.Exe";

            try
            {
                propId = DS.DataTable.Rows[0]["Id"] as string;

                if (string.IsNullOrEmpty(propId))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_Proposal_Execute_ErrorMsg"), resourceManager.GetResource("EditCartController_ExecuteCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
                cartId = DSL[1].DataTable.Rows[0]["Id"] as string;
                if (string.IsNullOrEmpty(cartId))
                {
                    ApttusMessageUtil.ShowError(resourceManager.GetResource("EditCartController_Execute_CartId_ErrorMsg"), resourceManager.GetResource("EditCartController_ExecuteCap_ErrorMsg"));
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
            }
            catch (Exception ex)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("COMMON_Proposal_Execute_ErrorMsg"), resourceManager.GetResource("COMMON_Proposal_Cap_ErrorMsg"));
                Result.Status = ActionResultStatus.Failure;
                return Result;
            }

            /* to navigate to cart page directly, cart id wont work , we need to grab the business object id instead
             *  SELECT Id, Apttus_Config2__BusinessObjectId__c FROM Apttus_Config2__ProductConfiguration__c 
             *  WHERE (Apttus_QPConfig__Proposald__c = 'a0ii000000GtCTJAA3' AND Apttus_Config2__Status__c = 'Finalized')
             */



            string queryString = null;
            StringBuilder sb = new StringBuilder("SELECT ");
            sb.Append("Apttus_Config2__BusinessObjectId__c");
            sb.Append(" FROM Apttus_Config2__ProductConfiguration__c");
            sb.Append(" WHERE Apttus_QPConfig__Proposald__c =").Append("'").Append(propId).Append("'");
            sb.Append(" AND Apttus_Config2__Status__c =  ").Append("'Finalized'");

            queryString = sb.ToString();
            DataTable DT = new DataTable();
            DS = objectManager.QueryDataSet(new SalesforceQuery { SOQL = queryString });
            string busObjectId = DS.DataTable.Rows[0]["Apttus_Config2__BusinessObjectId__c"] as string;



            CPQUtil CPQUtilNav = new CPQUtil();
            CPQUtilNav.CartId = busObjectId;
            CPQUtilNav.GetEditURL(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken, propId);
            string sURL = CPQUtilNav.GetEditURL(oAuthWrapper.InstanceURL, oAuthWrapper.AccessToken, propId);
            if (sURL != null)
            {
                var p = new System.Diagnostics.Process();
                // Get the current directory. 
                var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                //     string path = Directory.GetCurrentDirectory();
                p.StartInfo.FileName = path; // path + "\\" + "XAuthor Proposal Navigator.Exe"; // @"C:\Users\Refeekh Peermohammed\Documents\Visual Studio 2013\Projects\WindowsFormsApplication4\WindowsFormsApplication4\bin\Debug\WindowsFormsApplication4.exe";
                p.StartInfo.Arguments = sURL;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();

                string output = p.StandardOutput.ReadToEnd();

                string newVal = output.Replace("\r\n", string.Empty);
                string[] ReturnIDs = null;
                if (!string.IsNullOrEmpty(newVal))
                {
                    ReturnIDs = newVal.Split('|');
                }

            }
            //


            Result.Status = ActionResultStatus.Success;
            return Result;

        }


        protected bool IsInputValid(string[] inputName, string caption)
        {
            List<ApttusDataSet> DSL = DataManager.GetInstance.GetDatasetsByNames(InputDataName);
            if (DSL == null || DSL.Count < 0)
            {
                ApttusMessageUtil.ShowError(resourceManager.GetResource("EditCartController_IsInputValid_ErrorMsg"), caption);

                return false;
            }
            return true;

        }
    }


}
