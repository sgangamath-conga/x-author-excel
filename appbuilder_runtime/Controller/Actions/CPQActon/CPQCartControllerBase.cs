/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core.Model.Actions.CPQAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppRuntime.Controller.Actions.CPQAction
{
    public class CPQCartControllerBase
    {
        public static CartController GetCartController(Core.Action objAction, Core.WorkflowAction wfAction, string[] inputDataNames)
        {

            if (objAction.GetType() == typeof(CreateCartModel))
            {
                CartController cart = new CartController((CreateCartModel)objAction, inputDataNames);

                if (wfAction.WorkflowActionData.InputData)
                {
                    cart.InputData = wfAction.WorkflowActionData.InputData;
                    cart.InputDataName = inputDataNames;


                }

                if (wfAction.WorkflowActionData.OutputPersistData)
                {
                    cart.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                    cart.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                }

                return cart;

            }
            else if (objAction.GetType() == typeof(ApplyPricingModel))
            {
                ApplyPricingController cart = new ApplyPricingController((ApplyPricingModel)objAction, inputDataNames);

                if (wfAction.WorkflowActionData.InputData)
                {
                    cart.InputData = wfAction.WorkflowActionData.InputData;
                    cart.InputDataName = inputDataNames;


                }

                if (wfAction.WorkflowActionData.OutputPersistData)
                {
                    cart.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                    cart.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                }

                return cart;

            }
            else if (objAction.GetType() == typeof(FinalizeCartModel))
            {
                FinalizeCartController cart = new FinalizeCartController((FinalizeCartModel)objAction);

                if (wfAction.WorkflowActionData.InputData)
                {
                    cart.InputData = wfAction.WorkflowActionData.InputData;
                    cart.InputDataName = inputDataNames;


                }

                if (wfAction.WorkflowActionData.OutputPersistData)
                {
                    cart.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                    cart.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                }
                return cart;
                
            }
            else if (objAction.GetType() == typeof(CreateEmptyCartModel))
            {
                CreateEmptyCartController cart = new CreateEmptyCartController((CreateEmptyCartModel)objAction, inputDataNames);

                if (wfAction.WorkflowActionData.InputData)
                {
                    cart.InputData = wfAction.WorkflowActionData.InputData;
                    cart.InputDataName = inputDataNames;


                }

                if (wfAction.WorkflowActionData.OutputPersistData)
                {
                    cart.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                    cart.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
                }

                return cart;
            }
            else if (objAction.GetType() == typeof(UpdateCartModel))
            {
                UpdateCartController cart = new UpdateCartController((UpdateCartModel)objAction, inputDataNames);
                SetCartInputOutPut(cart, objAction,wfAction,inputDataNames);
                return cart;

            }
            else if (objAction.GetType() == typeof(CreateProposalModel))
            {
                CreateProposalController cart = new CreateProposalController((CreateProposalModel)objAction, inputDataNames);
                SetCartInputOutPut(cart, objAction, wfAction, inputDataNames);
                return cart;

            }
            else if (objAction.GetType() == typeof(ViewProposalModel))
            {
                ViewProposalController cart = new ViewProposalController((ViewProposalModel)objAction, inputDataNames);
                SetCartInputOutPut(cart, objAction, wfAction, inputDataNames);
                return cart;

            }
            else if (objAction.GetType() == typeof(EditCartModel))
            {
                EditCartController cart = new EditCartController((EditCartModel)objAction, inputDataNames);
                SetCartInputOutPut(cart, objAction, wfAction, inputDataNames);
                return cart;

            }
            return null;
        }

        private static void SetCartInputOutPut(CartController cart, Core.Action objAction, Core.WorkflowAction wfAction, string[] inputDataNames)
        {
            if (wfAction.WorkflowActionData.InputData)
            {
                cart.InputData = wfAction.WorkflowActionData.InputData;
                cart.InputDataName = inputDataNames;


            }

            if (wfAction.WorkflowActionData.OutputPersistData)
            {
                cart.OutputPersistData = wfAction.WorkflowActionData.OutputPersistData;
                cart.OutputDataName = wfAction.WorkflowActionData.OutputDataName;
            }

        }
    }
}
