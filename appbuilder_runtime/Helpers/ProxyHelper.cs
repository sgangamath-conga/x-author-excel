/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Net;

using Apttus.XAuthor.AppRuntime.Modules;

namespace Apttus.XAuthor.AppRuntime.Helpers
{
    class ProxyHelper
    {
        static IWebProxy proxyObject = null;
        static bool isProxyByPassed = false;

        public static IWebProxy GetProxyObject(string destination)
        {
            IWebProxy proxyObject = null;

            IWebProxy webProxyObject = WebRequest.GetSystemWebProxy();
            Uri sProxyURI = webProxyObject.GetProxy(new Uri(destination));

            if (!webProxyObject.IsBypassed(new Uri(destination)))
            {

                proxyObject = new WebProxy(sProxyURI.OriginalString);
                proxyObject.Credentials = CredentialCache.DefaultCredentials;
            }

            return proxyObject;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResetProxy()
        {
            proxyObject = null;
            isProxyByPassed = false;
            ApttusCommandBarManager.GetInstance().ResetProxy();
        }
    }
}
