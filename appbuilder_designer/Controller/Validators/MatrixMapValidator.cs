/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public class MatrixMapValidator : IValidator
    {
        private static MatrixMapValidator instance;
        private static object syncRoot = new Object();
        private static ConfigurationManager configManager;
        static ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public static MatrixMapValidator GetInstance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new MatrixMapValidator();
                        configManager = ConfigurationManager.GetInstance;
                    }
                }
                return instance;
            }
        }

        /// <summary>
        ///  Validate method
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        public List<ValidationResult> Validate<T>(T map)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<MatrixMap> maps = new List<MatrixMap>();

            if (map == null)
                maps = ConfigurationManager.GetInstance.MatrixMaps;
            else
                maps = new List<MatrixMap> { (map as MatrixMap) };

            foreach (MatrixMap mm in maps)
            {
                // 1. Validate Matrix
                ValidationResult resultRep = ValidateMatrix(mm);
                if (resultRep != null)
                    results.Add(resultRep);               
            }

            return results;
        }

        /// <summary>
        /// Validate Matrix
        /// </summary>
        /// <param name="mMap"></param>
        /// <returns></returns>
        internal static ValidationResult ValidateMatrix(MatrixMap mMap)
        {
            ValidationResult result = null;           

            // For DataFields 
            var results = (from mdata in mMap.MatrixData.MatrixDataFields
                           select new
                           {
                               TargetNamedRange = mdata.TargetNamedRange,
                               RenderingType = mdata.RenderingType,
                               LayoutType = LayoutType.Vertical,
                               FieldName = mdata.FieldName,
                               ObjectName = applicationDefinitionManager.GetAppObject(mdata.AppObjectUniqueID).Name
                              
                           }).ToList();

            // For Column fields
            results = results.Union((from mFieldColumn in mMap.MatrixColumn.MatrixFields
                                     select new
                                     {
                                         TargetNamedRange = mFieldColumn.TargetNamedRange,
                                         RenderingType = mFieldColumn.RenderingType,
                                         LayoutType = LayoutType.Horizontal,
                                         FieldName = mFieldColumn.FieldName,
                                         ObjectName = applicationDefinitionManager.GetAppObject(mFieldColumn.AppObjectUniqueID).Name
                                         
                                     })).ToList();

            // For Row fields
            results = results.Union((from mFieldRow in mMap.MatrixRow.MatrixFields
                                     select new
                                     {
                                         TargetNamedRange = mFieldRow.TargetNamedRange,
                                         RenderingType = mFieldRow.RenderingType,
                                         LayoutType = LayoutType.Vertical,
                                         FieldName = mFieldRow.FieldName,
                                         ObjectName = applicationDefinitionManager.GetAppObject(mFieldRow.AppObjectUniqueID).Name
                                        
                                     })).ToList();

            // For loop on final results and delete name range. in case of static , we will not remove contents of fields
            if (results != null)
            {
                results.ForEach((item) =>
                {                    
                        int TargetIndex = -1;
                        if (item.LayoutType == LayoutType.Vertical)
                            TargetIndex = ExcelHelper.GetColumnIndex(item.TargetNamedRange, item.TargetNamedRange);
                        else
                            TargetIndex = ExcelHelper.GetRowIndex(item.TargetNamedRange, item.TargetNamedRange);

                        if (TargetIndex < 0)
                        {
                            if (result == null)
                                result = new ValidationResult();
                            result.EntityName = mMap.Name;
                            result.EntityType = EntityType.MatrixMap;
                            String msg = new StringBuilder(100).Append("Field ").Append(item.FieldName).Append(" for ").Append(item.ObjectName)
                                                                .Append(" is either moved or deleted from Sheet, Please remove field from Matrix Map or add again.").ToString();
                            if (result.Messages == null)
                                result.Messages = new List<string>();
                            result.Messages.Add(msg);
                            result.Success = false;                            
                        }
                });

                if (result == null) //If successfull, then update the targetlocation for row, column & data
                {
                    //Individual fields
                    foreach (MatrixIndividualField field in mMap.IndependentFields)
                        field.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));

                    //Column Fields
                    foreach (MatrixField field in mMap.MatrixColumn.MatrixFields)
                        field.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));

                    //Row Fields
                    foreach (MatrixField field in mMap.MatrixRow.MatrixFields)
                        field.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));

                    //Data Fields
                    foreach (MatrixDataField field in mMap.MatrixData.MatrixDataFields)
                        field.TargetLocation = ExcelHelper.GetAddress(ExcelHelper.GetRange(field.TargetNamedRange));
                }
            }
            return result;
        }

        /// <summary>
        /// Check save field in matrix map
        /// </summary>
        /// <param name="mMap"></param>
        /// <returns></returns>
        private List<ValidationResult> IsSaveFieldInMatrixMap(MatrixMap mMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            results.AddRange(IsMatrixSaveFieldInMatrixMap(mMap));
            return results;
        }       

        /// <summary>
        /// Check Matrix save field in matrix map
        /// </summary>
        /// <param name="mMap"></param>
        /// <returns></returns>
        private List<ValidationResult> IsMatrixSaveFieldInMatrixMap(MatrixMap mMap)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            foreach (SaveMap saveMap in configManager.SaveMaps)
            {
                var matrixSaveFields = (from field in saveMap.SaveFields
                                        where field.SaveFieldType == SaveType.MatrixField && field.Type == ObjectType.Repeating && field.MatrixMapId == mMap.Id
                                        select field).ToList();

                //If matrixSaveFields is null which means this matrix map is not part of this save map.
                if (matrixSaveFields != null && matrixSaveFields.Count > 0)
                {
                    ValidationResult result = new ValidationResult();
                    result.Success = false;
                    result.EntityType = EntityType.MatrixMap;
                    result.EntityName = mMap.Name;

                    if (result.Messages == null)
                        result.Messages = new List<string>();

                    StringBuilder errorBuilder = new StringBuilder(mMap.Name).Append(resourceManager.GetResource("MATRIXMAPVALID_MatrixHas_Text"));
                    foreach (SaveField sf in matrixSaveFields)
                        errorBuilder.Append(sf.FieldId).Append(", ");

                    errorBuilder.Append(String.Format(resourceManager.GetResource("COMMON_PartOf_Msg"), saveMap.Name));
                    result.Messages.Add(errorBuilder.ToString());

                    results.Add(result);
                }
                
            }
            return results;
        }

        public List<ValidationResult> ValidateDelete<T>(T t)
        {
            MatrixMap matrixMap = t as MatrixMap;

            List<ValidationResult> results = new List<ValidationResult>();

            List<Core.RetrieveActionModel> retrieveActions = ValidationManager.GetMatrixActionsForMatrixMap(matrixMap);

            //1. Are there any save fields referring to this matirx map.
            results.AddRange(IsSaveFieldInMatrixMap(matrixMap));

            //2. There exists a retreive action which uses this matrix map.
            //Build Error Message and let the user know which retrieve action(s) are using this matrixMap.
            if (retrieveActions.Count != 0)
            {
                ValidationResult result = new ValidationResult();
                result.EntityName = matrixMap.Name;
                result.EntityType = EntityType.MatrixMap;
                result.Success = false;

                StringBuilder errorMsgBuilder = new StringBuilder(matrixMap.Name);
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_IsUsedBy_Msg"));

                bool bAddComma = false;

                foreach (Core.RetrieveActionModel retreiveAction in retrieveActions)
                {
                    if (bAddComma)
                        errorMsgBuilder.Append(", ");
                    errorMsgBuilder.Append(retreiveAction.Name);
                    bAddComma = true;
                }
                errorMsgBuilder.Append(resourceManager.GetResource("COMMON_RetrieveACTValidate_Msg"));

                result.Messages = new List<string>();
                result.Messages.Add(errorMsgBuilder.ToString());

                results.Add(result);
            }
            return results;            
        }      

        public ValidationResult ValidateAdd<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
