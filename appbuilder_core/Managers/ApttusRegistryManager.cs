/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using Microsoft.Win32;

namespace Apttus.XAuthor.Core
{
    public class ApttusRegistryManager
    {
        private static RegistryKey regParentKey;
        private static RegistryKey regSubKey;
        private static string GetValue;
        public static int IEMajorVersion = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyRoot"></param>
        /// <param name="KeyName"></param>
        /// <param name="SubKeyRef"></param>
        /// <returns></returns>
        public static string GetKeyValue(RegistryHive KeyRoot, string KeyName, string SubKeyRef)
        {
            switch (KeyRoot)
            {
                case RegistryHive.ClassesRoot:
                    {
                        regParentKey = Registry.ClassesRoot;
                        break;
                    }
                case RegistryHive.CurrentConfig:
                    {
                        regParentKey = Registry.CurrentConfig;
                        break;
                    }
                case RegistryHive.CurrentUser:
                    {
                        regParentKey = Registry.CurrentUser;
                        break;
                    }
                case RegistryHive.LocalMachine:
                    {
                        regParentKey = Registry.LocalMachine;
                        break;
                    }
                case RegistryHive.PerformanceData:
                    {
                        regParentKey = Registry.PerformanceData;
                        break;
                    }
                case RegistryHive.Users:
                    {
                        regParentKey = Registry.Users;
                        break;
                    }
            }

            try
            {
                regSubKey = regParentKey.OpenSubKey(KeyName);
                if (regSubKey != null)
                {
                    GetValue = "" + regSubKey.GetValue(SubKeyRef);
                }
                else
                {
                    GetValue = "";
                }
            }
            catch (Exception)
            {
                GetValue = "";
            }

            return GetValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyRoot"></param>
        /// <param name="KeyName"></param>
        /// <param name="SubKeyRef"></param>
        /// <param name="SubKeyValue"></param>
        public static void UpdateKeyValue(RegistryHive KeyRoot, string KeyName, string SubKeyRef, string SubKeyValue)
        {
            switch (KeyRoot)
            {
                case RegistryHive.ClassesRoot:
                    {
                        regParentKey = Registry.ClassesRoot;
                        break;
                    }
                case RegistryHive.CurrentConfig:
                    {
                        regParentKey = Registry.CurrentConfig;
                        break;
                    }
                case RegistryHive.CurrentUser:
                    {
                        regParentKey = Registry.CurrentUser;
                        break;
                    }
                case RegistryHive.LocalMachine:
                    {
                        regParentKey = Registry.LocalMachine;
                        break;
                    }
                case RegistryHive.PerformanceData:
                    {
                        regParentKey = Registry.PerformanceData;
                        break;
                    }
                case RegistryHive.Users:
                    {
                        regParentKey = Registry.Users;
                        break;
                    }
            }

            try
            {
                regSubKey = regParentKey.CreateSubKey(KeyName);
                regSubKey.SetValue(SubKeyRef, SubKeyValue);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyName"></param>
        public static void DeleteRegistryKey(string KeyName)
        {
            regParentKey = Registry.CurrentUser;
            regParentKey.DeleteSubKey(KeyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyName"></param>
        public static void DeleteRegistryKey(RegistryHive KeyRoot, string KeyName, string sUserName)
        {
            switch (KeyRoot)
            {
                case RegistryHive.ClassesRoot:
                    {
                        regParentKey = Registry.ClassesRoot;
                        break;
                    }
                case RegistryHive.CurrentConfig:
                    {
                        regParentKey = Registry.CurrentConfig;
                        break;
                    }
                case RegistryHive.CurrentUser:
                    {
                        regParentKey = Registry.CurrentUser;
                        break;
                    }
                case RegistryHive.LocalMachine:
                    {
                        regParentKey = Registry.LocalMachine;
                        break;
                    }
                case RegistryHive.PerformanceData:
                    {
                        regParentKey = Registry.PerformanceData;
                        break;
                    }
                case RegistryHive.Users:
                    {
                        regParentKey = Registry.Users;
                        break;
                    }
            }

            regSubKey = regParentKey.OpenSubKey(KeyName, true);
            try
            {
                regSubKey.DeleteSubKey(sUserName);
            }
            catch (Exception ex)
            {
               // ApttusMessageUtil.ShowError(ex, true, ex.Message, "Delete Registry Key");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyRoot"></param>
        /// <param name="KeyName"></param>
        /// <param name="sUserName"></param>
        /// <returns></returns>
        public static bool IsUserNameExists(RegistryHive KeyRoot, string KeyName, string sUserName)
        {
            bool bIsExist = false;

            string[] userNames = GetAvailableUserNames(KeyRoot, KeyName);

            for (int i = 0; i < userNames.Length; i++)
            {

                if (userNames[i].Equals(sUserName))
                {
                    bIsExist = true;
                    break;
                }
            }

            return bIsExist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyRoot"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public static string[] GetAvailableUserNames(RegistryHive KeyRoot, string KeyName)
        {
            string[] userNames = null;

            switch (KeyRoot)
            {
                case RegistryHive.ClassesRoot:
                    {
                        regParentKey = Registry.ClassesRoot;
                        break;
                    }
                case RegistryHive.CurrentConfig:
                    {
                        regParentKey = Registry.CurrentConfig;
                        break;
                    }
                case RegistryHive.CurrentUser:
                    {
                        regParentKey = Registry.CurrentUser;
                        break;
                    }
                case RegistryHive.LocalMachine:
                    {
                        regParentKey = Registry.LocalMachine;
                        break;
                    }
                case RegistryHive.PerformanceData:
                    {
                        regParentKey = Registry.PerformanceData;
                        break;
                    }
                case RegistryHive.Users:
                    {
                        regParentKey = Registry.Users;
                        break;
                    }
            }

            try
            {
                regSubKey = regParentKey.OpenSubKey(KeyName);

                userNames = regSubKey.GetSubKeyNames();
            }
            catch (Exception)
            {
            }

            return userNames;
        }

        // AB-3164. On 4th Feb 2018, Salesforce changed login script which started giving JS errors to most customer (5 known customers in a day).
        // This is the exact same code which is in X-Author Word which fixes the issue. X-Author Word and Excel code should be identical.
        // This is required only in Runtime (not Designer) as it is invoked on addin_startup event. Doesnt need to be fired from both addins.
        public static void CreateOrUpdateKeyForIECompatibility(bool bUserIE8Settings = false)
        {

            int IEMajorVersion = (new System.Windows.Forms.WebBrowser()).Version.Major;

            RegistryKey regKey = Registry.CurrentUser;
            string KeyName = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
            string SubKeyRef = "excel.exe";
            string SubKeyValue = string.Empty;

            switch (IEMajorVersion)
            {

                case 7:
                case 8:
                case 9:
                    SubKeyValue = "8888";
                    break;

                case 10:
                    SubKeyValue = "10001";
                    break;
                case 11:
                    SubKeyValue = "11001";
                    break;
                case 12:
                    SubKeyValue = "12001";
                    break;
                default:
                    SubKeyValue = "8888";
                    break;
            }

            try
            {
                regSubKey = regKey.CreateSubKey(KeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                regSubKey.SetValue(SubKeyRef, bUserIE8Settings ? SubKeyValue : "8888", RegistryValueKind.DWord);
            }
            catch (Exception)
            {
            }
        }
    }
}
