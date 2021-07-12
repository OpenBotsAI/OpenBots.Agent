using System.Runtime.InteropServices;
using System.Text;

namespace OpenBots.Service.Client.Manager.Win32
{
    public static class SHLWAPI
    {
        #region Win32 API Imports

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PathFindOnPath([In, Out] StringBuilder pszFile, [In] string[] ppszOtherDirs);

        #endregion
    }
}
