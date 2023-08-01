/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class ApplicationObject
    {
        public string TemplateName { get; set; }
        public byte[] Config { get; set; }
        public byte[] Template { get; set; }
        public byte[] Schema { get; set; }
    }
}
