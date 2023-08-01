/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    static class NameValidationUtil
    {

        public static bool Contains(this string original, string value, StringComparison comparisionType)
        {
            return original.IndexOf(value, comparisionType) >= 0;
        }

        private static readonly char[] specialChrs =
   new char[] 
    { '#', '$', '(', ')', 
      '*', '+', '.', '?', 
      '[', '\\', '^', '{', '|', '~' ,'`', '/','<','>',';'};


        private static readonly char[] ExlspecialChrs =
  new char[] 
    { '#', '$', '(', ')', 
      '*', '+',  '?', 
      '[', '\\', '^', '{', '|' , ' ' };


        private static readonly char[] ExcelNameExcludeList = new char[]{ '?','.', '*','[',']','\\', '/','(',
           ')'
       
       };


        private static readonly char[] nameSpecialChrs = new char[] { '#', '$', '(', ')', '*', '+', '.', '?', 
            '[', ']','\\', '^', '{', '}','|', '!','-', 
            '@', '%', '&', '>', '<', '~', '`', '=', ',','"'};

        public static bool CheckExlSpecialChrs(string strTobeChecked)
        {
            if (String.IsNullOrEmpty(strTobeChecked))
            {
                return false;
            }
            return strTobeChecked.IndexOfAny(ExlspecialChrs) >= 0;
        }

        public static bool CheckSpecialChrs(string strTobeChecked)
        {
            if (String.IsNullOrEmpty(strTobeChecked))
            {
                return false;
            }
            return strTobeChecked.IndexOfAny(specialChrs) >= 0;
        }
        public static bool IsNumeric  (string val, System.Globalization.NumberStyles NumberStyle, string CultureName)
        {
            Double result;
            return Double.TryParse(val, NumberStyle, new System.Globalization.CultureInfo
                        (CultureName), out result);
        }
        public static bool IsSheetNameValid(string strTobeChecked)
        {
            bool IsValid = true;
            string msg = "";
            if (String.IsNullOrEmpty(strTobeChecked))
            {
                //TODO : add message
                IsValid = false;
            }
            if (strTobeChecked.Length > 31)
            {
                //TODO : add message
                IsValid = false;
            }
            if (strTobeChecked.IndexOfAny(ExcelNameExcludeList) >= 0)
            {
                //TODO : add message
                IsValid = false;
            }
            if (strTobeChecked.IndexOfAny(specialChrs) >= 0)
            {
                //TODO : add message
                IsValid = false;
            }

            if (!IsValid)
                ApttusMessageUtil.ShowError(msg, "");
            return IsValid;
        }

        public static bool CheckNameSpecialChrs(string strTobeChecked)
        {
            if (String.IsNullOrEmpty(strTobeChecked))
            {
                return false;
            }
            return strTobeChecked.IndexOfAny(nameSpecialChrs) >= 0;
        }

        public static string WrapSpecialChrString(string source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return null;
            }

            if (CheckSpecialChrs(source))
            {
                return "[" + source + "]";
            }
            return source;

        }

        public static bool ValidateSheetName(string name)
        {

            return IsSheetNameValid(name);


        }
        public static bool ValidateName(string name)
        {
            if (CheckNameSpecialChrs(name))
            {
                return false;
            }
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            if (name.StartsWith(" "))
            {
                return false;
            }
            return true;
        }

        public static bool ValidateFileName(string name)
        {
            return name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0;
        }

       
    }
}
