/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace Apttus.XAuthor.AppRuntime
{
	public static class XmlAuthorUtil {

		/// <summary>
		/// Determine whether the string has any value
		/// </summary>
		/// <param name="str">string to inspect</param>
		/// <returns>true if has value, else false</returns>
		public static bool HasValue(string str) {
			if ((str != null) && (str.Length > 0)) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the requested field value from the provided field elements.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public static string GetFieldValue(string fieldName, XmlElement[] fields) {
			try {
				string fieldVal = "";
				if (fields != null) {
					for (int i = 0;i < fields.Length;i++) {
						if (fields[i] != null) {
							if (fields[i].LocalName.ToLower().Equals(fieldName.ToLower()) == true) {
								fieldVal = fields[i].InnerText;
								break;
							}
						}
					}
				}
				return fieldVal;
			} catch (Exception) {
				return null;
			}
		}

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static string SanitizeXmlString(string xml) {

            if (xml == null) {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml) {

                if (IsLegalXmlChar(c)) {
                    
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character) {
            return (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        /// <summary>
        /// Creates frontdoor URL for OAuth and returns navigate URL
        /// </summary>
        /// <param name="instanceURL"></param>
        /// <param name="sessionId"></param>
        /// <param name="sURL"></param>
        /// <returns></returns>
        public static string GetFrontDoorURL(string instanceURL, string sessionId, string sURL)
        {
            String urlString = instanceURL + "/secur/frontdoor.jsp?sid=" + System.Web.HttpUtility.UrlEncode(sessionId) + "&retURL=" + System.Web.HttpUtility.UrlEncode(sURL);
            return urlString;
        }
	}
}
