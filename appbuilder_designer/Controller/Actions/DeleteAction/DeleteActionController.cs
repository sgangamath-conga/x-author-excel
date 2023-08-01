using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class DeleteActionController
    {
        private DeleteActionModel model;
        private DeleteActionView view;

        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ObjectManager objectManager = ObjectManager.GetInstance;

        public FormOpenMode FormOpenMode { get; set; }

        public DeleteActionController(DeleteActionModel model, DeleteActionView view, FormOpenMode formOpenMode)
        {
            this.model = model;
            this.view = view;
            this.FormOpenMode = formOpenMode;
            if (view != null)
            {
                this.view.SetController(this);
                this.view.ShowDialog();
            }
        }

        public void SetView()
        {
            switch (FormOpenMode)
            {
                case FormOpenMode.Edit:
                    view.FillForm(model);
                    break;                
            }
        }

        public List<SearchFilterGroup> GetFilterGroup()
        {
            return model.DeleteFilterGroups;
        }

        public Dictionary<Guid,string> getMaps()
        {
            Dictionary<Guid, string> allMaps = new Dictionary<Guid, string>();            

            List<SaveMap> saveMaps = (from saveMap in configurationManager.SaveMaps
                     from sg in saveMap.SaveGroups
                     from rm in ConfigurationManager.GetInstance.RetrieveMaps
                     from rg in rm.RepeatingGroups
                     where rg.TargetNamedRange == sg.TargetNamedRange && rg.AppObject == sg.AppObject
                     && saveMap.SaveFields.Where(sf => sf.GroupId == sg.GroupId).Any(sf => sf.SaveFieldType == SaveType.RetrievedField)
                     select saveMap).Distinct().ToList();

            saveMaps.ForEach(sm => allMaps.Add(sm.Id, sm.Name + " (Save Map)"));

            return allMaps;
        }

        internal void Save(string actionName,Guid MapId, Guid saveGroupId, Guid ObjectId,  List<SearchFilterGroup> deleteFilters, bool bPromptConfirmationMessage)
        {
            model.Type = Constants.DELETE_ACTION;
            model.Name = actionName;
            model.MapId = MapId;
            model.SaveGroupId = saveGroupId;
            model.AppObjectUniqueId = ObjectId;
            model.DeleteFilterGroups = deleteFilters;
            model.PromptConfirmationDialog = bPromptConfirmationMessage;

            if (FormOpenMode == FormOpenMode.Create)
                model.Write();
        }
    }
}
