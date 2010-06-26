using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SquishIt.Framework.Utilities;

namespace SquishIt.Framework.Css
{
    public class CssPathRewriter
    {

        public static string RewriteCssPaths(string outputPath, string sourcePath, string css)
        {
            var sourceUri = new Uri(Path.GetDirectoryName(sourcePath) + "/", UriKind.Absolute);
            var outputUri = new Uri(Path.GetDirectoryName(outputPath) + "/", UriKind.Absolute);

            var relativePaths = FindDistinctRelativePathsIn(css);

            foreach (string relativePath in  relativePaths)
            {
                if (!relativePath.StartsWith("/"))
                {
                    var resolvedSourcePath = new Uri(sourceUri + relativePath );
                    var resolvedOutput = outputUri.MakeRelativeUri(resolvedSourcePath);
                    
                    css = css.Replace(relativePath , resolvedOutput.OriginalString);    
                }
            }
            return css;
        }
        
        private static IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = Regex.Matches(css, @"url\(""{0,1}(.+?)""{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("/"))
                {
                    if (matchesHash.Add(path))
                    {
                        yield return path;
                    }
                }

            }
        }
    }
}