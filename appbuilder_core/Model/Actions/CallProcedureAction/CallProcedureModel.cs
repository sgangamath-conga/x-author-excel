/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Apttus.XAuthor.Core
{
    [Serializable]
    public class CallProcedureModel : Action
    {
        public CallProcedureModel()
        {
            MethodParams = new List<MethodParam>();
        }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public bool EnableCache { get; set; }
        public List<MethodParam> MethodParams { get; set; }

        public Guid ReturnObject { get; set; }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);

        }

        public List<string> GetInputObjects()
        {
            HashSet<string> objectList = new HashSet<string>();

            if (MethodParams != null)
            {
                foreach (MethodParam mParam in this.MethodParams)
                {
                    if (mParam.Type == MethodParam.ParamType.Object || mParam.Type == MethodParam.ParamType.Field)
                        objectList.Add(ApplicationDefinitionManager.GetInstance.GetAppObject(Guid.Parse(mParam.ParamObject)).UniqueId.ToString());
                }
            }

            return objectList.ToList();
        }

        public bool ShouldSerializeReturnObject()
        {
            return ReturnObject != Guid.Empty;
        }
    }

    [Serializable]
    public class MethodParam
    {
        public enum ParamType { Static, Object, Field , UserInput};

        public MethodParam()
        { 
        }

        [XmlElement("ParamName")]
        public string ParamName { get; set; }

        [XmlElement("ParamType")]
        public ParamType Type { get; set; }

        [XmlElement("ParamObject")]
        public string ParamObject { get; set; }

        [XmlElement("ParamField")]
        public string ParamField { get; set; }

        [XmlElement("ParamValue")]
        public string ParamValue { get; set; }

        [XmlElement("ChunkParam")]
        public bool ChunkParam { get; set; }

        [XmlElement("ChunkParamBy")]
        public int ChunkParamBy { get; set; }
    }
}
