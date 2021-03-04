using System.Runtime.InteropServices;
using System.Text;

namespace OpenBots.Agent.Core.Utilities
{
    public static class ExternalMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = false)]
        public static extern bool PathFindOnPath([In, Out] StringBuilder pszFile, [In] string[] ppszOtherDirs);
    }
}
