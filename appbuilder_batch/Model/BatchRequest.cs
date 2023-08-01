/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Apttus.XAuthor.Batch
{
    public class BatchRequest
    {
        public string TemplateName { get; set; }
        public byte[] Config { get; set; }
        public byte[] Template { get; set; }
        public string InputData { get; set; }
        public string OutputPath { get; set; }
        public string OutputType { get; set; }

    }
}
