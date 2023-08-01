using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Apttus.OAuthLoginControl.Helpers
{
    public class StringCipher
    {
        public static string encryptString(string strText)
        {
            //return Encrypt(strText, "!#$a54?3");
            return Encrypt(strText, Convert.FromBase64String(Properties.Settings.Default.app_Data));
        }
        public static string decryptString(string strText)
        {
            //return Decrypt(strText, "!#$a54?3");
            return Decrypt(strText, Convert.FromBase64String(Properties.Settings.Default.app_Data));
        }
        private static string Encrypt(string stringToEncrypt, byte[] key)
        {
            byte[] inputByteArray = new byte[stringToEncrypt.Length];
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                byte[] IV = Convert.FromBase64String(Properties.Settings.Default.app_Value);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        private static string Decrypt(string stringToDecrypt, byte[] key)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                byte[] IV = Convert.FromBase64String(Properties.Settings.Default.app_Value);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();

                        Encoding encoding = Encoding.UTF8;
                        return encoding.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
