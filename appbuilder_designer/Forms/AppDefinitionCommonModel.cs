using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apttus.XAuthor.AppDesigner.Forms.CrossTab
{
  public  abstract class AppDefinitionCommonModel
    {
       ObjectManager objectManager ; 
       public AppDefinitionCommonModel()
       {
           objectManager  = ObjectManager.GetInstance;
       }
       public virtual List<ApttusObject> GetAllStandardObjects()
       {
           return objectManager.GetAllStandardObjects();
       }
       
       public virtual  List<ApttusFieldDS> GetFieldsList(ApttusObject AppObject, List<ApttusField> allFields)
       {
           List<ApttusFieldDS> list = new List<ApttusFieldDS>();
           if (AppObject.Fields != null)
           {
               foreach (ApttusField field in allFields)
               {
                   ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype);
                   if (AppObject.Fields.Exists(a => a.Id == field.Id))
                       FieldDS.Included = true;
                   list.Add(FieldDS);
               }
               return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
           }
           else
           {
               foreach (ApttusField field in allFields)
               {
                   ApttusFieldDS FieldDS = new ApttusFieldDS(field.Id, field.Name, field.Datatype);
                   list.Add(FieldDS);
               }
               return list.OrderByDescending(x => x.Included).ThenBy(x => x.Name).ToList();
           }

       }
    }

   public class CrossTabAppDefModel : AppDefinitionCommonModel
   {
       public CrossTabAppDefModel() : base() { }
   }
}
