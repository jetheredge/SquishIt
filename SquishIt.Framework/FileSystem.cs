using System;
using System.IO;
using System.Web;

namespace SquishIt.Framework
{
    public class FileSystem
    {
        public static bool Unix
        {
            //assuming this means mono, hoping to avoid a compiler directive / separate assemblies
            get { return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX; }
        }

        public static string ResolveAppRelativePathToFileSystem(string file)
        {
            // Remove query string
            if (file.IndexOf('?') != -1)
            {
                file = file.Substring(0, file.IndexOf('?'));
            }

            if (HttpContext.Current == null)
            {
                if (!(Unix))
                {
                    file = file.Replace("/", "\\").TrimStart('~').TrimStart('\\');
                    return @"C:\" + file.Replace("/", "\\");
                }
                file = file.TrimStart('~', '/');
                return Path.Combine(Environment.CurrentDirectory, file);
            }
            return HttpContext.Current.Server.MapPath(file);
        }
    }
}