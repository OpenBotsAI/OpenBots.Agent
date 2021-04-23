using System;
using System.Runtime.InteropServices;

namespace OpenBots.Service.Client.Manager.Win32
{
    public static class USERENV32
    {
        #region Win32 API Imports

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        #endregion
    }
}
