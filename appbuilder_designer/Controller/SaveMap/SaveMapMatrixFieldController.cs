/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Apttus.XAuthor.AppDesigner
{
    public class SaveMapMatrixFieldController
    {
        public List<SaveMapMatrixField> Model { get; set; }
        private SaveMapMatrixFieldView View;
        private SaveMap CurrentSaveMap;
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SaveMapMatrixFieldController(SaveMap saveMap, SaveMapMatrixFieldView view)
        {
            CurrentSaveMap = saveMap;
            if (view != null)
            {
                // Set the view
                this.View = view;

                this.View.SetController(this);
            }
        }

        public void SetView(SaveMap saveMapModel)
        {
          
            //1.1 Get list of Matrix Maps
            List<SaveMapRetrieveMap> matrixMaps = (from m in configurationManager.MatrixMaps
                                                   select new SaveMapRetrieveMap
                                                   {
                                                       RetrieveMapId = m.Id,
                                                       RetrieveMapName = m.Name
                                                   }).ToList();
            matrixMaps.Add(new SaveMapRetrieveMap { RetrieveMapId = Guid.NewGuid(), RetrieveMapName = resourceManager.GetResource("ShowAll_Msg") });

            //2. Bind Filter by dropdown
            View.BindFilterBy(matrixMaps);

            //3. Get model based on dropdown selection
            View.SetInitialMatrixMap();

        }
        /// <summary>
        /// Select of Matrix Map
        /// </summary>
        /// <param name="selection"></param>
        // SaveMapRetrieveMap is use for display all map in dropdown
        public void MatrixMapSelectionChange(SaveMapRetrieveMap selection)
        {
            if (selection.RetrieveMapName == resourceManager.GetResource("ShowAll_Msg"))
            {
                List<SaveMapMatrixField> results = new List<SaveMapMatrixField>();
                foreach (MatrixMap rMap in configurationManager.MatrixMaps)
                {
                    results.AddRange(GenerateModel(CurrentSaveMap, rMap.Id));
                }
                Model = results;
            }
            else
            {
                Model = GenerateModel(CurrentSaveMap, selection.RetrieveMapId);
            }

            // Bind data grid with model.
            if (View != null)
                View.BindGrid(Model);
        }

        /// <summary>
        /// Generate Model
        /// </summary>
        /// <param name="SaveMap"></param>
        /// <param name="RetrieveMapId"></param>
        /// <returns></returns>
        private List<SaveMapMatrixField> GenerateModel(SaveMap SaveMap, Guid RetrieveMapId)
        {
            MatrixMap mMap = configurationManager.MatrixMaps.SingleOrDefault(m => m.Id == RetrieveMapId);
            List<SaveMapMatrixField> results = new List<SaveMapMatrixField>();
            if (mMap != null)
            {  
                results = (from matrixMap in configurationManager.MatrixMaps.Where(map => map.Id.Equals(RetrieveMapId))
                           from mDataField in matrixMap.MatrixData.MatrixDataFields.GroupBy(df => df.MatrixComponentId).Select(g => g.First()).ToList()
                           where !(from sf in SaveMap.SaveFields
                                   where sf.MatrixComponentId == mDataField.MatrixComponentId select sf.MatrixComponentId).Contains(mDataField.MatrixComponentId)                                 
                                        select new SaveMapMatrixField
                                        {
                                            Included = false,
                                            Type = ObjectType.Repeating,
                                            AppObjectUniqueId = mDataField.AppObjectUniqueID,
                                            MatrixObjectName = applicationDefinitionManager.GetAppObject(mDataField.AppObjectUniqueID).Name,  
                                            MatrixComponentId = mDataField.MatrixComponentId,
                                            MatrixComponentName = mDataField.Name ,
                                            MatrixMapId = matrixMap.Id,
                                            MatrixMapName = matrixMap.Name,
                                            TargetNamedRange = mDataField.TargetNamedRange
                                        }).ToList();
            }
            return results;
        }

        /// <summary>
        /// Add Matrix fields to save map
        /// </summary>
        internal void AddMatrixFieldsToSaveMap()
        {            

            List<SaveField> matrixFields = (from f in Model                                            
                                            where f.Included == true && f.Type == ObjectType.Repeating                                        
                                            select new SaveField
                                            {
                                                AppObject = f.AppObjectUniqueId,
                                                FieldId = f.MatrixComponentName ,
                                                Type = f.Type,
                                                SaveCondition = null,
                                                // In case of Matrix save, we are assign here name of matrix in designer location
                                                DesignerLocation = f.MatrixMapName,
                                                TargetNamedRange = f.TargetNamedRange,
                                                TargetColumnIndex = 0,
                                                SaveFieldType = SaveType.MatrixField,
                                                MatrixMapId = f.MatrixMapId,
                                                MatrixComponentId = f.MatrixComponentId,    
                                                SaveFieldName = f.MatrixComponentName,
                                                MultiLevelFieldId = string.Empty
                                            }).ToList();

            CurrentSaveMap.SaveFields.AddRange(matrixFields);           
            
            View.Dispose();
        }

        /// <summary>
        /// Toggle select 
        /// </summary>
        /// <param name="SelectAll"></param>
        internal void ToggleSelectAll(bool SelectAll)
        {
            if (SelectAll)
                Model.FindAll(f => f.Included = true).Where(f => f.Included == false);
            else
                Model.FindAll(f => f.Included = false).Where(f => f.Included == true);

            // Bind data grid with model.
            View.BindGrid(Model);
        }

        /// <summary>
        /// close view
        /// </summary>
        internal void Close()
        {
            View.Dispose();
        }
    }
}
