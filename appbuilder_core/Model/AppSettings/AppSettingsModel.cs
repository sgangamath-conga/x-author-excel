/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{

    [Serializable]
    public class SheetProtect :  ICloneable<SheetProtect>
    {
        public string SheetName { get; set; }
        public string Password { get; set; }

        public SheetProtect Clone()
        {
            return new SheetProtect
            {
                SheetName = this.SheetName,
                Password = this.Password
            };
        }
    } 

    [Serializable]
    public class AppSettings 
    {       
        public List<SheetProtect> SheetProtectSettings { get; set; }
        public bool DisableLocalSaveFile { get; set; }
        public bool DisablePrint { get; set; }
        public bool DisableRichTextEditing { get; set; }
        public bool IgnorePicklistValidation { get; set; }

        // Suppress Message Settings
        public bool SuppressNoOfRecords { get; set; }
        public bool SuppressDependent { get; set; }
        public bool SuppressSave { get; set; }
        public bool SuppressAllRecordsSaveSuccess { get; set; }

        public bool AutoSizeColumns { get; set; }
        public bool ShowFilters { get; set; }
        public int MaxColumnWidth { get; set; }

        public bool EnableRowHighlight { get; set; }

        public string RowErrorColor { get; set; }

        public decimal MaxAttachmentSize { get; set; }

        public AppSettings()
        {
            SheetProtectSettings = new List<SheetProtect>();
        }
       
    }

    
}
