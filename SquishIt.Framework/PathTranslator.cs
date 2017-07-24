using System;
using System.IO;
using System.Web;

namespace SquishIt.Framework
{
    public class PathTranslator : IPathTranslator
    {
        public string ResolveAppRelativePathToFileSystem(string file)
        {
            // Remove query string
            if (file.IndexOf('?') != -1)
            {
                file = file.Substring(0, file.IndexOf('?'));
            }
            
            return HttpContext.Current == null 
                ? ProcessWithoutHttpContext(file) 
                : HttpContext.Current.Server.MapPath(HttpRuntime.AppDomainAppVirtualPath + "/" + file.TrimStart("~/").TrimStart("~"));
        }

        //TODO: clean up this file in general but specifically this and ResolveAppRelativePathToFileSystem
        public string ExpandAppRelativePath(string file)
        {
            if (file.StartsWith("~/"))
            {
                string appRelativePath = HttpRuntime.AppDomainAppVirtualPath;
                if (appRelativePath != null && !appRelativePath.EndsWith("/"))
                    appRelativePath += "/";
                return file.Replace("~/", appRelativePath);
            }
            return file;
        }

        static string ProcessWithoutHttpContext(string file)
        {
            file = Platform.Unix 
                ? file.TrimStart('~', '/') // does this need to stay this way to account for multiple leading slashes or can string trimstart be used?
                : file.Replace("/", "\\").TrimStart('~').TrimStart('\\');

            return Path.Combine(Environment.CurrentDirectory, file);
        }

        public string ResolveFileSystemPathToAppRelative(string file)
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

        public string BuildAbsolutePath(string siteRelativePath)
        {
            if (HttpContext.Current == null)
                throw new InvalidOperationException(
                    "Absolute path can only be constructed in the presence of an HttpContext.");
            if (!siteRelativePath.StartsWith("/"))
                throw new InvalidOperationException("This helper method only works with site relative paths.");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;
            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port,
                                 VirtualPathUtility.ToAbsolute(siteRelativePath));
        }
    }
}
