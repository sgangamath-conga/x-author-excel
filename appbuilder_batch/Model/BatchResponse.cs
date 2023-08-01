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
    public class BatchResponse
    {
        public bool Success { get; set; }
        public byte[] OutputExcelFile { get; set; }
        public string OutputExcelFileName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
