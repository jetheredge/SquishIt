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
            //not sure its worth investigating issues on windows mono, as most problems we have seem file-system related
            get { return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX; }
        }

        public static string ResolveAppRelativePathToFileSystem(string file)
        {
            // Remove query string
            if (file.IndexOf('?') != -1)
            {
                file = file.Substring(0, file.IndexOf('?'));
            }

            return HttpContext.Current == null 
                ? ProcessWithoutHttpContext(file) 
                : HttpContext.Current.Server.MapPath(file);
        }

        static string ProcessWithoutHttpContext(string file)
        {
            file = Unix 
                ? file.TrimStart('~', '/') // does this need to stay this way to account for multiple leading slashes or can string trimstart be used?
                : file.Replace("/", "\\").TrimStart('~').TrimStart('\\');

            return Path.Combine(Environment.CurrentDirectory, file);
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
                return path.Substring(path.IndexOf("/", StringComparison.InvariantCulture) + 1);
            }
        }
    }
}