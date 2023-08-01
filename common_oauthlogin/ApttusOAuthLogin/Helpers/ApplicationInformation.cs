using System;
using System.Collections.Generic;
using System.Drawing;
using Apttus.OAuthLoginControl.Modules;
using System.Resources;

namespace Apttus.OAuthLoginControl.Helpers
{
    public class ApplicationInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApplicationName
        {

            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ApttusGlobals.Application ApplicationType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Bitmap ApplicationLogo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Icon ApplicationIcon
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationRegistryBase
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SaveInRegistry
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseResources
        {
            get;
            set;
        }

        /// <summary>
        /// Resource Manager
        /// </summary>
        public ResourceManager ResourceManager
        {
            get;
            set;
        }

    }
}
