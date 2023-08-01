using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Apttus.OAuthLoginControl.Modules
{
    public class ApttusRegistryManager
    {
        private static RegistryKey regParentKey;
        private static RegistryKey regSubKey;
        private static RegistryKey InitializeRegParentKey;
        private static RegistryKey InitializeRegSubKey;
        private static string GetValue;

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
                case RegistryHive.DynData:
                    {
                        regParentKey = Registry.DynData;
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
            catch (Exception ex)
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
                case RegistryHive.DynData:
                    {
                        regParentKey = Registry.DynData;
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
            catch (Exception ex)
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
                case RegistryHive.DynData:
                    {
                        regParentKey = Registry.DynData;
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
                //ApttusObjectManager.HandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyRoot"></param>
        /// <param name="KeyName"></param>
        public static void CreateOrUpdateUserNameKey(RegistryHive KeyRoot, string KeyName, string sUserName, bool bUseSecurityToken, string sServerHost, string sSecurityToken)
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
                case RegistryHive.DynData:
                    {
                        regParentKey = Registry.DynData;
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

                if (regSubKey == null)
                {
                    regSubKey = regParentKey.CreateSubKey(KeyName);
                }

                regSubKey.CreateSubKey(sUserName);
            }
            catch (Exception ex)
            {
                //ApttusObjectManager.HandleError(ex);
            }

            UpdateKeyValue(KeyRoot, KeyName + "\\" + sUserName, ApttusGlobals.ServerHostKey, sServerHost);
            UpdateKeyValue(KeyRoot, KeyName + "\\" + sUserName, ApttusGlobals.OAuthTokenKey, sSecurityToken);
            //UpdateKeyValue(KeyRoot, KeyName + "\\" + sUserName, ApttusGlobals.UseSecurityTokenKey, bUseSecurityToken.ToString());
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
                case RegistryHive.DynData:
                    {
                        regParentKey = Registry.DynData;
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
            catch (Exception ex)
            {
            }

            return userNames;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CreateOrUpdateKeyForIECompatibility()
        {

            RegistryKey regKey = Registry.CurrentUser;
            string KeyName = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
            string SubKeyRef = "winword.exe";
            string SubKeyValue = "8888";

            try
            {
                regSubKey = regKey.CreateSubKey(KeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                regSubKey.SetValue(SubKeyRef, SubKeyValue, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
