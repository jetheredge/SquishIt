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
                }
                else
                {
                    file = file.TrimStart('~', '/');
                }
                return Path.Combine(Environment.CurrentDirectory, file);
            }
            return HttpContext.Current.Server.MapPath(file);
        }

        public static string ResolveFileSystemPathToAppRelative(string file)
        {
            if (HttpContext.Current != null)
            {
                var root = new Uri(HttpRuntime.AppDomainAppPath);
                return root.MakeRelativeUri(new Uri(file, UriKind.RelativeOrAbsolute)).ToString();
            }
            else
            {
                var root = new Uri(Environment.CurrentDirectory);
                var path = root.MakeRelativeUri(new Uri(file, UriKind.RelativeOrAbsolute)).ToString();
                return path.Substring(path.IndexOf("/") + 1);
            }
        }
    }
}