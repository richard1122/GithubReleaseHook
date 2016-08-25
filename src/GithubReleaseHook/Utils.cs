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
            path = $"/{c}{path.Substring(2)}";
            return path;
        }

        public static string ConvertPathToCrossPlatform(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = path.Replace(@"\", "/");
                path = ConvertPathToCygwinStyle(path);
            }
            return path;
        }
    }
}
