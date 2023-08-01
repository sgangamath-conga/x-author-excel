/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core.Model.Actions.CPQAction;
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core.CPQ
{
    public class CPQBase
    {
        public enum CPQAction
        {
            
            CPQ_CREATECART,
            CPQ_CREATEEMPTYCART,
            CPQ_FINALIZECART,
            CPQ_APPLYPRICING,
            CPQ_UPDATECART,
            CPQ_CREATEPROPOSAL,
            CPQ_VIEWPROPOSAL,
            CPQ_EDITCART
        }
        public const string CPQID = "CPQ_";
        public static List<string> GetCPQModel(Action objAction)
        {
            List<string> lstObjects = new List<string>();
            string Id = objAction.Id;


            CPQ.CPQBase.CPQAction enumAction = (CPQ.CPQBase.CPQAction)Enum.Parse(typeof(CPQ.CPQBase.CPQAction), Id);
            if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATECART))
            {
                CreateCartModel model = objAction as CreateCartModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_APPLYPRICING))
            {
                ApplyPricingModel model = objAction as ApplyPricingModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_FINALIZECART))
            {
                FinalizeCartModel model = objAction as FinalizeCartModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATEEMPTYCART))
            {
                CreateEmptyCartModel model = objAction as CreateEmptyCartModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_UPDATECART))
            {
                UpdateCartModel model = objAction as UpdateCartModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATEPROPOSAL))
            {
                CreateProposalModel model = objAction as CreateProposalModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_VIEWPROPOSAL))
            {
                ViewProposalModel model = objAction as ViewProposalModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_EDITCART))
            {
                EditCartModel model = objAction as EditCartModel;
                if (model != null)
                {
                    lstObjects = model.GetInputObjects();
                }

            }
            return lstObjects;
        }
        public static Action GetCPQAction(string Id)
        {
            CPQ.CPQBase.CPQAction enumAction = (CPQ.CPQBase.CPQAction)Enum.Parse(typeof(CPQ.CPQBase.CPQAction), Id);

            if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATECART))
            {
                return new CreateCartModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_APPLYPRICING))
            {
                return new ApplyPricingModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_FINALIZECART))
            {
                return new FinalizeCartModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATEEMPTYCART))
            {
                return new CreateEmptyCartModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_UPDATECART))
            {
                return new UpdateCartModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_CREATEPROPOSAL))
            {
                return new CreateProposalModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_VIEWPROPOSAL))
            {
                return new ViewProposalModel();
            }
            else if (enumAction.Equals(CPQ.CPQBase.CPQAction.CPQ_EDITCART))
            {
                return new EditCartModel();
            }
            //CPQ_CREATEPROPOSAL
            return new CreateCartModel();
           

        }
        public static Dictionary<string, string> AddCPQActions(Dictionary<string, string> lstActions)
        {

            CreateCartModel mdl = new CreateCartModel();
            lstActions.Add(mdl.Id, mdl.Name + " (" + mdl.Type + ")");
            ApplyPricingModel applyPricingmdl = new ApplyPricingModel();
            lstActions.Add(applyPricingmdl.Id, applyPricingmdl.Name + " (" + applyPricingmdl.Type + ")");
            FinalizeCartModel finalizemdl = new FinalizeCartModel();
            lstActions.Add(finalizemdl.Id, finalizemdl.Name + " (" + finalizemdl.Type + ")");
            CreateEmptyCartModel emptyCart = new CreateEmptyCartModel();
            lstActions.Add(emptyCart.Id, emptyCart.Name + " (" + emptyCart.Type + ")");
            UpdateCartModel UdpateCart = new UpdateCartModel();
            lstActions.Add(UdpateCart.Id, UdpateCart.Name + " (" + UdpateCart.Type + ")");
            /*a4a actions */
            CreateProposalModel CreateProp = new CreateProposalModel();
            lstActions.Add(CreateProp.Id, CreateProp.Name + " (" + CreateProp.Type + ")");
            ViewProposalModel ViewProp = new ViewProposalModel();
            lstActions.Add(ViewProp.Id, ViewProp.Name + " (" + ViewProp.Type + ")");
            EditCartModel EditCart = new EditCartModel();
            lstActions.Add(EditCart.Id, EditCart.Name + " (" + EditCart.Type + ")");
            return lstActions;
            
        }
    }
}
