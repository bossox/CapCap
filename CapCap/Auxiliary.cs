using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapCap
{
    internal static class Auxiliary
    {
        public static string GetFileName(string fullname)
        {
            return fullname.Substring(fullname.LastIndexOf('\\') + 1);
        }

        public static string GetFolderName(string fullpath)
        {
            if (System.IO.Directory.GetDirectoryRoot(fullpath) != fullpath)
                fullpath = fullpath.Substring(fullpath.LastIndexOf('\\') + 1);
            return fullpath;
        }
    }
}
