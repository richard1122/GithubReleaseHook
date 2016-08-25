using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GithubReleaseHook
{
    public class Utils
    {
        public static string ConvertPathToCygwinStyle(string path)
        {
            var c = char.ToLower(path[0]);
            path = $"/mnt/{c}{path.Substring(2)}";
            return path;
        }

        public static string ConvertPathToCrossPlatform(string path)
        {
            path = path.Replace(@"\", "/");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = ConvertPathToCygwinStyle(path);
            }
            return path;
        }
    }
}
