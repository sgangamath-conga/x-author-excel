/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.IO;


namespace Apttus.XAuthor.Core
{
    public class EncryptionHelper
   {
        #region Constant

        static readonly string PasswordHash = "";
        static readonly string SaltKey = "";
        //static readonly string VIKey = "";
        const string XAUTHOROPEN = "<XAuthor>";
        const string XAUTHORCLOSING = "</XAuthor>";
        const string APPLICATION = "Application";

       #endregion
        static ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        /// <summary>
        /// Encrypt XML
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>    
        public static string Encrypt(string plainText, string elementEncrypt = APPLICATION)
        {
            // Create an XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(plainText);
            
            string ElementToEncrypt = elementEncrypt;
            // Check the arguments. 
            if (xmlDoc == null)
                throw new ArgumentNullException(resourceManager.GetResource("COREENCRYPTIONHELP_Doc_Text"));           
            XmlElement elementToEncrypt = xmlDoc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;       

            // Throw an XmlException if the element was not found. 
            if (elementToEncrypt == null)
            {
                throw new XmlException(resourceManager.GetResource("COREENCRYPTHELP_Encrypt_ErrMsg"));

            }
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(Properties.Settings.Default.Password, Encoding.ASCII.GetBytes(Properties.Settings.Default.Salt)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(Properties.Settings.Default.EncryptorKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            xmlDoc.LoadXml(XAUTHOROPEN + Convert.ToBase64String(cipherTextBytes) + XAUTHORCLOSING);
            return xmlDoc.OuterXml;
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string elementDecrypt = APPLICATION)
        {            
            // Create an XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(encryptedText);

            // Check for Existing Application
            XmlElement elementToEncrypt = xmlDoc.GetElementsByTagName(elementDecrypt)[0] as XmlElement;
            encryptedText = encryptedText.Replace(XAUTHOROPEN, "");
            encryptedText = encryptedText.Replace(XAUTHORCLOSING, "");
            if (elementToEncrypt == null)
            {

                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(Properties.Settings.Default.Password, Encoding.ASCII.GetBytes(Properties.Settings.Default.Salt)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(Properties.Settings.Default.EncryptorKey));
                var memoryStream = new MemoryStream(cipherTextBytes);
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            else
            {
                return encryptedText;
            }
        }
    }
}
