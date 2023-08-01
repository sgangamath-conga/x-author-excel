/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Runtime.InteropServices;
using System;
using System.Security;
using System.Security.Permissions;

namespace Apttus.XAuthor.Core
{
    // Based on http://support.microsoft.com/kb/830033

    [SuppressUnmanagedCodeSecurity]
    internal class ActivationContextHelper : IDisposable
    {
        // Private data
        private IntPtr cookie;
        private static ACTCTX enableThemingActivationContext;
        private static IntPtr hActCtx;
        private static bool contextCreationSucceeded = false;

        /// <summary>
        /// Constructor that creates Activation Context
        /// </summary>
        /// <param name="enable">Must be set to true to create Activation Context</param>
        public ActivationContextHelper(bool enable)
        {
            cookie = IntPtr.Zero;
            if (enable)
            {
                if (EnsureActivateContextCreated())
                {
                    if (!ActivateActCtx(hActCtx, out cookie))
                    {
                        // Be sure cookie always zero if activation failed
                        cookie = IntPtr.Zero;
                    }
                }
            }
        }

        /// <summary>
        /// Destructor/Dispose that deactivates Activation Context
        /// </summary>
        ~ActivationContextHelper()
        {
            Dispose(false);
        }

        /// <summary>
        /// Destructor/Dispose that deactivates Activation Context
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor/Dispose that deactivates Activation Context
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (cookie != IntPtr.Zero)
            {
                if (DeactivateActCtx(0, cookie))
                {
                    // deactivation succeeded...
                    cookie = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Create Activation Context
        /// </summary>
        /// <returns>True, if successful</returns>
        private bool EnsureActivateContextCreated()
        {
            lock (typeof(ActivationContextHelper))
            {
                if (!contextCreationSucceeded)
                {
                    string assemblyLoc = null;

                    FileIOPermission fiop = new FileIOPermission(PermissionState.None);
                    fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
                    fiop.Assert();
                    try
                    {
                        assemblyLoc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }

                    if (assemblyLoc != null)
                    {
                        enableThemingActivationContext = new ACTCTX();
                        enableThemingActivationContext.cbSize = Marshal.SizeOf(typeof(ACTCTX));
                        enableThemingActivationContext.lpSource = assemblyLoc;

                        enableThemingActivationContext.dwFlags = ACTCTX_FLAG_RESOURCE_NAME_VALID;
                        enableThemingActivationContext.lpResourceName = (IntPtr)0x2;

                        // Note this will fail gracefully
                        hActCtx = CreateActCtx(ref enableThemingActivationContext);
                        contextCreationSucceeded = (hActCtx != new IntPtr(-1));
                    }
                }

                // If we return false, we'll try again on the next call into
                // EnsureActivateContextCreated(), which is fine.
                return contextCreationSucceeded;
            }
        }

        // All the pinvoke goo...
        [DllImport("Kernel32.dll")]
        private extern static IntPtr CreateActCtx(ref ACTCTX actctx);
        [DllImport("Kernel32.dll")]
        private extern static bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);
        [DllImport("Kernel32.dll")]
        private extern static bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

        private const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;
        private const int ACTCTX_FLAG_RESOURCE_NAME_VALID = 0x008;

        private struct ACTCTX
        {
            public int cbSize;
            public uint dwFlags;
            public string lpSource;
            public ushort wProcessorArchitecture;
            public ushort wLangId;
            public string lpAssemblyDirectory;
            public IntPtr lpResourceName;
            public string lpApplicationName;
        }
    }
}