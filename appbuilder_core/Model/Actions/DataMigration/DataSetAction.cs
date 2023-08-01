using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{
    public enum DataSetOperation 
    {
        OR,
        AND
    }

    public class DataSetActionModel : Action
    {
        public DataSetActionModel()
        {
            Type = Constants.DataSetAction;
        }
        public Guid TargetObject { get; set; }
        public DataSetOperation Operation { get; set; }

        public void Write()
        {
            ConfigurationManager.GetInstance.AddAction(this);
        }

        internal List<string> GetInputObjects()
        {
            return new List<string>( new string []{ TargetObject.ToString(), TargetObject.ToString() } );
        }
    }
}
