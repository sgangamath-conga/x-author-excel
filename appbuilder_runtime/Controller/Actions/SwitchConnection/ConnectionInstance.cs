/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apttus.XAuthor.Core;
using Apttus.OAuthLoginControl;

namespace Apttus.XAuthor.AppRuntime
{
    public class ConnectionInstance
    {
        public string SwitchConnectionActionId { get; set; }
        public ApttusUserInfo UserInfo { get; set; }
        public ApttusOAuth OAuthSession { get; set; }
    }
}
