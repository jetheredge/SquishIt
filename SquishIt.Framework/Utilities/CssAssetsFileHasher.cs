using System;
using System.IO;
using System.Linq;
using System.Web;
using SquishIt.Framework.Resolvers;

namespace SquishIt.Framework.Utilities
{
    public class CssAssetsFileHasher : ICssAssetsFileHasher
    {
        protected readonly string HashQueryStringKeyName;
        protected readonly IResolver FileResolver;
        protected readonly IHasher Hasher;

        public CssAssetsFileHasher(string hashQueryStringKeyName, IResolver fileResolver, IHasher hasher)
        {
            HashQueryStringKeyName = hashQueryStringKeyName;
            FileResolver = fileResolver;
            Hasher = hasher;
        }

        public string AppendFileHash(string cssFilePath, string url)
        {
            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                return url;
            }

            var assetFilePath = GetAssetFilePath(cssFilePath, url);
            var fileInfo = new FileInfo(assetFilePath);

            if (!fileInfo.Exists)
            {
                return url;
            }

            var hash = Hasher.GetHash(fileInfo);

            url = AppendQueryStringPairValue(url, HashQueryStringKeyName, hash);

            return url;
        }

        private string GetAssetFilePath(string cssFilePath, string url)
        {
            var queryStringPosition = url.IndexOf('?');

            if (queryStringPosition > -1)
            {
                url = url.Substring(0, queryStringPosition);
            }

            var resolvedUrl = string.Empty;

            var urlUri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!urlUri.IsAbsoluteUri)
            {
                if (!url.StartsWith("/"))
                {
                    var resolvedPath = Path.GetDirectoryName(cssFilePath);
                    var outputUri = new Uri(resolvedPath + "/", UriKind.Absolute);

                    var resolvedSourcePath = new Uri(outputUri, urlUri);
                    resolvedUrl = resolvedSourcePath.LocalPath;
                }
                else
                {
                    resolvedUrl = ResolveAppRelativePathToFileSystem(url);
                }

                return FileResolver.TryResolve(resolvedUrl).ToList()[0];
            }

            return urlUri.LocalPath;
        }

        private string ResolveAppRelativePathToFileSystem(string file)
        {
            if (HttpContext.Current == null)
            {
                file = file.Replace("/", "\\").TrimStart('~').TrimStart('\\');
                return @"C:\" + file.Replace("/", "\\");
            }
            return HttpContext.Current.Server.MapPath(file);
        }

        /// <summary>
        /// Append a query string pair value to a url
        /// </summary>
        /// <param name="url">The url to add query string pair value value to.</param>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The value to use.</param>
        /// <returns></returns>
        private static string AppendQueryStringPairValue(string url, string key, string value)
        {
            var path = url;
            var queryString = string.Empty;

            var queryStringPosition = url.IndexOf('?');

            if (queryStringPosition > -1)
            {
                path = url.Substring(0, queryStringPosition);
                queryString = url.Substring(queryStringPosition);
            }

            var querystring = HttpUtility.ParseQueryString(queryString);

            querystring.Add(key, value);

            var querystringwithAppendedValue = querystring.ToString();
            if (!string.IsNullOrEmpty(querystringwithAppendedValue))
            {
                querystringwithAppendedValue = "?" + querystringwithAppendedValue;
            }

            return path + querystringwithAppendedValue;
        }
    }
}
