/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apttus.XAuthor.Core.Model.Actions.CPQAction
{
    public class CreateCartModel : Action
    {
        protected const string OPP = "Opportunity";
        protected const string PRICELISTITEM = "Apttus_Config2__PriceListItem__c";
        protected const string CART = "Apttus_Config2__ProductConfiguration__c";
        protected const string LINEITEM = "Apttus_Config2__LineItem__c";
        protected const string PROPOSAL = "Apttus_Proposal__Proposal__c";
        protected ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public CreateCartModel()
        {

            Name = "Create Cart";
            Type = "CPQ";
            Id =  "CPQ_CREATECART";
        }
       

        public Guid AppObjectId
        {
            get;
            set;
        }
        public virtual Guid ReturnObject
        {
            get
            {
                List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
                ApttusObject Cart = appObjects.Find(item => item.Id.Equals(CART));
                return Cart.UniqueId;
            }
        }
        public virtual List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject opp = appObjects.Find(item => item.Id.Equals(OPP));
            ApttusObject PL = appObjects.Find(item => item.Id.Equals(PRICELISTITEM));
            if (opp == null)
            {
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CORECARTMODEL_InputObj_ErrorMsg"), OPP), resourceManager.GetResource("CORECARTMODEL_InputObjCap_ErrorMsg"));
                return null;
            }
            if (PL == null)
            {
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CORECARTMODEL_InputObj_ErrorMsg"),PRICELISTITEM), resourceManager.GetResource("CORECARTMODEL_InputObjCap_ErrorMsg"));
                return null;
            }
            objectList.Add(opp.UniqueId.ToString());
            objectList.Add(PL.UniqueId.ToString());

            return objectList.ToList();
        }

        public virtual List<string> GetCartInputs()
        {
            HashSet<string> objectList = new HashSet<string>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();

            
            ApttusObject LI = appObjects.Find(item => item.Id.Equals(CART));
            if (LI == null)
            {                
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CORECARTMODEL_InputObj_ErrorMsg"), CART), resourceManager.GetResource("CORECARTMODEL_InputObjCap_ErrorMsg"));
                return null;
            }
            objectList.Add(LI.UniqueId.ToString());


            return objectList.ToList();

        }
    }

    public class ApplyPricingModel : CreateCartModel
    {
        public ApplyPricingModel()
        {
            Name = "Apply Pricing";
            Type = "CPQ";
            Id = "CPQ_APPLYPRICING";

        }


        public override List<string> GetInputObjects()
        {
            return GetCartInputs();
            //HashSet<string> objectList = new HashSet<string>();
            //List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();

            //ApttusObject LI = appObjects.Find(item => item.Id.Equals(CART));
            //objectList.Add(LI.UniqueId.ToString());


            //return objectList.ToList();
        }
    }
    public class FinalizeCartModel : CreateCartModel
    {
        public FinalizeCartModel()
        {
            Name = "Finalize Cart";
            Type = "CPQ";
            Id = "CPQ_FINALIZECART";

        }

        public override List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();

            ApttusObject cart = appObjects.Find(item => item.Id.Equals(CART));
            if (cart == null)
            {
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CORECARTMODEL_InputObj_ErrorMsg"), CART), resourceManager.GetResource("CORECARTMODEL_InputObjCap_ErrorMsg"));
                return null;
            }
            objectList.Add(cart.UniqueId.ToString());
            return objectList.ToList();
        }

    }

    public class CreateEmptyCartModel:CreateCartModel
    {
        public CreateEmptyCartModel()
        {
            Name = "Create Empty  Cart";
            Type = "CPQ";
            Id = "CPQ_CREATEEMPTYCART";

        }

        public override List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject opp = appObjects.Find(item => item.Id.Equals(OPP));
            if (opp == null)
            {
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("CORECARTMODEL_InputObj_ErrorMsg"), OPP), resourceManager.GetResource("CORECARTMODEL_InputObjCap_ErrorMsg"));
                return null;
            }
            objectList.Add(opp.UniqueId.ToString());
          
            return objectList.ToList();
        }
    }

    public class UpdateCartModel : CreateCartModel
    {
        public UpdateCartModel()
        {
            Name = "Update Cart";
            Type = "CPQ";
            Id = "CPQ_UPDATECART";

        }

        public override List<string> GetInputObjects()
        {
            return GetCartInputs();
        }
    }

    public class CreateProposalModel : CreateCartModel
    {
        public CreateProposalModel()
        {
            Name = "Create Proposal";
            Type = "CPQ";
            Id = "CPQ_CREATEPROPOSAL";

        }

        public override List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();
            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject opp = appObjects.Find(item => item.Id.Equals(OPP));
            if (opp == null)
            {
                ApttusMessageUtil.ShowError("Required object " + OPP + " is missing. Please select it from Sales Force Objects UI", "Action flow Input");
                return null;
            }
            objectList.Add(opp.UniqueId.ToString());

            return objectList.ToList();
        }
    }
    /*a4a actions */
    /* VIEW THE PROPOSAL UI */
    public class ViewProposalModel : CreateCartModel
    {
        public ViewProposalModel()
        {
            Name = "View Proposal";
            Type = "CPQ";
            Id = "CPQ_VIEWPROPOSAL";

        }

        public override List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();

            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject prop = appObjects.Find(item => item.Id.Equals(PROPOSAL));
            if (prop == null)
            {
                ApttusMessageUtil.ShowError("Proposal Object is not selected . Please select it from Salesforce Objects UI", "Action flow Input");
                return null;
            }
            objectList.Add(prop.UniqueId.ToString());

            return objectList.ToList();

        }
    }
    /* VIEW CART UI  */
    public class EditCartModel : CreateCartModel
    {
        public EditCartModel()
        {
            Name = "Edit Cart";
            Type = "CPQ";
            Id = "CPQ_EDITCART";

        }

        public override List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();

            List<ApttusObject> appObjects = ApplicationDefinitionManager.GetInstance.GetAllObjects();
            ApttusObject prop = appObjects.Find(item => item.Id.Equals(PROPOSAL));
            if (prop == null)
            {
                ApttusMessageUtil.ShowError("Cart object is not selected. Please select it from Sales Force Objects UI", "Action flow Input");
                return null;
            }
            objectList.Add(prop.UniqueId.ToString());

            prop = appObjects.Find(item => item.Id.Equals(CART));
            if (prop == null)
            {
                ApttusMessageUtil.ShowError("Cart object is not selected. Please select it from Sales Force Objects UI", "Action flow Input");
                return null;
            }
            objectList.Add(prop.UniqueId.ToString());

            return objectList.ToList();

        }
    }

}
